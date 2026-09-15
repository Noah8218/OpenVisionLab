# OpenVisionLab Grayscale Learn View Partial retention — PL-0050

상태: `Complete` (no-change 구조 감사)

이번 slice는 `GrayscaleLearnView.xaml.cs`의 Partial을 Threshold, Brightness,
Filtering, Arithmetic 네 학습 주제 기준으로 다시 추적했다. 기존
`GrayscaleLearnPresenter`, WPF-free simulation model, Learn Window composition,
명시적 Apply/Close/Tool 이벤트가 이미 독립 경계를 제공하므로 운영 Partial을
파일 크기만으로 합치거나 삭제하지 않았다.

## 현재 구조와 owner

| 책임 | 현재 owner | 판정 |
| --- | --- | --- |
| 네 topic panel, named controls, cells, histogram, threshold marker, brush/text projection | `GrayscaleLearnView.xaml` + `.xaml.cs` | Required WPF presentation/framework adapter로 유지 |
| Threshold/Brightness/Arithmetic/Filtering 평가, 단계, cell role, formula/status와 Tool 안내 문구 | `GrayscaleLearnPresenter.cs` | WPF 없는 mutable lesson state/policy owner |
| 샘플·평가 primitive | `OpenVisionLearnBasicGrayscaleSimulationModel` via presenter | View Partial은 model을 직접 호출하지 않음 |
| Topic 선택과 related-tool callback composition | `OpenVisionLearnWindow.xaml.cs` + existing Learn policy/caller | Window composition owner |
| Apply/Close/Threshold Tool 결과 경계 | `GrayscaleLearnView` events -> `OpenVisionLearnWindow` handlers | View는 payload/result를 발신하고 Window가 public contract로 변환 |
| 80/420/520ms animation clock 및 Tick 수명 | `GrayscaleLearnView` | View-local WPF lifetime; Loaded/Unloaded 쌍으로 해제 |
| Threshold/brightness/arithmetic/filter 입력 facade와 visual state | View controls + presenter updates | 기존 public/internal test facade와 binding contract 유지 |

## 실제 호출 경로

```text
OpenVisionLearnWindow constructor
  -> grayscaleLearnView.InitializeThreshold(...)
  -> grayscaleLearnView.UpdateGuide/UpdateBrightnessGuide/UpdateArithmeticGuide/UpdateFilterGuide

OpenVisionLearnWindow.UpdateSelectedTopic
  -> GrayscaleLearnView.SelectTopic(topicIndex)

OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> GrayscaleLearnView.SetOpenRelatedToolAction(action)
  -> GrayscaleLearnView.OpenRelatedToolButton_Click
  -> action(VISION_MENU.Threshold | Mean | Histogram | Arithmetic | Filter)
  -> explicit hint/event after callback success

GrayscaleLearnView input/Timer
  -> GrayscaleLearnPresenter.Update*/Advance*/NextThresholdAnimationValue
  -> WPF cells, marker, formula, status, opacity and brush projection

GrayscaleLearnView.ApplyButton_Click
  -> ApplyThresholdRequested(this, OpenVisionLearnThresholdApplyEventArgs)
  -> OpenVisionLearnWindow.OnGrayscaleThresholdApplied
  -> existing Tool Learn controller/public Window event

GrayscaleLearnView.Unloaded / OpenVisionLearnWindow.OnClosed
  -> StopAnimations + Tick/event unsubscribe
```

## Binding/public/test contract

`GrayscaleLearnView.xaml`의 `x:Class`, topic panel names, threshold tabs,
ComboBox/Slider/Button event surface, AutomationId와 Apply/Close controls를
유지했다. `OpenVisionLearnWindow`의 public `*ForTest` facade는 threshold,
invert, formula, brightness/arithmetic/filter stage, Tool location과 Apply
호출을 기존 View에 전달한다. `LearnGrayscalePresentationContract`는 presenter
계산/단계/독립성, `LearnGrayscaleSmoke`는 Window 및 standalone View의 topic,
timer, hidden playback, callback 예외 순서, Apply/Close 결과를 검증한다.

## 왜 production split을 만들지 않았는가

View에는 `System.IO`, dialog, OpenCV, simulation model, persistence, Tool
factory, algorithm lifetime 호출이 없다. Threshold 입력과 네 topic의 표시
변환은 WPF control/namescope에 결합되어 있고, 실제 상태·평가·formula/status
정책은 이미 presenter에 있다. Apply/Close는 concrete Window를 참조하지 않는
명시적 event contract이며, Window가 public sender/result와 child lifetime을
소유한다. 새 ViewModel/service/interface/wrapper/Partial을 추가하면 기존
owner를 감싸는 forwarding 경계가 되므로 현재 dependency direction을
악화시킨다.

## 가장 짧은 읽기 순서

`rg -n "GrayscaleLearnView|GrayscaleLearnPresenter|ApplyThresholdRequested" src tools docs`
한 번으로 찾은 뒤 다음 순서로 읽는다.

1. `GrayscaleLearnView.xaml.cs` — controls, four timers, projection, events.
2. `GrayscaleLearnPresenter.cs` — four topic state/evaluation/formula policy.
3. `OpenVisionLearnBasicGrayscaleSimulationModel.cs` — WPF-free sample primitives.
4. `OpenVisionLearnWindow.xaml.cs` — topic/action composition, Apply/Close forwarding,
   public facade, close lifetime.
5. `GrayscaleLearnView.xaml` — binding/name/AutomationId contract.
6. `tools/VisionRecipeRunnerSmoke/GrayscaleLearnPartialBoundaryContract.cs`,
   `LearnGrayscalePresentationContract.cs`, and
   `tools/PipelineViewerScreenshotSmoke/LearnGrayscaleSmoke.cs` — focused proof.

## 검증

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --nologo` — 0 warnings/errors.
- `VisionRecipeRunnerSmoke.dll --grayscale-learn-partial-boundary-contract ...\debug` — `PASS|checks=11`.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Release --nologo` — 0 warnings/errors.
- `VisionRecipeRunnerSmoke.dll --grayscale-learn-partial-boundary-contract ...\release` — `PASS|checks=11`.
- `RunUiPrecheck.ps1` Debug: `wpf_openvision_learn_grayscale_contract` and `wpf_openvision_learn_grayscale_view` — both `OK`, `layout=0`, `text=0`, `internal=0`.
- `RunUiPrecheck.ps1` Release: same two targets — both `OK`, `layout=0`, `text=0`, `internal=0`.
- Dynamic monitor/window probe: selected smaller-left monitor `\\.\DISPLAY2`, bounds `X=-1920,Y=365,1920x1080`; after explicit placement, one visible intersecting Grayscale Learn smoke window — `PASS`.

증거 root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\grayscale-learn-partial-retention-20260914`.

## 미검증 범위

전체 WPF theme/DPI/input matrix, native related-tool click/file association,
camera/SDK/GPU, and long-running native runtime는 이 slice에서 검증하지 않았다.
따라서 결과는 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` 범위다.

## 구조 결론

현재 owner/call path/state/lifetime/public contract가 분리되어 있으며,
`GrayscaleLearnView` Partial은 required XAML/presentation adapter로 유지한다.
새 요구사항, 재현 결함, 실패한 criterion 또는 dependency/lifetime boundary
변경 없이는 이 경계를 다시 열지 않는다.
