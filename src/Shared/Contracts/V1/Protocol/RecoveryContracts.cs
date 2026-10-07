namespace GameNet.Shared.Contracts.V1.Protocol;

public sealed record AgentReconciliationRequest(
    string DeviceId,
    DateTimeOffset RequestedAtUtc,
    string Reason);

public sealed record AgentReconciliationResponse(
    string DeviceId,
    DateTimeOffset ServerTimeUtc,
    int AgentProtocolVersion,
    bool AuthoritativeConnection);

public sealed record CommandExpiry(DateTimeOffset ExpiresAtUtc);
