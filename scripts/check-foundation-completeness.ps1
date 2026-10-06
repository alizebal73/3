$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."

$requiredFiles = @(
    "src\Server\Infrastructure\Security\AuthExtensions.cs",
    "src\Server\Infrastructure\Security\CurrentActor.cs",
    "src\Server\Infrastructure\Observability\CorrelationIdMiddleware.cs",
    "src\Server\Infrastructure\Observability\ExceptionHandlingMiddleware.cs",
    "src\Server\Infrastructure\Outbox\OutboxMessage.cs",
    "src\Server\Infrastructure\Outbox\IOutboxWriter.cs",
    "src\Server\Infrastructure\Outbox\EfOutboxWriter.cs",
    "src\Server\Persistence\Configurations\OutboxMessageConfiguration.cs",
    "src\Client\Agent\AgentIdentity.cs"
)

foreach ($relativePath in $requiredFiles) {
    $path = Join-Path $root $relativePath
    if (-not (Test-Path $path)) {
        throw "Required Foundation implementation is missing: $relativePath"
    }
}

$workflowDirectory = Join-Path $root ".github\workflows"
$workflowFiles = @(Get-ChildItem $workflowDirectory -File -Include *.yml,*.yaml)
if ($workflowFiles.Count -ne 1) {
    throw "Foundation requires exactly one GitHub Actions workflow; found $($workflowFiles.Count)."
}

$placeholderTests = Get-ChildItem (Join-Path $root "tests") -Recurse -File -Filter *.cs |
    Select-String -Pattern "Assert\.True\(true\)"

if ($placeholderTests) {
    $placeholderTests | ForEach-Object {
        Write-Host "FOUNDATION TEST PLACEHOLDER: $($_.Path):$($_.LineNumber)"
    }
    throw "Placeholder tests are not accepted as Foundation evidence."
}

Write-Host "Foundation completeness guard passed."
