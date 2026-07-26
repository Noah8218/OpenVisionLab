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
