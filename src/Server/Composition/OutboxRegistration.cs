using GameNet.Server.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace GameNet.Server.Composition;

public static class OutboxRegistration
{
    public static IServiceCollection AddGameNetOutbox(this IServiceCollection services)
    {
        services.AddOptions<OutboxDeliveryOptions>()
            .BindConfiguration(OutboxDeliveryOptions.SectionName);

        services.AddScoped<IOutboxWriter, EfOutboxWriter>();
        services.AddScoped<IOutboxDispatcher, EfOutboxDispatcher>();
        services.AddScoped<OutboxDeliveryProcessor>();
        services.AddSingleton<IOutboxDeliveryRegistry, OutboxDeliveryRegistry>();
        services.AddHostedService<OutboxDeliveryWorker>();

        return services;
    }
}