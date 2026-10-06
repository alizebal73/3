# CI Foundation Gates

The single CI workflow must enforce:
1. toolchain baseline
2. architecture guard
3. source-size guard
4. dotnet tool restore
5. restore
6. release build
7. tests
8. deterministic Dashboard install with npm ci
9. Dashboard typecheck
10. Dashboard production build

A green CI result is necessary but not sufficient for feature completion. Business features also need integration, concurrency, retry/idempotency, authorization, audit and recovery coverage.
