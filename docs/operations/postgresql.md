# PostgreSQL Foundation

Production persistence is PostgreSQL.

The Server owns the DbContext and persistence boundary. Modules do not access PostgreSQL directly from Domain, Application or Api code.

Rules:
- migrations are versioned and committed
- production schema changes are reviewed before release
- integration tests use real PostgreSQL
- unique constraints and foreign keys are part of business invariants
- transaction isolation is chosen per use case
- restore is tested on a clean database

The current clean-slate foundation intentionally has only Audit and Idempotency infrastructure entities. Business tables begin with the first vertical slice.
