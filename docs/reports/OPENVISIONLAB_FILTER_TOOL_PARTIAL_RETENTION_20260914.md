# OpenVisionLab Filter Tool View Partial retention — PL-0045

Status: `Complete`

## Scope

이번 cycle은 `FilterToolWpfView.xaml.cs`를 Filter 전용 interaction, kernel
input, text/summary, parameter guide, shared single-input runtime, factory,
document, binding/test contract, registry, and lifetime owner와 대조한
no-change 구조 감사다. Production Partial을 분리하거나 합치지 않았다.
현재 View는 required XAML namescope와 기존 custom-tool collaborators를
조합하는 WPF adapter이며, 독립 state/lifetime/test seam 없이 다시 추출하면
기존 owner를 감싸는 forwarding wrapper가 된다.

## Ownership and boundary

| Concern | Current owner | Result |
| --- | --- | --- |
| XAML namescope, tool shell, parameter layout, Filter Learn caption/topic | `FilterToolWpfView.xaml` + View Partial | Required WPF composition으로 유지. |
| Filter type/border selection events, parameter binding flush, mode panel visibility | `VisionToolFilterInteractionController` | Existing controller가 ComboBox event lifetime과 mode policy를 소유. |
| Kernel text input, W=H lock, 3/5/7 presets | `VisionToolKernelSizeController` | Existing shared controller가 TextChanged/Checked/Unchecked/Click attach/detach와 preset policy를 소유. |
| Debounced summary/Preview change ordering | `VisionToolParameterChangeController` + `VisionToolDebouncedPreviewScheduler` | View는 suppression callback과 scheduler를 조합할 뿐 policy를 복제하지 않음. |
| Localized labels and group headers | `FilterToolTextPresenter` | Existing presenter가 visible text projection을 소유. |
| Parameter guide focus/value refresh | `VisionToolCustomParameterGuideBinder` | Existing binder가 focus/value/language handler와 release를 소유. |
| Mutable Filter parameters, normalization, summary, settings save | `FilterToolViewModel` via `FilterToolPresenter` | ViewModel이 mutable writer와 property snapshot을 소유하고 presenter가 binding facade를 제공. |
| Layer bindings, preview slots, summary binding, result review, language/runtime disposal | `VisionToolSingleInputCustomToolController` + Runtime | Shared custom-tool owner가 shell/event/lifetime을 소유. |
| Settings load, `FilterTool` creation, preview/document/pipeline routing | `VisionToolCompositionService` + `OpenVisionNativeCustomToolFactory` + document builder | View Partial에는 persistence, algorithm construction, or pipeline policy가 없음. |
| Tool registration and final release | `OpenVisionNativeToolRegistry` + `VisionToolSingleInputCustomToolViewBase` | Registry creates the document; base releases View resources then shared controller. |

`suppressEvents`는 WPF re-entry guard를 existing controllers에 전달하기 위한
adapter state이며 Filter domain state나 persistence owner가 아니다.

## Actual call path

```text
OpenVisionNativeToolRegistry (VISION_MENU.Filter)
  -> OpenVisionNativeCustomToolFactory.CreateFilter
  -> VisionToolCompositionService.CreateFilterToolViewModel (settings Load)
  -> FilterToolPresenter
  -> FilterToolWpfView + FilterInteraction/Kernel/Text/Guide/Scheduler collaborators
  -> VisionToolSingleInputCustomToolController / Runtime
  -> OpenVisionNativeCustomToolDocumentBuilder
  -> OpenVisionNativeSingleInputToolDocumentBuilder
  -> FilterTool preview, tool execution, and VisionPipelineStepBuilder.FromFilterProperty
  -> VisionToolSingleInputCustomToolViewBase.DisposeView
```

The View owns only XAML composition, facade projection, and its directly-created
WPF collaborator release. No View Partial file I/O, dialog, OpenCV construction,
or Filter algorithm execution was found.

## Binding, public, and test contract

- `FilterToolWpfView.xaml` retains its `x:Class`, `toolShell`, parameter
  namescope, `Learn Filter` caption, topic index `3`, and TwoWay bindings for
  `FilterType`, `BorderType`, `KernelWidth`, `KernelHeight`, and other Filter
  properties.
- `ISingleInputPropertyVisionToolWpfView<FilterToolProperty>` remains the
  generic factory/document contract; `CreateProperty()` flushes the existing
  controls and returns the presenter snapshot.
- `AttachToolController` continues to provide the shared layer, Preview, output
  layer, Add Pipeline, summary, result-review, language, and test facade.
- `DisposeToolResources` disposes the parameter-guide binder, detaches Filter
  and kernel controllers, and disposes the debounced scheduler. The base then
  disposes the shared custom-tool controller; the View does not duplicate it.

## Why no production split is justified

The inspected code-behind contains only construction of existing WPF adapter
collaborators, facade methods, binding/summary composition, and View-owned
resource cleanup. Mutable Filter policy, presentation, input event lifetime,
preview scheduling, parameter help, settings persistence, pipeline routing,
and shared runtime lifetime already have concrete owners. A new Filter
ViewModel/service/interface/manager or another Partial would duplicate or hide
those owners. The smallest correct structural result is to retain the XAML
Partial and protect its owner map with a source contract.

## Shortest code-reading order

Search once for `FilterToolWpfView`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/FilterToolWpfView.xaml.cs`
   and `.xaml` for namescope, controller attachment, facade, and bindings.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/VisionToolFilterInteractionController.cs`
   and `VisionToolKernelSizeController.cs` for event, panel, lock, and preset
   ownership.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Presentation/FilterToolTextPresenter.cs`,
   `Tooling/PropertyGrid/VisionToolParameterChangeController.cs`, and
   `Tooling/Preview/VisionToolDebouncedPreviewScheduler.cs` for visible text,
   change ordering, and debounce lifetime.
4. `src/OpenVisionLab/UI/VisionTest/ViewModels/FilterToolViewModel.cs` and
   `Wpf/Tooling/PropertyGrid/VisionToolParameterPresenters.cs` for mutable
   state, normalization, summary, and binding facade.
5. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SingleInput/` plus
   `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeCustomToolFactory.cs`
   and `OpenVisionNativeCustomToolDocumentBuilder.cs` for runtime, creation,
   routing, and release.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\filter-tool-partial-retention-20260914`

- `FilterToolPartialBoundaryContract` passed 9/9 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with 0 warnings/errors.
- `RunUiPrecheck.ps1` passed the Filter/Morphology layout guard and the Filter
  shell-host smoke target with `check=OK`, layout/text/internal counts all zero,
  and fresh screenshots.
- The dynamic monitor/window probe selected the reported test monitor by
  topology and recorded one intersecting visible smoke window.
- Refactor audit, documentation index, readiness, ledger validation, JSON parse,
  and `git diff --check` passed for this slice.

This is a source/contract and focused WPF smoke qualification only. Full theme,
Wide/Compact layout, 100/125/150/175/200% DPI, every input/keyboard visual
state, native dialog interaction, camera/SDK/GPU, and long-running native runtime
remain unverified: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Boundary lock

Do not reopen this retained Partial without a new requirement, reproducible
defect, failed criterion, or changed dependency/lifetime boundary. Continue
with another unprotected Tool/Learn Partial in the next scheduled cycle.
