# GameNet Manager 3

Clean-slate rewrite of the GameNet/CyberCafe management platform.

Principles:
- Server is the single source of truth.
- Domain/application logic stays outside HTTP/UI code.
- Business modules own their rules, contracts, persistence boundary, and tests.
- Concurrency, authorization, identity, audit, and financial invariants are first-class.
- main is always releasable; develop is the integration branch.

Architecture: docs/architecture/README.md
Branching: docs/development/branching.md
Foundation roadmap: docs/roadmap/00-platform-foundation.md
