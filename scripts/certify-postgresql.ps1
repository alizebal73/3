$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) {
    throw "Set GAMENET_DATABASE to the clean PostgreSQL connection string."
}

$env:GAMENET_TEST_DATABASE = $env:GAMENET_DATABASE

dotnet tool restore
dotnet ef database update --project src\Server\GameNet.Server.csproj --connection $env:GAMENET_DATABASE
dotnet test tests\Postgres.CertificationTests\GameNet.Postgres.CertificationTests.csproj --configuration Release --no-restore

Write-Host "REAL POSTGRESQL FOUNDATION CERTIFICATION PASSED."
