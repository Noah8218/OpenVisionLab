# OpenVisionLab 2D-030 — ROI·fixture·Affine 좌표 의미 검증

## 결론

기존 owner를 재사용한 선행 계약 검증을 완료했다. fixture의 identity/translation은
저장된 ROI를 바꾸지 않고 runtime effective ROI와 offset metric만 만든다. Affine은
operator가 지정한 source/destination 대응 순서를 그대로 사용하며 identity,
translation, 90-degree rotation, horizontal flip의 2 x 3 pixel matrix, destination
point overlay, XML reopen, Run History metric, overlay export가 서로 같은 좌표를
유지했다.

새 production abstraction이나 두 번째 좌표 계산기는 추가하지 않았다.

## Owner와 call path

1. `VisionPipelineExecutionService.RunAsync`가 기존 fixture frame을 먼저 적용한다.
2. `VisionPipelineFixtureFrameService.PrepareRuntimeStep`가 같은 source layer의
   fixture를 확인하고 translation-only v1의 `CvROI` clone/effective ROI를 만든다.
   `AddApplicationMetrics`가 `FixtureOffsetX/Y`와 `FixtureEffectiveRoiX/Y`를 기록한다.
3. 같은 execution path에서 `VisionPipelineAffinePointBindingService`가 필요한
   detected point를 해석한 뒤, 실제 Affine 계산은 SDK tool owner가 수행한다.
4. `VisionRecipeRunner`가 metric과 overlay를 `VisionRecipeStepRunSummary`로
   투영하고, `VisionPipelineRunReportStorage`가 metric·result·overlay image를
   Run History로 저장한다.
5. ROI definition/bounds gate는 기존 `VisionPipelineValidator`와 execution
   `ValidateStepInput` owner가 담당한다. scalar calibration은 기존
   `VisionPipelineMetricEnrichmentService`/`PIXELPERMM` contract가 담당한다.

## 검증한 의미와 실패 경계

- Affine cases: identity, translation, 90-degree rotation, horizontal flip.
- 각 case에서 SDK matrix와 `AffineM11..AffineM23`를 비교하고, 세 destination
  point overlay의 중심 좌표를 확인했다.
- pipeline XML save/reopen 후 matrix와 overlay 좌표가 동일했다.
- Run History reload 후 affine metric, overlay count, exported overlay image를
  확인했다.
- fixture identity/translation은 effective ROI 및 offset metric을 확인했다.
- fixture rotation(translation-only limit 초과)과 missing frame은 fail-closed다.
- singular destination triangle은 shared validator가 거부한다.
- malformed/out-of-bounds ROI와 missing/non-finite/negative scalar calibration은
  각각 기존 2D-010 ROI 및 2D-014 `PIXELPERMM` contract가 계속 담당한다.
- 비등방 scale을 단일 scalar mm/px로 추론하지 않는다. Affine 결과는 pixel frame으로
  남고, 유효한 scalar calibration이 없으면 mm metric을 만들지 않는다.

## Evidence

- Debug contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d030-debug-20260916\coordinate-transform-meaning\coordinate-transform-meaning-contract.txt`
- Debug regressions: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d030-debug-20260916\affine-transform`, `affine-detected-points`, `roi-meaning`, `pixelpermm-finite-unit`
- Release contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d030-release-20260916\coordinate-transform-meaning\coordinate-transform-meaning-contract.txt`
- Release regressions: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d030-release-20260916\affine-transform`, `affine-detected-points`, `roi-meaning`, `pixelpermm-finite-unit`

Debug/Release 모두 contract와 regression process exit code 0이다. Smoke build는
0 errors였고 기존 nullable warning 16개가 남아 있다. 새 2D-030 contract는 해당
warning을 추가하지 않았다.

## Scope boundary

`소스/contract 기준 검토 완료 / 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input
interaction, camera/hardware, 장시간 운전은 미검증`이다. 따라서 이 결과는 좌표
의미와 persistence/export 계약의 증거이지 현장 정확도나 release qualification의
주장이 아니다.
