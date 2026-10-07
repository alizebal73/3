using GameNet.Shared.Primitives;

namespace GameNet.Shared.Contracts.V1.Protocol;

public static class AgentMessageTypes
{
    public const string Heartbeat = "agent.heartbeat";
    public const string StateSnapshot = "agent.state";
    public const string Command = "server.command";
    public const string CommandAck = "agent.command_ack";
}

public sealed record AgentCommand<TPayload>(
    CommandId CommandId,
    string DeviceId,
    string LeaseToken,
    DateTimeOffset SentAtUtc,
    DateTimeOffset ExpiresAtUtc,
    string Type,
    TPayload Payload);

public sealed record CommandAcknowledgement(
    CommandId CommandId,
    string DeviceId,
    string LeaseToken,
    DateTimeOffset ReceivedAtUtc,
    bool Success,
    bool Duplicate,
    string? ErrorCode);

public sealed record AgentHeartbeat(
    string DeviceId,
    DateTimeOffset SentAtUtc,
    string AgentVersion,
    string StationState);

public sealed record AgentStateSnapshot(
    string DeviceId,
    DateTimeOffset ObservedAtUtc,
    bool Connected,
    string StationState,
    string? SessionId,
    string? CustomerLoginId,
    string? ActiveProcessId);
