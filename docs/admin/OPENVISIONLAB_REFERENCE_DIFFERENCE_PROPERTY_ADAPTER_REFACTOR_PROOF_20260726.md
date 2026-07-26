# ReferenceDifference Property Adapter Refactor Proof

Date: 2026-07-26
Baseline: Dev commit `e7e6aa7`

## Outcome

`VisionPipelineReferenceDifferencePropertyAdapter` now owns the complete
PropertyGrid/XML mapping boundary for the `ReferenceDifference` Tool family:

- ToolType recognition
- current parameters and defaults
- legacy semicolon-delimited `ReferencePaths` fallback
- PropertyGrid categories and editable values
- canonical `VisionPipelineStep` reconstruction
- metric-owner identification

The root `VisionPipelineStepPropertyMapper` only dispatches to the adapter and
continues to apply shared Step metadata and parameter preservation. The adapter
is a standalone non-partial class; this change did not create another command
surface or partial-file split.

## Structural Evidence

Previous owner:

- `VisionPipelineStepPropertyMapper.CreatePropertyCore` directly recognized
  `referencedifference`, interpreted all defaults and legacy paths, and created
  its private PropertyGrid model.
- `VisionPipelineStepPropertyMapper.ApplyProperty` directly called the private
  model's `ToStep`.
- The private nested model owned all editable reference, defect, registration,
  and acceptance fields.

Current owner:

- `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineReferenceDifferencePropertyAdapter.cs`
  contains the recognition, projection, model, fallback, and reconstruction
  behavior.
- `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`
  contains only three adapter integration points: property creation, Step
  creation, and metric ToolType resolution.
- The former nested `PipelineReferenceDifferenceProperty`, direct switch case,
  and root `GetReferencePath` helper are absent.

The readiness contract now checks the adapter as the owner and separately
checks that the root mapper dispatches to it. This prevents the structural
verification from requiring the old coupling.

## Preserved Contract

- Missing `ReferencePath1..4` values still fall back by position to the legacy
  semicolon-delimited `ReferencePaths` parameter.
- Defaults remain `35`, `80`, `20000`, `3`, `8`, `1600`, `0.75`, `12`, and
  `3.0` for the existing defect and registration fields.
- Apply still emits canonical ToolType `ReferenceDifference` and all thirteen
  current parameters using invariant formatting.
- Step name, enabled state, input/output layers, and acceptance metadata still
  use the root mapper's shared copy path.
- PropertyGrid editing does not trigger Preview or Run.

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_reference_difference_properties artifacts\refactor_reference_difference_adapter_20260726`
  - pass
  - verifies four reference paths, edited parameter round trip, actual Recipe
    Manager PropertyGrid/search visibility, and zero Preview/Run
- Visual inspection:
  - `artifacts/refactor_reference_difference_adapter_20260726/wpf_shell_host_recipe_reference_difference_properties.png`
  - all four reference paths are visible in the current-source Recipe Manager
    PropertyGrid and the layout remains usable
- `dotnet build tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug --nologo`
  - pass, 0 warnings, 0 errors
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - pass
- structural search and `git diff --check`
  - pass

## Boundary

This is a responsibility extraction only. It does not change the
`ReferenceDifference` runtime algorithm, validation rules, result metrics,
recipe schema, LLM workflow, visible layout, layers, routing, or explicit
Preview/Run contract. It does not establish new inspection or field-robustness
evidence.

## Completion Record

Status: Complete
Scope: Move the existing `ReferenceDifference` PropertyGrid/XML mapping family
from the root mapper to one standalone non-partial adapter.
Acceptance criteria: Root direct family implementation absent; legacy/current
mapping preserved; dedicated round-trip/current UI smoke passes; full build and
readiness pass.
Verification: Commands and results listed above.
Evidence:
`docs/admin/OPENVISIONLAB_REFERENCE_DIFFERENCE_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
and
`artifacts/refactor_reference_difference_adapter_20260726/wpf_shell_host_recipe_reference_difference_properties.png`.
Boundary / next dependency: Re-audit remaining root mapper families and extract
another only when a dedicated round-trip regression can prove the boundary.
