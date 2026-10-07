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

    [string]$CurrentManifestPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Get-SafeManifestPath {
    param([string]$Root, [string]$RelativePath)

    $full = [System.IO.Path]::GetFullPath(
        [System.IO.Path]::Combine($Root, $RelativePath.Replace("/", "")))

    $rootFull = [System.IO.Path]::GetFullPath($Root).TrimEnd("") + ""

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
            if ($response.Readiness -eq "Ready" -and
                $response.Status -eq "Healthy") {
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
$leaf = Split-Path -Leaf $install
New-Item -ItemType Directory -Path $parent -Force | Out-Null

$staging = Join-Path $parent ".gamenet-staging-$Component-$($manifest.ProductVersion)"
$previous = Join-Path $parent ".gamenet-previous-$Component-$((Get-Date).ToUniversalTime().ToString('yyyyMMddHHmmss'))"

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

    if (Test-Path $previous) { Remove-Item $previous -Recurse -Force }

    if (Test-Path $install) {
        Move-Item -LiteralPath $install -Destination $previous
    }

    Move-Item -LiteralPath $staging -Destination $install

    if ($ServiceName) {
        Start-Service -Name $ServiceName
    }

    Invoke-ServiceHealthCheck -Url $HealthUrl

    if (Test-Path $previous) {
        Remove-Item $previous -Recurse -Force
    }

    Write-Host "Local update applied successfully. Component=$Component Version=$($manifest.ProductVersion)"
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
