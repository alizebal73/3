# Stage 0 Foundation Audit

## Implemented in repository
- Modular-monolith composition boundary and module manifest.
- Native Windows WPF Desktop and bilingual resource foundation.
- Server/Agent/Shared topology and Windows Service capability.
- Explicit human/device/customer/session identity model.
- Shared V1 API envelope, headers and release compatibility rules.
- Money, CorrelationId, CommandId and IdempotencyKey primitives.
- PostgreSQL EF Core provider, snake_case naming and Foundation migration baseline.
- Serializable transaction coordinator with bounded serialization/deadlock retry.
- Idempotency persistence boundary.
- Audit persistence boundary.
- JWT validation, permission policies and current-actor foundation.
- Correlation and exception middleware.
- Business-time clock abstraction.
- Durable transactional outbox writer plus atomic claim/lease dispatcher boundary.
- State machines, module blueprints, permission matrix and billing/station rules.
- Backup/restore boundary and deployment/update architecture.
- Local certification scripts and isolated PostgreSQL certification suite.
- Platform-skeleton, architecture, source-size and Foundation-completeness guards.
- No business implementation in Server Modules.

## Still requiring local evidence
- Windows restore/build/test of the complete solution.
- Native Desktop launch and bilingual runtime verification.
- Real PostgreSQL migration/concurrency certification.
- Recovery/restore smoke execution against disposable infrastructure.
- Final sign-off and merge to main.

## Policy
No business Feature branch starts until the remaining Foundation evidence is proven locally.
