# Agent Transport Foundation

## Selected transport

ASP.NET Core SignalR over the Server's authenticated /hubs/agent endpoint.

The Agent is a native .NET Windows Service; no browser runtime is involved.

## Identity and authority

- DeviceId is durable and stored locally under Windows Common Application Data.
- ConnectionId is ephemeral and owned by the realtime transport.
- LeaseToken fences stale connections.
- JWT authentication supplies the DeviceId claim.
- Server remains authoritative; Agent reports observed state but cannot authorize itself.

## Lifecycle

1. Agent loads/creates durable DeviceId.
2. Agent connects to /hubs/agent.
3. Agent acquires a persistent Server lease.
4. Agent sends periodic heartbeats.
5. Agent can request V1 Lease/Identity Reconciliation.
6. SignalR reconnects after transient network loss.
7. Reconnected Agent reacquires the lease before continuing.
8. Stale connections cannot renew or release the newer lease.

## Compatibility

Agent transport compatibility is represented by AgentProtocolVersion in the release compatibility model.

Transport changes that alter message semantics require a protocol compatibility decision; additive fields remain compatible only when older peers can ignore them safely.

## Security

Production uses HTTPS and authenticated JWT bearer tokens. Agent secrets are provisioned during deployment and never committed to source control.

The token is not treated as business authorization. Server-side permission and ownership rules remain authoritative.

## Certification scenarios

Required local evidence:
- first connection obtains one authoritative lease;
- competing connections for one DeviceId produce one authority;
- heartbeat from the owner succeeds;
- stale lease heartbeat fails;
- reconnect reacquires authority;
- old connection cannot release the new lease;
- Server restart followed by Agent reconnect is deterministic;
- network loss followed by reconnect does not duplicate a business command.


## Reconciliation boundary

V1 reconciliation proves transport authority, DeviceId binding, protocol compatibility and lease ownership. It does not claim to reconcile future business state such as Session, game runtime, billing, inventory or policy versions.

Business-state reconciliation must be added with the first vertical slice that owns that state, using the same evidence rules as every other authoritative workflow. This prevents the Foundation from pretending that a connection heartbeat is a full application-state reconciliation.

## Transport security

The Agent may use HTTP only in Development when `AllowInsecureHttpForDevelopment=true` is explicitly configured. Production must use HTTPS so the bootstrap credential and short-lived token exchange are never sent over plaintext transport.


## Bootstrap and installation

Production installation must provision a DeviceId and an Agent credential before starting the service. The supported deployment flow may supply the one-time bootstrap secret through the protected Windows environment only long enough for the Windows Service to consume it.

The Agent Service then stores the credential with Windows DPAPI under its service identity and no longer requires the plaintext bootstrap variable. The deployment script removes the machine bootstrap environment variable after the protected credential file is confirmed.

A rotation changes the server-side active credential and requires the replacement secret to be delivered to the Agent through an approved deployment/maintenance operation. Rotation must not be implemented by committing the secret into appsettings or source control.
