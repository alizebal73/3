# Stage 0 — Platform Foundation

No business feature is started until the foundation gates below are satisfied.

Required foundation:
1. Product/domain specification and module ownership.
2. Invariants and explicit state machines.
3. Shared IDs, Money, Time and Result/Error primitives.
4. Versioned API contracts.
5. PostgreSQL provider and DbContext boundary.
6. Transaction, concurrency and idempotency infrastructure.
7. Authentication and authorization foundation.
8. Audit infrastructure.
9. Agent realtime command, acknowledgement and state protocol.
10. Recovery and reconciliation rules.
11. Background-job boundary.
12. Observability and correlation IDs.
13. Configuration and secret provisioning rules.
14. Test infrastructure.
15. Architecture enforcement.
16. CI and repository protection.
17. Release compatibility and rollback foundation.

Exit gates:
- dedicated self-hosted Windows/X64 runner for repo 3
- exact-commit CI green
- active main/develop protection
- committed package-lock.json and npm ci in CI
- architecture guard green
- PostgreSQL-backed integration lane executable
- migration strategy tested on a clean database
- no fake green E2E

Only then start:
Station -> Customer -> Agent -> Customer Login -> Session -> Timing -> Transfer -> Invoice -> Payment -> Audit.
