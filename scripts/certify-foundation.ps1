$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

& "$PSScriptRoot/verify.ps1"
& "$PSScriptRoot/certify-desktop.ps1"

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) {
    throw "Set GAMENET_DATABASE before running full Foundation certification."
}

& "$PSScriptRoot/certify-postgresql.ps1"

& "$PSScriptRoot/write-foundation-evidence.ps1" -OutputPath (Join-Path (Resolve-Path ".").Path "artifacts\foundation-evidence.json")

Write-Host "FOUNDATION CERTIFICATION COMMANDS COMPLETED. Review the evidence artifact and close the Foundation matrix before allowing feature work."
