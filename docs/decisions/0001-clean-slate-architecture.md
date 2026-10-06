# ADR 0001 — Clean-slate modular monolith

Decision: use one deployable Server with strict module boundaries, not microservices.

Reason: GameNet needs strong transactional consistency for sessions, money, inventory, identity, and authorization while staying simple to install and operate on a local server.

Consequences:
- one authoritative transaction boundary
- lower deployment complexity
- module extraction remains possible later if a real operational need appears
