param(
    [Parameter(Mandatory = $true)]
    [string]$ExecutablePath,
    [string]$ShortcutName = "GameNet Manager"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if (-not (Test-Path $ExecutablePath)) {
    throw "Desktop executable not found: $ExecutablePath"
}

$resolved = (Resolve-Path $ExecutablePath).Path
$shell = New-Object -ComObject WScript.Shell
$desktop = [Environment]::GetFolderPath("Desktop")
$shortcut = $shell.CreateShortcut((Join-Path $desktop ($ShortcutName + ".lnk")))
$shortcut.TargetPath = $resolved
$shortcut.WorkingDirectory = Split-Path -Parent $resolved
$shortcut.Save()

Write-Host "Created Desktop shortcut: $($shortcut.FullName)"
