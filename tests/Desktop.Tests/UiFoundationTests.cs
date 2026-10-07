using GameNet.Desktop.UI.Commands;
using GameNet.Desktop.UI.Navigation;

namespace GameNet.Desktop.Tests;

public sealed class UiFoundationTests
{
    [Fact]
    public void Navigation_catalog_has_unique_ids_and_resource_keys()
    {
        var items = NavigationCatalog.Items;

        Assert.NotEmpty(items);
        Assert.Equal(
            items.Count,
            items.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(
            items.Count,
            items.Select(x => x.ResourceKey).Distinct(StringComparer.Ordinal).Count());
        Assert.All(
            items,
            item => Assert.False(string.IsNullOrWhiteSpace(item.DisplayName)));
    }

    [Fact]
    public void Ui_command_invokes_the_same_command_handler_once()
    {
        var executionCount = 0;
        var command = new UiCommand(_ => executionCount++);

        Assert.True(command.CanExecute(null));
        command.Execute(null);

        Assert.Equal(1, executionCount);
    }
}
