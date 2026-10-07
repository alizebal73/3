param(
    [Parameter(Mandatory = $true)]
    [string]$Root,

    [string]$ExpectedPublisher
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

if ([System.Environment]::OSVersion.Platform -ne [System.PlatformID]::Win32NT) {
    throw "Release signature verification is Windows-only."
}

$rootPath = (Resolve-Path $Root).Path
$files = Get-ChildItem $rootPath -Recurse -File |
    Where-Object { $_.Extension -in ".exe", ".dll" }

if ($files.Count -eq 0) {
    throw "No signable binaries were found under '$rootPath'."
}

foreach ($file in $files) {
    $signature = Get-AuthenticodeSignature -FilePath $file.FullName

    if ($signature.Status -ne "Valid") {
        throw "Invalid or missing Authenticode signature: $($file.FullName) [$($signature.Status)]"
    }

    if (-not [string]::IsNullOrWhiteSpace($ExpectedPublisher) -and
        $signature.SignerCertificate.Subject -notlike "*$ExpectedPublisher*") {
        throw "Unexpected signer for '$($file.FullName)': $($signature.SignerCertificate.Subject)"
    }
}

Write-Host "Verified $($files.Count) signed release binaries."
