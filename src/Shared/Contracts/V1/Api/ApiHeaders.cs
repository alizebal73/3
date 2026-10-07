namespace GameNet.Shared.Contracts.V1.Api;

public static class ApiHeaders
{
    public const string CorrelationId = "X-Correlation-Id";
    public const string OperationId = "X-Operation-Id";
    public const string IdempotencyKey = "X-Idempotency-Key";
    public const string ContractVersion = "X-Contract-Version";
    public const string ETag = "ETag";
    public const string IfMatch = "If-Match";
    public const string IfNoneMatch = "If-None-Match";
}
