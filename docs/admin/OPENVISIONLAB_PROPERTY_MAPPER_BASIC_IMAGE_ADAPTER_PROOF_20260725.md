# Property Mapper Basic Image Adapter Refactor Proof

## User goal

Reduce `VisionPipelineStepPropertyMapper` responsibility mixing while preserving existing PropertyGrid/XML behavior.

## Current structure

- Current responsibility owner: one mapper switch creates and reapplies every tool family.
- Current call path: `CreateProperty` / `ApplyProperty` branch directly through Threshold, Morphology, Filter, and EdgeDetection details.
- Current dependency direction: the root mapper owns both tool-family selection and every parameter mapping.
- Current state/data owner: the pipeline Step owns parameters; PropertyGrid models only project them.

## Intended new structure

- New responsibility owner: a Basic Image adapter partial owns the four independent pixel-preprocessing tool mappings.
- New call path: the root mapper dispatches the tool family; the adapter creates or reapplies the family model.
- New dependency direction: the adapter reuses the existing mapper metadata/helpers and `VisionPipelineStepBuilder` without new abstractions.
- New state/data owner: unchanged.

## Structural conditions

1. The root mapper contains no parameter-level create/apply mapping for the Basic Image family.
2. Existing ToolType aliases and default values remain unchanged.
3. The original builder paths remain the only Step serialization path.

## Proof checks

- Search checks: Basic Image mapping methods are owned by the new partial file.
- Test/build: PropertyGrid smoke, readiness check, and Debug solution build.

## Refactor proof report

Status: Complete

### Structural changes confirmed

- Before: the root mapper directly owned the Basic Image family create/apply parameter mappings.
- After: `VisionPipelineStepPropertyMapper.BasicImage.cs` owns Threshold, Morphology, Filter, and EdgeDetection mapping details.
- Evidence: the root mapper now dispatches only through `CreateBasicImageProperty` and `TryApplyBasicImageProperty`.

### Call path

- Old path: root mapper tool switch -> family parameter model; root apply chain -> builder.
- New path: root mapper dispatch -> Basic Image adapter -> unchanged `VisionPipelineStepBuilder` factory.
- Evidence: `wpf_shell_host_pipeline_step_edit_handoff` current-source smoke passed without Preview/Run side effects.

### Responsibility split

- Moved responsibility: Basic Image PropertyGrid projection and pipeline Step reconstruction.
- New owner: `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.BasicImage.cs`.
- Preserved owner: `VisionPipelineStepBuilder` remains the only XML-compatible Step serialization owner.

### Checks run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: pass, 0 warnings, 0 errors.
- `dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: pass, 0 warnings, 0 errors.
- `dotnet run --no-build --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_pipeline_step_edit_handoff artifacts\maintenance_property_mapper_basic_image_adapter_20260725`: pass.

### Evidence

- `artifacts/maintenance_property_mapper_basic_image_adapter_20260725/wpf_shell_host_pipeline_step_edit_handoff.png`

### Boundary / next dependency

- This completes one independent tool-family adapter only. Object/line, geometry, matching, and transform mapping remain in the root mapper for later, separate slices.
