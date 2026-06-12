#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs the test suite with code coverage, builds an HTML + summary report, and
    enforces a minimum line-coverage threshold.

.DESCRIPTION
    This is the single source of truth for code coverage, used BOTH locally and
    in CI (.github/workflows/build.yml) so the two never drift. It:

      1. Runs the tests in Debug with coverlet's "XPlat Code Coverage" collector
         (Debug gives accurate line mapping; optimized Release IL does not).
      2. Generates a human-friendly HTML report + text/markdown summaries via the
         ReportGenerator local dotnet tool.
      3. Fails (exit 1) if line coverage is below -Threshold.

    Required tooling is restored from .config/dotnet-tools.json — run
    `dotnet tool restore` once (the script does this for you).

.PARAMETER Threshold
    Minimum acceptable line-coverage percentage. Default: 90.
    This is a FLOOR to ratchet upward as coverage improves — raise it, never lower
    it, as tests are added.

.PARAMETER Framework
    Target framework to collect coverage on. Default: net10.0. Coverage measures
    source lines, so one framework is sufficient and keeps the run fast.

.PARAMETER Open
    Open the generated HTML report in the default browser when finished (local
    convenience; ignored in CI).

.EXAMPLE
    pwsh ./scripts/coverage.ps1
    Run with defaults (90% floor, net10.0) and print the summary.

.EXAMPLE
    pwsh ./scripts/coverage.ps1 -Threshold 95 -Open
    Enforce 95% and pop the HTML report open in a browser.
#>
[CmdletBinding()]
param(
    [double]$Threshold = 90,
    [string]$Framework = 'net10.0',
    [switch]$Open
)

$ErrorActionPreference = 'Stop'

# Resolve paths relative to the repo root (this script lives in <root>/scripts).
$repoRoot    = Resolve-Path (Join-Path $PSScriptRoot '..')
$testProject = Join-Path $repoRoot 'test/AB.Extensions.Tests/AB.Extensions.Tests.csproj'
$resultsDir  = Join-Path $repoRoot 'test/AB.Extensions.Tests/TestResults'
$reportDir   = Join-Path $repoRoot 'coverage-report'

Write-Host "==> Restoring local dotnet tools" -ForegroundColor Cyan
dotnet tool restore

# Clean prior results so we never report stale numbers.
foreach ($dir in @($resultsDir, $reportDir)) {
    if (Test-Path $dir) { Remove-Item $dir -Recurse -Force }
}

Write-Host "==> Running tests with coverage (Debug / $Framework)" -ForegroundColor Cyan
dotnet test $testProject `
    --configuration Debug `
    --framework $Framework `
    --collect:"XPlat Code Coverage" `
    --results-directory $resultsDir
if ($LASTEXITCODE -ne 0) { throw "Tests failed (exit $LASTEXITCODE)." }

$cobertura = Get-ChildItem $resultsDir -Recurse -Filter 'coverage.cobertura.xml' |
    Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $cobertura) { throw "No coverage.cobertura.xml was produced." }

Write-Host "==> Generating report" -ForegroundColor Cyan
dotnet tool run reportgenerator `
    "-reports:$($cobertura.FullName)" `
    "-targetdir:$reportDir" `
    "-reporttypes:Html;TextSummary;MarkdownSummaryGithub"
if ($LASTEXITCODE -ne 0) { throw "ReportGenerator failed (exit $LASTEXITCODE)." }

# Pull the authoritative line-coverage % straight from the cobertura root.
[xml]$xml = Get-Content $cobertura.FullName
$lineRate = [double]$xml.coverage.'line-rate'
$linePct  = [math]::Round($lineRate * 100, 1)

# Surface the report in the GitHub Actions job summary when running in CI.
if ($env:GITHUB_STEP_SUMMARY) {
    Get-Content (Join-Path $reportDir 'SummaryGithub.md') | Add-Content $env:GITHUB_STEP_SUMMARY
}

Write-Host ""
Write-Host "Line coverage: $linePct%  (threshold: $Threshold%)" -ForegroundColor White
Write-Host "HTML report:   $(Join-Path $reportDir 'index.html')"

if ($Open -and -not $env:GITHUB_ACTIONS) {
    $index = Join-Path $reportDir 'index.html'
    if ($IsWindows)   { Start-Process $index }
    elseif ($IsMacOS) { & open $index }
    else              { & xdg-open $index }
}

if ($linePct -lt $Threshold) {
    Write-Host "FAIL: line coverage $linePct% is below the $Threshold% threshold." -ForegroundColor Red
    exit 1
}
Write-Host "PASS: line coverage meets the $Threshold% threshold." -ForegroundColor Green
