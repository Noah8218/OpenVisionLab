# OpenVisionLab Sample Picker Accessibility Fix

Date: 2026-08-29 KST

Status: Complete

Scope: Keep the Sample Catalog's sample list and explicit bottom actions reachable at the default window size and after maximize/restore. Preserve the main shell's existing monitor work-area behavior and all explicit Preview/Run, layer, and routing contracts.

Source boundary: `C:\Git\OpenVisionLab_Dev` at `bcb3d6aa`, with pre-existing unrelated dirty worktree changes preserved. No original-repository, commit, push, release, or deployment action was performed.

## Root Cause

- The left Sample Catalog column placed six filter sections in `Auto` rows before the only `*` sample-list row. At the default dialog height, the filters consumed the available height and collapsed the actual sample list out of the usable viewport.
- The main shell handled `WM_GETMINMAXINFO` against the active monitor work area, but the Sample Catalog window did not. Maximizing that borderless window therefore used the full monitor bounds and placed the bottom action strip behind the Windows taskbar.

## Change

- The filter block now owns a bounded, independently scrollable region. The sample list owns a separate `2*` region with a 140-DIP minimum, so filter growth cannot remove the core selection surface.
- The sample item template now defines its third row explicitly for the goal text.
- Monitor work-area calculation moved from the main shell's private implementation to `OpenVisionWindowWorkArea`. Both the main shell and Sample Catalog call the same owner; the old duplicate native definitions no longer remain in the shell window.
- The focused screenshot smoke now checks a usable sample-list height, all three bottom-action bounds, maximized work-area containment, and preservation of the main shell maximize contract.

## Acceptance Criteria

| Criterion | Result | Evidence |
| --- | --- | --- |
| Default Sample Catalog shows a usable sample list and the three bottom actions together | Pass | Focused smoke `1040x742`; actual EXE default dialog `832x594` at the workstation's 125% scale |
| Filter navigation remains reachable without moving the sample list out of view | Pass | Actual EXE filter-region scrolling retained the independently scrollable sample list and bottom actions |
| Maximized Sample Catalog stays inside the selected monitor working area | Pass | Actual EXE changed from the reproduced `1536x864` full-monitor bounds to `1536x826`, matching the left monitor working height; bottom actions remained visible |
| Maximize -> restore keeps list and actions reachable | Pass | Actual EXE returned to `832x594` with list and actions visible |
| Existing main-shell work-area behavior remains intact | Pass | `wpf_shell_host_window_maximized=OK`, `1920x1032` |
| Opening, scrolling, maximizing, restoring, and closing the catalog causes no implicit inspection action | Pass | Actual EXE remained on the unchanged empty workspace; no Preview/Run or sample-open action was invoked |

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed after the UI source change with 0 warnings and 0 errors.
- `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug`: passed for the final focused-smoke source with one pre-existing nullable warning at `Program.cs:10691`, outside this diff.
- Focused smoke targets, all passed:
  - `wpf_shell_host_workspace_sample_picker=OK`
  - `wpf_shell_host_workspace_sample_picker_maximized=OK`
  - `wpf_shell_host_window_maximized=OK`
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`: passed all 13 contracts.
- `git diff --check` on the five changed implementation/test files: passed; only existing LF-to-CRLF notices were emitted.
- Actual EXE: `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe` on `\\.\DISPLAY2`, monitor bounds `-1920,365,1920x1080`, working area `-1920,365,1920x1032`.

## Evidence

- Before/current-source baseline: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ux-audit-20260829\current_source_core\wpf_shell_host_workspace_sample_picker.png`
- After/default: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ux-sample-picker-20260829\focused-smoke-final\wpf_shell_host_workspace_sample_picker.png`
- After/maximized: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ux-sample-picker-20260829\focused-smoke-final\wpf_shell_host_workspace_sample_picker_maximized.png`
- Main-shell regression: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ux-sample-picker-20260829\focused-smoke-final\wpf_shell_host_window_maximized.png`

## Boundary / Next Dependency

- The actual workstation path exercised the current Korean dark Sample Catalog at the available 125% Windows scale. The direct WPF capture also exercised 96-DPI layout. Separate 150%, 175%, and 200% workstation sessions were not available and remain unverified.
- A later attempt to rebuild the moving dirty worktree was blocked by a newly edited, unrelated `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs:343` definite-assignment error. That file changed after the successful full build and was not modified for this task. The scoped UI binary and focused smoke correspond to the successful build recorded above.
