# CLAUDE.md

Guidance for Claude Code in this repo. Build/test/coverage mechanics live in the
[README](README.md) — not repeated here. This file holds only what isn't obvious
from the README or the code.

## Build & test verification

For any build/test verification, delegate to the **`build-test`** subagent rather
than running `dotnet` inline. It runs `scripts/build-test.sh` in an isolated context
and returns only a compact pass/fail summary, keeping verbose MSBuild/test output out
of the main conversation. Reach for it after edits to confirm the change compiles and
tests pass across all TFMs.

Exception: a single one-off check where the script's own output is already terse —
running `scripts/build-test.sh` directly is cheaper than a subagent cold start.
