param(
    [Parameter(Mandatory = $true)]
    [string]$BuildDrop,

    [Parameter(Mandatory = $true)]
    [string]$OutputPackage,

    [Parameter(Mandatory = $true)]
    [string]$ProductVersion,

    [Parameter(Mandatory = $true)]
    [int]$SchemaVersion,

    [Parameter(Mandatory = $true)]
    [string]$ApiContractVersion,

    [Parameter(Mandatory = $true)]
    [int]$AgentProtocolVersion,

    [Parameter(Mandatory = $true)]
    [int]$MigrationFromSchemaVersion,

    [Parameter(Mandatory = $true)]
    [int]$MigrationToSchemaVersion,

    [Parameter(Mandatory = $true)]
    [string]$CertificateThumbprint,

    [string]$TimestampServer
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$build = (Resolve-Path (Join-Path $root $BuildDrop)).Path
$package = [System.IO.Path]::GetFullPath((Join-Path $root $OutputPackage))

if (Test-Path $package) {
    Remove-Item $package -Recurse -Force
}
New-Item -ItemType Directory -Path $package -Force | Out-Null
Copy-Item (Join-Path $build "*") $package -Recurse -Force

& (Join-Path $PSScriptRoot "sign-release-artifacts.ps1") `
    -Root $package `
    -CertificateThumbprint $CertificateThumbprint `
    -TimestampServer $TimestampServer

& (Join-Path $PSScriptRoot "create-release-manifest.ps1") `
    -PackageRoot $package `
    -ProductVersion $ProductVersion `
    -SchemaVersion $SchemaVersion `
    -ApiContractVersion $ApiContractVersion `
    -AgentProtocolVersion $AgentProtocolVersion `
    -MigrationFromSchemaVersion $MigrationFromSchemaVersion `
    -MigrationToSchemaVersion $MigrationToSchemaVersion `
    -OutputPath (Join-Path $package "manifest.json")

& (Join-Path $PSScriptRoot "verify-release-signatures.ps1") `
    -Root $package

Write-Host "Release package prepared: $package"
