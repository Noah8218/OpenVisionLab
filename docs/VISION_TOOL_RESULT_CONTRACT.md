# Vision Tool Result Contract

This document defines what each OpenVisionLab tool should return after execution.

The purpose is to keep UI preview, pipeline reports, AI Recipe validation, and external runner output consistent.

## Common Result Shape

Every tool execution returns `VisionToolResult`.

Required fields:

- `Success`: true when the tool completed normally.
- `Message`: empty or a clear failure/review message.
- `ResultImage`: the image that should be written to the output layer.
- `Elapsed`: execution time.
- `Metrics`: numeric values used by UI, reports, acceptance, and runner output.
- `Overlays`: review geometry used by preview viewers.

If a tool is a pure image transform, it may return no metrics and no overlays. If a tool detects or measures something, it should return metrics and overlays.

## Standard Metrics

Use the names already defined in `VisionPipelineKnownMetrics`.

| Metric | Purpose |
| --- | --- |
| `ResultCount` | Number of detected objects/results |
| `AreaMin` | Minimum object area |
| `AreaMax` | Maximum object area |
| `AreaAvg` | Average object area |
| `ScoreMin` | Minimum matching score |
| `ScoreMax` | Maximum matching score |
| `ScoreAvg` | Average matching score |
| `AngleMin` | Minimum result angle |
| `AngleMax` | Maximum result angle |
| `AngleAvg` | Average result angle |
| `MeanValueMin` | Minimum mean/intensity value |
| `MeanValueMax` | Maximum mean/intensity value |
| `MeanValueAvg` | Average mean/intensity value |
| `EdgeCount` | Number of edge/line groups |
| `EdgePointCount` | Number of edge points |

## Overlay Kinds

Supported overlay kinds:

- `Rectangle`: detected region, object, match, contour, or blob.
- `Point`: single detected point or center.
- `Points`: many edge/contour points.
- `Line`: measured or fitted line.

Overlay labels should be short and useful. Good labels include count/index, area, score, center, angle, or length.

## Tool Expectations

| Tool | Result Image | Metrics | Overlays |
| --- | --- | --- | --- |
| `Threshold` | Required | Optional | Optional |
| `Morphology` | Required | Optional | Optional |
| `Filter` | Required | Optional | Optional |
| `EdgeDetection` | Required | Optional `EdgePointCount` | Optional points |
| `Blob` | Required | `ResultCount`, area, angle | Rectangles |
| `Contour` | Required | `ResultCount`, area, angle | Rectangles or points |
| `LineGauge` | Required | `ResultCount`, `EdgeCount`, `EdgePointCount` | Lines and points |
| `Matching` | Required | `ResultCount`, score, angle | Rectangles |
| `Mean` | Required or source clone | mean metrics | Optional ROI rectangle |
| `FeatureMatching` | Required | `ResultCount`, score | Rectangles/points |

## Acceptance Rule

Acceptance should use standard metrics only. A first-pass recipe should prefer loose ranges:

- Count exists: `ResultCount >= 1`
- Text/symbol candidates: `ResultCount` loose range
- Matching: `ScoreMax >= threshold`
- Line/edge: `EdgeCount >= 1`
- Brightness: `MeanValueAvg` min/max

## Validation Baseline

The platform precheck executes runnable sample recipes and verifies that final detection steps return metrics or overlays:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn
```

Add or update a sample recipe when a new tool becomes pipeline-ready.
