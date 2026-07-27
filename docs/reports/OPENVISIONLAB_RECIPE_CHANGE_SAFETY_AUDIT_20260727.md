# OpenVisionLab Recipe Change Safety Audit

Date: 2026-07-27
Source commit: `9d7fa796ed94d90e50d840607b441a2954278947`
Workspace: `C:\Git\OpenVisionLab_Dev`

## Decision

The current Recipe Manager can show that a selected-Step PropertyGrid edit is dirty and can explicitly apply that edit to XML. However, the pending edit is silently discarded when the operator:

1. selects another Step;
2. selects another Pipeline;
3. selects another Recipe;
4. closes and reopens the Recipe Manager panel.

No save, discard, or cancel choice is offered on those paths. The stored XML remains unchanged, so the defect is silent loss of the in-memory operator edit rather than accidental autosave.

This is a confirmed operator-blocking workflow gap. The next implementation priority is a bounded `Recipe Change Safety` slice. Qualified Recipe snapshots, new inspection algorithms, LLM expansion, and general navigation work remain deferred until this gap is closed.

## Post-Audit Implementation Status

The defect recorded by this audit was fixed later on 2026-07-27. The audit
matrix below remains the immutable before-state reproduction; it is not the
current product result.

The completed implementation now uses one Apply/Discard/Cancel contract for
Step, Pipeline, Recipe, and Recipe Manager close transitions. Its full 4-by-3
integration matrix, failed-commit retention, forced-save failure retention,
post-save round-trip rollback, and zero Preview/Run/layer/route side-effect
evidence are recorded in:

- `docs\reports\OPENVISIONLAB_RECIPE_CHANGE_SAFETY_IMPLEMENTATION_20260727.md`
- `artifacts\recipe_change_safety_20260727\final`

## Scope

Included:

- selected-Step PropertyGrid load and dirty state;
- explicit XML apply and save/reload;
- Step, Pipeline, Recipe, and Recipe Manager panel transitions;
- Preview/Run, layer, active-layer, and route side effects;
- presence of last-saved or recovery actions.

Excluded:

- application-process crash simulation;
- power-loss or storage-failure injection;
- recipe qualification or release promotion;
- algorithm correctness and N-sample inspection validation;
- LLM XML authoring.

## Current-Build Result Matrix

| Scenario | Expected safe behavior | Current result | Verdict |
|---|---|---|---|
| Load Step and edit PropertyGrid | Dirty state is visible; XML is unchanged until explicit apply | Dirty status is visible and XML remains unchanged | Pass |
| Explicit `Apply to XML` | Persist, round-trip, clear dirty state, and do not run | Persist/reload passed with no Preview/Run, layer, or route mutation | Pass |
| Select another Step while dirty | Save / Discard / Cancel, or block transition | Edit session is cleared and the new Step is selected without a choice | Fail — silent discard |
| Select another Pipeline while dirty | Save / Discard / Cancel, or block transition | Edit session is cleared and the new Pipeline is selected without a choice | Fail — silent discard |
| Select another Recipe while dirty | Save / Discard / Cancel, or block transition | Edit session is cleared and the new Recipe is selected without a choice | Fail — silent discard |
| Close and reopen Recipe Manager while dirty | Save / Discard / Cancel, or retain the pending edit | Manager returns to its summary state and the pending edit is gone | Fail — silent discard |
| Transition side effects | No Preview/Run, new layer, active-layer change, or route change | No side effects were observed in all four discard cases | Pass |
| Revert to last saved | Explicit recovery action is available | No Recipe Manager recovery command was found | Gap |
| Crash/session recovery | Pending edit can be recovered after abnormal exit | No recovery contract or implementation was found | Gap; not runtime-tested |

## Source Findings

- `OpenVisionRecipeStepEditSessionViewModel.MarkDirty` records only an in-memory dirty flag and status.
- `SelectedPipelinePreviewStep` clears the edit session whenever the selected Step changes.
- Recipe and Pipeline selection refresh the summary and selected Step without checking the dirty session first.
- Closing the Recipe Manager panel also returns through a path that clears the selected edit session.
- There is no centralized transition guard, save/discard/cancel decision, last-saved restore command, or pending-edit recovery store.

Relevant source:

- `UI\Menu\Wpf\Recipe\Review\OpenVisionRecipeStepEditSessionViewModel.cs`
- `UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.cs`
- `UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
- `UI\Menu\Wpf\OpenVisionShellHostView.Interactions.cs`

## Evidence

Current solution build:

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" /nr:false -m:1 /p:UseSharedCompilation=false
Result: PASS, 0 warnings, 0 errors
```

Existing current-build UI smokes:

```text
wpf_shell_host_pipeline_step_edit_handoff=OK
wpf_shell_host_fixture_step_edit_apply_rerun=OK
wpf_shell_host_recipe_context_switch=OK
wpf_shell_host_recipe_manager_summary=OK
```

Audit probe:

```text
wpf_shell_host_recipe_change_safety_audit=OK
StepSwitch=SilentDiscard
PipelineSwitch=SilentDiscard
RecipeSwitch=SilentDiscard
ManagerPanelCloseReopen=SilentDiscard
StoredXmlAfterDiscard=Unchanged
PreviewRunSideEffects=None
LayerSideEffects=None
RouteSideEffects=None
RecoveryCommand=NotAvailable
CrashRecovery=NotAvailable
```

Artifacts:

- `artifacts\recipe_change_safety_audit_20260727\current_build_smokes`
- `artifacts\recipe_change_safety_audit_20260727\audit_probe`
- `artifacts\recipe_change_safety_audit_20260727\audit_probe\wpf_shell_host_recipe_change_safety_audit.diagnostics\audit_observations.txt`
- `artifacts\recipe_change_safety_audit_20260727\audit_probe\wpf_shell_host_recipe_change_safety_audit.diagnostics\01-dirty-before-switch.png`
- `artifacts\recipe_change_safety_audit_20260727\audit_probe\wpf_shell_host_recipe_change_safety_audit.diagnostics\02-after-step-switch.png`
- `artifacts\recipe_change_safety_audit_20260727\audit_probe\wpf_shell_host_recipe_change_safety_audit.diagnostics\03-manager-reopened-after-discard.png`

The audit target was added only long enough to reproduce and record current behavior, then removed. Product source and the permanent smoke catalog were not changed by this audit.

## Next Implementation Contract

Implement one centralized leave-edit-session decision before any Step, Pipeline, Recipe, or Recipe Manager close transition.

Required operator choices:

- `Apply and continue`: commit the PropertyGrid editor, validate/save/round-trip XML, then continue only if save succeeds.
- `Discard`: reload the stored Step state and continue without changing XML.
- `Cancel`: keep the current Recipe/Pipeline/Step and the dirty editor unchanged.

Acceptance criteria:

1. All four audited transitions use the same decision contract.
2. A failed commit, validation, save, or round-trip keeps the operator on the current edit session and reports the exact reason.
3. Apply, discard, cancel, and merely opening the decision UI do not trigger Preview/Run.
4. Those actions do not create/delete layers, change the active layer, or mutate input/output routing.
5. Existing explicit apply/save/reload behavior remains compatible.
6. Focused tests cover every choice for every transition, including invalid-save failure.
7. Fresh current-build captures show the dirty state, decision UI, canceled transition, and successful apply/discard outcomes.

Deferred from the first slice:

- autosave;
- application-crash recovery;
- Recipe version history;
- qualified/immutable Recipe promotion;
- change-reason audit trails.

## Completion Record

Status: Complete
Scope: Current-build Recipe pending-edit safety audit only
Acceptance criteria: Dirty/apply, four transitions, stored XML, and workspace side effects were all observed and recorded
Verification: Solution build passed; four existing UI smokes passed; the focused audit probe passed
Evidence: `artifacts\recipe_change_safety_audit_20260727` and this report
Boundary / next dependency: The confirmed defect is not fixed. Implementation requires the bounded `Recipe Change Safety` contract above.
