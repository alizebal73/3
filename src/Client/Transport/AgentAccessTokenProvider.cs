using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using GameNet.Agent.Identity;
using GameNet.Shared.Contracts.V1.Security;
using Microsoft.Extensions.Options;

namespace GameNet.Agent.Transport;

public sealed class AgentAccessTokenProvider(
    IHttpClientFactory httpClientFactory,
    IAgentCredentialStore credentialStore,
    IOptions<AgentTransportOptions> options,
    TimeProvider timeProvider) : IAgentAccessTokenProvider, IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _deviceId;
    private string? _accessToken;
    private DateTimeOffset _expiresAtUtc;

    public async Task<string> GetAccessTokenAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var now = timeProvider.GetUtcNow();

            if (string.Equals(_deviceId, deviceId, StringComparison.Ordinal) &&
                !string.IsNullOrWhiteSpace(_accessToken) &&
                _expiresAtUtc > now.AddSeconds(30))
            {
                return _accessToken;
            }

            var secret = await credentialStore.GetOrBootstrapAsync(cancellationToken);
            var client = httpClientFactory.CreateClient("GameNetAgentCredentialClient");
            var endpoint =
                $"{options.Value.ServerBaseUrl.TrimEnd('/')}/api/v1/agent/auth/token";

            using var response = await client.PostAsJsonAsync(
                endpoint,
                new AgentTokenRequest(deviceId, secret),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"Agent token request failed with HTTP {(int)response.StatusCode}: {body}");
            }

            var token = await response.Content.ReadFromJsonAsync<AgentTokenResponse>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web),
                cancellationToken);

            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
                throw new InvalidOperationException("Server returned an empty Agent access token.");

            _deviceId = deviceId;
            _accessToken = token.AccessToken;
            _expiresAtUtc = token.ExpiresAtUtc;

            return token.AccessToken;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose() => _gate.Dispose();
}
