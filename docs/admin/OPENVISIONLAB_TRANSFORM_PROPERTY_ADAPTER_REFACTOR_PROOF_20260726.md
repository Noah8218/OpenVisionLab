# Transform Property Adapter Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move `RotateScale` / `RotateAndScale` and `Affine` / `AffineMatrix` /
  `AffineTransform` PropertyGrid Create/Apply mapping out of the root
  `VisionPipelineStepPropertyMapper`.
- Preserve fixture-consumer parameters, detected Point bindings, aliases,
  defaults, acceptance metadata, layers, and XML round-trip behavior.

## Excluded

- No transform algorithm, interpolation, validation-gate, Preview/Run, layer,
  routing, or product-scope change.
- No additional mapper family or speculative interface/factory.

## Structural Change

- Previous owner:
  `VisionPipelineStepPropertyMapper.CreatePropertyCore` and `ApplyProperty`
  directly constructed transform properties, invoked the two builders, and
  appended fixture/detected-Point parameters.
- Current owner:
  `VisionPipelineTransformPropertyAdapter` owns transform alias recognition,
  parameter/default projection, Step creation, fixture parameters, and
  detected-Point binding parameters.
- Current call path:
  root mapper dispatch -> `VisionPipelineTransformPropertyAdapter` -> existing
  `VisionPipelineStepBuilder`.
- The root mapper retains shared Step metadata capture/copy and dispatch only.

## Acceptance Criteria

1. The root mapper contains no transform ToolType cases and no direct
   `FromRotateScaleProperty` / `FromAffineTransformProperty` call.
2. One non-partial adapter owns both Create and Apply directions.
3. Current-source Affine contract, focused PropertyGrid UI smokes, Debug build,
   and readiness check pass.

## Verification

- `rg` confirmed no transform cases or direct transform builder calls remain in
  the root mapper.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed
  with 0 warnings and 0 errors.
- `dotnet run --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --affine-transform-contract "artifacts\refactor_transform_adapter_20260726"`
  passed aliases, known matrix, PropertyGrid/XML round trip, collinear
  rejection, and coverage checks.
- Current-source UI smokes passed:
  `wpf_shell_host_rotate_scale_tool`,
  `wpf_shell_host_affine_transform_tool`, and
  `p219_affine_point_binding_property_grid`.
- UI artifacts:
  `artifacts/refactor_transform_adapter_20260726/ui`.

## Boundary

This proves a real mapping call-path and responsibility change. The existing
PropertyGrid presentation models remain nested in the root mapper for
compatibility; their parameter projection and Step construction are no longer
owned there. This does not qualify transform metrology or unseen-image
robustness.
