# Stage 0 — Platform Foundation

## Completed structure
- Modular-monolith Server boundary
- Composition layer
- Module ownership folders
- Shared API/error/result/primitives
- Separate unit/integration/contract test layers
- Client Agent sub-boundaries
- Dashboard feature boundaries
- Main/develop branch policy
- One self-hosted CI workflow

## Exit gate
The foundation is complete only when the self-hosted Runner for repo 3 passes the CI workflow on the exact main commit.

## Next
Build the first vertical slice only:
Station → Customer → Agent → Session.

Do not begin Reports, Settings, Buffet, VIP, or installers before that slice has real state transitions, concurrency tests, authorization, audit, and integration coverage.
