# Quality Strategy

Quality is built continuously, not postponed to a final test phase.

## Verification layers

### Unit
Domain rules, value objects, pure calculations and policy decisions.

### Application
Use-case orchestration, authorization decisions, transaction boundaries and idempotency behavior.

### Integration
Real PostgreSQL constraints, indexes, transactions, migrations and persistence behavior.

### Contract
Shared API/Agent wire contracts and compatibility behavior.

### Desktop
Native process launch, localization, navigation, command routing and API boundary behavior.

### Agent
Service lifecycle, transport, command execution safety and reconciliation.

### E2E
Operator-critical flows across Desktop → Server → PostgreSQL and Server → Agent.

### Certification
Clean-database migrations, concurrency, recovery, update/rollback and restore.

## Regression rule

Every production defect must produce either:
- a regression test;
- a new invariant/guard; or
- an ADR explaining why a test/guard is not appropriate.

## Fake boundary rule

Mocks/fakes may isolate a unit, but they cannot be the only evidence for:
- PostgreSQL transaction semantics;
- uniqueness/index enforcement;
- concurrent lease/idempotency behavior;
- Windows service behavior;
- native Desktop process behavior;
- release/update/rollback behavior.

## Quality gate

A change is not accepted because tests are green alone. Architecture guards, requirements traceability, security impact, operational behavior and release impact must also be clean.
