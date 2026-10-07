using GameNet.Server.Infrastructure;
using GameNet.Server.Infrastructure.Hosting;
using GameNet.Server.Infrastructure.Observability;
using GameNet.Shared.Contracts.V1.System;
using GameNet.Shared.Primitives;

namespace GameNet.Server.Composition;

public static class Routing
{
    public static void MapGameNetRoutes(this WebApplication app)
    {
        app.MapGet("/health/live", () => Results.Ok(new { status = "ok" }))
            .AllowAnonymous()
            .WithName("LiveHealth");

        app.MapGet("/health", (
            StartupState state,
            ServerReadiness readiness,
            HttpContext context) =>
        {
            var correlationId = context.Items[typeof(CorrelationId)] is CorrelationId id
                ? id.Value
                : string.Empty;

            var status = readiness.IsReady ? "ok" : "starting";

            return Results.Ok(new HealthResponse(
                status,
                state.Service,
                StartupState.Version,
                readiness.IsReady ? "ready" : "not_ready",
                correlationId));
        }).AllowAnonymous().WithName("Health");

        app.MapGet("/ready", (ServerReadiness readiness) =>
            readiness.IsReady
                ? Results.Ok(new { status = "ready" })
                : Results.StatusCode(StatusCodes.Status503ServiceUnavailable))
            .AllowAnonymous();

        app.MapGet("/", () => Results.Ok(new
        {
            service = "GameNet.Server",
            status = "running"
        })).AllowAnonymous();

        app.MapGameNetModules(
            app.Services.GetServices<IGameNetModule>());

        app.MapAgentCredentialRoutes();
    }
}
