# OpenVisionLab Direct Teaching Pipeline Persistence

Date: 2026-07-29  
Work item: P254  
Status: Complete

## Outcome

An agent-operated novice-role walkthrough of the current Debug EXE found one
bounded user-facing blocker in:

`Direct Teaching -> explicit Preview -> Add to Pipeline -> reopen -> Run Review`

The existing action already appended the taught Step to the active Pipeline
and saved the Pipeline immediately. However, the button said only
`파이프라인 추가 / Add Pipeline`, and the completion status said only that
`Blob_1` was added. A first-time operator could not tell that saving was
complete, where it was saved, or which action came next.

The same single explicit action now says
`파이프라인에 추가·저장 / Add and save to Pipeline`. Its completion status
shows:

`<Step> saved / Recipe <name> > Pipeline <name> / Next: Open Pipeline`

No extra save dialog, automatic navigation, Preview, Run, layer mutation, or
route mutation was added.

## Scope

### Production behavior

- The shared direct Tool View action identifies its existing add operation as
  add-and-save.
- `OpenVisionNativePipelineCommandController` retains the exact confirmed
  Recipe/Pipeline context used by `VisionPipelineAppendService.AddStep`.
- The completion status names the saved Step, Recipe, Pipeline, and next
  operator action.
- Existing default localization catalogs migrate from the former default
  `파이프라인 추가 / Add Pipeline` without overwriting a customized entry.

### Verification support

- `PipelineViewerScreenshotSmoke` adds
  `p254_direct_teaching_pipeline_persistence`.
- The smoke performs a direct Blob Preview, adds/saves the Step, reloads the
  storage-backed Pipeline, refreshes Recipe Manager, reopens Pipeline Review,
  and runs Review explicitly.
- `Record-OperatorWalkthrough.ps1` retains the
  `novice-blob-pipeline-persistence` workflow and now rejects an unopened
  sample picker, accepts an already-set threshold, and verifies Tool View
  closure more defensively.

## Acceptance Criteria And Evidence

| Criterion | Result | Evidence |
|---|---|---|
| The action states that the reviewed setup is added and saved | Pass | current-source Tool View capture and actual EXE frame at 54 seconds |
| Completion shows the exact Step, Recipe, Pipeline, and next action | Pass | actual EXE `frame_54s.png`; focused P254 smoke |
| The saved Pipeline reloads with the new Blob Step | Pass | focused P254 storage reload |
| Recipe Manager refresh/reopen retains the exact Pipeline and 3-Step list | Pass | focused P254 Recipe Manager refresh |
| Explicit Run Review retains the added Step result, metrics, and drawing | Pass | `p254_direct_teaching_pipeline_persistence.png` |
| Add/save itself causes no additional Preview/Run, layer, active-layer, or route mutation | Pass | focused P254 before/after state assertions |
| Existing default localization updates without overwriting customization | Pass | default-catalog migration plus localization catalog smoke |

## Current EXE Evidence

Executable:

`C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`

Executable SHA-256:

`239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E`

Application assembly SHA-256:

`5ACE016C85CA96C36594E9B51EF78E62DB44975DA5CD84D59AF185DD2FE3D5FF`

Recording:

`artifacts\p254_direct_teaching_pipeline_persistence_20260729\exe_walkthrough_r2\novice-blob-pipeline-persistence.mp4`

Recording SHA-256:

`785769DF4412713659A984FF0CC38989AD67B785CD8466B60412D837BE0DD488`

The recording reaches the successful explicit Preview and the new visible
add/save completion state. The extracted frame shows:

`Blob_1 저장 / Recipe Default > Pipeline Sample_Public_Blob_Particles_Good /
다음: Pipeline 보기`

The recording then stops because the capture helper's visible close click did
not dismiss the floating Tool View. It is retained as current-EXE wording and
action-order evidence, not as proof of the later reopen/Run Review stages.
Those stages are proved by the focused current-source runtime smoke below.

## Visual Evidence

- Before, actual EXE:
  `artifacts\novice_blob_pipeline_persistence_20260729\raw_r1_review\frame_55s.png`
- After add/save wording, actual EXE:
  `artifacts\p254_direct_teaching_pipeline_persistence_20260729\exe_walkthrough_r2_review\frame_54s.png`
- After add/save status, current-source Tool View:
  `artifacts\p254_direct_teaching_pipeline_persistence_20260729\final_visual\p254_direct_teaching_pipeline_persistence.tool.png`
- After reopen and explicit Run Review, current-source Pipeline Review:
  `artifacts\p254_direct_teaching_pipeline_persistence_20260729\final_visual\p254_direct_teaching_pipeline_persistence.png`

Visual review found that the Korean add-and-save label fits the existing
button, the exact storage path remains readable, and the reopened third Blob
Step has a visible OK result, object rows, metrics, and output drawing.

## Verification

Commands run:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo
dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug --nologo
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p254_direct_teaching_pipeline_persistence artifacts\p254_direct_teaching_pipeline_persistence_20260729
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p254_direct_teaching_pipeline_persistence,localization_catalog_contract_check artifacts\p254_direct_teaching_pipeline_persistence_20260729\final_visual
```

Results:

- Solution build: 0 warnings, 0 errors.
- Screenshot-runner build: 0 warnings, 0 errors.
- P254 focused runtime/capture: OK.
- PowerShell capture-helper parser: OK.

## Boundaries

- This is agent/developer workflow evidence. It does not complete CVR-00,
  which still requires three independent first-time participants and unedited
  observations.
- The actual EXE recording does not complete its post-save stages because of
  desktop/capture-window automation interference; do not describe it as an
  end-to-end actual-user recording.
- This does not qualify Blob thresholds, the sample, or an inspection
  algorithm on production data.
- This adds no automatic execution, automatic navigation, algorithm family,
  LLM behavior, or camera/lighting/PLC/I/O platform scope.

## Durable Completion Record

Status: Complete  
Scope: Beginner-visible add-and-save identity plus exact Recipe/Pipeline
completion context for direct Tool View teaching.  
Acceptance criteria: add/save wording -> pass; exact destination and next
action -> pass; storage reload -> pass; Recipe Manager refresh/reopen -> pass;
explicit Run Review result/drawing -> pass; zero add/save execution/layer/route
side effects -> pass.  
Verification: Debug solution build; screenshot-runner build; focused P254
runtime/capture; PowerShell parser; actual EXE recording and extracted-frame
review.  
Evidence:
`artifacts\p254_direct_teaching_pipeline_persistence_20260729`,
`artifacts\novice_blob_pipeline_persistence_20260729\raw_r1_review\frame_55s.png`,
and this report.  
Boundary / next dependency: CVR-00 remains dependent on three independent
first-time participants. No additional deterministic feature is admitted from
this completed workflow; wait for a named current-build operator blocker,
verified regression, or the existing named data/participant prerequisite.
