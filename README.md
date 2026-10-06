# GameNet Manager 3

Clean-slate rewrite of the GameNet/CyberCafe management platform.

Principles:
- Server is the single source of truth.
- Domain/application logic stays outside HTTP/UI code.
- Business modules own their rules, contracts, persistence boundary and tests.
- Concurrency, authorization, identity, audit and financial invariants are first-class.
- main is always releasable; develop is the integration branch.
- No business feature starts until Stage 0 Foundation is externally certified.

Start with:
- docs/architecture/README.md
- docs/architecture/module-rules.md
- docs/domain/product-requirements.md
- docs/invariants/
- docs/roadmap/00-platform-foundation.md
- docs/roadmap/00-foundation-completion.md
- docs/operations/foundation-external-gates.md
