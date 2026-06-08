#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Packs AB.Extensions into the repo-local ./feed NuGet source so you can
    smoke-test consuming the real package before publishing.

.DESCRIPTION
    A ProjectReference proves the *code* compiles; it does NOT prove the packaged
    artifact works for a consumer (TFM-specific DLLs, XML docs, dependencies, the
    license/readme, SourceLink). This script closes that gap: it packs the library
    (Release) into ./feed, which is wired up as a NuGet source in Nuget.config and
    mapped so AB.Extensions can resolve from it.

    The CoreConsoleApp20 sandbox floats to '*-*' (see Directory.Packages.props), so
    after packing here it consumes THIS build. Restore/run it to dogfood the package:

        pwsh ./scripts/pack-local.ps1
        dotnet run --project test/CoreConsoleApp20

    By default each pack gets a unique prerelease suffix (local.<timestamp>). That
    matters: NuGet caches an extracted package by id+version in the global packages
    folder, so re-packing the *same* version would be silently ignored. A fresh
    suffix every time sidesteps that trap entirely.

.PARAMETER Stable
    Pack the plain VersionPrefix (e.g. 5.0.0) with no prerelease suffix instead of
    a unique local.* build. Useful to mimic exactly what ships — but remember the
    cache caveat above if you pack the same stable version twice.

.PARAMETER Configuration
    Build configuration to pack. Default: Release (what actually ships).

.EXAMPLE
    pwsh ./scripts/pack-local.ps1
    Pack a unique 5.0.0-local.<timestamp> build into ./feed.

.EXAMPLE
    pwsh ./scripts/pack-local.ps1 -Stable
    Pack the exact shippable version (5.0.0) into ./feed.
#>
[CmdletBinding()]
param(
    [switch]$Stable,
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

# Resolve paths relative to the repo root (this script lives in <root>/scripts).
$repoRoot   = Resolve-Path (Join-Path $PSScriptRoot '..')
$project    = Join-Path $repoRoot 'src/AB.Extensions/AB.Extensions.csproj'
$feedDir    = Join-Path $repoRoot 'feed'

if (-not (Test-Path $feedDir)) { New-Item -ItemType Directory -Path $feedDir | Out-Null }

$packArgs = @(
    'pack', $project,
    '--configuration', $Configuration,
    '--output', $feedDir
)

if (-not $Stable) {
    # Unique, time-ordered prerelease label so floating '*-*' always picks the
    # newest pack and the global packages cache never goes stale.
    $suffix = 'local.' + (Get-Date -Format 'yyyyMMddHHmmss')
    $packArgs += "-p:VersionSuffix=$suffix"
    Write-Host "==> Packing AB.Extensions ($Configuration, prerelease suffix '$suffix') into ./feed" -ForegroundColor Cyan
}
else {
    Write-Host "==> Packing AB.Extensions ($Configuration, stable version) into ./feed" -ForegroundColor Cyan
}

dotnet @packArgs
if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed (exit $LASTEXITCODE)." }

$nupkg = Get-ChildItem $feedDir -Filter 'AB.Extensions.*.nupkg' |
    Where-Object { $_.Name -notlike '*.symbols.nupkg' } |
    Sort-Object LastWriteTime -Descending | Select-Object -First 1

Write-Host ""
Write-Host "Packed: $($nupkg.Name)" -ForegroundColor Green
Write-Host "Feed:   $feedDir"
Write-Host ""
Write-Host "Smoke-test it by consuming the package:" -ForegroundColor White
Write-Host "    dotnet run --project test/CoreConsoleApp20"
