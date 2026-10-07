using GameNet.Server.Composition;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Observability;
using GameNet.Server.Infrastructure.Security;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.foundation.json",
    optional: false,
    reloadOnChange: false);

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "GameNet Server";
});

builder.Services.AddOpenApi();
builder.Services.AddGameNet();

var configuredOptions = builder.Configuration
    .GetSection(GameNetOptions.SectionName)
    .Get<GameNetOptions>() ?? new GameNetOptions();

builder.Services.AddGameNetAuthentication(configuredOptions);

var app = builder.Build();

var proxyOptions = app.Services.GetRequiredService<IOptions<GameNetOptions>>().Value;

if (proxyOptions.TrustedProxies.Count > 0)
{
    var forwarded = new ForwardedHeadersOptions
    {
        ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    };

    foreach (var value in proxyOptions.TrustedProxies)
    {
        if (System.Net.IPAddress.TryParse(value, out var address))
            forwarded.KnownProxies.Add(address);
    }

    app.UseForwardedHeaders(forwarded);
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (proxyOptions.Authentication.Enabled)
    app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

await StartupChecks.ValidateAsync(app);
app.MapGameNetRoutes();

app.Run();

public partial class Program;
