# Foundation Completion Checklist

## Pre-coding governance
- product charter and explicit non-goals
- requirement traceability matrix
- Definition of Ready / Done
- risk register
- assumptions/open-decisions register
- data ownership and integration map
- scale/capacity/SLO envelope
- environment matrix
- database change policy
- support diagnostics and runbook index
- release gates
- security/data classification/supply-chain policy
- operator workflow and product-validation foundation
- feature and vertical-slice templates
- ADR template and change-control rules
- pre-coding governance guard

## Repository-enforced Foundation
- complete native WPF Desktop project and shell
- fa-IR RTL and en-US LTR localization resources
- complete Server module skeleton for all planned business boundaries
- Agent/Shared/Test skeleton
- explicit actor/auth identity model
- stable Shared V1 API envelope and runtime headers
- API compatibility/versioning contract
- Money and operation/idempotency primitives
- business clock boundary
- configuration validation
- correlation and exception pipeline
- authentication validation and permission-policy foundation
- PostgreSQL provider, snake_case convention and committed foundation migration
- audit and idempotency persistence boundaries
- durable transactional outbox with atomic lease/claim boundary
- serializable transaction coordinator with bounded PostgreSQL retry
- Agent command/ack/heartbeat/reconciliation contracts
- DeviceId/ConnectionId separation with connection lease/fencing contract
- transaction side-effect rules
- state machines and module blueprints
- initial permission matrix
- backup/restore boundary and recovery rules
- Windows Service topology for Server and Agent
- native Desktop deployment/update architecture
- product/schema/API/Agent compatibility primitive
- source-size, architecture, platform-skeleton and Foundation-completeness guards
- pre-coding governance guard
- no placeholder tests
- no browser artifacts

## Local certification
- scripts/verify.ps1 on approved Windows
- scripts/certify-desktop.ps1 launch verification
- scripts/certify-postgresql.ps1 against disposable PostgreSQL
- real PostgreSQL concurrency tests
- migration review on clean database
- recovery/restore smoke test
- update/rollback smoke test
- Agent connection/lease/reconciliation proof
- Desktop-to-Server smoke proof
- final Foundation evidence record

No business feature is permitted until every repository-enforced item and every local certification item is satisfied.
