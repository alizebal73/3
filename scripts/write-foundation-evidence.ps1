param(
    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$gitRevision = (& git -C $root rev-parse HEAD).Trim()
$dotnetVersion = (& dotnet --version).Trim()

$evidence = [ordered]@{
    Product = "GameNet Manager 3"
    GitRevision = $gitRevision
    CapturedAtUtc = [DateTimeOffset]::UtcNow.ToString("O")
    MachineName = $env:COMPUTERNAME
    UserName = $env:USERNAME
    DotnetVersion = $dotnetVersion
    DatabaseCertification = [bool](-not [string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE))
    RestoreCertification = [bool](-not [string]::IsNullOrWhiteSpace($env:GAMENET_RESTORE_DATABASE))
}

$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
$directory = Split-Path -Parent $OutputPath
if ($directory) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

$evidence | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $OutputPath -Encoding utf8
Write-Host "Foundation evidence written: $OutputPath"
