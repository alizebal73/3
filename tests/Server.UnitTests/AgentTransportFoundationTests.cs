using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GameNet.Server.UnitTests;

public sealed class AgentTransportFoundationTests
{
    [Fact]
    public async Task Agent_transport_policy_requires_agent_actor_and_device_id()
    {
        var services = new ServiceCollection();
        services.AddGameNetAuthorization();

        await using var provider = services.BuildServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();
        var policy = await policyProvider.GetPolicyAsync("AgentTransport");

        Assert.NotNull(policy);
        Assert.Contains(
            policy!.Requirements,
            x => x is DenyAnonymousAuthorizationRequirement);

        Assert.Contains(
            policy.Requirements,
            x => x is ClaimsAuthorizationRequirement claim &&
                 claim.ClaimType == "actor_type" &&
                 claim.AllowedValues?.Contains("Agent", StringComparer.Ordinal) == true);

        Assert.Contains(
            policy.Requirements,
            x => x is ClaimsAuthorizationRequirement claim &&
                 claim.ClaimType == "device_id");
    }

    [Fact]
    public void Agent_timing_validation_rejects_heartbeat_equal_to_lease()
    {
        var validator = new GameNetOptionsValidator();
        var options = new GameNetOptions
        {
            Agent = new AgentOptions
            {
                LeaseDurationSeconds = 15,
                HeartbeatIntervalSeconds = 15
            }
        };

        var result = validator.Validate(
            Options.DefaultName,
            options);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Agent_timing_validation_accepts_default_foundation_values()
    {
        var validator = new GameNetOptionsValidator();
        var options = new GameNetOptions
        {
            Agent = new AgentOptions
            {
                LeaseDurationSeconds = 15,
                HeartbeatIntervalSeconds = 5
            }
        };

        var result = validator.Validate(
            Options.DefaultName,
            options);

        Assert.True(result.Succeeded);
    }
}
