# Step Edit Session ViewModel Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move selected Pipeline Step edit-session state out of
  `OpenVisionShellHostRecipeCommandSurface`.
- Preserve PropertyGrid load, dirty, explicit XML apply, clean/reload,
  corrected-output review, command visibility, and side-effect contracts.

## Excluded

- No PropertyGrid mapper, Pipeline XML format, tool algorithm, Preview/Run,
  layer, route, recipe, or visible layout change.
- No new persistence service, interface, or command-surface partial.

## Structural Change

- Previous owner: Shell fields directly held the edit object, dirty flag,
  status text, and corrected-output review text. Shell handlers manually
  changed those fields and raised related property notifications.
- Current owner: `OpenVisionRecipeStepEditSessionViewModel` owns the four
  mutable values and the `Load`, `MarkDirty`, `MarkClean`, `SetStatus`,
  `SetCorrectedOutputReview`, and `Clear` transitions.
- Current call path:
  Shell load/apply command -> Step Edit Session ViewModel state transition ->
  Shell notification adapter -> existing PropertyGrid/XAML binding.
- XML lookup/save, tool-session seeding, and workspace coordination remain in
  the Shell because they are not edit-session state.

## Acceptance Criteria

1. The old four mutable Shell fields are absent.
2. One non-partial ViewModel owns edit-session state and transitions.
3. Selected-Step handoff and Fixture edit/apply/rerun smokes pass from current
   source.
4. Explicit edit/apply does not trigger native Preview, change layers, or
   change routes.
5. Debug build and readiness check pass.

## Verification

- Source search confirmed
  `selectedStepEditObject`, `selectedStepEditStatusText`,
  `correctedOutputReviewText`, and `selectedStepEditDirty` are absent from the
  Shell.
- `wpf_shell_host_pipeline_step_edit_handoff` passed an extended
  load -> dirty -> explicit apply -> clean/reload state contract.
- `wpf_shell_host_fixture_step_edit_apply_rerun` passed after correcting its
  pre-existing smoke precondition to select the Step Details tab before
  clicking its edit button. The unchanged original baseline had failed at the
  same hidden-button precondition.
- Current-source artifacts:
  - `artifacts/mvvm_step_edit_session_viewmodel_20260726_r3`
  - `artifacts/mvvm_step_edit_session_viewmodel_20260726_r4`

## Boundary

This proves a real mutable-state owner and call-path change. It does not claim
that the whole Recipe command surface is MVVM-complete or requalify inspection
algorithms.
