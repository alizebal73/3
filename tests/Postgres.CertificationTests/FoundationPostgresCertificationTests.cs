using Npgsql;
using Xunit;

namespace GameNet.Postgres.CertificationTests;

public sealed class FoundationPostgresCertificationTests
{
    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("GAMENET_TEST_DATABASE")
        ?? throw new SkipException("GAMENET_TEST_DATABASE is not set.");

    [Fact]
    public async Task FoundationSchemaExists()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var count = await ScalarLong(connection, """
            select count(*)
            from information_schema.tables
            where table_schema = 'public'
              and table_name in (
                'audit_entries',
                'idempotency_records',
                'outbox_messages',
                'agent_connection_leases',
                'agent_credentials');
            """);

        Assert.Equal(5, count);
    }

    [Fact]
    public async Task AuditTableRejectsUpdateAndDelete()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var id = Guid.NewGuid();
        await Execute(connection, """
            insert into audit_entries
            (id, occurred_at_utc, actor_type, operation, correlation_id)
            values ($1, now(), 'Certification', 'FoundationProbe', $2);
            """, id, Guid.NewGuid().ToString("N"));

        await Assert.ThrowsAsync<PostgresException>(() =>
            Execute(connection, "update audit_entries set operation='MutationAttempt' where id=$1;", id));

        await Assert.ThrowsAsync<PostgresException>(() =>
            Execute(connection, "delete from audit_entries where id=$1;", id));
    }

    [Fact]
    public async Task IdempotencyScopeAndKeyAreUniqueUnderConcurrency()
    {
        var scope = "foundation-cert-" + Guid.NewGuid().ToString("N");
        var key = Guid.NewGuid().ToString("N");

        async Task<bool> InsertAsync()
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            try
            {
                await Execute(connection, """
                    insert into idempotency_records
                    (scope,key,operation,state,lease_token,status_code,response_json,created_at_utc,lease_expires_at_utc,expires_at_utc)
                    values ($1,$2,'Certification','Completed',$3,200,'{}',now(),now()+interval '1 minute',now()+interval '1 hour');
                    """, scope, key, Guid.NewGuid().ToString("N"));
                return true;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return false;
            }
        }

        var results = await Task.WhenAll(InsertAsync(), InsertAsync());
        Assert.Equal(1, results.Count(static x => x));
    }

    [Fact]
    public async Task OutboxEventIdIsUnique()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var eventId = Guid.NewGuid();
        await Execute(connection, """
            insert into outbox_messages
            (event_id,occurred_at_utc,type,payload_json)
            values ($1,now(),'FoundationProbe','{}');
            """, eventId);

        await Assert.ThrowsAsync<PostgresException>(() => Execute(connection, """
            insert into outbox_messages
            (event_id,occurred_at_utc,type,payload_json)
            values ($1,now(),'FoundationProbeDuplicate','{}');
            """, eventId));
    }

    [Fact]
    public async Task AgentLeaseDeviceIdIsSingleAuthorityKey()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var deviceId = "foundation-cert-" + Guid.NewGuid().ToString("N");
        await Execute(connection, """
            insert into agent_connection_leases
            (device_id,connection_id,lease_token,lease_expires_at_utc,updated_at_utc)
            values ($1,$2,$3,now()+interval '1 minute',now());
            """, deviceId, Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));

        await Assert.ThrowsAsync<PostgresException>(() => Execute(connection, """
            insert into agent_connection_leases
            (device_id,connection_id,lease_token,lease_expires_at_utc,updated_at_utc)
            values ($1,$2,$3,now()+interval '1 minute',now());
            """, deviceId, Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N")));
    }

    private static async Task<long> ScalarLong(NpgsqlConnection connection, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }

    private static async Task Execute(NpgsqlConnection connection, string sql, params object[] values)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        for (var i = 0; i < values.Length; i++)
            command.Parameters.AddWithValue($"p{i + 1}", values[i]);
        await command.ExecuteNonQueryAsync();
    }
}
