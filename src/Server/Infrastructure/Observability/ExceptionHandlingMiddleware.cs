using System.Text.Json;
using GameNet.Shared.Api;
using GameNet.Shared.Contracts.Errors;
using GameNet.Shared.Primitives;

namespace GameNet.Server.Infrastructure.Observability;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            var correlationId = context.Items[typeof(CorrelationId)] is CorrelationId id
                ? id.Value
                : context.TraceIdentifier;

            var operationId = context.Items[typeof(OperationId)] is OperationId operation
                ? operation.Value
                : null;

            logger.LogError(
                exception,
                "Unhandled request failure. CorrelationId={CorrelationId}",
                correlationId);

            if (context.Response.HasStarted)
                throw;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ApiFailure(
                new ApiError(
                    "server.unhandled",
                    "An unexpected server error occurred."),
                correlationId,
                operationId);

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
