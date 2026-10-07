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

$serverUrl = if ([string]::IsNullOrWhiteSpace($env:GAMENET_AGENT_SERVER_URL)) {
    "http://127.0.0.1:5080"
} else { $env:GAMENET_AGENT_SERVER_URL }

if ([string]::IsNullOrWhiteSpace($env:GAMENET_AGENT_BOOTSTRAP_SECRET)) {
    throw "Set GAMENET_AGENT_BOOTSTRAP_SECRET for Agent certification."
}

$identityRoot = Join-Path $env:TEMP ("GameNet-Agent-Cert-" + [guid]::NewGuid().ToString("N"))
$logPath = Join-Path $identityRoot "agent.log"
New-Item -ItemType Directory -Path $identityRoot -Force | Out-Null

$old = @{}
foreach ($name in @(
    "GameNet__AgentTransport__ServerBaseUrl",
    "GameNet__AgentTransport__AllowInsecureHttpForDevelopment",
    "GameNet__AgentIdentity__RootPath",
    "DOTNET_ENVIRONMENT"
)) { $old[$name] = [Environment]::GetEnvironmentVariable($name) }

try {
    $env:GameNet__AgentTransport__ServerBaseUrl = $serverUrl
    $env:GameNet__AgentTransport__AllowInsecureHttpForDevelopment = "true"
    $env:GameNet__AgentIdentity__RootPath = $identityRoot
    $env:DOTNET_ENVIRONMENT = "Development"

    $health = Invoke-RestMethod -Uri "$($serverUrl.TrimEnd('/'))/health" -Method Get -TimeoutSec 10
    if ($health.status -ne "ok" -or $health.readiness -ne "ready") {
        throw "Server is reachable but not ready. Status=$($health.status); Readiness=$($health.readiness)"
    }

    $process = Start-Process -FilePath $exe -PassThru -RedirectStandardOutput $logPath -RedirectStandardError $logPath
    try {
        $deadline = (Get-Date).AddSeconds(30)
        while ((Get-Date) -lt $deadline) {
            Start-Sleep -Seconds 2
            if ($process.HasExited) {
                $output = if (Test-Path $logPath) { Get-Content $logPath -Raw } else { "" }
                throw "Agent exited during real transport certification with code $($process.ExitCode). Output: $output"
            }

            $output = if (Test-Path $logPath) { Get-Content $logPath -Raw } else { "" }
            if ($output -match "Agent heartbeat accepted") {
                Write-Host "REAL AGENT TRANSPORT CERTIFICATION PASSED: Token -> SignalR -> Lease -> Heartbeat -> Reconciliation."
                break
            }
        }

        if ($output -notmatch "Agent heartbeat accepted") {
            throw "Agent process stayed alive but never proved Token -> SignalR -> Lease -> Heartbeat -> Reconciliation. Output: $output"
        }
    }
    finally {
        if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
    }
}
finally {
    foreach ($name in $old.Keys) {
        if ($null -eq $old[$name]) { Remove-Item "Env:$name" -ErrorAction SilentlyContinue }
        else { Set-Item "Env:$name" $old[$name] }
    }
    if (Test-Path $identityRoot) { Remove-Item $identityRoot -Recurse -Force -ErrorAction SilentlyContinue }
}
