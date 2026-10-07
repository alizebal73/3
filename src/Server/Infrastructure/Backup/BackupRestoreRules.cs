namespace GameNet.Server.Infrastructure.Backup;

public static class BackupRestoreRules
{
    public static void EnsureRestoreAllowed(
        bool verificationPassed,
        bool explicitOperatorApproval,
        bool serverStopped)
    {
        if (!verificationPassed)
            throw new InvalidOperationException("Only verified backups may be restored.");

        if (!explicitOperatorApproval)
            throw new UnauthorizedAccessException("Restore requires explicit authorization.");

        if (!serverStopped)
            throw new InvalidOperationException("Restore requires the authoritative Server to be stopped.");
    }
}
