# Agent Credential Lifecycle

## Purpose

Durable DeviceId identifies a client device; it is not itself an authentication credential.

The Agent credential lifecycle must be explicit before Foundation certification.

## Required lifecycle

1. Provisioning: a trusted deployment operation binds one credential to one DeviceId.
2. Authentication: the Agent obtains a short-lived authenticated access token for the approved Agent transport.
3. Authorization: Server policy requires authenticated Agent identity plus the matching DeviceId claim.
4. Rotation: a new credential can be issued and confirmed before the old credential is retired.
5. Revocation: Server can revoke a DeviceId credential without deleting the durable DeviceId.
6. Expiry: short-lived access tokens must expire automatically.
7. Recovery: reinstall/reprovision must be deterministic and must not accidentally reuse a stale credential.
8. Audit: provisioning, rotation, revocation and failed credential use are auditable security events.

## Storage rules

- Raw Agent credentials must never be committed to source control.
- Access tokens must not be stored as ordinary application configuration values.
- Durable local secrets must use an approved Windows protected-secret mechanism.
- Server-side credential records must be revocable and must not rely on the DeviceId alone.

## Current implementation boundary

Repo 3 now has the Foundation implementation for credential provisioning, rotation, revocation, short-lived JWT issuance, and Windows DPAPI-protected Agent bootstrap-secret storage. Runtime certification remains required.

This is an explicit Foundation Stabilization item, not a business-feature dependency. Foundation certification must not claim Agent authentication is fully closed until the lifecycle and its Windows smoke tests exist.

## Required certification

- fresh Agent provisioning;
- authenticated first connection;
- wrong/revoked credential rejected;
- credential rotation without duplicate Device authority;
- old credential rejected after revocation/expiry;
- Agent restart reloads the correct protected credential;
- Server restart and Agent reconnect remain deterministic;
- all security lifecycle events are auditable.


## Transport requirement

Credential provisioning and token exchange must use HTTPS outside Development. The Agent configuration contains an explicit development-only HTTP escape hatch; Production validation rejects HTTP.

## Windows service deployment invariant

The credential store is bound to the Windows identity that runs the Agent Service. The installation process must provision the durable DeviceId before the first authenticated connection and may expose the bootstrap secret only for the bounded first-start window. After DPAPI storage succeeds, the bootstrap environment variable must be removed.
