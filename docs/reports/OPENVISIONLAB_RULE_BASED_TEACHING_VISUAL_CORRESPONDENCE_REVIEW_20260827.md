# OpenVisionLab Rule-Based Teaching Skill — 시각적 매칭 대응 재검토

Date: 2026-08-27 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — bounded spot review; corpus-wide visual approval remains WAIT**
Skill: `openvisionlab-rule-based-teaching`

## 1. 사용자 지적과 재검토 목적

앞선 A/B 비교는 `ScoreMax`, `ResultCount`, 후속 Blob 통과 수를 중심으로
판단했다. 사용자는 “점수가 높아도 실제 이미지가 template과 매칭되는지는
별도로 봐야 하며, 초록색이 최종 검출인지도 명확히 해야 한다”고 지적했다.
이 지적을 반영하여 이번 재검토의 우선순위를 다음처럼 변경했다.

```text
실제 구조 대응(visual correspondence)
  -> 후보 위치/자세/문맥의 물리적 타당성
  -> 실제 경쟁 후보 존재 여부
  -> ScoreMax/ScoreMargin/ResultCount
  -> 후속 검사 결과
```

시각 대응이 `WAIT` 또는 `FAIL`이면 점수 gate를 통과해도 매칭을 성공으로
판정하지 않는다.

## 2. 초록색 overlay의 의미

현재 `VisionRecipeRunnerSmoke` 코드의 `SaveStepOverlayImage`에서 초록색은
`Matching` Step 1 overlay에 사용된다. 초록 `01` 사각형과 중심 표시는
Matching 후보 geometry를 보여주는 것이며, 최종 Good/NG 판정이나 semantic
검출 확정이 아니다.

현재 current-run 결과의 색상 의미는 다음과 같다.

- 초록 `01`: Matching Step 1 ROI/후보 geometry
- 파랑 `02`: publish된 locator pose
- 노랑/주황 `03`: Normalize/transform 단계
- 자홍 `05`: Blob inspection ROI와 결과 geometry

색상은 step presentation이며 acceptance truth가 아니다. 반드시 step label,
template 구조, source patch, 후속 결과를 함께 확인해야 한다.

## 3. 재검토 방법

기존 A/B 122장 배치의 동일 source/template/XML/metric/overlay를 사용하고,
대표 5장에 대해 A와 B 각각을 다음 방식으로 확인했다.

1. template의 stable core를 명시한다: 두 패드, 패드 수와 순서, 패드 간격,
   L형 연결 trace, 우측 수직 trace.
2. Step 2에서 보고한 center/scale을 기준으로 source candidate patch를
   추출한다.
3. template, source patch, 50% blend panel, exact runtime Matching overlay를
   함께 본다.
4. 같은 구조가 대응하는지, 가려짐/추가 구조/반복 구조 오검출이 있는지
   분류한다.
5. 점수는 그 다음에만 참고한다. 대표 5장만으로 122장 전체를 승인하지
   않으며, 나머지 행은 `NOT_REVIEWED`로 남긴다.

candidate patch panel은 center/scale 기반의 시각 검토 보조물이다. 이는
새로운 runtime detector나 수동 annotation이 아니며, exact runtime geometry는
각 run 폴더의 `01_matching_overlay.png`와 `runtime_result.png`에 보존되어
있다.

## 4. 대표 결과

| Sample | A 상태 | B 상태 | 관찰 |
| --- | --- | --- | --- |
| `die_pad_003_ok.jpg` | `PASS_ON_SPOT_CHECK` | `PASS_CORE_SINGLE_SAMPLE` | 두 패드·L형 trace·우측 수직 trace가 core와 대응하는 것으로 보임. 단일 샘플 증거 |
| `die_pad_074_ng.jpg` | `PASS_CORE_ON_SPOT_CHECK` | `PASS_CORE_CONTEXT_UNQUALIFIED` | core는 대응하지만 B 추가 문맥의 반복성은 확인되지 않음 |
| `die_pad_163_ng.jpg` | `PASS_CORE_ON_SPOT_CHECK` | `WAIT_CONTEXT_MISMATCH` | B candidate patch의 추가 문맥에 template와 다른 밝은/사선 구조 |
| `die_pad_026_ng.jpg` | `WAIT_OCCLUSION_TOLERANCE` | `WAIT_OCCLUSION_TOLERANCE` | 좌측 패드가 밝은 구조물에 가려짐. 허용 occlusion 규칙 없음 |
| `die_pad_198_ng.jpg` | `WAIT_EXTRA_STRUCTURE` | `WAIT_EXTRA_STRUCTURE` | candidate 좌측에 template에 없는 밝은 구조가 추가됨 |

`die_pad_026_ng.jpg`는 특히 중요한 반례다. A 점수는 약 `84.46`, B 점수는
약 `83.18`이지만, 좌측 패드의 실제 시각 대응이 가려져 있다. 점수 gate를
통과하거나 초록 사각형이 그려졌다는 사실만으로 이 후보를 “좋은 매칭”으로
부를 수 없다. `die_pad_198_ng.jpg`도 추가 구조의 물리적 의미가 확인되지
않아 operator tolerance 없이는 `WAIT`다.

## 5. Skill 변경

다음 제약을 실제 Skill에 추가했다.

- Matching/EdgeBasedMatching은 **visual correspondence를 hard gate**로
  먼저 확인한다.
- template와 reported pose의 source patch, side-by-side/blend, full-image
  overlay, hash를 evidence packet에 보존한다.
- 같은 패드/객체 수와 순서, trace/edge 관계, 상대 간격, polarity/contrast,
  필요한 경계 문맥이 대응하지 않으면 score와 무관하게 `WAIT`/`REJECTED`다.
- 가려짐은 operator가 명시한 허용 범위와 현재 tool contract가 있을 때만
  허용한다. 그 외에는 `WAIT`다.
- 초록 `01`은 Matching Step 1 geometry일 뿐 최종 검출/Good/NG가 아니다.
- 높은 ScoreMax나 ScoreMargin을 시각 대응의 대체 근거로 사용하지 않는다.
- 실제 두 번째 후보가 별도로 보이지 않으면 ambiguity 감소를 주장하지 않는다.
- 출력 계약에 `visualCorrespondence` 상태
  (`PASS|FAIL|WAIT|NOT_REVIEWED`)를 추가했다.

변경 파일:

- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\visual-correspondence-review.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\template-selection-policy.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\detection-point-contract.md`

## 6. 현재 판정

앞선 numeric A/B 결과의 “A 기본, B 보류” 결론은 유지한다. 다만 이를
“Matching이 122장 모두 시각적으로 잘 되었다”로 확대해서는 안 된다.

- A/B 둘 다 representative visual review가 전체 corpus에 대해 완료되지 않음
- 일부 high-score row에는 실제 구조 가림/추가 구조가 존재
- 따라서 현재 Skill teaching state는 `WAIT_FOR_OPERATOR_REVIEW`
- A도 operator 승인 전의 보수적 기본 제안이지 `QUALIFIED`가 아님
- B는 visual context mismatch와 downstream regression 때문에 승인 대상이 아님

## 7. 증거 위치

Current-run visual review root:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\visual-correspondence`

- `visual-correspondence-review.md`: 대표 재검토 결과
- `visual-correspondence-review.json`: 각 panel/source/template/overlay/metric/hash와 상태
- `visual-correspondence\A_*_correspondence.png`, `B_*_correspondence.png`:
  template / source patch / 50% blend
- 각 A/B `evidence\runs\<sample>\01_matching_overlay.png`:
  exact runtime Matching overlay
- 각 A/B `evidence\runs\<sample>\runtime_result.png`:
  전체 step 결과 overlay

기존 A/B 전체 행 비교는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206\ab-comparison.csv`와
`ab-comparison-summary.json`에 있다.

## 8. 경계

- corpus의 OK/NG는 이 재검토에서 defect truth가 아니라 role label이다.
- 이번 대표 5장 spot review는 122장 전체의 시각 대응 qualification이 아니다.
- ScoreSecond CSV 열의 current runner labeling 문제는 기존 A/B 보고서의
  caveat로 보존했으며, 실제 후보 수는 `ResultCount`로 판단했다.
- 제품 runtime을 AI detector로 변경하거나, 이미지별 template/ROI를 자동
  튜닝하거나, release/deployment를 수행하지 않았다.

## 9. 2026-08-28 전수 재검토

이 문서는 당시 5장 bounded spot review를 보존한다. 전체 122장에 대한 새
runner 재실행과 A/B 244행 시각 ledger는 다음 문서와 D-drive evidence root에
기록했다.

`docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_FULL_VISUAL_CORRESPONDENCE_20260828.md`

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260828-full-visual\visual-correspondence-full\full-visual-review.csv`

전수 결과는 A `104 PASS / 18 WAIT`, B `103 PASS / 19 WAIT`이며 operator 승인
전에는 어느 variant도 qualified로 부르지 않는다.
