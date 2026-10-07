using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GameNet.Agent.Transport;

public sealed class AgentTransportOptionsValidator(
    IHostEnvironment environment) : IValidateOptions<AgentTransportOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        AgentTransportOptions options)
    {
        if (!Uri.TryCreate(
                options.ServerBaseUrl,
                UriKind.Absolute,
                out var uri) ||
            uri is null ||
            (uri.Scheme != Uri.UriSchemeHttp &&
             uri.Scheme != Uri.UriSchemeHttps))
        {
            return ValidateOptionsResult.Fail(
                "GameNet:AgentTransport:ServerBaseUrl must be an absolute HTTP or HTTPS URL.");
        }

        if (uri.Scheme == Uri.UriSchemeHttp &&
            (environment.IsProduction() ||
             !options.AllowInsecureHttpForDevelopment))
        {
            return ValidateOptionsResult.Fail(
                "Agent ServerBaseUrl must use HTTPS outside explicit Development HTTP mode.");
        }

        if (options.HeartbeatIntervalSeconds is < 2 or > 60)
        {
            return ValidateOptionsResult.Fail(
                "HeartbeatIntervalSeconds must be between 2 and 60.");
        }

        if (options.InitialRetrySeconds is < 1 or > 120)
        {
            return ValidateOptionsResult.Fail(
                "InitialRetrySeconds must be between 1 and 120.");
        }

        if (string.IsNullOrWhiteSpace(
                options.BootstrapCredentialEnvironmentVariableName))
        {
            return ValidateOptionsResult.Fail(
                "BootstrapCredentialEnvironmentVariableName is required.");
        }

        return ValidateOptionsResult.Success;
    }
}
