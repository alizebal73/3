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
- persistence-backed Agent lease fencing;
- backup creation/verification/restore implementation;
- release manifest and file-integrity verification;
- Windows Service topology;
- deployment/update boundaries;
- no browser runtime and no remote certification dependency.

## Required local Windows evidence

Run from the exact final commit:

1. `scripts/verify.ps1`
2. `scripts/certify-desktop.ps1`
3. set `GAMENET_DATABASE` to a disposable PostgreSQL database
4. set `GAMENET_RESTORE_DATABASE` to a separate disposable PostgreSQL database
5. `scripts/certify-postgresql.ps1`
6. execute the documented Agent reconnect/fencing scenarios;
7. execute local install/update/rollback smoke;
8. record the final evidence artifact.

`certify-postgresql.ps1` now exercises migration state, idempotency concurrency, database-level audit immutability, Outbox fencing, Agent lease fencing, real PostgreSQL backup creation/verification and restore into an isolated target.

## Intentionally open platform decisions

The following are visible and may not be silently assumed by feature code:
- production Agent transport;
- RPO/RTO owner targets;
- backup/log retention;
- production update signing ownership;
- payment-provider integration scope;
- supported network failure envelope;
- heavy-reporting strategy.

A feature touching an open decision must first close it with an ADR or an explicit approved requirement.

## Certification authority

The approved Windows machine is the certification authority.

GitHub is source control and review. The manual self-hosted workflow may orchestrate the same scripts, but a GitHub checkmark cannot replace local runtime evidence.
