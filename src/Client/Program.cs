using GameNet.Agent;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<AgentWorker>();
builder.Build().Run();
