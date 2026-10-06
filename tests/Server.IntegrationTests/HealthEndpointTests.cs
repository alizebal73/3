using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GameNet.Server.IntegrationTests;

public sealed class HealthEndpointTests(IClassFixture<WebApplicationFactory<Program>> fixture)
{
    [Fact]
    public async Task Health_endpoint_is_available()
    {
        using var client = fixture.CreateClient();
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("GameNet.Server", body, StringComparison.Ordinal);
        Assert.Contains("Foundation", body, StringComparison.Ordinal);
    }
}
