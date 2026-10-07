using GameNet.Agent.Identity;
using Microsoft.Extensions.Options;

namespace GameNet.Agent.Tests;

public sealed class AgentIdentityStoreTests
{
    [Fact]
    public async Task Identity_store_reuses_the_same_device_id()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            "gamenet-agent-identity-test",
            Guid.NewGuid().ToString("N"));

        try
        {
            var store = new AgentIdentityStore(
                Options.Create(new AgentIdentityOptions { RootPath = root }));

            var first = await store.GetOrCreateAsync();
            var second = await store.GetOrCreateAsync();

            Assert.Equal(first.DeviceId, second.DeviceId);
            Assert.False(string.IsNullOrWhiteSpace(first.DeviceId));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }
}
