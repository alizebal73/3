using System.Diagnostics;

namespace GameNet.Server.Infrastructure.Observability;

public sealed class RequestMetricsMiddleware(
    RequestDelegate next,
    PlatformMetrics metrics)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            metrics.RecordRequest(
                context.Request.Method,
                context.Request.Path.ToString(),
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
