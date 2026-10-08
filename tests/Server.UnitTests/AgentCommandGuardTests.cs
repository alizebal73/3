using GameNet.Server.Infrastructure.Idempotency;
using GameNet.Server.Infrastructure.Realtime;
using GameNet.Server.Infrastructure.Time;
using GameNet.Shared.Contracts.V1.Api;
using GameNet.Shared.Contracts.V1.Protocol;
using GameNet.Shared.Primitives;

namespace GameNet.Server.UnitTests;

public sealed class AgentCommandGuardTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 10, 8, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Expired_command_is_rejected_before_idempotency_claim()
    {
        var idempotency = new RecordingIdempotencyStore();
        var guard = CreateGuard(
            new AgentConnectionLeaseState(
                "PC-01",
                "conn-1",
                "lease-1",
                Now.AddMinutes(1),
                true),
            idempotency);

        var admission = await guard.AdmitAsync(CreateCommand(
            expiresAtUtc: Now.AddSeconds(-1)));

        Assert.False(admission.Accepted);
        Assert.Equal(ApiErrorCodes.AgentCommandExpired, admission.ErrorCode);
        Assert.Equal(0, idempotency.TryClaimCalls);
    }

    [Fact]
    public async Task Stale_lease_is_rejected_before_idempotency_claim()
    {
        var idempotency = new RecordingIdempotencyStore();
        var guard = CreateGuard(
            new AgentConnectionLeaseState(
                "PC-01",
                "conn-1",
                "new-lease",
                Now.AddMinutes(1),
                true),
            idempotency);

        var admission = await guard.AdmitAsync(CreateCommand());

        Assert.False(admission.Accepted);
        Assert.Equal(ApiErrorCodes.AgentLeaseNotAuthoritative, admission.ErrorCode);
        Assert.Equal(0, idempotency.TryClaimCalls);
    }

    [Fact]
    public async Task Current_lease_and_new_command_are_accepted()
    {
        var idempotency = new RecordingIdempotencyStore
        {
            Next = new IdempotencyClaim(
                Acquired: true,
                Completed: false,
                InProgress: false,
                LeaseToken: "idempotency-lease",
                StatusCode: null,
                ResponseJson: null)
        };

        var guard = CreateGuard(
            new AgentConnectionLeaseState(
                "PC-01",
                "conn-1",
                "lease-1",
                Now.AddMinutes(1),
                true),
            idempotency);

        var admission = await guard.AdmitAsync(CreateCommand());

        Assert.True(admission.Accepted);
        Assert.False(admission.Duplicate);
        Assert.False(admission.InProgress);
        Assert.Equal("idempotency-lease", admission.IdempotencyLeaseToken);
        Assert.Equal(1, idempotency.TryClaimCalls);
    }

    [Fact]
    public async Task Completed_command_is_rejected_as_replay()
    {
        var idempotency = new RecordingIdempotencyStore
        {
            Next = new IdempotencyClaim(
                Acquired: false,
                Completed: true,
                InProgress: false,
                LeaseToken: string.Empty,
                StatusCode: 200,
                ResponseJson: "{}")
        };

        var guard = CreateGuard(
            new AgentConnectionLeaseState(
                "PC-01",
                "conn-1",
                "lease-1",
                Now.AddMinutes(1),
                true),
            idempotency);

        var admission = await guard.AdmitAsync(CreateCommand());

        Assert.False(admission.Accepted);
        Assert.True(admission.Duplicate);
        Assert.Equal(ApiErrorCodes.AgentCommandReplay, admission.ErrorCode);
        Assert.Equal(202, admission.ReplayStatusCode);
        Assert.Equal("{\"accepted\":true}", admission.ReplayResponseJson);
    }

    [Fact]
    public async Task In_progress_command_is_not_executed_twice()
    {
        var idempotency = new RecordingIdempotencyStore
        {
            Next = new IdempotencyClaim(
                Acquired: false,
                Completed: false,
                InProgress: true,
                LeaseToken: string.Empty,
                StatusCode: null,
                ResponseJson: null)
        };

        var guard = CreateGuard(
            new AgentConnectionLeaseState(
                "PC-01",
                "conn-1",
                "lease-1",
                Now.AddMinutes(1),
                true),
            idempotency);

        var admission = await guard.AdmitAsync(CreateCommand());

        Assert.False(admission.Accepted);
        Assert.True(admission.InProgress);
        Assert.Equal(ApiErrorCodes.AgentCommandInProgress, admission.ErrorCode);
    }

    private static AgentCommandGuard CreateGuard(
        AgentConnectionLeaseState lease,
        RecordingIdempotencyStore idempotency) =>
        new(
            new FakeLeaseStore(lease),
            idempotency,
            new FixedClock(Now));

    private static AgentCommand<string> CreateCommand(
        DateTimeOffset? expiresAtUtc = null) =>
        new(
            CommandId.New(),
            "PC-01",
            "lease-1",
            Now,
            expiresAtUtc ?? Now.AddMinutes(1),
            AgentCommandTypes.LaunchGame,
            "payload");

    private sealed class FixedClock(DateTimeOffset now) : IGameClock
    {
        public DateTimeOffset UtcNow => now;
        public DateTimeOffset LocalNow => now;
        public DateOnly BusinessDate => DateOnly.FromDateTime(now.Date);
    }

    private sealed class FakeLeaseStore(
        AgentConnectionLeaseState? current) : IAgentConnectionLeaseStore
    {
        public Task<AgentConnectionLeaseState?> TryAcquireAsync(
            AgentConnectionLeaseRequest request,
            TimeSpan leaseDuration,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(current);

        public Task<AgentConnectionLeaseState?> GetCurrentAsync(
            string deviceId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(current);

        public Task<bool> RenewAsync(
            string deviceId,
            string connectionId,
            string leaseToken,
            TimeSpan leaseDuration,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task<bool> RecordHeartbeatAsync(
            AgentHeartbeat heartbeat,
            string connectionId,
            string leaseToken,
            TimeSpan leaseDuration,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task ReleaseIfOwnerAsync(
            string deviceId,
            string connectionId,
            string leaseToken,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> IsCurrentOwnerAsync(
            string deviceId,
            string connectionId,
            string leaseToken,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(false);
    }

    private sealed class RecordingIdempotencyStore : IIdempotencyStore
    {
        public int TryClaimCalls { get; private set; }

        public IdempotencyClaim Next { get; set; } =
            new(
                Acquired: false,
                Completed: false,
                InProgress: true,
                LeaseToken: string.Empty,
                StatusCode: null,
                ResponseJson: null);

        public Task<IdempotencyClaim> TryClaimAsync(
            string scope,
            string key,
            string operation,
            DateTimeOffset leaseExpiresAtUtc,
            DateTimeOffset expiresAtUtc,
            CancellationToken cancellationToken = default)
        {
            TryClaimCalls++;
            return Task.FromResult(Next);
        }

        public Task CompleteAsync(
            string scope,
            string key,
            string leaseToken,
            int statusCode,
            string responseJson,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
