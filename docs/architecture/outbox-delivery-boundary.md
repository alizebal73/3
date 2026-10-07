# Outbox Delivery Boundary

GameNet 3 uses the Outbox table as the durable handoff point for side effects that must not be lost when a database mutation commits.

## Foundation boundary

The Foundation owns:

1. atomic Outbox insertion in the same database transaction as the business mutation;
2. concurrent batch claiming with PostgreSQL row locking;
3. lease tokens and lease expiry;
4. stale-owner fencing when a message is marked Published;
5. the delivery boundary and its fail-closed behavior.

The Foundation does not provide a fake publisher and never treats a missing handler as successful delivery.

## Delivery contract

Every durable external side effect is represented by an IOutboxDeliveryHandler registered for its exact message Type.

A handler must:

- use OutboxMessage.EventId as its idempotency key;
- perform or submit the external durable side effect;
- return successfully only when that side effect has reached the handler defined acceptance point.

Only after the handler returns successfully does the Server call MarkPublishedAsync.

If a message has no handler, or the handler fails, the message remains unpublished and its lease is allowed to expire. It is never silently marked as delivered.

This gives the platform an explicit at-least-once delivery boundary without creating a no-op or mock publisher.

## Activation policy

OutboxDelivery:Enabled defaults to false.

This is intentional: the first product slice may create durable internal records without enabling arbitrary external side effects. When the first real external side effect is introduced, its handler must be implemented, registered, tested for idempotency, and the delivery worker explicitly enabled.

No business module may call MarkPublishedAsync directly.

## Failure semantics

- claim succeeds, handler succeeds, publish mark succeeds: message is complete;
- claim succeeds, handler fails: message remains unpublished;
- claim succeeds, process crashes before publish mark: message becomes eligible after lease expiry;
- stale owner attempts the publish mark: PostgreSQL fencing rejects it;
- unknown message type: message remains unpublished.

The Outbox is therefore a durable at-least-once delivery mechanism, not an exactly-once transport.