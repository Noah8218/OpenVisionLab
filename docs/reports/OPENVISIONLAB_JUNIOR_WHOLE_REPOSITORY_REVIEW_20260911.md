# OpenVisionLab 2D 전체 코드베이스 주니어 탐색성 검토

Updated: 2026-09-11 KST

Status: **Complete** for the current-source and documentation audit. This report
does not claim that every WPF state, camera/SDK path, DPI/theme combination, or
long-running native operation was executed.

## 검토 기준

| 항목 | 현재 확인값 |
| --- | --- |
| Repository | `C:\Git\2D\Dev` |
| Branch | `codex/public-sample-ux-docs` |
| HEAD | `0a77e60e444b12757565ee977216a994446b53a6` |
| Target framework | `net8.0-windows7.0` |
| Product version | `2.2.0-dev.2` |
| Project graph | 27 projects, 34 `ProjectReference`, cycle 0 |
| Source inventory | 816 C# files / 60 XAML files (excluding `bin` and `obj`) |
| Partial inventory | 59 protected compiled declarations; 2 smoke-contract literal matches |
| Evidence | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-whole-repo-review-20260911` |

The review read the solution, every relevant project reference, the application
entry point, Shell/View/ViewModel/Service code, ImageSpace and Bitmap/Mat
ownership code, Pipeline/Recipe/Review paths, smoke contracts, and current
architecture records. README-only conclusions were not used.

## 한 줄 평가

OpenVisionLab은 임시 샘플이나 무작위 파일 모음이 아니다. 기능별 concrete
owner, 프로젝트 경계, 실행 계약, 이미지 소유권이 이미 상당히 갖춰진
제품형 코드베이스다. 그러나 처음 참여하는 개발자는 Shell 조합 지점과 큰
Recipe/Pipeline 화면을 여러 단계 따라가야 하므로 **모듈화는 양호하지만 첫
수정 난이도는 중간 이상**으로 느낄 가능성이 높다.

## 처음 열었을 때 따라갈 시작 경로

```text
Program.Main
  -> OpenVisionLabApplication.Run
  -> OpenVisionShellHostWindow
  -> OpenVisionShellHostView
  -> Image / Layer / Tool
  -> VisionPipelineExecutionService
  -> Recipe / persisted run result
  -> Pipeline Review / Run History
```

코드에서 바로 시작할 파일은 다음 순서다.

1. [`Program.cs`](C:/Git/2D/Dev/src/OpenVisionLab/Program.cs:45)
2. [`OpenVisionLabApplication.cs`](C:/Git/2D/Dev/src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs:64)
3. [`OpenVisionShellHostWindow.xaml.cs`](C:/Git/2D/Dev/src/OpenVisionLab/UI/Menu/Wpf/Windows/OpenVisionShellHostWindow.xaml.cs:33)
4. [`OpenVisionShellHostView.xaml.cs`](C:/Git/2D/Dev/src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs:154)
5. `Shell/Tooling`의 tool selection/window lifecycle owner
6. `Shell/Documents`의 document restore/create owner
7. `Core/Pipeline/Execution`의 plan와 execution service
8. `Core/Recipe` 및 `Core/Pipeline/Storage`의 Recipe/XML/result 저장 owner
9. `UI/Menu/Wpf/PipelineReview`의 execution/document owner
10. [`OpenVisionPipelineReviewView.xaml.cs`](C:/Git/2D/Dev/src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml.cs:1)의 표시 상태

이 순서는 `Image → Layer → Tool → Inspection → Pipeline → Recipe → Result →
Review` 흐름을 유지하면서 조합·실행·저장·표시의 소유자를 구분한다.

## 주니어가 긍정적으로 느낄 부분

### 프로젝트와 폴더 경계

27개 프로젝트 사이에 참조 순환이 없고, `Core`, `UI/Menu/Wpf/Recipe`,
`UI/Menu/Wpf/PipelineReview`, `Common`의 책임 폴더가 실제 호출 경계와
대체로 맞는다. `Common` 아래에는 `Account`, `Events`, `Imaging`,
`Persistence`, `PropertyGrid`, `Recipe`, `Results`, `Runtime` 같은 책임별
하위 폴더가 있다. 남은 밀집 폴더도 대체로 `Recipe/Review`, `Learn`, 또는
독립 smoke 도구처럼 한 주제를 가진다.

### 이미지 수명 API

[`ImageSpaceFrame`](C:/Git/2D/Dev/src/Libraries/OpenVisionLab.ImageSpace.Core/ImageSpaceFrame.cs:23)은
`Borrow`와 `TakeOwnership`을 구분한다. 이 이름만으로 원본 이미지를 누가
해제하는지 추적할 수 있고, Dispose 이후 접근도 방어한다. ImageSpace lease,
Pipeline 늦은 결과 이미지 해제, Mat 교체/Dispose도 별도 owner가 존재한다.

### 저장과 실행 검증

기존 `SerializeHelper`의 임시 파일/교체 저장, Pipeline 실행 결과 계약,
ReadinessCheck, Recipe/Review smoke가 있다. 따라서 새 저장 서비스나 새
테스트 프레임워크를 추가할 필요가 없다. 현재 검증은 표준 unit-test 특성보다
실행 가능한 contract/smoke 도구 중심이다.

### 이미 정리된 MVVM 경계

Image Compare 디렉터리 정책과 Log Panel 파일 접근은 기존 concrete owner로
이동되었다. Shell Recipe dialog/pending-edit 결정도 `RecipeDialogAdapter`가
담당한다. ViewModel이 Window나 구체 컨트롤을 직접 생성하지 않도록 정리된
경계가 존재한다.

## 주니어가 부담스럽게 느낄 부분

### 1. Shell 조합 생성자

`OpenVisionShellHostView`는 실제 화면 상태의 단일 owner라기보다 Session,
Layer, Workspace, Tooling, Recipe, Document, Test facade를 조합하는
composition layer다. 생성자에서 수십 개의 collaborator와 callback을
연결하므로, 처음 보는 개발자는 “이 상태를 누가 실제로 쓰는가?”를 각
concrete owner까지 따라가야 한다. 이 파일을 더 작은 partial이나 Manager로
감싸는 것은 현재 독립 lifetime 경계가 증명되지 않았으므로 보류한다.

### 2. Recipe CommandSurface

[`RecipeCommandSurface.cs`](C:/Git/2D/Dev/src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs:23)은
약 10,076줄이다. `Run History`, command 생성, LLM/XML draft, Pipeline
exchange, review bundle, qualified snapshot, validation-set projection이
region으로 구분되어 있지만 공통 binding/state 조합 지점이 크다. 기존
History/Validation owner를 호출하는 것은 확인했지만, 독립 snapshot/result
lifetime과 focused seam이 새로 증명되지 않았으므로 파일 크기만으로 다시
분리하지 않는다.

### 3. Pipeline Review View

[`OpenVisionPipelineReviewView.xaml.cs`](C:/Git/2D/Dev/src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml.cs:1)은
object/geometry/instance/circle/scale 결과, 이미지, 선택 상태, plot/layout
상태와 mouse hit-testing을 함께 다룬다. Review 실행 세대와 이미지 수명은
기존 Document/Execution/Layer owner가 가지고 있어 전면 MVVM 위반은 아니다.
다만 표시 state와 결과 projection을 처음 수정할 때는 View, ViewModel,
Document, execution controller를 함께 읽어야 한다.

### 4. 큰 smoke 도구와 제품 코드를 혼동하기 쉬움

가장 큰 파일은 제품 architecture가 아니라 실행 가능한 검증 도구다.

| 파일 | 줄 수 | 해석 |
| --- | ---: | --- |
| `tools/PipelineViewerScreenshotSmoke/Program.cs` | 40,324 | fixture/runner/assertion이 한 실행 도구에 집중 |
| `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | 18,795 | desktop smoke orchestration |
| `tools/VisionRecipeRunnerSmoke/Program.cs` | 13,848 | Recipe contract dispatch |
| `src/.../RecipeCommandSurface.cs` | 10,076 | 제품 Shell command/state 조합 |
| `src/.../OpenVisionShellHostView.xaml.cs` | 2,121 | WPF composition/lifecycle |

도구 파일의 줄 수를 제품 모듈의 결함으로 판단하지 않는다. 다만 검증
실패를 수정하는 개발자는 fixture, runner, assertion의 실행 경계를 함께
읽어야 한다.

## MVVM·partial·모듈화 판정

### MVVM

현재 ViewModel 파일 30개에서 WPF binding/image/input 또는 파일 경로 신호가
검색된다. 이 수치는 위반 개수가 아니라 검토 신호다.

- `OpenVisionPipelineReviewViewModel`과 `RoiEditorViewModel`의
  `BitmapSource/BitmapImage`는 화면 binding을 위한 표시 타입이다.
- `ImageCompareViewModel`은 `ImageCompareDirectoryPolicy`와
  `ImageCompareImageResource`를 사용하지만 BitmapSource/Brush를 노출하므로
  WPF 표시 결합이 남아 있다.
- `OpenVisionWorkspaceSamplePickerViewModel`은 CollectionView/BitmapImage와
  sample path 검사를 함께 다루지만, 문서·파일/프로세스 동작은 기존
  `OpenVisionWorkspaceLearnDocumentService`가 소유한다. 화면별 선택 상태와
  미리보기 binding을 분리하면 오히려 호출 경로가 늘어나므로, 이번 후속
  검토에서는 새 UI/파일 owner를 추가하지 않고 현재 concrete owner를 유지했다.
- `VisionToolPropertySummaryViewModel`의 `File.Exists`는 경로 표시 상태를
  계산하는 신호이며, 저장 책임으로 확대해서 해석하지 않는다.

code-behind의 `OpenFileDialog`, `SaveFileDialog`, `File.Exists`도 22개
검색 신호가 있지만, Window/UserControl에 고유한 file picker·pointer·image
loading이면 허용된 UI adapter일 수 있다. `ImageCompareWindow`의 picker와
`VisionToolSignalInspectorView`의 TSV 저장 dialog는 이 분류에 해당하며,
실제 업무 정책을 View로 옮겼다고 단정하지 않는다.

### Sample Picker 후속 구조화 slice

`OpenVisionWorkspaceSamplePickerWindow`가 창 수명과 `ShowDialog` 결과를 소유하고,
`OpenVisionWorkspaceSamplePickerViewModel`이 카탈로그·선택·검색·쌍 결정·미리보기
binding을 소유한다. Learn 문서의 Markdown/HTML 파일 접근과 프로세스 실행은
기존 `OpenVisionWorkspaceLearnDocumentService`에 남아 있다. 실제 호출 경로는
다음과 같다.

```text
OpenVisionWorkspaceSamplePickerWindow.TrySelectSample
  -> OpenVisionWorkspaceSamplePickerViewModel
  -> SamplesView / command / pair-selection state
  -> OpenVisionWorkspaceLearnDocumentService.OpenDocument (Learn 선택 시)
```

이번 slice에서는 ViewModel 내부에 `Fields`, `Constructors`, `Properties`,
`Catalog Selection and Filtering`, `Commands`, `Pair Selection and Filtering`,
`Presentation Helpers`, `Image Preview`, `Localization Helpers` region만
추가했다. 메서드·binding 이름·public contract·상태 소유자·호출 순서는 바꾸지
않았다. region marker와 빈 탐색 줄을 제거한 normalized SHA-256이 변경 전과
동일하다.

따라서 이 변경은 새 Interface/Service/Factory/partial을 만들지 않고, 처음 읽는
개발자가 한 파일 안에서 상태·명령·선택·표시 책임을 바로 찾도록 하는 P2 탐색성
개선으로 분류한다. 실제 WPF smoke는 기존
`wpf_shell_host_workspace_sample_picker` target을 재사용했다.

### Partial

현재 compiled `partial` 선언 59개는 XAML/generated/framework/native
composition과 보호된 cohesive type으로 분류된다. 별도의 manual partial
family는 PL-0013에서 concrete owner로 통합되었고, 남은 2개는 smoke 계약
문자열이다. 따라서 “partial이 남아 있다”는 사실만으로 구조 미완료로
판정하지 않는다.

### 책임과 의존성

Core/Pipeline/Recipe/Review는 concrete owner와 focused contract가 있다.
구현체 하나뿐인 영역에 Interface/AbstractBase/Factory/Provider/Manager를
추가하지 않았다. SDK/WPF 경계와 Shell composition은 여전히 큰 조합점이지만,
새 독립 lifetime·state·test seam이 입증될 때만 분리한다.

## 비동기·예외·이미지 위험

- `async void` 17개가 검색된다. WPF event/command adapter로 정당한 항목도
  있으므로 일괄 변환하지 않는다. 장시간 실행, 중복 클릭, 예외 전달이 있는
  명령부터 개별 계약으로 확인한다.
- 빈 `catch` 정규식은 5개이며, 추가 cleanup catch는 의도적인 복구 경로일
  수 있다. 빈 catch를 전부 로그로 치환하면 종료/Dispose 의미가 바뀔 수 있으므로
  실패 owner와 복구 가능성을 확인한 뒤 수정한다.
- [`VisionPipelineExecutionService.cs`](C:/Git/2D/Dev/src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineExecutionService.cs:501)은
  timeout/cancel 뒤 native worker가 끝날 때까지 결과를 drain한다. 이 설계는
  Mat 조기 Dispose를 막지만 native 함수가 영구 정지하면 호출자도 오래
  기다릴 수 있다. 현재 smoke는 `TimedOut/Canceled`와 `WorkerDrained`를
  구분하는 범위까지 검증하며, 영구 native hang은 미검증이다.

## 기능 상태 분류

| 상태 | 이번 검토에서 확인한 예 |
| --- | --- |
| 구현되었고 테스트됨 | 진입/종료 exit code, ImageSpace Borrow/TakeOwnership, atomic XML save, Pipeline drain/generation 계약, Recipe/Review focused smoke, partial closure, project cycle 0 |
| 구현되었지만 검증 부족 | 전체 WPF theme/layout/DPI/monitor/input, 실제 camera/SDK/GPU, 장시간 native hang/운영 종료, Log Panel 외부 Explorer 실행 |
| 문서에만 존재 또는 계획 | 단일 Threshold 안정 구간 검토, operator-facing “중단 요청/실제 종료” 상태 표시 |
| 폐기/미사용으로 단정하지 않음 | public/internal 타입, legacy worker, SDK/serialization 연결이 있는 파일은 caller/reflection 계약 확인 전 삭제하지 않음 |

## 유지해야 하는 기존 구조

다음은 주니어 가독성을 이유로 재구현하지 않는다.

- `ImageSpaceFrame`의 ownership API와 `ImageSpaceImageLease`
- `SerializeHelper`와 기존 Pipeline/Recipe storage format
- `VisionPipelineExecutionPlan`/`VisionPipelineExecutionService`
- Recipe/Pipeline/Review의 현재 concrete owner와 XAML/public/test 계약
- PL-0013~PL-0017에서 검증된 partial, Shell composition, Recipe dialog 경계
- 기존 contract/smoke 도구와 D: 드라이브 검증 산출물 정책

## 다음 작업 순서

현재 소스 전수검토에서 즉시 추가할 독립 production boundary는 증명되지
않았다. 따라서 다음 순서를 사용한다.

1. **Alternate WPF runtime qualification** — 125/150/175/200% DPI, alternate
   theme, one-monitor/headless 조건이 실제 제공될 때 대표 smoke를 실행한다.
   사용자 입력이나 하드웨어를 기다리지 않고, 제공되지 않은 행은
   `미검증`으로 기록한다. Recommended model: `gpt-5.5` | Reasoning effort:
   `medium`.
2. **`OpenVisionWorkspaceSamplePickerViewModel` 책임 재분리** — 이번 source와
   focused UI smoke에서 기존 owner·Learn service 경계가 확인되었으므로, 새
   adapter를 추가하지 않는다. 재개 조건은 독립 state/lifetime 또는 재현 가능한
   파일/이미지 수명 결함이다. Recommended model: `gpt-5.6-terra` |
   Reasoning effort: `high`.
3. **Pipeline Review 표시 state와 execution state의 재현 결함 조사** —
   새 stale callback/image lifetime 실패가 재현될 때만 기존 owner를 다시 연다.
   현재는 문서·focused contract 경계를 유지한다. Recommended model:
   `gpt-5.6-terra` | Reasoning effort: `high`.
4. **Release precheck/clean candidate** — PL-0012/PL-0011의 clean candidate,
   commit, tag, publication은 별도 authorization이 있어야 하며 이번 주니어
   검토에서 자동 실행하지 않는다. Recommended model: `gpt-5.6-luna` |
   Reasoning effort: `high`.

## 완료 증거

이번 slice에서 실행한 검증:

- 현재 branch/HEAD/framework/version과 worktree 상태 확인
- `src`/`tools` C#·XAML inventory, project reference count, partial declaration,
  ViewModel/code-behind signal, large-file/folder density 수집
- 기존 PL-0013~PL-0017, R16~R25 build/focused-contract evidence와 current
  handoff 대조
- source-only ImageCompare owner 재확인
- `TestDocumentationIndex.ps1`: `DocumentationIndex=PASS IndexedPaths=289
  Routes=16 RootRedirects=102`
- `Invoke-RefactorAudit.ps1 -Verify`:
  `REFACTOR_AUDIT=PASS|CSharpFiles=816|XamlFiles=60|PartialDeclarations=59|
  PartialTextMatches=2|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`
- `git diff --check` 대상 문서: whitespace error 없음; 기존 LF/CRLF conversion
  warning만 보고됨
- `dotnet build OpenVisionLab.sln --configuration Debug --no-restore -m:1`:
  경고 0개, 오류 0개.
- `dotnet build OpenVisionLab.sln --configuration Release --no-restore -m:1`:
  경고 0개, 오류 0개.
- `RunUiScreenshotSmoke.ps1 -Targets wpf_shell_host_workspace_sample_picker
  -VisibleCapture`:
  `wpf_shell_host_workspace_sample_picker=OK|check=OK|elapsed=2249ms|size=1040x742`.
  새 캡처와 monitor topology는 evidence root의 `sample-picker-ui`에 있다.
- `OpenVisionReadinessCheck` Debug/Release: readiness contract passed.
- Sample Picker region contract: `REGION_BALANCE=0`, normalized logic hash 동일
  (`b3bf3110e029044a9e341ec1f938a8aae9ac63ee7099264d248e6394c53d3528`).

이번 후속 slice는 production source에서 탐색용 region만 추가했으며 동작 토큰은
변경하지 않았다. 따라서 이 문서의 결론은 **소스 코드·Debug build·기존
focused contract·Sample Picker Runtime smoke 기준 검토 완료**다. 다만 전체
WPF theme/125·150·175·200% DPI/input matrix, 실제 camera/SDK/GPU, 장시간
native hang와 종료 회수는 여전히 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요` 범위다.

Evidence root: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-whole-repo-review-20260911`
