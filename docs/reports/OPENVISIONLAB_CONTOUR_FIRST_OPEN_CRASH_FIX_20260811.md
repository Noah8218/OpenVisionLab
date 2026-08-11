# OpenVisionLab Contour First-Open Crash Fix

Status: Complete

Date: 2026-08-11 KST

## Scope And Acceptance

Opening Contour must succeed even when native Tool prewarm has not created or
laid out the Contour document. Opening it must not run Preview/Run, create
layers, or change the active layer or Tool routes.

Algorithm execution, Contour parameters, Recipe XML, detection results, and
unrelated UI behavior are excluded.

## Reproduced Cause

The saved recent Tool was Line. Startup therefore queued the heavy Tool views
with Line before Contour. Selecting Contour before that queue completed called
`PauseForOperatorSelection`, which cancelled the remaining native Tool
prewarm. The on-demand document path then sent a newly created Contour
PropertyGrid view directly to `Window.Show()`.

The first WPF Grid arrangement failed inside
`Grid.SetFinalSizeMaxDiscrepancy` with `NullReferenceException`. Contour opened
normally when its document had received the existing
`WarmPrewarmedNativeToolDocument` layout preparation. A prepared floating
window did not prevent the failure when the Contour document itself was not
prepared, and Blob passed under the same prewarm-disabled condition. Recipe
XML and floating-window creation were therefore not the cause.

## Change

`OpenVisionShellHostToolWindowController.ShowSelectedTool` now checks whether
the selected native Tool document already exists in the document cache. A
newly created document receives the existing hosted-layout preparation after
its layer state is refreshed and before the first floating or docked show.
Cached, background-prewarmed, and reopened documents keep the existing fast
path.

The existing `learn-contour-practice` actual-EXE smoke now retains a screenshot
of the opened Contour Tool and its monitor placement in addition to checking
that Preview and layer state remain unchanged.

## Verification

| Check | Result | Evidence |
| --- | --- | --- |
| Embedded-smoke OpenVisionLab build | PASS | 0 warnings, 0 errors |
| Native prewarm disabled, Contour first open | PASS | `after_native_prewarmless_contour` |
| Native and floating prepare disabled, Contour first open | PASS | `after_fully_prewarmless_contour` |
| Normal startup Contour open | PASS | `after_normal_contour` |
| Fully prewarm-disabled Blob comparison | PASS | `after_fully_prewarmless_blob` |
| All 17 native Tool layer selection/creation routes | PASS | `wpf_layer_selection_all_native_tools`: layout/text/internal 0 |
| Contour floating and docked focused targets | PASS | both targets: layout/text/internal 0 |
| Actual-EXE Tool dock/float cycle | PASS | final Matching document docked; zero floating windows |
| Debug solution build | PASS | 0 warnings, 0 errors |
| OpenVisionReadinessCheck | PASS | 13/13 contracts |

Physical evidence root:

`D:\OpenVisionLab-TestData\OpenVisionLab\contour_tool_crash_fix_20260811`

The selected actual-EXE monitor was `\\.\DISPLAY2`; the captured Contour Tool
window intersected that monitor. The after screenshot is
`after_fully_prewarmless_contour\OpenVisionLab_Contour_Tool.png`.

## Boundary

The original repository received the same reviewed source. Its actual EXE
independently passed `learn-contour-practice` with
`OPENVISIONLAB_DISABLE_NATIVE_PREWARM=1` and
`OPENVISIONLAB_DISABLE_FLOATING_PREPARE=1`: Contour opened on
`\\.\DISPLAY2`, `PreviewRunCount=0`, and `LayerCount=0`. The original full
Debug build passed with zero warnings/errors and readiness passed 13/13.
