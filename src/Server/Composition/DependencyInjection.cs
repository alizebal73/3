using GameNet.Server.Infrastructure;

namespace GameNet.Server.Composition;

public static class DependencyInjection
{
    public static IServiceCollection AddGameNet(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<StartupState>();
        services.AddHealthChecks();
        return services;
    }
}
