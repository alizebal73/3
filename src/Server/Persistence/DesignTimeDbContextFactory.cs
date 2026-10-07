using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameNet.Server.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GameNetDbContext>
{
    public GameNetDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("GAMENET_DATABASE")
            ?? "Host=localhost;Port=5432;Database=gamenet;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<GameNetDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new GameNetDbContext(options);
    }
}
