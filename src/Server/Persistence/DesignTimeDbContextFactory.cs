using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GameNet.Server.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GameNetDbContext>
{
    public GameNetDbContext CreateDbContext(string[] args)
    {
        var connection =
            Environment.GetEnvironmentVariable("GAMENET_DATABASE")
            ?? "Host=localhost;Database=GameNetDesign;Username=GameNetDesign;Password=design-only";

        var options = new DbContextOptionsBuilder<GameNetDbContext>()
             .UseNpgsql(connection).UseSnakeCaseNamingConvention()
            .Options;

        return new GameNetDbContext(options);
    }
}
