$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-CheckedDotnet {
    param([string[]]$Arguments)
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) { throw "dotnet failed with exit code $LASTEXITCODE." }
}

Invoke-CheckedDotnet @("build","src\Client\GameNet.Agent.csproj","--configuration","Release")
Invoke-CheckedDotnet @("test","tests\Agent.Tests\GameNet.Agent.Tests.csproj","--configuration","Release","--no-restore")

$exe = Join-Path (Resolve-Path ".").Path "src\Client\bin\Release\net10.0-windows\GameNet.Agent.exe"
if (-not (Test-Path $exe)) { throw "Agent executable was not produced: $exe" }
if ([string]::IsNullOrWhiteSpace($env:GAMENET_AGENT_BOOTSTRAP_SECRET)) { throw "Set GAMENET_AGENT_BOOTSTRAP_SECRET." }

$serverUrl = if ([string]::IsNullOrWhiteSpace($env:GAMENET_AGENT_SERVER_URL)) { "http://127.0.0.1:5080" } else { $env:GAMENET_AGENT_SERVER_URL }
if ($env:ASPNETCORE_ENVIRONMENT -eq "Production" -and $serverUrl -notmatch "^https://") { throw "Production Agent certification requires HTTPS." }

$identityRoot = Join-Path $env:TEMP ("GameNet-Agent-Cert-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $identityRoot -Force | Out-Null
$oldUrl=$env:GAMENET_AGENT_SERVER_URL
$oldRoot=$env:GAMENET_AGENT_IDENTITY_ROOT
try {
    $env:GAMENET_AGENT_SERVER_URL=$serverUrl
    $env:GAMENET_AGENT_IDENTITY_ROOT=$identityRoot
    $process=Start-Process -FilePath $exe -PassThru
    try {
        Start-Sleep -Seconds 8
        if ($process.HasExited) { throw "Agent exited during runtime smoke with code $($process.ExitCode)." }
    } finally {
        if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
    }
} finally {
    if ($null -eq $oldUrl) { Remove-Item Env:GAMENET_AGENT_SERVER_URL -ErrorAction SilentlyContinue } else { $env:GAMENET_AGENT_SERVER_URL=$oldUrl }
    if ($null -eq $oldRoot) { Remove-Item Env:GAMENET_AGENT_IDENTITY_ROOT -ErrorAction SilentlyContinue } else { $env:GAMENET_AGENT_IDENTITY_ROOT=$oldRoot }
    if (Test-Path $identityRoot) { Remove-Item $identityRoot -Recurse -Force -ErrorAction SilentlyContinue }
}
Write-Host "AGENT BUILD, TEST AND PROCESS SMOKE PASSED."
