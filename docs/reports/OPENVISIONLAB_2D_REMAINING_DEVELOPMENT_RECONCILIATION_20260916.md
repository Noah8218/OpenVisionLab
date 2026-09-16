# OpenVisionLab 2D 잔여 개발항목 정합화·중복 방지 기록

Updated: 2026-09-16 KST
Scope: `C:\Git\2D\Dev`
Parent issue: `PL-0094`
Status: Active — 모든 잔여 항목이 닫힌 상태가 아니며, 독립 실행 가능 항목과 외부 결정 대기 항목을 분리한다.

## 1. 이 문서의 역할

사용자가 승인한 “문서의 잔여 항목을 모두 진행하되 이미 처리한 항목은 중복 처리하지 않는다”는 요청에 따라, 첨부된 52개 계획과 현재 Dev 원장·완료 owner·Handoff를 하나의 작업 경계로 정리한다.

이 문서는 새 기능을 자동으로 활성화하지 않는다. 현재 owner와 증거가 이미 완료된 항목은 다시 구현하지 않고, 새 요구·재현 결함·실패 criterion·dependency boundary가 생길 때만 재개한다.

## 2. 권위 순서와 확인된 기준

1. `AGENTS.md`와 제품 범위/변경/검증 규칙
2. `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
3. `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
4. `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
5. `.proofline/issues/*.json` 및 완료 owner registry
6. 첨부 `OpenVisionLab_2D_Audit_and_Plan.md/json`과 이전 roadmap/tracker

첨부 52개 문서는 읽기 전용 감사·계획 산출물이다. 그 문서의 “미실행 구현 명세”를 현재 구현 증거로 취급하지 않는다.

현재 issue ledger는 106건이며 `resolved=101`, `doing=1`, `blocked=4`이다. 이번
정합화 parent `PL-0094`와 child `PL-0095/2D-047`, `PL-0096/2D-042`,
`PL-0097/2D-037`, `PL-0098/2D-038`, `PL-0099/2D-039`,
`PL-0100/2D-040`, `PL-0101/2D-041`, `PL-0102/2D-044`,
`PL-0103/2D-045`, `PL-0104/2D-046`, `PL-0105/2D-051`, `PL-0106/2D-052`가
추가되었고, 열린 child는
각자의 기존 owner 경계에서 resolved로 닫혔다.

## 3. 첨부 52개 계획과 현재 원장 대조

### 3.1 현재 resolved로 확인된 범위

`2D-001`, `2D-002`, `2D-004`~`2D-022`, `2D-024`~`2D-034`는 현재 Dev issue ledger와 완료 evidence에 연결되어 있다.

대표 연결:

- `2D-001` → `PL-0060`
- `2D-002` → `PL-0059`
- `2D-004`~`2D-022` → `PL-0062`~`PL-0080`
- `2D-024` → `PL-0082`
- `2D-025` → `PL-0083`
- `2D-026`~`2D-034` → `PL-0084`~`PL-0092`
- `2D-037` → `PL-0097`
- `2D-038` → `PL-0098` (기존 Preview/Pipeline drain 계약의 bounded timing/state evidence)
- `2D-039` → `PL-0099` (실행 중 `Window.Close`와 새 shell session 수명 evidence)
- `2D-040` → `PL-0100` (손상 transaction discovery 격리와 typed diagnostic evidence)
- `2D-041` → `PL-0101` (RunRecord orphan의 자동 재실행 차단과 manual recovery Result)
- `2D-044` → `PL-0102` (TCP loopback 지연·부분 전송·재연결·wire 순서·correlation evidence)
- `2D-045` → `PL-0103` (검증한 source bytes와 RunRecord input hash의 교체·부분 쓰기 거부 evidence)
- `2D-046` → `PL-0104` (배포 sidecar와 loaded SDK 파일 length/SHA-256 provenance evidence)
- `2D-051` → `PL-0105` (C# 소비자 typed outcome/correlation/await-dispose example evidence)
- `2D-052` → `PL-0106` (기존 LLM/XML maintenance-mode 검증과 transaction/Undo N/A/deferred 결정)
- `2D-042` → `PL-0096` (bounded multi-process ACK/Run at-most-once evidence; separate
  cancel/crash/reconnect/throughput/hardware boundaries remain open)

이 항목들은 새 defect나 failed criterion 없이 재구현하지 않는다.

### 3.2 현재 blocked로 유지하는 범위

| 계획 | 현재 owner | 차단 이유 | 재개 조건 |
| --- | --- | --- | --- |
| `2D-003` | `PL-0061` | dirty Dev worktree 때문에 전체 RC distribution gate를 실행할 수 없음 | clean verification checkout/staging copy 승인 후 tampered DLL/hash/version/missing-metadata fixture 실행 |
| `2D-023` | `PL-0081` | 일반 XML/JSON/message/artifact의 공통 입력 크기와 typed oversized 오류가 제품 정책으로 정해지지 않음 | per-entry/aggregate ceiling, TCP 상속 여부, 오류 계약 결정 |
| `2D-036` | `PL-0093` | 산술 output preflight는 완료됐지만 memory budget/cap과 low-memory recovery 계약이 없음 | RAM cap, scope, 오류 문구, recovery 정책 결정 |

### 3.3 조건부·최근 admission 판정

다음 항목은 계획상 조건부 범위이거나 이번 cycle에 최근 admission된 경계다. 임의
구현하지 않으며, 실행을 시작할 때 `PL-0094`의 하위 issue 또는 명시된 기존
owner를 먼저 지정한다.

| 계획 | 항목 | 현재 판정 |
| --- | --- | --- |
| `2D-035` | 중간 결과 cache byte budget/eviction | `2D-034` 실측으로 병목이 확인될 때만 admission |
| `2D-043` | 여러 transaction 처리량·대기·메모리 상한 | 운영 데이터와 명시적 상한 필요 |
| `2D-044` | TCP 지연·재연결·순서 역전 | `PL-0102`가 기존 TCP wrapper 주변 loopback fault-injection fixture로 닫힘; two-PC 네트워크·방화벽·전원·hardware qualification은 별도 경계 |
| `2D-047` | RC gate 단계별 evidence summary | 기존 `VerifyReleaseCandidate.ps1`의 성공 요약과 실패-path stage ledger가 `PL-0095`로 한 owner 안에 정리됨; 별도 runner는 만들지 않으며 clean run은 `PL-0061` 경계로 남김 |
| `2D-048` | offline 새 PC 설치·실행·data root | 네트워크 차단 Windows 환경 필요 |
| `2D-049` | 독립 신규 사용자 첫 검사 완결 | `CVR-00`, 실제 참여자 3명 필요 |
| `2D-050` | diagnostic output 축소 실험 | 2D-034 병목 확인 후 선택적으로 admission |
| `2D-051` | C# typed outcome 소비 예제 | `PL-0105`가 기존 TCP C# example owner에서 four-outcome dispatch/correlation/await-dispose contract로 닫힘 |
| `2D-052` | LLM/XML draft→validate→apply→run→undo | `PL-0106`으로 기존 maintenance-mode 안전 경계를 검증하고 N/A/deferred 결정을 기록함; 새 provider/agent/benchmark·transaction/Undo 활성화는 승인 전 금지 |

## 4. 문서상 명시된 제품 잔여 범위

다음은 코드 결함으로 단정하지 않고, 현재 제품·검증 범위의 미완료 경계로 유지한다.

- 모든 UI의 final visual polish
- 모든 Tool/결함군의 OK/NG pair 및 실제 재료 qualification
- LLM full-auto parameter tuning
- 외부 library/package/install 정책의 최종 확정
- DockPanel float/dock/split layout Undo/Redo
- 전체 theme/layout/DPI/input/keyboard/pointer matrix
- camera/GPU/driver/PLC/I/O, 장시간 운전, physical metrology
- low-memory/cancellation 운영 recovery
- clean release distribution, offline new-PC, installer/signing/update/uninstall
- 2D-024의 일반 dataset role/perceptual similarity/N-image label 확장
- 2D-025의 변경 visual stale-state runtime 확인

## 5. 상용 비교·조건부 backlog

- `CVR-00`: 독립 신규 사용자 3명 이상 관찰 — 외부 prerequisite
- `CVR-09`: physical straight-edge/dual-edge qualification — named part/data packet 필요
- `CVR-11`: physical polarity qualification — labelled packet 필요
- `CVR-12`~`CVR-18`: trigger audit만 완료, implementation admission packet 없음
- `CVR-10`: 다른 sub-inspection family는 별도 task 필요

`CVR-01`~bounded `CVR-11`, `CVR-19`, `CVR-20`은 기록된 bounded scope에서 완료됐으며 재구현하지 않는다.

## 6. 중복 방지 규칙

작업 시작 전 반드시 다음을 확인한다.

1. `.proofline/issues`에서 같은 ID/alias/owner가 `doing` 또는 `resolved`인지 확인한다.
2. `docs/admin/CODEBASE_STRUCTURE.md`와 해당 완료 보고서에서 현재 owner/call path를 확인한다.
3. 새 abstraction·wrapper·partial·registry를 만들기 전에 기존 concrete owner를 검색한다.
4. 기존 완료 owner는 새 defect, 명시 요구 변경, failed criterion, dependency boundary 변경 없이는 열지 않는다.
5. 완료 시 issue evidence, D-drive artifact, Handoff의 한 곳에만 canonical summary를 남기고 다른 문서는 링크만 둔다.
6. 제품 결정이 없는 입력 크기·RAM cap·자동 tuning·hardware 정책은 추정으로 구현하지 않는다.

## 7. 다음 실행 경계

이번 cycle에서 독립적으로 admission하여 완료한 경계는 `2D-047`, `2D-042`,
`2D-037`, `2D-038`, `2D-039`, `2D-040`, `2D-041`, `2D-044`, `2D-045`,
`2D-046`, `2D-051`이다.
`2D-047`은 [`PL-0095`](../../.proofline/issues/PL-0095.json)의 기존
`tools/VerifyReleaseCandidate.ps1` owner에서 닫혔고, `2D-042`는
[`PL-0096`](../../.proofline/issues/PL-0096.json)의 기존
`TwoDIntegrationExchange`·`VisionRecipeRunnerSmoke` owner에서 닫혔다.
`2D-052`는 [`PL-0106`](../../.proofline/issues/PL-0106.json)으로 기존
maintenance-mode 경계를 검증하고 N/A/deferred로 닫았다. 새 provider/agent/
benchmark나 transaction/Undo 표면은 활성화하지 않는다. 현재 남은 외부 전제조건
후보는 `2D-048`(offline/new PC), `2D-049`(독립 참가자), `2D-050`(명명된
hardware/long-run 또는 측정 병목)이며, 전제조건이 없으면 구현·검증을 시작하지
않는다.

`PL-0061`, `PL-0081`, `PL-0093`, `CVR-00`, physical qualification, offline PC, hardware/long-run은 각 prerequisite가 준비되기 전까지 blocked로 유지한다. `2D-052`의 저장 실패 transaction/Undo는 명시적 제품 재개 결정 전까지 N/A/deferred다.

## 8. 2D-047 slice completion

`PL-0095`는 기존 `tools/VerifyReleaseCandidate.ps1`의 failure-path 증거 손실을
해결했다. `finally` 기반 summary writer와 단계 ledger를 추가했으며 별도
runner/wrapper는 만들지 않았다.

- 실제 dirty-tree preflight: exit `1`, `Status=FAIL`, `FailureStage=Preflight`
- downstream 단계: `NotRun`, 명시적 `-SkipDebugBuild`/`-SkipLaunch`: `Skipped`
- PowerShell parser 및 failure-summary 정적 계약 검사: `PASS`
- 기존 clean success evidence: `docs/reports/OPENVISIONLAB_RELEASE_CANDIDATE_MAIN_20260911.md`
- 새 failure artifact: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d047-rc-gate-failure-20260916\verification.txt`

이 결과는 새 clean-checkout RC gate를 증명하지 않는다. 그 경계는 기존
`PL-0061`에 남겨 중복 실행하지 않는다.

## 9. 2D-042 slice completion

`PL-0096`은 2D-042의 다중 프로세스 at-most-once 경계를 기존 통합 owner 안에서
검증했다. 새 전역 lock·transport·parallel runner는 추가하지 않았고, ACK 게시의
원자적 target collision만 기존 `InvalidState` 계약으로 변환했다.

- Owner/call path: `VisionRecipeRunnerSmoke.RunConcurrentProcessAsync`가 두 worker
  process를 시작하고, 각 worker가 `TwoDIntegrationExchange.AcknowledgeHandoff` 또는
  `RunAcceptedHandoffAsync`를 호출한다. 실행 owner는 기존 `.2d-run.lock`과 atomic
  handoff/result message 파일이다.
- ACK 결과: 같은 transaction에 대해 `Accepted=1`, `InvalidState=1`, persisted
  acknowledgement=`Accepted`.
- Run 결과: 같은 accepted transaction에 대해 `Completed/Pass=1`,
  `InvalidState=1`, persisted result=`Completed/Pass`, `persistedResultCount=1`.
- 실행 원장: [`.proofline/issues/PL-0096.json`](../../.proofline/issues/PL-0096.json)
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d042-multiprocess-20260916-r3\verification.txt`
  및 동일 run의 `two-d-concurrent-process-smoke.json`
- Runtime manifest의 `sourceState=clean`은 기존 fake-git source-state shim을 사용한
  focused contract build identity이며, 실제 Dev worktree는 dirty 상태였다. 따라서
  clean-checkout release evidence로 해석하지 않는다.
- 이 slice가 증명하지 않는 경계: cancel race, crash/restart recovery, TCP 지연·재연결·
  순서 역전, 처리량·메모리 상한, hardware/field qualification, clean-checkout
  release distribution.

## 10. 2D-037 slice completion

`PL-0097`은 직접 Tool Preview에서 동일 input/output Layer를 지정할 때 결과
publisher가 작업공간의 source layer를 덮어쓰던 공백을 기존 Preview 실행 owner에서
fail-closed로 닫았다. 새 서비스·전역 정책·자동 reroute는 추가하지 않았고, Pipeline의
기존 same-layer warning 계약은 유지했다.

- Owner/call path: `OpenVisionNativeToolDocument.RunPreview` →
  `OpenVisionNativePreviewExecutionBoundary.TryStartSingleInput` →
  `OpenVisionNativePreviewExecutionController.TryCaptureSingleInput` → 기존
  `OpenVisionNativePreviewLayerPublisher.PublishPreviewBitmap`; Arithmetic는
  `RunArithmeticPreview` → `RunArithmetic`에서 A/B 입력과 output 충돌을 같은 경계에서
  차단한다.
- 변경: 입력·출력 이름이 비어 있지 않고 대소문자 무시로 같으면 computation/snapshot/
  publish 전에 `Preview NG / 검사 판정 미평가 / input and output layers must be different
  to preserve the source image`를 반환한다. 별도 layer route는 기존 동작을 유지한다.
- Headless evidence: 기존 `PipelineLayerReferenceInvariantContract`가 Main→Main Run을
  validation warning 1개와 함께 통과시키고 source bytes 불변, 결과 독립 storage,
  별도 Arithmetic B 경로를 확인했다.
- WPF evidence: `wpf_layer_selection_same_input_output_guard`가 Threshold와 Arithmetic
  각각에서 Main/Main 선택 후 Run을 시도해 result 없음, run count 증가 없음,
  `Threshold_Preview` 미생성, Main changed pixels=0, visible NG status를 확인했다.
  화면 캡처:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d037-wpf-20260916-r3\wpf_layer_selection_same_input_output_guard.png`
- D-drive artifacts:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d037-layer-collision-20260916-r1\pipeline-layer-reference-invariant-contract.txt`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d037-wpf-20260916-r3\ui_precheck_report.md`
- 미검증 경계: 다른 theme/layout, DPI 125/150/175/200%, 실제 다중 모니터 배치별
  상호작용, cancel 중 충돌, camera/field hardware, 장시간/low-memory 운전은 이 slice에서
  닫지 않았다.

## 11. 2D-038 slice completion

`PL-0098`은 새 cancellation/deadline 정책을 추가하지 않고, 기존 Preview와
Pipeline drain owner가 실제로 late work를 끝까지 기다린 뒤 idle/`WorkerDrained`로
전환하는 시간과 상태를 기록했다. 첫 번째 Preview 실행은 `Applies=0`으로 실패한
산출물을 보존했고, 같은 Debug binary의 재실행 두 번이 통과하여 harness의 실행
변동을 숨기지 않았다.

- Owner/call path: `OpenVisionNativeToolDocument.RunPreview` →
  `OpenVisionNativePreviewExecutionBoundary.CancelAndDiscard/CompleteOnUi` →
  `OpenVisionNativePreviewExecutionController.ComputeSingleInput`; Pipeline은
  `VisionPipelineExecutionService.WaitForStepCompletionStatusAsync`가
  `TimedOut`/`Canceled`를 구분하고 late result image를 Dispose한다.
- Native Preview evidence: controlled 420ms delegate 진입 후 cancel을 요청하면
  `RunningImmediatelyAfterCancel=True`, `DrainCompleted=True`,
  `TerminalBoundaryState=Idle`, stale/canceled output publish 없음이 확인되었고,
  두 통과 run의 drain 시간은 `428.8ms`와 `431.0ms`였다. 모니터는 `DISPLAY2`
  교차를 기록했다.
- Pipeline evidence: deadline `TimedOut`/`WorkerDrained=True`와 cancellation
  `Canceled`/`WorkerDrained=True`가 late result release 뒤에 기록되었으며, 측정
  값은 각각 `639.9ms`와 `14.0ms`였다.
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d038-drain-20260916-r1\preview-ui\report.txt`
  (첫 실행 실패), `...\preview-ui-r2\report.txt`,
  `...\preview-ui-r3\report.txt`,
  `...\pipeline-contract\pipeline-review-execution-contract.txt`,
  `...\app-debug-embedded-smoke-build.log`, `...\runner-debug-build.log`
- 원장: [`.proofline/issues/PL-0098.json`](../../.proofline/issues/PL-0098.json)
- 미검증 경계: 무한 hang/강제 종료 recovery, 실제 camera/SDK native call,
  low-memory, 장시간 반복, 다른 theme/layout/DPI와 field hardware는 이 bounded
  fixture에서 qualification하지 않았다.

## 12. 2D-039 slice completion

`PL-0099`는 force-stop smoke나 기존 document-level `CancelAndDiscard` 검증을
반복하지 않고, 실제 WPF `Window.Close` 요청이 실행 중 Preview의 기존 수명 owner를
통과하는지를 확인했다. 사전 자체평가에서는 `OnClosed`부터 session/document/native
document Dispose까지의 호출 순서는 이미 명시되어 있었지만, 실행 중 창 닫기와 재시작을
한 케이스로 기록한 증거가 없었다. 따라서 production lifetime 코드는 바꾸지 않고
`OpenVisionLab.DirectSmokeRunner`에 한 focused scenario만 추가했다.

- Owner/call path: `Window.Close` → `OpenVisionShellHostWindow.OnClosed` →
  `OpenVisionShellHostView.Dispose` → `OpenVisionShellHostSessionController.DisposeSession`
  → `OpenVisionShellHostDocumentController.Dispose` →
  `OpenVisionNativeToolDocument.Dispose` → existing Preview boundary Dispose.
- Embedded WPF evidence: active Preview delegate가 실행 중인 상태에서 실제
  `Window.Close`를 요청했고 `WindowClosed=True`, `DispatcherAliveAfterClose=True`,
  `WorkerDrainedAfterClose=True`, `LatePreviewResultPublished=False`,
  `StaleOutputLayerAfterClose=False`를 확인했다. close 경과는 `382.3ms`였고
  선택 모니터 `DISPLAY2`와 창 교차도 기록했다. 같은 scenario에서 두 번째 shell은
  `RestartFreshSession=True`(Main layer/native document/Preview run 없음)였다.
- Process restart evidence: 현재 Release `OpenVisionLab.exe`를 같은
  `OPENVISIONLAB_DATA_ROOT`로 두 번 연속 실행해 두 번 모두
  `CloseMainWindow`/exit `0`/monitor placement를 확인했다. 의도적인 invalid TCP
  smoke는 두 실행 모두 exit `1`로 남겨 정상 종료와 실패 종료를 구분했다.
  `SYSTEM.xml`과 `RECIPE/Default/VISION.xml`이 재실행 뒤 존재했으며, Error log는
  BOM+개행만 기록했다.
- 변경 파일: `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs`
  (테스트 harness route/method만 추가; production Window/session/native lifetime
  owner는 변경하지 않음).
- Build/runtime commands:
  `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform=x64
  -p:WpgCustomBuildEnabled=false -p:OpenVisionLabEnableEmbeddedSmokeRunner=true
  -p:UseAppHost=false -m:1 -nr:false --no-restore`;
  `dotnet bin/x64/Debug/OpenVisionLab.dll --smoke normal-close-restart-lifetime
  --output D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d039-normal-close-restart-20260916-r1\normal-close-restart`;
  `dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Release -p:Platform=x64
  -p:WpgCustomBuildEnabled=false -p:UseAppHost=true -m:1 -nr:false --no-restore`;
  그리고 기존 `Invoke-DesktopExitCodeContract.ps1`를 같은 restart evidence root에서
  두 번 실행했다.
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d039-normal-close-restart-20260916-r1\normal-close-restart\report.txt`,
  `...\app-debug-embedded-build.log`, `...\app-release-build.log`,
  `...\desktop-exit-code-contract\desktop-exit-code-contract.json`,
  `...\desktop-exit-code-contract-restart\pass1-desktop-exit-code-contract.json`,
  `...\desktop-exit-code-contract-restart\pass2-desktop-exit-code-contract.json`,
  `...\desktop-exit-code-contract-restart\restart-consistency.txt`.
- 사후 자체평가: bounded active-Preview close, worker drain, stale-result 차단,
  새 session 초기 상태, 현재 Release 정상 종료·재실행은 통과했다. 실제 camera/SDK
  작업, 무한 hang/강제종료 recovery, 저장 실패·취소 경합, 다른 theme/layout/DPI,
  다중 모니터, qualified Recipe의 변경 저장 후 재복원, hardware/장시간 운전은
  이 slice에서 검증하지 않았으며 별도 prerequisite/issue로 남긴다.
- 원장: [`.proofline/issues/PL-0099.json`](../../.proofline/issues/PL-0099.json)

## 13. 2D-040 discovery isolation slice completion

`PL-0100`은 정상 transaction 두 개와 손상된 `handoff.json` 하나를 같은
격리 fixture에 두고, 기존 `TwoDIntegrationExchange`의 목록 순회가 손상 항목
하나 때문에 중단되지 않는지 확인했다. 사전 자체평가에서 확인한 gap은
`DiscoverHandoffs`가 `ReadHandoffEnvelope` 예외를 항목 단위로 격리하지 않는
것이었다. 기존 owner를 분리하지 않고, additive한
`DiscoverHandoffsDetailed`/`TwoDIntegrationDiscoveryResult` 경계와 TCP wrapper만
추가했으며 legacy `DiscoverHandoffs`는 valid transaction projection을 유지했다.

- Owner/call path: `TwoDIntegrationTcpExchange.DiscoverHandoffs` →
  `TwoDIntegrationExchange.DiscoverHandoffsDetailed` → transaction directory별
  `ReadHandoffEnvelope`; 손상 항목은 `TwoDIntegrationDiscoveryDiagnostic`으로
  수집하고 ACK/Result/delete를 하지 않는다.
- Focused fixture evidence: valid transaction `2`개, malformed transaction
  diagnostic `1`개(`MalformedMessage`), legacy API valid transaction `2`개를
  Debug와 Release runner에서 각각 확인했다. 손상 transaction에는
  Acknowledgement/Result가 생성되지 않았고 자동 삭제도 없었다.
- 변경 파일: `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs`,
  `src/OpenVisionLab/Core/Integration/TwoDIntegrationTcpExchange.cs`,
  `tools/VisionRecipeRunnerSmoke/Program.cs`,
  `tools/VisionRecipeRunnerSmoke/TwoDIntegrationSmoke.cs`.
- Build/runtime commands:
  `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c
  Debug --no-restore -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1
  -nr:false`; `dotnet
  tools/VisionRecipeRunnerSmoke/bin/x64/Debug/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll
  --integration-2d-discovery-isolation-contract
  D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1
  docs/samples/public/Contour_Shapes_Synthetic_OK.png
  docs/samples/Contour_Template_Matching.pipeline.xml`.
  이어서 동일 옵션의 `Release` build와
  `tools/VisionRecipeRunnerSmoke/bin/x64/Release/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll`
  실행도 수행했다. 두 build 모두 오류 `0`, 기존 nullable warning `19`개이며,
  두 fixture 모두 exit `0`이다.
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1\build-debug-r3.log`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1\runtime-r3.log`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1\build-release-r2.log`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1\runtime-release-r2.log`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1\two-d-discovery-isolation-20260916-043041-e90ffb038aeb439bacc197e7a196d226\two-d-discovery-isolation-contract.json`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d040-discovery-isolation-20260916-r1\two-d-discovery-isolation-20260916-043043-a81b1790af474ac2a792de423c32bc4d\two-d-discovery-isolation-contract.json`.
- 사후 자체평가: 한 항목의 malformed/read failure가 뒤의 정상 목록을 막지 않고
  typed diagnostic으로 남는 bounded 계약은 통과했다. 실제 권한 거부·동시 writer,
  crash/reconnect, 처리량, offline PC, hardware와 UI 전체 행렬은 이 slice에서
  검증하지 않았으며 별도 경계로 남긴다.
- 원장: [`.proofline/issues/PL-0100.json`](../../.proofline/issues/PL-0100.json)

## 14. 2D-041 RunRecord/Result process-recovery slice completion

`PL-0101`은 `RunRecord`과 `Result`가 별도 immutable 파일로 게시되는 기존
`TwoDIntegrationExchange` owner에서, `RunRecord` 게시 직후 프로세스가 중단된
경계를 닫았다. 사전 자체평가에서 확인한 gap은 orphan record가 남은 상태에서
다음 실행이 recipe를 다시 실행할 수 있다는 점이었다. 기존 `.2d-run.lock`,
RunRecord writer, Result publisher를 재분리하지 않고 다음 최소 정책을 적용했다.

- `artifacts/2d-run-record.json`이 있고 Result가 없으면 source/recipe 상대경로,
  SHA-256, byte length를 Handoff artifact와 검증한다.
- 검증 성공·실패와 무관하게 자동 재실행하지 않고, 기존 contract가 허용하는
  `Failed`/`executionError` + `executionFailed` terminal Result와
  `Automatic rerun is blocked; manual recovery is required.` 메시지를 게시한다.
  RunRecord는 삭제하지 않으며, 이후 실행은 기존 Result 존재 guard에서 거부된다.
- 내부 test-only pause 및 one-shot Result-write failure seam은 deterministic
  fixture에만 사용하고 정상 실행 경로에는 영향을 주지 않는다. 별도 recovery
  service, database, global lock, Result 합성/추측은 추가하지 않았다.

- Owner/call path: `TwoDIntegrationExchange.RunAcceptedHandoffAsync` →
  `RunAcceptedHandoffCoreAsync` → orphan preflight/`DescribeOrphanedRunRecord` →
  기존 `PublishFailedResult` 또는 `WriteRunRecord` → `WriteNewMessage(Result)`.
  Process fixture child는 `RunRunRecordRecoveryWorkerAsync`에서 기존 Run owner를
  호출하고, 부모가 marker 직후 child를 kill한 다음 동일 exchange root를 재오픈한다.
- Focused Debug evidence: 정상 `Completed/Pass`, Result-write failure의
  `Failed/ExecutionFailed`·RunRecord 삭제·Result 1개, `markerObserved=true`와
  `killedWorkerExitCode=-1`, orphan record 유지, 첫 restart의
  `Failed/ExecutionError/ExecutionFailed` recovery Result 1개, 두 번째 restart
  거부와 Result byte hash 불변을 모두 확인했다.
- Focused Release evidence: Debug와 동일한 결과를 Release runner에서 반복했다.
- 변경 파일: `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs`,
  `tools/VisionRecipeRunnerSmoke/Program.cs`,
  `tools/VisionRecipeRunnerSmoke/TwoDIntegrationSmoke.cs`.
- Build commands (clean-source-state shim은 runtime identity fixture에만 사용):
  `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c
  Debug --no-restore -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1
  -nr:false` 및 동일 옵션의 `Release` build. 두 build 모두 오류 `0`, 기존
  nullable warning `19`개였다.
- Runtime commands:
  `dotnet tools/VisionRecipeRunnerSmoke/bin/x64/Debug/net8.0-windows7.0/VisionRecipeRunnerSmoke.dll
  --integration-2d-run-record-recovery-contract <evidenceRoot>
  docs/samples/public/Contour_Shapes_Synthetic_OK.png
  docs/samples/public/Public_Contour_Shapes.pipeline.xml
  bin/x64/Debug/openvisionlab.runtime.json` 및 동일한 Release runner/manifest.
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d041-run-record-recovery-20260916-r1\build-debug-r5-clean-shim.log`,
  `...\runtime-debug-r6-clean-shim.log`,
  `...\build-release-r1-clean-shim.log`,
  `...\runtime-release-r1-clean-shim.log`,
  `...\two-d-run-record-recovery-20260916-044816-d389dbff0b1c45bbb69ce9afae4a1e98\two-d-run-record-recovery-contract.json`,
  `...\two-d-run-record-recovery-20260916-044900-16f5edc4461443969d73b9772806fb7b\two-d-run-record-recovery-contract.json`.
- 사후 자체평가: 정상 게시, write failure 정리, 실제 process kill 후 orphan 보존,
  자동 재실행 차단, terminal recovery Result, second-restart 중복 방지는 통과했다.
  실제 전원 장애/파일시스템 손상, 비협조 native worker, TCP reconnect/순서 역전,
  처리량·메모리 상한, offline PC, hardware/long-run, WPF UI·DPI 행렬은 이
  bounded slice에서 검증하지 않았으며 별도 경계로 남긴다.
- 원장: [`.proofline/issues/PL-0101.json`](../../.proofline/issues/PL-0101.json)

## 15. 2D-044 TCP loopback fault and correlation slice completion

`PL-0102`는 기존 `TwoDIntegrationTcpExchange`와 shared
`TcpIntegrationClient`/`TcpIntegrationServer`의 실제 owner를 재구현하지 않고,
문서에 남아 있던 TCP 지연·부분 전송·재연결·순서 역전·잘못된 응답 identity
경계를 2D-owned loopback fixture로 닫았다. 사전 자체평가에서 기존 정상
push/pull smoke와 shared transport unit test에는 이 조합의 2D 증거가 없음을
확인했다. production protocol, shared package, Original repository는 바꾸지
않았다.

- Owner/call path: `TwoDIntegrationTcpExchange.PushTransactionAsync` → shared
  `TcpIntegrationClient` → loopback `ChunkingTcpProxy` → shared
  `TcpIntegrationServer` → `TcpTransactionStore`의 staging/immutable merge;
  pull 경로는 `TwoDIntegrationTcpExchange.PullTransactionAsync` → scripted peer
  wire frame → 기존 `TcpTransactionStore.ReceiveFilesAsync`이다.
- Reconnect/partial case: 첫 push 연결은 4 KiB chunk 단위로 일부만 전달한 뒤
  proxy가 연결을 끊었고, 기존 retry가 두 번째 request로 재연결했다. 결과는
  `Reconnect=2`, `byteIdentical=true`, `proxyChunkCount=66/67` 및 수신 측
  `Acknowledgement`/`Result` 미생성으로 확인했다.
- Wire-order case: fake peer가 `result.json` → `acknowledgement.json` →
  `handoff.json` 순서로 1 KiB chunk/지연 전송했다. receiver는 immutable snapshot을
  materialize했으며 기존 Handoff/ACK/Result correlation이 유지되고, 명시적
  재실행은 기존 `Result` guard의 `InvalidState`로 거부되었고 Result bytes는
  불변이었다.
- Wrong-correlation case: fake peer가 HMAC-valid 응답의 `requestId`를 바꾸어
  보냈고, 기존 client가 `correlationMismatch`로 거부했으며 destination에는
  transaction이 게시되지 않았다. TCP 수신은 ACK/Run을 자동 실행하지 않는다.
- 변경 파일: `tools/TwoDIntegrationTcpSmoke/Program.cs`,
  `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationTcpFaultInjectionContract.cs`
  (검증 harness/fixture만 추가; `TwoDIntegrationTcpExchange`와 shared package
  production owner는 변경하지 않음).
- Build/runtime commands: fake-git clean-source-state shim을 PATH에 둔 상태에서
  `dotnet build tools/TwoDIntegrationTcpSmoke/TwoDIntegrationTcpSmoke.csproj -c
  Debug --no-restore` 및 `Release --no-restore`를 실행했고 두 build 모두 오류와
  warning `0`이다. 각 runner에
  `--fault-injection-contract <evidenceRoot> docs/samples/public/Contour_Shapes_Synthetic_OK.png
  docs/samples/public/Public_Contour_Shapes.pipeline.xml bin/<configuration>/openvisionlab.runtime.json`를
  실행해 exit `0`을 확인했다.
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d044-tcp-fault-20260916-r1\build-debug-r4-clean-shim.log`,
  `...\runtime-debug-r4-clean-shim.log`,
  `...\build-release-r1-clean-shim.log`,
  `...\runtime-release-r1-clean-shim.log`,
  `...\two-d-tcp-fault-20260916-051531-a43c1f9b81f545c780e12795455b5a7f\two-d-tcp-fault-injection-contract.json`,
  `...\two-d-tcp-fault-20260916-051609-608d726f3c2e4ca2a0c607409099f998\two-d-tcp-fault-injection-contract.json`.
- 사후 자체평가: 계획된 loopback partial/delay/reconnect, Result-before-ACK wire
  delivery, existing-result guard, and wrong-response correlation 기준은 Debug와
  Release에서 통과했다. 두 PC의 firewall/routing/DNS/VPN, 실제 전원 장애,
  native worker, 처리량·메모리 상한, offline 새 PC, camera/hardware, WPF
  theme/layout/DPI/monitor/input qualification은 검증하지 않았고 별도
  prerequisite/issue로 남긴다.
- 원장: [`.proofline/issues/PL-0102.json`](../../.proofline/issues/PL-0102.json)

## 16. 2D-045 input hash/decode generation slice completion

`PL-0103`은 기존 `TwoDIntegrationExchange.RunAcceptedHandoffAsync`의 입력
세대 경계를 닫았다. 사전 자체평가에서 `ReadHandoff`/공유 validator가 source
경로를 해시한 뒤 `Cv2.ImRead(sourcePath)`가 같은 경로를 다시 열고 있어, 로컬
writer가 검증과 decode 사이에 원자 교체하면 A hash와 B 픽셀이 섞일 수 있음을
확인했다. 32×32 BMP A(평균 32)와 같은 길이 B(평균 224), 그리고 잘린 파일을
사용한 deterministic fixture로 기준선을 먼저 보존한 뒤, 기존 owner 안에 source
전용 verified-byte snapshot과 길이/hash 재검증을 추가했다.

- Owner/call path: `TwoDIntegrationExchange.ReadHandoff` →
  `IntegrationContractValidator.ValidateArtifactFile`(기존 사전 검증) →
  `RunAcceptedHandoffCoreAsync`의 run-record preflight →
  `ReadVerifiedArtifactBytes`(동일 byte array의 길이·SHA-256 재검증) →
  `Cv2.ImDecode(sourceBytes, ImreadModes.Unchanged)` →
  `VisionRecipeRunner` → `CreateRunRecord`. Mutable source state의 유일한
  writer는 exchange transaction artifact이며, RunRecord는 기존
  `CreateRunRecord`가 Handoff identity에서 작성한다. Recipe XML snapshot
  (`PL-0062`)과 Mat lifetime (`PL-0086`) owner는 다시 열지 않았다.
- 기준선: Debug pre-fix fixture에서 immutable A는 `Completed/Pass`와
  `MeanValueAvg=32`였지만, validation pause 뒤 same-length A→B atomic
  replacement는 `Completed/Pass`, `MeanValueAvg=224`를 게시하면서 RunRecord에
  A SHA-256을 남겼다. truncated replacement도 `Failed/ExecutionError` Result를
  게시했다. 이 결과는 `baselineMismatchObserved=true`인 report로 보존했다.
- 수정: source 하나만 `File.ReadAllBytes`로 읽고 같은 배열에서 byte length와
  SHA-256을 확인한다. 불일치는 `ArtifactHashMismatch` 또는
  `ArtifactLengthMismatch`로 decode/Result/RunRecord 전에 거부하며, 기존 ACK,
  Run lease, Result guard, recipe path, `Cv2.Mat` dispose owner, protocol은
  유지했다. 새 protocol/package/parallel exchange owner는 만들지 않았다.
- Fixture/route: `VisionRecipeRunnerSmoke`
  `--integration-2d-input-hash-decode-contract <evidenceRoot> <runtimeManifest>`;
  immutable, same-length atomic replacement, partial/truncated replacement를
  각각 isolated transaction에서 실행하고 Result/RunRecord 존재 여부와
  `MeanValueAvg`/source SHA를 기록한다.
- Build/runtime: fake-git clean-source-state shim을 PATH에 둔 상태에서 Debug
  build는 오류 `0`, warning `0`; Release build는 오류 `0`, 기존 nullable
  warning `19`개였다. Debug와 Release runner 모두 exit `0`이며 immutable
  identity/mean, atomic `ArtifactHashMismatch`, partial
  `ArtifactLengthMismatch`, replacement 시 Result/RunRecord 미게시를 통과했다.
- 기존 `--integration-2d` regression smoke도 같은 변경 후 Debug와 Release에서
  exit `0`을 유지했다. Good/Bad `Pass/Ng`, 명시적 Rejected 실행 차단, tamper
  rejection, metric/coordinate round-trip, concurrent Run lease 보호가 그대로
  통과했다(`normal-integration/runtime-debug.log`,
  `normal-integration/runtime-release.log`).
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d045-input-hash-decode-20260916-r1\runtime-debug-baseline-r4.log`,
  `...\two-d-input-hash-decode-20260916-054717-20a7e3fdc7f04ca5b28ece9e1a53b197\two-d-input-hash-decode-contract.json`,
  `...\build-debug-clean-shim-final-r1.log`, `...\runtime-debug-final-r1.log`,
  `...\two-d-input-hash-decode-20260916-054906-8b9f72833a1742908c4366a986085a13\two-d-input-hash-decode-contract.json`,
  `...\build-release-clean-shim-final-r1.log`, `...\runtime-release-final-r1.log`,
  `...\two-d-input-hash-decode-20260916-054951-e9847a7248b54b34b5f8f27d339a9d30\two-d-input-hash-decode-contract.json`.
- 사후 자체평가: 계획된 immutable/atomic same-length/partial replacement
  acceptance와 Debug/Release focused build/runtime 기준은 통과했다. 이 조치는
  source decode byte generation만 닫으며, locator evidence exporter의 decode 후
  late `sourcePath` 재읽기, recipe/locator template의 별도 writer 경합, 대형
  이미지 메모리 예산, privileged writer, 전원/파일시스템 손상, camera/hardware,
  WPF UI/DPI/monitor qualification은 검증하지 않았다. 이 범위는 별도
  prerequisite/issue로 유지한다.
- 원장: [`.proofline/issues/PL-0103.json`](../../.proofline/issues/PL-0103.json)

## 17. 2D-046 deployed SDK provenance slice completion

`PL-0104`는 저장소 밖에서 실행되는 runtime이 실제로 로드한 SDK 파일과
source provenance를 정확히 말하는지 닫았다. 사전 자체평가에서는 기존
`ResolveVisionSdkIdentity`가 repository-relative manifest만 읽고 loaded DLL의
length/SHA-256을 비교하지 않는 것과, checkout-free copy가 `Manifest=unavailable`로
남는 기준선을 먼저 보존했다.

- Owner/call path: `VisionRecipeRunner` →
  `VisionPipelineExecutionPlan.Create/CreateProvenance` →
  `ResolveVisionSdkIdentity`. 배포 root의 `sdk-manifest.json`을 먼저 찾고,
  없을 때만 repository fallback을 사용한다. sidecar의 모든 declared SDK file은
  contained path, length, SHA-256을 확인하며 loaded `OpenVisionLab.Vision2D.dll`
  entry와 embedded `OpenVisionVisionSdkManifestSha256` anchor도 일치해야 한다.
- Packaging: `src/OpenVisionLab/OpenVisionLab.csproj`가 sidecar를 build/publish
  output에 복사하고 manifest hash를 assembly metadata로 고정한다.
  `tools/BuildCleanRuntime.ps1`는 sidecar를 required payload와
  `clean_runtime_manifest.json`의 `VisionSdkManifestSha256`로 기록하며,
  `tools/TestReleaseDistribution.ps1`는 sidecar/package identity와 전체 payload를
  검증한다. SDK DLL contents, public provenance fields, protocol, shared package는
  바꾸지 않았다.
- Self-evaluation correction: PowerShell이 생성한 BOM-bearing
  `clean_runtime_manifest.json`을 첫 canonical copy가 거부한 실패를 보존했다.
  JSON parser를 BOM-safe string 경로로 최소 수정한 뒤 같은 full manifest가
  `match`로 통과했다. 이는 제품 결함을 숨기지 않고 fixture 기준선을 갱신한
  후속 검수다.
- Debug/Release fixture: `match`는 version `3.0.0`, SDK commit
  `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`, loaded Vision2D length `355328`,
  SHA-256 `91A6D4DB01AA2E97F54BB5FF7A61FB0D0404432D0D86821E889350CCA5007B62`를
  확인했다. sidecar missing은 명시적 `Manifest=unavailable`로 통과했고,
  metadata/length/loaded-DLL/package anchor 변조는 모두 `ManifestStatus=mismatch`로
  차단됐다. 기존 PL-0008 pipeline provenance Debug regression도 통과했다.
- Build/distribution: Dev clean runtime build는 오류/경고 `0/0`으로 완료되었고,
  Release clean runtime/archive와 `TestReleaseDistribution.ps1 -SkipLaunch`가
  `ReleaseDistributionCheck=PASS`(payload `76`, archive SHA-256
  `7915E913F99BFC1F93EDD308E1E2F67FC67F73EA0BF8B9A7C417F401FE672D7E`)로
  완료됐다. Release PL-0008 broad runner는 sidecar 유무와 무관하게 기존
  sample-validation 단계에서 중단되었으므로 이 slice의 acceptance 증거로
  사용하지 않고 별도 인접 경계로 남겼다.
- D-drive evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d046-sdk-deployment-20260916-r7-build-dev.log`,
  `...\2d046-sdk-deployment-20260916-r7-build-release-r2.log`,
  `...\2d046-sdk-deployment-20260916-r7-test-release-distribution-skip-launch.log`,
  `...\2d046-sdk-deployment-20260916-r8\debug-match\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\debug-missing\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\release-match\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\release-missing\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\release-metadata-mismatch\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\release-length-mismatch\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\release-loaded-dll-mismatch\evidence\sdk-deployment-provenance-contract.json`,
  `...\2d046-sdk-deployment-20260916-r8\release-package-mismatch\evidence\sdk-deployment-provenance-contract.json`.
- 미검증/경계: 실제 다른 물리 PC·offline 설치, installer/signing/update/rollback,
  privileged writer, multi-process file replacement, SDK 내부 동작, camera/GPU/
  hardware, 장시간 운전, 전체 WPF UI/DPI/theme matrix는 이 local copied-folder
  증거로 주장하지 않는다.
- 원장: [`.proofline/issues/PL-0104.json`](../../.proofline/issues/PL-0104.json)

## 18. 2D-051 C# consumer typed outcome example completion

`PL-0105`는 기존 `TwoDIntegrationTcpSmoke` C# 소비자 예제에서 `Success`
boolean에 의존할 수 있는 가시적 gap을 닫았다. 생산 교환기·공유 v2 DTO·TCP
transport owner는 유지하고 예제 전용 dispatch/correlation contract만 추가했다.

- Owner/call path: `TwoDIntegrationTcpSmoke.Program` → explicit
  `AcknowledgeHandoff` → `await RunAcceptedHandoffAsync` →
  `TwoDIntegrationConsumerExample.AssertCorrelation` → typed `Dispatch` →
  `await using` exchange dispose. Receive/discovery는 계속 ACK/Run/Result를
  자동 생성하지 않는다.
- Typed outcome: `Completed/Pass → QualityPass`, `Completed/Ng → QualityNg`,
  `Failed/ExecutionError → ExecutionError`,
  `Cancelled/Indeterminate → Cancelled`; 미분류 조합과 transaction mismatch는
  fail-closed다.
- Debug/Release example contract는 각 `passed=6`, `failed=0`이다. 실제
  loopback example은 Debug/Release 모두 `Accepted`, `Completed/Pass`,
  `consumerAction=QualityPass`, `RunId`와 correlation, receive non-execution을
  기록했다. dirty runtime은 `InvalidIdentity`로 ACK/Result를 게시하지 않았다.
- 기존 result-disposition contract(각 `passed=9`)와 TCP fault/correlation
  contract(Debug/Release)도 통과했다. Debug fault의 첫 실패는 Release
  manifest를 Debug 바이너리에 적용한 구성 오류였고, Debug 전용 clean-shim
  manifest로 재실행해 통과했다.
- 변경 파일: `tools/TwoDIntegrationTcpSmoke/Program.cs`,
  `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationConsumerExample.cs`,
  `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationConsumerExampleContract.cs`.
- D-drive evidence와 상세 보고서:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d051-csharp-consumer-example-20260916`,
  `docs/reports/OPENVISIONLAB_2D_CSHARP_CONSUMER_OUTCOME_20260916.md`.
- 미검증 경계: 두 대 PC 네트워크/offline 신규 PC 설치,
  installer/signing/update·rollback, .NET 4.8 host, camera/hardware,
  long-run, WPF UI interaction은 이 slice에서 주장하지 않는다.
- 원장: [`.proofline/issues/PL-0105.json`](../../.proofline/issues/PL-0105.json)

## 19. 2D-052 LLM/XML maintenance-mode boundary completion

`PL-0106`은 2D-052를 새 LLM 기능 개발이 아닌 기존 compatibility surface의
안전 경계 검증으로 한정해 닫았다. P196과 stable feature contract가 planned
LLM expansion을 동결하고 있으므로 provider/agent/benchmark/자동 실행은
추가하지 않았다.

- Owner/call path: `RecipeCommandSurface.ImportLlmXmlDraft` →
  `OpenVisionRecipeLlmDraftValidationService` →
  `OpenVisionRecipeDependencyReviewService` →
  `OpenVisionRecipeLlmDraftReviewOwner` → `VisionPipelineStorage`.
  Draft/load/validate/review/diff는 읽기 전용이며, Import와 Preview/Run은
  명시적 별도 명령이다.
- Current-source evidence: LLM draft review owner contract `3/3`,
  `OpenVisionReadinessCheck` 13개 계약, Guided Setup WPF target `OK`, review
  bundle dry-run target `OK`가 유지된다. Guided Setup target은 stale draft,
  invalid/custom `Inspection.*`, dependency/intent gate, no-auto-Run/layer/
  route, explicit import 회복을 포함한 현재 source assertions를 실행했다.
- Storage boundary: Import는 기존 active Pipeline을 덮지 않고 새 고유 Pipeline을
  저장한 뒤 active pointer를 갱신한다. 그러나 의존 파일/참조 이미지를 저장보다
  먼저 복사하며 Save와 pointer write는 하나의 transaction이 아니고 Import Undo
  명령도 없다. 따라서 failed persistence rollback과 Undo는 검증된 완료가
  아니라 maintenance-mode `N/A/deferred`다.
- WPF limitation: `wpf_shell_host_recipe_manager_summary`는 현재 없는
  `HostRecipeManagerCommandStrip` AutomationId를 기대해 `NG`, `0x0` capture를
  남겼다. 이는 LLM 경로 결함으로 해석하지 않고 별도 stale smoke 제한으로
  보존한다.
- New-PC boundary: 단일 `DISPLAY2` 환경에서만 실행했다. 실제 offline/new-PC
  설치는 장비가 없어 검증하지 않았으며 `2D-048`은 계속 blocked다.
- 상세 보고서: [`OPENVISIONLAB_2D_LLM_XML_BOUNDARY_20260916.md`](OPENVISIONLAB_2D_LLM_XML_BOUNDARY_20260916.md)
- 원장: [`.proofline/issues/PL-0106.json`](../../.proofline/issues/PL-0106.json)

새 transaction/Undo 계약은 제품 방향을 명시적으로 재개할 때만 별도 issue로
admit한다. 그때는 asset staging/cleanup, Pipeline XML·active pointer 원자성,
fault injection, 실패 후 재열기와 Undo snapshot을 먼저 정의한다.

## 20. Completion evidence

- 현재 원장: `.proofline/issues/PL-0094.json`
- 현재 2D-047 실행 원장: `.proofline/issues/PL-0095.json`
- 현재 2D-042 실행 원장: `.proofline/issues/PL-0096.json`
- 현재 2D-037 실행 원장: `.proofline/issues/PL-0097.json`
- 현재 2D-038 실행 원장: `.proofline/issues/PL-0098.json`
- 현재 2D-039 실행 원장: `.proofline/issues/PL-0099.json`
- 현재 2D-040 실행 원장: `.proofline/issues/PL-0100.json`
- 현재 2D-041 실행 원장: `.proofline/issues/PL-0101.json`
- 현재 2D-044 실행 원장: `.proofline/issues/PL-0102.json`
- 현재 2D-045 실행 원장: `.proofline/issues/PL-0103.json`
- 현재 2D-046 실행 원장: `.proofline/issues/PL-0104.json`
- 현재 2D-051 실행 원장: `.proofline/issues/PL-0105.json`
- 현재 2D-052 실행 원장: `.proofline/issues/PL-0106.json`
- 현재 상태 권위: `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- owner 권위: `docs/admin/CODEBASE_STRUCTURE.md`
- 첨부 계획: `C:/Users/USER/Downloads/OpenVisionLab_2D_Audit_and_Plan.md`, `OpenVisionLab_2D_tasks.json`
- 테스트 산출물: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev`

이 문서는 모든 항목이 완료되었다는 보고서가 아니다. 처리된 항목과 처리하지 않은 항목을 분리해 이후 예약 실행·수동 작업이 같은 owner를 다시 구현하지 않도록 하는 기준 문서다.
