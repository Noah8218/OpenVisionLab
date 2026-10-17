# OpenVisionLab Codebase Structure

Updated: 2026-10-09 KST

이 문서는 owner를 찾는 최소 코드 탐색 지도입니다. 실제 타입·호출·상태 쓰기·수명과
프로젝트 의존성은 코드와 `.csproj`가 원본입니다. 아래 이름은 검색 진입점이며 완전한
호출 그래프나 구현 완료 선언이 아닙니다. 구조 변경 작성자가 관련 진입점을 갱신합니다.
## Start Here: solution and projects

| 역할 | 프로젝트 | 출력/진입점 |
| --- | --- | --- |
| 제품 실행 앱 | `src/OpenVisionLab/OpenVisionLab.csproj` | `Program.Main`; Windows x64 WPF `WinExe` |
| UI/표시 라이브러리 | `OpenVisionLab.Display.Core`, `ImageCanvas`, `Docking.Controls`, `Logging.Controls`, `Pipeline.Controls`, `WpfPropertyGridBridge` | 앱이 참조하는 View/adapter/control |
| 공통 라이브러리 | `ImageSpace.Core`, `Localization`, `Logging`, `History`, `Mvvm`, `PropertyGrid.Abstractions` | 이미지 상태·현지화·로그·이력·MVVM·속성 계약 |
| 화면 없는 검증 | `OpenVisionReadinessCheck` | source/contract readiness |
| 외부 consumer 검증 | `ImageCanvasExternalConsumerSmoke` | ImageCanvas public dependency/lifetime 계약 |
| 보조 실행 도구 | `OpenVisionLab.ImageCompare` | 별도 Image Compare 실행 도구 |

실행 방법은 [문서 시작점](../README.md#visual-studio에서-열고-실행하기)을 따릅니다. 현재 목록·출력·참조 방향은 `OpenVisionLab.sln`과 각 `.csproj`의 `OutputType`,
`TargetFramework`, `ProjectReference`로 확인합니다. 라이브러리 이름만으로 독립성을
판정하지 않습니다. 지원 플랫폼과 실행 증거 규칙은 [AGENTS](../../AGENTS.md)가 정합니다.

## Product startup reading order

[Program.cs](../../src/OpenVisionLab/Program.cs) →
[OpenVisionLabApplication.cs](../../src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs)
→ `ApplicationRuntimeContext` → `OpenVisionShellHostWindow` → `OpenVisionShellHostView`. 해당 메서드와 생성·해제 지점을 따라갑니다. Visual Studio의 개인별 시작 설정은
솔루션 나열 순서로 추정하지 말고 실제 설정을 확인합니다.

## Main workflow owners

| 책임 | 현재 owner | 읽기 시작점 |
| --- | --- | --- |
| Shell 화면 조합/가시성 | `OpenVisionShellHostView`와 책임별 Shell controller | `UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs` |
| Shell binding 상태 | Shell ViewModel/state presenter | `UI/Menu/Wpf/Shell/State` |
| Recipe 편집·명령 상태 | `OpenVisionShellHostRecipeCommandSurface`와 기존 session/controller owners | `UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` |
| 예제 이미지 첫 선택 | 기존 `SamplePickerWindow/View/ViewModel` → `CommandController.AddSampleImages` → 기존 catalog/importer/Images | `WorkspaceCommands.OpenSampleCommand` → `PromptAndOpenSampleImages` → `Workspace/Samples`; completion/E230에 modal 결과·role/label·theme/binding·수명·검사 경계를 둔다. |
| 현재 이미지 레이어 관리 | Root `OpenVisionWorkbenchViewModel` → 기존 `LayerCommandSurface`/`LayerManagementController`/`LayerListPresenter`/`LayerViewerController` → display/ImageSpace | `OpenVisionShellHostWindow.xaml`의 `layerManagerOverlay` → Root `AttachLayers`와 Shell 구성 → `Shell/Layers`; completion/E229에 browse·변경·입력 보호·수명·binding·검사 경계를 둔다. |
| 선택형 Main LLM 세 단계 | Root `OpenVisionWorkbenchViewModel` → 기존 `RecipeCommandSurface` prompt/validation/import → 기존 LLM review/비교/storage owners | `OpenVisionShellHostWindow.xaml` menu와 HostView footer의 `OpenMainLlmCommand`; completion/E228에 inactive/Legacy activate 분기와 binding·state·lifetime·검사 경계를 둔다. |
| 독립 readonly Main History | Root `OpenVisionWorkbenchViewModel` → `RecipeCommandSurface.ReadMainHistory` → 기존 RunHistory owner/storage → `BatchRunOption` 읽기 projection | `OpenVisionShellHostWindow.xaml`의 `OpenMainHistoryCommand`; completion/E227에 상태·binding·수명·집중 검사와 미검증 경계를 둔다. |
| Main 제목·Layer 이름 어절 표시 | `OpenVisionWorkbenchNameText` (`Shell/Chrome`) | `OpenVisionShellHostView.xaml`의 `txtDockedToolTitle`와 `Windows/OpenVisionShellHostWindow.xaml`의 `managedLayerRows` → `WrapLines`/`MeasureOverride`/`OnRender`. 기존 `Tool.Title`/`LayerCommandSurface.ManagerRows → LayerListPresenter.Rows` 원문·binding을 읽으며 writer는 기존 Pipeline/Layer owner다. 부모 visual tree가 수명을 소유하고 구독·저장·실행은 없다. 기존 title controller는 같은 Text DP의 binding을 보존한다. `WorkbenchNameTextContract`와 를 따른다. |
| Recipe/Pipeline 저장 | Recipe/Pipeline lifecycle·storage owner | `Core/Recipe`, `Core/Pipeline`, Shell Recipe controller |
| Pipeline Review 조합 | `OpenVisionPipelineReviewDocument` | `UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs` |
| Pipeline 실행/취소 | Pipeline Review execution controller → `VisionPipelineExecutionService` | `Core/Pipeline/Execution` |
| Pipeline 출력 할당 preflight | `VisionPipelineOutputAllocationGuard` | `Core/Pipeline/Validation/VisionPipelineOutputAllocationGuard.cs` |
| 결과·drawing 표시 | result/display owner와 Pipeline Review ViewModel/View | `UI/Menu/Wpf/Recipe/Review`, `OpenVisionLab.Display.Core` |
| Native Tool 편집 | Tool ViewModel + Property adapter + interaction/preview controller | `UI/VisionTest/Wpf/ToolViews`에서 해당 Tool 검색 |
| Learn | Learn Window composition + topic presenter | `UI/VisionTest/Wpf/Learn` |
| 이미지 lifetime | 생성한 owner가 명시적으로 Dispose; View는 WPF presentation resource 해제 | 해당 `*ImageResourceOwner`, `DisplayManager`, ImageCanvas owner |

## Recipe/Pipeline reading order

위 표의 Recipe command surface에서 해당 command를 찾고 연결된 session/controller,
Pipeline Review document, execution service와 실제 Tool을 따라갑니다. 결과는 run result에서
review ViewModel binding으로 돌아오는 경로를 확인합니다. 파일명 `RecipeCommandSurface.cs`와
타입명 `OpenVisionShellHostRecipeCommandSurface`는 구분합니다. Teach/Preview/Run·설정 복원·라우팅의 비회귀 조건은
[안정 계약](../contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md)의 일반 규칙과 해당 절을 읽습니다.

관리 목록은 기존 `FilteredRecipeOptions` string과 `RecipeManagerSelectedRecipeName` 선택을 유지한다. `RecipeCommandSurface.RefreshOptions` → 읽기 metadata 캐시 → `RecipeManagerLibraryItemConverter` → manager 행 TextBlock 순으로 읽는다. converter는 WPF MultiBinding adapter이며 I/O·업무 상태·구독을 소유하지 않는다. 저장 세트와 최근 검사 조회는 기존 storage/RunHistoryOrchestrationOwner를 사용한다.

## WPF/MVVM boundary

- View code-behind는 XAML namescope, control event adapter, focus/drag/popup/dialog host,
  framework lifetime과 렌더링 plumbing만 소유합니다.
- ViewModel은 binding state, command state, 사용자 의도와 결과 projection을 소유하며
  `Window`, `UserControl`, concrete control 또는 visual tree를 참조하지 않습니다.
- 파일·네트워크·device/native side effect는 기존 adapter/service/controller owner에 둡니다.
- 새 interface/service/helper는 실제 교체 seam, 독립 lifetime, reuse 또는 test seam이
  있을 때만 추가합니다.
- `partial`은 XAML/generated/framework 조합 또는 기록된 cohesive 경계에서만 유지합니다.

## Main viewer navigation reading order

현재 도구막대의 표시 계약은 `OpenVisionShellHostView.xaml`의 `WorkbenchZoomPercentage`와
`WorkbenchOriginalSize`에서 시작합니다. `WorkspaceCommands` → 기존
`OpenVisionShellHostWorkspaceCommandSurface` → `OpenVisionShellHostCommandController` →
`OpenVisionDockedLayerWorkspaceRuntime.ActiveLayerViewer` → `OpenVisionLayerViewerView` →
`OpenVisionZoomableImageController` 순서로 현재 영상·transform·생성/해제를 찾습니다. 새 owner 분리는 없으며 업무 입력 레이어 선택과 viewer viewport 활성화는 구분합니다. 집중 검사 진입점은 `wpf_workbench_viewer_zoom`, `_compact`, `_single`입니다. 현재 소스 버전·실제 실행/미검증 범위는
Main 배율 기록에 있습니다.

## ImageCanvas owner and reading order

`RoiImageCanvasView.xaml.cs` → `Presentation/RoiImageCanvasPresentation.cs` →
`RoiImageCanvasViewModel.cs` → `External/ImageCanvasExternalConsumerSession.cs` 순서로
생성·DataContext·상태 쓰기·해제 경계를 확인합니다. 연결된 concrete control/input/render
owner는 해당 생성 지점에서 따라갑니다.

관련 검사 진입점은 `RoiImageCanvasBoundaryContract`, `RoiImageCanvasPathPolicyContract`입니다.

## Verification owners

| 검증 | owner/명령 |
| --- | --- |
| solution build | `dotnet build OpenVisionLab.sln -c <Debug|Release> -p:Platform="Any CPU"` |
| source/MVVM/refactor audit | `tools/Invoke-RefactorAudit.ps1 -Verify` |
| readiness | `tools/OpenVisionReadinessCheck` |
| 문서 route/redirect | `tools/TestDocumentationIndex.ps1` |
| 외부 DLL hash/allowlist | `tools/TestExternalReferences.ps1` |
| NOTICE | `tools/TestThirdPartyNoticeCoverage.ps1` |
| sample catalog/assets | `tools/TestPublicSampleAssets.ps1` 및 public catalog runner |
| WPF UI | `tools/RunUiPrecheck.ps1` → `PipelineViewerScreenshotSmoke` |
| actual product EXE | opt-in embedded `OpenVisionLab.DirectSmokeRunner` |
| ImageCanvas external consumer | `tools/ImageCanvasExternalConsumerSmoke` |

검증 도구는 제품 정책을 소유하지 않습니다. 실패를 통과시키기 위해 assertion, fixture,
expected result를 완화하지 않습니다.

## 작업대 owner와 수명

은 기존
`OpenVisionLab.Localization/Resources/LocalizationCatalog.tsv`의 표시값·누락 행과
`OpenVisionLanguageService`의 출고 기본값 이행 목록을 반영했다. 언어 상태·CONFIG merge와 수명은 `OpenVisionLanguageService`, 파라미터 표시와 편집기 연결은
`WpfPropertyGridBridge/PropertyGridMetadataAdapters.cs`가 계속 소유한다. Tool model·runtime·View
소유권은 이동하지 않았고 새 완료 owner도 없다.

는 기존
`OpenVisionLabWpfTheme.xaml`에 Workbench Dark/Light 토큰·font·버튼 스타일을 추가했다. Shell/TitleBar → theme dictionary → packaged Pretendard 경로를 실제 glyph·대비와 집중 UI에서
확인했다. CommandCatalog는 icon Kind, DirectRunPresenter는 badge brush 표현만 계속 소유한다. Tool parameter adapter·형식/순서/편집기와 UI/DataContext/lifetime owner 이동은 없다.

owner는
`UI/Menu/Wpf/Shell/State/OpenVisionWorkbenchViewModel.cs`다(명세 §11.2). 한 번 검색할 읽는 순서: `OpenVisionShellHostWindow.xaml/.cs` → `OpenVisionShellHostView.xaml/.cs`의
`WorkbenchProperty`/생성 → Workbench ViewModel → `Core/State/SystemState.SaveWorkbenchLayout` →
`PipelineViewerScreenshotSmoke`의 `wpf_workbench_layout`/compact/policy/log. Window DataContext와 제목 content slot은 Workbench, 기존 Shell DataContext는 Preview ViewModel을 유지한다. 새 Grid/메뉴는 Workbench에 명시 바인딩하며 기존 Recipe/Chrome/Workspace/Layer 명령 surface를 재사용한다. 배치·접힘·탭의 상태 쓰기는 Workbench가, SYSTEM.xml IO는 callback으로 호출한 기존 SystemState가 맡는다. View의 메뉴/관리 표시 이벤트와 attached size/drag command는 presentation plumbing만 맡는다. Shell이 생성하고 Dispose에서 Workbench의 Recipe/Language 구독·표시 이벤트를 해제한다. K7은 기존 LogPanel/reader를 탭 내용 밖 하단 Grid에 계속 붙여 탭 전환 중 reader 해제를 방지한다. 기존 rail/docked inspector는 binding이 있는 geometry를 덮어쓰지 않으며 옛 unbound 호출 계약은 유지한다. Core 독립성/새 실행 엔진을 주장하지 않는다. 이 완료 경계는 새 요구·재현 결함·기준 실패에서만 재개한다.

owner는
`Core/Pipeline/Definition/VisionPipelineStepEditService.cs`와
`UI/Menu/Wpf/Shell/State/OpenVisionWorkbenchPipelineViewModel.cs`(명세 §11.2)다. 한 번 검색할 읽는 순서: `OpenVisionShellHostView.xaml.cs`의 `AttachWorkbenchPipeline` →
Pipeline VM의 `Edit`/`RefreshRows`/`RunAsync` → Core 단계 편집/Validator → 기존 Review execution/결과 presenter →
RecipeCommandSurface의 `SaveWorkbenchPipeline` → StepEditApplyOwner → PipelineStepEditContract/UI smoke/native roundtrip. Window는 Workbench.Pipeline 명령, 단계 ListBox는 Pipeline VM을 DataContext로 사용하며 기존 Shell Preview VM은 유지한다. 작업본/Undo/Redo/선택/결과 stale/판정은 Pipeline VM만 쓰고, Core는 복사본+검증 결과만 반환한다. Core의 고유 이름은 기존 Append 함수를 공유하며 UI/IO/실행·자동 참조 수정 의존이 없다. 실행은 기존 Review controller, 디스크 쓰기는 기존 Recipe ApplyOwner의 트랜잭션(`applyProperty:false`)이다. 기존 Tool parameter 기본 apply/Append 계약·XML 스키마를 유지한다. RefreshCoordinator의 기존 이미지 로드
갱신 경로가 Pipeline.RefreshInputState까지 이어지며 복원/표시/언어 전환은 실행하지 않는다. View는 rename focus/키·drag/context menu/Closing 전달만 한다. Shell 생성/Dispose가 VM의 Recipe/Display/
Language/execution 구독과 역방향 attach를 해제한다. §3A/K3/K5 계약과 Core/UI/native 증거는 관련 계약에 둔다.
native125%/Light/Tool parameter 전체/결과 전체/현장 운영은 별도 경계다. 기존 closed owner를 나누거나 옮기지 않았다.

저장 단계 되돌리기는 이 기존 Pipeline VM에서 메모리 단계 식별·마지막 성공 저장
snapshot을 소유한다. 이동/이름/편집·Undo/Redo에서 식별을 유지하고 새 삽입/복제에 새 식별을 주며
XML 스키마에는 넣지 않는다. `RevertSavedStep` → 기존 Core `Replace/Validate` → `RefreshRows` →
Tool VM `OnPipelineChanged` → 기존 native document의 복원 억제 구간이 실제 연결 경로다. 저장 baseline writer와 release owner도 기존 Pipeline VM이며 성공 저장/Reload/Dispose에서 관리한다. SingleInput/DoubleInput의 `workbenchStepActions`는 `NativeToolDocument.ActivateWorkbenchEditor` →
`VisionToolChromePresenter.ApplyWorkbenchEditor`가 Tool VM을 DataContext로 주입한다. 자식 버튼은
`RevertCommand`/`RunToCommand`와 카탈로그 indexer를 상속 바인딩하고 새 code-behind 사업 이벤트는 없다. 파라미터 writer·편집기·순서·기존 shell lifetime은 그대로다.

DoubleInput의 기존 `DockedInspectorLayoutController`는 같은 shellRoot의 footer 행 배치도 소유한다. 도킹 단계 모드는 전체 너비의 복원/이 단계까지/실행 한 줄, 별도 창은 기존 오른쪽 열이다.
`WorkbenchStepActions_IsVisibleChanged`는 이 controller의 배치 갱신만 호출하는 View 이벤트다. 오프셋 실행 label은 같은 ChromePresenter가 Tool VM `RunText`에 연결하며 parameter ContentPresenter의
양식/순서/editor와 native writer는 유지한다. 새 owner·partial·release 경계는 없다.

Workbench Arithmetic 입력 B의 읽기 경로는 Tool VM의 첫 테스트 생성 → Document의
`InitializeTestRoute` → 기존 LayerRouteController의 후보 선택이다. 이후 Offset 캡처도 B를 유지한다. 명시 B 파일 로드는 Document의 현재 A/B/출력 snapshot → LayerRouteController의 로드 대상 →
PreviewImageCommandController의 디코드/이미지 게시 → Document의 `OnPreviewImageLoaded` →
Tool VM `SetRoute` 순서다. 실패한 디코드는 마지막 route writer에 도달하지 않는다. 작업본 writer는 기존 Tool VM, 이미지 side effect와 callback 수명은 기존 Document/preview controller다. 캐시 재열기·저장 단계는 초기화하지 않으며 legacy Offset 캡처는 보존한다. 새로운 owner는 없다.

새 테스트 하단은 같은 SingleInput/DoubleInput shell의 actionPanel을 사용한다. 읽기 경로는 `ActivateWorkbenchEditor` → `VisionToolChromePresenter.ApplyWorkbenchEditor` →
Tool VM의 `RestoreDefaultsCommand`/`AddAfterCommand`/`AddBeforeCommand`/`AddEndCommand` →
기존 `AddCurrent`/Pipeline VM `InsertStep`이다. defaults와 ContextMenu의 DataContext는
ChromePresenter가 같은 Tool VM으로 주입하며 결과·파라미터의 writer는 기존 owner를 유지한다.
`AddPipelineButton_Click`는 메뉴를 열고, `WorkbenchActions_IsVisibleChanged`/기존 DoubleInput
visibility 이벤트는 기존 layout controller만 호출한다. View가 실행·저장·단계 정책을 소유하지 않는다.

비동기 활성 상태는 기존 PreviewExecutionBoundary가 쓰고 입력 snapshot·작업 객체·CTS를 해제한다. Boundary의 선택적 callback → Document `QueueWorkbenchExecutionStateChanged`의 UI Dispatcher/세션
확인 → Tool VM `NotifyExecutionState`/`RaiseState` → ChromePresenter의 IsEnabled 바인딩이 실제 경로다. Run은 Workbench의 기존 PrimaryButtonStyle을 재사용하며 legacy 경로와 전역 스타일은 바꾸지 않는다. Doc/cache와 Shell의 기존 Dispose가 이 수명을 소유한다. 대응 검사는 기존
`WorkbenchToolWorkingCopySmoke.RunTestFooter`/Revert와 세 native preview production 계약이다.

판정 조건도 이 같은 Tool VM을 사용한다. NativeToolDocument → ChromePresenter가
SingleInput shell의 `WorkbenchAcceptanceHost.Content`에 Tool VM을 주입하고,
기존 VisionToolWpfTheme의 DataTemplate이 판정 switch·지표·범위·시간을 바인딩한다. 새 code-behind 이벤트는 없다. 숫자 입력/오류와 테스트 판정 writer는 Tool VM이고 단계 값은
`EditAcceptance` → Pipeline VM `ReplaceStep` → 기존 Core 편집/검증 경로로 쓴다. 실행/저장 전 `CanCommitToolEdits` callback은 현재 단계의 미완성 숫자를 확인하며 Tool Dispose가 해제한다. Unit/이름은 기존 ResultPresenter, native property와 parameter editor/lifetime은 기존 owner 그대로다. 읽기 순서는 `RunAcceptance` → `ActivateWorkbenchEditor` → `ApplyWorkbenchEditor` →
`WorkbenchAcceptanceTemplate` → Tool VM `EditAcceptance`/`AcceptanceInputError` → `ReplaceStep`다.

지표 목록/선택 객체는 이 Tool VM의 툴·목적·언어별 cache가 소유한다. 숫자 raw 입력은
현재 편집 세션 범위이며 전환/restore/Dispose에서 해제한다. 본문 높이와 스크롤은 기존
SingleInput `DockedInspectorLayoutController.Apply`가 소유한다. 전용 화면은 자연 높이,
PropertyGrid는 유한 높이와 기존 자체 스크롤이다. parameter subtree는 변경하지 않는다. 후속 경로 행의 읽기 순서는 같은 `ActivateWorkbenchEditor` → `ApplyWorkbenchRoute` → 기존
SingleInput/DoubleInput layout controller → Workbench ComboBox template이다. 경로 Header와 판정
template의 DataContext는 Tool VM에 명시 주입하고 실제 선택 객체를 표시한다. SDK/XML layer key와
파라미터 writer·DataContext·수명은 유지한다. unavailable Line 신호 버튼의 Collapsed는 View 배치만 바꾼다. Workbench 전용 template의 mouse-open은 기존 `ComboBoxInteractionBehavior`를 재사용한다. 직접 출력 거부 복원은 기존 Combo helper의 editable text 동기화와 NativeDoc의 session/lifetime 가드가 있는 Dispatcher 복원으로 검증했다. 읽기 경로는 같은 RefreshLayerState → Binder/Behavior → Presenter → helper다. 정책·파라미터 writer와 수명 owner는 유지한다. 입력 후보의 가장 짧은 읽기 경로는 Tool VM.GetInputLayerChoices → NativeDoc.RefreshLayerState →
기존 Binder/Behavior의 apply 가드 → LayerSelectionPresenter.SetWorkbenchInputChoices/ApplyInputLayerSelection →
Combo helper.GetLayerText다. View의 ItemsSource/ItemTemplate은 표시 snapshot이고 route writer는 같은
Tool VM.SetRoute 및 Pipeline VM.ReplaceStep/test draft다. SDK/XML raw key와 기존 workspace-only
LayerSelection VM, parameter DataContext·writer·Document/Binder Dispose 수명은 바꾸지 않았다. 실제 Native Mean ko/en·1920×1032/96DPI의
목록/키보드 선택 범위만 확인했다. Compact 폭·결과·전체 Light·native125%는 남아 있으며 완료 owner 등록은 아니다.

결과의 읽기 경로는 Tool VM.RunCommand → NativeToolDocument.RunPreview/RunArithmeticOffset →
CaptureWorkbenchResultStep/CaptureStep → 기존 Preview Boundary/Controller → Core VisionPipelineResultSummaryService →
generation/session 가드가 있는 PublishWorkbenchResult → Tool VM.PublishResult 또는 Pipeline VM.PublishToolResult →
ResultPresenter/ChromePresenter.ApplyWorkbenchResult → Single/Double shell template다. 실제 실행 파라미터는 읽기 전용 snapshot이며 작업본/XML에 기본값을 보충하지 않는다. SDK Dispose 전에 managed 값만 전달한다. SDK/Bitmap lifetime은 기존 preview owner, 테스트/단계 결과·stale writer와 release는 기존 두 Workbench VM이다. DataContext는 NativeDoc/Chrome이 주입한다. Window/Host의 F8과 메뉴/툴 버튼은 같은 Tool VM.RunCommand로 연결된다. 새 WorkbenchBody_SizeChanged는 기존 SingleInput layout controller의 body 높이 갱신만 담당한다. Chrome의 완료 후 카드 표시와 Blob/Contour legacy guide 숨김은 presentation이며 parameter writer는 옮기지 않는다.
matching check는 result_summary/test_footer Wide·Compact와 기존 production delegate 24검사다. 완료 owner 등록은 아니다.

AUTO는 기존 Workbench VM.IsAutoRunEnabled/SystemStateConfig.WorkbenchAutoRunEnabled의 사용자 설정 writer를 재사용한다. 읽기 경로는 NativeDoc.ActivateWorkbenchEditor → Tool VM/Factory의 Matching AUTO_PREVIEW getter →
기존 결과 template의 switch와 Scheduler.AllowAutoPreview inherited gate → 원래 parameter event/90·120ms scheduler →
기존 NativeDoc.CaptureWorkbenchResultStep의 pending 확정/createStep → 작업본 동기화/result snapshot → 동일 preview/result 경로다. ON/OFF 전환·선택/복원/새 이미지 자체는 실행하지 않으며 pending 취소/Dispose는 기존 Scheduler·Document/View lifetime owner다.

프리셋은 기존 PresetButtonPresenter/Shell density와 SingleInput/Matching runtime을 재사용한다. 읽기 순서는 LineToolWpfView constructor → PresetButtonPresenter → LinePreviewController.ApplyPreset →
LineInteractionController.ApplyPreset → PG CommitPendingEdit/Refresh → PersistProperties/NativeDoc RecordParameterChange
→ Tool/Pipeline VM이다. View의 private ApplyPreset은 제거했다. property writer는 기존 Line VM/property,
작업본 writer는 Tool/Pipeline VM, scheduler/overlay와 Dispose는 기존 PreviewController가 소유한다. WorkbenchStepActions DataContext는 NativeDoc/Chrome가 주입하고 presenter의 DataContextChanged/dock/click
이벤트는 같은 IDisposable owner가 해제한다.

프리셋 적용 메시지는 PresetButtonPresenter.ApplyPresetAndRefresh → 기존 ApplyPreset →
Tool VM.NotifyPresetApplied → WorkbenchActionStatus binding 순서다. 표시 이름의 key writer는 Tool VM이며
실행 중 click gate는 기존 Presenter가 확인한다. 기존 preset 값/PG 양식/route/lifetime을 유지한다.

라인 상단은 NativeCustomToolFactory → NativeCustomToolDocumentBuilder(view.DataContext=LineToolPresenter)
→ LineToolWpfView → 기존 LineInteraction/ReviewController → LineToolTextPresenter 경로다. 상단 선택/ROI는 기존 Interaction/NativeDoc ROI command, property writer는 LineToolPresenter/property,
작업본 writer는 Tool/Pipeline VM이다. WorkbenchStepActions에는 NativeDoc/Chrome가 기존 Tool VM을 주입한다. View.DisposeToolResources가 기존 controllers/scheduler를 해제하며 TextPresenter에는 새 이벤트가 없다. SingleInput shell의 SizeChanged → 기존 docked layout controller가 floating host padding/border를 포함해
실제 PG380px을 확보한다. PG 양식/순서/editor와 docked160px을 유지하는 presentation 계산이다. 옛 기본 번역 갱신/저장은 기존 OpenVisionLanguageService의 EnsureCatalogFile/DefaultCatalogMigrations가 소유한다. 읽기 순서는 위 경로와 WorkbenchToolWorkingCopySmoke의 Line header/template floating 검사다. 기존 테스트 owner는 Program.cs의 CaptureShellHostLineMeasureTool/LineIntersectionTool과
AssertDockedSingleInputToolLayout이다. 명시 Main/Line_1·SDK managed 근거/current 카드·저장 단계
Revert/RunUntil 경로를 같은 fixture에서 검사하며 production owner는 그대로다.

템플릿 카드는 Tool VM command → NativeDoc.ChangeTemplate → 기존 PropertyGridEditorFactory.SelectTemplate
→ ImageEditorService/EditView → Matching-family VM.ApplyTemplatePath → 기존 presenter refresh → Tool/Pipeline 작업본 순서다. DataContext는 NativeDoc EditorBinding/Chrome가 기존 Tool VM을 주입한다. property·Mat writer/release는 기존 툴 owner,
임시 source/EditView 해제와 ROI asset 저장은 기존 Factory/editor, editor/session guard는 NativeDoc가 소유한다. 카드는 기존 Mat에서 복사한 frozen ImageSource를 읽으며 새 owner/interface/partial·View event는 없다.

파라미터 안내는 기존 `UI/VisionTest/Wpf/Tooling/PropertyGuide` owner를 재사용한다. 읽기 경로는 `NativeToolDocument.ActivateWorkbenchEditor` → SingleInput/DoubleInput shell의
`ToggleParameterGuide` → `VisionToolParameterGuideSidecarController` → 기존 binder/PropertyGridHost →
Presenter/Catalog/View다. 선택 snapshot·표시 내용은 Presenter/Catalog, 창의 생성/해제는 기존 shell/Sidecar,
custom binder 해제는 Tool View Dispose가 소유한다. Guide에는 별도 DataContext를 추가하지 않고 기존
명시 content/focus 결과 계약과 template의 TemplatedParent binding을 유지한다. 이미지 연산과
HSV/히스토그램은 기존 settings를 공급한다.

제목 언어 필드의 읽기 경로는 `rg "WorkbenchLanguage|LanguageComboBox" src/OpenVisionLab/UI`로 찾는다.
`Windows/OpenVisionShellHostWindow.xaml`의 Header Grid는 Window에 주입한 ShellHost.Workbench를
상속하며 `Shell.LanguageOptions`/`SelectedLanguageOption` TwoWay를 사용한다. 표시 owner는 기존
`UI/Wpf/OpenVisionLabWpfTheme.xaml`의 LanguageComboBox style/template이고 상태·언어 저장 writer는
기존 `Shell/Commands/OpenVisionShellCommandCatalog.cs`의 OpenVisionShellPreviewViewModel과 언어 서비스다.
`OpenVisionLab.Mvvm/Behaviors/ComboBoxInteractionBehavior`는 field press의 Focus/IsDropDownOpen/Handled만
담당한다. View code-behind event 변경은 없고 기존 Window.OnClosed→ShellHost.Dispose·VM 언어 이벤트
unsubscribe가 수명을 소유한다. 기존 tool template owner/bytes는 유지한다. 대응 검사는 기존 smoke의
`CaptureWorkbenchLayout`이며 현재 빌드/실제 입력/미검증 상태는
checkpoint19를 따른다..

## Simple parameter choice의 현재 owner

`SimplePreprocessParameterController.AddChoice` → 기존 `OpenVisionLab.Mvvm/Behaviors/ComboBoxInteractionBehavior` → `OpenVisionNativeSimplePreprocessViewConfigurator` → `OpenVisionNativeSimplePreprocessDocumentFactory` 순으로 읽는다. Simple View는 기존 SingleInput ViewModel을 상속하며 새 DataContext를 만들지 않는다. 선택의 mutable-state writer는 기존 controller의 SelectionChanged → ParameterChangeController → NativeDoc working copy다. View/NativeDoc의 생성·Dispose 및 guide binder/preview scheduler의 release 경로를 유지한다. 기존 Behavior가 자신의 ComboBox에서 Focus/IsDropDownOpen/Handled를 처리하도록 연결했으며 business state/정책/저장을 이전하지 않았다. 대응 검사는 smoke `CaptureWorkbenchSimpleChoiceInteraction`.

공유 choice 표시의 읽기 경로는 `VisionToolWpfTheme.xaml`의 implicit ComboBoxItem317/ComboBox357 → template/trigger → 기존 semantic `OpenVision.Workbench.*Brush` DynamicResource다.

## Before changing structure

1. owning feature/module, current caller, matching test를 찾습니다.
2. mutable-state writer와 create/release owner를 기록합니다.
3. 기존 concrete owner 재사용, 최소 in-place 변경, 새 owner가 실제 대안인지 비교합니다.
4. 변경 후 intended call path가 실제 사용되는지와 old coupling 제거를 검색합니다.
5. focused build/test와 미검증 runtime 범위를 completion record에 남깁니다.

## Search shortcuts

```powershell
rg -n "class <Type>|<Method>\(" src tools
rg -n "DataContext|Command=|Binding " src -g "*.xaml" -g "*.cs"
rg -n "ProjectReference" src tools -g "*.csproj"
rg -n "Dispose\(|event |\+=|-=|Timer" src -g "*.cs"
```

### Main ROI·Template 직접 편집의 UI host

Main 전용 교체 seam은 `PropertyGridEditorFactory.ShowImageEdit`와 `BeginImageEditWriteScope`부터 읽는다. RootWindow가 `InlineImageEditHost`를 생성해 attached host를 주입하고 PreviewKeyDown/Closed를 연결한다. NativeDoc의 기존 EditAction/context와 caller Window.Owner 체인을 함께 확인하므로 docked/Main 소유 floating caller만 선택되며 standalone은 기존 ShowDialog를 유지한다.

`InlineImageEditHost`는 UI workflow와 임시 UserControl View의 배치·focus·키·부모 복원을 소유한다. 새 View의 DataContext는 순수 `InlineImageEditViewModel`이며 기존 rehosted viewport/field는 각 editor VM을 유지한다. `IInlineImageEditPresentation`의 두 구현은 기존 `RoiEditorWindow`/`OpenGlTemplateEditorWindow`다. 기존 geometry/rotation/preview·result와 factory/NativeDoc 값·asset writer를 바꾸지 않았다. Main/ Pipeline의 transient busy 상태만 기존 owner가 쓴다.

caller의 using이 editor와 source clone/Mat을 해제한다. host finally가 parent/DataContext·visibility/focus·enabled 상태를 복원하고 resize·좌표·collection·caller Closed 구독을 해제한다. Root Closed는 host를 Dispose한다. 동기 AUTO scope는 최종 writer까지 유지되고 현재 scope/context가 자신이 소유한 경우만 원래 binding을 복원한다. 크게 편집은 별도 기존 modal을 using으로 해제한다. 대응 검사는 `CaptureWorkbenchInlineImageEdit`.
