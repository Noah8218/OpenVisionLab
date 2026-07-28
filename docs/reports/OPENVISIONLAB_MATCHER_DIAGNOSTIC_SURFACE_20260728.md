# OpenVisionLab Matcher Diagnostic Surface

Date: 2026-07-28 KST
Backlog item: `CVR-06`
Status: Complete

## Outcome

Pipeline Review now exposes a read-only `Matcher Diagnostics` tab for the
existing `EdgeBasedMatching`, `EdgeBasedTemplateMatching`, and
`EdgeTemplateMatching` aliases. The tab explains one retained explicit Run; it
does not execute the matcher again.

The diagnostic retains and displays:

- the exact trained edge-model points and model center;
- template size and search ROI;
- model-pyramid level estimates and the actual fixed coarse proposal scale;
- actual coarse-proposal attempts, candidates, verification, acceptance, and
  fallback counters;
- the retained primary hypothesis and strongest spatially distinct
  alternative, when each exists;
- exact candidate score, center, bounds, angle, and scale;
- uniqueness state, selected/alternative scores, margin, required margin, and
  plausible-alternative count;
- the exact runtime `Success`, `MatchingNoResult`, or `MatchingAmbiguous`
  reason;
- a stable SHA-256 evidence ID derived from the source image, model,
  candidates, state/reason, and retained matcher metrics.

`NoMatch` does not label a below-gate observation as an accepted match. It uses
`Best observed (below gate)`. `Ambiguous` uses
`Rejected primary hypothesis`. A missing spatial alternative is shown as
`None retained`; it is not fabricated.

## Ownership

`C:\Git\Library-Noah` owns runtime diagnostic capture:

- `EdgeBasedMatchingDiagnosticEvidence` and candidate snapshots;
- exact trained model points;
- primary/alternative candidate geometry;
- runtime decision state and reason;
- model/pyramid/candidate/uniqueness metrics.

OpenVisionLab owns presentation and retained-run integration:

- `VisionPipelineResultSummary` clones the Library-Noah evidence;
- `OpenVisionPipelineReviewMatcherDiagnosticPresenter` creates the two
  drawings, read-only table, and stable evidence ID;
- Pipeline Review selects the diagnostic tab only for the supported matcher
  aliases and never changes layer routing or executes Preview/Run.

## Frozen Verification Matrix

The deterministic matrix uses one public L-shaped edge template and identical
settings:

| Case | Runtime state | Primary hypothesis | Spatial alternative | Expected diagnostic |
| --- | --- | --- | --- | --- |
| one target | `Success` | retained | none | accepted primary and exact success reason |
| blank field | `NoMatch` | best observed below gate | retained only when the matcher returns one | exact `MatchingNoResult` reason without inventing an alternative |
| two identical targets | `Ambiguous` | rejected primary | retained | equal-score spatially distinct hypotheses and exact `MatchingAmbiguous` reason |

The public product path also passed:

- `Public_Edge_Fiducial_Good`: `Success`, 260 trained points, accepted score
  `0.996`, 40 diagnostic rows;
- `Public_Edge_Fiducial_Wrong_Bad`: `NoMatch`, 260 trained points, below-gate
  score `0.611`, exact `MatchingNoResult` reason, 40 diagnostic rows.

## Verification

Commands actually run:

```powershell
cd C:\Git\Library-Noah
dotnet build Lib.OpenCV\Lib.OpenCV.csproj -c Release
dotnet run --project Lib.Inspection.Smoke\Lib.Inspection.Smoke.csproj -c Release

cd C:\Git\OpenVisionLab_Dev
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target cvr06_matcher_diagnostic artifacts\cvr06_matcher_diagnostic_20260728\final
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_edge_ng_metrics artifacts\cvr06_matcher_diagnostic_20260728\edge_ng
dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_edge_based_matching_tool,cvr05_object_metric_distribution,cvr04_circle_residual_review,p213_geometry_review,p214_two_point_scale artifacts\cvr06_matcher_diagnostic_20260728\regression
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug
```

Final results:

- Library-Noah Release build: zero warnings, zero errors;
- Library-Noah inspection smoke: `66/66` passed;
- OpenVisionLab Debug solution build: zero warnings, zero errors;
- CVR-06 Success/NoMatch/Ambiguous matrix: passed;
- public matcher NoMatch Pipeline Review: passed;
- legacy Edge Tool and CVR-04/CVR-05/P213/P214 UI regressions: passed;
- full readiness contract: passed.

Library identity after the final build/copy:

- assembly version: `2.1.0.0`;
- file version: `2.8.0.0`;
- SHA-256:
  `13CF973DCD485B245AD32D1DCEE7B45F84FFA98D940C6B3C70710851C02FB2BB`.

The Library-Noah Release output, OpenVisionLab vendored DLL, and OpenVisionLab
Debug output had the same hash.

## Evidence

- current-source before capture:
  `artifacts\cvr06_matcher_diagnostic_20260728\before`;
- final Success UI and three-state matrix:
  `artifacts\cvr06_matcher_diagnostic_20260728\final`;
- public product-path NoMatch UI:
  `artifacts\cvr06_matcher_diagnostic_20260728\edge_ng`;
- related UI regressions:
  `artifacts\cvr06_matcher_diagnostic_20260728\regression`.

## Boundary

This is diagnostic evidence only.

- No matcher score, gate, default, candidate ordering, acceptance, XML,
  PropertyGrid, report, template-selection, or layer-routing behavior changed.
- The model pyramid rows describe diagnostic model usability; the coarse scale
  and proposal counters describe the actual existing runtime path.
- The surface does not identify a durable physical feature, qualify a
  template, auto-select a pattern, tune a gate, add polarity/deformation/
  anisotropic-scale/overlap behavior, or claim commercial parity.
- The synthetic/public matrix is integration evidence, not unseen-data,
  production, or field qualification.

## Durable Completion Record

```text
Status: Complete
Scope: Read-only retained-run EdgeBasedMatching model/pyramid/coarse-path/candidate/alternative/decision diagnostics in Pipeline Review.
Acceptance criteria: Exact model and candidate geometry -> pass; Success/NoMatch/Ambiguous reasons -> pass; public Good/Bad product paths -> pass; zero rerun/layer/routing side effects -> pass; existing matcher and adjacent review regressions -> pass.
Verification: Library-Noah Release build and 66/66 smoke; OpenVisionLab Debug build; CVR-06 matrix; public NoMatch UI; Edge/CVR-04/CVR-05/P213/P214 UI regressions; readiness.
Evidence: docs/reports/OPENVISIONLAB_MATCHER_DIAGNOSTIC_SURFACE_20260728.md and artifacts/cvr06_matcher_diagnostic_20260728.
Boundary / next dependency: CVR-00 still requires three real novice participants. CVR-07 remains conditional and needs a repeated, named Threshold/Line/Circle teaching blocker or another explicit user selection; it is not auto-activated.
```
