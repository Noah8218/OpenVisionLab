# OpenVisionLab Matching 스킬·TaktTime 후보 연구 세션 기록

작성일: 2026-08-30 KST  
범위: 이번 대화에서 수행한 PCB `Matching` 템플릿 크기·검출력·처리시간 연구와
`openvisionlab-rule-based-teaching` Skill 반영  
상태: **Complete (이번 대화의 문서화 범위)**

> 경로 주의: 이 보고서의 실행 명령은 당시 루트
> `C:\Git\OpenVisionLab_Dev`를 그대로 기록한다. 그 뒤 같은 날 저장소가
> 내용 변경 없이 `C:\Git\2D\Dev`로 이동했으므로 재실행에는 새 루트를
> 사용한다. 이동 동일성 근거는
> `docs/reports/OPENVISIONLAB_DEV_REPOSITORY_PATH_MIGRATION_20260830.md`에 있다.

> 이 문서는 생산 qualification, 최종 Recipe 승인, 배포 승인, 또는 OK/NG
> 분류 정확도 인증이 아니다. Matching 위치를 찾는 전역 후보 연구와 다음
> 대화의 재개 지점을 보존하는 세션 기록이다.

## 0. 이 대화에서 누적된 Skill 개발 맥락

이번 기록은 마지막 PCB/Takt 실험만을 뜻하지 않는다. 다음 대화에서 Skill
개발을 이어갈 수 있도록, 이 대화에서 반복해서 확인한 판단 기준과 교정
사항을 함께 고정한다. 세부 수치와 이전 이미지 검토 결과는 참조 보고서에
중복 보존하고, 이 문서는 결정의 연결 관계를 요약한다.

### 0.1 처음의 목표와 범위

- 제품의 중심은 OpenCvSharp 기반의 결정론적 rule-based vision workbench이며,
  LLM/Codex는 선택적인 XML 작성 보조이다.
- 목표는 사용자의 승인 클릭이나 이미지별 재티칭 없이, 고정된 corpus에서
  물리적으로 타당한 Matching/ROI/파라미터 전역 후보를 선택하도록 Skill을
  만드는 것이다.
- Score `90+`, 녹색 검출 박스, `ResultCount=1`은 참고 지표일 뿐이며, 실제
  자재의 동일 물리 구조를 감쌌는지와 후속 검사에 필요한 경계를 보존하는지가
  우선이다.

### 0.2 Die Pad 사례에서 교정된 등록 규칙

초기 Die Pad 검토에서 다음 오류가 드러났다. 특히 실제 후보를 만들 때
기울어진 원본을 그대로 템플릿에 등록한 적이 있어, Skill 문장과 실행 판단이
일치하지 않는 문제가 확인되었다. 이 오류 자체가 이번 Skill 보강의 중요한
교정 대상이다.

1. 기울어진 자재를 그대로 템플릿에 등록하면, 런타임의 axis-aligned 박스가
   실제 패턴과 어긋날 수 있다. 등록 전에 ROI/자재 footprint를 반대 각도로
   정렬해 수직·수평 기준으로 만든 뒤 템플릿을 저장해야 한다.
2. 템플릿은 배경을 넓게 포함하는 것보다, 외곽 경계·trace·인접 pad처럼
   반복되고 물리적으로 설명 가능한 특징을 우선해야 한다. 다만 너무 작아져
   모호해지면 검출력이 떨어지므로 “작을수록 무조건 좋다”는 규칙은 금지한다.
3. `019`와 `008_NG...MATCH_OK` 비교에서 사용자가 지적한 기준은 단순히
   박스가 나왔는지가 아니라, 필요한 외곽선과 두 Die Pad의 관계를 실제로
   감싸는지였다. 회전이 존재하면 검출 footprint도 회전된 형상으로 평가해야
   한다.
4. 따라서 기존의 “score가 높으니 개선”이라는 판단은 폐기한다. 성공 여부는
   `physical correspondence -> required boundary -> ambiguity/downstream`
   순서로 확인하고, 수치 score는 그 뒤에만 사용한다.

Die Pad 고정 A/B 후보는 122장 corpus에서 각각 실행되었고(A/B 합계 244개
correspondence panel), 모든 행의 overlay와 상태를 보존했다. 다만 이 실행의
`pipeline pass`나 높은 score를 최종 승인으로 해석하지 않고, operator가 정한
외곽·회전·허용오차가 없는 행은 `WAIT`로 남기는 것으로 정리했다.

관련 Die Pad의 A/B 템플릿, 회전 footprint, `SCORE_MIN=90` 재검증, 전수 시각
검토 결과는 다음 보고서에 남아 있다.

- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_DIE_PAD_20260827.md`
- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_TEMPLATE_AB_VALIDATION_20260827.md`
- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_FULL_VISUAL_CORRESPONDENCE_20260828.md`
- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_ROTATED_FOOTPRINT_20260828.md`
- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_SCORE90_ROTATION_REVALIDATION_20260828.md`

### 0.3 Skill에 반영하기로 한 운영 원칙

- 자동 선택 단위는 개별 이미지가 아니라 고정 corpus에 대한 전역 Recipe이다.
- 이미지별 ROI 이동, 템플릿 교체, 숨은 retry/fallback, score만 올리기 위한
  파라미터 완화는 하지 않는다.
- `Matching`과 `EdgeBasedMatching`은 별도 후보로 비교하며, Edge 기반으로
  바꿀 때도 동일한 물리 correspondence와 시각 증거 gate를 유지한다.
- Blob은 현재 Matching template/footprint 판단의 필수 전제에서 제외했다.
  Blob을 다시 포함하려면 별도의 명시적 operator intent와 검증 증거가 필요하다.
- 배치 결과는 CSV/점수만으로 완료하지 않고, 원본·템플릿·실행 overlay·recipe
  identity를 한 evidence 폴더에 보존하고 대표 성공/저점수/복구/NoResult를
  직접 확인한다.

### 0.4 PCB로 확장한 이유와 현재 결론

Die Pad에서 고정한 규칙이 다른 자재에도 일반화되는지 확인하기 위해 PCB
500장 corpus로 옮겼다. `bottom_center` 후보와 A0~A2 extent ladder를 같은
  좌표계·검색영역·회전/스케일 조건에서 비교했고, 그 결과는 아래 3절 이후에
  수치와 현재 overlay로 기록한다. 현재 결론은 다음과 같다.

> 이번 corpus에서는 작은 충분 코어가 NoResult를 줄였지만, 최종 템플릿은
> 검출률·물리 대응·모호성·Takt 예산을 함께 통과해야 하며, 어느 한 score나
> 템플릿 면적만으로 확정할 수 없다.

## 1. 사용자 목적과 이번 대화의 결정

사용자가 원하는 것은 LLM/Codex가 이미지마다 임의로 ROI·템플릿·파라미터를
바꾸는 것이 아니라, 룰베이스 Skill을 읽고 다음을 **사용자 승인 클릭 없이
전역 Recipe 후보 하나로 자동 선정**하는 것이다.

- 실제 이미지에서 같은 물리 구조를 감싸는 Matching 위치를 찾는다.
- Score, 녹색 박스, `ResultCount`만으로 양호하다고 판단하지 않는다.
- 회전·기울기·외곽 범위·배경 포함 여부를 분리해 판단한다.
- 템플릿은 작게 시작하되 검출에 필요한 물리 특징은 유지한다.
- TaktTime/사이클 시간도 고려하지만, 속도 때문에 물리 correspondence나
  검출력을 희생하지 않는다.
- 런타임에 이미지별 템플릿 전환, 이미지별 ROI 이동, 자동 재시도는 하지 않는다.

이번 연구에서 확정한 원칙은 다음과 같다.

> **가장 작은 템플릿이 아니라, 물리적으로 충분하고 선언된 TaktTime을
> 만족하는 최소 충분 전역 템플릿을 선택한다.**

## 2. 대화에서 수행한 Matching 검증 흐름

### 2.1 PCB corpus와 기준 이미지

- Dataset: `E:\라벨테스트\EasyMatch_PCB_Board_500(1)\EasyMatch_PCB_Board_500`
- 512×512 JPG 500장 (`OK 250 + NG 250`)
- 데이터셋은 synthetic validation corpus이며 생산 성능을 의미하지 않는다.
- 기준 이미지: `all_images\OK\pcb_board_001_ok.jpg`
- 전체 이미지 목록: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pcb-board-matching-20260830-r1\full-500-image-list.txt`
- Image-list SHA-256: `E78A5040258CC1FF16D23FE9DCB17327459BD5991D48A23E32838CAE1AA7F09C`

### 2.2 기존 PCB 후보 비교

동일 대표 12장에 대해 여러 물리 후보를 비교했고, 하단 중앙의 IC·Trace·인접
구조를 포함한 `bottom_center`가 기존 후보 중 가장 안정적이었다.

| 후보 | Matching 검출(대표 12장) |
|---|---:|
| `center_logic` | 6/12 |
| `right_logic` | 1/12 |
| `bottom_center` | 11/12 |
| `left_lower_datum` | 4/12 |
| `whole_board` | 1/12 |

기존 `bottom_center` 템플릿은 중앙 IC, 주변 수평·수직 Trace, 인접 부품,
`LCACFG.A3` 부품을 함께 포함한다. 라벨 문자는 단독 기준이 아니며 주변 물리
관계의 보조 특징이다.

### 2.3 Takt-aware extent ladder

기준 이미지·좌표계·Search ROI·Matching 파라미터는 고정하고 템플릿 외곽만
바꾼 전역 후보 4개를 만들었다.

| 후보 | ROI `(x,y,w,h)` | 면적 | 의미 |
|---|---|---:|---|
| `A0_small_core` | `(245,350,120,110)` | 13,200 px | 중앙 IC 중심 최소 코어 |
| `A1_medium_core` | `(215,335,140,140)` | 19,600 px | 코어 + 인접 구조 |
| `A2_large_core` | `(180,320,180,165)` | 29,700 px | 더 넓은 안정 코어 |
| `B_bottom_center` | `(150,310,280,190)` | 53,200 px | 기존 물리 context 포함 |

고정 파라미터:

- `Matching / CCoeffNormed`
- `SCORE_MIN=0.60`, `NUM_MATCH=1`
- angle `-10..+10°`, step `1°`
- scale `0.90..1.10`, step `0.05`
- Canny/threshold 미사용
- 전체 이미지 Search ROI
- 후보별 템플릿 외곽만 변경

## 3. 500장 전체 결과

`Found`는 Matching 단계의 수치·실행 게이트 통과를 뜻한다. OK/NG 폴더명은
검증 corpus의 라벨이며, Matching 위치가 물리적으로 옳다는 증거나 최종
검사 판정이 아니다.

| 후보 | Found | NoResult | 검출률 | 평균 ms | 중앙값 ms | p95 ms | 최대 ms | 단일 실행 wall s |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| `A0_small_core` | 464 | 36 | 92.80% | 217.096 | 204.392 | 306.508 | 440.458 | 139.39 |
| `A1_medium_core` | 436 | 64 | 87.20% | 195.788 | 190.557 | 299.575 | 449.212 | 127.48 |
| `A2_large_core` | 409 | 91 | 81.80% | 164.244 | 156.830 | 191.708 | 490.841 | 110.24 |
| `B_bottom_center` | 406 | 94 | 81.20% | 173.389 | 164.444 | 237.186 | 478.028 | 115.93 |

측정 시간은 현재 runner의 **Matching Step elapsed**이다. 이미지 로드, 프로세스
시작, 생산라인 전체 처리, 카메라 취득 시간은 포함하지 않는다.

### 관찰 결과

1. 작은 템플릿은 이번 corpus에서 `NoResult`를 줄이는 데 유리했다.
2. 그러나 가장 작은 `A0`가 가장 빠르지는 않았다.
3. 전형적인 처리시간은 `A2`가 가장 짧았고, 검출률은 `A0`가 가장 높았다.
4. 템플릿 면적과 처리시간은 단조롭게 비례하지 않았다. 회전·스케일 후보 수,
   Search ROI, 내부 구현과 후보 응답 수가 함께 영향을 줄 수 있다.
5. 따라서 픽셀 면적을 TaktTime의 대리값으로 사용하면 안 되고 실제 median/
   nearest-rank p95/max를 측정해야 한다.

### 대표 시각 증거

비교 시트는 모두 현재 실행에서 생성된 raw runtime overlay를 조합한 것이다.

![템플릿 extent ladder](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/pcb-board-matching-20260830-r1/takt-extent-study-20260830/visual/template-extent-ladder.png)

![OK001 후보별 검출](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/pcb-board-matching-20260830-r1/takt-extent-study-20260830/visual/overlay-extent-ladder-OK001.png)

![OK006 복구 비교](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/pcb-board-matching-20260830-r1/takt-extent-study-20260830/visual/overlay-extent-ladder-OK006.png)

![NG241 후보별 검출](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/pcb-board-matching-20260830-r1/takt-extent-study-20260830/visual/overlay-extent-ladder-NG241.png)

대표 행 관찰:

- `pcb_board_001_ok.jpg`: 네 후보 모두 하단 중앙의 동일한 물리 코어를 찾고,
  템플릿 크기만큼 박스 범위가 달라진다.
- `pcb_board_006_ok.jpg`: B는 `NoResult`였지만 A0/A1/A2는 같은 중앙 IC/Trace
  주변을 찾았다. 이는 검출 복구 증거이지 전체 corpus의 물리 qualification은
  아니다.
- `pcb_board_241_ng.jpg`: 네 후보 모두 하단 중앙 영역을 감쌌다. B의 낮은
  점수는 추가 context가 변형에 민감했을 가능성을 보여주지만, 원인은
  별도 pixel-level 연구 없이는 확정하지 않는다.

전체 raw evidence:

```text
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pcb-board-matching-20260830-r1\takt-extent-study-20260830
```

각 후보는 `evidence-<candidate>-full500\runs` 아래에 500개의 source,
runtime result, `01_matching_overlay.png`를 보존한다. 검증 결과 4개 후보 모두
CSV 500행과 overlay 500장이 존재하고 누락은 0건이다.

## 4. 자동 선택 결과와 보류 사유

현재 자동 선택 상태는 **`WAIT_TAKT_BUDGET`**이다.

생산 TaktTime의 다음 값이 아직 선언되지 않았다.

- 측정 범위: Matching Step / 전체 Pipeline / End-to-End 중 무엇인지
- 단위와 양의 목표 예산(ms 등)
- 허용 p95와 hard maximum
- warm-up 및 반복 측정 정책

따라서 현재 증거만으로는 다음처럼 역할만 구분한다.

- `A0_small_core`: 검출률·NoResult 관점의 후보
- `A2_large_core`: 전형적인 Matching Step 시간 관점의 후보
- `B_bottom_center`: 넓은 물리 context 후보지만 이번 corpus에서 A2보다
  검출률과 p95가 모두 불리함

Takt 예산을 모르는 상태에서 A0 또는 A2를 제품 기본값으로 확정하지 않는다.
예산이 선언되면 먼저 물리 correspondence/ambiguity/downstream gate를 통과한
후보만 남기고, 그 안에서 p95/max 예산을 적용한다. 조건을 만족하는 후보가
없으면 `AUTO_REJECTED_TAKT`로 끝내며 이미지별 fallback을 만들지 않는다.

## 5. Skill에 반영한 변경

다음 두 파일에 Takt-aware extent study 규칙을 반영했다.

- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\takt-time-template-study.md`

추가된 내용:

- TaktTime 범위·단위·양의 예산이 없으면 `WAIT_TAKT_BUDGET`
- 후보별 median/nearest-rank p95/max 측정
- 같은 source/angle/scale/Search ROI에서 template extent만 바꾸는 유한 후보
  사다리
- 물리 correspondence, required boundary, ambiguity, downstream 결과를
  timing보다 먼저 hard gate로 적용
- template pixel area만으로 속도를 추정하거나 선택하지 않음
- 선언된 p95/max 실패 시 `AUTO_REJECTED_TAKT`
- coverage와 timing이 충돌하면 Pareto 대안으로 보존하고 임의 선택하지 않음
- per-image template/ROI 변경과 암묵적 runtime retry 금지
- machine-readable packet의 `taktPolicy` 필드와 상태값 추가

Skill 검증:

```text
quick_validate.py -> Skill is valid!
test_template_registration_manifest.py -> PASS: template registration manifest regression cases
```

수정 후 Skill SHA-256:

- `SKILL.md`: `2D7CF92B29ADF052AAE9263C616F7F6B65C55FF3421AE949E188E2402937C78D`
- `references/takt-time-template-study.md`:
  `865FB8C4BAB60890CC9B8875E50A4A0139485F1F9305ED61C6889465AD1D3FFE`

## 6. 재현 및 검증 명령

현재 source build 산출물로 runner를 빌드한 뒤 네 후보를 각각 대표 12장과
전체 500장에 실행했다.

```powershell
dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release

dotnet C:\Git\OpenVisionLab_Dev\tools\VisionRecipeRunnerSmoke\bin\Release\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll `
  --batch-evidence <full-500-image-list.txt> <dataset-root> <recipe-xml> <csv> <evidence-root>

dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev
```

검증 결과:

- `VisionRecipeRunnerSmoke` Release build: 0 warnings/errors
- 4개 후보 full run: 각각 `BatchRows=500`, `BatchCompleted=500`,
  `BatchMissingImages=0`, exit code 0
- 각 후보 evidence: CSV 500행 + Matching overlay 500장
- OpenVisionLab readiness check: `passed`
- 원본 repository, branch push, tag, release, deployment, installation,
  EXE launch smoke: 수행하지 않음

## 7. 다음 대화에서 이어갈 입력과 순서

다음 대화는 아래 입력을 먼저 확정한 뒤 이어간다.

1. `TaktTime` 측정 범위를 지정한다: `MATCHING_STEP`, `PIPELINE`, 또는
   `END_TO_END`.
2. 목표 예산과 단위를 지정한다. 예: `Matching Step p95 <= ___ ms`.
3. 필요하면 hard maximum, warm-up 횟수, 반복 측정 횟수를 지정한다.
4. 동일 extent ladder를 held-out 또는 별도 고정 검증 세트에 재실행한다.
5. 대표 성공·저점수·복구·NoResult 행의 물리 correspondence를 먼저 확인한다.
6. 예산과 시각 gate를 모두 통과한 후보가 있을 때만 Skill의 자동 전역 선택
   상태를 `AUTO_SELECTED`로 기록한다.

다음 대화에서 바로 사용할 수 있는 재개 문장:

```text
PCB Matching Takt 정책을 이어간다.
측정 범위: <MATCHING_STEP|PIPELINE|END_TO_END>
목표 예산: <양의 값과 단위>
hard maximum: <값 또는 없음>
현재 후보: A0/A1/A2/B extent ladder를 동일 조건으로 유지한다.
먼저 물리 correspondence를 통과시키고, 그 다음 p95/max와 coverage를 비교한다.
```

## 8. 참조 보고서

이 세션 이전의 관련 결정은 아래 문서에 보존되어 있다.

- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_AUTO_SELECTION_20260828.md`
- `docs/reports/OPENVISIONLAB_MATCHING_DETECTION_POWER_STUDY_20260829.md`
- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_FULL_VISUAL_CORRESPONDENCE_20260828.md`
- `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_TEMPLATE_AB_VALIDATION_20260827.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pcb-board-matching-20260830-r1\pcb-board-matching-validation-summary.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pcb-board-matching-20260830-r1\takt-extent-study-20260830\takt-extent-study-report.md`

## 종료 기록

Status: **Complete**  
Scope: 이번 대화에서 수행한 Matching extent 후보 연구, 시각 증거 보존, Takt-aware Skill 반영, 재개 문서화  
Acceptance evidence: 4개 후보 × 500장 실행, 2,000개 overlay, current visual sheets, Skill validator/regression PASS, readiness PASS  
Boundary: 생산 TaktTime 예산 미지정, Matching Step 시간만 측정, 전체 corpus의 개별 물리 review와 held-out qualification 미완료  
