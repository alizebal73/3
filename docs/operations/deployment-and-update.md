# Windows Deployment and Incremental Update Architecture

## Runtime

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

`GameNet.Shared` owns manifest shape and validation. Runtime update code must validate the manifest before staging any binary.

Relative paths must remain inside the package; traversal, absolute paths, duplicate paths and malformed SHA-256 values are rejected.

## Incremental local updates

Updates are first delivered from a local update folder.

The updater stages files beside the active installation, verifies the manifest and compatibility matrix, stops only components being updated, applies required migrations, swaps binaries atomically where possible and records success/failure.

A failed update must leave a recoverable previous version.

Cloud delivery is a future distribution transport, not a different application architecture.

## Security

Update packages must be integrity-checked. Production signing/verification cannot be silently bypassed.
