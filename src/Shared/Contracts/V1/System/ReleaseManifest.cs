namespace GameNet.Shared.Contracts.V1.System;

public sealed record ReleaseManifest(
    string ProductVersion,
    int SchemaVersion,
    string ApiContractVersion,
    int AgentProtocolVersion,
    int MigrationFromSchemaVersion,
    int MigrationToSchemaVersion,
    IReadOnlyList<ReleaseFileEntry> Files);

public sealed record ReleaseFileEntry(
    string RelativePath,
    string Sha256,
    long SizeBytes);
