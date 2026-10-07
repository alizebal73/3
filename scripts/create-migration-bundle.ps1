param(
    [Parameter(Mandatory = $true)]
    [string]$OutputPath = ".\\artifacts\\database\\GameNet.Migrations.exe"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$project = Join-Path $PSScriptRoot "..\\src\\Server\\GameNet.Server.csproj"
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
$directory = Split-Path -Parent $OutputPath
if ($directory) { New-Item -ItemType Directory -Path $directory -Force | Out-Null }

dotnet ef migrations bundle `
    --project $project `
    --startup-project $project `
    --configuration Release `
    --self-contained `
    --runtime win-x64 `
    --output $OutputPath

if ($LASTEXITCODE -ne 0) {
    throw "dotnet ef migrations bundle failed with exit code ${LASTEXITCODE}."
}

Write-Host "Migration deployment bundle created: $OutputPath"
