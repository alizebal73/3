# Architecture Baseline

GameNet 3 is a modular monolith.

Runtime surfaces:
1. Server — authoritative source of truth, business rules, persistence, authorization, audit, and real-time commands.
2. Dashboard — operator/admin UI; it never owns authoritative state.
3. Client Agent — machine-side executor; it executes server-authorized commands and reports health.

Every business module owns Domain, Application, Infrastructure, Api, and tests.

Critical invariants:
- one active session per station
- explicit Agent identity; IP is never the security identity
- Session ownership follows the active station/Agent
- money operations are transactional and idempotent
- inventory reversal preserves stock-area provenance
- every mutation is server-authorized
- sensitive mutations are audited
- concurrent state transitions are atomic

Prefer focused files under 400 lines. A file above 600 lines requires review.
