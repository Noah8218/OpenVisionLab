# OpenVisionLab Current Project Handoff

Updated: 2026-09-15 KST

이 문서는 현재 상태와 다음 행동만 담는 live handoff입니다. 완료 chronology는
[`OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_20260914.md`](archive/OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_20260914.md),
구조 세부 이력은
[`CODEBASE_STRUCTURE_HISTORY_20260914.md`](archive/CODEBASE_STRUCTURE_HISTORY_20260914.md)에
보존했습니다.

## Product identity

OpenVisionLab은 Windows x64/.NET 8 기반의 OpenCvSharp4 규칙 기반 vision recipe
workbench입니다. 핵심 흐름은 `sample image → PropertyGrid teaching/Pipeline 구성 →
명시적 Preview 또는 Run → layer/result 비교 → N-sample validation → Recipe 저장`입니다.
LLM XML 작성 지원은 선택적 유지 기능이며, 카메라·조명·PLC·I/O 통합 플랫폼은 현재
제품 범위가 아닙니다.

## Start Here

1. `OpenVisionLab.sln`을 Visual Studio에서 엽니다.
2. 시작 프로젝트는 `src/OpenVisionLab/OpenVisionLab.csproj`입니다.
3. `Debug | Any CPU` 또는 `Debug | x64`를 사용합니다. 앱 결과물은 x64입니다.
4. 코드는 `Program.cs` → `App/Bootstrap/OpenVisionLabApplication.cs` →
   `UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs` 순서로 읽습니다.
5. Recipe 실행은 `RecipeCommandSurface` → `OpenVisionPipelineReviewDocument` →
   `VisionPipelineExecutionService` 순서로 추적합니다.

## Current repository boundary

- 작업 위치: `C:\Git\2D\Dev`
- 브랜치: `codex/public-sample-ux-docs`
- 시작 HEAD: `176eec95e0081502dc07d89ddaee666abac8123d`
- 이번 작업 시작 시 상태: 244 paths (`54` tracked, `190` untracked; `-uall` 기준)
- 시작 상태·추적 patch·파일 SHA-256:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\predevelopment-priorities-20260914\01-baseline`
- 기존 변경은 사용자 작업으로 취급하며 reset/checkout/bulk overwrite하지 않습니다.
- `C:\Git\2D\Original`, commit, push, tag, release, deploy는 승인 범위 밖입니다.
- 현재 이 프로젝트는 `openvisionlab-2d` heartbeat가 5분 간격으로 한 slice씩 실행합니다.

## Latest completed work — PL-0057

사용자가 선행개발 전 우선순위 1~5의 실행을 요청했습니다.

| Milestone | 상태 | 범위 |
| --- | --- | --- |
| M1 | Done | 시작 worktree/HEAD/patch/hash 기준선 고정 |
| M2 | Done | 거짓 x86 구성 제거, embedded smoke 종료 코드, SDK NOTICE commit 정합성 |
| M3 | Done | 문서 제어면 compact화, 전체 1,331-file scan, 검증된 dead-file 정리 |
| M4 | Done | Line 결과/진단 label 충돌 방지와 actual EXE UI 증거 |
| M5 | Done | concrete ImageCanvas control/lifetime을 View/presentation 경계로 이동 |

완료 보고서:

[`OPENVISIONLAB_PREDEVELOPMENT_PRIORITIES_1_TO_5_20260915.md`](../reports/OPENVISIONLAB_PREDEVELOPMENT_PRIORITIES_1_TO_5_20260915.md)

최종 local evidence:

- solution Debug/Release: 각각 warnings `0`, errors `0`
- readiness 13개 범주, 문서 12 routes, external DLL Debug/Release, NOTICE,
  public sample 33/230/17, refactor audit 모두 PASS
- Line/ImageCanvas/path contract: `5/8/6` checks PASS
- actual 제품 EXE: exit `0`, 37 px / 0.222 mm / 24 detections, 최종 동적 topology의
  단일 `DISPLAY2` 교차 PASS; 앞선 2-monitor run의 작은 왼쪽 `DISPLAY2` 배치도 PASS
- actual 외부 ImageCanvas consumer: exit `0`, 100 create/dispose, handle delta `+2`,
  최종 단일 `DISPLAY2` 내부 배치 PASS
- 전체 문서 scan: invalid UTF-8/JSON `0`, missing inline relative link `0`

증거 루트:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\predevelopment-priorities-20260914`

## Latest completed follow-up — PL-0058

실패 Step 파라미터 수정 후 corrected-output 재검사 경계를 보강했습니다. PropertyGrid
편집이 dirty인 동안 `RerunCorrectedOutputCommand`는 비활성화되고, 버튼 텍스트/툴팁은
XML 반영 또는 취소가 먼저임을 표시합니다. XML 반영 성공 후에는 기존 Good/Bad 또는
동일 Validation Set 재검사 경로가 복원됩니다. XML 반영 자체는 계속 Preview/Run을
실행하지 않습니다.

- owner: `RecipeCommandSurface` (`IsSelectedStepEditDirty`, `CanRerunCorrectedOutput`,
  `RerunCorrectedOutput`)
- View contract: `HostRecipeCorrectedOutputRerunButton` in
  `OpenVisionShellHostView.xaml`
- issue/report: [`PL-0058`](../../.proofline/issues/PL-0058.json),
  [`OPENVISIONLAB_CORRECTED_OUTPUT_DIRTY_EDIT_GUARD_20260915.md`](../reports/OPENVISIONLAB_CORRECTED_OUTPUT_DIRTY_EDIT_GUARD_20260915.md)
- verification: solution Debug/Any CPU build warnings `0`, errors `0`; focused WPF
  smoke `wpf_shell_host_fixture_step_edit_apply_rerun=OK`; fresh pending/applied
  captures under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\PL-0058_dirty-rerun-20260915-final2`

## Latest scheduled 2D follow-up — PL-0062

Recipe 파일 실행 경로는 파일을 한 번 읽은 byte snapshot에서 deserialize·원본 SHA·실행
provenance를 생성하도록 고정했습니다. UTF-8 BOM/non-BOM과 UTF-16을 유지하며, 잘못된
XML은 실행 전에 실패합니다.

- owner: `VisionRecipeRunner.RunAsync(file)` → `SerializeHelper.TryLoadFromXmlBytes` →
  `VisionPipelineExecutionPlan.Create(bytes)`
- issue: [`PL-0062`](../../.proofline/issues/PL-0062.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d004-recipe-snapshot-20260915`
- verification: recipe-file-snapshot `6/0` in Debug/Release; app, smoke, and solution
  Debug/Release builds warnings `0`, errors `0`; cancellation `5/0`, failure-lifetime
  `7/0`, finite-acceptance `10/0` regressions remain passing

## Latest scheduled 2D follow-up — PL-0063

2D 외부 Result 분류는 typed Step 상태를 사용하도록 고정했습니다. Acceptance NG만
`Completed/Ng`로 남고, `ToolFactoryFailed`·`InvalidRoi`·`StepTimeout`·미완료/알 수 없는
상태는 v2 validator가 허용하는 run identity 없는 `Failed/ExecutionError`, `StepCanceled`는
`Cancelled/Indeterminate`로 기록됩니다.

- owner: `TwoDIntegrationExchange.RunAcceptedHandoffCoreAsync` → `ClassifyRunResult` → `IntegrationResultV2`
- issue: [`PL-0063`](../../.proofline/issues/PL-0063.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d005-result-disposition-20260915`
- verification: focused disposition contract `9/0` in Debug/Release; OpenVisionLab and
  VisionRecipeRunnerSmoke Debug/Release builds warnings `0`, errors `0`; existing dirty-runtime
  integration fail-closed smoke passed; matching-assembly clean integration smoke preserved
  `Good=Pass`, `Bad=Ng`, run-record/metric correlation, and concurrent-run guards

## Latest scheduled 2D follow-up — PL-0064

Native Preview 시도는 시작할 때 현재 결과와 기존 result-review를 무효화합니다. 성공한
경우에만 다시 current로 올리고, 실패한 경우 이전 output layer는 삭제하지 않지만 현재
결과로 취급하지 않습니다. 입력 변경도 같은 invalidation owner를 사용합니다.

- owner: `OpenVisionNativeToolDocument.RunPreview/RunArithmeticPreview` →
  `ClearPreviewResult` → `ApplyPreviewExecutionResult`
- issue: [`PL-0064`](../../.proofline/issues/PL-0064.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d006-preview-validity-20260915`
- verification: Debug/Release embedded app builds warnings `0`, errors `0`; actual WPF
  preview-validity smoke passed for success → missing-template failure → viewer reopen →
  valid retry → input change; Release `line-pins-measure` signal regression passed; monitor
  `DISPLAY2` intersection and before/after/recovery screenshots were recorded. Full theme/DPI
  matrix remains unverified.

## Latest scheduled 2D follow-up — PL-0065

Preview 처리 성공과 Pipeline Acceptance 판정을 기존 화면과 export에서 분리해 표시합니다.
직접 Native Preview는 기존 `Preview OK/NG`·`Offset OK` prefix를 유지하면서 `검사 판정 미평가`를
명시하고, Pipeline Review는 `처리 OK/판정 PASS·NG·미평가` 또는 `처리 NG/판정 미평가`를
표시합니다. Run Report에는 `AcceptanceEvaluated`를 저장하며, recipe drawing evidence는
적용 기준이 없는 실행을 `미평가: 적용 기준 없음`으로 보여줍니다.

- owner: `OpenVisionNativePreviewExecutionController` → `OpenVisionNativeToolDocument.SetStatus`;
  `OpenVisionPipelineReviewResultPresenter` → `OpenVisionPipelineReviewGuideResultProjectionOwner`;
  `VisionPipelineRunReportStorage` → `OpenVisionRecipeRunEvidenceDrawing`
- issue: [`PL-0065`](../../.proofline/issues/PL-0065.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d007-preview-acceptance-clarity-20260915`
- verification: VisionRecipeRunnerSmoke guide/result contract Debug/Release passed with missing,
  no-criteria OK, acceptance PASS, tool NG, acceptance NG, persisted evidence, and Korean/English
  text cases; OpenVisionLab embedded Debug/Release builds and PipelineViewerScreenshotSmoke Debug
  acceptance-NG WPF target passed (`layout=0`, `text=0`, `internal=0`). Actual Debug native Preview
  smoke passed success → failed rerun → viewer reopen → recovery → input change with `DISPLAY2`
  monitor intersection. A post-patch native rerun opened the expected shell but timed out without a
  report and is recorded as an execution-harness limitation, not a product result. The focused
  recipe-pipeline-roundtrip probe remains an existing pre-review window failure and is not used as
  PL-0065 evidence. Full theme/DPI/native hardware matrix remains unverified.

## Completed scheduled 2D slice — PL-0066 (2D-008 async boundary)

The 2D-008 baseline remains the reason for the change: a controlled 420 ms delegate produced a
664 ms synchronous blocking window with zero Dispatcher heartbeat ticks, exceeding the proposed
250 ms budget. The existing owner now has a concrete single-input boundary:
`OpenVisionNativeToolDocument.RunPreview` → `OpenVisionNativePreviewExecutionBoundary` →
`OpenVisionNativePreviewExecutionController`; input Bitmap snapshot and property/layer names are
captured on the UI thread, only the Mat/native delegate runs on the worker, and the owned result
Bitmap is published or discarded on the Dispatcher.

- issue: [`PL-0066`](../../.proofline/issues/PL-0066.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d008-preview-ui-heartbeat-20260915-r6`
  and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d008-preview-ui-async-20260915\image-space-contract`
- verification: embedded Debug build and `VisionRecipeRunnerSmoke` build passed with 0
  warnings/0 errors; `preview-ui-heartbeat-async` recorded 33 Dispatcher ticks during the 420 ms
  delegate, rejected the duplicate start, published the first result, and discarded the canceled
  late result. The smoke used `DISPLAY2`, saved `PreviewAsyncOutput.png`, and left no
  OpenVisionLab process after the follow-up check.
- click-path evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d008-preview-ui-click-20260915-r5`
  invoked the visible `VisionToolRunPreviewButton` through its WPF command contract. The actual
  Blob path recorded 6 Dispatcher ticks during a 420 ms delegate, one delegate call for two rapid
  clicks, input-change late-result discard, valid recovery, and document-dispose late-result
  discard. The final screenshot shows `Preview CANCELED / 검사 판정 미평가 / document disposed`;
  the stale RUN state is no longer left visible. Both shell and floating tool intersected
  `DISPLAY2`; no OpenVisionLab process remained after the check.
- boundary: one active cancellation source and generation/lifetime guard are now owned by the
  concrete execution boundary; input/output layer changes cancel and invalidate the current
  result; arithmetic Preview remains synchronous by design until its own risk slice. The focused
  `property-grid-roi-editor` smoke passed against the changed Debug binary, including the actual
  PropertyGrid ROI button path and clean process exit. PL-0066 is resolved. An explicit user
  Cancel command is not part of the current 2D-008 acceptance contract and remains a separate
  product decision; it was not added speculatively. Full theme/DPI and native hardware coverage
  remain unverified.

## Completed scheduled 2D slice — PL-0067 (2D-009 pre-validation alignment)

2D-009의 source gap은 UI Review와 공개 Runner의 admission 경계가 달랐다는 점입니다. UI는
`OpenVisionPipelineReviewDocument`에서 `VisionPipelineValidator`를 호출했지만, 공개
`VisionRecipeRunner`는 실행 계획과 `VisionPipelineExecutionService`로 바로 들어갔고 TCP
검사는 Runner를 재사용했습니다. 현재 변경은 정규화된 execution copy에 기존 Validator를
적용하고, 오류를 `VisionPipelineValidationException`으로 전달하는 최소 경계입니다.

- issue: [`PL-0067`](../../.proofline/issues/PL-0067.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d009-prevalidation-20260915`
- verification: OpenVisionLab Debug build, VisionRecipeRunnerSmoke build, TwoDIntegrationTcpSmoke
  build 모두 0 warnings/0 errors; pre-validation contract가 valid object/XML 경로와
  unsupported ToolType, missing layer, invalid parameter, invalid acceptance를 shared Validator
  오류와 동일하게 거절했고, 기존 Pipeline Review execution contract도 통과했습니다.
- boundary: UI 표시 문구와 기존 Validator owner는 유지하며, TCP의 실제 외부 handoff/장비와
  전체 theme/DPI/native hardware 행은 이 slice에서 검증하지 않습니다.

## Completed scheduled 2D slice — PL-0068 (2D-010 ROI meaning boundaries)

2D-010의 source gap은 `USE_ROI=true`에서 malformed/missing ROI가 `(0,0,0,0)`으로
정규화되고, 0 크기가 유효하지 않은 값으로 거절되지 않으며, EdgeDetection/HsvMask가
malformed 또는 범위 밖 ROI를 전체 이미지로 흘리거나 잘라내던 점입니다. 현재 변경은
`USE_ROI=false`의 의도적인 full-image/unset 의미를 보존하면서, `USE_ROI=true`의 정의
문법·양수 크기·음수 좌표를 shared Validator에서 거절하고, 실행 경계와 직접 실행 Tool도
범위 밖/overflow 값을 Reject 정책으로 일치시킵니다.

- issue: [`PL-0068`](../../.proofline/issues/PL-0068.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d010-roi-meaning-20260915`
- verification: `VisionRecipeRunnerSmoke` Debug build, `OpenVisionFixtureSmoke` Debug build,
  ROI meaning contract, and public fixture smoke all passed with 0 warnings/0 errors. The
  contract covers unset/full-image, valid 4x4 ROI, malformed/zero-size/negative definitions,
  partial and overflow out-of-bounds rejection, direct EdgeDetection/HsvMask rejection,
  multi-ROI definition retention, and public save/reopen `CvROI` preservation.
- boundary: this slice does not claim a full legacy-recipe corpus migration, actual desktop EXE
  UI execution, full theme/DPI/native hardware coverage, or changes to the Original repository.

## Completed scheduled 2D slice — PL-0069 (2D-011 duplicate Step identity mapping)

2D-011의 source gap은 같은 이름·Tool·입력/출력 route를 가진 Step을 `AreSameStep`와
`CompleteRun`의 이름/route first-match로 찾고, 중간 output을 layer 이름 하나로만 캐시해
두 행의 summary·metric·NG 이유·이미지가 충돌할 수 있던 점입니다. 현재 Review 실행은 이번
Run의 ordered Step index를 identity로 사용하고, 원본/실행 pipeline index를 통해 각 행을
정확히 연결합니다. 동일 output route도 Step index별 이미지 캐시를 유지하며, 기존 layer-only
preview 호출은 최신 일치 output을 반환하는 호환 fallback으로 남겼습니다.

- owner: `OpenVisionPipelineReviewExecutionController` (run-scoped Step summary/output
  cache) → `OpenVisionPipelineReviewLayerImageOwner` → `OpenVisionPipelineReviewDocument.SelectStep`
- issue: [`PL-0069`](../../.proofline/issues/PL-0069.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d011-duplicate-step-identity-20260915`
- verification: duplicate identity contract passed with distinct metric/NG mapping, same-route
  output pixels, reorder, disabled-step, and legacy lookup checks. Existing execution-generation,
  cache-lifetime, stale-callback, and document-revision/selection contracts also passed;
  `VisionRecipeRunnerSmoke` Debug build
  completed with 0 warnings/0 errors. This slice is source/contract verified only; no desktop EXE,
  WPF UI interaction, theme/DPI matrix, native hardware, or Original-repository change was run.
- boundary: XML serializer format and persisted Step order remain unchanged; this does not claim
  a GUID migration, full legacy recipe corpus, or actual rendered UI coverage.

## Completed scheduled 2D slice — PL-0070 (2D-012 previous output layer mistaken for current input)

2D-012의 source gap은 `CreateReviewContextFromDisplayLayers`가 작업공간의 모든 image
layer를 새 Run context에 seed해, A Run의 `A_Output`이 producer 삭제·disabled·재정렬 후에도
B Run의 입력으로 소비되던 점입니다. 현재 execution controller는 현재 effective pipeline이
선언한 output과 같은 document에서 앞선 Review가 실제로 만든 output을 초기 seed에서 제외하고,
`ALLOW_BRANCH_INPUT=true`로 명시한 외부 branch만 다시 허용합니다. 기본 `Main` source는 기존
입력 계약을 유지합니다.

- owner: `OpenVisionPipelineReviewExecutionController.RunAsync` →
  `CreateReviewContextFromDisplayLayers(effectivePipeline)` → `VisionPipelineContext`
- issue: [`PL-0070`](../../.proofline/issues/PL-0070.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d012-run-input-isolation-20260915`
- verification: run-input isolation contract passed for deleted, disabled, and reordered producer
  cases; explicit `ALLOW_BRANCH_INPUT` recovery also passed. Existing execution-generation,
  cache-lifetime, stale-callback, document-revision, and PL-0069 duplicate-identity contracts
  passed; `VisionRecipeRunnerSmoke` Debug build completed with 0 warnings/0 errors. Baseline
  contract evidence recorded the pre-patch stale-layer consumption. No desktop EXE/WPF UI,
  theme/DPI, native hardware, or Original-repository mutation was performed.
- boundary: the pipeline format is unchanged and the branch opt-in remains an existing parameter;
  this does not claim arbitrary external provenance detection for layers created outside Review,
  full legacy corpus coverage, or actual rendered UI verification.

## Completed scheduled 2D slice — PL-0071 (2D-013 Preview·Run·reopen result equivalence)

2D-013의 선행 검증은 제품 알고리즘을 바꾸지 않고 기존 Preview/Runner/XML owner를 재사용하는
결정적 계약으로 닫았습니다. 공개 Mean, Blob, Contour, Matching, Line 샘플을 같은
`OpenCvHelper.SetImageChannel1` 입력 의미로 실행해 원시 metric·판정·overlay/object/geometry
좌표와 저장/재열기 결과를 비교했으며, 렌더링 PNG byte 동일성은 요구하지 않았습니다.

- owner: `OpenVisionNativePreviewExecutionController.ComputeSingleInput` →
  `VisionRecipeRunner.RunAsync` → `VisionPipelineExecutionService`; comparison owner:
  `PreviewRunEquivalenceContract`
- issue: [`PL-0071`](../../.proofline/issues/PL-0071.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d013-preview-run-reopen-equivalence-20260915`
- verification: five cases passed with integer exact, floating metric tolerance `1e-5`, coordinate
  tolerance `1e-3`, zero normalized-input max difference, exact decisions, and saved/reopened
  summaries. Missing-input/invalid-acceptance prevalidation, finite/missing-metric acceptance
  gates, and en-US/ko-KR locale stability also passed. `VisionRecipeRunnerSmoke` Debug build
  completed with 0 errors (15 nullable warnings in the new smoke contract).
- boundary: no desktop EXE/WPF click path, rendered overlay-pixel equality, theme/DPI/monitor,
  native hardware, or release/Original mutation was performed. A broader recipe-file-snapshot
  recheck still reproduces its existing A/B `Pipeline has no steps` failure; it is not claimed as
  fixed by this slice.

## Completed scheduled 2D slice — PL-0072 (2D-014 PIXELPERMM finite unit contract)

`PIXELPERMM`의 공개 XML 이름은 유지하고 runtime 의미를 `mm/px`로 고정했습니다. 기존
`> 0` 검사만으로 `Infinity`가 통과하던 공통/직접 계측 경로를 하나의 finite scale·finite
product conversion owner로 연결했으며, `PIXELPERMM=0`은 의도적인 pixel-only 상태로
남겼습니다. 비유한·음수 배율은 실행 전에 Validator가 거부하고, 유한 배율이라도 곱셈이
overflow하면 mm metric을 만들지 않습니다.

- owner/call path: `VisionPipelineMetricEnrichmentService.TryConvertPixelToMillimeters` /
  `AddConvertedMetric` → `VisionPipelineExecutionService.Enrich` 및
  `VisionRecipeRunner.CreateEnrichedMetrics`; `VisionPipelineValidation.ValidatePixelPerMm`
  는 `PIXELPERMM`/`LeftPIXELPERMM`/`RightPIXELPERMM` admission을 담당합니다. 직접
  `LineDistance`, `CurveBandProfile`, `GapEdgePair`, `PinArrayGap`, `InspectionAlgorithm`
  경로도 같은 conversion owner를 재사용합니다.
- issue: [`PL-0072`](../../.proofline/issues/PL-0072.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d014-pixelpermm-finite-unit-20260915`
- verification: `100 px × 0.01 mm/px = 1 mm`을 Line/Bounds/Circle/Geometry/Distance/Curve
  metric에 적용했고, NaN/±Infinity/0/음수/overflow fail-closed, Validator, legacy XML
  save/reopen 계약이 `5/0`으로 통과했습니다. 기존 finite acceptance `10/0`과 Runner
  prevalidation도 통과했으며, OpenVisionLab Debug/Release build는 0 warnings/0
  errors, smoke Debug/Release build는 0 errors/16 nullable warnings였습니다.
- boundary: 실제 WPF calibration 표시, desktop EXE/monitor·theme·DPI/input, 카메라/장비,
  물리 계측 인증, Original 변경은 이 slice에서 검증하지 않았습니다. 양의 배율은
  configuration evidence이지 물리 calibration 인증이 아닙니다.

## Completed scheduled 2D slice — PL-0073 (2D-015 Layer·fixture 참조 불변식)

기존 Validator·normalizer·LineFixture/Affine reference owner·SerializeHelper 경계를
재사용해 Step graph 참조를 fail-closed로 고정했습니다. 직렬 layer, 명시적 branch,
Arithmetic `InputLayerB`, LineFixture `SourceStep/SourceFeature`, detected-point
Affine `StepName/FeatureName`을 포함한 대표 graph에서 producer 삭제·복제·재정렬·stale
Layer/Step rename은 자동 추정 재연결 없이 명시적 오류를 냈고, 모든 참조를 명시적으로
갱신하면 다시 유효해졌습니다. XML save/reopen은 참조 문자열과 validation 결과를
보존했고, normalizer는 끊어진 참조를 복구하지 않았으며, clone을 폐기한 Cancel/Undo
경로는 원본 graph를 보존했습니다.

- owner/call path: Recipe Manager Step edit/XML load → `VisionPipeline` →
  `VisionPipelineValidator.Validate` → `VisionRecipeRunner` admission; typed fixture와
  Affine 검사는 각각 `VisionPipelineLineFixtureService`와
  `VisionPipelineAffinePointBindingService`가 소유합니다. mutable graph writer는
  `VisionPipeline.Steps`와 Step의 `InputLayer`/`OutputLayer`/`Parameters`입니다.
- issue: [`PL-0073`](../../.proofline/issues/PL-0073.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d015-layer-reference-invariant-20260915`
- verification: `--pipeline-layer-reference-invariant-contract` Debug/Release가
  모두 통과했습니다. baseline/XML round-trip, delete/duplicate/reorder/stale rename,
  explicit repair, no-auto-reconnect, clone-discard, valid Arithmetic B execution을
  기록했습니다. 기존 Runner prevalidation과 duplicate-Step identity 회귀 계약도
  Debug/Release에서 통과했으며, smoke Debug/Release build는 각 0 warnings/0 errors입니다.
- boundary: 실제 WPF editor click/Undo/Cancel 화면, desktop EXE monitor·theme·DPI·keyboard
  matrix, camera/hardware, 물리 계측 인증, Original 변경은 이 slice에서 검증하지 않았습니다.

## Completed scheduled 2D slice — PL-0074 (2D-016 실패 후 미실행 Step 표시)

기존 fail-fast 실행 정책은 그대로 두고 `VisionPipelineResultSummaryService`가 전체
effective Pipeline 계획을 기준으로 결과 행을 투영하도록 확장했습니다. 공개
`VisionRecipeRunResult`, Pipeline Review 실행 캐시/flow/progress/guide/result presenter,
그리고 Run Report가 `Executed`, `Disabled`, `NotRunAfterFailure`, `Cancelled` 상태를
구분합니다. 중간 acceptance NG 뒤 enabled tail은 `NOT RUN`이며 NG/first-issue에
추가되지 않고, disabled 행은 `SKIP`, 취소된 현재 Step과 tail은 `CANCEL`로 남습니다.
합성 tail에는 image/object/instance/geometry/metric/overlay가 없고 acceptance도 평가된
것으로 표시하지 않습니다.

- owner/call path: `VisionPipelineExecutionService.RunPreparedAsync`가 실제
  `VisionPipelineRunResult.StepResults`와 Mat/object lifetime을 소유하고,
  `VisionPipelineResultSummaryService.CreateStepSummaries(pipeline, runResult)`가
  전체 계획 상태를 투영합니다. `VisionRecipeRunner.CreateResult`와
  `OpenVisionPipelineReviewExecutionController.CompleteRun`이 이를 public/Review
  결과에 전달하며, flow/progress/guide/result presenter와
  `VisionPipelineRunReportStorage`가 동일 상태를 표시/기록합니다.
- mutable-state/public contract: 실제 Step 결과와 Review Step-index cache만 기존
  owner가 쓰며, `VisionRecipeStepRunSummary`와 `VisionPipelineStepRunReport`에
  `Executed`/`ExecutionState`를 추가했습니다. 기존 `Status`, `Success`, `Skipped`,
  acceptance/image/object/metric/`ResultCount` 의미는 유지했습니다.
- issue: [`PL-0074`](../../.proofline/issues/PL-0074.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d016-not-run-tail-status-20260915`
  및 `...-release`; WPF capture는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d016-wpf-pipeline-review-20260915`입니다.
- verification: focused tail contract는 Debug/Release 모두 `passed=3|failed=0`;
  cancellation `5/0`, failure lifetime `7/0`, prevalidation, duplicate-Step identity,
  Review result/status, Review guide/result 회귀가 Debug/Release에서 통과했습니다.
  VisionRecipeRunnerSmoke Debug/Release와 PipelineViewerScreenshotSmoke Release build는
  각각 0 warnings/0 errors였고, single-monitor Korean `wpf_shell_host_pipeline_review`
  capture가 통과했습니다.
- reading order: `VisionPipelineExecutionService.cs` →
  `VisionPipelineResultSummary.cs` → `VisionRecipeRunner.cs` →
  `OpenVisionPipelineReviewExecutionController.cs` → Review projection/presenter files →
  `VisionPipelineRunReportStorage.cs` → `PipelineNotRunTailStatusContract.cs`.
- boundary: WPF full theme, Wide/Compact, keyboard/mouse states, 100/125/150/175/200%
  DPI matrix, camera/hardware, physical metrology, release/publication, Original 변경은
  이 slice에서 검증하지 않았습니다.

## Scheduled boundary covered by PL-0075 — 2D-017 (미래 XML schema fail-closed 검증)

다음 ready boundary는 지원하는 Pipeline XML schema 범위를 명시하고, 알 수 없는 미래
version 또는 required 의미를 조용히 버리고 실행하지 않도록 load/inspect 경계를 검증하는
것입니다. 현재/이전 지원 파일과 optional extension/comment round-trip은 보존하되,
미래 critical XML은 read-only inspect와 원본 보존을 허용하고 실행은 명시적 오류로
차단합니다. 기존 PL-0067(공용 Runner/UI pre-validation)과 PL-0069 이후의 Step-index
결과 identity owner는 재분할하지 않습니다.

## Completed scheduled 2D slice — PL-0075 (2D-017 미래 XML schema fail-closed 검증)

Pipeline XML의 지원 범위를 unversioned legacy와 명시적 schema v1로 고정했습니다.
미래 version, 지원하지 않는 version, unknown unqualified critical element/attribute,
unsupported namespace는 기존 XML bytes를 보존한 채 `SerializeHelper` load와
`VisionPipelineExecutionPlan` 실행 admission 전에 typed error로 차단합니다. 이름이
명시된 `<Extensions>` container, `urn:openvisionlab:extension` namespace, XML comment는
선택적 비의미 확장으로 허용합니다. `VisionPipelineStorage.Load`는 semantic schema
failure에서 source를 backup/default로 덮어쓰지 않고 `LoadFailed`를 기록합니다.

- owner/call path: raw Pipeline XML → `SerializeHelper.TryLoadFromXmlBytes/Text`의
  `VisionPipelineXmlSchemaPolicy` → `VisionPipelineStorage.Load` 또는
  `VisionPipelineExecutionPlan.Create` → 기존 Validator/Runner admission. mutable
  source writer는 기존 caller와 `SerializeHelper.SaveXmlFile`이며, blocked source에는
  저장 writer가 호출되지 않습니다.
- public/binding contract: 기존 `TryLoad` 실패·error message와
  `VisionPipelinePersistenceStateKind.LoadFailed`를 재사용했습니다. 새 UI enum이나
  ViewModel 경계는 만들지 않았고 기존 LoadFailed/Run guard가 명시적 차단을 표시합니다.
- reading order: `VisionPipelineXmlSchemaPolicy.cs` → `SerializeHelper.cs` →
  `VisionPipelineStorage.cs` → `VisionPipelineExecutionPlan.cs` →
  `PipelineXmlSchemaCompatibilityContract.cs`.
- issue: [`PL-0075`](../../.proofline/issues/PL-0075.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d017-xml-schema-20260915-debug`,
  `...-release`, `...-regressions`
- verification: Debug/Release `VisionRecipeRunnerSmoke` build가 각각 오류 0으로
  완료했고, 새 schema compatibility contract가 각각 `passed=7|failed=0`으로
  통과했습니다. contract는 legacy/v1 load, optional extension/comment, future v2와
  unknown critical element의 typed fail-closed, execution-plan 차단, storage source
  byte 보존·LoadFailed/no-backup를 확인했습니다. `recipe-load-recovery-contract`와
  `pipeline-layer-reference-invariant-contract` 회귀도 통과했습니다. 기존 nullable
  경고 16개는 변경 범위 밖에 남아 있습니다.
- boundary: XML opaque extension/comment의 canonical save 후 byte-preserving round-trip,
  실제 WPF 오류 dialog/theme/Wide-Compact/DPI/keyboard, desktop EXE monitor matrix,
  camera/hardware, release/publication, Original 변경은 이 slice에서 검증하지 않았습니다.

## Completed scheduled 2D slice — PL-0076 (2D-018 현재/기본/단위/locale XML round-trip)

현재 serializer와 PropertyGrid/app-tool mapper를 재사용해 Pipeline XML의 현재값,
생략 기본값, 단위, locale 경계를 닫았습니다. `ko-KR`, `en-US`, `de-DE`에서 저장·재개방
후 typed parameter, bool/enum, `PIXELPERMM`, acceptance 판정, 한글 identity/template
path가 보존되고 생성 XML byte도 동일함을 확인했습니다. 생략 Blob parameter는 source에
주입하지 않으면서 기존 mapper의 effective default와 명시적 default set이 일치합니다.
올바른 XML 선언의 UTF-16 파일은 재개방되며, de-DE comma-decimal과 잘못된 enum은
inspectable source로 남되 invariant Validator와 Runner admission에서 차단됩니다.

- owner/call path: `SerializeHelper.SaveXmlFile/TryLoadFromXmlFile` →
  `VisionPipelineStepBuilder`/`VisionPipelineStepPropertyMapper` →
  `VisionPipelineAppToolFactory` default mapping → `VisionPipelineValidator` →
  `VisionRecipeRunner` pre-execution admission
- mutable-state/public contract: 기존 `VisionPipeline`/`VisionPipelineStep.Parameters`와
  `SaveXmlFile` caller만 XML을 쓰며, mapper는 effective property만 생성합니다. 기존
  `TryLoad`/Validator/Runner failure contract와 invariant typed parsing을 유지했고 새
  serializer·format·UI state는 추가하지 않았습니다.
- reading order: `src/OpenVisionLab/Common/Persistence/SerializeHelper.cs` →
  `src/OpenVisionLab/Core/Pipeline/Definition/VisionPipelineStepBuilder.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/PropertyGrid/VisionPipelineStepPropertyMapper.cs`
  및 `VisionPipelineObjectInspectionPropertyAdapter.cs` →
  `src/OpenVisionLab/Core/Pipeline/Definition/VisionPipelineAppToolFactory.cs` →
  `src/OpenVisionLab/Core/Pipeline/Validation/VisionPipelineValidation.cs` →
  `tools/VisionRecipeRunnerSmoke/PipelineXmlRoundTripCompatibilityContract.cs`
- issue: [`PL-0076`](../../.proofline/issues/PL-0076.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d018-xml-roundtrip-20260915-debug`,
  `...-release`, `...-regressions`, and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d018-public-samples-valid-20260915`
- verification: `VisionRecipeRunnerSmoke` Debug/Release builds completed with 0 errors;
  current/default/unit/locale contract passed `4/0` in both configurations. PL-0075 schema
  compatibility passed `7/0`, recipe-load recovery `8/0`, and layer/reference invariant
  passed. `RecipeXmlCompatibilityCheck` passed all 13 XML roots and 106 public sample XML
  files when pointed at `docs\\samples\\public`. The broader generated `bin\\Debug\\RECIPE`
  scan still reports its pre-existing empty default Pipeline (`Pipeline has no steps`),
  recorded as a boundary rather than changed here.
- boundary: no desktop EXE/WPF click path, theme/Wide-Compact/DPI/input matrix,
  camera/hardware, physical calibration, release/publication, or Original-repository
  mutation was performed. UTF-16 evidence covers correctly declared XML; arbitrary
  encoding-declaration mismatch remains invalid input. The broader PL-0008
  `recipe-file-snapshot` recheck remains `passed=5|failed=1` because its A/B generation
  fixture is an empty Pipeline and the existing Validator reports `Pipeline has no steps`;
  this known baseline was not changed by 2D-018.

## Completed scheduled 2D slice — PL-0077 (2D-019 손상 Recipe 명시적 복구 전 실행 차단)

손상된 기존 Pipeline XML/Recipe data가 메모리 대체 객체로 복원되어도 정상 Recipe로
오인되어 실행되지 않도록 실제 Run owner의 persistence gate를 닫았습니다. 새 Recipe의
누락 파일 기본 생성은 그대로 허용하고, 외부에서 XML을 고쳐도 기존 failure state는
지워지지 않으며 명시적인 `VisionPipelineStorage.Save`/`RecipeDataStorage.Save` 후
`SaveRecovered`가 되어야 실행됩니다. 원본 hash와 `.invalid-*` backup은 격리 계약에서
확인했습니다.

- owner/call path: `VisionPipelineStorage.Load`/`RecipeDataStorage.Load` →
  `PersistenceStateKind` 및 source/backup state →
  `OpenVisionRecipePersistenceStatusPresenter` →
  `OpenVisionPipelineReviewDocument.RunReviewAsync` 및
  `RecipeCommandSurface`의 sample/pair/catalog/validation/local-validation
  `CanRun*` gate → explicit SaveRecovered 이후 execution
- mutable-state/public contract: `VisionPipelineStorage.Save`와
  `RecipeDataStorage.Save`만 recovery state를 쓰며 Review document는 state를 읽어
  validation/guide를 표시합니다. 기존 editable-substitute, missing-file default,
  schema `LoadFailed`, command surface contract는 유지했습니다.
- reading order: `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineStorage.cs` →
  `src/OpenVisionLab/Core/Recipe/RecipeDataStorage.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipePersistenceStatusPresenter.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Models/OpenVisionRecipeValidationReviewModels.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs` →
  `tools/VisionRecipeRunnerSmoke/RecipePersistenceExecutionGateContract.cs`
- issue: [`PL-0077`](../../.proofline/issues/PL-0077.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d019-persistence-gate-20260915-debug`,
  `...-release`, and the regression reports under the Debug `regressions` directory
- verification: Debug/Release `VisionRecipeRunnerSmoke` builds completed with 0 errors
  and the existing 16 nullable warnings outside this slice. The focused persistence
  execution contract passed `9/0` in both configurations. Recipe-load recovery passed
  `8/0`, future-schema compatibility `7/0`, and layer/reference invariant passed.
- boundary: source/in-process Run guard and storage contracts are verified. Desktop
  WPF/EXE click path, rendered recovery state, theme/Wide-Compact/DPI/input matrix,
  camera/hardware, release/publication, and Original repository remain unverified or
  out of scope.

## Completed scheduled 2D slice — PL-0078 (2D-020 누락·이동·손상 외부 자산의 식별과 재연결 검증)

기존 review-bundle/asset/qualification owner를 대조하고, 새 bundle이나
`QualifiedRecipeSnapshotStore` 중복 없이 외부 자산 portability 경계를 계약으로
검증했습니다. review bundle은 원본 경로·인접 SHA 일치 후보·누락·동명이 파일의
내용 불일치를 구분하고 영향 Step/parameter를 보고합니다. 후보는 자동 적용하지
않으며, 사용자가 XML 경로를 명시적으로 수정하고 다시 검증한 뒤에만 Recipe
workspace 복사와 저장이 진행됩니다. 채택된 파일의 원본 bytes는 보존되고 저장된
Pipeline content hash가 Recipe revision 경계를 남깁니다.

- owner/call path: `OpenVisionRecipeReviewBundleExporter` →
  `OpenVisionRecipeReviewBundleInspector.TryInspect` →
  `OpenVisionRecipeDependencyReviewService.Review` → explicit XML path update →
  `copyDependencies: true` → `VisionPipelineStorage.Save`; qualification path는
  `QualifiedRecipeSnapshotPreflight` → `QualifiedRecipeSnapshotStore` →
  `QualifiedRecipeSnapshotWorkingCopyService`로 별도 archive/hash verification을
  소유합니다.
- mutable-state/public contract: dry review는 bundle/pipeline/Recipe/workspace를
  변경하지 않습니다. 명시적 import/copy만 Pipeline parameter를 data-relative path로
  바꾸고 Recipe XML을 저장합니다. `Found`, `RelocationCandidate`, `Missing`,
  `ContentMismatch` 분류와 Preview/Run 미실행 정책은 기존 contract를 유지합니다.
- reading order: `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeReviewBundleExporter.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeReviewBundleInspector.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeDependencyReviewService.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/CommandSurface/RecipeCommandSurface.cs` →
  `src/OpenVisionLab/Core/Recipe/Qualification/QualifiedRecipeSnapshotPreflight.cs` →
  `src/OpenVisionLab/Core/Recipe/Qualification/QualifiedRecipeSnapshotStore.cs` →
  `src/OpenVisionLab/Core/Recipe/Qualification/QualifiedRecipeSnapshotWorkingCopyService.cs` →
  `tools/VisionRecipeRunnerSmoke/RecipeExternalAssetReconnectionContract.cs`
- issue: [`PL-0078`](../../.proofline/issues/PL-0078.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d020-asset-reconnection-20260915-debug\contract-final3`,
  `...-debug\regression-review-bundle`, `...-debug\regression-persistence`, and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d020-asset-reconnection-20260915-release\contract-final\qualification`
- verification: Debug/Release `VisionRecipeRunnerSmoke` builds completed with 0 errors
  and the existing 16 nullable warnings outside this slice. The focused asset
  reconnection contract passed `17/0` in both configurations, existing review-bundle
  projection passed, persistence execution gate passed `9/0`, and
  `QualifiedRecipeSnapshotSmoke` completed `qualified_recipe_snapshot_core=OK`.
- boundary: source/in-process asset and qualification contracts are verified. Desktop
  WPF/EXE click path, rendered candidate selection, ACL/read-permission failure,
  themes/Wide-Compact/DPI/input matrix, camera/hardware, field qualification,
  release/publication, and Original repository remain unverified or out of scope.

## Completed scheduled 2D slice — PL-0079 (2D-021 저장 실패 주입으로 기존 XML·백업 보존 검증)

기존 `SerializeHelper` atomic writer를 재작성하지 않고 격리 fixture 전용 failure
injection seam을 추가해 write·replace·cleanup 경계를 검증했습니다. 실패한 저장은
이전 XML hash를 보존하고 성공 상태를 만들지 않으며, cleanup 실패로 남은 임시 파일은
기존 XML을 건드리지 않고 명시적으로 정리할 수 있습니다. 정상 retry는 새 XML을
완전히 게시합니다.

- owner/call path: `SerializeHelper.SaveXmlFile` → hidden sibling temporary XML write →
  `ReplaceFile` (`File.Replace` existing target / `File.Move` missing target) → `finally`
  temporary cleanup; `RecipeState.SaveTools`는 Tool 저장 실패 시 dependent data 저장을
  중단하고, `VisionPipelineStorage.Save`/`RecipeDataStorage.Save`는 기존 persistence
  failure state를 유지합니다.
- mutable-state/public contract: target XML과 `.invalid-*` backup은 atomic replace가
  성공하기 전 변경되지 않습니다. write/replace 실패는 기존 bytes/hash와 실패 상태를
  보존하고, cleanup failure는 격리된 temporary artifact만 남길 수 있습니다. 주입 seam은
  test-only이며 기본 실행에는 영향이 없습니다.
- reading order: `src/OpenVisionLab/Common/Persistence/SerializeHelper.cs` →
  `src/OpenVisionLab/Core/Recipe/RecipeState.cs` →
  `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineStorage.cs` →
  `tools/VisionRecipeRunnerSmoke/RecipeSaveFailureContract.cs` →
  `tools/VisionRecipeRunnerSmoke/RecipeMultiFileSaveRecoveryContract.cs`
- issue: [`PL-0079`](../../.proofline/issues/PL-0079.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915-debug\build-smoke-debug-injection.log`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915-debug\injected-contract\recipe-save-failure-contract.txt`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915-release\build-smoke-release-injection.log`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915-release\injected-contract\recipe-save-failure-contract.txt`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915\debug-multi-file`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915\debug-persistence`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915\debug-recovery`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915\release-multi-file`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915\release-persistence`, and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d021-save-failure-20260915\release-recovery`
- verification: Debug/Release `VisionRecipeRunnerSmoke` builds completed with 0 errors and
  the existing 16 nullable warnings outside this slice. The injected save contract passed
  `13/0` in both configurations; multi-file save recovery passed `7/0`; persistence execution
  gate passed `9/0`; and load recovery passed `8/0` in both configurations.
- boundary: isolated source/in-process contracts and file-lock failure simulation are verified.
  WPF/EXE click path, toast suppression in the rendered UI, themes/Wide-Compact/DPI/input
  matrix, ACL/read-permission failures, camera/hardware, field qualification, clean-checkout
  RC distribution, release/publication, and Original repository remain unverified or out of scope.

## Completed scheduled 2D slice — PL-0080 (2D-022 Recipe lifecycle journal 단계별 프로세스 종료 복구 검증)

2D-022와 동일 범위의 기존 PL-0009/OVL-06a process-boundary owner와 증거를
재검토했습니다. 현재 `VisionPipelineStorage` lifecycle code는 해당 증거 이후
변경되지 않았고, 새 journal·recovery writer나 중복 harness는 필요하지 않습니다.
기존 child process kill/reopen 계약은 rename 6단계와 delete 5단계를 모두 통과시켜
이전 상태 rollback 또는 검증된 완료 상태 채택 중 하나만 남기고, 모호한 상태는
`LifecycleRecoveryRequired`로 설명합니다.

- owner/call path: `VisionPipelineStorage.TryRenamePipeline`/`TryDeletePipeline` →
  `SaveLifecycleJournal` → transaction-owned backup → target/active-pointer/source
  stage → journal/backup cleanup; storage reopen
  (`LoadActivePipelineName`/`Load`/`TryGetPersistenceState`) →
  `TryRecoverPendingLifecycleTransactionCore` → prior-state rollback 또는 proven
  completed-state adoption → `LifecycleRecovered`/`LifecycleRecoveryRequired`.
- mutable-state/public contract: journal, backup, target, pointer, and source are
  changed only by the existing lifecycle owner. Recovery never silently deletes an
  uncertain file and never runs Preview/Run or changes layer/routing state.
- reading order: `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineStorage.cs` →
  `src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelinePersistenceState.cs` →
  `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipePersistenceStatusPresenter.cs` →
  `tools/VisionRecipeRunnerSmoke/Program.cs` (`RunPipelinePersistenceProcessRecoveryContract`)
- issue: [`PL-0080`](../../.proofline/issues/PL-0080.json); prior owner/evidence:
  [`PL-0009`](../../.proofline/issues/PL-0009.json)
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl06-process-recovery-20260907\run-final\pipeline_persistence_process_recovery_contract.txt` and
  `...\run-dotnet-r2\pipeline_persistence_process_recovery_contract.txt`; source and
  method boundary are documented in
  `docs/reports/OPENVISIONLAB_OVL06_BEHAVIOR_REGRESSION_PROCESS_RECOVERY_20260907.md`.
- verification: existing current-owner evidence records `Cases=11`, `Recovered=11`,
  `ChildLaunches=11`, `ChildKills=11` for both apphost and `dotnet` DLL launchers;
  all rename/delete stages left no journal, lifecycle backup, or temporary pointer file.
  Same-process PL-0009 recovery and storage-path regressions also passed. No duplicate
  process harness was run in this cycle because the responsibility is already protected
  as complete and the lifecycle owner has no source diff after that evidence.
- boundary: process-stop after durable journal stages is verified. Power loss,
  filesystem/disk corruption, non-cooperative native worker shutdown, full WPF
  theme/Wide-Compact/DPI/input matrix, camera/hardware, clean-checkout RC distribution,
  release/publication, and Original repository remain unverified or out of scope.

## 2D-023 공개 파일 입력 경계 감사 — 제품 크기 정책 대기

2D-023 source/package audit는 완료했지만, 일반 Pipeline XML, locator JSON, local
integration message, generic artifact validator에 공통 최대 바이트가 없음을
확인했습니다. review bundle은 `pipeline.xml=5 MiB`, `manifest=2 MiB`, shared TCP
transport는 기본 `file=4 GiB`, `transaction=16 GiB`, `control=1 MiB`,
`relative path=1 KiB`를 사용합니다. 따라서 현재 acceptance의 “모든 입구에서
oversized를 계산 전에 명확히 거절”은 제품 상한과 typed error mapping 없이는
완료할 수 없습니다.

기존 AppPath/PL-0007 root·traversal·reserved-device·control/trailing·case
collision owner와 `TwoDIntegrationExchange`의 root/reparse 검사는 유지합니다.
한글/정상 segment 회귀는 기존 증거를 채택했고, 명시적 Windows long-path 조합은
아직 검증하지 않았습니다. 임의 상한이나 새 보안 framework를 추가하지 않았습니다.

- 상세 audit: `docs/reports/OPENVISIONLAB_2D_PUBLIC_FILE_INPUT_BOUNDARY_AUDIT_20260915.md`
- D: evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d023-file-input-20260915\`
- 재개 조건: local XML/JSON/message/artifact의 per-entry·aggregate 상한, TCP
  상속 여부, oversized typed error 계약에 대한 제품 결정

## Completed scheduled 2D slice — PL-0082 / 2D-024 (튜닝 데이터와 별도 평가 데이터의 내용 중복 검출)

기존 SampleCheck/N-image/Good-Bad/evaluation 진입을 대조한 결과, 일반 Local
Validation Set은 `OK`/`NG`와 경로 중복만 알고 있고 Tool View N-image는
source SHA-256을 보존하지만 `UNLABELED` 실행 증거로 남는 것을 확인했습니다.
따라서 일반 데이터셋에 임의의 tuning/evaluation 역할을 부여하지 않고, 이미
`Train`/`Validation`/`Test` 역할을 동결하는 PinArrayGap identity owner에만
최소 보완을 적용했습니다.

- `OpenVisionRecipePinArrayGapValidationRecordStorage`가 split 내부와
  Train/Validation/Test 사이의 현재 파일 SHA-256 중복을 거절합니다. 동일 bytes를
  다른 파일명/경로로 등록해도 역할별 오류로 동결되지 않습니다.
- 동결 record의 `Role`, `SetName`, `ContentSha256`는 그대로 보존됩니다.
  set 이름/역할 변경이나 source bytes 변경은 stale로 평가되며 기존 record는
  재작성되지 않습니다.
- 유사 이미지·재인코딩 감지는 보장하지 않으며, 일반 Local Validation Set의
  `OK`/`NG`와 N-image의 `UNLABELED` semantics를 변경하지 않았습니다.

- 상세 감사: `docs/reports/OPENVISIONLAB_2D_VALIDATION_CONTENT_HASH_AUDIT_20260915.md`
- D: evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d024-content-hash-20260915-run\`
- 원장: `.proofline/issues/PL-0082.json` (`resolved`)
- 검증: Debug/Release build 0 errors; Debug/Release
  `pinarraygap-validation-identity-owner` 각각 7/7 pass

## Completed scheduled 2D slice — PL-0083 / 2D-025 (평가 중 Recipe·입력·SDK 변경 시 기존 자격 결과 무효화 검증)

기존 `QualifiedRecipeSnapshotStore`가 보존하는 immutable payload와 현재
검증 identity를 분리해 대조했습니다. Recipe/Pipeline 이름과 현재 파일
bytes, 선택된 Validation Set의 실제 input/dependency SHA-256·순서·expected/
variant/metric identity, 그리고 현재 로드된 SDK/runtime 파일의 경로·버전·크기·
SHA-256이 하나라도 달라지면 controller의 Verify와 목록 projection이
`Qualification stale`로 닫힙니다. 과거 payload/보고서는 삭제하지 않고 열람할 수
있으며, 재평가 전 승인 상태를 자동 복구하지 않습니다.

- owner/call path: `RecipeCommandSurface` contextual list/Verify →
  `OpenVisionRecipeQualifiedSnapshotController.EvaluateCurrentIdentity` →
  `QualifiedRecipeSnapshotStore.Verify` + current identity comparison
- 상세 보고서: [`OPENVISIONLAB_2D_QUALIFICATION_IDENTITY_AUDIT_20260915.md`](../reports/OPENVISIONLAB_2D_QUALIFICATION_IDENTITY_AUDIT_20260915.md)
- 원장: `.proofline/issues/PL-0083.json` (`resolved`)
- D: evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d025-qualification-20260915`
- 검증: `QualifiedRecipeSnapshotSmoke` Debug/Release build 및 실행 통과. 동일
  Snapshot 재열기, Recipe parameter/input bytes/SDK replacement stale, stale 중
  과거 evidence 열람, 재평가 전 새 Snapshot 생성 거절을 모두 확인했습니다.
- 경계: `wpf_shell_host_recipe_qualified_snapshot`은 두 번 모두 기존
  `Batch row 1: stored source snapshot is missing or changed` preflight에서
  Snapshot 생성 전에 중단되어, 이번 변경의 stale UI 상태는 실제 렌더링까지
  검증하지 못했습니다. 이 owner의 이번 source diff에는 hash helper 접근범위
  변경만 있고 preflight 판정 로직 변경은 없습니다. 전체 theme/DPI/native
  hardware 행도 별도 미검증입니다.

## Completed scheduled 2D slice — PL-0084 / 2D-026 (ExpectedFailure 실행 오류와 품질 NG 분리)

기존 runner가 이미 제공하던 `ToolSuccess`, `AcceptanceEvaluated`,
`AcceptancePassed`, `Status`, `ErrorCode`, `ErrorName`을 재사용해
`ExpectedFailure`를 typed contract로 닫았습니다. `QualityNG`는 tool-successful
step의 acceptance NG만 통과하고, `ControlledNoResult`는 기존 no-result
diagnostic allow-list와 선택적 expected error/failed step이 일치할 때만
통과합니다. timeout, cancellation, invalid tool, ROI/input/template 또는
unknown error는 정상 NG로 승격되지 않고 `ERROR`/incomplete execution으로
남습니다. 선택 필드가 없는 기존 CSV row는 호환성을 유지하되 `Legacy`로
표시합니다.

- owner/call path: `VisionPipelineSampleCatalogItem.LoadRunnable` →
  `VisionPipelineExpectedFailureContract.Evaluate` →
  `VisionPipelineSampleCheckService.RunSampleCheckAsync`; 독립 CLI는
  `tools/RunVisionSampleCatalog.ps1`이 같은 typed output/metric/artifact
  contract를 재평가합니다.
- optional catalog columns: `ExpectedOutcome`, `ExpectedError`,
  `ExpectedFailedStep`; 대표 product row 3개는 strict contract로 명시했고
  나머지 row는 legacy-compatible입니다.
- 상세 보고서: [`OPENVISIONLAB_2D_EXPECTED_FAILURE_CONTRACT_20260915.md`](../reports/OPENVISIONLAB_2D_EXPECTED_FAILURE_CONTRACT_20260915.md)
- 원장: `.proofline/issues/PL-0084.json` (`resolved`)
- D: evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d026-debug-20260915-final`,
  `2d026-release-20260915-final`, `2d026-script-pass-20260915`,
  `2d026-script-fail-20260915`
- 검증: Debug/Release build 0 errors; ExpectedFailure contract 각각 8/8
  pass; bounded catalog strict fixture 3/3 `GateStatus=OK`; invalid/missing
  pipeline fixture는 의도대로 nonzero exit와 `GateStatus=NG`, missing artifact를
  보존했습니다. PowerShell parse도 pass입니다.
- 경계: 전체 catalog 재실행, 실제 runner의 non-finite metric emission,
  hardware/장시간 timeout, WPF theme/DPI matrix는 이번 slice에서 실행하지
  않았습니다.

## Completed scheduled 2D slice — PL-0085 / 2D-027 (오류·미실행 분리 혼동행렬)

기존 `VisionPipelineBatchOutcomeContract`와 Run History projection을 재사용해
`OK`를 positive acceptance class로 정의한 TP/TN/FP/FN을 제공하고, 실행 오류,
partial run 미실행, unknown expected label을 별도 집계합니다. 새 Local
Validation Set 실행은 등록된 전체 입력 수를 `InputSampleCount`로 저장하므로
중단된 실행도 결과 행 수와 입력 분모를 혼동하지 않습니다. legacy summary는
관찰된 결과 수로 fallback하며, 0분모 비율은 `N/A`입니다.

- owner/call path: `OpenVisionRecipeValidationSetRunner` →
  `VisionPipelineBatchRunSummaryStorage.Save(inputSampleCount)` →
  `VisionPipelineBatchOutcomeContract.BuildConfusionMatrix` →
  `OpenVisionRecipeBatchRunOption` →
  `OpenVisionRecipeRunHistoryPresenter.BuildConfusionMatrixText` → 기존
  `RecentBatchRunComparisonSummaryText` binding
- 상세 보고서:
  [`OPENVISIONLAB_2D_CONFUSION_MATRIX_20260915.md`](../reports/OPENVISIONLAB_2D_CONFUSION_MATRIX_20260915.md)
- 원장: `.proofline/issues/PL-0085.json` (`resolved`)
- D: evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d027-confusion-matrix-20260915-final`,
  `2d027-confusion-matrix-20260915-release-final`
- 검증: Debug/Release smoke build 0 errors, focused confusion-matrix contract
  각각 3/3 pass. four-cell matrix, execution error, not-run, unknown label,
  zero-sample `N/A`, save/reload projection을 포함합니다.
- 경계: 전체 catalog와 실제 WPF 렌더링·theme/DPI/monitor/input matrix는 이번
  slice에서 실행하지 않았습니다.

## Completed scheduled 2D slice — PL-0086 / 2D-028 (비연속 Mat·stride·ROI view 입력 소유권)

기존 `BitmapImageConverter`와 PL-0006 contract를 재사용해 실제 공백이었던
non-contiguous ROI/padded stride, caller/output 수명, disposed input 경계를
별도 smoke contract로 닫았습니다. zero-copy 재작성이나 PL-0006 생산 코드
재개발은 하지 않았습니다.

- owner/call path: `MatViewOwnershipContract` →
  `BitmapImageConverter.ToBitmap/ToMat` → 기존 row-byte/stride 및 clone-copy
  경계
- 상세 보고서:
  [`OPENVISIONLAB_2D_MAT_VIEW_OWNERSHIP_20260915.md`](../reports/OPENVISIONLAB_2D_MAT_VIEW_OWNERSHIP_20260915.md)
- 원장: `.proofline/issues/PL-0086.json` (`resolved`)
- D: evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d028-debug-20260915`와
  `2d028-release-20260915`
- 검증: Debug/Release smoke build 0 errors, 2D-028 contract 각각 3/3 pass,
  PL-0006 regression 각각 5/5 pass. 기존 nullable warning 16개는 변경 없는
  smoke contract 파일에 남아 있습니다.
- 경계: 16-bit 의미 정규화, 전체 catalog, 실제 WPF 렌더링·theme/DPI/monitor/input
  matrix, hardware/장시간 memory qualification은 이번 slice에서 실행하지
  않았습니다.

## Completed scheduled 2D slice — PL-0087 / 2D-029 (Gray/BGR/BGRA/16-bit 입력 의미 대조)

검사 Runner와 ImageCanvas 표시의 입력 의미를 분리해 고정했습니다. Runner는
`VisionPipelineSampleCheckService.RunSampleCheck*`에서
`ImreadModes.Unchanged`로 원본 depth/decode를 먼저 확인한 뒤 기존
`BitmapImageConverter`로 전달합니다. 8-bit Gray/BGR/BGRA는 기존 channel 의미를
유지하고, 16-bit depth와 손상 파일은 계산 전에 명확한 오류로 끝납니다.

ImageCanvas의 `CanvasImageLoader.LoadMatFromFile`은 표시 전용으로 16-bit를
8-bit로 축소하고 BGRA alpha를 제거할 수 있지만, 이 Mat을 검사 입력으로
재사용하지 않습니다. 첫 계약에서 GDI+ `Bitmap`만 검사할 때 16-bit PNG가
8-bit처럼 받아들여질 수 있는 결함을 재현했고, 기존 sample-check owner의
사전 검사 한 곳으로 보완했습니다.

- 상세 보고서:
  [`OPENVISIONLAB_2D_INPUT_FORMAT_MEANING_20260915.md`](../reports/OPENVISIONLAB_2D_INPUT_FORMAT_MEANING_20260915.md)
- 원장: `.proofline/issues/PL-0087.json` (`resolved`)
- D: evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d029-debug-20260915`,
  `2d029-release-20260915`
- 검증: Debug/Release build 0 errors; input-format contract는 8-bit
  Gray/BGR/BGRA, 16-bit 계산 전 거절, 손상 decode 거절, display-only 16-bit/
  alpha mapping을 모두 통과했습니다. 기존 Bitmap converter regression은
  Debug/Release 5/5, Preview/Run/Reopen Mean·Blob·Contour·Matching·Line은
  Debug/Release 모두 PASS입니다.
- 경계: 실제 WPF EXE 표시 픽셀, theme/layout/DPI/monitor/input interaction,
  camera/hardware와 장시간 운전은 검증하지 않았습니다.

## Completed scheduled 2D slice — PL-0088 / 2D-030 (ROI·fixture·회전·flip 후 좌표와 Overlay 일치 검증)

기존 fixture/affine owner를 재사용해 identity/translation/90-degree rotation/
horizontal flip의 pixel matrix, destination point overlay, XML reopen, Run History
metric, overlay export가 같은 좌표 의미를 유지하는지 검증했습니다. fixture
identity/rounded translation은 저장된 `CvROI`를 바꾸지 않고 runtime effective ROI와
offset metric만 만듭니다. translation-only limit을 넘는 rotation과 missing frame,
singular destination triangle은 fail-closed입니다. 비등방 affine은 pixel frame으로
남기며 scalar mm/px를 추론하지 않습니다.

- owner/call path: `VisionPipelineExecutionService` →
  `VisionPipelineFixtureFrameService` / `VisionPipelineAffinePointBindingService` →
  existing Vision SDK Affine tool → `VisionRecipeRunner` →
  `VisionPipelineRunReportStorage`
- 상세 보고서:
  [`OPENVISIONLAB_2D_COORDINATE_TRANSFORM_MEANING_20260916.md`](../reports/OPENVISIONLAB_2D_COORDINATE_TRANSFORM_MEANING_20260916.md)
- 원장: `.proofline/issues/PL-0088.json` (`resolved`)
- D: Debug/Release contract와 regressions는 상세 보고서의
  `2d030-debug-20260916`/`2d030-release-20260916` 경로에 보관했습니다.
- 검증: Debug/Release build 0 errors; coordinate-transform contract와 기존
  Affine, detected-point Affine, ROI, finite-PIXELPERMM regressions 모두 exit 0.
- 경계: 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input interaction,
  camera/hardware와 장시간 운전은 이번 slice에서 검증하지 않았습니다.

## Completed scheduled 2D slice — PL-0089 / 2D-031 (Matching 무검출·다중검출·score 경계 계약)

기존 Matching property/SDK 실행과 결과 캡처·보고서 owner를 재사용해 blank no-result,
`NUM_MATCH=2` multiple-result, `SCORE_MIN` 경계의 ResultCount, score, acceptance,
overlay, XML reopen, Run History 의미를 같은 execution snapshot에서 검증했습니다.
계약 fixture는 SDK의 기본 threshold 전처리가 synthetic 입력을 재해석하지 않도록
`USE_THRESHOLD=false`, `USE_ADAPTIVE_THRESHOLD=false`, `USE_CANNY=false`와 전체 ROI를
명시했으며 production default는 변경하지 않았습니다.

- owner/call path: `VisionPipelineAppToolFactory/CreateMatchingTool` →
  `VisionPipelineExecutionService` → existing Matching result capture/summary →
  `VisionPipelineRunReportStorage`
- 상세 보고서:
  [`OPENVISIONLAB_2D_MATCHING_BOUNDARY_20260916.md`](../reports/OPENVISIONLAB_2D_MATCHING_BOUNDARY_20260916.md)
- 원장: `.proofline/issues/PL-0089.json` (`resolved`)
- D: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d031-debug-20260916s` 및
  `2d031-release-20260916`
- 검증: Debug/Release build 0 errors; Matching boundary contract와 기존
  expected-failure 및 Preview/Run/Reopen 회귀 모두 exit 0.
- 경계: 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input interaction,
  camera/hardware와 장시간 운전은 이번 slice에서 검증하지 않았습니다.

## Completed scheduled 2D slice — PL-0090 / 2D-032 (Blob/Contour 후보·제외 사유와 단일 실행 결과 일치)

기존 SDK `candidates`와 `TryCaptureSdkCandidates/CaptureCandidates` owner를 재사용해
Blob/Contour의 혼합 accepted/rejected 후보, 동일 면적 객체, 두 ROI, exact area와
width/height 경계를 한 번의 실행 결과와 대조했습니다. CandidateId·native/region
index·SourceImage 좌표·generation stage·AppliedLimits·reject code/text가 최종
ResultCount, area/dimension metric, accepted overlay와 Run History 행에 보존되며,
동일 입력 반복에서도 ordering이 변하지 않음을 확인했습니다. relaxed Tool 재실행,
후보 수집 재구현, 새 candidate abstraction은 추가하지 않았습니다.

- owner/call path: existing `BlobTool`/`ContourTool` candidates →
  `VisionPipelineObjectResultCaptureService` → `VisionPipelineObjectResult` →
  `VisionRecipeRunner` summary / `VisionPipelineRunReportStorage`
- 상세 보고서:
  [`OPENVISIONLAB_2D_BLOB_CONTOUR_CANDIDATE_BOUNDARY_20260916.md`](../reports/OPENVISIONLAB_2D_BLOB_CONTOUR_CANDIDATE_BOUNDARY_20260916.md)
- 원장: `.proofline/issues/PL-0090.json` (`resolved`)
- D: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d032-debug-20260916` 및
  `2d032-release-20260916`
- 검증: Debug/Release build 0 errors; 새 Blob/Contour boundary contract와
  기존 object-candidate parity, object-dimension, expected-failure,
  Preview/Run/Reopen 회귀 모두 exit 0.
- 경계: 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input interaction,
  camera/hardware와 장시간 운전은 이번 slice에서 검증하지 않았습니다.

## Completed scheduled 2D slice — PL-0091 / 2D-033 (Line·Length·Mean 퇴화 입력과 유한 결과)

기존 `LineGauge`, `LineDistance`, `MeanTool`, metric-enrichment와 acceptance owner를
재사용해 정상 LineDistance/Mean, 양·음 극성, 균일 입력, 빈 입력/ROI, 미검출선,
동일선·길이 0 경계를 실행했습니다. 정상 계측은 유한 pixel/mm 또는 GV metric과
overlay를 유지하고, 균일 dark Mean은 acceptance band 밖에서 NG가 되며, 퇴화
LineDistance는 positive·finite 거리만 남긴 뒤 명시적인 `LineGaugeEdgeNotFound`
사유로 종료합니다. 빈 목록 `Min()`/`Average()` 예외가 정상 결과로 노출되지 않도록
기존 `VisionPipelineLineDistanceTool`에 최소 guard만 추가했으며 새 계측 모듈/보정
알고리즘은 추가하지 않았습니다.

- owner/call path: `VisionPipelineAppToolFactory` → existing LineGauge/
  `VisionPipelineLineDistanceTool`/`MeanTool` → `VisionToolResult` metrics/overlays
  → `VisionPipelineMetricEnrichmentService`/acceptance → `VisionRecipeRunner` summary
- 상세 보고서:
  [`OPENVISIONLAB_2D_LINE_LENGTH_MEAN_DEGENERATE_20260916.md`](../reports/OPENVISIONLAB_2D_LINE_LENGTH_MEAN_DEGENERATE_20260916.md)
- 원장: `.proofline/issues/PL-0091.json` (`resolved`)
- D: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d033-debug-20260916-run4` 및
  `2d033-release-20260916-run1`, focused Debug/Release roots
- 검증: Debug/Release build 0 errors; 새 Line/Length/Mean contract와
  expected-failure, Preview/Run/Reopen, PixelPerMm 회귀 모두 exit 0.
- 경계: 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input interaction,
  camera/hardware와 장시간 운전은 이번 slice에서 검증하지 않았습니다. 기존
  LineIntersection no-cross 표시 상태는 변경하지 않았습니다.

## Completed scheduled 2D slice — PL-0092 / 2D-034 (지원 영상 크기별 메모리·시간·복사량 기준선)

기존 `VisionRecipeRunner`, Mean Tool, `VisionRecipeRunResult`,
`BitmapImageConverter` 경계를 바꾸지 않고, 1MP·4512²·8192² 입력의 단일
Layer와 2단계 Layer pipeline을 각각 warm-up 5회·본 측정 30회 실행했습니다.
입력 Bitmap→Mat 변환, Runner 계산, 결과/context clone, bitmap 게시를 분리하고
P50/P95 시간, private bytes peak/종료 후, managed GC, result identity와
estimated layer/cache bytes를 CSV로 남겼습니다. 8192² 2-layer의 최대 관측
private peak는 Debug 약 601MiB, Release 약 602MiB였으며, 이는 정책이나 cap을
정한 값이 아니라 다음 검증의 기준선입니다. 20000²는 사전 계산만 하고 할당하지
않았습니다.

- owner/call path: `VisionRecipeRunnerSmoke` → `VisionRecipeRunner` → existing
  Mean Tool → `VisionRecipeRunResult` → `BitmapImageConverter`
- 상세 보고서:
  [`OPENVISIONLAB_2D_VIDEO_SIZE_MEMORY_BASELINE_20260916.md`](../reports/OPENVISIONLAB_2D_VIDEO_SIZE_MEMORY_BASELINE_20260916.md)
- 원장: `.proofline/issues/PL-0092.json` (`resolved`)
- D: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d034-debug-20260916-run1` 및
  `2d034-release-20260916-run1`
- 검증: Debug/Release smoke build 0 errors; 각 기본 크기/Layer 형태에서
  5 warm-up + 30 measured run 모두 PASS
- 경계: 실제 WPF EXE 렌더링·theme/layout/DPI/monitor/input interaction,
  cancellation·low-memory, camera/hardware와 장시간 운전은 검증하지 않았습니다.
  이 slice에서 production owner·diagnostic policy·output cap·cache eviction은
  변경하지 않았습니다.

## Completed safe boundary — PL-0093 / 2D-036 (출력 할당 preflight)

`VisionPipelineOutputAllocationGuard`가 기존 Affine `32768` per-dimension
계약, RotateScale finite-positive scale, native integer dimension 범위와
checked `width × height × element-bytes` 계산을 하나의 owner로 소유합니다.
Recipe 실행은 SDK 호출 전에 이 guard를 통과하고, direct Affine/RotateScale
Preview도 같은 guard를 재사용합니다. 실패한 Step은 native allocation과
`VisionPipelineContext.SetLayer`를 건너뛰며 이전 성공 결과를 보존합니다.

- owner/call path: `VisionRecipeRunner` → `VisionPipelineExecutionService` →
  `VisionPipelineOutputAllocationGuard` → `VisionPipelineAppToolFactory`/SDK;
  direct Preview adapters도 guard → SDK 경계를 사용합니다.
- 상세 보고서:
  [`OPENVISIONLAB_2D_OUTPUT_ALLOCATION_PREFLIGHT_20260916.md`](../reports/OPENVISIONLAB_2D_OUTPUT_ALLOCATION_PREFLIGHT_20260916.md)
- 원장: `.proofline/issues/PL-0093.json` (`blocked`)
- D: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d036-debug-20260916-final2` 및
  `2d036-release-20260916-final2`; focused WPF smoke는 `2d036-wpf-debug-20260916-run2`와
  `2d036-wpf-release-20260916-run1`
- 검증: OpenVisionLab solution Debug/Release build 0 errors; smoke Debug/Release
  build 0 errors; output preflight contract Debug/Release exit 0; focused
  `wpf_shell_host_rotate_scale_tool,wpf_shell_host_affine_transform_tool` smoke
  Debug/Release both `OK` with fresh screenshots. The first Debug attempt exposed
  a cross-thread Preview exception; the two direct Preview adapters now marshal
  View property snapshots and Affine result-review updates through the View
  dispatcher.
- 경계: 2D-034 수치만으로 제품 RAM cap/예산을 추정하지 않았습니다. actual
  desktop EXE rendering, alternate theme/layout/DPI/monitor/input matrices,
  low-memory, cancellation, camera/hardware와 장시간 운전은 여전히 검증하지
  않았습니다. 현재 환경의 두 변경 Preview owner만 in-process WPF smoke로
  검증했습니다.

## Blocked decision boundary — PL-0093 / 2D-036 memory budget

전체 2D-036의 `메모리 예산 초과` 동작은 제품 정책 결정이 필요합니다.
현재 근거에는 사용자용 RAM cap, 설정 scope, 오류 문구와 low-memory recovery
계약이 없습니다. 다음 실행은 임의의 available-memory heuristic, 자동
downscale, cache eviction, parallelism을 추가하지 않고 이 결정만 대기합니다.

## Current blocked boundary — PL-0061

2D-003 메타데이터 reader 구현은 완료됐지만, 전체 RC distribution 및 변조 fixture 검증은
현재 Dev worktree의 기존 dirty tracked 변경 때문에 보류되어 있습니다. clean verification
checkout 승인 전에는 같은 RC 시도를 반복하지 않습니다.

## Current evidence-based maturity

- 선행개발 전 local source/build/contract/대표 actual EXE 기준선은 `Complete`입니다.
- 다음 독립 2D 기능 개발을 막는 source/build/document blocker는 없습니다. 다만
  2D-023 일반 파일 입력 상한은 제품 정책 결정 전까지 별도 blocker입니다.
- 이는 field qualification, release readiness 또는 모든 UI 상태 완료를 뜻하지 않습니다.
- OpenVisionLab Vision SDK와 OpenCvSharp native binary는 x64 계약입니다.
- `OpenVisionLab.ImageCanvas.dll`은 단독 배포 계약이 아닙니다. 선언된 managed/native
  dependency closure와 Windows Desktop runtime을 함께 검증해야 합니다.
- WPF는 실용적 MVVM 구조입니다. `RoiImageCanvasViewModel`의 concrete control
  생성·노출·호출·Dispose는 제거됐고, View/presentation이 native lifetime을 소유합니다.
- ViewModel UI/IO audit signal 1개는 ImageCanvas의 WPF keyboard/directory-policy 연결이며,
  순수 domain 모듈로 과장하지 않습니다.
- 실제 카메라/장비, 장시간 운전, 전체 theme/DPI/input 행은 소스나 단일 smoke 성공으로
  완료했다고 주장하지 않습니다.

## Commercial lessons to retain

- 설정 → 명시적 Preview/Run → 결과/근거 → N-sample validation의 짧은 흐름
- operator가 input/output layer, 단위, 결과 상태, 실패 이유를 즉시 구분하는 화면
- 설정 복원만으로 실행하지 않는 안전 계약과 결과 provenance
- 실제 evidence가 없는 범용 자동화나 장비 플랫폼 확장은 하지 않음

## Out of scope until separately requested

- camera/light/PLC/I/O 통합 플랫폼
- cross-platform/Avalonia/Linux와 단일-DLL 배포
- LLM candidate activation 또는 새 benchmark
- Original 반영, commit/push/tag/release/deploy

## Next action

PL-0065·PL-0066·PL-0067·PL-0068·PL-0069·PL-0070·PL-0071·PL-0072·PL-0073·PL-0074·PL-0075·PL-0076·PL-0077·PL-0078·PL-0079·PL-0080·PL-0082·PL-0083·PL-0084·PL-0085·PL-0086·PL-0087·PL-0088·PL-0089·PL-0090·PL-0091·PL-0092는 완료되었습니다. PL-0093은 제품 memory budget/cap 결정 전까지
blocked로 유지합니다. PL-0061은 clean verification checkout 선행 조건이 충족될 때까지
blocked로 유지하고, PL-0081(2D-023)은 제품 입력 크기 정책 결정 전까지 blocked로 유지합니다. PL-0057·PL-0058·PL-0059·PL-0060·PL-0062·PL-0063·PL-0064·PL-0065·PL-0066·PL-0067·PL-0068·PL-0069·PL-0070·PL-0071의
 완료 owner는 새 요구, 재현 defect, 실패 criterion 또는 dependency 변경 없이 다시 분할하지 않습니다.
완료 owner PL-0069·PL-0070·PL-0071·PL-0072·PL-0073·PL-0074·PL-0075·PL-0076·PL-0077·PL-0078·PL-0079·PL-0080·PL-0087·PL-0088도 동일한 경계를 유지합니다. 2D-023은
제품 결정 전까지 재구현하지 않으며, 완료된 2D-024 owner도 새 요구·재현
defect·실패 criterion·dependency 변경 없이는 다시 분할하지 않습니다. 다음
작업은 PL-0093의 memory budget/cap 결정을 받은 뒤 해당 정책 boundary를
명시하고 검증하는 것입니다.
