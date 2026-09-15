# OpenVisionLab 2D Shell·Pipeline Review code-behind 경계 최종 감사

Updated: 2026-09-10 KST

Status: VERIFIED as a source-level boundary audit. No additional code split was
justified after R10–R13. Full WPF theme/DPI/input/long-run qualification remains
unverified.

## Audit scope

이번 slice는 다음 production View를 다시 읽어 남은 직접 호출을 분류했다.

- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.Interactions.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.Events.cs`

R10–R13에서 실제로 확인된 업무 판단·persistence는 기존
`ToolWindowController`, Recipe/Workspace controller, Shell ViewModel owner로
이동했으므로 이번 감사에서는 이름이나 파일 길이만으로 다시 나누지 않았다.

## Remaining production View responsibilities

| 영역 | 남은 직접 코드 | 판정 |
| --- | --- | --- |
| Shell | WPF dialog/file picker, MessageBox confirmation, external evidence folder open | Window owner와 control event에 종속된 UI adapter. 기존 `Func` callback으로 command surface에 결과만 전달 |
| Shell | control layout, recipe panel drag, log panel visibility, localization | View 자체의 시각 상태. Domain/Recipe/Inspection 정책을 변경하지 않음 |
| Shell | `GetLayerImage`를 통한 layer-card preview projection | 새 이미지 owner가 아니라 표시용 snapshot/preview factory 연결 |
| Pipeline Review | Details/Guide/Step Flow toggle와 row layout | `OpenVisionPipelineReviewView`가 소유해야 하는 WPF layout state |
| Pipeline Review | object/geometry/circle hit-test와 highlight Bitmap 교체·Dispose | 화면 입력 좌표와 표시 이미지 수명에 종속된 View responsibility |
| Pipeline Review | scale 입력의 양수 검증과 event args 생성 | 화면 입력을 유효한 요청으로 변환하고 저장/apply는 Document가 수행 |
| Pipeline Review | Run/Edit/Return/Learn/Fixture 버튼 event forwarding | 기존 `OpenVisionPipelineReviewDocument` callback contract 유지 |

다음 production View에서는 아래 업무·저장 신호가 발견되지 않았다.

- `VisionPipelineStorage` 직접 호출
- `VisionPipelineExecutionService` 직접 호출
- `Recipe.SaveTools`, `System.SaveConfig` 직접 호출
- `OpenVisionNativeToolSettingsStore.Load` 또는 `RequiresInputLayerB` 직접 호출

## Partial audit

전체 현재 source audit의 partial 선언은 110개이며, 대부분 WPF/XAML 또는
기존 generator/framework composition입니다. 이번 범위에서 확인한 수동
partial은 `OpenVisionShellHostView.Interactions.cs`, Shell test hooks,
Recipe command-surface responsibility files, `OpenVisionPipelineReviewView.Events.cs`,
그리고 `OpenVisionPipelineReviewDocument.Events.cs`입니다. 이 파일들은 각각
UI event/lifecycle, test-only facade, Recipe command responsibility, Pipeline
Review event/input, Document event bridge를 나누며 private domain state를 새
업무 owner로 위장하지 않습니다. R14에서는 새 partial을 만들지 않았고,
기존 partial을 합치거나 이름만 바꾸지 않았습니다.

## Ownership and reading order

```text
Shell composition
  -> OpenVisionShellHostView.xaml.cs
  -> OpenVisionShellHostView.Interactions.cs (UI adapter only)
  -> existing Shell controller/ViewModel owner

Pipeline Review
  -> OpenVisionPipelineReviewDocument
  -> OpenVisionPipelineReviewExecutionController / presenters
  -> OpenVisionPipelineReviewView (display and input adapter)
```

가장 짧은 확인 명령은 다음과 같습니다.

```powershell
rg -n "ShowSelectedTool|RefreshToolReadiness|SaveRuntimeRecipeTools|SaveTools|RequiresInputLayerB|OnReviewDetailsToggleChanged|ScaleCalibrationRequested" src/OpenVisionLab/UI/Menu/Wpf
```

## Verification and boundary

- Direct-signal search recorded no prohibited production View calls in
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r14-codebehind-audit-20260910`.
- Existing R13 Debug/Release app builds, readiness contract, Recipe smoke,
  documentation index, and refactor audit remain the focused gates for this
  source-only closure.
- Evidence summary:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r14-codebehind-audit-20260910\summary.txt`
- This is source-level proof. Alternate themes, Wide/Compact layouts, 100/125/
  150/175/200% DPI, physical pointer/keyboard, camera/SDK, and long-run native
  shutdown remain `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## Self-evaluation

For the stated junior-readability concerns, the remaining View code has a clear
reason to exist and a named downstream owner. Moving it again would either put
WPF control state into a service or create a forwarding wrapper without a new
test or lifetime boundary. The Shell constructor is still dense, but its
composition-root role is explicit and the feature paths now have direct named
entry points and owner documents. The autonomous refactor schedule can pause
after this audit if no new reproducible defect or changed requirement appears.
