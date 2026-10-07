param(
    [Parameter(Mandatory = $true)]
    [string]$PackageRoot,

    [Parameter(Mandatory = $true)]
    [string]$InstallRoot,

    [Parameter(Mandatory = $true)]
    [ValidateSet("Server", "Agent", "Desktop")]
    [string]$Component,

    [string]$ServiceName,
    [string]$HealthUrl,

    [Parameter(Mandatory = $true)]
    [string]$CurrentManifestPath,

    [string]$ExpectedPublisher
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Get-SafeManifestPath {
    param([string]$Root, [string]$RelativePath)

    if ([System.IO.Path]::IsPathRooted($RelativePath)) {
        throw "Manifest path must be relative: $RelativePath"
    }

    $rootFull = [System.IO.Path]::GetFullPath($Root)
    $rootFull = $rootFull.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) +
        [System.IO.Path]::DirectorySeparatorChar

    $normalizedRelative = $RelativePath.Replace("/", [System.IO.Path]::DirectorySeparatorChar)
    $full = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($Root, $normalizedRelative))

    if (-not $full.StartsWith($rootFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Manifest path escapes package root: $RelativePath"
    }

    return $full
}

function Read-And-VerifyManifest {
    param([string]$Root)

    $manifestPath = Join-Path $Root "manifest.json"
    if (-not (Test-Path $manifestPath -PathType Leaf)) {
        throw "Update package manifest.json is missing."
    }

    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json

    if ([string]::IsNullOrWhiteSpace($manifest.ProductVersion)) {
        throw "Release manifest ProductVersion is missing."
    }

    foreach ($entry in $manifest.Files) {
        $filePath = Get-SafeManifestPath -Root $Root -RelativePath $entry.RelativePath

        if (-not (Test-Path $filePath -PathType Leaf)) {
            throw "Package file is missing: $($entry.RelativePath)"
        }

        $info = Get-Item $filePath
        if ([int64]$info.Length -ne [int64]$entry.SizeBytes) {
            throw "Package size mismatch: $($entry.RelativePath)"
        }

        $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $filePath).Hash.ToLowerInvariant()
        if ($hash -ne $entry.Sha256.ToLowerInvariant()) {
            throw "Package checksum mismatch: $($entry.RelativePath)"
        }
    }

    return $manifest
}

function Invoke-ServiceHealthCheck {
    param([string]$Url)

    if ([string]::IsNullOrWhiteSpace($Url)) {
        return
    }

    for ($attempt = 1; $attempt -le 20; $attempt++) {
        try {
            $response = Invoke-RestMethod -Uri $Url -Method Get -TimeoutSec 5
            if ($response.Readiness -in @("Ready", "ready") -and
                $response.Status -in @("Healthy", "healthy", "ok")) {
                return
            }
        }
        catch {
            # Continue retrying until the bounded health window expires.
        }

        Start-Sleep -Seconds 2
    }

    throw "Component health check failed after update: $Url"
}

$package = (Resolve-Path $PackageRoot).Path
$install = [System.IO.Path]::GetFullPath($InstallRoot)
$manifest = Read-And-VerifyManifest -Root $package

if (-not [string]::IsNullOrWhiteSpace($ExpectedPublisher)) {
    & (Join-Path $PSScriptRoot "verify-release-signatures.ps1") `
        -Root $package `
        -ExpectedPublisher $ExpectedPublisher
}

if (-not [string]::IsNullOrWhiteSpace($CurrentManifestPath)) {
    if (-not (Test-Path $CurrentManifestPath -PathType Leaf)) {
        throw "Current release manifest was not found: $CurrentManifestPath"
    }

    $current = Get-Content $CurrentManifestPath -Raw | ConvertFrom-Json

    if ([int]$manifest.SchemaVersion -lt [int]$current.SchemaVersion) {
        throw "Incoming SchemaVersion $($manifest.SchemaVersion) cannot downgrade current schema $($current.SchemaVersion)."
    }

    if ($Component -in @("Server", "Desktop") -and
        $manifest.ApiContractVersion -ne $current.ApiContractVersion) {
        throw "API contract change requires an explicit coordinated release. Current=$($current.ApiContractVersion); Incoming=$($manifest.ApiContractVersion)"
    }

    if ($Component -in @("Server", "Agent") -and
        [int]$manifest.AgentProtocolVersion -ne [int]$current.AgentProtocolVersion) {
        throw "Agent protocol change requires an explicit coordinated release. Current=$($current.AgentProtocolVersion); Incoming=$($manifest.AgentProtocolVersion)"
    }
}

$parent = Split-Path -Parent $install
New-Item -ItemType Directory -Path $parent -Force | Out-Null

$staging = Join-Path $parent ".gamenet-staging-$Component-$($manifest.ProductVersion)"
$previous = Join-Path $parent ".gamenet-previous-$Component-$([DateTimeOffset]::UtcNow.ToString('yyyyMMddHHmmssfff'))"
$rollbackPointer = Join-Path $parent ".gamenet-rollback-$Component.json"

if (Test-Path $staging) { Remove-Item $staging -Recurse -Force }
New-Item -ItemType Directory -Path $staging -Force | Out-Null

try {
    foreach ($entry in $manifest.Files) {
        $source = Get-SafeManifestPath -Root $package -RelativePath $entry.RelativePath
        $target = Get-SafeManifestPath -Root $staging -RelativePath $entry.RelativePath

        New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force | Out-Null
        Copy-Item $source $target -Force
    }

    if ($ServiceName) {
        $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
        if ($service -and $service.Status -ne "Stopped") {
            Stop-Service -Name $ServiceName -Force
            $service.WaitForStatus(
                [System.ServiceProcess.ServiceControllerStatus]::Stopped,
                [TimeSpan]::FromSeconds(30))
        }
    }

    if (Test-Path $rollbackPointer) {
        $oldPointer = Get-Content $rollbackPointer -Raw | ConvertFrom-Json
        if ($oldPointer.BackupPath -and (Test-Path $oldPointer.BackupPath)) {
            Remove-Item $oldPointer.BackupPath -Recurse -Force
        }
        Remove-Item $rollbackPointer -Force
    }

    if (Test-Path $install) {
        Move-Item -LiteralPath $install -Destination $previous
    }

    Move-Item -LiteralPath $staging -Destination $install

    if ($ServiceName) {
        Start-Service -Name $ServiceName
    }

    Invoke-ServiceHealthCheck -Url $HealthUrl

    @{ BackupPath = $previous; InstalledPath = $install; ProductVersion = $manifest.ProductVersion; CreatedUtc = [DateTimeOffset]::UtcNow.ToString("O") } |
        ConvertTo-Json | Set-Content -LiteralPath $rollbackPointer -Encoding utf8

    Write-Host "Local update applied successfully. Component=$Component Version=$($manifest.ProductVersion)"
    Write-Host "Rollback point preserved at: $previous"
}
catch {
    Write-Warning "Update failed. Starting rollback: $($_.Exception.Message)"

    if ($ServiceName) {
        Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path $install) {
        Remove-Item $install -Recurse -Force
    }

    if (Test-Path $previous) {
        Move-Item -LiteralPath $previous -Destination $install
    }

    if ($ServiceName) {
        Start-Service -Name $ServiceName -ErrorAction Stop
    }

    throw
}
finally {
    if (Test-Path $staging) {
        Remove-Item $staging -Recurse -Force -ErrorAction SilentlyContinue
    }
}
