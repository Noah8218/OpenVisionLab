# OpenVisionLab Tool Signal Inspector Foundation

Updated: 2026-07-27 KST
Backlog item: `CVR-01`

## Outcome

The shared Tool Signal Inspector foundation is complete. It is a reusable,
read-only evidence surface bound to one current Preview result. The first
representative integration is the existing Histogram Tool, which now shows
the source and processed 256-bin grayscale distributions after a successful
Preview.

This implementation does not complete the separate Threshold, Line, Circle,
Blob/Contour, or matcher diagnostic rows.

## Product Contract

- Evidence identity retains the tool, selected input layer, reviewed region,
  parameter summary, source SHA-256, result SHA-256, axes, named series, and a
  deterministic evidence ID.
- The shared WPF plot supports X-axis mouse-wheel zoom, left-drag pan, cursor
  values, reset, legend, and explicit TSV export.
- A Histogram parameter or input-layer change clears the previous chart
  synchronously. The existing debounced Preview contract creates replacement
  evidence; the chart never presents an old curve as current.
- Plot navigation, reset, and export do not run Preview/Run, edit parameters,
  create/select a layer, change the active layer, or mutate input/output routes.
- The TSV includes the full provenance header and all 256 grayscale bins for
  both `Source` and `Result`.

## Ownership

| Responsibility | Owner |
| --- | --- |
| Immutable signal/provenance data | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalEvidence.cs` |
| Provenance-preserving TSV | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalEvidenceExporter.cs` |
| Shared plot gestures/rendering | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalPlotSurface.cs` |
| Shared inspector presentation | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SignalInspection/VisionToolSignalInspectorView.xaml(.cs)` |
| Histogram data extraction/hash | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/SignalInspection/OpenVisionNativeHistogramSignalEvidenceFactory.cs` |
| Histogram Preview integration | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativeSimplePreprocessPreviewExecutor.cs` |

The Histogram view reuses the shared inspector. Future tool-specific rows must
produce the shared evidence model and prove their own image-coordinate/drawing
identity instead of cloning a chart implementation.

## Verification

Commands executed against the current Dev source:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false -m:1 -nr:false

dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -p:WpgCustomBuildEnabled=false -m:1 -nr:false

dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_simple_preprocess_result_review "artifacts\cvr01_tool_signal_inspector_20260727\after" --quiet

dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_preprocess_output_preview_flow "artifacts\cvr01_tool_signal_inspector_20260727\focused" --quiet
```

Results:

- Debug/screenshot-runner builds: PASS, 0 warnings, 0 errors.
- `wpf_simple_preprocess_result_review`: PASS, `920x660`, zero
  layout/text/internal failures.
- `wpf_preprocess_output_preview_flow`: PASS, `920x660`, zero
  layout/text/internal failures.
- Focused assertions: two series, 64-character evidence/source hashes,
  stale-evidence clear and replacement, zoom/pan navigation, 256-bin TSV with
  provenance, and unchanged Preview count/layers/active layer/routes for
  navigation/reset/export.

## Current-Source UI Evidence

- Before:
  `artifacts/cvr01_tool_signal_inspector_20260727/before/wpf_simple_preprocess_result_review.png`
- After:
  `artifacts/cvr01_tool_signal_inspector_20260727/after/wpf_simple_preprocess_result_review.png`

Visual comparison: the before view contained Histogram parameters and the
mean/contrast result explanation only. The after view adds the current-Preview
signal card above the parameters while retaining input/output previews, result
review, layer selectors, and the explicit Preview button. No existing primary
control is removed.

## Boundary

- Full-image grayscale populations only; no ROI marker or linked drawing
  selection is claimed by this representative Histogram integration.
- No automatic threshold, parameter, or acceptance-gate selection.
- No Threshold dual markers (`CVR-02`), Line intensity/edge response
  (`CVR-03`), Circle samples/residuals (`CVR-04`), object distributions
  (`CVR-05`), or matcher diagnostics (`CVR-06`).
- No production, inspection, calibration, or field qualification claim.

## Completion Record

```text
Status: Complete
Scope: Shared Tool Signal Inspector evidence, read-only interactive plot, TSV export, stale-evidence policy, and one current Histogram Preview integration.
Acceptance criteria: Current evidence/provenance retained -> pass; zoom/pan/cursor/reset/export available -> pass; stale parameter evidence blocked and replaced -> pass; no reset/navigation/export Preview/layer/route side effects -> pass; fresh before/after current-source UI evidence -> pass.
Verification: Current Debug and screenshot-runner builds passed with 0 warnings/errors; wpf_simple_preprocess_result_review and wpf_preprocess_output_preview_flow passed.
Evidence: docs/reports/OPENVISIONLAB_TOOL_SIGNAL_INSPECTOR_FOUNDATION_20260727.md and artifacts/cvr01_tool_signal_inspector_20260727.
Boundary / next dependency: CVR-02 through CVR-06 remain conditional and require their own named operator blocker or explicit user selection; CVR-00 independent novice observations remain the active external prerequisite.
```
