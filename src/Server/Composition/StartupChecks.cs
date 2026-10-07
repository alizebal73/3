using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Hosting;
using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Composition;

public static class StartupChecks
{
    public static async Task ValidateAsync(WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<GameNetOptions>>().Value;

        Directory.CreateDirectory(Path.GetFullPath(options.DataRoot));
        Directory.CreateDirectory(Path.GetFullPath(options.BackupRoot));

        if (app.Environment.IsProduction() &&
            string.IsNullOrWhiteSpace(options.DatabaseConnectionString))
        {
            throw new InvalidOperationException(
                "GameNet:DatabaseConnectionString is required in Production.");
        }

        if (app.Environment.IsProduction() && !options.Authentication.Enabled)
        {
            throw new InvalidOperationException(
                "GameNet:Authentication must be enabled in Production.");
        }

        if (!string.IsNullOrWhiteSpace(options.DatabaseConnectionString))
        {
            var db = app.Services.GetRequiredService<GameNetDbContext>();

            if (!await db.Database.CanConnectAsync())
                throw new InvalidOperationException("Configured PostgreSQL database is not reachable.");

            var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToArray();
            if (pendingMigrations.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Database schema is behind the application. Pending migrations: {string.Join(", ", pendingMigrations)}");
            }
        }

        app.Services.GetRequiredService<ServerReadiness>().MarkReady();
    }
}
