# Arithmetic and Logical Operations

Updated: 2026-07-07

Arithmetic explains what happens when two images, one image and a constant, or one image and an offset are combined.

OpenVisionLab already has an `Arithmetic` tool. Use it to understand chapter-style image arithmetic without adding a separate programming lesson.

## Core Concepts

| Concept | Meaning in OpenCvSharp/OpenVisionLab |
| --- | --- |
| Add/Subtract | Brighten, darken, or compare two images by pixel value |
| AbsDiff | Show changed pixels as an absolute difference image |
| Bitwise AND | Keep pixels where both inputs or mask conditions are active |
| Bitwise OR | Combine two binary/result masks |
| Bitwise XOR | Show pixels active in one input but not both |
| Bitwise NOT | Invert a binary or grayscale image |
| Constant input | Use one image plus a fixed scalar value |

## What To Check

1. Confirm `InputLayer` and `InputLayerB` before running.
2. Use `UseConstantInput` only when the second image is not needed.
3. For binary images, verify whether white means object or background.
4. Keep output on a separate layer so the original input remains reviewable.
5. Run Preview explicitly and compare input/output layers.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Output is fully black | Wrong input layer, wrong operation, or values clipped to 0 | Check route and operation |
| Output is fully white | Values saturated to 255 or masks merged too broadly | Lower constant or use AND |
| Object disappears | Binary polarity is wrong | Try Bitwise NOT before logic |
| Validation fails | `InputLayerB` missing for a two-input operation | Select a second input or use constant mode |

## Relation To Recipes

In XML, `Arithmetic` operations are useful before final review layers:

- `AbsDiff` for before/after change evidence.
- `Bitwise_AND` for masking one result with another.
- `Bitwise_OR` for combining multiple defect candidates.
- `Bitwise_NOT` for polarity correction.

Do not use arithmetic to hide unclear inspection intent. A good recipe should still expose the source layers and measurable result metrics.
Opening this guide or changing Arithmetic parameters must not run Preview/Run automatically.
