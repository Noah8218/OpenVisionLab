# OpenVisionLab Rule-Based Teaching — SCORE_MIN 90% 및 회전 간격 재검증

Date: 2026-08-28 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — bounded validation scope**
Qualification: **false**
Operator approval: **REQUIRED**
Skill: `openvisionlab-rule-based-teaching`

## 1. 결론

이번 검증은 “모든 결과의 점수를 90 이상으로 올리는 설정”이 아니라,
`SCORE_MIN=0.9`를 적용하여 **표시 점수 90 미만 후보를 거절하는 게이트**를
시험한 것이다. 게이트가 낮은 점수를 높여 주지는 않는다.

- A(boundary-tight)는 122장 중 76장만 90% 이상 후보가 남았고 46장은
  `MatchingNoResult`가 됐다.
- B(edge-plus-context)는 122장 중 83장만 90% 이상 후보가 남았고 39장은
  `MatchingNoResult`가 됐다.
- 사용자가 지적한 `die_pad_012_ng.jpg`는 A와 B 모두 90% 게이트에서 후보가
  없어졌다. 초록 박스가 사라진 것은 게이트가 낮은 품질 후보를 숨기지 않고
  fail-closed한 결과이지, 시각적 매칭 문제가 해결됐다는 뜻이 아니다.
- `FIND_ANGLE=1°`를 `0.5°`로 좁힌 별도 후보에서는 A/B 각각 4장의 추가 후보가
  생겼고 일부 점수·포즈가 바뀌었다. 그러나 `die_pad_012_ng.jpg`는 여전히
  후보가 없었다. 따라서 회전 간격 조정만으로 production 기본값을 바꾸지 않는다.
- 현재 판단은 **A를 보수적 기본 후보로 유지**, B와 90%/0.5° 조합은 operator
  검토 대기다. 점수나 초록 박스만으로 qualification하지 않는다.

## 2. 고정 입력과 범위

- Dataset: `E:\라벨테스트\EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`
- Corpus: 122장 (`OK` 62 / `NG` 60; train 82 / validation 27 / test 13)
- Pipeline: `Matching(2) -> Matching(1) -> RotateScale -> Threshold -> Blob`
- Search ROI: `0,0,512,512`
- Downstream Blob ROI: `190,220,175,130`
- A template: stable boundary-tight core, `169x128`
- B template: same core plus edge/context, `179x138`
- `FIND_ANGLE_MIN=-5`, `FIND_ANGLE_MAX=5`, `USE_FIND_ANGLE=true`
- `SCORE_MIN=0.9` (runner에 표시되는 90점 기준)
- 모든 실행은 원본 저장소·제품 레시피를 변경하지 않고 D 드라이브의 별도
  evidence root에서 수행했다.

이번 문서의 `PipelineSuccess`는 모든 Pipeline Step이 실행 성공했는지를
뜻한다. `OK/NG` 분류 정확도나 defect truth로 해석하지 않는다.

## 3. 후보별 전체 122장 실행 결과

| 후보 | Template / angle step | Matching 후보 >=90 | NoResult | PipelineSuccess | 비영(非零) 각도 | 수락 후보 점수 범위 | 평균 |
|---|---|---:|---:|---:|---:|---:|---:|
| A score90 | A / 1° | 76 | 46 | 49 | 22 | 90.186–99.078 | 94.244 |
| B score90 | B / 1° | 83 | 39 | 51 | 20 | 90.145–98.856 | 94.282 |
| A angle05 | A / 0.5° | 80 | 42 | 49 | 49 | 90.301–99.078 | 94.399 |
| B angle05 | B / 0.5° | 87 | 35 | 54 | 48 | 90.256–98.856 | 94.373 |

`Matching 후보 >=90`은 Step 1의 `StepStatus=OK` 행 수다. 모든 수락 행은
`ScoreMax >= 90`인지 별도로 확인했고, 각 122행에 `01_matching_overlay.png`와
`runtime_result.png`가 존재하는지 확인했다.

## 4. 90% 게이트가 실제로 한 일

score90 후보와 기존 0.8 게이트 실행의 공통 수락 행을 비교한 결과는 다음과 같다.

| Variant | 공통 수락 행 | ScoreMax 최대 차이 | Center 최대 차이 | Angle 최대 차이 | Scale 최대 차이 |
|---|---:|---:|---:|---:|---:|
| A | 76 | 0 | 0 | 0 | 0 |
| B | 83 | 0 | 0 | 0 | 0 |

즉 90% 게이트는 기존에 90 미만이던 후보를 새 점수로 끌어올린 것이 아니라,
동일한 matcher 결과 중 90 미만 행을 `MatchingNoResult`로 거절했다.

대표 행:

- A `die_pad_004_ng.jpg`: `91.2472248`, pose `(296,257)`, angle `0°`, scale
  `1.15`; 매칭 Step은 통과하지만 downstream Blob은 결과가 없어 Pipeline은
  실패한다.
- A `die_pad_012_ng.jpg`: 기존 `88.7357950`에서 `SCORE_MIN=0.9` 적용 후
  `MatchingNoResult`.
- B `die_pad_004_ng.jpg`: 기존 `89.3266439`에서 `MatchingNoResult`.
- B `die_pad_012_ng.jpg`: 기존 `89.8352623`에서 `MatchingNoResult`.

## 5. 회전 간격 0.5° 후보

90% 게이트를 유지하고 `FIND_ANGLE`만 `1° -> 0.5°`로 변경한 별도 후보를
전체 122장에 실행했다.

- A: 수락 후보 `76 -> 80`, NoResult `46 -> 42`
- B: 수락 후보 `83 -> 87`, NoResult `39 -> 35`
- 공통 수락 행 중 포즈가 바뀐 행: A `45`, B `44`
- 각 후보에서 새로 90%를 넘은 행: A `4`, B `4`
- A PipelineSuccess: `49 -> 49` (변화 없음)
- B PipelineSuccess: `51 -> 54` (3행 증가)
- A angle 분포: `-0.5° 24`, `0° 31`, `0.5° 24`, `1.5° 1`
- B angle 분포: `-0.5° 22`, `0° 39`, `0.5° 26`

이 결과는 1° 격자에서 놓치거나 낮게 평가되던 일부 sub-degree 후보가 있다는
근거는 제공한다. 하지만 angle05에서도 `die_pad_012_ng.jpg`는 A/B 모두
`MatchingNoResult`이고, 회전 결과가 실제 물리적 정렬을 보장한다는 뜻은 아니다.
특히 현재 runner의 green box는 후보 위치를 표시하는 축 정렬 geometry이며,
회전된 템플릿의 모든 경계가 올바르게 대응한다는 시각 판정 자체가 아니다.

## 6. 현재 실행의 시각 검토

점수만 보고 통과시키지 않기 위해 angle05 실행의 exact runner overlay와
pose-normalized spot correspondence panel을 별도로 보존했다.

### 6.1 관찰된 대응

- `A / die_pad_020_ok.jpg`, score `94.987`, angle `0.5°`, pose `(251,259)`:
  두 pad, hole 순서, L자 trace, 우측 수직 trace가 template/source patch/blend에서
  대응한다. 이 spot은 visual correspondence PASS 후보이지만 전체 qualification은
  아니다.
- `A / die_pad_232_ng.jpg`, score `98.230`, angle `0.5°`, pose `(281,223)`:
  template/source patch/blend의 stable core는 대응한다. 전체 원본에는 matching
  box 밖 하단 우측에 밝은 speck이 보이므로, 높은 score만으로 Good/NG 의미를
  대신하지 않고 full-image context와 downstream 결과를 별도로 본다.
- `A / die_pad_127_ok.jpg`, score `98.450`, angle `0.5°`, pose `(314,229)`:
  angle05 pose에서 core/trace가 대응하지만, 한 행의 spot 확인일 뿐이다.
- `A / die_pad_012_ng.jpg`와 `B / die_pad_012_ng.jpg`:
  current `01_matching_overlay.png`에 후보 box가 없고, runner message는
  `ScoreMin=0.9 ... Matching found no result`다. 이 행을 성공으로 포장하지
  않는다.
- `B / die_pad_004_ng.jpg`:
  score90와 angle05 모두 `MatchingNoResult`다. A의 91.247 후보가 보인다고 해서
  B가 같은 위치를 검증했다고 간주하지 않는다.

### 6.2 현행 evidence 위치

score90 / 1° 실행:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-20260828-rerun01`

angle05 / 0.5° 실행:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-rerun01`

angle05 집계:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-rerun01\score90-angle05-summary.json`

검증 로그:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-rerun01\validation-verification.txt`

현재-run 대표 대응 패널:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-rerun01\visual-spots`

대표 패널의 source/template/overlay/panel SHA-256과 pose/score를 묶은 manifest:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-rerun01\visual-spots\manifest.json`

대표 exact runner overlay:

- `A\evidence\runs\OK_die_pad_020_ok\01_matching_overlay.png`
- `A\evidence\runs\NG_die_pad_232_ng\01_matching_overlay.png`
- `A\evidence\runs\NG_die_pad_012_ng\01_matching_overlay.png`
- `B\evidence\runs\NG_die_pad_004_ng\01_matching_overlay.png`

각 variant의 `evidence\evidence_rows.csv`는 122개 입력 행의 Step 상태,
점수, pose, angle, scale, 오류명과 current-run 파일 경로를 보존한다.

## 7. 개발 방향 결정

1. **90%는 quality-boost 설정이 아니라 rejection floor로만 사용한다.**
   `SCORE_MIN=0.9`를 넣어도 90 미만 샘플의 실제 영상 대응은 개선되지 않는다.
2. **시각 대응 게이트를 점수보다 앞에 둔다.** stable core, hole 순서, trace,
   필요한 경계가 source patch와 template에서 대응하지 않으면 높은 score도
   `WAIT/REJECTED`다.
3. **A를 기본 teaching 후보로 유지한다.** B의 추가 문맥은 일부 행의 후보 수를
   늘릴 수 있지만, 현재 전수 visual review에서 operator 승인이나 ambiguity 감소가
   입증되지 않았다.
4. **angle05는 실험 후보로 보존한다.** sub-degree 변화가 4행씩 추가 수락을
   만들었지만, `die_pad_012_ng`를 해결하지 못했으므로 production 기본값으로
   승격하지 않는다.
5. **die_pad_012는 다음 검토의 대표 실패 행이다.** source/template 대응 패널을
   기준으로 구조 차이·가림·경계 선택 문제를 확인한 뒤, 필요할 때만 matcher
   종류/템플릿 범위/허용 각도 계약을 operator-level로 재정의한다. 샘플별 ROI나
   score를 튜닝하지 않는다.
6. 이번 실행에서는 제품 소스, 원본 레시피, 배포물, Skill 문서를 변경하지 않았다.
   현재 Skill의 visual-first/`NO EVIDENCE, NO COORDINATE`/score-only 금지 규칙과
   이번 결과는 일치한다. Skill에 90% floor 문구를 추가하는 것은 operator가 A/B와
   angle05 후보를 승인한 후 별도 변경으로 다룬다.

## 8. 재현 명령

score90 / 1° A:

```text
dotnet run --no-build --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -- --batch-evidence "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-20260828-rerun01\image-list.txt" "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-e-die-pad-20260827" "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-20260828-rerun01\A\pipeline.xml" "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-20260828-rerun01\A\batch.csv" "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-20260828-rerun01\A\evidence"
```

score90 / 1° B는 위 명령의 `A` 경로를 `B`로 바꿔 실행했다. angle05도 같은
명령 구조에서 `die-pad-template-ab-score90-angle05-20260828-rerun01`을 사용했다.

## 9. 완료 기록

```text
Status: Complete
Scope: 122장 × A/B에 SCORE_MIN=0.9를 적용한 전체 재실행, angle05 분리 후보 재실행,
       score/pose/angle/result 상태 집계, 대표 current-run 시각 evidence 검토
Acceptance criteria:
  - A/B score90 pipeline runner: PASS (각 122/122 completed, missing 0)
  - 모든 수락 행 ScoreMax >= 90: PASS (A 76/76, B 83/83)
  - angle05 runner: PASS (각 122/122 completed, missing 0)
  - angle05 모든 수락 행 ScoreMax >= 90: PASS (A 80/80, B 87/87)
  - score90 공통 행이 기존 geometry/score를 보존: PASS (max delta 0)
  - current-run overlay 파일 보존: PASS (각 후보/variant 122행)
  - die_pad_012의 미매칭을 숨기지 않음: PASS (A/B MatchingNoResult 유지)
  - visual correspondence 및 production qualification: PENDING (operator approval required)
Verification:
  - VisionRecipeRunnerSmoke --batch-evidence: score90 A/B 및 angle05 A/B
  - score90-angle05-summary.json 생성 및 validation-verification.txt 확인
  - current angle05 exact overlay와 pose-normalized representative panel 재열람
Evidence:
  - D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-20260828-rerun01
  - D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-rerun01
Boundary / next dependency:
  - operator가 A/B, 90% floor, angle05 및 WAIT/NoResult 행을 승인하기 전에는
    recipe qualification/production release로 승격하지 않음
  - 이 검증은 defect truth, calibrated metrology, field robustness, release,
    deployment를 증명하지 않음
```
