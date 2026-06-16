# Vision Tool Execution Path Audit

## New Development API Policy

- New OpenVisionLab code must use the `*Tool` execution APIs and `*Result` result models.
- `CV*` and `CResult*` classes are compatibility APIs for older code paths only.
- Legacy compatibility APIs must remain buildable, but they are marked with `ObsoleteAttribute` so new code does not accidentally depend on them.
- `CVLineGuage` keeps its original spelling only for compatibility. New code must use `LineGaugeTool`, `LineGaugeResult`, and `LineGaugeEdge`.
- `CVBlob` keeps the existing OpenCvSharp Blob dependency and DLL version. Refactoring in this area is limited to naming, result contract, validation, and error reporting unless a separate version-up decision is made.

## Result Contract Baseline

- Every tool execution path returns `VisionToolResult`.
- Failure paths must still expose `ResultStatus`, `ErrorCode`, `Message`, and available debug image metrics.
- Detection tools expose count and range metrics such as `ResultCount`, `AreaMin`, `AreaMax`, `ScoreMax`, `AngleAvg`, `EdgeCount`, and `EdgePointCount` when applicable.
- Visual tools expose overlays when a result is detected.
- The smoke target `pipeline_tool_result_contract_check` verifies both runtime result contracts and legacy API marking.

OpenVisionLab의 기준 실행 경로는 `IVisionTool.Execute(Mat)`이다.
이 경로는 `VisionToolResult`, `ErrorCode`, `ResultStatus`, `Metrics`, `Overlays`를 함께 반환하므로 Pipeline, LLM Recipe, 외부 Runner에서 동일한 계약으로 사용할 수 있다.

## Standard Path

- `VisionPipelineExecutionService`는 `tool.Execute(input)`를 사용한다.
- `VisionRecipeRunner`는 Pipeline 실행 결과를 `VisionRecipeRunResult`로 변환한다.
- `VisionRecipeRunResult.SchemaVersion`은 Runner 결과 계약 버전이다.

## Converted Direct Run Paths

아래 경로는 기존에 Tool의 `Run()`을 직접 호출했으나, 현재는 `Execute()` 계약으로 전환되었다.
기존 프리뷰/그리기 UX는 유지하고, 실행 실패만 `VisionToolResult` 기반 메시지로 통일한다.

| Form / Module | Previous Direct Tool Call | Status | Note |
| --- | --- | --- | --- |
| `FormVision_Blob` | `BlobTool.Run()` | Converted | `BlobNoResult` 계약 적용 |
| `FormVision_Contour` | `ContourTool.Run()` | Converted | `ContourNoResult` 계약 적용 |
| `FormVision_Line` | `LineGaugeTool.Run()` | Converted | `LineGaugeEdgeNotFound` 계약 적용 |
| `FormVision_Matching` | `MatchingTool.Run()` | Converted | `MatchingNoResult` 계약 적용 |
| `FormVision_FeatureMatching` | `SiftTool.Run()` | Converted | Feature no-result 세부 ErrorCode 적용 |
| `FormVision_Mean` | `MeanTool.Run()` | Converted | 측정 결과도 `VisionToolResult` 경로 사용 |
| `FormVision_EdgeDetection` | `EdgeDetectionTool.Run()` | Converted | 변환 결과 이미지를 `VisionToolResult.ResultImage`로 사용 |
| `FormVision_Filter` | `FilterTool.Run()` | Converted | 변환 결과 이미지를 `VisionToolResult.ResultImage`로 사용 |
| `FormVision_Morphology` | `MorphologyTool.Run()` | Converted | 변환 결과 이미지를 `VisionToolResult.ResultImage`로 사용 |
| `FormThreshold` | `ThresholdTool.Run()` | Converted | 프리뷰 처리 함수가 `Execute()` 결과를 사용 |
| `InspectionAlgorithm` | `LineGaugeTool.Run()` | Converted | 내부 알고리즘 호출 경로도 `Execute()` 계약 적용 |

## Conversion Rule

1. 각 폼은 `Run()` 대신 `Execute(source)`를 호출한다.
2. `result.Success == false`이면 `result.ErrorCode`, `result.ResultStatus`, `result.Message`를 로그/상태 패널에 표시한다.
3. 검출 없음은 예외가 아니라 표준 NG로 처리한다.
4. 프리뷰 이미지는 기존 방식으로 유지하되, 파이프라인/외부 Runner와 같은 결과 계약을 사용한다.
