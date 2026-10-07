param(
    [Parameter(Mandatory = $true)]
    [string]$InstallRoot,

    [Parameter(Mandatory = $true)]
    [string]$RollbackRoot,

    [string]$ServiceName,
    [string]$HealthUrl
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$install = [System.IO.Path]::GetFullPath($InstallRoot)
$rollback = [System.IO.Path]::GetFullPath($RollbackRoot)

if (-not (Test-Path $rollback -PathType Container)) {
    throw "Rollback package does not exist: $rollback"
}

$parent = Split-Path -Parent $install
$failed = Join-Path $parent (".gamenet-failed-" + [DateTimeOffset]::UtcNow.ToString("yyyyMMddHHmmssfff"))

function Invoke-Health {
    param([string]$Url)
    if ([string]::IsNullOrWhiteSpace($Url)) { return }

    for ($attempt = 1; $attempt -le 20; $attempt++) {
        try {
            $response = Invoke-RestMethod -Uri $Url -Method Get -TimeoutSec 5
            if ($response.Readiness -in @("Ready", "ready") -and
                $response.Status -in @("Healthy", "healthy", "ok")) {
                return
            }
        } catch { }
        Start-Sleep -Seconds 2
    }

    throw "Health check failed after rollback: $Url"
}

try {
    if ($ServiceName) {
        $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
        if ($service -and $service.Status -ne "Stopped") {
            Stop-Service -Name $ServiceName -Force
            $service.WaitForStatus(
                [System.ServiceProcess.ServiceControllerStatus]::Stopped,
                [TimeSpan]::FromSeconds(30))
        }
    }

    if (Test-Path $failed) { Remove-Item $failed -Recurse -Force }
    if (Test-Path $install) { Move-Item -LiteralPath $install -Destination $failed }
    Move-Item -LiteralPath $rollback -Destination $install

    if ($ServiceName) { Start-Service -Name $ServiceName }
    Invoke-Health -Url $HealthUrl

    Write-Host "Local rollback completed successfully."
    Write-Host "Failed release preserved at: $failed"
}
catch {
    if ($ServiceName) {
        Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path $install) { Remove-Item $install -Recurse -Force -ErrorAction SilentlyContinue }
    if (Test-Path $failed) { Move-Item -LiteralPath $failed -Destination $install }

    if ($ServiceName) { Start-Service -Name $ServiceName -ErrorAction SilentlyContinue }
    throw
}
