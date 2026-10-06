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

        if (!string.IsNullOrWhiteSpace(options.DatabaseConnectionString))
        {
            var db = app.Services.GetRequiredService<GameNetDbContext>();

            if (!await db.Database.CanConnectAsync())
                throw new InvalidOperationException("Configured PostgreSQL database is not reachable.");
        }

        app.Services.GetRequiredService<ServerReadiness>().MarkReady();
    }
}
