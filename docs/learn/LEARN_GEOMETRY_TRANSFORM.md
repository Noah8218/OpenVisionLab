# Geometric Transform Operations

Updated: 2026-07-08

Geometric transforms change where pixels appear. In OpenVisionLab this is mainly covered by the `RotateScale` tool.

Use this topic to understand rotation, scale, interpolation, border fill, and why ROI or measurement coordinates must be reviewed again after transforming an image.

## Core Concepts

| Concept | Meaning in OpenCvSharp/OpenVisionLab |
| --- | --- |
| Rotate | Move pixels around an image center by an angle in degrees |
| Scale X/Y | Resize width and height independently by percent |
| Interpolation | Decide how new pixel values are estimated between old pixels |
| Border type | Decide what fills empty areas created by rotation |
| Coordinate change | A point or ROI before transform may not match the same object after transform |

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

## Open The Tool From Learn

- `Rotate / Scale Tool 열기` selects the existing RotateScale Tool View.
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

## Relation To Recipes

Use `RotateScale` when the recipe normalizes image orientation or size before detection. Confirm the effect with a measurable result such as count, score, area, distance, or output size.

Keep the transform result on a descriptive output layer and use Preview to re-check ROI, Template, edge direction, and pixel/mm coordinates.
