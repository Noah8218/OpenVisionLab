# OpenVisionLab Line Signal Profile

Updated: 2026-07-28 KST

Status: Complete

## Scope

This record closes the bounded `CVR-03` Line intensity and edge-response
profile. It adds current-Preview diagnostic evidence to the existing Line Tool
View without changing `LineGauge`, `LineDistance`, XML, measurement, fitting,
or acceptance semantics.

The named operator blocker was reproduced on the public Line Pins measurement:
the current output drawing showed ROI, edge endpoints, sampling distances, and
the final measurement, but it could not explain why `WTOB`, minimum contrast
`18`, and thickness `2` selected one particular edge or why another visible
transition was not selected.

## Completed Contract

After a successful explicit Line Edge or Measure Preview:

1. The currently selected Line A/B result chooses the median successful scan
   row or column as one deterministic representative scan.
2. The diagnostic uses the same prepared grayscale/threshold/invert signal as
   the configured `LineGauge` property.
3. It publishes:
   - intensity in gray values;
   - signed response in scan direction;
   - polarity and minimum contrast;
   - thickness and sampling interval;
   - ROI and exact source-image scan endpoints;
   - selected first-stable edge point and signed response;
   - bounded spatially distinct alternatives and their reason;
   - source/result SHA-256 and deterministic evidence ID.
4. The diagnostic independently replays the existing runtime rule and refuses
   to publish unless its first contrast-plus-thickness-stable point exactly
   matches the retained `LineGauge` point.
5. The result image draws the representative scan, selected point, and bounded
   alternatives. The chart and drawing therefore use the same image
   coordinates.
6. The shared plot supports negative signed values and a visible zero axis.
   Existing positive Histogram and Threshold plots retain their behavior.
7. Parameter/input changes, active Tool input-image load, and replacement of
   the active `Main` workspace image clear stale Line evidence/result state
   without running Preview.
8. Open/back, cursor review, zoom, pan, reset, and TSV export do not run
   Preview/Run, create or select layers, change the active layer, or change
   input/output routes.

The Line Tool remains PropertyGrid-based. Signal review is a right-side overlay
so the input and current result drawings remain visible.

## Frozen Replay

The same Line A/B settings were used for both public images:

```text
ROI=430,170,125,145
Line A=X_LTOR
Line B=X_RTOL
Polarity=WTOB
Min contrast=18 GV
Thickness=2
Sampling step=6
Point range=8
Manual scan angle=89
```

| Role | Source file SHA-256 | Decoded source SHA-256 | Result | Representative evidence |
| --- | --- | --- | --- | --- |
| Good | `9CD5466296D4A660AA2B95809B81C4A877E0AB0D6CE65C55C3D5BC4C4747C49D` | `B036195D8A959F93DCFBB38292CABAB2C8A041B2B04F47AB54ACA85646302891` | `37 px / 0.222 mm / 24 edge points` | scan `(430,242)->(554,242)`; selected `(462,242)` at scan `32`, signed response `-26 GV`; later stable alternative `(500,242)` |
| WidePin Bad | `80F3B00D38D753EF6928B09A283050A8FF1B7C25D75DFAAC2CD9E43590EAE6F8` | `2B9727564A918465D091DB4CBBA765DF5783A48C729BDC5DADD2B5E68874DF14` | `17.7 px / 0.106 mm / 24 edge points` | scan `(430,242)->(554,242)`; selected `(478,242)` at scan `48`, signed response `-22 GV`; later stable alternative `(538,242)` |

Good evidence ID:
`C84ED25C884AABB14B25B79DC46E0805064AA939FBFB25D394074E9BF30489B0`.

Bad evidence ID:
`D52BB6CCF504B7112625566A3CA9F82960C2F6A7FEDC52003216A307F8273077`.

The selected coordinate moved by 16 pixels on the WidePin image while the
frozen teaching parameters stayed unchanged. This is the intended explanatory
evidence for the changed width; it is not a new classification rule.

## UI Evidence

Baseline before implementation:

- `artifacts/cvr03_line_signal_profile_20260728/before/wpf_shell_host_line_pins_measure_tool.png`

Final current-source evidence:

- `artifacts/cvr03_line_signal_profile_20260728/final/wpf_line_signal_profile.png`
- `artifacts/cvr03_line_signal_profile_20260728/final/wpf_line_signal_profile.diagnostics/line-signal-good.png`
- `artifacts/cvr03_line_signal_profile_20260728/final/wpf_line_signal_profile.diagnostics/line-signal-bad.png`
- `artifacts/cvr03_line_signal_profile_20260728/final/wpf_line_signal_profile.diagnostics/line-signal-good-preview.png`
- `artifacts/cvr03_line_signal_profile_20260728/final/wpf_line_signal_profile.diagnostics/line-signal-bad-preview.png`
- `artifacts/cvr03_line_signal_profile_20260728/final/line-signal-good.tsv`
- `artifacts/cvr03_line_signal_profile_20260728/final/line-signal-bad.tsv`
- `artifacts/cvr03_line_signal_profile_20260728/final/line-signal-replay.txt`

Visual inspection confirmed:

- no clipped buttons, axis labels, series legend, or selected marker;
- the signed curve crosses the visible zero axis;
- selected and later stable markers remain distinguishable;
- the current input and result drawings remain visible beside the inspector;
- the result drawing retains the measurement lines and adds only the reviewed
  scan/point diagnostic.

## Verification

Commands:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -p:UseWpfAppHost=false
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_line_signal_profile,wpf_shell_host_line_tool,wpf_shell_host_line_pins_measure_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool,wpf_shell_host_line_presets "artifacts\cvr03_line_signal_profile_20260728\regression_line_final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_threshold_signal_good_bad_replay,wpf_shell_host_threshold_tool,wpf_simple_preprocess_result_review "artifacts\cvr03_line_signal_profile_20260728\regression_signal_final"
dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug
```

Passed evidence:

- `wpf_line_signal_profile`:
  - `X_LTOR`, `X_RTOL`, `Y_TTOB`, and `Y_BTOT` first-stable replay;
  - public Good/Bad same-parameter replay;
  - two signal series, selected edge, distinct alternative, source/result
    identity, image coordinates, TSV, active `Main` replacement stale-clear,
    and no-side-effect checks;
- all five related Line targets plus Line presets;
- Threshold Good/Bad, Threshold Tool, and Histogram result-review regressions;
- solution and screenshot-runner builds with zero warnings and zero errors;
- readiness contract.

## Boundary

- One representative successful scan is shown. This slice does not display
  every sampling row at once or add row selection.
- A later stable transition is a diagnostic alternative. It is not an
  acceptance candidate, automatic parameter recommendation, or gate change.
- The diagnostic replays existing behavior but does not change
  `LineGauge`/`LineDistance` detection, fitting, distance, calibration, XML, or
  acceptance.
- The public pair and synthetic four-direction matrix do not prove unseen-data
  robustness, certified metrology, production accuracy, or field
  qualification.
- `CVR-04` and all later commercial-video candidates remain conditional.

## Durable Closure

```text
Status: Complete
Scope: Current-Preview representative Line intensity/signed-response evidence with exact first-stable runtime replay, source coordinates, result drawing, provenance, TSV, and Good/Bad verification.
Acceptance criteria: Shared inspector reused -> pass; intensity and signed response -> pass; polarity/contrast/selected point/distinct alternative/image coordinates -> pass; chart/drawing correspondence -> pass; stale clear and no review side effects -> pass; same-parameter Good/Bad and four-direction replay -> pass; no LineGauge/LineDistance semantic change -> pass.
Verification: Debug solution and screenshot-runner builds; wpf_line_signal_profile; five related Line smokes plus presets; Threshold/Histogram signal regressions; readiness.
Evidence: docs/reports/OPENVISIONLAB_LINE_SIGNAL_PROFILE_20260728.md and artifacts/cvr03_line_signal_profile_20260728.
Boundary / next dependency: Representative-scan diagnostic only; no algorithm, gate, calibration, unseen robustness, or field qualification claim. CVR-00 independent novice observations remain the active external prerequisite; CVR-04 requires its exact Circle blocker or explicit user selection.
```
