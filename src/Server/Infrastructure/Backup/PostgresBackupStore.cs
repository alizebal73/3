using System.Diagnostics;
using System.Security.Cryptography;
using GameNet.Server.Infrastructure.Configuration;
using GameNet.Server.Infrastructure.Time;
using Microsoft.Extensions.Options;
using Npgsql;

namespace GameNet.Server.Infrastructure.Backup;

public sealed class PostgresBackupStore(
    IOptions<GameNetOptions> options,
    IGameClock clock) : IBackupStore
{
    public async Task<BackupArtifact> CreateAsync(
        CancellationToken cancellationToken = default)
    {
        var backupRoot = Path.GetFullPath(options.Value.BackupRoot);
        Directory.CreateDirectory(backupRoot);

        var backupId = $"{clock.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}";
        var filePath = Path.Combine(backupRoot, $"{backupId}.dump");

        var builder = CreateConnectionBuilder();
        await RunPgToolAsync(
            options.Value.Backup.PgDumpPath,
            builder,
            filePath,
            restore: false,
            cancellationToken);

        var artifact = await CreateArtifactAsync(backupId, filePath, cancellationToken);

        if (!await VerifyAsync(artifact, cancellationToken))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // Preserve the original verification failure.
            }

            throw new InvalidOperationException(
                $"PostgreSQL backup verification failed: {artifact.FilePath}");
        }

        return artifact;
    }

    public async Task<bool> VerifyAsync(
        BackupArtifact artifact,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(artifact);

        if (!File.Exists(artifact.FilePath))
            return false;

        var actualSize = new FileInfo(artifact.FilePath).Length;
        if (actualSize != artifact.SizeBytes)
            return false;

        await using var stream = File.OpenRead(artifact.FilePath);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        var actualSha = Convert.ToHexString(hash);

        if (!string.Equals(actualSha, artifact.Sha256, StringComparison.OrdinalIgnoreCase))
            return false;

        await RunPgToolAsync(
            options.Value.Backup.PgRestorePath,
            CreateConnectionBuilder(),
            artifact.FilePath,
            restore: true,
            cancellationToken,
            listOnly: true);

        return true;
    }

    public async Task RestoreAsync(
        BackupArtifact artifact,
        string targetConnectionString,
        bool explicitOperatorApproval,
        bool serverStopped,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetConnectionString);

        if (!await VerifyAsync(artifact, cancellationToken))
            throw new InvalidOperationException("Only a verified backup may be restored.");

        BackupRestoreRules.EnsureRestoreAllowed(
            verificationPassed: true,
            explicitOperatorApproval,
            serverStopped);

        var source = CreateConnectionBuilder();
        var target = CreateConnectionBuilder(targetConnectionString);

        if (SameDatabase(source, target))
        {
            throw new InvalidOperationException(
                "Restore target must be isolated from the authoritative source database.");
        }

        await RunPgToolAsync(
            options.Value.Backup.PgRestorePath,
            target,
            artifact.FilePath,
            restore: true,
            cancellationToken,
            listOnly: false);
    }

    private async Task<BackupArtifact> CreateArtifactAsync(
        string backupId,
        string filePath,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);

        return new BackupArtifact(
            backupId,
            filePath,
            Convert.ToHexString(hash),
            new FileInfo(filePath).Length,
            clock.UtcNow);
    }

    private NpgsqlConnectionStringBuilder CreateConnectionBuilder() =>
        CreateConnectionBuilder(
            options.Value.DatabaseConnectionString
            ?? throw new InvalidOperationException(
                "GameNet:DatabaseConnectionString is required for PostgreSQL backup operations."));

    private static NpgsqlConnectionStringBuilder CreateConnectionBuilder(string connectionString) =>
        new(connectionString);

    private static async Task RunPgToolAsync(
        string executable,
        NpgsqlConnectionStringBuilder connection,
        string artifactPath,
        bool restore,
        CancellationToken cancellationToken,
        bool listOnly = false)
    {
        if (string.IsNullOrWhiteSpace(executable))
            throw new InvalidOperationException("PostgreSQL tool path is not configured.");

        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (restore)
        {
            startInfo.ArgumentList.Add("--dbname");
            startInfo.ArgumentList.Add(BuildDatabaseTarget(connection));
            startInfo.ArgumentList.Add("--no-owner");
            if (listOnly)
            {
                startInfo.ArgumentList.Add("--list");
            }
            else
            {
                startInfo.ArgumentList.Add("--clean");
                startInfo.ArgumentList.Add("--if-exists");
            }

            startInfo.ArgumentList.Add(artifactPath);
        }
        else
        {
            startInfo.ArgumentList.Add("--format");
            startInfo.ArgumentList.Add("custom");
            startInfo.ArgumentList.Add("--file");
            startInfo.ArgumentList.Add(artifactPath);
            startInfo.ArgumentList.Add("--host");
            startInfo.ArgumentList.Add(connection.Host ?? "localhost");
            startInfo.ArgumentList.Add("--port");
            startInfo.ArgumentList.Add(connection.Port.ToString());
            startInfo.ArgumentList.Add("--username");
            startInfo.ArgumentList.Add(connection.Username ?? string.Empty);
            startInfo.ArgumentList.Add("--dbname");
            startInfo.ArgumentList.Add(connection.Database ?? string.Empty);
        }

        if (!string.IsNullOrEmpty(connection.Password))
            startInfo.Environment["PGPASSWORD"] = connection.Password;

        if (connection.SslMode != SslMode.Disable)
        {
            startInfo.Environment["PGSSLMODE"] = connection.SslMode switch
            {
                SslMode.Allow => "allow",
                SslMode.Prefer => "prefer",
                SslMode.Require => "require",
                SslMode.VerifyCA => "verify-ca",
                SslMode.VerifyFull => "verify-full",
                _ => "prefer"
            };
        }

        using var process = new Process { StartInfo = startInfo };

        if (!process.Start())
            throw new InvalidOperationException($"Unable to start PostgreSQL tool '{executable}'.");

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"PostgreSQL tool '{executable}' failed with exit code {process.ExitCode}. {error}".Trim());
        }
    }

    private static bool SameDatabase(
        NpgsqlConnectionStringBuilder left,
        NpgsqlConnectionStringBuilder right) =>
        string.Equals(left.Host, right.Host, StringComparison.OrdinalIgnoreCase) &&
        left.Port == right.Port &&
        string.Equals(left.Database, right.Database, StringComparison.OrdinalIgnoreCase);

    private static string BuildDatabaseTarget(NpgsqlConnectionStringBuilder connection)
    {
        var target = new NpgsqlConnectionStringBuilder
        {
            Host = connection.Host,
            Port = connection.Port,
            Username = connection.Username,
            Database = connection.Database,
            SslMode = connection.SslMode,
            TrustServerCertificate = connection.TrustServerCertificate
        };

        return target.ConnectionString;
    }
}
