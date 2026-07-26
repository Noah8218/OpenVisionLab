# Matching Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `56d1bd8`
Status: Complete

## User Goal

Continue structural MVVM-style refactoring through a cohesive responsibility
owner rather than another partial-file split.

## Refactor Proof Plan

### Current Structure

- Current responsibility owner:
  `VisionPipelineStepPropertyMapper.Matching.cs`, as a partial of the root
  mapper.
- Current call path:
  root ToolType switch -> partial property creation -> generic
  `VisionPipelineStepBuilder.FromProperty` -> root matching-parameter
  post-processing.
- Current dependency direction:
  the root mapper directly knows the Matching property model and its fixture
  publish behavior.
- Current state/data owner:
  the partial owns editable Matching and fixture values, while the root owns
  reconstruction orchestration and fixture parameter application.

### Intended New Structure

- New responsibility owner:
  standalone non-partial `VisionPipelineMatchingPropertyAdapter`.
- New call path:
  root adapter dispatch -> adapter property creation/reconstruction/metric
  identification -> root generic Step metadata/final copy.
- New dependency direction:
  the root depends on the Matching adapter contract; the adapter reuses the
  existing root generic parameter readers and shared metadata interface.
- New state/data owner:
  the adapter owns Matching PropertyGrid state and fixture-publish
  reconstruction. Pipeline execution, saved recipes, layers, and UI selection
  remain unchanged.

### Structural Conditions

1. The old Matching partial, root ToolType cases, property-model reference,
   metric case, and post-processing call are absent.
2. The standalone adapter owns Matching/TemplateMatching recognition, editable
   model, fixture parameters, Step reconstruction, and metric identification.
3. The root calls the adapter for create, apply, and metric paths without a new
   interface, factory, or duplicated generic parameter codec.
4. Existing canonical Matching fixture round-trip, TemplateMatchingTool alias
   round-trip, metadata, layers, and zero automatic Preview/Run behavior pass.

### Proof Checks

- Search checks:
  old partial/root owner absence and new adapter owner presence.
- Dependency checks:
  root dispatches to the adapter; adapter uses only existing shared mapper
  helpers and product property/builder contracts.
- Call path checks:
  create, apply, and metric resolution all reach the adapter.
- Focused check:
  current-source `wpf_shell_host_recipe_fixture_properties`.
- Final checks:
  Debug solution build, readiness, and `git diff --check`.

## Structural Changes Confirmed

- Before:
  Matching and TemplateMatching recognition, the editable Matching/fixture
  model, and fixture parameter reconstruction lived inside a partial of the
  root mapper.
- After:
  `VisionPipelineMatchingPropertyAdapter` owns recognition, projection,
  editable state, reconstruction, fixture parameter application, and metric
  identification.
- Evidence:
  the old partial is deleted; the root has only create/apply/metric adapter
  dispatch; the new standalone class contains the property model and fixture
  reconstruction.

## Call And Data Flow

```text
Recipe Manager selected Matching Step
  -> VisionPipelineStepPropertyMapper
  -> VisionPipelineMatchingPropertyAdapter
  -> Matching/Fixture editable property state
  -> VisionPipelineStepBuilder.FromProperty
  -> adapter fixture parameter application
  -> root shared metadata/final copy
```

The adapter reuses the root mapper's existing parameter readers,
`ApplyCommonOpenCvProperty`, metadata interface, and layer/metric converters.
No additional interface, factory, base class, or generic codec was added.

## Preserved Behavior

- `Matching`, `MatchingTool`, `TemplateMatching`, and
  `TemplateMatchingTool` recognition remains supported.
- TemplateMatching aliases reconstruct the canonical `Matching` ToolType.
- Existing Matching defaults, common OpenCV parameters, fixture reference
  pose/image size, optional scale limits, layer routing, and acceptance
  metadata remain unchanged.
- Property loading/apply does not trigger Preview or Run.

## Checks Run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_fixture_properties artifacts\refactor_matching_adapter_20260726`
  - pass
  - verifies canonical Matching fixture/scale/layer round-trip, XML
    save/reload, TemplateMatchingTool alias canonicalization, and zero
    automatic Preview/Run
- `dotnet build tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- structural searches and `git diff --check`
  - pass

The first readiness run exposed an over-broad new forbidden-token check:
`PipelineMatchingProperty` also matched the valid adapter class name. The
source check was narrowed to the exact old private-model declaration, rebuilt,
and passed. No production behavior changed for that correction.

Current-source visual evidence:

- `artifacts/refactor_matching_adapter_20260726/wpf_shell_host_recipe_fixture_properties.png`
- `artifacts/refactor_matching_adapter_20260726/wpf_shell_host_recipe_fixture_properties.diagnostics/matching-fixture-property-grid.png`
- `artifacts/refactor_matching_adapter_20260726/wpf_shell_host_recipe_fixture_properties.diagnostics/normalize-image-fixture-property-grid.png`

The main image shows the final NormalizeImage consumer. The diagnostic Matching
image is the applicable visual evidence for the moved property owner.

## Boundary

This changes PropertyGrid/XML mapping ownership only. It does not change the
Matching runtime algorithm, fixture math, validation, result metrics, drawings,
saved recipe schema, visible layout, layers, routing, or explicit Preview/Run
behavior. It adds no locator qualification or field-robustness evidence.

## Completion Record

Status: Complete
Scope: Move Matching/TemplateMatching PropertyGrid projection, fixture state,
Step reconstruction, fixture parameter application, and metric identification
into one standalone non-partial adapter.
Acceptance criteria: Old partial/root direct owner absent; new create/apply/
metric paths active; canonical and alias round-trips preserve fixture, scale,
layer, acceptance, and explicit-run contracts; build/readiness/searches pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and the current-source artifacts above.
Boundary / next dependency: Do not extract ObjectInspection, BasicImage,
EdgeBasedMatching, FeatureMatching, single LineGauge, or Mean until a focused
selected-Step create/apply round-trip gate exists for the chosen family.
