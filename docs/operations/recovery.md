# Recovery Requirements

Recovery is part of correctness.

Scenarios required before release:
1. Server restart while a Session is active.
2. Agent restart while a Session is active.
3. Agent network loss and reconnect.
4. Duplicate command after timeout.
5. Payment response lost and request retried.
6. Inventory mutation committed while acknowledgement is lost.
7. Database restore into a clean instance.
8. Backup verification followed by restore smoke test.

Each path defines the authoritative source, reconciliation direction, duplicate behavior, audit record and operator-visible status.
