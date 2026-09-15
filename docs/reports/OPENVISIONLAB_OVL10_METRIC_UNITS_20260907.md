# OVL-10 통합 측정 단위와 좌표 문맥 보존

작성일: 2026-09-07 KST  
상태: **Complete** (OVL-10 독립 slice)

## 범위

`TwoDIntegrationExchange`가 `VisionRecipeRunResult`의 수치를 Integration
`Metric`으로 투영할 때 모든 Step metric을 `unitless`로 보내던 경로를
명시적 전체 이름 매핑으로 교체했다. 숫자와 metric 이름은 그대로 두고,
기존 `VisionPipelineKnownMetrics` 및 MultiMatchMean metric 이름만 정확히
조회한다. 이름에 `Mm`, `Px`, `Ratio`가 포함된다는 이유로 단위를 추론하지
않는다.

변경 파일:

- `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs`
- `src/OpenVisionLab/Core/Integration/TwoDIntegrationMetricUnits.cs`
- `tools/VisionRecipeRunnerSmoke/TwoDIntegrationSmoke.cs`
- `docs/LLM_DOCUMENT_INDEX.json`

과거 Transaction 파일은 수정하지 않았다. Run Record schema는 `1.1`을
유지하고 새 필드는 additive로 기록하므로 기존 consumer가 모르는 JSON
속성은 무시할 수 있다.

## 구조 증거

변경 전:

```text
Step.Metrics → finite 값 필터 → 모든 metric unit = "unitless"
```

변경 후:

```text
Step.Metrics → finite 값 필터
             → TwoDIntegrationMetricUnits의 exact-name 조회
             → 알려진 의미 단위 또는 "unknown"
             → unknown이면 Run Record.metricUnitDiagnostics에 진단 기록
```

매핑 소유자는 `TwoDIntegrationMetricUnits`이며, 실행/파일 교환 소유자인
`TwoDIntegrationExchange`는 투영 결과와 진단을 Run Record와 Result에
전달한다. 이로써 metric 의미 테이블과 거래 오케스트레이션 책임이
분리되지만, 기존 public Result contract와 수치 계산 경로는 건드리지
않는다.

단위 토큰은 현재 계약과 기존 UI 의미를 다음처럼 보존한다.

| 의미 | unit token | 적용 예 |
| --- | --- | --- |
| 물리 길이 | `mm` | `DistanceMm*`, `LineLengthMm*`, `Bounds*Mm`, `Geometry*Mm`, `Circle*Mm` |
| 이미지 좌표·픽셀 길이 | `px` | `SourceImageWidth/Height`, bounds, distance/pitch/curve px, affine 좌표·translation |
| 픽셀 면적 | `px²` | `Area*`, affine source/destination triangle area |
| 각도 | `deg` | `Angle*`, line/fixture/geometry/affine rotation/circle coverage |
| 실행 시간 | `ms` | `totalMilliseconds` |
| 0..1 비율 | `fraction` | mask, valid-pixel, coverage, support, inlier, unique score margin |
| 개수 | `count` | result/edge/distance/pair/support/image-channel/instance count |
| 점수 의미 | `score` | matching, gap, registration, MultiMatchMean score 및 기존 scale |
| 물리 단위가 없는 값 | `unitless` | matrix linear coefficient, scale, state/flag, grayscale, reference index |
| 선언되지 않은 이름 | `unknown` | 새 metric 이름; Run Record에 명시적 diagnostic 추가 |

`AffineM13/M23`는 행렬의 translation coefficient이므로 `px`로 분류하고,
`AffineM11/M12/M21/M22`와 determinant/scale/shear는 무차원으로 유지했다.
`ScoreMargin`은 기존 Matching의 percentage-point 표현을 숫자 변환 없이
`score`로 전달하며, `UniqueMatch.ScoreMargin`은 기존 normalized `0..1`
계약에 따라 `fraction`으로 전달한다.

## 좌표 문맥

`TwoDIntegrationStepRecord`에 `InputLayer`, `OutputLayer`,
`OverlayCoordinateLayer`를 추가했다.

- 일반 측정 Tool overlay는 실제 입력 frame인 `InputLayer`에 연결한다.
- `RotateScale`/`Affine` 계열처럼 출력 canvas에 geometry를 그리는 변환
  Tool은 `OutputLayer`를 기록한다.
- `OverlayMerge`/`ResultMerge` 계열은 서로 다른 이전 source layer의
  overlay를 복사할 수 있으므로 `mixed`를 기록한다.
- layer 정보가 없으면 `unknown`이며, 좌표를 임의로 변환하거나 source로
  가장하지 않는다.

기존 overlay 숫자와 도형은 그대로 저장한다. 소비자는 이 필드를 보고
좌표 frame을 선택해야 하며, 이번 변경은 좌표 변환 자체를 수행하지 않는다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| DistanceMm 계열은 `mm` | 통과 | clean 2D smoke의 Run Record/Result round-trip 검사 |
| 각도 계열은 `deg` | 통과 | `AngleMin` 및 exact map contract 검사 |
| 이미지 치수·좌표는 `px` | 통과 | `SourceImageWidth/Height`, bounds, affine 좌표 매핑 검사 |
| 픽셀 면적은 `px²` | 통과 | 기존 Affine review 표시와 `Area*` exact map 검사 |
| 비율·score·count·무차원 값의 의미 보존 | 통과 | ratio/score/count/unitless exact map 검사 |
| 미정의 metric은 조용히 `unitless`가 되지 않음 | 통과 | synthetic unknown projection이 값과 diagnostic을 유지하고 계약 validator가 `unknown`을 수용 |
| 숫자 값 불변 및 JSON 왕복 | 통과 | Good/Bad 각각 finite 38개 metric의 값을 Run Record와 Result에서 exact 비교 |
| 다른 좌표 layer 오해 방지 | 통과 | 실제 Contour overlay는 입력 layer을 기록하고, RotateScale는 output, Merge는 `mixed`를 기록하는 focused contract 검사 |
| 기존 consumer fixture/교차 애플리케이션 호환 | 통과 | TCP smoke 및 Machine Studio → 2D consumer cross-repository smoke |

## 검증

모든 test output과 evidence는 `D:\OpenVisionLab-TestData`에 저장했다.
소스 worktree가 의도적으로 dirty인 상태이므로 정상 경로 검증은 테스트
전용 Git shim으로 `status`만 비워 만든 합성 Clean runtime identity를
사용했다. 이는 공개 release cleanliness를 의미하지 않는다.

```text
dotnet build .\tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build .\tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build .\tools\TwoDIntegrationTcpSmoke\TwoDIntegrationTcpSmoke.csproj -c Release -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

실행한 focused 검증:

- [Debug build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/build-debug-final-2.log)
- [Release synthetic Clean build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/build-release-synthetic-final-3.log)
- [2D integration clean smoke](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/integration-clean-final-5.log)
- [Metric/coordinate exact-map evidence](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/integration-clean-final-5/two-d-integration-20260907-052330-d58bbe7341064d8b82c9ddd550ad805c/two-d-metric-unit-contract.json)
- [Good Run Record round-trip evidence](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/integration-clean-final-5/two-d-integration-20260907-052330-d58bbe7341064d8b82c9ddd550ad805c/two-d-good-metric-contract.json)
- [Bad Run Record round-trip evidence](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/integration-clean-final-5/two-d-integration-20260907-052330-d58bbe7341064d8b82c9ddd550ad805c/two-d-bad-metric-contract.json)
- [TCP consumer smoke](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/tcp-integration-clean-final-2.log)
- [Machine Studio → 2D cross-repository smoke](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/cross-repo-clean-final-1.log)
- [Dirty runtime fail-closed regression](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/integration-dirty-final-1.log)
- [Documentation index validation](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl10-20260907/documentation-index-final-2.log)
- `git diff --check --` 대상 파일: 통과 (LF→CRLF 변환 경고만 보고됨)

## 경계와 남은 위험

`unknown` unit은 계약의 비어 있지 않은 unit 요구를 만족시키는 보수적
표현이다. 새 metric을 제품에 추가할 때는 계산 의미를 검토한 뒤
`TwoDIntegrationMetricUnits`에 exact name을 등록해야 한다. 이번 slice는
새 metric 이름이나 숫자를 만들지 않았고, 픽셀을 calibration 없이 mm로
변환하지 않았다.

실제 3D 애플리케이션이 `OverlayCoordinateLayer`를 사용해 좌표를 변환하는
동작 자체는 이번 실행에서 변경하지 않았다. 교차 저장·재로드와 consumer
fixture 수용만 확인했으며, 각 3D 화면의 시각적 좌표 투영은 별도 범위다.
WPF UI를 변경하지 않았으므로 theme/DPI/UI 상태 검증도 이 slice 대상이
아니다. `C:\Git\2D\Original`은 수정하지 않았고 commit/push도 수행하지
않았다.

다음 프로젝트 우선순위는 OVL-04 이미지 snapshot/Lease와 Layer 제거 원자성이다.  
Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
