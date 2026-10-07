# Test Infrastructure

All evidence is generated locally on the approved Windows machine.

The first vertical slice must use:
- real PostgreSQL for database integration tests;
- isolated test database or schema per run;
- fake TimeProvider for deterministic timing;
- deterministic Agent transport doubles;
- deterministic realtime command/ack doubles;
- concurrency harnesses for race tests;
- test data builders for valid domain state;
- native Desktop tests for localization and platform boundaries.

EF InMemory is not accepted as evidence for PostgreSQL concurrency, indexes, constraints or transaction semantics.

A test skipped because the local environment is not configured is not certification. Database-backed and concurrency lanes must run successfully before Foundation certification.

GitHub checkmarks are never substituted for local test evidence.
