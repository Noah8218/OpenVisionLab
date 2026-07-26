# Line Pair Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `4f7c0a0`

## User Goal

Continue the mapper refactor through real responsibility boundaries instead of
adding another partial file.

## Current To Intended Structure

Before:

- `VisionPipelineStepPropertyMapper.LinePair.cs` was another part of the root
  partial class.
- It owned LineDistance/LineIntersection projection, the Line Pair PropertyGrid
  model, Step reconstruction, and the public Tool View edit handoff.
- The same Line Pair-named partial also owned
  `PipelineGeometryPropertyBase`, which is used only by the root
  GeometryMeasure and CircleGauge models.

After:

- `VisionPipelineLinePairPropertyAdapter` is a standalone non-partial owner for:
  - LineDistance/LineIntersection ToolType recognition
  - prefixed Line A/B parameter projection
  - the editable Line Pair PropertyGrid model
  - Line Pair Step reconstruction
  - LineGauge pair creation for Tool View edit handoff
  - metric-owner identification
- `VisionPipelineStepPropertyMapper` dispatches create/apply/metric work to the
  adapter and keeps its existing public `TryCreateLineGaugePair` API as a thin
  compatibility forwarder.
- `PipelineGeometryPropertyBase` is colocated in the root mapper beside its
  only derived models, `PipelineGeometryMeasureProperty` and
  `PipelineCircleGaugeProperty`.
- `VisionPipelineStepPropertyMapper.LinePair.cs` no longer exists.

## Call Path

```text
Recipe Manager or Tool View
  -> VisionPipelineStepPropertyMapper compatibility/dispatch surface
  -> VisionPipelineLinePairPropertyAdapter
  -> Line Pair PropertyGrid model / left-right LineGauge properties
  -> VisionPipelineStep
  -> root shared metadata and final copy
```

The adapter uses the root mapper's existing generic parameter-reading,
OpenCV-property projection, metadata-interface, and converter contracts. Those
helpers remain shared because several existing mapper families use them; no new
interface, factory, or duplicate codec was introduced.

## Structural Conditions And Evidence

- The old partial file is absent.
- Root direct `linedistance`/`lineintersection` cases and private Line Pair
  model are absent.
- Root search confirms adapter `TryCreateProperty`, `TryCreateStep`,
  `TryCreateLineGaugePair`, and `IsProperty` call paths.
- Adapter search confirms the standalone class and private Line Pair model.
- The adapter does not contain `PipelineGeometryPropertyBase`.
- Root search confirms the Geometry base immediately precedes its derived
  GeometryMeasure/CircleGauge models.
- Readiness checks enforce these ownership and regression-target conditions.

## Preserved Behavior

- `LineDistance`, `LineDistanceTool`, `LineIntersection`, and
  `LineIntersectionTool` normalization remains supported.
- Original ToolType spelling is retained on round trip.
- Independent Line A/B ROI, polarity, projection direction, vertical
  direction, and manual-angle values remain lossless.
- Existing baseline-specific per-line values remain preserved when a shared
  compact field is not edited.
- GapEdgePair parameters and Tool View left/right LineGauge handoff remain
  unchanged.
- Step metadata, acceptance metadata, explicit Preview/Run, layers, and routing
  remain under the existing root/shared workflows.

## Checks Run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_line_pair_properties artifacts\refactor_line_pair_adapter_20260726_r2`
  - pass
  - verifies asymmetric A/B direct and saved round trip, independent edits,
    `LineIntersectionTool` alias/default directions, PropertyGrid search, and
    zero Preview/Run
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p213_geometry_property_grid artifacts\refactor_line_pair_adapter_20260726`
  - pass
  - verifies GeometryMeasure PropertyGrid source selection, save/reload, and
    zero Preview/Run after base relocation
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p213_geometry_review artifacts\refactor_line_pair_adapter_20260726`
  - pass
  - executes the P213 geometry core including CircleGauge and all seven
    GeometryMeasure modes before rendering the review
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- structural searches and `git diff --check`
  - pass

Visual evidence:

- `artifacts/refactor_line_pair_adapter_20260726_r2/wpf_shell_host_recipe_line_pair_properties.png`
- `artifacts/refactor_line_pair_adapter_20260726/p213_geometry_property_grid.png`
- `artifacts/refactor_line_pair_adapter_20260726/p213_geometry_review.png`

## Boundary

This is a mapping ownership change only. It does not change the LineGauge,
LineDistance, LineIntersection, GapEdgePair, CircleGauge, or GeometryMeasure
runtime algorithms, validation, metrics, recipe schema, drawings, visible
layout, layers, routing, or explicit Preview/Run contract. It does not add
metrology, calibration, unseen-data, or field-robustness evidence.

The shared mapper parameter codecs were not extracted because they already
serve multiple families and doing so is unnecessary for the selected Line Pair
ownership change.

## Completion Record

Status: Complete
Scope: Replace the Line Pair root partial with one standalone adapter and
relocate its misplaced Geometry base to the owning mapper region.
Acceptance criteria: Old partial/direct owner absent; adapter call paths active;
Tool View compatibility preserved; Line Pair alias/asymmetric round trip passes;
GeometryMeasure/CircleGauge regressions pass; build/readiness/searches pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_LINE_PAIR_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and the three current-source artifacts listed above.
Boundary / next dependency: Audit GeometryMeasure/CircleGauge as one cohesive
adapter candidate only if both existing P213 regressions remain the required
completion gate.
