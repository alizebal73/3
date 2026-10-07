namespace GameNet.Shared.Contracts.V1.Api;

public sealed record ApiError(
    string Code,
    string MessageKey,
    string CorrelationId,
    string? OperationId,
    bool Retryable,
    IReadOnlyList<ApiValidationError>? Validation = null);

public sealed record ApiValidationError(
    string Field,
    string Code,
    string MessageKey);
