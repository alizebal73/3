# Release Gates

A release is a package plus evidence.

## Gate 1 — Source and dependency integrity
- exact source revision recorded;
- dependency versions recorded;
- SBOM generated and validated;
- vulnerability review complete;
- license review complete;
- no unresolved secret scan findings.

## Gate 2 — Build and tests
- local restore;
- Release build;
- unit/integration/contract/E2E as applicable;
- source/architecture/governance guards;
- real PostgreSQL certification where applicable.

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

## Gate 5 — Deployment
- install/update package manifest valid;
- file checksums valid;
- selected component paths verified;
- health/readiness verified after install/update.

## Gate 6 — Recovery
- backup verification passed;
- restore smoke passed;
- update rollback smoke passed;
- critical recovery scenarios exercised.

## Gate 7 — Operator acceptance
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
