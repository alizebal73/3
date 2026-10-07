# Foundation Closure Matrix

This matrix is the authoritative pre-feature closure list. A row is not closed merely because a file, interface, README, or test project exists.

## Status definitions

- **Implemented** — production-capable code path exists.
- **Verified** — implementation has evidence from the required real environment.
- **Contract-only** — deliberately deferred because a production choice is still open; it cannot be described as implemented.
- **Business-deferred** — intentionally belongs to a future business vertical slice and is not a Foundation defect.

| Area | Architecture | Implementation | Real verification | Status / gate |
|---|---|---|---|---|
| SDK/tooling | yes | yes | local gate | Foundation |
| Layer boundaries | yes | yes | architecture guard | Foundation |
| PostgreSQL persistence | yes | yes | disposable PostgreSQL | Foundation |
| Serializable transaction coordinator | yes | yes | PostgreSQL concurrency | Foundation |
| Idempotency ownership/fencing | yes | yes | PostgreSQL concurrency | Foundation |
| Audit append-only | yes | yes, including DB trigger | direct PostgreSQL mutation test | Foundation |
| Durable Outbox claim fencing | yes | yes | PostgreSQL concurrency | Foundation |
| Agent connection lease/fencing | yes | yes | PostgreSQL concurrency | Foundation |
| Agent network transport | yes | contract boundary | production transport decision pending | Contract-only |
| Agent heartbeat/reconciliation | yes | contract boundary | real Agent proof pending | Contract-only |
| Authentication/authorization | yes | JWT validation/policies | real credential flow pending | Foundation platform proof |
| Native WPF shell | yes | yes | native process launch | Foundation platform proof |
| Desktop ↔ Server | yes | API boundary | real Server/Desktop smoke pending | Foundation platform proof |
| Localization direction | yes | yes | native UI evidence | Foundation platform proof |
| Backup creation/verification | yes | yes | real PostgreSQL + pg_dump | Foundation platform proof |
| Backup restore | yes | yes, isolated target only | disposable restore target | Foundation platform proof |
| Update manifest/checksum | yes | yes | package verification tests | Foundation |
| Update application/swap | yes | staging architecture | local install/update smoke pending | Foundation platform proof |
| Installer packaging | yes | service/install scripts | clean-machine install pending | Foundation platform proof |
| Scale/load harness | yes | scenarios defined | agreed load evidence pending | Release/Foundation proof |
| RPO/RTO policy | yes | documented requirement | owner approval pending | Open decision |
| Update signing | yes | verification boundary | production signing process pending | Open decision |
| Retention | yes | configuration boundary | owner policy pending | Open decision |
| Payment integration | yes | business-deferred | feature-specific | Business-deferred |
| Customer/Station/Session/Billing/etc. | yes | intentionally empty | first vertical slice | Business-deferred |

## Closure rule

Foundation is not called "100% certified" until every row required for the current release gate is either Implemented+Verified or explicitly marked Contract-only/Business-deferred with no hidden dependency on an unresolved decision.

A future feature may not silently implement a Contract-only row. It must first close the relevant decision/ADR and add the required runtime proof.
