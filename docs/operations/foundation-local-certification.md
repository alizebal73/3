# Foundation Local Certification

There is no GitHub Actions execution dependency for GameNet 3.

GitHub is source control, review and history only. Build, restore, test, migration, packaging and certification are performed on the approved Windows machine.

## Repository gate

Run:

scripts/verify.ps1

This verifies:
1. platform/project skeleton;
2. architecture boundaries;
3. source-size limits;
4. Foundation completeness;
5. supply-chain guard;
6. final Foundation structural readiness check;
7. .NET restore;
8. Release build;
9. all ordinary tests.

## Desktop certification

Run:

scripts/certify-desktop.ps1

It builds the native WPF WinExe and launches the actual executable as a Windows process.

Verify on the machine:
- Desktop launches without a browser;
- fa-IR loads with RTL;
- en-US loads with LTR;
- the application can be started from the produced executable.

## PostgreSQL certification

Set:

- GAMENET_DATABASE = disposable clean certification database;
- GAMENET_RESTORE_DATABASE = separate disposable restore target.

Run:

scripts/certify-postgresql.ps1

This:
- applies the committed Foundation migrations;
- checks that no migrations remain pending;
- runs real PostgreSQL idempotency-concurrency tests;
- runs database-level Audit immutability tests;
- runs Outbox claim/expiry fencing tests;
- runs Agent connection lease and heartbeat fencing tests;
- creates and verifies a real PostgreSQL backup;
- restores the verified backup into the isolated target.

## Agent certification

The selected production transport is authenticated ASP.NET Core SignalR.

Local evidence must cover:
- durable DeviceId survives Agent restart;
- one DeviceId cannot hold two authoritative connections;
- heartbeat renews the lease;
- stale lease tokens cannot renew or release;
- reconnect reacquires the lease;
- Server restart followed by Agent reconnect is deterministic;
- unauthorized/non-Agent tokens cannot use the Agent hub;
- network loss does not produce a duplicate authoritative business command.

## Database deployment artifacts

Before release, generate and review both forms as appropriate:

- scripts/create-migration-sql.ps1
- scripts/create-migration-bundle.ps1

The application runtime does not silently apply production migrations.

## SBOM

Provision the pinned Microsoft SBOM tool version recorded in tools/sbom-tool.version and run:

scripts/create-sbom.ps1

Record the generated SPDX SBOM with the release evidence and complete the dependency/license/vulnerability review.

## Recovery and deployment evidence

Before Foundation sign-off, perform and record:
- Server restart with clean/known database;
- Desktop launch after Server restart;
- Agent restart/reconnect boundary verification;
- verified backup and restore smoke test on an isolated target;
- install/update/rollback smoke test using the local update package boundary.

## Final gate

Foundation is not certified until the exact final Foundation commit passes the local repository gate plus Desktop, PostgreSQL and Agent platform certification, and the required recovery/deployment/release evidence is recorded.

Until that evidence exists, no business feature may start.

GitHub is source control only; a remote status/checkmark can never replace local evidence.
