# Foundation Completion Checklist

## Repository-enforced

- source-size guard
- architecture dependency guard
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
- background-job boundary
- recovery, security, observability and compatibility policy
- dotnet-ef tool manifest
- one CI workflow
- no placeholder tests

## External certification

- connect repo 3 self-hosted Windows/X64 runner
- generate and commit package-lock.json
- activate GitHub main/develop protection
- configure a real PostgreSQL test database
- generate and review the first migration against a clean database
- run the exact foundation merge commit green in CI

No business code is permitted until every repository-enforced item and every external certification gate is satisfied.
