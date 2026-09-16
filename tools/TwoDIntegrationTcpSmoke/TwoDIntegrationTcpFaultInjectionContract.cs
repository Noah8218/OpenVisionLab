#nullable enable

using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;
using OpenVisionLab.Integration.Transport.Tcp;

internal static class TwoDIntegrationTcpFaultInjectionContract
{
    private const string ProducerCommit = "1111111111111111111111111111111111111111";
    private static readonly byte[] WireMagic = "OVLTCP01"u8.ToArray();
    private static readonly JsonSerializerOptions WireJsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Default,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static async Task<int> RunAsync(
        string evidenceRoot,
        string sourceImagePath,
        string recipePath,
        string runtimeBuildManifestPath)
    {
        string root = Path.GetFullPath(evidenceRoot);
        string image = Path.GetFullPath(sourceImagePath);
        string recipe = Path.GetFullPath(recipePath);
        string runtimeManifestPath = Path.GetFullPath(runtimeBuildManifestPath);
        Require(File.Exists(image), $"Source image was not found: {image}");
        Require(File.Exists(recipe), $"Recipe was not found: {recipe}");
        Require(
            File.Exists(runtimeManifestPath),
            $"Runtime build manifest was not found: {runtimeManifestPath}");

        Directory.CreateDirectory(root);
        string runRoot = Path.Combine(
            root,
            $"two-d-tcp-fault-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runRoot);

        var runtimeManifest = IntegrationContractJson.DeserializeRuntimeBuildManifest(
            File.ReadAllBytes(runtimeManifestPath));
        string cleanManifestPath = Path.Combine(
            runRoot,
            "openvisionlab.runtime.clean.json");
        File.WriteAllBytes(
            cleanManifestPath,
            IntegrationContractJson.SerializeCanonical(
                runtimeManifest with
                {
                    Identity = runtimeManifest.Identity with
                    {
                        SourceState = IntegrationSourceState.Clean
                    }
                }));
        var consumer = TwoDIntegrationBuildIdentity.LoadQualifiedIdentity(cleanManifestPath);
        byte[] sharedKey = RandomNumberGenerator.GetBytes(32);

        try
        {
            var options = new TcpIntegrationOptions
            {
                MaxAttempts = 2,
                RetryDelay = TimeSpan.FromMilliseconds(20),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                IdleTimeout = TimeSpan.FromSeconds(30)
            };

            string pushSourceRoot = Path.Combine(runRoot, "push-source");
            string pushPeerRoot = Path.Combine(runRoot, "push-peer");
            var pushHandoff = CreateTransaction(
                pushSourceRoot,
                image,
                recipe,
                consumer,
                includeCompletedSnapshot: false,
                includePadding: true);
            Dictionary<string, string> pushSourceTree = ComputeTree(
                GetTransactionDirectory(pushSourceRoot, pushHandoff.TransactionId));
            Dictionary<string, string> pushPeerTree;
            bool pushByteIdentical;
            int pushFilesTransferred;
            bool receiveDidNotAcknowledge;
            bool receiveDidNotRun;
            bool firstConnectionDropped;
            int proxyConnectionCount;
            int proxyChunkCount;
            int proxyMaxChunkBytes;

            await using (var backend = new TcpIntegrationServer(
                             IntegrationApplicationIds.MachineStudio,
                             pushPeerRoot,
                             IPAddress.Loopback,
                             0,
                             sharedKey,
                             options))
            {
                await backend.StartAsync().ConfigureAwait(false);
                await using var proxy = new ChunkingTcpProxy(
                    backend.LocalEndpoint!,
                    dropAfterBytes: 8_192,
                    chunkBytes: 4_096,
                    chunkDelay: TimeSpan.FromMilliseconds(1));
                await proxy.StartAsync().ConfigureAwait(false);
                await using var twoD = new TwoDIntegrationTcpExchange(
                    pushSourceRoot,
                    IPAddress.Loopback,
                    0,
                    sharedKey,
                    options);
                var pushed = await twoD.PushTransactionAsync(
                        new TcpIntegrationEndpoint(
                            proxy.LocalEndpoint.Address.ToString(),
                            proxy.LocalEndpoint.Port),
                        pushHandoff.TransactionId)
                    .ConfigureAwait(false);
                string pushPeerTransactionDirectory = GetTransactionDirectory(
                    pushPeerRoot,
                    pushHandoff.TransactionId);
                pushPeerTree = ComputeTree(pushPeerTransactionDirectory);
                pushByteIdentical = TreesEqual(pushSourceTree, pushPeerTree);
                receiveDidNotAcknowledge =
                    !File.Exists(Path.Combine(
                        pushPeerTransactionDirectory,
                        IntegrationTransactionLayout.AcknowledgementFileName));
                receiveDidNotRun =
                    !File.Exists(Path.Combine(
                        pushPeerTransactionDirectory,
                        IntegrationTransactionLayout.ResultFileName));
                var discovered = TwoDIntegrationExchange.DiscoverHandoffs(pushPeerRoot);
                Require(
                    discovered.Count == 1
                    && discovered[0].Handoff.TransactionId == pushHandoff.TransactionId,
                    "The proxy-delivered Handoff was not discoverable at the peer.");
                _ = TwoDIntegrationExchange.ReadHandoff(
                    pushPeerRoot,
                    pushHandoff.TransactionId);
                Require(
                    pushed.TransactionId == pushHandoff.TransactionId
                    && pushed.FilesTransferred == pushSourceTree.Count
                    && pushed.BytesTransferred > 0,
                    "The retry push receipt did not describe the complete transaction.");
                pushFilesTransferred = pushed.FilesTransferred;
                firstConnectionDropped = proxy.FirstConnectionDropped;
                proxyConnectionCount = proxy.ConnectionCount;
                proxyChunkCount = proxy.ChunkCount;
                proxyMaxChunkBytes = proxy.MaxChunkBytes;
            }

            Require(pushByteIdentical, "The reconnect retry did not preserve transaction bytes.");
            Require(receiveDidNotAcknowledge, "TCP receive unexpectedly created an Acknowledgement.");
            Require(receiveDidNotRun, "TCP receive unexpectedly published a Result.");
            Require(firstConnectionDropped, "The proxy did not drop the first partial connection.");
            Require(proxyConnectionCount >= 2, "The client did not reconnect through its retry path.");
            Require(proxyChunkCount > 1 && proxyMaxChunkBytes <= 4_096, "Chunked forwarding was not observed.");

            string reversedPeerRoot = Path.Combine(runRoot, "reversed-peer");
            string reversedConsumerRoot = Path.Combine(runRoot, "reversed-consumer");
            var reversedHandoff = CreateTransaction(
                reversedPeerRoot,
                image,
                recipe,
                consumer,
                includeCompletedSnapshot: true,
                includePadding: false);
            IReadOnlyList<string> reversedWireOrder;
            bool runRejectedByExistingResultGuard;
            bool resultBytesUnchanged;
            string pulledResultStatus;
            string pulledResultOutcome;
            string pulledResultErrorCode;
            int pulledFilesTransferred;

            await using (var peer = new ScriptedTcpPeer(
                             reversedPeerRoot,
                             reversedHandoff.TransactionId,
                             sharedKey,
                             chunkBytes: 1_024,
                             chunkDelay: TimeSpan.FromMilliseconds(1)))
            {
                await peer.StartAsync().ConfigureAwait(false);
                await using var puller = new TwoDIntegrationTcpExchange(
                    reversedConsumerRoot,
                    IPAddress.Loopback,
                    0,
                    sharedKey,
                    new TcpIntegrationOptions
                    {
                        MaxAttempts = 1,
                        ConnectTimeout = TimeSpan.FromSeconds(5),
                        IdleTimeout = TimeSpan.FromSeconds(30)
                    });
                var pulled = await puller.PullTransactionAsync(
                        new TcpIntegrationEndpoint(
                            peer.LocalEndpoint.Address.ToString(),
                            peer.LocalEndpoint.Port),
                        reversedHandoff.TransactionId)
                    .ConfigureAwait(false);
                var pulledHandoff = TwoDIntegrationExchange.ReadHandoff(
                    reversedConsumerRoot,
                    reversedHandoff.TransactionId);
                var pulledAcknowledgement = TwoDIntegrationExchange.ReadAcknowledgement(
                    reversedConsumerRoot,
                    reversedHandoff.TransactionId);
                var pulledResult = TwoDIntegrationExchange.ReadResult(
                    reversedConsumerRoot,
                    reversedHandoff.TransactionId);
                string resultPath = Path.Combine(
                    GetTransactionDirectory(reversedConsumerRoot, reversedHandoff.TransactionId),
                    IntegrationTransactionLayout.ResultFileName);
                string resultHashBefore = ComputeFileSha256(resultPath);
                runRejectedByExistingResultGuard = false;
                try
                {
                    _ = await puller.RunAcceptedHandoffAsync(
                            reversedHandoff.TransactionId,
                            cleanManifestPath)
                        .ConfigureAwait(false);
                }
                catch (IntegrationContractException exception)
                    when (exception.ErrorCode == IntegrationErrorCode.InvalidState)
                {
                    runRejectedByExistingResultGuard = true;
                }

                string resultHashAfter = ComputeFileSha256(resultPath);
                resultBytesUnchanged = string.Equals(
                    resultHashBefore,
                    resultHashAfter,
                    StringComparison.OrdinalIgnoreCase);
                reversedWireOrder = peer.LastWireOrder;
                pulledFilesTransferred = pulled.FilesTransferred;
                pulledResultStatus = pulledResult.Status.ToString();
                pulledResultOutcome = pulledResult.Outcome.ToString();
                pulledResultErrorCode = pulledResult.Error?.Code.ToString() ?? string.Empty;
                Require(
                    pulledHandoff.TransactionId == reversedHandoff.TransactionId
                    && pulledAcknowledgement.HandoffMessageId == pulledHandoff.MessageId
                    && pulledResult.TransactionId == pulledHandoff.TransactionId
                    && pulledResult.Outcome == IntegrationInspectionOutcome.Pass,
                    "The reversed-wire snapshot lost its explicit Handoff/ACK/Result correlation.");
            }

            Require(
                reversedWireOrder.Count >= 2
                && reversedWireOrder[0].Equals(
                    IntegrationTransactionLayout.ResultFileName,
                    StringComparison.Ordinal)
                && reversedWireOrder[1].Equals(
                    IntegrationTransactionLayout.AcknowledgementFileName,
                    StringComparison.Ordinal),
                "The fake peer did not deliver Result before Acknowledgement on the wire.");
            Require(
                pulledFilesTransferred == reversedWireOrder.Count,
                "The reversed pull receipt did not contain the complete manifest.");
            Require(
                runRejectedByExistingResultGuard,
                "An explicit Run was not rejected after a Result already existed.");
            Require(resultBytesUnchanged, "The duplicate Run attempt changed the persisted Result bytes.");

            string wrongConsumerRoot = Path.Combine(runRoot, "wrong-correlation-consumer");
            string wrongCorrelationCode = string.Empty;
            bool wrongCorrelationPublished = false;
            await using (var peer = new ScriptedTcpPeer(
                             reversedPeerRoot,
                             reversedHandoff.TransactionId,
                             sharedKey,
                             chunkBytes: 1_024,
                             chunkDelay: TimeSpan.Zero,
                             wrongCorrelationOnly: true))
            {
                await peer.StartAsync().ConfigureAwait(false);
                await using var puller = new TwoDIntegrationTcpExchange(
                    wrongConsumerRoot,
                    IPAddress.Loopback,
                    0,
                    sharedKey,
                    new TcpIntegrationOptions
                    {
                        MaxAttempts = 1,
                        ConnectTimeout = TimeSpan.FromSeconds(5),
                        IdleTimeout = TimeSpan.FromSeconds(30)
                    });
                Guid wrongTransactionId = Guid.NewGuid();
                try
                {
                    _ = await puller.PullTransactionAsync(
                            new TcpIntegrationEndpoint(
                                peer.LocalEndpoint.Address.ToString(),
                                peer.LocalEndpoint.Port),
                            wrongTransactionId)
                        .ConfigureAwait(false);
                }
                catch (TcpIntegrationTransportException exception)
                {
                    wrongCorrelationCode = exception.Code;
                }

                wrongCorrelationPublished = Directory.Exists(
                    GetTransactionDirectory(wrongConsumerRoot, wrongTransactionId));
            }

            Require(
                string.Equals(
                    wrongCorrelationCode,
                    "correlationMismatch",
                    StringComparison.Ordinal),
                $"Wrong response identity was not rejected as correlationMismatch: {wrongCorrelationCode}");
            Require(!wrongCorrelationPublished, "Wrong-correlation pull published a transaction unexpectedly.");

            string reportPath = Path.Combine(runRoot, "two-d-tcp-fault-injection-contract.json");
            File.WriteAllText(
                reportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        schemaVersion = "1.0",
                        push = new
                        {
                            transactionId = pushHandoff.TransactionId,
                            filesTransferred = pushFilesTransferred,
                            sourceFileCount = pushSourceTree.Count,
                            peerFileCount = pushPeerTree.Count,
                            byteIdentical = pushByteIdentical,
                            firstConnectionDropped,
                            proxyConnectionCount,
                            proxyChunkCount,
                            proxyMaxChunkBytes,
                            receiveDidNotAcknowledge,
                            receiveDidNotRun
                        },
                        reversedPull = new
                        {
                            transactionId = reversedHandoff.TransactionId,
                            wireOrder = reversedWireOrder,
                            filesTransferred = pulledFilesTransferred,
                            resultStatus = pulledResultStatus,
                            resultOutcome = pulledResultOutcome,
                            resultErrorCode = pulledResultErrorCode,
                            runRejectedByExistingResultGuard,
                            resultBytesUnchanged
                        },
                        wrongCorrelation = new
                        {
                            errorCode = wrongCorrelationCode,
                            transactionPublished = wrongCorrelationPublished
                        },
                        policy = new
                        {
                            receiveIsMaterializationOnly = true,
                            executionRequiresExplicitAcknowledgementAndRun = true,
                            noNewProtocolOrProductionOwner = true
                        }
                    },
                    new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(
                $"2D TCP fault-injection contract passed. Reconnect={proxyConnectionCount}, ReverseOrder={string.Join(",", reversedWireOrder)}, Correlation={wrongCorrelationCode}");
            Console.WriteLine($"Evidence={reportPath}");
            return 0;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(sharedKey);
        }
    }

    private static IntegrationHandoffV2 CreateTransaction(
        string exchangeRoot,
        string sourceImagePath,
        string recipePath,
        IntegrationApplicationIdentity consumer,
        bool includeCompletedSnapshot,
        bool includePadding)
    {
        string root = Path.GetFullPath(exchangeRoot);
        var transactionId = Guid.NewGuid();
        string transactionDirectory = GetTransactionDirectory(root, transactionId);
        Directory.CreateDirectory(Path.Combine(
            transactionDirectory,
            IntegrationTransactionLayout.ArtifactsDirectoryName));

        var machineProject = WriteArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.MachineProject,
            "machine-project",
            "{\"schema\":\"machine-project/1.0\",\"fixture\":true}",
            "artifacts/machine-project.json");
        var source = CopyArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.InspectionSource,
            "source",
            sourceImagePath,
            "artifacts/source.png");
        var recipe = CopyArtifact(
            transactionDirectory,
            IntegrationArtifactRoles.InspectionRecipe,
            "recipe",
            recipePath,
            "artifacts/recipe.xml");
        if (includePadding)
        {
            File.WriteAllBytes(
                Path.Combine(
                    transactionDirectory,
                    IntegrationTransactionLayout.ArtifactsDirectoryName,
                    "fault-padding.bin"),
                Enumerable.Range(0, 64 * 1024)
                    .Select(index => (byte)(index % 251))
                    .ToArray());
        }

        var context = new IntegrationInspectionContextV2(
            "tcp-fault-project",
            "machine-project/1.0",
            "tcp-fault-sequence",
            "inspect-image",
            "camera-loopback",
            "acquisition-loopback",
            "frame-loopback",
            "px",
            IntegrationInspectionModality.TwoD,
            IntegrationInspectionInputKind.Image,
            source.Sha256,
            recipe.Sha256,
            consumer,
            [machineProject, source, recipe]);
        var handoff = new IntegrationHandoffV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Handoff,
            Guid.NewGuid(),
            transactionId,
            DateTimeOffset.UtcNow,
            new IntegrationApplicationIdentity(
                IntegrationApplicationIds.MachineStudio,
                "2.2.0-dev.4",
                ProducerCommit,
                IntegrationSourceState.Clean),
            context);
        File.WriteAllBytes(
            Path.Combine(
                transactionDirectory,
                IntegrationTransactionLayout.HandoffFileName),
            IntegrationContractJson.SerializeCanonical(handoff));

        if (!includeCompletedSnapshot)
        {
            return handoff;
        }

        var acknowledgement = new IntegrationAcknowledgementV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Acknowledgement,
            Guid.NewGuid(),
            handoff.TransactionId,
            handoff.MessageId,
            handoff.CreatedAtUtc,
            consumer,
            IntegrationAcknowledgementStatus.Accepted,
            null);
        File.WriteAllBytes(
            Path.Combine(
                transactionDirectory,
                IntegrationTransactionLayout.AcknowledgementFileName),
            IntegrationContractJson.SerializeCanonical(acknowledgement));

        string runId = $"scripted-{transactionId:N}";
        string runRecordRelativePath =
            $"{IntegrationTransactionLayout.ArtifactsDirectoryName}/run-record.json";
        string runRecordPath = Path.Combine(
            transactionDirectory,
            runRecordRelativePath.Replace('/', Path.DirectorySeparatorChar));
        File.WriteAllText(
            runRecordPath,
            $"{{\"schemaVersion\":\"1.0\",\"runId\":\"{runId}\",\"sourceSha256\":\"{source.Sha256}\",\"recipeSha256\":\"{recipe.Sha256}\"}}",
            new UTF8Encoding(false));
        var runRecord = CreateArtifactReference(
            IntegrationArtifactRoles.RunRecord,
            runId,
            runRecordPath,
            runRecordRelativePath);
        var result = new IntegrationResultV2(
            IntegrationContractSchema.V2,
            IntegrationMessageKind.Result,
            Guid.NewGuid(),
            handoff.TransactionId,
            handoff.MessageId,
            acknowledgement.MessageId,
            acknowledgement.CreatedAtUtc,
            consumer,
            IntegrationResultStatus.Completed,
            IntegrationInspectionOutcome.Pass,
            runId,
            runRecord,
            IntegrationRunCorrelation.FromContext(handoff.Context),
            [],
            [],
            null);
        File.WriteAllBytes(
            Path.Combine(
                transactionDirectory,
                IntegrationTransactionLayout.ResultFileName),
            IntegrationContractJson.SerializeCanonical(result));
        return handoff;
    }

    private static IntegrationArtifactReference CopyArtifact(
        string transactionDirectory,
        string role,
        string artifactId,
        string sourcePath,
        string relativePath)
    {
        string targetPath = Path.Combine(
            transactionDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.Copy(sourcePath, targetPath, overwrite: false);
        return CreateArtifactReference(role, artifactId, targetPath, relativePath);
    }

    private static IntegrationArtifactReference WriteArtifact(
        string transactionDirectory,
        string role,
        string artifactId,
        string content,
        string relativePath)
    {
        string targetPath = Path.Combine(
            transactionDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        File.WriteAllText(targetPath, content, new UTF8Encoding(false));
        return CreateArtifactReference(role, artifactId, targetPath, relativePath);
    }

    private static IntegrationArtifactReference CreateArtifactReference(
        string role,
        string artifactId,
        string fullPath,
        string relativePath)
    {
        using var stream = File.OpenRead(fullPath);
        return new(
            role,
            artifactId,
            relativePath,
            stream.Length,
            Convert.ToHexString(SHA256.HashData(stream)));
    }

    private static Dictionary<string, string> ComputeTree(string root)
    {
        if (!Directory.Exists(root))
        {
            return new(StringComparer.OrdinalIgnoreCase);
        }

        return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                path => Path.GetRelativePath(root, path).Replace('\\', '/'),
                path => ComputeFileSha256(path),
                StringComparer.OrdinalIgnoreCase);
    }

    private static bool TreesEqual(
        IReadOnlyDictionary<string, string> expected,
        IReadOnlyDictionary<string, string> actual) =>
        expected.Count == actual.Count
        && expected.All(pair => actual.TryGetValue(pair.Key, out var hash)
            && string.Equals(hash, pair.Value, StringComparison.OrdinalIgnoreCase));

    private static string ComputeFileSha256(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static string GetTransactionDirectory(
        string exchangeRoot,
        Guid transactionId) =>
        Path.Combine(
            Path.GetFullPath(exchangeRoot),
            IntegrationTransactionLayout.TransactionsDirectoryName,
            transactionId.ToString("D"));

    private static async Task<WireFrame> ReadWireFrameAsync(
        Stream stream,
        byte[] sharedKey,
        CancellationToken cancellationToken)
    {
        var magic = new byte[WireMagic.Length];
        await stream.ReadExactlyAsync(magic, cancellationToken).ConfigureAwait(false);
        Require(magic.AsSpan().SequenceEqual(WireMagic), "The fake peer received an invalid TCP magic header.");
        var lengthBytes = new byte[sizeof(int)];
        await stream.ReadExactlyAsync(lengthBytes, cancellationToken).ConfigureAwait(false);
        int length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);
        Require(length > 0 && length <= 1_048_576, "The fake peer received an invalid control-frame length.");
        var json = new byte[length];
        var tag = new byte[32];
        await stream.ReadExactlyAsync(json, cancellationToken).ConfigureAwait(false);
        await stream.ReadExactlyAsync(tag, cancellationToken).ConfigureAwait(false);
        byte[] expectedTag = HMACSHA256.HashData(sharedKey, json);
        Require(
            CryptographicOperations.FixedTimeEquals(tag, expectedTag),
            "The fake peer received an unauthenticated control frame.");
        return JsonSerializer.Deserialize<WireFrame>(json, WireJsonOptions)
               ?? throw new InvalidDataException("The fake peer received a null control frame.");
    }

    private static async Task WriteWireFrameAsync(
        Stream stream,
        WireFrame frame,
        byte[] sharedKey,
        CancellationToken cancellationToken)
    {
        byte[] json = JsonSerializer.SerializeToUtf8Bytes(frame, WireJsonOptions);
        var length = new byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(length, json.Length);
        byte[] tag = HMACSHA256.HashData(sharedKey, json);
        await stream.WriteAsync(WireMagic, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(length, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(tag, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private static string CreateNonce() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class ChunkingTcpProxy : IAsyncDisposable
    {
        private readonly TcpIntegrationEndpoint _backend;
        private readonly int _dropAfterBytes;
        private readonly int _chunkBytes;
        private readonly TimeSpan _chunkDelay;
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stopSource = new();
        private readonly ConcurrentBag<Task> _active = new();
        private int _dropPending = 1;
        private int _connectionCount;
        private int _chunkCount;
        private int _maxChunkBytes;
        private int _firstConnectionDropped;
        private Task? _acceptTask;

        public ChunkingTcpProxy(
            IPEndPoint backend,
            int dropAfterBytes,
            int chunkBytes,
            TimeSpan chunkDelay)
        {
            _backend = new TcpIntegrationEndpoint("127.0.0.1", backend.Port);
            _dropAfterBytes = dropAfterBytes;
            _chunkBytes = chunkBytes;
            _chunkDelay = chunkDelay;
        }

        public IPEndPoint LocalEndpoint =>
            (IPEndPoint)(_listener.LocalEndpoint
                ?? throw new InvalidOperationException("The TCP proxy is not started."));

        public int ConnectionCount => Volatile.Read(ref _connectionCount);

        public int ChunkCount => Volatile.Read(ref _chunkCount);

        public int MaxChunkBytes => Volatile.Read(ref _maxChunkBytes);

        public bool FirstConnectionDropped => Volatile.Read(ref _firstConnectionDropped) == 1;

        public Task StartAsync()
        {
            _listener.Start();
            _acceptTask = AcceptLoopAsync(_stopSource.Token);
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            _stopSource.Cancel();
            _listener.Stop();
            if (_acceptTask is not null)
            {
                try
                {
                    await _acceptTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            await Task.WhenAll(_active.ToArray()).ConfigureAwait(false);
            _stopSource.Dispose();
        }

        private async Task AcceptLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await _listener.AcceptTcpClientAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var task = HandleClientAsync(client, cancellationToken);
                _active.Add(task);
                _ = task.ContinueWith(
                    static completed => _ = completed.Exception,
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.Default);
            }
        }

        private async Task HandleClientAsync(
            TcpClient client,
            CancellationToken cancellationToken)
        {
            using (client)
            using (var backend = new TcpClient { NoDelay = true })
            {
                Interlocked.Increment(ref _connectionCount);
                try
                {
                    await backend.ConnectAsync(
                            _backend.Host,
                            _backend.Port,
                            cancellationToken)
                        .ConfigureAwait(false);
                    client.NoDelay = true;
                    await using NetworkStream clientStream = client.GetStream();
                    await using NetworkStream backendStream = backend.GetStream();
                    if (Interlocked.Exchange(ref _dropPending, 0) == 1)
                    {
                        await ForwardUntilDropAsync(
                                clientStream,
                                backendStream,
                                cancellationToken)
                            .ConfigureAwait(false);
                        return;
                    }

                    await ForwardBidirectionalAsync(
                            clientStream,
                            backendStream,
                            cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                }
                catch (IOException)
                {
                }
                catch (SocketException)
                {
                }
            }
        }

        private async Task ForwardUntilDropAsync(
            Stream source,
            Stream destination,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[_chunkBytes];
            int forwarded = 0;
            while (forwarded < _dropAfterBytes)
            {
                int count = await source.ReadAsync(
                        buffer.AsMemory(0, buffer.Length),
                        cancellationToken)
                    .ConfigureAwait(false);
                if (count == 0)
                {
                    return;
                }

                await destination.WriteAsync(
                        buffer.AsMemory(0, count),
                        cancellationToken)
                    .ConfigureAwait(false);
                await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
                forwarded = checked(forwarded + count);
                ObserveChunk(count);
                if (_chunkDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_chunkDelay, cancellationToken).ConfigureAwait(false);
                }
            }

            Interlocked.Exchange(ref _firstConnectionDropped, 1);
        }

        private async Task ForwardBidirectionalAsync(
            Stream clientStream,
            Stream backendStream,
            CancellationToken cancellationToken)
        {
            using var connectionStop = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);
            Task clientToBackend = CopyAsync(
                clientStream,
                backendStream,
                connectionStop.Token);
            Task backendToClient = CopyAsync(
                backendStream,
                clientStream,
                connectionStop.Token);
            try
            {
                await Task.WhenAny(clientToBackend, backendToClient).ConfigureAwait(false);
            }
            finally
            {
                connectionStop.Cancel();
                try
                {
                    await Task.WhenAll(clientToBackend, backendToClient).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        private async Task CopyAsync(
            Stream source,
            Stream destination,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[_chunkBytes];
            while (!cancellationToken.IsCancellationRequested)
            {
                int count = await source.ReadAsync(
                        buffer.AsMemory(0, buffer.Length),
                        cancellationToken)
                    .ConfigureAwait(false);
                if (count == 0)
                {
                    return;
                }

                await destination.WriteAsync(
                        buffer.AsMemory(0, count),
                        cancellationToken)
                    .ConfigureAwait(false);
                await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
                ObserveChunk(count);
                if (_chunkDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_chunkDelay, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private void ObserveChunk(int count)
        {
            Interlocked.Increment(ref _chunkCount);
            while (true)
            {
                int current = Volatile.Read(ref _maxChunkBytes);
                if (current >= count
                    || Interlocked.CompareExchange(ref _maxChunkBytes, count, current) == current)
                {
                    return;
                }
            }
        }
    }

    private sealed class ScriptedTcpPeer : IAsyncDisposable
    {
        private readonly string _exchangeRoot;
        private readonly Guid _reversedTransactionId;
        private readonly byte[] _sharedKey;
        private readonly int _chunkBytes;
        private readonly TimeSpan _chunkDelay;
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stopSource = new();
        private readonly ConcurrentBag<Task> _active = new();
        private IReadOnlyList<string> _lastWireOrder = [];
        private int _connectionCount;
        private Task? _acceptTask;

        public ScriptedTcpPeer(
            string exchangeRoot,
            Guid reversedTransactionId,
            byte[] sharedKey,
            int chunkBytes,
            TimeSpan chunkDelay,
            bool wrongCorrelationOnly = false)
        {
            _exchangeRoot = Path.GetFullPath(exchangeRoot);
            _reversedTransactionId = reversedTransactionId;
            _sharedKey = sharedKey;
            _chunkBytes = chunkBytes;
            _chunkDelay = chunkDelay;
            WrongCorrelationOnly = wrongCorrelationOnly;
        }

        private bool WrongCorrelationOnly { get; }

        public IPEndPoint LocalEndpoint =>
            (IPEndPoint)(_listener.LocalEndpoint
                ?? throw new InvalidOperationException("The scripted TCP peer is not started."));

        public IReadOnlyList<string> LastWireOrder => _lastWireOrder;

        public Task StartAsync()
        {
            _listener.Start();
            _acceptTask = AcceptLoopAsync(_stopSource.Token);
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            _stopSource.Cancel();
            _listener.Stop();
            if (_acceptTask is not null)
            {
                try
                {
                    await _acceptTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            await Task.WhenAll(_active.ToArray()).ConfigureAwait(false);
            _stopSource.Dispose();
        }

        private async Task AcceptLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await _listener.AcceptTcpClientAsync(cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var task = HandleClientAsync(
                    client,
                    Interlocked.Increment(ref _connectionCount),
                    cancellationToken);
                _active.Add(task);
                _ = task.ContinueWith(
                    static completed => _ = completed.Exception,
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.Default);
            }
        }

        private async Task HandleClientAsync(
            TcpClient client,
            int connectionNumber,
            CancellationToken cancellationToken)
        {
            using (client)
            {
                client.NoDelay = true;
                await using NetworkStream stream = client.GetStream();
                var request = await ReadWireFrameAsync(
                        stream,
                        _sharedKey,
                        cancellationToken)
                    .ConfigureAwait(false);
                Require(
                    string.Equals(request.Kind, "pull", StringComparison.Ordinal),
                    "The scripted peer expected a pull request.");
                if (!WrongCorrelationOnly && connectionNumber == 1)
                {
                    Require(
                        request.TransactionId == _reversedTransactionId,
                        "The scripted peer received an unexpected reverse-order transaction.");
                    var manifests = CreateReverseOrderManifest(_reversedTransactionId);
                    _lastWireOrder = manifests.Select(file => file.RelativePath).ToArray();
                    await WriteWireFrameAsync(
                            stream,
                            new WireFrame
                            {
                                Kind = "files",
                                RequestId = request.RequestId,
                                CreatedAtUtc = DateTimeOffset.UtcNow,
                                Nonce = CreateNonce(),
                                ReplyToNonce = request.Nonce,
                                ApplicationId = IntegrationApplicationIds.MachineStudio,
                                TransactionId = request.TransactionId,
                                Files = manifests.ToList(),
                                FilesTransferred = manifests.Count,
                                BytesTransferred = manifests.Sum(file => file.ByteLength),
                                Idempotent = false
                            },
                            _sharedKey,
                            cancellationToken)
                        .ConfigureAwait(false);
                    foreach (var file in manifests)
                    {
                        await SendFileAsync(
                                stream,
                                _reversedTransactionId,
                                file,
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                    return;
                }

                await WriteWireFrameAsync(
                        stream,
                        new WireFrame
                        {
                            Kind = "ok",
                            RequestId = Guid.NewGuid(),
                            CreatedAtUtc = DateTimeOffset.UtcNow,
                            Nonce = CreateNonce(),
                            ReplyToNonce = request.Nonce,
                            ApplicationId = IntegrationApplicationIds.MachineStudio,
                            TransactionId = request.TransactionId,
                            Files = [],
                            FilesTransferred = 0,
                            BytesTransferred = 0,
                            Idempotent = false
                        },
                        _sharedKey,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private List<WireFile> CreateReverseOrderManifest(Guid transactionId)
        {
            string transactionDirectory = GetTransactionDirectory(
                _exchangeRoot,
                transactionId);
            var files = Directory.EnumerateFiles(
                    transactionDirectory,
                    "*",
                    SearchOption.AllDirectories)
                .Select(path => new WireFile
                {
                    RelativePath = Path.GetRelativePath(transactionDirectory, path)
                        .Replace(Path.DirectorySeparatorChar, '/'),
                    ByteLength = new FileInfo(path).Length,
                    Sha256 = ComputeFileSha256(path)
                })
                .ToList();
            return files
                .OrderBy(file => file.RelativePath switch
                {
                    IntegrationTransactionLayout.ResultFileName => 0,
                    IntegrationTransactionLayout.AcknowledgementFileName => 1,
                    IntegrationTransactionLayout.HandoffFileName => 2,
                    _ => 3
                })
                .ThenBy(file => file.RelativePath, StringComparer.Ordinal)
                .ToList();
        }

        private async Task SendFileAsync(
            Stream stream,
            Guid transactionId,
            WireFile file,
            CancellationToken cancellationToken)
        {
            string path = Path.Combine(
                GetTransactionDirectory(_exchangeRoot, transactionId),
                file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            byte[] bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
            Require(
                bytes.LongLength == file.ByteLength
                && string.Equals(
                    ComputeFileSha256(path),
                    file.Sha256,
                    StringComparison.OrdinalIgnoreCase),
                $"The scripted peer manifest changed before sending '{file.RelativePath}'.");
            for (int offset = 0; offset < bytes.Length; offset += _chunkBytes)
            {
                int count = Math.Min(_chunkBytes, bytes.Length - offset);
                await stream.WriteAsync(
                        bytes.AsMemory(offset, count),
                        cancellationToken)
                    .ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                if (_chunkDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_chunkDelay, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    private sealed class WireFrame
    {
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; init; } = "1.0";

        [JsonPropertyName("kind")]
        public string Kind { get; init; } = string.Empty;

        [JsonPropertyName("requestId")]
        public Guid RequestId { get; init; }

        [JsonPropertyName("createdAtUtc")]
        public DateTimeOffset CreatedAtUtc { get; init; }

        [JsonPropertyName("nonce")]
        public string Nonce { get; init; } = string.Empty;

        [JsonPropertyName("replyToNonce")]
        public string? ReplyToNonce { get; init; }

        [JsonPropertyName("applicationId")]
        public string ApplicationId { get; init; } = string.Empty;

        [JsonPropertyName("transactionId")]
        public Guid? TransactionId { get; init; }

        [JsonPropertyName("files")]
        public List<WireFile> Files { get; init; } = [];

        [JsonPropertyName("filesTransferred")]
        public int FilesTransferred { get; init; }

        [JsonPropertyName("bytesTransferred")]
        public long BytesTransferred { get; init; }

        [JsonPropertyName("idempotent")]
        public bool Idempotent { get; init; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; init; }

        [JsonPropertyName("retryable")]
        public bool Retryable { get; init; }
    }

    private sealed class WireFile
    {
        [JsonPropertyName("relativePath")]
        public string RelativePath { get; init; } = string.Empty;

        [JsonPropertyName("byteLength")]
        public long ByteLength { get; init; }

        [JsonPropertyName("sha256")]
        public string Sha256 { get; init; } = string.Empty;
    }
}
