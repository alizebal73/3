# Release Compatibility

The authoritative compatibility dimensions are:
- ProductVersion
- SchemaVersion
- API contract version
- Agent protocol version

The canonical rules are in docs/contracts/release-compatibility.md and are enforced by Shared release compatibility primitives and certification tests.

Database migrations are forward-only during supported upgrades.

Rollback must not silently reinterpret committed financial, inventory or Session records.
