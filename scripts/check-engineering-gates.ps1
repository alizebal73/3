$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$workflowPath = Join-Path $root '.github\workflows\foundation-local.yml'
$verifyPath = Join-Path $root 'scripts\verify.ps1'
$solutionPath = Join-Path $root 'GameNet.slnx'
$constitutionPath = Join-Path $root 'docs\development\engineering-constitution.md'
$branchPolicyPath = Join-Path $root 'docs\operations\github-settings.md'
$branchingPath = Join-Path $root 'docs\development\branching.md'

foreach ($path in @($workflowPath, $verifyPath, $solutionPath, $constitutionPath, $branchPolicyPath, $branchingPath)) {
    if (-not (Test-Path $path)) {
        throw "Engineering gate input is missing: $path"
    }
}

$workflow = Get-Content $workflowPath -Raw
$workflowRequirements = @(
    'workflow_dispatch:',
    'push:',
    'foundation/engineering-complete',
    'pull_request:',
    'main',
    'runs-on: [self-hosted, Windows, X64]',
    '.\scripts\verify.ps1'
)
foreach ($required in $workflowRequirements) {
    if (-not $workflow.Contains($required)) {
        throw "Foundation workflow is missing required gate contract text: $required"
    }
}

$verify = Get-Content $verifyPath -Raw
$verifyRequirements = @(
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @("--version")',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @("restore", "GameNet.slnx")',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @("build", "GameNet.slnx", "--configuration", "Release", "--no-restore")',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @("test", "GameNet.slnx", "--configuration", "Release", "--no-build", "--no-restore")'
)
foreach ($required in $verifyRequirements) {
    if (-not $verify.Contains($required)) {
        throw "Canonical verify.ps1 is missing required command: $required"
    }
}

foreach ($docRule in @(
    @{ Path = $branchPolicyPath; Forbidden = @('- develop is the integration branch.', 'manual-only') },
    @{ Path = $branchingPath; Forbidden = @('long-lived `develop` branch') }
)) {
    $docText = Get-Content $docRule.Path -Raw
    foreach ($forbidden in $docRule.Forbidden) {
        if ($docText.Contains($forbidden)) {
            throw "Governance contradiction detected in $($docRule.Path): $forbidden"
        }
    }
}

[xml]$solution = Get-Content $solutionPath -Raw
$solutionDirectory = Split-Path -Parent $solutionPath
$solutionProjects = @(
    $solution.Solution.Project |
        ForEach-Object {
            $candidate = Join-Path $solutionDirectory ([string]$_.Path)
            [System.IO.Path]::GetFullPath($candidate).Replace([string][char]92, '/')
        }
)

$testProjects = @(Get-ChildItem (Join-Path $root 'tests') -Recurse -File -Filter *.csproj)
if ($testProjects.Count -eq 0) {
    throw 'No test projects were found under tests.'
}

$certificationCount = 0
foreach ($project in $testProjects) {
    $projectFullPath = [System.IO.Path]::GetFullPath($project.FullName)
    $relative = [System.IO.Path]::GetRelativePath($root, $projectFullPath).Replace([string][char]92, '/')
    [xml]$projectXml = Get-Content $project.FullName -Raw

    $isTestProject = @($projectXml.Project.PropertyGroup.IsTestProject) |
        Where-Object { "$($_)" -eq 'true' } |
        Select-Object -First 1
    if (-not $isTestProject) {
        throw "Test project is not explicitly marked IsTestProject=true: $relative"
    }

    $packageNames = @($projectXml.Project.ItemGroup.PackageReference | ForEach-Object { [string]$_.Include })
    if ($packageNames -notcontains 'Microsoft.NET.Test.Sdk') {
        throw "Test project is missing Microsoft.NET.Test.Sdk: $relative"
    }
    if ($packageNames -notcontains 'xunit') {
        throw "Test project is missing xunit: $relative"
    }

    $isCertificationProject = @($projectXml.Project.PropertyGroup.GameNetCertificationProject) |
        Where-Object { "$($_)" -eq 'true' } |
        Select-Object -First 1

    if ($isCertificationProject) {
        $certificationCount++
        if ($solutionProjects -contains $projectFullPath.Replace([string][char]92, '/')) {
            throw "Environment certification project must not be part of the normal GameNet.slnx test gate: $relative"
        }
    }
    elseif ($solutionProjects -notcontains $projectFullPath.Replace([string][char]92, '/')) {
        throw "Normal test project is missing from GameNet.slnx: $relative"
    }
}

if ($certificationCount -eq 0) {
    throw 'At least one explicit Foundation certification test project must exist.'
}

$pgScript = Get-Content (Join-Path $root 'scripts\certify-postgresql.ps1') -Raw
if (-not $pgScript.Contains('tests\Postgres.CertificationTests\GameNet.Postgres.CertificationTests.csproj')) {
    throw 'PostgreSQL certification script is not wired to the certification test project.'
}

Write-Host 'Engineering gate contract passed: workflow, governance, canonical verification and test layers are aligned.'