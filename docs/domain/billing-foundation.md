# First Vertical Slice — Commercial Core Contract

This document is the product boundary for the first real GameNet workflow after Foundation closure.

## Goal

Prove one complete operator workflow end-to-end:

Authentication/identity → permission → customer → station → tariff/custom pricing → agent readiness → session lifecycle → settlement → wallet/debt/payment → audit → recovery.

A feature is not complete when its screen exists. The whole server-authoritative chain must work.

## Operator pricing model

Tariffs are shortcuts and reusable pricing rules, not mandatory fixed choices.

When an operator finds a customer, the session flow supports both:

1. **Tariff mode** — select a configured tariff such as 1 hour, 2 hours, VIP, PC, PS5, or another configured rule.
2. **Custom/manual mode** — enter an explicit price and, when applicable, an explicit duration/time basis.

The Desktop may present recommended tariff buttons, but it must never force the operator to select a predefined duration when the business permission allows custom pricing.

## Manual/custom price rules

- The operator may enter a custom amount in Toman.
- The Server validates the amount, permission, applicable station/session rules, and the final pricing snapshot.
- Manual pricing never means bypassing authorization or audit.
- A custom price is recorded as a pricing decision/snapshot attached to the session or charge; later tariff edits do not rewrite it.
- The authoritative amount is calculated and persisted by the Server, not by the Desktop.
- Every manual/custom pricing mutation is auditable and carries the actor, time, reason/description when required by policy, and operation/idempotency identity.
- Permission can distinguish ordinary tariff selection from privileged manual-price override.
- The UI must show clearly whether the session uses a configured tariff or a custom/manual price.

## Customer wallet / profile charge

The operator may also directly credit a customer's wallet/profile with an entered amount.

Example:
- Operator selects customer.
- Operator enters 100,000 Toman.
- Server records an exactly 100,000 Toman wallet credit.
- Payment method is recorded when money is received.
- Wallet balance is derived from the authoritative ledger.

This wallet top-up is a different operation from setting a custom session price. Neither operation is forced to use fixed hourly values.

## Discount, free money, and free time

Discounts, promotional free money, and free time are distinct concepts and must not be collapsed into one field.

Example:
- Paid amount = 100,000 Toman.
- A 10% free-money rule produces 10,000 Toman promotional credit.
- Total usable wallet credit = 110,000 Toman.

Free time is separate from free money and requires its own ledger/entitlement semantics.

## Settlement

Session settlement calculates the authoritative charge from the recorded pricing snapshot, actual session state, applicable discounts/promotions, and other approved business rules.

The operator may settle to:
- wallet;
- cash/card/POS/transfer;
- combined approved methods;
- debt, when the actor has permission and the business rule allows it.

The same payment cannot be applied twice because financial mutations are idempotent.

## Non-negotiable invariants

- No floating-point money arithmetic.
- No money mutation without idempotency.
- No financial mutation without audit.
- Customer wallet and Cash Register are separate authorities.
- Reversal references the original transaction.
- Historical charges retain their pricing snapshot.
- Desktop never becomes the financial authority.
