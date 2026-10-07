# Local Quality Gates

The canonical engineering gate runs on the approved Windows machine.

The canonical command is:

scripts/verify.ps1

A single manual GitHub Actions workflow may invoke the same script on the approved self-hosted Windows runner. That workflow is orchestration only; it is not a separate build/test implementation and its result never replaces local certification evidence.

Quality order:

1. pre-coding governance;
2. platform skeleton;
3. architecture guard;
4. source-size guard;
5. Foundation completeness;
6. .NET restore;
7. Release build;
8. all solution tests;
9. native Desktop certification;
10. real PostgreSQL certification;
11. recovery/update evidence.

A remote checkmark never overrides a local failing test or missing certification evidence.
