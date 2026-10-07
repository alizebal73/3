namespace GameNet.Shared.Primitives;

public readonly record struct ReleaseVersion(
    Version ProductVersion,
    int SchemaVersion,
    string ApiContractVersion,
    int AgentProtocolVersion);

public static class ReleaseCompatibility
{
    public static bool IsAgentCompatible(ReleaseVersion server, ReleaseVersion agent) =>
        string.Equals(server.ApiContractVersion, agent.ApiContractVersion, StringComparison.Ordinal) &&
        server.AgentProtocolVersion == agent.AgentProtocolVersion;

    public static bool IsSchemaCompatible(ReleaseVersion current, ReleaseVersion incoming) =>
        current.SchemaVersion == incoming.SchemaVersion;

    public static bool CanUpgradeSchema(ReleaseVersion current, ReleaseVersion incoming) =>
        incoming.SchemaVersion >= current.SchemaVersion;
}
