param(
    [Parameter(Mandatory = $true)]
    [string]$ExecutablePath,
    [string]$ServiceName = "GameNet Agent"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw "Installing GameNet Agent requires an elevated PowerShell session."
}

if (-not (Test-Path $ExecutablePath -PathType Leaf)) {
    throw "Agent executable not found: $ExecutablePath"
}

$resolved = (Resolve-Path $ExecutablePath).Path
$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if ($existing) {
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    & sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Milliseconds 500
}

New-Service -Name $ServiceName -BinaryPathName ('"' + $resolved + '"') -DisplayName $ServiceName -StartupType Automatic
& sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null

Write-Host "Installed Windows Service '$ServiceName' from $resolved"
