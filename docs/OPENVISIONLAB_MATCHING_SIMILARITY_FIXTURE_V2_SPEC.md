# OpenVisionLab Matching Similarity Fixture V2 Spec

Status: Bounded C9/P175 implementation complete through P183 on 2026-07-21. P179 completes Matching pose/scale Pipeline/XML publication, P181 adds reviewed dimensions and fail-closed full-image inverse-similarity `NormalizeImage`, P182 qualifies the exact 24-row coordinate/drawing path through the unchanged reviewed `LineDistance` ROI, and P183 freezes the C9 pre-measurement score/ambiguity/pose/coverage gate with deliberate failures. Broader/all-500/unseen robustness and black-strip judgement are not complete.

## Purpose

This specification defines the smallest pose-normalization workflow needed to measure the vertical thickness of the operator-selected black strip when the part moves, rotates, or changes image scale.

It extends the proven translation-only Matching fixture concept without turning OpenVisionLab into a generic affine-coordinate framework. The bounded workflow is:

```text
Main image
  -> Matching locates one reviewed rigid reference feature
  -> RotateScale normalizes the current image back to the taught reference pose
  -> LineDistance measures one unchanged reference-coordinate ROI
```

This remains an offline, explicit-run recipe workflow. It does not add a camera, calibration system, PLC/I/O, deployment, or automatic inspection loop.

## P173 Decision

The fixed-acquisition option is rejected for the supplied `device_top_left` corpus.

A deterministic 24-image audit sampled four OK and four NG images from each Train, Validation, and Test list. The reviewed audit overlays observed:

| Quantity | Minimum | Median | Maximum |
| --- | ---: | ---: | ---: |
| Strip center Y | 45.55 px | 174.95 px | 361.23 px |
| Strip angle | -2.544 deg | 0.255 deg | 2.154 deg |
| Visible strip length | 435 px | 557.5 px | 640 px |
| Observed outer thickness | 36.70 px | 45.63 px | 78.66 px |

The 315.68 px center-Y range rules out one fixed narrow ROI. The 4.70 degree angle span and 2.14:1 observed thickness ratio also mean that X/Y-only ROI translation is not an adequate final metrology contract. All sampled strips touch the left image boundary, so the strip itself does not provide an uncensored X-position or physical-length reference.

These are audit-heuristic pixel observations, not runtime tool results, calibrated dimensions, or OK/NG ground truth.

## Why Image Normalization Is Required

The current v1 fixture moves only `CvROI.X/Y`. After P179 it retains and publishes Matching center, angle, scale, and scale ratio, but still applies only X/Y translation to a downstream ROI. `LineDistance` also stores left/right gauge ROIs separately in the product editor.

V2 must not rotate an axis-aligned ROI and replace it with the enclosing axis-aligned bounding box. That box includes unrelated pixels and can change the selected edges. Instead, V2 applies the inverse Matching similarity transform to the image and produces a reference-coordinate layer. The existing axis-aligned `LineDistance` measurement then runs against its unchanged taught ROI on that layer.

## Required Operator Inputs

Full V2 completion and dataset qualification cannot finish until these inputs exist. The bounded Pipeline/XML round-trip slice may start with items 1-4 and 7 resolved, while every unresolved item remains a blocker for its dependent runtime or qualification slice:

1. One reference image and its reviewed image size.
2. One reviewed Matching template ROI on rigid device geometry that remains visible across the intended pose range.
3. Confirmation that the locator feature is not the strip edge whose thickness is being judged, unless using the target itself is an intentional and documented compromise.
4. One reference Matching center, angle, and scale captured after an explicit successful Review.
5. One reviewed `LineDistance` ROI on the normalized reference image. For the P172 reference this is `20,200,510,60`, subject to operator confirmation after normalization.
6. Allowed Matching score, angle delta, and scale-ratio ranges.
7. Pixel-only versus calibrated-unit intent. P173 remains pixel-only.
8. If judgement is required later: nominal thickness, tolerance, and independently justified labels. The current corpus OK/NG labels are not black-strip thickness truth.

P174 narrowed the locator decision to the exact operator-marked `device_top_left_OK_0001.jpg` reference and `C9 = 240,270,65,60`. The user approved that one locator for native qualification. The earlier `P0 = 130,260,200,35` remains rejected because `NG_0248` produced a roughly 274-300 px wrong-region match when a defect overlapped the locator.

P175 then ran the actual current EXE Matching Tool View with one explicit Preview per case. Synthetic angle/scale semantics passed `3/3`, and the exact observed P174 set passed `24/24`: minimum score `80.358`, maximum center error `2.032 px`, minimum polygon IoU `0.895`, maximum scale error `0.05691`, and maximum local strip-angle error `0.92995 deg`. All 24 drawings selected the intended C9 joint. The native reference result is center `(272,300)`, angle `0 deg`, and scale `1` on the 640x480 reference.

This resolves locator choice and current Tool View feasibility only. P175 used `SCORE_MIN=0` for evidence collection and therefore does not establish an operating score threshold. The exact search bounds `angle=-5..+5 deg, step=1` and `scale=0.8..1.9, step=0.1` cover the observed set; unseen robustness and fail-closed score/ambiguity limits remain to be proven.

P179 then used the operator-approved P178 tight Die Pad template to prove the common pose plumbing independently of C9. Existing Matching angle/scale properties now round-trip through builder, PropertyGrid, XML save/load/apply-back, and the app tool factory. Explicit current-EXE XML runs publish `FixtureCenterX/Y`, `FixtureAngle`, `FixtureScale`, and `FixtureScaleRatio`; the three exact P178 source rows reproduced native Tool View center/angle/scale results at `0.90/+3 deg`, `0.95/-2.5 deg`, and `1.15/+2.5 deg`. Reference teach saves scale and leaves Preview/Run explicit.

P181 completes the next bounded slice. Reference teach now also saves the reviewed input width/height. A same-source `RotateScale` branch with `FIXTURE_APPLY_MODE=NormalizeImage` consumes the Matching frame, applies the inverse center/angle/uniform-scale transform to a new reference-sized layer, publishes valid-coverage and applied-transform metrics, and fails closed on missing dimensions, source-size mismatch, invalid pose/scale/angle, ROI/masks, or insufficient valid pixels. Fixed `RotateScale` behavior remains unchanged when fixture mode is off. Current-build synthetic identity, `-5/+5 deg`, and `0.8/1.2` scale boundary cases passed with reviewed-region mean absolute differences `0 / 2.225 / 2.208 / 2.990 / 2.016`; current-run Matching and normalized drawings are under `artifacts\p181_matching_similarity_normalize_image_20260721\runtime\similarity_normalization`. This proves only the normalization slice, not C9 strip measurement.

P182 completes the bounded coordinate-correct measurement slice. It replays the exact P175 24 rows with the frozen C9 producer, P181 normalization, and unchanged P172 ROI `20,200,510,60` on `DeviceAligned`. The existing `USE_EXTEND_FIT_LINE=true` pair mode measures intersections against the two fitted strip edges and discards endpoints outside the reviewed ROI. All 24 normalized rows produced ROI-valid runtime measurements and retained source, clean normalized image, per-Step overlays, XML, and hashes; the identical raw-coordinate control executed only 18/24. Normalized `DistancePxAvg` was `38.5..50.5`, maximum `DistancePxRange` was `23`, minimum Matching score was `80.367`, and minimum valid-pixel ratio was `0.309`. These values describe the observed evidence set; they are not acceptance thresholds or black-strip OK/NG truth.

P183 completes the bounded pre-measurement operating gate. A separate `NUM_MATCH=2` preflight with `SCORE_MIN=0.8` requires `ScoreMargin >= 10` percentage points before the existing `NUM_MATCH=1` fixture producer. The producer requires absolute angle delta `<= 5.25` degrees and scale ratio `0.8..1.8`; NormalizeImage requires valid-pixel ratio `>= 0.25`. The exact observed rows passed `24/24`; deliberate no-target, exact-duplicate ambiguity, 8-degree angle, 1.9x scale, and `0.227` coverage cases failed at the intended Steps with runtime drawings. Diagnostic angle/coverage XML widens only the upstream search/bound needed to reach each downstream gate and is not operating policy.

For the bounded initial implementation, required inputs 1-7 are now resolved through P183. P182's current-run drawings confirm that `20,200,510,60` selects the intended two strip edges after normalization on the exact observed 24 rows, and P183 supplies the C9/P175 starter gate. Item 8 remains out of scope until independent black-strip tolerance labels exist; all-500 and unseen robustness also remain separate.

## Proposed XML Shape

The following is the bounded four-Step C9/P175 authoring shape through P183. It remains measurement-only because black-strip tolerance labels, calibration, all-500, and broader robustness are not qualified.

```xml
<VisionPipeline>
  <Name>DeviceTopLeft_BlackStrip_NormalizedGap</Name>
	  <Steps>
	    <Step>
	      <Name>01 Reject Missing Or Ambiguous C9</Name>
	      <ToolType>Matching</ToolType>
	      <Enabled>true</Enabled>
	      <InputLayer>Main</InputLayer>
	      <OutputLayer>CandidateAudit</OutputLayer>
	      <Parameters>
	        <Parameter><Key>TemplatePath</Key><Value>templates\device_pose_locator.png</Value></Parameter>
	        <Parameter><Key>SCORE_MIN</Key><Value>0.8</Value></Parameter>
	        <Parameter><Key>NUM_MATCH</Key><Value>2</Value></Parameter>
	        <Parameter><Key>USE_FIND_ANGLE</Key><Value>true</Value></Parameter>
	        <Parameter><Key>FIND_ANGLE_MIN</Key><Value>-5</Value></Parameter>
	        <Parameter><Key>FIND_ANGLE_MAX</Key><Value>5</Value></Parameter>
	        <Parameter><Key>USE_FIND_SCALE</Key><Value>true</Value></Parameter>
	        <Parameter><Key>FIND_SCALE_MIN</Key><Value>0.8</Value></Parameter>
	        <Parameter><Key>FIND_SCALE_MAX</Key><Value>1.9</Value></Parameter>
	        <Parameter><Key>FIND_SCALE_STEP</Key><Value>0.1</Value></Parameter>
	      </Parameters>
	      <UseAcceptance>true</UseAcceptance>
	      <ExpectedSuccess>true</ExpectedSuccess>
	      <AcceptanceMetricName>ScoreMargin</AcceptanceMetricName>
	      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
	      <AcceptanceMetricMinimum>10</AcceptanceMetricMinimum>
	    </Step>
	    <Step>
	      <Name>02 Publish Bounded Device Pose</Name>
	      <ToolType>Matching</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>DevicePoseMatch</OutputLayer>
      <Parameters>
	        <Parameter><Key>TemplatePath</Key><Value>templates\device_pose_locator.png</Value></Parameter>
	        <Parameter><Key>SCORE_MIN</Key><Value>0.8</Value></Parameter>
	        <Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter>
        <Parameter><Key>USE_FIND_ANGLE</Key><Value>true</Value></Parameter>
        <Parameter><Key>FIND_ANGLE_MIN</Key><Value>-5</Value></Parameter>
        <Parameter><Key>FIND_ANGLE_MAX</Key><Value>5</Value></Parameter>
        <Parameter><Key>FIND_ANGLE</Key><Value>1</Value></Parameter>
        <Parameter><Key>USE_FIND_SCALE</Key><Value>true</Value></Parameter>
        <Parameter><Key>FIND_SCALE_MIN</Key><Value>0.8</Value></Parameter>
        <Parameter><Key>FIND_SCALE_MAX</Key><Value>1.9</Value></Parameter>
        <Parameter><Key>FIND_SCALE_STEP</Key><Value>0.1</Value></Parameter>
        <Parameter><Key>USE_AS_FIXTURE_FRAME</Key><Value>true</Value></Parameter>
        <Parameter><Key>FIXTURE_FRAME_NAME</Key><Value>DeviceFrame</Value></Parameter>
        <Parameter><Key>FIXTURE_REFERENCE_X</Key><Value>operator-reviewed</Value></Parameter>
        <Parameter><Key>FIXTURE_REFERENCE_Y</Key><Value>operator-reviewed</Value></Parameter>
        <Parameter><Key>FIXTURE_REFERENCE_ANGLE</Key><Value>operator-reviewed</Value></Parameter>
        <Parameter><Key>FIXTURE_REFERENCE_SCALE</Key><Value>operator-reviewed</Value></Parameter>
        <Parameter><Key>FIXTURE_REFERENCE_IMAGE_WIDTH</Key><Value>640</Value></Parameter>
        <Parameter><Key>FIXTURE_REFERENCE_IMAGE_HEIGHT</Key><Value>480</Value></Parameter>
	        <Parameter><Key>FIXTURE_MAX_ANGLE_DELTA</Key><Value>5.25</Value></Parameter>
	        <Parameter><Key>FIXTURE_MIN_SCALE_RATIO</Key><Value>0.8</Value></Parameter>
	        <Parameter><Key>FIXTURE_MAX_SCALE_RATIO</Key><Value>1.8</Value></Parameter>
	      </Parameters>
	    </Step>
	    <Step>
	      <Name>03 Normalize Device Pose</Name>
      <ToolType>RotateScale</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>DeviceAligned</OutputLayer>
      <Parameters>
        <Parameter><Key>ALLOW_BRANCH_INPUT</Key><Value>true</Value></Parameter>
        <Parameter><Key>USE_FIXTURE_FRAME</Key><Value>true</Value></Parameter>
        <Parameter><Key>FIXTURE_FRAME_NAME</Key><Value>DeviceFrame</Value></Parameter>
        <Parameter><Key>FIXTURE_APPLY_MODE</Key><Value>NormalizeImage</Value></Parameter>
        <Parameter><Key>FIXTURE_MIN_VALID_PIXEL_RATIO</Key><Value>0.25</Value></Parameter>
        <Parameter><Key>Interpolation</Key><Value>Linear</Value></Parameter>
        <Parameter><Key>BorderType</Key><Value>Constant</Value></Parameter>
      </Parameters>
    </Step>
	    <Step>
	      <Name>04 Measure Black Strip Thickness</Name>
      <ToolType>LineDistance</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>DeviceAligned</InputLayer>
      <OutputLayer>BlackStripGap</OutputLayer>
      <Parameters>
        <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
        <Parameter><Key>CvROI</Key><Value>20,200,510,60</Value></Parameter>
        <Parameter><Key>PIXELPERMM</Key><Value>0</Value></Parameter>
        <Parameter><Key>LeftPRJ_DIR</Key><Value>Y_TTOB</Value></Parameter>
        <Parameter><Key>RightPRJ_DIR</Key><Value>Y_BTOT</Value></Parameter>
        <Parameter><Key>PRJ_PORALITY</Key><Value>WTOB</Value></Parameter>
        <Parameter><Key>CONTRAST</Key><Value>18</Value></Parameter>
        <Parameter><Key>THICKNESS</Key><Value>2</Value></Parameter>
        <Parameter><Key>SAMPLING_STEP</Key><Value>8</Value></Parameter>
        <Parameter><Key>POINT_RANGE</Key><Value>12</Value></Parameter>
        <Parameter><Key>VER_PRJ_DIR</Key><Value>Y_BTOT</Value></Parameter>
        <Parameter><Key>USE_MANUAL_ANGLE</Key><Value>true</Value></Parameter>
        <Parameter><Key>MANUAL_ANGLE_VALUE</Key><Value>0</Value></Parameter>
        <Parameter><Key>USE_EXTEND_FIT_LINE</Key><Value>true</Value></Parameter>
        <Parameter><Key>EXTEND_FIT_LINE_VALUE</Key><Value>100</Value></Parameter>
      </Parameters>
    </Step>
  </Steps>
</VisionPipeline>
```

The final implementation must use the actual `LineDistance` left/right PropertyGrid serialization contract and preserve it on round trip. It must reject incompatible per-gauge ROI overrides rather than appearing to normalize geometry that remains in source coordinates.

## Similarity Transform

For a reference pose `(Cref, Aref, Sref)` and a current pose `(Ccur, Acur, Scur)`:

```text
angleDelta = normalize(Acur - Aref)
scaleRatio = Scur / Sref

pCurrent = Ccur + scaleRatio * R(angleDelta) * (pReference - Cref)
pReference = Cref + (1 / scaleRatio) * R(-angleDelta) * (pCurrent - Ccur)
```

The normalization Step uses the inverse equation and writes a canvas in the taught reference coordinate system. V2 initially requires the current source dimensions to equal the taught reference dimensions and supports one uniform scale value. Perspective correction, anisotropic scale, homography, and generic frame chaining remain out of scope.

## Runtime And Layer Contract

- The ambiguity preflight requests exactly two candidates and must pass its score-margin acceptance before pose publication.
- The fixture Matching Step must return exactly one accepted pose.
- Matching, normalization, and measurement execute only after explicit Preview or Run.
- Matching reads `Main`; normalization intentionally branches back to the same unannotated `Main` source.
- Normalization creates `DeviceAligned` without changing the selected input layer or any saved route.
- `LineDistance` reads only `DeviceAligned` and keeps its saved reference-coordinate ROI unchanged.
- The executor must not rewrite serialized Matching reference values, the saved LineDistance ROI, or input/output layer names.
- A normalized output is a new image layer, not an implicit display-only coordinate conversion.

## Required Metrics

Matching/fixture evidence:

- `FixtureCenterX`, `FixtureCenterY`, `FixtureAngle`
- `FixtureScale`
- `FixtureOffsetX`, `FixtureOffsetY`, `FixtureAngleDelta`
- `FixtureScaleRatio`
- Matching score and one-match count

Normalization evidence:

- `FixtureReferenceImageWidth`, `FixtureReferenceImageHeight`
- `FixtureNormalizedImageWidth`, `FixtureNormalizedImageHeight`
- `FixtureValidPixelRatio`
- applied center, angle, and scale values

Measurement evidence remains the existing `LineDistance` pixel metrics, including `DistancePxMin`, `DistancePxMax`, `DistancePxAvg`, and `DistancePxRange`. No millimetre metric may be used until calibration is independently verified.

## Fail-Closed Rules

The run must stop with a structured failure when any of these conditions applies:

- Matching produces zero, multiple, below-score, or ambiguous poses.
- the reference center, angle, scale, or reference image dimensions are missing or invalid;
- the current Matching scale is unavailable, non-positive, or outside the reviewed ratio range;
- angle delta is outside the reviewed range;
- the current input dimensions differ from the taught reference dimensions in v2;
- the frame is missing, duplicated, published later, or read from a different source image;
- normalization produces an empty image or insufficient valid-pixel coverage;
- the measurement ROI falls outside the normalized canvas;
- LineDistance uses unsupported independent left/right ROIs, multi-ROI, or masks;
- a report cannot bind a drawing to the exact per-Step image coordinate space.

None of these failures may auto-run another attempt, change routing, change the active layer, or silently fall back to an unnormalized measurement.

## Drawing And Report Contract

Every validation row must retain coordinate-correct evidence from the exact XML and image run:

1. Raw `Main` source with the Matching template bounds, center, angle, scale, score, and search ROI.
2. Exact normalized `DeviceAligned` image with its valid-pixel boundary and reference axes.
3. `LineDistance` drawing on `DeviceAligned`, including the effective ROI, both selected strip edges, every measurement line, and the reported pixel metrics.
4. A row linking source hash, XML hash, reference pose identity, current pose, scale ratio, normalization result, distance metrics, elapsed time, and final outcome.

Current run-report rendering can draw a saved ROI over the original source even when a runtime clone used different geometry. That path is not acceptable proof for V2. The implementation must retain the actual per-Step input or an equivalent runtime geometry snapshot and render overlays in that image's coordinate space.

Batch proof must show at least: an ordinary success, maximum-angle case, minimum-scale case, maximum-scale case, and one genuine failure/outlier. A numerical CSV without those drawings does not complete this feature.

## Implementation Slices

1. **Pose publication and XML round trip**
   - **P179 complete:** expose Matching scale search in pipeline factory/builder/PropertyGrid;
   - **P179 complete:** publish the existing native Matching pose into fixture center/angle/scale metrics and preserve reference scale;
   - **P181 complete:** teach and preserve the reviewed reference image width and height.
2. **Dynamic normalization layer**
   - **P181 complete:** add fixture-driven `NormalizeImage` behavior to the existing `RotateScale` family;
   - **P181 complete:** implement the inverse similarity warp and fail-closed limits;
   - **P181 complete:** keep fixed Angle/Scale behavior unchanged when fixture mode is off.
3. **LineDistance product round trip**
   - **P182 complete:** preserve its actual left/right ROI serialization;
   - **P182 complete:** require one common reviewed ROI for this bounded workflow;
   - **P182 complete:** keep the saved ROI unchanged and constrain fitted intersections to it.
4. **Coordinate-correct evidence**
   - **P182 complete:** retain per-Step image/geometry evidence;
   - **P182 complete:** extend batch CSV/error rows with pose, scale, normalization, and distance metrics;
   - **P182 complete:** display Matching and normalized-measurement drawings without mixing coordinate spaces.
5. **Current-build proof**
   - **P182 complete for the observed set:** add focused Matching -> normalize -> LineDistance smokes;
   - **P182 complete for the observed set:** run the reviewed Train/Validation/Test lists without per-image tuning;
   - **P182 complete for the observed set:** compare against an identical unnormalized control;
   - **P182 complete for the observed set:** retain representative current-run drawings and exact source/XML hashes.
6. **Fail-closed pre-measurement policy**
   - **P183 complete for C9/P175:** reject no target and exact strong duplicates before fixture publication;
   - **P183 complete for C9/P175:** reject out-of-angle, out-of-scale, and insufficient-coverage cases at the intended Step;
   - **P183 complete for C9/P175:** preserve Matching geometry/metrics even when fixture publication rejects the pose.

## Completion Gate

V2 is complete only when all of the following pass:

- the operator has approved the locator template and taught pose;
- pipeline XML, PropertyGrid, save/load, and apply-back preserve every supported field;
- independently translated, rotated, and uniformly scaled samples execute with zero load/runtime errors inside the declared pose range;
- drawings prove that the same physical black-strip edges are measured after normalization;
- the unnormalized control demonstrably drifts or fails on the same pose variants;
- out-of-range and unsupported geometry fail closed;
- saved XML ROIs and routing remain unchanged;
- no Preview/Run, layer creation, or layer selection occurs without an explicit user action;
- no OK/NG or millimetre claim is made without separate tolerance and calibration evidence.

P183 makes `two-candidate Matching preflight -> one-pose Matching fixture -> NormalizeImage -> LineDistance` a supported bounded, pixel-only C9/P175 authoring contract. The starter operating gate is complete for the exact observed set and deliberate gate exercises, but broader replay still must test robustness. Do not convert the P182/P183 metrics into black-strip OK/NG, calibrated-unit, all-500, unseen-data, or field-robustness claims.
