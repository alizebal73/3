# Foundation Completion Checklist

## Repository-enforced

- complete native Desktop project skeleton
- complete Server module skeleton
- complete Agent/Shared/Test skeleton
- desktop localization resources for fa-IR and en-US
- source-size guard
- architecture dependency guard
- platform-skeleton guard
- Foundation completeness guard
- Shared V1 contracts
- Money and operation IDs
- business clock boundary
- configuration validation
- correlation and exception pipeline
- authentication validation and current actor boundary
- permission-policy foundation
- PostgreSQL provider and DbContext
- audit and idempotency persistence boundaries
- durable transactional outbox writer boundary
- serializable transaction coordinator with transient retry
- Agent command, acknowledgement, heartbeat and reconciliation contracts
- transaction side-effect rules
- background-job boundary
- recovery, security, configuration, compatibility, domain and observability policy
- no placeholder tests
- no browser Dashboard project
- no GitHub Actions execution dependency

## Local certification

- run scripts/verify.ps1 on the approved Windows machine
- restore/build/test the entire solution locally
- produce the native Desktop executable
- verify fa-IR and en-US resource loading
- configure a real PostgreSQL test database
- generate and review the first migration against a clean database
- run database-backed integration and concurrency tests
- perform recovery/restore smoke tests

No business feature is permitted until every repository-enforced item and every local certification item is satisfied.
