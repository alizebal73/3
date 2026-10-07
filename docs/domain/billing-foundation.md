# Billing and Money Foundation

## Money representation

Money is integer minor-unit-free Toman amounts through the shared Money primitive. No floating point money arithmetic is permitted.

Every financial record carries currency and uses checked arithmetic.

## Pricing decision

A charge captures the pricing rule used at the moment of charge. Later tariff edits never rewrite historical charges.

## Free credit / bonus

When a business rule says “pay X and receive Y percent free”, the Server calculates the bonus from the paid amount. Example conceptually:

Paid = 100,000
Bonus 10% = 10,000
Usable credit/time basis = 110,000

The actual domain decides whether the result is time credit, wallet credit or promotional entitlement. The Desktop never calculates authoritative totals.

## Financial invariants

- No money mutation without idempotency.
- Every mutation is auditable.
- Reversal references the original transaction.
- Balance is derived from the authoritative ledger; UI cached balance is never authoritative.
- Debt cannot disappear through a display-only operation.
- Concurrent debit/payment/reversal is serialized or rejected using explicit concurrency rules.
