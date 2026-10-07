using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using GameNet.Agent.Transport;

namespace GameNet.Agent.Identity;

public sealed class AgentCredentialStore(
    IOptions<AgentIdentityOptions> identityOptions,
    IOptions<AgentTransportOptions> transportOptions) : IAgentCredentialStore
{
    public async Task<string> GetOrBootstrapAsync(
        CancellationToken cancellationToken = default)
    {
        var root = Path.GetFullPath(identityOptions.Value.RootPath);
        Directory.CreateDirectory(root);
        var path = GetPath(root);

        if (File.Exists(path))
        {
            var protectedBytes = await File.ReadAllBytesAsync(path, cancellationToken);
            try
            {
                var clear = ProtectedData.Unprotect(
                    protectedBytes,
                    optionalEntropy: null,
                    DataProtectionScope.CurrentUser);

                var stored = Encoding.UTF8.GetString(clear);
                if (!string.IsNullOrWhiteSpace(stored))
                    return stored;
            }
            catch (CryptographicException exception)
            {
                throw new InvalidOperationException(
                    "The stored Agent credential cannot be decrypted under the current Windows service identity.",
                    exception);
            }
        }

        var variableName =
            transportOptions.Value.BootstrapCredentialEnvironmentVariableName;

        var bootstrap = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(bootstrap))
        {
            throw new InvalidOperationException(
                $"No Agent bootstrap credential is available. Provision the device and set {variableName} for the Agent service identity.");
        }

        await SaveAsync(bootstrap, cancellationToken);
        return bootstrap;
    }

    public async Task SaveAsync(
        string secret,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            throw new ArgumentException(
                "Agent credential must contain at least 32 characters.",
                nameof(secret));

        var root = Path.GetFullPath(identityOptions.Value.RootPath);
        Directory.CreateDirectory(root);

        var protectedBytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(secret),
            optionalEntropy: null,
            DataProtectionScope.CurrentUser);

        var path = GetPath(root);
        var tempPath = path + ".tmp";

        await File.WriteAllBytesAsync(
            tempPath,
            protectedBytes,
            cancellationToken);

        File.Move(tempPath, path, overwrite: true);
    }

    private static string GetPath(string root) =>
        Path.Combine(root, "credential.bin");
}
