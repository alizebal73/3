# Release and Compatibility Contract

GameNet releases four related compatibility dimensions:
- ProductVersion
- SchemaVersion
- ApiContractVersion
- AgentProtocolVersion

Rules:
1. Desktop updates never require a database downgrade.
2. Database migrations are forward-only during supported upgrades.
3. Additive API fields are backward compatible.
4. Semantic API changes require a new contract version.
5. Agent protocol changes require an explicit compatibility window.
6. Unsupported Agent protocol versions receive a stable error.
7. Desktop and Agent expose versions to Server compatibility diagnostics.
8. Every release records a compatibility matrix and migration note.
9. Application rollback is supported only when the existing schema remains compatible; destructive schema rollback is not a normal update mechanism.

Installer/update code validates this matrix before replacing binaries.
