# Invariants

Every stateful business rule must be documented here and enforced by business logic, database constraints where applicable, and automated tests.

Initial invariants:
- one active Session per Station
- explicit Agent identity
- Session ownership follows Station/Agent
- financial mutations are transactional and idempotent
- Inventory reverse preserves original StockArea
- mutations require server authorization
- sensitive mutations are audited
- concurrent state transitions are atomic
