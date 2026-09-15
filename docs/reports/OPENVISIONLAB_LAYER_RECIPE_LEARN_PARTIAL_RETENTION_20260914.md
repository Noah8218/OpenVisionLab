# OpenVisionLab Layer/Recipe Learn View Partial retention — PL-0051

상태: `Complete` (no-change 구조 감사)

이번 slice는 `LayerRecipeLearnView.xaml.cs`의 Partial을 Layer 흐름, Pipeline
Step 연결, Recipe 설명 및 애니메이션 기준으로 다시 추적했다. 기존
`LayerRecipeLearnPresenter`, Learn Window composition, WPF 셀/브러시 projection과
timer lifetime이 이미 독립 책임을 제공하므로 운영 Partial을 파일 크기만으로
합치거나 삭제하지 않았다.

## 현재 구조와 owner

| 책임 | 현재 owner | 판정 |
| --- | --- | --- |
| Layer/Recipe topic panel, named controls, layer/flow cells, formula/meaning/status projection | `LayerRecipeLearnView.xaml` + `.xaml.cs` | Required WPF presentation/framework adapter로 유지 |
| 고정 Layer 목록, Step Input/Tool/Output route, 선택 단계, route/row 판정, 설명/상태 문구 | `LayerRecipeLearnPresenter.cs` | WPF 없는 mutable lesson state/policy owner |
| 520ms animation clock, Tick 구독/해제와 Play/Pause/Step/Reset control lifetime | `LayerRecipeLearnView.xaml.cs` | View-local WPF lifetime; Loaded/Unloaded 쌍으로 해제 |
| Layer/Recipe topic visibility와 guide refresh | `OpenVisionLearnWindow.xaml.cs` + existing topic policy | Window composition owner |
| 선택 단계, formula, animation stage/status, reset/advance/toggle 접근 | View internal facade → Window public `*ForTest` facade | 기존 binding/public/test contract 유지 |

## 실제 호출 경로

```text
OpenVisionLearnWindow constructor
  -> layerRecipeLearnView (XAML child)
  -> UpdateSelectedTopic / ApplyTopicGuideUpdates
  -> layerRecipeLearnView.Visibility = presentation.ShowLayerRecipeTopic
  -> layerRecipeLearnView.RefreshSelection()

LayerRecipeLearnView slider/button/timer
  -> LayerRecipeLearnPresenter.SelectStep/ResetAnimation/AdvanceAnimation
  -> LayerRecipeLearnPresenter.Formula/Meaning/Status/route decisions
  -> WPF layer/flow cells, text, slider, and Play state projection

OpenVisionLearnWindow.OnClosed
  -> layerRecipeLearnView.StopAnimation()
  -> LayerRecipeLearnView.Unloaded
  -> timer Stop + Tick unsubscribe
```

## Binding/public/test contract

`LayerRecipeLearnView.xaml`의 `x:Class`, topic panel, Layer/flow grids,
slider, Play/Step/Reset buttons, status text, `ValueChanged`/`Click` handlers와
AutomationId를 유지했다. `OpenVisionLearnWindow`는 topic visibility, refresh,
close lifetime과 `LayerRecipe*ForTest` facade를 유지한다.
`LearnLayerRecipeContract`는 route/formula, highlight, reset/restart, rounding과
instance isolation을 검증하고, `LearnLayerRecipeSmoke`는 Window와 standalone
View의 초기화, unload/reload, duplicate Tick, Play/Pause, slider stop, topic
return, close cleanup과 fresh PNG를 검증한다.

## 왜 production split을 만들지 않았는가

View에는 `System.IO`, dialog, OpenCV, persistence, real Recipe execution,
Tool factory 또는 algorithm lifetime 호출이 없다. Layer/flow cell과 brush/text
변환은 WPF namescope에 결합되어 있고, route/state/formula/status 정책은 이미
`LayerRecipeLearnPresenter`가 소유한다. Learn Window가 topic visibility와
public facade/child lifetime을 소유하므로 새 ViewModel/service/interface/
forwarding wrapper를 추가하면 기존 owner를 감싸는 seam이 된다. 현재 구조에서
독립 state/lifetime/test 경계를 더 만들 근거가 없어 no-change로 유지한다.

## 가장 짧은 읽기 순서

`rg -n "LayerRecipeLearnView|LayerRecipeLearnPresenter|LayerRecipe" src tools docs`
한 번으로 찾은 뒤 다음 순서로 읽는다.

1. `LayerRecipeLearnView.xaml.cs` — WPF controls, cells, timer, projection, test facade.
2. `LayerRecipeLearnPresenter.cs` — route/state/formula/meaning/status policy.
3. `OpenVisionLearnWindow.xaml.cs` — topic visibility, refresh, public facade, close lifetime.
4. `LayerRecipeLearnView.xaml` — namescope, binding/event/AutomationId contract.
5. `tools/VisionRecipeRunnerSmoke/LayerRecipeLearnPartialBoundaryContract.cs`,
   `LearnLayerRecipeContract.cs`, and
   `tools/PipelineViewerScreenshotSmoke/LearnLayerRecipeSmoke.cs` — focused proof.

## 검증

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --nologo` — 0 warnings/errors.
- `VisionRecipeRunnerSmoke.dll --layer-recipe-learn-partial-boundary-contract ...\\debug` — `PASS|checks=12`.
- `VisionRecipeRunnerSmoke.dll --learn-layer-recipe-contract ...\\debug\\learn-layer-recipe-contract` — 4 passed, 0 failed.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Release --nologo` — 0 warnings/errors.
- Release Layer/Recipe boundary contract — `PASS|checks=12`; existing presenter contract — 4 passed, 0 failed.
- `RunUiPrecheck.ps1` Debug: `wpf_openvision_learn_layer_recipe_contract` and `wpf_openvision_learn_layer_recipe_view` — both `OK`, layout/text/internal counts zero.
- `RunUiPrecheck.ps1` Release: same two targets — both `OK`, layout/text/internal counts zero.
- Dynamic Debug and Release monitor/window probes: two monitors detected, smaller-left `\\.\\DISPLAY2` selected (`X=-1920,Y=365,1920x1080`); after explicit placement each visible smoke window intersected the selected monitor — `PASS`.

증거 root: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\layer-recipe-learn-partial-retention-20260914`.

## 미검증 범위

전체 WPF theme/DPI/input matrix, native Tool/file association, camera/SDK/GPU,
실제 Recipe execution/persistence와 long-running native runtime은 이 slice에서
검증하지 않았다. 따라서 결과는 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요` 범위다.

## 구조 결론

현재 owner/call path/state/lifetime/public contract가 분리되어 있으며,
`LayerRecipeLearnView` Partial은 required XAML/presentation adapter로 유지한다.
새 요구사항, 재현 결함, 실패한 criterion 또는 dependency/lifetime boundary
변경 없이는 이 경계를 다시 열지 않는다.
