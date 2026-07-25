# OpenVisionLab Status And Next Steps

Updated: 2026-06-25

## Tracker Policy

Use the tracker documents as the current source of truth for completion state.

- Completed work is tracked in `docs/OPENVISIONLAB_COMPLETED_TRACKER.md`.
- Active and deferred work is tracked in `docs/OPENVISIONLAB_PROGRESS_TRACKER.md`.
- Items in the completed tracker should not be re-added as generic future work unless a concrete regression, user redesign request, or failed contract check reopens them.

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

## Known Bugs Logged 2026-06-25

These items are user-reported regressions and must not be treated as completed until reproduced in the actual EXE and verified with the same user workflow.

- PropertyGrid ROI editing can unexpectedly affect the Output side. Reported workflow: click `ROI` in a tool PropertyGrid, then an unexpected value/state appears in the Output area. Expected behavior: editing a tool ROI must only update the tool property (`CvROI` / related ROI property) and must not create, select, rename, or write an Output layer unless the user explicitly runs preview, creates an output layer, or confirms a publish action.
- Output preview drag state can remain stuck after mouse release. Reported workflow: drag inside the Output preview, release the mouse button, and the preview continues behaving as if drag mode is still active. Expected behavior: `MouseUp` must release mouse capture and reset the preview interaction mode to idle/none, even when the pointer leaves the preview area or the preview is hosted in a floating tool window.

Fix applied 2026-06-25:

- ROI editor save now normalizes the selected region before assigning it to the PropertyGrid value, and ROI/mask editor results (`CvROI`, `CvROIS`, `CvMASKS`) no longer schedule automatic preview execution. Verified with `dotnet build` and `OpenVisionLab.exe --smoke property-grid-roi-editor`; the exact user workflow still needs manual EXE confirmation.
- Inline preview pan now completes through `PreviewMouseUp` and `LostMouseCapture`, and `MouseLeave` no longer cancels a captured pan before the release event. Verified with `dotnet build` and the focused `wpf_shell_host_blob_tool` UI precheck; the exact output-pane drag workflow still needs manual EXE confirmation.

## Work Completed In This Pass

Latest 2026-06-22 docking/OpenGL/large-image hardening pass:

- Follow-up bottom-docking overlap pass completed: docked layer viewers now enter a smaller compact-pane mode, hide their inner OpenGL toolbar/status chrome, and apply clipping from the layer viewer down to the shared OpenGL canvas host. This targets the AvalonDock bottom split case where hosted OpenGL content could visually bleed into neighboring panes.
- Follow-up comparison workspace width pass completed: the left tool rail now collapses into a single expander handle so users can recover horizontal space while comparing multiple docked image layers without reading a stack of ambiguous icons.
- Added focused docking smoke coverage for vertical/bottom docking, N-panel docking, and compact tool rail behavior: `wpf_shell_host_layer_docking_vertical`, `wpf_shell_host_layer_docking_n_panels`, and `wpf_shell_host_tool_rail_compact`.
- Follow-up docking bounds guard completed: `AssertDockedLayerLayout` now checks each visible docked layer viewer's shell-relative bounds and fails if panes escape the shell or overlap. This protects the bottom-docking/N-panel comparison UX from returning to visually stacked or bleeding panes.
- Follow-up large-image memory pass completed: workspace and layer viewers no longer keep separate full-size Bitmap clones for display, the OpenGL canvas ViewModel can use lazy save delegates instead of retaining a full Mat copy for viewer-only surfaces, and texture upload no longer clones the whole Mat for 1-channel/3-channel images before tiling.
- Follow-up Bitmap tile-upload pass completed: workspace, docked layer viewers, and popout layer viewers now upload Bitmap tiles directly into OpenGL for viewer-only surfaces. This removes the remaining full-frame `Bitmap -> Mat` conversion from the large layer viewing path while preserving lazy save behavior.
- Follow-up Tool preview memory pass completed: `VisionToolOpenGlPreviewSlot` no longer keeps a cloned Mat for display-only previews, and preview publication no longer creates an extra undisposed Bitmap before handing the result to the layer store. Output previews now display the stable published layer image, so preview save remains layer-based.
- Follow-up processing input pass completed: native Tool Run Preview no longer performs an extra full-frame `.Clone()` immediately after `BitmapImageConverter.ToMat(...)` for single-input and arithmetic inputs. The Bitmap-to-Mat conversion still creates an independent processing Mat, but the second full-image copy is removed.
- Follow-up Pipeline Review cache pass completed: Pipeline Review now transfers ownership of each Mat-to-Bitmap conversion directly into its review-layer cache and only fills missing outputs at run completion. This avoids a second full-size Bitmap clone and avoids recaching step outputs that were already published during step updates.
- Follow-up Arithmetic execution pass completed: Arithmetic pipeline execution no longer clones an already-1-channel input just to treat it as grayscale. It borrows the existing Mat for read-only operations and disposes only the Mat created by color-to-gray conversion.
- Follow-up Line rendering pass completed: Line Edge preview and Line Intersection result rendering no longer roundtrip through `Bitmap`/GDI just to draw overlays. ROI boxes, edge points, fit lines, and intersection markers are drawn directly on OpenCV `Mat` images.
- Follow-up Matching overlay pass completed: Matching and FeatureMatching preview overlays now draw directly on the tool result image when that result is already owned by the preview result. They still clone only when falling back to the input image, so the input preview is not mutated.
- Follow-up Run Report overlay pass completed: saved report overlay PNGs now draw overlays in-place on the already-created result bitmap instead of allocating a second full-size `Bitmap` copy just for report rendering.
- AvalonDock layer tabs and pane titles now expose a clearer drag affordance (`SizeAll` cursor, larger tab hit area, and tooltip), and smoke asserts dock header readiness through `DockHeadersReady=True`. Clearing docked layers now resets the AvalonDock root pane before re-docking, avoiding stale narrow title/pane remnants after clear/re-dock cycles.
- Added `wpf_shell_host_layer_auto_docking` to lock the intended comparison UX: selecting `Main` and then a generated preview layer must enter the AvalonDock workspace, create two panes, load OpenGL tiles for both layers, and expose drag-ready dock headers without requiring the old explicit Dock button path.
- Tool View slider chrome was tightened in the shared WPF theme and SimplePreprocess parameter rows. Threshold and Rotate/Scale smoke now assert visible sliders have enough height, rendered thumb/track parts, and no clipping-prone `ClipToBounds` setting.
- UI smoke now writes OpenGL runtime diagnostics next to captured PNGs as `.opengl.txt` sidecars. These files record shell workspace tiles, docked pane/header state, layer viewer image sizes, tile counts, compact chrome state, and Tool preview slot tile/ROI overlay counts so dark OpenGL screenshots can be interpreted without guessing.
- The Line tool input preview now uses the same `VisionToolOpenGlPreviewSlot` path as the other native WPF Tool Views. Line A/B input-preview ROIs are now published into the OpenGL canvas overlay path instead of a separate WPF Canvas badge layer, and smoke asserts both OpenGL texture tiles and Line A/B overlay publication.
- Large-image coverage was added to the WPF tool gate. `wpf_shell_host_large_image` creates and verifies a 5200x5200 8bpp grayscale image across workspace, docked layer, and popout OpenGL viewers.
- A 16384x16384 8bpp grayscale performance smoke was added as an explicit heavy target: `wpf_shell_host_large_image_16k_perf`. It passed functionally with 16 OpenGL tiles in workspace, docked, and popout viewers.
- 16K baseline before the memory pass was `artifacts/ui_precheck_large_16k_perf_r2_20260622`: total 57.8s, working set 142.8 MB -> 9178.1 MB.
- 16K result after the first memory pass was `artifacts/ui_precheck_large_16k_perf_memory_r1_20260622`: total 33.2s, working set 149.3 MB -> 3139.4 MB, managed memory 1819.8 MB.
- 16K result after Bitmap tile upload was `artifacts/ui_precheck_large_16k_perf_bitmap_tile_r1_20260622`: total 16.9s, working set 147.2 MB -> 2729.7 MB, managed memory 335.1 MB.
- Latest 16K result after Tool preview slot cleanup is `artifacts/ui_precheck_large_16k_perf_tool_slot_r1_20260622`: create image 7.3s, set Main layer 3.6s, workspace pump 1.1s, dock 0.3s, popout 0.4s, total 13.8s, working set 142.8 MB -> 1646.7 MB, managed memory 411.3 MB. The 16K viewer path is now fast enough for practical UX validation; the remaining large-image work is mostly around processing-time Mat conversions and explicit source ownership.
- Latest 16K result after processing-input clone cleanup is `artifacts/ui_precheck_large_16k_perf_input_mat_r1_20260622`: create image 7.5s, set Main layer 3.3s, workspace pump 1.0s, dock 0.2s, popout 0.4s, total 13.7s, working set 143.8 MB -> 1635.7 MB, managed memory 663.7 MB. The visible 16K viewer path remains stable; this pass mainly reduces Run Preview input-copy cost for tool execution.
- Latest 16K recheck after the auto-docking/slider UX pass is `artifacts/wpf_16k_perf_auto_dock_slider_r1_20260622`: create image 8.3s, set Main layer 5.1s, workspace pump 0.4s, dock 0.9s, popout 0.7s, total 16.5s, 16 OpenGL tiles in workspace/docked/popout viewers, working set 147.6 MB -> 4300.1 MB, managed memory 2074.1 MB. Functional coverage is OK, but the higher working-set result should be treated as a large-image memory follow-up item.
- Latest 16K release-gate recheck is `artifacts/ui_precheck_release_gate_16k_r1_20260622`: create image 7.5s, set Main layer 3.4s, workspace pump 1.0s, dock 0.3s, popout 0.5s, total 14.0s, 16 OpenGL tiles in workspace/docked/popout viewers, working set 143.4 MB -> 1592.9 MB, managed memory 484.0 MB. This brings the visible 16K viewer path back in line with the stable memory results from the earlier tile-upload/tool-slot passes.
- Tool preview image load/save smoke now uses unique temp image paths to avoid GDI+ file-lock collisions during full-sequence runs.
- Tutorial screenshots for docking, Line measure, and Line intersection were refreshed, and `docs/OPENVISIONLAB_TUTORIAL_PORTABLE.html` was regenerated with embedded images.
- Verification passed:
  - `dotnet build .\OpenVisionLab.csproj -c Debug -v:minimal`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking_functional,wpf_shell_host_layer_docking,wpf_shell_host_large_image,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool" -OutputDir artifacts\ui_precheck_docking_large_line_opengl_r2_20260622 -TimeoutSeconds 420 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\ui_precheck_large_16k_perf_r2_20260622 -TimeoutSeconds 1200 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool" -OutputDir artifacts\ui_precheck_docking_line_visible_r1_20260622 -TimeoutSeconds 420 -VisibleCapture -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_tool_input_image_load_save" -OutputDir artifacts\ui_precheck_tool_input_save_unique_r1_20260622 -TimeoutSeconds 240 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_docking_line_large_full_r3_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_tool_input_image_load_save,wpf_shell_host_layer_docking,wpf_shell_host_large_image,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool" -OutputDir artifacts\ui_precheck_large_memory_focused_r1_20260622 -TimeoutSeconds 420 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\ui_precheck_large_16k_perf_memory_r1_20260622 -TimeoutSeconds 1200 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_large_memory_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image,wpf_shell_host_layer_docking,wpf_shell_host_layer_popout,wpf_shell_host_tool_input_image_load_save" -OutputDir artifacts\ui_precheck_bitmap_tile_upload_focused_r1_20260622 -TimeoutSeconds 420 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\ui_precheck_large_16k_perf_bitmap_tile_r1_20260622 -TimeoutSeconds 1200 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_bitmap_tile_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_tool_input_image_load_save,wpf_shell_host_threshold_tool,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_tool_preview_bitmap_slot_r1_20260622 -TimeoutSeconds 520 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\ui_precheck_large_16k_perf_tool_slot_r1_20260622 -TimeoutSeconds 1200 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_tool_slot_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_threshold_tool,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_tool_input_mat_clone_r1_20260622 -TimeoutSeconds 620 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\ui_precheck_large_16k_perf_input_mat_r1_20260622 -TimeoutSeconds 1200 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_input_mat_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_pipeline_review,wpf_shell_host_workspace_output,wpf_shell_host_layer_docking_functional" -OutputDir artifacts\ui_precheck_pipeline_review_cache_r1_20260622 -TimeoutSeconds 420 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_pipeline_review_cache_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_arithmetic_gray_ownership_r1_20260622`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_line_intersection_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_pipeline_review,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_line_intersection_mat_draw_r1_20260622 -TimeoutSeconds 520 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_line_mat_draw_fullpath_r1_20260622 -TimeoutSeconds 520 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_line_mat_draw_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_line_mat_draw_r1_20260622`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_match_feature_overlay_ownership_r1_20260622 -TimeoutSeconds 520 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_pipeline_review,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_match_feature_report_render_r1_20260622 -TimeoutSeconds 620 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_match_report_render_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_match_report_render_r1_20260622`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool" -OutputDir artifacts\ui_precheck_line_opengl_roi_overlay_assert_r1_20260622 -TimeoutSeconds 520 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_line_opengl_roi_overlay_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_line_opengl_roi_overlay_r1_20260622`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking,wpf_shell_host_layer_docking_functional" -OutputDir artifacts\ui_precheck_docking_header_diagnostics_r3_20260622 -TimeoutSeconds 300 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking" -OutputDir artifacts\ui_precheck_docking_header_visible_r1_20260622 -TimeoutSeconds 300 -VisibleCapture -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_docking_header_diagnostics_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_docking_header_diagnostics_r1_20260622`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_auto_docking,wpf_shell_host_threshold_tool,wpf_shell_host_rotate_scale_tool" -OutputDir artifacts\ui_precheck_auto_dock_slider_r2_20260622 -TimeoutSeconds 360 -FailOnWarn`
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_auto_dock_slider_full_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed `30 OK / 0 WARN / 0 NG`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_auto_docking,wpf_shell_host_layer_docking,wpf_shell_host_layer_docking_functional,wpf_shell_host_layer_docking_vertical,wpf_shell_host_layer_docking_n_panels,wpf_shell_host_tool_rail_compact" -OutputDir artifacts\ui_precheck_docking_full_contract_r2_20260622 -TimeoutSeconds 520 -FailOnWarn` passed `6 OK / 0 WARN / 0 NG`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -OutputDir artifacts\ui_precheck_default_docking_clip_r2_20260622 -TimeoutSeconds 720 -FailOnWarn` passed `33 OK / 0 WARN / 0 NG`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking_vertical,wpf_shell_host_layer_docking_n_panels,wpf_shell_host_layer_docking_functional,wpf_shell_host_tool_rail_compact,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_release_gate_docking_bounds_r1_20260622 -TimeoutSeconds 520 -FailOnWarn` passed `5 OK / 0 WARN / 0 NG` after adding docked viewer bounds-overlap assertions.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_tool_input_empty,wpf_shell_host_tool_input_image_load_save,wpf_shell_host_threshold_tool,wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool,wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool,wpf_shell_host_workspace_output" -OutputDir artifacts\ui_precheck_release_gate_tool_views_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed `10 OK / 0 WARN / 0 NG`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\ui_precheck_release_gate_16k_r1_20260622 -TimeoutSeconds 1200 -FailOnWarn` passed with 16K workspace/docked/popout OpenGL tiles and working set `143.4 MB -> 1592.9 MB`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -OutputDir artifacts\ui_precheck_release_gate_full_r1_20260622 -TimeoutSeconds 900 -FailOnWarn` passed the full default WPF UI gate: `33 OK / 0 WARN / 0 NG`.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_release_gate_r1_20260622` passed all non-UI platform gates: vendored DLLs, build, UI/history/localization/readiness/XML contracts, runner API, AI recipe/tool-result/sample inventory contracts, sample catalog `58 OK / 0 NG`, and tutorial portable `28/28` embedded images.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -OutputDir artifacts\platform_precheck_auto_dock_slider_r1_20260622` passed all platform gates; sample catalog `58 OK / 0 NG`, tutorial portable `28/28` images embedded.
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_large_image_16k_perf" -OutputDir artifacts\wpf_16k_perf_auto_dock_slider_r1_20260622 -TimeoutSeconds 720 -FailOnWarn` passed functionally with 16384x16384 input and 16 OpenGL tiles.
  - `git diff --check` passed with LF/CRLF conversion warnings only.
  - Direct temporary Arithmetic smoke passed: 1-channel pixel value `7` -> `Bitwise_NOT` -> `ADD 5` produced expected pixel value `253`.
- PropertyGrid remains frozen and was not edited in this pass.

Latest 2026-06-22 WPF docking/viewer/tutorial pass:

- Layer comparison has been moved to the AvalonDock-based workspace path. The shell can dock multiple layers, and the smoke now asserts that `Main` and `HSV_Preview` can be split into separate AvalonDock panes instead of only appearing as simple tabs.
- Follow-up UX correction: the selected-layer `Dock` and `Clear docked layers` actions are now visible in the right rail instead of hidden behind a right-click-only path. Dragging a layer row from the right layer/result list into the workspace also docks it, and adding a second docked layer automatically splits it into a separate AvalonDock pane.
- Follow-up UX correction 2: layer viewing now defaults to docking. Selecting a layer row or publishing a new preview layer docks the layer into the central comparison workspace automatically, the right-rail `Dock` action is hidden, and the visible selected-layer action area now keeps `Popout` as the explicit separate-window path. Docked layer panes remain floatable/draggable so the workspace follows the expected Visual Studio-style docking model instead of becoming a fixed split view.
- Docking functional precheck was added. `wpf_shell_host_layer_docking_functional` now verifies that `Main` and `HSV_Preview` dock into split panes, docked panels are floatable, repeated dock requests do not remove existing layers, a layer can merge back to the primary pane and split again, and clearing/re-docking recreates a missing AvalonDock primary pane instead of leaving titles without viewer content.
- Docked layer viewer chrome was compacted. AvalonDock already supplies the layer title, so docked viewers now hide their internal title strip and internal footer status label while keeping the image-canvas coordinate/GV status bar. Popout layer viewers keep the full header/footer chrome.
- The main/docked/popout layer viewers continue to use the shared OpenGL image canvas path with image load/save, Fit, mouse-wheel zoom, and middle-button pan support. Tool input/output preview image load/save and empty-image guidance remain covered by WPF UI smoke.
- Tool View preview first pass now uses the shared OpenGL canvas for most input/output preview slots instead of WPF `Image.Source`. The common `VisionToolOpenGlPreviewSlot` keeps per-slot zoom/pan through the existing canvas, hides the canvas chrome inside compact Tool Views, exposes `HasImage`, texture tile counts, and Line ROI overlay counts for smoke, and keeps the existing right-click load/save and empty-image guidance behavior. The Line tool input preview also uses the OpenGL slot now, with Line A/B rectangles drawn through the same OpenGL overlay path instead of a separate WPF overlay layer.
- The user tutorial was updated around the current WPF workflow: load an image from the workspace prompt or right-click menu, tune Threshold, add the result to the pipeline, dock original/result layers, drag docked tab headers to compare, and use the separate OpenGL popout for a single large layer view.
- `docs/assets/tutorial/layer_docking.png` was refreshed with the AvalonDock split comparison screenshot, and `docs/OPENVISIONLAB_TUTORIAL_PORTABLE.html` was regenerated with 28 embedded images.
- Verification passed:
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpfTools -OutputDir artifacts\platform_precheck_wpf_docs_docking_r1_20260622`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\wpf_tools_docking_docs_r2_20260622 -TimeoutSeconds 300`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\wpf_tools_docking_real_ux_full_r1_20260622 -TimeoutSeconds 360`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\wpf_tools_opengl_preview_full_r1_20260622 -TimeoutSeconds 420`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\wpf_tools_docking_default_full_r1_20260622 -TimeoutSeconds 420`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking_functional,wpf_shell_host_layer_docking" -OutputDir artifacts\ui_precheck_docking_functional_r3_20260622 -TimeoutSeconds 240 -FailOnWarn`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking" -OutputDir artifacts\ui_precheck_docking_visible_r1_20260622 -TimeoutSeconds 240 -VisibleCapture -FailOnWarn`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -WpfTools -OutputDir artifacts\wpf_tools_docking_functional_full_r1_20260622 -TimeoutSeconds 300 -FailOnWarn`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking_functional,wpf_shell_host_layer_docking" -OutputDir artifacts\ui_precheck_docking_compact_chrome_r1_20260622 -TimeoutSeconds 240 -FailOnWarn`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_docking" -OutputDir artifacts\ui_precheck_docking_compact_chrome_visible_r1_20260622 -TimeoutSeconds 240 -VisibleCapture -FailOnWarn`
  - `powershell -ExecutionPolicy Bypass -File .\tools\RunUiPrecheck.ps1 -Targets "wpf_shell_host_layer_popout" -OutputDir artifacts\ui_precheck_layer_popout_after_compact_chrome_r1_20260622 -TimeoutSeconds 240 -FailOnWarn`
  - `dotnet run --project .\tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- .`
  - UI summary: `Status=OK`, latest WPF Tools `OK/WARN/NG=29/0/0`.
  - Platform summary: `Status=OK`, Sample Catalog `58 OK / 0 NG`, Tutorial Portable `OK`.
- PropertyGrid remains frozen. Blob/Contour/Line/Matching/FeatureMatching WPF work should improve surrounding preview/review surfaces without changing the PropertyGrid editor itself.

Remaining focused work:

- Optional final AvalonDock handoff: run one manual desktop drag/drop pass against the tab/title headers before release. Automated smoke now verifies default docking, split panes, floatable panel state, merge/split layout recovery, clear/re-dock recovery, header hit-area readiness, and visible capture, but it still does not physically drag the mouse through every AvalonDock drop target.
- Persist richer AvalonDock pane geometry if required. Current persistence restores docked layer titles and pane grouping; full floating window geometry should be added only after choosing a stable AvalonDock serialization path for the current package.
- True OpenGL pixel capture remains optional future work. Current smoke now writes `.opengl.txt` runtime diagnostics for OpenGL-backed viewers; visual screenshots can still show dark OpenGL regions because the WPF capture path does not reliably include hosted OpenGL pixels.
- Continue large-image memory pressure reduction outside the viewer/preview display path. The optimized path reduced the 16K working set from about 9.18 GB to about 1.64 GB and total runtime from 57.8s to 13.7s. Next work should focus on deeper pipeline execution image ownership inside runtime contexts and replacing unavoidable full-frame processing conversions with explicit ROI/tile/operation-specific policies where the tool can support them.
- Continue WPF-native refinement around algorithm result review panels and sample-backed explanations while leaving PropertyGrid behavior untouched.

Latest 2026-06-21 WPF migration update:

- WPF-only cleanup pass completed: `OpenVisionLab.csproj` now uses `UseWPF=true` and `UseWindowsForms=false`, and normal startup remains `OpenVisionShellHostWindow`.
- Removed legacy WinForms/RJ UI dependency projects and wrappers from the active solution path: `RJControls`, `OpenVisionLab.MessageBox`, `OpenVisionLab.Controls.Init`, `OpenVisionLab.ImageCanvas`, old `FormMainFrame`/`FormTeachingVision`, old `FormVision_*`, old popup forms, old `DisplayDockHost`, old `VisionPipelineFormBridge`, and old Image Compare coordinate-contract tooling.
- `AppCommon`/`CCommon` now route message prompts through a WPF message wrapper; path/config helpers no longer depend on `Application.StartupPath`.
- `Library/OpenVisionLab.Localization` is WPF/console-safe (`UseWindowsForms=false`) and no longer contains the WinForms localization editor/localizer.
- Unfinished algorithm tools now show a neutral pending tool surface (`준비 중` / `Pending`) instead of exposing implementation wording.
- Latest WPF-only UI verification passed: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_only_cycle221 -WpgCustomBuildEnabled false -TimeoutSeconds 300`; screenshots were visually reviewed for the shell workspace/output switch, native HSV tool, ROI editor, and Image Compare.
- Latest focused pending-tool wording verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_pending_wording_cycle225 -WpgCustomBuildEnabled false -TimeoutSeconds 300`; captured UX no longer shows WPF implementation copy.
- WPF-only cleanup platform verification passed: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_only_cycle226`.
- Latest Blob/Contour/Line result-review verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_blob_tool,wpf_shell_host_contour_tool,wpf_shell_host_line_measure_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_blob_contour_line_review_labels_cycle298 -WpgCustomBuildEnabled false -TimeoutSeconds 420`; the Blob, Contour, and Line windows open as native WPF, preview publishes their preview layers, Add Pipeline creates valid steps, and smoke asserts tool/mode-specific `Blob`, `Contour`, and `Edge` result-review labels with count/metric details.
- Latest Line paired-setting verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool -FailOnWarn -OutputDir artifacts\ui_precheck_line_opengl_roi_overlay_assert_r1_20260622 -TimeoutSeconds 520`; the Line window opens as native WPF, switches through `Edge`, `Measure`, and `Intersection`, switches the PropertyGrid between `Line A` and `Line B`, exposes the selected-line ROI edit button, publishes both `Line A` / `Line B` input-preview rectangles through the OpenGL canvas overlay path before Run Preview, uses separate length and intersection smoke samples, preview publishes `Line_Preview`, Measure runs through `LineDistance` with non-zero distance count, Intersection runs through `LineIntersection` with `Point 345,307` / `Cross Yes`, and Add Pipeline creates paired-line metadata including individual `LeftCvROI` / `RightCvROI`.
- Latest Matching WPF verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_matching_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_matching_cycle241 -WpgCustomBuildEnabled false -TimeoutSeconds 300`; the Matching window opens as native WPF, shows template-ready state, defaults to original/full-image matching, publishes `Matching_Preview`, draws a match-box overlay, creates a valid `Matching` step, and FeatureMatching still shows neutral pending copy.
- Latest FeatureMatching WPF verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_feature_matching_tool,wpf_shell_host_pending_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_feature_matching_cycle246 -WpgCustomBuildEnabled false -TimeoutSeconds 360`; the FeatureMatching window opens as native WPF, shows template-ready state, defaults to original/full-image feature matching, publishes `FeatureMatching_Preview`, draws a SIFT match-box overlay, and creates a valid `FeatureMatching` step.
- Latest Matching/FeatureMatching result-review verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_matching_tool,wpf_shell_host_feature_matching_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_match_feature_review_prefix_cycle295 -WpgCustomBuildEnabled false -TimeoutSeconds 420`; the smoke asserts Run Preview updates tool-specific `Template Match` / `Feature Match` result-review text with `Count`, `Score`, `Center`, and `Box`.
- Latest workspace image-load UX verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_workspace_empty,wpf_shell_host_workspace_image_load,wpf_shell_host_workspace -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_workspace_empty_load_cycle308 -WpgCustomBuildEnabled false -TimeoutSeconds 420`; the shell now starts from a clear no-image prompt, exposes `Image Load` from the empty prompt and right-click menu, loading a file publishes it to `Main`, and the active tool input preview refreshes.
- Latest Tool View input image-load/save verification passed: `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_tool_input_empty,wpf_shell_host_tool_input_image_load_save,wpf_shell_host_workspace_image_load -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_tool_preview_image_commands_cycle313 -WpgCustomBuildEnabled false -TimeoutSeconds 420`; native WPF Tool Views show no-input guidance, keep `Main` available in the input-layer combo even before an image exists, load images from the input preview area, and save selected preview images.
- Latest full WPF UI verification passed: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_tool_preview_image_commands_full_cycle314 -WpgCustomBuildEnabled false -TimeoutSeconds 720`; summary `OK/WARN/NG=20/0/0`.
- Latest platform verification passed: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_tool_preview_image_commands_cycle315`; summary `Status=OK`, sample catalog `OKRows=58`, `NGRows=0`, Tutorial Portable `OK`.
- FeatureMatching platform verification passed: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_feature_matching_cycle248`; summary `Status=OK`.
- Algorithm first-pass platform verification passed: `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled false -OutputDir artifacts\platform_precheck_wpf_algorithm_first_pass_cycle239`; summary `Status=OK`.
- Latest ImageCompare publish verification passed: `scripts\Publish-ImageCompare.ps1 -Configuration Release -SmokeTest`; the WPF ImageCompare package now publishes as an 8-file package, 4.48 MB in `dist\OpenVisionLab.ImageCompare`.
- The layer display core no longer depends on WinForms `DockPanel`/`FormLayerDisplay`. `DisplayLayerStore`, `DisplayLayerPresenter`, `DisplayImageSyncService`, and `DisplayManagerService` now drive the WPF `ImageSpace` workspace directly, while the bottom shell area remains the real `LogPanelView`.
- Removed the remaining dock/log WinForms wrappers in this path: `FormLayerDisplay`, `DisplayDockHost`, `IDisplayHostBinder`, `FormLogViewer`, and `WorkbenchDockPaneCaptionFactory`. `DockPanelSuite` package references and the old WeifenLuo binding redirect were removed from the main app.
- The standalone Image Compare tool is now WPF-native through `ImageCompareWindow` and `ImageCompareViewModel`. It keeps multi-image loading, source format display, last-open directory, pixel coordinate/RGB/GV/Delta status, Fit reset, and synchronized mouse-wheel zoom. The old `FormImageCompare` files were removed.
- `OpenVisionLab.ImageCanvas` has been removed from the active repo path; WPF `ImageSpace`, WPF ROI editor, and WPF Image Compare now cover the active display/edit/compare paths.
- Latest verification passed: `tools\RunUiPrecheck.ps1 -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_layer_core_cycle212 -WpgCustomBuildEnabled false`, focused Image Compare UX `artifacts\ui_precheck_wpf_image_compare_cycle214\wpf_image_compare.png`, ImageCompare publish smoke through `scripts\Publish-ImageCompare.ps1 -Configuration Release -SmokeTest`, and platform precheck `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_layer_core_cycle217`.
- ROI/Pattern template selection now opens a native WPF `RoiEditorWindow` instead of the old `FormImageEditView` WinForms form. The editor keeps the existing PropertyGrid contracts (`SelectedRegion`, `SelectedRegions`, template image save/load), supports single ROI, multi ROI, and TRAIN preview modes, and provides image-overlay handles plus coordinate fields. The old WinForms ROI editor and `PatternMatchPreviewView` files were removed. Focused verification passed through `tools\RunUiPrecheck.ps1 -Targets wpf_roi_editor -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_roi_cycle208 -WpgCustomBuildEnabled false`; the captured UX shows the ROI overlay and handles over the source image. The platform WPF shell contract now also includes `wpf_roi_editor` and passed through `tools\RunVisionPlatformPrecheck.ps1 -SkipUi -WpgCustomBuildEnabled:$false -OutputDir artifacts\platform_precheck_wpf_roi_cycle209`.
- Promoted the WPF shell host to the default application shell: normal startup now opens `OpenVisionShellHostWindow`.
- The WPF host reuses the command catalog, tool-window factory, runtime context, layer seeding, native WPF Tool Views, and the WPF `ImageSpace` workspace. This keeps command routing and layer/document hosting in the WPF shell without exposing implementation terms in the UI.
- The active app path is WPF-only: Filter, Morphology, EdgeDetection, Rotate/Scale, Mean, Arithmetic, HSV, Histogram, Blob, Contour, Line, Matching, and FeatureMatching open as native WPF floating tool windows.
- UX correction: the WPF shell keeps the central image workspace visible, keeps the bottom area as the real OpenVisionLab log panel, and opens preprocessing/algorithm views as separate windows instead of docking them into the bottom shell area.
- Visual verification passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_native_tool artifacts\smoke\wpf_shell_log_panel_cycle151`; the captured UX shows the WPF shell with a floating HSV tool window, right layer/result rail, central image workspace, and bottom `LogPanelView`.
- Follow-up native tool-window pass: Filter, Morphology, EdgeDetection, Rotate/Scale, Mean, Arithmetic, HSV, and Histogram now run as native WPF floating tool windows through `OpenVisionNativeToolDocument`. Native previews create `EdgeDetection_Preview`, `RotateScale_Preview`, `Mean_Preview`, `Arithmetic_Preview`, `HSV_Preview`, `Histogram_Preview`, and `Morphology_Preview`, and the right layer/result rail now reflects the actual DisplayManager layer list instead of static shell-preview rows. The rail scrolls to the latest result when many preview layers exist.
- Latest visual verification for the native tool-window path passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_native_tool artifacts\smoke\wpf_shell_log_panel_cycle151`.
- PropertyGrid-preserved algorithm work is now reopened as WPF surface work: Blob, Contour, Line, Matching, and FeatureMatching have first-pass native WPF views without restoring the old WinForms forms or changing the frozen PropertyGrid behavior.
- Direct-result state UX pass: the WPF shell right rail no longer shows `OK` just because a tool is selected. It shows `대기` / `도구를 실행하면 결과가 표시됩니다.` with a warning-colored badge and border before execution, then changes to green `OK` / `완료 | {elapsed}` only after native WPF preview result publication. The shell preview was aligned to the same pre-execution state. Focused visual verification passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool,wpf_shell_preview -FailOnWarn`.
- The older hosted-form fallback run verification from cycle167 is superseded by the WPF-only cleanup. Algorithm tools without native WPF views now use the neutral pending surface until their WPF views are implemented.
- Expected-route wording pass: the direct-result route text now says `예상 경로:` / `Expected:` so pre-execution selection does not read like a produced result. The localization service migrates old `Shell.RouteEmpty` and `Shell.RouteFormat` values from existing `CONFIG\localization_catalog.tsv` files. Focused smoke passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_property_tool,wpf_shell_preview artifacts\smoke\wpf_shell_expected_route_cycle174`; targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_property_tool,wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_expected_route_cycle175` with `OK/WARN/NG=2/0/0`.
- Dynamic route synchronization pass: the WPF shell host now renders the direct-result expected route from the active tool's actual input/output layer selectors instead of a static `{Tool}_Preview` guess. Blob fallback now displays `Main -> Main` when its output combo is `Main`, while native HSV displays `Main -> HSV_Preview`. Focused smoke passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_property_tool,wpf_shell_host_native_tool,wpf_shell_preview artifacts\smoke\wpf_shell_dynamic_route_cycle176`; targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_property_tool,wpf_shell_host_native_tool,wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_dynamic_route_cycle177` with `OK/WARN/NG=3/0/0`.
- Native WPF status localization pass: `Output layer ready` and `Pipeline add unavailable` status prefixes now pass through the shared WPF status presenter. HSV output-layer creation visually shows `출력 레이어 준비됨 / HSV_Preview`. Focused smoke passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_native_tool artifacts\smoke\wpf_native_status_localized_cycle178`; targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_native_status_localized_cycle179` with `OK/WARN/NG=1/0/0`.
- Native WPF status color pass: the shared WPF status presenter now evaluates both raw status text and localized display text for success/error/review colors, so localized Korean states such as `출력 레이어 준비됨 / HSV_Preview` render with the success tone instead of a neutral or warning tone. Focused smoke passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_native_tool artifacts\smoke\wpf_status_color_tokens_cycle184`; targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_native_tool -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_status_color_tokens_cycle185` with `OK/WARN/NG=1/0/0`.
- Broad WPF UI verification pass: `tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_broad_cycle195` passed after the latest Pipeline Review localization, WPF status-color updates, implementation-term cleanup, and preview shell tooltip localization. The pass covered 27 targets, including the WPF shell preview, WPF Tool View screenshots, PropertyGrid-preserved floating tool route, WPF Add Pipeline parity, tool-window factory, and localization catalog checks. Summary JSON: `Status=OK`, `WpfTools=true`, `OK/WARN/NG=27/0/0`.
- Full WPF platform verification pass: `tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools -OutputDir artifacts\platform_precheck_wpf_full_cycle196` passed all platform gates after the latest WPF UI changes. Platform summary: `Status=OK`, `WpfTools=true`, `UiPrecheck.OK/WARN/NG=27/0/0`, Sample Catalog `58 OK / 0 NG`.
- Shell implementation-term cleanup pass: hidden active-document text now stores user-facing titles instead of WPF view type names, the direct-result status field no longer uses `BridgeStatus` naming, and UI diagnostics reject leaked implementation terms such as `ToolWpfView`, `OpenVisionPipelineReviewView`, `ActiveForm=`, `WPF Host`, and `BridgeStatus`. Targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_bridge -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_shell_direct_status_cycle190` with `OK/WARN/NG=1/0/0`.
- Preview shell tooltip localization pass: Settings/Export/Minimize/Maximize/Close icon tooltips now use localization bindings, including the new `Common.Export` key, and smoke validates Korean/English language switching. Targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_preview -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_shell_preview_tooltips_cycle194` with `OK/WARN/NG=1/0/0`.
- WPF shell right-rail detail pass: the layer/result list is now selectable, clicking a row activates the corresponding DisplayManager layer, and the pipeline rail shows the selected layer thumbnail, image size, recent tack time, and display state. Latest visual verification passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_shell_right_rail_detail_cycle131`.
- WPF Pipeline Review pass: selecting `Pipeline` in the default shell now opens a native WPF review window instead of immediately falling back to the WinForms editor. The review surface uses `OpenVisionPipelineReviewView`, `OpenVisionPipelineReviewViewModel`, and `OpenVisionPipelineReviewDocument` under `Wpf/Views`, `Wpf/ViewModels`, and `Wpf/Documents`; it shows Step Flow, input/output previews, branch reason, parameters, validation status/detail, explicit Run Review execution, selected-step result summary/detail, and run-log context. The Pipeline Review window title, section labels, run button/status, flow/validation/result/run-log labels, and selected-layer empty-image state now use the localization catalog. Latest focused visual verification passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_bridge artifacts\smoke\wpf_pipeline_review_localized_cycle181`; targeted UI precheck passed through `tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_bridge -FailOnWarn -OutputDir artifacts\ui_precheck_wpf_pipeline_review_localized_cycle182` with `OK/WARN/NG=1/0/0`.
- Default startup verification passed without WPF opt-in environment variables: launching `dotnet bin\Debug\OpenVisionLab.dll` opens the WPF shell with the product title `OpenVisionLab`. Latest focused UX capture passed through `PipelineViewerScreenshotSmoke --target wpf_shell_host_native_tool artifacts\smoke\wpf_shell_log_panel_cycle151`.
- Next WPF work should focus on richer result-review surfaces for the completed WPF algorithm views without changing the PropertyGrid itself.

Latest 2026-06-19 direction pass:

- PropertyGrid Threshold UX is now treated as stable/frozen unless a regression or explicit user request reopens it. Threshold children are visually grouped, inversion is consolidated into inline trackbar `Invert`, and enable toggles do not force preview by themselves.
- Pipeline input/output flow now keeps placeholder layers out of runnable input selection, keeps `Run Preview` non-destructive, and requires explicit `Publish Result` for main workspace updates.
- Sample Catalog added the public `Fiducial_Solder` Good/Bad pair and is verified at 58 runnable rows, 58 OK / 0 NG, 12 pair groups / 27 pair rows.
- Acceptance metric failures now include tool-specific tuning guidance so users see likely parameters to inspect instead of only a failed metric range.
- Final non-UI precheck passed: `artifacts\platform_precheck_20260619\platform_precheck_summary.json`.
- The next product direction remains conservative: expand public sample-backed OK/NG coverage and runner/package evidence, avoid broad PropertyGrid redesign, and keep AI Recipe changes operator-reviewed rather than fully automatic.

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
  - Current Sample Catalog baseline after MasterImage removal: 55 runnable, 38 Required, 14 Explore, 3 ExpectedFailure, 55 OK, 0 NG.
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_no_master_platform_precheck_20260618\platform_precheck_report.md`
  - Sample Catalog UI smoke also passed:
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_ui_complete\ui_precheck_report.md`
  - Sample Catalog backlog-none UI contract passed:
    - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_12_work_sample_backlog_none_contract\ui_precheck_report.md`

Latest UI polish update:

- AI Recipe interactive tuning now includes a direct `Apply Suggested Fix` action:
  - Validation/run feedback can apply safe XML corrections without manual XML editing.
  - The `XML Patch Request` panel now shows a `Safe Auto Fix Preview` before applying automatic changes, so users can review Step/Parameter/Layer Flow edits without opening the XML manually.
  - The form now includes `Safe Fix Selection`, so each safe correction can be checked or unchecked before applying. This prevents broad automatic correction when the operator only wants to apply one proven-safe parameter or layer-flow fix.
  - Current safe fixes cover layer-flow mistakes, accidental chained inspection from `Main`, invalid min/max ordering, gray-value clamping, odd kernel/block sizes, positive scale/pixel/sampling values, and Canny/derivative parameter guards.
  - Acceptance loosen/tighten decisions remain manual because they can hide real NG cases.
- The Sample Catalog now has explicit Good/Bad pair metadata:
  - `PairGroup`
  - `PairRole`
  - `BentPin_GoodShaft` and `BentPin_BadShaft` are grouped as `BentPin_Shaft`.
- Added a second Good/Bad inspection pair for film dark-spot inspection:
  - `docs/samples/Film_DarkSpot_Contour.pipeline.xml`
  - `EasyObject_FilmOk_DarkSpot`
  - `EasyObject_FilmBad_DarkSpot`
  - The pair is separated by `ResultCount`, `AreaMax`, and `BoundsWidthMax`.
- AI Recipe prompts now include the sample pair role next to the catalog entry so LLM-generated recipes can use Good/Bad samples as acceptance references.
- Focused AI Recipe and sample-pair smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_pair_contract_20260617`
- Focused Good/Bad Film sample contract passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_goodbad_film_contract_20260617`
- Focused AI Recipe safe-fix preview smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_fix_preview_20260617_b`
- Focused AI Recipe selectable safe-fix smoke passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_selectable_fix_20260617_b`
  - Contract: selectable Safe Fix rows are shown, checked fixes are applied, and unchecked fixes remain unchanged in XML.
- Latest non-UI platform precheck after safe-fix preview passed:
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_safe_fix_platform_20260617\platform_precheck_report.md`
  - `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_ai_recipe_safe_fix_platform_20260617\platform_precheck_summary.json`
- Duplicate-work checkpoint:
  - Good/Bad pair metadata, external reference preflight, direct AI Recipe safe-fix apply, and WPG Threshold/Range editor contracts already exist.
  - Do not reimplement those broad structures. Next work should extend missing coverage or improve operator review, not rebuild the same mechanisms.

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
  - `wpf_shell_preview`
  - `wpf_shell_host_workspace`
  - `wpf_shell_host_workspace_output`
  - `wpf_shell_host_bridge`
  - `wpf_shell_host_native_tool`
  - `wpf_shell_host_pending_tool`
  - `wpf_roi_editor`
  - `wpf_image_compare`
  - `log_panel_contract_check`
  - `localization_catalog_contract_check`
- Message box smoke targets are still available explicitly, but are no longer included in the default UI precheck.
- UI precheck should be scoped to the changed surface whenever possible. For example, workspace-only work should run `wpf_shell_host_workspace` or `wpf_shell_host_workspace_output` instead of every UI target.
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
  - `Contour_MeanBrightness` validates raw source brightness with threshold preprocessing disabled: `MeanValueAvg=240.5` against the sample-backed range `238..243`.
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
  - Real-material `MasterImage` samples were removed from the active catalog and sample tree. Production material images should not be committed as reusable public samples unless they are explicitly sanitized.
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
| Contour_MeanBrightness | Contour_MeanBrightness | MeanValueAvg 238-243 | 240.5 |
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
| Sample catalog runner | OK | Required rows and recursive Explore representatives passed, including LLM OverlayMerge, Blob, LineGauge, Matching, EasyImage, EasyGauge, EasyMatch, EasyObject, EasyColor, EasyFind, EasyBarCode, EasyQRCode, and EasyOcr sample paths. Real-material MasterImage paths are excluded. The report includes category summary, GateStatus, failed sample list, per-sample failure messages, input image metadata validation, artifact validation, and sample-folder coverage/backlog. |
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
  - Runnable rows: `52`
  - Required rows: `36`
  - Explore rows: `15`
  - Expected-failure rows: `1`
  - OK rows: `52`
  - NG rows: `0`
  - Categories: `31`
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
  - Current sample catalog after removing real-material MasterImage rows: `55` runnable rows, `38` Required, `14` Explore, `3` expected-failure rows, `55` OK, `0` NG, `0` artifact issues, `0` metadata issues, with Good/Bad coverage at `11` complete groups and `25` pair rows.
  - Verified by `C:\Users\Public\Documents\ESTsoft\CreatorTemp\openvisionlab_no_master_platform_precheck_20260618\platform_precheck_summary.json`.
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
| Pipeline persistence and samples | 100% | Catalog, sample image load, expected metrics, explicit Good/Bad pair metadata, broader recursive sample inventory, defect-specific representatives, basic Tool representatives, EasyMatrixCode, EasyOCR2, and Film dark-spot representative coverage, preview flow, category summary, md/json runner reports, failed-sample/artifact/metadata/folder-coverage JSON fields, zero uncovered sample folders, and JSON content gates are validated. Real-material MasterImage samples are intentionally excluded; restart/load edge cases and semantic decoder/OCR validation remain separate future work. |
| Result metrics and overlays | 96% | Contour, Blob, LineGauge, Matching, OverlayMerge, rectangle bounds metrics, line length/angle metrics, Pixel/mm-derived bounds/line metrics, and metric-based Acceptance are validated; Sample Catalog rows now gate stronger multi-metric decisions such as count/area, count/bounds, px/mm size, edge-count/line-length/line-length-mm/angle, score/count, merge overlay/source counts, and Good/Bad sample separation by defect size; remaining work is calibration UX and more measurement-specific sample recipes. |
| Logging and message UX | 94% | Log panel level/filter contract is smoke-tested, operator levels are simplified, active-filter wording is explicit, All Logs reports that filters are off, message details/copy actions are clearer, and the current WPF rendering path passed focused smoke; remaining work is message taxonomy final review. |
| Threshold/WPG editors | 92% | Pipeline WPG Threshold/Range editor contract is smoke-tested, duplicate Range helper rows are hidden, LineGauge tuning values now use shared slider/number-range editor metadata, Threshold form mode/input/output/purpose text is contract-tested, and layout/text/internal checks pass; shared editor reuse still needs final consolidation and a stricter visual pass for flat-looking property surfaces. |
| AI Recipe workflow | 98% | Prompt contract, supported ToolType guard, form-only ToolType rejection, final OverlayMerge rule, validation feedback, validation-loop guidance, retry edit-scope guidance, visible first-failed-step retry preview, first-failed-row focus, direct-dependent-step feedback, safe `Apply Suggested Fix` action with visible safe-fix preview, tool-specific patch proposals, Required/Explore sample prompt separation, expected-gate examples, explicit Good/Bad pair prompt entries, sample metric-to-check guidance, distance/size metric guidance, parameter-error diagnostics, and LLM sample UI smoke exist; remaining work is broader automatic tuning coverage and operator review of which fixes should stay manual. |
| External runner/DLL path | 95% | XML runner, CLI smoke, sample catalog execution, multi-metric sample gates, machine-readable sample summary JSON with GateStatus/failed-sample/artifact/metadata/folder-coverage/runtime fields, platform-level summary JSON, Runner API OK/NG summary contracts, action summary, step-flow summary, and sample-backed Tool Result status contract are stable; package/version policy remains. |
| Algorithm robustness | 97% | Sample-backed Contour, Blob, LineGauge, Matching, Mean, RotateScale, Threshold channel normalization, line-gauge helper guards, Blob Required sample coverage, broader recursive representatives, execution-level overlay bounds metrics, line length/angle metrics, Pixel/mm bounds/line metrics, metric-based Acceptance, BentPin branch/merge ROI contract, BentPin good/bad shaft-width px/mm contract, Film good/bad dark-spot contract, DiePad geometry contract, SurfaceDefect edge-contour contract, category-level sample reporting, and successful-step status contracts are stronger; more NG/OK paired defect contracts and tool-specific summary rows still need expansion. |
| Automated UI QA | 98% | Scoped screenshot smoke, designer constructor check, catalog checks, recursive sample contracts, recipe catalog coverage gate, WPG contract check, AI Recipe prompt contract check, log contract, MessageBox contract, Main/Pipeline/Threshold UI 95 pass, Runner API gate, enum/sample-backed Tool Result gates, Sample Catalog JSON/artifact/metadata gate, sample metric review checks, quick/default LLM sample UI validation, Guide/tool-guide document resolver check, tutorial portable contract, strengthened Branch Review contract, backlog-none sample UI contract, and fallback capture exist; visual regression thresholds can still be stricter. |

## Immediate Next Decisions

After the current UX pass is verified, choose one of these tracks.

Recommended order after the current 98% checkpoint:

1. More Good/Bad inspection pairs
   - The catalog now supports explicit `PairGroup`/`PairRole` metadata, and BentPin shaft plus Film dark-spot pairs are contract-tested.
   - Add defect-specific OK/NG pairs beyond the current bent-pin shaft-width coverage.
   - Prioritize pin, die-pad, surface defect, and line/measurement samples.
   - Gate each pair with one explainable metric such as count, bounds width/height, line length, angle, score, or mean value.

2. Interactive AI Recipe tuning expansion
   - Extend `Apply Suggested Fix` beyond safe structural fixes only after each fix type has a sample-backed contract.
   - Add a clearer review surface that shows the exact Step/Parameter/Layer Flow change before applying.
   - Keep acceptance threshold changes manual unless a Good/Bad pair proves the proposed gate.

3. WPF shell/tool-view migration finish
   - Keep the current PropertyGrid UX frozen unless a PropertyGrid-specific request explicitly reopens it.
   - Move polish into WPF shell/tool surfaces: document host, layer/result rail, preview/review panels, and shared WPF controls.
   - Use `RunUiPrecheck.ps1 -WpfTools` and `RunVisionPlatformPrecheck.ps1 -WpfTools` as the migration gate.

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
   - Use the generated public contour sample pair as the first redistributable benchmark.
   - Create stable recipes for text/symbol contour detection.
   - Store expected metrics such as result count, area range, and elapsed time.

3. WPF migration track
   - Promote the WPF shell preview toward a real document host after layer/result, pipeline, log, language, and runner contracts stay green.
   - Keep PropertyGrid-driven tools as preserved editor islands while improving WPF preview/review surfaces around them.
   - Retire WinForms wrappers only after equivalent WPF smoke and XML/pipeline parity coverage exists.

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

WPF migration UI precheck:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -WpfTools -FailOnWarn
```

Full visual capture should remain explicit:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -All
```

Use scoped targets for focused UI work:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunUiPrecheck.ps1 -Targets wpf_shell_host_workspace_output
```

Platform precheck can pass the same scoped UI target:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -UiTargets wpf_shell_host_workspace_output
```

For WPF migration work, prefer the WPF-expanded platform gate before handoff:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn -WpfTools
```

Latest WPF-expanded platform gate:

- `artifacts\platform_precheck_wpf_only_cycle226\platform_precheck_summary.json`
- Summary: platform precheck passed; external references, build, Vision UI, History, Localization, Readiness, XML compatibility, recipe runner/sample execution, WPF shell contract, and Tutorial Portable gates passed.
- Latest broad WPF UI baseline: `artifacts\ui_precheck_wpf_blob_contour_line_review_labels_full_cycle299\ui_precheck_summary.json` with `Status=OK`, `OK/WARN/NG=16/0/0`.
- Latest focused pending-tool wording baseline: `artifacts\ui_precheck_pending_wording_cycle225\ui_precheck_summary.json` with `Status=OK`, `OK/WARN/NG=1/0/0`.

If a previous UI smoke process is still running, clean up only the smoke executable:

```powershell
powershell -ExecutionPolicy Bypass -File tools\StopUiSmoke.ps1
```
