using GameNet.Desktop.Api;
using GameNet.Desktop.Infrastructure.Hosting;
using GameNet.Desktop.Localization;
using GameNet.Desktop.UI.Services;
using GameNet.Desktop.UI.Shell;
using GameNet.Shared.Contracts.V1.System;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace GameNet.Desktop.Tests;

public sealed class ShellViewModelFoundationTests
{
    [Fact]
    public async Task Shell_exposes_one_central_navigation_catalog_with_home_selected()
    {
        await using var shell = CreateShell();

        Assert.Equal("home", shell.CurrentPageId);
        Assert.NotEmpty(shell.NavigationItems);
        Assert.Equal(
            shell.NavigationItems.Count,
            shell.NavigationItems.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Contains(
            shell.NavigationItems,
            item => item.Id == "stations");
        Assert.Contains(
            shell.NavigationItems,
            item => item.Id == "customers");
    }

    [Fact]
    public async Task Global_command_surface_is_registered_once()
    {
        await using var shell = CreateShell();

        Assert.NotNull(shell.RefreshCommand);
        Assert.NotNull(shell.ToggleLanguageCommand);
        Assert.NotNull(shell.NavigateHomeCommand);
        Assert.NotNull(shell.ToggleCommandPaletteCommand);
        Assert.NotNull(shell.ToggleHelpCommand);
        Assert.NotNull(shell.ToggleNotificationCenterCommand);
        Assert.NotNull(shell.CloseOverlaysCommand);
    }

    [Fact]
    public async Task Command_palette_filters_central_navigation_without_creating_new_routes()
    {
        await using var shell = CreateShell();

        shell.CommandPaletteQuery = "customer";

        var result = Assert.Single(shell.CommandPaletteItems);

        Assert.Equal("customers", result.Id);
        Assert.Equal(
            shell.NavigationItems.Single(x => x.Id == "customers"),
            result);
    }

    [Fact]
    public async Task Global_overlays_are_mutually_exclusive_and_escape_closes_them()
    {
        await using var shell = CreateShell();

        shell.ToggleCommandPaletteCommand.Execute(null);
        Assert.True(shell.IsCommandPaletteOpen);
        Assert.False(shell.IsHelpOpen);

        shell.ToggleHelpCommand.Execute(null);
        Assert.False(shell.IsCommandPaletteOpen);
        Assert.True(shell.IsHelpOpen);

        shell.CloseOverlaysCommand.Execute(null);

        Assert.False(shell.IsCommandPaletteOpen);
        Assert.False(shell.IsHelpOpen);
        Assert.False(shell.IsNotificationCenterOpen);
    }

    private static ShellViewModel CreateShell()
    {
        var monitor = new ServerConnectivityMonitor(
            new FakeServerClient(),
            Options.Create(new DesktopOptions()),
            TimeProvider.System,
            NullLogger<ServerConnectivityMonitor>.Instance);

        return new ShellViewModel(
            new LanguageService(),
            monitor);
    }

    private sealed class FakeServerClient : IGameNetServerClient
    {
        public Task<HealthResponse> GetHealthAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(
                new HealthResponse(
                    "ok",
                    "GameNet.Server",
                    "foundation-test",
                    "ready",
                    "test-correlation"));
    }
}
