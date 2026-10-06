# Idempotency Foundation

Money, inventory and Session mutations must use a stable idempotency key.

The persistence protocol has three states:
- processing
- completed
- expired and reclaimable

A claim has a lease token. Only the current lease owner can complete the record.

Expired processing records are atomically reclaimable. A concurrent claimant cannot silently acquire the same active lease.

The business mutation and idempotency completion belong to the same transaction whenever the use case requires atomic financial or Session state.
