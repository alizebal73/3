namespace GameNet.Server.Infrastructure.Idempotency;

public sealed record IdempotencyClaim(
    bool Acquired,
    bool Completed,
    bool InProgress,
    string LeaseToken,
    int? StatusCode,
    string? ResponseJson);

public interface IIdempotencyStore
{
    Task<IdempotencyClaim> TryClaimAsync(
        string scope,
        string key,
        string operation,
        DateTimeOffset leaseExpiresAtUtc,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        string scope,
        string key,
        string leaseToken,
        int statusCode,
        string responseJson,
        CancellationToken cancellationToken = default);
}
