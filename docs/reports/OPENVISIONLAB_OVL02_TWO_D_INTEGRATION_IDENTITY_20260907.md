# OVL-02 TCP 실제 빌드 신원과 Transaction 실행 진입 리팩토링

작성일: 2026-09-07 KST  
상태: **Complete** (OVL-02 독립 slice)

## 범위

`TwoDIntegrationExchange.RunAcceptedHandoffAsync`의 거래별 실행 진입을
원자적으로 보호했다. 기존 코드는 `result.json`의 존재를 확인한 뒤 레시피를
실행했으므로, 같은 Transaction ID에 두 공용 API 호출이 동시에 들어오면 두
호출이 모두 검사 경계까지 진행할 수 있었다. 이제 거래 디렉터리의
`.2d-run.lock`을 `FileShare.None`으로 열어 lease를 얻은 호출만
`Result` 확인, 실행, Run Record/Result 기록을 수행한다. 다른 호출은
`IntegrationErrorCode.InvalidState`로 실패하고, lease는 정상 종료 시 해제된다.

변경 파일:

- `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs`
- `tools/VisionRecipeRunnerSmoke/TwoDIntegrationSmoke.cs`
- `docs/LLM_DOCUMENT_INDEX.json`

`TwoDIntegrationBuildIdentity.cs`와 `OpenVisionTcpIntegrationController.cs`는
변경하지 않았다. 현재 구현은 공개 ACK/Run 경로가 모두 같은 private
`PublishAcknowledgement`/`RunAcceptedHandoffCoreAsync`로 수렴하고,
`TwoDIntegrationBuildIdentity`가 실제 `ApplicationAssembly`와 Handoff의
expected identity를 `IntegrationRuntimeBuildVerifier`에 전달한다. 따라서
identity 값 공급원을 요청 payload로 바꾸는 별도 수정은 필요하지 않았다.

## 구조 증거

리팩토링 전 실행 흐름:

```text
ReadHandoff → 실제 assembly/manifest 재검증 → ReadAcknowledgement
→ File.Exists(result.json) 확인
→ 레시피 실행
→ Run Record/Result 기록
```

`File.Exists`와 레시피 실행 사이에 독점 owner가 없어 두 호출이 같은
검사에 진입할 수 있었다.

현재 실행 흐름:

```text
ReadHandoff → 실제 assembly/manifest 재검증 → ReadAcknowledgement
→ 거래별 .2d-run.lock 독점 lease 획득
→ Result 존재 확인
→ 레시피 실행 및 Run Record/Result 기록
→ lease 해제
```

`RunLease`는 `OpenOrCreate`와 `FileShare.None`을 사용하므로 프로세스 경계를
넘어 같은 파일 핸들을 동시에 얻을 수 없다. 공유 위반만
`InvalidState`로 변환하고, 디스크/경로와 같은 다른 I/O 오류는 원래 오류로
남긴다. 실패·취소 Result를 기록하는 경로도 같은 lease 안에서 실행되므로
두 호출이 서로의 Result를 덮어쓰지 않는다.

## 합격 기준과 결과

| 기준 | 결과 | 근거 |
| --- | --- | --- |
| 실제 version/source commit/source state가 다른 요청을 ACK 전에 거부 | 통과 | `integration-clean-final-1.log`: manifest identity tamper=`InvalidIdentity`, target mismatch=`CorrelationMismatch` |
| runtime manifest 누락·assembly SHA 변조를 ACK 전에 거부 | 통과 | `integration-clean-final-1.log`: missing=`ArtifactMissing`, hash tamper=`ArtifactHashMismatch` |
| 정상 Handoff→ACK→Run→Result 유지 | 통과 | `integration-clean-final-1.log`의 Good=`Pass`, Bad=`Ng`; `tcp-integration-clean-final-1.log`의 TCP=`Pass` |
| 같은 Transaction ID 동시 Run의 실제 진입을 1회로 제한 | 통과 | `two-d-concurrent-run-smoke.json`: `leaseObserved=true`, 첫 Run=`Completed/Pass`, 두 번째=`InvalidState`, `persistedResultCount=1` |
| 완료된 Transaction 재요청을 재검사하지 않음 | 통과 | 같은 JSON의 `completedRerequestRejected=true` |
| TCP 전송 후 수신이 ACK/Run을 자동 실행하지 않음 | 통과 | `tcp-integration-clean-final-1.log` 및 `two-d-tcp-smoke.json`의 receive/ack explicit checks |

## 검증

실제 Dev worktree의 dirty 상태를 확인하는 Debug 빌드는 경고·오류 없이
통과했고, 같은 상태에서 ACK/Run identity 검사가 fail-closed인지 별도로
확인했다.

```text
dotnet build .\tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

긍정적인 정상·동시성 실행은 소스 변경을 포함한 Release 산출물을 검증하기
위해 테스트 전용 Git shim으로 `status` 결과만 비워 생성했다. 이 과정은
런타임 contract의 정상 경로를 열기 위한 합성 Clean identity이며, 공개
release의 repository cleanliness를 증명하지 않는다.

```text
dotnet build .\tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
dotnet build .\tools\TwoDIntegrationTcpSmoke\TwoDIntegrationTcpSmoke.csproj -c Release -p:Platform=x64 -p:WpgCustomBuildEnabled=false -m:1 -nr:false
```

추가로 실제 OpenVisionLab assembly 옆의 기본 manifest를 사용한
`IntegrationRuntimeBuildVerifier.LoadQualifiedTargetIdentity(..., null)` 호출이
현재 identity를 반환하는 것을 확인했다. 이는 공개 기본 overload가
실제 assembly/manifest verifier를 사용하는지 확인하는 package-level
검증이며, 별도 EXE UI 조작은 포함하지 않는다.

증거:

- [Debug 빌드](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/build-debug-final-1.log)
- [합성 Clean 정상 경로 빌드](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/build-clean-synthetic-final-1.log)
- [Dirty identity fail-closed smoke](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/integration-dirty-final-1.log)
- [정상·identity·동시 Run smoke](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/integration-clean-final-1.log)
- [동시 Run 상세 JSON](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/smoke-clean-final/two-d-integration-20260907-043909-ea31bee419c347d6bfe44a51fd4d4a84/two-d-concurrent-run-smoke.json)
- [TCP smoke 빌드](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/build-tcp-clean-synthetic-final-1.log)
- [TCP smoke 실행](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/tcp-integration-clean-final-1.log)
- [TCP 결과 JSON](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/tcp-clean-final/two-d-tcp-20260907-043930-96e0413a08bd4960b6411a30c3b568e2/two-d-tcp-smoke.json)
- [공개 기본 verifier 확인](D:/OpenVisionLab-TestData/OpenVisionLab_Dev/refactor-ovl02-20260907/identity-public-default-final-2.log)

## 경계

동시성 smoke는 한 프로세스 안의 두 공용 API Task와 실제 lease 점유를
검사했다. lease 구현은 OS 파일 공유 경계를 사용하므로 프로세스 간에도
같은 원칙을 적용하지만, 두 개의 별도 OpenVisionLab EXE가 하나의 새
Transaction을 동시에 실행하는 시나리오는 이번 실행에서 직접 조작하지
않았다. 비정상 프로세스 종료 뒤 재개 정책은 lock 파일이 남아도 다음
호출이 `OpenOrCreate`로 다시 lease를 취득할 수 있게 한 수준이며, 별도의
crash/restart 복구 테스트는 수행하지 않았다.

WPF UI 레이아웃을 변경하지 않았으므로 UI 상태·theme·DPI 매트릭스는 이
slice의 검증 대상이 아니다. 기존 Dev worktree의 미커밋 변경은 보존했고
`C:\Git\2D\Original`은 수정하지 않았다. commit/push는 수행하지 않았다.

다음 프로젝트 우선순위는 OVL-10 통합 metric 단위의 의미 보존 검증이다.  
Recommended model: `gpt-5.6-terra`  
Reasoning effort: `high`
