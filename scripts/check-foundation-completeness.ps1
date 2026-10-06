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
    "src\Client\Agent\AgentIdentity.cs",
    "src\Desktop\GameNet.Desktop.csproj",
    "src\Desktop\App.xaml",
    "src\Desktop\App.xaml.cs",
    "src\Desktop\Shell\MainWindow.xaml",
    "src\Desktop\Shell\MainWindow.xaml.cs",
    "src\Desktop\Localization\LanguageService.cs",
    "src\Desktop\Resources\Languages\Strings.fa-IR.xaml",
    "src\Desktop\Resources\Languages\Strings.en-US.xaml",
    "tests\Desktop.Tests\LocalizationFoundationTests.cs"
)

foreach ($relativePath in $requiredFiles) {
    $path = Join-Path $root $relativePath
    if (-not (Test-Path $path)) {
        throw "Required Foundation implementation is missing: $relativePath"
    }
}

$placeholderTests = Get-ChildItem (Join-Path $root "tests") -Recurse -File -Filter *.cs |
    Select-String -Pattern "Assert\.True\(true\)"

if ($placeholderTests) {
    $placeholderTests | ForEach-Object {
        Write-Host "FOUNDATION TEST PLACEHOLDER: $($_.Path):$($_.LineNumber)"
    }
    throw "Placeholder tests are not accepted as Foundation evidence."
}

if (Test-Path (Join-Path $root ".github\workflows")) {
    throw "GitHub Actions workflows are forbidden. GameNet 3 is certified locally only."
}

Write-Host "Foundation completeness guard passed."
