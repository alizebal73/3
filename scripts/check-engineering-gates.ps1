$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$workflowPath = Join-Path $root ".github\workflows\foundation-local.yml"
$verifyPath = Join-Path $root "scripts\verify.ps1"
$solutionPath = Join-Path $root "GameNet.slnx"

foreach ($path in @($workflowPath, $verifyPath, $solutionPath)) {
    if (-not (Test-Path $path)) {
        throw "Engineering gate input is missing: $path"
    }
}

$workflow = Get-Content $workflowPath -Raw
foreach ($pattern in @(
    "workflow_dispatch:",
    "push:\s*branches:\s*- foundation/engineering-complete",
    "pull_request:\s*branches:\s*- main",
    "runs-on:\s*\[self-hosted, Windows, X64\]",
    "scripts\\verify\.ps1"
)) {
    if ($workflow -notmatch $pattern) {
        throw "Foundation workflow is missing required gate contract: $pattern"
    }
}

$verify = Get-Content $verifyPath -Raw
foreach ($pattern in @(
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("--version"\)',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("restore", "GameNet\.slnx"\)',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("build", "GameNet\.slnx".*Release',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("test", "GameNet\.slnx".*Release'
)) {
    if ($verify -notmatch $pattern) {
        throw "Canonical verify.ps1 is missing required command: $pattern"
    }
}

[xml]$solution = Get-Content $solutionPath -Raw
$solutionDirectory = Split-Path -Parent $solutionPath
$solutionProjects = @(
    $solution.Solution.Project |
        ForEach-Object {
            $candidate = Join-Path $solutionDirectory ([string]$_.Path)
            [System.IO.Path]::GetFullPath($candidate).Replace([string][char]92, "/")
        }
)

$testProjects = @(Get-ChildItem (Join-Path $root "tests") -Recurse -File -Filter *.csproj)
if ($testProjects.Count -eq 0) {
    throw "No test projects were found under tests."
}

foreach ($project in $testProjects) {
    $projectFullPath = [System.IO.Path]::GetFullPath($project.FullName)
    $relative = [System.IO.Path]::GetRelativePath($root, $projectFullPath).Replace([string][char]92, "/")
    [xml]$projectXml = Get-Content $project.FullName -Raw

    $isTestProject = @($projectXml.Project.PropertyGroup.IsTestProject) |
        Where-Object { "$_" -eq "true" } |
        Select-Object -First 1
    if (-not $isTestProject) {
        throw "Test project is not explicitly marked IsTestProject=true: $relative"
    }

    $packageText = Get-Content $project.FullName -Raw
    if ($packageText -notmatch '<PackageReference Include="Microsoft\.NET\.Test\.Sdk"') {
        throw "Test project is missing Microsoft.NET.Test.Sdk: $relative"
    }

    if ($packageText -notmatch '<PackageReference Include="xunit"') {
        throw "Test project is missing xunit: $relative"
    }

    $isCertificationProject = @($projectXml.Project.PropertyGroup.GameNetCertificationProject) |
        Where-Object { "$_" -eq "true" } |
        Select-Object -First 1

    if ($isCertificationProject) {
        if ($solutionProjects -contains $projectFullPath.Replace([string][char]92, "/")) {
            throw "Environment certification project must not be part of the normal GameNet.slnx test gate: $relative"
        }
    }
    elseif ($solutionProjects -notcontains $projectFullPath.Replace([string][char]92, "/")) {
        throw "Normal test project is missing from GameNet.slnx: $relative"
    }
}

$certificationProjects = @($testProjects | Where-Object {
    [xml]$xml = Get-Content $_.FullName -Raw
    @($xml.Project.PropertyGroup.GameNetCertificationProject) -contains "true"
})
if ($certificationProjects.Count -eq 0) {
    throw "At least one explicit Foundation certification test project must exist."
}

$pgScript = Get-Content (Join-Path $root "scripts\certify-postgresql.ps1") -Raw
if ($pgScript -notmatch "Postgres\.CertificationTests\\GameNet\.Postgres\.CertificationTests\.csproj") {
    throw "PostgreSQL certification script is not wired to the certification test project."
}

Write-Host "Engineering gate contract passed: workflow, canonical verification and all test projects are aligned."
