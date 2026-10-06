$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE_CONNECTION)) {
    throw "Set GAMENET_DATABASE_CONNECTION before creating a migration."
}

$project = Join-Path $PSScriptRoot "..\src\Server\GameNet.Server.csproj"
dotnet ef migrations add $args[0] --project $project --startup-project $project --output-dir "Persistence\Migrations"
