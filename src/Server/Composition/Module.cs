namespace GameNet.Server.Composition;

public interface IGameNetModule
{
    string Name { get; }
    void AddServices(IServiceCollection services);
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
