# Transform Property Model Ownership Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Complete the existing `VisionPipelineTransformPropertyAdapter` ownership
  boundary for `RotateScale` / `RotateAndScale` and `Affine` /
  `AffineMatrix` / `AffineTransform`.
- Move the two transform-specific PropertyGrid presentation models, the
  detected-Point feature converter, and transform metric classification out of
  the root `VisionPipelineStepPropertyMapper`.
- Preserve aliases, defaults, fixture parameters, detected Point bindings,
  acceptance metadata, layers, XML round trip, explicit Preview/Run, and
  visible PropertyGrid behavior.

## Excluded

- No `Mean` adapter was added solely to remove the last direct mapper case.
- No new interface, factory, registry, codec, algorithm, validation gate, XML
  key, layer/routing behavior, or product feature was introduced.

## Structural Changes Confirmed

- Before:
  `VisionPipelineTransformPropertyAdapter` owned create/apply mapping but
  instantiated
  `VisionPipelineStepPropertyMapper.PipelineRotateScaleToolProperty` and
  `VisionPipelineStepPropertyMapper.PipelineAffineTransformToolProperty`.
  The root also owned `PipelinePointFeatureConverter` and directly classified
  the two transform models for acceptance-metric choices.
- After:
  `VisionPipelineTransformPropertyAdapter` owns both transform PropertyGrid
  models, the Point feature converter, create/apply mapping, fixture/detected
  Point serialization, and transform metric classification.
- Evidence:
  `VisionPipelineStepPropertyMapper.cs` contains none of the three
  transform-specific type names and delegates transform metric classification
  to `VisionPipelineTransformPropertyAdapter.ResolveMetricToolType`.

## Call Path

- Old path:
  root mapper dispatch -> transform adapter -> root mapper nested presentation
  model -> transform adapter -> existing `VisionPipelineStepBuilder`.
- New path:
  root mapper dispatch -> transform adapter-owned presentation model ->
  transform adapter -> existing `VisionPipelineStepBuilder`.
- The root now owns shared metadata capture/copy, shared converters/codecs,
  feature-reference queries, family dispatch, and the deliberately retained
  single `Mean` mapping only.

## Acceptance Criteria

1. The root mapper contains no
   `PipelineRotateScaleToolProperty`,
   `PipelineAffineTransformToolProperty`, or
   `PipelinePointFeatureConverter`.
2. The existing transform adapter contains both models, the converter, both
   create/apply directions, and transform metric classification.
3. Affine aliases, known matrix, PropertyGrid/XML round trip, collinear
   rejection, coverage evidence, RotateScale UI, Affine UI, and P219 detected
   Point binding remain valid.
4. Debug build and readiness pass with zero warnings/errors.

## Checks Run

- `rg` ownership search:
  root has zero transform-specific model/converter hits; adapter owns all
  expected hits.
- `dotnet run --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --affine-transform-contract "artifacts\refactor_transform_model_ownership_20260726\affine_contract"`:
  passed aliases, known matrix, PropertyGrid/XML round trip, collinear
  rejection, and coverage evidence.
- `dotnet run --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target "wpf_shell_host_rotate_scale_tool,wpf_shell_host_affine_transform_tool,p219_affine_point_binding_property_grid" "artifacts\refactor_transform_model_ownership_20260726\ui"`:
  all three current-source targets passed.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  passed with 0 warnings and 0 errors after recovery from one intermediate
  truncated-edit build failure.
- Current-source visual review:
  RotateScale, AffineTransform, and P219 detected Point binding retained their
  controls and values without a new layout defect.

## Evidence

- `artifacts/refactor_transform_model_ownership_20260726/affine_contract`
- `artifacts/refactor_transform_model_ownership_20260726/ui/wpf_shell_host_rotate_scale_tool.png`
- `artifacts/refactor_transform_model_ownership_20260726/ui/wpf_shell_host_affine_transform_tool.png`
- `artifacts/refactor_transform_model_ownership_20260726/ui/p219_affine_point_binding_property_grid.png`

## Boundary / Next Dependency

The mapper decomposition campaign has no remaining cohesive OpenCV family to
extract. `Mean` is one small direct mapping whose removal would require a
one-case adapter without reducing a real maintenance boundary. Reopen mapper
structure only when a concrete `Mean` maintenance change or a verified
selected-Step regression supplies both a responsibility boundary and a focused
round-trip gate. This refactor does not prove transform metrology,
unseen-image robustness, or field qualification.
