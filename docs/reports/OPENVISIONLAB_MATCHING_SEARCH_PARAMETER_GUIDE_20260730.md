# OpenVisionLab Matching Search Parameter Guide

Date: 2026-07-30 KST  
Priority: P264  
Status: Complete

## Outcome

The eight remaining Basic entries in `Matching` now have runtime-grounded
Korean/English detailed guidance. They are deliberately presented as four
separate responsibilities rather than one undifferentiated advanced group:

1. Working resolution: `MAGNIFIATION`
2. Coarse-to-fine angle search:
   `USE_COARSE_TO_FINE_ANGLE_SEARCH`, `COARSE_ANGLE_STEP`,
   `COARSE_ANGLE_TOP_K`
3. Pyramid position proposal:
   `USE_PYRAMID_POSITION_PROPOSAL`, `PYRAMID_POSITION_TOP_N`,
   `PYRAMID_POSITION_MIN_SCORE`
4. Rotated-template border policy: `USE_PADDING_COLOR_WHITE`

The Matching runtime, PropertyGrid fields, template workflow, explicit
Preview/Run, layers, routes, and saved XML names were not changed.

## Runtime-Grounded Semantics

### Working-resolution divisor

`MAGNIFIATION` divides both source and scaled-template width/height for the
first search. It is not the target size-variation range. Values above `1`
reduce work but can erase small features; `1` keeps original working
resolution. Non-positive or size-empty configurations fail closed. The
misspelled property name remains unchanged for Recipe/XML compatibility.

### Coarse-to-fine angle search

The opt-in path first searches the entire angle range using the larger coarse
step, retains the best K angle candidates, and then searches their
neighborhoods using the fine `FIND_ANGLE` step. It runs only when angle search
is enabled and the coarse step is greater than the fine step. The guide
therefore treats it as a measured speed/coverage trade-off, not an automatic
accuracy improvement.

### Pyramid position proposal

This opt-in path proposes up to N positions per scale on a smaller image and
verifies them at the original working resolution. It is currently used only
when angle search is off. No surviving proposal or failed verification returns
to the existing full search. `PYRAMID_POSITION_MIN_SCORE` is a separate 0..1
proposal gate, not final `SCORE_MIN` or `ScoreMax`.

### Rotated-template border

`USE_PADDING_COLOR_WHITE=true` fills rotated-template exterior pixels with
constant white `255`. `false` uses reflected template borders, not black
padding. The guide asks operators to compare real rotated OK/NG samples because
either choice can introduce artificial corner structure.

## Actual EXE Evidence

The current Debug EXE was built before each capture and tested with the same
`920 x 660` Matching Tool:

- Before: selected Coarse angle search showed Basic fallback.
- After: the same property showed detailed guidance.
- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

Evidence:

- `artifacts\p264_matching_search_parameter_guide_20260730\actual_exe_before`
- `artifacts\p264_matching_search_parameter_guide_20260730\actual_exe_after`

The sidecar remains beside the Tool and does not cover the PropertyGrid,
template status, image viewers, Pipeline/N-image actions, or explicit Preview.

## Verification

- `p264_matching_search_parameter_guide=OK`
- Matching detailed coverage: `42/42`
- Audited properties: `8/8`
- Shared contextual guide regression: passed
- FeatureMatching P263 regression: passed
- Matching pyramid PropertyGrid and angle runtime regressions: passed
- Matching Tool explicit Preview regression: passed in isolated current-source
  execution
- Matching presets regression: passed in isolated current-source execution
- Actual Debug EXE before/after non-obstruction: passed
- Full Debug solution build: zero warnings and zero errors
- OpenVisionLab readiness: all 12 categories passed

The verification harness was corrected to exclude the nonmodal Parameter Guide
sidecar from “active Tool window” lookup. Its Matching preset assertion was
also aligned with the current localized applied-detail format (`기본: ... 값만
바뀌며 미리보기로 검증하세요`). These are evidence-harness corrections and
do not alter product runtime behavior.

The standalone canonical audit is now:

- Browsable: `318`
- Detailed: `255`
- Basic fallback: `63`

Evidence:

`artifacts\p264_matching_search_parameter_guide_20260730`

## Completion Record

Status: Complete  
Scope: Detailed contextual guidance for the eight formerly Basic Matching
properties, plus accurate active-Tool smoke lookup in the presence of the
nonmodal guide sidecar.  
Acceptance criteria: Matching 42/42 detailed in Korean/English; working
resolution separated from target scale; coarse angle and pyramid activation/
fallback conditions stated; padding false identified as Reflect; actual EXE
guide remains non-obstructing; focus, hide/reopen, and zero Preview/Run/layer/
route effects preserved.  
Verification: Focused and shared guide smokes, Matching pyramid/angle/Tool/
preset regressions, actual Debug EXE before/after, standalone canonical audit,
full Debug solution build, and readiness.  
Evidence:
`artifacts\p264_matching_search_parameter_guide_20260730` and this report.  
Boundary / next dependency: This is operator guidance, not automatic search
configuration, Matching qualification, unseen-data robustness, or field
evidence. The next bounded priority is to audit the 11 remaining
`LineGauge/LineDistance` Basic entries and separate algorithm controls from
drawing-only toggles before admitting any implementation. CVR-00 still
requires three independent first-time participants and raw observations.
