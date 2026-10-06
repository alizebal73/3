# Transactional Outbox Foundation

Business state changes that need reliable external publication must append an OutboxMessage in the same database transaction.

Rules:
- The database transaction is authoritative.
- Outbox rows are durable PostgreSQL records, not an in-memory queue.
- An event has a unique EventId.
- Publishing is downstream of commit; external delivery must never be used as the transaction boundary.
- Failed publication may be retried by a future dispatcher without changing the authoritative business transaction.
- The first realtime vertical slice must add a durable claim/lease mechanism and a transport-specific publisher before emitting business events.
- A business use case must never mutate authoritative state only through the process-local background queue.
