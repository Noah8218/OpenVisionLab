# Workspace Empty Pipeline Open Performance

Date: 2026-08-09 KST
Status: Complete in Dev

## Scope

- Diagnose the exact `Pipeline 열기` button in the main no-image workspace.
- Reduce the time from `WorkspaceEmptyPipelineButton` invocation until the
  visible Pipeline Review `리뷰 실행` control is ready.
- Preserve the no-auto Preview/Run, layer, active-layer, recipe, and Pipeline
  routing contracts.
- Keep Recipe Manager entry performance outside this correction.

## Reproduced Baseline

- The button routes through
  `WorkspaceCommands.OpenSamplePipelineCommand -> selectTool(Pipeline)`.
  This is separate from Recipe Manager's Open Pipeline command.
- Three fresh current-build EXE processes reached visible Pipeline Review in
  1,075 ms, 768 ms, and 820 ms; the median was 820 ms.
- Internal selection took 327 ms, 255 ms, and 244 ms; the median was 255 ms.
  Pipeline Review construction synchronously created the complete WPF view,
  loaded and validated the current Pipeline, and performed its first layout
  only after the operator clicked.
- The reported three-second delay was not reproduced on this workstation, but
  the exact requested button had a repeatable 0.8-1.1 second first-open delay.

## Change

- Startup background work now creates one Pipeline Review document for the
  exact current Recipe/Pipeline and retains it in the existing document
  controller cache.
- The central Pipeline Review workspace attaches that cached document with
  `Visibility.Hidden` and completes its WPF measure/arrange pass during idle
  work. The no-image guide remains visible and interactive until the operator
  explicitly clicks `Pipeline 열기`.
- Clicking the button restores the context-matched document and changes the
  prepared central workspace to visible instead of constructing and attaching
  the entire review tree at click time.
- The existing exact Recipe/Pipeline context comparison remains authoritative.
  A mismatched cached document is disposed, and Recipe changes schedule a new
  idle preparation for the new current context.
- If the operator selects Pipeline before background preparation runs, the
  normal creation path is used and the queued preparation does not create a
  duplicate document.

## Current Result

- Three fresh actual-EXE processes reached the visible `리뷰 실행` control in
  435 ms, 428 ms, and 411 ms; the median is 428 ms, 48% below the 820 ms
  baseline median.
- Internal selection completed in 22 ms, 28 ms, and 27 ms; the median is
  27 ms, 89% below the 255 ms baseline median.
- Central host display completed internally in 4-11 ms. Remaining UI Automation
  readiness time is WPF visibility/render and accessibility-tree publication,
  not Pipeline execution or document construction.
- The actual EXE remained responsive on all runs. The prepared Pipeline was not
  visible before the click. Wide 1600x900 showed the complete review surface;
  Compact 1280x760 kept the explicit `리뷰 실행` control on-screen.

## Verification

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_shell_host_workspace_empty" "D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\focused-current-isolated\wpf_shell_host_workspace_empty"
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_shell_host_recipe_context_switch" "D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\focused-current-isolated\wpf_shell_host_recipe_context_switch"
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_tool_window_dock_float_cycle" "D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\focused-current-isolated\wpf_tool_window_dock_float_cycle"
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_shell_host_workspace_sample_pipeline_review_metrics" "D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\focused-current-isolated\wpf_shell_host_workspace_sample_pipeline_review_metrics"
dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
dotnet run --project tools\VisionUiContractCheck\VisionUiContractCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev\bin\Debug"
git diff --check
```

- Debug build: 0 warnings, 0 errors.
- Empty workspace, Recipe context switch, dock/float cycle, and deterministic
  public-sample Pipeline Review smokes: passed with zero layout, text, and
  internal issues.
- Readiness: 13/13 contracts passed.
- Vision UI contract: passed, including native prewarm/navigation contracts.
- One broader smoke invocation exposed an existing data-order assumption:
  `wpf_shell_host_workspace_sample_actions` expects the first catalog sample's
  first Tool to be Threshold, while the current first sample uses Matching.
  The window opened and Preview remained false; the deterministic named-sample
  smoke above replaced that unstable ordering assumption for this verification.
- Actual EXE tests dynamically selected the smaller left monitor
  `\\.\DISPLAY2`, bounds `-1920,365 1920x1080`; the `1600x900` and `1280x760`
  windows intersected that monitor. Runtime state, TEMP/TMP, captures, and
  measurements were stored on `D:`.

## Evidence

- Exact-button before actual EXE:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\before`.
- Current actual EXE Wide/Compact and three-run measurements:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\after-current`.
- Current isolated focused smokes:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809\focused-current-isolated`.

## Closure Record

```text
Status: Complete
Scope: Dev main no-image workspace WorkspaceEmptyPipelineButton first-open performance
Acceptance criteria: exact button path measured before/after; current Recipe/Pipeline Review is prepared before click; no prepared UI leaks into the no-image workspace; context changes reject stale documents; no Preview/Run or routing side effect; Wide/Compact explicit Run control remains visible
Verification: Debug build 0 warnings/errors; exact actual-EXE median 820 ms -> 428 ms and internal median 255 ms -> 27 ms across three fresh processes; four deterministic focused UI smokes passed; readiness 13/13; Vision UI contract passed; git diff check passed
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\workspace-empty-pipeline-performance-20260809 and this report
Boundary / next dependency: this proves the exact current-build main no-image Pipeline button on this workstation, not every PC or arbitrarily large Pipeline. The reported three-second value was not reproduced. The catalog-order-dependent workspace_sample_actions assertion remains a separate test-maintenance issue. Original-repository promotion, commit, and push were not requested.
```
