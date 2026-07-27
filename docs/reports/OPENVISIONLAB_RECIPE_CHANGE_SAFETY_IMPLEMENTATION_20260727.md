# OpenVisionLab Recipe Change Safety Implementation

Date: 2026-07-27
Source baseline: `9d7fa796ed94d90e50d840607b441a2954278947` plus the changes recorded here
Workspace: `C:\Git\OpenVisionLab_Dev`

## Outcome

Recipe Manager selected-Step edits no longer disappear silently when the
operator changes Step, Pipeline, or Recipe, or closes Recipe Manager.

All four transitions use one decision contract:

- `Apply and continue`: commit the visible PropertyGrid editor, map the Step
  property back to XML, save, round-trip validate, and transition only after
  success.
- `Discard changes`: clear the pending edit and continue without changing the
  stored XML.
- `Cancel`: retain the current Recipe, Pipeline, Step, edit object, and dirty
  state.

If PropertyGrid commit fails, the transition is blocked and the dirty editor is
retained. If XML was saved but round-trip validation fails, the previous
pipeline XML is restored and validated before the transition remains blocked.

## Included Scope

- one centralized pending-edit transition controller;
- one explicit Korean/English decision dialog;
- Step, Pipeline, Recipe, and Recipe Manager close transitions;
- Recipe/Pipeline lifecycle actions that can leave the selected Step;
- Pipeline XML and preserved LLM XML draft imports that select a new Pipeline;
- save-before-transition and save-then-validation-failure rollback;
- permanent current-source screenshot regression coverage.

## Excluded Scope

- autosave;
- application crash or power-loss recovery;
- Recipe history or undo across application sessions;
- qualified/immutable Recipe promotion;
- new inspection algorithms or datasets;
- LLM provider, prompt-family, or browser automation expansion.

## Verification

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
PASS: 0 warnings, 0 errors

dotnet build "tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj" -c Debug
PASS: 0 warnings, 0 errors

wpf_recipe_pending_edit_dialog=OK
wpf_shell_host_recipe_change_safety=OK
wpf_shell_host_pipeline_step_edit_handoff=OK
wpf_shell_host_fixture_step_edit_apply_rerun=OK
wpf_shell_host_recipe_context_switch=OK
wpf_shell_host_recipe_manager_summary=OK
wpf_shell_host_recipe_line_pair_properties=OK

dotnet run --no-build --project "tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj" -c Debug
PASS: OpenVisionLab readiness contract passed
```

The focused integration smoke covers the full transition/choice matrix:

| Transition | Cancel | Discard | Apply and continue |
| --- | --- | --- | --- |
| Step | retains dirty Step/editor | moves without save | saves, validates, then moves |
| Pipeline | retains current Pipeline/editor | moves without save | saves, validates, then moves |
| Recipe | retains current Recipe/editor | moves without save | saves, validates, then moves |
| Recipe Manager close | remains open and dirty | closes without save | saves, validates, then closes |

It also proves:

- failed PropertyGrid commit blocks the transition and retains the dirty edit;
- forced XML save failure blocks the transition, retains the dirty edit, and
  preserves the previous stored XML;
- forced post-save round-trip failure blocks the transition and restores the
  original stored XML;
- no choice triggers Preview/Run;
- no choice creates/deletes a layer, changes the active layer, or changes input
  or output routing.

## Evidence

Before evidence from the baseline audit:

- `artifacts\recipe_change_safety_audit_20260727\audit_probe`

Final current-source view captures and diagnostics:

- `artifacts\recipe_change_safety_20260727\final`
- `artifacts\recipe_change_safety_20260727\final\wpf_recipe_pending_edit_dialog.png`
- `artifacts\recipe_change_safety_20260727\final\wpf_shell_host_recipe_change_safety.diagnostics\recipe_change_safety_results.txt`
- `artifacts\recipe_change_safety_20260727\final\wpf_shell_host_recipe_change_safety.diagnostics\01-step-cancel-preserves-dirty.png`
- `artifacts\recipe_change_safety_20260727\final\wpf_shell_host_recipe_change_safety.diagnostics\03-failed-save-keeps-dirty.png`
- `artifacts\recipe_change_safety_20260727\final\wpf_shell_host_recipe_change_safety.diagnostics\04-failed-roundtrip-restores-xml.png`

These are current-source WPF view captures generated after the final Debug
build. They are not a production EXE, installer, deployment, or field-runtime
qualification claim.

## Completion Record

Status: Complete
Scope: Centralized Recipe selected-Step pending-edit safety for Step, Pipeline,
Recipe, and Recipe Manager close transitions
Acceptance criteria: Full 4-by-3 decision matrix passed; failed commit, save,
and round-trip remain on the dirty editor; failed save preserved and failed
round-trip restored the previous XML; Preview/Run, layers, active layer, and
routes were unchanged
Verification: Debug solution and screenshot-runner builds passed with zero
warnings/errors; seven current-source UI targets and the readiness contract
passed
Evidence: `artifacts\recipe_change_safety_20260727\final` and this report
Boundary / next dependency: This does not provide autosave, crash recovery,
Recipe version history, or qualified immutable promotion. The next bounded
product priority is a `Qualified Recipe Snapshot` contract/design audit before
implementation.
