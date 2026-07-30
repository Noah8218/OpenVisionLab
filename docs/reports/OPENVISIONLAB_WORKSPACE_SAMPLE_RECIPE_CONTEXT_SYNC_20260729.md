# OpenVisionLab Workspace Sample / Recipe Context Sync

Date: 2026-07-29<br>
Work item: P253<br>
Status: Complete

## Outcome

An agent-operated novice walkthrough of the current Debug EXE reproduced and
closed a real context mismatch in the commercial-style correction loop:

`Catalog Good/Bad pair -> save Local Validation Set -> run -> retain NG ->
prepare correction -> apply XML -> rerun the same set -> compare`

Before this change, opening `Public_Matching_DiePad_Good` correctly changed the
workspace image and active generated Pipeline, but Recipe Manager could retain
an older selected sample and Pipeline. The visible `Save pair as Local
Validation Set` action therefore saved and ran the stale pair instead of the
currently opened Matching pair.

After this change, a successful workspace sample open synchronizes Recipe
Manager to the exact generated Pipeline and exact sample before pair actions
are enabled. The existing pending-Step-edit Save/Discard/Cancel guard runs
before the workspace image/Pipeline is changed, so Cancel leaves both contexts
and the edit intact.

## Scope

### Production behavior

- `OpenVisionShellHostCommandController` performs sample-context preflight
  before loading the image or saving the generated Pipeline.
- `OpenVisionShellHostRecipeCommandSurface` exposes:
  - `PrepareWorkspaceSampleContext` for the existing pending-edit transition;
  - `SynchronizeWorkspaceSampleContext` for exact Pipeline/sample selection
    after a successful workspace load.
- `OpenVisionShellHostView` connects those two phases to the workspace sample
  command.
- Synchronization does not execute Preview, Run, or a validation suite. It
  does not create/delete result layers or change input/output routing.

### Verification support

- `PipelineViewerScreenshotSmoke` adds
  `p253_workspace_sample_recipe_context_sync`.
- `Record-OperatorWalkthrough.ps1` adds
  `novice-matching-correction-loop` and robust descendant/ListBox handling
  needed to record the actual WPF workflow.

## Acceptance Criteria And Evidence

| Criterion | Result | Evidence |
|---|---|---|
| Reproduce the stale-context failure in the actual EXE | Pass | `artifacts\novice_matching_correction_loop_20260729\raw_r1\novice-matching-correction-loop.mp4`; `raw_r1_review\pipeline_workflow_46s.png` |
| Opening the Matching Good sample selects the exact generated Pipeline and exact sample in Recipe Manager | Pass | focused P253 smoke and `after_r6_review\01_context_synced.png` |
| Pair save uses Matching Good plus Matching NoTarget Bad, not a stale pair | Pass | focused P253 smoke and `after_r6_review\02_matching_set_saved.png` |
| Pending Step edit Cancel prevents image/Pipeline/sample context mutation and retains the edit | Pass | focused P253 smoke |
| Normal synchronization causes zero automatic Preview/Run and no additional layer/route mutation | Pass | focused P253 smoke |
| Actual novice-role EXE walkthrough reaches first run, correction preparation, XML Apply, explicit same-set rerun, and comparison | Pass | `after_r6\novice-matching-correction-loop.mp4` and timeline |
| XML Apply remains non-executing | Pass | timeline records Apply before the later explicit rerun; focused P252/P253 contracts |

## Actual EXE Walkthrough

Executable:

`C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`

Recording:

`artifacts\novice_matching_correction_loop_20260729\after_r6\novice-matching-correction-loop.mp4`

Recording SHA-256:

`4DAD5B3CDE0BA837B89DB073E33DC21BCDB8CA24A9A79245A4771AD4E691D477`

Timeline:

`artifacts\novice_matching_correction_loop_20260729\after_r6\novice-matching-correction-loop.timeline.tsv`

The 112.43-second recording proves:

1. the visible workspace, Recipe Manager Pipeline, sample source, and catalog
   pair all refer to `Public_Matching_DiePad`;
2. the exact pair is saved as a Local Validation Set;
3. the first explicit run stores a two-row history with one retained NG;
4. `Prepare correction` restores that exact sample and failed Matching Step;
5. a no-op XML round trip passes validation without running;
6. the operator explicitly reruns the same saved set;
7. comparison reports `Compared 2`, `Regression 0`, `Recovered 0`,
   `Still NG 1`.

The unchanged XML intentionally does not manufacture a correction. This is
workflow and context-fidelity evidence, not evidence that the Matching
inspection was semantically improved.

## Visual Evidence

- Before defect:
  `artifacts\novice_matching_correction_loop_20260729\raw_r1_review\pipeline_workflow_46s.png`
- After context synchronization:
  `artifacts\novice_matching_correction_loop_20260729\after_r6_review\01_context_synced.png`
- After same-set rerun and comparison:
  `artifacts\novice_matching_correction_loop_20260729\after_r6_review\06_same_set_comparison.png`
- Current-build contact sheet:
  `artifacts\novice_matching_correction_loop_20260729\after_r6_review\contact_sheet.png`

Contact-sheet SHA-256:

`8212984CD7FAF0C4E8E5D9927C56C6A07929C62B06B67F0E48BC3B83CDA0EEEE`

## Boundaries

- This is an agent-operated novice-role walkthrough. It is developer evidence
  and does not complete CVR-00, which still requires three independent
  first-time participants and unedited observations.
- It does not qualify the Matching template, thresholds, or algorithm on
  production data.
- It does not add automatic execution, an algorithm family, LLM behavior, or
  camera/lighting/PLC/I/O scope.
- Intermediate `after_r2` through `after_r5` recordings are capture-harness
  diagnostics, not completion evidence.

## Durable Completion Record

Status: Complete<br>
Scope: Exact workspace sample/generated-Pipeline synchronization into Recipe
Manager plus one actual-EXE correction-loop walkthrough.<br>
Acceptance criteria: stale-context reproduction -> pass; exact Mapping pair
selection -> pass; pending-edit cancellation -> pass; zero unintended
execution/layer/route mutation -> pass; end-to-end recorded walkthrough ->
pass.<br>
Verification: Debug solution build; screenshot-runner build; focused P253,
P250, P251, P252 and readiness checks; PowerShell parser; actual EXE recording
and visual frame review.<br>
Evidence:
`artifacts\novice_matching_correction_loop_20260729`,
`artifacts\p253_workspace_sample_recipe_context_sync_20260729`, and this
report.<br>
Boundary / next dependency: CVR-00 remains dependent on three independent
first-time participants. The next agent-operated workflow should test direct
teaching result persistence through Add-to-Pipeline, recipe save/reopen, and
Run Review rather than claim external usability validation.
