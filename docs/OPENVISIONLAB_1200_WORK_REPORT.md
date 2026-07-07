# OpenVisionLab 12:00 Work Report

Updated: 2026-06-16 11:20 KST

## Scope

This pass follows the current 1~7 priority queue:

1. Write practical scenario validation checklist.
2. Validate Main/Pipeline/Tool flow against that checklist.
3. Fix only UX/contract issues found during validation.
4. Check WPG common editor surface.
5. Document AI Recipe interactive correction flow.
6. Document Library-Noah and WPG-CUSTOM reference policy.
7. Update README/tutorial path so users can learn the program from inside the platform.

## Completed

- Added `docs/OPENVISIONLAB_SCENARIO_VALIDATION.md`.
  - Defines expected behavior for Main Workspace, Tool Forms, Pipeline authoring, Preview/Publish, persistence, sample catalog, and external runner.
- Added `docs/OPENVISIONLAB_TUTORIAL.md`.
  - Explains the OpenVisionLab workflow from image load to Threshold, Pipeline, Preview, Publish, Sample Catalog, AI Recipe, and XML runner use.
- Added `docs/OPENVISIONLAB_TUTORIAL.html`.
  - Provides a user-facing image tutorial that can be opened directly from the program.
  - Uses local tutorial screenshots under `docs/assets/tutorial`.
- Added `docs/OPENVISIONLAB_AI_RECIPE_INTERACTIVE_EDIT_PLAN.md`.
  - Defines the target flow for first-failed-step focus, patch proposal, editable correction, and re-preview.
- Added `docs/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`.
  - Defines how `Library-Noah` and `WPG-CUSTOM` should be referenced and released.
- Updated `README.md`.
  - Added tutorial/operation document links.
  - Clarified quiet UI precheck behavior.
  - Clarified external reference policy.
  - Added note that the `Guide` menu opens the tutorial.
- Added Main `Guide` menu.
  - Opens `docs/OPENVISIONLAB_TUTORIAL.html` from the running program.
  - Falls back to `docs/OPENVISIONLAB_TUTORIAL.md` if the HTML file is missing.
  - Searches upward from current directory and application base directory so Debug output can still find repository docs.
- Added Pipeline `More > Open Tutorial...` as a second in-program tutorial entry point for users editing Step flow.
- Added remaining recursive sample representatives.
  - `EasyMatrixCode_AutoRead_Contour` covers `Sample\EasyMatrixCode`.
  - `EasyOCR2_Characters_Contour` covers `Sample\EasyOCR2`.
  - Both are generic image-processing Explore rows, not decoder/OCR recognition contracts.
- Adjusted Main toolbar menu width.
  - Prevents the menu strip from staying sized for the old two-menu layout after adding `Guide` and runtime `보기`.
- Extended UI smoke text collection.
  - ToolStrip item text is now included in control text collection.
  - Main workspace smoke now verifies that `Guide` is present.
  - Main workspace smoke now verifies that `OPENVISIONLAB_TUTORIAL.html` resolves from the running program.
  - Main workspace smoke now verifies that the referenced tutorial screenshot assets exist.
- Extended the in-program tutorial with tool-specific test guidance.
  - Added Contour, Blob, Pattern Matching, EdgeDetection, LineGauge, and distance/Pixel-mm measurement guide entries.
  - The guide explains which sample to open, which pipeline/tool flow to run, and which output image, overlay, metric, and log fields should be checked.
- Strengthened Sample Catalog metric gates.
  - LLM OverlayMerge now checks merged overlay count and source count.
  - Blob, BentPin, DiePad, LineGauge, Matching, EasyGauge, and EasyMatch representatives now use multi-metric gates where stable metrics exist.
  - Pipeline Samples and AI Recipe prompt contracts now expose expected range, actual value, and judgment text.
- Added Pixel/mm measurement metric contracts.
  - Rectangle-overlay tools can expose `BoundsWidthMm*` and `BoundsHeightMm*` from `PIXELPERMM`.
  - Line-overlay tools can expose `LineLengthMm*` from `PIXELPERMM`.
  - Acceptance presets cover rectangle width/height in px and mm, plus fitted line length in px and mm.
  - BentPin shaft samples validate px/mm shaft width plus result count.
  - Pins LineGauge samples validate edge count, fitted-line length in pixels/mm, and angle.
- Strengthened the Matching rotated fixture smoke contract.
  - The fixture now uses a more asymmetric template so the angle-search contract does not pass or fail because a weak 0-degree candidate wins by accident.
  - This change is limited to the smoke fixture; Library-Noah MatchingTool behavior and CVBlob DLL version were not changed.
- Improved sample learning guidance.
  - `RecipeGuideText` now translates expected metrics into practical check points such as object count, object width in px/mm, fitted line length/angle, matching score, mean brightness, and output size.
  - Pipeline Samples and AI Recipe share this guidance path, so the user and LLM prompt both see the same expected review intent.

## Verification

Scoped quiet UI precheck:

- Command: `tools/RunUiPrecheck.ps1 -Targets "main_workspace,pipeline_form,pipeline_form_branch,threshold_form"`
- Capture mode: quiet offscreen render.
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_ui_quiet\ui_precheck_report.md`
- Result:
  - `main_workspace`: OK
  - `pipeline_form`: OK
  - `pipeline_form_branch`: OK
  - `threshold_form`: OK

Guide documentation contract:

- Command: `tools/RunUiPrecheck.ps1 -Targets "main_workspace"`
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_html_guide_contract\ui_precheck_report.md`
- Result: OK
- Contract:
  - Main menu includes `Guide`.
  - Runtime documentation resolver finds `docs/OPENVISIONLAB_TUTORIAL.html`.
  - HTML tutorial references image assets that exist in `docs/assets/tutorial`.

Initial non-UI platform precheck before sample coverage:

- Command: `tools/RunVisionPlatformPrecheck.ps1 -SkipUi`
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_skipui\platform_precheck_report.md`
- Result: OK
- Gates:
  - Build: OK
  - XML Compatibility: OK
  - Sample Catalog Runner: OK
  - Sample Catalog Summary: OK
  - Runner API Contract: OK
  - Tool Result Contract: OK
  - Sample Inventory And Algorithm Contract: OK
- Sample Catalog:
  - Runnable rows: 34
  - Required rows: 18
  - Explore rows: 16
  - OK rows: 34
  - NG rows: 0
  - Uncovered backlog folders at that point: `EasyMatrixCode`, `EasyOCR2`

Sample coverage completion check:

- Command: `tools/RunVisionSampleCatalog.ps1`
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_catalog_complete\sample_catalog_report.md`
- Result: OK
- Sample Catalog:
  - Runnable rows: 36
  - Required rows: 18
  - Explore rows: 18
  - OK rows: 36
  - NG rows: 0
  - Uncovered sample folders: 0

Non-UI platform precheck after sample coverage:

- Command: `tools/RunVisionPlatformPrecheck.ps1 -SkipUi`
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_sample_complete_skipui\platform_precheck_report.md`
- Result: OK
- Sample Catalog:
  - Runnable rows: 36
  - Required rows: 18
  - Explore rows: 18
  - OK rows: 36
  - NG rows: 0
  - Uncovered sample folders: 0

Sample Catalog UI check after sample coverage:

- Command: `tools/RunUiPrecheck.ps1 -Targets "pipeline_samples_form,pipeline_samples_check_action,pipeline_sample_open_preview,pipeline_sample_llm_open_preview"`
- Capture mode: quiet offscreen render.
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_ui_complete\ui_precheck_report.md`
- Result:
  - `pipeline_samples_form`: OK
  - `pipeline_samples_check_action`: OK
  - `pipeline_sample_open_preview`: OK
  - `pipeline_sample_llm_open_preview`: OK

Sample Catalog backlog-none UI contract:

- Command: `tools/RunUiPrecheck.ps1 -Targets "pipeline_samples_check_action"`
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_backlog_none_contract\ui_precheck_report.md`
- Result: OK
- Contract:
  - Sample check details include `Catalog coverage`.
  - Sample check details show `Backlog: none`.

Full platform precheck with quiet UI:

- Command: `tools/RunVisionPlatformPrecheck.ps1`
- Capture mode: quiet offscreen render.
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\platform_precheck_report.md`
- UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\ui\ui_precheck_report.md`
- Result: OK
- Gates:
  - Build: OK
  - XML Compatibility: OK
  - Sample Catalog Runner: OK
  - Sample Catalog Summary: OK
  - Runner API Contract: OK
  - Tool Result Contract: OK
  - Sample Inventory And Algorithm Contract: OK
  - UI Precheck: OK
- UI targets:
  - `main_workspace`: OK
  - `pipeline_form`: OK
  - `pipeline_form_branch`: OK
  - `pipeline_designable_forms`: OK
  - `pipeline_add_step_form`: OK
  - `pipeline_add_step_branch_form`: OK
  - `pipeline_property_grid_contract_check`: OK with visual `WARN` only
  - `log_panel_contract_check`: OK
  - `pipeline_sample_open_preview`: OK
  - `pipeline_sample_llm_open_preview`: OK
  - `threshold_form`: OK
  - `ai_recipe_form`: OK

Latest strengthened sample metric check:

- Command: `tools/RunVisionSampleCatalog.ps1`
- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_tool_guide_metrics\sample_catalog_report.md`
- Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_tool_guide_metrics\sample_catalog_summary.json`
- Result: OK
- Scope:
  - 36 runnable sample rows.
  - Required and Explore samples all passed after strengthening count/area, count/bounds, line, matching, and merge metrics.

Focused tutorial and sample metric UI contracts:

- Pipeline Samples metric review UI: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_metric_review\ui_precheck_report.md`
- Pipeline Samples catalog run UI: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_metric_report\ui_precheck_report.md`
- AI Recipe sample-gate prompt: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_sample_gate_prompt\ui_precheck_report.md`
- Tool guide contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_guide_contract\ui_precheck_report.md`
- Final focused UI smoke after strengthened catalog update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_guide_metric_ui_final\ui_precheck_report.md`
- Pipeline Samples recipe-guide detail smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_recipe_guide_contract\ui_precheck_report.md`

Focused Pixel/mm measurement contracts:

- Sample Catalog runner: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_contract\sample_catalog_report.md`
- Sample Catalog summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_contract\sample_catalog_summary.json`
- Pipeline Samples measurement UI: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_ui_contract\ui_precheck_report.md`
- AI Recipe feedback wait-fix smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_feedback_wait_fix\ui_precheck_report.md`
- Non-UI platform precheck: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_platform_skipui\platform_precheck_report.md`
- Non-UI platform summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_platform_skipui\platform_precheck_summary.json`
- Matching rotated fixture focused smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_matching_rotated_fixture_fix\ui_precheck_report.md`
- Sample Check guide and AI Recipe feedback smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract2\ui_precheck_report.md`
- Final non-UI platform precheck: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_docs_final_skipui2\platform_precheck_report.md`
- Final non-UI platform summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_docs_final_skipui2\platform_precheck_summary.json`
- Final non-UI platform precheck after Sample Check guide contract update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract_final_skipui\platform_precheck_report.md`
- Final non-UI platform summary after Sample Check guide contract update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract_final_skipui\platform_precheck_summary.json`

## Current Assessment

- Main/Pipeline/Tool operating contract: 98%.
- Pipeline preview/publish separation: 97%.
- Sample catalog and runner validation: 99%.
- Tool result/error-code contract: 96%.
- Main/Pipeline/Threshold UI polish: 96%.
- Result metrics and measurement contracts: 95%.
- Sample learning/guide UX: 94%.
- WPG editor commonization: 90%.
- AI Recipe interactive editing: 84%.
- In-program learning/accessibility: 93%.

## Remaining Work

1. Finish WPG common editor consolidation.
   - Shared Threshold/Range editor behavior should live in the common WPG/control path.
   - Keep WinForms Threshold form designer-friendly.
2. Build AI Recipe interactive patch UI.
   - Convert text patch proposal into editable parameter/layer-flow actions.
   - Preserve successful previous steps during retry.
3. Add tutorial entry points beyond Main `Guide`.
   - Pipeline Samples can show a short recipe guide per sample.
   - AI Recipe can link to the recipe contract/tutorial.
   - Tool-specific guide content now exists; the remaining work is deeper in-app linking from each tool/sample.
4. Add paired OK/NG sample contracts where stable metrics exist.
   - Defect-oriented samples should expose a clear decision metric rather than only overlay count.
   - Some stronger multi-metric gates are now in place; Pixel/mm contracts now exist for representative bounds and line samples.
   - Remaining measurement work is calibration UX, dedicated measure-tool samples, and more OK/NG paired measurement scenarios.
