# GameNet Business Module Blueprints

No business implementation starts unless the module's invariant, state model and transaction owner are known.

## Customers
Owns customer identity, profile, PIN/password lifecycle, notes and status.
Invariant: one durable customer identity; credentials are never station/session identity.

## Users/Auth
Owns operator/admin identities, permissions, authentication and approval requirements.
Invariant: sensitive mutations are authorized by the Server.

## Agents
Owns DeviceId, pairing, credential rotation, connection lease, heartbeat and command delivery.
Invariant: DeviceId is durable; ConnectionId is ephemeral; one authoritative connection per DeviceId.

## Stations
Owns PC/PS5/foosball identity, category, availability, maintenance and Agent binding.
Invariant: a station cannot have two active authoritative Sessions.

## Sessions
Owns customer login, usage, pause/resume, transfer and end/settlement orchestration.
Invariant: Session ownership moves atomically with Station and Agent ownership.

## Tariffs/Vip
Owns price rules, effective dates, normal/VIP rules and usage entitlements.
Invariant: historical charges use an immutable pricing decision captured at charge time.

## Billing/Wallet
Owns money mutations, invoices, payments, reversals, debt and wallet ledger.
Invariant: every financial mutation is transactional, auditable and idempotent.

## Inventory/Buffet
Inventory owns stock truth and provenance. Buffet owns sale workflows.
Invariant: a sale decrements the exact source StockArea and reversal restores it when provenance is known.

## Reports
Read-only queries/projections only. Reports never become a source of truth.

## Settings
Owns validated shop/runtime configuration and approved feature settings.
Invariant: settings changes are permission-controlled and audited.

## Approvals/Backup
Approvals owns controlled high-risk approvals. Backup owns backup creation, verification, retention and restore operations.

## Required lifecycle work before implementation
Every stateful module defines states/transitions, commands/queries, permissions, transaction boundary, idempotency scope, concurrency rule, audit events, recovery/reconciliation, operator-visible status and integration contracts.
