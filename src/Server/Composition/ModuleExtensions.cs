namespace GameNet.Server.Composition;

public static class ModuleExtensions
{
    public static IServiceCollection AddGameNetModule(
        this IServiceCollection services,
        IGameNetModule module)
    {
        module.AddServices(services);
        services.AddSingleton(module);
        return services;
    }

    public static void MapGameNetModules(
        this IEndpointRouteBuilder endpoints,
        IEnumerable<IGameNetModule> modules)
    {
        foreach (var module in modules)
            module.MapEndpoints(endpoints);
    }
}
