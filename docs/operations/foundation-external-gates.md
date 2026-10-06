# Foundation External Gates

These items require the approved Windows build environment or GitHub repository settings.

## Repo 3 runner

Runner requirements:
- self-hosted
- Windows
- X64
- .NET 10 SDK matching global.json
- Node 22.12+ matching .nvmrc/engines
- PostgreSQL client/server access for integration work

The runner must be connected to repository 3, not repository 2.

## Dashboard lockfile

Run scripts/bootstrap-dashboard-lock.ps1 on the approved build runner, review package-lock.json, commit it, and keep CI on npm ci.

## PostgreSQL

Set GAMENET_DATABASE_CONNECTION on the build/test environment before generating migrations or running database-backed integration tests.

Use scripts/create-migration.ps1 for reviewed EF migrations.

## GitHub protection

Activate branch protection/rules on main and develop using docs/operations/github-settings.md.

## Certification

The final foundation commit is certified only after the exact commit passes the complete CI workflow on the repo 3 runner.
