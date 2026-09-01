using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using OpenVisionLab.Core.Integration;
using OpenVisionLab.Integration.Contracts;
using OpenVisionLab.Integration.Transport.Tcp;

if (args.Length != 4)
{
    Console.Error.WriteLine(
        "Usage: TwoDIntegrationTcpSmoke <evidenceRoot> <sourceImagePath> <recipePath> <runtimeBuildManifestPath>");
    return 2;
}

var evidenceRoot = Path.GetFullPath(args[0]);
var sourceImagePath = Path.GetFullPath(args[1]);
var recipePath = Path.GetFullPath(args[2]);
var runtimeBuildManifestPath = Path.GetFullPath(args[3]);
Require(File.Exists(sourceImagePath), $"Source image was not found: {sourceImagePath}");
Require(File.Exists(recipePath), $"Recipe was not found: {recipePath}");
Require(File.Exists(runtimeBuildManifestPath), $"Runtime build manifest was not found: {runtimeBuildManifestPath}");

var runRoot = Path.Combine(
    evidenceRoot,
    $"two-d-tcp-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}");
var producerRoot = Path.Combine(runRoot, "producer");
var consumerRoot = Path.Combine(runRoot, "consumer");
Directory.CreateDirectory(producerRoot);
Directory.CreateDirectory(consumerRoot);

var sharedKey = RandomNumberGenerator.GetBytes(32);
try
{
    var runtimeBuildManifest = IntegrationContractJson.DeserializeRuntimeBuildManifest(
        File.ReadAllBytes(runtimeBuildManifestPath));
    bool dirtyRuntime = runtimeBuildManifest.Identity.SourceState != IntegrationSourceState.Clean;
    var consumer = dirtyRuntime
        ? runtimeBuildManifest.Identity with
        {
            SourceState = IntegrationSourceState.Clean
        }
        : TwoDIntegrationBuildIdentity.LoadQualifiedIdentity(runtimeBuildManifestPath);
    var handoff = CreateHandoff(
        producerRoot,
        sourceImagePath,
        recipePath,
        consumer);
    var options = new TcpIntegrationOptions
    {
        MaxAttempts = 1,
        ConnectTimeout = TimeSpan.FromSeconds(5),
        IdleTimeout = TimeSpan.FromSeconds(30)
    };

    await using var machineServer = new TcpIntegrationServer(
        IntegrationApplicationIds.MachineStudio,
        producerRoot,
        IPAddress.Loopback,
        0,
        sharedKey,
        options);
    await using var twoD = new TwoDIntegrationTcpExchange(
        consumerRoot,
        IPAddress.Loopback,
        0,
        sharedKey,
        options);
    await machineServer.StartAsync();
    await twoD.StartAsync();

    var machineEndpoint = new TcpIntegrationEndpoint(
        "127.0.0.1",
        machineServer.LocalEndpoint!.Port);
    var twoDEndpoint = new TcpIntegrationEndpoint(
        "127.0.0.1",
        twoD.LocalEndpoint!.Port);

    var ping = await twoD.PingPeerAsync(machineEndpoint);
    TcpIntegrationTransferReceipt pushed;
    using (var machineClient = new TcpIntegrationClient(
               IntegrationApplicationIds.MachineStudio,
               twoDEndpoint,
               sharedKey,
               options))
    {
        pushed = await machineClient.PushTransactionAsync(
            producerRoot,
            handoff.TransactionId);
    }

    var consumerTransactionDirectory = GetTransactionDirectory(
        consumerRoot,
        handoff.TransactionId);
    Require(
        !File.Exists(Path.Combine(
            consumerTransactionDirectory,
            IntegrationTransactionLayout.AcknowledgementFileName)),
        "Receiving the TCP Handoff unexpectedly created an Acknowledgement.");
    Require(
        !File.Exists(Path.Combine(
            consumerTransactionDirectory,
            IntegrationTransactionLayout.ResultFileName)),
        "Receiving the TCP Handoff unexpectedly ran the 2D inspection.");

    var pulledHandoff = CreateHandoff(
        producerRoot,
        sourceImagePath,
        recipePath,
        consumer);
    var pulled = await twoD.PullTransactionAsync(
        machineEndpoint,
        pulledHandoff.TransactionId);
    var pulledTransactionDirectory = GetTransactionDirectory(
        consumerRoot,
        pulledHandoff.TransactionId);
    Require(
        !File.Exists(Path.Combine(
            pulledTransactionDirectory,
            IntegrationTransactionLayout.AcknowledgementFileName))
        && !File.Exists(Path.Combine(
            pulledTransactionDirectory,
            IntegrationTransactionLayout.ResultFileName)),
        "Pulling a TCP Handoff unexpectedly acknowledged or ran it.");

    var discovered = twoD.DiscoverHandoffs();
    Require(
        discovered.Count == 2
            && discovered.All(transaction =>
                !transaction.HasAcknowledgement && !transaction.HasResult)
            && discovered.Any(transaction =>
                transaction.Handoff.TransactionId == handoff.TransactionId)
            && discovered.Any(transaction =>
                transaction.Handoff.TransactionId == pulledHandoff.TransactionId),
        "The pushed and pulled TCP Handoffs were not discovered as untouched local transactions.");

    if (dirtyRuntime)
    {
        try
        {
            _ = twoD.AcknowledgeHandoff(
                handoff.TransactionId,
                runtimeBuildManifestPath);
        }
        catch (IntegrationContractException exception)
            when (exception.ErrorCode == IntegrationErrorCode.InvalidIdentity)
        {
            Require(
                !File.Exists(Path.Combine(
                    consumerTransactionDirectory,
                    IntegrationTransactionLayout.AcknowledgementFileName))
                && !File.Exists(Path.Combine(
                    consumerTransactionDirectory,
                    IntegrationTransactionLayout.ResultFileName))
                && !File.Exists(Path.Combine(
                    pulledTransactionDirectory,
                    IntegrationTransactionLayout.AcknowledgementFileName))
                && !File.Exists(Path.Combine(
                    pulledTransactionDirectory,
                    IntegrationTransactionLayout.ResultFileName)),
                "A dirty 2D runtime unexpectedly acknowledged or ran a TCP Handoff.");

            string dirtyReportPath = Path.Combine(
                runRoot,
                "two-d-tcp-dirty-runtime-smoke.json");
            File.WriteAllText(
                dirtyReportPath,
                JsonSerializer.Serialize(
                    new
                    {
                        schemaVersion = "1.0",
                        runtimeSourceState = runtimeBuildManifest.Identity.SourceState.ToString(),
                        pushedTransactionId = handoff.TransactionId,
                        pulledTransactionId = pulledHandoff.TransactionId,
                        twoDListenEndpoint = twoD.LocalEndpoint!.ToString(),
                        peerApplicationId = ping.PeerApplicationId,
                        discoveredTransactions = discovered.Count,
                        pushedFilesTransferred = pushed.FilesTransferred,
                        pushedBytesTransferred = pushed.BytesTransferred,
                        pulledFilesTransferred = pulled.FilesTransferred,
                        pulledBytesTransferred = pulled.BytesTransferred,
                        acknowledgementErrorCode = exception.ErrorCode.ToString(),
                        acknowledgementPublished = false,
                        resultPublished = false
                    },
                    new JsonSerializerOptions { WriteIndented = true }));

            Console.WriteLine(
                $"2D TCP dirty runtime fail-closed smoke passed. Transactions={discovered.Count}, Error={exception.ErrorCode}");
            Console.WriteLine($"Evidence={dirtyReportPath}");
            return 0;
        }

        throw new InvalidOperationException(
            "The dirty 2D runtime accepted a TCP Handoff unexpectedly.");
    }

    var acknowledgement = twoD.AcknowledgeHandoff(
        handoff.TransactionId,
        runtimeBuildManifestPath);
    Require(
        acknowledgement.Status == IntegrationAcknowledgementStatus.Accepted,
        "The explicit 2D acknowledgement was not accepted.");
    Require(
        !File.Exists(Path.Combine(
            consumerTransactionDirectory,
            IntegrationTransactionLayout.ResultFileName)),
        "Acknowledging the TCP Handoff unexpectedly ran the 2D inspection.");

    var result = await twoD.RunAcceptedHandoffAsync(
        handoff.TransactionId,
        runtimeBuildManifestPath);
    Require(
        result.Status == IntegrationResultStatus.Completed
            && result.Outcome == IntegrationInspectionOutcome.Pass
            && !string.IsNullOrWhiteSpace(result.RunId),
        $"The explicit 2D TCP inspection did not complete as Pass: {result.Status}/{result.Outcome}.");

    var delivered = await twoD.PushTransactionAsync(
        machineEndpoint,
        handoff.TransactionId);
    var returned = TwoDIntegrationExchange.ReadResult(
        producerRoot,
        handoff.TransactionId);
    Require(
        returned.MessageId == result.MessageId
            && returned.RunId == result.RunId
            && returned.Outcome == result.Outcome,
        "The Machine-side transaction did not receive the correlated 2D Result.");

    var reportPath = Path.Combine(runRoot, "two-d-tcp-smoke.json");
    File.WriteAllText(
        reportPath,
        JsonSerializer.Serialize(
            new
            {
                schemaVersion = "1.0",
                handoff.TransactionId,
                twoDListenEndpoint = twoD.LocalEndpoint!.ToString(),
                peerApplicationId = ping.PeerApplicationId,
                acknowledgement = acknowledgement.Status.ToString(),
                status = result.Status.ToString(),
                outcome = result.Outcome.ToString(),
                result.RunId,
                deliveredFilesTransferred = delivered.FilesTransferred,
                deliveredBytesTransferred = delivered.BytesTransferred,
                pulledFilesTransferred = pulled.FilesTransferred,
                pulledBytesTransferred = pulled.BytesTransferred,
                receiveDidNotAcknowledge = true,
                receiveDidNotRun = true,
                acknowledgeDidNotRun = true
            },
            new JsonSerializerOptions { WriteIndented = true }));

    Console.WriteLine(
        $"2D TCP smoke passed. Transaction={handoff.TransactionId:D}, Outcome={result.Outcome}, RunId={result.RunId}");
    Console.WriteLine($"Evidence={reportPath}");
    return 0;
}
finally
{
    CryptographicOperations.ZeroMemory(sharedKey);
}

static IntegrationHandoffV2 CreateHandoff(
    string exchangeRoot,
    string sourceImagePath,
    string recipePath,
    IntegrationApplicationIdentity consumer)
{
    var transactionId = Guid.NewGuid();
    var transactionDirectory = GetTransactionDirectory(exchangeRoot, transactionId);
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
    var context = new IntegrationInspectionContextV2(
        "machine-tcp-smoke",
        "machine-project/1.0",
        "sequence-001",
        "inspect-image",
        "camera-virtual",
        "acquisition-tcp",
        "frame-tcp",
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
            "2.1.0",
            "1111111111111111111111111111111111111111",
            IntegrationSourceState.Clean),
        context);
    File.WriteAllBytes(
        Path.Combine(
            transactionDirectory,
            IntegrationTransactionLayout.HandoffFileName),
        IntegrationContractJson.SerializeCanonical(handoff));
    return handoff;
}

static IntegrationArtifactReference CopyArtifact(
    string transactionDirectory,
    string role,
    string artifactId,
    string sourcePath,
    string relativePath)
{
    var targetPath = Path.Combine(
        transactionDirectory,
        relativePath.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
    File.Copy(sourcePath, targetPath, overwrite: false);
    return CreateArtifactReference(role, artifactId, targetPath, relativePath);
}

static IntegrationArtifactReference WriteArtifact(
    string transactionDirectory,
    string role,
    string artifactId,
    string content,
    string relativePath)
{
    var targetPath = Path.Combine(
        transactionDirectory,
        relativePath.Replace('/', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
    File.WriteAllText(targetPath, content, new UTF8Encoding(false));
    return CreateArtifactReference(role, artifactId, targetPath, relativePath);
}

static IntegrationArtifactReference CreateArtifactReference(
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

static string GetTransactionDirectory(string exchangeRoot, Guid transactionId) =>
    Path.Combine(
        Path.GetFullPath(exchangeRoot),
        IntegrationTransactionLayout.TransactionsDirectoryName,
        transactionId.ToString("D"));

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
