using System.Globalization;
using System.Windows;
using GameNet.Desktop.Infrastructure.Hosting;
using GameNet.Desktop.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameNet.Desktop;

public partial class App : Application
{
    private IHost? _host;

    public static LanguageService Language =>
        Current.Properties["GameNet.LanguageService"] as LanguageService
        ?? throw new InvalidOperationException("Desktop language service is not initialized.");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = DesktopHost.Build();
        await _host.StartAsync();

        var language = _host.Services.GetRequiredService<LanguageService>();
        Current.Properties["GameNet.LanguageService"] = language;
        language.SetLanguage(CultureInfo.GetCultureInfo("fa-IR"));

        MainWindow = _host.Services.GetRequiredService<Shell.MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
