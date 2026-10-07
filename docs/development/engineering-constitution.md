# GameNet 3 Engineering Constitution

This document is the non-negotiable engineering contract for the repository. It exists to prevent the failure modes learned from earlier iterations.

## 1. Authority
- Server is the sole business authority.
- PostgreSQL is the authoritative persistent store for Server-owned state.
- Desktop is a native WPF API client and never owns authoritative business calculations.
- Agent is a constrained executor/observer and never authorizes itself.
- Shared V1 contracts are the single wire-contract authority.

## 2. Change order
A material change follows:

Requirement → acceptance → ownership/invariants → contract/ADR → implementation → regression tests → build → runtime evidence → release evidence.

Business code must not be used to discover unresolved ownership, contract or security decisions.

## 3. Evidence states
`Designed`, `Implemented`, `Tested`, `Verified`, `Runtime Certified`, and `Released` are distinct states.

No document may call a feature or Foundation item certified merely because files exist or a static guard passes.

## 4. Build discipline
- The canonical verification path is `scripts/verify.ps1`.
- Release restore, build and tests are mandatory gates.
- Structural guards never substitute for compilation or execution.
- Every Foundation or feature branch change must be verified on the approved self-hosted Windows runner before merge.

## 5. Test discipline
Normal tests remain in `GameNet.slnx` and must be runnable without external infrastructure unless their project explicitly says otherwise.

Environment certification tests use `GameNetCertificationProject=true` and are invoked only by their dedicated certification scripts with the required real environment.

Every defect discovered in production code or verification infrastructure must leave a regression test, invariant, guard, ADR exception, or equivalent permanent evidence.

## 6. Contracts and schema
- No duplicate DTO/Request/Response authority is allowed.
- Schema changes require migration review and runtime migration evidence.
- Breaking contract changes require an explicit compatibility decision.
- A future feature may not silently alter a Foundation contract.

## 7. Failure first
For every stateful, financial, security, or distributed operation, define retry, duplicate, concurrency, timeout, disconnect, restart, recovery and reconciliation behavior before implementation.

## 8. Security
Authentication is not authorization.

Identity, credential provisioning, rotation, revocation, expiry and audit are Foundation concerns for Agent and user actors. DeviceId alone is never treated as a secret.

## 9. Runtime truth
Windows process/service behavior, PostgreSQL behavior, network reconnect/fencing, backup/restore, update/rollback and installer behavior require real-environment evidence. Fakes and structural checks may support but never replace that evidence.

## 10. Small-batch and rollback
A change must have one clear reason, remain reviewable, and have a known rollback path. Unrelated refactors must not be bundled into feature work.

## 11. Governance
- `main` is the only release line.
- Foundation branches are temporary.
- No new work starts on legacy `develop` or obsolete Foundation stage branches.
- Merge is prohibited while the required Foundation/feature gate is red.

## 12. Feature freeze rule
While Foundation Stabilization is open, no business vertical slice may be implemented. The first permitted slice is Station → Customer → Agent → Session after Foundation certification.
