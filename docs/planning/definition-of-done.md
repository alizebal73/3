# Definition of Done

A backlog item is Done only when implementation and evidence agree.

## Required
- Acceptance criteria pass.
- Domain invariants are enforced by code.
- Correct owning module is used.
- No forbidden dependency was introduced.
- Shared contracts remain authoritative.
- Authorization is server-side.
- Audit is present where required.
- Money/time calculations follow foundation rules.
- Retry/idempotency/concurrency rules are tested where applicable.
- Recovery behavior is verified where applicable.
- Unit tests pass.
- Integration/contract/E2E tests pass where applicable.
- No unresolved build warnings were introduced without an approved reason.
- Architecture guards pass.
- Localization keys/resources are complete when UI changed.
- Operator-visible failure states are implemented.
- Documentation/traceability is updated.
- Migration/release notes are updated when schema/contract changes.
- Observability is sufficient to diagnose failure.
- Change is included in the appropriate release evidence.

“Works on my machine” is not a Definition of Done.
