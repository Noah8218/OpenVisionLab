# OpenVisionLab Matching Tool View Partial boundary — PL-0039

Status: `Complete`

## Scope

This slice reviewed `MatchingToolWpfView.xaml.cs` as the next scheduled Partial
candidate and moved its sample-property projection policy to the existing
`VisionPipelineMatchingPropertyAdapter`. The View still owns XAML composition,
the matching controller facade, and WPF-specific test forwarding. No new
service, interface, wrapper, ViewModel, or Partial was added.

The change is intentionally behavior-preserving. `AUTO_PREVIEW = false`,
template-path resolution, every common OpenCV and Matching assignment, and the
defensive `CvROIS`/`CvMASKS` list copies remain in the same order and with the
same values as before.

## Ownership and boundary

| Concern | Current owner | Intended owner / result |
| --- | --- | --- |
| XAML namescope, shell composition, controller attachment | `MatchingToolWpfView` Partial | Retained in the View Partial; this is required WPF plumbing. |
| Sample template-path resolution | Previously the View | `VisionPipelineMatchingPropertyAdapter.ResolveSampleTemplatePath`. |
| Sample common OpenCV/Matching property projection | Previously the View callback body | `VisionPipelineMatchingPropertyAdapter.ApplySampleProperty`. |
| Mutable Matching property, defaults, normalization, template reload | `MatchingToolViewModel` | Unchanged. |
| PropertyGrid, preview, result review, preset and delayed-preview state | `VisionToolMatchingPropertyRuntime<TProperty>` and `VisionToolSingleInputMatchingToolRuntime<TProperty>` | Unchanged. |
| Event/language/controller lifetime | `VisionToolSingleInputMatchingToolController<TProperty>` | Unchanged. |
| Final View/controller disposal | `VisionToolSingleInputPropertyToolViewBase` | Unchanged. |

The mapping owner is an existing concrete adapter that already creates
`MatchingProperty` objects from pipeline steps and converts them back. Reusing
that owner keeps the dependency direction explicit and avoids a one-implementation
interface or a second copy/codec for the same property family.

## Actual call path

```text
OpenVisionNativePropertyGridToolFactory.CreateMatching
  -> VisionToolCompositionService.CreateMatchingToolViewModel
  -> MatchingToolWpfView
  -> OpenVisionNativeToolDocument.ApplySampleStepParameters
  -> MatchingToolWpfView.ApplySampleProperty
  -> VisionToolSingleInputMatchingToolController.SetTemplatePathForTest
  -> VisionPipelineMatchingPropertyAdapter.ResolveSampleTemplatePath
  -> VisionPipelineAppToolFactory.ResolveTemplatePath
  -> VisionToolSingleInputMatchingToolController.ConfigurePropertyForTest
  -> VisionPipelineMatchingPropertyAdapter.ApplySampleProperty
  -> MatchingProperty target / presenter / runtime
```

`SetTemplatePathForTest` remains before the property-copy callback. This
preserves the existing template registration and reload behavior. The adapter
only maps values; it does not create a Window, open a dialog, run Preview/Run,
or own the target property's lifetime.

## State, binding, and public contract

- The mutable source property remains the `MatchingToolViewModel.property`
  instance and is normalized before `CreateProperty` returns a copy.
- The callback target is still supplied by the existing matching runtime and
  presenter. The adapter writes that target and clones the two mutable ROI
  lists, preserving the previous aliasing boundary.
- `MatchingToolWpfView.xaml` remains a required Partial with the existing
  `toolShell` namescope, `Learn Matching` caption, topic `9`, and visible
  template status.
- Existing public/test facades remain unchanged:
  `ResultReviewTextForTest`, `CreateProperty`, `SetTemplatePathForTest`,
  `ConfigurePropertyForTest`, `ApplyPresetForTest`, and `SetResultReview`.
- The base View remains the final release owner. The Matching Partial does not
  add a second `Dispose` path.

## Shortest code-reading order

Search for `ApplySampleProperty` once, then read in this order:

1. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeToolDocument.cs`
   — caller and sample-step dispatch.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/MatchingToolWpfView.xaml.cs`
   — the thin WPF orchestration and public/test facade.
3. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineMatchingPropertyAdapter.cs`
   — template-path and property projection policy.
4. `src/OpenVisionLab/UI/VisionTest/ViewModels/MatchingToolViewModel.cs`
   — mutable state, defaults, normalization, and template reload.
5. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SingleInput/VisionToolSingleInputMatchingToolController.cs`
   and `VisionToolSingleInputMatchingToolRuntime.cs` — event, PropertyGrid,
   preview, result-review, and disposal ownership.
6. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativePropertyGridToolFactory.cs`
   and `src/OpenVisionLab/UI/VisionTest/Composition/VisionToolCompositionService.cs`
   — creation and composition.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\matching-tool-partial-boundary-20260914`

- `MatchingToolPartialBoundaryContract` passed `11/11` in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with `0` warnings
  and `0` errors. The first D-drive redirected build attempt was discarded as
  an invalid shared-intermediate-path experiment; the standard project build
  is the authoritative result.
- `PipelineViewerScreenshotSmoke` x64-equivalent Debug build passed with `0`
  errors and one pre-existing nullable warning in its unrelated smoke source.
- `RunUiPrecheck.ps1 -Targets wpf_shell_host_matching_tool -FailOnWarn` passed
  with `check=OK`, `layout=0`, `text=0`, `internal=0`, and a fresh 1600x900
  screenshot at
  `matching-tool-partial-boundary-20260914/ui-precheck/wpf_shell_host_matching_tool.png`.
- `RunUiPrecheck.ps1 -Targets wpf_shell_host_recipe_fixture_properties
  -FailOnWarn` also passed with `check=OK`, `layout=0`, `text=0`, `internal=0`.
  This target exercises the recipe-step load path that calls
  `OpenVisionNativeToolDocument.ApplySampleStepParameters`, with evidence at
  `matching-tool-partial-boundary-20260914/fixture-ui-precheck/`.
- Dynamic monitor detection reported one logical monitor `\\.\DISPLAY2`,
  1920x1080; the actual smoke process probe recorded one visible window
  intersecting that monitor in `monitor-geometry/monitor-window-geometry.json`.
  The fixture-property smoke process produced the same monitor intersection in
  `fixture-monitor-geometry/monitor-window-geometry.json`.

The changed XAML and visual resources are none; the fresh Matching smoke
screenshot confirms the unchanged shell/property-grid presentation. Full
theme, 100/125/150/175/200% DPI, every pointer/keyboard state, native dialog
interaction, camera/SDK/GPU, and long-running native runtime remain outside
this focused slice: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Decision and closure

The original View callback owned a concrete, independently testable mapping
policy. The existing Matching pipeline adapter was the smallest correct owner,
so the policy moved there while the View remained a WPF composition adapter.
Do not reopen PL-0039 unless a new requirement, reproducible defect, failed
criterion, or changed dependency/lifetime boundary appears.
