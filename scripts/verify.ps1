$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $false)]
        [string[]]$ArgumentList = @()
    )

    & $FilePath @ArgumentList

    if ($LASTEXITCODE -ne 0) {
        throw "Command failed with exit code $LASTEXITCODE: $FilePath $($ArgumentList -join ' ')"
    }
}

Invoke-Checked -FilePath "dotnet" -ArgumentList @("--version")

& "$PSScriptRoot/check-pre-coding-readiness.ps1"
if ($LASTEXITCODE -ne 0) { throw "Pre-coding readiness guard failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-platform-skeleton.ps1"
if ($LASTEXITCODE -ne 0) { throw "Platform skeleton guard failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-source-size.ps1"
if ($LASTEXITCODE -ne 0) { throw "Source-size architecture guard failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-architecture.ps1"
if ($LASTEXITCODE -ne 0) { throw "Architecture guard failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-foundation-completeness.ps1"
if ($LASTEXITCODE -ne 0) { throw "Foundation completeness guard failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-supply-chain.ps1"
if ($LASTEXITCODE -ne 0) { throw "Supply-chain guard failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-foundation-readiness-final.ps1"
if ($LASTEXITCODE -ne 0) { throw "Foundation final-readiness structure check failed with exit code $LASTEXITCODE." }

& "$PSScriptRoot/check-ui-foundation.ps1"
if ($LASTEXITCODE -ne 0) { throw "Desktop UI foundation guard failed with exit code $LASTEXITCODE." }

Invoke-Checked -FilePath "dotnet" -ArgumentList @("tool", "restore")
Invoke-Checked -FilePath "dotnet" -ArgumentList @("restore", "GameNet.slnx")
Invoke-Checked -FilePath "dotnet" -ArgumentList @("build", "GameNet.slnx", "--configuration", "Release", "--no-restore")
Invoke-Checked -FilePath "dotnet" -ArgumentList @("test", "GameNet.slnx", "--configuration", "Release", "--no-build", "--no-restore")

Write-Host "LOCAL FOUNDATION VERIFICATION PASSED."
Write-Host "Next required evidence on Windows:"
Write-Host "1. launch src/Desktop/GameNet.Desktop.csproj and verify native window + fa/en direction;"
Write-Host "2. start a clean PostgreSQL instance and apply/validate migrations;"
Write-Host "3. run integration/concurrency/recovery certification against real PostgreSQL;"
Write-Host "4. record release/deployment compatibility evidence."
Write-Host "GitHub is source control; the manual self-hosted workflow only orchestrates this same local gate."
