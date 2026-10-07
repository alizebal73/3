namespace GameNet.Shared.Contracts.V1.Protocol;

public static class AgentCommandTypes
{
    public const string LaunchGame = "client.game.launch";
    public const string StopGame = "client.game.stop";
    public const string LockClient = "client.lock";
    public const string UnlockClient = "client.unlock";
    public const string RestartClient = "client.restart";
    public const string ShutdownClient = "client.shutdown";
    public const string EnterMaintenance = "client.maintenance.enter";
    public const string ExitMaintenance = "client.maintenance.exit";
    public const string ApplyClientPolicy = "client.policy.apply";
    public const string RefreshGameCatalog = "client.game_catalog.refresh";
    public const string RefreshGameAccounts = "client.game_accounts.refresh";
    public const string CollectDiagnostics = "client.diagnostics.collect";
}

public sealed record LaunchGameCommand(
    string GameId,
    string SessionId,
    string? GameAccountAllocationId,
    string LaunchProfileId,
    CommandExpiry Expiry);

public sealed record StopGameCommand(
    string SessionId,
    string? ProcessExecutionId,
    string Reason,
    CommandExpiry Expiry);

public sealed record ClientControlCommand(
    string Action,
    string Reason,
    CommandExpiry Expiry);

public sealed record ApplyClientPolicyCommand(
    string PolicyVersion,
    IReadOnlyDictionary<string, string> Values,
    CommandExpiry Expiry);

public sealed record RefreshClientCatalogCommand(
    string CatalogVersion,
    CommandExpiry Expiry);
