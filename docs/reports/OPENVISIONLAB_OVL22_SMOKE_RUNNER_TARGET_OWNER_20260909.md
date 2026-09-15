# OpenVisionLab OVL-22 smoke runner target owner — 2026-09-09

Status: **Complete for one command-line target execution boundary**.

## Scope

The smoke-runner audit selected the command-line target selection and execution
path as the smallest independent responsibility in
`tools/PipelineViewerScreenshotSmoke/Program.cs`. The path has no WPF state of
its own: it receives the existing target and suite catalogs and invokes the
capture delegates owned by `Program`. The extraction therefore preserves the
existing screenshot target catalog, WPF capture methods, and product runtime
contracts while making the runner lifecycle directly testable without opening a
window.

## Ownership proof

| Boundary | Before | After |
| --- | --- | --- |
| Command-line target execution | `Program.CaptureTargets` mixed with WPF entrypoint | `ScreenshotSmokeTargetRunner.CaptureTargets` owns output directory creation, delegate invocation, result formatting, error evidence, and exit status |
| Target-name parsing and suite expansion | `Program.SplitNames` / `ExpandSuites` | `ScreenshotSmokeTargetRunner` owns trim, order, duplicate removal, and unknown-suite errors |
| Catalog presentation | `Program.PrintTargetsAndSuites` | `ScreenshotSmokeTargetRunner.PrintTargetsAndSuites` owns sorted discovery output |
| Capture delegate state | private `Program.Targets` dictionary and WPF capture methods | unchanged; `Program` passes the catalog and remains the capture-delegate owner |
| Result value | nested `Program.CaptureResult` | internal `CaptureResult` in the runner module; fields and formatting are unchanged |

The new owner has explicit inputs and no reference to `Window`, `Application`, or
the visual tree. `Program.Main` remains the composition entrypoint and delegates
`--all`, `--target`, `--suite`, `--list`, and the existing manual-language name
parsing to the runner. This is a concrete responsibility move, not a partial
file split or a line-count-driven rename.

## Observable contract preserved

- `--all` invokes every existing target in catalog order.
- `--target a,b output` keeps comma splitting, trimming, and empty-entry removal.
- `--suite a,b output` keeps suite order and case-insensitive duplicate target removal.
- Unknown targets still print an `NG` line and return a failing exit code without invoking a delegate.
- Capture exceptions still print an `NG` line, write `<target>.png.error.txt`, and return a failing exit code while allowing later targets to run.
- `--list` still prints suites and targets in case-insensitive sorted order.
- The existing `CaptureResult` width, height, and elapsed-time fields remain unchanged.

No target catalog entry, WPF capture method, Recipe/XML contract, Preview/Run
behavior, Layer/ImageSpace ownership, or PropertyGrid policy changed.

## Changed files

- `tools/PipelineViewerScreenshotSmoke/ScreenshotSmokeTargetRunner.cs`
  - New concrete owner for command-line target execution, parsing, suite
    expansion, catalog output, and the internal capture result value.
- `tools/PipelineViewerScreenshotSmoke/ScreenshotSmokeTargetRunnerContract.cs`
  - Window-free contract covering structure and success, unknown-target,
    exception, parsing, suite, and catalog-output behavior.
- `tools/PipelineViewerScreenshotSmoke/Program.cs`
  - Removes the former command-line helper methods and delegates the existing
    switches to the new owner. Adds the contract dispatch before WPF
    application creation.
- `docs/reports/OPENVISIONLAB_OVL22_SMOKE_RUNNER_TARGET_OWNER_20260909.md`
  - This bounded completion and evidence record.

Existing dirty worktree changes were preserved. Only the new owner, its focused
contract, the dispatch hunk, and this report belong to this slice; the large
pre-existing `Program.cs` changes were not staged as part of the checkpoint.

## Verification

- `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU" -m:1 -nr:false --nologo`: **0 errors**; one pre-existing `CS8600` warning remains at `Program.cs:10726`.
- `dotnet .\tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --command-line-contract D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-contract-20260909`: **10/10 passed**.
- Debug `--list` smoke invocation completed successfully and wrote its output to `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-list-debug-20260909.txt`.
- `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform="Any CPU" -m:1 -nr:false --nologo`: **0 errors**; the same pre-existing `CS8600` warning remains.
- `dotnet .\tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Release\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --command-line-contract D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-release-contract-20260909`: **10/10 passed**.
- `git diff --check` for the changed runner path: **PASS**. The repository's pre-existing mixed line-ending warning remains informational.

The focused contract report is stored at:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-contract-20260909\screenshot-smoke-target-runner-contract.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-release-contract-20260909\screenshot-smoke-target-runner-contract.txt`

This slice does not claim a physical WPF screenshot qualification. No WPF view
or visual state changed; full desktop target runs remain governed by the
existing monitor, theme, layout, and DPI matrix.

## Junior readability assessment

**PASS for this slice.** A junior maintainer can now read `Program.Main` as the
composition point, follow the command-line call to one small concrete runner,
and inspect all output/error policy in one file. The capture implementations and
catalog entries remain with the existing owner, so the extraction does not hide
the WPF behavior behind a new interface, factory, wrapper, or message bus.

## Compatibility and no-duplicate rule

Do not recreate or re-split `ScreenshotSmokeTargetRunner` unless a new command
contract, reproduced lifecycle defect, or proven responsibility conflict exists.
Do not move individual WPF capture methods by file size alone. A future smoke
slice must identify an independently owned fixture, capture, or reporting
responsibility, prove its call path, and add a focused contract before moving
code. Another model, agent, or scheduled run must not repeat this extraction or
add documentation-only follow-up work for the same boundary.

## Boundary and next priority

This closes one command-line orchestration boundary inside the smoke tooling. The
remaining `PipelineViewerScreenshotSmoke/Program.cs` capture groups are still
large, but no further split is justified by line count alone. The next run must
inspect the remaining fixture/capture/reporting groups and select at most one
independent owner with a focused proof; if no boundary is demonstrated, record
the result without changing code.

Recommended model: `gpt-5.4-mini` | Reasoning effort: `medium`.

## Completion record

```text
Status: Complete
Scope: Move PipelineViewerScreenshotSmoke command-line target execution, parsing, suite expansion, catalog output, and CaptureResult into ScreenshotSmokeTargetRunner
Acceptance: existing --all/--target/--suite/--list contracts preserved; Program no longer owns moved helpers; 10/10 Debug contract; 10/10 Release contract; Debug/Release build with 0 errors; no unrelated staged changes
Verification: PipelineViewerScreenshotSmoke Debug/Release build; ScreenshotSmokeTargetRunnerContract Debug/Release; Debug --list; git diff --check
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-contract-20260909; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-release-contract-20260909; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-list-debug-20260909.txt
Boundary: physical WPF target matrix and remaining capture groups require separate evidence; no product runtime or UI behavior changed
```
