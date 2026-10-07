using GameNet.Server.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace GameNet.Server.Composition;

public static class OutboxRegistration
{
    public static IServiceCollection AddGameNetOutbox(this IServiceCollection services)
    {
        services.AddScoped<IOutboxWriter, EfOutboxWriter>();
        services.AddScoped<IOutboxDispatcher, EfOutboxDispatcher>();
        return services;
    }
}
