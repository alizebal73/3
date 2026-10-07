using System.Text.Json;
using GameNet.Server.Infrastructure.Time;
using GameNet.Shared.Contracts.V1.Api;
using GameNet.Shared.Primitives;

namespace GameNet.Server.Infrastructure.Observability;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IGameClock clock)
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
                "Unhandled request failure. CorrelationId={CorrelationId} OperationId={OperationId}",
                correlationId,
                operationId);

            if (context.Response.HasStarted)
                throw;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var error = new ApiError(
                ApiErrorCodes.Internal,
                "server.unhandled",
                correlationId,
                operationId,
                Retryable: false);

            var payload = new ApiEnvelope<ApiError>(
                ContractVersions.V1,
                correlationId,
                operationId,
                clock.UtcNow,
                error);

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
