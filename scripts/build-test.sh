#!/usr/bin/env bash
#
# build-test.sh — build + test AB.Extensions across all target frameworks.
#
# Quiet by default: streams nothing on success except a compact summary, so the
# happy path is a handful of lines. On failure it prints only the actionable
# error/warning lines (full log path is shown for drill-down). Cross-platform —
# runs in Git Bash on Windows, and on Linux/macOS shells.
#
# Usage:
#   scripts/build-test.sh                 # Release (what ships)
#   scripts/build-test.sh -c Debug        # Debug
#   scripts/build-test.sh --verbose       # also stream the raw dotnet output
#
set -euo pipefail

CONFIG="Release"
VERBOSE=0

while [ $# -gt 0 ]; do
  case "$1" in
    -c|--configuration) CONFIG="$2"; shift 2 ;;
    --verbose)          VERBOSE=1; shift ;;
    -h|--help)
      grep '^#' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
    *) echo "Unknown argument: $1" >&2; exit 2 ;;
  esac
done

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SLN="$ROOT/AB.Extensions.sln"
TEST_PROJECT="$ROOT/test/AB.Extensions.Tests/AB.Extensions.Tests.csproj"
LOG="$(mktemp)"
trap 'rm -f "$LOG"' EXIT

# Run a dotnet command into the log; on failure print the distilled error lines.
run_step() {
  local label="$1"; shift
  echo "==> $label"
  if [ "$VERBOSE" -eq 1 ]; then
    "$@" 2>&1 | tee "$LOG"
  else
    "$@" >"$LOG" 2>&1
  fi
}

# --- Build (all TFMs: netstandard2.0;net8.0;net9.0;net10.0) ------------------
if ! run_step "Build ($CONFIG, all TFMs)" \
     dotnet build "$SLN" -c "$CONFIG" --nologo --verbosity minimal; then
  echo "BUILD FAILED — log: $LOG"
  grep -E ": (error|warning) [A-Z]" "$LOG" || tail -n 30 "$LOG"
  trap - EXIT   # keep the log around for inspection
  exit 1
fi
WARN_COUNT="$(grep -cE ": warning [A-Z]" "$LOG" || true)"

# --- Test (net8.0/net9.0/net10.0; reuse build output) ------------------------
if ! run_step "Test ($CONFIG, net8.0/net9.0/net10.0)" \
     dotnet test "$TEST_PROJECT" -c "$CONFIG" --no-build --nologo; then
  echo "TESTS FAILED — log: $LOG"
  grep -E "(Failed!|Failed +!|: error|\[FAIL\]|^\s+Failed )" "$LOG" || tail -n 40 "$LOG"
  trap - EXIT
  exit 1
fi

# --- Compact summary ---------------------------------------------------------
echo ""
echo "Build:  0 errors / ${WARN_COUNT} warnings  (netstandard2.0;net8.0;net9.0;net10.0)"
echo "Tests:"
grep -E "Passed!|Failed!" "$LOG" | sed 's/^/  /' || echo "  (no test summary parsed)"
echo ""
echo "ALL GREEN ($CONFIG)"
