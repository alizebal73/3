using GameNet.Shared.Primitives;

namespace GameNet.Server.Infrastructure.Observability;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var incoming = context.Request.Headers[HeaderName].FirstOrDefault();
        var value = IsValid(incoming) ? incoming! : CorrelationId.New().Value;
        var correlationId = new CorrelationId(value);

        context.Items[typeof(CorrelationId)] = correlationId;
        context.Response.Headers[HeaderName] = value;

        await next(context);
    }

    private static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length <= 128 &&
        value.All(c => char.IsLetterOrDigit(c) || c is '-' or '_' or '.');
}
