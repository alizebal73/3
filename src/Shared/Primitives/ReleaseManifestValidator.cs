using System.Text.RegularExpressions;
using GameNet.Shared.Contracts.V1.System;

namespace GameNet.Shared.Primitives;

public static partial class ReleaseManifestValidator
{
    public static void Validate(ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        if (!Version.TryParse(manifest.ProductVersion, out _))
            throw new ArgumentException("ProductVersion must be a valid semantic version.", nameof(manifest));

        if (manifest.SchemaVersion < 0)
            throw new ArgumentOutOfRangeException(nameof(manifest.SchemaVersion));

        if (manifest.AgentProtocolVersion < 0)
            throw new ArgumentOutOfRangeException(nameof(manifest.AgentProtocolVersion));

        if (string.IsNullOrWhiteSpace(manifest.ApiContractVersion))
            throw new ArgumentException("ApiContractVersion is required.", nameof(manifest));

        if (manifest.MigrationFromSchemaVersion < 0 ||
            manifest.MigrationToSchemaVersion < manifest.MigrationFromSchemaVersion)
        {
            throw new ArgumentException("Migration schema range is invalid.", nameof(manifest));
        }

        if (manifest.Files is null || manifest.Files.Count == 0)
            throw new ArgumentException("Release manifest must contain at least one file.", nameof(manifest));

        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in manifest.Files)
        {
            ValidateRelativePath(file.RelativePath);

            if (!paths.Add(NormalizePath(file.RelativePath)))
                throw new ArgumentException(
                    $"Release manifest contains a duplicate file path: {file.RelativePath}",
                    nameof(manifest));

            if (file.SizeBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(manifest), "Release file size cannot be negative.");

            if (!Sha256Regex().IsMatch(file.Sha256))
                throw new ArgumentException(
                    $"Release file checksum must be a 64-character SHA-256 hex value: {file.RelativePath}",
                    nameof(manifest));
        }
    }

    private static void ValidateRelativePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Release file path is required.");

        var normalized = NormalizePath(value);

        if (Path.IsPathRooted(value) ||
            normalized.StartsWith("../", StringComparison.Ordinal) ||
            normalized == ".." ||
            normalized.Contains("/../", StringComparison.Ordinal) ||
            normalized.Contains(':'))
        {
            throw new ArgumentException($"Release file path must stay inside the package: {value}");
        }
    }

    private static string NormalizePath(string value) =>
        value.Replace('\\', '/').TrimStart('/');

    [GeneratedRegex("^[0-9a-fA-F]{64}$", RegexOptions.CultureInvariant)]
    private static partial Regex Sha256Regex();
}
