using GameNet.Server.Persistence;

namespace GameNet.Server.Infrastructure.Outbox;

public sealed class EfOutboxWriter(GameNetDbContext dbContext) : IOutboxWriter
{
    public OutboxMessage Append(
        string type,
        string payloadJson,
        DateTimeOffset occurredAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(payloadJson);

        var message = new OutboxMessage
        {
            Type = type,
            PayloadJson = payloadJson,
            OccurredAtUtc = occurredAtUtc
        };

        dbContext.OutboxMessages.Add(message);
        return message;
    }
}
