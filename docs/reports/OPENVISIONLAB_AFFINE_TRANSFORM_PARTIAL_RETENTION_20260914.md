# OpenVisionLab Affine Transform Tool View Partial retention — PL-0043

Status: `Complete`

## Scope

이번 cycle은 `AffineTransformToolWpfView.xaml.cs`를 factory, composition,
ViewModel, shared property controller, preview executor, result presenter, XAML
binding, test facade, and base lifetime owner와 대조한 no-change 구조 감사다.
View Partial에는 production 변경을 추가하지 않았다. 현재 View는 required XAML
namescope와 `VisionToolSingleInputPropertyToolController<AffineTransformProperty>`
를 연결하는 얇은 WPF adapter이며, 별도 state/lifetime/test seam 없이 분리하면
기존 controller 계약을 감싸는 forwarding wrapper가 된다.

## Ownership and boundary

| Concern | Current owner | Result |
| --- | --- | --- |
| XAML namescope, tool shell, Learn caption/topic | `AffineTransformToolWpfView.xaml` + View Partial | Retained required WPF composition. |
| Mutable affine property and summary | `AffineTransformToolViewModel` | Existing ViewModel returns a defensive `DeepCopy()` and owns summary policy. |
| PropertyGrid binding, layer/preview events, result review facade, test configuration, controller events | `VisionToolSingleInputPropertyToolController<AffineTransformProperty>` | Existing generic controller remains the concrete interaction/lifetime owner. |
| Matrix/coverage/determinant result explanation | `AffineTransformResultReviewPresenter` | Existing presentation policy remains outside the View's Partial. |
| Affine algorithm execution and overlay image | `OpenVisionNativeToolPreviewExecutor` + `OpenVisionNativeToolPreviewOverlayRenderer` | View only exposes property/result facade; it does not construct or execute the algorithm. |
| Property load/save, ViewModel/View creation, native document composition | `OpenVisionNativePropertyGridToolFactory` + `VisionToolCompositionService` + `OpenVisionNativePropertyGridToolDocumentBuilder` | Existing document/factory owners remain responsible for persistence and creation. |
| Tool registration and final release | `OpenVisionNativeToolRegistry` + `VisionToolSingleInputPropertyToolViewBase` | Existing registry and base `DisposeView` path remain unchanged. |

The View's `toolController` field is the controller attachment required by the
base View contract. It does not duplicate affine state; `CreateProperty()` and
`ConfigurePropertyForTest()` delegate to the controller/ViewModel contract.

## Actual call path

```text
OpenVisionNativeToolRegistry (VISION_MENU.AffineTransform)
  -> OpenVisionNativePropertyGridToolFactory.CreateAffineTransform
  -> OpenVisionNativeToolPropertySessionStore.GetOrLoad
  -> VisionToolCompositionService.CreateAffineTransformToolViewModel
  -> VisionToolPropertyGridPresenter<AffineTransformProperty>
  -> AffineTransformToolWpfView
  -> VisionToolSingleInputPropertyToolController<AffineTransformProperty>
  -> OpenVisionNativeToolPreviewExecutor.ExecuteAffineTransformPreview
  -> AffineTransformResultReviewPresenter / overlay renderer
  -> OpenVisionNativePropertyGridToolDocumentBuilder
  -> VisionToolSingleInputPropertyToolViewBase.DisposeView
```

Property persistence is wired by the existing factory's
`persistSelectedObject` callback. The Preview executor owns
`new AffineTransformTool()` and the transformed-image overlay. No View Partial
file I/O, dialog, image, algorithm, or persistence owner was found.

## Binding, public, and test contract

- `AffineTransformToolWpfView.xaml` retains its `x:Class`, `toolShell`, Learn
  caption (`Learn Affine Transform`), and topic index `15`.
- `CreateProperty()` remains the preview/property-factory facade and delegates to
  the shared controller.
- `ResultReviewTextForTest`, `SetResultReview`, and
  `ConfigurePropertyForTest` remain available to the native document and Shell
  test surfaces.
- The base View continues to release View-owned resources first and then the
  attached generic controller; the Affine Partial does not duplicate disposal.

## Why no production split is justified

The inspected code-behind has no independent mutable state, persistence policy,
algorithm construction, or file/dialog workflow. Every non-presentation concern
already routes through a concrete ViewModel, generic property controller,
result presenter, preview executor, factory, composition service, or base View.
A new Affine ViewModel/service/interface/manager or forwarding Partial would
only rename or duplicate an existing owner. The smallest correct structural
result is to retain this XAML Partial and protect its ownership boundary with a
source contract.

## Shortest code-reading order

Search once for `AffineTransformToolWpfView`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/AffineTransformToolWpfView.xaml.cs`
   and `.xaml` for controller attachment and facade/binding contracts.
2. `src/OpenVisionLab/UI/VisionTest/ViewModels/AffineTransformToolViewModel.cs`
   for mutable property and summary ownership.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SingleInput/VisionToolSingleInputPropertyToolController.cs`
   and `VisionToolSingleInputPropertyToolViewBase.cs` for PropertyGrid,
   preview/event, test, and release ownership.
4. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativePropertyGridToolFactory.cs`
   and `src/OpenVisionLab/UI/VisionTest/Composition/VisionToolCompositionService.cs`
   for load/create/persist composition.
5. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativeToolPreviewExecutor.cs`
   and `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Review/AffineTransformResultReviewPresenter.cs`
   for algorithm and result presentation.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\affine-transform-partial-retention-20260914`

- `AffineTransformPartialBoundaryContract` passed 9/9 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with 0 warnings/errors.
- `PipelineViewerScreenshotSmoke` Debug build passed with 0 warnings/errors.
- `wpf_shell_host_affine_transform_tool` WPF precheck passed with `check=OK`,
  layout/text/internal counts all zero, and a fresh screenshot.
- Sequential dynamic monitor probe passed for the Affine Transform smoke: one
  detected `DISPLAY2` monitor (`1920x1080`) and one intersecting visible smoke
  window.
- Repository readiness, RefactorAudit, DocumentationIndex, issue validation,
  and diff checks are recorded under this evidence root.

This is a source/contract and focused WPF smoke qualification only. Full theme,
Wide/Compact layout, 100/125/150/175/200% DPI, every input/keyboard visual
state, native dialog interaction, camera/SDK/GPU, and long-running native runtime
remain unverified: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Boundary lock

Do not reopen this retained Partial without a new requirement, reproducible
defect, failed criterion, or changed dependency/lifetime boundary. Continue with
another unprotected Tool/Learn Partial in the next scheduled cycle.
