# OpenVisionLab Threshold Histogram Teaching

Updated: 2026-07-27 KST
Backlog item: `CVR-02`

## Outcome

The bounded Threshold gray-histogram teaching slice is complete. The existing
Threshold Tool now reuses the CVR-01 Tool Signal Inspector after a successful
explicit Preview:

- Basic publishes one editable `T` marker;
- Range publishes editable `Lower` and `Upper` markers;
- Adaptive retains its existing controls and publishes no misleading global
  cutoff chart.

This is evidence-assisted teaching of existing parameters. It is not automatic
threshold selection.

## Operator Contract

- The chart contains one 256-bin grayscale population for the current source.
- Provenance retains Threshold mode, input layer, `Full image` region,
  parameters, source/result SHA-256, and deterministic evidence ID.
- Marker drag is transient. Release updates only the existing Threshold
  teaching model, clears stale evidence synchronously, and schedules the
  existing debounced Preview.
- Range marker commits preserve `Lower <= Upper`.
- The distribution view is a full parameter-panel overlay with explicit
  `Back to parameters` and `Review distribution` actions. This preserves the
  usable docked parameter layout.
- Opening/closing the overlay, selecting, zooming, panning, cursor inspection,
  reset, and TSV export do not execute Preview/Run or mutate layers, active
  layer, or input/output routes.
- Changing the input or a Threshold parameter invalidates the retained
  evidence before a replacement Preview succeeds.

## Good/Bad Completion Gate

The Good distribution was first retained with `T=127`. The shared chart marker
then committed `T=130`, producing a new evidence identity against the same
source SHA-256. That taught value exactly matches the existing frozen public
Pipeline; the pair was then replayed without result-driven tuning:

| Reference | Frozen Threshold | Actual Pipeline outcome | Metric |
| --- | --- | --- | --- |
| `Public_Threshold_BandPads_Good` | Basic, `T=130`, Binary, Max 255 | OK | `ResultCount=4` |
| `Public_Threshold_BandPads_Missing_Bad` | Basic, `T=130`, Binary, Max 255 | Expected NG | `ResultCount=1` |

The Pipeline is
`docs/samples/public/Public_Threshold_BandPads.pipeline.xml`. The source
SHA-256 values, the Good before-teach `T=127` TSV, and separate frozen `T=130`
Good/Bad 256-bin TSVs are retained with the replay artifact. No parameter was
tuned after observing the outcomes.

## Ownership

| Responsibility | Owner |
| --- | --- |
| Common gray histogram and image identity | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/SignalInspection/OpenVisionNativeGraySignalEvidenceCalculator.cs` |
| Threshold evidence/marker contract | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/SignalInspection/OpenVisionNativeThresholdSignalEvidenceFactory.cs` |
| Marker drawing and release commit request | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalPlotSurface.cs` |
| Shared inspector presentation/export | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalInspectorView.xaml(.cs)` |
| Threshold overlay and stale-evidence policy | `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/ThresholdToolWpfView.xaml(.cs)` |
| Existing-model marker synchronization | `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/VisionToolThresholdInteractionController.cs` |
| Explicit Preview integration | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativeThresholdPreviewExecutor.cs` |

## Verification

Commands executed against the current Dev source:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false -m:1 -nr:false

dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false -m:1 -nr:false

dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"

dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_threshold_basic_tool "artifacts\cvr02_threshold_histogram_teaching_20260727\after" --quiet

dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_threshold_tool "artifacts\cvr02_threshold_histogram_teaching_20260727\focused" --quiet

dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_threshold_signal_good_bad_replay "artifacts\cvr02_threshold_histogram_teaching_20260727\replay" --quiet
```

Final verification results are recorded in the artifact README. The focused
assertions cover Basic/Range markers, release-to-model synchronization,
stale-evidence replacement, Adaptive exclusion, provenance/export, full-image
identity, public Good/Bad outcomes, and no layer/route/run side effects.

## Current-Source UI Evidence

- Before:
  `artifacts/cvr02_threshold_histogram_teaching_20260727/before/wpf_shell_host_threshold_basic_tool.png`
- After:
  `artifacts/cvr02_threshold_histogram_teaching_20260727/after/wpf_shell_host_threshold_basic_tool.png`
- Frozen Good/Bad replay:
  `artifacts/cvr02_threshold_histogram_teaching_20260727/replay/wpf_threshold_signal_good_bad_replay.png`

Visual comparison: the before view had the Threshold Basic controls but no
distribution evidence. The after view presents the current-Preview population,
the exact threshold marker, provenance, technical guidance, and plot controls
inside a dedicated overlay while preserving input/result previews and explicit
Tool actions.

## Boundary

- The current Threshold Tool has no ROI teaching contract. This slice claims
  `Full image` only.
- Adaptive Threshold has local cutoffs and no global editable marker.
- Good and Bad evidence is exported separately; no dual-population comparison
  chart is claimed.
- No automatic threshold optimizer, automatic acceptance gate, production
  qualification, unseen-data robustness, or field reliability is claimed.

## Completion Record

```text
Status: Complete
Scope: Threshold Basic/Range full-image grayscale population evidence, existing-value teaching markers, stale-evidence replacement, overlay review/export, and one frozen public Good/Bad replay.
Acceptance criteria: Successful Preview binds current provenance -> pass; Basic and Range markers synchronize only on release through the existing model/Preview policy -> pass; Adaptive avoids a misleading global chart -> pass; navigation/export preserve Preview/layer/route state -> pass; Good chart teaching changes T=127 to the frozen T=130 and the same T=130 Good/Bad replay returns ResultCount 4/1 -> pass; fresh current-source before/after evidence exists -> pass.
Verification: Current Debug/screenshot-runner builds, OpenVisionReadinessCheck, wpf_shell_host_threshold_basic_tool, wpf_shell_host_threshold_tool, wpf_threshold_signal_good_bad_replay, wpf_threshold_to_blob_detection_e2e, and wpf_simple_preprocess_result_review passed.
Evidence: docs/reports/OPENVISIONLAB_THRESHOLD_HISTOGRAM_TEACHING_20260727.md and artifacts/cvr02_threshold_histogram_teaching_20260727.
Boundary / next dependency: Full-image Basic/Range only; no automatic selection, Adaptive global cutoff, ROI histogram, dual Good/Bad overlay, or field qualification. CVR-00 remains the active external prerequisite; CVR-03 requires an exact current-source Line blocker or explicit user selection.
```
