# Foundation Certification Gate

No business feature may start until every row is closed.

Repository gates:
- native Desktop WPF WinExe
- bilingual resources
- complete module skeleton
- architecture/source-size/platform guards
- stable V1 contracts and compatibility model
- actor/auth separation
- transaction/idempotency/audit/outbox foundations
- state machines and module blueprints
- backup/recovery architecture
- Windows Service topology
- deployment/update contract
- no browser runtime and no remote CI

Local evidence commands:
1. scripts/verify.ps1
2. scripts/certify-desktop.ps1
3. set GAMENET_DATABASE to a disposable PostgreSQL database
4. scripts/certify-postgresql.ps1
5. execute recovery/restore smoke scenarios and record evidence

The PostgreSQL certification suite is tests/Postgres.CertificationTests and is intentionally separate from the ordinary solution test gate because it requires real infrastructure.

The authoritative evidence is produced on the approved Windows machine. GitHub is source control only.
