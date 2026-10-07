# GameNet Manager 3 — Product Capability Map

این سند مرز محصول را قبل از Business Coding تثبیت می‌کند. اینجا implementation نداریم؛ فقط capability، مالک، وابستگی و شرط آمادگی را مشخص می‌کنیم.

| Capability | Owning module | Primary user | Authoritative state | Main dependencies | First-release role |
|---|---|---|---|---|---|
| Authentication and operator identity | Users/Auth | Owner/Admin/Operator | Server | Security/Permissions | Required |
| Permissions and approvals | Users/Auth/Approvals | Owner/Admin | Server | Audit | Required |
| Customer identity/profile/PIN | Customers | Operator/Customer | Server | Auth/Sessions | Required |
| Device pairing and Agent health | Agents | Admin/Operator | Server + Agent observation | SignalR/Lease | Required |
| Station management | Stations | Operator | Server | Agents/Tariffs | Required |
| PC/PS5/Foosball categories | Stations | Operator | Server | Tariffs | Required |
| Session start/pause/resume/transfer/end | Sessions | Operator | Server | Customers/Stations/Tariffs/Agents | Required |
| Tariff calculation | Tariffs | Admin/Owner/Operator | Server | Station/VIP/Business time | Required |
| VIP plans and entitlements | Vip | Admin/Owner | Server | Customers/Tariffs/Sessions | Required |
| Wallet/ledger/top-up/debit/debt | Wallet/Billing | Operator/Admin/Owner | Server ledger | Customers/Idempotency/Audit | Required |
| Payment/reversal/settlement | Billing | Operator/Admin/Owner | Server | Wallet/Sessions/Approvals | Required |
| Discounts/promotional free credit | Billing | Owner/Admin/Operator policy | Server | Pricing/Permissions/Audit | Required |
| Inventory stock/provenance | Inventory | Admin/Operator | Server | Audit/Billing | Required |
| Buffet products/sales/returns | Buffet | Operator | Server | Inventory/Billing | Required |
| Reports | Reports | Owner/Admin/Operator | Read-only projections/queries | All read models | Required |
| Audit | Platform Audit | Owner/Admin | Append-only Server store | Every sensitive module | Required |
| Settings | Settings | Owner/Admin | Server | Permissions/Audit | Required |
| Backup/restore | Backup | Owner/Admin | Server/storage | PostgreSQL/Release | Required |
| Local installation/update/rollback | Platform Release | Admin/Owner | Release artifacts | Installer/Manifest/Migrations | Required |
| Diagnostics/support | Platform Observability | Admin/Support | Server logs/evidence | Correlation/Health | Required |

## First usable product path

Authentication → choose Customer → choose Station → resolve Tariff/VIP → start Session → Agent/Station state → pause/resume/transfer → settle → payment/debt/wallet → audit → recovery.

Buffet/Inventory extends the same financial and audit path rather than inventing a separate accounting authority.

## Cross-module rule

No capability owns data merely because its screen displays it.
The owning module owns the invariant and mutation transaction. Other modules consume explicit application/contract boundaries.

## Product acceptance boundary

A capability is not considered implemented when its screen exists. It is complete only when its Server rule, persistence, contract, permission, audit behavior, failure/retry path, recovery, tests, observability and operator workflow are all complete.

## Scope change

Adding a capability or changing an owner requires an update to the requirements traceability matrix, module boundary manifest and roadmap. Architecture-impacting changes require an ADR.