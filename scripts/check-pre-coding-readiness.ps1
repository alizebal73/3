$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."

$required = @(
    "docs\planning\product-charter.md",
    "docs\planning\pre-coding-master-plan.md",
    "docs\planning\requirements-traceability.md",
    "docs\planning\definition-of-ready.md",
    "docs\planning\definition-of-done.md",
    "docs\planning\risk-register.md",
    "docs\planning\assumptions-and-open-decisions.md",
    "docs\planning\product-validation.md",
    "docs\architecture\runtime-topology.md",
    "docs\architecture\data-ownership-and-integration-map.md",
    "docs\operations\agent-transport.md",
    "docs\research\foundation-research-and-repo2-failure-analysis.md",
    "docs\research\repo2-to-repo3-foundation-gap-matrix.md",
    "docs\architecture\foundation-closure-matrix.md",
    "docs\operations\scale-capacity-slos.md",
    "docs\operations\environment-matrix.md",
    "docs\operations\database-change-policy.md",
    "docs\operations\support-diagnostics.md",
    "docs\operations\runbook-index.md",
    "docs\release\release-gates.md",
    "docs\security\data-classification-and-supply-chain.md",
    "docs\quality\quality-strategy.md",
    "docs\ux\operator-workflow-foundation.md",
    "docs\roadmap\00-pre-coding-roadmap.md",
    "docs\templates\feature-spec-template.md",
    "docs\templates\vertical-slice-template.md",
    "docs\templates\adr-template.md",
    "scripts\check-architecture.ps1",
    "scripts\check-foundation-completeness.ps1"
)

foreach ($relative in $required) {
    if (-not (Test-Path (Join-Path $root $relative))) {
        throw "Pre-coding governance artifact is missing: $relative"
    }
}

$moduleCs = Get-ChildItem (Join-Path $root "src\Server\Modules") -Recurse -File -Filter *.cs
if ($moduleCs.Count -gt 0) {
    throw "Business implementation exists under src/Server/Modules before Foundation certification."
}

$featureImpl = Get-ChildItem (Join-Path $root "src\Desktop\Features") -Recurse -File |
    Where-Object { $_.Extension -in ".cs", ".xaml" }
if ($featureImpl.Count -gt 0) {
    throw "Desktop business feature implementation exists before Foundation certification."
}

$workflowRoot = Join-Path $root ".github\workflows"
if (Test-Path $workflowRoot) {
    $files = Get-ChildItem $workflowRoot -File
    if ($files.Count -ne 1 -or $files[0].Name -ne "foundation-local.yml") {
        throw "Only foundation-local.yml is allowed during Foundation."
    }
}

$traceability = Get-Content (Join-Path $root "docs\planning\requirements-traceability.md") -Raw
if ($traceability -notmatch "Requirement ID" -or $traceability -notmatch "Acceptance criteria") {
    throw "Requirements traceability matrix is incomplete."
}

Write-Host "Pre-coding governance gate passed."
