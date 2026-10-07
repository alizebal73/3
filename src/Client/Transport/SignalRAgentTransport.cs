using GameNet.Shared.Contracts.V1.Protocol;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace GameNet.Agent.Transport;

public sealed class SignalRAgentTransport(
    IOptions<AgentTransportOptions> options,
    ILogger<SignalRAgentTransport> logger) : IAgentTransport
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private HubConnection? _connection;
    private AgentIdentity? _identity;
    private string? _leaseToken;

    public bool IsConnected =>
        _connection?.State == HubConnectionState.Connected &&
        !string.IsNullOrWhiteSpace(_leaseToken);

    public async Task ConnectAsync(
        AgentIdentity identity,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ServerBaseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.AccessToken);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            _identity = identity;

            if (_connection is null)
            {
                var hubUrl = new Uri(
                    $"{options.Value.ServerBaseUrl.TrimEnd('/')}/hubs/agent",
                    UriKind.Absolute);

                _connection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, builder =>
                    {
                        builder.AccessTokenProvider = () =>
                            Task.FromResult<string?>(options.Value.AccessToken);
                    })
                    .WithAutomaticReconnect(new[]
                    {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30)
                    })
                    .Build();

                _connection.Reconnecting += error =>
                {
                    _leaseToken = null;
                    logger.LogWarning(
                        error,
                        "Agent transport is reconnecting. DeviceId={DeviceId}",
                        identity.DeviceId);
                    return Task.CompletedTask;
                };

                _connection.Reconnected += _ =>
                {
                    _leaseToken = null;
                    logger.LogInformation(
                        "Agent transport reconnected; lease will be reacquired before the next heartbeat. DeviceId={DeviceId}",
                        identity.DeviceId);
                    return Task.CompletedTask;
                };

                _connection.Closed += error =>
                {
                    _leaseToken = null;
                    logger.LogWarning(
                        error,
                        "Agent transport closed. DeviceId={DeviceId}",
                        identity.DeviceId);
                    return Task.CompletedTask;
                };
            }

            if (_connection.State == HubConnectionState.Disconnected)
                await _connection.StartAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(_leaseToken))
                await AcquireLeaseAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> HeartbeatAsync(
        AgentHeartbeat heartbeat,
        CancellationToken cancellationToken = default)
    {
        var connection = _connection
            ?? throw new InvalidOperationException("Agent transport is not initialized.");

        if (!IsConnected)
        {
            await ConnectAsync(
                _identity ?? throw new InvalidOperationException("Agent identity is not initialized."),
                cancellationToken);
        }

        try
        {
            return await connection.InvokeAsync<bool>(
                "HeartbeatAsync",
                heartbeat,
                cancellationToken);
        }
        catch (HubException)
        {
            _leaseToken = null;
            throw;
        }
    }

    public async Task<ReconciliationResponse> ReconcileAsync(
        string deviceId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            await ConnectAsync(
                _identity ?? throw new InvalidOperationException("Agent identity is not initialized."),
                cancellationToken);
        }

        var connection = _connection
            ?? throw new InvalidOperationException("Agent transport is not initialized.");

        return await connection.InvokeAsync<ReconciliationResponse>(
            "ReconcileAsync",
            new ReconciliationRequest(deviceId, DateTimeOffset.UtcNow, reason),
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();

        _gate.Dispose();
    }

    private async Task AcquireLeaseAsync(CancellationToken cancellationToken)
    {
        var identity = _identity
            ?? throw new InvalidOperationException("Agent identity is not initialized.");

        var connection = _connection
            ?? throw new InvalidOperationException("Agent transport is not initialized.");

        var lease = await connection.InvokeAsync<AgentConnectionLeaseState>(
            "ConnectAsync",
            new AgentConnectionLeaseRequest(
                identity.DeviceId,
                connection.ConnectionId ?? string.Empty,
                DateTimeOffset.UtcNow),
            cancellationToken);

        if (!lease.IsAuthoritative)
        {
            throw new InvalidOperationException(
                $"Server did not grant authoritative Agent lease for DeviceId={identity.DeviceId}.");
        }

        _leaseToken = lease.LeaseToken;
    }
}
