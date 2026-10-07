using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using GameNet.Desktop.Api;
using GameNet.Shared.Contracts.V1.System;

namespace GameNet.Desktop.Tests;

public sealed class GameNetServerClientTests
{
    [Fact]
    public async Task Health_request_uses_shared_contract_and_correlation_header()
    {
        var handler = new CaptureHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new HealthResponse(
                    "ok",
                    "GameNet.Server",
                    "foundation-test",
                    "ready",
                    "server-correlation"))
            });

        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://127.0.0.1:5080")
        };

        var sut = new GameNetServerClient(client);

        var health = await sut.GetHealthAsync();

        Assert.Equal("ok", health.Status);
        Assert.Equal("GameNet.Server", health.Service);
        Assert.True(handler.LastRequest!.Headers.Contains("X-Correlation-Id"));
    }

    private sealed class CaptureHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(responder(request));
        }
    }
}
