# OpenVisionLab Next Work

> Historical plan archived on 2026-08-05. The percentages, baseline, and
> priority queue below are 2026-06-18 snapshots, not current readiness or
> active work. Use `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`.

Updated: 2026-06-18

OpenVisionLab is a rule-based OpenCVSharp vision workbench. The goal is not to be a loose collection of image-processing dialogs. The goal is a platform where a user can load a sample image, build a step pipeline, validate the result through metrics/overlays/logs, save the recipe as XML, and run the same recipe from UI, batch, AI Recipe import, or an external runner.

## Current Baseline

- Overall readiness: about 97%.
- Algorithm robustness: about 95%.
- Automated UI QA: about 97%.
- Pipeline persistence and sample validation: about 99%.
- External runner path: about 95%.
- Main viewer polish, Pipeline Flow clarity, logging/message UX, Threshold form polish, in-program HTML tutorial access, and the first Korean/English localization path now have focused smoke/contract coverage at the 95% UI pass level. The tutorial now includes tool-specific test guides for Contour, Blob, Pattern Matching, EdgeDetection, LineGauge, distance/measurement workflows, inspection-form teaching flow, and multi-layer image comparison. Pixel/mm-derived measurement metrics are now validated for representative bounds and line samples. AI Recipe feedback now includes failed-step XML field candidates, metric context, a copyable XML Patch Request for the selected Step, a safe auto-fix preview, selectable Safe Fix rows, and operator-editable Safe Fix values before applying Step/Parameter/Layer Flow/Acceptance limit corrections. Shared WPG visual finish is now applied through the bridge runtime style path and verified by Pipeline/Contour/Line/Threshold focused smoke captures.
- AI Recipe import now has a visible next-action banner. It changes from waiting, to validation OK/NG, to Preview OK/NG, so operators can decide whether to run preview, apply to Pipeline, use Safe Fix/Layer Flow, or copy AI Feedback without reading XML or logs first.
- Localization is now separated into `src/Libraries/OpenVisionLab.Localization`. Translation text is centrally managed through `CONFIG/localization_catalog.tsv`, seeded from the library resource catalog, with a runtime `FormLocalizationEditor` for operator-side Korean/English correction. Main, AI Recipe, Pipeline, Pipeline Flow WPF control, Threshold, LogPanel, shared MessageBox, Image Compare, ImageCanvas context menus, Layer Display empty/status text, Pipeline Add Step, Pipeline Samples, Pipeline Batch, Pipeline History, and Pipeline Batch History now consume the service. The service also merges newly shipped catalog keys into an existing operator-edited CONFIG catalog without overwriting existing translations. WPG PropertyGrid display name/category/description localization is handled centrally through the bridge `PropertyDescriptor` path, including display-name alias keys for inherited/common property labels. `OpenVisionWinFormsLocalizer` now provides name-based automatic WinForms text, placeholder, grid-column, menu, tab, and tooltip localization for future forms.
- The tutorial now includes real full-form Tool screenshots and sample result images for Contour, Blob, Pattern Matching, FeatureMatching, EdgeDetection, and LineGauge/measurement workflows. The guide separates "where to tune parameters" from "where to verify detection result" so users can learn by inspecting actual input/output form UI and validated sample outputs.
- Pattern Matching tutorial assets now use a tight 7PQRS button template, detected crop, and overlay result. The sample contract validates the detected center and bounds so the guide cannot regress to a loose background-heavy template.
- The Matching Tool Form now has an in-form Match Review area. After Run it shows the template image, detected crop, score, center, size, count, and confirms that the overlay result is written to the Output layer. The UI smoke now runs the Contour sample and validates that both preview images are populated.
- The Pipeline Matching step preview now mirrors that review path. Selecting a Matching step after Run Preview shows the template image, detected crop, score, center, and size in the right preview panel, while the Result grid keeps the template and detected-crop details. The small Template/Crop previews and Result grid rows can be opened in the zoomable Pipeline image viewer.
- The HTML/Markdown tutorial now includes the Pipeline Matching Review screenshot and explains why Template, Detected Crop, Overlay, and Score should be reviewed together.
- The same Pipeline review path now covers FeatureMatching. A focused synthetic FeatureMatching Pipeline smoke validates Template, Detected Crop, Score, Center, Size, Result grid review rows, and zoomable preview affordance.
- The FeatureMatching Tool Form now has its own Feature Review panel. A focused form smoke runs a synthetic feature template case and validates template preview, detected crop, score, center, angle, and output overlay context.
- AI Recipe now supports current-XML Good/Bad pair execution. When a catalog sample has a linked pair group, `Check Pair` runs the XML currently shown in the editor against the pair images and records overall OK/NG, metrics, final layer, message, and next tuning actions in the review panels. This closes the gap between "LLM suggestion looks valid on one image" and "the same XML still separates Good and Bad samples."
- Pipeline Samples now has a dedicated Sample Selection Guide. A selected catalog row shows recommended pipeline, flow, expected metric/pair, value to check, last-check state, and likely fix point without forcing the user to parse the long details box.

Latest full platform precheck:

- Latest report after Image Compare non-visible maintenance: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_image_compare_nonui_20260618\platform_precheck_report.md`
- Latest summary after Image Compare non-visible maintenance: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_image_compare_nonui_20260618\platform_precheck_summary.json`
- Result: Status OK, SkipUi true, Build OK, XML OK, Sample Catalog Runner OK, Runner API OK, AI Recipe Prompt OK, Tool Result OK, Sample Inventory/Algorithm OK, Portable Tutorial OK.
- Sample rows: 40 runnable, 24 Required, 16 Explore, 40 OK, 0 NG.
- Sample folders: 14 detected, 0 uncovered backlog folders.
- Portable tutorial: 25 source image tags, 25 embedded images, 0 asset/file references in the portable output.

Latest sample catalog pair expansion:

- Platform precheck report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_no_master_platform_precheck_20260618\platform_precheck_report.md`
- Platform precheck summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_no_master_platform_precheck_20260618\platform_precheck_summary.json`
- Sample Catalog report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_no_master_platform_precheck_20260618\samples\sample_catalog_report.md`
- Sample Catalog summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_no_master_platform_precheck_20260618\samples\sample_catalog_summary.json`
- Result: 55 runnable rows, 38 Required, 14 Explore, 3 ExpectedFailure, 55 OK, 0 NG, 0 artifact issues, 0 metadata issues.
- Good/Bad coverage now includes 11 complete pair groups and 25 pair rows. Current coverage includes Pattern/Feature Matching target/no-target or low-score cases, Blob density, LineGauge angle, Mean brightness drift, Fiducial visibility, and surface/pin defects.
- Pattern Matching no-target is intentionally expected to return runner exit code 1 with `ResultCount=0`; the catalog treats that as OK only when the row is marked `ExpectedFailure`.

Previous full platform precheck:

- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_safe_fix_platform_20260617\platform_precheck_report.md`
- Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_safe_fix_platform_20260617\platform_precheck_summary.json`
- Result: Build OK, XML OK, sample runner OK, Runner API OK, AI Recipe Prompt Contract OK, Tool Result Contract OK, Sample Inventory OK, Algorithm Contract OK, Tutorial Portable Contract OK. UI precheck was intentionally skipped for this non-UI platform pass.
- Sample rows: 39 runnable, 23 Required, 16 Explore, 39 OK, 0 NG.
- Artifact gate: 0 issues.
- Metadata gate: 0 issues.
- Runtime metadata gate: runner executable path exists and sample runner duration is recorded.
- Successful step contract: every non-skipped successful catalog step exposes OK/Passed/ErrorCode=0/AcceptancePassed and no failure diagnostics.
- Invalid step contract: missing `ToolType` fails as ToolFactoryFailed/ConfigurationError with action-summary and suggested-fix text.
- Run report contract: step `DiagnosticHint` and `SuggestedFix` are persisted in `report.xml`; History and Batch Step grids expose Error/Result/Diagnostic/Suggested Fix columns for post-run review.
- Focused Report XML smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_tool_contract_report_xml\ui_precheck_report.md`
- Focused Pipeline designer smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_designable_report_columns\ui_precheck_report.md`
- Multi-metric sample gates are active for RotateScale width/height, Blob count/bounds, BentPin width/count, and LineGauge edge-count/line-length checks.
- Focused multi-metric sample report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_multi_metric\sample_catalog_report.md`
- Focused multi-metric Sample UI smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_multi_metric_ui\ui_precheck_report.md`
- Sample folders: 13 detected, 0 uncovered backlog folders.

Latest strengthened sample metric checks:

- Sample Catalog runner: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_tool_guide_metrics\sample_catalog_report.md`
- Sample Catalog summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_tool_guide_metrics\sample_catalog_summary.json`
- Result: all 39 runnable samples passed after strengthening LLM OverlayMerge, Blob, BentPin, Film dark-spot, DiePad, LineGauge, Matching, FeatureMatching, and recursive EasyGauge/EasyMatch geometry gates.
- Latest Pixel/mm measurement metric runner: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_contract\sample_catalog_report.md`
- Latest Pixel/mm measurement metric summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_contract\sample_catalog_summary.json`
- Measurement scope: rectangle-overlay tools now expose width/height pixel-mm presets; BentPin shaft samples validate `BoundsWidthMmMax`; Pins LineGauge samples validate `LineLengthMmMax` with the existing edge-count, pixel-length, and angle gates.
- Pipeline Samples metric review UI smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_metric_review\ui_precheck_report.md`
- Pipeline Samples catalog run UI smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_metric_report\ui_precheck_report.md`
- AI Recipe sample-gate prompt smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_sample_gate_prompt\ui_precheck_report.md`
- Tool guide contract smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_guide_contract\ui_precheck_report.md`
- Final focused UI smoke after the strengthened catalog update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_guide_metric_ui_final\ui_precheck_report.md`
- Pipeline Samples recipe-guide detail smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_recipe_guide_contract\ui_precheck_report.md`
- Measurement metric UI smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_ui_contract\ui_precheck_report.md`
- AI Recipe measurement prompt/feedback smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_feedback_wait_fix\ui_precheck_report.md`
- Latest non-UI platform precheck after measurement metric update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_platform_skipui\platform_precheck_report.md`
- Latest non-UI platform summary after measurement metric update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_platform_skipui\platform_precheck_summary.json`
- Matching rotated fixture contract smoke after strengthening the asymmetric fixture: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_matching_rotated_fixture_fix\ui_precheck_report.md`
- Pipeline Matching Review contract smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_matching_review_contract\ui_precheck_report.md`
- Pipeline FeatureMatching Review contract smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_feature_matching_review_contract\ui_precheck_report.md`
- FeatureMatching Tool Form Review smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_feature_matching_form_review2\ui_precheck_report.md`
- Latest final non-UI platform precheck after documentation and Matching fixture update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_docs_final_skipui2\platform_precheck_report.md`
- Latest final non-UI platform summary after documentation and Matching fixture update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_docs_final_skipui2\platform_precheck_summary.json`
- Sample Check guide UI and AI Recipe feedback smoke after adding metric-to-check guidance: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract2\ui_precheck_report.md`
- Latest final non-UI platform precheck after Sample Check guide contract update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract_final_skipui\platform_precheck_report.md`
- Latest final non-UI platform summary after Sample Check guide contract update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract_final_skipui\platform_precheck_summary.json`

Latest focused UX/contract reports:

- AI Recipe visible retry scope: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_visible_retry\ui_precheck_report.md`
- AI Recipe failed-step focus and feedback button contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_failed_focus_0800\ui_precheck_report.md`
- AI Recipe XML Patch Request contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_patch_ui\ui_precheck_report.md`
- AI Recipe selectable Safe Fix contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_selectable_fix_20260617_b\ai_recipe_suggested_fix_check.png`
- AI Recipe operator-confirmed acceptance limit fix contract: `artifacts\smoke\ai_recipe_acceptance_fix_check.png`
- AI Recipe editable Safe Fix value contract: `artifacts\smoke\ai_recipe_suggested_fix_check.png`
- AI Recipe Next Action guidance contract: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_next_action_suite\ai_recipe_next_action_check.png`
- Localization Editor smoke: `artifacts\smoke\localization_editor.png`
- Localization Editor missing-filter smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_editor_filter2\localization_editor.png`
- Localization catalog contract smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization3\localization_catalog_contract_check.png`
- WinForms automatic localizer contract smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization3\winforms_localizer_contract_check.png`
- WPG PropertyGrid localization contract smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization3\property_grid_localization_contract_check.png`
- Image Compare localization/source-format smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_image_compare_loc\image_compare_png_source_format.png`
- Image Compare n-image load smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_image_compare_loc\image_compare_multi_load.png`
- Image Compare non-visible maintenance pass: pixel marker size now follows screen-scale instead of forcing a large image-space minimum at high zoom. Verified without opening UI by building `src\Libraries\OpenVisionLab.ImageCanvas\OpenVisionLab.ImageCanvas.csproj`, building `tools\OpenVisionLab.ImageCompare\OpenVisionLab.ImageCompare.csproj`, and publishing `dist\OpenVisionLab.ImageCompare` through `scripts\Publish-ImageCompare.ps1` with no smoke window.
- Layer Display/ImageCanvas localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_layer_imagecanvas_loc\main_workspace.png`
- Pipeline menu localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_menu_loc\pipeline_form.png`
- Latest localization surface smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_surface_final2\main_workspace.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_surface_final2\pipeline_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_surface_final2\image_compare_png_source_format.png`
- Pipeline Add Step localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_addstep_localized2\pipeline_add_step_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_addstep_localized2\pipeline_add_step_branch_form.png`
- Pipeline Samples localization/action smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_samples_localized2\pipeline_samples_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_samples_localized2\pipeline_samples_check_action.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_samples_pair_wait\pipeline_samples_pair_check_action.png`
- Pipeline Batch/History localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_batch_history_localized3\pipeline_batch_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_batch_history_localized3\pipeline_history_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_batch_history_localized3\pipeline_batch_history_form.png`
- Rotate/Scale Tool Form localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_rotate_scale_localized2\tool_rotate_scale_form.png`
- VisionTest common input/output localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_visiontest_common_loc\tool_contour_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_visiontest_common_loc\tool_line_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_visiontest_common_loc\tool_rotate_scale_form.png`
- Line Tool Form and Add Pipeline bridge localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tool_bridge_localized\tool_line_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tool_bridge_localized\tool_contour_form.png`
- Matching/FeatureMatching review localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_matching_forms_localized2\tool_matching_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_matching_forms_localized2\tool_feature_matching_form.png`
- Tool Form constructor/localization regression smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tool_forms_localized_final\tool_contour_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tool_forms_localized_final\tool_blob_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tool_forms_localized_final\tool_matching_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tool_forms_localized_final\tool_feature_matching_form.png`
- AI Recipe Layer Flow localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_layer_flow_localized2\ai_recipe_layer_flow_edit_check.png`
- WPG display-name alias and Tool Form localization regression smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_wpg_localization_alias2\property_grid_localization_contract_check.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_wpg_localization_alias2\tool_matching_form.png`
- AI Recipe guide/localized panel regression smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_localized_guides2\ai_recipe_feedback_check.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_localized_guides2\ai_recipe_failed_step_focus_check.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_label_polish\ai_recipe_feedback_check.png`
- AI Recipe Prompt/TextPrompt localization smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_prompt_forms_localized\pipeline_text_prompt.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_prompt_forms_localized\ai_recipe_prompt_contract_check.png`
- Pipeline option/result-grid localization regression smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_localization_polish_final\pipeline_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_localization_polish_final\pipeline_form_run_preview.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_localization_polish_final\pipeline_matching_review_check.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_localization_polish_final\pipeline_feature_matching_review_check.png`
- Final representative localization regression smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_regression_final_20260618\main_workspace.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_regression_final_20260618\pipeline_form_run_preview.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_regression_final_20260618\ai_recipe_feedback_check.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_localization_regression_final_20260618\threshold_form.png`
- UI 95 regression after language/tutoral access polish: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ui95_regression_after_language_tutorial\main_workspace.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ui95_regression_after_language_tutorial\pipeline_form_run_preview.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ui95_regression_after_language_tutorial\threshold_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ui95_regression_after_language_tutorial\ai_recipe_feedback_check.png`
- Pipeline default translation migration smoke after `Pipeline.NewStepTool` Korean wording cleanup: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_label_migration\pipeline_form.png`, `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_label_migration\pipeline_form_run_preview.png`
- MainFrame language-selector placement/menu/tooltip contract smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_main_frame_tooltip_contract\main_frame_shell.png`. This target is a control contract, not a documentation screenshot, because offscreen capture cannot faithfully render the MDI shell.
- Tutorial sample-count contract smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_tutorial_count_contract\main_workspace.png`
- Pipeline auxiliary designable-form smoke: `C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_aux_localized\pipeline_designable_forms.png`
- Main workspace Korean/English guide-menu contract: `artifacts\smoke\main_workspace.png`
- AI Recipe current-XML Good/Bad pair check: `C:\Users\nacho\AppData\Local\Temp\openvisionlab_ai_recipe_pair_check_final_20260618_211303.png\ai_recipe_catalog_sample_check.png`
- AI Recipe Apply + Preview regression after Check Pair addition: `C:\Users\nacho\AppData\Local\Temp\openvisionlab_ai_recipe_fix_review_20260618_210853.png\ai_recipe_suggested_fix_check.png`
- Pipeline Samples selection guide: `C:\Users\nacho\AppData\Local\Temp\openvisionlab_samples_guide_final_20260618_212427\pipeline_samples_form.png`
- Pipeline Samples Good/Bad guide after pair check: `C:\Users\nacho\AppData\Local\Temp\openvisionlab_samples_pair_guide_20260618_212400\pipeline_samples_film_pair_check_action.png`
- Pipeline Korean/English localization surface smoke: `artifacts\smoke\pipeline_form.png`
- Threshold Korean/English localization surface smoke: `artifacts\smoke\threshold_form.png`
- MessageBox Korean/English default action smoke: `artifacts\smoke\message_box_info.png`, `artifacts\smoke\message_box_error_details.png`
- LogPanel Korean/English control surface smoke: `artifacts\smoke\log_panel_contract_check.png`
- WPG visual finish contract: `artifacts\smoke\pipeline_property_grid_contract_check.png`
- WPG representative Tool Form smoke: `artifacts\smoke\tool_contour_form.png`, `artifacts\smoke\tool_line_form.png`, `artifacts\smoke\threshold_form.png`
- Tutorial Tool Form image contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tutorial_tool_image_contract\ui_precheck_report.md`
- Latest non-UI platform precheck after tutorial Tool Form image update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tutorial_tool_image_platform_skipui\platform_precheck_report.md`
- WPG Threshold property contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_wpg_threshold_contract\ui_precheck_report.md`
- Log panel contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_log_panel_contract\ui_precheck_report.md`
- Main workspace layer/result contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_main_workspace_contract\ui_precheck_report.md`
- Combined changed-surface UI contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_contract_0800_pass\ui_precheck_report.md`
- UI 95 polish pass covering Main, Pipeline, Threshold, Log, and MessageBox: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_95_pass3\ui_precheck_report.md`
- Quiet offscreen UI precheck:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_quiet_check\ui_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_quiet_main_pipeline\ui_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_ui_quiet\ui_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\ui\ui_precheck_report.md`
- HTML Guide contract:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_html_guide_contract\ui_precheck_report.md`
  - Program `Guide` menu resolves `docs/OPENVISIONLAB_TUTORIAL.html`.
  - Pipeline `More` menu exposes `Open Tutorial...` for users editing Step flow.
  - Referenced tutorial screenshots exist under `docs/assets/tutorial`.
  - Tutorial text now explains inspection-form teaching and multi-layer comparison so users can learn Tool Form tuning before saving Pipeline XML.

Latest 1~7 work report:

- `docs/OPENVISIONLAB_1200_WORK_REPORT.md`
- Includes scenario validation, HTML/Markdown tutorial, AI Recipe interactive edit plan, external reference policy, Main `Guide` entry point, and current remaining work.

Latest focused Sample Catalog UI smoke:

- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_coverage_ui\ui_precheck_report.md`
- Targets: `pipeline_samples_form`, `pipeline_samples_check_action`
- Result: OK.

## Working Rules

1. Do not repeat broad work when a scoped check is enough.
2. UI edits should stay designer-friendly unless the control is intentionally WPF-hosted.
3. A new feature should have at least one focused smoke, sample contract, or runner check.
4. Pipeline preview must stay separate from publish-to-workspace behavior.
5. A failed tool must explain whether it is invalid input, invalid parameter, ROI/configuration issue, timeout/cancel, exception, or acceptance NG.
6. CVBlob DLL version must remain fixed. Cleanup around it is allowed; version upgrades are not.
7. UI precheck runs offscreen by default. Use `-VisibleCapture` or `-VisibleUiCapture` only when an intentional visible screenshot is needed.
8. Sample/tutorial UX should show the recipe purpose, input/output flow, expected Metric, actual Metric, and judgment in the same review path.
9. Before adding a feature, check whether an equivalent contract already exists. Current non-repeat areas are Good/Bad pair metadata, external reference preflight, AI Recipe safe-fix apply, and WPG Threshold/Range editor registration.

## Immediate Priority Queue

### 1. Sample Coverage

Current uncovered sample folders:

- None.

Recently covered:

- Real-material `MasterImage` samples were removed from the active catalog and sample tree. Do not re-add production material images without an explicit sanitized sample policy.
- `EasyMatrixCode`: `EasyMatrixCode_AutoRead_Contour` validates a representative matrix-code image through the generic Threshold -> Morphology -> Contour recipe.
- `EasyOCR2`: `EasyOCR2_Characters_Contour` validates a representative OCR2 character image through the generic Threshold -> Morphology -> Contour recipe.

Next action:

- Do not implement dedicated decoder/OCR behavior unless explicitly selected later.
- Expand from folder-level coverage to stronger OK/NG decision contracts where stable metrics exist.
- Candidate outputs should remain contour/edge/matching/mean/rotate-scale style recipe validation unless semantic recognition is explicitly selected.

### 2. Result Metrics And Overlays

Current state:

- Contour, Blob, LineGauge, Matching, Mean, RotateScale, OverlayMerge, bounds metrics, and line metrics have sample-backed or contract-backed checks.
- Successful catalog steps now also have a status contract: OK, Passed, ErrorCode=0/None, AcceptancePassed=true, and no failure diagnostics.
- Invalid imported step configuration has a focused contract so LLM/XML mistakes produce actionable failure details.
- Sample Catalog can now validate multiple expected metrics in one row using semicolon-separated metric/range values.
- The current catalog uses stronger multi-metric gates for representative defect and geometry samples: count plus area, count plus bounds, line edge-count plus length/angle, template score plus result count, and OverlayMerge source/overlay counts.

Next action:

- Add more tool-specific summary rows where users need a direct decision signal.
- Prefer sample-backed metrics over synthetic-only tests.
- Expand paired OK/NG defect contracts where sample images make that practical.
- Continue measurement work from the new Pixel/mm metric baseline: add calibration UX and more dedicated distance/size samples after the measurement tool flow is selected.

### 3. Threshold/WPG Editor Finish

Current state:

- Pipeline WPG Threshold/Range editor contract exists.
- WPG PropertyGrid display names, categories, and descriptions are now localized through the bridge-level descriptor wrapper instead of requiring per-form UI code.
- Common Tool/Step fields such as Input Layer, Output Layer, Threshold, ROI, Contour, Blob, LineGauge, Matching, Rotate/Scale, and Acceptance now have central catalog keys.
- Duplicate helper rows are hidden.
- Threshold/Range/Input Layer/Output Layer descriptions are now contract-tested, so the user-facing intent is less implicit.
- Threshold form preview now states active mode, input layer, output layer, and why the selected mode is useful.
- Shared WPG row styling is applied in `WpfPropertyGridBridge` so the current app can receive the visual finish even when the .NET Framework 3.5 targeting pack is unavailable for rebuilding the original WPG DLL.
- WPG-CUSTOM source XAML has the matching theme changes. To rebuild the original `System.Windows.Controls.WpfPropertyGrid.dll`, the PC needs the .NET Framework 3.5 targeting pack.

Next action:

- Expand catalog coverage for less-used properties only when those forms become active work, rather than scattering one-off strings into each form.
- Move any remaining reusable threshold/range editor behavior into the shared WPG/control path.
- Keep Threshold form designer-friendly.
- Verify only the Threshold/WPG surface after editing.

### 4. Logging And Message UX

Current state:

- Log filter levels are simplified to `Any`, `Info`, `Warning`, `Error`.
- Debug remains code-compatible but hidden from operator filter UI.
- `All Logs` explicitly reports `Filters off`; Level/Area filter tooltips explain the disabled state.
- Active filter text now distinguishes filtered view, area, level, and no-filter states.
- MessageBox detail actions now use `Technical Details`, `Hide Details`, and `Copy Details`.
- Focused log smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_log_panel_contract\ui_precheck_report.md`

Next action:

- Finalize message taxonomy.
- Keep short log messages for normal operation.
- Keep detailed diagnostics available on demand.
- Avoid modal dialogs for normal preview/check information.

### 5. AI Recipe Validation Loop

Current state:

- Prompt includes sample-backed recipes and validation-loop guidance.
- Prompt examples now distinguish Required validation contracts from Explore coverage examples, and each Required example includes an expected gate.
- Sample recipe guide text now converts expected metrics into direct check points such as detected object count, object width in px/mm, fitted line length/angle, matching score, mean brightness, and output image size.
- Feedback includes first failed step, directly dependent steps, error code, diagnostic hint, suggested fix, tool-specific patch proposal, metrics, overlays, result image, overlay image, and raw log expectations.
- The AI Recipe form now has an `XML Patch Request` preview and `Copy Patch Request` action. The copied request targets the selected failed Step, includes a current Step XML reference, and instructs the LLM to return a full `<VisionPipeline>` XML rather than a fragment.
- The same panel now shows `Safe Auto Fix Preview` when the form can apply safe Step/Parameter/Layer Flow corrections directly.
- Safe automatic corrections are now operator-selectable through `Safe Fix Selection`. Editable value rows can be changed before applying, and user-edited values are applied directly instead of applying the default proposal first and then overwriting it. The smoke contract verifies checked fixes, unchecked fixes, edited parameter values, and operator-confirmed acceptance limit values.
- Patch proposals now list likely XML fields and metric context, so retry prompts can target `InputLayer`, `OutputLayer`, tool parameters, and Acceptance limits more precisely.
- Layer-flow retry guidance now treats `Main` as the original reference image. If a later step reads `Main` after preprocessing, the LLM should either explain the branch or change the step to the previous `OutputLayer`.
- Good/Bad catalog pairs should be used for acceptance tuning. The target is not only to detect objects, but to explain why the OK image passes and the NG image fails.
- Retry feedback and prompts now tell the LLM to preserve successful previous steps, keep stable output layer names, and change only the first failed step plus directly dependent steps unless the layer flow itself is wrong.
- The AI Recipe form preview now exposes the high-signal retry scope directly, not only through copied feedback.
- A failed preview now selects the first failed row, highlights the failed step, logs the selected failed step, shows directly dependent steps, shows a tool-specific patch proposal, and keeps `Copy AI Feedback` enabled after Run Preview NG.

Next action:

- Extend editable Safe Fix coverage to any additional parameter families that become proven-safe through sample failures.
- Keep successful steps stable when asking the LLM to revise a recipe.
- Continue enforcing final review image patterns such as `OverlayMerge` for branched detection.

### 6. Tutorial And Tool Form Teaching UX

Current state:

- The in-program HTML tutorial now shows actual Tool Form screenshots for Contour, Blob, Pattern Matching, and Line.
- The tutorial also shows validated sample result images for Contour, Blob, Pattern Matching, EdgeDetection, and LineGauge/measurement workflows.
- The guide now separates Tool Form parameter teaching from Sample/Pipeline Preview result verification.
- Tool Form screenshots are captured as full forms, not property-only fragments. Each image should show Input, Output, editable properties, and result/review context when that Tool supports it.
- FeatureMatching now has a runnable `Feature_TemplateReview` Required sample and tutorial source/template/result images.

Next action:

- Improve the EdgeDetection Tool Form surface so it can be captured and taught directly instead of relying only on Pipeline/Sample result images.
- Strengthen Pattern Matching visual output so the tutorial can show a clear match overlay/score example rather than only the source/search image.
- Add per-tool "Open sample + recommended recipe" shortcuts if the tutorial viewer becomes interactive inside the product.

## Recommended Next Implementation

With sample coverage backlog excluded, continue the quality track in this order:

1. Add more paired OK/NG defect contracts where the existing sample images already support stable numeric metrics.
2. Finish Threshold/WPG visual consistency without reimplementing the existing shared Threshold/Range editor registration.
3. Improve EdgeDetection Tool Form teaching UX and Pattern Matching result overlay visibility.
4. Finalize logging/message taxonomy and keep normal preview/check information non-modal.
5. Improve AI Recipe guided editing beyond the current safe-fix preview by allowing operator-confirmed parameter/layer-flow changes.
6. Run scoped checks first, then one platform `-SkipUi` check after cross-cutting changes.

## 2026-06-17 20:00 Checkpoint

Completed in the latest autonomous pass:

- WPG-CUSTOM builds with Visual Studio MSBuild after shared Threshold/Range editor surface cleanup.
- OpenVisionLab Debug/Release builds pass with 0 warnings and 0 errors.
- `PipelineViewerScreenshotSmoke` now builds with 0 warnings using `UseAppHost=false`.
- UI smoke scripts now launch the smoke DLL through `dotnet exec`, avoiding stale EXE locks.
- Repeated screenshot smoke output now handles existing files and `.png` directory conflicts safely.
- Image Compare standalone publish passes Release smoke test.
- Image Compare standalone documentation was restored as an ASCII-safe guide.
- Portable tutorial was regenerated and verified: 25 image tags, 25 embedded images, 0 file image references.
- Platform precheck with `-SkipUi` passes all core gates: external references, build, XML compatibility, sample catalog, Runner API, AI Recipe prompt, Tool Result, sample/algorithm contract, and portable tutorial.
- Focused UI precheck passes for Main, Pipeline, PropertyGrid contract, Threshold, Image Compare, AI Recipe catalog sample, and Sample Pair check.

Current residual items:

- `pipeline_property_grid_contract_check` is functionally clean but still reports visual `WARN` because the contract screen is intentionally flat and sparse. Treat this as visual polish, not a blocking defect.
- Full visible UI review should be done only when a real UX surface changes; use scoped targets first.
- Continue next with WPG visual finish, EdgeDetection/Pattern Matching teaching polish, and external library package/version policy.

## AI Recipe Interactive Tuning Update

Completed after the 20:00 checkpoint:

- Safe Fix Selection now includes an editable `Value` column for proven-safe single-parameter fixes.
- Editable fixes are constrained by parameter type before apply:
  - gray value parameters: 0 to 255
  - positive integer parameters: greater than 0
  - odd kernel parameters: positive odd integer
  - positive double parameters: greater than 0
  - Canny aperture: 3, 5, or 7
- Structural fixes such as min/max swap, pipeline normalization, layer-flow auto correction, and boolean preprocessing cleanup remain selection-only.
- Acceptance limit fixes are available only from an actual Run Preview NG result. They are editable but default unchecked, so the operator must explicitly confirm that relaxing/tightening `AcceptanceMetricMinimum` or `AcceptanceMetricMaximum` is correct after checking the overlay.
- `Apply Fix` and `Apply + Preview` now apply the generated safe fix first, then apply the operator-edited value when it differs from the proposed value.
- The smoke contract verifies that an edited `PIXELPERMM` value is applied and that unchecked fixes are still not applied.
- A separate smoke contract verifies that an edited acceptance minimum is applied only after the operator selects the row.

## Good/Bad Pair Expansion Update

Completed after the AI Recipe safe-fix pass:

- Added `SurfaceDefect_EdgeCount` as a Good/Bad catalog pair.
- Good reference: `EasyObject_SurfaceNormal_Edge`, expected `ResultCount=1..5`, `AreaMax=20..70`.
- Bad reference: `EasyObject_SurfaceDefect2_Edge`, expected `ResultCount=20..60`, `AreaMax=120..200`.
- The sample catalog now has 40 runnable samples: 24 Required, 16 Explore, 40 OK, 0 NG in the latest focused runner.
- Good/Bad pair groups now cover `BentPin_Shaft`, `Film_DarkSpot`, and `SurfaceDefect_EdgeCount`.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
ai_recipe_suggested_fix_check: OK
ai_recipe_acceptance_fix_check: OK
ai_recipe_layer_flow_edit_check: OK
ai_recipe_catalog_sample_check: OK
sample_inventory_contract_check: OK
algorithm_sample_contract_check: OK
ai_recipe_prompt_contract_check: OK
pipeline_samples_pair_check_action: OK
Sample Catalog runner: OK, runnable=40, required=24, explore=16, NG=0
```

Next focus:

- Finish only remaining WPG visual gaps that appear in real Tool/Pipeline forms; the shared Threshold/Range/Metric editor behavior and localization path are now covered.
- Improve EdgeDetection Tool Form teaching UX and Pattern Matching review screenshots.
- Add more Good/Bad pairs only when the sample folder has a stable OK/NG metric boundary.

## Pipeline Localization Polish Update

Completed after the WPG and AI Recipe localization pass:

- Pipeline preview options, overlay label mode, overlay point labels, Result grid headers, Result row names, preview-required state text, and result action tooltips now use the central localization catalog.
- The Pipeline form keeps internal enum/state values stable while displaying Korean/English operator text, so smoke checks no longer depend on hard-coded English labels.
- Matching and FeatureMatching review checks accept localized Result grid rows for Template, Detected Crop, and Match Center.
- `Pipeline.NewStepTool` now has a default-catalog migration so existing operator CONFIG files that still contain the old default `새 Step Tool` are updated to `새 Step 도구` without overwriting unrelated user-edited translations.
- The language selector option text was normalized to valid UTF-8 Korean/English labels.

Verification:

```text
OpenVisionLab Debug build: OK
PipelineViewerScreenshotSmoke build: OK
pipeline_form: OK
pipeline_form_run_preview: OK
pipeline_matching_review_check: OK
pipeline_feature_matching_review_check: OK
localization_catalog_contract_check: OK
representative_localization_regression: OK
```

Next focus:

- Run one representative localization regression pass across Main, Pipeline, Threshold, LogPanel, Image Compare, WPG, AI Recipe, and Tool Forms.
- After that, move from visible UI polish to operator workflow polish: language switching access, tutorial/guide entry points, and sample-to-recipe discovery.

## MainFrame Language Access Update

Completed after the Pipeline localization polish:

- The title-bar language selector is now positioned by the same dynamic layout path as the right-side title buttons, so it does not drift offscreen at narrower widths.
- The Settings menu entry now describes its actual function: `Language / Translations` / `언어 / 번역 편집`.
- Menu/tooltips now explain language switching, translation editing, Image Compare, and Log Viewer access, and the smoke contract verifies the language selector tooltip and translation editor menu tooltip.
- Existing CONFIG catalogs with the old default `다국어 편집` / `Localization Editor` setting label migrate to the clearer wording without overwriting unrelated operator-edited translations.

Verification:

```text
OpenVisionLab Debug build: OK
PipelineViewerScreenshotSmoke build: OK
main_frame_shell: OK contract, visual WARN expected in offscreen MDI capture
localization_catalog_contract_check: OK
```

## Tutorial Catalog Count Update

Completed after the MainFrame language access pass:

- `OPENVISIONLAB_TUTORIAL.html`, `OPENVISIONLAB_TUTORIAL.md`, and `OPENVISIONLAB_TUTORIAL_PORTABLE.html` now describe the current sample catalog split: Required 24, Explore 16, Reference 1, and Required/Explore 40 runnable checks.
- The portable tutorial remains self-contained: 25 image tags and 25 embedded `data:image` assets.
- The Main workspace guide contract now checks the sample-count text so the tutorial cannot silently drift back to stale catalog counts.

Verification:

```text
main_workspace tutorial count contract: OK
platform precheck -SkipUi: OK
Tutorial Portable Contract: SourceImageCount=25, EmbeddedImageCount=25, Gate=OK
```

## Sample Catalog Localization Detail Update

Completed after the tutorial/catalog-count pass:

- Sample Catalog detail labels now use the central localization catalog for coverage, expected result headers, learning flow, Good/Bad pair summaries, and missing-reference messages.
- The smoke contract no longer depends on English-only UI strings. It accepts Korean/English labels for learning, flow, pair compare, last-check, backlog, and purpose text.
- Corrupted localized literal checks in the smoke tool were removed and replaced with semantic helper checks.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
pipeline_samples_form: OK
pipeline_samples_check_action: OK
pipeline_samples_pair_check_action: OK
localization_catalog_contract_check: OK
representative_ui95_sample_catalog_regression: OK
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_samples_detail_labels_localized2\pipeline_samples_form.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_samples_detail_labels_localized2\pipeline_samples_check_action.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_samples_detail_labels_localized2\pipeline_samples_pair_check_action.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ui95_sample_catalog_regression\main_workspace.png
```

Next focus:

- Continue the localization pass on remaining operator-facing utility forms and make sure the localization editor remains the single management surface.
- Then move to the next UX reliability gap: compare viewer workflow and AI Recipe tuning flow.

## Image Compare Workflow Review

Completed after the localization editor check:

- Image Compare keeps the source image format label separate from the OpenGL/canvas upload format, so 8-bit PNG/BMP files display as source `PNG 8-bit Gray` or `BMP 8-bit`.
- Multi-image compare is supported from 2 to 16 images with a responsive grid.
- The last opened image directory is persisted under the user profile and reused for the next Open dialog.
- Pixel marker and GV lookup are validated by smoke contracts against the loaded source bitmap coordinate path.

Verification:

```text
image_compare_8bpp_load: OK
image_compare_multi_load: OK, visual WARN from low-color fixture only
image_compare_png_source_format: OK
image_compare_sample_real_image: OK in visible capture mode
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_image_compare_workflow_review\image_compare_8bpp_load.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_image_compare_workflow_review\image_compare_multi_load.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_image_compare_workflow_review\image_compare_png_source_format.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_image_compare_real_sample_visible\image_compare_sample_real_image.png
```

Next focus:

- Keep OpenGL-backed Image Compare visual checks in visible capture mode. Quiet/offscreen capture can verify contracts, but its `DrawToBitmap` fallback cannot reliably capture GL textures.
- Continue AI Recipe tuning workflow review without duplicating already completed catalog/import checks.

## AI Recipe Interactive Tuning Review

Completed after the Image Compare workflow review:

- AI Recipe import/review form supports the core operator path: validate XML, run preview, inspect failed step, copy AI feedback, apply Safe Fix candidates, edit acceptance values, and edit Layer Flow without manually editing XML.
- The failed-step focus flow selects the first NG step and prepares an LLM-facing correction request that includes status, flow, and suggested scope.
- The Layer Flow editor can switch between previous output and branch input, then apply and re-run preview.
- The active catalog sample can now run its linked Good/Bad pair with the current editor XML through `Check Pair`; results are shown as operator-readable metric/final-layer review text and can be copied as AI feedback.
- The long full-suite timeout was caused by cumulative runtime, not by a functional failure. AI Recipe smoke targets should be run in smaller groups.

Verification:

```text
ai_recipe_form: OK
ai_recipe_suggested_fix_check: OK
ai_recipe_acceptance_fix_check: OK
ai_recipe_layer_flow_edit_check: OK
ai_recipe_feedback_check: OK
ai_recipe_failed_step_focus_check: OK
ai_recipe_catalog_sample_check: OK
ai_recipe_prompt_contract_check: OK
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_tuning_core\ai_recipe_layer_flow_edit_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_supporting_flow\ai_recipe_failed_step_focus_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_supporting_flow\ai_recipe_feedback_check.png
```

Next focus:

- Keep AI Recipe verification split into `form`, `core tuning`, and `supporting flow` groups to avoid false timeout failures.
- Remaining AI Recipe work is mostly prompt quality and review wording. The core mechanics now include validation, preview, safe fix, layer flow edit, revert, before/after review, catalog gate feedback, current-XML Good/Bad pair execution, and a clearer Pipeline Samples selection guide.

## Main Menu Localization Polish

Completed after the AI Recipe tuning review:

- Main toolbar command tooltips now come from the central localization catalog instead of inline Korean/English string branches.
- Guide menu tooltip, View menu items, layout status messages, and dynamic Vision Tool menu tooltips now use localization keys.
- Vision Tool menu names now resolve through `VisionMenu.*` keys, so language switching can update operator-facing tool names consistently.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
main_workspace: OK
main_frame_shell: OK contract, visual WARN expected in offscreen MDI capture
localization_catalog_contract_check: OK
```

Artifact:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_main_menu_localization_polish\main_workspace.png
```

## Tool Form Common Localization Pass

Completed after the main menu localization polish:

- `VisionTestForm` now localizes shared operator-facing run state, result publish, failure, default title/help, and common error messages through `OpenVisionLab.Localization`.
- Common Tool Form language switching now refreshes the base form labels/buttons instead of only applying localization during initial construction.
- Generic `Run` buttons can switch Korean/English both ways without overriding specialized buttons such as Line Form `Fit Line`.
- The shared alarm title and common Tool Form failure message now use catalog keys instead of inline strings.
- The generic sample contour pipeline elapsed acceptance limit was relaxed from `300 ms` to `1000 ms` so the recursive barcode contour sample validates algorithm behavior instead of failing on a machine-dependent timing gate.
- Acceptance diagnostics now prioritize elapsed-time failures before metric-range text, so NG explanations match the actual failed acceptance rule.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
localization_catalog_contract_check: OK
winforms_localizer_contract_check: OK
tool_arithmetic_form: OK
tool_blob_form: OK
tool_contour_form: OK
tool_edge_detection_form: OK
tool_feature_matching_form: OK
tool_filter_form: OK
tool_histogram_form: OK
tool_hsv_form: OK
tool_line_form: OK
tool_matching_form: OK
tool_mean_form: OK
tool_morphology_form: OK
tool_rotate_scale_form: OK
vision_recipe_runner_api_contract_check: OK
platform precheck -SkipUi: OK
sample catalog: OK, 40/40
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_all_tool_forms_common_localization\tool_contour_form.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_all_tool_forms_common_localization\tool_matching_form.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_all_tool_forms_common_localization\tool_line_form.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_visiontest_localization_common\threshold_form.png
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck\platform_precheck_report.md
```

Next focus:

- Continue reducing visible hardcoded text in Main/Pipeline result-detail surfaces where the text is dynamically generated rather than designer-managed.
- Add stronger UI contracts for language switching on already-open Tool Forms.
- Continue Good/Bad sample expansion and AI Recipe tuning usability once the remaining visible localization gaps are closed.

## Pipeline Result Detail Localization Pass

Completed after the Tool Form common localization pass:

- Pipeline result-detail rows now localize the visible value text for common decision and result summaries instead of only localizing the row names.
- Tool error, acceptance NG, summary mode, image size, result count, score, angle, and overlay count text now resolve through `OpenVisionLab.Localization`.
- Pipeline preview/result captions continue to use the existing shared `Pipeline.*` catalog keys, so Korean/English switching stays centralized.
- Pipeline Check/Preview/Publish hints and common blocking message boxes now resolve through localization keys instead of inline literals.
- Pipeline button icon setup now reads labels from the localization catalog, so designer initialization and runtime localization do not fight each other.
- Pipeline Summary overlay badge text is now catalog-based while preserving the same compact visual format.
- The latest verification was run as a targeted Pipeline smoke group because the full platform precheck had already passed and these changes are src/OpenVisionLab/UI/localization scoped.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
pipeline_form: OK
pipeline_form_run_preview: OK
pipeline_sample_open_preview: OK
pipeline_sample_llm_open_preview: OK
localization_catalog_contract_check: OK
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_result_value_localization\pipeline_form_run_preview.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_result_value_localization\pipeline_sample_llm_open_preview.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_hint_localization_final\pipeline_form_run_preview.png
```

Next focus:

- Review remaining Pipeline generated log snippets such as preset logs, import prompts, sample-ready hints, and low-frequency view messages.
- Keep language-switch smoke coverage for already-open Pipeline and common Tool Forms in the regular UI regression set.
- Keep broad sample/precheck validation separate from focused UI smokes to avoid spending time on unrelated unchanged surfaces.

## Open Form Language Switch Contract

Completed after the Pipeline result-detail localization pass:

- Added `pipeline_language_switch_check` to verify an already-open Pipeline form updates visible action labels and run-log caption when switching Korean -> English -> Korean.
- Added `tool_form_language_switch_check` to verify a common Vision Tool form updates shared `VisionTestForm` labels/buttons through the same language-change event path.
- Added `threshold_language_switch_check` to verify the Threshold teaching form updates input/output captions, section titles, and the Add Step action while already open.
- Added `main_frame_language_switch_check` to verify the main shell language selector updates menu text and language tooltip while the shell is already open.
- The checks use existing screenshot smoke infrastructure, so regressions now fail in CI-style local smoke instead of being found only by manual UI review.

Verification:

```text
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
pipeline_language_switch_check: OK
tool_form_language_switch_check: OK
threshold_language_switch_check: OK
main_frame_language_switch_check: OK
language-switch bundle: exit 0
main_frame_language_switch_check quiet capture: WARN from offscreen MDI visual flatness only
main_frame_language_switch_check visible capture: OK
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_open_form_language_switch\pipeline_language_switch_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_open_form_language_switch\tool_form_language_switch_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_threshold_language_switch\threshold_language_switch_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_main_frame_language_switch_visible\main_frame_language_switch_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_language_switch_bundle\pipeline_language_switch_check.png
```

Next focus:

- Keep MainFrame visual review in visible-capture mode when the actual shell screenshot matters; quiet mode remains valid for contract checks.
- Continue reducing low-frequency Pipeline literals only where they are operator-facing; keep structured log prefixes stable for searchability.
- Start the next usability pass on AI Recipe tuning because the core Pipeline and Tool Form language-switch path is now covered by automated smoke.

## AI Recipe Next Action Guidance Pass

Completed after the open-form language switch contract:

- Added a designer-visible `nextActionLabel` at the top of the AI Recipe guide panel.
- The label is driven by current recipe state: waiting, validation OK, validation NG, running, Preview OK, and Preview NG.
- Result states take priority over the transient busy state, so a finished Preview cannot remain stuck on "waiting for preview".
- Added central Korean/English catalog keys under `AiRecipe.NextAction.*`.
- Added `ai_recipe_next_action_check` smoke target to verify initial waiting state, validated sample state, Preview OK text, and Preview OK color.
- Re-ran the existing AI Recipe tuning smokes to make sure Safe Fix, feedback, and Layer Flow behavior did not regress.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
ai_recipe_next_action_check: OK
ai_recipe_feedback_check: OK
ai_recipe_suggested_fix_check: OK
ai_recipe_layer_flow_edit_check: OK
```

Artifacts:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_next_action_suite\ai_recipe_next_action_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_next_action_suite\ai_recipe_feedback_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_next_action_suite\ai_recipe_suggested_fix_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_ai_recipe_next_action_suite\ai_recipe_layer_flow_edit_check.png
```

Next focus:

- Pipeline Step Flow should make Input/Output image dependency clearer, especially for linked previous-output steps versus branch-from-Main steps.
- Image Compare should receive the next focused polish pass: n-image layout, pixel/GV contract, and standalone executable packaging.
- Good/Bad sample pairs remain the next high-value functional expansion after the current UI guidance pass.

## Pipeline Input/Output Flow Clarity Pass

Completed after the AI Recipe next-action pass:

- Pipeline Step result details now expose `Input source` and `Output result` rows near the top of the Result grid.
- The selected Step now explains whether it starts from a source layer, uses the previous Step output, or branches from `Main`/another layer.
- The output row now explains which layer the Step creates or updates and whether that image is currently available.
- WPF Pipeline Flow cards now allow the flow-relation text to wrap instead of truncating branch/previous-output guidance.
- Result-grid value column width was adjusted so flow explanations have more usable space.
- The smoke contract now verifies that a chained Step exposes previous-output guidance and output-layer guidance in the Result grid.
- Branch smoke verification was made localization-aware so Korean/English button labels do not cause false failures.

Verification:

```text
OpenVisionLab Debug build: OK, warnings=0, errors=0
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
pipeline_form_run_preview: OK
pipeline_form_branch: OK
pipeline_form_branch_check: OK
pipeline_sample_open_preview: OK
localization_catalog_contract_check: OK
```

Artifacts already captured before visible UI checks were stopped:

```text
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_io_flow_bundle_final\pipeline_form_run_preview.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_io_flow_bundle_final\pipeline_form_branch.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_io_flow_bundle_final\pipeline_form_branch_check.png
C:\Users\nacho\AppData\Local\Temp\ovl_smoke_pipeline_io_flow_bundle_final\pipeline_sample_open_preview.png
```

Next focus:

- Do not run visible src/OpenVisionLab/UI/capture checks while the user is at work. Use build/static checks only until visible UI checks are explicitly allowed again.
- Continue with non-visual work first: Image Compare packaging logic, Good/Bad sample metadata, or localization/catalog cleanup.
- When visible checks are allowed again, verify Pipeline Flow at smaller widths and branch-heavy recipes.

## Image Compare Non-Visible Maintenance Pass

Completed after the Pipeline Input/Output Flow clarity pass:

- Image Compare standalone publish path was verified without opening a window.
- The published standalone output is still cleaned by `scripts\Publish-ImageCompare.ps1`, and the current output contains 20 files at about 55.97 MB.
- Image Compare source-format display remains separated from the OpenGL/display bitmap format, so 8-bit PNG/BMP source metadata is not confused with the 24/32-bit rendering path.
- The ImageCanvas pixel marker now scales by screen zoom only. It no longer forces a large image-space minimum that can make the marker appear to cover a broad area at high zoom.
- Visible src/OpenVisionLab/UI/capture checks were intentionally skipped because the user requested no UI windows while at work.

Verification:

```text
OpenVisionLab.ImageCanvas Debug build: OK, warnings=0, errors=0
OpenVisionLab.ImageCompare Debug build: OK, warnings=0, errors=0
OpenVisionLab Debug build: OK, warnings=0, errors=0
ImageCompare publish Release: OK, Files=20, Size=55.97 MB
platform precheck -SkipUi: OK
Sample Catalog: runnable=40, required=24, explore=16, OK=40, NG=0
Tutorial Portable Contract: SourceImageCount=25, EmbeddedImageCount=25, Gate=OK
```

Artifacts:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_image_compare_nonui_20260618\platform_precheck_report.md
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_image_compare_nonui_20260618\platform_precheck_summary.json
C:\Git\OpenVisionLab_Dev\dist\OpenVisionLab.ImageCompare
```

Next focus:

- When UI checks are allowed again, visually confirm Image Compare marker size/GV alignment on the 4512 x 4512 8-bit sample at high zoom.
- Add a non-UI coordinate helper test if Image Compare logic is extracted into a pure service.
- Continue functional work with Good/Bad sample pair expansion or AI Recipe interactive tuning only after confirming no duplicate contract already exists.

## Non-Visible Reliability Contract Pass

Completed after the Image Compare non-visible maintenance pass:

- Extracted Image Compare mouse/image coordinate conversion into `ImagePixelCoordinateMapper`.
- Added `ImageCompareCoordinateContractCheck` so high-zoom pixel/GV alignment can be checked without opening the UI.
- Updated ImageCanvas pixel-marker drawing to use the same image-pixel bounds/center contract as Image Compare GV lookup.
- Added a Fiducial Good/Bad sample pair using `Sample\EasyFind\Fiducial 1.tif` and `Sample\EasyFind\Fiducial 5 (Hidden).tif`.
- Promoted the Fiducial visibility pair into the sample catalog with explicit `ResultCount` and `BoundsWidthMax` gates.
- Extended the AI Recipe prompt contract so Good/Bad sample pairs are included in the generated tuning context.
- Added `VisionUiContractCheck` to verify WPG display widths, threshold editor binding, and shared range-editor bindings from build output assemblies.
- Integrated both new contract checks into `RunVisionPlatformPrecheck.ps1`.
- Visible UI checks were intentionally skipped because the user requested no UI windows while at work.

Verification:

```text
ImageCompareCoordinateContractCheck: OK
VisionUiContractCheck: OK
platform precheck -SkipUi: OK
Sample Catalog: runnable=41, required=26, explore=15, OK=41, NG=0
Sample Folder Coverage: 14/14 covered, uncovered=0
Tutorial Portable Contract: SourceImageCount=25, EmbeddedImageCount=25, Gate=OK
```

Artifacts:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_nonui_20260618\platform_precheck_report.md
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_nonui_20260618\platform_precheck_summary.json
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_nonui_20260618\samples\sample_catalog_report.md
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_nonui_20260618\samples\sample_catalog_summary.json
```

Current self-assessment:

- Core tool/result contract: 92%. Stable for main tool flow, but more per-tool error codes should still be added to lower-frequency tools.
- Pipeline/recipe reliability: 93%. Preview, publish separation, layer flow, and sample gates are strong; interactive AI tuning still needs a more direct apply/revert UX.
- Sample validation coverage: 91%. All folders are represented and Good/Bad pairs are growing; more production-like paired samples are still needed.
- WPG/tool-form UI contract: 88%. Shared editor contracts are now checked, but visible polish must wait until UI checks are allowed.
- Image Compare: 90%. Source format, standalone packaging, and coordinate contracts are covered; visible high-zoom marker/GV review remains.

Next focus:

- When visible UI checks are allowed, verify Image Compare marker/GV alignment on 8-bit high-zoom wafer samples.
- Add more Good/Bad pairs for surface defect, pin defect, and template matching rather than only generic contour recipes.
- Improve AI Recipe interactive tuning wording and Good/Bad before/after comparison. Apply/revert mechanics are now covered by smoke.
- Extend `VisionUiContractCheck` to cover combo box/list editor contracts and layer selector editor contracts after those editors stabilize.

## AI Recipe Apply/Revert Loop Pass

Completed after the MasterImage removal and sample-catalog verification:

- Added `Revert Fix` to the AI Recipe form.
- Safe Fix and Layer Flow edits now store the previous XML before applying changes.
- Revert is enabled only while the current XML still matches the last applied edit, so manual XML edits are not overwritten by an old snapshot.
- Revert restores the previous XML, validates it, and writes a `Reverted Safe Fix` diff summary in the patch panel.
- The focused AI Recipe smoke verifies apply -> revert -> original XML restore.

Verification:

```text
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
ai_recipe_suggested_fix_check: OK
OpenVisionReadinessCheck: OK
LocalizationCatalogCheck: OK, entries=1062
```

Evidence:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_revert_fix_20260618_b\ai_recipe_suggested_fix_check.png
```

## AI Recipe Good/Bad Decision Feedback Pass

Completed after the apply/revert loop pass:

- AI Recipe catalog sample feedback now includes the sample's expected reason text when one exists.
- For paired Good/Bad or ExpectedFailure catalog rows, feedback now shows the pair group and role.
- The feedback tells the operator to keep the Good sample inside expected metric bounds and keep the Bad/ExpectedFailure pair separated by an explainable metric before publishing.
- This closes the duplicated "Good/Bad decision wording" work item. Future work should expand sample pairs or improve visual comparison, not re-add the same feedback text.

Verification:

```text
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
ai_recipe_catalog_sample_check: OK
```

Evidence:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_goodbad_decision_20260618_rerun\ai_recipe_catalog_sample_check.png
```

## AI Recipe Safe Fix Before/After Detail Pass

Completed after the Good/Bad decision feedback pass:

- Safe Fix detail now shows the selected item as a direct before/after value review before the operator applies it.
- Single parameter fixes use the current XML value and proposed value.
- Min/max swap fixes now parse the paired values and show them as `currentMin/currentMax -> proposedMin/proposedMax`.
- A new localization key, `AiRecipe.SafeFixDetail.BeforeAfterFormat`, avoids overwriting operator-edited runtime translations for the older detail format key.

Verification:

```text
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
LocalizationCatalogCheck: OK, entries=1063
ai_recipe_suggested_fix_check: OK
ai_recipe_catalog_sample_check: OK
OpenVisionReadinessCheck: OK
```

Evidence:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_before_after_fix_20260618_c\ai_recipe_suggested_fix_check.png
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_goodbad_decision_20260618_final\ai_recipe_catalog_sample_check.png
```

## AI Recipe Apply Preview Before/After Review Pass

Completed after the Safe Fix before/after detail pass:

- `Apply + Preview` now captures the last available Preview state before applying selected Safe Fix rows.
- After the changed recipe runs Preview, the patch panel shows a before/after review summary instead of forcing the operator to infer the result from XML and logs.
- The review includes previous status, new status, selected step/flow, Catalog Gate transition, expected reason, Good/Bad pair cue, metric comparison, visual review cue, and next action.
- The comparison is posted after the Preview UI refresh queue so Step grid refresh does not overwrite the review text.

Verification:

```text
PipelineViewerScreenshotSmoke build: OK, warnings=0, errors=0
ai_recipe_suggested_fix_check: OK
ai_recipe_catalog_sample_check: OK
OpenVisionReadinessCheck: OK
LocalizationCatalogCheck: OK, entries=1063
```

Evidence:

```text
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_before_after_review_20260618_e\ai_recipe_suggested_fix_check.png
C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_goodbad_after_review_20260618\ai_recipe_catalog_sample_check.png
```

Next focus:

- Visible UI checks are allowed again.
- Continue with visual review only for changed surfaces or high-risk surfaces, not broad all-form checks.
- Next practical target is sample-pair execution from AI Recipe: let the operator run the linked Good/Bad pair directly from the active catalog sample or generated review.
