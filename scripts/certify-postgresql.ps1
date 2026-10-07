$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-Checked {
    param([string]$FilePath, [string[]]$Arguments)
    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$FilePath failed with exit code $LASTEXITCODE." }
}

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) { throw "Set GAMENET_DATABASE to a disposable clean PostgreSQL certification database." }
if ([string]::IsNullOrWhiteSpace($env:GAMENET_RESTORE_DATABASE)) { throw "Set GAMENET_RESTORE_DATABASE to a separate disposable PostgreSQL restore target." }
if ([string]::IsNullOrWhiteSpace($env:GAMENET_UPGRADE_DATABASE)) { throw "Set GAMENET_UPGRADE_DATABASE to a separate disposable PostgreSQL upgrade-path certification database." }

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$backupDir = Join-Path $root "artifacts\postgresql"
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
$backupFile = Join-Path $backupDir ("foundation-backup-{0}.dump" -f (Get-Date -Format "yyyyMMdd-HHmmss"))
$env:GAMENET_TEST_DATABASE = $env:GAMENET_DATABASE
$env:GameNet__DatabaseConnectionString = $env:GAMENET_DATABASE

Invoke-Checked "dotnet" @("tool","restore")

# Fail closed if the built application does not expose the expected migration chain.
$migrations = & dotnet ef migrations list "--project","src\Server\GameNet.Server.csproj" "--startup-project","src\Server\GameNet.Server.csproj" "--configuration","Release" 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) { throw "Unable to enumerate EF migrations.`n$migrations" }
if ($migrations -notmatch "202610070001_FoundationInfrastructure" -or
    $migrations -notmatch "202610070002_FoundationClosureHardening" -or
    $migrations -notmatch "202610070003_AgentCredentialLifecycle") {
    throw "Expected Foundation EF migration chain was not discovered.`n$migrations"
}

# Clean-install evidence: the latest migration must apply from an empty database.
Invoke-Checked "dotnet" @(
    "ef","database","update",
    "--project","src\Server\GameNet.Server.csproj",
    "--startup-project","src\Server\GameNet.Server.csproj",
    "--connection",$env:GAMENET_DATABASE,
    "--configuration","Release"
)

# Restore the certification test project explicitly so local certification does not
# depend on a previous unrelated build populating obj/project.assets.json.
Invoke-Checked "dotnet" @(
    "restore",
    "tests\Postgres.CertificationTests\GameNet.Postgres.CertificationTests.csproj"
)

Invoke-Checked "dotnet" @(
    "test",
    "tests\Postgres.CertificationTests\GameNet.Postgres.CertificationTests.csproj",
    "--configuration","Release",
    "--no-restore"
)

# Upgrade evidence: establish a previous supported schema, then upgrade that same
# database through the current migration chain without recreating it.
Invoke-Checked "dotnet" @(
    "ef","database","update","202610070001_FoundationInfrastructure",
    "--project","src\Server\GameNet.Server.csproj",
    "--startup-project","src\Server\GameNet.Server.csproj",
    "--connection",$env:GAMENET_UPGRADE_DATABASE,
    "--configuration","Release"
)

Invoke-Checked "dotnet" @(
    "ef","database","update",
    "--project","src\Server\GameNet.Server.csproj",
    "--startup-project","src\Server\GameNet.Server.csproj",
    "--connection",$env:GAMENET_UPGRADE_DATABASE,
    "--configuration","Release"
)

$psql = (Get-Command psql -ErrorAction Stop).Source
$pending = & $psql $env:GAMENET_UPGRADE_DATABASE -tAc "select count(*) from information_schema.tables where table_schema = 'public' and table_name = '__EFMigrationsHistory';" 2>&1
if ($LASTEXITCODE -ne 0 -or ($pending | Out-String).Trim() -ne "1") {
    throw "Upgrade verification could not confirm EF migration history."
}

$latestMigration = & $psql $env:GAMENET_UPGRADE_DATABASE -tAc "select migration_id from `"__EFMigrationsHistory`" order by migration_id desc limit 1;" 2>&1
if ($LASTEXITCODE -ne 0 -or ($latestMigration | Out-String).Trim() -ne "202610070003_AgentCredentialLifecycle") {
    throw "Upgrade verification did not reach the expected latest migration."
}

$pgDump = (Get-Command pg_dump -ErrorAction Stop).Source
$pgRestore = (Get-Command pg_restore -ErrorAction Stop).Source

Invoke-Checked $pgDump @("--format=custom","--file=$backupFile",$env:GAMENET_DATABASE)
if (-not (Test-Path $backupFile -PathType Leaf)) { throw "pg_dump did not create the backup file." }
if ((Get-Item $backupFile).Length -le 0) { throw "Backup file is empty." }

Invoke-Checked $pgRestore @("--no-owner","--no-privileges","--dbname=$($env:GAMENET_RESTORE_DATABASE)",$backupFile)
$probe = & $psql $env:GAMENET_RESTORE_DATABASE -tAc "select 1;" 2>&1
if ($LASTEXITCODE -ne 0 -or ($probe | Out-String).Trim() -ne "1") { throw "Isolated restore verification failed." }

Write-Host "REAL POSTGRESQL FOUNDATION CERTIFICATION PASSED. Clean + upgrade + backup/restore evidence succeeded. Backup: $backupFile"
