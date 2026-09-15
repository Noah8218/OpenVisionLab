# OpenVisionLab Rule-Based Skill Research Plan

Date: 2026-08-31 KST
Repository: `C:\Git\2D\Dev`
Status: **Approved — v0.1.5 Round 1 benchmark FAIL; v0.1.6/v0.1.7/v0.1.8 historical admissions include incomplete evidence and a completed v0.1.8 backend-recovery FAIL; v0.1.10 focused correction and strict preflight PASS with admitted Round 1 FAIL; v0.1.11 focused correction and strict corpus preflight PASS with admitted Round 1 FAIL; v0.1.12 failure-derived focused correction PASS; candidate remains inactive; Round 2 not authorized**

이 문서는 사용자가 2026-08-31 승인한 1~3차 Rule-Based Skill 연구의
목표, 책임 구조, 평가 방식, 승격 조건을 고정한다. 상위 거버넌스는
`OPENVISIONLAB_RULE_BASED_SKILL_DEVELOPMENT_WORK_CONTRACT_20260831.md`,
실제 설치 상태와 책임은
`OPENVISIONLAB_RULE_BASED_SKILL_REGISTRY.json`이 소유한다.

여기서 `Research Round 1~3`은 기존 Pin intent 문서의 제품 증거
`Phase 1~3`과 구분한다. 이 계획은 새 제품 알고리즘, 암묵적 Preview/Run,
Recipe 자동 적용, 생산 qualification, release 또는 deployment를
승인하지 않는다.

## 1. 연구 목표

최종 목표는 다음과 같다.

> 실제 이미지와 운영자 검사 설명을 근거가 추적되는 하나의 전역
> Rule-Based Recipe 후보로 컴파일하고, 동일 데이터·도구·시간 조건의
> blind held-out 평가에서 숙련 작업자보다 우수한 범위를 입증한다.

이 목표에서 LLM/Codex는 매 이미지의 생산 검출기가 아니다. LLM은
검사 의도와 근거를 구조화하고 지원 Tool을 조합하며 XML 후보와 실패
이유를 만든다. 실제 검출은 동결된 OpenCvSharp4 Tool과 명시적인
OpenVisionLab 실행이 담당한다.

### 성공 주장의 두 수준

1. `AUTHORING_SUPERIOR`
   - 숙련자 대비 held-out Recipe 품질이 비열등하다.
   - Recipe 동결까지의 시간이 더 짧다.
   - critical safety violation이 없다.
2. `DETECTION_SUPERIOR`
   - 사전 등록된 검사 family에서 held-out 검출 품질 자체가 숙련자보다
     통계적으로 우수하다.
   - false accept와 wrong-geometry pass가 사람보다 나쁘지 않다.

Round 3 전에는 두 번째 주장을 사용하지 않는다. 전체 인간 또는 임의
이미지에 대한 범용 우월성은 이 연구의 목표가 아니다.

## 2. 비협상 계약

- 한 장의 이미지와 설명은 `PROPOSED` 또는 `MEASURE_ONLY` 초안에는
  충분할 수 있지만 생산 qualification 증거는 아니다.
- `No evidence, no coordinate`: 좌표, ROI, threshold, tolerance, calibration,
  dependency path, Tool, metric을 발명하지 않는다.
- 모든 이미지에 하나의 Recipe, Tool chain, ROI, template, parameter,
  orientation, acceptance gate를 사용한다.
- `perImageOverrides`는 항상 빈 배열이다.
- datum, ROI, geometry, metric은 하나의 명시된 frame과 owner를 가진다.
- 실제 물리 구조 대응과 runtime drawing을 score/count/time보다 먼저
  검토한다.
- XML 생성, 정적 검증, Import, Preview/Run, N-sample qualification은
  별도 상태와 별도 권한이다.
- upstream `WAIT|FAIL|REJECTED|NOT_REVIEWED`는 downstream success를
  차단한다.
- correction은 working Train/Validation에서 최대 두 번이며 한 번에
  하나의 반복 원인만 바꾼다.
- frozen held-out은 후보 동결 후 독립 runner가 한 번만 연다.

## 3. 책임 구조

```text
이미지 + 검사 설명
  -> openvisionlab-rule-based-teaching
     intent / observation / datum / ROI / frame / Tool plan / envelope
  -> openvisionlab-matching-teaching (Matching이 주 책임일 때만)
     template / pose / correspondence / Matching packet
  -> openvisionlab-recipe-xml-handoff candidate
     reviewed envelope -> VisionPipeline XML + validation handoff
  -> OpenVisionLab validator
     syntax / Tool / parameter / layer / dependency / intent contract
  -> 운영자가 명시적으로 승인한 Preview/Run
     metrics / overlays / fail reason / elapsed evidence
  -> frozen review queue / bounded correction / held-out / human comparison
```

| Owner | Owns | Does not own |
| --- | --- | --- |
| General teaching skill | 이미지·의도, datum, ROI/frame, 최소 Tool chain, generic envelope | XML runtime 실행, Matching 등록 |
| Matching skill | template, pose/frame, correspondence, finite global Matching selection | Blob/Contour/Line 정책, per-image 전환 |
| Recipe XML handoff candidate | reviewed envelope의 XML 직렬화와 validation handoff | 원본 이미지 재해석, Tool 정책 변경, Import/Run |
| OpenVisionLab | XML 검증, 명시적 실행, metrics/overlay/error | LLM 자동 승인 |
| Operator | 물리 의미, Good/Bad, tolerance, calibration, 실행과 qualification 승인 | skill 자동 진화 |
| Independent evaluator | frozen corpus, blind review, 인간 비교, 주장 범위 | 후보 작성자의 사후 조정 |

## 4. 입력 성숙도와 허용 출력

| 입력 수준 | 필수 입력 | 허용 출력 | 금지 주장 |
| --- | --- | --- | --- |
| L0 | 이미지 1장, 검사 설명, output primitive | Tool/ROI 후보, `PROPOSED` 또는 `WAIT` | 정확도, 강건성 |
| L1 | 대표 Good/Bad, 변동 조건, reviewed datum/ROI | 하나의 전역 Recipe 후보 | 생산 qualification |
| L2 | operator truth, tolerance/calibration, frozen Train/Validation | frozen candidate, error table, review queue | 인간 우월성 |
| L3 | 신규 held-out, 숙련 인간 기준선, 독립 reviewer | bounded qualification | 범용 우월성 |

Good/Bad나 tolerance가 없으면 `MEASURE_ONLY`, calibration 없이 물리 단위를
요구하면 `WAIT`, 여러 대상 후보를 구분할 근거가 없으면 `WAIT`, 현재 Tool로
표현할 수 없으면 `WAIT_ALGORITHM_GAP` 또는 `REJECTED`로 종료한다.

## 5. Research Round 1 — Grounded Recipe Authoring

### 질문

이미지와 설명이 현재 지원 Tool로 구성된 검토 가능한 전역 Recipe/XML
후보로 변환되는가?

### 범위

- S1 광도·색상: `Mean`, `Threshold`, `HSV`
- S2 영역·형상: `Threshold/HSV -> Morphology -> Blob/Contour`
- S3 경계·기하: `EdgeDetection -> LineGauge/LineDistance/GeometryMeasure`
- S4 위치 정규화: Matching packet -> normalized frame -> fixed ROI 검사
- 기존 Line, Blob, Contour, Matching, Mean starter intent를 우선 사용한다.

### 개발 단위

Round 1의 유효한 benchmark 비교 결과는 `openvisionlab-recipe-xml-handoff
0.1.3`이며, 현재 설치된 correction candidate는 `0.1.10`이다. 초기
`0.1.0` forward 평가에서 유사하지만 비호환인 handoff key drift가 실제로
관찰되어, `0.1.1`은 exact-shape/hash validator와 회귀 테스트를 추가했고,
후속 `0.1.2`는 상태 우선순위, authority/path/hash 전파, operator lock,
specialist frame/parameter 및 NormalizeImage gate, immutable baseline 보존,
artifact identity/order 검사를 보강했다.
후속 `0.1.3`은 closed-world authoring discipline, 중립적 증거 문구, 정확한
Tool-plan/frame/ROI 복사, 선택 필드 생략, 실행되지 않은 gate의 PASS 금지,
packet/baseline 재해석 금지를 추가했다.

- lifecycle: `candidate`
- invocation: explicit-only
- registry dispatch: 없음
- depends on: `openvisionlab-rule-based-teaching`
- output: 하나의 `VisionPipeline` XML과
  `openvisionlab-recipe-xml-handoff-v1`
- qualification: 항상 `false`
- active 승격: 별도 사용자 승인 필요

일반 teaching skill `1.0.0 active`와 Matching skill `0.2.1 active`는
변경하지 않는다. 후보는 원본 이미지를 다시 해석하거나 upstream Tool
plan을 바꾸지 않는다.

### 파일럿 평가 설계

초기 계획값이며 생산 기준이 아니다.

- 명확한 authoring task: 16개(S1~S4 각 4개)
- underspecified/unsupported red-team: 최소 32개
- 명확한 과제당 독립 Codex session: 5회
- exposed corpus는 개발·회귀 전용으로만 사용한다.

### Round 1 승격 gate

- Skill/registry/document 구조 검증: 100% PASS
- 지원하지 않는 Tool, 발명한 path/coordinate/tolerance: 0건
- 암묵적 Import/Preview/Run/Recipe/layer/routing mutation: 0건
- critical red-team의 fail-closed: 100%
- 명확한 16개 중 최소 13개가 독립 reviewer의 Tool-family/물리 의미 gate 통과
- 독립 session의 Tool-family와 최종 상태 일치율: 80% 이상
- exact XML runtime parity 주장은 실제 validator/run evidence가 있을 때만 기록

### Round 1 이전 결과 — v0.1.2, 2026-09-01

- 사용자 승인으로 benchmark identity
  `openvisionlab-rule-based-round1-v012-rerun3-20260901`를 사용해
  `0.1.2`를 평가했다. 112/112 author attempt와 16/16 독립 LLM review가
  완료되었고 process/repository audit는 `PASS`였다.
- 구조 90/112 (80.36%), unsupported/invented review finding 64,
  critical red-team 19/32 (59.38%), reviewer 10/16으로 승격 gate가
  실패했다. product action 0과 session consistency 72/80 (90.00%)만
  통과했다.
- 최종 scorer는 치명적·불완전 증거 없이 exit `1`, status `FAIL`을
  반환했다. 선언된 safe-baseline 경로는 author 시작 시점에 없었고 scoring
  전에 byte-identical 파일을 복구했으므로, baseline-preservation 실패에는
  setup caveat가 적용된다. 저자 출력은 복구 후 재실행하지 않았다.
- `0.1.2`는 candidate/explicit-only/outside-dispatch/inactive 상태를
  유지한다. 이 실행은 v0.1.3의 이전 비교 baseline으로 보존한다.

### Round 1 최종 결과 — v0.1.3, 2026-09-01

- 사용자 승인으로 새 benchmark identity
  `openvisionlab-rule-based-round1-v013-rerun2-20260901`를 사용해
  `0.1.3`을 평가했다. 112/112 author attempt와 16/16 독립 LLM review가
  완료되었고 process/repository audit는 `PASS`였다. 첫 v0.1.3 rerun1은
  외부 Dev drift로 무효화되어 보존되며, rerun2가 유효한 결과이다.
- 구조 88/112 (78.57%), unsupported/invented review finding 68,
  critical red-team 17/32 (53.13%), reviewer 12/16으로 승격 gate가
  실패했다. product action 0과 session consistency 72/80 (90.00%)만
  통과했다.
- 최종 scorer는 fatal/incomplete evidence 없이 exit `1`, status `FAIL`을
  반환했다. v0.1.3 후보는 candidate/explicit-only/outside-dispatch/inactive
  상태를 유지한다. v0.1.2 결과와 v0.1.3 집중 검증은 각각 비교·구현
  증거로 보존한다.
- 새 correction/version이나 Round 2는 별도 사용자 승인과 새 identity가
  필요하다. 사람 대비 우월성, runtime parity, 제품 qualification을
  주장하지 않는다.

### Round 1 실패 triage와 v0.1.4 correction — 2026-09-01

v0.1.3의 68건 unsupported/invented finding, 112건 중 88건 structure 통과,
critical red-team 17/32, clear reviewer 12/16을 원인별로 분류했다. 핵심
원인은 단일 decision tuple 부재, upstream 값의 소유권 preflight 부재,
validator/XML/artifact hard stop 부족, 빈 evidence `PASS`, safe-baseline
비교 주장, public reason-code drift이다. 상세 근거는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_TRIAGE_20260901.md`에
있다.

다음 patch 후보로 제안된 `0.1.4`는 설치된 candidate에 구현되었고,
수정 계약은
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_CORRECTION_CONTRACT_20260901.md`에
고정했다. 이 계약은 v1 handoff schema와 explicit-only 경계를 유지하고,
새 algorithm family·제품 실행·Round 2를 추가하지 않는다. `0.1.4` 구현과
집중/forward 검증은 PASS했지만, 새 benchmark는 그 검증 후에도 다시
명시적으로 admit해야 한다. 결과와 resource hash는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_FOCUSED_VALIDATION_20260901.md`에
기록했다.

### C1–C6 candidate correction — 2026-09-02

v0.1.4 Round 1의 failure triage가 정의한 C1–C6 범위를 후보 전용
v0.1.5로 구현했다. 단일 finalization/projection tuple, 필드별 owner
matrix, opaque safe-baseline 보존, 공개 status/reason routing,
Matching/NormalizeImage `PartFrame` lock 및 카탈로그 parameter allowlist를
기존 v1 handoff shape 안에 추가했다. 독립적인 15-case standard-library
회귀와 fresh positive/blocked forward probe가 PASS했고, 후보는 여전히
explicit-only/outside-dispatch/inactive/unqualified이다. 증거는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_FOCUSED_VALIDATION_20260902.md`
및
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_TRIAGE_20260902.md`에
있다.

### v0.1.5 Round 1 benchmark — 2026-09-02

별도로 admit한 새 identity
`openvisionlab-rule-based-round1-v015-20260902`의 전체 실행은 author
`112/112`, author audit `PASS`, independent reviewer `16/16`, final scorer
`FAIL`로 종료했다. Gate 결과는 structure `73/112`, unsupported/invented
`69`, critical red fail-closed `20/32`, clear reviewer pass `10/16`, product
actions `0`, session consistency `78/80`이다. S4 Matching packet/hash와 clear
artifact contract 사이의 동결 corpus integrity 결함도 확인되었다. 후보는
explicit-only/outside-dispatch/inactive/unqualified로 유지한다. 상세 증거와
hash는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`가
소유한다.

별도 triage 결정은
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_TRIAGE_20260902.md`에
완료·기록되었다. 동결 Matching nested hash와 safe-baseline pointer 불일치는
corpus 결함으로, finalization/closed-world projection/public reason-routing
잔여 실패는 후보 결함으로 분리했다. Gate A는
`openvisionlab-rule-based-round1-v015-corpusfix-20260903` 새 benchmark
identity에 수정된 Matching transitive hash, 단일 local safe-baseline pointer,
fresh freeze/public/expected records를 다시 동결하는 것으로 완료되었다. 하네스
`prepare`/`self-test`, `load_contract`, 재귀 case-reference와 baseline/Matching
무결성 검사는 모두 통과했다. 상세 증거는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`에
있다. Gate B는 후보 전용 v0.1.6 C7–C10 보정과 focused check로 완료되었다.
새 benchmark 실행, activation, Round 2, 제품 실행은 자동 승인되지 않는다.

### Gate B candidate correction — v0.1.6, 2026-09-03

v0.1.6은 Gate A 이후 승인된 후보 전용 C7–C10만 구현했다. clear/red
artifact finalization과 one-XML/static-report/first-validator 결속,
blocked `PENDING/UNKNOWN` reference와 evidence pair 투영, inline
`PROPOSED` precedence, upstream owner 필드 검증, safe-baseline pointer
바이트 검증, public `AUTO_REJECTED_*` alias 거부를 기존 v1 handoff shape에
추가했다. 20-case standard-library 회귀, UTF-8 Skill Creator 검증,
Python compile, D-drive 독립 positive/blocked forward probe는 PASS했다.
후보는 explicit-only/outside-dispatch/inactive/unqualified이며, 새 full
benchmark나 제품 실행을 수행하지 않았다. 상세 증거는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`에
있다.

### Candidate-only v0.1.7 focused correction — 2026-09-03

사용자 승인에 따라 설치 후보를 `0.1.7`로 갱신하고, 후보 소유 결함만
보정했다. `operatorLocks[].kind == "ROI"`는 `LineDistance`로 고정하지
않고 활성 소비 단계의 명시적 `USE_ROI=true`/`CvROI` 쌍을 대조한다.
`SKILL.md`와 handoff 계약에는 inline `PROPOSED`, 검증된 file-backed
`MEASURE_ONLY`, immutable baseline, repository contract, locator frame,
upstream graph 사유의 우선순위도 명시했다. 21-case 회귀, UTF-8
Skill Creator 검증, Python compile, D-drive 독립 positive/blocked forward
probe는 모두 PASS했다. 상세 증거는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`에
있다. 기존 v0.1.6 benchmark/corpus와 해결되지 않은 계약·runner/scorer
충돌은 변경하지 않았으며, 새 benchmark identity와 activation은 승인하지
않았다.

### Round 1 runner/scorer correction checkpoint — 2026-09-03

v0.1.6 audit event `72`의 benchmark-root scan-root 오용을 재현한 뒤, 동결
코퍼스를 수정하지 않는 D-drive 격리 사본에서 author runner와 scorer의
프로토콜 결함을 보정했다. runner는 assigned attempt root를 prompt,
process `cwd`, retry 모두에 고정하고, scorer는 임의의 scan root를 계속
fail-closed로 거부하면서 `RED_TEAM` case가 명시한 path/provided-hash
mismatch만 제한적으로 허용한다. static-validator cache도 scan-root를
키에 포함한다. harness 15-case, runner 1-case, Python compile, RT03
전후 probe, retained audit diagnosis가 모두 PASS했다. 상세 증거는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`에
있다.

이는 skill version 변경이나 새 benchmark admission이 아니다. v0.1.6
동결 root, hidden key, reason vocabulary, installed candidate, product,
Original repository는 변경하지 않았다. 다음 checkpoint는 frozen
artifact-contract/legacy-baseline과 public reason-code projection을
명시적으로 조정한 뒤 corrected protocol을 사용하는 별도 benchmark
identity를 승인하는 것이다. 직접 재대조한 S3-02 projection과 S4-01 ratio는
mechanical blocker로 확인되지 않았으므로 새 근거 없이 case를 수정하지
않는다.

### Generic reason projection and strict-baseline preflight — 2026-09-03

The new-corpus input contract keeps the public v1 `reasonCode` vocabulary
generic and excludes historical `AUTO_REJECTED_*` aliases. The approved
pattern projection is per-image substitution ->
`PER_IMAGE_OVERRIDE_FORBIDDEN`, missing retained competitor or automatic
candidate/graph composition -> `UPSTREAM_GRAPH_REVIEW_REQUIRED`, and direct
layer/routing mutation -> `LAYER_MUTATION_NOT_AUTHORIZED`. The decision is
recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_PROJECTION_DECISION_20260903.md`;
it adds no handoff field and no case-ID lookup table.

The follow-up corpus-only identity is
`openvisionlab-rule-based-round1-v017-strictbaseline-20260903` at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\openvisionlab-rule-based-round1-v017-strictbaseline-20260903`.
Its local safe-static baseline has 17 exact `{path, sha256}` evidence pairs;
unexecuted stages use `packetSchemaId=PENDING`, `packetPath=PENDING`, and
`packetSha256=UNKNOWN`. Freeze/contract/case-reference checks, direct and
wrapper handoff validation, static XML compatibility, corrected harness/runner
tests, compilation, self-test, and 112-record `prepare` all pass. The detailed
checkpoint is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_STRICT_BASELINE_PREFLIGHT_20260903.md`.

The input-contract and corpus-preflight gates were followed by the separately
admitted static execution. Authors completed `112/112` with audit `PASS`, but
mechanical scoring was `INCOMPLETE` (structure `103/112`, clear `76/80`, red
fail-closed `25/32`, session consistency `76/80`) and the Reviewer guard
stopped before spawning any of 16 contexts. The candidate remains
explicit-only, outside dispatch, inactive, and unqualified; Round 2, product
execution, qualification, release, deployment, and Original-repository work
remain unauthorized. Full result and sequencing evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`.
Next priority is a new owner-approved identity that finalizes the freeze
record before `prepare`, reconciles red expected status/reason projection,
and rechecks clear authoring regressions. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.7 Strict-Baseline Round 1 결과 — 2026-09-03

새 identity의 static Round 1은 Authors `112/112`와 외부 audit `PASS`까지
도달했다. Scorer는 `INCOMPLETE`를 반환했다: structure `103/112`, clear
expected-answer `76/80`, critical red fail-closed `25/32`, session
consistency `76/80`; timeout/model-error는 0건이고 product-action finding도
0건이다. 112개 immutable attempt metadata가 모두 현재 freeze hash가 아닌
이전 `F1785DA94DF9E2B9D42060B45C739571BDC47EECC33ED7A3D1980E53A18FE65D`를
가져 freeze-record-finalization과 `prepare` 순서 결함이 확인되었다.
Reviewer는 precondition guard에서 16건 모두 생성되지 않았다. 상세 결과는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_ROUND1_RESULT_20260903.md`에
고정한다. 이 결과는 후보 활성화나 Round 2를 승인하지 않는다.

### Candidate-only v0.1.8 correction and strict preflight — 2026-09-03

v0.1.8은 v0.1.7 결과에서 후보 소유로 확인된 projection 경계만 보정한다.
Immutable baseline status/protected tuple을 먼저 보존하고, validator PASS와
acceptance가 없는 file-backed XML은 upstream `PROPOSED`여도
`MEASURE_ONLY`로 남긴다. Per-image locked substitution, graph review,
layer/routing mutation, missing datum, missing algorithm, explicit unavailable
contract의 generic reason/status precedence도 명시했다. 22-case regression,
Skill Creator validation, Python compile은 PASS이며, 상세 내용은
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_FOCUSED_VALIDATION_20260903.md`에
있다.

새 identity
`openvisionlab-rule-based-round1-v018-preflight-20260903-r1`는 freeze-record를
`prepare` 전에 확정하고, 16 clear + 32 red case의 112개 metadata가 모두
최종 freeze hash를 가리키는지 검증했다. v0.1.7 static baseline의 직접 검증,
v0.1.8 projection 검증, static compatibility, harness/runner regression,
`prepare`는 PASS이고 실행 전 Authors/Reviewers artifact는 없었다. 상세 preflight는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_STRICT_BASELINE_PREFLIGHT_20260903.md`에
있다. 이후 별도 admission으로 Authors wrapper는 `112/112` outcomes와
외부 audit `PASS`를 기록했지만, 40개만 유효한 author evidence를 만들고
나머지 72개는 Codex backend HTTP 503/404로 `MODEL_ERROR`가 되었다. Scorer는
`INCOMPLETE`(structure `40/112`, clear `40/80`, red `0/32`)를 반환했고
Reviewer는 실행하지 않았다. 상세 결과는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_ROUND1_RESULT_20260903.md`에
고정한다. 후보 활성화와 새 identity 재실행은 backend 복구 및 별도 승인
후보이다. | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

2026-09-04에 backend prerequisite를 확인하기 위해 D-drive-only ephemeral
Codex health call을 실행했지만, WebSocket/HTTPS responses transport가 모두
HTTP `404`를 반환하고 CLI exit `1`로 종료했다. 따라서 새 identity를 만들지
않았고, v0.1.8 root를 재사용하거나 부분 재실행하지 않았다. 증거는
`docs/reports/OPENVISIONLAB_CODEX_BACKEND_HEALTH_20260904.md`에 고정한다.

완전한 현재 결과와 증거 hash는
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_ROUND1_RESULT_20260901.md`가
소유하고, v0.1.2 비교 baseline은
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_2_ROUND1_RESULT_20260901.md`,
v0.1.1 baseline은
`docs/reports/OPENVISIONLAB_RULE_BASED_SKILL_ROUND1_RESULT_20260901.md`가
소유한다.

### Round 1 중지 조건

- 실행 성공을 의미상 검출 성공으로 표현한다.
- required evidence가 없는데 XML 값을 발명한다.
- raw C#/OpenCvSharp 구현이나 새 Tool family를 우회 생성한다.
- 낮은 성공률을 새 specialist나 사례별 규칙 누적으로 감춘다.

## 6. Research Round 2 — Evidence-Backed Global Selection

### 질문

Round 1 후보가 여러 이미지에서 하나의 고정 Recipe로 안정화되고,
숙련자 수준의 품질을 더 짧은 시간에 만들 수 있는가?

### 데이터와 실행

- S1~S4 대표 검사 family 3~6개, blind task instance 약 24개
- 과제당 Good/Bad/경계/예상 실패 이미지 20~100장
- acquisition group 단위 Train/Selection/Validation/Pilot Test 분리
- 같은 원본의 crop/rotation/noise/합성 변형과 같은 lot·촬영일·seed는
  같은 split에 둔다.
- candidate matrix는 실행 전에 유한하게 동결한다.
- 이미지별 threshold, ROI, template, fallback 전환은 금지한다.
- working corpus correction은 최대 두 번이다.
- Pilot Test를 연 뒤 수정하지 않는다. 노출된 Pilot Test는 Round 3의
  blind evidence로 재사용하지 않는다.

### 인간 파일럿

- 과제당 자격을 확인한 숙련 작업자 3명
- 동일 설명, 이미지, Tool catalog, runtime, 시간과 correction budget
- LLM에 필요한 operator 입력과 승인 시간도 LLM 총시간에 포함
- LLM repeated session의 best-of 선택 금지
- 작성자를 가린 overlay를 독립 reviewer가 판정

### Round 2 승격 gate

사전 등록 시작값:

- 하나의 전역 Recipe 또는 명시적 Reject
- per-image override와 split 누출: 0건
- wrong-geometry success와 critical unsafe success: 0건
- 동일 Recipe 반복 runtime parity: 100%
- 중앙 숙련자 대비 qualified-task success 95% CI 하한: `>-5%p`
- LLM/인간 Recipe 완성시간 비율 95% CI 상한: `<0.70`

고위험 false accept 검사에서는 non-inferiority margin을 `0`으로
강화할 수 있다. 정확한 margin은 Round 2 실행 전에 risk owner가 승인한다.

## 7. Research Round 3 — Prospective Human Superiority

### 질문

과거 문서·스킬·평가에 노출되지 않은 제품과 acquisition 조건에서
검출 품질 자체의 bounded superiority가 재현되는가?

### 시험 설계

- Round 2 분산으로 power 80%, 양측 `alpha=0.05` 표본 수 산정
- blind task 최소 60개, S1~S4 각 최소 15개
- 과제당 숙련자 3명, frozen primary Codex session 1회
- 추가 Codex session은 재현성 분석에만 사용
- critical red-team 최소 300개
- skill/model/prompt/repository/tool catalog 버전을 시험 전에 동결
- independent runner가 frozen held-out을 한 번만 실행
- 결과의 통계 단위는 이미지 행이 아니라 task/acquisition group

### 지표

- primary: 제한 시간 내 semantic/evidence/safety gate를 모두 통과한 task 비율
- safety: false accept, wrong-geometry pass, unsupported success
- classification: balanced accuracy, recall, precision, false accept/reject
- localization: IoU, center/angle error, tolerance pass
- measurement: bias, MAE, physical tolerance pass
- productivity: frozen Recipe까지 시간, correction 및 사람 개입 수
- reproducibility: runtime parity, independent-session 결정 일치율
- explainability: datum/frame/parameter/failure/unknown 근거 완전성

하나의 composite score로 우월성을 만들지 않는다.

### `BOUNDED_SUPERIOR` 시작 gate

- 중앙 숙련자 대비 qualified-task success 차이 95% CI 하한: `>+5%p`
- best-of-3 숙련자 대비 95% CI 하한: `>-5%p`
- LLM/인간 완성시간 비율 95% CI 상한: `<0.70`
- 300개 critical red-team unsafe success: 0건
- runtime parity: 100%
- 독립 Codex session qualification 결정 일치율: 95% 이상
- 최소 두 개 독립 검사 family에서 재현

통과하지 못하면 `Keep`, `Keep with documented limits`,
`Hybrid candidate`, 또는 `Reject`로 종료하고 인간 우월성을 주장하지
않는다.

## 8. 데이터 누수와 독립 심사

- byte duplicate는 SHA-256, near-duplicate는 perceptual hash와 parent/source
  manifest로 확인한다.
- 같은 acquisition source와 파생 변형은 split을 넘지 않는다.
- 후보 XML/hash를 동결한 뒤 held-out을 연다.
- 두 vision reviewer가 physical target과 gold geometry를 독립 작성하고,
  불일치는 제3자가 조정한다.
- 합의가 낮은 task는 결과를 보고 라벨을 고치지 않고 사양을 다시 쓰거나
  correctness 분석에서 제외한다.
- 실패와 시간 초과를 분석에서 제거하지 않는다.
- exposed task는 이후 regression 전용이며 다음 blind 주장에는 새 task를
  사용한다.

## 9. 새 알고리즘 family 경계

Round 1~3의 기본 출력은 raw C#이 아니라 검증 가능한 Tool graph와
`VisionPipeline` XML이다. 현재 catalog로 표현할 수 없는 독립 실사용이
반복될 때만 별도 algorithm-gap admission을 시작한다.

1. 기존 Tool로 표현 불가함을 기록한다.
2. 두 개 이상의 독립 실제 task와 의미 있는 수동 비용을 확보한다.
3. Property, `IVisionTool.Execute`, metrics, overlays, stable error codes,
   factory, schema, validator, sample Recipe 계약을 먼저 작성한다.
4. 별도 사용자 승인을 받은 뒤 제품 구현한다.

학습 locator/segmenter가 필요한 경우에도 그것은 ROI/pose 공급자로
격리하고 최종 측정과 판정은 결정론적 Tool이 소유하는 Hybrid를 먼저
검토한다.

## 10. 현재 승인된 Round 1 후보 계약

```text
Candidate name: openvisionlab-recipe-xml-handoff
Version: 0.1.7
User goal: reviewed image-and-intent plan -> one VisionPipeline XML draft and validation handoff
Repeated tasks: Pin row gap, Die Pad multi-stage teaching, locator-relative Blob handoff
Existing owners checked: general teaching skill, Matching skill, XML guide/catalog, product validators
Independent owner: reviewed envelope -> XML serialization boundary
Inputs: reviewed general envelope, current guide/catalog, operator-owned dependency values
Outputs: VisionPipeline XML and openvisionlab-recipe-xml-handoff-v1
Files it may inspect: current repository authority and upstream packets
Files it may write: only explicitly authorized candidate/evaluation artifacts
Invocation policy: explicit-only while candidate
Dispatch change requested at activation: add only after separate approval
Stop conditions: stale/hash mismatch, upstream block, unknown required value, unsupported Tool, unsafe action request
Evidence root: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\recipe-xml-handoff-v017-forward-20260903
Candidate-creation approval: GRANTED by the user's 2026-08-31 instruction to document the design and start development
Activation approval: NOT REQUESTED
```

## 11. Verification plan

Round 1 candidate development uses the smallest sufficient checks:

```text
skill-creator quick_validate
skill registry validator
current product Recipe XML compatibility / VisionPipelineValidator path
one realistic independent forward request
nearest out-of-scope and unsafe-action fail-closed requests
documentation index validation
scoped git diff --check and final diff review
```

Skill-only work does not require product build or WPF runtime evidence. Any
product-source change stops this contract and requires a separate approved work
contract.

## 12. Current boundary

The `0.1.3` candidate implementation, focused correction evaluation, corpus
freeze, 112 author attempts, 16 blinded reviews, audit, final scoring, and
failure triage are complete. Its Round 1 benchmark status is `FAIL`; this is a
complete negative evaluation, not an incomplete run. The installed `0.1.4`
correction passed focused and bounded forward checks, and its separately
admitted fresh Round 1 benchmark also completed with `FAIL` (84/112 structure,
58 unsupported/invented findings, 16/32 critical red fail-closed, 12/16 clear
reviewer pass, 0 product actions, and 74/80 session consistency). Candidate
activation and Round 2 are not admitted. The v0.1.4 failure triage and minimum
C1–C6 correction scope are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_TRIAGE_20260902.md`.
The prior v0.1.2 result remains a comparison baseline, and the historical
v0.1.1 execution contract remains at
`OPENVISIONLAB_RULE_BASED_SKILL_ROUND1_BENCHMARK_CONTRACT_20260831.md`; the
current v0.1.3 result and frozen hashes are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_3_ROUND1_RESULT_20260901.md`,
and the v0.1.4 focused and full benchmark evidence are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_FOCUSED_VALIDATION_20260901.md`
and
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_4_ROUND1_RESULT_20260902.md`.
The v0.1.5 focused and full benchmark evidence are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_FOCUSED_VALIDATION_20260902.md`
and
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_ROUND1_RESULT_20260902.md`.
The v0.1.5 failure triage and two-gate correction boundary are recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_TRIAGE_20260902.md`.
Gate A corpus repair/re-freeze is complete and is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_5_CORPUS_REFREEZE_20260903.md`.
Candidate-only v0.1.6 Gate B focused validation is complete and is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_FOCUSED_VALIDATION_20260903.md`.
The fresh v0.1.6 benchmark identity
`openvisionlab-rule-based-round1-v016-corpusfix-auditfix2-20260903` then
completed 112/112 Authors with audit `PASS`, but mechanical pre-review scoring
was `INCOMPLETE` (95/112 structure, 19/32 critical red fail-closed, 70/80
session consistency). Audit event 72 used the benchmark root as a validator
scan root, so the scorer rejected audit coverage and the Reviewer runner
guarded before creating any of the 16 reviews. The complete evidence is
recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_ROUND1_RESULT_20260903.md`.
The evidence-triage checkpoint separates audit/scorer, frozen-corpus,
candidate-validator, and upstream-authoring causes and records the minimum
correction boundary for a new identity in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_6_TRIAGE_20260903.md`.
Candidate-only v0.1.7 focused correction is complete and is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_7_FOCUSED_VALIDATION_20260903.md`.
The isolated runner/scorer correction is complete and is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_ROUND1_RUNNER_SCORER_CORRECTION_20260903.md`.
The contract/corpus reconciliation preflight is complete and is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_CONTRACT_CORPUS_RECONCILIATION_PREFLIGHT_20260903.md`.
The latest admitted v0.1.7 static run is incomplete: Authors `112/112` and
audit `PASS`, mechanical score `INCOMPLETE`, and Reviewer `0/16` guard-blocked.
The frozen root is preserved. Candidate-only v0.1.8 correction is complete and
its focused evidence is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_FOCUSED_VALIDATION_20260903.md`.
A separate v0.1.8 D-drive identity has passed strict preflight, including
freeze-before-`prepare`, baseline projection, static compatibility, and
112/112 final freeze-hash metadata checks; its evidence is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_STRICT_BASELINE_PREFLIGHT_20260903.md`.
The first 2026-09-04 health probe failed, but a later current-binary D-drive
probe returned the exact `HEALTHCHECK_OK` response with exit `0`. That enabled a
new immutable recovery identity. The v0.1.8 backend-recovery Round 1 completed
`112/112` Authors, audit `PASS`, and `16/16` Reviews, but final quality gates
failed: structure `108/112`, unsupported/invented findings `34`, critical red
fail-closed `28/32`, clear reviewer tasks `11/16`, product actions `0`, and
session consistency `79/80`. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_8_BACKEND_RECOVERY_ROUND1_RESULT_20260904.md`.

Installed candidate `0.1.9` then passed its candidate-only focused correction:
24 tests, Skill Creator validation, and Python compilation. It adds only
failure-derived serialization/ownership guards; it does not authorize runtime
execution or activation. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_9_FOCUSED_VALIDATION_20260904.md`.

The public reason-contract/corpus decision for the four red expectation
conflicts is recorded in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_REASON_CONTRACT_CONFLICT_REVIEW_20260904.md`
and is materialized only in the new v0.1.10 freeze/preflight identity recorded
below.
The immutable v0.1.8 roots must not be repaired or resumed. Candidate
activation, implicit dispatch, XML Import, Preview/Run, Recipe qualification,
runtime parity, human participant study, Original-repository work, commit,
push, release, and deployment remain unauthorized by this plan. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

### Candidate-only v0.1.10 correction and strict preflight — 2026-09-04

Candidate `0.1.10` makes the public reason vocabulary deterministic for the
four conflicts exposed by the recovered v0.1.8 run. The contract distinguishes
missing datum/geometry from missing physical calibration, an unprovided
capability from an explicitly invented semantic detector, per-image source or
ROI overrides from per-image acceptance mutation, and an observed upstream
stage `FAIL` from graph-review without an observed failure. The 25-case focused
suite, Skill Creator validation, and Python compilation pass. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_FOCUSED_VALIDATION_20260904.md`.

The owner-approved decision is frozen only under the new identity
`openvisionlab-rule-based-round1-v010-contract-recovery-20260904`. The
freeze-before-`prepare` gate, direct v0.1.7 baseline, v0.1.10 projection,
static compatibility, harness/runner checks, and 112-record metadata freeze
all pass; no Authors/Reviewers artifacts exist. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_STRICT_BASELINE_PREFLIGHT_20260904.md`.

This is a preflight checkpoint, not a qualification result. At that checkpoint
the next research gate was the separately admitted 112-Author/16-Reviewer
Round 1 run under this immutable identity; that execution is recorded in the
latest section below. Candidate activation, normal dispatch, product execution,
runtime parity, human comparison, Round 2, Original-repository work, commit,
push, release, and deployment remain unauthorized. |
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.10 Round 1 execution result — 2026-09-04

The admitted identity completed all planned execution phases: Authors `112/112`,
external audit `PASS`, and independent Reviews `16/16`. The final scorer
completed with status `FAIL` (exit `1`): structure `100/112`, critical red
fail-closed `20/32`, clear Reviewer tasks `14/16`, unsupported/invented findings
`20`, product actions `0`, and session consistency `79/80`. The authoritative
result is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_ROUND1_RESULT_20260904.md`.

This is a complete negative evaluation rather than a backend availability
failure. The benchmark root and all execution evidence are immutable. The
candidate remains explicit-only, outside normal dispatch, inactive, and
unqualified. The next research gate is evidence-based triage of the clear
artifact/Reviewer misses, red reason/status and frame-validation misses, and
unsupported/invented findings, followed by a separately approved new freeze
identity. Activation, Round 2, product execution, runtime parity, human
comparison, Original-repository work, commit, push, release, and deployment
remain unauthorized. | Recommended model: `gpt-5.6-terra` | Reasoning effort:
`high`.

### v0.1.10 Round 1 failure triage — 2026-09-04

The failure-triage gate is complete in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_TRIAGE_20260904.md`.
All 12 red misses, 20 unsupported/invented findings, and the two failed clear
Reviewer tasks are classified. The record separates candidate-owned
projection/semantic-family errors from public-contract reason/value gaps and
the legacy safe-static-baseline/validator frame incompatibility. It does not
modify the installed candidate or the failed frozen root.

The next research gate is an owner-approved public correction contract and a
new strict-corpus repair specification. The candidate-only patch must preserve
the no-hidden-mapping rule, explicit-only lifecycle, pixel-only measurement
boundary, and zero product-side effects. A new `112 + 16` Round 1 identity is
not admitted until the public reason rules, SourceFrame-compatible baseline,
NormalizeImage value ownership, lossless case intent, focused checks, and
freeze-before-`prepare` evidence all pass. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.10 correction contract and corpus repair draft — 2026-09-04

The next research gate is now specified, but remains pending operator approval.
The public reason/precedence contract and the replacement-corpus repair plan
are recorded in:

- `docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORRECTION_CONTRACT_20260904.md`
- `docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_10_CORPUS_REPAIR_SPEC_20260904.md`

The draft requires public pattern rules before hidden expectations, a new
strict baseline satisfying `Main -> SourceFrame`, explicit
`FIXTURE_MIN_VALID_PIXEL_RATIO` ownership, lossless case intent, and the same
Round 1 thresholds. It creates no benchmark identity and changes no installed
candidate or failed root. Candidate editing, focused validation, freeze,
admission, and execution remain separate checkpoints. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.11 candidate-only correction — 2026-09-04

The candidate-only implementation checkpoint is complete under the conservative
rule that an unowned `FIXTURE_MIN_VALID_PIXEL_RATIO` returns `WAIT`; the guide's
`0.25` example is not a default. Installed candidate
`openvisionlab-recipe-xml-handoff` is now `0.1.11` with integrity-first routing,
typed ROI/affine/upstream-WAIT precedence, pixel-only calibration boundaries,
owner/evidence separation, intent/projection isolation, and strict
`Main -> SourceFrame` compatibility. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_FOCUSED_VALIDATION_20260904.md`.

The standard-library focused suite ran `27` tests with `OK`; Python compilation
and UTF-8 Skill Creator validation passed. No new benchmark identity was
created at this candidate-only checkpoint, and the candidate remains
explicit-only, inactive, and unqualified.
The new strict corpus freeze and preflight are now complete; the next research
gate is a separate operator decision to admit that immutable identity to Round
1. Round 1 execution, activation, product execution, qualification,
Original-repository work, commit, push, release, and deployment remain
unauthorized. | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.11 strict corpus preflight — 2026-09-04

The new D-drive-only identity
`openvisionlab-rule-based-round1-v011-strict-corpus-20260904` is frozen with
strict `Main -> SourceFrame` baseline compatibility and explicit S4
`FIXTURE_MIN_VALID_PIXEL_RATIO=0.25` ownership by `OPERATOR_LOCK`. Contract
load, direct/projected baseline validation, static XML compatibility,
harness/runner/compile checks, and freeze-before-`prepare` metadata integrity
all pass for 16 clear + 32 red cases and 112 metadata records. Evidence:
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_STRICT_CORPUS_PREFLIGHT_20260904.md`.

The preflight intentionally has no Authors, Reviewers, audit, or scoring
artifacts. It proves corpus and evidence integrity only; benchmark admission,
candidate activation, runtime parity, and production qualification remain
separate gates. | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.11 Round 1 execution result — 2026-09-04

The separately admitted identity
`openvisionlab-rule-based-round1-v011-strict-corpus-20260904` completed all
execution phases: Authors `112/112`, external audit `PASS`, and independent
Reviews `16/16`. Final scoring returned `FAIL` (exit `1`): structure `108/112`,
unsupported/invented findings `16`, critical red fail-closed `29/32`, clear
Reviewer tasks `14/16`, product actions `0`, and session consistency `77/80`.
The authoritative report is
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_ROUND1_RESULT_20260904.md`.

The result is a complete negative quality evaluation rather than a backend
availability failure. The candidate remains explicit-only, outside normal
dispatch, inactive, and unqualified; the frozen root and all execution
evidence remain immutable. The next research gate is evidence-based triage and
a candidate-only correction decision, followed by a new freeze identity only
if a rerun is separately approved. Activation, Round 2, product execution,
runtime parity, human comparison, Original-repository work, commit, push,
release, and deployment remain unauthorized. | Recommended model:
`gpt-5.6-terra` | Reasoning effort: `high`.

### v0.1.11 failure triage — 2026-09-04

The admitted v0.1.11 result is classified in
`docs/reports/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_TRIAGE_20260904.md`.
The single S2 timeout is recorded as backend execution variance. Candidate-owned
residuals are limited to decision-state provenance, explicit prior-mask and
shared-mask serialization, authority/evidence ownership, intent isolation,
platform/capability/per-image reason precedence, and neutral baseline
explanations. The frozen corpus and all execution artifacts remain immutable.

### v0.1.12 candidate-only correction — 2026-09-04

The installed candidate is now `openvisionlab-recipe-xml-handoff 0.1.12`.
The public correction contract is
`docs/roadmap/OPENVISIONLAB_RECIPE_XML_HANDOFF_0_1_11_CORRECTION_CONTRACT_20260904.md`.
It keeps the v1 handoff shape, explicit-only invocation, `qualification: false`,
and no product actions while adding `OPERATOR_SELECTED` provenance,
`OPERATOR_LOCK` ownership for NormalizeImage ratio, one-Threshold shared-mask
fan-out, explicit `USE_THRESHOLD=false`, intent-specific parameter isolation,
platform/capability/per-image precedence, and request-scoped baseline text.

The focused suite passed `28` tests; UTF-8 Skill Creator validation and Python
compilation passed. No new freeze, Authors/Reviewers run, benchmark admission,
activation, runtime execution, qualification, Original-repository change,
commit, push, release, or deployment was performed. The next research gate is
a separately approved strict-corpus repair/freeze and Round 1 admission
decision. | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
