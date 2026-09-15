# OpenVisionLab 2D 우선순위 리팩토링 및 5분 실행 계획

Updated: 2026-09-11 KST

Execution state: R1~R15 autonomous source/build/focused-contract slices are
verified. R16~R30 junior discoverability, Pipeline Review recheck, release
precheck, and debug-output cleanup slices are
also verified. A new explicit user requirement opened a separate junior
discoverability program; it did not reopen the completed R1~R15 owners by
itself. R12 moved the reproduced Shell readiness policy to the existing
ViewModel owner and R13 moved the direct Recipe save callback to the existing
Recipe controller. R14 completed the source-level Shell/Pipeline Review
code-behind boundary audit without reopening closed owners. R15 completed the
Common responsibility-folder cleanup while preserving namespaces and public
contracts. The targeted
Threshold desktop/UI smoke, representative default/performance WPF smoke
suites, cross-window DisplayManager lifetime sequence, public sample
learn/pair contract sequence, sequential sample-review async deadlock
regression, and Pipeline Review smoke ownership/UI-contract follow-up are
verified; the full WPF qualification matrix remains unverified.
이 문서는 사용자 제공 Astra 분석을 현재 Dev
checkout의 실제 소스·문서·검증 결과와 대조해 정리한 실행 기준이다. 대규모
재작성이나 새 기능의 선행 설계가 아니라, 기존 owner와 공개 계약을 보존한
작은 검증 가능한 변경을 순서대로 진행한다.

## 기준과 증거 범위

| 항목 | 현재 Dev checkout에서 확인한 값 |
| --- | --- |
| Repository | `C:\\Git\\2D\\Dev` |
| Remote | `https://github.com/Noah8218/OpenVisionLab_Dev.git` |
| Branch | `codex/public-sample-ux-docs` |
| HEAD | `0a77e60e444b12757565ee977216a994446b53a6` |
| Target framework | `net8.0-windows7.0` |
| Product version | `2.2.0-dev.2` (`src/OpenVisionLab/OpenVisionLab.csproj`) |
| Product identity | Rule-based OpenCvSharp 2D recipe workbench |
| Current automation | None; the R16~R25 heartbeat and the partial-structure heartbeat were deleted after their verified boundaries |

사용자가 붙여 넣은 분석의 기준(`main`, `bd4659b...`, `2.2.0-dev.1`)은 현재
checkout과 일치하지 않는다. 따라서 구현·검증의 사실 기준은 위의 현재
checkout이며, 붙여 넣은 내용은 문제 후보와 우선순위를 정하는 입력 자료로만
사용한다. 기존 폴더 정리, MVVM/partial 감사, AppPath 경계, BackgroundLoopWorker
수정은 현재 handoff에 완료 근거가 있으므로 새 작업으로 중복하지 않는다.

직접 확인한 범위는 솔루션·프로젝트 참조, 시작점, release 검증 스크립트,
Pipeline 실행 서비스, Threshold View, 현재 handoff·문서 색인·issue ledger다.
실제 카메라/SDK, 모든 WPF theme·DPI·monitor, 장시간 native 운전은 이 문서가
보증하지 않는다. 해당 결론은 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요`로 기록한다.

## 현재 구조와 유지할 owner

현재 시작 경로는 다음과 같다.

```text
src/OpenVisionLab/Program.cs:Main
  -> App/Bootstrap/OpenVisionLabApplication.Run
  -> OpenVisionShellHostWindow
  -> UI/Menu/Wpf/Shell/OpenVisionShellHostView
```

주요 기능 흐름은 기존 owner를 따라간다.

```text
ImageSpaceFrame/ImageSpace
  -> Layer/Tool View 및 PropertyGrid
  -> VisionPipelineStep
  -> VisionPipelineExecutionService
  -> RecipeRuntimeStorage/RecipeState
  -> VisionToolResult 및 persisted run report
  -> Pipeline Review/Run History
```

| 영역 | 현재 owner·호출 경로 | 이번 계획의 원칙 |
| --- | --- | --- |
| 실행/종료 | `Program.Main`이 `OpenVisionLabApplication.Run`을 호출 | `Application.Run` 결과와 shutdown 순서를 보존하면서 exit code만 정확히 전달 |
| Release 검증 | `VerifyReleaseCandidate.ps1`가 build/policy/sample/publish 후 `TestReleaseDistribution.ps1` 호출 | .NET 8 metadata 검증을 실행 가능한 PowerShell 경계에서 수행하고 실패를 전파 |
| Pipeline timeout | `VisionPipelineExecutionService.Execute` → `WaitForStepCompletionStatusAsync` | timeout/cancel과 실제 worker drain을 분리하고 native 결과를 drain 전까지 해제하지 않음 |
| Threshold suggestion | `ThresholdToolWpfView` → 기존 `VisionToolThresholdSuggestionAnalyzer`와 `ThresholdInteractionController` | 기존 analyzer/presenter/view model 중 실제 state owner를 확인한 뒤 정책만 이동; View에는 UI lifecycle 유지 |
| Root/folder | 각 프로젝트의 책임 폴더와 기존 namespace/XAML 계약 | 완료된 `Docking.Controls` 이동과 root visibility 정책을 재개방하지 않음 |

## 우선순위와 완료 조건

각 행은 하나의 독립적인 실행 slice다. 상태는 소스 확인만으로 VERIFIED가
되지 않으며, 명시한 focused check와 변경 후 호출 경로 확인이 있어야 한다.

| ID | 우선순위 | 상태 | 작업 및 최소 변경 | 완료 조건·focused check |
| --- | --- | --- | --- | --- |
| R1 | P0 | VERIFIED (clean gate + no-`-SkipLaunch` migration + desktop runtime) | `tools/TestReleaseDistribution.ps1`의 `Assembly.LoadFrom` metadata 검증과 `VerifyReleaseCandidate.ps1`/CI의 PowerShell 호출 경계를 대조한 뒤, 현재 변경을 임시 clean worktree/branch에 구성해 canonical gate를 실행했다. distribution child는 기존 수정(`19d7ab0b`)대로 `pwsh`에서 실행되었고, 상위 `Invoke-NativeStep`의 non-zero 전파는 source로 확인했다. | `VerifyReleaseCandidate.ps1 -OutputDir artifacts\release_candidate_no_skip_20260910`가 Debug/Release build, readiness, 외부 DLL/NOTICE, 33-row public sample gate, clean Release publish, manifest/hash/archive, `TestReleaseDistribution.ps1` metadata 검증과 `-SkipLaunch` 없는 두 번의 실행·데이터 루트 마이그레이션까지 207.643초 후 PASS했다. 임시 validation commit은 `4c13f145f7557b49693ee09f85da281ba3cdd6b0`이며, 복사 보존 증거는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-clean-no-skip-20260910`이고 archive SHA-256은 `CF94D370035D9C70661AE3B15BF746A27664320C48FC8C637E14B017BA83E8A9`이다. 같은 설치 EXE에 동적 모니터 배치 desktop contract를 추가 실행해 작은 왼쪽 `\\.\DISPLAY2`에서 정상 종료 `0`, 의도적 TCP smoke 실패 `1`, 두 창의 실제 교차를 확인했다. |
| R2 | P0 | VERIFIED (source + desktop runtime) | `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs:99-102`의 `application.Run(shellWindow)` 반환값을 `exitCode`로 캡처해 `Run`의 결과로 전달한다. `Program.Main`의 반환 계약과 startup loading/shutdown/dispose 순서를 유지했다. | Debug/Release 앱 빌드와 source contract가 통과했다. D: 격리된 desktop contract가 두 창을 동적으로 선택한 작은 왼쪽 모니터 `\\.\\DISPLAY2`에 배치했으며, 정상 `CloseMainWindow`는 `ExitCode=0`, 잘못된 TCP smoke 포트 입력은 `Application.Shutdown(1)`을 통해 `ExitCode=1`을 반환했다. 증거: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\application-exit-code-20260910-rerun\\desktop-exit-code-contract.json`. |
| R3 | P1 | VERIFIED (focused smoke; native hang unverified) | `VisionPipelineExecutionService.WaitForStepCompletionStatusAsync`의 기존 구현은 `Completed/TimedOut/Canceled`, `WorkerDrained`, `DrainException`을 반환하고 늦은 `ResultImage`를 drain 후 해제한다. 기존 실행 계약 smoke를 재검증했다. | `dotnet exec tools\\VisionRecipeRunnerSmoke\\bin\\Any CPU\\Debug\\net8.0-windows7.0\\VisionRecipeRunnerSmoke.dll --pipeline-review-execution-contract D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pipeline-completion-status-20260910-final`가 통과했다. timeout/cancel 상태가 worker drain 전후로 구분되고, 늦은 성공 이미지·예외, 실행 계획 실패 복구, generation guard, 중복 실행 차단, 비동기 Dispose 대기가 모두 확인되었다. 증거: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pipeline-completion-status-20260910-final\\pipeline-review-execution-contract.txt`. 네이티브 함수가 영구 정지하는 상황은 프로세스 격리 없이는 보증하지 않는다. |
| R4 | P1 | VERIFIED (source + focused contract + cvr07 UI smoke; full matrix unverified) | `ThresholdToolWpfView.xaml.cs`의 제안·Undo 정책을 기존 analyzer/controller를 재사용하는 `VisionToolThresholdSuggestionSession`으로 이동했다. View는 증거 표시, 버튼/패널 상태, 마커 적용, 타이머와 control event를 유지한다. 기존 `cvr07_threshold_suggestion` 대상은 제품의 명시적 signal inspector 열기 동작을 거친 뒤 panel을 검사하도록 최소 보정했다. | 실제 call path는 `ThresholdToolWpfView` 버튼 이벤트 → `VisionToolThresholdSuggestionSession` → `VisionToolThresholdSuggestionAnalyzer`이며, 적용은 기존 `VisionToolThresholdInteractionController` → `ThresholdToolPresenter` → `ThresholdToolViewModel`로 이어진다. mutable suggestion/Undo write owner는 session 하나다. `--threshold-suggestion-session-contract`가 View 경계, 분석/적용, AlreadyCurrent, stale evidence, Clear 후 Undo 보존·소비, non-Basic 차단을 7/7 통과했고, `cvr07_threshold_suggestion` UI smoke가 overlay/panel/buttons/plot AutomationId를 6.6초에 확인했다. XAML event 이름과 `CreateProperty` 공개 계약은 유지했다. 전체 WPF 입력·theme·DPI·monitor 행렬은 `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 남긴다. |
| R5 | P2 | VERIFIED (audit-only; no justified mutation) | P0/P1 완료 후 큰 파일·partial·Shell 식별자 후보를 다시 조사했다. 기존 full refactor audit에서 844 C#/110 partial/27 project/34 reference/cycle 0을 확인했고, 이번 slice에서도 완료 owner를 재개방할 새 결함·호출자·수명 경계는 발견하지 않았다. Shell 이름은 공개/XAML/serialization/reflection 영향이 있어 현 상태를 보존한다. | 새 wrapper/partial/factory chain과 미관 목적의 기계적 rename을 추가하지 않는 결정을 기록했다. 기존 완료 구조를 유지하는 것이 최소 변경 조건을 충족하며, 향후 변경은 재현 가능한 결함 또는 경계 변경이 생길 때 별도 slice로 연다. |
| R6 | P0 | VERIFIED (resource lifetime + stale Dispatcher callback) | 순차 WPF 대상에서 `OpenVisionShellHostView.ReleaseDisplayManager`가 정적 `DisplayManagerService.Default`를 Dispose해 다음 대상의 `ImageSpaceService` 접근이 실패하는 결함을 수정했다. View는 주입된 세션별 manager만 정리하고, process-scoped 기본 manager는 `OpenVisionLabApplication`의 WPF dispatcher 종료 후 정리한다. 추가로 `OpenVisionBitmapCanvasPresenter`와 workspace preview owner가 Dispose 이후 `ImageViewer`를 참조하는 stale Dispatcher callback을 무시하도록 보정했다. | `wpf_shell_preview,wpf_shell_host_window_chrome,wpf_shell_host_window_maximized` 순차 대상과 `wpf_shell_host_workspace_empty,wpf_shell_host_learn_entry,wpf_shell_host_tool_search` race 재현 대상이 모두 `OK`가 되었고, 수정 후 기본 22개와 `perf` 4개도 전부 `OK`였다. `--all`은 사용자 입력이 필요한 후속 대상에서 정체되어 중단했으며, 중단 전 출력에는 `ObjectDisposedException`이 다시 나타나지 않았다. 당시 분리해 둔 sample fixture/learn-pair 계약은 R7에서 별도 검증했다. 전체 matrix는 여전히 미검증이다. |
| R7 | P1 | VERIFIED (public sample learn/pair contracts) | `OpenVisionWorkspaceLearnDocumentService`가 Geometry 학습 경로에서 다중 도구 Fixture pipeline의 Threshold 텍스트를 먼저 선택하던 문서 매핑 오류를 수정해 명시적으로 선택한 Geometry path를 우선한다. `PipelineViewerScreenshotSmoke`의 Fixture picker 계약은 검색 결과 전체가 아니라 선택된 PairGroup의 Good/Bad 두 행을 검증하도록 보정했고, Good-only AffineTransform benchmark를 pair coverage 대상에서 제외하며 `DistanceMm`/`DistancePx` suffix가 다른 평균·range·max metric을 같은 거리 판정군으로 비교한다. 기존 샘플 카탈로그와 실행 알고리즘은 변경하지 않았다. | Debug/Release `RunUiScreenshotSmoke.ps1 -Targets wpf_shell_host_workspace_sample_learn_paths,wpf_shell_host_workspace_sample_fixture_picker,wpf_shell_host_workspace_sample_pair_coverage`가 세 대상 모두 `OK`로 완료되었다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-sample-contracts-final-20260910` 및 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-sample-contracts-release-20260910`. 변경 smoke 프로젝트 Release build는 오류 0, 기존 nullable warning 1개로 완료되었다. 전체 WPF theme/DPI/monitor/keyboard 행렬과 사용자 선택 의존 `--all` 후속 대상은 검증하지 않았다. |
| R8 | P0/P1 | VERIFIED (sequential sample-review async deadlock) | `VisionPipelineSampleCheckService.RunSampleCheckSafe`가 UI thread에서 동기적으로 `RunAsync`를 기다릴 때, 빠른 첫 실행과 달리 두 번째 샘플 검토의 Pipeline continuation이 Dispatcher를 캡처해 교착하는 재현 결함을 확인했다. 기존 `VisionRecipeRunner`와 `VisionPipelineExecutionService` owner를 유지하고, 준비된 단계 실행·step completion·late tool result 대기에 `ConfigureAwait(false)`를 적용해 UI synchronization context 의존을 제거했다. 새 interface/worker/process boundary는 추가하지 않았다. | `wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_sample_pipeline_review_metrics` 순차 Debug smoke와 `wpf_shell_host_workspace_sample_open,wpf_shell_host_workspace_sample_pipeline_review_ng_metrics` 순차 Debug smoke가 각각 두 대상 모두 `OK`로 완료되었다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r8-sample-open-metrics-20260910` 및 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r8-sample-open-ng-metrics-20260910`. `VisionRecipeRunnerSmoke` pipeline-review execution contract도 Debug/Release에서 통과했으며 증거는 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-completion-status-r8-20260910` 및 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-completion-status-r8-release-20260910`이다. Solution Debug/Release build는 경고 0·오류 0, 변경 smoke Debug build는 오류 0·기존 MSB3270 경고 1개로 완료되었다. 전체 WPF theme/DPI/monitor/keyboard 행렬과 camera/SDK/long-run native 경계는 검증하지 않았다. |
| R9 | P1 | VERIFIED (Pipeline Review smoke ownership/UI contract) | 일부 Pipeline Review smoke가 제품의 현재 도킹된 `OpenVisionPipelineReviewDocument`를 부동 `OpenVisionFloatingToolWindow`로 잘못 가정했고, 접힌 Details 영역의 Tab/diagnostic AutomationId를 바로 검사했다. `tools/PipelineViewerScreenshotSmoke/Program.cs`에서 기존 `GetActiveToolVisualRoot`와 Details 토글을 사용하도록 테스트 경계만 보정했으며, `ScoreMax` 번역 키가 없는 경우 기존 `VisionPipelineKnownMetrics` display name으로 fallback하도록 했다. production View, document owner, binding/public contract는 변경하지 않았다. | Fixture review, Normalize Fixture review, Fixture teach, CVR-06 matcher, Edge NG diagnostics, Feature/Line/Blob/BentPin/Film NG metrics 대상이 Debug에서 모두 `OK`였다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-fixture-review-20260910-rerun`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-normalize-fixture-review-20260910`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-fixture-teach-20260910`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-cvr06-20260910`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-edge-ng-20260910-rerun`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-feature-ng-20260910-rerun`, `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-ng-metrics-final-20260910`. 변경 smoke Debug build는 오류 0·기존 MSB3270/CS8600 경고 2개, Release build는 오류 0·CS8600 경고 1개였다. 전체 WPF theme/DPI/monitor/keyboard 행렬과 실제 hardware/long-run 경계는 검증하지 않았다. |
| R10 | P1 | VERIFIED (Shell/Pipeline Review reading path) | `OpenVisionShellHostToolWindowController.ShowSelectedTool`의 Pipeline branch가 document restore/create와 도킹 표시를 모두 inline으로 수행해, Pipeline Review가 부동 Tool인지 도킹 Document인지 처음 읽을 때 바로 드러나지 않았다. 기존 호출·state owner는 유지하고 branch를 이름 있는 `ShowPipelineReview` private call path로 옮겼다. `CODEBASE_STRUCTURE.md`와 별도 탐색성 보고서에 생성·도킹·Details·실행·종료 owner와 최단 reading order를 기록했다. 긴 타입명 일괄 rename, 새 Manager/Factory/Interface/partial, Shell 전체 재분리는 하지 않았다. | OpenVisionLab Debug/Release build는 경고 0·오류 0. `wpf_shell_host_pipeline_review`, `wpf_shell_host_pipeline_review_ng`, `wpf_shell_host_workspace_sample_pipeline_review_metrics`가 모두 `OK`였다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r10-pipeline-review-20260910`. `TestDocumentationIndex.ps1`와 `Invoke-RefactorAudit.ps1 -Verify`도 PASS였다. alternate theme/layout/DPI/monitor, keyboard/pointer, camera/SDK, permanent native hang, long-run은 미검증으로 유지한다. |
| R11 | P1 | VERIFIED (Shell MVVM state/I/O ownership) | `OpenVisionShellHostView.Interactions.cs`가 Recipe 선택 persistence와 Workspace 이미지 경로 persistence를 직접 수행해 View code-behind에 업무 상태·파일 I/O가 남아 있었다. 기존 `OpenVisionShellHostRecipeController`와 `OpenVisionShellHostWorkspaceImageController`로 각각 이동하고, View는 기존 callback만 전달하도록 최소 변경했다. | OpenVisionLab Debug/Release build는 경고 0·오류 0, `OpenVisionReadinessCheck` PASS, `wpf_shell_host_workspace_image_load` 및 `wpf_shell_host_recipe_context_switch`가 모두 `OK`였다. `TestDocumentationIndex.ps1`와 `Invoke-RefactorAudit.ps1 -Verify`도 PASS였다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r11-shell-state-20260910`. 기존 `SaveConfig` 조건과 XAML/command callback 계약을 유지했다. 전체 WPF theme/layout/DPI/monitor/keyboard/long-run은 미검증으로 유지한다. |
| R12 | P1 | VERIFIED (Shell readiness MVVM ownership) | `OpenVisionShellHostView.Interactions.cs`가 Arithmetic settings를 읽고 `RequiresInputLayerB`와 Main/보조 Layer 증거를 직접 계산해 ViewModel에 전달하고 있었다. 기존 `OpenVisionShellPreviewViewModel`에 `RefreshToolReadiness`를 추가해 readiness 정책과 상태 투영을 같은 owner에 모으고, View는 기존 이벤트에서 owner를 호출하도록 최소 변경했다. | OpenVisionLab Debug build는 경고 0·오류 0, `OpenVisionReadinessCheck` PASS. 기존 `wpf_shell_host_workspace_image_load`, `wpf_shell_host_recipe_context_switch`, `wpf_shell_host_tool_search`가 모두 `OK`였다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r12-tool-readiness-20260910`. Arithmetic/Layer readiness, Preview/Run 명시 동작, XAML binding 계약을 유지했다. Smoke project Debug build는 오류 0·기존 CS8600 warning 1개. 전체 WPF theme/layout/DPI/monitor/keyboard/long-run은 미검증으로 유지한다. |
| R13 | P1 | VERIFIED (Shell Recipe save callback ownership) | `OpenVisionShellHostView.xaml.cs`가 `runtimeContext.Global.Recipe.SaveTools()`를 lambda로 직접 전달해 View composition에서 Recipe persistence owner가 바로 드러나지 않았다. 기존 `OpenVisionShellHostRecipeController`에 `SaveRuntimeRecipeTools`를 추가하고 동일한 `Func<bool>` callback을 그 메서드로 연결했다. | OpenVisionLab Debug/Release build는 경고 0·오류 0, `OpenVisionReadinessCheck` PASS. `wpf_shell_host_recipe_context_switch` 및 `wpf_shell_host_recipe_change_safety`가 모두 `OK`였다. 증거: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r13-recipe-save-owner-20260910`. Recipe XML/rollback/save result와 XAML command contract는 유지했다. `TestDocumentationIndex.ps1`와 `Invoke-RefactorAudit.ps1 -Verify`도 PASS였다. 전체 WPF theme/layout/DPI/monitor/keyboard/long-run은 미검증으로 유지한다. |
| R14 | P1/P2 | VERIFIED (Shell/Pipeline Review code-behind boundary audit) | Shell/Pipeline Review production View의 남은 직접 호출과 110개 partial 선언을 다시 분류했다. 남은 코드는 WPF dialog/MessageBox, control layout/event lifetime, display-only Bitmap/hit-test, localization, callback forwarding으로 확인되었고, Pipeline storage/execution, Recipe persistence, readiness policy는 production View에 남아 있지 않았다. 새 split/rename/partial은 추가하지 않았다. | direct-signal search 결과와 partial 목록을 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r14-codebehind-audit-20260910`에 기록했다. R13 Debug/Release build, `OpenVisionReadinessCheck`, Recipe smoke, `TestDocumentationIndex.ps1`, `Invoke-RefactorAudit.ps1 -Verify`, `git diff --check`를 유지 확인했다. 새로운 재현 결함이 없으므로 no-new-split 결정을 닫았다. 전체 WPF theme/layout/DPI/monitor/keyboard/long-run은 미검증으로 유지한다. |
| R15 | P2 | VERIFIED (Common responsibility folders) | `src/OpenVisionLab/Common`의 23개 파일을 책임별 `Runtime`, `Imaging`, `PropertyGrid`, `Recipe`, `Persistence`, `Results`, `Events`, `MessageDialogs`, `Account` 폴더로 이동했다. `AppCommon`, `CCommon`, `DEFINE`는 owner를 추측하지 않고 Common root에 유지했으며, namespace/type/XML/XAML/SDK 계약은 보존했다. `OpenVisionReadinessCheck`의 AppPath source path만 새 physical path로 갱신했다. | OpenVisionLab Debug/Release build는 각각 경고 0·오류 0, Readiness Debug/Release PASS, `TestDocumentationIndex.ps1` PASS (`IndexedPaths=285`, `Routes=16`, `RootRedirects=102`), `Invoke-RefactorAudit.ps1 -Verify` PASS (`CSharpFiles=847`, `XamlFiles=60`, `PartialDeclarations=110`, `ViewModelUiIoFiles=1`, `ProjectCycles=0`, `ShellStorageCalls=0`), `git diff --check` 통과. 전후 inventory와 path scan은 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-r15-common-folder-audit-20260910`에 기록했다. 전체 WPF runtime은 별도 경계다. |

## 주니어 탐색성·릴리스 검증 프로그램 — R16~R30

사용자가 새로 명시한 요구사항은 Shell의 Controller/Presenter/Document/
Workspace 연결, 긴 타입명, 수동 partial 파일이 처음 보는 개발자에게 실제
소유자를 숨긴다는 문제를 해결하는 것이다. 이 프로그램은 이미 닫힌 owner를
파일 길이나 취향만으로 다시 분리하지 않는다. 먼저 현재 생성자·호출자·mutable
state writer·binding/public 계약을 표로 고정하고, 그 표가 드러내는 독립적인
변경 이유가 있을 때만 한 번에 한 concrete owner를 옮긴다.

| 단계 | 우선순위 | 상태 | 범위와 완료 조건 |
| --- | --- | --- | --- |
| R16 | P1 | VERIFIED (owner/partial inventory) | `OpenVisionShellHostView`의 59개 composition field와 WPF 영역의 37개 partial 파일을 실제 경로·줄 수·역할로 분류하고, 생성·호출·mutable-state writer·Dispose·XAML/public 계약을 `CODEBASE_STRUCTURE.md`의 3.6과 이 문서의 proof boundary에 고정했다. existing Pipeline Review/MVVM/Common 문서를 중복하지 않고 first-time reading order만 보강했다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r16-audit\shell-composition-fields.csv`, `shell-partial-inventory.csv`, `summary.txt`를 생성했다. `Invoke-RefactorAudit.ps1 -Verify` PASS (`CSharpFiles=847`, `XamlFiles=60`, `PartialDeclarations=110`, `ProjectCycles=0`, `ShellStorageCalls=0`), `TestDocumentationIndex.ps1` PASS (`IndexedPaths=285`, `Routes=16`, `RootRedirects=102`), OpenVisionLab solution Debug/Release build 모두 경고 0·오류 0. R16은 문서/source inventory 경계이며 UI runtime PASS를 주장하지 않는다. |
| R17 | P1 | VERIFIED (no safe extraction) | `OpenVisionShellHostRecipeCommandSurface`의 10개 partial family를 caller·mutable-state writer·binding/public 계약별로 조사했다. LLM draft/validation-set/pipeline exchange/run-history/qualified snapshot은 기존 concrete owner를 이미 호출하지만 binding-facing 상태·command callback·recipe selection이 base class private state를 공유한다. 현재 snapshot/result seam과 focused test가 없는 독립 lifecycle boundary는 증명되지 않아 production extraction을 하지 않았다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r17-audit\recipe-command-partial-coupling.csv`와 `summary.txt`에 partial별 shared field/method/direct I/O/existing owner 호출을 기록했다. 기존 owner를 재사용하고 새 interface/factory/manager/partial을 만들지 않은 no-change 판단이다. R18에서 계약이 없는 내부 이름만 최소 개명한다. |
| R18 | P2 | VERIFIED (contract-safe internal rename) | `OpenVisionShellHostLayerListRefreshResult`는 Shell/Layers 내부에서만 사용되고 XAML/public/reflection/serialization 계약이 없음을 확인해 `LayerListRefreshResult`로 개명했다. 호출 경로와 결과 값은 동일하며, partial은 추가하지 않았다. | old name은 active `src`/`tools` C#/XAML에서 사라지고 new name은 Presenter→LayerRefreshController 호출 경로에만 남는다. OpenVisionLab solution Debug/Release build, Readiness Debug/Release, `Invoke-RefactorAudit.ps1 -Verify` (`847/60/110/1/0/0`), `TestDocumentationIndex.ps1` (`285/16/102`), `git diff --check`를 실행했다. LF→CRLF notices만 있고 whitespace error는 없었다. R19 folder audit로 이동한다. |
| R19 | P2 | VERIFIED (no safe physical move) | `src/OpenVisionLab` entry/configuration, `Common` legacy root, `UI/Menu/Wpf` Shell composition root, nested responsibility folders, and standalone smoke project boundaries를 inventory했다. AGENTS가 고정한 temporary/root 경계와 dirty Host/Recipe 파일은 이동하지 않았고, dependency closure를 바꾸는 speculative move도 추가하지 않았다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r19-folder-audit\root-file-inventory.csv`, `large-folder-inventory.csv`, `summary.txt`에 19개 10-file 초과 폴더와 root 파일을 기록했다. 추가 namespace/XAML/project dependency 변경 없음; 기존 R15 Common 정리와 현재 folder owner를 보존했다. |
| R20 | P2/P3 | VERIFIED (source/focused smoke) | Shell 전체를 처음부터 다시 조합하지 않고, R16~R19 owner map을 기준으로 `Program -> Application -> Shell -> Tool selection -> Document/Workspace -> Tool/ViewModel -> Result/Review` 읽기 경로, old-name absence, Shell direct policy-call absence, build/readiness/audit를 재확인했다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r20-final`의 reading route 235 lines, old-name search 0 lines, Shell direct policy search 0 lines, diff check와 focused smoke를 기록했다. `wpf_shell_host_workspace_empty`가 2376ms `OK`였다. 전체 theme/layout/DPI/monitor/keyboard, camera/SDK, 장시간 native 검증은 별도 환경 경계로 남긴다. |
| R21 | P2 | VERIFIED (contract-safe internal rename) | `OpenVisionShellHostDockedLayerWorkspaceComposition`은 내부 Shell adapter로만 사용되고 XAML/public/reflection/serialization 계약이 없어 `ShellDockedLayerWorkspaceComposition`으로 개명했다. factory, Shell View field, constructor, file path만 갱신했으며 workspace state/lifetime/callback 동작은 유지했다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r21-audit\summary.txt`, `old-name-search.txt`, `new-name-call-path.txt`; old-name 0, new-name 5 refs, OpenVisionLab Debug/Release build 각각 경고 0·오류 0, `git diff --check` whitespace error 없음. Runtime UI matrix는 미검증이다. |
| R22 | P2 | VERIFIED (contract-safe internal rename) | 기존 내부 test-support adapter `OpenVisionShellHostDockingTestFacade`의 caller·mutable-state·lifetime·계약을 확인하고 `ShellDockingTestFacade`로 개명했다. 기존 diagnostics/action forwarding과 Shell disposal 경계는 유지했다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r22-audit\summary.txt`, `old-name-search.txt`, `new-name-call-path.txt`; old-name 0, new-name 5 refs, OpenVisionLab Debug/Release build 각각 경고 0·오류 0, `git diff --check` whitespace error 없음. Runtime UI matrix는 미검증이다. |
| R23 | P2 | VERIFIED (contract-safe internal rename) | 기존 내부 layer test-support adapter `OpenVisionShellHostLayerTestFacade`와 callback holder `OpenVisionShellHostLayerTestFacadeBindings`의 caller·state·lifetime·계약을 확인하고 각각 `ShellLayerTestFacade`와 `ShellLayerTestFacadeBindings`로 개명했다. TestHooks forwarding과 Shell disposal 경계는 유지했다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r23-audit\summary.txt`, `old-name-search.txt`, `old-bindings-name-search.txt`, `new-name-call-path.txt`; old-name 0, new-name 8 refs, OpenVisionLab Debug/Release build 각각 경고 0·오류 0, `git diff --check` whitespace error 없음. Runtime UI matrix는 미검증이다. |
| R24 | P2 | VERIFIED (contract-safe internal rename) | 기존 내부 tool/test-support adapter `OpenVisionShellHostToolTestFacade`와 callback holder `OpenVisionShellHostToolTestFacadeBindings`의 caller·state·lifetime·계약을 확인하고 각각 `ShellToolTestFacade`와 `ShellToolTestFacadeBindings`로 개명했다. TestHooks, native-tool, docking, Pipeline Review forwarding과 Shell disposal 경계는 유지했다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r24-audit\summary.txt`, `old-name-search.txt`, `old-bindings-name-search.txt`, `new-name-call-path.txt`; old-name 0, new-name 8 refs, OpenVisionLab Debug/Release build 각각 경고 0·오류 0, `git diff --check` whitespace error 없음. Runtime UI matrix는 미검증이다. |
| R25 | P2 | VERIFIED (no-change lifecycle naming audit) | `OpenVisionShellHostToolWindowLifecycleController`의 실제 caller·mutable-state·lifetime·계약을 조사했다. `OpenVisionShellHost`/`ToolWindow`/`LifecycleController`가 Shell composition 경계와 실제 상태 전이 책임을 각각 나타내므로 개명하지 않았다. 별칭·wrapper도 추가하지 않았다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r25-audit\summary.txt`, `caller-inventory.txt`; source-only internal contract search 완료. 변경 코드가 없어 R24 Debug/Release build를 마지막 변경 코드 gate로 유지한다. Runtime UI matrix는 미검증이다. |
| R26 | P2 | VERIFIED (member navigation) | `OpenVisionWorkspaceSamplePickerViewModel`을 새 클래스로 나누지 않고 기존 멤버를 Fields/Constructors/Properties/Filtering/Commands/Preview/Localization 책임 영역으로 표시했다. binding 이름·command 동작·selection state owner를 유지했다. | normalized logic SHA-256가 전후 동일했고, Debug/Release solution build, Sample Picker UI smoke (`wpf_shell_host_workspace_sample_picker=OK`), readiness, refactor audit, documentation index를 실행했다. Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r26-sample-picker-20260911` 및 `docs/reports/OPENVISIONLAB_JUNIOR_WHOLE_REPOSITORY_REVIEW_20260911.md`. |
| R27 | P2/P3 | VERIFIED (environment matrix) | 현재 Windows 세션의 WPF runtime 환경, monitor bounds, DPI, theme registry와 실행 가능한 대체 행을 기록했다. 제품 source나 display setting을 바꾸지 않았고, 사용할 수 없는 DPI/theme/topology를 시뮬레이션하지 않았다. | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\wpf-runtime-environment-matrix-20260911\environment-matrix.json`에 Windows 11, 2-monitor/100% DPI, available/missing rows를 기록했다. 125/150/175/200%, one-monitor/headless, alternate theme은 미검증으로 고정했다. |
| R28 | P1 | VERIFIED (Pipeline Review focused recheck; no source change) | 기존 `OpenVisionPipelineReviewExecutionController`의 run identity, cancellation/drain, image cache, stale callback, document revision owner를 현재 checkout에서 재검증했다. 새 wrapper/interface/partial은 추가하지 않았다. | VisionRecipeRunnerSmoke Debug/Release build는 각각 경고 0·오류 0. layer image owner, cache lifetime, stale callback, execution, document revision 5개 계약을 양 구성에서 모두 exit code 0으로 통과했다. Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-review-recheck-20260911` 및 `docs/reports/OPENVISIONLAB_PIPELINE_REVIEW_RECHECK_20260911.md`. 새 결함은 재현되지 않았으며 full WPF alternate theme/DPI/input/long-run은 미검증이다. |
| R29 | P0/P1 | VERIFIED (Dev main promotion and last verified clean non-UI candidate gate) | 기존 dirty-worktree precheck 이후 승인된 refactor batch를 `176eec95`로 고정하고 `origin/main`과 충돌 없이 병합한 `37971e3a`를 Dev `main`에 강제 없이 push했다. docs-only release record commit `f35994b3`까지 포함한 clean validation branch에서 canonical `VerifyReleaseCandidate.ps1 -SkipLaunch`를 실행해 source/build, readiness, vendored dependency/NOTICE, public catalog, clean publish, manifest와 archive를 재검증했다. 태그·릴리스·배포는 실행하지 않았다. 이후 문서 기록 커밋은 제품 source를 바꾸지 않지만 release tag 전에는 최종 SHA로 gate를 다시 확인해야 한다. | Last verified main SHA `f35994b3c152b056c3649ef2a668083ba407122e`; ReleaseCandidateVerification `PASS`; Debug/Release/readiness/external references/public assets `PASS`; public catalog 33/33; framework-dependent win-x64 payload 77개; archive SHA-256 `40A45ED2EA09D1A63172D00A8FF0D26545D4ED981DAE3E112023312A31E00B52`. Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-main-20260911\artifacts\release_candidate_main_f359_20260911` 및 `docs/reports/OPENVISIONLAB_RELEASE_CANDIDATE_MAIN_20260911.md`. Desktop launch smoke, alternate WPF matrix, hosted CI 재실행은 별도 경계다. |
| R30 | P2 | VERIFIED (debug-output cleanup) | `RoiImageCanvasViewModel.OpenLoadImage`에서 운영 stdout으로 출력하던 두 개의 개발용 timing line과 unused `System.Diagnostics` import를 제거했다. ImageDialogHost → CanvasImageLoader → LoadImage → directory policy call path, Mat `using` lifetime, binding/command contract는 유지했다. | Solution Debug/Release 모두 경고 0·오류 0, Release Readiness PASS, `src` active Console.Write scan 0. Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-review-recheck-20260911\debug-output-cleanup` 및 `docs/reports/OPENVISIONLAB_IMAGECANVAS_DEBUG_OUTPUT_CLEANUP_20260911.md`. |

### R16 proof boundary

현재 owner는 `OpenVisionShellHostView`의 composition layer이며, 의도한
owner map은 기존 `Session`, `Layer`, `Workspace`, `Tooling`, `Recipe`,
`Documents` concrete owner를 그대로 가리키는 탐색 문서다. 호출 경로는
`Program.Main -> OpenVisionLabApplication.Run -> OpenVisionShellHostWindow ->
OpenVisionShellHostView`에서 시작해 Shell constructor가 각 owner를 만들고,
`ShowSelectedTool -> ShowPipelineReview`가 Pipeline Review document로 이어진다.
mutable state와 dispose 책임은 기존 owner에 남기며, XAML dependency property와
test facade 공개 계약도 유지한다. R16은 이 경계를 코드로 숨기지 않고 한 번의
검색으로 찾게 만드는 문서·inventory slice다. 실제 책임 이동은 R17에서 독립
경계가 재현될 때만 허용한다.

R16 검증은 다음으로 제한한다.

```powershell
rg -n "OpenVisionShellHostView|ShowSelectedTool|ShowPipelineReview|Dispose|partial class" src/OpenVisionLab/UI/Menu/Wpf
Invoke-RefactorAudit.ps1 -RepositoryRoot C:\Git\2D\Dev -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r16-audit -Verify
```

실제 Runtime UI, 모든 theme/DPI/monitor/input 상태는 이 문서만으로
`PASS`가 아니며, `소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로
남긴다.

### 기능 상태 분류

| 분류 | 현재 확인된 항목 |
| --- | --- |
| 구현되었고 테스트됨 | ImageSpaceFrame `Borrow/TakeOwnership`, atomic XML helper, 기존 folder/root/AppPath/BackgroundLoopWorker 경계, clean Release distribution metadata gate, Pipeline late-result drain smoke, sequential sample-review async deadlock regression, Threshold suggestion session policy contract, targeted `cvr07_threshold_suggestion` UI smoke, desktop exit-code contract, process-scoped DisplayManager lifetime guard와 순차 WPF smoke |
| 구현되었지만 현재 slice 재검증 필요 | Pipeline timeout의 장시간 native 반환 한계 |
| 구현되었지만 검증 부족 | 실제 WPF theme/DPI/monitor/long-run, `VerifyReleaseCandidate` hosted CI 재실행 |
| 문서에만 존재 또는 계획 | 단일 Threshold 안정 구간 검토 기능, 향후 중단 요청과 실제 종료 상태를 보여주는 운영 UI |
| 폐기/사용하지 않는 것으로 단정하지 않음 | public/internal 타입은 caller·reflection·serialization 조사가 끝나기 전 삭제하지 않음 |

## Threshold UI smoke 재검증 경계

변경된 Threshold 화면의 기존 `cvr07_threshold_suggestion` 대상은 처음에
현재 checkout과 변경 전 `HEAD` `0a77e60e444b12757565ee977216a994446b53a6`의
별도 clean worktree에서 같은 명령과 제한 시간으로 각각 NG가 되었다. 두
실행 모두 semantic replay 산출물(`threshold-suggestion.tsv`, 적용 PNG)은
만들었지만, 마지막 `AssertVisibleAutomationIds`가
`ThresholdSuggestionPanel`을 `size=0x0`/비가시 상태로 보고했다. 원인은
제품이 Preview 직후 `ThresholdSignalInspectorOverlay`를 접어 두고 panel을
그 안에 표시하는 기존 UI 계약인데, 대상이 overlay를 여는 호출 없이 내부
panel의 실제 가시성을 검사한 데 있었다. 다른 Threshold 대상과 제품의
기존 `OpenSignalInspectorForTest()` 경로를 대조해 원인을 확정했다.

`tools/PipelineViewerScreenshotSmoke/Program.cs`에 기존 명시적 signal
inspector 열기 호출과 4회 pump만 추가한 뒤 production View나 테스트 조건을
완화하지 않았다. 수정 후 대상은 6.6초에 `OK`로 완료되어 overlay, 제안
panel, Analyze/Use/Undo 버튼, signal plot AutomationId를 확인했고
1500×880 PNG를 생성했다.

증거 경로:

- 초기 현재 checkout: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-ui-smoke-20260910-fresh`
- 초기 baseline clean worktree: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-ui-smoke-20260910-baseline`
- 수정 후 통과: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-ui-smoke-20260910-fixed`

이 검증은 전체 WPF theme·DPI·monitor·keyboard/mouse 상태 행렬과 장시간
운영을 대신하지 않는다. 남은 행렬은 별도 환경 검증 경계로 유지한다.

## Threshold→Blob e2e smoke 재검증

`wpf_threshold_to_blob_detection_e2e`는 현재 checkout과 변경 전 HEAD
`0a77e60e444b12757565ee977216a994446b53a6`에서 같은 NG를 재현했다. 기존
`VisionToolThresholdInteractionController`가 Threshold 슬라이더 변경을
90ms debounced Preview로 예약하는데, 테스트가 이를 명시적 Preview 전용
계약으로 검사한 것이 원인이었다. 또한 저장된 Threshold 값이 이미 84인
실행에서는 값이 바뀌지 않아 이벤트가 발생하지 않는 비결정성도 확인했다.

제품 동작은 변경하지 않고 `tools/PipelineViewerScreenshotSmoke/Program.cs`의
테스트만 기존 동작에 맞췄다. `SetDifferentFloatingSliderValueByName`으로
실제 값 변경을 보장하고, 슬라이더 변경 후 Preview 1회·결과 존재를 검증한
뒤 중복 명시적 Preview 호출을 제거했다. 수정 후 단일 대상과 `e2e`, `route`,
`property-grid-auto-preview`, `preprocess-auto-preview` suites가 모두
bounded 실행에서 `OK`로 완료했다.

증거 경로:

- 변경 전 baseline 단일 대상: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-e2e-baseline-20260910`
- 수정 후 단일 대상: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-threshold-to-blob-20260910-rerun`
- 수정 후 e2e suite: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-e2e-20260910-fixed`
- route suite: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-route-20260910`
- PropertyGrid suite: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-property-grid-20260910`
- preprocess suite: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-preprocess-20260910`

이 suites는 기본 Korean UI와 현재 환경의 offscreen 렌더링만 검증한다. 전체
theme·DPI·monitor·keyboard/mouse 상태 행렬과 장시간 운전은 여전히
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요` 경계다.

## 대표 WPF 기본·성능 smoke 재검증

후속 독립 검증으로 기존 기본 target 묶음 22개와 `perf` suite 4개를 Debug
offscreen 경로에서 실행했다. Shell 시작·workspace/image load·tool input/output,
Threshold/Blob/Contour/Line/Matching, Pipeline Review, ROI editor, Image Compare,
Log Panel, localization catalog, tool-open 및 fast-click/first-heavy 성능 대상이
모두 `OK`였다.

최신 수명 수정 후 재실행한 증거 경로:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-default-fixed-20260910`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-perf-fixed-20260910`

초기 대표 실행의 증거도 보존한다:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-default-20260910`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-perf-20260910`

이 실행은 현재 기본 Korean UI의 quiet offscreen 렌더링과 계약 검사를 증명한다.
다른 theme, Wide/Compact layout, 100/125/150/175/200% DPI, 실제 pointer/keyboard
상태, 다중 모니터 desktop 배치와 장시간 실행은 포함하지 않는다.

## DisplayManager 수명 순차 재검증

초기 전체 목록 실행에서 `wpf_shell_preview`와
`wpf_shell_host_window_chrome` 뒤 세 번째 Shell Window를 만들 때 정적
`DisplayManagerService.Default`가 이미 Dispose되어
`ImageSpaceService.GetImage`에서 `ObjectDisposedException`이 발생했다. 호출
경로는 `OpenVisionShellHostWindow.OnClosed -> OpenVisionShellHostView.Dispose
-> OpenVisionShellHostSessionController.DisposeSession -> ReleaseDisplayManager`
였다. View가 process-scoped 기본 manager를 해제하지 않고, 애플리케이션
Bootstrap이 `Application.Run` 반환 뒤 기본 manager를 해제하도록 소유권을
정리했다. 주입된 per-session manager의 기존 Dispose 경로는 유지했다.

수정 후 다음 순차 대상이 모두 `OK`였다:

`wpf_shell_preview`, `wpf_shell_host_window_chrome`,
`wpf_shell_host_window_maximized`.

증거 경로:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-preview-window-max-fixed-20260910`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-lifetime-race-fixed-20260910`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-default-fixed-20260910`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-perf-fixed-20260910`

전체 `--all` 실행은 `wpf_shell_host_workspace_sample_pair_coverage` 이후
사용자 선택을 요구하는 다음 UI 대상에서 진행이 멈춰 제한 시간 전에 중단했다.
중단 전 초기 실행에서 `wpf_shell_host_learn_entry`와
`wpf_shell_host_tool_search`의 `OpenVisionBitmapCanvasPresenter.RefreshCanvas`
NullReferenceException이 관찰됐지만, Dispose 상태 guard를 적용한 별도 순차
검증에서는 두 대상 모두 `OK`였다. 남은 sample learn/pair fixture 데이터 계약
오류는 이번 DisplayManager 수명 수정과 다른 owner이며 별도 후속 작업으로
남긴다. 전체 WPF matrix는
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`로 유지한다.

## 5분 heartbeat 실행 계약

자동화 ID는 `openvisionlab-2d`이며 현재 thread에 연결된 5분 heartbeat다.
R16~R20 완료 뒤 `PAUSED`였지만, 2026-09-11 사용자 요청으로 기존 예약을
`ACTIVE`로 재개했다. R21~R25를 완료했고 R25에서 남은 production owner의
이름이 실제 책임을 정확히 나타내는 것을 확인했다. 독립적인 source boundary가
더 이상 없어 기존 `openvisionlab-2d` heartbeat는 R25 evidence를 남긴 뒤
삭제했다. 새 범위가 명시되기 전까지 중복 예약이나 추측성 리팩토링을 만들지
않는다.
공식 자동화 문서가 설명하는 chat-linked scheduled task의 반복 실행 모델을
사용하되, 이 저장소의 작업 규칙이 우선한다.

매 실행은 다음 순서를 따른다.

1. `AGENTS.md`, 이 문서, current handoff, 관련 report, `.proofline/issues`,
   활성 automation을 읽고 dirty work와 완료 owner를 보존한다.
2. 가장 높은 미완료 항목 하나만 `TODO -> IN PROGRESS`로 선택한다.
3. 현재 caller·mutable-state write owner·lifetime/release owner·binding/public
   contract를 확인하고 기존 concrete owner를 재사용한다.
4. 최소 변경을 적용한 뒤 관련 Debug/Release build, focused contract/smoke,
   정적 호출 경로·리소스 검색을 실행한다. 실행하지 않은 검사는 PASS로
   기록하지 않는다.
5. `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, 이 보고서, 필요 시 issue
   ledger에 수정 이유, 변경 파일, 기존/변경 동작, 위험, 실제 증거와 다음
   slice를 기록한다.
6. 사용자 입력·UI 조작·카메라·하드웨어·자격 증명이 필요한 단계는 기다리지
   않고 `BLOCKED` 또는 미검증 경계로 기록한 뒤 독립적인 코드 slice로 이동한다.

이 heartbeat는 `git commit`, `git push`, tag, release, deployment,
`C:\\Git\\2D\\Original` 변경, 새 automation 생성, 무관한 대규모 정리를
수행하지 않는다. 모든 P0/P1/P2 acceptance criteria가 검증되거나 남은
항목이 환경 의존 검증뿐이면 handoff에 종료 근거를 남기고 automation을
`PAUSED`로 전환한다. 사용자가 별도로 `PUSH`를 요청하기 전까지 Dev 변경은
로컬에 유지한다.

## 개발자 discoverability 기준

구조 변경을 완료로 기록할 때마다 다음을 함께 적는다.

- 현재 owner와 의도한 owner
- 실제 call path와 mutable-state write owner
- 기존 binding/public/serialization 계약
- 생성·소유·Dispose/해제 책임
- 한 번의 검색으로 따라갈 수 있는 최단 읽기 순서
- 실제 실행한 focused check와 증거 경로
- 아직 실행하지 않은 runtime/UI 범위

추천 읽기 순서는 `AGENTS.md` → `docs/README.md` → 이 문서 → current handoff
→ `docs/admin/CODEBASE_STRUCTURE.md` 1.1 → 해당 owner 파일 → 해당 focused
smoke/report다. 전체 chronology나 오래된 P-number 문서는 현재 call path가
필요할 때만 연다.

## 현재 작업 기록

| 날짜 | 작업 | 결과 |
| --- | --- | --- |
| 2026-09-10 | 현재 branch/SHA/framework/version·issue ledger·automation 대조 | 현재 Dev checkout을 구현 기준으로 확정; 사용자 제공 기준과 불일치 기록 |
| 2026-09-10 | 5분 heartbeat R16~R20 재개 | `openvisionlab-2d`, target thread `01a085df-41c7-7be3-a3ea-6a1cbf8999b0`; explicit junior discoverability requirement에 따라 ACTIVE, 매 실행 하나의 slice |
| 2026-09-11 | 5분 heartbeat 재개 및 R21 시작 | 기존 `openvisionlab-2d`가 R16~R20 완료 후 `PAUSED`였음을 확인하고 동일 예약을 `ACTIVE`로 복구; R21은 사용자 입력·하드웨어를 기다리지 않는 bounded source-only follow-up |
| 2026-09-11 | R22 Shell docking test facade rename | 내부 `OpenVisionShellHostDockingTestFacade`를 `ShellDockingTestFacade`로 최소 개명; caller/owner/계약 조사와 Debug/Release build 통과. Evidence: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r22-audit` |
| 2026-09-11 | R23 다음 bounded slice 예약 | R22 완료 후 같은 heartbeat를 `OpenVisionShellHostLayerTestFacade` 계약 조사로 이동; 기존 owner·dirty worktree 보존 |
| 2026-09-11 | R23 Shell layer test facade rename | 내부 `OpenVisionShellHostLayerTestFacade`와 bindings를 `ShellLayerTestFacade` 계열로 최소 개명; caller/owner/계약 조사와 Debug/Release build 통과. Evidence: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r23-audit` |
| 2026-09-11 | R24 다음 bounded slice 예약 | R23 완료 후 같은 heartbeat를 `OpenVisionShellHostToolTestFacade` 계약 조사로 이동; 기존 owner·dirty worktree 보존 |
| 2026-09-11 | R24 Shell tool test facade rename | 내부 `OpenVisionShellHostToolTestFacade`와 bindings를 `ShellToolTestFacade` 계열로 최소 개명; caller/owner/계약 조사와 Debug/Release build 통과. Evidence: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r24-audit` |
| 2026-09-11 | R25 다음 bounded slice 예약 | R24 완료 후 같은 heartbeat를 `OpenVisionShellHostToolWindowLifecycleController` 계약 조사로 이동; 기존 owner·dirty worktree 보존 |
| 2026-09-11 | R25 lifecycle controller no-change audit | `OpenVisionShellHostToolWindowLifecycleController`의 이름이 실제 Shell/ToolWindow/Lifecycle 책임을 나타내므로 개명하지 않음; evidence: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\junior-structure-r25-audit` |
| 2026-09-11 | R16~R25 heartbeat 종료 | R25 no-change boundary 후 `openvisionlab-2d` 기존 heartbeat를 삭제; 중복 예약과 무기한 대기를 남기지 않음 |
| 2026-09-11 | R26 Sample Picker member navigation | 기존 `OpenVisionWorkspaceSamplePickerViewModel`의 binding·selection owner를 유지하고 책임별 region만 추가; normalized logic contract와 Debug/Release/UI/readiness/audit/index 검증 통과 |
| 2026-09-11 | R27 WPF runtime environment matrix | 현재 Windows 11 2-monitor/100% DPI 환경과 사용 불가 alternate rows를 기록; display setting과 product source를 변경하지 않음 |
| 2026-09-11 | R28 Pipeline Review focused recheck | `OpenVisionPipelineReviewExecutionController`의 image owner/cache/stale callback/execution/document revision 계약을 Debug/Release 모두 재실행해 10/10 exit code 0; 새 source defect 없음. Evidence: `D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\pipeline-review-recheck-20260911` |
| 2026-09-10 | R16 Shell owner/partial inventory | composition field 59개와 WPF partial file 37개를 D: evidence로 분류; docs index, refactor audit, Debug/Release build 통과 |
| 2026-09-10 | R17 Recipe command-surface partial audit | partial 10개와 shared binding/private state를 증명; 독립 lifecycle/test seam이 없어 no-change로 종료 |
| 2026-09-10 | R18 contract-safe internal rename | source-only `OpenVisionShellHostLayerListRefreshResult`를 `LayerListRefreshResult`로 최소 개명; Debug/Release/readiness/audit/smoke 통과 |
| 2026-09-10 | R19 folder ownership audit | Wpf composition root, Common legacy root, standalone smoke 경계를 확인하고 무근거 이동을 하지 않음 |
| 2026-09-10 | R20 final discoverability proof | reading route 235 lines, old-name 0, Shell direct policy-call 0, `wpf_shell_host_workspace_empty` 2376ms `OK` |
| 2026-09-10 | R8 sequential sample-review async deadlock | 기존 Pipeline 실행 owner를 유지하고 `ConfigureAwait(false)` 경계를 보강; sample-open 뒤 metrics/NG review 순차 smoke와 solution Debug/Release build 통과 |
| 2026-09-10 | R9 Pipeline Review smoke ownership/UI contract | 도킹된 Pipeline Review root와 접힌 Details 영역을 반영하도록 기존 smoke 탐색 경계를 보정; Fixture/Normalize/Teach/CVR-06/Edge/Feature·Line·Blob·BentPin·Film 대상 통과 |
| 2026-09-10 | AppPath runtime path boundary follow-up | 기존 owner 유지, Debug/Release boundary contract 8/8 및 solution/readiness/refactor audit 통과; 상세 report는 별도 완료 문서 |
| 2026-09-10 | R2 Application exit-code propagation | `OpenVisionLabApplication.Run`이 `Application.Run(shellWindow)` 결과를 반환하도록 수정; 앱 Debug/Release 경고 0·오류 0, source contract PASS, 동적 모니터 배치 desktop contract에서 정상 0·의도적 실패 1 확인 |
| 2026-09-10 | R3 Pipeline completion-state verification | 기존 `WaitForStepCompletionStatusAsync` owner와 실행 경계를 유지; execution contract smoke가 timeout/cancel/drain, late result/exception, plan failure, generation guard, duplicate run, async disposal을 통과 |
| 2026-09-10 | R4 Threshold suggestion ownership | 제안·Undo mutable policy를 `VisionToolThresholdSuggestionSession`으로 이동; View 경계와 7개 focused contract, 기존 `cvr07_threshold_suggestion` UI smoke를 통과. 기존 Analyzer, InteractionController, Presenter, ViewModel, XAML event/binding 계약은 재사용·유지 |
| 2026-09-10 | R5 P2 structural audit decision | 큰 파일·partial·긴 Shell 이름을 재검토했지만 새 재현 결함이나 독립 수명/호출 경계가 없어 변경하지 않음; 기존 full refactor audit와 완료 owner를 유지 |
| 2026-09-10 | R6 DisplayManager/Dispatcher lifetime correction | 공유 기본 DisplayManager를 View에서 해제하지 않도록 owner를 Bootstrap으로 정리하고, Dispose 후 ImageViewer를 참조하는 stale Dispatcher callback을 차단; 순차 대상·기본 22개·perf 4개 통과 |

## 이슈 원장 등록 경계

실행 계획을 별도 `PL-0013` composite 이슈로 등록하려 했으나, 현재
`.proofline/issues/PL-0010.json`의 이미 종료된 상태에 허용되지 않는
`state.next_action`이 남아 있어 issue-ledger CLI가 기존 원장 전체를
검증 단계에서 거부했다. 과거 종료 이슈를 임의로 수정해 새 작업을 숨기지
않고, 입력 JSON과 실패 출력을
`D:\\OpenVisionLab-TestData\\OpenVisionLab_Dev\\prioritized-refactor-schedule-20260910`
에 보존했다. 현재 실행의 권위 상태는 이 보고서와 live handoff이며, 기존
PL-0010의 schema repair는 별도 후속 작업으로 남긴다.

## R16~R20 완료 기록

Status: Complete (source/focused discoverability scope)

Scope: 사용자가 명시한 Shell owner 탐색성, 긴 이름, partial, 폴더 배치 문제를
현재 Dev baseline에서 재조사하고, 기존 완료 owner·XAML/public/serialization
계약을 보존하면서 R16 owner/partial inventory, R17 no-safe-extraction audit,
R18 contract-safe internal rename, R19 no-safe-folder-move audit, R20 final
source/focused-smoke proof를 수행했다.

Acceptance criteria: 생성·소유·mutable-state writer·Dispose·binding/public
계약과 shortest reading order를 문서화; partial 10개와 Shell composition
field 59개를 실제 evidence로 기록; 독립 owner가 없는 command-surface partial을
재분리하지 않음; 계약 없는 내부 result type 하나만 문맥 중복을 제거하는
이름으로 개명; AGENTS가 고정한 root/folder 경계를 보존; reading route에서
old name과 Shell direct policy call이 0; focused Shell smoke가 `OK`.

Verification: OpenVisionLab solution Debug/Release build 각각 경고 0·오류 0;
OpenVisionReadinessCheck Debug/Release PASS; `Invoke-RefactorAudit.ps1 -Verify`
PASS (`CSharpFiles=847`, `XamlFiles=60`, `PartialDeclarations=110`,
`ProjectCycles=0`, `ShellStorageCalls=0`); `TestDocumentationIndex.ps1` PASS
(`IndexedPaths=285`, `Routes=16`, `RootRedirects=102`); `wpf_shell_host_workspace_empty`
focused UI smoke `2376ms OK`; `git diff --check`는 whitespace error 없이
기존 LF→CRLF conversion notices만 보고했다.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\junior-structure-r16-audit`,
`junior-structure-r17-audit`, `junior-structure-r19-folder-audit`,
`junior-structure-r20-final`, `junior-structure-r20-smoke`.

Boundary: 전체 WPF theme/layout/DPI/monitor/keyboard, camera/SDK, 장시간
native 운전, hosted CI rerun은 실제 실행 전까지 검증하지 않았다. 이 범위에
대해 사용자가 입력할 때까지 기다리지 않고 `소스 코드 기준 검토 완료 /
실제 Runtime UI 검증 필요`로 남긴다. 이 문단은 R20을 닫던 당시의 상태를
기록한 것이며, 2026-09-11 명시적 재개 요청으로 기존 heartbeat는 R21에서
`ACTIVE`가 되었다.

## R1~R15 완료 기록

Status: Complete (R1~R15 scope)

Scope: 사용자 제공 Astra 분석을 현재 `C:\Git\2D\Dev` baseline과 대조하고,
P0 release/exit-code/DisplayManager lifetime, P1 Pipeline drain/Threshold
suggestion ownership, sequential sample-review async deadlock correction,
Pipeline Review smoke ownership/UI contract correction, Shell MVVM state/readiness/
Recipe-save owner moves, Shell/Pipeline Review code-behind boundary audit, and
P2 responsibility-folder cleanup/partial/naming audit를 독립 slice로 실행한
범위.

Acceptance criteria: R1 clean `-SkipLaunch` and no-`-SkipLaunch` gate success, R2 application exit
code capture/return source contract, R3 Pipeline timeout/cancel/drain focused
contract, R4 Threshold session ownership/stale/Undo focused contract plus
targeted `cvr07_threshold_suggestion` UI smoke and Threshold→Blob e2e/route/
PropertyGrid/preprocess suites, R5 completed-owner preservation decision, R6
cross-window DisplayManager lifetime sequence, R8 sequential sample-review
metrics/NG deadlock regression, and R9 Pipeline Review smoke ownership/UI
contract, R10 named Pipeline Review path, R11 Shell state owners, R12 readiness
owner, R13 Recipe save owner, R14 code-behind boundary audit, and R15 Common
folder contracts — 모두 해당 상태와 증거를 위 표에 기록했다.

Verification: 최종 변경 후 solution Debug/Release와
`PipelineViewerScreenshotSmoke` Debug/Release build가 경고 0·오류 0으로
완료됐다. `VisionRecipeRunnerSmoke` Debug/Release
build 각각 경고 0·오류 0; documentation index PASS; readiness PASS;
`--pipeline-review-execution-contract` PASS;
`--threshold-suggestion-session-contract` 7/7 PASS; targeted
`cvr07_threshold_suggestion` UI smoke 6.6초 `OK` (1500x880) with overlay,
panel, buttons, and signal plot AutomationIds; clean candidate
`VerifyReleaseCandidate.ps1` (without `-SkipLaunch`) PASS; two launch cycles,
data-root migration, immutable install snapshot, and migration report were
verified. The installed EXE desktop contract also passed with dynamic monitor
placement, normal `ExitCode=0`, intentional failure `ExitCode=1`, and no
remaining processes. 전체 WPF theme/layout/DPI/keyboard 상태 행렬은 실행하지
않았다. Threshold→Blob e2e 단일 대상과 `e2e`, `route`,
`property-grid-auto-preview`, `preprocess-auto-preview` suites도 모두
`OK`로 완료했다. R10~R14 Shell/Pipeline Review focused smoke and source
boundary evidence, and R15 Common folder Debug/Release/readiness/source checks
were also recorded in the R10~R15 rows above. R15 final checks were
`DocumentationIndex=PASS IndexedPaths=285 Routes=16 RootRedirects=102`,
`REFACTOR_AUDIT=PASS|CSharpFiles=847|XamlFiles=60|PartialDeclarations=110|
ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`, and
`git diff --check` with no whitespace errors.
대표 기본 target 22개와 `perf` suite 4개도 각각 전부 `OK`로 완료했다.
DisplayManager 수명 수정 후에도 순차 Window 대상 3개, 기본 target 22개,
`perf` suite 4개가 모두 `OK`였다. R8 sample-open 뒤 metrics/NG review 순차
smoke 두 묶음도 각각 두 대상 `OK`였고, R9 Fixture/Normalize/Teach/CVR-06/Edge/
Feature·Line·Blob·BentPin·Film review 대상도 `OK`였다. 전체 `--all`은 사용자 입력 의존 대상에서
중단했으며 PASS로 기록하지 않았다.
Dispose 후 ImageViewer race 대상 3개도 전부 `OK`였다.
변경한 `PipelineViewerScreenshotSmoke` Debug 빌드는 exit 0이며 기존
Any CPU→x64 참조 MSB3270과 nullable CS8600(`Program.cs:10181`) 2개 경고,
Release 빌드는 exit 0이며 CS8600 1개 경고가 남는다. 이번 변경은 해당
코드나 경고를 수정하지 않았다.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-clean-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-clean-no-skip-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-completion-status-20260910-final`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\threshold-suggestion-session-contract-20260910-final`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\application-exit-code-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\application-exit-code-20260910-rerun`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-preview-window-max-fixed-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-lifetime-race-fixed-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-default-fixed-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-perf-fixed-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-all-fixed-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r8-sample-open-metrics-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r8-sample-open-ng-metrics-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-fixture-review-20260910-rerun`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-normalize-fixture-review-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-fixture-teach-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-cvr06-20260910`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-edge-ng-20260910-rerun`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-feature-ng-20260910-rerun`,
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r9-ng-metrics-final-20260910`.

Boundary / next dependency: full WPF UI qualification and hosted CI rerun require
a separate execution environment/scope. R16~R20 source/document slices는
사용자 입력이나 UI 조작을 기다리지 않고 진행하되, 독립적인 코드 경계가
증명되지 않으면 no-change 결과를 기록하고 다음 slice 또는 PAUSED로
전환한다. no commit/push/Original change is implied.

## 남은 위험과 범위 밖

- P0 CI 호출 수정은 현재 source에 있고, 임시 clean candidate의 local gate와
  no-`-SkipLaunch` 설치·마이그레이션·desktop launch가 통과했다. GitHub hosted
  CI 재실행은 별도 범위다.
- `Application.Run` exit code는 source와 D: 격리 desktop contract에서
  `0` 정상 종료와 `1` 의도적 실패 전파를 확인했고, clean distribution 설치
  EXE에서도 같은 모니터·종료 계약을 재확인했다.
- Threshold 제안/Undo는 source, WPF 비의존 focused contract, targeted
  `cvr07_threshold_suggestion` UI smoke로 검증했다. 전체 UI 상태·테마·DPI·
  monitor 행렬과 장시간 운전은 아직 검증하지 않았다.
- Pipeline drain은 native task를 먼저 해제하지 않는 안전한 계약을 유지한다.
  영구 정지 native call을 강제 회수하려면 별도 process boundary 요구가
  필요하며 이번 범위에서 임의로 추가하지 않는다.
- 실제 WPF theme/layout/DPI/monitor 상태 행렬, camera/SDK, long-run native resource와
  사용자 first-time workflow는 현재 환경에서 완전히 검증하지 않았다.
- 전체 Shell 생성자 재작성, 프로젝트 재분할, 자동 Threshold 채택, 다중
  파라미터 최적화, installer/signing/update/PLC/MES/LLM 확장은 범위 밖이다.
