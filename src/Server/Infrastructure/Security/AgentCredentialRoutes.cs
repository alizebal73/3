using System.Security.Cryptography;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Shared.Contracts.V1.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Infrastructure.Security;

public static class AgentCredentialRoutes
{
    private const string ProvisioningHeader = "X-GameNet-Agent-Provisioning-Key";

    public static void MapAgentCredentialRoutes(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agent");

        group.MapPost(
            "/auth/token",
            async (
                HttpContext context,
                AgentTokenRequest request,
                IAgentCredentialService credentials,
                IAgentAccessTokenIssuer tokenIssuer,
                IOptions<GameNetOptions> options,
                CancellationToken cancellationToken) =>
            {
                if (!options.Value.Authentication.Enabled)
                    return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

                var accepted = await credentials.AuthenticateAsync(
                    request.DeviceId,
                    request.Secret,
                    cancellationToken);

                if (!accepted)
                    return Results.Unauthorized();

                var token = tokenIssuer.Issue(request.DeviceId);
                context.Response.Headers.CacheControl = "no-store";

                return Results.Ok(token);
            })
            .RequireRateLimiting("AgentToken")
            .AllowAnonymous()
            .WithName("AgentToken");

        group.MapPost(
            "/credentials/provision",
            async (
                HttpContext context,
                AgentCredentialProvisionRequest request,
                IAgentCredentialService credentials,
                IOptions<GameNetOptions> options,
                CancellationToken cancellationToken) =>
            {
                var authorization = ValidateProvisioningKey(context, options.Value.Agent.ProvisioningKey);
                if (authorization is not null)
                    return authorization;

                try
                {
                    var issued = await credentials.ProvisionAsync(request, cancellationToken);
                    context.Response.Headers.CacheControl = "no-store";
                    return Results.Ok(issued);
                }
                catch (InvalidOperationException exception)
                {
                    return Results.Conflict(new { error = exception.Message });
                }
            })
            .RequireRateLimiting("AgentCredentialManagement")
            .AllowAnonymous()
            .WithName("AgentCredentialProvision");

        group.MapPost(
            "/credentials/rotate",
            async (
                HttpContext context,
                AgentCredentialRotateRequest request,
                IAgentCredentialService credentials,
                IOptions<GameNetOptions> options,
                CancellationToken cancellationToken) =>
            {
                var authorization = ValidateProvisioningKey(context, options.Value.Agent.ProvisioningKey);
                if (authorization is not null)
                    return authorization;

                try
                {
                    var issued = await credentials.RotateAsync(request, cancellationToken);
                    context.Response.Headers.CacheControl = "no-store";
                    return Results.Ok(issued);
                }
                catch (InvalidOperationException exception)
                {
                    return Results.Conflict(new { error = exception.Message });
                }
            })
            .RequireRateLimiting("AgentCredentialManagement")
            .AllowAnonymous()
            .WithName("AgentCredentialRotate");

        group.MapPost(
            "/credentials/revoke",
            async (
                HttpContext context,
                AgentCredentialRevokeRequest request,
                IAgentCredentialService credentials,
                IOptions<GameNetOptions> options,
                CancellationToken cancellationToken) =>
            {
                var authorization = ValidateProvisioningKey(context, options.Value.Agent.ProvisioningKey);
                if (authorization is not null)
                    return authorization;

                var revoked = await credentials.RevokeAsync(request, cancellationToken);
                return Results.Ok(new { revoked });
            })
            .RequireRateLimiting("AgentCredentialManagement")
            .AllowAnonymous()
            .WithName("AgentCredentialRevoke");
    }

    private static IResult? ValidateProvisioningKey(
        HttpContext context,
        string? expectedKey)
    {
        if (string.IsNullOrWhiteSpace(expectedKey))
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

        var providedKey =
            context.Request.Headers[ProvisioningHeader].ToString();

        if (string.IsNullOrWhiteSpace(providedKey))
            return Results.Unauthorized();

        var expected = System.Text.Encoding.UTF8.GetBytes(expectedKey);
        var provided = System.Text.Encoding.UTF8.GetBytes(providedKey);

        if (expected.Length != provided.Length ||
            !CryptographicOperations.FixedTimeEquals(expected, provided))
        {
            return Results.Unauthorized();
        }

        return null;
    }
}
