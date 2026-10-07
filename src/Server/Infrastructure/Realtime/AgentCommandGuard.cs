using GameNet.Server.Infrastructure.Idempotency;
using GameNet.Server.Infrastructure.Time;
using GameNet.Shared.Contracts.V1.Api;
using GameNet.Shared.Contracts.V1.Protocol;

namespace GameNet.Server.Infrastructure.Realtime;

public sealed record AgentCommandAdmission(
    bool Accepted,
    bool Duplicate,
    bool InProgress,
    string? ErrorCode,
    string? IdempotencyLeaseToken);

public sealed class AgentCommandGuard(
    IAgentConnectionLeaseStore leases,
    IIdempotencyStore idempotency,
    IGameClock clock)
{
    private static readonly TimeSpan ClaimLease = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan RecordRetention = TimeSpan.FromHours(1);

    public async Task<AgentCommandAdmission> AdmitAsync<TPayload>(
        AgentCommand<TPayload> command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var now = clock.UtcNow;

        if (string.IsNullOrWhiteSpace(command.DeviceId) ||
            string.IsNullOrWhiteSpace(command.LeaseToken) ||
            string.IsNullOrWhiteSpace(command.Type))
        {
            return Rejected(ApiErrorCodes.Validation);
        }

        if (command.CommandId.Value == Guid.Empty)
            return Rejected(ApiErrorCodes.Validation);

        if (command.ExpiresAtUtc <= now ||
            command.ExpiresAtUtc <= command.SentAtUtc)
        {
            return Rejected(ApiErrorCodes.AgentCommandExpired);
        }

        var lease = await leases.GetCurrentAsync(
            command.DeviceId,
            cancellationToken);

        if (lease is null ||
            lease.LeaseExpiresAtUtc <= now ||
            !string.Equals(
                lease.LeaseToken,
                command.LeaseToken,
                StringComparison.Ordinal))
        {
            return Rejected(ApiErrorCodes.AgentLeaseNotAuthoritative);
        }

        var key = command.CommandId.Value.ToString("N");
        var leaseExpires = now.Add(ClaimLease);
        if (leaseExpires > command.ExpiresAtUtc)
            leaseExpires = command.ExpiresAtUtc;

        var expires = command.ExpiresAtUtc.Add(RecordRetention);

        try
        {
            var claim = await idempotency.TryClaimAsync(
                "agent-command",
                key,
                command.Type,
                leaseExpires,
                expires,
                cancellationToken);

            if (claim.Completed)
            {
                return new AgentCommandAdmission(
                    Accepted: false,
                    Duplicate: true,
                    InProgress: false,
                    ErrorCode: ApiErrorCodes.AgentCommandReplay,
                    IdempotencyLeaseToken: null);
            }

            if (claim.InProgress)
            {
                return new AgentCommandAdmission(
                    Accepted: false,
                    Duplicate: false,
                    InProgress: true,
                    ErrorCode: ApiErrorCodes.AgentCommandInProgress,
                    IdempotencyLeaseToken: null);
            }

            if (!claim.Acquired ||
                string.IsNullOrWhiteSpace(claim.LeaseToken))
            {
                return Rejected(ApiErrorCodes.AgentCommandInProgress);
            }

            return new AgentCommandAdmission(
                Accepted: true,
                Duplicate: false,
                InProgress: false,
                ErrorCode: null,
                IdempotencyLeaseToken: claim.LeaseToken);
        }
        catch (InvalidOperationException)
        {
            return Rejected(ApiErrorCodes.AgentCommandReuseConflict);
        }
    }

    public Task CompleteAsync(
        AgentCommandIdempotency command,
        int statusCode,
        string responseJson,
        CancellationToken cancellationToken = default)
    {
        return idempotency.CompleteAsync(
            "agent-command",
            command.CommandId.ToString("N"),
            command.IdempotencyLeaseToken,
            statusCode,
            responseJson,
            cancellationToken);
    }

    private static AgentCommandAdmission Rejected(string code) =>
        new(
            Accepted: false,
            Duplicate: false,
            InProgress: false,
            ErrorCode: code,
            IdempotencyLeaseToken: null);
}

public sealed record AgentCommandIdempotency(
    CommandId CommandId,
    string IdempotencyLeaseToken);
