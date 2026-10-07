param(
    [Parameter(Mandatory = $true)]
    [string]$ExecutablePath,
    [string]$ServiceName = "GameNet Server"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if (-not (Test-Path $ExecutablePath)) {
    throw "Server executable not found: $ExecutablePath"
}

$resolved = (Resolve-Path $ExecutablePath).Path
$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if ($existing) {
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    & sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Milliseconds 500
}

New-Service -Name $ServiceName -BinaryPathName ('"' + $resolved + '"') -DisplayName $ServiceName -StartupType Automatic
Write-Host "Installed Windows Service '$ServiceName' from $resolved"
