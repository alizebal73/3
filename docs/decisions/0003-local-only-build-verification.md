# ADR 0003 — Local-Only Build and Verification

Status: Accepted

## Decision

GameNet 3 is built, restored, tested, migrated, packaged and certified only on the approved local Windows machine.

GitHub is source control, review and history. A single manual-only GitHub Actions workflow may orchestrate the canonical local scripts on that approved self-hosted machine, but GitHub-hosted infrastructure is never an authority for product certification.

## Reasons

- The project must use the user's own Windows hardware/environment as the authoritative test environment.
- Database, Agent, Windows desktop and LAN behavior must be verified in the real target environment.
- Remote CI availability must never block or falsely certify the product.
- The same local scripts are used for repeatable verification.
- A manual-only runner entry point makes the connected Windows runner useful without turning workflow files into a second build system.

## Rules

- scripts/verify.ps1 is the canonical local gate.
- The repository permits exactly one workflow: .github/workflows/foundation-local.yml.
- The workflow is workflow_dispatch only and runs on the approved self-hosted Windows runner.
- A remote checkmark can never replace local evidence.
- Build artifacts and machine credentials remain local unless explicitly released.
- PostgreSQL integration and concurrency certification must run locally.
- Desktop executable launch verification must run locally.
