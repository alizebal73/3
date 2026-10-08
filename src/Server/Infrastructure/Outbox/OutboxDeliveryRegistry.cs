namespace GameNet.Server.Infrastructure.Outbox;

public sealed class OutboxDeliveryRegistry : IOutboxDeliveryRegistry
{
    private readonly IReadOnlyDictionary<string, IOutboxDeliveryHandler> _handlers;

    public OutboxDeliveryRegistry(IEnumerable<IOutboxDeliveryHandler> handlers)
    {
        var map = new Dictionary<string, IOutboxDeliveryHandler>(StringComparer.Ordinal);

        foreach (var handler in handlers)
        {
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentException.ThrowIfNullOrWhiteSpace(handler.MessageType);

            if (!map.TryAdd(handler.MessageType, handler))
                throw new InvalidOperationException(
                    $"Duplicate Outbox delivery handler registered for message type '{handler.MessageType}'.");
        }

        _handlers = map;
    }

    public bool TryGet(string messageType, out IOutboxDeliveryHandler? handler)
    {
        if (string.IsNullOrWhiteSpace(messageType))
        {
            handler = null;
            return false;
        }

        return _handlers.TryGetValue(messageType, out handler);
    }
}