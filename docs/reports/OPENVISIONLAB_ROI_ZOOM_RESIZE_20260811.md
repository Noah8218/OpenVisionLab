# OpenVisionLab ROI Edge Resize And Zoom Editing

Date: 2026-08-11 KST

Repositories: `C:\Git\OpenVisionLab_Dev` and `C:\Git\OpenVisionLab`

## Goal And Boundary

Make a full-image ROI editable from every edge and keep ROI creation, movement,
and resize available while the source image is zoomed and panned.

The change is limited to the existing ROI editor. It does not run Preview/Run,
change layers or routing, alter Tool parameters outside the accepted ROI, or
add a new image viewer.

## Reproduced Cause

- Fit view placed a full-image ROI exactly on the clipped viewport boundary.
  Half of the edge handle was outside the interactive canvas.
- Pointer-to-image conversion was derived from an always-fit rectangle and had
  no zoom or pan state.
- The original actual EXE baseline kept `X=0 / W=640` after an 80-pixel inward
  left-edge drag. No zoom controls were present.

Baseline evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\before_actual_exe\before_full_roi.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\before_actual_exe\before_left_edge_drag.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\before_actual_exe\result.json`

## Implemented Behavior

- Fit view reserves a 14-pixel image margin so all eight ROI handles remain
  visible and hit-testable, including a full-image left edge.
- One display rectangle now owns fit scale, zoom, pan, image-to-display, and
  display-to-image conversion.
- The mouse wheel and localized side-panel buttons zoom from 25% to 1600%.
- Middle-button drag pans within bounded image edges.
- Left-button ROI create/move/resize continues to use image coordinates after
  zoom and pan; dragging outside the visible image clamps to the image bounds.
- Fit View restores 100% view state without changing the ROI.
- Zoom controls reuse the existing themed button style and expose accessible
  automation names. Full ROI exposes `RoiFullButton` for accessibility and
  actual-EXE verification.

## Verification

### Dev

- Full solution Debug build: PASS, 0 warnings / 0 errors.
- OpenVisionReadinessCheck: PASS, 13/13.
- Current-source `wpf_roi_editor`: PASS.
- The focused gate checks full-image left-handle visibility, inward left
  resize, 4x zoom plus pan, finer zoomed resize, and fit restoration.

Current-source capture:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\artifacts\roi_zoom_resize_20260811\dev_final_accessible\wpf_roi_editor.png`

### Original

- Full solution Debug build: PASS, 0 warnings / 0 errors.
- OpenVisionReadinessCheck: PASS, 13/13.
- Current-source `wpf_roi_editor`: PASS.
- The five exact-port source files have identical canonical Git blobs in Dev
  and original.

Original actual-EXE evidence used:

- Runtime:
  `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_runtime_final\OpenVisionLab.exe`
- Runtime SHA-256:
  `D2A907AC24BEB7BBE781848ACAE8E7B992F7ED9DFC225533843BA1A7D227807C`
- Input:
  `E:\라벨테스트\Pins_500_OK_NG\Pins\images\OK\Pins_OK_0001.jpg`
  (`768x576`)
- Monitor:
  `\\.\DISPLAY2`, bounds `-1920,365,1920,1080`; the ROI window intersected
  the selected monitor.
- Full ROI: `X=0 / W=768 / H=576`.
- Fit-view left resize: `X=92 / W=676`; right boundary stayed at 768.
- Zoom in: `125%`.
- Middle pan: `25 px`.
- Zoomed fine resize: `X=101 / W=667`, a 9-pixel image-coordinate adjustment;
  right boundary stayed at 768.
- Zoom out: `100%`.
- Zoom-in hover and actual pointer-down captures differ by 1,008 pixels in the
  button region, and both remain within the light Tool View theme.

Actual-EXE evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_actual_exe\result.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_actual_exe\final_02_left_edge_resized_current_exe.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_actual_exe\final_03_zoom_hover_current_exe.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_actual_exe\final_04_zoom_pressed_current_exe.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_actual_exe\final_05_zoom_pan_current_exe.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab\artifacts\roi_zoom_resize_20260811\after_actual_exe\final_06_zoomed_fine_edit_current_exe.png`

## Boundary

This proves the current ROI editor workflow with the Line Tool and the named
pin image. It does not qualify metrology accuracy or other algorithm results.
No commit, stage, or push was performed.
