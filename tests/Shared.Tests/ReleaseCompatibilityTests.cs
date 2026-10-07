using GameNet.Shared.Primitives;

namespace GameNet.Shared.Tests;

public sealed class ReleaseCompatibilityTests
{
    [Fact]
    public void Matching_agent_protocol_is_compatible()
    {
        var server = new ReleaseVersion(new Version(1, 0, 0), 3, "v1", 2);
        var agent = new ReleaseVersion(new Version(1, 1, 0), 3, "v1", 2);

        Assert.True(ReleaseCompatibility.IsAgentCompatible(server, agent));
    }

    [Fact]
    public void Different_agent_protocol_is_not_compatible()
    {
        var server = new ReleaseVersion(new Version(1, 0, 0), 3, "v1", 2);
        var agent = new ReleaseVersion(new Version(1, 1, 0), 3, "v1", 3);

        Assert.False(ReleaseCompatibility.IsAgentCompatible(server, agent));
    }

    [Fact]
    public void Incoming_schema_must_not_be_older()
    {
        var current = new ReleaseVersion(new Version(1, 0, 0), 4, "v1", 1);
        var incoming = new ReleaseVersion(new Version(1, 1, 0), 3, "v1", 1);

        Assert.False(ReleaseCompatibility.IsSchemaCompatible(current, incoming));
    }
}
