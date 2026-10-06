# Reliability Foundation

The authoritative Server survives retries and reconnects without duplicating state.

Required protocol properties:
- command ID
- acknowledgement
- correlation ID
- device identity
- heartbeat
- state snapshot
- reconnect and reconciliation
- bounded retry
- command expiry where appropriate

Server restart must not create a second Session or payment.

Agent restart must reconcile with Server state before executing deferred commands.

Network partitions fail closed for privileged commands and allow safe health reporting.
