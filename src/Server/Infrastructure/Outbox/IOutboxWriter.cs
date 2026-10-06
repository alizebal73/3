namespace GameNet.Server.Infrastructure.Outbox;

public interface IOutboxWriter
{
    OutboxMessage Append(
        string type,
        string payloadJson,
        DateTimeOffset occurredAtUtc);
}
