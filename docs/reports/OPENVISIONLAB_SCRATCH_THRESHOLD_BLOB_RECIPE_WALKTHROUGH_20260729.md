# OpenVisionLab Scratch Threshold -> Blob Recipe Walkthrough

Date: 2026-07-29<br>
Work item: P255<br>
Status: Complete

## Outcome

An agent-operated beginner-role walkthrough completed this exact current-EXE
task from a blank workspace:

`new Recipe -> load image -> Threshold Preview -> add/save -> Blob input =
Threshold_Preview -> Blob Preview -> add/save -> inspect route -> application
restart -> restore Recipe -> reload image -> explicit Run Review`

The final Recipe was `P255_Novice_Threshold_Blob_18656`. Its saved Pipeline
contains:

1. `Threshold`: `Main -> Threshold_Preview`
2. `Blob_1`: `Threshold_Preview -> Blob_Preview`

Before the explicit post-restart Run, Pipeline Review showed both Steps in
`WAIT` state and the restored Blob route. The one explicit Run completed both
Steps as `OK`, retained 13 Blob object candidates with drawings and metrics,
and reported `OK / 21.5 ms`.

No production behavior change was required. The bounded implementation work
was verification infrastructure:

- added the complete scratch-Recipe recording scenario;
- made Windows file-dialog selection target the actual Open button;
- waited on visible completion evidence rather than non-UIA container
  elements;
- used a unique Recipe name per recording;
- used the shortest normal transition from saved Threshold directly to Blob;
- corrected the focused Threshold -> Blob smoke, which incorrectly expected a
  threshold-slider change to auto-run Preview. It now proves zero run on
  setting change and exactly one run after explicit Preview.

## Acceptance Criteria And Evidence

| Criterion | Result | Evidence |
|---|---|---|
| Blank workspace can create a named Recipe without entering Advanced Review | Pass | successful recording, 0-20 seconds |
| The chosen image loads into `Main` | Pass | recording timeline `image-loaded` |
| Threshold runs only after explicit Preview and saves to the active Recipe | Pass | timeline; corrected focused smoke |
| Blob can select the immediately preceding `Threshold_Preview` output | Pass | timeline `combo-selection`; saved Pipeline XML |
| Both Steps remain saved before restart | Pass | Pipeline Review shows `01 Threshold`, `02 Blob_1`, and restored route |
| Application restart restores Recipe, Step order, and Blob input route | Pass | post-restart route frame; Pipeline XML SHA-256 |
| Restoration causes no automatic Run | Pass | post-restart frame shows both Steps `WAIT` before explicit Run |
| One explicit Run completes the restored Pipeline with drawings/metrics | Pass | `OK / 21.5 ms`; final frame; 13 Blob candidates |

## Clean Runtime And Stored Recipe

Clean runtime:

`artifacts\openvisionlab_clean_runtime_p255_r7_20260729`

- EXE SHA-256:
  `239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E`
- Application DLL SHA-256:
  `4EFD81371ABCD31F345835194989AB366E01C2FB0F15652BD27F4A03658D7B64`
- Manifest SHA-256:
  `799852E3A777A466D3EA113927321F125BA727701DDA98FB71BFDF0FD8DD5F42`

Saved Pipeline:

`artifacts\openvisionlab_clean_runtime_p255_r7_20260729\RECIPE\P255_Novice_Threshold_Blob_18656\VISION\Pipeline.xml`

Pipeline XML SHA-256:

`208C5D3BA1C5E8B60498794FE22A558833C6C1CE1B1FBC539C023B9800C87F7F`

## Video And Visual Evidence

Successful 171-second recording:

`artifacts\p255_scratch_threshold_blob_recipe_20260729\after_r8\novice-scratch-threshold-blob-recipe.mp4`

Video SHA-256:

`12904794D52C04CC5B56BA1BF3352EC35AADD21AA5130B65429EE6F0C56BE884`

Reviewed frames:

- blank workspace:
  `artifacts\p255_scratch_threshold_blob_recipe_20260729\evidence\before_empty_workspace.png`
- saved route before restart:
  `artifacts\p255_scratch_threshold_blob_recipe_20260729\evidence\before_restart_saved_route.png`
- restored two-Step route before Run:
  `artifacts\p255_scratch_threshold_blob_recipe_20260729\evidence\after_restart_restored_route.png`
- final explicit Run result:
  `artifacts\p255_scratch_threshold_blob_recipe_20260729\evidence\after_explicit_run_ok.png`

The restored-route frame visibly proves that reload did not itself execute the
Pipeline: both Steps are present, Blob still consumes `Threshold_Preview`, and
both results remain `WAIT`. The final frame shows both Steps as `OK`,
`Threshold_Preview` and `Blob_Preview`, the Blob drawing, and object rows.

## Verification

Commands/checks run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\BuildCleanRuntime.ps1 `
  -Mode Dev `
  -OutputDir artifacts\openvisionlab_clean_runtime_p255_r7_20260729

powershell -NoProfile -ExecutionPolicy Bypass `
  -File tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1 `
  -Scenario novice-scratch-threshold-blob-recipe `
  -OutputDirectory artifacts\p255_scratch_threshold_blob_recipe_20260729\after_r8 `
  -RuntimeDirectory artifacts\openvisionlab_clean_runtime_p255_r7_20260729

dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj `
  -c Debug -- `
  --target wpf_threshold_to_blob_detection_e2e `
  artifacts\p255_scratch_threshold_blob_recipe_20260729\focused_smoke_explicit_preview
```

Results:

- clean Debug runtime build: pass;
- PowerShell parser: pass;
- actual clean-runtime walkthrough: `Status=Complete`;
- focused explicit-Preview Threshold -> Blob smoke:
  `OK`, 1600x900, zero layout/text/internal failures;
- Pipeline XML inspection: two Steps in order with exact route;
- video and extracted frames: visually reviewed.

## Boundaries

- This is agent/developer workflow evidence. It does not complete CVR-00,
  which still requires three independent first-time participants and their raw
  observations.
- The successful sample result does not qualify Threshold, Blob, or the
  selected parameters on production data.
- This proves one two-Step sequential Recipe, not arbitrary long-Pipeline
  usability, N-image Teaching, calibration, field robustness, or commercial
  platform parity.
- No camera, lighting, PLC, I/O, account, deployment, MES, new algorithm,
  LLM, or automatic execution scope was added.

## Durable Completion Record

Status: Complete<br>
Scope: one blank-workspace Threshold -> Blob Recipe authored through direct
Teaching, saved, restarted, restored, and explicitly run in a clean current
runtime.<br>
Acceptance criteria: named Recipe -> pass; image load -> pass; explicit
Threshold Preview/save -> pass; Blob predecessor selection/save -> pass;
two-Step pre-restart route -> pass; restart restoration without auto-run ->
pass; explicit Run with drawings/metrics -> pass.<br>
Verification: clean-runtime build; 171-second actual-EXE recording; saved XML
hash inspection; corrected focused runtime smoke; visual frame review.<br>
Evidence:
`artifacts\p255_scratch_threshold_blob_recipe_20260729`,
`artifacts\openvisionlab_clean_runtime_p255_r7_20260729`, and this report.<br>
Boundary / next dependency: CVR-00 remains externally dependent on three real
independent first-time participants. The next bounded development priority is
input/output route clarity across a longer operator-authored Pipeline only if
the next agent-role recording reproduces a concrete blocker; otherwise stop
and wait for named evidence.
