param(
    [Parameter(Mandatory = $true)]
    [string]$PackageRoot,

    [Parameter(Mandatory = $true)]
    [string]$InstallRoot,

    [Parameter(Mandatory = $true)]
    [string]$CurrentManifestPath,

    [string]$ExpectedPublisher
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$apply = Join-Path $root "scripts\apply-local-update.ps1"
$rollback = Join-Path $root "scripts\rollback-local-package.ps1"

if (-not (Test-Path $PackageRoot -PathType Container)) { throw "Package root does not exist: $PackageRoot" }
if (-not (Test-Path $CurrentManifestPath -PathType Leaf)) { throw "Current manifest does not exist: $CurrentManifestPath" }

$install = [System.IO.Path]::GetFullPath($InstallRoot)
$parent = Split-Path -Parent $install
$pointer = Join-Path $parent ".gamenet-rollback-Desktop.json"

& $apply -PackageRoot $PackageRoot -InstallRoot $install -Component Desktop -CurrentManifestPath $CurrentManifestPath -ExpectedPublisher $ExpectedPublisher
if ($LASTEXITCODE -ne 0) { throw "Local update command failed." }

if (-not (Test-Path $pointer -PathType Leaf)) { throw "Successful update did not leave a rollback point: $pointer" }

$state = Get-Content $pointer -Raw | ConvertFrom-Json
if ([string]::IsNullOrWhiteSpace($state.BackupPath) -or -not (Test-Path $state.BackupPath -PathType Container)) { throw "Rollback point is invalid: $($state.BackupPath)" }

& $rollback -InstallRoot $install -RollbackRoot $state.BackupPath
if ($LASTEXITCODE -ne 0) { throw "Rollback command failed." }
if (-not (Test-Path $install -PathType Container)) { throw "Install root is missing after rollback: $install" }

Write-Host "LOCAL UPDATE + ROLLBACK CERTIFICATION PASSED."
