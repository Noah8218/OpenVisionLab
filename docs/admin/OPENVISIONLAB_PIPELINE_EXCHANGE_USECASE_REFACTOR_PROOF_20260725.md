# Pipeline Exchange UseCase Refactor Proof (2026-07-25)

## Status

Complete.

## Scope

- Extract pipeline XML import, XML export, review-bundle construction, and pipeline XML serialization from the recipe command surface into `OpenVisionRecipePipelineExchangeUseCase`.
- Keep `OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs` as the UI adapter for file-dialog results, selected-recipe guards, UI refresh/status projection, review-bundle dry-run routing, and selected UI reference collection.

## Excluded

- No change to pipeline lifecycle, LLM draft validation/import, Preview/Run, layers, routes, or validation execution.
- Review-bundle dry-run remains in the existing LLM workflow because it coordinates that UI-only review state.

## Acceptance Criteria

1. The exchange behavior has one non-WPF owner with explicit input and result types.
2. The command surface no longer directly loads/saves pipeline files, creates review bundles, or serializes a pipeline for that export path.
3. Existing review-bundle import behavior, Debug build, and repository readiness checks pass from the current source.

## Evidence

- `UI/Menu/Wpf/Recipe/Review/OpenVisionRecipePipelineExchangeUseCase.cs` owns `Import`, `Export`, and `ExportReviewBundle`; each returns `OpenVisionRecipePipelineExchangeResult` with explicit success, selected pipeline name, and detail values.
- `UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs` contains only the selected-recipe/UI workflow adapter and calls `pipelineExchangeUseCase`; structural search found `VisionPipelineStorage.TryLoadFromFile`, `VisionPipelineStorage.TrySaveToFile`, `VisionPipelineStorage.Save`, `VisionPipelineStorage.SaveActivePipelineName`, `OpenVisionRecipeReviewBundleExporter`, and `SerializePipelineToXmlText` only in the UseCase file.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project "tools\\PipelineViewerScreenshotSmoke\\PipelineViewerScreenshotSmoke.csproj" -c Debug -- --target wpf_shell_host_recipe_review_bundle_import artifacts\\mvvm_pipeline_exchange_usecase_20260725` passed (`OK`, 1600x900).
- `dotnet run --no-build --project "tools\\OpenVisionReadinessCheck\\OpenVisionReadinessCheck.csproj" -c Debug` passed every readiness contract.
- Current-source UI artifact: `artifacts/mvvm_pipeline_exchange_usecase_20260725/wpf_shell_host_recipe_review_bundle_import.png`.

## Boundary

This is an application-service boundary inside the WPF Recipe/Review area, not a new domain/Core layer. It deliberately keeps direct storage services as its small persistence seam; a later test can substitute that seam only if a concrete storage-behavior need requires it. The existing partial file is retained as a UI adapter, not presented as the architectural boundary.
