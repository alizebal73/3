# Realtime and Agent Connection Reliability

Realtime transport is not authoritative state.

## Identity
- DeviceId is durable machine identity.
- ConnectionId is an ephemeral transport connection.
- LeaseToken fences stale connections.
- IP address is never identity.

## Single-owner rule
For one DeviceId, at most one connection lease is authoritative at a time.

A newly acquired lease supersedes an expired/stale lease. A stale connection cannot renew, release or acknowledge commands for the newer lease.

## Lifecycle
Connect -> AcquireLease -> Heartbeat/Renew -> Commands/Acks -> Reconcile -> Release/Expire.

Reconnect always re-establishes the lease first, then asks the Server for authoritative state and reconciliation commands.

## Delivery
Authoritative mutations commit to PostgreSQL first. Durable Outbox publication happens after commit. Agent commands carry unique CommandIds and the receiving Agent acknowledges them.

A lost acknowledgement is retried according to command idempotency policy. The Agent never turns an acknowledgement into authoritative business state without Server validation.

## Foundation boundary
Foundation defines the protocol and lease boundary. The first Agent vertical slice must provide the persistence-backed lease store and transport publisher, plus concurrent fencing/reconnect tests against real PostgreSQL.
