# Foundation Completion Checklist

Repository-enforced:
- source-size guard
- architecture dependency guard
- Shared V1 contracts
- Money and operation IDs
- business clock boundary
- configuration validation
- correlation and exception pipeline
- auth and permission foundation
- PostgreSQL provider and DbContext
- audit and idempotency persistence boundary
- serializable transaction coordinator
- Agent command, acknowledgement, heartbeat and reconciliation contracts
- background-job boundary
- recovery, security, observability and compatibility policy
- dotnet-ef tool manifest
- one CI workflow

External certification:
- connect repo 3 self-hosted Windows/X64 runner
- generate and commit package-lock.json
- activate GitHub main/develop protection
- configure a real PostgreSQL test database
- generate and review the first migration against a clean database
- run the exact foundation merge commit green in CI

No business code is permitted until this gate is satisfied.
