# Recovery Invariants

- Restart cannot create a second active Session for a Station.
- Retried payment cannot create a second settlement.
- Retried inventory sale cannot consume stock twice.
- Reconnect cannot let stale Agent state overwrite newer Server state.
- Server state is authoritative unless a documented reconciliation rule says otherwise.
- Recovery corrections are auditable.
