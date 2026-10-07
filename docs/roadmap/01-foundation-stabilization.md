# Foundation Stabilization Action Matrix

This is the active execution matrix for closing Foundation before the first business vertical slice.

## Rules

- No business feature work is permitted while any required row below is open.
- Green structural guards are not sufficient evidence of Foundation closure.
- Every implementation change must be followed by the self-hosted Windows build/test gate.
- Platform-proof rows require evidence from the approved Windows environment; they cannot be closed from static inspection alone.

| ID | Area | Action | State | Closure evidence |
|---|---|---|---|---|
| FS-01 | Build truth | Keep one canonical `verify.ps1` path for restore, Release build and Release tests. | In progress | Self-hosted Runner green on current head |
| FS-02 | Automatic gate | Run the same canonical verification on Foundation branch pushes and PRs targeting `main`. | Implemented | Workflow config + green run |
| FS-03 | Contract coherence | Eliminate compile-time drift between Shared contracts and Server/Agent/Desktop consumers. | In progress | Clean Release build |
| FS-04 | Test coherence | Ensure all Foundation test projects are discoverable and executable by the solution test command. | In progress | Clean `dotnet test` with all expected test projects |
| FS-05 | Architecture guards | Keep structural guards, but treat compile/test as mandatory gates rather than substitutes. | Implemented | Verify script ordering + green run |
| FS-06 | Agent identity | Keep durable DeviceId separate from authentication credentials; define secure provisioning/rotation/revocation path. | Open | Documented credential lifecycle + implementation + smoke evidence |
| FS-07 | Agent lease | Prove one authoritative lease per DeviceId, stale-token rejection and reconnect fencing. | Pending platform proof | Real PostgreSQL concurrency/certification evidence |
| FS-08 | Reconciliation | Define observed Agent state versus authoritative Server state; current heartbeat response is only connection/lease reconciliation. | Open | Contract + implementation + reconnect/state-divergence test |
| FS-09 | Native Desktop | Prove WPF process launch, navigation, localization and Server connectivity on Windows. | Pending platform proof | `certify-desktop.ps1` evidence |
| FS-10 | PostgreSQL | Prove serializable transactions, idempotency, audit immutability, outbox fencing and concurrent Agent lease behavior. | Pending platform proof | `certify-postgresql.ps1` evidence |
| FS-11 | Backup/restore | Prove backup creation, verification and isolated restore. | Pending platform proof | Disposable PostgreSQL restore evidence |
| FS-12 | Update/rollback | Prove side-by-side update, health check and rollback on Windows. | Pending platform proof | Clean install/update/rollback evidence |
| FS-13 | Installer | Prove clean-machine Server/Agent/Desktop installation and service registration. | Pending platform proof | Clean-machine evidence |
| FS-14 | Governance | Keep `main` as the only release line; no long-lived `develop` policy. | Implemented | Branching/GitHub policy consistency |
| FS-15 | Foundation certification | Mark Foundation certified only after required platform rows and open policy decisions are closed. | Blocked | Full foundation certification evidence pack |

## Current order

1. FS-01 through FS-05: make the engineering gate deterministically green.
2. FS-06 and FS-08: close Agent identity/reconciliation design and implementation gaps.
3. FS-07 and FS-10: run real PostgreSQL concurrency/fencing certification.
4. FS-09: certify native Desktop runtime.
5. FS-11 through FS-13: certify recovery, update and installation behavior.
6. Review the closure matrix and certify Foundation.
7. Only then begin the first vertical slice: Station → Customer → Agent → Session.
