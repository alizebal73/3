# PostgreSQL Foundation

Production persistence is PostgreSQL.

The Server owns the DbContext and persistence boundary. Modules do not access PostgreSQL directly from Domain, Application or Api code.

## Foundation schema

The committed Foundation migrations are:
- `202610070001_FoundationInfrastructure`: audit_entries, idempotency_records, outbox_messages and EF migration history;
- `202610070002_FoundationClosureHardening`: database-level audit immutability trigger and persistent Agent connection leases.

Business tables start only with the first approved vertical slice.

Business tables start only with the first approved vertical slice.

PostgreSQL identifiers use the EF/Npgsql snake_case convention so raw infrastructure SQL and migrations address the same schema.

## Rules

- migrations are versioned and committed;
- production schema changes are reviewed before release;
- integration tests use real PostgreSQL;
- unique constraints and foreign keys are part of business invariants;
- transaction isolation is chosen per use case;
- restore is tested on a clean database.

The local certification command is scripts/certify-postgresql.ps1. It requires an explicit GAMENET_DATABASE connection string and must run against a disposable clean certification database.
