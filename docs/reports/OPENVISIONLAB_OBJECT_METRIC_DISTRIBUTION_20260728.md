# OpenVisionLab Object Metric Distribution

Updated: 2026-07-28 KST

Status: Complete

## Scope

This record closes the bounded `CVR-05` Blob/Contour object-metric
distribution. It extends the existing Pipeline Review Object Results surface
with current-Run distributions of the already retained `Area`, `BoundsWidth`,
and `BoundsHeight` values.

The named blocker was visible in the pre-change Object Results baseline:
accepted/rejected candidates, object bounds, and exact reject reasons existed,
but the operator had to read the population one row at a time and could not
see it relative to the current PropertyGrid range.

This slice does not add a descriptor or rerun Blob/Contour segmentation.

## Completed Contract

For an executed Blob or Contour Step:

1. Pipeline Review reuses only the current `VisionPipelineObjectResult` rows.
2. The operator explicitly selects one existing metric:
   - `Area`;
   - `Bounds width`;
   - `Bounds height`.
3. The selected metric reads exactly one existing Pipeline/PropertyGrid range:
   - `MIN_AREA` / `MAX_AREA`;
   - `MIN_WIDTH` / `MAX_WIDTH`;
   - `MIN_HEIGHT` / `MAX_HEIGHT`.
4. The shared signal-evidence model publishes accepted and rejected binned
   object counts as separate green/red series with:
   - tool, input layer, and reviewed region;
   - current range values;
   - object/accepted/rejected counts;
   - source/result SHA-256;
   - stable evidence ID;
   - provenance-preserving TSV output.
5. Finite lower/upper limits appear as read-only range markers. They do not edit
   the recipe or recommend a gate.
6. A missing legacy maximum retains the existing `1000000` unbounded
   compatibility sentinel. The review states `unbounded` and omits that
   sentinel from distribution scaling.
7. Table, selected-object drawing, and plot selection resolve to the same stable
   object number. The compact summary repeats the selected row's exact reject
   reason.
8. Metric switching and row/plot/image selection do not request another
   Preview/Run, create/select layers, or change input/output routes.

The Object Results table remains the source of full per-object center, bounds,
angle, accepted/rejected state, and reason data. The distribution is an
additional review surface, not a replacement.

## Frozen Five-Row Matrix

The focused UI matrix used:

```text
Tool=Blob
MIN_AREA=200
MAX_AREA=2000
MIN_WIDTH=10
MAX_WIDTH=40
MIN_HEIGHT=10
MAX_HEIGHT=50
Rows=5
Accepted=2
Rejected=3
```

Retained reasons included:

- `Area 12 < MIN_AREA 200`;
- `Area 2400 > MAX_AREA 2000`;
- `Width 44 > MAX_WIDTH 40`.

The same rows also created a separate Contour Area identity to prove that tool
identity is not collapsed.

| Evidence | Evidence ID |
| --- | --- |
| Blob Area | `667254AE2AB48DB634D895C4BA840FFDC09EF62A2B7857438AAB3E5041D04A85` |
| Blob Bounds width | `ADD42B071AE003F590DF39BAD13343AEA881E1BB92C84CCCA186D12DA4E9E8AD` |
| Blob Bounds height | `EDFBD7CCD23C88C75B6E9F53A4048F68B02820E4D96FEB3211BC61ECB4B05DBA` |
| Contour Area | `842446D33F5BB9591B05C97524A1BCD047C1E90EF963EB240861F0265F5C8506` |

The focused source/result bitmap SHA-256 was
`5065883D4744373621BDB37BAB7DE65F52A28CCD0CECC6C3A765FC6DF0A42D5B`.

## Public Product-Path Replay

The current public sample workflow exercised the actual Pipeline execution,
retained object rows, current Step parameters, Object Results view, and
selection contract.

| Role | Public sample | Result | Distribution checks |
| --- | --- | --- | --- |
| Blob Good | `Public_Blob_Particles_Good` | `ResultCount=12`, OK | 245 retained audit rows; two Area series; two finite markers; evidence `04B60625C28A...` |
| Blob Bad | `Public_Blob_Particles_Sparse_Bad` | `ResultCount=3`, NG | 253 retained audit rows; two Area series; two finite markers; evidence `BB330EEEBEAE...`; row/image and no-side-effect checks |
| Contour Bad | `Public_Contour_Shapes_Missing_Bad` | `ResultCount=2`, NG | 2 retained rows; two Area series; two finite markers; evidence `F31737C37EBF...`; row/image and no-side-effect checks |

These are bounded public samples. They demonstrate product-path integration,
not representative production distributions.

## UI Evidence

Current-source baseline captured before CVR-05:

- `artifacts/cvr05_object_metric_distribution_20260728/before/cvr05_object_metric_distribution.png`
- `artifacts/cvr05_object_metric_distribution_20260728/before_blob/wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics.png`
- `artifacts/cvr05_object_metric_distribution_20260728/before_contour/wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics.png`

Final current-source evidence:

- `artifacts/cvr05_object_metric_distribution_20260728/final/cvr05_object_metric_distribution.png`
- `artifacts/cvr05_object_metric_distribution_20260728/final/blob-area-distribution.tsv`
- `artifacts/cvr05_object_metric_distribution_20260728/final/blob-width-distribution.tsv`
- `artifacts/cvr05_object_metric_distribution_20260728/final/blob-height-distribution.tsv`
- `artifacts/cvr05_object_metric_distribution_20260728/final/contour-area-distribution.tsv`
- `artifacts/cvr05_object_metric_distribution_20260728/runtime_blob_good`
- `artifacts/cvr05_object_metric_distribution_20260728/runtime_blob_bad`
- `artifacts/cvr05_object_metric_distribution_20260728/runtime_contour_bad`

Visual inspection confirmed:

- the selected drawing, object table, and distribution remain visible together;
- Area/Bounds W/Bounds H actions are recognizable text controls with tooltips
  and accessible names;
- accepted/rejected lines and both finite range markers are distinguishable;
- the selected high-area reject repeats
  `Area 2400 > MAX_AREA 2000`;
- the main Pipeline Review navigation and input/output previews remain visible.

## Verification

Commands:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -p:UseWpfAppHost=false
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target cvr05_object_metric_distribution "artifacts\cvr05_object_metric_distribution_20260728\final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_workspace_sample_pipeline_review_metrics "artifacts\cvr05_object_metric_distribution_20260728\runtime_blob_good"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics "artifacts\cvr05_object_metric_distribution_20260728\runtime_blob_bad"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics "artifacts\cvr05_object_metric_distribution_20260728\runtime_contour_bad"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target cvr04_circle_residual_review "artifacts\cvr05_object_metric_distribution_20260728\regression_circle"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p213_geometry_review "artifacts\cvr05_object_metric_distribution_20260728\regression_geometry"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p214_two_point_scale "artifacts\cvr05_object_metric_distribution_20260728\regression_scale"
dotnet run --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --object-dimension-filter-contract "artifacts\cvr05_object_metric_distribution_20260728\regression_object_contract"
dotnet run --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug
```

Passed evidence:

- focused Blob Area/Width/Height and Contour Area distributions;
- finite exact marker values and legacy unbounded maximum behavior;
- two-way row/plot/image selection and zero Run Review requests;
- public Blob Good/Bad and Contour Bad product paths;
- existing direct/Recipe report object-row persistence and
  Preview/layer/route non-regression checks;
- existing Blob/Contour dimension filter and reject-reason contract;
- solution/screenshot-runner builds and readiness contract.

## Boundary

- Metrics remain axis-aligned pixel `Area`, `BoundsWidth`, and `BoundsHeight`.
- This does not add rotated dimensions, aspect ratio, circularity, angle gates,
  holes, grayscale descriptors, or semantic classification.
- It does not automatically recommend, edit, or apply a range.
- It does not change Blob/Contour detection, candidate capture, filter order,
  `ResultCount`, aggregate metrics, XML/property defaults, saved report schema,
  or acceptance.
- The synthetic/public evidence does not prove unseen-data robustness,
  production accuracy, or field qualification.
- `CVR-06` and later commercial-video candidates remain conditional.

## Durable Closure

```text
Status: Complete
Scope: Current-Run Blob/Contour Area/BoundsWidth/BoundsHeight accepted/rejected distributions with exact existing ranges, shared evidence/TSV provenance, and row/plot/drawing selection.
Acceptance criteria: Existing retained rows only -> pass; Area/Width/Height selection -> pass; accepted/rejected distribution -> pass; exact existing range markers -> pass; exact reject reason -> pass; row/plot/image identity -> pass; legacy unbounded compatibility -> pass; public Blob Good/Bad and Contour Bad product path -> pass; no review-triggered execution/layer/route change -> pass; no detector/XML/filter/acceptance change -> pass.
Verification: Debug solution and screenshot-runner builds; cvr05_object_metric_distribution; three public product-path UI smokes; object-dimension filter contract; related Pipeline Review regressions; readiness.
Evidence: docs/reports/OPENVISIONLAB_OBJECT_METRIC_DISTRIBUTION_20260728.md and artifacts/cvr05_object_metric_distribution_20260728.
Boundary / next dependency: Existing pixel object metrics and current-run review only; no new descriptor, automatic gate, semantic classification, unseen robustness, or field qualification claim. CVR-00 independent novice observations remain the active external prerequisite; CVR-06 requires its exact frozen matcher-diagnosis blocker or explicit user selection.
```
