# OpenVisionLab Source Ownership Refactor Proof (2026-07-17)

## User Goal

Make source folders communicate ownership and reduce large WPF host coupling without changing the explicit rule-based inspection workflow.

## Structural Changes Confirmed

- Before: `1. Core`, `0. UI\6) Vision Test\Wpf`, and `0. UI\0) MENU\Wpf` contained broad flat source groups; `OpenVisionShellHostRecipeCommandSurface` owned recipe DTOs, LLM prompt text, XML validation rules, dependency scanning/copying, and correction-packet text.
- After: Core, Tool View, and owner-ready MENU sources have explicit folder owners. The Core and Tool View roots have no direct C#/XAML source files, and MENU WPF retains only the intentional Shell composition boundary (`OpenVisionShellHostView.xaml`, code-behind, and recipe command surface).
- Evidence: `OpenVisionReadinessCheck` enforces these exact roots and rejects restored Host declarations for the extracted responsibilities.

## Call Path

- Old LLM path: Host -> local prompt/validation/dependency/correction-packet methods.
- New LLM path: Host -> `Recipe\IntentSkills\OpenVisionRecipeLlmPromptBuilder` or `OpenVisionRecipeLlmReviewBundleBuilder`; Host -> `Recipe\Validation\OpenVisionRecipeLlmDraftValidationService` -> `OpenVisionRecipeLlmDraftValidationRules` + `Recipe\Review\OpenVisionRecipeDependencyReviewService`.
- Review bundle exporter/inspector now call `OpenVisionRecipeDependencyReviewService` for path classification and resolution instead of Host static helpers.
- Pipeline Review execution now flows through `PipelineReview\Execution\OpenVisionPipelineReviewExecutionController`: the document owns View event wiring and selected-Step presentation, while the controller owns the explicit runner call, display-layer execution context, review-only Step/result-image caches, and result-image disposal.

- Recipe operator review now flows through Recipe\Review\OpenVisionRecipeRunReviewPresenter. The Host supplies selected recipe/sample/pair/history state and command wiring; the presenter formats the operator summary, role suffix, saved-run review, and ordered next action.

## Responsibility Split

- `Recipe\Models`: recipe, validation, sample-run, and batch DTOs plus localized recipe text.
- `Recipe\IntentSkills`: deterministic recipe starters, LLM prompt/intent contracts, and correction-packet construction.
- `Recipe\Validation`: pure XML syntax/result-channel/Intent rules and request/result XML draft orchestration.
- `Recipe\Review`: review bundle export/inspection, dependency scan/copy execution, and pure LLM draft/variant comparison, selected-step/branch-output review, Good/Bad sample-matrix presentation, local validation-set/dashboard and Validation Suite summary presentation, run-review, Run History filter/baseline/comparison/performance presentation, operator decision-board, and handoff presentation.
- The Host retains command availability, selected state, and UI property updates. It does not execute Preview/Run, create layers, or change routing through these paths.

- Recipe\Review owns review bundle export/inspection, dependency scan/copy execution, and pure LLM draft/variant comparison, selected-step/branch-output review, operator run-review/next-action, decision-board, and handoff presentation.

## Dependency and State Flow

- LLM XML validation returns `OpenVisionRecipeLlmDraftValidationResult` with Pipeline, reports, and dependency rows.
- Dependency review returns `OpenVisionRecipeDependencyReviewResult` with scan/copy report, rows, and blocking issue count.
- The Host applies those results to observable UI state; services do not own WPF controls, Preview/Run, active layers, or docking state.
- Pipeline Review controller emits an explicit Step-update event after caching a result. The document receives it on the View dispatcher and refreshes only its flow/selection presentation. Opening, selecting, or refreshing the document still does not run a pipeline.

## Checks Run

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`: passed, 0 warnings, 0 errors.
- Latest `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-llm-intent-skills`: passed under `artifacts\p90_llm_review_bundle_builder_20260717\final_build_exe_recipe_manager_llm_intent_skills`.
- `dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`: passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1`: passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1`: passed (`30` catalog rows, `229` manifest assets, `15` pipelines).
- `git diff --check`: passed; existing CRLF normalization warnings only.

## Pipeline Review Execution Proof

- `OpenVisionPipelineReviewDocument` no longer contains `VisionPipelineExecutionService.RunAsync`, display-layer execution-context construction, Step-result dictionaries, or review output-image dictionaries.
- `OpenVisionPipelineReviewExecutionController` owns those runtime concerns and returns `OpenVisionPipelineReviewExecutionResult`; `OpenVisionPipelineReviewStepUpdatedEventArgs` is the explicit UI-update contract.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-pipeline-roundtrip` passed under `artifacts\p91_pipeline_review_execution_controller_20260717\final_build_exe_recipe_pipeline_roundtrip` with `WAIT`, explicit Run Review, `NativePreviewRuns: 0`, and `LayerCount: 1` retained.
- Current-source WPF smoke passed: `wpf_shell_host_pipeline_review` and `wpf_shell_host_pipeline_review_input_state`, both with `layout=0`, `text=0`, and `internal=0`.

## Recipe Run Review Presentation Proof

- OpenVisionShellHostRecipeCommandSurface no longer declares the operator summary, selected-role suffix, saved batch-run review, or ordered next-action policy methods. It still owns selected-state lookup, commands, and clipboard copy state.
- OpenVisionRecipeRunReviewPresenter owns text derivation from recipe/sample/pair/history DTOs. The linked failed Step is supplied by the Host, so the presenter does not query selection state or navigate the UI.
- Latest actual EXE: bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs passed under artifacts\p92_recipe_run_review_presenter_20260717\final_build_exe_recipe_manager_tabs. The report confirms role drill-down, failed-run Step linking, selected-run review/copy, Guided Setup next-action flow, and Preview/Run-free state changes.
- OpenVisionReadinessCheck rejects restoration of the four Host text/policy methods and requires the dedicated Recipe\Review presenter methods.

## Recipe Operator Decision Presentation Proof

- OpenVisionShellHostRecipeCommandSurface now resolves only selected DTOs and the evidence/handoff Step. It delegates XML, sample, Good/Bad, final-status, metric-evidence, validation-row, result-channel, and handoff-report text to `OpenVisionRecipeOperatorDecisionPresenter`.
- `OpenVisionRecipeOperatorDecisionRequest` and `OpenVisionRecipeOperatorDecisionPresentation` make the boundary explicit: no presenter code accesses Host selection, WPF controls, Preview/Run, layers, routing, or recipe mutation.
- OpenVisionReadinessCheck rejects restoration of all nine former decision-board/report composition methods in the Host and requires the dedicated request/result/presenter contracts under `Recipe\Review`.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p93_recipe_operator_decision_presenter_20260717\final_build_exe_recipe_manager_tabs`. The report confirms operator decision summary, Good/Bad Role drill-down, failed-Step link, run-review copy, and Preview/Run-free workflow behavior.

## Recipe Pipeline Comparison Presentation Proof

- The Host now resolves the active and selected pipelines only. `OpenVisionRecipePipelineComparisonPresenter` derives LLM draft-import review, LLM XML diff, active/selected variant comparison, step/parameter deltas, and dependency-path deltas from supplied pipelines.
- The presenter has no recipe storage, Host selection, WPF, Preview/Run, layer, routing, XML mutation, or recipe mutation dependency. The Host preserves the existing read-only variant-selection contract and delegates display formatting.
- OpenVisionReadinessCheck rejects restoration of the Host pipeline-diff helpers and requires the dedicated Recipe\Review presenter methods.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p94_recipe_pipeline_comparison_presenter_20260717\final_build_exe_recipe_manager_tabs`. The report confirms `PipelineVariantComparison: active/selected diff visible without Preview/Run`, `LlmXmlDiff: visible`, and existing Recipe Manager execution contracts.

## Recipe Pipeline Step Review Presentation Proof

- `OpenVisionRecipePipelineStepReviewPresenter` now derives selected-step context, failure guidance, corrected-output guidance/evidence, Step-flow text, branch/output summary and rows, and step-slot labels from supplied DTO state.
- The Host retains selection changes, PropertyGrid/XML apply, tool navigation, and layer navigation. The presenter has no WPF, Preview/Run, layer mutation, routing mutation, storage, or recipe mutation dependency.
- OpenVisionReadinessCheck rejects restoration of the eight Host step-review composition methods and requires the dedicated `Recipe\Review` presenter methods.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p95_recipe_pipeline_step_review_presenter_20260717\final_build_exe_recipe_manager_tabs`. The report confirms `FailedRunLink`, `CorrectedOutputReview`, `StepComparisonGrid`, `BranchOutputComparison`, and explicit Preview/Run-free state transitions.

## Recipe Run History Presentation Proof

- The Host retains persisted run-summary loading, selected run/baseline state, command invalidation, and PropertyChanged coordination. It now supplies already-loaded current/baseline summaries to `OpenVisionRecipeRunHistoryPresenter`.
- `OpenVisionRecipeRunHistoryPresenter` owns the read-only NG filter, NG-cause summary, automatic/selected baseline resolution, sample comparison rows, default comparison-row priority, and correctness/performance comparison text. It does not load persisted summaries, access WPF, execute Preview/Run, change layers/routes, or mutate recipe state.
- The equivalence rule remains unchanged: timing comparison requires matching suite kind, suite name, exact sorted sample-image identity set, and complete timing coverage. Outcome rows remain visible when timing comparison is intentionally skipped.
- OpenVisionReadinessCheck rejects restoration of the former Host Run History presentation helpers, requires the dedicated presenter methods, and rejects direct persisted-summary loading from the presenter.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p96_recipe_run_history_presenter_20260717\final_build_exe_recipe_manager_tabs`. The report confirms NG filtering, selectable baseline changing `Regression` to `Still NG` and back, and average/p95 `+0.3 ms` comparison without Preview/Run side effects.

## Recipe Good/Bad Sample-Matrix Presentation Proof

- The Host retains current sample/pair-run selection and PropertyChanged coordination. It delegates Good/Bad matrix row construction, prior-row preservation/default selection priority, and summary formatting to `OpenVisionRecipeSampleMatrixPresenter`.
- The presenter accepts only the selected catalog sample, latest pair-run DTO, current matrix rows, and the prior row. It has no storage, WPF, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency.
- Existing selection behavior is preserved: keep the same sample when it remains available; otherwise prefer an NG result, then a pending result, then the first row.
- OpenVisionReadinessCheck rejects restoration of the three former Host sample-matrix presentation methods and requires the dedicated presenter methods. It also rejects pipeline execution from the presenter.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p97_recipe_sample_matrix_presenter_20260717\final_build_exe_recipe_manager_tabs`; the report confirms PairRoleCards, RoleDrilldown, failed-Step linking, and explicit Preview/Run-free Recipe Manager state transitions.

## Recipe Local Validation-Set Dashboard Presentation Proof

- The Host retains validation-set file loading/persistence, XML-based acceptance/calibration evidence, execution state, selected set/image state, commands, and PropertyChanged coordination. It delegates only the already-selected validation-set/dashboard and Validation Suite summary text to `OpenVisionRecipeValidationSetPresenter`.
- The presenter receives DTO state and scalar status only. It owns expected-role counts, selected-set summary, next-action ordering, and the Validation Suite top summary. It has no validation-set storage, pipeline XML load, WPF, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency.
- The split intentionally excludes acceptance/calibration evidence because those paths load the active pipeline XML and evaluate gate/scaling values; they remain with the existing Host/storage boundary pending a distinct request/result audit.
- OpenVisionReadinessCheck rejects restoration of the four Host dashboard methods, requires the dedicated presenter methods, and rejects validation-set persistence, pipeline XML loading, or pipeline execution from the presenter.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p98_recipe_validation_set_presenter_20260717\final_build_exe_recipe_manager_tabs`; the report confirms LocalValidationSet file/folder/repair controls, saved Validation Suite state, and explicit Preview/Run-free behavior.

## Recipe Guided Intent Feedback Presentation Proof

- The Host retains Guided Setup input fields, latest sample selection, Starter XML commands, stale-draft state, and PropertyChanged coordination. It now supplies the selected sample summary, Pin-gap unit mode, and current gate text to `OpenVisionRecipeIntentFeedbackPresenter`.
- `OpenVisionRecipeIntentFeedbackPresenter` owns read-only Pin-gap latest-run/calibration feedback, Blob `ResultCount` feedback, and Contour `ResultCount`/`AreaMax` feedback. It has no WPF, Preview/Run, layer/routing mutation, pipeline XML load, sample-check execution, run-history persistence, XML mutation, or recipe mutation dependency.
- Existing feedback behavior remains explicit: Pin-gap warns when average-only evidence lacks a range/outlier gate; Blob and Contour show the current threshold/ROI/area tuning axis only after an explicit sample result exists.
- OpenVisionReadinessCheck rejects restoration of the seven former Host feedback/advice methods, requires all four Presenter entry points, and rejects sample execution, pipeline XML loading, or run-history persistence from the Presenter.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p99_recipe_intent_feedback_presenter_20260717\final_build_exe_recipe_manager_tabs_final`; the report confirms Pin-gap `DistanceMmAvg`/`DistanceMmRange`, Blob `ResultCount`, Contour `ResultCount`/`AreaMax`, and existing explicit Preview/Run-free Recipe Manager behavior.

## Recipe Guided Setup Readiness Presentation Proof

- The Host retains Guided Setup field state, PropertyChanged/command invalidation, Starter XML commands, stale-draft state, recipe selection, and all XML/layer/Preview/Run paths. It now maps the current field values into `OpenVisionRecipeGuidedSetupReadinessInput` and delegates only readiness guidance and `READY`/`MISSING` formatting.
- `OpenVisionRecipeGuidedSetupReadinessPresenter` belongs in `Recipe\IntentSkills` because it applies deterministic intent contracts to the current read-only input DTO. It covers LineDistance, Blob, Contour, EdgeBasedMatching, FeatureMatching, Matching, and Mean required inputs without creating a pipeline or changing any recipe state.
- The presenter may read whether a selected template image exists, but it has no WPF, Preview/Run, layer/routing mutation, pipeline XML load, sample-check execution, run-history persistence, Starter XML creation, XML mutation, or recipe mutation dependency.
- OpenVisionReadinessCheck rejects restoration of the two former Host methods, requires Host delegation and the explicit input DTO/Presenter entry points, and rejects execution, storage, XML load, and starter-pipeline creation from the Presenter.
- Latest actual EXE: `bin\Debug\OpenVisionLab.exe --smoke recipe-manager-tabs` passed under `artifacts\p100_guided_setup_readiness_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; the report confirms all seven Guided Setup starters, both Pin-gap unit modes, and explicit Preview/Run-free behavior.

## Recipe Guided Workflow Presentation Proof

- Audited the Guided Setup strip and its ordered next-action chain before editing. The Host had duplicate ordered conditions: one path chose the displayed instruction and another returned the matching command delegate. The conditions consume current recipe/sample/pair state and existing command availability only, so the decision policy is a complete read-only presentation boundary.
- Added `Recipe\Review\OpenVisionRecipeGuidedWorkflowPresenter` with an explicit request DTO and action enum. It owns Guided Setup strip text, the single ordered action decision, and the matching label. The Host maps selected state and `Can...` flags into the request, then retains the explicit command switch that invokes the existing operation.
- No execution behavior changed. The presenter has no WPF, pipeline storage, XML mutation, recipe mutation, layer/routing mutation, or Preview/Run dependency. The Host remains the sole owner of command execution, so selecting/opening Recipe Manager is still read-only and Preview/Run remains an explicit action.
- OpenVisionReadinessCheck rejects restoration of the three former Host methods, requires Host delegation and the dedicated `Recipe\Review` presenter, and rejects pipeline execution, storage loading, pipeline creation, or clipboard access from the presenter. `P101_SOURCE_OWNERSHIP` passed.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:14:35 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p101_guided_workflow_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; the report confirms Guided Setup starters, run-review/history links, operator decision summary, failed-Step navigation, and explicit Preview/Run-free behavior.

## Recipe Lifecycle Validation Presentation Proof

- Audited Recipe Manager name guidance before editing. `BuildRecipeEditValidationText` and `BuildPipelineEditValidationText` only classify selected names, requested names, existing-name collisions, and normalized pipeline-name feedback. Workspace/pipeline create, duplicate, rename, delete, selection refresh, and command execution remain separate Host responsibilities.
- Added `Recipe\Review\OpenVisionRecipeLifecycleValidationPresenter` with explicit recipe-edit and pipeline-edit request contracts. The Host supplies selected names, current lists, and the existing normalized pipeline name; the presenter owns only localized read-only validation guidance.
- Exact branch order is preserved, including blank recipe/pipeline names, no selected recipe/pipeline, invalid names, pipeline invalid-character normalization, selected-name feedback, duplicate names, and available-name guidance. A separate selected-pipeline-object flag preserves the former null-option branch even for malformed option data.
- The presenter may call the existing Core name-validity rule, but it has no WPF, workspace creation/duplication/rename/deletion, pipeline storage, XML mutation, recipe mutation, layer/routing mutation, or Preview/Run dependency. Host command methods remain the only lifecycle mutators.
- OpenVisionReadinessCheck rejects restoration of both Host text methods, requires the `Recipe\Review` request/presenter contracts, and rejects workspace mutation, pipeline storage, or pipeline execution from the presenter. `P102_SOURCE_OWNERSHIP` passed.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:24:07 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p102_recipe_lifecycle_validation_presenter_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; it retained Recipe Manager summary/advanced modes, lifecycle commands, Guided Setup, review/history, and explicit Preview/Run behavior.

## Stored Pipeline XML Validation Report Proof

- Audited `BuildLlmXmlValidationReport` before editing. Although its learner-facing label is LLM XML validation, the method receives an already-loaded selected pipeline and XML load result, then formats schema/routing evidence, name/path mismatch warnings, and bounded error/warning rows. It has no Host state, WPF, persistence, or execution responsibility.
- Added `Recipe\Validation\OpenVisionRecipeStoredPipelineValidationReportBuilder` with an explicit request contract. The Host retains storage loading and passes the pipeline path, load result, pipeline instance, and load message to the Builder; the Builder owns all report composition and schema/routing validation formatting.
- Existing report text and order are retained: XML status, load status, assumed `Main` layer, missing/invalid XML corrective action, pipeline and Step count, file/name mismatch, schema/routing totals, and first four errors/warnings with remaining counts.
- The Builder has no `VisionPipelineStorage`, WPF, Preview/Run, layer/routing mutation, XML mutation, or recipe mutation dependency. It validates only the supplied in-memory pipeline, so Recipe Manager selection and refresh remain read-only.
- OpenVisionReadinessCheck rejects restoration of the Host method, requires the `Recipe\Validation` Builder/request contract and `VisionPipelineValidator.Validate`, and rejects storage, execution, or WPF APIs in the Builder. `P103_SOURCE_OWNERSHIP` passed.
- Fresh solution build passed with 0 warnings/0 errors. Latest `bin\Debug\OpenVisionLab.exe` built at `2026-07-17 20:30:10 KST` passed `--smoke recipe-manager-tabs` under `artifacts\p103_stored_pipeline_validation_report_builder_20260717\final_build_openvisionlab_exe_recipe_manager_tabs`; the report retained Recipe Manager summary/advanced modes, LLM XML evidence, Guided Setup, review/history, and explicit Preview/Run behavior.

## Remaining Structural Work

- Post-P103 root audit: `1. Core`, `0. UI\0) MENU`, `0. UI\6) Vision Test`, and `0. UI\6) Vision Test\Wpf` have no direct C#/XAML files. `0. UI\0) MENU\Wpf` retains only the approved Host composition boundary: `OpenVisionShellHostRecipeCommandSurface.cs`, `OpenVisionShellHostView.xaml`, and its code-behind.
- Remaining Host work is now genuine integration work: selection/`PropertyChanged` coordination, workspace/pipeline storage and lifecycle commands, explicit sample/validation execution, file/dialog/clipboard callbacks, viewer/layer callbacks, and current-session LLM prompt/review-bundle assembly. Those paths require current Host state or side-effect boundaries.
- Do not extract isolated helpers such as review-reference mapping, catalog-message formatting, or ROI suggestion lookup merely to shorten the Host. They do not form a complete independently testable owner apart from their command/session flows.
- Future source work requires a new complete Presenter, Controller, or request/result service identified by a call-path audit. Do not move the Host XAML/code-behind until an actual responsibility boundary is proven. Return to product evidence or current-build UX work before another mechanical refactor.
