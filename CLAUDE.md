# CLAUDE.md

Shared context for both humans and AI agents working in this repo. This file
captures the *conventions, decisions, and direction* that aren't obvious from the
README or the code.

**The [README](README.md) is the single source of truth for how to build, test,
measure coverage, pack, and release.** It is deliberately thorough and kept current —
treat it as authoritative. Before running any build/test/coverage/packaging/release
command, read the relevant README section and follow it exactly rather than improvising
or relying on memory. If something here ever appears to conflict with the README, the
README wins and this file should be corrected. Do not duplicate README mechanics into
this file.

## What this project is

AB.Extensions is a dependency-free C# library of extension methods and constants,
published to NuGet. It aims to set a high, current bar for .NET library packaging best practices:
modern targeting, a clean documented developer experience, zero-warning builds, and
honest nullable annotations. When making changes, optimize for code that reads as
idiomatic, current, expert-level .NET.

## Targeting

The README states the supported framework matrix; don't restate or second-guess it
here. The rationale behind it:

- `netstandard2.0` is kept for reach; the modern TFMs exist for current surface and
  BCL access. The test project mirrors the library's modern TFMs so each test build
  exercises its matching library build.
- Console sandboxes (`CoreConsoleApp20`, `AB.Benchmarks.ConsoleApp`) target the current
  .NET.
  These are **kept intentionally** — there is a longer-term vision for performance and
  benchmarking work, so the benchmark project is strategic, not disposable. (The
  `CoreConsoleApp20` name is a historical artifact post-retarget; renaming is optional.)

## Conventions

- **Nullable reference types are enabled repo-wide.** Annotate the public surface
  honestly (`string?`, `IComparer<TKey>?`, `Random?`). Test `InlineData(null)` cases
  use `string?` parameters.
- **Zero warnings is the standard** across all four TFMs. `GenerateDocumentationFile`
  is **library-only** (tests/console don't get spurious CS1591). Resolve missing-doc
  warnings by *documenting* the member, not by suppressing.
- `LangVersion=latest`, `AnalysisLevel=latest`, `EnforceCodeStyleInBuild`,
  `ImplicitUsings` are on.
- **Central Package Management** is in force: versions live in
  `Directory.Packages.props`; never add `Version=` to a `PackageReference`. Shared
  authoring/language/SourceLink settings live in `Directory.Build.props`.
- **`.editorconfig`** is based on the official Microsoft template
  (`dotnet new editorconfig`) — that's the agreed upstream source for future refreshes.
  Local deviations are marked with `AB:` comments (e.g. `insert_final_newline = true`,
  and a `[*_line_ending.txt]` guard protecting the line-ending test fixtures).
- **Line-ending fixtures** (`*_line_ending.txt`) are pinned in `.gitattributes`
  (`eol=lf` / `eol=crlf` / `-text`). Don't let them get renormalized.

## API surface & versioning

- **Prefer the BCL over reinventing it.** Members the modern BCL covers have been
  removed or `#if NETSTANDARD2_0`-gated (e.g. `DistinctBy`, `Random.Shared` vs the old
  `ThreadSafeRandom`, `DateTimeOffset` over a hand-rolled Unix-timestamp helper). When
  adding helpers, check whether net8+ already provides it and gate the polyfill to
  `netstandard2.0` only.

## Developer experience (a priority)

A smooth, **documented** local developer experience is a first-class goal — not just
working CI. The README documents the actual tools, scripts, and flows; honor and
preserve them. The principles that must hold as the repo evolves:

- Local and CI must run the **same** scripts so the two never drift; "what CI does" is
  always reproducible locally.
- Any new `dotnet` tool a workflow needs MUST be added to the local tool manifest **and**
  documented in the README in the same change — never one without the other.
- Keep the local-feed dogfooding loop working, and keep the README the place that
  explains how to use it.

## Build & test verification

For any build/test verification, delegate to the **`build-test`** subagent rather than
running `dotnet` inline. It runs `scripts/build-test.sh` in an isolated context and
returns only a compact pass/fail summary, keeping verbose MSBuild/test output out of
the main conversation. Reach for it after edits to confirm the change compiles and
tests pass across all TFMs.

Exception: a single one-off check where the script's own output is already terse —
running `scripts/build-test.sh` directly is cheaper than a subagent cold start.

## Outstanding work

Remaining design/correctness cleanups (the tail of the modernization effort):

- `ConsoleExtensions` isn't actually extension methods / not static — rename/reshape.
- `FileSizeString` boundary (`>` vs `>=`) and precision review.
