using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GameNet.E2E.Tests;

public sealed class FoundationSmokeTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Server_live_endpoint_is_reachable_through_real_host_pipeline()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}