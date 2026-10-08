$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$moduleRoot = Join-Path $PSScriptRoot "..\src\Server\Modules"
$serverRoot = Join-Path $PSScriptRoot "..\src\Server"
$clientRoot = Join-Path $PSScriptRoot "..\src\Client"
$desktopRoot = Join-Path $PSScriptRoot "..\src\Desktop"
$sharedRoot = Join-Path $PSScriptRoot "..\src\Shared"

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

Assert-NoMatch -Root (Join-Path $moduleRoot "*\Domain") -Pattern "Microsoft\.EntityFrameworkCore|GameNet\.Server\.Persistence|GameNet\.Server\.Infrastructure" -Message "Domain code must not access EF, Persistence or Infrastructure."
Assert-NoMatch -Root (Join-Path $moduleRoot "*\Application") -Pattern "Microsoft\.EntityFrameworkCore|GameNet\.Server\.Persistence" -Message "Application code must not access EF or Persistence directly."
Assert-NoMatch -Root (Join-Path $moduleRoot "*\Api") -Pattern "Microsoft\.EntityFrameworkCore|GameNet\.Server\.Persistence" -Message "API code must not access EF or Persistence directly."

$moduleDirectories = Get-ChildItem $moduleRoot -Directory
foreach ($module in $moduleDirectories) {
    foreach ($file in Get-ChildItem $module.FullName -Recurse -File -Filter *.cs) {
        $text = Get-Content -Raw $file.FullName
        $refs = [regex]::Matches($text, "GameNet\.Server\.Modules\.([A-Za-z0-9_]+)") |
            ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique

        foreach ($ref in $refs) {
            if ($ref -ne $module.Name) {
                throw "Cross-module namespace reference detected: $($file.FullName) -> $ref. Use explicit contracts instead."
            }
        }
    }
}

Assert-NoMatch -Root $serverRoot -Pattern "DateTime\.Now|DateTime\.UtcNow|DateTimeOffset\.Now|DateTimeOffset\.UtcNow" -Message "Server code must use IGameClock/TimeProvider instead of wall-clock statics."

$program = Join-Path $serverRoot "Program.cs"
if ((Get-Content $program).Count -gt 200) {
    throw "Program.cs exceeded the composition-only 200 line limit."
}

Assert-NoMatch -Root $clientRoot -Pattern "GameNet\.Server\.(Persistence|Modules|Infrastructure)" -Message "Client Agent must not reference Server implementation namespaces."
Assert-NoMatch -Root $clientRoot -Pattern "DateTime\.Now|DateTime\.UtcNow|DateTimeOffset\.Now|DateTimeOffset\.UtcNow" -Message "Client Agent code must use TimeProvider/clock abstractions instead of wall-clock statics."

Assert-NoMatch -Root (Join-Path $clientRoot "Agent") -Pattern "HubConnection|HubConnectionBuilder|Microsoft\.AspNetCore\.SignalR\.Client" -Message "Agent core/worker code must not own SignalR transport mechanics; use Client/Transport."

Assert-NoMatch -Root (Join-Path $clientRoot "Transport") -Pattern "GameNet\.Server\.(Persistence|Modules)" -Message "Agent transport must not depend on Server implementation namespaces."

Assert-NoMatch -Root (Join-Path $clientRoot "Agent") -Pattern "HttpClient|HttpRequestMessage|HttpResponseMessage|WebClient|Socket" -Message "Agent runtime code must not own transport details; use Client/Transport."
Assert-NoMatch -Root (Join-Path $clientRoot "GameLaunch") -Pattern "HttpClient|HttpRequestMessage|HttpResponseMessage|WebClient|Socket" -Message "Game launch code must not own network transport details."

Assert-NoMatch -Root $desktopRoot -Pattern "GameNet\.Server\.(Persistence|Modules|Infrastructure)|Microsoft\.EntityFrameworkCore|Npgsql|DbContext|GameNetDbContext" -Message "Desktop must not reference Server implementation or direct database namespaces." -Include @("*.cs","*.xaml","*.csproj")
Assert-NoMatch -Root $desktopRoot -Pattern "WebView|Chromium|iframe|Vite|React|package\.json|node_modules" -Message "Desktop code contains a forbidden browser/web UI dependency." -Include @("*.cs","*.csproj","*.json")
Assert-NoMatch -Root $desktopRoot -Pattern "WebView|Chromium|iframe|Vite|React|package\.json|node_modules" -Message "Desktop XAML contains a forbidden browser/web UI dependency." -Include @("*.xaml")
Assert-NoMatch -Root (Join-Path $desktopRoot "Features") -Pattern "HttpClient|HttpRequestMessage|HttpResponseMessage|WebClient|Socket" -Message "Desktop feature UI must not own network transport; use Desktop/Api."
Assert-NoMatch -Root (Join-Path $desktopRoot "Shell") -Pattern "HttpClient|HttpRequestMessage|HttpResponseMessage|WebClient|Socket" -Message "Desktop shell must not own network transport; use Desktop/Api."

Assert-NoMatch -Root (Join-Path $desktopRoot "Features") -Pattern "\b(class|record)\s+\w+(Dto|Request|Response)\b" -Message "Desktop features must not define duplicate transport DTO/Request/Response types."
Assert-NoMatch -Root (Join-Path $desktopRoot "Api") -Pattern "\b(class|record)\s+\w+(Dto|Request|Response)\b" -Message "Desktop API boundary must consume Shared contracts rather than define duplicate transport DTO/Request/Response types."
Assert-NoMatch -Root (Join-Path $clientRoot "Agent") -Pattern "\b(class|record)\s+\w+(Dto|Request|Response)\b" -Message "Agent must not define duplicate transport DTO/Request/Response types."

Assert-NoMatch -Root (Join-Path $moduleRoot "*\Domain") -Pattern "IHubContext|HttpClient|WebClient|Process\.Start|File\.|Directory\.|Socket" -Message "Domain code must not perform external side effects."
Assert-NoMatch -Root (Join-Path $moduleRoot "*\Application") -Pattern "IHubContext|HttpClient|WebClient|Process\.Start|File\.|Directory\.|Socket" -Message "Application code must use explicit side-effect ports rather than performing external effects directly."

# Canonical Shared V1 contract authority: retired namespaces must never reappear.
Assert-NoMatch -Root $sharedRoot -Pattern "namespace\s+GameNet\.Shared\.Api\b|namespace\s+GameNet\.Shared\.Contracts\.Errors\b|using\s+GameNet\.Shared\.Api\b|using\s+GameNet\.Shared\.Contracts\.Errors\b" -Message "Legacy Shared API/error namespaces are forbidden; use Shared.Contracts.V1.Api."

$apiErrorDeclarations = @(Get-ChildItem $sharedRoot -Recurse -File -Filter *.cs |
    Select-String -Pattern "\b(record|class)\s+ApiError\b")

if ($apiErrorDeclarations.Count -ne 1) {
    throw "Exactly one canonical ApiError declaration is required under Shared; found $($apiErrorDeclarations.Count)."
}

Write-Host "Architecture guard passed."
