# ADR 0005 — Foundation Release Baseline Policies

## Status
Accepted as the engineering baseline for the first release. Product/release sign-off may tighten these values without changing the architecture.

## Decisions

### Recovery
- Target RPO: no more than 15 minutes for authoritative PostgreSQL data when scheduled backup is enabled.
- Target RTO: no more than 60 minutes for a standard single-Server recovery.
- A release cannot claim these targets until the scheduled-backup and restore exercise is evidenced on the approved Windows environment.

### Backup retention
- Baseline retention: 30 days of verified backups.
- A future approved retention policy may increase this; reducing it requires a privacy/business review.

### LAN/WAN failure envelope
- The authoritative Server remains the only authority during network partition.
- Desktop enters Offline/Disconnected state and must not invent authoritative business results.
- Agent loses authoritative command capability when its lease expires.
- Existing local process/session state may be observed by the Agent, but no financial/session ownership mutation is accepted without renewed Server authority.
- Reconnect is followed by lease reacquisition and reconciliation before business commands resume.

### Scale envelope
- Foundation architecture baseline: up to 50 simultaneously connected Agents and multiple concurrent Desktop operator sessions on one shop/server.
- Tier L remains a future measured expansion target up to 100 Agents; it is not a first-release performance promise.
- Scale claims require measured load evidence; changing a target does not automatically justify distributed architecture.

### Reporting
- Operational writes remain on the authoritative transactional path.
- Heavy reports are read-only and must not hold business mutation transactions open.
- When measurements show reporting contention, projections/read replicas may be introduced behind an explicit ADR; the initial architecture does not require them.

### Update signing
- Release packages intended for production are signed by an organization-controlled code-signing identity.
- Verification occurs before installation in addition to SHA-256 integrity checks.
- Private signing material is never stored in the repository or on the normal build/runtime machine.
- Development/local update packages may use checksum verification only and must never be treated as production release artifacts.

## Consequences

These policies remove architecture ambiguity while keeping the implementation configurable. Release evidence, not documentation alone, is the authority for claiming the targets.