# Testing Policy

All authoritative test runs happen on the approved Windows machine.

The normal command is:

scripts/verify.ps1

The optional manual self-hosted workflow executes the same local script on the approved runner. It does not create a second test definition.

Before business features:
- pre-coding governance must pass;
- platform skeleton must pass;
- architecture guard must pass;
- solution must restore/build/test locally;
- native Desktop project must build and launch;
- both UI cultures must be loadable;
- PostgreSQL certification must pass when database-backed tests exist;
- platform recovery/update evidence must exist.

A remote green check never substitutes for local evidence.
