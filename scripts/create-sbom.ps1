param(
    [Parameter(Mandatory = $true)]
    [string]$BuildDrop,

    [Parameter(Mandatory = $true)]
    [string]$ProductVersion,

    [string]$ProjectPath = ".",
    [string]$ToolPath = ".\tools\sbom-tool-win-x64.exe",
    [string]$OutputDirectory = ".\artifacts\sbom"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$ToolPath = [System.IO.Path]::GetFullPath((Join-Path $root $ToolPath))
$BuildDrop = [System.IO.Path]::GetFullPath((Join-Path $root $BuildDrop))
$ProjectPath = [System.IO.Path]::GetFullPath((Join-Path $root $ProjectPath))
$OutputDirectory = [System.IO.Path]::GetFullPath((Join-Path $root $OutputDirectory))

if (-not (Test-Path $ToolPath -PathType Leaf)) {
    throw "SBOM tool not found: $ToolPath. Provision the pinned tool version in tools/sbom-tool.version."
}
if (-not (Test-Path $BuildDrop -PathType Container)) {
    throw "BuildDrop does not exist: $BuildDrop"
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

& $ToolPath generate `
    -b $BuildDrop `
    -bc $ProjectPath `
    -pn "GameNet Manager 3" `
    -pv $ProductVersion `
    -ps "GameNet" `
    -nsb "https://gamenet.example/sbom/$ProductVersion" `
    -m $OutputDirectory

if ($LASTEXITCODE -ne 0) {
    throw "SBOM generation failed with exit code $LASTEXITCODE."
}

$manifest = Get-ChildItem $OutputDirectory -Recurse -File -Filter "manifest.spdx.json" |
    Select-Object -First 1

if (-not $manifest) {
    throw "SBOM tool completed but no manifest.spdx.json was produced."
}

Write-Host "SBOM created: $($manifest.FullName)"