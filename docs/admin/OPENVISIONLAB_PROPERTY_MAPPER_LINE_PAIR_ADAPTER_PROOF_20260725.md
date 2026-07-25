# Property Mapper Line Pair Adapter Proof (2026-07-25)

Status: Complete

## Scope

Move the existing LineDistance/LineIntersection two-line PropertyGrid mapping from the root mapper into a dedicated partial. The slice includes pair construction, independent Left/Right baseline restoration, pair serialization, and the public line-pair projection used by the existing caller.

## Intended responsibility boundary

- `VisionPipelineStepPropertyMapper.LinePair.cs` owns LineDistance/LineIntersection pair creation and persistence.
- The root mapper retains only tool-family dispatch and common step flow.
- The single `LineGauge` mapping, runtime caliper behavior, XML keys, and Preview/Run are out of scope.

## Acceptance criteria

- The pair model and its creation/persistence helpers reside in the Line Pair partial.
- Existing asymmetric Left/Right settings still round-trip through the Recipe Manager workflow.
- A focused current-source Line Pair smoke, Debug solution build, and readiness check pass.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- Current-source `wpf_shell_host_recipe_line_pair_properties` smoke passed. It exercises the existing LineDistance pair editor, independent Line A/B ROI fields, and XML round-trip validation.
- A source search confirms that the pair model, pair construction, pair-to-step persistence, and the public line-pair projection now reside in `VisionPipelineStepPropertyMapper.LinePair.cs`; the root retains dispatch and common flow only.

## Evidence

- `artifacts\\maintenance_property_mapper_line_pair_adapter_20260725\\wpf_shell_host_recipe_line_pair_properties.png\\wpf_shell_host_recipe_line_pair_properties.png`
- `UI\\Menu\\Wpf\\Recipe\\PropertyGrid\\VisionPipelineStepPropertyMapper.LinePair.cs`

## Boundary

This is a structural maintenance slice. It does not change the caliper algorithm, LineGauge single-step behavior, XML key meanings, calibration, drawing semantics, or explicit Preview/Run behavior.
