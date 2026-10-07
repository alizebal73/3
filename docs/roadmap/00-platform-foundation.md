# Stage 0 — Platform Foundation

Stage 0 has two gates:

1. Pre-Coding Governance.
2. Foundation Platform Certification.

## Pre-Coding Governance

Required:
- product charter and non-goals;
- requirements traceability;
- module/data ownership;
- state machines and invariants;
- architecture decisions;
- runtime topology;
- security/data classification;
- quality/testing strategy;
- scale/capacity/SLO envelope;
- environment matrix;
- database change policy;
- backup/recovery/update policy;
- operator workflow/product validation;
- Definition of Ready/Done;
- risk and assumptions registers;
- release gates and templates.

Exit:
No major business capability is allowed to start with an undocumented ownership, contract, failure, recovery or acceptance question.

## Foundation Platform Certification

Required:
1. Shared IDs, Money, Time and Result/Error primitives.
2. Versioned API contracts.
3. PostgreSQL provider and DbContext boundary.
4. Transaction, concurrency and idempotency infrastructure.
5. Authentication and authorization foundation.
6. Audit infrastructure.
7. Agent protocol, acknowledgement and reconciliation contracts.
8. Recovery and reconciliation rules.
9. Background-job boundary.
10. Observability and correlation IDs.
11. Configuration and secret provisioning rules.
12. Test infrastructure.
13. Architecture and platform-skeleton enforcement.
14. Native Windows Desktop application foundation.
15. Persian/English localization foundation.
16. Local build/test/certification process.
17. Release compatibility and rollback foundation.

## Platform-proof exit

Before the first vertical slice:
- native WPF Desktop builds and launches locally;
- both fa-IR and en-US resources load;
- architecture/platform/governance guards pass;
- solution restore/build/test passes locally;
- real PostgreSQL migration/concurrency lane passes;
- Agent transport/lease/reconciliation proof passes;
- Desktop-to-Server smoke passes;
- update/rollback smoke passes;
- recovery/restore smoke passes;
- no fake-green placeholder tests;
- no retired browser operator UI remains.

Only after these gates may the first business vertical slice start.
