# AB.Extensions

This is a C# extensions library that I maintain to help reduce errors in code that I find in the wild. It is free of dependencies, includes tests, benchmarks and light commentary on usage. Some methods are pulled from StackOverflow, but with fixed corner cases and added test coverage. Others are just functions that I've found helpful or grown tired of re-googling and copy-pasting. Some useful constant strings and dates are scattered throughout, for those who hate [magic strings](https://softwareengineering.stackexchange.com/questions/365339/what-is-wrong-with-magic-strings) like me.

The source now officially lives in Azure DevOps, but is continuously pushed to Github via Azure Pipelines CI.

## Builds by Azure Pipelines

[![Primary Build in Azure Pipelines](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/AB.Extensions%20Github%20Project)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build?definitionId=2)

## CI Sync to Github

[![CI Sync](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/Sync%20to%20Github)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build/latest?definitionId=3)

## Install the Package

Import easily by editing your csproj:

```xml
<PackageReference Include="AB.Extensions" Version="4.0.0" />
```

Alternatively with dotnet CLI:

```c#
dotnet add package AB.Extensions
```

## CI Packaging Notes

The Azure Artifacts feed which hosts the prerelease packages (uploaded by CI) is publically available [here](https://zep519.pkgs.visualstudio.com/_packaging/Ab.Extensions-CI/nuget/v3/index.json).

If you prefer Myget, those are located [here](https://www.myget.org/F/andrew-ci/api/v3/index.json).

Release versions are automatically uploaded to Nuget.org by CI under the following conditions:

- master branch
- all previous steps succeeded in the build
- manual queue of build
- when manually queuing the build, a variable of name **PushReleaseNuget** should be provided with value **confirm**.

This means that when making new branches for code modifications, it's a good practice to immediately identify the desired SemVer in the csproj metadata, and ensure the code change adheres accordingly.

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

```shell
# Build everything
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
# Default: 40% floor, net10.0, prints a summary
pwsh ./scripts/coverage.ps1

# Raise the bar and open the HTML report in your browser
pwsh ./scripts/coverage.ps1 -Threshold 45 -Open
```

The HTML report lands in `coverage-report/index.html` (git-ignored). The
threshold is a **floor to ratchet upward** as coverage improves — raise it as
tests are added; never lower it.

## Apply Linting/Formatting

```shell
dotnet tool restore
dotnet tool run dotnet-format
```
