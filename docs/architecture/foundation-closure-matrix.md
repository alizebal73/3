# Foundation Closure Matrix

This matrix is the authoritative pre-feature closure list. A row is not closed merely because a file, interface, README, or test project exists.

## Status definitions

- **Implemented** — production-capable code path exists.
- **Verified** — implementation has evidence from the required real environment.
- **Platform-proof pending** — implementation exists, but the approved Windows machine must still produce the required runtime evidence.
- **Policy-open** — a business/release policy decision is still required; no hidden default may be invented.
- **Business-deferred** — intentionally belongs to a future business vertical slice and is not a Foundation defect.

| Area | Architecture | Implementation | Verification requirement | Current state |
|---|---|---|---|---|
| SDK/tooling | yes | yes | local gate | Implemented |
| Layer boundaries | yes | yes | architecture guard | Implemented |
| PostgreSQL persistence | yes | yes | disposable PostgreSQL | Platform-proof pending |
| Serializable transaction coordinator | yes | yes | PostgreSQL concurrency | Platform-proof pending |
| Idempotency ownership/fencing | yes | yes | PostgreSQL concurrency | Platform-proof pending |
| Audit append-only | yes | yes, including DB trigger | direct PostgreSQL mutation test | Platform-proof pending |
| Durable Outbox claim fencing | yes | yes | PostgreSQL concurrency | Platform-proof pending |
| Agent connection lease/fencing | yes | yes | PostgreSQL concurrent lease test | Platform-proof pending |
| Agent SignalR transport | yes | yes | authenticated Windows Agent smoke | Platform-proof pending |
| Agent heartbeat/reconciliation | yes | yes | reconnect/heartbeat/fencing smoke | Platform-proof pending |
| Agent durable DeviceId | yes | yes | Agent restart/reload smoke | Platform-proof pending |
| JWT/authentication policy | yes | yes | real credential flow | Platform-proof pending |
| Native WPF shell | yes | yes | native process launch | Platform-proof pending |
| Desktop ↔ Server | yes | API boundary | real Server/Desktop smoke | Platform-proof pending |
| Localization direction | yes | yes | fa-IR RTL + en-US LTR native UI | Platform-proof pending |
| Backup creation/verification | yes | yes | pg_dump + archive verification | Platform-proof pending |
| Backup restore | yes | yes, isolated target only | disposable restore target | Platform-proof pending |
| Migration deployment artifact | yes | bundle + reviewable SQL | migration build/review evidence | Implemented |
| Update manifest/checksum | yes | yes | package verification tests | Implemented |
| Update application/swap | yes | staging architecture | clean install/update/rollback smoke | Platform-proof pending |
| Installer packaging | yes | service/install scripts | clean-machine install | Platform-proof pending |
| Supply-chain guard | yes | yes | canonical local verification | Implemented |
| SBOM/dependency evidence | yes | policy defined | release artifact generation/review | Release-proof pending |
| Scale/load harness | yes | scenarios defined | agreed load evidence | Release/Foundation proof pending |
| RPO/RTO policy | yes | documented requirement | owner approval | Policy-open |
| Update signing | yes | verification boundary | signing process | Policy-open |
| Retention | yes | policy boundary | owner approval | Policy-open |
| Network failure envelope | yes | transport/recovery policy | owner/engineering approval | Policy-open |
| Payment integration | yes | business-deferred | feature-specific | Business-deferred |
| Customer/Station/Session/Billing/etc. | yes | intentionally empty | first vertical slice | Business-deferred |

## Closure rule

Foundation is not called **100% certified** until every required platform row has implementation plus real evidence on the approved Windows environment, and every policy-open row is either approved or explicitly excluded from the current release.

Business-deferred rows are not defects. They are blocked intentionally until Foundation is certified.

A future feature may not silently change a Foundation row. It must update the relevant ADR, traceability, contract, tests and release evidence.
