# OpenVisionLab 2D Partial 구조 제거 계획

작성일: 2026-09-13 KST  
작업 원장: `PL-0028`  
대상: 현재 `C:\Git\2D\Dev`의 59개 컴파일 Partial 선언

## 사용자 문제

처음 코드를 읽는 개발자가 Partial 파일을 따라가도 실제 상태 소유자, 호출자,
Dispose 책임, XAML 계약을 바로 찾기 어렵다. 단순히 파일을 합치거나 삭제하면
WPF generated code, WinForms designer, native lifetime, binding 계약을 깨뜨릴 수
있으므로 책임을 concrete owner로 옮길 수 있는 경우만 변경한다.

## 이번 범위

### 지금 진행

1. 59개 선언을 최신 caller, mutable-state writer, lifetime/release owner,
   binding/public/test contract 기준으로 다시 대조한다.
2. 수동 Partial이 독립 책임을 숨기고 있으면 기존 ViewModel, Presenter,
   Controller, adapter, service 또는 test owner로 이동한다.
3. 이전 Partial에서 이동한 책임이 제거되었는지 source search와 focused
   contract로 확인한다.
4. 한 heartbeat 실행마다 하나의 독립 경계만 수정하고 Build/Test 후 다음
   경계로 이동한다.

### 별도 기록만 하는 범위

- XAML generated/InitializeComponent 계약
- WinForms designer와 SharpGL/native context 수명 계약
- 이미 독립 owner가 있는 Shell composition, Pipeline Review projection,
  Learn presenter, ImageCanvas native host
- 제품 코드가 아닌 smoke/test host

이 항목들은 Partial 키워드를 남기는 것이 목표 실패가 아니다. 제거 시 깨지는
계약과 현재 owner를 기록하고, 실제 업무 정책이 남아 있지 않은지 확인한다.

### 하지 않음

- 59개를 줄 수 기준으로 기계 병합·삭제
- 새 `Manager`/`Provider`/`Factory`/`Interface`/forwarding wrapper 추가
- XAML 이름, 공개 생성자, Recipe/XML, Preview/Run, PropertyGrid, image/native
  Dispose 계약 변경
- WPF runtime, camera, GPU, SDK, 장시간 native 검증을 실행한 것처럼 기록

## 작업 순서

| 단계 | 결과 | 완료 조건 |
| --- | --- | --- |
| M1 | 59행 owner map 재검증 | 각 행에 current/intended owner, caller, state writer, lifetime, contract가 있음 |
| M2 | 첫 실제 Partial 책임 이동 | 기존 concrete owner로 호출 경로가 바뀌고 이전 Partial의 책임이 사라짐 |
| M3 | 잔여 removable 경계 반복 처리 | 각 slice마다 focused contract, Build, forbidden search, diff check 통과 |
| M4 | 최종 구조 감사 | Partial count, retained reason, 문서 지도, Debug/Release, 원장 검증 통과 |

## 현재 진행 상태

- M1 `VERIFIED`: 현재 source scan은 59개 compiled Partial과 2개 literal
  smoke-contract match를 확인했고, 기존 59행 owner review를 새 작업 증거에
  연결했다.
- M2 `VERIFIED`: `OpenVisionLearnWindow`의 Color/HSV animation state, HSV
  range 계산, guide 문구와 mask 판단을 `ColorHsvLearnPresenter`로 이동했다.
  Window는 XAML event, DispatcherTimer, Brush와 control projection을 유지한다.
- M3 `VERIFIED`: 여섯 개의 removable 경계를 조사했고, 다섯 개의 삭제가 유지되었다. `OpenVisionRecipeBasicLifecycleView.xaml.cs`와
  `OpenVisionRecipeValidationSuiteView.xaml.cs`는 각각 `InitializeComponent()`만 가진
  수동 code-behind로 확인되어 삭제했다. `OpenVisionWorkspaceSamplePickerView.xaml.cs`는
  같은 모양이었지만 실제 WPF smoke에서 자식 UserControl 본문이 비어 있음을 재현했고,
  `InitializeComponent()` 생성자를 복원했다. 이 경계는 이름만 남은 파일이 아니라 XAML
  생성물 초기화 계약이었다. 현재 source scan은 54개 compiled Partial을 보고하며, Sample
  Picker Window/ViewModel command-result contract와 Readiness 계약도 통과했다.
  Solution Debug/Release도 통과했으며, 마지막 Image Compare 경계까지 caller/
  lifetime/contract를 확인한 결과 추가 extraction seam은 없었다. `OpenVisionShellHostWindow.xaml.cs`는 Shell View 생성,
  HWND hook, responsive scale, smoke projection, 종료 Dispose를 함께 소유하는 WPF composition
  root로 확인되어 강제 분리하지 않고 retained reason을 기록했다. 이어서
  `OpenVisionPendingToolView.xaml.cs`는 기존 ViewModel 주입/DataContext 어댑터이며
  ViewModel과 DocumentController가 상태·Dispose를 소유하므로 retained reason을 기록했다.
  `OpenVisionLayerDockWorkspaceView.xaml.cs`는 AvalonDock DP, event bridge,
  guide/geometry, visual snapshot을 소유하는 concrete visual adapter로 확인되어
  동일한 WPF control state를 감싸는 새 owner 없이 retained reason을 기록했다.
  이어서 `OpenVisionLayerDockingGuideOverlayView.xaml.cs`는 XAML namescope의
  docking guide 시각 요소와 active zone·margin 투영만 소유하는 concrete visual
  adapter로 확인되어 retained reason을 기록했다.
  `ImageCanvasControl.cs`와 `ImageCanvasControl.designer.cs`는 SharpGL
  `OpenGLControl`의 designer 생성 필드와 본체의 rendering·event·P/Invoke·native
  cleanup이 하나의 수명 경계를 공유하므로 retained reason을 기록했다.
  `AddRoiArrayView.xaml.cs`와 `AddRoiArrayView.xaml`은 프로젝트에서 제외된
  중복 파일이고, 동일한 공개 타입을 제공하는 compiled compatibility owner가
  이미 호출 경로를 담당하므로 제거했다.
  `AutoAlignTeachingView.xaml.cs`, 대응 XAML과 제외된 ViewModel은 저장소
  호출자가 없는 legacy feature로 확인되어 stale project exclusions와 함께
  제거했다.
  `AutoWarpageTeachingView.xaml.cs`, 대응 XAML과 제외된 ViewModel도 동일하게
  호출자가 없는 legacy feature로 확인되어 stale project exclusions와 함께
  제거했다. 이어서 `RoiImageCanvasView.xaml.cs`는 `WindowsFormsHost` namescope,
  DataContext attach/detach, dialog/context-menu host 연결, WPF key event 전달과
  view-owned native host 정리를 소유하는 concrete WPF adapter로 확인되어
  retained reason을 기록했다. ViewModel은 ROI/mode/snapshot state, current Mat,
  ImageCanvasControl, refresh timer와 Dispose를 계속 소유한다.
  `LogPanelView.xaml.cs`는 LogPanelViewModel, LogPanelFileAccess와
  RuntimeLogStream이 이미 상태·파일·버퍼 정책을 소유하고 있고, View에는
  WPF compact layout·ScrollIntoView와 event cleanup만 남아 있어 cohesive
  adapter로 retained reason을 기록했다.
  `Settings.Designer.cs`는 Visual Studio SettingsSingleFileGenerator가 만든
  `ApplicationSettingsBase` Partial이며 `Settings.settings`의 70개 항목과
  generated default/serialization property 계약을 그대로 보존해야 하므로
  retained reason을 기록했다. 현재 source caller는 없지만 수동 wrapper로
  대체하거나 삭제하는 settings migration은 이번 범위가 아니다.
  `OpenVisionTcpIntegrationWindow.xaml.cs`는 PasswordBox·close guard·XAML
  event cleanup만 담당하는 WPF Window adapter이며, TCP exchange·설정 저장·
  검증·취소·Dispose는 기존 `OpenVisionTcpIntegrationController`가 소유하므로
  retained reason을 기록했다.
  `VisionToolNImageVerificationWindow.xaml.cs`는 XAML namescope, 두 이미지
  surface의 zoom controller, 선택 이미지 변경 시 시각 상태 reset, Closed
  cleanup만 담당하는 WPF Window adapter로 확인했다. 이미지 선택·파일 대화상자·
  검증 실행·취소·결과·export·승격과 내부 리소스 Dispose는 기존
  `VisionToolNImageVerificationController`가 소유하고,
  `OpenVisionNativeToolDocument`가 controller/window를 생성해 owner를 설정한
  뒤 `ShowDialog()`를 호출한다. 따라서 독립 workflow owner 없이 partial을
  합치면 XAML/visual lifetime 경계를 숨기게 되므로 retained reason을 기록했다.
  `OpenVisionShellHostView.xaml.cs`는 Shell의 XAML composition root로 확인했다.
  Window가 생성·소유하고 종료 시 `Dispose`를 호출하며, View는 기존
  session/recipe/layer/tool/workspace controller와 presenter를 의존성 순서대로
  연결하고 XAML command surface·visual event·callback cleanup을 투영한다.
  View 내부에는 직접적인 파일 I/O·Pipeline 실행/storage·OpenCvSharp·network
  정책이 없고, 실제 mutable state와 native/session release는 기존 concrete
  owner가 담당하므로 새 manager나 수동 Partial로 나누지 않고 retained reason을
  기록했다. 이어서 `OpenVisionPipelineReviewView.xaml.cs`는 Shell의 Pipeline
  Review 문서가 생성·소유하는 XAML visual adapter로 확인했다. ViewModel은
  binding 가능한 review text/readiness와 preview projection을, 기존
  `OpenVisionPipelineReviewLayoutController`는 compact/details/step-flow layout
  상태를, `OpenVisionPipelineReviewExecutionController`는 run identity·취소·
  revision callback·Bitmap cache 수명을, `OpenVisionPipelineReviewImageResourceOwner`
  는 View가 보유한 Bitmap/diagnostic 리소스를 각각 소유한다. View에 남은
  selection/highlight와 control event 메서드는 XAML namescope와 WPF control
  상태를 직접 변경하는 UI adapter 책임이며 독립 수명·테스트 경계가 없어
  새 wrapper/partial로 분리하지 않았다. source contract 13/13과
  `wpf_shell_host_pipeline_review` smoke가 통과했고 OpenVisionLab Debug/Release,
  PipelineViewerScreenshotSmoke Debug 빌드도 경고·오류 0으로 통과했다.
  이어서 `OpenVisionFloatingToolWindow.xaml.cs`는 WPF Window의 XAML
  namescope, title/icon/content host, airspace activation, DockRequested
  bridge와 local event cleanup만 소유하는 것으로 확인했다. 기존
  `OpenVisionFloatingToolWindowHost`가 create/reuse/placement/close/dock routing을,
  `OpenVisionLayerViewerWindowRegistry`와 각 feature controller가 hosted
  content의 Dispose를 소유하므로 Window partial을 새 wrapper나 partial로
  분리하지 않았다. source contract 14/14와
  `wpf_tool_window_dock_float_cycle` smoke가 통과했고 OpenVisionLab
  Debug/Release 및 PipelineViewerScreenshotSmoke Debug 빌드도 경고·오류
  0으로 통과했다.
- M4 `PENDING`: 모든 slice가 종료되었으므로 Partial count와 잔류 이유를 최종 감사한다.

첫 slice의 source/build/contract 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913`
아래 `m1-partial-revalidation`과 `m2-color-hsv`에 있다.
이번 M3 slice의 증거는 같은 경로 아래 `m3-basic-lifecycle`, `m4-validation-suite`,
`m5-workspace-sample-picker`, `m6-shell-host-window-retention`에 있다.
`m7-pending-tool-retention`에는 Pending Tool의 caller/state/lifetime 계약과 유지 근거가 있다.
`m8-docking-workspace-retention`에는 Docking workspace의 visual contract와 유지 근거가 있다.
`m9-docking-guide-overlay-retention`에는 Docking guide overlay의 XAML visual contract와 유지 근거가 있다.
`m10-image-canvas-native-retention`에는 ImageCanvas SharpGL/WinForms native contract와 유지 근거가 있다.
`m11-add-roi-array-dead-partial-removal`에는 제외된 AddRoiArrayView 중복 Partial 제거와 compatibility 호출 경로 보존 근거가 있다.
`m12-auto-align-dead-feature-removal`에는 호출자가 없는 AutoAlign legacy feature 제거와 project 계약 보존 근거가 있다.
`m13-auto-warpage-dead-feature-removal`에는 호출자가 없는 AutoWarpage legacy feature 제거와 project 계약 보존 근거가 있다.
`m14-roi-image-canvas-view-retention`에는 RoiImageCanvasView의 WPF/WindowsFormsHost
adapter owner, caller/state/lifetime 계약과 유지 근거가 있다.
`m15-log-panel-view-retention`에는 LogPanelView의 WPF adapter owner,
LogPanelViewModel/LogPanelFileAccess/RuntimeLogStream 호출·수명 계약과 유지 근거가 있다.
`m16-settings-designer-retention`에는 generated Settings Partial의 generator,
70개 property/default/serialization 계약과 유지 근거가 있다.
`m17-tcp-integration-window-retention`에는 TCP Integration Window의 XAML,
controller call path, close/event cleanup과 async disposal 계약의 유지 근거가 있다.
`m18-n-image-verification-window-retention`에는 N-image Verification Window의
XAML/zoom/lifetime adapter, controller/document call path, source contract와
실제 WPF smoke 계약의 유지 근거가 있다.
`m19-shell-host-view-retention`에는 Shell Host View의 XAML composition root,
Window 생성·Dispose call path, 기존 concrete owner 연결, Readiness와 실제 Shell
smoke 계약의 유지 근거가 있다.
`OpenVisionRecipePendingEditDialog.xaml.cs`는 XAML `Window` partial을
유지하되, 파일 안에 섞여 있던 독립적인 비-WPF
`OpenVisionRecipePendingEditDialogViewModel`을
`OpenVisionRecipePendingEditDialogViewModel.cs`로 이동했다. Window에는
`InitializeComponent`, DataContext 연결, `Decision`/`DialogResult`, Escape와
버튼 이벤트만 남겼고, 표시 문자열·현재/대상 문맥 투영은 별도 concrete
ViewModel이 소유한다. `RecipeDialogAdapter`가 Owner/`ShowDialog`/결과를
소유하고 `OpenVisionRecipePendingEditTransitionController`가 적용·폐기·취소
정책을 유지하므로 동작 계약은 바뀌지 않았다. Partial 선언 수는 53개로
유지되며, 이번 slice의 구조 개선은 XAML partial을 삭제하는 대신 MVVM
책임을 별도 파일과 owner로 명시한 것이다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m20-pending-edit-dialog-viewmodel-extraction`에 있다.
`OpenVisionRecipeRunEvidenceViewerView.xaml.cs`도 재확인했다. 이전 P1 변경으로
파일 기반 `Bitmap` 디코드는 기존 concrete
`OpenVisionBitmapImagePreviewFactory.LoadBitmap`이 소유하고, View에는
evidence 선택·상태/오류 투영·두 viewer의 visual lifetime만 남아 있다.
`OpenVisionLayerViewerView`는 입력 Bitmap을 clone한 뒤 자신의 이전 이미지를
해제하고, `OpenVisionLayerViewerWindowRegistry`는 floating window가 닫힐 때
hosted View를 Dispose한다. 따라서 새 ViewModel이나 wrapper를 추가하지 않고
필수 XAML partial을 retained boundary로 기록했다. 이미지 경계 contract는
통과했으며, dataset root가 설정되지 않아 drawing-evidence WPF smoke는
실행 전 NG로 기록했다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m21-evidence-viewer-retention`에 있다.
`OpenVisionLayerViewerView.xaml.cs`도 재확인했다. 이 View는 docked layer,
floating layer, tool preview, evidence viewer가 공유하는 concrete visual/native
adapter이며, 입력 Bitmap을 clone해 `ownedLayerImage`로 소유하고 이전 이미지를
해제한다. `OpenVisionBitmapCanvasPresenter`는 해당 View 소유 이미지를 OpenGL
업로드·저장에 사용하고 Dispose 시 참조를 끊으며, View가 canvas presenter,
fallback zoom, `RoiImageCanvasView`, 언어/Loaded 이벤트와 pending refresh의
수명을 함께 정리한다. Dock workspace controller와 floating window registry는
문서/창이 닫힐 때 hosted View를 Dispose한다. 따라서 같은 control/native 상태를
새 ViewModel이나 wrapper로 나누지 않고 required XAML partial을 retained
boundary로 기록했다. `wpf_shell_host_layer_popout` smoke와 source/build
계약은 통과했다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m22-layer-viewer-retention`에 있다.
`OpenVisionShellPreviewView.xaml.cs`도 재확인했다. 이 파일은 standalone
`wpf_shell_preview` surface의 XAML/DataContext adapter이며,
`OpenVisionShellPreviewViewModel.CreatePreview()`로 기존 concrete ViewModel을
생성하고 `Unloaded`에서 Dispose하는 수명 bridge만 가진다. navigation group,
selected tool/layer, readiness, localization, command와 language subscription은
ViewModel이 소유하고, production `OpenVisionShellHostView`도 같은 ViewModel을
직접 소유한다. View에는 image/native/file/inspection 정책이 없으므로 별도
owner나 wrapper를 만들지 않고 required XAML partial을 retained boundary로
기록했다. `wpf_shell_preview` smoke와 source/build 계약은 통과했다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m23-shell-preview-retention`에 있다.
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m24-pipeline-review-retention`에는
Pipeline Review View의 Document/VM/Layout/Execution/Image owner 호출 경로와
XAML contract, smoke/build 검증이 있다.
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m25-floating-tool-window-retention`에는
Floating Tool Window의 XAML/chrome adapter와 host/registry/feature caller의
content 수명 경계, smoke/build 검증이 있다.
`OpenVisionStartupLoadingWindow.xaml.cs`도 재확인했다. 이 Window partial은
XAML namescope의 title/detail 투영, localization lookup, `ShowReady`의
render pump, `Complete`의 닫힘 허용 상태와 `OnClosing`의 조기 종료 차단만
소유한다. `OpenVisionLabApplication.Run`이 실제 생성·표시·Shell 준비 완료 후
완료·Dispatcher 종료를 소유하고, `OpenVisionLabDirectSmokeRunner`는
`startup-loading-feedback` 시나리오에서 같은 호출 경로와 한국어/영어 표시,
완료 전 사용자 닫힘 차단을 검증한다. Window에는 Recipe, Pipeline, 검사,
파일/네이티브 리소스 정책 또는 독립 DataContext 상태가 없으므로 concrete
owner로 이동할 책임이 입증되지 않았고 required XAML partial을 유지했다.
source contract 12/12, startup-loading-feedback smoke가 fresh screenshot과
monitor intersection을 포함해 통과했으며, embedded Debug/Release와
PipelineViewerScreenshotSmoke Debug build도 오류 없이 완료했다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m26-startup-loading-window-retention`에 있다.

M3 remains in progress. The next action is to inspect
`src/OpenVisionLab/UI/Menu/Wpf/Windows/OpenVisionWindowTitleBar.xaml.cs` after
confirming its caller, mutable-state writer, lifetime/release owner, and
public/XAML/test contract. Reopen it only if a reproducible responsibility
conflict or changed dependency boundary is found.

`OpenVisionWindowTitleBar.xaml.cs`도 재확인했다. 이 UserControl partial은
공유 XAML namescope, 제목·아이콘·Dock 표시 투영, localization tooltip과
AutomationProperties, Window의 drag/minimize/maximize/close framework 동작 및
`DockRequested` bridge만 담당한다. Shell Host, Floating Tool Window, Sample
Picker Window가 제목·아이콘과 부모 Window 수명을 주입·소유하며, TitleBar에는
Recipe/Pipeline/검사/파일/native/DataContext 정책이 없다. 따라서 MVVM View의
presentation/framework plumbing 경계를 유지하고 새 ViewModel·wrapper·partial을
추가하지 않았다. source contract 14/14, `wpf_shell_host_window_chrome`와
`wpf_shell_host_window_maximized` smoke가 fresh screenshot 및 automation ID/
work-area 검증과 함께 통과했고 OpenVisionLab Debug/Release 및
PipelineViewerScreenshotSmoke Debug build도 오류 없이 완료했다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m27-window-title-bar-retention`에 있다.

M3 remains in progress. The next action is to inspect
`src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs`
after confirming its caller, mutable-state writer, lifetime/release owner, and
public/XAML/test contract. Reopen it only if a reproducible responsibility
conflict or changed dependency boundary is found.

`OpenVisionWorkspaceSamplePickerWindow.xaml.cs`도 재확인했다. Window는
`OpenVisionWorkspaceSamplePickerViewModel`을 DataContext로 주입하고, owner/modal
`ShowDialog()` 경계와 제목 표시, Cancel/`DialogResult`, HWND work-area hook의
수명을 담당한다. 샘플 목록·필터·선택 가능 여부·Learn 문서 정책은 기존 ViewModel과
`OpenVisionWorkspaceLearnDocumentService`가 소유한다. 기존 Window의 직접적인
`OpenLearnDocumentForSelection()` 호출은 ViewModel의
`OpenLearnAndSelectCommand`로 이동했고, ViewModel은 `SelectionAccepted`라는 명시적
결과 계약만 발행하며 Window가 이를 `DialogResult`로 변환한다. 새 Manager/Service/
wrapper는 만들지 않았다. 자식 `OpenVisionWorkspaceSamplePickerView.xaml.cs`는
삭제 후 smoke에서 본문이 비는 결함이 재현되어 `InitializeComponent()` 생성자를
복원했다. source contract 14/14, `wpf_shell_host_workspace_sample_picker` 및
`wpf_shell_host_workspace_sample_picker_maximized` smoke가 fresh screenshot,
automation ID, work-area 검사와 함께 통과했고 OpenVisionLab Debug 및
PipelineViewerScreenshotSmoke Debug build도 완료했다. 생성 산출물 정리 후
OpenVisionLab Release build도 0 warning/error로 완료했다. 증거는
원래 증거 경로는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m28-workspace-sample-picker-window-retention`였으며, 아래 정리로 raw 파일은 삭제했다.

`src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs`도 재확인했다.
현재 caller는 standalone `OpenVisionLab.ImageCompare` EXE와
`PipelineViewerScreenshotSmoke`의 `wpf_image_compare` target이다. Window는
required XAML `partial`, public `LoadImages` facade, `OpenFileDialog`, pointer/
window chrome 이벤트와 ViewModel Dispose만 소유한다. 선택 디렉터리 저장/복원은
`ImageCompareViewModel`이 소유한 `ImageCompareDirectoryPolicy`가, decoded
`Bitmap`/frozen `BitmapSource` replacement 수명은
`ImageCompareImageResource`와 `ImageCompareSlotViewModel`이 소유한다.
따라서 독립 state/lifetime/test owner로 이동할 책임이나 제거 가능한 manual
Partial이 입증되지 않았고, 새 wrapper/ViewModel/partial을 만들지 않았다.
`ImageCompareDirectoryPolicyContract`, `ImageCompareResourceContract`,
standalone ImageCompare Debug build, 그리고 `wpf_image_compare` UI smoke가
통과했다. UI smoke는 현재 off-screen capture 경로이므로 source review와
focused runtime evidence는 확보했지만, monitor-visible EXE 및 전체 theme/DPI/
input matrix는 여전히 미검증이다. 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structural-elimination-20260913\m29-image-compare-retention`에 있다.

M3의 removable/retained 경계 처리는 이 Image Compare 재확인으로 종료하고,
다음 heartbeat는 M4 최종 Partial count·owner map·문서·Debug/Release·관련
contract 감사만 수행한다. 파일 크기만으로 `OpenGlTemplateEditorWindow`,
`RoiEditorWindow`, Tool View, Learn View를 다시 나누지 않는다.

2026-09-13 저장 공간 정리를 위해 위 D: 검증 산출물과 현재 Dev의 생성
`bin`/`obj`/`.vs`/`build`/`dist`/`tmp`/`temp`, 그리고
`D:\OpenVisionLab_Data\Dev\artifacts`를 삭제했다. 소스·문서·샘플·사용자
데이터는 삭제하지 않았다. 아래 결과 기록은 저장소 문서와 원장에 남기고 raw
스크린샷·로그·빌드 산출물은 보존하지 않는다.

## 스케줄 규칙

- PL-0029가 등록한 `openvisionlab-2d` heartbeat 하나만 사용했다
  (10분 간격). PL-0028의 책임은 그대로 유지했고 별도 Partial 예약은
  만들지 않았다. M4 최종 감사 후 이 heartbeat는 중지했다.
- 매 실행 시작 시 `PL-0028`, 현재 automation, working tree, current handoff,
  실제 caller와 matching test를 확인한다.
- 같은 책임이 이미 `doing` 또는 `resolved`이면 재실행하지 않는다.
- 사용자 입력·장비·UI runtime을 기다리지 않고, 가능한 source/build 계약부터
  진행한다.
- 실제 runtime 검증이 필요한 경계는 `미검증`으로 남기고 다음 독립 경계를
  계속 검토한다.
- 한 단계가 검증되면 automation prompt와 원장의 `next_action`을 다음
  미완료 경계로 갱신한다.

## 완료 기준

- 제거 가능한 수동 Partial은 concrete owner로 이동되어 이전 owner에 같은
  책임이 남아 있지 않다.
- 남은 Partial마다 생성/XAML/designer/native/test/cohesive 이유와 owner가
  문서화되어 있다.
- Solution Debug/Release와 영향 범위 focused contract가 실제로 통과한다.
- 문서 index, owner map, issue ledger, diff check가 통과한다.
- WPF visual/theme/DPI/monitor/input, camera/SDK/GPU, 장시간 native 범위는
  실제 실행 여부를 분리해 기록한다.

## 최종 M4 감사 결과 (2026-09-13)

M4는 `D:/OpenVisionLab-TestData/OpenVisionLab_Dev/partial-structural-elimination-20260913/m4-final-audit/phase-summary.txt`
의 실행 증거로 완료했다. 현재 source audit은 컴파일된 Partial 54개와 literal
smoke-contract match 0개를 보고하며, dead/중복 행 제거와 Sample Picker child
`InitializeComponent()` 계약 복원으로 최초 59행 baseline을 현재 소스와 정합화했다.
각 retained row는 current/intended owner, caller path, mutable-state writer,
lifetime/release owner, binding/public/test contract 또는 retained reason을 가진다.

Solution Debug/Release, VisionRecipeRunnerSmoke Debug/Release,
OpenVisionReadinessCheck Debug/Release, focused contracts, WPF screenshot smoke,
DocumentationIndex, proofline issue-ledger, RefactorAudit, `git diff --check`가 모두
통과했다. WPF smoke의 대표 대상은 Shell Preview, Window Chrome, Sample Picker,
Pipeline Review, Image Compare이며 전체 theme/DPI/input/monitor 및 hardware/GPU/
SDK/장시간 native 범위는 미검증으로 유지한다. 파일 크기만으로 남은 cohesive,
generated/XAML/designer/native/test Partial을 다시 나누지 않는다.

M4 이후 PL-0028/PL-0029를 닫고 현재 `openvisionlab-2d` 예약 실행을 중지한다.
새 예약은 만들지 않으며, 새 defect·요구사항·failed criterion·dependency boundary
가 생길 때만 별도 issue로 다시 연다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0032)

기존 PL-0028 M4 종료 이후 사용자가 Partial 구조 리팩터링을 계속하도록
요청했으므로, 별도 단일 executor인 `openvisionlab-2d-partial` heartbeat를
다시 활성화했다. 이 automation은 10분마다 하나의 독립 경계만 처리하며,
기존 `openvisionlab-2d`나 다른 Partial executor와 중복되지 않는다.

첫 후속 slice는 Image Compare retained Window Partial에서 확인했다. Window는
required XAML namescope와 file picker/pointer/window lifetime을 유지하고,
WPF 객체가 필요 없는 표시 좌표 매핑 계산만 기존
`ImageCompareViewModel.TryMapDisplayedPoint`로 이동했다. 기존 status binding,
`ImageCompareImageResource`/`ImageCompareSlotViewModel` resource lifetime,
`LoadImages` facade는 변경하지 않았다. 새 interface/service/wrapper/partial은
추가하지 않았다.

`ImageComparePointMappingContract`가 center/letterbox/edge clamp/invalid
dimensions 4/4를 Debug·Release에서 통과했고 Smoke build도 0 warning/error였다.
상세 owner/call-path/수명/검증 기록은
`docs/reports/OPENVISIONLAB_IMAGE_COMPARE_POINT_MAPPING_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0032.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-point-mapping-20260914`
에 있다. 나머지 XAML/generated/designer/native/cohesive Partial은 새 결함,
요구사항, failed criterion 또는 dependency boundary가 입증될 때만 다음
heartbeat에서 다시 연다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0033)

두 번째 후속 slice는 `RoiImageCanvasViewModel`의 WPF/native/path coupling을
재확인했다. ViewModel은 공유 `ImageCanvasControl`, ROI/input/timer 상태,
Mat와 `Dispose` 수명, 그리고 여러 consumer가 사용하는
`ImageViewer`/`LoadImage`/`SaveCurrentImage` facade를 함께 소유한다. 독립
state/lifetime/test seam이 확인되지 않았으므로 파일 길이만으로 ViewModel을
나누거나 새 Partial을 추가하지 않았다.

대신 순수 파일명 정책만 기존 `ImageCanvasDirectoryPolicy`로 이동했다.
`ResolveImageName`은 두 `LoadImage` 경로에서 `_currentImageName`을 만들고,
`CreateDefaultSaveFileName`은 `OnSaveIamge`에서 기존
`IImageCanvasDialogHost`로 전달할 기본 PNG 이름을 만든다. ViewModel의
Mat/image viewer/input/timer state, binding/public contract, resource lifetime은
변경하지 않았다. 상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_ROI_IMAGE_CANVAS_PATH_POLICY_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0033.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\roi-image-canvas-path-policy-20260914`
에 있다.

`RoiImageCanvasPathPolicyContract`와 기존
`RoiImageCanvasBoundaryContract`가 Debug·Release에서 각각 6/6을 통과했고,
Solution/Smoke Debug·Release build, Readiness, RefactorAudit,
DocumentationIndex, issue ledger, `git diff --check`도 통과했다. 전체 WPF
visual/theme/DPI/monitor/input, OpenGL, hardware/native runtime은 계속
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 다음 heartbeat는
완료된 ROI/Image Compare 경계를 재분리하지 않고, 아직 보호되지 않은 Tool
View/Learn Partial 가족에서 실제 owner seam이 있는지 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0034)

세 번째 후속 slice는 `OpenVisionLearnWindow.xaml.cs`의 concrete document
service coupling을 확인했다. Window는 required XAML Partial로서 topic list,
control projection, DispatcherTimer/animation, threshold/practice/tool callback
및 Window lifetime을 함께 유지해야 하므로 Partial 자체를 제거하지 않았다.

다만 `OpenLearnDocsButton_Click`와 `OpenFoundationDocsButton_Click`가
`OpenVisionWorkspaceLearnDocumentService`를 직접 호출하던 책임은 View의
presentation/framework plumbing보다 넓었다. Window는 이제
`SetOpenLearnDocumentAction(Action<string>)`으로 filename을 전달하고,
`OpenVisionShellHostLearnWindowController`, `VisionToolLearnWindowController`,
`ThresholdToolLearnWindowController`가 기존 service owner를 연결한다. XAML
namescope, automation ID, topic catalog mapping, threshold event와 기존
controller cleanup은 변경하지 않았다.

`LearnWindowDocumentBoundaryContract`가 Debug·Release에서 6/6을 통과했고,
단일 모니터(`\\.\DISPLAY2`, 1920x1080)에서
`wpf_openvision_learn_foundation_contract`와 `wpf_shell_host_learn_entry`
WPF smoke가 통과하여 fresh PNG를 남겼다. Solution/Smoke Debug·Release
build, Readiness, RefactorAudit, DocumentationIndex, issue ledger,
`git diff --check`도 통과했다. 외부 browser launch, 전체 topic/theme/DPI/input,
hardware/native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_LEARN_WINDOW_DOCUMENT_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0034.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\learn-window-document-boundary-20260914`
에 있다. 다음 heartbeat는 완료된 Learn/ROI/Image Compare 경계를 재분리하지
않고 다음 unprotected Tool View/Learn family를 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0035)

네 번째 후속 slice는 `VisionToolSignalInspectorView.xaml.cs`의 TSV
export/save-dialog coupling을 확인했다. Inspector는 current evidence와
plot/marker/XAML state, native `SaveFileDialog` presentation을 소유하지만,
기존 `VisionToolSignalEvidenceExporter`의 file-I/O를 직접 호출하는 책임까지
소유할 필요는 없었다.

따라서 새 abstraction을 만들지 않고 `SetExportAction(Action<VisionToolSignalEvidence,
string>)` seam을 추가했다. Threshold/Simple Preprocess/Line의 기존 Tool View
composition owner가 기존 exporter를 연결하며, UI button과 `ExportForTest`는
동일 `ExportEvidence` 경로를 사용한다. exporter의 TSV metadata/path/UTF-8
계약, evidence mutable writer, marker/public/test facade, XAML child lifetime은
변경하지 않았다.

`SignalInspectorExportBoundaryContract`가 6/6으로 Debug·Release에서 통과했고,
Solution/Smoke Debug·Release build, 단일 모니터 WPF smoke
(`wpf_threshold_signal_good_bad_replay`, `wpf_line_signal_profile`,
`wpf_shell_host_threshold_tool`, `wpf_shell_host_line_tool`), Readiness,
RefactorAudit, DocumentationIndex, issue ledger, `git diff --check`가 통과했다.
외부 native SaveFileDialog click/file association, 전체 theme/DPI/input,
hardware/native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_SIGNAL_INSPECTOR_EXPORT_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0035.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\signal-inspector-export-boundary-20260914`
에 있다. 다음 heartbeat는 완료된 Signal Inspector 경계를 재분리하지 않고
아직 보호되지 않은 Tool View/Learn View Partial 가족에서 독립 owner seam을
확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0036)

다섯 번째 후속 slice는 `MorphologyToolWpfView.xaml.cs`를 no-change 구조 감사로
확인했다. View Partial은 required XAML namescope와 composition adapter만
소유하고, `MorphologyToolPresenter`/`MorphologyToolViewModel`,
`VisionToolMorphologyInteractionController`, `VisionToolKernelSizeController`,
기존 single-input base가 각각 property facade, mutable/settings state,
operation/shape interaction, kernel input, preview/lifetime을 소유한다.

따라서 독립 state/lifetime/test seam이 새로 입증되지 않았고, 파일 길이만으로
새 Partial·wrapper·service를 추가하지 않았다. `MorphologyToolPartialBoundaryContract`
6/6 Debug·Release, Smoke build, 단일 모니터 WPF
(`wpf_filter_morphology_layout_guard`, `manual_morphology_tool_ui`)가 통과했다.
전체 theme/DPI/input, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_MORPHOLOGY_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0036.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\morphology-tool-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Morphology/Signal Inspector 경계를 재분리하지
않고 Edge Based Matching/Auto MPoint Tool View family처럼 아직 보호되지 않은
Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0037)

여섯 번째 후속 slice는 `EdgeBasedMatchingToolWpfView.xaml.cs`와
`AutoMPointTeachingPanel.xaml.cs`를 Edge Based Matching/Auto MPoint composition
family no-change 구조 감사로 확인했다. Edge View는 required XAML/composition
adapter, Panel은 presentation/localization View Partial이며,
`AutoMPointTeachingController`가 teaching mutable state와 event/dialog/algorithm
workflow를, `AutoMPointHtmlReportExporter`가 report file-I/O를, 기존 matching
controller/factory/composition/base가 property/preview/creation/lifetime을
각각 소유한다.

따라서 독립 state/lifetime/test seam이 새로 입증되지 않았고, mechanical split로
새 ViewModel·service·wrapper·Partial을 추가하지 않았다.
`EdgeBasedMatchingPartialBoundaryContract` 8/8 Debug·Release, Smoke build,
단일 모니터 WPF (`wpf_shell_host_edge_based_matching_tool`,
`wpf_shell_host_edge_based_matching_auto_mpoint`)가 통과했다. 전체
theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_EDGE_BASED_MATCHING_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0037.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\edge-based-mpoint-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Edge Based Matching/Auto MPoint, Morphology,
Signal Inspector 경계를 재분리하지 않고 Feature Matching Tool View composition
family처럼 아직 보호되지 않은 Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0038)

일곱 번째 후속 slice는 `FeatureMatchingToolWpfView.xaml.cs`와 XAML을
Feature Matching Tool View composition family no-change 구조 감사로 확인했다.
View는 required XAML/composition adapter이며,
`FeatureMatchingToolViewModel`은 mutable property/template policy를,
`VisionToolSingleInputMatchingToolController`와 shared matching runtime은
PropertyGrid/preview/result-review/preset/event/language/lifetime을,
factory/composition/base는 creation/release를 각각 소유한다.

따라서 독립 state/lifetime/test seam이 새로 입증되지 않았고, mechanical split로
새 ViewModel·service·wrapper·Partial을 추가하지 않았다.
`FeatureMatchingPartialBoundaryContract` 9/9 Debug·Release, Smoke build,
단일 모니터 WPF (`wpf_shell_host_feature_matching_tool`)가 통과했다. 전체
theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_FEATURE_MATCHING_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0038.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\feature-matching-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Feature Matching, Edge Based Matching/Auto
MPoint, Morphology, Signal Inspector 경계를 재분리하지 않고 아직 보호되지 않은
Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0039)

여덟 번째 후속 slice는 `MatchingToolWpfView.xaml.cs`의 sample property
projection 책임을 실제 caller와 owner까지 다시 추적했다. View Partial은
required XAML namescope와 matching controller orchestration을 유지해야 하지만,
`VisionPipelineAppToolFactory.ResolveTemplatePath` 호출과 common OpenCV/Matching
field copy callback까지 소유할 이유는 없었다.

새 abstraction을 만들지 않고, 이미 Matching pipeline property 생성/저장을
소유한 `VisionPipelineMatchingPropertyAdapter`에
`ResolveSampleTemplatePath`와 `ApplySampleProperty`를 추가했다. View는
`SetTemplatePathForTest`를 먼저 호출한 뒤 adapter callback을 연결한다.
`AUTO_PREVIEW=false`, 전체 field projection, `CvROIS`/`CvMASKS` 방어적 복사,
ViewModel mutable state, shared runtime/controller lifetime, XAML/test facade는
그대로 유지했다.

`MatchingToolPartialBoundaryContract`가 Debug·Release에서 각각 11/11을
통과했고, `VisionRecipeRunnerSmoke` Debug·Release build, `wpf_shell_host_matching_tool`
WPF smoke와 `wpf_shell_host_recipe_fixture_properties` sample-step smoke
(`check=OK`, `layout=0`, `text=0`, `internal=0`), 단일 모니터
window geometry probe가 통과했다. 전체 theme/DPI/input, native dialog,
camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_MATCHING_TOOL_PARTIAL_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0039.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-tool-partial-boundary-20260914`
에 있다. 다음 heartbeat는 완료된 Matching, Feature Matching, Edge Based Matching/
Auto MPoint, Morphology, Signal Inspector 경계를 재분리하지 않고 아직 보호되지
않은 Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0040)

아홉 번째 후속 slice는 `ThresholdToolWpfView.xaml.cs`의 teaching-suggestion
workflow 조정 책임을 실제 caller, mutable writer, binding/test facade, 그리고
lifetime owner까지 다시 추적했다. View Partial은 required XAML namescope와
signal-inspector/parameter/Learn composition을 유지해야 하지만, Analyze/Use/Undo
와 evidence availability를 한 파일에서 직접 조정할 이유는 없었다.

기존 `VisionToolThresholdSuggestionSession`이 suggestion 분석, stale evidence,
Applied/Previous snapshot과 Undo policy를 이미 소유하고 있고,
`VisionToolThresholdInteractionController`가 threshold parameter와 debounced
Preview를 이미 소유한다. 따라서 중복 policy owner나 interface를 만들지 않고,
WPF control을 참조하지 않는 구체적인 `ThresholdToolSuggestionController`를
추가해 workflow callback projection만 이동했다. View의 buttons, status,
advisory marker, automation IDs, binding/public/test facade, creation/release와
실행 경로는 유지했다.

`ThresholdSuggestionSessionContract`가 Debug·Release에서 각각 7/7을 통과했고,
`VisionRecipeRunnerSmoke` Debug·Release build, `wpf_shell_host_threshold_tool`
precheck, `cvr07_threshold_suggestion` selection/apply/Undo smoke, 단일 모니터
window geometry probe가 통과했다. 전체 theme/DPI/input, native dialog,
camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_THRESHOLD_TOOL_PARTIAL_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0040.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-suggestion-controller-20260914`
에 있다. 다음 heartbeat는 완료된 Threshold, Matching, Feature Matching, Edge
Based Matching/Auto MPoint, Morphology, Signal Inspector 경계를 재분리하지 않고
아직 보호되지 않은 Tool/Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0041)

열 번째 후속 slice는 `LineToolWpfView.xaml.cs`의 `ApplySampleLinePair`에 남은
sample A/B property projection 책임을 실제 caller, existing adapter, mutable
state writer, persistence seam, binding/test facade, lifetime owner까지 다시
추적했다. View Partial은 required XAML/PropertyGrid와 sample 후속 순서를
유지해야 하지만, common OpenCV와 Line field copy body를 직접 소유할 이유는
없었다.

기존 `VisionPipelineLinePropertyAdapter`는 Line/LineDistance/LineIntersection
property 생성과 pipeline step 변환을 이미 소유한다. 따라서 새 abstraction을
만들지 않고 `ApplySampleProperty`를 기존 adapter에 추가했다. View는 Line A/B
각각에 adapter를 호출한 뒤 기존처럼 purpose, `LineToolPresenter.PersistProperties`,
PropertyGrid visibility, input ROI overlay, summary, result cleanup을 같은 순서로
실행한다. `CvROIS`/`CvMASKS` defensive copy, mutable ViewModel state, PL-0027
persistence boundary와 XAML/public/test contract는 유지했다.

`LineToolPartialBoundaryContract`가 Debug·Release에서 각각 4/4를 통과했고,
`VisionRecipeRunnerSmoke` Debug·Release build, Line Tool/Measure/Intersection 및
Recipe Line Pair PropertyGrid WPF smoke, 단일 모니터 window geometry probe가
통과했다. 병렬 monitor probe의 첫 Line 실행은 shared localization fixture
경합으로 창이 생성되지 않아 폐기하고, 순차 retry를 authoritative evidence로
기록했다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_LINE_TOOL_PARTIAL_BOUNDARY_20260914.md`,
`.proofline/issues/PL-0041.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\line-tool-partial-boundary-20260914`
에 있다. 다음 heartbeat는 완료된 Line, Threshold, Matching, Feature Matching,
Edge Based Matching/Auto MPoint, Morphology, Signal Inspector 경계를 재분리하지
않고 아직 보호되지 않은 Tool/Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0042)

열한 번째 후속 slice는 `SimplePreprocessToolWpfView.xaml.cs`를
parameter/preview/signal owner와 함께 재검토하는 no-change 구조 감사였다.
View Partial은 required XAML/composition, presentation facade, callback wiring,
Parameter Guide binder와 debounced Preview scheduler lifetime만 소유했다.
Dynamic editor/settings snapshot은 `SimplePreprocessParameterController`,
header/summary는 `SimplePreprocessTextPresenter`, property projection은
`OpenVisionNativeSimplePreprocessPropertyFactory`, algorithm/result/signal은
`OpenVisionNativeSimplePreprocessPreviewExecutor`, settings persistence와
document composition은 `OpenVisionNativeSimplePreprocessDocumentFactory`,
최종 controller release는 `VisionToolSingleInputCustomToolViewBase`가 소유한다.
View 안에 직접 persistence/file-dialog/System.IO/OpenCvSharp/algorithm
construction이 없고 독립 state/lifetime/test seam이 없으므로 production
split은 추가하지 않았다.

`SimplePreprocessPartialBoundaryContract`가 owner wiring, binding/public/test
facade, no-direct-coupling, creation/release 경계를 9/9로 Debug·Release에서
통과했다. `VisionRecipeRunnerSmoke` Debug/Release builds, Rotate/Scale,
Simple Preprocess result-review/Learn-button WPF precheck와 동적 monitor/window
probe도 통과했다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및
long-running native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_SIMPLE_PREPROCESS_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0042.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\simple-preprocess-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Simple Preprocess와 기존 보호 경계를 재분리하지
않고 다른 미보호 Tool/Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0043)

열두 번째 후속 slice는 `AffineTransformToolWpfView.xaml.cs`를
ViewModel/generic property controller/preview/result owner와 함께 재검토하는
no-change 구조 감사였다. View Partial은 required XAML/controller adapter,
result-review facade, public/test facade만 소유했다. Mutable property와
summary는 `AffineTransformToolViewModel`, PropertyGrid/layer/preview/test와
controller lifetime은 `VisionToolSingleInputPropertyToolController`, matrix와
coverage 설명은 `AffineTransformResultReviewPresenter`, algorithm/overlay는
`OpenVisionNativeToolPreviewExecutor`와 overlay renderer, load/save/creation은
factory/composition/document builder, registration/release는 registry/base가
각각 소유한다. View 안에 직접 persistence/dialog/file-I/O/OpenCvSharp/
algorithm construction이 없고 독립 state/lifetime/test seam이 없으므로
production split은 추가하지 않았다.

`AffineTransformPartialBoundaryContract`가 owner wiring, binding/public/test
facade, no-direct-coupling, creation/release 경계를 9/9로 Debug·Release에서
통과했다. `VisionRecipeRunnerSmoke` Debug/Release builds, Affine Transform WPF
precheck와 동적 monitor/window probe도 통과했다. 전체 theme/DPI/input, native
dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_AFFINE_TRANSFORM_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0043.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\affine-transform-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Affine Transform과 기존 보호 경계를 재분리하지
않고 다른 미보호 Tool/Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0044)

열세 번째 후속 slice는 `ArithmeticToolWpfView.xaml.cs`를 Arithmetic 전용
interaction/text/preview owner와 shared double-input runtime, factory/document,
binding/test contract, registry/base lifetime까지 다시 추적한 no-change 구조
감사였다. View Partial은 required XAML/double-input composition, public facade,
and View-owned collaborator release만 소유한다. Operation/source/constant/
offset editor state와 event policy는 `ArithmeticToolInteractionController`,
localization/summary는 `ArithmeticToolTextPresenter`, debounce는
`ArithmeticToolPreviewController`, layer/command/preview runtime은 shared
double-input controller/ViewModel/binder, settings/pipeline routing은
`OpenVisionNativeArithmeticDocumentFactory`와 `OpenVisionNativeToolDocument`,
최종 release는 base View가 각각 소유한다. View 안에 직접 persistence/dialog/
file-I/O/OpenCvSharp/algorithm construction이 없고 독립 state/lifetime/test
seam이 없으므로 production split은 추가하지 않았다.

`ArithmeticToolPartialBoundaryContract`가 owner wiring, binding/public/test
facade, no-direct-coupling, creation/routing/release 경계를 9/9로 Debug·Release
에서 통과했다. `VisionRecipeRunnerSmoke` Debug/Release builds,
`wpf_arithmetic_tool_learn_button` 및 `wpf_layer_selection_arithmetic_tool`
WPF precheck와 동적 monitor/window probe도 통과했다. 전체 theme/DPI/input,
native dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_ARITHMETIC_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0044.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\arithmetic-tool-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Arithmetic과 기존 보호 경계를 재분리하지
않고 다른 미보호 Tool/Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0045)

열네 번째 후속 slice는 `FilterToolWpfView.xaml.cs`를 Filter interaction,
kernel input, text/guide, shared single-input runtime, ViewModel/presenter,
factory/document, binding/test contract, registry/base lifetime까지 다시
추적한 no-change 구조 감사였다. View Partial은 required XAML/custom-tool
composition, `CreateProperty()` facade, and View-owned collaborator release만
소유한다. Filter type/border selection과 mode visibility는
`VisionToolFilterInteractionController`, kernel lock/presets와 input event
수명은 `VisionToolKernelSizeController`, visible text는
`FilterToolTextPresenter`, parameter help는
`VisionToolCustomParameterGuideBinder`, mutable settings/summary는
`FilterToolViewModel`/`FilterToolPresenter`, layer/preview/result/language와
final release는 shared single-input controller/runtime/base가 각각 소유한다.
Settings load, Filter construction, document/preview/pipeline routing은
composition/factory/document builder가 소유한다.

따라서 독립 state/lifetime/test seam이 새로 입증되지 않았고, mechanical split로
새 ViewModel·service·wrapper·Partial을 추가하지 않았다.
`FilterToolPartialBoundaryContract`가 owner wiring, no-direct-coupling,
binding/public/test, creation/routing/release 경계를 9/9로 Debug·Release에서
통과했다. `VisionRecipeRunnerSmoke` Debug/Release builds, Filter/Morphology
focused WPF smoke, 동적 monitor/window probe와 repository documentation gates도
통과했다. 전체 theme/DPI/input, native dialog, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_FILTER_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0045.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\filter-tool-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Filter와 기존 보호 경계를 재분리하지 않고
Blob/Contour 또는 Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0046)

열다섯 번째 후속 slice는 `BlobToolWpfView.xaml.cs`를 Blob ViewModel/
normalization, generic property-grid controller/runtime/presenter, area
verification/threshold-teaching presenter, factory/document, preview/overlay,
binding/test contract, registry/pipeline/base lifetime까지 다시 추적한
no-change 구조 감사였다. View Partial은 required XAML/property-grid
composition, `CreateProperty()`/threshold/review facade, and View-owned WPF
collaborator construction만 소유한다. Mutable Blob property와 summary는
`BlobToolViewModel`/composition service, binding/layer/preview/persistence와
shared release는 generic controller/runtime/presenter, teaching/review는
area guide/criteria와 threshold controller, settings/document routing은
property-grid factory/builders, Blob algorithm/result/overlay는 preview
executor/overlay renderer, registration/pipeline/final release는 registry/
pipeline/base가 각각 소유한다. View 안에 직접 persistence/dialog/file-I/O/
OpenCV/algorithm construction이 없고 독립 state/lifetime/test seam이 없으므로
production split은 추가하지 않았다.

`BlobToolPartialBoundaryContract`가 owner wiring, no-direct-coupling,
binding/public/test, creation/document, algorithm/overlay, registry/pipeline/
release 경계를 11/11로 Debug·Release에서 통과했다. `VisionRecipeRunnerSmoke`
Debug/Release builds, `wpf_shell_host_blob_tool` 및
`wpf_openvision_learn_blob` WPF precheck, 동적 monitor/window probe와
repository documentation gates도 통과했다. 전체 theme/DPI/input, native
dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_BLOB_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0046.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\blob-tool-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Blob과 기존 보호 경계를 재분리하지 않고
Contour 또는 Learn Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0047)

열여섯 번째 후속 slice는 `ContourToolWpfView.xaml.cs`를 Contour ViewModel/
normalization, generic property-grid controller/runtime/presenter, area
verification/threshold-teaching presenter, factory/document, preview/overlay,
binding/test contract, registry/pipeline/base lifetime까지 다시 추적한
no-change 구조 감사였다. View Partial은 required XAML/property-grid
composition, `CreateProperty()`/threshold/review facade, and View-owned WPF
collaborator construction만 소유한다. Mutable Contour property와
range/epsilon/thickness normalization 및 summary는 `ContourToolViewModel`/
composition service, binding/layer/preview/persistence와 shared release는
generic controller/runtime/presenter, teaching/review는 area guide/criteria와
threshold controller, settings/document routing은 property-grid factory/
builders, Contour algorithm/result/overlay는 preview executor/overlay renderer,
registration/pipeline/final release는 registry/pipeline/base가 각각 소유한다.
View 안에 직접 persistence/dialog/file-I/O/OpenCV/algorithm construction이
없고 독립 state/lifetime/test seam이 없으므로 production split은 추가하지
않았다.

`ContourToolPartialBoundaryContract`가 owner wiring, no-direct-coupling,
binding/public/test, creation/document, algorithm/overlay, registry/pipeline/
release 경계를 11/11로 Debug·Release에서 통과했다. `VisionRecipeRunnerSmoke`
Debug/Release builds, `wpf_shell_host_contour_tool` 및
`wpf_openvision_learn_contour` WPF precheck, 동적 monitor/window probe와
repository documentation gates도 통과했다. 전체 theme/DPI/input, native
dialog, camera/SDK/GPU 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_CONTOUR_TOOL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0047.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\contour-tool-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Contour와 기존 보호 경계를 재분리하지 않고
Learn View Partial family에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0048)

열일곱 번째 후속 slice는 `BinaryLearnView.xaml.cs`를 Binary Learn의 fixed
lesson state/simulation, Window topic/action composition, WPF cell painting,
three animation timer lifetime, binding/test contract owner까지 다시 추적한
no-change 구조 감사였다. View Partial은 required XAML/presentation adapter,
topic visibility, animation/cell rendering, related-tool callback forwarding,
and test facade만 소유한다. Fixed samples, morphology/blob/contour stage
decisions, formulas, explanations, and simulation results are owned by
`BinaryLearnPresenter` and `OpenVisionLearnBinarySimulationModel`; topic and
callback composition are owned by `OpenVisionLearnWindow` and its existing
policy/caller. View 안에 직접 file-I/O/dialog/OpenCV/tool creation/persistence/
inspection policy가 없고 독립 state/lifetime/test seam이 없으므로 production
split은 추가하지 않았다.

`BinaryLearnPartialBoundaryContract`가 owner wiring, no-direct-coupling,
timer lifetime, topic/action composition, public/test facade, and XAML
automation contracts를 9/9로 Debug·Release에서 통과했다. `VisionRecipeRunnerSmoke`
Debug/Release builds, `wpf_openvision_learn_binary_line_contract` 및
`wpf_openvision_learn_binary_line_views` WPF precheck, 동적 monitor/window
probe와 repository documentation gates도 통과했다. 전체 theme/DPI/input,
native related-tool click/file association, camera/SDK/GPU 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_BINARY_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0048.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\binary-learn-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Binary Learn과 기존 보호 경계를 재분리하지
않고 다른 Learn View Partial에서 독립 owner seam을 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0049)

열여덟 번째 후속 slice는 `FoundationLearnView.xaml.cs`를 Foundation Learn의
Point/ROI·Mat-channel lesson state, WPF 셀/marker/brush projection, 두 timer
lifetime, related-tool callback, binding/test contract owner까지 다시 추적한
no-change 구조 감사였다. View Partial은 required XAML/presentation adapter와
WPF rendering/timer/callback만 소유하고, `FoundationLearnPresenter`가 stage
state, role/visibility, fixed guidance text와 Tool location policy를 소유한다.
`OpenVisionLearnWindow`는 topic/action composition과 public facade를 유지한다.
View 안에 직접 file-I/O/dialog/OpenCV/Tool creation/persistence/inspection
policy가 없고 독립 state/lifetime/test seam이 없으므로 production split은
추가하지 않았다.

`FoundationLearnPartialBoundaryContract`가 owner wiring, no-direct-coupling,
timer lifetime, topic/action composition, public/test facade와 XAML automation
contract를 9/9로 Debug·Release에서 통과했다. `VisionRecipeRunnerSmoke`
Debug/Release builds, `wpf_openvision_learn_foundation_contract` 및
`wpf_openvision_learn_foundation_view` Debug/Release WPF precheck, 그리고
작은 왼쪽 모니터로 명시 배치한 동적 monitor/window probe가 통과했다. 전체
theme/DPI/input, native related-tool click/file association, camera/SDK/GPU 및
long-running native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`다. 상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_FOUNDATION_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0049.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\foundation-learn-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Foundation/Binary와 기존 보호 경계를 다시
분리하지 않고 남은 Tool/Learn Partial 후보의 실제 owner seam만 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0050)

열아홉 번째 후속 slice는 `GrayscaleLearnView.xaml.cs`를 Threshold,
Brightness, Filtering, Arithmetic의 lesson state/evaluation, WPF cells/marker,
네 timer lifetime, related-tool callback, Apply/Close result event,
binding/test contract owner까지 다시 추적한 no-change 구조 감사였다. View
Partial은 required XAML/presentation adapter와 WPF input/rendering/timer/event
forwarding만 소유하고, `GrayscaleLearnPresenter`와
`OpenVisionLearnBasicGrayscaleSimulationModel`이 평가·단계·문구·primitive를
소유한다. `OpenVisionLearnWindow`는 topic/action composition, Apply/Close
forwarding과 child lifetime을 유지한다. View 안에 직접 file-I/O/dialog/OpenCV/
Tool creation/persistence/inspection policy가 없고 독립 state/lifetime/test
seam이 없으므로 production split은 추가하지 않았다.

`GrayscaleLearnPartialBoundaryContract`가 owner wiring, no-direct-coupling,
timer lifetime, topic/action composition, explicit result events, public/test
facade와 XAML automation contract를 11/11로 Debug·Release에서 통과했다.
`VisionRecipeRunnerSmoke` Debug/Release builds,
`wpf_openvision_learn_grayscale_contract` 및
`wpf_openvision_learn_grayscale_view` Debug/Release WPF precheck, 그리고
작은 왼쪽 모니터로 명시 배치한 동적 monitor/window probe가 통과했다. 전체
theme/DPI/input, native related-tool click/file association, camera/SDK/GPU 및
long-running native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`다. 상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_GRAYSCALE_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0050.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\grayscale-learn-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Grayscale/Foundation/Binary와 기존 보호
경계를 다시 분리하지 않고 남은 Tool/Learn Partial 후보의 실제 owner seam만
확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0051)

스무 번째 후속 slice는 `LayerRecipeLearnView.xaml.cs`를 Layer 목록, Step
Input/Tool/Output route, formula/meaning/status, WPF 셀/브러시 projection,
520ms timer lifetime, binding/test contract owner까지 다시 추적한 no-change
구조 감사였다. View Partial은 required XAML/presentation adapter와 WPF
input/rendering/timer만 소유하고, `LayerRecipeLearnPresenter`가 route/state와
설명 정책을 소유한다. `OpenVisionLearnWindow`는 topic visibility, refresh,
public facade와 child lifetime을 유지한다. View 안에 직접 file-I/O/dialog/
OpenCV/persistence/real Recipe execution/Tool creation policy가 없고 독립
state/lifetime/test seam이 없으므로 production split은 추가하지 않았다.

`LayerRecipeLearnPartialBoundaryContract`가 owner wiring, WPF-free presenter,
no-direct-coupling, timer lifetime, presenter state flow, public/test facade,
XAML automation과 Window composition contract를 12/12로 Debug·Release에서
통과했다. 기존 `LearnLayerRecipeContract`도 Debug·Release에서 4/4로
통과했으며, `VisionRecipeRunnerSmoke` Debug/Release builds,
`wpf_openvision_learn_layer_recipe_contract` 및
`wpf_openvision_learn_layer_recipe_view` Debug/Release WPF precheck, 그리고
작은 왼쪽 모니터로 명시 배치한 Debug/Release dynamic monitor/window probe가
통과했다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 Recipe execution/persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_LAYER_RECIPE_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0051.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\layer-recipe-learn-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Layer/Recipe와 기존 보호 경계를 다시
분리하지 않고 남은 Tool/Learn Partial 후보의 실제 owner seam만 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0052)

스물한 번째 후속 slice는 `GeometryLearnView.xaml.cs`를 Angle/Scale 설정,
Rotate→Scale→ROI review 단계, semantic role, formula/status와 Tool hint,
WPF transform/brush projection, 520ms timer lifetime, binding/test contract
owner까지 다시 추적한 no-change 구조 감사였다. View Partial은 required
XAML/presentation adapter와 WPF input/rendering/timer/callback만 소유하고,
`GeometryLearnPresenter`가 transform state와 단계·문구·Tool hint policy를
소유한다. `OpenVisionLearnWindow`는 topic selection, guide refresh, callback
composition, public facade와 child lifetime을 유지한다. View 안에 직접
file-I/O/dialog/OpenCV/persistence/real Recipe execution/Tool creation policy가
없고 독립 state/lifetime/test seam이 없으므로 production split은 추가하지
않았다.

`GeometryLearnPartialBoundaryContract`가 owner wiring, WPF-free presenter,
no-direct-coupling, transform/timer projection, callback ordering, public/test
facade, XAML automation과 Window composition contract를 12/12로
Debug·Release에서 통과했다. 기존 `LearnGeometryPresentationContract`도
Debug·Release에서 4/4로 통과했으며, `VisionRecipeRunnerSmoke`
Debug/Release builds, `wpf_openvision_learn_geometry_contract` 및
`wpf_openvision_learn_geometry_view` Debug/Release WPF precheck, 그리고 작은
왼쪽 모니터로 명시 배치한 Debug/Release dynamic monitor/window probe가
통과했다. 전체 theme/DPI/input, native Tool/file association, camera/SDK/GPU,
실제 Rotate/Scale·Affine Recipe execution/persistence 및 long-running native
runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세
owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_GEOMETRY_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0052.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\geometry-learn-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Geometry와 기존 보호 경계를 다시 분리하지
않고 남은 Tool/Learn Partial 후보의 실제 owner seam만 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0053)

스물두 번째 후속 slice는 `MetricsAcceptanceLearnView.xaml.cs`를 fixed
Good/Bad metric sample, average/range/maximum gate, animation stage, WPF
sample-cell/brush/text projection, 520ms timer lifetime, binding/test contract
owner까지 다시 추적한 no-change 구조 감사였다. View Partial은 required
XAML/presentation adapter와 WPF input/rendering/timer만 소유하고,
`MetricsAcceptanceLearnPresenter`가 samples, statistics, gate decisions,
stage/formula/status policy를 소유한다. `OpenVisionLearnWindow`는 topic
visibility, refresh, public facade와 child close lifetime을 유지한다. View
안에 직접 file-I/O/dialog/OpenCV/persistence/real Recipe execution/Tool
creation policy가 없고 독립 state/lifetime/test seam이 없으므로 production
split은 추가하지 않았다.

`MetricsAcceptanceLearnPartialBoundaryContract`가 owner wiring, WPF-free
presenter, no-direct-coupling, timer lifetime, presenter state flow,
public/test facade, XAML automation과 Window composition contract를 12/12로
Debug·Release에서 통과했다. 기존 `LearnMetricsAcceptanceContract`도
Debug·Release에서 3/3으로 통과했으며, VisionRecipeRunnerSmoke Debug 빌드는
오류 0개(기존 MSB3270 경고 2개), Release 빌드는 경고/오류 0개였다. WPF
contract/view precheck와 작은 왼쪽 모니터로 명시 배치한 Debug/Release dynamic
monitor/window probe도 통과했다. 전체 theme/DPI/input, native Tool/file
association, camera/SDK/GPU, 실제 Recipe execution/persistence 및 long-running
native runtime은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다.
OpenVisionReadinessCheck Debug/Release, RefactorAudit, DocumentationIndex,
LLM index JSON parse와 `git diff --check`도 통과했다.
상세 owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_METRICS_ACCEPTANCE_LEARN_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0053.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\metrics-acceptance-learn-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Metrics Acceptance와 기존 보호 경계를 다시
분리하지 않고 아직 보호되지 않은 Learn View Partial의 실제 owner seam만
확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0054)

스물세 번째 후속 slice는 `VisionToolVerificationGuideView.xaml.cs`를
dependency-property contract, compact-density projection, area/matching
verification presenter policy, 공통 single-input Tool shell composition과
대표 Blob/Contour/Edge/Matching consumer까지 다시 추적한 no-change 구조
감사였다. View Partial은 required XAML/presentation adapter와 named
automation/text/brush projection만 소유하고, 두 verification presenter가
criteria/result/status/next-action policy를, `VisionToolSingleInputPropertyToolShell`이
ToolContent/visibility/density routing을 소유한다. View 안에 직접
file-I/O/dialog/OpenCV/Recipe execution/persistence/async/business state/
lifetime coupling이 없고 독립 state/lifetime/test seam이 없으므로 production
split은 추가하지 않았다.

`VisionToolVerificationGuidePartialBoundaryContract`가 owner wiring,
dependency-property/binding/automation contract, no-direct-coupling,
compact-density projection, presenter/shell ownership과 representative
consumer composition을 12/12로 Debug·Release에서 통과했다. WPF shared-shell
Blob/Contour/Matching precheck와 Debug/Release dynamic monitor/window probe,
Readiness, RefactorAudit, DocumentationIndex, LLM index parse 및
`git diff --check`로 구조와 실행 경계를 보호한다. 전체 theme/DPI/input,
native Tool/file association, camera/SDK/GPU, 실제 inspection execution/
persistence 및 long-running native runtime은
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_VERIFICATION_GUIDE_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0054.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\verification-guide-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Verification Guide와 기존 보호 경계를
다시 분리하지 않고 남은 `VisionToolParameterGuideView` Partial의 실제
owner seam만 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0055)

스물네 번째 후속 slice는 `VisionToolParameterGuideView.xaml.cs`를
XAML/content projection, related-property callback, presenter/catalog,
PropertyGrid binder/host, sidecar와 shared single-input shell composition까지
다시 추적했다. View Partial은 required WPF presentation adapter로 유지했으며,
별도 state/lifetime/test seam이 없는 상태에서 새 ViewModel/service/interface/
forwarding Partial을 만들지 않았다.

대신 실제 재현된 owner 결함을 기존 catalog에서 보정했다. 전역
`DynamicPropertyGridTypeDescriptionProvider`가 inactive dependent property를
`TypeDescriptor`에서 숨기면 parameter guide가 applicability를 계산하지
못했으므로, `VisionToolParameterGuideCatalog.GetPropertyDescriptor`가 정상
descriptor를 우선 사용하고 누락된 public-instance property에만 attributes를
보존한 reflection descriptor fallback을 적용한다. 이 변경은 WPF visibility
정책을 View로 옮기지 않고 catalog의 guidance-policy owner에 남긴다.

`VisionToolParameterGuidePartialBoundaryContract`는 Debug/Release에서
14/14로 통과했고, Pipeline smoke Debug는 0 errors/기존 MSB3270 경고 2개,
Release는 0 errors/경고 3개(기존 MSB3270 2개와 `Program.cs:10181`의 기존
CS8600)였다. `p257_contextual_parameter_guide`,
`p259_parameter_guide_expansion`, `p260_parameter_guide_fallback_audit` WPF
precheck가 Debug/Release 모두 `OK`였고, 작은 왼쪽 모니터에 명시 배치한
dynamic monitor/window probe도 양 구성에서 통과했다. 전체 theme/DPI/input,
native Tool/file association, camera/SDK/GPU, 실제 inspection execution/
persistence와 long-running native runtime은 미검증이다. 상세 owner/call-path/
reading order는
`docs/reports/OPENVISIONLAB_PARAMETER_GUIDE_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0055.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\parameter-guide-partial-retention-20260914`
에 있다. 다음 heartbeat는 완료된 Parameter Guide와 기존 보호 경계를 다시
분리하지 않고 아직 보호되지 않은 `MatchingLearnView.xaml.cs`의 실제 owner
seam만 확인한다.

## 사용자 요청 후속 cycle — 2026-09-14 (PL-0056)

스물다섯 번째 후속 slice는 예약 후보였던 `MatchingLearnView.xaml.cs`가
기존 `OPENVISIONLAB_LEARN_MATCHING_VIEW_20260908` 완료 경계임을 먼저 확인해
중복 재작업을 건너뛰고, 별도 owner 증거가 부족했던
`VisionToolDoubleInputCustomToolShell.xaml.cs`를 점검했다. Shell Partial은
required XAML namescope, six dependency properties, preview/combo/button
visual facade, Learn click adapter와 docked/floating layout projection만
소유한다. `DockedInspectorLayoutController`도 같은 control의 named row/
column/group를 변경하는 presentation adapter다.

실행 정책이나 mutable layer/preview state는
`VisionToolDoubleInputCustomToolRuntime`/`VisionToolDoubleInputViewRuntime`이,
event/language/release는 `VisionToolDoubleInputCustomToolController`가,
attach와 `DisposeView` 순서는 `VisionToolDoubleInputCustomToolViewBase`가,
Arithmetic policy/preview/lifetime은 concrete View와 기존 factory/document
owner가 소유한다. Dock helper는 dock DP만 바꾸고 Learn Window 생성은
`VisionToolLearnWindowController`가 담당한다. 따라서 새 ViewModel/service/
wrapper/forwarding Partial을 추가하면 기존 WPF state를 감싸는 중복 owner가
되므로 production split은 추가하지 않았다.

`VisionToolDoubleInputCustomToolShellPartialBoundaryContract`가 Debug/Release
각각 12/12로 통과했다. `wpf_arithmetic_tool_learn_button`와
`wpf_layer_selection_arithmetic_tool` WPF smoke, 실제 창을 작은 왼쪽
모니터에 배치한 Debug/Release dynamic monitor/window probe도 통과했다.
VisionRecipeRunnerSmoke Debug/Release build는 오류 0개·경고 0개였다. 전체
Tool consumer, theme/DPI/input, native SDK/GPU/camera, 실제 inspection
execution/persistence와 long-running native runtime은 미검증이다. 상세
owner/call-path/reading order는
`docs/reports/OPENVISIONLAB_DOUBLE_INPUT_SHELL_PARTIAL_RETENTION_20260914.md`,
`.proofline/issues/PL-0056.json`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\double-input-shell-partial-retention-20260914`
에 있다. 다음 heartbeat는 PL-0056과 기존 보호 경계를 중복 처리하지 않고
현재 `Invoke-RefactorAudit -Verify`의 54개 compiled Partial과 PL-0028의
완료된 owner-map을 대조한다. 새 결함·요구사항·실패 기준이 없으면 더 이상
재분리할 안전한 경계가 없으므로 Partial 예약 automation을 종료하고,
새로운 사용자 요구가 들어올 때만 재개한다.
