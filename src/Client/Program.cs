using GameNet.Agent;
using GameNet.Agent.Identity;
using GameNet.Agent.Transport;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "GameNet Agent";
});

builder.Services
    .AddOptions<AgentTransportOptions>()
    .BindConfiguration(AgentTransportOptions.SectionName)
    .Validate(options =>
        Uri.TryCreate(options.ServerBaseUrl, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps),
        "GameNet:AgentTransport:ServerBaseUrl must be an absolute HTTP or HTTPS URL.")
    .Validate(options => options.HeartbeatIntervalSeconds is >= 2 and <= 60,
        "HeartbeatIntervalSeconds must be between 2 and 60.")
    .Validate(options => options.InitialRetrySeconds is >= 1 and <= 120,
        "InitialRetrySeconds must be between 1 and 120.")
    .ValidateOnStart();

builder.Services
    .AddOptions<AgentIdentityOptions>()
    .BindConfiguration(AgentIdentityOptions.SectionName)
    .Validate(options => !string.IsNullOrWhiteSpace(options.RootPath),
        "GameNet:AgentIdentity:RootPath is required.")
    .ValidateOnStart();

builder.Services.AddSingleton<IAgentIdentityStore, AgentIdentityStore>();
builder.Services.AddSingleton<IAgentTransport, SignalRAgentTransport>();
builder.Services.AddHostedService<AgentWorker>();

builder.Build().Run();
