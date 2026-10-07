using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Time;
using GameNet.Shared.Contracts.V1.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace GameNet.Server.Infrastructure.Security;

public sealed class AgentAccessTokenIssuer(
    IOptions<GameNetOptions> options,
    IGameClock clock) : IAgentAccessTokenIssuer
{
    public AgentTokenResponse Issue(string deviceId)
    {
        var authentication = options.Value.Authentication;
        var signingKey = authentication.SigningKey;

        if (!authentication.Enabled ||
            string.IsNullOrWhiteSpace(authentication.Issuer) ||
            string.IsNullOrWhiteSpace(authentication.Audience) ||
            string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                "JWT authentication is not fully configured.");
        }

        var now = clock.UtcNow;
        var expires = now.AddSeconds(options.Value.Agent.AccessTokenLifetimeSeconds);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, deviceId),
            new Claim(JwtRegisteredClaimNames.ClientId, deviceId),
            new Claim("actor_type", "Agent"),
            new Claim("device_id", deviceId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = authentication.Issuer,
            Audience = authentication.Audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = now.UtcDateTime,
            Expires = expires.UtcDateTime,
            SigningCredentials = credentials
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);

        return new AgentTokenResponse(
            handler.WriteToken(token),
            expires);
    }
}
