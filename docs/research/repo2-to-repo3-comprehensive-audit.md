# Repo 2 → Repo 3 Comprehensive Foundation Audit

## Executive result

Repo 3 is a clean-slate native Windows architecture rather than a browser migration of Repo 2. The Foundation branch contains the engineering controls required to prevent the main classes of Repo 2 drift. Final certification still requires real Windows/PostgreSQL/runtime evidence; this document never treats static structure as runtime proof.

## Repo 2 failure → Repo 3 control

| Repo 2 problem | Evidence pattern | Repo 3 control | Current Foundation evidence |
|---|---|---|---|
| Giant Server Program | 262 KB Program.cs with endpoints, DTOs and helpers | composition-only Program; module boundaries; source-size guard | implemented + guard |
| Giant CI workflow | 237 KB manual workflow became a second program | one thin manual workflow + canonical scripts/guards | implemented + guard |
| Browser became second product | 79 Dashboard files, React/Vite/package-lock and recurring JSX/TS fixes | native WPF WinExe; browser artifact ban | implemented + guard |
| Contract/model drift | Station/customer/buffet model-alignment fixes | Shared V1 contract authority + no duplicate client DTOs | implemented + contract tests |
| SQLite translation mismatch | repeated DateTimeOffset query fixes | PostgreSQL authoritative store from foundation | implemented + real PG certification pending |
| Migration churn | many migrations while features changed | migration policy + controlled bundle/SQL artifact + startup compatibility gate | implemented |
| Runtime production migration | Repo 2 startup/model initialization coupled to migration behavior | Server never silently migrates Production; deployment owns schema change | implemented |
| Session ownership added late | many ownership DB guards/fixes in Stage 12 | Device/session authority and fencing are Foundation concerns | architecture + Agent fencing implemented; business session slice deferred |
| Agent ownership/reconnect fixes | pairing, process ownership and Session start fixes | durable DeviceId + ConnectionId + persistent LeaseToken + SignalR + heartbeat/reconnect | implemented; local Agent proof pending |
| Mock/server drift | mock product/inventory alignment fixes | server source of truth + contract tests + fake-boundary rule | implemented |
| Preview became a second runtime path | many preview-specific fixes and demo data patches | no browser preview runtime; Desktop native executable is the runtime surface | implemented |
| Installer drift | repeated runtime/installer/registration fixes | separate components, manifest, migration/update compatibility and deploy boundaries | implemented foundation; clean-machine smoke pending |
| Payment/free-credit semantics drift | later payment/report corrections | money primitive + server-owned billing rules + traceability/acceptance before feature code | foundation rules; billing deferred |
| Large-file responsibility creep | large services/DbContext/Hub | size thresholds, module/layer ownership, composition guards | implemented + guard |

## Engineering controls added from external research

- ADRs for decisions with durable consequences.
- Architecture fitness guards executed locally.
- Version-controlled database/deployment artifacts.
- Migration bundle plus reviewable SQL generation.
- Security design review before trust-boundary changes.
- Central dependency pinning and supply-chain guard.
- SBOM generation path for release evidence.
- Native WPF Generic Host for DI/config/logging/lifetime.
- SignalR transport with automatic reconnect plus explicit Server-side fencing/reconciliation.
- User-oriented reliability targets rather than a promise of 100% uptime.

## Foundation boundary

Business modules remain empty by design. The following are not bugs in Foundation:
- customer/station/session/billing/inventory business rules;
- tariff engine details;
- buffet/inventory business workflows;
- payment-provider integration;
- full operator screens beyond the platform shell;
- production-scale business load evidence that depends on real business workflows.

## Final remaining gates

Only these categories remain before the phrase Foundation Certified can truthfully be used:
1. run the canonical verification on the approved Windows machine;
2. certify native Desktop launch/localization;
3. certify real PostgreSQL migrations, concurrency, backup and isolated restore;
4. certify authenticated Agent connection, heartbeat, reconnect and fencing on real Windows;
5. run install/update/rollback and recovery smoke on the real target machines;
6. generate/review release migration artifacts and SBOM;
7. approve policy-open items (RPO/RTO, retention, signing ownership, network failure envelope, report strategy, scale envelope) or explicitly exclude them from the first release.

Until these gates are closed, Repo 3 is not called 100% certified and no business vertical slice is started.