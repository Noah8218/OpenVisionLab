# OpenVisionLab Tool View And Chained Processing Walkthrough

Date: 2026-07-28 KST
Status: Complete

## Outcome

The current Dev Debug EXE was operated through visible controls and recorded
with a naturally moving cursor. The completed evidence covers the product's
primary Tool View workflow and two deterministic preprocessing-to-detection
chains:

- Matching;
- Line Edge, distance/length measurement, and intersection;
- Blob;
- Contour;
- Filter;
- Morphology;
- `Filter -> Threshold -> Contour`;
- `Threshold -> Morphology -> Contour`.

The recordings exposed real defects in sample-to-Tool-View parameter handoff,
NormalizeImage valid-pixel bounds, Contour execution, and WPF shutdown. Those
defects were corrected and replayed before the README media was selected.

## Scope And Acceptance Criteria

Included:

- build the latest Dev source before actual-EXE capture;
- open and inspect the named Tool Views through visible UI controls;
- preserve explicit Preview/Run behavior;
- record full cursor motion without target-to-target teleportation;
- show direct Filter/Morphology operation and chained preprocessing;
- review the videos and representative frames;
- fix reproducible problems exposed by the recordings;
- create GitHub README GIF and MP4 assets;
- leave a durable next-chat record.

Excluded:

- production qualification, unseen-data robustness, certified metrology, or
  commercial-platform parity;
- camera, lighting, PLC, I/O, deployment, or account integration;
- changes to `C:\Git\OpenVisionLab`;
- commit or push in this slice.

| Criterion | Result | Evidence |
| --- | --- | --- |
| Current Debug solution build | Pass | `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: 0 warnings, 0 errors |
| Actual visible Tool View operation | Pass | Eight completed `.run.txt` records and source MP4 clips under `artifacts\operator_walkthrough_20260728` |
| Natural cursor movement | Pass | Capture timeline records 420–1150 ms stepped cubic Bézier moves, smooth acceleration/deceleration, curved approach, and final settling |
| Explicit Preview/Run preserved | Pass | Every direct Tool View timeline records an explicit Preview click; both chains record explicit Run Review |
| Matching/Line/Blob/Contour review | Pass | Matching 3 results; Line Edge/Measure/Intersection; Blob 12; Contour 5 |
| Filter/Morphology chained detection | Pass | Both three-Step Pipeline Review recordings completed and retained per-Step output review |
| Recording-exposed crashes repaired | Pass | Actual fixture replay completed; final Contour replay completed; no matching Application error event after final captures |
| README GIF/MP4 ready | Pass | Two reviewed GIF/MP4 pairs under `docs\assets\demo` |

## Actual EXE Results

The capture executable recorded in the final run records has SHA-256
`0FF4CB79BCB38545A6488A99FA0320490F669C58CD6F854EDD2779DC7ED5011C`.

| Workflow | Actual result | Source clip |
| --- | --- | --- |
| Matching Tool View | 3 detections, score 93.074, 90x75 public template | `tool_views_r2_matching\matching-tool-view.mp4` |
| Line Tool View | Edge: 1 line / 25 edges / length 144.8; Measure: 37 px / 0.222 mm / 24 detections; Intersection: point 500,573 / 50 edges | `tool_views_r2_line\line-tool-view.mp4` |
| Blob Tool View | Visible Basic preset, threshold 100 to 150 edit, 12 objects | `tool_views_r2_blob_threshold150\blob-tool-view.mp4` |
| Contour Tool View | Visible Basic preset, threshold 100 to 150 edit, 5 objects | `tool_views_r9_contour_threshold150\contour-tool-view.mp4` |
| Filter Tool View | Explicit Preview and visible output image | `tool_views_r2_filter\filter-tool-view.mp4` |
| Morphology Tool View | Explicit Preview and visible output image | `tool_views_r1_morphology\morphology-tool-view.mp4` |
| Filter chain | `Filter -> Threshold -> Contour`, explicit Run Review, all three outputs reviewed | `chains_r2_filter_fixed\filter-chain.mp4` |
| Morphology chain | `Threshold -> Morphology -> Contour`, explicit Run Review, all three outputs reviewed | `chains_r1_morphology_fixed\morphology-chain.mp4` |

All paths above are below
`artifacts\operator_walkthrough_20260728`. Earlier failed or semantically
incorrect trial recordings remain diagnostic evidence only and are not used
by the README.

## Problems Found And Corrections

### Sample Tool View did not inherit the sample's first Step

Matching initially opened with stale/default parameters instead of the public
sample template. The Sample Workflow presenter now retains the first enabled
`VisionPipelineStep`, and the workspace command applies that Step after opening
the corresponding Matching or Line Tool View. Auto Preview remains disabled.

### NormalizeImage actual-EXE process termination

The public Fixture path terminated in native contour extraction while finding
the valid normalized pixel bounds. `ResolveValidBounds` now scans the validated
single-channel mask in managed code and publishes a bounded rectangle. The
fixture smoke checks the rectangle, and the repaired actual-EXE fixture replay
completed without a crash.

### Contour actual-EXE access violation

CLI and direct-view smoke hosts passed while the real WPF EXE repeatedly
terminated in the native `Cv2.FindContours` return-marshalling path. Library-Noah
now labels the binary image with `OpenCvSharp.Blob` and converts each retained
outer/internal contour chain into the existing Contour result model. It keeps
the existing area, center, bounds, angle, result, and drawing contracts.

The final Library-Noah source, OpenVisionLab vendored DLL, and current Debug
DLL all have:

- assembly version `2.1.0.0`;
- file version `2.8.0.0`;
- SHA-256
  `AA30B922C925A7AE7A169F89DA1C132205B1C130BF9C6863C44BE04099980DC3`.

### Normal application close raised an exception

The bootstrap previously reassigned `ShutdownMode` after the dispatcher had
already shut down. It now restores the previous mode only while the dispatcher
is active. Final actual-EXE captures closed normally and no matching Application
error event was recorded after the final-capture start time.

### Capture reliability and semantic review

- The capture tool minimizes other OpenVisionLab windows during a scenario and
  restores them afterward.
- Line signal evidence opens after Preview, so the script returns through the
  visible Back control before selecting the next Line purpose.
- Filter/Morphology completion waits for output preview visibility rather than
  a detector result summary.
- Blob and Contour final clips visibly teach threshold 150 before Preview.
  This avoids presenting the earlier 0-object Blob and 6-object Contour trials
  as successful product evidence.

## README Media

| Asset | Content |
| --- | --- |
| `docs\assets\demo\openvisionlab_tool_views_actual_exe.gif` | 2x2 Matching, Line, Blob, Contour comparison |
| `docs\assets\demo\openvisionlab_tool_views_actual_exe.mp4` | 17.4-second companion MP4 |
| `docs\assets\demo\openvisionlab_preprocess_chains_actual_exe.gif` | 2x2 direct Filter/Morphology and chained Pipeline Review comparison |
| `docs\assets\demo\openvisionlab_preprocess_chains_actual_exe.mp4` | 16.93-second companion MP4 |

Reviewed contact sheets:

- `artifacts\operator_walkthrough_20260728\tool_views_montage_contact_sheet.png`;
- `artifacts\operator_walkthrough_20260728\preprocess_montage_contact_sheet.png`.

## Verification

Passed:

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug -- --object-dimension-filter-contract artifacts/operator_walkthrough_20260728/contour_dimension_contract_final
dotnet run --project tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -- --target "wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool" artifacts/operator_walkthrough_20260728/final_toolview_smoke
dotnet run --project tools/OpenVisionFixtureSmoke/OpenVisionFixtureSmoke.csproj -c Debug
dotnet build Lib.OpenCV/Lib.OpenCV.csproj -c Release
dotnet run --project Lib.Inspection.Smoke/Lib.Inspection.Smoke.csproj -c Release
```

Results:

- solution build: 0 warnings, 0 errors;
- Library-Noah Release build: 0 warnings, 0 errors;
- Library-Noah inspection smoke: 66/66 passed;
- Blob/Contour dimension contract: pass, including exact reject reasons and
  legacy missing-key behavior;
- Fixture/NormalizeImage smoke: pass, including identity, angle/scale extremes,
  fail-closed dimension/coverage cases, and valid normalized-pixel bounds;
- five current-source Tool View smoke targets: all `OK|check=OK`;
- final actual-EXE `.run.txt` files: all `Status=Complete`;
- final post-capture matching crash events: 0;
- source/vendored/Debug Library-Noah DLL hashes: identical.

## Durable Completion Record

Status: Complete
Scope: Actual-EXE Tool View operation, natural-cursor recordings, direct
Filter/Morphology Preview, two three-Step preprocessing-to-Contour runs,
recording-exposed crash repairs, README GIF/MP4 assets, and durable handoff.
Acceptance criteria: Current build pass; named Tool Views pass; natural cursor
pass; explicit Preview/Run pass; both chains pass; crash replays pass; README
media pass.
Verification: Commands and results listed above, frame/contact-sheet review,
timeline review, DLL hash comparison, and Application event check.
Evidence: `artifacts\operator_walkthrough_20260728`,
`docs\assets\demo`, and this report.
Boundary / next dependency: This proves public synthetic actual-EXE workflows,
not independent novice usability or production robustness. The next external
product-study prerequisite is actual CVR00 novice participants.
