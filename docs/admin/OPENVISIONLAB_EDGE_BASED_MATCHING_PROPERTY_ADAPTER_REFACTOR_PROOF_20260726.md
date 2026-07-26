# Edge Based Matching Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `adafea7`
Status: Complete

## User Goal

Continue structural MVVM-style refactoring through a cohesive responsibility
owner rather than another partial-file split.

## Refactor Proof Plan

### Current Structure

- Current responsibility owner:
  `VisionPipelineStepPropertyMapper.EdgeBasedMatching.cs`, as a partial of the
  root mapper.
- Current call path:
  root ToolType switch -> partial property creation -> root generic
  `VisionPipelineStepBuilder.FromProperty` -> root final copy.
- Current dependency direction:
  the root mapper directly knows the EdgeBasedMatching editable model and
  metric type.
- Current state/data owner:
  the partial owns projection/model state while the root owns reconstruction
  orchestration.

### Intended New Structure

- New responsibility owner:
  standalone non-partial `VisionPipelineEdgeBasedMatchingPropertyAdapter`.
- New call path:
  root adapter dispatch -> adapter creation/reconstruction/metric
  identification -> root shared metadata/final Step copy.
- New dependency direction:
  the root depends on one EdgeBasedMatching adapter contract; the adapter
  reuses existing parameter readers, builder, and metadata interface.
- New state/data owner:
  the adapter owns editable mapping state. Matcher runtime, template files,
  results, layers, and Tool View state remain unchanged.

### Structural Conditions

1. Existing create checks are extended to apply, alias, metadata, and layer
   round-trip and pass before production ownership moves.
2. The old partial and root direct ToolType/model/metric references are absent.
3. The standalone adapter owns recognition, model, reconstruction, and metric
   identification.
4. Existing defaults, unique-match parameters, threshold compatibility,
   metadata, layers, and explicit Preview/Run behavior remain unchanged.

### Proof Checks

- Baseline:
  current EdgeBasedMatching Tool smoke with added apply/alias assertions.
- Search:
  old partial/root owner absent; adapter and root dispatch present.
- Focused:
  current-source EdgeBasedMatching Tool smoke.
- Final:
  Debug solution build, readiness, and `git diff --check`.

## Result

### Ownership Change

- Removed the partial owner
  `VisionPipelineStepPropertyMapper.EdgeBasedMatching.cs`.
- Added the standalone, non-partial
  `VisionPipelineEdgeBasedMatchingPropertyAdapter`.
- The adapter now owns:
  - canonical and legacy ToolType recognition;
  - Pipeline parameter/default projection;
  - the editable EdgeBasedMatching PropertyGrid model;
  - PropertyGrid model to Pipeline Step reconstruction;
  - metric-family identification.
- `VisionPipelineStepPropertyMapper` now keeps only adapter dispatch plus the
  existing shared metadata and final Step copy orchestration.
- No matcher runtime, template, score, Top-K, uniqueness gate, Auto MPoint,
  layer routing, Preview, or Run behavior was changed.

### Before And After Flow

```text
Before
root ToolType switch
  -> root partial creates private EdgeBasedMatching model
  -> root generic OpenCV reconstruction
  -> root metric switch

After
root adapter dispatch
  -> EdgeBasedMatching adapter creates/reconstructs/identifies model
  -> root shared metadata and final Step copy
```

The former root owner consisted of the 1,233-line root mapper plus the
132-line EdgeBasedMatching partial. The resulting root mapper is 1,248 lines
and the cohesive standalone adapter is 200 lines. The dispatch grew slightly,
but the root no longer owns EdgeBasedMatching aliases, editable state, or
metric type knowledge.

## Acceptance Criteria And Evidence

- Baseline contract: PASS.
  - Command:
    `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\refactor_edge_based_adapter_baseline_20260726`
  - Result: `OK`, 1600x900, layout/text/internal errors all zero.
  - The added assertion proves alias read, canonical apply, name/XML-name,
    input/output layers, acceptance metadata, pattern path, score, unique-match
    settings, threshold compatibility, and Canny settings.
- Structural ownership: PASS.
  - Root contains adapter create/apply/metric dispatch.
  - Root contains none of the three direct EdgeBasedMatching ToolType cases
    and no private EdgeBasedMatching PropertyGrid model.
  - The standalone adapter owns all three aliases and the private model.
- Post-move focused contract: PASS.
  - Command:
    `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\refactor_edge_based_adapter_20260726`
  - Result: `OK`, 1600x900, layout/text/internal errors all zero.
- Current-source visual check: PASS.
  - Artifact:
    `artifacts\refactor_edge_based_adapter_20260726\wpf_shell_host_edge_based_matching_tool.png`
  - The current build retained the Edge Based Matching Tool View, layer
    selectors, PropertyGrid, unique-match controls, preview result, viewer, and
    docking surface without clipping or overlap introduced by this refactor.
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

## Verification Notes

- The first smoke command invocation exceeded the command-yield window and
  temporarily left its own screenshot process holding `OpenCvSharpExtern.dll`;
  the process had exited before inspection, and the same command then ran
  normally.
- The first new assertion expected `VisionPipelineStep.Name` alone to override
  the persisted parameter `Name`. The existing mapper deliberately reads the
  parameter first. The test setup was corrected to set both representations,
  after which the unchanged pre-move implementation passed. This was a test
  expectation correction, not a product behavior change.

## Durable Completion Record

Status: Complete
Scope: EdgeBasedMatching selected-Step PropertyGrid mapping ownership moved
from a root partial to one standalone adapter, with existing behavior retained.
Acceptance criteria: baseline round-trip PASS; ownership search PASS;
post-move round-trip/UI PASS; build PASS; readiness PASS; patch hygiene PASS.
Verification: commands and results are recorded above.
Evidence:
`docs/admin/OPENVISIONLAB_EDGE_BASED_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and `artifacts/refactor_edge_based_adapter_20260726`.
Boundary / next dependency: this proves structural ownership and the named
PropertyGrid round-trip only. It does not qualify matching accuracy or authorize
matcher tuning. FeatureMatching should be considered next only after its own
selected-Step baseline exists.
