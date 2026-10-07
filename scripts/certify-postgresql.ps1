$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-Checked {
    param([string]$FilePath, [string[]]$Arguments)
    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$FilePath failed with exit code $LASTEXITCODE." }
}

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) { throw "Set GAMENET_DATABASE to a disposable clean PostgreSQL certification database." }
if ([string]::IsNullOrWhiteSpace($env:GAMENET_RESTORE_DATABASE)) { throw "Set GAMENET_RESTORE_DATABASE to a separate disposable PostgreSQL restore target." }

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$backupDir = Join-Path $root "artifacts\postgresql"
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
$backupFile = Join-Path $backupDir ("foundation-backup-{0}.dump" -f (Get-Date -Format "yyyyMMdd-HHmmss"))
$env:GAMENET_TEST_DATABASE = $env:GAMENET_DATABASE

Invoke-Checked "dotnet" @("tool","restore")
Invoke-Checked "dotnet" @("ef","database","update","--project","src\Server\GameNet.Server.csproj","--startup-project","src\Server\GameNet.Server.csproj","--connection",$env:GAMENET_DATABASE,"--configuration","Release")

Invoke-Checked "dotnet" @("test","tests\Postgres.CertificationTests\GameNet.Postgres.CertificationTests.csproj","--configuration","Release","--no-restore")

$pgDump = (Get-Command pg_dump -ErrorAction Stop).Source
$pgRestore = (Get-Command pg_restore -ErrorAction Stop).Source
$psql = (Get-Command psql -ErrorAction Stop).Source

Invoke-Checked $pgDump @("--format=custom","--file=$backupFile",$env:GAMENET_DATABASE)
if (-not (Test-Path $backupFile -PathType Leaf)) { throw "pg_dump did not create the backup file." }
if ((Get-Item $backupFile).Length -le 0) { throw "Backup file is empty." }

Invoke-Checked $pgRestore @("--no-owner","--no-privileges","--dbname=$($env:GAMENET_RESTORE_DATABASE)",$backupFile)
$probe = & $psql $env:GAMENET_RESTORE_DATABASE -tAc "select 1;" 2>&1
if ($LASTEXITCODE -ne 0 -or ($probe | Out-String).Trim() -ne "1") { throw "Isolated restore verification failed." }

Write-Host "REAL POSTGRESQL FOUNDATION CERTIFICATION PASSED. Backup: $backupFile"
