# Branching Policy

- `main` is the only release line.
- Foundation branches are temporary certification branches only.
- Feature/fix/refactor/chore branches are short-lived.
- Never develop directly on `main`.
- Merge only reviewed, green, traceable work.
- Every bug fix adds a regression test, invariant/guard, or an ADR-approved exception.
- Schema/contract/security/release changes must update their governing artifact in the same change.

There is no long-lived `develop` branch. The release line stays authoritative and integration happens through small reviewable branches.
