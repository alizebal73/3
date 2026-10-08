# Branching Policy

- `main` is the only release line.
- Foundation branches are temporary certification branches only.
- Feature/fix/refactor/chore branches are short-lived.
- Never develop directly on `main`.
- Merge only reviewed, green, traceable work.
- Every bug fix adds a regression test, invariant/guard, or an ADR-approved exception.
- Schema/contract/security/release changes must update their governing artifact in the same change.

There is no long-lived `develop` branch. If a compatibility branch exists, it must mirror the canonical release baseline and must not become a second development line.

## Foundation stabilization rule

Foundation work is consolidated into the canonical baseline. The active release line must contain the same Foundation state as the final Foundation implementation branch before business development begins.

Historical Foundation branches may be retained temporarily for traceability, but they are not independent sources of truth.

Every Foundation correction must be traceable to the Foundation stabilization matrix or an explicit architecture/security decision.

## Source-of-truth rule

For the current Foundation state, use:

- `docs/roadmap/00-foundation-current-state.md`
- `docs/roadmap/00-pre-coding-roadmap.md`
- `docs/roadmap/01-foundation-stabilization.md`
- `docs/architecture/foundation-closure-matrix.md`

A stale roadmap, branch snapshot, README summary, or historical audit never overrides these sources.
