# OpenVisionLab WPF View MVVM·이동 가능성 감사

## 감사 기준

- 감사일: 2026-09-13 KST
- 저장소: `C:\Git\2D\Dev`
- 브랜치: `codex/public-sample-ux-docs`
- 기준 커밋: `176eec95 refactor: complete junior discoverability and reliability cleanup`
- 실행 프로젝트: `src/OpenVisionLab/OpenVisionLab.csproj`
- 대상 프레임워크: `.NET 8 / WPF / net8.0-windows7.0`
- 범위: `src`의 XAML·code-behind·ViewModel·WPF 인프라와 화면 호출 경로
- 제외: 사용자 입력이 필요한 제품 실행, 카메라/SDK/GPU, 장시간 실행, 전체 테마·DPI·다중 모니터 런타임 자격 검증

이 문서는 구현 완료 보고서가 아니라 소스 기준 구조 감사입니다. 문서의 설계 설명은 현재 동작의 근거가 아니며, 호출 경로·소스·정적 감사 결과를 근거로 판단했습니다.

## 결론

OpenVisionLab의 View 전체가 순수한 `View -> ViewModel -> Service` 형태인 것은 아닙니다. 현재 제품은 다음을 함께 사용하는 **혼합 MVVM 구조**입니다.

```text
Shell composition View
  + MVVM binding ViewModel
  + Tool Presenter/Controller
  + PropertyGrid/custom-control contract
  + Window/Dispatcher/Bitmap lifetime adapter
```

이 혼합은 전부 실패한 MVVM이 아닙니다. PropertyGrid 기반 Tool, Learn 교육 화면, Docking control, Shell 조합부는 WPF lifecycle과 기존 공개 계약을 View에서 연결해야 합니다. 다만 “XAML과 `.xaml.cs` 한 쌍만 복사하면 다른 화면에서 바로 쓸 수 있는가?”라는 기준에는 대부분 `아니오`입니다. 실제 재사용 단위는 단일 View가 아니라 책임이 선언된 기능 묶음입니다.

현재 소스 기준으로 P0 결함은 발견하지 못했습니다. 다음 세 곳은 실제 책임 혼재가 확인된 P1 후보입니다.

1. `OpenVisionRecipeRunEvidenceViewerView.xaml.cs`가 파일을 직접 열고 `System.Drawing.Bitmap`을 디코딩하며 상태와 Dispose까지 소유합니다.
2. `LineToolWpfView.xaml.cs`가 `OpenVisionNativeToolPropertySessionStore.Save`를 직접 호출합니다.
3. `RoiImageCanvasViewModel.cs`가 WPF 입력·컨트롤·OpenCvSharp Mat·파일명/대화상자 경계를 함께 소유합니다. 기존 감사에서도 유일한 직접 ViewModel UI 신호로 남아 있습니다.

이 세 항목은 이번 감사에서 코드를 옮기지 않았습니다. 각각 기존 concrete owner와 공개 계약을 먼저 대조해야 하며, 완료된 Shell/Pipeline Review owner를 파일 크기만으로 다시 열 근거는 확인되지 않았습니다.

## 전수조사 결과

정량 감사 명령:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File `
  .\tools\RefactorAudit\Invoke-RefactorAudit.ps1 `
  -RepositoryRoot C:\Git\2D\Dev `
  -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\mvvm-view-audit-20260913 `
  -Verify
```

결과:

```text
REFACTOR_AUDIT=PASS|CSharpFiles=822|XamlFiles=60|PartialDeclarations=59|PartialTextMatches=2|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0
```

| 항목 | 확인값 | 해석 |
| --- | ---: | --- |
| C# 파일 | 822 | `src`·`tools`의 `bin/obj` 제외 |
| XAML 파일 | 60 | 실제 화면 XAML 56개 + ResourceDictionary 4개 |
| View code-behind | 56개 | 모든 화면 XAML의 `.xaml.cs` 쌍을 확인 |
| ViewModel 파일 | 30개 | 직접 UI/대화상자 신호 1개, 직접 파일/경로 신호 5개 |
| Partial 선언 | 59개 | XAML/generated/framework/cohesive composition 중심; 수동 분할 sprawl 증거 없음 |
| 프로젝트/참조 | 27 / 34 | 외부 프로젝트 참조 0, 순환 0 |
| 대형 파일 | 1,000줄 이상 28개 | 파일 크기만으로 owner를 재분할하지 않음 |

XAML 60개는 다음처럼 분류했습니다. `tools`의 2개 XAML은 제품 화면이 아니라
검증용 smoke host이므로 제품 View 이동성 판정에서 별도로 제외했습니다.

| 물리 영역 | XAML 수 | 포함 책임 |
| --- | ---: | --- |
| `src/OpenVisionLab/UI/Menu/Wpf` | 17 | Shell, Recipe, Viewer, Window, Integration, Sample |
| `src/OpenVisionLab/UI/VisionTest/Wpf/Learn` | 10 | Learn Window, 8개 topic View, LearnResources |
| `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling` | 6 | Tool shell, guide, signal, theme |
| `src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews` | 13 | 실제 검사 Tool View |
| `src/OpenVisionLab/UI/Popup/Wpf` | 3 | Image Compare, ROI, OpenGL editor |
| `src/Libraries/OpenVisionLab.ImageCanvas` | 4 | ROI/teaching/image canvas |
| `src/Libraries/OpenVisionLab.Docking.Controls` | 3 | Dock workspace, overlay, resource dictionary |
| `src/Libraries/OpenVisionLab.Logging.Controls` | 1 | Log Panel |
| `src/OpenVisionLab/UI/Wpf` | 1 | 제품 공통 theme dictionary |
| `tools/*Smoke` | 2 | 테스트 전용 Window/App host |

XAML 밖의 code-only WPF control인 `ImageCanvasControl`, `PipelineFlowView`,
`PropertyGrid`, `VisionToolSignalPlotSurface`, `VisionToolInlinePreviewSlot`도
확인했습니다. 이 타입들은 ViewModel이 아니라 control contract와 rendering/input
수명을 제공하므로, 화면 MVVM 감사에서 별도 화면으로 세지 않고 해당 library/tool
모듈의 이동 의존성으로 기록했습니다.

## View 가족별 MVVM 판정

판정은 다음 세 등급을 사용합니다.

- **A — 모듈 owner가 명확함**: View는 바인딩·UI lifecycle에 집중하고, 선언된 ViewModel/controller/resource owner와 함께 재사용할 수 있습니다.
- **B — 의도된 혼합 adapter**: View 자체에 업무 규칙을 두지는 않지만 Presenter/Controller/custom-control 계약을 함께 가져야 합니다.
- **C — 책임 혼재**: 파일/상태/업무 또는 native resource 경계가 View에 남아 있어 이동 전에 기존 concrete owner로 책임을 옮길 근거가 있습니다.

| View 가족 | 대표 경로 | 현재 판정 | 단일 View 이동 |
| --- | --- | --- | --- |
| 시작·Shell | `Program.Main` → `OpenVisionLabApplication.Run` → `OpenVisionShellHostWindow` → `OpenVisionShellHostView` | B, 조합 root | 불가. RuntimeContext, Session, Document, Tool registry, Docking, lifecycle을 함께 구성 |
| Shell Preview/Layer Viewer | `ShellPreviewView`, `OpenVisionLayerViewerView` | B, 명시적 viewer/presenter/lifetime owner | View + presenter + ImageCanvas/Display contract 필요 |
| Recipe binding fragment | `OpenVisionRecipeBasicLifecycleView`, `OpenVisionRecipeValidationSuiteView` | A- (부모 DataContext 의존) | 단독 불가. `RecipeCommands`를 제공하는 Shell DataContext가 필요 |
| Recipe dialog | `OpenVisionRecipePendingEditDialog` | A | VM, request/decision model, localization과 함께 이동 가능 |
| Recipe evidence review | `OpenVisionRecipeRunEvidenceViewerView` | C | 현재 View가 파일 decode와 Bitmap 상태를 직접 소유하므로 단독 이동 부적합 |
| Pipeline Review | `OpenVisionPipelineReviewView` | B | Document, ViewModel, `LayoutController`, `ImageResourceOwner`, Pipeline.Controls와 함께 이동 |
| Tool View | `Threshold`, `Line`, `Matching`, `Blob`, `Contour`, `Filter`, `Morphology`, `Arithmetic`, `AffineTransform`, `FeatureMatching`, `EdgeBasedMatching`, `SimplePreprocess`, `AutoMPointTeachingPanel` | B | 공통 base + presenter/controller + tool model + PropertyGrid + registry/lifetime 필요 |
| Learn | `OpenVisionLearnWindow` 및 8개 topic View | B | 교육 기능 묶음으로는 가능하지만, topic View 한 파일만으로는 불가. Presenter, resource, topic navigation 필요 |
| Image/ROI editor | `RoiEditorWindow`, `OpenGlTemplateEditorWindow`, `RoiImageCanvasView` | B | ImageCanvas/OpenGL/PropertyGrid/native image contract와 함께 이동 |
| Image Compare | `ImageCompareWindow` | A- | VM + `ImageCompareDirectoryPolicy` + `ImageCompareImageResource` + dialog host 필요 |
| Log Panel | `LogPanelView` | A- | VM + `LogPanelFileAccess` + RuntimeLogStream 필요 |
| Sample Picker | `OpenVisionWorkspaceSamplePickerWindow/View` | A- | VM, sample model/catalog, Window host와 함께 이동 |
| Docking controls | `OpenVisionLayerDockWorkspaceView`, guide overlay | A/B | Docking.Controls 프로젝트와 ResourceDictionary/DP·event contract 필요 |
| Window chrome·floating·startup | `OpenVisionShellHostWindow`, `OpenVisionFloatingToolWindow`, `OpenVisionWindowTitleBar`, `OpenVisionStartupLoadingWindow` | B, infrastructure | Shell host/lifecycle/theme contract 없이는 이동 불가 |
| Signal/guide/custom controls | `VisionToolSignalInspectorView`, parameter/verification guide, single/double input shell | B | Tooling base, exporter/property contract, theme/resource 필요 |

따라서 “모든 View가 MVVM인가?”의 답은 **아니오**이고, “업무 판단이 전부 View에 있는가?”의 답도 **아니오**입니다. 대부분은 기존 Presenter/Controller/VM으로 이동되어 있으며, 남은 C 등급만 우선 수정 대상입니다.

## 핵심 호출 경로와 첫 탐색 순서

처음 Visual Studio에서 솔루션을 연 개발자는 다음 순서로 읽으면 됩니다.

1. `AGENTS.md` → `docs/README.md` → `docs/LLM_DOCUMENT_INDEX.json`
2. `OpenVisionLab.sln` → 실행 프로젝트 `src/OpenVisionLab/OpenVisionLab.csproj`와 `ProjectReference`
3. `src/OpenVisionLab/Program.cs`
4. `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`
5. `src/OpenVisionLab/UI/Menu/Wpf/Windows/OpenVisionShellHostWindow.xaml(.cs)`
6. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml(.cs)`
7. 목적에 따라 다음 한 번의 검색을 수행합니다.

| 목적 | 검색 시작점 | 다음 owner |
| --- | --- | --- |
| Image → Layer | `LoadImage`, `ApplyMainLayerImage`, `DisplayManagerService` | ImageCanvas / ImageSpace / Display owner |
| Layer → Tool | `OpenVisionNativeToolRegistry`, `ShowSelectedTool` | Tool document/cache → Tool View base/controller/presenter |
| Tool → Inspection | `Preview` 또는 `Run` command | 기존 tool model/SDK contract → Pipeline execution |
| Recipe | `RecipeCommandSurface.cs`의 command/selection | `OpenVisionRecipeExecutionSessionViewModel` → 기존 Recipe execution owner |
| Pipeline Review | `ShowPipelineReview`, `OpenVisionPipelineReviewDocument` | Review View → ViewModel / `LayoutController` / `ImageResourceOwner` |
| Result → Review | `OpenVisionRecipeRunEvidenceViewerController` | evidence model → viewer; 현재 bitmap file decode 경계는 P1 후보 |

가장 짧은 제품 진입 경로는 다음입니다.

```text
Program.Main
  -> OpenVisionLabApplication.Run
  -> OpenVisionShellHostWindow
  -> OpenVisionShellHostView
  -> Core / Pipeline / Execution
  -> RecipeCommandSurface child owners
  -> Pipeline Review Document / ExecutionController / LayerImageOwner
  -> focused contract / smoke target
```

`OpenVisionShellHostView` 생성자는 약 2,082줄 파일의 조합부이고, `OpenVisionPipelineReviewView`는 약 1,716줄의 UI adapter입니다. 이 숫자만으로 다시 partial이나 Manager를 추가하면 owner가 더 숨겨집니다. 이미 `LayoutController`, `ImageResourceOwner`, Recipe panel drag controller로 독립 상태·수명·테스트 경계가 증명된 부분은 유지합니다.

## 실제 발견 항목

### P1 — Recipe run evidence View의 파일·Bitmap 책임

`src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeRunEvidenceViewerView.xaml.cs`의 `TrySetEvidence`는 `LoadBitmap`을 호출하고, `LoadBitmap`은 `FileStream`과 `System.Drawing.Image.FromStream`으로 경로를 직접 읽습니다. 같은 View가 `LoadError`, 두 viewer, Bitmap replacement와 Dispose를 함께 관리합니다.

이 View는 “화면에 표시”와 “evidence 파일을 읽고 디코드”하는 변경 이유가 분리되어 있습니다. 다음 작업에서는 기존 `OpenVisionRecipeRunEvidenceViewerController` 또는 기존 image resource concrete owner 중 하나를 재사용해 파일 입력·디코드·수명 계약을 이동하고, View에는 표시 상태와 UI lifecycle만 남기는 것이 기준입니다. 새 `I...Service` 계층은 현재 근거가 없습니다.

### P1 — Line Tool View의 저장 호출

`src/OpenVisionLab/UI/VisionTest/Wpf/ToolViews/LineToolWpfView.xaml.cs`의 `PersistLineProperties`가 `OpenVisionNativeToolPropertySessionStore.Save`를 직접 호출합니다. PropertyGrid 변경·preset·sample 적용 이벤트는 View에서 시작할 수 있지만, 저장 정책과 실패 처리는 View가 소유하면 화면 이동과 테스트가 어려워집니다.

다음 작업은 기존 tool session/controller가 이미 같은 계약을 제공하는지 확인한 뒤 최소 이동해야 합니다. 저장 형식과 사용자 workflow는 유지합니다.

### P1 — RoiImageCanvasViewModel의 UI/native/path 혼합

`src/Libraries/OpenVisionLab.ImageCanvas/ViewModel/RoiImageCanvasViewModel.cs`는 `ImageCanvasControl`, WPF keyboard input controller, context menu/dialog host, OpenCvSharp 및 파일명/path helper를 함께 참조합니다. 정적 감사의 `ViewModelUiIoFiles=1`이 이 파일을 보고합니다.

이는 View 하나의 이동 문제보다 라이브러리 경계 문제입니다. 먼저 현재 consumer와 Mat/Bitmap 소유권을 재현 가능한 테스트로 고정한 다음, 독립 상태 owner를 증명할 때만 분리합니다. 이번 감사에서는 unsafe/pointer나 새 추상화를 추가하지 않았습니다.

### P2 — 첫 탐색 비용

- Shell은 상태 소유자보다 조합 순서를 먼저 읽어야 합니다.
- Tool은 MVVM ViewModel 대신 base/controller/presenter/PropertyGrid 계약을 따라가야 합니다.
- Learn은 DataContext/Binding이 거의 없고 topic Presenter와 DispatcherTimer가 있으므로 순수 MVVM으로 오해하기 쉽습니다.
- Recipe fragment는 부모 Shell DataContext 없이는 실행되지 않습니다.
- Partial 59개는 대체로 XAML/generated/framework/cohesive composition이며, 수동 파일 분할만으로 구조가 좋아진다고 볼 근거는 없습니다.

이 비용은 먼저 `docs/admin/CODEBASE_STRUCTURE.md`의 Start Here와 가족별 읽기 순서로 줄이는 것이 안전합니다. 완료된 owner를 다시 쪼개는 것은 새 결함·요구사항·실패한 완료 조건·변경된 의존성 경계가 생길 때만 허용합니다.

## 유지해야 하는 기존 구조

- `OpenVisionShellHostView`는 화면 조합·lifecycle root로 유지합니다.
- `OpenVisionPipelineReviewLayoutController`는 compact/details/step-flow 상태 owner로 유지합니다.
- `OpenVisionPipelineReviewImageResourceOwner`는 Bitmap/diagnostic Dispose owner로 유지합니다.
- `OpenVisionShellHostRecipePanelDragController`는 drag/capture cleanup owner로 유지합니다.
- `LogPanelViewModel` + `LogPanelFileAccess`, `ImageCompareViewModel` + directory/resource owner 경계는 재사용합니다.
- Tool의 PropertyGrid와 shared base/controller/presenter 계약, Learn Presenter/SimulationModel, Docking.Controls public contract를 새 MVVM 계층으로 감싸지 않습니다.

## 이번 감사에서 하지 않은 변경

- View 전체를 기계적으로 ViewModel로 변환하지 않았습니다.
- 수동 Partial을 일괄 삭제하거나 파일별로 다시 쪼개지 않았습니다.
- XAML binding 이름, 공개 Window/View 계약, Recipe/XML, SDK/PropertyGrid 계약을 변경하지 않았습니다.
- P1 후보 세 곳의 구현을 추측으로 이동하지 않았습니다.

## 검증과 미검증 범위

실행한 검증:

- `Invoke-RefactorAudit.ps1 -Verify`: **PASS**
- 프로젝트 순환: 0
- Shell 직접 Run History 저장 호출: 0
- 감사 산출물: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\mvvm-view-audit-20260913\source-survey.json` 및 CSV 목록

이번 턴에는 소스와 정적 구조만 확인했습니다. WPF 화면을 실제로 열어 확인하는 작업, 테마별 렌더링, 100/125/150/175/200% DPI, 다중 모니터, 키보드/마우스, 카메라·SDK·GPU, 장시간 실행은 수행하지 않았습니다.

**소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요**

## 종료 기록

```text
Status: Complete
Scope: 60 XAML 전체와 연결된 56개 View code-behind, ViewModel·Partial·프로젝트 참조의 MVVM 경계와 단일 View 이동 가능성 감사
Acceptance criteria:
  - 전체 XAML/Views 분류: 충족
  - MVVM·Controller·Presenter·custom-control 경계 기록: 충족
  - 단일 View 이동 가능성 및 필요한 기능 묶음 기록: 충족
  - Visual Studio 첫 코드 읽기 순서와 호출 경로 기록: 충족
  - 정적 구조 감사 통과: 충족
Verification: Invoke-RefactorAudit.ps1 -Verify (PASS)
Evidence: 이 문서와 D:\OpenVisionLab-TestData\OpenVisionLab_Dev\mvvm-view-audit-20260913
Boundary / next dependency: 런타임 UI·하드웨어·장시간 실행은 미검증; P1 후보 구현은 별도 작업으로 최소 경계 검증 필요
```

## 다음 우선순위

1. Recipe run evidence의 파일/Bitmap decode를 기존 concrete owner로 이동하고 View를 표시 adapter로 축소 | Recommended model: GPT-5.3-Codex | Reasoning effort: medium
2. Line Tool 저장 호출의 기존 session/controller owner를 확인하고 View 직접 persistence 제거 | Recommended model: GPT-5.3-Codex | Reasoning effort: medium
3. `RoiImageCanvasViewModel` consumer·Mat/Bitmap lifetime 재현 검증 후 경계 판단 | Recommended model: gpt-5.6-terra | Reasoning effort: high
4. 대표 화면의 theme/DPI/monitor/input 런타임 matrix 실행 | Recommended model: GPT-5.3-Codex | Reasoning effort: medium
