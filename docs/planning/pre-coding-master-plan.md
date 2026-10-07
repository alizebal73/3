# GameNet 3 — Pre-Coding Master Plan

Status: Mandatory gate before the first business vertical slice.

This document answers the question: “What must be known, decided, documented, testable and operationally reproducible before we start building business features?”

## 1. Non-negotiable rule

No business implementation is started until every item in this document is either:
- implemented and locally verified;
- explicitly documented as a deliberate future boundary with an owner and acceptance condition; or
- rejected by an architecture decision with a recorded reason.

A checkbox without evidence is not complete.

## 2. The chain we require for every feature

Every future capability must be traceable through this chain:

Problem → Requirement → User/Operator outcome → Acceptance criteria → Domain rule → Data ownership → Use case → Transaction boundary → Contract → Permission → Audit event → Failure/retry behavior → Recovery/reconciliation → Test evidence → Observability → Release/rollback impact.

A feature that cannot complete this chain is not ready to code.

## 3. Foundation areas

### A. Product and scope
Required before feature work:
- Product purpose and non-goals.
- User roles and operational personas.
- Functional capability map.
- MVP / first usable release boundary.
- Explicit out-of-scope list.
- Roadmap and dependency order.
- Acceptance criteria for every roadmap item.
- Change-control rule for new requirements.
- Requirements traceability matrix.

### B. Domain
Required:
- Module ownership.
- Aggregate/entity ownership.
- State machines.
- Invariants.
- Commands/queries.
- Transaction owner.
- Concurrency rule.
- Idempotency scope.
- Audit semantics.
- Recovery/reconciliation behavior.
- Historical-data rules.
- Money/time rules.
- Cross-module interaction contracts.

### C. Architecture
Required:
- Runtime topology.
- Project/layer dependency rules.
- Data-flow map.
- Data ownership map.
- Trust boundaries.
- Integration contracts.
- Failure boundaries.
- Side-effect boundaries.
- Architecture decision records for irreversible or expensive choices.
- Automated architecture fitness checks.

### D. Contracts
Required:
- Versioned Shared contracts.
- API envelope and error model.
- Correlation/operation/idempotency headers.
- Agent protocol.
- Agent/Server compatibility policy.
- Schema compatibility policy.
- Release manifest.
- Breaking-change policy.
- Contract tests.
- No duplicate DTOs in Desktop or Agent.

### E. Persistence
Required:
- PostgreSQL as authoritative store.
- Explicit schema ownership.
- Migration strategy.
- Clean-database migration test.
- Forward-only upgrade policy.
- Data backfill strategy.
- Rollback strategy that does not depend on destructive schema rollback.
- Constraints/indexes specified before dependent features.
- Transaction and isolation policy.
- Backup/restore policy.

### F. Reliability
Required:
- Transaction coordinator.
- Idempotency with ownership fencing.
- Outbox for committed side effects.
- Agent connection lease/fencing.
- Reconciliation protocol.
- Retry policy and retry-safe operations.
- Timeout policy.
- Duplicate command handling.
- Recovery runbooks.
- Failure injection/concurrency tests.

### G. Security
Required:
- Threat model.
- Trust boundaries.
- Actor identity model.
- Authentication model.
- Authorization/permission matrix.
- Secret storage policy.
- Credential rotation/revocation.
- Rate limiting where relevant.
- Replay protection.
- Audit requirements.
- Dependency/SBOM policy.
- Secure build and release policy.
- Security verification gates.

Security is part of requirements, design, implementation and verification, not a separate final phase.

### H. Desktop/UX
Required before building screens:
- Information architecture.
- Navigation model.
- Global command/keyboard model.
- Permission-aware UI rules.
- Loading/empty/error/offline states.
- Dialog and confirmation policy.
- RTL/LTR behavior.
- Localization key policy.
- Accessibility baseline.
- Consistent formatting rules for money, dates, durations and status.
- Operator workflow for every high-frequency task.
- Screen-level acceptance criteria.

### I. Agent/Server
Required before real client management:
- Device identity lifecycle.
- Pairing/revocation.
- ConnectionId lifecycle.
- Connection lease and fencing.
- Heartbeat.
- State snapshot.
- Command envelope.
- Command expiry.
- Acknowledgement.
- Reconciliation.
- Local execution safety boundaries.
- Server-authoritative ownership.
- Recovery after Agent restart/network loss.

### J. Observability and supportability
Required:
- Live/readiness health model.
- Structured logs.
- Correlation/operation identifiers.
- Metrics for system health and user-visible outcomes.
- Diagnostic bundle/log retention strategy.
- Version/build information endpoint.
- Failure classification.
- Alert thresholds.
- Post-incident learning process.

### K. Scale and performance
Required:
- Capacity envelope.
- Supported station/Agent tiers.
- Concurrent operator/device/session assumptions.
- Database growth assumptions.
- Peak/burst behavior.
- Latency targets for critical operations.
- Concurrency targets.
- Load-test scenarios.
- Performance regression gate.
- Resource ceilings and graceful degradation behavior.

### L. Delivery and update
Required:
- Reproducible local build.
- Pinned SDK/tooling.
- Dependency policy.
- Release manifest.
- Checksums.
- Compatibility matrix.
- Installer component boundaries.
- Arbitrary installation path support.
- Local update folder transport.
- Side-by-side staging.
- Migration ordering.
- Health check after update.
- Rollback path.
- Release evidence.

### M. Backup and recovery
Required:
- Backup schedule/policy.
- Backup verification.
- Restore procedure.
- Clean-database restore test.
- Recovery Point Objective (RPO).
- Recovery Time Objective (RTO).
- Recovery ownership.
- Data-loss scenarios.
- Recovery audit record.

### N. Engineering governance
Required:
- Branch policy.
- ADR policy.
- Definition of Ready.
- Definition of Done.
- Regression-test policy.
- Source-size/complexity thresholds.
- Code review rules.
- Dependency update policy.
- Security review triggers.
- Feature-size limits.
- Small-batch rule.
- WIP limit.
- Changelog/release-note policy.

## 4. Foundation implementation sequence

### F0 — Toolchain and repository
Exit evidence:
- pinned SDK/tooling;
- reproducible restore/build;
- local canonical gate;
- clean solution structure;
- one intentional self-hosted workflow only.

### F1 — Product definition and traceability
Exit evidence:
- approved product charter;
- capability map;
- requirements matrix;
- MVP boundary;
- initial roadmap;
- change-control rules.

### F2 — Architecture and domain boundaries
Exit evidence:
- runtime topology;
- module ownership;
- data ownership;
- state machines;
- dependency rules;
- ADR set;
- automated architecture guards.

### F3 — Contracts and compatibility
Exit evidence:
- V1 contracts;
- versioning rules;
- API error/envelope rules;
- Agent protocol;
- release compatibility matrix;
- contract tests.

### F4 — Persistence and transaction foundation
Exit evidence:
- clean PostgreSQL migration;
- constraints/indexes;
- transaction coordinator;
- idempotency;
- append-only audit;
- Outbox;
- real PostgreSQL certification.

### F5 — Security foundation
Exit evidence:
- threat model;
- actor/authorization matrix;
- secret policy;
- dependency/SBOM policy;
- security verification rules.

### F6 — Platform observability
Exit evidence:
- live/readiness;
- correlation;
- structured diagnostics;
- version/build reporting;
- baseline metrics/log policy.

### F7 — Agent platform proof
Exit evidence:
- real authenticated Agent connection;
- lease/fencing;
- heartbeat;
- command acknowledgement;
- reconciliation;
- restart/network-loss tests.

### F8 — Desktop platform proof
Exit evidence:
- native launch;
- API client boundary;
- global navigation/command foundation;
- error/loading/offline states;
- localization and direction switching;
- Desktop-to-Server smoke test.

### F9 — Deployment/update platform
Exit evidence:
- install paths;
- Server/Agent services;
- Desktop shortcut/install;
- manifest/checksum validation;
- side-by-side update staging;
- migration compatibility;
- rollback smoke test.

### F10 — Backup/recovery proof
Exit evidence:
- verified backup;
- clean restore;
- restart/reconnect scenarios;
- documented RPO/RTO;
- operator recovery status.

### F11 — Quality and scale proof
Exit evidence:
- unit/integration/contract/E2E layers;
- concurrency harness;
- performance envelope;
- load tests against agreed targets;
- regression gate.

### F12 — First vertical slice
Only after F0–F11 are certified.

## 5. Change-control rule

A change that affects any of the following requires an ADR or explicit update to the governing artifact:
- module ownership;
- database ownership;
- contract compatibility;
- transaction boundary;
- security boundary;
- authentication/authorization;
- Agent protocol;
- installation/update semantics;
- data retention;
- release compatibility;
- scale envelope;
- operator navigation;
- state-machine transitions.

ADRs record the decision, its context and important consequences; superseding decisions should link to the old decision rather than silently rewriting history.

## 6. What “Foundation Certified” means

Foundation is certified only when:
1. Repository guards pass.
2. Local build/test gate passes.
3. Native Desktop launch passes.
4. PostgreSQL migration and concurrency certification pass.
5. Security/design gates have evidence.
6. Agent platform proof passes.
7. Desktop platform proof passes.
8. Install/update/rollback smoke passes.
9. Backup/restore smoke passes.
10. Scale/capacity evidence exists.
11. Recovery scenarios have evidence.
12. Requirements traceability and roadmap are up to date.
13. There are no undocumented architecture exceptions.
14. There are no business features hidden inside Foundation folders.
15. The first vertical slice has a completed Definition of Ready.

Only then is the product allowed to move from Foundation to Business Implementation.
