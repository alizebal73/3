# Foundation Certification Gate

No business feature may start until every row is closed.

Native Desktop: WPF WinExe builds and launches locally on Windows.
Bilingual UI: fa-IR RTL and en-US LTR tests pass.
Server: headless and free of UI dependency.
Agent: only Shared/approved contract dependencies.
Modules: every module has Api/Application/Domain/Infrastructure.
Module guard: forbidden dependencies are rejected.
API V1: stable envelope, headers, compatibility rules.
Identity: Human, DeviceId, ConnectionId, CustomerLogin and Session remain distinct.
Transactions: isolation, concurrency and side-effect rules are enforced.
Idempotency: lease ownership, replay and reclaim semantics are tested.
Outbox: durable row plus lease/claim and post-commit publication boundary exists.
PostgreSQL: clean migration and real PostgreSQL integration pass.
Recovery: restart/reconnect/retry/restore scenarios have executable coverage.
Deployment: Server/Agent services and Desktop installer/update boundary are defined.
E2E: native Windows E2E project exists.
Source guards: architecture and source-size guards pass.
No browser: no Dashboard/React/Vite/WebView/Node artifacts.
No remote CI: GitHub is source control only.
Documentation: active architecture/security docs contain no retired Dashboard runtime reference.

Authoritative evidence is produced on the approved Windows machine using scripts/verify.ps1 plus explicit PostgreSQL/Desktop certification commands.
