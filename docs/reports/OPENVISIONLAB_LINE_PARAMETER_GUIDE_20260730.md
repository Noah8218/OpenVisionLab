# OpenVisionLab Line Parameter Guide

Date: 2026-07-30 KST<br>
Priority: P265<br>
Status: Complete

## Outcome

The 11 remaining Basic entries in `LineGauge/LineDistance` now have
runtime-grounded Korean/English detailed guidance:

1. Distance-sampling direction:
   `USE_MANUAL_ANGLE`, `MANUAL_ANGLE_VALUE`
2. Fitted-edge distance and drawing extent:
   `USE_EXTEND_FIT_LINE`, `EXTEND_FIT_LINE_VALUE`
3. Persisted but currently inactive average-filter compatibility fields:
   `USE_AVERAGE_FILTER`, `AVERAGE_Diff`, `AVERAGE_FILTER_TYPE`
4. Legacy/current drawing-path boundaries:
   `SHOW_VERTICAL_LINE`, `SHOW_EDGE`, `SHOW_CONTOUR`, `SHOW_FITLINE`

The LineGauge/LineDistance runtime, Recipe/XML field names, explicit
Preview/Run contract, layer behavior, and Pipeline routing were not changed.

## Runtime-Grounded Semantics

### Manual angle

Manual angle does not rotate the edge-search profile and does not change
LineGauge edge fitting. `LineDistance` uses it to construct distance-sampling
lines from Line A. When disabled, the current path uses `POINT_RANGE` and
`VER_PRJ_DIR`. Changing the angle can therefore change intersection positions,
distance count, and the `DistancePx*` distribution without changing the
detected edge points.

### Extended fit line

Direct Line results use the option to extend the visible fitted-line segment.
`LineDistance` changes from local edge-point intersections to fitted-edge
distance only when both Line A and Line B enable
`USE_EXTEND_FIT_LINE`. Enabling only one side does not switch the pair.

`EXTEND_FIT_LINE_VALUE` changes drawing extent. The current distance
implementation intersects infinite fitted lines, so this value is not a
tolerance, calibration value, or measurement range and should not change
distance metrics when the fitted lines are unchanged.

### Average-filter compatibility fields

The three average-filter properties are saved by current PropertyGrid,
Recipe, and preset paths, but the Library-Noah LineGauge runtime interface and
edge-detection implementation do not consume them. They currently have no
filtering effect. The guide states this directly and tells operators not to
tune them as active detection controls.

### Drawing flags

The four drawing flags are honored by the legacy bitmap Draw path. The
current WPF direct Preview overlay and Pipeline Review retain edge/fit/distance
evidence and do not apply those flags. The guide distinguishes display from
algorithm behavior and explains that unchanged current drawings are a path
boundary, not a persistence failure.

## Actual EXE Evidence

The current Debug EXE was built before each capture and tested with the same
`920 x 660` Line Tool:

- Before: `USE_MANUAL_ANGLE` showed Basic fallback.
- After: the same property showed detailed runtime guidance.
- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

Evidence:

- `artifacts\p265_line_parameter_guide_20260730\actual_exe_before`
- `artifacts\p265_line_parameter_guide_20260730\actual_exe_after`

The sidecar remains beside the Tool and does not cover the PropertyGrid,
image viewers, Pipeline actions, layer selectors, or explicit Preview.

## Verification

- `p265_line_parameter_guide=OK`
- LineGauge/LineDistance detailed coverage: `36/36`
- Audited properties: `11/11`
- Shared contextual guide regression: passed
- Line Tool, preset, and LineDistance measurement regressions: passed
- Line Signal evidence regression: passed in isolated current-build execution
- Actual Debug EXE before/after non-obstruction: passed
- Standalone canonical audit:
  - Browsable: `318`
  - Detailed: `266`
  - Basic fallback: `52`

The first multi-target Line Signal capture could not find its overlay after
several prior nonmodal UI captures in the same process. The exact target
passed when run independently from the same current build. This was treated
as test-process isolation evidence, not hidden as a product pass.

Evidence:

`artifacts\p265_line_parameter_guide_20260730`

## Next Bounded Priority

The audit exposed a concrete operator-trust gap beyond guide text: seven
properties look active in the current PropertyGrid while either having no
runtime consumer (three average-filter fields) or affecting only a legacy
drawing path (four drawing flags).

The next bounded priority is to design and implement an explicit inactive/
legacy-control treatment that preserves old Recipe/Preset values without
pretending those values affect current execution. Do not connect drawing
toggles by hiding mandatory current-run review evidence, and do not invent an
average-filter algorithm without a separately approved runtime contract.

Recommended model: `gpt-5.6-sol` | Reasoning effort: `medium`.

CVR-00 remains incomplete and deferred until three independent first-time
participants and their raw observations exist.

## Completion Record

Status: Complete<br>
Scope: Detailed contextual guidance for the 11 formerly Basic
LineGauge/LineDistance properties, with verified runtime/legacy boundaries.<br>
Acceptance criteria: LineGauge/LineDistance 36/36 detailed in Korean/English;
manual angle separated from edge search; fitted-edge pair activation and
drawing-only extend length stated; inactive average fields and legacy drawing
flags disclosed; actual EXE guide remains non-obstructing; focus, hide/reopen,
and zero Preview/Run/layer/route effects preserved.<br>
Verification: Focused/shared guide smokes, Line Tool/preset/measurement/signal
regressions, actual Debug EXE before/after, standalone canonical audit, full
Debug solution build, and readiness.<br>
Evidence: `artifacts\p265_line_parameter_guide_20260730` and this report.<br>
Boundary / next dependency: This is operator guidance, not an average-filter
implementation, a change to current review drawings, Line metrology
qualification, field robustness, equipment integration, or CVR-00 participant
evidence.
