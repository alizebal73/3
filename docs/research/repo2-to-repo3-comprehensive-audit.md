# Repo 2 → Repo 3 Comprehensive Foundation Audit

## Executive result

Repo 3 is a clean-slate native Windows architecture rather than a browser migration of Repo 2. The Foundation branch contains the engineering controls required to prevent the main classes of Repo 2 drift.

Static structure and governance are now aligned with the recorded lessons. Final Foundation certification still requires real Windows/PostgreSQL/runtime evidence; this document never treats static structure as runtime proof.

## Repo 2 failure → Repo 3 control

| Repo 2 problem | Evidence pattern | Repo 3 control | Current Foundation state |
|---|---|---|---|
| Giant Server Program | 262 KB Program.cs with endpoints, DTOs and helpers | composition-only Program; module boundaries; source-size guard | Implemented + guard |
| Giant CI workflow | 237 KB manual workflow became a second program | one thin workflow + canonical scripts/guards | Implemented + guard |
| Browser became second product | 79 Dashboard files, React/Vite/package-lock and recurring JSX/TS fixes | native WPF WinExe; browser artifact ban | Implemented + guard |
| Contract/model drift | Station/customer/buffet model-alignment fixes | Shared V1 contract authority + no duplicate client DTOs | Implemented + contract tests |
| SQLite translation mismatch | repeated DateTimeOffset query fixes | PostgreSQL authoritative store from foundation | Implemented; real PG certification pending |
| Migration churn | many migrations while features changed | migration policy + controlled bundle/SQL artifact + startup compatibility gate | Implemented; runtime certification pending |
| Runtime production migration | Repo 2 startup/model initialization coupled to migration behavior | Server never silently migrates Production; deployment owns schema change | Implemented |
| Session ownership added late | many ownership DB guards/fixes in Stage 12 | Device/session authority and fencing are Foundation concerns | Architecture implemented; Session business slice deferred |
| Agent ownership/reconnect fixes | pairing, process ownership and Session start fixes | durable DeviceId + ConnectionId + persistent LeaseToken + SignalR + heartbeat/reconnect | Implemented; local Agent proof pending |
| Mock/server drift | mock product/inventory alignment fixes | server source of truth + contract tests + explicit fake boundary | Implemented |
| Preview became a second runtime path | many preview-specific fixes and demo data patches | native executable is the runtime surface | Implemented; browser runtime forbidden |
| Installer drift | repeated runtime/installer/registration fixes | separate components, manifest, migration/update compatibility | Implemented foundation; clean-machine proof pending |
| Payment/free-credit semantics drift | later payment/report corrections | money primitive + server-owned billing rules + traceability/acceptance before feature code | Foundation rules implemented; billing deferred |
| Large-file responsibility creep | large services/DbContext/Hub | size thresholds, module/layer ownership, composition guards | Implemented + guard |

## Engineering controls added from external research

- ADRs for decisions with durable consequences.
- Architecture fitness guards executed locally.
- Version-controlled database/deployment artifacts.
- Migration bundle plus reviewable SQL generation.
- Security design review before trust-boundary changes.
- Central dependency pinning and supply-chain guard.
- SBOM generation path for release evidence.
- Native WPF Generic Host for DI/config/logging/lifetime.
- SignalR transport with explicit Server-side fencing/reconciliation.
- User-oriented reliability targets rather than a promise of 100% uptime.

## Foundation boundary

Business modules remain empty by design. The following are not bugs in Foundation:
- customer/station/session/billing/inventory business rules;
- tariff engine details;
- buffet/inventory business workflows;
- external payment-provider integration;
- full operator screens beyond the platform shell;
- production-scale business load evidence that depends on real business workflows.

## Policy decisions

The current release baseline is explicitly recorded in docs/planning/assumptions-and-open-decisions.md:

- scale baseline accepted;
- RPO/RTO baseline accepted;
- verified-backup retention baseline accepted;
- Agent transport accepted as authenticated SignalR;
- update-signing ownership accepted as organization-controlled signing with SHA-256 verification;
- first-release multi-shop/multi-tenant scope explicitly out;
- reporting baseline accepted as read-only with ADR-based expansion if measured need exists;
- fa-IR first-class and en-US supported;
- external payment-provider integration explicitly deferred from Foundation and must be resolved before the affected Business slice reaches Definition of Ready.

These are not runtime proof. Evidence must still be produced for the relevant implementation claims.

## Final remaining gates

Only these categories remain before the phrase Foundation Certified can truthfully be used:

1. canonical verification on the approved Windows machine;
2. native Desktop launch/localization certification;
3. real PostgreSQL migration, concurrency, backup and isolated-restore certification;
4. authenticated Agent connection, heartbeat, reconnect and fencing proof on real Windows;
5. Desktop-to-Server real smoke;
6. install/update/rollback and recovery smoke on target Windows machines;
7. release migration artifact + SBOM generation/review evidence;
8. final exact-commit evidence pack and closure-matrix review.

Until these gates are closed, Repo 3 is not called 100% certified and no business vertical slice is started.