# Pipeline Review Reopen Performance

Date: 2026-08-09 KST
Status: Complete in Dev

## Scope

- Remove disk/catalog enumeration from command `CanExecute` reevaluation.
- Reuse the floating Pipeline Review document and window only when returning to
  the same Recipe and Pipeline.
- Preserve explicit Preview/Run, layer, and routing contracts.
- Keep the docked return path and user-close path destructive as before.

## Result

The actual EXE remained responsive in every measured cycle. Reopening the same
Pipeline Review five times took 96-115 ms from command activation until the
Run Review control was ready. The internal open path took 35-39 ms. Before the
change, five equivalent reopen cycles took 423-451 ms; after command-state
caching alone they took 268-310 ms.

The fresh-process first open remains a construction path and measured 409 ms.
This change intentionally optimizes `Return to Recipe -> Open Pipeline` rather
than hiding first construction behind startup work.

## Structural Changes Confirmed

- Before: returning to Recipe Manager closed and disposed the Pipeline Review
  document and floating window. Reopening rebuilt the WPF document, View, and
  window.
- After: `OpenVisionShellHostDocumentController` owns one suspended Pipeline
  Review document. `OpenVisionFloatingToolWindowHost` hides the existing window
  for the same-context return path. The Tool Window controller restores the
  document and refreshes layer presentation before showing it again.
- Evidence: source search for `TryRestorePipelineReview`,
  `SuspendPipelineReviewForRecipeReturn`, `HideForReuse`, and
  `CloseCachedPipelineReviewDocument`; actual timing diagnostics report
  `InternalActivateDocumentMs=4-6` and `InternalTotalMs=35-39` on reopen.

## Call Path

- Old: Pipeline Review `Return to Recipe` -> close window -> dispose document ->
  Recipe Manager -> create document/View/window on `Open Pipeline`.
- New: Pipeline Review `Return to Recipe` -> hide window -> suspend exact-context
  document -> Recipe Manager -> restore document -> refresh layer state -> show
  existing window.
- Context mismatch, another Tool, application close, explicit user close, and
  docked return keep the existing close/dispose behavior.

## State And Side-Effect Evidence

- The current-build actual-EXE capture shows Pipeline Review in `not run`
  state with no input or output image. Opening and restoration did not invoke
  Preview/Run.
- The focused Recipe Manager smoke covers the docked Return to Recipe fallback.
- The Recipe Context Switch smoke covers active Recipe/Pipeline context
  propagation and no automatic Preview.

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed,
  0 warnings, 0 errors.
- `wpf_shell_host_recipe_manager_summary`: passed, layout/text/internal checks 0.
- `wpf_shell_host_recipe_context_switch`: passed, layout/text/internal checks 0.
- `OpenVisionReadinessCheck`: passed, 13/13 contracts.
- `git diff --check`: passed.
- Actual EXE: `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`, placed on
  leftmost `DISPLAY2` at `-1920,365 1920x1080`; actual window rectangle
  `-1900,385 1600x900` intersected that monitor.

## Evidence

- Performance data:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\pipeline-open-cache-20260809\actual-exe-performance.json`
- Current-build after capture:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\pipeline-open-cache-20260809\actual-exe-after.png`
- Closest reproducible before capture:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\dock-float-performance-20260809\actual-exe-after.png`
- CPU trace used for diagnosis:
  `D:\OpenVisionLab-TestData\OpenVisionLab\diagnostics\pipeline-open-review-20260809\pipeline-open-cycles.nettrace`

## Boundary

This proves the same-Recipe/same-Pipeline floating reopen workflow on the
current workstation. It does not claim a faster first construction, parallel
Pipeline execution, multi-PC qualification, or camera/PLC behavior. Two wider
exploratory smokes reached the changed command preconditions but later failed
at unrelated existing assertions: LLM draft active-pipeline selection and the
workspace sample first-Step action assertion. They were not used as completion
evidence for this bounded change.
