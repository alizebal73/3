namespace GameNet.Shared.Contracts.V1.Api;

public static class ApiErrorCodes
{
    public const string Validation = "validation_error";
    public const string Unauthorized = "unauthorized";
    public const string Forbidden = "forbidden";
    public const string NotFound = "not_found";
    public const string Conflict = "conflict";
    public const string ConcurrencyConflict = "concurrency_conflict";
    public const string IdempotencyInProgress = "idempotency_in_progress";
    public const string IdempotencyReplay = "idempotency_replay";
    public const string RateLimited = "rate_limited";
    public const string DependencyUnavailable = "dependency_unavailable";
    public const string Internal = "internal_error";
}
