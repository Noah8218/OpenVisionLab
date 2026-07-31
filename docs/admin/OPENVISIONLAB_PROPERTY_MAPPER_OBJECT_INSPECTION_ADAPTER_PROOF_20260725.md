# Property Mapper Object Inspection Adapter Proof (2026-07-25)

Status: Complete

## Scope

Move the existing Blob and Contour PropertyGrid mapping family from the root `VisionPipelineStepPropertyMapper` into a dedicated partial. The moved responsibility includes property creation, Blob fixture parameter serialization, and the two pipeline-aware PropertyGrid models.

## Intended responsibility boundary

- `VisionPipelineStepPropertyMapper.ObjectInspection.cs` owns Blob/Contour mapping and its object-inspection-specific serialization.
- The root mapper retains only family dispatch and common step persistence.
- Existing defaults, aliases, XML keys, acceptance metadata, and explicit Preview/Run behavior are unchanged.

## Acceptance criteria

- The root mapper delegates Blob/Contour creation and no longer owns their models.
- Blob fixture parameters retain their existing serialization behavior.
- A focused current-source PropertyGrid smoke, Debug solution build, and readiness check pass.

## Verification

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- Current-source `p216_object_dimension_filters_property_grid` smoke passed. It covers the Blob/Contour PropertyGrid family, the existing area and width/height filters, and the explicit Preview-only workflow.
- A source search confirms that Blob/Contour creation, object-inspection serialization, and both specialized PropertyGrid models now belong to `VisionPipelineStepPropertyMapper.ObjectInspection.cs`; the root only dispatches the family and applies common step persistence.

## Evidence

- `artifacts\\maintenance_property_mapper_object_inspection_adapter_20260725\\p216_object_dimension_filters_property_grid.png\\p216_object_dimension_filters_property_grid.png`
- `src\OpenVisionLab\UI\\Menu\\Wpf\\Recipe\\PropertyGrid\\VisionPipelineStepPropertyMapper.ObjectInspection.cs`

## Boundary

This is a structural maintenance slice. It does not add Blob/Contour metrics or filters, change legacy XML defaults, alter object-result semantics, or run Preview/Run automatically.
