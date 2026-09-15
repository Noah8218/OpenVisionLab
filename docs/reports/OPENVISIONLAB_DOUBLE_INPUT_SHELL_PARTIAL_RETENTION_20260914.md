# OpenVisionLab Double-Input Tool Shell Partial Retention

Date: 2026-09-14 (KST)  
Issue: `PL-0056`  
Scope: `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/DoubleInput/VisionToolDoubleInputCustomToolShell.xaml.cs`

## Decision

`VisionToolDoubleInputCustomToolShell.xaml.cs`는 현재 Partial을 제거하거나
새 파일로 분할하지 않는다. 이 Partial은 `UserControl`의 XAML namescope와
dependency-property/public visual facade를 연결하는 WPF View 어댑터이며,
실행 정책·Property/Layer 상태·파일 I/O·알고리즘·독립 수명을 소유하지 않는다.
중첩 `DockedInspectorLayoutController`도 별도 도메인 책임이 아니라 같은
control의 named row/column/group를 floating/docked 상태로 투영하는
Presentation 전용 adapter다.

파일 길이만으로 controller, wrapper, ViewModel, forwarding Partial을
추가하면 동일한 WPF control state를 공유하는 중복 owner가 된다. 따라서 이번
slice의 구조 변경은 **no-change retention**이며, owner/call-path 계약과
representative runtime 검증을 새로 고정했다.

## Owner and call path

| Concern | Current owner | Evidence / call path |
| --- | --- | --- |
| XAML namescope, DP, visual elements, docked/floating geometry | `VisionToolDoubleInputCustomToolShell.xaml(.cs)` | `InitializeComponent` → `DockedInspectorLayoutController.Apply`; `IsDockedInspectorMode` callback updates named rows/columns and preview groups |
| Tool composition entry | `ArithmeticToolWpfView.xaml(.cs)` | XAML `toolShell` → `VisionToolDoubleInputCustomToolViewBase.AttachToolController` |
| Layer/preview binding and command callback routing | `VisionToolDoubleInputCustomToolRuntime` → `VisionToolDoubleInputViewRuntime` | `Attach` discovers `toolShell`, binds combos/buttons/preview slots, and disposes `inputRuntime` |
| Event forwarding and language lifetime | `VisionToolDoubleInputCustomToolController` | `VisionToolDoubleInputToolEventHub` raises public events; language controller and runtime are disposed here |
| View lifetime and mutable resource release | `VisionToolDoubleInputCustomToolViewBase` and concrete Tool View | `DisposeView` calls `DisposeToolResources` then `toolController.Dispose`; `ArithmeticToolWpfView` releases interaction/preview owners |
| Dock/float mode selection | `OpenVisionToolDockModeHelper` | Sets only `VisionToolDoubleInputCustomToolShell.IsDockedInspectorMode`; it does not execute a tool or write recipe state |
| Learn topic Window creation/re-entry | `VisionToolLearnWindowController` | Shell click adapter calls `learnWindowController.Open(LearnTopicIndex)`; the Shell does not construct or show a Window |
| Arithmetic policy and execution | `ArithmeticToolWpfView` collaborators, ViewModel/factory/document/preview owners | Shell receives visual values and callbacks; it has no native tool, persistence, or pipeline policy |

The shortest reading order is:

```text
ArithmeticToolWpfView.xaml
 -> ArithmeticToolWpfView.xaml.cs
 -> VisionToolDoubleInputCustomToolViewBase
 -> VisionToolDoubleInputCustomToolController
 -> VisionToolDoubleInputCustomToolRuntime
 -> VisionToolDoubleInputCustomToolShell.xaml(.cs)
 -> VisionToolDoubleInputViewRuntime / ArithmeticToolPreviewController
```

Search once for `toolShell` to reach the composition and runtime call path.

## Binding and public contract

The XAML contract remains intact:

- `TitleIconKind`, `ParameterContent`, `IsDockedInspectorMode`,
  `LearnButtonVisibility`, `LearnButtonText`, and `LearnTopicIndex` remain
  dependency properties.
- `InputAGroup`, `InputBGroup`, `OutputLayerGroup`, preview frames/slots,
  layer combos, image-load/create buttons, run buttons, and Add Pipeline remain
  the existing public visual facade used by the shared runtime and WPF smoke.
- `VisionToolHeaderLearnButton`, `VisionToolParametersGroup`, and the three
  preview automation IDs remain in the XAML namescope.
- `ArithmeticToolWpfView` still supplies parameter content and uses the shared
  base/controller contract; no binding or public facade was renamed.

## MVVM assessment

The shell is a View, not a ViewModel. Its code-behind contains only framework
plumbing that is specific to this View: `InitializeComponent`, dependency
property wrappers, named-element projections, a Click-to-controller adapter,
and layout density changes. It does not reference a concrete ViewModel,
perform persistence, open dialogs, call OpenCV/native execution, or own
algorithm/recipe policy. The existing runtime/controller/View base provide the
MVVM-adjacent adapter boundaries already used by the tool family.

## Focused proof

`VisionToolDoubleInputCustomToolShellPartialBoundaryContract` records 12 source
checks for the XAML/DP contract, visual-only layout owner, arithmetic composition,
runtime/controller/base lifetime, docking caller, Learn controller, and absence
of persistence/algorithm coupling. It passed in both configurations:

```text
Debug:   VISION_TOOL_DOUBLE_INPUT_SHELL_PARTIAL_BOUNDARY_CONTRACT=PASS|checks=12
Release: VISION_TOOL_DOUBLE_INPUT_SHELL_PARTIAL_BOUNDARY_CONTRACT=PASS|checks=12
```

Representative WPF smoke passed in both configurations:

- `wpf_arithmetic_tool_learn_button`
- `wpf_layer_selection_arithmetic_tool`

The dynamic monitor probe moved the actual smoke window to the selected smaller
left monitor and verified one intersecting window and a passing screenshot
contract in both Debug and Release. Evidence is under:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\double-input-shell-partial-retention-20260914`

## Remaining verification boundary

This slice proves source ownership and representative Arithmetic shell behavior.
It does not prove every Tool View consumer, every theme/layout/DPI (100–200%)
combination, keyboard/tab/hover/pressed/error matrices, native SDK/GPU/camera
behavior, actual inspection execution/persistence, or long-running shutdown.
Those remain `소스 코드 기준 검토 완료 / 대표 WPF Runtime UI 및 동적 모니터 배치 검증 완료 / 전체 Runtime 행렬은 미검증`.

