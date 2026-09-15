# OpenVisionLab 2D 공개 파일 입력 경계 감사

작성일: 2026-09-15 KST  
대상 저장소: `C:\Git\2D\Dev`  
상태: **Blocked (제품 크기 정책 결정 대기)**

## 결론

PL-0007과 AppPath owner는 저장 root, 상대 세그먼트, traversal, rooted path,
예약 장치명, control 문자, trailing dot/space, case collision을 mutation 전에
검사한다. `TwoDIntegrationExchange`도 transaction-relative 경로의 canonical
root containment와 기존 reparse point를 검사한다.

그러나 공개 파일 입구 전체에 적용되는 하나의 입력 크기 정책은 현재 존재하지
않는다. review bundle과 TCP transport에는 별도 상한이 있지만, 일반 Pipeline
XML, locator JSON, local integration message, hash-checked artifact validator는
파일을 읽기 전에 공통 최대 바이트를 거절하지 않는다. 제품 호환을 깨뜨릴 상한을
추측해 추가하지 않고 이 경계를 사람 결정 대기 상태로 남긴다.

## Owner와 호출 경로

| 입력 | 현재 owner / 호출 경로 | 확인된 경계 |
| --- | --- | --- |
| Recipe/Pipeline XML | `SerializeHelper.TryLoadFromXmlFile` → `VisionPipelineXmlSchemaPolicy.Inspect` → `File.ReadAllBytes` | schema/unknown critical element/Dtd는 검사하지만 파일 크기 상한은 없음 |
| locator JSON | `TwoDIntegrationLocatorRecipeContract.IsLocatorRecipe`/`Read` → `File.ReadAllText` → `JsonDocument`/`JsonSerializer` | schema와 parameter 검사는 있으나 바이트 상한은 없음 |
| local integration message/artifact | `TwoDIntegrationExchange.ReadMessage` → `File.ReadAllBytes`; `ReadHandoff` → `EnsureNoReparsePoints` → `IntegrationContractValidator.ValidateArtifactFile` | transaction root/reparse/실제 length/hash는 검사하지만 local max artifact/message 상한은 없음 |
| review bundle | `OpenVisionRecipeReviewBundleInspector.ReadEntryBytes` | `pipeline.xml` 5 MiB, `review-manifest.json` 2 MiB를 압축 해제·parse 전에 검사 |
| TCP transaction | shared `TcpIntegrationOptions`/`TcpIntegrationProtocol.ValidateManifest` | 기본 1,024 files, file 4 GiB, transaction 16 GiB, control frame 1 MiB, UTF-8 relative path 1 KiB; product가 낮출 수 있음 |

`ResolveArtifactPath`와 `EnsureNoReparsePoints`는 기존 transaction directory와
각 artifact segment를 같은 경계에서 다룬다. PL-0007의 Recipe/Pipeline/sample-set/
Run Report/Batch Summary contract와 AppPath contract는 기존 owner를 재구현하지
않고 회귀 증거로 채택했다.

## 조사 증거

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d023-file-input-20260915\source-and-upstream-audit.txt`
  는 현재 source owner, shared commit `f4743f3307d20a963b2197f2019713320b9859b9`,
  alpha.3 package SHA-256, TCP limits, validator reflection probe를 보존한다.
- package `OpenVisionLab.Integration.Contracts`의 직접 `ValidateArtifact` probe는
  `ByteLength=4 GiB`, `16 GiB`, `Int64.MaxValue`를 max-size issue 없이 통과시켰다.
  이는 실제 대용량 파일을 만들었다는 뜻이 아니라 generic contract validator에
  local maximum이 없다는 source/package 증거다.
- 기존 OVL-02 정상 integration evidence는 Handoff→Acknowledgement→Run→Result와
  TCP 수신 후 자동 실행 금지를 통과시켰다:
  `docs/reports/OPENVISIONLAB_OVL02_TWO_D_INTEGRATION_IDENTITY_20260907.md`.
- 현재 source 기준 Debug/Release `VisionRecipeRunnerSmoke` build는 오류 없이
  통과했고, AppPath와 PL-0007 storage path regression은 각각 `Passed=8
  Failed=0` 및 기존 Recipe storage contract 통과를 재확인했다. 결과는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d023-file-input-20260915\` 아래에 있다.

## Acceptance 대조

| 기준 | 결과 |
| --- | --- |
| 허용 root와 기존 path/reparse 정책 | **통과** — existing owner와 regression evidence |
| 일반 JSON/XML/local message/artifact의 공통 size policy | **미충족** — 입구별 상한이 없거나 서로 다름 |
| oversized를 계산 전에 명확한 오류로 종료 | **미검증/차단** — 제품 상한과 오류 계약이 없음 |
| 정상 한글/상대 경로 | **기존 경로 contract 통과** |
| 지원 long path | **미검증** — Windows/transport별 길이 기준을 먼저 결정해야 함 |
| 기존 정상 artifact import 회귀 | **기존 OVL-02 evidence 채택** |

## Blocker와 다음 조치

다음 제품 결정을 정하면 이 slice를 재개할 수 있다.

1. local Pipeline XML, locator JSON, integration envelope, artifact 각각의 최대
   바이트와 transaction 합계 상한을 정한다.
2. local file adapter가 TCP 기본값을 그대로 상속할지, 계산 전 메모리 보호를 위해
   더 낮은 제품 상한을 둘지 정한다.
3. oversized 오류를 기존 typed error로 매핑하고, 한글/허용 long path와
   root/traversal/reparse 회귀 fixture를 한 번에 계약화한다.

결정 전에는 기존 root 정책을 축소하거나 임의의 보안 framework·새 service를
추가하지 않는다. 사용자 파일, `C:\Git\2D\Original`, hardware, external service,
commit/push/release/deploy는 이 감사에서 건드리지 않았다.

## 개발자 reading order

1. `src/OpenVisionLab/Common/Runtime/AppPathService.cs`와
   `src/OpenVisionLab/Core/Recipe/RecipeWorkspaceService.cs`에서 root owner를 읽는다.
2. `src/OpenVisionLab/Common/Persistence/SerializeHelper.cs`와
   `VisionPipelineXmlSchemaPolicy.cs`에서 XML load gate를 읽는다.
3. `src/OpenVisionLab/Core/Integration/TwoDIntegrationExchange.cs`와
   `TwoDIntegrationLocatorRecipeContract.cs`에서 local message/JSON path를 읽는다.
4. `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeReviewBundleInspector.cs`
   및 shared TCP `TcpIntegrationModels.cs`/`TcpIntegrationProtocol.cs`에서 이미
   존재하는 별도 size policy를 비교한다.
5. `docs/reports/OPENVISIONLAB_OVL02_TWO_D_INTEGRATION_IDENTITY_20260907.md`와
   PL-0007/AppPath evidence를 회귀 기준으로 확인한다.

소스 코드 기준 검토 완료 / 실제 WPF UI, DPI, hardware, clean-release, 실제
대용량 파일과 long-path 조합은 검증 필요.
