# VisionToolVerificationGuideView Partial retention proof — PL-0054

Status: `Complete`

## Scope

This slice rechecked `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Review/VisionToolVerificationGuideView.xaml.cs` as the next scheduled Partial boundary. The result is a no-change structural decision: the existing Partial remains the required WPF/XAML visual adapter, and no production split or deletion is justified by the current ownership evidence.

## Owner and call path

- Current owner: `VisionToolVerificationGuideView` (XAML namescope, dependency properties, text/brush projection, compact-density visual projection).
- Intended owner: the existing View remains the owner of presentation-only WPF state. Verification policy is not in this Partial.
- Criteria/result policy owner: `VisionToolAreaVerificationGuidePresenter<TProperty, TResult>` for area tools and `VisionToolMatchingVerificationGuidePresenter` for matching tools.
- Shared composition owner: `VisionToolSingleInputPropertyToolShell` owns `ToolContent`, visibility, and compact density routing.
- Mutable-state writer: the two verification presenters write `HeaderText`, `StateText`, `CriteriaText`, `NextActionText`, and `StateBrush`; the View only projects those dependency properties.
- Lifetime owner: the containing Tool view/runtime and `VisionToolSingleInputPropertyToolShell`; the guide has no timer, worker, disposable resource, file handle, or independent algorithm lifetime.

```text
VisionToolAreaVerificationGuidePresenter / VisionToolMatchingVerificationGuidePresenter
  -> VisionToolVerificationGuideView dependency properties
  -> VisionToolVerificationGuideView.xaml text/brush/automation projection
VisionToolSingleInputPropertyToolShell.ApplyToolContentDensity
  -> VisionToolVerificationGuideView.IsCompactMode
  -> guide chrome/header/next-action density projection
BlobToolWpfView / ContourToolWpfView / EdgeBasedMatchingToolWpfView /
VisionToolSingleInputMatchingToolRuntime
  -> shared Tool shell ToolContent composition
```

## Binding and public contract

The existing public dependency-property contract is retained: `HeaderText`,
`StateText`, `CriteriaText`, `NextActionText`, `StateBrush`, and
`IsCompactMode`. XAML keeps the existing `AutomationProperties.AutomationId`
values for the guide, state, criteria, and next-action surfaces, along with
tooltips and `CharacterEllipsis` trimming. The shared shell continues to bind
`ToolContent` and `ToolContentVisibility` through its existing ancestor bindings.

## Why no production split was made

The Partial contains no file or dialog I/O, OpenCV/algorithm calls, Recipe
execution/persistence, ViewModel policy, asynchronous work, or independent
mutable business state. `IsCompactMode` changes only named WPF row/padding
visuals. Extracting a ViewModel, service, interface, forwarding Partial, or
generic guide manager would duplicate the existing presenter and shell owners
without creating an independent state, lifetime, reuse, or test seam.

## Focused verification

- `VisionToolVerificationGuidePartialBoundaryContract`: 12/12 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug build: 0 errors, 2 existing MSB3270 processor-architecture warnings; contract PASS.
- `VisionRecipeRunnerSmoke` Release build: 0 warnings, 0 errors; contract PASS.
- Debug WPF precheck: `wpf_shell_host_blob_tool_docked_verification`, `wpf_shell_host_contour_tool_docked_verification`, and `wpf_shell_host_matching_tool_docked_verification` all `OK` with layout/text/internal diagnostics at zero.
- Release WPF precheck: the same three shared-shell targets all `OK` with layout/text/internal diagnostics at zero.
- Debug and Release dynamic monitor probes selected the smaller-left `\\.\DISPLAY2` monitor, explicitly moved the smoke window into its working area, observed one intersecting window, and produced the target PNG without an error artifact.
- Repository gates passed after the source/WPF checks: Readiness Debug/Release, RefactorAudit (`PartialDeclarations=54`, `ProjectCycles=0`), DocumentationIndex, LLM index parse, and `git diff --check`.

Evidence is under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\verification-guide-partial-retention-20260914`.

## Developer reading order

1. `VisionToolVerificationGuideView.xaml.cs` and `.xaml` — dependency properties and visual projection.
2. `VisionToolAreaVerificationGuidePresenter.cs` — area criteria/result policy.
3. `VisionToolMatchingVerificationGuidePresenter.cs` — matching criteria/result policy.
4. `VisionToolSingleInputPropertyToolShell.xaml.cs` and `.xaml` — ToolContent composition and compact-density routing.
5. `BlobToolWpfView.xaml.cs`, `ContourToolWpfView.xaml.cs`, `EdgeBasedMatchingToolWpfView.xaml.cs`, and `VisionToolSingleInputMatchingToolRuntime.cs` — representative consumers.
6. `VisionToolVerificationGuidePartialBoundaryContract.cs` — executable boundary proof.

## Remaining boundary

This is source and focused WPF smoke evidence. Full theme/DPI/input state
matrices, native dialog/file association, camera/SDK/GPU execution, actual
inspection execution/persistence, and long-running native runtime remain
unverified. The correct statement for those rows is `소스 코드 기준 검토 완료 /
실제 Runtime UI 검증 필요`.
