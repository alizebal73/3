using GameNet.Server.Infrastructure.Audit;
using GameNet.Server.Infrastructure.Idempotency;
using GameNet.Server.Infrastructure.Outbox;
using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Persistence;
using GameNet.Server.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Postgres.CertificationTests;

public sealed class PostgresFoundationTests
{
    private static string Connection =>
        Environment.GetEnvironmentVariable("GAMENET_TEST_DATABASE")
        ?? throw new InvalidOperationException("GAMENET_TEST_DATABASE is required.");

    private static DbContextOptions<GameNetDbContext> Options() =>
        new DbContextOptionsBuilder<GameNetDbContext>()
            .UseNpgsql(Connection)
            .UseSnakeCaseNamingConvention()
            .Options;

    [Fact]
    public async Task Clean_database_is_reachable_and_migrated()
    {
        await using var db = new GameNetDbContext(Options());

        Assert.True(await db.Database.CanConnectAsync());
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());

        var result = await db.Database
            .SqlQueryRaw<int>("select 1 as \"Value\"")
            .SingleAsync();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Idempotency_scope_and_key_are_unique_under_concurrent_insert()
    {
        await using var setup = new GameNetDbContext(Options());
        await setup.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'same-key';");

        var first = InsertIdempotencyAsync();
        var second = InsertIdempotencyAsync();
        await Task.WhenAll(first, second);

        await using var verify = new GameNetDbContext(Options());
        var count = await verify.IdempotencyRecords.CountAsync(
            x => x.Scope == "foundation-cert" && x.Key == "same-key");

        Assert.Equal(1, count);

        await verify.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'same-key';");
    }

    [Fact]
    public async Task Idempotency_key_cannot_be_reused_for_a_different_operation()
    {
        await using var setup = new GameNetDbContext(Options());
        await setup.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'operation-key';");

        await InsertIdempotencyAsync("operation-key", "operation-one");

        await using var contender = new GameNetDbContext(Options());
        var clock = new FixedClock(DateTimeOffset.UtcNow);
        var store = new EfIdempotencyStore(contender, clock);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            store.TryClaimAsync(
                "foundation-cert",
                "operation-key",
                "operation-two",
                clock.UtcNow.AddMinutes(5),
                clock.UtcNow.AddHours(1)));

        await contender.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'operation-key';");
    }

    [Fact]
    public async Task Audit_entries_are_append_only()
    {
        var audit = new AuditEntry
        {
            OccurredAtUtc = DateTimeOffset.UtcNow,
            ActorType = "test",
            ActorId = "foundation-cert",
            Operation = "append-only-test",
            CorrelationId = "foundation-cert"
        };

        await using (var write = new GameNetDbContext(Options()))
        {
            write.AuditEntries.Add(audit);
            await write.SaveChangesAsync();
        }

        await using (var mutate = new GameNetDbContext(Options()))
        {
            var loaded = await mutate.AuditEntries.SingleAsync(x => x.Id == audit.Id);
            mutate.Entry(loaded).Property(x => x.Operation).CurrentValue = "mutated";

            Assert.Throws<InvalidOperationException>(() => mutate.SaveChanges());
            mutate.Entry(loaded).State = EntityState.Unchanged;
        }

        await using var cleanup = new GameNetDbContext(Options());
        await cleanup.Database.ExecuteSqlInterpolatedAsync(
            $"delete from audit_entries where id = {audit.Id};");
    }

    [Fact]
    public async Task Outbox_claims_are_not_duplicated_across_two_dispatchers()
    {
        await using (var cleanup = new GameNetDbContext(Options()))
        {
            await cleanup.Database.ExecuteSqlRawAsync(
                "delete from outbox_messages where type = 'foundation-cert';");

            cleanup.OutboxMessages.AddRange(
                new OutboxMessage
                {
                    Type = "foundation-cert",
                    PayloadJson = "{}",
                    OccurredAtUtc = DateTimeOffset.UtcNow.AddMinutes(-2)
                },
                new OutboxMessage
                {
                    Type = "foundation-cert",
                    PayloadJson = "{}",
                    OccurredAtUtc = DateTimeOffset.UtcNow.AddMinutes(-1)
                });

            await cleanup.SaveChangesAsync();
        }

        var clock = new FixedClock(DateTimeOffset.UtcNow);
        var first = new EfOutboxDispatcher(new GameNetDbContext(Options()), clock);
        var second = new EfOutboxDispatcher(new GameNetDbContext(Options()), clock);

        var claims = await Task.WhenAll(
            first.ClaimBatchAsync(2, TimeSpan.FromMinutes(1)),
            second.ClaimBatchAsync(2, TimeSpan.FromMinutes(1)));

        var ids = claims.SelectMany(x => x).Select(x => x.Id).ToList();
        Assert.Equal(2, ids.Count);
        Assert.Equal(ids.Count, ids.Distinct().Count());

        await using var verify = new GameNetDbContext(Options());
        await verify.Database.ExecuteSqlRawAsync(
            "delete from outbox_messages where type = 'foundation-cert';");
    }

    private static Task InsertIdempotencyAsync() =>
        InsertIdempotencyAsync("same-key", "test");

    private static async Task InsertIdempotencyAsync(
        string key,
        string operation)
    {
        await using var db = new GameNetDbContext(Options());

        await db.Database.ExecuteSqlInterpolatedAsync($"""
            insert into idempotency_records
                (scope, key, operation, state, lease_token, status_code, response_json, created_at_utc, lease_expires_at_utc, expires_at_utc)
            values
                ({"foundation-cert"}, {key}, {operation}, {"processing"}, {Guid.NewGuid().ToString("N")}, {0}, {("{}")}, {DateTimeOffset.UtcNow}, {DateTimeOffset.UtcNow.AddMinutes(5)}, {DateTimeOffset.UtcNow.AddHours(1)})
            on conflict (scope, key) do nothing;""");
    }

    private sealed class FixedClock(DateTimeOffset now) : IGameClock
    {
        public DateTimeOffset UtcNow => now;
        public DateTimeOffset LocalNow => now;
        public DateOnly BusinessDate => DateOnly.FromDateTime(now.Date);
    }
}
