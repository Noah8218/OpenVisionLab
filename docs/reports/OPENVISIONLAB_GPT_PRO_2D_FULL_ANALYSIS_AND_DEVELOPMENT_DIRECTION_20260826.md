# OpenVisionLab GPT Pro 2D 전면 분석 조정 및 개발 방향

Date: 2026-08-26 KST

Repository: `C:\Git\OpenVisionLab_Dev`

Reviewed Dev HEAD: `8e8311395d91ca449797946c7003de2d846cdd2f`

Input SHA-256: `8F6F44A44B0F9EAF988E71E386C54E1B0BA9CA3A48F5BEBA305F102D57220A8A`

Status: Analysis and planning complete; product implementation not started

## 1. 작업 계약

### 사용자 목표

- 사용자가 제공한 GPT Pro의 OpenVisionLab 2D 전면 분석을 빠짐없이 검토한다.
- 현재 프로젝트의 공식 방향, 현재 상태, 코드와 증거에 맞춰 수용 여부를 다시 판단한다.
- 부족한 점을 방어적으로 축소하지 않고 적극적으로 인정하되, 검증되지 않은 제안을 사실이나 활성 우선순위로 바꾸지 않는다.
- 최종적으로 무엇을 개발할 것인지 한 문장과 단계별 계획으로 정리한다.
- 코드 개발보다 먼저 분석 결과와 개발 방향 전체를 사용자에게 보여준다.

### 비타협 요구사항

- OpenVisionLab의 deterministic rule-based Recipe Workbench 정체성을 유지한다.
- PropertyGrid Tool, 명시적 Preview/Run, Layer/Route 비자동 변경, Evidence-first review 계약을 보존한다.
- 카메라, 조명, PLC/I/O, MES, 계정, HMI, 배포 제어를 2D Workbench 안으로 끌어오지 않는다.
- LLM 기능은 현재 maintenance mode라는 사실과, 새 Skill 개발에는 별도 재개 결정이 필요하다는 사실을 숨기지 않는다.
- 가상 Persona나 agent recording을 실제 초보 사용자 증거로 표현하지 않는다.
- 릴리스 검토, tag, tag push, draft, publication, deployment를 각각 별도 승인 경계로 유지한다.
- 현재 dirty 파일인 `.proofline/issues/PL-0010.json`, `.proofline/issues/PL-0011.json`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`를 덮어쓰거나 이번 작업 성과로 주장하지 않는다.

### 포함 범위

- 첨부 분석의 1장부터 27장까지의 주장과 제안 분류
- 현재 Dev 문서, 코드, issue evidence와의 조정
- 제품 정체성, 성숙도, 상용 제품 교훈, 범위 제외 항목 정리
- 현재 권위 우선순위와 새 제안 로드맵의 분리
- 단계별 목표, 포함/제외 범위, 승인 조건, acceptance, verification 계획

### 제외 범위

- 애플리케이션 또는 SDK 코드 변경
- Viewer sample/facade 생성
- LLM Skill, Provider, browser automation 구현
- WPF UI 수정 또는 Runtime UI 검증
- Vision SDK 저장소, 원본 OpenVisionLab 저장소 수정
- commit, push, tag, GitHub Release, deployment
- GPT Pro가 인용한 상용 제품 기능에 대한 새로운 웹 조사

### 검증 계획

- 첨부 파일 전체 2,206줄과 SHA-256 확인
- `git status --short`, `git log --oneline -5`, 현재 Dev HEAD 확인
- 공식 문서 권위 순서와 task route 확인
- 핵심 source claim을 현재 코드에서 검색
- 새 문서가 모든 원문 장을 다루는지 coverage table로 확인
- `tools/TestDocumentationIndex.ps1`, readiness, JSON parse, `git diff --check`로 문서 변경 검증

### 알려진 한계와 승인 필요사항

- 첨부 분석이 기준으로 명시한 공개 `main` commit `346f89da681fb18eeef1d93e0e6e5f7452257525`는 현재 Dev Git object database에 없어, 그 commit과 현재 Dev의 직접 diff는 확인하지 못했다.
- 현재 Dev의 권위 있는 상태는 local source, current handoff, stable contracts, issue evidence를 사용한다.
- 아래 개발안은 proposal이다. 현재 handoff의 활성 큐를 자동으로 바꾸지 않는다.
- SDK 작업에는 SDK source repository/path와 cross-repository 작업에 대한 명시적 승인이 필요하다.
- RC2 tag/release, 새 LLM Skill track, 초대형 TIFF 지원 범위는 각각 사용자 결정이 필요하다.

## 2. 먼저 보여주는 결론과 권장 개발 방향

### 2.1 최종적으로 개발하려는 것

> OpenVisionLab 2D를 장비 없이도 샘플 이미지에서 Rule 기반 검사를 직접 티칭하고, Pipeline의 모든 중간 Layer와 객체별 Accepted/Rejected 근거를 검토하고, 동일 Recipe를 Good/Bad/N-image에 재현하여 검증·보존하는 evidence-first C# Workbench로 완성한다. 그 위에만 외부 재사용 가능한 Viewer/SDK 계약과, 임의 좌표를 만들 수 없는 선택형 Evidence-constrained LLM 보조 기능을 선택적으로 올린다.

핵심은 “알고리즘을 계속 늘리는 프로그램”도, “LLM이 XML을 대신 쓰는 프로그램”도 아니다.

개발 중심은 다음 세 층이다.

1. **제품 코어**: 직접 PropertyGrid teaching -> 명시적 Preview/Run -> Pipeline Review -> Validation -> Qualified evidence.
2. **재사용 기반**: 한 번의 SDK 실행에서 완전한 Candidate evidence를 반환하고, ImageCanvas를 실제 두 번째 .NET 8 소비자가 사용할 수 있게 한다.
3. **선택적 차별화**: LLM은 좌표를 생성하지 않고 deterministic SDK가 만든 Candidate ID만 선택하며, compiler와 held-out gate가 Recipe를 검증한다.

### 2.2 GPT Pro 분석에서 적극 수용할 핵심

- OpenVisionLab의 현재 제품 정체성 정의는 정확하다.
- Pipeline Review와 Qualified Recipe 중심의 Evidence-first 설계가 가장 강한 차별점이라는 판단을 수용한다.
- Blob/Contour rejected evidence를 위한 숨은 두 번째 실행은 실제로 존재하며 SDK result contract가 해결 주체라는 판단을 수용한다.
- ImageCanvas는 내부 Library이지만 아직 독립 소비자용 facade/package contract는 아니라는 판단을 수용한다.
- Viewer와 Pipeline Review의 책임 경계는 전면 MVVM 재작성 대신 실제 변경 영역에서만 개선해야 한다는 판단을 수용한다.
- LLM에게 자유 좌표를 쓰게 하지 않고 검증 가능한 Candidate를 선택하게 해야 한다는 `NO EVIDENCE, NO COORDINATE` 원칙을 미래 재개 조건으로 수용한다.
- 실제 초보 사용자 3명 검증이 agent/Persona audit과 별개라는 판단을 수용한다.
- 상용 제품의 graph/debug/guided evidence 장점만 참고하고 장비 플랫폼 범위는 확장하지 않는다는 판단을 수용한다.

### 2.3 그대로 수용하지 않을 핵심

- “전체 Workbench 약 80%” 같은 백분율은 분모와 gate가 고정되지 않아 현재 사실로 채택하지 않는다. 현재 공식 표현은 **범위가 한정되고 근거가 있는 Release Candidate**이다.
- RC2 전체 gate는 이미 Dev와 original-main에서 통과했다. 지금 필요한 것은 같은 gate의 반복이 아니라 exact release target 조정과 tag/draft/publication별 별도 승인이다.
- 새 LLM Skill은 현재 maintenance-mode 계약과 충돌하므로 자동 P2로 활성화하지 않는다.
- Visual Pipeline Graph, Step debugger/cache, Subpipeline, Recipe status/diff, dashboard, signature를 한 번에 backlog로 활성화하지 않는다.
- ImageCanvas facade의 정확한 interface와 `Borrow/Clone/TakeOwnership` 이름을 설계 사실로 고정하지 않는다. 먼저 실제 consumer가 요구하는 최소 표면을 측정한다.
- SDK Tool Descriptor를 모든 UI/XML/manual 생성의 거대한 단일 메타시스템으로 바로 만들지 않는다. Candidate Evidence 문제를 먼저 해결하고 실제 중복 변경 비용이 확인된 항목부터 좁게 통합한다.
- Persona 3명 자동 감사용 새 framework를 먼저 만들지 않는다. 기존 walkthrough/smoke infrastructure를 재사용하고, named scenario에서만 상태 trace를 확장한다.

## 3. 현재 프로젝트 기준점

### 3.1 제품 정체성

OpenVisionLab은 OpenCvSharp4 기반 deterministic rule-based vision recipe workbench다.

정상 사용자 흐름은 다음과 같다.

```text
Sample image
  -> direct PropertyGrid teaching
  -> explicit Tool Preview
  -> Pipeline composition and layer routing
  -> explicit Run Review
  -> drawing / metric / object / layer evidence
  -> Good/Bad and N-image validation
  -> Run History
  -> saved or Qualified Recipe evidence
```

LLM XML authoring은 선택적 maintenance-mode 보조 기능이다. 계정, API key, 인터넷, generated XML은 코어 workflow의 전제가 아니다.

### 3.2 근거 기반 성숙도

- Tool Views, Pipeline, Pipeline Review, Recipe Manager, Validation Sets, Run History, public samples, drawings, saved reports는 넓게 연결되어 있다.
- 한국어/영어 offline Guide, Vision SDK 3.0 vendored identity, source-build/release gates, resource lifetime와 original/effective Pipeline provenance가 현재 근거로 존재한다.
- `v2.1.0-rc.1`은 기록상 공개 pre-release이고, 현재 source version은 `2.1.0`이다.
- Dev clean Release candidate gate와 original-main full gate는 current handoff 기준 완료됐다.
- 설치기, 서명, 자동 update/rollback, multi-PC, hardware, calibrated metrology, field robustness, commercial GA는 입증되지 않았다.

따라서 현재 성숙도 표현은 다음이 적절하다.

> Bounded evidence-backed Release Candidate. 제품 코어 연결은 강하지만, 독립 초보 사용자 증거·현장 qualification·상용 배포 체계는 미완이다.

### 3.3 상용 제품에서 유지할 교훈

- 이미지 중심의 Tool/Step/result context
- guided configuration과 명확한 next action
- 중간 결과, rejected reason, signal/distribution evidence
- fixture와 reference-coordinate 관계 가시화
- compact Recipe lifecycle과 변경 전후 근거
- Good/Bad sequence와 deterministic validation
- 명시적 operator acceptance

### 3.4 계속 범위 밖인 플랫폼 영역

- camera acquisition, lighting control
- PLC/I/O, MES, equipment sequence
- account/role/SSO/regulatory audit platform
- HMI designer와 deployment controller
- HALCON-style general scripting IDE
- autonomous AI classification과 automatic tolerance tuning
- 범용 Plugin Framework
- installer/fleet/cloud management는 별도 distribution 결정 전까지 비활성

### 3.5 현재 권위 우선순위와 제안 로드맵의 구분

현재 handoff의 권위 우선순위는 다음과 같다.

1. `PL-0010` SDK one-pass evidence는 새 SDK contract/manifest가 제공될 때까지 명시적으로 defer | Recommended model: none until SDK prerequisite exists | Reasoning effort: none until SDK prerequisite exists
2. `PL-0011`은 Dev/original gate 이후 release-stage preparation 상태이며 exact target, tag, draft, publication 승인을 각각 기다림 | Recommended model: `gpt-5.6-luna` | Reasoning effort: high
3. `CVR-00`은 독립 초보 사용자 3명과 원시 관찰이 생길 때까지 defer | Recommended model: none until observations exist | Reasoning effort: none until observations exist

이 문서의 새 로드맵은 위 상태를 덮어쓰지 않는다. 사용자가 승인하면 현재 queue의 activation condition을 만족시키는 순서로 사용한다.

## 4. 권장 실행 순서

### Priority 0. RC2 release-stage 의사결정

Prerequisite: 사용자가 RC2 tag/release 준비를 계속할지 먼저 결정해야 한다.

Recommended model: none until the release-stage decision; after approval `gpt-5.6-luna`

Reasoning effort: none until the decision; high afterward

현재 Dev와 original gate는 이미 통과했다. 반복 build가 아니라 exact original repository/branch/commit, 포함·제외 범위, `v2.1.0-rc.2` target을 read-only로 조정한 뒤 다음을 별도 승인한다.

1. annotated local tag
2. exact tag push
3. GitHub Release draft
4. publication

Deployment는 이 순서에 포함되지 않는다.

### Priority 1. SDK Candidate Evidence v3.1

Prerequisite: OpenVisionLab Vision SDK source repository와 변경 권한, SDK release order에 대한 명시적 승인.

Recommended model: none until the SDK prerequisite; after approval `gpt-5.6-sol`

Reasoning effort: none until the SDK prerequisite; high afterward

이것이 GPT Pro 분석에서 가장 강하게 근거가 확인된 code priority다. 목표는 Blob/Contour 한 번의 SDK 실행으로 다음을 모두 반환하는 것이다.

- stable Candidate identity
- raw measurements와 geometry
- applied limits
- accepted/rejected state
- machine-readable reject reason
- drawings/metrics와 source coordinate frame

SDK package와 manifest를 먼저 검증한 뒤 OpenVisionLab이 새 package를 소비하고, parity가 모두 통과한 뒤에만 `TryCaptureUnfiltered` second run과 silent fallback을 제거한다.

### Priority 2. ImageCanvas 외부 Consumer audit

Prerequisite: 실제 두 번째 .NET 8 WPF consumer를 지정하거나, 사용자가 sample consumer를 public contract의 승인된 proxy로 명시해야 한다.

Recommended model: none until the consumer decision; after approval `gpt-5.6-terra`

Reasoning effort: none until the consumer decision; medium afterward

첫 단계는 facade 구현이 아니라 별도 consumer가 현재 Library를 얼마나 직접 사용할 수 있는지 증명하는 것이다.

- OpenVisionLab app ProjectReference 없이 build/run
- exact managed/native dependencies 기록
- image ownership, ROI, overlays, view-state, disposal, thread affinity 확인
- public implementation-type leakage와 hard-coded localization/theme inventory
- 기존 OpenVisionLab viewer 회귀 범위 식별

Consumer가 실제로 요구한 method만 facade로 추가한다. net48 multi-target, single-DLL 약속, NuGet publication은 별도 필요가 확인되기 전까지 제외한다.

### Priority 3. LLM catalog/contract consistency audit

Prerequisite: maintenance-mode compatibility audit로 한정한다는 사용자 승인. 새 Skill이나 Provider 구현은 포함하지 않는다.

Recommended model: `gpt-5.6-luna`

Reasoning effort: medium

현재 machine-readable catalog에는 `inspectionIntentSkills` 한 항목이 있고, code-side Guided Setup catalog에는 더 많은 template가 존재한다. 이것이 의도된 “정식 skill과 일반 starter의 구분”인지 실제 drift인지 inventory로 판정한다. 검증된 mismatch만 최소 수정한다.

### Priority 4. Evidence-constrained LLM Skill v1

Prerequisite: 사용자가 LLM maintenance mode를 명시적으로 재개하고, non-LLM equivalent workflow, stable candidate contract, frozen corpus가 먼저 존재해야 한다.

Recommended model: none until all prerequisites; after approval `gpt-5.6-sol`

Reasoning effort: none until all prerequisites; high afterward

재개할 경우에도 범용 Recipe generator가 아니라 `locator-relative-blob-v1` 하나만 대상으로 한다.

```text
Image
  -> deterministic candidate extraction
  -> evidence packet
  -> candidate-ID proposal
  -> typed plan validation
  -> deterministic Pipeline compiler
  -> explicit Run
  -> frozen Train/Validation/Held-out evidence
```

처음에는 manual packet export/import만 구현한다. Provider interface, API, local VLM adapter는 manual path가 가치를 증명한 뒤 결정한다.

### Priority 5. UX evidence와 실제 사용자 검증

Prerequisite: named operator task 또는 실제 participant observations.

Recommended model: none until evidence; after a named agent audit `gpt-5.6-terra`, after real participant observations `gpt-5.6-terra`

Reasoning effort: none until evidence; medium for audit automation, low for observation synthesis

기존 smoke/walkthrough infrastructure로 P1/P2/P3 Persona의 한 named scenario를 측정한다. 새 generic automation framework를 먼저 만들지 않는다. 실제 `CVR-00`은 별도로 유지한다.

### Conditional Priority L1. 초대형 이미지

Prerequisite: `31,800 x 96,800` TIFF 또는 동급 이미지를 OpenVisionLab의 실제 지원 대상으로 삼는다는 사용자 결정과, 필요한 작업이 전체 inspection인지 bounded region inspection인지의 정의.

Recommended model: none until the product requirement; after approval `gpt-5.6-sol`

Reasoning effort: none until the requirement; high afterward

현재 loader는 crash 대신 fail closed하지만 원본 TIFF를 검사하지 못한다. 이 사용 사례가 실제 목표라면 full Tile/Pyramid framework보다 먼저 bounded crop/region metadata loader가 요구를 충족하는지 검토한다. 실제 전체 화면 탐색과 다중 scale inspection이 필요할 때만 tile/pyramid를 설계한다.

### Conditional Priority C1. Graph, Step Debug, Profiler, Subpipeline

Prerequisite: named operator task, reproduced route/debug/performance blocker, current evidence packet.

Recommended model: none until admission; after approval `gpt-5.6-sol`

Reasoning effort: none until admission; high afterward

P256은 현재 4-Step route를 blocker 없이 닫았다. 따라서 Visual Graph와 Debugger는 상용 제품이 갖고 있다는 이유만으로 활성화하지 않는다. 먼저 read-only dependency visualization 또는 current selected-Step timing처럼 가장 작은 slice를 선택한다.

## 5. GPT Pro 분석 전체 조정표

판정 용어:

- **수용**: 현재 근거와 방향이 일치하며 활성 조건 충족 시 구현 가치가 있다.
- **수정 수용**: 문제 또는 목표는 맞지만 제안된 범위·순서·설계를 축소하거나 바꾼다.
- **이미 완료/부분 완료**: 현재 Dev가 제안의 전부 또는 일부를 이미 갖고 있다.
- **보류**: 가치가 있을 수 있으나 named task, evidence, prerequisite, 승인이 없다.
- **배제**: 현재 제품 정체성 또는 stable contract와 충돌한다.
- **미검증**: 정적 분석만으로 사실을 확정할 수 없다.

### 5.1 결론, 제품 정체성, 성숙도, README

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| 장비 전 샘플로 Rule 2D 검사를 티칭·검증하는 Workbench | `AGENTS.md`, product target, current handoff와 일치 | 수용 | 최종 제품 정의의 중심으로 유지한다. |
| Sample -> Layer -> Tool -> Preview -> Pipeline -> Review -> Validation -> Qualified 흐름 | 현재 주요 화면과 storage/report 계약에 연결 | 수용 | 코어 workflow로 채택한다. |
| 단순 OpenCV 테스트 프로그램이 아님 | Recipe, Pipeline, review, history, qualification evidence가 존재 | 수용 | 정확하다. |
| 전체 Workbench 약 80%, 분야별 55~90% | 고정 분모·측정법·현재 runtime replay 없음 | 미검증 | 외부 정적 평가로만 보존하고 현재 사실로 사용하지 않는다. |
| 상용 전체 플랫폼 대비 50~60% | 범위가 다른 제품 비교 | 미검증 | 상용 breadth 점수는 제품 우선순위 근거로 사용하지 않는다. |
| 현재 RC2 안정화 단계 | gate는 이미 Dev/original에서 완료, release stage만 남음 | 부분적으로 오래됨 | 현재는 “release-stage authorization pending”으로 바꿔 이해한다. |
| README 전면에 Object evidence, provenance, validation, snapshot, Viewer/SDK를 더 강조 | 일부는 사용자 가치, 일부는 내부 기술 용어 | 수정 수용 | README editorial contract에 맞춰 사용자 workflow 가치만 별도 audit 후 반영한다. Runtime Fingerprint 같은 기술 용어를 전면에 쌓지 않는다. |
| Noah profile README와 생태계 방향이 대체로 일치 | 첨부 분석의 외부 확인이며 현재 작업에서 profile 원문을 재조회하지 않음 | 입력 기준 수용 | 이 문서에서는 current repository identity와 충돌하지 않는다는 범위만 인정한다. |

### 5.2 Image Workspace, Layer, 대용량 이미지

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| Layer, docking, zoom/pan, ROI, pixel/GV, undo/redo, viewer lifetime이 강함 | stable contract와 current completion evidence 존재 | 수용/이미 완료 | broad rewrite 금지. 회귀 시에만 reopen한다. |
| 이전 Bitmap/ImageSpace/OpenGL lifetime 문제 재개 금지 | `PL-0004`와 관련 reports 완료 | 수용 | source/build/owner가 바뀌거나 regression이 재현될 때만 재개한다. |
| Layer Dependency Graph | current P256 route는 명확성 blocker 없이 완료 | 보류 | 실제 대형/branch Recipe에서 관계 누락이 재현될 때 read-only graph부터 검토한다. |
| Layer별 메모리, 생성 Step, consumer 수 표시 | 현재 operator blocker와 acceptance가 없음 | 보류 | measured memory/debug task가 생기면 선택한다. |
| 동일 Layer overwrite 경고 | 정확한 현재 overwrite defect가 제시되지 않음 | 보류 | route validator가 표현하지 못하는 실제 overwrite 사례가 먼저 필요하다. |
| 초대형 image Tile/Pyramid | 실제 `31,800 x 96,800` TIFF는 load 불가 | 수정 수용 | real gap은 인정한다. 먼저 bounded crop/region loader가 충분한지 판단하고 full pyramid는 필요할 때만 개발한다. |

### 5.3 Tool Teaching

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| PropertyGrid가 핵심이며 유지해야 함 | stable contract | 수용 | hand-written form으로 교체하지 않는다. |
| 초보자에게 parameter 우선순위·단위·영향이 어렵다 | 기존 Learn, preset, signal inspector, result explanation이 부분 대응 | 수정 수용 | 새 panel을 추가하기 전에 현재 runtime에서 남은 exact friction을 측정한다. |
| Teaching Readiness Score | Tool rail readiness와 Guided Setup readiness가 이미 일부 존재 | 부분 완료/보류 | 중복 score system을 만들지 않는다. missing prerequisite가 실제로 숨겨지는 Tool만 기존 presenter를 확장한다. |
| Guided Teaching Panel 6단계 | Learn/Tool/Pipeline 책임이 이미 구분됨 | 수정 수용 | Tool View를 wizard로 바꾸지 않는다. compact next-action이 부족한 named Tool에서만 추가한다. |
| Parameter 영향 설명 | Learn과 signal evidence가 일부 제공 | 수용 조건부 | parameter와 결과의 causal claim이 검증된 항목만 표시한다. generic 화살표 문구는 과도한 단순화를 피한다. |
| 마지막 성공 Preview와 현재 parameter/result 비교 | 유용하지만 retained evidence identity와 stale-state 규칙 필요 | 보류 | 실제 teaching task가 요구하면 source/parameter hash를 가진 bounded comparison으로 개발한다. |
| Matching template quality 평가 | Auto MPoint와 matcher diagnostics가 이미 suggestion/ambiguity evidence를 제공 | 부분 완료 | `Suggested`와 `Qualified`를 계속 분리하고 physical task 없이 자동 선택을 추가하지 않는다. |

### 5.4 Pipeline

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| ordered Step, branch, fixture, acceptance, original/effective provenance가 강함 | current source/reports와 일치 | 수용 | OpenVisionLab의 core strength로 유지한다. |
| Visual Pipeline Graph | commercial lesson은 타당하나 current blocker 없음 | 보류 | 첫 구현은 editor가 아니라 read-only layer/Step graph여야 하며 named task가 필요하다. |
| Run Selected/To/From, breakpoint, cache | general IDE는 exclusion register에 포함, cache invalidation 위험 큼 | 수정 수용/보류 | exact debugging blocker가 생기면 `Run to selected Step` 같은 한 slice만 검토한다. |
| Step copy/convert/evidence timing profiler | per-Step elapsed/p95는 이미 존재, 내부 phase profiler는 Tool open용에 한정 | 부분 완료 | 측정된 bottleneck이 생기면 timing breakdown을 해당 owner에 추가한다. |
| reusable Subpipeline/ToolBlock | CVR-10은 bounded fan-out을 이미 제공, generic nested graph는 없음 | 보류 | 두 번째 재사용 task와 explicit I/O contract가 생기기 전 generic subsystem을 만들지 않는다. |
| Tool Type/parameter/catalog 분산 | 실제 여러 source가 존재 | 수용 문제 / 수정 수용 해법 | descriptor inventory를 먼저 만들고, 모든 것을 한 번에 생성하는 framework는 피한다. |

### 5.5 Recipe Manager, Review, Validation, Learn

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| Recipe Manager lifecycle와 Summary/Advanced 분리는 좋은 구조 | product target/stable contracts | 수용/이미 완료 | 다시 additive dense layout으로 되돌리지 않는다. |
| Draft/Teaching/Validation/Qualified/Superseded/Deprecated 상태 | Working Copy, Qualified/Supersede/Revoke는 일부 존재 | 부분 완료/보류 | 실제 lifecycle 혼동이 재현될 때 최소 상태만 추가한다. |
| operator-facing Recipe Diff | XML/Step comparison 일부 존재 | 수정 수용 | parameter/dependency/validation invalidation 중심의 bounded diff부터 검토한다. |
| Version timeline/change reason | current task 없음 | 보류 | external release/compliance history와 혼동하지 않도록 Recipe-local need가 먼저 필요하다. |
| Pipeline Review는 가장 강한 영역 | object/reject/fixture/geometry/signal evidence 존재 | 수용 | 제품 차별점으로 유지한다. |
| Evidence/Parameters/Diagnostics 3개 공통 탭으로 축소 | 실제 current Runtime layout을 이번 작업에서 실행하지 않음 | 미검증/보류 | 소스 구조만으로 탭 축소를 결정하지 않는다. |
| Tool type에 따라 evidence tab을 동적으로 줄임 | 일부 contextual evidence가 이미 존재 | 수정 수용 | fresh runtime audit에서 irrelevant tab이 실제 blocker일 때만 최소 visibility rule을 추가한다. |
| Validation impact-based rerun 목록 | qualification invalidation에는 유용 | 보류 | parameter/dependency/app/SDK identity의 영향 규칙과 false-negative 위험을 먼저 정의한다. |
| Metric distribution/drift dashboard | object distribution과 Run History baseline 통계 일부 존재 | 부분 완료/보류 | 새로운 dashboard보다 기존 history에서 실제 판단이 막히는 지점을 먼저 찾는다. |
| Snapshot 서명 | distribution/compliance scope | 보류 | signing trust owner와 verification consumer를 먼저 결정해야 한다. |
| F1/context help, Learn progress | contextual Learn entry와 bilingual manual은 완료 | 부분 완료/보류 | parameter-level help는 useful candidate, 학습 progress 시스템은 named learning task가 없으면 만들지 않는다. |

### 5.6 ImageCanvas 외부 재사용

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| `OpenVisionLab.ImageCanvas`는 별도 net8 Windows library | csproj 확인 | 수용 | .NET 8 Windows consumer가 현실적인 첫 대상이다. |
| DLL 하나만 복사하는 독립 SDK는 아님 | OpenCvSharp, SharpGL, native runtime, Localization 의존 | 수용 | dependency manifest가 필요하다. |
| net48에서 직접 참조 불가 | target `net8.0-windows7.0` | 수용 | 실제 net48 consumer가 생기기 전 multi-target/IPC를 개발하지 않는다. |
| `RoiImageCanvasViewModel`이 native control, Mat, timer를 소유 | current source 확인 | 수용 | 외부 consumer 작업에서 session/controller seam을 검토한다. 이름 변경만 하는 refactor는 금지한다. |
| public control이 내부 구현을 많이 노출 | 많은 public implementation/compatibility type 확인 | 수용 | consumer-facing facade로 표면을 좁힐 가치가 있다. 기존 public type 제거는 별도 compatibility 결정이다. |
| theme/localization hard-coded | source/runtime 전체 확인은 미수행 | audit candidate | consumer audit에서 exact string/resource/theme 목록을 만든다. |
| WPF/WinForms/OpenGL host code-behind는 View/Adapter에 남아도 됨 | global/project UI rule과 일치 | 수용 | HWND, focus, reparent, reshape, dispatcher refresh를 VM으로 밀지 않는다. |
| 제안된 `IImageCanvasSession`와 ownership enum | 좋은 방향이지만 consumer 요구 전 설계 고정은 이르다 | 수정 수용 | 기존 owner/lease semantics를 재사용하고 실제 consumer가 필요한 method만 노출한다. |
| sample consumer -> facade -> cleanup -> package 순서 | YAGNI와 compatibility에 적합 | 수용 | package/publication은 sample 및 두 번째 consumer 통과 후 별도 결정한다. |
| 100회 create/dispose, DPI/resize 검증 | relevant runtime gate | 수용 | EXE monitor/D-drive artifact/UI verification rules를 적용한다. |

### 5.7 Vision SDK

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| app은 vendored SDK 3.0을 실제 실행 core로 사용 | manifest/source factory evidence | 수용 | app algorithm copy를 새로 만들지 않는다. |
| SDK commit/hash가 고정됨 | manifest `3.0.0`, commit `ba0055...` | 수용 | release order를 보존한다. |
| Parameter schema가 여러 위치에 분산 | SDK property, factory, PropertyGrid mapper, validator, catalog가 존재 | 수용 문제 | 먼저 inventory와 drift test를 만들고 descriptor의 최소 owner를 결정한다. |
| 모든 consumer가 하나의 `VisionToolDescriptor` 사용 | 장기 방향은 유용하나 큰 framework 위험 | 수정 수용 | Candidate contract 후 실제 중복되는 parameter/metric field부터 도입한다. manual generation까지 한 번에 묶지 않는다. |
| Blob/Contour evidence 때문에 두 번 실행 | `TryCaptureUnfiltered`에서 audit Tool execute 확인 | 수용 | `PL-0010`의 측정된 gap이다. |
| App만 수정해 second run 제거 불가 | SDK result에 limits/accepted/reason 없음 | 수용 | accepted-only fallback으로 evidence를 약화하지 않는다. |
| SDK 3.1 Candidate contract | 필드 방향은 타당 | 수정 수용 | exact version은 SDK policy가 정한다. stable ID, raw measurement, limits, accept/reason, geometry가 최소 contract다. |
| App Factory와 SDK Factory 역할 중첩 | source inventory 필요, drift risk는 타당 | audit candidate | candidate evidence 작업과 섞지 않고 별도 bounded inventory 후 판단한다. |
| composite Tool은 App에 유지 | product responsibility와 일치 | 수용 | 실제 두 번째 SDK consumer가 요구할 때만 이동한다. |
| SDK 전체 재작성 불필요 | current integration이 작동함 | 수용 | focused contract extension을 권장한다. |

### 5.8 MVVM과 코드 책임

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| MVVM + Controller + Presenter + Adapter 혼합은 합리적 | repository structural rules와 일치 | 수용 | pure MVVM 점수를 목표로 삼지 않는다. |
| 파일 크기만으로 전면 refactor 금지 | AGENTS structural contract | 수용 | large file는 review signal일 뿐이다. |
| `RoiImageCanvasViewModel`에서 session owner 추출 | real responsibility seam 확인 | 수정 수용 | external consumer 작업이 활성화될 때 focused proof와 함께 수행한다. |
| Pipeline Review selection/calibration controller | 현재 기능 수정 시에만 가치 | 조건부 수용 | standalone cleanup으로 활성화하지 않는다. |
| Recipe Command Surface에서 LLM use case 추출 | LLM track이 재개될 때 natural owner 가능 | 조건부 수용 | 기존 giant file을 부분 파일로만 더 나누지 않는다. |
| `ImageCanvasControl` renderer/input/state/resource 전면 분리 | 회귀 위험과 second-consumer 부재 | 보류 | facade/consumer가 실제 boundary를 보여준 뒤 결정한다. |
| host/interoperability code를 View에 유지 | global UI rule과 일치 | 수용 | business/domain validation만 밖으로 이동한다. |

### 5.9 사용자 UX와 Persona 감사

| GPT Pro 주장/제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| Preview/Run Review/Validate 용어를 일관되게 설명 | stable meaning은 존재 | 수용 조건부 | tooltip/manual/runtime copy drift audit은 가능하나 새 용어 체계를 만들지 않는다. |
| LLM XML 흐름이 길다 | 안전한 validate/import/run 분리는 의도적 | 수정 수용 | safety gate는 줄이지 않는다. 향후 Evidence flow도 Generate/Review/Compile/Run을 분리한다. |
| Tool Rail primary action 하나 강조 | existing rail contract에 Open/Learn/Sample/Guided Setup 역할 존재 | 부분 완료 | 실제 ambiguity가 runtime에서 재현될 때만 강조 hierarchy를 수정한다. |
| Viewer와 Shell command 중복 | external consumer에서 실제 문제가 될 수 있음 | 조건부 수용 | toolbar visibility options는 consumer audit 결과로 결정한다. |
| advanced technical text를 Summary에서 감춤 | Summary/Advanced 분리가 이미 구현됨 | 이미 대부분 완료 | regression이 없다면 새 layout 작업을 열지 않는다. |
| Persona 3명 자동 audit | regression 관점에서 유용 | 수정 수용 | 새 generic DSL/framework보다 기존 walkthrough에 named scenario를 추가한다. |
| 두 Persona 동일 실패 또는 crash/data loss/implicit run만 수정 admission | existing CVR/video-gated rule과 대체로 일치 | 수용 | 실제 user evidence와 agent evidence를 계속 분리한다. |
| Persona 성공을 novice proof로 사용 금지 | CVR-00 contract | 수용 | 필수 경계다. |

### 5.10 상용 기능 비교

| GPT Pro 제안 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| Visual Pipeline Graph | 상용 교훈은 타당, current trigger 없음 | 보류 | read-only smallest slice부터, named blocker 필요. |
| Step Debug/Profiler | per-Step timing은 존재, general IDE는 out of scope | 수정 수용 | bounded run-to-step 또는 timing breakdown만 evidence 후 검토. |
| Matching/Measurement Assistant | existing Auto MPoint/suggestions 일부 존재 | 부분 완료 | candidate 제안 -> operator 선택 -> explicit Preview 원칙을 유지한다. |
| Reusable Subpipeline | generic need 미확인 | 보류 | 두 번째 reusable workflow와 stable I/O가 필요하다. |
| Calibration/Coordinate Frame Assistant | two-point scale와 fixture review 일부 존재 | 부분 완료/보류 | lens/camera/certified metrology로 확대하지 않는다. |
| Custom Tool Descriptor | plugin framework보다 작아 좋은 방향 | 수정 수용 | 실제 schema drift 해결 범위로 좁게 시작한다. |
| OCR/Barcode | named task 없음 | 보류 | product core 뒤가 아니라 admission packet이 생길 때만 검토한다. |
| HMI, acquisition, PLC, accounts, deployment, deep-learning runtime | product scope와 충돌 | 배제 | 다른 제품/별도 Runtime 영역이다. |

### 5.11 현재 LLM 기능 진단

| GPT Pro 주장 | 현재 근거 | 판정 | 조정 결론 |
| --- | --- | --- | --- |
| 현재 기능은 image understanding보다 XML authoring/validation | Browser Assist, Prompt Builder, validator 구조와 일치 | 수용 | 정확한 현재 정의다. |
| local path 문자열만으로 hosted LLM이 pixel을 볼 수 없음 | 구조적으로 타당 | 수용 | 현재 기능을 multimodal detector라고 설명하면 안 된다. |
| Browser Assist가 image upload/API/correction loop를 하지 않음 | code-side browser URI assist와 maintenance contract | 수용 | 기능 한계를 명시한다. |
| XML/route/dependency/intent gate는 강함 | current validator/contracts | 수용 | 유지한다. |
| XML 값 일치가 physical correctness를 증명하지 않음 | algorithm evidence contract와 일치 | 수용 | runtime drawings/metrics/frozen samples가 별도로 필요하다. |
| catalog와 source-of-truth drift | catalog 1 formal skill, code Guided Setup templates 다수 | audit 필요 | 의도된 계층인지 mismatch인지 먼저 판정한다. 즉시 framework를 만들지 않는다. |
| LLM image meaning 20~30% | 측정 기준 없음 | 미검증 | 현재 사실로 사용하지 않는다. |

### 5.12 Evidence-constrained LLM Skill 설계

| GPT Pro 제안 | 판정 | 조정 결론 |
| --- | --- | --- |
| `NO EVIDENCE, NO COORDINATE` | 수용할 미래 invariant | Candidate ID와 source hash가 없는 좌표를 compiler가 거부한다. |
| LLM은 Candidate ID만 선택 | 수용 | 좌표 materialization은 deterministic compiler가 담당한다. |
| coordinate provenance enum | 수정 수용 | 최소 proven states만 먼저 정의하고 generic enum을 과도하게 넓히지 않는다. |
| Evidence Packet에 source, hash, candidates, contact sheet, schema, prompt 포함 | 수정 수용 | v1은 source hash, candidate list, coordinate frame, preview overlay, producer/version, plan schema만 필수로 시작한다. |
| entropy/contrast/edge density/support/stability/uniqueness 공통 필드 | 보류/도구별 선택 | 모든 candidate kind에 의미가 같은 값만 공통화한다. 나머지는 typed measurement로 둔다. |
| Matching candidate transform test | 수용 조건부 | frozen positive/negative/transform set이 있을 때만 qualification에 사용한다. |
| repeated pattern은 unique anchor로 사용 금지 | 수용 원칙 | ambiguity gate와 operator review를 요구한다. |
| Edge pair는 gradient 최대가 아니라 관계 evidence로 선택 | 수용 원칙 | same-band relationship과 multi-scan support가 있는 typed candidate가 필요하다. |
| 첫 Skill은 Locator Relative Blob v1 | 조건부 수용 | user가 LLM track을 재개하고 exact operator task/corpus를 승인할 때 가장 합리적인 단일 v1 후보다. |
| measurement-only에서 시작 후 Good/Bad로 gate 설정 | 수용 | tolerance를 LLM이 발명하지 않는다. |
| fixed ROI, full image, background-only, unknown ID, hash mismatch, duplicate split, ambiguity, per-image tuning, empty success, missing drawing tests | 수용 | v1 anti-cheating gate의 핵심이다. 구현 시 smallest deterministic test set으로 남긴다. |
| Draft -> Qualified 상태 모델 | 수정 수용 | generic workflow engine보다 immutable report/state file로 최소 구현하고 실제 전이가 반복될 때만 확장한다. |
| manual/API/local provider interface | 보류 | manual export/import가 먼저다. provider abstraction은 두 번째 구현이 실제 필요할 때 만든다. |

### 5.13 GPT Pro가 제시한 Codex 작업 5개

| 작업 | 판정 | 실행 조건 |
| --- | --- | --- |
| 1. Viewer 외부 재사용성 | 수정 수용 | audit와 consumer부터. facade/package는 측정 후. |
| 2. SDK Candidate Evidence | 수용, 최우선 code candidate | SDK repo/권한/manifest release order가 필요. |
| 3. LLM Evidence Skill v1 | 설계 방향 수용, 구현 보류 | explicit LLM track reopen + SDK candidate + frozen corpus. |
| 4. 가상 사용자 3명 감사 | 수정 수용 | 기존 harness 재사용, named scenario, UI 수정은 별도 승인. |
| 5. 선택적 MVVM 경계 | 수용 조건부 | Viewer/LLM 등 실제 touched area에서만 refactor proof와 함께. |

## 6. 단계별 개발 계획과 완료 기준

### Phase A. RC2 release-stage 조정

Goal: 이미 통과한 gate를 중복 실행하지 않고 exact release target과 승인 경계를 확정한다.

Included:

- current original release target read-only 확인
- included/excluded scope와 known issues
- source version `2.1.0`, channel `v2.1.0-rc.2` consistency
- existing artifact/evidence identity 재사용 가능성 판정

Excluded:

- 승인 전 tag/tag push/release action
- deployment

Acceptance:

- exact repository/branch/commit/version/channel이 한 기록에 존재
- tag, push, draft, publication 각각 승인 상태가 명시됨
- 바뀌지 않은 gate를 불필요하게 재실행하지 않음

Evidence:

- release policy, current handoff, `PL-0011`, exact source/asset hashes

### Phase B. SDK one-pass Candidate Evidence

Goal: Blob/Contour 한 번의 SDK run으로 current App review/report가 필요로 하는 모든 candidate evidence를 제공한다.

Included:

- SDK result inventory
- minimal backward-compatible candidate/result contract
- Blob/Contour one-pass implementation
- SDK smoke/package consumer
- SDK release identity/manifest
- App consume와 second-run removal
- full parity replay

Excluded:

- generic descriptor framework
- unrelated SDK tools
- App-side accepted-only simplification

Acceptance:

- candidate count/order/identity parity
- accepted/rejected count와 reason parity
- area/bounds/center/angle/limits parity
- result metric, overlay, selection, Run Report parity
- one primary SDK execution only
- audit failure가 hidden accepted-only evidence로 바뀌지 않음

Evidence:

- SDK source commit/package manifest
- standalone SDK smoke
- clean package consumer
- OpenVisionLab focused Blob/Contour review/report smoke
- exact before/after timing and execution-count trace

### Phase C. ImageCanvas external consumer contract

Goal: OpenVisionLab app과 분리된 .NET 8 WPF consumer가 최소 public contract로 viewer를 사용할 수 있게 한다.

Included:

- separate consumer
- dependency/ownership/thread/theme/localization inventory
- current API usage trace
- minimum facade only after trace
- create/dispose, resize, DPI, ROI, overlay, zoom/pan/fit checks

Excluded:

- net48 support
- Linux/Avalonia port
- single-DLL promise
- public NuGet publication
- complete renderer rewrite

Acceptance:

- OpenVisionLab app ProjectReference 없음
- clean output에서 build/run
- image and overlay/ROI/view-state round trip
- caller/library ownership cases가 deterministic하게 종료
- 100 create/dispose cycle resource plateau
- current OpenVisionLab viewer/ROI/docking regression checks pass

Evidence:

- consumer project and dependency manifest
- D-drive runtime artifacts
- monitor/DPI/window bounds
- resource snapshots
- current-build viewer smokes

### Phase D. LLM contract consistency audit

Goal: machine catalog, Guided Setup, prompt builder, validator, manual의 실제 source-of-truth 관계를 밝힌다.

Included:

- inventory와 mapping table
- intended formal skill/starter distinction
- missing/extra field test
- confirmed mismatch의 minimal correction proposal

Excluded:

- new Skill/provider/prompt family
- XML behavior change without compatibility defect

Acceptance:

- 각 template/skill owner와 consumer가 명확함
- confirmed mismatch는 deterministic check로 재현됨
- maintenance-only fix가 필요 없으면 No Change로 종료

### Phase E. Evidence-constrained LLM Skill v1

Goal: LLM이 arbitrary coordinates를 생성하지 않고 frozen evidence candidate만 선택해 deterministic Pipeline draft를 만든다.

Included:

- one skill: `locator-relative-blob-v1`
- manual evidence export/import
- typed plan and validator
- deterministic compiler
- explicit Run
- frozen Good/Bad/transform/held-out validation

Excluded:

- multi-provider framework
- browser automation
- autonomous tolerance/gate tuning
- per-image ROI
- generic Recipe generation

Acceptance:

- unknown candidate/hash/frame 실패
- coordinate provenance 전수 확인
- LLM 없이 fixed plan replay 가능
- compile 전 Recipe/Layer/Route mutation 없음
- explicit Run 전 실행 없음
- same-source overlays/metrics 존재
- held-out 이전 freeze와 split de-duplication 통과

### Phase F. UX evidence

Goal: 현재 제품 코드 수정 전에 named workflow의 실제 friction과 side effect를 측정한다.

Included:

- Beginner, Vision Engineer, Maintainer 중 하나의 named scenario씩
- action before/after state
- click/navigation/backtrack/dialog counts
- Preview/Run/layer/route/recipe hashes
- fresh screenshots

Excluded:

- virtual success를 human evidence로 표현
- audit와 UI fix를 같은 승인으로 처리
- generic automation platform

Acceptance:

- unexpected mutation zero 또는 exact defect report
- 같은 transition의 2-of-3 failure만 shared UX candidate로 승격
- crash/data loss/implicit run은 즉시 defect candidate로 기록
- CVR-00 상태는 별도로 유지

## 7. 위험과 의사결정

### 가장 큰 위험

1. **좋은 아이디어의 동시 활성화**: graph, profiler, subpipeline, descriptor, LLM, Viewer를 함께 시작하면 제품 코어보다 framework 작업이 커진다.
2. **외부 분석 baseline 혼동**: 공개 main 정적 분석을 current Dev runtime 사실로 그대로 옮길 수 없다.
3. **SDK/App release order 위반**: App부터 second run을 지우면 evidence가 약해진다.
4. **LLM maintenance-mode 무단 재개**: 계획 검토는 새 provider/skill 구현 승인과 다르다.
5. **Viewer facade의 speculative API**: 실제 consumer 없이 interface를 고정하면 새 compatibility burden만 생긴다.
6. **상용 비교로 scope 확대**: 상용 제품의 camera/HMI/deployment breadth는 OpenVisionLab 2D 목표가 아니다.
7. **백분율 과신**: maturity percentage는 completed gate와 unverified boundary를 가린다.
8. **source-only UX overclaim**: 이번 작업은 Runtime WPF UX를 실행하지 않았다.

### 사용자 결정이 필요한 항목

1. RC2 release stage를 계속할지, 계속한다면 어느 original commit을 target으로 할지.
2. Vision SDK source repository 작업을 허용할지와 정확한 repository/path.
3. ImageCanvas의 실제 두 번째 consumer가 무엇인지, 또는 sample consumer를 contract proxy로 인정할지.
4. LLM maintenance mode를 지금 재개할지. 권장 답은 SDK candidate와 consumer evidence가 먼저다.
5. `31,800 x 96,800` TIFF 전체 inspection이 제품 요구인지, bounded ROI 접근이면 충분한지.

## 8. 원문 1~27장 coverage

| 원문 장 | 이 문서의 대응 |
| ---: | --- |
| 1 결론과 권장 방향 | 2, 4 |
| 2 분석 기준과 한계 | 1, 7 |
| 3 Noah README/생태계 일치 | 3, 5.1 |
| 4 분야별 완성도 | 3.2, 5.1 |
| 5.1 Image Workspace/Layer | 5.2 |
| 5.2 Tool Teaching | 5.3 |
| 5.3 Pipeline | 5.4 |
| 5.4 Recipe Manager | 5.5 |
| 5.5 Pipeline Review | 5.5 |
| 5.6 Validation/History/Qualified | 5.5 |
| 5.7 Learn/Manual | 5.5 |
| 6 Viewer 외부 사용 | 5.6, Phase C |
| 7 Vision SDK | 5.7, Phase B |
| 8 MVVM | 5.8 |
| 9 UI/UX | 5.9 |
| 10 Persona 감사 | 5.9, Phase F |
| 11 상용 비교 | 5.10 |
| 12 현재 LLM XML 문제 | 5.11 |
| 13 새 LLM Skill 구조 | 5.12, Phase E |
| 14 Evidence Packet | 5.12, Phase E |
| 15 Matching Point Skill | 5.12, Phase E |
| 16 Edge Point Skill | 5.12, Phase E |
| 17 첫 LLM Skill | 5.12, Phase E |
| 18 Anti-cheating tests | 5.12, Phase E |
| 19 Skill 상태 모델 | 5.12 |
| 20 LLM Provider | 5.12, Phase E exclusions |
| 21 최종 우선순위 | 4, 6 |
| 22 Codex 공통 지시 | 1, 3, 6, 7 |
| 23 Viewer 작업 | 5.13, Phase C |
| 24 SDK Candidate 작업 | 5.13, Phase B |
| 25 LLM Evidence Skill | 5.13, Phase E |
| 26 Persona 감사 | 5.13, Phase F |
| 27 선택적 MVVM | 5.13, Phase C/E 조건 |

## 9. 근거 파일

- `AGENTS.md`
- `docs/README.md`
- `docs/LLM_DOCUMENT_INDEX.json`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
- `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- `docs/contracts/openvisionlab/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`
- `docs/reports/OPENVISIONLAB_SHARED_GPT_PRO_ANALYSIS_RECONCILIATION_20260825.md`
- `docs/reports/OPENVISIONLAB_BLOB_CONTOUR_AUDIT_BASELINE_20260825.md`
- `docs/reports/OPENVISIONLAB_DESKTOP_TIFF_LOAD_20260824.md`
- `docs/reports/OPENVISIONLAB_P256_FOUR_STEP_ROUTE_CLARITY_20260824.md`
- `docs/roadmap/OPENVISIONLAB_COMMERCIAL_VIDEO_DEVELOPMENT_BACKLOG_20260727.md`
- `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineObjectResults.cs`
- `src/Libraries/OpenVisionLab.ImageCanvas/OpenVisionLab.ImageCanvas.csproj`
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`
- `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`
- `docs/contracts/openvisionlab/OPENVISIONLAB_LLM_TOOL_CATALOG.json`
- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/IntentSkills/OpenVisionRecipeGuidedSetupCatalog.cs`

## 10. 검증 경계

- 제품/코드 구조 판단은 current source와 문서에 근거했다.
- 첨부의 상용 제품 설명은 사용자가 제공한 분석과 기존 repository research 문서를 조정했으며 이번 작업에서 웹을 새로 검증하지 않았다.
- UI/UX 판단은 source와 기존 evidence의 범위다.

`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`

- 새 코드, WPF UI, SDK package, external consumer, LLM Skill은 구현하거나 실행하지 않았다.
- 원문 baseline commit과 current Dev의 직접 Git diff는 해당 object가 local Dev에 없어 수행하지 못했다.

실제 문서 검증:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
-> DocumentationIndex=PASS IndexedPaths=80 Routes=12 RootRedirects=101

dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev
-> OpenVisionLab readiness contract passed (13/13)

docs/LLM_DOCUMENT_INDEX.json PowerShell JSON parse
-> PASS

new report/index/map trailing-whitespace scan
-> PASS

chapter/subchapter coverage scan
-> PASS, 33/33 rows (1~4, 5.1~5.7, 6~27)

git diff --check -- docs/LLM_DOCUMENT_INDEX.json docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md
-> PASS
```

첫 coverage 검사에서는 5.1~5.7을 별도 행으로 계산하지 않아 expected
count를 27로 잘못 설정했다. 실제 coverage 행은 33개이며, 기대값을 33으로
수정한 재검사에서 정확한 장 번호 전부가 통과했다. 이는 제품/문서 실패가
아니라 검증식의 기대값 오류다.

## 11. Durable Closure Record

```text
Status: Complete
Scope: User-supplied GPT Pro 2D analysis reconciliation and development-direction documentation only
Acceptance criteria: full attachment inspected -> pass; chapters 1-27 covered -> pass; current authority and source evidence reconciled -> pass; accepted/deferred/rejected/already-complete items separated -> pass; final product goal and staged plan recorded -> pass; implementation/release authorization boundaries preserved -> pass
Verification: TestDocumentationIndex PASS (80/12/101); readiness PASS (13/13); JSON parse PASS; trailing-whitespace PASS; chapter coverage PASS (33/33); tracked documentation diff check PASS; one initial coverage assertion failed only because its expected count omitted the seven 5.x subchapters, then passed after correction
Evidence: this report; attachment SHA-256; current Dev source/docs listed in section 9
Boundary / next dependency: no application/SDK/UI implementation, commit, push, tag, release, or deployment; user decisions in section 7 are prerequisites for any development activation
```
