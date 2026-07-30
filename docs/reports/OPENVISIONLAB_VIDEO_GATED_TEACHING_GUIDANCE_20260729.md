# OpenVisionLab Video-Gated Direct-Teaching Guidance

Date: 2026-07-29 KST

Status: Complete

## Scope

Freeze one beginner task:

1. Open `Public_Blob_Particles_Good`.
2. Open the direct Blob Tool View.
3. Choose the Basic preset.
4. Change the threshold from 100 to 150.
5. Run Preview explicitly and review the detected particles.

The bounded correction makes the preset choice and threshold rationale visible.
It does not add automatic parameter recommendation, automatic Preview/Run,
recipe mutation, or algorithm changes.

## Before Evidence

The authoritative before run is:

- Video:
  `artifacts\video_gated_teaching_guidance_20260729\before_r4\novice-blob-teaching-self-trial.mp4`
- Timeline:
  `artifacts\video_gated_teaching_guidance_20260729\before_r4\novice-blob-teaching-self-trial.timeline.tsv`
- Run identity:
  `artifacts\video_gated_teaching_guidance_20260729\before_r4\novice-blob-teaching-self-trial.run.txt`
- Extracted selected-preset frame:
  `artifacts\video_gated_teaching_guidance_20260729\before_r4_review\02_basic_applied.png`

The exact sample selection and loaded particle image were verified. Before the
change, the visible preset panel showed only the three names and the generic
message `기본 적용됨 / 미리보기로 검증하세요.` It did not explain the
Basic/Fast/Precise roles or why a beginner would adjust the threshold.

The earlier `before`, `before_r2`, and `before_r3` folders are retained as
capture-harness failure diagnostics only. They are not product evidence:
`before` opened the wrong sample, while `before_r2` and `before_r3` correctly
failed the newly added selection/load assertions before completing the task.

## Implemented Correction

- Before selection, the preset panel now shows:
  `기본=첫 검사 / 빠른=빠른 선별 / 정밀=최종 튜닝`.
- After selection, the same fixed panel shows the selected preset's localized
  rationale instead of only `applied`.
- Blob Basic states that simple thresholding and a medium area floor are the
  starting point and that the threshold should be adjusted where target and
  background brightness separate.
- The detail text wraps instead of truncating the reason.
- Selecting a preset still changes PropertyGrid values only. Preview remains an
  explicit action.
- The walkthrough capture now fails closed unless the requested sample appears
  in both the selected-sample summary and the visibly loaded application state.
  Its run record now includes the managed `OpenVisionLab.dll` hash as well as
  the launcher EXE hash.

Changed implementation:

- `UI\VisionTest\Wpf\Tooling\Presets\VisionToolPresetButtonPresenter.cs`
- `UI\VisionTest\Wpf\Tooling\SingleInput\VisionToolSingleInputPropertyToolShell.xaml`
- `Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`
- `tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1`

## After Evidence

The authoritative after run is:

- Video:
  `artifacts\video_gated_teaching_guidance_20260729\after_final_r2\novice-blob-teaching-self-trial.mp4`
- Timeline:
  `artifacts\video_gated_teaching_guidance_20260729\after_final_r2\novice-blob-teaching-self-trial.timeline.tsv`
- Run identity:
  `artifacts\video_gated_teaching_guidance_20260729\after_final_r2\novice-blob-teaching-self-trial.run.txt`
- Choice guide:
  `artifacts\video_gated_teaching_guidance_20260729\after_final_r2_review\01_choice_guide.png`
- Selected Basic rationale:
  `artifacts\video_gated_teaching_guidance_20260729\after_final_r2_review\02_basic_reason.png`
- Explicit Preview result:
  `artifacts\video_gated_teaching_guidance_20260729\after_final_r2_review\03_preview_12.png`

Current application identity:

- Launcher SHA-256:
  `239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E`
- Managed application assembly SHA-256:
  `587C4793A203E53CD62D4763EA7705DAA405B5BAF63E2C765A0419C1FFE4FA29`

The final timeline verifies exact selection and visible load of
`Public_Blob_Particles_Good`. Preset application and threshold editing occurred
before the one explicit Preview click. The result remained:

- `ResultCount=12`
- `MaxArea=892`
- center `377,261.1`
- bounds `33x33`

## Acceptance Criteria

| Criterion | Result | Evidence |
| --- | --- | --- |
| Exact requested sample selected and loaded | Pass | `sample-selection-verified` and `sample-loaded-verified` timeline events |
| Basic/Fast/Precise roles visible before selection | Pass | `after_final_r2_review\01_choice_guide.png` |
| Selected Basic rationale and threshold choice reason visible | Pass | `after_final_r2_review\02_basic_reason.png` |
| Preset application does not run Preview | Pass | focused smoke checks `NativePreviewRunCount`; actual timeline has no Preview before the explicit click |
| Explicit Preview result unchanged | Pass | actual timeline and `after_final_r2_review\03_preview_12.png` show 12 objects and the same metrics |
| Existing shipped defaults migrate; user-customized localization remains untouched | Pass | localization smoke invokes the exact-default migration and customized-value preservation paths |
| Current code builds and localization is valid | Pass | Debug solution build: 0 warnings, 0 errors; focused Blob and localization smokes passed |

## Verification

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"

dotnet run --no-build `
  --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" `
  -c Debug -- `
  --target "wpf_shell_host_blob_tool,localization_catalog_contract_check" `
  "artifacts\video_gated_teaching_guidance_20260729\focused_smoke_final"

powershell -NoProfile -ExecutionPolicy Bypass `
  -File "tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1" `
  -Scenario "novice-blob-teaching-self-trial" `
  -OutputDirectory "artifacts\video_gated_teaching_guidance_20260729\after_final_r2"
```

## Boundary / Next Dependency

This is agent-operated, one-sample workflow evidence. It is not independent
novice-participant evidence, threshold optimality proof, N-sample algorithm
qualification, production robustness, or field qualification.

The video-gated beginner-friction queue stops here. Reopen it only when a new
current-build recording exposes a concrete operator blocker or the user names a
new task. CVR-00 remains incomplete until three independent first-time
participants and their unedited observations are available.
