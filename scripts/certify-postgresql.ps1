$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-CheckedDotnet {
    param([string[]]$Arguments)
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed with exit code \${LASTEXITCODE}: dotnet $($Arguments -join ' ')"
    }
}

if ([string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE)) {
    throw "Set GAMENET_DATABASE to the clean PostgreSQL connection string."
}

if ([string]::IsNullOrWhiteSpace($env:GAMENET_RESTORE_DATABASE)) {
    throw "Set GAMENET_RESTORE_DATABASE to an isolated disposable PostgreSQL connection string."
}

$env:GAMENET_TEST_DATABASE = $env:GAMENET_DATABASE

Invoke-CheckedDotnet -Arguments @("tool", "restore")
Invoke-CheckedDotnet -Arguments @(
    "ef", "database", "update",
    "--project", "src\Server\GameNet.Server.csproj",
    "--connection", $env:GAMENET_DATABASE
)
Invoke-CheckedDotnet -Arguments @(
    "test", "tests\Postgres.CertificationTests\GameNet.Postgres.CertificationTests.csproj",
    "--configuration", "Release",
    "--no-restore"
)

Write-Host "REAL POSTGRESQL FOUNDATION, BACKUP AND RESTORE CERTIFICATION PASSED."
