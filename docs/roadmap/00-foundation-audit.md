# Stage 0 Foundation Audit

This document records the actual state of the clean-slate foundation.

## Confirmed in repository

- Modular-monolith Server boundary.
- Composition layer separate from business modules.
- 15 business-module boundaries with Domain/Application/Infrastructure/Api directories.
- Shared Api/Contracts/Errors/Results/Primitives boundaries.
- Client Agent/Transport/Identity/GameLaunch boundaries.
- Dashboard feature boundaries.
- Separate Server Unit, Server Integration, Client Unit, Contract, Shared, and Agent test projects.
- One CI workflow.
- Warnings treated as errors.
- .NET SDK baseline and Node baseline documented.
- Source-size architecture guard enforced locally and in CI.
- API contract versioning policy documented.
- main and develop are aligned to the same exact commit.

## Not yet verifiable or not yet configured

1. Self-hosted runner for repo 3: the latest CI run remains queued, so no green build/test claim is made.
2. GitHub branch protection/ruleset: GitHub currently reports no repository rulesets for repo 3. The desired policy is documented in docs/operations/github-settings.md, but it is not active yet.
3. npm lockfile: there is no package-lock.json yet. Until one is generated and committed, CI must use npm install; deterministic npm ci is not yet possible.
4. Real persistence: PostgreSQL boundary is documented, but no DbContext, migrations, transaction infrastructure, or database provider is implemented yet. This is intentional because persistence belongs to the next foundational implementation stage.
5. Real authorization/audit/idempotency/concurrency implementation: the invariants and boundaries are defined, but the implementations will be built before the first business mutation is considered complete.
6. Real E2E harness: only the test boundary exists; no fake green E2E suite is claimed.

## Foundation verdict

The repository structure is sound and materially different from repo 2, but Stage 0 is not yet certified green. Certification requires the external runner/rules configuration and the deterministic Node lockfile, followed by a green exact-SHA CI run.
