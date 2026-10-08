# Environment Matrix

## Developer / review

Purpose: fast local feedback.

Characteristics:
- pinned SDK/tooling;
- isolated test data;
- no production secrets;
- unit/contract/build gates.

## Foundation certification

Purpose: prove the platform on the approved Windows machine.

Requires:
- real PostgreSQL;
- native Desktop process launch;
- Windows service behavior;
- concurrency/recovery scripts;
- update/rollback smoke;
- approved local network behavior.

## Shop staging

Purpose: pre-release verification in a shop-like environment.

Requires:
- actual LAN topology;
- representative Agent count;
- real service installation;
- real PostgreSQL;
- representative tariffs/data;
- backup/restore exercise;
- operator workflow checks.

## Production/shop

Purpose: supported operation.

Requires:
- approved release manifest;
- compatible schema/API/Agent versions;
- verified backup;
- monitoring/diagnostics;
- recovery procedure;
- support/rollback procedure.

## Agent transport certification profile

The Agent/Server pair is configured through normal .NET configuration keys. Secret-only certification inputs may be supplied directly as environment variables because the credential store intentionally reads the bootstrap secret without placing it in appsettings.

Development-only local certification:
- `GameNet__AgentTransport__ServerBaseUrl`
- `GameNet__AgentTransport__AllowInsecureHttpForDevelopment=true`
- `GameNet__AgentIdentity__RootPath=<temporary>`
- `DOTNET_ENVIRONMENT=Development`
- `GAMENET_AGENT_PROVISIONING_KEY=<temporary certification provisioning authority>` for the certification script.
- The Server process must also receive the same secret through `GameNet__Agent__ProvisioningKey`.
- For the real token path the Server process must have `GameNet__Authentication__Enabled=true`, `GameNet__Authentication__Issuer=<issuer>`, `GameNet__Authentication__Audience=<audience>`, and `GameNet__Authentication__SigningKey=<temporary 32+ character key>`.
- optionally `GAMENET_AGENT_DEVICE_ID=<temporary certification identity>`; otherwise the script generates a unique DeviceId.
- The certification script provisions the credential through the Server, injects the returned secret only into the temporary Agent process, and revokes the credential during cleanup.
- Server authentication must be enabled for the real Token -> SignalR -> Lease -> Heartbeat -> Reconciliation path, with issuer/audience/signing/provisioning values supplied through the Server's normal .NET configuration environment variables above.

Production:
- HTTPS only.
- No development HTTP override.
- Agent credential is provisioned and stored by the Agent credential store.
- Authentication remains a server authority; the Agent never receives business authorization.

## Rule

An environment-specific value belongs in configuration, not source-code forks.
