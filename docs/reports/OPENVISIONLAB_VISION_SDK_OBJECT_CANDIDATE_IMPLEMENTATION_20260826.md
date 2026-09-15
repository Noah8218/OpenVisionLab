# OpenVisionLab Vision SDK Object Candidate 구현 보고서

작성일: 2026-08-26 (KST)

## 1. 상태와 범위

**Status: Complete (이번 단계의 bounded implementation 범위)**

이번 단계의 목표는 GPT Pro 분석에서 P1로 제시한 `VISION-SDK-OBJECT-CANDIDATE-EVIDENCE-V3_1`의 최소 계약을 SDK 원본과 OpenVisionLab Dev 소비 경로에 구현하는 것이었다. 다음 범위는 완료했지만, 전체 PL-0010 패리티 작업이 모두 끝났다는 의미는 아니다.

- SDK 원본: `C:\Git\OpenVisionLab-Vision-SDK`
- SDK 고정 커밋: `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`
- SDK 패리티 테스트 보강 커밋: `87a42ed` (`f4f0c0d` 이후 test-only)
- Dev 소비 저장소: `C:\Git\OpenVisionLab_Dev`
- 입력 분석 원문: `C:\Users\USER\.codex\attachments\82bf32fc-7e81-4208-8b8f-f714724cd9f7\pasted-text.txt`
- 입력 원문 SHA-256: `8F6F44A44B0F9EAF988E71E386C54E1B0BA9CA3A48F5BEBA305F102D57220A8A`

이번 단계에서는 SDK 원본의 로컬 커밋과 D: 드라이브의 로컬 패키지/테스트 산출물만 만들었다. SDK push, Dev push, 원본 저장소 반영, GitHub Release 공개, 설치/롤아웃/배포, EXE launch smoke는 수행하지 않았다.

## 2. GPT Pro 분석에서 이번 단계로 선택한 개발 항목

분석 문서의 전체 목록 중 현재 가장 먼저 증거를 만들 수 있고, Viewer/UI 개선의 전제인 항목을 선택했다.

1. Blob/Contour가 후보를 한 번만 생성하고 원시 후보를 모두 노출한다.
2. 후보마다 안정적인 `CandidateId`와 native index를 제공한다.
3. Area뿐 아니라 Bounds/Center/Angle, 적용 제한, `Accepted`, reject code/text, drawing geometry, generation stage, coordinate frame을 제공한다.
4. Dev Pipeline이 SDK 후보를 직접 소비하여 이전의 `TryCaptureUnfiltered`/relaxed second execution audit를 제거한다.
5. Run History와 TSV evidence가 후보 식별자와 판정 원인을 보존한다.
6. SDK 패키지 소비자와 vendored DLL manifest가 같은 계약을 확인한다.

이 순서는 “정확한 후보 증거를 먼저 확보한 뒤 Viewer에서 클릭/강조/Good-Bad 설명을 개선한다”는 제품 방향과 맞는다. LLM은 여전히 선택적 XML authoring 보조이며 이번 구현의 실행 전제가 아니다.

## 3. 구현 내용

### 3.1 SDK 원본

커밋 `f4f0c0d`에서 다음을 추가했다.

- `VisionObjectCandidate` 및 관련 enum/limits/decision 계약
  - `CandidateId`, `RegionIndex`, `NativeIndex`
  - `Area`, `Center`, `Bounding`, `Angle`
  - `Accepted`, `RejectReasonCode`, `RejectReasonText`
  - `AppliedLimits`, `Drawing`, `GenerationStage`, `CoordinateFrame`
- Blob/Contour 공통 차원 제한 인터페이스 `IVisionObjectFilterProperty`
- Blob/Contour 단일 실행 후보 컬렉션
  - 기존 호환용 `results`는 유지
  - 새 `candidates`는 accepted/rejected 원시 후보를 모두 보존
  - 후보 ID와 정렬은 반복 실행에서 결정적
- SDK smoke suite에 Blob/Contour 후보 및 reject-code/drawing/determinism 검증 추가
- SDK smoke suite에 Blob mask 보존 및 Blob/Contour multi-ROI source-coordinate 검증 추가
- package consumer smoke에 공개 패키지 API 검증 추가
- 계약 문서 `docs/OBJECT_CANDIDATE_CONTRACT.md` 추가 및 README 연결

### 3.2 Dev 애플리케이션

- `BlobProperty`, `ContourProperty`가 SDK의 `IVisionObjectFilterProperty`를 구현
- `VisionPipelineObjectResults`가 SDK `candidates`를 직접 매핑
- `TryCaptureUnfiltered`, `CloneForAreaAudit`, `auditTool.Execute` 및 silent catch 기반의 두 번째 실행 제거
- 기존 dimension filter는 Dev에서 계속 적용하되, 후보 원본과 reject reason은 별도 행으로 보존
- `VisionPipelineObjectResult`와 `VisionPipelineObjectRunReport`에 다음 메타데이터 저장
  - `CandidateId`, `NativeIndex`
  - `RejectReasonCode`
  - `GenerationStage`, `CoordinateFrame`
- object-dimension smoke가 후보 식별자, native index, `SourceImage`, reject code, accepted drawing, persisted Run History를 검증

### 3.3 Vendored DLL 및 manifest

SDK Release 산출물을 Dev의 `dll/OpenVisionLab-Vision-SDK`에 반영하고 다음 두 manifest를 갱신했다.

- `dll/OpenVisionLab-Vision-SDK/sdk-manifest.json`
- `docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json`

최종 Dev 파일의 길이/SHA-256은 다음과 같다.

| 파일 | 길이 | SHA-256 |
|---|---:|---|
| `OpenVisionLab.Core.dll` | 35,840 | `6233CE81E3EFB83BF70C701873D83A2829463EC9F45AA7BD41CA037E132006B6` |
| `OpenVisionLab.Vision2D.dll` | 355,328 | `91A6D4DB01AA2E97F54BB5FF7A61FB0D0404432D0D86821E889350CCA5007B62` |
| `OpenVisionLab.Vision2D.Blob.dll` | 24,576 | `EF9E85C37794B066EB97FA4E959241AD02F7B850EDF4EDF05B1AB41B903D8FCA` |
| `OpenCvSharp.dll` | 862,208 | `A5C477750EB4321B608F4B9183949915D4A42FE0B5D80CFB8376F5A326FA5F24` |
| `OpenCvSharp.Blob.dll` | 40,960 | `E03FE75D2C9D88886384EDBC445C63DA051EE3450286C0982FCD9F4BC24D54` |
| `OpenCVSharp/OpenCvSharpExtern.dll` | 53,231,104 | `C9E02A255DD83C9B06CA56EC6F435F15B53A863435238FCC5D8B9082B035F249` |

기존 DLL은 복구 가능한 백업으로 다음 위치에 보관했다.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\vision-sdk-candidate-f4f0c0d\pre-update-vendored-dlls`

## 4. 검증 결과

### SDK 원본

- `dotnet build OpenVisionLab.VisionSdk.sln -c Debug -p:Platform="Any CPU"` — 0 warnings / 0 errors
- 동일 Release build — 0 warnings / 0 errors
- `dotnet run --no-build --project tests\OpenVisionLab.Inspection.Smoke\OpenVisionLab.Inspection.Smoke.csproj -c Debug` — **189/189 passed**
- 동일 Release smoke — **189/189 passed**
- 로컬 패키지 `3.0.1-dev.20260826.candidate.2`를 D: 전용 source/cache로 restore한 package consumer — **passed**

패키지 산출물 위치:

`D:\OpenVisionLab-TestData\OpenVisionLab-Vision-SDK\candidate-20260826-r2\packages`

### Dev 애플리케이션

- `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` — 0 warnings / 0 errors
- `dotnet build tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj -c Debug` — 0 warnings / 0 errors
- `OpenVisionReadinessCheck` — `OpenVisionLab readiness contract passed`
- `TestExternalReferences.ps1` — `Vendored DLL check passed`
- `TestPublicSampleAssets.ps1` — `PublicSampleAssetCheck=PASS | CatalogRows=33 ManifestAssets=229 Pipelines=17`
- object-dimension contract smoke — **passed**
  - Blob/Contour 각각 1 accepted + 4 rejected
  - width/height reject code/text 보존
  - CandidateId 유일성 및 native index 보존
  - `GenerationStage`/`CoordinateFrame=SourceImage` 보존
  - accepted drawing 및 Blob Run History 메타데이터 보존
  - 기존 dimension 키가 없는 legacy XML의 5개 area-valid object 동작 보존
- SDK parity smoke — **passed**
  - Blob mask 후보는 `Masked` reject code/drawing으로 보존되고 legacy `results`에서는 제외
  - Blob/Contour multi-ROI 후보는 `RegionIndex`가 `0/1`로 구분되고 source-coordinate bounds/points와 stage-prefix ID를 보존
- public Blob sample direct replay — **passed**
  - `Blob_Particles_Synthetic_OK.png`: `ResultCount=12`, `Success=True`, `Outcome=OK`
  - `Blob_Particles_Synthetic_Sparse_NG.png`: `ResultCount=3`, expected acceptance NG, `Outcome=NG`
- current-source direct WPF object-metric view smoke — **passed**
  - `cvr05_object_metric_distribution` passed and captured current selection/distribution evidence at
    `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\vision-sdk-candidate-f4f0c0d\ui-object-parity-r1\cvr05_object_metric_distribution.png`
- broader WPF host Pipeline Review targets — **failed / not used as parity proof**
  - `wpf_shell_host_pipeline_review`, `wpf_shell_host_pipeline_review_ng`, and
    `wpf_shell_host_workspace_sample_pipeline_review_metrics` returned `리뷰 실행 필요`
    after explicit review execution; the current run did not expose completed step results.
  - This failure is recorded as an unresolved current-build verification boundary; no UI or
    host fix is claimed in this SDK slice.

현재 실행 증거:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\vision-sdk-candidate-f4f0c0d\app-object-dimension-r3`

대표 TSV는 다음을 직접 보여준다.

```text
CandidateId NativeIndex Accepted RejectReasonCode GenerationStage CoordinateFrame
BlobLabeling:0:4 4 False HeightBelowMinimum BlobLabeling SourceImage
BlobLabeling:0:2 2 False WidthAboveMaximum BlobLabeling SourceImage
BlobLabeling:0:1 1 True None BlobLabeling SourceImage
```

## 5. 수용한 부족한 점과 남은 검증

이번 구현은 GPT Pro 분석의 “SDK 계약과 단일 실행”을 닫았지만 다음을 아직 완료로 주장하지 않는다.

- SDK-level mask/multi-ROI candidate parity는 통과했지만, Dev Pipeline/UI에서의 전체 mask/multi-ROI replay는 아직 미검증
- 실제 public-candidate를 공급한 Viewer의 선택 강조/클릭 상호작용 (synthetic direct view smoke만 통과)
- drawing geometry의 모든 도형 종류에 대한 시각 검증
- Run History/Report의 전체 후보 분석 UX
- public sample을 이용한 end-to-end 후보/결과 패리티
- 현재 Dev build의 EXE launch smoke 및 여러 DPI/테마/레이아웃 UI 검증
- 원본 저장소 반영, push, Release 공개, 설치/롤아웃/배포

따라서 PL-0010은 전체 패리티가 끝날 때까지 `doing`으로 유지한다. 이번 단계에서 확인하지 않은 내용을 SDK 계약 완료로 확대 해석하지 않는다.

## 6. 다음 개발 우선순위

1. PL-0010의 Dev Pipeline/UI mask/multi-ROI/selection/timing/drawing/Run History/public-sample 패리티 replay를 수행하고, divergence가 있으면 그 한정 범위만 수정한다. 통과 시 PL-0010을 resolved로 전환한다.
   **Recommended model: gpt-5.6-terra | Reasoning effort: medium**
2. PL-0010 종료 후 `IMAGE-CANVAS-EXTERNAL-CONSUMER-CONTRACT`를 구현하여 Viewer가 후보를 클릭해 강조하고 accepted/rejected 사유를 설명하도록 한다.
   **Recommended model: gpt-5.6-terra | Reasoning effort: medium**
3. LLM/consumer-web/API 통합은 유지보수 모드로 보류한다. 새 검증 캠페인이나 prompt family는 명시적인 재개 요청과 고정 corpus가 있을 때만 계획한다.
   **Recommended model: none until explicitly reopened | Reasoning effort: none until explicitly reopened**

## 7. 경계와 복구

- 원본 `C:\Git\OpenVisionLab`에는 이번 단계에서 변경을 반영하지 않았다.
- SDK `main`에는 로컬 커밋만 만들었고 push하지 않았다.
- 배포 대상/설치 환경/고객 롤아웃은 지정하지 않았고 실행하지 않았다.
- DLL 교체 전 파일은 D: 백업 폴더에서 복구할 수 있다.
- 전체 프로젝트의 최종 완료 조건은 위 5절의 남은 패리티와 UI/EXE 증거가 추가로 통과해야 충족된다.
