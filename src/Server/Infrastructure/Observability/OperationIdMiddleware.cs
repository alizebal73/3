using GameNet.Shared.Contracts.V1.Api;
using GameNet.Shared.Primitives;

namespace GameNet.Server.Infrastructure.Observability;

public sealed class OperationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var incoming = context.Request.Headers[ApiHeaders.OperationId].FirstOrDefault();
        var value = IsValid(incoming) ? incoming! : OperationId.New().Value;

        context.Items[typeof(OperationId)] = new OperationId(value);
        context.Response.Headers[ApiHeaders.OperationId] = value;

        await next(context);
    }

    private static bool IsValid(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length <= 128 &&
        value.All(c => char.IsLetterOrDigit(c) || c is '-' or '_' or '.');
}
