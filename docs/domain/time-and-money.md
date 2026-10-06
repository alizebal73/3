# Time and Money Rules

## Time

- Persist machine timestamps as UTC.
- Convert to configured business timezone for business-day decisions and display.
- Domain calculations use IGameClock or TimeProvider, never wall-clock statics.
- VIP daily limits, tariff periods, shifts and reports declare their business-day basis.
- Billable minutes come from one authoritative SessionTiming model once Sessions are implemented.

## Money

- Domain code uses Shared Money, not raw decimal for monetary state.
- Default business currency is Toman, represented internally as TOM.
- Money arithmetic rejects currency mismatches.
- Rounding rules are explicit before billing is implemented.
- Ledger entries are authoritative; cached balances are projections.
- Refund, reverse, adjustment and settlement require immutable references and idempotency keys.
