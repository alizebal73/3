$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

dotnet --version

& "$PSScriptRoot/check-platform-skeleton.ps1"
& "$PSScriptRoot/check-source-size.ps1"
& "$PSScriptRoot/check-architecture.ps1"
& "$PSScriptRoot/check-foundation-completeness.ps1"

dotnet tool restore
dotnet restore GameNet.slnx
dotnet build GameNet.slnx --configuration Release --no-restore
dotnet test GameNet.slnx --configuration Release --no-build --no-restore

Write-Host "LOCAL FOUNDATION VERIFICATION PASSED."
Write-Host "GitHub is source control only; this result is produced on the local Windows machine."
