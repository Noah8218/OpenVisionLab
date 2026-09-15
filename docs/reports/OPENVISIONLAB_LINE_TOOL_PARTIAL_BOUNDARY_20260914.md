# OpenVisionLab Line Tool View Partial boundary — PL-0041

Status: `Complete`

## Scope

이번 cycle은 `LineToolWpfView.xaml.cs`의 `ApplySampleLinePair` 안에 남아 있던
LineGauge property 복사 정책을 기존 `VisionPipelineLinePropertyAdapter`로
이동했다. View는 XAML namescope, PropertyGrid/controller 조합, sample 적용 후
purpose/persistence/overlay/summary 순서, signal/review presentation과 기존
public/test facade를 유지한다. 새 ViewModel, service interface, forwarding
Partial, 두 번째 property codec은 추가하지 않았다.

동작은 보존했다. 공통 OpenCV threshold/ROI/mask 값과 Line edge/scan/fit/draw
값을 모두 복사하고, `CvROIS`/`CvMASKS`는 이전과 동일하게 방어적 list copy를
유지한다. Sample 적용은 기존처럼 Preview를 직접 실행하지 않으며, 기존
`LineToolPresenter.PersistProperties` 및 PropertyGrid/overlay 갱신 순서를
그대로 따른다. PL-0027에서 완료한 persistence owner는 다시 열지 않았다.

## Ownership and boundary

| Concern | Current owner | Intended owner / result |
| --- | --- | --- |
| XAML namescope, PropertyGrid host, controls, signal/review presentation | `LineToolWpfView` Partial | Retained required WPF composition and presentation plumbing. |
| Sample A/B LineGauge value projection | Previously the View's `CopyLineProperty` body | `VisionPipelineLinePropertyAdapter.ApplySampleProperty`. |
| Pipeline Line/LineDistance/LineIntersection property creation and step conversion | `VisionPipelineLinePropertyAdapter` | Retained existing concrete property adapter; it now also owns sample projection. |
| Mutable Line A/B properties and normalization | `LineToolViewModel` through `LineToolPresenter` | Unchanged. |
| PropertyGrid change, visibility, preview scheduling, ROI/purpose interaction | `VisionToolPropertyGridHost`, `LineToolInteractionController`, `LineToolPreviewController` | Unchanged. |
| Native property persistence | `LineToolPresenter.PersistProperties` wired by `OpenVisionNativeCustomToolFactory` | Unchanged from PL-0027. |
| Sample dispatch and purpose selection | `OpenVisionNativeToolDocument.ApplySampleStepParameters` | Unchanged; it still creates the pair through the adapter and calls `ApplySampleLinePair`. |
| Creation and final resource release | `OpenVisionNativeCustomToolFactory` and `VisionToolSingleInputPropertyToolViewBase` | Unchanged. |

The moved body was an independently testable mapping policy in a View that already
had a concrete adapter with the same Line property family and pipeline conversion
responsibility. Reusing that adapter keeps one field mapping owner and preserves
the existing dependency direction. A new generic mapper or interface was not
needed.

## Actual call path

```text
OpenVisionNativeCustomToolFactory.CreateLine
  -> VisionToolCompositionService.CreateLineToolViewModel
  -> LineToolPresenter(viewModel, persistProperties)
  -> LineToolWpfView
  -> OpenVisionNativeToolDocument.ApplySampleStepParameters
  -> VisionPipelineStepPropertyMapper.CreateProperty
  -> VisionPipelineLinePropertyAdapter.TryCreateLineGaugePair
  -> LineToolWpfView.ApplySampleLinePair
  -> VisionPipelineLinePropertyAdapter.ApplySampleProperty (Line A/B)
  -> LineToolInteractionController.SetPurposeForTest
  -> LineToolPresenter.PersistProperties
  -> PropertyGrid refresh / input ROI overlay / summary / result cleanup
```

`ApplySampleProperty` only writes the supplied target property. It does not create
views, access WPF controls, persist files, schedule Preview, or own target
lifetime. The View remains the owner of the post-copy presentation sequence.

## State, binding, and public/test contract

- `LineToolWpfView.xaml` remains the required XAML Partial and its existing
  PropertyGrid bindings, purpose controls, signal inspector, result review, and
  Learn/guide surfaces are unchanged.
- `LineToolViewModel.LineAProperty`/`LineBProperty` remain the mutable writers;
  `LineToolPresenter` remains the facade and persistence seam.
- Existing public/test contracts remain available: `CreateProperty`,
  `CreateLineAProperty`, `CreateLineBProperty`, `ApplySampleLinePair`, purpose and
  ROI test configuration, result review, signal evidence and preview facades.
- `CvROIS` and `CvMASKS` are cloned into the target property. Later source-list
  mutation cannot change the active ViewModel property.
- The View's sample callback ordering and `LineToolPresenter.PersistProperties`
  call path are unchanged; no second persistence writer was introduced.

## Shortest code-reading order

Search once for `ApplySampleProperty`, then read:

1. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeToolDocument.cs`
   — sample-step caller and Line purpose selection.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/LineToolWpfView.xaml.cs`
   — XAML composition, post-copy sequence, and public/test facade.
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineLinePropertyAdapter.cs`
   — Line pair creation, pipeline conversion, and complete sample projection.
4. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Presentation/LineToolPresenter.cs`
   and `src/OpenVisionLab/UI/VisionTest/ViewModels/LineToolViewModel.cs`
   — mutable A/B state, normalization, summary, and persistence seam.
5. `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/LineToolInteractionController.cs`,
   `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Preview/LineToolPreviewController.cs`,
   and `VisionToolPropertyGridHost.cs` — interaction, overlay, PropertyGrid, and
   Preview ownership.
6. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeCustomToolFactory.cs`
   and `src/OpenVisionLab/UI/VisionTest/Composition/VisionToolCompositionService.cs`
   — creation, persistence composition, and release ownership.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\line-tool-partial-boundary-20260914`

- `LineToolPartialBoundaryContract` passed `4/4` in Debug and Release. It proves
  View-to-adapter routing, complete field projection, sample caller/composition
  preservation, representative runtime values, and defensive ROI/mask list
  ownership.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with `0` warnings and
  `0` errors.
- `PipelineViewerScreenshotSmoke` Debug build passed with `0` errors and one
  pre-existing nullable warning in unrelated smoke source.
- `RunUiPrecheck.ps1 -Targets wpf_shell_host_line_tool,wpf_shell_host_line_measure_tool,
  wpf_shell_host_line_intersection_tool,wpf_shell_host_recipe_line_pair_properties
  -FailOnWarn` passed every target with `check=OK`, `layout=0`, `text=0`, and
  `internal=0`; fresh screenshots are under `ui-precheck/`.
- Dynamic monitor/window probing reported one logical monitor `\\.\DISPLAY2`
  (1920x1080) and one visible smoke window intersecting it for Line Tool and the
  Recipe Line Pair target. Geometry is recorded under `monitor-line-retry/` and
  `monitor-recipe-line-pair/`. The first parallel probe was discarded because two
  smoke processes contended for the shared localization fixture; the sequential
  retry is the authoritative Line Tool geometry evidence.

The full alternate theme, 100/125/150/175/200% DPI matrix, every pointer/keyboard
state, native dialog interaction, camera/SDK/GPU, and long-running native runtime
remain outside this focused slice: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Decision and closure

The View Partial owned a concrete, independently testable property projection that
duplicated the existing Line pipeline adapter's property family. Moving it to that
adapter removes the duplicate owner while preserving the View's required WPF
composition and the already-completed persistence boundary. Do not reopen PL-0041
without a new requirement, reproducible defect, failed criterion, or changed
dependency/lifetime boundary.
