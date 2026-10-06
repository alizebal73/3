$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

dotnet --version
node --version
npm --version

& "$PSScriptRoot/check-source-size.ps1"

dotnet restore GameNet.slnx
dotnet build GameNet.slnx --configuration Release --no-restore
dotnet test GameNet.slnx --configuration Release --no-build --no-restore

Push-Location (Join-Path $PSScriptRoot "..\src\Dashboard")
try {
    npm install
    npm run typecheck
    npm run build
}
finally {
    Pop-Location
}
