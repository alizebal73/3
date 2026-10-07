# Security Foundation / Threat Model

Trust boundaries:
- Operator/Admin using Desktop -> Server API
- Client Agent -> Server realtime/API
- Server -> PostgreSQL
- Server -> filesystem/DataRoot
- Update package -> installed binaries

Rules:
- Device identity is explicit. IP is never identity or authorization.
- Agent credentials are durable, rotatable and revocable.
- Pairing is authenticated and rate-limited.
- Customer Login binds to an explicit Customer plus Agent Device.
- Sensitive mutations require server-side permission checks.
- JWT validation checks issuer, audience, lifetime and signing key when enabled.
- Secrets never live in source control.
- Forwarded headers are trusted only for explicitly configured proxies.
- Login, pairing, payment, reversal, transfer and privileged settings mutations are auditable.
- Replay protection and idempotency are required for money, inventory and Session mutations.
- Update packages must pass integrity verification before installation.
