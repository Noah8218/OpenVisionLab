# Property Mapper Context Refactor Proof

## User goal

Make the selected-Step PropertyGrid mapping easier to understand and remove hidden mutable mapper context without changing recipe behavior.

## Current structure

- Current responsibility owner: `VisionPipelineStepPropertyMapper` stores selected-pipeline feature accessors as mutable static delegates.
- Current call path: Recipe Manager loads a Step, sets global delegates, then creates a PropertyGrid model.
- Current dependency direction: PropertyGrid converters depend on mapper-global state.
- Current state/data owner: Recipe Manager owns the selected pipeline and step, but the mapper retains its derived context globally.

## Intended new structure

- New responsibility owner: `VisionPipelinePropertyContext` carries the selected pipeline and step index for one PropertyGrid model.
- New call path: Recipe Manager creates the context and passes it to `CreateProperty`.
- New dependency direction: PropertyGrid feature converters read the context attached to their own model.
- New state/data owner: Recipe Manager remains the state owner; the mapper receives only an explicit read-only snapshot.

## Structural conditions

1. No mutable static accessor delegate remains in `VisionPipelineStepPropertyMapper`.
2. Geometry and Affine Point dropdowns use the context for the selected Step.
3. Existing parameter mapping, XML keys, and Preview/Run behavior remain unchanged.

## Proof checks

- Search checks: no `SetGeometryFeatureContext` or `SetPointFeatureContext` caller remains.
- Call path checks: Recipe Manager and mapper smoke use `VisionPipelinePropertyContext`.
- Test/build: focused mapper smoke, readiness check, and Debug solution build.

## Refactor proof report

Status: Complete

### Structural changes confirmed

- Before: `VisionPipelineStepPropertyMapper` held mutable static delegates for Geometry and Point feature lookup.
- After: `VisionPipelinePropertyContext` owns one selected-pipeline/Step-index snapshot and is supplied to the affected PropertyGrid model.
- Evidence: `SetGeometryFeatureContext`, `SetPointFeatureContext`, and the three mutable accessor fields have no remaining source caller or declaration.

### Call path

- Old path: Recipe Manager set mapper-global delegates, then called `CreateProperty(step)`.
- New path: Recipe Manager calls `CreateProperty(step, new VisionPipelinePropertyContext(pipeline, selectedStepIndex))`.
- Evidence: the same explicit context path is used by the focused P213 PropertyGrid smoke.

### Responsibility split

- Moved responsibility: selected-Step feature lookup context.
- New owner: `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelinePropertyContext.cs`.
- Evidence: Geometry and Affine feature converters read the context stored by their own PropertyGrid model.

### Dependency and state flow

- Dependency direction now: Recipe Manager state -> explicit PropertyGrid context -> feature converter.
- State/data owner now: Recipe Manager remains the selected recipe/pipeline owner; the mapper no longer stores selected-pipeline delegates globally.
- Evidence: no XML key, default-value, Preview/Run, layer, or routing code changed in this slice.

### Related regression repaired

- The current screenshot-smoke build exposed two Pipeline Review test members lost during the earlier event partial extraction.
- `OpenVisionPipelineReviewView.Events.cs` now preserves the original internal selection contract, success/failure result, and geometry hit tolerance.

### Checks run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: pass, 0 warnings, 0 errors.
- `dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU"`: pass, 0 warnings, 0 errors.
- `dotnet run --no-build --project "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target p213_geometry_property_grid artifacts\maintenance_property_mapper_context_refactor_20260725`: pass.
- `dotnet run --no-build --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"`: pass.

### Evidence

- `artifacts/maintenance_property_mapper_context_refactor_20260725/p213_geometry_property_grid.png`
- This proof record and the current Debug build/readiness output.

### Boundary / next dependency

- This completes removal of the hidden mapper context only. Tool-family adapter extraction remains a separate MVVM slice.
