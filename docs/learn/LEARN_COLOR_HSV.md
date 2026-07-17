# Color, HSV, and Histogram Review

Updated: 2026-07-13

Color inspection starts by separating what changed: brightness, hue, saturation, or channel distribution.

OpenVisionLab currently supports this area with `HSV`, `Mean`, and `Histogram` tool views. The pipeline runner has an `HSV` mask ToolType with `MaskPixelCount` and `MaskPixelRatio` metrics, and the public sample catalog includes a focused HSV Good/Bad pair for color-mask practice.

## Core Concepts

| Concept | Meaning in OpenCvSharp/OpenVisionLab |
| --- | --- |
| BGR/RGB channel | A color image stores multiple values per pixel, not one GV value |
| `Vec3b` | One 8-bit three-channel pixel; BGR Mat uses B/G/R and converted HSV Mat uses H/S/V |
| `Cv2.Split` | Separate a multi-channel BGR Mat into three single-channel Mats in B, G, R order |
| `Cv2.Merge` | Recombine single-channel Mats in the supplied order; B, G, R order restores the original BGR pixel |
| `Scalar` | Four numeric slots; HSV lower/upper bounds use the first three slots for H/S/V |
| HSV | Hue is color family, Saturation is color strength, Value is brightness |
| Color range | Select pixels whose HSV values are inside a chosen range |
| Mean | Summarize brightness or channel drift with one bounded metric |
| Histogram | Review how pixel values are distributed before/after preprocessing |

## Related Tools

| Tool | Use it for | Result to review |
| --- | --- | --- |
| `HSV` | Color range mask review | `MaskPixelCount`, `MaskPixelRatio`, output mask |
| `Mean` | Brightness or intensity drift judgment | `MeanValueAvg` and Good/Bad range |
| `Histogram` | Contrast and distribution review | Pixel-value distribution before/after preprocessing |

## Public Samples To Start With

| Role | SampleName | What it teaches |
| --- | --- | --- |
| Good | `Public_Mean_Brightness_Good` | Normal brightness band with `MeanValueAvg` |
| Bad | `Public_Mean_Brightness_Dark_Bad` | Controlled brightness drift with the same Mean pipeline |
| Good | `Public_HSV_ColorPatch_Good` | Normal red color-mask coverage with `MaskPixelRatio` |
| Bad | `Public_HSV_ColorPatch_Missing_Bad` | Missing red target patches fail the same `MaskPixelRatio` gate |
| Product | `Product_Display_ColorFilterShift_Good` | Color-related product context, currently measured by LineDistance |

The `Product_Display_ColorFilterShift` pair is not an HSV color-classification sample. It is useful context for explaining that a color-looking problem may still require a geometric measurement tool.

## Practice Samples Path

Practice Samples path: `color-hsv`

- Use `Public_HSV_ColorPatch_Good` and `Public_HSV_ColorPatch_Missing_Bad` to practice color-range masking with `MaskPixelRatio`.
- Use `Public_Mean_Brightness_Good` and `Public_Mean_Brightness_Dark_Bad` only when the inspection intent is brightness/channel drift with `MeanValueAvg`.
- Treat an HSV recipe as complete only when the output mask is separate from the input layer and an acceptance gate such as `MaskPixelRatio`, count, area, or a downstream measurement is checked.

## Good/Bad Comparison

Use the HSV sample pair in this order:

1. Open the Good image and confirm that the intended color area becomes white in the mask.
2. Record `MaskPixelCount` and `MaskPixelRatio`, or a downstream Blob/Contour metric for that area.
3. Open the Bad image with the same HSV range and ROI.
4. Confirm that the Bad sample differs for the intended color defect in both the mask and metric.

## HSV Parameters And Output

An HSV Step converts the selected color range into a binary mask that later tools can count or measure.

Main `HSV` parameters:

- `HueMin`, `HueMax`, `SaturationMin`, `SaturationMax`, `ValueMin`, `ValueMax`
- `USE_ROI` and `CvROI` for optional area restriction
- `InputLayer` for the source image and `OutputLayer` for the generated mask or review image

Operator type mapping:

`BGR Mat pixel = Vec3b(B,G,R) = (25,185,105)`

`Mat[] bgrChannels = Cv2.Split(bgrMat)` produces three `CV_8UC1` Mats whose sample values are B=25, G=185, and R=105.

`Cv2.Merge(bgrChannels, mergedBgrMat)` restores `Vec3b(B,G,R) = (25,185,105)`. Channel order matters; merging R/G/B would produce a different color.

`Cv2.CvtColor(bgrMat, hsvMat, ColorConversionCodes.BGR2HSV)`

`HSV Mat pixel = Vec3b(H,S,V) = (45,221,185)`

`Cv2.InRange(hsvMat, lowerScalar, upperScalar, mask)` compares each H/S/V channel with the corresponding lower/upper value. A pixel inside all three ranges becomes mask value 255; otherwise it becomes 0.

Result metrics:

- `MaskPixelCount`
- `MaskPixelRatio`
- `ResultImageWidth`
- `ResultImageHeight`

Good/Bad review pattern:

1. Good sample passes a bounded `MaskPixelRatio` range.
2. Bad sample fails the same bounded metric for a controlled color defect or missing color region.
3. The sample catalog records the expected metric values, not just the output image appearance.
4. The result image, mask ratio, and any downstream count/area metric tell the same Good/Bad story.

## What To Check

1. Decide whether the inspection is about hue, brightness, or geometry.
2. If it is brightness drift, start with `Mean` and `MeanValueAvg`.
3. If it is color range selection, use `HSV` and compare the source image with the output mask.
4. If the image is low contrast, use `Histogram` to review distribution changes before choosing thresholds.
5. Confirm the visible color difference with a metric gate or a follow-up count, area, score, or distance result.

## Learn To Tool View

Use `HSV Tool 열기` in this Learn topic to locate the HSV PropertyGrid and route controls.

In the HSV Tool View, find these PropertyGrid and route controls:

- `Hue Min/Max`, `Saturation Min/Max`, and `Value Min/Max`
- `ROI` / `CvROI` when the background has similar colors
- `InputLayer` and `OutputLayer`; keep the generated mask separate from the source layer

After confirming those values, run Preview/Run and review `MaskPixelRatio` or a downstream count/area metric.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Color mask selects background | Hue range is too wide or saturation is too low | Narrow H/S range or add an ROI |
| Same part changes under lighting | Value channel dominates the decision | Use Mean/Histogram evidence or lighting-normalized sample |
| Histogram looks better but detection fails | Contrast changed without a stable downstream metric | Recheck the next Threshold/Blob/Contour step |
| Product defect is geometric, not color | Color words in the request hid the real measurement | Use LineDistance, Matching, or Contour instead |

## Learning Check

After this topic, you should be able to explain BGR and HSV channel order, choose H/S/V ranges, use ROI to exclude similar background colors, and prove the Good/Bad difference with `MaskPixelRatio` or a downstream count/area metric.
