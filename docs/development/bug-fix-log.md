# GameNet 3 — Engineering Bug-Fix Log

## Rules

1. Every regression is recorded here before or with its fix.
2. A fix is not considered complete until the relevant local verification gate is green.
3. Do not stack unrelated changes on a failing checkpoint.
4. Stable checkpoints are immutable references; new work starts from the latest certified checkpoint.
5. The pre-reset main history is preserved in `backup/main-before-clean-reset-2026-10-08`.

## 2026-10-08 — Clean Foundation Reset

- Problem: `main` had accumulated planning/documentation changes after the stable Foundation checkpoint without a certified product implementation checkpoint.
- Action: reset `main` to `01e07fe6fe78ee7a2b078bba06edfea4252ca007`.
- Backup: `backup/main-before-clean-reset-2026-10-08`.
- Stable branch: `stable/clean-foundation-2026-10-08`.
- Result: clean Foundation is now the only base for product implementation.

## 2026-10-08 — First Product Vertical Slice

- Feature: Stations.
- Scope: Domain aggregate, application service, EF persistence, PostgreSQL migration, API contract, REST endpoints, Desktop client, WPF list and domain tests.
- Rule: no direct PostgreSQL access from Desktop and no business rules in UI/API transport.
- Verification: PR `#5`, self-hosted Foundation verification required before merge.

## Next vertical slices

1. Customers + identity/profile
2. Sessions + station ownership/transfer
3. Tariffs + billing/settlement
4. Wallet ledger + payments/debt
5. Users/roles/permissions + approvals
6. Inventory + buffet
7. VIP
8. Agent control + heartbeat/recovery integration
9. Reports/audit
10. Backup/update/recovery and release certification
