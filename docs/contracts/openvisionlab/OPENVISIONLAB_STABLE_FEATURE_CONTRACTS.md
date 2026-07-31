# OpenVisionLab Stable Feature Contracts

Last updated: 2026-07-29

This document protects restored and verified behavior. Future LLM or developer work must read this before changing WPF shell, tool view, layer routing, viewer, or PropertyGrid code.

## CVR-11 Edge Global Polarity v1

- `ALLOW_GLOBAL_POLARITY_REVERSAL` is opt-in and defaults to `false` when
  absent from Pipeline/XML.
- Same-only mode retains the legacy signed gradient-direction score.
- Enabled mode may choose only one whole-candidate global reversal. Do not
  replace it with per-edge absolute direction or local polarity ignore.
- Successful matches publish exact `Same`/`Reversed` state through
  MatchingResult, `GlobalPolarity.*` metrics, and the drawing label.
- Existing score, unique-match, search ROI, angle, scale, result-count, and
  suppression behavior remains active.
- Tool View/PropertyGrid edits must not automatically Preview/Run or mutate
  layers/routing.
- A present non-Boolean Pipeline value fails validation; missing values restore
  `false`.
- Synthetic completion does not authorize enabling reversal in a qualified
  physical recipe. Follow
  `OPENVISIONLAB_EDGE_GLOBAL_POLARITY_V1_CONTRACT.md` for the required physical
  evidence boundary.

## Purpose

OpenVisionLab is a multi-layer vision workbench. Operators load images into named layers, select input/output layers explicitly, tune tools, compare layers, and then add verified steps to pipelines.

When a feature below is marked stable, do not refactor, simplify, replace, or remove it unless the user explicitly asks to change that behavior. If a change must touch a stable path, run the listed smoke targets before claiming completion.

## General Rules

- Do not remove existing operator affordances while refactoring: layer selection, output layer creation, preview image load/save, zoom, pan, drag, result review, and PropertyGrid editing are product behavior, not incidental UI.
- A reusable workflow must be designed from the operator goal and the shortest
  safe normal path. Related settings must not be scattered across unrelated
  views, dialogs, or buttons merely because the implementation has separate
  components.
- Related settings that form one durable task must use one coherent first-use
  setup or option surface. After explicit operator confirmation, persist them
  at the narrowest correct Tool, Recipe, project, workspace, or user scope.
- Restored setup must remain visible and editable, provide an explicit
  reset/default path, validate stale or incompatible values before reuse, and
  explain why rejected state cannot be restored.
- Restoring setup must not run Preview/Run, create/delete/select layers, change
  the active layer, or mutate Pipeline routing. Reusable setup is stable only
  after save/reload/reopen and zero-side-effect verification pass.
- Task-specific ROI, tolerance, template, dependency, and coordinate-frame
  state must not leak across unrelated Recipes, projects, workspaces, or users.
- Do not auto-change an input layer just because an output layer was created or previewed.
- Do not auto-split, auto-rearrange, or auto-create comparison panes just because an output layer was created or previewed. Live layers may be mirrored into the central AvalonDock same-pane tab workspace by default, but comparison placement remains an explicit operator action.
- Layer management commands are explicit operator actions. Creating a layer, loading an image into a selected/current layer, renaming an operator layer, or deleting a non-Main layer must not run Preview/Run, auto-open a tool, or change the active native tool input route.
- Output/input routing remains explicit. Loading or creating an operator layer may activate that layer in the main workspace for viewing, but it must not silently rewrite a tool's selected input layer.
- Deleting a layer must also remove stale docked-layer documents for that layer and fall back to an existing layer, normally `Main`, without leaving blank white panes or dead tabs.
- Renaming a layer must preserve the layer image and refresh host/docked titles. `Main` is the stable default layer name and must not be renamed through the operator-facing rename command.
- Keep completed behavior covered by focused smoke tests. Run the smallest target that covers the changed path.
- Do not replace a PropertyGrid tool with hand-written controls unless a separate design explicitly says that tool is no longer PropertyGrid based.
- Large-corpus recipe evidence must not equate execution success with semantic correctness. Preserve source/result hashes and current-run drawings, select the review queue deterministically, and stop per-image tuning according to `OPENVISIONLAB_SCALABLE_SKILL_VALIDATION_PROTOCOL.md`.
- MainView image-ready guidance is display-only. Showing a next-action bar, quick tool buttons, or top status banner state must not auto-open a tool, run Preview, create an output layer, or change the selected input layer.
- Tool rail readiness badges are display-only. The initial state may report `입력 없음` when `Main` has no image and `설정 가능` when a Main image exists, but `설정 가능` must not be presented as proof that template, second input, ROI, calibration, or other tool-specific Preview requirements are complete. Readiness refresh must not disable tool selection, open a tool, run Preview/Run, create or select a layer, or change input/output routing.
- Matching-family template readiness must reuse the first recipe-owned `MatchingProperty`, `EdgeBasedMatchingProperty`, or `FeatureMatchingProperty` and its loaded template status. With `Main` ready, a missing or invalid template may show `템플릿 필요`; PropertyGrid template registration may refresh that display, but the refresh must not execute a tool or mutate layer or routing state.
- Arithmetic second-input readiness must reuse `VisionPipelineArithmeticStep.RequiresInputLayerB` and the persisted `ArithmeticToolSettings`. When the current setting requires B and fewer than two non-placeholder image layers exist, the Tool rail may show `B 입력 필요`. This is an advisory setup state, not proof that the eventual A/B routes or image sizes are compatible, and settings/layer refresh must not execute a tool or mutate layer or routing state.
- Tool rail search filters the existing tool/group visibility only. It may match canonical bilingual tool names, inspection intents, PropertyGrid terms, and result metrics, but typing or clearing a query must not open tools, run Preview/Run, create layers, change the visible workspace layer, or change input/output routing. Compact icon mode hides the search row and keeps every tool icon clickable.
- A found Tool rail item may expose explicit Learn and public-sample shortcuts only when canonical existing destinations are available. Learn must reuse the Learn window and select the mapped topic. Samples must open the existing Sample Picker at the mapped Learn path; opening or cancelling the Picker must not load a sample, select/open the Tool View, run Preview/Run, create layers, or change workspace/input/output routing. Loading remains a separate explicit Picker confirmation.
- A found Tool rail item may expose Guided Setup only for the five existing starter-intent contracts: Line -> pin gap/pitch, Blob -> count, Contour -> shape/count, Matching -> target presence, and Mean -> brightness. The shortcut must only open Recipe Manager, select the existing Guided Setup tab, and select the mapped intent. It must not create Starter XML, open the Tool View, run Preview/Run, create layers, or change workspace/input/output routing. Unsupported tools must not show a Guided Setup shortcut.
- Do not label a generic Tool View as `ROI needed` merely because its property model exposes `USE_ROI`. Blob/Contour support full-image execution, Line supplies its full-image default on explicit Preview, and Matching-family tools have a template prerequisite. A fixture-consuming pipeline Step remains the proven required-ROI case and must fail closed through pipeline validation.
- Recipe Manager review-bundle export is an explicit command separate from XML export. Schema v1 contains only `pipeline.xml` and `review-manifest.json`; the manifest records application version, validation, ToolTypes, Step routes, acceptance metrics, and referenced dependency/sample path status, size, and SHA-256. It must not copy referenced/private files, import a recipe, run Preview/Run, create layers, or change workspace/input/output routing. Any later asset-copy or import workflow must remain a separate explicit operator action with a review step.
- Keep comments around non-obvious routing, viewer gesture, and preview/result separation logic. These are easy places for regressions.
- Public repository material must preserve `LICENSE`, `NOTICE`, copyright text, and attribution to `최노아(Noah-Choi)`. Do not remove or obscure these notices in README, package metadata, or redistributed source copies.
- Public sample assets must be project-authored synthetic assets or otherwise clearly licensed for redistribution. The root `Sample/` folder is local/vendor sample reference material and must not be tracked or reintroduced into public GitHub output. Public sample and tutorial flows should use `docs/samples/public/` and `docs/samples/public/product/`.
- Public README/tutorial/Learn content must not mention private goals such as portfolio, hiring, submission, or internal-only intent. Keep those notes in recovery/handoff documents only.

### LLM Maintenance-Mode Boundary

- Planned LLM feature expansion is frozen by P196. Existing LLM Assistant, Guided Setup, XML prompt/guide/catalog, validation, correction display, and explicit import behavior remain supported compatibility surfaces.
- Do not add a provider, consumer-web automation, API credential dependency, prompt family, intent skill, or transcript campaign without an explicit user decision to reopen the track.
- Maintenance changes require a concrete regression, unsafe XML acceptance/import behavior, data-loss risk, or compatibility break. Cosmetic or speculative LLM improvements are not current priorities.
- LLM draft creation, paste, validation, diff/dependency review, and import must never run Preview/Run, create or select layers, change routing, or silently accept a recipe.
- The workbench must remain fully operable without an LLM account, provider session, API key, transcript, or generated XML.
- Historical incomplete LLM gates, including the missing natural Pin Phase 3 failure and frozen P169 Test replay, are deferred evidence rather than active blockers. Preserve them unchanged; do not manufacture a failure or execute reserved evidence early.
- Reopening requires an explicit user decision after the equivalent non-LLM workflow and deterministic N-sample evidence exist.

## Stable Contracts

### 1. PropertyGrid-Based Algorithm Tools

Stable behavior:
- Blob, Contour, Line, Matching, EdgeBasedMatching, and FeatureMatching keep the model-to-PropertyGrid structure.
- A tool property model is assigned to the PropertyGrid selected object, and the editor UI is generated from properties and attributes.
- Visibility rules, range editors, enum combos, and image/template editors must remain compatible with this model-driven path.
- Enum ComboBox editors must open, show real items, and render the selected text only once.
- Min/Max range editors must reserve enough width for long numeric values and must not expose the old `Invert` checkbox. Threshold inversion belongs to the threshold editor, not area/angle Min/Max range editors.
- Min/Max range editors represent one operator concept. The companion max property, such as `MAX_AREA`, `FIND_ANGLE_MAX`, `CANNY_HIGH`, or `MEAN_MAX`, remains on the property model for XML/execution but must not be shown again as a separate duplicate PropertyGrid row.
- RangeEditor companion max descriptors must remain available to the original WPG descriptor/PropertyItem path. Do not remove `FIND_ANGLE_MAX`, `MAX_AREA`, `CANNY_HIGH`, or similar companion descriptors from TypeDescriptor just to hide duplicate UI rows; hide only the duplicate visual row while keeping the descriptor and model property alive.
- RangeEditor numeric TextBoxes must allow transient operator edits such as empty text, `-`, or other partial numeric input while typing. TextBox endpoints commit on Enter or focus loss, not on every TextChanged event. Slider endpoints may continue to update immediately.
- PropertyGrid tool commands must commit the currently edited TextBox/ComboBox/Slider values before creating the execution property. If an operator types a value and immediately clicks Preview/Run/Add Pipeline without moving focus first, the command must use the visible value, not the previous model value.
- Conditional child rows must be visually distinguishable from their parent switch/selector row. The shared `WpfPropertyGridBridge` owns this row styling; do not copy the styling into individual tool views.
- Boolean visibility toggles such as `USE_THRESHOLD`, `USE_ADAPTIVE_THRESHOLD`, and draw/result toggles reveal or hide child rows only; they must not run preview by themselves.
- The masking editor row is hidden by default and appears only when `USE_MASKING` is enabled or a loaded legacy recipe already has saved masks.
- `USE_MASKING` is a src/OpenVisionLab/UI/editor visibility switch; toggling it must not run preview by itself.
- Contour `USE_APPROXPOLYDP`, Matching `USE_FIND_ANGLE` / `USE_CANNY`, EdgeBasedMatching `USE_FIND_ANGLE`, and Line `USE_MANUAL_ANGLE` / `USE_EXTEND_FIT_LINE` / `USE_AVERAGE_FILTER` are also visibility switches for their child rows; toggling them must not run preview by itself.
- Matching defaults to manual preview. `AUTO_PREVIEW=false` means template registration, angle range/step, score, count, magnification, and matching mode edits update teaching state only; they must not run preview until the operator explicitly clicks Preview. When `AUTO_PREVIEW=true`, expensive parameter edits may auto-preview through the shared debounce policy, while visibility switches such as `USE_FIND_ANGLE` and `USE_CANNY` still must not run preview by themselves.
- Tool presets are explicit teaching commands, not execution commands. Applying a preset must update the selected PropertyGrid-backed model, persist/refresh the generated PropertyGrid rows, update summaries/overlays, and clear stale result review; it must not run Preview/Run, create output layers, change input/output routing, or bypass the property model. Matching-family tools expose the full preset strip in floating mode and a compact `Parameters` header menu in docked mode so the stable PropertyGrid editor viewport is not reduced. Blob and Contour expose the same beginner Basic/Fast/Precise preset surface through the shared PropertyGrid runtime. Line exposes Basic/Fast/Precise presets through its special PropertyGrid runtime; Line presets apply only to the currently selected Line A/B property model and must preserve purpose, ROI, projection direction, polarity, and layer routes. This is validated by `wpf_shell_host_matching_presets`, `wpf_shell_host_area_tool_presets`, and `wpf_shell_host_line_presets`.
- Matching and EdgeBasedMatching `USE_COARSE_TO_FINE_ANGLE_SEARCH` are explicit performance options for wide angle ranges. They default to false and must not change existing exhaustive angle-search behavior until the operator enables them.
- Matching and EdgeBasedMatching `COARSE_ANGLE_STEP` and `COARSE_ANGLE_TOP_K` are child options of `USE_COARSE_TO_FINE_ANGLE_SEARCH`. They are visible only when angle search and coarse-to-fine search are both enabled, and changing their visibility must not run preview by itself.
- Contour detection defaults to external contour retrieval, and Contour result drawing defaults to the actual `ContourResult.Contours` outline. Bounding rectangle drawing is an explicit `DrawMode=BoundingBox` option, not the default contour visualization.
- Contour `USE_DRAW_IMAGE` is a legacy compatibility field and must stay hidden in the WPF PropertyGrid. Operators configure contour visualization through `DrawMode`, `DrawColor`, and `DrawThickness`.
- Contour display options are stable operator-facing controls. `DrawMode` is shown as `컨투어 표시`, followed immediately by `DrawColor` (`표시 색상`) and `DrawThickness` (`선 두께`). The default contour display color is `Aquamarine`.
- Contour display-only options (`DrawMode`, `DrawColor`, `DrawThickness`) and visibility switches (`USE_APPROXPOLYDP`) must not schedule auto-preview by themselves. Preview is scheduled by actual teaching values such as threshold/range slider changes, or by explicit Run/Preview.
- Contour Tool View may show a compact verification guide above the PropertyGrid. The guide is display-only and summarizes Preview state, area/threshold/ROI criteria, and next action; it must not replace the PropertyGrid or trigger Preview/Run/Add Pipeline.
- Line tool scan-helper terminology is stable. The WPF PropertyGrid must show the operator-facing category/display text as `Scan Line`, `Scan direction`, `Scan interval`, `Use scan angle`, `Scan angle`, and `Show scan line`.
- Keep Line recipe/model/pipeline compatibility names unchanged: `VER_PRJ_DIR`, `POINT_RANGE`, `USE_MANUAL_ANGLE`, `MANUAL_ANGLE_VALUE`, `SHOW_VERTICAL_LINE`, `VerticalLineCalculator`, and existing XML/pipeline parameter names. Do not rename these internals to `Scan*` without an explicit migration plan.
- `Distance` means scan-line based intersection plus distance measurement. Do not split it into a separate `Scan Intersection` UI mode unless a future spec explicitly requires it. `Intersection` means fit-line to fit-line crossing.
- Image/template editor buttons must remain clickable after a main image is loaded.
- ROI and template editors must use the active WPF Shell `IDisplayManager` context, not an unrelated global/default display manager.
- ROI and template editors must open with grayscale/indexed source images and with an empty current ROI. Do not reintroduce direct bitmap PNG serialization for WPF image display.
- PropertyGrid tool parameters are operator teaching state. The last edited values must be preserved per recipe/tool and must be restored when the tool is reopened or the native tool document is recreated.
- Recipe switching must not let a cached native tool document or selected PropertyGrid object keep the previous recipe's values. Native PropertyGrid tools must be closed/recreated or rebound so Add Pipeline uses the active recipe's persisted model values.
- Threshold custom WPF mode is also teaching state. UI smoke or tests must not assume the tool opens in Basic mode after a previous Adaptive/Range run; tests should explicitly select the mode they are validating.
- ROI editor results (`CvROI`, `CvROIS`, `CvMASKS`) must be stored with the tool property model and must not be treated as temporary UI-only state.
- When an input image is loaded or refreshed, the tool input preview must display the currently configured ROI overlay so the operator can immediately see the last taught region.
- ROI overlay display is informational; showing the ROI must not create, select, rename, or write an output layer, and must not imply that detection/run has happened.
- Matching fixture translation is an explicit pipeline runtime option. It may clone a downstream step and translate the clone's effective `CvROI`, but it must not rewrite the saved `CvROI`, change input/output routing, create layers outside normal explicit Run output, or trigger Preview/Run. Translation-only v1 requires one Matching result, one prior named frame, the same source layer, one ROI, and an angle delta within the configured limit; unsupported rotation, multi-ROI, mask, or missing-frame cases must fail closed.
- Blob and Contour Tool Views may show a compact verification guide above the PropertyGrid. The guide is display-only and summarizes Preview state, area/threshold/ROI criteria, and next action; it must not replace the PropertyGrid or trigger Preview/Run/Add Pipeline.
- Blob/Contour result explanation may translate area-style metrics into beginner-facing reasons through `VisionToolAreaResultExplanation`, including count, max area, max box size, and likely threshold/ROI/area failure-cause hints. This is presentation state only; it must not change Blob/Contour detection metrics, pass/fail logic, Preview/Run execution, layer routing, output layer creation, or pipeline step parameters.
- Line Tool View may show compact verification guidance in the shared summary/result area. The guide is display-only, summarizes Edge/Measure/Intersection Preview state and next action, and must not replace the Line PropertyGrid, Line A/B controls, ROI edit affordance, or trigger Preview/Run/Add Pipeline.
- Line result explanation may translate Edge/Measure/Intersection metrics into beginner-facing reasons through `LineToolResultExplanation`, including edge-point count, fitted-line length, distance px/mm, cross/no-cross state, and likely ROI/contrast/polarity/scan-setting failure-cause hints. This is presentation state only; it must not change Line gauge metrics, distance/intersection semantics, Preview/Run execution, layer routing, output layer creation, or pipeline step parameters.
- Tool View verification and result-review wording is presentation state. Shared helpers such as `VisionToolVerificationText`, `VisionToolAreaResultExplanation`, `VisionToolAreaVerificationGuidePresenter`, `VisionToolMatchingVerificationGuidePresenter`, and `LineToolVerificationGuidePresenter` may format beginner-facing text, but they must not execute Preview/Run, create layers, change routing, or replace the PropertyGrid model.
- Matching-family template status, criteria summaries, FeatureMatching Ratio/RANSAC guide text, and Line purpose/setting labels are display-only learning aids. They may be localized or reformatted for readability, but they must not change the selected property object, route input/output layers, Preview/Run execution, Add Pipeline metadata, or internal compatibility identifiers such as `Line A`, `Line B`, `Edge`, `Measure`, and `Intersection`.
- Result-review summary/chip labels may be localized for beginner readability, but the underlying result metrics, pipeline metadata, property model values, and Preview/Add Pipeline semantics must remain unchanged.
- SimplePreprocess Mean/HSV/Histogram may show beginner-facing result explanations in the shared result-review area. Mean can explain average/range/count, HSV can explain selected mask pixel ratio, and Histogram can explain input/output mean/contrast changes. This is display-only preview interpretation; it must not change preprocessing output pixels, Preview/Run scheduling, Add Pipeline availability, layer routing, output layer creation, or any downstream pass/fail metric semantics.
- The shared Tool Signal Inspector is retained current-Preview evidence and does not execute a tool by itself. Histogram publishes read-only source/result 256-bin grayscale population series with tool/input/region/parameter identity and source/result SHA-256. Threshold Basic may publish one editable `T` marker and Threshold Range may publish editable `Lower`/`Upper` markers; marker release edits only the existing Threshold teaching model, clears stale evidence synchronously, and schedules the existing debounced Preview. Threshold Adaptive must not present a misleading single global cutoff marker or chart. Until Threshold gains an explicit ROI contract, its signal region is exactly `Full image`. A successful Line Edge or Measure Preview may publish one deterministic representative scan with prepared intensity, signed scan-direction response, polarity/contrast/thickness, exact source-image scan/selected-point coordinates, and spatially distinct alternatives only when an independent replay matches the retained runtime first-stable edge. The result drawing must use those same coordinates. Parameter edits, active Tool input-image load, and replacement of the active `Main` workspace image must clear retained Line evidence/result state without running Preview. This Line diagnostic must not change `LineGauge`/`LineDistance` detection, fitting, measurement, XML, calibration, or acceptance semantics. A current `CircleGauge` Pipeline Run may retain its actual runtime radial scans with prepared intensity, signed response, selected edge, contrast acceptance, robust-fit inlier/outlier state, signed radius residual, and exact reject reason. Circle sample table, residual plot, selected radial profile, and compact result drawing must share one stable scan identity, and review selection must not request another Run or mutate layers/routes. Circle evidence must reuse the existing edge selection, initial/refined fit, robust rejection, support/radius/residual gates, and pixel coordinates; it must not introduce a second fitting implementation, new XML settings, calibration semantics, or automatic gate selection. Plot selection, cursor inspection, X zoom/pan, reset, overlay open/back, and TSV export must not run Preview/Run, create/select a layer, change the active layer, or mutate input/output routes. A later tool integration must use the shared evidence/plot/export contract and prove its own drawing/coordinate identity; it must not copy a one-off chart implementation.
- Blob and Contour Pipeline Review may publish a current-Run object-metric distribution only from the retained `VisionPipelineObjectResult` rows. The selectable metrics are the existing axis-aligned pixel `Area`, `BoundsWidth`, and `BoundsHeight`; each reads its existing `MIN/MAX_AREA`, `MIN/MAX_WIDTH`, or `MIN/MAX_HEIGHT` Pipeline/PropertyGrid range. Accepted/rejected bin series, range markers, source/result identity, table row, selected-object drawing, and plot selection must refer to the same current run and stable object number. Metric/range review and row/plot/image selection must not rerun segmentation, request Preview/Run, change a gate, create/select a layer, or mutate routes. The legacy `1000000` maximum sentinel remains unbounded compatibility behavior and must not be presented as a newly certified finite gate. This distribution must not add a descriptor, change Blob/Contour filtering, `ResultCount`, aggregate metrics, XML, report persistence, or acceptance semantics.
- PropertyGrid-heavy tools must open with enough initial space for comfortable parameter review. Line/Blob/Contour/Matching-style tool windows use the large hosted size, and the common parameter group keeps a minimum grid height.
- Saved floating tool-window bounds must not reopen PropertyGrid-heavy tools below their usable editor size. Preserving an operator's previous placement is allowed only after the large-tool minimum width/height needed for readable PropertyGrid editing is respected.
- Docked single-input PropertyGrid tools must keep a usable editor viewport for long parameter lists. Line and Contour style tools may use the PropertyGrid's internal scroll, but the visible editor area must not collapse and Add Pipeline / Run Preview must remain inside the docked inspector viewport.

Do not:
- Replace these tools with fully hand-coded parameter panels.
- Bypass `VisionToolPropertyGridHost` / PropertyGrid presenter/controller flow for routine property edits.
- Remove PropertyGrid rows because a current smoke does not inspect that row.
- Hide RangeEditor companion max rows by deleting descriptors, removing model properties, or setting companion rows non-browsable at the descriptor level. This breaks Max endpoint editing in the original WPG RangeEditor.
- Bind RangeEditor TextBox endpoints with `UpdateSourceTrigger=PropertyChanged` or reintroduce TextChanged-to-model commits. This prevents operators from clearing/retyping values and causes the editor to snap back while typing.
- Reset tool parameters to constructor defaults just because a View, ViewModel, document cache, or floating tool window was recreated.
- Remove ROI overlays from input previews while changing viewer, PropertyGrid, or preview presenter code.
- Persist PropertyGrid values by reading visual controls ad hoc; the property model is the source of truth.
- Run a tool or add a pipeline step from stale PropertyGrid values while a TextBox still has focus. Commit pending editor bindings through the shared PropertyGrid host before reading the property model.
- Remove the Blob, Contour, or Line compact verification guide/result guidance without replacing it with an equivalent display-only teaching aid.
- Replace Contour outline drawing with bounding rectangle drawing unless the operator explicitly selected the BoundingBox draw mode.
- Reintroduce `Vertical Line` as WPF PropertyGrid wording for the Line tool. The legacy localization key may remain for compatibility, but the operator-facing WPF terminology is `Scan Line`.
- Rename Line internal compatibility identifiers such as `SHOW_VERTICAL_LINE` or `VerticalLineCalculator` just to match the WPF wording.

Relevant smoke:
- `wpf_property_grid_matching_combo`
- `wpf_shell_host_blob_tool`
  - Covers Blob PropertyGrid rows, no auto-preview from threshold/adaptive/masking visibility toggles, compact verification guide, result guidance, and docked inspector preservation through the alias target `wpf_shell_host_blob_tool_docked_verification`.
- `wpf_shell_host_blob_tool_docked_verification`
- `wpf_shell_host_contour_tool`
  - Covers Contour PropertyGrid rows, no auto-preview from visibility/display toggles, compact verification guide, result guidance, and docked inspector preservation through the alias target `wpf_shell_host_contour_tool_docked_verification`.
- `wpf_shell_host_contour_tool_docked_verification`
- `wpf_shell_host_line_measure_tool`
- `wpf_shell_host_line_tool_docked_verification`
  - Covers Line PropertyGrid rows, no auto-preview from Line visibility switches, compact summary-strip verification guidance, result guidance, and docked inspector preservation.
- `wpf_shell_host_line_pins_measure_tool`
- Direct EXE: `bin\x64\Debug\OpenVisionLab.exe --smoke line-pins-measure --output .\.codex\smoke-output\actual-exe-line-pins-measure`
- `wpf_shell_host_line_intersection_tool`
- `wpf_line_signal_profile`
  - Covers four projection directions, current public Good/Bad same-parameter replay, intensity/signed response, first-stable runtime correspondence, exact source-image coordinates, distinct alternative, result drawing, provenance TSV, stale input clear, and no review-control Preview/layer/route side effects.
- `cvr04_circle_residual_review`
  - Covers unchanged frozen CircleGauge Good/Bad execution, all radial sample states and exact reject reasons, residual and selected intensity/signed-response series, source/result identity, row/plot/drawing two-way selection, and zero Run Review/layer/route side effects.
- `cvr05_object_metric_distribution`
  - Covers Blob Area/Width/Height and Contour Area distributions, exact existing range markers, accepted/rejected series, shared TSV provenance, row/plot/drawing selection, legacy unbounded maximum behavior, and zero Run Review requests.
- `wpf_shell_host_workspace_sample_pipeline_review_metrics`
  - Covers the actual public Blob Good current-Run Area distribution.
- `wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics`
  - Covers the actual public Blob Bad object rows, Area distribution, table/image selection, report persistence, and no Preview/layer/route side effects.
- `wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics`
  - Covers the actual public Contour Bad object rows, Area distribution, table/image selection, report persistence, and no Preview/layer/route side effects.
- `wpf_preprocess_output_preview_flow`
  - Covers SimplePreprocess output routing, parameter-triggered preview behavior, Mean/HSV/Histogram result-review explanations, Histogram signal-evidence replacement, plot navigation, provenance-preserving TSV export, and no reset/export layer/route/run side effects.
- `wpf_simple_preprocess_result_review`
  - Covers the current-build Histogram Signal Inspector presentation, source/result series and SHA-256 provenance, stale-evidence blocking, and read-only plot/export behavior.
- `wpf_shell_host_threshold_basic_tool`
  - Covers Basic `T` marker presentation, release-to-existing-model synchronization, stale-evidence clear/replacement through the existing debounced Preview, full-image provenance, overlay navigation, TSV export, and unchanged layers/active layer/routes.
- `wpf_shell_host_threshold_tool`
  - Covers Range `Lower`/`Upper` marker synchronization, `Lower <= Upper`, Adaptive no-global-chart behavior, and preservation of the docked Threshold parameter controls.
- `wpf_threshold_signal_good_bad_replay`
  - Covers the unchanged public `T=130` Binary Pipeline identity, current Good `ResultCount=4`, expected-NG Bad `ResultCount=1`, full-image signal provenance, and separate 256-bin TSV evidence for both source images.
- `wpf_shell_host_matching_tool`
  - Covers Matching `FIND_ANGLE_MAX` RangeEditor endpoint adjustment and transient clear/retype behavior for angle TextBoxes.
- `wpf_shell_host_matching_presets`
  - Covers floating Matching-family Basic/Fast/Precise preset UI, docked `Parameters` header preset menu, exact PropertyGrid-backed model updates, generated-row visibility refresh, and no Preview/Run execution even when `AUTO_PREVIEW=true` was enabled before applying a preset.
- `wpf_shell_host_area_tool_presets`
  - Covers Blob and Contour Basic/Fast/Precise preset UI, docked preset menu behavior for Blob, exact PropertyGrid-backed model updates, generated-row visibility refresh, and no Preview/Run execution.
- `wpf_shell_host_line_presets`
  - Covers Line Basic/Fast/Precise preset UI, selected Line A/B model isolation, generated-row visibility refresh, docked preset menu behavior, and no Preview/Run execution.
- `wpf_shell_host_edge_based_matching_tool`
  - Covers EdgeBasedMatching template-ready state, compact edge verification guide text, Canny/search/point criteria, Preview OK/NG result guidance, and Add Pipeline validation.
- `wpf_shell_host_feature_matching_tool`
- `wpf_roi_editor`

#### Edge Based Matching Addendum

Stable behavior:
- EdgeBasedMatching remains a PropertyGrid-based algorithm tool.
- EdgeBasedMatching PropertyGrid order is operator-teaching order: `Parameter`, `Matching`, `Edge Model`, `Angle`, `Search`, `ROI`, `Threshold`, then legacy image-process categories. Do not move base `Threshold`/`ROI` switches ahead of template registration and match criteria.
- EdgeBasedMatching `Matching` rows start with `Pattern path`, followed by `Min score`, `Match count`, and `Draw result`.
- EdgeBasedMatching edge extraction/model rows live under `Edge Model`, not a top-level `Canny` workflow category. `Max template points` and `Min gradient magnitude` belong with this model-building group.
- EdgeBasedMatching conditional rows must stay directly below their parent option: angle range/step/coarse rows under `Use angle search`, coarse step/top K under `Coarse angle search`, pyramid top N/min score under `Pyramid proposal`, and hybrid top N/image weight under `Hybrid verify`.
- EdgeBasedMatching option descriptions must stay operator-readable. `Greediness`, `Pyramid proposal`, and `Hybrid verify` descriptions must explain when to use the option, the speed benefit, and the miss/false-match risk instead of only naming the algorithm.
- EdgeBasedMatching summary text must expose active cost/risk controls: angle search state, edge threshold range, search step/refine, greediness, pyramid proposal settings when enabled, and hybrid verify settings when enabled.
- EdgeBasedMatching may show a compact verification guide above the PropertyGrid. The guide is display-only and summarizes edge matching state, Canny/score/search/point criteria, Preview OK/NG, and next action; it must not replace the PropertyGrid or trigger Preview/Run/Add Pipeline.
- `USE_FIND_ANGLE` defaults to false so existing recipes keep the previous translation-only edge matching behavior.
- When `USE_FIND_ANGLE=true`, the algorithm rotates template edge points and gradient vectors for each candidate angle, writes the detected angle into `MatchingResult.Angle`, and draws the result as a rotated box when the result angle is non-zero.
- EdgeBasedMatching angle sign follows OpenCV/image-matching angle semantics. A target rotated by `+12` degrees should report a positive angle near `+12`, not the opposite sign.
- `FIND_ANGLE_MIN`/`FIND_ANGLE_MAX` are edited through one RangeEditor row. `FIND_ANGLE_MAX` remains a model/descriptor companion for XML/execution and endpoint editing, but the duplicate visual row must not be shown as a separate operator row.
- `USE_FIND_ANGLE` is a visibility/teaching switch for the angle child rows and must not run preview by itself.
- `USE_COARSE_TO_FINE_ANGLE_SEARCH` defaults to false and is an explicit EdgeBasedMatching speed option for wide angle ranges. It first scans the full range with `COARSE_ANGLE_STEP`, then refines only around the best `COARSE_ANGLE_TOP_K` angles with the fine `FIND_ANGLE` step.
- EdgeBasedMatching coarse-to-fine search must keep the same score/angle/rotated-box result semantics as exhaustive angle search. It is an operator-selected optimization, not a behavior migration for existing recipes.
- EdgeBasedMatching summaries must show when coarse-to-fine is enabled and include the coarse step/top K/estimated search count so the operator can predict runtime.
- EdgeBasedMatching scoring uses cached gradient arrays internally. Do not reintroduce repeated `Mat.At<T>()` reads inside the candidate scoring hot loop unless a benchmark proves it is faster and result-equivalent.
- EdgeBasedMatching may cache the prepared template, edge template model, and rotated edge models per tool instance. This cache is an internal performance optimization only and must not change score, angle, center, result count, or rotated-box semantics.
- EdgeBasedMatching template/model cache must be invalidated when the template image, pattern path, template preprocessing settings, Canny/contour settings, max template point count, or minimum gradient magnitude changes. Rotated search-model cache keys must include angle-search range and step.
- EdgeBasedMatching score calculation is a gradient unit-vector dot product. The implementation may precompute source/template unit gradient arrays and avoid per-point magnitude division, but the public `Score` semantics must remain the same cosine-style edge-direction similarity.
- EdgeBasedMatching may expose `Candidate.Ambiguous*` diagnostics for repeated-pattern review. These metrics are diagnostic only and must not change public score, selected result, recipe compatibility, or pass/fail behavior unless a future spec explicitly uses them.
- EdgeBasedMatching `USE_POSITION_REFINE` defaults to false. When enabled with `SEARCH_STEP > 1`, the tool first performs the normal coarse center scan and then rechecks only the best coarse center neighborhoods at 1px spacing. This option is for balancing wide position search speed with center accuracy and must be operator-controlled.
- Do not present `SEARCH_STEP=4` plus `USE_POSITION_REFINE=true` as universally safe. A large coarse position step can still miss the correct coarse candidate on simple/repetitive edge geometry; validate with representative samples and prefer ROI or `SEARCH_STEP=2` when false positives appear.
- EdgeBasedMatching angle/position search may run in parallel internally. Parallelization must preserve score/angle/center semantics; if tie-break behavior is touched, keep deterministic top-left preference for equal scores.
- EdgeBasedMatching is more robust than image matching when edge geometry is stable but fill/illumination changes. It is weaker when similar edge-only distractors exist nearby, so ROI or a second-stage verifier is required for commercial-grade reliability.

Do not:
- Make EdgeBasedMatching angle search default-on without explicit recipe migration approval.
- Enable EdgeBasedMatching coarse-to-fine angle search silently for existing recipes.
- Enable EdgeBasedMatching position refine silently for existing recipes.
- Revert EdgeBasedMatching results to always report `Angle=0` when angle search is enabled.
- Draw only axis-aligned result rectangles for non-zero EdgeBasedMatching angles.
- Remove ambiguity diagnostics while changing candidate reduction. They are the guardrail for repeated-pattern scenes.

#### Matching Tool Addendum

Stable behavior:
- Matching opens as a native WPF floating tool window and keeps the PropertyGrid-driven parameter editor. Do not replace the Matching property grid with a hand-coded parameter panel.
- The template status row remains visible and must show whether a template is selected/ready. A valid template registration updates `PATTERN_PATH` and the status text without hiding the editor.
- The template editor/registration window must open with the active input image visible, not a blank canvas. It must keep ROI/template selection handles interactive for position and size adjustment.
- The template editor ROI supports rotation teaching. Clicking/dragging the rotation handle changes the ROI angle, and the pattern preview shows the exact zero-degree template that will be saved.
- Saving a rotated template ROI must affine-extract the rotated ROI from the original source image into an upright 0-degree pattern image. Do not implement this as "crop first, then rotate the cropped bitmap", because that changes the taught ROI semantics and can clip the intended pattern.
- Template ROI metadata stores the original logical ROI and `RotationDegrees` so reopening the editor restores the taught area and angle.
- The pattern/template editor button must remain clickable after the main/input image is loaded.
- Matching publishes and displays `Matching_Preview` only through explicit preview/run or through the opt-in auto-preview policy. Parameter editing alone must not silently imply that a new inspection result exists.
- Matching result review uses the `Template Match` label and shows at least Count, Score, Center, Box, Angle when angle search is used, and Tact in milliseconds.
- Matching result review also exposes operator-facing verification guidance: Preview OK/NG, configured criteria such as score/count, and the next action. This guidance is display-only and must not change matching execution, preview scheduling, input/output layer routing, or pipeline creation.
- Matching result explanation may translate score/count/angle/scale metrics into beginner-facing reasons and likely failure-cause hints through `VisionToolMatchingResultExplanation`, but it is presentation state only. It must not change matching score semantics, pass/fail logic, Preview/Run execution, input/output layer routing, output layer creation, or pipeline step parameters.
- Matching-family Tool Views may show a compact verification guide above the PropertyGrid. The guide is display-only and summarizes the current verification flow, pass criteria, and next action; it must not replace the PropertyGrid or trigger preview/run.
- `Tact` is an operator-facing execution-time indicator. Do not remove it from Matching result review or rename it to an unrelated label without explicit user approval.
- Match result boxes/ROI overlays must respect detected rotation when angle search is used. The overlay geometry rotates with the match result. Text labels may remain screen-upright for readability unless a future UX decision explicitly changes that.
- `MAGNIFIATION` is the existing model property name. The operator-facing description explains the image-pyramid scale-search concept. Do not rename the internal property casually because recipe/XML compatibility depends on it.
- `AUTO_PREVIEW` defaults to false. With `AUTO_PREVIEW=false`, template registration, angle range/step, score, count, magnification, and matching mode edits update teaching state only and do not run preview. With `AUTO_PREVIEW=true`, expensive parameter edits may auto-preview through the shared debounce policy; visibility switches such as `USE_FIND_ANGLE` and `USE_CANNY` still do not run preview by themselves.
- `USE_COARSE_TO_FINE_ANGLE_SEARCH` defaults to false and is an operator-controlled speed option for wide angle ranges. Do not make it implicit or default-on without explicit user approval and matching sample verification.
- `COARSE_ANGLE_STEP` and `COARSE_ANGLE_TOP_K` stay under the angle-search/coarse-search child row visibility rules. They must not be promoted to always-visible top-level controls.
- Matching coarse option descriptions must remain operator-readable: explain that the tool first scans the full angle range with a larger step, then rechecks only the best candidate angles with the fine `Angle step`.
- Matching summary must show coarse configuration and estimated search cost when coarse search is enabled, for example `Coarse step 5 x3 / Candidates ~342/1901`.
- Rotated-template cache is an internal performance optimization only. It must be bounded, must not change Matching result score/angle/box semantics, and must stay disabled for very small prepared templates where cache overhead is higher than rotation cost.
- `USE_PYRAMID_POSITION_PROPOSAL` is an explicit opt-in speed option for Image Matching scale search. It proposes locations on a smaller image, verifies the candidates at the original working resolution, and falls back to full search when proposals are weak. It defaults to false and is currently validated for angle-search-off scale search only.
- `PYRAMID_POSITION_TOP_N` and `PYRAMID_POSITION_MIN_SCORE` are child rows under `Pyramid proposal`. They must be hidden when the parent option is off, must not run preview merely by appearing/disappearing, and must stay operator-readable.
- Matching angle range is edited through one RangeEditor row. `FIND_ANGLE_MAX` remains a model/descriptor companion for XML/execution and WPG endpoint editing, but the duplicate visual row must not be shown as a separate operator row.
- Matching angle RangeEditor TextBoxes must allow transient edits, including clearing the value or typing `-`, until Enter/focus loss commits a valid numeric value.
- Matching ordinary numeric TextBoxes such as `SCORE_MIN`, `NUM_MATCH`, and `MAGNIFIATION` must be committed before explicit Preview/Run/Add Pipeline. Typing a value and immediately clicking the command button must not execute with the previous value.
- 2026-06-26 user verification: Matching PropertyGrid UX is verified by the operator. Do not reopen this area for broad refactoring unless there is a new concrete regression or an explicit redesign request.

Do not:
- Remove the Matching template status row, result review chips, or Tact chip because another smoke happens not to inspect them.
- Make Matching auto-preview the default again.
- Reintroduce TextChanged-to-model commits for Matching angle TextBoxes.
- Hide `FIND_ANGLE_MAX` by deleting it from the descriptor/model path. That breaks Max endpoint editing.
- Convert rotated match boxes back to unrotated rectangles when angle search is enabled.
- Enable coarse-to-fine angle search silently for existing recipes. It is a performance option, not a behavior migration.
- Remove template ROI rotation handles or replace rotated ROI extraction with post-crop bitmap rotation.
- Enable Image Matching `Pyramid proposal` silently for existing recipes or use it while angle search is enabled without a separate validation pass.
- Change Matching public score, center, box, angle, or scale semantics for the pyramid proposal path. The proposal path may reduce search work only; final results must remain verified at the original working resolution.

Relevant smoke:
- `wpf_shell_host_matching_tool`
  - Covers docked Matching result guidance through the alias target `wpf_shell_host_matching_tool_docked_verification`.
  - Covers compact Matching verification guide text: verification flow, Preview OK/NG, pass criteria, and next action.
- `wpf_template_editor_opengl`
- `artifacts\ui_precheck_matching_template_image_fixed_20260626_final`
- `artifacts\ui_precheck_matching_rotated_overlay_20260626`
- `artifacts\ui_precheck_matching_tact_label_ui_20260626`
- `artifacts\ui_precheck_matching_auto_preview_option_20260626`
- `artifacts\ui_precheck_matching_angle_max_range_20260626_visual_hidden`
- `artifacts\ui_precheck_matching_range_text_transient_20260626`
- `artifacts\ui_precheck_matching_text_commit_20260626_rerun`
- `artifacts\ui_precheck_matching_coarse_angle_20260626_diag`
- `artifacts\ui_precheck_matching_rotated_template_cache_20260626_final`
- `artifacts\matching_scale_pyramid_comparison_20260628`
- `artifacts\actual_exe_matching_pyramid_scale_20260628`
- `artifacts\ui_precheck_matching_pyramid_property_grid_20260628`

#### Affine Transform v1 Addendum

Stable behavior:

- `AffineTransform` is a PropertyGrid-based three-point pixel transform. Canonical
  XML uses `AffineTransform`; `Affine` and `AffineMatrix` remain accepted aliases.
- The source/destination point order is authoritative. Both point triangles must be
  non-collinear even when the configured minimum-area gate is zero.
- The separately built Library-Noah `Lib.OpenCV.dll` owns matrix validation,
  `WarpAffine`, matrix/decomposition/triangle/valid-pixel metrics, stable errors,
  and destination-point/triangle/transformed-source-frame drawings. Do not copy a
  second Affine calculation into OpenVisionLab.
- The Affine Tool View exposes source/destination points, output size, interpolation,
  border policy, and validation gates. Unrelated inherited Threshold, ROI, masking,
  and pixel/mm rows remain hidden.
- Opening the view, editing PropertyGrid values, selecting layers, or adding a Step
  must not execute Preview/Run. One explicit Preview may write the chosen output
  layer and must preserve the input route.
- Successful result review shows the authoritative 2 x 3 matrix, valid-pixel ratio,
  determinant, and source/destination triangle areas. The current-run output draws
  the three destination points, destination triangle, and transformed input frame.
- The public known-matrix sample and focused contract are regression evidence, not
  automatic correspondence, homography, camera/lens calibration, calibrated-unit,
  industrial-accuracy, unseen-robustness, or field-qualification evidence.
- The frozen DLL identity is assembly `2.1.0.0`, file `2.8.0.0`, SHA-256
  `B128CA282C0CD02C36F5CCF0C78C69C6F4834C3376158E8667EEAA7DE494A08B`.
- Missing `USE_DETECTED_SOURCE_POINTS` keeps the fixed numeric P218 behavior.
  When the flag is true, `SOURCE_POINT_1_FEATURE` through
  `SOURCE_POINT_3_FEATURE` must name three distinct earlier accepted typed
  `Point` results in the same input coordinate layer and image size.
- `Matching`/`TemplateMatching` and the three accepted EdgeBasedMatching aliases
  declare `Center` and publish it only for one usable result. The Affine source
  picker may also use declared Line, CircleGauge, and GeometryMeasure Point
  outputs.
- Detected-point Affine never silently falls back to the saved fixed source
  coordinates. Missing, duplicate, failed/NG, ambiguous, wrong-kind,
  cross-frame, non-finite, or out-of-image Point sources fail closed before the
  Library-Noah transform executes.
- The ordered source/destination correspondence remains operator-authored.
  OpenVisionLab does not reorder, infer, or automatically select points.
- Runtime review retains the three resolved source coordinates through
  `AffineDetectedSourcePointCount` and `AffineSourcePoint*X/Y`; downstream
  inspection continues to use a fixed ROI on the reference-coordinate output.

Relevant smoke:

- `--affine-transform-contract`
- `wpf_shell_host_affine_transform_tool`
- `wpf_openvision_learn_geometry`
- `wpf_shell_host_rotate_scale_tool`
- `artifacts\p218_affine_transform_v1_20260723`

### 2. Layer Selection And Routing

Stable behavior:
- Input layer combos must open and list valid image layers.
- Output layer creation prepares/selects an output layer without forcing the input layer to that output.
- Tool View output-layer selectors are result write targets. Selecting an existing output layer means the next explicit Preview/Run writes into that layer; it must not silently create a default tool output layer instead.
- Preview/run publishes to the selected output layer while preserving the operator's selected input route.
- Clicking an input preview activates the input layer.
- Clicking an output preview activates the output layer in the main workspace.

Do not:
- Treat the newest output layer as the next implicit input.
- Treat an existing selected output layer as read-only or ignore it in favor of a generated `{Tool}_Preview` layer.
- Hide `Main` or other valid input layers from input combos.
- Conflate workspace preview display with route input selection.

Relevant smoke:
- `wpf_layer_selection_all_native_tools`
- `wpf_layer_selection_existing_output_write`
  - Covers selecting an existing operator output layer, Preview writing into that layer, preserving `Main` as the input route, avoiding generated default output creation, and avoiding host active-layer side effects.
- `wpf_layer_selection_algorithm_existing_output_write`
  - Covers Blob, Contour, Matching, EdgeBasedMatching, and FeatureMatching writing explicit Preview results into selected existing operator output layers, preserving `Main` as input, avoiding generated default `{Tool}_Preview` layers, and avoiding host active-layer side effects.
- `wpf_layer_selection_preprocess_existing_output_write`
  - Covers Filter, Morphology, EdgeDetection, RotateScale, HSV, Mean, and Histogram writing explicit Preview results into selected existing operator output layers, preserving `Main` as input, avoiding generated default `{Tool}_Preview` layers, and avoiding host active-layer side effects.
- `wpf_layer_selection_threshold_tool`
- `wpf_layer_selection_arithmetic_tool`
- `wpf_shell_host_blob_tool`

### 3. Workspace Image Viewer

Stable behavior:
- Starting the Shell with no loaded image shows the localized `이미지 없음` / image-load prompt in the main workspace.
- The no-image main workspace prompt includes a beginner workflow for loading an image, selecting a tool, and checking Preview; it remains display-only except for explicit command buttons.
- The no-image main workspace prompt points operators to the bottom Run Log, and the empty Run Log shows a compact waiting card instead of a bare placeholder line.
- The no-image main workspace sample button must use a real command. When multiple runnable catalog samples exist, it opens a sample catalog picker that shows sample goal, tool flow, expected metrics, benchmark OK/NG reference state, Learn Mode guidance, recommended start, result interpretation, failure-cause summary, check guidance, NG fix guidance, image/pipeline paths, and Good/Bad pair context.
- The sample catalog picker may expose task-oriented Learn paths such as Matching, Blob, Contour, Line, Mean, and Good/Bad. Selecting a Learn path only filters the catalog list and selected sample; it must not open a sample, run Preview/Run, open tools, create output layers, change routing, or rewrite recipe values.
- Operator-facing sample catalogs and Recipe Manager sample selectors show only public-safe and product sample sources. Keep `LocalLegacy` loading available for old recipe/history compatibility, but do not expose Local Legacy as a new-user catalog source.
- The sample catalog window uses the shared OpenVisionLab custom title bar with minimize, maximize/restore, and close controls; do not return it to the default Windows title bar.
- Learn document actions render repository Markdown into a styled local HTML guide and open that HTML in the default browser. Do not shell-open `.md` files into an editor for the beginner workflow.
- Learn topic `Tool 열기` actions may select the related PropertyGrid Tool View and show the expected parameter location, but must not run Preview/Run, create output layers, or change input routing. The Brightness/Histogram topic maps to the existing Mean Tool View (`Mean Type`, `Min Mean`, `Max Mean`) and Histogram Tool View (`Type`, `Clip Limit`, `Tile Grid`, `Normalize Alpha/Beta`). The Filtering topic maps to the existing Filter Tool View (`Input/Output Layer`, `Filter Type`, `Border Type`, Kernel `Width/Height`, plus type-specific Median/Bilateral fields). The Morphology topic maps to the existing Morphology Tool View (`Input/Output Layer`, `Operation`, Kernel `Width/Height`, size presets, and `Shape`). The Blob topic maps to the existing Blob PropertyGrid (`Use ROI`, `ROI`, `Min area`, `Max area`) and points result review to `ResultCount`, `AreaMin/AreaMax`, and `BoundsWidth/BoundsHeight`. The Contour topic maps to the existing Contour PropertyGrid (`컨투어 표시`, `Retrieval mode`, `Min area`, `Max area`, optional approximation/drawing fields) and points result review to `ResultCount`, `AreaMax`, `BoundsWidthMax`, and `BoundsHeightMax`. The Edge/Line topic separates EdgeDetection edge-map creation (`Edge Type`, Canny/Sobel/Scharr/Laplacian fields) from Line ROI-based edge/fit-line work (Purpose, Line A/B, ROI, Polarity/Direction/Contrast/Thickness, scan fields) and may open either existing Tool View explicitly. The Arithmetic topic maps to the existing double-input Tool View (`Input A`, `Input B`, `Output Layer`, `Mode`, `Arithmetic Type`, `Input B Source`). The Geometry topic maps to the existing RotateScale Tool View (`Input/Output Layer`, `Angle`, `Scale X`, `Scale Y`); `OutputSize` remains an explicit Preview result rather than an input field. The Color/HSV topic maps to the existing HSV Tool View and its Hue/Saturation/Value, ROI, and OutputLayer controls.
- The LineDistance topic maps to the existing Line Tool View. It guides the operator to select `Purpose > Measure` explicitly, configure Line A/B, ROI, `Pixel / mm`, and edge/scan fields, then review `DistanceMmAvg` together with `DistanceMmRange`/`DistanceMmMax`. Opening the Tool View must not select Measure, mutate parameters, or run Preview/Run automatically.
- The Matching topic maps to the existing Matching Tool View. It points to Tool Shell `Template Ready`, PropertyGrid `Pattern path`, `Matching > Min score`, `Match count`, ROI, and optional angle/scale search, then requires explicit Preview or Run Review before interpreting overlay position, `ScoreMax`, and `ResultCount`. Opening the Tool View must not register a template, mutate parameters, or run Preview/Run automatically.
- The EdgeBasedMatching topic maps to the existing EdgeBasedMatching Tool View. It points to Tool Shell `Template Ready`, PropertyGrid `Pattern path`, `Matching > Min score / Match count`, `Edge Model > Canny range / Max template points`, `Search > Search step`, ROI, and optional angle/scale search, then requires explicit Preview or Run Review before interpreting overlay position, `ScoreMax`, and `ResultCount`. Opening the Tool View must not register a template, mutate parameters, or run Preview/Run automatically.
- The FeatureMatching topic maps to the existing FeatureMatching Tool View. It points to Tool Shell `Template Ready`, PropertyGrid `Feature template path`, `Matching > Ratio threshold`, `RANSAC tolerance`, and ROI. The serialized key remains `SCORE_MIN` for compatibility, but its FeatureMatching meaning is the Lowe descriptor ratio and smaller values are stricter. Explicit Preview or Run Review is required before interpreting overlay position, `ScoreMax`, and `ResultCount`. Opening the Tool View must not register a template, mutate parameters, or run Preview/Run automatically.
- The Color/HSV Learn animation may demonstrate actual `Cv2.Split`, `Cv2.Merge`, `Cv2.CvtColor`, and `Cv2.InRange` data flow. This remains display-only learning state and does not create a new pipeline ToolType, mutate image layers, or execute Preview/Run.
- The default Shell Host bottom status bar shows current recipe, workspace layer, tool/task state, and operation status. Do not reintroduce generic or hard-coded drive-capacity bars.
- Maximizing the custom Shell Host window must stay inside the current monitor work area so the Windows taskbar remains visible and the OpenVisionLab bottom status/log controls remain usable.
- Compacting the left Tool rail keeps an icon-only, tooltip-enabled, clickable tool list. Do not reduce compact mode to an empty expand handle.
- The Line Tool rail readiness reads the first recipe-owned Line A/B `PIXELPERMM` values. Equal zero values show pixel-only mode, equal positive values show the configured mm/px scale, and missing, invalid, negative, or inconsistent values require scale review.
- A displayed positive Line scale is configuration evidence, not proof that physical calibration was performed. The readiness description must require real calibration evidence before mm results are trusted, while remaining display-only and never opening Line, running Preview/Run, creating layers, or changing routing.
- Good/Bad pair sample picker UI may add a decision guide that explains which shared metrics separate OK and NG references, a compact validation checklist, and the recommended manual review order. This guide is display-only and must not run Preview/Run, open tools, create output layers, change routing, or rewrite recipe thresholds.
- Good/Bad sample catalog coverage must keep representative public-safe pair groups for Blob, Contour, LineDistance, Matching, EdgeBasedMatching, FeatureMatching, Mean, Threshold, and product-domain flows. Each pair group must include both Good and Bad references, one shared baseline pipeline, bounded expected metrics, and at least one shared Good/Bad metric.
- A Bad reference may be `ExpectedFailure` when the shared baseline pipeline intentionally rejects the sample through a stable metric acceptance gate. `Public_Mean_Brightness_Dark_Bad` is a controlled NG reference: the Mean tool still produces `MeanValueAvg`, but the public sample pipeline rejects values below the normal-brightness acceptance threshold.
- Feature score-discrimination Bad references are controlled NG references. `Public_Feature_Card.pipeline.xml` must gate acceptance on `ScoreMax` for the normal target range, so low-score/wrong-target Feature hypotheses can still produce a result image while Pipeline Review reports metric NG.
- LineDistance Bad references may be controlled NG references when the shared pipeline measures edge spacing. `Public_Line_Pins_Distance.pipeline.xml` gates acceptance on `DistanceMmAvg` in the normal range, so width/spacing drift can still produce line overlays while Pipeline Review reports metric NG.
- Pipeline `LineDistance` keeps raw edge-point intersections as the default. When both paired gauges carry the existing `USE_EXTEND_FIT_LINE=true`, distance samples are intersections against the two fitted edges; every reported endpoint must remain inside the source image and its configured gauge ROI. The runtime evidence must retain the measurement ROI, both fitted edges, and the final distance lines. `EXTEND_FIT_LINE_VALUE` continues to control the displayed fit-line extent; it is not a tolerance, calibration, or acceptance value.
- Pipeline `LineDistance` drawing evidence must represent both configured gauge ROIs. Equal Line A/B ROIs retain one compact `Measurement ROI` overlay; distinct ROIs retain separately labelled `Line A ROI` and `Line B ROI` overlays. This drawing rule must not change edge detection, paired-distance values, acceptance, output routing, or explicit Preview/Run behavior.
- Recipe Manager selected-Step PropertyGrid must preserve Line A/B identity. ROI/use-ROI, primary and vertical projection direction, polarity, and manual-angle fields are independently labelled and independently serialized. Applying an unchanged edit object must not collapse unrepresented per-line values to Line A; changing an explicitly shared compact field may continue to apply that field to both lines. Load/apply/save/reload must not trigger Preview/Run or change layer routing.
- General `LineDistance` orientation/polarity teaching must use the actual projection-direction contract, not ROI rotation alone. Horizontal opposing edges use `X_LTOR/X_RTOL`; the exact 90-degree clockwise equivalent uses `Y_TTOB/Y_BTOT`, transformed A/B ROIs, and the corresponding scan-angle frame. Bright/dark inversion changes polarity but must retain the same physical boundary and distance distribution. P200 is the frozen public synthetic reference for this contract; it does not establish calibration or general industrial robustness.
- `PinArrayGap` measurement semantics must remain explicit. Missing `MeasurementMode` and `MeasurementMode=EdgeGap` measure adjacent empty clearance and publish `DistancePx*`; `MeasurementMode=CenterPitch` measures adjacent detected dark-pin centers and publishes `PitchCount` plus `PitchPxMin/Max/Avg/Range`. CenterPitch must not publish mm pitch without a separately verified calibration contract, and the frozen LLM Pin Guided Setup v1 must remain EdgeGap-only until explicitly reopened. Recipe Manager PropertyGrid load/apply/save must preserve mode, row ROI, detection values, unrepresented parameters, and zero Preview/Run side effects. Exact drawings must show the reviewed row ROI, detected pins, center points, and `P#` center-to-center lines.
- The `Dark band thickness / Gap (LineDistance)` intent is a separate measurement-only contract. It requires exactly one operator-reviewed coarse ROI and one `LineDistance` Step with `USE_GAP_EDGE_PAIR=true` and `PIXELPERMM=0`; it must not silently add Matching, locator, normalization, template dependencies, acceptance, or calibration. The selected lower edge must be fitted from the nearest sustained bright transition after the dark core below a supported upper edge; a farther Hough line is not an eligible substitute. Explicit Run evidence must retain the coarse ROI, all candidate lines, selected upper/lower edges, five Gap samples, named PASS/REJECT state, and distance/stage/support/dark-coverage/ambiguity metrics.
- Blob density Bad references may be controlled NG references when the shared pipeline measures particle count. `Public_Blob_Particles.pipeline.xml` gates acceptance on `ResultCount` in the normal dense-particle range, so sparse-density samples can still produce Blob result images while Pipeline Review reports metric NG.
- Contour Bad references may be controlled NG references when the shared pipeline measures shape count. `Public_Contour_Shapes.pipeline.xml` gates acceptance on `ResultCount` in the normal shape-count range, so missing-shape samples can still produce contour result images while Pipeline Review reports metric NG.
- Threshold Bad references may be controlled NG references when the shared pipeline measures isolated pad count. `Public_Threshold_BandPads.pipeline.xml` gates acceptance on `ResultCount` in the normal pad-count range, so missing-pad samples can still produce threshold/contour result images while Pipeline Review reports metric NG.
- Not every Bad reference should become `ExpectedFailure`. Some Bad references are comparative references: they must remain runnable and metric-bounded so the operator can compare Good/Bad separation without treating the shared recipe execution itself as failed.
- Opening a selected sample loads it into `Main`, saves/activates its pipeline as a `Sample_` pipeline for the current recipe, and leaves Preview/Run/manual tool opening under explicit operator control.
- Opening Pipeline Review from a loaded sample must bind to the active `Sample_` pipeline for the current recipe. Running Review explicitly may execute the sample pipeline and show OK/NG, metrics, run log, and output preview, but opening the sample or opening Pipeline Review must not trigger native Preview automatically.
- Pipeline Review must show its owning recipe and provide an explicit `Return to Recipe` route. Returning must reopen the same Recipe Manager summary without rerunning Review/native Preview, creating or removing layers, changing the active layer, or changing recipe/pipeline routing.
- Recipe Manager summary must distinguish the workspace-global current work sample from recipe-bound sample-run evidence. Selecting or automatically defaulting a catalog sample may update the work-sample name, but the latest recipe result must remain `not checked` unless the in-memory result records the same recipe and selected pipeline; recipe or pipeline switches must not present another context's result as current evidence.
- Recipe Manager summary and advanced review are distinct workspace states. Summary owns recipe search/library, the selected-recipe overview, and create/duplicate/rename/delete lifecycle commands. Advanced review hides those outer controls, uses the detail workspace at full width, opens on Pipeline review, and exposes only technical tabs plus explicit XML/review transfer commands and `Back to summary`.
- Switching Recipe Manager summary/advanced state is navigation only. It must not run Preview/Run, create/delete/load layers, change the active layer, modify Step parameters, or change recipe/pipeline input/output routing.
- Smoke-created recipe workspaces must use a reserved scenario prefix plus an exact generated suffix and be deleted in `finally`. Cleanup may remove only names that match the reserved prefix and generated suffix contract; it must not delete arbitrary operator recipes.
- After a sample is opened, the main workspace may show a compact sample workflow strip with the active `Sample_` pipeline, first step, step count, and next-action guidance. Manual image load hides the strip.
- The Shell top bar uses the recipe selector as the single operator-facing recipe context. Do not add a separate read-only `Scope`/pipeline chip beside it; the internal recipe/pipeline context remains available to tool and recipe workflows.
- The Shell top bar exposes a recipe selector and a new-recipe command. Selecting or creating a recipe must use the same recipe reload path as `RecipeState.Name`, refresh active recipe/pipeline context, and must not auto-open a tool, run Preview/Run, create output layers, or change input routing by itself.
- The language selector must show readable operator text (`한국어`, `English`) in the dark Shell chrome. Changing language through the selector must persist the selected language and refresh Shell text only; it must not open tools, run Preview/Run, create layers, or change routes.
- Native Tool View `Add Pipeline` commands must append to the active recipe/pipeline context captured when the tool is opened or reactivated from cache. They must not fall back to a global default recipe/pipeline when the Shell is showing a different active context, and they must not run Preview/Run or create output layers by themselves.
- The top direct-status banner must track the workspace state: empty/start, image-ready, or sample-ready. This banner is display-only and must not run Preview/Run, open tools, create output layers, or change routing by itself.
- MainView empty/start, image-ready, sample-ready, and tool-selected guidance must be localized through the product language system. Switching between Korean and English refreshes text only; it must not open a tool, run Preview/Run, create output layers, or change route/input selection.
- The empty workspace beginner workflow should use operator-facing terms consistently, including Korean `미리보기` instead of mixed `Preview 확인` in Korean mode.
- The image-ready quick action buttons may open the selected WPF Tool View only through explicit operator command execution. Their labels are display text and must not imply Preview already ran.
- The sample workflow strip may expose explicit operator actions such as `Pipeline 보기` and `첫 단계 열기`. These actions must use the existing Shell tool-selection path and must not run Preview/Run, create output layers, or change routing by themselves.
- The empty startup workspace must not show the AvalonDock layer workspace, create docked layer documents, seed a `Main` image, or auto-open a tool window.
- Loading an image into `Main` displays it in the main workspace.
- Loading an image must not auto-open or retarget a tool window.
- Workspace zoom is cursor anchored.
- Workspace pan moves the image coordinate under a fixed viewport point.
- Pointer coordinate and pixel status update over the loaded image.

Do not:
- Remove zoom, pan, or pointer status while changing canvas/presenter code.
- Cover the OpenGL/image viewer with fallback images or overlays that block interaction.

Relevant smoke:
- `wpf_shell_host_workspace_empty`
  - Covers the localized empty prompt, beginner workflow, empty/start top status banner, Korean/English language refresh, and no auto tool open.
- `wpf_shell_host_learn_entry`
  - Covers Shell-backed Learn actions opening Foundation and Color/HSV related Tool Views without Preview/Run, layer creation, or routing side effects.
- `wpf_openvision_learn_color_hsv`
  - Covers actual one-pixel BGR Split/Merge values and channel order, the Color/HSV animation, BGR-to-HSV data mapping, HSV parameter-location guide, and Shell-only HSV Tool View action contract.
- `wpf_shell_host_workspace_sample_picker`
  - Covers runnable sample search/list/detail UI, Learn path entry grouping, selected sample image, tool flow, expected metrics, benchmark strip, Learn Mode guidance, Good/Bad or single-sample reference state, and explicit no-auto Preview/Run guidance.
- `wpf_shell_host_workspace_sample_learn_paths`
  - Covers task-oriented Learn path filtering for representative paths and verifies selecting a path only filters the list/selected sample.
- `wpf_shell_host_workspace_sample_pair_picker`
  - Covers the selected Good/Bad pair path, pair-comparison strip, pair decision guide with separating metrics, compact validation checklist, opposite reference summary, Learn Mode guidance, and explicit no-auto Preview/Run guidance.
- `wpf_shell_host_workspace_sample_pair_coverage`
  - Covers sample catalog Good/Bad pair metadata across representative pair groups, shared baseline pipelines, bounded expected metrics, shared Good/Bad metric names, and actual recipe-run metric checks for selected OK/NG references.
- `wpf_shell_host_workspace_sample_bad_reference_audit`
  - Covers every runnable Bad reference, classifies it as controlled NG or comparative Bad, verifies expected metric/result behavior, and guards against blindly converting all Bad references to `ExpectedFailure`.
- `wpf_shell_host_workspace_sample_open`
  - Covers the display-only sample workflow strip, sample-ready top status banner, and no auto-open/no auto-preview behavior.
- `wpf_shell_host_workspace_sample_actions`
  - Covers explicit sample next-action buttons for Pipeline Review and first-step tool open without auto Preview/Run.
- `wpf_shell_host_workspace_sample_pipeline_review_metrics`
  - Covers opening public-safe `Public_Blob_Particles_Good`, verifying catalog baseline metrics, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, OK decision text, primary result metric, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_ng_metrics`
  - Covers opening public-safe `Public_Mean_Brightness_Dark_Bad`, verifying controlled `MeanValueAvg` NG baseline behavior, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, beginner NG next action, metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics`
  - Covers opening public-safe `Public_Feature_Card_Wrong_Bad`, verifying controlled `ScoreMax` NG baseline behavior, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, beginner NG next action, score metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics`
  - Covers opening public-safe `Public_Line_Pins_WidePin_Bad`, verifying controlled `DistanceMmAvg` NG baseline behavior, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, beginner NG next action, distance metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics`
  - Covers opening public-safe `Public_Blob_Particles_Sparse_Bad`, verifying controlled `ResultCount` NG baseline behavior, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, beginner NG next action, Blob count metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics`
  - Legacy-named target retained for compatibility; covers opening public-safe `Public_Contour_Shapes_Missing_Bad`, verifying controlled `ResultCount` NG baseline behavior, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, beginner NG next action, result-count metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_film_ng_metrics`
  - Legacy-named target retained for compatibility; covers opening public-safe `Public_Threshold_BandPads_Missing_Bad`, verifying controlled `ResultCount` NG baseline behavior, binding Pipeline Review to the active `Sample_` pipeline, explicit Run Review, beginner NG next action, result-count metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_recipe_context_switch`
  - Covers explicit recipe A/B context switching, active pipeline display/source resolution, no auto tool open/no auto Preview behavior, native Tool View Add Pipeline appending only to the active recipe/pipeline context, Blob PropertyGrid parameters loading from each recipe's persisted `Blob_1` model after recipe switches, and Threshold custom WPF mode/value state loading from each recipe's `Threshold_ToolState`.
- `wpf_shell_host_recipe_output_route_isolation`
  - Covers selecting different Threshold output write layers in recipe A/B, Add Pipeline writing each selected output only into the active recipe pipeline, recipe switching not leaking the previous recipe's selected output route into the next recipe, and no Preview/Run side effects.
- `wpf_shell_host_recipe_language_controls`
  - Covers readable Shell language selector text, Korean/English selector-driven language switching, recipe selector listing existing workspaces, recipe selection, new recipe creation, and no tool/Preview side effects.
- `wpf_shell_host_layer_management_commands`
  - Covers explicit layer create, image load into an operator layer, docked-layer deletion synchronization, and no tool/Preview side effects.
- `wpf_shell_host_layer_rename_command`
  - Covers explicit operator-layer rename, preserved image data, host/docked title refresh, `Main`/duplicate rename rejection, live layer selector refresh, and no tool/Preview side effects.
- `wpf_shell_host_workspace_image_load`
  - Covers the localized image-ready next-action strip, quick action labels, top status banner, Korean/English language refresh, zoom/pan/pointer status, and no auto-open/no auto-preview behavior.
- `wpf_shell_host_workspace_quick_actions`
  - Covers MainView Threshold/Matching/Line quick action commands opening the expected WPF Tool View against `Main` without running Preview.
- `wpf_shell_host_workspace`
- `wpf_shell_host_large_image`
- Direct EXE: `OpenVisionLab.exe --smoke workspace-startup-empty`

### 3A. Pipeline Review Beginner Loop

Stable behavior:
- Pipeline Review shows the active pipeline step flow, selected step route, input/output previews, validation state, result summary, and run-log context.
- The selected Step summary and guide strip must show one coherent Step identity even when the persisted Step name already begins with its ordinal. Do not render duplicated labels such as `02 02 ...`; the same selection must expose its tool, route, input/output previews, parameter summary, result status, and elapsed time.
- Pipeline Review may show a compact guide strip for the selected step: review position, current step/route, next check, and result decision.
- Pipeline Review may show a localized guide detail row. The detail row explains why a step is pre-run, missing input, disabled, branch-routed, NG, ready for the next step, or final OK.
- Pipeline Review previous/next controls may select another review step, but they must not execute Review/Preview or alter workspace layers.
- Pipeline Review dynamic guide text must be recalculated through localization when the product language changes between Korean and English, while preserving the selected review step.
- Product language changes must refresh selected tool labels without reopening the selected tool document.
- The guide strip is display-only. Opening Pipeline Review, selecting a step, or changing preview mode must not run Review/Preview, create layers, publish results to the main workspace, or change tool input routing.
- `Run Review` remains the explicit execution command. Review execution caches result images inside the review document and updates the guide/result state from the run result.
- Pipeline Review opened from a sample workflow must use the active `Sample_` pipeline and expose sample-result metrics after explicit Review execution.
- Pre-run guide state must not claim `OK`; completed OK/NG state must come from the review execution result.
- Acceptance NG is a first-class review state. A step may execute successfully but fail metric acceptance; Pipeline Review must show an NG decision, a localized beginner-readable reason/next action, run-log context, and the failed step output image for visual inspection.

Relevant smoke:
- `wpf_shell_host_pipeline_review`
  - Covers selected step flow, previous/next navigation, branch input explanation, localized guide text, input/output preview modes, explicit Run Review, validation/result/run-log context, and the guide strip's pre-run/completed decision state.
- `wpf_shell_host_pipeline_review_ng`
  - Covers acceptance NG after successful tool execution, metric target guidance, populated run-log context, and retained failed-step output preview.
- `wpf_shell_host_workspace_sample_pipeline_review_metrics`
  - Covers a real catalog sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, OK decision, primary result metric, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_fixture_review`
  - Covers the real three-Step `Public_Matching_FixturePad` flow with Step 2 selected and verifies one non-duplicated Step identity, Blob tool, branch route, both previews, Fixture ROI/frame parameters, result metrics, elapsed time, and no first-issue state.
- `wpf_shell_host_workspace_sample_normalize_fixture_review`
  - Covers the public non-LLM five-Step `Matching -> RotateScale NormalizeImage -> Threshold -> Blob datum + Blob pad` flow. It verifies the Good and controlled missing-pad catalog contracts, explicit Review execution, reference-sized `DeviceAligned` output, immutable datum `CvROI=210,240,55,55` and pad `CvROI=320,180,60,50`, distinct per-consumer status/evidence, both previews, and no first-issue state. This is one synthetic-pair workflow contract, not a general fixture-robustness claim.
- `wpf_shell_host_workspace_sample_pipeline_review_ng_metrics`
  - Covers a real catalog Bad sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, controlled metric NG, beginner next action, metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_feature_ng_metrics`
  - Covers a real FeatureMatching Bad sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, controlled `ScoreMax` NG, beginner next action, score metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_line_ng_metrics`
  - Covers a public-safe Line Bad sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, controlled `DistanceMmAvg` NG, beginner next action, distance metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics`
  - Covers a real Blob Bad sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, controlled `ResultCount` NG, beginner next action, Blob count metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics`
  - Legacy-named target retained for compatibility; covers a public-safe Contour Bad sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, controlled `ResultCount` NG, beginner next action, result-count metric detail, run log, output preview, and no native Preview side effects before explicit review.
- `wpf_shell_host_workspace_sample_pipeline_review_film_ng_metrics`
  - Legacy-named target retained for compatibility; covers a public-safe Threshold Bad sample opening into Pipeline Review, active `Sample_` pipeline binding, explicit Review execution, controlled `ResultCount` NG, beginner next action, result-count metric detail, run log, output preview, and no native Preview side effects before explicit review.

### 3B. Matching Fixture Reference Teach

Stable behavior:
- The reference-teach action is visible only for a fixture-producing Matching Step and is enabled only when an explicit successful Review has produced finite `FixtureCenterX`, `FixtureCenterY`, `FixtureAngle`, and positive `FixtureScale` metrics.
- The action copies only those reviewed values into `FIXTURE_REFERENCE_X`, `FIXTURE_REFERENCE_Y`, `FIXTURE_REFERENCE_ANGLE`, and `FIXTURE_REFERENCE_SCALE` and saves the active pipeline.
- Saving a reference invalidates the previous review evidence and requires another explicit Review. It must not launch Preview/Run, create or select a layer, change input/output routing, or rewrite any consumer parameter or `CvROI`.
- The UI must tell the operator that the reference image must be confirmed and that consumer ROI is preserved.

Relevant smoke:
- `wpf_shell_host_workspace_sample_fixture_teach`
  - Covers reviewed-pose availability, explicit save, persisted center/angle/scale reference values, stale-result invalidation, and unchanged consumer parameters/routes/layers/native Preview count.

### 3C. Fixture And Relative-ROI Designer

Stable behavior:
- Pipeline Review shows the designer only when one enabled named Matching fixture producer reaches one enabled `NormalizeImage` consumer and one or more later enabled single-`CvROI` Steps through declared layer routing.
- The designer is read-only evidence plus explicit workflow entry points. It shows the named relationship, template/search ROI, reference pose/image size, current pose, score, same-template preflight margin when present, normalized valid-pixel ratio, and every reachable downstream ROI consumer.
- Every consumer row retains stable Step evidence identity, Step/tool name, immutable reference ROI, declared route, and current-run status. Selecting a row changes only the reviewed/highlighted consumer and the target of the existing measurement-ROI edit handoff.
- All saved reference-coordinate ROIs are drawn as transformed polygons on the current source only when a current reviewed pose exists, and as unchanged rectangles on the current normalized image only when NormalizeImage succeeded. The selected consumer is visually distinguished from the other consumers.
- `참조 자세 저장`, producer edit, measurement-ROI edit, and `리뷰 실행` reuse the existing reference-teach, Recipe Manager PropertyGrid, and explicit Run Review paths.
- Selecting the tab, a consumer row, or either drawing must not execute Preview/Run, create/select a layer, change the active layer, alter input/output routing, or modify the saved recipe.
- Translation-only legacy Fixture reference teach remains available for pipelines without a NormalizeImage/downstream-ROI chain.

Do not:
- Add a locator, move the saved ROI per image, infer a margin from an unrelated Matching Step, silently weaken gates, or present the designer as recipe qualification.
- Add a second parameter editor or make template/ROI/reference edits auto-run the pipeline.

Relevant smoke:
- `wpf_shell_host_workspace_sample_normalize_fixture_review`
  - Covers two-consumer relationship resolution, stable consumer identities, source and normalized ROI drawings, selected-row highlight/edit target, template/reference/current/quality state, explicit action availability, controlled Good/Bad replay, and zero tab/row-selection execution/layer/routing side effects.
- `wpf_shell_host_workspace_sample_fixture_teach`
  - Preserves legacy translation-Fixture reference teach and its zero-auto-run contract.
- `wpf_shell_host_pipeline_step_edit_handoff`
  - Covers the authoritative Recipe Manager PropertyGrid edit route.
- `wpf_shell_host_recipe_fixture_properties`
  - Covers Matching/NormalizeImage Fixture parameter round trip.

### 4. Tool Inline Preview Viewer

Stable behavior:
- Tool input/output preview slots support image display, zoom, pan, and fit reset.
- Tool input preview slots can show configured ROI overlays for the currently selected PropertyGrid tool.
- A plain left click activates the associated layer.
- A real pan gesture must not be treated as a layer click.
- Output preview click must make both active layer and workspace layer match the output layer.

Do not:
- Remove drag/pan behavior from inline preview slots.
- Remove ROI overlay rendering from tool input previews.
- Handle all mouse events in the viewer without forwarding plain clicks to the tool action layer.

Relevant smoke:
- `wpf_shell_host_blob_tool`
- `wpf_shell_host_contour_tool`
- `wpf_shell_host_line_measure_tool`
- `wpf_shell_host_matching_tool`
- `wpf_shell_host_tool_input_image_load_save`

### 5. Blob Threshold Teaching Versus Run Result

Stable behavior:
- `USE_THRESHOLD` and `USE_ADAPTIVE_THRESHOLD` toggles update PropertyGrid visibility only; they do not immediately execute preview/run.
- Moving the threshold slider schedules an auto-preview that shows the threshold teaching image.
- During threshold teaching auto-preview, result review stays in the not-run state and must not show Blob detection count/area/center/box.
- Pressing Run/Preview executes Blob detection and then updates result review with count, max area, center, and box.
- Blob output preview is binary-like/grayscale during threshold teaching and must not show detection markers before Run.

Do not:
- Draw Blob detection markers during threshold teaching auto-preview.
- Leave stale Run result chips visible while the operator is only tuning threshold values.
- Reuse a previous Run result to imply a new detection happened.

Relevant smoke:
- `wpf_shell_host_blob_tool`
- `wpf_threshold_to_blob_detection_e2e`

### 5A. Tool Parameter Persistence And ROI Teaching Memory

Stable behavior:
- The operator's last edited tool parameters are part of the teaching workflow and must survive tool close/reopen within the same recipe.
- PropertyGrid-based inspection tools persist their selected property object through the common tool property session/config path.
- The legacy recipe XML path already exists through `VisionToolStorage`, `RecipeRuntimeStorage`, and `OpenCvPropertyBase.LoadConfig/SaveConfig`. WPF native tool creation must reuse that persistence contract instead of bypassing it.
- WPF native PropertyGrid tools must use repository-owned recipe property objects when they exist: `Blob_1`, `Contour_1`, `Line(L)_1`, `Line(R)_1`, `Matching_1`, `Feature_1`, and `EdgeBasedMatching_1`.
- Recipe context changes must clear/recreate cached native PropertyGrid documents before the operator can Add Pipeline from them; otherwise a reused selected object can leak recipe A parameters into recipe B.
- WPF custom/dynamic tools also persist teaching parameters per recipe under `RECIPE/<recipe>/VISION/*_ToolState.xml` (`Threshold_ToolState`, `Filter_ToolState`, `Morphology_ToolState`, `Arithmetic_ToolState`, and SimplePreprocess tool states).
- Threshold custom WPF mode/value state is covered by `wpf_shell_host_recipe_context_switch`; after switching recipes, Add Pipeline must use the active recipe's `Threshold_ToolState`, not the previously opened Threshold view's state.
- Line A/B ROI and parameter edits persist separately; Line A must not overwrite Line B and vice versa.
- Threshold, Filter, and Morphology custom WPF tools keep their ViewModel state when the tool view is reopened during the current app session.
- When a tool has a saved ROI, loading or refreshing the input preview shows that ROI over the image before Run/Preview.

Do not:
- Recreate fresh default parameter objects for existing tools unless the user explicitly requested reset/defaults.
- Introduce a WPF factory path that creates ad hoc property objects and bypasses recipe XML load/save.
- Change persisted tool names casually. XML file naming is part of the recipe compatibility contract.
- Remove `*_ToolState.xml` persistence from WPF custom/dynamic tools just because they are not PropertyGrid-backed.
- Treat ROI as a one-shot dialog result that disappears after the editor closes.
- Hide the previously configured ROI just because no preview/run result exists yet.
- Couple parameter persistence to output layer creation or pipeline append; these are separate operator actions.

Relevant smoke:
- `wpf_shell_host_blob_tool`
- `wpf_shell_host_line_tool`
- `wpf_shell_host_matching_tool`

Completion evidence:
- 2026-06-28: `artifacts\ui_precheck_product_ux_core_tools_after_threshold_fix_20260628`
- 2026-06-28: `artifacts\ui_precheck_product_ux_matching_edge_20260628`

Related docs:
- `docs\VISION_TOOL_PROPERTY_GRID_POLICY.md`
- `docs\VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`

### 5B. Native WPF Tool Addition Guard

Stable behavior:
- Every visible native WPF tool menu must have exactly one creation path through `OpenVisionNativeToolRegistry`.
- Every `OpenVisionNativeToolRegistry` entry must be visible from the shell navigation. `Pipeline` is the explicit non-native exception.
- Native tool prewarm must include every registered native tool and keep the heavy inspection tools first in the base priority order.
- A new tool must choose one extension lane before implementation: PropertyGrid tool, custom UI tool, or SimplePreprocess tool.
- Tool-specific behavior may stay visible in the lane factory, but layer selectors, preview state, run state, and Add Pipeline wiring should come from the shared runtime/controller path.

Do not:
- Add a shell navigation item for a native tool without registering a document factory.
- Register a native tool that the operator cannot reach from the shell menu.
- Copy layer combo, preview button, status, and pipeline-button wiring into each new view when an existing runtime/controller lane applies.
- Hide a new algorithm tool's parameters in a one-off custom panel when the property model and PropertyGrid lane can express them.

Relevant check:
- `VisionUiContractCheck`: `NativeToolNavigationContract=OK`
- `VisionUiContractCheck`: `NativeToolPrewarmContract=OK`

### 5C. Floating Tool Window Placement

Stable behavior:
- Floating tool windows must not default to owner-centered placement because that hides the main image workspace and weakens operator verification.
- First show uses smart placement: prefer the right side of the main window, then left side, then a right-aligned in-owner fallback when screen space is limited.
- The last user-visible floating tool window position and size are persisted under `CONFIG/UI/FloatingToolWindow.bounds`.
- Hidden warm-up windows must never persist offscreen coordinates.

Do not:
- Reintroduce center-owner placement as the normal path for native tool windows.
- Save placement while the prewarmed hidden window is parked offscreen.
- Couple this placement policy to layer docking or result publishing. Tool-window layout and result comparison layout are separate operator concerns.

### 5D. Tool Window UX And Theme Direction

Stable behavior:
- Native WPF tools open as floating tool windows by default. Docked tool inspection is an explicit operator mode, not the default open path.
- Do not auto-dock a tool just because the shell has an inspector panel available. The main workspace and layer comparison workflow must remain operator controlled.
- Floating tool windows expose an operator-visible `우측 고정` action. This moves the current hosted tool view into the right inspector without recreating the tool view or treating the action as a close.
- Docked tool inspectors expose the reverse float action. Floating and docking must preserve the current tool content, selected layers, parameter state, and preview state.
- Docked tool inspector headers must show the active tool title and operator-visible float/close actions. These buttons must stay clickable, localized, and large enough to use without relying on the floating-window chrome.
- When the docked tool inspector is visible, selecting the same native WPF tool again must reuse the docked inspector instead of creating a duplicate floating tool window.
- When the docked tool inspector is visible, selecting another native WPF tool must replace the hosted docked content in that inspector and must not leave the previous tool behind as a hidden or visible duplicate floating window.
- Reselecting a docked native tool with an existing displayable preview result must restore the Shell top direct-result `OK` state and route text from the active native document. This is a status synchronization step only; it must not rerun Preview/Run, create a layer, or change the input route.
- The docked tool inspector is an operator-sized work panel. If the operator adjusts its width, reopening or reselecting docked tools must preserve a practical inspector width instead of resetting to a narrow default.
- Docked single-input tool views use compact input/output preview cards above the parameter area, and the PropertyGrid/parameter editor gets the inspector's full width. Do not return docked PropertyGrid tools to the old narrow right-column layout.
- Docked single-input preview cards are compact route/thumbnail checks, not the primary image workspace. Keep their image frames small enough that PropertyGrid editing gets the inspector height priority.
- Docked preview cards with an image show a small non-text route hint icon and hand cursor. This means the thumbnail can route the selected input/output layer to the central workspace; do not add explanatory text inside the small thumbnail frame or turn it into a second full viewer.
- Compact docked preview empty states show only short status content. Do not render the full no-image description/load card inside the small docked preview frame where it clips.
- Docked tool content must fit the inspector viewport so primary actions such as Add Pipeline and Run Preview remain visible. Prefer internal editor scrolling over whole-tool scrolling that hides the action row below the fold.
- Docked tool hosts temporarily relax floating-view `MinWidth`/`MinHeight` constraints and restore them when floated again. This prevents Threshold/Filter/Morphology/Arithmetic views from clipping inside the 600px inspector.
- Docked single-input PropertyGrid tool views keep compact side-by-side input/output previews, give the PropertyGrid the inspector's full width, and keep Result Review plus Add Pipeline/Run Preview anchored near the bottom without a large trailing blank area.
- Docked PropertyGrid tools must not let Result Review, summary, status, or action rows overlap the PropertyGrid editor. The PropertyGrid must resize to the remaining inspector height and use its own internal scroll for lower parameters.
- Docked single-input PropertyGrid tool views enable WPG compact density for TextBox, ComboBox, ComboBoxItem, CheckBox, Slider, and bridge surface padding. This is docked-inspector-only density; do not apply it globally to floating tool windows unless the operator UX is explicitly revalidated.
- Docked single-input PropertyGrid tool result review stays compact. Long result chip lists must scroll inside the result review area instead of taking height away from the PropertyGrid or pushing Add Pipeline / Run Preview below the inspector.
- Docked PropertyGrid tools preserve the operator's vertical parameter scroll position per selected property object when switching away from a tool and returning to it. Do not reset long Matching/EdgeBased/Line-style grids back to the top unless the selected property object actually changes.
- PropertyGrid search boxes are part of the operator's parameter navigation state. The shared WPG bridge keeps search instant, supports Escape-to-clear, localizes the search hint, and preserves the search text per selected property object when switching docked tools.
- PropertyGrid search must remain operator-readable in docked tools. If the active search text matches no visible property rows, the shared WPG bridge shows a non-blocking empty-search message; clearing the search hides it again.
- Docked double-input/custom tool views keep compact input/output preview cards on the left, reserve the remaining height for the parameter panel, and keep Add Pipeline plus the active Run action near the bottom of the inspector instead of leaving a large trailing blank area below the action row.
- Docked tool summary/status areas use compact inspector strips. Summary text remains a single trimmed line; status text appears as a compact bordered strip only when non-empty and collapses when empty so the inspector does not show blank status rows.
- 2026-06-28 docked Matching/EdgeBasedMatching/FeatureMatching/Line smoke guards: these tools must preserve the hosted native tool view when docked, keep input/output preview cards visible, keep PropertyGrid editing readable, keep result review text available, and must not leave a duplicate floating tool window behind. Validation artifact: `artifacts\ui_precheck_docked_matching_line_20260628`.
- 2026-06-28 docked Threshold/RotateScale/Morphology/Arithmetic smoke guards: preprocessing and arithmetic tools must keep preview cards, parameter controls, result/summary area, and primary actions visible in the docked inspector. Arithmetic smoke must select Operation mode before asserting Input B controls, because Offset mode legitimately hides Input B. Validation artifact: `artifacts\ui_precheck_docked_preprocess_arithmetic_20260628_b`.
- 2026-06-28 dock/float cycle guard: Blob floating -> docked -> floating -> docked -> Matching docked switching must keep exactly one active native WPF tool, preserve the operator-adjusted dock width, and end with no duplicate floating windows. Validation target: `wpf_tool_window_dock_float_cycle`. Validation artifacts: `artifacts\ui_precheck_tool_dock_float_cycle_20260628` and actual product entrypoint `artifacts\actual_exe_tool_dock_float_cycle_20260628`.
- 2026-06-28 docked tool header guard: docked Blob/Matching switching must keep a non-empty tool title plus visible float/close buttons with localized tooltips. Validation artifact: `artifacts\ui_precheck_docked_tool_header_20260628`.
- 2026-06-28 compact docked preview guard: docked single-input tools keep input/output preview frames compact so parameter editing remains the primary inspector task. Validation artifact: `artifacts\ui_precheck_compact_docked_previews_20260628`.
- 2026-06-28 docked PropertyGrid density guard: docked Blob/Matching switching must keep WPG compact density enabled while preserving floating PropertyGrid spacing. Validation artifact: `artifacts\ui_precheck_docked_property_grid_density_20260628`.
- 2026-06-28 docked long-parameter review guard: Matching/EdgeBasedMatching/FeatureMatching style docked tools must keep the result review compact with internal chip scrolling while preserving PropertyGrid editing height and action visibility. Validation artifact: `artifacts\ui_precheck_docked_long_property_review_20260628`.
- 2026-06-28 docked PropertyGrid navigation guard: docked Matching scroll position must survive switching to Blob and back to Matching. Validation artifact: `artifacts\ui_precheck_docked_property_grid_navigation_20260628`.
- 2026-06-28 docked PropertyGrid search guard: docked Matching search text must survive switching to Blob and back to Matching. Validation artifact: `artifacts\ui_precheck_docked_property_grid_search_20260628`.
- 2026-06-28 docked PropertyGrid empty-search guard: docked Matching must show the empty-search message for a no-match search and hide it when the search is cleared. Validation artifact: `artifacts\ui_precheck_docked_property_grid_search_empty_20260628`.
- 2026-06-28 docked summary/status strip guard: docked Matching and Arithmetic keep compact summary/status strips; empty status rows collapse and non-empty status rows remain single-line and visible. Validation artifact: `artifacts\ui_precheck_docked_status_result_strip_20260628`.
- 2026-06-28 docked preview route hint guard: docked Matching and Arithmetic image thumbnails show the route hint icon/click affordance, while empty thumbnails do not. Validation artifact: `artifacts\ui_precheck_docked_preview_route_hint_20260628`.
- 2026-06-28 central layer tab readability guard: Shell layer tabs render structured index/title/status fields, reject placeholder tabs, and remain readable before layer docking comparison. Validation artifact: `artifacts\ui_precheck_central_layer_tabs_20260628`.
- 2026-06-28 docked layer header readability guard: docked comparison headers and tabs keep readable layer titles, drag handles, and image-size badges while preserving AvalonDock tab gestures. Validation artifact: `artifacts\ui_precheck_docked_layer_headers_20260628`.
- 2026-06-28 docking/result layout guard: docked comparison panes reject too-narrow viewer columns, AvalonDock tab defaults remain intact, and docked PropertyGrid editors must not be overlapped by Result/Summary/Action rows. Validation artifact: `artifacts\ui_precheck_docking_result_layout_20260628_b`.
- Every live image/result layer must be mirrored into the central AvalonDock workspace as a same-pane tab as soon as the layer exists. The top Shell layer strip is a selection/status strip only; it must not be the primary drag source for docking. Operators drag the AvalonDock layer tab itself to split/compare layers. Validation artifact: `artifacts\ui_precheck_workspace_custom_docking_20260629_v2`.
- Docked/AvalonDock layer tab/title dragging must not spawn a large native floating document window over the workspace. Layer comparison movement is owned by the wrapper workspace gesture/guide path, while AvalonDock remains the underlying pane/layout control.
- 2026-06-29 Visual Studio-style layer docking contract: global and pane-local dock semantics must stay aligned with DockingManager expectations: global side commands split the whole workspace, pane-side commands split the target pane, and center/tab commands merge into the target pane. Docked layer tabs must be top-aligned, not bottom tab strips. Validation: `tools\RunDockingVerification.ps1` runs the actual EXE `--smoke layer-docking-verification` and asserts tabbed, GlobalRight, GlobalBottom, pane-local Bottom, nested restore, center-tab merge behavior, and native floating preview suppression.
- 2026-06-29 docking DLL boundary guard: AvalonDock package ownership starts in `src/Libraries/OpenVisionLab.Docking.Controls/OpenVisionLab.Docking.Controls.csproj`. Do not add `Dirkster.AvalonDock` back as a direct `src/OpenVisionLab/OpenVisionLab.csproj` package reference. New docking policy/control code should move toward `OpenVisionLab.Docking.Controls`; ShellHost should consume it through wrapper/control APIs instead of growing more AvalonDock-specific code-behind. Validation: `dotnet list src/OpenVisionLab/OpenVisionLab.csproj package --include-transitive` must show `Dirkster.AvalonDock` only as transitive, plus `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` and `OpenVisionLab.exe --smoke layer-global-docking`.
- 2026-06-29 docked-layer state DTO guard: Shell/test diagnostics should read docked comparison state through snapshot DTOs such as `OpenVisionDockedLayerWorkspaceState`, `OpenVisionDockedLayerDocumentState`, and wrapper-owned visual snapshots, not by reaching into AvalonDock document/pane objects from ShellHost. The snapshot must preserve content id, native float suppression state, viewer tile readiness, compact chrome state, pane count, root orientation, nested layout count, and top-aligned tab/header bounds. Validation target: `tools\RunDockingVerification.ps1`.
- Operator-facing tool labels must resolve to localized/display text in both floating and docked modes. Do not expose resource keys such as `Morphology.Operation.Erode` or `Morphology.Shape.Rect` in buttons, radio buttons, or summaries.
- The former right-side Shell rail must not return as a permanently visible three-card stack. Direct tool execution state belongs in a compact top status strip, while layer/result selection belongs in the central workspace tab strip.
- The selected-layer detail card is no longer a primary Shell surface. Keep selected-layer information visible in the central workspace overlay/status where needed, but do not re-add the old fixed right detail panel without an explicit UX decision.
- The central layer/result tab strip shows real layers only. Do not render placeholder rows such as `00 - none` as operator tabs when no actual layer image exists.
- The central layer/result tab strip uses structured tab items: stable index, trimmed layer title, and a separate state badge such as `OK` or `표시`. Do not collapse it back into one formatted string, because tab width, overflow readability, and status scanning depend on the separated fields.
- Docked layer comparison headers and tabs are operator navigation surfaces. Keep the drag affordance, trimmed layer title, and compact image-size badge visible so operators can tell which comparison layer is selected without opening another detail rail.
- Docked layer comparison panes must remain large enough for visual comparison after tab drag/drop or right-side docking. Do not let AvalonDock create sliver panes where the image is technically present but not usable.
- Docked layer comparison tab chrome should stay close to AvalonDock behavior. Wrapper-owned tab/pane styles are allowed only to keep tabs top-aligned, readable, model-bound, and compatible with the wrapper drag/drop gesture. Do not add duplicate title rows, boxed metadata cards, or custom tab controls that bypass the wrapper workspace contract.
- Clicking a docked layer tab to switch images must not show the docking guide. The guide appears only after a tab/header drag passes the normal WPF drag threshold or while an actual drag/drop is in progress. Validation target: `OpenVisionLab.exe --smoke layer-docking-tab-click-no-guide`.
- If WPF `DragDrop` returns without a `Drop` event while a docked layer drag is active, the wrapper may finalize the dock command from the current cursor position. This fallback exists to keep global bottom/top edge drops reliable after native floating previews are suppressed.
- Multi-document docked layer panes must show one continuous top tab row only. Do not show a separate pane title/header row above the tabs; keep the pane title/header only for single-document panes where it is needed as the drag surface.
- Pane-local and global docking guides are owned by the docking wrapper. Do not route ShellHost directly back to raw AvalonDock overlay APIs, and do not let AvalonDock native floating previews cover the workspace during layer comparison dragging.
- The top direct-run status strip is a status surface, not a command toolbar. Keep it compact, single-line, and text-trimmed so it does not compete with the workspace or tool inspector.
- Floating tool windows use smart placement so they do not cover the main workspace by default.
- Tool views use shared `VisionToolWpfTheme.xaml` tokens. The visual direction is Shell-compatible neutral inspector styling: dark image workspace, teal accent, muted blue-gray tool surface, bright readable input fields.
- PropertyGrid-heavy tool views must keep value editing readable. TextBox, ComboBox, RangeEditor, and Slider fields stay high-contrast and easy to scan.
- Tool views should not return to disconnected pure-white card styling when they are meant to sit beside or dock into the WPF shell.
- 2026-06-28 inspector theme alignment guard: `VisionToolWpfTheme.xaml` keeps tool panels on the same neutral inspector palette as the WPF Shell, while preserving readable light input fields and teal actions. Validation artifact: `artifacts\ui_precheck_inspector_theme_alignment_20260628`.
- 2026-06-28 single-input docked density guard: Blob and Matching docked layouts keep PropertyGrid editing readable, preserve result review/action visibility, and reject excessive blank space below Run Preview. Validation artifact: `artifacts\ui_precheck_single_input_docked_density_20260628`.
- 2026-06-28 double-input docked density guard: Arithmetic docked layout keeps input/output preview frames readable, places the active run action near the bottom of the inspector, and preserves route behavior. Validation artifact: `artifacts\ui_precheck_double_input_docked_density_20260628_retry`.

Do not:
- Make native tools open docked by default without an explicit operator UX decision.
- Treat dock-to-right as a close path that clears `OpenVisionNativeToolDocument` state.
- Recreate a fresh tool view when docking/floating an already-open tool. Move the hosted content instead.
- Open duplicate floating native tool windows while that native tool is already hosted in the docked inspector.
- Leave a previous native tool window alive when selecting a different tool from docked mode.
- Reapply floating tool minimum sizes while a tool is hosted inside the docked inspector.
- Let localization/resource keys leak into operator-facing tool text.
- Reintroduce a permanent right-side selected-layer detail rail that competes with the docked tool inspector.
- Make PropertyGrid-heavy tools fully dark if that reduces parameter readability.
- Hard-code one-off tool colors when a shared `VisionTool.*` or `OpenVision.*` design token should be used.

### 6. Output Layers And Comparison Panels

Stable behavior:
- Output layer creation is explicit.
- Every live image/result layer is mirrored into the central AvalonDock workspace as a same-pane tab when the layer exists.
- Workspace comparison placement is explicit: operators drag AvalonDock layer tabs or use docking commands to split, merge, or compare layers.
- Operators may create multiple output layers and choose how named result layers are arranged for comparison.

Do not:
- Automatically create extra output panels while a tool is merely opened or previewed.
- Automatically split or rearrange the workspace when a preview/result layer is created.
- Reintroduce a separate top Shell layer strip as the primary docking source while the AvalonDock workspace owns layer tabs.
- Remove the ability to compare multiple named result layers.

Relevant smoke:
- `wpf_shell_host_layer_docking_functional`
- `wpf_shell_host_layer_global_docking`
- `wpf_shell_host_layer_docking_n_panels`
- `wpf_shell_host_workspace_output`
- `tools\RunDockingVerification.ps1`
- `OpenVisionLab.exe --smoke workspace-startup-empty`
- `OpenVisionLab.exe --smoke layer-initial-docked-workspace`
- `OpenVisionLab.exe --smoke layer-docking-mouse-drag`

### 7. Preprocessing Custom Parameter Layout

Stable behavior:
- Filter and Morphology custom parameter controls must keep labels, text boxes, lock toggles, preset buttons, and shape/options controls in separate visible rows.
- Korean labels must not overlap adjacent controls or preset buttons.
- Mode-specific panels may hide, but the remaining controls must not jump into confusing positions.

Do not:
- Put kernel inputs, lock toggles, and preset buttons back into one uncontrolled flow row.
- Treat screenshot-only appearance as enough; keep bounds-based layout smoke for these controls.

Relevant smoke:
- `wpf_filter_morphology_layout_guard`

### 8. Arithmetic Double Input Layers

Stable behavior:
- Arithmetic has independent Input A and Input B layer routes.
- If more than one input layer exists, Input B must not silently default to the same layer as Input A.
- Creating/changing the output layer must preserve the selected A/B input route.
- Offset mode may hide Input B because it intentionally uses Input A plus offset.

Do not:
- Collapse Arithmetic back to single-input routing.
- Treat `Main` as a forced Input B when another layer is available.

Relevant smoke:
- `wpf_layer_selection_arithmetic_tool`

### 9. EdgeBasedMatching Hybrid Verification

Stable behavior:
- EdgeBasedMatching keeps the PropertyGrid model-driven parameter UI.
- Hybrid verification is default off and explicit opt-in through `USE_HYBRID_VERIFY`.
- `HYBRID_VERIFY_TOP_N` and `HYBRID_VERIFY_IMAGE_WEIGHT` are visible only under Hybrid verify.
- Hybrid verification re-ranks edge candidates with image-template similarity. It also preserves spatial grid candidates and adds one image-matching proposal whose edge score is recomputed before selection.
- Hybrid verification must not change `SCORE_MIN` semantics or the public result `Score`; the result score remains the edge score.
- Template/model caching keeps the prepared template, base edge model, and rotated edge models for repeated preview/run on the same tool instance. It is result-neutral and must be cleared on template or relevant preprocessing/model parameter changes.
- Probe validation on 2026-06-27 showed the template/model cache kept the same EasyMatch and synthetic probe results. The repeated EasyMatch run still measured about 45-58 ms, so the next speed bottleneck is source gradient generation and position scanning, not template model creation.
- UI validation on 2026-06-27: `artifacts\ui_precheck_edge_based_template_model_cache_20260627` passed for `wpf_shell_host_edge_based_matching_tool`.
- 2026-06-27 speed pass: source/template gradient unit vectors are precomputed and scoring uses array-based dot products. `Cv2.CartToPolar` was replaced with `Cv2.Magnitude` because angle output was unused. Focused probes kept the same score/angle/center outputs; EasyMatch `EdgeBased coarse angle step4 refine` measured about 32.6 ms in the sequential probe run. UI validation: `artifacts\ui_precheck_edge_based_unit_gradient_20260627`.
- 2026-06-27 sample rotation benchmark: `.codex\EdgeBasedSampleRotationBenchmark` validates 10 EasyMatch sample images with inserted `0, +5, +10, -5, -10` degree targets. Pass criteria are center error <= 6 px and angle error <= 2 deg. Latest artifact: `artifacts\edge_based_hybrid_perf_20260627`.
- Latest 10-sample rotation result with threshold disabled for measurement after downscaled Hybrid proposal, edge-descriptor near-tie re-rank, per-angle-batch source resize reuse, and scaled proposal top K=2: `ImageCoarse` 50/50 pass, average 55.351 ms; `ImageExhaustive` 50/50 pass, average 58.934 ms; `EdgeOptimized` 43/50 pass, average 53.018 ms; `EdgeHybrid` 50/50 pass, average 126.511 ms, median 121.569 ms. This means EdgeHybrid is the safer repeated-pattern option, but it is still not a free default because it runs image proposals during angle search.
- EdgeBasedMatching Hybrid verify may add image-matching proposals even when angle search is enabled. This is required for repeated edge-pattern samples such as Floppies where pure edge score can select the wrong repeated structure. The image proposal path may search on a downscaled source/template and then refine the selected top-left location on the original image, but it must recompute the edge score from the original gradient image before selecting the result. The downscaled source image is resized once per angle proposal batch; do not move that resize back inside the angle loop. The scaled proposal keeps top K=2 because K=1 kept accuracy in the current benchmark but measured slower and leaves less candidate margin. Hybrid re-rank may use a Canny edge-map descriptor only as a near-tie resolver for candidates near the best image location; it must not let a far repeated-pattern location override the image proposal. Keep this opt-in because it materially increases runtime. UI validation: `artifacts\ui_precheck_edge_based_hybrid_perf_20260627`.
- 2026-06-27 follow-up: Hybrid verification reuses the image proposal candidate's existing image score instead of recomputing the same template match during re-rank. This is a result-neutral duplicate-work removal; public edge score, angle, center, and result drawing semantics must remain unchanged. Validation artifact: `artifacts\edge_based_hybrid_score_reuse_20260627`; UI validation: `artifacts\ui_precheck_edge_based_hybrid_score_reuse_20260627`.
- 2026-06-27 diagnostic pass: EdgeBasedMatching now has opt-in phase timing through `CollectPhaseTimings`. It is diagnostic-only and must remain off by default. Benchmark artifact: `artifacts\edge_based_phase_timing_20260627`. The measured bottleneck order is `SearchEdgeCandidate` first, `HybridImageProposal` / `HybridProposal.ScaledMatch` second, and source gradient third. Descriptor matching, draw result, model cache, and preprocess are not first-priority speed targets. See `docs\EDGE_BASED_MATCHING_PERFORMANCE_ANALYSIS.md`.
- 2026-06-27 Hybrid proposal speed pass: angle proposals inside Hybrid image verification may run in parallel when enough proposal angles are present. This is result-neutral and must keep public edge-score semantics unchanged. Validation artifact: `artifacts\edge_based_parallel_hybrid_proposal_20260627`; UI validation: `artifacts\ui_precheck_edge_based_parallel_hybrid_proposal_20260627`.
- 2026-06-27 Search hot-loop speed pass: candidate scoring may cache per-model source index offsets and per-context early-break thresholds. This is result-neutral and must not change public edge-score semantics. Validation artifact: `artifacts\edge_based_score_context_20260627`; UI validation: `artifacts\ui_precheck_edge_based_score_context_20260627`.
- 2026-06-27 Hybrid fast path speed pass: single-match Hybrid verify may skip full edge candidate search only when the image proposal is high-confidence (`ImageVerifyScore >= 0.985`) and its recomputed edge score is at least `max(SCORE_MIN, 0.70)`. Otherwise it must fall back to full edge search plus Hybrid verification. Public result `Score` remains the recomputed edge score. Validation artifact: `artifacts\edge_based_hybrid_fast_path_20260627`; UI validation: `artifacts\ui_precheck_edge_based_hybrid_fast_path_20260627`.
- 2026-06-27 model quality diagnostics pass: EdgeBasedMatching may publish `Model.*` metrics for template size, raw/final edge point count, point sample ratio, edge density, edge coverage, quadrant balance, and simple risk flags. These metrics are telemetry only and must not change result scoring, overlays, recipe values, pipeline acceptance, or preview/run behavior. Validation artifact: `artifacts\edge_based_model_diagnostics_20260627`; visible UI validation: `artifacts\ui_precheck_edge_based_model_diagnostics_visible_20260627`.
- 2026-06-27 candidate retention diagnostics pass: EdgeBasedMatching may publish `Candidate.*` metrics for image proposal count, fast path count, fallback search count, seed count, hybrid verification candidate count, verified count, image-proposal selection count, fallback selection count, and max proposal/search scores. These metrics are telemetry only and must not change result scoring, overlays, recipe values, pipeline acceptance, preview/run behavior, or candidate pruning. Validation artifact: `artifacts\edge_based_candidate_diagnostics_20260627`; UI validation: `artifacts\ui_precheck_edge_based_candidate_diagnostics_20260627`.
- 2026-06-27 rejected experiment: position-pyramid coarse search was not accepted. It either worsened runtime or broke the 50/50 EdgeHybrid rotation benchmark. Do not reintroduce that approach without a redesigned candidate-retention strategy and sample-backed proof.
- 2026-06-27 option matrix notes: `SearchStep=5/6`, `CoarseStep=10`, and `Greediness=0.95` did not improve the 10-sample benchmark. `HybridTopN=3` and `MaxTemplatePoints=180` passed the current sample set, but they were not promoted to product defaults because the measured speed win was small/noisy and `MaxTemplatePoints` can affect generalization on richer edge templates.
- Probe validation on 2026-06-27 showed `Search step=2 + Hybrid verify` and `Search step=4 + Refine position + Hybrid verify` suppress the synthetic similar-edge clutter false positive and return the true target region.
- `Search step=4 + Refine position` without Hybrid verify can still pick the wrong coarse candidate in clutter-heavy samples. Treat it as a speed option requiring sample validation, not as a universal default.
- 2026-06-28 current recommendation recheck: `artifacts\edge_based_current_benchmark_20260628` compared default and large EasyMatch sets with baseline and pyramid variants. `EdgeHybrid` remains the safe edge-based recommendation. `EdgeOptimized` alone must not become the production default because repeated-pattern samples such as `Floppies`/`FloppiesLarge` still select wrong repeated structures without Hybrid verification. `Pyramid proposal top N = 6` remains an explicit operator option, not a silent default migration.
- 2026-06-28 pyramid proposal center mapping fix: scaled proposal candidates must be mapped back through `TemplateCenter`, then converted to the full-resolution model origin per refine angle. Do not revert this to `proposal.Center / scale`; that reintroduces large-template shifted verification windows. Validation artifact: `artifacts\edge_based_pyramid_center_mapping_guard_20260628`.
- Pyramid proposal acceptance must keep the weak-verified fallback guard. A proposal whose full-resolution verified edge score only barely clears the configured threshold must fall back to the normal full search rather than being accepted as a confident proposal result.
- 2026-06-28 scale/subpixel pass: EdgeBasedMatching has opt-in scale search through `USE_FIND_SCALE`, `FIND_SCALE_MIN`, `FIND_SCALE_MAX`, and `FIND_SCALE_STEP`, plus final-candidate `USE_SUBPIXEL_REFINE`. Scale search builds scale-specific edge template models and reports result `Scale`; it must not be simulated by resizing only the source while keeping a fixed template model. Subpixel refine is a local 3x3 score-peak center refinement and must not change `SCORE_MIN` semantics.
- P225 Pipeline mapping guard: EdgeBasedMatching PropertyGrid/Pipeline creation and runtime factory execution must preserve scale, subpixel, and pyramid proposal settings. Do not silently restore their defaults when a saved Step explicitly contains these keys.
- When EdgeBasedMatching scale search is enabled, `Pyramid proposal` is intentionally bypassed for correctness. The current scale path uses the full edge search over angle x scale candidates, while Hybrid verify can still re-rank the selected edge candidates with scale-aware verification templates.
- 2026-06-28 EdgeBasedMatching scale multi-match speed path: when `USE_FIND_SCALE=true`, `USE_FIND_ANGLE=false`, `USE_HYBRID_VERIFY=true`, `USE_MULTI_ROI=false`, and `NUM_MATCH>1`, the tool may reuse the first full edge-search candidate seed pool to select multiple non-overlapping matches. If the seed pool is depleted, it must fall back to the existing full edge search for the remaining results. Do not broaden this to angle search or multi-ROI without a separate validation pass.
- EdgeBasedMatching result center semantics are now the visual template center. Drawing edge-model outlines must convert this result center back to the rotated/scaled model center through `TemplateCenterOffset`; do not mix edge-centroid and template-center coordinate contracts.
- Scale verification images must be generated by resizing the whole original sample image. Do not create composite/blurred/pasted test images for scale validation. Current focused probe writes whole-image resized samples to `bin\Debug\EasyMatch\EdgeBasedScaleProbe` and validates 10 EasyMatch samples at 0.90x and 1.10x.
- 2026-06-28 scale probe result: `artifacts\edge_based_scale_subpixel_20260628\scale_summary.csv` reported 20/20 pass, average 189.566 ms, median 155.292 ms, average center error 0.717 px, average scale error 0. BOARD dimensions verified as 772x480 original, 695x432 at 0.90x, and 849x528 at 1.10x.
- 2026-06-28 scale speed probe after seed reuse: `.codex\EdgeBasedScaleProbe` reported 20/20 pass, average 170.187 ms, median 154.718 ms, average center error 0.717 px, average scale error 0. The `SearchEdgeCandidate` phase average was 75.718 ms.
- 2026-06-28 actual EXE scale smoke: `OpenVisionLab.exe --smoke edge-based-scale-matching` passed in `artifacts\actual_exe_edge_based_scale_20260628_final`. The smoke loads a whole-image 0.90x BOARD sample into the real WPF shell, registers a 120x90 template, enables `USE_FIND_SCALE`, runs preview, checks `EdgeBasedMatching_Preview`, verifies `Scale 0.9` in the result review, and verifies XML save/load for `USE_FIND_SCALE`, `FIND_SCALE_MIN/MAX/STEP`, `SEARCH_STEP`, and `USE_SUBPIXEL_REFINE`.
- 2026-06-28 Image Matching scale-search contract: Matching has opt-in target scale search through `USE_FIND_SCALE`, `FIND_SCALE_MIN`, `FIND_SCALE_MAX`, and `FIND_SCALE_STEP`. This creates scale-specific templates and reports result `Scale`. `MAGNIFIATION` remains the existing calculation/downsample magnification and must not be reused as target size correction. Do not auto-enable scale search without an explicit recipe/operator setting.
- 2026-06-28 actual EXE image-vs-edge scale comparison after Image Matching scale search: `OpenVisionLab.exe --smoke matching-vs-edge-based-scale-comparison` passed in `artifacts\actual_exe_matching_vs_edge_scale_after_image_scale_20260628`. On the same whole-image 0.90x BOARD source and 120x90 original template, Image Matching returned `Score 94.458`, `Box 108x80`, `Scale 0.9`, center error 0.500 px, tact 166.7 ms; EdgeBasedMatching returned `Score 97.476`, `Box 108x81`, `Scale 0.9`, center error 0.721 px, tact 269.7 ms. This is a focused actual-EXE scale behavior check, not a universal speed ranking.
- 2026-06-28 10-sample Image Matching vs EdgeBased scale comparison: `.codex\MatchingScaleComparisonProbe` passed 40/40 rows in `artifacts\matching_scale_comparison_20260628`. It uses whole-image 0.90x/1.10x resized samples only. Image Matching scale search passed 20/20 with average 473.771 ms, median 455.425 ms, average center error 0.845 px, average scale error 0. EdgeBasedMatching passed 20/20 with average 179.842 ms, median 147.206 ms, average center error 0.717 px, average scale error 0. Do not treat score values as directly comparable between tools; compare pass, geometry, and tact time.
- 2026-06-28 Image Matching pyramid proposal scale comparison: `.codex\MatchingScaleComparisonProbe` passed 60/60 rows in `artifacts\matching_scale_pyramid_comparison_20260628`. In the same 10-sample whole-image 0.90x/1.10x run, baseline Image Matching averaged 626.925 ms, Image Matching with `USE_PYRAMID_POSITION_PROPOSAL=true` averaged 342.530 ms, and EdgeBasedMatching averaged 236.045 ms. The pyramid option preserved Image Matching score, center error, and scale error on this run. Keep it opt-in and angle-search-off until a separate validation proves broader use.
- 2026-06-28 scale comparison after EdgeBased seed reuse: `.codex\MatchingScaleComparisonProbe` passed 60/60 rows in `artifacts\matching_scale_after_edge_seed_reuse_20260628`. In the same 10-sample whole-image scale run, EdgeBasedMatching averaged 116.962 ms, Image Matching averaged 586.619 ms, and Image Matching with pyramid proposal averaged 326.588 ms. Scores are still not directly comparable across tools; compare pass, geometry, and tact time.
- 2026-06-28 actual EXE recheck after adding Image Matching pyramid proposal: `OpenVisionLab.exe --smoke matching-vs-edge-based-scale-comparison` passed in `artifacts\actual_exe_matching_vs_edge_scale_after_pyramid_20260628`. This confirms the default scale-search path still loads source/template images, publishes `Matching_Preview` and `EdgeBasedMatching_Preview`, and reports `Scale 0.9` for both tools after the new opt-in option was added.
- 2026-06-28 actual EXE Image Matching pyramid smoke: `OpenVisionLab.exe --smoke matching-pyramid-scale` passed in `artifacts\actual_exe_matching_pyramid_scale_20260628`. The smoke enables `USE_PYRAMID_POSITION_PROPOSAL=true`, `PYRAMID_POSITION_TOP_N=8`, and `PYRAMID_POSITION_MIN_SCORE=0.7`, verifies XML save/load for those fields, runs preview through the real WPF shell, checks `Matching_Preview`, and confirms `Scale 0.9` with center error 0.5 px.
- 2026-06-28 actual EXE EdgeBased scale speed smoke: `OpenVisionLab.exe --smoke edge-based-scale-matching` passed in `artifacts\actual_exe_edge_based_scale_after_speed_20260628`. It confirmed `EdgeBasedMatching_Preview`, `Count 5`, `Scale 0.9`, and tact `113.5 ms` on the BOARD 0.90x sample.
- 2026-06-28 Matching Pyramid proposal PropertyGrid UI smoke: `wpf_shell_host_matching_pyramid_property_grid` passed in `artifacts\ui_precheck_matching_pyramid_property_grid_20260628`. It verifies `PYRAMID_POSITION_TOP_N` and `PYRAMID_POSITION_MIN_SCORE` are hidden when `USE_PYRAMID_POSITION_PROPOSAL=false`, become visible under the parent option, preserve edited values, and do not trigger auto-preview when the parent visibility switch is toggled.
- 2026-06-28 EdgeBased scale pyramid survival audit: `.codex\EdgeBasedPyramidScaleSurvivalProbe` wrote `artifacts\edge_based_pyramid_scale_survival_20260628`. On 10 EasyMatch samples, target scales `0.75/0.90/1.10/1.25`, and search range `0.70..1.30`, the true candidate survived Top12 in 37/40 cases at 1/2 level and 25/40 cases at 1/4 level. This is not enough for a hard pyramid candidate cutoff.
- 2026-06-28 EdgeBased scale pyramid gate variants: `.codex\EdgeBasedPyramidScaleGateProbe` wrote `artifacts\edge_based_pyramid_scale_gate_20260628`. With whole-image resized EasyMatch samples and a 1/2 working-level gate, no tested variant reached 40/40 survival under center error `<=16 px` and scale error `<=0.051`. `top24_step1_p220` and `top24_step2_p220_hybrid` reached 39/40 but were slower than the accepted EdgeBased scale seed-reuse path. Do not promote these gate variants into production without a redesigned fallback/candidate-propagation validation.
- 2026-06-28 rejected EdgeBased scale candidate-propagation attempt: `artifacts\matching_scale_edge_pyramid_guarded_single_multi_20260628` showed that a guarded scale-aware 1/2 proposal path still accepted the wrong repeated candidate for `EdgeBasedMatchingSinglePyramid` on `DiePad2@0.9` with center error `49.798 px`. The attempted production code was reverted. `artifacts\matching_scale_after_candidate_propagation_revert_20260628` passed 60/60 rows after the revert. Do not reintroduce scale candidate propagation through the current position-proposal path.
- 2026-06-28 EdgeBased scale quality metrics: `EdgeBasedTemplateMatchingTool` now reports diagnostic-only `Model.ScaleSearchRisk`, `Model.ScaleCoverageWarningRisk`, `Model.LowQuadrantBalanceRisk`, `Candidate.ScaleAmbiguityRisk`, same/different-scale ambiguous alternative counts, and `Candidate.MaxAmbiguousScaleDelta`. `.codex\MatchingScaleComparisonProbe` writes these into `ModelMetrics`/`CandidateMetrics`; `artifacts\matching_scale_quality_metrics_20260628` passed 60/60 rows. These metrics must not reject matches, auto-tune recipes, or accept pyramid proposals until a separate acceptance-rule validation proves the rule.
- 2026-06-28 EdgeBased scale proposal acceptance-rule probe: `.codex\EdgeBasedScaleAcceptanceRuleProbe` wrote `artifacts\edge_based_scale_acceptance_rule_probe_20260628`. The offline rule `ProposalAmbiguousAndGapLe003` blocked the known wrong proposal accept for `DiePad2@0.9` with 0/15 good proposal fallback cost in the rejected artifact. This is not yet a production behavior change. If implemented later, the rule must only reject the proposal shortcut and fall back to the existing full-resolution multi-match search; it must not fail the match, change public score semantics, or auto-edit recipe values.
- 2026-06-28 diverse local image scale probe: `.codex\EdgeBasedDiverseScaleProbe` sampled 60 images from `bin\Debug`, generated whole-image `0.90x/1.10x` sources, and wrote `artifacts\edge_based_diverse_scale_probe_20260628_60`. The current full EdgeBased scale path passed 104/120 rows; 8 failures were low-edge/reference images where the automatic probe crop could not build a template, and 8 were repeated/weak auto-crop geometry failures. Passed rows averaged `0.525 px` center error. `Candidate.ScaleAmbiguityRisk=1` appeared in 99/120 rows, proving again that ambiguity telemetry must not be used as a hard fail.
- 2026-06-28 rejected EdgeBased scale pyramid proposal attempt: `artifacts\matching_scale_edge_pyramid_proposal_20260628` showed `EdgeBasedMatchingPyramid` passed only 19/20 rows and averaged 257.241 ms, while the existing EdgeBased scale path passed 20/20 and averaged 105.661 ms in the same run. The production change was reverted. Keep `Pyramid proposal` bypassed when `USE_FIND_SCALE=true`.
- 2026-06-28 post-revert scale comparison: `artifacts\matching_scale_after_pyramid_revert_check_20260628` passed 60/60 rows, confirming the accepted scale paths were restored after the rejected attempt.
- 2026-06-28 post-audit actual EXE smoke: `artifacts\actual_exe_edge_based_scale_after_pyramid_audit_20260628` passed and confirmed `EdgeBasedMatching_Preview`, `Scale 0.9`, and tact `109.5 ms`.
- Matching-family test hooks must expose result review text for Matching, EdgeBasedMatching, and FeatureMatching, not only Line. This is required for actual EXE smoke verification and must remain read-only.

Do not:
- Auto-enable Hybrid verify, coarse angle search, or position refine without an explicit recipe/operator setting.
- Change `SCORE_MIN` to compare hybrid score unless recipe compatibility is deliberately redesigned.
- Promote `Search step=4 + Refine position` without Hybrid verify as the default for production matching without sample-backed gates.
- Change `HYBRID_VERIFY_TOP_N` or `MAX_TEMPLATE_POINTS` defaults based only on the 2026-06-27 option matrix. Broader sample-backed validation is required before changing those defaults.
- Reintroduce rotated verify-template or verify-template-edge caches for Hybrid verification without a same-sample benchmark win. A 2026-06-27 cache test kept 50/50 accuracy but worsened EdgeHybrid average runtime to 143.204 ms.
- Optimize descriptor matching, draw result, or preprocess before addressing `SearchEdgeCandidate` and `HybridProposal.ScaledMatch`; phase timing shows those are not the dominant costs.
- Reintroduce scaled position-pyramid candidate search as previously attempted; the rejected 2026-06-27 version worsened speed and once reduced EdgeHybrid rotation accuracy to 49/50.
- Convert EdgeBasedMatching scale multi-match seed reuse into a seed-only shortcut. The fallback full search is part of the accepted behavior.
- Apply EdgeBasedMatching scale seed reuse to angle search or multi-ROI without a new sample-backed validation pass.
- Enable EdgeBasedMatching `Pyramid proposal` while `USE_FIND_SCALE=true` until a redesigned candidate-propagation path proves both full pass rate and a speed win on the whole-image scale probes.
- Broaden the Hybrid fast path to multi-match runs, lower its high-confidence thresholds, or report image score as the public edge score without new sample-backed validation.
- Use `Model.*` telemetry to reject results or auto-tune recipe parameters without a separately documented acceptance rule and sample-backed validation.
- Use `Candidate.*` telemetry to skip fallback search, reduce seed count, or prune candidates without a separately documented candidate-retention rule and sample-backed validation.
- Use `Model.ScaleSearchRisk` or `Candidate.ScaleAmbiguityRisk` as hard fail conditions. They are conservative diagnostics for deciding whether a future candidate-propagation experiment is safe enough to test.
- Promote `ProposalAmbiguousAndGapLe003` directly to a match-failure rule. Its only validated role is to block a scale proposal shortcut and continue through full search.
- Generate scale-search validation images by pasting a scaled template into a separate background or by adding artificial blur/noise unless the test is explicitly marked as a synthetic stress test. The normal scale regression must resize the whole original image only.
- Replace Image Matching target scale search with `MAGNIFIATION`. They are different controls: `MAGNIFIATION` changes the working resolution for speed, while `USE_FIND_SCALE` searches actual target size changes.

Relevant focused checks:
- `.codex\MatchingRobustnessProbe`
- `.codex\EdgeBasedAngleProbe`
- `wpf_shell_host_edge_based_matching_tool`
- `wpf_shell_host_edge_based_matching_auto_mpoint`

### EdgeBasedMatching Unique-Result Addendum

Stable behavior:

- Unique-result validation is opt-in. Missing XML keys restore
  `USE_UNIQUE_MATCH_VALIDATION=false` and
  `UNIQUE_MATCH_MIN_SCORE_MARGIN=0.03`.
- Enabled mode accepts only `NUM_MATCH=1`, `USE_MULTI_ROI=false`, and a finite
  normalized margin in `0..1`.
- The Library-Noah matcher retains at least eight internal candidates even when
  the external result count is one.
- A candidate below the existing `SCORE_MIN` is `MatchingNoResult`; a
  spatially distinct plausible alternative whose score margin is below the
  configured gate is `MatchingAmbiguous`. Both return zero `MatchingResult`
  rows.
- A success returns exactly one result and publishes normalized
  `UniqueMatch.State`, `UniqueMatch.PlausibleAlternativeCount`, and
  `UniqueMatch.ScoreMargin` metrics. The result-row margin uses percentage
  points; legacy-disabled rows keep it unavailable.
- PropertyGrid edits and XML mapping do not auto-run Preview. Pipeline validation
  fails closed for invalid one-result/one-region combinations, and Pipeline
  diagnostics retain the exact Library-Noah ambiguity reason.

Do not:

- infer uniqueness from external `NUM_MATCH=1` alone;
- lower the margin merely to force a repeated template to pass;
- relabel bounded synthetic evidence as template, ROI, pose, or field
  qualification;
- begin joint refinement, adaptive size, ODB/CAD, Homography, or multi-anchor
  expansion without the separately required fixed-ROI evidence.

Evidence:

- `artifacts\p224_unique_match_runtime_20260724`
- `docs\OPENVISIONLAB_EDGE_BASED_UNIQUE_MATCH_V1_CONTRACT.md`

## Recipe Review Bundle Import Dry-Run

Stable behavior:

- Selecting a `.review.zip` through either XML import or XML/bundle load opens the existing Recipe Manager `LLM XML` review tab. It does not create a second import window.
- Schema v1 accepts exactly `pipeline.xml` and `review-manifest.json`, applies bounded entry reads, and verifies format/schema, package policy, XML size/SHA-256, summary counts, and manifest dependency rows against the XML Step parameters before exposing the draft.
- The ZIP is read in memory. No entry is extracted, no referenced file is copied, no pipeline is saved/activated, and Preview/Run, Tool View, layer, workspace, and routing state remain unchanged.
- Missing absolute paths may expose one deterministic SHA-matched candidate beside the bundle. This is review evidence only; OpenVisionLab must not rewrite the XML path automatically.
- A referenced dependency whose current size/SHA differs from the export manifest blocks copy/import. A missing or relocation-candidate dependency keeps XML validation NG.
- `Import` is enabled only for the current unchanged draft after validation succeeds. Editing the draft or changing the selected inspection intent invalidates the prior import-ready state.

Do not:

- Treat review-bundle selection as normal XML import.
- Search the disk recursively for replacement files.
- auto-apply a relocation candidate, embed private/local assets, or run Preview/Run during dry validation.

Relevant smoke:

- `wpf_shell_host_recipe_review_bundle_import`
- `wpf_shell_host_recipe_review_bundle`
- latest-build direct EXE `recipe-manager-tabs`

## Recipe Manager Pipeline Inventory

Stable behavior:

- The recipe `VISION` directory may contain both pipeline documents and PropertyGrid/tool-state XML. Recipe Manager pipeline inventory includes only well-formed, no-namespace XML documents whose root element is exactly `VisionPipeline`.
- `pipeline.active.xml`, tool-state/property XML, malformed XML, and unrelated metadata must not appear as pipeline options.
- Inventory refresh is read-only. Excluded files remain untouched and are not renamed, deleted, migrated, or overwritten.
- Filtering the inventory must not activate a pipeline, run Preview/Run, create a layer, or change workspace/input/output routing.

Relevant smoke:

- `wpf_shell_host_recipe_local_validation_set`
- latest-build direct EXE `recipe-manager-tabs` with `PipelineInventory: valid VisionPipeline XML only`

## Recipe-Local Validation Sets

Stable behavior:

- Recipe Manager `Pipeline > Runs` may register named local Validation Sets without adding files to the public/product sample catalogs.
- Schema v1 is stored under `RECIPE\<recipe>\VISION\ValidationSets\validation-sets.xml`; it must not be stored beside pipeline XML files or appear as a pipeline option.
- Each registered image keeps an absolute local path, expected `OK` or `NG`, and optional operator notes. Adding the same path again updates its expectation/notes instead of duplicating the row.
- Folder registration includes supported images from the selected folder's top level only. It assigns one explicit expected `OK` or `NG` role to the batch, ignores unsupported files, does not recurse into subfolders, and rejects a batch that exceeds the validation-set image limit instead of partially registering it.
- Path repair is enabled only for one selected missing image. The operator must select an existing supported replacement image that is not already registered in the set; repair changes only that row's absolute path and preserves its expected `OK`/`NG` role and notes.
- Path repair must not recursively search folders or disks, infer a replacement, merge rows, or rewrite any other path. The suite remains blocked while any registered image is missing.
- Registration, selection, deletion, and missing-file review do not run Preview/Run, create/delete/load layers, open a Tool View, or change workspace/input/output routing. Deleting a set removes metadata only, never the source images.
- Missing images remain visible and disable the explicit suite command. They must not be silently skipped.
- The explicit Local set suite reuses the current selected pipeline, `VisionPipelineSampleCheckService`, batch result rows, run-history storage, NG filtering, failed-step actions, and baseline comparison surfaces. Expected `NG` means the pipeline is expected to reject/fail that image.
- New Local set rows persist outcome schema v1 with execution state, expected `OK`/`NG`, raw Pipeline actual `OK`/`NG`, and judgment correctness as separate fields. The legacy `Success` field remains aggregate validation pass/fail and must not be reinterpreted as the authoritative actual outcome for explicit rows.
- A completed correct reject is actual `NG` with `JudgmentCorrect=true`; it is not an execution error. An execution error publishes no actual outcome and must not be classified as a false accept or false reject.
- Legacy rows without the explicit outcome schema remain readable through their saved `ExpectedActual:` role/text fallback, but must remain distinguishable from explicit rows and are not equivalent qualification evidence.
- An unreadable or unsupported validation-set XML is preserved and blocks mutation; the UI must not overwrite it with an empty document.

Relevant smoke:

- `wpf_shell_host_recipe_local_validation_set`
- latest-build direct EXE `recipe-manager-tabs`

## Run History Batch Analytics

Stable behavior:

- The selected saved batch run derives `failure rate`, average, median, nearest-rank p95, and maximum from its persisted sample rows. No second telemetry file or database is required.
- Correctness and performance remain separate labels: failure rate describes judgement outcomes; elapsed aggregates describe observed sample execution time.
- Non-positive, NaN, or infinite elapsed values are excluded from timing aggregates. They do not remove the row from the correctness denominator.
- Baseline timing comparison requires the same non-empty `SuiteKind`, `SuiteName`, and exact sample-image multiset. Result order does not matter, duplicate images still count, and a saved sample name is used only when neither image path field is available.
- Average and p95 deltas are shown as baseline to current only when every result in both runs has a valid positive elapsed value. A different suite/sample set or incomplete timing set shows an explicit skipped-performance message; it must not be labeled a performance regression.
- Outcome regression rows remain independent from timing compatibility. A selected baseline with the same sample names may still show `Regression`, `Recovered`, or `Still NG` even when its timing comparison is skipped.
- Selecting a run or baseline and calculating analytics is read-only. It must not trigger Preview/Run, load an image, create a layer, or change input/output routing.
- Explicit selected-sample, Good/Bad pair, Catalog, and Local Validation Set suite executions persist one structured Step report per sample and link it through `RunReportPath`. The plain single check remains non-persisting.
- Per-Step timing is available only when every batch row has a readable linked report, report recipe/pipeline identity matches the batch, and Step index/name/tool/enabled/input/output definitions match across all reports.
- Missing paths/files, unreadable reports, identity mismatch, or Step-definition mismatch must show an unavailable reason. Do not mix partial reports into apparently complete Step statistics.
- Enabled Step rows are ordered by descending p95 and expose timing coverage plus average, nearest-rank p95, and maximum. Non-positive, NaN, and infinite Step timings are excluded and remain visible through reduced coverage.
- New saved batch summaries persist their deterministic review-queue policy, canonical SHA-256, selected result indices, and per-row reasons. Selection is derived once at save time so reopening Run History cannot silently change the reviewed population.
- The v2 generic queue contains every explicit execution error, every false accept/false reject when expected roles exist, every missing or unreadable source/report/drawing evidence row, minimum and maximum rows for each varying finite Step metric, and three content-hash-ordered audit rows per declared role stratum (or `ALL`). An invariant metric must not generate fake minimum/maximum rows. Legacy rows retain the previous runtime-failure fallback.
- Older saved summaries without this data must display the queue as unavailable and require a new explicit suite run. They must not recompute a different historical queue or claim equivalent evidence.
- `검토 큐만` is a read-only filter, mutually exclusive with the existing NG/misclassification filter. Selecting a queued row and opening its retained drawing must reuse the current sample-result viewer and must not trigger Preview/Run, create layers, or change routing.

Relevant smoke:

- `wpf_shell_host_recipe_local_validation_set`
- `wpf_shell_host_recipe_run_history_review_queue`
- latest-build direct EXE `recipe-manager-tabs` with linked-report Step aggregation, missing-report rejection, `RunHistoryAnalytics`, and `RunHistoryPerformanceComparison`

## Pipeline Review Input-State Semantics

Stable behavior:

- An enabled Step with no current input image is `입력 없음` only when no earlier enabled Step produces that input layer.
- If an earlier enabled Step produces the input layer, the downstream Step remains `WAIT` until the operator explicitly runs Review.
- Missing-input selection, Step navigation, and status refresh are read-only. They must not trigger Preview/Run, create layers, or change input/output routing.
- Pipeline Review reuses the existing flow document/control and must not introduce partial-run semantics merely to display this state.

Relevant smoke:

- `wpf_shell_host_pipeline_review_input_state`
- `wpf_shell_host_pipeline_review`
- latest-build direct EXE `recipe-pipeline-roundtrip`

## Pipeline Review Selected-Step Edit Handoff

Stable behavior:

- `설정 수정` in Pipeline Review opens the same recipe, pipeline, and 1-based selected Step in Recipe Manager `Advanced > Pipeline > XML/Step`.
- The existing Recipe Manager PropertyGrid is the authoritative selected-Step parameter editor and must be brought into the visible viewport. The separate Tool View remains a detached tool session unless a future explicit apply-back contract is designed and verified.
- Opening the handoff must not run Preview/Run, create/delete/load layers, change the active layer, change workspace or pipeline routing, or create recipe sample evidence.
- Parameter persistence remains an explicit Recipe Manager XML apply action; rerun remains an explicit operator action.
- When the requested pipeline exactly matches a runnable catalog workspace pipeline (`Sample_<catalog sample name>`), the Recipe Manager work sample must align to that same catalog sample before Good/Bad rerun. It must not retain an unrelated prior/default sample.
- A pipeline with no exact runnable catalog match must not change the current work-sample selection merely because selected-Step edit was opened.
- An unsupported PropertyGrid mapping may still navigate to the exact XML/Step detail, but it must show an unavailable edit status rather than silently changing the Step.

Relevant smoke:

- `wpf_shell_host_workspace_sample_fixture_review`
- `wpf_shell_host_pipeline_step_edit_handoff`
- `wpf_shell_host_fixture_step_edit_apply_rerun`
- latest-build direct EXE `recipe-pipeline-roundtrip` with `StepEditHandoff`
- latest-build direct EXE `public-fixture-review` with Fixture `MIN_AREA` XML apply and explicit Fixture Good/Bad rerun

## Tool View N-Image Verification

Stable behavior:

- The common action is visible only for native single-input Tool Views with a
  current one-Step Pipeline adapter. Arithmetic, HSV, Histogram, AutoMPoint, and
  Pipeline-only families do not silently execute a substituted algorithm.
- Selecting files or one top-level folder, clearing rows, selecting a result,
  opening/closing the modal window, and exporting do not run the Tool View
  Preview, create/select/delete a layer, or change input/output routing.
- Explicit N-image Run creates and serializes the current Step exactly once,
  then freezes its SHA-256 and the ordered deduplicated image list.
- The transient Step always executes from isolated `Main` to `NImageResult`.
  Native Tool View grayscale normalization is applied only to the execution
  copy. The saved source snapshot remains the original loaded image and is
  verified by SHA-256.
- Phase 1 executes sequentially. Stop is checked between images and therefore
  means stop after the current image. Do not describe this as parallel
  execution.
- Every completed row retains its source snapshot, result drawing, run report,
  status, message, metrics, and elapsed time. The batch retains XML/TSV summary,
  Pipeline snapshot, and the same deterministic review-queue contract used by
  current batch history.
- The HTML report must use retained summary/report/image evidence only. It must
  not instantiate or rerun the tool, and missing or hash-mismatched evidence is
  labelled explicitly.
- Changing the file list clears the retained session. The modal owner prevents
  editing the underlying Tool View parameters while results are open.
- A successful row means the frozen Step executed and passed only its explicit
  Step acceptance contract. The quick surface does not infer expected OK/NG
  roles, accuracy, semantic correctness, or recipe qualification.
- Formal labelled OK/NG validation and saved recipe history remain Recipe
  Manager Validation Set and Run History responsibilities.
- A completed all-success `Matching`, `EdgeBasedMatching`, or
  `FeatureMatching` Tool View session may be explicitly promoted as a
  hash-locked locator expected-success set. Promotion must preserve the exact
  one-Step Pipeline name/text/SHA-256, dependency/template hashes, ordered
  original-file hashes, and image-set hash. It must not activate the Pipeline,
  start Preview/Run, or mutate layers/routing.
- Every promoted row is `Expected OK` only for locator execution. Source-corpus
  defect OK/NG roles must not be copied or inferred. The retained source hash
  and decoded-pixel identity must validate before the current original bytes
  are locked.
- A hash-locked set is idempotent and read-only at row level. A different
  selected Pipeline is not runnable; Pipeline, dependency, or image hash drift
  fails before image execution. Legacy unlocked Validation Sets keep their
  existing add/remove/repair and OK/NG behavior.
- P234's first real-folder acceptance must remain reproducible: the frozen P230
  Die Pad 1 Step SHA-256
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`
  registers and executes a deterministic 12 OK + 12 NG top-level folder with
  once-only Step creation, 24/24 retained drawings, verified source
  SHA-256/decoded pixels, and `ScoreMax` parity within `0.1` percentage points.
  The role labels balance the integration sample only and must not be presented
  as an OK/NG classification result.

Relevant smoke:

- `wpf_shell_host_edge_based_matching_tool`
- `wpf_tool_n_image_verification_window`
- `wpf_tool_n_image_locator_promotion_window`
- `p235_locator_validation_promotion`
- `wpf_tool_n_image_entry_side_effect_contract`
- `--tool-n-image-verification-contract` in `VisionRecipeRunnerSmoke`
- `--tool-n-image-real-folder-acceptance` in `VisionRecipeRunnerSmoke`, with
  the frozen P230 dataset/template/baseline arguments recorded in
  `artifacts\p234_tool_n_image_real_folder_acceptance_20260724`

## Qualified Recipe Snapshot Core And Run History UI

Stable behavior:

- A qualified object is a content-addressed archive under
  `QUALIFIED_RECIPE\<SnapshotId>`, never the mutable Recipe folder itself.
- Qualification requires one completed schema-v2 `LocalValidationSet` batch
  whose ordered rows, explicit outcomes, source hashes, per-row Pipeline/source
  snapshots, drawings, review queue, and frozen Validation Set identity all
  match.
- `InspectionJudgment` requires both expected OK and NG evidence.
  `LocatorStability` accepts expected-OK locator evidence only. Neither scope
  may be shortened to production or field qualification.
- Creation writes and fully verifies a `.creating-*` sibling before atomic
  rename. A temporary or interrupted directory must never appear in the
  qualified list.
- The same immutable identity is idempotent and reuses the same verified
  Snapshot ID. Creation UTC alone must not produce a duplicate.
- `inventory.sha256` covers all copied payload files. The manifest binds its
  hash, and the canonical manifest identity binds the payload to the Snapshot
  directory name.
- Verification distinguishes intact payload from current-runtime fingerprint
  match, but the combined qualification verification fails closed for either
  mismatch and reports the exact reason.
- Product APIs never edit or delete a qualified payload. Supersede/revoke are
  create-once external lifecycle event files with a required reason; supersede
  also requires a different verified successor.
- Recipe rename/delete cannot remove or invalidate the self-contained archive.
- This core contract has no Preview/Run, layer, active-layer, or route mutation.
  The Run History UI adapter preserves those invariants.
- The panel consumes one selected completed `LocalValidationSet` history item.
  A pending selected-Step edit, mismatched set/Pipeline, incomplete evidence,
  missing operator note, or exact preflight error disables or blocks creation.
- An unlocked manual set is frozen into the Snapshot request using its current
  ordered file hashes and selected Pipeline definition; this does not mutate or
  relabel the source set. An already hash-locked set must match its locked
  Pipeline identity.
- `Open evidence` is read-only. `Working copy` creates a new Recipe, restores
  the Pipeline and archived dependencies, and never inherits qualification,
  lifecycle, Run History, or Validation Set status.
- Supersede/revoke require a non-empty reason and explicit confirmation.
  Cancellation changes nothing.

Relevant smoke:

- `tools\QualifiedRecipeSnapshotSmoke`
- `wpf_shell_host_recipe_qualified_snapshot`
- retained evidence:
  `artifacts\qualified_recipe_snapshot_core_20260727\final` and
  `artifacts\qualified_recipe_snapshot_ui_20260727`

## EdgeBased Matcher Retained-Run Diagnostics

Stable behavior:

- A completed EdgeBasedMatching Step may expose one read-only
  `Matcher Diagnostics` tab from the already retained explicit Run.
- Opening the tab, selecting its rows, or reviewing its images must not execute
  Preview/Run, create or select a layer, change the active layer, or mutate
  input/output routes.
- Library-Noah owns the runtime evidence: exact trained model points/model
  center, search ROI, retained primary hypothesis, strongest spatially
  distinct alternative when one exists, candidate score/pose/bounds,
  model/pyramid/candidate/uniqueness metrics, and exact decision state/reason.
- OpenVisionLab owns presentation and a stable evidence ID. It must clone
  retained evidence rather than query or rerun the matcher.
- `Success` labels an accepted result as `Selected`. `NoMatch` labels any
  retained below-gate primary as `Best observed (below gate)`. `Ambiguous`
  labels the rejected primary as `Rejected primary hypothesis`.
- A spatial alternative may legitimately be absent. The UI reports
  `None retained` and never synthesizes an alternative.
- Model-pyramid usability estimates remain distinct from the actual existing
  runtime coarse proposal scale and proposal/verification/acceptance/fallback
  counters.
- Diagnostics never lower a score or margin gate, change defaults or candidate
  ordering, auto-select a template/pattern, change XML/PropertyGrid/report
  contracts, or turn risk metrics into acceptance.
- Exact `MatchingNoResult` and `MatchingAmbiguous` errors and reasons remain
  visible even though the Step failed.

Relevant smoke:

- `cvr06_matcher_diagnostic`
- `wpf_shell_host_workspace_sample_pipeline_review_edge_ng_metrics`
- `wpf_shell_host_edge_based_matching_tool`
- retained evidence:
  `artifacts\cvr06_matcher_diagnostic_20260728`
- completion report:
  `docs\reports\OPENVISIONLAB_MATCHER_DIAGNOSTIC_SURFACE_20260728.md`

## Threshold Basic Retained-Preview Teaching Suggestion

Stable behavior:

- A Threshold Tool View may analyze only the retained full-image 256-bin Gray
  histogram from the current explicit Basic Preview. Analysis must not execute
  Preview/Run, create or select a layer, change the active layer, or mutate
  input/output routes.
- The v1 contract applies only to Basic `Binary` and `BinaryInv`. `Binary`
  proposes a bright-object cutoff between the selected high-gray significant
  mode and its lower neighbor; `BinaryInv` mirrors that policy for a dark
  object. Range, Adaptive, ROI, Line, and Circle suggestions are not implied.
- An accepted proposal shows the exact candidate marker, selected mode pair,
  separation, class populations, source/region provenance, and a stable
  evidence ID. A single-mode histogram, invalid evidence, or an undersized
  class rejects the proposal and leaves manual teaching unchanged.
- Analysis and candidate selection are advisory. Only explicit `Use T` may
  write the Threshold teaching value. It follows the existing debounced Preview
  policy; it must not silently change a gate, Pipeline, layer, or route.
- The immediately replaced same-source teaching value remains recoverable
  through explicit `Undo`. Source/evidence drift, a later unrelated teaching
  edit, or a different applied value invalidates that recovery instead of
  restoring stale state.
- The known public regression remains Good `ResultCount=4` and Bad
  `ResultCount=1` with the retained corrected candidate `T=138`. The rejected
  first global Otsu candidate `T=73` returning `0/0` remains genuine failure
  evidence and must not be rewritten as success.
- This contract does not authorize automatic apply, automatic acceptance-gate
  changes, generic easyTouch behavior, a new inspection algorithm, or
  additional suggestion families without their own trigger and verification.

Relevant smoke and evidence:

- `cvr07_threshold_suggestion`
- `wpf_shell_host_threshold_basic_tool`
- `wpf_shell_host_threshold_tool`
- `wpf_threshold_signal_good_bad_replay`
- retained evidence:
  `artifacts\cvr07_threshold_suggestion_20260728`
- completion report:
  `docs\reports\OPENVISIONLAB_THRESHOLD_TEACHING_SUGGESTION_20260728.md`

## LineFixture Typed Dual-Datum Producer

Stable behavior:

- `LineFixture` and alias `DualEdgeFixture` consume two distinct exact typed
  `Segment` results from earlier enabled, successful, accepted
  `Line`/`LineGauge` Steps. They must not run a duplicate edge detector or
  resolve geometry by display label alone.
- Polarity and contrast remain owned and judged by each earlier Line Step;
  LineFixture can consume the Segment only after that execution and acceptance
  passes.
- Both sources must share the consumer's coordinate layer and image size.
  Missing, failed, rejected, ambiguous, wrong-kind, cross-frame, non-finite,
  degenerate, or out-of-image sources fail closed.
- The infinite-line intersection is the fixture origin. Datum A defines the
  undirected X axis nearest the taught reference direction. The image-Y-down
  Line angle must be converted into the existing OpenCV positive
  counter-clockwise Fixture/`NormalizeImage` convention.
- v1 scale remains exactly one. The producer must not infer scale, perspective,
  calibration, or homography from two lines.
- Support, per-line fit residual, included angle, per-line intersection
  extension, reference pose, and in-image gates fail closed with exact reasons.
  Runtime datum/geometry-gate rejects retain available datum drawings and
  metrics but publish no usable Fixture frame. Definition failures do not
  execute and therefore create no current-run drawing.
- Successful current-run evidence retains Datum A/B segments, intersection,
  Fixture X/Y axes, source quality metrics, and typed `Origin/Point`.
- The existing Fixture frame owner remains responsible for publication,
  duplicate-name rejection, angle/scale policy, `NormalizeImage`, and relative
  ROI consumption.
- Selected-Step PropertyGrid load/edit/apply/save must preserve typed source
  identities and datum gates without Preview/Run, layer creation/selection, or
  route mutation.
- The frozen eight-case synthetic matrix must remain replayable. It is
  integration evidence only; physical-task qualification still requires the
  named operator/data packet and reviewed N-sample rail/reflection evidence in
  the dedicated contract.

Relevant smoke and evidence:

- `OpenVisionFixtureSmoke --cvr09-line-fixture`;
- `cvr09_line_fixture_property_grid`;
- retained evidence:
  `artifacts\cvr09_line_fixture_20260728_r11`;
- contract:
  `docs\contracts\openvisionlab\OPENVISIONLAB_LINE_FIXTURE_V1_CONTRACT.md`;
- completion report:
  `docs\reports\OPENVISIONLAB_CVR09_LINE_FIXTURE_20260728.md`.

## MultiMatchMean Bounded Multi-Instance Consumer

Stable behavior:

- `MultiMatchMean` and alias `MultiFixtureMean` consume one exact earlier
  successful and accepted multi-result Matching/EdgeBasedMatching Step in the
  same coordinate layer and image size. The source must request
  `NUM_MATCH >= 2`.
- Runtime retains finite source score, center, bounds, angle, and positive
  scale evidence. Missing, ambiguous, later, rejected, wrong-family,
  cross-frame, empty, or invalid sources fail closed.
- Stable same-run instance IDs use row-major ordering with
  `ROW_TOLERANCE_PX`: rows top to bottom and instances left to right. IDs are
  review identities, not cross-image physical serial numbers.
- Count limits and pairwise source bounding-box IoU are checked before
  fan-out. `MAX_INSTANCES` must not exceed 64.
- Each retained instance reuses the existing `NormalizeImage` transform and
  existing `Mean` Tool for one fixed reference-coordinate `RELATIVE_ROI`.
  Individual angle, scale, valid-pixel, and Mean failures retain an exact
  reason and do not prevent the remaining instances from being inspected.
- Current-run drawings retain every transformed ROI with `Ixx`, OK/NG, and
  finite Mean. Green means accepted and red means rejected.
- `REQUIRE_ALL=true` requires zero individual failures.
  `REQUIRE_ALL=false` requires `MIN_PASS_COUNT`. The Pipeline definition must
  gate `InstanceAggregatePassed` with exact `1..1` so individual rows and
  drawings survive an aggregate NG.
- Selected-Step PropertyGrid and XML preserve the typed source, reference
  pose/image size, fixed ROI, count/overlap, pose/Mean, and aggregate settings
  without Preview/Run, layer, or route side effects.
- Pipeline Review `Instance Results` row selection highlights the same
  transformed ROI without another Run. Direct and recipe Run Reports preserve
  the ordered rows and exact reject reasons.
- The frozen synthetic matrix and current-source UI capture remain bounded
  evidence for one fixed Mean sub-inspection. They do not authorize a generic
  graph engine, arbitrary nested sub-recipe, calibrated measurement,
  cross-image tracking, another fan-out tool family, or field qualification.

Relevant smoke and evidence:

- `OpenVisionFixtureSmoke --cvr10-multi-match-mean`;
- `cvr10_multi_match_mean_review`;
- retained evidence:
  `artifacts\cvr10_multi_match_mean_20260728_r6`;
- contract:
  `docs\contracts\openvisionlab\OPENVISIONLAB_MULTI_MATCH_MEAN_V1_CONTRACT.md`;
- completion report:
  `docs\reports\OPENVISIONLAB_CVR10_MULTI_MATCH_MEAN_20260728.md`.

## Validation Variant v1 Contract

CVR-19 is stable at the bounded image-level Variant scope:

- one unchanged recipe/Pipeline executes every row;
- each validation image may own one named Variant and one expected metric range;
- selection restores visible/editable values; Apply and Reset are explicit;
- setup edits do not Preview/Run or mutate layers, workspace, or routes;
- batch history, Variant+role review queue, comparison identity, and Qualified
  Snapshot retain and reverify the contract;
- missing fields remain legacy Default/no-gate behavior; invalid ranges fail
  closed.

Relevant smoke and evidence:

- `wpf_shell_host_recipe_local_validation_set`;
- `QualifiedRecipeSnapshotSmoke`;
- `artifacts\cvr19_validation_variants_20260729`;
- `docs\contracts\openvisionlab\OPENVISIONLAB_VALIDATION_VARIANT_V1_CONTRACT.md`.

## Overlay Rendering v1 Contract

CVR-20 is stable inside the existing `OverlayMerge` Step:

- missing new keys preserve the legacy palette, marker sizes, and
  `DrawLabels` behavior;
- Recipe Manager keeps source/output and display-only settings in one
  PropertyGrid and restores them from recipe XML;
- `LegacyDefault`, `HighContrast`, and `ColorBlindSafe`, label mode, bounded
  line/point size, label backing, and margin affect only burned-in pixels;
- the explicit `Display defaults` action requires a separate `Apply to XML`;
- edits, apply, reset, and reopen do not Preview/Run or mutate layers,
  active layer, or routes;
- metrics, returned overlays, acceptance, and Pipeline outcome remain
  unchanged;
- Run Reports and Pipeline snapshots retain the rendering parameters.

Relevant smoke and evidence:

- `cvr20_overlay_rendering`;
- `wpf_shell_host_pipeline_review`;
- `wpf_shell_host_recipe_manager_summary`;
- `artifacts\cvr20_overlay_rendering_20260729`;
- `docs\contracts\openvisionlab\OPENVISIONLAB_OVERLAY_RENDERING_V1_CONTRACT.md`.

## Before Touching Stable Paths

1. Identify which contract above is affected.
2. Keep the change scoped to the smallest shared controller/runtime/presenter that owns the behavior.
3. Run the listed focused smoke target or explain why it could not be run.
4. In the final report, state whether the stable contract still passes.

Example focused commands:

```powershell
dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build .\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --target wpf_shell_host_blob_tool .\.codex\smoke-output\stable-blob
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --target wpf_shell_host_workspace_image_load .\.codex\smoke-output\stable-workspace-load
Start-Process -FilePath .\bin\x64\Debug\OpenVisionLab.exe -WorkingDirectory . -ArgumentList @('--smoke','line-pins-measure','--output','.\.codex\smoke-output\actual-exe-line-pins-measure') -Wait
```

## Completion Rule

A restored feature becomes "do not touch casually" only when:
- the user confirms the behavior looks restored, and
- a focused smoke target exists or is added, and
- the smoke passes after the final code change.

If all three are true, future work should preserve the behavior and avoid opportunistic redesign.
