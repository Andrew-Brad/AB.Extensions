# Local NuGet feed

This folder is a **local NuGet source** used to dogfood the `AB.Extensions`
package before it's published to nuget.org.

A `ProjectReference` only proves the *code* compiles. Consuming the packed
`.nupkg` from here proves the *package* works for a real consumer — the
TFM-specific DLLs, XML docs, dependency graph, license/readme, and SourceLink.

## How it's wired

- [`Nuget.config`](../Nuget.config) registers this folder as the `local` source and
  maps `AB.Extensions` to it (plus nuget.org as fallback).
- [`Directory.Packages.props`](../Directory.Packages.props) floats `AB.Extensions`
  to `*-*`, so the [`CoreConsoleApp20`](../test/CoreConsoleApp20) sandbox consumes
  the newest build available — a local pack from here when present, else the latest
  release on nuget.org.

## Usage

```shell
# Pack the library into this folder (unique prerelease version each run)
pwsh ./scripts/pack-local.ps1

# Consume it — restores the just-packed build and runs the sandbox
dotnet run --project test/CoreConsoleApp20
```

The packed `*.nupkg` / `*.snupkg` files are git-ignored (see
[`.gitignore`](../.gitignore)); only this README is tracked, to keep the folder
present so the `local` source always resolves.
