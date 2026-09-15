# OVL-11 ImageCanvas WPF 키보드 입력 owner — 2026-09-08

## Status

**Complete** for one independently verifiable WPF PreviewKeyDown/KeyUp
ownership boundary. This report records the current Dev checkout only. No
commit, push, Original checkout, release, or deployment action was performed.

## Scope

`RoiImageCanvasViewModel.Commands.cs`에 있던 WPF `KeyEventArgs`와
`Keyboard.Modifiers` 정책을 `RoiImageCanvasWpfKeyboardInputController`로
이동했다. 기존 public `PreviewKeyDownCommand`와 `KeyUpCommand`는 유지하고,
View는 기존처럼 WPF 이벤트를 구독해 command로 전달한다.

보존한 계약:

- Control modifier가 포함된 PreviewKeyDown은 기존처럼 아무 동작도 하지
  않는다.
- Control 없이 Delete를 누르면 기존 `RemoveSelectedOverlay()`를 호출하고
  `args.Handled = true`를 설정한다.
- F2와 Enter PreviewKeyDown은 기존처럼 no-op이다.
- Control 상태의 C/V/S KeyUp은 기존처럼 no-op이며 다른 KeyUp도 처리하지
  않는다.
- `RoiImageCanvasView.PreviewKeyDown`/`KeyUp` 구독과 `Dispose()` 해제,
  public command 이름, ROI 상태 소유권을 변경하지 않았다.
- WinForms keyboard owner, 마우스 입력, ContextMenu/dialog host,
  Mat/Layer/ImageSpace, Recipe/XML, 명시적 Preview/Run, SDK/외부 consumer
  계약은 이 slice에서 변경하지 않았다.

## Ownership and lifecycle

현재 호출 경로는 다음과 같다.

```text
RoiImageCanvasView.PreviewKeyDown
  -> ImageCanvasView_PreviewKeyDown
  -> RoiImageCanvasViewModel.PreviewKeyDownCommand
  -> RoiImageCanvasWpfKeyboardInputController.HandlePreviewKeyDown
  -> RoiImageCanvasViewModel.RemoveSelectedOverlay (Delete)

RoiImageCanvasView.KeyUp
  -> ImageCanvasView_KeyUp
  -> RoiImageCanvasViewModel.KeyUpCommand
  -> RoiImageCanvasWpfKeyboardInputController.HandleKeyUp (기존 no-op 정책)
```

`RoiImageCanvasView`가 WPF 이벤트 수명과 전달을 소유하고,
`RoiImageCanvasViewModel`은 public command facade와 ROI mutable state를
소유한다. Controller는 WPF key/modifier 정책만 소유하며 이벤트를 직접
구독하지 않는 stateless owner다. ViewModel은 constructor에서 controller를
`InitCommand()`보다 먼저 조합해 command delegate가 유효하도록 한다.

## Changed files

- `src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiImageCanvasWpfKeyboardInputController.cs`
  - WPF PreviewKeyDown/KeyUp 정책과 Delete callback을 소유한다.
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`
  - WPF policy owner를 조합하고 callback으로 ROI 상태를 연결한다.
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.Commands.cs`
  - 기존 public commands를 WPF controller delegate로 연결하고 ViewModel의
    WPF key policy handler를 제거한다.

View의 event forwarding과 `Dispose()` 해제는 소유권을 유지해야 하므로
`RoiImageCanvasView.xaml.cs`는 이 slice에서 수정하지 않았다.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-wpf-keyboard-input-20260908`

- 임시 WPF keyboard contract harness:
  `WPF_KEYBOARD_CONTRACT=PASS`. Controller를 reflection으로 생성해 Delete의
  callback/Handled와 F2·KeyUp no-op을 실행했다. 하네스 소스와 로그는
  evidence root의 `wpf-keyboard-contract`에 있으며 repository source에는
  추가하지 않았다.
- ImageCanvas Debug/Release build: 0 warnings / 0 errors.
- ImageCanvasExternalConsumerSmoke Release build: 0 warnings / 0 errors.
- OpenVisionLab x64 Debug/Release build: 0 warnings / 0 errors.
- `RunUiPrecheck.ps1` Debug/Release:
  `wpf_imagecanvas_owned_mat_load`와
  `wpf_shell_host_tool_input_image_load_save`가 모두 `OK`였다.
- `Invoke-RefactorAudit.ps1 -Verify`:
  `REFACTOR_AUDIT=PASS`, CSharpFiles=769, CSharpLines=268466,
  CSharpBytes=12224956, PartialDeclarations=106, ViewModelFiles=32,
  DirectUiOrDialogFiles=1, ProjectCycles=0.
- `structure-proof.txt`, `lifecycle-after.txt`,
  `wpf-keyboard-call-path-after.txt`, `wpf-keyboard-ownership-after.txt`가
  이전 ViewModel handler 제거, controller 위임, View event lifecycle,
  callback 방향을 확인한다.

실제 데스크톱 WPF 키보드 interaction matrix(포커스, pressed, disabled,
popup, theme/layout/DPI/monitor 및 Control modifier를 포함한 물리 입력)는
실행하지 않았다. 따라서 이 보고서는 소스·계약·빌드와 기존 focused UI
precheck 범위의 완료 기록이다.

## Junior readability assessment

**PASS for this boundary.** 새 개발자는
`RoiImageCanvasView`의 이벤트 전달, public command facade,
`RoiImageCanvasWpfKeyboardInputController`의 key/modifier 정책,
`RemoveSelectedOverlay`의 상태 변경을 순서대로 찾을 수 있다. View event
수명과 ViewModel 상태가 controller 정책과 섞이지 않고, WinForms keyboard
owner도 별도 파일로 남아 있어 입력 종류별 책임이 구분된다.

전체 코드베이스가 즉시 자명해졌다는 뜻은 아니다. 마우스 입력과 Shell/Learn
공통 조합은 아직 남아 있으며, WPF 실제 키보드 runtime matrix는 별도
qualification 범위다.

`소스 코드 기준 검토 완료 / 실제 Runtime WPF keyboard UI 검증 필요`.

Next priority: OVL-11 mouse input ownership | Recommended model:
gpt-5.6-terra | Reasoning effort: high.

## Next slice and non-reopen rule

다음 단일 slice는 OVL-11 마우스 입력 소유권이다. 완료된 Mat 저장,
OpenFileDialog, SaveFileDialog, ContextMenu, WinForms keyboard 및 이번 WPF
keyboard owner는 새 결함·명시 요구사항 변경·입증된 책임 충돌이 없으면 다시
나누지 않는다. Repository `AGENTS.md`의 one-slice 및 no-duplicate 규칙을
따른다.
