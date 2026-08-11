# OpenVisionLab Recipe Switch Loading And Responsiveness

Date: 2026-08-11 KST

Repository: `C:\Git\OpenVisionLab_Dev`

Source baseline: `ccc6b275`

## Goal And Scope

Make selection of an existing Recipe in Recipe Manager visibly acknowledge the
operation before restoration starts, avoid redundant post-switch work, and
keep the selected Recipe/Pipeline context correct. This change does not move
Recipe persistence to a worker thread, change Preview/Run, alter layer routing,
or add concurrency.

## Reproduced Cause

1. `SelectRecipe` set `IsSwitchingRecipe` and immediately continued into the
   synchronous Recipe load. A render-priority dispatcher invocation did not
   guarantee that DWM composited the small status indicator first. The actual
   EXE could therefore look frozen or show mixed old/new Recipe content.
2. `RecipeState.EventChangedRecipe` already refreshes Recipe context, Pipeline
   options, validation/history presentation, layers, and routing. `SelectRecipe`
   repeated the command-surface refresh after the same event returned.
3. Every Recipe change also restarted native Tool document prewarming. A
   diagnostic run with native prewarm disabled removed the post-switch
   responsiveness stall, identifying that queue as competing UI-thread work.

## Implemented Behavior

- Existing-Recipe selection is an asynchronous UI command only at its render
  boundary. It sets the switching state, yields one WPF dispatcher turn, then
  performs the existing synchronous Recipe restoration on the UI thread.
- Recipe Manager is covered by a localized Korean/English loading overlay with
  themed background, text, icon, border, and progress presentation. The cover
  also prevents conflicting Recipe actions during the switch.
- The normal shell event path is now the single refresh owner. A small fallback
  remains for isolated command-surface test hosts that do not subscribe to the
  Recipe event.
- Recipe changes no longer restart every native Tool prewarm immediately.
  Explicit Tool selection remains the owner of opening/rebuilding the selected
  Tool document; Pipeline Review prewarm remains unchanged.
- A second selection received while switching restores the displayed selected
  value instead of changing the underlying Recipe.

## Structure Change Proof

- Previous refresh owners: `RecipeState.EventChangedRecipe` and the tail of
  `SelectRecipe` both refreshed Recipe Manager state.
- Current refresh owner: the Recipe event is authoritative in the normal shell;
  the command surface refreshes only when an isolated host did not receive it.
- Previous native-prewarm trigger: every Recipe change.
- Current native-prewarm trigger: explicit Tool selection/resume behavior in the
  existing Tool orchestration path. Recipe change retains only Pipeline Review
  preparation.
- Search and focused smoke evidence confirm no new persistence service, task
  worker, parallel executor, or Preview/Run route was introduced.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`
  - passed with 0 warnings and 0 errors after the final source change.
- Current-source focused WPF checks passed with layout/text/internal errors all
  equal to zero:
  - `wpf_shell_host_recipe_change_safety`
  - `wpf_shell_host_recipe_manager_summary`
  - `wpf_shell_host_recipe_context_switch`
  - `wpf_shell_host_native_tool`
- `OpenVisionReadinessCheck`
  - passed all 13 readiness contracts.
- `git diff --check`
  - passed before this completion record; rerun at handoff.

## Actual EXE Evidence

The current Debug EXE was launched on the dynamically selected smaller left
monitor, `DISPLAY2`, bounds `-1920,365,1920x1080`, working area
`-1920,365,1920x1032`. Its window rectangle was
`-1920,365,0,1397`, which intersects that monitor exactly as required.

- Before: the old small status line appeared over mixed Recipe state, without
  an interaction-blocking loading surface.
- After: the current EXE shows `레시피 불러오는 중` and the target Recipe on a
  dimmed, fully themed Recipe Manager before exposing the completed summary.
- Final captured sequence selection request return: 39.2 ms; its first stable
  completed frame was captured at 433.8 ms.
- Separate responsiveness probe selection request return: 29.2 ms. Its samples
  were busy at 72.5 ms, responsive at 115.2 ms,
  synchronous restoration at 209.2-303.1 ms, then responsive from 414.3 ms
  through the last 1,205.6 ms sample.
- This is evidence for the tested `Default`/`FieldPilot_BentPin` transitions,
  not a guarantee for every Recipe size or storage device.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_switch_loading_20260811`

Key files:

- `before\before_during_confirmed_120ms.png`
- `after\final_actual_exe\frame_01.png`
- `after\final_actual_exe\complete.png`
- `after\final_actual_exe\capture_timeline.json`
- `after\final_actual_exe\responsiveness.json`
- `focused_final\wpf_shell_host_native_tool.png`
- `focused_final\wpf_shell_host_recipe_context_switch.png`

## Completion Record

```text
Status: Complete
Scope: Existing-Recipe selection loading visibility, duplicate refresh removal, and post-switch native prewarm contention removal in Dev
Acceptance criteria: Actual EXE shows a themed loading surface before completed Recipe state -> pass; tested switch completes without the previously reproduced post-switch prewarm stall -> pass; Recipe context and explicit native Tool open remain correct -> pass
Verification: Debug solution build 0 warnings/errors; readiness 13/13; four focused WPF checks passed; current actual-EXE screen sequence and responsiveness timeline inspected
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\recipe_switch_loading_20260811 and this report
Boundary / next dependency: This proves the tested existing-Recipe transitions in Dev; changes are not staged, committed, pushed, or promoted to the original repository
```
