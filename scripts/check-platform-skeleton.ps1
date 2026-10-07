$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot

$requiredPaths = @(
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
    "src\Server\Modules\README.md",
    "src\Server\Persistence\Migrations\202610070001_FoundationInfrastructure.cs",
    "src\Server\Persistence\Migrations\GameNetDbContextModelSnapshot.cs"
)

foreach ($relativePath in $requiredPaths) {
    $path = Join-Path $root $relativePath
    if (-not (Test-Path $path)) {
        throw "Required platform skeleton path is missing: $relativePath"
    }
}

$modules = @(
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
    ".nvmrc",
    ".github\workflows"
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
