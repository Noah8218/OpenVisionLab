# OpenVisionLab AffineTransform Parameter Guide

Date: 2026-07-30 KST<br>
Priority: P267<br>
Status: Complete

## Outcome

The 20 formerly Basic `AffineTransform` entries now have runtime-grounded
Korean/English detailed guidance. `AffineTransform` is 38/38 detailed and the
standalone canonical audit is now 286/318 detailed with 32 Basic entries, all
in `EdgeBasedMatching`.

The admitted properties are grouped by the operator task they control:

1. Ordered source/destination correspondence:
   `SourcePoint1X/Y` through `SourcePoint3X/Y` and
   `DestinationPoint1X/Y` through `DestinationPoint3X/Y`
2. Output canvas:
   `OutputWidth`, `OutputHeight`
3. Sampling and border:
   `Interpolation`, `BorderType`, `BorderValue`
4. Fail-closed geometry and coverage:
   `MinimumSourceTriangleArea`,
   `MinimumDestinationTriangleArea`,
   `MinimumValidPixelRatio`

No Affine calculation, XML key, Recipe value, runtime gate, Preview/Run
contract, layer behavior, or Pipeline routing was changed.

## Runtime-Grounded Semantics

### Ordered correspondence

Source point 1 maps to destination point 1, and so on. The runtime does not
guess or reorder correspondences. Changing any coordinate can alter all six
matrix coefficients and therefore translation, rotation, X/Y scale, shear,
cropping, and downstream fixed-ROI position.

Coordinates are pixel-only. Points outside the image are allowed, so execution
success does not prove that the chosen physical features are correct. Wrong
ordering can produce a numerically valid mirrored or sheared result.

The Direct Tool uses the fixed numeric source coordinates. In Recipe detected-
Point mode, three earlier accepted Point features replace them at Run time.
The guide marks fixed source coordinates inactive in that mode.

### Output canvas

`0` retains the corresponding input width or height. A positive value selects
an explicit output dimension up to 32768. Changing the canvas does not rescale
source/destination coordinates. A small canvas can crop transformed content;
a large canvas can add border-filled area and reduce the valid-pixel ratio.

### Sampling and border

The Library-Noah runtime accepts only:

- interpolation: `Nearest`, `Linear`, `Cubic`, `Lanczos4`
- border: `Constant`, `Replicate`, `Reflect`, `Wrap`, `Reflect101`

Interpolation changes output pixel values and edge shape, not the affine
matrix. Border policies can create artificial Threshold/Blob/Contour/Edge
candidates. `BorderValue` applies only to `Constant`, uses one scalar for all
channels, and is shown as conditionally inactive for other border types.

### Fail-closed gates

Source and destination triangle-area gates use absolute pixel² area.
Collinear points remain invalid even when the configured minimum is zero.

`MinimumValidPixelRatio` compares the output canvas against a separately
warped source mask. Border fill does not count as valid source coverage.
A coverage failure retains the transformed image, metrics, and geometry
drawings for correction. The global ratio does not prove that one critical
downstream ROI remains uncut, so the guide requires visual ROI review.

## Operator Review Checklist

After changing an Affine parameter, explicitly Preview and check:

- point 1/2/3 physical identity and source-to-destination order;
- `AffineSourceTriangleArea` and `AffineDestinationTriangleArea`;
- all six `AffineM*` coefficients plus scale/rotation/shear review metrics;
- destination-point/triangle and transformed-source-frame drawings;
- `AffineValidPixelRatio` and border-generated artifacts;
- every downstream fixed ROI lies on real source content;
- representative sample repeatability before saving the Recipe.

## Actual EXE Evidence

The current Debug EXE used the same `920 x 660` Affine Tool before and after.

- Before: `SourcePoint1X` displayed Basic fallback guidance.
- After: the same row explains ordered correspondence, detected-Point
  replacement, matrix/drawing impact, and review checks.
- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

Evidence:

- `artifacts\p267_affine_transform_parameter_guide_20260730\actual_exe_before`
- `artifacts\p267_affine_transform_parameter_guide_20260730\actual_exe_after`

The sidecar remains beside the Tool and does not cover the PropertyGrid, image
viewers, layer selectors, Pipeline actions, or explicit Preview.

## Verification

- P267 focused smoke: `AuditedProperties=20/20`
- AffineTransform detailed coverage: `38/38`
- Direct Tool Korean/English selection and zero-side-effect checks: passed
- Recipe detected-Point fixed-coordinate applicability: passed
- Shared P257 contextual guide regression: passed
- P266 inactive/legacy Line regression: passed
- Existing Affine Tool explicit Preview/runtime route regression: passed
- Affine runtime contract:
  - aliases `AffineTransform`, `Affine`, `AffineMatrix`: passed
  - known matrix `1.2,0.25,12;-0.1,0.9,18`: passed
  - PropertyGrid/XML round trip: passed
  - collinear source fail-closed gate: passed
  - insufficient-coverage failure evidence: passed
- Standalone canonical audit:
  - Browsable: `318`
  - Detailed: `286`
  - Basic fallback: `32`

Evidence:

`artifacts\p267_affine_transform_parameter_guide_20260730`

## Next Bounded Priority

All remaining Basic entries belong to `EdgeBasedMatching`. Audit those 32
properties before implementation, separating:

1. template/source identity and drawing compatibility;
2. Auto MPoint training-time candidate controls;
3. Canny/contour/model-construction controls;
4. coarse/refine/pyramid/hybrid runtime search controls.

Do not treat one score or candidate rank as physical-feature identity, and do
not reopen image campaigns, automatic correspondence, Homography, calibration,
or LLM expansion during the guide audit.

Recommended model: `gpt-5.6-sol` | Reasoning effort: `medium`.

CVR-00 remains deferred until three independent first-time participants and
their raw observations exist.

## Completion Record

Status: Complete<br>
Scope: Runtime-grounded bilingual detailed guidance for the 20 formerly Basic
AffineTransform coordinates, output, sampling/border, and geometry/coverage
gate properties, including Recipe detected-Point applicability.<br>
Acceptance criteria: all 20 audited and AffineTransform 38/38 detailed;
ordered correspondence and pixel-only boundary stated; output zero/canvas
behavior stated; supported sampling/border policies and conditional
BorderValue stated; collinear and valid-source-coverage gates stated; actual
EXE remains non-obstructing; no Preview/Run/layer/route side effects.<br>
Verification: Focused/shared guide and Affine Tool regressions, known-matrix/
alias/round-trip/fail-closed runtime contract, actual Debug EXE before/after,
standalone canonical audit, full Debug build, and readiness.<br>
Evidence: `artifacts\p267_affine_transform_parameter_guide_20260730` and this
report.<br>
Boundary / next dependency: This is guidance for an existing deterministic
pixel-only Affine contract, not automatic correspondence, Homography,
calibration, certified metrology, unseen-data robustness, field qualification,
equipment integration, or CVR-00 participant evidence.
