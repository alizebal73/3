$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$central = Join-Path $root "Directory.Packages.props"
if (-not (Test-Path $central)) { throw "Directory.Packages.props is required for centrally pinned package versions." }

$projects = Get-ChildItem $root -Recurse -File -Filter *.csproj | Where-Object { $_.FullName -notmatch "\\bin\\|\\obj\\" }
$floating = foreach ($project in $projects) { Select-String -Path $project.FullName -Pattern "<PackageReference[^>]*\\sVersion\\s*=" }
if ($floating) { $floating | ForEach-Object { throw "Package version is declared locally instead of centrally: $($_.Path):$($_.LineNumber)" } }

$secretPatterns = @("AKIA[0-9A-Z]{16}", "-----BEGIN (RSA|EC|OPENSSH|PRIVATE) KEY-----", "(?i)password\\s*=\\s*["]{1}[^"]{8,}["]{1}")
foreach ($scanRoot in @((Join-Path $root "src"), (Join-Path $root "scripts"), (Join-Path $root "deploy"))) {
    if (-not (Test-Path $scanRoot)) { continue }
    $matches = Get-ChildItem $scanRoot -Recurse -File | Where-Object { $_.Extension -in ".cs", ".csproj", ".json", ".yml", ".yaml", ".ps1", ".md" } | Select-String -Pattern $secretPatterns
    if ($matches) { $matches | ForEach-Object { Write-Host "SUPPLY-CHAIN SECRET MATCH: $($_.Path):$($_.LineNumber): $($_.Line.Trim())" }; throw "Potential secret material detected in tracked source/configuration." }
}
if (Test-Path (Join-Path $root ".npmrc")) { throw "Unexpected browser/npm configuration exists in a native Desktop repository." }
Write-Host "Supply-chain guard passed."
