# API Contract Versioning

Shared contracts are the compatibility boundary between Server, Dashboard and Agent.

Rules:
- new backward-compatible fields are additive
- semantic changes require a new contract version
- removing or renaming a field requires a new version
- endpoints declare the contract version they implement
- Dashboard and Agent do not recreate server DTOs when a shared contract exists
- contract changes require ContractTests and a migration note

Initial namespace is GameNet.Shared.Contracts.V1.

Protocol messages, error envelopes, health responses and security permissions in V1 are shared foundation contracts. Business contracts are added only when their domain specification is approved.
