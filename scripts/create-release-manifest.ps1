param(
    [Parameter(Mandatory = $true)]
    [string]$PackageRoot,

    [Parameter(Mandatory = $true)]
    [string]$ProductVersion,

    [Parameter(Mandatory = $true)]
    [int]$SchemaVersion,

    [Parameter(Mandatory = $true)]
    [string]$ApiContractVersion,

    [Parameter(Mandatory = $true)]
    [int]$AgentProtocolVersion,

    [Parameter(Mandatory = $true)]
    [int]$MigrationFromSchemaVersion,

    [Parameter(Mandatory = $true)]
    [int]$MigrationToSchemaVersion,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = (Resolve-Path $PackageRoot).Path

$files = Get-ChildItem $root -Recurse -File |
    Where-Object {
        $_.FullName -notmatch "\(manifest\.json|manifest\.sha256)$"
    } |
    Sort-Object FullName

$entries = foreach ($file in $files) {
    $relative = [System.IO.Path]::GetRelativePath($root, $file.FullName)
    $relative = $relative.Replace("\", "/")
    $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $file.FullName).Hash.ToLowerInvariant()

    [ordered]@{
        RelativePath = $relative
        Sha256 = $hash
        SizeBytes = $file.Length
    }
}

$manifest = [ordered]@{
    ProductVersion = $ProductVersion
    SchemaVersion = $SchemaVersion
    ApiContractVersion = $ApiContractVersion
    AgentProtocolVersion = $AgentProtocolVersion
    MigrationFromSchemaVersion = $MigrationFromSchemaVersion
    MigrationToSchemaVersion = $MigrationToSchemaVersion
    Files = @($entries)
}

$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
$directory = Split-Path -Parent $OutputPath
if ($directory) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $OutputPath -Encoding utf8

Write-Host "Release manifest created: $OutputPath"
