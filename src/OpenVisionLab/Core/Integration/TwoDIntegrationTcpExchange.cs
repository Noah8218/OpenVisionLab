#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using OpenVisionLab.Integration.Contracts;
using OpenVisionLab.Integration.Transport.Tcp;

namespace OpenVisionLab.Core.Integration;

/// <summary>
/// Direct TCP transport boundary for the existing explicit 2D integration workflow.
/// Receiving a transaction only publishes its immutable files to the local exchange root;
/// acknowledgement and inspection remain separate explicit calls.
/// </summary>
public sealed class TwoDIntegrationTcpExchange : IAsyncDisposable
{
    private readonly byte[] _sharedKey;
    private readonly TcpIntegrationOptions _options;
    private readonly TcpIntegrationServer _server;
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _activeRunCancellations = new();
    private bool _disposed;

    public TwoDIntegrationTcpExchange(
        string localExchangeRoot,
        IPAddress listenAddress,
        int port,
        ReadOnlySpan<byte> sharedKey,
        TcpIntegrationOptions? options = null)
    {
        LocalExchangeRoot = Path.GetFullPath(
            string.IsNullOrWhiteSpace(localExchangeRoot)
                ? throw new ArgumentException(
                    "A local 2D integration exchange root is required.",
                    nameof(localExchangeRoot))
                : localExchangeRoot.Trim());
        _sharedKey = sharedKey.ToArray();
        _options = options ?? new TcpIntegrationOptions();
        try
        {
            _server = new TcpIntegrationServer(
                IntegrationApplicationIds.TwoDStudio,
                LocalExchangeRoot,
                listenAddress,
                port,
                _sharedKey,
                _options);
            _server.CancellationRequested += HandleCancellationRequestedAsync;
        }
        catch
        {
            CryptographicOperations.ZeroMemory(_sharedKey);
            throw;
        }
    }

    public string LocalExchangeRoot { get; }

    public IPEndPoint? LocalEndpoint => _server.LocalEndpoint;

    public event Action<TcpIntegrationTransferReceipt>? RequestCompleted
    {
        add => _server.RequestCompleted += value;
        remove => _server.RequestCompleted -= value;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _server.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _server.StopAsync(cancellationToken);
    }

    public async Task<TcpIntegrationTransferReceipt> PingPeerAsync(
        TcpIntegrationEndpoint peer,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient(peer);
        return await client.PingAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<TcpIntegrationTransferReceipt> PushTransactionAsync(
        TcpIntegrationEndpoint peer,
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient(peer);
        return await client.PushTransactionAsync(
                LocalExchangeRoot,
                transactionId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<TcpIntegrationTransferReceipt> PullTransactionAsync(
        TcpIntegrationEndpoint peer,
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        using var client = CreateClient(peer);
        return await client.PullTransactionAsync(
                LocalExchangeRoot,
                transactionId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public IReadOnlyList<TwoDIntegrationTransactionSummary> DiscoverHandoffs()
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.DiscoverHandoffs(LocalExchangeRoot);
    }

    public TwoDIntegrationDiscoveryResult DiscoverHandoffsDetailed()
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.DiscoverHandoffsDetailed(LocalExchangeRoot);
    }

    public IntegrationHandoffV2 ReadHandoff(Guid transactionId)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.ReadHandoff(LocalExchangeRoot, transactionId);
    }

    public IntegrationAcknowledgementV2 AcknowledgeHandoff(
        Guid transactionId)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.AcknowledgeHandoff(
            LocalExchangeRoot,
            transactionId);
    }

    internal IntegrationAcknowledgementV2 AcknowledgeHandoff(
        Guid transactionId,
        string runtimeBuildManifestPath)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.AcknowledgeHandoff(
            LocalExchangeRoot,
            transactionId,
            runtimeBuildManifestPath);
    }

    public IntegrationAcknowledgementV2 RejectHandoff(
        Guid transactionId,
        string rejectionReason)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.RejectHandoff(
            LocalExchangeRoot,
            transactionId,
            rejectionReason);
    }

    internal IntegrationAcknowledgementV2 RejectHandoff(
        Guid transactionId,
        string rejectionReason,
        string runtimeBuildManifestPath)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.RejectHandoff(
            LocalExchangeRoot,
            transactionId,
            rejectionReason,
            runtimeBuildManifestPath);
    }

    public IntegrationAcknowledgementV2 ReadAcknowledgement(Guid transactionId)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.ReadAcknowledgement(
            LocalExchangeRoot,
            transactionId);
    }

    public IntegrationResultV2 ReadResult(Guid transactionId)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.ReadResult(LocalExchangeRoot, transactionId);
    }

    public Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        Guid transactionId,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return RunAcceptedHandoffWithCancellationAsync(
            transactionId,
            runtimeBuildManifestPath: null,
            stepTimeoutMilliseconds,
            preInspectionDelayMilliseconds: 0,
            cancellationToken);
    }

    internal Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        Guid transactionId,
        int stepTimeoutMilliseconds,
        int preInspectionDelayMilliseconds,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return RunAcceptedHandoffWithCancellationAsync(
            transactionId,
            runtimeBuildManifestPath: null,
            stepTimeoutMilliseconds,
            preInspectionDelayMilliseconds,
            cancellationToken);
    }

    internal Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        Guid transactionId,
        string runtimeBuildManifestPath,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return RunAcceptedHandoffWithCancellationAsync(
            transactionId,
            runtimeBuildManifestPath,
            stepTimeoutMilliseconds,
            preInspectionDelayMilliseconds: 0,
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            await _server.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(_sharedKey);
            _disposed = true;
        }
    }

    private TcpIntegrationClient CreateClient(TcpIntegrationEndpoint peer)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(peer);
        return new TcpIntegrationClient(
            IntegrationApplicationIds.TwoDStudio,
            peer,
            _sharedKey,
            _options);
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed, this);

    private async Task<IntegrationResultV2> RunAcceptedHandoffWithCancellationAsync(
        Guid transactionId,
        string? runtimeBuildManifestPath,
        int stepTimeoutMilliseconds,
        int preInspectionDelayMilliseconds,
        CancellationToken cancellationToken)
    {
        if (preInspectionDelayMilliseconds is < 0 or > 30000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(preInspectionDelayMilliseconds),
                "The pre-inspection delay must be between 0 and 30000 milliseconds.");
        }

        using var runCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        if (!_activeRunCancellations.TryAdd(transactionId, runCancellation))
        {
            throw new IntegrationContractException(
                IntegrationErrorCode.InvalidState,
                "The 2D transaction already has an active inspection.");
        }

        try
        {
            if (preInspectionDelayMilliseconds > 0)
            {
                try
                {
                    await Task.Delay(
                            preInspectionDelayMilliseconds,
                            runCancellation.Token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Continue into the existing consumer-owned execution path
                    // so it publishes the typed Cancelled Result.
                }
            }

            return runtimeBuildManifestPath is null
                ? await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                        LocalExchangeRoot,
                        transactionId,
                        stepTimeoutMilliseconds,
                        runCancellation.Token)
                    .ConfigureAwait(false)
                : await TwoDIntegrationExchange.RunAcceptedHandoffAsync(
                        LocalExchangeRoot,
                        transactionId,
                        runtimeBuildManifestPath,
                        stepTimeoutMilliseconds,
                        runCancellation.Token)
                    .ConfigureAwait(false);
        }
        finally
        {
            _activeRunCancellations.TryRemove(
                new KeyValuePair<Guid, CancellationTokenSource>(
                    transactionId,
                    runCancellation));
        }
    }

    private Task<TcpIntegrationCancellationReceipt> HandleCancellationRequestedAsync(
        IntegrationCancelRequestV2 request,
        CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (!string.Equals(
                request.Requester.ApplicationId,
                IntegrationApplicationIds.MachineStudio,
                StringComparison.Ordinal))
        {
            return Task.FromResult(CreateCancellationReceipt(
                request,
                TcpIntegrationCancellationStatus.Rejected,
                idempotent: false));
        }

        IntegrationHandoffV2 handoff;
        try
        {
            handoff = TwoDIntegrationExchange.ReadHandoffEnvelope(
                LocalExchangeRoot,
                request.TransactionId);
        }
        catch (Exception exception) when (
            exception is FileNotFoundException
                or DirectoryNotFoundException
                or IntegrationContractException)
        {
            return Task.FromResult(CreateCancellationReceipt(
                request,
                TcpIntegrationCancellationStatus.NotFound,
                idempotent: true));
        }

        if (handoff.MessageId != request.HandoffMessageId)
        {
            return Task.FromResult(CreateCancellationReceipt(
                request,
                TcpIntegrationCancellationStatus.Rejected,
                idempotent: false));
        }

        var resultPath = Path.Combine(
            LocalExchangeRoot,
            IntegrationTransactionLayout.TransactionsDirectoryName,
            request.TransactionId.ToString("D"),
            IntegrationTransactionLayout.ResultFileName);
        if (File.Exists(resultPath))
        {
            try
            {
                var result = TwoDIntegrationExchange.ReadResult(
                    LocalExchangeRoot,
                    request.TransactionId);
                var status = result.Status switch
                {
                    IntegrationResultStatus.Completed =>
                        TcpIntegrationCancellationStatus.AlreadyCompleted,
                    IntegrationResultStatus.Failed =>
                        TcpIntegrationCancellationStatus.AlreadyFailed,
                    IntegrationResultStatus.Cancelled =>
                        TcpIntegrationCancellationStatus.AlreadyCancelled,
                    _ => TcpIntegrationCancellationStatus.Rejected
                };
                return Task.FromResult(CreateCancellationReceipt(
                    request,
                    status,
                    idempotent: true));
            }
            catch (IntegrationContractException)
            {
                return Task.FromResult(CreateCancellationReceipt(
                    request,
                    TcpIntegrationCancellationStatus.Rejected,
                    idempotent: false));
            }
        }

        if (!_activeRunCancellations.TryGetValue(
                request.TransactionId,
                out var runCancellation))
        {
            return Task.FromResult(CreateCancellationReceipt(
                request,
                TcpIntegrationCancellationStatus.NotFound,
                idempotent: true));
        }

        if (runCancellation.IsCancellationRequested)
        {
            return Task.FromResult(CreateCancellationReceipt(
                request,
                TcpIntegrationCancellationStatus.AlreadyCancelled,
                idempotent: true));
        }

        runCancellation.Cancel();
        return Task.FromResult(CreateCancellationReceipt(
            request,
            TcpIntegrationCancellationStatus.Accepted,
            idempotent: false));
    }

    private static TcpIntegrationCancellationReceipt CreateCancellationReceipt(
        IntegrationCancelRequestV2 request,
        TcpIntegrationCancellationStatus status,
        bool idempotent) =>
        new(
            request.RequestId,
            request.TransactionId,
            status,
            idempotent,
            IntegrationApplicationIds.TwoDStudio);
}
