# OpenVisionLab 2D-032 — Blob/Contour 후보·제외 사유와 단일 실행 결과 계약

## 결론

기존 SDK 후보 컬렉션과 `VisionPipelineObjectResultCaptureService`를 그대로 사용해
Blob/Contour의 혼합 accepted/rejected 후보, 동일 면적 객체, 두 ROI 경계, 정확한
area/width/height 제한을 한 번의 실행 결과와 대조했다. 후보 행의 ID·native index·ROI
영역·좌표계·적용 제한·제외 사유가 최종 `ResultCount`, accepted overlay, metric,
Run History 행과 일치했고, 동일 입력을 반복해도 정렬과 메타데이터가 안정적이었다.

새 후보 수집, relaxed Tool 재실행, 후보 abstraction 또는 production 알고리즘 변경은
추가하지 않았다.

## Owner와 call path

1. 기존 `BlobTool`/`ContourTool`이 한 번의 SDK 실행에서 `candidates`를 만든다.
2. `VisionPipelineExecutionService`가 실행을 완료하고
   `VisionPipelineObjectResultCaptureService.TryCaptureSdkCandidates/CaptureCandidates`
   가 native 후보를 `VisionPipelineObjectResult`로 보존한다.
3. 기존 dimension filter가 `Accepted`/`RejectReasonCode`/`RejectReason`와
   `AppliedLimits`를 기준으로 SDK result, metric, accepted rectangle overlay를
   동기화한다.
4. `VisionRecipeRunner` 요약과 `VisionPipelineRunReportStorage`가 같은 후보 행,
   metric, overlay count를 Run History로 저장·재로드한다.

## 검증 매트릭스

결정적 400×150 binary fixture에는 각 ROI에 동일한 24×32 target을 두 개씩 두고,
작은/큰 area와 width/height 경계 객체를 함께 배치했다. Blob의 target area는
768 px, Contour의 target area는 현재 SDK의 외곽선 계산값 713 px로 확인했다.

- `area`: `MIN_AREA=MAX_AREA`를 target area와 동일하게 설정해 exact equality를
  accepted로 유지하고 `AreaBelowMinimum`/`AreaAboveMaximum`을 보존했다.
- `dimension`: `MIN_WIDTH=MAX_WIDTH=24`, `MIN_HEIGHT=MAX_HEIGHT=32`로 설정해
  exact equality를 accepted로 유지하고 네 가지 width/height reject code를 보존했다.
- 각 case는 `USE_MULTI_ROI=true`, `CvROIS=0,0,200,150;200,0,200,150`을 사용했다.
  12개 후보 중 4개 target만 accepted였고, accepted target은 ROI 0/1에 걸쳐 같은
  area와 고유 `CandidateId`를 유지했다.

## Evidence

- Debug contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d032-debug-20260916\blob-contour-candidate-boundary-contract.txt`
- Debug candidate rows/source/Run History: 같은 경로의 `blob\area`, `blob\dimension`,
  `contour\area`, `contour\dimension` 하위 산출물
- Release contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d032-release-20260916\blob-contour-candidate-boundary-contract.txt`
- Focused regressions: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d032-debug-20260916\focused`
  의 object-candidate parity, object-dimension, expected-failure, Preview/Run/Reopen
  산출물

Debug/Release Runner build는 각각 0 errors였고 기존 16개 nullable warning만 남았다.
새 contract와 기존 parity/dimension/expected-failure/Preview·Run·Reopen 회귀는 모두
exit 0이다.

## Scope boundary

`소스/contract 기준 검토 완료 / 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input
interaction, camera/hardware, 장시간 운전은 미검증`이다. 후보 설명과 실행 snapshot
패리티의 근거이며, 현장 정확도·배포·release qualification을 주장하지 않는다.
