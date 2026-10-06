# Transaction Rules

Authoritative database mutations must be completed inside an explicit use-case transaction boundary.

Rules:
- The use-case handler owns the business operation; the transaction coordinator owns BEGIN/COMMIT/ROLLBACK.
- Serializable isolation is the default for contention-sensitive operations until a narrower isolation strategy is proven safe.
- Serialization failures and deadlocks may be retried only when the use case is retry-safe and idempotent.
- The transaction callback must not perform network calls, SignalR sends, Agent commands, filesystem side effects, or other irreversible external effects.
- External publication is represented by a durable OutboxMessage written in the same transaction and published only after commit.
- Idempotency records, authoritative business state, audit rows and outbox rows that must be atomic belong to the same transaction.
- A transaction callback must not call SaveChanges directly; the coordinator performs the final persistence step.
- After a failed serialization retry, tracked EF state is cleared before the operation is evaluated again.
