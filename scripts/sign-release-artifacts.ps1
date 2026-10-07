param(
    [Parameter(Mandatory = $true)]
    [string]$Root,

    [Parameter(Mandatory = $true)]
    [string]$CertificateThumbprint,

    [string]$TimestampServer
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if ([System.Environment]::OSVersion.Platform -ne [System.PlatformID]::Win32NT) {
    throw "Release signing is Windows-only."
}

$rootPath = (Resolve-Path $Root).Path
$thumbprint = $CertificateThumbprint.Replace(" ", "").ToUpperInvariant()

$certificate = Get-ChildItem Cert:\CurrentUser\My\$thumbprint -ErrorAction SilentlyContinue
if (-not $certificate) {
    throw "Signing certificate was not found in the current user's certificate store: $thumbprint"
}

if (-not $certificate.HasPrivateKey) {
    throw "Signing certificate does not have an accessible private key."
}

$files = @(Get-ChildItem $rootPath -Recurse -File |
    Where-Object { $_.Extension -in ".exe", ".dll" })

foreach ($file in $files) {
    $params = @{
        FilePath = $file.FullName
        Certificate = $certificate
        HashAlgorithm = "SHA256"
    }

    if (-not [string]::IsNullOrWhiteSpace($TimestampServer)) {
        $params.TimestampServer = $TimestampServer
    }

    $result = Set-AuthenticodeSignature @params

    if ($result.Status -ne "Valid") {
        throw "Signing failed for '$($file.FullName)': $($result.Status)"
    }
}

Write-Host "Signed $($files.Count) release binaries under $rootPath."
