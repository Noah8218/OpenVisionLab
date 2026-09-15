# OpenVisionLab Simple Preprocess Tool View Partial retention — PL-0042

Status: `Complete`

## Scope

이번 cycle은 `SimplePreprocessToolWpfView.xaml.cs`를 실제 호출자, mutable-state
writer, persistence/file owner, binding/public contract, preview/signal owner와
대조한 no-change 구조 감사다. View Partial에는 새 production 책임을 옮기거나
새 Partial을 추가하지 않았다. 현재 View는 required XAML namescope와 custom
parameter/signal/result presentation을 조합하는 concrete WPF adapter이며,
독립 state/lifetime/test seam 없이 분리하면 기존 계약을 감싸는 forwarding
wrapper 또는 중복 상태가 된다.

## Ownership and boundary

| Concern | Current owner | Result |
| --- | --- | --- |
| XAML namescope, `toolShell`, dynamic parameter panel, signal inspector | `SimplePreprocessToolWpfView` Partial + `SimplePreprocessToolWpfView.xaml` | Retained required WPF composition and presentation plumbing. |
| Dynamic editor controls, parsing/clamping, visibility, parameter-guide bindings, settings snapshot | `SimplePreprocessParameterController` | Existing concrete parameter owner remains in place. |
| Header/title/summary/localization projection | `SimplePreprocessTextPresenter` and `ToolController` | View delegates presentation; no algorithm policy is stored in the View. |
| Parameter-change invalidation and debounced Preview scheduling | `VisionToolParameterChangeController` + `VisionToolDebouncedPreviewScheduler` | Existing change/preview owners remain; View only wires callbacks and releases scheduler. |
| Edge/RotateScale/Mean/HSV/Histogram property projection | `OpenVisionNativeSimplePreprocessPropertyFactory` | Existing property factory remains the policy owner. |
| Preview algorithms, result review, Histogram signal evidence | `OpenVisionNativeSimplePreprocessPreviewExecutor` + `SimplePreprocessResultExplanation` | View receives presentation results; it does not construct algorithms or inspect images. |
| Tool settings load/save and native document composition | `OpenVisionNativeSimplePreprocessDocumentFactory` + `OpenVisionNativeSingleInputToolDocumentBuilder` | Persistence and document lifetime stay outside the View. |
| Tool registration and creation | `OpenVisionNativeToolRegistry` and SimplePreprocess document factory | Five menu registrations continue to use the same View type. |
| Final controller release | `VisionToolSingleInputCustomToolViewBase.DisposeView` | Base calls `DisposeToolResources()` then releases the shared tool controller. |

The View's `suppressEvents` flag is a narrow UI re-entry guard used while the
existing parameter controller applies settings. It is not an algorithm,
persistence, or recipe state owner. `parameterGuideBinder` and the scheduler are
created by the View because their lifetime is the View's WPF lifetime; both are
released from `DisposeToolResources()`.

## Actual call path

```text
OpenVisionNativeToolRegistry (Edge / RotateScale / HSV / Mean / Histogram)
  -> OpenVisionNativeSimplePreprocessDocumentFactory.Create*Document
  -> SimplePreprocessToolWpfView.InitializeComponent
  -> SimplePreprocessParameterController + VisionToolParameterChangeController
  -> OpenVisionNativeSimplePreprocessViewConfigurator.Configure*View
  -> OpenVisionNativeSimplePreprocessPropertyFactory / parameter-guide binder
  -> OpenVisionNativeSimplePreprocessDocumentFactory settings Load/Save callback
  -> OpenVisionNativeSingleInputToolDocumentBuilder
  -> VisionToolDebouncedPreviewScheduler.RequestRunPreview
  -> OpenVisionNativeSimplePreprocessPreviewExecutor
  -> SimplePreprocessToolWpfView.ShowResultReview / ShowSignalEvidence
  -> VisionToolSingleInputCustomToolViewBase.DisposeView
```

`OpenVisionNativeSimplePreprocessPreviewExecutor` calls the View only through
the existing parameter/result/evidence facade. The View does not create an
`EdgeDetectionTool`, `RotateScaleTool`, or `MeanTool`, read/write settings,
open a file dialog, access `System.IO`, or own an OpenCvSharp image.

## Binding, public, and test contract

- `SimplePreprocessToolWpfView.xaml` keeps `x:Class`, `toolShell`,
  `parameterContentHost`, `parameterPanel`, and `signalInspector` names.
- The `Parameters` facade remains the configurator/property-factory entry point;
  `ParameterChanged` remains the settings/summary callback contract.
- `SetHeader`, `SetLocalizedHeader`, `SetSummary`, `SetLearnTopic`,
  `ShowResultReview`, `ShowSignalEvidence`, and `ClearSignalEvidence` remain
  available to the existing configurator and preview executor.
- Signal evidence test properties, navigation/reset, and export test facade stay
  on the View because they are presentation/test adapters around the existing
  `VisionToolSignalInspectorView`.
- `VisionToolSingleInputCustomToolViewBase.DisposeView` remains the lifetime
  entry point; no duplicate controller disposal was introduced.

## Why no production split is justified

The inspected manual code-behind contains only WPF composition, presentation
facades, callback wiring, and release of two View-owned UI helpers. Parameter
state, property creation, settings persistence, algorithm execution, result
explanation, signal evidence generation, creation, and final controller release
already have concrete owners. A new ViewModel/service/interface/manager or
forwarding Partial would not reduce coupling or establish an independent state,
lifetime, or test seam. The correct structural result for this slice is to keep
the Partial and protect its retained boundary with a source contract.

## Shortest code-reading order

Search once for `SimplePreprocessToolWpfView`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/SimplePreprocessToolWpfView.xaml.cs`
   and its XAML for namescope, facade, callback, and View lifetime.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/SimplePreprocessParameterController.cs`
   for editor state, parsing, settings snapshot, and guide bindings.
3. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeSimplePreprocessViewConfigurator.cs`
   and `OpenVisionNativeSimplePreprocessPropertyFactory.cs` for tool-specific
   parameter setup/property projection.
4. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativeSimplePreprocessDocumentFactory.cs`
   for settings persistence and document composition.
5. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Preview/OpenVisionNativeSimplePreprocessPreviewExecutor.cs`
   for algorithm/result/signal flow.
6. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/SingleInput/VisionToolSingleInputCustomToolViewBase.cs`
   for final controller release.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\simple-preprocess-partial-retention-20260914`

- `SimplePreprocessPartialBoundaryContract` passed 9/9 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with 0 warnings/errors.
- `PipelineViewerScreenshotSmoke` Debug build passed with 0 errors; the existing
  nullable warning profile was not expanded by this source-only audit.
- WPF precheck passed for `wpf_shell_host_rotate_scale_tool`,
  `wpf_simple_preprocess_result_review`, and
  `wpf_simple_preprocess_tool_learn_button` with `check=OK`, layout/text/internal
  counts all zero, and fresh screenshots.
- Sequential monitor probes passed for Rotate/Scale and Simple Preprocess result
  review: one detected `DISPLAY2` monitor (`1920x1080`) and one intersecting
  visible smoke window per target. The probe was run sequentially to avoid the
  shared localization fixture contention observed in an earlier unrelated cycle.
- Repository-wide readiness, RefactorAudit, DocumentationIndex, issue validation,
  and diff checks are recorded under the same evidence root.

This is a source/contract and focused WPF smoke qualification only. Full theme,
Wide/Compact layout, 100/125/150/175/200% DPI, every input/keyboard visual state,
native dialog interaction, camera/SDK/GPU, and long-running native runtime remain
unverified: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Boundary lock

Do not reopen this retained Partial without a new requirement, reproducible
defect, failed criterion, or changed dependency/lifetime boundary. The next
scheduled candidate is another unprotected Tool/Learn Partial, not a mechanical
attempt to reduce the Partial count.
