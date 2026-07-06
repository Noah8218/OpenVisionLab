# OpenVisionLab Next Session Handoff

Updated: 2026-07-06 15:04 KST

This document is the minimum handoff needed to continue without re-discovering the current state. Work starts in `C:\Git\OpenVisionLab_Dev`; only reviewed and stabilized changes are imported into the original repo at `C:\Git\OpenVisionLab`. Do not run `git push` unless the user explicitly requests `PUSH`.

## Read First

- Product target, final program shape, main view architecture, stable areas that should not be rediscovered, and current development priorities are summarized in `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`.
- Use that document as the first orientation source for future sessions before starting UI/Recipe/LLM/sample work.
- If this handoff conflicts with `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`, prefer the newer target/main-view document and then verify against source, tests, and screenshots.

## Product Direction

- OpenVisionLab is an OpenCvSharp4-based rule-based vision workbench.
- Its purpose is image-based algorithm learning, verification, LLM-assisted XML recipe generation, and recipe composition.
- It is not a camera, lighting, PLC, or I/O integration platform.
- Algorithm tools must stay PropertyGrid-based.
- Preview/Run must be explicit user actions. Layer create/delete/load-image, visibility toggles, and output layer creation must not auto-run tools.
- Viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, and docking features must be preserved.
- Main window title-bar minimize, maximize/restore, and close controls must remain visible and verified. These are window controls, not account/session UI.

## Current Dev Baseline On 2026-07-05

Recent Dev commits on `codex/public-sample-ux-docs` include:

- `487106f Show selected recipe step details`
- `646dce5 Add recipe step comparison grid`
- `8bea861a Explain failed recipe history samples`
- `3d5767ec Show recipe step parameter previews`
- `c1a16bb5 Split recipe review panel into tabs`
- `7e4cd81 Document OpenVisionLab target views`
- `e76a440 Show LLM XML validation issue rows`
- `53fbfc3d Add selected step layer navigation`
- `eeb47e69 Show Good Bad pair role cards`

Current Recipe Manager baseline:

- Searchable recipe list, create, duplicate, rename, delete, XML import/export are already present.
- Recipe Manager is now a workbench-sized overlay with a dedicated recipe library pane, review workspace header, and command strip. It is no longer treated as a small floating settings panel.
- Pipeline review is split into Review, Runs, and XML/Step sub-tabs.
- `Duplicate from sample`, LLM XML validation report, structured LLM XML validation issue rows, LLM XML before/after diff review, actionable dependency/path scan hints and dependency path drill-down rows, pipeline preview step list, Step comparison table, selected Step detail panel, selected Step input/output layer thumbnail cards with click navigation, selected Step ROI/template metadata, selected Step PropertyGrid parameter review with explicit XML apply-back and corrected-output review, branch/output comparison rows for selected multi-step correction paths, Good/Bad role result cards with failed-Step drill-down, failed Step rerun/comparison action strip, and failed-history explanation are already present.
- Top account/operator chrome has been reviewed and removed from Shell Host/Shell Preview. It was only an `Account` icon plus `OperatorText`, with no login/profile/permission command behind it. Keep operator review wording inside Recipe/Pipeline Manager, but do not bring back top-level account UI unless real account/session features are intentionally added.
- Do not re-spend the next session re-evaluating these from scratch unless a regression is reported.

Current priority order:

1. Continue Tool View code-behind cleanup only where established controller/presenter/base patterns fit; current test hooks and preview command paths are in use and should not be removed just to reduce line count. The double-input Arithmetic shell, Blob/Contour single-input PropertyGrid shell, and Matching-family single-input PropertyGrid shell now have shared bases, so do not recreate those extractions.
2. Continue Recipe Manager density polish only when screenshots show actual clipping, overlap, or workflow friction.
3. Continue Pipeline/Recipe operator review UX only when a real workflow gap is visible from current EXE evidence.

Latest UI evidence for corrected-output review after Step XML apply:

- Before: `artifacts\corrected_output_review_before_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- After: `artifacts\corrected_output_review_after_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\corrected_output_review_after_20260705_r1\report.txt` with `Result: PASS`, `CorrectedOutputReview: visible after XML apply`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: `HostRecipeCorrectedOutputReviewPanel` appears under the embedded Step PropertyGrid status. It reuses existing explicit output navigation and Good/Bad rerun commands; XML apply still does not run Preview/Run.

Latest UI evidence for LLM dependency path drill-down rows:

- Before: `artifacts\corrected_output_review_after_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_dependency_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_LlmXml.png`
- Direct EXE smoke: `artifacts\llm_dependency_drilldown_after_20260705_r2\report.txt` with `Result: PASS`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`.
- Structure note: `LlmXmlDraftDependencyRows` exposes row-level status, step, parameter, path, and action. The text dependency report remains for copy/paste review.

Latest UI evidence for Recipe Manager internal workbench density adjustment:

- Before: `artifacts\llm_dependency_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\report.txt` with `Result: PASS`, `CorrectedOutputReview: visible after XML apply`, `LlmDependencyRows: 1`, and `MovedTo: -64.0,18.0`.
- Structure note: Pipeline tab internal management column is narrower so the Step review/PropertyGrid area gets more horizontal room on large workbench screens.

Latest UI evidence for branch/output comparison rows:

- True before note: `artifacts\branch_output_comparison_before_20260705_r1\wpf_shell_host_recipe_language_controls.png` was captured from the current smoke target but Visual Studio was in front, so it is not a clean true-before UI capture. Treat the immediately previous Recipe Manager layout capture as the closest baseline: `artifacts\recipe_manager_workbench_layout_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`.
- After full-window capture: `artifacts\branch_output_comparison_after_20260705_r5_screenshot_smoke\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\branch_output_comparison_after_20260705_r2\report.txt` with `Result: PASS` and `BranchOutputComparison: 2`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\branch_output_comparison_after_20260705_r5_screenshot_smoke` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipeBranchOutputComparisonPanel` shows selected Step, same-input candidates, input producers, and output consumers for the selected multi-step route. Step navigation still does not run Preview/Run.

Latest UI evidence for selected Step operator context:

- Before: `artifacts\operator_step_review_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\operator_step_context_after_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\operator_step_context_after_20260706_r1_direct\report.txt` passed and now checks `PipelineSelectedStepOperatorContextText`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_step_context_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
- Structure note: `HostRecipePipelineSelectedStepOperatorContext` sits inside the selected Step detail panel and summarizes selected Step, route, Good/Bad or run-history failure link, and next action. It is read-only guidance and does not run Preview/Run.

Latest Tool View code-behind candidate review:

- Search evidence: `rg -n "SetTemplatePathForTest|ConfigurePropertyForTest|ApplyPresetForTest|ResultReviewTextForTest|ConsumeThresholdTeachingPreviewRequest" .` shows these hooks are used by native tool document, preview executor, and smoke paths.
- Decision: no code-behind deletion was made in this pass. Removing these forwarding/test hooks would be higher risk than value until a natural controller/base extraction target appears.

Latest Tool View code-behind cleanup for Blob/Contour single-input PropertyGrid base:

- Added `VisionToolSingleInputPropertyToolViewBase` and `IVisionToolSingleInputPropertyToolController` so Blob/Contour no longer duplicate source/destination layer events, preview image command events, selected layer getters, preview setters, status setter, and controller disposal.
- `BlobToolWpfView.xaml` and `ContourToolWpfView.xaml` now use `VisionToolSingleInputPropertyToolViewBase` as the XAML root.
- `BlobToolWpfView.xaml.cs` and `ContourToolWpfView.xaml.cs` now keep only tool-specific presenter setup, threshold teaching preview state, property creation, and area result review.
- Code-behind reduction: repeated forwarding removal reduced Blob and Contour from roughly 160 lines each to roughly 75 lines each.
- UI evidence: `artifacts\tool_view_property_base_smoke_20260705_r1\wpf_shell_host_blob_tool.png` and `artifacts\tool_view_property_base_smoke_20260705_r1\wpf_shell_host_contour_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_blob_tool,wpf_shell_host_contour_tool" -OutputDir "artifacts\tool_view_property_base_smoke_20260705_r1"` passed with `layout=0`, `text=0`, and `internal=0` for both targets.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `git diff --check` passed with CRLF warnings only.

Latest Tool View code-behind cleanup for Matching-family single-input PropertyGrid base:

- Reused `VisionToolSingleInputPropertyToolViewBase` for `MatchingToolWpfView`, `EdgeBasedMatchingToolWpfView`, and `FeatureMatchingToolWpfView`.
- `VisionToolSingleInputMatchingToolController<TProperty>` now implements the same shared controller bridge used by Blob/Contour, so the Matching-family Views no longer duplicate source/destination layer events, preview image command events, selected layer getters, preview setters, status setter, and controller disposal.
- The Matching-family Views still own only tool-specific construction, template/test hooks, property creation, and matching result review.
- Code-behind line counts after cleanup: Matching 58, EdgeBasedMatching 53, FeatureMatching 49.
- Smoke stability note: the screenshot smoke matching template files now use unique temp names instead of a fixed `OpenVisionLab_matching_smoke_template.png`, preventing serial target cleanup/recreate interference during multi-target UI verification.
- UI evidence:
  - Before: `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_matching_tool.png`, `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_edge_based_matching_tool.png`, `artifacts\matching_tool_base_before_20260706_r1\wpf_shell_host_feature_matching_tool.png`.
  - After: `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_matching_tool.png`, `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_edge_based_matching_tool.png`, `artifacts\matching_tool_base_after_20260706_r7\wpf_shell_host_feature_matching_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool,wpf_property_grid_matching_combo" -OutputDir "artifacts\matching_tool_base_after_20260706_r7"` passed with `layout=0`, `text=0`, and `internal=0` for all targets.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\matching_tool_base_after_20260706_r3_route` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Tool View code-behind cleanup for Line single-input special PropertyGrid base:

- Reused `VisionToolSingleInputPropertyToolViewBase` for `LineToolWpfView`.
- `VisionToolSingleInputSpecialPropertyToolController` now implements `IVisionToolSingleInputPropertyToolController`, while keeping the Line-specific input-preview callback path for ROI overlay refresh.
- `LineToolWpfView.xaml` now uses `VisionToolSingleInputPropertyToolViewBase` as the XAML root.
- `LineToolWpfView.xaml.cs` now keeps Line-specific purpose/line selection, ROI editing, preset, result review, and test hooks; repeated source/destination layer events, preview image events, selected layer getters, layer list/output/status setters, and controller disposal moved to the shared base.
- Code-behind reduction: `LineToolWpfView.xaml.cs` went from 323 lines to 263 lines.
- UI evidence:
  - Before: `artifacts\line_tool_base_before_20260706_r1\wpf_shell_host_line_tool.png`.
  - After: `artifacts\line_tool_base_after_20260706_r1\wpf_shell_host_line_tool.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_base_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\line_tool_base_after_20260706_r1_route` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Tool View shared-base stability recheck:

- Rechecked the current Dev build after Matching-family and Line shared-base cleanup.
- Verification: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunUiScreenshotSmoke.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_edge_based_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_line_tool" -OutputDir "artifacts\tool_view_shared_base_recheck_20260706_r1"` passed for all four targets with `layout=0`, `text=0`, and `internal=0`.
- UI evidence:
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_edge_based_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_feature_matching_tool.png`
  - `artifacts\tool_view_shared_base_recheck_20260706_r1\wpf_shell_host_line_tool.png`
- Decision: do not continue deleting Tool View code-behind just for line count. Next Tool View work should start only from a visible bug, duplicated owner path, or already-established controller/base pattern.

Latest Tool View code-behind cleanup for double-input custom tool base:

- Changed `ArithmeticToolWpfView` from direct `UserControl` inheritance to `VisionToolDoubleInputCustomToolViewBase`.
- Added `VisionToolDoubleInputCustomToolViewBase` to own double-input event forwarding, preview-image command forwarding, layer preview setters, status setter, and controller disposal.
- `ArithmeticToolWpfView.xaml.cs` now focuses on arithmetic-specific interaction/settings/text behavior. Code-behind reduced from 276 lines to 172 lines; shared base is 164 lines.
- UI evidence: `artifacts\double_input_custom_tool_base_refactor_20260705_r1\wpf_layer_selection_arithmetic_tool.png`
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\double_input_custom_tool_base_refactor_20260705_r1` passed with `layout=0`, `text=0`, and `internal=0`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_algorithm_output_preview_flow artifacts\double_input_custom_tool_base_refactor_20260705_r1_route` passed with `layout=0`, `text=0`, and `internal=0`.
  - `git diff --check -- "0. UI/6) Vision Test/Wpf/ArithmeticToolWpfView.xaml" "0. UI/6) Vision Test/Wpf/ArithmeticToolWpfView.xaml.cs" "0. UI/6) Vision Test/Wpf/VisionToolDoubleInputCustomToolViewBase.cs"` passed with CRLF warnings only.

Latest UI evidence for main window title-bar controls:

- Before/current check: `artifacts\main_window_chrome_before_20260705_r1\wpf_shell_host_window_chrome.png`
- After: `artifacts\main_window_chrome_after_20260705_r1\wpf_shell_host_window_chrome.png`
- Structure note: `OpenVisionWindowTitleBar` keeps minimize, maximize/restore, and close controls. `OpenVisionWindowTitleBar.xaml` now exposes `OpenVisionWindowMinimizeButton`, `OpenVisionWindowMaximizeRestoreButton`, and `OpenVisionWindowCloseButton` automation IDs.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_window_chrome artifacts\main_window_chrome_after_20260705_r1` passed with `layout=0`, `text=0`, and `internal=0`, and the smoke asserts all three window controls are visible.

Latest UI evidence for Recipe Manager density/status cleanup:

- Before: `artifacts\recipe_manager_density_before_20260705_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_20260705_r3\wpf_shell_host_recipe_language_controls.png`
- Direct EXE smoke: `artifacts\recipe_manager_density_after_20260705_r3_direct\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `BranchOutputComparison: 2`.
- Structure note: `HostRecipeSelectedStepPropertyGridHost` is now visible only after selected Step parameters are explicitly loaded. Changing selected Step clears stale edit status text, so the view no longer shows an old Step 2 XML apply status while Step 1 is selected.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_20260705_r3` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager footer density:

- Before: `artifacts\recipe_manager_density_before_20260706_r1\wpf_shell_host_recipe_language_controls.png`
- After: `artifacts\recipe_manager_density_after_20260706_r2\wpf_shell_host_recipe_language_controls.png`
- Structure note: `HostRecipeManagerNameStrip` and `HostRecipeManagerCommandStrip` now share one compact footer row. The long recipe name editor remains visible while create/duplicate/rename/delete/XML import/export buttons stay inside the 1600x900 workbench viewport.
- Verification: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_density_after_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`.

Latest commercial-comparison guided workflow improvement:

- Commercial comparison basis checked during the 2026-07-06 loop:
  - Cognex In-Sight EasyBuilder emphasizes a step workflow such as image setup, location, inspection, and result/output review.
  - MVTec MERLIC recipe documentation emphasizes recipe files as parameter sets for reusable app variants.
  - NI Vision Builder AI documentation emphasizes state/step review.
  - KEYENCE CV-X simulator material emphasizes PC-side configuration/review and generated operating material.
- Dev scope decision: do not add camera/PLC/runtime integration. Add the useful part only: an in-app guided setup strip in Recipe Manager showing sample readiness, XML validation, Step count, sample run, Good/Bad run, and next action.
- Structure note: `HostRecipeGuidedSetupStrip` is now shown inside the Recipe Manager detail header and bound to `RecipeCommands.RecipeGuidedSetupText`.
- UI evidence: `artifacts\recipe_manager_guided_setup_after_20260706_r2\wpf_shell_host_recipe_language_controls.png`.
- Verification:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_guided_setup_after_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`; the smoke now asserts `HostRecipeGuidedSetupStrip`.

Latest commercial-comparison self-evaluation:

- Official sources were rechecked on 2026-07-06:
  - Cognex EasyBuilder Inspect help and In-Sight Explorer product page: guided inspect/configuration/management workflow.
  - MVTec MERLIC recipe docs: `.mrcp` recipe files, MVApp references, and predefined parameter sets.
  - NI Vision Builder AI pages/readme: configure, benchmark, deploy, camera/image analysis, automation hardware.
  - KEYENCE CV-X product/software pages: camera/lighting/controller ecosystem plus PC simulator/terminal software.
- Product conclusion: OpenVisionLab should not chase camera, lighting, PLC/I/O, controller simulator, deployment runtime, account/session, or production audit features. Its differentiator is local image-based recipe design plus GPT/Gemini/Claude-style LLM XML generation, validation, and explicit OpenCvSharp4 rule-based verification.
- Completion estimate updated in `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`: about 25-30% versus broad commercial equipment platforms by design, and about 62-66% versus the intended LLM-assisted rule-based recipe workbench.
- Next highest-value development target: real LLM XML failure examples and replayable validation scenarios for bad paths, wrong layers, wrong parameters, and unsafe imports. Do this before adding another generic Recipe Manager panel.

Latest LLM XML bad-route validation scenario:

- Added a direct EXE smoke case that creates a valid `VisionPipeline` XML draft with `InputLayer="Missing_Input_Layer"` and verifies validation blocks it as a route/layer error.
- The smoke asserts the malformed-XML case still reports a line/position fix, and the bad-route case reports the missing input layer without marking draft review/diff as ready.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_bad_route_validation_20260706_r1_direct"` passed with `Result: PASS` and `LlmBadRouteValidation: blocked`.

Latest LLM XML unsupported-tool/import-block scenario:

- Added a direct EXE smoke case that creates a valid `VisionPipeline` XML draft with `ToolType="ImaginaryLlmTool"` and verifies validation blocks it as an unsupported tool.
- The smoke then attempts the explicit import command when available and verifies the selected pipeline does not change and validation context is preserved. This covers the unsafe-import failure path without adding auto Preview/Run.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_unsupported_tool_validation_20260706_r1_direct"` passed with `Result: PASS`, `LlmBadRouteValidation: blocked`, and `LlmUnsupportedToolImport: blocked`.

Latest LLM XML failure corpus expansion:

- Missing dependency paths now block validation/import. `BuildDependencyReport(...)` reports the missing count back into LLM XML validation, so an XML draft with a missing template/image path is not importable even when schema/routing is otherwise valid.
- Added replayable direct EXE smoke cases for:
  - missing template dependency path on a Matching draft;
  - invalid parameter values such as `Threshold=bright` and `USE_ROI=sometimes`;
  - missing Arithmetic `InputLayerB` for a two-input operation.
- Each smoke case validates the draft, attempts explicit import when the command is available, and verifies the selected pipeline does not change.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_failure_corpus_20260706_r2_direct"` passed with `Result: PASS`, `LlmMissingDependencyImport: blocked`, `LlmBadParameterImport: blocked`, and `LlmMissingInputBImport: blocked`.

Latest LLM XML correction-loop scenario:

- The LLM review bundle now includes explicit correction rules: return only OpenVisionLab VisionPipeline XML, use `Main` or previous enabled `OutputLayer`, use supported ToolTypes and PropertyGrid-compatible values, fix missing dependency paths before import, and do not add equipment/Preview/Run instructions.
- The same review bundle now includes selected Step operator context and failed-Step review text, so GPT/Gemini/Claude-style correction requests carry the current Step, route, failure link, and next action without adding another UI surface.
- Added a direct EXE smoke path for bad draft -> correction bundle copy -> corrected XML validation -> explicit import.
- The corrected draft uses `Threshold=128` and `USE_ROI=False`, validates OK, imports as a new selected pipeline, and then the smoke restores the previous pipeline selection so the remaining Recipe Manager checks stay stable.
- Direct EXE evidence: `dotnet run --no-build --project OpenVisionLab.csproj -c Debug -- --smoke recipe-manager-tabs artifacts\llm_step_context_bundle_after_20260706_r1_direct` passed with `Result: PASS`, `LlmCorrectionBundle: copied`, and `LlmCorrectedDraftImport: imported`.
- Screenshot smoke evidence: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_step_context_bundle_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest LLM XML tool-parameter compatibility guard:

- `VisionPipelineValidator` now treats 0..1 score/weight parameters as validation errors when LLM output uses percentage-style values. Current guarded keys: `SCORE_MIN`, `GREEDINESS`, and `HYBRID_VERIFY_IMAGE_WEIGHT`.
- Matching/feature scale and tolerance parameters now have basic compatibility guards: `MAGNIFIATION`, `RANSAC_REPROJ_THRESHOLD`, and `COARSE_ANGLE_STEP` must be positive; `FIND_ANGLE_MIN` must not exceed `FIND_ANGLE_MAX`.
- Direct EXE smoke now includes `Direct_LLM_BadScoreRange` with `SCORE_MIN=80` and verifies it cannot import.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_parameter_compat_20260706_r1_direct"` passed with `Result: PASS` and `LlmBadScoreRangeImport: blocked`.

Latest LLM prompt/contract alignment:

- The in-app LLM prompt now tells GPT/Gemini/Claude-style assistants to use score/weight parameters as `0..1` decimals, keep angle min/max ordered, use positive matching/feature tolerance values, and avoid unresolved template/image dependency paths.
- `docs\VISION_PIPELINE_LLM_PROMPT_TEMPLATE.md` and `docs\VISION_PIPELINE_LLM_RECIPE_CONTRACT.md` now match the in-app import path: direct OpenVisionLab import expects complete `VisionPipeline` XML only, not extra prose.
- Direct EXE smoke now asserts the copied LLM prompt includes the new score, angle, and dependency-path rules.
- Direct EXE evidence: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_prompt_contract_20260706_r1_direct"` passed with `Result: PASS`.

Latest LLM XML result-channel contract:

- The in-app LLM prompt, review bundle, validation report, and Recipe Manager Report tab now define the operator result channels: `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction`.
- These are logical outputs derived from XML validation and explicit sample runs. LLM drafts must not emit custom `Inspection.*` XML nodes or parameters.
- Contract docs were updated in `docs\VISION_PIPELINE_LLM_RECIPE_CONTRACT.md`, `docs\VISION_PIPELINE_LLM_PROMPT_TEMPLATE.md`, and `docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`.
- Direct EXE smoke evidence from the implementation step: `artifacts\llm_result_channel_requirements_after_20260706_r2\report.txt` passed with prompt/review/validation checks for `Inspection.Status` and `Inspection.Evidence`.

Latest UI evidence for Recipe Manager result-channel board:

- Before: `artifacts\result_channel_board_before_20260706_r3_direct\OpenVisionLab_RecipeManager_Report.png`
- After: `artifacts\result_channel_board_after_20260706_r3_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: `HostRecipeOperatorResultChannelBoard` now shows compact cards for `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` above the detailed result-channel list in the Recipe Manager Report tab.
- Direct EXE smoke: `artifacts\result_channel_board_after_20260706_r3_direct\report.txt` passed with `Result: PASS`; the smoke asserts the board automation id and the Status/Evidence rows.

Latest LLM XML `Inspection.*` misuse block:

- `Inspection.*` names are now treated as logical review channels only. If an LLM draft emits `Inspection.Status` or another `Inspection.*` name inside XML, validation is NG and import keeps the previous pipeline selection.
- Direct EXE smoke: `artifacts\llm_custom_inspection_block_after_20260706_r4_direct\report.txt` passed with `LlmCustomInspectionImport: blocked`.
- Screenshot smoke: `artifacts\llm_custom_inspection_block_after_20260706_r2\wpf_shell_host_recipe_language_controls.png` passed with `layout=0`, `text=0`, and `internal=0`.

Latest Dev verification checkpoint at 2026-07-06 09:04 KST:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors after the LLM prompt/contract alignment.
- `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\llm_prompt_contract_20260706_r1_direct"` passed with `Result: PASS`, `LlmBadScoreRangeImport: blocked`, `LlmCorrectionBundle: copied`, and `LlmCorrectedDraftImport: imported`.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- `git diff --check` passed with CRLF warnings only.
- Original repo was not touched.

Latest UI evidence for Recipe Manager guided next action:

- Before/current baseline: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\guided_next_action_after_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The Recipe Manager guided setup strip now exposes `HostRecipeGuidedNextActionButton`. It routes one explicit user click to the current next existing action, such as Validate XML, Duplicate from sample, Activate pipeline, Run check, Run Good/Bad, load selected Step parameters, or open the selected Step tool. It does not add automatic Preview/Run.
- Direct EXE smoke: `artifacts\guided_next_action_after_20260706_r2_direct\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and the smoke asserts the guided next action command is enabled during failed-Step review.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\guided_next_action_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it asserts `HostRecipeGuidedNextActionButton`.

Latest UI evidence for Recipe Manager guided next action label:

- Before: `artifacts\guided_next_action_after_20260706_r2_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\guided_next_label_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: `RecipeGuidedNextActionText` now shows the concrete next action instead of a generic "Run next" label. Failed-Step review shows `도구 열기`/`Open tool`; other states can show Validate XML, Duplicate sample, Activate, Run check, Load params, Run Good/Bad, or Complete.
- Direct EXE smoke: `artifacts\guided_next_label_after_20260706_r1_direct\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and the smoke asserts the failed-Step guided action label includes tool/도구.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\guided_next_label_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Run History selected-review copy action:

- Before: `artifacts\run_history_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_RunHistory.png`
- After: `artifacts\run_history_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RunHistory.png`
- Structure note: The Run History tab now exposes `HostRecipeCopySelectedRunReviewButton`. It copies the selected run review text to the clipboard and shows inline status. It does not rerun checks, change layers, import XML, or run Preview.
- Direct EXE smoke: `artifacts\run_history_copy_after_20260706_r2_direct\report.txt` with `Result: PASS`, `SelectedRunReview: linked failed step`, `SelectedRunReviewCopy: copied`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`; the smoke executes the selected run review copy command and checks the success status. The command is enabled only when a saved run with `SummaryPath` is selected.
- Clipboard payload smoke: `artifacts\clipboard_payload_smoke_20260706_r1_direct\report.txt` passed after checking copied clipboard text for operator handoff report, selected run review, LLM prompt, and LLM review bundle.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\run_history_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the selected run review copy command.
- Current-build Recipe Manager recheck: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_current_recheck_20260706_r2` passed with `layout=0`, `text=0`, and `internal=0`. The screenshot smoke now treats selected-run review copy as enabled only when a saved run with `SummaryPath` is selected.
- Direct EXE recheck: `dotnet "C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.dll" --smoke recipe-manager-tabs --output "C:\Git\OpenVisionLab_Dev\artifacts\recipe_manager_current_recheck_20260706_r2_direct"` passed with `Result: PASS` and `SelectedRunReviewCopy: copied`, so the saved-run copy path remains covered.
- Follow-up screenshot recheck after smoke cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\recipe_manager_current_recheck_20260706_r3` passed with `layout=0`, `text=0`, and `internal=0`. Visual inspection of `artifacts\recipe_manager_current_recheck_20260706_r3\wpf_shell_host_recipe_language_controls.png` did not show a new control clipping/overlap issue that justifies another UI change in this loop.

Latest UI evidence for Recipe Manager operator decision board:

- Before: `artifacts\recipe_manager_guided_setup_after_20260706_r3_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- After: `artifacts\operator_review_board_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- Structure note: Review tab now shows `HostRecipeOperatorDecisionBoard` with XML/Step, selected sample, Good/Bad, and next-action cards above the existing long operator review text. It reuses existing sample/pair/pipeline state and does not add Preview/Run triggers.
- Direct EXE smoke: `artifacts\operator_review_board_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_review_board_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager operator handoff report:

- Before: `artifacts\operator_review_board_after_20260706_r1_direct\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- After: `artifacts\operator_report_tab_after_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: Pipeline review now has a `Report` tab (`HostRecipePipelineReportTab`) with `HostRecipeOperatorHandoffReport`. The report summarizes recipe, pipeline, active pipeline, XML/Step status, selected sample result, Good/Bad result, next action, selected role, review Step, route, and first LLM XML validation line.
- Direct EXE smoke: `artifacts\operator_report_tab_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_report_tab_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`.

Latest UI evidence for Recipe Manager operator report copy action:

- Before: `artifacts\operator_report_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- After: `artifacts\operator_report_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_Report.png`
- Structure note: The Pipeline review `Report` tab now exposes `HostRecipeCopyOperatorHandoffReportButton`. It copies the generated operator report to the clipboard and shows an inline success/failure status without running Preview or changing layers.
- Direct EXE smoke: `artifacts\operator_report_copy_after_20260706_r1_direct\report.txt` with `Result: PASS`, `PairRoleCards: 2`, `FailedStepRerunComparison: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`; the smoke now executes the copy command and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\operator_report_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the copy command.

Latest UI evidence for LLM prompt copy action:

- Before: `artifacts\operator_report_copy_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_prompt_copy_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipeCopyLlmPromptButton`. It copies the generated prompt to the clipboard and shows an inline success/failure status. It does not validate/import XML and does not run Preview.
- Direct EXE smoke: `artifacts\llm_prompt_copy_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now builds the prompt, executes the copy command, and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_prompt_copy_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the copy command.

Latest UI evidence for LLM review bundle copy action:

- Before: `artifacts\llm_review_bundle_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_review_bundle_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipeCopyLlmReviewBundleButton`. It copies a correction bundle containing recipe/pipeline context, validation report, dependency report, draft import review, diff review, and current XML draft. It does not validate/import XML and does not run Preview.
- Direct EXE smoke: `artifacts\llm_review_bundle_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now executes the review bundle copy command and checks the success status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_review_bundle_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the review bundle copy command.

Latest UI evidence for LLM XML clipboard paste action:

- Before: `artifacts\llm_clipboard_paste_before_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\OpenVisionLab_RecipeManager_LlmXml.png`
- Structure note: The LLM XML tab now exposes `HostRecipePasteLlmXmlDraftButton`. It pastes clipboard XML text into the draft editor and shows an inline status. It does not validate, import, run Preview, or change layers; the operator still must press Validate and Import explicitly.
- Direct EXE smoke: `artifacts\llm_clipboard_paste_after_20260706_r1_direct\report.txt` with `Result: PASS`, `LlmValidationIssues: visible`, `LlmDependencyRows: 1`, and `LlmXmlDiff: visible`; the smoke now sets clipboard XML, executes the paste command, and checks the pasted draft/status.
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_clipboard_paste_after_20260706_r1` passed with `layout=0`, `text=0`, and `internal=0`; it also executes the paste command.

Latest UI evidence for top account/operator chrome removal:

- Before: `artifacts\account_header_before_20260705_r1\wpf_shell_host_layer_management_commands.png`
- After: `artifacts\account_header_after_20260705_r1\wpf_shell_host_layer_management_commands.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\account_header_after_20260705_r1` passed.
- Structure evidence: `rg -n 'OperatorText|Shell\.Operator|Kind="Account"' -g '*.xaml' -g '*.cs' "0. UI/0) MENU/Wpf"` returns no matches.
- Product decision: account/session UI is not part of the current OpenVisionLab workbench scope.

Latest UI evidence for failed Step rerun/comparison action strip:

- True before note: this action strip was implemented before a fresh before capture was taken. Closest baseline is the immediately prior Recipe Manager role drill-down capture: `artifacts\llm_xml_diff_after_20260705_r1\OpenVisionLab_RecipeManager_RoleDrilldown.png`.
- After: `artifacts\failure_rerun_comparison_after_20260705_r1\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- WPF screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\failure_rerun_comparison_after_20260705_r2_screenshot_smoke` passed.
- Direct EXE smoke: `artifacts\failure_rerun_comparison_after_20260705_r1\report.txt` with `Result: PASS`, `FailedStepRerunComparison: visible`, `RoleDrilldown: Bad -> 01 Battery Cell Vent Alignment Distance`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: Review tab now shows `HostRecipeFailureRerunComparisonPanel` after a failed Step is selected, with direct output/input layer navigation, Step parameter review, and Good/Bad rerun actions. It reuses existing explicit commands and does not introduce auto Preview/Run.

Latest UI evidence for top layer command icon stabilization:

- Before: `artifacts\top_layer_icon_before_20260705_r1\wpf_shell_host_layer_management_commands.png`
- After: `artifacts\top_layer_icon_after_20260705_r2\wpf_shell_host_layer_management_commands.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_layer_management_commands artifacts\top_layer_icon_after_20260705_r2` passed.
- Structure note: top layer create/load/delete icon buttons now share a fixed 28x26 centered style, and the smoke asserts visible button size/order so the white icons do not drift, clip, or disappear under header pressure.

Latest UI evidence for LLM XML diff review and dependency path action hints:

- Before: `artifacts\llm_xml_diff_before_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_xml_diff_after_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- WPF screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\llm_xml_diff_after_20260705_r1_screenshot_smoke` passed.
- Direct EXE smoke: `artifacts\llm_xml_diff_after_20260705_r1\report.txt` with `Result: PASS`, `LlmXmlDiff: visible`, `LlmValidationIssues: visible`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.
- Structure note: LLM XML tab now separates draft validation, dependency scan/copy report, draft import review, LLM XML diff review, and validation issue rows. The diff compares the active pipeline with the draft before import and reports step count, dependency count, added/removed/changed steps, and parameter changes without running Preview.

Latest UI evidence for selected Step PropertyGrid parameter review and explicit XML apply-back:

- Before: `artifacts\recipe_step_parameter_apply_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_step_parameter_apply_after_20260705_r6\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\recipe_step_parameter_apply_after_20260705_r6\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `StepToolEntry: 도구 열기: LineDistance`.
- Structure note: Recipe Manager owns the embedded Step PropertyGrid review/apply path. Opening the native tool seeds repository-backed tool sessions for inspection, but XML apply-back is still an explicit Recipe Manager action.

Latest UI evidence for Recipe Manager workbench layout:

- Before: `artifacts\recipe_workbench_layout_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_workbench_layout_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After with Step PropertyGrid loaded: `artifacts\recipe_workbench_layout_after_20260705_r1\OpenVisionLab_RecipeManager_StepPropertyGrid.png`
- Direct EXE smoke: `artifacts\recipe_workbench_layout_after_20260705_r1\report.txt` with `Result: PASS`, `StepPropertyGridApply: explicit XML apply without Preview/Run`, and `MovedTo: -64.0,18.0`.

Latest UI evidence for Good/Bad role failed-Step drill-down:

- Before: `artifacts\sample_role_drilldown_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\sample_role_drilldown_after_20260705_r3\OpenVisionLab_RecipeManager_RoleDrilldown.png`
- Direct EXE smoke: `artifacts\sample_role_drilldown_after_20260705_r3\report.txt` with `Result: PASS`, `RoleDrilldown: Bad -> 01 Battery Cell Vent Alignment Distance`, and `StepPropertyGridApply: explicit XML apply without Preview/Run`.

Latest UI evidence for multi-step pipeline flow focus:

- Baseline note: the first current-build before capture used the wrong recipe-context screenshot target, so it is only a closest reproducible baseline for the shell state, not a true Recipe Manager before view.
- Closest baseline: `artifacts\multi_step_flow_before_20260705_r1\wpf_shell_host_recipe_context_switch.png`
- After full-window Recipe Manager capture: `artifacts\multi_step_flow_after_20260705_r3_recipe_manager\wpf_shell_host_recipe_language_controls.png`
- After Recipe Manager panel crop: `artifacts\multi_step_flow_after_20260705_r3_recipe_manager\wpf_shell_host_recipe_language_controls.diagnostics\recipe-manager-panel.png`
- Screenshot smoke: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_recipe_language_controls artifacts\multi_step_flow_after_20260705_r3_recipe_manager` passed.
- Structure note: Recipe Manager now exposes current selected Step flow in the header (`HostRecipePipelineHeaderStepFlow`), adds an XML/Step flow focus strip with Previous/Next commands, and verifies next-Step navigation does not trigger Preview/Run.

Latest UI evidence for structured LLM XML validation rows:

- Before: `artifacts\llm_xml_validation_rows_before_20260705_r1\OpenVisionLab_RecipeManager_LlmXml.png`
- After: `artifacts\llm_xml_validation_rows_after_20260705_r5\OpenVisionLab_RecipeManager_LlmXml.png`
- Direct EXE smoke: `artifacts\llm_xml_validation_rows_after_20260705_r5\report.txt` with `Result: PASS` and `LlmValidationIssues: visible`.

Latest UI evidence for selected Step input/output layer cards:

- Before: `artifacts\step_layer_navigation_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\step_layer_navigation_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\step_layer_navigation_after_20260705_r1\report.txt` with `Result: PASS` and `StepLayerCards: visible`.

Latest UI evidence for selected Step thumbnail cards and click navigation:

- Before: `artifacts\step_layer_click_nav_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\step_layer_click_nav_after_20260705_r4\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\step_layer_click_nav_after_20260705_r4\report.txt` with `Result: PASS` and `StepLayerNavigation: Battery_CellVentAlignment_Preview -> Main`.

Latest UI evidence for Good/Bad role result cards:

- Before: `artifacts\sample_review_drilldown_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\sample_review_drilldown_after_20260705_r2\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\sample_review_drilldown_after_20260705_r2\report.txt` with `Result: PASS` and `PairRoleCards: 2`.

Latest UI evidence for selected Step ROI/template metadata and tool entry:

- Before: `artifacts\recipe_step_roi_template_before_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- After: `artifacts\recipe_step_roi_template_after_20260705_r1\OpenVisionLab_RecipeManager_Pipeline.png`
- Direct EXE smoke: `artifacts\recipe_step_roi_template_after_20260705_r1\report.txt` with `Result: PASS`, `StepRoiTemplate: ROI: 172,166,116,136 | Template: 없음`, and `StepToolEntry: 도구 열기: LineDistance`.

## Latest Original Repo Commits

Key latest stable code/workflow commits in `C:\Git\OpenVisionLab`:

- `9c2bbe1 Show pipeline review parameter focus hints`
- `c90d60a Record pipeline review parameter location hints`
- `2371b37 Add pipeline review parameter location hints`
- `bc42e0e Record pipeline review label polish`
- `71ecc21 Localize pipeline review guide labels`
- `b8a95cf Record catalog audit after metric cleanup`

## Completed On 2026-07-03

- Product sample catalog/native runner gate is stable.
  - Dev evidence: `artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json`
  - Original full evidence: `artifacts\original_product_catalog_full_20260703_1919\sample_catalog_summary.json`
  - Original final evidence: `artifacts\product_catalog_final_20260703_1920\sample_catalog_summary.json`
  - Original after Line cleanup evidence: `artifacts\product_catalog_after_line_controller_cleanup_20260703_1935\sample_catalog_summary.json`
  - Original full result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`
  - Original final result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`, `DurationSeconds=81.234`
  - Original after Line cleanup result: `GateStatus=OK`, `RunnableRows=168`, `RequiredRows=84`, `ExpectedFailureRows=84`, `OKRows=168`, `NGRows=0`, `DurationSeconds=70.815`
  - Quality audit: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
  - Latest quality audit after Line cleanup: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`
- Original repo reviewed import, Product Field Explore samples and picker affordance:
  - Imported the reviewed Dev Field Explore sample bundle into `C:\Git\OpenVisionLab` without bulk-copying the repo.
  - Original data/assets added:
    - 16 PNGs under `C:\Git\OpenVisionLab\docs\samples\public\product\field\`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_DarkFeature_Contour.pipeline.xml`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_BrightFeature_Contour.pipeline.xml`
    - `C:\Git\OpenVisionLab\docs\samples\public\product\Product_Field_SurfaceMean.pipeline.xml`
    - 16 `Product_Field_*` Explore rows in `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`
    - 16 field provenance rows in `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv`
  - Original UI/runtime imported:
    - `OpenVisionWorkspaceSampleFocusOption` now exposes a `field` focus only for `ValidationMode=Explore` field-style samples.
    - `OpenVisionWorkspaceSamplePickerViewModel` shows `Explore sample`, reference metric copy, and `ExploratoryGuideText` for Explore rows.
    - `OpenVisionWorkspaceSamplePickerView.xaml` displays `WorkspaceSamplePickerExploreGuide`.
    - `VisionPipelineSampleCatalog` Product source copy now describes Good/Bad plus Field Explore samples.
    - `PipelineViewerScreenshotSmoke` has `wpf_shell_host_workspace_sample_product_field_focus_picker`.
  - Before/after Original UI evidence:
    - Before Field affordance: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_before_ui_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - After Field affordance: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_after_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Existing Product focus after: `C:\Git\OpenVisionLab\artifacts\product_field_explore_original_after_20260703_02\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Original verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\product_field_explore_original_after_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\product_field_explore_original_after_20260703_02` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed: `PublicSampleAssetCheck=PASS | CatalogRows=184 ManifestAssets=214 Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_184_original_after_field_import_20260703_01 -SkipRunnerBuild` passed: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=76.231`.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `rg -n "ChatGPT|C:\\Git\\새 폴더|새 폴더|DALL|OpenAI" docs\samples\public\product docs\samples\OpenVisionLab.ProductSampleCatalog.csv tools\GenerateOpenVisionProductSamples.ps1` returned no matches.
  - Source-target evidence:
    - Field PNG hashes match Dev: 16 files, 0 mismatches.
    - Field pipeline hashes match Dev: 3 files, 0 mismatches.
    - Dev/Original text equality confirmed for Field focus/view/viewmodel/catalog CSV/manifest/README/generator and `VisionPipelineSampleCatalog`.
    - Known deviation: `tools\PipelineViewerScreenshotSmoke\Program.cs` differs from Dev by one non-Field line, `resultCountMetricText`, which was left outside this import scope.
- Dev Tool View code-behind cleanup, text presenter extraction:
  - Added small text presenters so View code-behind no longer owns static localization assignments for Threshold, Filter, and Morphology:
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\ThresholdToolTextPresenter.cs`
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\FilterToolTextPresenter.cs`
    - `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\MorphologyToolTextPresenter.cs`
  - Updated Views:
    - `ThresholdToolWpfView.xaml.cs` now delegates Threshold parameter/mode labels to `ThresholdToolTextPresenter`.
    - `FilterToolWpfView.xaml.cs` now delegates operation/kernel labels to `FilterToolTextPresenter`.
    - `MorphologyToolWpfView.xaml.cs` now delegates operation/kernel/shape labels to `MorphologyToolTextPresenter`, while keeping operation/shape button state in `VisionToolMorphologyInteractionController`.
  - Structure evidence:
    - `rg -n "OpenVisionLanguageService\.T\("` over the three View code-behind files returns no matches; localization calls remain in the presenter classes.
    - Responsibility moved from View code-behind to text presenter layer; existing tool runtime/controller event paths were not changed.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\FilterToolTextPresenter.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolTextPresenter.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolTextPresenter.cs"` passed with CRLF warnings only.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\tool_text_presenters_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\tool_text_presenters_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\tool_text_presenters_dev_20260703_01` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_filter_morphology_layout_guard.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_shell_host_threshold_basic_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\tool_text_presenters_dev_20260703_01\wpf_shell_host_threshold_tool.png`
- Dev Tool View code-behind cleanup, Line text presenter and Filter/Morphology event attach:
  - Added `C:\Git\OpenVisionLab_Dev\0. UI\6) Vision Test\Wpf\LineToolTextPresenter.cs`.
  - `LineToolWpfView.xaml.cs` now delegates Line purpose labels, ROI tooltip, purpose hint, and summary text composition to `LineToolTextPresenter`.
  - `VisionToolKernelSizeController` now attaches/detaches kernel text, lock, and preset button events directly.
  - `VisionToolFilterInteractionController` now attaches/detaches Filter type and border type selection events directly.
  - `VisionToolMorphologyInteractionController` now attaches/detaches operation button and shape radio events directly.
  - Removed direct XAML event handler attributes from `FilterToolWpfView.xaml` and `MorphologyToolWpfView.xaml` for those controller-owned paths.
  - Structure evidence:
    - `LineToolWpfView.xaml.cs` no longer contains `VisionToolVerificationText`, `VisionToolChromePresenter.ApplyTooltip`, or direct `presenter.CreateSummary(...)` usage.
    - `FilterToolWpfView.xaml` and `MorphologyToolWpfView.xaml` no longer contain controller-owned `SelectionChanged`, `TextChanged`, `Checked`, `Unchecked`, or `Click` handler attributes, except normal `IsChecked` state.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolTextPresenter.cs"` passed with CRLF warnings only.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolKernelSizeController.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolFilterInteractionController.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolMorphologyInteractionController.cs" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs"` passed with CRLF warnings only.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_text_presenter_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morph_controller_event_attach_dev_20260703_01` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_measure_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\line_text_presenter_dev_20260703_01\wpf_shell_host_line_intersection_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\filter_morph_controller_event_attach_dev_20260703_01\wpf_filter_morphology_layout_guard.png`
- Dev Pipeline Review result presenter extraction:
  - Added `C:\Git\OpenVisionLab_Dev\0. UI\0) MENU\Wpf\OpenVisionPipelineReviewResultPresenter.cs`.
  - `OpenVisionPipelineReviewDocument` now delegates selected-step run log, result summary/detail, Good/Bad pair action text, and pair metric comparison text to the presenter.
  - The document keeps pipeline execution, layer image cache, validation, sample-pair resolution, and View update orchestration.
  - `ResultCount` display now keeps the public-smoke `Result` token while retaining localized operator text, for example `Result (결과 수)` in Korean.
  - Structure evidence:
    - `rg -n "private .*FormatRunLog|private .*FormatResultSummary|private .*FormatResultDetails|private .*ResolvePairMetricComparisonText|private .*ResolvePairActionText|FormatPrimaryMetricText|FormatMetricName|LocalText\(" "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs"` returns no matches.
    - `OpenVisionPipelineReviewDocument.cs` calls `OpenVisionPipelineReviewResultPresenter.FormatRunLog`, `FormatResultSummary`, `FormatResultDetails`, `ResolvePairActionText`, and `ResolvePairMetricComparisonText`.
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_metrics artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\pipeline_review_result_presenter_dev_20260703_02` passed.
  - Dev screenshot evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_result_presenter_dev_20260703_02\wpf_shell_host_workspace_product_sample_review_ng.png`
- Dev Pipeline Review operator NG triage UX:
  - Added NG-only operator triage fields to `OpenVisionPipelineReviewGuideState` and `OpenVisionPipelineReviewViewModel`.
  - `OpenVisionPipelineReviewGuidePresenter` now supplies separate cause, adjustment, and rerun texts for NG/acceptance-NG steps.
  - `OpenVisionPipelineReviewView.xaml` shows the triage strip inside the guide detail area only when NG triage text exists.
  - `OpenVisionShellHostStatePresenter`, `OpenVisionShellHostToolTestFacade`, and `OpenVisionShellHostView.TestHooks.cs` expose the new triage texts for smoke evidence.
  - Added localization keys:
    - `PipelineReview.Guide.TriageFailure`
    - `PipelineReview.Guide.TriageAdjustment`
    - `PipelineReview.Guide.TriageRerun`
    - `PipelineReview.Guide.TriageRerunPair`
  - Reduced the lower detail row height in Pipeline Review from `250` to `210` so the input/output preview area remains visible after the triage strip is shown.
  - Before/after Dev UI evidence:
    - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_ux_before_dev_20260703_01\wpf_shell_host_workspace_product_sample_review_ng.png`
    - After: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_ux_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_ux_after_dev_20260703_02` passed.
    - `git diff --check --` over the touched Pipeline Review/localization/smoke files passed with CRLF warnings only.
  - Structure evidence:
    - `rg -n "PipelineReviewGuideTriage(Failure|Adjustment|Rerun)|ReviewGuideTriage(Failure|Adjustment|Rerun)|HasReviewGuideTriage|TriageRerunPair" "0. UI\0) MENU\Wpf" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` shows the ViewModel, View, document/test hook, localization, and smoke assertion path.
- Dev MainView/Product sample counterpart affordance:
  - Current-flow evaluation screenshots were refreshed before changing the MainView workflow strip:
    - Picker before/evaluation: `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_eval_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - Open before/evaluation: `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_eval_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Added a direct counterpart sample button to the bottom sample workflow strip:
    - Good samples show `NG 기준 열기`.
    - Bad/NG samples show `OK 기준 열기`.
  - `OpenVisionShellHostSampleWorkflowPresenter` now resolves the opposite Good/Bad sample in the same PairGroup and exposes `CounterpartSampleName`.
  - `OpenVisionShellHostWorkspaceCommandSurface` adds `OpenSampleCounterpartCommand`, reusing `OpenRunnableSampleByName` so the action swaps the sample image/pipeline only and does not run Preview or open a tool.
  - `OpenVisionShellHostView.TestHooks.cs` exposes `CanOpenSampleCounterpartForTest` and `OpenSampleCounterpartForTest`.
  - Added smoke target `wpf_shell_host_workspace_sample_product_counterpart_open`.
  - After Dev UI evidence:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_counterpart_open.png`
  - Dev verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_after_dev_20260703_01` passed.
    - `dotnet run --no-build --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_after_dev_20260703_01` passed.
    - `git diff --check --` over the touched MainView workflow/smoke files passed with CRLF warnings only.
  - Stable-contract evidence:
    - The counterpart command smoke asserts no active WPF tool, no active native document, no native Preview result, and unchanged `NativePreviewRunCount` after switching samples.
- Self-evaluation document was added.
  - File: `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md`
  - Conclusion: target-product maturity `4.0/5`; industrial integrated-platform maturity `2.0/5`.
  - Keep the product advantage focused on PropertyGrid tools, transparent layer routes, Preview/Pipeline separation, and sample-backed review.
- MainView/Product sample workflow was improved.
  - After opening a sample, the bottom workflow strip exposes the hint `Pipeline Review에서 NG/OK 기준 열기`.
  - Product group labels are shortened to `Secondary Battery`, `Display`, and `Semiconductor` so the review hint stays visible.
  - Original commit: `b011ee2`
- Pipeline Review operator guide was improved.
  - Final OK no longer implies a misleading "next step"; it points to output/support-layer review and Good/Bad pair comparison.
  - NG review now shows tool-type-specific `우선 확인:` guidance.
  - Original commits: `95ed902`, `e98a0b2`
- Product sample NG review smoke was fixed.
  - `wpf_shell_host_workspace_product_sample_review_ng` now accepts Product catalog samples instead of asserting Public source kind.
  - Original commit: `b0da050`
- Contour teaching preview stale review was fixed.
  - `ContourToolWpfView.RequestThresholdTeachingPreview()` now clears stale result review before teaching preview, matching Blob behavior.
  - Original commit: `bab969e`
- Korean duplicate-key detection was fixed.
  - `WpfPropertyGridAdapter` now detects `같은 키` in duplicate-key messages instead of a mojibake string.
  - Original commit: `01c7aa4`
- Public/product sample review smoke coverage was consolidated.
  - New script: `tools\RunSampleReviewUiSmokes.ps1`
  - The script runs single WPF targets sequentially to avoid the previously observed multi-target suite hang.
  - Required pair coverage now uses public/product representative groups instead of legacy root-only sample groups.
  - Bad-reference audit now requires controlled NG samples and treats legacy comparative bad references as optional/private.
  - Original commit: `6ca54d3`
- Tool View code-behind cleanup continued.
  - `VisionToolKernelSizeController` now owns shared kernel preset click parsing for Filter and Morphology.
  - Filter/Morphology views now use the same Tag-based preset click path instead of separate 3/5/7 handlers.
  - Original commit: `567fefc`
- Filter/Morphology layout smoke was restored.
  - `SelectComboBoxItemText` now accepts the already-selected value as a valid selection.
  - The layout guard now clicks Filter and Morphology kernel preset buttons, so the shared preset handler is covered by smoke.
  - Original commit: `0a2e026`
- Pipeline Review top-card next-action copy was shortened.
  - The long final OK/NG next-action strings now fit the top summary card while the detailed guide still carries the longer explanation.
  - Existing runtime `CONFIG\localization_catalog.tsv` files migrate from the previous default strings to the shorter defaults.
  - Original commit: `5f76663`
- Filter/Morphology code-behind was trimmed.
  - Unused imaging, IO, and OpenCvSharp morphology usings were removed after the shared preset controller extraction.
  - Original commit: `4278e43`
- Self-evaluation evidence was refreshed after the final catalog and UI smoke passes.
  - The self-evaluation conclusion remains unchanged: OpenVisionLab should stay a rule-based OpenCvSharp4 PropertyGrid-centered workbench, not a hardware integration platform.
  - Original commit: `031c347`
- Line tool code-behind cleanup continued.
  - Test-only selected-line configuration now lives in `LineToolInteractionController`; `LineToolWpfView` keeps the public test hooks as thin wrappers.
  - Original commit: `2ed377a`
- Pipeline Review metric wording was clarified.
  - `ResultCount` now displays as `결과 수` in Korean Pipeline Review NG guidance.
  - The Product sample NG review smoke now asserts localized metric display text in the guide detail.
  - Original commit: `dabf398`
- MainView/Product sample user-flow was re-evaluated with current Dev build screenshots.
  - The bottom workflow strip shows product group, Good/Bad direction, NG/OK counterpart action, Pipeline Review, and first-step action.
  - Pipeline Review shows Good/Bad pair context, metric check, checklist, and explicit counterpart-open action.
  - No additional UI change was made in this pass.
- Pipeline Review sample metric explanations were localized.
  - `ResultCount`, `MeanValueAvg`, and `DistanceMmAvg` now use localized display names in Pipeline Review result detail, Good/Bad pair text, metric check, and checklist text.
  - Mean NG fix detail no longer repeats raw `AcceptanceMessage`; it points the operator to input layer, ROI, Mean type, lighting/brightness drift, and target range.
  - `PipelineViewerScreenshotSmoke` now rejects raw expected metric keys in localized guide/detail/pair text.
  - Original commit: `470f863`
- Product sample catalog was re-audited after metric explanation cleanup.
  - Dev and Original audits both passed: `ProductSampleQualityAudit=PASS | PairRecords=84 OK=84 Review=0 Critical=0`.
  - The catalog still has 84 Good rows and 84 Bad rows, with 84 PairGroups and one shared baseline pipeline per pair.
  - No new product samples are warranted before improving review/explanation UX further.
- Tool View code-behind candidate review was performed.
  - `git diff --no-index --ignore-space-at-eol` between Original and Dev WPF Tool View files produced no semantic diff.
  - Current Dev Tool View diff is mostly Dev baseline/line-ending noise; Original already has the reviewed controller/runtime cleanup.
  - No Tool View code change was made in this pass.
- Pipeline Review guide labels were localized.
  - `Good/Bad Pair` now displays as `Good/Bad 쌍`.
  - `Metric Check` now displays as `지표 확인`.
  - Original commit: `71ecc21`
- Pipeline Review NG parameter-location hints were added.
  - NG detail now keeps the reason, first check, and `조정 위치:` in the same operator guide line.
  - Tool-type-specific hints point to the PropertyGrid parameter panel areas for Threshold, Blob, Contour, Line, Mean, Matching/Feature, and generic steps.
  - `PipelineViewerScreenshotSmoke` now requires the localized parameter-location prefix and `파라미터 패널` text in NG guide detail.
  - Original commit: `2371b37`
- Pipeline Review parameter panel focus hints were added.
  - The lower Parameters panel now repeats the selected NG step's `조정 위치:` hint directly above the parameter list.
  - The hint is data-bound from `OpenVisionPipelineReviewGuideState.ParameterFocusText`; it does not trigger Preview or Run.
  - ShellHost test hooks and `PipelineViewerScreenshotSmoke` now verify the same focus text.
  - Original commit: `9c2bbe1`
- Pipeline Review metric gap explanation was added in Dev.
  - Acceptance NG text now keeps the localized metric name, measured value, target range, and adds the target gap such as `511` over max or `67.5` under min.
  - The change is centralized in `OpenVisionPipelineReviewGuidePresenter.FormatAcceptanceMetricNgReason`, so Pipeline Review detail and result detail use the same wording.
  - Current-build before/after screenshots were captured for the generic NG Pipeline Review path.

## Verification Evidence

- Dev build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Original build:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed.
- Smoke tool build:
  - Dev: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
  - Original: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
- Readiness:
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
- Reference and sample policy checks:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed in Dev and Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed in Dev and Original.
  - Re-run at 2026-07-03 19:27 KST passed in Dev and Original.
  - Re-run at 2026-07-03 21:16 KST passed in Dev and Original after parameter-location hints.
  - Re-run at 2026-07-03 21:48 KST passed in Dev and Original after parameter focus hints.
- Product sample full catalog:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\original_product_catalog_full_20260703_1919` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_final_20260703_1920` passed in Original.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_line_controller_cleanup_20260703_1935` passed in Original.
  - Dev quality audit after metric cleanup: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\self_evaluation_product_catalog_20260703_1750\sample_catalog_summary.json -OutputDir artifacts\product_quality_after_metric_cleanup_20260703_2018 -FailOnCritical` passed.
  - Original quality audit after metric cleanup: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\product_catalog_after_line_controller_cleanup_20260703_1935\sample_catalog_summary.json -OutputDir artifacts\product_quality_after_metric_cleanup_20260703_2018 -FailOnCritical` passed.
- Sample review UI smoke runner:
  - Dev current-flow evaluation: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_user_flow_eval_20260703_1959` passed.
  - Dev: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_script_after_auditfix_20260703_1918` passed.
  - Original: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\original_sample_review_ui_smoke_script_after_auditfix_20260703_1919` passed.
  - Original re-run after layout smoke restore: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_layout_guard_restore_20260703_1903` passed.
  - Original re-run after Pipeline Review copy shortening: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_after_pipeline_copy_short_20260703_1915` passed.
  - Original final re-run: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260703_1930` passed.
  - Product sample NG after `ResultCount` wording: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948` passed.
  - Dev Product OK after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\metric_display_product_ok_after_dev2_20260703_2030` passed.
  - Dev Product NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\metric_display_product_ng_after_dev2_20260703_2031` passed.
  - Dev Mean NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\metric_display_mean_after_dev6_20260703_2031` passed.
  - Dev Line NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\metric_display_line_after_dev3_20260703_2031` passed.
  - Original Product OK after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\metric_display_product_ok_after_original_20260703_2038` passed.
  - Original Product NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\metric_display_product_ng_after_original_20260703_2038` passed.
  - Original Mean NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\metric_display_mean_after_original_20260703_2039` passed.
  - Original Line NG after metric display cleanup: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\metric_display_line_after_original_20260703_2039` passed.
  - Dev Pipeline Review label localization: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_labels_after_dev_20260703_2027` passed.
  - Original Pipeline Review label localization: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_labels_after_original_20260703_2028` passed.
  - Dev Mean NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_location_after_dev_20260703_2105` passed.
  - Dev Line NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_location_after_dev_20260703_2105` passed.
  - Dev generic NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_location_after_dev_20260703_2105` passed.
  - Original Mean NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_location_after_original_20260703_2115` passed.
  - Original Line NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_location_after_original_20260703_2115` passed.
  - Original generic NG parameter-location hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_location_after_original_20260703_2115` passed.
  - Dev Mean NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_focus_after_dev_20260703_2135` passed.
  - Dev Line NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_focus_after_dev_20260703_2135` passed.
  - Dev generic NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_focus_after_dev_20260703_2135` passed.
  - Original Mean NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_parameter_focus_after_original_20260703_2145` passed.
  - Original Line NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics artifacts\pipeline_review_line_parameter_focus_after_original_20260703_2145` passed.
  - Original generic NG parameter focus hint: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_parameter_focus_after_original_20260703_2145` passed.
  - Dev metric gap before: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_metric_gap_before_dev_20260703_01` passed.
  - Dev metric gap after: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_metric_gap_after_dev_20260703_01` passed.
  - Dev sample NG gap after: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_metric_gap_sample_ng_after_dev_20260703_01` passed.
  - Dev metric gap build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed.
- Filter/Morphology guard:
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morphology_layout_guard_after_dev_20260703_1903` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\filter_morphology_layout_guard_after_original_20260703_1908` passed.
- Pipeline Review OK/NG:
  - Original OK: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_ok_after_smoke_restore_20260703_1906` passed.
  - Original NG: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_after_smoke_restore_20260703_1906` passed.
  - Original OK after copy shortening: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_top_card_short_after_original_20260703_1917` passed.
  - Original NG after copy shortening: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_top_card_short_ng_after_original_20260703_1917` passed.
  - Original final OK: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review artifacts\pipeline_review_ok_final_20260703_1931` passed.
  - Original final NG: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_ng_final_20260703_1931` passed.
- Line tool controller cleanup:
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_controller_test_hook_after_dev_20260703_1932` passed.
  - Dev: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_controller_intersection_after_dev_20260703_1932` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_controller_test_hook_after_original_20260703_1934` passed.
  - Original: `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_controller_intersection_after_original_20260703_1935` passed.

## Screenshot Evidence

- Product sample focus after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\sample_workflow_pair_hint_after2_20260703_1821\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_sample_workflow_pair_hint_after_20260703_1825\wpf_shell_host_workspace_sample_product_focus_open.png`
- Product sample NG review after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\operator_review_pair_flow_after_fix_20260703_1833\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_product_sample_review_ng_after_fix_20260703_1836\wpf_shell_host_workspace_product_sample_review_ng.png`
- Contour teaching preview clear after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\contour_teaching_clear_after_20260703_1849\wpf_shell_host_contour_tool.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_contour_teaching_clear_after_20260703_1851\wpf_shell_host_contour_tool.png`
- PropertyGrid duplicate-key smoke after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\property_grid_duplicate_key_string_after_20260703_1855\wpf_property_grid_matching_combo.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\original_property_grid_duplicate_key_string_after_20260703_1857\wpf_property_grid_matching_combo.png`
- Filter/Morphology layout guard after:
  - Dev: `C:\Git\OpenVisionLab_Dev\artifacts\filter_morphology_layout_guard_after_dev_20260703_1903\wpf_filter_morphology_layout_guard.png`
  - Original: `C:\Git\OpenVisionLab\artifacts\filter_morphology_layout_guard_after_original_20260703_1908\wpf_filter_morphology_layout_guard.png`
- Pipeline review OK/NG after:
  - OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ok_after_smoke_restore_20260703_1906\wpf_shell_host_pipeline_review.png`
  - NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_after_smoke_restore_20260703_1906\wpf_shell_host_pipeline_review_ng.png`
- Pipeline review next-action copy after:
  - OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_top_card_short_after_original_20260703_1917\wpf_shell_host_pipeline_review.png`
  - NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_top_card_short_ng_after_original_20260703_1917\wpf_shell_host_pipeline_review_ng.png`
- Final review smoke after:
  - Sample runner: `C:\Git\OpenVisionLab\artifacts\sample_review_ui_smoke_final_20260703_1930`
  - Pipeline OK: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ok_final_20260703_1931\wpf_shell_host_pipeline_review.png`
  - Pipeline NG: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_final_20260703_1931\wpf_shell_host_pipeline_review_ng.png`
- Product sample NG metric wording after:
  - Before: `C:\Git\OpenVisionLab\artifacts\sample_review_ui_smoke_final_20260703_1930\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Dev after: `C:\Git\OpenVisionLab_Dev\artifacts\product_sample_review_ng_metric_display_after_dev_20260703_1946\wpf_shell_host_workspace_product_sample_review_ng.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\product_sample_review_ng_metric_display_after_original_20260703_1948\wpf_shell_host_workspace_product_sample_review_ng.png`
- MainView/Product sample current-flow evaluation:
  - Dev Product focus: `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_user_flow_eval_20260703_1959\wpf_shell_host_workspace_sample_product_focus_open.png`
  - Dev Product pair handoff: `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_user_flow_eval_20260703_1959\wpf_shell_host_workspace_product_sample_pair_open.png`
- Pipeline Review metric explanation cleanup:
  - Mean before: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_before_original_20260703_2002\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Line before: `C:\Git\OpenVisionLab\artifacts\metric_display_line_before_original_20260703_2002\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\metric_display_mean_after_dev6_20260703_2031\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\metric_display_line_after_dev3_20260703_2031\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\metric_display_line_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Product OK after: `C:\Git\OpenVisionLab\artifacts\metric_display_product_ok_after_original_20260703_2038\wpf_shell_host_workspace_product_sample_review.png`
  - Original Product NG after: `C:\Git\OpenVisionLab\artifacts\metric_display_product_ng_after_original_20260703_2038\wpf_shell_host_workspace_product_sample_review_ng.png`
- Pipeline Review guide label localization:
  - Before: `C:\Git\OpenVisionLab\artifacts\metric_display_mean_after_original_20260703_2039\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_labels_after_dev_20260703_2027\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_labels_after_original_20260703_2028\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
- Pipeline Review parameter-location hints:
  - Original Mean before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_location_before_original_20260703_2110\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_location_after_dev_20260703_2105\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_location_after_original_20260703_2115\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_line_parameter_location_after_dev_20260703_2105\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_line_parameter_location_after_original_20260703_2115\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original generic NG before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_location_before_original_20260703_2110\wpf_shell_host_pipeline_review_ng.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_ng_parameter_location_after_dev_20260703_2105\wpf_shell_host_pipeline_review_ng.png`
  - Original generic NG after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_location_after_original_20260703_2115\wpf_shell_host_pipeline_review_ng.png`
- Pipeline Review parameter focus hints:
  - Dev Mean before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_focus_before_dev_20260703_2130\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean before: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_focus_before_original_20260703_2140\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Mean after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_parameter_focus_after_dev_20260703_2135\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original Mean after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_parameter_focus_after_original_20260703_2145\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Dev Line after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_line_parameter_focus_after_dev_20260703_2135\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Original Line after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_line_parameter_focus_after_original_20260703_2145\wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_ng_parameter_focus_after_dev_20260703_2135\wpf_shell_host_pipeline_review_ng.png`
  - Original generic NG after: `C:\Git\OpenVisionLab\artifacts\pipeline_review_ng_parameter_focus_after_original_20260703_2145\wpf_shell_host_pipeline_review_ng.png`
- Pipeline Review metric gap explanation:
  - Dev generic NG before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_before_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Dev generic NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_after_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Dev sample Mean NG after: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_metric_gap_sample_ng_after_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
- Product field-style sample import:
  - Imported 16 project-authored field-style PNGs into `docs\samples\public\product\field` with clean product/inspection names and 960px max dimension.
  - Added 16 `Product_Field_*` Explore rows to `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`.
  - Added field sample provenance rows to `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` and the product sample generator manifest output.
  - Added three exploratory baseline pipelines: `Product_Field_DarkFeature_Contour.pipeline.xml`, `Product_Field_BrightFeature_Contour.pipeline.xml`, and `Product_Field_SurfaceMean.pipeline.xml`.
  - Added a `Field` product/tool focus option in the sample picker so field-style samples are not buried in the Product catalog list.
  - Contact sheet: `C:\Git\OpenVisionLab_Dev\artifacts\user_sample_import_review_20260703\imported_field_sample_contact_sheet.png`.
  - Current overlay contact sheet after metric tuning: `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_catalog_20260703_03\field_overlay_contact_sheet.png`.
  - Field rows now carry expected metric ranges based on current runner output: `ResultCount` for contour pipelines and `MeanValueAvg` for the surface mean pipeline.
  - Self-evaluation: keep these 16 samples as `Explore` rows for now. They are useful, more field-like recipe setup examples, but several overlays are intentionally broad and should not be promoted to controlled Good/Bad pairs without tighter per-sample pipelines.
  - UI before/after captures:
    - Before Original: `C:\Git\OpenVisionLab\artifacts\field_sample_focus_before_original_20260703\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - After Dev: `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_focus_after_dev_20260703\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Verification:
    - `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU"` passed.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed in Dev and Original for sample picker capture.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed.
    - Representative runner smoke passed for dark contour, bright contour, and surface mean pipelines under `artifacts\field_sample_smoke_20260703`.
    - Field-only catalog gate after expected metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath artifacts\field_sample_catalog_20260703_03\field_sample_catalog.csv -OutputDir artifacts\field_sample_catalog_20260703_03 -SkipRunnerBuild -FailOnExplore` passed with `GateStatus=OK`, `RunnableRows=16`, `OKRows=16`, `NGRows=0`.
    - Full Product catalog after field metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_184_after_field_metric_gap_dev_20260703_01 -SkipRunnerBuild` passed with `GateStatus=OK`, `RunnableRows=184`, `OKRows=184`, `NGRows=0`.
    - Public sample policy after metric tuning: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - Source/privacy check after rename/import: `rg -n "ChatGPT|C:\\Git\\새 폴더|새 폴더|DALL|OpenAI" docs\samples\public\product docs\samples\OpenVisionLab.ProductSampleCatalog.csv tools\GenerateOpenVisionProductSamples.ps1` returned no matches.

- Tool View code-behind cleanup, Line tool:
  - Moved Line ROI/default-ROI mutation from `0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs` into `0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs`.
  - The View now delegates `EnsureDefaultRoi`, `ApplySelectedLineRoi`, and `SetRoiForTest` to the controller. Existing `VisionToolPropertyChangeController.RefreshAfterExternalUpdate` behavior remains the single path for summary/overlay/preview policy updates.
  - Preserved PropertyGrid-based Line parameter editing and did not add broad base classes or new runtime abstractions.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_tool_controller_refactor_dev_20260703_01` passed.
- Pipeline/Recipe operator review UX, NG next action focus:
  - `OpenVisionPipelineReviewGuidePresenter.ResolveNextActionText` now keeps the existing generic NG instruction but prefixes it with the failed acceptance metric or tool-specific focus area.
  - Example after: `평균 밝기(Mean) 기준 확인 / 파라미터/라우트 조정 후 재리뷰`.
  - `PipelineViewerScreenshotSmoke` now asserts the NG next action contains the localized failed metric name for sample NG and generic acceptance NG review targets.
  - UI before/after captures:
    - Before Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_before_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - After Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_after_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - Generic NG after Dev: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_operator_review_after_dev_20260703_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs" "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\LineToolInteractionController.cs" "docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_after_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_operator_review_after_dev_20260703_01` passed.
- Tool View code-behind cleanup, SimplePreprocess settings restore:
  - `SimplePreprocessParameterController.ApplySettings` now preserves the previous suppress state using its existing `isSuppressed` dependency.
  - `SimplePreprocessToolWpfView.ApplyPersistedSettings` now delegates suppression to the controller and only replays the lightweight `ParameterChanged` path after restore.
  - This keeps dynamic SimplePreprocess parameter restore behavior out of View code-behind without changing the generated parameter UI contract.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\SimplePreprocessParameterController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_controller_refactor_dev_20260703_01` passed.
- MainView/Product sample review, Field Explore affordance:
  - Fixed Field focus filtering in `OpenVisionWorkspaceSampleFocusOption.Matches`; `field` now requires `ValidationMode=Explore` and field-style product tokens instead of falling through the generic LearnPath fallback.
  - Added Field focus smoke target `wpf_shell_host_workspace_sample_product_field_focus_picker`.
  - For Explore samples, `OpenVisionWorkspaceSamplePickerViewModel` now shows `Explore 샘플`, formats expected ranges as reference metrics rather than fixed OK/NG criteria, and exposes a short guide explaining that the sample is for recipe setup rather than a controlled Good/Bad decision pair.
  - `OpenVisionWorkspaceSamplePickerView.xaml` displays the Explore guide in the benchmark strip via `WorkspaceSamplePickerExploreGuide`.
  - Before/after evidence:
    - Initial Field target before focus fix failed: selected `Product_Battery_TabGap_Good` with `Mode=Required` after choosing Field.
    - Field after focus fix, before Explore guide: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_before_dev_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Field after Explore guide: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
    - Existing Product focus after: `C:\Git\OpenVisionLab_Dev\artifacts\product_field_explore_guide_after_dev_20260703_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSampleFocusOption.cs" "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerView.xaml" "0. UI\0) MENU\Wpf\OpenVisionWorkspaceSamplePickerViewModel.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\product_field_explore_guide_after_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\product_field_explore_guide_after_dev_20260703_01` passed.
- Original repo reviewed patch import, Pipeline Review metric guidance:
  - Imported only the reviewed Pipeline Review metric-gap/NG next-action focus change into `C:\Git\OpenVisionLab`; did not bulk-copy Dev.
  - Original touched files:
    - `C:\Git\OpenVisionLab\0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs`
    - `C:\Git\OpenVisionLab\tools\PipelineViewerScreenshotSmoke\Program.cs`
  - Deferred Field Explore sample/UI import into Original because Original does not yet contain the Product Field sample assets/catalog rows. Importing only the Field UI would expose dead or untestable affordance.
  - Original after capture:
    - `C:\Git\OpenVisionLab\artifacts\pipeline_operator_review_original_after_20260703_01\wpf_shell_host_pipeline_review_ng.png`
    - `C:\Git\OpenVisionLab\artifacts\pipeline_operator_review_original_after_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - Original verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\OpenVisionPipelineReviewGuidePresenter.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_operator_review_original_after_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_operator_review_original_after_20260703_01` passed.
- Product sample catalog quality follow-up, Dev 184-row gate:
  - Re-ran the full Product catalog after the Field Explore import and current review UX changes.
  - Full catalog summary: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=72.891`.
  - Product sample quality audit passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
  - Evidence:
    - Catalog run output: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json`
    - Catalog run report: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_report.md`
    - Quality audit report: `C:\Git\OpenVisionLab_Dev\artifacts\product_catalog_quality_followup_audit_dev_20260703_01\product_sample_quality_audit.md`
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_quality_followup_dev_20260703_01 -SkipRunnerBuild` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json -OutputDir artifacts\product_catalog_quality_followup_audit_dev_20260703_01 -FailOnCritical` passed.
  - Self-evaluation: no additional product sample generation is needed before stabilizing/importing the current Dev UX and Tool View changes. The remaining sample risk is not quantity, but whether the 16 Field Explore samples should later receive tighter per-sample pipelines before promotion to controlled Good/Bad pairs.
- Tool View code-behind cleanup, single-input PropertyGrid shell layout:
  - Moved docked/floating density and layout mutation out of `VisionToolSingleInputPropertyToolShell.xaml.cs` into `VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs`.
  - The original shell file now keeps dependency properties, exposed controls, and the `DockedInspectorModeChanged` event path; `ApplyDockedInspectorMode` delegates to the layout controller.
  - `VisionToolSingleInputPropertyToolShell.xaml.cs` is now 189 lines; the extracted layout controller is 150 lines.
  - This is behavior-preserving: no algorithm tool PropertyGrid contract was changed, and no Preview/Run trigger path was added.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolShell.DockedInspectorLayoutController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_blob_tool artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_contour_tool_docked_verification artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\single_input_property_shell_layout_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, double-input custom shell layout:
  - Moved docked/floating preview-card density, input-B visibility, and offset action row layout out of `VisionToolDoubleInputCustomToolShell.xaml.cs` into `VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs`.
  - The shell file now keeps dependency properties, exposed controls, and public layout commands that delegate to the controller.
  - `VisionToolDoubleInputCustomToolShell.xaml.cs` is now 107 lines; the extracted layout controller is 121 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolShell.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolDoubleInputCustomToolShell.DockedInspectorLayoutController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\double_input_shell_layout_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Line result review controller:
  - Added `LineToolReviewController` to coordinate Line result chips and verification/failure guide updates.
  - `LineToolWpfView.xaml.cs` now delegates Line, Distance, and Intersection result review presentation plus teaching-summary reset to the controller.
  - `LineToolWpfView.xaml.cs` is now 407 lines; the extracted review controller is 102 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolReviewController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_tool_review_controller_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Arithmetic settings restore:
  - `ArithmeticToolInteractionController` now receives the current suppress state and preserves it while applying operation lists or persisted settings.
  - `ArithmeticToolWpfView.ApplyPersistedSettings` now delegates directly to the controller.
  - `ArithmeticToolWpfView.xaml.cs` is now 290 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\ArithmeticToolInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_controller_suppression_refactor_dev_20260703_01` passed.
- Pipeline/Recipe operator review UX, Step Flow operator focus:
  - Added a small `PipelineReviewStepFlowOperatorFocus` strip inside the Step Flow panel.
  - It reuses `ReviewGuideParameterFocusText`, so the selected NG step shows the operator/parameter location where the user is already choosing the step.
  - This does not add a Tool View launch command; no command surface is available there yet, so the safer improvement is clearer operator focus near the selected step.
  - Before capture: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_before_dev_20260703_01\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - After captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_operator_focus_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_operator_focus_after_dev_20260703_02` passed.
- MainView/Product sample review current-flow recheck:
  - Rechecked the current Dev build after Tool View and Pipeline Review changes.
  - The product sample workflow strip still shows the explicit counterpart/sample review actions and does not auto-run Preview/Run during open/counterpart switching.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_counterpart_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260703_02\wpf_shell_host_workspace_sample_product_field_focus_picker.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_field_focus_picker artifacts\mainview_product_flow_recheck_dev_20260703_02` passed.
  - Self-evaluation: no further Product sample workflow UI work is needed before stabilization. The next value is import/readiness review or additional Tool View cleanup, not more visible copy.
- Tool View code-behind cleanup, Threshold test configuration:
  - `VisionToolThresholdInteractionController` now owns the Basic/Invert test configuration path and preserves the previous suppress state while changing coupled radio buttons.
  - `ThresholdToolWpfView.ConfigureBasicInvertForTest` delegates to the controller.
  - `ThresholdToolWpfView.xaml.cs` is now 221 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolThresholdInteractionController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\threshold_controller_test_config_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\threshold_controller_test_config_refactor_dev_20260703_01` passed.
- Tool View code-behind cleanup, Morphology kernel binding flush:
  - Added `VisionToolKernelSizeController.FlushParameterBindings`.
  - `MorphologyToolWpfView` now delegates width/height binding flush to the kernel controller before creating properties or refreshing the summary.
  - `MorphologyToolWpfView.xaml.cs` is now 195 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\VisionToolKernelSizeController.cs"` passed with CRLF warnings only.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard artifacts\morphology_kernel_flush_controller_refactor_dev_20260703_01` passed.
- Dev stabilization checkpoint after Tool View/Pipeline Review/MainView loop:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Pipeline/Recipe operator review UX, first NG step navigation:
  - Added a manual `NG Step` button to Pipeline Review next to Previous/Next.
  - The button selects the first enabled step whose review result is NG after explicit `Run Review`; it does not trigger Preview/Run.
  - The button is disabled for OK-only review results and is exposed through shell-host test hooks.
  - The multi-step NG smoke now selects a later OK step, clicks the visible `btnFirstIssueStep` button, verifies that the first NG Threshold step is selected, and confirms no native Preview/Run count increase.
  - Before first-issue button capture:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_operator_focus_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
  - After captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_after_dev_20260703_02\wpf_shell_host_workspace_sample_pipeline_review_ng_metrics.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_after_dev_20260703_02\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_first_issue_navigation_dev_20260704_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_pipeline_review_ng_metrics artifacts\pipeline_review_first_issue_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\pipeline_review_first_issue_after_dev_20260703_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_first_issue_navigation_dev_20260704_01` passed.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs" "0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Line preview controller:
  - Added `LineToolPreviewController` to own Line tool debounced auto-preview scheduling, threshold teaching preview requests, and input ROI overlay refresh.
  - `LineToolWpfView.xaml.cs` now delegates preview/ROI state to the controller and is reduced from 407 lines to 369 lines.
  - This is behavior-preserving: Line still uses explicit Preview/Run requests, and property changes only schedule through the existing `VisionToolPropertyPreviewPolicy`.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_measure_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_line_intersection_tool artifacts\line_preview_controller_refactor_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\LineToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\LineToolPreviewController.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Arithmetic preview controller:
  - Added `ArithmeticToolPreviewController` to own debounced auto-preview scheduling and the Offset-mode vs normal Preview request split.
  - `ArithmeticToolWpfView.xaml.cs` now delegates preview scheduling to the controller and is reduced from 290 lines to 276 lines.
  - Behavior is unchanged: Offset mode still uses `Run Offset`, normal mode still uses `Run Preview`, and parameter changes go through the existing `VisionToolParameterChangeController`.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_preview_controller_refactor_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ArithmeticToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ArithmeticToolPreviewController.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, Threshold schedule wrapper removal:
  - Removed the View-local `ScheduleAutoPreview` wrapper from `ThresholdToolWpfView`.
  - Scheduling now goes directly through `VisionToolParameterChangeController` and `VisionToolDebouncedPreviewScheduler`, which already own suppress and loaded-state checks.
  - `ThresholdToolWpfView.xaml.cs` is now 212 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_basic_tool artifacts\threshold_schedule_simplification_dev_20260703_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_threshold_tool artifacts\threshold_schedule_simplification_dev_20260703_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, SimplePreprocess apply-settings ownership:
  - `SimplePreprocessParameterController.ApplySettings` now owns the post-restore `RefreshProgrammatic(notifyChanged: true)` path.
  - `SimplePreprocessToolWpfView.ApplyPersistedSettings` no longer manually raises `ParameterChanged`; the View is reduced from 285 lines to 283 lines.
  - `PipelineViewerScreenshotSmoke` combo/slider auto-preview mutations now choose a value different from the current persisted setting, so the smoke is not dependent on local settings store state.
  - Verification:
    - Initial `wpf_preprocess_output_preview_flow` run failed because the smoke re-selected an already persisted Filter/RotateScale value and did not trigger a change event.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors after the smoke fix.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_preprocess_output_preview_flow artifacts\simple_preprocess_apply_settings_controller_dev_20260703_03` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_apply_settings_controller_dev_20260703_03` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\simple_preprocess_layer_contract_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\Behaviors\SimplePreprocessParameterController.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Tool View code-behind cleanup, SimplePreprocess parameter facade removal:
  - `SimplePreprocessToolWpfView` now exposes its existing `SimplePreprocessParameterController` through an internal `Parameters` property instead of forwarding every `Add*`, `Get*`, visibility, settings capture, and settings restore method.
  - SimplePreprocess configurator/property/preview/factory code now uses `view.Parameters` directly, so parameter generation and mapping stay in the controller/runtime path.
  - `SimplePreprocessToolWpfView.xaml.cs` is now 180 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_preprocess_output_preview_flow artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_simple_preprocess_result_review artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_preprocess_existing_output_write artifacts\simple_preprocess_parameter_facade_refactor_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessDocumentFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessViewConfigurator.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPropertyFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPreviewExecutor.cs"` passed with CRLF warnings only.
- Arithmetic route smoke stabilization:
  - `wpf_layer_selection_arithmetic_tool` exposed a WPF `PopupControlService` stale HWND exception while docking the floating Arithmetic tool after combo popup checks.
  - The smoke runner now closes the Arithmetic combo popups before docking and ignores only Win32 error 1400 during dispatcher pump cleanup; behavioral assertions still fail normally from the verification action.
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_arithmetic_tool artifacts\arithmetic_popup_cleanup_stabilized_20260704_01` passed.
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessDocumentFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessViewConfigurator.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPropertyFactory.cs" "0. UI\0) MENU\Wpf\OpenVisionNativeSimplePreprocessPreviewExecutor.cs" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Dev stabilization checkpoint after first-issue navigation and Tool View preview-controller refactors:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Self-evaluation document refresh:
  - `docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md` now references the current Dev product catalog result: `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`.
  - The Product sample catalog score was adjusted from `4.2 / 5` to `4.3 / 5`.
  - Verification:
    - `git diff --check -- "docs\OPENVISIONLAB_SELF_EVALUATION_20260703.md"` passed.
- Product field-style sample catalog follow-up:
  - Confirmed `C:\Git\새 폴더` source images are represented in Dev as 16 renamed field-style images under `docs\samples\public\product\field`.
  - Current repo sample names/catalog rows do not include `ChatGPT` or `OpenAI` markers.
  - Visual contact sheet for review:
    - `C:\Git\OpenVisionLab_Dev\artifacts\field_sample_quality_review_20260704_01\field_sample_contact_sheet.png`
  - Verification:
    - `rg -n "ChatGPT|OpenAI|generated by" "docs\samples\public\product" "docs\samples\OpenVisionLab.ProductSampleCatalog.csv"` found only the README policy line describing deterministic/project-authored samples; no ChatGPT/OpenAI marker is present in catalog/image names.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_quality_followup_dev_20260703_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_full_recheck_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=62.722`.
- MainView/Product sample review current-flow recheck:
  - Rechecked current Dev build after Pipeline Review first-issue navigation, Tool View cleanup, and sample catalog follow-up.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_focus_picker.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\mainview_product_flow_recheck_dev_20260704_01\wpf_shell_host_workspace_sample_product_counterpart_open.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_picker artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_focus_open artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_sample_product_counterpart_open artifacts\mainview_product_flow_recheck_dev_20260704_01` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_20260704_01` passed with `Targets=6`.
- Latest Dev stabilization checkpoint after 2026-07-04 00:00 changes:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
- Final Dev recheck on 2026-07-04 before the 02:00 handoff:
  - `git diff --check` passed with CRLF warnings only.
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_final_20260704_01` passed with `Targets=6`.
  - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\final_ui_recheck_dev_20260704_01` passed; capture: `C:\Git\OpenVisionLab_Dev\artifacts\final_ui_recheck_dev_20260704_01\wpf_shell_host_pipeline_review_ng.png`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_full_final_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=61.536`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_full_final_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- Post-00:23 continuation, Pipeline Review progress summary:
  - Added a compact header progress line for Pipeline Review: `OK x / NG y / 대기 z`, plus `OFF z` when disabled steps exist.
  - The progress text is owned by `OpenVisionPipelineReviewDocument.FormatReviewProgressText`, surfaced through the Pipeline Review view/viewmodel, and exposed through ShellHost test hooks for smoke assertions.
  - During current-build visual verification, the first after-capture showed `실행 중...` still present after completion. The run completion path now recalculates progress after `isRunningReview=false` in the common `finally` block.
  - Current before/after captures:
    - Before: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_progress_before_20260704_01\wpf_shell_host_pipeline_review_ng.png`
    - After: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_review_progress_after_20260704_03\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_review_progress_after_20260704_03` passed and now asserts the progress text directly.
    - `git diff --check -- "0. UI\0) MENU\Wpf\Documents\OpenVisionPipelineReviewDocument.cs" "0. UI\0) MENU\Wpf\ViewModels\OpenVisionPipelineReviewViewModel.cs" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml" "0. UI\0) MENU\Wpf\Views\OpenVisionPipelineReviewView.xaml.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostStatePresenter.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostToolTestFacade.cs" "0. UI\0) MENU\Wpf\OpenVisionShellHostView.TestHooks.cs" "Library\OpenVisionLab.Localization\Resources\LocalizationCatalog.tsv" "tools\PipelineViewerScreenshotSmoke\Program.cs"` passed with CRLF warnings only.
- Post-00:44 continuation, Blob/Contour area review controller cleanup:
  - Added `VisionToolThresholdTeachingPreviewController` for the shared threshold-teaching preview request flag used by Blob and Contour.
  - Added a `VisionToolSingleInputPropertyToolController<TProperty>.ShowAreaResultReview(...)` overload that pairs verification guide update with the existing area result review presenter.
  - `BlobToolWpfView.xaml.cs` and `ContourToolWpfView.xaml.cs` now delegate the duplicated result-list filtering and teaching-preview state to shared runtime/controller code.
  - Code-behind line counts after cleanup: Blob 161, Contour 159.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_blob_tool artifacts\area_review_controller_refactor_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_contour_tool artifacts\area_review_controller_refactor_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\BlobToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ContourToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputPropertyToolController.cs"` passed with CRLF warnings only.
    - `Select-String -LiteralPath "0. UI\6) Vision Test\Wpf\VisionToolThresholdTeachingPreviewController.cs" -Pattern '[ \t]+$'` found no trailing whitespace.
- Post-tool-review stabilization checkpoint:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_post_tool_review_20260704_01 -SkipRunnerBuild` completed with summary `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=68.489`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_post_tool_review_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- MainView/Product sample review post-progress recheck:
  - Rechecked the sample-selection-to-product-review flow after the Pipeline Review progress summary change.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_sample_product_focus_open.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_review_ng.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\sample_review_ui_smoke_post_progress_20260704_01\wpf_shell_host_workspace_product_sample_pair_open.png`
  - Visual check: the header progress line does not overlap the Good/Bad pair guide; it shows `미실행` before review and `OK 1 / NG 1 / 대기 0` after the controlled NG product review run.
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunSampleReviewUiSmokes.ps1 -OutputDir artifacts\sample_review_ui_smoke_post_progress_20260704_01` passed with `Targets=6`.
- Product sample review progress assertion hardening:
  - Added `AssertPipelineReviewProgressText` in `PipelineViewerScreenshotSmoke` and wired it into Product sample review OK/NG paths.
  - The smoke now verifies the visible progress summary counts and also checks that `실행 중...` does not remain after review completion.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review artifacts\product_review_progress_assertion_20260704_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_progress_assertion_20260704_02` passed.
    - `git diff --check -- tools\PipelineViewerScreenshotSmoke\Program.cs` passed with CRLF warnings only.
- Matching-family result review title cleanup:
  - `VisionToolSingleInputMatchingToolController<TProperty>` now owns the result review title supplied at attach time.
  - Matching, EdgeBasedMatching, and FeatureMatching views no longer pass the same title string on every `SetResultReview` call.
  - Code-behind line counts after cleanup: Matching 142, EdgeBasedMatching 137, FeatureMatching 133.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_edge_based_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_feature_matching_tool artifacts\matching_review_title_controller_20260704_01` passed.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\MatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\EdgeBasedMatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\FeatureMatchingToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\VisionToolSingleInputMatchingToolController.cs"` passed with CRLF warnings only.
- Post-matching-cleanup stabilization checkpoint:
  - Verification:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `git diff --check` passed with CRLF warnings only.
- Single-input custom tool shell base refactor:
  - Added `VisionToolSingleInputCustomToolViewBase` to own the repeated single-input custom tool shell/event/status/preview-image command forwarding.
  - Switched `ThresholdToolWpfView`, `FilterToolWpfView`, `MorphologyToolWpfView`, and `SimplePreprocessToolWpfView` XAML roots from `UserControl` to the shared base so the generated WPF partial classes share the same controller plumbing.
  - Removed repeated forwarding code from the four code-behind files while keeping each tool's parameter UI, presenter/controller setup, and explicit Preview/Run path unchanged.
  - Code-behind line counts after this cleanup: Filter 121, Morphology 116, Threshold 129, SimplePreprocess 86. Shared base is 165 lines.
  - Verification:
    - `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_filter_morphology_layout_guard,wpf_shell_host_threshold_tool,wpf_simple_preprocess_result_review artifacts\custom_tool_base_refactor_20260704_02` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_layer_selection_threshold_tool,wpf_preprocess_output_preview_flow,wpf_layer_selection_preprocess_existing_output_write artifacts\custom_tool_base_refactor_route_20260704_01` passed.
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
    - `git diff --check` passed with CRLF warnings only.
    - `git diff --check -- "0. UI\6) Vision Test\Wpf\VisionToolSingleInputCustomToolViewBase.cs" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\FilterToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\MorphologyToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\ThresholdToolWpfView.xaml.cs" "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml" "0. UI\6) Vision Test\Wpf\SimplePreprocessToolWpfView.xaml.cs"` passed with CRLF warnings only.
  - Current captures:
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_filter_morphology_layout_guard.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_shell_host_threshold_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_20260704_02\wpf_simple_preprocess_result_review.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_layer_selection_threshold_tool.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_preprocess_output_preview_flow.png`
    - `C:\Git\OpenVisionLab_Dev\artifacts\custom_tool_base_refactor_route_20260704_01\wpf_layer_selection_preprocess_existing_output_write.png`
- Pipeline Review flow status badge improvement:
  - `OpenVisionPipelineReviewDocument.ResolveFlowStatus` now maps completed successful review steps to `Passed` and NG/acceptance-NG review steps to `Failed`.
  - This uses the existing `PipelineFlowView` OK/NG badge colors instead of leaving completed review rows as generic `LOAD/WAIT`, making the left Step Flow usable as an operator review map.
  - UI evidence:
    - Before capture with the old LOAD/WAIT mapping: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_flow_status_before_20260704_01\wpf_shell_host_pipeline_review_ng.png`
    - After capture with the NG flow badge visible: `C:\Git\OpenVisionLab_Dev\artifacts\pipeline_flow_status_after_20260704_01\wpf_shell_host_pipeline_review_ng.png`
  - Verification:
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_flow_status_before_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_pipeline_review_ng artifacts\pipeline_flow_status_after_20260704_01` passed.
    - `dotnet run --project tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -- --target wpf_shell_host_workspace_product_sample_review_ng artifacts\product_review_flow_status_after_20260704_01` passed.
  - Product sample review recheck capture:
    - `C:\Git\OpenVisionLab_Dev\artifacts\product_review_flow_status_after_20260704_01\wpf_shell_host_workspace_product_sample_review_ng.png`
- Product catalog recheck after Tool/Pipeline UX changes:
  - Verification:
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\RunVisionSampleCatalog.ps1 -CatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -OutputDir artifacts\product_catalog_after_base_and_flow_20260704_01 -SkipRunnerBuild` passed.
    - Summary: `GateStatus=OK`, `RunnableRows=184`, `RequiredRows=84`, `ExploreRows=16`, `ExpectedFailureRows=84`, `OKRows=184`, `NGRows=0`, `DurationSeconds=63.456`, `ArtifactIssueCount=0`, `MetadataIssueCount=0`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\AuditProductSampleQuality.ps1 -SummaryPath artifacts\product_catalog_after_base_and_flow_20260704_01\sample_catalog_summary.json -FailOnCritical` passed with `PairRecords=84`, `OK=84`, `Review=0`, `Critical=0`.
- Final Dev verification checkpoint at 2026-07-04 01:21 +09:00:
  - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
  - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` passed with `CatalogRows=16`, `ManifestAssets=214`, `Pipelines=8`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
  - `git diff --check` passed with CRLF warnings only.
- Custom tool extension guide update:
  - `docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md` now names `VisionToolSingleInputCustomToolViewBase`, `AttachToolController(...)`, and the rule that single-input custom UI views must not copy event/status/preview forwarding or call `VisionToolSingleInputCustomToolRuntime` directly.
  - Verification:
    - `git diff --check -- docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md` passed with CRLF warnings only.
- Original repo recheck at 2026-07-04 01:21 +09:00:
  - `git fetch origin` completed in `C:\Git\OpenVisionLab`.
  - Original remains dirty with a subset of earlier sample/catalog/Pipeline Review/Tool View changes and field sample files.
  - Latest original commits remain:
    - `e11b724 Record pipeline review parameter focus hints`
    - `9c2bbe1 Show pipeline review parameter focus hints`
    - `c90d60a Record pipeline review parameter location hints`
    - `2371b37 Add pipeline review parameter location hints`
    - `bc42e0e Record pipeline review label polish`
  - No Dev-to-Original import was performed for the latest custom tool base refactor or Pipeline Review flow status badge change.
- Original repo status check:
  - `C:\Git\OpenVisionLab` is already dirty with a subset of sample/catalog/Pipeline Review/Tool View changes.
  - No bulk copy from Dev was performed in this checkpoint.
  - `git fetch origin` completed with no output.
  - Dev changes not yet present in Original include Pipeline Review first-issue navigation and the latest Tool View preview-controller cleanup. Import these later as reviewed patch groups, not as a bulk folder copy.
  - Verification in `C:\Git\OpenVisionLab`:
    - `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` passed with 0 warnings and 0 errors.
    - `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab"` passed.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1 -PublicCatalogPath docs\samples\OpenVisionLab.ProductSampleCatalog.csv -ManifestPath docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv` passed with `CatalogRows=184`, `ManifestAssets=214`, `Pipelines=87`.
    - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` passed.
- Original repo recheck after the post-00:23 Dev continuation:
  - `git fetch origin` completed.
  - `C:\Git\OpenVisionLab` remains dirty with a subset of earlier sample/catalog/Pipeline Review changes and field sample files.
  - Latest original commits:
    - `e11b724 Record pipeline review parameter focus hints`
    - `9c2bbe1 Show pipeline review parameter focus hints`
    - `c90d60a Record pipeline review parameter location hints`
    - `2371b37 Add pipeline review parameter location hints`
    - `bc42e0e Record pipeline review label polish`
  - No Dev-to-Original import was performed for the latest Pipeline Review progress summary, Blob/Contour cleanup, Product review progress smoke assertion, or Matching-family title cleanup.
  - Import only by reviewed patch groups after choosing the target group; do not copy the Dev tree over Original.

## Dev To Original Import Groups

Do not import the whole Dev tree. Review and move the current Dev changes in small groups:

1. Product field sample catalog
   - Candidate files: `docs\samples\OpenVisionLab.ProductSampleCatalog.csv`, `docs\samples\public\product\OpenVisionLab.ProductSampleManifest.csv`, `docs\samples\public\product\field\*`, `docs\samples\public\product\Product_Field_*.pipeline.xml`, `0. UI\6) Vision Test\VisionPipelineSampleCatalog.cs`, and sample generation/policy docs.
   - Required checks after import: product asset policy, full product catalog gate, sample quality audit.
2. MainView/Product sample review UX
   - Candidate files: `OpenVisionShellHostSampleWorkflowPresenter`, sample picker view/viewmodel, sample focus/pair decision helpers, shell command surface, and related UI smoke updates.
   - Required checks after import: `tools\RunSampleReviewUiSmokes.ps1` and current-build screenshots.
3. Pipeline Review operator UX
   - Candidate files: Pipeline Review document/view/viewmodel, guide/result presenters, localization keys, shell host test hooks, and PipelineViewerScreenshotSmoke updates.
   - Required checks after import: `wpf_shell_host_pipeline_review_ng`, product sample review target, and no Preview/Run count increase on `NG Step`.
4. Tool View controller cleanup
   - Candidate files: preview/review/text/controller classes and the touched Tool View code-behind files.
   - Required checks after import: focused Tool View WPF smokes for Threshold, Line, Arithmetic, SimplePreprocess, Filter/Morphology.
5. Policy/runtime cleanup
   - Candidate files: external reference/readiness scripts, project references, native DLL placement, and policy docs.
   - Required checks after import: solution build, readiness, external references, public sample assets.

## Start Checklist

```powershell
cd C:\Git\OpenVisionLab_Dev
git status --short
git log --oneline -5

cd C:\Git\OpenVisionLab
git fetch origin
git status --short
git log --oneline -5
```

## Next Priorities

1. LLM XML failure corpus and replayable validation scenarios
   - Completed first replayable set: malformed XML, missing input layer, unsupported tool, missing dependency file, invalid parameter values, score percentage misuse, missing Arithmetic InputLayerB, and bad-draft-to-corrected-import loop.
   - Next value: add real external LLM transcript examples and tool-specific parameter compatibility cases only when they expose gaps beyond the current validation/correction loop.
   - Keep the workflow explicit: paste/load draft, validate, review diff/dependencies, then import. Do not add auto Preview/Run.
2. Recipe Manager density/layout follow-up
   - Use current-build screenshots to fix only actual clipping, overlap, or workflow friction.
3. Pipeline/Recipe operator review UX polish
   - NG next action, NG triage, rerun/comparison actions, corrected-output review, and selected Step branch/output comparison now exist. Next value is broader multi-step recipe navigation clarity only where current EXE evidence shows operator friction.
4. Tool View code-behind cleanup
   - Filter/Morphology preset handling, Threshold/Line test configuration, Line ROI/default-ROI mutation, Line result review coordination, SimplePreprocess/Arithmetic setting restore, and single/double-input tool shell layout have moved into controllers/presenters. Continue only where existing controller/runtime patterns already fit; avoid broad base-class or interface refactors.
5. Product sample catalog quality
   - Existing 84-pair audit and full 184-row catalog gate are PASS in Dev after the field import and current review UX changes. Do not add more samples until the current UX/runtime changes are stabilized.
6. Product field-style sample follow-up
   - Keep the imported field samples as recipe setup examples unless tighter per-sample pipelines are added for deliberate Good/Bad pair promotion.

## Cautions

- UI/UX changes require fresh current-build before/after screenshots. Do not reuse old screenshots.
- `PipelineViewerScreenshotSmoke` can hang when multiple WPF targets are run in one process. Use `tools\RunSampleReviewUiSmokes.ps1` or single-target runs.
- Do not run WPF smoke targets in parallel; `OpenCvSharpExtern.dll` lock warnings can appear.
- Do not bulk-copy Dev into Original.
- Do not restore GitHub Desktop stashes unless the user explicitly asks.
- Do not reintroduce SDK sample assets or `dll\Library-Noah\OpenCvSharpExtern.dll` into public paths.
