using GameNet.Server.Composition;
using GameNet.Server.Modules.Stations.Application;
using GameNet.Server.Modules.Stations.Infrastructure;
using GameNet.Shared.Contracts.V1.Stations;

namespace GameNet.Server.Modules.Stations.Api;

public sealed class StationsModule : IGameNetModule
{
    public string Name => "Stations";

    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<StationService>();
        services.AddScoped<IStationRepository, EfStationRepository>();
        services.AddSingleton<StationsApiMapper>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/stations")
            .WithTags("Stations");

        group.MapGet("/", async (
            StationService service,
            CancellationToken cancellationToken) =>
            Results.Ok(await service.ListAsync(cancellationToken)));

        group.MapPost("/", async (
            CreateStationRequest request,
            StationService service,
            CancellationToken cancellationToken) =>
        {
            try
            {
                return Results.Created(
                    "/api/v1/stations",
                    await service.CreateAsync(request, cancellationToken));
            }
            catch (InvalidOperationException ex) when (ex.Message == "STATION_CODE_EXISTS")
            {
                return Results.Conflict(new { code = "STATION_CODE_EXISTS" });
            }
        });

        group.MapPatch("/{id:guid}/name", async (
            Guid id,
            RenameStationRequest request,
            StationService service,
            CancellationToken cancellationToken) =>
        {
            try
            {
                return Results.Ok(
                    await service.RenameAsync(id, request, cancellationToken));
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { code = "STATION_NOT_FOUND" });
            }
        });

        group.MapPatch("/{id:guid}/state", async (
            Guid id,
            SetStationStateRequest request,
            StationService service,
            CancellationToken cancellationToken) =>
        {
            try
            {
                return Results.Ok(
                    await service.SetStateAsync(id, request, cancellationToken));
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound(new { code = "STATION_NOT_FOUND" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { code = "STATION_INVALID_STATE", detail = ex.Message });
            }
        });
    }
}

public sealed class StationsApiMapper
{
}