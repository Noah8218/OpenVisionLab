# Run History Presenter Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Move recent-run three-item projection, previous-selection retention, baseline option/automatic baseline selection, and default batch/pair sample-result selection into the existing `OpenVisionRecipeRunHistoryPresenter`.
- Keep `OpenVisionShellHostRecipeCommandSurface.RunHistory.cs` as the adapter that reads persisted run candidates and assigns the presenter projection to bindable UI properties.

## Excluded

- No batch execution, report persistence, drawing evidence, performance calculation, Preview/Run, layer, route, validation, or LLM workflow change.

## Acceptance Criteria

1. The Presenter owns the named run-history selection policy and returns explicit options/selection state.
2. The command partial no longer directly applies the three-run limit, empty placeholder, retained-option matching, automatic baseline, or default sample-result ordering.
3. Current-source local-validation smoke, Debug build, and readiness checks pass.

## Evidence

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeRunHistoryPresenter.cs` now owns `BuildRecentRunSelection`, `BuildBaselineRunSelection`, `SelectDefaultBatchSampleResult`, and `SelectDefaultPairSampleResult`. `OpenVisionRecipeRunHistorySelection` explicitly carries options and the selected option.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RunHistory.cs` only reads `VisionPipelineBatchRunSummaryStorage` and projects the Presenter result; the root command surface delegates default-result selection to the Presenter.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors after correcting a compile-only list null-coalescing type mismatch in the Presenter.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_local_validation_set artifacts\\mvvm_run_history_presenter_20260725` passed (`OK`, 1600x900).
- `dotnet run --no-build --project "tools\\OpenVisionReadinessCheck\\OpenVisionReadinessCheck.csproj" -c Debug` passed every readiness contract.
- Current-source UI artifact: `artifacts/mvvm_run_history_presenter_20260725/wpf_shell_host_recipe_local_validation_set.png`.

## Boundary

This is a presentation-state boundary in the existing Recipe/Review presenter. The Presenter remains read-only over supplied run options; storage lookup and WPF property assignment deliberately remain in the command-surface adapter. No new repository abstraction was added.
