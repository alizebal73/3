using System.Security.Cryptography;
using GameNet.Shared.Contracts.V1.System;

namespace GameNet.Shared.Primitives;

public static class ReleasePackageVerifier
{
    public static async Task VerifyAsync(
        string packageRoot,
        ReleaseManifest manifest,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageRoot);
        ReleaseManifestValidator.Validate(manifest);

        var root = Path.GetFullPath(packageRoot);

        foreach (var file in manifest.Files)
        {
            var path = GetSafePath(root, file.RelativePath);

            if (!File.Exists(path))
                throw new InvalidOperationException(
                    $"Release package is missing '{file.RelativePath}'.");

            var info = new FileInfo(path);
            if (info.Length != file.SizeBytes)
                throw new InvalidOperationException(
                    $"Release package size mismatch for '{file.RelativePath}'.");

            await using var stream = File.OpenRead(path);
            var hash = await SHA256.HashDataAsync(stream, cancellationToken);
            var actualSha = Convert.ToHexString(hash);

            if (!string.Equals(actualSha, file.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Release package checksum mismatch for '{file.RelativePath}'.");
        }
    }

    private static string GetSafePath(string root, string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(root, relativePath));

        var rootWithSeparator = root.EndsWith(
            Path.DirectorySeparatorChar,
            StringComparison.Ordinal)
            ? root
            : root + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Release package path escapes its root: {relativePath}");
        }

        return fullPath;
    }
}
