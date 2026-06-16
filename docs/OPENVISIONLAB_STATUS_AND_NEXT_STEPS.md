# OpenVisionLab Status And Next Steps

Updated: 2026-06-16

## Product Direction

OpenVisionLab is moving toward a rule-based vision recipe workbench.

The final shape should let a user:

- Load an image and inspect it through layers, ROI, coordinates, pixels, and zoom/pan.
- Tune OpenCvSharp tools with immediate preview.
- Build a pipeline where every step has a clear input image and output image.
- Validate the pipeline through overlays, metrics, acceptance criteria, and logs.
- Save the approved recipe as XML.
- Run the same XML from the main UI, pipeline UI, batch/samples, AI Recipe import, and an external runner/DLL.

The key UX principle is that every detail should reduce user uncertainty. A user should always know:

- Which image is being read.
- Which layer will be written.
- Whether a step is chained or intentionally branched.
- Whether the result is only a preview or published to the main workspace.
- Why a step is OK, NG, or needs review.

## Work Completed In This Pass

Latest 1~7 platform/accessibility update:

- Added a practical scenario validation checklist:
  - `docs/OPENVISIONLAB_SCENARIO_VALIDATION.md`
- Added an operator/tutorial document:
  - `docs/OPENVISIONLAB_TUTORIAL.md`
- Added a user-facing HTML tutorial with local screenshots:
  - `docs/OPENVISIONLAB_TUTORIAL.html`
  - `docs/assets/tutorial/*.png`
- The tutorial now explains two user-facing workflows that were previously implicit:
  - how to teach each inspection from the actual Tool Form before adding it to Pipeline,
  - how to compare `Main`, preprocessing, and final detection images through multiple layers.
- Added AI Recipe interactive correction plan:
  - `docs/OPENVISIONLAB_AI_RECIPE_INTERACTIVE_EDIT_PLAN.md`
- Added external reference policy for `Library-Noah` and `WPG-CUSTOM`:
  - `docs/OPENVISIONLAB_EXTERNAL_REFERENCE_POLICY.md`
- Added 12:00 work report:
  - `docs/OPENVISIONLAB_1200_WORK_REPORT.md`
- Main now exposes a `Guide` menu that opens the image-based HTML tutorial from inside the program.
- Pipeline `More` menu now exposes `Open Tutorial...` so users can open the same HTML tutorial while editing Step flow.
- AI Recipe now exposes an `XML Patch Request` panel and `Copy Patch Request` action for the selected failed Step. The copied text includes a current Step XML reference and requires the LLM to return a full `<VisionPipeline>` XML.
- The image-based tutorial now includes actual Tool Form screenshots and validated sample result images for Contour, Blob, Pattern Matching, EdgeDetection, and LineGauge/measurement workflows. This makes the guide closer to the real teaching process: tune in the Tool Form, verify in Sample/Pipeline Preview, then save or publish.
- Pattern Matching tutorial/sample assets now use a tight 7PQRS button template, matching detected crop, and overlay result. The smoke contract checks center and bounds against that specific button to prevent ambiguous template crops from returning.
- Matching Form UX now includes a Match Review panel for Template, Detected Crop, Score, Center, Size, Count, and Output overlay context. Focused smoke executes the matching sample and checks that template/crop previews are actually filled after Run.
- Pipeline Matching step UX now includes the same review concept in the selected-step preview. After Run Preview, the Pipeline view shows Template, Detected Crop, Score, Center, and Size next to the overlay image, and keeps Template/Detected Crop rows in Result Details for review. The small Template/Crop previews and Result Details rows open the zoomable image viewer for closer inspection.
- The HTML and Markdown tutorials now include the Pipeline Matching Review screenshot and explain the tight-template, detected-crop, overlay, and score review flow.
- FeatureMatching now uses the same template-based Pipeline Review path. A focused synthetic Pipeline smoke verifies FeatureMatching Template, Detected Crop, Score, Center, Size, Result Details rows, and zoomable review affordance.
- The HTML and Markdown tutorials now include a separate FeatureMatching section so users can distinguish feature-based matching from simple template matching.
- FeatureMatching Form now includes a Feature Review panel. After Run it shows the feature template, detected crop, score, center, size, angle, count, and output overlay context.
- The tutorial now includes the FeatureMatching Tool Form screenshot as well as the Pipeline FeatureMatching Review screenshot.
- Focused Pipeline Matching Review smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_matching_review_contract\ui_precheck_report.md`
- Tutorial Tool Form image contract passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tutorial_tool_image_contract\ui_precheck_report.md`
- Non-UI platform precheck after the tutorial Tool Form update passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tutorial_tool_image_platform_skipui\platform_precheck_report.md`
- Main toolbar menu width was adjusted for the added `Guide` menu and runtime `보기` menu.
- UI smoke text collection now includes ToolStrip item text, and `main_workspace` verifies the `Guide` menu.
- `main_workspace` now also verifies that the runtime documentation resolver can find `OPENVISIONLAB_TUTORIAL.html` and that the referenced tutorial image assets exist.
- Focused HTML guide contract passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_html_guide_contract\ui_precheck_report.md`
- Focused guide contract passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_guide_contract\ui_precheck_report.md`
- Scoped quiet UI precheck passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_ui_quiet\ui_precheck_report.md`
- Non-UI platform precheck passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_skipui\platform_precheck_report.md`
- Full quiet platform precheck passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\platform_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_full_sample_complete\ui\ui_precheck_report.md`
- Recursive sample folder coverage is now complete at the generic image-processing level:
  - Added `EasyMatrixCode_AutoRead_Contour`.
  - Added `EasyOCR2_Characters_Contour`.
  - Sample Catalog: 37 runnable, 21 Required, 16 Explore, 37 OK, 0 NG, 0 uncovered folders.
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_platform_sample_complete_skipui\platform_precheck_report.md`
  - Sample Catalog UI smoke also passed:
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_ui_complete\ui_precheck_report.md`
  - Sample Catalog backlog-none UI contract passed:
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_backlog_none_contract\ui_precheck_report.md`

Latest UI polish update:

- Pipeline Flow input/output pills now state the action directly:
  - `View input image`
  - `View output image`
  - `Run Preview required`
- Main workspace smoke now validates not only stored image size, but also the right-side source-layer role and top toolbar layer/source/flow state.
- Threshold form preview text now explains the active mode, input layer, output layer, and mode purpose.
- Log panel active-filter text now distinguishes `Filtered view`, `Area`, `Level`, and `No filter` states.
- MessageBox detail actions now use clearer text:
  - `Technical Details`
  - `Hide Details`
  - `Copy Details`
- Focused UI 95 pass completed:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_95_pass3\ui_precheck_report.md`
  - Targets: `main_workspace`, `pipeline_form`, `pipeline_form_branch`, `pipeline_property_grid_contract_check`, `log_panel_contract_check`, `threshold_form`, `message_box_error_details`, `message_box_error`, `message_box_confirm`
  - Result: all targets OK; `pipeline_property_grid_contract_check` remains `WARN` only because the visual check reports a flat static contract image, not because layout/text/internal checks failed.
- UI precheck now runs in quiet offscreen mode by default so forms are not brought to the user's desktop during normal development.
  - Use `-VisibleCapture` only when an intentional screen-visible capture is needed.
  - Quiet check reports:
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_quiet_check\ui_precheck_report.md`
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_quiet_main_pipeline\ui_precheck_report.md`

Latest pipeline clarity update:

- Add Step normal flow now treats the previous enabled step output as the default next input.
- Branch input confirmation now reads as `Allow branch input`.
- Pipeline Flow input labels are clearer:
  - `SOURCE`: first/source image input.
  - `PREV OUT`: normal chained input from the previous step output.
  - `BRANCH IN`: intentionally reading from a different layer.
- Duplicating a step now creates a chained copy after the selected step instead of preserving an ambiguous old input.
- Selected-step preview is clearer:
  - Preview caption now shows `Preview - MODE | Layer`.
  - Result Details has a `Viewing` row.
  - Clicking `Input image`, `Output image`, or `Overlays` switches the preview mode.
  - Pipeline Flow highlights the selected input/output pill more visibly.
- UI screenshot smoke was run for `pipeline_form`, `pipeline_form_branch`, `pipeline_add_step_form`, and `pipeline_add_step_branch_form`; all returned `OK`.

Latest sample catalog and platform validation update:

- Added `docs/samples/OpenVisionLab.SampleCatalog.csv` as the first shared benchmark catalog.
- Added `tools/RunVisionSampleCatalog.ps1` so sample images can be validated from the command line without opening the UI.
- Pipeline Samples now has a `Recipe Catalog` tab.
- Opening a catalog sample loads the sample image to `Main`, imports the recommended pipeline XML, shows the expected metric in the run log, and starts Run Preview.
- The existing saved workspace sample workflow remains available under `Saved Workspace`.
- The sample catalog now stores expected metric checks through:
  - `ExpectedMetricName`
  - `ExpectedMetricMinimum`
  - `ExpectedMetricMaximum`
- `ExpectedMetricName`, `ExpectedMetricMinimum`, and `ExpectedMetricMaximum` also support semicolon-separated multi-metric gates. This lets one sample validate paired signals such as width/height, count/bounds, or edge-count/line-length without adding duplicate catalog rows.
- `tools/RunVisionSampleCatalog.ps1` now fails required samples when the expected metric is missing or outside the expected range.
- Added sample-family recipe baselines:
  - `docs/samples/Rice_Particle_Contour.pipeline.xml`
  - `docs/samples/Pin_Feature_Contour.pipeline.xml`
  - `docs/samples/BentPin_LargeContour.pipeline.xml`
  - `docs/samples/DiePad_Surface_Contour.pipeline.xml`
- `tools/RunVisionPlatformPrecheck.ps1` now runs build, XML compatibility, sample catalog validation, and selected UI smoke as one platform-level check.
- Default UI precheck coverage now includes:
  - `main_workspace`
  - `pipeline_form`
  - `pipeline_form_branch`
  - `pipeline_designable_forms`
  - `pipeline_add_step_form`
  - `pipeline_add_step_branch_form`
  - `pipeline_property_grid_contract_check`
  - `log_panel_contract_check`
  - `pipeline_sample_open_preview`
  - `pipeline_sample_llm_open_preview`
  - `threshold_form`
  - `ai_recipe_form`
- Message box smoke targets are still available explicitly, but are no longer included in the default UI precheck.
- UI precheck should be scoped to the changed surface whenever possible. For example, main-view-only work should run `main_workspace` instead of every UI target.
- The LLM Recipe prompt now references the sample catalog and explicitly warns against accidentally branching back to `Main` or an older layer.
- Required sample catalog runs currently pass for:
  - `Contour_TextSymbols`
  - `Contour_AllSymbolsAndFaint_LLM`
  - `Contour_Generic`
  - `Contour_MeanBrightness`
  - `Contour_RotateScale_Resize`
  - `Rice_Particle`
  - `Rice_Particle_Blob`
  - `Pins_Feature`
  - `BentPin_Large`
  - `BentPin_TopBottom_Overlay`
  - `BentPin_GoodShaft`
  - `BentPin_BadShaft`
  - `DiePad1_Surface`
  - `DiePad2_Surface`
  - `DiePad3_Surface`
  - `DiePad4_Surface`
  - `Pins_LineGauge`
  - `Contour_TemplateMatching`
  - `EasyObject_SurfaceDefect1_Edge`
  - `EasyObject_SurfaceDefect2_Edge`
- Sample Catalog UX now exposes sample name, category, expected metric, and ready/missing state directly in the list item text.
- `Check Sample` now surfaces the last check result, actual metric, final layer, overlay count, elapsed time, and failed step details at the top of the detail panel.
- Catalog lists keep long sample and metric text reachable through horizontal scrolling.
- Screenshot smoke now validates sample-list metric/readiness text, result detail visibility, expected-result empty state, and sample open preview flow.
- Pipeline now keeps Sample Catalog context after `Open + Preview`: the header, run log, and Summary result grid show expected metric, actual metric, final layer, overlay count, and sample OK/NG state.
- Pipeline sample context now also shows the recipe guide:
  - `SAMPLE GUIDE` is written to the run log.
  - Summary details include `Goal` and `Recipe flow`.
  - The footer workflow hint keeps the preview/publish distinction visible while showing the active sample flow.
- AI Recipe prompt generation now reads the current Sample Catalog instead of relying only on hard-coded examples, so LLM requests include the latest Contour, OverlayMerge, LineGauge, and Matching reference recipes.
- AI Recipe retry feedback now includes concrete XML field candidates and metric context for the first failed Step, reducing vague "tune this" retry instructions.
- AI Recipe retry flow now also produces a copyable XML Patch Request for the selected Step, so retry instructions are no longer only a general feedback block.
- `Lib.OpenCV` Threshold `Threshold` and `Range` modes now normalize 3/4-channel input to grayscale before binary/range execution. This keeps UI Bitmap execution and external runner execution consistent, especially for branched low-contrast recipes.
- Added `pipeline_sample_llm_open_preview` screenshot smoke. It opens `Contour_AllSymbolsAndFaint_LLM`, runs the final `OverlayMerge`, and verifies `MergeOverlayCount=55` and `AllSymbols_Overlay` in the Pipeline UI path.
- Added `pipeline_property_grid_contract_check` screenshot smoke. It verifies Pipeline Threshold property metadata, WPG Threshold/Range editor registration, Range helper-property hiding, and actual WPF PropertyGrid rendering.
- Pipeline Threshold Range mode now shows `RangeMin` through the combined WPG Range editor and keeps helper properties `RangeMax` and `Invert` out of separate duplicate rows.
- `VisionRecipeRunner` now exposes external-call convenience properties:
  - `OutcomeText`
  - `SummaryText`
  - `ActionSummaryText`
  - `StepSummaryText`
  - `FirstFailedSummaryText`
  - `NormalizationText`
  - `HasFailedStep`
  - `FinalStepSummary`
  - `FinalMetricCount`
  - `FinalOverlayCount`
  - `FinalMetricsText`
  - `HasFinalResultImage`
- `VisionRecipeRunnerSmoke` now prints those fields so DLL/API users can quickly see final layer, result image, metrics, overlays, step flow, action guidance, and first-failure state without parsing every step.
- `RunVisionPlatformPrecheck.ps1` now includes `vision_recipe_runner_api_contract_check` as a separate Runner API gate between sample execution and UI precheck.
- The Runner API contract now validates both:
  - OK recipe summary for `Contour_TextSymbols`.
  - NG failure summary for an invalid Threshold Range recipe, including first failed step, `ThresholdInvalidRange`, `InvalidParameter`, and actionable `RangeMin`/`RangeMax` fix text.
- AI Recipe sample prompt generation now lists Required sample recipes first and limits Explore samples to representative groups, so the LLM receives useful patterns without an unbounded catalog dump.
- Added recursive sample coverage:
  - `docs/samples/OpenVisionLab.SampleCatalog.csv` now includes stable Explore representatives from `Sample/EasyImage`, `Sample/EasyGauge`, `Sample/EasyMatch`, `Sample/EasyObject`, `Sample/EasyColor`, `Sample/EasyFind`, `Sample/EasyBarCode`, `Sample/EasyQRCode`, and `Sample/EasyOcr`.
  - `sample_inventory_contract_check` scans `Sample` recursively and verifies representative folders plus recursive catalog rows.
  - BentPin and DiePad algorithm contracts now verify area statistics and overlay/result-count consistency, not only `ResultCount`.
  - Generic recursive contour representatives verify that the baseline Threshold -> Morphology -> Contour recipe runs on color-dot, fiducial, barcode, QR, and OCR-style sample images.
- Pipeline sample list text is now compact (`SampleName | Ready`); category, goal, expected metric, and pipeline details stay in the details panel.
- `VisionPipelineStepDiagnosticService` now returns more specific Hint/Fix text for common parameter errors in Threshold, Morphology, Filter, EdgeDetection, Contour, Blob, Matching, LineGauge, Mean, Feature, and Rotate/Scale.
- `tool_result_status_contract_check` now validates the whole non-None `VisionToolErrorCode` set:
  - ErrorCode resolves to the expected `VisionToolResultStatus`.
  - `VisionToolResult.Failed(...)` preserves the expected error/status.
  - Every ErrorCode returns non-empty diagnostic Hint/Fix text.
- `RunVisionPlatformPrecheck.ps1` now includes Tool Result Contract as a separate platform gate after Runner API Contract.
- `pipeline_designable_forms` is now included in the default UI precheck target list so Pipeline-related form constructor/designer regressions are caught earlier.
- Log panel filter UX now exposes only the normal operator-facing levels in the Level filter:
  - `Any`
  - `Info`
  - `Warning`
  - `Error`
- `LogLevel.Debug` remains available for code compatibility, but it is hidden from the normal log filter UI.
- `All Logs` now reports `Filters off` in the active-filter text, and Level/Area filter tooltips explain why the controls are disabled.
- Added `log_panel_contract_check` screenshot smoke. It verifies level list simplification, All Logs filter disabling, Pipeline/Warning filtering, Auto Scroll wording, active filter text, and actual WPF log panel rendering.
  - Focused log report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_log_panel_filters_off\ui_precheck_report.md`
- `InspectionAlgorithm` line-gauge helper code now shares the duplicated left/right execution path and reports no-result or result-count mismatch as explicit `InvalidOperationException` messages instead of falling through to index errors.
- `algorithm_sample_contract_check` and `pipeline_samples_pins_line_check_action` passed after the line-gauge helper cleanup.
- Full platform precheck passed after the latest log, Runner API, and line-gauge helper changes:
  - Build OK
  - XML compatibility OK
  - 12 required sample rows OK
  - Runner API OK/NG contract OK
  - Tool Result Contract OK
  - Default UI precheck OK
- A later `-SkipUi` platform precheck also passed after adding the Tool Result Contract gate.
- Final UI-included platform precheck also passed:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_final_autonomous\platform_precheck_report.md`
  - UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_final_autonomous\ui\ui_precheck_report.md`
- Latest recursive-sample platform precheck also passed:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_extended_samples\platform_precheck_report.md`
  - Includes Build, XML, recursive Sample Runner, Runner API Action/Step summary contract, Tool Result Contract, Sample Inventory Contract, and Algorithm Sample Contract.
- Extended recursive sample contract passed after adding EasyColor, EasyFind, EasyBarCode, EasyQRCode, and EasyOcr representatives:
  - UI contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_contract_extended\ui_precheck_report.md`
  - Sample runner report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_extended\sample_catalog_report.md`
- LLM Recipe prompt and sample preview smoke passed after sample prompt selection was narrowed:
  - UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_llm_prompt_sample_scope2\ui_precheck_report.md`
- BentPin branch/merge recipe was promoted to a Required sample:
  - Added `docs/samples/BentPin_TopBottom_Overlay.pipeline.xml`.
  - The recipe detects upper and lower bent-pin regions through separate ROI contour branches.
  - The final `OverlayMerge` step publishes one `BentPin_Review` layer with both branch results.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_bentpin_branch_contract2\ui_precheck_report.md`
- BentPin good/bad shaft classification contract was added:
  - Added `docs/samples/BentPin_ShaftContour.pipeline.xml`.
  - Added Required samples `BentPin_GoodShaft` and `BentPin_BadShaft`.
  - Both samples must detect 13 upper pin shafts.
  - The runner now exposes rectangle overlay width/height summary metrics such as `BoundsWidthMax`.
  - The good sample must keep `BoundsWidthMax` within the normal range.
  - The bad sample must expose the bent shaft as an abnormally wide contour through `BoundsWidthMax`.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_bentpin_shaft_contract\ui_precheck_report.md`
- SurfaceDefect edge-contour benchmark was promoted to Required for low-contrast defect candidates:
  - Added `docs/samples/SurfaceDefect_EdgeContour.pipeline.xml`.
  - `EasyObject_SurfaceDefect1_Edge` and `EasyObject_SurfaceDefect2_Edge` now run as Required catalog rows.
  - The algorithm contract now rejects broad whole-surface overlays and requires small defect-candidate rectangles.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_surface_defect_contract\ui_precheck_report.md`
- Latest non-UI platform precheck passed after BentPin branch/merge, BentPin shaft, SurfaceDefect, and overlay-derived bounds metric updates:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_bounds_metric\platform_precheck_report.md`
- Pipeline Samples UI smoke passed after adding the SurfaceDefect catalog rows:
  - UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_surface_defect_samples_ui\ui_precheck_report.md`
- Pipeline Samples and AI Recipe UI smoke passed after adding BentPin shaft Required samples:
  - UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_bentpin_shaft_llm_ui\ui_precheck_report.md`
- Runner/API, algorithm contract, and sample catalog checks passed after adding overlay-derived bounds metrics:
  - UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_bounds_metric_contract\ui_precheck_report.md`
- Pipeline Samples UI smoke passed after switching BentPin sample expected metric to `BoundsWidthMax`:
  - UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_bounds_metric_samples_ui\ui_precheck_report.md`
- Bounds metrics are now enriched at the pipeline execution result level, not only in `VisionRecipeRunner` summaries:
  - `BoundsWidthMin`, `BoundsWidthMax`, `BoundsWidthAvg`, `BoundsHeightMin`, `BoundsHeightMax`, and `BoundsHeightAvg` are added to `VisionToolResult.Metrics` from rectangle overlays before Acceptance evaluation.
  - Pipeline Acceptance can now use `BoundsWidthMax` directly, so BentPin-style OK/NG checks can be expressed as normal Step criteria.
  - Added acceptance presets for rectangle-overlay width/height checks in px and mm, including `Max Bounds Width <= 20 px`, `Max Bounds Height <= 20 px`, and their `0.12 mm` variants.
  - Runner API smoke now validates Good/Bad BentPin shaft acceptance and a mismatch NG case using `BoundsWidthMax`.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_bounds_acceptance_contract\ui_precheck_report.md`
  - Platform report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_bounds_acceptance\platform_precheck_report.md`
- LineGauge now has execution-level fitted-line metrics derived from line overlays:
  - `LineLengthMin`, `LineLengthMax`, `LineLengthAvg`, `LineAngleMin`, `LineAngleMax`, and `LineAngleAvg` are added from line overlay start/end points.
  - Added acceptance preset `Fitted Line Length >= 100 px` for Line/LineGauge tools.
  - `Pins_LineGauge` now verifies EdgeCount, EdgePointCount, fitted line length, fitted line angle, point overlay count, line ROI position, and step flow.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_line_overlay_metric_contract\ui_precheck_report.md`
- Basic Tool sample recipes were added to make the catalog less contour-only:
  - Added `docs/samples/Contour_MeanBrightness.pipeline.xml`.
  - Added `docs/samples/Contour_RotateScale_Resize.pipeline.xml`.
  - Added Required catalog rows `Contour_MeanBrightness` and `Contour_RotateScale_Resize`.
  - `Contour_MeanBrightness` validates `MeanValueAvg=254.7` against the sample-backed range `250..256`.
  - `Contour_RotateScale_Resize` validates 50% resize through `ResultImageWidth=384` and `ResultImageHeight=288`.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_basic_tool_contract\ui_precheck_report.md`
  - Sample catalog report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_basic_tool_catalog2\sample_catalog_report.md`
  - Pipeline Samples UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_basic_tool_samples_ui\ui_precheck_report.md`
  - AI Recipe UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_basic_tool_llm_ui\ui_precheck_report.md`
  - Platform report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_basic_tools_metrics\platform_precheck_report.md`
- Blob sample coverage is now part of the Required benchmark set:
  - Added Required catalog row `Rice_Particle_Blob`.

Latest scoped UX/contract pass:

- AI Recipe failed-preview feedback now shows the high-signal retry scope directly in the form preview:
  - `Preview Result`
  - `First Failed Step`
  - `Status`
  - `Flow`
  - `Direct Dependents`
  - `Message`
  - `Diagnostic`
  - `Suggested Fix`
  - `Patch Proposal`
  - `Change Scope`
  - concrete `Fix step XX` guidance
- `Copy AI Feedback` remains the full-detail path, but the visible preview now carries enough information for the user to understand which step should change first.
- Focused AI Recipe smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_visible_retry\ui_precheck_report.md`
- AI Recipe failed-preview focus now selects the first failed row, highlights the failed result, logs the selected failed step, shows directly dependent steps, shows a tool-specific patch proposal, and keeps `Copy AI Feedback` enabled after Run Preview NG:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_failed_focus_0800\ui_precheck_report.md`
- Pipeline Threshold WPG metadata was strengthened:
  - Input Layer explains that linked steps normally use the previous step output.
  - Output Layer explains that unique layer names make later review possible.
  - Mode explains Threshold, Range, and Adaptive behavior.
  - Threshold explains single gray-level classification.
  - Range explains combined Min/Max plus Invert behavior.
  - Adaptive algorithm explains MeanC versus GaussianC at a basic operator level.
- `pipeline_property_grid_contract_check` now verifies those descriptions in addition to editor registration and duplicate helper-row hiding.
- Focused WPG smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_wpg_threshold_contract\ui_precheck_report.md`
- Focused log panel smoke passed on the current implementation:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_log_panel_contract\ui_precheck_report.md`
- Main workspace smoke was strengthened:
  - The smoke now injects the Main image through the same layer-image update path used by real image loading.
  - It verifies that `Main` is stored as a non-placeholder image.
  - It verifies that the right-side layer/result list exposes the stored image size (`768x576`) instead of reporting a missing base image.
- Focused Main workspace smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_main_workspace_contract\ui_precheck_report.md`
- Latest full platform precheck passed after this scoped UX/contract pass:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_0800_pass\platform_precheck_report.md`
  - Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_0800_pass\platform_precheck_summary.json`
  - Gates: Build, XML compatibility, Sample Catalog Runner/Summary, Runner API, Tool Result, Sample Inventory/Algorithm, and UI Precheck all OK.
- Combined UI contract smoke passed for the changed surfaces:
  - Targets: `ai_recipe_failed_step_focus_check`, `ai_recipe_feedback_check`, `pipeline_property_grid_contract_check`, `log_panel_contract_check`, `main_workspace`
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_contract_0800_pass\ui_precheck_report.md`
- The existing `docs/samples/Rice_Particle_Blob.pipeline.xml` now runs through the same Sample Catalog and Platform Precheck gates as Contour/LineGauge/Matching/Mean/RotateScale.
- Algorithm contract now validates Blob result count, area average, bounds width average, overlay/result-count parity, final layer, and step flow.
- Latest sample catalog report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_rice_blob\sample_catalog_report.md`
- Latest platform report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_converter_summary\platform_precheck_report.md`
- Latest platform summary JSON: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_converter_summary\platform_precheck_summary.json`
- Scoped Sample UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_precheck_rice_blob_samples\ui_precheck_report.md`
- Scoped AI Recipe UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_precheck_ai_recipe_blob_prompt\ui_precheck_report.md`
- Metric recommendation lists now match execution-level derived metrics:
  - `Mean` now recommends rectangle bounds metrics because Mean ROI results produce rectangle overlays.
  - `OverlayMerge`/`ResultMerge` now recommend rectangle bounds metrics because merged review layers can produce rectangle overlay summaries.
  - Scoped contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_metric_recommendation_contract\ui_precheck_report.md`
- Sample Catalog reports now expose both human-readable and machine-readable summaries:
  - `sample_catalog_report.md` includes runnable/required/explore/OK/NG counts and a category summary table.
  - `sample_catalog_summary.json` includes the same counts, category totals, per-sample status, generated overlay image paths, result image paths, and raw log paths.
  - This makes the sample set usable from CI, external runners, and later LLM review loops without parsing console output.
  - The JSON now exposes `GateStatus`, `GateMessage`, `FailedSamples`, per-sample `ExitCode`, and per-sample `FailureMessages`.
  - The JSON now also exposes `ArtifactStatus`, `ArtifactFailureMessages`, `ArtifactIssueCount`, and `ArtifactIssues` so a sample cannot pass without result image, overlay image, and raw log artifacts.
  - The JSON now also exposes `MetadataStatus`, `MetadataFailureMessages`, `MetadataIssueCount`, and `MetadataIssues` so a sample cannot pass with a missing image, missing pipeline XML, or catalog/actual image size mismatch.
  - `sample_catalog_report.md` now shows the actual input image size beside the expected catalog size for every runnable row.
  - The JSON and Markdown report now expose `SampleFolderCoverage` and `UncoveredSampleFolders`.
  - Current sample-folder backlog is empty after adding generic Explore representatives:
    - `EasyMatrixCode_AutoRead_Contour`
    - `EasyOCR2_Characters_Contour`
  - Added `MasterImage_Left_Mean` as an Explore representative:
    - Image: `Sample\MasterImage\_20200912-105504298_L.bmp`
    - Recipe: `docs\samples\Contour_MeanBrightness.pipeline.xml`
    - Expected metric: `MeanValueAvg 254..256`
    - Current metric: `255`
  - Pipeline Samples now shows a `Catalog coverage` summary in the sample detail panel so users can see covered/backlog folders without reading the generated JSON.
  - Pipeline Samples check action now also treats catalog/actual image size mismatch as NG, matching the external sample catalog runner.
  - Latest scoped Sample Catalog UI smoke passed:
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_ui_metadata_check\ui_precheck_report.md`
  - `RunVisionPlatformPrecheck.ps1` now validates the JSON contents, not only file creation:
    - `OKRows` must match `RunnableRows`.
    - `NGRows` must be `0`.
    - Every category must have `OK == Total` and `NG == 0`.
    - `GateStatus` must be `OK`.
    - `FailedSamples` must exist and be empty.
    - `ArtifactIssueCount` must be `0` and `ArtifactIssues` must be empty.
    - `MetadataIssueCount` must be `0` and `MetadataIssues` must be empty.
  - Latest catalog report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_runtime_metadata\sample_catalog_report.md`
  - Latest catalog JSON: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_runtime_metadata\sample_catalog_summary.json`
- Platform Precheck now also writes `platform_precheck_summary.json` for CI, LLM review loops, and external automation:
  - It records overall status, duration, gate list, sample catalog counts, artifact issue count, metadata issue count, sample runner duration, runner executable path, sample-folder backlog count, and generated artifact paths.
  - Latest platform report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_multi_metric\platform_precheck_report.md`
  - Latest platform summary JSON: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_platform_precheck_multi_metric\platform_precheck_summary.json`
- Sample Catalog metric gates now support multiple expected metrics per row:
  - `Contour_RotateScale_Resize` validates both `ResultImageWidth` and `ResultImageHeight`.
  - `Rice_Particle_Blob` validates both `ResultCount` and `BoundsWidthAvg`.
  - `BentPin_GoodShaft` and `BentPin_BadShaft` validate shaft width in pixels, shaft width in mm, and `ResultCount`.
  - `Pins_LineGauge` validates `EdgeCount`, `LineLengthMax`, `LineLengthMmMax`, and `LineAngleAvg`.
  - Focused CLI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_multi_metric\sample_catalog_report.md`
  - Focused UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_multi_metric_ui\ui_precheck_report.md`
- The latest sample metric pass strengthens the catalog beyond count-only checks:
  - `Contour_AllSymbolsAndFaint_LLM` now validates final merged overlay count and merge source count.
  - `Rice_Particle_Blob` now validates count, average bounds width, and average area.
  - `BentPin_Large` and `EasyGauge_BentPin_Large` now validate count, maximum area, and average area.
  - `DiePad*_Surface` and `EasyMatch_DiePad*_Surface` now validate count, maximum area, and average area.
  - `Pins_LineGauge` and `EasyGauge_Pins_LineGauge` now validate edge count, maximum fitted-line length in pixels/mm, and average angle.
  - `Contour_TemplateMatching` now validates score and result count.
  - Latest strengthened catalog report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_tool_guide_metrics\sample_catalog_report.md`
  - Latest strengthened catalog JSON: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_catalog_tool_guide_metrics\sample_catalog_summary.json`
- Pipeline Samples and AI Recipe now surface the same expected-vs-actual metric gate information:
  - Sample metric review UI: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_metric_review\ui_precheck_report.md`
  - Sample catalog run UI: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_metric_report\ui_precheck_report.md`
  - AI Recipe sample-gate prompt: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_sample_gate_prompt\ui_precheck_report.md`
  - Final focused UI smoke after the strengthened catalog update: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_guide_metric_ui_final\ui_precheck_report.md`
  - Pipeline Samples recipe-guide detail smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_recipe_guide_contract\ui_precheck_report.md`
- The in-program tutorial now includes a tool-specific test guide for Contour, Blob, Pattern Matching, EdgeDetection, LineGauge, and distance/Pixel-mm measurement workflows:
  - Tool guide contract: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_guide_contract\ui_precheck_report.md`
- Pixel/mm measurement metrics now have sample-backed gates:
  - `VisionPipelineMetricEnrichmentService` derives `BoundsWidthMm*`, `BoundsHeightMm*`, and `LineLengthMm*` from `PIXELPERMM`.
  - Acceptance presets now cover both rectangle width and height so Blob/Contour/Corner size gates can be expressed in either px or mm.
  - BentPin shaft samples validate `BoundsWidthMmMax` together with pixel width and result count.
  - Pins LineGauge samples validate `LineLengthMmMax` together with edge count, pixel line length, and angle.
  - Sample Catalog report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_contract\sample_catalog_report.md`
  - Sample Catalog summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_contract\sample_catalog_summary.json`
  - Focused UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_metric_ui_contract\ui_precheck_report.md`
  - AI Recipe feedback wait-fix report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_feedback_wait_fix\ui_precheck_report.md`
  - Platform report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_platform_skipui\platform_precheck_report.md`
  - Platform summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_platform_skipui\platform_precheck_summary.json`
- Matching rotated fixture contract smoke now uses a stronger asymmetric fixture so angle-search regressions are caught without false 0-degree wins:
  - Focused report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_matching_rotated_fixture_fix\ui_precheck_report.md`
- Sample Catalog guide text now converts expected metrics into operator-facing check points:
  - Examples: detected object count, object width in px/mm, fitted line length/angle, matching score, mean brightness, and output image size.
  - Pipeline Samples and AI Recipe both consume the same `RecipeGuideText`, so the sample UI and generated LLM prompt now explain what the user should verify rather than only listing metric names.
  - Focused report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract2\ui_precheck_report.md`
- Latest final non-UI platform precheck after measurement documentation and Matching fixture updates:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_docs_final_skipui2\platform_precheck_report.md`
  - Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_measurement_docs_final_skipui2\platform_precheck_summary.json`
- Latest final non-UI platform precheck after Sample Check guide contract update:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract_final_skipui\platform_precheck_report.md`
  - Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_sample_check_guide_contract_final_skipui\platform_precheck_summary.json`
- Platform Precheck now includes the sample-backed Pipeline Tool Result Contract:
  - `pipeline_tool_result_contract_check` runs all runnable catalog recipes through `VisionRecipeRunner`.
  - Every successful non-skipped step must expose `Status=OK`, `ResultStatus=Passed`, `ErrorCode=0`, `ErrorName=None`, `AcceptancePassed=true`, and no failure diagnostic/fix text.
  - Invalid imported/XML steps with a missing `ToolType` now fail as `ToolFactoryFailed` / `ConfigurationError` and produce action-summary and suggested-fix text instead of falling into a factory exception.
  - The latest `-SkipUi` platform precheck passed with this stricter gate.
- Pipeline Run Report now persists step diagnostics:
  - `DiagnosticHint` and `SuggestedFix` are stored in `report.xml`.
  - History and Batch Step grids display Error, Result, Diagnostic, and Suggested Fix columns so failed runs are reviewable after restart.
  - Focused contract report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_tool_contract_report_xml\ui_precheck_report.md`
  - Focused designer report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_designable_report_columns\ui_precheck_report.md`
- ImageCanvas WPF converters no longer throw `NotImplementedException` from `ConvertBack`:
  - `BooleanToEyeIconConverter` and `BooleanToColorConverter` now return `Binding.DoNothing`.
  - This prevents accidental two-way binding or template refresh paths from killing the UI.
- AI Recipe prompt guidance now includes the validation loop explicitly:
  - Imported XML is validated, previewed, and reviewed through step metrics, overlays, result image, overlay image, and raw log.
  - The prompt tells the LLM that usable recipes should reach `GateStatus=OK`, `ArtifactIssueCount=0`, and `MetadataIssueCount=0`.
  - Retry prompts tell the LLM to use the first failed step, error code, diagnostic hint, suggested fix, and metrics before rewriting successful steps.
  - Retry prompts now explicitly preserve successful previous steps and stable output layer names, and limit edits to the first failed step plus directly dependent steps unless layer flow is the root cause.
  - Scoped AI Recipe feedback smoke: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_retry_scope\ui_precheck_report.md`
- Sample Inventory contract now checks recipe catalog coverage:
  - Every `docs/samples/*.pipeline.xml` must be covered by the Sample Catalog unless it is explicitly listed as an uncataloged template/example.
  - Current explicit exception: `Filter_Edge_Line.pipeline.xml`.
  - Scoped report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ui_precheck_recipe_catalog_coverage\ui_precheck_report.md`

Current expected sample metrics:

| Sample | Recipe | Expected | Current |
| --- | --- | --- | --- |
| Contour_TextSymbols | Contour_TextSymbols | ResultCount 35-80 | 51 |
| Contour_AllSymbolsAndFaint_LLM | Contour_AllSymbolsAndFaint_LLM | MergeOverlayCount 37-100; MergeSourceCount 3-3 | 55; 3 |
| Contour_Generic | Threshold_Morphology_Contour | ResultCount 10-30 | 21 |
| Contour_MeanBrightness | Contour_MeanBrightness | MeanValueAvg 250-256 | 254.7 |
| Contour_RotateScale_Resize | Contour_RotateScale_Resize | ResultImageWidth 384-384; ResultImageHeight 288-288 | 384; 288 |
| Rice_Particle | Rice_Particle_Contour | ResultCount 100-170 | 123 |
| Rice_Particle_Blob | Rice_Particle_Blob | ResultCount 120-170; BoundsWidthAvg 15-35; AreaAvg 250-400 | 143; 24.105; 320.762 |
| Pins_Feature | Pin_Feature_Contour | ResultCount 40-70 | 54 |
| BentPin_Large | BentPin_LargeContour | ResultCount 1-5; AreaMax 100000-300000; AreaAvg 90000-260000 | 2; within range; within range |
| BentPin_TopBottom_Overlay | BentPin_TopBottom_Overlay | MergeOverlayCount 2-2 | 2 |
| BentPin_GoodShaft | BentPin_ShaftContour | BoundsWidthMax 0-18; BoundsWidthMmMax 0-0.108; ResultCount 13-13 | 14; within range; 13 |
| BentPin_BadShaft | BentPin_ShaftContour | BoundsWidthMax 24-40; BoundsWidthMmMax 0.144-0.24; ResultCount 13-13 | 26; within range; 13 |
| DiePad1_Surface | DiePad_Surface_Contour | ResultCount 8-25; AreaMax 45000-90000; AreaAvg 2500-12000 | 11; within range; within range |
| DiePad2_Surface | DiePad_Surface_Contour | ResultCount 8-25; AreaMax 45000-90000; AreaAvg 2500-12000 | 14; within range; within range |
| DiePad3_Surface | DiePad_Surface_Contour | ResultCount 8-25; AreaMax 45000-90000; AreaAvg 2500-12000 | 16; within range; within range |
| DiePad4_Surface | DiePad_Surface_Contour | ResultCount 8-25; AreaMax 45000-90000; AreaAvg 2500-12000 | 14; within range; within range |
| Pins_LineGauge | Pins_Edge_LineGauge | EdgeCount 30-70; LineLengthMax 500-900; LineLengthMmMax 3-6; LineAngleAvg -20-20 | 47; 741.852; within range; within range |
| Contour_TemplateMatching | Contour_Template_Matching | ScoreMax 90-100; ResultCount 1-3 | 99.177; within range |
| EasyObject_SurfaceDefect1_Edge | SurfaceDefect_EdgeContour | ResultCount 1-20 | 5 |
| EasyObject_SurfaceDefect2_Edge | SurfaceDefect_EdgeContour | ResultCount 20-60 | 36 |
| MasterImage_Left_Mean | Contour_MeanBrightness | MeanValueAvg 254-256 | 255 |

1. Test process cleanup
   - Added `tools/StopUiSmoke.ps1`.
   - It only targets `PipelineViewerScreenshotSmoke.exe`.
   - It does not stop `OpenVisionLab.exe`.
   - Current environment still has smoke processes that cannot be stopped automatically because Windows returns access denied.

2. UI smoke execution safety
   - `tools/RunUiScreenshotSmoke.ps1` now builds the smoke executable first, runs selected targets by default, and applies a timeout.
   - Its default quick target set now includes `pipeline_property_grid_contract_check`, `pipeline_sample_open_preview`, and `pipeline_sample_llm_open_preview` so WPG, sample, and LLM UI regressions are caught without running `--all`.
   - `tools/RunUiPrecheck.ps1` no longer runs `--all` by default.
   - `-All` is now explicit.
   - The precheck report records targets, timeout, raw output, and image links.

3. Pipeline Check UX
   - Branch and duplicated preprocessing messages now use review language instead of a hard warning tone.
   - Check logs now use `CHECK REVIEW` for review items.
   - The UI message says the flow is valid but review is recommended when the pipeline has intentional branch-like behavior.

4. Branch flow stabilization
   - Branch input is treated as a review item when a step reads a different layer than the previous step output.
   - This matches the current UX direction: branching is allowed, but the user should confirm it intentionally.

5. Add Step / chain UX coverage
   - Smoke targets exist for:
     - `pipeline_add_step_form`
     - `pipeline_add_step_branch_form`
     - `pipeline_form_branch`
     - `pipeline_form_branch_check`
   - The branch check target validates branch-review behavior without opening the full pipeline form.

6. Threshold form coverage
   - `threshold_form` remains included in the default UI smoke target list.
   - Default quick UI smoke captures the Threshold form and verifies layout/text integrity.

7. Build verification
   - `tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj` builds successfully.
   - `OpenVisionLab.sln` builds successfully.
   - The first build attempt failed only because the sandbox blocked SDK cache access, not because of source errors.

## Current Verification Status

| Area | Status | Note |
| --- | --- | --- |
| Solution build | OK | Full Debug / Any CPU build passed. |
| Smoke project build | OK | Screenshot smoke tool builds. |
| UI smoke script safety | OK | Timeout and selected-target defaults added. |
| Smoke process cleanup | OK | `tools/StopUiSmoke.ps1` exists for targeted cleanup; current UI smoke runs complete through timeout-guarded scripts. |
| Pipeline Check message logic | OK | Code confirms review wording and log level mapping. |
| Branch validation | OK | UI smoke target passed. |
| Add Step UX smoke targets | OK | UI smoke targets passed. |
| Pipeline designer constructor contract | OK | `pipeline_designable_forms` is included in default UI smoke and passed. |
| Pipeline Samples catalog UX | OK | Scoped smoke targets `pipeline_samples_form`, `pipeline_samples_check_action`, `pipeline_samples_pins_line_check_action`, `pipeline_sample_open_preview`, and `pipeline_sample_llm_open_preview` passed. Latest sample-preview smoke passed after adding the Rice Blob Required row. |
| Pipeline PropertyGrid/WPG contract | OK | `pipeline_property_grid_contract_check` passed; Range helper rows are hidden and WPF rendering does not expose internal WPG type names. |
| Log panel contract | OK | `log_panel_contract_check` passed; normal filter levels are Any/Info/Warning/Error, Debug is not exposed in the operator filter, and All Logs explicitly reports that filters are off. |
| Threshold visual smoke | OK | Default UI precheck target passed. |
| AI Recipe form smoke | OK | Default UI precheck target passed; scoped AI Recipe smoke also verifies that the prompt includes the Required Blob sample recipe. |
| Message box smoke | Optional | Available as explicit smoke targets, not included in default UI precheck. |
| Sample catalog runner | OK | Required rows and recursive Explore representatives passed, including LLM OverlayMerge, Blob, LineGauge, Matching, EasyImage, EasyGauge, EasyMatch, EasyObject, EasyColor, EasyFind, EasyBarCode, EasyQRCode, EasyOcr, and MasterImage sample paths. The report now includes category summary, GateStatus, failed sample list, per-sample failure messages, input image metadata validation, artifact validation, and sample-folder coverage/backlog. |
| Sample inventory contract | OK | `sample_inventory_contract_check` scans `Sample` recursively, verifies representative folders, requires recursive catalog representatives across image/gauge/match/object/color/find/barcode/QR/OCR groups, reports uncovered optional folders, and fails if a recipe XML is not cataloged or explicitly listed as a template exception. |
| LineGauge helper cleanup | OK | Shared pair execution and no-result/mismatch guards build and pass algorithm/Pins line sample smokes. |
| Tool Result Contract | OK | Every non-None `VisionToolErrorCode` resolves to an expected status and non-empty Hint/Fix text; runnable catalog steps also verify OK/Passed/ErrorCode=0/AcceptancePassed status consistency, invalid step configuration returns actionable ToolFactoryFailed diagnostics, and failed-step RunReport XML persists DiagnosticHint/SuggestedFix. |
| Runner API contract | OK | `VisionRecipeRunner` exposes outcome, final layer, final metrics, overlay-derived bounds metrics, overlays, first-failure summary, normalization summary, action summary, and step-flow summary; platform precheck validates OK and NG paths. |
| Platform precheck | OK | Latest `-SkipUi` platform precheck passed: Build, XML, samples, Sample Catalog JSON content/GateStatus/artifact/metadata/folder-coverage gate, Runner API OK/NG/action contract, enum Tool Result Contract, sample-backed Pipeline Tool Result Contract, Sample Inventory Contract, and Algorithm Sample Contract. |

## Latest Pipeline Clarity/LLM Contract Pass

- Selected Step I/O now distinguishes a normal chained input from a review branch more directly.
- When a later step reads `Main` instead of the previous output, the status explicitly says this is a review branch and suggests `Link Prev` unless the step intentionally starts again from the original image.
- Pipeline Flow branch text now carries the same rule so users can identify accidental `Main` reuse without opening the property grid.
- The LLM prompt and recipe contract now state that `Main` should stay the original reference image, later processing should normally read the previous `OutputLayer`, and Good/Bad sample pairs should be used to set conservative acceptance gates.
- Focused UI smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_llm_clarity_check\ui_precheck_report.md`
- Strengthened Pipeline branch review smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_branch_review_contract\ui_precheck_report.md`
- Guide/tutorial contract now verifies the new Input/Output flow checklist and Good/Bad sample-pair workflow:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tutorial_flow_guide_contract\ui_precheck_report.md`
- Platform precheck with scoped Pipeline/LLM UI smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_llm_clarity_platform\platform_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_pipeline_llm_clarity_platform\ui\ui_precheck_report.md`

## Latest Tool Form Result Contract Pass

- Standalone Tool Form execution now carries the same core result fields used by Pipeline/Runner:
  - `ResultStatus`
  - `ErrorCode`
  - `ErrorName`
  - `MetricCount`
  - `OverlayCount`
- `VisionTestForm.ExecuteVisionTool(...)` stores the latest `VisionToolResult`, and `RunVisionStep(...)` publishes that information through `VisionToolRunEventArgs`.
- Main-side Tool Run summaries now preserve Metric/Overlay/Error/ResultStatus fields instead of flattening standalone Tool Form results to only OK/NG text.
- Tool run logs now include `ResultStatus`, `ErrorCode`, and `ErrorName` where available.
- `tool_result_status_contract_check` now also verifies standalone Tool Form notification contracts.
- Legacy direct Tool Forms that publish output without `ExecuteVisionTool(...)` now record a `VisionToolResult` before publish:
  - `FormVision_Arithmetic`
  - `FormVision_Histogram`
  - `FormVision_Line`
  - `FormVision_RotateAndScale`
- The obsolete typo form `FormVision_EdgeDection` is excluded from project compilation; the active menu/smoke path is `FormVision_EdgeDetection`, which uses `EdgeDetectionTool`.
- Direct results record image size/channel metrics so Main-side summaries and logs can distinguish `Passed/None` from true `NG/ErrorCode` cases.
- `FormVision_HSV` remains excluded from the Tool Run contract because it is a timer-based preview form that uses `PublishPreviewBitmap(...)`, not a formal inspection Run path.
- `tool_result_status_contract_check` now also verifies that direct legacy forms keep this result-recording contract.
- Focused UI smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_run_notification_contract2\ui_precheck_report.md`
- Platform precheck with scoped Tool Result/Main UI smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_run_notification_platform\platform_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_tool_run_notification_platform\ui\ui_precheck_report.md`
- Latest focused UI smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_direct_tool_form_contract\ui_precheck_report.md`
- Latest platform precheck passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_direct_tool_form_platform\platform_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_direct_tool_form_platform\ui\ui_precheck_report.md`
- Obsolete EdgeDetection typo-form cleanup smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_obsolete_edge_form_contract\ui_precheck_report.md`

## Latest WPG Property Editor Pass

- LineGauge tuning properties now use the shared WPG slider/number-range editor contract:
  - `CONTRAST`
  - `THICKNESS`
  - `SAMPLING_STEP`
  - `POINT_RANGE`
  - `MANUAL_ANGLE_VALUE`
  - `EXTEND_FIT_LINE_VALUE`
  - `AVERAGE_Diff`
- This keeps frequently tuned LineGauge values out of plain text-only editing and aligns the tool with Threshold/Range editor behavior already used by Contour, Blob, Matching, and Mean.
- `pipeline_property_grid_contract_check` now verifies these LineGauge editor attributes and number ranges.
- Focused WPG contract smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_linegauge_wpg_contract\ui_precheck_report.md`
- Platform precheck passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_linegauge_wpg_platform\platform_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_linegauge_wpg_platform\ui\ui_precheck_report.md`

## Latest Tutorial Portable Export Pass

- `OPENVISIONLAB_TUTORIAL.html` is kept as the maintainable source document with relative `docs/assets/tutorial` image references.
- Added `tools/BuildPortableTutorial.ps1` to generate a one-file tutorial for copying or sharing outside the repository.
- The converter handles both double-quoted and single-quoted local `<img src=...>` paths, and fails fast if any local image is missing.
- Generated `docs/OPENVISIONLAB_TUTORIAL_PORTABLE.html`; all local tutorial images are embedded as `data:image/...` URIs.
- Verification:
  - `img=25`
  - `data:image` sources: `25`
  - remaining `assets/` image sources: `0`
- The source tutorial now includes a visible note telling users to use `OPENVISIONLAB_TUTORIAL_PORTABLE.html` when copying a single HTML file.

## Latest 2026-06-17 Self Evaluation

- Non-UI platform precheck passed:
  - Report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_self_eval_20260617_skipui\platform_precheck_report.md`
  - Summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_self_eval_20260617_skipui\platform_precheck_summary.json`
- Gates:
  - Build: `OK`
  - XML Compatibility: `OK`
  - Sample Catalog Runner: `OK`
  - Sample Catalog Summary: `OK`
  - Runner API Contract: `OK`
  - Tool Result Contract: `OK`
  - Sample Inventory And Algorithm Contract: `OK`
- Sample catalog result:
  - Runnable rows: `37`
  - Required rows: `21`
  - Explore rows: `16`
  - OK rows: `37`
  - NG rows: `0`
  - Categories: `29`
  - Failed samples: `0`
  - Artifact issues: `0`
  - Metadata issues: `0`
  - Uncovered sample folders: `0`
- Current assessment:
  - Core Pipeline/Runner/Tool contracts are stable enough to treat the platform backbone as validated.
  - Remaining work should focus less on broad refactoring and more on targeted product quality: richer sample recipes, UI/operator clarity, shared property editors, and packaging/version policy.
- 2026-06-17 follow-up hardening:
  - WPG common editor metadata was expanded for `Threshold`, `Morphology`, `Filter`, and `RotateScale` pipeline properties.
  - `pipeline_property_grid_contract_check` now asserts shared slider/range editor contracts for these tools.
  - UI smoke result: `OK`, with a visual `WARN` only from the current WPG empty surface flatness check.
  - AI Recipe prompt rules now explicitly reject form-only/demo-only ToolTypes and require one final review layer for branched detections.
  - Added `docs/OPENVISIONLAB_RUNNER_TOOLTYPE_COVERAGE.md` to separate runner-supported ToolTypes from form-only/demo features.
  - Regenerated `docs/OPENVISIONLAB_TUTORIAL_PORTABLE.html`; it embeds `25` tutorial images and has no remaining `assets/tutorial` references.
- Final verification for this pass:
  - Targeted UI report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_prompt_contract_20260617_b\ui_precheck_report.md`
  - Final platform report: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_self_eval_20260617_contract_final\platform_precheck_report.md`
  - Final platform summary: `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_self_eval_20260617_contract_final\platform_precheck_summary.json`
  - Final gates: Build, XML Compatibility, Sample Catalog Runner, Sample Catalog Summary, Runner API Contract, AI Recipe Prompt Contract, Tool Result Contract, Sample Inventory/Algorithm Contract, and Tutorial Portable Contract are all `OK`.
  - Final sample catalog: `37` runnable rows, `37` OK, `0` NG, `0` failed samples, `0` artifact issues, `0` metadata issues, and `0` uncovered sample folders.
  - Portable tutorial contract: source image tags `25`, embedded images `25`, gate `OK`.
  - AI Recipe Prompt Contract: supported ToolTypes, form-only ToolType guard, sample-backed metric guidance, and final OverlayMerge review rule are contract-tested.

## Completion Estimate

These are practical estimates for product readiness, not code quantity.

Overall product readiness is about **98%**.

The core platform direction is now correct:

- The user can build and run step-based recipes.
- Input/output layer flow is explicit enough for chained and branched inspections.
- Samples can validate real image behavior with expected metrics.
- AI Recipe output has a concrete XML and final-review contract.
- External runner validation proves that UI-created XML can run outside the UI path.

The remaining risk is not basic feasibility. The remaining risk is inspection depth and UX finish: more defect-specific recipes, stronger property editors, clearer result explanations, and tighter packaging/version policy.

| Area | Completion | Remaining Work |
| --- | ---: | --- |
| Main viewer and layer workspace | 92% | Main workspace smoke now validates real layer-image storage, right-side source/result role text, stored image size, and top toolbar layer/source/flow state; remaining work is broader interaction polish and operator trial feedback. |
| Tool standardization | 94% | Core tool result/status contracts are aligned, standalone Tool Form notifications now preserve ResultStatus/ErrorCode/Metric/Overlay fields, direct legacy Tool Forms record `VisionToolResult` before publish, obsolete `FormVision_EdgeDection` is excluded from compilation, all ErrorCodes have diagnostic coverage, common parameter-error fixes are explicit, runner action summaries are contract-tested, and run reports retain diagnostic/fix text; remaining work is tool-specific UX review and final obsolete file removal policy. |
| Pipeline UX | 95% | Input/output, branch review, sample context, preview/publish separation, metric-based acceptance, History/Batch diagnostic review, explicit Pipeline Flow image-action text, and Review Branch/Link Prev contracts are now stable; remaining work is refinement and operator trial feedback. |
| Pipeline persistence and samples | 100% | Catalog, sample image load, expected metrics, broader recursive sample inventory, defect-specific representatives, basic Tool representatives, MasterImage, EasyMatrixCode, and EasyOCR2 representative coverage, preview flow, category summary, md/json runner reports, failed-sample/artifact/metadata/folder-coverage JSON fields, zero uncovered sample folders, and JSON content gates are validated; restart/load edge cases and semantic decoder/OCR validation remain separate future work. |
| Result metrics and overlays | 95% | Contour, Blob, LineGauge, Matching, OverlayMerge, rectangle bounds metrics, line length/angle metrics, Pixel/mm-derived bounds/line metrics, and metric-based Acceptance are validated; Sample Catalog rows now gate stronger multi-metric decisions such as count/area, count/bounds, px/mm size, edge-count/line-length/line-length-mm/angle, score/count, and merge overlay/source counts; remaining work is calibration UX and more measurement-specific sample recipes. |
| Logging and message UX | 94% | Log panel level/filter contract is smoke-tested, operator levels are simplified, active-filter wording is explicit, All Logs reports that filters are off, message details/copy actions are clearer, and the current WPF rendering path passed focused smoke; remaining work is message taxonomy final review. |
| Threshold/WPG editors | 92% | Pipeline WPG Threshold/Range editor contract is smoke-tested, duplicate Range helper rows are hidden, LineGauge tuning values now use shared slider/number-range editor metadata, Threshold form mode/input/output/purpose text is contract-tested, and layout/text/internal checks pass; shared editor reuse still needs final consolidation and a stricter visual pass for flat-looking property surfaces. |
| AI Recipe workflow | 96% | Prompt contract, supported ToolType guard, form-only ToolType rejection, final OverlayMerge rule, validation feedback, validation-loop guidance, retry edit-scope guidance, visible first-failed-step retry preview, first-failed-row focus, direct-dependent-step feedback, tool-specific patch proposals, Required/Explore sample prompt separation, expected-gate examples, Good/Bad acceptance-pair guidance, sample metric-to-check guidance, distance/size metric guidance, parameter-error diagnostics, and LLM sample UI smoke exist; generated recipe tuning loop still needs interactive parameter/layer-flow editing rather than text-only guidance. |
| External runner/DLL path | 95% | XML runner, CLI smoke, sample catalog execution, multi-metric sample gates, machine-readable sample summary JSON with GateStatus/failed-sample/artifact/metadata/folder-coverage/runtime fields, platform-level summary JSON, Runner API OK/NG summary contracts, action summary, step-flow summary, and sample-backed Tool Result status contract are stable; package/version policy remains. |
| Algorithm robustness | 96% | Sample-backed Contour, Blob, LineGauge, Matching, Mean, RotateScale, Threshold channel normalization, line-gauge helper guards, Blob Required sample coverage, broader recursive representatives, execution-level overlay bounds metrics, line length/angle metrics, Pixel/mm bounds/line metrics, metric-based Acceptance, BentPin branch/merge ROI contract, BentPin good/bad shaft-width px/mm contract, DiePad geometry contract, SurfaceDefect edge-contour contract, category-level sample reporting, and successful-step status contracts are stronger; more NG/OK paired defect contracts and tool-specific summary rows still need expansion. |
| Automated UI QA | 98% | Scoped screenshot smoke, designer constructor check, catalog checks, recursive sample contracts, recipe catalog coverage gate, WPG contract check, AI Recipe prompt contract check, log contract, MessageBox contract, Main/Pipeline/Threshold UI 95 pass, Runner API gate, enum/sample-backed Tool Result gates, Sample Catalog JSON/artifact/metadata gate, sample metric review checks, quick/default LLM sample UI validation, Guide/tool-guide document resolver check, tutorial portable contract, strengthened Branch Review contract, backlog-none sample UI contract, and fallback capture exist; visual regression thresholds can still be stricter. |

## Immediate Next Decisions

After the current UX pass is verified, choose one of these tracks.

Recommended order after the current 98% checkpoint:

1. Interactive AI Recipe tuning
   - Convert text-only retry guidance into editable step/parameter/layer-flow actions.
   - Let the user select a failed step and apply a suggested fix without manually editing XML.
   - Keep `Main` overwrite prevention and final `OverlayMerge` review contract enforced.

2. More Good/Bad inspection pairs
   - Add defect-specific OK/NG pairs beyond the current bent-pin shaft-width coverage.
   - Prioritize pin, die-pad, surface defect, and line/measurement samples.
   - Gate each pair with one explainable metric such as count, bounds width/height, line length, angle, score, or mean value.

3. PropertyGrid visual finish
   - Keep the current WPG editor contracts, but reduce the flat/empty surface feeling.
   - Prefer shared editor templates over form-specific UI work.
   - Do this after the contract checks remain green, not before.

4. Package/version policy
   - Define how `Library-Noah` and `WPG-CUSTOM` are referenced on a new PC.
   - Decide whether release builds use source references, binary packages, or documented external roots.
   - Add a preflight check that reports missing external roots before build.

5. Tool-specific operator guidance
   - Add compact guides for Contour, Blob, Matching, FeatureMatching, LineGauge, and measurement workflows.
   - Show which input/output layer should be used, which metric matters, and what a common NG means.

1. Pipeline clarity track
   - Make input/output image flow even more explicit.
   - Add a step detail surface that shows input image, output image, output layer, and branch reason.
   - Improve Add Step so the recommended input defaults to the previous step output, while branch input requires explicit confirmation.

2. Algorithm reliability track
   - Use `Sample/Contour.jpg` as the first benchmark.
   - Create stable recipes for text/symbol contour detection.
   - Store expected metrics such as result count, area range, and elapsed time.

3. Threshold and WPG editor track
   - Move range threshold and threshold-with-invert editors into the shared WPG/control library.
   - Keep forms designer-friendly.
   - Make Threshold form and pipeline property grid use the same editor behavior.

4. AI Recipe track
   - Define the LLM prompt and XML schema contract.
   - Let LLM generate a first-pass pipeline XML.
   - OpenVisionLab validates, previews, and highlights review items before users accept it.

5. External execution track
   - Harden `VisionRecipeRunner`.
   - Define the DLL/API surface.
   - Guarantee that UI-created XML runs without UI dependencies.

## Recommended Next Step

The best next step is the Algorithm reliability track, while keeping the Pipeline clarity UX polished.

Reason:

- The pipeline UX now explains input/output flow well enough to start validating real inspection behavior.
- The sample catalog gives us repeatable images, XML, overlays, and metrics.
- AI Recipe quality depends on having reliable sample-backed recipes to imitate.

Recommended concrete work:

1. Add per-sample result review UI that shows expected metric versus actual metric after preview.
2. Feed the same sample catalog into AI Recipe so generated XML can follow known good patterns.
3. Add a small `Learn` or `Recipe Guide` panel that explains why each sample uses its threshold/morphology/contour settings.
4. Expand paired OK/NG contracts where sample images provide stable metrics.
5. Keep UI smoke scoped to the changed surface; only run full capture when checking cross-window regressions.

## Verification Commands

Default UI precheck:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1
```

Full visual capture should remain explicit:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -All
```

Use scoped targets for focused UI work:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets main_workspace
```

Platform precheck can pass the same scoped UI target:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -UiTargets main_workspace
```

If a previous UI smoke process is still running, clean up only the smoke executable:

```powershell
powershell -ExecutionPolicy Bypass -File tools\StopUiSmoke.ps1
```
