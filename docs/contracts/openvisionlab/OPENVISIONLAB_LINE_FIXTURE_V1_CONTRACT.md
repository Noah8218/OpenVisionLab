# OpenVisionLab LineFixture v1 Contract

Updated: 2026-07-28 KST

## Status

The bounded implementation and synthetic integration slice is complete.
Qualification on a named physical part remains blocked until the operator
supplies the part/task packet described below.

## Purpose

`LineFixture` publishes one pixel-space fixture frame from two exact typed
`Segment` results produced by earlier accepted `Line`/`LineGauge` Steps.
It reuses the existing Line detector and existing Fixture/`NormalizeImage`
consumer. It is not another edge detector.

The intended operator workflow is:

```text
review and teach Datum A Line
  -> review and teach Datum B Line
  -> explicit Run
  -> LineFixture validates both retained Segment results
  -> intersection publishes the fixture origin
  -> Datum A publishes the fixture X-axis angle
  -> existing NormalizeImage or relative-ROI consumer applies the frame
  -> downstream fixed reference-coordinate inspection runs
```

## XML And Pipeline Contract

Canonical `ToolType`:

- `LineFixture`

Accepted compatibility alias:

- `DualEdgeFixture`

Required source identity:

- `SourceStepA` and `SourceFeatureA`;
- `SourceStepB` and `SourceFeatureB`;
- each source must be one distinct, earlier, enabled, successful, accepted
  `Segment`;
- each source Step must be `Line` or `LineGauge`; the operator teaches
  polarity and contrast in those owning Line Steps, and their execution plus
  acceptance gates must pass before `LineFixture` can consume the results;
- both source results must use the same input layer, image dimensions, and
  coordinate frame as `LineFixture`.

Fixture publication uses the existing keys:

- `USE_AS_FIXTURE_FRAME=true`;
- `FIXTURE_FRAME_NAME`;
- `FIXTURE_REFERENCE_X`;
- `FIXTURE_REFERENCE_Y`;
- `FIXTURE_REFERENCE_ANGLE`;
- `FIXTURE_REFERENCE_SCALE=1`;
- `FIXTURE_MAX_ANGLE_DELTA`;
- `FIXTURE_MIN_SCALE_RATIO=1`;
- `FIXTURE_MAX_SCALE_RATIO=1`;
- `FIXTURE_REFERENCE_IMAGE_WIDTH` and
  `FIXTURE_REFERENCE_IMAGE_HEIGHT` when `NormalizeImage` is used.

v1 datum gates:

- `MIN_SUPPORT_A`, `MIN_SUPPORT_B`;
- `MAX_FIT_RESIDUAL_A_PX`, `MAX_FIT_RESIDUAL_B_PX`;
- `MIN_INCLUDED_ANGLE_DEG`, `MAX_INCLUDED_ANGLE_DEG`, with a maximum of
  90 degrees;
- `MAX_EXTENSION_A_PX`, `MAX_EXTENSION_B_PX`.

## Pose And Coordinate Convention

- The infinite-line intersection is the fixture origin.
- Datum A defines the undirected X axis. Its orientation is chosen as the
  equivalent direction nearest the taught reference angle.
- Line geometry is recorded in image coordinates, where Y increases downward.
  Fixture angle follows the existing OpenCV/`NormalizeImage` convention,
  where positive angle is counter-clockwise. `LineFixture` performs this sign
  conversion explicitly.
- v1 scale is always exactly `1`. Two lines do not prove uniform scale.
- Datum B verifies the second physical boundary and included angle. It does not
  silently redefine Datum A as the X axis.

## Metrics And Drawings

Successful execution publishes:

- `ResultCount=1`;
- `FixtureCenterX`, `FixtureCenterY`;
- `FixtureAngle`, `FixtureScale=1`;
- `FixtureOffsetX`, `FixtureOffsetY`, `FixtureAngleDelta`,
  `FixtureScaleRatio=1`;
- `FixtureLineASupportCount`, `FixtureLineBSupportCount`;
- `FixtureLineAFitResidualPx`, `FixtureLineBFitResidualPx`;
- `FixtureIncludedAngleDeg`;
- `GeometryExtensionAPx`, `GeometryExtensionBPx`;
- reference image dimensions when taught.

The current-run result retains:

- the exact Datum A and Datum B segments;
- the intersection/origin mark;
- the fixture X and Y axes;
- pass or exact reject text.

It also publishes the origin as typed geometry feature `Origin/Point`.

## Fail-Closed Rules

Execution or definition validation rejects:

- missing, later, disabled, failed, rejected, duplicate, or ambiguous source
  identity;
- a non-`Segment` source;
- cross-layer, cross-frame, or different-size source results;
- non-finite, degenerate, or out-of-image segment coordinates;
- support below the configured minimum;
- a source that is not an actual `Line`/`LineGauge` producer; source polarity
  or contrast rejection remains the owning Line Step's exact failure;
- fit residual above the configured maximum;
- parallel or out-of-range included angle;
- intersection extension beyond either configured limit;
- an out-of-image intersection;
- missing/non-finite taught reference pose;
- Fixture name conflicts or incompatible consumer configuration.

Runtime datum/geometry-gate rejects retain available datum drawings, metrics,
and the exact reason. Definition failures do not execute and therefore create
no current-run drawing. Neither path publishes a usable Fixture frame.

## PropertyGrid And Side Effects

Recipe Manager selected-Step PropertyGrid owns:

- typed Datum A/B pickers;
- frame name and taught reference pose/image size;
- angle-delta, support, residual, included-angle, and extension gates.

Load, selection, editing, and apply/save do not execute Preview or Run, create
or select layers, or change input/output routes. Runtime application remains
an explicit Run behavior.

## Bounded Verification

The frozen synthetic matrix uses eight cases:

- four translation/reference cases;
- two `+/-3 deg` rotation cases;
- two `+/-2 deg` cases with internal repeated horizontal and vertical rails.

All eight actual `LineGauge -> LineFixture -> NormalizeImage -> fixed-ROI Mean`
runs passed. Support remained `50` for Datum A and `36..37` for Datum B;
included angle remained `89.594..90 deg`; residuals remained `0..1.344 px`;
valid normalized coverage remained `0.923..0.995`; the fixed pad ROI retained
mean `185.4..192.2`. Duplicate source identity and a valid-but-incompatible
included-angle gate failed closed.

Evidence:

- `artifacts/cvr09_line_fixture_20260728_r11`;
- `docs/reports/OPENVISIONLAB_CVR09_LINE_FIXTURE_20260728.md`.

## Qualification Prerequisite And Boundary

This evidence is synthetic pixel-space integration evidence. It does not prove
polarity variation, scale, perspective, calibration, certified metrology,
unseen-data robustness, production variation, or field qualification.

Before calling CVR-09 qualified for a physical task, obtain:

- the named part and inspection intent;
- representative images;
- operator-certified Datum A and Datum B physical identities;
- allowed pose range;
- polarity/contrast expectations;
- the downstream ROI or measurement intent;
- evidence that a durable Matching/Affine locator is unsuitable;
- an N-sample review proving reflections, repeated rails, and nearby parallel
  boundaries do not replace either intended datum.

## Completion Record

```text
Status: Complete
Scope: Bounded LineFixture v1 runtime, validation, typed-result wiring, existing Fixture/NormalizeImage consumption, PropertyGrid editing, Pipeline Review quality presentation, XML round trip, and eight-case synthetic integration.
Acceptance criteria: Two distinct accepted Line Segments publish one gated pixel fixture; existing NormalizeImage consumes it; fixed reference ROI survives translation/rotation/distractors; invalid angle and duplicate source fail closed; PropertyGrid apply has zero Preview/Run side effects.
Verification: Debug solution and focused tool builds passed with zero warnings/errors; OpenVisionFixtureSmoke --cvr09-line-fixture passed 8/8 plus fail-closed and round-trip checks; cvr09_line_fixture_property_grid passed check/layout/text/internal guards.
Evidence: artifacts/cvr09_line_fixture_20260728_r11 and docs/reports/OPENVISIONLAB_CVR09_LINE_FIXTURE_20260728.md
Boundary / next dependency: Named physical-part qualification remains blocked on the operator packet and reviewed N-sample evidence listed above.
```
