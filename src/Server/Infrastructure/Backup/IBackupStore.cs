namespace GameNet.Server.Infrastructure.Backup;

public sealed record BackupArtifact(
    string BackupId,
    string FilePath,
    string Sha256,
    long SizeBytes,
    DateTimeOffset CreatedAtUtc);

public interface IBackupStore
{
    Task<BackupArtifact> CreateAsync(CancellationToken cancellationToken = default);
    Task<bool> VerifyAsync(BackupArtifact artifact, CancellationToken cancellationToken = default);
}
