# Module Boundary Manifest

This manifest is the machine-reviewed dependency contract for GameNet 3.

## Module ownership
Agents: registration, pairing, leases, command delivery.
Approvals: approval requests for controlled privileged actions.
Auth: authentication, credentials, permissions.
Backup: backup, verification and restore metadata.
Billing: charges, invoices, payments, reversals, settlement.
Buffet: sale-facing product workflows.
ClientControl: Server-side authorization of client-machine control commands and policies.
Customers: customer identity, profile and lifecycle.
GameAccounts: GameNet-managed game-account pools, allocation, release and audit references.
Games: authoritative game catalog, launch policy, runtime metadata and compatibility.
Inventory: stock areas, movements and provenance.
Reports: read-only reporting queries/projections.
Sessions: customer logins, active usage, transfer and end.
Settings: validated operational configuration.
Stations: PC/PS5/foosball definitions, availability and health.
Tariffs: prices and effective-date tariff rules.
Users: owner/admin/operator administration.
Vip: VIP plans, entitlements and limits.
Wallet: wallet ledger, balance and top-up/debit operations.

## Allowed dependency direction
Domain -> Shared
Application -> Domain + Shared + explicitly approved contracts
Infrastructure -> Application + Domain + Persistence + Shared
Api -> Application + Shared
Composition -> registration and routing only
Desktop -> Shared + approved Server contracts
Agent -> Shared + approved Server contracts

## Client capability boundary
Client execution is split from Server authority:
- Games owns the catalog and authorization decision.
- GameAccounts owns allocation and lifecycle.
- ClientControl owns the authoritative command decision.
- Agent Capabilities only execute and observe.
- Desktop only requests/observes through Server contracts.

## Forbidden
- Domain/Application/Api -> EF Core, DbContext or persistence implementation.
- One module -> another module's Domain, Infrastructure or Api.
- Desktop/Agent -> Server implementation assemblies/namespaces.
- UI -> authoritative money, Session, inventory or permission calculations.
- Authoritative transaction -> network, SignalR, Agent, process, filesystem or email side effects.
- Agent capability -> authoritative account allocation, pricing, Session settlement or permission decision.
- Wall-clock static APIs outside time infrastructure.

## Enforcement
The architecture guard is a repository gate. New cross-module dependencies require an explicit contract and guard/test coverage.
