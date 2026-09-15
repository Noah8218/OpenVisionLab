# OVL-11 ImageCanvas WinForms 키보드 입력 owner — 2026-09-08

## Status

**Complete** for one independently verifiable WinForms keyboard input ownership
boundary. This report records the current Dev checkout only. No commit, push,
Original checkout, release, or deployment action was performed.

## Scope

ImageCanvasControl.KeyDown에서 실행되는 기존 Ctrl+Z/Ctrl+Y/Ctrl+Shift+Z,
Delete, Ctrl+C, Ctrl+V 정책을 RoiImageCanvasViewModel에서
RoiImageCanvasKeyboardInputController로 이동했다.

보존한 계약:

- Ctrl+Z는 UndoRequested를 발생시키고 Handled/SuppressKeyPress를 설정한다.
- Ctrl+Y와 Ctrl+Shift+Z는 RedoRequested를 발생시키고 같은 입력 억제를
  유지한다.
- Delete는 기존 RemoveSelectedOverlay와 ROI snapshot publication 순서를
  유지한다.
- Ctrl+C/Ctrl+V는 기존 RoiInteractionKeyDown helper와
  OnRoiAdded/OnRoiGrouped callback, snapshot publication을 그대로 사용한다.
- 선택 ROI와 복사 ROI 상태, Undo/Redo 및 snapshot event는 ViewModel이
  보유하고 controller는 명시적 callback으로 호출한다.
- ImageCanvasControl의 public KeyDown/KeyUp 선언과 OpenGL event forwarding은
  변경하지 않았다.
- WPF PreviewKeyDown/KeyUp, 마우스 입력, ContextMenu/dialog host,
  Mat/Layer/ImageSpace, Recipe/XML, 명시적 Preview/Run, SDK/외부 consumer
  계약은 이 slice에서 변경하지 않았다.

## Ownership and lifecycle

현재 호출 경로는 다음과 같다.

ImageCanvasControl.KeyDown
-> RoiImageCanvasKeyboardInputController.OnKeyDown
-> existing VM state/snapshot/Undo/Redo callbacks
-> existing RoiInteractionKeyDown overlay helper (C/V)

RoiImageCanvasViewModel은 constructor에서 controller 하나를 만들고,
Dispose()에서 ImageCanvasControl의 이벤트를 해제하기 전에 controller를
dispose한다. Controller는 동일 handler를 한 번만 구독하고 Dispose()를
idempotent하게 해제한다. ViewModel에는 WinForms KeyEventArgs handler나
KeyDown/KeyUp subscription이 남아 있지 않다.

## Changed files

- src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiImageCanvasKeyboardInputController.cs
  - WinForms KeyDown 구독, 기존 단축키 분기, callback 연결, 수명 해제.
- src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs
  - controller 조합, 키보드 구독/핸들러 제거.
- src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.Refresh.cs
  - Dispose()에서 controller를 먼저 해제.

## Verification

Evidence root:
D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-keyboard-input-20260908

- keyboard-contract temporary harness: KEYBOARD_INPUT_CONTRACT=PASS.
  Ctrl+Z, Ctrl+Y, Ctrl+Shift+Z, Delete, Ctrl+C 및 dispose 후 구독 해제를
  직접 실행했다. 하네스 소스와 로그는 evidence root의 keyboard-contract에
  있으며 repository source에는 추가하지 않았다.
- ImageCanvas Debug build: 0 warnings / 0 errors.
- ImageCanvas Release build: 0 warnings / 0 errors.
- ImageCanvasExternalConsumerSmoke Release build: 0 warnings / 0 errors.
- OpenVisionLab x64 Debug build: 0 warnings / 0 errors.
- OpenVisionLab x64 Release build: 0 warnings / 0 errors.
- RunUiPrecheck.ps1 Debug:
  wpf_imagecanvas_owned_mat_load=OK,
  wpf_shell_host_tool_input_image_load_save=OK.
- RunUiPrecheck.ps1 Release:
  같은 두 target가 OK로 통과했다.
- Invoke-RefactorAudit.ps1 -Verify:
  REFACTOR_AUDIT=PASS,
  CSharpFiles=768, CSharpLines=268447, CSharpBytes=12224124,
  PartialDeclarations=106, ViewModelFiles=32,
  DirectUiOrDialogFiles=1, ProjectCycles=0.
- structure-proof.txt, lifecycle-after.txt,
  keyboard-call-path-after.txt 및 keyboard-ownership-after.txt가
  controller owner, 이전 owner 제거, helper 재사용, 수명 순서를 확인한다.

실제 전체 데스크톱 키보드 interaction matrix(포커스, pressed, disabled,
theme/layout/DPI/monitor, WPF PreviewKeyDown과 WinForms host의 동시 입력)는
실행하지 않았다.

## Junior readability assessment

**PASS for this boundary.** 새 개발자는 ImageCanvasControl.KeyDown에서
controller로 들어와 Ctrl+Z/Y, Delete, C/V 분기와 VM callback을 따라갈 수
있고, 복사/붙여넣기 세부 구현은 RoiInteractionKeyDown에서 찾을 수 있다.
상태는 ViewModel, 이벤트 정책은 controller, WinForms event forwarding은
ImageCanvasControl로 나뉘어 파일명과 호출 경로가 책임을 설명한다.

전체 코드베이스가 즉시 자명해졌다는 뜻은 아니다. WPF PreviewKeyDown/KeyUp와
마우스 입력은 아직 ViewModel/View 경계에 남아 있으며 다음 독립 slice에서
별도로 다룬다.

Next priority: OVL-11 WPF PreviewKeyDown/KeyUp input ownership | Recommended model: gpt-5.6-terra | Reasoning effort: high.

## Next slice and non-reopen rule

다음 단일 slice는 OVL-11 WPF PreviewKeyDown/KeyUp 입력 소유권이다.
그 후 마우스 입력을 별도 boundary로 검토한다. 완료된 Mat 저장,
OpenFileDialog, SaveFileDialog, ContextMenu, WinForms keyboard owner는 새
결함·명시 요구사항 변경·입증된 책임 충돌이 없으면 다시 나누지 않는다.
Repository AGENTS.md의 one-slice 및 no-duplicate 규칙을 따른다.
