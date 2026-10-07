# Foundation Closure Contract

This is the atomic closure register for GameNet 3 Foundation.

The existing `foundation-closure-matrix.md` remains the summary authority. This document prevents hidden obligations from living only in prose, tests, or chat. Every Foundation obligation that can cause a later product bug must have an explicit row here.

## Status rules

- **Closed — Static**: implementation and automated structural evidence are present; real-environment proof may still be pending.
- **Pending — Runtime**: implementation exists, but approved Windows/PostgreSQL evidence is still required.
- **Open — Implementation**: a required implementation, guard, test, or contract is still missing.
- **Open — Policy**: a deliberate operational/security/product decision is still required.
- **Business — Deferred**: intentionally belongs to a future vertical slice and is not a Foundation defect.

No item is called Foundation Certified until all required Static/Runtime/Policy rows are closed.

| ID | Obligation | Required closure evidence | Current status |
|---|---|---|---|
| FND-001 | Canonical verification gate is self-consistent | `scripts\verify.ps1` passes and every guard agrees with the current architecture | Closed — Static / Runtime gate #132 passed |
| FND-002 | Layer/dependency boundaries | Architecture guard + no direct Desktop/Agent DB or Server-implementation references | Closed — Static |
| FND-003 | Clean PostgreSQL migration | Disposable clean DB migrates from empty state with no pending migrations | Pending — Runtime |
| FND-004 | Upgrade migration path | Upgrade-from-previous-version test plus reviewed migration SQL/bundle | Open — Implementation |
| FND-005 | Serializable transaction coordinator | Real PostgreSQL concurrency/serialization retry evidence | Pending — Runtime |
| FND-006 | Idempotency ownership/fencing | Concurrent claim, stale-owner completion rejection, replay evidence | Pending — Runtime |
| FND-007 | Append-only audit | EF mutation guard plus PostgreSQL UPDATE/DELETE rejection evidence | Pending — Runtime |
| FND-008 | Outbox claim fencing | Concurrent claim and expired-owner publish rejection evidence | Pending — Runtime |
| FND-009 | Outbox delivery boundary | Explicit publisher/dispatcher ownership before first durable external side effect | Open — Policy/Implementation |
| FND-010 | Agent connection lease | One authority per DeviceId under concurrent connections | Pending — Runtime |
| FND-011 | Agent stale-owner fencing | Stale heartbeat/release/reconnect cannot affect a newer lease | Pending — Runtime |
| FND-012 | Agent reconciliation authority | Reconciliation re-checks live DB lease ownership and expiry | Closed — Static; Runtime pending |
| FND-013 | Durable DeviceId | Restart/relaunch reuses stored identity and never accepts mutable runtime override | Closed — Static; Runtime pending |
| FND-014 | Credential lifecycle | Real HTTP provision → token → rotation → revocation → expiry evidence | Pending — Runtime |
| FND-015 | Agent abuse/rate limiting | Token/provision/rotate/revoke endpoints have bounded abuse controls and tests | Open — Implementation |
| FND-016 | Production TLS posture | HTTPS endpoint/certificate/trust path verified on approved Windows environment | Open — Runtime/Policy |
| FND-017 | Health/readiness authority | live/health/ready behavior verified with healthy, starting and dependency-failure states | Pending — Runtime |
| FND-018 | Correlation/error contract | Correlation + stable API failure envelope + operation identity are end-to-end testable | Open — Implementation |
| FND-019 | Build/version diagnostics | BuildInfo/compatible version information is exposed by the Server and consumed by diagnostics | Open — Implementation |
| FND-020 | Operational diagnostics | Support bundle/status surface can collect non-sensitive diagnostic evidence | Open — Implementation |
| FND-021 | Native WPF shell | Native process launch and real Desktop↔Server smoke on approved Windows | Pending — Runtime |
| FND-022 | UI shell completeness | Global navigation, command model, keyboard/F1, notification/status and core state model are closed before business screens | Open — Implementation |
| FND-023 | UI resilience/accessibility | Loading/empty/error/offline/stale/permission-denied + RTL/LTR + keyboard/accessibility evidence | Open — Implementation/Runtime |
| FND-024 | Agent command safety contract | CommandId, expiry, acknowledgement, duplicate handling and authoritative Device/lease checks are implemented before real ClientControl | Open — Implementation |
| FND-025 | Agent command runtime proof | Real launch/stop/lock/restart/maintenance/diagnostic command acknowledgement and reconnect safety | Business — Deferred to ClientControl slice |
| FND-026 | Desktop↔Server runtime smoke | Real Server health + reconnect/offline recovery through native Desktop | Pending — Runtime |
| FND-027 | Agent service installation | Clean install under intended Windows service identity with DPAPI credential persistence | Pending — Runtime |
| FND-028 | Server service installation | Clean install, startup, restart and recovery on target Windows machine | Pending — Runtime |
| FND-029 | Arbitrary installation paths | Installer/update scripts succeed outside the system drive on approved test paths | Pending — Runtime |
| FND-030 | Update manifest/checksum | Manifest validation, compatibility and safe-path verification tests | Closed — Static |
| FND-031 | Production update signing | Signature verification is mandatory for supported production update paths | Open — Implementation |
| FND-032 | Side-by-side update/rollback | Clean update, health verification, failed update rollback and service recovery | Pending — Runtime |
| FND-033 | SBOM and dependency evidence | SPDX SBOM, license/vulnerability review and exact source revision recorded | Pending — Release proof |
| FND-034 | Backup creation/verification | Real pg_dump/archive verification on disposable PostgreSQL | Pending — Runtime |
| FND-035 | Backup restore | Isolated restore plus data/schema probe on disposable target | Pending — Runtime |
| FND-036 | Recovery policy evidence | RPO/RTO scenario evidence, restart/reconnect/restore/update recovery record | Pending — Runtime |
| FND-037 | Scale/load harness | Executable Tier S/M scenarios with measured thresholds and regression comparison | Open — Implementation |
| FND-038 | Observability metrics | Baseline counters/histograms/health signals for critical platform operations | Open — Implementation |
| FND-039 | Agent/server restart recovery | Server restart + Agent restart/reconnect + lease recovery evidence | Pending — Runtime |
| FND-040 | Exact-SHA evidence pack | Evidence records exact commit, machine, tool versions, binaries and scenario results | Pending — Runtime |
| FND-041 | Governance protection | Main release line protected against unsafe direct/force changes and legacy branch drift | Open — Operational policy |
| FND-042 | Foundation closure guard itself | Guard validates every required closure ID remains present and the summary matrix links to this contract | Closed — Static |

## Policy-open items intentionally allowed

- Production backup encryption/storage policy.
- Exact Windows service identity and least-privilege posture for Server/Agent.
- Approved TLS certificate provisioning/trust procedure.
- Production credential provisioning operator/authority model.

## Business-deferred boundary

The following do not belong in Foundation closure, but they must reach Definition of Ready before implementation:

- human authentication/roles/permissions workflow;
- Customer/Station/Session/Billing first vertical slice;
- Tariffs/VIP;
- Buffet/Inventory business ledger;
- Reports;
- Users/Shifts/Cash;
- Reservations/Queue;
- Games/GameAccounts/ClientControl command execution;
- external payment provider;
- multi-shop/multi-tenant;
- cloud update transport.

## Closure rule

A green build is never sufficient to close a row.

For every required row the chain must be:

**Contract → Implementation → Guard/Test → Real Evidence → Traceable SHA → Closure review**

Any change to a closed Foundation contract reopens the affected row and invalidates the previous certification SHA.