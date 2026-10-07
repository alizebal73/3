$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$contractPath = Join-Path $root "docs\architecture\foundation-closure-contract.md"
$matrixPath = Join-Path $root "docs\architecture\foundation-closure-matrix.md"
$verifyPath = Join-Path $PSScriptRoot "verify.ps1"
$matrix = Get-Content $matrixPath -Raw -ErrorAction Stop
$verify = Get-Content $verifyPath -Raw -ErrorAction Stop

foreach ($path in @($contractPath, $matrixPath, $verifyPath)) {
    if (-not (Test-Path $path -PathType Leaf)) {
        throw "Foundation closure contract input is missing: $path"
    }
}

$contract = Get-Content $contractPath -Raw
$ids = @([regex]::Matches($contract, 'FND-[0-9]{3}') |
    ForEach-Object { $_.Value } |
    Sort-Object -Unique)

if ($ids.Count -lt 42) {
    throw "Foundation closure contract is missing required atomic rows. Found $($ids.Count), expected at least 42."
}

$rowIds = @([regex]::Matches($contract, '^\|\s*(FND-[0-9]{3})\s*\|', [System.Text.RegularExpressions.RegexOptions]::Multiline) |
    ForEach-Object { $_.Groups[1].Value })

$duplicates = @($rowIds | Group-Object | Where-Object Count -gt 1)
if ($duplicates.Count -gt 0) {
    throw "Foundation closure contract contains duplicate IDs: $($duplicates.Name -join ', ')"
}

$allowedStatusPatterns = @(
    '^Closed — .+',
    '^Pending — .+',
    '^Open — .+',
    '^Business — Deferred.*
)

foreach ($line in ($contract -split "`r?`n")) {
    if ($line -notmatch '^\|\s*(FND-[0-9]{3})\s*\|') { continue }
    $cells = $line.Trim('|').Split('|') | ForEach-Object { $_.Trim() }
    if ($cells.Count -lt 4) { throw "Malformed Foundation closure row: $line" }
    $status = $cells[3]
    if (-not ($allowedStatusPatterns | Where-Object { $status -match $_ })) {
        throw "Unknown Foundation closure status in row $($cells[0]): $status"
    }
}

if ($matrix -notmatch 'foundation-closure-contract\.md') {
    throw "Foundation closure matrix must reference the atomic closure contract."
}

if ($verify -notmatch 'check-foundation-closure-contract\.ps1') {
    throw "scripts\verify.ps1 must execute the Foundation closure contract guard."
}

Write-Host "Foundation closure contract guard passed. AtomicRowCount=$($ids.Count)"
)

foreach ($line in ($contract -split "`r?`n")) {
    if ($line -notmatch '^\|\s*(FND-[0-9]{3})\s*\|') { continue }
    $cells = $line.Trim('|').Split('|') | ForEach-Object { $_.Trim() }
    if ($cells.Count -lt 4) { throw "Malformed Foundation closure row: $line" }
    $status = $cells[3]
    if (-not ($allowedStatusPatterns | Where-Object { $status -match $_ })) {
        throw "Unknown Foundation closure status in row $($cells[0]): $status"
    }
}

if ($matrix -notmatch 'foundation-closure-contract\.md') {
    throw "Foundation closure matrix must reference the atomic closure contract."
}

if ($verify -notmatch 'check-foundation-closure-contract\.ps1') {
    throw "scripts\verify.ps1 must execute the Foundation closure contract guard."
}

Write-Host "Foundation closure contract guard passed. AtomicRowCount=$($ids.Count)"