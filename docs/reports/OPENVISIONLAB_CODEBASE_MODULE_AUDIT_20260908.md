# OpenVisionLab 코드베이스 모듈화 전수조사 — 2026-09-08

## 조사 결론

현재 Dev 코드베이스는 **부분적으로 모듈화되어 있지만, 주니어 개발자가 전체
구조를 바로 이해할 수 있는 상태는 아니다.** Recipe/Pipeline의 실행·검증
owner, ImageCanvas의 입력·대화상자 owner, Learn topic presentation policy,
PropertyGrid 애플리케이션 정책처럼 책임을 이름으로 찾을 수 있는 영역은
좋아졌다. 반면 Shell CommandSurface와 Shell XAML은 여러 유스케이스와 상태를
한 조합 지점에 계속 보유하고 있고, 제품 앱 프로젝트와 기본 namespace도 매우
크다. 따라서 파일 수나 `partial` 수를 줄이는 작업보다 **이미 경계가 드러난
실행·리소스 owner를 먼저 독립 검증하고, 그 뒤 Shell을 vertical slice로
나누는 순서**가 안전하다.

이번 문서는 이전 `OPENVISIONLAB_REFACTOR_PROGRAM_AUDIT_20260908.md`의 단순
반복이 아니라, 현재 worktree를 같은 도구와 추가 구조 계측으로 다시 확인한
결과다. 이전 보고서의 1,317 타입 수와 이번 계측의 1,321 타입 선언 수는
조사기·dirty worktree 시점이 다르므로 직접 증감률로 해석하지 않는다. 이번
보고서의 우선순위는 이번 실행의 산출물끼리만 비교한다.

## 조사 범위와 권위

- 작업 대상: `C:\Git\2D\Dev`, branch `codex/public-sample-ux-docs`.
- 기준: `AGENTS.md`, `docs/README.md`, `docs/LLM_DOCUMENT_INDEX.json`, 현재
  Handoff, 제품 목표, 안정 기능 계약, 현재 소스와 기존 완료 기록.
- 첨부 문서 `C:\Users\USER\Downloads\OpenVisionLab_Implementation_Tasks_604c7fb.md`는
  2026-09-07의 과거 후보 명세다. 문서가 명시한 원격 SHA와 E01–E10 항목은
  현재 미완료라는 증거로 사용하지 않았다. 현재 코드에서 다시 확인된 항목만
  이 문서의 조사 결과에 반영했다.
- 기존 dirty 변경은 보존했다. 이 조사에서는 reset, clean, stage, commit,
  push, Original checkout 변경을 수행하지 않았다.

## 현재 스냅샷

조사 시점의 Git 상태는 HEAD `d875559577c85984d54900df973a6fb35fb20146`,
`git status --short` 272개 항목, staged 0개였다. dirty 상태가 포함되므로
이 수치는 다른 실행의 작업량이나 품질 점수로 사용하지 않는다.

### 정량 계측

| 영역 | 측정값 | 해석 |
| --- | ---: | --- |
| C# source | 773 files / 268,742 lines / 12,238,522 bytes | `src`와 `tools`에서 `bin`·`obj` 제외 |
| XAML | 58 files / 24,624 lines / 1,752,829 bytes | WPF view, template, resource 포함 |
| Type/method heuristic | 1,321 type declarations / 9,804 method-like declarations | 정규식 기반 탐색 신호이며 AST 의미 분석이 아님 |
| `partial` | 106 declarations / 63 partial types | 숫자 자체는 결함이 아님; 13개 type만 여러 파일에 걸침 |
| Large files | >=1,000: 28 / >=2,000: 12 / >=3,000: 8 | 분리 필요성의 조사 신호일 뿐 기준선이 아님 |
| Projects | 27 projects / 34 ProjectReferences | ProjectReference cycle 0 |
| ViewModel signals | 32 files / direct UI-or-dialog 1 / direct IO 8 | UI 1건은 `EnumItemType.Window` 오탐으로 수동 분류 |
| Event operations | 149 files with `+=`/`-=` | 개수 차이만으로 누수 판정하지 않음 |
| IO signals | 139 files | path formatting, display lookup, real file ownership을 구분해야 함 |

재현 명령:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RefactorAudit\Invoke-RefactorAudit.ps1 `
  -RepositoryRoot (Get-Location).Path `
  -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-full-module-audit-20260908 `
  -Verify
```

실행 결과:

```text
REFACTOR_AUDIT=PASS|CSharpFiles=773|XamlFiles=58|PartialDeclarations=106|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0
```

추가 구조 계측은 type, method, multi-file partial, ViewModel UI/IO, event
operation, project graph, namespace/path 분포를 읽기 전용으로 기록했다.
원본 CSV/JSON과 요약은 다음 D: evidence root에 있다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-full-module-audit-20260908`

## 구조 지도

### 프로젝트 경계

`OpenVisionLab` 앱 프로젝트 하나에 562 C# files, 48 XAML files,
171,819 lines가 들어 있다. 라이브러리 프로젝트는 다음처럼 나뉘어 있다.

| 프로젝트/영역 | C# | XAML | lines | 판단 |
| --- | ---: | ---: | ---: | --- |
| `OpenVisionLab` app | 562 | 48 | 171,819 | 제품 composition과 대부분 UI가 한 compile unit |
| `OpenVisionLab.ImageCanvas` | 78 | 4 | 12,917 | 비교적 명확한 canvas library boundary |
| `OpenVisionLab.Docking.Controls` | 49 | 3 | 5,890 | docking 책임이 별도 library에 있음 |
| `WpfPropertyGridBridge` | 1 | 0 | 4,008 | generic bridge가 단일 대형 파일에 집중 |
| `PipelineViewerScreenshotSmoke` | 9 | 0 | 43,688 | smoke runner; 제품 runtime과 분리 |
| `VisionRecipeRunnerSmoke` | 14 | 0 | 16,943 | recipe smoke runner; 제품 runtime과 분리 |

제품 앱 내부는 `UI` 436 C# / 109,269 lines, `Core` 78 C# / 31,928 lines다.
`src/OpenVisionLab` 파일 중 515개가 기본 `OpenVisionLab` namespace에 남아
있고, `OpenVisionLab.ViewModels`는 12개 파일에 그친다. 디렉터리는 책임별로
나뉘어도 namespace와 compile unit이 이를 충분히 표현하지 못해, 주니어가
파일을 찾을 때 경로·namespace·실제 호출자를 함께 확인해야 한다.

### Shell CommandSurface

`OpenVisionShellHostRecipeCommandSurface`는 10개 파일, 10,367 lines의
`partial` type이다. root/handlers/validation/recipe workspace/pipeline
lifecycle/pipeline exchange/qualified snapshots/LLM XML/run history/commands가
하나의 private state와 facade를 공유한다. root 파일만 `ICommand` property가
71개이고, qualified snapshot 파일에 7개가 더 있다. 이 수치는 화면 command
개수가 많다는 뜻이기도 하지만, 하나의 facade가 recipe CRUD, validation,
sample/pair/catalog run, LLM draft, locator evidence, step edit, run navigation
상태를 함께 projection한다는 구조 신호다.

현재 handlers에는 다음 file/XML 접근이 남아 있다.

- `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`: pipeline XML
  `File.ReadAllText`와 여러 sample/template `File.Exists`, `XmlSerializer`.
- `OpenVisionShellHostRecipeCommandSurface.LlmXmlDraftWorkflow.cs`:
  draft `File.Exists`/`File.ReadAllText`.
- `OpenVisionShellHostRecipeCommandSurface.QualifiedSnapshots.cs`:
  summary path `File.Exists`.

Run History 저장 직접 호출은 0이며 기존
`OpenVisionRecipeRunHistoryOrchestrationOwner`가 그 책임을 갖는다. 이 완료된
owner를 다시 Shell로 옮기거나 새 partial로 재분산하지 않는다. 다음 Shell
작업은 먼저 Recipe execution/session 경계를 줄인 뒤, 한 번에 하나의
validation/evidence 또는 step-edit call path만 이동해야 한다.

### Shell XAML

`OpenVisionShellHostView.xaml`은 9,035 lines다. 한 UserControl 안에 resource와
template, shell chrome/tool rail, workspace/layer context menu, log panel,
recipe manager/CRUD, guided setup, pipeline, review/report, run history, XML
step edit, LLM XML/browser assist, Preview/basic lifecycle이 함께 있다.

이 화면은 binding, `RelativeSource`, `ContextMenu`, AutomationId, keyboard,
focus, drag/drop, theme, DPI, popup 경계를 많이 공유하므로 XAML 파일을 길이로
쪼개면 안 된다. command owner가 먼저 안정된 뒤 recipe manager overview,
pipeline review/history, LLM draft 같은 **수직 흐름 단위**를 하나씩 추출하고
각각의 DataContext와 UI smoke를 확인해야 한다.

### Recipe execution session

`OpenVisionRecipeExecutionSessionViewModel`은 실행 중 상태·summary·command
state를 projection하는 facade이면서 pipeline XML `File.ReadAllText`, sample
catalog `File.Exists`, sample/pair/catalog 실행, batch summary 저장까지 수행한다.
이미 존재하는 `OpenVisionRecipeValidationSetRunner`,
`VisionPipelineSampleCheckService`, `VisionPipelineBatchRunSummaryStorage`를
재사용할 수 있는 명확한 후보이며, 새 service/factory를 먼저 만들 이유는
없다. 이 경계를 분리할 때 explicit Run/Stop, cancellation, summary path,
Recipe/XML 호환성을 focused test로 잠가야 한다.

### ImageCompare resource ownership

`ImageCompareViewModel`/`ImageCompareSlotViewModel`은 binding 상태와 함께
파일 경로 정규화, `Bitmap` 생성·교체·dispose, `BitmapImage` stream load,
PNG/BMP signature 읽기를 보유한다. `ImageCompareSlotViewModel.Dispose`가
현재 bitmap을 해제하는 것은 올바른 수명 신호지만, file decoding과 WPF
projection이 같은 type에 있어 테스트와 종료 순서를 추적하기 어렵다.
기존 N-image, format detection, last-directory, coordinate sampling 계약을
유지하면서 file/bitmap owner를 분리할 수 있는 이번 조사 기준 P1 후보다.

### PropertyGrid

`WpfPropertyGridAdapter.cs`는 4,008 lines의 generic bridge이며 metadata
attribute, WPF style/template/theme, navigation/edit commit, editor event
wiring, property visibility/registration, wrapper/comparer를 모두 포함한다.
반면 OpenVisionLab 도메인 property-name 집합은
`src/OpenVisionLab/Common/PropertyGridToolPolicy.cs`로 이동했고,
`VisionToolPropertyGridHost`가 `ChildParameterPredicate` delegate로 주입한다.
따라서 과거의 “adapter가 OpenVisionLab tool policy를 직접 안다”는 문제는
현재 코드에서 확인되지 않는다. 남은 adapter 분리는 generic visual/theme와
navigation/editor lifecycle처럼 독립 테스트 가능한 경계가 실제로 생길 때만
수행한다. public attribute, extern alias, selected-object binding 계약을
깨는 단순 파일 분할은 금지한다.

### Learn Window

`OpenVisionLearnWindow.xaml.cs` 1,069 lines와 XAML 624 lines에는 여러 topic
view/presenter, test hooks, HSV `DispatcherTimer`, window lifecycle이 있다.
현재 `TopicList_SelectionChanged -> UpdateSelectedTopic ->
OpenVisionLearnTopicPresentationPolicy.Resolve -> topic view/presenter` 경로가
명시적이다. Focus, animation, rendering, Window close는 View에 남겨도 되는
책임이다. 새 독립 state owner가 발견되지 않는 한 Learn Window를 다시 나누는
것은 우선순위가 아니다.

## 주니어 개발자 이해도 평가

| 영역 | 현재 평가 | 근거 |
| --- | --- | --- |
| Core/Recipe/Pipeline | 양호 | validation, execution, storage, review owner가 이름과 경로로 추적됨 |
| ImageCanvas 입력/대화상자 | 양호 | 이미 explicit controller/host로 이동했고 완료 기록이 있음 |
| Shell CommandSurface | 개선 필요 | 10개 partial과 많은 command/state가 한 facade에 결합됨 |
| Shell XAML | 개선 필요 | 한 화면에 서로 다른 operator workflow가 9,035 lines로 공존 |
| Recipe execution session | 개선 필요 | 상태 projection과 XML/file/batch orchestration이 동일 VM에 있음 |
| ImageCompare | 개선 필요 | WPF binding과 Bitmap/file lifetime을 같은 VM이 소유 |
| PropertyGrid | 조건부 양호 | app policy는 분리됐지만 generic bridge가 크고 복합적 |
| Learn | 양호 | topic policy와 presenter 경계가 확인됨; Window는 composition root |
| namespace/project | 개선 필요 | app compile unit 171k lines, 기본 namespace 515 files |

판정은 “전체가 잘못되었다”가 아니다. **핵심 기능 owner는 상당 부분
모듈화되었지만, 진입점·화면·일부 resource owner가 주니어가 한 번에 읽을
수 있는 모듈 경계를 아직 제공하지 않는다**가 현재 증거에 맞는 결론이다.

## 우선순위와 실행 순서

각 실행은 독립적으로 검증 가능한 작업 하나만 포함한다. 아래 순서는 새
결함·변경 계약이 발견되면 그 증거를 우선하고, 그렇지 않으면 완료된 owner를
재개하지 않는 기준이다.

1. **기준선·완료 owner 보호** — 현재 `Invoke-RefactorAudit.ps1 -Verify`를
   먼저 통과시키고, OVL-01/02/03/04/05/06a/07/08/10과 완료된 OVL-11,
   OVL-09 owner에 새 결함이 없는지 확인한다. 코드 변경 없음.
   **Recommended model: gpt-5.4-mini | Reasoning effort: low**
2. **ImageCompare 이미지·Bitmap resource owner** — 파일 decoding/Bitmap
   교체·dispose와 binding summary를 독립 owner로 분리하고 기존 ViewModel
   facade/계약을 유지한다. N-image, format, last-directory, coordinate,
   dispose 회귀를 focused test로 확인한다.
   **Recommended model: gpt-5.6-terra | Reasoning effort: high**
3. **Recipe execution session 경계** — XML/sample catalog 읽기와 batch
   orchestration을 기존 service/runner/storage 조합으로 이동하고 VM은 실행
   상태와 결과 projection을 담당하게 한다. Preview/Run, Stop, cancellation,
   Recipe/XML을 검증한다.
   **Recommended model: gpt-5.6-terra | Reasoning effort: high**
4. **Shell residual CommandSurface** — 3번 이후 남은 validation/evidence,
   LLM draft, step edit 중 한 call path만 선택해 기존 owner를 재사용한다.
   새 partial을 추가하지 않고 former owner의 책임·의존성·호출 경로가 실제로
   줄었는지 구조 검증한다.
   **Recommended model: gpt-5.6-terra | Reasoning effort: high**
5. **Shell XAML vertical slice** — command owner가 안정된 뒤 recipe manager,
   pipeline review/history, LLM draft 중 하나를 별도 View로 이동한다. binding,
   DataContext, AutomationId, ContextMenu, focus, theme, DPI, popup과 focused
   UI smoke를 함께 확인한다.
   **Recommended model: gpt-6-astra | Reasoning effort: high**
6. **PropertyGrid generic adapter internals** — 실제 독립 테스트 경계가
   확인될 때만 visual/theme와 navigation/editor lifecycle을 분리한다. 현재
   `PropertyGridToolPolicy` 분리를 다시 하지 않는다.
   **Recommended model: gpt-5.6-terra | Reasoning effort: high**
7. **namespace/project boundary migration** — `OpenVisionLab` 기본 namespace와
   171k-line app project를 바꾸는 작업은 XAML `x:Class`, reflection,
   serialization, public type, ProjectReference cycle를 inventory한 뒤
   compatibility shim과 단계별 build를 갖출 때만 실행한다.
   **Recommended model: gpt-6-astra | Reasoning effort: high**
8. **대형 smoke runner 정리** — 41,770/19,199/13,713-line 파일을 line count로
   자르지 않고 fixture preparation, runner, assertion/reporting처럼 독립된
   test responsibility가 확인될 때만 분리한다. 제품 runtime 우선순위를
   가로채지 않는다.
   **Recommended model: gpt-5.4-mini | Reasoning effort: medium**

## 완료 기준과 중복 방지 규칙

- 각 slice에는 current owner, intended owner, dependency direction, state
  owner, observable contract, focused proof를 기록한다.
- former owner에서 이동한 책임이 실제로 사라졌는지 정적 검색과 호출 경로로
  확인한다. 이름 변경, namespace 이동, `partial` 파일 추가만으로 완료하지
  않는다.
- 변경된 경계에 필요한 Debug/Release build 또는 focused unit/integration/UI
  check를 실행하고, 실행하지 못한 검증은 미실행으로 기록한다.
- Recipe/XML serialization, explicit Preview/Run, Layer/ImageSpace
  Lease/Dispose, cancellation/late-result, PropertyGrid binding 계약은
  해당 slice에서 회귀하지 않아야 한다.
- 완료된 owner는 다음 조건 중 하나가 새로 입증될 때만 다시 연다: 새 결함,
  변경된 계약, 실제 책임 충돌, 또는 기존 증거를 반박하는 재현 가능한 실패.
  “더 작게 보이게 하기”, 문서 중복, 주니어가 좋아할 것 같다는 추측은 근거가
  아니다.
- 자동 실행은 5분 주기여도 한 번에 한 slice만 수행하고, 동일 report/owner를
  다른 모델·에이전트가 반복 구현·재분할·재문서화하지 않는다. 실행 중 dirty
  변경과 충돌하면 해당 owner를 건드리지 않고 증거만 남긴다.

## 이번 조사 완료 기록

Status: Complete

Scope: 현재 Dev 소스·프로젝트·partial·namespace·ViewModel UI/IO·event·Shell·PropertyGrid·Learn 구조의 정적 전수 계측과 수동 hotspot 검토, 주니어 이해도 평가, 우선순위와 완료 기준 수립.

Acceptance criteria:

- 전체 source inventory와 project graph를 재현 가능한 D: 산출물로 남김 — PASS (`REFACTOR_AUDIT=PASS`, cycles 0).
- 기존 완료 owner와 현재 미완료 책임을 구분함 — PASS (각 영역의 현재 call path와 재개 조건 기록).
- 주니어 관점의 모듈화 판정과 단계별 순서를 정함 — PASS (부분 모듈화 판정과 8단계 순서).
- 이번 조사에서 source behavior를 변경하지 않음 — PASS (source 변경 0; 문서/인덱스/핸드오프 기록만 갱신).

Verification:

- `Invoke-RefactorAudit.ps1 -Verify` — PASS.
- 추가 module audit script — PASS; `module-audit-summary.json`, CSV inventory, project cycle output 생성.
- `git status --short`, branch/HEAD 확인 — PASS; staged 0, 기존 dirty 상태 보존.
- 전체 solution build, full unit/UI/runtime matrix — 이번 audit scope에서는 미실행. 다음 구현 slice의 focused gate에서 실행한다.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-full-module-audit-20260908`

Boundary / next dependency: 이번 문서는 구조를 진단하고 순서를 정한 결과이며
source refactor 완료를 주장하지 않는다. 다음 단일 slice는
`ImageCompareViewModel`의 file/Bitmap resource ownership 경계 검토와 focused
regression 설계다.
