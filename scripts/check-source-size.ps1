$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$reviewLines = 600
$hardLines = 800
$roots = @("src/Server", "src/Shared", "src/Client", "src/Desktop")

$excluded = @(
    "\bin\", "\obj\", "\Migrations\", ".Designer.cs", "ModelSnapshot.cs"
)

$reviewViolations = @()
$hardViolations = @()

foreach ($root in $roots) {
    if (-not (Test-Path $root)) { continue }

    Get-ChildItem $root -Recurse -File -Include *.cs,*.tsx,*.ts,*.xaml |
        Where-Object {
            $path = $_.FullName
            -not ($excluded | Where-Object { $path.Contains($_) })
        } |
        ForEach-Object {
            $lines = (Get-Content -LiteralPath $_.FullName).Count

            if ($lines -gt $hardLines) {
                $hardViolations += "$($_.FullName): $lines lines (hard limit $hardLines)"
            }
            elseif ($lines -gt $reviewLines) {
                $reviewViolations += "$($_.FullName): $lines lines (review threshold $reviewLines)"
            }
        }
}

$reviewViolations | ForEach-Object { Write-Host "REVIEW: $_" }

if ($hardViolations.Count -gt 0) {
    $hardViolations | ForEach-Object { Write-Host "HARD: $_" }
    throw "Source-size architecture guard failed."
}

Write-Host "Source-size architecture guard passed."
