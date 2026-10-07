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
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<AgentTransportOptions>, AgentTransportOptionsValidator>();

builder.Services
    .AddOptions<AgentIdentityOptions>()
    .BindConfiguration(AgentIdentityOptions.SectionName)
    .Validate(options => !string.IsNullOrWhiteSpace(options.RootPath),
        "GameNet:AgentIdentity:RootPath is required.")
    .ValidateOnStart();

builder.Services.AddHttpClient("GameNetAgentCredentialClient");

builder.Services.AddSingleton<IAgentIdentityStore, AgentIdentityStore>();
builder.Services.AddSingleton<IAgentCredentialStore, AgentCredentialStore>();
builder.Services.AddSingleton<IAgentAccessTokenProvider, AgentAccessTokenProvider>();
builder.Services.AddSingleton<IAgentTransport, SignalRAgentTransport>();
builder.Services.AddHostedService<AgentWorker>();

builder.Build().Run();
