# V1 API Envelope Contract

All new HTTP contracts use a consistent envelope model.

## Success
A successful command/query returns contract version, correlation id, optional operation id, payload and authoritative server timestamp.

## Error
Errors expose a stable error code, localized message key, correlation id, optional operation id, validation details and retryability classification.

Standard codes:
validation_error
unauthorized
forbidden
not_found
conflict
concurrency_conflict
idempotency_in_progress
idempotency_replay
rate_limited
dependency_unavailable
internal_error

Raw exception messages, SQL details, stack traces and secrets are never exposed.

## Concurrency
Mutable aggregate responses expose an explicit version/ETag-style value where optimistic concurrency is required. A stale mutation returns concurrency_conflict.

## Headers
X-Correlation-Id
X-Operation-Id
X-Idempotency-Key
X-Contract-Version
ETag / If-Match when required
