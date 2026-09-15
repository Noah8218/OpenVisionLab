# OpenVisionLab Rule-Based Teaching Skill

Date: 2026-08-27 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Status: **Complete — personal Codex Skill updated with conditional A/B template policy and validator-checked**

## 1. 사용자 요청의 정확한 목적

이번 요청의 핵심은 “회전된 템플릿을 처리하는 문구”가 아니다. 사용자가
샘플 이미지를 제공하면 Codex/LLM이 OpenVisionLab의 **룰베이스 검사 방식**을
읽고 다음을 제대로 선택·설명하도록 하는 재사용 가능한 Codex Skill을 만드는
것이다.

- 어떤 물리적 특징을 검출 기준점으로 삼을지
- 어떤 Point/Line/Intersection/Center/Blob/Template 표현이 맞는지
- 어떤 ROI와 좌표계를 사용할지
- Matching, NormalizeImage, Threshold, Blob, Contour, Geometry 등의 도구를
  어떤 순서로 연결할지
- 무엇이 관찰 사실이고 무엇이 operator 승인 전의 제안인지
- 어떤 overlay, hash, metric, gate가 있어야 실제 티칭·검증으로 넘어갈 수 있는지

따라서 기존 `locator-relative-blob-v1` 문서만 고치는 것으로는 부족했고,
개인 Codex 환경에서 자동 발견/명시 호출할 수 있는 실제 `SKILL.md`가
필요했다.

## 2. 생성된 산출물

### 실제 Codex Skill

- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\SKILL.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\agents\openai.yaml`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\detection-point-contract.md`
- `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\template-selection-policy.md`

호출 예시는 다음과 같다.

```text
$openvisionlab-rule-based-teaching
```

또는 이미지 작업 요청에 다음처럼 명시할 수 있다.

```text
Use $openvisionlab-rule-based-teaching to teach the detection point from the
supplied Good/Bad images and return an operator-reviewable plan.
```

### Skill이 담당하는 범위

1. 입력 이미지의 경로·크기·해시·시각 근거 확인
2. 안정적인 물리 datum과 후보 검출점의 선택 근거 기록
3. Point/Line/Intersection/Center/Blob/Template 표현 선택
4. SourceFrame/LocatorFrame 등 좌표계의 명시와 혼용 방지
5. ROI의 경계·문맥·경쟁 구조 검토
6. Matching/NormalizeImage/Threshold/Blob/Contour/Geometry 등 현재 도구
   계열에 맞는 최소 결정론적 조합 제안
7. operator-owned gate와 measurement-only metric의 분리
8. `READY_FOR_OPERATOR_REVIEW`, `WAIT`, `MEASURE_ONLY`, `REJECTED` 상태와
   증거 패킷 후보 반환
9. 안정적인 구조 중심의 A안과 반복되는 물리 문맥을 포함하는 B안의 조건부
   템플릿 선택 및 operator 승인 상태 기록

### Skill 문서가 여러 파일로 나뉘는 이유

이 Skill은 하나의 긴 문서가 아니라 역할별로 나뉜다.

- `SKILL.md`: 자동 발견 시 먼저 읽는 진입점, 적용 범위, 핵심 불변 규칙,
  출력 계약, 세부 reference 라우팅
- `agents/openai.yaml`: Codex UI의 표시명·설명·기본 호출 문구
- `references/detection-point-contract.md`: Tool family, 좌표계, 회전,
  evidence packet의 상세 계약
- `references/template-selection-policy.md`: 이번에 추가한 A/B 템플릿 문맥
  선택 절차와 B 승인 조건
- `docs/reports/...`: 저장소에서 재사용할 결정·근거·검증 기록. Skill이
  매번 자동으로 모두 읽는 파일은 아니며, `docs/LLM_DOCUMENT_INDEX.json`의
  route가 필요한 프로젝트 문서를 안내한다.

따라서 파일이 여러 개인 것은 중복 관리가 아니라, **진입점·세부 규칙·UI
메타데이터·프로젝트 증거를 분리하여 필요한 부분만 읽게 하는 구조**다.

## 3. 핵심 규칙

### 검출점 규칙

- `NO EVIDENCE, NO COORDINATE`: 이미지·현재 overlay·검토된 plan·해시 검증
  packet에 없는 좌표/ROI/template/candidate ID/XML 값을 만들지 않는다.
- 결함, 글자, 테두리, glare/shadow, 압축 노이즈, 이미지 하나의 우연한
  outlier는 기본 datum으로 선택하지 않는다.
- 가장 강한 근거는 operator가 명명한 물리 datum, 여러 라벨 이미지에 반복되는
  unique template/fixture, 이전 deterministic Step이 낸 typed feature 순서다.
- 후보가 여러 개이고 disambiguation rule이 없으면 성공으로 고르지 않고
  `WAIT`/`REJECTED`로 남긴다.
- raw pixel은 operator가 검토할 임시 제안일 뿐, 설명 없는 최종 teaching 값이
  아니다.

### 회전 템플릿 규칙

- coarse orientation은 operator가 `0/90/180/270` 중 명시적으로 선택한다.
- 원본을 덮어쓰지 않고 derived copy에 한 번만 적용한 다음 그 보정 이미지에서
  template/point/ROI를 가르친다.
- 90/270은 이미지 width/height가 교환되며 point, line endpoint, template ROI,
  search ROI, reference pose를 동일한 방향으로 변환한다.
- orientation/source hash/corrected hash/template/ROI/frame가 바뀌면 이전
  packet과 compile 상태는 stale이다.
- 실행 중 입력 이미지 자동 회전, 샘플별 임의 회전은 하지 않는다. 측정된 작은
  residual angle은 coarse orientation과 별도의 locator fine-alignment 근거다.

### 템플릿 문맥 A/B 규칙

- A안은 stable core를 담는 최소 충분 영역이며 기본 제안이다.
- B안은 동일한 core에 반복성이 입증된 물리적 외곽선·Fixture 문맥을 추가한
  경우에만 제안한다. 균일하거나 변동 가능한 배경은 B의 근거가 아니다.
- B는 고정된 대표 샘플에서 동일한 matcher·gate·좌표계로 A/B를 비교하고,
  실제 경쟁 후보 감소와 위치·각도·배율·후속 검사 안정성을 확인한 뒤에만
  operator 승인 대상으로 올린다.
- 증거가 부족하면 A를 보수적 제안으로 남기고 `READY_FOR_OPERATOR_REVIEW`
  상태를 유지한다. 이미지마다 variant를 바꾸거나 runtime에서 자동 전환하지
  않는다.
- 자세한 절차는
  `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\template-selection-policy.md`
  에 둔다.

### LLM 경계

- 기존 `locator-relative-blob-v1`에서는 LLM이 자유 좌표를 XML에 쓰지 않는다.
  유효한 evidence packet 안의 `CandidateId`만 선택할 수 있고, 좌표와 고정
  5-Step graph는 deterministic compiler가 소유한다.
- Skill은 packet validator, XML validation/import, explicit Preview/Run을
  우회하지 않는다.
- Skill 사용만으로 레이어/라우팅을 바꾸거나 Preview/Run, 배포, release를
  실행하지 않는다.

## 4. 현재 제품과의 연결

이 Skill은 OpenVisionLab의 제품 정체성을 바꾸지 않는다.

```text
샘플 이미지
  -> Skill이 근거 기반 teaching plan 제안
  -> operator가 overlay/좌표/ROI 검토·승인
  -> 기존 PropertyGrid / Pipeline / explicit Preview·Run
  -> Pipeline Review의 drawing·metric·object evidence
  -> Good/Bad/N-image validation
```

`locator-relative-blob-v1`의 기존 계약은 그대로 유지한다. Skill은 그 계약에
넣을 수 있는 teaching 입력을 준비·검토하는 upstream guidance이고, 새로운
runtime detector나 두 번째 coordinate contract가 아니다.

참고로 현재 Die Pad pilot에서 기록된 `-1.790° -> 0°`는 이미 측정된 fine
residual correction이다. 새 Skill의 coarse 90-degree teaching 선택으로
재해석하지 않는다.

## 5. 산출물 형식

Skill은 Markdown 결과와 요청 시 다음 JSON 구조를 사용한다.

```json
{
  "skillId": "openvisionlab-rule-based-teaching",
  "status": "READY_FOR_OPERATOR_REVIEW",
  "inspectionIntent": "<operator wording>",
  "reference": {
    "sourcePath": "<absolute path>",
    "sourceSha256": "<64 hex chars or UNKNOWN>",
    "orientationMode": "0|90|180|270",
    "orientationSelection": "OPERATOR_SELECTED|PENDING",
    "originalSize": { "width": 0, "height": 0 },
    "taughtSize": { "width": 0, "height": 0 }
  },
  "coordinateFrame": "SourceFrame|LocatorFrame|<current contract frame>",
  "templateSelection": {
    "variant": "A|B|PENDING",
    "state": "PROPOSED|OPERATOR_APPROVED|WAIT",
    "contextType": "NONE|PHYSICAL_BOUNDARY|FIXTURE|UNKNOWN",
    "reason": "<evidence-backed reason>",
    "evidence": [],
    "operatorApproval": "REQUIRED"
  },
  "visualCorrespondence": {
    "state": "PASS|FAIL|WAIT|NOT_REVIEWED",
    "reviewedSamples": [],
    "stableFeatures": [],
    "missingOrExtraStructure": [],
    "evidence": [],
    "operatorApproval": "REQUIRED"
  },
  "detectionPoints": [],
  "regions": [],
  "toolPlan": [],
  "gates": [],
  "rejectedAlternatives": [],
  "unknowns": [],
  "nextAction": "Review overlay, then explicitly teach/compile/run"
}
```

모르는 값은 `0`으로 위장하지 않고 `UNKNOWN`/`PENDING`으로 표시한다.
모든 좌표는 operator가 현재 overlay를 확인하기 전까지 `PROPOSED`이다.

## 6. 검증 결과

실행한 검증:

```text
python C:\Users\USER\.codex\skills\.system\skill-creator\scripts\quick_validate.py C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching
=> Skill is valid!

독립 점검
=> JSON output contract parse: PASS
=> NO EVIDENCE, NO COORDINATE rule: PASS
=> per-image tuning 금지 규칙: PASS
=> conditional A/B template-selection rule: PASS
=> B variant requires fixed-sample evidence and operator approval: PASS
=> 0/90/180/270 orientation 규칙: PASS
=> rectangular ROI 90/180/270 변환 및 bounds: PASS
```

검증용 기본 Python에 `PyYAML`이 없어 처음 validator가 중단되었고, 로컬
사용자 범위에만 `PyYAML 6.0.3`을 설치한 후 재실행하여 통과했다. 이 설치는
제품 저장소, 제품 코드, 원본 저장소, 배포물에는 영향을 주지 않는다.

## 7. 의도적으로 포함하지 않은 것

- LLM provider/API/browser automation
- 이미지마다 threshold/area/ROI를 자동 튜닝하는 loop
- runtime에서 방향을 추정해 자동 회전하는 detector
- 새 WPF UI, 새 Pipeline Step, 새 XML schema
- frozen Train/Validation/Held-out qualification
- release/tag/push/deployment

이 항목들은 Skill의 부족함을 숨긴 것이 아니라, 현재 OpenVisionLab의
rule-based-first 및 evidence-first 계약을 지키기 위한 경계다. 실제 operator가
반복적으로 같은 입력을 주고도 특정 안내가 부족하다는 evidence가 생기면 그
사례 하나를 기준으로 Skill을 좁게 보완한다.

## 8. 이번 A/B 템플릿 정책 산출물

현재 Die Pad 기준의 비교 후보도 별도 D-drive evidence 폴더에 보존했다.

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-20260827-213530\template_A_boundary_tight.png`
  — 실제 외곽선까지 포함한 A안, `169x128`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-20260827-213530\template_B_edge_plus_context.png`
  — 동일 core에 우측·하단 문맥을 추가한 B안, `179x138`
- 두 파일 모두 원본 `die_pad_003_ok.jpg`에서 `0°`로 생성했으며, 별도
  리사이즈·보정·자동 회전은 하지 않았다.
- 두 안을 고정 Die Pad 122장에 같은 pipeline/ROI/gate로 실행했다. A는
  122장 중 77장, B는 73장이 고정 downstream Blob 단계까지 통과했다.
- 두 안 모두 122장 전부에서 Matching `ResultCount=1`만 기록되어 실제
  두 번째 후보는 관찰되지 않았다. 따라서 B가 모호성을 줄였다는 근거는
  확보되지 않았다.
- B는 template 중심 확장 때문에 published fixture center가 전 행에서
  평균 `+5.742 px X / +5.574 px Y` 이동했고, A pass -> B fail이 5행
  발생했다. 고정 recipe 기준 판정은 **A 기본 유지, B 승인 보류**다.
- 전체 비교 보고서:
  `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_TEMPLATE_AB_VALIDATION_20260827.md`
- current-run evidence root:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\die-pad-template-ab-validation-20260827-215206`

## 9. 시각적 매칭 대응 재검토

사용자 피드백을 반영해 Matching의 초록 `01` overlay를 최종 검출로
해석하지 않도록 수정했다. 초록색은 현재 runner에서 Matching Step 1의
ROI/후보 geometry일 뿐이며, template와 source patch의 실제 구조 대응을
먼저 확인해야 한다.

대표 5장 spot review에서 `die_pad_026_ng`는 좌측 패드가 밝은 구조물에
가려졌고, `die_pad_198_ng`는 template에 없는 추가 구조가 후보 patch에
나타났다. 두 샘플 모두 높은 점수만으로 매칭을 승인하지 않고
`WAIT`로 남겼다. 122장 전체 시각 대응은 아직 operator review 전이므로
현재 Skill 상태는 `WAIT_FOR_OPERATOR_REVIEW`다.

- 세부 보고서:
  `docs/reports/OPENVISIONLAB_RULE_BASED_TEACHING_VISUAL_CORRESPONDENCE_REVIEW_20260827.md`
- 세부 reference:
  `C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\visual-correspondence-review.md`

## 10. 다음 우선순위

1. B를 다시 검토하려면 B 기준 reference pose/후속 ROI를 별도 재티칭한
   recipe variant를 만들고 고정 Train/Validation/Held-out corpus에서
   실제 경쟁 후보·overlay·후속 metric regression을 다시 증명한다. 그 전에는
   A를 보수적 기본안으로 유지한다.
   `Recommended model: gpt-5.6-terra` | `Reasoning effort: medium`
2. 별도 operator 승인이 생기면 선택된 variant만 기존
   `locator-relative-blob-v1` packet 경계에 연결한다.
   `Recommended model: gpt-5.6-sol` | `Reasoning effort: high`
3. 제품의 현재 남은 큰 우선순위는 CVR-00이며, 독립 초보 사용자 3명의
   실제 관찰 전까지 보류한다. `Recommended model: none until evidence` |
   `Reasoning effort: none until evidence`
