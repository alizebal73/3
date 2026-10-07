using GameNet.Desktop.Api;
using GameNet.Desktop.Localization;
using GameNet.Desktop.Shell;
using GameNet.Desktop.UI.Services;
using GameNet.Desktop.UI.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNet.Desktop.Infrastructure.Hosting;

public static class DesktopHost
{
    public static IHost Build()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services
            .AddOptions<DesktopOptions>()
            .BindConfiguration(DesktopOptions.SectionName)
            .Validate(options => Uri.TryCreate(
                options.ServerBaseUrl,
                UriKind.Absolute,
                out var uri) &&
                uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps,
                "GameNet:Desktop:ServerBaseUrl must be an absolute HTTP or HTTPS URL.")
            .Validate(options =>
                options.HealthRefreshSeconds is >= 5 and <= 300,
                "GameNet:Desktop:HealthRefreshSeconds must be between 5 and 300.")
            .ValidateOnStart();

        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddSingleton<ServerConnectivityMonitor>();
        builder.Services.AddSingleton<ShellViewModel>();

        builder.Services.AddHttpClient<IGameNetServerClient, GameNetServerClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<DesktopOptions>>().Value;
            client.BaseAddress = new Uri(options.ServerBaseUrl, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        builder.Services.AddSingleton<MainWindow>();

        builder.Logging.AddDebug();

        return builder.Build();
    }
}
