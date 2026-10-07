# Transactional Outbox Foundation

Business state changes that need reliable external publication append an OutboxMessage in the same database transaction.

## Foundation runtime
- Outbox rows are durable PostgreSQL records.
- Every event has a unique EventId.
- Foundation provides an atomic PostgreSQL claim/lease boundary using row locking and SKIP LOCKED.
- A worker that loses or expires a lease can be reclaimed without changing authoritative business state.
- Mark-published requires the current lease token.

## Publication rule
The database transaction is authoritative. Publishing is downstream of commit; external delivery is never the transaction boundary.

The first realtime vertical slice adds the transport-specific Agent/SignalR publisher and retry policy on top of this durable boundary.
