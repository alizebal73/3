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


## Games
Owns the authoritative game catalog, install/runtime metadata, allowed launch policy and version/compatibility metadata.
Invariant: the Agent never decides independently whether a game may launch; Server policy and Session ownership authorize execution.

## GameAccounts
Owns GameNet-managed game-account pools, allocation, release, cooldown/locking policy and audit references.
Invariant: an account assignment belongs to one authoritative allocation at a time and must be recoverable after client/network failure.

## ClientControl
Owns Server-side commands that control a client machine: lock/unlock, process launch/stop, restart/shutdown, maintenance mode and client-policy refresh.
Invariant: every command is bound to DeviceId + current authoritative Agent lease + authorized actor/operation. Agent cannot originate authoritative commands.

## Client configuration
Client configuration is split into Server-owned policy and Agent-local execution settings.
Server-owned policy is authoritative and versioned. Agent-local files are only cached/execution state and can be replaced during reconciliation/update.

## Client diagnostics
Owns health, software version, execution capability and diagnostic evidence. Diagnostic data never becomes the authority for billing/session state.
