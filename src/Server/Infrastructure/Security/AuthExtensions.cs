using GameNet.Shared.Contracts.V1.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GameNet.Server.Infrastructure.Security;

public static class AuthExtensions
{
    public static IServiceCollection AddGameNetAuthentication(
        this IServiceCollection services,
        GameNet.Server.Infrastructure.Configuration.GameNetOptions options)
    {
        if (options.Authentication.Enabled)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwt =>
                {
                    jwt.RequireHttpsMetadata = true;
                    jwt.SaveToken = false;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = options.Authentication.Issuer,
                        ValidateAudience = true,
                        ValidAudience = options.Authentication.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(options.Authentication.SigningKey!)),
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };

                    jwt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"].ToString();
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrWhiteSpace(accessToken) &&
                                path.StartsWithSegments("/hubs/agent"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        services.AddGameNetAuthorization();
        return services;
    }

    public static IServiceCollection AddGameNetAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AgentTransport", policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("actor_type", "Agent")
                    .RequireClaim("device_id"));

            foreach (var permission in typeof(Permissions)
                         .GetFields(System.Reflection.BindingFlags.Public |
                                    System.Reflection.BindingFlags.Static)
                         .Where(x => x.FieldType == typeof(string))
                         .Select(x => (string)x.GetValue(null)!)
                         .Distinct(StringComparer.Ordinal))
            {
                options.AddPolicy(permission, policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("permission", permission));
            }
        });

        return services;
    }
}
