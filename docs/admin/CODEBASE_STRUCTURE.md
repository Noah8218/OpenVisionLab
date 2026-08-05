# OpenVisionLab Codebase Structure

이 문서는 OpenVisionLab 코드베이스의 상위 구조를 빠르게 파악하기 위한 입구 문서입니다.
세부 구현 이력보다 "어느 책임이 어디에 있는지", "새 툴을 추가하거나 버그를 추적할 때 어디서 시작해야 하는지"에 초점을 둡니다.

## 1. 제품 관점의 큰 흐름

OpenVisionLab은 여러 이미지 레이어를 중심으로 동작하는 rule-based vision workbench입니다.
사용자는 `Main` 같은 입력 레이어에 이미지를 로드하고, 각 툴에서 입력/출력 레이어를 선택한 뒤, 파라미터를 조정하면서 결과를 미리 보고 비교합니다.

핵심 방향성은 다음과 같습니다.

- 레이어는 독립적으로 선택되고 비교되어야 합니다.
- 출력 레이어 생성이 입력 레이어를 강제로 바꾸면 안 됩니다.
- 툴 내부 input/output preview를 클릭하면 메인 작업영역도 해당 레이어를 보여야 합니다.
- Blob, Contour, Matching 등 알고리즘 툴은 PropertyGrid 기반 구조를 유지합니다.
- 새 툴이 늘어나도 반복 배선은 줄이고, 툴 고유 동작은 숨기지 않습니다.

```text
이미지 레이어
  -> 툴 선택
  -> 입력/출력 레이어 라우팅
  -> 파라미터 편집
  -> 프리뷰 실행 또는 자동 프리뷰
  -> 결과 레이어 갱신
  -> 메인 작업영역/툴 preview/레이어 목록 동기화
  -> 파이프라인 step 저장
```

## 2. 루트 디렉터리 지도

| 경로 | 역할 |
| --- | --- |
| `src/OpenVisionLab/Program.cs` | WPF 애플리케이션 진입점입니다. |
| `src/OpenVisionLab/OpenVisionLab.csproj` | 메인 WPF 앱 프로젝트입니다. `net8.0-windows7.0`, WPF 사용, x64 중심입니다. |
| `Directory.Build.props` | 저장소 공통 MSBuild 경로와 vendored DLL root를 정의합니다. |
| `src/OpenVisionLab/UI/` | 현재 WPF UI, Shell, tool view, popup, teaching panel 중심 코드입니다. |
| `src/OpenVisionLab/Core/` | application, recipe, pipeline, storage, state service가 위치하는 핵심 영역입니다. |
| `src/OpenVisionLab/Common/` | 공통 event args, binder, shared helper류가 위치합니다. |
| `src/OpenVisionLab/Vision/` | OpenCV 기반 vision tool property/model/wrapper 계층입니다. |
| `src/OpenVisionLab/Property/` | 저수준 property container와 PropertyGrid 연결 모델이 위치합니다. |
| `src/Libraries/` | 재사용 가능한 분리 라이브러리 프로젝트들입니다. MVVM, 이미지 캔버스, 레이어 core, logging, localization 등을 포함합니다. |
| `tools/` | UI smoke, contract check, recipe runner 등 검증/보조 실행 프로젝트입니다. |
| `docs/` | 설계 문서, 운영 문서, smoke 정책, extension guide가 위치합니다. |
| `dll/` | vendored runtime DLL 위치입니다. OpenVisionLab Vision SDK 3.0, 공용 OpenCvSharp native runtime, WPG PropertyGrid DLL 참조가 여기서 해결됩니다. |
| `Sample/` | 로컬/vendor 샘플 참고 영역입니다. 공개 배포나 GitHub 원본 반영 대상이 아니며, 공개 검증 자산은 `docs/samples/public/`와 `docs/samples/public/product/`를 사용합니다. |
| `scripts/` | 보조 스크립트 영역입니다. |

## 3. 메인 앱과 Shell 구조

메인 작업영역은 `src/OpenVisionLab/UI/Menu/Wpf` 아래에 집중되어 있습니다.

| 파일/영역 | 책임 |
| --- | --- |
| `OpenVisionShellHostView.xaml(.cs)` | 메인 WPF Shell view입니다. UI 이벤트의 마지막 얇은 연결층입니다. |
| `OpenVisionShellHost*Controller.cs` | ShellHost의 명령, 문서, 도킹 레이어, 툴 창, 상태 표시 등 책임을 분리한 controller/presenter 계층입니다. |
| `OpenVisionShellHost*Presenter.cs` | Shell UI 표시 상태를 갱신하는 presentation 계층입니다. |
| `OpenVisionNativeToolDocument.cs` | 선택된 native WPF tool 하나의 runtime 문서입니다. 레이어 라우팅, preview, pipeline, command controller를 묶습니다. |
| `OpenVisionNativeToolDocumentCache.cs` | 툴 document 재사용/cache를 담당합니다. 툴 창 표시 속도 개선과 관련됩니다. |
| `OpenVisionNativeToolRegistry.cs` | native WPF tool 등록의 중심입니다. 새 툴 연결은 여기서 시작합니다. |
| `OpenVisionNativeToolPrewarm*.cs` | 툴 표시 속도 개선을 위한 선생성/prewarm 정책과 서비스입니다. |
| `OpenVisionLayerViewerView.xaml(.cs)` | 레이어 단독 보기 창입니다. WPF fallback viewer와 확대/이동을 포함합니다. |
| `OpenVisionZoomableImageController.cs` | WPF 이미지 viewer의 공통 확대/이동/좌표 상태 controller입니다. |
| `Documents/` | Pipeline review 같은 Shell 문서형 화면입니다. |
| `ViewModels/`, `Views/` | Shell 주변 MVVM view model과 view입니다. |

Shell 쪽 변경 시 우선 확인할 것:

- `OpenVisionShellHostView.xaml.cs`에 새 책임을 직접 추가하지 말고, 이미 있는 controller/presenter로 이동할 수 있는지 확인합니다.
- document lifetime은 `OpenVisionNativeToolDocument`, cache, floating window host와 얽혀 있으므로 `Unloaded`에서 무조건 dispose하지 않습니다.
- 메인 작업영역이 어떤 레이어를 보여야 하는지는 `IDisplayManager` 활성 레이어와 workspace presenter 상태가 같이 맞아야 합니다.

## 4. 레이어와 이미지 표시 구조

OpenVisionLab의 중심 모델은 레이어입니다.
툴은 입력 레이어에서 이미지를 읽고, 출력 레이어에 결과 이미지를 씁니다.

주요 책임 분리:

| 구성요소 | 책임 |
| --- | --- |
| `IDisplayManager` | 레이어 목록, 선택 레이어, 레이어 이미지 조회/갱신의 중심 인터페이스입니다. |
| `OpenVisionNativeLayerRouteController` | input/output 레이어 선택 규칙을 관리합니다. 출력 레이어가 자기 자신의 입력 후보가 되지 않도록 관리합니다. |
| `OpenVisionNativeToolLayerViewController` | 툴 preview와 메인 display manager 사이의 레이어 표시를 갱신합니다. |
| `OpenVisionNativePreviewLayerPublisher` | preview 결과 bitmap을 출력 레이어에 publish하고, 필요한 경우 output layer를 생성합니다. |
| `OpenVisionBitmapCanvasPresenter` | WPF bitmap workspace 표시와 zoom state 보존을 담당합니다. |
| `OpenVisionBitmapImagePreviewFactory` | bitmap을 WPF 표시용 image source로 변환하는 공통 경로입니다. |

레이어 관련 불변 조건:

- output layer 생성은 input layer selection을 바꾸면 안 됩니다.
- 툴 output preview 클릭은 메인 작업영역을 output layer로 전환해야 합니다.
- input preview 클릭은 메인 작업영역을 input layer로 전환해야 합니다.
- 자동 preview는 결과 이미지를 output layer에 갱신하되, operator가 선택한 input route를 깨면 안 됩니다.

## 5. Native WPF Tool 구조

툴 UI는 `src/OpenVisionLab/UI/VisionTest` 아래에 집중되어 있습니다.

| 경로 | 역할 |
| --- | --- |
| `Contracts/` | tool view model contract, preview canvas contract 등 인터페이스 정의입니다. |
| `Composition/` | tool view model 생성과 조합 서비스입니다. |
| `Services/` | tool 주변 서비스입니다. |
| `ViewModels/` | tool parameter/state ViewModel입니다. 이름에는 가능하면 `ViewModel`을 명시합니다. |
| `Wpf/` | 실제 WPF tool view, shell, runtime, binder, behavior가 위치합니다. |
| `Wpf/Behaviors/` | view event와 command를 연결하는 재사용 behavior/controller입니다. |

### 5.1 공통 tool shell

| 구성요소 | 책임 |
| --- | --- |
| `VisionToolSingleInputPropertyToolShell.xaml(.cs)` | input 1개, output 1개, PropertyGrid 기반 툴의 공통 shell입니다. |
| `VisionToolDoubleInputCustomToolShell.xaml(.cs)` | input A/B와 output이 필요한 툴, 예: Arithmetic 계열 shell입니다. |
| `VisionToolInlinePreviewSlot.cs` | 툴 내부 input/output preview viewer입니다. 이미지 표시, zoom, pan, line ROI overlay 표시를 담당합니다. |
| `VisionToolChromePresenter.cs` | tool chrome/header/status 등 공통 표시 책임입니다. |
| `VisionToolPropertyGridHost.cs` | PropertyGrid host 생성과 selected object 연결을 담당합니다. |

### 5.2 Runtime / Binder / ViewModel

| 구성요소 | 책임 |
| --- | --- |
| `VisionToolSingleInputViewModel.cs` | input/output layer selection, command 요청 상태를 보관합니다. |
| `VisionToolDoubleInputViewModel.cs` | A/B input과 output을 쓰는 툴의 selection/command 상태입니다. |
| `VisionToolSingleInputViewRuntime.cs` | single input view의 runtime wiring입니다. |
| `VisionToolDoubleInputViewRuntime.cs` | double input view의 runtime wiring입니다. |
| `VisionToolSingleInputViewBinder.cs` | ComboBox, preview, button 등 view element와 ViewModel/command를 연결합니다. |
| `VisionToolDoubleInputViewBinder.cs` | double input tool의 binder입니다. |
| `VisionToolLayerSelectionBehavior.cs` | layer ComboBox 동작을 공통화합니다. |
| `VisionToolActionBehavior.cs` | preview click, run, add pipeline, create output 같은 action 연결을 공통화합니다. |
| `VisionToolPropertyChangeController.cs` | PropertyGrid 변경과 preview scheduling 흐름을 연결합니다. |
| `VisionToolDebouncedPreviewScheduler.cs` | slider/property 변경 시 preview를 과도하게 실행하지 않도록 debounce합니다. |

View 코드비하인드의 목표:

- View는 XAML element expose와 최소한의 event bridge만 갖습니다.
- 상태와 명령은 ViewModel/Runtime/Binder/Behavior로 이동합니다.
- View 안에 ViewModel을 강하게 박아 넣지 않습니다. View만 이동해도 가능한 한 깨지지 않아야 합니다.

## 6. Tool Document 생성 흐름

Native WPF tool은 registry와 factory lane을 통해 생성됩니다.

```text
OpenVisionNativeToolRegistry
  -> tool lane factory 선택
  -> WPF view 생성
  -> property model / view model / preview delegate 구성
  -> OpenVisionNativeToolDocument 생성
  -> Floating tool window에 표시
```

주요 factory lane:

| Lane | 사용 대상 | 주요 파일 |
| --- | --- | --- |
| PropertyGrid tool | Blob, Contour, Matching 등 모델 property로 UI가 만들어져야 하는 알고리즘 툴 | `OpenVisionNativePropertyGridToolFactory`, `OpenVisionNativePropertyGridToolDocumentBuilder` |
| Custom UI tool | Line처럼 고유 UI와 여러 mode가 필요한 툴 | `OpenVisionNativeCustomToolFactory` |
| SimplePreprocess tool | Threshold, Filter, Morphology처럼 공통 preprocess shell로 표현 가능한 툴 | `OpenVisionNativeSimplePreprocessDocumentFactory`, `OpenVisionNativeSimplePreprocessViewConfigurator`, `OpenVisionNativeSimplePreprocessPreviewExecutor` |
| Arithmetic tool | A/B input 또는 offset/operation mode가 필요한 툴 | `OpenVisionNativeArithmeticDocumentFactory` |

새 툴을 추가할 때는 먼저 `docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`를 확인합니다.
반복 배선을 줄이되, 모든 툴을 하나의 거대한 generic factory로 합치지는 않습니다.

## 7. PropertyGrid 구조

PropertyGrid 기반 알고리즘 툴은 반드시 유지해야 하는 핵심 구조입니다.

기본 원칙:

- 모델에 public property와 attribute를 정의하면 PropertyGrid UI가 자동으로 생성되어야 합니다.
- Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching 같은 알고리즘 툴은 PropertyGrid 기반을 유지합니다.
- PropertyGrid에서 선택된 object는 tool runtime이 관리하며, view는 host 역할을 합니다.

관련 프로젝트/파일:

| 위치 | 역할 |
| --- | --- |
| `src/Libraries/PropertyGrid.Abstractions` | PropertyGrid 관련 추상 contract입니다. |
| `src/Libraries/WpfPropertyGridBridge` | 기존 WPG/WPF PropertyGrid 연결 계층입니다. 현재 WPF 직접 사용 방향으로 점진 정리 대상입니다. |
| `src/OpenVisionLab/Common/PropertyGridEventBinder.cs` | PropertyGrid event 연결 helper입니다. |
| `VisionToolPropertyGridHost.cs` | tool shell 안에서 PropertyGrid control을 생성/유지하는 host입니다. |
| `OpenVisionNativePropertyGridToolFactory.cs` | PropertyGrid 기반 tool 생성 lane입니다. |

주의:

- WinForms bridge 시절의 흔적과 WPF 직접 사용 구조가 섞일 수 있습니다.
- PropertyGrid 문제가 생기면 editor template, selected object, descriptor 중 어느 층이 깨졌는지 분리해서 확인합니다.

## 8. Vision / OpenCV 계층

`src/OpenVisionLab/Vision/OpenCV`는 OpenCV 기반 툴 property, wrapper, execution 관련 코드가 위치하는 영역입니다.

관련 외부 runtime:

- `dll/OpenVisionLab-Vision-SDK/OpenVisionLab.Core.dll`
- `dll/OpenVisionLab-Vision-SDK/OpenVisionLab.Vision2D.dll`
- `dll/OpenVisionLab-Vision-SDK/OpenVisionLab.Vision2D.Blob.dll`
- `dll/OpenVisionLab-Vision-SDK/OpenCvSharp*.dll`
- `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`

메인 앱은 vendored DLL이 없으면 build target에서 실패하도록 구성되어 있습니다.
OpenCV 실행 경로를 수정할 때는 UI preview뿐 아니라 pipeline step 실행과 recipe runner 호환성도 같이 확인해야 합니다.

## 9. Pipeline / Recipe / Result 구조

Pipeline은 툴 실행을 반복 가능한 step으로 저장하고 재실행하기 위한 계층입니다.

| 영역 | 역할 |
| --- | --- |
| `OpenVisionLab.Vision2D.Pipeline` 참조 | `VisionPipelineStep` 등 SDK 3.0 pipeline model을 제공합니다. |
| `OpenVisionNativePipelineCommandController.cs` | 현재 tool state를 pipeline step으로 추가합니다. |
| `Documents/OpenVisionPipelineReviewDocument.cs` | pipeline review UI document입니다. |
| `src/Libraries/OpenVisionLab.Pipeline.Controls` | pipeline UI control library입니다. |
| `tools/VisionRecipeRunnerSmoke` | recipe/pipeline 실행 smoke입니다. |
| `docs/VISION_PIPELINE_*` | XML recipe schema, LLM recipe contract, runtime plan 문서입니다. |

Pipeline 관련 변경 시 확인할 것:

- UI preview와 pipeline execution이 같은 parameter semantics를 쓰는지 확인합니다.
- output layer 이름, input layer 이름, parameter key가 recipe contract와 맞는지 확인합니다.
- 결과 metric/overlay contract는 `docs/VISION_TOOL_RESULT_CONTRACT.md`를 기준으로 확인합니다.

## 10. Libraries 프로젝트

| 프로젝트 | 역할 |
| --- | --- |
| `OpenVisionLab.Mvvm` | `ObservableObject`, `RelayCommand` 등 MVVM 기반입니다. |
| `OpenVisionLab.ImageSpace.Core` | 레이어/image space core model입니다. |
| `OpenVisionLab.Display.Core` | display 관련 core abstraction입니다. |
| `OpenVisionLab.ImageCanvas` | 이미지 캔버스/viewer 관련 library입니다. |
| `OpenVisionLab.Logging` | logging core입니다. |
| `OpenVisionLab.Logging.Controls` | logging UI controls입니다. |
| `OpenVisionLab.Pipeline.Controls` | pipeline UI controls입니다. |
| `OpenVisionLab.Localization` | localization catalog/service입니다. |
| `OpenVisionLab.History` | history/undo-redo 관련 library입니다. |
| `PropertyGrid.Abstractions` | PropertyGrid abstraction입니다. |
| `WpfPropertyGridBridge` | WPF PropertyGrid bridge입니다. |

메인 앱과 내부 라이브러리는 각각 `src/OpenVisionLab/`과 `src/Libraries/`의 독립 프로젝트 루트에 있습니다. 메인 앱은 `ProjectReference`로 필요한 라이브러리를 참조하며, 라이브러리 소스는 메인 프로젝트의 기본 compile glob 범위에 포함되지 않습니다.

## 11. 검증 도구와 smoke 전략

검증 도구는 `tools/` 아래에 있습니다.

| 도구 | 역할 |
| --- | --- |
| `PipelineViewerScreenshotSmoke` | WPF UI를 실제로 띄워 screenshot과 runtime 상태를 검사하는 핵심 smoke입니다. |
| `VisionUiContractCheck` | UI contract/static contract 점검입니다. |
| `OpenVisionReadinessCheck` | readiness/precheck 성격의 검증입니다. |
| `LocalizationCatalogCheck` | localization key/catalog 검증입니다. |
| `HistoryContractCheck` | history contract 검증입니다. |
| `RecipeXmlCompatibilityCheck` | recipe XML compatibility 검증입니다. |
| `VisionRecipeRunnerSmoke` | recipe runner smoke입니다. |
| `OpenVisionLab.ImageCompare` | standalone image compare utility입니다. |

변경 포인트 검증 원칙:

- 작은 UI 수정은 관련 smoke target만 실행합니다.
- shared Shell, layer route, tool runtime을 건드리면 layer selection/tool preview smoke를 추가로 실행합니다.
- 성능 변경은 `wpf_tool_open_perf` 또는 관련 perf smoke로 baseline을 확인합니다.
- "보인다"만 확인하지 말고, runtime state도 같이 확인합니다. 예: workspace title, active layer, preview result, output bitmap difference.

자주 쓰는 명령:

```powershell
dotnet build .\src\OpenVisionLab\OpenVisionLab.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build .\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet .\tools\PipelineViewerScreenshotSmoke\bin\x64\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll --list
```

## 12. 대표 런타임 시퀀스

### 12.1 툴 열기

```text
사용자 tool rail 클릭
  -> OpenVisionShellHostToolWindowController
  -> OpenVisionNativeToolDocumentCache
  -> OpenVisionNativeToolRegistry.TryCreateDocument
  -> lane factory
  -> OpenVisionNativeToolDocument
  -> OpenVisionFloatingToolWindowHost
```

### 12.2 파라미터 변경 후 preview

```text
PropertyGrid 또는 custom parameter 변경
  -> VisionToolPropertyChangeController
  -> VisionToolDebouncedPreviewScheduler
  -> OpenVisionNativeToolDocument.RunPreview
  -> OpenVisionNativePreviewExecutionController
  -> OpenCV/tool execution
  -> OpenVisionNativePreviewLayerPublisher.PublishPreviewBitmap
  -> tool preview refresh + layer list refresh
```

### 12.3 output preview 클릭

```text
VisionToolInlinePreviewSlot 좌클릭
  -> VisionToolActionBehavior.OutputPreview_MouseUp
  -> OpenVisionNativeToolDocument.OnOutputPreviewClicked
  -> OpenVisionNativeToolRouteInteractionController.HandleSingleOutputPreviewClicked
  -> OpenVisionNativeToolLayerViewController.ActivateLayerIfPresent
  -> DisplayManager active layer 변경
  -> Shell workspace가 output layer 표시
```

### 12.4 output layer 생성

```text
툴의 output 추가 버튼 클릭
  -> VisionToolActionBehavior
  -> OpenVisionNativeToolRouteInteractionController.Handle*CreateOutputLayerRequested
  -> OpenVisionNativeLayerRouteController.SelectNext*OutputLayerName
  -> OpenVisionNativeToolLayerViewController.EnsureOutputLayerFromInput
  -> output layer 준비
  -> input layer route 복원
```

## 13. 새 작업을 시작할 때 보는 순서

| 작업 유형 | 먼저 볼 파일 |
| --- | --- |
| 툴 추가 | `docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`, `OpenVisionNativeToolRegistry.cs` |
| PropertyGrid 버그 | `VisionToolPropertyGridHost.cs`, `WpfPropertyGridBridge`, property model |
| input/output 레이어 버그 | `OpenVisionNativeLayerRouteController.cs`, `OpenVisionNativeToolLayerViewController.cs`, `VisionToolLayerSelectionBehavior.cs` |
| output preview 클릭/메인 표시 버그 | `VisionToolInlinePreviewSlot.cs`, `VisionToolActionBehavior.cs`, `OpenVisionNativeToolRouteInteractionController.cs` |
| 툴 창 표시 속도 | `OpenVisionNativeToolDocumentCache.cs`, `OpenVisionNativeToolPrewarmService.cs`, `OpenVisionNativeToolPrewarmPolicy.cs` |
| 메인 workspace 이미지 표시 | `OpenVisionShellHostView.xaml.cs`, `OpenVisionBitmapCanvasPresenter.cs`, `OpenVisionZoomableImageController.cs` |
| pipeline 저장/실행 | `OpenVisionNativePipelineCommandController.cs`, `OpenVisionPipelineReviewDocument.cs`, `docs/VISION_PIPELINE_*` |
| UI smoke 추가 | `tools/PipelineViewerScreenshotSmoke/Program.cs`, `docs/UI_SCREENSHOT_SMOKE.md` |

## 14. 구조화 원칙

1. View code-behind는 얇게 유지합니다.
2. 공통 동작은 Runtime, Binder, Behavior, Controller, Presenter로 이동합니다.
3. PropertyGrid 기반 툴은 model-driven UI 원칙을 유지합니다.
4. 레이어 라우팅은 개별 view에서 임의로 바꾸지 말고 route controller를 통해 바꿉니다.
5. output 생성과 output 선택은 다른 동작입니다. 자동으로 input을 바꾸지 않습니다.
6. 툴별 특수성은 숨기지 않습니다. 반복 배선을 줄이되 모든 툴을 무리하게 하나의 추상화로 합치지 않습니다.
7. 실제 EXE/스모크로 변경 포인트를 검증합니다.
8. 핵심 책임을 바꾸는 경우 짧은 주석을 남깁니다. 단순 설명 주석은 피합니다.

## 15. 관련 문서

- `docs/VISION_TOOL_NATIVE_WPF_EXTENSION_GUIDE.md`
- `docs/VISION_TOOL_PROPERTY_GRID_POLICY.md`
- `docs/VISION_TOOL_CONTRACT.md`
- `docs/VISION_TOOL_RESULT_CONTRACT.md`
- `docs/UI_SCREENSHOT_SMOKE.md`
- `docs/OPENVISIONLAB_PLATFORM_DIRECTION.md`
- `docs/OPENVISIONLAB_WPF_MIGRATION_PLAN.md`
- `docs/VISION_PIPELINE_RECIPE_SPEC.md`
- `docs/VISION_PIPELINE_LLM_RECIPE_CONTRACT.md`
