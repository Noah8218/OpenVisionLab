# OpenVisionLab Binary Learn View Partial retention — PL-0048

Status: `Complete`

## Scope

이번 cycle은 `BinaryLearnView.xaml.cs`를 Binary Learn의 lesson state,
simulation decisions, topic composition, related-tool action, WPF animation
and visual-resource lifetime, binding/test contract owner와 대조한 no-change
구조 감사다. Production Partial을 분리하거나 합치지 않았다. 현재 View는
Morphology/Blob/Contour 세 lesson panel의 XAML-backed visual adapter이며,
독립 business-state/lifetime/test seam 없이 다시 추출하면 기존 presenter와
window composition을 감싸는 forwarding wrapper가 된다.

## Ownership and boundary

| Concern | Current owner | Result |
| --- | --- | --- |
| XAML namescope, topic panels, cell controls, brush projection, button content | `BinaryLearnView.xaml` + `BinaryLearnView.xaml.cs` | View-specific presentation/framework plumbing으로 유지. |
| Fixed binary samples, morphology/blob/contour state, simulation results, stage decisions, formulas and explanations | `BinaryLearnPresenter` | Lesson policy와 mutable simulation state의 concrete owner. |
| Binary morphology/blob/contour calculation primitives | `OpenVisionLearnBinarySimulationModel` via `BinaryLearnPresenter` | View Partial은 simulation model을 직접 호출하지 않음. |
| Topic selection and child View composition | `OpenVisionLearnWindow` + `OpenVisionLearnTopicPresentationPolicy` | Window가 selected topic을 해석하고 `SelectTopic`을 호출. |
| Related-tool open routing | `OpenVisionLearnWindow.SetOpenRelatedToolAction` caller callback | View는 `Action<VISION_MENU>`를 받아 invoke할 뿐 concrete Tool/factory를 생성하지 않음. |
| Animation clock and callback lifetime | `BinaryLearnView` | Three `DispatcherTimer` instances are presentation-local and stopped/unsubscribed on `Unloaded`. |
| Screenshot/test facade | `LearnBinaryLineSmoke` and internal `*ForTest` members | Existing test contract is kept; it does not become lesson state owner. |

The View's `presenter` field is an existing concrete presenter composition
dependency. The cell lists and brushes are rendered control state; they are not
the source of lesson decisions. `openRelatedToolAction` is a callback boundary,
not a hidden service or tool registry.

## Actual call path

```text
OpenVisionLearnWindow.UpdateSelectedTopic
  -> binaryLearnView.SelectTopic(topicIndex)

OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> binaryLearnView.SetOpenRelatedToolAction(action)
  -> BinaryLearnView.OpenRelatedToolButton_Click
  -> action(VISION_MENU.Morphology | Blob | Contour)
  -> existing Learn/Shell composition opens the related Tool

BinaryLearnView constructor
  -> BinaryLearnPresenter (fixed state + simulation result)
  -> BuildMorphologyCells / BuildBlobCells / BuildContourCells
  -> Update*Guide and WPF DispatcherTimer presentation
  -> Loaded/Unloaded timer attach and release
```

The View owns only WPF control projection, animation stepping, topic visibility,
and callback invocation. No View Partial file I/O, dialog, OpenCV construction,
tool creation, persistence, or inspection algorithm was found.

## Binding, public, and test contract

- `BinaryLearnView.xaml` retains `x:Class="OpenVisionLab.BinaryLearnView"`,
  `Focusable="False"`, the three named topic panels, and existing
  `AutomationProperties.AutomationId` values for practice/open-tool controls.
- Existing XAML event contracts remain `Loaded`, `Unloaded`, Morphology mode
  selection, Blob minimum-area slider, Contour draw-mode selection, animation
  buttons, and `OpenRelatedToolButton_Click`.
- `OpenVisionLearnWindow` remains the DataContext/composition owner for topic
  routing and the related-tool callback; no ViewModel or service was introduced
  to hide the callback.
- Existing internal test accessors for animation steps, formula/status text,
  topic visibility, and tool-location guidance remain unchanged.

## Why no production split is justified

The inspected code-behind contains WPF control creation, cell painting, timer
events, topic visibility, and callback forwarding. Fixed lesson state,
simulation algorithms, formulas, stage decisions, and related-tool copy are
already owned by `BinaryLearnPresenter` and the existing Learn Window
composition. Moving painting or timer handlers into a new presenter would leak
WPF controls and timer lifetime; moving presenter state into a new ViewModel
would duplicate the existing owner. A new service/interface/manager or another
Partial would therefore add a wrapper boundary without independent ownership or
test value. The smallest correct structural result is to retain the XAML Partial
and protect its owner map with a source contract.

## Shortest code-reading order

Search once for `BinaryLearnView`, then read:

1. `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/BinaryLearnView.xaml.cs` and
   `.xaml` for topic panels, timer lifetime, cell rendering, callbacks, and
   automation/test surface.
2. `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/BinaryLearnPresenter.cs` for
   fixed samples, simulation result state, formulas, thresholds, stages, and
   related-tool guidance.
3. `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnWindow.xaml.cs`
   and `OpenVisionLearnTopicPresentationPolicy.cs` for child composition,
   topic selection, callback injection, and Window lifetime.
4. `src/OpenVisionLab/UI/VisionTest/Wpf/Learn/OpenVisionLearnBinarySimulationModel.cs`
   for the WPF-free simulation primitives called by the presenter.
5. `tools/PipelineViewerScreenshotSmoke/LearnBinaryLineSmoke.cs` and its
   target catalog entries for the focused visual/test contract.

## Verification

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\binary-learn-partial-retention-20260914`

- `BinaryLearnPartialBoundaryContract` passed 9/9 in Debug and Release.
- `VisionRecipeRunnerSmoke` Debug and Release builds passed with 0
  warnings/errors.
- `RunUiPrecheck.ps1` passed `wpf_openvision_learn_binary_line_contract` and
  `wpf_openvision_learn_binary_line_views` with `check=OK`, layout/text/internal
  counts all zero, and fresh screenshots.
- The dynamic monitor/window probe selected the reported test monitor by
  topology and recorded one intersecting visible smoke window on `DISPLAY2`
  (1920x1080).
- Refactor audit, documentation index, readiness, ledger validation, JSON parse,
  and `git diff --check` are recorded as the final slice gates.

This is a source/contract and focused WPF smoke qualification only. Full theme,
Wide/Compact layout, 100/125/150/175/200% DPI, every animation/input/keyboard
visual state, native related-tool click/file association, camera/SDK/GPU, and
long-running native runtime remain unverified: `소스 코드 기준 검토 완료 /
실제 Runtime UI 검증 필요`.

## Boundary lock

Do not reopen this retained Partial without a new requirement, reproducible
defect, failed criterion, or changed dependency/lifetime boundary. Continue
with another unprotected Learn View Partial in the next scheduled cycle.
