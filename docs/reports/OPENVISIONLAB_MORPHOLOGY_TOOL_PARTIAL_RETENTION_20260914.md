# Morphology Tool Partial retention audit

Date: 2026-09-14 (KST)  
Issue: `PL-0036`  
Scope: one scheduled Tool Partial no-change audit in `C:\Git\2D\Dev`

## Result

`MorphologyToolWpfView.xaml.cs` was rechecked as the next unprotected Tool View
Partial. No production change was made: the Partial already has a real View
composition boundary, while its mutable parameter state, settings persistence,
interaction policy, preview runtime, and release ownership are held by existing
concrete owners. Splitting or deleting the Partial would require a new seam or
would duplicate the existing controller/base contracts.

The Partial remains MVVM-compatible for this feature family: it is a View
adapter/composition root, `MorphologyToolPresenter` is the ViewModel-facing
facade, `MorphologyToolViewModel` owns parameter state and settings persistence,
and `VisionToolMorphologyInteractionController` owns operation/shape event and
visual-state policy.

## Owner and call-path proof

| Concern | Current owner/path | Mutable state/lifetime |
| --- | --- | --- |
| Tool creation | `OpenVisionNativeCustomToolFactory.CreateMorphology -> VisionToolCompositionService.CreateMorphologyToolViewModel -> MorphologyToolPresenter -> MorphologyToolWpfView` | Factory/document composition owns collaborator creation |
| XAML/presentation | `MorphologyToolWpfView.xaml(.cs)` owns namescope, resource lookup, controller attachment, localization presenter, and `CreateProperty` facade | Required XAML Partial and parent document own View lifetime |
| Parameter state/persistence | `MorphologyToolPresenter -> IMorphologyToolViewModel -> MorphologyToolViewModel` | ViewModel fields (`operation`, `shape`, kernel sizes, iterations) are the mutable writer; existing settings store remains there |
| Operation/shape interaction | `VisionToolMorphologyInteractionController` receives the existing buttons/radios and `MorphologyToolPresenter` | Controller attaches/detaches WPF events and updates visual selection state |
| Kernel input/preview | `VisionToolKernelSizeController`, `VisionToolParameterChangeController`, and existing base `VisionToolSingleInputCustomToolController` | View owns cleanup ordering; base `DisposeView` calls `DisposeToolResources` then tool controller disposal |

The actual path is:

```text
OpenVisionNativeCustomToolFactory.CreateMorphology
  -> VisionToolCompositionService.CreateMorphologyToolViewModel
  -> MorphologyToolPresenter(viewModel)
  -> MorphologyToolWpfView(presenter)
  -> InitializeComponent / AttachToolController
  -> existing kernel, interaction, parameter-change, guide, and preview owners
```

No direct file system, SaveFileDialog, concrete algorithm construction, or
settings-store call remains in the Morphology View Partial. Those operations
are either absent from the View or owned by the existing ViewModel/factory and
runtime owners.

## Binding/public contract and reading order

The XAML binding/public contract remains unchanged: `MorphologyToolWpfView`
continues to implement `ISingleInputPropertyVisionToolWpfView<MorphologyToolProperty>`,
exposes `CreateProperty`, keeps the existing `toolShell`, operation/kernel/shape
names, Learn topic index, and base `IVisionToolViewLifetime.DisposeView` path.

Shortest code-reading order:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/MorphologyToolWpfView.xaml(.cs)`
   — View namescope, composition, and cleanup.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/PropertyGrid/VisionToolParameterPresenters.cs`
   — presenter facade and property contract.
3. `src/OpenVisionLab/UI/VisionTest/ViewModels/MorphologyToolViewModel.cs`
   — mutable parameters, normalization, and settings persistence.
4. `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/VisionToolMorphologyInteractionController.cs`
   and `VisionToolKernelSizeController.cs` — event/visual/input policy.
5. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeCustomToolFactory.cs`
   and `VisionToolSingleInputCustomToolViewBase.cs` — creation and release path.
6. `tools/VisionRecipeRunnerSmoke/MorphologyToolPartialBoundaryContract.cs`
   — retained-owner proof.

## Decision

No new service, interface, manager, wrapper, forwarding Partial, or ViewModel
split was added. The shortest safe result is to keep the existing cohesive View
Partial and record its concrete owners. Reopen only with a new requirement,
reproducible defect, failed criterion, or changed dependency/lifetime boundary.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\morphology-tool-partial-retention-20260914`

- `MorphologyToolPartialBoundaryContract` — Debug and Release passed 6/6.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj` — Debug and Release passed with 0 warnings/errors.
- `PipelineViewerScreenshotSmoke` x64 Debug build — 0 errors; two pre-existing `MSB3270` architecture warnings remain.
- WPF smoke on detected single monitor `\\.\DISPLAY2` (1920x1080, working area 1920x1032): `wpf_filter_morphology_layout_guard` and `manual_morphology_tool_ui` passed and produced fresh PNG evidence.
- Screenshot review showed the docked and standalone Morphology Tool controls, operation buttons, kernel inputs/presets, shape radios, Learn action, and Preview button without clipping or overlap in the exercised states.

## Boundary and remaining risk

The source audit and focused WPF smoke prove the retained owner/call path and
the exercised visual states. They do not prove every theme/DPI/keyboard/mouse
matrix, camera/SDK/GPU behavior, or long-running native qualification. Those
remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.
