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
    public void Different_schema_versions_are_not_runtime_compatible()
    {
        var current = new ReleaseVersion(new Version(1, 0, 0), 4, "v1", 1);
        var incoming = new ReleaseVersion(new Version(1, 1, 0), 5, "v1", 1);

        Assert.False(ReleaseCompatibility.IsSchemaCompatible(current, incoming));
    }

    [Fact]
    public void A_newer_schema_is_upgradeable_but_requires_migration()
    {
        var current = new ReleaseVersion(new Version(1, 0, 0), 4, "v1", 1);
        var incoming = new ReleaseVersion(new Version(1, 1, 0), 5, "v1", 1);

        Assert.True(ReleaseCompatibility.CanUpgradeSchema(current, incoming));
    }

    [Fact]
    public void An_older_schema_cannot_be_an_upgrade()
    {
        var current = new ReleaseVersion(new Version(1, 0, 0), 4, "v1", 1);
        var incoming = new ReleaseVersion(new Version(1, 1, 0), 3, "v1", 1);

        Assert.False(ReleaseCompatibility.CanUpgradeSchema(current, incoming));
    }
}
