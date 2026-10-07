using GameNet.Server.Infrastructure;
using GameNet.Server.Infrastructure.Hosting;
using GameNet.Server.Infrastructure.Observability;
using GameNet.Server.Infrastructure.Security;
using GameNet.Server.Persistence;
using GameNet.Shared.Contracts.V1.Api;
using GameNet.Shared.Contracts.V1.System;
using GameNet.Shared.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace GameNet.Server.Composition;

public static class Routing
{
    public static void MapGameNetRoutes(this WebApplication app)
    {
        app.MapGet("/health/live", () => Results.Ok(new { status = "ok" }))
            .AllowAnonymous()
            .WithName("LiveHealth");

        app.MapGet("/health", (
            [FromServices] StartupState state,
            [FromServices] ServerReadiness readiness,
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

        app.MapGet("/ready", ([FromServices] ServerReadiness readiness) =>
            readiness.IsReady
                ? Results.Ok(new { status = "ready" })
                : Results.StatusCode(StatusCodes.Status503ServiceUnavailable))
            .AllowAnonymous();

        app.MapGet("/api/v1/system/build-info", (
            [FromServices] StartupState state,
            HttpContext context) =>
        {
            var operationId = context.Items[typeof(OperationId)] is OperationId id
                ? id.Value
                : string.Empty;

            return Results.Ok(new ApiEnvelope<BuildInfoResponse>(
                ContractVersions.V1,
                context.Items[typeof(CorrelationId)] is CorrelationId correlation
                    ? correlation.Value
                    : string.Empty,
                operationId,
                DateTimeOffset.UtcNow,
                new BuildInfoResponse(
                    state.Version,
                    ContractVersions.V1,
                    DatabaseSchemaVersion.Current)));
        }).AllowAnonymous().WithName("BuildInfo");

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
