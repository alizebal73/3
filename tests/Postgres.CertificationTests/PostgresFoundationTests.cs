using GameNet.Server.Infrastructure.Audit;
using GameNet.Server.Infrastructure.Backup;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Idempotency;
using GameNet.Server.Infrastructure.Outbox;
using GameNet.Server.Infrastructure.Realtime;
using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Persistence;
using GameNet.Server.Persistence.Entities;
using GameNet.Shared.Contracts.V1.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

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
    public async Task Backup_store_creates_and_verifies_a_real_postgres_backup()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "gamenet-backup-cert",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var options = Options.Create(new GameNetOptions
            {
                DatabaseConnectionString = Connection,
                BackupRoot = root,
                Backup = new BackupOptions()
            });

            var store = new PostgresBackupStore(
                options,
                new FixedClock(DateTimeOffset.UtcNow));

            var artifact = await store.CreateAsync();

            Assert.True(File.Exists(artifact.FilePath));
            Assert.True(artifact.SizeBytes > 0);
            Assert.Matches("^[0-9A-F]{64}$", artifact.Sha256);
            Assert.True(await store.VerifyAsync(artifact));

            File.Delete(artifact.FilePath);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Backup_can_be_restored_into_a_disposable_target()
    {
        var restoreConnection = Environment.GetEnvironmentVariable("GAMENET_RESTORE_DATABASE")
            ?? throw new InvalidOperationException(
                "GAMENET_RESTORE_DATABASE is required for restore certification.");

        var root = Path.Combine(
            Path.GetTempPath(),
            "gamenet-backup-restore-cert",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(root);

        try
        {
            var options = Options.Create(new GameNetOptions
            {
                DatabaseConnectionString = Connection,
                BackupRoot = root,
                Backup = new BackupOptions()
            });

            var store = new PostgresBackupStore(
                options,
                new FixedClock(DateTimeOffset.UtcNow));

            var artifact = await store.CreateAsync();
            await store.RestoreAsync(
                artifact,
                restoreConnection,
                explicitOperatorApproval: true,
                serverStopped: true);

            await using var target = new GameNetDbContext(
                new DbContextOptionsBuilder<GameNetDbContext>()
                    .UseNpgsql(restoreConnection)
                    .UseSnakeCaseNamingConvention()
                    .Options);

            Assert.True(await target.Database.CanConnectAsync());
            var tableExists = await target.Database.SqlQueryRaw<bool>(
                "select exists (select 1 from information_schema.tables where table_name = 'audit_entries') as \"Value\"")
                .SingleAsync();

            Assert.True(tableExists);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

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
    public async Task Idempotency_first_claim_is_owned_by_the_request_that_inserted_it()
    {
        await using var db = new GameNetDbContext(Options());
        await db.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'first-claim';");

        var clock = new FixedClock(DateTimeOffset.UtcNow);
        var store = new EfIdempotencyStore(db, clock);

        var claim = await store.TryClaimAsync(
            "foundation-cert",
            "first-claim",
            "operation-one",
            clock.UtcNow.AddMinutes(5),
            clock.UtcNow.AddHours(1));

        Assert.True(claim.Acquired);
        Assert.False(claim.InProgress);
        Assert.False(claim.Completed);
        Assert.False(string.IsNullOrWhiteSpace(claim.LeaseToken));

        await db.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'first-claim';");
    }

    [Fact]
    public async Task Concurrent_idempotency_claims_produce_one_owner()
    {
        await using (var setup = new GameNetDbContext(Options()))
        {
            await setup.Database.ExecuteSqlRawAsync(
                "delete from idempotency_records where scope = 'foundation-cert' and key = 'same-owner';");
        }

        var now = DateTimeOffset.UtcNow;
        var first = ClaimAsync(now);
        var second = ClaimAsync(now);

        var claims = await Task.WhenAll(first, second);

        Assert.Single(claims, x => x.Acquired);
        Assert.Single(claims, x => x.InProgress);

        await using var cleanup = new GameNetDbContext(Options());
        await cleanup.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'same-owner';");
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
    public async Task Audit_entries_are_append_only_at_the_database_boundary()
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

        await using var mutate = new GameNetDbContext(Options());

        await Assert.ThrowsAsync<PostgresException>(async () =>
            await mutate.Database.ExecuteSqlInterpolatedAsync(
                $"update audit_entries set operation = {"mutated"} where id = {audit.Id};"));

        await Assert.ThrowsAsync<PostgresException>(async () =>
            await mutate.Database.ExecuteSqlInterpolatedAsync(
                $"delete from audit_entries where id = {audit.Id};"));
    }

    [Fact]
    public async Task Expired_idempotency_owner_cannot_complete_after_lease_expiry()
    {
        await using var cleanup = new GameNetDbContext(Options());
        await cleanup.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'expired-complete';");

        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var oldLease = createdAt.AddMinutes(1);

        await cleanup.Database.ExecuteSqlInterpolatedAsync($"""
            insert into idempotency_records
                (scope, key, operation, state, lease_token, status_code, response_json,
                 created_at_utc, lease_expires_at_utc, expires_at_utc)
            values
                ({"foundation-cert"}, {"expired-complete"}, {"operation-one"}, {"processing"},
                 {"old-token"}, {0}, {("{}")}, {createdAt}, {oldLease}, {createdAt.AddHours(1)});
            """);

        await using var db = new GameNetDbContext(Options());
        var store = new EfIdempotencyStore(
            db,
            new FixedClock(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            store.CompleteAsync(
                "foundation-cert",
                "expired-complete",
                "old-token",
                200,
                "{}"));

        await cleanup.Database.ExecuteSqlRawAsync(
            "delete from idempotency_records where scope = 'foundation-cert' and key = 'expired-complete';");
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

    [Fact]
    public async Task Agent_connection_lease_fences_competing_connections()
    {
        await using var cleanup = new GameNetDbContext(Options());
        await cleanup.Database.ExecuteSqlRawAsync(
            "delete from agent_connection_leases where device_id = 'foundation-device';");

        var now = DateTimeOffset.UtcNow;
        var first = new EfAgentConnectionLeaseStore(new GameNetDbContext(Options()), new FixedClock(now));
        var second = new EfAgentConnectionLeaseStore(new GameNetDbContext(Options()), new FixedClock(now));

        var results = await Task.WhenAll(
            first.TryAcquireAsync(
                new AgentConnectionLeaseRequest("foundation-device", "connection-a", now),
                TimeSpan.FromMinutes(1)),
            second.TryAcquireAsync(
                new AgentConnectionLeaseRequest("foundation-device", "connection-b", now),
                TimeSpan.FromMinutes(1)));

        Assert.Single(results, x => x.IsAuthoritative);
        Assert.Single(results, x => !x.IsAuthoritative);

        await cleanup.Database.ExecuteSqlRawAsync(
            "delete from agent_connection_leases where device_id = 'foundation-device';");
    }

    private static async Task<IdempotencyClaim> ClaimAsync(DateTimeOffset now)
    {
        await using var db = new GameNetDbContext(Options());
        var store = new EfIdempotencyStore(db, new FixedClock(now));

        return await store.TryClaimAsync(
            "foundation-cert",
            "same-owner",
            "operation-one",
            now.AddMinutes(5),
            now.AddHours(1));
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
                ({"foundation-cert"}, {key}, {operation}, {"processing"}, {Guid.NewGuid().ToString("N")},
                 {0}, {("{}")}, {DateTimeOffset.UtcNow}, {DateTimeOffset.UtcNow.AddMinutes(5)},
                 {DateTimeOffset.UtcNow.AddHours(1)})
            on conflict (scope, key) do nothing;""");
    }

    private sealed class FixedClock(DateTimeOffset now) : IGameClock
    {
        public DateTimeOffset UtcNow => now;
        public DateTimeOffset LocalNow => now;
        public DateOnly BusinessDate => DateOnly.FromDateTime(now.Date);
    }
}
