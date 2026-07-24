# Geometric Transform Operations

Updated: 2026-07-23

Geometric transforms change where pixels appear. OpenVisionLab exposes `RotateScale`
for center-based rotation/resize and `AffineTransform` for a taught three-point
source-to-destination mapping.

Use this topic to understand rotation, scale, interpolation, border fill, and why ROI or measurement coordinates must be reviewed again after transforming an image.

## Core Concepts

| Concept | Meaning in OpenCvSharp/OpenVisionLab |
| --- | --- |
| Rotate | Move pixels around an image center by an angle in degrees |
| Scale X/Y | Resize width and height independently by percent |
| Interpolation | Decide how new pixel values are estimated between old pixels |
| Border type | Decide what fills empty areas created by rotation |
| Coordinate change | A point or ROI before transform may not match the same object after transform |
| Affine 3-point mapping | Three non-collinear source points map to three destination points in the same physical-feature order |
| Valid pixel ratio | Fraction of the output canvas covered by transformed source pixels |

## Sample To Try

| Role | SampleName | Criterion |
| --- | --- | --- |
| Required | `Public_Geometry_RotateScale_Good` | `ResultImageWidth=286`, `ResultImageHeight=210` |
| ExpectedFailure | `Public_Geometry_RotateScale_Wide_Bad` | Input width drift creates `ResultImageWidth=320`, so the normal 286 px gate fails |

This public-safe pair resizes `docs\samples\public\Geometry_RotateScale_Synthetic_OK.png` and `docs\samples\public\Geometry_RotateScale_Synthetic_Wide_NG.png` to 50 percent with `RotateScale`. The Good sample proves the expected output size. The Bad sample teaches that geometric preprocessing can expose input-size drift through a measurable output-size gate.

## What To Check

1. Open the `Rotate / Scale` tool.
2. Confirm the input layer is the original image layer.
3. Set the output layer to a new name so the original image remains reviewable.
4. Change `Angle`, `ScaleXPercent`, or `ScaleYPercent`.
5. Run Preview.
6. Compare input and output size, visible crop, border fill, and object position.
7. If another tool follows this step, confirm its ROI is still on the intended object.

## Affine Transform: Three-Point Teaching

Use `AffineTransform` when one deterministic transform must combine translation,
rotation, independent scale, and shear.

1. Choose three stable, well-separated physical points. Do not place them on one line.
2. Record their source image pixel coordinates as point 1, 2, and 3.
3. Define the desired destination pixel coordinates in the same physical-feature order.
4. Set `OutputWidth` and `OutputHeight`, or leave each at `0` to retain the input size.
5. Set the source/destination triangle-area gates high enough to reject accidental
   near-collinear teaching.
6. Set `MinimumValidPixelRatio` to reject unintended clipping.
7. Run Preview explicitly.
8. Review the destination points/triangle, transformed source frame, all six
   `AffineM*` coefficients, and `AffineValidPixelRatio`.
9. Only then teach downstream fixed ROIs on the affine output layer.

The authoritative result is the six-value `2 x 3` matrix. Derived rotation, scale, and
shear metrics are review aids. Reversing point order or mismatching physical features
can create a mathematically valid but semantically wrong image.

### Same-Run Detected Source Points

When the source pose changes per image, the Affine Step can take its three source
points from earlier deterministic Point-producing Steps instead of fixed numeric
coordinates.

1. Create and gate three earlier locators for three different stable physical
   features. A `Matching` Step publishes `Center` only when exactly one usable
   match exists.
2. Keep all three locator outputs and the Affine input on the same unmodified image
   layer and frame.
3. In the Affine PropertyGrid, enable `검출 소스 포인트 사용`.
4. Select source references 1, 2, and 3 in the same physical-feature order as the
   taught destination points.
5. Run explicitly and review each locator drawing, the Affine destination triangle,
   transformed source frame, `AffineDetectedSourcePointCount=3`, resolved source
   coordinates, matrix, and valid-pixel ratio.
6. Run the downstream inspection with an unchanged ROI on the normalized output
   layer.

The runtime fails closed instead of reusing old numeric coordinates when a source is
missing, ambiguous, rejected, duplicated, from another frame, or outside the image.
This wiring does not choose the three features for the operator and does not prove
that the locators are robust on production variation.

## Open The Tool From Learn

- `Rotate / Scale Tool 열기` selects the existing RotateScale Tool View.
- `Affine Transform Tool 열기` selects the PropertyGrid-based Affine Transform Tool View.
- Check the shared input/output layer controls, then `Angle`, `Scale X`, and `Scale Y`. Recipe XML uses `ScaleXPercent` and `ScaleYPercent` for the scale values.
- `OutputSize` shows the width and height of the image produced by Preview.
- Compare the transformed image with the original, then review downstream ROI and measurement positions.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Object is clipped | Rotation moved pixels outside the output frame | Reduce angle or add upstream margin |
| Object looks blocky | Nearest interpolation on grayscale/detail image | Try linear interpolation |
| ROI misses the object | ROI was copied from the pre-transform image | Recreate ROI on the transformed layer |
| Measurement value changed unexpectedly | Scale changed pixel distance | Recheck pixel/mm calibration and acceptance gates |
| Blank corners appear | Rotation creates empty border area | Review border type and downstream threshold polarity |
| Affine teaching is rejected | Source or destination points are collinear or too close | Spread the three points across stable features and review triangle-area gates |
| Detected-point Affine fails before transform | A source reference is missing, duplicated, rejected, ambiguous, wrong-kind, or from another image frame | Review all three earlier locator results and reselect the ordered Point references |
| Valid-pixel coverage fails | Destination points/output size move too much source content outside the canvas | Review destination coordinates and output size before lowering the gate |
| Image is mirrored or sheared unexpectedly | Point ordering or physical correspondences do not match | Re-enter points 1/2/3 in the same physical-feature order |

## Relation To Recipes

Use `RotateScale` when the recipe normalizes image orientation or size before detection. Confirm the effect with a measurable result such as count, score, area, distance, or output size.

Use `AffineTransform` when the destination frame is taught and the source side is
either three fixed numeric points or three explicitly bound same-run typed Point
results. It is not automatic physical-feature selection, perspective correction,
lens calibration, or unconstrained per-image feature tracking.

Keep the transform result on a descriptive output layer and use Preview to re-check ROI, Template, edge direction, and pixel/mm coordinates.
