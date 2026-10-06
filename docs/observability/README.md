# Observability

Every request and command must be traceable through:
- correlation ID
- structured logs
- actor identity
- device identity when applicable
- operation/reference ID
- audit record for sensitive mutations

Health distinguishes liveness from readiness.

Logs must avoid secrets, passwords, access tokens and full sensitive payloads.

Operator-facing errors use stable error codes plus correlation/trace IDs.
