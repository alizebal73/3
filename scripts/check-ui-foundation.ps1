$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot
$desktop = Join-Path $root "src\Desktop"
$mainWindow = Join-Path $desktop "Shell\MainWindow.xaml"
$fa = Join-Path $desktop "Resources\Languages\Strings.fa-IR.xaml"
$en = Join-Path $desktop "Resources\Languages\Strings.en-US.xaml"

foreach ($path in @($mainWindow, $fa, $en)) {
    if (-not (Test-Path $path -PathType Leaf)) {
        throw "UI foundation resource is missing: $path"
    }
}

$mainText = Get-Content $mainWindow -Raw
$faText = Get-Content $fa -Raw
$enText = Get-Content $en -Raw

$dynamicKeys = @([regex]::Matches($mainText, 'DynamicResource\s+([A-Za-z0-9_.-]+)') |
    ForEach-Object { $_.Groups[1].Value } |
    Sort-Object -Unique)

foreach ($key in $dynamicKeys) {
    $escaped = [regex]::Escape($key)
    if ($faText -notmatch ('x:Key="' + $escaped + '"')) {
        throw "fa-IR resource is missing UI key: $key"
    }
    if ($enText -notmatch ('x:Key="' + $escaped + '"')) {
        throw "en-US resource is missing UI key: $key"
    }
}

$navigationCatalog = Get-Content (Join-Path $desktop "UI\Navigation\NavigationCatalog.cs") -Raw
$navigationKeys = @([regex]::Matches($navigationCatalog, 'new\("[^"]+",\s*"([^"]+)"') |
    ForEach-Object { $_.Groups[1].Value } |
    Sort-Object -Unique)

foreach ($key in $navigationKeys) {
    $escaped = [regex]::Escape($key)
    if ($faText -notmatch ('x:Key="' + $escaped + '"')) {
        throw "Navigation resource missing in fa-IR: $key"
    }
    if ($enText -notmatch ('x:Key="' + $escaped + '"')) {
        throw "Navigation resource missing in en-US: $key"
    }
}

foreach ($dictionary in @("Theme.xaml", "Controls.xaml", "Converters.xaml", "DataTemplates.xaml")) {
    $path = Join-Path $desktop "Resources\Styles\$dictionary"
    if (-not (Test-Path $path -PathType Leaf)) {
        throw "Required WPF UI dictionary is missing: $dictionary"
    }
}

$featureFiles = Get-ChildItem (Join-Path $desktop "Features") -Recurse -File -Include *.cs,*.xaml
foreach ($file in $featureFiles) {
    $text = Get-Content $file.FullName -Raw
    if ($text -match 'GameNet\.Server\.|Microsoft\.EntityFrameworkCore|Npgsql') {
        throw "Desktop feature crosses the Server/DB boundary: $($file.FullName)"
    }
}

Write-Host "Desktop UI foundation guard passed. ResourceCount=$($dynamicKeys.Count); NavigationCount=$($navigationKeys.Count)"
