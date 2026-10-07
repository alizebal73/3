using GameNet.Server.Infrastructure.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GameNet.Server.UnitTests;

public sealed class GameNetOptionsValidatorTests
{
    [Fact]
    public void Production_rejects_disabled_authentication()
    {
        var validator = new GameNetOptionsValidator(
            new TestHostEnvironment("Production"));

        var result = validator.Validate(
            Options.DefaultName,
            new GameNetOptions());

        Assert.False(result.Succeeded);
        Assert.Contains(
            "Production Server authentication must be enabled.",
            result.FailureMessage,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Development_can_keep_authentication_disabled_for_local_foundation_work()
    {
        var validator = new GameNetOptionsValidator(
            new TestHostEnvironment("Development"));

        var result = validator.Validate(
            Options.DefaultName,
            new GameNetOptions());

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Production_accepts_enabled_authentication_with_required_secrets()
    {
        var validator = new GameNetOptionsValidator(
            new TestHostEnvironment("Production"));

        var result = validator.Validate(
            Options.DefaultName,
            new GameNetOptions
            {
                Authentication = new AuthenticationOptions
                {
                    Enabled = true,
                    Issuer = "https://issuer.gamenet.local",
                    Audience = "gamenet-agent",
                    SigningKey = new string('s', 32)
                },
                Agent = new AgentOptions
                {
                    ProvisioningKey = new string('p', 32)
                }
            });

        Assert.True(result.Succeeded);
    }

    private sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "GameNet.Server.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}
