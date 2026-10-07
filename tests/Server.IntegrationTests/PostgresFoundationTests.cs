using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.IntegrationTests;

[Trait("Category", "Postgres")]
public sealed class PostgresFoundationTests
{
    [Fact]
    public async Task Real_postgresql_has_a_clean_migration_state()
    {
        var connection = Environment.GetEnvironmentVariable("GAMENET_TEST_DATABASE");
        Assert.False(string.IsNullOrWhiteSpace(connection), "GAMENET_TEST_DATABASE is required for PostgreSQL certification.");

        var options = new DbContextOptionsBuilder<GameNetDbContext>()
            .UseNpgsql(connection)
            .Options;

        await using var db = new GameNetDbContext(options);

        Assert.True(await db.Database.CanConnectAsync());

        var pending = await db.Database.GetPendingMigrationsAsync();
        Assert.Empty(pending);

        var result = await db.Database.SqlQueryRaw<int>("select 1").SingleAsync();
        Assert.Equal(1, result);
    }
}
