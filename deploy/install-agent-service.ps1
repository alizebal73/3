param(
    [Parameter(Mandatory = $true)]
    [string]$ExecutablePath,

    [string]$ServiceName = "GameNet Agent",

    [string]$IdentityRoot = (
        Join-Path (
            [Environment]::GetFolderPath(
                [Environment+SpecialFolder]::CommonApplicationData)
        ) "GameNet Manager\Agent"
    ),

    [string]$DeviceId,

    [string]$BootstrapCredential,

    [int]$StartupTimeoutSeconds = 30
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$principal = New-Object Security.Principal.WindowsPrincipal(
    [Security.Principal.WindowsIdentity]::GetCurrent())

if (-not $principal.IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw "Installing GameNet Agent requires an elevated PowerShell session."
}

if (-not (Test-Path $ExecutablePath -PathType Leaf)) {
    throw "Agent executable not found: $ExecutablePath"
}

if ($StartupTimeoutSeconds -lt 5 -or $StartupTimeoutSeconds -gt 300) {
    throw "StartupTimeoutSeconds must be between 5 and 300."
}

$resolved = (Resolve-Path $ExecutablePath).Path
$identityRootPath = [System.IO.Path]::GetFullPath($IdentityRoot)
$credentialPath = Join-Path $identityRootPath "credential.bin"
$identityPath = Join-Path $identityRootPath "identity.json"
$bootstrapVariable = "GAMENET_AGENT_BOOTSTRAP_SECRET"
$bootstrapWasSet = $false

if (-not [string]::IsNullOrWhiteSpace($BootstrapCredential) -and
    [string]::IsNullOrWhiteSpace($DeviceId)) {
    throw "DeviceId is required when BootstrapCredential is supplied."
}

if (-not [string]::IsNullOrWhiteSpace($BootstrapCredential) -and
    $BootstrapCredential.Length -lt 32) {
    throw "BootstrapCredential must contain at least 32 characters."
}

New-Item -ItemType Directory -Path $identityRootPath -Force | Out-Null

if (-not [string]::IsNullOrWhiteSpace($DeviceId)) {
    $identityJson = @{ DeviceId = $DeviceId } | ConvertTo-Json -Compress
    [System.IO.File]::WriteAllText($identityPath, $identityJson)
}

$existing = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existing) {
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    & sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Milliseconds 500
}

try {
    if (-not [string]::IsNullOrWhiteSpace($BootstrapCredential)) {
        [Environment]::SetEnvironmentVariable(
            $bootstrapVariable,
            $BootstrapCredential,
            [EnvironmentVariableTarget]::Machine)
        $bootstrapWasSet = $true
    }

    New-Service -Name $ServiceName -BinaryPathName ('"' + $resolved + '"' ) -DisplayName $ServiceName -StartupType Automatic | Out-Null

    & sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null

    Start-Service -Name $ServiceName

    if ($bootstrapWasSet) {
        $deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)

        while ((Get-Date) -lt $deadline) {
            if (Test-Path $credentialPath -PathType Leaf) {
                break
            }

            $service = Get-Service -Name $ServiceName
            if ($service.Status -eq "Stopped") {
                throw "Agent service stopped before creating its protected credential store."
            }

            Start-Sleep -Seconds 1
        }

        if (-not (Test-Path $credentialPath -PathType Leaf)) {
            throw "Agent did not create the protected credential store within $StartupTimeoutSeconds seconds."
        }
    }

    Write-Host "Installed Windows Service '$ServiceName' from $resolved"
    Write-Host "Agent identity root: $identityRootPath"
    if ($bootstrapWasSet) {
        Write-Host "Bootstrap credential was consumed by the service and the machine environment variable will be removed."
    }
}
finally {
    if ($bootstrapWasSet) {
        [Environment]::SetEnvironmentVariable(
            $bootstrapVariable,
            $null,
            [EnvironmentVariableTarget]::Machine)
    }
}
