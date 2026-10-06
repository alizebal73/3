# API Contract Versioning

Shared contracts are the compatibility boundary between Server, Dashboard, and Agent.

## Rules

- New fields should be additive and optional when backward compatible.
- Meaning changes require a new contract version.
- Removing or renaming a field requires a new version.
- Server endpoints must declare the contract version they implement once the first versioned API is introduced.
- Dashboard and Agent must not recreate server DTOs locally when a shared contract exists.
- Contract changes require ContractTests and an explicit migration note.

Initial namespace:
- `GameNet.Shared.Contracts.V1`

No business feature may silently change an existing contract's meaning.
