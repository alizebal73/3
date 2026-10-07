param(
    [string]$ServerUrl = "http://127.0.0.1:5080",
    [string]$OutputRoot = ".\artifacts\diagnostics",
    [string[]]$ServiceNames = @("GameNet Server", "GameNet Agent")
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$outRoot = [System.IO.Path]::GetFullPath((Join-Path $root $OutputRoot))
$stamp = [DateTimeOffset]::UtcNow.ToString("yyyyMMdd-HHmmss")
$bundleRoot = Join-Path $outRoot "GameNet-Diagnostics-$stamp"
New-Item -ItemType Directory -Path $bundleRoot -Force | Out-Null

function Write-JsonFile {
    param([string]$Name, [object]$Value)
    $Value | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $bundleRoot $Name) -Encoding utf8
}

$metadata = [ordered]@{
    CollectedAtUtc = [DateTimeOffset]::UtcNow.ToString("O")
    MachineName = $env:COMPUTERNAME
    UserName = $env:USERNAME
    OS = [System.Environment]::OSVersion.VersionString
    PowerShell = $PSVersionTable.PSVersion.ToString()
}

try { $metadata.DotNet = (& dotnet --version 2>$null).Trim() } catch { $metadata.DotNet = $null }
try { $metadata.GitSha = (& git -C $root rev-parse HEAD 2>$null).Trim() } catch { $metadata.GitSha = $null }

Write-JsonFile -Name "environment.json" -Value $metadata

$serviceState = foreach ($serviceName in $ServiceNames) {
    $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

    [ordered]@{
        Name = $serviceName
        Exists = $null -ne $service
        Status = if ($service) { $service.Status.ToString() } else { $null }
        StartType = if ($service) { (Get-CimInstance Win32_Service -Filter ("Name='{0}'" -f $service.Name) -ErrorAction SilentlyContinue).StartMode } else { $null }
    }
}
Write-JsonFile -Name "services.json" -Value @($serviceState)

$health = [ordered]@{
    Reachable = $false
    Status = $null
    Readiness = $null
    CorrelationId = $null
    OperationId = $null
    Error = $null
}

try {
    $response = Invoke-WebRequest -Uri "$($ServerUrl.TrimEnd('/'))/health" -Method Get -TimeoutSec 5
    $payload = $response.Content | ConvertFrom-Json
    $health.Reachable = $true
    $health.Status = $payload.status
    $health.Readiness = $payload.readiness
    $health.CorrelationId = $response.Headers["X-Correlation-Id"]
    $health.OperationId = $response.Headers["X-Operation-Id"]
}
catch {
    $health.Error = $_.Exception.Message
}

Write-JsonFile -Name "health.json" -Value $health

$buildInfo = [ordered]@{
    Reachable = $false
    Data = $null
    Error = $null
}

try {
    $response = Invoke-WebRequest -Uri "$($ServerUrl.TrimEnd('/'))/api/v1/system/build-info" -Method Get -TimeoutSec 5
    $buildInfo.Reachable = $true
    $buildInfo.Data = $response.Content | ConvertFrom-Json
}
catch {
    $buildInfo.Error = $_.Exception.Message
}

Write-JsonFile -Name "build-info.json" -Value $buildInfo

$summary = [ordered]@{
    Product = "GameNet"
    CollectedAtUtc = $metadata.CollectedAtUtc
    ServerUrl = $ServerUrl
    ServerReachable = $health.Reachable
    ServerReadiness = $health.Readiness
    Services = @($serviceState)
    BuildInfoAvailable = $buildInfo.Reachable
    IncludesSecrets = $false
    IncludesPasswords = $false
    IncludesAccessTokens = $false
    IncludesPaymentCredentials = $false
}
Write-JsonFile -Name "summary.json" -Value $summary

$zip = "$bundleRoot.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path (Join-Path $bundleRoot "*") -DestinationPath $zip -CompressionLevel Optimal

Write-Host "Diagnostic bundle created: $zip"
Write-Host "Bundle is intentionally limited to non-secret operational metadata and health/build evidence."
