using System.Net.Http;
using System.Net.Http.Json;
using GameNet.Shared.Contracts.V1.Api;
using GameNet.Shared.Contracts.V1.System;
using GameNet.Shared.Primitives;

namespace GameNet.Desktop.Api;

public sealed class GameNetServerClient(HttpClient httpClient) : IGameNetServerClient
{
    public async Task<HealthResponse> GetHealthAsync(
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.TryAddWithoutValidation(
            ApiHeaders.CorrelationId,
            CorrelationId.New().Value);
        request.Headers.TryAddWithoutValidation(
            ApiHeaders.ContractVersion,
            ContractVersions.V1);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<HealthResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException(
                   "GameNet Server returned an empty health response.");
    }
}
