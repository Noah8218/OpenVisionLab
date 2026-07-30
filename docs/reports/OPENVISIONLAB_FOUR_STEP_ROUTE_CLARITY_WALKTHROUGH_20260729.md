# OpenVisionLab Four-Step Route Clarity Walkthrough

Date: 2026-07-29<br>
Work item: P256<br>
Status: Complete

## Outcome

An agent-operated beginner-role walkthrough completed this exact current-EXE
task from a blank workspace:

`new Recipe -> load image -> Filter -> Threshold -> Morphology -> Blob ->
inspect all routes -> application restart -> restore Recipe -> reload image ->
confirm WAIT -> explicit Run Review`

The final Recipe was `P256_FourStep_Route_17224`. Its saved Pipeline contains:

1. `Filter`: `Main -> Filter_Preview`
2. `Threshold`: `Filter_Preview -> Threshold_Preview`
3. `Morphology`: `Threshold_Preview -> Morphology_Preview`
4. `Blob_1`: `Morphology_Preview -> Blob_Preview`

Each direct-Teaching Step required an explicit Preview and an explicit
`Add and save to Pipeline` action. Blob visibly selected
`Morphology_Preview`; applying the Basic preset preserved that input route.
Before restart, Pipeline Review showed all four ordered routes. After restart
and image reload, the same four routes returned with all four Steps in `WAIT`.
One explicit Run Review then completed `OK 4 / NG 0 / WAIT 0`, retained the
Blob drawing and 12 accepted object rows, and reported `OK / 21.5 ms`.

No production behavior change was required. The bounded implementation work
was verification infrastructure:

- added a four-Step beginner-role recording scenario;
- made the recorder position and activate the rule-based Workbench window
  rather than the same process's legacy labelling window;
- made ComboBox interaction verify the exact retained selection instead of
  logging an assumed success;
- used the dedicated docked Tool View close action before Recipe review;
- extended the focused area-preset smoke to prove Basic, Fast, and Precise
  preserve an explicitly selected input route and do not run Preview.

## Rejected Attempts

Three preliminary recordings are retained only as diagnostic evidence and are
not completion evidence:

- `before_r1`: the recorder's slow synthetic ComboBox click lost the Blob
  selection but incorrectly logged success;
- `before_r2`: the recorder positioned the legacy labelling window instead of
  the rule-based Workbench;
- `before_r3`: a concurrently running WPF smoke window covered the route list
  and invalidated the user-path recording.

The focused current-source WPF smoke proved that product presets preserve the
selected route. The final recording was therefore run alone after correcting
the recorder; no product defect was claimed from the invalid attempts.

## Acceptance Criteria And Evidence

| Criterion | Result | Evidence |
|---|---|---|
| A blank workspace can create a named Recipe and load the chosen image | Pass | final video and timeline |
| Four direct-Teaching Steps can be Previewed and saved in order | Pass | timeline `pipeline-step-saved` x4 |
| Every next Tool can select the prior Tool's output as input | Pass | verified ComboBox selections and saved XML |
| Blob Basic preset preserves `Morphology_Preview` and does not auto-run | Pass | final video/XML and focused area-preset smoke |
| Pipeline Review shows all four exact routes before restart | Pass | timeline `pipeline-route-reviewed` x4 and reviewed frame |
| Restart restores Recipe, Step order, and exact routes | Pass | post-restart route checks and saved XML |
| Restoration does not execute the Pipeline | Pass | restored frame shows all four Steps `WAIT` |
| One explicit Run completes with current-run drawings and object rows | Pass | `OK / 21.5 ms`, 12 Blob rows, final frame |

## Clean Runtime And Stored Recipe

Clean runtime:

`artifacts\openvisionlab_clean_runtime_p256_before_20260729`

- EXE SHA-256:
  `239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E`
- Application DLL SHA-256:
  `DC230D3A80DC62BA8647268AF89B67313E7E1A20E6E6B4C6173293738D938EE2`
- Manifest SHA-256:
  `055C47EC20BE863962AF8F56890ED68DB9074E9F9C5BAF0E9642BC79D89BF1B4`

Saved Pipeline:

`artifacts\openvisionlab_clean_runtime_p256_before_20260729\RECIPE\P256_FourStep_Route_17224\VISION\Pipeline.xml`

Pipeline XML SHA-256:

`258D2C1080C09DC57C3DF543DE0B0AAF7FE1D2384294C329F19EF85725F982C0`

## Video And Visual Evidence

Successful 335.33-second recording:

`artifacts\p256_four_step_route_clarity_20260729\final_r1\novice-four-step-route-clarity.mp4`

Video SHA-256:

`8D43FA7ED34B0AE162431E78A22993B8831FA992C91A30218F12639381E922C6`

Reviewed frames:

- blank workspace:
  `artifacts\p256_four_step_route_clarity_20260729\evidence\empty_workspace.png`
- Blob input selected:
  `artifacts\p256_four_step_route_clarity_20260729\evidence\blob_input_selected.png`
- four routes before restart:
  `artifacts\p256_four_step_route_clarity_20260729\evidence\saved_four_routes_before_restart.png`
- restored four routes, all `WAIT`:
  `artifacts\p256_four_step_route_clarity_20260729\evidence\restored_four_routes_wait.png`
- final explicit Run:
  `artifacts\p256_four_step_route_clarity_20260729\evidence\explicit_run_ok.png`

## Verification

Commands/checks run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\BuildCleanRuntime.ps1 `
  -Mode Dev `
  -OutputDir artifacts\openvisionlab_clean_runtime_p256_before_20260729

powershell -NoProfile -ExecutionPolicy Bypass `
  -File tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1 `
  -Scenario novice-four-step-route-clarity `
  -OutputDirectory artifacts\p256_four_step_route_clarity_20260729\final_r1 `
  -RuntimeDirectory artifacts\openvisionlab_clean_runtime_p256_before_20260729

dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj `
  -c Debug -p:Platform="Any CPU" --nologo

dotnet tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll `
  --target wpf_shell_host_area_tool_presets `
  artifacts\p256_four_step_route_clarity_20260729\focused_area_preset_route_final

dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --nologo
```

Results:

- clean Debug runtime build: pass;
- final actual-EXE walkthrough: `Status=Complete`;
- PowerShell parser: pass;
- exact four-Step XML route assertion: pass;
- focused Basic/Fast/Precise preset route/no-run smoke:
  `OK`, 1600x900, zero layout/text/internal failures;
- Debug solution build: pass, zero warnings and zero errors;
- final video and five extracted frames: visually reviewed.

## Boundaries

- This is agent/developer workflow evidence. It does not complete CVR-00,
  which still requires three independent first-time participants and their
  unedited observations.
- This proves one four-Step sequential route on one public synthetic image.
  It does not prove arbitrary branching usability, production parameter
  quality, N-image qualification, calibration, unseen robustness, or field
  readiness.
- The successful output does not qualify Filter, Threshold, Morphology, or
  Blob for a production inspection.
- No camera, lighting, PLC, I/O, MES, account, deployment, LLM, new
  algorithm, or automatic execution scope was added.

## Durable Completion Record

Status: Complete<br>
Scope: one blank-workspace four-Step direct-Teaching Recipe authored, saved,
reviewed, restarted, restored without execution, and explicitly run in a clean
current runtime.<br>
Acceptance criteria: named Recipe/image -> pass; four explicit
Preview/save Steps -> pass; predecessor-output selection -> pass; preset route
preservation/no-run -> pass; pre-restart route review -> pass; post-restart
exact restoration in WAIT -> pass; one explicit Run with drawings/object rows
-> pass.<br>
Verification: clean-runtime build; 335.33-second actual-EXE recording; exact
XML route/hash assertion; focused area-preset route/no-run smoke; Debug
solution build; visual frame review.<br>
Evidence:
`artifacts\p256_four_step_route_clarity_20260729`,
`artifacts\openvisionlab_clean_runtime_p256_before_20260729`, and this report.<br>
Boundary / next dependency: no route-selection blocker remains from this
bounded workflow. CVR-00 remains externally dependent on three real
independent first-time participants. Do not admit another product feature
without a named operator task or verified current-build regression.
