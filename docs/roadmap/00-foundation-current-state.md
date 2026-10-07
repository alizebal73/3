# GameNet 3 — Current Foundation State & Source-of-Truth Map

**Date:** 2026-10-07  
**Repository:** `alizebal73/3`  
**Current canonical baseline before this cleanup:** `01e07fe6fe78ee7a2b078bba06edfea4252ca007`

## Purpose

This file is the cleanup and synchronization map for the Foundation phase. It prevents the repository from having several competing roadmaps, branch snapshots, or status documents that disagree about what is current.

It does **not** certify runtime readiness. Structural readiness and runtime certification remain separate.

## Canonical sources

| Concern | Canonical source |
|---|---|
| Master pre-coding roadmap | `docs/roadmap/00-pre-coding-roadmap.md` |
| Active Foundation execution | `docs/roadmap/01-foundation-stabilization.md` |
| Foundation closure truth | `docs/architecture/foundation-closure-matrix.md` |
| Product/capability map | `docs/planning/product-capability-map.md` |
| Requirements traceability | `docs/planning/requirements-traceability.md` |
| Scale/capacity envelope | `docs/operations/scale-capacity-slos.md` |
| Assumptions/open decisions | `docs/planning/assumptions-and-open-decisions.md` |
| Repo 2 lessons/audit | `docs/research/repo2-to-repo3-comprehensive-audit.md` |
| Local certification procedure | `docs/operations/foundation-local-certification.md` |
| Release gates | `docs/release/release-gates.md` |
| Branch policy | `docs/development/branching.md` |

## Roadmap cleanup rule

The older Foundation roadmap files are retained for historical traceability, but they are **not independent status authorities**:

- `docs/roadmap/00-foundation-audit.md`
- `docs/roadmap/00-foundation-completion.md`
- `docs/roadmap/00-platform-foundation.md`

If one of these differs from the canonical roadmap/current-state map, the canonical documents above win. A future cleanup may archive these historical summaries if their traceability is no longer useful.

## Current branch policy

The intended release line is `main`.

Foundation implementation has been consolidated into the same canonical commit. Temporary Foundation branches are no longer separate development states.

The historical backup branch `backup/pre-foundation-hardening-2026-10-07` is intentionally preserved as an archive/reference point and must not be used as the current working baseline.

A long-lived `develop` branch is not part of the intended workflow; if it exists for compatibility, it must point to the same canonical baseline and must not become a second development line.

## Current repository state

The canonical Foundation baseline contains:

- native WPF Desktop shell;
- authoritative Server/API and PostgreSQL boundary;
- Windows Agent with SignalR/identity/lease foundation;
- Shared V1 contracts;
- transaction/idempotency/audit/outbox/fencing infrastructure;
- migration and release tooling;
- update/rollback boundaries;
- architecture/source-size/platform/supply-chain guards;
- Foundation tests and certification scripts;
- product map, requirements traceability, scale envelope, security and recovery policies;
- business module boundaries intentionally reserved without business implementation.

## Certification truth

The current baseline is **structurally prepared** but is **not yet Runtime Certified**.

Required runtime evidence remains:

1. real PostgreSQL migration/concurrency/backup/restore;
2. real Agent authentication, durable DeviceId, lease, heartbeat, reconnect and fencing;
3. real WPF Desktop launch/localization and Desktop↔Server smoke;
4. clean install/update/rollback;
5. recovery/RPO/RTO evidence;
6. SBOM/signature/migration release evidence;
7. final exact-SHA evidence pack and closure-matrix review.

A green GitHub workflow or the existence of a certification script never closes these rows by itself.

## No business start rule

No Customer, Station, Session, Billing, Wallet, Inventory, Buffet, Tariff, VIP or other business implementation may start until the Foundation closure matrix is fully supported by the required real-environment evidence.

## Synchronization rule

Any future architecture, security, schema, contract, deployment, recovery or scale change must update the affected canonical document in the same change and must not silently create a second source of truth.
