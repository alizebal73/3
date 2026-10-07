using GameNet.Server.Infrastructure.Audit;
using GameNet.Server.Infrastructure.Outbox;
using GameNet.Server.Persistence.Configurations;
using GameNet.Server.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.Persistence;

public sealed class GameNetDbContext(DbContextOptions<GameNetDbContext> options) : DbContext(options)
{
    public DbSet<AgentCredential> AgentCredentials => Set<AgentCredential>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        RejectAuditMutation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
        => SaveChanges(acceptAllChangesOnSuccess: true);

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        RejectAuditMutation();
        return await base.SaveChangesAsync(
            acceptAllChangesOnSuccess,
            cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => SaveChangesAsync(true, cancellationToken);

    private void RejectAuditMutation()
    {
        var mutation = ChangeTracker
            .Entries<AuditEntry>()
            .FirstOrDefault(entry =>
                entry.State is EntityState.Modified or EntityState.Deleted);

        if (mutation is not null)
            throw new InvalidOperationException(
                "Audit entries are append-only and cannot be modified or deleted.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AgentCredentialConfiguration());
        modelBuilder.ApplyConfiguration(new AuditEntryConfiguration());
        modelBuilder.ApplyConfiguration(new IdempotencyRecordConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
