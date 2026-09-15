# OpenVisionLab 리팩토링 프로그램 전수조사·우선순위·완료 기준 — 2026-09-08

## 조사 범위와 권위

이번 조사는 `C:\Git\2D\Dev`의 현재 `src`·`tools` 소스와 현재 Handoff,
완료 보고서, 안정 계약을 기준으로 수행했다. 첨부된
`C:\Users\USER\Downloads\OpenVisionLab_Implementation_Tasks_604c7fb.md`는
과거 후보 명세이므로 현재 미완료 상태의 증거로 사용하지 않았다. 기존 dirty
변경은 보존했으며 reset, clean, staging, commit, push, Original 변경은 하지
않았다.

## 현재 전수 계측

현재 worktree를 `bin`·`obj`를 제외하고 다시 계측한 결과는 다음과 같다.

| 항목 | 현재 값 | 판정에 쓰는 의미 |
| --- | ---: | --- |
| C# 소스 | 773개 / 268,742줄 / 12,238,522 bytes | 제품·라이브러리·검증 도구를 포함한 실제 Dev 소스 규모 |
| XAML 소스 | 58개 / 24,624줄 | WPF 화면·템플릿 책임이 남아 있는 범위 |
| partial 선언 | 106개 | 숫자 자체가 결함이 아니며 생성 코드·View·Shell 책임별로 재확인 |
| 프로젝트 / ProjectReference | 27개 / 34개 | 현재 `src`·`tools` 참조 그래프에서 순환 0개 |
| Shell CommandSurface | 10개 / 10,367줄 | 최근 owner 추출 후에도 남은 공통 조합 지점 |
| 대형 도구 파일 | 41,770줄, 19,199줄, 13,713줄 | `PipelineViewerScreenshotSmoke`, `DirectSmokeRunner`, `VisionRecipeRunnerSmoke`; 제품 구조와 분리해 판단 |
| 대형 제품 파일 | 4,087줄, 4,008줄, 3,110줄 | Shell Handlers, PropertyGrid adapter, Shell root; 파일 길이만으로 분리하지 않음 |

기존 타입 단위 조사에서 1,317개 타입 중 1,000줄 이상 28개, 2,000줄
이상 12개, 3,000줄 이상 8개였다. 이번에 추가된 owner·계약 파일은 모두
1,000줄 미만이어서 임계치 판정은 변하지 않는다. 계측 원본과 큰 파일 목록은
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-audit-20260908`에
보관했다. 이후 동일 기준을 반복 실행할 수 있도록
`tools/RefactorAudit/Invoke-RefactorAudit.ps1`를 추가했고, 실행 결과와
검증 증거는 [OVL-06b 완료 보고서](OPENVISIONLAB_OVL06B_QUANTITATIVE_AUDIT_20260908.md)와
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl06b-quantitative-audit-20260908`에
기록했다.

OVL-11 SaveFileDialog host slice 이후의 최신 계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-save-dialog-host-20260908\quantitative-audit-after-dialog-host`에
별도로 보관했다.

OVL-11 OpenFileDialog host slice 이후의 최신 계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-open-dialog-host-20260908\quantitative-audit-after-open-dialog-host`에
별도로 보관했다.

OVL-11 ContextMenu host slice 이후의 최신 계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-context-menu-host-20260908\quantitative-audit-after-context-menu-host`에
별도로 보관했다.

OVL-11 WinForms keyboard input slice 이후의 최신 계측 결과는
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-keyboard-input-20260908\quantitative-audit-after-keyboard-input에
별도로 보관했다.

OVL-11 WPF keyboard input slice 이후의 최신 계측 결과는
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-wpf-keyboard-input-20260908\quantitative-audit-after-wpf-keyboard에
별도로 보관했다.

OVL-11 mouse input slice 이후의 최신 계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-mouse-input-20260908\quantitative-audit-after-mouse-input`에
별도로 보관했다.

OVL-09 Learn Window/Shell host slice 이후의 최신 계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-learn-host-20260908\quantitative-audit-after-learn-host`에
별도로 보관했다.

OVL-09 Learn topic-selection/presentation composition slice 이후의 최신
계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-topic-composition-20260908\quantitative-audit-after-topic-policy`에
별도로 보관했다.

OVL-11 ImageCanvas Directory policy slice 이후의 최신 계측 결과는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-directory-policy-20260908\quantitative-audit-after-directory-policy`에
별도로 보관했다.

## OVL-06b 실행 결과

P0 계측 slice는 완료됐다. 감사 도구는 `src`·`tools`의 C#·XAML 규모와
partial/type 선언, 큰 파일, ViewModel UI·IO 신호, Shell Run History 저장소
호출, ProjectReference 순환을 읽기 전용으로 수집한다. `-Verify` 실행은
`CSharpFiles=763`, `XamlFiles=58`, `PartialDeclarations=106`,
`ViewModelUiIoFiles=2`, `ProjectCycles=0`, `ShellStorageCalls=0`으로
통과했으며, C: 출력 경로는 fail-closed로 거부됐다. 이 도구는 기존 제품
호출 경로를 변경하지 않고 다음 구조 slice의 회귀 기준선만 제공한다.

## OVL-11 첫 번째 실행 결과

P1의 첫 번째 독립 slice로 `RoiImageCanvasViewModel.SaveCurrentImage`의
Mat 파일 저장 정책을 `CanvasImageSaver`로 이동했다. 기존 public 메서드와
호출자는 그대로 두고, 확장자·디렉터리·callback·빈 Mat·`Cv2.ImWrite` 순서를
보존했다. ImageCanvas Debug/Release 빌드와 owned-Mat 저장 smoke가 모두
통과했다. `OpenFileDialog`·`SaveFileDialog`·`ContextMenu`·입력 이벤트는
아직 ViewModel에 남아 있으며 다음 OVL-11 slice의 대상이다.

[상세 완료 보고서](OPENVISIONLAB_OVL11_IMAGECANVAS_SAVE_OWNER_20260908.md)
와 D: 증거 root
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-image-save-owner-20260908`
에 현재 owner, 호출 경로와 실제 검증 결과를 기록했다.

## OVL-11 두 번째 실행 결과

두 번째 독립 slice로 `SaveFileDialog` 생성과 modal 결과 변환을
`RoiImageCanvasDialogHost`로 이동했다. `RoiImageCanvasView`가 attach 시
`IImageCanvasDialogHost`를 연결하고 detach/dispose 시 해제한다. 기존 dialog
옵션과 `SaveCurrentImage` 호출·마지막 디렉터리 갱신은 유지했으며, ImageCanvas와
기존 ImageCanvas/Shell focused smoke의 Debug/Release 결과가 모두 `OK`다.
대화상자 자체의 full WPF visual/theme/DPI matrix는 아직 실행하지 않았다.

[상세 완료 보고서](OPENVISIONLAB_OVL11_IMAGECANVAS_SAVE_DIALOG_HOST_20260908.md)
와 D: 증거 root
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-save-dialog-host-20260908`
에 contract, 구조 검색, lifecycle wiring, 빌드와 smoke 결과를 기록했다.

## OVL-11 세 번째 실행 결과

세 번째 독립 slice로 `OpenFileDialog` 생성과 modal 결과 변환을 기존
`RoiImageCanvasDialogHost`로 이동했다. `IImageCanvasDialogHost`는 Open/Save
path/null 계약을 함께 제공하고, host의 하나의 transient guard가 두 modal의
중복 진입을 차단한다. ViewModel은 `CanvasImageLoader.LoadMatFromFile`,
`LoadImage`, 로그, Mat using 범위, 마지막 디렉터리 갱신을 계속 소유한다.

ImageCanvas·외부 consumer·OpenVisionLab x64 Debug/Release 빌드와
ImageCanvas/Shell focused precheck가 모두 통과했으며, 구조 증명은
ViewModel의 직접 `OpenFileDialog`/`ShowDialog` 호출 제거와 View attach/
detach/dispose wiring을 확인했다. 전체 Windows OpenFileDialog
visual/theme/layout/DPI matrix는 실행하지 않았다.

[상세 완료 보고서](OPENVISIONLAB_OVL11_IMAGECANVAS_OPEN_DIALOG_HOST_20260908.md)
와 D: 증거 root
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-open-dialog-host-20260908`
에 work contract, 구조 검색, 빌드·smoke·audit 결과와 주니어 이해도 평가를
기록했다.

## OVL-11 네 번째 실행 결과

네 번째 독립 slice로 ImageCanvas의 ContextMenu instance와 `IsOpen` 호출을
ViewModel에서 기존 WPF View host로 이동했다. 새 내부 계약
`IImageCanvasContextMenuHost`는 `OpenContextMenu()` 하나만 제공하며,
`RoiImageCanvasView`가 기존 XAML `MainGrid.ContextMenu`와 DataContext wiring을
그대로 소유한다. ViewModel은 Measure/Teaching/AddRoiArray mode reset 정책과
오른쪽 클릭 명령을 유지하고 host 호출만 수행한다.

구조 증명은 concrete WPF ContextMenu와 `.IsOpen`의 ViewModel 잔존 여부,
View attach/detach/dispose lifecycle, 기존 `MenuItems`/명령 바인딩, 그리고
`ImageCanvasControl -> OnMouseRightClick -> ExecuteRightClickCommand ->
ContextMenuHost.OpenContextMenu -> View.MainGrid.ContextMenu.IsOpen` 호출 경로를
확인했다. ContextMenu 내용과 공개 command 계약은 변경하지 않았다.

ImageCanvas Debug/Release, 외부 consumer Release, OpenVisionLab x64
Debug/Release 빌드가 모두 경고 0/오류 0으로 통과했다. 기존 ImageCanvas/Shell
focused precheck도 Debug/Release에서 `OK 2 / WARN 0 / NG 0`이었다. 최신
감사는 `CSharpFiles=768`, `CSharpLines=268447`, `CSharpBytes=12224124`,
`ViewModelFiles=32`, `DirectUiOrDialogFiles=1`, `ProjectCycles=0`으로
`REFACTOR_AUDIT=PASS`를 반환했다. 상세 증거와 로그는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-context-menu-host-20260908`
에 있다.

주니어 개발자 자체 평가는 이 slice에 대해 **PASS**다. `right-click -> mode
policy -> ContextMenuHost -> View.MainGrid.ContextMenu` 순서와 XAML의
`MenuItems` binding을 한 번에 찾을 수 있다. 남은 WPF preview/마우스
입력과 `Directory` 접근은 다음 소유권 경계로 명시되어 있으며, WPF
PreviewKeyDown/KeyUp은 아래 여섯 번째 실행에서 완료됐다. 완료된 ContextMenu
owner를 다시 쪼개는 근거로 사용하지 않는다.

`소스 코드 기준 검토 완료 / 실제 Runtime ContextMenu UI 검증 필요`.
키보드/마우스/focus/pressed/selected/disabled/popup/theme/layout/DPI와
monitor 상호작용 전체는 실행하지 않았다. mouse owner는 아래 일곱 번째
실행에서 완료됐고 현재 다음 단일 slice는 OVL-09다.

[상세 완료 보고서](OPENVISIONLAB_OVL11_IMAGECANVAS_CONTEXT_MENU_HOST_20260908.md)
와 D: 증거 root
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-context-menu-host-20260908`
에 work contract, 구조 증명, 빌드·precheck·audit 결과와 주니어 평가를
기록했다.

## OVL-11 다섯 번째 실행 결과

다섯 번째 독립 slice로 ImageCanvasControl의 WinForms KeyDown 정책을
RoiImageCanvasKeyboardInputController로 이동했다. Controller는 Ctrl+Z,
Ctrl+Y, Ctrl+Shift+Z, Delete, Ctrl+C, Ctrl+V 분기와 ImageCanvasControl
KeyDown 구독/해제를 소유한다. 선택 ROI·복사 ROI 상태, snapshot publication,
Undo/Redo, ROI callback은 ViewModel callback으로 명시하고, C/V의 세부
동작은 기존 RoiInteractionKeyDown helper를 재사용한다.

RoiImageCanvasViewModel에는 WinForms KeyEventArgs handler와 KeyDown/KeyUp
구독이 남아 있지 않으며, Dispose는 ImageCanvasControl을 해제하기 전에
controller를 dispose한다. ImageCanvasControl의 public key event와 OpenGL
forwarding, WPF PreviewKeyDown/KeyUp 경로 및 마우스 경로는 변경하지 않았다.

D: 임시 keyboard-contract harness가 Ctrl+Z/Y/Shift+Z, Delete, Ctrl+C와
dispose 후 구독 해제를 실행해 KEYBOARD_INPUT_CONTRACT=PASS를 반환했다.
ImageCanvas Debug/Release, 외부 consumer Release, OpenVisionLab x64
Debug/Release 빌드가 경고 0/오류 0으로 통과했고, ImageCanvas/Shell
focused UI precheck의 두 target가 Debug/Release 모두 OK였다. 최신 감사는
CSharpFiles=768, CSharpLines=268447, CSharpBytes=12224124,
PartialDeclarations=106, DirectUiOrDialogFiles=1, ProjectCycles=0이다.

주니어 개발자 자체 평가는 이 slice에 대해 **PASS**다.
ImageCanvasControl.KeyDown -> RoiImageCanvasKeyboardInputController ->
ViewModel callbacks 순서로 이벤트 정책·상태·forwarding owner를 찾을 수
있고, C/V helper도 별도 파일에서 확인된다. 전체 데스크톱 키보드
focus/pressed/disabled/theme/layout/DPI/monitor matrix와 WPF preview/mouse
runtime은 실행하지 않았다.

완료된 Mat 저장, OpenFileDialog, SaveFileDialog, ContextMenu, WinForms
keyboard owner는 새 결함·명시 요구사항 변경·입증된 책임 충돌이 없으면
다시 나누지 않는다. 이 다섯 번째 실행의 다음 범위였던 WPF
PreviewKeyDown/KeyUp은 아래 여섯 번째 실행에서 완료됐고, mouse owner는
그 다음 일곱 번째 실행에서 완료됐다.

상세 보고서:
docs/reports/OPENVISIONLAB_OVL11_IMAGECANVAS_KEYBOARD_INPUT_20260908.md
Evidence:
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-keyboard-input-20260908

Historical next priority (completed below): OVL-11 WPF PreviewKeyDown/KeyUp input ownership | Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-11 여섯 번째 실행 결과

여섯 번째 독립 slice로 `RoiImageCanvasViewModel.Commands.cs`의 WPF
`PreviewKeyDown`/`KeyUp` 정책을 `RoiImageCanvasWpfKeyboardInputController`로
이동했다. 기존 public `PreviewKeyDownCommand`와 `KeyUpCommand`는 facade로
남고, `RoiImageCanvasView`는 이벤트 구독·전달·해제를 계속 소유한다.
Controller는 Control modifier guard, Delete callback/`Handled`, F2·Enter
no-op과 Ctrl+C/V/S KeyUp no-op을 보존하며 ViewModel의
`RemoveSelectedOverlay`만 명시적 callback으로 호출한다.

WPF keyboard controller는 View 이벤트를 직접 구독하지 않는 stateless
policy owner다. 따라서 View event lifetime, ViewModel ROI state, WinForms
keyboard policy가 각각 분리되어 실제 호출 경로를 따라갈 수 있고, 이번
slice에서 마우스·dialog·Mat·Recipe/XML 책임은 이동하지 않았다.

D: 임시 WPF keyboard contract harness가 `WPF_KEYBOARD_CONTRACT=PASS`를
반환했다. ImageCanvas Debug/Release, 외부 consumer Release, OpenVisionLab
x64 Debug/Release 빌드가 경고 0/오류 0으로 통과했고, ImageCanvas/Shell
focused UI precheck의 두 target가 Debug/Release 모두 `OK`였다. 최신 감사는
`CSharpFiles=769`, `CSharpLines=268466`, `CSharpBytes=12224956`,
`PartialDeclarations=106`, `DirectUiOrDialogFiles=1`,
`ProjectCycles=0`으로 `REFACTOR_AUDIT=PASS`를 반환했다.

주니어 개발자 자체 평가는 이 slice에 대해 **PASS**다. `RoiImageCanvasView`
event -> public command -> WPF keyboard controller ->
`RemoveSelectedOverlay` 순서로 정책·이벤트 수명·상태 owner를 읽을 수 있다.
실제 데스크톱 WPF keyboard focus/pressed/disabled/theme/layout/DPI/monitor
matrix와 물리 Control modifier 입력은 실행하지 않았으므로 runtime
qualification은 남아 있다.

완료된 Mat 저장, OpenFileDialog, SaveFileDialog, ContextMenu, WinForms
keyboard, WPF keyboard owner는 새 결함·명시 요구사항 변경·입증된 책임
충돌이 없으면 다시 나누지 않는다. mouse owner는 아래 일곱 번째 실행에서
완료됐고 현재 다음 단일 slice는 OVL-09다.

상세 보고서:
`OPENVISIONLAB_OVL11_IMAGECANVAS_WPF_KEYBOARD_INPUT_20260908.md`
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-wpf-keyboard-input-20260908`

Next priority: OVL-09 Learn Window/Shell residual composition | Recommended model: gpt-5.6-terra | Reasoning effort: high.

## OVL-11 일곱 번째 실행 결과

일곱 번째 독립 slice로 ImageCanvasControl의 7개 마우스 이벤트 수명과
ROI 마우스 orchestration을 `RoiImageCanvasMouseInputController`로 이동했다.
ViewModel은 selected/drawing ROI, measurement, pan 상태와 기존 snapshot,
ContextMenu, ROI 결과 callback을 유지하고, controller는 button/mode policy와
기존 `RoiInteractionMouse*`/`RoiInteractionCursor` helper 호출을 소유한다.

`InitEvent()`/`ReleaseEvents()`에는 Load, Resized, Draw만 남았고, controller의
`Dispose()`는 ImageCanvasControl dispose 전에 실행된다. public mouse event,
OpenGL forwarding, ROI callback 순서, Recipe/XML·Preview/Run·Layer/ImageSpace
계약은 변경하지 않았다. Mat, dialog, ContextMenu, WinForms keyboard, WPF
keyboard owner와 P0 instrumentation은 완료 경계로 닫았다.

D: mouse contract harness가 `MOUSE_INPUT_CONTRACT=PASS`를 반환했다. 7개
구독이 각각 한 번 추가되고 dispose 후 원래 invocation count로 돌아오며,
두 번째 dispose가 추가 변경을 만들지 않는 경로를 확인했다. ImageCanvas
Debug/Release, 외부 consumer Release, OpenVisionLab x64 Debug/Release 빌드와
ImageCanvas/Shell focused UI precheck가 모두 통과했다. 최신 감사는
`REFACTOR_AUDIT=PASS`, `CSharpFiles=770`, `CSharpLines=268637`,
`CSharpBytes=12232911`, `PartialDeclarations=106`, `ViewModelUiIoFiles=1`,
`ProjectCycles=0`, `ShellStorageCalls=0`이다.

주니어 개발자 자체 평가는 이 slice에 대해 **PASS**다. 이벤트 source에서
단일 mouse controller로 들어가 기존 helper와 명시적 ViewModel callback으로
나오는 경로가 보이며, ViewModel의 lifecycle 이벤트와 상태 소유도 분리되어
있다. 실제 데스크톱 mouse focus/hover/pressed/selected/disabled/popup/
theme/layout/DPI/monitor matrix는 실행하지 않았으므로 runtime qualification은
남아 있다.

상세 보고서:
`OPENVISIONLAB_OVL11_IMAGECANVAS_MOUSE_INPUT_20260908.md`
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-mouse-input-20260908`

다음 우선순위는 **OVL-09 Learn Window/Shell 잔여 topic composition**이다.
`Recommended model: gpt-5.6-terra | Reasoning effort: high`

## OVL-09 여덟 번째 실행 결과

여덟 번째 독립 slice로 Shell의 Learn Window 수명과 진입 routing을
`OpenVisionShellHostLearnWindowController`로 이동했다. 이전에는
`OpenVisionShellHostCommandController`가 workspace image/sample workflow와
Learn Window 생성·재사용·owner·callback·`Closed` 정리를 함께 소유했다.
현재 Shell Chrome과 Pipeline Review의 Learn 진입은 새 owner를 거치고,
workspace sample 실행은 기존 CommandController callback으로만 연결된다.

새 owner는 `OpenVisionLearnTopicCatalog`, `OpenVisionLearnWindow`, 기존
topic View/Presenter를 재사용한다. Recipe/XML, 명시적 Preview/Run,
Layer/ImageSpace와 Tool View callback 계약은 변경하지 않았다.

D: 계약 하네스가 `LEARN_HOST_CONTRACT=PASS`를 반환했다. Tool menu/type
topic routing, 기존 창 재사용, sample path callback, 암묵적 Tool 실행 금지,
`Closed` 후 새 창 생성, unknown tool no-op을 확인했다. 구조 증명은
`LEARN_HOST_STRUCTURE_PROOF=True`였다. OpenVisionLab x64 Debug/Release,
external consumer Release 빌드는 0 warnings/0 errors였고, Learn/Shell
focused UI precheck는 Debug/Release 모두 `OK 2 / WARN 0 / NG 0`이었다.
최신 감사는 `REFACTOR_AUDIT=PASS`, `CSharpFiles=771`,
`CSharpLines=268667`, `CSharpBytes=12234466`, `PartialDeclarations=106`,
`ProjectCycles=0`, `ShellStorageCalls=0`이다.

주니어 개발자 자체 평가는 이 경계에 대해 **PASS**다. Shell command 또는
Pipeline Review에서 하나의 명명된 Learn Window owner로 들어가 topic catalog,
window, 명시적 sample/Tool callback으로 이어지는 경로를 따라갈 수 있다.
`OpenVisionLearnWindow.xaml.cs`의 topic selection·panel visibility·문서/실습
presentation은 아직 공통 composition으로 남아 있다. 물리 desktop
focus/pressed/theme/layout/DPI/monitor/resize 행렬은 실행하지 않았다.

상세 완료 보고서:
`OPENVISIONLAB_OVL09_LEARN_WINDOW_HOST_20260908.md`
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-learn-host-20260908`.

다음 단일 slice는 **OVL-09 Learn Window topic-selection/presentation composition**이다.
`Recommended model: gpt-5.6-terra | Reasoning effort: high`

## OVL-09 아홉 번째 실행 결과

아홉 번째 독립 slice로 `OpenVisionLearnWindow.xaml.cs`의 17개 topic 조건문이
함께 결정하던 제목·부제·실습 문구·패널 표시·실습 expander·guide 갱신
정책을 `OpenVisionLearnTopicPresentationPolicy`로 이동했다. 정책은
WPF-independent immutable state를 반환하고, Window는 기존 child
View/Presenter의 `SelectTopic` 및 guide 갱신 메서드를 호출하는 View 적용
책임만 유지한다. 기존 `OpenVisionLearnTopicCatalog`, Recipe/XML,
명시적 Preview/Run, Layer/ImageSpace, SDK 계약은 변경하지 않았다.

D: pure policy 계약은 `TOPIC_POLICY_CONTRACT=PASS`, 실제 17개 topic을
선택하는 WPF 계약은 `TOPIC_WINDOW_CONTRACT=PASS`, 구조 증명은
`TopicPolicyStructureProof=True`였다. OpenVisionLab x64 Debug/Release와
external consumer Release 빌드는 모두 0 warnings/0 errors였다. Brightness,
Threshold, Matching 대표 Learn UI smoke와 Debug/Release Threshold precheck
가 통과했다(`OK 1 / WARN 0 / NG 0`). 최신 감사는
`REFACTOR_AUDIT=PASS`, `CSharpFiles=772`, `CSharpLines=268718`,
`CSharpBytes=12237951`, `PartialDeclarations=106`, `ProjectCycles=0`,
`ShellStorageCalls=0`이다.

주니어 개발자 자체 평가는 이 경계에 대해 **PASS**다. 읽기 경로는
`TopicList_SelectionChanged -> UpdateSelectedTopic -> policy.Resolve ->
Window applies state -> existing topic View/Presenter guide owner`로
구분된다. 정책은 WPF control/Window를 참조하지 않고, Window는 화면 적용
책임을 명시적으로 보유한다. 전체 physical desktop focus/hover/pressed/
selected/disabled/theme/layout/DPI/monitor/resize 행렬은 실행하지 않았다.
Baseline broader Learn screenshot smoke attempts reported curriculum 내부
문구, Layer/Recipe routing safety, Metrics/Acceptance animated outlier,
Color/HSV practice precondition failures as unrelated baseline; expectations
were not relaxed.

상세 완료 보고서:
`OPENVISIONLAB_OVL09_LEARN_TOPIC_PRESENTATION_20260908.md`
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl09-topic-composition-20260908`.

다음 단일 slice는 **OVL-11 ImageCanvas ViewModel Directory policy owner**다.
`Recommended model: gpt-5.6-terra | Reasoning effort: high`

## OVL-11 ImageCanvas Directory policy 실행 결과

ImageCanvas의 독립적인 Directory 정책 slice로
`RoiImageCanvasViewModel.Commands.cs`가 소유하던 `lastImageDirectory`,
`Sample`/`Samples`/`samples` 상위 탐색, `Directory.Exists` 기반 초기 경로
선택을 `ImageCanvasDirectoryPolicy`로 이동했다. ViewModel은 Open/Save
command 순서, filename sanitization, dialog host, Mat load/save와 성공 후
경로 기억 호출만 유지한다. 앱 계층의 `OpenVisionImageDirectoryResolver`는
다른 caller contract를 가지므로 재사용·재분할하지 않았다.

기존 경로 우선순위는 remembered directory -> sample search -> application
base -> Pictures -> Desktop으로 보존했다. D: contract 네 케이스가
`DIRECTORY_POLICY_CONTRACT=PASS`를 반환했고, 구조 증명은
`PolicyStructureProof=True`였다. ImageCanvas Debug/Release, OpenVisionLab
x64 Debug/Release, external consumer Release 빌드는 0 warnings/0 errors였다.
기준선 ImageCanvas UI precheck와 변경 후 Debug/Release의
`wpf_imagecanvas_owned_mat_load` 및
`wpf_shell_host_tool_input_image_load_save`가 모두 통과했다(`OK 2 / WARN 0 /
NG 0`). 최신 감사는 `REFACTOR_AUDIT=PASS`, `CSharpFiles=773`,
`CSharpLines=268742`, `CSharpBytes=12238522`, `PartialDeclarations=106`,
`ProjectCycles=0`, `ShellStorageCalls=0`이다.

주니어 개발자 자체 평가는 이 경계에 대해 **PASS**다. 읽기 경로는
`LoadImageCommand/SaveImageCommand -> ImageCanvasDirectoryPolicy ->
ImageDialogHost -> 기존 image owner`로 분리된다. 전체 physical desktop
focus/hover/pressed/selected/disabled/theme/layout/DPI/monitor/resize
행렬은 실행하지 않았다.

상세 완료 보고서:
`OPENVISIONLAB_OVL11_IMAGECANVAS_DIRECTORY_POLICY_20260908.md`
Evidence:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-directory-policy-20260908`.

대표 WPF runtime qualification matrix 실행 결과를 아래에 기록한다.
`Recommended model: gpt-5.6-terra | Reasoning effort: high`

## 대표 WPF runtime qualification 결과

대표 WPF runtime qualification은 별도 검증 slice로 완료했다. 제품 소스나
Recipe/XML·Preview/Run·Layer/ImageSpace 계약은 바꾸지 않고, 기존 Debug/Release
바이너리를 현재 데스크톱에서 물리적으로 실행했다. 하네스는 모니터를 동적으로
열거해 더 작은 왼쪽 비주 모니터 `\\.\DISPLAY2`를 선택했고, 정상 창을
`-1900,385,1600x900`에 배치했다. `GetDpiForWindow=96`(100%)와 maximize
working-area 경계를 기록했다.

Debug와 Release가 각각 17개 결과를 모두 `PASS`로 반환했고 `FAIL=0`,
`WARN=0`, 종료 코드 0이었다. 확인한 흐름은 시작/AutomationId 계약, keyboard
focus와 검색 값, Compact rail, language popup, Learn Window와 topic 선택,
native `#32770` 이미지 dialog의 monitor 배치·파일명 입력·Open 수락 및
`도킹 레이어 1개` 상태, maximize/restore, Threshold 선택,
Pipeline 선택, top-window inventory, deterministic shutdown이다. 초기·Learn·
image-loaded·Threshold·Pipeline·maximized PNG는 같은 증거 root에 저장했다.

상세 보고서는
`OPENVISIONLAB_WPF_RUNTIME_QUALIFICATION_20260908.md`이며 원시 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-wpf-runtime-20260908`의
`debug`/`release`, `work-contract.md`, `run-metadata.txt`에 있다. 하네스
parser는 `PARSER=PASS`였고, 실행 뒤 `OpenVisionLab` 프로세스는 남지 않았다.

주니어 개발자 평가는 **PASS for this slice**다. 증거 흐름이
`monitor -> app handle -> AutomationId -> physical action -> visible/result
assertion -> shutdown`으로 읽히며 native dialog 보조 로직도 하네스 내부 한
owner로 격리돼 있다. 이 결과는 현재 96% two-monitor 환경만 증명한다. 125%,
150%, 175%, 200% DPI, 다른 theme, one-monitor/headless 행은 실행하지 않아
환경 전제 조건으로 남긴다.

완료된 96% runtime owner를 다른 모델이나 agent가 재실행·재분할·주석/문서
복제로 반복하지 않는다. 새 실패, 변경된 계약, 또는 미검증 환경 행이 실제로
가능해진 경우에만 다시 연다.

다음 단일 우선순위는 **사용 가능한 alternate DPI/theme runtime 행 확보 또는
환경 전제 기록**이다.
`Recommended model: gpt-5.5 | Reasoning effort: medium`

### 현재 결합 신호

- `src`의 ViewModel 이름 파일 32개 중 1개가 직접 WPF/WinForms UI 또는 대화상자를 사용한다.
- `RoiImageCanvasViewModel.Commands.cs`의 Directory 접근과 shared
  `lastImageDirectory` state는 `ImageCanvasDirectoryPolicy`로 이동했다.
  ContextMenu, WPF PreviewKeyDown/KeyUp, mouse policy는 각각 명시적 owner로,
  `OpenFileDialog`와 `SaveFileDialog`는 `RoiImageCanvasDialogHost`로 이동했다.
  Other ViewModel IO signals remain in the audit, and representative 96% WPF runtime
  qualification is complete; alternate DPI/theme rows remain environment-bound.
- `OpenVisionShellHostRecipeCommandSurface*.cs`에는 Run History summary
  `List`·`Load` 직접 호출이 0개다. 최근 Run History owner 분리는 완료된
  경계이므로 다시 나누지 않는다.
- 27개 프로젝트와 34개 참조의 현재 그래프에는 순환이 없다.
## 주니어 개발자 관점의 모듈화 판정

전체를 “완전히 이해 가능한 모듈”로 판정할 수는 없다. 주요 흐름은
책임-oriented 폴더와 owner/presenter/use-case 이름으로 따라갈 수 있지만,
공통 조합점과 한 개의 UI/IO 혼합 ViewModel은 추가 안내가 필요하다.

| 점검 항목 | 결과 | 근거 |
| --- | --- | --- |
| Recipe/Pipeline 책임 이름과 폴더 | 대체로 충족 | `Recipe`, `Validation`, `PipelineReview`, `Review` owner·presenter가 존재하고 구조 문서에 경로가 있음 |
| Recipe CommandSurface | 부분 충족 | 10개 partial로 기능별 진입점은 보이지만 root/Handlers가 여전히 큰 공통 조합점임 |
| Learn Window | 대체로 충족 | 주제별 View/Presenter와 WPF-independent topic presentation policy가 분리됐고, `OpenVisionLearnWindow.xaml.cs`는 control 적용과 기존 View/Presenter 호출을 담당하는 composition 지점으로 남음 |
| ViewModel UI 순수성 | 미충족 지점 존재 | `RoiImageCanvasViewModel.Commands.cs`의 Directory traversal은 제거됐고, filename sanitization만 남아 있으며 다른 ViewModel IO signals는 별도 owner 검토 대상임 |
| 테스트 가능한 경계 | 부분 충족 | OVL-01~10의 여러 완료 계약과 최근 Run History 계약, OVL-06b 계측 도구, OVL-11 저장/Open/Save/ContextMenu/WinForms/WPF/mouse/Directory focused proof, OVL-09 topic policy/all-topic WPF proof, 대표 96% Debug/Release runtime proof가 있음. Alternate DPI/theme 행은 남아 있음 |
| 문서 탐색성 | 충족(현재 작업 기준) | Handoff, `CODEBASE_STRUCTURE`, 완료 보고서와 인덱스가 owner별 읽기 순서를 제공함 |

따라서 현재 성숙도는 “RC/pre-production 수준의 핵심 owner와 계약이 넓게
구축된 상태”이며, “모든 코드가 주니어에게 즉시 자명한 상태”는 아니다.

## 우선순위

완료된 경계를 파일 크기나 새 모델의 선호만으로 다시 열지 않고, 현재 결합
신호와 검증 공백을 기준으로 다음 순서를 사용한다.

1. **OVL-06b 전수 정량 감사 instrumentation (완료)**  \
   `Recommended model: gpt-5.6-luna | Reasoning effort: low`  \
   [완료 보고서](OPENVISIONLAB_OVL06B_QUANTITATIVE_AUDIT_20260908.md)의
   읽기 전용 계측 도구와 D: 증거 출력으로 반복 기준선을 고정했다.
2. **OVL-11 ImageCanvas ViewModel UI/IO 경계 (Mat 저장·Open/SaveFileDialog·ContextMenu·WinForms/WPF keyboard/mouse/Directory owner 완료)**  \
   `Recommended model: gpt-5.6-terra | Reasoning effort: high`  \
   Mat 저장 정책은 `CanvasImageSaver`, Open/SaveFileDialog 생성·결과 변환은
   `RoiImageCanvasDialogHost`, ContextMenu 표시 호출은
   `IImageCanvasContextMenuHost`/`RoiImageCanvasView`, WPF
   PreviewKeyDown/KeyUp 정책은 `RoiImageCanvasWpfKeyboardInputController`로,
   마우스 이벤트 수명·ROI policy는 `RoiImageCanvasMouseInputController`로
   Directory policy도 `ImageCanvasDirectoryPolicy`로 이동했고 호출 경로가
   D: contract로 입증됐다. Canvas의 Focus·Drag & Drop·DPI·렌더링과 외부
   consumer 계약은 View/adapter 경계에 남긴다.
3. **OVL-09 Learn topic presentation boundary (완료)와 runtime evidence (96% 완료)**  \
   `Recommended model: gpt-5.6-terra | Reasoning effort: high`  \
   WPF-independent policy와 all-topic WPF 계약은 완료됐다. 물리 desktop
   theme/layout/DPI/monitor/focus 행렬은 별도 증거 단계로 남긴다.
4. **Alternate WPF runtime qualification rows (환경 의존)**  \
   `Recommended model: gpt-5.5 | Reasoning effort: medium`  \
   대표 96% two-monitor Debug/Release 실행은 완료됐다. 125%·150%·175%·200% DPI,
   alternate theme, one-monitor/headless는 실제 환경이 제공될 때만 실행하고, 실패가
   있을 때만 해당 root cause를 수정한다.
5. **대형 검증 도구의 가독성 개선**  \
   `Recommended model: gpt-5.6-luna | Reasoning effort: low`  \
   41,770줄·19,199줄·13,713줄 파일은 도구 유지보수 신호이지만, 파일 크기만
   이유로 partial이나 wrapper를 추가하지 않는다. 테스트 책임 또는 실행 경계가
   실제로 충돌할 때만 독립 contract/runner로 이동한다.

다음 경계는 현재 닫혀 있다: OVL-01 Native Tool lifetime, OVL-02 TCP identity,
OVL-03 Runtime log queue, OVL-04 ImageSpace/Layer lease, OVL-05 execution
lifetime, OVL-06a process recovery, OVL-07의 완료된 Pipeline Review·Recipe
manager·Run History owner, OVL-08 PropertyGrid 정책, OVL-10 metric 단위와
완료된 Learn topic owner. 재개 조건은 재현된 결함, 변경된 명시 요구사항 또는
입증된 책임 충돌이다.

## 리팩토링 완료 기준

프로그램 전체 완료는 날짜나 커밋 수가 아니라 아래 모든 기준이 통과한 시점으로
판정한다.

### 구조 기준

- 우선순위 P0~P3의 각 열린 항목에 현재 owner, 새 owner, 의존 방향,
  상태/data 소유자, 보존 계약이 문서로 남아 있다.
- 이전 owner가 이동한 책임을 더 이상 직접 소유하지 않고, 새 호출 경로가
  실제 실행된다. 단순 partial 파일 추가·이름 변경은 통과로 보지 않는다.
- ViewModel은 Window/UserControl/구체 컨트롤/모달 대화상자를 직접 참조하지
  않으며, 예외가 필요한 Canvas UI adapter에는 이유와 경계가 문서화돼 있다.
- Recipe/XML 형식, 명시적 Preview/Run, Layer 생성·삭제·선택, ImageSpace
  Lease/Dispose, 기존 SDK·외부 consumer 계약이 변경되지 않는다.
- 큰 타입은 추출·유지 결정을 각각 기록한다. 크기만을 이유로 재분할하지 않는다.

### 검증 기준

- 각 slice에 독립 실행 가능한 focused contract가 있고 Debug/Release에서
  통과한다.
- 영향을 받는 앱·Runner 빌드가 x64 Debug/Release에서 0 warnings/0 errors다.
- Recipe/XML 호환성과 관련된 변경은 기존 fixture/compatibility check를
  통과한다.
- UI를 건드린 slice는 실제 runtime에서 해당 View/control의 관련 상태·테마·
  layout·DPI·키보드/마우스 경로를 실행한다. 실행하지 않은 행렬은 완료로
  주장하지 않는다.
- 최종 전수조사에서 미해결 P0/P1 구조 결합, scope 내 TODO, 중복 owner가
  남지 않는다.
- Handoff·구조 문서·LLM 인덱스·각 완료 보고서가 현재 call path와 일치한다.

### 운영 기준

- 한 예약 실행은 한 independently verifiable slice만 수행한다.
- 같은 owner를 다른 모델·에이전트·예약 실행이 동시에 수정하지 않는다.
- 현재 dirty 변경을 보존하고, 자동화는 commit/push/Original/release/deploy를
  수행하지 않는다.
- 모든 기준이 통과하면 heartbeat가 자기 자신을 삭제하거나 `PAUSED`로
  전환하고, 외부 입력·하드웨어·권한이 필요한 경우 `Blocked`로 기록하고
  자동 실행을 멈춘다.

## 예상 기간과 기준 시점

예상 열린 구조 slice는 OVL-09 2~3회이며, 마지막 통합 계측·runtime
qualification·문서 종료에 1~2회를 둔다. P0과 OVL-11의 Mat 저장·Open/
SaveFileDialog·ContextMenu·WinForms/WPF keyboard/mouse owner는 완료됐으므로
현재 남은 작업은 3~5회(5분 heartbeat 기준)로 추정한다.
전체 프로그램은 최초
기준으로 **약 8~12회(최소 2~3시간의 활성 실행 시간)**가 예상치였으며,
빌드 실패, 외부 데이터, 하드웨어 또는 사용자 결정이 발생하면 이 예상치는
무효가 되며 자동화는 중단하고 blocker를 남긴다. 따라서 실제 완료 시점은
“8~12회 후”가 아니라 위 구조·검증·운영 기준이 모두 PASS가 된 첫 시점이다.

## 자동화 계약

기존 `openvisionlab-2d` heartbeat를 5분 주기로 활성화했다(`status=ACTIVE`,
`RRULE:FREQ=MINUTELY;INTERVAL=5`, 현재 task에 연결). 매 실행은
현재 Handoff와 completed-owner registry를 읽고, 가장 높은 미완료 항목 한
개만 선택해 구현·focused 검증·증거·Handoff 갱신을 수행한다. 이미 완료된
owner를 다시 나누거나, 병렬 모델/에이전트를 생성하거나, 무관한 주석·문서·
정리 작업을 추가하지 않는다. 완료 기준을 모두 만족하면 자동화가 스스로
종료되고, 새 범위는 사용자가 별도로 명시해야 한다.

## Completion record

Status: Complete (전수조사·우선순위·주니어 모듈화 판정·완료 기준·자동화
계약 정의)

Scope: 현재 Dev worktree의 구조 조사와 단계적 리팩토링 운영 기준 수립.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-audit-20260908`
의 `source-survey-current.json`, `largest-files.csv`, `shell-files.csv`,
`inventory-summary.txt`, `priority-evidence.txt`, `junior-modularity-check.txt`,
`project-cycle-check.txt`.

Final state check: branch `codex/public-sample-ux-docs`, HEAD
`d875559577c85984d54900df973a6fb35fb20146`, dirty status 257 entries,
staged entries 0. The `openvisionlab-2d` heartbeat is `ACTIVE` with the
5-minute RRULE recorded in the automation file.

Boundary: P0 계측과 OVL-11의 Mat 저장·Open/SaveFileDialog·ContextMenu·WinForms/WPF
keyboard/mouse/Directory owner, OVL-09 Learn host와 topic presentation policy는
완료됐다. WPF runtime qualification은 아직 남아 있으며 다음 독립 slice는
대표 WPF runtime qualification matrix다.
