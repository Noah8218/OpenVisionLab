# Color, HSV, and Histogram Review

Updated: 2026-07-08

Color inspection starts by separating what changed: brightness, hue, saturation, or channel distribution.

OpenVisionLab currently supports this area with `HSV`, `Mean`, and `Histogram` tool views. The pipeline runner now has an initial `HSV` mask ToolType with `MaskPixelCount` and `MaskPixelRatio` metrics, but a stable public color-classification Good/Bad sample still needs a focused sample pair and smoke evidence before promotion.

## Core Concepts

| Concept | Meaning in OpenCvSharp/OpenVisionLab |
| --- | --- |
| BGR/RGB channel | A color image stores multiple values per pixel, not one GV value |
| HSV | Hue is color family, Saturation is color strength, Value is brightness |
| Color range | Select pixels whose HSV values are inside a chosen range |
| Mean | Summarize brightness or channel drift with one bounded metric |
| Histogram | Review how pixel values are distributed before/after preprocessing |

## Current Tool Coverage

| Tool | Use it for | Current status |
| --- | --- | --- |
| `HSV` | Color range mask review | Initial XML runner support exists with `MaskPixelCount` and `MaskPixelRatio`; no stable public HSV Good/Bad pair yet |
| `Mean` | Brightness or intensity drift judgment | Has public Good/Bad samples and `MeanValueAvg` gates |
| `Histogram` | Contrast and distribution review | Tool view exists; use as visual evidence before adding a stricter step |

## Public Samples To Start With

| Role | SampleName | What it teaches |
| --- | --- | --- |
| Good | `Public_Mean_Brightness_Good` | Normal brightness band with `MeanValueAvg` |
| Bad | `Public_Mean_Brightness_Dark_Bad` | Controlled brightness drift with the same Mean pipeline |
| Good | `Public_HSV_ColorPatch_Good` | Normal red color-mask coverage with `MaskPixelRatio` |
| Bad | `Public_HSV_ColorPatch_Missing_Bad` | Missing red target patches fail the same `MaskPixelRatio` gate |
| Product | `Product_Display_ColorFilterShift_Good` | Color-related product context, currently measured by LineDistance |

The `Product_Display_ColorFilterShift` pair is not an HSV color-classification sample. It is useful context for explaining that a color-looking problem may still require a geometric measurement tool.

## Current Practice Bridge

The Color / HSV Learn topic currently opens Sample Picker path `mean` in some UI entry points. Prefer the public HSV color patch pair when validating color-mask recipes.

- Use `Public_Mean_Brightness_Good` and `Public_Mean_Brightness_Dark_Bad` to practice brightness/channel drift review with `MeanValueAvg`.
- Do not report those samples as HSV color classification evidence.
- Treat an HSV recipe as complete only after a color-range sample, separate output mask, and metric gate such as `MaskPixelRatio`, count, area, or a downstream measurement are available.

## Public Sample Promotion Prerequisite

Do not add public HSV sample rows until these conditions are true:

1. The intended sample pair uses the `HSV` `VisionPipeline` ToolType and not only the exploratory Tool View.
2. The runner emits stable color-mask metrics such as `MaskPixelCount` and `MaskPixelRatio`, or a downstream Blob/Contour metric that represents the selected color area.
3. The Good sample passes and the Bad sample fails for the intended metric, not just for visual appearance.
4. The smoke/readiness evidence proves validation, import, explicit Preview/Run, and result review for that metric.

## HSV Pipeline Runtime Contract

The first stable HSV pipeline slice should be a mask-producing inspection step, not a visual-only color demo.

Required `HSV` pipeline parameters:

- `HueMin`, `HueMax`, `SaturationMin`, `SaturationMax`, `ValueMin`, `ValueMax`
- `USE_ROI` and `CvROI` for optional area restriction
- `InputLayer` and `OutputLayer`, where the output is a separate mask or masked-review layer and the input layer is not changed

Required runner metrics before public Good/Bad samples:

- `MaskPixelCount`
- `MaskPixelRatio`
- `ResultImageWidth`
- `ResultImageHeight`

Required acceptance pattern:

1. Good sample passes a bounded `MaskPixelRatio` range.
2. Bad sample fails the same bounded metric for a controlled color defect or missing color region.
3. The sample catalog records the expected metric values, not just the output image appearance.
4. Smoke evidence confirms that importing, validating, previewing, and running the recipe still require explicit user action.

## What To Check

1. Decide whether the inspection is about hue, brightness, or geometry.
2. If it is brightness drift, start with `Mean` and `MeanValueAvg`.
3. If it is color range selection, use `HSV` as an exploratory preview and keep the output on a separate layer.
4. If the image is low contrast, use `Histogram` to review distribution changes before choosing thresholds.
5. Do not accept a color recipe by appearance only. Add a metric gate or a follow-up tool that produces count, area, score, or distance.

## Common Failures

| Symptom | Likely Cause | First Fix |
| --- | --- | --- |
| Color mask selects background | Hue range is too wide or saturation is too low | Narrow H/S range or add an ROI |
| Same part changes under lighting | Value channel dominates the decision | Use Mean/Histogram evidence or lighting-normalized sample |
| Histogram looks better but detection fails | Contrast changed without a stable downstream metric | Recheck the next Threshold/Blob/Contour step |
| Product defect is geometric, not color | Color words in the request hid the real measurement | Use LineDistance, Matching, or Contour instead |

## Completion Standard

This chapter is partially covered until OpenVisionLab has at least one public HSV Good/Bad sample pair with stable acceptance metrics. `MaskPixelRatio` is available only for the `HSV` pipeline runner path and must not be claimed as public sample evidence until a Good/Bad catalog smoke proves it.

Opening this guide or changing HSV/Histogram/Mean parameters must not run Preview/Run automatically.
