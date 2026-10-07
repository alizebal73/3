# Stage 0 Foundation Audit

## Repository baseline

Implemented:
- modular-monolith composition boundary and module manifest;
- native Windows WPF Desktop and bilingual resource foundation;
- Server/Agent/Shared topology and Windows Service capability;
- explicit human/device/customer/session identity model;
- Shared V1 API envelope, headers and release compatibility rules;
- Money, CorrelationId, CommandId and IdempotencyKey primitives;
- PostgreSQL EF Core provider, snake_case naming and Foundation migration baseline;
- serializable transaction coordinator with bounded serialization/deadlock retry;
- idempotency persistence boundary with operation fencing and lease ownership;
- append-only audit persistence boundary enforced by the DbContext;
- JWT validation, permission policies and current-actor foundation;
- correlation and exception middleware;
- business-time clock abstraction;
- durable transactional outbox writer plus atomic PostgreSQL claim/lease dispatcher boundary;
- Agent connection lease/realtime contract boundary separating DeviceId from ConnectionId;
- state machines, module blueprints, permission matrix and billing/station rules;
- backup/restore boundary and deployment/update architecture;
- local certification scripts and isolated PostgreSQL certification suite;
- platform-skeleton, architecture, source-size and Foundation-completeness guards;
- pre-coding product/requirements/governance artifacts and pre-coding readiness guard;
- runtime topology, data ownership, scale/SLO, quality, security supply-chain, environment and release-gate documentation;
- no business implementation in Server Modules or Desktop Features.

## Still requiring local or platform-proof evidence

- Windows restore/build/test of the complete solution.
- Native Desktop launch and bilingual runtime verification.
- Real PostgreSQL migration/concurrency certification.
- Agent transport, connection lease persistence and reconciliation proof.
- Desktop-to-Server real API smoke.
- Recovery/restore smoke execution against disposable infrastructure.
- Install/update/rollback smoke execution.
- Final Foundation sign-off and merge to main.

## Policy

No business Feature branch starts until the repository governance gate and remaining Foundation/platform-proof evidence are proven locally.
