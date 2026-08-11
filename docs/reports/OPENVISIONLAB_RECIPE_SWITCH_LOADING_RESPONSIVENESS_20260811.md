# OpenVisionLab Recipe Switch Loading Lifetime

Date: 2026-08-11 KST

Repositories: `C:\Git\OpenVisionLab_Dev` and `C:\Git\OpenVisionLab`

Lifecycle-correction baseline: `42d840a9`

## Correction

The first implementation proved that the Recipe loading overlay appeared, but
it closed before deferred Pipeline Review preparation finished. The user's
actual-EXE report invalidated that closure. This report supersedes the loading
lifetime claim in pushed Dev `42d840a9` and original `0582d226`.

## Goal And Boundary

Keep the Recipe Manager loading overlay open until Recipe restoration and the
required deferred Recipe preparation both finish. The completed Recipe Manager
must be interactive when the overlay closes.

This change does not move Recipe persistence to a worker thread, change
Preview/Run, alter layers or routing, add concurrency, or restart native Tool
prewarming on Recipe changes.

## Reproduced Cause

1. `SelectRecipe` correctly yielded a render turn and showed the overlay.
2. `RecipeState.EventChangedRecipe` synchronously restored Recipe context and
   scheduled Pipeline Review preparation with `Dispatcher.BeginInvoke`.
3. The queued work had no completion handle. `SelectRecipe` therefore cleared
   `IsSwitchingRecipe` in `finally` before that work ran.
4. The current actual EXE reproduced the defect: after the overlay disappeared,
   the process again failed a responsiveness probe at 1,069.9 ms.

## Implemented Behavior

- Pipeline Review preparation now returns its dispatcher `Task`.
- The Recipe controller owns the current preparation task.
- Existing-Recipe selection and create/switch await that task before clearing
  `IsSwitchingRecipe`.
- The `Selected`/`Created` status is assigned only after preparation finishes.
- The normal Recipe event remains the single refresh owner. No second
  preparation or persistence path was added.
- A controlled smoke gate verifies both sides of the contract: loading remains
  visible while preparation is incomplete and closes after completion.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --no-restore`
  - passed with 0 warnings and 0 errors.
- `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug --no-restore`
  - passed with 0 warnings and 0 errors.
- Rebuilt current-source WPF targets passed with zero layout, text, and internal
  errors:
  - `wpf_shell_host_recipe_change_safety`
  - `wpf_shell_host_recipe_context_switch`
  - `wpf_shell_host_recipe_manager_summary`
  - `wpf_shell_host_native_tool`
- The safety result records
  `RecipeLoadingOverlay=HeldUntilPreparationComplete`.
- `OpenVisionReadinessCheck` passed all 13 contracts.
- `git diff --check` passed before this record and is rerun at handoff.
- The same nine modified files were applied to the original repository through
  one reviewed Git patch. Dev/original `git hash-object` values match for all
  nine paths, and the original repository has no staged changes.
- The original solution and ScreenshotSmoke project built with 0 warnings and
  0 errors. The same four rebuilt WPF targets, readiness 13/13, and the
  documentation index passed in the original repository.

## Actual EXE Evidence

The latest Debug EXE was launched on the dynamically selected smaller left
monitor, `DISPLAY2`, with reported working area `-1920,365,1920x1032`.

Test transition: `FieldPilot_BentPin -> Default`.

- Before correction: the overlay had closed when the process again failed the
  responsiveness probe at 1,069.9 ms.
- After correction: the overlay remained visible through the preparation
  interval. The process was responsive from the 866.0 ms sample through the
  final 3,269.4 ms sample with no second stall.
- After the overlay closed, the Recipe filter accepted and rendered `Default`
  in 40.6 ms.
- No Preview/Run, layer, active-layer, or route side effect was recorded by the
  focused contract.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_switch_lifetime_20260811`

Key evidence:

- Before: `before_actual_exe\frame_00.png`, `frame_04.png`, `timeline.json`
- After latest build: `after_actual_exe_final\frame_00.png`, `frame_01.png`,
  `frame_05.png`, `timeline.json`
- Focused: `focused_current_rebuilt` and `focused_final`

### Original Repository Exact-Port Evidence

The rebuilt original Debug EXE was launched on the dynamically selected smaller
left monitor, `DISPLAY2`, with reported working area
`-1920,365,1920x1032`.

Test transition: `Edge_Base -> Default`, followed by restoration to
`Edge_Base`.

- `frame_00.png` shows the loading overlay during the Recipe transition.
- The process was responsive at every recorded post-overlay frame from
  1,503.4 ms through 12,634.3 ms, with no second stall during that approximately
  11-second post-overlay observation interval.
- After the overlay closed, the Recipe filter accepted and rendered `Default`
  in 71.1 ms.
- The test restored the initially selected `Edge_Base` Recipe before closing
  the EXE.

Original evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe_switch_lifetime_20260811`

Exact-port mapping and hash evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_switch_lifetime_20260811\exact_port_original`

## Completion Record

```text
Status: Complete
Scope: Existing-Recipe selection and create/switch retain the loading overlay until required Recipe preparation completes in the Dev and original working trees
Acceptance criteria: Loading remains while preparation Task is incomplete -> pass; closes after Task completion -> pass; no post-overlay responsiveness stall in the tested Dev and original actual-EXE transitions -> pass; post-load Recipe Manager input works -> pass; Dev/original modified paths match -> pass
Verification: Dev and original Debug solution and ScreenshotSmoke builds 0 warnings/errors; readiness 13/13; rebuilt Recipe safety/context plus Recipe summary/native Tool targets passed; original documentation index passed; actual-EXE timelines inspected; 9/9 path hashes matched
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_switch_lifetime_20260811, D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\recipe_switch_lifetime_20260811, and this report
Boundary / next dependency: This proves the tested FieldPilot_BentPin -> Default lifecycle in Dev and Edge_Base -> Default lifecycle in the original working tree; the correction is not staged, committed, or pushed
```
