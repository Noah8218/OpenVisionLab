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
- `ErrorCode`: `VisionToolErrorCode.None` for success, otherwise a stable failure category.
- `ErrorCodeValue`: integer value of `ErrorCode` for external DLL callers.
- `ErrorName`: string name of `ErrorCode` for logs, reports, and human review.
- `Metrics`: numeric values used by UI, reports, acceptance, and runner output.
- `Overlays`: review geometry used by preview viewers.

If a tool is a pure image transform, it may return no metrics and no overlays. If a tool detects or measures something, it should return metrics and overlays.

## Standard Error Codes

Tools should fail with a code, not only a message. Messages can change for users, but codes are used by callers, reports, and batch validation.

| Code | Name | When to use |
| --- | --- | --- |
| `0` | `None` | Execution passed |
| `1` | `Unknown` | Legacy or uncategorized failure |
| `100` | `InputImageInvalid` | Source image is null/empty/invalid |
| `101` | `InputLayerMissing` | Pipeline input layer has no image |
| `110` | `InvalidRoi` | ROI is outside image bounds or invalid |
| `120` | `InvalidParameter` | Tool parameter cannot be used safely |
| `121` | `ToolPropertyMissing` | Tool property object was not assigned |
| `130` | `TemplateImageMissing` | Template-based tool has no template image |
| `131` | `TemplateImageInvalid` | Template image is present but cannot be used |
| `200` | `ToolFactoryFailed` | Pipeline could not create the requested tool |
| `210` | `ToolExecutionException` | Tool threw during execution |
| `220` | `OpenCvExecutionFailed` | OpenCV raised an assertion or bad-argument failure |
| `300` | `StepTimeout` | Pipeline step exceeded timeout |
| `301` | `StepCanceled` | Pipeline step was canceled |
| `350` | `ThresholdInvalidRange` | Threshold range min/max is invalid |
| `351` | `ThresholdInvalidMaxValue` | Threshold max value is invalid |
| `352` | `ThresholdInvalidAdaptiveBlockSize` | Threshold adaptive block size is invalid |
| `360` | `MorphologyInvalidKernel` | Morphology kernel size is invalid |
| `361` | `MorphologyInvalidIterations` | Morphology iteration count is invalid |
| `370` | `FilterInvalidKernel` | Filter kernel size is invalid |
| `371` | `FilterInvalidSigma` | Filter sigma value is invalid |
| `380` | `EdgeDetectionInvalidThreshold` | Edge threshold range is invalid |
| `381` | `EdgeDetectionInvalidKernel` | Edge kernel size is invalid |
| `382` | `EdgeDetectionInvalidDerivative` | Edge derivative parameter is invalid |
| `400` | `ContourInvalidAreaRange` | Contour area min/max is invalid |
| `401` | `ContourRoiInvalid` | Contour ROI configuration is invalid |
| `402` | `ContourInvalidAdaptiveBlockSize` | Contour internal adaptive threshold block size is invalid |
| `403` | `ContourNoResult` | Contour executed but no contour passed ROI/area filters |
| `500` | `BlobInvalidAreaRange` | Blob area min/max is invalid |
| `501` | `BlobRoiInvalid` | Blob ROI configuration is invalid |
| `502` | `BlobLabelingFailed` | Blob labeling failed inside OpenCV/OpenCvSharp.Blob |
| `503` | `BlobInvalidAdaptiveBlockSize` | Blob internal adaptive threshold block size is invalid |
| `504` | `BlobNoResult` | Blob executed but no blob passed ROI/area filters |
| `600` | `MatchingTemplateMissing` | Template matching has no template image |
| `601` | `MatchingTemplateInvalid` | Template matching template cannot be used |
| `602` | `MatchingRoiInvalid` | Template matching ROI configuration is invalid |
| `603` | `MatchingInvalidScale` | Template matching scale/magnification makes the working image invalid |
| `604` | `MatchingInvalidAngleStep` | Template matching angle-search step is invalid |
| `605` | `MatchingInvalidAdaptiveBlockSize` | Matching internal adaptive threshold block size is invalid |
| `606` | `MatchingNoResult` | Template matching executed but no candidate passed `ScoreMin` |
| `700` | `LineGaugeRoiInvalid` | LineGauge ROI configuration is invalid |
| `701` | `LineGaugeInvalidSampling` | LineGauge sampling parameters are invalid |
| `702` | `LineGaugeInvalidAdaptiveBlockSize` | LineGauge internal adaptive threshold block size is invalid |
| `703` | `LineGaugeEdgeNotFound` | LineGauge executed but no stable edge points were found |
| `704` | `LineGaugeFitFailed` | LineGauge found edge points but could not produce a valid fit line |
| `800` | `MeanRoiInvalid` | Mean/brightness ROI configuration is invalid |
| `801` | `MeanInvalidAdaptiveBlockSize` | Mean internal adaptive threshold block size is invalid |
| `900` | `FeatureTemplateMissing` | Feature matching has no template image |
| `901` | `FeatureRoiInvalid` | Feature matching ROI configuration is invalid |
| `902` | `FeatureTemplateInvalid` | Feature matching template cannot be used |
| `903` | `FeatureInvalidAdaptiveBlockSize` | Feature matching internal adaptive threshold block size is invalid |
| `904` | `FeatureNoKeypoints` | Feature matching could not extract keypoints/descriptors |
| `905` | `FeatureNotEnoughMatches` | Feature matching found too few good matches for homography |
| `906` | `FeatureHomographyFailed` | Feature matching could not calculate a valid homography |
| `907` | `FeatureNoResult` | Feature matching executed but did not produce a result |
| `1000` | `RotateScaleInvalidScale` | Rotate/scale scale percentage is invalid |

Tool-specific detail codes can be added later, but they should remain stable once published.

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
| `SourceImageWidth` | Input image width used by the tool |
| `SourceImageHeight` | Input image height used by the tool |
| `SourceImageChannels` | Input image channel count used by the tool |
| `ResultImageWidth` | Result image width returned by the tool |
| `ResultImageHeight` | Result image height returned by the tool |
| `ResultImageChannels` | Result image channel count returned by the tool |

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
| `Threshold` | Required | image metrics | Optional |
| `Morphology` | Required | image metrics | Optional |
| `Filter` | Required | image metrics | Optional |
| `EdgeDetection` | Required | image metrics, optional `EdgePointCount` | Optional points |
| `Blob` | Required | image metrics, `ResultCount`, area, angle | Rectangles |
| `Contour` | Required | image metrics, `ResultCount`, area, angle | Rectangles or points |
| `LineGauge` | Required | image metrics, `ResultCount`, `EdgeCount`, `EdgePointCount`, line length, mm-converted line length | Lines and points |
| `Matching` | Required | image metrics, `ResultCount`, score, angle | Rectangles |
| `Mean` | Required or source clone | image metrics, mean metrics | Optional ROI rectangle |
| `FeatureMatching` | Required | image metrics, `ResultCount`, score | Rectangles/points |
| `RotateScale` | Required | image metrics | Optional |

Result-list metrics should expose both the total count and aggregate values where the result object has the matching property:

- Count: `ResultCount`
- Area tools: `AreaMin`, `AreaMax`, `AreaAvg`
- Score tools: `ScoreMin`, `ScoreMax`, `ScoreAvg`
- Angle tools: `AngleMin`, `AngleMax`, `AngleAvg`
- Line tools: `EdgeCount`, `EdgeCountMin`, `EdgeCountMax`, `EdgeCountAvg`, `EdgePointCount`, `EdgePointCountMin`, `EdgePointCountMax`, `EdgePointCountAvg`, `LineLengthMin`, `LineLengthMax`, `LineLengthAvg`, `LineLengthMmMin`, `LineLengthMmMax`, `LineLengthMmAvg`
- LineDistance tools: `DistanceCount`, `DistancePxMin`, `DistancePxMax`, `DistancePxAvg`, `DistanceMmMin`, `DistanceMmMax`, `DistanceMmAvg`
- Measurement metrics: when `PIXELPERMM` is available, rectangle and line overlays also expose `BoundsWidthMm*`, `BoundsHeightMm*`, and `LineLengthMm*` values. Use `LineLengthMm*` for fitted line length only; use `DistanceMm*` for edge-to-edge spacing.

Matching must not report the same physical location repeatedly when `NUM_MATCH > 1`. Candidate suppression should remove the matched region before the next search, and high-score candidates from flat/background regions should be rejected when the candidate image differs too much from the template.

Matching `Score` metrics are normalized as a quality score where a higher value is always better. For OpenCV `SqDiff` modes, the raw OpenCV score is inverted before it is exposed as `Score`, `ScoreMax`, or compared with `SCORE_MIN`.

## Acceptance Rule

Acceptance should use standard metrics only. A first-pass recipe should prefer loose ranges:

- Count exists: `ResultCount >= 1`
- Text/symbol candidates: `ResultCount` loose range
- Matching: `ScoreMax >= threshold`
- Line/edge: `EdgeCount >= 1`
- Distance/size: `DistanceMmAvg` or `DistanceMmMin/Max` for edge-to-edge spacing, `BoundsWidthMmMax` or `BoundsHeightMmMax` for object box size, and `LineLengthMmMax` only for fitted line length after `PIXELPERMM` is set
- Brightness: `MeanValueAvg` min/max

## Status Meaning

Pipeline status should distinguish execution failures from inspection decisions:

| Status | Meaning |
| --- | --- |
| `OK` | Tool executed and acceptance passed |
| `NG` | Tool executed, but acceptance failed |
| `ERROR` | Tool could not execute correctly; check `ErrorCodeValue` and `ErrorName` |
| `TIMEOUT` | Tool exceeded the step timeout |
| `CANCEL` | Step was canceled |
| `SKIP` | Step was disabled/skipped |

Use `NG` for recipe/parameter tuning and `ERROR` for configuration, image, ROI, template, or execution failures.

## ResultStatus Meaning

`VisionToolResult.ResultStatus` is the stable machine-readable status for external callers.

| ResultStatus | Meaning |
| --- | --- |
| `Passed` | Tool completed successfully |
| `Failed` | Uncategorized tool failure |
| `InvalidInput` | Source image or pipeline input layer is missing/invalid |
| `InvalidParameter` | Tool parameter is unsafe or impossible to run |
| `InvalidRoi` | ROI is missing, invalid, or outside the image |
| `ConfigurationError` | Tool property, factory, or template configuration is missing/invalid |
| `Timeout` | Pipeline step exceeded timeout |
| `Canceled` | Pipeline step was canceled |
| `Exception` | Tool/OpenCV execution threw an exception |

The mapping from `VisionToolErrorCode` to `ResultStatus` is centralized in `VisionToolResult.ResolveStatus`.
`VisionToolResult.Failed(VisionToolErrorCode.None, ...)` is normalized to `Unknown` and `Failed`; it must never return `Passed`.

## Runner Result Schema

`VisionRecipeRunner` exposes a UI-free result object for external callers.

Required runner fields:

- `SchemaVersion`: current runner result contract version. Current value is `1.2`.
- `PipelineName`, `Success`, `Message`
- `OutcomeText`, `SummaryText`, `ActionSummaryText`, `StepSummaryText`
- `FinalLayer`, `FinalStepName`, `FinalToolType`
- `ResultImageWidth`, `ResultImageHeight`, `ResultImageSizeText`
- `StepCount`, `PassedStepCount`, `FailedStepCount`, `SkippedStepCount`
- `FirstFailedStepIndex`, `FirstFailedStepName`, `FirstFailedErrorCode`, `FirstFailedErrorName`, `FirstFailedResultStatus`
- `FirstFailedDiagnosticHint`, `FirstFailedSuggestedFix`
- `Steps`: per-step summaries with layer, status, image size, metrics, overlays, error information, `DiagnosticHint`, and `SuggestedFix`.

External callers should treat `ErrorCodeValue` / `ErrorName` as stable and treat `Message`, `DiagnosticHint`, and `SuggestedFix` as user-facing text that may be refined.

`ActionSummaryText` is intended for operator-facing or LLM-facing feedback. On OK it points to the final result layer. On NG it points to the first failed step and uses the diagnostic fix text when available.

`StepSummaryText` is intended for logs and external callers that need a compact ordered flow such as `01 OK Threshold Main->Binary | 02 ERROR Contour Binary->Contour Error=...`.

## Validation Baseline

The platform precheck executes runnable sample recipes and verifies that final detection steps return metrics or overlays:

```powershell
powershell -ExecutionPolicy Bypass -File tools\RunVisionPlatformPrecheck.ps1 -FailOnUiWarn
```

Add or update a sample recipe when a new tool becomes pipeline-ready.
