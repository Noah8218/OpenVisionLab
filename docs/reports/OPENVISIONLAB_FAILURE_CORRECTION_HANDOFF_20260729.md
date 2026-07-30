# OpenVisionLab Failure Correction Handoff

Date: 2026-07-29 KST<br>
Status: Complete<br>
Scope: P251, explicit Run History failure-to-PropertyGrid correction preparation

## Outcome

Recipe Manager Run History now exposes one explicit
`실패 수정 준비 / Prepare correction` action for a retained row with a linked
failed Step.

The action performs this bounded preparation sequence:

1. Resolve and select the linked failed Step.
2. Respect the existing pending-edit Save/Discard/Cancel decision.
3. Load that Step's persisted parameters into the existing PropertyGrid.
4. Load the retained sample bytes into the Step's existing input layer.
5. Open the existing XML/Steps editing surface.

The operator still decides what to change, applies the XML explicitly, and
uses the existing explicit Good/Bad or Validation Set rerun action. Individual
`Failed step`, `Sample -> input`, drawing, input, and output review actions
remain available.

## Acceptance Evidence

- The public `Public_Matching_DiePad` OK/NG pair was saved as a Validation Set
  and run through the actual suite path.
- The retained NG row resolved `01 Synthetic Die Pad Match`.
- The image loaded into `Main` had the same decoded bitmap SHA-256 as the
  retained failed sample.
- The Matching PropertyGrid was loaded cleanly and the XML/Steps tab became
  selected.
- A dirty same-Step edit followed by `Cancel` retained both the dirty edit and
  the pre-action input image.
- A subsequent explicit `Discard` allowed correction preparation to finish.
- Preparation did not increment Preview/Run, create/delete a layer, change the
  selected workspace layer, or change input/output routing.
- A workspace without the required input layer still fails visibly; the action
  does not silently create a layer.

## Verification

```text
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"
PASS: 0 warnings, 0 errors

dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p251_failure_correction_handoff artifacts\p251_failure_correction_handoff_20260729\final_current
PASS: check=OK, layout=0, text=0, internal=0

p250_catalog_pair_validation_set
PASS: check=OK, layout=0, text=0, internal=0

wpf_shell_host_recipe_local_validation_set
PASS: check=OK, layout=0, text=0, internal=0

OpenVisionReadinessCheck
PASS: all 12 contract categories

git diff --check
PASS: no whitespace errors; line-ending notices only
```

## Evidence

- Fresh pre-change overall Recipe Manager baseline:
  `artifacts\p251_failure_correction_handoff_20260729\before\wpf_shell_host_recipe_local_validation_set.png`
- Final Run History action:
  `artifacts\p251_failure_correction_handoff_20260729\final_current\p251_failure_correction_handoff.diagnostics\p251-run-history-before-action.png`
- Final prepared XML/Steps PropertyGrid:
  `artifacts\p251_failure_correction_handoff_20260729\final_current\p251_failure_correction_handoff.png`

## Boundary / Next Dependency

This proves a deterministic correction handoff for one public Matching pair.
It does not prove that an operator's parameter correction is semantically
correct, qualify the matcher, add an algorithm, or automate Preview/Run.

The inspected chain now has a short safe path:

`Run suite -> failed row -> Prepare correction -> edit/apply -> explicit rerun`

No further feature is admitted from this chain without a new current-build
operator blocker or verified regression. CVR-00 remains incomplete pending
three independent first-time participants and their unedited observations.
