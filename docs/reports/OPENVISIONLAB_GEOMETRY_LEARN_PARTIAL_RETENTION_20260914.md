# OpenVisionLab Geometry Learn View Partial retention — PL-0052

상태: `Complete` (no-change 구조 감사)

이번 slice는 `GeometryLearnView.xaml.cs`의 Partial을 Rotate/Scale 학습,
Affine 관련 Tool 안내, ROI 좌표 재검토, 애니메이션 및 WPF lifetime 기준으로
다시 추적했다. 기존 `GeometryLearnPresenter`, Learn Window composition,
WPF transform/brush projection과 timer lifetime이 이미 독립 책임을 제공하므로
운영 Partial을 파일 크기만으로 합치거나 삭제하지 않았다.

## 현재 구조와 owner

| 책임 | 현재 owner | 판정 |
| --- | --- | --- |
| Geometry topic panel, sliders, transform shapes, formula/meaning/status/tool location projection | `GeometryLearnView.xaml` + `.xaml.cs` | Required WPF presentation/framework adapter로 유지 |
| Angle/Scale state, Rotate→Scale→ROI review stage, output size, semantic roles, formula/status, Tool hint policy | `GeometryLearnPresenter.cs` | WPF 없는 mutable lesson state/policy owner |
| 520ms animation clock, Tick 구독/해제와 Play/Pause/Step/Reset lifetime | `GeometryLearnView.xaml.cs` | View-local WPF lifetime; Loaded/Unloaded 쌍으로 해제 |
| Geometry topic visibility, guide refresh, related-tool callback composition | `OpenVisionLearnWindow.xaml.cs` + existing topic policy | Window composition owner |
| Angle/Scale, formula, stage, rendered transform, Tool location, reset/advance/toggle access | View internal facade → Window public `*ForTest` facade | 기존 binding/public/test contract 유지 |

## 실제 호출 경로

```text
OpenVisionLearnWindow constructor
  -> geometryLearnView.UpdateGeometryGuide()
  -> UpdateSelectedTopic -> geometryLearnView.SelectTopic(topicIndex)
  -> ApplyTopicGuideUpdates -> geometryLearnView.UpdateGeometryGuide()

OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> GeometryLearnView.SetOpenRelatedToolAction(action)
  -> GeometryLearnView.OpenRelatedToolButton_Click
  -> action(VISION_MENU.RotateAndScale | AffineTransform)
  -> GeometryLearnPresenter.UpdateToolLocation -> WPF hint projection

GeometryLearnView slider/button/timer
  -> GeometryLearnPresenter.UpdateSettings/ResetAnimation/AdvanceAnimation
  -> WPF source/target roles, RotateTransform/ScaleTransform, formula/status

OpenVisionLearnWindow.OnClosed
  -> geometryLearnView.StopAnimations()
  -> GeometryLearnView.Unloaded
  -> timer Stop + Tick unsubscribe
```

## Binding/public/test contract

`GeometryLearnView.xaml`의 `x:Class`, topic panel, related Tool buttons,
tool-location panel, Angle/Scale sliders, Play/Step/Reset controls,
`ValueChanged`/`Click` handlers와 AutomationId를 유지했다.
`OpenVisionLearnWindow`는 topic selection, callback injection, public
`Geometry*ForTest` facade와 child close lifetime을 유지한다.
`LearnGeometryPresentationContract`는 stage/role, culture-invariant text,
output dimensions, instance isolation과 Tool hint policy를 검증하고,
`LearnGeometrySmoke`/`LearnGeometryViewSmoke`는 Window와 standalone View의
transform projection, callback ordering, hidden-topic retention, keyboard
focus, unload/reload, duplicate Tick 및 close cleanup을 검증한다.

## 왜 production split을 만들지 않았는가

View에는 `System.IO`, dialog, OpenCV, persistence, real Recipe execution,
Tool factory 또는 algorithm lifetime 호출이 없다. Source/target transform과
brush/text는 WPF namescope에 결합되어 있고, Angle/Scale/stage/role/formula/
status/Tool hint 정책은 이미 `GeometryLearnPresenter`가 소유한다. Learn Window가
topic visibility, callback composition, public facade와 child lifetime을
소유하므로 새 ViewModel/service/interface/forwarding wrapper를 추가하면
기존 owner를 감싸는 seam이 된다. 현재 구조에서 독립 state/lifetime/test
경계를 더 만들 근거가 없어 no-change로 유지한다.

## 가장 짧은 읽기 순서

`rg -n "GeometryLearnView|GeometryLearnPresenter|GeometryTransform" src tools docs`
한 번으로 찾은 뒤 다음 순서로 읽는다.

1. `GeometryLearnView.xaml.cs` — controls, transforms, timer, callback, test facade.
2. `GeometryLearnPresenter.cs` — settings/stage/role/formula/status/hint policy.
3. `OpenVisionLearnWindow.xaml.cs` — topic selection, callback composition, public facade, close lifetime.
4. `GeometryLearnView.xaml` — namescope, binding/event/AutomationId contract.
5. `tools/VisionRecipeRunnerSmoke/GeometryLearnPartialBoundaryContract.cs`,
   `LearnGeometryPresentationContract.cs`,
   `tools/PipelineViewerScreenshotSmoke/LearnGeometrySmoke.cs`, and
   `LearnGeometryViewSmoke.cs` — focused proof.

## 검증

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --nologo` — 0 warnings/errors.
- `VisionRecipeRunnerSmoke.dll --geometry-learn-partial-boundary-contract ...\\debug` — `PASS|checks=12`.
- `VisionRecipeRunnerSmoke.dll --learn-geometry-presentation-contract ...\\debug\\learn-geometry-presentation-contract` — 4 passed, 0 failed.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Release --nologo` — 0 warnings/errors.
- Release Geometry boundary contract — `PASS|checks=12`; existing presenter contract — 4 passed, 0 failed.
- `RunUiPrecheck.ps1` Debug: `wpf_openvision_learn_geometry_contract` and `wpf_openvision_learn_geometry_view` — both `OK`, layout/text/internal counts zero.
- `RunUiPrecheck.ps1` Release: same two targets — both `OK`, layout/text/internal counts zero.
- Dynamic Debug and Release monitor/window probes: two monitors detected, smaller-left `\\.\\DISPLAY2` selected (`X=-1920,Y=365,1920x1080`); after explicit placement each visible smoke window intersected the selected monitor — `PASS`.

증거 root: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\geometry-learn-partial-retention-20260914`.

## 미검증 범위

전체 WPF theme/DPI/input matrix, native Tool/file association, camera/SDK/GPU,
실제 Rotate/Scale·Affine Recipe execution/persistence와 long-running native
runtime은 이 slice에서 검증하지 않았다. 따라서 결과는
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` 범위다.

## 구조 결론

현재 owner/call path/state/lifetime/public contract가 분리되어 있으며,
`GeometryLearnView` Partial은 required XAML/presentation adapter로 유지한다.
새 요구사항, 재현 결함, 실패한 criterion 또는 dependency/lifetime boundary
변경 없이는 이 경계를 다시 열지 않는다.
