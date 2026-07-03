# OpenVisionLab WPF Migration Plan

Updated: 2026-06-21

## Direction

OpenVisionLab is now a WPF-only application at the active app/UI project layer:

- `OpenVisionLab.csproj` targets `net8.0-windows` with `UseWPF=true` and `UseWindowsForms=false`.
- `OpenVisionLab.ImageSpace.Core`, `OpenVisionLab.Logging.Controls`, `OpenVisionLab.Pipeline.Controls`, and `WpfPropertyGridBridge` are the active WPF-side UI investments.
- The legacy WinForms UI projects and wrappers have been removed from the active solution path, including `RJControls`, `OpenVisionLab.MessageBox`, `OpenVisionLab.Controls.Init`, `OpenVisionLab.ImageCanvas`, and old `FormVision_*` surfaces.
- The default shell, layer workspace, ROI editor, native preprocessing tools, bottom log panel, and standalone Image Compare are WPF.

The product direction remains a modern, user-friendly rule-based vision workbench. WPF is now the default execution and validation path; remaining work is filling the incomplete algorithm tool views without regressing the pipeline/runtime contracts.

Near-term non-goals:

- Replacing the frozen PropertyGrid editor itself during unrelated WPF migration work.
- Changing runtime contracts just to satisfy UI migration.
- Reintroducing WinForms UI wrappers as a shortcut for unfinished algorithm surfaces.

## Migration Rules

1. Do not redesign the frozen PropertyGrid UX unless the user explicitly reopens it or a regression is found.
2. Keep vision tool execution, pipeline step mapping, XML serialization, and runner contracts UI-neutral.
3. A WPF tool view must use the same runtime services as the current forms:
   - `IDisplayManager`
   - layer image access
   - `RunVisionStep` equivalent lifecycle logging
   - pipeline step creation
   - result publish behavior
4. Preview must stay separate from publish-to-workspace behavior.
5. Layer input preview must refresh from the latest layer image before running a tool.
6. Each migrated surface needs a focused screenshot smoke or interaction smoke before it is considered done.
7. Use `WPF UI` as the WPF theme baseline, with OpenVisionLab compact color/spacing overrides for inspection-tool density.
8. Keep the application shell WPF-only; add missing tool views as WPF documents/windows instead of restoring WinForms hosts.
9. Treat the current `WpfPropertyGridBridge`/PropertyGrid UX as frozen unless the user explicitly asks for a PropertyGrid change.
   - Do not replace PropertyGrid-driven parameter panels with custom WPF parameter controls.
   - Do not alter PropertyGrid row visibility, editors, threshold affordances, or stable behavior during unrelated WPF migration work.
   - PropertyGrid-driven tools can be surrounded by WPF shell/preview/panel work, but the PropertyGrid itself remains the user-facing editor.
10. Scope `WPF UI` theme dictionaries to WPF shell/tool views, not global application resources.
   - `OpenVisionLabWpfTheme.xaml` may be merged by WPF views that intentionally opt in.
   - Do not merge WPF UI resources into `Application.Resources` or the PropertyGrid bridge.
   - PropertyGrid styling remains owned by `WpfPropertyGridBridge` and its existing resource path.
11. Every new WPF `View` should have a corresponding `ViewModel` class with the `ViewModel` suffix.
   - Prefer `Wpf/Views`, `Wpf/ViewModels`, and `Wpf/Documents` folders for new shell/document surfaces.
   - Keep host adapters in `Documents`; keep bindable state in `ViewModels`; keep view code-behind thin.

## Recommended Architecture

```text
Current WPF shell
  -> WPF floating tool/document windows
    -> native WPF preprocessing views
    -> WPF pending view for unfinished algorithm tools
    -> WPF Pipeline Review
    -> WPF ROI editor / Image Compare
      -> UI-neutral runtime services
      -> VisionPipelineExecutionService / RunVisionStep-compatible flow
      -> IDisplayManager / Pipeline step builder

Next architecture target
  -> WPF algorithm review/edit views for Blob, Contour, Line, Matching, FeatureMatching
    -> preserve PropertyGrid behavior where still required
    -> add clearer image, ROI, metric, and overlay review around it
    -> keep XML/pipeline/runtime contracts unchanged
```

## First Pilot

Use `Filter` as the first WPF preprocessing pilot.

Reasons:

- It has the standard preprocessing shape: input layer, output layer, parameters, preview, add-to-pipeline.
- It has several parameter modes, but no special ROI editor, template editor, or detection review panel.
- It is currently a good candidate for improvement, but a WinForms redesign would be throwaway work.

The pilot should prove:

- Input/output layer selectors bind to the same layer list as WinForms.
- The input preview refreshes when the backing layer changes.
- `Run Preview` updates only the output preview/layer according to the chosen action.
- `Add Pipeline` creates the same `Filter` step as the current form.
- Screenshot smoke can open the WPF-hosted pilot and validate key controls.

Current WPF UI status on 2026-06-21:

- The app starts directly in `OpenVisionShellHostWindow`.
- Native WPF floating tool windows are used for Filter, Morphology, EdgeDetection, Rotate/Scale, Mean, Arithmetic, HSV, Histogram, Blob, Contour, Line, Matching, and FeatureMatching.
- The central WPF image workspace remains visible while tool windows are opened separately.
- The bottom shell area remains the real OpenVisionLab `LogPanelView`.
- No active algorithm menu item falls back to old WinForms forms; the generic pending surface remains only as a reusable fallback contract.
- Blob now opens a native WPF tool window that preserves the existing PropertyGrid behavior, runs preview, publishes `Blob_Preview`, and creates a valid `Blob` pipeline step.
- Contour now opens a native WPF tool window that preserves the existing PropertyGrid behavior, runs preview, publishes `Contour_Preview`, and creates a valid `Contour` pipeline step.
- Line now opens a native WPF tool window that preserves the existing PropertyGrid behavior, initializes runnable ROIs for `Line A` and `Line B`, defaults Line A's measurement-line direction toward Line B for A-to-B distance checks, offers an `Edge / Measure / Intersection` purpose selector, switches the PropertyGrid between `Line A` and `Line B`, provides a selected-line ROI edit button backed by the WPF ROI editor, publishes `Line_Preview`, runs Edge as `LineGauge`, Measure as `LineDistance`, and Intersection as `LineIntersection`, shows purpose-specific edge/distance/cross result-review text, uses separate Measure length and Intersection cross-point smoke samples for operator clarity, clears stale review text on ROI/parameter/layer changes, and creates valid pipeline steps with paired-line metadata.
- Matching now opens a native WPF tool window that preserves the existing PropertyGrid behavior, shows template-load state, defaults to original/full-image matching, runs preview with a match-box overlay, publishes `Matching_Preview`, and creates a valid `Matching` pipeline step.
- FeatureMatching now opens a native WPF tool window that preserves the existing PropertyGrid behavior, shows template-load state, defaults to original/full-image feature matching, runs preview with a SIFT match-box overlay, publishes `FeatureMatching_Preview`, and creates a valid `FeatureMatching` pipeline step.
- Focused smoke validates WPF shell load, output-layer switching, native tool windows, pending tool copy, ROI editor, Image Compare, log panel, and localization catalog.
- The WPF Tool View theme baseline is `WPF UI` plus `VisionToolWpfTheme.xaml` compact overrides.
- The WPF Tool View theme now treats common controls as first-class migration surfaces: layer preview frames, action buttons, compact combo boxes, icon buttons, and sliders are styled centrally instead of inheriting default desktop control chrome.
- `VisionToolWpfTheme.xaml` now gives combo boxes a focused/dropdown state, keeps slider thumbs inside the track at min/max, and the WPF tool views use an `ImagePlusOutline` output-layer action icon so layer creation reads as image/layer output creation rather than an ambiguous generic plus.
- WPF surfaces now use a shared neutral/teal visual language: teal indicates flow, selected state, primary execution, and active WPF shell surfaces; success/error/warning status text is color coded by the common WPF status presenter.
- Filter and Morphology now use the same compact tool-title header pattern as the shared preprocessing and Arithmetic WPF views, and screenshot smoke verifies the localized header text.
- The legacy typo form `FormVision_EdgeDection` is explicitly excluded from compilation.
- `ISingleInputVisionToolWpfView` now captures the shared single-input Tool View host contract for layer selection, input/output previews, output layer creation, preview run, and add-to-pipeline actions.
- `VisionTestForm` owns the common single-input WPF host wiring for layer selection activation, layer-list synchronization, and preview image refresh, reducing repeated WinForms wrapper synchronization across Filter, Morphology, EdgeDetection, Histogram, HSV, Mean, and RotateAndScale.
- `IArithmeticVisionToolWpfView` now captures the separate multi-input Arithmetic Tool View contract for input A/B layers, output layer, previews, operation/constant/offset parameters, preview run, and offset run.
- `VisionTestForm` owns the Arithmetic WPF host wiring for input A/B/output layer activation, output layer creation, layer-list synchronization, and preview image refresh without forcing Arithmetic into the single-input contract.
- `Arithmetic` now has a first-class pipeline step contract for `InputLayerB`, image/constant source modes, gray/color constants, offset mode metadata, validation, runtime execution, and XML round-trip.
- `ArithmeticToolWpfView` now exposes an explicit segmented `Operation / Offset` mode selector. `Add Pipeline` preserves the selected mode, and the view hides unrelated Input B, constant, operation-type, or offset sections by mode so the saved recipe matches what the operator previewed.

Current WPF Tool Views:

- `FilterToolWpfView`
- `MorphologyToolWpfView`
- `BlobToolWpfView`
- `ContourToolWpfView`
- `LineToolWpfView`
- `MatchingToolWpfView`
- `FeatureMatchingToolWpfView`
- `ArithmeticToolWpfView`
- `SimplePreprocessToolWpfView` for EdgeDetection, Histogram, HSV, and RotateAndScale
- `SimplePreprocessToolWpfView` for Mean
- `RoiEditorWindow` / `RoiEditorViewModel` for PropertyGrid ROI, multi ROI, and pattern TRAIN selection
- `ImageCompareWindow` / `ImageCompareViewModel` for standalone multi-image comparison

Remaining WPF algorithm surface candidates:

- None in the active algorithm menu for first-pass WPF preview/add-pipeline coverage.

Known gaps:

- ROI editing is now WPF-native at the PropertyGrid editor entry point, and the old ImageCanvas/WinForms ROI editor path has been removed.
- The migrated views no longer rely on WinForms form wrappers in the active app path. Unfinished algorithm tools must receive native WPF surfaces rather than fallback forms.
- UX correction: the default WPF shell should keep the main image workspace central, keep the bottom area as the real `LogPanelView`, and open preprocessing/algorithm views as separate tool windows. Do not dock large Tool Views into the bottom shell area.
- `Histogram` and `HSV` are currently form-only/demo-style tools and keep `Add Pipeline` hidden; the preprocessing smoke suite now asserts that visibility contract.
- `Mean`, `EdgeDetection`, `RotateAndScale`, `Filter`, `Morphology`, and `Arithmetic` expose `Add Pipeline`; the preprocessing smoke suite now asserts that visibility contract.
- `wpf_tool_add_pipeline_parity_check` clicks the WPF `Add Pipeline` action for Filter, Morphology, EdgeDetection, RotateAndScale, Mean, and Arithmetic, then validates saved Step metadata, key parameters, and XML round-trip.
- `pipeline_arithmetic_multi_input_check` validates Arithmetic runtime execution for image B input, constant input, offset-mode persistence/XML round-trip, and actionable offset failure diagnostics in the same recipe.
- WPF Tool View common headers and action buttons now use `OpenVisionLanguageService` for input/output layer captions, `Parameters`, `Add Pipeline`, and `Run Preview`; the preprocessing smoke suite asserts the localized text for these shared labels.
- WPF-hosted tool captions, Filter/Morphology titles, `SimplePreprocessToolWpfView` titles, and `ArithmeticToolWpfView` shared headers/actions/section labels reuse `OpenVisionLanguageService`; the preprocessing smoke suite now asserts English/Korean title switching.
- The preprocessing smoke suite also asserts localized SimplePreprocess parameter labels for EdgeDetection, Rotate/Scale, Histogram, HSV, and Mean.
- WPF preview frames and output-layer create buttons expose localized tooltips, and the preprocessing smoke suite asserts the shared tooltip contract for Filter, Morphology, Arithmetic, and SimplePreprocess views.
- The preprocessing smoke suite now also asserts that output-layer create buttons use the `ImagePlusOutline` Material icon, preventing regressions back to ambiguous generic add icons.
- HSV now groups its six threshold sliders into Hue/Saturation/Value range sections. The existing `HueMin`/`HueMax`/`SatMin`/`SatMax`/`ValMin`/`ValMax` runtime keys remain unchanged, while the first screen shows all three ranges without a scroll bar.
- EdgeDetection WPF now hides unrelated parameter rows when the operator switches between Canny, Sobel, Scharr, and Laplacian. Hidden values are still preserved for pipeline/runtime compatibility, and smoke coverage checks the Canny/Sobel visibility contract.
- Filter WPF now hides unrelated kernel rows when switching between Blur/Gaussian/Box, Median, and Bilateral modes. Preset buttons remain available where they affect the active kernel value, and smoke coverage checks Blur/Median/Bilateral visibility.
- Histogram WPF now hides unrelated rows by mode: CLAHE shows Clip/Tile, Normalize shows Alpha/Beta, and equalizeHist shows no extra parameters. Smoke coverage checks the mode visibility transitions.
- Morphology WPF operation and kernel-shape controls now separate display text from internal values through `Tag`, so localized labels do not change saved `Operator`/`Shape` XML values.
- Arithmetic WPF mode/radio labels, constant labels, source summary text, and offset summary text now refresh on language changes while preserving the existing operation names and pipeline values.
- Arithmetic WPF now has mode-specific visibility coverage: operation mode shows Input B/source controls, constant mode shows constant values and hides the Input B preview, and offset mode hides unrelated operation/source controls while showing only offset parameters.
- Arithmetic WPF now shows only the relevant execution action per mode: operation mode exposes `Run Preview`, while offset mode exposes the primary `Run Offset` action and hides the redundant preview button.
- WPF Tool View status text now localizes common display prefixes such as preview success, offset success, and pipeline-added notifications while leaving raw runner/log phrases unchanged.
- `SimplePreprocessToolWpfView` now stores the raw summary text and re-renders common summary tokens on language changes, so Mean, EdgeDetection, Histogram, and Rotate/Scale summaries do not stay stuck in the previous language.
- Clickable preview images and output-layer create buttons now carry localized WPF Automation names in addition to tooltips, giving keyboard/automation paths the same semantic labels as the visible UI.
- WPF Shell Preview sample layer/result and pipeline state rows now use localized status text, so the shell language switch no longer leaves `display`, `none`, `Passed`, or `Needs Preview` in Korean preview mode.
- WPF Shell Preview layer/result rows are now selectable. Selecting a row updates the main canvas layer label, status bar, and top layer combo; choosing a layer from the combo updates the same canvas state, and the selected row receives a stronger teal background/border so the shell preserves the expected "layer/result click changes main view" contract.
- WPF Shell Preview combo boxes now use the same compact dark-field styling as the shell chrome. The language combo explicitly displays `OpenVisionLanguageOption.DisplayName`, and screenshot smoke validates that contract.
- WPF Shell Preview input/output preview cards now use the same dark panel tone as the shell workspace, while preserving black image-preview wells and readable preview titles.
- WPF Shell navigation badges now use compact localized user-facing labels (`도구`/`Tool`, `속성`/`PG`, `흐름`/`Flow`) so PropertyGrid-preserved tools are clearly marked without exposing implementation labels in the UI.
- WPF Shell Preview document tabs now bind to localized shell text, including the Pipeline tab, instead of leaving fixed English labels in Korean mode.
- The actual `FormTeachingVision` layer/result rail now follows the same user contract: selecting a result row updates the active display layer, the selected input layer, the top layer combo, and toolbar state so visible selection and next-run input do not drift apart.
- Simple preprocessing sliders now show the active numeric value with visible min/max context, so operators can tune ranges without guessing the allowed domain.
- WPF Tool View checkboxes now use the shared teal/neutral template instead of the default desktop checkbox, keeping compact option rows such as `W=H` visually aligned with combo boxes, segmented buttons, and sliders.
- Tool views now show an input-to-output flow rail and a primary `Run Preview` action, making the expected work sequence clearer without adding explanatory copy.
- The WPF shell preview palette has been aligned with the Tool View palette through shared `OpenVisionLabWpfTheme.xaml` tokens.
- `PipelineFlowView` now shares the neutral/teal state palette for step cards, selected preview modes, layer pills, and loaded/passed status marks.
- `LogPanelView` filter combo boxes now use a dark WPF template that matches the log surface instead of the default desktop combo chrome, and the log-panel smoke contract asserts the dark combo foreground/background.
- `FormVisionPipelineLlmRecipe` keeps the toolbar image-status label within the available width, exposes the full status through tooltip/accessibility text, and the AI Recipe smoke checks that full-text contract.
- WPF Shell Preview and Filter/Morphology WPF parameter labels now participate in the language switch instead of leaving fixed English labels in Korean UI.
- `SimplePreprocessToolWpfView` now accepts localized parameter label keys, so EdgeDetection, Rotate/Scale, Histogram, HSV, and Mean can switch their WPF labels without rebuilding each view.
- `Mean` is WPF-hosted and supports `Add Pipeline`, but still keeps the hidden legacy PropertyGrid path for compatibility.
- Matching and FeatureMatching should not receive custom replacement parameter panels while the PropertyGrid freeze is active.
- Blob, Contour, Line, Matching, and FeatureMatching first-pass WPF preview/add-pipeline are complete; they still need richer metrics/overlay/measurement review behavior around the existing PropertyGrid before any full Tool View replacement can be considered.
- Main shell startup is WPF-only. Unsupported future tool documents can still use the generic pending view until their native WPF surfaces are implemented.
- The WPF shell tool-window strategy is proven for native preprocessing tools; the next work is completing algorithm-specific WPF views while preserving the central image workspace and bottom log.

Shell foundation status on 2026-06-20:

- `OpenVisionShellPreviewView` provides a non-invasive WPF shell preview for the future full-app shell.
- The preview covers title chrome, tool navigation, document tabs, workspace, preview panels, result/pipeline/log rail, and status bar.
- Shell navigation is ViewModel-driven through `OpenVisionShellPreviewViewModel`.
- Shell navigation items are clickable through `SelectToolCommand`; the active tool, preview title, direct-result label, route text, and status bar update from the same selected command.
- The shell language combo uses `OpenVisionLanguageService` options and refreshes the WPF command catalog while preserving the selected tool.
- Shell combo styling was checked in screenshot smoke after rebuilding the smoke project output; the captured language selector is readable in the dark title chrome.
- `OpenVisionShellCommandCatalog` maps the existing `VISION_MENU` command set into WPF navigation groups.
- Navigation badges distinguish active WPF Tool Views from PropertyGrid-preserved tools and pipeline surfaces.
- `OpenVisionToolWindowFactory` centralizes `VISION_MENU` -> WinForms tool window creation so the future WPF shell can reuse the same command routing.
- `OpenVisionShellHostView` / `OpenVisionShellHostWindow` are now the default application shell.
- The old WinForms main-frame fallback has been retired from the active app path; the shell is WPF-first.
- The default shell presents itself as `OpenVisionLab` in visible UI. Internal migration terms such as `WPF Host`, `Bridge`, and `Native WPF` should stay out of operator-facing labels.
- The host reuses the WPF command catalog, tool-window factory, `ApplicationRuntimeContext`, `DisplayManagerService`, and WPF `ImageSpace` workspace. This proves the WPF shell can route commands, seed/select layers, and keep image display UI-neutral.
- `Filter`, `Morphology`, `EdgeDetection`, `Rotate/Scale`, `Mean`, `Arithmetic`, `HSV`, and `Histogram` now open from the WPF host as native WPF floating tool windows, without embedding their WinForms tool forms. The shell itself keeps the central image viewer and bottom `LogPanelView` visible. Their native document adapter wires layer selection, preview image refresh, output-layer creation, preview execution, result-layer publishing, and pipeline-step append where the tool supports pipeline append. Mean additionally renders its measurement overlay into the preview result. HSV keeps source color channels for HSV masking; the other simple preprocessing tools follow the existing single-channel preview convention.
- The host layer/result rail now reads from the actual `IDisplayManager` layer set, so native preview outputs such as `EdgeDetection_Preview`, `RotateScale_Preview`, `Mean_Preview`, `Arithmetic_Preview`, `HSV_Preview`, `Histogram_Preview`, and `Morphology_Preview` appear in the right rail immediately after execution instead of showing static sample rows. The rail scrolls to the latest result when many preview layers exist.
- The host layer/result rail now has a WPF selected-layer detail surface: selecting a row activates that DisplayManager layer, keeps the selected row visually marked, and shows a thumbnail, image size, latest tack time, and display state.
- `Pipeline` now opens a native WPF Pipeline Review floating window from the default shell. `OpenVisionPipelineReviewView` lives under `Wpf/Views`, its bindable state is `OpenVisionPipelineReviewViewModel` under `Wpf/ViewModels`, and `OpenVisionPipelineReviewDocument` under `Wpf/Documents` loads the active recipe pipeline and maps it to the shared `PipelineFlowView`.
- The Pipeline Review document shows Step Flow, selected Step/tool/state, input/output preview images, branch relation, parameter summary, validation status/detail, selected-step run result, and run-log context. It also has an explicit `Run Review` action that executes into an internal review cache instead of publishing directly into the main workspace.
- The active app path is WPF-only. Native preprocessing tools use WPF tool-window adapters, and unfinished algorithm tools open a neutral pending surface until their WPF review/edit views are implemented.
- Screenshot smoke target `wpf_shell_preview` validates shell load, navigation group count, command count, command status, navigation selection behavior, layer/result button command routing, language switching, workspace sizing, rail sizing, and key visible text.
- Screenshot smoke target `wpf_shell_host_bridge` validates the default WPF shell: WPF shell load, Main layer seeding, native tool-window switching and preview output layers, right-rail layer refresh/scrolling, selected-layer thumbnail/detail population, layer/result click activation, native Pipeline Review floating-window loading, review execution, selected-step result summary, input/output preview binding, and one active tool window at a time.
- Screenshot smoke target `wpf_shell_host_pending_tool` validates the WPF pending surface for tools that are not yet native WPF.
- Legacy `main_frame_*` and `main_workspace` WinForms smoke targets are retired from the WPF-first validation path.
- Contract smoke target `tool_window_factory_contract_check` validates every `VISION_MENU` value against the expected tool form and asserts that only `Pipeline` requires workspace refresh.
- `tools/RunUiPrecheck.ps1 -WpfTools` now appends the WPF shell preview, WPF Tool View screenshots, the PropertyGrid-preserved floating tool route, WPF Add Pipeline parity, tool-window factory, and localization catalog checks to the default UI precheck target set; it raises the timeout floor to 240 seconds for that broader pass. The UI precheck writes both a Markdown review report and `ui_precheck_summary.json` with status/counts/targets/artifact paths. `tools/RunVisionPlatformPrecheck.ps1 -WpfTools` forwards the same option and records the UI summary path in the platform summary JSON.
- `RunUiPrecheck.ps1` now discovers the installed Visual Studio MSBuild path instead of assuming the Professional SKU path, and writes its temporary build output under the requested `-OutputDir` so UI validation works across PCs.

## Phases

### Phase 0: Stabilize Current WinForms Bridge

Keep the existing forms usable while migration is in progress.

- Fix input preview refresh bugs in the common WinForms base.
- Avoid broad WinForms layout redesign for preprocessing forms.
- Keep Morphology/Threshold improvements only as compatibility UX until the WPF version exists.

### Phase 1: Extract The Tool Host Contract

Create a small UI-neutral contract for preprocessing views:

- selected input layer
- selected output layer
- source preview bitmap
- result preview bitmap
- run preview command
- add pipeline command
- status/result summary

This contract should be independent from WinForms controls.

### Phase 2: Build WPF Filter Pilot

Create a WPF `Filter` user control and host it from the current WinForms menu/action path.

Keep the old `FormVision_Filter` available until the pilot passes build and smoke verification.

### Phase 3: Convert The Rest Of Preprocessing

Recommended order:

1. Remove duplicated WinForms wrapper dependency from native WPF tool-window paths only after the legacy fallback path has enough parity coverage.
2. Keep `Blob`, `Contour`, `Line`, `Matching`, and `FeatureMatching` on the current PropertyGrid editor while improving only surrounding preview/result-review surfaces.
3. Convert `Line` only after edge/measurement display behavior is stable and the PropertyGrid freeze is explicitly revisited.
4. Convert `Matching` and `FeatureMatching` last because they need template/review-specific layouts and PropertyGrid-safe review surfaces.

### Phase 4: Shared WPF Panels

After representative tool views are stable, move shared operational surfaces to WPF:

- Pipeline document host and step preview.
- Property-adjacent host surfaces while preserving the existing PropertyGrid UX.
- Layer/result list panels.
- Log, history, batch, AI Recipe, sample, and validation dialogs where WPF improves consistency.

### Phase 5: WPF Application Shell

Move the application shell only after the representative surfaces are WPF-ready:

- Main menu/toolbar/status bar.
- Docking/document layout.
- Window state, language switch, settings, and command routing.
- Existing WinForms forms hosted only as temporary compatibility islands.
- Promote the current WPF shell preview into a real host only after command-routing, image workspace, bottom log, and floating tool-window contracts are proven.

### Phase 6: WinForms Retirement

Remove compatibility wrappers only after parity is proven:

- Replace hidden legacy parameter controls with UI-neutral presenters/services.
- Keep XML/pipeline compatibility unchanged.
- Delete obsolete designer files and resources when smoke coverage exists.
- Preserve user workflows while shrinking WinForms dependency surface.

## Definition Of Done

A migrated preprocessing form is done when:

- Build passes.
- The old and new forms produce matching pipeline step XML for the same parameter values.
- Preview and publish behavior are visibly distinct.
- Input preview refreshes from the latest selected layer before execution.
- Screenshot/interaction smoke validates the opened UI.
- The WPF Tool View is the active user-facing surface.
- The WinForms wrapper can stay temporarily as the host/lifecycle layer without exposing duplicate controls.
- For shell-level migration, the WPF shell must preserve the same layer, pipeline, logging, localization, and runner contracts.

## Verification Notes

- 2026-06-20 cycle105: `tools/RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_tools_cycle105` passed all gates with `WpfTools=true`, including Build, Vision UI Contract, XML Compatibility, Sample Catalog Runner, AI Recipe Interactive Contract, Tool Result Contract, Tutorial Portable Contract, default UI precheck, WPF shell preview, WPF Tool View screenshots, WPF Add Pipeline parity, tool-window factory, and localization catalog checks. Sample Catalog summary: 58 OK / 0 NG.
- 2026-06-20 cycle107: `tools/RunUiPrecheck.ps1 -Targets tool_filter_form -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_summary_json_cycle107` passed WPF tool coverage with summary JSON `Status=OK`, `WpfTools=true`, `OK=12`, `WARN=0`, `NG=0`.
- 2026-06-20 cycle108: `tools/RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -UiTargets tool_filter_form -OutputDir artifacts\platform_precheck_ui_summary_cycle108` passed all gates and validated the UI summary JSON inside the platform precheck. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.Status=OK`, `UiPrecheck.OK/WARN/NG=12/0/0`, Sample Catalog 58 OK / 0 NG.
- 2026-06-20 cycle110: `tools/RunUiPrecheck.ps1 -Targets wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_shell_badges_cycle110` passed after compact shell navigation badge localization. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`.
- 2026-06-20 cycle111: `PipelineViewerScreenshotSmoke --target wpf_shell_preview artifacts\smoke\shell_pipeline_tab_cycle111` passed after localizing the WPF shell Pipeline document tab.
- 2026-06-20 cycle112: `tools/RunUiPrecheck.ps1 -Targets wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_shell_localization_cycle112` passed the latest shell badge + document-tab localization state. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`.
- 2026-06-20 cycle113: `tools/RunUiPrecheck.ps1 -Targets tool_filter_form -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle113` passed the broader WPF shell/tool-view bundle after the shell localization changes. Summary JSON: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=12/0/0`.
- 2026-06-20 cycle114: `tools/RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_full_cycle114` passed the full WPF-expanded platform gate after the latest shell/tool/report/document updates. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.OK/WARN/NG=26/0/0`, Sample Catalog 58 OK / 0 NG.
- 2026-06-20 cycle115: `PipelineViewerScreenshotSmoke --target wpf_shell_preview artifacts\smoke\shell_layer_button_cycle115` and `tools/RunUiPrecheck.ps1 -Targets wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_shell_layer_button_cycle115` passed after strengthening WPF shell layer/result row selection visuals and button-command smoke coverage. UI summary: `Status=OK`, `OK/WARN/NG=1/0/0`.
- 2026-06-20 cycle116: `tools/RunUiPrecheck.ps1 -Targets tool_filter_form -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle116` passed the broader WPF shell/tool-view bundle after the layer/result row interaction update. UI summary: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=12/0/0`.
- 2026-06-20 cycle117: `PipelineViewerScreenshotSmoke --target tool_filter_form artifacts\smoke\tool_filter_checkbox_cycle117` and `tools/RunUiPrecheck.ps1 -Targets tool_filter_form -FailOnWarn -OutputDir artifacts\ui_precheck_filter_checkbox_cycle117` passed after replacing the WPF Tool View checkbox with the shared teal/neutral template. UI summary: `Status=OK`, `OK/WARN/NG=1/0/0`.
- 2026-06-20 cycle118: `tools/RunUiPrecheck.ps1 -Targets tool_filter_form -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle118` passed the broader WPF shell/tool-view bundle after the shared checkbox template update. UI summary: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=12/0/0`.
- 2026-06-20 cycle119: `tools/RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_full_cycle119` passed the full WPF-expanded platform gate after the shell layer/result interaction and shared checkbox template updates. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.OK/WARN/NG=26/0/0`, Sample Catalog 58 OK / 0 NG.
- 2026-06-20 cycle130: `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_native_all_preprocess_cycle130` passed after adding native WPF host documents for Arithmetic, HSV, and Histogram and making the host layer/result rail scroll to the latest preview result.
- 2026-06-20 cycle131: `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_shell_right_rail_detail_cycle131` passed after adding selectable layer/result rows and the selected-layer detail thumbnail/size/time surface to the WPF shell host.
- 2026-06-20 cycle133: `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_pipeline_review_viewmodel_cycle133` passed after adding the native WPF Pipeline Review document and moving the new review surface into `Views` / `ViewModels` / `Documents` with `OpenVisionPipelineReviewViewModel`.
- 2026-06-20 cycle136: `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_pipeline_review_validation_cycle136` passed after connecting `VisionPipelineValidator` feedback to the native WPF Pipeline Review document. The visual check confirms the Validation panel surfaces the branch warning concisely beside Flow, Parameters, and Run Log.
- 2026-06-20 cycle139: `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_pipeline_review_run_result_cycle139` passed after adding explicit WPF Pipeline Review execution. The review document now runs the active pipeline through `VisionPipelineExecutionService`, caches result images internally, updates Step Flow state to OK, and shows selected-step result summary/detail without mutating the main workspace layers.
- 2026-06-21 cycle157: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_property_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_property_tool_cycle157` passed after routing PropertyGrid-preserved fallback tools through `OpenVisionFloatingToolWindow`. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle156: `PipelineViewerScreenshotSmoke --target wpf_shell_host_property_tool artifacts\smoke\wpf_shell_property_tool_cycle156` passed after widening the fallback-route contract to Blob, Contour, Line, Matching, and FeatureMatching while keeping the final visual capture on Blob.
- 2026-06-21 cycle158: `PipelineViewerScreenshotSmoke --target wpf_shell_host_property_tool artifacts\smoke\wpf_shell_property_tool_cycle158` passed after localizing the shared floating-window minimize/maximize/close tooltips through the catalog and asserting those values in the PropertyGrid-preserved tool smoke.
- 2026-06-21 cycle159: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_property_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_property_tool_cycle159` passed the same localized floating-window chrome contract through the UI precheck gate. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle161: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool,wpf_shell_host_property_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_direct_status_cycle161` passed after splitting the right-rail direct-result state into pre-execution `대기` and post-preview `OK/완료`. Summary JSON: `Status=OK`, `OK/WARN/NG=2/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle163: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_shell_preview_direct_status_cycle163` passed after aligning the shell preview's direct-result panel to the same pre-execution `대기` state. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle165: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool,wpf_shell_host_property_tool,wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_direct_status_color_cycle165` passed after matching the direct-result panel border color to the pending/success badge state. Summary JSON: `Status=OK`, `OK/WARN/NG=3/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle167: historical hosted-form fallback smoke passed for Blob. This path is now superseded by the WPF-only cleanup; pending algorithm tools use a neutral WPF surface until native views are implemented.
- 2026-06-21 cycle175: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_property_tool,wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_expected_route_cycle175` passed after changing the direct-result route label to `예상 경로:` / `Expected:` and adding localization migration for old `Shell.RouteEmpty` / `Shell.RouteFormat` CONFIG values. Summary JSON: `Status=OK`, `OK/WARN/NG=2/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle177: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_property_tool,wpf_shell_host_native_tool,wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_dynamic_route_cycle177` passed after switching the WPF shell host's expected-route line from static `{Tool}_Preview` text to the active tool's current input/output layer selections. Summary JSON: `Status=OK`, `OK/WARN/NG=3/0/0`, `layout=0`, `text=0`.
- 2026-06-21 cycle179: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_native_status_localized_cycle179` passed after localizing the native WPF tool status prefixes for output-layer creation and unavailable pipeline add actions. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`.

- 2026-06-21 cycle180: `tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle180` passed the broader WPF UI bundle after the direct-route and native-status fixes. Summary JSON: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=27/0/0`, all target rows `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle182: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_bridge -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_pipeline_review_localized_cycle182` passed after localizing the WPF Pipeline Review title, section labels, run/status text, flow/validation/result/run-log labels, and shell selected-layer empty-image state. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle185: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_status_color_tokens_cycle185` passed after making the WPF Tool status presenter evaluate raw and localized text together for success/error/review color tones. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle186: `tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle186` passed the broader WPF UI bundle after the Pipeline Review localization and WPF status-color updates. Summary JSON: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=27/0/0`, all target rows `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle187: `tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_full_cycle187` passed all platform gates after the latest WPF UI changes. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.OK/WARN/NG=27/0/0`, Sample Catalog `58 OK / 0 NG`.

- 2026-06-21 cycle190: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_bridge -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_shell_direct_status_cycle190` passed after removing hidden WPF view type text from the shell active-document marker, renaming the direct-result status field away from `BridgeStatus`, and extending UI diagnostics to reject implementation terms such as `ToolWpfView`, `OpenVisionPipelineReviewView`, `ActiveForm=`, `WPF Host`, and `BridgeStatus`. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle191: `tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle191` passed the broader WPF UI bundle after the implementation-term cleanup. Summary JSON: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=27/0/0`, all target rows `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle192: `tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_full_cycle192` passed all platform gates after the implementation-term cleanup and WPF UI baseline refresh. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.OK/WARN/NG=27/0/0`, Sample Catalog `58 OK / 0 NG`.

- 2026-06-21 cycle194: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_shell_preview_tooltips_cycle194` passed after binding the preview shell's Settings/Export/Minimize/Maximize/Close icon tooltips to localization keys and adding `Common.Export`. Summary JSON: `Status=OK`, `OK/WARN/NG=1/0/0`, `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle195: `tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle195` passed the broader WPF UI bundle after the preview shell tooltip localization. Summary JSON: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=27/0/0`, all target rows `layout=0`, `text=0`, `internal=0`.

- 2026-06-21 cycle196: `tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_full_cycle196` passed all platform gates after the preview shell tooltip localization and latest WPF UI baseline refresh. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.OK/WARN/NG=27/0/0`, Sample Catalog `58 OK / 0 NG`.
- 2026-06-21 cycle221: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_only_cycle221 -WpgCustomBuildEnabled false -TimeoutSeconds 300` passed the WPF-only UI bundle. Targets included shell preview, workspace output switching, native tool window, pending tool window, ROI editor, Image Compare, log panel, and localization catalog. Summary JSON: `Status=OK`, `OK/WARN/NG=10/0/0`.
- 2026-06-21 cycle222: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_only_cycle222` passed external reference, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample, WPF shell contract, and tutorial portable gates.
- 2026-06-21 cycle225: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_pending_wording_cycle225 -WpgCustomBuildEnabled false -TimeoutSeconds 300` passed after replacing visible implementation wording on pending algorithm tool windows with neutral `Pending` / `준비 중` copy.
- 2026-06-21 cycle226: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_only_cycle226` passed after the WPF-only cleanup, pending-copy cleanup, and documentation update.
- 2026-06-21 cycle229: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_blob_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_blob_cycle229 -WpgCustomBuildEnabled false -TimeoutSeconds 300` passed after adding the native WPF Blob window and verifying preview, visual capture, pending-tool copy, and Blob Add Pipeline step metadata.
- 2026-06-21 cycle230: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_contour_tool,wpf_shell_host_blob_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_contour_cycle230 -WpgCustomBuildEnabled false -TimeoutSeconds 300` passed after adding the native WPF Contour window and moving the pending-tool smoke to Line.
- 2026-06-21 cycle237: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_cycle237 -WpgCustomBuildEnabled false -TimeoutSeconds 300` passed after adding the native WPF Line window, LineGauge preview drawing, test ROI diagnostics, and moving the pending-tool smoke to Matching.
- 2026-06-21 cycle238: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_algorithm_first_pass_cycle238 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed all 13 default WPF UI targets, including Blob, Contour, Line, pending Matching, ROI editor, Image Compare, log panel, and localization catalog.
- 2026-06-21 cycle239: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled false -OutputDir artifacts\platform_precheck_wpf_algorithm_first_pass_cycle239` passed build, Vision UI, History, Localization, Readiness, XML, runner/sample catalog, AI Recipe, Tool Result, sample inventory, WPF shell contract, and tutorial portable gates.
- 2026-06-21 cycle241: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_matching_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_matching_cycle241 -WpgCustomBuildEnabled false -TimeoutSeconds 300` passed after adding the native WPF Matching window, template-ready state, default original/full-image matching, preview overlay, Add Pipeline validation, and moving the pending-tool smoke to FeatureMatching.
- 2026-06-21 cycle244: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_matching_full_cycle244 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed all 14 default WPF UI targets, including Blob, Contour, Line, Matching, pending FeatureMatching, ROI editor, Image Compare, log panel, and localization catalog.
- 2026-06-21 cycle245: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_matching_cycle245` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Matching WPF pass.
- 2026-06-21 cycle246: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_feature_matching_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_feature_matching_cycle246 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding the native WPF FeatureMatching window, template-ready state, default original/full-image feature matching, SIFT preview overlay, Add Pipeline validation, and converting the pending-tool smoke into a generic pending-surface contract.
- 2026-06-21 cycle247: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_feature_matching_full_cycle247 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets, including Blob, Contour, Line, Matching, FeatureMatching, the generic pending surface, ROI editor, Image Compare, log panel, and localization catalog.
- 2026-06-21 cycle248: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_feature_matching_cycle248` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the FeatureMatching WPF pass.
- 2026-06-21 cycle254: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_match_review_assert_cycle254 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding compact Matching/FeatureMatching score/center/box result-review text. The smoke now asserts the review text updates after Run Preview and includes `Center` and `Box` instead of remaining `Result not run`.
- 2026-06-21 cycle253: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_match_review_center_full_cycle253 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the result-review change.
- 2026-06-21 cycle255: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_blob_tool,wpf_shell_host_contour_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_blob_contour_review_cycle255 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding compact Blob/Contour area/center/box result-review text and smoke assertions.
- 2026-06-21 cycle256: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_blob_contour_review_full_cycle256 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the Blob/Contour result-review change.
- 2026-06-21 cycle257: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_review_cycle257 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding compact LineGauge edge/line-length result-review text and smoke assertions.
- 2026-06-21 cycle258: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_review_full_cycle258 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the LineGauge result-review change.
- 2026-06-21 cycle260: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_purpose_cycle260 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding the Line `Edge / Measure / Intersection` purpose selector, purpose-specific preview review assertions, and `LinePurpose` pipeline metadata.
- 2026-06-21 cycle261: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_purpose_full_cycle261 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the Line purpose selector change.
- 2026-06-21 cycle265: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_pair_cycle265 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding `Line A / Line B` PropertyGrid switching, Measure-as-`LineDistance`, Intersection-as-`LineIntersection`, paired-line pipeline metadata, and result-review assertions.
- 2026-06-21 cycle266: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_pair_full_cycle266 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the paired-line Line tool change.
- 2026-06-21 cycle267: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_line_pair_cycle267` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after registering paired-line `LineDistance` / `LineIntersection` execution.
- 2026-06-21 cycle268: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_roi_edit_cycle268 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after adding the selected-line ROI edit button, WPF ROI-editor handoff, and `RightCvROI` paired-line metadata assertion.
- 2026-06-21 cycle269: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_roi_edit_full_cycle269 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the Line selected-ROI edit change.
- 2026-06-21 cycle270: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_line_roi_edit_cycle270` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Line selected-ROI edit change.
- 2026-06-21 cycle274: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_distance_sample_cycle274 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after replacing the Line smoke image with paired vertical edge regions, defaulting the Line A measurement direction toward Line B, keeping the final screenshot on Measure, and asserting the result review is not `Distance none` / `Count 0`.
- 2026-06-21 cycle276: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_line_distance_sample_cycle276` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Line distance sample change.
- 2026-06-21 cycle277: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_distance_sample_full_cycle277 -WpgCustomBuildEnabled false -TimeoutSeconds 480` passed all 15 default WPF UI targets after the Line distance sample change.
- 2026-06-21 cycle280: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_two_samples_cycle280 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed after splitting Line smoke coverage into separate Measure length and Intersection cross-point samples. The visual review confirms repeated red distance lines for Measure and fitted red crossing lines plus a point for Intersection.
- 2026-06-21 cycle281: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_two_samples_full_cycle281 -WpgCustomBuildEnabled false -TimeoutSeconds 540` passed all 16 default WPF UI targets after the Line sample split.
- 2026-06-21 cycle282: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_line_two_samples_cycle282` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Line two-sample change.
- 2026-06-21 cycle288: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_intersection_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_intersection_point_sample_cycle288 -WpgCustomBuildEnabled false -TimeoutSeconds 360` passed after replacing the Intersection sample with a part-corner image matching the user reference: the lower horizontal edge and right vertical edge are fitted separately, then extended to `Point 345,307`.
- 2026-06-21 cycle289: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_two_samples_point_cycle289 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed the separated Measure and Intersection samples after removing the misleading `Align` wording.
- 2026-06-21 cycle290: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_point_full_cycle290 -WpgCustomBuildEnabled false -TimeoutSeconds 540` passed all 16 default WPF UI targets after the Intersection point sample update.
- 2026-06-21 cycle291: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_line_point_cycle291` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after regenerating the portable tutorial with the two new Line sample images.
- 2026-06-21 cycle292: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_roi_overlay_cycle292 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed after adding initial input-preview `Line A` / `Line B` ROI markers. Visual review confirmed the paired Measure ROIs and the part-corner Intersection ROIs are visible before Run Preview without changing the PropertyGrid editor.
- 2026-06-21 cycle293: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_line_roi_overlay_full_cycle293_retry -WpgCustomBuildEnabled false -TimeoutSeconds 540` passed all 16 default WPF UI targets after the Line ROI input-overlay update.
- 2026-06-21 cycle294: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_line_roi_overlay_cycle294` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Line ROI input-overlay update.
- 2026-06-21 cycle295: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_match_feature_review_prefix_cycle295 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed after clarifying Matching/FeatureMatching result-review labels as `Template Match` and `Feature Match`. Visual review confirmed both screens show count, score, center, and box on the result-review row.
- 2026-06-21 cycle296: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_match_feature_review_prefix_full_cycle296 -WpgCustomBuildEnabled false -TimeoutSeconds 540` passed all 16 default WPF UI targets after the Matching/FeatureMatching result-review label update.
- 2026-06-21 cycle297: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_match_feature_review_prefix_cycle297` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Matching/FeatureMatching result-review label update.
- 2026-06-21 cycle298: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_blob_contour_line_review_labels_cycle298 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed after clarifying Blob/Contour/Line result-review labels as `Blob`, `Contour`, and `Edge` with count/metric details. Visual review confirmed Blob and Contour show count/max-area/center/box and Line keeps clear Measure output after Edge assertion.
- 2026-06-21 cycle299: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_blob_contour_line_review_labels_full_cycle299 -WpgCustomBuildEnabled false -TimeoutSeconds 540` passed all 16 default WPF UI targets after the Blob/Contour/Line result-review label update.
- 2026-06-21 cycle300: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_blob_contour_line_review_labels_cycle300` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Blob/Contour/Line result-review label update.
- 2026-06-21 cycle308: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load,wpf_shell_host_workspace -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_workspace_empty_load_cycle308 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed after restoring the WPF workspace no-image prompt and image-load entry. Visual review confirmed the shell starts with a clear no-image prompt, loads a selected file into `Main`, and refreshes the active tool input preview.
- 2026-06-21 cycle309: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_workspace_empty_load_full_cycle309 -WpgCustomBuildEnabled false -TimeoutSeconds 600` passed all 18 default WPF UI targets after adding workspace empty/load coverage.
- 2026-06-21 cycle310: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_workspace_empty_load_cycle310` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the workspace empty/load update.
- 2026-06-21 cycle313: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_tool_input_empty,wpf_shell_host_tool_input_image_load_save,wpf_shell_host_workspace_image_load -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_tool_preview_image_commands_cycle313 -WpgCustomBuildEnabled false -TimeoutSeconds 420` passed after adding per-Tool-View input image loading, empty-image prompts, `Main` fallback input-layer selection, and preview image save actions. Visual review confirmed the Filter tool input slot starts with a clear prompt, then loads and saves the selected image.
- 2026-06-21 cycle314: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_tool_preview_image_commands_full_cycle314 -WpgCustomBuildEnabled false -TimeoutSeconds 720` passed all 20 default WPF UI targets after adding Tool View input load/save coverage.
- 2026-06-21 cycle315: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_tool_preview_image_commands_cycle315` passed external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample catalog, WPF shell contract, and tutorial portable gates after the Tool View input load/save update.
- 2026-06-22: `dotnet build .\OpenVisionLab.csproj -c Debug -v:minimal` passed after moving Line Tool input ROI rectangles from the separate WPF Canvas overlay into `VisionToolOpenGlPreviewSlot` OpenGL overlays.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool -OutputDir artifacts\ui_precheck_line_opengl_roi_overlay_assert_r1_20260622 -TimeoutSeconds 520 -FailOnWarn` passed after adding smoke assertions that both Line A/B OpenGL ROI overlays are published before Run Preview.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_line_opengl_roi_overlay_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed all 29 WPF tool/shell targets after the Line OpenGL ROI overlay move.
- 2026-06-22: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_line_opengl_roi_overlay_r1_20260622` passed build, contracts, XML compatibility, sample catalog (`58 OK / 0 NG`), WPF shell contract, and portable tutorial gates after the Line OpenGL ROI overlay move.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_layer_docking,wpf_shell_host_layer_docking_functional -OutputDir artifacts\ui_precheck_docking_header_diagnostics_r3_20260622 -TimeoutSeconds 300 -FailOnWarn` passed after strengthening AvalonDock tab/title drag affordance checks and resetting the docking root pane on clear/re-dock.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_layer_docking -OutputDir artifacts\ui_precheck_docking_header_visible_r1_20260622 -TimeoutSeconds 300 -VisibleCapture -FailOnWarn` passed visible capture after the dock header UX update.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_docking_header_diagnostics_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed all 29 WPF tool/shell targets after adding OpenGL `.opengl.txt` screenshot diagnostics.
- 2026-06-22: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_docking_header_diagnostics_r1_20260622` passed build, contracts, XML compatibility, sample catalog (`58 OK / 0 NG`), WPF shell contract, and portable tutorial gates after the docking header/OpenGL diagnostics update.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_layer_auto_docking,wpf_shell_host_threshold_tool,wpf_shell_host_rotate_scale_tool -OutputDir artifacts\ui_precheck_auto_dock_slider_r2_20260622 -TimeoutSeconds 360 -FailOnWarn` passed after adding the selection-first auto-docking contract and shared slider chrome breathing-room checks.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_auto_dock_slider_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed all 30 WPF tool/shell targets after the auto-docking and slider breathing-room checks.
- 2026-06-22: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_auto_dock_slider_r1_20260622` passed all platform gates after regenerating `OPENVISIONLAB_TUTORIAL_PORTABLE.html`; sample catalog summary was `58 OK / 0 NG`, and the portable tutorial embedded `28/28` source images.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_large_image_16k_perf -OutputDir artifacts\wpf_16k_perf_auto_dock_slider_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed the 16384x16384 viewer functional smoke with 16 OpenGL tiles in workspace, docked, and popout viewers. The working set ended at 4300.1 MB, so large-image memory should stay on the next optimization list.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_layer_docking_vertical,wpf_shell_host_layer_docking_n_panels,wpf_shell_host_layer_docking_functional,wpf_shell_host_tool_rail_compact,wpf_shell_host_workspace_output -OutputDir artifacts\ui_precheck_release_gate_docking_bounds_r1_20260622 -TimeoutSeconds 520 -FailOnWarn` passed after adding shell-relative docked viewer bounds checks to catch pane overlap/escape regressions.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_tool_input_empty,wpf_shell_host_tool_input_image_load_save,wpf_shell_host_threshold_tool,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool,wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_workspace_output -OutputDir artifacts\ui_precheck_release_gate_tool_views_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed the focused Tool View UX regression gate.
- 2026-06-22: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_large_image_16k_perf -OutputDir artifacts\ui_precheck_release_gate_16k_r1_20260622 -TimeoutSeconds 1200 -FailOnWarn` passed the 16384x16384 viewer smoke with 16 OpenGL tiles in workspace/docked/popout viewers and working set `143.4 MB -> 1592.9 MB`.

## Immediate Next Work

1. Keep applying the `Views` / `ViewModels` / `Documents` structure to new WPF surfaces.
2. Preserve user workflows: selected input/output layers must update the main workspace, preview must stay separate from publish, and logs must remain visible at the bottom.
3. Review common WPF tool-window helpers only where they remove real duplication without hiding tool-specific behavior.
4. Keep result-review labels guarded by screenshot smoke whenever a completed WPF tool changes.
5. Keep `tools\RunUiPrecheck.ps1 -FailOnWarn` plus a focused target for the changed surface as the UI handoff gate; run `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false` after structural migration changes.
