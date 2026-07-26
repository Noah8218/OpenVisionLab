# Object Inspection Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `1534922`
Status: Complete

## User Goal

Continue structural MVVM-style refactoring through a cohesive responsibility
owner rather than another partial-file split.

## Refactor Proof Plan

### Current Structure

- Current responsibility owner:
  `VisionPipelineStepPropertyMapper.ObjectInspection.cs`, as a partial of the
  root mapper.
- Current call path:
  root ToolType switch -> partial Blob/Contour property creation -> generic
  `VisionPipelineStepBuilder.FromProperty` -> root Blob fixture-parameter
  post-processing.
- Current dependency direction:
  the root mapper directly knows both object-inspection property models and
  their metric types.
- Current state/data owner:
  the partial owns editable Blob/Contour state, while the root owns
  reconstruction orchestration and Blob fixture parameter application.

### Intended New Structure

- New responsibility owner:
  standalone non-partial `VisionPipelineObjectInspectionPropertyAdapter`.
- New call path:
  root adapter dispatch -> adapter Blob/Contour property creation,
  reconstruction, fixture application, and metric identification -> root
  generic metadata/final Step copy.
- New dependency direction:
  the root depends on the ObjectInspection adapter contract; the adapter
  reuses the existing root parameter readers and metadata interface.
- New state/data owner:
  the adapter owns Blob/Contour PropertyGrid state and reconstruction. Runtime
  object detection, result rows, saved recipes, layers, and UI selection stay
  in their existing owners.

### Structural Conditions

1. A focused baseline gate proves current Blob/Contour create/apply behavior
   before production ownership moves.
2. The old ObjectInspection partial, root ToolType/model/metric references,
   and fixture post-processing are absent after extraction.
3. The standalone adapter owns Blob/Contour recognition, models, Step
   reconstruction, Blob fixture parameters, and metric identification.
4. Existing parameters, aliases, metadata, layers, and explicit Preview/Run
   behavior remain unchanged.

### Proof Checks

- Baseline check:
  add BlobTool/ContourTool selected-Step create/apply assertions to the
  current P216 smoke and run them before extraction.
- Search checks:
  old partial/root owner absence and new adapter owner presence.
- Call path checks:
  root create/apply/metric dispatch reaches the adapter.
- Focused checks:
  current-source `p216_object_dimension_filters_property_grid`,
  `wpf_shell_host_recipe_fixture_properties`, and Blob/Contour alias
  round-trips.
- Final checks:
  Debug solution build, readiness, and `git diff --check`.

## Structural Changes Confirmed

- Before:
  Blob/Contour recognition and editable models lived in a root-mapper partial;
  reconstruction used the root generic path and a separate root Blob-fixture
  post-processing call.
- After:
  `VisionPipelineObjectInspectionPropertyAdapter` owns Blob/Contour
  recognition, property projection, editable models, Step reconstruction,
  Blob fixture application, and metric identification.
- Evidence:
  the old partial is deleted; root direct ToolType/model/metric/post-processing
  ownership is absent; root create/apply/metric calls reach the standalone
  adapter.

## Call And Data Flow

```text
Recipe Manager selected Blob/Contour Step
  -> VisionPipelineStepPropertyMapper
  -> VisionPipelineObjectInspectionPropertyAdapter
  -> Blob/Contour editable property state
  -> VisionPipelineStepBuilder.FromProperty
  -> Blob fixture parameter application when applicable
  -> root shared metadata/final Step copy
```

The adapter reuses the existing root parameter readers,
`ApplyCommonOpenCvProperty`, metadata interface, and layer/metric converters.
No new interface, factory, base class, or generic parameter codec was added.

## Preserved Behavior

- `Blob`, `BlobTool`, `Contour`, and `ContourTool` recognition remains
  supported; aliases reconstruct canonical `Blob`/`Contour` ToolTypes.
- Blob area/dimension/common OpenCV/ROI/threshold values, fixture consumption,
  frame name, and branch-input opt-in remain unchanged.
- Contour approximation, retrieval, drawing mode/color/thickness,
  area/dimension/common OpenCV/ROI values remain unchanged.
- Step enabled state, acceptance metadata, and input/output layers remain
  unchanged.
- Property loading and edits do not trigger Preview or Run.

## Checks Run

- Baseline before production extraction:
  `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p216_object_dimension_filters_property_grid artifacts\refactor_object_inspection_baseline_20260726`
  - pass after correcting test-only enum qualification
  - proves the old partial satisfied the new Blob/Contour selected-Step
    create/apply assertions
- After extraction:
  `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target p216_object_dimension_filters_property_grid artifacts\refactor_object_inspection_adapter_20260726_r2`
  - pass
  - proves BlobTool/ContourTool aliases, canonical reconstruction, object
    parameters, ROI/threshold, Blob fixture/branch parameters, layers, and
    acceptance metadata
- Related fixture regression:
  `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_fixture_properties artifacts\refactor_object_inspection_adapter_fixture_20260726`
  - pass
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- structural searches and `git diff --check`
  - pass

The first baseline command failed to compile because the new smoke assertions
used two unqualified OpenCvSharp enum names; fully qualifying those test-only
types fixed it before production code moved.

The first post-extraction P216 run found that its fixed smoke recipe name
retained `MIN_WIDTH=15` from the baseline run, violating the target's own
default-value assumption. The target now cleans transient smoke workspaces and
uses a unique recipe name. The same extracted production code then passed.
No product default or persisted user workspace rule was changed.

Current-source visual evidence:

- `artifacts/refactor_object_inspection_adapter_20260726_r2/p216_object_dimension_filters_property_grid.png`
- `artifacts/refactor_object_inspection_adapter_fixture_20260726/wpf_shell_host_recipe_fixture_properties.png`

The P216 image was visually checked for visible field text, controls, values,
buttons, and overlap. It is current-source Native Tool evidence; the direct
assertions executed by the same target are the selected-Step mapping evidence.

## Boundary

This changes PropertyGrid/XML mapping ownership and smoke isolation only. It
does not change Blob/Contour algorithms, filtering, metrics, object rows,
drawings, validation, saved recipe schema, visible layout, layers, routing, or
explicit Preview/Run behavior. It adds no semantic classification or
field-robustness evidence.

## Completion Record

Status: Complete
Scope: Move Blob/Contour PropertyGrid projection, editable models, Step
reconstruction, Blob fixture application, and metric identification into one
standalone non-partial adapter, backed by a pre-move baseline gate.
Acceptance criteria: Baseline gate passes before extraction; old partial/root
owner absent; new create/apply/metric paths active; Blob/Contour aliases,
parameters, metadata, layers, and explicit-run contracts pass; build,
readiness, and searches pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_OBJECT_INSPECTION_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and the current-source artifacts above.
Boundary / next dependency: Audit BasicImage as one cohesive adapter only
after a focused Threshold/Morphology/Filter/EdgeDetection selected-Step
create/apply baseline gate is defined and passes.
