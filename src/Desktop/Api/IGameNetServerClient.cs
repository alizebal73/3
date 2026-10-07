using GameNet.Shared.Contracts.V1.System;

namespace GameNet.Desktop.Api;

public interface IGameNetServerClient
{
    Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);
}
