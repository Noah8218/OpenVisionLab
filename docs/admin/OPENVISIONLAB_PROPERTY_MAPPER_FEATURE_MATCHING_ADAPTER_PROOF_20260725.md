# FeatureMatching Property Mapper Adapter Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move the Feature/FeatureMatching/SIFT PropertyGrid adapter from the root mapper into a dedicated partial.
- Preserve the score, RANSAC reprojection, template-path, common OpenCV parameter, and acceptance metadata contract.

## Excluded

- No feature-matcher/runtime algorithm change.
- No Preview/Run, layer, route, or matching qualification behavior change.

## Acceptance Criteria

1. The root mapper retains only the FeatureMatching tool-family dispatch.
2. The dedicated partial owns the PropertyGrid model and parameter/default mapping.
3. Current-source FeatureMatching Tool View smoke, Debug build, and readiness check pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs` now delegates only the three FeatureMatching aliases to `CreateFeatureMatchingProperty`.
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.FeatureMatching.cs` owns the adapter defaults, PropertyGrid model, step metadata, and acceptance metadata.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_feature_matching_tool "C:\\Git\\OpenVisionLab_Dev\\artifacts\\maintenance_property_mapper_feature_matching_adapter_20260725"` passed.
- Current-source UI artifact: `artifacts/maintenance_property_mapper_feature_matching_adapter_20260725/wpf_shell_host_feature_matching_tool.png`.

## Boundary

This proves the mapper responsibility moved without changing the focused Tool View contract. It does not requalify feature-template identity, RANSAC thresholds, or inspection semantics.
