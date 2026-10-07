# Agent Transport Boundary

Agent code outside this directory owns identity, command execution and local station control.

Transport code is the only place allowed to own HTTP/WebSocket or other network mechanics. Shared/V1 owns the wire contracts.

The first real Agent connectivity slice must add:
- authenticated connection establishment
- DeviceId/ConnectionId separation
- connection lease/fencing
- heartbeat and state reconciliation
- command acknowledgement with expiry and idempotency

Until that slice exists, the Agent service remains a host/process boundary only; it must not grow ad-hoc networking in AgentWorker.
