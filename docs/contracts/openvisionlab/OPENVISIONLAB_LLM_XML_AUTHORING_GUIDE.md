# OpenVisionLab LLM XML Authoring Guide

Updated: 2026-07-19 KST

This guide is the API reference to give to GPT, Gemini, Claude, or another LLM before asking it to draft OpenVisionLab recipe XML.

OpenVisionLab is an OpenCvSharp4 rule-based vision workbench plus an LLM-assisted XML recipe authoring flow. It is not a camera, lighting, PLC, I/O, account, or deployment platform.

## Authoring Loop

Use this loop when collecting real LLM transcripts:

1. Give the LLM this guide plus `docs/contracts/openvisionlab/OPENVISIONLAB_LLM_TOOL_CATALOG.json`.
2. Provide the reference image context, inspection goal, detection points, expected OK/NG condition, and any existing template/image dependency paths. For a packaged runtime, the operator must supply a path the running application can access; an operator-selected absolute path is the clearest draft-time default.
3. Ask for one `VisionPipeline` XML document only.
4. Paste or load the XML into Recipe Manager.
5. Run `Validate`. Do not import yet.
6. If validation is NG, copy the validation report and ask the LLM to repair the XML without changing the intent.
7. Import only after validation is OK and dependencies are present.
8. Run sample/Good/Bad checks explicitly in OpenVisionLab.

Do not ask the LLM to run Preview, Run, load images, switch layers, or accept the recipe. Those are explicit user actions inside OpenVisionLab.

For a local pair of edges, prefer the in-app `Pin gap / edge distance (LineDistance)` flow in Recipe Manager. For a full, translated row of repeated dark pins, use `PinArrayGap` with one row ROI instead of emitting many fixed X-position `LineDistance` Steps. For a curved dark band whose position can move laterally, use `CurveBandProfile` only after reviewing that its connected-component overlay selects the intended band. For a large bright card/label whose lower-right virtual corner must follow translation, use `OuterCornerIntersection` and confirm both drawn outer edges meet at the intended corner. All paths require reviewed ROI/spec fields before the LLM draft is accepted.

## Minimal XML Shape

```xml
<?xml version="1.0" encoding="utf-8"?>
<VisionPipeline>
  <Name>Example_Pipeline</Name>
  <Steps>
    <Step>
      <Name>01 Threshold Precheck</Name>
      <ToolType>Threshold</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Threshold_Result</OutputLayer>
      <Parameters>
        <Parameter>
          <Key>Mode</Key>
          <Value>Threshold</Value>
        </Parameter>
        <Parameter>
          <Key>Threshold</Key>
          <Value>128</Value>
        </Parameter>
        <Parameter>
          <Key>MaxValue</Key>
          <Value>255</Value>
        </Parameter>
        <Parameter>
          <Key>ThresholdType</Key>
          <Value>Binary</Value>
        </Parameter>
      </Parameters>
    </Step>
  </Steps>
</VisionPipeline>
```

Required Step fields:

- `Name`: short, unique, numbered, and meaningful.
- `ToolType`: exact supported tool name or supported alias.
- `Enabled`: usually `true`.
- `InputLayer`: use `Main` for the source image, or use a previous enabled Step `OutputLayer`.
- `OutputLayer`: unique result layer for evidence review.
- `Parameters`: zero or more `Parameter` entries with `Key` and `Value`.

Optional judgement fields:

- `UseAcceptance`: `true` when this Step has an OK/NG gate.
- `ExpectedSuccess`: usually `true`; use `false` only when the Step is expected to fail.
- `MaxElapsedMilliseconds`: positive time budget for this Step.
- `AcceptanceMetricName`: known metric such as `ResultCount`, `ScoreMax`, `AreaAvg`, `DistanceMmAvg`, `DistanceMmRange`, `MergeOverlayCount`, `CornerOuterContourVerified`.
- `UseAcceptanceMetricMinimum`, `AcceptanceMetricMinimum`
- `UseAcceptanceMetricMaximum`, `AcceptanceMetricMaximum`

Parameter values are strings in XML but are validated by type. Boolean values must be `true` or `false`. Numeric values must use invariant decimal text such as `0.6`, `128`, or `3.5`.

## Hard Rules

- Return only a `VisionPipeline` XML document when the user asks for XML.
- Do not invent OpenVisionLab screens, hardware nodes, camera settings, lighting settings, PLC tags, I/O signals, users, roles, or deployment targets.
- Use `Main` only when the Step intentionally starts from the source image.
- Use previous Step output layers for sequential pipelines.
- For intentional fan-out from `Main` or another earlier layer, set parameter `ALLOW_BRANCH_INPUT` to `true`.
- Do not overwrite input layers. Prefer a separate `OutputLayer` for every enabled Step.
- Do not write the same output layer from multiple Steps unless overwrite review is intentional.
- Do not create custom `Inspection.*` XML nodes or parameters. `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` are review channels derived by OpenVisionLab after validation/run evidence.
- Do not use placeholder dependency paths. If a template path is unknown, omit `TemplatePath` and `PATTERN_PATH`, then explain outside XML that the user must choose a real template path.
- Do not use catalog source paths such as `docs\samples\...` as an LLM draft dependency path in a packaged runtime. They are repository catalog references, not packaged assets. Use only an existing operator-supplied path that the running application can access; Import validates it, copies it into the recipe `Template` folder, and updates the imported XML to an installation-root-relative `RECIPE\...\Template\...` path.
- Matching score parameters such as `SCORE_MIN`, `GREEDINESS`, and `HYBRID_VERIFY_IMAGE_WEIGHT` are 0..1 decimals. Use `0.6`, not `60` or `80`.
- For `FeatureMatching`, `SCORE_MIN` is the Lowe descriptor-ratio threshold, not the final `ScoreMax` acceptance value. Smaller values are stricter; use a separate acceptance metric gate for `ScoreMax`.
- Positive parameters must be positive: `MAGNIFIATION`, `RANSAC_REPROJ_THRESHOLD`, `COARSE_ANGLE_STEP`, `PIXELPERMM`, `ScaleXPercent`, `ScaleYPercent`, kernel sizes, `NUM_MATCH`, `SEARCH_STEP`.
- `FIND_ANGLE_MIN` must be less than or equal to `FIND_ANGLE_MAX`.
- Gray-level values such as `Threshold`, `MaxValue`, `RangeMin`, `RangeMax`, `CANNY_LOW`, and `CANNY_HIGH` must be within 0..255.
- HSV `HueMin` and `HueMax` use OpenCV's 0..179 scale. `HueMin > HueMax` intentionally wraps across the 179/0 boundary for colors such as red; `SaturationMin <= SaturationMax` and `ValueMin <= ValueMax` remain required within 0..255.
- `Arithmetic` operation mode needs `InputLayerB` unless the operation is `Bitwise_NOT` or `ABS`, the mode is `Offset`, or `UseConstantInput` is `true`.
- `ReferenceDifference` requires an existing `ReferencePath1`; `ReferencePath2` through `ReferencePath4` are optional approved Good references. Keep each file in its own parameter so import can scan and copy every dependency.
- `OverlayMerge` should be the final enabled Step when it is the user-facing review result.
- For pin-to-pin, edge-to-edge, pitch, width, or clearance checks using `LineDistance` or `PinArrayGap`, do not judge only an average. Edge-gap mode also needs `DistancePxRange`/`DistanceMmRange` or a maximum outlier gate; center-pitch mode also needs `PitchPxRange` or `PitchPxMax`. If one Step must judge the nominal distance and another must judge consistency, duplicate the same measurement parameters into a second validation Step with a separate `OutputLayer`.
- `PinArrayGap` is for one roughly vertical dark-pin row within a reviewed ROI. Missing `MeasurementMode` defaults to `EdgeGap` and returns adjacent **edge-to-edge clearances** as `DistancePx*`. Direct deterministic XML/PropertyGrid may set `MeasurementMode=CenterPitch`, which returns adjacent detected-center spacing as pixel-only `PitchPx*`. Do not substitute one metric family for the other, use `ResultCount` as a gate before representative Good samples prove stable visible pin count, or claim calibrated center pitch. The frozen Pin Guided Setup/LLM intent v1 remains EdgeGap-only.
- `CurveBandProfile` is for one curved dark component in a reviewed ROI. It measures row-wise band width plus outer, inner, and center-path arc length. It is not a generic full-image curve finder, and its px arc length must not be presented as a physical length until calibration is supplied. Confirm the component and both drawn curves on representative Good images before setting `CurveCenterArcLengthPx` or width gates.
- `OuterCornerIntersection` is experimental and must not be selected as a default LLM inspection-intent skill. It attempts a virtual sharp corner for one large bright card/label-like object, but current evidence does not prove that the fitted lower line belongs to the operator-intended physical card-bottom edge. `CornerOuterContourVerified=1` proves support from the runtime's selected threshold contour only; it does not prove physical-boundary ownership or semantic correctness. A fallback labelled `hough` or `projection` publishes `0` and is review-only. Do not create an `IntersectionX`, `IntersectionY`, angle, or `CornerOuterContourVerified` acceptance gate from this family without independent same-image ground truth for both physical edge segments and the intended intersection. Do not tune it image by image as a substitute for a bounded inspection-intent skill.
- For one long dark band whose position moves inside a reviewed coarse ROI, `LineDistance` may use the opt-in `USE_GAP_EDGE_PAIR=true` mode. It selects a supported upper candidate and fits the lower edge from the nearest sustained bright transition after the immediately following dark core, then applies separation, angle, shared-support, local-dark-coverage, and ambiguity gates. A farther Hough line is not the same-band lower boundary. It does not locate an arbitrary object, move the ROI, or replace operator review. Require current-run drawings with the green ROI, yellow candidates, blue/magenta selected edges, and red Gap samples before accepting the XML.
- `CircleGauge` and `GeometryMeasure` are approved deterministic PropertyGrid tools, not a reopening of autonomous LLM recipe development. Use them only when the operator supplies the annular sector or explicitly selects compatible earlier typed features. `CircleGauge` is pixel-only and requires support/coverage/residual drawing evidence. `GeometryMeasure` supports exactly `PointPointDistance`, `PointLineDistance`, `SegmentSegmentDistance`, `LineLineDistance`, `LineLineAngle`, `LineLineIntersection`, and `CircleSegmentClearance`; it fails closed on source order, type, coordinate, producer-gate, parallel, extension, image, or optional ROI mismatch.
- `LineFixture` (`DualEdgeFixture` alias) is an approved deterministic PropertyGrid/XML fixture producer, not a request to invent physical datum identities. Use it only after the operator selects two distinct durable physical edges and their exact earlier accepted `Line/Segment` identities. Datum A defines the X axis; support, residual, included angle, extension, coordinate, and reference-pose gates fail closed. v1 scale is exactly one. Review Datum A/B, intersection, axes, and metrics after explicit Run; do not claim scale, perspective, calibration, automatic feature identity, production robustness, or field qualification.
- `AffineTransform` is an approved deterministic PropertyGrid/XML preprocessing tool, not a reopening of autonomous LLM recipe development. The operator must provide three ordered, non-collinear source/destination pixel-point correspondences. Source points may be fixed numeric teaching or three explicitly selected earlier typed `Point` results; the LLM must never invent Step/Feature references. Review all six matrix coefficients, the destination triangle, transformed source frame, and `AffineValidPixelRatio` after explicit Preview/Run. Do not present it as perspective correction, lens calibration, automatic physical-feature selection, per-image ROI movement, or calibrated metrology.
- `Blob` and `Contour` may filter each detected object by the axis-aligned pixel bounding box using `MIN_WIDTH`, `MAX_WIDTH`, `MIN_HEIGHT`, and `MAX_HEIGHT`. These filters change `ResultCount` and the accepted drawings; rejected candidates remain reviewable with an exact reason in Object Results Inspector. Omit the four keys to preserve legacy area-only behavior. Do not confuse these per-object filters with a later aggregate `BoundsWidthMax` or `BoundsHeightMax` acceptance gate.

Compare an operator mark only with the result drawing from the **same executed source image**. Do not compare absolute `IntersectionX`/`IntersectionY` values across translated or rotated cards. If source-image provenance is uncertain, retain the exact source, XML, runtime drawing, and run report first; do not ask the LLM to guess a replacement coordinate, tune XML parameters, or create a coordinate/angle gate from a cross-image comparison.

## Result Channel Contract

LLMs often try to emit judgement objects directly. Do not do that.

OpenVisionLab derives these review channels:

- `Inspection.Status`: final OK/NG from XML validation plus explicit sample/Good/Bad checks.
- `Inspection.FailedStep`: based on each enabled Step `Name`, `InputLayer`, `OutputLayer`, and `ToolType`.
- `Inspection.Evidence`: output layers, metrics, overlays, and judgement parameters.
- `Inspection.Benchmark`: deterministic parameters, dependency paths, and run-history comparison.
- `Inspection.NextAction`: derived guidance after validation or run review.

The XML must provide enough Step structure and measurable parameters for those channels to be derived.

## Supported Tool Types

The validator currently accepts these names case-insensitively:

- `Threshold`
- `Morphology`
- `Filter`
- `EdgeDetection`, `Edge`
- `Blob`
- `Contour`
- `Line`, `LineGauge`
- `LineDistance`, `LineDistanceGauge`
- `PinArrayGap`, `AdjacentPinGap`
- `CurveBandProfile`, `DarkBandCurve`
- `OuterCornerIntersection`, `BrightObjectCorner`
- `LineIntersection`, `LineIntersectionGauge`
- `CircleGauge`
- `GeometryMeasure`, `GeometricMeasurement`
- `LineFixture`, `DualEdgeFixture`
- `Matching`, `TemplateMatching`
- `EdgeBasedMatching`, `EdgeBasedTemplateMatching`, `EdgeTemplateMatching`
- `Mean`
- `HSV`, `HsvMask`, `ColorHSV`, `ColorMask`
- `RotateScale`, `RotateAndScale`
- `AffineTransform`, `Affine`, `AffineMatrix`
- `Feature`, `FeatureMatching`, `Sift`
- `Arithmetic`
- `ReferenceDifference`
- `OverlayMerge`, `ResultMerge`, `MergeResult`

Prefer canonical names: `Threshold`, `Morphology`, `Filter`, `EdgeDetection`, `Blob`, `Contour`, `LineGauge`, `LineDistance`, `PinArrayGap`, `CurveBandProfile`, `OuterCornerIntersection`, `CircleGauge`, `GeometryMeasure`, `LineFixture`, `Matching`, `EdgeBasedMatching`, `FeatureMatching`, `Mean`, `HSV`, `RotateScale`, `AffineTransform`, `Arithmetic`, `ReferenceDifference`, and `OverlayMerge`.

## Common Patterns

### Threshold to Blob

Use when the target can be separated by brightness and measured by connected regions.

```xml
<VisionPipeline>
  <Name>LLM_Threshold_Blob</Name>
  <Steps>
    <Step>
      <Name>01 Binary</Name>
      <ToolType>Threshold</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Binary_Result</OutputLayer>
      <Parameters>
        <Parameter><Key>Threshold</Key><Value>128</Value></Parameter>
        <Parameter><Key>MaxValue</Key><Value>255</Value></Parameter>
        <Parameter><Key>ThresholdType</Key><Value>Binary</Value></Parameter>
      </Parameters>
    </Step>
    <Step>
      <Name>02 Blob Inspect</Name>
      <ToolType>Blob</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Binary_Result</InputLayer>
      <OutputLayer>Blob_Result</OutputLayer>
      <Parameters>
        <Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>
        <Parameter><Key>MIN_AREA</Key><Value>50</Value></Parameter>
        <Parameter><Key>MAX_AREA</Key><Value>999999</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>1000</MaxElapsedMilliseconds>
      <AcceptanceMetricName>ResultCount</AcceptanceMetricName>
      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
      <AcceptanceMetricMinimum>1</AcceptanceMetricMinimum>
    </Step>
  </Steps>
</VisionPipeline>
```

### Median Denoise to Contour

Use when isolated salt-like noise should be removed before a brightness threshold and final shape/count inspection. `MedianBlur` uses `MedianKernelSize`, not `KernelWidth`/`KernelHeight`; choose a positive odd value such as `3`, `5`, or `7`. Put the acceptance gate on the downstream `Contour` Step because Filter itself supplies an image for the next Step rather than an inspection decision.

```xml
<VisionPipeline>
  <Name>LLM_Filter_Denoise_Count</Name>
  <Steps>
    <Step>
      <Name>01 Median Denoise</Name>
      <ToolType>Filter</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Denoised_Image</OutputLayer>
      <Parameters>
        <Parameter><Key>FilterType</Key><Value>MedianBlur</Value></Parameter>
        <Parameter><Key>MedianKernelSize</Key><Value>5</Value></Parameter>
        <Parameter><Key>BorderType</Key><Value>Reflect101</Value></Parameter>
      </Parameters>
    </Step>
    <Step>
      <Name>02 Binary</Name>
      <ToolType>Threshold</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Denoised_Image</InputLayer>
      <OutputLayer>Denoised_Binary</OutputLayer>
      <Parameters>
        <Parameter><Key>Mode</Key><Value>Threshold</Value></Parameter>
        <Parameter><Key>Threshold</Key><Value>130</Value></Parameter>
        <Parameter><Key>MaxValue</Key><Value>255</Value></Parameter>
        <Parameter><Key>ThresholdType</Key><Value>Binary</Value></Parameter>
      </Parameters>
    </Step>
    <Step>
      <Name>03 Target Count</Name>
      <ToolType>Contour</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Denoised_Binary</InputLayer>
      <OutputLayer>Denoised_Targets</OutputLayer>
      <Parameters>
        <Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>
        <Parameter><Key>MIN_AREA</Key><Value>20</Value></Parameter>
        <Parameter><Key>MAX_AREA</Key><Value>5000</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>1000</MaxElapsedMilliseconds>
      <AcceptanceMetricName>ResultCount</AcceptanceMetricName>
      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
      <AcceptanceMetricMinimum>1</AcceptanceMetricMinimum>
    </Step>
  </Steps>
</VisionPipeline>
```

Keep the route sequential: `Main -> Denoised_Image -> Denoised_Binary -> Denoised_Targets`. Set an exact count or range from representative Good/NG samples instead of assuming the generic minimum above is a production criterion.

### Dynamic Pin Array Clearance

Use this for a reviewed, single row of dark vertical pins that may move horizontally in the image. The default `EdgeGap` mode dynamically finds pin runs inside the row ROI, draws every adjacent empty clearance, and reports `DistancePxMin`, `DistancePxMax`, `DistancePxAvg`, and `DistancePxRange`.

```xml
<VisionPipeline>
  <Name>Pin_Array_Clearance_Review</Name>
  <Steps>
    <Step>
      <Name>01 Top Pin Array Clearance</Name>
      <ToolType>PinArrayGap</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Top_Pin_Clearance</OutputLayer>
      <Parameters>
        <Parameter><Key>MeasurementMode</Key><Value>EdgeGap</Value></Parameter>
        <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
        <Parameter><Key>CvROI</Key><Value>0,120,768,130</Value></Parameter>
        <Parameter><Key>DarkThreshold</Key><Value>128</Value></Parameter>
        <Parameter><Key>MinDarkCoverageRatio</Key><Value>0.55</Value></Parameter>
        <Parameter><Key>MinPinWidth</Key><Value>5</Value></Parameter>
        <Parameter><Key>MaxPinBreakWidth</Key><Value>2</Value></Parameter>
        <Parameter><Key>MinGapWidth</Key><Value>3</Value></Parameter>
      </Parameters>
    </Step>
  </Steps>
</VisionPipeline>
```

This is a measurement starter, not a released acceptance recipe. Derive row-specific min/max/range gates from the training split only, then confirm those gates unchanged on validation and test data. Use a separate duplicated step for each independent gate when needed.

For direct deterministic center-pitch teaching, change only `MeasurementMode` to `CenterPitch` and use `PitchPxMin`, `PitchPxMax`, `PitchPxAvg`, and `PitchPxRange` gates. The runtime drawing must show cyan center points and `P1..Pn` center-to-center lines on the operator-reviewed row before any tolerance is accepted. This mode is dark-pin and pixel-only in the current bounded contract; it is not enabled by the frozen Pin Guided Setup v1.

### HSV Color Mask

Use when the inspection decision is color coverage, not brightness-only thresholding or geometry.

```xml
<VisionPipeline>
  <Name>LLM_HSV_ColorMask</Name>
  <Steps>
    <Step>
      <Name>01 Red Color Coverage</Name>
      <ToolType>HSV</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Red_Color_Mask</OutputLayer>
      <Parameters>
        <Parameter><Key>HueMin</Key><Value>0</Value></Parameter>
        <Parameter><Key>HueMax</Key><Value>10</Value></Parameter>
        <Parameter><Key>SaturationMin</Key><Value>100</Value></Parameter>
        <Parameter><Key>SaturationMax</Key><Value>255</Value></Parameter>
        <Parameter><Key>ValueMin</Key><Value>100</Value></Parameter>
        <Parameter><Key>ValueMax</Key><Value>255</Value></Parameter>
        <Parameter><Key>USE_ROI</Key><Value>false</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>500</MaxElapsedMilliseconds>
      <AcceptanceMetricName>MaskPixelRatio</AcceptanceMetricName>
      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
      <AcceptanceMetricMinimum>0.05</AcceptanceMetricMinimum>
      <UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>
      <AcceptanceMetricMaximum>0.07</AcceptanceMetricMaximum>
    </Step>
  </Steps>
</VisionPipeline>
```

Use `HueMin=170` and `HueMax=10` when the requested red range crosses OpenCV's 179/0 hue boundary. Keep the mask in a distinct output layer. Add a later Blob or Contour Step only when the operator needs count, area, or shape evidence from that mask.

### Template Matching

Use when a stable local template exists.

```xml
<VisionPipeline>
  <Name>LLM_Template_Matching</Name>
  <Steps>
    <Step>
      <Name>01 Template Match</Name>
      <ToolType>Matching</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Matching_Result</OutputLayer>
      <Parameters>
        <Parameter><Key>MATCH_MODE</Key><Value>CCoeffNormed</Value></Parameter>
        <Parameter><Key>SCORE_MIN</Key><Value>0.85</Value></Parameter>
        <Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter>
        <Parameter><Key>MAGNIFIATION</Key><Value>1</Value></Parameter>
        <Parameter><Key>USE_FIND_ANGLE</Key><Value>false</Value></Parameter>
        <Parameter><Key>USE_FIND_SCALE</Key><Value>false</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>1000</MaxElapsedMilliseconds>
      <AcceptanceMetricName>ResultCount</AcceptanceMetricName>
      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
      <AcceptanceMetricMinimum>1</AcceptanceMetricMinimum>
    </Step>
  </Steps>
</VisionPipeline>
```

Before validating this pattern, add both `TemplatePath` and `PATTERN_PATH` with the same existing operator-supplied image path. Import copies that file into the selected recipe's `Template` directory and rewrites the imported XML to an installation-root-relative copied path. Do not copy a literal `docs\samples\...` catalog path into an LLM draft.

When the operator explicitly enables pose search, use `FIND_ANGLE_MIN/MAX` plus `FIND_ANGLE` for angle search and positive `FIND_SCALE_MIN/MAX/STEP` values for uniform scale search. OpenVisionLab publishes the resulting `FixtureCenterX/Y`, `FixtureAngle`, `FixtureScale`, and `FixtureScaleRatio` metrics for a fixture-producing one-match Step. These pose metrics do not by themselves rotate or scale a downstream ROI.

### Matching Fixture Translation V1

Use this only when the operator supplies a reviewed reference center/angle and wants one downstream axis-aligned ROI to follow X/Y part movement. Do not guess the reference pose from a text prompt, and do not claim rotation or scale compensation.

Producer requirements:

- `ToolType=Matching` or `TemplateMatching`;
- `NUM_MATCH=1`;
- `USE_AS_FIXTURE_FRAME=true`;
- unique `FIXTURE_FRAME_NAME`;
- numeric `FIXTURE_REFERENCE_X`, `FIXTURE_REFERENCE_Y`, and `FIXTURE_REFERENCE_ANGLE`;
- positive `FIXTURE_REFERENCE_SCALE` (normally `1` for the original taught template size);
- non-negative `FIXTURE_MAX_ANGLE_DELTA`.

Consumer requirements:

- it appears after the producer;
- it reads the same source layer;
- `ALLOW_BRANCH_INPUT=true`;
- one `USE_ROI=true` and valid `CvROI`;
- `USE_FIXTURE_FRAME=true` and the same `FIXTURE_FRAME_NAME`;
- no multi-ROI or masks.

The runtime publishes center/angle/scale evidence but this V1 mode moves only a cloned effective ROI by X/Y and leaves the XML `CvROI` unchanged. It does not compensate rotation or scale. See `docs\OPENVISIONLAB_MATCHING_FIXTURE_WORKFLOW_SPEC.md` for the full V1 XML and failure contract. When reviewed reference dimensions and angle/scale compensation are required, use the separately supported and fail-closed `Matching Fixture NormalizeImage` contract below; do not mix the two modes or claim that V1 rotates an ROI.

### Fan-Out With OverlayMerge

Use when several inspections start from the same prepared layer and should be reviewed together. Mark branch inputs explicitly.

```xml
<Step>
  <Name>03 Top Region</Name>
  <ToolType>Contour</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Clean_Binary</InputLayer>
  <OutputLayer>Top_Contour</OutputLayer>
  <Parameters>
    <Parameter><Key>ALLOW_BRANCH_INPUT</Key><Value>true</Value></Parameter>
    <Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>
    <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
    <Parameter><Key>CvROI</Key><Value>0,0,768,270</Value></Parameter>
    <Parameter><Key>MIN_AREA</Key><Value>100000</Value></Parameter>
    <Parameter><Key>MAX_AREA</Key><Value>300000</Value></Parameter>
  </Parameters>
</Step>
```

Then add a final merge Step:

```xml
<Step>
  <Name>05 Merge Review</Name>
  <ToolType>OverlayMerge</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>Review_Result</OutputLayer>
  <Parameters>
    <Parameter><Key>SourceLayers</Key><Value>Top_Contour;Bottom_Contour</Value></Parameter>
    <Parameter><Key>BurnIn</Key><Value>true</Value></Parameter>
    <Parameter><Key>DrawLabels</Key><Value>false</Value></Parameter>
    <Parameter><Key>AllowEmpty</Key><Value>false</Value></Parameter>
  </Parameters>
</Step>
```

### Arithmetic With Second Input

Use only after the second input layer already exists.

```xml
<Step>
  <Name>03 Compare Binary Layers</Name>
  <ToolType>Arithmetic</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Binary_A</InputLayer>
  <OutputLayer>Binary_Diff</OutputLayer>
  <Parameters>
    <Parameter><Key>ArithmeticMode</Key><Value>Operation</Value></Parameter>
    <Parameter><Key>ArithmeticOperation</Key><Value>ABSDIFF</Value></Parameter>
    <Parameter><Key>InputLayerB</Key><Value>Binary_B</Value></Parameter>
  </Parameters>
</Step>
```

### Geometry Resize With RotateScale

Use `RotateScale` when the inspection decision is the output image geometry itself, such as a controlled resize result. `ResultImageWidth`, `ResultImageHeight`, and `ResultImageChannels` are produced result metrics; choose one metric/range as the Step acceptance gate.

```xml
<Step>
  <Name>01 Resize Geometry Half</Name>
  <ToolType>RotateScale</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>Geometry_ResizeHalf_Result</OutputLayer>
  <Parameters>
    <Parameter><Key>Angle</Key><Value>0</Value></Parameter>
    <Parameter><Key>ScaleXPercent</Key><Value>50</Value></Parameter>
    <Parameter><Key>ScaleYPercent</Key><Value>50</Value></Parameter>
    <Parameter><Key>Interpolation</Key><Value>Linear</Value></Parameter>
    <Parameter><Key>BorderType</Key><Value>Constant</Value></Parameter>
  </Parameters>
  <UseAcceptance>true</UseAcceptance>
  <ExpectedSuccess>true</ExpectedSuccess>
  <AcceptanceMetricName>ResultImageWidth</AcceptanceMetricName>
  <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
  <AcceptanceMetricMinimum>286</AcceptanceMetricMinimum>
  <UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>
  <AcceptanceMetricMaximum>286</AcceptanceMetricMaximum>
</Step>
```

For a 572x420 input with both scales at 50 percent, the expected result is 286x210. A 640x420 source produces width 320 and must be rejected by this nominal-width gate. This is an output-size check, not a physical measurement or calibration claim.

### Three-Point Pixel Mapping With AffineTransform

Use `AffineTransform` only when the operator has identified three stable physical
features and entered the three source/destination point pairs in the same order.
The two triangles must remain non-collinear even if an area gate is set to zero.

```xml
<Step>
  <Name>01 Three Point Affine Normalize</Name>
  <ToolType>AffineTransform</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>Affine_Normalized</OutputLayer>
  <Parameters>
    <Parameter><Key>SourcePoint1X</Key><Value>0</Value></Parameter>
    <Parameter><Key>SourcePoint1Y</Key><Value>0</Value></Parameter>
    <Parameter><Key>SourcePoint2X</Key><Value>100</Value></Parameter>
    <Parameter><Key>SourcePoint2Y</Key><Value>0</Value></Parameter>
    <Parameter><Key>SourcePoint3X</Key><Value>0</Value></Parameter>
    <Parameter><Key>SourcePoint3Y</Key><Value>100</Value></Parameter>
    <Parameter><Key>DestinationPoint1X</Key><Value>20</Value></Parameter>
    <Parameter><Key>DestinationPoint1Y</Key><Value>10</Value></Parameter>
    <Parameter><Key>DestinationPoint2X</Key><Value>110</Value></Parameter>
    <Parameter><Key>DestinationPoint2Y</Key><Value>15</Value></Parameter>
    <Parameter><Key>DestinationPoint3X</Key><Value>30</Value></Parameter>
    <Parameter><Key>DestinationPoint3Y</Key><Value>100</Value></Parameter>
    <Parameter><Key>OutputWidth</Key><Value>572</Value></Parameter>
    <Parameter><Key>OutputHeight</Key><Value>420</Value></Parameter>
    <Parameter><Key>Interpolation</Key><Value>Linear</Value></Parameter>
    <Parameter><Key>BorderType</Key><Value>Constant</Value></Parameter>
    <Parameter><Key>BorderValue</Key><Value>0</Value></Parameter>
    <Parameter><Key>MinimumSourceTriangleArea</Key><Value>100</Value></Parameter>
    <Parameter><Key>MinimumDestinationTriangleArea</Key><Value>100</Value></Parameter>
    <Parameter><Key>MinimumValidPixelRatio</Key><Value>0.75</Value></Parameter>
  </Parameters>
</Step>
```

`OutputWidth=0` or `OutputHeight=0` keeps the corresponding input dimension.
The authoritative transform is `AffineM11..AffineM23`; scale, rotation, and shear
metrics are review aids. A valid matrix does not prove that the operator selected
the right physical features. Freeze downstream reference-coordinate ROIs only after
the current-run triangle/frame drawing and coverage metric are accepted.

To resolve the three source points from the same explicit Run, keep the taught
destination points above and add these parameters:

```xml
<Parameter><Key>USE_DETECTED_SOURCE_POINTS</Key><Value>true</Value></Parameter>
<Parameter><Key>SOURCE_POINT_1_FEATURE</Key><Value>01 Locate A::Center</Value></Parameter>
<Parameter><Key>SOURCE_POINT_2_FEATURE</Key><Value>02 Locate B::Center</Value></Parameter>
<Parameter><Key>SOURCE_POINT_3_FEATURE</Key><Value>03 Locate C::Center</Value></Parameter>
```

Each reference must name a distinct typed `Point` published by an earlier enabled
Step on the Affine input layer and current image frame. `Matching::Center` is
published only for exactly one usable result. A missing, ambiguous, rejected,
wrong-kind, duplicate, cross-frame, non-finite, or out-of-image source fails closed;
the runtime never falls back to stale `SourcePoint*X/Y` values. Successful runs
publish `AffineDetectedSourcePointCount=3` plus the six resolved
`AffineSourcePoint*X/Y` metrics. The operator still owns physical-feature identity,
point order, locator gates, destination teaching, and downstream fixed-ROI truth.

### EdgeBasedMatching Unique-Result Gate

Use this opt-in gate only when an inspection requires one unambiguous
`EdgeBasedMatching` result. It preserves an internal candidate pool even though
the external result count is one:

```xml
<Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter>
<Parameter><Key>USE_MULTI_ROI</Key><Value>false</Value></Parameter>
<Parameter><Key>USE_UNIQUE_MATCH_VALIDATION</Key><Value>true</Value></Parameter>
<Parameter><Key>UNIQUE_MATCH_MIN_SCORE_MARGIN</Key><Value>0.03</Value></Parameter>
```

The margin is normalized `0..1`. The validator rejects unique mode with
`NUM_MATCH != 1`, `USE_MULTI_ROI=true`, or an out-of-range margin. Runtime state
is `UniqueMatch.State=1` for `NoMatch`, `2` for `Success`, and `3` for
`Ambiguous`. `NoMatch` and `Ambiguous` return no `MatchingResult`; the latter
also returns the selected score, strongest spatially distinct plausible
alternative, normalized score margin, required margin, alternative count, and
the exact reject reason.

Use `UniqueMatch.PlausibleAlternativeCount` and
`UniqueMatch.ScoreMargin` for review. The `MatchingResult.ScoreMargin` field on
a successful result is the same difference in percentage points, while pipeline
metrics remain normalized. Missing keys keep legacy behavior:
`USE_UNIQUE_MATCH_VALIDATION=false` and default margin `0.03`.

This is a deterministic acceptance gate, not automatic threshold selection,
template qualification, pose-accuracy proof, or a replacement for a reviewed
search ROI. Do not lower the gate merely to force a repeated pattern to pass.

### EdgeBasedMatching Global Polarity

Use this option only when reviewed evidence shows that the same complete
physical feature can reverse contrast:

```xml
<Parameter><Key>ALLOW_GLOBAL_POLARITY_REVERSAL</Key><Value>true</Value></Parameter>
```

Missing keys preserve Same-only scoring. Enabled mode permits one globally
consistent direction reversal for the complete candidate; it does not ignore
polarity independently at each edge. Review
`GlobalPolarity.Reversed` (`0=Same`, `1=Reversed`) with the result drawing and
retain the existing score, uniqueness, ROI, angle, and scale gates.

Do not enable it from one convenient image or treat the bounded synthetic
contract as physical qualification. A qualified recipe needs a named feature,
labelled representative captures, frozen settings, and held-out replay.

### Matching Fixture NormalizeImage

Use this bounded mode only when the operator has reviewed one Matching template, one reference pose, and the exact reference image width/height. It creates a new reference-coordinate image layer by applying the inverse Matching center/angle/uniform-scale pose. It does not rotate an ROI, perform perspective correction, calibrate pixels, or prove a downstream measurement.

The producer must be one earlier `Matching` Step with `NUM_MATCH=1`. Add these parameters to that Step:

```xml
<Parameter><Key>USE_AS_FIXTURE_FRAME</Key><Value>true</Value></Parameter>
<Parameter><Key>FIXTURE_FRAME_NAME</Key><Value>DeviceFrame</Value></Parameter>
<Parameter><Key>FIXTURE_REFERENCE_X</Key><Value>110</Value></Parameter>
<Parameter><Key>FIXTURE_REFERENCE_Y</Key><Value>90</Value></Parameter>
<Parameter><Key>FIXTURE_REFERENCE_ANGLE</Key><Value>0</Value></Parameter>
<Parameter><Key>FIXTURE_REFERENCE_SCALE</Key><Value>1</Value></Parameter>
<Parameter><Key>FIXTURE_REFERENCE_IMAGE_WIDTH</Key><Value>320</Value></Parameter>
<Parameter><Key>FIXTURE_REFERENCE_IMAGE_HEIGHT</Key><Value>240</Value></Parameter>
<Parameter><Key>FIXTURE_MAX_ANGLE_DELTA</Key><Value>5.25</Value></Parameter>
<Parameter><Key>FIXTURE_MIN_SCALE_RATIO</Key><Value>0.8</Value></Parameter>
<Parameter><Key>FIXTURE_MAX_SCALE_RATIO</Key><Value>1.8</Value></Parameter>
```

Then branch from the same unannotated source layer into the existing `RotateScale` family:

```xml
<Step>
  <Name>02 Normalize Device Image</Name>
  <ToolType>RotateScale</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>DeviceAligned</OutputLayer>
  <Parameters>
    <Parameter><Key>Name</Key><Value>DeviceNormalizeImage</Value></Parameter>
    <Parameter><Key>Angle</Key><Value>0</Value></Parameter>
    <Parameter><Key>ScaleXPercent</Key><Value>100</Value></Parameter>
    <Parameter><Key>ScaleYPercent</Key><Value>100</Value></Parameter>
    <Parameter><Key>Interpolation</Key><Value>Linear</Value></Parameter>
    <Parameter><Key>BorderType</Key><Value>Constant</Value></Parameter>
    <Parameter><Key>USE_FIXTURE_FRAME</Key><Value>true</Value></Parameter>
    <Parameter><Key>FIXTURE_FRAME_NAME</Key><Value>DeviceFrame</Value></Parameter>
    <Parameter><Key>FIXTURE_APPLY_MODE</Key><Value>NormalizeImage</Value></Parameter>
    <Parameter><Key>FIXTURE_MIN_VALID_PIXEL_RATIO</Key><Value>0.25</Value></Parameter>
    <Parameter><Key>ALLOW_BRANCH_INPUT</Key><Value>true</Value></Parameter>
  </Parameters>
</Step>
```

`FIXTURE_MIN_SCALE_RATIO` and `FIXTURE_MAX_SCALE_RATIO` are optional as a pair for backward compatibility. When used, both must be finite and satisfy `0 < minimum <= maximum`; the fixture publisher fails before normalization when the current/reference scale ratio is outside the range. `NormalizeImage` requires the current source size to equal the taught reference size and rejects missing/invalid pose, angle-limit violations, non-positive or out-of-range scale, ROI/masks on the normalization Step, and insufficient valid-pixel coverage. It never falls back to the fixed `Angle`/`ScaleXPercent`/`ScaleYPercent` values. When fixture mode is off, those fixed `RotateScale` values keep their existing behavior.

Review `FixtureValidPixelRatio`, the valid-boundary/reference-axis drawing on `DeviceAligned`, and the raw Matching drawing before adding a measurement Step. Do not emit `FIXTURE_RUNTIME_*` parameters; OpenVisionLab supplies those only during the explicit run. A valid normalized image is not, by itself, proof that a later `LineDistance` ROI selected the intended physical edges.

For the operator-reviewed C9 black-strip workflow, P183 adds a bounded fail-closed pre-measurement policy. Use a separate earlier Matching Step with the same template/search configuration, `NUM_MATCH=2`, `SCORE_MIN=0.8`, and acceptance `ScoreMargin >= 10`. Keep the fixture publisher itself at `NUM_MATCH=1`. `ScoreMargin` is emitted only when Matching requests exactly two candidates; it is the best score minus the second-best score in percentage points, with a missing second candidate contributing zero. This separation rejects two equally strong targets without violating the unambiguous fixture-producer contract. The C9 values `ScoreMax >= 80`, `ScoreMargin >= 10`, angle delta `<= 5.25`, scale ratio `0.8..1.8`, and valid-pixel ratio `>= 0.25` are starter policy grounded only by the P175 24 rows plus deliberate P183 gate exercises; do not use them as general Matching defaults.

P182 qualifies the following bounded pixel-only consumer on the exact P175 24-row set:

```xml
<Step>
  <Name>03 Measure Reviewed Black Strip Gap Px</Name>
  <ToolType>LineDistance</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>DeviceAligned</InputLayer>
  <OutputLayer>GapMeasured</OutputLayer>
  <Parameters>
    <Parameter><Key>Name</Key><Value>ReviewedBlackStripGap</Value></Parameter>
    <Parameter><Key>PIXELPERMM</Key><Value>0</Value></Parameter>
    <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
    <Parameter><Key>CvROI</Key><Value>20,200,510,60</Value></Parameter>
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
    <Parameter><Key>SHOW_VERTICAL_LINE</Key><Value>true</Value></Parameter>
    <Parameter><Key>SHOW_EDGE</Key><Value>true</Value></Parameter>
    <Parameter><Key>SHOW_FITLINE</Key><Value>true</Value></Parameter>
  </Parameters>
</Step>
```

`USE_EXTEND_FIT_LINE=true` is an explicit fitted-edge distance choice for Pipeline `LineDistance`: the final samples connect the two fitted strip boundaries, and endpoints outside the configured ROI are discarded. The default `false` path keeps raw edge-point intersections. Do not set this flag merely to make a drawing longer, and do not derive a tolerance from `EXTEND_FIT_LINE_VALUE`. Before accepting the XML, review both Matching Steps, the normalized valid boundary, measurement ROI, both fitted edges, and final distance lines from the same run. P183 supplies only the bounded pre-measurement gate; it is not Gap OK/NG truth, millimetres, all-500 behavior, unseen robustness, or field qualification.

### Direct dark-band Gap edge pair without Matching

Use this only when the operator supplies a coarse ROI containing the intended long dark band and explicitly wants pixel thickness without a locator. The unchanged starter values below use the P189-corrected nearest same-band lower-boundary runtime and remain bounded top-right evidence, not general defaults:

The user-visible inspection-intent contract, required drawings, failure table, and phase gates are frozen in `docs\OPENVISIONLAB_DARK_BAND_GAP_INTENT_SKILL.md`. Select `Dark band thickness / Gap (LineDistance)` in Guided Setup; do not route this intent through the generic pin-gap template.

```xml
<Step>
  <Name>Detect Dark Band Gap Edges</Name>
  <ToolType>LineDistance</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>GapMeasured</OutputLayer>
  <Parameters>
    <Parameter><Key>PIXELPERMM</Key><Value>0</Value></Parameter>
    <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
    <Parameter><Key>CvROI</Key><Value>100,80,530,230</Value></Parameter>
    <Parameter><Key>USE_GAP_EDGE_PAIR</Key><Value>true</Value></Parameter>
    <Parameter><Key>CANNY_LOW</Key><Value>10</Value></Parameter>
    <Parameter><Key>CANNY_HIGH</Key><Value>45</Value></Parameter>
    <Parameter><Key>GAP_MIN_PX</Key><Value>12</Value></Parameter>
    <Parameter><Key>GAP_MAX_PX</Key><Value>60</Value></Parameter>
    <Parameter><Key>GAP_MAX_ANGLE_DEG</Key><Value>8</Value></Parameter>
    <Parameter><Key>GAP_MAX_PARALLEL_DELTA_DEG</Key><Value>4</Value></Parameter>
    <Parameter><Key>GAP_MIN_SUPPORT_RATIO</Key><Value>0.26</Value></Parameter>
    <Parameter><Key>GAP_MIN_DARK_CONTRAST</Key><Value>8</Value></Parameter>
    <Parameter><Key>GAP_MIN_DARK_COVERAGE_RATIO</Key><Value>0.25</Value></Parameter>
    <Parameter><Key>GAP_MIN_SCORE_MARGIN</Key><Value>0.05</Value></Parameter>
  </Parameters>
</Step>
```

Review `GapCandidateLineCount`, each stage count, `GapSelectedSupportRatio`, `GapSelectedAngleDeltaDeg`, `GapDarkCoverageRatio`, `GapBandMeanGray`, `GapScoreMargin`, and the drawing together. A successful execution is still measurement-only; add no OK/NG or millimetre claim until the operator supplies tolerance and calibration evidence.

P190 establishes a stricter authoring boundary: this raw-coordinate starter is valid only when the reviewed ROI contains exactly one complete intended long band and no competing long band. On the full 500-image top-right corpus, successful executions sometimes selected a lower secondary structure, and raising shared support did not remove that class. The LLM must not repair those cases by inventing per-image ROIs or repeatedly tuning numeric thresholds. Require controlled acquisition/ROI placement or an explicitly approved localization/segmentation stage before expanding this skill.

## Correction Prompt Template

Use this when validation fails:

```text
You are repairing an OpenVisionLab VisionPipeline XML draft.

Keep the original inspection intent. Return only one VisionPipeline XML document.

Validation report:
{paste OpenVisionLab validation report}

Dependency report:
{paste dependency report if present}

Original draft:
{paste previous XML}

Repair rules:
- Use only supported ToolType names.
- Do not invent layers. InputLayer must be Main or a previous enabled Step OutputLayer.
- Do not invent dependency files. Use only an existing path supplied by the operator; do not replace a missing path with a `docs\samples\...` catalog reference.
- Do not emit Inspection.* XML nodes or parameters.
- SCORE_MIN, GREEDINESS, and HYBRID_VERIFY_IMAGE_WEIGHT are 0..1 decimals.
- HSV HueMin/HueMax are 0..179 and may wrap when HueMin is greater than HueMax; Saturation and Value ranges remain ordered 0..255.
- For fixture normalization, preserve the operator-supplied reference pose and dimensions; do not invent them or emit FIXTURE_RUNTIME_* parameters.
- For fitted-edge LineDistance, preserve the operator-reviewed ROI and show both fitted edges plus final distance lines; do not invent tolerance or calibration from EXTEND_FIT_LINE_VALUE.
- Use separate OutputLayer values for evidence.
```

## Transcript Corpus Policy

Store raw external LLM experiments outside public sample paths until reviewed:

- Raw prompt/response: `artifacts\llm_transcripts\raw\...`
- User-provided or operator-repaired XML replay cases: `artifacts\llm_transcripts\manual\...`
- Sanitized replay corpus candidate: `artifacts\llm_transcripts\sanitized\...`
- Public documentation examples: only after private product names, customer data, old company recipe names, and non-public assets are removed.

Manual replay cases are useful validation evidence, but they are not real GPT/Gemini/Claude transcript evidence. Do not report them as external LLM correction-loop transcripts.

Commit only sanitized examples that are safe under the public sample asset policy.

### Public Transcript Publication Gate

Before moving a sanitized transcript from `artifacts` into a tracked documentation path, verify every item below:

- Record the provider, transfer method, known model/version, direct-success or correction-round classification, and whether the evidence came from an API. Use `unknown` instead of inferring missing metadata.
- Confirm prompt/response hashes against the preserved raw evidence and scan the publishable files for credentials, absolute paths, user names, customer/product names, and non-public asset references.
- Verify every input asset against `OPENVISIONLAB_PUBLIC_SAMPLE_ASSET_POLICY.md`, record input hashes, and keep transcript evidence outside `docs\samples\public`.
- Replay validation/import plus the documented OK/NG cases with a current build. Preserve explicit Preview/Run behavior and record the exact commands and results.
- Add a conspicuous AI-generated-content disclosure, human-review attribution, and the user's explicit approval for repository publication.
- Publish only the minimum reproducible package. Exclude raw manifests, local-path reports, attachment paths, credentials, and unrelated debug artifacts.

The reusable review format and the first audited candidate are documented in `OPENVISIONLAB_LLM_TRANSCRIPT_PUBLICATION_REVIEW_20260715.md`.

## Grounding Sources

This guide is based on:

- `0. UI\0) MENU\Wpf\OpenVisionShellHostRecipeCommandSurface.cs`
- `1. Core\Pipeline\Validation\VisionPipelineValidation.cs`
- `1. Core\Pipeline\Definition\VisionPipelineStepParameterSchema.cs`
- `1. Core\Pipeline\Validation\VisionPipelineKnownMetrics.cs`
- `1. Core\Pipeline\Tools\VisionPipelineHsvMaskTool.cs`
- `1. Core\Pipeline\Tools\VisionPipelineArithmeticStep.cs`
- `1. Core\Pipeline\Tools\VisionPipelineReferenceDifferenceTool.cs`
- `docs\samples\*.pipeline.xml`
- Direct smoke coverage in `OpenVisionLabDirectSmokeRunner.cs` for malformed XML, missing input layer, unsupported ToolType, missing dependency path, invalid parameter values, matching score percentage misuse, `Inspection.*` misuse, correction-bundle copy, corrected import, and missing Arithmetic `InputLayerB`.
