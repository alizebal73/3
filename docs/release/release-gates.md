# Release Gates

A release is a package plus evidence.

## Gate 1 — Source and dependency integrity
- exact source revision recorded;
- dependency versions recorded;
- SBOM generated and validated;
- vulnerability review complete;
- license review complete;
- no unresolved secret scan findings;
- Agent credential and provisioning secrets are supplied only through approved protected deployment channels.

## Gate 2 — Build and tests
- local restore;
- Release build;
- unit/integration/contract/E2E as applicable;
- source/architecture/governance guards;
- real PostgreSQL certification where applicable;
- Agent credential/token lifecycle tests;
- HTTPS-only Production Agent transport validation.

## Gate 3 — Compatibility
- ProductVersion recorded;
- SchemaVersion recorded;
- ApiContractVersion recorded;
- AgentProtocolVersion recorded;
- compatibility matrix reviewed;
- migration notes present.

## Gate 4 — Database deployment
- generated migration reviewed;
- idempotent SQL or migration bundle available;
- deployment identity is separate from runtime application identity;
- clean and upgrade migration evidence present.

## Gate 5 — Package and Deployment
- scripts/prepare-release-package.ps1 is the standard release-package assembly path;
- binaries are signed and signatures verified;
- manifest/checksum compatibility is valid;
- selected component install paths are verified;
- scripts/apply-local-update.ps1 is used for local incremental updates and rollback testing;
- health/readiness is verified after install/update;
- Windows Service installation preserves the Agent credential/DeviceId lifecycle without plaintext secrets in installed configuration.

## Gate 6 — Deployment
- install/update package manifest valid;
- file checksums valid;
- selected component paths verified;
- health/readiness verified after install/update.

## Gate 7 — Recovery
- backup verification passed;
- restore smoke passed;
- update rollback smoke passed;
- critical recovery scenarios exercised.

## Gate 8 — Operator acceptance
- approved workflows complete;
- Persian RTL and English LTR complete;
- permissions match intended role behavior;
- errors and offline states are understandable.

## No-go conditions
- undocumented schema change;
- unapproved breaking contract;
- failed recovery path;
- secrets in artifacts;
- unresolved critical/high vulnerability without documented exception;
- missing evidence for a critical financial or security workflow.
