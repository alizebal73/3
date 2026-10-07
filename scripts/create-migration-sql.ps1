param(
    [Parameter(Mandatory = $true)]
    [string]$OutputPath = ".\artifacts\database\GameNet.Migrations.sql"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$project = Join-Path $PSScriptRoot "..\src\Server\GameNet.Server.csproj"
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
$directory = Split-Path -Parent $OutputPath

if ($directory) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

$env:ASPNETCORE_ENVIRONMENT = "Production"

dotnet ef migrations script `
    --project $project `
    --startup-project $project `
    --idempotent `
    --configuration Release `
    --output $OutputPath

if ($LASTEXITCODE -ne 0) {
    throw "dotnet ef migrations script failed with exit code ${LASTEXITCODE}."
}

Write-Host "Idempotent migration SQL created: $OutputPath"