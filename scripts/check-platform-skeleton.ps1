$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot

$requiredPaths = @(
    "global.json",
    ".config\dotnet-tools.json",
    "Directory.Packages.props",
    "GameNet.slnx",
    "src\Server\GameNet.Server.csproj",
    "src\Client\GameNet.Agent.csproj",
    "src\Shared\GameNet.Shared.csproj",
    "src\Desktop\GameNet.Desktop.csproj",
    "tests\Desktop.Tests\GameNet.Desktop.Tests.csproj",
    "tests\E2E\GameNet.E2E.Tests.csproj",
    "src\Desktop\App.xaml",
    "src\Desktop\Shell\MainWindow.xaml",
    "src\Desktop\Localization\LanguageService.cs",
    "src\Desktop\Resources\Languages\Strings.fa-IR.xaml",
    "src\Desktop\Resources\Languages\Strings.en-US.xaml",
    "src\Desktop\Infrastructure\Hosting\DesktopHost.cs",
    "src\Desktop\Resources\Styles\Converters.xaml",
    "src\Desktop\Resources\Styles\DataTemplates.xaml",
    "src\Desktop\Resources\Styles\Controls.xaml",
    "src\Desktop\Resources\Styles\Theme.xaml",
    "src\Desktop\UI\Shell\ShellViewModel.cs",
    "src\Desktop\UI\State\UiState.cs",
    "src\Desktop\UI\Services\ServerConnectivityMonitor.cs",
    "src\Desktop\UI\Commands\UiCommand.cs",
    "src\Desktop\UI\Navigation\NavigationCatalog.cs",
    "src\Desktop\UI\Navigation\NavigationItem.cs",
    "src\Desktop\Infrastructure\Hosting\DesktopOptions.cs",
    "src\Desktop\appsettings.json",
    "src\Client\Transport\IAgentTransport.cs",
    "src\Client\Transport\SignalRAgentTransport.cs",
    "src\Client\Transport\AgentTransportOptions.cs",
    "src\Client\Identity\AgentIdentityOptions.cs",
    "src\Client\Identity\AgentIdentityStore.cs",
    "src\Client\appsettings.json",
    "src\Server\Infrastructure\Realtime\AgentHub.cs",
    "src\Server\Modules\README.md",
    "src\Server\Persistence\Migrations\202610070001_FoundationInfrastructure.cs",
    "src\Server\Persistence\Migrations\GameNetDbContextModelSnapshot.cs",
    "src\Server\appsettings.json"
)

foreach ($relativePath in $requiredPaths) {
    $path = Join-Path $root $relativePath
    if (-not (Test-Path $path)) {
        throw "Required platform skeleton path is missing: $relativePath"
    }
}

$modules = @(
    "Games","GameAccounts","ClientControl",
    "Agents","Approvals","Auth","Backup","Billing","Buffet","Customers",
    "Inventory","Reports","Sessions","Settings","Stations","Tariffs","Users","Vip","Wallet"
)

foreach ($module in $modules) {
    foreach ($layer in @("Api","Application","Domain","Infrastructure")) {
        $path = Join-Path $root "src\Server\Modules\$module\$layer"
        if (-not (Test-Path $path)) {
            throw "Required module layer is missing: $module/$layer"
        }
    }
}

$desktopFeatures = @(
    "Home","Agents","Stations","Customers","Sessions","Billing","Wallet",
    "Inventory","Buffet","Tariffs","Vip","Reports","Settings","Users",
    "Approvals","Backup","Audit"
)

foreach ($feature in $desktopFeatures) {
    $path = Join-Path $root "src\Desktop\Features\$feature"
    if (-not (Test-Path $path)) {
        throw "Required Desktop feature folder is missing: $feature"
    }
}

$forbiddenArtifacts = @(
    "src\Dashboard",
    "package-lock.json",
    ".nvmrc"
)

foreach ($relativePath in $forbiddenArtifacts) {
    if (Test-Path (Join-Path $root $relativePath)) {
        throw "Browser/remote-CI artifact is forbidden by the foundation: $relativePath"
    }
}

$moduleBusinessFiles = Get-ChildItem (Join-Path $root "src\Server\Modules") -Recurse -File -Include *.cs |
    Where-Object { $_.Name -notlike "*.Designer.cs" }

if ($moduleBusinessFiles.Count -gt 0) {
    $moduleBusinessFiles | ForEach-Object {
        Write-Host "FOUNDATION BUSINESS FILE: $($_.FullName)"
    }
    throw "Business implementation appeared before Foundation certification."
}

$projects = Get-ChildItem (Join-Path $root "src") -Recurse -File -Filter *.csproj
foreach ($project in $projects) {
    $text = Get-Content -Raw $project.FullName
    if ($project.FullName -like "*\Desktop\*") {
        if ($text -notmatch "<UseWPF>true</UseWPF>") {
            throw "Desktop project is not configured as WPF: $($project.FullName)"
        }
        if ($text -match "Microsoft.EntityFrameworkCore|Npgsql|GameNet.Server") {
            throw "Desktop project references server/database implementation: $($project.FullName)"
        }
    }
}

Write-Host "Platform skeleton guard passed."
