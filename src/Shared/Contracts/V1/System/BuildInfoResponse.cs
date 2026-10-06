namespace GameNet.Shared.Contracts.V1.System;

public sealed record BuildInfoResponse(
    string ApplicationVersion,
    string ContractVersion,
    string DatabaseSchemaVersion);
