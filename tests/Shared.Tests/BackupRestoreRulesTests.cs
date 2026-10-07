using GameNet.Server.Infrastructure.Backup;

namespace GameNet.Shared.Tests;

public sealed class BackupRestoreRulesTests
{
    [Fact]
    public void Restore_requires_verified_backup_approval_and_stopped_server()
    {
        BackupRestoreRules.EnsureRestoreAllowed(
            verificationPassed: true,
            explicitOperatorApproval: true,
            serverStopped: true);
    }

    [Fact]
    public void Unverified_backup_is_rejected()
    {
        Assert.Throws<InvalidOperationException>(() =>
            BackupRestoreRules.EnsureRestoreAllowed(true: false, explicitOperatorApproval: true, serverStopped: true));
    }

    [Fact]
    public void Running_server_is_rejected()
    {
        Assert.Throws<InvalidOperationException>(() =>
            BackupRestoreRules.EnsureRestoreAllowed(true, true, false));
    }
}
