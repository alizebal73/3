using System.IdentityModel.Tokens.Jwt;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Security;
using GameNet.Server.Infrastructure.Time;
using Microsoft.Extensions.Options;

namespace GameNet.Server.UnitTests;

public sealed class AgentCredentialFoundationTests
{
    [Fact]
    public void Agent_token_contains_agent_identity_and_short_lifetime()
    {
        var now = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero);
        var options = Options.Create(new GameNetOptions
        {
            Authentication = new AuthenticationOptions
            {
                Enabled = true,
                Issuer = "GameNet.Test",
                Audience = "GameNet.Agent",
                SigningKey = new string('k', 64)
            },
            Agent = new AgentOptions
            {
                AccessTokenLifetimeSeconds = 300,
                ProvisioningKey = new string('p', 64)
            }
        });

        var issuer = new AgentAccessTokenIssuer(
            options,
            new FixedClock(now));

        var issued = issuer.Issue("device-123");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(issued.AccessToken);

        Assert.Equal("GameNet.Test", jwt.Issuer);
        Assert.Contains("GameNet.Agent", jwt.Audiences);
        Assert.Contains(jwt.Claims, x => x.Type == "actor_type" && x.Value == "Agent");
        Assert.Contains(jwt.Claims, x => x.Type == "device_id" && x.Value == "device-123");
        Assert.Equal(now.UtcDateTime, jwt.ValidFrom);
        Assert.Equal(now.AddSeconds(300).UtcDateTime, jwt.ValidTo);
        Assert.Equal(now.AddSeconds(300), issued.ExpiresAtUtc);
    }

    [Fact]
    public void Enabled_agent_authentication_requires_a_provisioning_key()
    {
        var validator = new GameNetOptionsValidator();
        var options = new GameNetOptions
        {
            Authentication = new AuthenticationOptions
            {
                Enabled = true,
                Issuer = "GameNet.Test",
                Audience = "GameNet.Agent",
                SigningKey = new string('k', 64)
            },
            Agent = new AgentOptions
            {
                AccessTokenLifetimeSeconds = 300
            }
        };

        var result = validator.Validate(
            Options.DefaultName,
            options);

        Assert.False(result.Succeeded);
    }

    private sealed class FixedClock(DateTimeOffset now) : IGameClock
    {
        public DateTimeOffset UtcNow => now;
        public DateTimeOffset LocalNow => now;
        public DateOnly BusinessDate => DateOnly.FromDateTime(now.Date);
    }
}
