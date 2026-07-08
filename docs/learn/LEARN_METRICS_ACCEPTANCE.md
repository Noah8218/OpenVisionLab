# Metrics and Acceptance Gates

Updated: 2026-07-07

OpenVisionLab recipes should explain OK/NG with measurable values, not only with a good-looking overlay.

A metric is the value produced by a tool. An acceptance gate is the allowed range for that metric.

## Core Concepts

| Concept | Meaning in OpenVisionLab |
| --- | --- |
| Metric | Numeric result such as count, score, area, mean, distance, angle, or output size |
| Minimum gate | Fails when the value is too low |
| Maximum gate | Fails when the value is too high |
| ExpectedSuccess | Whether the step is expected to pass or fail for the selected sample |
| Good/Bad pair | Same recipe on controlled OK and NG samples |
| Review overlay | Visual evidence that explains where the metric came from |

## Common Metric Families

| Task | Typical metrics |
| --- | --- |
| Presence / matching | `ScoreMax`, `ResultCount` |
| Blob / contour count | `ResultCount`, `AreaMin`, `AreaMax`, `AreaAvg` |
| Brightness drift | `MeanValueAvg`, `MeanValueMin`, `MeanValueMax` |
| Color mask / HSV | `MaskPixelCount`, `MaskPixelRatio` |
| Edge distance / gap / pitch | `DistancePxAvg`, `DistanceMmAvg`, `DistancePxRange`, `DistanceMmRange` |
| Line angle | `LineAngleAvg`, `LineLengthMax`, `EdgeCount` |
| Transform output | `ResultImageWidth`, `ResultImageHeight` |

## Minimum Tool Gate Cheat Sheet

| Tool family | Minimum Good/Bad gates | Visual evidence |
| --- | --- | --- |
| Matching | `ScoreMax`, `ResultCount` | Box and center on the intended template target |
| EdgeBasedMatching | `ScoreMax`, `ResultCount` | Box on the intended edge shape |
| FeatureMatching | `GoodMatches`, `ScoreMax`, RANSAC/overlay | Homography or overlay on the intended feature target |
| Blob / Contour | `ResultCount`, `AreaMin`, `AreaMax`, `BoundsWidth`, `BoundsHeight` | Binary input plus object boxes or outlines |
| LineDistance | `DistanceMmAvg`, `DistanceMmRange` or `DistanceMmMax` | All sampled distance lines on the intended edges |
| HSV / Mean / Geometry | `MaskPixelRatio`, `MeanValueAvg`, `ResultImageWidth`, `ResultImageHeight` | Mask, brightness region, or output-size comparison |

Use the shortest gate set that separates the current Good/Bad pair for the intended reason. Add more gates only when a visible false pass remains.

## Why Average Is Not Enough

For distance inspections, `DistanceMmAvg` alone can hide one bad long or short line. Use a consistency gate when the measurement creates multiple lines:

- nominal value: `DistanceMmAvg` or `DistancePxAvg`
- consistency: `DistanceMmRange`, `DistancePxRange`, `DistanceMmMax`, or `DistancePxMax`

This is required for pin gap, pitch, width, clearance, and similar edge-to-edge inspections.

## What To Check

1. Confirm which tool creates the metric.
2. Confirm the metric name in the pipeline matches the tool result.
3. Open the Good sample and run Preview/Run explicitly.
4. Check that the value falls inside the Good range.
5. Open the Bad sample from the same pair and run the same recipe.
6. Check that the Bad sample fails for the intended metric, not for a missing input or unrelated exception.
7. Confirm the overlay explains where the value came from.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Good sample fails | Gate too tight, ROI wrong, or tool result missing | Check overlay and result metric |
| Bad sample passes | Gate too loose or wrong metric selected | Tighten range or choose a more discriminating metric |
| Failure says metric missing | Tool did not produce the requested metric | Use a metric supported by that tool |
| Distance looks visually wrong but average passes | Outlier line hidden by average | Add range/max consistency gate |
| Overlay looks right but value is wrong | Pixel/mm, ROI, or selected edge polarity mismatch | Recheck calibration and scan direction |

## Public Samples To Practice

Use the exact sample names from `docs/samples/OpenVisionLab.PublicSampleCatalog.csv`.

| Pair group | Good sample | Bad sample | Tool family | Primary gate |
| --- | --- | --- | --- | --- |
| `Public_Matching_DiePad` | `Public_Matching_DiePad_Good` | `Public_Matching_DiePad_NoTarget_Bad` | Matching | Good `ResultCount=3`, `ScoreMax=80..100`; Bad `ResultCount=0` |
| `Public_Blob_Particles` | `Public_Blob_Particles_Good` | `Public_Blob_Particles_Sparse_Bad` | Blob | Good `ResultCount=8..14`; Bad `ResultCount=2..4` |
| `Public_Filter_Denoise` | `Public_Filter_Denoise_Good` | `Public_Filter_Denoise_Missing_Bad` | Filter + Contour | Good `ResultCount=4`; Bad `ResultCount=2` |
| `Public_EdgeDetection_Shapes` | `Public_EdgeDetection_Shapes_Good` | `Public_EdgeDetection_Shapes_Missing_Bad` | EdgeDetection + Contour | Good `ResultCount=4`; Bad `ResultCount=2` |
| `Public_Morphology_Cleanup` | `Public_Morphology_Cleanup_Good` | `Public_Morphology_Cleanup_Missing_Bad` | Morphology + Contour | Good `ResultCount=4`; Bad `ResultCount=2` |
| `Public_HSV_ColorPatch` | `Public_HSV_ColorPatch_Good` | `Public_HSV_ColorPatch_Missing_Bad` | HSV | Good `MaskPixelRatio=0.05..0.07`; Bad `MaskPixelRatio=0.01..0.02` |
| `Public_Mean_BrightnessDrift` | `Public_Mean_Brightness_Good` | `Public_Mean_Brightness_Dark_Bad` | Mean | Good `MeanValueAvg=185..220`; Bad `MeanValueAvg=105..130` |
| `Public_Line_Pins` | `Public_Line_Pins_Good` | `Public_Line_Pins_WidePin_Bad` | LineDistance | Good `DistanceMmAvg=0.20..0.25`; Bad `DistanceMmAvg=0.09..0.13`; add range/max gates for outliers |
| `Public_Edge_Fiducial` | `Public_Edge_Fiducial_Good` | `Public_Edge_Fiducial_Wrong_Bad` | EdgeBasedMatching | Good `ScoreMax=70..100`, `ResultCount=1`; Bad `ResultCount=0` |
| `Public_Geometry_RotateScale` | `Public_Geometry_RotateScale_Good` | - | RotateScale | Good `ResultImageWidth=286`, `ResultImageHeight=210` |

Good/Bad comparison is useful only when the Bad row fails for the intended metric. If it fails because an image path, template path, or input layer is missing, fix the recipe setup first and do not treat that as inspection evidence.

Transform samples may be Good-only when the goal is output metadata such as image size. In that case, the review evidence is the exact output width/height plus explicit input/output layer comparison.

## Completion Standard

A recipe is reviewable when a user can answer:

1. Which image region was measured?
2. Which tool produced the value?
3. Which metric decided OK/NG?
4. Why does the Bad sample fail?
5. Which overlay proves the value came from the intended target?

Changing gates or opening this guide must not run Preview/Run automatically.
