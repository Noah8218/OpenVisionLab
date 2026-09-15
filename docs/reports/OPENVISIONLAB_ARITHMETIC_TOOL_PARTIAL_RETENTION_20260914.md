# OpenVisionLab Arithmetic Tool View Partial retention — PL-0044

Status: `Complete`

## Scope

이번 cycle은 `ArithmeticToolWpfView.xaml.cs`를 XAML, double-input runtime,
interaction, text/summary, preview, factory, document, binding/test contract,
registry, and lifetime owner와 대조한 no-change 구조 감사다. Production
Partial을 분리하거나 합치지 않았다. 현재 View는 required XAML namescope와
기존 double-input controller를 조합하는 WPF adapter이며, 독립 state/lifetime/
test seam 없이 다시 추출하면 기존 controller를 감싸는 forwarding wrapper가
된다.

## Ownership and boundary

| Concern | Current owner | Result |
| --- | --- | --- |
| XAML namescope, tool shell, parameter layout, Learn caption/topic | `ArithmeticToolWpfView.xaml` + View Partial | Required WPF composition으로 유지. |
| Operation/source/constant/offset editor state, event attach/detach, input validation, persisted-settings projection, visibility policy | `ArithmeticToolInteractionController` | Existing interaction owner가 WPF controls와 Arithmetic settings projection을 소유. |
| Localized labels, Run Offset caption, summary text | `ArithmeticToolTextPresenter` | Existing presenter가 presentation policy를 소유. |
| Debounced mode-specific Preview/Offset scheduling | `ArithmeticToolPreviewController` | Existing scheduler/controller가 실행 요청과 release를 소유. |
| Layer bindings, command routing, preview slots, language/runtime disposal | `VisionToolDoubleInputCustomToolController` → `VisionToolDoubleInputViewModel`/`VisionToolDoubleInputViewBinder` | Shared double-input owners가 binding, command, event, preview, disposal을 소유. |
| Settings load/save, operation list composition, native document routing, Arithmetic pipeline step | `OpenVisionNativeArithmeticDocumentFactory` + `OpenVisionNativeToolDocument` | View Partial에는 persistence/file policy가 없음. |
| Tool registration and final release | `OpenVisionNativeToolRegistry` + `VisionToolDoubleInputCustomToolViewBase` | Registry creates the document; base releases View resources then shared controller. |

`suppressEvents`는 controller에 전달되는 WPF re-entry guard이며, Arithmetic
domain state나 persistence owner가 아니다. `CaptureSettings`와
`ApplyPersistedSettings`도 기존 `ArithmeticToolInteractionController`로
위임한다.

## Actual call path

```text
OpenVisionNativeToolRegistry (VISION_MENU.Arithmetic)
  -> OpenVisionNativeArithmeticDocumentFactory.Create
  -> ArithmeticToolWpfView + ArithmeticToolInteractionController/TextPresenter/PreviewController
  -> VisionToolDoubleInputCustomToolController
  -> VisionToolDoubleInputViewModel / ViewBinder / shared shell runtime
  -> OpenVisionNativeToolDocument (BindArithmetic, layer/preview/pipeline routing)
  -> existing settings store and Arithmetic pipeline step
  -> VisionToolDoubleInputCustomToolViewBase.DisposeView
```

The factory owns settings `Load/Save` and operation-list composition. The
document owns layer routing, Preview/Offset event handling, output-layer and
pipeline composition. No View Partial file I/O, dialog, OpenCV construction, or
algorithm execution was found.

## Binding, public, and test contract

- `ArithmeticToolWpfView.xaml` retains its `x:Class`, `toolShell`, parameter
  namescope, Learn caption, and topic index `14`.
- `IArithmeticVisionToolWpfView` remains the public composition contract for
  layer/preview/Offset/Add Pipeline events, operation selection, constants, and
  offset values.
- `ParameterChanged`, `SetOperationList`, `CaptureSettings`, and all layer/
  preview facade members remain available to the factory, document, and smoke
  paths.
- `DisposeToolResources` detaches the Arithmetic interaction controller and
  disposes the preview scheduler; the base then disposes the shared double-input
  controller. The View does not duplicate shared-controller disposal.

## Why no production split is justified

The inspected code-behind contains only construction of existing WPF adapter
collaborators, facade properties/methods, and View-owned resource cleanup. The
mutable editor policy, summary projection, preview scheduling, command/binding
runtime, settings persistence, pipeline routing, and lifetime owners already
have concrete boundaries. A new Arithmetic ViewModel/service/interface/manager
or another Partial would duplicate or hide those owners. The smallest correct
structural result is to retain the XAML Partial and protect its owner map with a
source contract.

## Shortest code-reading order

Search once for `ArithmeticToolWpfView`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/ArithmeticToolWpfView.xaml.cs`
   and `.xaml` for controller attachment and facade/binding contracts.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/ArithmeticToolInteractionController.cs`
   for editor state, settings projection, event lifetime, and visibility policy.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Presentation/ArithmeticToolTextPresenter.cs`
   and `Tooling/Preview/ArithmeticToolPreviewController.cs` for presentation
   and debounce ownership.
4. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/DoubleInput/` controller,
   ViewModel, binder, and base View for command/binding/runtime/release ownership.
5. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeArithmeticDocumentFactory.cs`
   and `OpenVisionNativeToolDocument.cs` for settings, routing, and pipeline
   composition.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\arithmetic-tool-partial-retention-20260914`

- `ArithmeticToolPartialBoundaryContract` passed 9/9 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with 0 warnings/errors.
- `RunUiPrecheck.ps1` passed both `wpf_arithmetic_tool_learn_button` and
  `wpf_layer_selection_arithmetic_tool` with `check=OK`, layout/text/internal
  counts all zero, and fresh screenshots.
- The dynamic monitor/window probe passed on one detected `DISPLAY2` monitor
  (`1920x1080`) with one intersecting visible smoke window.

This is a source/contract and focused WPF smoke qualification only. Full theme,
Wide/Compact layout, 100/125/150/175/200% DPI, every input/keyboard visual
state, native dialog interaction, camera/SDK/GPU, and long-running native runtime
remain unverified: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Boundary lock

Do not reopen this retained Partial without a new requirement, reproducible
defect, failed criterion, or changed dependency/lifetime boundary. Continue with
another unprotected Tool/Learn Partial in the next scheduled cycle.
