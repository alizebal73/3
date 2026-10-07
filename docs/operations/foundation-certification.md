# Foundation Certification Gate

No business feature may start until every required Foundation row is closed and the exact final commit has local evidence.

The authoritative closure matrix is:
- docs/architecture/foundation-closure-matrix.md

A README, interface, test project or green static guard is not runtime certification by itself.

## Required repository evidence

- native WPF WinExe;
- bilingual resources;
- module skeleton with zero hidden business implementation;
- architecture/source-size/platform guards;
- stable V1 contracts and compatibility model;
- actor/auth separation;
- transaction/idempotency/audit/outbox foundations;
- persistent Agent lease/fencing and selected SignalR transport;
- backup creation/verification/restore implementation;
- release manifest and file-integrity verification;
- migration bundle and reviewable migration SQL tooling;
- Windows Service topology;
- deployment/update boundaries;
- supply-chain guard and SBOM generation tooling;
- no browser runtime and no remote certification dependency.

## Required local Windows evidence

Run from the exact final commit:

1. `scripts/verify.ps1`
2. `scripts/certify-desktop.ps1`
3. set `GAMENET_DATABASE` to a disposable PostgreSQL database
4. set `GAMENET_RESTORE_DATABASE` to a separate disposable PostgreSQL database
5. `scripts/certify-postgresql.ps1`
6. run the documented authenticated Agent connection/reconnect/heartbeat/fencing scenarios;
7. generate and validate the release migration artifact;
8. generate the release SBOM and record dependency/license/vulnerability review;
9. execute local install/update/rollback smoke;
10. record the final evidence artifact.

`certify-postgresql.ps1` exercises migration state, idempotency concurrency, database-level audit immutability, Outbox fencing, Agent lease fencing, real PostgreSQL backup creation/verification and restore into an isolated target.

## Intentionally policy-open items

These are not silently assumed by Foundation code:
- owner-approved RPO/RTO;
- retention periods;
- production update signing ownership;
- supported LAN/WAN failure envelope;
- payment-provider integration scope;
- heavy-reporting strategy;
- scale-envelope approval.

A feature touching an open policy must first close it with an ADR or approved requirement.

## Certification authority

The approved Windows machine is the certification authority.

GitHub is source control and review. The manual self-hosted workflow may orchestrate the same scripts, but a GitHub checkmark cannot replace local runtime evidence.
