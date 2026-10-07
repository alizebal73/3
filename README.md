# GameNet Manager 3

Clean-slate Windows GameNet/CyberCafe management platform.

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
- All build, restore, test and certification runs are local on the approved Windows machine. A manual self-hosted workflow may orchestrate the same local scripts, but never becomes certification authority.
- No business feature starts until pre-coding governance and Foundation certification gates are complete.
- Repo 2 architecture is not a source for code reuse; it is forensic reference only.

## Start here

- docs/planning/product-charter.md
- docs/planning/pre-coding-master-plan.md
- docs/planning/requirements-traceability.md
- docs/planning/definition-of-ready.md
- docs/architecture/README.md
- docs/architecture/data-ownership-and-integration-map.md
- docs/domain/product-requirements.md
- docs/invariants/
- docs/roadmap/00-pre-coding-roadmap.md
- docs/roadmap/00-platform-foundation.md
- docs/operations/foundation-local-certification.md
- docs/architecture/foundation-closure-matrix.md
- docs/research/repo2-to-repo3-comprehensive-audit.md
- docs/development/engineering-workflow.md
- docs/security/design-review.md
- scripts/verify.ps1

The first vertical slice is intentionally blocked until Foundation certification is complete.
