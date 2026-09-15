# OpenVisionLab Threshold Tool Partial boundary — PL-0040

Status: `Complete`

## Scope

이번 cycle은 `ThresholdToolWpfView.xaml.cs`의 XAML Partial에서 Threshold
teaching-suggestion workflow 조정 책임을 분리했다. View는 XAML namescope,
presentation callback, signal-inspector 표시, 기존 parameter interaction 및
Learn composition만 유지하고, 새 `ThresholdToolSuggestionController`가
suggestion 분석/사용/Undo의 호출 흐름과 View callback projection을 맡는다.
분석·stale evidence·Undo policy 자체는 기존
`VisionToolThresholdSuggestionSession`을 그대로 재사용한다. 새 ViewModel,
service interface, forwarding Partial, duplicate analyzer는 추가하지 않았다.

동작은 보존했다. 분석은 Preview를 실행하지 않고 advisory marker/status만
갱신하며, Use/Undo는 기존 `ApplySignalMarkerValue`와 debounced Preview 경로를
그대로 호출한다. Basic mode/evidence 검증, `AlreadyCurrent`, 이전 threshold
복원, evidence ID와 상태 메시지 계약은 유지된다.

## Ownership and boundary

| Concern | Current owner | Intended owner / result |
| --- | --- | --- |
| XAML namescope, buttons, status text, advisory marker rendering | `ThresholdToolWpfView` Partial | Retained as presentation callbacks only. |
| Suggestion workflow orchestration and UI projection | Previously the View Partial | `ThresholdToolSuggestionController`; no WPF control reference. |
| Suggestion analysis, stale-evidence validation, Applied/Previous snapshot and Undo policy | `VisionToolThresholdSuggestionSession` | Retained existing concrete policy owner. |
| Threshold parameter mutation, marker application, debounced Preview scheduling | `VisionToolThresholdInteractionController` and existing parameter-change path | Unchanged. |
| Mutable threshold parameters, normalization, settings persistence | `ThresholdToolViewModel` through `ThresholdToolPresenter` | Unchanged. |
| Signal evidence/plot/marker presentation and TSV callback seam | `VisionToolSignalInspectorView` | Unchanged; View supplies the marker callback. |
| Learn Window creation and cleanup | `ThresholdToolLearnWindowController` | Unchanged. |
| View creation and final resource release | native tool factory/composition and `VisionToolSingleInputCustomToolViewBase` | Unchanged. |

The new controller is justified by a concrete responsibility boundary: it owns
the multi-step suggestion workflow but does not own controls, Window instances,
file I/O, persistence, or mutable threshold policy. Reusing the existing session
and interaction controller prevents a second policy owner or a one-implementation
interface.

## Actual call path

```text
OpenVisionNativePropertyGridToolFactory.CreateThreshold
  -> VisionToolCompositionService.CreateThresholdToolViewModel
  -> ThresholdToolWpfView
  -> SignalInspector.ShowEvidence / ThresholdToolWpfView.ShowSignalEvidence
  -> ThresholdToolSuggestionController.UpdateAvailability
  -> ThresholdToolSuggestionController.Analyze
  -> VisionToolThresholdSuggestionSession.Analyze
  -> VisionToolThresholdSuggestionAnalyzer.Analyze
  -> ThresholdToolSuggestionController.Use/Undo
  -> VisionToolThresholdSuggestionSession.Use/Undo
  -> VisionToolThresholdInteractionController.ApplySignalMarkerValue
  -> existing parameter-change / debounced Preview path
```

The XAML button handlers are presentation-only adapters and call the controller.
The controller writes status/button/panel state through explicit callbacks and
never reaches into a WPF visual tree. The session remains the mutable suggestion
and Undo writer; the interaction controller remains the threshold parameter
writer.

## State, binding, and public/test contract

- `ThresholdToolWpfView.xaml` remains the required XAML Partial with the existing
  suggestion panel and automation IDs: `ThresholdSuggestionPanel`,
  `ThresholdSuggestionAnalyzeButton`, `ThresholdSuggestionUseButton`, and
  `ThresholdSuggestionUndoButton`.
- `ThresholdToolPresenter`/`ThresholdToolViewModel` continue to own the bound
  threshold values, normalization, summary, and settings persistence.
- `CreateProperty`, marker test facades, suggestion test facades, signal evidence
  binding, Learn entry, and base View lifetime remain unchanged.
- The controller exposes only read-only suggestion projections used by the
  existing WPF test facade; it does not duplicate session state.

## Shortest code-reading order

Search once for `ThresholdToolSuggestionController`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/ThresholdToolWpfView.xaml.cs`
   — XAML composition, callbacks, test facade, and button routing.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Threshold/ThresholdToolSuggestionController.cs`
   — workflow call path and presentation callback contract.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Threshold/VisionToolThresholdSuggestionSession.cs`
   — mutable suggestion, stale-evidence, and Undo policy.
4. `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Threshold/VisionToolThresholdSuggestionAnalyzer.cs`
   — deterministic histogram analysis and evidence ID.
5. `src/OpenVisionLab/UI/VisionTest/Wpf/Behaviors/VisionToolThresholdInteractionController.cs`
   — mutable parameter writer and Preview scheduling boundary.
6. `src/OpenVisionLab/UI/VisionTest/ViewModels/ThresholdToolViewModel.cs` and
   `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/PropertyGrid/VisionToolParameterPresenters.cs`
   — binding state, normalization, summary, and settings persistence.
7. `src/OpenVisionLab/UI/Menu/Wpf/NativeTools/Documents/OpenVisionNativePropertyGridToolFactory.cs`
   and `src/OpenVisionLab/UI/VisionTest/Composition/VisionToolCompositionService.cs`
   — creation and composition.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-suggestion-controller-20260914`

- `ThresholdSuggestionSessionContract` passed `7/7` in Debug and Release. The
  contract proves that the View routes to the WPF-free controller, the controller
  delegates to the existing session, no View/session policy state remains, and
  analysis/Use/Undo/stale/non-Basic behavior is preserved.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with `0` warnings and
  `0` errors.
- `PipelineViewerScreenshotSmoke` Debug build passed with `0` errors and one
  pre-existing nullable warning in unrelated smoke source.
- `RunUiPrecheck.ps1 -Targets wpf_shell_host_threshold_tool -FailOnWarn` passed
  `check=OK`, `layout=0`, `text=0`, `internal=0`; fresh screenshot is under
  `ui-precheck/wpf_shell_host_threshold_tool.png`.
- The `cvr07_threshold_suggestion` runtime smoke passed with `check=OK`,
  `layout=0`, `text=0`, `internal=0`; the fresh selection/apply/Undo screenshot
  is under `monitor-cvr07/monitor-window-smoke-output/`.
- Dynamic monitor/window probing reported one logical monitor `\\.\DISPLAY2`
  (1920x1080) and one visible smoke window intersecting it for both the Threshold
  Tool and CVR-07 targets. Geometry is recorded in
  `monitor-threshold/monitor-window-geometry.json` and
  `monitor-cvr07/monitor-window-geometry.json`.

The full alternate theme, 100/125/150/175/200% DPI matrix, every pointer/keyboard
state, native dialog interaction, camera/SDK/GPU, and long-running native runtime
remain outside this focused slice: `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Decision and closure

The View Partial contained an independently testable multi-step workflow
orchestration boundary, while the existing session already owned the policy and
the interaction controller already owned parameter mutation. A small concrete
controller is therefore the narrowest structural correction. Do not reopen
PL-0040 without a new requirement, reproducible defect, failed criterion, or
changed dependency/lifetime boundary.
