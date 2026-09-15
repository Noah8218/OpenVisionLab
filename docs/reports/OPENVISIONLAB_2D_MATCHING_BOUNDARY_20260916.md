# OpenVisionLab 2D-031 — Matching 무검출·다중검출·score 경계 계약

## 결론

기존 Matching owner를 재사용한 결정적 선행 계약 검증을 완료했다. 빈/비유사
입력은 `MatchingNoResult`로 fail-closed되고 양의 `ResultCount`나 overlay를
게시하지 않는다. 두 개의 동일 target은 `NUM_MATCH=2`와 두 rectangle overlay로
보존되며, acceptance·XML 저장/재열기·Run History export가 같은 결과를 유지한다.
관측 score 97.434067%는 `SCORE_MIN=0.925624`에서 통과하고 `SCORE_MIN=1`에서
`MatchingNoResult`가 되었다.

새 detector, candidate abstraction, score 재계산 또는 production Matching
변경은 추가하지 않았다.

## Owner와 call path

1. `VisionPipelineAppToolFactory.CreateMatchingTool`가 기존 `MatchingProperty`
   (`MATCH_MODE`, `SCORE_MIN`, `NUM_MATCH`, template path, ROI/전처리)를 SDK
   `MatchingTool`로 구성한다.
2. `VisionPipelineExecutionService.ExecuteStep`가 같은 SDK tool을 실행하고
   `VisionPipelineMatchResultCaptureService`가 native `MatchingResult`의 score,
   center, bounds를 execution snapshot에 캡처한다.
3. `VisionRecipeRunner.RunAsync`가 `ResultCount`, `ScoreMin/Max`, acceptance,
   rectangle overlay와 `MatchingNoResult` 상태를 `VisionRecipeStepRunSummary`로
   투영한다.
4. `VisionPipelineRunReportStorage`가 reopened pipeline의 metric, overlay count,
   overlay image를 Run History로 저장·재로드한다.
5. no-result error 이름과 진단은 기존 `VisionToolErrorCode.MatchingNoResult` 및
   `VisionPipelineExpectedFailureContract` owner를 그대로 사용한다.

## 검증한 의미와 실패 경계

- no-result: deterministic noise source에 template target이 없을 때
  `run.Success=false`, `ErrorName=MatchingNoResult`, `OverlayCount=0`,
  `ResultCount` positive 값 없음.
- multiple-result: 서로 떨어진 두 target에 `NUM_MATCH=2`를 적용하고 exact
  `ResultCount=2` acceptance, ordered finite `ScoreMin/ScoreMax`, 두 target
  중심(약 x=40, x=132)의 rectangle overlay를 확인했다.
- score boundary: 작은 template mutation으로 0과 100 사이의 실제 score를 만들고
  관측값보다 낮은 `SCORE_MIN`은 보존, 높은 `SCORE_MIN`은 `MatchingNoResult`가
  되는 경계를 확인했다. score metric은 percent(예: 97.434067)이고 property의
  `SCORE_MIN`은 0..1 fraction으로 비교된다.
- pipeline XML save/reopen 후 `NUM_MATCH=2` 결과 count와 overlay count가 동일했고,
  Run History reload 후 count·overlay evidence와 exported overlay image가 존재했다.
- contract fixture는 source/template를 BGR로 만들고 `USE_THRESHOLD=false`,
  `USE_ADAPTIVE_THRESHOLD=false`, `USE_CANNY=false`, full-image ROI를 명시했다.
  이는 기본 threshold 설정을 숨겨진 전처리로 두지 않고 현재 Matching property
  의미를 고정하기 위한 contract 입력이며 production default를 바꾸지 않는다.

## Evidence

- Debug contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d031-debug-20260916s\matching-boundary\matching-boundary-contract.txt`
- Debug fixtures/report artifacts: 같은 경로의 `matching_*_source.png`, pipeline XML,
  Run History report와 overlay export
- Release contract: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d031-release-20260916\matching-boundary\matching-boundary-contract.txt`
- Focused regression: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d031-debug-20260916s\focused\expected-failure-contract.txt`,
  `...\focused\preview-run-reopen\preview-run-reopen-equivalence.txt`

Debug/Release build는 각각 0 errors, 새 Matching boundary contract는 각각 exit 0이다.
기존 nullable warning 16개는 남아 있지만 이번 contract가 warning을 추가하지 않았다.
Expected-failure(5개 tool)와 Preview/Run/reopen(Mean·Blob·Contour·Matching·Line,
locale 포함) focused regression도 exit 0이다.

## Scope boundary

`소스/contract 기준 검토 완료 / 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input
interaction, camera/hardware, 장시간 운전은 미검증`이다. 이 결과는 기존 Matching
결과·판정·persistence 의미의 증거이며 현장 정확도나 release qualification을
주장하지 않는다.
