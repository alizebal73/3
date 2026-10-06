namespace GameNet.Server.Infrastructure.Audit;

public interface IAuditWriter
{
    void Append(AuditEntry entry);
}
