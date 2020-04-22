# AB.Extensions

This is a C# extensions library that I maintain to help reduce errors in code that I find in the wild. It is free of dependencies, includes some benchmarks and helpful constant strings, simple enums, and LINQ helper methods. Some methods are pulled from StackOverflow, but with fixed corner cases and added test coverage. Others are just functions that I've found helpful or grown tired of re-googling and copy-pasting. Some useful static strings and dates are scattered throughout, for those who hate [magic strings](https://softwareengineering.stackexchange.com/questions/365339/what-is-wrong-with-magic-strings) like me.

The source now officially lives in Azure DevOps, but is continuously pushed to Github via Azure Pipelines CI.

## Builds by Azure Pipelines

[![Primary Build in Azure Pipelines](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/AB.Extensions%20Github%20Project)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build?definitionId=2)

## CI Sync to Github

[![CI Sync](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/Sync%20to%20Github)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build/latest?definitionId=3)

## Install the Package

Import easily by editing your csproj:

```xml
<PackageReference Include="AB.Extensions" Version="3.0.0" />
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

## Apply Linting/Formatting

A [dotnet local tool](https://docs.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use) is present in the repo which can help keep formatting & linting adherent to the .editorconfig file, even if you don't have dotnet format installed.

```c#
dotnet tool restore;
dotnet tool run dotnet-format;
```
