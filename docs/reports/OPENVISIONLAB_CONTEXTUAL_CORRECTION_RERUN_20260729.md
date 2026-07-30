# OpenVisionLab Contextual Correction Rerun

Date: 2026-07-29 KST<br>
Status: Complete<br>
Scope: P252, source-aware explicit rerun after selected-Step correction

## Reproduced Gap

P251 starts from a persisted Local Validation Set failure, but the existing
corrected-output action always ran the currently selected catalog Good/Bad
pair. For a larger or differently composed set this did not replay the source
evidence and did not create a comparable saved suite run.

## Outcome

The corrected-output action is now contextual:

- A selected `LocalValidationSet` or `LocalValidationSetPartial` run shows
  `동일 세트 재검사 / Rerun same set`.
- The explicit action resolves the source set by the persisted
  recipe/pipeline/suite identity, selects that Local Set scope, and runs the
  current corrected pipeline against the full source set.
- The result is persisted through the existing Run History path and can use
  the previous run as its baseline comparison.
- If the source set is missing or belongs to a different current
  recipe/pipeline, the command is disabled and explains why. It does not
  silently fall back to an unrelated catalog pair.
- Without Local Validation Set history, the existing
  `Good/Bad 재검사 / Rerun Good/Bad` behavior remains unchanged.
- The corrected-output guidance text uses the same context as the button.

XML Apply still does not execute anything. The operator must press the rerun
button explicitly.

## Acceptance Evidence

The actual public `Public_Matching_DiePad` pair was saved as a two-row Local
Validation Set and executed once. The retained NG failure was prepared through
P251, its parameters were applied without an automatic run, and the contextual
action was pressed explicitly.

- A second `LocalValidationSet` summary was persisted under the same suite
  name.
- The second run retained both source rows.
- Run History exposed the previous run as a two-row baseline comparison.
- `LatestPairRunSummary` did not change, proving the command did not run the
  catalog-pair fallback.
- Native Preview/Run count, layer count, workspace selection, and input/output
  routes remained unchanged.
- Removing the source set disabled the action; restoring it restored the same
  set contract.
- The existing three-Step Fixture edit/apply/rerun smoke still passed through
  the Good/Bad fallback.

## Verification

```text
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"
PASS: 0 warnings, 0 errors

p252_contextual_correction_rerun
PASS: check=OK, layout=0, text=0, internal=0

p251_failure_correction_handoff
PASS: check=OK, layout=0, text=0, internal=0

wpf_shell_host_fixture_step_edit_apply_rerun
PASS: check=OK, layout=0, text=0, internal=0

wpf_shell_host_recipe_local_validation_set
PASS: check=OK, layout=0, text=0, internal=0

OpenVisionReadinessCheck
PASS: all 12 contract categories

git diff --check
PASS: no whitespace errors
```

## Evidence

- Fresh pre-change pair-only behavior:
  `artifacts\p252_contextual_correction_rerun_20260729\before\wpf_shell_host_fixture_step_edit_apply_rerun.png`
- Contextual explicit action:
  `artifacts\p252_contextual_correction_rerun_20260729\final_current_r2\p252_contextual_correction_rerun.diagnostics\p252-same-validation-rerun-action.png`
- Persisted same-set rerun and baseline comparison:
  `artifacts\p252_contextual_correction_rerun_20260729\final_current_r2\p252_contextual_correction_rerun.png`

## Boundary / Next Dependency

This proves rerun-scope fidelity for one public two-row Local Validation Set
and preserves the catalog-pair fallback. It does not select correction values,
claim the repeated NG result was fixed, qualify Matching, or automatically run
after XML Apply.

The explicit chain is now:

`Run suite -> failed row -> Prepare correction -> edit/apply -> Rerun same set -> compare saved runs`

No further feature is admitted from this chain without a new current-build
operator blocker or verified regression. CVR-00 remains incomplete pending
three independent first-time participants and their unedited observations.
