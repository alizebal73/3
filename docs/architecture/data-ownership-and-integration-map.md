# Data Ownership and Integration Map

The Server owns all authoritative state.

## Authoritative data

| Data | Owner | Consumers | Mutation path |
|---|---|---|---|
| Customer identity/profile | Customers | Sessions, Billing, Desktop | Server use case |
| Customer credentials/login identity | Customers/Auth | Sessions, Agent | Server use case |
| User/operator identity | Users/Auth | every privileged module | Server use case |
| Device identity/pairing | Agents | Stations, Sessions | Server use case |
| Station identity/status | Stations | Sessions, Desktop, Agent | Server use case |
| Session ownership/state | Sessions | Stations, Billing, Agent | Server use case |
| Tariff/rate definitions | Tariffs | Sessions/Billing | Server use case |
| VIP entitlements | Vip | Tariffs/Sessions | Server use case |
| Money ledger | Wallet/Billing | Sessions/Buffet/Reports | transactional Server use case |
| Inventory stock/provenance | Inventory | Buffet/Reports | transactional Server use case |
| Audit trail | platform Audit boundary | Reports/Admin | append-only Server write |
| Outbox messages | platform Outbox | side-effect dispatchers | transactionally committed Server write |
| Configuration | Settings | Server/Desktop | validated Server write |

## Integration rule

Modules never reach into another module's persistence tables to mutate another module's data.

Cross-module behavior uses an explicit application contract, domain event, or Server orchestration boundary.

Desktop and Agent never become alternate sources of truth.

## Anti-corruption rule

A module may translate an external/shared contract into its own domain model, but it must not leak persistence models or transport models into another module.
