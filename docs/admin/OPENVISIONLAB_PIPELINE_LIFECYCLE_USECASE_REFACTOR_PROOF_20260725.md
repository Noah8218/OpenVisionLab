# Pipeline Lifecycle UseCase Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Extract pipeline activation, duplicate-name selection, duplicate, rename, delete/fallback resolution, and sample-pipeline import/activation into `OpenVisionRecipePipelineLifecycleUseCase`.
- Keep `OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs` as the UI adapter for command guards, delete confirmation, active-state-dependent refresh, selected sample access, UI status projection, and UI refresh.

## Excluded

- No pipeline XML exchange/review bundle, LLM draft workflow, Preview/Run, layer, route, validation execution, drawing, or report behavior change.
- The shared legacy `CreateUniquePipelineName` helper remains in the command surface for LLM draft workflows; Pipeline Lifecycle no longer calls it.

## Acceptance Criteria

1. One non-WPF owner accepts primitive recipe/pipeline/sample inputs and returns explicit success, resulting pipeline name, and detail values.
2. The Pipeline Lifecycle command adapter no longer directly activates, duplicates, renames, deletes, loads, or saves a pipeline.
3. Current-source recipe-context smoke, Debug build, and readiness checks pass.

## Evidence

- `UI/Menu/Wpf/Recipe/OpenVisionRecipePipelineLifecycleUseCase.cs` owns `Activate`, `Duplicate`, `Rename`, `Delete`, and `DuplicateFromSample`; `OpenVisionRecipePipelineLifecycleResult` carries the outcome.
- Structural search found `VisionPipelineStorage.SaveActivePipelineName`, `TryDuplicatePipeline`, `TryRenamePipeline`, `TryDeletePipeline`, `TryLoadFromFile`, `Save`, and lifecycle unique-name generation only in the UseCase for this command path.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_context_switch artifacts\\mvvm_pipeline_lifecycle_usecase_20260725` passed (`OK`, 1600x900).
- `dotnet run --no-build --project "tools\\OpenVisionReadinessCheck\\OpenVisionReadinessCheck.csproj" -c Debug` passed every readiness contract.
- Current-source UI artifact: `artifacts/mvvm_pipeline_lifecycle_usecase_20260725/wpf_shell_host_recipe_context_switch.png`.

## Boundary

This is an application-service boundary inside the WPF Recipe area, not a new domain/Core layer. It deliberately reuses the existing static pipeline storage as the persistence seam and adds no interface/factory abstraction. The partial file remains a UI adapter/readability inventory, not the architecture by itself.
