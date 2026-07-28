# OpenVisionLab Circle Residual Review

Updated: 2026-07-28 KST

Status: Complete

## Scope

This record closes the bounded `CVR-04` Circle radial-sample, inlier/outlier,
and residual review. It adds current-Run evidence to the existing
`CircleGauge` Pipeline Review without adding a new algorithm family or changing
CircleGauge edge selection, fitting, robust rejection, gates, XML, or
calibration semantics.

The named blocker was reproduced in the pre-change Geometry Review: it retained
typed Circle/Center results and aggregate metrics, but an operator could not
inspect which radial scans failed contrast, which edge candidates were removed
by robust fitting, or how one residual mapped back to its image scan.

## Completed Contract

For an executed `CircleGauge` Step:

1. The existing runtime loop retains every taught radial scan with:
   - stable scan index and angle;
   - source-image scan endpoints;
   - prepared gray intensity and signed polarity response;
   - selected edge point, radius, strength, and signed response;
   - contrast acceptance;
   - final robust-fit inlier/outlier state;
   - signed radius residual and exact reject reason.
2. Evidence is attached to the actual `VisionToolResult`, including failed
   support, fitting, radius, and residual outcomes. It is not reconstructed
   approximately from the final bitmap.
3. The current edge selection, initial least-squares fit, existing
   `max(1.5, 2.5 * initial RMS)` robust rejection, refined fit, support, radius,
   and residual gates remain the only runtime decision path.
4. Pipeline Review exposes a Circle Evidence tab only for CircleGauge. It
   presents taught/fitted values, support/coverage/RMS gates, the complete
   sample table, absolute-residual series, and selected radial
   intensity/signed-response series.
5. Selecting a table row, residual plot position, or compact drawing resolves
   to the same stable radial scan. The selected scan, fitted circle, edge point,
   state, and residual use the actual source-image coordinates.
6. Review selection requests no new Run and does not create/select layers or
   rewrite routes.
7. Invalid scans without an edge point are no longer drawn at the default
   `(0,0)` coordinate. This corrects drawing fidelity only; detection and
   acceptance are unchanged.

## Frozen Replay

The Good circle and Bad ellipse used identical settings:

```text
ROI=100,50,200,200
TaughtCenter=(200,150)
RadiusRange=50..80 px
AngleRange=0..360 deg
ScanCount=180
Polarity=LightToDark
MinimumContrast=40 GV
MinimumSupport=0.8
MaximumFitResidual=1 px
```

| Role | Outcome | Fit evidence | Sample evidence |
| --- | --- | --- | --- |
| Good circle | Pass | `R=67.831 px`, support `0.917`, coverage `330 deg`, RMS `0.517 px <= 1 px` | 180 scans; 171 edge candidates; 165 inliers; 9 contrast rejects; 6 robust-fit outliers |
| Bad ellipse | Reject | `CircleGauge fit residual 3.427px exceeds 1px.` | Same frozen teaching and gate settings |

The Good robust-rejection threshold was
`6.5145156250082 px`. Its retained evidence ID is
`644E195C02272D2A4590DC41579C8A153121CABFC3B131F8704DCDFEC7AF648A`.

The Bad retained evidence ID is
`259E098F5F8CE2F82DD46B9E1207D32B46E1D8925B96552CA32D0ABEE5DC14DC`.

Decoded source/result identity:

| Role | Source bitmap SHA-256 | Result bitmap SHA-256 |
| --- | --- | --- |
| Good | `6F4FFDBE7403A5A3A67B1BBAFB45A9DC8DA27624A17FD84C043448637D30D277` | `81B4F1E4642F4D10620E0E5B9646A52347FC9CFFD94FD598123CB684FC154279` |
| Bad | `95A93665D48FD4FAF1EE3659FB860A4FFF47FBD4FCD9C37323960206584CB347` | `84F8D787210F5293B9361503E21832BC6F068085CB736DEFB514055295899E96` |

## UI Evidence

Current-source baseline captured before CVR-04:

- `artifacts/cvr04_circle_residual_review_20260728/before/p213_geometry_review.png`

Final current-source evidence:

- `artifacts/cvr04_circle_residual_review_20260728/final/cvr04_circle_residual_review.png`
- `artifacts/cvr04_circle_residual_review_20260728/final/cvr04_circle_residual_review.diagnostics/circle-selected-profile.png`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-good-source.png`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-good-result.png`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-bad-source.png`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-bad-result.png`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-good-residuals.tsv`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-good-selected-profile.tsv`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-bad-residuals.tsv`
- `artifacts/cvr04_circle_residual_review_20260728/final/circle-evidence-replay.txt`

Visual inspection confirmed that the summary, sample table, selected
fit-outlier row, compact reviewed drawing, residual plot, and profile plot are
readable and not clipped. The selected outlier is visible in the table,
drawing, and residual plot as the same sample.

## Verification

Commands:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -p:UseWpfAppHost=false
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target cvr04_circle_residual_review "artifacts\cvr04_circle_residual_review_20260728\final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p213_geometry_review "artifacts\cvr04_circle_residual_review_20260728\regression_geometry_final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p213_geometry_property_grid "artifacts\cvr04_circle_residual_review_20260728\regression_geometry_final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p214_two_point_scale "artifacts\cvr04_circle_residual_review_20260728\regression_geometry_final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_line_signal_profile "artifacts\cvr04_circle_residual_review_20260728\regression_signal_line"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_threshold_signal_good_bad_replay "artifacts\cvr04_circle_residual_review_20260728\regression_signal_threshold_replay"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_threshold_tool "artifacts\cvr04_circle_residual_review_20260728\regression_signal_threshold_tool"
dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug
```

Passed evidence:

- `cvr04_circle_residual_review`: frozen Good pass and Bad residual reject,
  exact scan-state counts, one residual series, two selected-profile series,
  source/result identity, TSV/replay records, row/plot/drawing selection, and
  zero Run Review requests;
- `p213_geometry_review`, `p213_geometry_property_grid`, and
  `p214_two_point_scale`;
- `wpf_line_signal_profile`, `wpf_threshold_signal_good_bad_replay`, and
  `wpf_shell_host_threshold_tool`;
- solution and screenshot-runner Debug builds with zero warnings and zero
  errors;
- readiness contract.

## Boundary

- Circle evidence is retained for the current in-memory Pipeline Run. This
  slice does not add it to saved Run Report/history persistence.
- All new values are pixel geometry. It does not add camera/lens calibration,
  distortion or perspective correction, non-uniform scale, or certified
  metrology.
- The synthetic Good/Bad pair proves the diagnostic correspondence and one
  unchanged frozen-gate replay. It does not prove unseen-data robustness,
  production accuracy, or field qualification.
- No new CircleGauge parameter, XML key, detector, fitting path, automatic
  suggestion, or acceptance rule was introduced.
- `CVR-05` and later commercial-video candidates remain conditional.

## Durable Closure

```text
Status: Complete
Scope: Current-Run CircleGauge radial sample evidence, contrast/inlier/outlier and exact reject states, residual/profile review, provenance, and row/plot/drawing selection.
Acceptance criteria: Actual runtime samples retained -> pass; intensity and signed response -> pass; accepted/rejected/inlier state and exact reason -> pass; residual/support/gate review -> pass; row/plot/drawing identity -> pass; frozen Good pass and Bad residual reject -> pass; zero review-triggered Run -> pass; no CircleGauge fit/XML/gate semantic change -> pass.
Verification: Debug solution and screenshot-runner builds; cvr04_circle_residual_review; three related geometry smokes; three shared signal regressions; readiness.
Evidence: docs/reports/OPENVISIONLAB_CIRCLE_RESIDUAL_REVIEW_20260728.md and artifacts/cvr04_circle_residual_review_20260728.
Boundary / next dependency: Current-run pixel diagnostic only; no saved Run Report persistence, algorithm/gate/calibration change, unseen robustness, certified metrology, or field qualification claim. CVR-00 independent novice observations remain the active external prerequisite; CVR-05 requires its exact labelled Blob/Contour population blocker or explicit user selection.
```
