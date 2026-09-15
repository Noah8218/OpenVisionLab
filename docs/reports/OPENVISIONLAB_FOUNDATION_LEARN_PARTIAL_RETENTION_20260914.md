# OpenVisionLab Foundation Learn View Partial retention — PL-0049

상태: `Complete` (no-change 구조 감사)

이번 slice는 `FoundationLearnView.xaml.cs`의 Partial을 다시 추적해 실제
책임 충돌과 독립 state/lifetime/test seam이 있는지 확인했다. 기존 concrete
presenter와 Learn Window composition이 이미 경계를 제공하므로 운영 코드의
Partial을 기계적으로 합치거나 삭제하지 않았다.

## 현재 구조와 owner

| 책임 | 현재 owner | 판정 |
| --- | --- | --- |
| XAML namescope, topic panel, Mat/ROI 셀, marker/transform, brush와 button projection | `FoundationLearnView.xaml` + `.xaml.cs` | Required WPF presentation/framework adapter로 유지 |
| Point/ROI와 Mat channel lesson stage, role/visibility 결정, 문구와 Tool 위치 안내 | `FoundationLearnPresenter.cs` | WPF 없는 lesson state/policy owner |
| Topic 선택과 related-tool callback composition | `OpenVisionLearnWindow.xaml.cs` + 기존 Learn topic policy/caller | Window composition owner; View는 callback을 저장·전달만 함 |
| 520/620ms animation clock과 Tick 구독 수명 | `FoundationLearnView` | View-local visual lifetime; `Loaded`/`Unloaded` 쌍으로 해제 |
| 셀/marker/text/opacity/stroke 렌더링 | `FoundationLearnView` | Presenter 결과를 WPF 요소에 투영 |
| 계산·저장·검사 알고리즘·Tool 생성 | 기존 presenter/caller 및 제품 Tool composition | Foundation View Partial은 소유하지 않음 |

## 실제 호출 경로

```text
OpenVisionLearnWindow.UpdateSelectedTopic
  -> FoundationLearnView.SelectTopic(topicIndex)

OpenVisionLearnWindow.SetOpenRelatedToolAction(action)
  -> FoundationLearnView.SetOpenRelatedToolAction(action)
  -> FoundationLearnView.OpenRelatedToolButton_Click
  -> action(VISION_MENU.Blob | Filter | RotateAndScale)
  -> existing Learn/Shell Tool composition

FoundationLearnView constructor
  -> FoundationLearnPresenter
  -> BuildFoundationCells / UpdateFoundationGuide / UpdateMatChannelGuide
  -> DispatcherTimer Tick -> presenter.Advance* -> WPF projection
  -> Unloaded -> StopAnimations + Tick unsubscribe
```

## Binding/public/test contract

`FoundationLearnView.xaml`의 `x:Class`, `Focusable`, named grid/panel,
AutomationId와 Play/Step/Reset/related-tool event surface를 유지했다.
`OpenVisionLearnWindow`의 public `*ForTest` facade는 View의 단계·marker·문구
접근자를 전달하며, lesson mutable state는 `FoundationLearnPresenter`에 남아
있다. 기존 `LearnFoundationPresentationContract`와
`wpf_openvision_learn_foundation_{contract,view}` smoke가 이 경계를 사용한다.

## 왜 production split을 만들지 않았는가

View에는 `System.IO`, dialog, OpenCV, simulation, persistence, Tool factory,
알고리즘 lifetime 호출이 없다. Presenter가 이미 독립적으로 계산·단계·문구를
소유하고, View의 remaining state는 WPF control/timer lifetime과 XAML
namescope에 결합되어 있다. 새 ViewModel/service/interface/wrapper/Partial을
추가하면 기존 owner를 감싸는 forwarding 경계가 되므로 추가 이동은 YAGNI이며
현재 구조의 dependency direction을 악화시킨다.

## 가장 짧은 읽기 순서

`rg -n "FoundationLearnView|FoundationLearnPresenter" src tools docs` 한 번으로
찾은 뒤 다음 순서로 읽는다.

1. `FoundationLearnView.xaml.cs` — controls, cell/brush projection, timer and callback.
2. `FoundationLearnPresenter.cs` — stage state, role decisions, text and Tool guidance.
3. `OpenVisionLearnWindow.xaml.cs` — topic/action composition and public facade.
4. `FoundationLearnView.xaml` — binding/name/AutomationId contract.
5. `tools/VisionRecipeRunnerSmoke/FoundationLearnPartialBoundaryContract.cs`와
   기존 `LearnFoundationPresentationContract.cs` — focused proof.

## 검증

- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Debug --nologo` — 0 warnings/errors.
- `VisionRecipeRunnerSmoke.dll --foundation-learn-partial-boundary-contract ...\debug` — `PASS|checks=9`.
- `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj --configuration Release --nologo` — 0 warnings/errors.
- `VisionRecipeRunnerSmoke.dll --foundation-learn-partial-boundary-contract ...\release` — `PASS|checks=9`.
- `RunUiPrecheck.ps1` Debug: `wpf_openvision_learn_foundation_contract` and `wpf_openvision_learn_foundation_view` — both `OK`, `layout=0`, `text=0`, `internal=0`.
- `RunUiPrecheck.ps1` Release: same two targets — both `OK`, `layout=0`, `text=0`, `internal=0`.
- Dynamic monitor/window probe: selected smaller left monitor `\\.\DISPLAY2`, bounds `X=-1920,Y=365,1920x1080`; after explicit placement, one visible intersecting window — `PASS`. Initial unplaced probe was rejected because the window opened on the other monitor; the moved rerun is the accepted evidence.

증거 root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\foundation-learn-partial-retention-20260914`.

## 미검증 범위

전체 WPF theme/DPI/input matrix, native related-tool click/file association,
camera/SDK/GPU, and long-running native runtime는 이 slice에서 검증하지 않았다.
따라서 결과는 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` 범위다.

## 구조 결론

현재 owner/call path/state/lifetime/public contract가 분리되어 있으며,
`FoundationLearnView` Partial은 required XAML/presentation adapter로 유지한다.
새 요구사항, 재현 결함, 실패한 criterion 또는 dependency/lifetime boundary
변경 없이는 이 경계를 다시 열지 않는다.
