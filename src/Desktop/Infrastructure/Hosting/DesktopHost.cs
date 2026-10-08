using GameNet.Desktop.Api;
using GameNet.Desktop.Localization;
using GameNet.Desktop.Shell;
using GameNet.Desktop.UI.Services;
using GameNet.Desktop.UI.Stations;
using GameNet.Desktop.UI.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNet.Desktop.Infrastructure.Hosting;

public static class DesktopHost
{
    private static bool IsSupportedServerUri(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri is null)
            return false;

        return uri.Scheme == Uri.UriSchemeHttp ||
               uri.Scheme == Uri.UriSchemeHttps;
    }

    public static IHost Build()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services
            .AddOptions<DesktopOptions>()
            .BindConfiguration(DesktopOptions.SectionName)
.Validate(
                options => IsSupportedServerUri(options.ServerBaseUrl),
                "GameNet:Desktop:ServerBaseUrl must be an absolute HTTP or HTTPS URL.")
            .Validate(options =>
                options.HealthRefreshSeconds is >= 5 and <= 300,
                "GameNet:Desktop:HealthRefreshSeconds must be between 5 and 300.")
            .ValidateOnStart();

        builder.Services.AddSingleton<LanguageService>();
        builder.Services.AddSingleton<ServerConnectivityMonitor>();
        builder.Services.AddSingleton<ShellViewModel>();
        builder.Services.AddSingleton<StationsViewModel>();

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
