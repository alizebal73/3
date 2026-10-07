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
| JWT/authentication policy | yes | transport/auth policy exists; Agent credential lifecycle incomplete | real credential provisioning/rotation/revocation flow | Implementation gap |
| Native WPF shell | yes | yes | native process launch | Platform-proof pending |
| Desktop ↔ Server | yes | API boundary + central connectivity state | real Server/Desktop smoke | Platform-proof pending |
| UI Foundation | yes | reusable shell/navigation/state/command/theme/accessibility baseline | native shell workflow | Platform-proof pending |
| Game catalog foundation | yes | Server Games module + V1 launch-policy contracts | contract/architecture guard | Implemented; business workflow deferred |
| GameAccounts foundation | yes | Server GameAccounts module + allocation contract boundary | contract/architecture guard | Implemented; business workflow deferred |
| ClientControl foundation | yes | Server ClientControl module + Agent capability boundaries | contract/architecture guard | Implemented; execution workflow deferred |
| Client execution contract | yes | game launch/stop, lock, restart/shutdown, maintenance, diagnostics, policy contracts | real Agent command/reconnect smoke | Platform-proof pending |
| Backup creation/verification | yes | yes | pg_dump + archive verification | Platform-proof pending |
| Backup restore | yes | yes, isolated target only | disposable restore target | Platform-proof pending |
| Migration deployment artifact | yes | bundle + reviewable SQL | migration build/review evidence | Implemented |
| Update manifest/checksum | yes | yes | package verification tests | Implemented |
| Update application/swap | yes | local side-by-side staging + rollback script | clean install/update/rollback smoke | Platform-proof pending |
| Installer packaging | yes | service/install scripts | clean-machine install | Platform-proof pending |
| Supply-chain guard | yes | yes | canonical local verification | Implemented |
| SBOM/dependency evidence | yes | policy defined | release artifact generation/review | Release-proof pending |
| Scale/load harness | yes | scenarios + Tier S/M baseline | measured load evidence | Release/Foundation proof pending |
| RPO/RTO policy | yes | baseline <=15m / <=60m | runtime recovery evidence | Accepted baseline; evidence pending |
| Update signing | yes | Authenticode signing + verification scripts | signed-package release evidence | Platform-proof pending |
| Retention | yes | 30-day verified-backup baseline | release/ops evidence | Accepted baseline |
| Network failure envelope | yes | explicit Server/Offline/lease-expiry baseline | reconnect/fencing evidence | Accepted baseline; evidence pending |
| Payment integration | yes | business-deferred | feature-specific | Business-deferred |
| Customer/Station/Session/Billing/etc. | yes | intentionally empty | first vertical slice | Business-deferred |

## Closure rule

Foundation is not called **100% certified** until every required platform row has implementation plus real evidence on the approved Windows environment, and every policy-open row is either approved or explicitly excluded from the current release.

Business-deferred rows are not defects. They are blocked intentionally until Foundation is certified.

A future feature may not silently change a Foundation row. It must update the relevant ADR, traceability, contract, tests and release evidence.
