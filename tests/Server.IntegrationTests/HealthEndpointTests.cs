using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GameNet.Server.IntegrationTests;

public sealed class HealthEndpointTests(WebApplicationFactory<Program> fixture)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Live_health_endpoint_is_available()
    {
        using var client = fixture.CreateClient();
        using var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Correlation_id_is_returned()
    {
        using var client = fixture.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("X-Correlation-Id", "foundation-test-1");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("foundation-test-1", response.Headers.GetValues("X-Correlation-Id").Single());
    }

    [Fact]
    public async Task Operation_id_is_generated_and_returned()
    {
        using var client = fixture.CreateClient();
        using var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var operationId = response.Headers.GetValues("X-Operation-Id").Single();
        Assert.False(string.IsNullOrWhiteSpace(operationId));
        Assert.True(operationId.Length <= 128);
    }

    [Fact]
    public async Task Build_info_exposes_contract_and_schema_versions()
    {
        using var client = fixture.CreateClient();
        using var response = await client.GetAsync("/api/v1/system/build-info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("ApplicationVersion", body, StringComparison.Ordinal);
        Assert.Contains("ContractVersion", body, StringComparison.Ordinal);
        Assert.Contains("DatabaseSchemaVersion", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Agent_token_endpoint_is_rate_limited()
    {
        using var client = fixture.CreateClient();
        var sawRateLimit = false;

        for (var i = 0; i < 31; i++)
        {
            using var requestBody = new StringContent(
                "{\"DeviceId\":\"rate-test\",\"Secret\":\"invalid\"}",
                System.Text.Encoding.UTF8,
                "application/json");

            using var response = await client.PostAsync(
                "/api/v1/agent/auth/token",
                requestBody);

            if (response.StatusCode == (HttpStatusCode)429)
            {
                sawRateLimit = true;
                break;
            }
        }

        Assert.True(sawRateLimit);
    }
}
