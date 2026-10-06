using GameNet.Server.Persistence;

namespace GameNet.Server.Infrastructure.Audit;

public sealed class EfAuditWriter(GameNetDbContext dbContext) : IAuditWriter
{
    public void Append(AuditEntry entry) => dbContext.AuditEntries.Add(entry);
}
