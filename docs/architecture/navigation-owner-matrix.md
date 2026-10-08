# Canonical Navigation Owner Matrix

This matrix is the single product-information-architecture authority for the native Desktop shell.

## Canonical owners

| Owner | Current visible implementation items | Product authority |
|---|---|---|
| Dashboard | Home | Operator landing and live operating overview |
| Customers | Customers | Customer master/detail and identity |
| Buffet & Inventory | Buffet, Inventory | Product, stock, supplier and sale workflow |
| Tariffs & VIP | Tariffs, Vip | Pricing, plans, VIP rules and pricing snapshots |
| Games | Games, GameAccounts | Games catalogue and game-account management |
| Clients/Stations | Stations, ClientManagement, Agents | Station identity, client control and Agent health |
| Accounts | Wallet, Billing | Customer wallet, debt, payments and account ledger |
| Reports | Reports | Read-only reporting and analysis |
| Users & Shifts | Users, Approvals | Staff, roles, shifts and controlled approvals |
| Cash & Finance | Billing | Cash register, expenses, reconciliation and finance |
| Reservations & Queue | Sessions | Reservation and queue ownership until a dedicated surface exists |
| Settings | Settings | System configuration |
| Health/Support | Backup, Audit, Agents | Diagnostics, backup/recovery and support evidence |

## Rules

1. A visible navigation item belongs to exactly one canonical owner.
2. Implementation folders are not product authorities.
3. Accounts and Cash & Finance are separate authorities even when they share UI infrastructure.
4. Agents may appear under Clients/Stations and Health/Support as context surfaces, but Agent runtime authority remains server-side.
5. Buffet and Inventory share one operational owner and must never create competing stock ledgers.
6. Reservations/Queue must not become a second session/accounting authority.
7. Adding a new top-level navigation owner requires an architecture/product decision before code is added.

The current 16-item Desktop catalog is therefore treated as implementation-level navigation slots mapped to these canonical owners, not as 16 independent business authorities.
