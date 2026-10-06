$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Push-Location (Join-Path $PSScriptRoot "..\src\Dashboard")
try {
    if (-not (Test-Path "package-lock.json")) {
        throw "package-lock.json is required before CI certification. Generate and review it on the approved build runner."
    }

    npm ci
    npm run typecheck
    npm run build
}
finally {
    Pop-Location
}
