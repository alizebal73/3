using GameNet.Shared.Contracts.V1.System;
using GameNet.Shared.Contracts.V1.Stations;

namespace GameNet.Desktop.Api;

public interface IGameNetServerClient
{
    Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StationDto>> GetStationsAsync(CancellationToken cancellationToken = default);
}
