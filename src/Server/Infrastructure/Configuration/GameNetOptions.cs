namespace GameNet.Server.Infrastructure.Configuration;

public sealed class GameNetOptions
{
    public const string SectionName = "GameNet";

    public string BusinessTimeZone { get; init; } = "Asia/Tehran";
    public string Currency { get; init; } = "TOM";
    public string DataRoot { get; init; } = Path.Combine(AppContext.BaseDirectory, "Data");
    public string BackupRoot { get; init; } = Path.Combine(AppContext.BaseDirectory, "Backups");
    public string? DatabaseConnectionString { get; init; }
    public AuthenticationOptions Authentication { get; init; } = new();
    public IReadOnlyList<string> TrustedProxies { get; init; } = Array.Empty<string>();
}

public sealed class AuthenticationOptions
{
    public bool Enabled { get; init; }
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public string? SigningKey { get; init; }
}
