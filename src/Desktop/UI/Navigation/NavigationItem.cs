using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameNet.Desktop.UI.Navigation;

public sealed record NavigationItem(
    string Id,
    string ResourceKey,
    string IconGlyph,
    string DisplayName,
    bool IsEnabled = true);
