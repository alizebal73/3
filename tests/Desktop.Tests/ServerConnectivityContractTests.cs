using GameNet.Desktop.UI.Services;

namespace GameNet.Desktop.Tests;

public sealed class ServerConnectivityContractTests
{
    [Fact]
    public void Canonical_server_health_is_online()
    {
        Assert.True(ServerConnectivityMonitor.IsReadyHealthResponse("ok", "ready"));
    }

    [Fact]
    public void Non_ready_server_health_is_not_online()
    {
        Assert.False(ServerConnectivityMonitor.IsReadyHealthResponse("starting", "ready"));
        Assert.False(ServerConnectivityMonitor.IsReadyHealthResponse("ok", "not_ready"));
        Assert.False(ServerConnectivityMonitor.IsReadyHealthResponse("Healthy", "Ready"));
    }
}
