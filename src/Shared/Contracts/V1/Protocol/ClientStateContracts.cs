namespace GameNet.Shared.Contracts.V1.Protocol;

public enum AgentClientLockState
{
    Unknown,
    Unlocked,
    Locked,
    Maintenance
}

public sealed record ClientCapabilitySnapshot(
    string DeviceId,
    string AgentVersion,
    AgentClientLockState LockState,
    bool CanLaunchGames,
    bool CanStopGames,
    bool CanRestart,
    bool CanShutdown,
    bool CanCollectDiagnostics);

public sealed record GameRuntimeObservation(
    string? GameId,
    string? SessionId,
    string? GameAccountAllocationId,
    string? ProcessExecutionId,
    DateTimeOffset ObservedAtUtc,
    string State);

public sealed record ClientPolicySnapshot(
    string DeviceId,
    string PolicyVersion,
    IReadOnlyDictionary<string, string> Values,
    DateTimeOffset AppliedAtUtc);
