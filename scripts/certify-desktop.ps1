$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Invoke-CheckedDotnet {
    param([string[]]$Arguments)
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet failed with exit code \${LASTEXITCODE}: dotnet $($Arguments -join ' ')"
    }
}

$project = "src\Desktop\GameNet.Desktop.csproj"
Invoke-CheckedDotnet -Arguments @("build", $project, "--configuration", "Release")

$exe = Join-Path (Resolve-Path ".").Path "src\Desktop\bin\Release\net10.0-windows\GameNet.Manager.Desktop.exe"
if (-not (Test-Path $exe)) {
    throw "Desktop executable was not produced: $exe"
}

$process = Start-Process -FilePath $exe -PassThru
try {
    Start-Sleep -Seconds 3
    if ($process.HasExited) {
        throw "Desktop process exited during launch certification with code $($process.ExitCode)."
    }
}
finally {
    if (-not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }
}

Write-Host "NATIVE DESKTOP LAUNCH CERTIFICATION PASSED."
