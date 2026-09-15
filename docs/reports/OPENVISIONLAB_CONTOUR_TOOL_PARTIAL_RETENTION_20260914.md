# OpenVisionLab Contour Tool View Partial retention — PL-0047

Status: `Complete`

## Scope

이번 cycle은 `ContourToolWpfView.xaml.cs`를 Contour 전용 property state,
teaching preview, area verification, shared single-input runtime, factory,
document, binding/test contract, registry, pipeline, and lifetime owner와
대조한 no-change 구조 감사다. Production Partial을 분리하거나 합치지
않았다. 현재 View는 required XAML namescope와 existing generic property-grid
collaborators를 조합하는 WPF adapter이며, 독립 state/lifetime/test seam 없이
다시 추출하면 기존 owner를 감싸는 forwarding wrapper가 된다.

## Ownership and boundary

| Concern | Current owner | Result |
| --- | --- | --- |
| XAML namescope, tool shell, Contour Learn caption/topic, verification-guide content | `ContourToolWpfView.xaml` + View Partial | Required WPF composition으로 유지. |
| Property-grid binding, layer/preview/output events, auto-preview, result review, presets, language, release | `VisionToolSingleInputPropertyToolController<ContourProperty>` | Existing generic controller가 shared facade와 disposal을 소유. |
| Mutable Contour parameters, range/epsilon/thickness normalization, summary, property snapshot | `ContourToolViewModel` via `VisionToolCompositionService` | Contour property writer와 ViewModel creation을 소유. |
| Property-grid presenter, binding flush, settings callback | `VisionToolPropertyGridPresenter<ContourProperty>` + generic runtime | View는 presenter/controller를 조합하고 state를 복제하지 않음. |
| Debounce, property-change policy, persistence callback, preview state | `VisionToolSingleInputPropertyToolRuntime<ContourProperty>` | Shared runtime이 edit ordering과 lifetime을 소유. |
| Threshold teaching request/review reset | `VisionToolThresholdTeachingPreviewController` | Request flag와 result-review clear를 View-owned adapter에서 분리해 소유. |
| Area teaching text, result metrics, verification status | `VisionToolAreaVerificationGuidePresenter<ContourProperty, ContourResult>` + `VisionToolAreaVerificationCriteriaText` | Inspection presentation policy가 View 밖의 existing presenter/criteria에 있음. |
| Contour settings/session and property-grid composition | `OpenVisionNativePropertyGridToolFactory` + `VisionToolCompositionService` | View Partial에는 session store 또는 persistence coupling이 없음. |
| Native document and input/output routing | `OpenVisionNativePropertyGridToolDocumentBuilder` + `OpenVisionNativeSingleInputToolDocumentBuilder` | Generic document/lifetime wiring이 builder owner에 있음. |
| Contour algorithm, result capture, overlay image | `OpenVisionNativeToolPreviewExecutor` + `OpenVisionNativeToolPreviewOverlayRenderer` | `ContourTool` construction과 algorithm policy가 View 밖에 있음. |
| Tool registration, pipeline step, final View release | `OpenVisionNativeToolRegistry` + `VisionPipelineStepBuilder` + `VisionToolSingleInputPropertyToolViewBase` | Registry/pipeline/base가 creation, step, and release contracts를 소유. |

`CurrentProperty`와 `ConsumeThresholdTeachingPreviewRequest()`는 View가
공개하는 narrow facade이며 Contour domain state나 persistence owner가 아니다.
`toolController.ShowAreaResultReview(...)` 또한 generic review owner에
verification presenter와 metric selectors를 전달하는 adapter call이다.

## Actual call path

```text
OpenVisionNativeToolRegistry (Contour)
  -> OpenVisionNativePropertyGridToolFactory.CreateContour
  -> property-session store and VisionToolCompositionService.CreateContourToolViewModel
  -> OpenVisionNativePropertyGridToolDocumentBuilder
  -> ContourToolWpfView + generic presenter/controller/runtime
     + area-verification presenter + threshold-teaching controller
  -> OpenVisionNativeSingleInputToolDocumentBuilder
  -> OpenVisionNativeToolPreviewExecutor.ExecuteContourPreview
     -> ContourTool algorithm -> Contour result -> overlay renderer
  -> VisionPipelineStepBuilder ContourProperty step (when pipelined)
  -> VisionToolSingleInputPropertyToolViewBase.DisposeView
```

The View owns only XAML composition, facade projection, and the direct
construction of WPF collaborators. No View Partial file I/O, dialog,
`System.IO`, OpenCV/Contour algorithm construction, persistence store, or
pipeline policy was found.

## Binding, public, and test contract

- `ContourToolWpfView.xaml` retains its `x:Class`, `toolShell` namescope,
  `VectorSquare` icon, `Learn Contour` caption, topic index `6`, and compact
  `VisionToolVerificationGuideView` content.
- `ISingleInputPropertyVisionToolWpfView<ContourProperty>` remains the generic
  factory/document contract; `CreateProperty()` delegates to the controller's
  presenter snapshot.
- `ConsumeThresholdTeachingPreviewRequest()` and `SetResultReview(...)` remain
  the existing preview/result facade used by the native preview path.
- `VisionToolSingleInputPropertyToolViewBase.DisposeView()` still releases
  View-owned resources before the generic controller; the View does not dispose
  the controller a second time.

## Why no production split is justified

The inspected code-behind contains only construction of existing WPF adapter
collaborators, narrow facade methods, and Contour-specific metric selector
wiring. Mutable Contour policy, normalization, presentation, teaching preview
state, property binding, preview scheduling, settings persistence, document
creation, algorithm execution, pipeline routing, and shared runtime lifetime
already have concrete owners. A new Contour ViewModel/service/interface/manager
or another Partial would duplicate or hide those owners. The smallest correct
structural result is to retain the XAML Partial and protect its owner map with a
source contract.

## Shortest code-reading order

Search once for `ContourToolWpfView`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/ContourToolWpfView.xaml.cs`
   and `.xaml` for namescope, controller attachment, facade, teaching, and
   result-review wiring.
2. `src/OpenVisionLab/UI/VisionTest/ViewModels/ContourToolViewModel.cs` and
   `src/OpenVisionLab/UI/VisionTest/Composition/VisionToolCompositionService.cs`
   for mutable state, normalization, summary, and ViewModel creation.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SingleInput/`,
   `Tooling/PropertyGrid/`, and `Tooling/Review/` for generic binding,
   debounce, persistence, teaching, review, and release ownership.
4. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativePropertyGridToolFactory.cs`,
   `OpenVisionNativePropertyGridToolDocumentBuilder.cs`, and
   `OpenVisionNativeSingleInputToolDocumentBuilder.cs` for creation and document
   routing.
5. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativeToolPreviewExecutor.cs`,
   `OpenVisionNativeToolPreviewOverlayRenderer.cs`,
   `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Runtime/OpenVisionNativeToolRegistry.cs`,
   and `src/OpenVisionLab/Core/Pipeline/Definition/VisionPipelineStepBuilder.cs`
   for algorithm, overlay, registration, pipeline, and final lifetime paths.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\contour-tool-partial-retention-20260914`

- `ContourToolPartialBoundaryContract` passed 11/11 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with 0
  warnings/errors.
- `RunUiPrecheck.ps1` passed `wpf_shell_host_contour_tool` and
  `wpf_openvision_learn_contour` with `check=OK`, layout/text/internal counts
  all zero, and fresh screenshots.
- The dynamic monitor/window probe selected the reported test monitor by
  topology and recorded one intersecting visible smoke window on `DISPLAY2`
  (1920x1080).
- Refactor audit, documentation index, readiness, ledger validation, JSON parse,
  and `git diff --check` are recorded as the final slice gates.

This is a source/contract and focused WPF smoke qualification only. Full theme,
Wide/Compact layout, 100/125/150/175/200% DPI, every input/keyboard visual
state, native dialog interaction, camera/SDK/GPU, and long-running native runtime
remain unverified: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Boundary lock

Do not reopen this retained Partial without a new requirement, reproducible
defect, failed criterion, or changed dependency/lifetime boundary. Continue
with another unprotected Tool/Learn Partial in the next scheduled cycle.
