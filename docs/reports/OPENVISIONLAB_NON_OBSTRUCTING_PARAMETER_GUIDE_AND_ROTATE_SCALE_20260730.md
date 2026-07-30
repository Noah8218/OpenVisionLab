# OpenVisionLab Non-Obstructing Parameter Guide And RotateScale

Date: 2026-07-30 KST<br>
Priority: P261<br>
Status: Complete

## Outcome

The contextual Parameter Guide no longer occupies or covers the existing
teaching surface. A current Debug EXE reproduced the previous overlay covering
three EdgeDetection controls. The guide is now a nonmodal sidecar owned by the
Tool window, and the same current EXE verified that neither EdgeDetection nor
RotateScale teaching controls are obstructed.

P261 also replaces Basic fallback for all five RotateScale properties:

- `Angle`
- `ScaleXPercent`
- `ScaleYPercent`
- `Interpolation`
- `BorderType`

## Operator Contract

- In a floating Tool window, selecting a supported parameter may show the guide
  beside the Tool without activating the guide or taking keyboard focus.
- The guide must not cover parameter controls, input/output images, result
  status, Pipeline actions, or explicit Preview/Run controls.
- The `?` button in the parameter header explicitly hides or reopens the guide.
- If the operator hides the guide, it stays hidden for that Tool session until
  explicitly reopened.
- Docked Tool views do not automatically open an external guide. The operator
  can open it explicitly with the same `?` button.
- Guide selection, open, hide, and reopen never execute Preview/Run and never
  create, select, or reroute layers.

## Actual EXE Finding And Correction

The before and after checks used:

`C:\Git\OpenVisionLab_Dev\bin\Debug\OpenVisionLab.exe`

At a `920 x 660` Tool window size, the previous in-Tool overlay intersected:

- `txtCannyThresholdHigh`
- `txtCannyApertureSize`
- `chkUseL2Gradient`

The corrected sidecar was positioned at `948,106`, outside the Tool window.
Both final checks reported:

- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

Evidence:

- Before report and screenshot:
  `artifacts\p261_parameter_guide_non_obstructing_20260730\actual_exe_before`
- EdgeDetection after:
  `artifacts\p261_parameter_guide_non_obstructing_20260730\actual_exe_after_edge_final`
- RotateScale after:
  `artifacts\p261_parameter_guide_non_obstructing_20260730\actual_exe_after_rotate_scale_final`

## RotateScale Guidance Grounding

The detailed text follows the current Library-Noah runtime:

- scale dimensions are calculated from source width/height and X/Y percentages;
- resizing occurs before rotation;
- rotation uses the scaled image center;
- the rotated canvas keeps the resized dimensions;
- interpolation and border modes are passed to the OpenCV operations.

The guide therefore tells the operator to verify:

- rotation direction, center, cropped corners, and generated borders;
- `ResultImageWidth` and `ResultImageHeight`;
- aspect distortion when X/Y scales differ;
- interpolation edge quality, ringing, gray-value changes, and runtime;
- artificial border edges that can affect downstream Blob/Contour or fixed ROI
  inspection.

Direct RotateScale controls bind Angle, Scale X, and Scale Y to the guide.
Interpolation and Border Type remain available through the selected-Step
PropertyGrid and receive the same detailed catalog content.

## Verification

- `p261_rotate_scale_parameter_guide=OK`
- Detailed RotateScale coverage: `5/5`
- Korean/English and exact `deg`/`%` units: passed
- P259 and P260 shared-guide regressions: passed
- `wpf_preprocess_output_preview_flow=OK`
- `wpf_tool_window_dock_float_cycle=OK`
- `wpf_shell_host_rotate_scale_tool=OK`
- Debug solution build: 0 warnings, 0 errors
- OpenVisionLab readiness: all 12 contract categories passed
- Post-change catalog audit: 318 browsable, 241 detailed, 77 Basic fallback

Reusable audit:

`artifacts\p261_parameter_guide_non_obstructing_20260730\audit_after_rotate_scale_r2\p260-parameter-guide-fallback-audit.tsv`

## Completion Record

Status: Complete<br>
Scope: Non-obstructing shared Parameter Guide presentation and detailed
RotateScale guidance only.<br>
Acceptance criteria: Actual EXE before obstruction reproduced; actual EXE
EdgeDetection and RotateScale after checks found no intersecting teaching
controls; automatic display retained input focus; explicit hide/reopen passed;
zero Preview/Run/layer/routing side effects; RotateScale detailed coverage is
5/5 in Korean and English.<br>
Verification: Current Debug EXE layout reports/screenshots, focused guide
smokes, RotateScale runtime and dock/float regressions, full Debug solution
build, and readiness all passed.<br>
Evidence:
`artifacts\p261_parameter_guide_non_obstructing_20260730` and this report.<br>
Boundary / next dependency: The visual proof covers the current workstation
and the tested `920 x 660` Tool size, not every monitor/DPI topology. It is
operator guidance, not automatic tuning or algorithm qualification. CVR-00
still requires three independent first-time participants and raw observations.
The remaining 77 Basic entries require a fresh bounded-family selection; they
must not be bulk-authored without runtime and workflow grounding.
