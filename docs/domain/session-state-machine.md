# Session State Machine

Specification only. No Session feature is implemented yet.

Canonical path:
Created -> Active -> Paused -> Active -> Ended -> PendingSettlement -> Paid or Debt

Exceptional states/transitions:
- Cancelled
- Recovery
- Transfer

Before coding Session, define start preconditions, pause/resume semantics, billable-minute calculation, transfer ownership, tariff snapshot, recovery, settlement, debt, idempotency and concurrency.

The UI cannot invent a Session state that the Server does not understand.
