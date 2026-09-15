# OVL-05 실행 세대·취소·종료 수명 관리

작성일: 2026-09-07 KST  
상태: **Complete** (OVL-05 구현 slice와 지정 집중 검증 gate 통과)

## 범위

첨부 GPT Pro 분석의 OVL-05에서 Review 실행 세대, 취소·timeout 판정,
worker drain, TCP controller 종료, Recipe command의 Task 내부 경계를 구현했다.
첨부 문서의 구현·테스트 목록은 작업 기준으로 사용했으며, 이 보고서는 이번
Dev worktree에서 실제로 변경하고 확인한 범위만 기록한다.

변경 파일:

- `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineExecutionService.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionResult.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.Events.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/Integration/OpenVisionTcpIntegrationController.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`
- `src/Libraries/OpenVisionLab.Localization/Resources/LocalizationCatalog.tsv`
- `tools/VisionRecipeRunnerSmoke/Program.cs`
- `tools/PipelineViewerScreenshotSmoke/Program.cs` — Recipe dependency assertion을 제품의
  `VisionPipelineAppToolFactory.ResolveTemplatePath` 계약에 맞추고, 도킹 Review root
  탐색을 `GetActiveToolVisualRoot`로 정렬했다.

## 구조 증거

Review 실행 경로는 다음과 같이 바뀌었다.

```text
기존: Document → Controller → CancellationToken.None 실행
      → Reset/Dispose 뒤 callback·cache 적용 가능

변경: Document(inputRevision, recipeRevision)
       → Controller(ExecutionStamp: RunId + 두 revision + CancellationToken)
       → ExecutionService(Completed / TimedOut / Canceled + worker drain)
       → 유효한 stamp만 UI summary·output cache에 적용
```

`Reset`은 실행 세대를 증가시키고 진행 중인 실행을 취소한다. 이미 큐에 들어간
Step/completion callback은 stamp 검사를 통과하지 못하므로 summary나 Review
cache를 다시 만들 수 없다. 실행계획 생성도 실행 수명 영역 안에서 수행해 예외가
나도 `IsRunning`이 남지 않는다. `Dispose`는 활성 worker가 끝나기 전에 native
결과와 cache를 해제하지 않으며, `DisposeAsync`는 해당 drain 이후에만 반환한다.
실행이 끝나지 않는 비협조 worker에 대해 유한 시간 종료를 보장한다고 주장하지
않는다. UI에는 이 동안 `종료 대기` 상태가 표시된다.

`VisionPipelineExecutionService`는 timeout/cancel 판정과 worker drain을 하나의
bool로 합치지 않고 `VisionPipelineStepCompletion`으로 분리한다. timeout 또는
cancel 뒤 늦게 도착한 `VisionToolResult.ResultImage`는 drain 과정에서 해제하며,
worker 예외는 drain 진단으로 남긴다. 정상 완료의 예외 전파 동작과 기존 결과
수명은 유지했다.

TCP controller는 `disposalSync`에서 active operation 예약과 `Dispose` 관찰을
직렬화한다. `Dispose`는 UI thread에서 `DisposeAsync().GetResult`로 기다리지
않고, active operation 취소·창 닫기·상태 정리만 수행한다. 교환 객체의
`StopAsync`/`DisposeAsync`와 cancellation source 해제는 operation task가 끝난
뒤 `DisposeExchangeAfterOperationAsync`가 담당한다.

Recipe command surface의 사용자 이벤트·RelayCommand 경계는 `async void`로
남겨 두되, 선택·검증·샘플 pair·catalog 실행과 recipe 생성의 실제 작업은
`Task` 반환 내부 메서드로 분리했다. `async void` 검색 결과는 이 명시적 UI
경계와 기존 TCP/Review 이벤트 경계에 한정된다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| 실행 A의 결과가 입력/Recipe B에 적용되지 않음 | 통과 | `--pipeline-review-execution-contract`에서 RunId·revision stamp와 queued callback을 교차 검사 |
| Reset 뒤 stale callback이 summary/cache를 재생성하지 않음 | 통과 | generation guard 계약의 summary 없음 및 `Review_Output` cache 없음 검사 |
| 실행계획 생성 예외 뒤 `IsRunning` 원복 | 통과 | invalid XML 계획 생성 예외 후 controller 상태 검사 |
| timeout과 cancel이 worker drain 전 조기 반환하지 않음 | 통과 | timeout/cancel 상태를 각각 검사하고 `WorkerDrained` 및 늦은 image Dispose 확인 |
| 중복 Review 실행 차단 | 통과 | 첫 실행이 dispatcher 경계에서 대기하는 동안 두 번째 실행이 `InvalidOperationException`으로 거부됨 |
| Review `DisposeAsync`가 실행 종료와 cache 수명을 기다림 | 통과 | 완료 실행 후 async disposal 및 no-active-run 검사 |
| TCP 처리 중 종료가 UI thread를 동기 대기시키지 않음 | 통과 | 실제 loopback listening exchange를 시작한 뒤 `DisposeAsync`로 stop/dispose 및 `IsListening=false` 확인 |
| Recipe 내부 작업이 Task 반환 경계를 가짐 | 통과 | `SelectRecipeAsync`, validation/sample/pair/catalog 및 `CreateAndSwitchRecipeAsync` 구조 검색과 Release 빌드 |
| dispatcher 종료 뒤 queued Review callback이 view를 만지지 않음 | 소스·빌드 확인 | disposed/shutdown guard와 dispatcher 예외 필터를 추가했고 Review 도킹 화면을 최신 x64 smoke에서 확인함 |
| 비협조 native worker의 유한 시간 강제 종료 | 주장하지 않음 | 자원 소유권을 위해 worker drain을 유지하며 `Thread.Abort`나 별도 process runner는 추가하지 않음 |

## 검증

모든 테스트 출력과 evidence는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl05-review-20260907`에 저장했다. 기존 Dev worktree 변경은 유지했고 `C:\Git\2D\Original`은 수정하지 않았다.

- [OpenVisionLab Release build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/build-openvisionlab-release-ovl05-final.log)
- [OpenVisionLab Debug build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/build-openvisionlab-debug-ovl05-final.log)
- [VisionRecipeRunnerSmoke Release build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/build-runner-release-ovl05-final.log)
- [PipelineViewerScreenshotSmoke Debug build, 0 warnings/0 errors](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/build-screenshot-debug-ovl05-final.log)
- [OVL-05 Review execution contract (Debug)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/pipeline-review-execution-contract/pipeline-review-execution-contract.txt)
- [OVL-05 Review execution contract (Release)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/pipeline-review-execution-contract-release/pipeline-review-execution-contract.txt)
- [OVL-05 TCP disposal contract (Debug)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/tcp-controller-disposal-contract/tcp-controller-disposal-contract.txt)
- [OVL-05 TCP disposal contract (Release)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/tcp-controller-disposal-contract-release/tcp-controller-disposal-contract.txt)
- [History contract (Release)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/history-contract/history-contract-release.log)
- [WPF explicit x64 Review/Preview/NG/input/Recipe smoke with monitor record](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-final-explicit-all-run.log)
- [Pipeline Review screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-final-explicit-all/wpf_shell_host_pipeline_review.png)
- [Native Preview screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-final-explicit-all/wpf_preprocess_output_preview_flow.png)
- [Review NG-state screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-final-explicit-all/wpf_shell_host_pipeline_review_ng.png)
- [Review missing-input screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-final-explicit-all/wpf_shell_host_pipeline_review_input_state.png)
- [Recipe language/import screenshot](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-final-explicit-all/wpf_shell_host_recipe_language_controls.png)
- [문서 색인 검증](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/documentation-index-ovl05-final.log)
- [정적 구조·동기 disposal 검색](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/git-diff-and-static-check-ovl05.log)
- [git diff --check](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/git-diff-check-ovl05.log): whitespace 오류 없음 (기존 LF→CRLF 안내만 발생)
- [입력 상태 target 하네스 보정 전 실패 로그(역사 evidence)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-review-input-rerun.log)
- [Recipe broad target raw 경로 assertion 실패 로그(역사 evidence)](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl05-review-20260907/wpf-recipe-fixed-run.log)

기존 dirty worktree에는 파일별·구간별 혼합 line ending이 이미 있어 이를
전역 정규화하지 않았다. 정규화는 OVL-05 동작 변경과 분리된 별도 작업으로 둔다.

WPF smoke는 실행 직전에 확인한 단일 `DISPLAY1` (Bounds 1920×1080,
WorkingArea 1920×1032)에서 수행했고, Review/Preview/NG 화면의 실제 캡처를
확인했다. 이번 변경은 실행 수명과 상태 텍스트에 한정되며 XAML style/template를
추가하지 않았으므로 지원 theme·Wide/Compact·100/125/150/175/200% DPI와
pressed/focus/popup 전수 행렬은 실행하지 않았다.

## 경계와 남은 위험

최신 명시적 x64 실행에서 `wpf_shell_host_pipeline_review`,
`wpf_preprocess_output_preview_flow`, `wpf_shell_host_pipeline_review_ng`,
`wpf_shell_host_pipeline_review_input_state` 네 target이 모두 `OK`였고 실제
캡처에서 텍스트·레이아웃·입력 누락 상태를 확인했다. 입력 상태의 이전 실패는
도킹 가능한 Review를 floating window로만 찾던 하네스 전제였으며
`GetActiveToolVisualRoot`로 정렬한 뒤 통과했다.

`wpf_shell_host_recipe_language_controls`도 resolver-aligned assertion으로
재실행해 `OK`였다. 제품은 `RECIPE\\...` 데이터 상대 경로를
`AppPathService.ResolveExistingDataOrInstallationPath`로 해석하므로, raw
`File.Exists` 검사를 동일 resolver를 거치는 확인으로 보정했다. 보정 전
실패 로그는 역사 evidence로 남겼다.

전용 dispatcher 종료 EXE target은 실행하지 않았다. 대신 document의
disposed/shutdown guard, TCP callback guard, Release/Debug 빌드, Review 실행
계약과 실제 x64 Review 화면 종료 경계를 확인했다. 이 범위는 dispatcher의
모든 DPI/theme 조합이나 비협조 native worker의 강제 종료를 증명하지 않는다.

이번 slice는 강제 process 종료, native worker 협조성, peak memory, 전체
theme/DPI 행렬을 증명하지 않는다. 변경은 Dev에만 남아 있으며 commit, push,
Original promotion, release/deployment는 수행하지 않았다.

다음 프로젝트 우선순위는 OVL-06 행동 회귀·프로세스 복구·정량 감사 baseline이다.  
Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
