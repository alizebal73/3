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
