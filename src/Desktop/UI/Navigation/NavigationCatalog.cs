namespace GameNet.Desktop.UI.Navigation;

public static class NavigationCatalog
{
    public static IReadOnlyList<NavigationItem> Items { get; } =
    [
        new("home", "NavHome", "⌂", string.Empty),
        new("stations", "NavStations", "▣", string.Empty),
        new("customers", "NavCustomers", "◉", string.Empty),
        new("sessions", "NavSessions", "◷", string.Empty),
        new("billing", "NavBilling", "₼", string.Empty),
        new("wallet", "NavWallet", "◈", string.Empty),
        new("inventory", "NavInventory", "▤", string.Empty),
        new("buffet", "NavBuffet", "◫", string.Empty),
        new("tariffs", "NavTariffs", "◌", string.Empty),
        new("vip", "NavVip", "★", string.Empty),
        new("reports", "NavReports", "▥", string.Empty),
        new("approvals", "NavApprovals", "✓", string.Empty),
        new("users", "NavUsers", "◍", string.Empty),
        new("backup", "NavBackup", "↥", string.Empty),
        new("audit", "NavAudit", "≡", string.Empty),
        new("settings", "NavSettings", "⚙", string.Empty)
    ];
}
