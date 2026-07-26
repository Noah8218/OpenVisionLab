# OpenVisionLab Structural Refactoring Completion

Updated: 2026-07-26 KST

## Completion Record

Status: Complete

Scope: 2026-07-24부터 2026-07-26까지 진행한 문서/폴더 정리와
MVVM·책임 소유권 중심의 구조 리팩토링 계획 범위입니다. 큰 파일을
줄 수나 대칭 때문에 나눈 것이 아니라, 독립 상태·정책·표시 계산·도구군
매핑·resource 수명을 가진 책임만 실제 owner로 이동했습니다.

Acceptance criteria:

- 문서와 주요 소스 위치가 번호형 임시 구조가 아니라 안정된 책임 이름으로
  탐색 가능하다: Pass
- Recipe, Pipeline Review, PropertyGrid mapper, Learn, Tool teaching의
  확인된 독립 책임이 View/Shell/root mapper에서 빠져 실제
  ViewModel·UseCase·Presenter·Controller·Model·Adapter owner로 이동한다:
  Pass
- partial 파일 추가만으로 완료를 주장하지 않고, 이동 전 owner가 해당
  책임을 더 이상 보유하지 않는다: Pass
- 명시적 Preview/Run, layer, route, XML, PropertyGrid와 기존 UI 계약이
  유지된다: Pass
- 각 bounded slice에 build/smoke/readiness/search/diff 증거가 남는다:
  Pass
- 마지막 저장소 감사에서 근거 없는 추가 분리 우선순위가 남지 않는다:
  Pass

Verification:

- 각 slice의 상세 명령과 결과는 아래 proof 문서에 기록되어 있습니다.
- 최종 source baseline:
  - Dev `6523bc1` — `Extract Pipeline Review fixture presenter`
  - Original `49858eb` — 동일 변경의 reviewed cherry-pick
  - 두 저장소 source tree:
    `6e6289eb324c3be3363c52f8acb5ef763f3afd97`
- 최종 source baseline에서 Debug solution build는 0 warnings /
  0 errors였습니다.
- `OpenVisionReadinessCheck`의 전체 계약이 통과했습니다.
- 마지막 Fixture Designer와 Reference Teach current-source UI smoke는
  `check=OK`, `layout=0`, `text=0`, `internal=0`으로 통과했습니다.
- Dev와 원본은 각각 원격과 동기화된 clean 상태에서 이 문서화 작업을
  시작했습니다.

Evidence: 이 문서, `OPENVISIONLAB_MVVM_REFACTORING_ACTION_PLAN_20260724.md`,
`OPENVISIONLAB_CURRENT_HANDOFF.md`, 아래 proof 문서와 각 문서가 가리키는
`artifacts/`입니다.

Boundary / next dependency: 이 완료 상태는 합의한 구조 리팩토링 계획
범위에 대한 것입니다. 전체 코드가 교과서적인 순수 MVVM이라는 주장,
모든 큰 파일의 제거, 산업 현장 정확도, 배포 준비도 또는 제품 전체 완성을
뜻하지 않습니다. 새 구조 작업은 구체적인 유지보수 변경, 재현된 회귀,
테스트 불가능한 책임 또는 두 번째 실제 consumer가 확인될 때만 다시
엽니다.

## Final Decision

현재의 선제적 구조 리팩토링 캠페인은 종료합니다.

- 파일 길이만으로 owner, 폴더, interface, factory, partial을 추가하지
  않습니다.
- WPF 좌표 변환, hit test, drag, control wiring, timer, animation과
  rendering처럼 View-local인 책임은 View에 남깁니다.
- 작은 한 도구군이나 한 switch case를 없애기 위한 one-off abstraction은
  만들지 않습니다.
- 새 기능이나 버그 수정 중 독립된 상태/정책/표시 계산 경계가 실제로
  드러나면 해당 영역만 bounded slice로 다시 엽니다.

## Completed Ownership Boundaries

### Documentation And Source Navigation

- `docs/`를 `admin`, `contracts`, `research`, `analysis`, `roadmap`,
  `learn`, `reports`, `runbooks`, `evidence`, `assets`로 정리했습니다.
- 루트 `docs/*.md`는 canonical 하위 문서로 연결하는 호환 안내
  문서로 유지합니다.
- 번호형 임시 폴더/파일 이름 대신 `UI`, `Core`, `Common`, `Vision`,
  `Property`, `Library`, `tools`와 안정된 책임 이름을 사용합니다.
- 일반 구조 원칙과 “real boundaries before partials” 규칙은
  `AGENTS.md`에 고정했습니다.

### Recipe And Application Flow

| Final owner | Responsibility removed from broad Shell/command surface | Proof |
| --- | --- | --- |
| `OpenVisionRecipePipelineExchangeUseCase` | Pipeline XML import/export, review bundle 생성과 직렬화 정책 | `OPENVISIONLAB_PIPELINE_EXCHANGE_USECASE_REFACTOR_PROOF_20260725.md` |
| `OpenVisionRecipeWorkspaceUseCase` | Recipe 생성·복제·이름변경·삭제와 기본 Pipeline 준비 | `OPENVISIONLAB_RECIPE_WORKSPACE_USECASE_REFACTOR_PROOF_20260725.md` |
| `OpenVisionRecipePipelineLifecycleUseCase` | Pipeline 활성화·복제·이름변경·삭제·샘플 복제 정책 | `OPENVISIONLAB_PIPELINE_LIFECYCLE_USECASE_REFACTOR_PROOF_20260725.md` |
| `OpenVisionRecipeRunHistoryPresenter` | 최근 이력·baseline·기본 결과 selection projection | `OPENVISIONLAB_RUN_HISTORY_PRESENTER_REFACTOR_PROOF_20260725.md` |
| `OpenVisionRecipeValidationSetPresenter` | Validation Set option/image-row projection과 선택 보존 | `OPENVISIONLAB_VALIDATION_SET_PRESENTER_REFACTOR_PROOF_20260726.md` |
| `OpenVisionRecipeExecutionSessionViewModel` | Validation/sample/pair/catalog 실행 상태와 stop/status 전이 | `OPENVISIONLAB_RECIPE_EXECUTION_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md` |
| `OpenVisionRecipeStepEditSessionViewModel` | selected-Step edit session 상태와 전이 | `OPENVISIONLAB_STEP_EDIT_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md` |
| `VisionPipelinePropertyContext` | mapper-global mutable feature lookup context | `OPENVISIONLAB_PROPERTY_MAPPER_CONTEXT_REFACTOR_PROOF_20260725.md` |

Command partial은 호환 command binding과 WPF adapter 역할로 남습니다.
UseCase/ViewModel/Presenter가 storage 정책, 상태 전이 또는 projection의
실제 owner입니다. frozen LLM XML draft 경계는 유지보수 정리만 했고 새
provider, prompt family 또는 자동화는 추가하지 않았습니다.

### Pipeline Review

| Final owner | Responsibility | Proof |
| --- | --- | --- |
| `OpenVisionPipelineReviewExecutionController`와 기존 result contracts | Pipeline 실행, execution context와 결과 image/cache 수명 | readiness/source ownership contract |
| `OpenVisionPipelineReviewFlowPresenter` | upstream/branch/missing-input/status flow projection | `OPENVISIONLAB_PIPELINE_REVIEW_FLOW_PRESENTER_REFACTOR_PROOF_20260726.md` |
| `OpenVisionPipelineReviewFixturePresenter` | Fixture chain, 기준/현재 pose, 상대 ROI, template/ROI preview와 표시 계산 | `OPENVISIONLAB_PIPELINE_FIXTURE_PRESENTER_REFACTOR_PROOF_20260726.md` |

`OpenVisionPipelineReviewDocument`는 document selection, command
orchestration, reference 저장/재검증과 WPF-bound property 적용을
유지합니다. 마지막 Fixture 분리 후 Document는 1,362줄에서 942줄로
감소했습니다.

### PropertyGrid Mapping

`VisionPipelineStepPropertyMapper`의 숨은 mutable context와 독립
도구군별 create/apply/model 책임을 다음 adapter로 이동했습니다.

- `VisionPipelineTransformPropertyAdapter`
- `VisionPipelineReferenceDifferencePropertyAdapter`
- `VisionPipelinePinArrayGapPropertyAdapter`
- `VisionPipelineLinePropertyAdapter`
- `VisionPipelineGeometryPropertyAdapter`
- `VisionPipelineMatchingPropertyAdapter`
- `VisionPipelineObjectInspectionPropertyAdapter`
- `VisionPipelineBasicImagePropertyAdapter`
- `VisionPipelineEdgeBasedMatchingPropertyAdapter`
- `VisionPipelineFeatureMatchingPropertyAdapter`

Transform PropertyGrid model/converter도 Transform adapter로 이동했고,
single Line과 pair Line은 같은 Line domain owner로 통합했습니다. root
mapper에는 공통 metadata/copy/dispatch와 작은 `Mean` mapping을
의도적으로 남겼습니다. “switch case 0개”를 위한 one-case adapter는
만들지 않습니다.

주요 proof:

- `OPENVISIONLAB_TRANSFORM_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_TRANSFORM_PROPERTY_MODEL_OWNERSHIP_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_LINE_PROPERTY_ADAPTER_CONSOLIDATION_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_GEOMETRY_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_OBJECT_INSPECTION_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_BASIC_IMAGE_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_EDGE_BASED_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`
- `OPENVISIONLAB_FEATURE_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`

### Learn And Tool Teaching

| Final owner | Responsibility | Proof |
| --- | --- | --- |
| `OpenVisionLearnBinarySimulationModel` | Morphology/Blob/Contour의 순수 binary 계산 | `OPENVISIONLAB_LEARN_BINARY_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md` |
| `OpenVisionLearnMatchingSimulationModel` | Matching/FeatureMatching scenario와 score 판정 | `OPENVISIONLAB_LEARN_MATCHING_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md` |
| `OpenVisionLearnLineSimulationModel` | Edge/Line/LineDistance lesson 계산 | `OPENVISIONLAB_LEARN_LINE_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md` |
| `OpenVisionLearnBasicGrayscaleSimulationModel` | Brightness/Arithmetic/Filtering/Threshold 계산 | `OPENVISIONLAB_LEARN_BASIC_GRAYSCALE_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md` |
| `AutoMPointTeachingController` | source/대표 이미지 수명, 분석 identity, 후보/report/template 적용 workflow | `OPENVISIONLAB_AUTO_MPOINT_TEACHING_CONTROLLER_REFACTOR_PROOF_20260726.md` |

Learn 마감 감사에서 Metrics Acceptance, Layer Recipe, Geometry,
Color/HSV, event/timer/rendering은 독립 domain owner가 아니라
View-local presentation임을 확인했습니다. 자세한 stop decision은
`OPENVISIONLAB_LEARN_MODEL_EXTRACTION_CLOSURE_20260726.md`에 있습니다.

## Preserved Contracts

모든 구조 변경은 다음 계약을 유지하는 조건으로 완료했습니다.

- Preview와 Run은 명시적 사용자 동작입니다.
- layer 생성/삭제/선택과 boolean visibility 변경은 자동 실행하지
  않습니다.
- output layer 생성은 input layer를 자동 변경하지 않습니다.
- Tool PropertyGrid와 기존 Pipeline/XML key/default/alias를 유지합니다.
- viewer zoom/pan/drag, ROI overlay, template editor, layer comparison,
  docking과 기본 window chrome을 유지합니다.
- LLM은 선택적 XML authoring 호환 경로이며 제품 동작의 필수 의존성이
  아닙니다.

## Reopen Checklist

구조 리팩토링을 다시 시작하기 전에 아래 중 하나가 실제 증거로 있어야
합니다.

- 구체적인 기능 변경이 둘 이상의 기존 owner에 같은 정책을 중복시킴
- 현재 빌드에서 재현되는 결함이 잘못된 상태/책임 소유권에서 발생함
- 독립 테스트가 필요한 계산이나 상태 전이가 WPF/Shell private state에
  갇혀 있음
- 두 번째 실제 consumer가 등장해 현재 View-local 계산을 재사용해야 함
- resource lifetime 또는 dependency 방향이 실제 leak/coupling 문제를
  만듦

재개 시에는 owner, 입력/출력, 상태 수명, 금지 의존성, focused smoke와
완료 기준을 먼저 정의합니다. 이 조건이 없으면 현재 완료 상태를
유지합니다.
