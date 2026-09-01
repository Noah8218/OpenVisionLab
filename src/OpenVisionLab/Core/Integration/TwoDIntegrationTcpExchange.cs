#nullable enable

using System;
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
        return TwoDIntegrationExchange.RunAcceptedHandoffAsync(
            LocalExchangeRoot,
            transactionId,
            stepTimeoutMilliseconds,
            cancellationToken);
    }

    internal Task<IntegrationResultV2> RunAcceptedHandoffAsync(
        Guid transactionId,
        string runtimeBuildManifestPath,
        int stepTimeoutMilliseconds = 60000,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return TwoDIntegrationExchange.RunAcceptedHandoffAsync(
            LocalExchangeRoot,
            transactionId,
            runtimeBuildManifestPath,
            stepTimeoutMilliseconds,
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
}
