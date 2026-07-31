# Property Mapper Matching Adapter Proof (2026-07-25)

Status: Complete

## Scope

Move the existing general `Matching`/`TemplateMatching` PropertyGrid mapping from the root mapper into a dedicated partial. The slice includes existing matching defaults, fixture-frame publication serialization, and the matching-specific PropertyGrid model.

## Intended responsibility boundary

- `VisionPipelineStepPropertyMapper.Matching.cs` owns general Matching creation and fixture publication serialization.
- The root mapper retains family dispatch and common step persistence.
- EdgeBasedMatching and FeatureMatching remain out of scope because their contracts have independent unique-result and feature-specific behavior.

## Acceptance criteria

- Matching creation and its model live in the Matching partial.
- Fixture frame publication parameters serialize exactly as before.
- A focused current-source Matching PropertyGrid smoke, Debug solution build, and readiness check pass.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- Current-source `wpf_shell_host_recipe_fixture_properties` smoke passed. It covers the existing Matching fixture workflow and its PropertyGrid-related fixture settings without an automatic Preview/Run.
- A source search confirms that general Matching creation, fixture publication serialization, and the specialized PropertyGrid model now reside in `VisionPipelineStepPropertyMapper.Matching.cs`; the root retains only family dispatch and common persistence.

## Evidence

- `artifacts\\maintenance_property_mapper_matching_adapter_20260725\\wpf_shell_host_recipe_fixture_properties.png\\wpf_shell_host_recipe_fixture_properties.png`
- `src\OpenVisionLab\UI\\Menu\\Wpf\\Recipe\\PropertyGrid\\VisionPipelineStepPropertyMapper.Matching.cs`

## Boundary

This is a structural maintenance slice. It does not alter Matching score/angle/scale behavior, template ownership, fixture semantics, XML defaults, EdgeBasedMatching, FeatureMatching, or explicit Preview/Run behavior.
