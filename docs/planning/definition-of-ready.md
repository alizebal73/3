# Definition of Ready

A backlog item is Ready only when:

## Product
- The user/operator problem is explicit.
- Scope and non-goals are explicit.
- Acceptance criteria are testable.
- Dependencies are identified.
- Out-of-scope behavior is stated.

## Domain
- The owning module is known.
- The source of truth is known.
- Invariants are known.
- State transitions are known if the item is stateful.
- Historical-data behavior is known.
- Transaction owner is known for mutations.

## Contracts
- Required Server/Desktop/Agent contracts are identified.
- Version impact is known.
- Duplicate DTOs are prohibited.
- Permission names are known.

## Reliability
- Retry behavior is defined.
- Idempotency scope is defined where retry is possible.
- Concurrency behavior is defined.
- Failure and recovery paths are defined.

## Security
- Actor and authorization requirements are defined.
- Sensitive data is classified.
- Audit requirements are defined.
- Threat impact is reviewed for high-risk changes.

## UX
- Navigation location is defined.
- Loading/empty/error/offline states are defined.
- Keyboard/RTL/LTR/localization implications are identified.

## Verification
- Unit tests are identified.
- Integration/contract/E2E tests are identified where applicable.
- Performance target is identified where applicable.
- Migration/update/rollback impact is identified.

No code is started to discover these fundamentals accidentally.
