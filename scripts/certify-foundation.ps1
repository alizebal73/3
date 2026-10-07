$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$evidenceDir = Join-Path $root "artifacts\foundation"
New-Item -ItemType Directory -Path $evidenceDir -Force | Out-Null

& "$PSScriptRoot/verify.ps1"
& "$PSScriptRoot/certify-desktop.ps1"

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE) -or [string]::IsNullOrWhiteSpace($env:GAMENET_RESTORE_DATABASE)) {
    throw "Set GAMENET_DATABASE and GAMENET_RESTORE_DATABASE before Foundation certification."
}
& "$PSScriptRoot/certify-postgresql.ps1"

if ([string]::IsNullOrWhiteSpace($env:GAMENET_AGENT_BOOTSTRAP_SECRET)) {
    throw "Set GAMENET_AGENT_BOOTSTRAP_SECRET before Agent certification."
}
& "$PSScriptRoot/certify-agent.ps1"

& "$PSScriptRoot/write-foundation-evidence.ps1" -OutputPath (Join-Path $evidenceDir "foundation-evidence.json")
Write-Host "FOUNDATION PLATFORM COMMANDS PASSED. Review evidence before declaring Foundation Certified."
