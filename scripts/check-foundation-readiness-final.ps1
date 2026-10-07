$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$matrix = Join-Path $root "docs\architecture\foundation-closure-matrix.md"

if (-not (Test-Path $matrix)) {
    throw "Foundation closure matrix is missing."
}

$text = Get-Content $matrix -Raw

if ($text -match "Agent SignalR transport.*contract-only") {
    throw "Foundation closure matrix still treats the selected Agent transport as contract-only."
}

$requiredRuntimePaths = @(
    "src\Desktop\Infrastructure\Hosting\DesktopHost.cs",
    "src\Client\Transport\SignalRAgentTransport.cs",
    "src\Server\Infrastructure\Realtime\AgentHub.cs",
    "src\Server\Infrastructure\Backup\PostgresBackupStore.cs",
    "src\Shared\Primitives\ReleasePackageVerifier.cs"
)

foreach ($relative in $requiredRuntimePaths) {
    if (-not (Test-Path (Join-Path $root $relative))) {
        throw "Foundation runtime path is missing: $relative"
    }
}

Write-Host "Foundation final-readiness structure check passed."