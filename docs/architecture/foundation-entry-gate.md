# Foundation Entry Gate

This gate prevents Foundation work from becoming an unbounded pre-product project.

## Must close before the first commercial Vertical Slice

These obligations protect the first real business chain from the failure modes learned from GameNet 2:

- canonical verification/architecture guards;
- clean and upgradeable PostgreSQL migration path;
- transaction coordinator and retry-safe/idempotent mutation foundation;
- append-only audit;
- Outbox claim fencing and an explicit delivery boundary for the first durable external side effect;
- Agent lease/fencing, durable DeviceId and credential lifecycle;
- bounded Agent authentication/provisioning abuse;
- health/readiness authority;
- correlation + operation identity + stable failure envelope;
- authoritative build/schema diagnostics;
- native WPF shell and Desktop↔Server runtime smoke;
- resilient UI state model;
- Agent command safety contract, even though real command execution remains deferred;
- baseline backup/restore and restart/reconnect recovery proof needed by the first session slice.

## Required before production release, but not a blocker for starting the first Vertical Slice

These items are deliberately kept on the Release/Platform-proof track:

- production TLS/certificate trust procedure;
- Windows service installation proof;
- arbitrary installation path certification;
- production-signed update enforcement and signed release evidence;
- SBOM/license/vulnerability evidence;
- Tier S/M load harness and measured scale regression;
- expanded operational metrics/export pipeline;
- complete update rollback/recovery drills;
- final RPO/RTO evidence pack;
- governance/branch protection policy.

A release cannot bypass these items. They are simply scheduled after the product core has a real end-to-end workflow to exercise them.

## Non-negotiable rule

No business module is allowed to grow beyond its Definition of Ready while a Must-close Foundation item is still open.

Conversely, a Release-only item must not be used as an excuse to postpone the first Vertical Slice indefinitely.

## First Vertical Slice

The first product chain is:

Authentication/identity
→ Permission
→ Customer
→ Station
→ Tariff or Custom Pricing
→ Agent Ready
→ Session lifecycle
→ Settlement
→ Wallet/Debt/Payment
→ Audit
→ Recovery/Reconciliation

The slice is accepted only when the same workflow is proven through Desktop, Server, PostgreSQL and Agent on the approved Windows environment.
