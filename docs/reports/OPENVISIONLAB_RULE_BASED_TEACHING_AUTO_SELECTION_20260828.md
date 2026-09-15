# OpenVisionLab Rule-Based Teaching 자동 전역 후보 선정 및 재검증

작성일: 2026-08-28
범위: `openvisionlab-rule-based-teaching` Skill의 자동 선정 모드와 Die Pad
122장 고정 데이터셋 재실행
상태: **Complete (bounded validation)**

> `AUTO_SELECTED`는 사용자의 승인 클릭 없이 하나의 전역 후보를 고른다는
> 뜻이다. `QUALIFIED` 또는 배포 가능하다는 뜻은 아니다. 현재 결과는
> `qualification=false`로 남긴다.

## 사용자 기준 보정

`008_NG_die_pad_188_ng__MATCH_OK` (`die_pad_188_ng.jpg`)는 사용자가 직접
“잘 잡힌” 양성 기준 샘플로 지정했다. 이미지의 빨간 하단/우측 선은 다른
샘플이 이 기준과 같은 패드·트레이스·외곽선의 상대 배치와 박스 범위를
재현해야 한다는 **정렬 시각 단서**다. 이는 기준 샘플의 A 박스 바깥에 선이
조금 보인다는 이유만으로 A를 탈락시키거나 B의 추가 배경을 강제하라는
뜻이 아니다. 명시적으로 “경계가 박스 안에 포함되어야 한다”고 요구한
경우에만 `requiredPhysicalFeature` 하드 게이트를 적용한다.

따라서 이번 재검토에서 008 행은 A에 대해
`OPERATOR_ACCEPTED_REFERENCE_EXEMPLAR`로 기록하고, 나머지 행은 이 기준의
물리 correspondence를 먼저 확인한다. 수치 score, `MATCH_OK`, 녹색 박스,
`ResultCount`만으로 다른 행을 OK로 승격하지 않는다.

## 1. 사용자 목적과 이번 변경

기존 Skill이 모든 후보를 `operatorApproval=REQUIRED`로 끝내 사용자가 직접
템플릿/파라미터를 승인해야 하는 것처럼 작성되어 있었다. 이는 이번 요청의
목적과 맞지 않았다. 이번 변경은 다음을 분리했다.

- `AUTO_SELECTION`: 사용자가 “승인 없이 자동 선정”을 요청하면 후보를 고정된
  corpus 전체에 실행하고, 최대 하나의 전역 Recipe 후보를 자동 선정한다.
- `REVIEW_ONLY`: 사용자가 검토용 teaching plan만 요청할 때 사용하는 기존
  수동 검토 모드다.
- 두 모드 모두 이미지마다 템플릿/ROI/threshold/angle을 바꾸거나 LLM을 런타임
  검출기로 사용하는 것은 금지한다.
- 자동 선정 실패는 사용자 승인 대기 대신 `AUTO_REJECTED`,
  `AUTO_REJECTED_AMBIGUOUS`, `AUTO_REJECTED_VISUAL_MISMATCH` 중 구체적인
  사유로 끝낸다.

## 2. 자동 선정 규칙

`AUTO_SELECTION`은 아래 순서로 동작한다.

1. 이미지 목록, 라벨, held-out 분할, 후보 파일, XML, 좌표계, score floor,
   실행 결과를 해시와 함께 고정한다.
2. 템플릿 A/B, 전역 coarse orientation `{0,90,180,270}`, 전역 angle/scale
   범위, ROI, 매칭 파라미터만 유한 후보로 열거한다.
3. 모든 후보를 동일한 122장과 동일한 downstream pipeline에 실행한다.
4. 증거 완전성, 해시, 한 좌표계, per-image override 부재, 선언된 score floor,
   물리적 visual correspondence, ambiguity를 hard gate로 검사한다.
5. 살아남은 후보를 다음 lexicographic 순서로 비교한다.
   `physical correspondence -> wrong-structure/ambiguity -> coverage/NoResult
   -> pose/scale stability -> downstream pass -> score/margin -> timing`.
6. 높은 점수만으로 후보를 승격하지 않는다. 후보가 없으면 자동 거부한다.
7. 선정과 qualification, Preview/Run, XML import, layer 변경, release/deploy를
   별도 경계로 유지한다.

상세 규칙은 다음 Skill 문서에 고정했다.

- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\auto-selection-policy.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\template-selection-policy.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\agents\openai.yaml`

## 3. 자동 후보 평가 결과

입력 corpus는 기존 angle05/score90 실행의 122장 목록이며 SHA-256은
`CB9B33F82B0FA5E31E7620A47444023472448F5DD797F59976EF69E0344EC1BD`이다.
두 후보 모두 `SCORE_MIN=0.90`을 사용했고, angle 검색은 `-5..5°`, `0.5°`
step, scale은 `0.75..1.35`, `0.1` step이다.

| 후보 | 매칭 수 | NoResult | coverage | downstream Pipeline pass | 수락 score min / avg | 기존 full visual PASS / WAIT | 자동 결과 |
|---|---:|---:|---:|---:|---:|---:|---|
| A: minimal stable core | 80/122 | 42 | 65.5738% | 49 | 90.301228 / 94.399145 | 104 / 18 | **AUTO_SELECTED** |
| B: edge-plus-context | 87/122 | 35 | 71.3115% | 54 | 90.256393 / 94.373293 | 103 / 19 | `AUTO_REJECTED` (B gate) |

B는 수치상 coverage와 pipeline pass가 더 높았다. 그러나 자동 정책은 먼저
물리 correspondence와 ambiguity를 본다. 같은 source/template identity의 full
visual ledger에서 B의 WAIT가 A보다 1건 많고(19 대 18), 현재 angle05 대표
correspondence도 B는 1건만 있어 요구된 ordinary/difficult 비교를 충족하지
못했다. 따라서 “점수가 높다/더 많이 찾는다”는 이유로 B를 고르지 않고,
stable-core A를 전역 후보로 자동 선정했다. 이는 이미지별로 B로 fallback하지
않는 결과다.

자동 선정 manifest:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selection-20260828-rerun01\auto-selection-manifest.json`

사용자 기준 exemplar 해석을 반영한 최신 재검증 manifest:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selection-exemplar-20260828-rerun02\auto-selection-manifest.json`

요약:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selection-20260828-rerun01\auto-selection-summary.md`

manifest의 핵심 상태는 다음과 같다.

```text
mode=AUTO_SELECTION
status=AUTO_SELECTED
selected=die-pad-a-score90-angle05-global
operatorApproval=NOT_REQUIRED_FOR_AUTO_SELECTION
qualification=false
```

## 4. 선택 후보의 전체 재실행

자동 선택된 A의 XML/template/image-list를 새 출력 폴더에 복제하고, 현재
빌드로 122장을 다시 실행했다. 선택 전 angle05 A 결과와의 비교는 다음과
같다.

| 검증 항목 | 결과 |
|---|---|
| source rows / rerun rows | 122 / 122 |
| accepted matches | 80 / 80 |
| NoResult set | 42 / 42, 동일 |
| downstream Pipeline pass | 49 / 49 |
| 행별 Step1 상태 차이 | 0건 |
| accepted ScoreMax 최대 절대 차이 | 0 |
| template SHA-256 | 동일 `80C643849C42E35FD0A7CD85A8FDC45E5B4DD15EAFFD41A6FE24FDCC067AF404` |
| 현재 rerun Matching overlay / runtime overlay | 122 / 122 |

실행 증거:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selected-A-score90-angle05-20260828-rerun01`

재현 비교 기록:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selected-A-score90-angle05-20260828-rerun01\rerun-verification.txt`

008 exemplar 해석을 반영해 현재 Debug 빌드로 다시 실행한 A 증거:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-exemplar-A-run-20260828-rerun02\A\evidence`

이 실행은 122개 이미지와 442개 Step 행을 완료했고, 기존 A 결과와의 행별
상태/score 비교 차이는 0건이며 `01_matching_overlay.png` 122개와
`runtime_result.png` 122개를 생성했다. 008의 Step 1은 `OK`,
`ScoreMax=93.242287635803`으로 재현됐지만, 이 수치는 exemplar 시각 게이트를
대체하지 않는다.

현재 선택된 A의 대표 runtime overlay도 새 실행 폴더에 보존되어 있다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selected-A-score90-angle05-20260828-rerun01\A\evidence\runs\OK_die_pad_020_ok\01_matching_overlay.png`

`die_pad_012_ng`는 이전과 같이 A/B 모두 `MatchingNoResult`로 남았다. 자동
정책은 이 한 행을 통과시키려고 threshold나 template을 바꾸지 않았다.

## 5. Skill 효과와 한계

### 5-0. 008 기준 exemplar 재검증

사용자 보정 후 `WriteDiePadFullVisualReview.ps1`를 다시 실행해 244행을
갱신했다. A는 `104 PASS / 18 WAIT`, B는 `103 PASS / 19 WAIT`이며,
`die_pad_188_ng.jpg`는 A에서
`OPERATOR_ACCEPTED_REFERENCE_EXEMPLAR`, B에서
`REFERENCE_EXEMPLAR_CORE_VISIBLE_ALTERNATIVE`로 기록됐다. 즉 A의 박스를
빨간 표시선 바깥까지 임의로 넓히지 않고, 008의 두 패드·홀 순서·L형/우측
수직 trace와 상대적인 하단/우측 배치를 다른 행의 시각 게이트 기준으로
삼았다.

최신 자동 선정 결과는 `AUTO_SELECTED=A`이며, `referenceExemplar`가
`die_pad_188_ng.jpg`, `extentRule=MATCH_EXEMPLAR`로 고정됐다. B는
coverage 71.3115%와 pipeline pass 54로 수치상 높지만, 시각 WAIT가
19건으로 A의 18건보다 많아 전역 후보로 승격되지 않았다.

### 확인된 효과

- 사용자의 승인 클릭 없이 A/B 후보를 비교하고 하나의 전역 후보를 반환했다.
- B의 높은 coverage/score가 물리 correspondence gate를 우회하지 못했다.
- 자동 선택 후 새 폴더에서 재실행해 122행 상태, 80 accepted, 42 NoResult,
  49 pipeline pass, score 최대 차이 0을 재현했다.
- `AUTO_SELECTION`에서도 orientation/ROI/template/parameter는 전역 값으로만
  유지되고, 이미지별 후보 switching이 발생하지 않았다.

### 아직 증명하지 않은 것

- 현재 angle05 실행에 대해 122행 모두를 새 pose-normalized panel로 다시
  시각 분류한 것은 아니다. 현재 자동 manifest는 angle05의 대표 spot evidence와
  source/template identity가 동일한 기존 full visual ledger를 함께 사용하고,
  그 범위를 명시했다.
- 따라서 자동 선정은 성공했지만 `qualification=false`다. held-out corpus,
  current full-corpus visual reclassification, 실제 defect truth/calibration,
  field robustness가 추가되기 전에는 생산용 `QUALIFIED`로 승격하지 않는다.
- 수치 score는 여전히 후보 선택의 마지막 보조 지표일 뿐이며, 잘못된 반복
  구조를 완전히 판별하는 독립적인 시각 의미 모델을 뜻하지 않는다.
- 후보 XML의 `TemplatePath`/`PATTERN_PATH`가 이전 validation 폴더를 가리키지만,
  selector가 해당 파일 존재와 선택 template SHA-256 일치를 확인했다. 현재
  재실행에서는 문제가 없었고, 다른 머신으로 이 패키지를 이동하는 self-contained
  경로 재작성은 이번 범위에서 수행하지 않았다.

이 한계는 “사용자에게 승인시키기”로 우회하지 않고, 다음 자동 상태로 남기도록
설계했다. 증거가 부족하면 `AUTO_REJECTED_*` 또는 `qualification=false`를
기록하고, per-image 보정은 하지 않는다.

## 6. 실행한 검증

```text
python C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching
-> Skill is valid!

dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"
-> 경고 0개 / 오류 0개

powershell -NoProfile -ExecutionPolicy Bypass -File tools\SelectDiePadAutoTemplate.ps1 ...
-> Status=AUTO_SELECTED Selected=A

dotnet run --no-build --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -- --batch-evidence ...
-> BatchRows=122, BatchCompleted=122, BatchPipelinePasses=49, BatchMissingImages=0
```

이번 작업은 Dev 저장소의 Skill/문서/검증 도구와 D: 테스트 증거만 변경했다.
원본 저장소 수정, `main` push, tag/release 공개, 설치·롤아웃·배포, EXE launch
smoke는 수행하지 않았다.

## 7. 122장×A/B 개별 이미지 내보내기

사용자가 각 검출 결과를 한 장씩 확인할 수 있도록 현재 angle05/score90 실행의
`Matching Step 1` overlay를 A/B별로 분리했다. 원본 overlay를 재렌더링하거나
색상을 바꾸지 않고 그대로 복사했으며, 각 복사본의 SHA-256이 원본과 같은지
검사했다.

개별 이미지 폴더:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-individual-detections`

사용자 기준 exemplar 재검증 시점의 동일한 개별 이미지 폴더(244행, 해시
검사 포함):

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-exemplar-20260828-rerun02-individual-detections`

- `A\`: 122개 (`001`~`122`), MATCH_OK 80개, NORESULT 42개
- `B\`: 122개 (`001`~`122`), MATCH_OK 87개, NORESULT 35개
- 파일명 예: `A\001_NG_die_pad_013_ng__MATCH_OK.png`
- `NORESULT`도 제외하지 않고 해당 실행 overlay를 그대로 포함했다.

전체 manifest:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-score90-angle05-20260828-individual-detections\manifest.json`

각 variant manifest와 `README.md`도 A/B 폴더 안에 있다. manifest 기준 전체
244행, `copiedExact=true`, 원본/개별 파일 해시 불일치 0건이다.

## 8. 완료 기록

```text
Status: Complete
Scope: AUTO_SELECTION Skill 규칙 추가, 008 positive-reference exemplar 보정, A/B 전역 후보 평가, 자동 선택 A 재실행, 122장×A/B 개별 overlay export
Acceptance criteria:
  - 승인 없이 전역 후보를 선택하거나 명시적 자동 거부: PASS (A selected; 008 reference exemplar checked)
  - per-image template/ROI/parameter switching 없음: PASS (고정 XML/corpus)
  - SCORE_MIN=90 수락 행 유지: PASS (selected A 80/80)
  - 선택 후보 전체 재실행: PASS (122/122, missing 0; exemplar 해석 최신 A 실행도 122/122)
  - 선택 전/후 결과 재현: PASS (상태 차이 0, score max delta 0)
  - A/B 개별 이미지 export: PASS (244 files, hash mismatch 0)
  - qualification 과장 방지: PASS (qualification=false)
Verification: quick_validate.py, dotnet build, OpenVisionReadinessCheck, TestExternalReferences.ps1, TestPublicSampleAssets.ps1, WriteDiePadFullVisualReview.ps1 (244 rows), SelectDiePadAutoTemplate.ps1 (reference exemplar), ExportDiePadIndividualDetections.ps1 (244 files/hash check)
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-auto-selection-exemplar-20260828-rerun02; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-exemplar-20260828-rerun02-individual-detections; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full
Boundary / next dependency: current angle05 full visual reclassification, held-out evidence, and production qualification remain unverified; no operator approval is required for the selection itself.
```
