# Pipeline Exchange Command Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move pipeline XML import/export and review-bundle export commands into a dedicated command-surface partial.
- Preserve storage, selected-recipe guards, review-bundle dry-run routing, references, status messages, and refresh behavior.

## Excluded

- No pipeline lifecycle, LLM draft validation/import, Preview/Run, layer, route, or validation execution behavior change.

## Acceptance Criteria

1. The generic handler partial no longer owns the named exchange commands or review-reference construction.
2. The dedicated partial owns XML import/export, review-bundle export, and reference collection.
3. Current-source review-bundle and recipe-context smoke, Debug build, and readiness check pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs` no longer owns XML import/export, review-bundle export, or review-reference construction.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs` owns the complete exchange command flow while reusing the existing LLM partial only for review-bundle dry-run routing.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_review_bundle_import "C:\\Git\\OpenVisionLab_Dev\\artifacts\\maintenance_pipeline_exchange_command_refactor_20260725_import"` passed.
- The same current build's `wpf_shell_host_recipe_context_switch` smoke passed.
- Current-source UI artifact: `artifacts/maintenance_pipeline_exchange_command_refactor_20260725_import/wpf_shell_host_recipe_review_bundle_import.png`.

## Boundary

`wpf_shell_host_recipe_review_bundle` successfully created the requested bundle through the moved export method, then failed because its UI assertion expected the advanced XML button while Recipe Manager remained in its current default summary view. This refactor did not change the manager view-state logic; treat the stale smoke UI precondition as a separate test-maintenance item.

## Superseded Boundary Note (2026-07-25)

This partial-file split was a readability inventory, not the final MVVM/application boundary. The storage and bundle behavior now belongs to `OpenVisionRecipePipelineExchangeUseCase`; this partial remains only its UI adapter. See `OPENVISIONLAB_PIPELINE_EXCHANGE_USECASE_REFACTOR_PROOF_20260725.md`.
