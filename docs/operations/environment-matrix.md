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

The Agent/Server pair is configured through normal .NET configuration keys; certification must not rely on custom environment variables that are invisible to options binding.

Development-only local certification:
- `GameNet__AgentTransport__ServerBaseUrl`
- `GameNet__AgentTransport__AllowInsecureHttpForDevelopment=true`
- `GameNet__AgentIdentity__RootPath=<temporary>`
- `DOTNET_ENVIRONMENT=Development`
- `GAMENET_AGENT_BOOTSTRAP_SECRET=<provisioned credential>`
- Server authentication must be enabled for the real Token -> SignalR -> Lease -> Heartbeat -> Reconciliation path, with issuer/audience/signing/provisioning values supplied by the certification environment.

Production:
- HTTPS only.
- No development HTTP override.
- Agent credential is provisioned and stored by the Agent credential store.
- Authentication remains a server authority; the Agent never receives business authorization.

## Rule

An environment-specific value belongs in configuration, not source-code forks.
