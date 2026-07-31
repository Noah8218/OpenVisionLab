# Run History Command Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move recent batch-history refresh, baseline selection, and default sample-result selection into a dedicated command-surface partial.
- Preserve the three-run history limit, previous-selection retention, automatic baseline choice, and review-queue/NG selection order.

## Excluded

- No execution, report persistence, drawing evidence, Preview/Run, layer, route, validation, or LLM workflow change.

## Acceptance Criteria

1. The generic handler partial no longer owns the named run-history selection methods.
2. The dedicated partial owns complete history/baseline/default-selection logic.
3. Current-source local-validation smoke, Debug build, and readiness check pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs` no longer owns recent batch-history refresh, baseline selection, or default-result selection.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RunHistory.cs` owns the complete history/baseline/default-selection flow.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_local_validation_set "C:\\Git\\OpenVisionLab_Dev\\artifacts\\maintenance_run_history_command_refactor_20260725"` passed.
- Current-source UI artifact: `artifacts/maintenance_run_history_command_refactor_20260725/wpf_shell_host_recipe_local_validation_set.png`.

## Boundary

This proves the history-selection responsibility moved without changing execution, persisted reports, drawings, Preview/Run, layers, routing, validation semantics, or LLM workflows.

## Superseded Boundary Note (2026-07-25)

This partial-file split was a readability inventory, not the final presentation boundary. Run-history projection and default-selection policy now belong to `OpenVisionRecipeRunHistoryPresenter`; this partial remains only its storage/UI adapter. See `OPENVISIONLAB_RUN_HISTORY_PRESENTER_REFACTOR_PROOF_20260725.md`.
