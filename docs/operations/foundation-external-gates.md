# Foundation Certification Gates

There is no GitHub Actions execution dependency for GameNet 3.

GitHub is used for source control, review and history only. Build, restore, test, migration generation and certification are performed on the approved Windows machine.

## Required local environment

- Windows x64.
- .NET 10 SDK matching global.json.
- PostgreSQL access for database integration certification.
- WPF desktop build support.

## Local certification

Run:

scripts/verify.ps1

This verifies:
1. platform/project skeleton;
2. architecture boundaries;
3. source-size limits;
4. Foundation completeness;
5. .NET restore;
6. Release build;
7. all tests;
8. desktop build.

## PostgreSQL certification

Before the first business vertical slice:
- configure GAMENET_DATABASE_CONNECTION on the local build/test machine;
- generate the first migration with scripts/create-migration.ps1;
- review the migration;
- apply it to a clean PostgreSQL database;
- execute database-backed integration tests and concurrency tests.

## Desktop certification

The local machine must produce a native Windows executable from src/Desktop/GameNet.Desktop.csproj.

The application must launch as a normal Windows process and must not require a browser.

Both fa-IR and en-US must load from application resources.

## Release gate

The Foundation is not certified until the exact final commit passes scripts/verify.ps1 on the approved local Windows machine and the PostgreSQL certification is complete.

Until then, no business feature may start.
