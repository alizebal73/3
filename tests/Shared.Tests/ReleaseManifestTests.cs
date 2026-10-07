using System.Security.Cryptography;
using GameNet.Shared.Contracts.V1.System;
using GameNet.Shared.Primitives;

namespace GameNet.Shared.Tests;

public sealed class ReleaseManifestTests
{
    [Fact]
    public void Valid_manifest_is_accepted()
    {
        var manifest = CreateManifest();

        ReleaseManifestValidator.Validate(manifest);
    }

    [Fact]
    public void Semantic_version_prerelease_is_accepted()
    {
        ReleaseManifestValidator.Validate(
            CreateManifestWithVersion("1.2.3-rc.1"));
    }

    [Fact]
    public void Non_semantic_version_is_rejected()
    {
        Assert.Throws<ArgumentException>(() =>
            ReleaseManifestValidator.Validate(
                CreateManifestWithVersion("1.2")));
    }

    [Fact]
    public async Task Release_package_files_are_verified()
    {
        var root = Path.Combine(Path.GetTempPath(), "gamenet-release-test", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "server"));

        try
        {
            var path = Path.Combine(root, "server", "GameNet.dll");
            await File.WriteAllTextAsync(path, "foundation");
            var bytes = await File.ReadAllBytesAsync(path);
            var manifest = CreateManifest(
                new ReleaseFileEntry(
                    "server/GameNet.dll",
                    Convert.ToHexString(SHA256.HashData(bytes)),
                    bytes.Length));

            await ReleasePackageVerifier.VerifyAsync(root, manifest);
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Release_package_checksum_mismatch_is_rejected()
    {
        var root = Path.Combine(Path.GetTempPath(), "gamenet-release-test", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "server"));

        try
        {
            var path = Path.Combine(root, "server", "GameNet.dll");
            await File.WriteAllTextAsync(path, "foundation");

            var manifest = CreateManifest(
                new ReleaseFileEntry(
                    "server/GameNet.dll",
                    new string('a', 64),
                    new FileInfo(path).Length));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                ReleasePackageVerifier.VerifyAsync(root, manifest));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Traversal_path_is_rejected()
    {
        var manifest = CreateManifest(
            new ReleaseFileEntry("../outside.dll", new string('a', 64), 1));

        Assert.Throws<ArgumentException>(() =>
            ReleaseManifestValidator.Validate(manifest));
    }

    [Fact]
    public void Duplicate_paths_are_rejected_case_insensitively()
    {
        var manifest = CreateManifest(
            new ReleaseFileEntry("server/GameNet.dll", new string('a', 64), 1),
            new ReleaseFileEntry("SERVER/gamenet.dll", new string('b', 64), 2));

        Assert.Throws<ArgumentException>(() =>
            ReleaseManifestValidator.Validate(manifest));
    }

    [Fact]
    public void Invalid_sha256_is_rejected()
    {
        var manifest = CreateManifest(
            new ReleaseFileEntry("server/GameNet.dll", "not-a-checksum", 1));

        Assert.Throws<ArgumentException>(() =>
            ReleaseManifestValidator.Validate(manifest));
    }

    private static ReleaseManifest CreateManifestWithVersion(
        string version,
        params ReleaseFileEntry[] files) =>
        new(
            version,
            1,
            "v1",
            1,
            1,
            1,
            files.Length == 0
                ? [new ReleaseFileEntry("server/GameNet.dll", new string('a', 64), 1)]
                : files);

    private static ReleaseManifest CreateManifest(params ReleaseFileEntry[] files) =>

        new(
            "1.0.0",
            1,
            "v1",
            1,
            1,
            1,
            files.Length == 0
                ? [new ReleaseFileEntry("server/GameNet.dll", new string('a', 64), 1)]
                : files);
}
