# OpenVisionLab Video-Gated Effective ROI Clarity

Date: 2026-07-29 KST

## Outcome

Status: **Complete**

Blob and Contour Teaching now state the effective inspection scope when
`USE_ROI=true` but the single ROI has zero width or height:

`전체 이미지 (ROI 미지정)`

This matches the existing Library-Noah runtime contract. Blob normalizes a
zero-width or zero-height single ROI to the complete source image before
labeling. The saved property values, algorithm behavior, results, and explicit
Preview contract were not changed.

## Scope

Included:

- effective ROI wording in the shared Blob/Contour area-verification summary;
- Korean and English localization;
- focused UI assertion for the zero-size fallback;
- explicit assertion that displaying the explanation causes no Preview;
- current-build actual-EXE before/after Teaching recordings;
- one transient UIAutomation retry hardening in the recording helper.

Excluded:

- changing `USE_ROI`, `CvROI`, presets, or saved tool settings;
- auto-initializing an ROI from the source image;
- changing Blob/Contour processing or acceptance;
- automatic Preview/Run, layer changes, or route changes;
- Basic/Fast/Precise explanation and threshold-selection guidance.

## Acceptance Criteria And Evidence

1. The current mismatch is reproducible: pass. Before evidence shows checked
   `ROI 사용`, `(x:0 y:0 width:0 height:0)`, and the old effective summary
   `ROI 사용`.
2. Runtime semantics are grounded: pass. The consumed Library-Noah
   `BlobTool.NormalizeBlobRoi` maps zero width or height to
   `0,0,imageWidth,imageHeight`; validation explicitly allows that fallback.
3. The visible effective scope is explicit: pass. Current-source UI and the
   after video show `전체 이미지 (ROI 미지정)`.
4. Displaying the explanation causes no execution: pass. The focused Blob UI
   smoke asserts `NativePreviewRunCount == 0` immediately after the initial
   explanation is shown.
5. The established Teaching result is unchanged: pass. Before and after use
   Basic, threshold 150, one explicit Preview, and both report 12 detections
   with maximum area 892 and box 33x33.
6. Build, localization, and focused UI checks pass: pass.

## Verification

```text
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/RunUiScreenshotSmoke.ps1 `
  -Targets "wpf_shell_host_blob_tool,localization_catalog_contract_check"
```

Results:

```text
Build: 0 warnings, 0 errors
wpf_shell_host_blob_tool=OK
localization_catalog_contract_check=OK
```

Actual-EXE after replay:

```text
Status=Complete
EXE=bin/Debug/OpenVisionLab.exe
EXESHA256=239C081C7B05BA2E825F661FB2F468FA98E207480CA8331FA3793068629F010E
Result=Blob / 12 detections / max area 892 / box 33x33
Video=1920x1080, 30 fps
```

The first after-capture attempt stopped before sample selection because the
Windows UIAutomation root briefly returned an unavailable-element error. The
recording helper now retries that read and skips stale button elements. Its
PowerShell parse check and the complete `after_r2` replay passed. This was not a
product runtime failure.

## Evidence

- Before actual-EXE video:
  `artifacts/video_gated_roi_clarity_20260729/before/novice-blob-teaching-self-trial.mp4`
  (`B4FEA27E26CEA0E4044722CBF79B60852AB1F9D434E8B51B8950269615D976BD`)
- After actual-EXE video:
  `artifacts/video_gated_roi_clarity_20260729/after_r2/novice-blob-teaching-self-trial.mp4`
  (`26D770C0D02A7A9581BA4F9EC138F496087FF2F1DB7D90569F510F0B817363F0`)
- Before/after comparison:
  `artifacts/video_gated_roi_clarity_20260729/comparison/roi_before_after.png`
- Focused current-source UI:
  `artifacts/video_gated_roi_clarity_20260729/ui_smoke/wpf_shell_host_blob_tool.png`
- Timelines and runtime identities:
  `artifacts/video_gated_roi_clarity_20260729/before` and
  `artifacts/video_gated_roi_clarity_20260729/after_r2`

## Boundary And Next Priority

This proves the effective-scope explanation for the recorded Blob Teaching path
and the shared Blob/Contour area-summary rule only. It does not qualify
arbitrary tools to accept zero-size ROI, because some specialized tools require
one non-empty operator-reviewed ROI and fail closed.

Next priority: audit the visible Basic/Fast/Precise tradeoffs together with
threshold-selection guidance in direct Teaching. Admit one bounded video-gated
correction only if the current UI leaves the operator without a reasonable
choice path. Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

## Durable Completion Record

```text
Status: Complete
Scope: Effective full-image explanation for zero-size single ROI in Blob/Contour area Teaching, localization, focused no-auto-Preview assertion, and actual-EXE before/after evidence.
Acceptance criteria: mismatch reproduced -> pass; Library-Noah fallback grounded -> pass; effective scope visible -> pass; explanation causes zero Preview -> pass; explicit 12-detection replay unchanged -> pass; build/localization/UI checks -> pass.
Verification: Debug build passed with 0 warnings/errors; wpf_shell_host_blob_tool and localization_catalog_contract_check passed; PowerShell capture helper parsed; complete actual-EXE after_r2 replay passed.
Evidence: artifacts/video_gated_roi_clarity_20260729; docs/reports/OPENVISIONLAB_VIDEO_GATED_ROI_CLARITY_20260729.md
Boundary / next dependency: Blob/Contour zero-size fallback wording only; no runtime or preset change, no CVR-00 completion, and no claim for specialized tools that require non-empty ROI.
```
