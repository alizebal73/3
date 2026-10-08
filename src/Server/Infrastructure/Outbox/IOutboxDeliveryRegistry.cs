namespace GameNet.Server.Infrastructure.Outbox;

public interface IOutboxDeliveryRegistry
{
    bool TryGet(string messageType, out IOutboxDeliveryHandler? handler);
}