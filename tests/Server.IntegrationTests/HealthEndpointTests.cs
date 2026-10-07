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
}
