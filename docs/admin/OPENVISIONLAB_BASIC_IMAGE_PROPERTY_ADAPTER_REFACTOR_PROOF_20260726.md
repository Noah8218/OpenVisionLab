# Basic Image Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `e98e8bb`
Status: Complete

## User Goal

Continue structural MVVM-style refactoring through a cohesive responsibility
owner rather than another partial-file split.

## Refactor Proof Plan

### Current Structure

- Current responsibility owner:
  `VisionPipelineStepPropertyMapper.BasicImage.cs` owns create/apply helpers,
  while the root mapper owns the four editable PropertyGrid models and metric
  identification.
- Current call path:
  root ToolType switch -> BasicImage partial create helper -> root model;
  root apply orchestration -> partial builder selection -> root final copy.
- Current dependency direction:
  the root mapper directly knows Threshold, Morphology, Filter, and
  EdgeDetection model types and their metric types.
- Current state/data owner:
  editable parameter/metadata state is split between root models and partial
  create/apply functions.

### Intended New Structure

- New responsibility owner:
  standalone non-partial `VisionPipelineBasicImagePropertyAdapter`.
- New call path:
  root adapter dispatch -> adapter property creation/reconstruction/metric
  identification -> root shared metadata/final Step copy.
- New dependency direction:
  the root depends on one BasicImage adapter contract; the adapter reuses the
  existing parameter readers, builders, and metadata interface.
- New state/data owner:
  the adapter owns the four editable models and mapping state. Runtime
  preprocessing, saved recipes, layers, and UI selection stay unchanged.

### Structural Conditions

1. A focused baseline proves all four current create/apply paths before
   production ownership moves.
2. The BasicImage partial and root Threshold/Morphology/Filter/EdgeDetection
   cases, models, and metric cases are absent after extraction.
3. The standalone adapter owns recognition, models, reconstruction, and metric
   identification for all four families.
4. Existing aliases, parameters, metadata, layers, and explicit Preview/Run
   behavior remain unchanged.

### Proof Checks

- Baseline:
  add direct selected-Step round-trips to the existing Filter/Morphology layout
  smoke and pass them before extraction.
- Search:
  old partial/root owners absent; adapter and root dispatch present.
- Focused:
  current-source Filter/Morphology layout, Threshold Tool, and Edge/Line Learn
  smokes.
- Final:
  Debug solution build, readiness, and `git diff --check`.

## Structural Changes Confirmed

- Before:
  the BasicImage partial owned create/apply helpers while the root mapper owned
  four PropertyGrid models and metric cases.
- After:
  `VisionPipelineBasicImagePropertyAdapter` owns recognition, projection,
  editable models, reconstruction, and metric identification for Threshold,
  Morphology, Filter, and EdgeDetection.
- Evidence:
  the old partial is gone; the four models and direct ToolType/metric cases are
  absent from the root; root create/apply/metric calls reach the standalone
  adapter.

## Call And Data Flow

```text
Recipe Manager selected preprocessing Step
  -> VisionPipelineStepPropertyMapper
  -> VisionPipelineBasicImagePropertyAdapter
  -> Threshold/Morphology/Filter/EdgeDetection editable state
  -> existing VisionPipelineStepBuilder method
  -> root shared metadata/final Step copy
```

The adapter reuses existing root parameter readers, builder methods, metadata
interface, and layer/metric converters. No new interface, factory, base class,
or generic parameter codec was added.

## Preserved Behavior

- `ThresholdTool`, `MorphologyTool`, `FilterTool`, `EdgeDetectionTool`, and
  `EdgeTool` recognition remains supported; aliases reconstruct canonical
  ToolTypes.
- Every existing tool-specific parameter and default remains unchanged.
- Step name, enabled state, acceptance metadata, and input/output layers remain
  unchanged.
- Property loading and edits do not add automatic Preview or Run.

## Checks Run

- Baseline before production extraction:
  `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\refactor_basic_image_baseline_20260726`
  - pass
  - proves all four old create/apply paths, aliases, parameters, layers, and
    acceptance metadata
- After extraction:
  `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\refactor_basic_image_adapter_20260726`
  - pass
- Related current-source UI:
  `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool,wpf_openvision_learn_edge_line artifacts\refactor_basic_image_related_20260726`
  - both pass
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - final pass, 0 warnings, 0 errors
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- structural searches and `git diff --check`
  - pass

The first post-move build failed only because the adapter still called the
root's private `GetPropertyName` helper. The adapter now uses the same
metadata-interface rule locally; the next full build passed.

Two initial mechanical model-block moves stopped before writing because their
validated markers did not match the actual mixed line endings/category list.
The final move located the exact Threshold class and root/adapter closures,
verified all four model names in the contiguous block, preserved each file's
line endings, and then wrote the unchanged block. Root fell from 1,958 to
1,233 lines; the adapter owns the 898-line cohesive presentation/mapping
family.

Current-source visual evidence:

- `artifacts/refactor_basic_image_adapter_20260726/wpf_filter_morphology_layout_guard.png`
- `artifacts/refactor_basic_image_related_20260726/wpf_shell_host_threshold_tool.png`
- `artifacts/refactor_basic_image_related_20260726/wpf_openvision_learn_edge_line.png`

The current images were inspected for visible field text, values, buttons,
clipping, and overlap. This refactor makes no visible layout change.

## Boundary

This changes PropertyGrid/XML mapping ownership only. It does not change
preprocessing algorithms, parameter defaults, validators, saved recipe schema,
visible layout, layers, routing, or explicit Preview/Run behavior.

## Completion Record

Status: Complete
Scope: Move Threshold/Morphology/Filter/EdgeDetection recognition, editable
models, Step reconstruction, and metric identification into one standalone
non-partial adapter, backed by a pre-move four-tool baseline.
Acceptance criteria: Baseline passes before extraction; old partial/root owner
absent; new create/apply/metric paths active; all four aliases, parameters,
metadata, layers, and explicit-run contracts pass; build/readiness/searches
pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_BASIC_IMAGE_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and the current-source artifacts above.
Boundary / next dependency: Audit EdgeBasedMatching only after its existing
create checks are extended to a focused selected-Step apply round-trip baseline.
