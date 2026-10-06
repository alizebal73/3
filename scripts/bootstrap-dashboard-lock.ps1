$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Push-Location (Join-Path $PSScriptRoot "..\src\Dashboard")
try {
    npm install --package-lock-only
    npm ci
    npm run typecheck
    npm run build

    if (-not (Test-Path "package-lock.json")) {
        throw "npm did not create package-lock.json."
    }
}
finally {
    Pop-Location
}
