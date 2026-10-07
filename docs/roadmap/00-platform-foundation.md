# Stage 0 — Platform Foundation

No business feature is started until every Foundation gate below is satisfied.

Required foundation:
1. Product/domain specification and module ownership.
2. Invariants and explicit state machines.
3. Shared IDs, Money, Time and Result/Error primitives.
4. Versioned API contracts.
5. PostgreSQL provider and DbContext boundary.
6. Transaction, concurrency and idempotency infrastructure.
7. Authentication and authorization foundation.
8. Audit infrastructure.
9. Agent protocol, acknowledgement and reconciliation contracts.
10. Recovery and reconciliation rules.
11. Background-job boundary.
12. Observability and correlation IDs.
13. Configuration and secret provisioning rules.
14. Test infrastructure.
15. Architecture and platform-skeleton enforcement.
16. Native Windows Desktop application foundation.
17. Persian/English localization foundation.
18. Local-only build/test/certification process.
19. Release compatibility and rollback foundation.

Exit gates:
- complete source/project/module skeleton;
- native WPF Desktop builds locally;
- both fa-IR and en-US resources load;
- architecture and platform-skeleton guards pass;
- solution restore/build/test passes locally;
- real PostgreSQL integration lane is executable;
- migration strategy is tested on a clean database;
- transaction/idempotency/outbox rules are tested where applicable;
- no fake-green placeholder tests;
- no retired browser operator UI or Node/Vite pipeline remains;
- no GitHub Actions execution dependency.

Only then start the first business vertical slice:
Station -> Customer -> Agent -> Customer Login -> Session -> Timing -> Transfer -> Invoice -> Payment -> Audit.
