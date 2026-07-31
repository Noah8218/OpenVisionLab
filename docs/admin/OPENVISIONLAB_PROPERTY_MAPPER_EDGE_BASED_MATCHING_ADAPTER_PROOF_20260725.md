# EdgeBasedMatching Property Mapper Adapter Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move the EdgeBasedMatching/EdgeBasedTemplateMatching/EdgeTemplateMatching PropertyGrid adapter from the root mapper into a dedicated partial.
- Preserve all existing default values, XML parameter names, acceptance metadata, and the opt-in unique-match/Top-K contract.

## Excluded

- No matcher/runtime algorithm change.
- No unique-match threshold, candidate-count, Preview/Run, layer, or routing behavior change.

## Acceptance Criteria

1. The root mapper retains only the EdgeBasedMatching tool-family dispatch.
2. The dedicated partial owns the PropertyGrid model and parameter/default mapping.
3. Current-source EdgeBasedMatching Tool View smoke, Debug build, and readiness check pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs` now delegates only the three EdgeBasedMatching aliases to `CreateEdgeBasedMatchingProperty`.
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.EdgeBasedMatching.cs` owns the adapter defaults, PropertyGrid model, step metadata, and acceptance metadata.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_edge_based_matching_tool "C:\\Git\\OpenVisionLab_Dev\\artifacts\\maintenance_property_mapper_edge_based_matching_adapter_20260725"` passed.
- Current-source UI artifact: `artifacts/maintenance_property_mapper_edge_based_matching_adapter_20260725/wpf_shell_host_edge_based_matching_tool.png`.

## Boundary

This proves the mapper responsibility moved without changing the focused Tool View contract. It does not requalify EdgeBasedMatching template identity, unique-match thresholds, or inspection semantics.
