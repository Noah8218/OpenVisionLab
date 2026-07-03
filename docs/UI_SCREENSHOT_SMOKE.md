# UI Screenshot Smoke

OpenVisionLab UI smoke checks are WPF-first. The active screenshot runner no longer opens legacy WinForms tool forms.

## Run

Full WPF UI precheck:

```powershell
.\tools\RunUiPrecheck.ps1 -FailOnWarn
```

Screenshot-only pass:

```powershell
.\tools\RunUiScreenshotSmoke.ps1
```

List available focused suites and individual targets:

```powershell
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --list
```

Run one focused suite:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiScreenshotSmoke.ps1 -Platform x64 -Suite route
```

## Focused Suites

Use focused suites for normal development. They keep the smoke pass tied to the code path that changed, instead of running every UI scenario after each small patch.

- `route`: layer selection, input/output route persistence, no unintended auto-docking, arithmetic double-input selection, and matching-family template route checks.
- `property-grid-auto-preview`: PropertyGrid-based algorithm tools where `SelectedObject` is still the model source, property rows render, and property changes schedule preview.
- `preprocess-auto-preview`: Threshold, Filter, Morphology, and simple preprocess custom controls where parameter changes should publish an output preview image.
- `e2e`: file-load and operator-style end-to-end flows. Use this before handoff when workspace load, pipeline review, or result publication behavior changed.
- `perf`: tool-window open timing and prewarm/cache behavior. Run this only when startup, tool creation, document caching, or viewer reuse changed.

Selection policy:

- Use `--target` for one isolated control or regression.
- Use `-Suite route` after layer, route, output panel, template registration, or matching-family changes.
- Use `-Suite property-grid-auto-preview` after PropertyGrid editor, metadata, model, or algorithm parameter binding changes.
- Use `-Suite preprocess-auto-preview` after Threshold/Filter/Morphology/custom preprocess view changes.
- Use `-Suite e2e` before handing off broad operator workflow changes.
- Use `-Suite perf` after performance-related refactors only.
- After docking wrapper, ShellHost docking orchestration, or docked tool-window changes, run the explicit docking targets because there is no dedicated `docking` suite yet: `wpf_shell_host_layer_global_docking,wpf_shell_host_layer_docking_guide_visible,wpf_shell_host_layer_tab_drag_guide_visible,wpf_shell_host_layer_docking_persistence`. Add `wpf_tool_window_dock_float_cycle` when docked/floating tool inspector behavior changed.
- Use `-All` only for broad UI refactors, release checks, or when several suites fail and the failure boundary is unclear.

The `--list` output is the source of truth for the currently registered targets and suite membership.

Frequently used targets:

- `wpf_shell_preview.png`: WPF shell preview surface.
- `wpf_shell_host_window_chrome.png`: default WPF shell window using the shared custom title bar instead of the native Windows title bar.
- `wpf_shell_host_workspace_empty.png`: WPF shell host empty workspace prompt; verifies no auto-seeded image, localized no-image guidance, beginner workflow steps, Korean/English text refresh without opening tools, guide entry, Run Log empty-state guidance, and the image-load action.
- `wpf_shell_host_workspace_sample_picker.png`: WPF shell host sample catalog picker; verifies runnable sample search/list/detail UI, selected sample image, benchmark strip, Learn Mode guidance, tool flow, expected metrics, Good/Bad or single-sample reference state, and explicit no-auto Preview/Run guidance.
- `wpf_shell_host_workspace_sample_pair_picker.png`: WPF shell host sample catalog Good/Bad pair picker; verifies a selected pair sample, pair-comparison strip, opposite reference summary, Learn Mode guidance, and explicit no-auto Preview/Run guidance.
- `wpf_shell_host_workspace_sample_open.png`: WPF shell host sample entry; verifies the no-image sample button, loads a runnable catalog sample into `Main`, activates a `Sample_` pipeline, shows the sample workflow strip/top sample-ready status with explicit next-action buttons, and does not auto-open a tool or run Preview.
- `wpf_shell_host_recipe_context_switch.png`: WPF shell host recipe-context contract; verifies recipe A/B active pipeline resolution, visible top-bar recipe/pipeline context, source path resolution, no auto tool open/no auto Preview on context switching, and explicit Pipeline Review opening against the active context.
- `wpf_shell_host_workspace_sample_actions.png`: WPF shell host sample next-action check; verifies that the sample strip can explicitly open Pipeline Review and the first step tool through the Shell command path without running Preview automatically.
- `wpf_shell_host_workspace.png`: WPF shell host with the central WPF image workspace.
- `wpf_shell_host_workspace_image_load.png`: WPF shell host after loading an image into `Main` from the workspace image-load path; verifies localized image-ready guidance, quick action labels, Korean/English text refresh without opening tools, zoom/pan/pointer status, and no auto-preview.
- `wpf_shell_host_tool_input_empty.png`: floating native WPF tool window with an empty input preview prompt and a non-empty `Main` input-layer selector.
- `wpf_shell_host_tool_input_image_load_save.png`: floating native WPF tool window after loading an image from the input preview path and saving the selected input image.
- `wpf_shell_host_workspace_output.png`: WPF shell host after native preview switches the central workspace to the output layer.
- `wpf_shell_host_large_image.png`: 5200x5200 8bpp grayscale image coverage across workspace, docked layer, and popout OpenGL viewers.
- `wpf_shell_host_large_image_16k_perf.png`: explicit heavy 16384x16384 8bpp grayscale performance target. This is not part of the default `-WpfTools` gate; run it directly when large-image performance needs to be checked.
- `wpf_shell_host_layer_auto_docking.png`: selection-first layer comparison contract. Selecting `Main` and then a generated preview layer must switch the workspace into AvalonDock comparison mode without requiring the old right-rail Dock button path.
- `wpf_shell_host_layer_docking_vertical.png`: bottom/top AvalonDock layer comparison contract. Docked OpenGL layer viewers must stay clipped inside their panes, keep compact chrome in narrow vertical splits, and pass non-overlapping viewer-bounds assertions.
- `wpf_shell_host_layer_docking_n_panels.png`: N-panel AvalonDock comparison contract. `Main` plus three generated layers are arranged through horizontal, vertical, and final horizontal pane layouts without dropping panes, tiles, drag-ready headers, or overlapping viewer bounds.
- `wpf_shell_host_tool_rail_compact.png`: compact left tool rail contract. The tool list collapses into a single expander handle to recover workspace width for multi-layer comparison without leaving an unreadable icon column.
- `wpf_shell_host_layer_docking.png`: WPF shell host with multiple layer documents docked for side-by-side comparison, drag-ready AvalonDock tab/title headers, and OpenGL diagnostics sidecar output.
- `wpf_shell_host_layer_docking_functional.png`: functional docking contract for default docking, split panes, floatable panels, drag header readiness, clear/re-dock root-pane recovery, and layout persistence.
- `wpf_shell_host_layer_docking_grid.png`: grid-style docked layer comparison contract for two-row/four-pane workspace arrangements.
- `wpf_shell_host_layer_docking_tabs.png`: same-pane tab merge contract for docked layer comparison.
- `wpf_shell_host_layer_global_docking.png`: Visual Studio-style workspace-level layer docking contract. Global guide zones split the whole workspace, while pane-local guide zones remain available for hovered-pane split/tab docking.
- `wpf_shell_host_layer_docking_guide_visible.png`: two-level docking guide visibility contract with global workspace zones plus pane-local zones.
- `wpf_shell_host_layer_tab_drag_guide_visible.png`: AvalonDock layer tab/title drag affordance contract. Dragging a live layer tab must show the Shell-owned guide instead of native white floating windows.
- `wpf_shell_host_layer_docking_persistence.png`: explicit docked-layer layout save/restore contract. The smoke backs up the live `CONFIG/UI/LayerDocking.*` files, saves a horizontal split layout through Shell test hooks, restores it, and then restores the original operator files.
- `wpf_shell_host_layer_popout.png`: separate OpenGL layer popout viewer with image metadata and zoom/pan canvas.
- `wpf_shell_host_workspace_avalondock_tabs.png`: shell workspace tab contract for AvalonDock-backed layer documents.
- `wpf_shell_host_bridge.png`: WPF shell host with seeded layer/workspace state.
- `wpf_shell_host_native_tool.png`: floating native WPF tool window.
- `wpf_tool_window_dock_float_cycle.png`: docked tool inspector cycle contract. Floating and docking an active tool must preserve hosted content, selected layers, parameter state, preview state, and the reverse float action.
- `wpf_shell_host_threshold_basic_tool.png`: native WPF Threshold tool in basic mode with debounced preview, modern parameter layout, and readable slider/input alignment.
- `wpf_shell_host_threshold_tool.png`: native WPF Threshold tool with basic/range/adaptive modes, combo/button selection checks, and preview publication. Because Threshold persists the last taught mode, the smoke must explicitly put the view into Basic before checking Basic slider layout, then switch to Range/Adaptive for their layout checks.
- `wpf_shell_host_pipeline_review.png`: WPF Pipeline Review surface with a 3-step flow, selected-step guide strip/detail row, previous/next step navigation, branch-route explanation, Korean/English guide recalculation without reopening the review document, input/output previews, explicit Run Review, result decision, validation detail, and run-log context.
- `wpf_shell_host_pipeline_review_ng.png`: WPF Pipeline Review acceptance-NG surface with a successful Threshold execution that fails a metric target, visible NG decision/next action, localized metric-target failure reason, run-log context, and retained failed-step output preview.
- `wpf_shell_host_rotate_scale_tool.png`: native WPF Rotate/Scale tool with slider/value synchronization, slider chrome breathing-room validation, preview, and Add Pipeline validation.
- `wpf_shell_host_blob_tool.png`: native WPF Blob tool window with PropertyGrid-preserved parameters, compact verification guide, threshold teaching preview, explicit detection preview, localized `Blob` result-review text with count/max-area/center/box size, Preview OK/NG guidance, and Add Pipeline step validation.
- `wpf_shell_host_blob_tool_docked_verification.png`: focused alias for the Blob tool path that verifies the right-docked inspector layout, compact verification guide text, result guidance, and usable PropertyGrid editor space while the image workspace remains visible.
- `wpf_shell_host_contour_tool.png`: native WPF Contour tool window with PropertyGrid-preserved parameters, compact verification guide, preview execution, localized `Contour` result-review text with count/max-area/center/box size, Preview OK/NG guidance, and Add Pipeline step validation.
- `wpf_shell_host_contour_tool_docked_verification.png`: focused alias for the Contour tool path that verifies the right-docked inspector layout, compact verification guide text, result guidance, and usable PropertyGrid editor space while the image workspace remains visible.
- `wpf_shell_host_line_measure_tool.png`: native WPF Line tool path with PropertyGrid-preserved `Line A` / `Line B` settings, localized `목적` / `라인` purpose controls, input-preview OpenGL Line A/B ROI overlays, selected-line ROI edit affordance, paired vertical-edge length sample, compact summary-strip verification guidance, localized mode-specific Edge/Measure result-review text, Measure preview that shows repeated Line A measurement lines intersecting Line B edges, non-zero distance result-review text, `LineDistance` metadata, Add Pipeline validation, and docked inspector density checks.
- `wpf_shell_host_line_tool_docked_verification.png`: focused alias for the Line tool path that verifies the right-docked inspector layout, compact summary-strip verification guidance, Line A/B controls, ROI edit affordance, and usable PropertyGrid editor space while the image workspace remains visible.
- `wpf_shell_host_line_intersection_tool.png`: floating native WPF Line tool window with a part-corner intersection sample, input-preview OpenGL Line A/B ROI overlays, `Line A` / `Line B` fit-line settings, Intersection preview that extends the horizontal and vertical fitted edges to a crossing point, `Point` / `Cross Yes` result-review text, `LineIntersection` metadata, and Add Pipeline validation.
- `wpf_shell_host_matching_tool.png`: floating native WPF Matching tool window with PropertyGrid-preserved parameters, localized template-ready state, compact verification guide, preview overlay, localized teaching summary for score/count/search and original/full-image state, localized `Template Match` result-review text with count/score/center/box size, Preview OK/NG guidance, criteria/next-action text, and Add Pipeline step validation.
- `wpf_shell_host_matching_tool_docked_verification.png`: focused alias for the Matching tool path that verifies the right-docked inspector layout, compact verification guide text, compact result guidance, and usable PropertyGrid editor space while the image workspace remains visible.
- `wpf_shell_host_matching_presets.png`: Matching-family preset contract. Verifies the floating Basic/Fast/Precise preset strip, the docked `Parameters` header preset menu, exact PropertyGrid model updates, generated row visibility refresh, preserved docked editor height, and no Preview/Run execution even when `AUTO_PREVIEW=true` was enabled before applying a preset.
- `wpf_shell_host_edge_based_matching_tool.png`: native WPF EdgeBasedMatching tool window with PropertyGrid-preserved parameters, template-ready state, compact edge verification guide, Canny/search/point criteria, `Edge Match` result-review text with count/score/center/box size, Preview OK/NG guidance, and Add Pipeline step validation.
- `wpf_shell_host_feature_matching_tool.png`: floating native WPF FeatureMatching tool window with PropertyGrid-preserved parameters, localized template-ready state, FeatureMatching-specific compact verification guide, Ratio/RANSAC criteria summary, SIFT preview overlay, `Feature Match` result-review text with count/score/center/box size, and Add Pipeline step validation.
- `wpf_shell_host_pending_tool.png`: generic pending view contract for future tools whose WPF surface is not ready yet.
- `wpf_roi_editor.png`: WPF ROI editor with source image, selected ROI overlay, coordinate fields, and action controls.
- `wpf_image_compare.png`: WPF Image Compare window with two loaded images, source format headers, selected slot state, pixel coordinate/RGB/GV/Delta status, and synchronized zoom contract.
- `log_panel_contract_check.png`: WPF log panel layout and filtering controls.
- `localization_catalog_contract_check.png`: localization catalog contract surface.

Console output uses the standard contract format:

```text
wpf_shell_host_native_tool=OK|check=OK|colors=64|flat=0%|layout=0|text=0|...
```

## Review Checklist

- Floating tool windows render with WPF chrome and no implementation wording.
- Native tool input/output previews, combo boxes, sliders, and action buttons are readable.
- Pending tool windows clearly communicate that the tool is not ready yet without exposing implementation terms.
- ROI editor overlays must render over the image with handles, coordinate fields, and action buttons visible.
- Image Compare must render loaded image slots, compact tool buttons, selected-slot border, and populated pixel status without reverting to the old WinForms/OpenGL compare form.
- OpenGL-backed shell/tool captures may still look dark in PNG output. For those targets, inspect the matching `.opengl.txt` sidecar for runtime tile counts, image sizes, ROI overlay counts, docked pane counts, and `DockHeadersReady`.
- AvalonDock layer comparison must be checked in horizontal, vertical/bottom, and N-panel arrangements; docked OpenGL viewers should not bleed outside their pane bounds or overlap each other.
- The shell preview keeps the main workspace, right result rail, and bottom log readable.
- No generated screenshot is blank, clipped, or dominated by placeholder-only content.

## Development Gate

During implementation, run the smallest suite that covers the changed path:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiScreenshotSmoke.ps1 -Platform x64 -Suite route
```

Run the full WPF UI precheck before handing off broad UI-heavy changes:

```powershell
.\tools\RunUiPrecheck.ps1 -FailOnWarn
```

For larger platform checks without re-running UI screenshots, use:

```powershell
.\tools\RunVisionPlatformPrecheck.ps1 -SkipUi
```
