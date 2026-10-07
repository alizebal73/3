using GameNet.Server.Infrastructure.Security;
using GameNet.Shared.Contracts.V1.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace GameNet.Server.UnitTests;

public sealed class AuthFoundationTests
{
    [Fact]
    public async Task Every_declared_permission_has_an_authenticated_policy()
    {
        var services = new ServiceCollection();
        services.AddGameNetAuthorization();

        await using var provider = services.BuildServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();

        foreach (var permission in typeof(Permissions)
                     .GetFields(System.Reflection.BindingFlags.Public |
                                System.Reflection.BindingFlags.Static)
                     .Where(x => x.FieldType == typeof(string))
                     .Select(x => (string)x.GetValue(null)!)
                     .Distinct(StringComparer.Ordinal))
        {
            var policy = await policyProvider.GetPolicyAsync(permission);

            Assert.NotNull(policy);
            Assert.Contains(policy!.Requirements, x => x is DenyAnonymousAuthorizationRequirement);
            Assert.Contains(policy.Requirements, x =>
                x is ClaimsAuthorizationRequirement claim &&
                claim.ClaimType == "permission" &&
                claim.AllowedValues?.Contains(permission, StringComparer.Ordinal) == true);
        }
    }
}
