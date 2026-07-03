# OpenVisionLab Beginner Learn Mode And Recipe Context - 2026-07-01

This document turns the beginner-friendly product direction into implementation priorities. It is intentionally scoped around the existing OpenVisionLab contracts: PropertyGrid remains the model-driven editor, Preview/Run remain explicit user commands, and output layer creation must not silently change input routing.

## Product Direction

OpenVisionLab is a rule-based vision workbench for users who need to learn, tune, and verify inspection logic with real images. The beginner path should not hide the professional tool model. It should put a learning layer over the existing sample, recipe, PropertyGrid, result, and Pipeline Review flow.

## Requirements Reflected

1. Sample-centered Learn Mode
   - Users should enter through examples such as `Matching 배우기`, `Blob으로 얼룩/입자 찾기`, and `Line으로 거리/각도 측정하기`.
   - Samples are not demos only; they are benchmark references tied to a recipe, expected metrics, and Good/Bad context.

2. Tool presets and recommended starts
   - Each major tool family needs beginner starts such as `기본 검사`, `빠른 검사`, and `정밀 검사`.
   - Presets must be explicit commands that update model properties in the PropertyGrid-owned model.
   - Applying a preset must not run Preview/Run automatically.

3. Result explanation
   - Result panels should translate metrics into decisions:
     - `Score 0.87 >= 0.60, OK`
     - `Angle -5.1 deg, inside target band`
     - `Many candidates, false-positive risk`
   - Numeric metric visibility stays; the beginner explanation sits above it.

4. Failure cause guidance
   - NG/error states should point to likely causes:
     - Template too large
     - ROI too wide
     - Threshold range too narrow
     - Weak edges
     - Too many candidates
   - Guidance should name the parameter family to inspect before suggesting arbitrary tuning.

5. Good/Bad image pairs
   - Rule-based reliability depends on explaining why normal and defect samples separate by stable metrics.
   - Pair expansion should focus on public, repeatable samples with clear metric margins.

6. Recipe context switching
   - A user should be able to work with more than one recipe context, for example using one recipe for a matching inspection and another recipe for a measurement/defect inspection.
   - Recipe switching must be an explicit context selection, not a hidden global state mutation.
   - Tool views, Pipeline Review, docked inspectors, and sample flows must show which recipe context they are using.

## Implemented In This Pass

### Pass 1. Learn Mode Display Foundation

- Sample Catalog now has a Learn Mode strip for the selected sample:
  - Learn path
  - Recommended start
  - Result interpretation
  - Failure cause summary
- Good/Bad pair samples now show a compact pair-comparison strip:
  - OK count
  - NG count
  - opposite reference sample
- The implementation is display-only:
  - no Preview/Run
  - no tool open
  - no output layer creation
  - no route mutation

### Pass 2. RecipeContext Foundation

- Added an app-local `OpenVisionRecipeContext` shape:
  - `Id`
  - `Name`
  - `PipelineName`
  - `SourcePath`
  - `IsDirty`
  - `ActiveLayerName`
  - `LastReviewState`
- Added `OpenVisionRecipeContextStore` as the single ShellHost recipe/pipeline context resolver.
- Added `OpenVisionShellHostRecipeContextPresenter` so the top bar shows the active recipe and active pipeline as a compact status chip.
- Existing Shell controllers still receive recipe names through a callback, but the callback now resolves through the context store.
- Recipe switching remains explicit through the existing `RecipeState.Name` path. Context refresh does not run Preview/Run, open a tool, create a layer, or mutate routing.
- Added focused smoke coverage:
  - `wpf_shell_host_recipe_context_switch`
  - verifies recipe A/B active pipeline changes
  - verifies no auto Preview/Run/tool open on context switch

### Pass 3. RecipeContext Controller Propagation

- Updated key Shell controller boundaries to receive `Func<OpenVisionRecipeContext>` instead of `Func<string>`:
  - `OpenVisionShellHostCommandController`
  - `OpenVisionShellHostSampleWorkflowPresenter`
  - `OpenVisionShellHostToolWindowController`
- `OpenVisionPipelineReviewDocument` now stores a recipe context snapshot and resolves its active pipeline from that context.
- The Pipeline Review path remains explicit:
  - switching recipe context does not open Pipeline Review
  - opening Pipeline Review is still a user command
  - opening Pipeline Review does not run Preview/Run
- `wpf_shell_host_recipe_context_switch` now also verifies Pipeline Review context propagation.

### Pass 4. Tool Preset Foundation

- Added a shared `VisionToolPreset<TProperty>` contract for model-level beginner starts.
- Added `basic`, `fast`, and `precise` presets for Matching, EdgeBasedMatching, and FeatureMatching property models.
- Added a reusable preset host to the shared single-input PropertyGrid shell.
- Preset application updates the selected PropertyGrid-backed model only:
  - it does not change input/output layer routing
  - it does not create an output layer
  - it does not run Preview/Run
  - it does refresh generated PropertyGrid rows and matching summaries
- The first UI exposure is floating Matching-family tools. Docked inspectors currently collapse the preset bar to preserve the stable PropertyGrid editor viewport.
- Added focused smoke coverage:
  - `wpf_shell_host_matching_presets`
  - verifies preset buttons and localized text
  - verifies exact model values after Basic/Fast/Precise
  - verifies no Preview/Run execution even when `AUTO_PREVIEW=true` was enabled before applying a preset

### Pass 5. Docked Preset Header Menu

- Docked Matching-family inspectors now expose the same preset commands through a compact button in the existing `Parameters` header.
- The docked header menu does not consume PropertyGrid body height.
- Floating tools still show the full preset strip with title/detail text.
- The shared shell keeps the custom parameter header localized through `VisionToolChromePresenter`.
- `wpf_shell_host_matching_presets` now verifies:
  - floating preset strip behavior
  - docked preset menu visibility
  - docked preset application
  - no Preview/Run execution from either path

### Pass 6. Matching Result Explanation Helper

- Added `VisionToolMatchingResultExplanation` as a display-only formatter for Matching-family result interpretation.
- Matching result guidance now explains the core pass reason with configured metrics:
  - best score against minimum score
  - detected count against requested count
  - angle/scale result when those searches are enabled
  - repeated-candidate risk when multiple matches pass
- NG/empty guidance now names likely parameter families:
  - template ROI
  - minimum score
  - Canny range and edge contrast
  - Ratio/RANSAC for Feature Matching
  - contrast, blur, and candidate count
- The helper does not run Preview/Run, create layers, change routing, or change pass/fail semantics.
- Smoke coverage:
  - `wpf_shell_host_matching_tool`
  - `wpf_shell_host_edge_based_matching_tool`
  - `wpf_shell_host_feature_matching_tool`
  - `localization_catalog_contract_check`

### Pass 7. Blob/Contour Area Result Explanation Helper

- Added `VisionToolAreaResultExplanation` as a display-only formatter for area-style tool result interpretation.
- Blob/Contour result guidance now explains:
  - detected region count
  - maximum area
  - maximum box size
  - threshold/ROI/area criteria pass state
  - likely failure-cause parameter families
- Failure guidance points beginners toward:
  - threshold range
  - ROI
  - area limits
  - masking/morphology for Blob
  - contour retrieval and weak boundaries for Contour
- The helper does not run Preview/Run, create layers, change routing, or change Blob/Contour metric semantics.
- Smoke coverage:
  - `wpf_shell_host_blob_tool`
  - `wpf_shell_host_contour_tool`
  - `localization_catalog_contract_check`

### Pass 8. Line Result Explanation Helper

- Added `LineToolResultExplanation` as a display-only formatter for Line tool result interpretation.
- Line result guidance now explains:
  - Edge: line count, edge-point count, fitted-line length, and stability check
  - Measure: px/mm distance, detected count, and Line A/B scan-direction meaning
  - Intersection: cross/no-cross result and likely failure-cause parameter families
- Failure guidance points beginners toward:
  - ROI
  - contrast
  - polarity
  - threshold
  - scan angle/direction
  - sampling interval
  - Line A/B geometry
- The helper does not run Preview/Run, create layers, change routing, or change Line metric semantics.
- Smoke coverage:
  - `wpf_shell_host_line_tool`
  - `wpf_shell_host_line_pins_measure_tool`
  - `wpf_shell_host_line_intersection_tool`
  - `localization_catalog_contract_check`

### Pass 9. Good/Bad Pair Decision Guide

- Added `OpenVisionWorkspaceSamplePairDecisionGuidePresenter` to format pair-specific decision guidance outside the sample picker XAML.
- Good/Bad pair samples now explain:
  - selected sample role
  - opposite OK/NG reference samples
  - shared separating metrics and their expected ranges
  - manual review order: verify OK first, then run the same pipeline on NG and compare margin
- The guide is display-only:
  - no Preview/Run
  - no tool open
  - no output layer creation
  - no route mutation
  - no recipe threshold rewrite
- Smoke coverage:
  - `wpf_shell_host_workspace_sample_picker`
  - `wpf_shell_host_workspace_sample_pair_picker`
  - `localization_catalog_contract_check`

### Pass 10. Task-Oriented Learn Path Entry Grouping

- Added `OpenVisionWorkspaceSampleLearnPathOption` and `OpenVisionWorkspaceSampleLearnPathClassifier`.
- The sample picker now exposes task-oriented Learn paths before the raw sample list:
  - Matching
  - Blob
  - Contour
  - Line
  - Mean
  - Good/Bad
- Selecting a Learn path filters the catalog and selected sample only.
- The implementation is display/filter-only:
  - no sample open
  - no Preview/Run
  - no tool open
  - no output layer creation
  - no route mutation
  - no recipe value rewrite
- Smoke coverage:
  - `wpf_shell_host_workspace_sample_picker`
  - `wpf_shell_host_workspace_sample_learn_paths`
  - `wpf_shell_host_workspace_sample_pair_picker`

## Next Implementation Priorities

### P1. Result Explanation Presenter

Unify metric-to-language formatting. Matching-family, Blob/Contour, Line, and SimplePreprocess now have first shared implementations; remaining work is to consolidate the repeated formatting patterns where it reduces duplication.

Examples:

- Matching: score, count, angle, scale, candidate risk. Done for the shared Matching-family result guidance path.
- Blob/Contour: count, area, bounds, candidate density. Done for the shared area-style result guidance path.
- Line: edge count, line length, angle, distance. Done for the Line result guidance path.
- Mean/measurement: actual value against normal band. First SimplePreprocess path is implemented.

Rules:

- Result explanations are display-only.
- They must not run Preview/Run, create layers, change routing, or alter pass/fail semantics.
- They should name the metric and the configured threshold so beginners can understand why a result is OK/NG.

### P2. Failure Cause Presenter

Map failures to likely parameter families.

Examples:

- Template/feature tools: template crop, ROI, score threshold, candidate count, edge/feature weakness.
- Blob/Contour: threshold range, ROI, area limits, morphology size.
- Line: ROI, polarity, contrast, sampling step, edge count.

### P3. Remaining Recipe Context Propagation

The app-local recipe context model exists and the main Shell controller boundaries now receive context providers. Remaining work is to push context IDs deeper into tool runtime/session persistence where needed.

- ShellHost can expose an active recipe selector without every tool assuming a single global recipe.
- Tool windows receive a recipe context reference or ID through a controller/service boundary.
- Switching recipe context updates displayed context only; it must not run Preview/Run.
- Output layer creation in one recipe must not rewrite another recipe's selected input layer.

### P4. Learn Mode Entry Points

Make sample selection more task-oriented. The first entry grouping is now implemented inside the sample picker. Remaining work is to refine the visual learning cards and expand sample metadata when new public samples are added.

Examples:

- `Matching 배우기`
- `Blob으로 얼룩/입자 찾기`
- `Contour로 형상/개수 검출하기`
- `Line으로 거리/각도 측정하기`
- `Mean으로 밝기 변화 측정하기`

Implementation rule:

- This should be sample metadata and view-model grouping first, not a separate wizard that bypasses the recipe/sample contract.

### P5. Tool Preset Expansion

Expand the shared preset model to more major tool families.

Minimum shape:

```text
VisionToolPreset
- ToolType
- PresetId
- DisplayName
- Description
- AppliesToPropertyNames
- Apply(model)
```

Rules:

- PropertyGrid remains the source of editable truth.
- Preset application updates model properties and refreshes generated PropertyGrid rows.
- Preset application must not run Preview/Run, even if the selected model has an opt-in auto-preview switch.

### P6. Good/Bad Pair Expansion

Expand only when the pair has a clear and repeatable metric boundary.

Preferred additions:

- matching target/no-target and wrong-target pairs
- surface defect size/severity pairs
- pin/line angle and distance pairs
- brightness drift pairs
- calibration-sensitive measurement pairs

## Verification Checklist

Every UI change in this area should include:

- before/after screenshot artifacts
- focused screenshot smoke target
- build check
- explicit assertion that display-only guidance does not run Preview/Run
- explicit assertion that sample/recipe context changes do not mutate unrelated input routing
