# OpenVisionLab Actual EXE Operator Walkthrough And Self-Evaluation

Date: 2026-07-28 KST
Status: Incomplete

> Historical diagnostic record. The defects and missing Tool View/chained
> processing evidence identified here were subsequently corrected and replayed.
> The completed follow-up record is
> `OPENVISIONLAB_TOOL_VIEW_AND_CHAIN_WALKTHROUGH_20260728.md`. Do not use this
> report's `Incomplete` state as the current project status.

## Outcome

The current Debug EXE was operated through the visible UI while the desktop
and mouse cursor were recorded. The public Blob Good sample completed the
intended flow:

`empty workspace -> public sample catalog -> sample search/open -> Pipeline Review -> explicit Run Review -> OK result`

The successful segment was reviewed frame by frame and converted into a
GitHub README GIF and a companion MP4. The program cannot be assessed as
problem-free, however. A separate actual-EXE Fixture run terminated in native
OpenCV contour processing, and normal window closure repeatedly raised an
unhandled WPF shutdown exception.

No production implementation was changed in this evaluation slice. The defects
below are preserved as the next evidence-based corrective work.

## Scope And Acceptance Criteria

Included:

- build and run the current Dev EXE;
- operate it through visible controls rather than an embedded smoke facade;
- record the cursor and application at 1920x1080;
- move the cursor with human-like acceleration, deceleration, curvature, and
  settling instead of coordinate teleportation;
- review the successful video and representative frames;
- attempt both a basic Blob path and a fixture/relative-ROI path;
- produce a GitHub-appropriate GIF and companion MP4;
- record any observed defects and evidence boundaries.

Excluded:

- fixing defects found during this diagnostic/evaluation request;
- claiming novice usability, production qualification, field robustness, or
  parity with a commercial vision platform;
- touching or publishing to `C:\Git\OpenVisionLab`.

| Criterion | Result | Evidence |
| --- | --- | --- |
| Current Debug EXE built before capture | Pass | `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: 0 warnings, 0 errors |
| Visible actual-EXE operation recorded | Pass | `artifacts\operator_walkthrough_20260728\raw_r2\blob-good-bad.mp4` |
| Cursor does not teleport between targets | Pass | Five recorded moves took 582–970 ms; the capture tool uses stepped cubic Bézier motion, smoothstep acceleration/deceleration, randomized arc, and final micro-settling |
| Explicit Run Review visible and successful | Pass | Timeline reaches `review-complete OK / 7.7 ms`; contact sheet and final frames show the Pipeline Review result |
| Complete Good/Bad comparison recorded | Fail | Good completed; the attempt did not reach the paired Bad case because the script's Korean-name lookup was decoded incorrectly by Windows PowerShell |
| Fixture/relative-ROI path completes | Fail | Actual EXE terminated with `System.AccessViolationException` in `ResolveValidBounds` |
| Application closes without an unhandled exception | Fail | Repeated `.NET Runtime` event 1026 identifies `OpenVisionLabApplication.Run`, line 53 |
| README GIF and MP4 produced and checked | Pass | 17.5-second GIF and MP4 under `docs\assets\demo` |
| Recorded UI visually reviewed | Pass | Full contact sheet plus start/end/key-frame inspection |

## Tested Build

- EXE: `C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`
- EXE timestamp: `2026-07-28T11:51:11.6026305+09:00`
- EXE SHA-256:
  `0FF4CB79BCB38545A6488A99FA0320490F669C58CD6F854EDD2779DC7ED5011C`
- Runtime reported by Windows event evidence: .NET 8.0.28

## Natural Mouse Contract

The reusable capture script is:

`tools\OperatorWalkthroughCapture\Record-OperatorWalkthrough.ps1`

Each movement:

1. reads the current cursor position;
2. chooses a duration from distance, bounded to 420–1150 ms;
3. builds a cubic Bézier curve with a small randomized perpendicular arc;
4. advances in approximately 12 ms increments;
5. applies smoothstep acceleration/deceleration;
6. performs small final settling movements before the press/release click.

Typing is sent character by character. The raw timeline records the semantic
action, start/end coordinate, and duration. In the successful run, target
movements were 864, 744, 801, 582, and 970 ms. This provides inspectable
evidence that the cursor did not jump instantly between controls.

## Successful Actual-EXE Walkthrough

Source video:

`artifacts\operator_walkthrough_20260728\raw_r2\blob-good-bad.mp4`

Source timeline:

`artifacts\operator_walkthrough_20260728\raw_r2\blob-good-bad.timeline.tsv`

Observed sequence:

| Time | Observation |
| ---: | --- |
| 0.018 s | Current EXE ready at the empty workspace |
| 2.622–4.338 s | Natural move and click on the public sample catalog |
| 6.491–10.822 s | Natural move, click, and character-by-character search for `Public_Blob_Particles_Good` |
| 13.078–17.301 s | Natural move, open selected sample, and sample load |
| 18.264–21.995 s | Natural move and open Pipeline Review before execution |
| 23.717–26.687 s | Natural move, explicit Run Review click, and `OK / 7.7 ms` completion |

The reviewed frames show a coherent operator story: the sample is searchable,
the public result is visible before review, Pipeline Review makes execution an
explicit action, and the final view exposes the Step sequence, intermediate
images, and OK status together. These are strong parts of the current product.

The raw recording continues after the successful result because the automation
looked for a Korean `다음` label using a Windows PowerShell decoding path that
produced mojibake. That automation failure is not used as product-failure
evidence and was removed from the README segment.

## README Demo Assets

| Asset | Media | Size | SHA-256 |
| --- | --- | ---: | --- |
| `docs\assets\demo\openvisionlab_rule_based_workflow.gif` | 1100x619, 10 fps, 175 frames, 17.5 s | 1,102,030 bytes | `7532B1B323ED14F36D6EE9B3330B65EC73FD9903D646EB013055EE92AECB7470` |
| `docs\assets\demo\openvisionlab_rule_based_workflow.mp4` | H.264, 1280x720, 30 fps, 17.5 s | 458,233 bytes | `B77983C1D5970A2B0F39CDB5ADF8B75B2D137C320439B890F3A5C712E8F53F48` |

The README embeds the GIF and links the MP4. The caption explicitly limits the
claim to one public synthetic workflow. The GIF starts at the sample-selection
state and ends after the explicit successful Run Review; failed automation and
crash material are not presented as a product demo.

Visual review evidence:

- `artifacts\operator_walkthrough_20260728\final\blob_good_walkthrough_contact_sheet.png`
- `artifacts\operator_walkthrough_20260728\final\readme_demo_contact_sheet.png`
- `artifacts\operator_walkthrough_20260728\final\readme_demo_frames`

## Problems Found

### 1. Critical: NormalizeImage review can terminate the process

Reproduction:

1. Start the current Debug EXE.
2. Open the public
   `Public_Fixture_Normalize_RelativeRoi_Good` workflow.
3. Open Pipeline Review.
4. Click Run Review.

Observed result:

- the process terminated rather than returning a failed Step result;
- Windows `.NET Runtime` event 1026 at 2026-07-28 13:12:28 KST reports
  `System.AccessViolationException`;
- the stack reaches
  `OpenCvSharp.NativeMethods.imgproc_findContours1_vector`,
  `Cv2.FindContours`,
  `VisionPipelineNormalizeImageTool.ResolveValidBounds`, and
  `VisionPipelineNormalizeImageTool.Execute`;
- paired Application Error event 1000 reports exception code `0xc0000005` in
  `coreclr.dll`.

Current source location:

`src\OpenVisionLab\Core\Pipeline\Tools\VisionPipelineNormalizeImageTool.cs`,
`ResolveValidBounds`, where `Cv2.FindContours(validMask, ...)` is called.

Assessment: this is a release-blocking reliability defect for any public
workflow that reaches the failing NormalizeImage path. It must be converted
from a native process termination into valid deterministic execution or a
fail-closed Step result, with an actual-EXE regression.

### 2. High: normal window closure raises an unhandled shutdown exception

Observed result:

- `.NET Runtime` event 1026 was recorded repeatedly at 13:19:25, 13:21:57,
  13:23:30, 13:26:40, and 13:27:44 KST;
- each event reports
  `System.InvalidOperationException: 애플리케이션이 종료 중이거나 이미 종료되었으면 ShutdownMode를 설정할 수 없습니다.`;
- the location is `OpenVisionLabApplication.Run`, line 53.

Current source location:

`src\OpenVisionLab\App\Bootstrap\OpenVisionLabApplication.cs`; the `finally` block assigns
`application.ShutdownMode = OnExplicitShutdown` after `Application.Run`
returns.

Assessment: even though the window disappears, this is not a clean application
shutdown and pollutes crash telemetry. The lifecycle contract needs a focused
normal-close and exceptional-close regression.

### 3. Medium, confirmation needed: Sample Catalog is not persistently visible
from a no-input active Tool View

One current-source launch exposed the Filter Tool View without an input image.
The empty-workspace sample entry was no longer visible, while UI Automation
still exposed the hidden onboarding element as though it were available. This
both blocked the external operator automation and left no obvious public-sample
recovery path in that visible state.

Assessment: confirm the startup/session-restoration condition manually. If
reproduced, add or expose a stable Sample Catalog entry in the persistent shell
and correct the automation visibility semantics. This observation is not yet a
release-blocking claim because the exact state-restoration prerequisite has not
been isolated.

## Self-Evaluation

What worked well:

- the public sample catalog provided a concrete, low-friction entry from the
  empty workspace;
- sample search and open were legible at the recorded resolution;
- Pipeline Review preserved the important explicit-Run contract;
- after execution, Step order, intermediate images, and the overall result
  were visible in one review surface;
- the workflow is understandable enough to produce a concise README demo
  without staged or synthetic UI.

What remains weak:

- one public fixture path can terminate the whole process instead of failing a
  Step safely;
- closing the application is not clean in crash telemetry;
- the first-run/sample entry is tied too closely to the empty-workspace state;
- the successful recording proves one guided public sample, not independent
  first-time usability;
- a complete Good-to-Bad comparison and a repaired Fixture walkthrough must be
  re-recorded after the defects are fixed.

## Corrective Priority

1. Fix and regression-test the NormalizeImage native crash using the exact
   public fixture path and actual EXE.
2. Fix and regression-test the WPF shutdown lifecycle.
3. Confirm the no-input active Tool View recovery path; if reproduced, make
   Sample Catalog persistently reachable and automation-visible.
4. Re-record one complete Good/Bad Blob walkthrough and the repaired
   Fixture/multiple-relative-ROI walkthrough with the same natural-mouse
   contract.
5. After those corrections, proceed with the existing `CVR-00` independent
   novice study. The recording is self-evaluation evidence, not a substitute
   for three novice participants.

## Durable Completion Record

Status: Incomplete
Scope: Actual current-EXE operation, natural-cursor recording, reviewed public
Blob Good path, README GIF/MP4 generation, and defect discovery.
Acceptance criteria: Current EXE/build pass; natural cursor pass; successful
Blob Good review pass; README media pass; full Good/Bad recording fail;
Fixture execution fail; clean shutdown fail.
Verification: Debug solution build (0 warnings/errors); PowerShell capture
script syntax parse; FFprobe duration/dimension/frame checks; SHA-256 inventory;
timeline review; contact-sheet and key-frame visual review; Windows Application
event 1026/1000 inspection; `git diff --check`.
Evidence: `artifacts\operator_walkthrough_20260728`,
`docs\assets\demo`, this report, and the README demo section.
Boundary / next dependency: The two confirmed defects require a separate
implementation/fix slice before the walkthrough can be called complete. An
independent novice study still requires actual participants.
