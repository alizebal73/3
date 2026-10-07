# Foundation Research and Repo 2 Failure Analysis

## Purpose

This document records why Repo 2 became expensive to maintain and which engineering controls Repo 3 must enforce before business feature development.

## Repo 2 findings

The latest Repo 2 tree contains:
- a 262 KB Server Program.cs;
- a 237 KB manual CI workflow;
- a 116 KB Dashboard page plus other large React/Vite UI files;
- a 54 KB Client Program.cs;
- a 52 KB SessionSettlementService.cs;
- a 38 KB DbContext;
- a 35 KB AgentHub;
- dozens of sequential database migrations;
- a 79-file Dashboard/browser runtime.

These are structural signals, not proof that file size alone caused every defect. They show that too many responsibilities accumulated in a few runtime and delivery boundaries.

### Failure pattern 1 — Multiple authorities for the same contract

Repo 2 contains Server records, Dashboard types/services, Agent/client types and API-specific models evolving together. Recent fixes repeatedly repair model alignment, for example:
- Station/model constructor alignment;
- customer request/password fields;
- buffet/product/inventory shapes;
- nullable customer/debt projection mismatches.

The engineering lesson is one authoritative contract boundary plus contract tests, with no duplicate transport models in clients.

### Failure pattern 2 — Browser runtime became a second product

Repo 2 still contains React/Vite/package-lock and a large Dashboard surface. Recent commits repaired JSX blocks, dashboard station grouping, buffet UI blocks and reporting TypeScript/JSX issues.

The lesson is that UI architecture, toolchain and runtime must be decided before features. Repo 3 therefore uses native WPF WinExe as the human-facing application and forbids browser runtime artifacts.

### Failure pattern 3 — Business logic accumulated in composition/runtime files

The Repo 2 Server Program.cs is extremely large and contains endpoint orchestration, DTO declarations, helper functions and infrastructure concerns. This makes ownership, review and test isolation difficult.

Repo 3 keeps Program.cs composition-only and puts business modules behind Domain/Application/Infrastructure/Api boundaries.

### Failure pattern 4 — Database model and migration evolution happened while features were already moving

Repo 2 contains a long sequence of migrations that were added while customer accounts, buffet, sessions, payments, Agent features and reports were still changing. Recent fixes include separating customer pending/debt accounts and preserving/transforming existing buffet stock.

The lesson is not “avoid migrations”; it is to make schema evolution a release artifact, use forward-compatible migration sequencing, test clean and upgraded databases, and review migration SQL before production. Microsoft recommends controlled migration bundles/scripts for production deployment; DORA likewise treats database change management as part of continuous delivery. 

### Failure pattern 5 — CI became a second application

The Repo 2 manual workflow is itself 237 KB and contains:
- toolchain discovery;
- Node setup;
- restore/build/test;
- EF checks;
- Server process startup;
- authentication smoke;
- Agent pairing/connection smoke;
- UI/SignalR checks;
- settings/payment/shift/game/account-pool checks;
- installer/package generation.

The lesson is to keep the canonical engineering behavior in repository scripts and make CI a thin orchestrator. GitHub is not the source of truth for product certification.

### Failure pattern 6 — Deployment/install concerns were coupled to moving application internals

Repo 2 repeatedly changed installer targets, runtime outputs, versioning and Agent registration tokens. Recent commits explicitly mention restoring Server model alignment for installer builds, restoring runtime targets, and restoring Agent registration token behavior.

Repo 3 therefore separates Server/Desktop/Agent installation boundaries and carries explicit Product/Schema/API/Agent compatibility metadata.

### Failure pattern 7 — Platform invariants arrived after business features

Repo 2 accumulated later-stage fixes for Agent pairing, account allocation, session ownership, audit, settings, inventory, reports, payment and recovery.

The engineering lesson is to establish identity, transaction, idempotency, ownership/fencing, audit, time, money, migration and recovery foundations before those business paths are implemented.

## External engineering research

### Architecture decisions and fitness functions

Martin Fowler's March 2026 ADR guidance recommends small decision records containing the decision, context and consequences, with superseding decisions linked rather than silently rewritten. Fowler's fitness-function guidance recommends turning architecture objectives into executable tests so governance shifts left from inspection to continuous enforcement. 

Repo 3 implements ADRs plus architecture/source-size/platform guards.

### Delivery and database change management

DORA recommends keeping deployment scripts/configuration under version control, treating database changes as part of continuous delivery, and using deployment tests/smoke tests. DORA's database-change-management research specifically says database changes should be managed through the same version-control/release process as application changes.

Repo 3 therefore adds controlled migration bundle generation, migration policy, compatibility sequencing and local smoke/certification scripts.

### Production migrations

Microsoft's current EF Core guidance says production teams should inspect and test migrations before production and recommends migration bundles for automated deployment or reviewed SQL scripts when human review is needed. Runtime migration is possible but has trade-offs around permissions, startup behavior and deployment coordination.

Repo 3 therefore does not silently migrate Production on Server startup; startup checks verify that the database is compatible, while deployment owns migration execution.

### Security by design

OWASP's current Secure-by-Design guidance treats security review as a design-time activity and explicitly covers trust zones, service boundaries, versioned contracts, idempotency, data classification, TLS, least privilege and retention. NIST SSDF 1.1 likewise integrates security requirements, risk, design review, secure implementation and vulnerability response throughout the lifecycle.

Repo 3 therefore carries threat modeling, trust boundaries, authorization, data classification, dependency/SBOM rules and evidence requirements before feature work.

### Supply chain

OWASP Top 10:2025 identifies software supply-chain failures as including unmanaged direct/transitive dependencies, unsupported components, weak change management and poor repository/artifact controls.

Repo 3 centrally pins package versions, forbids local package version drift, scans tracked source/configuration for obvious secret material and requires an SBOM/release dependency review.

### Reliability and SLOs

Google SRE recommends user-oriented SLOs and explicitly warns that 100% reliability is not a sensible target. Reliability should be measured against user-visible outcomes and linked to an operational policy.

Repo 3 therefore treats latency/recovery targets as engineering proposals until approved, instead of claiming 100% reliability.

### Native Desktop platform

Microsoft documents using the .NET Generic Host with WPF to obtain dependency injection, configuration, logging and controlled application lifetime. Repo 3 now uses a Generic Host in the native WPF application rather than constructing the main window directly in App.xaml.cs.

### Agent realtime transport

Microsoft's ASP.NET Core SignalR .NET client supports automatic reconnect lifecycle events and initial-start retry patterns. SignalR stateful reconnect is available in current .NET versions. Repo 3 selects SignalR for Agent realtime transport, while retaining explicit Server-side lease/fencing and reconciliation because reconnect itself is not business-state authority.

## Foundation controls derived from the research

1. One authoritative transport contract source.
2. Native Desktop runtime with no browser runtime.
3. Composition files kept small and side-effect free.
4. Domain/Application code isolated from infrastructure.
5. Database changes versioned, reviewable and deployable independently of app startup.
6. Durable idempotency, lease/fencing and audit guarantees.
7. Security requirements captured before implementation.
8. Dependency versions pinned and release provenance traceable.
9. SBOM and vulnerability review before release.
10. Architecture rules executable as guards.
11. Real-environment certification separate from unit-test confidence.
12. Product/feature acceptance tied to operator workflow and recovery behavior.

## Primary external sources consulted

- Martin Fowler, Architecture Decision Records and architectural fitness functions.
- DORA, database change management, deployment automation and version-control guidance.
- Google SRE, SLOs and reliability/error-budget guidance.
- OWASP Secure by Design and OWASP Top 10:2025 Software Supply Chain failures.
- NIST SP 800-218 SSDF v1.1.
- Microsoft EF Core migrations/deployment guidance.
- Microsoft .NET Generic Host / WPF Generic Host guidance.
- Microsoft ASP.NET Core SignalR .NET client/reconnect guidance.
- Microsoft SBOM Tool documentation and current release information.

The current engineering choices are also recorded in the ADRs and operational policies in this repository; research is evidence for the decisions, not a substitute for an approved repository artifact.

## Research status

This document is intentionally a durable engineering record. New external evidence should update the relevant design/ADR/guard rather than becoming an untracked chat conclusion.
