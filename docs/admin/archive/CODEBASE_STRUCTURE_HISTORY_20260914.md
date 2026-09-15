# OpenVisionLab Codebase Structure

이 문서는 OpenVisionLab 코드베이스의 상위 구조를 빠르게 파악하기 위한 입구 문서입니다.
세부 구현 이력보다 "어느 책임이 어디에 있는지", "새 툴을 추가하거나 버그를 추적할 때 어디서 시작해야 하는지"에 초점을 둡니다.

## 0. 완료 경계 확인과 중복 작업 방지

구조 변경 전에 아래 기존 소유자와 연결된 완료 기록을 확인합니다. 새로운 모델이
읽기 편하다는 이유나 파일 길이만으로 다시 분리·이동·래핑하지 않습니다.
기존 호출 경로가 유지되고 새 결함·요구사항·책임 충돌이 없으면 그대로 재사용합니다.

| 유지할 소유자/경계 | 확인할 증거 |
| --- | --- |
| Learn Matching/FeatureMatching Presenter·View | [완료 기록](../../reports/OPENVISIONLAB_LEARN_MATCHING_VIEW_20260908.md), 5.3 |
| Learn Layer/Recipe Presenter·View | [완료 기록](../../reports/OPENVISIONLAB_LEARN_LAYER_RECIPE_VIEW_20260908.md), 5.4 |
| Learn Metrics/Acceptance Presenter·View | [완료 기록](../../reports/OPENVISIONLAB_LEARN_METRICS_ACCEPTANCE_VIEW_20260908.md), 5.5 |
| Learn Binary / Line Presenter·View (다섯 주제) | [완료 기록](../../reports/OPENVISIONLAB_LEARN_BINARY_LINE_BATCH_20260908.md), 5.6; 기존 SimulationModel 재사용 |
| Learn Grayscale Presenter·View (네 주제) | [완료 기록](../../reports/OPENVISIONLAB_LEARN_GRAYSCALE_BATCH_20260908.md), 5.7; 기존 Grayscale 계산 Model 재사용 |
| Learn Foundation Presenter·View (Point/ROI·Mat/채널) | [완료 기록](../../reports/OPENVISIONLAB_LEARN_FOUNDATION_BATCH_20260908.md), 5.8; 기존 완료 owner 재분리 없음 |
| Learn Geometry Transform Presenter·View | [완료 기록](../../reports/OPENVISIONLAB_LEARN_GEOMETRY_BATCH_20260908.md), 5.9; 기존 Transform/Recipe 계약 유지 |
| 기존 Learn SimulationModel 4개, LearnResources·LearnCellVisuals | 기존 계산·공유 UI 소유자. 이번 [묶음 기록](../../reports/OPENVISIONLAB_LEARN_BINARY_LINE_BATCH_20260908.md)에서 변경 전후 동일성 확인 |
| Recipe 실행 세션·실행 owner 경계 | [기존 구조](../../reports/OPENVISIONLAB_RECIPE_EXECUTION_STRUCTURE_20260908.md), [Local validation-set owner](../../reports/OPENVISIONLAB_OVL16_RECIPE_LOCAL_VALIDATION_EXECUTION_OWNER_20260908.md), [Catalog owner](../../reports/OPENVISIONLAB_OVL15_RECIPE_CATALOG_EXECUTION_OWNER_20260908.md), [Good/Bad pair owner](../../reports/OPENVISIONLAB_OVL14_RECIPE_PAIR_EXECUTION_OWNER_20260908.md), [선택 샘플 owner](../../reports/OPENVISIONLAB_OVL13_RECIPE_SELECTED_SAMPLE_EXECUTION_OWNER_20260908.md) |
| Recipe Validation·Step Edit의 독립 owner | [Validation](../../reports/OPENVISIONLAB_OVL07_VALIDATION_RESPONSIBILITY_20260907.md), [Step Edit](../../reports/OPENVISIONLAB_OVL07_STEP_EDIT_APPLY_OWNER_20260907.md) |
| Recipe Workspace lifecycle·summary·pipeline option projection | [Lifecycle](../../reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_LIFECYCLE_POLICY_20260907.md), [Summary](../../reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_SUMMARY_PROJECTION_20260907.md), [Options](../../reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_PIPELINE_OPTION_PROJECTION_20260908.md) |
| Pipeline Review 결과·guide·domain evidence·image owner | [결과](../../reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_RESULT_STATUS_PROJECTION_20260908.md), [Guide](../../reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_GUIDE_RESULT_PROJECTION_20260908.md), [Domain](../../reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_DOMAIN_EVIDENCE_PROJECTION_20260908.md), [Image](../../reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_LAYER_IMAGE_OWNER_20260908.md) |
| Pipeline Review Document revision gate | [완료 기록](../../reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_DOCUMENT_REVISION_GUARD_20260908.md), 9.2 |
| Recipe Run History inventory·baseline·comparison orchestration | [완료 기록](../../reports/OPENVISIONLAB_OVL07_RECIPE_RUN_HISTORY_ORCHESTRATION_20260908.md), 9.3 |
| Recipe run evidence image decode owner | [현재 P1 owner 기록](../../reports/OPENVISIONLAB_P1_VIEW_BOUNDARIES_20260913.md), 9.48 |
| Line Tool property persistence owner | [현재 P1 owner 기록](../../reports/OPENVISIONLAB_P1_VIEW_BOUNDARIES_20260913.md), 9.49 |
| RoiImageCanvasViewModel boundary proof | [현재 P1 owner 기록](../../reports/OPENVISIONLAB_P1_VIEW_BOUNDARIES_20260913.md), 9.50 |
| Partial 59 responsibility review | [Partial 구조 책임 검토](../../reports/OPENVISIONLAB_PARTIAL_RESPONSIBILITY_REVIEW_20260913.md), 9.51 |
| Partial structural elimination (PL-0028) | [Partial 구조 제거 계획](../../reports/OPENVISIONLAB_PARTIAL_STRUCTURAL_ELIMINATION_PLAN_20260913.md), 9.52 |
| OVL-06b 정량 감사 instrumentation | [완료 기록](../../reports/OPENVISIONLAB_OVL06B_QUANTITATIVE_AUDIT_20260908.md), 0.1 |
| ImageCanvas Mat 저장 정책 | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_SAVE_OWNER_20260908.md), 9.4 |
| ImageCanvas SaveFileDialog host | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_SAVE_DIALOG_HOST_20260908.md), 9.5 |
| ImageCanvas OpenFileDialog host | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_OPEN_DIALOG_HOST_20260908.md), 9.5 |
| ImageCanvas ContextMenu host | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_CONTEXT_MENU_HOST_20260908.md), 9.6 |
| ImageCanvas WinForms keyboard input owner | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_KEYBOARD_INPUT_20260908.md), 9.7 |
| ImageCanvas WPF keyboard input owner | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_WPF_KEYBOARD_INPUT_20260908.md), 9.8 |
| ImageCanvas mouse input owner | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_MOUSE_INPUT_20260908.md), 9.9 |
| Learn Window/Shell host owner | [완료 기록](../../reports/OPENVISIONLAB_OVL09_LEARN_WINDOW_HOST_20260908.md), 9.10 |
| Learn Window topic presentation policy | [완료 기록](../../reports/OPENVISIONLAB_OVL09_LEARN_TOPIC_PRESENTATION_20260908.md), 9.11 |
| ImageCanvas Directory policy owner | [완료 기록](../../reports/OPENVISIONLAB_OVL11_IMAGECANVAS_DIRECTORY_POLICY_20260908.md), 9.12 |
| Representative WPF runtime qualification | [완료 기록](../../reports/OPENVISIONLAB_WPF_RUNTIME_QUALIFICATION_20260908.md), 9.13 |
| Shell LLM draft active-baseline review owner | [완료 기록](../../reports/OPENVISIONLAB_OVL45_LLM_DRAFT_REVIEW_OWNER_20260909.md), 9.37 |
| Shell PinArrayGap validation identity owner | [완료 기록](../../reports/OPENVISIONLAB_OVL46_PINARRAYGAP_VALIDATION_IDENTITY_OWNER_20260909.md), 9.38 |
| Validation Set status presentation owner | [완료 기록](../../reports/OPENVISIONLAB_OVL47_VALIDATION_SET_STATUS_PRESENTER_20260909.md), 9.39 |
| Validation Set save-failure status owner | [완료 기록](../../reports/OPENVISIONLAB_OVL48_VALIDATION_SET_SAVE_STATUS_OWNER_20260909.md), 9.40 |
| PinArrayGap frozen split-name restoration owner | [완료 기록](../../reports/OPENVISIONLAB_OVL49_PINARRAYGAP_SELECTION_RESTORATION_OWNER_20260909.md), 9.41 |
| Validation Set evidence `PropertyChanged` projection owner | [완료 기록](../../reports/OPENVISIONLAB_OVL50_VALIDATION_SET_EVIDENCE_NOTIFICATION_OWNER_20260909.md), 9.42 |
| Validation Set projection error branch owner | [완료 기록](../../reports/OPENVISIONLAB_OVL51_VALIDATION_SET_PROJECTION_ERROR_OWNER_20260909.md), 9.43 |
| Validation Set success projection owner | [완료 기록](../../reports/OPENVISIONLAB_OVL52_VALIDATION_SET_SUCCESS_PROJECTION_OWNER_20260909.md), 9.44 |
| RefreshOptions composite command-state owner | [완료 기록](../../reports/OPENVISIONLAB_OVL53_REFRESH_OPTIONS_COMMAND_STATE_OWNER_20260909.md), 9.45 |
| Recipe 저장 결과 전달 / Pipeline 실패 결과 수명 | [신뢰성 검토](../../reports/OPENVISIONLAB_ARCHITECTURE_RELIABILITY_REVIEW_20260909.md), 1.1; 기존 저장소·execution service 안의 P0 수정 |
| AppPathService runtime path containment | [완료 기록](../../reports/OPENVISIONLAB_APPPATH_BOUNDARY_REVIEW_20260910.md); 기존 static owner와 `CombineUnderRoot` 경계, package-relative fallback 보존 |

재개가 필요하면 변경 전에 다음을 작업 기록에 작성합니다.

```text
기존 완료 기록 / 현재 소유자:
새 실패 또는 변경된 명시적 요구사항:
현재 코드 / 재현 절차 / 실패 검증:
기존 소유자 안의 수정으로 해결할 수 없는 이유:
최소 변경 경계 / 보존할 계약:
집중 검증과 종료 조건:
```

예: 아직 Window에 남은 Binary 화면 상태를 옮기는 작업은 기존 BinarySimulationModel을
다시 나눌 이유가 아닙니다. 계산은 그 모델을 호출하고, 새로 분리한 상태·화면만 검증합니다.
완료 후 새 소유자도 이 표에 등록합니다. 상세 이력을 이 문서에 복제하지 않습니다.

현재 전수조사와 단계별 종료 기준은
[리팩토링 프로그램 감사 보고서](../../reports/OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md)에
있습니다. 이 보고서의 P0~P3 순서와 완료 gate를 먼저 확인하고, 이미 표에
등록된 owner는 새 결함·명시 요구사항 변경·책임 충돌이 없는 한 다시 나누지
않습니다.

### 0.1 반복 가능한 정량 감사 실행

현재 기준선을 다시 만들 때는 수동 검색을 조합하지 말고 아래 단일 진입점을
사용합니다.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File `
  .\tools\RefactorAudit\Invoke-RefactorAudit.ps1 `
  -RepositoryRoot C:\Git\2D\Dev `
  -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-audit-current `
  -Verify
```

스크립트는 `bin`·`obj`를 제외한 `src`·`tools`를 읽고
`source-survey.json`, CSV 목록, `source-survey-summary.txt`,
`project-cycle-check.txt`를 출력합니다. `-Verify`는 프로젝트 순환과 Shell의
Run History 저장소 직접 호출을 회귀 gate로 사용하고, `RoiImageCanvasViewModel`
UI/IO 결합 신호가 계속 보고되는지도 확인합니다. 증거는 D:에 남기며 C: 경로는
`-AllowNonDOutput`를 명시하지 않으면 거부됩니다. 이 도구는 제품 상태를
변경하지 않습니다. 구현 결과와 실제 수치는
[OVL-06b 완료 보고서](../../reports/OPENVISIONLAB_OVL06B_QUANTITATIVE_AUDIT_20260908.md)를
참조합니다.

## 1. 제품 관점의 큰 흐름

OpenVisionLab은 여러 이미지 레이어를 중심으로 동작하는 rule-based vision workbench입니다.
사용자는 `Main` 같은 입력 레이어에 이미지를 로드하고, 각 툴에서 입력/출력 레이어를 선택한 뒤, 파라미터를 조정하면서 결과를 미리 보고 비교합니다.

핵심 방향성은 다음과 같습니다.

- 레이어는 독립적으로 선택되고 비교되어야 합니다.
- 출력 레이어 생성이 입력 레이어를 강제로 바꾸면 안 됩니다.
- 툴 내부 input/output preview를 클릭하면 메인 작업영역도 해당 레이어를 보여야 합니다.
- Blob, Contour, Matching 등 알고리즘 툴은 PropertyGrid 기반 구조를 유지합니다.
- 새 툴이 늘어나도 반복 배선은 줄이고, 툴 고유 동작은 숨기지 않습니다.

```text
이미지 레이어
  -> 툴 선택
  -> 입력/출력 레이어 라우팅
  -> 파라미터 편집
  -> 사용자의 명시적 Preview/Run
  -> 결과 레이어 갱신
  -> 메인 작업영역/툴 preview/레이어 목록 동기화
  -> 파이프라인 step 저장
```

### 1.1 Image → Layer → Tool → Inspection → Pipeline → Recipe → Result → Review

프로젝트 열기/실행은 기존 [README Start Here](../../README.md#start-here-choose-the-project)를
사용합니다. 여기서는 그 다음 코드 읽기 순서와 상태·해제 소유자를 연결합니다.
전체 경로를 찾는 한 번의 검색은 `rg -n "1.1 Image" docs/admin/CODEBASE_STRUCTURE.md`입니다.

| 읽는 순서 | 검색할 실제 진입점 | 생성·상태 변경·해제 책임 |
| --- | --- | --- |
| 시작 | `Program.Main -> OpenVisionLabApplication.Run -> ApplicationRuntimeContext -> OpenVisionShellHostWindow` | Bootstrap이 context와 Shell을 조합. 기본 context/DisplayManager는 앱 수명의 공유 instance이며, View는 이를 Dispose하지 않고 Bootstrap이 `Application.Run` 반환 후 해제한다. 주입된 per-session manager는 기존 View/session owner가 해제 |
| Image | `CanvasImageLoader`, `BitmapImageConverter`, `RoiImageCanvasViewModel.LoadImage` | loader/converter가 Mat 생성, Canvas는 clone을 보유하고 교체/clear에서 해제. 반환된 변환 Mat은 호출자가 using으로 해제 |
| Layer | `DisplayManagerService -> DisplayLayerStore / ImageSpaceService` | DisplayManager가 ImageSpace를 생성. metadata와 이미지/ROI 상태를 나눠 소유. Lease.Dispose -> 참조수 release, 최종 image Dispose |
| Tool | `OpenVisionNativeToolRegistry -> OpenVisionNativeToolDocument -> ViewModel/PropertyGrid` | 기존 factory가 document를 생성. document가 route/preview/controller와 구독 수명을 묶고 Dispose에서 해제 |
| Inspection | `VisionPipelineAppToolFactory.Create -> VisionPipelineExecutionService.ExecuteStep` | SDK 또는 기존 앱 Tool adapter 생성, ExecuteStep의 using이 Tool 수명을 소유. 검사 정책을 View에 복제하지 않음 |
| Pipeline | `VisionPipelineExecutionPlan.Create -> RunPreparedAsync -> RunPreparedStepsAsync` | plan은 실행 정의 snapshot, steps는 순서/fixture/acceptance/context write. Step 입력 Mat은 using, 실패 결과는 RunPreparedAsync, 성공 결과는 caller가 해제 |
| Recipe | `RecipeState.Name/LoadTools -> RecipeRuntimeStorage.Load -> VisionToolRepository.LoadTools -> VisionToolStorage`, 저장은 `RecipeState.SaveTools -> RecipeRuntimeStorage.Save -> VisionToolRepository.SaveTools` | Load는 임시 repository에서 성공한 Tool set만 기존 list에 반영하고 실패 시 identity/Data/event를 보존하며 임시 템플릿 Mat을 정리한다. Save는 VISION Tool XML snapshot/rollback 후 Data를 저장하며 false는 기존 Shell 저장 실패 branch로 전달. SerializeHelper는 파일별 temp/replace 수명을 계속 소유 |
| Result | `VisionRecipeRunner.CreateResult`, `VisionPipelineResultSummaryService`, `VisionPipelineRunReportStorage` | 실행별 결과/summary와 persisted report. VisionRecipeRunResult.Dispose가 호출자에게 넘긴 최종 Mat 해제 |
| Review | `OpenVisionPipelineReviewDocument -> OpenVisionPipelineReviewExecutionController -> ViewModel/Presenter` | Document가 controller 생성/해제. controller가 stamp·취소·review cache, revision gate가 현재 문서 투영을 소유 |

실패 추적 예: `SaveSelectedRecipe -> RecipeState.SaveTools -> RecipeRuntimeStorage.Save`;
로드 실패는 `RecipeState.Name -> RecipeRuntimeStorage.Load -> VisionToolRepository.LastStorageError`에서
중단되고, `VisionToolStorage` 후반 저장 실패는 VISION snapshot 복구 후 원래 예외를 전달한다.
에서 false가 어디서 생겼는지 보고 `VisionToolRepository.LastStorageError`와 해당 XML을 확인합니다.
검사 callback 예외는 `RunPreparedAsync`에서 반환 전 결과를 해제한 뒤 같은 예외가
기존 caller로 전달됩니다. 성공 결과를 받았다면 해제 책임도 그 caller로 이동합니다.
Recipe Data/Pipeline까지 포함한 전체 다중 파일 transaction과 실제 UI/장시간 검증의 남은
경계는 [2026-09-09 검토](../../reports/OPENVISIONLAB_ARCHITECTURE_RELIABILITY_REVIEW_20260909.md)에 있습니다.

## 2. 루트 디렉터리 지도

| 경로 | 역할 |
| --- | --- |
| `src/OpenVisionLab/Program.cs` | WPF 애플리케이션 진입점입니다. |
| `src/OpenVisionLab/OpenVisionLab.csproj` | 메인 WPF 앱 프로젝트입니다. `net8.0-windows7.0`, WPF 사용, x64 중심입니다. |
| `Directory.Build.props` | 저장소 공통 MSBuild 경로와 vendored DLL root를 정의합니다. |
| `src/OpenVisionLab/UI/` | 현재 WPF UI, Shell, tool view, popup, teaching panel 중심 코드입니다. |
| `src/OpenVisionLab/Core/` | application, recipe, pipeline, storage, state service가 위치하는 핵심 영역입니다. |
| `src/OpenVisionLab/Common/` | 호환성을 유지한 공통 타입의 책임별 하위 폴더입니다. `Runtime`, `Imaging`, `PropertyGrid`, `Recipe`, `Persistence`, `Results`, `Events`, `MessageDialogs`를 먼저 확인하고, 아직 owner가 넓은 레거시 공통 타입만 루트에 남깁니다. |
| `src/OpenVisionLab/Vision/` | OpenCV 기반 vision tool property/model/wrapper 계층입니다. |
| `src/OpenVisionLab/Property/` | 저수준 property container와 PropertyGrid 연결 모델이 위치합니다. |
| `src/Libraries/` | 재사용 가능한 분리 라이브러리 프로젝트들입니다. MVVM, 이미지 캔버스, 레이어 core, logging, localization 등을 포함합니다. |
| `tools/` | UI smoke, contract check, recipe runner 등 검증/보조 실행 프로젝트입니다. |
| `docs/` | 설계 문서, 운영 문서, smoke 정책, extension guide가 위치합니다. |
| `dll/` | vendored runtime DLL 위치입니다. OpenVisionLab Vision SDK 3.0, 공용 OpenCvSharp native runtime, WPG PropertyGrid DLL 참조가 여기서 해결됩니다. |
| `Sample/` | 로컬/vendor 샘플 참고 영역입니다. 공개 배포나 GitHub 원본 반영 대상이 아니며, 공개 검증 자산은 `docs/samples/public/`와 `docs/samples/public/product/`를 사용합니다. |
| `scripts/` | 보조 스크립트 영역입니다. |
| `bin/`, `obj/`, `artifacts/`, `dist/`, `.vs/`, `.codex/`, `.codex-temp/`, `tmp/` | 소스가 아닌 로컬/생성 데이터용 junction mount입니다. 외부 D: 저장소를 가리키며 기본 Explorer 보기에서는 숨겨집니다. 논리 경로는 기존 빌드·스모크 계약을 위해 유지합니다. 상세 target과 복원 규칙은 [Root Layout](../OPENVISIONLAB_ROOT_LAYOUT_20260910.md)을 참고합니다. |
`ParameterPropertyStorage`는 `OpenVisionLab.Property`에 속한 내부 Recipe persistence owner이고, 공개 XML 모델 `OpenVisionLab.ParameterProperty`는 호환성을 위해 기존 namespace에 남아 있습니다.

### 2.2 Common 책임별 하위 폴더

`src/OpenVisionLab/Common/`은 공통이라는 이유만으로 모든 클래스를 한곳에
두지 않습니다. 현재 physical path는 namespace와 공개 타입 이름을 유지한 채
책임별로 다음과 같이 읽습니다.

| 경로 | 먼저 읽을 owner |
| --- | --- |
| `Common/Runtime/` | `AppPathService`, `BackgroundLoopWorker` — runtime path, worker lifetime/shutdown |
| `Common/Imaging/` | `BitmapDrawing`, `BitmapImageConverter` — Bitmap/Mat 표시 변환 |
| `Common/PropertyGrid/` | PropertyGrid editor contract/runtime/policy |
| `Common/Recipe/` | `RecipeModel` |
| `Common/Persistence/` | `SerializeHelper` — atomic XML load/save |
| `Common/Results/` | `DefectList`, `DefectListResult`, `EdgeLineList` |
| `Common/Events/` | `VisionEventArgs` |
| `Common/MessageDialogs/` | `VisionMessageBox` — WPF message-dialog adapter |
| `Common/Account/` | `CAccountManager` |
| `Common/` root | `AppCommon`, `CCommon`, `DEFINE` — owner가 여러 기능에 걸친 legacy shared surface |

이번 배치는 physical path만 바꾸고 namespace, public type, Recipe/XML,
XAML, SDK 계약을 유지했습니다. 상세 전후 목록과 검증은
[Common 폴더 정리 보고서](../../reports/OPENVISIONLAB_COMMON_FOLDER_REORGANIZATION_20260910.md)를
참조합니다.

### 2.1 Docking.Controls 물리 배치

`src/Libraries/OpenVisionLab.Docking.Controls/`는 public contract, document
state, workspace lifecycle, guide policy, layer docking, converter, WPF View를
책임별 하위 폴더로 구분합니다. 이동은 physical path만 바꾸고
`OpenVisionLab.Docking.Controls` namespace, public type 이름, XAML binding과
AvalonDock package 경계를 유지합니다. 상세 owner/call path와 전후 정량
감사는 [2026-09-10 폴더 감사](../../reports/OPENVISIONLAB_FOLDER_ORGANIZATION_AUDIT_20260910.md)를
참조합니다.

| 경로 | 책임 |
| --- | --- |
| `Contracts/` | Shell/library 공개 docking 계약 |
| `Models/` | document/workspace 상태·레이아웃·snapshot |
| `Documents/` | document projection·state 동기화 |
| `Workspace/` | composition·layout·move·cleanup·save lifecycle |
| `Guides/` | guide policy·parser·presenter·진단 |
| `LayerDocking/` | layer docking command/gesture |
| `Converters/` | XAML 값 변환 |
| `Views/` | WPF View, code-behind, resource dictionary |

## 3. 메인 앱과 Shell 구조

메인 작업영역은 `src/OpenVisionLab/UI/Menu/Wpf` 아래에 집중되어 있습니다.

| 파일/영역 | 책임 |
| --- | --- |
| `OpenVisionShellHostView.xaml(.cs)` | 메인 WPF Shell view입니다. UI 이벤트의 마지막 얇은 연결층입니다. |
| `OpenVisionShellHost*Controller.cs` | ShellHost의 명령, 문서, 도킹 레이어, 툴 창, 상태 표시 등 책임을 분리한 controller/presenter 계층입니다. |
| `OpenVisionShellHost*Presenter.cs` | Shell UI 표시 상태를 갱신하는 presentation 계층입니다. |
| `OpenVisionNativeToolDocument.cs` | 선택된 native WPF tool 하나의 runtime 문서입니다. 레이어 라우팅, preview, pipeline, command controller를 묶습니다. |
| `OpenVisionNativeToolDocumentCache.cs` | 툴 document 재사용/cache를 담당합니다. 툴 창 표시 속도 개선과 관련됩니다. |
| `OpenVisionNativeToolRegistry.cs` | native WPF tool 등록의 중심입니다. 새 툴 연결은 여기서 시작합니다. |
| `OpenVisionNativeToolPrewarm*.cs` | 툴 표시 속도 개선을 위한 선생성/prewarm 정책과 서비스입니다. |
| `OpenVisionLayerViewerView.xaml(.cs)` | 레이어 단독 보기 창입니다. WPF fallback viewer와 확대/이동을 포함합니다. |
| `OpenVisionZoomableImageController.cs` | WPF 이미지 viewer의 공통 확대/이동/좌표 상태 controller입니다. |
| `Shell/Recipe/OpenVisionShellHostRecipePanelDragController.cs` | Recipe Manager 패널의 마우스 drag 상태, capture 해제, 화면 밖 이탈 제한을 소유합니다. Shell View는 XAML 이벤트를 전달하고 lifecycle에서 Dispose합니다. |
| `Documents/` | Pipeline review 같은 Shell 문서형 화면입니다. |
| `ViewModels/`, `Views/` | Shell 주변 MVVM view model과 view입니다. |

Shell 쪽 변경 시 우선 확인할 것:

- `OpenVisionShellHostView.xaml.cs`에 새 책임을 직접 추가하지 말고, 이미 있는 controller/presenter로 이동할 수 있는지 확인합니다.
- document lifetime은 `OpenVisionNativeToolDocument`, cache, floating window host와 얽혀 있으므로 `Unloaded`에서 무조건 dispose하지 않습니다.
- 메인 작업영역이 어떤 레이어를 보여야 하는지는 `IDisplayManager` 활성 레이어와 workspace presenter 상태가 같이 맞아야 합니다.

### 3.1 Shell에서 Pipeline Review를 읽는 순서

Pipeline Review는 Shell의 일반 부동 Tool 경로와 구분되는 **도킹 문서 경로**로
처음 열립니다. 사용자가 Details와 Step Flow를 펼치는 동작의 상태·크기·표시
정책은 `OpenVisionPipelineReviewLayoutController`가 소유합니다. Review View는
XAML 이벤트를 controller에 전달하고 구독 수명을 정리하는 UI adapter입니다.

| 순서 | 실제 경로 | 확인할 책임 |
| --- | --- | --- |
| 1 | `OpenVisionShellHostView.xaml.cs` | Shell 조합과 `ToolSelectionController` 연결 |
| 2 | `Shell/Tooling/OpenVisionShellHostToolSelectionController.cs` | 선택된 메뉴를 기존 Tool Window controller로 전달 |
| 3 | `Shell/Tooling/OpenVisionShellHostToolWindowController.cs`의 `ShowSelectedTool` → `ShowPipelineReview` | Pipeline Review document restore/create와 도킹 workspace 진입 |
| 4 | `Shell/Documents/OpenVisionShellHostDocumentController.cs` | active/cached Pipeline Review document 상태와 restore |
| 5 | `Shell/Tooling/OpenVisionShellHostToolWindowLifecycleController.cs` | 도킹 문서 표시와 floating/dock 전환 수명주기 |
| 6 | `Documents/OpenVisionPipelineReviewDocument.cs` | Review document 이벤트·실행 controller·revision gate 연결 |
| 7 | `Views/OpenVisionPipelineReviewView.xaml.cs` | XAML 이벤트 전달, View 수명, 공개 View 이벤트 연결 |
| 8 | `Views/OpenVisionPipelineReviewLayoutController.cs` | compact/details/step-flow 상태, 행 높이·표시 정책, 토글 아이콘·tooltip |
| 9 | `ViewModels/OpenVisionPipelineReviewViewModel.cs` | 바인딩 가능한 review/readiness/preview projection과 표시 상태 |
| 10 | `PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs` | 실행 세대·취소·revision callback·Step summary·cached output 이미지 |
| 11 | `Views/OpenVisionPipelineReviewImageResourceOwner.cs` | View가 보유하는 Bitmap/diagnostic resource 교체·Dispose |

```text
ShowSelectedTool(Pipeline)
  -> ShowPipelineReview
  -> TryRestorePipelineReview / CreatePipelineReviewDocument
-> ShowDockedDocumentWorkspace
-> OpenVisionPipelineReviewDocument.View
-> btnReviewDetailsToggle
-> OpenVisionPipelineReviewLayoutController
-> OpenVisionPipelineReviewViewModel / OpenVisionPipelineReviewExecutionController
```

`OpenVisionFloatingToolWindowHost`는 일반 WPF Tool과 도킹 문서의 부동 전환에
필요한 기존 host이며, Pipeline Review를 처음 생성하는 owner가 아닙니다. 실행
세대·stale callback·이미지 수명은 9.2와 Pipeline Review 완료 기록을 함께
읽습니다. 빠른 관련 소스 검색은 다음과 같습니다.

```powershell
rg -n "ShowSelectedTool|ShowPipelineReview|TryRestorePipelineReview|ShowDockedDocumentWorkspace|btnReviewDetailsToggle|OnReviewDetailsToggleChanged" src/OpenVisionLab/UI/Menu/Wpf
```

이 경로는 [`OPENVISIONLAB_JUNIOR_NAVIGATION_AND_SHELL_REVIEW_20260910.md`](../../reports/OPENVISIONLAB_JUNIOR_NAVIGATION_AND_SHELL_REVIEW_20260910.md)의
주니어 탐색성 검토와 동일한 source owner를 사용합니다.

### 3.2 Shell code-behind의 상태·파일 경계

`OpenVisionShellHostView.xaml.cs`는 WPF 이벤트, 탭 이동, 대화상자와
control lifecycle을 조합하는 View adapter로 유지합니다. Recipe 선택과
Workspace 이미지 경로 저장은 View가 직접 persistence하지 않습니다.

| 동작 | concrete owner | View에서 찾을 연결 |
| --- | --- | --- |
| Recipe 이름/마지막 Recipe 저장 | `Shell/Recipe/OpenVisionShellHostRecipeController.cs` | `RecipeCommandSurface`에 `recipeController.SwitchRuntimeRecipe` 전달 |
| Workspace 이미지 경로 기억 | `Shell/Workspace/OpenVisionShellHostWorkspaceImageController.cs` | Workspace/Layer command에 `workspaceImageController.RememberWorkspaceImagePath` 전달 |
| 이미지 로드·Main layer 반영 | 같은 Workspace image controller | `LoadImage` → `ApplyMainLayerImage` |
| WPF 파일 선택·확인·탭 이동 | `OpenVisionShellHostView.xaml.cs` | Window owner와 control event를 직접 다루는 UI 전용 코드 |

이 구분은 새 ViewModel이나 persistence interface를 추가한 것이 아니라 기존
Recipe/Workspace concrete owner에 이미 존재하던 runtime context와 이미지 수명을
연결한 것입니다. 다음 검색으로 직접 확인할 수 있습니다.

```powershell
rg -n "SwitchRuntimeRecipe|RememberWorkspaceImagePath|SaveConfig|LastWorkspaceImagePath" src/OpenVisionLab/UI/Menu/Wpf
```

소유권 변경 기록은 [`OPENVISIONLAB_MVVM_SHELL_STATE_OWNERSHIP_20260910.md`](../../reports/OPENVISIONLAB_MVVM_SHELL_STATE_OWNERSHIP_20260910.md)에
있습니다. View의 다른 파일 대화상자·Process/Shell 동작은 각각 실제 UI
adapter 또는 별도 소유권 감사가 필요한 범위이므로 이 slice에서 일괄 이동하지
않습니다.

### 3.3 Shell readiness 정책을 읽는 순서

도구 readiness 표시는 View가 직접 판단하지 않고 기존
`OpenVisionShellPreviewViewModel`이 계산·투영합니다. View에는 Recipe, Layer,
Native settings 변경 시 갱신을 요청하는 연결만 남아 있습니다.

| 동작 | concrete owner | 호출 경로 |
| --- | --- | --- |
| Arithmetic Input B 필요 여부 | `OpenVisionShellPreviewViewModel` + 기존 `VisionPipelineArithmeticStep` | `RefreshToolReadiness` → `OpenVisionNativeToolSettingsStore.Load` → `RequiresInputLayerB` |
| Main/보조 Layer 증거 | `OpenVisionShellPreviewViewModel` | `HasSecondaryWorkspaceImage(IDisplayManager)` |
| 표시용 readiness 상태 | `OpenVisionShellPreviewViewModel.SetToolReadiness` / `ApplyToolReadiness` | Navigation item badge, description, command projection |
| 변경 알림 연결 | `OpenVisionShellHostView.xaml.cs` | `RefreshToolReadiness` → `viewModel.RefreshToolReadiness(...)` |

```text
Recipe/Layer/Native settings event
  -> OpenVisionShellHostView.xaml.cs.RefreshToolReadiness
  -> OpenVisionShellPreviewViewModel.RefreshToolReadiness
  -> SetToolReadiness
  -> ApplyToolReadiness
```

다음 검색으로 업무 판단과 표시 상태의 owner를 함께 찾을 수 있습니다.

```powershell
rg -n "RefreshToolReadiness|SetToolReadiness|RequiresInputLayerB|HasSecondaryWorkspaceImage" src/OpenVisionLab/UI/Menu/Wpf
```

상세 변경 및 검증은 [`OPENVISIONLAB_MVVM_SHELL_READINESS_20260910.md`](../../reports/OPENVISIONLAB_MVVM_SHELL_READINESS_20260910.md)에 기록합니다.

### 3.4 Shell Recipe 저장 callback을 읽는 순서

Recipe Manager의 저장 command는 View constructor에서 Recipe state를 직접
호출하지 않고 기존 Recipe controller를 통과합니다.

```text
RecipeCommandSurface.saveRecipe
  -> OpenVisionShellHostRecipeController.SaveRuntimeRecipeTools
  -> runtimeContext.Global.Recipe.SaveTools
  -> RecipeRuntimeStorage.Save
```

| 책임 | owner |
| --- | --- |
| WPF command/callback 연결 | `OpenVisionShellHostView.xaml.cs` |
| Shell Recipe runtime context 연결 | `Shell/Recipe/OpenVisionShellHostRecipeController.cs` |
| Tool XML 저장·rollback | `RecipeState` → `RecipeRuntimeStorage` → `VisionToolRepository` |

다음 검색으로 View가 직접 Recipe persistence를 호출하지 않는지 확인할 수
있습니다.

```powershell
rg -n "SaveRuntimeRecipeTools|Global\.Recipe\.SaveTools|saveRecipe" src/OpenVisionLab/UI/Menu/Wpf
```

상세 변경 및 검증은 [`OPENVISIONLAB_MVVM_SHELL_RECIPE_SAVE_20260910.md`](../../reports/OPENVISIONLAB_MVVM_SHELL_RECIPE_SAVE_20260910.md)에 기록합니다.

### 3.5 Shell·Pipeline Review code-behind 감사 결과

R10~R13 이후 production View에 남은 직접 코드는 WPF dialog/file picker,
MessageBox, control layout, event lifetime, display-only Bitmap snapshot,
hit-test, localization, callback forwarding으로 분류됩니다. Pipeline storage,
execution service, Recipe persistence, Tool readiness policy는 View에서 직접
호출하지 않습니다.

기존 partial 선언은 XAML/event/test-hook 또는 책임별 command/document 파일로
사용되고 있습니다. 파일 길이만 줄이기 위한 새 partial이나 forwarding wrapper는
추가하지 않았습니다. 상세 표와 증거는
[`OPENVISIONLAB_SHELL_PIPELINE_CODEBEHIND_BOUNDARY_AUDIT_20260910.md`](../../reports/OPENVISIONLAB_SHELL_PIPELINE_CODEBEHIND_BOUNDARY_AUDIT_20260910.md)에
있습니다.

### 3.6 Shell 조합·partial·긴 이름을 읽는 새 기준

사용자가 제기한 “실제 상태 owner가 누구인가” 문제를 해결하기 위해
R16~R20 구조 개선 프로그램을 시작했습니다. 이 프로그램의 첫 기준은
새 클래스를 추가하는 것이 아니라 현재 생성 지점을 한 곳에서 찾는 것입니다.

| 읽을 순서 | 생성/호출 지점 | 상태와 수명 owner |
| --- | --- | --- |
| 1 | `Program.Main` → `OpenVisionLabApplication.Run` → `OpenVisionShellHostWindow` | Application이 process-scoped context와 기본 `DisplayManager`를 만들고 종료 순서를 시작합니다. |
| 2 | `OpenVisionShellHostView` constructor | Shell composition layer가 기존 Session/Layer/Workspace/Tooling/Recipe/Document concrete owner를 만들고, XAML command surface를 연결합니다. 이 View는 조합과 UI lifecycle을 소유하며 업무 persistence를 소유하지 않습니다. |
| 3 | `Shell/Session`, `Shell/Layers`, `Shell/Workspace`, `Shell/Recipe`, `Shell/Tooling`, `Shell/Documents` | 각 폴더의 controller/presenter/document가 해당 mutable state와 독립 수명을 소유합니다. 필드 이름의 `OpenVisionShellHost` 접두사는 같은 Shell 문맥에서 public/XAML 계약을 보존하기 위해 현재 유지합니다. |
| 4 | `ShowSelectedTool` → `ShowPipelineReview` | Tool 선택 이후 Document restore/create와 도킹 workspace 진입을 확인합니다. |
| 5 | `Views/OpenVisionPipelineReviewView.xaml.cs` → `OpenVisionPipelineReviewLayoutController` | View는 XAML 이벤트 adapter와 구독 해제를 소유하고, layout controller가 compact/details/step-flow 상태와 sizing/visibility 정책을 소유합니다. |
| 6 | `Dispose`와 각 owner의 Dispose/unsubscribe | 주입된 session resource는 session/view owner가 정리하고 process-scoped 기본 resource는 Application이 정리합니다. View가 임의로 공유 기본 manager를 해제하지 않습니다. |

partial 파일은 세 종류로만 분류합니다.

1. XAML/generated 또는 framework가 요구하는 partial (`Window`, `UserControl`,
   XAML code-behind).
2. 동일 View의 event/test hook partial처럼 별도 object lifetime을 만들 수 없는
   UI adapter.
3. 실제 책임 이름을 가진 command/document/orchestration partial.

2번과 3번은 file length만 줄이기 위한 근거가 되지 않습니다. R16 inventory에서
각 partial의 caller, mutable-state writer, binding/public/serialization 계약을
확인했고, `OpenVisionShellHostRecipeCommandSurface`의 10개 partial은 공통
binding 상태와 callback을 공유해 R17에서 독립 owner 추출을 닫았습니다.
독립 owner로 추출할 수 있는 새 경계가 재현될 때만 후속 이동을 허용합니다.
`Shell/Support/OpenVisionShellHostViewTestSurface.cs`와
`Views/OpenVisionPipelineReviewView.xaml.cs` 같은 UI/test/event partial을 바로
삭제하거나 concrete wrapper로 바꾸지 않는 이유는 이 계약과 수명 경계를
보존하기 위해서입니다.

긴 타입명도 동일한 기준으로 처리합니다. `OpenVisionShellHost*` 이름은 Shell
문맥 밖에서 public/XAML/reflection으로 사용되는지 먼저 검색하고, 계약이 없는
private/internal 타입만 후보로 삼습니다. R18에서는 실제 source-only 결과 타입
`OpenVisionShellHostLayerListRefreshResult`를 `LayerListRefreshResult`로
개명했으며, 나머지는 계약과 문맥이 확인될 때까지 유지합니다. 약어·forwarding
alias·새 Manager/Factory는 추가하지 않습니다.

R16 빠른 검색:

```powershell
rg -n "OpenVisionShellHostView|ShowSelectedTool|ShowPipelineReview|Dispose|partial class" src/OpenVisionLab/UI/Menu/Wpf
rg -n "OpenVisionShellHostRecipeCommandSurface|\.cs$" src/OpenVisionLab/UI/Menu/Wpf
```

## 4. 레이어와 이미지 표시 구조

OpenVisionLab의 중심 모델은 레이어입니다.
툴은 입력 레이어에서 이미지를 읽고, 출력 레이어에 결과 이미지를 씁니다.

주요 책임 분리:

| 구성요소 | 책임 |
| --- | --- |
| `IDisplayManager` | 레이어 목록, 선택 레이어, 레이어 이미지 조회/갱신의 중심 인터페이스입니다. |
| `OpenVisionNativeLayerRouteController` | input/output 레이어 선택 규칙을 관리합니다. 출력 레이어가 자기 자신의 입력 후보가 되지 않도록 관리합니다. |
| `OpenVisionNativeToolLayerViewController` | 툴 preview와 메인 display manager 사이의 레이어 표시를 갱신합니다. |
| `OpenVisionNativePreviewLayerPublisher` | preview 결과 bitmap을 출력 레이어에 publish하고, 필요한 경우 output layer를 생성합니다. |
| `OpenVisionBitmapCanvasPresenter` | WPF bitmap workspace 표시와 zoom state 보존을 담당합니다. |
| `OpenVisionBitmapImagePreviewFactory` | bitmap을 WPF 표시용 image source로 변환하는 공통 경로입니다. |

레이어 관련 불변 조건:

- output layer 생성은 input layer selection을 바꾸면 안 됩니다.
- 툴 output preview 클릭은 메인 작업영역을 output layer로 전환해야 합니다.
- input preview 클릭은 메인 작업영역을 input layer로 전환해야 합니다.
- preview 결과를 output layer에 갱신할 때 operator가 선택한 input route를 깨면 안 됩니다.

## 5. Native WPF Tool 구조

툴 UI는 `src/OpenVisionLab/UI/VisionTest` 아래에 집중되어 있습니다.

| 경로 | 역할 |
| --- | --- |
| `Contracts/` | tool view model contract, preview canvas contract 등 인터페이스 정의입니다. |
| `Composition/` | tool view model 생성과 조합 서비스입니다. |
| `Services/` | tool 주변 서비스입니다. |
| `ViewModels/` | tool parameter/state ViewModel입니다. 이름에는 가능하면 `ViewModel`을 명시합니다. |
| `Wpf/` | 실제 WPF tool view, shell, runtime, binder, behavior가 위치합니다. |
| `Wpf/Behaviors/` | view event와 command를 연결하는 재사용 behavior/controller입니다. |

### 5.1 공통 tool shell

| 구성요소 | 책임 |
| --- | --- |
| `VisionToolSingleInputPropertyToolShell.xaml(.cs)` | input 1개, output 1개, PropertyGrid 기반 툴의 공통 shell입니다. |
| `VisionToolDoubleInputCustomToolShell.xaml(.cs)` | input A/B와 output이 필요한 툴, 예: Arithmetic 계열 shell입니다. |
| `VisionToolInlinePreviewSlot.cs` | 툴 내부 input/output preview viewer입니다. 이미지 표시, zoom, pan, line ROI overlay 표시를 담당합니다. |
| `VisionToolChromePresenter.cs` | tool chrome/header/status 등 공통 표시 책임입니다. |
| `VisionToolPropertyGridHost.cs` | PropertyGrid host 생성과 selected object 연결을 담당합니다. |

### 5.2 Runtime / Binder / ViewModel

| 구성요소 | 책임 |
| --- | --- |
| `VisionToolSingleInputViewModel.cs` | input/output layer selection, command 요청 상태를 보관합니다. |
| `VisionToolDoubleInputViewModel.cs` | A/B input과 output을 쓰는 툴의 selection/command 상태입니다. |
| `VisionToolSingleInputViewRuntime.cs` | single input view의 runtime wiring입니다. |
| `VisionToolDoubleInputViewRuntime.cs` | double input view의 runtime wiring입니다. |
| `VisionToolSingleInputViewBinder.cs` | ComboBox, preview, button 등 view element와 ViewModel/command를 연결합니다. |
| `VisionToolDoubleInputViewBinder.cs` | double input tool의 binder입니다. |
| `VisionToolLayerSelectionBehavior.cs` | layer ComboBox 동작을 공통화합니다. |
| `VisionToolActionBehavior.cs` | preview click, run, add pipeline, create output 같은 action 연결을 공통화합니다. |
| `VisionToolPropertyChangeController.cs` | PropertyGrid 변경과 preview scheduling 흐름을 연결합니다. |
| `VisionToolDebouncedPreviewScheduler.cs` | slider/property 변경 시 preview를 과도하게 실행하지 않도록 debounce합니다. |

### 5.2.1 Threshold 제안·Undo 코드를 읽는 순서

Threshold Basic의 제안 분석과 Undo 상태는 View의 업무 판단과 분리되어
`Wpf/Tooling/Threshold/VisionToolThresholdSuggestionSession.cs`가 소유합니다.
이 클래스는 WPF를 참조하지 않고 Preview 증거와 Threshold 속성의 일치 여부,
현재 제안, 적용 전 값과 적용 후 값의 Undo snapshot을 관리합니다. 실제 값
적용과 marker 갱신은 기존 `ThresholdInteractionController`가 계속 담당합니다.

```text
ThresholdToolWpfView.AnalyzeThresholdSuggestion
  -> VisionToolThresholdSuggestionSession.Analyze
  -> 기존 VisionToolThresholdSuggestionAnalyzer
ThresholdToolWpfView.UseThresholdSuggestion
  -> session.Use
  -> 기존 ThresholdInteractionController.ApplySignalMarkerValue
ThresholdToolWpfView.UndoThresholdSuggestion
  -> session.Undo
  -> 기존 ThresholdInteractionController.ApplySignalMarkerValue
```

변경 시에는 먼저 session의 증거·Undo 계약을 확인하고, 다음으로 View의
상태 표시·버튼·marker 연결을 확인합니다. 이 경계의 독립 계약은
`tools/VisionRecipeRunnerSmoke/ThresholdSuggestionSessionContract.cs`에 있으며,
실제 WPF 입력·테마·DPI·모니터 운전은 별도 검증 범위입니다.

View 코드비하인드의 목표:

- View는 XAML element expose와 최소한의 event bridge만 갖습니다.
- 상태와 명령은 ViewModel/Runtime/Binder/Behavior로 이동합니다.
- View 안에 ViewModel을 강하게 박아 넣지 않습니다. View만 이동해도 가능한 한 깨지지 않아야 합니다.

### 5.3 Learn Matching 코드를 읽는 순서

아래 생산 코드는 `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/`에 있습니다.

| 소유자 | 책임 |
| --- | --- |
| `MatchingLearnPresenter.cs` | Matching/EdgeBasedMatching의 입력값, 후보 판정, 설명·수식·단계 상태 |
| `FeatureMatchingLearnPresenter.cs` | FeatureMatching의 Good Match 개수, 설명·판정·단계 상태 |
| `OpenVisionLearnMatchingSimulationModel.cs` | 두 Presenter가 재사용하는 시뮬레이션 데이터와 계산 |
| `MatchingLearnView.xaml(.cs)` | Matching 계열 컨트롤·namescope, 셀 그리기, 두 타이머, 주제별 Tool 링크, Loaded/Unloaded |
| `LearnResources.xaml` | Window와 독립 View가 함께 사용하는 Learn 스타일·애니메이션 브러시의 단일 정의 |
| `LearnCellVisuals.cs` | 기존 Window 주제와 Matching View가 공유하는 작은 셀·회색 브러시 생성 |
| `OpenVisionLearnWindow.xaml(.cs)` | 외부 주제 선택·창 수명·도구 callback 전달, 다른 Learn 주제, 기존 공개 테스트 진입점 |
| `tools/VisionRecipeRunnerSmoke/LearnMatchingPresentationContract.cs` | Window 없이 점수·판정 경계·설명·상태 독립성을 검증 |
| `tools/PipelineViewerScreenshotSmoke/LearnMatchingPresentationSmoke.cs` | 기존 화면 연결과 독립 View의 도구 호출·분리/재결합·타이머 종료 검증 |

예: “필요한 Good Match 개수를 6으로 바꿨을 때 설명이 잘못 나온다”면
`FeatureMatchingLearnPresenter.Update`와 `MeaningText`를 먼저 봅니다.
점수 계산 문제는 SimulationModel, Matching 색상·스크롤 문제는 MatchingLearnView에서 추적합니다.
Presenter는 WPF 컨트롤을 참조하지 않으므로 설명을 수정할 때 창을 생성할 필요가 없습니다.

```text
Window.SelectTopic / SetOpenRelatedToolAction
  -> MatchingLearnView의 주제·도구 진입점
MatchingLearnView의 슬라이더 / Step / Reset 이벤트
  -> Presenter.Update / AdvanceAnimation / ResetAnimation
  -> 기존 SimulationModel의 계산
  -> Presenter의 결과·설명
  -> MatchingLearnView의 TextBlock / 셀 그리기
```

예: “View를 다시 붙이면 한 번에 두 단계가 진행된다”면 MatchingLearnView의
Loaded/Unloaded 구독 쌍과 `CaptureViewBoundary` 재결합 회귀를 확인합니다.
View가 두 타이머를 소유하고 Unloaded에서 정지·구독 해제합니다. 재결합은
자동 재생하지 않습니다. Window의 Close도 View에 정지를 전달합니다.
기존 Window 테스트 진입점은 호환용 전달이며 자식 컨트롤을 노출하지 않습니다.

이 Matching View 경계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_MATCHING_VIEW_20260908.md)의
범위에서 종료합니다. 사용자의 새 명시적 요청 전에는 다른 모델이나 예약 실행이
추가 리팩토링·주석·문서 갱신·재검증을 자동으로 이어서 하지 않습니다.

### 5.4 Learn Layer / Pipeline / Recipe 코드를 읽는 순서

생산 코드는 같은 `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/` 폴더에 있습니다.

| 소유자 | 책임 |
| --- | --- |
| `LayerRecipeLearnPresenter.cs` | 네 개의 학습용 연결 경로, 선택·애니메이션 단계, 설명·수식, 강조할 Layer·행 결정 |
| `LayerRecipeLearnView.xaml(.cs)` | 해당 주제의 컨트롤·namescope, 셀 그리기, 슬라이더 동기화, 재생 타이머·Loaded/Unloaded |
| `OpenVisionLearnWindow.xaml(.cs)` | 주제 선택 시 View 갱신, 창 종료 시 정지, 기존 공개 테스트 진입점의 전달 |
| `LearnResources.xaml` / `LearnCellVisuals.cs` | 기존 스타일·브러시·셀 생성 재사용; 이번 작업에서는 변경 없음 |
| `tools/VisionRecipeRunnerSmoke/LearnLayerRecipeContract.cs` | Window 없이 경로·강조·Reset·재시작·상태 독립성 검증 |
| `tools/PipelineViewerScreenshotSmoke/LearnLayerRecipeSmoke.cs` | 실제 주제 연결과 독립 View의 재생·분리/재결합·종료 검증 |

```text
Window의 주제 선택 -> LayerRecipeLearnView.RefreshSelection
View의 Slider / Step / Reset -> LayerRecipeLearnPresenter
Presenter의 설명·강조 결정 -> View의 TextBlock / 셀 그리기
Window.Close 또는 View.Unloaded -> View가 타이머 정지
```

예: “3단계가 어떤 Layer를 읽는가?”는 Presenter의 `Steps`와 `IsRouteLayer`에서
확인합니다. 색상·셀 너비·스크롤 문제는 View를 봅니다. 이 데이터는 학습용
시뮬레이션이며 실제 Recipe 실행·저장이나 Layer 생성·선택을 수행하지 않습니다.

Reset은 애니메이션과 강조만 초기화하고 슬라이더·수식·설명을 유지합니다.
주제로 돌아오면 그 슬라이더 값으로 단계를 갱신합니다. 타이머가 슬라이더를
바꿀 때는 `isLayerRecipeAnimationAdvancing`이 수동 조작으로 인한 일시 정지를
막습니다. View 재결합 시 중복 진행 문제는 Loaded/Unloaded 구독 쌍과
`CaptureViewBoundary`의 두 번 재결합 회귀를 확인합니다.

이 경계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_LAYER_RECIPE_VIEW_20260908.md)의
범위에서 종료합니다. 새 명시적 사용자 요청 없이 다른 모델이나 예약 실행이
추가 코드·주석·문서·재검증 작업을 시작하지 않습니다.

### 5.5 Learn Metrics / Acceptance 코드를 읽는 순서

생산 코드는 `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/`에 있습니다.

| 소유자 | 책임 |
| --- | --- |
| `MetricsAcceptanceLearnPresenter.cs` | 고정 학습 샘플의 평균·범위·최대값, 단계, 판정·설명·표시값·강조 결정 |
| `MetricsAcceptanceLearnView.xaml(.cs)` | 주제 컨트롤·namescope, 셀 렌더링, 설명 패널, 타이머와 Loaded/Unloaded |
| `OpenVisionLearnWindow.xaml(.cs)` | 주제 선택·창 종료 전달, 기존 공개 테스트 진입점 호환 |
| `LearnResources.xaml` / `LearnCellVisuals.cs` | 기존 스타일·브러시·셀 생성 재사용 |
| `tools/VisionRecipeRunnerSmoke/LearnMetricsAcceptanceContract.cs` | 계산·판정·단계·인스턴스 독립성과 숫자 표기 검증 |
| `tools/PipelineViewerScreenshotSmoke/LearnMetricsAcceptanceSmoke.cs` | 실제 Window 연결, 독립 View, 재생·재결합·종료 검증 |

```text
Window 주제 선택 -> MetricsAcceptanceLearnView.RefreshFrame
View Step / Reset -> Presenter.AdvanceAnimation / ResetAnimation
Presenter FormulaText / AnimationStatusText / GetSampleText -> View 렌더링
Window.Close 또는 View.Unloaded -> View 타이머 정지
```

예: “평균은 OK인데 왜 최종 판정은 NG인가?”는 Presenter의 `AverageOk`,
`OutlierGateOk`, `FormulaText`를 봅니다. 고정 샘플의 평균은 0.564이고,
범위 0.33과 최대값 0.82가 기준을 넘는 기존 예제입니다. 실제 검사 엔진의
판정 정책이 아니라 학습용 시뮬레이션이므로 이곳에서 Recipe를 실행하지 않습니다.

초기 상태는 완료 단계 3입니다. Reset은 0, 완료 후 Step은 1로 이동합니다.
주제를 떠났다가 돌아오면 단계를 유지하며, View 재결합은 단계와 설명 패널
상태를 보존하면서 재생은 멈춥니다. 색상·스크롤 문제는 View에서,
중복 재생 문제는 Loaded/Unloaded와 `CaptureViewBoundary` 회귀에서 확인합니다.

이 경계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_METRICS_ACCEPTANCE_VIEW_20260908.md)의
범위에서 종료합니다. 새 명시적 사용자 요청 없이 다른 모델이나 예약 실행이
추가 코드·주석·문서·재검증 작업을 시작하지 않습니다.

### 5.6 Learn Binary / Line 코드를 읽는 순서

생산 코드는 `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/`에 있습니다.

| 소유자 | 책임 |
| --- | --- |
| `BinaryLearnPresenter.cs` | Morphology/Blob/Contour의 고정 샘플·단계·표시 판정·공식·설명 |
| `BinaryLearnView.xaml(.cs)` | 위 세 주제의 컨트롤·셀 렌더링·타이머·Tool 버튼 |
| `LineLearnPresenter.cs` | Edge/LineDistance 단계·표시값·강조·공식·설명 |
| `LineLearnView.xaml(.cs)` | 위 두 주제의 컨트롤·셀 렌더링·타이머·Tool 버튼 |
| 기존 `OpenVisionLearnBinarySimulationModel` / `OpenVisionLearnLineSimulationModel` | 영상·연결 영역·윤곽선·거리 계산; 이번 묶음에서 변경 없음 |
| `OpenVisionLearnWindow.xaml(.cs)` | 주제 선택·제목·부제·공통 실습·Focus·창 종료와 공개 facade |
| `tools/VisionRecipeRunnerSmoke/Learn{Binary,Line}PresentationContract.cs` | Window 없는 고정 입력·출력·판정·단계 회귀 |
| `tools/PipelineViewerScreenshotSmoke/LearnBinaryLineSmoke.cs` | 실제 Window 연결·설정·재생·독립 View 재결합·Tool callback 순서 |

```text
Window 주제 선택 -> BinaryLearnView / LineLearnView.SelectTopic + Update...Guide
View 설정 / Step / Reset -> 해당 Presenter -> 기존 SimulationModel
Presenter의 값·단계·표시 판정 -> View의 셀·설명 렌더링
View.Unloaded -> 타이머 정지와 Tick 해제 / Window.Close -> StopAnimations
```

예: “MIN_AREA를 바꿨는데 Blob은 왜 계속 재생되는가?”는 기존 동작입니다.
Binary Presenter는 면적 조건만 갱신하고 View는 타이머를 멈추지 않습니다.
Morphology 모드 변경은 완료 단계25, Contour/Line 설정 변경은 완료 단계3으로
이동하며 정지합니다. 주제로 돌아올 때 Morphology만 완료 단계로 복원하고
나머지는 유지합니다. 단순 주제 숨김은 재생을 유지하지만 View 분리는 정지합니다.

Line의 두 Tool 버튼은 같은 `VISION_MENU.Line`을 전달해도 검출과 Measure
안내가 다릅니다. callback 성공 후에만 안내를 바꾸며, 여기서 검사·Recipe 저장을
실행하지 않습니다. 화면 값은 기존 Paint/Update 위치에서 동기화하고 Loaded는
새 자동 실행·설정 복원을 추가하지 않습니다.

이 두 경계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_BINARY_LINE_BATCH_20260908.md)의
범위에서 종료합니다. 이 문서 0절의 새 증거 없이는 다시 분리하지 않습니다.
추가 구현·주석·문서·재검증을 다른 모델이나 예약 실행이 자동으로 이어가지 않습니다.

### 5.7 Learn Grayscale 코드를 읽는 순서

생산 코드는 `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/`에 있습니다.

| 소유자 | 책임 |
| --- | --- |
| `GrayscaleLearnPresenter.cs` | 밝기·Threshold·Arithmetic·Filtering의 단계, Threshold 왕복 방향/MaxValue, 공식·설명·셀 의미 |
| `GrayscaleLearnView.xaml(.cs)` | 네 주제 컨트롤, 히스토그램/셀/marker 렌더링, 타이머, Apply/Close/Tool 연결 |
| `OpenVisionLearnBasicGrayscaleSimulationModel.cs` | 기존 고정 입력과 픽셀 계산; 이번 묶음에서 수정 없음 |
| `OpenVisionLearnWindow.xaml(.cs)` | 주제·공통 실습·창 수명, 기존 public facade와 Apply 발신자, Foundation 안내 |
| `LearnGrayscalePresentationContract.cs` (Recipe runner) | Window 없는 계산/단계/판정/문구 계약6그룹 |
| `LearnGrayscaleSmoke.cs` (UI runner) | 네 주제 baseline 비교, 설정/타이머/재결합, explicit Apply/Close/Tool 회귀 |

```text
Window 초기값/주제 -> GrayscaleLearnView -> GrayscaleLearnPresenter -> 기존 Model
Presenter의 표시값/셀 역할 -> View 렌더링
View의 ApplyThresholdRequested -> Window의 기존 ApplyThresholdRequested(sender=Window)
View의 ThresholdToolOpened -> Window의 기존 Foundation 안내
Window.Close / View.Unloaded -> 타이머 정지 / Unloaded에서 Tick 해제
```

예: “Threshold를 수동으로 바꿨는데 계속 재생된다”는 기존 계약입니다.
Threshold timer는80ms마다25..230을5씩 왕복하고 수동 값/Invert 변경은
중지하지 않습니다. 밝기/Arithmetic/Filtering의 수동 설정은 완료 단계3으로
이동 후 멈춥니다. 주제 왕복은 단계와 설정을 유지하며, View 재결합은 재생을
자동 시작하지 않습니다. 계산 문제는 기존 Model, 설명·판정은 Presenter,
그리기·스크롤·초점·수명 문제는 View부터 확인합니다.

Threshold의 탭과 하단 Apply/Close는 기존 서로 다른 행의 배치를 View 안에서
유지합니다. Apply는 파라미터 이벤트만 발생시키고 창을 닫거나 Preview/Run을
실행하지 않습니다. 실제 Tool 제어는 기존 Tool Learn controller가 담당합니다.

이 경계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_GRAYSCALE_BATCH_20260908.md)과
0절의 재개 기준을 따릅니다. 파일 길이·모델 취향만으로 다시 분리하지 않습니다.
새 명시적 요청 없이 다른 모델이나 예약 실행이 후속 코드·주석·문서·재검증을
이어가지 않습니다.

### 5.8 Learn Foundation 코드를 읽는 순서

Learn topic0의 좌표·ROI·Mat/채널 규칙을 변경할 때는 다음 경로를 따른다.

| 소유자 | 역할 |
| --- | --- |
| `FoundationLearnPresenter.cs` | 단계5/4,48셀·12셀 ROI, 표시 역할·문구·Tool 위치 안내 |
| `FoundationLearnView.xaml(.cs)` | topic0 패널·렌더링·520/620ms 타이머·명시적 Tool 콜백 |
| `OpenVisionLearnWindow.xaml(.cs)` | 공통 Topic/문서/창·콜백 주입·28개 기존 API 전달·Threshold 안내 relay |

경로: `UI/VisionTest/Wpf/Learn`. 예를 들어 ROI 선택 셀과 문구는 Presenter,
타이머 해제나 실제 control 렌더링은 View, 공통 문서 버튼은 Window를 확인한다.
숨긴 Topic 재생과 View 제거를 구분한다. Unloaded는 stop+Tick 해제,
Loaded는 구독만 복구하며 기존 단계·안내를 보존하고 자동 재생하지 않는다.

집중 검증은 `LearnFoundationPresentationContract.cs`와 `LearnFoundationSmoke.cs`다.
완료·기존 검사 불일치·UI 한계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_FOUNDATION_BATCH_20260908.md)에 있다.
이 소유자를 다시 나누려면0절의 새 증거와 재개 조건이 먼저 필요하다.
다음 자동 작업은 없다.

### 5.9 Learn Geometry Transform 코드를 읽는 순서

Learn topic15의 RotateScale/Affine 안내를 변경할 때는 다음 경로를 따른다.

| 소유자 | 역할 |
| --- | --- |
| `GeometryLearnPresenter.cs` | Angle/Scale, OutputSize 수식, 0~3 단계, 변환 적용 순서, 색상 역할, 상태·Tool 안내 문구 |
| `GeometryLearnView.xaml(.cs)` | Geometry panel과 기존 control namescope, transform 렌더링, Slider/Play/Step/Reset 이벤트, 520ms timer, Loaded/Unloaded, Tool callback |
| `OpenVisionLearnWindow.xaml(.cs)` | topic 선택·공통 문서/실습·창 종료·callback 주입과 기존 Geometry public facade 전달 |
| `tools/VisionRecipeRunnerSmoke/LearnGeometryPresentationContract.cs` | Window 없는 단계·수식·문화권·역할·인스턴스 독립성·Tool 안내 계약 |
| `tools/PipelineViewerScreenshotSmoke/LearnGeometrySmoke.cs` / `LearnGeometryViewSmoke.cs` | 기존 Window 호환과 단독 View 수명·재호스팅·숨김 재생·callback/focus/Close 회귀 |

```text
Window topic selection -> GeometryLearnView.SelectTopic / UpdateGeometryGuide
View slider/button -> GeometryLearnPresenter -> View transform/text rendering
View Loaded -> one timer Tick subscription
View Unloaded / Window.Close -> timer stop + Tick removal
Tool callback success -> GeometryLearnPresenter hint update
```

Presenter를 먼저 보면 단계와 OutputSize 같은 학습 정책을 WPF 없이 확인할 수
있다. 색상·스크롤·Focus·실제 control은 View에서, 공통 Topic/문서/창 수명은
Window에서 추적한다. Reset은 stage0, 완료 후 Step은 stage1, topic 숨김은
재생 유지, View 재호스팅은 stage/settings 보존과 no-autoplay라는 기존 계약을
지킨다. Tool callback이 실패하면 안내를 바꾸지 않고 예외를 전달한다.

이 경계는 [완료 기록](../../reports/OPENVISIONLAB_LEARN_GEOMETRY_BATCH_20260908.md)의
범위에서 종료한다. Foundation, Grayscale, Binary/Line, Matching,
LayerRecipe, Metrics와 Recipe 실행 소유자는 이미 완료되어 있으며, 새 결함·
명시적 요구사항·책임 충돌이 없으면 다시 분리하지 않는다. 다른 모델이나 예약
실행은 새 사용자 요청 없이 코드·주석·문서·재검증을 이어가지 않는다.

## 6. Tool Document 생성 흐름

Native WPF tool은 registry와 factory lane을 통해 생성됩니다.

```text
OpenVisionNativeToolRegistry
  -> tool lane factory 선택
  -> WPF view 생성
  -> property model / view model / preview delegate 구성
  -> OpenVisionNativeToolDocument 생성
  -> Floating tool window에 표시
```

주요 factory lane:

| Lane | 사용 대상 | 주요 파일 |
| --- | --- | --- |
| PropertyGrid tool | Blob, Contour, Matching 등 모델 property로 UI가 만들어져야 하는 알고리즘 툴 | `OpenVisionNativePropertyGridToolFactory`, `OpenVisionNativePropertyGridToolDocumentBuilder` |
| Custom UI tool | Line처럼 고유 UI와 여러 mode가 필요한 툴 | `OpenVisionNativeCustomToolFactory` |
| SimplePreprocess tool | Threshold, Filter, Morphology처럼 공통 preprocess shell로 표현 가능한 툴 | `OpenVisionNativeSimplePreprocessDocumentFactory`, `OpenVisionNativeSimplePreprocessViewConfigurator`, `OpenVisionNativeSimplePreprocessPreviewExecutor` |
| Arithmetic tool | A/B input 또는 offset/operation mode가 필요한 툴 | `OpenVisionNativeArithmeticDocumentFactory` |

새 툴을 추가할 때는 먼저 `docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`를 확인합니다.
반복 배선을 줄이되, 모든 툴을 하나의 거대한 generic factory로 합치지는 않습니다.

## 7. PropertyGrid 구조

PropertyGrid 기반 알고리즘 툴은 반드시 유지해야 하는 핵심 구조입니다.

기본 원칙:

- 모델에 public property와 attribute를 정의하면 PropertyGrid UI가 자동으로 생성되어야 합니다.
- Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching 같은 알고리즘 툴은 PropertyGrid 기반을 유지합니다.
- PropertyGrid에서 선택된 object는 tool runtime이 관리하며, view는 host 역할을 합니다.

관련 프로젝트/파일:

| 위치 | 역할 |
| --- | --- |
| `src/Libraries/PropertyGrid.Abstractions` | PropertyGrid 관련 추상 contract입니다. |
| `src/Libraries/WpfPropertyGridBridge` | 기존 WPG/WPF PropertyGrid 연결 계층입니다. 현재 WPF 직접 사용 방향으로 점진 정리 대상입니다. |
| `src/Libraries/WpfPropertyGridBridge/PropertyGridPropertyValueChangeSubscription.cs` | 선택된 WPG PropertyItem의 ValueChanged/PropertyChanged 구독과 마지막 값 snapshot의 수명 owner입니다. `Attach`와 `Detach`가 교체 순서를 명시합니다. |
| `src/OpenVisionLab/Common/PropertyGrid/PropertyGridEventBinder.cs` | PropertyGrid event 연결 helper입니다. |
| `VisionToolPropertyGridHost.cs` | tool shell 안에서 PropertyGrid control을 생성/유지하는 host입니다. |
| `OpenVisionNativePropertyGridToolFactory.cs` | PropertyGrid 기반 tool 생성 lane입니다. |
| `tools/VisionRecipeRunnerSmoke/PropertyGridPropertyValueChangeSubscriptionContract.cs` | Window 없이 PropertyGrid 구독 owner의 registry, Attach/Detach, equality guard, adapter composition을 검증합니다. |
| `src/OpenVisionLab/Property/ParameterProperty.cs` | 공개 `ParameterProperty` 모델과 `CPropertyParam` XML root를 유지하고 내부 persistence owner를 호출합니다. |
| `src/OpenVisionLab/Property/ParameterPropertyStorage.cs` | `OpenVisionLab.Property` 내부 owner로 Recipe XML과 child property Load/Save만 담당합니다. |
| `tools/VisionRecipeRunnerSmoke/NamespaceProjectBoundaryContract.cs` | public namespace/XML root 보존과 moved owner의 caller/XAML/reflection 경계를 검증합니다. |

주의:

- WinForms bridge 시절의 흔적과 WPF 직접 사용 구조가 섞일 수 있습니다.
- PropertyGrid 문제가 생기면 editor template, selected object, descriptor 중 어느 층이 깨졌는지 분리해서 확인합니다.
- 선택된 object 교체 시에는 adapter의 `Detach`/`Attach` 호출과
  `PropertyGridPropertyValueChangeSubscription`의 event 해제를 먼저 확인합니다.
- 구독 owner를 다시 만들거나 adapter를 파일 크기만으로 나누지 않습니다. 새로운
  lifetime defect, 변경된 명시적 contract, 또는 책임 충돌이 재현될 때만 경계를 다시 검토합니다.

## 8. Vision / OpenCV 계층

`src/OpenVisionLab/Vision/OpenCV`는 OpenCV 기반 툴 property, wrapper, execution 관련 코드가 위치하는 영역입니다.

관련 외부 runtime:

- `dll/OpenVisionLab-Vision-SDK/OpenVisionLab.Core.dll`
- `dll/OpenVisionLab-Vision-SDK/OpenVisionLab.Vision2D.dll`
- `dll/OpenVisionLab-Vision-SDK/OpenVisionLab.Vision2D.Blob.dll`
- `dll/OpenVisionLab-Vision-SDK/OpenCvSharp*.dll`
- `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`

메인 앱은 vendored DLL이 없으면 build target에서 실패하도록 구성되어 있습니다.
OpenCV 실행 경로를 수정할 때는 UI preview뿐 아니라 pipeline step 실행과 recipe runner 호환성도 같이 확인해야 합니다.

## 9. Pipeline / Recipe / Result 구조

Pipeline은 툴 실행을 반복 가능한 step으로 저장하고 재실행하기 위한 계층입니다.

| 영역 | 역할 |
| --- | --- |
| `OpenVisionLab.Vision2D.Pipeline` 참조 | `VisionPipelineStep` 등 SDK 3.0 pipeline model을 제공합니다. |
| `OpenVisionNativePipelineCommandController.cs` | 현재 tool state를 pipeline step으로 추가합니다. |
| `Documents/OpenVisionPipelineReviewDocument.cs` | pipeline review UI document입니다. |
| `src/Libraries/OpenVisionLab.Pipeline.Controls` | pipeline UI control library입니다. |
| `tools/VisionRecipeRunnerSmoke` | recipe/pipeline 실행 smoke입니다. |
| `docs/VISION_PIPELINE_*` | XML recipe schema, LLM recipe contract, runtime plan 문서입니다. |

Pipeline 관련 변경 시 확인할 것:

- UI preview와 pipeline execution이 같은 parameter semantics를 쓰는지 확인합니다.
- output layer 이름, input layer 이름, parameter key가 recipe contract와 맞는지 확인합니다.
- 결과 metric/overlay contract는 `docs/VISION_TOOL_RESULT_CONTRACT.md`를 기준으로 확인합니다.

### 9.1 Recipe 검증 코드를 처음 읽는 순서

아래 UI 경로는 `src/OpenVisionLab/UI/Menu/Wpf/` 기준입니다.
`CommandSurface`는 기존 XAML 바인딩을 유지하는 하나의 concrete 진입점입니다.
현재 Recipe 폴더에는 `RecipeCommandSurface.cs` 한 파일만 있으며, 파일 길이만으로
새 partial이나 forwarding owner를 추가하지 않습니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. 명령과 선택 | `Recipe/CommandSurface/RecipeCommandSurface.cs` | 버튼의 Command, CanExecute, 선택한 Recipe/Pipeline/샘플을 실행에 전달하는 지점. 생성자에 있는 직접 `RelayCommand` wiring이 현재 활성 경로이며, validation-suite scope dispatch는 다음 단계의 기존 execution-session owner로 전달됩니다. |
| 2. 실행 상태와 결과 | `Recipe/Review/OpenVisionRecipeExecutionSessionViewModel.cs` | running/stop 상태, command-state 이벤트, 최근 결과와 localized status projection |
| 3. 선택 샘플 실행 owner | `Recipe/Review/OpenVisionRecipeSelectedSampleExecutionOwner.cs` | 선택 샘플 XML 읽기, 기존 검사 서비스 호출, 선택 샘플 suite batch 저장 |
| 4. Good/Bad pair 실행 owner | `Recipe/Review/OpenVisionRecipePairExecutionOwner.cs` | pair 샘플 순서, 기존 검사 서비스 호출, pair batch 저장 |
| 5. Catalog 실행 owner | `Recipe/Review/OpenVisionRecipeCatalogExecutionOwner.cs` | Product 샘플 필터·정렬, 순차 실행, 진행률 callback, Catalog batch 저장 |
| 6. Local validation-set 실행 owner | `Recipe/Validation/OpenVisionRecipeValidationSetRunner.cs` | 입력 snapshot·Pipeline 경로·순차 실행·중지/부분 저장·expected outcome·summary 저장 |
| 7. Validation evidence owner | `Recipe/Validation/OpenVisionRecipeValidationEvidenceOwner.cs` | 선택 Pipeline XML 읽기와 acceptance gate/mm 보정 근거 문구 계산 |
| 8. Step preview navigation owner | `Recipe/Review/OpenVisionRecipeStepPreviewNavigationOwner.cs` | 실패 Step 매칭, 이전/다음 preview 이동, 동일 Step 판정 |
| 9. Recipe basic lifecycle view | `Recipe/Views/OpenVisionRecipeBasicLifecycleView.xaml(.cs)` | 이름 편집, 기본 CRUD 버튼, validation/status 표시만 담당하는 presentation view; 상태와 Command는 `RecipeCommands`에 남음 |
| 10. 검사 엔진 호출 | `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineSampleCheckService.cs` | 실제 이미지 검사, 오류 분류, 검사 보고서 생성 |
| 11. 저장 형식 | `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineBatchRunSummaryStorage.cs` | batch 결과와 실행 근거의 저장/조회 계약 |
| 12. 화면 표현 | `Recipe/Models/OpenVisionRecipeSampleRunModels.cs`, `Recipe/Review/*Presenter.cs` | 실행 결과를 화면 문구, 선택 행, 요약으로 바꾸는 규칙 |
| 13. 회귀 검증 | `tools/VisionRecipeRunnerSmoke/RecipeExecutionSessionContract.cs`, `ValidationEvidenceOwnerContract.cs`, `StepPreviewNavigationOwnerContract.cs`, `ShellRecipeBasicLifecycleViewContract.cs` | Window 없이 실행·validation evidence·preview navigation·Shell basic lifecycle 경계를 검증하는 독립 테스트 |

```text
사용자가 검사 명령 실행
  -> CommandSurface: CanExecute 확인 + 선택값 전달
  -> ExecutionSession: running 상태 시작 + 선택 owner 호출
  -> SelectedSampleExecutionOwner: XML 읽기 -> 기존 검사 서비스 -> 선택 suite 저장
  -> PairExecutionOwner: XML 읽기 -> pair 순서대로 기존 검사 서비스 -> Good/Bad batch 저장
  -> CatalogExecutionOwner: Product 샘플 정렬 -> 기존 검사 서비스 -> 진행률 callback -> Catalog batch 저장
  -> ValidationSetRunner: Local 입력 snapshot/경로 해석 -> 기존 검사 서비스 -> 중지/부분 저장 -> Local batch 저장
  -> ValidationEvidenceOwner: 선택 Pipeline XML -> acceptance/mm 근거 projection
  -> StepPreviewNavigationOwner: 실패 Step/reference -> preview 선택 또는 이전/다음 이동
  -> PropertyChanged: 기존 화면 바인딩에 상태/결과 전달
  -> BatchRunSaved: CommandSurface의 Run History 다시 읽기
  -> CommandStateChanged: 시작/종료 상태에 맞춰 명령 활성화 갱신
```

2026-09-12 PL-0023에서 validation-suite의 Local validation-set, Good/Bad
pair, Catalog, 선택 샘플 fallback scope dispatch를 기존
`OpenVisionRecipeExecutionSessionViewModel.RunValidationSuiteAsync`로
이동했습니다. `RecipeCommandSurface`는 선택값, `CanExecute`, XAML binding,
상태/결과 projection을 계속 소유하며, 새 dispatcher·partial·forwarding
owner는 추가하지 않았습니다. 실행 상태·취소·저장·종료 수명은 기존 session과
하위 실행 owner를 그대로 따라갑니다. 검증 근거는
[현재 handoff의 PL-0023 기록](../OPENVISIONLAB_CURRENT_HANDOFF.md#current-recipe-validation-orchestration-refactor--2026-09-12)과
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-validation-orchestration-refactor-20260912`에 있습니다.

2026-09-12 PL-0024에서 `RecipeCommandSurface`의 다섯 직접
`System.Windows.Clipboard` 호출을 제거했습니다. Surface는 Copy/Paste
명령·상태·XML draft를 계속 소유하고, 실제 Clipboard I/O는 기존
`OpenVisionShellHostView` 조합부가 세 콜백으로 제공합니다. XAML 명령 이름과
실패 상태 처리는 유지되며, 새 interface/manager/partial은 추가하지
않았습니다. `Binding Properties` 안의 `Commands` region은 명령 선언을
찾기 위한 navigation 표식입니다. 검증 근거는
[현재 handoff의 PL-0024 기록](../OPENVISIONLAB_CURRENT_HANDOFF.md#current-recipecommandsurface-clipboard-boundary-refactor--2026-09-12)과
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-clipboard-boundary-20260912`에 있습니다.

2026-09-12 PL-0025에서 `RecipeCommandSurface`의 직접 WPF Dispatcher 결합을
제거했습니다. 레시피 선택·생성 중 `Dispatcher.Yield`와 switching-state의
Render flush는 기존 `OpenVisionShellHostView` 조합부가 `yieldToUi`와
`flushUi` 콜백으로 제공합니다. Surface는 recipe 상태, Command/CanExecute,
Status와 XAML 계약을 계속 소유하며, Shell 콜백은 dispatcher scheduling만
담당합니다. 두 인자는 기존 생성자 끝에 optional로 추가해 기존 계약 호출을
보존했습니다. 새 interface/manager/factory/provider/wrapper/partial은
추가하지 않았습니다. 검증 근거는
[현재 handoff의 PL-0025 기록](../OPENVISIONLAB_CURRENT_HANDOFF.md#current-recipecommandsurface-dispatcher-boundary-refactor--2026-09-12)과
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-dispatch-boundary-20260912`에 있습니다.

처음 읽는 개발자는 Shell의 `RecipeCommands` 생성 호출, Surface 생성자 끝의
두 scheduler callback, `SelectedRecipeName`/`CreateRecipeCommand` 진입점,
`SelectRecipeAsync`/`CreateAndSwitchRecipeAsync`, 기존 Recipe workspace와
status projection 순서로 따라가면 됩니다. `RecipeCommandSurface` 안의
직접 Dispatcher/Application.Current 호출은 없어졌고, overlay 이미지 표시와
BitmapSource binding은 Surface가 유지하며, 파일 decode는 기존 preview
factory가 소유합니다. recipe/file policy는 현재 계약을 유지합니다.

2026-09-12 PL-0026에서 `RecipeCommandSurface`의 네 WPF 파일 이미지 디코딩
지점을 기존 `OpenVisionBitmapImagePreviewFactory`로 이동했습니다. Locator
overlay는 `CreateFromPath`가 frozen `OnLoad` `BitmapSource`로 읽고, Pin Gap
ROI/Pin Array Gap/Locator intent의 세 dimension 판독은 `ReadPixelSize`가
담당합니다. Surface는 `LocatorEvidenceOverlayImage` XAML property와
recipe/intent 상태·status projection을 계속 소유하며 직접 `BitmapImage`나
`BitmapFrame`을 생성하지 않습니다. 새 image service/interface/manager/
wrapper/partial/folder는 추가하지 않았습니다. 검증 근거는
[현재 handoff의 PL-0026 기록](../OPENVISIONLAB_CURRENT_HANDOFF.md#current-recipecommandsurface-image-file-boundary-refactor--2026-09-12)과
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\recipe-command-surface-image-boundary-20260912`에 있습니다.

처음 읽는 개발자는 RecipeCommandSurface의 locator/intent 메서드에서
`OpenVisionBitmapImagePreviewFactory.CreateFromPath` 또는 `ReadPixelSize`로
이동한 뒤, 기존 validation/status/XAML projection을 따라가면 됩니다.
`File.Exists`, `Path`, XML, recipe/pipeline storage는 여러 기존 owner가 있어
이번 이미지 경계와 합치지 않았습니다.

2026-09-12 PL-0022 정리에서 이 파일 안의 실행되지 않는 `Command creation`
region과 10개 `Initialize*Commands` helper(153줄)를 제거했습니다. 실제 Shell
생성자는 이미 직접 명령을 만들고 있었으므로 XAML 이름, callback, Preview/Run,
Recipe/Layer/Pipeline 라우팅과 종료 수명은 그대로입니다. 현재 소유권 지도와
검증 근거는 [RecipeCommandSurface 정리 기록](../OPENVISIONLAB_CURRENT_HANDOFF.md#current-recipecommandsurface-cleanup--2026-09-12)에
있습니다. 다음 분리는 독립 상태·수명·테스트 경계가 재현될 때만 허용합니다.

예를 들어 “로컬 검증을 중지했는데 결과가 저장되지 않는다”는 문제는 먼저
`RequestValidationSuiteStop`에서 세션의 `RequestStop` 호출을 확인한 뒤,
세션과 `OpenVisionRecipeValidationSetRunner.RunAsync`의 부분 저장 경로를
따라갑니다. 버튼 레이아웃은 View, 중지 상태는 세션, 저장 형식은 Storage에서
수정합니다. 검증 실행 로직을 CommandSurface나 XAML code-behind에 다시 넣지 않습니다.

`StatusText`는 세션의 Suite 상태이고 `ExecutionStatusText`는 Shell의 공통 상태
표시로 전달할 실행 메시지입니다. 두 값은 기존 화면 계약이 달라 구분합니다.
선택 샘플·pair·Catalog·Local validation-set owner는 Window, DisplayManager,
Shell, 현재 UI 선택을 다시 읽는 callback을 소유하지 않습니다. 세션은 이제
실행 상태와 결과 projection만 조합하고, Local 입력 snapshot·경로·실행·부분
저장은 `OpenVisionRecipeValidationSetRunner`가 소유합니다.
Validation Set 패널의 acceptance/calibration 근거는
`OpenVisionRecipeValidationEvidenceOwner`가 기존 workspace/storage를 통해
읽고 계산하며, CommandSurface는 그 결과를 기존 바인딩에 전달합니다.
미리보기 목록의 실패 Step 매칭과 이전/다음 이동, 동일 Step 판정은
`OpenVisionRecipeStepPreviewNavigationOwner`가 소유하며, CommandSurface는
현재 목록과 선택값만 전달합니다. XML 재로드와 PropertyGrid projection은
기존 `OpenVisionRecipeStepEditLoader`가 계속 소유합니다.
Recipe/XML 형식과 입력/출력 Layer 규칙을 바꾸려면 별도의 계약 검토가 필요합니다.

관련 변경 범위와 재현 명령은
[Local validation-set 실행 owner 검증 기록](../../reports/OPENVISIONLAB_OVL16_RECIPE_LOCAL_VALIDATION_EXECUTION_OWNER_20260908.md),
[Shell validation evidence owner 검증 기록](../../reports/OPENVISIONLAB_OVL17_SHELL_VALIDATION_EVIDENCE_OWNER_20260909.md),
[Shell Step preview navigation owner 검증 기록](../../reports/OPENVISIONLAB_OVL18_SHELL_STEP_PREVIEW_NAVIGATION_OWNER_20260909.md),
[Catalog 실행 owner 검증 기록](../../reports/OPENVISIONLAB_OVL15_RECIPE_CATALOG_EXECUTION_OWNER_20260908.md),
[Good/Bad pair 실행 owner 검증 기록](../../reports/OPENVISIONLAB_OVL14_RECIPE_PAIR_EXECUTION_OWNER_20260908.md),
[선택 샘플 실행 owner 검증 기록](../../reports/OPENVISIONLAB_OVL13_RECIPE_SELECTED_SAMPLE_EXECUTION_OWNER_20260908.md)과
[기존 Recipe 실행 구조 기록](../../reports/OPENVISIONLAB_RECIPE_EXECUTION_STRUCTURE_20260908.md)을 참고합니다.
Learn의 Matching 계열 화면 경계는 5.3에 설명되어 있습니다. 다른 Learn 주제와 Shell의
추가 분리는 이번 완료 범위에 포함되지 않으며, 자동 후속 작업으로 승인된 상태가 아닙니다.

### 9.2 Pipeline Review Document revision guard를 읽는 순서

Pipeline Review controller는 실행 세대·취소·stale callback 원자성을 소유하고,
Document는 WPF View에 현재 결과를 투영합니다. 두 경계를 다시 섞지 않고,
Document의 lifecycle revision만 별도 상태 owner가 보관합니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. 문서 상태 | `Documents/OpenVisionPipelineReviewDocumentRevisionGate.cs` | input/recipe/run generation, disposal invalidation, current stamp 검사 |
| 2. 문서 호출 경로 | `Documents/OpenVisionPipelineReviewDocument.cs` | refresh·run·completion·exception·StepUpdated·finally 투영의 current revision guard |
| 3. 실행 callback | `PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs` | controller execution stamp와 StepUpdated revision 전달 |
| 4. event 계약 | `PipelineReview/Execution/OpenVisionPipelineReviewExecutionResult.cs` | StepUpdated input/recipe revision payload |
| 5. 독립 검증 | `tools/VisionRecipeRunnerSmoke/OpenVisionPipelineReviewDocumentRevisionContract.cs` | Window 없이 invalidation·run generation·dispose와 실제 source path 검증 |

```text
Document refresh -> RevisionGate invalidate -> existing controller.Reset
Document run -> RevisionGate.BeginRun -> existing controller.RunAsync
StepUpdated/completion/exception -> current stamp check -> existing View projection
Document dispose -> RevisionGate.Dispose -> old continuation rejected
```

이 경계는 Recipe/XML, explicit Preview/Run, Layer routing, ImageSpace ownership과
기존 controller stale callback 경계를 변경하지 않습니다. 새로운 결함·명시 요구사항
변경·책임 충돌이 없는 한 이 owner를 다시 분리하거나 partial 파일을 추가하지 않습니다.

### 9.3 Run History 조회·비교 orchestration을 읽는 순서

Run History의 저장된 실행을 찾고 기준 실행을 정하는 정책은 화면 바인딩과
분리되어 `OpenVisionRecipeRunHistoryOrchestrationOwner`가 담당합니다. Shell은
기존 선택 속성과 알림만 유지하므로 저장소 경로를 따라갈 때 partial 파일을
모두 검색할 필요가 없습니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. Shell 진입점 | `Recipe/CommandSurface/QualifiedSnapshots.cs` | Recipe/Pipeline 변경 후 recent 및 baseline 선택 결과를 기존 속성에 대입하는 지점 |
| 2. orchestration owner | `Recipe/Review/OpenVisionRecipeRunHistoryOrchestrationOwner.cs` | 저장된 batch 목록 조회, option 변환, baseline fallback, summary 로드와 비교 행 선택 |
| 3. 순수 표현 규칙 | `Recipe/Review/OpenVisionRecipeRunHistoryPresenter.cs` | 필터, 비교 행, 요약 문구와 기본 선택 규칙 |
| 4. 저장 계약 | `Core/Pipeline/Storage/VisionPipelineBatchRunSummaryStorage.cs` | `List`/`Load`와 summary XML의 실제 저장·조회 형식 |
| 5. 독립 검증 | `tools/VisionRecipeRunnerSmoke/RecipeRunHistoryOrchestrationContract.cs` | Window 없이 recent/baseline/comparison/empty 경로를 D: 격리 데이터로 검증 |

```text
Recipe/Pipeline 변경 또는 BatchRunSaved
  -> CommandSurface.RunHistory refresh
  -> RunHistoryOrchestrationOwner.List/Load + existing Presenter
  -> Shell binding properties and existing PropertyChanged notifications
```

새 Run History 동작을 추가할 때는 owner가 저장소·비교 입력을 조합하고,
Presenter가 순수한 표시 규칙을 유지하는지 먼저 확인합니다. WPF View나
Recipe 실행 세션에 저장소 조회를 다시 넣지 않습니다. 이 경계는 기존
Preview/Run 명시성, Recipe/XML, Validation 실행, UI 선택 알림 순서를 바꾸지
않으며 새 결함·요구사항 변경·책임 충돌 없이는 다시 나누지 않습니다.

### 9.4 ImageCanvas Mat 저장 owner를 읽는 순서

ImageCanvas의 기본 Mat 파일 저장 정책은 ViewModel에서 분리되어
`CanvasImageSaver`가 담당합니다. 기존 public 메서드와 호출자는 호환성
facade로 유지되므로 저장 경로를 따라갈 때는 아래 순서만 확인합니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. 호환성 facade | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | `SaveCurrentImage(string)`이 현재 Mat와 callback을 owner에 전달하는 지점 |
| 2. 저장 정책 owner | `src/Libraries/OpenVisionLab.ImageCanvas/Util/CanvasImageSaver.cs` | 확장자 fallback, 디렉터리 생성, callback dispatch, 빈 Mat 거부, `Cv2.ImWrite` |
| 3. 호출자 | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` 및 앱 presenter/view | 기존 public 저장 호출과 사용자 흐름 |
| 4. 독립 검증 | `tools/PipelineViewerScreenshotSmoke`의 `wpf_imagecanvas_owned_mat_load` | 입력 Mat dispose 후에도 현재 canvas 저장과 출력 형식이 유지되는지 |

```text
public SaveCurrentImage(path)
  -> CanvasImageSaver.SaveMat(current Mat, path, existing callback)
  -> callback 또는 Cv2.ImWrite
```

Mat와 callback의 lifetime은 ViewModel이 유지하고, 저장 owner는 호출 중의
동기 파일 정책만 소유합니다. Dialog와 ContextMenu는 각각 View host가
소유하며, WPF PreviewKeyDown/KeyUp policy는 별도 WPF keyboard owner로
이동했고 마우스 입력은 9.9의 별도 owner에 기록했습니다.
완료된 저장 owner를 파일 길이나 탐색 편의를 이유로 다시 partial로 나누지
않습니다. 새 결함·명시 요구사항 변경·책임 충돌이 없으면 이 경계를 유지합니다.

### 9.5 ImageCanvas Open/Save dialog host를 읽는 순서

ImageCanvas 명령이 필요한 modal UI를 ViewModel에서 직접 만들지 않도록,
`RoiImageCanvasView`가 host를 연결하고 `RoiImageCanvasDialogHost`가 WPF
Open/Save 대화상자 수명과 결과 변환을 담당합니다. ViewModel은 파일명·초기
경로 계산과 이미지 로드·저장 결과 처리만 유지합니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. command facade | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | `LoadImageCommand`/`SaveImageCommand`가 host에 경로를 요청하고 기존 이미지·저장 흐름을 호출하는 지점 |
| 2. dialog contract | `src/Libraries/OpenVisionLab.ImageCanvas/Dialogs/IImageCanvasDialogHost.cs` | Open/Save dialog의 최소 path/null 반환 계약 |
| 3. WPF host | `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasDialogHost.cs` | 기존 `OpenFileDialog`/`SaveFileDialog` option, 공용 modal re-entry guard, cancel/path 변환 |
| 4. lifecycle wiring | `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml.cs` | attach 시 host 연결, detach/dispose 시 host 해제 |
| 5. 독립 검증 | `tools/RunUiPrecheck.ps1`의 ImageCanvas/Shell target | View attach와 기존 저장·preview 경로의 Debug/Release 회귀 |

```text
LoadImageCommand -> ImageDialogHost.ShowOpenImageDialog -> LoadImage
SaveImageCommand -> ImageDialogHost.ShowSaveImageDialog -> SaveCurrentImage
RoiImageCanvasView attach -> ImageDialogHost 연결
RoiImageCanvasView detach/dispose -> ImageDialogHost 해제
```

WPF PreviewKeyDown/KeyUp policy와 마우스 입력은 별도 독립 경계입니다. Open/Save dialog host는 Mat이나
파일을 보관하지 않으며, ContextMenu host는 기존 XAML 메뉴 인스턴스와
`IsOpen` 호출만 소유합니다. 완료된 Mat 저장 owner와 Open/Save/ContextMenu
host를 파일 크기나 탐색 편의를 이유로 다시 partial로 나누지 않습니다.

### 9.6 ImageCanvas ContextMenu host를 읽는 순서

ContextMenu를 여는 정책과 WPF 메뉴 인스턴스의 소유권을 분리해 읽습니다.
ViewModel은 활성 interaction mode를 정리한 뒤 좁은 host 계약을 호출하고,
View는 기존 XAML 메뉴를 열고 생명주기를 연결합니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. command facade | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | `ExecuteRightClickCommand`의 Measure/Teaching/AddRoiArray mode reset과 `ContextMenuHost.OpenContextMenu()` 호출 |
| 2. ContextMenu contract | `src/Libraries/OpenVisionLab.ImageCanvas/Dialogs/IImageCanvasContextMenuHost.cs` | `OpenContextMenu()` 하나만 제공하는 내부 UI 계약 |
| 3. View host | `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml.cs` | `MainGrid.ContextMenu.IsOpen` 구현, attach/detach/dispose의 host 연결·해제 |
| 4. XAML menu | `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml` | 기존 `Grid.ContextMenu`, `ItemsSource`의 `MenuItems` binding, item command/icon/child binding |
| 5. 독립 검증 | D: `refactor-ovl11-context-menu-host-20260908`의 `structure-proof.txt`와 ImageCanvas/Shell precheck | concrete WPF 타입 제거, lifecycle, 호출 경로, Debug/Release 회귀 |

```text
ImageCanvasControl right-click
  -> OnMouseRightClick
  -> ExecuteRightClickCommand
  -> ContextMenuHost.OpenContextMenu
  -> RoiImageCanvasView.MainGrid.ContextMenu.IsOpen
```

상태 소유권은 ViewModel의 mode/command policy와 View의 ContextMenu
instance/DataContext/open operation으로 나뉩니다. WPF PreviewKeyDown/KeyUp
policy는 별도 keyboard owner로 이동했고, 마우스 입력 구현은 9.9의
별도 owner에 기록했습니다. Focus·Drag & Drop·DPI·렌더링은 기존 View/
adapter 경계에 남깁니다. 이 경계는 partial-only split이나 완료된 owner 재분할을
요구하지 않습니다.

### 9.7 ImageCanvas WinForms keyboard input owner를 읽는 순서

WinForms canvas 키보드 정책은 ImageCanvasControl의 이벤트 전달,
RoiImageCanvasKeyboardInputController의 단축키 분기, ViewModel의 mutable ROI
상태와 callback으로 나누어 읽습니다. WPF PreviewKeyDown/KeyUp와 마우스
입력은 이 owner에 포함되지 않고 각각 별도 경계로 유지됩니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. 이벤트 source | src/Libraries/OpenVisionLab.ImageCanvas/Engine/ImageCanvasControl.cs | public KeyDown/KeyUp 선언과 OpenGL control forwarding |
| 2. 키보드 policy owner | src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiImageCanvasKeyboardInputController.cs | KeyDown 구독/해제와 Ctrl+Z/Y/Shift+Z, Delete, C/V 분기 |
| 3. 기존 overlay helper | src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiInteractionKeyDown.cs | rectangle copy/paste와 기존 overlay callback 호출 |
| 4. 상태·결과 owner | src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs | selected/copy ROI state, snapshot, Undo/Redo, ROI callback |
| 5. lifecycle | src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs | ImageCanvasControl dispose 전에 controller 해제 |
| 6. 독립 검증 | D: refactor-ovl11-keyboard-input-20260908 | keyboard contract harness, structure/lifecycle proof, Debug/Release build, UI precheck, quantitative audit |

호출 경로는 다음과 같습니다.

ImageCanvasControl.KeyDown
-> RoiImageCanvasKeyboardInputController.OnKeyDown
-> ViewModel callback
-> RoiInteractionKeyDown (Ctrl+C/Ctrl+V)

Controller는 이벤트 정책만 소유하고, ROI mutable state와 공개 event는 기존
ViewModel이 소유합니다. 완료된 WinForms keyboard owner를 파일 크기나
탐색 편의를 이유로 다시 partial/wrapper로 나누지 않습니다. 새 결함·명시
요구사항 변경·입증된 책임 충돌이 있을 때만 registry와 보고서를 함께 갱신합니다.

### 9.8 ImageCanvas WPF keyboard input owner를 읽는 순서

WPF 키보드 입력은 View의 이벤트 수명, ViewModel의 public command facade,
WPF keyboard controller의 정책, ViewModel의 ROI 상태 callback으로 나누어
읽습니다. WinForms `KeyDown` owner와는 다른 WPF `PreviewKeyDown`/`KeyUp`
경로입니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. WPF event owner | `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml.cs` | `PreviewKeyDown`/`KeyUp` 구독·전달과 `Dispose()` 해제 |
| 2. command facade | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | 기존 `PreviewKeyDownCommand`/`KeyUpCommand` 이름과 controller 위임 |
| 3. WPF policy owner | `src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiImageCanvasWpfKeyboardInputController.cs` | Control guard, Delete/`Handled`, F2·Enter와 Ctrl+C/V/S no-op |
| 4. 상태 callback | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | `RemoveSelectedOverlay`와 ROI mutable state |
| 5. 독립 검증 | D: `refactor-ovl11-wpf-keyboard-input-20260908` | WPF keyboard contract, structure/lifecycle/call-path proof, Debug/Release build, UI precheck, quantitative audit |

호출 경로는 다음과 같습니다.

```text
RoiImageCanvasView.PreviewKeyDown
  -> PreviewKeyDownCommand
  -> RoiImageCanvasWpfKeyboardInputController.HandlePreviewKeyDown
  -> RoiImageCanvasViewModel.RemoveSelectedOverlay (Delete)

RoiImageCanvasView.KeyUp
  -> KeyUpCommand
  -> RoiImageCanvasWpfKeyboardInputController.HandleKeyUp (기존 no-op)
```

View가 이벤트 수명과 forwarding을, ViewModel이 public command와 상태를,
controller가 WPF key/modifier policy를 소유합니다. 이 owner는 이벤트를
직접 구독하지 않는 stateless policy boundary이며, 완료된 Mat/Open/Save,
ContextMenu, WinForms keyboard owner를 다시 분할하지 않습니다. 새 결함·명시
요구사항 변경·입증된 책임 충돌이 있을 때만 registry와 보고서를 갱신합니다.
마우스 입력은 다음 9.9 경계에서 별도 owner로 기록합니다.

### 9.9 ImageCanvas mouse input owner를 읽는 순서

ImageCanvas 마우스 입력은 ImageCanvasControl의 public event 전달, mouse
button/mode 정책, 기존 ROI helper, ViewModel mutable state/callback으로
나누어 읽습니다. Focus, Drag & Drop, DPI, Window, animation, rendering은
기존 View와 ImageCanvasControl 경계에 남습니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. 이벤트 source | `src/Libraries/OpenVisionLab.ImageCanvas/Engine/ImageCanvasControl.cs` | public mouse event 선언과 SharpGL control forwarding |
| 2. mouse policy owner | `src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiImageCanvasMouseInputController.cs` | 7개 mouse 구독/해제, Left/Right/Middle/Wheel/Leave 정책, cursor와 timer 호출 |
| 3. 기존 ROI helper | `src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiInteractionMouseDown.cs`, `RoiInteractionMouseMove.cs`, `RoiInteractionMouseUp.cs`, `RoiInteractionCursor.cs` | hit-test, draw/edit/move/measure, add-array와 cursor 계산 |
| 4. 상태·결과 owner | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | selected/drawing/measurement/pan state, snapshot·ROI·ContextMenu callback과 public facade |
| 5. lifecycle | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | ImageCanvasControl dispose 전에 mouse controller 해제 |
| 6. 독립 검증 | D: `refactor-ovl11-mouse-input-20260908` | mouse event lifecycle contract, structure/call-path proof, Debug/Release build, UI precheck, quantitative audit |

호출 경로는 다음과 같습니다.

```text
ImageCanvasControl mouse event
  -> RoiImageCanvasMouseInputController.OnMouseDown/Move/Up/Wheel/Leave
  -> existing RoiInteractionMouse* / RoiInteractionCursor helper
  -> ViewModel state accessor and explicit callback
  -> existing ROI/snapshot/context-menu/property-change behavior
```

Controller가 이벤트 lifetime과 입력 policy를, ViewModel이 상태·결과와
public command/event facade를 소유합니다. 완료된 Mat/save-dialog/open-dialog,
ContextMenu, WinForms keyboard, WPF keyboard, mouse owner는 새 결함·명시
요구사항 변경·입증된 책임 충돌 없이 다시 partial/wrapper로 나누지 않습니다.
물리 desktop mouse matrix는 아직 qualification 대상입니다.

### 9.10 Learn Window/Shell host를 읽는 순서

Shell의 Learn 진입은 Window 수명과 workspace sample 실행을 분리해 읽습니다.
Window 생성·재사용·owner·callback 연결·`Closed` 해제는 명명된
`OpenVisionShellHostLearnWindowController`가 소유하고, workspace image/sample
정책은 `OpenVisionShellHostCommandController`에 남아 callback으로 전달됩니다.

| 읽는 순서 | 파일/영역 | 여기서 찾는 것 |
| --- | --- | --- |
| 1. Learn host owner | `src/OpenVisionLab/UI/Menu/Wpf/Shell/Commands/OpenVisionShellHostLearnWindowController.cs` | Learn Window 생성·재사용·owner 지정·topic routing·sample/tool callback·`Closed` cleanup |
| 2. Shell command facade | `src/OpenVisionLab/UI/Menu/Wpf/Shell/Commands/OpenVisionShellHostCommandController.cs` | workspace image/sample 실행과 파일 선택 정책; Learn Window 상태는 소유하지 않음 |
| 3. Chrome entry surface | `src/OpenVisionLab/UI/Menu/Wpf/Shell/Chrome/OpenVisionShellHostChromeCommandSurface.cs` | OpenLearn/OpenToolLearn/OpenToolSamples가 host owner로 위임되는 지점 |
| 4. Shell composition | `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`, `OpenVisionShellHostView.xaml.cs` | host 조합과 Pipeline Review 선택 tool Learn route |
| 5. Learn presentation | `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`, `OpenVisionLearnTopicCatalog.cs`, `*LearnView*`, `*LearnPresenter*` | topic 선택·panel/document/practice 표시와 기존 presenter 계약 |
| 6. 독립 검증 | D: `refactor-ovl09-learn-host-20260908` | topic/reuse/Closed/sample callback 계약, 구조 증명, Debug/Release·외부 consumer build, UI precheck, 정량 감사 |

호출 경로는 다음과 같습니다.

```text
Shell Chrome 또는 Pipeline Review
  -> OpenVisionShellHostLearnWindowController
  -> OpenVisionLearnWindow + OpenVisionLearnTopicCatalog
  -> topic View/Presenter
  -> 명시적 sample/tool callback
```

이 경계는 Shell command가 Learn Window 수명을 직접 관리하던 결합을 제거합니다.
Recipe/XML, Preview/Run, Layer/ImageSpace, topic View/Presenter 동작은 그대로
유지합니다. 완료된 host owner는 파일 크기나 탐색 편의를 이유로 다시
partial/wrapper로 나누지 않으며, 새 결함·명시 요구사항 변경·입증된 책임 충돌이
있을 때만 재검토합니다. 내부 topic-selection/presentation policy는 9.11에서
별도 owner로 기록하며, 다음 단일 slice는 ImageCanvas Directory policy입니다.

### 9.11 Learn Window topic presentation policy를 읽는 순서

Learn Window의 topic 선택 결과는 WPF-independent 정책과 View 적용 지점으로
나누어 읽습니다. 정책은 화면을 만들지 않고 immutable presentation state만
반환하며, Window는 기존 topic View/Presenter를 호출해 화면에 적용합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 정책 owner | `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnTopicPresentationPolicy.cs` | catalog topic -> 제목·부제·실습·패널·expander·guide flag 상태 |
| 2. Window 적용 지점 | `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs` | `TopicList_SelectionChanged -> UpdateSelectedTopic -> policy.Resolve`, child View 선택과 guide callback 적용 |
| 3. 메타데이터 source | `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnTopics.cs` | topic 제목·문서·실습·path·tool 매핑 |
| 4. topic owner | `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/*LearnView.xaml.cs`, `*LearnPresenter.cs` | 주제별 UI 상태와 계산 |
| 5. 독립 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-topic-composition-20260908` | 정책/all-topic WPF 계약, before/after Threshold PNG, Debug/Release precheck, 정량 감사 |

완료된 Learn host와 topic View/Presenter는 새 결함·요구사항 변경·입증된
책임 충돌 없이는 다시 분할하지 않습니다. 다음 단일 slice는
`RoiImageCanvasViewModel.cs`의 Directory policy owner입니다.

### 9.12 ImageCanvas Directory policy를 읽는 순서

ImageCanvas Open/Save 명령의 초기 폴더와 마지막 선택 경로는 공통 정책
owner에서 읽습니다. WPF 대화상자 생성은 기존 dialog host에 남고, ViewModel은
명령 순서와 이미지 작업만 조합합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Directory policy owner | `src/Libraries/OpenVisionLab.ImageCanvas/Util/ImageCanvasDirectoryPolicy.cs` | remembered path, `Sample`/`Samples`/`samples` 상위 탐색, base/Pictures/Desktop fallback |
| 2. ViewModel command | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | `LoadImageCommand`/`SaveImageCommand` 순서, filename sanitization, policy 호출, 성공 후 path 기억 |
| 3. Dialog contract/host | `src/Libraries/OpenVisionLab.ImageCanvas/Dialogs/IImageCanvasDialogHost.cs`, `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasDialogHost.cs` | path/null 결과와 기존 Open/Save modal lifetime |
| 4. 이미지 소유자 | `src/Libraries/OpenVisionLab.ImageCanvas/Util/CanvasImageLoader.cs`, `CanvasImageSaver.cs`, `RoiImageCanvasViewModel.cs` | Mat load/save와 현재 이미지 상태 |
| 5. 독립 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-directory-policy-20260908` | policy cases, before/after UI precheck, structure proof, build/audit evidence |

Refresh timer 수명은 같은 ViewModel의 `Refresh` 영역에서 소유합니다. 콜백은
타이머를 지역 스냅샷으로 확인하고 Dispose 중 이미 해제된 타이머에는 작업하지
않습니다. 따라서 `RoiImageCanvasViewModel.Dispose`의 stop/unsubscribe/dispose
순서를 먼저 확인한 뒤 이미지·대화상자 경로를 따라갑니다. 이 경계의 재현 및
검증 증거는 PL-0019와
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\roi-image-canvas-timer-race-20260912`
에 있습니다.

완료된 ImageCanvas Mat·dialog host·ContextMenu·keyboard·mouse owner와
앱 계층 `OpenVisionImageDirectoryResolver`는 새 결함·요구사항 변경·입증된
책임 충돌 없이는 다시 나누지 않습니다. 대표 WPF runtime qualification은
9.13에서 완료했고 alternate 환경 행만 남아 있습니다.

### 9.13 Representative WPF runtime qualification을 읽는 순서

대표 WPF desktop qualification은 제품 책임을 새로 나누는 소스 변경이 아니라,
완료된 owner들의 실제 실행 경계를 확인하는 증거 단계입니다. 현재 하네스는
모니터를 동적으로 선택하고, 물리 pointer/keyboard 입력과 UI Automation
readback을 한 실행 안에서 순서대로 수행합니다.

| 순서 | owner/증거 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행 진입 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-wpf-runtime-20260908\runtime-qualification.ps1` | Debug/Release 프로세스, monitor 선택, window 배치, foreground와 종료 순서 |
| 2. Shell contract | `shellTitleBar`, `HostToolSearchTextBox`, `btnToggleToolRail`, `cbHostLanguage` | 시작 AutomationId, keyboard focus/search, Compact rail, language popup |
| 3. Learn/Tool route | `HostLearnButton`, `OpenVisionLearnTopicList`, `HostToolNav_Threshold`, `HostToolNav_Pipeline` | Learn Window/topic 선택, Threshold/Pipeline 선택 상태와 화면 |
| 4. Image route | `btnWorkspaceLoadImage`, native `#32770` dialog, `txtHostWorkspaceStatus` | 실제 이미지 dialog 배치·입력·Open 수락, loaded layer status |
| 5. Layout/lifetime | `OpenVisionWindowMaximizeRestoreButton`, process handle | maximize/restore, selected working area, deterministic shutdown |
| 6. 독립 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-wpf-runtime-20260908\debug`, `release` | 17 PASS/0 FAIL/0 WARN result JSON, runtime PNG, parser/hash/metadata |

호출·검증 경로는 다음과 같습니다.

```text
monitor topology -> selected test monitor -> OpenVisionLab handle
  -> AutomationId contract -> physical pointer/keyboard action
  -> visible state + UIA/result assertion -> maximize/restore -> shutdown
```

2026-09-08 현재 Debug/Release가 각각 17개 PASS, 0 FAIL, 0 WARN을 반환했고
정상 종료했다. `DISPLAY2`의 96% DPI(100%)와 두 모니터 환경만 실행했다.
125/150/175/200% DPI, alternate theme, one-monitor/headless는 환경이 제공되지
않아 미검증이다. 이 행들은 새 구조 owner를 만들 근거가 아니며, 가능해질 때
동일 harness에 추가합니다.

96% runtime evidence, ImageCanvas, Learn, Recipe/Pipeline 완료 owner는 새 모델의
선호나 파일 크기만으로 다시 분할하지 않습니다. 새 실패, 변경된 계약, 또는
실제 미검증 환경 행이 있을 때만 해당 증거를 갱신합니다.

상세 보고서:
[대표 WPF runtime qualification](../../reports/OPENVISIONLAB_WPF_RUNTIME_QUALIFICATION_20260908.md)

OVL-21에서 compatibility-safe namespace/project boundary의 내부 후보 1개를
완료했고, OVL-22에서 smoke runner의 명령행 target 실행 책임 1개를
분리했습니다. Alternate DPI/theme/topology는 환경 전제에 묶인 미검증 행으로
남기며, 새 runtime 결함이 없으면 완료된 owner를 다시 열지 않습니다.

다음 단일 slice는 남은 smoke runner의 fixture/capture/reporting 책임을
실제 경계 기준으로 조사하는 작업입니다. `Recommended model: gpt-5.4-mini`
| `Reasoning effort: medium`.

### 9.14 compatibility-safe namespace/project boundary를 읽는 순서

namespace/project boundary는 공개 타입을 일괄 rename하는 작업이 아닙니다. 먼저
XAML `x:Class`, reflection, serializer, public caller, ProjectReference cycle를
확인하고, 호환성 신호가 없는 내부 owner만 한 번에 하나씩 이동합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 공개 호환 모델 | `src/OpenVisionLab/Property/ParameterProperty.cs` | legacy `OpenVisionLab.ParameterProperty`와 `CPropertyParam` XML root |
| 2. 내부 persistence owner | `src/OpenVisionLab/Property/ParameterPropertyStorage.cs` | `OpenVisionLab.Property` namespace, Load/Save 및 child config 순서 |
| 3. 독립 검증 | `tools/VisionRecipeRunnerSmoke/NamespaceProjectBoundaryContract.cs` | caller 수, XAML/reflection/serializer 신호, XML map 보존 |
| 4. runtime 호환성 | `tools/RecipeXmlCompatibilityCheck/Program.cs` | 13개 XML root Debug/Release deserialize/save-reload |
| 5. 정량 감사 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl21-refactor-audit-20260909` | project cycle 0, Shell storage call 0, 현재 파일 수 |

`ParameterPropertyStorage` 이동은 public namespace나 assembly를 변경하지 않으며,
새 interface·wrapper·project를 만들지 않습니다. 이 owner를 다시 나누거나
public property를 이동하려면 새 결함·명시 계약 변경·호환성 inventory가 먼저
필요합니다. OVL-22에서 smoke-runner 명령행 경계를 완료했으며, 다음 structural
slice는 남은 fixture/capture/reporting 책임을 별도로 조사합니다. 이 namespace
이동을 다시 다루지 않습니다.

### 9.15 smoke runner 명령행 책임을 읽는 순서

OVL-22는 대형 `Program.cs`를 줄이는 것을 목표로 하지 않고, WPF를 생성하지
않는 명령행 orchestration 경계를 분리했습니다. 대상 목록과 실제 캡처 함수는
기존 `Program`이 소유하고, runner는 전달받은 catalog를 사용해 선택·실행·결과
보고·오류 evidence를 처리합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 진입점 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | `--all`, `--target`, `--suite`, `--list`가 기존 catalog와 runner를 연결하는 얇은 조합 지점 |
| 2. 명령행 owner | `tools/PipelineViewerScreenshotSmoke/ScreenshotSmokeTargetRunner.cs` | target 실행, 결과 행, 예외 `.error.txt`, 이름 trim, suite 순서·중복 제거, 정렬 출력 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ScreenshotSmokeTargetRunnerContract.cs` | WPF 창 없이 성공·미등록·예외·파싱·suite·catalog 출력과 소유권 연결 검증 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl22-screenshot-smoke-target-runner-contract-20260909` | Debug/Release 10/10 계약과 결과 파일 |

`ScreenshotSmokeTargetRunner`는 internal concrete owner이며 별도 interface,
factory, wrapper, message bus를 추가하지 않았습니다. 남은 WPF capture 그룹은
fixture/capture/reporting의 독립 상태와 호출 경계가 증명될 때만 한 번에 하나를
분리하며, 파일 크기만으로 다시 나누지 않습니다. 완료된 OVL-22 owner를 반복
추출하거나 문서만 추가하는 작업은 금지합니다.

### 9.16 Learn 문서 문구 정책을 읽는 순서

OVL-23은 `PipelineViewerScreenshotSmoke/Program.cs`의 Learn 문서 문구 검사를
WPF 창 생명주기와 분리했습니다. 정책 owner는 문서 파일과 화면에서 수집된
문자열만 받아 내부 engineering copy를 판정하며, 창을 만들지 않고 계약으로
검증할 수 있습니다. `Program`은 WPF visual-tree 순회와 topic/document 해석을
계속 소유합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 화면 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | `AssertLearnTopicDocument`, `CollectVisibleLearnCopy`, 기존 Learn topic/capture 호출 순서 |
| 2. 문서 정책 owner | `tools/PipelineViewerScreenshotSmoke/LearnDocumentationCopyPolicy.cs` | forbidden phrase 목록, 파일 내용 검사, visible copy 검사, 기존 오류 문구 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/LearnDocumentationCopyPolicyContract.cs` | source owner 연결, clean/forbidden 문서, visible copy, case-insensitive 매칭 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl23-learn-document-copy-policy-contract-20260909` | Debug/Release 7/7 계약 결과 |

`LearnDocumentationCopyPolicy`는 별도 interface/factory/wrapper 없이 concrete
internal owner입니다. `OpenVisionReadinessCheck`의 repository-readiness 문구
검사는 다른 실행 경계이므로 교차 의존성을 만들지 않았습니다. OVL-23 owner를
다시 추출하거나 파일 크기만으로 `Program`을 나누지 않습니다.
### 9.17 screenshot bitmap evidence 책임을 읽는 순서

OVL-24는 `PipelineViewerScreenshotSmoke/Program.cs`의 순수 이미지 증거
판정·저장 책임만 분리했습니다. `ScreenshotBitmapAssertions`는 WPF 창, OpenVision
상태, Recipe, Layer, PropertyGrid를 참조하지 않고 `Bitmap`/`Color`와 경로만
받습니다. 창 lifecycle, PNG 화면 렌더링, OpenGL 진단, fixture 생성과 캡처 시점은
기존 `Program` owner로 남습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | Preview/Run 후 어떤 Layer bitmap을 검사·진단 저장하는지와 기존 호출 순서 |
| 2. 이미지 증거 owner | `tools/PipelineViewerScreenshotSmoke/ScreenshotBitmapAssertions.cs` | presence, diagnostic PNG, difference/overlay/background, binary/grayscale/color 판정과 기존 임계값 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ScreenshotBitmapAssertionsContract.cs` | WPF 없는 source 연결, 성공/실패 이미지, PNG evidence 경로 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl24-screenshot-bitmap-assertions-contract-20260909` | Debug/Release 8/8 계약 결과와 생성된 diagnostic PNG |

이 owner는 새 interface, factory, wrapper 없이 concrete internal type으로 유지합니다.
`CaptureWindowWithContent`, `CaptureStandaloneWindow`, `CaptureElement`, PNG
writer의 state/cleanup 경계는 OVL-24에서 건드리지 않았으며 다음 조사 대상입니다.
완료된 OVL-24를 다시 분할하거나 `Program.cs`를 파일 크기로 자르지 않습니다.
### 9.21 Recipe context fixture 책임을 읽는 순서

OVL-28은 `PipelineViewerScreenshotSmoke/Program.cs`에서 반복 호출되던 순수
`CreateRecipeContextSmokePipeline` 구현을 `RecipeContextFixture` concrete owner로
옮겼습니다. owner는 이름과 step count를 받아 Threshold pipeline을 만들고, 첫
step의 `Main` 입력과 이후 preview layer 연결을 소유합니다. 호출자는 반환된
pipeline을 필요에 따라 수정·저장·실행하며, owner는 파일·WPF·Shell 상태를
참조하지 않습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | 26개 fixture 호출, specialized parameter 변경, Recipe XML 저장/실행 및 UI 검증 |
| 2. fixture owner | `tools/PipelineViewerScreenshotSmoke/RecipeContextFixture.cs` | 이름·count 기반 Threshold steps와 linked layer 생성 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/RecipeContextFixtureContract.cs` | owner 연결, old helper 제거, shape/empty-count/XML round-trip |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl28-recipe-context-fixture-debug-rerun-20260909`, `ovl28-recipe-context-fixture-release-contract-20260909` | Debug/Release 6/6 계약 결과 |

`RecipeContextFixture`는 interface, factory, wrapper chain 없이 concrete internal
owner입니다. caller가 반환 pipeline의 수명과 저장을 가지며 owner는 호출 간 상태를
보유하지 않습니다. OVL-28 owner는 새 fixture-shape/XML 결함·계약 변경·입증된
책임 충돌 없이는 다시 나누지 않으며, 남은 실행/summary/UI 코드는 파일 크기만으로
분할하지 않습니다.

### 9.22 smoke Recipe workspace cleanup 책임을 읽는 순서

OVL-29는 `PipelineViewerScreenshotSmoke/Program.cs`에 반복되던 임시 Recipe
workspace 선택·삭제 정책을 `SmokeRecipeWorkspaceCleanup` concrete owner로
옮겼습니다. owner는 `Default`와 호출자가 보존한 이름을 제외하고, 예약된
`Smoke_`/`Recipe_` 접두사만 선택한 뒤 기존 `RecipeWorkspaceService`를 통해
삭제합니다. `Program`은 각 target의 실행 순서와 keep 이름만 조합합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | 21개 cleanup 호출의 target 순서와 선택적 keep 이름 전달 |
| 2. cleanup owner | `tools/PipelineViewerScreenshotSmoke/SmokeRecipeWorkspaceCleanup.cs` | keep 집합, 접두사 필터, 기존 Recipe workspace 삭제 경계 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeRecipeWorkspaceCleanupContract.cs` | Program 위임, old helper 제거, 보존·접두사·순서 정책 |
| 4. 저장 경계 | `src/OpenVisionLab/Core/Recipe/RecipeWorkspaceService.cs` | Recipe 이름 조회와 안전한 workspace 삭제 |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl29-smoke-recipe-workspace-cleanup-contract-20260909-run2`, `ovl29-smoke-recipe-workspace-cleanup-contract-20260909-release` | Debug/Release 6/6 계약 결과 |

`SmokeRecipeWorkspaceCleanup`은 interface, factory, wrapper chain 없이 concrete
internal owner입니다. OVL-28의 fixture construction owner와 target별 fixture
실행·summary/UI 조합은 다시 나누지 않습니다. 새 workspace 수명 결함, 명시적
계약 변경, 또는 입증된 책임 충돌이 있을 때만 별도 slice를 엽니다.

### 9.24 validation dataset configuration 책임을 읽는 순서

OVL-31은 `PipelineViewerScreenshotSmoke/Program.cs`에 남아 있던 local
validation dataset의 환경·입력 준비 책임을 `ValidationDatasetSmokeConfiguration`
concrete owner로 옮겼습니다. owner는 `OPENVISIONLAB_VALIDATION_*` 입력,
OK/NG 폴더 선택, per-role limit, 기본 Matching baseline 파일 경로,
pipeline/suite/boundary 기본값과 baseline XML 치환을 담당합니다. `Program`은
반환된 configuration을 사용해 Recipe 저장, 명시적 validation-set 실행,
progress, summary, artifact와 Run History/UI 검증을 조합합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행/조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | configuration 호출, Recipe 저장, explicit validation run, progress와 summary/UI 검증 |
| 2. configuration owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetSmokeConfiguration.cs` | 환경 입력·폴더·pipeline XML/name·suite·boundary·per-role limit 정규화 |
| 3. artifact owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetArtifactWriter.cs` | OVL-27/30 CSV/evidence와 pipeline/batch/audit JSON 출력 |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetSmokeConfigurationContract.cs` | Program 위임, WPF-free owner, custom/default path와 XML, fail-closed 입력 |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl31-validation-dataset-configuration-contract-debug-20260909`, `ovl31-validation-dataset-configuration-contract-release-20260909` | Debug/Release 7/7 계약과 회귀 계약 |

`ValidationDatasetSmokeConfiguration`은 interface, factory, wrapper chain 없이
한 번 생성되어 호출자에게 전달되는 concrete owner입니다. OVL-27/30 artifact
owner와 dataset 실행·summary/UI 조합은 다시 분리하지 않으며, 새 configuration
결함·명시적 계약 변경·입증된 책임 충돌이 있을 때만 별도 slice를 엽니다.

### 9.25 validation dataset execution progress 책임을 읽는 순서

OVL-32는 `Program`에 남아 있던 local validation dataset의 실행 시작 이후
progress 파일 기록, UI pump, 저장 Run History 대기와 deadline 상태를
`ValidationDatasetExecutionProgress` concrete owner로 옮겼습니다. owner는
Shell/WPF 타입을 직접 참조하지 않고 command 실행·상태·저장 여부·clock을
callback으로 받습니다. `Program`은 validation-set 생성/이미지 등록과 summary,
artifact, Run History/drawing UI 조합을 계속 소유합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행/조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | validation-set 등록 후 callback을 연결하고 summary/UI 검증을 이어가는 target 흐름 |
| 2. execution progress owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetExecutionProgress.cs` | 등록 결과 기록, 기존 2초 상태 checkpoint, UI pump, 저장 Run 대기, 10분 deadline |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetExecutionProgressContract.cs` | call path, callback 경계, WPF 독립성, 조기 완료와 deadline progress 보존 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl32-validation-dataset-execution-progress-contract-debug3-20260909`, `ovl32-validation-dataset-execution-progress-contract-release3-20260909` | Debug/Release 7/7 계약과 OVL-22~31 회귀 |

`ValidationDatasetExecutionProgress`는 interface, factory, wrapper chain 없이
단일 실행 target에 필요한 상태와 시간 경계를 concrete owner로 가집니다.
OVL-27/30 artifact, OVL-31 configuration, 또는 남은 summary/UI 책임을 파일
크기만으로 다시 나누지 않으며, 새 실행 수명 결함·명시적 계약 변경·입증된
책임 충돌이 있을 때만 별도 slice를 엽니다.

### 9.26 validation dataset review-queue evidence 책임을 읽는 순서

OVL-33은 `Program`의 `captureReviewQueue` 블록에 섞여 있던 persisted
review-queue 의미 검증, filter 상태 전환, Preview/Layer/route side-effect
guard, panel 가시화, saved summary 복사와 contract 산출을
`ValidationDatasetReviewQueueEvidence` concrete owner로 옮겼습니다. 기존
visual-tree lookup은 smoke 공통 helper를 재사용하기 위해 조합부에 남아
있습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행/조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | review-queue panel lookup과 owner 호출, summary/Recipe/drawing 흐름 |
| 2. review-queue evidence owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetReviewQueueEvidence.cs` | Pitch metric/SHA identity, filter, workspace 불변식, panel visibility, saved summary와 contract 파일 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetReviewQueueEvidenceContract.cs` | old block 제거, owner call path, pure projection과 exact artifact contents |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl33-validation-dataset-review-queue-evidence-contract-debug-20260909`, `ovl33-validation-dataset-review-queue-evidence-contract-release-20260909` | Debug/Release 6/6 계약과 OVL-22~32 회귀 |

`ValidationDatasetReviewQueueEvidence`는 기존 `OpenVisionRecipeRunHistoryPresenter`
또는 OVL-30 artifact owner를 대체하지 않는 smoke evidence concrete owner입니다.
새 review-queue 결함·명시적 계약 변경·입증된 책임 충돌 없이 owner를 다시
나누거나 남은 drawing-evidence UI를 파일 크기로 분할하지 않습니다.

### 9.29 WpfPropertyGridAdapter generic metadata 책임을 읽는 순서

OVL-36는 `WpfPropertyGridAdapter.cs`에 함께 선언되어 있던 generic
PropertyGrid metadata helper를 concrete owner로 이동했습니다. 새 파일은
category/property 순서 비교, dynamic type-description projection,
localized descriptor와 localization-key fallback을 소유합니다. `PropertyGrid`
는 vendor control lifecycle, selected object, hidden-property registry,
progressive viewport, navigation, editor registration, event forwarding과
mutable state를 계속 소유합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. adapter composition | `src/Libraries/WpfPropertyGridBridge/WpfPropertyGridAdapter.cs` | `EnsurePropertyGridProvider`/`RegisterComparers` 호출과 기존 PropertyGrid state/lifecycle owner |
| 2. generic metadata owner | `src/Libraries/WpfPropertyGridBridge/PropertyGridMetadataAdapters.cs` | `BridgeCategoryOrderMap`, comparer 2종, dynamic descriptor/provider, localized descriptor, localization helper |
| 3. 독립 계약 | `tools/VisionRecipeRunnerSmoke/PropertyGridMetadataAdapterContract.cs` | 단일 concrete owner, token hash, composition/delegation, hidden/range/viewport, localization과 application-policy 경계 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl36-metadata-contract-debug2-20260909`, `ovl36-metadata-contract-release-20260909`, `ovl36-ui-property-grid-release2-20260909` | Debug/Release 7/7 계약, OVL-20 8/8 회귀, 동적 monitor-aware PropertyGrid EXE 화면 |

이 metadata owner는 `PropertyGridToolPolicy`나 OVL-20 subscription owner를
대체하지 않습니다. 새 결함·명시적 계약 변경·입증된 dependency 충돌 없이
metadata helper를 다시 나누거나 `WpfPropertyGridAdapter`를 파일 크기로
분할하지 않습니다.

### 9.45 RefreshOptions composite command-state 경계를 읽는 순서

OVL-53 재감사에서 `RefreshOptions`가 `SetSelectedRecipeName`을 통해
`RefreshPipelineOptions`와 `RefreshValidationSetOptions`를 연속 호출하고,
각 내부 갱신과 outer method가 모두 `RefreshCommandState`를 실행하는 중복을
확인했습니다. 같은 경로의 summary/recent-run setter도 일부 command-state
binding을 직접 알리고 있었습니다. 두 하위 갱신과 해당 setter는 상태와
binding projection을 그대로 수행하되 composite 경로에서는 겹치는 알림을
defer하고, `RefreshOptions`의 마지막 `RefreshCommandState`가 shared
projection을 한 번 소유합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Composite entry | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/Handlers.cs` | `RefreshOptions`의 re-entry guard, recipe option 갱신, outer command-state projection |
| 2. Recipe selection projection | 같은 파일 | `SetSelectedRecipeName`의 state reset과 nested refresh policy 전달 |
| 3. Pipeline projection | 같은 파일 | `RefreshPipelineOptions`의 pipeline/summary 상태와 `refreshCommandState` defer 경계 |
| 4. Validation Set projection | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | selection/row/evidence 상태와 `refreshCommandState` defer 경계 |
| 5. Shared command-state owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/Handlers.cs` | `RefreshCommandState`가 10개 command-state binding을 한 번 알림 |
| 6. 독립 계약 | `tools/VisionRecipeRunnerSmoke/RefreshOptionsCommandStateContract.cs` | composite refresh에서 각 command-state binding이 정확히 1회 알림 |
| 7. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | outer owner와 두 nested defer call path가 유지되는지 확인 |

호출과 상태 흐름은 다음과 같습니다.

```text
RefreshOptions
  -> SetSelectedRecipeName(current, refreshCommandState: false)
  -> RefreshPipelineOptions(..., false)
  -> RefreshValidationSetOptions(refreshCommandState: false)
  -> RecipeOptions / filter projection
  -> RefreshCommandState (outer, once)
```

Pipeline/Validation Set의 직접 호출자는 기본값 `true`를 계속 사용하므로
기존 직접 갱신의 command-state 동작은 유지됩니다. Pipeline/XML, Validation
Set document/XML, Preview/Run 명시 실행, binding 이름과 mutable-state owner는
변경하지 않았습니다. OVL-50/51/52의 완료 owner는 이 composite caller 수정의
전제이며 다시 나누지 않습니다. 다른 모델·agent·automation은 새 재현 결함·
명시적 계약 변경·dependency 충돌 없이는 이 경계를 재생성·이동·재분할·복제하지
않습니다.

### 9.44 Validation Set success projection 경계를 읽는 순서

OVL-52 재감사에서 `RefreshValidationSetOptions` 성공 branch가
`ValidationSuiteSummaryText`를 직접 알린 뒤 `RefreshCommandState`가 같은
알림을 다시 보내는 중복을 확인했습니다. 직접 알림을 제거하고 shared
command-state owner를 유지했습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 성공 branch projection | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | `RefreshValidationSetOptions`의 load, selection/row, summary/evidence 알림 순서 |
| 2. Command-state owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/Handlers.cs` | `RefreshCommandState`의 단일 `ValidationSuiteSummaryText` 알림 |
| 3. Binding properties | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` | selection summary와 suite summary getter 의존성 |
| 4. Selection mutable state | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` | `Refresh`가 쓰는 선택·image-row 상태 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/ValidationSetSuccessProjectionContract.cs` | 성공 refresh에서 summary/evidence 각 1회 알림 |
| 6. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | 직접 suite-summary 알림 제거와 shared owner 유지 |

호출과 상태 흐름은 다음과 같습니다.

```text
RefreshValidationSetOptions
  -> ValidationSetDocumentOwner.TryLoad
  -> ValidationSetSelectionOwner.Refresh
  -> options/selection/split/rows PropertyChanged
  -> Variant/identity/evidence projection
  -> ValidationSetSelectionSummaryText PropertyChanged
  -> RefreshCommandState -> ValidationSuiteSummaryText PropertyChanged (once)
```

`StorageReady`와 document는 document owner가, 선택·행 상태는 selection
owner가, WPF 알림 순서는 Shell이 계속 소유합니다. Recipe/XML,
Preview/Run 계약은 변경하지 않았습니다. 별도 `RefreshOptions` composite
caller의 중첩 갱신은 이후 감사 경계이며 OVL-52 owner를 다시 나누는 사유가
아닙니다. 다른 모델·agent·automation은 새 재현 결함·명시적 계약 변경·
dependency 충돌 없이는 이 경계를 재생성·이동·재분할·복제하지 않습니다.

### 9.43 Validation Set projection error branch 경계를 읽는 순서

OVL-51 재감사에서 malformed `validation-sets.xml`을 읽을 때
`RefreshValidationSetOptions`가 선택·행 상태를 비우면서도
`ValidationSetSelectionSummaryText` 알림을 누락하는 결함을 확인했습니다.
오류 branch가 선택 summary를 한 번 알린 뒤 기존 status/identity/evidence/
command-state projection을 수행하도록 최소 수정했습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 오류 branch projection | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | `RefreshValidationSetOptions`의 load failure, clear, summary 알림 순서 |
| 2. Document/XML owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` | `TryLoad`, `StorageReady`, 오류 반환 |
| 3. Selection mutable state | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` | `Clear`와 비어 있는 options/image rows |
| 4. Binding property | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` | `ValidationSetSelectionSummaryText`의 localized read-error 결과 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/ValidationSetProjectionErrorContract.cs` | malformed XML 후 summary 1회 알림과 cleared option 확인 |
| 6. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | 오류 branch 안의 selection summary projection 확인 |

호출과 상태 흐름은 다음과 같습니다.

```text
RefreshValidationSetOptions
  -> ValidationSetDocumentOwner.TryLoad
  -> (failure) ValidationSetSelectionOwner.Clear
  -> options/selection/rows/summary/Variant PropertyChanged
  -> status/identity/evidence/command-state projection
```

`StorageReady`와 document는 document owner가, cleared mutable state는
selection owner가, WPF 알림 순서는 Shell이 계속 소유합니다. 성공 branch와
Validation Set document/XML, Recipe/XML, Preview/Run 계약은 변경하지
않았습니다. OVL-51은 새 결함·명시적 binding/Recipe 계약 변경·dependency
충돌 없이는 다른 모델·agent·automation이 재생성·이동·재분할·복제하지
않습니다.

### 9.42 Validation Set evidence `PropertyChanged` projection 경계를 읽는 순서

OVL-50 재감사에서 Validation Set 선택 변경 시
`RefreshValidationSetImageRows`와 `SelectedValidationSetOption` setter가 같은
네 개의 evidence binding에 각각 알림을 보내는 중복 경로를 확인했습니다.
image-row helper는 행과 pending Variant 상태만 갱신하고, 네 evidence 알림은
선택 setter가 한 번만 발생시키도록 정리했습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 선택 setter projection | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` | `SelectedValidationSetOption`의 selection/summary/evidence/command 알림 순서 |
| 2. Image-row helper | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | `RefreshValidationSetImageRows`, pending Variant projection; evidence 알림을 소유하지 않음 |
| 3. Selection mutable state | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` | `SelectSet`, `RefreshImageRows` 상태 변경 |
| 4. Evidence policy | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationEvidenceOwner.cs` | WPF와 분리된 acceptance/calibration 결과 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/ValidationSetEvidenceNotificationContract.cs` | 한 번의 set 선택에 네 evidence binding이 각각 1회인지 확인 |
| 6. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | helper 중복 호출 제거와 setter projection owner 확인 |

호출과 상태 흐름은 다음과 같습니다.

```text
SelectedValidationSetOption setter
  -> ValidationSetSelectionOwner.SelectSet
  -> RefreshValidationSetImageRows
  -> ValidationSetSelectionOwner.RefreshImageRows
  -> Shell row/selected/pending PropertyChanged
  -> Shell selection/summary/evidence PropertyChanged (각 1회)
  -> RefreshCommandState
```

WPF binding-facing 알림 순서는 Shell이, 선택 mutable state는 selection owner가,
acceptance/calibration 정책은 기존 evidence owner가 계속 소유합니다. 새
owner·wrapper·interface는 필요하지 않았습니다. OVL-50은 새 결함·명시적
binding/Recipe 계약 변경·dependency 충돌 없이는 다른 모델·agent·automation이
재생성·이동·재분할·복제하지 않습니다.

### 9.41 PinArrayGap frozen split-name 복원 경계를 읽는 순서

OVL-49 재감사에서 `RefreshValidationSetOptions`가
`OpenVisionRecipePinArrayGapValidationRecordStorage.TryLoad`를 직접 호출해
OVL-46 identity owner를 우회하는 잔여 결합을 발견했습니다. 기존
`OpenVisionRecipePinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames`
가 record를 읽고 Train/Validation/Test 이름만 반환하도록 이동했으며, Shell은
그 결과를 기존 selection owner와 WPF `PropertyChanged` projection에
전달합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Shell 선택 복원 호출 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | `RefreshValidationSetOptions`, frozen split-name projection, 기존 알림 순서 |
| 2. Identity owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationIdentityOwner.cs` | `TryGetFrozenSelectionNames`, Freeze, Evaluate |
| 3. Record persistence | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationRecordStorage.cs` | `pin-row-edge-gap-v1.xml` 읽기와 shape 검증 |
| 4. Selection mutable state | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetSelectionOwner.cs` | Train/Validation/Test와 image-row 선택 상태 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/PinArrayGapValidationIdentityOwnerContract.cs` | frozen split-name restoration을 포함한 5개 계약 |
| 6. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | Shell direct record load 제거와 owner delegation |

호출과 상태 흐름은 다음과 같습니다.

```text
RefreshValidationSetOptions
  -> ValidationSetDocumentOwner.TryLoad
  -> PinArrayGapValidationIdentityOwner.TryGetFrozenSelectionNames
  -> ValidationSetSelectionOwner.Refresh(previous split names)
  -> Shell PropertyChanged / variant / identity / evidence / command projection
```

Record XML persistence는 기존 storage가, Validation Set document는
`OpenVisionRecipeValidationSetDocumentOwner`가, 선택 mutable state는
`OpenVisionRecipeValidationSetSelectionOwner`가 계속 소유합니다. 새 owner는
WPF나 파일 시스템을 노출하지 않습니다. 이 경계는 새 identity 결함·계약
변경·dependency 충돌 없이는 다른 모델·agent·automation이 재생성·이동·
재분할·복제하지 않습니다.

### 9.40 Validation Set 저장 실패 상태 문구와 복원 경계를 읽는 순서

OVL-48 audit에서 `TrySaveValidationSetDocument`의 실패 후
`RefreshValidationSetOptions`가 선택 owner, `PropertyChanged`, PinArrayGap
identity, evidence, command-state를 함께 갱신하는 Shell application
projection임을 확인했습니다. 이 mutable UI 경계를 새 recovery wrapper로
나누지 않았습니다. 저장 실패 문구의 직접 문자열 조합만 기존
`OpenVisionRecipeValidationSetPresenter.BuildSaveErrorStatus`로 이동했습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Shell 저장/복원 순서 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | `TrySaveValidationSetDocument`, 실패 후 `RefreshValidationSetOptions`, 기존 `ValidationSuiteStatusText` binding |
| 2. 저장 실패 문구 owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeValidationSetPresenter.cs` | `BuildSaveErrorStatus`와 기존 Validation Set status builders |
| 3. document persistence owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` | `TrySave`, mutable document와 `StorageReady` |
| 4. XML storage owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetStorage.cs` | schema/variant/limit 검증과 `validation-sets.xml` 저장 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/ValidationSetStatusPresenterContract.cs` | 한국어/영어 저장 실패 문구를 포함한 status contract |
| 6. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | Shell 직접 save-error concat 제거와 presenter delegation |

호출과 상태 흐름은 다음과 같습니다.

```text
Create/Delete/Add/Variant/Repair/Remove command
  -> TrySaveValidationSetDocument(operation)
  -> ValidationSetDocumentOwner.TrySave
  -> (failure) Shell RefreshValidationSetOptions
  -> ValidationSetPresenter.BuildSaveErrorStatus
  -> ValidationSuiteStatusText binding
```

`RefreshValidationSetOptions`는 파일 저장 자체가 아니라 Shell의 선택·
알림·화면 상태를 갱신하므로, 별도 owner를 만들면 현재 mutable state와
binding callback을 다시 연결하는 wrapper가 됩니다. 새 결함·명시적 계약
변경·독립 테스트 seam이 없는 한 이 경계를 다시 나누지 않습니다.

### 9.39 Validation Set 폴더 등록 상태 문구를 읽는 순서

OVL-47은 `Recipe/CommandSurface/ValidationSets.cs`에
남아 있던 Validation Set 폴더 등록 결과 문구 조합을 기존
`OpenVisionRecipeValidationSetPresenter`로 이동했습니다. 폴더 열거는
`OpenVisionRecipeValidationSetStorage.TryGetTopLevelImagePaths`, 문서의
mutable 변경은 `OpenVisionRecipeValidationSetDocumentOwner.TryAddImages`,
저장과 refresh 순서는 Shell이 계속 소유합니다. Recipe/XML과 명시적
Preview/Run 계약은 변경하지 않았습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Shell 호출 순서 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/ValidationSets.cs` | `AddValidationSetFolder`, `AddValidationSetImages`, `TrySaveValidationSetDocument`, 기존 `ValidationSuiteStatusText` binding projection |
| 2. 상태 문구 owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeValidationSetPresenter.cs` | `BuildFolderImageRegistrationError`, `BuildEmptyFolderImageRegistrationStatus`, `BuildImageRegistrationStatus` |
| 3. 파일 수집 owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetStorage.cs` | 지원 확장자, top-level 열거, 정렬·상한·오류 결과 |
| 4. 문서 mutation owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipeValidationSetDocumentOwner.cs` | 이미지 추가/갱신/건너뜀과 Validation Set document 상태 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/ValidationSetStatusPresenterContract.cs` | 한국어/영어 오류·빈 폴더·수량 문구와 D: 증거 |
| 6. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | Shell의 중복 localized literal 제거와 presenter delegation |

호출과 상태 흐름은 다음과 같습니다.

```text
AddValidationSetFolder
  -> ValidationSetStorage.TryGetTopLevelImagePaths
  -> ValidationSetDocumentOwner.TryAddImages
  -> TrySaveValidationSetDocument
  -> RefreshValidationSetOptions
  -> ValidationSetPresenter.BuildImageRegistrationStatus
  -> Shell ValidationSuiteStatusText binding
```

열거 실패와 빈 폴더는 각각 presenter의 전용 builder를 거쳐 상태에
투영됩니다. presenter는 WPF·Window·파일 시스템을 참조하지 않는 순수
문구 정책이며, mutable state와 lifetime owner를 추가하지 않습니다. 이
owner는 새 결함·명시적 문구/계약 변경·입증된 dependency 충돌 없이는
다른 모델·agent·automation이 재생성·이동·재분할·복제하지 않습니다.

### 9.38 Shell PinArrayGap validation identity 책임을 읽는 순서

OVL-46은 `Recipe/CommandSurface/Handlers.cs`에 남아 있던
PinArrayGap 2단계 검증 기준의 Pipeline XML 원문 읽기와 Freeze/Evaluate
orchestration을 `OpenVisionRecipePinArrayGapValidationIdentityOwner`
concrete owner로 이동했습니다. Shell은 선택된 Train/Validation/Test,
`CanExecute`, localized status, frozen flag와 명시적 Validation Set 실행
routing을 계속 소유하고, 기존
`OpenVisionRecipePinArrayGapValidationRecordStorage`는 XML·row·split hash와
record persistence를 계속 담당합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Shell 호출 조합 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/Handlers.cs` | `FreezePinArrayGapValidationIdentity`와 `RefreshPinArrayGapValidationIdentityState`, mutable selection/status projection |
| 2. identity workflow owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationIdentityOwner.cs` | 선택 Pipeline XML 원문 읽기, Freeze/Evaluate result contract |
| 3. identity storage owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationRecordStorage.cs` | Pipeline/row/split hash, Train/Validation/Test disjointness, XML record 저장·조회·비교 |
| 4. 독립 계약 | `tools/VisionRecipeRunnerSmoke/PinArrayGapValidationIdentityOwnerContract.cs` | Freeze/current/stale/missing XML과 Recipe XML no-mutation을 D: 격리 데이터로 검증 |
| 5. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | Handler의 직접 XML/record orchestration 제거와 owner delegation 확인 |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl46-pinarraygap-validation-identity-owner-contract-debug-20260909-run1`, `ovl46-pinarraygap-validation-identity-owner-contract-release-20260909-run1` | Debug/Release 4/4 계약과 app/readiness/audit 결과 |

호출과 상태 흐름은 다음과 같습니다.

```text
Freeze command
  -> Shell FreezePinArrayGapValidationIdentity
  -> PinArrayGapValidationIdentityOwner.Freeze
  -> raw Pipeline XML read
  -> PinArrayGapValidationRecordStorage.TrySave
  -> Shell frozen status/command-state projection

Pipeline/selection refresh
  -> Shell RefreshPinArrayGapValidationIdentityState
  -> PinArrayGapValidationIdentityOwner.Evaluate
  -> raw Pipeline XML read + record TryLoad/TryMatchesCurrent
  -> Shell NOT FROZEN / REVIEW / STALE / FROZEN status projection
```

`OpenVisionRecipePinArrayGapValidationIdentityOwner`는 WPF, Window,
Clipboard, execution session, Layer/ImageSpace를 참조하지 않는 호출별 결과
owner입니다. XML을 재직렬화하지 않고 원문을 읽어 기존 hash 계약을
보존합니다. `OpenVisionRecipePinArrayGapValidationRecordStorage`가
identity schema와 파일 persistence의 canonical owner이므로 새 owner는 그
정책을 복제하지 않습니다. 새 identity 결함, 계약 변경, 입증된 dependency
충돌 없이는 이 owner를 다시 나누거나 복제하지 않습니다.

### 9.37 Shell LLM draft review baseline 책임을 읽는 순서

OVL-45는 `OpenVisionShellHostRecipeCommandSurface`에 남아 있던 LLM XML
초안 import/diff 검토의 active Pipeline XML 조회를
`OpenVisionRecipeLlmDraftReviewOwner` concrete owner로 이동했습니다.
CommandSurface는 `LlmXmlDraft*` 바인딩 상태, 검증 결과, Import readiness와
명시적 Import/Preview/Run 경계를 계속 소유하고, 기존
`OpenVisionRecipePipelineComparisonPresenter`는 구조/parameter diff 문구를
계속 소유합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Shell 호출 조합 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/LlmXmlDraftWorkflow.cs` | `ValidateLlmXmlDraftText`와 draft load/import 호출 경로, UI projection |
| 2. active baseline review owner | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeLlmDraftReviewOwner.cs` | Recipe 이름 정규화, active Pipeline 선택/로드, import/diff 결과 반환 |
| 3. comparison presenter | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipePipelineComparisonPresenter.cs` | read-only draft import review와 구조/parameter diff formatting |
| 4. 독립 계약 | `tools/VisionRecipeRunnerSmoke/LlmDraftReviewOwnerContract.cs` | persisted baseline, no-mutation, null draft, D: evidence |
| 5. 정적 gate | `tools/OpenVisionReadinessCheck/Program.cs` | former helper 제거와 owner delegation 확인 |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl45-llm-draft-review-owner-contract-debug-20260909-run1`, `ovl45-llm-draft-review-owner-contract-release-20260909-run1` | Debug/Release 3/3 계약과 readiness pass |

호출과 상태 흐름은 다음과 같습니다.

```text
LLM draft load/create/validate
  -> ValidateLlmXmlDraftText
  -> llmDraftReviewOwner.Build(recipe, draft)
  -> active Pipeline storage read
  -> PipelineComparisonPresenter
  -> CommandSurface LlmXmlDraft* / Import readiness projection
```

`OpenVisionRecipeLlmDraftReviewOwner`는 무상태이며 호출당 결과만 반환합니다.
Recipe 저장·활성화·Preview·Run, locator 승인 gate, WPF binding, Window,
Clipboard와 execution lifetime은 caller가 소유합니다. 기존
`BuildPipelineVariantComparisonReport`는 별도 variant 비교 경로이므로 이
owner에 합치지 않았습니다. 새 draft-review 결함, 계약 변경, 입증된
dependency 충돌 없이는 이 owner를 다시 나누거나 복제하지 않습니다.

### 9.36 Smoke mouse input 책임을 읽는 순서

OVL-44는 `OpenVisionLabDirectSmokeRunner`에 남아 있던 저수준 Win32 마우스
입력 책임을 `SmokeMouseInput` concrete owner로 이동했습니다. WPF 좌표
계산, Dispatcher pump, docking 상태 판정과 시나리오 순서는 Direct caller가
계속 소유합니다. 완료된 screenshot, monitor, docking state, task waiter,
clipboard owner를 다시 나누지 않습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Direct 시나리오 조합 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | WPF 좌표 계산, click/drag 호출, Pump와 docking 상태 판정 |
| 2. mouse input owner | `tools/PipelineViewerScreenshotSmoke/SmokeMouseInput.cs` | user32 cursor/button injection, 좌표 반올림, drag interpolation, input thread lifetime |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeMouseInputContract.cs` | Direct 위임, old Win32 선언 제거, timing/thread/error, dependency와 rounding checks |
| 4. embedded composition | `src/OpenVisionLab/OpenVisionLab.csproj` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`일 때 owner source를 조건부 Link |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl44-smoke-mouse-input-contract-debug-20260909-run3`, `ovl44-smoke-mouse-input-contract-release-20260909-run2` | Debug/Release 10/10 계약, monitor/task/command-line 회귀와 4개 빌드 |

호출과 상태 흐름은 다음과 같습니다.

```text
Direct docking scenario -> WPF coordinate/state owner
  -> SmokeMouseInput.Release/Press/SetCursor/Drag(..., () => Pump(1))
  -> input thread -> user32 cursor/button calls -> Join -> scenario assertion
```

`SmokeMouseInput`는 per-call thread, exception, point 변환만 보유하며 static
mutable state를 남기지 않습니다. Window 생성·표시·종료, Dispatcher affinity,
docking document/layer/route 상태는 Direct caller가 소유합니다. owner는
WPF `Point`, user32, threading만 참조하고 Shell, Recipe, SDK, 파일 저장소,
제품 모듈을 참조하지 않습니다. 새 interface/factory/wrapper chain은
추가하지 않았습니다. 새 입력 결함, gesture/timeout 계약 변경, 또는 입증된
dependency 충돌 없이는 이 owner를 다시 나누거나 복제하지 않습니다.

### 9.35 Smoke Window monitor placement 책임을 읽는 순서

OVL-43은 `OpenVisionLabDirectSmokeRunner`의 20개 호출이 함께 사용하던
Win32 모니터 열거·왼쪽 모니터 선택·창 중앙 배치·실제 교차 검증을
`SmokeWindowMonitorPlacement` concrete owner로 이동했습니다. PNG 출력,
clipboard retry, task wait, docking state owner와는 다른 환경 어댑터이며,
Direct runner는 기존 wrapper와 시나리오 호출 정책을 유지합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Direct 호출 조합 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | 기존 `PlaceWindowOnLeftmostMonitor` wrapper와 20개 시나리오 호출 |
| 2. monitor placement owner | `tools/PipelineViewerScreenshotSmoke/SmokeWindowMonitorPlacement.cs` | Win32 monitor/window adapter, leftmost selection, centered placement, intersection guard |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeWindowMonitorPlacementContract.cs` | Direct 위임, old Win32 code 제거, conditional link, deterministic geometry/intersection/evidence checks |
| 4. embedded composition | `src/OpenVisionLab/OpenVisionLab.csproj` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`일 때 owner source를 조건부 Link |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl43-smoke-window-monitor-placement-contract-debug-20260909-run1`, `ovl43-smoke-window-monitor-placement-contract-release-20260909-run1` | Debug/Release 9/9 계약, Pipeline Smoke와 embedded OpenVisionLab build |

호출과 상태 흐름은 다음과 같습니다.

```text
Direct scenario -> existing PlaceWindowOnLeftmostMonitor wrapper
  -> SmokeWindowMonitorPlacement.PlaceOnLeftmostMonitor
  -> Win32 monitor enumeration/SetWindowPos -> actual intersection evidence
```

`SmokeWindowMonitorPlacement`는 per-call monitor 목록과 native rectangle만
보유하며 static mutable state를 남기지 않습니다. Window 생성·표시·종료,
Dispatcher affinity, scenario timing은 Direct caller가 소유합니다. owner는
WPF `Window`/interop와 user32만 참조하고 Shell, Recipe, 파일 시스템, SDK,
제품 모듈을 참조하지 않습니다. 새 interface·factory·wrapper chain을
추가하지 않았습니다. 새 monitor-placement 결함, 명시적 선택/배치 계약
변경, 또는 입증된 dependency 충돌 없이는 이 owner를 다시 나누거나
복제하지 않습니다.

### 9.34 Smoke Task waiter 책임을 읽는 순서

OVL-42는 `OpenVisionLabDirectSmokeRunner`와
`PipelineViewerScreenshotSmoke.Program`에 중복되어 있던 pump 기반
`WaitForTaskWithPump` loop를 WPF-free `SmokeTaskWaiter` concrete owner로
통합했습니다. owner는 task 완료 대기, caller pump 호출, delay, timeout,
완료 예외 전파를 담당합니다. Direct와 Pipeline wrapper는 각각의 null 처리,
timeout 값, timeout 문구, `Pump(4)` callback을 유지합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Direct 호출 조합 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | required-task wrapper, 20초 정책, `Pump(4)` callback |
| 2. Pipeline 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | optional-task wrapper, per-call timeout, `Pump(4)` callback |
| 3. task wait owner | `tools/PipelineViewerScreenshotSmoke/SmokeTaskWaiter.cs` | pump/delay/timeout loop와 task exception propagation |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeTaskWaiterContract.cs` | 두 caller 위임, WPF-free 경계, timeout/pump/exception/input 검증 |
| 5. embedded composition | `src/OpenVisionLab/OpenVisionLab.csproj` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`일 때 owner source를 조건부 Link |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl42-smoke-task-waiter-contract-20260909` | Debug/Release 8/8 계약, OVL-22 10/10, OVL-40 6/6, OVL-41 7/7, 4개 build |

호출과 상태 흐름은 다음과 같습니다.

```text
Direct/Pipeline scenario -> existing WaitForTaskWithPump wrapper
  -> SmokeTaskWaiter.Wait -> caller Pump(4) -> task result/exception
```

`SmokeTaskWaiter`는 상태를 보유하지 않으며 WPF, Shell, Recipe, 파일
시스템, 제품 모듈을 참조하지 않습니다. task 생성·수명과 dispatcher affinity는
caller가 소유하고, wait loop와 완료 예외 전파만 owner가 소유합니다. 새
interface·factory·wrapper chain을 추가하지 않았습니다. 새 task-wait 결함,
명시적 timeout 계약 변경, 입증된 dependency 충돌 없이는 이 owner를 다시
나누거나 복제하지 않습니다.

### 9.33 Smoke clipboard retry 책임을 읽는 순서

OVL-41은 `OpenVisionLabDirectSmokeRunner`와
`PipelineViewerScreenshotSmoke.Program`에 중복되어 있던 COM clipboard
재시도 loop를 WPF-free `SmokeClipboardRetry` concrete owner로 통합했습니다.
owner는 target HRESULT `0x800401D0`만 재시도하고, 최대 시도 수·지연·마지막
예외 전파를 담당합니다. 두 caller는 기존 `System.Windows.Clipboard` wrapper와
`Pump(4)` dispatcher callback을 유지하므로 Clipboard 접근과 UI thread
affinity가 scenario에 남아 있습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Direct 호출 조합 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | 기존 get/set wrapper와 `Pump(4)` callback |
| 2. Pipeline 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | 기존 get/set wrapper와 동일한 owner 위임 |
| 3. retry owner | `tools/PipelineViewerScreenshotSmoke/SmokeClipboardRetry.cs` | HRESULT filter, 40회 시도, delay, pump callback, exception propagation |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeClipboardRetryContract.cs` | 두 caller 위임, old loop 제거, WPF-free 경계, retry/exception/null-input 검증 |
| 5. embedded composition | `src/OpenVisionLab/OpenVisionLab.csproj` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`일 때 owner source를 조건부 Link |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl41-smoke-clipboard-retry-contract-20260909` | Debug/Release 7/7 계약, OVL-22 10/10, OVL-40 6/6, 4개 build |

호출과 상태 흐름은 다음과 같습니다.

```text
Direct/Pipeline scenario -> existing Clipboard wrapper -> SmokeClipboardRetry.Run
  -> caller-owned Clipboard action; retry owner invokes caller Pump(4)
```

`SmokeClipboardRetry`는 상태를 보유하지 않으며 Clipboard, WPF, Shell,
Recipe, 파일 시스템을 참조하지 않습니다. 새 interface·factory·wrapper chain을
추가하지 않았습니다. 새 clipboard 결함·명시적 retry 계약 변경·입증된
dependency 충돌 없이는 이 owner를 다시 나누거나 복제하지 않습니다.

### 9.32 Direct Smoke screenshot PNG 책임을 읽는 순서

OVL-40은 조건부 embedded `OpenVisionLabDirectSmokeRunner`에 남아 있던 세
가지 PNG 출력 구현을 기존 `ScreenshotPngWriter`로 통합했습니다. DPI-aware
WPF 창 렌더링, 단일 창 화면 복사, 여러 visible Window의 union 화면 복사와
PNG 인코딩은 이제 한 output owner가 담당합니다. Direct runner는 기존
scenario 이름과 wrapper를 유지하면서 `BringWindowToFront`, layout update,
visible-window 검증, dispatcher pump 순서만 조합합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Direct scenario 조합 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | 기존 `SaveWindowScreenshot`, `SaveWindowScreenScreenshot`, `SaveWindowsScreenScreenshot` 호출과 activation/pump wrapper |
| 2. PNG output owner | `tools/PipelineViewerScreenshotSmoke/ScreenshotPngWriter.cs` | `WriteDpiAwareWindowPng`, `WriteWindowScreenPng`, `WriteWindowsScreenPng`, WPF render, screen coordinates, union bounds, PNG 저장 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/DirectSmokeScreenshotWriterContract.cs` | Direct wrapper 위임, old rendering removal, conditional app link, 실제 WPF window PNG 출력 |
| 4. embedded composition | `src/OpenVisionLab/OpenVisionLab.csproj` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`일 때 기존 writer source를 한 번만 compile하는 조건부 Link |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\direct-smoke-screenshot-owner-20260909` | Direct 6/6, existing PNG writer 7/7, capture lifecycle 10/10, OVL-22 10/10, Debug/Release build |

호출과 상태 흐름은 다음과 같습니다.

```text
Direct scenario -> activation/visibility wrapper -> ScreenshotPngWriter
  -> DPI-aware render or screen geometry -> PNG file
```

`ScreenshotPngWriter`는 상태를 보유하지 않고 Window와 출력 경로를 한 번의
호출 입력으로 받습니다. Direct runner가 창 수명과 pump를 계속 소유하므로
기존 캡처 시점과 화면 전경 순서는 변하지 않습니다. OVL-25 owner를 새
Direct caller가 사용하게 된 것은 dependency boundary 변경이며, 두 번째
writer를 만들지 않고 기존 canonical owner를 확장한 근거입니다.

OVL-40 완료 후 이 owner를 파일 크기만으로 다시 나누거나 Direct wrapper를
새 owner로 복제하지 않습니다. 새 PNG 출력 결함, 명시적 Direct capture
계약 변경, 또는 입증된 책임/의존성 충돌이 있을 때만 재검토합니다.

### 9.31 smoke runner docking state file 책임을 읽는 순서

OVL-39는 `PipelineViewerScreenshotSmoke.Program`과 조건부 embedded
`OpenVisionLabDirectSmokeRunner`에 중복되어 있던 `LayerDocking.layers` /
`LayerDocking.layout` 파일 수명 정책을 `SmokeDockingStateFiles` concrete
owner로 모았습니다. owner는 WPF나 Shell을 참조하지 않고 두 파일의 경로
구성, byte backup/restore, absent-file cleanup, clear, evidence copy만
담당합니다. `Program`은 application data directory와 WPF callback을 계속
조합하며, Direct smoke의 기존 private wrapper 이름과 scenario call site는
호환성을 위해 유지하고 실제 파일 작업만 owner에 위임합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Pipeline smoke 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | docking-persistence target이 기존 AppPathService directory와 WPF capture callback을 owner에 전달하는 call path |
| 2. embedded direct 조합 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | 기존 backup/clear/copy wrapper와 scenario call site; wrapper에는 file-state 구현이 없음 |
| 3. persisted-state owner | `tools/PipelineViewerScreenshotSmoke/SmokeDockingStateFiles.cs` | 두 stable file name, invocation-local byte snapshot, finally restore/delete, clear, evidence copy |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeDockingStateFilesContract.cs` | 두 runner 위임, old implementation removal, conditional link, restore/delete/exception/clear/copy byte contract |
| 5. embedded composition | `src/OpenVisionLab/OpenVisionLab.csproj` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`일 때 owner source를 한 번만 compile하는 조건부 Link |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-docking-state-files-contract-debug-rerun-20260909`, `smoke-docking-state-files-contract-release-rerun-20260909`, `ovl22-regression-after-ovl39-debug-20260909`, `ovl22-regression-after-ovl39-release-20260909` | Debug/Release 9/9 state contract와 10/10 target-runner 회귀 |

호출과 상태 흐름은 다음과 같습니다.

```text
Program docking target -> AppPathService CONFIG/UI path + WPF callback
  -> SmokeDockingStateFiles.RunWithBackup<T>
  -> callback mutates persisted layout -> finally restores prior bytes or deletes newly-created file

Direct scenario -> existing wrapper -> SmokeDockingStateFiles
  -> same file policy -> existing window/evidence flow
```

owner는 callback 결과를 그대로 반환하고 callback 예외를 cleanup 후 다시
전파합니다. 기존 파일이 없었던 경우에만 callback이 만든 state file을
삭제하며, cleanup 예외는 기존 best-effort 정책대로 삼킵니다. owner에는
WPF, `OpenVisionShellHost`, `CaptureResult`, interface, factory, partial,
message bus, 또는 static mutable cache가 없습니다. 두 runner가 같은 stable
file policy를 직접 구현하지 않으므로, 이 경계는 파일 크기 기준 분할이
아닌 중복 책임 제거입니다.

OVL-39 후에는 `WithDockingStateFileBackup`이라는 이름의 Direct wrapper만
호환성 표면으로 남습니다. 이 wrapper와 OVL-38의 `SmokeFixtureResources`를
새 결함·명시적 계약 변경·입증된 책임/의존성 충돌 없이 다시 나누거나
옮기지 않습니다.

### 9.30 smoke runner fixture resource 책임을 읽는 순서

OVL-38은 `PipelineViewerScreenshotSmoke/Program.cs`에 남아 있던 합성 이미지,
템플릿, 대표 이미지 파일 생성과 best-effort 임시 파일 정리의 책임을
`SmokeFixtureResources` concrete owner로 옮겼습니다. 이 owner는 WPF,
OpenVision 제품 상태, Recipe 실행, 캡처 lifecycle을 참조하지 않으며, 모든
메서드는 호출자가 소유권을 받는 정적 resource factory입니다. `Program`은
기존 target별 실행 순서와 WPF capture 시점을 계속 조합하고, persisted
docking file policy는 OVL-39의 `SmokeDockingStateFiles`에 위임합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | 기존 target이 fixture를 만들고 Bitmap을 dispose하거나 파일 정리를 요청하는 호출 경로 |
| 2. fixture resource owner | `tools/PipelineViewerScreenshotSmoke/SmokeFixtureResources.cs` | 14개 합성 Bitmap/template/file 생성기, streaming SHA-256, 기존 `TryDeleteFile` 정리 경계 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/SmokeFixtureResourcesContract.cs` | Program 위임, old helper 제거, WPF 없는 차원/hash/template/file/cleanup 검증 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\smoke-fixture-resources-contract-20260909`, `smoke-fixture-resources-contract-release-20260909`, `ovl22-regression-after-ovl38-debug-20260909`, `ovl22-regression-after-ovl38-release-20260909` | Debug/Release 9/9 계약과 OVL-22 10/10 회귀 |

호출 경계는 다음과 같습니다.

```text
Program.Main -> selected WPF target -> SmokeFixtureResources factory
  -> caller owns returned Bitmap/path -> caller dispose/delete in existing finally
```

이동 전후 fixture 구현의 whitespace-stripped token hash는
`71962931406069898943373989cbcf87986c47d8bf2db75f64305e8b9132af04`로
동일합니다. 새 owner는 별도 interface, factory wrapper, partial, message bus를
추가하지 않았습니다. OVL-22~38 owner는 이 slice에서 다시 나누지 않았으며, 새 fixture 결함·명시적 계약 변경·입증된 책임/의존성 충돌 없이는 `SmokeFixtureResources`를 재분할하지 않습니다. OVL-39의 `SmokeDockingStateFiles`도 같은 경계 잠금을 따릅니다.

### 9.28 Shell Recipe Validation Suite View 책임을 읽는 순서

OVL-35는 Shell `PipelineRunHistory` 탭에 직접 들어 있던 Validation Suite
presentation을 `OpenVisionRecipeValidationSuiteView` concrete UserControl로
이동했습니다. View는 scope 선택, Run/Stop 버튼, evidence board, local
validation-set editor, image rows, summary의 XAML과 AutomationId를 소유하고
Shell은 같은 위치에 View를 조합합니다. `RecipeCommands` bindings와
상태·실행·저장 정책은 기존 CommandSurface에 남아 있으며 View code-behind는
`InitializeComponent`만 호출합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. Shell 조합 | `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml` | `PipelineRunHistory` 탭에서 `OpenVisionRecipeValidationSuiteView`를 배치하고 qualified snapshot을 다음 sibling으로 유지하는 지점 |
| 2. Validation Suite View | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Views/OpenVisionRecipeValidationSuiteView.xaml(.cs)` | 기존 30개 Validation AutomationId, `RecipeCommands` binding, inherited DataContext, local visibility converter, presentation-only code-behind |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/RecipeValidationSuiteViewContract.cs` | Shell old block 제거, owner AutomationId 단일성, binding/resource 경계, code-behind 최소성 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl35-validation-suite-view-contract-debug4-20260909`, `ovl35-validation-suite-view-contract-release4-20260909`, `ovl35-runtime-validation-suite-view-debug5-20260909`, `ovl35-runtime-validation-suite-view-release-20260909` | Debug/Release 4/4 계약, OVL-31~34 회귀, 단일 모니터 EXE 화면 |

이 View는 `OpenVisionRecipeBasicLifecycleView`, Shell CommandSurface, 기존
ValidationDataset configuration/progress/review/drawing evidence owner를
대체하지 않습니다. 새 UI 결함·명시적 계약 변경·입증된 책임 충돌 없이 View를
다시 나누거나 command/state policy를 이동하지 않습니다.

### 9.27 validation dataset selected-run drawing evidence 책임을 읽는 순서

OVL-34는 `Program`의 `openDrawingEvidence` 블록에 섞여 있던 persisted
selected-Run source 검증, PinArrayGap row drawing 확인, viewer selector와
floating window 조합, saved report 복사, workspace side-effect guard를
`ValidationDatasetDrawingEvidence` concrete owner로 이동했습니다. `Program`은
기존 `openDrawingEvidence` 결정과 configuration/source/artifact/pump 전달만
담당합니다. OVL-33 review-queue, OVL-32 execution-progress, OVL-30 summary
artifact, OVL-31 configuration owner는 그대로 별도 경계입니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행/조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | `openDrawingEvidence` 조건과 기존 Recipe configuration/source/artifact/UI pump 전달 |
| 2. drawing evidence owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetDrawingEvidence.cs` | 저장 source SHA 검증, PinArrayGap 두 row drawing, executed-failure 보존, selector/viewer/window, artifact와 Layer/Preview/route 불변식 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetDrawingEvidenceContract.cs` | owner 호출 경로, Program old block 제거, pure projection과 exact contract artifact |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl34-validation-dataset-drawing-evidence-contract-debug-20260909`, `ovl34-validation-dataset-drawing-evidence-contract-release-20260909`, `ovl34-runtime-drawing-evidence-debug2-20260909`, `ovl34-runtime-drawing-evidence-release2-20260909` | Debug/Release 7/7 계약, OVL-22~33 회귀, 단일 모니터 EXE target |

`ValidationDatasetDrawingEvidence`는 제품의 `OpenVisionRecipeRunEvidence`와
`OpenVisionRecipeRunEvidenceViewerController`를 대체하지 않는 validation
smoke composition owner입니다. 새 drawing 결함·명시적 계약 변경·입증된
책임 충돌 없이 owner를 다시 나누거나 OVL-27/30/31/32/33 책임을 이동하지
않습니다. 파일 크기만으로 split하지 않습니다.

### 9.23 validation dataset summary artifact 책임을 읽는 순서

OVL-30은 OVL-27에서 분리한 기존 `ValidationDatasetArtifactWriter`에 남아
있던 local validation dataset의 summary 출력 책임을 완성했습니다. owner는
`pipeline.xml`, `batch_summary.json`, `audit_summary.json`을 저장하고
expected/actual judgment 네 종류를 계산합니다. `Program`은 dataset 입력,
Recipe 실행, progress, Run History/UI 검증과 artifact 호출 순서를 유지합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행/조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | dataset 환경값, Recipe/pipeline 준비, validation-set 실행, progress와 UI review 검증 |
| 2. artifact owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetArtifactWriter.cs` | CSV/evidence와 pipeline XML, batch/audit JSON, expected/actual judgment projection |
| 3. 저장 모델 | `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineBatchRunSummaryStorage.cs` | `VisionPipelineBatchRunSummary`와 `VisionPipelineBatchSampleRunResult` JSON 필드 |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetArtifactWriterContract.cs` | writer 위임, old summary projection 제거, XML/JSON 결과와 네 judgment 판정 |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl30-validation-dataset-artifact-writer-contract-20260909-debug`, `ovl30-validation-dataset-artifact-writer-contract-20260909-release` | Debug/Release 9/9 계약 결과 |

OVL-27의 CSV/evidence owner를 다시 쪼개거나 `Program`을 파일 크기로
분할하지 않습니다. 새 artifact schema 결함, 명시적 계약 변경, 또는 입증된
책임 충돌이 있을 때만 이 경계를 다시 검토합니다.

### 9.20 validation dataset artifact/report 책임을 읽는 순서

OVL-27은 `PipelineViewerScreenshotSmoke/Program.cs`의 local validation dataset
결과 출력 책임 중 CSV와 misclassification evidence 생성 경계를 concrete owner로
옮겼습니다. `ValidationDatasetArtifactWriter`는 persisted Run Report metric을
CSV로 투영하고, report-relative drawing을 선택하며, original/drawing/run-report
artifact를 복사하고, false-accept/false-reject manifest와 evidence README를
작성합니다. dataset 설정·recipe fixture·suite 실행·progress·summary/audit JSON과
UI review queue 검사는 `Program`에 남습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 실행/조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | dataset 환경 설정, recipe/pipeline fixture, validation-suite 실행, summary/audit JSON, writer 호출 |
| 2. artifact/report owner | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetArtifactWriter.cs` | CSV metric projection, report-relative drawing 선택, artifact copy, manifest/README 출력 |
| 3. persisted report 계약 | `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineRunReportStorage.cs` | Run Report schema와 persisted image/overlay 경로 |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ValidationDatasetArtifactWriterContract.cs` | Program 위임, old helper 제거, synthetic report/CSV/evidence 출력 |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl27-validation-dataset-artifact-writer-contract-rerun-20260909`, `ovl27-validation-dataset-artifact-writer-release-contract-20260909` | Debug/Release 7/7 계약 결과 |

`ValidationDatasetArtifactWriter`는 interface, factory, wrapper chain 없이 concrete
internal owner입니다. 호출자가 result list와 artifact root를 보유하고 writer는
호출 중 output state만 보유합니다. OVL-27 owner는 새 artifact/report 결함·출력 계약
변경·입증된 책임 충돌 없이는 다시 나누지 않으며, 남은 fixture/execution/summary
코드는 파일 크기만으로 분할하지 않습니다.

### 9.19 screenshot capture lifecycle 책임을 읽는 순서

OVL-26은 `PipelineViewerScreenshotSmoke/Program.cs`에 남아 있던 Window 캡처
수명과 정리 순서를 concrete owner로 옮겼습니다. `ScreenshotCaptureLifecycle`은
임시 Window 생성·표시·활성화·floating Window 선택·검증 전후 pump·PNG 호출·진단
callback·content Dispose·Window close를 한 호출의 `try/finally` 경계에서 소유합니다.
`Program`은 shared `Pump`, OpenGL 진단 구현, visual assertion, fixture와 target
조합을 유지하고 `ScreenshotPngWriter`가 실제 PNG 렌더링을 소유합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | target catalog, verify callback, capture option, `Pump`/diagnostic callback 전달 |
| 2. lifecycle owner | `tools/PipelineViewerScreenshotSmoke/ScreenshotCaptureLifecycle.cs` | temporary/standalone Window 생성·표시·선택·정리, disposable content, `CaptureResult` |
| 3. rendering owner | `tools/PipelineViewerScreenshotSmoke/ScreenshotPngWriter.cs` | screen/element PNG render와 파일 출력 |
| 4. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ScreenshotCaptureLifecycleContract.cs` | wrapper 위임, cleanup 순서, pump/diagnostic callback, WPF output과 close 증거 |
| 5. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl26-screenshot-capture-lifecycle-contract-retry-20260909`, `ovl26-screenshot-capture-lifecycle-release-contract-20260909` | Debug/Release 10/10 계약 결과 |

`ScreenshotCaptureLifecycle`은 interface, factory, wrapper chain 없이 concrete
internal owner입니다. `Program`에 남은 wrapper는 callback을 명시적으로 조합하는
짧은 adapter이며 lifecycle state를 보유하지 않습니다. OVL-26 owner는 새 cleanup/
lifecycle 결함·capture 계약 변경·입증된 책임 충돌 없이는 다시 나누지 않으며,
남은 fixture/reporting 코드는 파일 크기만으로 분할하지 않습니다.

### 9.18 screenshot PNG writer 책임을 읽는 순서

OVL-25는 `PipelineViewerScreenshotSmoke/Program.cs`에서 PNG 출력 구현을
분리했습니다. OVL-40에서 조건부 embedded Direct runner도 같은 owner를
사용하도록 확장했습니다. `ScreenshotPngWriter`는 화면 좌표 복사, DPI-aware
WPF `Window`/`FrameworkElement` 렌더링, `RenderTargetBitmap`/PNG encoder,
단일·다중 Window 화면 합성, 출력 디렉터리 처리를 소유합니다.
`Program`은 임시 Window 수명, floating tool 선택, dispatcher pump, 검증 callback,
OpenGL 진단, fixture 생성, 캡처 시점을 계속 결정합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 호출 조합 | `tools/PipelineViewerScreenshotSmoke/Program.cs` | Window/element 생성, verify callback, 캡처 시점, lifecycle cleanup과 writer 호출 |
| 2. PNG output owner | `tools/PipelineViewerScreenshotSmoke/ScreenshotPngWriter.cs` | `WriteScreenPng`, `WriteDpiAwareWindowPng`, `WriteWindowScreenPng`, `WriteWindowsScreenPng`, `WriteElementPng`, `WriteVisibleElementPng`, WPF/screen render와 PNG 저장 |
| 3. 독립 계약 | `tools/PipelineViewerScreenshotSmoke/ScreenshotPngWriterContract.cs` | owner 연결, Program method 제거, WPF element 48x32 출력, screen-copy 경로 |
| 4. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl25-screenshot-png-writer-contract-20260909`, `ovl25-screenshot-png-writer-release-contract-20260909` | Debug/Release 7/7 계약과 PNG 결과 |

`ScreenshotPngWriter`는 별도 interface, factory, wrapper 없이 concrete internal
owner입니다. `CaptureWindowWithContent`, `CaptureStandaloneWindow`, `CaptureElement`의
window state/cleanup은 별도 lifecycle owner가 담당하며, Direct wrapper는 활성화와
pump만 조합합니다. 새 capture-format 결함, 렌더링 계약 변경, 또는 입증된
책임/의존성 충돌 없이는 이 canonical owner를 다시 나누거나 두 번째 writer를
만들지 않으며, 파일 크기만으로 `Program`을 분할하지 않습니다.

### 9.46 Image Compare MVVM와 디렉터리 정책을 읽는 순서

Image Compare의 Window code-behind는 파일 선택 대화상자와 포인터/창 수명만
담당합니다. 최근 디렉터리의 메모리·저장/복원은 ViewModel이 소유한 concrete
`ImageCompareDirectoryPolicy`가 담당하고, decoded `Bitmap`/`BitmapSource` 수명은
기존 `ImageCompareImageResource`가 담당합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. UI 진입 | `src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs` | `OpenFileDialog`, slot pointer mapping, window lifecycle만 연결하는 얇은 View 경계 |
| 2. binding/state | `src/OpenVisionLab/UI/Popup/Wpf/ViewModels/ImageCompareViewModel.cs` | `LoadImages`, slot collection, zoom/pixel status, `InitialImageDirectory` projection |
| 3. directory policy | `src/OpenVisionLab/UI/Popup/Wpf/ImageCompare/ImageCompareDirectoryPolicy.cs` | 선택 디렉터리 기억, `CONFIG/image_compare_last_directory.txt` 저장/복원, recoverable I/O 경고 |
| 4. image resource | `src/OpenVisionLab/UI/Popup/Wpf/ImageCompare/ImageCompareImageResource.cs` | Bitmap/BitmapSource 생성·동결·dispose 및 PNG/BMP metadata |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/ImageCompareDirectoryPolicyContract.cs`, `ImageCompareResourceContract.cs` | 저장/재오픈, invalid input, resource replacement/dispose 회귀 |

호출 경로는 다음과 같습니다.

```text
LoadImages_Click -> ImageCompareViewModel.LoadImages
  -> ImageCompareDirectoryPolicy.RememberImageDirectory
  -> ImageCompareSlotViewModel.Load
  -> ImageCompareImageResource.Load
```

새 파일 선택/경로 저장 책임을 Window에 다시 추가하지 않습니다. XAML binding,
Image Compare public facade, Recipe/XML, Preview/Run 동작은 이 경계에서 바뀌지
않습니다.

### 9.47 Log Panel MVVM와 외부 I/O를 읽는 순서

Log Panel View는 생성·크기 변경·unload lifecycle만 연결합니다. 필터, 요약,
타이머와 `ICommand` binding 상태는 `LogPanelViewModel`이 소유하고, 최신
`*ALL.log` 검색/읽기와 로그 폴더를 여는 shell 호출은 concrete
`LogPanelFileAccess`가 소유합니다. 화면 버퍼와 dropped-count는 기존
`RuntimeLogSink`/`RuntimeLogStream` 경계를 유지합니다.

```text
LogPanelView
  -> LogPanelViewModel (binding/filter/timer)
  -> RuntimeLogStream (display buffer)
  -> LogPanelFileAccess (latest file read / folder launch)
```

처음 읽을 파일 순서는 `View/LogPanelView.xaml.cs` →
`ViewModel/LogPanelViewModel.cs` → `Infrastructure/LogPanelFileAccess.cs` →
`OpenVisionLab.Logging/Model/RuntimeLogStream.cs` → 기존
`log_panel_contract_check`입니다. ViewModel에 `Process.Start`,
`FileStream`, `Directory.EnumerateFiles`를 다시 추가하지 않습니다.

### 9.48 Recipe run evidence View와 이미지 decode owner를 읽는 순서

Recipe run evidence 화면은 저장된 원본 이미지와 Step drawing을 읽기 전용으로
표시합니다. Evidence View는 선택 상태·상태 텍스트·ComboBox 이벤트와 두
`OpenVisionLayerViewerView`의 lifecycle을 소유하고, 파일 스트림과
`System.Drawing.Image.FromStream`은 기존 concrete
`OpenVisionBitmapImagePreviewFactory`가 소유합니다. 이 경계는 새 서비스나
인터페이스를 추가하지 않고 기존 이미지 preview owner를 재사용한 것입니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. 호출 조합 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeRunEvidenceViewerController.cs` | Evidence View 생성, owner Window, floating host와 registry 수명 |
| 2. 화면 상태 | `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeRunEvidenceViewerView.xaml.cs` | `TrySetEvidence`, drawing 선택, 상태/오류 표시, viewer Dispose; 직접 파일 I/O 없음 |
| 3. 이미지 decode owner | `src/OpenVisionLab/UI/Menu/Wpf/Viewer/OpenVisionBitmapImagePreviewFactory.cs` | `LoadBitmap(path, role)`의 FileStream/Bitmap clone과 기존 BitmapSource preview/read-size 계약 |
| 4. 이미지 수명 | `src/OpenVisionLab/UI/Menu/Wpf/Viewer/OpenVisionLayerViewerView.xaml.cs` | 입력 Bitmap clone, 이전 이미지 Dispose, canvas/fallback viewer lifecycle |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/RecipeRunEvidenceImageBoundaryContract.cs` | View direct decode 제거, valid dimensions, missing-file role error, factory call path |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m1-recipe-evidence` | phase summary, source owner check, 4/4 contract output |

호출 경로는 다음과 같습니다.

```text
OpenVisionShellHostView
  -> OpenVisionRecipeRunEvidenceViewerController.Open
  -> OpenVisionRecipeRunEvidenceViewerView.TrySetEvidence/TrySetDrawing
  -> OpenVisionBitmapImagePreviewFactory.LoadBitmap
  -> OpenVisionLayerViewerView.SetLayer (clone/Dispose owner)
```

Evidence View에 `FileStream`, `Image.FromStream`, 또는 두 번째 이미지 loader를
다시 추가하지 않습니다. WPF theme/DPI/monitor/input과 장시간 native 수명은
별도 런타임 행렬이며 소스 계약만으로 완료 처리하지 않습니다.

### 9.49 Line Tool 저장 경계를 읽는 순서

Line Tool의 PropertyGrid 변경·preset·sample 적용은 View 이벤트에서 시작할 수
있지만 Recipe 저장 정책은 View의 책임이 아닙니다. 기존
`OpenVisionNativeToolPropertySessionStore`를 새 서비스로 감싸지 않고, Line
도구를 조합하는 기존 `OpenVisionNativeCustomToolFactory`가 저장 Action을
`LineToolPresenter`에 주입합니다. Presenter가 현재 Line A/B property를
저장하고, View는 `presenter.PersistProperties()`만 호출합니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. property source | `OpenVisionNativeCustomToolFactory.CreateLine` | Repository에서 Line A/B property를 얻고 기존 persistence Action을 조합 |
| 2. presenter boundary | `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Presentation/LineToolPresenter.cs` | ViewModel facade와 injected `PersistProperties()` 호출 경계 |
| 3. 화면 adapter | `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/LineToolWpfView.xaml.cs` | PropertyGrid/preset/sample 변경을 presenter에 전달; native store 직접 참조 없음 |
| 4. 저장 owner | `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Runtime/OpenVisionNativeToolPropertySessionStore.cs` | Recipe별 key, 실패 상태, PropertySaved 이벤트, repository/cache 동기화 |
| 5. 독립 계약 | `tools/VisionRecipeRunnerSmoke/LineToolPersistenceBoundaryContract.cs` | View 직접 저장 제거, Presenter 경계, Line A/B key 보존 |
| 6. 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\p1-view-boundaries-20260913\m2-line-persistence` | source contract와 phase summary |

호출 경로는 다음과 같습니다.

```text
OpenVisionNativeCustomToolFactory.CreateLine
  -> LineToolPresenter(viewModel, persistProperties)
  -> LineToolWpfView property/preset/sample change
  -> LineToolPresenter.PersistProperties
  -> OpenVisionNativeToolPropertySessionStore.Save(Line(L)_1 / Line(R)_1)
```

View에 `OpenVisionNativeToolPropertySessionStore`나 별도의 저장 정책을 다시
추가하지 않습니다. PropertyGrid failure/recovery 표시와 실제 WPF 반복 입력은
제품 런타임 범위에서 별도로 검증해야 합니다.

### 9.50 RoiImageCanvasViewModel 경계를 읽는 순서

`RoiImageCanvasViewModel`은 5개의 소비 경로가 공유하는 ImageCanvas facade이며
`ImageCanvasControl`, 현재 Mat clone, ROI snapshot/mode 상태와 native Dispose
순서를 함께 소유합니다. 파일 길이만으로 이를 새 ViewModel·wrapper·partial로
나누면 소비자마다 서로 다른 Mat/control lifetime이 생깁니다. 대신 입력·대화상자·
context menu·경로·로드·저장 정책은 이미 concrete owner로 분리되어 있습니다.

| 순서 | 소유자 | 읽을 내용 |
| --- | --- | --- |
| 1. WPF host | `src/Libraries/OpenVisionLab.ImageCanvas/View/RoiImageCanvasView.xaml.cs` | DataContext attach/detach, dialog/context host 연결, WPF key event 전달 |
| 2. facade/state owner | `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs` | ROI/mode/snapshot 상태, current Mat clone, ImageCanvasControl 및 timer/native Dispose |
| 3. 입력 정책 | `RoiImageCanvasKeyboardInputController`, `RoiImageCanvasWpfKeyboardInputController`, `RoiImageCanvasMouseInputController` | WinForms/WPF keyboard와 mouse 이벤트의 별도 구독·해제 및 callback 전달 |
| 4. I/O/UI 경계 | `IImageCanvasDialogHost`, `RoiImageCanvasDialogHost`, `IImageCanvasContextMenuHost`, `ImageCanvasDirectoryPolicy`, `CanvasImageLoader`, `CanvasImageSaver` | modal path, context menu, remembered directory, Mat load/save policy |
| 5. 소비자 lifetime | `ImageCanvasExternalConsumerSession`, `VisionToolOpenGlPreviewCanvasAdapter`, `OpenVisionBitmapCanvasPresenter`, `OpenGlTemplateEditorWindow` | 각 owner가 ViewModel/host를 종료하는 기존 Dispose 경로 |
| 6. 독립 계약 | `tools/VisionRecipeRunnerSmoke/RoiImageCanvasBoundaryContract.cs` | concrete owner, no direct modal/encoding policy, Mat/native Dispose, known consumer release 확인 |

결정은 **추가 production split 없음**입니다. 현재 source에서 Mat/control/timer를
독립 snapshot으로 넘길 수 있는 별도 state·lifetime·test seam이 증명되지 않았고,
이미 분리된 정책 owner를 다시 감싸면 호출 경로만 길어집니다. 이 판단은 파일 크기
기준이 아니라 caller와 Dispose 계약 기준이며, 실제 WPF/OpenGL runtime 검증과는
구분합니다.

호출·수명 경로는 다음과 같습니다.

```text
RoiImageCanvasView/DataContext 또는 external consumer
  -> RoiImageCanvasViewModel command/facade
  -> input/dialog/path/image concrete owner
  -> ImageCanvasControl + current Mat
  -> consumer/host Dispose -> ViewModel.Dispose -> controller/Mat/control release
```

실제 WPF theme/DPI/monitor/input, OpenGL/GPU와 장시간 native 수명은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`입니다.

### 9.51 Partial 59개를 구조적으로 검토하는 순서

59개 선언은 파일 수를 줄이기 위해 일괄 병합하지 않았습니다. 먼저 각 선언의
caller, mutable-state writer, release owner, XAML/binding/public/test contract를
확인하고, 독립 state·lifetime·test seam이 있을 때만 기존 concrete owner로
책임을 이동합니다. 전체 row-level 결과는
`docs/reports/OPENVISIONLAB_PARTIAL_RESPONSIBILITY_REVIEW_20260913.md`와
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\p1-view-boundaries-20260913\\m4-partial-audit\\partial-structure-review.csv`에 있습니다.

이번 재검토에서 실제로 이동한 경계는 다음과 같습니다.

```text
OpenGlTemplateEditorWindow / RoiEditorWindow
  -> OpenVisionBitmapImagePreviewFactory.LoadBitmap(path, "pattern")
  -> View preview assignment and ROI/UI/resource lifetime
```

생성·디자이너·Settings partial, Shell composition root, Pipeline Review 결과
projection, Learn topic View와 presenter, ImageCanvas native host는 기존 owner가
수명과 상태를 이미 함께 소유하거나 화면 자체의 control contract가 남아 있어
유지했습니다. 따라서 `partial` 선언 수가 59라는 사실만으로 구조 개선이
완료됐다고 판단하지 않으며, 반대로 선언을 없애는 것만으로 MVVM 경계가
좋아진다고 판단하지도 않습니다. 새 결함이나 변경된 dependency/lifetime
경계가 생길 때 같은 검증 절차로 다시 엽니다.

Focused evidence: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\p1-view-boundaries-20260913\\m4-template-editor-image`.

### 9.52 Partial structural elimination schedule (PL-0028)

`PL-0028`은 59개 선언을 무조건 0개로 만드는 작업이 아니라, 수동 Partial이
독립 책임을 숨기는 경우에 기존 concrete owner로 이동하는 후속 작업입니다.
현재 계획과 단계별 완료 기준은
`docs/reports/OPENVISIONLAB_PARTIAL_STRUCTURAL_ELIMINATION_PLAN_20260913.md`에
있고, `.proofline/issues/PL-0028.json`이 현재 `next_action`과 증거를 소유합니다.

첫 slice는 `OpenVisionLearnWindow.xaml.cs`의 Color/HSV animation state와
guide/mask policy를 `ColorHsvLearnPresenter.cs`로 이동한 M2이며, Window에는
XAML event·timer·brush·control projection만 남겼습니다. 이어서
`OpenVisionRecipeBasicLifecycleView.xaml.cs`,
`OpenVisionRecipeValidationSuiteView.xaml.cs`,
`OpenVisionWorkspaceSamplePickerView.xaml.cs`가 각각 `InitializeComponent()`만
가진 수동 code-behind임을 확인하고 삭제했습니다. Shell lifecycle contract
7/7, Validation Suite contract 4/4, Sample Picker source contract 5/5, Readiness
Debug·Release contract, Solution/Smoke build와 `Invoke-RefactorAudit -Verify`
(`PartialDeclarations=56`, `PartialTextMatches=0`)가 통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m3-basic-lifecycle`,
`m4-validation-suite`, `m5-workspace-sample-picker`에 있습니다. 다음 slice는
별도의 caller/lifetime/contract proof 후에 하나만 선택합니다. 추가로
`OpenVisionShellHostWindow.xaml.cs`는 Shell View 생성, HWND hook, responsive
scale, smoke projection, 종료 Dispose를 함께 소유하는 WPF composition root로
검토되어 retained boundary로 기록했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m6-shell-host-window-retention`에 있습니다.
`OpenVisionPendingToolView.xaml.cs`도 ViewModel 주입/DataContext만 담당하는
WPF adapter로 확인되어 retained boundary로 기록했고, ViewModel과
DocumentController의 상태·Dispose owner를 유지했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m7-pending-tool-retention`에 있습니다.
`OpenVisionLayerDockWorkspaceView.xaml.cs`도 AvalonDock DependencyProperty,
event bridge, guide/geometry, visual snapshot을 소유하는 concrete visual
adapter로 확인되어 retained boundary로 기록했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m8-docking-workspace-retention`에 있습니다.
`OpenVisionLayerDockingGuideOverlayView.xaml.cs`도 docking guide zone의 XAML
namescope, active-zone visibility/brush 상태, guide count와 pane margin 투영을
소유하는 concrete visual adapter로 확인되어 retained boundary로 기록했습니다.
증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m9-docking-guide-overlay-retention`에 있습니다.
`ImageCanvasControl.cs`와 `ImageCanvasControl.designer.cs`도 SharpGL
`OpenGLControl`의 designer 생성 필드와 본체의 rendering·event·P/Invoke·native
cleanup이 하나의 lifetime 경계를 공유하는 concrete WinForms/native adapter로
확인되어 retained boundary로 기록했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m10-image-canvas-native-retention`에 있습니다.
`AddRoiArrayView.xaml.cs`와 `AddRoiArrayView.xaml`은 프로젝트에서 제외된
중복 WPF 파일이었고, compiled
`Compatibility/AddRoiArrayViewCompatibility.cs`가 동일한 공개 타입과
호출·binding·RequestClose 경계를 이미 소유하므로 제거했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m11-add-roi-array-dead-partial-removal`에 있습니다.
`AutoAlignTeachingView.xaml.cs`, 대응 XAML과 `AutoAlignTeachingViewModel.cs`는
프로젝트에서 제외되어 있고 저장소 호출자가 없는 legacy feature였으므로
stale project exclusions와 함께 제거했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m12-auto-align-dead-feature-removal`에 있습니다.
`AutoWarpageTeachingView.xaml.cs`, 대응 XAML과 `AutoWarpageTeachingViewModel.cs`도
동일하게 프로젝트에서 제외되어 있고 저장소 호출자가 없는 legacy feature였으므로
stale project exclusions와 함께 제거했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m13-auto-warpage-dead-feature-removal`에 있습니다.

`RoiImageCanvasView.xaml.cs`는 다음 Partial 경계로 검토되어 유지했습니다.
View는 XAML `WindowsFormsHost` namescope, `ShowStatusBar`/`ShowToolBar`
DependencyProperty, DataContext attach/detach, dialog/context-menu host 연결,
PreviewKeyDown/KeyUp 전달과 pending DispatcherOperation 및 view-owned host
정리를 담당합니다. `RoiImageCanvasViewModel`이 ROI/mode/snapshot 상태, current
`Mat`, `ImageCanvasControl`, refresh timer와 Dispose를 소유하므로 View는
ViewModel을 대신 해제하지 않습니다. Shell/Layer Viewer XAML과 preview,
bitmap presenter, template editor가 기존 호출 경로를 유지합니다. 이 경계를
별도 wrapper나 manual Partial로 옮기면 같은 WPF/native namescope를 감추게 되며
독립 상태·수명 owner가 생기지 않습니다.

M14 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m14-roi-image-canvas-view-retention`에 있습니다. Source contract 10/10,
기존 ROI boundary contract 6/6, ImageCanvas Debug/Release와 Smoke Debug
build 0 warning/error, RefactorAudit `PartialDeclarations=53`,
DocumentationIndex, diff check가 통과했습니다.

`LogPanelView.xaml.cs`는 다음 Partial 경계로 검토되어 유지했습니다.
View는 `LogPanelViewModel`을 생성·바인딩하고, WPF 크기 변경에 따른
compact layout과 `LogList.ScrollIntoView`만 투영하며, `Unloaded`에서 이벤트와
ViewModel 수명을 정리합니다. 필터·요약·명령·DispatcherTimer와 언어 구독은
`LogPanelViewModel`, 최신 로그 파일/폴더 I/O는 `LogPanelFileAccess`, 화면
버퍼는 `RuntimeLogStream`이 소유합니다. 따라서 View의 visual event adapter를
새 wrapper나 manual Partial로 이동할 독립 상태·수명 owner가 없습니다.

M15 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m15-log-panel-view-retention`에 있습니다. Source contract 10/10,
Logging.Controls Debug/Release build 0 warning/error, PipelineViewerScreenshotSmoke
Debug build 0 error(기존 `Program.cs:10181` CS8600 1건), RefactorAudit,
DocumentationIndex, diff check가 통과했습니다.

`src/OpenVisionLab/Properties/Settings.Designer.cs`는 generated Settings
Partial로 유지했습니다. `SettingsSingleFileGenerator`와
`System.Configuration.ApplicationSettingsBase`가 `Settings.settings`의
70개 UserScopedSetting/DefaultSettingValue property, synchronized `Default`
singleton과 serialization 계약을 생성합니다. 현재 source caller는 없지만
수동 wrapper나 삭제로 바꾸면 설정 property/default 계약과 사용자 설정
마이그레이션 경계가 바뀌므로 별도 settings migration 없이 제거하지 않습니다.

M16 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m16-settings-designer-retention`에 있습니다. Source contract 10/10,
OpenVisionLab Debug/Release build 0 warning/error, RefactorAudit,
DocumentationIndex, diff check가 통과했습니다.

`OpenVisionTcpIntegrationWindow.xaml.cs`는 다음 Partial 경계로 검토되어
유지했습니다. Window는 XAML namescope, PasswordBox의 shared-key 전달,
listening 중 close guard, Closing/Closed/PasswordChanged 해제와
`OnWindowClosed` 통지만 담당합니다. TCP exchange, settings JSON, validation,
command state, cancellation, dispatcher callback과 async Dispose는 기존
`OpenVisionTcpIntegrationController`가 소유합니다. Shell이 같은 controller로
창을 다시 열 수 있으므로 Window가 controller를 Dispose하지 않는 것도 기존
수명 계약입니다.

M17 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m17-tcp-integration-window-retention`에 있습니다. Source contract 10/10,
TCP controller disposal contract PASS, OpenVisionLab/VisionRecipeRunnerSmoke
Debug/Release 관련 build 0 warning/error, RefactorAudit, DocumentationIndex,
diff check가 통과했습니다.

`VisionToolNImageVerificationWindow.xaml.cs`는 다음 Partial 경계로 검토되어
유지했습니다. Window는 XAML namescope, source/drawing image surface의
`OpenVisionZoomableImageController`, 선택 이미지 변경 시 visual reset과
`Closed` 이벤트 해제를 소유합니다. 이미지 목록·파일 dialog·검증 실행·취소·
결과·HTML export·locator 승격과 내부 cancellation/language/image 리소스는
기존 `VisionToolNImageVerificationController`가 소유합니다. PL-0031에서는
controller의 중복 path-backed `BitmapImage` decode만 기존
`OpenVisionBitmapImagePreviewFactory.TryCreateFromPath`로 이동했으며,
선택 상태·binding 값·검증 결과 projection의 owner는 변경하지 않았습니다.
`OpenVisionNativeToolDocument`가 controller/window를 생성하고 owner를 설정한
뒤 `ShowDialog()`를 호출하므로 Window가 controller를 소유하는 현재 수명
계약도 명확합니다. View에는 파일 I/O나 검사 policy가 없고, 새 concrete
owner를 만들지 않고도 현재 호출·binding·test 경계를 유지할 수 있어
mechanical merge/extraction을 하지 않았습니다.

M18 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m18-n-image-verification-window-retention`에 있습니다. Source contract 10/10,
N-image Window 및 entry side-effect WPF smoke가 통과했고, OpenVisionLab
Debug/Release와 PipelineViewerScreenshotSmoke Debug build가 0 warning/error,
RefactorAudit, DocumentationIndex, diff check가 통과했습니다. Full theme/DPI/
monitor matrix, hardware/SDK, external deployment, long-duration native run은
검증하지 않았습니다.

`OpenVisionShellHostView.xaml.cs`는 다음 Partial 경계로 재검토되어
유지했습니다. `OpenVisionShellHostWindow`가 View를 생성하고 `OnClosed`에서
Dispose하므로 Window가 visual lifetime owner입니다. View는 XAML namescope와
Shell DependencyProperties, command surface binding, UI navigation/event
projection, tracked event release, test compatibility surface를 소유하고,
기존 concrete session/recipe/layer/tool/workspace/preview/presenter owner를
의존성 순서대로 연결합니다. 파일 내부에는 직접 파일 I/O, Pipeline 실행·저장,
OpenCvSharp, network 또는 검사 algorithm policy가 없으며, 새 wrapper/manager로
나눌 독립 state/lifetime/test 경계가 입증되지 않았습니다.

M19 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m19-shell-host-view-retention`에 있습니다. Source contract 12/12,
OpenVisionReadinessCheck, `wpf_shell_host_window_chrome`,
`wpf_shell_host_workspace`, `wpf_shell_host_workspace_image_load` smoke가
통과했고, OpenVisionLab Debug/Release와 PipelineViewerScreenshotSmoke/
Readiness Debug build가 0 warning/error였습니다. Full WPF theme/DPI/monitor
matrix, hardware/SDK, external deployment, long-duration native shutdown은
검증하지 않았습니다.

M20에서는 `OpenVisionRecipePendingEditDialog.xaml.cs`의 필수 XAML Window
partial을 유지하면서, 파일에 함께 있던 독립적인 비-WPF
`OpenVisionRecipePendingEditDialogViewModel`을
`OpenVisionRecipePendingEditDialogViewModel.cs`로 추출했습니다. Window는
namescope, modal result, Escape/button event, DataContext wiring만 소유하고,
`RecipeDialogAdapter`는 Window owner/`ShowDialog`/result를,
`OpenVisionRecipePendingEditTransitionController`는 apply/discard/cancel
정책을 소유합니다. Source contract 10/10, `wpf_recipe_pending_edit_dialog`
smoke, OpenVisionLab Debug/Release 및 PipelineViewerScreenshotSmoke Debug
build가 0 warning/error로 통과했습니다. PartialDeclarations는 53개로
유지됩니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m20-pending-edit-dialog-viewmodel-extraction`에 있습니다.

M21에서는 `OpenVisionRecipeRunEvidenceViewerView.xaml.cs`를 다시 확인했습니다.
파일 기반 Bitmap 디코드는 기존 `OpenVisionBitmapImagePreviewFactory.LoadBitmap`이
소유하며, View에는 evidence 선택, 상태/오류 투영, 두
`OpenVisionLayerViewerView`의 visual lifetime만 남아 있습니다. LayerViewer는
입력 Bitmap을 clone하고 이전 소유 이미지를 해제하며,
`OpenVisionLayerViewerWindowRegistry`는 floating window 종료 시 hosted View를
Dispose합니다. Source contract 10/10과
`RecipeRunEvidenceImageBoundaryContract` 4/4가 통과했습니다. Dataset root가
설정되지 않아 drawing-evidence WPF smoke는 실행 전 NG였으므로 해당 UI 경로는
미검증으로 기록했습니다. 새 ViewModel·wrapper는 추가하지 않았고 required
XAML partial을 retained boundary로 유지했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m21-evidence-viewer-retention`에 있습니다.

M22에서는 `OpenVisionLayerViewerView.xaml.cs`를 재확인했습니다. 이 View는
docked/floating layer, tool preview, run-evidence 화면이 공유하는 concrete
visual/native adapter입니다. 입력 Bitmap을 clone해 `ownedLayerImage`로
소유하고 이전 이미지를 해제하며, `OpenVisionBitmapCanvasPresenter`, fallback
zoom, `RoiImageCanvasView`, compact chrome, 언어/Loaded 이벤트와 pending
refresh를 같은 visual lifetime에서 정리합니다. Canvas presenter는 View 소유
Bitmap을 OpenGL 업로드·저장에 사용하고 Dispose 시 참조를 끊습니다. Dock
workspace controller와 floating window registry는 문서/창 종료 시 hosted
View를 Dispose합니다. Source contract 12/12, `wpf_shell_host_layer_popout`
smoke, OpenVisionLab/PipelineViewerScreenshotSmoke Debug/Release 빌드가
0 warning/error로 통과했습니다. 새 ViewModel·wrapper는 추가하지 않고
required XAML partial을 retained boundary로 유지했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m22-layer-viewer-retention`에 있습니다.

M23에서는 `OpenVisionShellPreviewView.xaml.cs`를 재확인했습니다. 이 파일은
standalone `wpf_shell_preview` surface의 XAML/DataContext adapter로,
`OpenVisionShellPreviewViewModel.CreatePreview()` 호출과 `Unloaded` 시
DataContext Dispose만 담당합니다. navigation/readiness/selected tool·layer/
localization/command 상태와 language subscription은 기존 concrete ViewModel이
소유하며 production Shell도 같은 ViewModel을 직접 사용합니다. View에는
image/native/file/inspection 정책이 없습니다. Source contract 10/10,
`wpf_shell_preview` smoke, OpenVisionLab Debug/Release 및
PipelineViewerScreenshotSmoke Debug build가 0 warning/error로 통과했습니다.
새 owner·wrapper는 추가하지 않고 required XAML partial을 retained boundary로
유지했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m23-shell-preview-retention`에 있습니다.

M24에서는 `OpenVisionPipelineReviewView.xaml.cs`를 재확인했습니다. Document가
View와 ExecutionController를 소유하고, ViewModel은 review/readiness/preview
projection, LayoutController는 compact/details/step-flow layout,
ExecutionController는 run identity/cancellation/revision callback/cached output,
ImageResourceOwner는 View-side Bitmap/diagnostic resource를 소유합니다. View에
남은 selection/highlight와 control event는 XAML namescope 투영 책임입니다.
Source contract 13/13, `wpf_shell_host_pipeline_review` smoke, OpenVisionLab
Debug/Release 및 PipelineViewerScreenshotSmoke Debug build가 0 warning/error로
통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m24-pipeline-review-retention`에 있습니다.

M25에서는 `OpenVisionFloatingToolWindow.xaml.cs`를 재확인했습니다. Window는
XAML namescope, title/icon/content host, WinForms/OpenGL airspace activation,
DockRequested bridge와 local event cleanup만 소유합니다. 기존
`OpenVisionFloatingToolWindowHost`가 create/reuse/placement/close/dock routing을,
`OpenVisionLayerViewerWindowRegistry`와 feature controller가 hosted content
Dispose를 소유하므로 새 wrapper나 partial은 추가하지 않았습니다. Source
contract 14/14, `wpf_tool_window_dock_float_cycle` smoke, OpenVisionLab
Debug/Release 및 PipelineViewerScreenshotSmoke Debug build가 0 warning/error로
통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m25-floating-tool-window-retention`에 있습니다.

M26에서는 `OpenVisionStartupLoadingWindow.xaml.cs`를 재확인했습니다. Window는
XAML namescope의 title/detail localization projection, `ShowReady`의 render
pump, `Complete` close gate와 `OnClosing` 조기 종료 방어만 담당합니다.
`OpenVisionLabApplication.Run`이 생성·표시·Shell 준비 완료 후 완료·종료를,
`OpenVisionLabDirectSmokeRunner`가 `startup-loading-feedback` 호출과
한국어/영어·조기 닫힘 검증을 소유합니다. Window에는 제품 업무 상태,
Recipe/Pipeline/검사 정책, 파일 또는 native resource 수명이 없으므로 별도
concrete owner로 추출하지 않고 required XAML partial을 유지했습니다.
Source contract 12/12, startup-loading smoke(신선한 한국어/영어 screenshot과
monitor intersection), embedded Debug/Release 및 PipelineViewerScreenshotSmoke
Debug build가 완료되었습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m26-startup-loading-window-retention`에 있습니다.

M27에서는 `OpenVisionWindowTitleBar.xaml.cs`를 재확인했습니다. 공유
UserControl은 XAML namescope, title/icon·Dock visibility, localized
tooltip/AutomationProperties, Window drag/minimize/maximize/close framework
동작과 `DockRequested` bridge만 소유합니다. Shell Host, Floating Tool Window,
Sample Picker Window가 제목·아이콘과 부모 Window 수명을 주입·소유하며,
TitleBar에는 Recipe/Pipeline/검사/파일/native/DataContext 정책이 없습니다.
따라서 MVVM View의 presentation/framework plumbing 경계를 유지하고 새
owner·wrapper·partial을 추가하지 않았습니다. Source contract 14/14,
`wpf_shell_host_window_chrome` 및 `wpf_shell_host_window_maximized` smoke,
OpenVisionLab Debug/Release와 PipelineViewerScreenshotSmoke Debug build가
통과했습니다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m27-window-title-bar-retention`에 있습니다.

M28에서는 `OpenVisionWorkspaceSamplePickerWindow.xaml.cs`를 재확인했습니다.
Window는 기존 `OpenVisionWorkspaceSamplePickerViewModel`을 DataContext로
주입하고 owner/modal `ShowDialog()` 결과, 제목 표시, Cancel/`DialogResult`,
HWND work-area hook과 이벤트 해제를 담당합니다. 목록·필터·선택 가능 여부와
Learn 문서 정책은 ViewModel과 기존
`OpenVisionWorkspaceLearnDocumentService`가 소유합니다. 직접 ViewModel을
호출하던 Learn+Sample click handler는 `OpenLearnAndSelectCommand`로 이동했고,
`SelectionAccepted` 결과 계약을 Window가 `DialogResult`로 변환합니다. 새
Manager/Service/wrapper는 추가하지 않았습니다. 함께 삭제했던
`OpenVisionWorkspaceSamplePickerView.xaml.cs`는 생성자에서
`InitializeComponent()`를 호출하지 않으면 실제 UserControl 본문이 비는 결함이
smoke에서 재현되어 복원했습니다. 이는 필수 XAML 초기화 Partial입니다.
Source contract 14/14와
`wpf_shell_host_workspace_sample_picker`,
`wpf_shell_host_workspace_sample_picker_maximized` smoke가 fresh screenshot,
automation ID, work-area 검사를 포함해 통과했고 OpenVisionLab Debug 및
PipelineViewerScreenshotSmoke Debug build도 완료했습니다. 생성 산출물 정리 후
OpenVisionLab Release build도 0 warning/error로 완료했습니다.
원래 증거 경로는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m28-workspace-sample-picker-window-retention`였으며, 아래 정리로 raw 파일은 삭제했습니다.

2026-09-13 저장 공간 정리로 위 raw 검증 트리와 현재 Dev의 생성
`bin`/`obj`/`.vs`/`build`/`dist`/`tmp`/`temp`,
`D:\OpenVisionLab_Data\Dev\artifacts`를 삭제했습니다. 소스·문서·샘플·사용자
데이터는 보존했으며, 결과 요약만 이 문서와 원장에 남아 있습니다.

다음 독립 경계는
`src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs`이며, 동일한
caller·mutable-state·lifetime·binding/test contract 확인 없이는 다시 분리하지
않습니다.

Heartbeat 한 회차는 하나의 독립 경계만 처리합니다. 이동 후에는 기존 Partial의
호출·상태·수명 책임이 실제로 제거됐는지 확인하고, generated/XAML/designer/native
또는 test contract 때문에 남기는 선언은 current owner와 잔류 이유를 기록합니다.

### 9.53 Image Compare Partial 후속 경계 (PL-0032)

사용자 요청에 따른 후속 cycle에서 `ImageCompareWindow.xaml.cs`의 caller,
mutable-state writer, lifetime/release owner, binding/public contract를 다시
확인했다. Window는 `OpenFileDialog`, XAML namescope, pointer/window chrome,
`LoadImages` facade와 ViewModel Dispose를 소유하는 required XAML Partial이므로
선언을 제거하지 않았다.

다만 `SlotImage_MouseMove`에 있던 표시 좌표 → 이미지 픽셀 좌표 계산은 WPF
객체가 없어도 실행되는 순수 정책이었다. 이를 기존
`ImageCompareViewModel.TryMapDisplayedPoint`로 이동하고 Window는
`ActualWidth`/`ActualHeight`와 포인터 snapshot만 전달하도록 변경했다.
status writer는 기존 `ImageCompareViewModel.UpdatePixelStatus`, Bitmap/BitmapSource
resource 수명은 기존 `ImageCompareImageResource`와
`ImageCompareSlotViewModel`이 계속 소유한다. 새 service/interface/wrapper/
partial은 추가하지 않았다.

호출 경로는 다음과 같다.

```text
ImageCompareWindow.SlotImage_MouseMove
  -> ImageCompareViewModel.TryMapDisplayedPoint
  -> ImageCompareViewModel.UpdatePixelStatus
  -> ImageCompareSlotViewModel.Bitmap.GetPixel / status binding
```

`ImageComparePointMappingContract`가 center/letterbox/edge clamp/invalid
dimensions 4/4를 Debug·Release에서 통과했고 Smoke build도 각 구성 0 warning/error였다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_IMAGE_COMPARE_POINT_MAPPING_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0032.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-point-mapping-20260914`
에 있다. 전체 WPF theme/DPI/monitor/input, hardware/GPU/native 장시간 경로는
여전히 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.54 ROI Image Canvas path-policy 후속 경계 (PL-0033)

`RoiImageCanvasViewModel`의 WPF/native/path coupling을 다시 확인했다. 이
ViewModel은 `ImageCanvasControl`, ROI 상태, input controller callback, timer,
Mat와 `Dispose` 수명, 그리고 여러 외부 consumer가 사용하는
`ImageViewer`/`LoadImage`/`SaveCurrentImage` facade를 함께 소유한다. 독립적인
state/lifetime/test seam이 확인되지 않았으므로 파일 길이만으로 분리하거나
새 Partial을 만들지 않았다.

대신 순수한 파일명 정책은 이미 존재하는
`ImageCanvasDirectoryPolicy`로 이동했다. `ResolveImageName`은 두
`LoadImage` 경로에서 `_currentImageName` 값을 만들고,
`CreateDefaultSaveFileName`은 `OnSaveIamge`에서 기존
`IImageCanvasDialogHost`로 전달할 PNG 기본 이름을 만든다. ViewModel은 더
이상 `System.IO` path API를 import/call하지 않으며, Mat/image viewer/input/
timer state와 binding/public contract는 그대로 유지된다.

호출 경로는 다음과 같다.

```text
consumer -> RoiImageCanvasViewModel.LoadImage(...)
         -> ImageCanvasDirectoryPolicy.ResolveImageName
         -> RoiImageCanvasViewModel._currentImageName

OnSaveIamge -> ImageCanvasDirectoryPolicy.CreateDefaultSaveFileName
            -> IImageCanvasDialogHost.ShowSaveImageDialog
            -> RoiImageCanvasViewModel.SaveCurrentImage
            -> CanvasImageSaver
```

`RoiImageCanvasPathPolicyContract`는 source ownership, null/whitespace default,
path stem, invalid-character replacement을 6/6으로 Debug·Release에서
통과했고 기존 `RoiImageCanvasBoundaryContract`도 6/6으로 통과했다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_ROI_IMAGE_CANVAS_PATH_POLICY_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0033.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\roi-image-canvas-path-policy-20260914`
에 있다. 전체 WPF visual/theme/DPI/monitor/input, OpenGL, hardware/native
runtime은 여전히 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.55 Learn Window 문서 action 경계 (PL-0034)

`OpenVisionLearnWindow.xaml.cs`는 required XAML Window Partial이므로
namescope, topic list와 control projection, DispatcherTimer/animation, 기존
threshold/practice/tool callback 및 Window lifetime을 계속 소유한다. 다만
`OpenLearnDocsButton_Click`와 `OpenFoundationDocsButton_Click`에서 concrete
`OpenVisionWorkspaceLearnDocumentService`를 직접 호출하던 coupling은
presentation adapter의 범위를 넘었다.

Window는 이제 `SetOpenLearnDocumentAction(Action<string>)`으로 filename을
전달하고, Shell/Tool/Threshold Learn composition controller가 기존
`OpenVisionWorkspaceLearnDocumentService.OpenLearnDocumentFile`를 연결한다.
따라서 document path/HTML cache/process launch는 기존 service owner에 남고,
Window의 XAML/public/automation 계약과 topic catalog mapping은 바뀌지 않는다.

호출 경로는 다음과 같다.

```text
OpenVisionShellHostLearnWindowController
  -> OpenVisionLearnWindow.SetOpenLearnDocumentAction(existing service)
  -> OpenVisionLearnWindow.OpenLearnDocsButton_Click
  -> callback(filename)
  -> OpenVisionWorkspaceLearnDocumentService
```

`LearnWindowDocumentBoundaryContract`가 source ownership과 세 production
controller wiring을 6/6으로 Debug·Release에서 통과했고,
`wpf_openvision_learn_foundation_contract`와 `wpf_shell_host_learn_entry`
WPF smoke도 단일 모니터에서 통과했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_LEARN_WINDOW_DOCUMENT_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0034.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\learn-window-document-boundary-20260914`
에 있다. 외부 browser launch, 전체 Learn topic/theme/DPI/input 및
hardware/native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.56 Signal Inspector TSV export 경계 (PL-0035)

`VisionToolSignalInspectorView.xaml.cs`는 required XAML Partial로서
`evidence`, plot/marker event, localization, provenance/legend projection과
`SaveFileDialog` presentation을 계속 소유한다. 다만 View가
`VisionToolSignalEvidenceExporter.ExportTsv`를 직접 호출하던 file-I/O coupling은
View presentation 범위를 넘으므로 제거했다.

Inspector는 이제 `SetExportAction(Action<VisionToolSignalEvidence, string>)`으로
선택된 경로와 현재 evidence를 전달한다. `ThresholdToolWpfView`,
`SimplePreprocessToolWpfView`, `LineToolWpfView`가 기존 exporter를 XAML child
생성 직후 연결하고, `ExportForTest`도 같은 action path를 사용한다. TSV
metadata/path/UTF-8 파일 출력은 기존 `VisionToolSignalEvidenceExporter`가
계속 소유한다.

호출 경로는 다음과 같다.

```text
Tool View XAML child
  -> SetExportAction(existing VisionToolSignalEvidenceExporter.ExportTsv)
  -> ExportButton_Click / ExportForTest
  -> SaveFileDialog (interactive path only)
  -> ExportEvidence
  -> existing VisionToolSignalEvidenceExporter
```

`SignalInspectorExportBoundaryContract`가 seam, SaveFileDialog presentation,
old direct coupling 제거, existing exporter owner, 세 production Tool wiring을
6/6으로 Debug·Release에서 통과했다. Threshold/Line Tool composition WPF
smoke와 source/build/readiness/refactor/documentation/diff checks도 통과했다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_SIGNAL_INSPECTOR_EXPORT_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0035.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\signal-inspector-export-boundary-20260914`
에 있다. 실제 native SaveFileDialog click/file association, 전체 theme/DPI/input
및 hardware/native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.57 Morphology Tool Partial retention (PL-0036)

`MorphologyToolWpfView.xaml.cs`는 required XAML View Partial이면서 existing
concrete collaborators를 조합하는 얇은 adapter다. View는 namescope,
`InitializeComponent`, `AttachToolController`, localization/presentation
presenter, `CreateProperty` facade와 cleanup ordering을 소유한다. Operation/
shape event와 selected visual state는 `VisionToolMorphologyInteractionController`,
kernel input은 `VisionToolKernelSizeController`, mutable parameter와 settings
persistence는 `MorphologyToolViewModel`, property facade는
`MorphologyToolPresenter`, base preview/lifetime는
`VisionToolSingleInputCustomToolViewBase`가 소유한다.

호출 경로는 다음과 같다.

```text
OpenVisionNativeCustomToolFactory.CreateMorphology
  -> VisionToolCompositionService.CreateMorphologyToolViewModel
  -> MorphologyToolPresenter
  -> MorphologyToolWpfView
  -> existing interaction/kernel/parameter/preview/base lifetime owners
```

View Partial에는 `OpenVisionNativeToolSettingsStore`, `SaveFileDialog`,
`System.IO`, 알고리즘 인스턴스 생성이 없고, `DisposeToolResources`가 binder/
interaction/kernel/preview 수명을 명시적으로 해제한다. 따라서 현재는 독립
state/lifetime/test seam이 없으며, 파일 길이만으로 추출하면 새 wrapper나
중복 상태 경계가 된다. `MorphologyToolPartialBoundaryContract`가 이 owner
map과 no-change 근거를 6/6으로 Debug·Release에서 통과했고,
`wpf_filter_morphology_layout_guard`와 `manual_morphology_tool_ui` WPF smoke도
통과했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_MORPHOLOGY_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0036.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\morphology-tool-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.58 Edge Based Matching / Auto MPoint Partial retention (PL-0037)

`EdgeBasedMatchingToolWpfView.xaml.cs`와 `AutoMPointTeachingPanel.xaml.cs`는
required XAML Partials이면서 각각 composition adapter와 presentation/
localization View 역할만 가진다. Edge View는 existing matching controller,
verification guide, Auto MPoint panel을 조합하고 guide/panel visibility와
cleanup ordering만 소유한다. Panel은 named controls, automation IDs,
localization controller와 그 수명만 소유한다.

Auto MPoint의 source/representative/candidate/analysis/applied-template mutable
state와 button/list event workflow는 `AutoMPointTeachingController`가 소유한다.
`AutoMPointHtmlReportExporter`가 HTML file-I/O를, existing matching controller가
property/preview facade를, factory/composition이 creation을, 그리고
`VisionToolSingleInputPropertyToolViewBase`가 base release를 소유한다.

호출 경로는 다음과 같다.

```text
OpenVisionNativePropertyGridToolFactory.CreateEdgeBasedMatching
  -> VisionToolCompositionService.CreateEdgeBasedMatchingToolViewModel
  -> EdgeBasedMatchingToolWpfView
  -> AutoMPointTeachingPanel + AutoMPointTeachingController
  -> AutoMPointHtmlReportExporter / matching controller / base lifetime
```

Edge View Partial에는 `SaveFileDialog`, `OpenFileDialog`, `System.IO`,
`AutoMPointTool` 생성, report exporter 직접 호출, template image 저장 호출이
없고, Panel Partial에도 algorithm/candidate/file/persistence state가 없다.
따라서 현재는 독립 state/lifetime/test seam이 없으며, 파일 길이만으로 새
ViewModel, service, wrapper, Partial을 추가하면 중복 상태나 owner 은닉이 된다.
`EdgeBasedMatchingPartialBoundaryContract`가 이 owner map과 binding/test/
creation/release 근거를 8/8로 Debug·Release에서 통과했고,
`wpf_shell_host_edge_based_matching_tool` 및
`wpf_shell_host_edge_based_matching_auto_mpoint` WPF smoke도 통과했다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_EDGE_BASED_MATCHING_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0037.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native file-dialog click, camera/SDK/GPU 및
long-running native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.59 Feature Matching Tool View Partial retention (PL-0038)

`FeatureMatchingToolWpfView.xaml.cs`와 XAML은 required XAML Partial이면서
factory가 주입한 presenter와 existing matching controller를 조합하는 얇은
View adapter다. View는 namescope, `InitializeComponent`, shared tool shell,
Learn Feature/topic 10, template status와 property/preview/result test facade를
소유한다. Mutable `FeatureMatchingProperty`와 template defaults/normalization은
`FeatureMatchingToolViewModel`이, PropertyGrid/template callback/delayed
preview/result-review/preset state는
`VisionToolMatchingPropertyRuntime<TProperty>`와
`VisionToolSingleInputMatchingToolRuntime<TProperty>`가, event/language
lifetime은 `VisionToolSingleInputMatchingToolController<TProperty>`가 소유한다.
Factory/composition이 creation과 persistence callback을 조합하고
`VisionToolSingleInputPropertyToolViewBase`가 마지막 controller release를
담당한다.

호출 경로는 다음과 같다.

```text
OpenVisionNativePropertyGridToolFactory.CreateFeatureMatching
  -> VisionToolCompositionService.CreateFeatureMatchingToolViewModel
  -> FeatureMatchingToolViewModel / VisionToolPropertyGridPresenter
  -> FeatureMatchingToolWpfView
  -> VisionToolSingleInputMatchingToolController<FeatureMatchingProperty>
  -> VisionToolSingleInputMatchingToolRuntime
  -> VisionToolMatchingPropertyRuntime / base View lifetime
```

View Partial에는 `System.IO`, file dialog, settings-store, algorithm
construction, template-image reload direct call이 없고, ViewModel과 shared
runtime이 각각 mutable policy와 WPF interaction state를 명확히 소유한다.
따라서 독립 state/lifetime/test seam 없이 새 ViewModel, service, wrapper 또는
Partial을 추가하면 owner를 숨기거나 상태를 복제하게 된다. 이 결론과
binding/public/test 계약은
`FeatureMatchingPartialBoundaryContract` 9/9 Debug·Release와
`docs/reports/OPENVISIONLAB_FEATURE_MATCHING_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0038.json`에 기록되어 있다. WPF smoke는
`wpf_shell_host_feature_matching_tool`로 확인했으며, 전체 theme/DPI/input,
native dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.60 Matching Tool View Partial boundary (PL-0039)

`MatchingToolWpfView.xaml.cs`는 required XAML Partial이면서 factory가 주입한
presenter와 existing matching controller를 조합하는 View adapter다. 이번
재검토에서 View 안에 있던 sample template-path resolution과 common
OpenCV/Matching property projection callback을 확인했다. 이 정책은 이미
pipeline step의 Matching property 생성/저장을 소유한
`VisionPipelineMatchingPropertyAdapter`가 맡아야 하므로, 새 service/interface/
wrapper 없이 `ResolveSampleTemplatePath`와 `ApplySampleProperty`를 기존
adapter에 추가했다. View에는 null guard, template-path 설정 호출,
property-copy callback 연결만 남겼다.

호출 경로는 다음과 같다.

```text
OpenVisionNativePropertyGridToolFactory.CreateMatching
  -> VisionToolCompositionService.CreateMatchingToolViewModel
  -> MatchingToolWpfView
  -> OpenVisionNativeToolDocument.ApplySampleStepParameters
  -> MatchingToolWpfView.ApplySampleProperty
  -> VisionPipelineMatchingPropertyAdapter.ResolveSampleTemplatePath
  -> VisionToolSingleInputMatchingToolController.SetTemplatePathForTest
  -> VisionPipelineMatchingPropertyAdapter.ApplySampleProperty
  -> MatchingProperty target / matching runtime
```

Mutable `MatchingProperty` state/defaults/normalization은
`MatchingToolViewModel`이, PropertyGrid/preview/result-review/preset과 delayed
preview는 shared matching runtime이, event/language lifetime은 matching
controller가, creation은 factory/composition이, 마지막 release는
`VisionToolSingleInputPropertyToolViewBase`가 계속 소유한다. `AUTO_PREVIEW=false`,
모든 common/Matching assignment와 `CvROIS`/`CvMASKS` 방어적 list copy는
adapter에 남아 이전 동작과 aliasing 경계를 보존한다.

`MatchingToolPartialBoundaryContract` 11/11 Debug·Release, `VisionRecipeRunnerSmoke`
Debug·Release build, `wpf_shell_host_matching_tool` WPF smoke와 fresh screenshot,
`wpf_shell_host_recipe_fixture_properties` sample-step WPF smoke와 fresh
screenshot, 동적 monitor/window probe가 통과했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_MATCHING_TOOL_PARTIAL_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0039.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-tool-partial-boundary-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.61 Threshold Tool suggestion workflow Partial boundary (PL-0040)

`ThresholdToolWpfView.xaml.cs`는 required XAML Partial로서 namescope, signal
inspector/버튼/status 표시, existing parameter interaction 및 Learn composition을
유지한다. 이전에는 View가 Threshold teaching suggestion의 Analyze/Use/Undo,
evidence availability, advisory marker/status projection을 한 파일에서 직접
조정했다. 이 multi-step workflow는 독립적으로 호출·검증할 수 있으므로
`ThresholdToolSuggestionController`가 소유하도록 옮겼다. 이 controller는
WPF control을 참조하지 않고 명시적인 callback으로 panel/button/status/marker를
투영한다.

실제 owner/call path는 다음과 같다.

```text
OpenVisionNativePropertyGridToolFactory.CreateThreshold
  -> VisionToolCompositionService.CreateThresholdToolViewModel
  -> ThresholdToolWpfView
  -> ThresholdToolSuggestionController.UpdateAvailability/Analyze
  -> VisionToolThresholdSuggestionSession
  -> VisionToolThresholdSuggestionAnalyzer
  -> ThresholdToolSuggestionController.Use/Undo
  -> VisionToolThresholdInteractionController.ApplySignalMarkerValue
  -> existing parameter-change / debounced Preview path
```

`VisionToolThresholdSuggestionSession`은 suggestion, stale-evidence, Applied/
Previous snapshot과 Undo mutable state의 writer이며, `VisionToolThresholdInteractionController`
는 threshold parameter와 Preview scheduling writer다. `ThresholdToolViewModel`/
`ThresholdToolPresenter`는 binding, normalization, summary, settings persistence를
계속 소유하고, factory/composition/base는 creation/release를 소유한다.
XAML의 `ThresholdSuggestionPanel`, `ThresholdSuggestionAnalyzeButton`,
`ThresholdSuggestionUseButton`, `ThresholdSuggestionUndoButton` automation ID와
기존 public/test facade는 유지했다. 따라서 View Partial에는 policy/analyzer/
Undo state가 남지 않으며, 새 ViewModel, interface, forwarding Partial을 추가할
근거는 없다.

`ThresholdSuggestionSessionContract`가 이 owner map과 View-to-controller/
controller-to-session 경계를 7/7로 Debug·Release에서 통과했고,
`wpf_shell_host_threshold_tool` 및 `cvr07_threshold_suggestion` WPF smoke와
동적 monitor/window probe도 통과했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_THRESHOLD_TOOL_PARTIAL_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0040.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-suggestion-controller-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.62 Line Tool sample property Partial boundary (PL-0041)

`LineToolWpfView.xaml.cs`는 required XAML Partial로서 PropertyGrid, purpose/
ROI controls, signal/review presentation, existing interaction/preview
controllers와 sample 적용 후속 순서를 유지한다. 이전에는
`ApplySampleLinePair` 내부의 `CopyLineProperty`가 common OpenCV threshold/
ROI/mask와 Line edge/scan/fit/draw 필드를 모두 직접 복사했다. 이 mapping은
독립적으로 검증할 수 있고 이미 Line pipeline property 생성/변환을 소유한
`VisionPipelineLinePropertyAdapter`와 같은 property family이므로,
`ApplySampleProperty`를 기존 adapter에 추가해 View의 중복 owner를 제거했다.

실제 owner/call path는 다음과 같다.

```text
OpenVisionNativeCustomToolFactory.CreateLine
  -> VisionToolCompositionService.CreateLineToolViewModel
  -> LineToolPresenter -> LineToolWpfView
  -> OpenVisionNativeToolDocument.ApplySampleStepParameters
  -> VisionPipelineStepPropertyMapper.CreateProperty
  -> VisionPipelineLinePropertyAdapter.TryCreateLineGaugePair
  -> LineToolWpfView.ApplySampleLinePair
  -> VisionPipelineLinePropertyAdapter.ApplySampleProperty (Line A/B)
  -> LineToolInteractionController.SetPurposeForTest
  -> LineToolPresenter.PersistProperties
  -> PropertyGrid refresh / input ROI overlay / summary / result cleanup
```

`LineToolViewModel`/`LineToolPresenter`는 mutable A/B property와 persistence를,
`VisionToolPropertyGridHost`/`LineToolInteractionController`/`LineToolPreviewController`
는 PropertyGrid·interaction·overlay·Preview를, factory/composition/base는
creation/release를 계속 소유한다. `CvROIS`/`CvMASKS`는 adapter에서 방어적으로
복사하며, View의 XAML binding/public/test facade와 sample callback 순서는
그대로다. PL-0027의 persistence owner를 다시 분리하지 않았고, 새 service,
interface, mapper, forwarding Partial을 추가할 근거는 없다.

`LineToolPartialBoundaryContract`가 View-to-adapter routing, 전체 mapping,
caller/composition, runtime value 및 list ownership을 4/4로 Debug·Release에서
통과했고, `wpf_shell_host_line_tool`, Line Measure/Intersection, Recipe Line
Pair PropertyGrid WPF smoke와 동적 monitor/window probe도 통과했다. 증거와
completion record는
`docs/reports/OPENVISIONLAB_LINE_TOOL_PARTIAL_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0041.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\line-tool-partial-boundary-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.63 Simple Preprocess Tool View Partial retention (PL-0042)

`SimplePreprocessToolWpfView.xaml.cs`는 required XAML/composition adapter로
유지한다. View는 `toolShell`, dynamic parameter panel, signal inspector,
header/summary/Learn/result facade와 두 개의 View-owned UI helper
(Parameter Guide binder, debounced Preview scheduler)의 lifetime만 조합한다.
직접 settings store, file dialog, `System.IO`, OpenCvSharp image, 또는
algorithm construction을 소유하지 않는다.

기존 owner는 다음과 같다.

- `SimplePreprocessParameterController`: dynamic ComboBox/TextBox/Slider/
  CheckBox registry, parsing/clamping, visibility, guide bindings, settings
  snapshot/apply.
- `SimplePreprocessTextPresenter`와 `ToolController`: header, title, summary,
  localization projection.
- `VisionToolParameterChangeController`와
  `VisionToolDebouncedPreviewScheduler`: parameter change invalidation과
  debounced Preview 요청.
- `OpenVisionNativeSimplePreprocessPropertyFactory`와
  `OpenVisionNativeSimplePreprocessPreviewExecutor`: property projection,
  algorithm execution, result review, Histogram signal evidence.
- `OpenVisionNativeSimplePreprocessDocumentFactory`: settings Load/Save,
  descriptor/step composition, document builder 연결.
- `VisionToolSingleInputCustomToolViewBase`: controller release 순서.

호출 경로는 다음과 같다.

```text
OpenVisionNativeToolRegistry (Edge / RotateScale / HSV / Mean / Histogram)
  -> OpenVisionNativeSimplePreprocessDocumentFactory.Create*Document
  -> SimplePreprocessToolWpfView + SimplePreprocessParameterController
  -> ViewConfigurator / PropertyFactory
  -> settings Load/Save + SingleInputToolDocumentBuilder
  -> DebouncedPreviewScheduler -> SimplePreprocessPreviewExecutor
  -> View result/signal facade -> base DisposeView
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 WPF 계약을 감싸는
중복 owner가 되므로 production split은 만들지 않았다. 이를
`SimplePreprocessPartialBoundaryContract` 9/9 Debug·Release와 focused WPF
smoke/동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_SIMPLE_PREPROCESS_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0042.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\simple-preprocess-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.64 Affine Transform Tool View Partial retention (PL-0043)

`AffineTransformToolWpfView.xaml.cs`는 required XAML/controller adapter로
유지한다. View는 `InitializeComponent`, `toolShell`/Learn contract,
`VisionToolSingleInputPropertyToolController<AffineTransformProperty>` 연결,
result-review/test facade만 소유한다. 직접 persistence, dialog, file-I/O,
OpenCvSharp image, 또는 Affine algorithm construction은 없다.

기존 owner는 다음과 같다.

- `AffineTransformToolViewModel`: mutable `AffineTransformProperty`, summary,
  `DeepCopy()` property facade.
- `VisionToolSingleInputPropertyToolController<AffineTransformProperty>`:
  PropertyGrid binding, layer/preview events, result-review/test facade, and
  controller lifetime.
- `AffineTransformResultReviewPresenter`: matrix, valid-pixel, determinant,
  triangle-area result explanation and guidance.
- `OpenVisionNativeToolPreviewExecutor`와
  `OpenVisionNativeToolPreviewOverlayRenderer`: Affine algorithm execution and
  transformed-image overlay.
- `OpenVisionNativePropertyGridToolFactory`,
  `VisionToolCompositionService`, and document builder: property load/save,
  ViewModel/View creation, and native document composition.
- `OpenVisionNativeToolRegistry`와
  `VisionToolSingleInputPropertyToolViewBase`: registration and final release.

호출 경로는 다음과 같다.

```text
OpenVisionNativeToolRegistry (AffineTransform)
  -> OpenVisionNativePropertyGridToolFactory.CreateAffineTransform
  -> PropertySessionStore.GetOrLoad
  -> VisionToolCompositionService.CreateAffineTransformToolViewModel
  -> AffineTransformToolWpfView + generic property controller
  -> OpenVisionNativeToolPreviewExecutor / result presenter / overlay renderer
  -> PropertyGrid document persistence and base DisposeView
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 controller/factory
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`AffineTransformPartialBoundaryContract` 9/9 Debug·Release와 focused WPF
smoke/동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_AFFINE_TRANSFORM_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0043.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\affine-transform-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.65 Arithmetic Tool View Partial retention (PL-0044)

`ArithmeticToolWpfView.xaml.cs`는 required XAML/double-input adapter로
유지한다. View는 `InitializeComponent`, `toolShell`/Learn contract,
`VisionToolDoubleInputCustomToolController` 연결, Arithmetic facade와
View-owned collaborator release만 소유한다. 직접 persistence, dialog,
file-I/O, OpenCvSharp image, 또는 Arithmetic algorithm construction은 없다.

기존 owner는 다음과 같다.

- `ArithmeticToolInteractionController`: operation/source/constant/offset
  editor state, input validation, event attach/detach, settings projection,
  and mode visibility policy.
- `ArithmeticToolTextPresenter`: localization, Run Offset caption, and
  summary text projection.
- `ArithmeticToolPreviewController`: debounced Preview/Offset scheduling and
  scheduler lifetime.
- `VisionToolDoubleInputCustomToolController`, `VisionToolDoubleInputViewModel`,
  and `VisionToolDoubleInputViewBinder`: layer binding, command/event routing,
  preview slots, language/runtime wiring, and shared release.
- `OpenVisionNativeArithmeticDocumentFactory` and
  `OpenVisionNativeToolDocument`: settings Load/Save, operation-list
  composition, layer/preview/Offset routing, and Arithmetic pipeline step.
- `OpenVisionNativeToolRegistry` and
  `VisionToolDoubleInputCustomToolViewBase`: registration and final release.

호출 경로는 다음과 같다.

```text
OpenVisionNativeToolRegistry (Arithmetic)
  -> OpenVisionNativeArithmeticDocumentFactory.Create
  -> ArithmeticToolWpfView + InteractionController/TextPresenter/PreviewController
  -> VisionToolDoubleInputCustomToolController
  -> VisionToolDoubleInputViewModel / ViewBinder / shared shell runtime
  -> OpenVisionNativeToolDocument (BindArithmetic, layer/preview/pipeline routing)
  -> settings store and Arithmetic pipeline step
  -> VisionToolDoubleInputCustomToolViewBase.DisposeView
```

`suppressEvents`는 controller에 전달되는 WPF re-entry guard일 뿐 독립
Arithmetic domain state가 아니다. 독립 state/lifetime/test seam이 없는
상태에서 새 ViewModel, service, interface, manager, forwarding Partial을
추가하면 기존 owner를 감싸는 중복 경계가 되므로 production split은 만들지
않았다. 이를 `ArithmeticToolPartialBoundaryContract` 9/9 Debug·Release,
`wpf_arithmetic_tool_learn_button`, `wpf_layer_selection_arithmetic_tool`
smoke와 동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_ARITHMETIC_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0044.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\arithmetic-tool-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.66 Filter Tool View Partial retention (PL-0045)

`FilterToolWpfView.xaml.cs`는 required XAML/custom-tool adapter로 유지한다.
View는 `InitializeComponent`, `toolShell`/Learn contract,
`VisionToolFilterInteractionController`, `VisionToolKernelSizeController`,
`FilterToolTextPresenter`, `VisionToolCustomParameterGuideBinder`, shared
custom-tool controller 연결, `CreateProperty()` facade, and View-owned
collaborator release만 소유한다. 직접 persistence, dialog, file-I/O,
OpenCvSharp image, 또는 Filter algorithm construction은 없다.

기존 owner는 다음과 같다.

- `VisionToolFilterInteractionController`: Filter type/border selection event,
  binding flush, and mode-panel visibility policy.
- `VisionToolKernelSizeController`: kernel TextChanged, W=H lock, preset
  buttons, event attach/detach, and parameter-binding update policy.
- `FilterToolTextPresenter` and `VisionToolCustomParameterGuideBinder`:
  localization and parameter-help focus/value projection.
- `FilterToolViewModel` + `FilterToolPresenter`: mutable parameters,
  normalization, summary, settings persistence, and binding facade.
- `VisionToolSingleInputCustomToolController`/Runtime and base View: layer,
  preview, summary/result-review, language, and final lifetime release.
- `VisionToolCompositionService`, `OpenVisionNativeCustomToolFactory`,
  `OpenVisionNativeCustomToolDocumentBuilder`, registry, and pipeline builder:
  settings load, Filter creation, document/preview/pipeline routing, and
  registration.

호출 경로는 다음과 같다.

```text
OpenVisionNativeToolRegistry (Filter)
  -> OpenVisionNativeCustomToolFactory.CreateFilter
  -> VisionToolCompositionService.CreateFilterToolViewModel
  -> FilterToolPresenter -> FilterToolWpfView + Filter collaborators
  -> VisionToolSingleInputCustomToolController / Runtime
  -> OpenVisionNativeCustomToolDocumentBuilder
  -> Filter preview/tool execution and VisionPipelineStepBuilder.FromFilterProperty
  -> VisionToolSingleInputCustomToolViewBase.DisposeView
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 owner를 감싸는
중복 경계가 되므로 production split은 만들지 않았다. 이를
`FilterToolPartialBoundaryContract` 9/9 Debug·Release와 focused WPF smoke/
동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_FILTER_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0045.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\filter-tool-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.67 Blob Tool View Partial retention (PL-0046)

`BlobToolWpfView.xaml.cs`는 required XAML/property-grid adapter로 유지한다.
View는 `InitializeComponent`, `toolShell`/Learn contract, generic property-grid
presenter/controller 연결, area verification presenter, threshold teaching
controller, `CreateProperty()`/review facade, and View-owned collaborator
composition만 소유한다. 직접 persistence, dialog, file-I/O, OpenCV/Blob
algorithm construction, 또는 pipeline policy는 없다.

기존 owner는 다음과 같다.

- `BlobToolViewModel` + `VisionToolCompositionService`: mutable Blob
  parameters, range normalization, summary, snapshot, and ViewModel creation.
- `VisionToolSingleInputPropertyToolController`/Runtime/Presenter: property
  binding, layer/preview/result-review events, auto-preview, debounce,
  persistence callback, presets, and final shared release.
- `VisionToolAreaVerificationGuidePresenter` + criteria text and
  `VisionToolThresholdTeachingPreviewController`: teaching text, Blob result
  metrics, status, request flag, and review reset.
- `OpenVisionNativePropertyGridToolFactory` and property-grid/single-input
  document builders: settings/session composition, View construction, document
  wiring, and input/output routing.
- `OpenVisionNativeToolPreviewExecutor` + overlay renderer: `BlobTool`
  construction, threshold teaching preview, Blob result capture, and result
  image projection.
- Registry, `VisionPipelineStepBuilder`, and
  `VisionToolSingleInputPropertyToolViewBase`: registration, Blob pipeline step,
  and final View/controller lifetime release.

호출 경로는 다음과 같다.

```text
OpenVisionNativeToolRegistry (Blob)
  -> OpenVisionNativePropertyGridToolFactory.CreateBlob
  -> VisionToolCompositionService.CreateBlobToolViewModel
  -> OpenVisionNativePropertyGridToolDocumentBuilder
  -> BlobToolWpfView + generic controller/runtime/presenter
     + area-verification and threshold-teaching presenters
  -> OpenVisionNativeSingleInputToolDocumentBuilder
  -> OpenVisionNativeToolPreviewExecutor -> BlobTool -> overlay renderer
  -> VisionPipelineStepBuilder BlobProperty step (when pipelined)
  -> VisionToolSingleInputPropertyToolViewBase.DisposeView
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 owner를 감싸는
중복 경계가 되므로 production split은 만들지 않았다. 이를
`BlobToolPartialBoundaryContract` 11/11 Debug·Release와 Blob shell/Learn
WPF smoke 및 동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_BLOB_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0046.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\blob-tool-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.68 Contour Tool View Partial retention (PL-0047)

`ContourToolWpfView.xaml.cs`는 required XAML/property-grid adapter로 유지한다.
View는 `InitializeComponent`, `toolShell`/Learn contract, generic property-grid
presenter/controller 연결, area verification presenter, threshold teaching
controller, `CreateProperty()`/review facade, and View-owned collaborator
composition만 소유한다. 직접 persistence, dialog, file-I/O, OpenCV/Contour
algorithm construction, 또는 pipeline policy는 없다.

기존 owner는 다음과 같다.

- `ContourToolViewModel` + `VisionToolCompositionService`: mutable Contour
  parameters, range/epsilon/thickness normalization, summary, snapshot, and
  ViewModel creation.
- `VisionToolSingleInputPropertyToolController`/Runtime/Presenter: property
  binding, layer/preview/result-review events, auto-preview, debounce,
  persistence callback, presets, and final shared release.
- `VisionToolAreaVerificationGuidePresenter` + criteria text and
  `VisionToolThresholdTeachingPreviewController`: teaching text, Contour result
  metrics, status, request flag, and review reset.
- `OpenVisionNativePropertyGridToolFactory` and property-grid/single-input
  document builders: settings/session composition, View construction, document
  wiring, and input/output routing.
- `OpenVisionNativeToolPreviewExecutor` + overlay renderer: `ContourTool`
  construction, threshold teaching preview, Contour result capture, and result
  image projection.
- Registry, `VisionPipelineStepBuilder`, and
  `VisionToolSingleInputPropertyToolViewBase`: registration, Contour pipeline
  step, and final View/controller lifetime release.

호출 경로는 다음과 같다.

```text
OpenVisionNativeToolRegistry (Contour)
  -> OpenVisionNativePropertyGridToolFactory.CreateContour
  -> VisionToolCompositionService.CreateContourToolViewModel
  -> OpenVisionNativePropertyGridToolDocumentBuilder
  -> ContourToolWpfView + generic controller/runtime/presenter
     + area-verification and threshold-teaching presenters
  -> OpenVisionNativeSingleInputToolDocumentBuilder
  -> OpenVisionNativeToolPreviewExecutor -> ContourTool -> overlay renderer
  -> VisionPipelineStepBuilder ContourProperty step (when pipelined)
  -> VisionToolSingleInputPropertyToolViewBase.DisposeView
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 owner를 감싸는
중복 경계가 되므로 production split은 만들지 않았다. 이를
`ContourToolPartialBoundaryContract` 11/11 Debug·Release와 Contour shell/Learn
WPF smoke 및 동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_CONTOUR_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0047.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\contour-tool-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.69 Binary Learn View Partial retention (PL-0048)

`BinaryLearnView.xaml.cs`는 Morphology/Blob/Contour Learn panel의 required
XAML/presentation adapter로 유지한다. View는 named controls와 cell lists,
brush projection, topic visibility, three `DispatcherTimer` instances,
animation button/selection handlers, related-tool callback invocation, and
internal test facade만 소유한다. 직접 simulation model, file-I/O, dialog,
OpenCV/tool creation, persistence, 또는 inspection policy는 없다.

기존 owner는 다음과 같다.

- `BinaryLearnPresenter`: fixed binary samples, morphology/blob/contour stage
  state, simulation results, threshold/draw decisions, formulas, explanations,
  and related-tool guidance text.
- `OpenVisionLearnBinarySimulationModel`: WPF-free morphology, connected blob,
  contour-pixel, and bounds calculation primitives called by the presenter.
- `OpenVisionLearnWindow` + `OpenVisionLearnTopicPresentationPolicy`: selected
  topic interpretation, child View composition, `SelectTopic` routing, and
  `Action<VISION_MENU>` callback injection.
- `BinaryLearnView`: only WPF cell painting, animation stepping, topic panel
  visibility, callback forwarding, and deterministic timer attach/detach.
- `LearnBinaryLineSmoke` and existing internal `*ForTest` accessors: focused
  screenshot/test contract; they are not lesson-state owners.

호출 경로는 다음과 같다.

```text
OpenVisionLearnWindow.UpdateSelectedTopic
  -> BinaryLearnView.SelectTopic(topicIndex)

OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> BinaryLearnView.SetOpenRelatedToolAction(action)
  -> BinaryLearnView.OpenRelatedToolButton_Click
  -> action(VISION_MENU.Morphology | Blob | Contour)
  -> existing Learn/Shell Tool composition

BinaryLearnView constructor
  -> BinaryLearnPresenter + simulation results
  -> Build*Cells + Update*Guide
  -> DispatcherTimer Tick -> Advance*Animation -> Paint*AnimationFrame
  -> Unloaded -> StopAnimations + Tick unsubscribe
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/window
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`BinaryLearnPartialBoundaryContract` 9/9 Debug·Release와 Binary/Line Learn
focused WPF smoke 및 동적 monitor probe로 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_BINARY_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0048.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\binary-learn-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native related-tool click/file association,
camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.70 Foundation Learn View Partial retention (PL-0049)

`FoundationLearnView.xaml.cs`는 Point/ROI와 Mat-channel Learn panel의
required XAML/presentation adapter로 유지한다. View는 named controls와
Foundation Mat cell/marker/brush projection, topic visibility, related-tool
callback forwarding, 두 `DispatcherTimer`, animation button/step/reset과
internal test facade만 소유한다. 직접 file-I/O, dialog, OpenCV, simulation,
Tool creation, persistence 또는 inspection policy는 없다.

기존 owner는 다음과 같다.

- `FoundationLearnPresenter`: Point/ROI·Mat channel stage, role/visibility,
  fixed guidance text와 Tool location policy.
- `OpenVisionLearnWindow` + existing topic policy/caller: selected topic,
  child View composition, callback injection, Window close propagation and
  public test facade.
- `FoundationLearnView`: WPF cell/marker/text/opacity/stroke rendering and
  520/620ms timer attach/detach lifetime only.
- `LearnFoundationPresentationContract` and Foundation WPF smoke: focused
  presenter/view/Window contract; they are not lesson-state owners.

```text
OpenVisionLearnWindow.UpdateSelectedTopic
  -> FoundationLearnView.SelectTopic(topicIndex)
OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> FoundationLearnView.SetOpenRelatedToolAction(action)
  -> FoundationLearnView.OpenRelatedToolButton_Click
  -> existing Learn/Shell Tool composition
FoundationLearnView constructor
  -> FoundationLearnPresenter
  -> BuildFoundationCells / Update*Guide
  -> DispatcherTimer Tick -> presenter.Advance* -> WPF projection
  -> Unloaded -> StopAnimations + Tick unsubscribe
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/window
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`FoundationLearnPartialBoundaryContract` 9/9 Debug·Release, Foundation
contract/view WPF smoke, and dynamic monitor/window placement probe로 보호했다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_FOUNDATION_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0049.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\foundation-learn-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native related-tool click/file association,
camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.71 Grayscale Learn View Partial retention (PL-0050)

`GrayscaleLearnView.xaml.cs`는 Threshold, Brightness, Filtering, Arithmetic
네 topic의 required XAML/presentation adapter로 유지한다. View는 named
controls, cells/histogram/threshold marker, brush/text projection, topic
visibility, related-tool callback, Apply/Close/ThresholdToolOpened events,
네 `DispatcherTimer`와 internal test facade만 소유한다. 직접 file-I/O,
dialog, OpenCV, simulation model, Tool creation, persistence 또는 inspection
policy는 없다.

기존 owner는 다음과 같다.

- `GrayscaleLearnPresenter`: 네 topic의 evaluation, mutable stage, cell roles,
  formulas/status, threshold normalization, and Tool location text.
- `OpenVisionLearnBasicGrayscaleSimulationModel`: WPF-free threshold,
  brightness, arithmetic, and filter sample/evaluation primitives, called only
  by the presenter.
- `OpenVisionLearnWindow`: selected topic, callback composition, Apply/Close
  result forwarding, child event lifetime, and public test facade.
- `GrayscaleLearnView`: WPF input/control projection, cell/marker rendering,
  callback/event forwarding, and 80/420/520ms timer attach/detach only.
- `LearnGrayscalePresentationContract` and `LearnGrayscaleSmoke`: focused
  presenter/Window/standalone View contract; they are not lesson-state owners.

```text
OpenVisionLearnWindow constructor
  -> GrayscaleLearnView.InitializeThreshold / Update*Guide
OpenVisionLearnWindow.UpdateSelectedTopic
  -> GrayscaleLearnView.SelectTopic(topicIndex)
OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> GrayscaleLearnView.SetOpenRelatedToolAction(action)
  -> GrayscaleLearnView.OpenRelatedToolButton_Click
  -> existing Learn/Shell Tool composition
GrayscaleLearnView input/Timer
  -> GrayscaleLearnPresenter.Update*/Advance*/NextThresholdAnimationValue
  -> WPF cells/marker/formula/status projection
GrayscaleLearnView.ApplyButton_Click
  -> ApplyThresholdRequested -> OpenVisionLearnWindow handler/public event
GrayscaleLearnView.Unloaded / Window.OnClosed
  -> StopAnimations + Tick/event unsubscribe
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/Window
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`GrayscaleLearnPartialBoundaryContract` 11/11 Debug·Release, Grayscale
contract/view WPF smoke, and dynamic monitor/window placement probe로 보호했다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_GRAYSCALE_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0050.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\grayscale-learn-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native related-tool click/file association,
camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.72 Layer/Recipe Learn View Partial retention (PL-0051)

`LayerRecipeLearnView.xaml.cs`는 Layer 흐름, Pipeline Step 연결, Recipe 설명을
표시하는 required XAML/presentation adapter로 유지한다. View는 named controls,
Layer/flow cells, brush/text projection, Play/Step/Reset handlers, 520ms
`DispatcherTimer`와 internal test facade만 소유한다. 직접 file-I/O, dialog,
OpenCV, persistence, real Recipe execution, Tool creation 또는 algorithm
lifetime 호출은 없다.

기존 owner는 다음과 같다.

- `LayerRecipeLearnPresenter`: 고정 Layer 목록, Step Input/Tool/Output route,
  selected/animation stage, formula/meaning/status와 route/row 판정.
- `OpenVisionLearnWindow`: topic visibility, `RefreshSelection`, public
  `LayerRecipe*ForTest` facade와 child close lifetime.
- `LayerRecipeLearnView`: WPF input/control projection, cell/brush/text rendering,
  Play/Pause/Step/Reset interaction과 timer attach/detach만 담당.
- `LearnLayerRecipeContract` and `LearnLayerRecipeSmoke`: route/state와
  Window/standalone View lifetime를 검증하는 focused test owners.

```text
OpenVisionLearnWindow.UpdateSelectedTopic
  -> layerRecipeLearnView.Visibility / RefreshSelection
LayerRecipeLearnView slider/button/timer
  -> LayerRecipeLearnPresenter.SelectStep/ResetAnimation/AdvanceAnimation
  -> WPF cells, formula/meaning/status, slider and Play projection
OpenVisionLearnWindow.OnClosed
  -> layerRecipeLearnView.StopAnimation
  -> LayerRecipeLearnView.Unloaded -> timer Stop + Tick unsubscribe
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/Window
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`LayerRecipeLearnPartialBoundaryContract` 12/12 Debug·Release, 기존
Layer/Recipe presenter contract 4/4 Debug·Release, Layer/Recipe contract/view
WPF smoke와 Debug/Release dynamic monitor/window placement probe로 보호했다.
증거와 completion record는
`docs/reports/OPENVISIONLAB_LAYER_RECIPE_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0051.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\layer-recipe-learn-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 Recipe execution/persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.73 Geometry Learn View Partial retention (PL-0052)

`GeometryLearnView.xaml.cs`는 Rotate/Scale 학습, Affine 관련 Tool 안내와 ROI
좌표 재검토를 표시하는 required XAML/presentation adapter로 유지한다. View는
named controls, source/target transform projection, brush/text projection,
related-tool callback, Play/Step/Reset handlers, 520ms `DispatcherTimer`와
internal test facade만 소유한다. 직접 file-I/O, dialog, OpenCV, persistence,
real Recipe execution, Tool creation 또는 algorithm lifetime 호출은 없다.

기존 owner는 다음과 같다.

- `GeometryLearnPresenter`: Angle/Scale state, Rotate→Scale→ROI review stage,
  output size, semantic roles, formula/status와 Rotate/Scale·Affine Tool hint policy.
- `OpenVisionLearnWindow`: topic selection/visibility, guide refresh, callback
  composition, public `Geometry*ForTest` facade와 child close lifetime.
- `GeometryLearnView`: WPF input/control projection, transform/brush/text rendering,
  callback forwarding과 timer attach/detach만 담당.
- `LearnGeometryPresentationContract`, `LearnGeometrySmoke` and
  `LearnGeometryViewSmoke`: stage/policy와 Window/standalone View lifetime를
  검증하는 focused test owners.

```text
OpenVisionLearnWindow.UpdateSelectedTopic / ApplyTopicGuideUpdates
  -> GeometryLearnView.SelectTopic / UpdateGeometryGuide
GeometryLearnView slider/button/timer
  -> GeometryLearnPresenter.UpdateSettings/ResetAnimation/AdvanceAnimation
  -> WPF roles, RotateTransform/ScaleTransform, formula/status projection
GeometryLearnView.OpenRelatedToolButton_Click
  -> callback(VISION_MENU) -> GeometryLearnPresenter.UpdateToolLocation
OpenVisionLearnWindow.OnClosed
  -> GeometryLearnView.StopAnimations
  -> GeometryLearnView.Unloaded -> timer Stop + Tick unsubscribe
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/Window
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`GeometryLearnPartialBoundaryContract` 12/12 Debug·Release, 기존 Geometry
presenter contract 4/4 Debug·Release, Geometry contract/view WPF smoke와
Debug/Release dynamic monitor/window placement probe로 보호했다. 증거와
completion record는
`docs/reports/OPENVISIONLAB_GEOMETRY_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0052.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\geometry-learn-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 Rotate/Scale·Affine Recipe execution/persistence 및 long-running native
runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.74 Metrics Acceptance Learn View Partial retention (PL-0053)

`MetricsAcceptanceLearnView.xaml.cs`는 Good/Bad metric gate 학습을 표시하는
required XAML/presentation adapter로 유지한다. View는 named controls,
sample-cell 생성, neutral/pass/warning brush와 formula/status text projection,
Play/Step/Reset interaction, 520ms `DispatcherTimer`와 internal test facade만
소유한다. 직접 file-I/O, dialog, OpenCV, persistence, real Recipe execution,
Tool creation 또는 algorithm lifetime 호출은 없다.

기존 owner는 다음과 같다.

- `MetricsAcceptanceLearnPresenter`: 고정 샘플, 평균·범위·최대값, average/
  outlier gate 판정, animation stage, formula/status와 숫자 text policy.
- `OpenVisionLearnWindow`: topic visibility, initial/topic refresh, public
  `MetricsAcceptance*ForTest` facade와 child close lifetime.
- `MetricsAcceptanceLearnView`: WPF sample-cell/brush/text projection,
  Play/Step/Reset 버튼 상태와 timer attach/detach만 담당.
- `LearnMetricsAcceptanceContract` and `LearnMetricsAcceptanceSmoke`: presenter
  invariant와 Window/standalone View stage/timer/close contract를 검증한다.

```text
OpenVisionLearnWindow constructor / topic update
  -> metricsAcceptanceLearnView.RefreshFrame / Visibility
MetricsAcceptanceLearnView Play/Step/Reset/timer
  -> MetricsAcceptanceLearnPresenter.ResetAnimation/AdvanceAnimation
  -> sample cells, formula/status, and button projection
OpenVisionLearnWindow.OnClosed
  -> MetricsAcceptanceLearnView.StopAnimation
  -> MetricsAcceptanceLearnView.Unloaded -> timer Stop + Tick unsubscribe
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/Window
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`MetricsAcceptanceLearnPartialBoundaryContract` 12/12 Debug·Release,
기존 Metrics Acceptance contract 3/3 Debug·Release, WPF contract/view smoke와
Debug/Release dynamic monitor/window placement probe로 보호했다. 증거와
completion record는
`docs/reports/OPENVISIONLAB_METRICS_ACCEPTANCE_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0053.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\metrics-acceptance-learn-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 Recipe execution/persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.75 VisionToolVerificationGuideView Partial retention (PL-0054)

`VisionToolVerificationGuideView.xaml.cs`는 area/matching verification
presenter가 계산한 Header/State/Criteria/NextAction/StateBrush를 표시하는
required XAML/presentation adapter로 유지한다. View는 dependency property
계약, named automation surfaces, tooltip/trimming과 `IsCompactMode`에 따른
header/gap/next-action row 및 chrome padding projection만 소유한다. 직접
file-I/O, dialog, OpenCV, Recipe execution/persistence, ViewModel policy,
async work 또는 독립 business state/lifetime 호출은 없다.

기존 owner는 다음과 같다.

- `VisionToolAreaVerificationGuidePresenter<TProperty, TResult>`: area criteria,
  result/status, next-action과 state brush policy.
- `VisionToolMatchingVerificationGuidePresenter`: matching/edge/feature criteria,
  result/status, next-action과 state brush policy.
- `VisionToolSingleInputPropertyToolShell`: `ToolContent`, visibility, shared
  shell binding과 compact-density routing.
- `VisionToolVerificationGuideView`: dependency-property and WPF visual
  projection only; no independent mutable business state or lifetime.
- Blob/Contour/Edge Based Matching views and the shared matching runtime:
  representative Tool composition consumers.

```text
Area/Matching VerificationGuidePresenter
  -> VisionToolVerificationGuideView dependency properties
  -> VisionToolVerificationGuideView.xaml visual/automation projection
VisionToolSingleInputPropertyToolShell.ApplyToolContentDensity
  -> IsCompactMode -> named guide row/padding projection
Tool view/runtime -> common shell ToolContent composition
```

독립 state/lifetime/test seam이 없는 상태에서 새 ViewModel, service,
interface, manager, forwarding Partial을 추가하면 기존 presenter/shell
owner를 감싸는 중복 경계가 되므로 production split은 만들지 않았다. 이를
`VisionToolVerificationGuidePartialBoundaryContract` 12/12 Debug·Release,
공통 Blob/Contour/Matching WPF shell smoke, Debug/Release dynamic
monitor/window placement probe, Readiness, RefactorAudit, DocumentationIndex,
LLM index parse와 `git diff --check`로 보호한다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_VERIFICATION_GUIDE_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0054.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\verification-guide-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 inspection execution/persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.

### 9.76 VisionToolParameterGuideView Partial retention and catalog fallback (PL-0055)

`VisionToolParameterGuideView.xaml.cs`는 selected property에 대한
`VisionToolParameterGuideContent`를 XAML control에 투영하고 related-property
button을 명시적 focus callback으로 전달하는 required presentation adapter로
유지한다. `ShowPrompt`, `ShowContent`, `SetCompactMode`는 WPF state와
automation/text/scroll projection만 변경하며, PropertyGrid selection policy,
file I/O, dialog, OpenCV, Recipe execution, persistence, algorithm, timer,
독립 lifetime을 소유하지 않는다.

기존 owner는 다음과 같다.

- `VisionToolParameterGuidePresenter`: selected object/property, refresh와
  related-property navigation.
- `VisionToolParameterGuideCatalog`: definitions, applicability, localization,
  fallback wording/value formatting와 descriptor resolution.
- `VisionToolCustomParameterGuideBinder`: PropertyGrid focus/value events,
  language refresh와 disposal.
- `VisionToolPropertyGridHost`: standard selection/value wiring와 presenter
  lifetime.
- `VisionToolParameterGuideSidecarController` 및
  `VisionToolSingleInputPropertyToolShell`: floating-window lifetime/position과
  guide visibility/composition.

전역 `DynamicPropertyGridTypeDescriptionProvider`가 inactive dependent
property를 `TypeDescriptor`에서 숨긴 경우에도 catalog가 guidance를 만들 수
있도록 `GetPropertyDescriptor`에 public-instance reflection fallback을 추가했다.
정상 descriptor와 그 attributes를 우선 사용하고, descriptor가 없는 public
property에만 `TypeDescriptor.CreateProperty`를 적용하므로 PropertyGrid
visibility policy가 View로 새어 나오지 않는다.

```text
PropertyGrid event
  -> VisionToolCustomParameterGuideBinder
  -> VisionToolParameterGuidePresenter
  -> VisionToolParameterGuideCatalog.Resolve/GetPropertyDescriptor
  -> VisionToolParameterGuideView.ShowContent
  -> related-property callback -> PropertyGrid focus
```

이 경계에는 별도 mutable state/lifetime/test seam이 없으므로 View를 새
ViewModel/service/Partial로 분리하지 않았다. `VisionToolParameterGuidePartialBoundaryContract`
14/14 Debug·Release, `p257`/`p259`/`p260` WPF precheck Debug·Release,
동적 작은 왼쪽 모니터 배치 Debug·Release와 Smoke build로 구조 및 실제
대표 runtime projection을 보호했다. 증거와 completion record는
`docs/reports/OPENVISIONLAB_PARAMETER_GUIDE_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0055.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\parameter-guide-partial-retention-20260914`
에 있다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 inspection execution/persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 대표 WPF Runtime UI 및 동적 모니터 배치 검증 완료 /
전체 Runtime 행렬은 미검증`이다.

### 9.77 VisionToolDoubleInputCustomToolShell Partial retention (PL-0056)

`VisionToolDoubleInputCustomToolShell.xaml.cs`는 두 입력과 output preview를
담는 required XAML `UserControl` Partial이다. `InitializeComponent`, six
dependency properties, named visual facade, Learn click adapter와
`DockedInspectorLayoutController`가 같은 control의 namescope를 구성한다.
중첩 controller는 `IsDockedInspectorMode`, input-B visibility, offset/preview
action row, summary/status density를 named row/column/group에 적용하는
presentation adapter일 뿐이며, recipe/algorithm/Property/Layer state나
파일·dialog·native execution을 소유하지 않는다.

실제 owner map은 다음과 같다.

- `ArithmeticToolWpfView.xaml(.cs)`가 `toolShell`과 parameter content를
  조합한다.
- `VisionToolDoubleInputCustomToolViewBase`가 controller attach와
  `DisposeView` 순서를 소유한다.
- `VisionToolDoubleInputCustomToolController`가 event hub, language refresh,
  controller/runtime release를 소유한다.
- `VisionToolDoubleInputCustomToolRuntime`와
  `VisionToolDoubleInputViewRuntime`이 layer/preview binding, command callback,
  status/summary projection과 preview resource lifetime을 소유한다.
- `OpenVisionToolDockModeHelper`는 shell의 dock DP만 변경하고,
  `VisionToolLearnWindowController`는 Learn Window 생성/re-entry를 소유한다.

```text
ArithmeticToolWpfView.xaml
  -> ArithmeticToolWpfView.xaml.cs
  -> VisionToolDoubleInputCustomToolViewBase.AttachToolController
  -> VisionToolDoubleInputCustomToolController
  -> VisionToolDoubleInputCustomToolRuntime
  -> VisionToolDoubleInputCustomToolShell.xaml(.cs)
  -> named preview/combo/button facade
```

따라서 새 ViewModel, service, interface, wrapper, forwarding Partial을
추가하면 기존 WPF namescope와 동일한 mutable state를 감싸는 중복 owner가
된다. `VisionToolDoubleInputCustomToolShellPartialBoundaryContract` 12/12가
Debug·Release에서 통과했고, `wpf_arithmetic_tool_learn_button`와
`wpf_layer_selection_arithmetic_tool` WPF smoke 및 실제 창을 작은 왼쪽
모니터에 배치한 dynamic monitor probe가 두 구성에서 통과했다. 증거와
developer reading order는
`docs/reports/OPENVISIONLAB_DOUBLE_INPUT_SHELL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0056.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\double-input-shell-partial-retention-20260914`
에 있다. 전체 Tool consumer, theme/DPI/input 행렬, native SDK/GPU/camera,
실제 inspection execution/persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 대표 WPF Runtime UI 및 동적 모니터 배치 검증 완료 /
전체 Runtime 행렬은 미검증`이다.

## 10. Libraries 프로젝트

| 프로젝트 | 역할 |
| --- | --- |
| `OpenVisionLab.Mvvm` | `ObservableObject`, `RelayCommand` 등 MVVM 기반입니다. |
| `OpenVisionLab.ImageSpace.Core` | 레이어/image space core model입니다. |
| `OpenVisionLab.Display.Core` | display 관련 core abstraction입니다. |
| `OpenVisionLab.ImageCanvas` | 이미지 캔버스/viewer 관련 library입니다. |
| `OpenVisionLab.Logging` | logging core입니다. |
| `OpenVisionLab.Logging.Controls` | logging UI controls입니다. |
| `OpenVisionLab.Pipeline.Controls` | pipeline UI controls입니다. |
| `OpenVisionLab.Localization` | localization catalog/service입니다. |
| `OpenVisionLab.History` | history/undo-redo 관련 library입니다. |
| `PropertyGrid.Abstractions` | PropertyGrid abstraction입니다. |
| `WpfPropertyGridBridge` | WPF PropertyGrid bridge입니다. |

메인 앱과 내부 라이브러리는 각각 `src/OpenVisionLab/`과 `src/Libraries/`의 독립 프로젝트 루트에 있습니다. 메인 앱은 `ProjectReference`로 필요한 라이브러리를 참조하며, 라이브러리 소스는 메인 프로젝트의 기본 compile glob 범위에 포함되지 않습니다.

## 11. 검증 도구와 smoke 전략

검증 도구는 `tools/` 아래에 있습니다.

| 도구 | 역할 |
| --- | --- |
| `PipelineViewerScreenshotSmoke` | WPF UI를 실제로 띄워 screenshot과 runtime 상태를 검사하는 핵심 smoke입니다. |
| `ScreenshotSmokeTargetRunner` | 기존 target/suite catalog를 받아 명령행 선택·실행·결과/오류 보고를 담당하는 window-free owner입니다. |
| `VisionUiContractCheck` | UI contract/static contract 점검입니다. |
| `OpenVisionReadinessCheck` | readiness/precheck 성격의 검증입니다. |
| `LocalizationCatalogCheck` | localization key/catalog 검증입니다. |
| `HistoryContractCheck` | history contract 검증입니다. |
| `RecipeXmlCompatibilityCheck` | recipe XML compatibility 검증입니다. |
| `VisionRecipeRunnerSmoke` | recipe runner smoke입니다. |
| `OpenVisionLab.ImageCompare` | standalone image compare utility입니다. |

변경 포인트 검증 원칙:

- 작은 UI 수정은 관련 smoke target만 실행합니다.
- shared Shell, layer route, tool runtime을 건드리면 layer selection/tool preview smoke를 추가로 실행합니다.
- 성능 변경은 `wpf_tool_open_perf` 또는 관련 perf smoke로 baseline을 확인합니다.
- "보인다"만 확인하지 말고, runtime state도 같이 확인합니다. 예: workspace title, active layer, preview result, output bitmap difference.

자주 쓰는 명령:

```powershell
dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build .\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --list
```

## 12. 대표 런타임 시퀀스

### 12.1 툴 열기

```text
사용자 tool rail 클릭
  -> OpenVisionShellHostToolWindowController
  -> OpenVisionNativeToolDocumentCache
  -> OpenVisionNativeToolRegistry.TryCreateDocument
  -> lane factory
  -> OpenVisionNativeToolDocument
  -> OpenVisionFloatingToolWindowHost
```

### 12.2 파라미터 변경 후 preview

```text
PropertyGrid 또는 custom parameter 변경
  -> VisionToolPropertyChangeController
  -> VisionToolDebouncedPreviewScheduler
  -> OpenVisionNativeToolDocument.RunPreview
  -> OpenVisionNativePreviewExecutionController
  -> OpenCV/tool execution
  -> OpenVisionNativePreviewLayerPublisher.PublishPreviewBitmap
  -> tool preview refresh + layer list refresh
```

### 12.3 output preview 클릭

```text
VisionToolInlinePreviewSlot 좌클릭
  -> VisionToolActionBehavior.OutputPreview_MouseUp
  -> OpenVisionNativeToolDocument.OnOutputPreviewClicked
  -> OpenVisionNativeToolRouteInteractionController.HandleSingleOutputPreviewClicked
  -> OpenVisionNativeToolLayerViewController.ActivateLayerIfPresent
  -> DisplayManager active layer 변경
  -> Shell workspace가 output layer 표시
```

### 12.4 output layer 생성

```text
툴의 output 추가 버튼 클릭
  -> VisionToolActionBehavior
  -> OpenVisionNativeToolRouteInteractionController.Handle*CreateOutputLayerRequested
  -> OpenVisionNativeLayerRouteController.SelectNext*OutputLayerName
  -> OpenVisionNativeToolLayerViewController.EnsureOutputLayerFromInput
  -> output layer 준비
  -> input layer route 복원
```

## 13. 새 작업을 시작할 때 보는 순서

| 작업 유형 | 먼저 볼 파일 |
| --- | --- |
| 툴 추가 | `docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`, `OpenVisionNativeToolRegistry.cs` |
| PropertyGrid 버그 | `VisionToolPropertyGridHost.cs`, `WpfPropertyGridBridge`, property model |
| input/output 레이어 버그 | `OpenVisionNativeLayerRouteController.cs`, `OpenVisionNativeToolLayerViewController.cs`, `VisionToolLayerSelectionBehavior.cs` |
| output preview 클릭/메인 표시 버그 | `VisionToolInlinePreviewSlot.cs`, `VisionToolActionBehavior.cs`, `OpenVisionNativeToolRouteInteractionController.cs` |
| 툴 창 표시 속도 | `OpenVisionNativeToolDocumentCache.cs`, `OpenVisionNativeToolPrewarmService.cs`, `OpenVisionNativeToolPrewarmPolicy.cs` |
| 메인 workspace 이미지 표시 | `OpenVisionShellHostView.xaml.cs`, `OpenVisionBitmapCanvasPresenter.cs`, `OpenVisionZoomableImageController.cs` |
| pipeline 저장/실행 | `OpenVisionNativePipelineCommandController.cs`, `OpenVisionPipelineReviewDocument.cs`, `docs/VISION_PIPELINE_*` |
| UI smoke 추가 | `tools/PipelineViewerScreenshotSmoke/Program.cs`, `docs/UI_SCREENSHOT_SMOKE.md` |

## 14. 구조화 원칙

1. View code-behind는 얇게 유지합니다.
2. 공통 동작은 Runtime, Binder, Behavior, Controller, Presenter로 이동합니다.
3. PropertyGrid 기반 툴은 model-driven UI 원칙을 유지합니다.
4. 레이어 라우팅은 개별 view에서 임의로 바꾸지 말고 route controller를 통해 바꿉니다.
5. output 생성과 output 선택은 다른 동작입니다. 자동으로 input을 바꾸지 않습니다.
6. 툴별 특수성은 숨기지 않습니다. 반복 배선을 줄이되 모든 툴을 무리하게 하나의 추상화로 합치지 않습니다.
7. 실제 EXE/스모크로 변경 포인트를 검증합니다.
8. 핵심 책임을 바꾸는 경우 짧은 주석을 남깁니다. 단순 설명 주석은 피합니다.

## 15. 관련 문서

- `docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`
- `docs/VISION_TOOL_PROPERTY_GRID_POLICY.md`
- `docs/VISION_TOOL_CONTRACT.md`
- `docs/VISION_TOOL_RESULT_CONTRACT.md`
- `docs/UI_SCREENSHOT_SMOKE.md`
- `docs/OPENVISIONLAB_PLATFORM_DIRECTION.md`
- `docs/OPENVISIONLAB_WPF_MIGRATION_PLAN.md`
- `docs/VISION_PIPELINE_RECIPE_SPEC.md`
- `docs/VISION_PIPELINE_LLM_RECIPE_CONTRACT.md`
