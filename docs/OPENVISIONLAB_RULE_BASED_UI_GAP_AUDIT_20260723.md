# OpenVisionLab Rule-Based UI And Tool Gap Audit

Updated: 2026-07-23 KST  
Status: Complete

## Decision

Repeated image campaigns, dataset switching, parameter tuning, and LLM XML correction work are closed as active priorities. Do not resume them until the user explicitly requests a named inspection validation.

The current development priority is no longer “run more images.” It is to make proven deterministic capabilities easier to teach, inspect, and compose through the UI. This document is a static source/document audit; it did not execute an image, Preview, Run, batch validation, or LLM provider workflow.

## Scope

Included:

- Current rule-based algorithm/tool inventory.
- Current PropertyGrid, ROI/template, Pipeline Review, Run History, and result-evidence UI.
- UI and deterministic-tool comparison against official commercial product documentation.
- A bounded shortlist of missing capabilities.

Excluded:

- New inspection recipes or image qualification.
- More N-sample or large-corpus execution.
- LLM XML authoring/correction/provider automation.
- Camera, lighting, PLC, I/O, account, deployment, MES, controller, 3D, and deep-learning platform scope.

## Current Static Inventory

The machine-readable catalog currently defines 23 canonical tool families and 42 accepted ToolType names/aliases.

| Area | Present capability | Static assessment |
| --- | --- | --- |
| Preprocessing | Threshold, Morphology, Filter, EdgeDetection, HSV, Arithmetic, RotateScale/NormalizeImage | Broad enough for the current workbench direction. |
| Segmentation/inspection | Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching, ReferenceDifference | Core families and P211 per-object review exist; P216 adds optional axis-aligned pixel width/height filtering while richer region descriptors remain deferred. |
| Metrology | LineGauge, LineDistance, PinArrayGap EdgeGap/CenterPitch, CurveBandProfile, LineIntersection, CircleGauge, GeometryMeasure | P213 adds bounded reusable point/segment/circle relations. `OuterCornerIntersection` remains experimental. |
| Fixture/relative ROI | Matching pose, reference teach, NormalizeImage, fixed reference-coordinate downstream ROI | P212 consolidates one supported chain into a visual Pipeline Review workflow. Locator qualification remains recipe-specific. |
| Evidence/review | Metrics, overlays, explicit Preview/Run, Pipeline Review, saved reports, Good/Bad validation, deterministic review queue, Blob/Contour object rows | Strong aggregate and per-object evidence exists; the object acceptance controls lag the displayed evidence. |
| Calibration | Scalar legacy `PIXELPERMM`, mm metrics, P214 two-point scale record/apply | One hash-locked uniform image-plane scale is teachable; lens/camera calibration and certified metrology remain out of scope. |

### Source findings that define the gaps

- P215 found that `BlobProperty` and `ContourProperty` exposed only `MIN_AREA` and `MAX_AREA` despite P211 already retaining `BoundsWidth` and `BoundsHeight`. P216 closes that exact gap with optional per-object pixel width/height ranges before `ResultCount`; aggregate acceptance remains a separate whole-Step decision.
- P211 continues to make current Blob/Contour rows first-class report evidence with table/drawing selection. P216 reuses that contract for exact dimension reject reasons rather than adding another results table.
- P212, P213, and P214 close the previously selected bounded Fixture designer, reusable geometry, and two-point uniform-scale slices. Their documented limits remain; this reassessment does not reopen their datasets or add another metrology family.
- The shell currently offers two broad groups (`IMAGE PROCESSING` and `ALGORITHM`) plus search terms for intent, parameter, and result. A Locate/Segment/Measure/Compare/Review reorganization is not selected without evidence that the current search/grouping blocks an operator task.
- Production source search still found no OCR, barcode/2D-code reader, or general region-feature evaluator. Those absences are real, but no approved inspection currently requires them.

## Commercial Comparison

The comparison is limited to product patterns that fit OpenVisionLab’s workbench scope.

| Commercial pattern | Official evidence | OpenVisionLab gap to adopt |
| --- | --- | --- |
| Image-centric tool teaching and results table | Cognex EasyBuilder lets operators define features through image graphics, edit a tool by selecting its region/result row, and monitor a Results Table. | Make detected results selectable from both image and table; selecting either side must focus the same object/tool evidence. |
| Fixture as a reusable coordinate output | Cognex fixtures orient downstream tool regions from a located pattern/blob/edge feature. | Present the existing Matching -> NormalizeImage -> reference ROI chain as one named visual fixture workflow. |
| Interactive measurement suggestion | MVTec MERLIC easyTouch previews and confirms circles or row-spacing edges while adapting parameters. | Add feature-first graphical teaching for supported measurements instead of requiring operators to infer every ROI/direction/polarity value from PropertyGrid alone. |
| Rich region/object evaluation | MERLIC Evaluate Regions supports area, circularity, convexity, rectangularity, center, width/height, radii, orientation, holes, diameter, gray values, and accepted/rejected region outputs. | Extend Blob/Contour from area-only filtering to an operator-visible object-feature filter and result inspector. |
| Composable geometry | MERLIC exposes point-to-point, segment-to-point, segment-to-segment, circle-to-segment, distance, and angle measurements. KEYENCE describes intersections, midpoints, distances, and click-to-extract line results. | Add a small reusable geometry-result model before proliferating one-off inspection-specific tools. |
| Intent/category-oriented tool selection | KEYENCE CV-X exposes an icon-driven tool catalog and application navigator so users choose by inspection purpose. | Keep PropertyGrid tools but group entry points by Locate, Segment, Measure, Compare, and Review intent. Do not create another wizard-only editor. |
| Graphical dataflow at larger scale | Zebra Aurora Vision Studio emphasizes graphical, ready-to-use filter composition. | Existing Pipeline is sufficient; improve visual result/feature links instead of replacing it with a new graph engine. |

Official sources:

- Cognex EasyBuilder development environment and Results Table: https://docs.cognex.com/is_621/web/EN/ise/Content/GettingStarted/DevEnvironment.htm
- Cognex feature/region editing: https://docs.cognex.com/is_573/web/EN/ezb/Content/EasyBuilder/Locate_AddDeleteEditTool.htm
- Cognex fixture contract: https://docs.cognex.com/is_592/web/en/ezb/content/easybuilder/Locate_Fixture.htm
- MVTec MERLIC Measure Circle: https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/Tool_reference/Processing/Measuring/measure_circle.html
- MVTec MERLIC Measure Row Spacing: https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/Tool_reference/Processing/Measuring/measure_row_spacing.html
- MVTec MERLIC Evaluate Regions: https://www.mvtec.com/doc/merlic/5.8/manual/en-us/Content/Tool_reference/Evaluation/evaluate_regions.html
- MVTec MERLIC Combined Measuring: https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/Tool_reference/Processing/Combined_measuring/combined_measuring.html
- KEYENCE CV-X: https://www.keyence.com/products/vision/vision-sys/cv-x100/
- KEYENCE Vision Systems: https://www.keyence.com/products/vision/vision-sys/
- Zebra Aurora Vision Studio: https://www.zebra.com/us/en/products/oem/software/aurora-vision-studio.html

## Selected Priority Backlog

### 1. Object Results Inspector — completed in P211

Purpose: make Blob/Contour detections explainable without adding another inspection algorithm.

Minimum UI:

- Selected Step result table with object number, accepted/rejected state, area, center X/Y, bounding W/H, and angle.
- Clicking a row highlights exactly that object in the current result drawing; clicking an object selects the matching row.
- A rejected row states which enabled filter failed.
- PropertyGrid remains the authoritative parameter editor; Preview/Run remains explicit.
- No automatic layer/routing changes and no new batch execution.

Algorithm contract needed only to support the UI:

- Persist stable per-object result rows from Blob/Contour into the Step result/report contract.
- Add feature filters only after the table exposes a concrete operator need. Start with existing area filtering; do not implement every commercial feature at once.

Recommended model: gpt-5.6-sol | Reasoning effort: high.

Implementation status: Complete for the bounded first slice. Pipeline Review now provides the named Blob/Contour rows, area-filter reasons, same-run table/drawing selection, and report persistence. Contour rejected candidates are intentionally bounded to the near-filter range at or above 25% of `MIN_AREA` to prevent pixel-noise flooding. P216 later adds width/height gates to this same object contract. Evidence: `artifacts\p211_object_results_inspector_20260723`.

### 2. Fixture And Relative-ROI Designer

Purpose: expose the already implemented coordinate-normalization workflow as one teachable UI.

Minimum UI:

- Named fixture producer and consumer relationship.
- Reference image/template, reference pose and image size, current match pose/score/margin, and valid-pixel status.
- Source and normalized-image preview with the same downstream ROI drawn in reference coordinates.
- Explicit `Teach reference`, `Edit template/search ROI`, `Edit measurement ROI`, and `Run Review` actions.
- Saving a reference or ROI never runs the pipeline automatically.

This is UI consolidation over existing Matching/NormalizeImage/ROI behavior, not a new locator algorithm.

Recommended model: gpt-5.6-sol | Reasoning effort: high.

Implementation status: Complete for the bounded P212 slice. Pipeline Review now resolves one named producer/NormalizeImage/downstream-ROI chain, shows the template/search ROI, reference/current pose, score/same-template margin/valid pixels, and draws the same saved ROI on source and normalized images. Reference teach, producer edit, measurement-ROI edit, and Run Review reuse existing explicit workflows with no tab-selection execution or layer/routing mutation. Evidence: `artifacts\p212_fixture_relative_roi_designer_20260723`.

### 3. General Geometric Measurement Workspace

Purpose: replace one-off geometry tools with reusable detected features and relationships.

First bounded slice:

- Circle/arc measurement with center, radius/diameter, edge completeness, fit residual, and drawing.
- Reusable point, line/segment, and circle results.
- Point-to-point distance, point-to-line distance, line-to-line distance/angle/intersection, and circle-to-line distance.
- Image selection or ROI establishes the feature; PropertyGrid exposes exact parameters and gates.

Its data/result model and visual teaching contract were approved and completed on 2026-07-23. P213 now provides the bounded typed results, radial circle fit, relationship math, PropertyGrid source selection, Geometry Review, persistence, and fail-closed gates.

Recommended model: gpt-5.6-sol | Reasoning effort: high.

Contract status: Complete. `docs\OPENVISIONLAB_GENERAL_GEOMETRIC_MEASUREMENT_WORKSPACE_CONTRACT.md` and `artifacts\p213_general_geometric_measurement_workspace_20260723` record the exact pixel-only scope and passing evidence. This does not qualify calibration, industrial semantic accuracy, unseen robustness, or field use.

### 4. Calibration/Scale Teaching — completed in P214

Purpose: replace manual `PIXELPERMM` entry with traceable evidence when physical units are required.

Minimum bounded scope is a two-point known-distance scale wizard with source image/hash, points, known distance/unit, derived mm-per-pixel value, and explicit apply. Lens distortion, camera calibration, robot coordinates, and field qualification remain out of scope.

Recommended model: gpt-5.6-sol | Reasoning effort: high.

Implementation status: Complete for the bounded P214 slice. Pipeline Review records two same-run point identities, the source image hash, the operator-supplied real distance/unit, and one derived uniform mm-per-pixel value before an explicit per-Step apply. It is not lens/camera calibration, uncertainty estimation, certified metrology, or field qualification. Evidence: `artifacts\p214_two_point_scale_teaching_20260723`.

## P215 Post-P214 Reassessment

### Decision

Select exactly one next deterministic slice: **Blob/Contour object dimension filters v1**.

The operator task is concrete: after Blob or Contour segmentation, reject individual components that pass the area range but have the wrong bounding width or height before deriving `ResultCount` and downstream evidence. Aggregate Step acceptance such as `BoundsWidthMax <= value` can reject a whole run, but it cannot remove the wrong component or explain a per-object dimension reject. P211 already displays each object's width and height. Historical P205 drawing evidence also showed why area alone is insufficient: a rail fragment could occupy a plausible area while representing the wrong physical shape. P215 uses that historical observation only as a product need; it does not reopen or rerun the missing-pin campaign.

### Bounded implementation contract

- Add optional minimum/maximum bounding-width and bounding-height gates to Blob and Contour PropertyGrid, Pipeline/XML mapping, runtime acceptance, saved reports, and P211 reject reasons.
- Missing new XML keys must preserve current area-only behavior.
- A rejected object must state the exact failed dimension gate, and the P211 row/drawing selection must continue to identify the same object.
- Preview/Run remains explicit; editing or selecting a gate must not execute the tool, create a layer, or change routing.
- Verify with a small deterministic synthetic shape matrix and current-build before/after UI evidence. Do not run an operator dataset, retune a prior recipe, or infer industrial accuracy.
- Do not include angle, aspect ratio, circularity, convexity, rectangularity, holes, gray-value features, automatic easyTouch-style threshold selection, or a generic region algebra/dataflow engine in v1.

Recommended model: gpt-5.6-sol | Reasoning effort: high.

### Candidates deliberately not selected

| Candidate | Decision | Reason |
| --- | --- | --- |
| Full MERLIC-style region feature evaluator | Defer | It would add many descriptors and a new region-result composition contract before a concrete OpenVisionLab inspection needs them. |
| easyTouch-style automatic parameter suggestion | Defer | Useful commercially, but current evidence does not define a stable suggestion target and MERLIC's automatic re-execution pattern conflicts with OpenVisionLab's explicit Preview/Run contract. |
| Locate/Segment/Measure/Compare/Review navigation rewrite | Defer | The shell already has tool search with intent/parameter/result terms. No blocked operator task justifies another navigation pass. |
| OCR/barcode/2D code | Not selected | Commercially common, but outside every currently approved inspection workflow. |
| General region algebra/mask painting | Not selected | Admit only after a named inspection cannot be expressed by current ROI, segmentation, arithmetic, and overlay tools. |
| More image replay or LLM work | Closed | P210/P196 require a new explicit user request. |

## Not Selected Now

- OCR, OCV, barcode, and 2D-code reading: commercially common but not required by the current approved inspection workflow.
- General region algebra/mask painting: useful after an actual inspection cannot be expressed with current ROI and segmentation tools.
- Deep learning/anomaly detection and 3D: outside the current rule-based product direction.
- New Pipeline graph engine: current Pipeline/layer routing already covers the product; improve feature/result links first.
- More image replay, dataset mining, or LLM correction evidence: explicitly stopped until the user names and authorizes a new validation task.

## Feature Admission Checklist

Before implementing any item from this audit:

1. Name the operator task and why the current UI cannot complete it.
2. Identify whether the algorithm already exists and only UI/result plumbing is missing.
3. Define PropertyGrid inputs, result rows/metrics, drawings, and failure reasons.
4. Preserve explicit Preview/Run and layer/routing contracts.
5. Capture fresh before/after UI evidence from the current build.
6. Do not run a dataset or tune an inspection unless the user separately approves that validation.

## Completion Record

```text
Status: Complete
Scope: Static rule-based algorithm/UI inventory, post-P214 commercial UI comparison, one bounded next-slice selection, and repeated-validation stop decision.
Acceptance criteria: current 23-tool/42-name catalog inventoried; P211-P214 closures accounted for; official commercial source rechecked; candidates ranked; exactly one bounded next slice selected; excluded scope and no-repeat rule recorded.
Verification: current source/catalog searches and official MVTec MERLIC 5.8/Cognex documentation review; no image execution, Preview/Run, batch validation, or LLM provider work.
Evidence: docs/OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md
Boundary / next dependency: P216 implements the selected width/height filter v1. The audit remains selection evidence only; use P216 artifacts for runtime claims and perform only a static post-P216 reassessment next.
```

## P216 Implementation Closure

- PropertyGrid and Pipeline/XML expose `MIN_WIDTH`, `MAX_WIDTH`, `MIN_HEIGHT`, and `MAX_HEIGHT` for Blob and Contour. The ranges are axis-aligned pixels.
- Missing keys use `0..1000000`, preserving legacy area-only results. Reversed min/max pairs fail pipeline validation.
- Runtime dimension filtering precedes `ResultCount`, accepted-object metrics, bounds metrics, accepted drawings, and acceptance. Rejected candidates remain in P211/Run History with the exact failed gate.
- The deterministic five-shape matrix passed for both tool families with one accepted object and four exact dimension rejects; legacy missing-key replays retained all five. No operator dataset or LLM workflow was used.
- Fresh current-build PropertyGrid, Blob, Contour, and Pipeline Review evidence passed with unchanged explicit Preview/Run behavior.

Evidence: `artifacts\p216_object_dimension_filters_20260723`.

Status: Complete for the bounded v1 contract. The next action is a static post-P216 reassessment only; do not infer that angle, aspect ratio, circularity, holes, gray features, semantic classification, or another algorithm family is now required.

## P217 Post-P216 Reassessment And Proactive Expansion Closure

### Decision

Select no additional deterministic feature slice.

The static review found a connected operator path rather than a new blocking gap:

- Algorithm tools remain PropertyGrid-driven, and Pipeline Review provides an explicit selected-Step edit handoff.
- `Run Review` remains an explicit action.
- P211 Object Results, P212 Fixture/relative ROI, P213 Geometry Review, and P214 Scale Calibration are available in the same review surface.
- Recipe persistence has an explicit round-trip validator.
- Run Reports retain per-Step drawings and object rows; batch summaries retain the deterministic review queue; saved evidence opens read-only without automatically rerunning Preview or Run.
- P216 closes the one concrete per-object acceptance mismatch selected by P215.

The remaining commercial candidates are product possibilities, not current work:

| Candidate | P217 decision | Evidence required before reopening |
| --- | --- | --- |
| Angle/aspect/circularity/holes/gray object filters | Defer | A named inspection where area plus axis-aligned width/height cannot express the operator's accepted object set. |
| Automatic parameter or feature suggestions | Defer | A repeated teaching failure with a stable suggestion target that preserves explicit Preview/Run. |
| Locate/Segment/Measure/Compare/Review navigation rewrite | Defer | A current operator task blocked by the existing tool search/groups and edit handoff. |
| OCR/barcode/2D code | Not selected | An explicitly approved inspection intent requiring decoded text/code results. |
| Region algebra/mask painting | Not selected | A named inspection that current ROI, segmentation, arithmetic, and overlay tools cannot compose. |
| Another algorithm family | Not selected | A proven deterministic inspection requirement that no existing family can implement. |
| Image campaigns or LLM work | Closed | A new explicit user request naming that validation or reopening the LLM track. |

### Feature admission gate after P217

Implementation restarts only when all of these are available:

1. A named operator task.
2. A current-source reproduction showing exactly where the existing workflow fails.
3. Evidence that the failure cannot be completed with the current PropertyGrid, Pipeline, review, and persistence contracts.
4. A smallest bounded input/result/drawing/failure contract.
5. A verification plan that preserves explicit Preview/Run and avoids an unapproved image campaign.

### Completion record

```text
Status: Complete
Scope: Static post-P216 PropertyGrid -> explicit Run -> Pipeline Review -> recipe/report persistence reassessment and proactive-expansion decision.
Acceptance criteria: current operator path checked in source; P211-P216 closures accounted for; remaining candidates tested against the feature-admission checklist; at most one slice selected; repeated image/LLM stop rule preserved.
Verification: targeted current-source/document searches plus OpenVisionReadinessCheck and git diff checks; no image execution, Preview/Run, batch validation, recipe tuning, UI change, algorithm change, or LLM provider work.
Evidence: artifacts/p217_post_p216_workflow_reassessment_20260723 and docs/OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md
Boundary / next dependency: no active feature priority. A concrete operator-blocking workflow or verified regression must be reproduced before implementation resumes.
```
