namespace GameNet.Shared.Contracts.V1.Api;

public sealed record ApiEnvelope<T>(
    string ContractVersion,
    string CorrelationId,
    string? OperationId,
    DateTimeOffset ServerTimeUtc,
    T Data);
