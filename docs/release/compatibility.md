# Release Compatibility

GameNet releases have at least four compatibility dimensions:
- application version
- database schema version
- API contract version
- Agent version

Every update declares the supported Server, Agent and Dashboard compatibility matrix.

Database migrations are forward-only unless a release plan explicitly defines rollback-safe data handling.

Rollback must not silently reinterpret committed financial or inventory records.
