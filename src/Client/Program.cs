using GameNet.Agent;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "GameNet Agent";
});

builder.Services.AddHostedService<AgentWorker>();
builder.Build().Run();
