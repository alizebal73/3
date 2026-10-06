using GameNet.Server.Composition;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddGameNet();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

StartupChecks.Validate(app);
app.MapGameNetRoutes();

app.Run();

public partial class Program;
