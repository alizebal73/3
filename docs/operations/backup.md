# Backup and Restore Architecture

Backup is a recovery mechanism, not a convenience export.

## Artifact requirements
Every backup has an immutable BackupId, creation timestamp, file size, SHA-256 digest and source schema/product version.

## Creation
- Server requests a database-consistent backup.
- The artifact is written outside the live PostgreSQL data directory.
- Completion is not reported until integrity verification succeeds.
- Backup metadata is audited.

## Verification
Verification recomputes the digest and validates that the archive is readable.

## Restore
Restore requires:
1. a verified backup;
2. explicit authorized approval;
3. the authoritative Server stopped;
4. a clean/isolated PostgreSQL restore target;
5. migration/compatibility validation before returning the system to service.

A restore never runs as an implicit part of a normal operator action.

## Retention
Retention policy is configuration-driven and auditable. Deletion of retained backups requires explicit authorization.
