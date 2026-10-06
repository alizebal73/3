# Stage 0 Foundation Audit

## Implemented in repository

- Modular-monolith composition boundary.
- Explicit module folder boundaries.
- Architecture rules and CI architecture guard.
- Shared V1 system, Agent protocol and permission contracts.
- Money, CorrelationId, CommandId and IdempotencyKey primitives.
- PostgreSQL EF Core provider and DbContext boundary.
- Serializable transaction coordinator with bounded retry for PostgreSQL serialization/deadlock failures.
- Idempotency persistence boundary.
- Audit persistence boundary.
- JWT authentication validation and current-actor foundation.
- Correlation and exception middleware.
- Business-time clock abstraction.
- Durable transactional outbox writer boundary.
- Background-job queue and dispatcher boundary.
- Recovery, security, configuration, compatibility, domain and observability specifications.
- dotnet-ef local tool manifest.
- CI architecture/source-size/foundation-completeness gates and deterministic dashboard verification.
- Placeholder Foundation tests removed.
- Main remains free of business features.

## Not yet claimed complete

- No Customer, Station, Session, Billing, Inventory or other business mutation.
- No business database migration is claimed; migration will be generated and reviewed before the first vertical slice.
- No real user/token issuance flow; JWT validation is infrastructure only.
- No durable distributed job scheduler; current background queue is process-local.
- No SignalR hub or transport-specific Agent command dispatcher; only protocol contracts and durable outbox persistence exist.
- No real E2E business workflow.
- package-lock.json still needs generation on the approved Node environment.
- GitHub branch protection/rulesets remain an external repository setting.
- CI cannot be certified green until repo 3 has its self-hosted runner connected.

## Policy

No business feature is considered complete without Domain Rule, Use Case, Persistence, Contract, Authorization, Audit, Unit, Integration, Concurrency, Retry/Idempotency and Failure/Recovery coverage.
