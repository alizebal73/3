using GameNet.Shared.Contracts.V1.Stations;
using GameNet.Server.Modules.Stations.Domain;

namespace GameNet.Server.Modules.Stations.Application;

public sealed class StationService(IStationRepository repository)
{
    public async Task<StationDto> CreateAsync(
        CreateStationRequest request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();

        if (await repository.CodeExistsAsync(code, null, cancellationToken))
            throw new InvalidOperationException("STATION_CODE_EXISTS");

        var station = Station.Create(code, request.Name, request.Type);
        await repository.AddAsync(station, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Map(station);
    }

    public async Task<IReadOnlyList<StationDto>> ListAsync(
        CancellationToken cancellationToken) =>
        (await repository.ListAsync(cancellationToken))
            .Select(Map)
            .ToArray();

    public async Task<StationDto> RenameAsync(
        Guid id,
        RenameStationRequest request,
        CancellationToken cancellationToken)
    {
        var station = await GetRequiredAsync(id, cancellationToken);
        station.Rename(request.Name);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(station);
    }

    public async Task<StationDto> SetStateAsync(
        Guid id,
        SetStationStateRequest request,
        CancellationToken cancellationToken)
    {
        var station = await GetRequiredAsync(id, cancellationToken);
        station.SetState(request.State);
        await repository.SaveChangesAsync(cancellationToken);
        return Map(station);
    }

    private async Task<Station> GetRequiredAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        await repository.FindByIdAsync(id, cancellationToken)
        ?? throw new KeyNotFoundException("STATION_NOT_FOUND");

    private static StationDto Map(Station station) =>
        new(
            station.Id,
            station.Code,
            station.Name,
            station.Type,
            station.State,
            station.AgentRequired,
            station.AgentDeviceId,
            station.LastSeenAtUtc);
}