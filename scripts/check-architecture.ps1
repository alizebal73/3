$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$moduleRoot = Join-Path $PSScriptRoot "..srcServerModules"
$serverRoot = Join-Path $PSScriptRoot "..srcServer"
$clientRoot = Join-Path $PSScriptRoot "..srcClient"
$dashboardRoot = Join-Path $PSScriptRoot "..srcDashboard"

function Assert-NoMatch {
    param(
        [string]$Root,
        [string]$Pattern,
        [string]$Message,
        [string[]]$Include = @("*.cs")
    )

    if (-not (Test-Path $Root)) { return }

    $matches = Get-ChildItem $Root -Recurse -File -Include $Include |
        Select-String -Pattern $Pattern

    if ($matches) {
        $matches | ForEach-Object {
            Write-Host "ARCHITECTURE: $($_.Path):$($_.LineNumber): $($_.Line.Trim())"
        }
        throw $Message
    }
}

Assert-NoMatch -Root (Join-Path $moduleRoot "*Domain") -Pattern "Microsoft.EntityFrameworkCore|GameNet.Server.Persistence|GameNet.Server.Infrastructure" -Message "Domain code must not access EF, Persistence or Infrastructure."
Assert-NoMatch -Root (Join-Path $moduleRoot "*Application") -Pattern "Microsoft.EntityFrameworkCore|GameNet.Server.Persistence" -Message "Application code must not access EF or Persistence directly."
Assert-NoMatch -Root (Join-Path $moduleRoot "*Api") -Pattern "Microsoft.EntityFrameworkCore|GameNet.Server.Persistence" -Message "API code must not access EF or Persistence directly."

$moduleDirectories = Get-ChildItem $moduleRoot -Directory
foreach ($module in $moduleDirectories) {
    foreach ($file in Get-ChildItem $module.FullName -Recurse -File -Filter *.cs) {
        $text = Get-Content -Raw $file.FullName
        $refs = [regex]::Matches($text, "GameNet.Server.Modules.([A-Za-z0-9_]+)") |
            ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

        foreach ($ref in $refs) {
            if ($ref -ne $module.Name) {
                throw "Cross-module namespace reference detected: $($file.FullName) -> $ref. Use explicit contracts instead."
            }
        }
    }
}

Assert-NoMatch -Root $serverRoot -Pattern "DateTime.Now|DateTime.UtcNow|DateTimeOffset.Now|DateTimeOffset.UtcNow" -Message "Server code must use IGameClock/TimeProvider instead of wall-clock statics."

$program = Join-Path $serverRoot "Program.cs"
if ((Get-Content $program).Count -gt 200) {
    throw "Program.cs exceeded the composition-only 200 line limit."
}

Assert-NoMatch -Root $clientRoot -Pattern "GameNet.Server.(Persistence|Modules|Infrastructure)" -Message "Client Agent must not reference Server implementation namespaces."
Assert-NoMatch -Root $dashboardRoot -Pattern "GameNet.Server.(Persistence|Modules|Infrastructure)" -Message "Dashboard must not reference Server implementation namespaces." -Include @("*.ts","*.tsx")
Assert-NoMatch -Root $dashboardRoot -Pattern "Microsoft.EntityFrameworkCore|DbContext|GameNetDbContext" -Message "Dashboard must not contain persistence/business database access." -Include @("*.ts","*.tsx")

Write-Host "Architecture guard passed."
