$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

dotnet --version
node --version
npm --version

& "$PSScriptRoot/check-source-size.ps1"
& "$PSScriptRoot/check-architecture.ps1"
& "$PSScriptRoot/check-foundation-completeness.ps1"

dotnet tool restore
dotnet restore GameNet.slnx
dotnet build GameNet.slnx --configuration Release --no-restore
dotnet test GameNet.slnx --configuration Release --no-build --no-restore

& "$PSScriptRoot/verify-dashboard.ps1"
