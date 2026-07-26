# Line Property Adapter Consolidation Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `c8e76c2`
Status: Complete

## User Goal

Continue structural MVVM-style refactoring through cohesive responsibility
owners without mechanically creating one file per remaining switch case.

## Refactor Proof Plan

### Current Structure

- Current responsibility owner:
  single `Line`/`LineGauge` mapping and its private editable model remain in
  `VisionPipelineStepPropertyMapper`; `VisionPipelineLinePairPropertyAdapter`
  separately owns `LineDistance`/`LineIntersection`.
- Current call path:
  single Line uses root ToolType/model/generic builder/metric switches, while
  pair Line uses root adapter dispatch -> LinePair adapter.
- Current dependency direction:
  the root directly knows one Line-family editable type even though a Line
  adapter already exists.
- Current state/data owner:
  single-Line projection state is root-owned; paired-Line projection and
  baseline-preservation state are adapter-owned.

### Intended New Structure

- New responsibility owner:
  renamed standalone `VisionPipelineLinePropertyAdapter` for the entire Line
  family.
- New call path:
  root Line adapter dispatch -> adapter single/pair create/reconstruction/
  metric identification -> root shared metadata/final Step copy.
- New dependency direction:
  the root depends on one Line-family adapter contract and no Line-family
  private model.
- New state/data owner:
  the Line adapter owns both single and paired editable mapping state. Tool
  View state, Pipeline Step persistence, runtime, layers, Preview, and Run
  remain unchanged.

### Structural Conditions

1. A real Tool View-generated single `LineGauge` Step passes canonical/alias
   create/apply, parameter, metadata, and layer round-trip before ownership
   moves.
2. Existing LineDistance/LineIntersection selected-Step and geometry
   regressions remain passing.
3. The root contains no direct single-Line ToolType/model/metric ownership.
4. The renamed adapter owns `Line`/`LineGauge`, `LineDistance`, and
   `LineIntersection` while preserving the existing public
   `TryCreateLineGaugePair` compatibility path.
5. `Mean` remains in the root because grouping it with Line would not form a
   cohesive domain boundary.

### Proof Checks

- Baseline:
  current Line Tool smoke with single-Line selected-Step assertions, plus the
  existing Line Pair PropertyGrid target.
- Search:
  root direct Line owner absent; one full Line adapter and root dispatch
  present; old adapter name/path absent.
- Focused:
  current-source Line Tool, Line Pair PropertyGrid, and P213 geometry checks.
- Final:
  Debug solution build, readiness, and `git diff --check`.

## Result

### Boundary Decision

- Rejected a proposed `LineGauge + Mean` adapter because the two tools share
  only their position as the final root cases, not a domain responsibility.
- Extended the existing Line adapter instead:
  `VisionPipelineLinePairPropertyAdapter` became
  `VisionPipelineLinePropertyAdapter`.
- `Mean` deliberately remains in the root. No one-case Mean adapter or generic
  measurement abstraction was added.

### Ownership Change

- `VisionPipelineLinePropertyAdapter` now owns:
  - `Line`/`LineGauge` recognition and single-Line parameter/default
    projection;
  - the single-Line editable PropertyGrid model;
  - single-Line Step reconstruction and metric identification;
  - the existing `LineDistance`/`LineIntersection` pair projection, baseline
    preservation, editable model, reconstruction, and metric identification;
  - the existing Tool View `TryCreateLineGaugePair` compatibility handoff.
- `VisionPipelineStepPropertyMapper` retains one Line adapter dispatch plus
  shared metadata/final Step copying. It no longer owns a direct Line case,
  single-Line projection, private Line model, or Line metric case.
- No Line algorithm, pair semantics, ROI, projection, drawing, result, layer,
  Tool View, Preview, Run, or Geometry behavior changed.

### Before And After Flow

```text
Before
single Line: root switch/model/generic builder/metric
paired Line: root -> LinePair adapter

After
all Line: root -> Line adapter -> single or paired mapping
          -> root shared metadata/final Step copy
```

The root mapper fell from 1,263 to 1,150 lines. The existing Line adapter grew
from 628 to 761 lines because it absorbed the related single-Line mapping and
model. No additional production file or abstraction was created.

## Acceptance Criteria And Evidence

- Single-Line baseline: PASS.
  - Command:
    `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\refactor_line_family_adapter_baseline_20260726`
  - Result: `OK`, 1600x900, layout/text/internal errors all zero.
  - The new assertion starts from a real Tool View-generated `LineGauge` Step
    and proves `LineTool`/`LineGaugeTool`, canonical apply, XML/Step name,
    layers, acceptance metadata, calibration, ROI/threshold, projection,
    polarity, scan, manual-angle, extended-line, averaging, and drawing
    parameters.
- Existing paired-Line baseline: PASS.
  - Target: `wpf_shell_host_recipe_line_pair_properties`.
  - Result: `OK`, 1600x900, layout/text/internal errors all zero.
- Structural ownership: PASS.
  - Root contains Line adapter create/apply/metric dispatch and the public
    compatibility forwarder.
  - Root contains no direct `Line`/`LineGauge` case, private single-Line model,
    Line metric case, or old adapter name.
  - The renamed adapter owns single and paired aliases/models.
  - Root still contains the intended `Mean` case and private Mean model.
- Post-move Line Tool: PASS.
  - Target: `wpf_shell_host_line_measure_tool`.
  - Artifact:
    `artifacts\refactor_line_family_adapter_20260726\wpf_shell_host_line_measure_tool.png`.
- Post-move Line Pair PropertyGrid: PASS.
  - Target: `wpf_shell_host_recipe_line_pair_properties`.
- P213 geometry non-regression: PASS.
  - Targets: `p213_geometry_property_grid`, `p213_geometry_review`.
- Current-source visual check: PASS.
  - The Line Tool retained input/output previews, purpose and Line A/B
    selectors, PropertyGrid controls, result drawing/review, action buttons,
    viewer, and docking controls without new clipped text/icons, hidden input
    content, or incoherent overlap.
- Full Debug solution build: PASS.
  - Command:
    `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" --nologo`
  - Result: 0 warnings, 0 errors.
- Readiness: PASS after correcting the check described below.
  - Command:
    `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - Result: all readiness contracts passed.
- Patch hygiene: PASS.
  - Command: `git diff --check`
  - Result: no whitespace errors; Git reported only existing Windows
    line-ending normalization notices.

## Verification Notes

- The first updated readiness run expected `case "line"` and
  `case "linegauge"` tokens. The adapter uses the simpler existing conditional
  `toolType == "line" || toolType == "linegauge"`. The structure gate was
  corrected to inspect the actual ownership syntax and then passed. Production
  behavior was not changed for this check.

## Refactor Proof Report

### Structural Changes Confirmed

- Before: root and LinePair adapter split one Line family across two owners.
- After: one Line adapter owns all single/pair mapping paths.
- Evidence: root/adapter ownership searches and all focused targets above.

### Dependency And State Flow

- Dependency direction now:
  root mapper -> one Line adapter -> existing shared mapper readers/builders.
- State/data owner now:
  adapter-owned single/pair editable projection; unchanged Pipeline Step,
  runtime, and Tool View remain persistence/execution/UI owners.

### Remaining Structural Work

- Current owner/coupling:
  `Mean` remains the sole direct root OpenCV family.
- Intended owner/path:
  no new owner until a concrete Mean mapper maintenance need exists.
- Required change:
  none for file-size, symmetry, or “zero remaining switch cases”.
- Next proof check:
  focused Mean selected-Step baseline only when actual Mean maintenance
  requires it.

## Durable Completion Record

Status: Complete
Scope: consolidated single `LineGauge`, `LineDistance`, and
`LineIntersection` PropertyGrid mapping under one renamed Line adapter while
leaving unrelated Mean mapping in the root.
Acceptance criteria: pre-move single/pair baselines PASS; ownership search
PASS; post-move Line/pair/geometry UI PASS; build PASS; readiness PASS; patch
hygiene PASS.
Verification: commands and results are recorded above.
Evidence:
`docs/admin/OPENVISIONLAB_LINE_PROPERTY_ADAPTER_CONSOLIDATION_REFACTOR_PROOF_20260726.md`
and `artifacts/refactor_line_family_adapter_20260726`.
Boundary / next dependency: this proves Line-family mapping ownership and named
round trips only. It does not qualify Line algorithm accuracy or authorize
parameter tuning. Mean remains intentionally unextracted without a concrete
maintenance trigger.
