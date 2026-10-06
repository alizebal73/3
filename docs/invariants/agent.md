# Agent Invariants

- DeviceId is durable identity and is not inferred from IP.
- Pairing can rotate credentials only through an authenticated server operation.
- A DeviceId has at most one authoritative active connection.
- Commands have unique IDs and acknowledgements.
- Retriable commands are safe to retry when marked idempotent.
- Agent state is reconciled against Server state after reconnect.
- Agent never computes authoritative price, debt, permission or Session ownership.
