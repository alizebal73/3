param([Parameter(Mandatory=$true)][string]$OutputPath)
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root=(Resolve-Path (Join-Path $PSScriptRoot "..")).Path
function Get-Version([string]$Name) {
    $c=Get-Command $Name -ErrorAction SilentlyContinue
    if ($null -eq $c) { return $null }
    try { return (& $Name --version 2>&1 | Select-Object -First 1).ToString().Trim() } catch { return $null }
}
function Hash([string]$Path) {
    if (Test-Path $Path -PathType Leaf) { return (Get-FileHash -Algorithm SHA256 $Path).Hash }
    return $null
}

$evidence=[ordered]@{
    Product="GameNet Manager 3"
    GitRevision=(& git -C $root rev-parse HEAD).Trim()
    Branch=(& git -C $root branch --show-current).Trim()
    CapturedAtUtc=[DateTimeOffset]::UtcNow.ToString("O")
    MachineName=$env:COMPUTERNAME
    UserName=$env:USERNAME
    OS=[Environment]::OSVersion.VersionString
    DotnetVersion=(& dotnet --version).Trim()
    PostgreSqlClientVersion=Get-Version "psql"
    PgDumpVersion=Get-Version "pg_dump"
    PgRestoreVersion=Get-Version "pg_restore"
    DesktopExecutableSha256=Hash (Join-Path $root "src\Desktop\bin\Release\net10.0-windows\GameNet.Manager.Desktop.exe")
    AgentExecutableSha256=Hash (Join-Path $root "src\Client\bin\Release\net10.0-windows\GameNet.Agent.exe")
    RequiredEnvironment=[ordered]@{
        GAMENET_DATABASE=[bool](-not [string]::IsNullOrWhiteSpace($env:GAMENET_DATABASE))
        GAMENET_RESTORE_DATABASE=[bool](-not [string]::IsNullOrWhiteSpace($env:GAMENET_RESTORE_DATABASE))
        GAMENET_AGENT_BOOTSTRAP_SECRET=[bool](-not [string]::IsNullOrWhiteSpace($env:GAMENET_AGENT_BOOTSTRAP_SECRET))
    }
}
$OutputPath=[System.IO.Path]::GetFullPath($OutputPath)
$directory=Split-Path -Parent $OutputPath
if($directory){New-Item -ItemType Directory -Path $directory -Force|Out-Null}
$evidence|ConvertTo-Json -Depth 12|Set-Content -LiteralPath $OutputPath -Encoding utf8
Write-Host "Foundation evidence written: $OutputPath"
