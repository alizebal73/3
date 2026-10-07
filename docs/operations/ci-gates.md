# Local Quality Gates

GameNet 3 has no remote CI workflow.

The local quality gate is:

1. platform skeleton;
2. architecture guard;
3. source-size guard;
4. Foundation completeness;
5. .NET restore;
6. Release build;
7. all tests;
8. native Desktop build.

GitHub status is never treated as evidence that the program works.

The approved Windows machine is the only certification environment for build, test and release evidence.
