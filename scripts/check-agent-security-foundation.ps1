$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$clientRoot = Join-Path $root "src\Client"
$serverAppSettings = Join-Path $root "src\Server\appsettings.json"
$clientAppSettings = Join-Path $clientRoot "appsettings.json"
$agentProject = Join-Path $clientRoot "GameNet.Agent.csproj"
$credentialStore = Join-Path $clientRoot "Identity\AgentCredentialStore.cs"
$transport = Join-Path $clientRoot "Transport\SignalRAgentTransport.cs"

foreach ($path in @($serverAppSettings, $clientAppSettings, $agentProject, $credentialStore, $transport)) {
    if (-not (Test-Path $path)) {
        throw "Agent security guard input is missing: $path"
    }
}

$clientJson = Get-Content $clientAppSettings -Raw
$serverJson = Get-Content $serverAppSettings -Raw

foreach ($forbidden in @('"AccessToken"s*:', '"ProvisioningKey"s*:', '"Secret"s*:')) {
    if ($clientJson -match $forbidden) {
        throw "Client appsettings contains a forbidden credential field: $forbidden"
    }

    if ($serverJson -match $forbidden) {
        throw "Server appsettings contains a forbidden raw credential field: $forbidden"
    }
}

$project = Get-Content $agentProject -Raw
if ($project -notmatch '<TargetFramework>net10.0-windows</TargetFramework>') {
    throw "Windows Agent must target net10.0-windows."
}

$store = Get-Content $credentialStore -Raw
foreach ($required in @("ProtectedData.Protect", "ProtectedData.Unprotect", "DataProtectionScope.CurrentUser")) {
    if (-not $store.Contains($required)) {
        throw "Agent credential store is missing protected-secret mechanism: $required"
    }
}

$transportText = Get-Content $transport -Raw
if ($transportText -match 'options.Value.AccessToken') {
    throw "SignalR Agent transport must not read a raw AccessToken configuration value."
}

Write-Host "Agent security foundation guard passed."
