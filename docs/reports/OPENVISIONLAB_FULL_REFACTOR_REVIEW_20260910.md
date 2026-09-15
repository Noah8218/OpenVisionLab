# OpenVisionLab 2D 전체 구조·MVVM·모듈화 검수 — 2026-09-10

Status: **Complete** for the bounded whole-project audit and the two concrete
MVVM ownership corrections described below. This does not claim that every
large class, every filesystem call, or every field-runtime/UI matrix has been
rewritten or qualified.

## 1. 검수 기준과 입력

| 항목 | 값 |
| --- | --- |
| 저장소 | `C:\Git\2D\Dev` |
| Branch | `codex/public-sample-ux-docs` |
| Commit SHA at review start | `65e2fd8b77989172d68d0aad81d489df94baf474` |
| Target Framework | `net8.0-windows7.0`, WPF, WinExe |
| 범위 | `src`, `tools`, `.sln`, 모든 관련 `.csproj`, View/ViewModel/Service/SDK 호출부, focused contract와 sample |
| 보존 경계 | 기존 dirty worktree와 완료 owner를 유지했으며 `C:\Git\2D\Original`, commit, push는 변경하지 않음 |
| 감사 증거 | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-directory-policy-20260910\refactor-audit-final5` |

검수는 파일명과 README만으로 판단하지 않았다. solution/project reference
graph, `Program.Main`/bootstrap, 실제 Recipe/Pipeline/Review 호출부, WPF
View와 ViewModel, OpenCvSharp/Bitmap/BitmapSource 경계, standalone smoke
project를 직접 확인했다. 정적 수치는 결함 개수가 아니라 추가 조사가 필요한
신호다.

## 2. 현재 Architecture와 기능 흐름

현재 제품의 핵심 흐름은 다음과 같이 추적된다.

```text
Program.Main
  -> OpenVisionLabApplication
  -> ApplicationRuntimeContext
  -> OpenVisionShellHostWindow
  -> Image/Layer
  -> NativeToolDocument / ViewModel / PropertyGrid
  -> VisionPipelineExecutionPlan / VisionPipelineExecutionService
  -> RecipeState / RecipeRuntimeStorage / VisionPipelineStorage
  -> VisionPipelineRunResult / VisionRecipeRunResult
  -> Review execution controller / document / presenter
```

실제 책임은 다음과 같다.

| 경계 | 현재 concrete owner |
| --- | --- |
| Image | `CanvasImageLoader`, `BitmapImageConverter`, `ImageSpaceService` |
| Layer | `DisplayLayerStore`, `ImageSpaceService`와 Lease |
| Tool | `OpenVisionNativeToolRegistry`, `NativeToolDocument`, 기존 PropertyGrid/parameter 경로 |
| Inspection | `VisionPipelineAppToolFactory`, `ExecuteStep`와 기존 SDK Tool/adapter |
| Pipeline | `VisionPipelineExecutionPlan`, `VisionPipelineExecutionService` |
| Recipe | `RecipeState`, `RecipeRuntimeStorage`, `VisionToolRepository`, `VisionToolStorage`, `VisionPipelineStorage` |
| Result | `VisionPipelineRunResult`, `VisionRecipeRunResult`, step summary/report |
| Review | `OpenVisionPipelineReviewExecutionController`, document revision gate, 기존 presenter/ViewModel |

27개 project와 34개 `ProjectReference`의 cycle은 0개이며 external project
reference도 0개였다. 따라서 현재 구조는 concrete responsibility 경계가
대체로 유지되는 부분 모듈화 상태다. Shell, smoke runner, PropertyGrid
adapter처럼 집중된 파일은 있지만 줄 수만으로 자동 분리하지 않았다.

## 3. 정적 전체 검수 결과

최종 감사 결과는 다음과 같다.

```text
REFACTOR_AUDIT=PASS
CSharpFiles=844, CSharpLines=277236
XamlFiles=60, XamlLines=24646
PartialDeclarations=110
ViewModelFiles=32, DirectUiOrDialogFiles=0, DirectIoFiles=6
Projects=27, ProjectReferences=34, ExternalReferences=0, Cycles=0
ShellFiles=10, ShellLines=10116, ShellRunHistoryStorageCalls=0
```

1,000줄 이상 파일은 28개, 2,000줄 이상은 12개, 3,000줄 이상은 8개다.
대표 집중 지점은 `tools/PipelineViewerScreenshotSmoke/Program.cs` 40,263줄,
`OpenVisionLabDirectSmokeRunner.cs` 18,795줄,
`VisionRecipeRunnerSmoke/Program.cs` 13,834줄,
Shell XAML 8,493줄,
`OpenVisionShellHostRecipeCommandSurface.Handlers.cs` 3,812줄,
`WpfPropertyGridAdapter.cs` 3,545줄이다. 이 목록은 책임 분리 후보를 찾는
출발점이며 기계적인 추출 지시가 아니다.

### MVVM 검수

초기 감사에서 `ImageCompareWindow.xaml.cs`가 파일 선택뿐 아니라 최근
디렉터리의 메모리 상태, 설정 파일 저장/복원, 경로 확인을 직접 소유하는 것을
확인했다. 이 책임은 View 수명과 섞일 이유가 없으므로 다음과 같이 수정했다.

- `ImageCompareDirectoryPolicy`가 ViewModel 수명 안에서 최근 디렉터리의
  메모리 상태와 `CONFIG/image_compare_last_directory.txt` 저장/복원을 소유한다.
- `ImageCompareViewModel`은 `InitialImageDirectory`를 binding/Window에
  투영하고 `LoadImages`에서 policy를 호출한다.
- `ImageCompareWindow.xaml.cs`는 OpenFileDialog, slot pointer mapping,
  fit/sync UI, window lifecycle만 남긴다.
- 기존 `ImageCompareImageResource`의 Bitmap/BitmapSource 생성·동결·Dispose와
  metadata 책임은 재사용하며 옮기지 않았다.

감사 정규식은 `EnumItemType.Window` 같은 enum 이름을 WPF 사용으로 세지 않으며,
`Process.Start`도 ViewModel UI 신호로 식별한다. 그 결과 Image Compare 수정
후에도 `LogPanelViewModel`이 로그 폴더 열기와 최신 로그 파일 I/O를 직접 갖고
있음을 확인했다. `LogPanelFileAccess`를 concrete infrastructure owner로
추가하고 다음을 이동했다.

- 최신 `*ALL.log` 검색과 `FileStream` 읽기
- recoverable file exception의 warning 기록
- 로그 폴더를 여는 `Process.Start`

`LogPanelViewModel`의 기존 public 생성자, command 이름, 필터/요약/타이머,
화면 binding은 유지했다. 최종 정적 감사의 `DirectUiOrDialogFiles=0`은
ViewModel이 concrete WPF Window/Dialog/Process.Start를 직접 소유하지 않는다는
뜻이다. `DirectIoFiles=6`은 모두 표시용 파일명·파일 존재 확인 또는 legacy
ImageCanvas/샘플 이미지 입력 검증으로 남아 있으며, 전부를 제거하는 것이 이번
범위의 목표는 아니다.

### Partial 검수

raw `partial` 선언은 110개다. 분류 결과는 다음과 같다.

- XAML/generated composition 56개
- generated settings 1개
- docking composition 19개
- ImageCanvas ViewModel 3개
- OpenGL rendering composition 9개
- Shell command surface 10개
- Pipeline review document 2개
- 기타 WPF composition 8개
- test source string match 2개

따라서 실제 production/framework partial을 일괄 삭제하지 않았다. XAML
generated code와 cohesive rendering/docking/Shell composition은 현재 소유권과
생성/호출 계약을 숨기지 않으며, 완료된 Shell owner를 파일 크기만으로 다시
나누지 않았다. 이 검수에서 새 partial은 추가하지 않았다.

### 모듈화와 의존성

Project graph에 cycle이 없고, Core/Recipe/Pipeline/Review에는 이미 직접적인
concrete owner와 focused contract가 있다. 구현체가 하나인 영역에
`Interface -> AbstractBase -> Factory -> Manager` 계층을 새로 만들지 않았다.
SDK 경계와 WPF PropertyGrid metadata가 남아 있으므로 Core를 독립 package라고
표현하지 않았고, 실제 host 분리 요구나 재현 결함 없이 `AppPathService`/기본
runtime static 또는 WPF metadata 경계를 재설계하지 않았다.

## 4. 수정 작업

### Image Compare 디렉터리 owner

변경 파일:

- `src/OpenVisionLab/UI/Popup/Wpf/ImageCompare/ImageCompareDirectoryPolicy.cs`
- `src/OpenVisionLab/UI/Popup/Wpf/ViewModels/ImageCompareViewModel.cs`
- `src/OpenVisionLab/UI/Popup/Wpf/ImageCompareWindow.xaml.cs`
- `tools/OpenVisionLab.ImageCompare/OpenVisionLab.ImageCompare.csproj`
- `tools/VisionRecipeRunnerSmoke/ImageCompareDirectoryPolicyContract.cs`
- `tools/VisionRecipeRunnerSmoke/Program.cs`의 contract dispatch

기존 동작은 선택된 이미지의 디렉터리를 다음 Image Compare 파일 선택에서
기억하는 것이었다. 변경 후에도 같은 동작을 유지하되 Window가 경로 저장을
직접 하지 않는다. 절대 경로인 `OPENVISIONLAB_DATA_ROOT`와 기존 fallback
경로 semantics를 보존했고, invalid image input은 기억된 디렉터리를 바꾸지
않는다.

### Log Panel 외부 I/O owner

변경 파일:

- `src/Libraries/OpenVisionLab.Logging.Controls/Infrastructure/LogPanelFileAccess.cs`
- `src/Libraries/OpenVisionLab.Logging.Controls/ViewModel/LogPanelViewModel.cs`
- `tools/RefactorAudit/Invoke-RefactorAudit.ps1`

기존 로그 패널 command와 화면 결과는 보존했다. ViewModel은 filter/summary/
timer 상태를 소유하고, 파일 탐색·읽기와 shell folder launch는 명시된
concrete owner가 소유한다. 새 interface나 manager 계층은 만들지 않았다.

## 5. 유지한 기존 구조와 삭제하지 않은 코드

다음 완료 owner는 재구현하거나 partial로 숨기지 않았다.

- `ImageCompareImageResource`의 Bitmap/BitmapSource 수명
- `VisionToolRepository`, `VisionToolStorage`, `SerializeHelper`
- `VisionPipelineStorage`, execution plan/service, review controller/revision gate
- ImageSpace Lease와 existing Mat clone/replace/clear lifetime
- 기존 Preview/Run, Recipe/XML, SDK factory와 public binding 계약
- 완료된 OVL-01~53의 concrete ownership slice

이 작업에서는 production 알고리즘, 공개 file format, SDK contract, 사용자
workflow를 삭제하지 않았다. dead code로 보이는 public `BackgroundLoopWorker`도
외부 binary/reflection 사용 가능성을 배제하지 못해 삭제하지 않았다.

## 6. 위험, 검증, 미검증

실행한 검증:

- solution Debug: 경고 0, 오류 0
- solution Release: 경고 0, 오류 0
- `OpenVisionLab.Logging.Controls` Debug build: 경고 0, 오류 0
- `VisionRecipeRunnerSmoke` Debug/Release build: 경고 0, 오류 0
- `PipelineViewerScreenshotSmoke` Release build: 경고 0, 오류 0
- `OpenVisionReadinessCheck` Debug/Release: PASS
- `log_panel_contract_check`: PASS
- `logging_buffer_contract`: PASS
- Image Compare directory policy contract: Debug/Release PASS
- Image Compare resource contract: Debug/Release PASS
- Image Compare WPF target: `wpf_image_compare=OK`, 1280x760, layout/text/internal 0
- Refactor audit final5: PASS, cycle 0, direct ViewModel UI 0

주요 증거 root는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\image-compare-directory-policy-20260910`이다.
정적 owner와 contract 검증은 완료했지만 다음은 이 실행에서 검증하지 않았다.

- 모든 supported theme와 Wide/Compact layout
- 125/150/175/200% DPI별 전체 Image Compare/Log Panel input matrix
- Log Panel의 `Open Folder` command는 외부 Explorer를 띄우는 동작이라 이번
  계약에서 실제 실행하지 않았으며, command binding과 concrete owner만 확인함
- 실제 desktop EXE의 장시간 camera/SDK 운전, field image 물리 정확도
- 강제 종료·disk-full·권한 오류를 포함한 전체 process-crash 저장 transaction

따라서 UI 소스와 대표 smoke 기준으로는 검토 완료했지만,
**소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요** 상태를 유지한다.

## 7. 개발자 읽기 순서

Image Compare를 처음 읽을 때는 다음 순서를 따른다.

1. `ImageCompareWindow.xaml.cs` — UI 진입과 허용된 interaction
2. `ImageCompareViewModel.cs` — binding, slot state, 호출 진입
3. `ImageCompareDirectoryPolicy.cs` — 최근 디렉터리 저장/복원
4. `ImageCompareImageResource.cs` — Bitmap/BitmapSource lifetime
5. `ImageCompareDirectoryPolicyContract.cs`와 `ImageCompareResourceContract.cs` — 독립 회귀 계약

호출 경로는 `LoadImages_Click -> ImageCompareViewModel.LoadImages ->
ImageCompareDirectoryPolicy.RememberImageDirectory ->
ImageCompareSlotViewModel.Load -> ImageCompareImageResource.Load`다.

전체 제품은 `docs/admin/CODEBASE_STRUCTURE.md`의 `1.1 Start Here`와
`9.46 Image Compare MVVM와 디렉터리 정책을 읽는 순서`, `9.47 Log Panel
MVVM와 외부 I/O를 읽는 순서`를 먼저 읽고, 그 다음 Shell command surface,
Pipeline/Recipe execution, Result/Review 순서로 읽는다.

## 8. 남은 기술 부채와 다음 우선순위

- 6개 ViewModel의 남은 파일 경로 확인/이미지 입력 투영은 표시와 입력 검증의
  실제 필요를 확인한 뒤 별도 owner가 더 단순해지는지 판단한다.
- Shell/PropertyGrid/smoke runner의 큰 파일은 독립 상태·수명·호출 경계가
  입증될 때만 후속 slice로 분리한다.
- `AppPathService`/기본 runtime static과 WPF metadata parameter의 portable
  경계는 실제 host 분리 요구 또는 재현 defect가 생길 때만 재평가한다.
- 전체 theme/DPI/monitor/장시간 native resource qualification은 운영 환경에서
  별도 수행한다.

다음 구조 작업: `AppPathService`/기본 runtime static 및 WPF metadata portable
경계의 source-only 재평가(실제 host split은 요구/재현 결함이 있을 때만).
Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.

## 9. 완료 증거

Status: **Complete**

Scope: 전체 source/XAML/project graph의 MVVM·partial·모듈화 검수와
Image Compare 및 Log Panel의 명확한 외부 I/O ownership correction.

Acceptance criteria:

- MVVM direct ViewModel UI/Dialog/Process.Start signal: **0**
- Project cycle: **0**
- Existing Image Compare/Log Panel binding and focused behavior: **preserved and tested**
- New abstraction count: **2 concrete owners, 0 new interface/manager/factory chain**
- Build: **solution Debug/Release warning 0, error 0**

Boundary: 전체 현장 운전과 모든 WPF theme/DPI 조합은 이 문서가 증명하지
않으며, 위에 적은 후속 검증 항목으로 남긴다.
