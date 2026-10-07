using GameNet.Agent.Transport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GameNet.Agent.Tests;

public sealed class AgentTransportOptionsTests
{
    [Fact]
    public void Production_rejects_http_even_when_development_flag_is_set()
    {
        var validator = new AgentTransportOptionsValidator(
            new TestHostEnvironment("Production"));

        var result = validator.Validate(
            Options.DefaultName,
            new AgentTransportOptions
            {
                ServerBaseUrl = "http://127.0.0.1:5080",
                AllowInsecureHttpForDevelopment = true
            });

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Development_accepts_http_only_when_explicitly_enabled()
    {
        var validator = new AgentTransportOptionsValidator(
            new TestHostEnvironment("Development"));

        var disabled = validator.Validate(
            Options.DefaultName,
            new AgentTransportOptions
            {
                ServerBaseUrl = "http://127.0.0.1:5080",
                AllowInsecureHttpForDevelopment = false
            });

        Assert.False(disabled.Succeeded);

        var enabled = validator.Validate(
            Options.DefaultName,
            new AgentTransportOptions
            {
                ServerBaseUrl = "http://127.0.0.1:5080",
                AllowInsecureHttpForDevelopment = true
            });

        Assert.True(enabled.Succeeded);
    }

    private sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "GameNet.Agent.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
