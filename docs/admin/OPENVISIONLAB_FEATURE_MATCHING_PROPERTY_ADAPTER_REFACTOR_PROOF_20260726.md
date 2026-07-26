# Feature Matching Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `25e9f47`
Status: Complete

## User Goal

Continue structural MVVM-style refactoring through a cohesive responsibility
owner rather than another partial-file split.

## Refactor Proof Plan

### Current Structure

- Current responsibility owner:
  `VisionPipelineStepPropertyMapper.FeatureMatching.cs`, as a partial of the
  root mapper.
- Current call path:
  root ToolType switch -> partial property creation -> root generic
  `VisionPipelineStepBuilder.FromProperty` -> root final copy.
- Current dependency direction:
  the root mapper directly knows the FeatureMatching editable model and metric
  type.
- Current state/data owner:
  the partial owns projection/model state while the root owns reconstruction
  orchestration.

### Intended New Structure

- New responsibility owner:
  standalone non-partial `VisionPipelineFeatureMatchingPropertyAdapter`.
- New call path:
  root adapter dispatch -> adapter creation/reconstruction/metric
  identification -> root shared metadata/final Step copy.
- New dependency direction:
  the root depends on one FeatureMatching adapter contract; the adapter reuses
  existing parameter readers, builder, and metadata interface.
- New state/data owner:
  the adapter owns editable mapping state. Feature matcher runtime, template
  files, results, layers, Tool View state, Preview, and Run remain unchanged.

### Structural Conditions

1. Existing create checks are extended to canonical/alias create, apply,
   metadata, layer, and parameter round-trip and pass before production
   ownership moves.
2. The old partial and root direct ToolType/model/metric references are absent.
3. The standalone adapter owns recognition, model, reconstruction, and metric
   identification.
4. Existing defaults, Lowe ratio, RANSAC threshold, template path, common
   OpenCV parameters, metadata, layers, and explicit Preview/Run behavior
   remain unchanged.

### Proof Checks

- Baseline:
  current FeatureMatching Tool smoke with added selected-Step assertions.
- Search:
  old partial/root owner absent; adapter and root dispatch present.
- Focused:
  current-source FeatureMatching Tool smoke.
- Final:
  Debug solution build, readiness, and `git diff --check`.

## Result

### Ownership Change

- Removed the partial owner
  `VisionPipelineStepPropertyMapper.FeatureMatching.cs`.
- Added the standalone, non-partial
  `VisionPipelineFeatureMatchingPropertyAdapter`.
- The adapter now owns:
  - canonical `FeatureMatching` and legacy `Feature`/`Sift` recognition;
  - Pipeline parameter/default projection;
  - the editable FeatureMatching PropertyGrid model;
  - PropertyGrid model to Pipeline Step reconstruction;
  - metric-family identification.
- `VisionPipelineStepPropertyMapper` now keeps only adapter dispatch plus the
  existing shared metadata and final Step copy orchestration.
- No feature-matcher runtime, Lowe ratio meaning/default, RANSAC behavior,
  template dependency, result, layer-routing, Tool View, Preview, or Run
  behavior was changed.

### Before And After Flow

```text
Before
root ToolType switch
  -> root partial creates private FeatureMatching model
  -> root generic OpenCV reconstruction
  -> root metric switch

After
root adapter dispatch
  -> FeatureMatching adapter creates/reconstructs/identifies model
  -> root shared metadata and final Step copy
```

The former root owner consisted of the 1,248-line root mapper plus the
108-line FeatureMatching partial. The resulting root mapper is 1,263 lines and
the cohesive standalone adapter is 176 lines. The dispatch grew slightly, but
the root no longer owns FeatureMatching aliases, editable state, or metric
type knowledge.

## Acceptance Criteria And Evidence

- Baseline contract: PASS.
  - Command:
    `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_feature_matching_tool artifacts\refactor_feature_matching_adapter_baseline_20260726`
  - Result: `OK`, 1600x900, layout/text/internal errors all zero.
  - The added assertions prove `FeatureTool`/`SiftTool` recognition, canonical
    apply, XML/Step name, input/output layers, acceptance metadata, Lowe ratio,
    RANSAC threshold, template paths, threshold flags, and ROI round-trip.
- Structural ownership: PASS.
  - Root contains adapter create/apply/metric dispatch.
  - Root contains none of the three direct FeatureMatching ToolType cases and
    no private FeatureMatching PropertyGrid model.
  - The standalone adapter owns all three aliases and the private model.
- Post-move focused contract: PASS.
  - Command:
    `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_feature_matching_tool artifacts\refactor_feature_matching_adapter_20260726`
  - Result: `OK`, 1600x900, layout/text/internal errors all zero.
- Current-source visual check: PASS.
  - Artifact:
    `artifacts\refactor_feature_matching_adapter_20260726\wpf_shell_host_feature_matching_tool.png`
  - The current build retained the FeatureMatching Tool View, name/template
    fields, Ratio/RANSAC/ROI controls, result drawing, layer selectors, viewer,
    and docking controls without new clipped text/icons, hidden input content,
    or incoherent overlap.
- Full Debug solution build: PASS.
  - Command:
    `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --nologo`
  - Result: 0 warnings, 0 errors.
- Readiness: PASS.
  - Command:
    `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - Result: all readiness contracts passed.
- Patch hygiene: PASS.
  - Command: `git diff --check`
  - Result: no whitespace errors; Git reported only the existing Windows
    line-ending normalization notices.

## Refactor Proof Report

### Structural Changes Confirmed

- Before: root partial owned FeatureMatching projection/model details.
- After: standalone adapter owns projection/model/reconstruction/metric
  identification.
- Evidence: root absence search, adapter ownership search, pre/post smoke.

### Dependency And State Flow

- Dependency direction now:
  root mapper -> FeatureMatching adapter -> existing shared mapper
  readers/builder.
- State/data owner now:
  adapter-owned editable projection; unchanged Pipeline Step remains the saved
  definition and unchanged runtime/Tool View remain execution/UI owners.

### Remaining Structural Work

- Current owner/coupling:
  single LineGauge and Mean remain direct root mapper families.
- Intended owner/path:
  none until a focused maintenance need and selected-Step baseline prove a
  cohesive boundary.
- Required change:
  no speculative extraction.
- Next proof check:
  add a focused baseline only when one of those families requires actual
  maintenance.

## Durable Completion Record

Status: Complete
Scope: FeatureMatching selected-Step PropertyGrid mapping ownership moved from
a root partial to one standalone adapter, with existing behavior retained.
Acceptance criteria: baseline round-trip PASS; ownership search PASS;
post-move round-trip/UI PASS; build PASS; readiness PASS; patch hygiene PASS.
Verification: commands and results are recorded above.
Evidence:
`docs/admin/OPENVISIONLAB_FEATURE_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and `artifacts/refactor_feature_matching_adapter_20260726`.
Boundary / next dependency: this proves structural ownership and the named
PropertyGrid round-trip only. It does not qualify matching accuracy, template
identity, robustness, or authorize algorithm tuning. No further mapper family
should move without a concrete maintenance trigger and focused baseline.
