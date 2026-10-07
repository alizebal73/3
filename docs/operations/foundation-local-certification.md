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
5. .NET restore;
6. Release build;
7. all ordinary tests.

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

Set GAMENET_DATABASE to a disposable clean PostgreSQL database and run:

scripts/certify-postgresql.ps1

This:
- applies the committed Foundation migration;
- checks that no migrations remain pending;
- runs real PostgreSQL idempotency-concurrency tests;
- runs real PostgreSQL Outbox claim-fencing tests.

## Recovery and deployment evidence

Before Foundation sign-off, perform and record:
- Server restart with clean/known database;
- Desktop launch after Server restart;
- Agent restart/reconnect boundary verification;
- verified backup and restore smoke test on an isolated target;
- install/update/rollback smoke test using the local update package boundary.

## Final gate

Foundation is not certified until the exact final Foundation commit passes the local repository gate plus Desktop and PostgreSQL certification, and the required recovery/deployment evidence is recorded.

Until that evidence exists, no business feature may start.

GitHub is source control only; a remote status/checkmark can never replace local evidence.
