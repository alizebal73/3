$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
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
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("restore", "GameNet\\.slnx"\)',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("build", "GameNet\\.slnx".*Release',
    'Invoke-Checked -FilePath "dotnet" -ArgumentList @\("test", "GameNet\\.slnx".*Release'
)) {
    if ($verify -notmatch $pattern) {
        throw "Canonical verify.ps1 is missing required command: $pattern"
    }
}

[xml]$solution = Get-Content $solutionPath -Raw
$solutionProjects = @($solution.Solution.Project | ForEach-Object { [string]$_.Path })

$testProjects = @(Get-ChildItem (Join-Path $root "tests") -Recurse -File -Filter *.csproj)
if ($testProjects.Count -eq 0) {
    throw "No test projects were found under tests."
}

foreach ($project in $testProjects) {
    $relative = $project.FullName.Substring($root.Length + 1).Replace([string][char]92, "/")
    [xml]$projectXml = Get-Content $project.FullName -Raw
    $isTestProject = @($projectXml.Project.PropertyGroup.IsTestProject) |
        Where-Object { "$_" -eq "true" } |
        Select-Object -First 1

    if (-not $isTestProject) {
        throw "Test project is not explicitly marked IsTestProject=true: $relative"
    }

    $packageText = Get-Content $project.FullName -Raw
    if ($packageText -notmatch 'Microsoft.NET.Test.Sdk') {
        throw "Test project is missing Microsoft.NET.Test.Sdk: $relative"
    }

    if ($packageText -notmatch '<PackageReference Include="xunit"') {
        throw "Test project is missing xunit: $relative"
    }

    if ($solutionProjects -notcontains $relative) {
        throw "Test project is missing from GameNet.slnx: $relative"
    }
}

Write-Host "Engineering gate contract passed: workflow, canonical verification and all test projects are aligned."
