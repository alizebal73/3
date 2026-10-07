# Local Testing Rule

All authoritative test runs happen on the approved Windows machine.

The normal command is:

scripts/verify.ps1

Do not use GitHub Actions as build/test evidence.

Before business features:
- platform skeleton must pass;
- architecture guard must pass;
- solution must restore/build/test locally;
- native Desktop project must build;
- both UI cultures must be loadable;
- PostgreSQL certification must pass when database-backed tests exist.

A remote green check never overrides a local failing test.
