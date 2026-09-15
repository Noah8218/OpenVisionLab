# OpenVisionLab Rule-Based Teaching — Die Pad 전수 시각 대응 재검토

Date: 2026-08-28 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **WAIT_FOR_OPERATOR_REVIEW**
Qualification: **false**
Skill: `openvisionlab-rule-based-teaching`

## 1. 목적과 범위

사용자가 요청한 “점수나 초록 박스가 아니라 실제 이미지가 template과 매칭되는지”를
확인하기 위해, Die Pad 고정 A/B 후보를 전체 122장에 대해 다시 시각 검토했다.

- Dataset: `E:\라벨테스트\EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`
- Corpus: 122장(train 82 / validation 27 / test 13), role label OK 62 / NG 60
- A: boundary-tight template, `169x128`
- B: edge-plus-context template, `179x138`
- pipeline: `Matching(2) -> Matching(1) -> RotateScale -> Threshold -> Blob`
- search ROI: `0,0,512,512`
- downstream inspection ROI: `190,220,175,130`
- 파라미터·XML·image list·template는 A/B 사이에서 고정하고 template 경로만 다르게 유지했다.
- role label은 이 locator 시각 대응의 defect truth로 사용하지 않았다.

이번 작업은 teaching-time evidence review이다. source 이미지를 수정하지 않았고,
runtime에서 샘플별 template/ROI/threshold를 바꾸거나 recipe를 자동 보정하지 않았다.

## 2. 현재 실행과 증거 생성

같은 XML과 image list를 새 D-drive evidence root에서 재실행했다.

- A: 122 images completed, missing `0`, pipeline pass `77`
- B: 122 images completed, missing `0`, pipeline pass `73`
- 이전 2026-08-27 비교와 새 실행의 A/B `ScoreMax`, center, scale, angle을 전 행 비교한 결과 최대 절대 차이 `0`

각 후보에 대해 다음을 생성했다.

- A/B 각 122장, 총 **244 pose-normalized correspondence panels**
- A/B 각 행의 `source patch`와 `50% blend`, 총 **488 patch/blend files**
- A/B 각 8장, 총 **16 contact sheets**
- 정확한 runner `01_matching_overlay.png`와 `runtime_result.png` 링크 및 SHA-256

Evidence manifest:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\full-visual-review-manifest.json`

전수 ledger:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\full-visual-review.csv`

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\full-visual-review.json`

시각 패널:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\panels\A`

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\panels\B`

접촉시트:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\contact-sheets`

운영자 전체 위치 검토 보드:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\operator-review-boards\contact-sheets`

각 보드는 왼쪽에 전체 원본의 실제 `01` 후보 위치를, 오른쪽에
`template | pose-normalized source patch | 50% blend`를 배치한다. 행의 텍스트에
`visual=PASS/WAIT`, Runner 상태, score, 실제 후보 수를 함께 표시한다. 초록색
사각형은 위치 확인용이며 visual state를 대신하지 않는다.

## 3. 검토 방법

각 행을 다음 순서로 보았다.

1. template의 안정 코어인 두 pad, hole의 개수·순서, 간격, L자 trace, 우측 수직 trace를 먼저 확인
2. runner가 보고한 center/scale/angle로 source patch를 template frame에 정렬
3. template / source patch / 50% blend를 직접 비교
4. 해당 행의 실제 full-image `01_matching_overlay.png`와 `runtime_result.png`를 확인
5. 시각 대응이 확인된 뒤에만 score/result count/downstream 상태를 보조 정보로 기록

전체 16개 contact sheet(244행)를 훑었고, 모든 WAIT 행과 대표 PASS 행을 full-resolution
개별 패널로 다시 열었다. 따라서 CSV의 각 행은 `PASS`, `WAIT`, `FAIL`,
`NOT_REVIEWED` 중 하나를 가진다. 집계 숫자는 행별 판정을 대체하지 않는다.

## 4. 전수 판정

| Variant | PASS | WAIT | FAIL | NOT_REVIEWED | 해석 |
|---|---:|---:|---:|---:|---|
| A | 104 | 18 | 0 | 0 | 안정 코어가 보이는 행은 많지만 18행에 미승인 가림/추가 구조가 있음 |
| B | 103 | 19 | 0 | 0 | A와 같은 18행 + `die_pad_163_ng.jpg`의 B 문맥 불일치 |

`PASS`는 “보고된 후보 위치에서 안정 코어와 필요한 문맥이 시각적으로 대응한다”는
현재 review state다. operator가 승인한 tolerance 또는 production qualification을
뜻하지 않는다.

현재 검토에서는 명백히 다른 반복 구조를 선택한 `FAIL` 행은 관찰하지 않았다.
이는 locator가 모든 변형과 현장 조건에 강하다는 뜻이 아니며, 물리적/held-out
qualification도 증명하지 않는다.

## 5. WAIT 행과 이유

아래 행은 두 pad 또는 trace가 일부 보이더라도, template 영역 안에 가림·추가 구조가
있고 승인된 tolerance가 없으므로 높은 score와 관계없이 WAIT로 남겼다.

| Image | A | B |
|---|---|---|
| `die_pad_004_ng.jpg` | 우측 설명되지 않은 추가 구조 | 동일 |
| `die_pad_026_ng.jpg` | 좌측 pad 가림, tolerance 없음 | 동일 |
| `die_pad_028_ng.jpg` | 상단 추가 구조 | 동일 |
| `die_pad_062_ng.jpg` | 우하단 추가 구조 | 동일 |
| `die_pad_072_ng.jpg` | 상단 trace 가림, tolerance 없음 | 동일 |
| `die_pad_089_ng.jpg` | 상단 추가 구조 | 동일 |
| `die_pad_098_ng.jpg` | 우측 pad 가림, tolerance 없음 | 동일 |
| `die_pad_109_ng.jpg` | 좌상단 문맥 가림, tolerance 없음 | 동일 |
| `die_pad_124_ng.jpg` | 우상단 추가 구조 | 동일 |
| `die_pad_161_ng.jpg` | 상단 추가 구조 | 동일 |
| `die_pad_162_ng.jpg` | 좌측 pad 내부 추가 dark mark | 동일 |
| `die_pad_163_ng.jpg` | 안정 코어는 보이므로 PASS | B의 좌측 추가 문맥이 B template와 불일치 |
| `die_pad_166_ng.jpg` | 하단 중앙 추가 구조 | 동일 |
| `die_pad_171_ng.jpg` | 우측 pad 하단 추가 dark structure | 동일 |
| `die_pad_189_ng.jpg` | 좌측 추가 구조 | 동일 |
| `die_pad_198_ng.jpg` | 좌측 추가 구조 | 동일 |
| `die_pad_207_ng.jpg` | 상단 및 우하단 추가 구조 | 동일 |
| `die_pad_209_ng.jpg` | 우하단 추가 구조 | 동일 |
| `die_pad_219_ng.jpg` | 좌측 추가 구조 | 동일 |

## 6. 결론과 개발 방향

1. **초록색 `01` 박스는 최종 검출/Good 판정이 아니다.** 현재 runner에서 green `01`은
   Matching Step 1의 ROI/candidate geometry이고, blue/yellow/magenta는 후속 단계다.
2. **score-only gate는 폐기한다.** 실제 template-to-source correspondence가 먼저이며,
   불일치하면 score가 높아도 `WAIT`/`REJECTED`다.
3. **A를 보수적 기본안으로 유지한다.** B의 경계 문맥은 일반 행에서 보이지만,
   B가 실제 ambiguity를 줄였다는 두 번째 후보 증거는 없고, 고정 downstream에서
   pipeline pass도 `77 -> 73`으로 감소했다. 따라서 B는 operator 승인 전까지 채택하지 않는다.
4. `die_pad_026_ng`, `die_pad_098_ng`처럼 결함이 locator core를 가리는 경우에는
   “검출 성공”으로 포장하지 않는다. operator가 허용할 물리적 occlusion tolerance와
   지원되는 도구 계약을 먼저 정해야 한다.
5. 이미지별로 A/B를 자동 전환하거나, score를 올리려고 ROI/template/gate를 샘플별로
   움직이는 방식은 허용하지 않는다.

## 7. Skill 반영

다음 규칙을 `openvisionlab-rule-based-teaching` 및
`references/visual-correspondence-review.md`에 추가했다.

- full-corpus 요청 시 candidate × variant마다 한 행의 current evidence ledger를 만든다.
- source/template/pose-normalized patch/blend/full overlay/runtime overlay와 hash를 함께 보존한다.
- 모든 행을 개별 state로 남기며, aggregate score로 대체하지 않는다.
- `WAIT`, `FAIL`, `NOT_REVIEWED`가 한 행이라도 있으면 variant를 qualified로 부르지 않는다.
- operator가 승인하지 않은 occlusion/extra structure는 score와 무관하게 WAIT다.

Skill validator 결과: `Skill is valid!`

## 8. 다음 명시적 조치

operator가 다음 중 하나를 명시적으로 결정해야 한다.

1. WAIT 행의 가림/추가 구조를 결함으로 허용할지, invalid locator로 거부할지 결정
2. 허용한다면 한 recipe-level tolerance와 도구 계약을 정의하고 재검증
3. A 또는 B를 recipe teaching variant로 승인

그 전까지 최종 상태는 `WAIT_FOR_OPERATOR_REVIEW`이며, 이 보고서는 제품 배포,
실제 생산 검증, defect truth, metrology, field qualification을 주장하지 않는다.

## 9. 완료 기록

```text
Status: Complete
Scope: Die Pad 122장 × A/B 후보의 전수 시각 대응 review packet과 행별 판정
Acceptance criteria:
  - 동일 고정 XML/image list로 A/B 재실행: PASS (122/122 completed; missing 0)
  - 재실행 geometry/score가 기존 frozen comparison과 일치: PASS (max delta 0)
  - A/B 모든 후보의 template/source patch/blend/panel/overlay 링크 보존: PASS (244 rows)
  - 모든 corpus row에 visual state 기록: PASS (A 122 + B 122; NOT_REVIEWED 0)
  - score/green overlay만으로 PASS하지 않음: PASS (WAIT rows retained)
  - operator 승인/recipe qualification: PENDING (required)
Verification:
  - VisionRecipeRunnerSmoke --batch-evidence A: 122 completed, pipeline pass 77
  - VisionRecipeRunnerSmoke --batch-evidence B: 122 completed, pipeline pass 73
  - GenerateDiePadVisualReviewEvidence.ps1: 244 panels, 488 patch/blend files, 16 sheets
  - GenerateDiePadOperatorReviewBoards.ps1: 16 operator sheets covering 244 rows
  - WriteDiePadFullVisualReview.ps1: 244 ledger rows, hashes and exact overlay paths
  - quick_validate.py: Skill is valid!
Evidence:
  - D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual
  - full-visual-review-manifest.json, visual-correspondence-full\full-visual-review.csv/.json/.md
Boundary / next dependency:
  - operator approval and tolerance decision are required before qualification
  - production/held-out/field qualification and release/deployment are not proven
```
