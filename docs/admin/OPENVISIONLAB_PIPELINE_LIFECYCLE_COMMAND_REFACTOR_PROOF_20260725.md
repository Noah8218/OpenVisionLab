# Pipeline Lifecycle Command Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move pipeline activation, duplication, rename, deletion, and sample-pipeline duplication into a dedicated command-surface partial.
- Preserve storage calls, deletion confirmation, selected-pipeline refresh, and active-pipeline switch behavior.

## Excluded

- No XML import/export, review-bundle export, Preview/Run, layer, route, validation execution, or LLM workflow change.

## Acceptance Criteria

1. The generic handler partial no longer owns the named pipeline lifecycle methods.
2. The dedicated partial owns the complete selected/sample pipeline lifecycle flow.
3. Current-source recipe-context smoke, Debug build, and readiness check pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs` no longer owns pipeline activation, duplication, rename, deletion, or sample-pipeline duplication.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs` owns that complete selected/sample pipeline lifecycle flow.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_context_switch "C:\\Git\\OpenVisionLab_Dev\\artifacts\\maintenance_pipeline_lifecycle_command_refactor_20260725_context"` passed.
- Current-source UI artifact: `artifacts/maintenance_pipeline_lifecycle_command_refactor_20260725_context/wpf_shell_host_recipe_context_switch.png`.

## Boundary

The broader `wpf_shell_host_recipe_language_controls` scenario reached its recipe and pipeline lifecycle assertions, then failed twice in its later frozen-LLM dependency-report Korean token assertion. The active pipeline was `LLM_Draft_Manager`, dependency copying reported one detected/one copied/zero missing file, and XML validation was OK. This refactor did not change that LLM workflow; treat the failing LLM-specific assertion as a separate maintenance regression to diagnose before relying on that broad target as a full-suite gate.

## Superseded Boundary Note (2026-07-25)

This partial-file split was a readability inventory, not the final MVVM/application boundary. Pipeline lifecycle state changes now belong to `OpenVisionRecipePipelineLifecycleUseCase`; this partial remains only its UI adapter. See `OPENVISIONLAB_PIPELINE_LIFECYCLE_USECASE_REFACTOR_PROOF_20260725.md`.
