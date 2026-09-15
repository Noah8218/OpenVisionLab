# OpenVisionLab Rule-Based Teaching Skill — A/B 템플릿 비교 검증

Date: 2026-08-27 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — bounded numeric pilot; visual correspondence review remains WAIT; B variant not admitted**
Skill: `openvisionlab-rule-based-teaching`

## 1. 검증 목적과 범위

이번 검증은 새 Skill의 조건부 템플릿 문맥 정책을 실제 샘플에 대입한 것이다.
사용자가 제안한 다음 두 안을 같은 결정론적 검사 경로에서 비교했다.

- **A안**: 안정적인 core와 실제 경계선만 포함하는 최소 충분 영역
- **B안**: 같은 core에 우측·하단의 추가 문맥을 포함하는 영역

다음 조건을 고정하고 template 파일 경로만 바꿨다.

- `Matching(NUM_MATCH=2)` -> `Matching(NUM_MATCH=1)` -> `RotateScale`
  -> `Threshold` -> `Blob`
- search ROI, inspection ROI, threshold, 면적 범위, angle/scale search,
  acceptance gate, image list를 동일하게 유지
- 각 variant에 대해 source snapshot, runtime result overlay, Matching overlay,
  CSV row, SHA-256을 보존

이 검증은 Skill 문서와 현재 Dev console runner의 연결을 확인하는 bounded pilot이다.
Good/Bad qualification, 제품 recipe 승인, UI/EXE smoke, release, deployment는
범위에 포함하지 않았다.

## 2. 입력 corpus와 variant

### Corpus

- Dataset: `E:\라벨테스트\EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`
- Frozen native batch root:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-e-die-pad-20260827`
- Image list:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\image-list.txt`
- 전체 122장: train 82 / validation 27 / test 13
- corpus role label: OK 62 / NG 60
- role label은 이 locator-relative-blob 실행의 defect truth가 아니다.
- source summary SHA-256:
  `633DAA12038204BB8E46D177743066ACB748CDD2BE5581D9B5B2D23921D50318`
- frozen native rows SHA-256:
  `A3D2EA52069369876D51A924BB443F03F437D1584D561931E51332BA2219E57D`
- image list SHA-256:
  `CB9B33F82B0FA5E31E7620A47444023472448F5DD797F59976EF69E0344EC1BD`

### Template source

두 template은 `die_pad_003_ok.jpg`에서 생성한 0° 비교 후보이다.
원본을 덮어쓰지 않았고, 자동 회전·리사이즈·샘플별 보정을 적용하지 않았다.

| Variant | Crop | Size | SHA-256 |
| --- | --- | ---: | --- |
| A | `(x=190, y=184, width=169, height=128)` | `169x128` | `80C643849C42E35FD0A7CD85A8FDC45E5B4DD15EAFFD41A6FE24FDCC067AF404` |
| B | `(x=190, y=184, width=179, height=138)` | `179x138` | `1AFD6A0DF1FD98A26308953AA970B9A94DA94004B15C860420021E3F16101CB2` |

원본 source SHA-256은 `33F3AFC3FB6B6A602E8D8A5EEB89AE83481519BF61CA7EA63259C6CB0147D889`이다.
템플릿 원본과 경계 비교 overlay는 다음에 있다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-20260827-213530\source_ab_rectangles.png`

## 3. 고정 pipeline과 실행 증거

비교 root:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206`

고정 조건:

- pipeline: `Matching(2) -> RotateScale -> Threshold -> Blob`
- search ROI: `0,0,512,512`
- downstream inspection ROI: `190,220,175,130`
- threshold: `170`
- Blob area: `700..1300`
- Matching angle: `-5..5`
- Matching scale: `0.75..1.35`
- acceptance: Step 1 `ScoreMargin >= 10`; Step 5 fixed Blob acceptance

`comparison-manifest.json`의 SHA와 XML을 확인했고, 두 pipeline XML은
template path placeholder로 정규화했을 때 동일했다. 각 XML에는 해당 variant의
template path가 두 Matching 단계에만 기록되어 있다.

실행한 명령:

```text
dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform="Any CPU" --nologo

dotnet run --no-build --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -- --batch-evidence <image-list> <dataset-root> <A pipeline.xml> <A batch.csv> <A evidence>

dotnet run --no-build --project tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -- --batch-evidence <image-list> <dataset-root> <B pipeline.xml> <B batch.csv> <B evidence>

python C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching
```

실행 결과:

- runner build: exit `0`, warnings `0`, errors `0`
- Skill validator: `Skill is valid!`
- A: 122 rows / 122 completed / missing `0` / pipeline pass `77`
- B: 122 rows / 122 completed / missing `0` / pipeline pass `73`

## 4. A/B 결과

| 지표 | A | B |
| --- | ---: | ---: |
| Matching Step 1 성공 | 122 / 122 | 122 / 122 |
| Step 1 `ResultCount=1` | 122행 | 122행 |
| 실제 두 번째 후보 | 0행 | 0행 |
| `ScoreMax` 평균 | 91.916 | 92.298 |
| `ScoreMax` 최소 / 최대 | 82.635 / 99.078 | 81.989 / 98.856 |
| Step 5 Blob 성공 | 77 / 122 | 73 / 122 |
| Step 5 Blob 실패 | 45 | 49 |
| Step 5 `ResultCount` 분포 | 0:45, 1:15, 2:25, 3:35, 4:2 | 0:49, 1:11, 2:24, 3:37, 4:1 |
| Fixture center 평균 | (285.746, 257.844) | (291.361, 263.418) |

점수 차이의 세부 결과:

- B - A 평균: `+0.382 pp`
- B - A 중앙값: `+0.475 pp`
- B가 높은 행: `89`
- 동일 행: `0`
- B가 낮은 행: `33`
- 절대 차이 평균: `0.800 pp`
- 절대 차이 최대: `2.087 pp`

### 두 번째 후보 해석 주의

현재 `VisionRecipeRunnerSmoke`의 `evidence_rows.csv` header에는
`ScoreSecond`가 있으나 `WriteEvidenceCsvRow`의 metric 목록은 해당 위치에
`ScoreMin`을 기록한다. 따라서 `ResultCount=1`인 행에서 CSV의
`ScoreSecond` 값은 실제 두 번째 후보 점수가 아니다. 이 비교에서는
`batch.csv`의 `ResultCount`를 실제 후보 수의 권위 있는 값으로 사용했다.

즉 두 variant 모두 122장 전부에서 단일 후보만 관찰했으므로, B가 ambiguity를
줄였다는 best-versus-second 근거는 확보되지 않았다. 점수 평균 상승만으로는
템플릿 문맥 정책의 B 조건을 충족하지 않는다.

### LocatorFrame과 후속 검사 영향

B template의 우측·하단 확장으로 template 중심이 바뀌어 published fixture pose도
전 행에서 이동했다.

- center가 달라진 행: `122 / 122`
- 평균 이동: `+5.742 px X`, `+5.574 px Y`
- 최대 이동: `+7 px X`, `+9 px Y`

후속 Blob ROI를 동일하게 고정한 상태에서의 전이는 다음과 같다.

- A pass -> B fail: `5`행
- A fail -> B pass: `1`행
- 양쪽 pass: `72`행
- 양쪽 fail: `44`행
- 순감소: `4`행

따라서 B의 점수 상승은 localization/downstream 안정성 상승으로 이어지지
않았다. 오히려 이 고정 recipe에서는 B가 downstream 결과를 악화시켰다.

## 5. 대표 current-run 시각 증거

아래 overlay는 이번 A/B 실행에서 생성된 현재 결과이다.

- ordinary sample A:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\A\evidence\runs\OK_die_pad_003_ok\runtime_result.png`
- ordinary sample B:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\B\evidence\runs\OK_die_pad_003_ok\runtime_result.png`
- A pass / B fail sample A:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\A\evidence\runs\NG_die_pad_074_ng\runtime_result.png`
- A pass / B fail sample B:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\B\evidence\runs\NG_die_pad_074_ng\runtime_result.png`
- A fail / B pass sample A:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\A\evidence\runs\OK_die_pad_218_ok\runtime_result.png`
- A fail / B pass sample B:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\B\evidence\runs\OK_die_pad_218_ok\runtime_result.png`

각 `runtime_result.png`에는 Matching/fixture/Normalize/Blob ROI 및 해당 실행에서
생성된 결과 geometry가 겹쳐져 있다. 개별 Matching overlay는 각 run directory의
`01_matching_overlay.png`에 있다. 모든 source snapshot, overlay, row metric은
같은 run directory에 보존했다.

## 6. 정책 판정

**A를 기본안으로 유지한다. B는 이번 fixed recipe에서 operator 승인 대상으로
올리지 않는다.**

판정 근거:

1. B에서 실제 경쟁 후보 감소가 관찰되지 않았다.
2. B는 template 중심 변경으로 전 행의 LocatorFrame pose를 이동시켰다.
3. 동일 downstream Blob ROI에서 pipeline pass가 77 -> 73으로 감소했다.
4. 따라서 `template-selection-policy.md`의 “반복되는 물리 문맥 + 실제 모호성
   감소 + localization/downstream regression 없음”을 만족하지 않는다.

B가 절대 사용할 수 없다는 뜻은 아니다. B를 다시 검토하려면 다음을 모두
별도의 명시적 recipe variant에서 수행해야 한다.

- B 기준으로 reference pose와 후속 ROI를 다시 teaching
- 고정 Train/Validation/Held-out corpus 재실행
- 실제 두 번째 후보 또는 다른 경쟁 구조의 감소를 overlay로 확인
- angle/scale/pose 및 후속 metric regression 확인
- operator가 variant를 명시적으로 승인

이미지마다 A/B를 자동 전환하거나 runtime에서 B로 바꾸는 것은 허용하지 않는다.

## 7. 산출물과 재현 경로

비교 manifest:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\comparison-manifest.json`

파일별 전체 비교:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\ab-comparison.csv`

전체 요약:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\ab-comparison-summary.json`

current-run 결과 보고서:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\ab-comparison-report.md`

두 variant의 pipeline/template 파일과 evidence는 같은 비교 root 아래 `A`와
`B`로 분리되어 있다. template/pipeline SHA-256은 manifest와 현재 파일이
일치한다.

## 8. 완료 기록

```text
Status: Complete
Scope: Die Pad 122장에 대한 A/B template-context bounded pilot 비교
Acceptance criteria:
  - 두 variant 동일 조건 실행: PASS
  - A/B template/pipeline hash 보존: PASS
  - 122행씩 실행, 누락 0: PASS
  - runtime result overlay 및 row metrics 보존: PASS
  - B 채택 여부를 ambiguity/pose/downstream 근거로 판정: PASS (B not admitted)
Verification:
  - VisionRecipeRunnerSmoke build: exit 0, warnings 0, errors 0
  - A batch: 122 completed, pipeline pass 77
  - B batch: 122 completed, pipeline pass 73
  - quick_validate.py: Skill is valid!
Evidence:
  - comparison-manifest.json, ab-comparison.csv, ab-comparison-summary.json
  - A/B evidence/runs/*/runtime_result.png and 01_matching_overlay.png
Boundary / next dependency:
  - qualification, defect truth, production approval, UI/EXE smoke, release/deployment는 증명하지 않음
  - B 재검토에는 별도 pose/ROI teaching과 held-out evidence 및 operator 승인이 필요
```

## 9. 시각 대응 재검토 연결

이 보고서의 숫자/overlay 비교만으로 실제 template-to-source 구조 대응을
확정하지 않는다. 대표 샘플의 source patch/template/blend를 별도로 확인한
결과와 Skill hard gate는 다음 보고서에 기록했다.

`docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_VISUAL_CORRESPONDENCE_REVIEW_20260827.md`

현재 상태는 `WAIT_FOR_OPERATOR_REVIEW`이며, 높은 `ScoreMax`나 초록 `01`
overlay가 시각적 대응을 대체하지 않는다.
