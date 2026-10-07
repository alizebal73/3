# GameNet Manager 3

Clean-slate Windows GameNet/CyberCafe management platform.

## Current status

Repo 3 is in **Foundation Stabilization / Runtime Certification preparation**.

The architecture and engineering foundation are implemented and structurally verified, but the product is **not yet Runtime Certified**. Real PostgreSQL, Agent, Desktop, deployment/recovery and release evidence must still close the Foundation matrix.

**No business feature implementation starts before Foundation certification.**

## Runtime surfaces

1. GameNet.Server — local authoritative headless server/API and persistence boundary.
2. GameNet.Manager.Desktop — native Windows WPF Operator/Admin application. This is the human-facing program; it is not a browser application.
3. GameNet.Agent — Windows machine-side Agent installed on client PCs.
4. GameNet.Shared — versioned contracts and platform primitives shared by the approved clients.

## Non-negotiable architecture rules

- Server is the single source of truth.
- Desktop never accesses PostgreSQL directly.
- Agent never owns authoritative money, pricing, permissions or session ownership.
- Business logic does not live in HTTP endpoints, WPF code-behind, Agent transport code or persistence classes.
- Domain/Application/Api cannot depend directly on EF Core or Persistence.
- Modules communicate through explicit contracts, not direct cross-module database access.
- Money, inventory, session and privileged mutations are transactional, auditable and idempotent.
- External side effects happen after database commit through the durable Outbox boundary.
- Wall-clock access is centralized behind TimeProvider/IGameClock.
- Every feature must have domain rules, use case, persistence, contract, authorization, audit, unit, integration, concurrency, retry/idempotency and recovery coverage.
- Persian (fa-IR, RTL) and English (en-US, LTR) are foundation-level UI cultures.
- Build, restore, test and certification runs are local on the approved Windows machine. GitHub is source/review/orchestration, not certification authority.
- Repo 2 is forensic reference only; it is not a code-reuse source.

## Start here

### Current-state and authority
- docs/roadmap/00-foundation-current-state.md
- docs/architecture/foundation-closure-matrix.md
- docs/roadmap/01-foundation-stabilization.md
- docs/roadmap/00-pre-coding-roadmap.md

### Product and planning
- docs/planning/product-charter.md
- docs/planning/pre-coding-master-plan.md
- docs/planning/product-capability-map.md
- docs/planning/requirements-traceability.md
- docs/planning/definition-of-ready.md
- docs/planning/definition-of-done.md
- docs/planning/assumptions-and-open-decisions.md
- docs/planning/risk-register.md

### Architecture / research
- docs/architecture/README.md
- docs/architecture/data-ownership-and-integration-map.md
- docs/architecture/project-map.md
- docs/research/repo2-to-repo3-comprehensive-audit.md
- docs/security/design-review.md

### Certification / operations
- docs/operations/foundation-local-certification.md
- docs/operations/foundation-certification.md
- docs/operations/scale-capacity-slos.md
- docs/release/release-gates.md
- scripts/verify.ps1

## Foundation exit

The first business vertical slice is intentionally blocked until the closure matrix is fully supported by real evidence:

**Station → Customer → Agent → Session**

Structural green ≠ runtime certified.
