param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$MigrationName
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) {
    throw "Set GAMENET_DATABASE before creating a migration."
}

$project = Join-Path $PSScriptRoot "..\src\Server\GameNet.Server.csproj"
dotnet ef migrations add $MigrationName --project $project --startup-project $project --output-dir "Persistence\Migrations"
