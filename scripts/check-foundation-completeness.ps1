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
    "src\Server\Infrastructure\Outbox\IOutboxDispatcher.cs",
    "src\Server\Infrastructure\Outbox\EfOutboxDispatcher.cs",
    "src\Server\Persistence\DesignTimeDbContextFactory.cs",
    "src\Server\Persistence\Migrations\202610070001_FoundationInfrastructure.cs",
    "src\Server\Persistence\Migrations\GameNetDbContextModelSnapshot.cs",
    "src\Server\Infrastructure\Backup\IBackupStore.cs",
    "src\Server\Infrastructure\Backup\BackupRestoreRules.cs",
    "src\Server\Infrastructure\Realtime\IAgentConnectionLeaseStore.cs",
    "src\Shared\Contracts\V1\Protocol\AgentConnection.cs",
    "src\Shared\Contracts\V1\System\ReleaseManifest.cs",
    "src\Client\Agent\AgentIdentity.cs",
    "src\Shared\Api\README.md",
    "src\Shared\Errors\README.md",
    "src\Shared\Results\README.md",
    "src\Shared\Primitives\ReleaseCompatibility.cs",
    "src\Shared\Primitives\ReleaseManifestValidator.cs",
    "src\Shared\Contracts\V1\Api\ApiEnvelope.cs",
    "src\Shared\Contracts\V1\Api\ApiError.cs",
    "src\Shared\Contracts\V1\Api\ApiErrorCodes.cs",
    "src\Shared\Contracts\V1\Api\ApiHeaders.cs",
    "src\Shared\Contracts\V1\Api\ContractVersions.cs",
    "src\Desktop\GameNet.Desktop.csproj",
    "src\Desktop\App.xaml",
    "src\Desktop\App.xaml.cs",
    "src\Desktop\Shell\MainWindow.xaml",
    "src\Desktop\Shell\MainWindow.xaml.cs",
    "src\Desktop\Api\IGameNetServerClient.cs",
    "src\Desktop\Api\GameNetServerClient.cs",
    "src\Desktop\Api\ServerConnectionOptions.cs",
    "src\Desktop\Localization\LanguageService.cs",
    "src\Desktop\Resources\Languages\Strings.fa-IR.xaml",
    "src\Desktop\Resources\Languages\Strings.en-US.xaml",
    "tests\Desktop.Tests\LocalizationFoundationTests.cs",
    "tests\Desktop.Tests\GameNetServerClientTests.cs",
    "tests\Server.UnitTests\BackupRestoreRulesTests.cs",
    "tests\Agent.Tests\AgentConnectionContractTests.cs",
    "tests\Shared.Tests\ReleaseCompatibilityTests.cs",
    "tests\Shared.Tests\ReleaseManifestTests.cs",
    "tests\ContractTests\ApiFoundationContractTests.cs",
    "tests\E2E\GameNet.E2E.Tests.csproj",
    "docs\architecture\module-boundary-manifest.md",
    "docs\domain\game-module-blueprints.md",
    "docs\domain\lifecycle-state-machines.md",
    "docs\domain\billing-foundation.md",
    "docs\domain\station-foundation.md",
    "docs\architecture\actor-and-auth-model.md",
    "docs\security\permission-matrix.md",
    "docs\contracts\api-envelope.md",
    "docs\contracts\release-compatibility.md",
    "docs\operations\deployment-and-update.md",
    "docs\operations\backup.md",
    "docs\operations\foundation-local-certification.md",
    "scripts\certify-desktop.ps1",
    "scripts\certify-postgresql.ps1",
    "scripts\certify-foundation.ps1"
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

$runtimeRoots = @(
    "src\Server",
    "src\Client",
    "src\Desktop",
    "src\Installer",
    "deploy"
)

$runtimePlaceholders = foreach ($relativeRoot in $runtimeRoots) {
    $runtimeRoot = Join-Path $root $relativeRoot
    if (Test-Path $runtimeRoot) {
        Get-ChildItem $runtimeRoot -Recurse -File -Include *.cs,*.csproj,*.xaml,*.ps1,*.json,*.md |
            Select-String -Pattern "\bplaceholder\b|\bTODO\b|\bFIXME\b"
    }
}

if ($runtimePlaceholders) {
    $runtimePlaceholders | ForEach-Object {
        Write-Host "FOUNDATION PLACEHOLDER: $($_.Path):$($_.LineNumber): $($_.Line.Trim())"
    }
    throw "Runtime/deployment placeholder markers are not accepted in Foundation."
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
            Write-Host "STALE FOUNDATION DOC: $($_.Path):$($_.LineNumber): $($_.Line.Trim())"
        }
        throw "Active architecture/security/contract documentation contains retired web-runtime references."
    }
}

Write-Host "Foundation completeness guard passed."
