$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

& "$PSScriptRoot/verify.ps1"
& "$PSScriptRoot/certify-desktop.ps1"

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) {
    throw "Set GAMENET_DATABASE before running full Foundation certification."
}

& "$PSScriptRoot/certify-postgresql.ps1"

Write-Host "FOUNDATION CERTIFICATION COMMANDS COMPLETED. Review runtime evidence before allowing feature work."
