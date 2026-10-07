namespace GameNet.Desktop.UI.Navigation;

public sealed record NavigationItem(
    string Id,
    string ResourceKey,
    string IconGlyph,
    bool IsEnabled = true);
