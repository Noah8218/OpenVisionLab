# Recipe Workspace Command Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move recipe creation, named creation, duplication, rename, deletion, command enablement, and the post-create workspace switch into a dedicated command-surface partial.
- Preserve workspace storage calls, confirmation gates, option refresh, and existing status messages.

## Excluded

- No XML import/export, Pipeline CRUD, validation execution, LLM workflow, Preview/Run, layer, or route behavior change.

## Acceptance Criteria

1. The generic handler partial no longer owns recipe-workspace lifecycle methods.
2. The dedicated partial owns the complete lifecycle flow and creation helper.
3. Current-source Recipe Manager smoke, Debug build, and readiness check pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs` no longer owns the recipe creation, duplication, rename, deletion, or post-create switch methods.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs` owns that complete lifecycle command flow.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_manager_summary "C:\\Git\\OpenVisionLab_Dev\\artifacts\\maintenance_recipe_workspace_command_refactor_20260725"` passed.
- `dotnet run --no-build --project "tools\\OpenVisionReadinessCheck\\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\\Git\\OpenVisionLab_Dev"` passed after the new partial was added to the explicit WPF shell composition allowlist.
- Current-source UI artifact: `artifacts/maintenance_recipe_workspace_command_refactor_20260725/wpf_shell_host_recipe_manager_summary.png`.

## Boundary

This proves the recipe-workspace command responsibility moved without changing the focused Recipe Manager contract. It does not requalify recipe XML import/export, Pipeline CRUD, validation execution, or inspection semantics.

## Superseded Boundary Note (2026-07-25)

This partial-file split was a readability inventory, not the final MVVM/application boundary. Workspace CRUD and default-pipeline preparation now belong to `OpenVisionRecipeWorkspaceUseCase`; this partial remains only its UI adapter. See `OPENVISIONLAB_RECIPE_WORKSPACE_USECASE_REFACTOR_PROOF_20260725.md`.
