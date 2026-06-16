# OpenVisionLab Next Work

Updated: 2026-06-16

OpenVisionLab is a rule-based OpenCVSharp vision workbench. The goal is not to be a loose collection of image-processing dialogs. The goal is a platform where a user can load a sample image, build a step pipeline, validate the result through metrics/overlays/logs, save the recipe as XML, and run the same recipe from UI, batch, AI Recipe import, or an external runner.

## Current Baseline

- Overall readiness: about 97%.
- Algorithm robustness: about 95%.
- Automated UI QA: about 97%.
- Pipeline persistence and sample validation: about 99%.
- External runner path: about 95%.
- Main viewer polish, Pipeline Flow clarity, logging/message UX, Threshold form polish, and in-program HTML tutorial access now have focused smoke/contract coverage at the 95% UI pass level. The tutorial now includes tool-specific test guides for Contour, Blob, Pattern Matching, EdgeDetection, LineGauge, distance/measurement workflows, inspection-form teaching flow, and multi-layer image comparison. Pixel/mm-derived measurement metrics are now validated for representative bounds and line samples. AI Recipe feedback now includes failed-step XML field candidates, metric context, and a copyable XML Patch Request for the selected Step; shared WPG editor consolidation remains the main UX finish item.
- The tutorial now includes real full-form Tool screenshots and sample result images for Contour, Blob, Pattern Matching, FeatureMatching, EdgeDetection, and LineGauge/measurement workflows. The guide separates "where to tune parameters" from "where to verify detection result" so users can learn by inspecting actual input/output form UI and validated sample outputs.
- Pattern Matching tutorial assets now use a tight 7PQRS button template, detected crop, and overlay result. The sample contract validates the detected center and bounds so the guide cannot regress to a loose background-heavy template.
- The Matching Tool Form now has an in-form Match Review area. After Run it shows the template image, detected crop, score, center, size, count, and confirms that the overlay result is written to the Output layer. The UI smoke now runs the Contour sample and validates that both preview images are populated.
- The Pipeline Matching step preview now mirrors that review path. Selecting a Matching step after Run Preview shows the template image, detected crop, score, center, and size in the right preview panel, while the Result grid keeps the template and detected-crop details. The small Template/Crop previews and Result grid rows can be opened in the zoomable Pipeline image viewer.
- The HTML/Markdown tutorial now includes the Pipeline Matching Review screenshot and explains why Template, Detected Crop, Overlay, and Score should be reviewed together.
- The same Pipeline review path now covers FeatureMatching. A focused synthetic FeatureMatching Pipeline smoke validates Template, Detected Crop, Score, Center, Size, Result grid review rows, and zoomable preview affordance.
- The FeatureMatching Tool Form now has its own Feature Review panel. A focused form smoke runs a synthetic feature template case and validates template preview, detected crop, score, center, angle, and output overlay context.

Latest full platform precheck:

- Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\platform_precheck_report.md`
- Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\platform_precheck_summary.json`
- Result: Build OK, XML OK, sample runner OK, Runner API OK, Tool Result Contract OK, Sample Inventory OK, Algorithm Contract OK, UI Precheck OK.
- Sample rows: 37 runnable, 21 Required, 16 Explore, 37 OK, 0 NG.
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
- Result: all 37 runnable samples passed after strengthening LLM OverlayMerge, Blob, BentPin, DiePad, LineGauge, Matching, FeatureMatching, and recursive EasyGauge/EasyMatch geometry gates.
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

## Immediate Priority Queue

### 1. Sample Coverage

Current uncovered sample folders:

- None.

Recently covered:

- `MasterImage`: `MasterImage_Left_Mean` now validates a 2056 x 2464 color BMP through the Mean tool and artifact generation path.
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
- Duplicate helper rows are hidden.
- Threshold/Range/Input Layer/Output Layer descriptions are now contract-tested, so the user-facing intent is less implicit.
- Threshold form preview now states active mode, input layer, output layer, and why the selected mode is useful.
- Shared editor reuse is not fully complete.

Next action:

- Move the final reusable threshold/range editor behavior into the shared WPG/control path.
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
- Patch proposals now list likely XML fields and metric context, so retry prompts can target `InputLayer`, `OutputLayer`, tool parameters, and Acceptance limits more precisely.
- Layer-flow retry guidance now treats `Main` as the original reference image. If a later step reads `Main` after preprocessing, the LLM should either explain the branch or change the step to the previous `OutputLayer`.
- Good/Bad catalog pairs should be used for acceptance tuning. The target is not only to detect objects, but to explain why the OK image passes and the NG image fails.
- Retry feedback and prompts now tell the LLM to preserve successful previous steps, keep stable output layer names, and change only the first failed step plus directly dependent steps unless the layer flow itself is wrong.
- The AI Recipe form preview now exposes the high-signal retry scope directly, not only through copied feedback.
- A failed preview now selects the first failed row, highlights the failed step, logs the selected failed step, shows directly dependent steps, shows a tool-specific patch proposal, and keeps `Copy AI Feedback` enabled after Run Preview NG.

Next action:

- Turn the text patch proposal into an interactive parameter/layer-flow edit surface.
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
2. Finish Threshold/WPG editor consistency inside the shared control path when the editable WPG source is available.
3. Improve EdgeDetection Tool Form teaching UX and Pattern Matching result overlay visibility.
4. Finalize logging/message taxonomy and keep normal preview/check information non-modal.
5. Improve AI Recipe guided editing beyond failed-row/dependent-step focus by turning patch proposals into editable parameter/layer-flow changes.
6. Run scoped checks first, then one platform `-SkipUi` check after cross-cutting changes.
