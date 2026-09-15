# OpenVisionLab 2D 구조·신뢰성 검토 — 2026-09-09

Status: Complete

최종 문서/변경 보존 검증까지 마친 기록이다. 범위는 현재 Dev 코드 조사, 실제로 재현한
두 P0 수정, 기존 P1 소유권 유지 판단과 P2 탐색 경로 보강이다. 전체 리팩토링
프로그램, 모든 알고리즘의 물리 정확도 또는 장시간 현장 운영 검증을 뜻하지 않는다.

## 기준과 조사 방법

| 항목 | 이번 작업의 입력 |
| --- | --- |
| 구현 저장소 | `C:\Git\2D\Dev`; remote `https://github.com/Noah8218/OpenVisionLab_Dev.git` |
| 사용자 지정 공개 프로젝트 | `https://github.com/Noah8218/OpenVisionLab`; 공개 페이지는 식별 목적으로만 확인 |
| Branch / Commit | `codex/public-sample-ux-docs` / `65e2fd8b77989172d68d0aad81d489df94baf474` |
| 제품 Target Framework | `net8.0-windows7.0`, `WinExe`, WPF; 검증은 x64 Debug/Release |
| 제품 버전 | 기존 `AppVersion.VERSION = 2.1.0`; 수정하지 않음 |
| SDK | vendored Vision SDK `3.0.0`, manifest commit `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b` |
| 시작 상태 | 기존 dirty 314개; 기존 변경을 입력으로 보존, commit/push/Original 변경 없음 |
| 증거 root | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\architecture-reliability-20260909` |

`.sln`, 27개 `.csproj`의 Framework/OutputType/ProjectReference, 공통 props,
제품 `.csproj.user`, Program/Bootstrap, 실제 Core·View·ViewModel·SDK 호출부,
focused tests와 공개 sample XML을 확인했다. 공유 `launchSettings.json`이나
`.slnLaunch`는 발견하지 못했고 제품 `.csproj.user`에도 startup override가 없다.
따라서 사용자별 Visual Studio startup 선택은 검증된 설정으로 주장하지 않는다.
실행 방법은 기존 [README Start Here](../../README.md#start-here-choose-the-project)를 사용한다.

기존 정량 감사 도구로 전체 `src`/`tools` 파일과 결합 신호를 조사하고 주요
호출 경로를 직접 읽었다. 276,378줄 전부의 의미나 모든 실행 분기를 수동 검증한
것은 아니다. 시작 계측은 C# 837개, XAML 60개, partial 선언 110개, 3,000줄
이상 파일 8개, Shell CommandSurface 10파일/10,103줄이다. 27개 프로젝트의
34개 ProjectReference에서 순환은 없었다. 이 수치는 결함 개수가 아니다.

## 1. 현재 Architecture

제품은 **OpenCvSharp 규칙형 Recipe 워크벤치**다. Image/Layer에서 Tool을
가르치고 명시적으로 검사한 뒤 Pipeline/Recipe와 결과 증거를 재사용한다.
현재 상태는 핵심 경계와 집중 회귀 계약이 갖춰진 RC/pre-production 수준으로
판단한다. 완성률 백분율이나 현장 qualification을 추정하지 않는다.

| 경계 | 실제 소유자와 호출/상태 |
| --- | --- |
| 시작·조합 | `Program.Main -> OpenVisionLabApplication.Run -> ApplicationRuntimeContext -> OpenVisionShellHostWindow`. 기본 runtime context와 DisplayManager는 앱 수명의 공유 instance이며 순수 domain 객체가 아님 |
| Image | `CanvasImageLoader`, `BitmapImageConverter`; Canvas VM은 현재 Mat clone을 보유하고 교체/clear에서 이전 Mat을 해제 |
| Layer | `DisplayManagerService -> DisplayLayerStore`는 제목/선택/목록, `ImageSpaceService`는 이미지/ROI와 Lease 수명을 소유 |
| Tool | `OpenVisionNativeToolRegistry -> NativeToolDocument -> ViewModel/PropertyGrid/controller`; parameter XML은 기존 `OpenCvPropertyBase` 경로 재사용 |
| Inspection | `VisionPipelineAppToolFactory`가 SDK Tool 또는 기존 앱 전용 검사 adapter를 만들고 `ExecuteStep`가 실행·결과 수집·Tool Dispose를 담당 |
| Pipeline | `VisionPipelineExecutionPlan`이 원본/effective 정의와 정규화 정보를 보존. `VisionPipelineExecutionService`가 Step 순서, layer routing, fixture, acceptance와 timeout/drain을 실행 |
| Recipe | `RecipeState -> RecipeRuntimeStorage -> VisionToolRepository/VisionToolStorage + RecipeDataStorage`; Pipeline XML·active pointer는 `VisionPipelineStorage` |
| Result | 실행별 `VisionPipelineRunResult`, `VisionRecipeRunResult`, Step summary/report. matching 부가 결과 저장은 `ConditionalWeakTable<VisionToolResult,...>`로 실행 결과에 결합되어 있음 |
| Review | `OpenVisionPipelineReviewExecutionController`가 실행 stamp와 review image cache, Document revision gate가 현재 문서에 대한 투영, ViewModel/Presenter가 표시를 소유 |

프로젝트 구성은 제품 1개, 라이브러리 12개, tools 14개다. Logging은
`netstandard2.0`, Readiness/LocalizationCatalogCheck/UserManualBuilder는
`net8.0`, 나머지는 `net8.0-windows7.0`이다. 제품은 12개 라이브러리를
참조하고, Display.Core는 ImageSpace.Core, History와 WpfPropertyGridBridge는
PropertyGrid.Abstractions, UI controls는 해당 Core/Localization을 참조한다.
실제 34개 edge는 증거 root의 `survey/source-survey.json`에 있다.
Vision SDK는 이 solution의 source project가 아니라 DLL 참조다.

`Core` 폴더 전체를 독립적으로 떼어 쓸 수 있는 package로 표현하면 안 된다.
일부 Tool parameter는 WPF PropertyGrid metadata를 갖고, AppPathService와
기본 runtime context에는 process/static 수명 경계가 남아 있다. SDK 내부
소스와의 알고리즘 중복 전수 비교나 host 없는 Core build는 이번에 수행하지 않았다.

### 기능 상태 분류

| 상태 | 실제 확인한 범위 | 증거의 한계 |
| --- | --- | --- |
| 구현 완료 | Native Tool 등록, PropertyGrid 기반 Tool Views, Preview/Run, Learn, Pipeline/Recipe/Review의 기존 owner와 UI가 존재 | 기존 완료 기록 및 소스 확인. 모든 Tool의 현재 UI 실행을 뜻하지 않음 |
| 구현되었고 테스트됨 | 저장 실패 전달, callback 실패 후 Mat 해제, Recipe load 실패 복구, Tool XML rollback, Recipe 경로, Step Edit 저장/복원, Review 실행/취소/drain/stale callback, ImageSpace snapshot | 이번 Debug/Release focused contract에 한정 |
| 구현되었지만 검증 부족 | Recipe Data/Pipeline까지 포함한 전체 process-crash transaction, 모든 Tool의 수치/물리 정확도, 전체 UI theme/DPI, 장시간 운전 | 코드/기존 기록과 이번 실행 증거를 구분해야 함 |
| 문서에만 존재 | 이번 추적 범위에서 소스 부재를 확정한 사용자 기능 없음 | 요구사항·검증 계획을 구현 수에 넣지 않음 |
| 계획 기능/검증 | 기존 roadmap의 deferred LLM Phase 3 및 미실행 UI/현장 qualification | 선택적 LLM은 유지보수 모드. 신규 알고리즘·provider·카메라/PLC 플랫폼 개발은 이번 범위 밖 |
| 폐기 또는 미사용 코드 | `BackgroundLoopWorker`는 `src`/`tools` C# 참조 검색에서 선언 외 사용처 없음 | public type이므로 외부 binary/reflection 계약까지 부재라고 단정하지 않고 유지 |

실제 sample `docs/samples/public/Public_Threshold_BandPads.pipeline.xml`의
`Threshold -> Contour`, `Main -> Threshold_Binary -> Threshold_Preview`,
`ResultCount 4..4` gate를 읽었다. 이것은 XML과 routing의 구현 확인이며 이번
작업에서 그 sample의 검출 정확도를 새로 검증했다고 주장하지 않는다.

## 2. 잘 설계된 부분

- Layer metadata와 이미지 수명을 구분하고, Lease가 기존 이미지 교체 뒤에도
  snapshot을 안전하게 만들 수 있도록 한다. 복사 횟수/추정 bytes 계측도 존재한다.
- Recipe XML은 `SerializeHelper`의 임시 파일/replace/finally 경로를 재사용한다.
  Pipeline rename/delete에는 journal과 복구 경로가 이미 있다.
- 실행 계획과 실행 결과가 분리되어 있고 acceptance·fixture·metrics가 명시적이다.
  Review는 run/input/recipe revision과 취소 후 drain을 가진다.
- Step Edit는 기존 XML을 읽은 뒤 apply/save/round-trip 및 복원을 수행하는
  concrete owner를 갖는다. 추가 factory/interface가 필요한 근거는 발견하지 못했다.
- 상용 도구에서 유지할 교훈은 단계별 입출력·ROI·판정 근거의 추적성과 명시적 실행이다.

## 3. 유지한 기존 코드와 수명

`VisionToolRepository`, `SerializeHelper`, `VisionPipelineStorage`, SDK factory,
ImageSpace Lease, Review controller/revision gate, Step Edit owner를 재사용했다.
생산용 class/interface/partial은 추가하지 않았다. 완료된 OVL-01~53의 경계를
파일 크기만으로 다시 분할하지 않았다.

`BitmapImageConverter`는 LockBits/stride/channel 검증과 예외 시 할당 Mat
해제를 갖는다. WPF preview factory는 최대 1024 크기로 제한하고 생성한
BitmapSource를 Freeze하며 HBITMAP은 finally에서 DeleteObject한다.
`byte[]`는 XML/hash 또는 변환 자료, `IntPtr`는 bitmap/native 경계다.
모든 복사를 제거할 대상이라고 보지 않았다. Canvas의 현재 이미지 clone,
작업 snapshot과 review cache는 소유 수명을 분리하는 목적이 있다. 이번 결과는
장시간 메모리 상한이나 전체 native allocation의 무누수를 증명하지 않는다.

## 4. 이번에 수정한 부분

### P0-A: Tool 저장 실패의 성공 오보고

변경 파일: `Core/Recipe/RecipeRuntimeStorage.cs`, `Core/State/RecipeState.cs`
(`src/OpenVisionLab/` 기준), 테스트 `tools/VisionRecipeRunnerSmoke/RecipeSaveFailureContract.cs`.

- 기존: `VisionToolRepository.SaveTools`의 false를 무시하고 Data를 저장한 뒤
  `RecipeState.SaveTools`가 true를 반환했다. Shell의 기존 실패 표시 branch에
  도달하지 않아 Tool XML이 저장되지 않아도 저장 성공으로 전달됐다.
- 변경: 기존 bool을 RuntimeStorage와 RecipeState를 통해 그대로 전달한다.
  Tool 저장이 실패하면 Data 접근/저장 전에 false를 반환한다. Data 저장 예외의
  기존 throw 동작, XML 내용/기본값/순서는 유지한다.
- 상태 owner: Tool 설정/LastStorageError는 Repository, 순서는 RuntimeStorage,
  사용자 표시 선택은 기존 `SaveSelectedRecipe`다. FileStream 수명은 테스트의
  using, 제품 XML temp/replace 수명은 기존 SerializeHelper다.
- 테스트: 실제 XML 파일을 `FileShare.Read`로 열어 atomic replace를 막는다.
  기준선 `passed=5/failed=3`, 수정 후 Debug/Release `passed=8/failed=0`.
  실패 전달, Data 저장 중단, XML bytes 유지, 임시 파일 제거, 잠금 해제 후
  재시도 및 Data 저장 예외 전파를 확인했다.
- 위험/경계: 이 저장 경계는 Tool XML 집합까지다. 뒤이어 저장되는 Recipe Data
  XML이나 Pipeline 파일까지 하나의 process-crash transaction으로 묶지는 않는다.

### P0-B: callback 예외 후 실행 결과 Mat 해제 누락

변경 파일: `Core/Pipeline/Execution/VisionPipelineExecutionService.cs`, 테스트
`tools/VisionRecipeRunnerSmoke/PipelineFailureLifetimeContract.cs`.

- 기존: `RunPreparedAsync` 내부 callback이 예외를 던지면 결과가 호출자에게
  반환되지 않아 호출자 finally도 누적 Step 결과를 해제하지 못했다. 입력 Mat
  해제도 정상/오류 branch의 여러 수동 Dispose에 의존했다.
- 변경: 같은 service의 `RunPreparedAsync`가 결과의 반환 전 수명을 소유한다.
  정상 완료는 기존 호출자에게 결과를 넘기고, 실패는 누적 ToolResult를 해제한
  뒤 원래 예외를 다시 던진다. 기존 Step traversal은 private
  `RunPreparedStepsAsync`, 입력 Mat은 Step별 using이 소유한다.
- 호출 경로: `VisionRecipeRunner` 또는 Review execution controller ->
  `RunPreparedAsync -> RunPreparedStepsAsync -> ExecuteStep/SDK -> callback`.
  성공 결과는 기존 caller가 해제하고, 실패 결과는 service가 해제한다.
  상태/write owner인 실행 context, layer routing, fixture map은 바꾸지 않았다.
- 테스트: 현재 결과 callback과 다음 Step RUN callback에서 같은 예외를 주입했다.
  기준선 `passed=5/failed=2`, 수정 후 Debug/Release `passed=7/failed=0`.
  현재/이전 결과 해제, 예외 identity, 호출자 source 생존과 정상 반환 시 live
  result 소유권 전달을 확인했다.
- 위험/경계: callback/예상치 못한 실패 시 결정적 해제의 수정이다. native worker
  무한 hang의 강제 종료나 메모리 성능 개선을 주장하지 않는다. timeout/cancel은
  기존 worker drain을 유지한다. UI style, binding 이름, gate, SDK는 변경하지 않았다.

### P0-C: Recipe Load 실패 시 이전 상태 보존

변경 파일: `Core/Recipe/RecipeRuntimeStorage.cs`, `Core/Recipe/VisionToolStorage.cs`,
`Core/Recipe/VisionToolRepository.cs`, `Core/State/RecipeState.cs` 및
`tools/VisionRecipeRunnerSmoke/RecipeLoadRecoveryContract.cs`.

- 기존: Tool load 실패가 RuntimeStorage에서 무시됐고, load 전에 repository
  collection과 Recipe identity/Data가 바뀌었다. 후반 XML 오류 뒤 old/new 상태가
  섞이고 Recipe 변경 이벤트도 발행될 수 있었다.
- 변경: VisionToolStorage는 임시 repository에 전체 Tool set을 읽은 뒤 성공할 때만
  기존 repository list를 유지한 채 교체한다. RuntimeStorage는 실패를 반환하고,
  RecipeState는 이전 Name/Model/Data/event를 보존한다. 실패한 임시 repository의
  Matching/EdgeBasedMatching/Feature 템플릿 Mat도 정리한다. Shell은 callback 뒤
  현재 Recipe 이름을 확인해 실패를 성공 선택으로 표시하지 않는다.
- 테스트: 손상된 `Contour_1.xml` 경로를 directory로 만들어 IOException을
  재현했다. 수정 후 Debug/Release `recipe-load-recovery` 각 8/8 통과,
  복구 후 재시도도 확인했다.

### P0-D: 후반 Tool XML 저장 rollback

변경 파일: `Core/Recipe/VisionToolStorage.cs` 및
`tools/VisionRecipeRunnerSmoke/RecipeMultiFileSaveRecoveryContract.cs`.

- 기존: Blob 등 앞선 Tool XML이 기록된 뒤 Contour 파일 교체가 실패하면
  Recipe 전체가 부분 저장 상태가 됐다.
- 변경: 기존 concrete storage owner가 VISION 파일 snapshot을 만들고, 저장 실패
  시 새 파일을 제거하고 변경된 기존 파일을 원래 bytes로 복원한 뒤 원래 예외를
  전달한다. 성공 시 snapshot은 삭제한다.
- 테스트: 후반 Contour XML을 `FileShare.Read`로 잠가 실제 atomic replace 실패를
  만들었다. 수정 후 앞선 Blob bytes 복원, Data 저장 중단, 임시/backup 잔존 없음,
  잠금 해제 후 재시도를 Debug/Release 각 7/7로 확인했다.

### P1: Shell residual validation/Step Edit audit

변경 파일: `OpenVisionShellHostRecipeCommandSurface.Handlers.cs` 및 상세 보고서
`OPENVISIONLAB_SHELL_VALIDATION_STEP_EDIT_AUDIT_20260909.md`.

Validation document/selection/evidence와 Step preview/load/apply/session/projection
owner의 실제 호출 경로를 다시 확인했다. 기존 owner가 책임을 단독 소유하므로
추가 분리나 interface는 만들지 않았다. Recipe load failure가 발생했을 때
Shell의 성공 메시지를 차단하는 guard만 추가했다.

두 계약의 CLI 선택과 사용법만 기존 `tools/VisionRecipeRunnerSmoke/Program.cs`에
추가했다. 두 테스트 class는 기존 거대 runner에 검사 본문을 더 쌓지 않도록 하는
독립 회귀 검사 단위이며, 제품 abstraction이 아니다.

### P2: BackgroundLoopWorker 예외 관측

변경 파일: `src/OpenVisionLab/Common/BackgroundLoopWorker.cs`.

기존 `BackgroundLoopWorker`는 public type이지만 현재 저장소 내부 caller가 없어서
삭제하지 않았다. 다만 작업 예외를 빈 `catch`로 버려 장시간 실행 중 원인을 추적할
수 없었다. 기존 `Start`/`Stop`/`Dispose`와 cancellation 동작은 유지하고, 예외
branch에서 worker 이름과 예외 전체를 `OVLog`의 System/Error context로 기록하도록
최소 변경했다. public API나 외부 caller를 가정하지 않았다.

검증은 source contract에서 빈 exception catch 제거와 worker 이름 포함 로그를 확인하고,
D: 임시 console smoke에서 의도한 예외를 발생시켜 worker가 종료되며
`BackgroundLoopWorker 'background-loop-contract' failed`가 ALL/Error log에 남는지
확인했다. Debug/Release solution·Runner build와 Readiness도 다시 실행했다.

### P1/P2 판단

P0와 같은 owner에서 수명 책임을 명확히 했다. 이후 Step Edit와 Review의
기존 owner를 실제 호출과 focused tests로 확인했으며 다른 class 추출을 강행하지
않았다. P2는 기존 `CODEBASE_STRUCTURE.md`에 아래 흐름의 읽기/소유권 표를
연결하는 작업으로 제한했다. 별도 Start Here 문서는 만들지 않았다.

## 5. 삭제한 중복/Dead Code

- Pipeline의 분기별 입력 Mat 수동 Dispose 5곳을 Step using 하나로 대체했다.
- RecipeState의 무조건 true 반환을 실제 저장 결과로 대체했다.
- production file/type 삭제는 없다. `BackgroundLoopWorker`는 사용처가 없다는
  소스 신호만으로 public 계약을 삭제하지 않았고, 빈 예외 catch만 context-rich
  error log로 바꿨다.

## 6. 남은 기술 부채와 TODO

| 우선순위 | 근거와 다음 검증 | 현재 판정 |
| --- | --- | --- |
| P0 | `RecipeRuntimeStorage.Load`의 실패 전달, `VisionToolStorage.Load`의 기존 collection reset, `RecipeState.Name`의 선행 identity 변경을 고쳤다. 실패 시 previous identity/Data/Tool collection/event를 보존하고 복구 후 재시도한다 | Debug/Release `recipe-load-recovery` 각 8/8 통과 |
| P0 | `VisionToolStorage.Save`의 Tool XML 파일별 순차 저장을 기존 owner 내부 snapshot/rollback으로 감쌌다. 후반 파일 잠금에서 앞선 XML을 원래 bytes로 복구한다 | Debug/Release `recipe-multi-file-save-recovery` 각 7/7 통과. Data XML과 Pipeline을 포함한 전체 recipe/process crash transaction은 별도 경계 |
| P1 | Shell validation/Step Edit owner를 다시 감사했다. Validation document/selection/evidence와 Step preview/load/apply/session/projection은 기존 concrete owner가 단독 소유하며, Shell은 orchestration/projection만 남긴다. Recipe switch 실패 성공 표시 guard만 추가했다 | `OPENVISIONLAB_SHELL_VALIDATION_STEP_EDIT_AUDIT_20260909.md`, owner call-path static evidence, Debug/Release focused contracts |
| P1 | AppPathService/default runtime static, WPF metadata를 가진 parameter, 앱 전용 pipeline adapter | Core 전체의 portable package 주장 불가 |
| P2 | 미사용 public `BackgroundLoopWorker` | 빈 catch는 worker 이름·예외를 System/Error log로 기록하도록 수정했다. public type 삭제는 외부 계약/실제 caller를 확인한 뒤 결정 |
| 검증 부채 | 실제 UI의 theme/layout/DPI/pointer/keyboard 행렬, 대용량·장시간 native memory/GDI 운전, 물리 측정 정확도 | 이번 source/contract 실행으로 대체하지 않음 |

저장·복구 조사는 이 표가 현재 durable 기록이다. PL-0013 등록은 기존
`PL-0010.json`에 종료 상태에서 허용되지 않는 `state.next_action`이 있어 CLI가
거부했다. 전체 원장 validator는 이로 인한 `PL-0011#M4` 참조 오류도 보고했다.
기존 원장은 수정하지 않았으며 **PL-0013은 등록된 이슈가 아니다**. 입력 초안은
증거 root의 `PL-0013-input.json`에만 있다.

## 7. 처음 읽는 개발자의 추천 순서

기존 README Start Here -> `CODEBASE_STRUCTURE.md`의 **Image → Layer → Tool →
Inspection → Pipeline → Recipe → Result → Review** 표를 단일 경로로 사용한다.

디버깅 예: 저장 오류는 `SaveSelectedRecipe -> RecipeState.SaveTools ->
RecipeRuntimeStorage.Save -> VisionToolRepository.SaveTools -> SerializeHelper`.
실행 오류는 `VisionRecipeRunner/Review controller -> RunPreparedAsync ->
RunPreparedStepsAsync -> ExecuteStep -> SDK`, 그 뒤 summary/report/Review를 읽는다.
각 경로에서 callback/state/lifetime owner를 먼저 확인한 뒤 View를 수정한다.

## 8. 다음 개발 우선순위

1. 실제 제공되는 DPI/운영 이미지/운전 시간 조건에서 UI·장시간 수명 검증. 해당 환경과 데이터가 먼저 필요하며 확보 전 모델 실행을 권하지 않는다.
2. `AppPathService`/기본 runtime static과 WPF metadata를 가진 parameter의 경계는 source-only 기술 부채로 유지한다. 실제 host 분리 요구나 재현 결함이 생길 때만 다음 owner를 정한다 | Recommended model: `gpt-5.6-terra` | Reasoning effort: `high`.
3. public `BackgroundLoopWorker`의 외부 binary/reflection 계약과 실제 caller를 확인한 뒤 유지 또는 삭제를 결정한다. 예외 관측 logging은 완료했다 | Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

## 실행한 검증과 종료 경계

증거 root 안에서 `TEMP`/`TMP`를 사용했고 기존 build output의 D: junction을
확인했다. desktop EXE를 띄우지 않은 console/in-process 계약 검사다.

```powershell
dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
# 동일 명령의 -c Release도 실행
dotnet tools\VisionRecipeRunnerSmoke\bin\x64\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll --recipe-save-failure-contract <D: evidence>
dotnet tools\VisionRecipeRunnerSmoke\bin\x64\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll --pipeline-failure-lifetime-contract <D: evidence>
# 두 configuration에서 추가 실행:
# --recipe-storage-path-contract, --step-edit-apply-owner-contract,
# --pipeline-review-execution-contract, --pipeline-review-stale-callback-contract,
# --image-space-snapshot-contract
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\2D\Dev
dotnet build OpenVisionLab.sln -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build OpenVisionLab.sln -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

최종 기능 변경의 solution·앱·runner Debug/Release build는 각각 경고 0/오류
0이었다. Readiness도 Debug/Release 모두 통과했다.
9개 focused target이 각각 두 configuration에서 통과했고 Readiness도 통과했다.
로그는 증거 root의 `build-final-solution-*-2.log`, `build-final-runner-*-2.log`,
`debug-final-*/stdout.log`, `release-final-*/stdout.log`, `readiness-final*.log`에 있다.

마지막 build와 두 새 계약도 Debug/Release 각각 경고 0/오류 0, 8/8 및 7/7로
통과했다. 최종 정량 감사는 `CSharpFiles=841`, `PartialDeclarations=110`,
`ProjectCycles=0`, `ShellStorageCalls=0`으로 통과했다(`audit-final-3/audit-final.log`).
문서 검사는 `DocumentationIndex=PASS IndexedPaths=274 Routes=14 RootRedirects=102`
(`documentation-index-final.log`), `git diff --check`도 종료 코드 0이었다
(`git-diff-check-final.log`).
BackgroundLoopWorker follow-up은 source contract와 D: runtime smoke가 통과했고,
worker failure log에 이름과 예외가 기록되었다
(`background-loop-worker-20260910`).
`preservation.txt`는 기존 tracked diff 중 이번 11개 대상 밖의 내용이 같고,
기존 status entry가 모두 남았음을 기록한다(314 -> 319). 이후 추가된 다섯
개 예상 항목(`preservation-final.txt`: 319 -> 324)도 비교해 삭제된 기존
항목이 없음을 확인했다. 시작 시 untracked 파일의 bytes hash는 수집하지
않았으므로 그 파일들까지 독립적인 byte 동일성 검증을 했다고 주장하지 않는다.

첫 기준선 build 중 테스트를 너무 일찍 호출한 1회는 이전 binary가 새 옵션을
인식하지 못했다. 그 실행은 증거에서 제외했다(`save-before.log`). 이 겹침으로
기준선 build에 일시적 DLL 복사 재시도 경고 2개가 있었으며, 이후 build/test는
순차 실행했다. 유효한 실패 기준선은 `save-before-current-build/`와
`lifetime-before/`다. 문제를 숨기기 위해 실패 로그를 삭제하지 않았다.

이번에 실행하지 않은 것: 무인 전체 suite, 전체 solution의 보조 GUI tool 실행,
실제 Shell EXE before/after screenshot, theme/layout/DPI 행렬, 장시간 soak,
SDK 내부 source audit, 현장 calibration/알고리즘 검출 품질 재검증.
UI 소스 결론은 **소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요**다.

Acceptance criteria: 두 재현 P0의 수정과 새/기존 집중 계약 통과, production
type/SDK/XML/binding 계약 유지, 현재 owner와 caller/상태/해제 탐색 경로 기록.
Evidence: 위 D: logs/contracts와 이 보고서, `CODEBASE_STRUCTURE.md`.
Boundary: 이번에 실제 검증한 경계에 한정하며 표에 남긴 기술 부채를 완료로
취급하지 않는다. 완료된 두 수정은 새 재현 또는 요구 변경 없이 다시 분리하지 않는다.
