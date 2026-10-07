using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.Infrastructure.Idempotency;

public sealed class EfIdempotencyStore(
    GameNetDbContext dbContext,
    IGameClock clock) : IIdempotencyStore
{
    public async Task<IdempotencyClaim> TryClaimAsync(
        string scope,
        string key,
        string operation,
        DateTimeOffset leaseExpiresAtUtc,
        DateTimeOffset expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        var now = clock.UtcNow;
        var token = Guid.NewGuid().ToString("N");

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO idempotency_records
                (scope, key, operation, state, lease_token, status_code, response_json, created_at_utc, lease_expires_at_utc, expires_at_utc)
            VALUES
                ({scope}, {key}, {operation}, {"processing"}, {token}, {0}, CAST({"{}"} AS jsonb), {now}, {leaseExpiresAtUtc}, {expiresAtUtc})
            ON CONFLICT (scope, key) DO NOTHING
            """,
            cancellationToken);

        var record = await dbContext.IdempotencyRecords
            .SingleAsync(x => x.Scope == scope && x.Key == key, cancellationToken);

        if (!string.Equals(record.Operation, operation, StringComparison.Ordinal))
            throw new InvalidOperationException(
                "The idempotency key is already associated with a different operation.");

        if (record.State == "completed" && record.ExpiresAtUtc > now)
        {
            return new IdempotencyClaim(false, true, false, string.Empty, record.StatusCode, record.ResponseJson);
        }

        if (record.State == "processing" && record.LeaseExpiresAtUtc > now)
        {
            return new IdempotencyClaim(false, false, true, string.Empty, null, null);
        }

        var claimed = await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            UPDATE idempotency_records
            SET state = {"processing"},
                lease_token = {token},
                operation = {operation},
                lease_expires_at_utc = {leaseExpiresAtUtc},
                expires_at_utc = {expiresAtUtc}
            WHERE scope = {scope}
              AND key = {key}
              AND (
                    (state = {"processing"} AND lease_expires_at_utc <= {now})
                 OR (state = {"completed"} AND expires_at_utc <= {now})
              )
            """,
            cancellationToken);

        return claimed == 1
            ? new IdempotencyClaim(true, false, false, token, null, null)
            : new IdempotencyClaim(false, false, true, string.Empty, null, null);
    }

    public async Task CompleteAsync(
        string scope,
        string key,
        string leaseToken,
        int statusCode,
        string responseJson,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(leaseToken);
        ArgumentNullException.ThrowIfNull(responseJson);

        var updated = await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            UPDATE idempotency_records
            SET state = {"completed"},
                status_code = {statusCode},
                response_json = CAST({responseJson} AS jsonb),
                lease_token = {""},
                lease_expires_at_utc = {clock.UtcNow}
            WHERE scope = {scope}
              AND key = {key}
              AND lease_token = {leaseToken}
            """,
            cancellationToken);

        if (updated != 1)
            throw new InvalidOperationException("Idempotency lease is no longer owned.");
    }
}
