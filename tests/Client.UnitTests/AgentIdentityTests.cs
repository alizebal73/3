using GameNet.Agent;

namespace GameNet.Client.UnitTests;

public sealed class AgentIdentityTests
{
    [Fact]
    public void Identity_uses_explicit_device_id()
    {
        var identity = AgentIdentity.FromDeviceId("PC-04");

        Assert.Equal("PC-04", identity.DeviceId);
    }

    [Fact]
    public void Identity_rejects_missing_device_id()
    {
        Assert.Throws<ArgumentException>(() => AgentIdentity.FromDeviceId(" "));
    }
}
