using GameNet.Agent.Identity;

namespace GameNet.Agent.Tests;

public sealed class AgentIdentityStoreTests
{
    [Fact]
    public async Task Identity_store_reuses_the_same_device_id()
    {
        var store = new AgentIdentityStore();

        var first = await store.GetOrCreateAsync();
        var second = await store.GetOrCreateAsync();

        Assert.Equal(first.DeviceId, second.DeviceId);
        Assert.False(string.IsNullOrWhiteSpace(first.DeviceId));
    }
}
