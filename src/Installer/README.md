# Windows Installation and Packaging Boundary

The product has three independently deployable components:

- Desktop: native WPF WinExe for the operator/admin machine.
- Server: headless Windows Service and PostgreSQL authority on the server machine.
- Agent: Windows Service installed on each client PC.

The components are independently installed and may use administrator-selected writable installation paths.

## First installation

Required steps:
1. validate prerequisites;
2. select component(s);
3. select arbitrary installation path;
4. install Windows Services only for selected Server/Agent components;
5. create Desktop shortcut when Desktop is selected;
6. deploy the release manifest;
7. apply controlled database migration artifact on the Server machine when schema changes;
8. run health/readiness verification.

Deployment scripts are under deploy/.

## Release package

Every package carries ProductVersion, SchemaVersion, ApiContractVersion, AgentProtocolVersion, migration compatibility range, and each file's relative path, size and SHA-256.

Production release binaries are Authenticode-signed. The signing and verification entrypoints are:
- scripts/sign-release-artifacts.ps1
- scripts/verify-release-signatures.ps1

Private signing keys never belong in the repository or the normal runtime machine.

## Local incremental update

The first supported distribution transport is a local update folder.

The implementation entrypoint is scripts/apply-local-update.ps1.

Its flow is:
1. parse and validate manifest.json;
2. reject traversal/absolute paths;
3. verify every package file size and SHA-256;
4. stage the package side-by-side;
5. stop only the affected Windows Service;
6. move the current installation to a recoverable previous-version folder;
7. activate the staged version;
8. restart the service;
9. run the configured readiness health check;
10. delete the previous version only after health succeeds;
11. rollback to the previous version on any failure.

Desktop can use the same staging/rollback boundary without a Windows Service; application shutdown/restart is owned by the installation/update workflow.

## Database update

The normal application process never performs an uncontrolled Production migration during startup.

Use:
- scripts/create-migration-sql.ps1 for reviewable idempotent SQL;
- scripts/create-migration-bundle.ps1 for a self-contained Windows migration artifact.

Migration compatibility must be checked before the application version is activated.

## No alternate update path

Cloud delivery may be added later, but it must feed the same manifest/checksum/signature/compatibility/staging/rollback pipeline. It may not introduce a second runtime update mechanism.

No installer or updater may bypass Server authority, release compatibility, audit/recovery rules or signature/integrity verification.