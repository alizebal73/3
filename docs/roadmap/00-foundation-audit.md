# Stage 0 Foundation Audit

## Implemented in repository

- Modular-monolith composition boundary.
- Explicit Server module folder boundaries.
- Native Windows WPF Desktop project and shell.
- Persian/English UI resources with RTL/LTR foundation.
- Shared V1 system, Agent protocol and permission contracts.
- Money, CorrelationId, CommandId and IdempotencyKey primitives.
- PostgreSQL EF Core provider and DbContext boundary.
- Serializable transaction coordinator with bounded PostgreSQL serialization/deadlock retry.
- Idempotency persistence boundary.
- Audit persistence boundary.
- JWT validation and current-actor foundation.
- Correlation and exception middleware.
- Business-time clock abstraction.
- Durable transactional outbox writer boundary.
- Background-job queue and dispatcher boundary.
- Transaction side-effect rules.
- Recovery, security, configuration, compatibility, domain and observability specifications.
- Platform-skeleton, architecture, source-size and Foundation-completeness guards.
- Local-only verification script; no remote CI execution dependency.
- Placeholder Foundation tests removed.
- No business implementation in Server Modules.

## Not yet certified

- Local Windows restore/build/test of the complete solution.
- Native Desktop executable launch verification.
- fa-IR and en-US runtime verification on the local Windows machine.
- Real PostgreSQL integration environment.
- First migration generated/reviewed/applied to a clean PostgreSQL database.
- Database-backed concurrency tests.
- Recovery/restore smoke tests.
- Durable Agent connection lease and transport implementation.
- Real E2E business workflow.

## Policy

No business feature is considered complete without Domain Rule, Use Case, Persistence, Contract, Authorization, Audit, Unit, Integration, Concurrency, Retry/Idempotency and Failure/Recovery coverage.

No business Feature branch may start until the remaining Foundation certification items are proven locally.
