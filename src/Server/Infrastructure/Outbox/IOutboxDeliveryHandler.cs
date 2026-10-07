namespace GameNet.Server.Infrastructure.Outbox;

/// <summary>
/// Handles one durable Outbox message. Delivery is at-least-once:
/// implementations must use the message EventId as their idempotency key
/// before causing an external durable side effect.
/// </summary>
public interface IOutboxDeliveryHandler
{
    string MessageType { get; }

    Task DeliverAsync(OutboxMessage message, CancellationToken cancellationToken = default);
}