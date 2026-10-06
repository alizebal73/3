# Testing Policy

Unit tests cover pure domain rules.
Integration tests cover HTTP, database, SignalR, and concurrency boundaries.
Contract tests protect Shared API shapes.
E2E tests cover real operator workflows.

Every fix must reproduce the bug before the fix and stay green afterward.
Test happy path, invalid input, authorization, identity mismatch, concurrency, retry/idempotency, and rollback where applicable.
