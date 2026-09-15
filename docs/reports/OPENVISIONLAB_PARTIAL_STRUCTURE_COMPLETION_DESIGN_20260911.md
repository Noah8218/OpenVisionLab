# OpenVisionLab 2D Partial 구조화 완료 설계

Updated: 2026-09-11 KST
Status: **PL-0013 COMPLETE — D5 protected-boundary closure verified**

## 1. 설계 목적

사용자가 요청한 “기존 partial을 전수조사하고 구조화”의 완료 기준을 먼저
고정한다. 이 문서는 파일을 옮기거나 이름을 바꾸는 작업 목록이 아니라, 각
partial의 현재 소유자, 실제 호출 경로, mutable state 기록자, 수명·해제
소유자, 공개/XAML/test 계약, 그리고 partial 제거 후의 구체적인 소유자 경계를
기록한 구현 설계다.

이 설계에서는 WPF/XAML generated contract를 억지로 제거하지 않는다. 목표는
partial 개수를 0으로 만드는 것이 아니라, 수동으로 클래스를 여러 파일에
나누어 책임을 숨긴 partial을 없애고, 프레임워크가 요구하는 partial은 이유와
검증을 갖춘 보호 목록으로 닫는 것이다.

## 2. 작업 기준과 현재 baseline

| 항목 | 값 |
| --- | --- |
| Repository | `C:\Git\2D\Dev` |
| Branch | `codex/public-sample-ux-docs` |
| Commit SHA | `0a77e60e444b12757565ee977216a994446b53a6` |
| Target Framework | `net8.0-windows7.0` |
| Product version | `2.2.0-dev.2` |
| 실제 compiled partial 선언 | 78 |
| 감사 도구의 raw partial text match | 2 (smoke contract 문자열, 선언 아님) |
| 현재 작업 트리 | 기존 Dev 변경이 많으므로 reset/clean 금지 |

현재 78개 선언의 분류는 다음과 같다.

| 분류 | 개수 | 설계 결정 |
| --- | ---: | --- |
| XAML code-behind contract | 56 | `InitializeComponent`, named element, WPF lifetime 계약을 보존 |
| `ImageCanvasControl` native/designer composition | 2 | WinForms/SharpGL designer 계약과 native dispose 순서를 보존 |
| `Settings` generated contract | 1 | generated file로 유지 |
| `OpenGlDrawing` rendering composition | 9 | public façade와 구체 renderer로 단계적 전환 |
| `OpenVisionShellHostRecipeCommandSurface` | 9 | binding façade와 독립 workflow owner로 단계적 전환 |
| `OpenVisionShellHostView.TestHooks` | 1 | 명시적인 test surface owner로 전환 |
| 합계 | 78 | 수동 partial 최종 목표는 0, 보호 partial 최종 목표는 59 |

`tools/PipelineViewerScreenshotSmoke/RecipeValidationSuiteViewContract.cs`와
`tools/VisionRecipeRunnerSmoke/ShellRecipeBasicLifecycleViewContract.cs`의
문자열 `public partial class ...`는 source declaration이 아니다. 다음 audit
slice에서 `PartialDeclarations`와 `PartialTextMatches`를 분리해 같은 오판을
방지한다.

## 3. 완료 정의

다음 조건을 모두 만족해야 PL-0013을 다시 `VERIFIED`로 닫는다.

1. 수동 책임 분할인 `OpenGlDrawing`, Recipe Command Surface,
   `OpenVisionShellHostView.TestHooks`에 `partial` 선언이 남지 않는다.
2. XAML 56개, `ImageCanvasControl` designer 조합 2개, `Settings.Designer` 1개는
   보호 목록에 파일·생성 이유·호출 경로·수명 소유자·검증 계약이 기록된다.
3. 각 이동 책임은 현재 owner와 새 owner, 호출자, mutable-state writer,
   lifetime/release owner, XAML/public/test contract를 evidence에 남긴다.
4. 기존 type name, namespace, XAML binding, command name, public test contract,
   Recipe XML, OpenGL public method signature, event 순서, Dispose 순서를
   유지한다. 계약 변경은 별도 승인 없이는 하지 않는다.
5. 이전 partial 파일명·namespace·stale call path가 `src`와 `tools`에 남지 않는다.
6. 각 단계마다 관련 Debug/Release build, focused contract/smoke, static call
   path check, `Invoke-RefactorAudit.ps1 -Verify`, `git diff --check`를 실행한다.
7. 실제 WPF theme/layout/DPI/monitor, camera/SDK, long-running native 검증은
   실행하지 않은 경우 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로
   분리해 기록한다. 사용자 입력이나 hardware를 무기한 기다리지 않는다.

최종 static audit의 기대치는 다음과 같다.

```text
PartialDeclarations = 59
    XAML contract                       56
    ImageCanvasControl designer pair    2
    Settings generated                   1
PartialTextMatches = 2 (intentional smoke strings, declaration count와 별도 보고)
Manual partial families                0
ProjectCycles                           0
ShellStorageCalls                       0
```

## 4. 소유자와 호출 경로 설계

### 4.1 XAML·generated 보호 경계

현재 owner는 각 `*.xaml`/`*.xaml.cs` 쌍의 WPF visual tree owner다. 호출 경로는
`XAML loader -> InitializeComponent -> View/Window -> existing ViewModel 또는
controller`이며 mutable visual state와 release는 해당 View/Window가 소유한다.
이 영역은 partial을 없애지 않고 다음 정적 검사를 완료한다.

- code-behind에는 View에 종속된 lifecycle/control event만 남긴다.
- Recipe, Pipeline, 검사 정책, 파일 저장, 장비 통신, mutable workflow state는
  기존 ViewModel/owner에 있는지 검색한다.
- XAML namescope, binding, automation id, `InitializeComponent`, event
  subscribe/unsubscribe 순서를 보존한다.
- 56개 전부를 한 번에 formatting하거나 mechanical merge하지 않는다.

`ImageCanvasControl.cs`와 `ImageCanvasControl.designer.cs`는 SharpGL/WinForms
designer가 생성한 control field와 `Dispose` 순서를 함께 소유하므로 한 concrete
control boundary로 닫는다. `Settings.Designer.cs`는 generated settings
contract로 닫는다.

### 4.2 `OpenVisionShellHostView.TestHooks` 최종 경계

현재 owner는 `OpenVisionShellHostView`이며 `TestHooks`는 다음 private owner를
직접 읽거나 호출한다.

- `ShellToolTestFacade`, `ShellLayerTestFacade`, `ShellDockingTestFacade`
- `OpenVisionShellHostStatePresenter`, `documentController`,
  `dockedLayerWorkspaceComposition`
- `layerManagementController`, `displayManager`, `runtimeContext`
- Shell의 docking/layer 활성화 callback과 TCP integration controller

현재 호출 경로는 `DirectSmokeRunner/PipelineViewerScreenshotSmoke ->
OpenVisionShellHostView`이고, public `*ForTest` 이름이 정적·화면 계약으로
사용된다.

최종 owner는 `OpenVisionShellHostViewTestSurface` concrete class다.

- 생성자는 위 기존 facade와 callback을 명시적으로 받는다.
- test surface는 상태를 새로 소유하지 않는다. mutable state writer는 기존
  `Shell*TestFacade`와 Shell owner로 유지한다.
- test surface에는 event subscription, timer, Bitmap/Mat 장기 보관, Window
  생성 책임을 넣지 않는다.
- View가 생성 시 test surface를 만들고 View 종료 시 callback을 끊는다.
- 기존 public/internal `*ForTest` 계약은 먼저 compatibility projection으로
  보존한다. in-repo smoke가 `TestSurface`를 직접 사용할 수 있다는 증거가
  확인된 뒤에만 projection 제거를 별도 작업으로 검토한다.

이렇게 하면 `TestHooks.cs`는 사라지지만 테스트 계약은 사라지지 않는다.
forwarding wrapper를 여러 개 만들지 않고, 하나의 실제 test boundary와 기존
계약 projection만 둔다.

완료 증거에는 다음을 포함한다.

- 228개 안팎의 public/internal test member별 caller 파일 목록
- property는 어느 기존 facade가 쓰는지, method는 어느 callback이 실행하는지
- View dispose 이후 test surface callback이 실행되지 않는지
- Direct/Viewer smoke의 기존 `*ForTest` assertions가 같은 결과를 내는지

### 4.3 `OpenGlDrawing` 최종 경계

현재 `OpenGlDrawing`은 9개 파일의 `public static partial class`이며 호출자는
주로 다음과 같다.

```text
ImageCanvasControl / RoiImageCanvasViewModel
 -> OpenGlDrawing public static method
 -> SharpGL OpenGL context
```

현재 static `ZoomFactor`와 handle-size state, intra-class method call,
`CanvasShape.DisplayListId`, `Bitmap`/texture와 OpenGL context의 수명을
공유한다. 따라서 partial 파일을 1개로 합쳐 2,500줄짜리 static 파일로 만드는
것도, method마다 forwarding class를 만드는 것도 하지 않는다.

최종 구조는 public compatibility façade 하나와 책임별 concrete renderer다.

```text
OpenGlDrawing (public static compatibility façade; exact signatures preserved)
 ├─ OpenGlColorConverter          (pure color conversion)
 ├─ OpenGlTextureRenderer         (texture/ODB texture)
 ├─ OpenGlShapeRenderer           (line/rectangle/circle/handles)
 ├─ OpenGlPenRenderer             (pen/point drawing)
 ├─ OpenGlTextRenderer            (font bitmap/text)
 ├─ OpenGlMeasurementRenderer     (measurement rendering)
 └─ OpenGlOverlayRenderer         (compile/overlay/group labels)
```

`OpenGlDrawing`은 외부 caller가 이미 사용하는 공개 계약 때문에 남기는 한 개의
compatibility façade다. 내부 renderer 사이에는 interface/factory/manager를
추가하지 않고 concrete 호출을 사용한다. `OpenGlRenderState`가 필요하다는
증거가 확인되면 `ZoomFactor`와 handle metrics만 담은 작은 concrete state를
도입하고, 그렇지 않으면 기존 public `ZoomFactor` field를 compatibility
source로 유지한다.

구현 순서는 색상 변환 → texture → shape/pen → text/measurement → overlay다.
각 단계의 renderer는 `OpenGL gl`을 호출자가 전달하고, OpenGL context와
`CanvasShape.DisplayListId`의 release 책임은 현재 `ImageCanvasControl`/
overlay owner에 남긴다. Mat/Bitmap 복사나 Dispose 정책은 변경하지 않는다.

완료 증거에는 public method signature 목록, `ImageCanvasControl`과
`RoiImageCanvasViewModel` caller 목록, external consumer smoke build, overlay
display-list allocation/release 정적 검사를 포함한다.

### 4.4 Recipe Command Surface 최종 경계

현재 owner는 `OpenVisionShellHostRecipeCommandSurface`이며 9개 partial이
다음 상태를 공유한다.

- WPF binding properties, commands, command invalidation
- Recipe/Pipeline/Sample selection과 filter 상태
- step edit session 및 execution session
- LLM draft/evidence/review 상태
- Validation Set와 Qualified Snapshot 선택·저장 상태
- Shell callback, file dialog, layer navigation, Preview/Run 연결

현재 호출 경로는 다음과 같다.

```text
OpenVisionShellHostView constructor
 -> OpenVisionShellHostRecipeCommandSurface
 -> XAML Recipe views / existing concrete Recipe owners
 -> Recipe XML, Pipeline execution, Layer navigation, Result/Review projection
```

최종 owner는 partial이 아닌 concrete `OpenVisionShellHostRecipeCommandSurface`
facade다. facade는 XAML에 노출되는 binding property/command와
`INotifyPropertyChanged` notification을 소유한다. workflow owner는 snapshot,
request, result를 입력·출력으로 받고 facade private field를 직접 읽지 않는다.

기존 concrete owner를 우선 재사용해 다음 순서로 이동한다.

| 순서 | 기존 파일/영역 | 최종 소유자 | 독립 경계 |
| --- | --- | --- | --- |
| R1 | `RecipeWorkspace.cs` | `OpenVisionRecipeWorkspaceUseCase` | Recipe 생성·복제·이름 변경·삭제·저장 결과 |
| R2 | `PipelineLifecycle.cs` | `OpenVisionRecipePipelineLifecycleUseCase` | Pipeline 활성화·복제·이름 변경·삭제 |
| R3 | `PipelineExchange.cs` | `OpenVisionRecipePipelineExchangeUseCase` + projection owner | XML import/export/review bundle 결과 |
| R4 | `ValidationSets.cs` | 기존 `ValidationSetDocumentOwner`, `ValidationEvidenceOwner`, `ValidationSetSelectionOwner` | 파일/선택/증거 snapshot과 저장 결과 |
| R5 | `QualifiedSnapshots.cs` | 기존 `OpenVisionRecipeQualifiedSnapshotController` | qualification action/result와 callback |
| R6 | `LlmXmlDraftWorkflow.cs` | 기존 `LlmDraftReviewOwner`, `ReviewBundleDryRunProjectionOwner`, `PinArrayGapValidationIdentityOwner`; residual이 독립 state를 가질 때만 한 concrete workflow owner | evidence packet/review decision/compiled draft lifecycle |
| R7 | `Handlers.cs` residual | 기존 `StepEditLoader`, `StepEditApplyOwner`, `StepPreviewNavigationOwner`, `RunHistoryOrchestrationOwner` | selection/navigation/edit/run-history result |
| R8 | `Commands.cs` 및 facade fields | final concrete facade | binding command creation과 notification만 유지 |

R6/R7에서 새 owner를 만들려면 다음 네 가지가 모두 증명되어야 한다.

1. owner가 자체 state와 dependency를 보유한다.
2. facade는 immutable request/snapshot/result만 전달한다.
3. owner를 Shell/Window를 만들지 않고 focused test로 실행할 수 있다.
4. 기존 owner로 이동할 수 없다는 dependency/lifetime 증거가 있다.

조건을 충족하지 않는 method는 facade의 named responsibility region에 남기고,
partial을 제거하기 위해 wrapper를 만들지 않는다. Binding property를 새
owner로 옮길 때는 property name, notification 순서, `CanExecute` 재계산,
XAML DataContext를 먼저 캡처하고 같은 contract test를 반복한다.

Recipe 완료 시 `CommandSurface` 폴더에는 partial 파일이 없고, facade 한 개와
실제 독립 owner만 남는다. `Handlers.cs`를 단순히 여러 partial로 다시 나누는
것은 완료로 인정하지 않는다.

## 5. 단계별 실행 스케줄

각 heartbeat 실행은 하나의 단계만 처리하고, 해당 단계의 검증이 끝난 뒤 같은
automation을 다음 미완료 단계로 갱신한다. 사용자 입력·카메라·하드웨어·실제
GUI 조작이 필요한 검증은 건너뛰고 unverified로 기록한다.

| 단계 | 목적 | 산출물 | 완료 조건 |
| --- | --- | --- | --- |
| D0 | 계약 freeze 및 78/2 inventory 교정 | declaration/text-match 분리 CSV, owner map | 모든 대상에 owner/call path/state/lifetime/contract 기록 |
| D1 | smoke contract false-positive 정리 | audit 출력 분리, smoke build | `PartialTextMatches`가 선언 수에 섞이지 않음 |
| D2 | Shell TestHooks 제거 | `OpenVisionShellHostViewTestSurface` 및 compatibility proof | 기존 smoke 호출 결과 보존, TestHooks partial 0 |
| D3 | OpenGL rendering family 구조화 | façade + concrete renderers | public signature/caller/overlay lifetime 보존, OpenGL partial 0 |
| D4 | Recipe Command Surface 구조화 | concrete facade + workflow owners | binding/command/XML/Preview/Run contract 보존, Recipe partial 0 |
| D5 | 보호 목록·폴더·문서 최종 검수 | final inventory, owner map, junior reading route | 기대치 59/0/0, Debug/Release/audit/smoke 통과 |

각 단계의 공통 검증 명령은 다음과 같다.

```text
dotnet build OpenVisionLab.sln --configuration Debug --no-restore
dotnet build OpenVisionLab.sln --configuration Release --no-restore
dotnet build <changed smoke or contract project> --configuration Debug --no-restore
dotnet build <changed smoke or contract project> --configuration Release --no-restore
powershell -ExecutionPolicy Bypass -File tools/RefactorAudit/Invoke-RefactorAudit.ps1 -Verify
powershell -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1
git diff --check
```

단계별 evidence는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-
design-20260911` 아래에 기록하고, 구현 slice마다 별도 timestamp 하위 폴더를
만든다. C:의 source tree 안에 build/smoke 산출물을 만들지 않는다.

## 6. 금지·보존 항목

- XAML generated partial을 일반 class로 합치지 않는다.
- `OpenGlDrawing` public signature, `ZoomFactor`, OpenGL display-list 수명,
  Bitmap/Mat/texture ownership를 바꾸지 않는다.
- Recipe XML serialization, command/property name, Preview/Run 명시 동작,
  save/validation/exception 경로를 바꾸지 않는다.
- Test surface를 이유 없이 공개 제품 API로 확장하지 않는다.
- interface, abstract base, factory, provider, manager, event bus를 추가하지
  않는다. 외부 계약·테스트 seam·독립 lifetime이 증명된 concrete owner만 허용한다.
- 이번 설계 범위에서 CI 배포 검증, pipeline timeout 정책, Threshold 업무 로직
  이동을 동시에 열지 않는다. 그것들은 현재 partial 구조화의 후속 별도 issue다.
- `C:\Git\2D\Original`은 읽기 전용이며 commit/push/tag/release/deploy는 이
  설계 단계와 구현 heartbeat에서 수행하지 않는다.

## 7. 주니어 개발자 코드 읽기 순서

최종 구조에서도 다음 한 줄 경로가 유지되어야 한다.

```text
Program.Main
 -> OpenVisionLabApplication.Run
 -> OpenVisionShellHostWindow
 -> OpenVisionShellHostView
 -> RecipeCommandSurface / TestSurface / concrete renderer or workflow owner
 -> Pipeline/Tool/Layer/Result/Review owner
```

각 구현 slice의 completion evidence는 “누가 생성하는가, 누가 소유하는가,
누가 상태를 쓰는가, 누가 해제하는가, 실패가 어디로 전달되는가”를 파일과
메서드 이름으로 직접 답해야 한다. 이름만 바뀌고 call path 또는 state owner가
그대로인 변경은 구조화 완료로 기록하지 않는다.

## 8. 현재 설계 단계의 상태

- 설계: **READY**
- 소스 구현: **D5 VERIFIED / closure complete**
- 완료된 구현 slice: **D0 → D1 → D2 → D3 → D4 → D5**. D2는 Shell TestSurface concrete
  owner 추출과 기존 test contract compatibility forwarding을 포함하고, D3는
  OpenGL renderer concrete owner와 public façade를 포함하며, D4는 Recipe
  CommandSurface를 하나의 concrete facade와 기존 workflow owner 집합으로
  정리하고, D5는 보호 partial inventory와 최종 구조 감사를 닫았다.
- 현재 검증: D5 protected inventory, solution/Smoke Debug·Release build,
  ReadinessCheck Debug·Release, Recipe focused contracts, source-body contract,
  declaration/text audit, DocumentationIndex, stale-path search가 PASS했다.
  실제 GPU/GUI visual state는 아직 검증하지 않았다.
- 기존 P3/P4/P5 build/smoke evidence는 과거 source 상태에 대한 기록으로
  보존하며, 남은 manual family를 닫은 증거로 재사용하지 않는다.

## 9. D2 구현 결과 — Shell TestSurface

D2는 설계된 경계대로 완료했다. 기존
`OpenVisionShellHostView.TestHooks.cs`의 수동 partial을 삭제하고, 검사 계약과
계약에 필요한 입력 의존성을 `OpenVisionShellHostViewTestSurface`와
`OpenVisionShellHostViewTestSurfaceBindings`가 소유한다.

호출 경로는 다음과 같다.

```text
OpenVisionShellHostView constructor
 -> OpenVisionShellHostViewTestSurface(bindings)
 -> concrete Shell/Layer/Tool/Recipe owners
 -> existing public/internal *ForTest contract
```

Shell View는 `testSurface`의 생성·수명과 UI owner 연결을 소유한다. 실제 test
상태를 쓰는 객체는 기존 `ShellToolTestFacade`, `ShellLayerTestFacade`,
`ShellDockingTestFacade`, `OpenVisionRecipeContextStore`, command surface와
기존 controller들이다. Surface는 이 상태를 새로 복제하지 않고 bindings를 통해
읽거나 기존 owner의 동작을 호출한다. Window/View 종료와 하위 객체 해제 순서는
기존 Shell View가 계속 소유한다.

기존 Smoke 및 외부 호출 호환을 위해 Shell root에는 얇은 forwarding contract와
`TestSurface` 탐색 속성만 남겼다. 이 전달자는 업무 로직·상태·수명주기를
소유하지 않으며, 기존 이름과 시그니처를 `testSurface`에 그대로 연결한다.

D2 검증 결과:

- `dotnet build OpenVisionLab.sln --configuration Debug --no-restore`: PASS,
  warning 0 / error 0.
- `OpenVisionReadinessCheck` Debug: PASS.
- `VisionRecipeRunnerSmoke --runtime-stability-contract`: PASS.
- `Invoke-RefactorAudit.ps1 -Verify`: PASS,
  `PartialDeclarations=77`, `PartialTextMatches=2`, `ProjectCycles=0`,
  `ShellStorageCalls=0`.
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d2-20260911`.

Release build도 같은 변경 범위에서 PASS했다. 실제 GUI Shell 화면/모니터 검증은
다음 bounded slice의 독립 검증 게이트로 남긴다. D2 source/build 계약만으로
실제 모든 WPF 시각 상태를 검증했다고 주장하지 않는다. 다음 구현 단계는 D3
OpenGL rendering family다.

## 10. D3 구현 설계 — OpenGL rendering family

D3의 현재 owner는 `OpenGlDrawing`이라는 하나의 public static partial type다.
호출자는 `ImageCanvasControl`, `RoiImageCanvasViewModel`,
`OpenGlFontRenderOptions`, `OpenGlOverlayExtensions`와 외부 ImageCanvas
consumer다. OpenGL context는 호출자가 제공하고, texture/display-list의 생성·
해제는 기존 `ImageCanvasControl` 및 `OpenGlOverlayExtensions`가 담당한다.
따라서 renderer가 context나 resource lifetime을 소유하는 구조는 만들지 않는다.

실제 이동 경계는 다음 concrete type으로 고정한다.

```text
OpenGlDrawing (public compatibility façade; 83 public methods + ZoomFactor)
 ├─ OpenGlColorConverter
 ├─ OpenGlTextureRenderer
 ├─ OpenGlShapeRenderer
 ├─ OpenGlPenRenderer
 ├─ OpenGlTextRenderer
 ├─ OpenGlMeasurementRenderer
 ├─ OpenGlOverlayRenderer
 ├─ OpenGlOverlayTextRenderer
 └─ OpenGlDrawingState
```

각 concrete renderer는 기존 partial 파일의 책임과 메서드를 그대로 보유하고,
다른 renderer의 동작은 public façade를 통해 호출한다. interface, abstract
base, factory, provider, manager를 만들지 않는다. façade는 기존 public
signature과 `ZoomFactor` field, Smoke가 reflection으로 확인하는
`FontGlyphCount` constant를 보존한다. `OpenGlDrawingState`는 ROI handle 크기
조정 값만 보유하며 `OpenGlDrawing` facade의 외부 상태 계약을 복제하지 않는다.

Mutable state와 lifetime은 다음과 같이 고정한다.

- `OpenGlShapeRenderer`가 handle size state를 쓴다.
- `ImageCanvasControl`이 ZoomFactor를 쓰고 OpenGL context를 만든다.
- `OpenGlTextRenderer`가 caller-owned font entry list에 display-list entry를
  추가하지만 list release 책임은 기존 owner에 남긴다.
- `OpenGlOverlayRenderer`는 `CanvasShape.DisplayListId`를 compile 시 갱신하고,
  `OpenGlOverlayExtensions`의 기존 release 경로가 삭제한다.
- 모든 renderer는 호출이 끝난 뒤 기존 OpenGL enable/bind/matrix 계약을
  유지하고, 새 Mat/Bitmap/texture 복사나 장기 보유를 추가하지 않는다.

D3 source evidence는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d3-design-20260911`
의 `owner-map.csv`, `method-map.txt`, `summary.txt`, caller 목록이다. 구현 후
facade signature 비교, ImageCanvas library build, solution Debug/Release build,
runtime stability/OpenGL glyph contract, refactor audit를 실행한다. 실제 GPU와
모니터가 필요한 UI rendering 검증은 별도로 unverified로 기록한다.

## 11. D3 구현 결과 — OpenGL concrete renderers

D3 설계대로 기존 9개 `OpenGlDrawing` partial 파일을 역할별 concrete owner로
이동했다. `OpenGlDrawing.cs`는 기존 공개 API를 유지하는 compatibility façade로
남고, 83개 public static method의 시그니처를 모두 같은 이름·인자·반환형으로
전달한다. `ZoomFactor` field와 Smoke reflection contract인 `FontGlyphCount`
constant도 façade에 유지했다.

실제 owner는 `OpenGlColorConverter`, `OpenGlTextureRenderer`,
`OpenGlShapeRenderer`, `OpenGlPenRenderer`, `OpenGlTextRenderer`,
`OpenGlMeasurementRenderer`, `OpenGlOverlayRenderer`,
`OpenGlOverlayTextRenderer`, `OpenGlDrawingState`다. Renderer 사이 호출은
직접 interface/manager를 통하지 않고 기존 façade를 통한 concrete static 호출로
남겼다. `ConvertDotInfoToPoints`처럼 partial 간 private 공유에 의존하던 helper는
실제 사용 책임자인 `OpenGlPenRenderer`로 이동했다.

D3에서 변경하지 않은 소유권은 다음과 같다. OpenGL context는
`ImageCanvasControl`/호출자가 만들고, texture와 overlay display-list 해제는
기존 `ImageCanvasControl` 및 `OpenGlOverlayExtensions`가 담당한다. Renderer는
context를 보유하지 않고 caller-owned font list와 shape의 기존 상태만 갱신한다.

D3 evidence와 검증 결과:

- `OpenVisionLab.ImageCanvas` Debug/Release build: PASS.
- `OpenVisionLab.sln` Debug/Release build: PASS, warning 0 / error 0.
- `VisionRecipeRunnerSmoke` Debug/Release build: PASS, warning 0 / error 0.
- Runtime stability contract Debug/Release: PASS.
- ReadinessCheck Debug/Release: PASS.
- Public signature comparison: original 83 / façade 83, missing 0, unexpected 0.
- Refactor audit: PASS, `PartialDeclarations=68`, `PartialTextMatches=2`,
  `ProjectCycles=0`, `ShellStorageCalls=0`.

실제 GPU context에서의 모든 도형·텍스트·overlay 시각 결과와 모니터/DPI 상태는
실행하지 않았으므로 unverified다. 다음 bounded slice는 D4 Recipe Command
Surface의 shared-state partial을 concrete façade/workflow owner로 정리하는
작업이다.

## 12. D4 Recipe CommandSurface 설계 — 구현 전 확정

D4는 Recipe CommandSurface의 9개 수동 partial을 먼저 실제 호출·상태·수명 기준으로 재확인한 뒤 구현한다. 이 가족은 이미 하나의 `OpenVisionShellHostRecipeCommandSurface` 인스턴스가 XAML binding, command invalidation, Recipe/Pipeline 선택, step edit, execution, LLM draft, validation set, qualified snapshot 상태를 함께 소유한다. 따라서 partial 파일을 다른 이름의 forwarding wrapper나 새 Manager 계층으로 치환하지 않는다.

### 12.1 현재/의도 owner와 호출 경로

| 현재 파일 | 현재 owner | 구현 후 owner/region | 실제 호출 경로 | mutable-state writer | lifetime/release owner | 계약 |
| --- | --- | --- | --- | --- | --- | --- |
| `RecipeCommandSurface.cs` | `OpenVisionShellHostRecipeCommandSurface` | concrete facade: fields, constructor, binding properties, notifications | `OpenVisionShellHostView` constructor → `RecipeCommands` DP/DataContext → Recipe XAML | facade fields + `SetProperty`/`OnPropertyChanged`; execution session events | `OpenVisionShellHostView`가 생성·보유·해제 | type name, binding names, commands, constructor callbacks |
| `Commands.cs` | 같은 partial instance | `Commands` region | constructor → RelayCommand → existing handlers | facade command properties | facade lifetime | command names/CanExecute |
| `RecipeWorkspace.cs` | 같은 partial instance | `Recipe workspace` region; domain action은 existing `OpenVisionRecipeWorkspaceUseCase` | command → use case → projection → switch callback | use case result은 facade가 projection | Shell/View callback lifetime | Recipe create/copy/rename/delete/save |
| `PipelineLifecycle.cs` | 같은 partial instance | `Pipeline lifecycle` region; action은 existing lifecycle use case/projection | command → `OpenVisionRecipePipelineLifecycleUseCase` → projection → refresh/switch | use case + facade selection state | facade lifetime | pipeline activation/copy/rename/delete |
| `PipelineExchange.cs` | 같은 partial instance | `Pipeline exchange` region; XML/bundle action은 existing exchange use case/projection | command → file callback → exchange owner → facade status | exchange owner result projected to facade | file callback and facade lifetime | XML import/export/review bundle |
| `ValidationSets.cs` | 같은 partial instance | `Validation sets` region; document/evidence/selection existing owners | command → existing validation owners/storage → facade bindings | validation owners + facade selected state | facade lifetime; image paths remain strings/BitmapSource contract | validation set names, image paths, evidence/status |
| `QualifiedSnapshots.cs` | 같은 partial instance | `Qualified snapshots` region; `OpenVisionRecipeQualifiedSnapshotController` remains action owner | command → controller → callback/open evidence → facade projection | controller result projected by facade | Shell facade lifetime | qualification command/property/test names |
| `LlmXmlDraftWorkflow.cs` | 같은 partial instance | `LLM/XML draft` region; existing review/dry-run/identity owners retained | command → draft parser/review owner → facade text/status/image properties | facade draft state plus existing owners' result | facade lifetime; `BitmapSource` is replaced only by existing load path | draft XML, evidence packet, review decision, validation |
| `Handlers.cs` | same partial instance | named regions `Selection`, `Run/Validation`, `Guided setup`, `Step edit`, `Review`, `Notifications` | XAML/commands/test surface → facade methods → existing concrete owner | facade shared binding/session state; existing run-history/edit owners own independent result calculations | facade owns event subscriptions and unsubscribes through existing View lifecycle | public/internal test methods and notification ordering |
| `RunHistory.cs` (historical source if present in prior tree) | same partial instance | `Run history` region in concrete facade | selected run commands → existing `OpenVisionRecipeRunHistoryOrchestrationOwner` | run-history owner result, facade selected projections | facade lifetime | recent run/review/benchmark bindings |

### 12.2 D4 결정

1. `OpenVisionShellHostRecipeCommandSurface`를 `partial`이 아닌 하나의 concrete facade로 만든다.
2. 9개 파일의 본문은 기존 책임 이름을 유지한 class-level regions로 같은 concrete owner에 통합한다. 이것은 새 실행 계층을 만드는 변경이 아니라, 이미 한 인스턴스가 공유하던 상태와 notification 순서를 한 파일에서 추적 가능하게 만드는 구조 변경이다.
3. Recipe/Pipeline/validation/step edit/run history/qualification의 실제 독립 동작은 현재 concrete owner를 계속 사용한다. 새 interface, abstract base, factory, manager, wrapper는 추가하지 않는다.
4. XAML binding property, command, public/internal test method, callback parameter, XML serialization, Preview/Run 명시 동작, event subscription/unsubscription 순서는 변경하지 않는다.
5. facade에서 독립 가능한 작업을 새로 추출하지 않는 이유는 현재 메서드가 `SetProperty`, selection, execution session, command invalidation, status text를 함께 읽고 쓰며, 각 caller가 immutable request/result로 대체될 호출 경계를 아직 제공하지 않기 때문이다. 이미 독립 경계를 가진 concrete owner는 기존 코드로 충분하다.
6. 완료 후 `CommandSurface` 폴더에는 `RecipeCommandSurface.cs`만 남고 수동 `partial` 선언은 0이다. 파일 이름은 공개 계약에 포함되지 않지만 smoke contract가 확인하므로 stale path 검색과 contract build를 수행한다.

### 12.3 구현/검증 순서

- 구현 전에 이 설계와 원본 9개 파일을 D4 design evidence에 보관한다.
- 동일한 encoding/line ending을 유지한 채 root file에 using union과 responsibility region을 적용하고, class declaration을 concrete로 바꾼다.
- 새 class에 대한 Debug/Release solution build, `VisionRecipeRunnerSmoke` Debug/Release build, 기존 Recipe lifecycle/validation/readiness 계약, `Invoke-RefactorAudit.ps1 -Verify`, `TestDocumentationIndex.ps1`, `git diff --check`를 실행한다.
- 최종 정적 기대치는 D3의 `PartialDeclarations=68`에서 Recipe 9개를 제거한 `59`, `PartialTextMatches=2`, Recipe manual partial 0, project cycle 0, Shell storage call 0이다.
- 실제 WPF visual state, GPU/OpenGL rendering, monitor/DPI, camera/SDK 장시간 실행은 이 source/build slice에서 실행하지 않고 미검증으로 기록한다.

## 13. D4 구현 결과 — Recipe concrete facade

D4 설계대로 `Recipe/CommandSurface`의 9개 수동 partial 선언을
`RecipeCommandSurface.cs` 하나의 `public sealed class`로 통합했다. 기존 파일의
본문은 `Command creation`, `Selection/execution/guided setup/step edit`,
`LLM/XML draft`, `Pipeline exchange`, `Pipeline lifecycle`, `Qualified snapshots`,
`Recipe workspace`, `Validation sets` named region에 보존했다. 기존 root의
Fields, Constructor, Binding Properties/Commands, Run History region도 명시적
영역으로 닫았다.

이 변경은 동작을 새로 구현한 것이 아니다. `OpenVisionRecipeWorkspaceUseCase`,
`OpenVisionRecipePipelineLifecycleUseCase`, `OpenVisionRecipePipelineExchangeUseCase`,
validation document/evidence/selection owners, qualified snapshot controller,
step-edit owners, run-history owner, LLM/review owners가 계속 독립 동작의
실제 owner다. facade는 XAML binding, command creation/CanExecute,
`INotifyPropertyChanged`, Shell callback과 결과 projection만 소유한다.

호출 경로와 상태·수명은 다음과 같이 유지된다.

```text
OpenVisionShellHostView constructor
 -> OpenVisionShellHostRecipeCommandSurface (one concrete facade)
 -> Recipe XAML bindings / RelayCommand
 -> existing Recipe workflow owner
 -> facade projection -> OpenVisionShellHostView lifetime
```

mutable state writer는 facade의 기존 fields와 `SetProperty`/`OnPropertyChanged`,
execution session event handler이며, 파일/Recipe/Pipeline 변경은 기존 concrete
owner가 수행한다. `OpenVisionShellHostView`가 surface를 생성·보유하고 기존
종료 흐름이 callback/event 수명을 닫는다. 새 interface, abstract base, factory,
manager, forwarding wrapper, 이미지/Bitmap/Mat lifetime은 추가하지 않았다.

Readiness 도구가 삭제된 `Handlers.cs`와 `ValidationSets.cs`를 직접 읽지 않도록
현재 `RecipeCommandSurface.cs` source family를 읽게 최소 수정했다. 이 수정은
기존 ownership token 검사를 유지하면서 physical consolidation 이후에도 같은
정책을 검증하기 위한 것이다.

D4 source/검증 evidence:

- 설계와 통합 전 원본: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d4-design-20260911`
- 구현/검증: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d4-20260911`
- source-body SHA-256 비교: 8개 기존 partial body와 기존 facade core 모두
  `match=True`.
- Refactor audit: `CSharpFiles=815`, `XamlFiles=60`,
  `PartialDeclarations=59`, `PartialTextMatches=2`, `ProjectCycles=0`,
  `ShellStorageCalls=0`.
- Solution Debug/Release 및 VisionRecipeRunnerSmoke Debug/Release build:
  0 warning / 0 error.
- ReadinessCheck Debug/Release: PASS.
- Recipe focused contracts: Debug 24/24 PASS(실행 session은 D: 복사 runtime에서
  12/12), Release 23/23 PASS.
- DocumentationIndex와 `git diff --check`: PASS. diff check의 기존
  LF/CRLF 변환 경고는 whitespace error가 아니다.

실제 WPF 화면의 theme/layout/DPI/monitor, GPU rendering, camera/SDK,
장시간 native shutdown은 실행하지 않았으므로 `소스 코드 기준 검토 완료 /
실제 Runtime UI 검증 필요`로 남긴다. 다음 bounded slice는 보호 partial
inventory와 최종 D5 closure다.

## 14. D5 보호 partial closure 설계 — 구현 전 확정

D5는 남은 59개 compiled partial declaration을 보호 계약으로 전수 고정하고, 수동
partial family가 0이라는 것을 정적 검색·감사·빌드·문서 인덱스로 증명하는
마지막 bounded slice다. D4에서 완료한 Recipe facade는 다시 열지 않는다.

### 14.1 보호 집합

| 집합 | 개수 | owner/reason | 호출·수명 계약 |
| --- | ---: | --- | --- |
| WPF/XAML code-behind 및 외부 smoke XAML contract | 56 | 각 Window/UserControl concrete View와 WPF generated `InitializeComponent` composition | XAML loader → `InitializeComponent` → View; visual state/binding/event/Dispose는 해당 View/Window가 소유 |
| `ImageCanvasControl` native/designer composition | 2 | SharpGL/WinForms designer가 생성한 control field와 native dispose 순서 | control constructor/designer → native context/control; `Dispose` 순서는 control owner가 소유 |
| `Settings.Designer.cs` generated contract | 1 | .NET Settings designer가 생성한 settings type/property contract | Settings runtime → generated property/default/serialization contract |
| 합계 | 59 | 모두 생성·프레임워크 또는 외부 smoke contract | 수동 workflow partial 0 |

### 14.2 D5 검증 설계

1. `src`와 `tools`에서 declaration line을 다시 수집하고, smoke 문자열 2개를
   compiled declaration과 분리한다.
2. 각 59개 row에 file, line, type, project, namespace/category, caller,
   mutable-state writer, lifetime/release owner, binding/public/test contract를
   기록한다.
3. `RecipeCommandSurface` manual declaration과 삭제된 8개 Recipe path,
   D2/D3 이전 manual family path가 `src`/`tools`에 없는지 검색한다.
4. `Invoke-RefactorAudit.ps1 -Verify`가
   `PartialDeclarations=59;PartialTextMatches=2;ProjectCycles=0;
   ShellStorageCalls=0`을 보고하는지 확인한다.
5. Solution/changed smoke Debug·Release build, ReadinessCheck, focused
   structural contracts, DocumentationIndex, `git diff --check`를 최종 실행한다.
6. 실제 WPF theme/layout/DPI/monitor, GPU/camera/SDK, long-running native
   shutdown은 환경 경계로 미검증 기록한다. 이 경계는 source/build closure를
   막지 않는다.

### 14.3 D5 완료 조건

- protected inventory 59 rows and owner map are saved in D: evidence.
- manual partial declaration count is 0; only protected categories remain.
- Recipe/OpenGL/TestHooks stale source/tool paths are absent.
- final Debug/Release build and static/focused checks pass.
- PL-0013 C5/C7/C8 evidence is updated and the same heartbeat is closed only
  after the final report is recorded.

## 15. D5 최종 결과 — 보호 partial closure (2026-09-11)

D5는 `src`와 `tools`를 다시 스캔해 compiled partial declaration 59개를
전수 고정했다. 분류는 WPF/XAML code-behind 및 외부 smoke XAML contract 56개,
`ImageCanvasControl` native/designer composition 2개,
`Settings.Designer.cs` generated contract 1개다. 수동 partial declaration은
0개이고, 문자열로 남은 smoke contract `public partial class ...` 2개는
`PartialTextMatches`로 별도 기록했다.

각 보호 row에는 file, line, project, namespace, type, category, caller,
mutable-state writer, lifetime/release owner, binding/public/test contract를
기록했다. 이 inventory는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d5-20260911\protected-partial-inventory.csv`
와 owner map에 있다.

최종 호출·소유권 경로는 다음과 같다.

```text
Program.Main
 -> OpenVisionLabApplication.Run
 -> OpenVisionShellHostWindow
 -> OpenVisionShellHostView
 -> RecipeCommandSurface concrete facade / concrete feature owner / XAML View
 -> Layer / Tool / Pipeline / Result / Review owner
```

D2/D3/D4에서 완료한 concrete owner는 다시 partial로 분리하지 않았다. XAML,
Settings, ImageCanvas designer/native 조합만 framework/generated contract로
유지했다. `src`/`tools` stale path 검색에서 Recipe 이전 파일명,
`OpenVisionShellHostView.TestHooks.cs`, OpenGL 이전 partial 파일명, Recipe
partial declaration은 모두 발견되지 않았다.

최종 검증 evidence는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\partial-structure-d5-20260911`에
있다.

- `D5_PROTECTED_INVENTORY=PASS`: declarations 59, text matches 2, manual 0,
  56+2+1 protected set.
- `Invoke-RefactorAudit.ps1 -Verify`: `CSharpFiles=815`, `XamlFiles=60`,
  `PartialDeclarations=59`, `PartialTextMatches=2`, `ProjectCycles=0`,
  `ShellStorageCalls=0`.
- Solution Debug/Release 및 VisionRecipeRunnerSmoke Debug/Release build:
  PASS, warning 0 / error 0. 이 결과는 D4 구현 evidence에 저장되어 있고 D5
  source는 변경하지 않았다.
- ReadinessCheck Debug/Release: PASS.
- Recipe focused contracts: Debug 24/24 PASS, Release 23/23 PASS; 실행 session은
  D: 복사 runtime에서 12/12 PASS.
- DocumentationIndex PASS (`IndexedPaths=288`, `Routes=16`, `RootRedirects=102`).
- `git diff --check`는 whitespace error 없이 기존 LF/CRLF 변환 경고만 출력했다.

PL-0013의 설계·구현·검증 조건은 모두 충족됐다. 실제 WPF theme/layout/DPI/
monitor, GPU rendering, camera/SDK, 장시간 native shutdown은 이 환경에서
실행하지 않았으므로 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로
남긴다. 이는 보호 partial closure의 정적 완료를 막지 않지만, 제품 운영 승인이나
배포 승인을 의미하지 않는다.
