# Pre-Coding Roadmap

This roadmap is the gate before the first business vertical slice.

## Stage P0 — Product definition
- charter
- actors
- scope/non-goals
- capability map
- MVP boundary
- requirements traceability
- backlog/change control

Exit: no major product ambiguity remains that would change architecture.

## Stage P1 — Domain and architecture
- module ownership
- data ownership
- state machines
- integration map
- trust boundaries
- ADRs
- architecture guards

Exit: every future business concept has a home and a source of truth.

## Stage P2 — Contracts and persistence
- Shared V1
- API envelope/errors
- Agent protocol
- schema/migration plan
- transaction/idempotency/outbox
- audit
- real PostgreSQL certification strategy

Exit: a vertical slice can be implemented without inventing data or contract rules.

## Stage P3 — Security and quality
- threat model
- permission matrix
- data classification
- dependency/SBOM policy
- test strategy
- Definition of Ready/Done
- regression policy

Exit: security and quality cannot be postponed to the end.

## Stage P4 — Operations and scale
- environment matrix
- health/readiness
- observability
- scale envelope
- SLO candidates
- backup/recovery
- update/rollback
- installation topology

Exit: the system can be operated and recovered, not just compiled.

## Stage P5 — Platform proof
- Server health smoke
- Desktop native launch + Server API smoke
- Agent transport/lease/reconciliation proof
- PostgreSQL concurrency certification
- update/rollback smoke
- restore smoke
- local certification record

Exit: Foundation is a working platform, not a folder skeleton.

## Stage P6 — First vertical slice
Only after P0–P5.

The slice must satisfy the feature template, Definition of Ready, Definition of Done and end-to-end recovery rules.

## Change control

Adding scope does not reset the roadmap automatically. Changes are classified:
- additive and local;
- contract/data-impacting;
- architecture-impacting;
- security-impacting;
- release-impacting.

Architecture/data/security/release-impacting changes require an ADR and traceability update.
