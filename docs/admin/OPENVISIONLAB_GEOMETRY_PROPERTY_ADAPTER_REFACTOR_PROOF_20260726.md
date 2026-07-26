# Geometry Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `6f78198`

## User Goal

Continue structural refactoring through cohesive responsibility owners rather
than another partial-file split.

## Current To Intended Structure

Before:

- `VisionPipelineStepPropertyMapper` directly recognized GeometryMeasure,
  GeometricMeasurement, and CircleGauge ToolTypes.
- The root mapper owned:
  - `PipelineGeometryPropertyBase`
  - `PipelineGeometryMeasureProperty`
  - `PipelineCircleGaugeProperty`
  - `PipelineGeometryFeatureConverter`
  - geometry feature-reference join/split helpers
  - geometry ROI formatting
  - direct apply and metric-type branches

After:

- `VisionPipelineGeometryPropertyAdapter` is the standalone non-partial owner
  for:
  - GeometryMeasure/GeometricMeasurement/CircleGauge recognition
  - shared geometry baseline and acceptance state
  - GeometryMeasure and CircleGauge PropertyGrid models
  - typed earlier-feature selection
  - feature-reference parsing
  - ROI formatting
  - Step reconstruction
  - metric-owner identification
- The root mapper retains only adapter dispatch, shared generic parameter
  codecs, metadata copying, and final Step copying.

## Call And Data Flow

```text
Recipe Manager selected Step
  -> VisionPipelineStepPropertyMapper
  -> VisionPipelineGeometryPropertyAdapter
  -> GeometryMeasure/CircleGauge editable property state
  -> typed feature converter reads VisionPipelinePropertyContext
  -> VisionPipelineStep
  -> root shared metadata/final copy
```

The adapter owns the baseline parameter dictionary and editable geometry state.
Pipeline execution, runtime result features, saved recipes, and UI selection
state remain in their existing owners.

The adapter reuses the root mapper's existing generic parameter readers,
`AddParameter`, metadata interface, and layer/metric converters. No additional
interface, factory, or duplicate codec was added.

## Structural Evidence

- Root search shows no direct GeometryMeasure/GeometricMeasurement/CircleGauge
  switch case.
- Root search shows no old geometry base/model/converter/reference-format
  helper.
- Root search confirms adapter create/apply/metric call paths.
- Adapter search confirms one standalone owner containing the base, both
  models, typed feature converter, reference helpers, and ROI formatter.
- Readiness checks enforce the new owner, old-owner absence, and both P213
  regression targets.

## Preserved Behavior

- GeometryMeasure, GeometryMeasureTool, GeometricMeasurement,
  GeometricMeasurementTool, CircleGauge, and CircleGaugeTool recognition
  remains supported.
- Geometry aliases continue to reconstruct the canonical
  `GeometryMeasure`/`CircleGauge` ToolType.
- Existing source identities, all geometry gate defaults, CircleGauge annular
  and edge-fit defaults, baseline parameters, acceptance metadata, and
  `ALLOW_BRANCH_INPUT=true` behavior remain unchanged.
- Typed feature dropdowns still expose only compatible earlier enabled
  features through `VisionPipelinePropertyContext`.
- Property loading/apply does not trigger Preview or Run.

## Checks Run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p213_geometry_property_grid artifacts\refactor_geometry_adapter_20260726`
  - pass
  - verifies typed source selection, PropertyGrid fields/search, save/reload,
    branch-input preservation, and zero Preview/Run
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p213_geometry_review artifacts\refactor_geometry_adapter_20260726`
  - pass
  - executes CircleGauge gates, all seven GeometryMeasure modes, direct
    property round trips, GeometricMeasurementTool/CircleGaugeTool aliases,
    report persistence, and Geometry Review selection
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- structural searches and `git diff --check`
  - pass

Visual evidence:

- `artifacts/refactor_geometry_adapter_20260726/p213_geometry_property_grid.png`
- `artifacts/refactor_geometry_adapter_20260726/p213_geometry_review.png`

## Boundary

This changes PropertyGrid/XML mapping ownership only. It does not change
GeometryMeasure or CircleGauge runtime algorithms, math, validation, metrics,
typed-result contracts, drawings, report schema, visible layout, layers,
routing, or explicit Preview/Run behavior. It adds no calibration, metrology,
unseen-data, or field-robustness evidence.

## Completion Record

Status: Complete
Scope: Move GeometryMeasure/CircleGauge PropertyGrid/XML mapping, shared
geometry state, typed feature selection, and reconstruction into one standalone
non-partial adapter.
Acceptance criteria: Root direct owner absent; new create/apply/metric paths
active; Geometry/Circle aliases and current values preserved; P213 PropertyGrid
and full geometry core/review pass; build/readiness/searches pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_GEOMETRY_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and the two current-source artifacts above.
Boundary / next dependency: Re-audit remaining direct and partial mapper
families; do not extract one without a dedicated current round-trip regression.
