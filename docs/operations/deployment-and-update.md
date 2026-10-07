# Windows Deployment and Incremental Update Architecture

## Runtime
Server: headless Windows Service on the server machine.
Desktop: native WPF WinExe with Desktop/Start Menu shortcut.
Agent: Windows Service on every client PC.

Desktop and Agent are independently installable.

## First installation
1. Validate OS/runtime prerequisites.
2. Select installation and data paths.
3. Install only requested Windows services.
4. Create Desktop/Start Menu shortcuts for Desktop.
5. Validate/apply database migration on the Server machine.
6. Write an installation manifest.

## Incremental local updates
Updates are first delivered from a local update folder.

Each package carries ProductVersion, component versions, compatibility range, migration range and file checksums.

The updater stages files beside the active installation, verifies the manifest, stops only components being updated, applies required migrations, swaps binaries atomically where possible and records success/failure.

A failed update must leave a recoverable previous version.

Cloud delivery is a future distribution transport, not a different application architecture.

## Security
Update packages must be integrity-checked. Production signing/verification cannot be silently bypassed.
