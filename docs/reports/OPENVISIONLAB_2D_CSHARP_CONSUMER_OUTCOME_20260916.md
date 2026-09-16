# OpenVisionLab 2D-051 — C# 소비자 typed outcome 실행 예제

작성일: 2026-09-16 KST
상태: **Complete**
저장소: `C:\Git\2D\Dev` (Dev 전용)

## 범위

기존 `TwoDIntegrationTcpSmoke` C# 소비자 예제가 `Success` 같은 단일
boolean을 추측하지 않고 v2 `IntegrationResultV2`의 typed terminal pair를
분기하도록 보완했다. 생산 교환기나 공유 계약 패키지는 바꾸지 않았으며,
명시적 ACK → `await RunAcceptedHandoffAsync` → correlation 확인 →
`await using` 종료 순서를 그대로 유지했다.

## Owner와 call path

```text
TwoDIntegrationTcpSmoke.Program
  -> TwoDIntegrationTcpExchange.AcknowledgeHandoff
  -> await TwoDIntegrationTcpExchange.RunAcceptedHandoffAsync
  -> TwoDIntegrationConsumerExample.AssertCorrelation
  -> TwoDIntegrationConsumerExample.Dispatch
  -> await using dispose of the TCP exchange
```

- 현재 실행/수명 owner는 `src/OpenVisionLab/Core/Integration/TwoDIntegrationTcpExchange.cs`다.
- v2 `Status`, `Outcome`, `RunId`, `Correlation`, `Error` 계약은
  `C:\Git\Shared\OpenVisionLab-Integration-Contracts`가 소유한다.
- 예제 전용 dispatch/correlation owner는
  `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationConsumerExample.cs`다.
- `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationConsumerExampleContract.cs`가
  네 가지 terminal pair, 미분류 조합, transaction 불일치를 검증한다.

## 소비자 예제

```csharp
await using var twoD = new TwoDIntegrationTcpExchange(...);
IntegrationAcknowledgementV2 acknowledgement =
    twoD.AcknowledgeHandoff(transactionId);
IntegrationResultV2 result = await twoD.RunAcceptedHandoffAsync(
    transactionId,
    runtimeBuildManifestPath);
TwoDIntegrationConsumerExample.AssertCorrelation(
    handoff,
    acknowledgement,
    result);
TwoDIntegrationConsumerAction action =
    TwoDIntegrationConsumerExample.Dispatch(result);
```

`Dispatch`는 다음 조합만 허용하며, 나머지는 `InvalidState`로 fail closed한다.

| v2 결과 | 소비자 처리 |
| --- | --- |
| `Completed/Pass` | `QualityPass` |
| `Completed/Ng` | `QualityNg` |
| `Failed/ExecutionError` | `ExecutionError` |
| `Cancelled/Indeterminate` | `Cancelled` |

`AssertCorrelation`는 accepted ACK, Transaction ID, Handoff message ID,
Acknowledgement message ID, 입력/Recipe/host correlation, consumer build,
Completed 결과의 `RunId`를 확인한다. TCP 수신은 기존 계약대로 ACK·Run·Result를
자동 생성하지 않는다.

## 변경 파일

- `tools/TwoDIntegrationTcpSmoke/Program.cs`
- `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationConsumerExample.cs`
- `tools/TwoDIntegrationTcpSmoke/TwoDIntegrationConsumerExampleContract.cs`
- `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
- `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`
- `docs/reports/OPENVISIONLAB_2D_REMAINING_DEVELOPMENT_RECONCILIATION_20260916.md`
- `.proofline/issues/PL-0105.json`

## 검증

- `TwoDIntegrationTcpSmoke` Debug/Release clean-shim build:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d051-csharp-consumer-example-20260916\build-debug-clean-shim.log`,
  `build-release-clean-shim.log` — 0 warning, 0 error.
- 네 가지 outcome·미분류 pair·correlation mismatch contract:
  `...\debug\two-d-integration-consumer-example-contract.json`,
  `...\release\two-d-integration-consumer-example-contract.json` — 각
  `passed=6`, `failed=0`.
- 실제 loopback C# example Debug/Release:
  `...\tcp-debug-clean\...\two-d-tcp-smoke.json`,
  `...\tcp-release-clean\...\two-d-tcp-smoke.json` — `Accepted`,
  `Completed/Pass`, `consumerAction=QualityPass`, `RunId` 보존,
  `receiveDidNotAcknowledge=true`, `receiveDidNotRun=true`,
  `acknowledgeDidNotRun=true`.
- dirty runtime fail-closed:
  `...\tcp-release-dirty\...\two-d-tcp-dirty-runtime-smoke.json` —
  `InvalidIdentity`, ACK/Result 미게시.
- 기존 결과 disposition 회귀 Debug/Release — 각 `passed=9`, `failed=0`:
  `...\result-disposition-debug\two-d-integration-result-disposition-contract.txt`,
  `...\result-disposition-release\two-d-integration-result-disposition-contract.txt`.
- 기존 TCP fault/correlation 회귀 Debug/Release — 두 실행 모두 exit 0:
  `...\tcp-fault-debug-clean\...\two-d-tcp-fault-injection-contract.json`,
  `...\tcp-fault-release\...\two-d-tcp-fault-injection-contract.json`.

## 사후 자체평가와 경계

계약 예제와 실제 loopback 실행 모두 통과했다. Debug fault 회귀의 첫 시도는
Release manifest를 Debug 바이너리에 적용한 검증 구성 오류였고,
`OpenVisionSourceState` 불일치로 중단된 뒤 Debug 전용 clean-shim manifest로
재실행해 통과시켰다. 따라서 그 실패는 제품 결함으로 집계하지 않는다.

현재 증거는 소스/계약/loopback 및 checkout 밖 복사 런타임에 한정된다. 실제
두 대 PC의 방화벽·라우팅, offline 신규 PC 설치, installer/signing/update·rollback,
카메라·하드웨어, 장시간 운전, .NET 4.8 호스트, WPF UI 상호작용은 검증하지
않았다. clean-shim의 `SourceState=clean`은 fake-git으로 만든 manifest identity일
뿐이며 실제 Dev worktree가 clean이라는 뜻이 아니다.

## Durable closure

Status: Complete
Scope: 2D-051의 기존 C# TCP 소비자 예제에 typed four-outcome dispatch,
correlation assertion, await/using lifecycle 설명과 focused contract를 추가했다.
Acceptance: `PL-0105` C1~C4의 현재 evidence가 모두 통과했다.
Evidence: `PL-0105`, 위 D: 드라이브 contract/loopback 보고서.
Boundary: production protocol·shared package ABI와 위의 offline/PC/hardware/UI
qualification은 이 slice의 범위가 아니다.
