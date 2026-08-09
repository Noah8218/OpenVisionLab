# Recipe Manager And Pipeline Entry UX

Date: 2026-08-09 KST
Status: Complete in Dev

## Scope

- Keep the Recipe name editor and Create, Duplicate, Rename, and Delete actions
  together at the top of Recipe Manager summary.
- Make entered Recipe names readable in the current dark Recipe Manager.
- Diagnose and reduce the delay from Recipe Manager to Pipeline Review.
- Preserve explicit Preview/Run, layer, active-layer, and routing behavior.

## Reproduced Baseline

- The Recipe lifecycle strip was the final row of Recipe Manager, below the
  library and summary workbench.
- The host assigned dark background and light foreground values to the Recipe
  name `TextBox`, but the inherited Wpf.Ui light control template rendered its
  own white field surface. The resulting light text on a light surface made the
  entered name effectively invisible.
- The two-Step `P255_Novice_Threshold_Blob_17824` Pipeline first opened in a
  separate top-level window. Actual-EXE readiness took 860 ms. Internal timing
  was 329 ms, including 201 ms in floating-window creation/show; same-context
  floating reopen took 99-105 ms.
- The reported three-second delay was not reproduced on this workstation. The
  measured path still exposed an avoidable first-open top-level window cost.

## Change

- Recipe Manager now owns one scoped dark `TextBox` template. Its normal,
  hover, keyboard-focus, read-only, disabled, and validation-error states use
  the existing Shell field roles instead of the platform light template.
- The lifecycle strip is before the library/summary workbench. Wide and Compact
  layouts keep the name editor and all four actions on that top row.
- Pipeline Review defaults to the existing central document workspace. This
  avoids constructing and positioning a separate top-level window on the normal
  Recipe Manager entry path. The explicit Float command remains available; a
  Pipeline Review that the operator floats continues to restore as floating.
- Returning from a centrally docked Pipeline Review keeps the same document
  attached while the central panel is suspended. Same-context reopen does not
  rebuild or reattach its document tree.

## Current Result

- The same two-Step actual-EXE first open took 661 ms and remained responsive.
  Internal activation completed in 214 ms, down from 329 ms.
- Three same-context central reopens reached the visible Run Review control in
  311-322 ms. The internal reopen path completed in 10-12 ms; the remaining time
  is WPF visibility/layout and UI Automation discovery, not document rebuild or
  Pipeline execution.
- The actual-EXE Wide and Compact captures show the entered name with clear
  contrast. `HostRecipeNameEditor`, Create, Duplicate, Rename, and Delete were
  all visible and on-screen in both layouts.
- Opening and reopening remained in the unexecuted state. No Preview/Run,
  layer, active-layer, or routing mutation was introduced.

## Verification

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_shell_host_recipe_manager_summary,manual_en_recipe_manager,wpf_tool_window_dock_float_cycle" "D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-pipeline-20260809\focused-final"
dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
git diff --check
```

- Debug build: 0 warnings, 0 errors.
- Korean and English Recipe Manager summary smokes: passed; layout/text/internal
  issue counts were all zero. The smoke also checks lifecycle-strip order,
  rendered field contrast, and the required field-state triggers.
- Pipeline central dock -> float -> dock -> Return to Recipe -> reopen: passed.
- Readiness: 13/13 contracts passed.
- Actual EXE used the dynamically selected smaller left monitor
  `\\.\DISPLAY2`, bounds `-1920,365 1920x1080`; the `1600x900` window rectangle
  intersected that monitor. Test runtime state, TEMP/TMP, captures, and timings
  were stored on `D:`.

## Evidence

- Before actual EXE:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-pipeline-20260809\before-heavy`.
- Current actual EXE:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-pipeline-20260809\after-final`.
- Focused Korean/English and dock-cycle smoke:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-pipeline-20260809\focused-final`.

## Closure Record

```text
Status: Complete
Scope: Dev Recipe Manager name/lifecycle layout and normal Pipeline Review entry/reopen UX
Acceptance criteria: entered Recipe name is readable; name and four lifecycle actions stay above the workbench in Wide/Compact; first Pipeline entry no longer creates a floating window; same-context reopen reuses the central document; no Preview/Run, layer, active-layer, or routing side effect
Verification: Debug build 0 warnings/errors; Korean/English Recipe Manager and dock-cycle smokes passed; readiness 13/13; actual-EXE first open 661 ms and central reopens 311-322 ms, all responsive; Wide/Compact required controls 5/5 visible
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe-manager-pipeline-20260809 and this report
Boundary / next dependency: the reported three-second delay was not reproduced; measurements prove this workstation and the tested 0-Step/2-Step paths, not every PC or an arbitrarily large Pipeline. Original-repository promotion, commit, and push were not requested.
```
