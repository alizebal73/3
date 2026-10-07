using System.Data;
using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Persistence;
using GameNet.Shared.Contracts.V1.Protocol;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.Infrastructure.Realtime;

public sealed class EfAgentConnectionLeaseStore(
    GameNetDbContext dbContext,
    IGameClock clock) : IAgentConnectionLeaseStore
{
    public async Task<AgentConnectionLeaseState?> TryAcquireAsync(
        AgentConnectionLeaseRequest request,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        ValidateLeaseDuration(leaseDuration);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var now = clock.UtcNow;
        var token = Guid.NewGuid().ToString("N");
        var expires = now.Add(leaseDuration);

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO agent_connection_leases
                (device_id, connection_id, lease_token, lease_expires_at_utc, updated_at_utc)
            VALUES
                ({request.DeviceId}, {request.ConnectionId}, {token}, {expires}, {now})
            ON CONFLICT (device_id) DO NOTHING
            """,
            cancellationToken);

        var current = await ReadAsync(request.DeviceId, cancellationToken);

        if (current is null)
            throw new InvalidOperationException("Agent connection lease disappeared during acquisition.");

        if (current.LeaseExpiresAtUtc > now &&
            !string.Equals(current.ConnectionId, request.ConnectionId, StringComparison.Ordinal))
        {
            await transaction.CommitAsync(cancellationToken);

            return current with { IsAuthoritative = false };
        }

        if (string.Equals(current.ConnectionId, request.ConnectionId, StringComparison.Ordinal) &&
            string.Equals(current.LeaseToken, token, StringComparison.Ordinal))
        {
            await transaction.CommitAsync(cancellationToken);

            return current with { IsAuthoritative = true };
        }

        var updated = await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            UPDATE agent_connection_leases
            SET connection_id = {request.ConnectionId},
                lease_token = {token},
                lease_expires_at_utc = {expires},
                updated_at_utc = {now}
            WHERE device_id = {request.DeviceId}
              AND (
                    lease_expires_at_utc <= {now}
                 OR connection_id = {request.ConnectionId}
              )
            """,
            cancellationToken);

        if (updated != 1)
        {
            var winner = await ReadAsync(request.DeviceId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return winner is null ? null : winner with { IsAuthoritative = false };
        }

        await transaction.CommitAsync(cancellationToken);

        return new AgentConnectionLeaseState(
            request.DeviceId,
            request.ConnectionId,
            token,
            expires,
            true);
    }

    public async Task<bool> RenewAsync(
        string deviceId,
        string connectionId,
        string leaseToken,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(leaseToken);
        ValidateLeaseDuration(leaseDuration);

        var now = clock.UtcNow;
        var expires = now.Add(leaseDuration);

        var updated = await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            UPDATE agent_connection_leases
            SET lease_expires_at_utc = {expires},
                updated_at_utc = {now}
            WHERE device_id = {deviceId}
              AND connection_id = {connectionId}
              AND lease_token = {leaseToken}
              AND lease_expires_at_utc > {now}
            """,
            cancellationToken);

        return updated == 1;
    }

    public async Task ReleaseIfOwnerAsync(
        string deviceId,
        string connectionId,
        string leaseToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(leaseToken);

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            DELETE FROM agent_connection_leases
            WHERE device_id = {deviceId}
              AND connection_id = {connectionId}
              AND lease_token = {leaseToken}
            """,
            cancellationToken);
    }

    private async Task<AgentConnectionLeaseState?> ReadAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Database
            .SqlQuery<LeaseRow>(
                $"SELECT device_id AS \"DeviceId\", connection_id AS \"ConnectionId\", lease_token AS \"LeaseToken\", lease_expires_at_utc AS \"LeaseExpiresAtUtc\" FROM agent_connection_leases WHERE device_id = {deviceId}")
            .Select(x => new AgentConnectionLeaseState(
                x.DeviceId,
                x.ConnectionId,
                x.LeaseToken,
                x.LeaseExpiresAtUtc,
                false))
            .SingleOrDefaultAsync(cancellationToken);
    }

    private static void ValidateRequest(AgentConnectionLeaseRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.DeviceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ConnectionId);
    }

    private static void ValidateLeaseDuration(TimeSpan leaseDuration)
    {
        if (leaseDuration <= TimeSpan.Zero || leaseDuration > TimeSpan.FromMinutes(30))
            throw new ArgumentOutOfRangeException(nameof(leaseDuration));
    }

    private sealed class LeaseRow
    {
        public string DeviceId { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public string LeaseToken { get; set; } = string.Empty;
        public DateTimeOffset LeaseExpiresAtUtc { get; set; }
    }
}
