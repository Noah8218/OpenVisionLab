# PinArrayGap Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `b0ef64a`

## User Goal

Continue structural MVVM-oriented cleanup without treating another partial-file
split as architecture progress.

## Current To Intended Structure

Before:

- `VisionPipelineStepPropertyMapper` directly recognized `PinArrayGap` and
  `AdjacentPinGap`.
- Its private nested `PipelinePinArrayGapProperty` owned parameter/default
  projection, PropertyGrid fields, unrepresented-parameter preservation, and
  Step reconstruction.
- Create, apply, and metric-owner paths depended directly on that nested type.

After:

- `VisionPipelinePinArrayGapPropertyAdapter` owns alias recognition,
  parameter/default projection, the PropertyGrid model, baseline parameter
  preservation, canonical Step reconstruction, and property identification.
- The root mapper dispatches create/apply/metric resolution to the adapter and
  retains only shared Step metadata and final copy behavior.
- The new owner is a standalone non-partial class.

Call path:

```text
Recipe Manager selected Step
  -> VisionPipelineStepPropertyMapper
  -> VisionPipelinePinArrayGapPropertyAdapter
  -> PinArrayGap PropertyGrid model / VisionPipelineStep
  -> root shared metadata and copy
```

Dependency direction:

- Root mapper depends on the bounded adapter.
- The adapter depends only on Pipeline/property data types and the root's
  existing shared metadata/converter contracts.
- It has no Shell, View, storage, execution-service, or runtime-tool
  dependency.

State/data owner:

- The adapter owns the temporary editable PinArrayGap property state and the
  baseline parameter dictionary required for lossless round trip.
- Pipeline execution and saved-recipe ownership are unchanged.

## Structural Conditions And Evidence

- Root direct aliases and nested model are absent.
  - Search confirms no `PipelinePinArrayGapProperty`, direct
    `case "pinarraygap"`, or direct `case "adjacentpingap"` remains in the
    root mapper.
- New create/apply/metric call paths are used.
  - Root search confirms `TryCreateProperty`, `TryCreateStep`, and `IsProperty`
    adapter calls.
- The readiness contract checks the new owner, the root dispatch, alias
  preservation, baseline preservation, and old-owner absence.

## Preserved Behavior

- `PinArrayGap`, `PinArrayGapTool`, `AdjacentPinGap`, and
  `AdjacentPinGapTool` normalization remains supported.
- The original ToolType spelling is retained on round trip.
- Existing defaults remain:
  - `MeasurementMode=EdgeGap`
  - `USE_ROI=false`
  - `DarkThreshold=128`
  - `MinDarkCoverageRatio=0.55`
  - `MinPinWidth=5`
  - `MaxPinBreakWidth=2`
  - `MinGapWidth=3`
- Existing parameters not represented in the PropertyGrid, including
  `ALLOW_BRANCH_INPUT`, remain in the reconstructed Step.
- Step name, enabled state, input/output layers, and acceptance metadata still
  use the root mapper's shared copy path.
- Loading or applying properties does not trigger Preview or Run.

## Checks Run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_pinarraygap_properties artifacts\refactor_pinarraygap_adapter_20260726_r2`
  - pass
  - verifies direct and saved round trip, unrepresented parameter retention,
    PropertyGrid fields/search, zero Preview/Run, alias/default/baseline
    preservation, and current-source UI rendering
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- `git diff --check` and ownership/call-path searches
  - pass

Visual evidence:

- `artifacts/refactor_pinarraygap_adapter_20260726_r2/wpf_shell_host_recipe_pinarraygap_properties.png`

## Boundary

This is a mapping responsibility extraction only. It does not change the
PinArrayGap runtime algorithm, EdgeGap/CenterPitch semantics, metrics,
validation, recipe schema, LLM maintenance status, visible layout, layers,
routing, or explicit Preview/Run contract. It does not add bright-pin,
calibrated-unit, unseen-data, or field-robustness evidence.

The existing Line Pair partial was inspected but not changed because it also
contains the GeometryMeasure/CircleGauge base class. Extracting it in this
slice would cross the selected one-family boundary.

## Completion Record

Status: Complete
Scope: Move the existing PinArrayGap/AdjacentPinGap PropertyGrid/XML mapping
family from the root mapper into one standalone non-partial adapter.
Acceptance criteria: Root direct owner absent; adapter create/apply/metric paths
active; aliases/defaults/baseline/current values preserved; focused current UI
smoke, full build, readiness, and structural searches pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_PIN_ARRAY_GAP_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and
`artifacts/refactor_pinarraygap_adapter_20260726_r2/wpf_shell_host_recipe_pinarraygap_properties.png`.
Boundary / next dependency: Design the smallest clean Line Pair boundary
without moving its currently shared geometry base accidentally; require both
Line Pair and geometry regression coverage before implementation.
