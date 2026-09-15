# OpenVisionLab PL-0010 C4 패리티 재생 보고서

작성일: 2026-08-27 (KST)
저장소: `C:\Git\OpenVisionLab_Dev`
브랜치: `codex/public-sample-ux-docs`
검증 시점 HEAD: `54f219acf0006c565688780bcc9175b6d3376c2f`

## 1. 상태와 범위

**Status: Complete (PL-0010 C4의 bounded Dev parity 범위)**

이번 재생은 SDK object-candidate 계약을 사용하는 현재 Dev 애플리케이션에서
Mask, Multi-ROI, 후보 행, 선택 강조, drawing, reject reason, metric,
Run Report/Run History, public Good/Bad 샘플이 한 번의 Tool 실행 결과와
일치하는지 확인했다. C4 기준을 통과했지만, 이것이 EXE/배포/현장 장비
자격을 의미하지는 않는다.

포함한 흐름:

- Blob mask 후보 재생
- Blob/Contour Multi-ROI 후보 재생
- source-coordinate geometry, `RegionIndex`, applied limits, reject code/text,
  generation stage, candidate identity 보존
- Object Results의 행/선택/분포 표시와 drawing evidence
- Step timing 및 persisted Run History round-trip
- public Blob/Contour Good/Bad 샘플 직접 replay
- 현재 소스 기준 WPF Pipeline Review/Run History 스모크

## 2. 입력과 실행 identity

- SDK 원본: `C:\Git\OpenVisionLab-Vision-SDK`
- vendored SDK manifest commit: `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`
- Dev SDK manifest: `dll\OpenVisionLab-Vision-SDK\sdk-manifest.json`
- SDK configuration: `Release`
- 입력 분석 원문: `C:\Users\USER\.codex\attachments\82bf32fc-7e81-4208-8b8f-f714724cd9f7\pasted-text.txt`
- 입력 원문 SHA-256: `8F6F44A44B0F9EAF988E71E386C54E1B0BA9CA3A48F5BEBA305F102D57220A8A`
- 실행 산출물은 모두 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev` 아래에
  저장했다.

## 3. 구현된 parity 변경

### 3.1 후보와 Run History 모델

`VisionPipelineObjectResult`와 `VisionPipelineObjectRunReport`가 SDK 후보의
다음 필드를 보존한다.

- `CandidateId`, `NativeIndex`, `RegionIndex`
- `Area`, `Center`, `Bounding`, `Angle`
- `Accepted`, `RejectReasonCode`, `RejectReason`
- `AppliedMinimum/MaximumArea`, `AppliedMinimum/MaximumWidth`,
  `AppliedMinimum/MaximumHeight`
- `GenerationStage`, `CoordinateFrame`

Pipeline execution은 `BlobTool`/`ContourTool`의 후보 컬렉션을 직접 소비한다.
이전의 relaxed-limit audit 재실행과 audit 실패 시 accepted-only fallback은
남아 있지 않다. 기존 SDK filtered result/metric 호환성은 유지하되, review와
report는 같은 실행의 원시 후보 정보를 사용한다.

### 3.2 Multi-ROI 표시

Pipeline Review Object Results 표에 `영역/Region` 열을 추가했다. `USE_MULTI_ROI`
와 `CvROIS`가 활성화되면 검토 요약은 다음 형식으로 표시된다.

```text
Multi ROI (2): 0,0,320,420 | 320,0,320,420
```

이는 실행이나 routing을 바꾸지 않는 표시 계층이며, `RegionIndex`는 각 후보
행과 Run History에 함께 저장된다.

### 3.3 WPF 종료 수명주기

Run History 전체 UI 스모크에서 기능 검증은 통과했지만, 종료 중
`Content = null`이 WPF template/popup animation을 null destination으로
해제하면서 `DoubleAnimationBase` 예외를 재현했다. 소유 Window가 이미
`contentHost.Content = null`로 호스트 분리를 담당하므로,
`OpenVisionShellHostView.ReleaseDisplayManager`는 중첩 `Content`를 다시
해제하지 않고 display manager만 정리하도록 수정했다. 수정 후 동일 타깃이
정상 종료했다.

## 4. 검증 결과

### 4.1 SDK/app object-candidate parity

최신 Runner 산출물:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4`

명령:

```powershell
dotnet `
  "C:\Git\OpenVisionLab_Dev\tools\VisionRecipeRunnerSmoke\bin\Any CPU\Debug\net8.0-windows7.0\VisionRecipeRunnerSmoke.dll" `
  --object-candidate-parity-contract `
  "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4"
```

결과는 `PL-0010 object-candidate parity contract passed.`이다.

| 경로 | 결과 |
| --- | --- |
| Blob / mask | 5 rows, 1 `Masked` reject, 4 accepted, `ResultCount=4`, Run History overlays 4 |
| Blob / Multi-ROI | `RegionIndex=0/1`, source geometry 유지, 5 rows/5 overlays, `ResultCount=5` |
| Contour / Multi-ROI | `RegionIndex=0/1`, source geometry 유지, 5 rows/5 overlays, `ResultCount=5` |
| public Blob Good | OK, `ResultCount=12`, 245 candidate rows, 12 overlays |
| public Blob Sparse Bad | expected NG, `ResultCount=3`, 253 candidate rows, 3 overlays |
| public Contour Good | OK, `ResultCount=5`, 5 rows/5 overlays |
| public Contour Missing Bad | expected NG, `ResultCount=2`, 2 rows/2 overlays |

상세 observation/timing/완료 기록:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4\observations.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4\timing.tsv`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4\completion.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4\blob\mask\candidate_rows.tsv`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4\blob\multi-roi\candidate_rows.tsv`

`timing.tsv`는 Tool Step callback 1회, StepResult 1개, Step elapsed와
persisted total elapsed를 각각 기록한다. 시간 비교는 correctness/acceptance와
분리했으며, public replay도 원본 image hash, object rows, overlay count,
ResultCount, expected outcome을 함께 보존했다.

### 4.2 현재 소스 WPF 검증

모든 캡처는 최신 변경 후 빌드된 현재 Dev 소스에서 생성했다.

| 타깃 | 결과 | 현재 산출물 |
| --- | --- | --- |
| `cvr05_object_metric_distribution` | OK, `1180x890` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-cvr05-20260827-r3\cvr05_object_metric_distribution.png` |
| `wpf_shell_host_workspace_sample_pipeline_review_metrics` | OK, `1600x900` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-blob-good-20260827-r4\wpf_shell_host_workspace_sample_pipeline_review_metrics.png` |
| `wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics` | OK, `1600x900` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-contour-ng-20260827-r2\wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics.png` |
| `wpf_shell_host_recipe_run_history_review_queue` | OK, `1600x900` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-run-history-20260827-r14\wpf_shell_host_recipe_run_history_review_queue.png` |
| Run History queue evidence | captured | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-run-history-20260827-r14\wpf_shell_host_recipe_run_history_review_queue.evidence\review-queue-contract.txt` |
| `wpf_shell_host_pipeline_review` | OK, `1600x900` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-pipeline-20260827-r4\wpf_shell_host_pipeline_review.png` |
| `wpf_shell_host_pipeline_review_ng` | OK, `1600x900` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-pipeline-ng-20260827-r3\wpf_shell_host_pipeline_review_ng.png` |
| `wpf_shell_host_pipeline_review_image_first_compact` | OK, `1280x800` | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-pipeline-20260827-r4-compact\wpf_shell_host_pipeline_review_image_first_compact.png` |

`cvr05` 검증은 selected candidate highlight, Area/Width/Height distribution,
Multi-ROI region description, plot click/table selection, no-Run side effect를
확인했다. Host Blob/Contour 캡처는 accepted/rejected drawing, NG reason,
source/output image를 확인했다. Run History 타깃은 최근 batch list/sample,
NG-only filter, comparison/baseline guidance, failure review action,
source/output evidence, copy action, timing/report rows를 확인한 후 정상
종료했다.

## 5. C4 판정

| C4 항목 | 판정 | 근거 |
| --- | --- | --- |
| object rows / identity / RegionIndex | 통과 | parity `candidate_rows.tsv`, current Object Results smoke |
| drawing / selection highlighting | 통과 | parity candidate drawings, `cvr05` selected highlight, Host Blob/Contour overlay |
| reject reason / acceptance / metrics | 통과 | Masked row, public expected NG, ResultCount/area/dimension checks |
| Run Report / Run History | 통과 | persisted report round-trip, Run History queue target and contract file |
| timing | 통과 | `timing.tsv`, persisted elapsed fields, Run History timing rows |
| public Good/Bad parity | 통과 | Blob/Contour public four-sample replay |
| approved contract change | 명시됨 | one-pass SDK candidate contract과 Region column/summary 추가 |

따라서 PL-0010의 C4 bounded Dev parity를 `Complete`로 판정하고 이슈를
`resolved`로 전환한다. 다음 제품 우선순위는
`IMAGE-CANVAS-EXTERNAL-CONSUMER-CONTRACT`의 bounded Viewer selection/
highlighting slice다.

## 6. 부족한 점과 경계

- SDK의 mask semantics는 Blob에서만 정의·검증했다. 현재 SDK release는
  Contour mask classification 계약을 정의하지 않으므로 Contour mask
  동작을 주장하지 않는다.
- `cvr05`는 현재 소스 직접 WPF view이고, host 캡처도 desktop EXE launch가
  아니다. EXE launch smoke는 이번 범위에서 수행하지 않았다.
- 100/125/150/175/200% DPI, dark/light theme, monitor topology,
  maximize/restore/resize matrix는 수행하지 않았다.
- 원본 `C:\Git\OpenVisionLab` 변경, commit, push, tag, Release 공개,
  설치, rollout, deployment는 수행하지 않았다.
- 실제 카메라, 조명, PLC/I/O, 현장 calibration/production robustness는
  제품 범위 밖이며 이 보고서가 자격을 부여하지 않는다.

## 7. 재현 명령과 결과

이번 재생에서 실행한 핵심 명령:

```powershell
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" --nologo --verbosity:minimal
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo --verbosity:minimal
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1
git diff --check
```

최종 결과:

- solution build: 0 warnings / 0 errors
- `OpenVisionLab readiness contract passed` (13/13 checks)
- `Vendored DLL check passed`
- `PublicSampleAssetCheck=PASS | CatalogRows=33 ManifestAssets=229 Pipelines=17`
- `DocumentationIndex=PASS IndexedPaths=86 Routes=12 RootRedirects=102`
- `git diff --check`: 통과 (공백 오류 없음)
