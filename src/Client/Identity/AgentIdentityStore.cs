using System.Text.Json;
using GameNet.Agent;

namespace GameNet.Agent.Identity;

public interface IAgentIdentityStore
{
    Task<AgentIdentity> GetOrCreateAsync(CancellationToken cancellationToken = default);
}

public sealed class AgentIdentityStore : IAgentIdentityStore
{
    private readonly string _path =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "GameNet Manager",
            "Agent",
            "identity.json");

    public async Task<AgentIdentity> GetOrCreateAsync(
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

        if (File.Exists(_path))
        {
            await using var read = File.OpenRead(_path);
            var stored = await JsonSerializer.DeserializeAsync<IdentityFile>(
                read,
                cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(stored?.DeviceId))
                return AgentIdentity.FromDeviceId(stored.DeviceId);
        }

        var identity = AgentIdentity.FromDeviceId(Guid.NewGuid().ToString("N"));

        var temp = _path + ".tmp";
        await File.WriteAllTextAsync(
            temp,
            JsonSerializer.Serialize(new IdentityFile(identity.DeviceId)),
            cancellationToken);

        File.Move(temp, _path, overwrite: true);

        return identity;
    }

    private sealed record IdentityFile(string DeviceId);
}
