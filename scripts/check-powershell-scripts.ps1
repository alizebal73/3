$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$files = @(Get-ChildItem (Join-Path $root "scripts") -Recurse -File -Filter *.ps1)

foreach ($file in $files) {
    $tokens = $null
    $errors = @()

    [void][System.Management.Automation.Language.Parser]::ParseFile(
        $file.FullName,
        [ref]$tokens,
        [ref]$errors)

    if (@($errors).Count -gt 0) {
        $details = $errors | ForEach-Object { "$($_.Message) at line $($_.Extent.StartLineNumber)" }
        throw "PowerShell syntax error(s) in $($file.FullName): $($details -join '; ')"
    }

    $text = Get-Content $file.FullName -Raw

    if ($text -match 'Join-Path\s+\$root\s+"[^"]+"\s*,') {
        throw "Suspicious Join-Path argument list detected in $($file.FullName). Build path arrays with parenthesized Join-Path expressions."
    }

    if ($text -match '\.Replace\(\s*""\s*,') {
        throw "Empty-string Replace() detected in $($file.FullName)."
    }

    if ($text -match '\.TrimEnd\(\s*""\s*\)') {
        throw "Empty-string TrimEnd() detected in $($file.FullName)."
    }
}

Write-Host "PowerShell script syntax and safety-pattern guard passed. ScriptCount=$($files.Count)"
