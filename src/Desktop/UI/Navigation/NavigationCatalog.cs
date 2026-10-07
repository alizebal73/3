namespace GameNet.Desktop.UI.Navigation;

public static class NavigationCatalog
{
    public static IReadOnlyList<NavigationItem> Items { get; } =
    [
        new("home", "NavHome", "⌂"),
        new("stations", "NavStations", "▣"),
        new("customers", "NavCustomers", "◉"),
        new("sessions", "NavSessions", "◷"),
        new("billing", "NavBilling", "₼"),
        new("wallet", "NavWallet", "◈"),
        new("inventory", "NavInventory", "▤"),
        new("buffet", "NavBuffet", "◫"),
        new("tariffs", "NavTariffs", "◌"),
        new("vip", "NavVip", "★"),
        new("reports", "NavReports", "▥"),
        new("approvals", "NavApprovals", "✓"),
        new("users", "NavUsers", "◍"),
        new("backup", "NavBackup", "↥"),
        new("audit", "NavAudit", "≡"),
        new("settings", "NavSettings", "⚙")
    ];
}
