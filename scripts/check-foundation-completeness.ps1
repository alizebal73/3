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
    "tests\Desktop.Tests\LocalizationFoundationTests.cs",
    "tests\E2E\GameNet.E2E.Tests.csproj",
    "docs\architecture\module-boundary-manifest.md",
    "docs\domain\game-module-blueprints.md",
    "docs\architecture\actor-and-auth-model.md",
    "docs\contracts\api-envelope.md",
    "docs\contracts\release-compatibility.md",
    "docs\operations\deployment-and-update.md",
    "docs\operations\foundation-certification.md"
)

foreach ($relativePath in $requiredFiles) {
    $path = Join-Path $root $relativePath
    if (-not (Test-Path $path)) {
        throw "Required Foundation implementation/document is missing: $relativePath"
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

$forbiddenRuntimeWords = @(
    "src\Dashboard",
    "package-lock.json",
    ".nvmrc",
    ".github\workflows"
)

foreach ($relativePath in $forbiddenRuntimeWords) {
    if (Test-Path (Join-Path $root $relativePath)) {
        throw "Forbidden browser/remote-CI artifact exists: $relativePath"
    }
}

$docRoots = @(
    Join-Path $root "docs\architecture",
    Join-Path $root "docs\security",
    Join-Path $root "docs\contracts"
)

foreach ($docRoot in $docRoots) {
    $stale = Get-ChildItem $docRoot -Recurse -File -Include *.md |
        Select-String -Pattern "\bDashboard\b|React|Vite|WebView"
    if ($stale) {
        $stale | ForEach-Object {
            Write-Host "STALE ARCHITECTURE DOC: $($_.Path):$($_.LineNumber): $($_.Line.Trim())"
        }
        throw "Active architecture/security/contract documentation contains retired web-runtime references."
    }
}

Write-Host "Foundation completeness guard passed."
