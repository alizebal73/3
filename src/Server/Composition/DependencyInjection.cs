using GameNet.Server.Infrastructure.Audit;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Idempotency;
using GameNet.Server.Infrastructure.Jobs;
using GameNet.Server.Infrastructure.Outbox;
using GameNet.Server.Infrastructure.Realtime;
using GameNet.Server.Infrastructure.Security;
using GameNet.Server.Infrastructure.Time;
using GameNet.Server.Infrastructure.Transactions;
using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Composition;

public static class DependencyInjection
{
    public static IServiceCollection AddGameNet(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services
            .AddOptions<GameNetOptions>()
            .BindConfiguration(GameNetOptions.SectionName)
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<GameNetOptions>, GameNetOptionsValidator>();
        services.AddHttpContextAccessor();

        services.AddSingleton<IGameClock, GameClock>();
        services.AddSingleton<ModuleRegistry>();
        services.AddSingleton<Server.Infrastructure.Hosting.ServerReadiness>();

        services.AddScoped<ICurrentActor, HttpCurrentActor>();
        services.AddScoped<IAuditWriter, EfAuditWriter>();
        services.AddScoped<IIdempotencyStore, EfIdempotencyStore>();
        services.AddGameNetOutbox();
        services.AddScoped<ITransactionCoordinator, EfTransactionCoordinator>();
        services.AddScoped<IAgentConnectionLeaseStore, EfAgentConnectionLeaseStore>();

        services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();
        services.AddHostedService<BackgroundJobDispatcher>();
        services.AddHealthChecks();

        services.AddGameNetAuthorization();

        services.AddDbContext<GameNetDbContext>((provider, db) =>
        {
            var configuration = provider
                .GetRequiredService<IOptions<GameNetOptions>>()
                .Value;

            var connectionString = configuration.DatabaseConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
                return;

            db.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        return services;
    }
}
