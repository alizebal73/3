# ADR 0004 — Agent Transport: ASP.NET Core SignalR

## Status
Accepted

## Context
GameNet Agent is a Windows Service on client machines. It requires authenticated bidirectional communication with the authoritative Server, reconnect behavior, heartbeat, connection identity, and lease fencing.

The transport must:
- support a native .NET client;
- integrate with the existing ASP.NET Core Server;
- provide reconnect lifecycle events;
- keep wire contracts in Shared;
- keep transport mechanics outside AgentWorker and business modules.

## Decision
Use ASP.NET Core SignalR as the Agent realtime transport.

Authentication uses the existing JWT validation pipeline. The authenticated Agent token contains a durable device_id claim. The Server accepts the bearer token on the /hubs/agent SignalR handshake.

The Server grants a persistence-backed connection lease keyed by DeviceId and fenced by ConnectionId + LeaseToken. Heartbeat updates the lease-owned presence state. Reconnection reacquires the lease before reconciliation.

## Consequences

Positive:
- one Server transport technology for realtime Agent traffic;
- native .NET Agent client;
- built-in reconnect lifecycle;
- explicit separation between transport, protocol contracts and business decisions.

Negative:
- SignalR becomes part of the Agent protocol compatibility surface;
- HTTPS/TLS and JWT provisioning are release prerequisites;
- connection loss still requires authoritative reconciliation; transport reconnect alone is never considered business-state recovery.

## Alternatives considered
- Raw WebSocket: lower-level and requires more lifecycle/reconnect plumbing.
- gRPC streaming: strong contracts but less aligned with the existing ASP.NET Core realtime endpoint boundary.
- HTTP polling: simpler but inferior for low-latency bidirectional Agent control.

## Revisit triggers
Reconsider only if:
- Agent protocol requirements exceed SignalR semantics;
- measured latency/throughput becomes a demonstrated bottleneck;
- a security or deployment constraint makes SignalR unsuitable.

Decision history is immutable; any future change supersedes this ADR.
