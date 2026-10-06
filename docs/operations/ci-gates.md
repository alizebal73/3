# CI gates

Foundation gate:
1. Restore
2. Build with warnings as errors
3. Unit tests
4. Integration tests
5. Contract tests
6. Dashboard typecheck/build

E2E and installer smoke are explicit later gates and are not represented as fake passing steps before their real harness exists.
