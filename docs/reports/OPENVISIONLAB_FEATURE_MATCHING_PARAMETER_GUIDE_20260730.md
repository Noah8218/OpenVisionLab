# OpenVisionLab FeatureMatching Parameter Guide

Date: 2026-07-30 KST  
Priority: P263  
Status: Complete

## Outcome

The three remaining Basic properties in the existing FeatureMatching family
now have runtime-grounded Korean/English detailed guidance:

- `PATTERN_PATH`
- `SCORE_MIN`
- `RANSAC_REPROJ_THRESHOLD`

The existing PropertyGrid teaching workflow, template editor, explicit
Preview/Run, layers, routes, and FeatureMatching runtime were not changed.

## Why This Family Was Admitted

These properties form one operator task: locate a feature-rich taught target
through descriptor correspondence and geometrically consistent homography.

The prior Basic guide left one especially dangerous ambiguity:

- FeatureMatching `SCORE_MIN` is the Lowe descriptor ratio threshold;
- smaller values are stricter and retain fewer, more distinctive matches;
- larger values are more permissive and may retain ambiguous matches;
- `0.6` is not a 60% result acceptance score;
- the runtime result `Score`/`ScoreMax` is separately calculated as
  `RANSAC inliers / GoodMatches * 100`.

Without that distinction, an operator could tune in the opposite direction or
compare values from incompatible scales.

## Guidance Contract

### Feature template path

- The file must exist and be readable or FeatureMatching fails before
  execution.
- Use a compact crop containing stable corners and texture with little
  unrelated background.
- Flat or repeated patterns increase missing features, ambiguity, and wrong
  homography risk.
- Active common preprocessing is applied to both template and input, so
  preprocessing changes require revalidation.
- Check template readiness, Template/Source keypoints, GoodMatches,
  transformed quadrilateral, ScoreMax, and Good/Bad plus N-sample replay.

### Ratio threshold

- Runtime keeps a descriptor pair only when:
  `nearest distance < SCORE_MIN * second-nearest distance`.
- Smaller is stricter; larger is more permissive.
- At least four GoodMatches are required before homography.
- Tune after freezing a feature-rich template and search ROI.
- Keep the ratio distinct from result ScoreMax, which is on a 0..100 scale.

### RANSAC tolerance

- This is the maximum reprojection error for treating a match as an inlier.
- The unit is pixels.
- Smaller values reject geometric mismatch more strictly but may fail on
  normal feature-location variation.
- Larger values may raise inlier count and ScoreMax while allowing distorted
  or wrong transforms.
- Judge transformed quadrilateral, center, size, angle, and ScoreMax together.

## UI Evidence

Before implementation, the current Debug EXE showed `SCORE_MIN` through Basic
fallback:

`artifacts\p263_feature_matching_parameter_guide_20260730\actual_exe_before`

After implementation, the actual current Debug EXE at `920 x 660` reported:

- `GuideCoverage: 상세 안내`
- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

After evidence:

`artifacts\p263_feature_matching_parameter_guide_20260730\actual_exe_after`

The guide remains outside the Tool and does not cover the PropertyGrid,
template teaching status, input/output images, Pipeline/N-image actions, or
explicit Preview.

## Verification

- `p263_feature_matching_parameter_guide=OK`
- FeatureMatching detailed coverage: `3/3`
- Korean/English guidance: passed
- `SCORE_MIN` ratio direction and 0..1 unit: passed
- Result Score 0..100 separation: passed
- RANSAC unit corrected to `px`, not generic threshold `GV`: passed
- Template/keypoint/GoodMatches/drawing evidence guidance: passed
- `p257_contextual_parameter_guide=OK`
- `wpf_shell_host_feature_matching_tool=OK`
- `wpf_shell_host_recipe_guided_setup=OK`
- Actual Debug EXE non-obstruction check: passed

The standalone canonical audit is now:

- Browsable: `318`
- Detailed: `247`
- Basic fallback: `71`

Evidence:

`artifacts\p263_feature_matching_parameter_guide_20260730\audit_standalone\p260-parameter-guide-fallback-audit.tsv`

## Completion Record

Status: Complete  
Scope: Detailed contextual guidance for the three FeatureMatching-specific
properties and correct unit presentation only.  
Acceptance criteria: FeatureMatching 3/3 detailed in Korean/English; Lowe
ratio and result Score scales explicitly separated; RANSAC tolerance shown in
pixels; template and geometry evidence named; actual EXE guide remains
non-obstructing; focus, explicit hide/reopen, and zero Preview/Run/layer/route
side effects preserved.  
Verification: Focused guide smoke, shared PropertyGrid guide regression,
actual FeatureMatching template/Preview/Pipeline round trip, Guided Setup,
actual Debug EXE before/after evidence, standalone canonical audit, full Debug
build, readiness, and patch hygiene.  
Evidence: `artifacts\p263_feature_matching_parameter_guide_20260730` and this
report.  
Boundary / next dependency: This is operator guidance, not automatic template
selection, parameter optimization, FeatureMatching qualification, unseen-data
robustness, or field evidence. The remaining 71 Basic entries require another
bounded-family admission. CVR-00 still requires three independent first-time
participants and raw observations.

