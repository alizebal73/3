# Test Infrastructure

The first vertical slice must use:
- real PostgreSQL for database integration tests
- isolated test database or schema per run
- fake TimeProvider for deterministic timing
- deterministic Agent transport doubles
- deterministic realtime command/ack doubles
- concurrency harnesses for race tests
- test data builders for valid domain state

EF InMemory is not accepted as evidence for PostgreSQL concurrency, indexes, constraints or transaction semantics.

Integration tests may be skipped when a real test database is not configured, but the skip must be explicit and the database-backed lane is required before release certification.
