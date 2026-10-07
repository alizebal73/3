# Database Change Policy

PostgreSQL schema evolution is part of the application release contract.

## Rules

1. Every schema change is represented by a committed migration.
2. No production schema is edited manually as a normal release mechanism.
3. Schema changes and data/backfill changes are reviewed separately when their failure modes differ.
4. Destructive changes require a compatibility plan and explicit approval.
5. A migration must be safe against the supported upgrade order.
6. New application code must not assume a new column exists before the deployment sequence makes that true.
7. Removing a column is a later step after dependent code no longer requires it.
8. Backfills must be restartable and observable.
9. Every schema change has rollback/forward-recovery reasoning.
10. Clean-database migration and upgrade-from-previous-version tests are required.
11. Large or long-running migrations must state locking, duration and operational risk.
12. Database version is part of release compatibility.

## EF Core rule

EF Core migrations are the schema history and source-controlled migration definition. The production deployment process controls when migrations execute; the Server must not silently perform an uncontrolled production migration merely because it started.

This separation prevents “application start” from becoming an implicit schema deployment.

## Evidence

For every release with schema impact:
- migration review;
- clean database test;
- upgrade test;
- data/backfill evidence;
- operational impact note;
- compatibility matrix update.
