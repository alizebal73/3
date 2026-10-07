$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

dotnet --version

& "$PSScriptRoot/check-pre-coding-readiness.ps1"
& "$PSScriptRoot/check-platform-skeleton.ps1"
& "$PSScriptRoot/check-source-size.ps1"
& "$PSScriptRoot/check-architecture.ps1"
& "$PSScriptRoot/check-foundation-completeness.ps1"
& "$PSScriptRoot/check-supply-chain.ps1"

dotnet tool restore
dotnet restore GameNet.slnx
dotnet build GameNet.slnx --configuration Release --no-restore
dotnet test GameNet.slnx --configuration Release --no-build --no-restore

Write-Host "LOCAL FOUNDATION VERIFICATION PASSED."
Write-Host "Next required evidence on Windows:"
Write-Host "1. launch src/Desktop/GameNet.Desktop.csproj and verify native window + fa/en direction;"
Write-Host "2. start a clean PostgreSQL instance and apply/validate migrations;"
Write-Host "3. run integration/concurrency/recovery certification against real PostgreSQL;"
Write-Host "4. record release/deployment compatibility evidence."
Write-Host "GitHub is source control; the manual self-hosted workflow only orchestrates this same local gate."
