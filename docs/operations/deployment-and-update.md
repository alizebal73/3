# Windows Deployment and Incremental Update Architecture

## Runtime boundaries

Server: headless Windows Service on the server machine.
Desktop: native WPF WinExe with Desktop/Start Menu shortcut.
Agent: Windows Service on every client PC.

Desktop and Agent are independently installable.

## First installation

1. Validate OS/runtime prerequisites.
2. Select installation and data paths; the administrator may choose any writable drive/folder.
3. Install only requested Windows services.
4. Create Desktop/Start Menu shortcuts for Desktop.
5. Validate/apply the database migration on the Server machine.
6. Write an installation manifest.

The service deployment scripts require an elevated PowerShell session, fail when the executable is missing, and configure automatic service recovery.

## Release manifest

Every update package carries:

- ProductVersion
- SchemaVersion
- ApiContractVersion
- AgentProtocolVersion
- migration schema range
- every packaged file's relative path, byte size and SHA-256 checksum

The Shared layer validates the manifest, and `scripts/create-release-manifest.ps1` generates it reproducibly from a package directory.

Relative paths must remain inside the package; traversal, absolute paths, duplicate paths and malformed SHA-256 values are rejected.

## Incremental local updates

Updates are first delivered from a local update folder.

The Foundation implementation currently provides package generation and file-integrity verification. The actual service-safe application/swap/rollback runner remains an explicit Foundation platform-proof slice and may not be described as implemented until its clean-machine smoke test passes.

The intended sequence is:

1. validate manifest and compatibility;
2. stage side-by-side;
3. validate all file checksums;
4. validate database migration ordering;
5. stop only affected components;
6. swap binaries atomically where possible;
7. start affected components;
8. verify live/readiness health;
9. retain a recoverable previous version;
10. restore the previous version on failed health/update checks.

Cloud delivery is a future distribution transport and must not alter runtime architecture.

## Security

Update packages must be integrity-checked. Production signing/verification ownership is an explicit open decision and may not be silently bypassed.
