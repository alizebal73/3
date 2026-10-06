namespace GameNet.Shared.Contracts.V1.Protocol;

public sealed record ReconciliationRequest(
    string DeviceId,
    DateTimeOffset RequestedAtUtc,
    string Reason);

public sealed record ReconciliationResponse(
    string DeviceId,
    DateTimeOffset ServerTimeUtc,
    string AuthoritativeStateHash);

public sealed record CommandExpiry(DateTimeOffset ExpiresAtUtc);
