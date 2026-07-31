# Recipe Workspace UseCase Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Extract recipe creation, duplicate-name selection, workspace duplication, rename, deletion, fallback workspace preparation, and new-recipe default-pipeline preparation into `OpenVisionRecipeWorkspaceUseCase`.
- Keep `OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs` as the UI adapter for command enablement, deletion confirmation, selected-option fallback choice, recipe switching, status projection, and UI refresh.

## Excluded

- No change to selected-recipe navigation, pipeline CRUD, XML exchange, validation execution, LLM workflow, Preview/Run, layers, or routes.
- The existing defensive `EnsureVisionWorkspace` call during ordinary recipe selection remains in the selection workflow; it is not recipe CRUD.

## Acceptance Criteria

1. One non-WPF owner accepts recipe names and returns an explicit success/result recipe name for all four CRUD operations.
2. The Recipe Workspace command adapter no longer directly creates, duplicates, renames, deletes, or initializes a recipe workspace/default pipeline.
3. The current-source Recipe Manager smoke, Debug build, and repository readiness checks pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/OpenVisionRecipeWorkspaceUseCase.cs` owns `Create`, `Duplicate`, `Rename`, `Delete`, and unique-name generation. `OpenVisionRecipeWorkspaceResult` carries success and the resulting recipe name.
- Structural search found `RecipeWorkspaceService.EnsureVisionWorkspace`, `DuplicateVisionWorkspace`, `RenameVisionWorkspace`, `DeleteVisionWorkspace`, `VisionPipelineStorage.SaveActivePipelineName`, `VisionPipelineStorage.Load`, and `CreateUniqueRecipeName` only in the UseCase for the Recipe Workspace CRUD path.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_manager_summary artifacts\\mvvm_recipe_workspace_usecase_20260725` passed (`OK`, 1600x900).
- `dotnet run --no-build --project "tools\\OpenVisionReadinessCheck\\OpenVisionReadinessCheck.csproj" -c Debug` passed every readiness contract.
- Current-source UI artifact: `artifacts/mvvm_recipe_workspace_usecase_20260725/wpf_shell_host_recipe_manager_summary.png`.

## Boundary

This is an application-service boundary inside the WPF Recipe area, not a new domain/Core layer. It deliberately reuses the existing static workspace/storage services as the persistence seam and does not introduce a speculative abstraction. The earlier partial split remains a UI adapter/readability inventory, not the completed architecture by itself.
