# Scale, Capacity and Reliability Envelope

This document defines the engineering envelope. Business capacity can be raised without changing the architecture by updating the approved targets and proving the new load.

## Capacity tiers

### Tier S — current single-shop baseline
- 1 authoritative Server
- PostgreSQL on the Server environment
- small operator Desktop count
- one shop's full station fleet
- local LAN / controlled WAN connectivity

### Tier M — growth target
- up to 50 Agent-connected stations
- multiple Desktop operator sessions
- sustained mixed read/write workload
- periodic reporting workloads
- reconnect storms after network interruption

### Tier L — expansion target
- up to 100 Agent-connected stations
- concurrent operator activity
- scheduled reporting/export workloads
- burst reconnects and command replay
- large audit and transaction history

The system must not introduce distributed architecture merely to satisfy Tier S or M. Scale decisions must be evidence-driven.

## Mandatory load scenarios

1. Normal operator activity.
2. All Agents send heartbeat/reconnect within a short window.
3. Multiple operators act on the same Station/session.
4. Repeated payment request with the same idempotency key.
5. Concurrent inventory sale/return.
6. Report query while mutations continue.
7. Server restart with active sessions.
8. Database connection interruption.
9. Outbox backlog growth.
10. Update/rollback health-check cycle.

## Initial SLO candidates

These are engineering targets, not business promises, until approved.

- Server local health response: p95 < 250 ms.
- Ordinary authoritative reads: p95 < 500 ms.
- Ordinary operator mutations: p95 < 1 s.
- Critical financial mutation: p95 < 1.5 s excluding human confirmation.
- Agent command acknowledgement: p95 < 1 s on healthy LAN.
- Reconnect/reconciliation: deterministic and idempotent; no duplicate financial/session effect.
- Desktop launch: succeeds without browser/network dependency.
- Recovery: documented RPO/RTO values must exist before release.

The target is not “100% reliability”. Reliability objectives should reflect user-visible outcomes and be tied to an explicit policy for responding to reliability degradation.

## Performance rule

No performance optimization is accepted because it “feels faster”. A change must state the workload, baseline, measurement and regression threshold.
