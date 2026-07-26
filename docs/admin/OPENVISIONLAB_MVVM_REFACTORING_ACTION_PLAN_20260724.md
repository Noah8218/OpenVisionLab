# OpenVisionLab MVVM Refactoring Action Plan (2026-07-24)

## 1) 현재 작업 상태 요약

- 문서/폴더 재편은 `docs/` 기준 카테고리 이동이 완료되었습니다.
  - `docs/admin`, `docs/contracts`, `docs/research`, `docs/analysis`, `docs/roadmap`, `docs/learn`, `docs/reports`, `docs/runbooks`, `docs/evidence`, `docs/assets`
  - 루트 `docs/*.md`은 인덱스/안내 스텁(현재 본문은 하위 폴더)로 유지됩니다.
- 소스 구조상 번호형(legacy) 루트 폴더는 현재 경로에서 제거되었고, 주요 실행 경로는 `UI`, `Core`, `Common`, `Vision`, `Property`, `Library`, `tools` 하위로 정리되어 있습니다.
- `llm_prompt_packets/pin_gap_distance/` 내 참조/파일명이 정리되었습니다.
  - `COPY_THIS_TO_GPT.md`
  - `SEND_VALIDATION_NG_TO_GPT.md`
  - `FINAL_USER_MESSAGE_XML_ONLY.md`
  - `CAPTURE_GPT_TRANSCRIPT.md`
- 기존 노트에서 오래된 `02_PASTE_VALIDATION_NG_BACK_TO_GPT.md` 참조는 해당 최신 문서로 교체했습니다.

## 2) 지금 즉시 리팩터 후보(우선순위)

`obj/`/`bin/`의 생성 코드와 산출물을 제외한 후보만 반영합니다.

1) **`UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`** (7,973줄)
   - 명령 라우팅/유효성/상태 갱신이 매우 밀집
   - 현재 가장 큰 이해장벽 후보

2) **`UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs` + `OpenVisionShellHostView.Interactions.cs`**
   - 생성/바인딩/이벤트 훅이 커맨드 표면과 결합
   - 분해 효과는 중간급 (현재 일부 분리된 상태)

3) **`UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`** (1,932줄)
   - 문서 오케스트레이션 + 출력 상태 반영이 혼재

4) **`UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml.cs`** (+ 분리된 `Events.cs`, `OpenVisionPipelineReviewViewRenderService.cs`)
   - 이미 부분 분리가 되어 있으나, 상태 갱신 경로를 더 분리할 수 있음

5) **`UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`** (3,750줄)
   - 도구별 매핑/어댑터 분해가 다음 단계에서 명확한 리스크 절감 효과

## 3) 다음 적용 Slice (1회차)

### Slice A: Recipe Command Surface 분해 (P1)

- `OpenVisionShellHostRecipeCommandSurface`를 기능별로 분해:
  - **ActionController**: 실행 및 결과 메시지
  - **StateCoordinator**: 선택/상태 보존
  - **ValidationService**: 커맨드 사전검증/예외 경고

- 유지 조건:
  - Preview/Run 계약 변경 없음
  - 레이어/라우팅/리포트/로드맵 동작 불변
  - 기존 빌드 및 검증 경로 통과

### Slice B: PipelineReview Document 분해 (P2)

- Document 클래스는 `orchestrator`와 `state projection`로 책임을 분리
- 대상: 문서 레벨 상태 변화와 UI 전달 채널의 분리

## 3-1) 바로 진행한 작업 (2026-07-25)

- `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`에서
  이벤트 핸들러/스텝 제어(선택/실행/언어 변경/보정 작업 요청) 경로를
  `OpenVisionPipelineReviewDocument.Events.cs`로 partial 분리했습니다.
- 동작은 유지되며 `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`는 PASS(0 경고/0 오류)했습니다.
- 다음 분해는 `OpenVisionShellHostRecipeCommandSurface`를 중심으로 같은 방식의 partial/event-service 분리입니다.

- **추가 완료 업데이트 (2026-07-25):**
  - `OpenVisionShellHostRecipeCommandSurface.cs`의 `FindPipelinePreviewStep`/`StepMatches`/`UpdateSelectedRecipeSummary`/`BuildPipelinePreviewSteps`/
    `NavigateSelectedStep*`/`Resolve/Load 실패 Step`/`SelectPipelinePreviewStep`/`CreateUniqueRecipeName`/`SanitizePathSegment`/
    `SetSelectedRecipeName`/`RefreshCommandState`를 `OpenVisionShellHostRecipeCommandSurface.StepNavigation.cs`로 분리했습니다.
  - 분리 후 빌드 재확인(`dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`) 통과(0 경고/0 오류).

- **완료 업데이트 (2026-07-25):**
  - `UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`를 partial class로 전환했고,
    모든 커맨드 바인딩 초기화를 `OpenVisionShellHostRecipeCommandSurface.Commands.cs`로 이동했습니다.
  - `public ICommand ...` 프로퍼티를 `get; private set;`로 변경해 생성자/분리 메서드 간 할당을 안정화했습니다.
  - 전체 빌드(`dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`) 통과(0 경고/0 오류).
  - 같은 파일을 책임 그룹(Recipe / Workflow / Review) 단위의 3개 초기화 메서드로 추가 분해해 가독성을 더 정리했습니다.

### Slice A 진행 상태 정리 (2026-07-25)

- Slice A-1: `OpenVisionShellHostRecipeCommandSurface` 커맨드 초기화 책임 분리 ✅ 완료
- Slice A-2: `OpenVisionShellHostRecipeCommandSurface`에서 Step 네비게이션/요약/명명/선택 유틸 책임을 `StepNavigation` partial로 분리 ✅ 완료
  - 완료 기준 충족: 해당 블록을 분리하고 동일 빌드/회귀 기준 통과
- Slice B(문서 오케스트레이션 partial 추가 분해) ✅ 완료
  - `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`의 이벤트/선택 제어 경로 분리 적용

### 다음 제안 우선순위 (2026-07-25)

- `VisionPipelineStepPropertyMapper.BasicImage.cs`로 Threshold / Morphology /
  Filter / EdgeDetection 도구군의 생성·재적용 책임을 분리 ✅ 완료
  - Root mapper는 도구군 dispatch만 유지하고, 기존
    `VisionPipelineStepBuilder` XML 경로는 변경하지 않음
  - Debug build 및 current-source `wpf_shell_host_pipeline_step_edit_handoff`
    smoke 통과
- `OpenVisionShellHostRecipeCommandSurface.ValidationSets.cs`로 로컬 Validation
  Set 생성/삭제, 이미지·폴더 등록, 누락 경로 복구, 저장, 선택/행 projection을
  분리 ✅ 완료
  - 기존 command/test 진입점과 explicit Preview/Run·레이어·라우팅 계약 유지
  - current-source `wpf_shell_host_recipe_local_validation_set` smoke, Debug
    build, readiness check 통과
- `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs`로 기존 LLM XML
  초안 로드/검증/의존성 검사/가져오기 책임을 분리 ✅ 완료
  - LLM 유지보수 모드 준수: 새 provider/prompt/template는 추가하지 않음
  - current-source `wpf_shell_host_recipe_manager_summary` smoke 및 Debug build 통과
- `VisionPipelineStepPropertyMapper.ObjectInspection.cs`로 Blob / Contour
  PropertyGrid family를 분리 ✅ 완료
  - 기존 area/width/height 필터 및 Blob fixture parameter 직렬화 유지
  - current-source `p216_object_dimension_filters_property_grid` smoke 및
    Debug build 통과
- `VisionPipelineStepPropertyMapper.LinePair.cs`로 LineDistance /
  LineIntersection two-line PropertyGrid 모델을 분리 ✅ 완료
  - 독립 Line A/B ROI·극성·방향·각도와 XML round-trip 계약 유지
  - current-source `wpf_shell_host_recipe_line_pair_properties` smoke 및 Debug build 통과
- `VisionPipelineStepPropertyMapper.Matching.cs`로 일반 Matching /
  TemplateMatching PropertyGrid family를 분리 ✅ 완료
  - fixture frame 발행 parameter와 기존 XML/default 계약 유지
  - current-source `wpf_shell_host_recipe_fixture_properties` smoke 및 Debug build 통과
- `VisionPipelineStepPropertyMapper.EdgeBasedMatching.cs`로 EdgeBasedMatching /
  EdgeBasedTemplateMatching PropertyGrid family를 분리 ✅ 완료
  - unique-match/Top-K XML 기본값과 기존 Preview/Run 계약 유지
  - current-source `wpf_shell_host_edge_based_matching_tool` smoke, Debug build 통과
- `VisionPipelineStepPropertyMapper.FeatureMatching.cs`로 Feature /
  FeatureMatching / SIFT PropertyGrid family를 분리 ✅ 완료
  - score, RANSAC, template-path XML 기본값과 기존 Preview/Run 계약 유지
  - current-source `wpf_shell_host_feature_matching_tool` smoke, Debug build 통과
- `OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`로 레시피 생성 /
  복제 / 이름 변경 / 삭제와 생성 직후 전환 책임을 분리 ✅ 완료
  - 기존 저장소 호출, 삭제 확인, 옵션 갱신, Preview/Run·레이어·라우팅 계약 유지
  - current-source `wpf_shell_host_recipe_manager_summary` smoke, Debug build 통과
- `OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs`로 Pipeline 활성화 /
  복제 / 이름 변경 / 삭제와 샘플 Pipeline 복제 책임을 분리 ✅ 완료
  - 기존 저장소 호출, 삭제 확인, active 전환, Preview/Run·레이어·라우팅 계약 유지
  - current-source `wpf_shell_host_recipe_context_switch` smoke, Debug build 통과
  - 별도 기록: 광범위 `wpf_shell_host_recipe_language_controls`는 이후 LLM 의존성 보고서 한국어 문자열 검사에서 재현 실패
- `OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs`로 Pipeline XML 가져오기 /
  내보내기와 review bundle export 책임을 분리 ✅ 완료
  - 기존 저장소, review bundle dry-run, reference 수집, Preview/Run·레이어·라우팅 계약 유지
  - current-source `wpf_shell_host_recipe_review_bundle_import` 및 context smoke, Debug build 통과
  - 별도 기록: review bundle 화면 smoke는 요약 화면에서 고급 XML 버튼을 기대하는 UI 전제 불일치로 export 이후 실패
- `OpenVisionShellHostRecipeCommandSurface.RunHistory.cs`로 최근 batch 이력 /
  baseline 선택 / 기본 sample 결과 선택 책임을 분리 ✅ 완료
  - 최근 3개 이력, 이전 선택 유지, 자동 baseline, review-queue/NG 우선순위 계약 유지
  - current-source `wpf_shell_host_recipe_local_validation_set` smoke, Debug build 통과
- **1순위:** `UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`의 다음 Tool family별 매핑 분해
- **2순위:** `UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface`의 비-LLM Recipe/Pipeline 관리 명령군 추가 분할
  - 유지조건: Preview/Run 라우팅·레이어 변경 계약 불변
- **3순위:** `UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`의 뷰 동작(애니메이션/탐색/상태)와 도메인 동작 분리

## 4) 완료 기준

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` 통과
- 기존 런타임 동작 회귀 없음 (기능별 수동/로그 근거 확인)
- 변경 범위가 한 번에 한 Slice로 제한
- 문서 링크(필요 시 current handoff / map) 업데이트

## 5) 권장 실행 모델

- Slice A (Recipe Command Surface): **GPT-5.3-Codex / high**
- Slice B (PipelineReviewDocument): **GPT-5.3-Codex / medium**

## 6) Actual Boundary Update (2026-07-25)

- Partial 분리는 임시 책임 인벤토리로만 취급합니다. 파일 수나 길이 감소만으로 MVVM/구조 리팩터 완료를 선언하지 않습니다.
- `OpenVisionRecipePipelineExchangeUseCase`가 Pipeline XML import/export, review-bundle 생성, XML 직렬화의 실제 owner가 되었습니다. `OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs`는 파일 선택, UI 상태 갱신, review dry-run 진입, UI 참조 수집만 하는 adapter입니다.
- 검증: Debug build(0 warning/0 error), current-source `wpf_shell_host_recipe_review_bundle_import`, readiness check PASS. Evidence: `docs/admin/OPENVISIONLAB_PIPELINE_EXCHANGE_USECASE_REFACTOR_PROOF_20260725.md`.
- 다음 구조 우선순위는 `RecipeWorkspace` 또는 `RunHistory`에서 같은 기준의 실제 UseCase/Presenter owner를 하나 추출하는 일입니다. partial을 추가하는 작업은 owner, 입력/결과, 의존성, focused evidence가 먼저 정리되지 않으면 진행하지 않습니다.

### Recipe Workspace UseCase Update (2026-07-25)

- `OpenVisionRecipeWorkspaceUseCase`가 레시피 생성, 고유 이름 결정, 복제, 이름변경, 삭제, fallback workspace 준비, 새 레시피 기본 Pipeline 준비의 실제 owner가 되었습니다.
- `OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`는 command enablement, 삭제 확인, UI fallback 선택, recipe switch/status/refresh만 담당합니다.
- 검증: Debug build(0 warning/0 error), current-source `wpf_shell_host_recipe_manager_summary`, readiness check PASS. Evidence: `docs/admin/OPENVISIONLAB_RECIPE_WORKSPACE_USECASE_REFACTOR_PROOF_20260725.md`.
- 다음 구조 우선순위는 `RunHistory`에서 UI 상태 projection을 Presenter로 분리하는 것입니다. partial 추가만으로 완료를 주장하지 않습니다.

### Run History Presenter Update (2026-07-25)

- 기존 `OpenVisionRecipeRunHistoryPresenter`가 최근 3건 projection, 이전 선택 유지, 기준선/자동 기준선 선택, batch/pair 기본 결과 선택을 실제로 소유합니다.
- `OpenVisionShellHostRecipeCommandSurface.RunHistory.cs`는 저장된 run 후보 조회와 Presenter 결과의 WPF property 대입만 담당합니다.
- 검증: Debug build(0 warning/0 error), current-source `wpf_shell_host_recipe_local_validation_set`, readiness check PASS. Evidence: `docs/admin/OPENVISIONLAB_RUN_HISTORY_PRESENTER_REFACTOR_PROOF_20260725.md`.
- 다음 구조 우선순위는 Pipeline Lifecycle 명령군의 storage CRUD를 실제 UseCase로 분리하는 것입니다. partial 추가만으로 완료를 주장하지 않습니다.

### Pipeline Lifecycle UseCase Update (2026-07-25)

- `OpenVisionRecipePipelineLifecycleUseCase`가 Pipeline 활성화, 고유 이름 결정, 복제, 이름변경, 삭제 fallback, 샘플 Pipeline 복제/활성화의 실제 owner가 되었습니다.
- `OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs`는 command guard, 삭제 확인, UI 상태/선택과 refresh만 담당합니다.
- 검증: Debug build(0 warning/0 error), current-source `wpf_shell_host_recipe_context_switch`, readiness check PASS. Evidence: `docs/admin/OPENVISIONLAB_PIPELINE_LIFECYCLE_USECASE_REFACTOR_PROOF_20260725.md`.
- 다음 우선순위는 남은 command partial을 추가로 쪼개는 것이 아니라, `ValidationSets` 또는 LLM 유지보수 workflow에서 테스트 가능한 owner를 실제로 추출할 수 있는지 재평가하는 것입니다.

### Validation Set Presenter Update (2026-07-26)

- 기존 `OpenVisionRecipeValidationSetPresenter`가 option/image-row projection과 선택 보존 규칙을 실제로 소유합니다.
- 검증: Debug build(0 warning/0 error), current-source `wpf_shell_host_recipe_local_validation_set`, readiness check PASS. Evidence: `docs/admin/OPENVISIONLAB_VALIDATION_SET_PRESENTER_REFACTOR_PROOF_20260726.md`.
- 다음에는 LLM 유지보수 workflow가 아니라, 남은 command partial의 실제 owner 후보를 다시 감사합니다.

### Next Boundary Audit (2026-07-26)

- LLM 명령군은 유지보수 동결로 제외하고, Pipeline Review Document는 현재 실행/화면 상태 결합 때문에 partial 이동만으로는 경계가 되지 않아 보류합니다.
- 다음 실제 Slice는 root `VisionPipelineStepPropertyMapper`에 남은 한 Tool family의 Create/Apply XML adapter를 비-WPF mapper로 추출하는 일입니다.
- Evidence: `docs/admin/OPENVISIONLAB_NEXT_STRUCTURAL_BOUNDARY_AUDIT_20260726.md`.

### Transform Property Adapter Update (2026-07-26)

- `VisionPipelineTransformPropertyAdapter`가 RotateScale/AffineTransform 별칭,
  parameter/default projection, Step 생성, fixture 소비 parameter, detected
  Point binding parameter의 실제 owner가 되었습니다.
- root `VisionPipelineStepPropertyMapper`는 transform ToolType case와 직접
  builder 호출을 제거하고 새 adapter dispatch 및 공통 Step metadata/copy만
  유지합니다. 새 adapter는 partial이 아닙니다.
- 검증: Debug build(0 warning/0 error), Affine aliases/known-matrix/
  PropertyGrid-XML round-trip/실패 gate contract, current-source
  RotateScale/Affine/P219 PropertyGrid UI smokes PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_TRANSFORM_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.

### Pipeline Review Flow Presenter Update (2026-07-26)

- `OpenVisionPipelineReviewFlowPresenter`가 이전 enabled Step output,
  branch input, upstream producer, missing input, waiting/loaded/execution
  status와 `PipelineFlowStepItem` projection의 실제 owner가 되었습니다.
- `OpenVisionPipelineReviewDocument`는 layer image와 execution summary를
  조회해 Presenter에 전달하고 결과를 기존 View에 적용합니다.
- 새 Presenter는 partial이 아니며 Document/View/display manager/execution
  controller에 의존하지 않습니다.
- 검증: Debug build(0 warning/0 error), current-source normal/input-state/NG
  Pipeline Review UI smokes PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_PIPELINE_REVIEW_FLOW_PRESENTER_REFACTOR_PROOF_20260726.md`.

### Step Edit Session ViewModel Update (2026-07-26)

- `OpenVisionRecipeStepEditSessionViewModel`이 selected Step의 edit object,
  dirty, status, corrected-output review 상태와 Load/Dirty/Clean/Clear 전이의
  실제 owner가 되었습니다.
- Shell은 XML lookup/save, Tool session 주입, 기존 XAML 알림 adapter만
  유지합니다. 새 ViewModel은 partial이 아닙니다.
- 검증: source old-field absence, extended selected-Step handoff,
  Fixture edit/apply/rerun, Debug build, readiness PASS.
- Fixture smoke의 기존 hidden-button 실패는 Step Details tab을 먼저
  선택하도록 테스트 전제를 수정했습니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_STEP_EDIT_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`.

### Validation Run Session ViewModel Update (2026-07-26, consolidated)

- 현재 `OpenVisionRecipeExecutionSessionViewModel`이 Validation Suite 실행 중,
  Local Validation Set 실행 중, 중지 요청, 상태 문구와
  Start/RequestStop/Complete/SetStatus 전이의 실제 owner가 되었습니다.
- Shell은 명시적 실행 명령, 이미지 순회, frozen identity 검사, 판정,
  report 저장, Run History 갱신과 기존 XAML 알림 adapter만 유지합니다.
  새 ViewModel은 partial이 아니며 Shell/저장소/실행 서비스/View에
  의존하지 않습니다.
- 검증: 기존 Shell 필드 부재, current-source Local Validation Set 전체
  실행 및 stop/partial-save, Preview/Run·레이어·workspace·route 불변,
  Debug build, focused smoke build, readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_VALIDATION_RUN_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`.

### Recipe Execution Session ViewModel Update (2026-07-26)

- 기존 Validation 실행 세션을 새 클래스로 늘리지 않고
  `OpenVisionRecipeExecutionSessionViewModel`로 통합했습니다.
- Validation Suite, Local Validation Set, selected sample, Good/Bad pair,
  Catalog의 여섯 running 상태와 validation stop/status 전이를 한 owner가
  보유합니다. Shell의 sample/pair/catalog running 필드 세 개는
  제거했습니다.
- 각 command guard의 기존 조합은 그대로 유지했으며 새로운 동시실행
  차단, 병렬화, 결과 Summary 이동은 하지 않았습니다.
- 검증: old-field absence, current-source Local Validation complete/stop/
  partial-save, real Good/Bad pair rerun, Preview/Run·레이어·workspace·route
  불변, Debug build, readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_RECIPE_EXECUTION_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 root `VisionPipelineStepPropertyMapper`에 남은 직접
  Create/Apply family를 다시 감사해 한 family만 비-partial adapter로
  옮길 가치가 있는지 판단하는 것입니다. Recommended model:
  gpt-5.6-terra | Reasoning effort: medium.

### ReferenceDifference Property Adapter Update (2026-07-26)

- `VisionPipelineReferenceDifferencePropertyAdapter`가 ToolType 인식,
  parameter/default projection, 레거시 `ReferencePaths` fallback,
  PropertyGrid 모델, canonical Step 재생성의 실제 owner가 되었습니다.
- root `VisionPipelineStepPropertyMapper`에서는 직접 ToolType case, private
  PropertyGrid 모델, `ToStep`, reference-path helper를 제거하고 adapter
  dispatch 및 공통 metadata/parameter copy만 유지합니다. 새 adapter는
  partial이 아닙니다.
- readiness도 예전 root 내부 구현을 요구하지 않고 새 owner와 root
  dispatch 경계를 각각 검사합니다.
- 검증: Debug build(0 warning/0 error), current-source
  `wpf_shell_host_recipe_reference_difference_properties`, visual inspection,
  readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_REFERENCE_DIFFERENCE_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 남은 root mapper family를 다시 감사하고 전용
  round-trip 회귀 근거가 있는 한 family만 선택하는 것입니다.
  Recommended model: gpt-5.6-terra | Reasoning effort: medium.

### PinArrayGap Property Adapter Update (2026-07-26)

- `VisionPipelinePinArrayGapPropertyAdapter`가 `PinArrayGap`/
  `AdjacentPinGap` alias, parameter/default projection, PropertyGrid 모델,
  미표현 baseline parameter 보존, Step 재생성의 실제 owner가 되었습니다.
- root `VisionPipelineStepPropertyMapper`에서는 직접 alias case, private
  PropertyGrid 모델, `ToStep`를 제거하고 adapter dispatch 및 공통
  metadata/parameter copy만 유지합니다. 새 adapter는 partial이 아닙니다.
- 전용 smoke는 `ALLOW_BRANCH_INPUT` 보존과 함께
  `AdjacentPinGapTool` alias/default/baseline round trip도 검사합니다.
- 검증: Debug build(0 warning/0 error), current-source
  `wpf_shell_host_recipe_pinarraygap_properties`, visual inspection,
  readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_PIN_ARRAY_GAP_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- Line Pair partial은 같은 파일에 GeometryMeasure/CircleGauge base가 있어
  이번 한-family 범위에서 제외했습니다. 다음 우선순위는 두 회귀 범위를
  모두 확보한 뒤 최소 독립 경계를 설계하는 것입니다. Recommended
  model: gpt-5.6-terra | Reasoning effort: medium.

### Line Pair Property Adapter Update (2026-07-26)

- 기존 `VisionPipelineStepPropertyMapper.LinePair.cs` partial을 제거하고
  `VisionPipelineLinePairPropertyAdapter`가 LineDistance/LineIntersection
  alias, Line A/B projection, PropertyGrid 모델, Step 재생성, Tool View
  LineGauge pair handoff의 실제 owner가 되었습니다.
- root mapper는 create/apply/metric dispatch와 기존 public
  `TryCreateLineGaugePair` compatibility forwarder만 유지합니다.
- Line Pair 파일에 잘못 포함됐던 `PipelineGeometryPropertyBase`는 유일한
  파생 모델인 GeometryMeasure/CircleGauge 옆으로 이동했습니다.
- 새 interface/factory/codec 복제는 추가하지 않았고 기존 공통 mapper
  helper를 그대로 사용합니다.
- 검증: Debug build(0 warning/0 error), current-source Line Pair PropertyGrid,
  P213 Geometry PropertyGrid/Review, visual inspection, readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_LINE_PAIR_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 GeometryMeasure/CircleGauge를 하나의 응집된
  adapter 후보로 감사하되, P213 두 회귀를 완료 gate로 유지하는
  것입니다. Recommended model: gpt-5.6-terra | Reasoning effort: medium.

### Geometry Property Adapter Update (2026-07-26)

- `VisionPipelineGeometryPropertyAdapter`가 GeometryMeasure/
  GeometricMeasurement/CircleGauge alias, 공통 baseline/acceptance state,
  두 PropertyGrid 모델, typed feature converter, reference parsing, ROI
  formatting, Step 재생성의 실제 owner가 되었습니다.
- root mapper에서는 직접 ToolType/apply/metric case와 Geometry
  base/model/converter/helper를 제거하고 adapter dispatch 및 기존 공통
  codec/metadata/final copy만 유지합니다. 새 adapter는 partial이
  아닙니다.
- 새 interface/factory/codec 복제 없이 기존 공통 mapper helper를
  재사용합니다.
- 검증: Debug build(0 warning/0 error), current-source P213 Geometry
  PropertyGrid/Review, 모든 geometry mode와 CircleGauge gate, alias
  round-trip, visual inspection, readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_GEOMETRY_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 남은 direct/partial mapper family를 다시
  감사하되 전용 current round-trip 회귀가 없는 family는 분리하지 않는
  것입니다. Recommended model: gpt-5.6-terra | Reasoning effort: medium.

### Matching Property Adapter Update (2026-07-26)

- 남은 mapper family를 재감사한 결과, 기존 Recipe Fixture PropertyGrid
  create/apply/XML reload 회귀를 가진 Matching만 다음 경계로
  선택했습니다.
- `VisionPipelineMatchingPropertyAdapter`가 Matching/TemplateMatching
  alias, parameter/default projection, Fixture publish 상태, PropertyGrid
  모델, Step 재생성, Fixture parameter 보강, metric 판정의 실제 owner가
  되었습니다.
- root mapper에서는 직접 Matching case, private 모델, post-apply helper,
  metric case를 제거하고 adapter create/apply/metric dispatch만
  유지합니다. 기존 partial은 제거됐으며 새 adapter는 partial이
  아닙니다.
- 검증: Debug build(0 warning/0 error), canonical Matching Fixture/scale/
  layer XML round-trip, `TemplateMatchingTool` alias canonical round-trip,
  current-source Recipe Fixture PropertyGrid, visual inspection, readiness
  PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- ObjectInspection, BasicImage, EdgeBasedMatching, FeatureMatching, single
  LineGauge, Mean은 focused selected-Step create/apply 회귀가 부족하므로
  추가 분리를 보류합니다. 다음 구조 우선순위는 새 partial 추출이
  아니라 해당 gate가 실제 유지보수 필요로 생길 때만 재평가하는
  것입니다. Prerequisite: concrete mapper maintenance need and focused
  selected-Step round-trip gate | Recommended model: none until evidence
  exists | Reasoning effort: none until evidence exists.

### Object Inspection Property Adapter Update (2026-07-26)

- 사용자의 명시적 구조 리팩토링 계속 요청에 따라, production 이동 전에
  P216 smoke에 BlobTool/ContourTool selected-Step create/apply baseline
  gate를 추가하고 기존 partial 상태에서 먼저 통과시켰습니다.
- `VisionPipelineObjectInspectionPropertyAdapter`가 Blob/Contour alias,
  parameter/default projection, PropertyGrid 모델, Step 재생성, Blob
  Fixture parameter 보강, metric 판정의 실제 owner가 되었습니다.
- root mapper에서는 직접 Blob/Contour case, private 모델, post-apply
  helper, metric case를 제거하고 adapter create/apply/metric dispatch만
  유지합니다. 기존 partial은 제거됐으며 새 adapter는 partial이
  아닙니다.
- P216 target은 반복 실행 시 이전 smoke recipe 설정이 남지 않도록
  transient workspace 정리와 고유 recipe 이름을 사용합니다.
- 검증: pre-move baseline, post-move Blob/Contour alias/parameter/metadata/
  layer round-trip, current-source P216 UI, 관련 Recipe Fixture PropertyGrid,
  Debug build(0 warning/0 error), visual inspection, readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_OBJECT_INSPECTION_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 BasicImage 네 도구를 하나의 adapter 후보로
  감사하되, 먼저 Threshold/Morphology/Filter/EdgeDetection focused
  selected-Step create/apply baseline gate를 정의하고 통과시키는
  것입니다. Recommended model: gpt-5.6-terra | Reasoning effort: medium.

### Basic Image Property Adapter Update (2026-07-26)

- production 이동 전에 기존 Filter/Morphology layout smoke에 Threshold,
  Morphology, Filter, EdgeDetection 네 selected-Step create/apply baseline을
  추가하고 기존 root/partial 상태에서 먼저 통과시켰습니다.
- `VisionPipelineBasicImagePropertyAdapter`가 네 Tool alias, parameter/default
  projection, 네 PropertyGrid 모델, Step 재생성, metric 판정의 실제
  owner가 되었습니다.
- root mapper에서는 직접 ToolType/metric case와 네 private 모델을
  제거하고 adapter create/apply/metric dispatch만 유지합니다. root는
  1,958줄에서 1,233줄로 감소했습니다.
- 검증: pre/post-move 네 도구 round-trip, current-source Filter/Morphology
  layout, Threshold Tool, Edge Learn, Debug build(0 warning/0 error), visual
  inspection, readiness PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_BASIC_IMAGE_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 EdgeBasedMatching의 기존 create 검사를
  selected-Step apply round-trip baseline으로 확장한 뒤 standalone owner
  추출 가치가 있는지 판단하는 것입니다. Recommended model:
  gpt-5.6-terra | Reasoning effort: medium.

### Edge Based Matching Property Adapter Update (2026-07-26)

- production 이동 전에 기존 Edge Based Matching Tool smoke를 별칭,
  canonical apply, XML/Step 이름, input/output layer, acceptance metadata,
  pattern/score/unique-match/threshold/Canny 설정 round-trip으로 확장하고
  기존 partial 상태에서 먼저 통과시켰습니다.
- `VisionPipelineEdgeBasedMatchingPropertyAdapter`가
  EdgeBasedMatching/EdgeBasedTemplateMatching/EdgeTemplateMatching alias,
  parameter/default projection, PropertyGrid 모델, Step 재생성, metric
  판정의 실제 owner가 되었습니다.
- root mapper에서는 직접 ToolType/metric case와 private 모델을 제거하고
  adapter create/apply/metric dispatch만 유지합니다. 기존 partial은
  제거됐으며 새 adapter는 partial이 아닙니다.
- 검증: pre/post-move selected-Step round-trip, current-source Edge Based
  Matching Tool UI, Debug build(0 warning/0 error), visual inspection,
  readiness, `git diff --check` PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_EDGE_BASED_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 다음 구조 우선순위는 FeatureMatching의 selected-Step create/apply
  baseline을 먼저 정의하고 통과시킨 뒤에만 standalone owner 후보로
  평가하는 것입니다. single LineGauge와 Mean은 focused gate가 생기기
  전까지 분리하지 않습니다. Recommended model: gpt-5.6-terra |
  Reasoning effort: medium.

### Feature Matching Property Adapter Update (2026-07-26)

- production 이동 전에 기존 Feature Matching Tool smoke를
  `FeatureTool`/`SiftTool` alias, canonical apply, XML/Step 이름,
  input/output layer, acceptance metadata, Lowe ratio, RANSAC, template
  path, threshold, ROI round-trip으로 확장하고 기존 partial 상태에서
  먼저 통과시켰습니다.
- `VisionPipelineFeatureMatchingPropertyAdapter`가
  FeatureMatching/Feature/Sift alias, parameter/default projection,
  PropertyGrid 모델, Step 재생성, metric 판정의 실제 owner가
  되었습니다.
- root mapper에서는 직접 ToolType/metric case와 private 모델을
  제거하고 adapter create/apply/metric dispatch만 유지합니다. 기존
  partial은 제거됐으며 새 adapter는 partial이 아닙니다.
- 검증: pre/post-move selected-Step round-trip, current-source Feature
  Matching Tool UI, Debug build(0 warning/0 error), visual inspection,
  readiness, `git diff --check` PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_FEATURE_MATCHING_PROPERTY_ADAPTER_REFACTOR_PROOF_20260726.md`.
- 남은 direct family는 single LineGauge와 Mean뿐이지만, 둘 다 현재
  focused selected-Step 유지보수 필요와 baseline이 없습니다. 숫자나
  파일 크기를 줄이기 위한 추출은 중단하고 실제 변경 필요가 생길
  때만 다시 평가합니다. Prerequisite: concrete mapper maintenance need
  and focused selected-Step round-trip gate | Recommended model: none until
  evidence exists | Reasoning effort: none until evidence exists.

### Line Property Adapter Consolidation Update (2026-07-26)

- 사용자의 명시적 계속 요청에 따라 남은 두 direct family를
  재감사했습니다. `LineGauge + Mean` 묶음은 domain 응집도가 없어
  폐기하고, single `LineGauge`를 이미 `LineDistance`/
  `LineIntersection`을 소유한 Line adapter에 합쳤습니다.
- production 이동 전에 실제 Tool View-generated `LineGauge` Step으로
  `LineTool`/`LineGaugeTool`, canonical apply, 모든 single-Line
  파라미터, metadata/layer round-trip을 통과했고 기존 Line Pair
  PropertyGrid도 다시 통과했습니다.
- `VisionPipelineLinePairPropertyAdapter`는
  `VisionPipelineLinePropertyAdapter`로 명명·확장되어 single/pair Line
  mapping과 기존 `TryCreateLineGaugePair` handoff를 모두 소유합니다.
- root mapper는 1,263줄에서 1,150줄로 감소했고 Line direct case/model/
  metric을 더 이상 소유하지 않습니다. `Mean`은 의도적으로 root에
  남겼으며 새 interface/factory/adapter를 만들지 않았습니다.
- 검증: pre/post single Line, Line Pair, P213 Geometry PropertyGrid/Review,
  Debug build(0 warning/0 error), current-source visual inspection,
  readiness, `git diff --check` PASS.
- Evidence:
  `docs/admin/OPENVISIONLAB_LINE_PROPERTY_ADAPTER_CONSOLIDATION_REFACTOR_PROOF_20260726.md`.
- 남은 direct OpenCV family는 `Mean` 하나뿐입니다. “switch case를 0개로
  만들기” 위한 one-case adapter는 만들지 않습니다. Prerequisite:
  concrete Mean mapper maintenance need and focused selected-Step round-trip
  gate | Recommended model: none until evidence exists | Reasoning effort:
  none until evidence exists.

### Transform Property Model Ownership Closure (2026-07-26)

- 마감 감사에서 기존 Transform adapter가 create/apply를 소유하면서도
  `PipelineRotateScaleToolProperty`,
  `PipelineAffineTransformToolProperty`, detected-Point converter, metric
  판정을 root mapper의 nested type으로 역참조하는 남은 결합을
  확인했습니다.
- 두 Transform PropertyGrid 모델, `PipelinePointFeatureConverter`,
  Transform metric 판정을 기존
  `VisionPipelineTransformPropertyAdapter`로 이동했습니다. 새 파일,
  interface, factory, registry는 추가하지 않았습니다.
- root mapper는 1,150줄에서 824줄로 감소했고 더 이상 Transform 전용
  모델/converter/type 판정을 소유하지 않습니다. partial 구현도
  존재하지 않으므로 class 선언의 `partial`을 제거했습니다.
- Affine contract, RotateScale/Affine/P219 current-source UI, Debug build,
  visual inspection, readiness, `git diff --check`를 완료 gate로
  유지합니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_TRANSFORM_PROPERTY_MODEL_OWNERSHIP_REFACTOR_PROOF_20260726.md`.
- mapper 분해 캠페인의 남은 direct family는 작은 `Mean` 한 건뿐이며
  추가 추출 우선순위는 없습니다. Prerequisite: concrete Mean mapper
  maintenance need and focused selected-Step round-trip gate | Recommended
  model: none until evidence exists | Reasoning effort: none until evidence
  exists.

### Learn Binary Simulation Model Update (2026-07-26)

- 오래된 후보 목록을 현재 소유권과 재대조했습니다. CommandSurface는
  여전히 크지만 Recipe/Pipeline CRUD, exchange, run-history projection,
  validation/storage의 기존 UseCase/Presenter가 확인됐고, 줄 수만으로
  추가 partial을 만들 응집 경계는 선택하지 않았습니다. Pipeline Review
  역시 execution controller와 presenter가 이미 존재합니다.
- 반면 `OpenVisionLearnWindow.xaml.cs`는 Morphology/Blob/Contour의 순수
  이진영상 계산을 WPF view가 직접 소유해 명확한 MVVM 경계가
  남아 있었습니다.
- 새 비-WPF `OpenVisionLearnBinarySimulationModel`이 erosion/dilation,
  connected-component flood fill, contour, bounds, bound-edge 계산을
  소유합니다. View는 sample/mode 전달, timer/control state, rendering만
  유지합니다.
- 전후 current-source Morphology/Blob/Contour UI, Debug build, readiness,
  visual inspection, `git diff --check`를 완료 gate로 사용했습니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_LEARN_BINARY_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md`.
- 다음 구조 후보는 Learn의 Matching/FeatureMatching 점수 계산을 하나의
  응집된 비-WPF simulation owner로 옮길 가치가 있는지 감사하는
  것입니다. 이벤트/타이머 partial 분리는 하지 않습니다. Recommended
  model: gpt-5.6-terra | Reasoning effort: medium.

### Learn Matching Simulation Model Update (2026-07-26)

- Matching/FeatureMatching의 고정 샘플, 후보 위치, 점수 계산, 최고 후보
  선택, descriptor Good Match 분류, required-count 판정이 WPF View에
  함께 있음을 확인했습니다.
- 새 비-WPF `OpenVisionLearnMatchingSimulationModel`이 두 lesson의
  scenario data와 typed evaluation result를 소유합니다. View는 slider
  값 전달, timer/control state, 설명 문구, painting만 유지합니다.
- production 이동 전에 오래된 영어 animation 문구와 실제 초기
  FeatureMatching panel에 없는 `ResultCount`를 요구하던 UI smoke
  assertion을 현재 한국어/`GoodMatches` 계약으로 정정했습니다.
- 전후 Matching/FeatureMatching current-source UI는 모두 통과했고 두
  PNG가 각각 byte-identical이었습니다. Debug build(0 warning/0 error),
  readiness, visual inspection, `git diff --check`도 통과했습니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_LEARN_MATCHING_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md`
  및
  `artifacts/refactor_learn_matching_simulation_model_20260726`.
- 다음 구조 감사 후보는 Learn의 Line/LineDistance 계산이 하나의 응집된
  비-WPF simulation owner인지 확인하는 것입니다. 숫자 축소용
  event/timer/rendering partial은 만들지 않습니다. Recommended model:
  gpt-5.6-terra | Reasoning effort: medium.

### Learn Line Simulation Model Update (2026-07-26)

- Edge/Line의 5x5 GV gradient/edge/run 계산과 LineDistance의 edge-pair
  거리/평균/range/mm/gate 계산이 같은 선 기반 측정 lesson 책임이며,
  View에서 각각 중복 계산되고 있음을 확인했습니다.
- 새 비-WPF `OpenVisionLearnLineSimulationModel`이 두 scenario data와
  typed evaluation result를 소유합니다. View는 slider 값 전달,
  timer/control state, 설명 문구, painting만 유지합니다.
- UI smoke를 정확한 `threshold 85`, `LineRun 5 px`, `4.2 px`, range `1`,
  mm `0.025/0.006/0.030` 계약까지 강화했습니다.
- 첫 post-move build는 이전 `strengths` 변수 참조 한 곳을 찾아
  실패했고 수정 후 0 warning/0 error로 통과했습니다. 전후/current
  source UI, readiness, 구조 검색, visual inspection,
  `git diff --check`도 통과했습니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_LEARN_LINE_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md`
  및 `artifacts/refactor_learn_line_simulation_model_20260726`.
- 다음 구조 감사 후보는 Brightness/Arithmetic/Filtering이 하나의
  basic grayscale simulation owner로 응집되는지 확인하는 것입니다.
  Metrics Acceptance, Color/HSV, event/timer/rendering은 숫자나 대칭을
  위해 분리하지 않습니다. Recommended model: gpt-5.6-terra |
  Reasoning effort: medium.

### Learn Basic Grayscale Simulation Model Update (2026-07-26)

- Brightness, Arithmetic, Filtering은 모두 고정 grayscale sample에 대한
  pixel/kernel 변환과 통계 계산으로 하나의 응집된 lesson 책임임을
  확인했습니다.
- 새 비-WPF `OpenVisionLearnBasicGrayscaleSimulationModel`이 brightness
  result/histogram/average, 다섯 arithmetic 결과, Mean/Median/Sharpen
  결과와 정렬/합계를 소유합니다. View는 selection/timer/control state,
  설명 문구, painting만 유지합니다.
- production 이동 전에 손상되거나 폐기된 초기 문구와 이전
  `Preview/Run` animation status를 찾던 Brightness/Arithmetic smoke를
  현재 안정 계약으로 정정했습니다.
- 정확 수치 smoke, 세 current-source UI, Debug build(0 warning/0 error),
  readiness, 구조 검색, visual inspection, `git diff --check`가
  통과했습니다. 세 before/after PNG가 각각 byte-identical입니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_LEARN_BASIC_GRAYSCALE_SIMULATION_MODEL_REFACTOR_PROOF_20260726.md`
  및 `artifacts/refactor_learn_basic_grayscale_model_20260726`.
- 다음 단계는 남은 Threshold/Metrics/Layer Recipe/Transform/Color 계산의
  마감 감사입니다. 응집 경계가 없다면 proactive Learn extraction을
  종료하고 generic grab-bag model이나 event/timer/rendering partial을
  만들지 않습니다. Recommended model: gpt-5.6-terra | Reasoning effort:
  medium.

### Learn Model Extraction Closure (2026-07-26)

- 마감 감사 결과 Threshold는 별도 모델이 아니라 기존 basic grayscale
  sample/clamp/pixel transform 책임과 동일했습니다.
  `OpenVisionLearnBasicGrayscaleSimulationModel.EvaluateThreshold`로
  통합했고 View의 `sampleValues`를 제거했습니다.
- Metrics Acceptance는 한 animation용 5-value 통계, Layer Recipe는
  교육용 routing row/셀 강조, Geometry는 WPF Transform 상태,
  Color/HSV는 WPF Color/brush/channel painting과 결합돼 있어 별도
  비-WPF owner가 되지 않습니다.
- Threshold current-source UI, Debug build(0 warning/0 error), readiness,
  구조 검색, visual inspection, `git diff --check`가 통과했습니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_LEARN_MODEL_EXTRACTION_CLOSURE_20260726.md`
  및 `artifacts/refactor_learn_threshold_model_closure_20260726`.
- proactive Learn calculation extraction은 종료합니다. 구체적 유지보수
  변경, 재현된 regression, 두 번째 비-WPF consumer가 생기기 전에는
  Metrics/Layer Recipe/Geometry/Color/HSV/event/timer/rendering을 더
  분리하지 않습니다. Prerequisite: concrete evidence | Recommended
  model: none until evidence exists | Reasoning effort: none until evidence
  exists.

### Auto MPoint Teaching Controller Update (2026-07-26)

- 저장소 전체의 큰 WPF code-behind를 현재 owner와 다시 대조했습니다.
  ROI Editor, OpenGL Template Editor, Pipeline Review, Learn은 각각
  기존 ViewModel/service/controller 또는 WPF-local 책임이 확인되어
  줄 수 기준 분해를 중단했습니다.
- `EdgeBasedMatchingToolWpfView`만 source/대표 이미지 수명, Auto MPoint
  실행, 후보 상태, 분석 identity, HTML report, template 적용을 직접
  소유하고 있었습니다.
- 새 `AutoMPointTeachingController`가 이 교육 워크플로와 상태의 실제
  owner가 됐고 View는 구성, verification guide 표시, 기존 facade 위임만
  유지합니다. 새 interface/factory/partial은 추가하지 않았습니다.
- Auto MPoint 전용 및 일반 Edge Based Tool current-source smoke, Debug
  build, readiness, 구조 검색, visual inspection, `git diff --check`가
  통과했습니다.
- Evidence:
  `docs/admin/OPENVISIONLAB_AUTO_MPOINT_TEACHING_CONTROLLER_REFACTOR_PROOF_20260726.md`
  및 `artifacts/refactor_auto_mpoint_teaching_controller_20260726`.
- 다음 구조 우선순위는 없습니다. 구체적인 유지보수 변경 또는 현재
  빌드 회귀가 새 owner 필요성을 보일 때만 재감사합니다.
  Prerequisite: concrete evidence | Recommended model: none until evidence
  exists | Reasoning effort: none until evidence exists.
