using System.Text.Json;
using Microsoft.Extensions.Options;

namespace GameNet.Agent.Identity;

public interface IAgentIdentityStore
{
    Task<AgentIdentity> GetOrCreateAsync(
        CancellationToken cancellationToken = default);
}

public sealed class AgentIdentityStore(
    IOptions<AgentIdentityOptions> options) : IAgentIdentityStore
{
    public async Task<AgentIdentity> GetOrCreateAsync(
        CancellationToken cancellationToken = default)
    {
        var rootPath = Path.GetFullPath(options.Value.RootPath);
        Directory.CreateDirectory(rootPath);

        var path = Path.Combine(rootPath, "identity.json");

        if (File.Exists(path))
        {
            await using var read = File.OpenRead(path);
            var stored = await JsonSerializer.DeserializeAsync<IdentityFile>(
                read,
                cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(stored?.DeviceId))
                return AgentIdentity.FromDeviceId(stored.DeviceId);
        }

        var identity = AgentIdentity.FromDeviceId(Guid.NewGuid().ToString("N"));
        var temp = path + ".tmp";

        await File.WriteAllTextAsync(
            temp,
            JsonSerializer.Serialize(new IdentityFile(identity.DeviceId)),
            cancellationToken);

        File.Move(temp, path, overwrite: true);

        return identity;
    }

    private sealed record IdentityFile(string DeviceId);
}
