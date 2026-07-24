# OpenVisionLab General Geometric Measurement Workspace Contract

Updated: 2026-07-23 KST  
Contract state: Complete on 2026-07-23. The approved checkpoints A through C and all bounded completion gates passed.

## Approved Decision

Approve one bounded deterministic geometry family that lets an operator detect a feature once and reuse that exact result in later measurements.

The proposed slice adds:

- one typed geometry-result sidecar for `Point`, `Segment`, and `Circle`;
- additive export of one reviewed Line result from the existing `Line` family;
- one new radial-caliper `CircleGauge` producer for a full circle or bounded arc;
- one new `GeometryMeasure` consumer for named feature-to-feature relationships;
- PropertyGrid-backed teaching and a Pipeline Review geometry result table/drawing;
- saved-report persistence and fail-closed source/coordinate validation.

It does not replace the Pipeline, create a graph editor, reopen LLM development, or qualify any inspection recipe.

## Why This Must Exist

The current tools can measure useful line cases, but they do not compose detected geometry:

- `LineDistance` runs its own Line A/B gauges and publishes sampled distance metrics.
- `LineIntersection` runs its own Line A/B gauges and publishes one intersection coordinate.
- neither tool publishes a typed feature that a later Step can reference by name;
- there is no rule-based circle/arc measurement family;
- `OuterCornerIntersection` remains experimental and cannot be a default geometry source.

Adding more one-off tools would duplicate edge detection and make drawings harder to audit. The minimum sustainable change is a small typed result contract plus one relationship consumer.

## Operator Task

1. Teach a Line or Circle/Arc feature on an image or normalized reference layer.
2. Run explicitly and review the detected feature, support, and fit evidence.
3. Select two earlier named features in a `GeometryMeasure` Step.
4. Choose one supported relationship and its explicit geometric semantics.
5. Run explicitly and review the source features, construction, value, and failure reason.
6. Save the recipe and retain the same typed features and drawings in Run History.

## Included Scope

### Feature producers

| Producer | Named outputs | Boundary |
| --- | --- | --- |
| Existing `Line` | `Segment`, `Start`, `End`, `Midpoint` | Exports only when one usable fitted line exists in one reviewed coordinate frame. Existing Line XML and metrics remain unchanged. |
| New `CircleGauge` | `Circle`, `Center` | Pixel-only radial edge sampling inside one operator-taught annular sector, followed by a robust circle fit. |
| New `GeometryMeasure` | Mode-specific construction points such as `Intersection`, `MeasureStart`, and `MeasureEnd` | Enables a later relationship Step to reuse a produced point; it does not detect image edges. |

### Relationship modes

| `MeasurementMode` | Required A/B kinds | Exact meaning | Primary metric |
| --- | --- | --- | --- |
| `PointPointDistance` | Point / Point | Euclidean distance. | `GeometryDistancePx` |
| `PointLineDistance` | Point / Segment | Perpendicular distance to the infinite support line of B. | `GeometryDistancePx` |
| `SegmentSegmentDistance` | Segment / Segment | Minimum distance between the two finite segments; zero when they cross. | `GeometryDistancePx` |
| `LineLineDistance` | Segment / Segment | Perpendicular distance between near-parallel infinite support lines; fails when the configured parallel-angle gate is exceeded. | `GeometryDistancePx` |
| `LineLineAngle` | Segment / Segment | Smaller undirected angle in the range `0..90` degrees. | `GeometryAngleDeg` |
| `LineLineIntersection` | Segment / Segment | Intersection of the two infinite support lines, accepted only when image/ROI and per-segment extension gates pass. | `IntersectionX`, `IntersectionY` |
| `CircleSegmentClearance` | Circle / Segment | Signed distance from the circle boundary to the nearest point on the finite segment; negative means overlap. | `GeometrySignedClearancePx` |

The mode names carry the finite/infinite meaning. There is no hidden global line-extension interpretation.

## Geometry Result Contract

Each successful producer may attach zero or more `VisionPipelineGeometryFeatureResult` rows to its `VisionToolResult`. The sidecar follows the same additive pattern as P211 object results and must also be copied into direct recipe summaries and saved Run Reports.

Required identity and provenance:

- `SourceStep`: exact producing Step name;
- `FeatureName`: producer-defined stable name;
- `Kind`: `Point`, `Segment`, or `Circle`;
- `CoordinateLayer`: exact producer input-layer name;
- `CoordinateWidth` and `CoordinateHeight`;
- finite pixel coordinates appropriate to the kind;
- optional producer evidence: `SupportCount`, `SupportRatio`, and `FitResidualPx` when meaningful.

Kind payloads:

- Point: `X`, `Y`.
- Segment: `X1`, `Y1`, `X2`, `Y2`.
- Circle: `CenterX`, `CenterY`, `RadiusPx`.

The stable feature identity is `<SourceStep>/<FeatureName>`. Recipe XML stores the two parts separately so Step names do not require delimiter escaping.

The result sidecar is run evidence, not saved teaching state. A later Step resolves it only from an earlier successful Step in the same explicit Pipeline run.

## Coordinate And Reference Rules

`GeometryMeasure` must fail closed unless all of the following are true:

1. `SourceStepA` and `SourceStepB` name unique, enabled, earlier Steps.
2. Each requested `SourceFeatureA`/`SourceFeatureB` exists exactly once and has the required kind.
3. Both features have the same `CoordinateLayer`, width, and height.
4. The `GeometryMeasure` input layer exactly equals that coordinate layer.
5. Every required coordinate is finite and inside the recorded image where the mode requires an in-image feature.
6. The referenced producer Steps succeeded and passed their own acceptance gates.

Cross-layer guessing, same-size-only guessing, implicit Fixture transforms, references to later Steps, and last-known-result fallback are prohibited. When Fixture normalization is needed, all geometry producers and consumers run after `NormalizeImage` on the same named reference-coordinate layer.

## CircleGauge Teaching Contract

### Required PropertyGrid parameters

- `USE_ROI=true` and one `CvROI` coarse boundary;
- `CENTER_X`, `CENTER_Y` for the taught approximate center;
- `RADIUS_MIN`, `RADIUS_MAX` for the annular search region;
- `START_ANGLE_DEG` and `SWEEP_ANGLE_DEG`; `360` means a full circle;
- `SCAN_COUNT`;
- `EDGE_POLARITY`: `LightToDark`, `DarkToLight`, or `Either`;
- `MIN_CONTRAST`;
- `MIN_SUPPORT_RATIO`;
- `MAX_FIT_RESIDUAL_PX`.

Image-coordinate convention:

- zero degrees points along positive X;
- angles increase clockwise because image Y increases downward;
- all v1 values and gates are pixel-only.

### Deterministic execution

1. Sample radial scan lines only through the taught annular sector.
2. Select the strongest polarity-compatible transition on each valid scan.
3. Fit one circle, reject gross residual outliers once, and refit.
4. Publish a result only when support ratio, support count, fitted radius, finite center, and RMS residual satisfy the configured gates.

Hough-circle voting alone is not sufficient completion evidence because it does not expose the required per-ray support and fit residual. The implementation should use existing OpenCvSharp primitives and add no dependency.

### Metrics

- `ResultCount` (`1` on a valid result);
- `CircleCenterX`, `CircleCenterY`;
- `CircleRadiusPx`, `CircleDiameterPx`;
- `CircleSupportCount`, `CircleSupportRatio`;
- `CircleCoverageDeg`;
- `CircleFitResidualPx`.

### Required drawing

- coarse ROI;
- inner and outer taught radii plus requested arc span;
- accepted edge points and rejected fit outliers in distinct colors;
- fitted circle/arc and center cross;
- support ratio, coverage, radius, and residual text;
- explicit `PASS` or named fail-closed reason.

## GeometryMeasure XML Contract

Required parameters:

- `MeasurementMode`;
- `SourceStepA`, `SourceFeatureA`;
- `SourceStepB`, `SourceFeatureB`.

Mode-specific parameters:

- `MAX_PARALLEL_ANGLE_DELTA_DEG` for `LineLineDistance`;
- `MAX_EXTENSION_A_PX` and `MAX_EXTENSION_B_PX` for `LineLineIntersection`;
- `REQUIRE_RESULT_IN_IMAGE=true` by default for intersection;
- optional one `CvROI` result gate when the operator must constrain an intersection or construction to a reviewed physical region.

Example fragment:

```xml
<Step>
  <Name>CardCorner</Name>
  <ToolType>GeometryMeasure</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>CardAligned</InputLayer>
  <OutputLayer>CardCorner_Result</OutputLayer>
  <Parameters>
    <Parameter><Key>MeasurementMode</Key><Value>LineLineIntersection</Value></Parameter>
    <Parameter><Key>SourceStepA</Key><Value>CardBottomLine</Value></Parameter>
    <Parameter><Key>SourceFeatureA</Key><Value>Segment</Value></Parameter>
    <Parameter><Key>SourceStepB</Key><Value>CardRightLine</Value></Parameter>
    <Parameter><Key>SourceFeatureB</Key><Value>Segment</Value></Parameter>
    <Parameter><Key>MAX_EXTENSION_A_PX</Key><Value>12</Value></Parameter>
    <Parameter><Key>MAX_EXTENSION_B_PX</Key><Value>12</Value></Parameter>
    <Parameter><Key>REQUIRE_RESULT_IN_IMAGE</Key><Value>true</Value></Parameter>
  </Parameters>
  <UseAcceptance>false</UseAcceptance>
</Step>
```

This fragment assumes two earlier successful `Line` Steps named `CardBottomLine` and `CardRightLine`, both operating on `CardAligned`.

## UI Contract

The workspace uses existing surfaces instead of adding a graph engine.

### PropertyGrid Tool Views

- `CircleGauge` remains a PropertyGrid algorithm tool with one explicit annular-sector editor.
- `GeometryMeasure` remains a PropertyGrid algorithm tool.
- The top `Sources` category of the `GeometryMeasure` PropertyGrid provides compact Source A/B dropdowns that list only compatible features from earlier Steps.
- Selecting A/B in the picker updates the same PropertyGrid-backed model only after explicit `적용` (`Apply`).
- Parameter or ROI edits never trigger Preview/Run.

### Pipeline Review

- Show a `Geometry / 기하 측정` result tab only when the selected run contains geometry features.
- Table columns: Step, Feature, Kind, coordinates, radius/length when applicable, support, residual, and state.
- Selecting a row highlights exactly that feature in the retained current-run drawing; clicking a drawn feature selects the row.
- Relationship rows show A/B identities, mode, value, units, and named failure/gate evidence.
- `피처 설정 편집`, `관계 설정 편집`, and `리뷰 실행` reuse the existing Recipe Manager handoff and explicit Run Review paths.

The result tab is evidence, not an editor. PropertyGrid remains authoritative.

## Failure And Drawing Rules

Named fail-closed reasons must distinguish at least:

- source Step missing, duplicated, disabled, later than consumer, failed, or acceptance-NG;
- source feature missing, duplicated, wrong kind, or non-finite;
- coordinate layer or image-size mismatch;
- Circle support, contrast, radius, coverage, or residual gate failure;
- parallel lines where intersection is requested;
- non-parallel lines where `LineLineDistance` is requested;
- intersection outside the image, optional result ROI, or either extension gate;
- degenerate zero-length segment;
- unsupported measurement mode.

A numeric metric without both source features and the final construction drawing is not valid geometry evidence.

## Persistence And Compatibility

- Add geometry rows to direct recipe summaries and saved Run Reports; old reports with no geometry list load as empty.
- Keep existing overlay and object-result schemas intact.
- Keep existing `Line`, `LineDistance`, and `LineIntersection` XML behavior unchanged.
- Do not migrate or rewrite old Steps automatically.
- `OuterCornerIntersection` is excluded as a default feature producer until independent evidence proves its physical-boundary semantics.
- Update deterministic validator/schema/catalog entries only when the runtime implementation passes its contract smokes. This compatibility update does not reopen LLM prompts, providers, or intent skills.

## Explicit Non-Scope

- automatic feature/ROI selection from arbitrary images;
- per-image LLM detection or ROI movement;
- mm units, calibration teaching, lens correction, camera calibration, or world/robot coordinates;
- perspective or nonuniform coordinate transforms;
- ellipse, spline, freeform curve, tangent, midpoint-construction library, region algebra, or CAD import;
- OCR, barcode, deep learning, 3D, camera, lighting, PLC, I/O, or deployment work;
- large-corpus inspection validation or recipe parameter tuning.

## Approved Implementation Checkpoints

### A. Typed feature plumbing and existing Line export

- Add the geometry sidecar/store, summaries, report persistence, and same-run resolution.
- Export `Segment`, `Start`, `End`, and `Midpoint` from one usable existing Line result.
- Add validator failures for order, identity, type, and coordinate mismatch.

### B. GeometryMeasure and feature picker

- Add the seven frozen relationship modes and exact pixel semantics above.
- Add bounded intersection/parallel gates, drawings, metrics, PropertyGrid mapping, and the compatible earlier-feature picker.

### C. CircleGauge and Geometry Review

- Add the radial-caliper full-circle/arc producer, visual annular teaching, metrics, drawings, result table, row/drawing selection, and saved-report replay.

No checkpoint is the completed workspace by itself. Completion requires all three checkpoints and the gates below.

## Completion Gates

1. Full solution and focused tool/UI builds pass with zero errors.
2. Old `Line`, `LineDistance`, and `LineIntersection` focused regressions remain unchanged.
3. XML/property load-apply-save-reload preserves every new parameter without Preview/Run or layer/routing mutation.
4. Synthetic math checks cover every relationship mode, degenerate input, incompatible type, coordinate mismatch, and bounded/far intersection.
5. Circle checks cover one full circle, one partial arc, wrong polarity/no edge, insufficient coverage, and excessive residual with exact drawings.
6. Direct summaries and saved reports round-trip all geometry rows and provenance.
7. Fresh current-build before/after UI captures show PropertyGrid teaching, feature selection, result table, and drawing selection.
8. Evidence states pixel-only algorithm/UI verification; it does not claim industrial semantic accuracy, calibration, unseen robustness, or field qualification.

## Approved Checklist

The operator approved all four decisions on 2026-07-23:

1. Typed feature identity is `SourceStep + FeatureName`, with only `Point`, `Segment`, and `Circle` in v1.
2. `CircleGauge` uses an operator-taught annular sector and radial-caliper fit evidence rather than a Hough-only detector.
3. `GeometryMeasure` supports exactly the seven frozen pixel-only modes in this document.
4. Editing stays PropertyGrid-based; Pipeline Review supplies read-only table/drawing evidence; Preview/Run stays explicit.

After approval, implementation may proceed through A, B, and C without requesting repeated approval for ordinary in-scope code, build, smoke, or documentation work.

## Completion Record

```text
Status: Complete
Scope: P213 typed same-run Point/Segment/Circle results, existing-Line export, CircleGauge, seven GeometryMeasure modes, compatible PropertyGrid source dropdowns, Geometry Review table/drawing selection, validator/runtime fail-closed gates, and direct/recipe Run Report persistence.
Acceptance criteria: checkpoints A-C and Completion Gates 1-8 passed in the bounded current-source smoke. All seven modes passed positive math cases; wrong kind, coordinate mismatch, degenerate segment, far intersection, invalid result ROI, missing Circle ROI, no edge, wrong polarity, insufficient support/coverage, and excessive residual rejected by named gates. Property apply/save/reload retained source identities with zero Preview/Run. Geometry rows and provenance round-tripped through both report paths. Legacy Line, LineDistance, and LineIntersection UI smokes remained green.
Verification: `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"`; `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug`; focused targets `p213_geometry_review,p213_geometry_property_grid`; regressions `wpf_shell_host_line_tool,wpf_shell_host_line_measure_tool,wpf_shell_host_line_intersection_tool`; JSON catalog parse; `git diff --check`.
Evidence: `artifacts/p213_general_geometric_measurement_workspace_20260723/README.md`, current-source PropertyGrid/Geometry Review captures, per-mode and Circle drawings, saved report XML, and legacy regression captures in that artifact folder.
Boundary / next dependency: pixel-only synthetic algorithm/UI verification. It does not prove calibration, industrial semantic accuracy, unseen-data robustness, field qualification, arbitrary automatic feature selection, or experimental OuterCornerIntersection semantics. The next separate priority is bounded two-point scale teaching.
```
