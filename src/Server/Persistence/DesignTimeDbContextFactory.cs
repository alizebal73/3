using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameNet.Server.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GameNetDbContext>
{
    public GameNetDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("GAMENET_DATABASE");

        if (string.IsNullOrWhiteSpace(connection))
            throw new InvalidOperationException("Set GAMENET_DATABASE before running Entity Framework migration commands.");

        var options = new DbContextOptionsBuilder<GameNetDbContext>()
             .UseNpgsql(connection).UseSnakeCaseNamingConvention()
            .Options;

        return new GameNetDbContext(options);
    }
}
