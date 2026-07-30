# OpenVisionLab Mean Parameter Guide

Date: 2026-07-30 KST<br>
Priority: P262<br>
Status: Complete

## Outcome

The current canonical audit selected the three remaining Basic properties in
the existing Mean family:

- `MEAN_TYPES`
- `MEAN_MIN`
- `MEAN_MAX`

They now provide runtime- and workflow-grounded Korean/English guidance in the
shared non-obstructing Parameter Guide. The Direct Mean Tool binds its friendly
control names to the exact stable PropertyGrid/XML identities:

- `MeanType -> MEAN_TYPES`
- `MeanMin -> MEAN_MIN`
- `MeanMax -> MEAN_MAX`

## Why This Family Was Admitted

The three properties form one operator task: judge whether one ROI's brightness
or contrast statistic stays inside an acceptable band.

The prior Basic state had a material beginner risk:

- `Mean` reports average gray value;
- `MeanStdDev` reports gray-value standard deviation, so its meaning is
  contrast/spread rather than average brightness;
- the same Min/Max controls remain visible after switching the statistic;
- a limit suitable for Mean is not transferable to MeanStdDev;
- Direct Preview uses Min/Max for its local result review, while a saved
  Pipeline uses its separate Step acceptance contract for final judgment.

The guide now states those boundaries explicitly instead of describing every
number as a generic brightness threshold.

## Operator Guidance Contract

### Mean type

- Use `Mean` as a starting point for exposure or surface-brightness drift.
- Use `MeanStdDev` for uniformity, texture, or contrast variation.
- Changing the type changes the meaning of the result; retune Min/Max and any
  Pipeline acceptance gate.
- Verify Type and the measured value on the same ROI across Good/Bad and
  N-sample distributions.

### Decision minimum and maximum

- The bounds are inclusive and use GV units.
- They change Direct Preview OK/NG judgment, not the image or calculated
  statistic.
- Tune from Good-sample distribution and allowed process variation, not one
  image.
- For a saved Pipeline, separately verify
  `AcceptanceMetricName`, `AcceptanceMetricMinimum`, and
  `AcceptanceMetricMaximum`.

## UI Evidence

Before implementation, the actual current Debug EXE showed the Mean Tool with
no contextual guide:

`artifacts\p262_mean_parameter_guide_20260730\actual_exe_before`

After implementation, the actual current Debug EXE at `920 x 660` reported:

- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

After evidence:

`artifacts\p262_mean_parameter_guide_20260730\actual_exe_after`

The guide remains outside the Tool window and does not cover Mean Type,
minimum/maximum sliders, input/output images, Pipeline actions, N-image
verification, or explicit Preview.

## Verification

- `p262_mean_parameter_guide=OK`
- Mean detailed coverage: `3/3`
- Korean/English and GV units: passed
- Direct-control aliases resolve to exact stable property identities: passed
- Mean versus MeanStdDev semantic guidance: passed
- Direct Preview versus Pipeline acceptance boundary: passed
- P259, P260, and P261 shared-guide regressions: passed
- `wpf_preprocess_output_preview_flow=OK`
- `wpf_simple_preprocess_result_review=OK`
- Actual Debug EXE non-obstruction check: passed

The canonical fallback audit must run standalone because opening Tool
PropertyGrids first registers session visibility filters for their compact UI.
The independent current-source audit retained the full canonical set:

- Browsable: `318`
- Detailed: `244`
- Basic fallback: `74`

Evidence:

`artifacts\p262_mean_parameter_guide_20260730\audit_standalone\p260-parameter-guide-fallback-audit.tsv`

## Completion Record

Status: Complete<br>
Scope: Detailed contextual guidance and Direct Tool binding for the three Mean
family properties only.<br>
Acceptance criteria: Mean 3/3 detailed in Korean/English; Mean and MeanStdDev
semantics distinguished; inclusive GV bounds and Direct Preview/Pipeline
acceptance boundary stated; actual EXE guide does not obstruct teaching;
focus, explicit hide/reopen, and zero Preview/Run/layer/routing side effects
preserved.<br>
Verification: Focused and shared-guide UI smokes, Mean preprocessing/result
review regressions, actual Debug EXE before/after evidence, standalone
canonical audit, full Debug build, readiness, and patch hygiene.<br>
Evidence: `artifacts\p262_mean_parameter_guide_20260730` and this report.<br>
Boundary / next dependency: This is operator guidance, not automatic
threshold selection, Mean algorithm qualification, unseen-data robustness, or
field evidence. The remaining 74 Basic entries need another bounded-family
admission. CVR-00 still requires three independent first-time participants and
raw observations.
