# Backup and Restore Architecture

Backup is a recovery mechanism, not a convenience export.

## Artifact requirements

Every backup artifact has:
- immutable BackupId;
- creation timestamp;
- file size;
- SHA-256 digest.

The successful creation and restore operations are recorded in the append-only Audit store.

Release evidence associates a backup with the product/schema compatibility information active at the time of the operation.

## Creation

- Server requests a database-consistent PostgreSQL backup.
- The custom-format archive is written outside the live PostgreSQL data directory.
- Completion is not reported until checksum and archive readability verification succeeds.
- A `backup.created` audit event is committed after the artifact is verified.

## Verification

Verification recomputes the SHA-256 digest and validates that the PostgreSQL archive can be enumerated by `pg_restore --list`.

## Restore

Restore requires:
1. a verified backup;
2. explicit authorized approval;
3. the authoritative Server stopped;
4. a target database isolated from the authoritative source;
5. migration/compatibility validation before returning the system to service.

A restore never runs as an implicit part of a normal operator action.

A `backup.restored` audit event is written without recording connection passwords or other credentials.

## Retention

Retention policy is configuration-driven and auditable. Deletion of retained backups requires explicit authorization.
