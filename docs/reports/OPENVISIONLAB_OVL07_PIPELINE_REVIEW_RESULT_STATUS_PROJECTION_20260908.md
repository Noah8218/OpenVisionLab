# OpenVisionLab OVL-07 Pipeline Review result/status projection owner 분리 — 2026-09-08

## 의미와 범위

이번 실행은 Pipeline Review의 실행 상태와 전체 진행 문구를
`OpenVisionPipelineReviewResultStatusProjectionOwner`로 분리했다. owner는
실행 상태 종류, Step 결과 수, 오류 문구와 Pipeline/결과 snapshot을 받아
기존 한국어/영어 상태, 결과 summary/detail, OK/NG/WAIT/OFF 진행 문구를
계산한다.

선택 Step의 run log와 결과 summary/detail은 기존
`OpenVisionPipelineReviewResultPresenter`가 계속 소유한다. 실행 생성,
취소, generation, stale-result 억제, validation, recipe/input revision,
dispatcher, View mutation은 `OpenVisionPipelineReviewDocument`와 기존
`OpenVisionPipelineReviewExecutionController`에 남겼다. Recipe/XML, Layer
routing과 명시적 Preview/Run 계약은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 실행 상태 문자열 | `OpenVisionPipelineReviewDocument` inline localization | `OpenVisionPipelineReviewResultStatusProjectionOwner.Project` |
| 전체 진행 문구 | `FormatReviewProgressText` | `ProjectProgress` + Document bridge |
| 실행 시작 progress | Document inline `Running...` | `ProjectRunningProgressText` |
| 선택 Step result/log | 기존 `OpenVisionPipelineReviewResultPresenter` | 변경 없음 |
| 실행/취소/stale 적용 | 기존 execution controller와 Document | 변경 없음 |

호출 경로는 다음과 같다.

Document lifecycle/RunReviewAsync → ResultStatusProjectionOwner →
Document의 `ApplyReviewResultStatus`/`ProjectReviewProgressText` → 기존 View와
ViewModel facade

owner는 Window, UserControl, FrameworkElement, Dispatcher, Document 또는
execution controller를 참조하지 않는 internal sealed concrete type이다.

## 보존한 observable contract

- `NotRun`, `Draining`, `AlreadyRunning`, `NoSteps`, `ValidationErrors`,
  `Started`, `Superseded`, `Failed`, `Completed`, `ReferenceChanged`,
  `RunRequired` 상태의 localized text와 fallback을 유지했다.
- 실행 실패의 exception message, 완료 Step 결과 수, validation/start
  summary/detail과 reference 변경 후 재실행 안내를 유지했다.
- 빈 Pipeline, 미실행, OK/NG/WAIT/OFF count, 실행 중 및 종료 대기 prefix를
  기존 규칙으로 계산한다.
- `SelectStep`의 선택 결과 summary/detail과 run log는 기존 presenter 호출을
  그대로 사용한다.
- Reset/Close와 늦은 결과 억제는 기존 revision/controller 경로에 남아 있고,
  이 owner는 상태 문자열만 계산한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewResultStatusProjectionOwner.cs`
  - run-level status/progress projection owner와 immutable-like projection을
    추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
  - inline status/progress 계산을 owner 호출로 교체하고 View 적용 bridge를
    남겼다.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.Events.cs`
  - localized refresh 경로가 owner progress projection을 사용하도록 했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--pipeline-review-result-status-projection-contract` 무창 계약과 usage를
    추가했다.
- `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_RESULT_STATUS_PROJECTION_20260908.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner 선언은 `internal enum`과 `internal sealed class`이며 `partial`이
  아니다.
- owner에 WPF/View/Document/Controller 참조가 없고, mutable workflow state를
  보관하지 않는다.
- Document와 Events partial에는 `FormatReviewProgressText`와
  `PipelineReview.Execution.*`/`PipelineReview.Progress.*` inline 호출이
  남아 있지 않다.
- Document는 모든 lifecycle/run result 경로에서 owner를 호출하고,
  `SelectStep`에는 기존 `FormatRunLog`, `FormatResultSummary`,
  `FormatResultDetails` 호출이 남아 있다.
- 정적 결과는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-result-status-projection-20260908\static-ownership-proof.log`에
  기록했다.

## 검증

- `OpenVisionLab` 앱 Debug/Release build가 경고 0개·오류 0개로 통과했다.
- `VisionRecipeRunnerSmoke` Debug/Release build가 경고 0개·오류 0개로
  통과했다.
- 새 `--pipeline-review-result-status-projection-contract`가 Debug/Release
  모두 PASS했다. 모든 실행 상태와 빈/미실행/count/running/draining 진행
  분기를 직접 확인했다.
- 기존 `--pipeline-review-execution-contract`가 Debug/Release 모두
  PASS했다.
- 기존 Recipe Manager summary/Pipeline option, Pipeline lifecycle/exchange,
  review-bundle dry-run projection 계약이 Debug/Release 모두 PASS했다.
- `OpenVisionLab.sln` Debug/Any CPU build와 `OpenVisionReadinessCheck`
  Debug가 경고 0개·오류 0개 및 PASS였다.
- `PipelineViewerScreenshotSmoke` Debug/Release build는 오류 0개로
  통과했다. 기존 `Program.cs:10722` CS8600 경고 1개가 두 구성에서
  그대로 발생했다.
- 정상 Pipeline Review와 acceptance NG Pipeline Review smoke가 Debug/Release
  모두 `check=OK`, `layout=0|text=0|internal=0`, `1600x900`으로 통과했다.
  대표 캡처는 다음과 같다.
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-result-status-projection-20260908\ui-final-release\wpf_shell_host_pipeline_review.png`
  - `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-result-status-projection-20260908\ui-final-release\wpf_shell_host_pipeline_review_ng.png`
- Windows가 보고한 모니터는 단일 `\\.\DISPLAY2`, bounds
  `0,0,1920,1080`, working area `0,0,1920,1032`였고 smoke 뒤 target
  process가 남지 않았다. topology와 cleanup 결과는 evidence root에 있다.
- Debug/Release 빌드, 계약, smoke, readiness, static proof 로그는
  다음 evidence root에 보관했다.
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-result-status-projection-20260908`
- 기존 dirty worktree와 `C:\Git\2D\Original`은 보존했고 commit/push/merge/tag/
  release/deployment는 수행하지 않았다.

## 범위 경계

이번 완료는 Pipeline Review run-level result/status와 progress projection
owner 분리에 한정한다. 선택 Step 결과 presenter, validation/Step Edit,
execution controller의 lifetime/stale-result 정책, 이미지·Layer lifetime,
전체 theme/layout/DPI(100/125/150/175/200%)와 다중 모니터 matrix는 별도
검증 범위다. 이번 UI evidence는 단일 모니터의 Debug/Release 대표 정상/NG
화면만 증명한다.

## Refactor proof

- Current owner: `OpenVisionPipelineReviewDocument`의 lifecycle/run 경로와
  기존 `FormatReviewProgressText`.
- Intended owner: `OpenVisionPipelineReviewResultStatusProjectionOwner`.
- Dependency direction: Document → projection owner; owner → plain Pipeline /
  result snapshot and localization service only.
- State owner: execution generation/cancellation/cache는
  `OpenVisionPipelineReviewExecutionController`, selected pipeline/revision/
  dispatcher/View mutation은 Document, selected Step detail/log는 기존
  result presenter가 소유한다.
- Observable contract: existing localized statuses, result text, progress
  counts, Recipe/XML/Layer routing, explicit Preview/Run and XAML bindings.

Status: Complete
Scope: OVL-07 Pipeline Review result/status projection owner 분리.
Acceptance criteria: run-level inline projection 제거, Window-free concrete
owner 연결, selected-Step presenter와 execution/lifetime/Recipe contracts
보존, Debug/Release focused contract와 representative UI smoke 통과.
Verification: app/runner/ScreenshotSmoke builds, focused and existing Review /
Recipe contracts, solution/readiness, Debug/Release normal+NG smoke, static
ownership proof, evidence capture and cleanup.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-result-status-projection-20260908`.
Boundary / next dependency: validation/Step Edit, image/layer lifetime와
full UI matrix는 이 slice에서 증명하지 않았다. 다음 단일 작업은 OVL-07
Pipeline Review guide/result-detail projection owner 분리다.
