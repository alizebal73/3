# Requirements Traceability Matrix

Every product requirement receives a stable ID, such as REQ-STN-001.

A requirement is incomplete until the chain below is populated.

| Field | Required |
|---|---|
| Requirement ID | yes |
| User/role | yes |
| Business problem/outcome | yes |
| Functional behavior | yes |
| Non-functional constraints | when applicable |
| Acceptance criteria | yes |
| Domain/module owner | yes |
| Data owner | yes |
| State-machine impact | when stateful |
| API/Agent/Desktop contract | when applicable |
| Permission | yes |
| Audit event | when sensitive or financial |
| Transaction boundary | when mutating |
| Idempotency scope | when retryable |
| Concurrency rule | when concurrent |
| Failure behavior | yes |
| Recovery/reconciliation | when recoverable |
| Observability | yes |
| Unit tests | yes |
| Integration tests | when integration exists |
| Contract tests | when a contract exists |
| E2E tests | for operator-critical flows |
| Performance target | when time/volume sensitive |
| Release/migration impact | yes |
| Documentation owner | yes |
| Evidence link | yes |

## Requirement classes

- FUNC — functional behavior
- SEC — security/privacy
- DATA — data integrity/retention
- PERF — performance/capacity
- REL — reliability/recovery
- UX — operator/user experience
- OPS — installation/operations/support
- COMP — compatibility/versioning
- ARCH — architecture constraints

A feature cannot enter implementation only because a ticket exists. It enters implementation when its traceability row is complete enough to satisfy the Definition of Ready.
