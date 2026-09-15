# OVL-11 ImageCanvas 마우스 입력 owner — 2026-09-08

## Status

**Complete** for one independently verifiable ImageCanvas mouse-input
ownership boundary. This report records the current Dev checkout only. No
commit, push, Original checkout, release, or deployment action was performed.

## Scope

`RoiImageCanvasViewModel`에 있던 ImageCanvasControl 마우스 이벤트 수명과
ROI 마우스 orchestration을 `RoiImageCanvasMouseInputController`로 이동했다.
ViewModel은 mutable ROI/measurement state와 기존 domain/UI callback을
명시적으로 제공하고, controller는 마우스 이벤트 구독·해제와 기존 helper
호출 순서를 소유한다.

보존한 계약:

- `ImageCanvasControl`의 public mouse event와 OpenGL control forwarding은
  변경하지 않았다.
- Left drawing/edit/move/measure, Right-click mode reset/context-menu 호출,
  Middle-button pan, wheel zoom, mouse-leave pan reset, drawing timer 호출의
  기존 순서를 유지했다.
- `RoiInteractionMouseDown`, `RoiInteractionMouseMove`,
  `RoiInteractionMouseUp`, `RoiInteractionCursor` helper를 재사용했다.
- `RoiImageCanvasViewModel`의 public properties/events/commands, ROI snapshot,
  Mat/Layer/ImageSpace, Recipe/XML, explicit Preview/Run 계약은 이 slice에서
  변경하지 않았다.
- Mat 저장, OpenFileDialog, SaveFileDialog, ContextMenu, WinForms keyboard,
  WPF keyboard owner는 완료 경계로 유지했으며 다시 분리하지 않았다.

## Ownership and lifecycle

현재 호출 경로는 다음과 같다.

```text
ImageCanvasControl mouse event
  -> RoiImageCanvasMouseInputController.OnMouseDown/Move/Up/Wheel/Leave
  -> existing RoiInteractionMouse* / RoiInteractionCursor helper
  -> ViewModel state accessors and explicit callbacks
  -> existing ROI/context-menu/snapshot/property-change behavior
```

`RoiImageCanvasMouseInputController`는 ImageCanvasControl의
`MouseDoubleClicked`, `MouseClicked`, `MouseDown`, `MouseMove`, `MouseUp`,
`MouseLeave`, `MouseWheel` 구독과 해제를 모두 소유한다. ViewModel의
`InitEvent()`/`ReleaseEvents()`에는 Load, Resized, Draw만 남아 있다.
`Dispose()`는 ViewModel이 ImageCanvasControl을 dispose하기 전에 mouse
controller를 해제하며, controller 자체는 idempotent disposal을 보장한다.

Controller가 소유하지 않는 상태는 callback으로 연결한다.

- selected/drawing rectangle, measurement, pan 상태, mouse-down 위치와
  ImageCanvas image size: ViewModel state accessors
- snapshot 시작/완료, ROI mouse-up/edit/add, 단일 draw 교체,
  pixel-property 갱신, ContextMenu mode policy, drawing timer: 기존
  ViewModel callbacks
- Focus, Drag & Drop, DPI, Window, animation, rendering: 기존 View/adapter와
  `ImageCanvasControl` 경계

## Changed files

- `src/Libraries/OpenVisionLab.ImageCanvas/RoiInteraction/RoiImageCanvasMouseInputController.cs`
  - 마우스 이벤트 lifetime, 입력 정책, 기존 ROI helper orchestration을
    독립 owner로 구현했다.
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`
  - controller를 조합하고 state/callback delegate를 연결했으며 기존 mouse
    handler와 mouse-only helper를 제거했다. Load/Resized/Draw lifecycle은
    그대로 유지한다.
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.Refresh.cs`
  - ImageCanvasControl dispose 전에 mouse controller를 해제한다.
- `src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.Commands.cs`
  - mouse owner로 이동한 사용되지 않는 right-click wrapper와 import를
    제거했다.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
  `docs/admin/CODEBASE_STRUCTURE.md`,
  `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`,
  `docs/LLM_DOCUMENT_INDEX.json`,
  `docs/reports/OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md`
  - 현재 owner/call path, 검증 증거, 주니어 평가와 다음 우선순위를
    갱신한다.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl11-mouse-input-20260908`

- Temporary mouse contract harness: `MOUSE_INPUT_CONTRACT=PASS`. Reflection으로
  controller를 조합해 7개 public mouse event가 각각 한 번씩 구독되고,
  `Dispose()` 후 원래 invocation count로 돌아오며, 두 번째 `Dispose()`가
  추가 변경을 만들지 않는지 확인했다.
- ImageCanvas Debug/Release build: 0 warnings / 0 errors.
- ImageCanvasExternalConsumerSmoke Release build: 0 warnings / 0 errors.
- OpenVisionLab x64 Debug/Release build: 0 warnings / 0 errors.
- `RunUiPrecheck.ps1` Debug/Release with
  `wpf_imagecanvas_owned_mat_load,wpf_shell_host_tool_input_image_load_save`:
  both targets `OK`, `WARN 0`, `NG 0` in each configuration.
- `Invoke-RefactorAudit.ps1 -Verify`:
  `REFACTOR_AUDIT=PASS`, `CSharpFiles=770`, `CSharpLines=268637`,
  `CSharpBytes=12232911`, `PartialDeclarations=106`,
  `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`.
- `structure-proof-check.log`: `MOUSE_STRUCTURE_PROOF=PASS`; the ViewModel no
  longer contains mouse subscriptions/handlers, the controller owns all seven
  subscriptions and removals, and disposal precedes ImageCanvasControl dispose.
- Source call-path and lifecycle excerpts are in
  `mouse-call-path-after.txt`, `mouse-ownership-after.txt`,
  `lifecycle-after.txt`, and `structure-proof.txt`.

The physical desktop mouse matrix (focus, hover, pressed, selected, disabled,
popup, theme/layout, supported DPI, monitor placement, and drag/pan/measure
input) was not executed. The focused precheck exercises representative
ImageCanvas/Shell rendering and load/save paths, not a full physical mouse
qualification.

## Junior readability assessment

**PASS for this boundary.** A junior developer can start at the
`ImageCanvasControl` event source, open the single
`RoiImageCanvasMouseInputController` to see button/mode policy, then follow
the explicit ViewModel callbacks for ROI state, ContextMenu, snapshots, and
property updates. Existing low-level ROI helpers remain discoverable in the
same `RoiInteraction` folder, and ViewModel `InitEvent` now shows only the
three lifecycle/rendering events it still owns.

The whole codebase is not yet immediately self-explanatory: Learn/Shell shared
composition and the remaining ViewModel `Directory` policy still require the
current Handoff and structure map. This slice does not claim the physical
desktop mouse runtime matrix.

`소스 코드 기준 검토 완료 / 실제 Runtime ImageCanvas mouse UI 검증 필요`.

## Next slice and non-reopen rule

The next ordered priority is **OVL-09 Learn Window/Shell residual composition**.
The completed Mat/save-dialog/open-dialog/ContextMenu/WinForms keyboard/WPF
keyboard/mouse owners and P0 audit instrumentation must not be repartitioned,
renamed, or moved without a new defect, changed explicit requirement, or
proven responsibility conflict. A new model or heartbeat must read this
record and the current Handoff before selecting work; no parallel or duplicate
slice is authorized.

