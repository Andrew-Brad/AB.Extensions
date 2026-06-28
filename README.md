# AB.Extensions

[![CI](https://github.com/Andrew-Brad/AB.Extensions/actions/workflows/build.yml/badge.svg)](https://github.com/Andrew-Brad/AB.Extensions/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/AB.Extensions.svg)](https://www.nuget.org/packages/AB.Extensions/)
[![Downloads](https://img.shields.io/nuget/dt/AB.Extensions.svg)](https://www.nuget.org/packages/AB.Extensions/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)

A dependency-free collection of simple C# extension methods and handy
constants. Some are sharpened versions of StackOverflow snippets; others are performance-tweaked beyond the normal versions you'd see.

Built and tested on GitHub Actions across Linux and Windows, multi-targeting
**`netstandard2.0`** (for Legacy compatibility) through **`net8.0` / `net9.0` / `net10.0`**.

## Install

```shell
dotnet add package AB.Extensions
```

Or add a `PackageReference` to your `.csproj` (see the NuGet badge above for the
current version):

```xml
<PackageReference Include="AB.Extensions" Version="5.0.0" />
```

## Usage

```csharp
using AB.Extensions;

// Reverse a string
"weRdNab".ToReverseString();                 // "baNdRew"

// Parse a Guid forgivingly — invalid input returns Guid.Empty instead of throwing
"d2650790-bc80-41a9-ae63-dd55e2240296".ToGuid();   // the parsed Guid
"not-a-guid".ToGuid();                              // Guid.Empty

// Count occurrences of a substring
"lol".CountOccurrencesOf("l");               // 2

// Human-readable file sizes
17179869184UL.FileSizeString();              // "16 GB"
46080UL.FileSizeString();                    // "45 KB"

// Split CSV while respecting quoted commas (quotes are preserved on the field)
@"111,222,""33,44,55"",666".SplitQuotedCsv();   // 111 | 222 | "33,44,55" | 666

// Split on any OS line ending (\r\n, \n, or \r) in one call
"line1\r\nline2\nline3".SplitStringByLineBreaks();   // ["line1", "line2", "line3"]

// Parse a string straight into an enum
"Ascending".ToEnumTypeOf<OrderByDirection>();        // OrderByDirection.Ascending

// Add business days, skipping weekends (negative subtracts)
DateOnly.FromDateTime(DateTime.Today).AddWorkdays(5);   // netstandard2.0 takes a DateTime instead
```

## Required Local Tooling

This repo uses [dotnet local tools](https://docs.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use), pinned in [`.config/dotnet-tools.json`](.config/dotnet-tools.json). Restore them once after cloning:

```shell
dotnet tool restore
```

| Tool | Purpose |
|------|---------|
| `dotnet-format` | Keeps formatting/linting adherent to `.editorconfig`. |
| `dotnet-reportgenerator-globaltool` | Turns raw coverage data into an HTML report + summaries (used by the coverage script below). |

To run an SDK that builds and tests the library, you need the **.NET 8, 9, and 10** runtimes installed (the test project multi-targets all three).

## Build, Test & Coverage

One command builds across every TFM and runs the suite, printing a compact summary
(it's quiet on success and surfaces only the failing lines on error). Works in Git
Bash on Windows as well as Linux/macOS:

```shell
scripts/build-test.sh            # Release (what ships)
scripts/build-test.sh -c Debug   # Debug
scripts/build-test.sh --verbose  # also stream raw dotnet output
```

Or run the steps directly:

```shell
# Build everything (all TFMs: netstandard2.0, net8.0, net9.0, net10.0)
dotnet build AB.Extensions.sln -c Release

# Run the suite (executes once per target framework: net8.0, net9.0, net10.0)
dotnet test test/AB.Extensions.Tests/AB.Extensions.Tests.csproj -c Release
```

### Code coverage

Coverage is produced and validated by a single cross-platform script,
[`scripts/coverage.ps1`](scripts/coverage.ps1) — the **exact same script CI runs**,
so local results match the pipeline. It runs the tests in Debug (accurate line
mapping), builds an HTML report, and fails if line coverage drops below a floor.

```shell
# Default: 90% floor, net10.0, prints a summary
pwsh ./scripts/coverage.ps1

# Raise the bar and open the HTML report in your browser
pwsh ./scripts/coverage.ps1 -Threshold 95 -Open
```

The HTML report lands in `coverage-report/index.html` (git-ignored). The
threshold is a **floor to ratchet upward** as coverage improves — raise it as
tests are added; never lower it.

### Dogfood the package locally

A `ProjectReference` proves the code compiles; it doesn't prove the *packaged*
artifact works for a consumer (the per-TFM DLLs, XML docs, dependencies,
license/readme, SourceLink). To close that gap, pack the library into the repo-local
[`feed/`](feed/) NuGet source and consume it from the `CoreConsoleApp20` sandbox:

```shell
# Pack AB.Extensions into ./feed (unique prerelease version each run)
pwsh ./scripts/pack-local.ps1

# Consume the just-packed build and run the sandbox
dotnet run --project test/CoreConsoleApp20
```

This works because [`Nuget.config`](Nuget.config) maps `AB.Extensions` to the local
feed (with nuget.org as fallback) and [`Directory.Packages.props`](Directory.Packages.props)
floats the sandbox's reference to the newest available build — your local pack when
present, otherwise the latest release on nuget.org. See [`feed/README.md`](feed/README.md)
for details.

## Apply Linting/Formatting

```shell
dotnet tool restore
dotnet tool run dotnet-format
```

## Continuous Integration

CI runs on GitHub Actions (status badge at the top):

- **[Build & Test](.github/workflows/build.yml)** — Release build and the full suite
  on **Linux + Windows**, run once per target framework (`net8.0`, `net9.0`,
  `net10.0`).
- **[Coverage](.github/workflows/build.yml)** — the same
  [`scripts/coverage.ps1`](scripts/coverage.ps1) developers run locally, enforcing
  the line-coverage floor and publishing the HTML report as a build artifact.
- **[Publish](.github/workflows/publish.yml)** — packs and pushes to NuGet.org
  (package **and** symbols) when a version tag is pushed.

### Cutting a release

The **git tag is the source of truth** for the version — there's no manual csproj
bump. Tag and push:

```shell
git tag v5.0.0
git push origin v5.0.0
```

The publish workflow parses the version from the tag, builds + tests, packs with
reproducible-build flags, pushes to NuGet.org, and opens a GitHub Release with
auto-generated notes. It requires a repository secret **`NUGET_API_KEY`** (an
api.nuget.org push key). Publishing is also available manually from the Actions tab
(`workflow_dispatch`).

## License

[MIT](LICENSE.txt) © Andrew Brad
