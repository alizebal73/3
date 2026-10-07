using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Time;
using GameNet.Shared.Contracts.V1.Protocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Infrastructure.Realtime;

[Authorize(Policy = "AgentTransport")]
public sealed class AgentHub(
    IAgentConnectionLeaseStore leases,
    IGameClock clock,
    IOptions<GameNetOptions> options) : Hub
{
    private const string LeaseTokenKey = "GameNet.Agent.LeaseToken";
    private const string DeviceIdKey = "GameNet.Agent.DeviceId";

    public async Task<AgentConnectionLeaseState> ConnectAsync(
        AgentConnectionLeaseRequest request)
    {
        var deviceId = RequireDeviceId();
        if (!string.Equals(deviceId, request.DeviceId, StringComparison.Ordinal))
            throw new HubException("Agent device identity does not match the authenticated device.");

        var lease = await leases.TryAcquireAsync(
            request with { ConnectionId = Context.ConnectionId },
            TimeSpan.FromSeconds(options.Value.Agent.LeaseDurationSeconds),
            Context.ConnectionAborted);

        if (lease is null)
            throw new HubException("Agent connection lease could not be acquired.");

        Context.Items[LeaseTokenKey] = lease.LeaseToken;
        Context.Items[DeviceIdKey] = deviceId;

        return lease;
    }

    public async Task<bool> HeartbeatAsync(AgentHeartbeat heartbeat)
    {
        var deviceId = RequireDeviceId();
        var token = RequireLeaseToken();

        if (!string.Equals(deviceId, heartbeat.DeviceId, StringComparison.Ordinal))
            throw new HubException("Agent device identity does not match the authenticated device.");

        return await leases.RecordHeartbeatAsync(
            heartbeat,
            Context.ConnectionId,
            token,
            TimeSpan.FromSeconds(options.Value.Agent.LeaseDurationSeconds),
            Context.ConnectionAborted);
    }

    public Task<AgentReconciliationResponse> ReconcileAsync(
        AgentReconciliationRequest request)
    {
        var deviceId = RequireDeviceId();
        _ = RequireLeaseToken();

        if (!string.Equals(deviceId, request.DeviceId, StringComparison.Ordinal))
            throw new HubException("Agent device identity does not match the authenticated device.");

        return Task.FromResult(
            new AgentReconciliationResponse(
                deviceId,
                clock.UtcNow,
                AgentProtocolVersions.V1,
                true));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(LeaseTokenKey, out var tokenValue) &&
            tokenValue is string token &&
            Context.Items.TryGetValue(DeviceIdKey, out var deviceValue) &&
            deviceValue is string deviceId)
        {
            await leases.ReleaseIfOwnerAsync(
                deviceId,
                Context.ConnectionId,
                token,
                CancellationToken.None);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private string RequireDeviceId() =>
        Context.User.FindFirst("device_id")?.Value
        ?? throw new HubException("Authenticated Agent token has no device_id claim.");

    private string RequireLeaseToken() =>
        Context.Items.TryGetValue(LeaseTokenKey, out var tokenValue) &&
        tokenValue is string token &&
        !string.IsNullOrWhiteSpace(token)
            ? token
            : throw new HubException("Agent connection lease has not been acquired.");
}
