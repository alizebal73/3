using GameNet.Shared.Api;

namespace GameNet.Shared.Contracts.Errors;

public sealed record ApiFailure(
    ApiError Error,
    string TraceId,
    string? OperationId = null);
