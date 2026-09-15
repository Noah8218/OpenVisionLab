# OpenVisionLab OVL-07 Pipeline Review guide/result-detail projection owner 분리 — 2026-09-08

## 의미와 범위

이번 실행은 Pipeline Review에서 선택 Step의 guide, 결과 summary/detail,
run-log, pair action/metric 문구를 조합하는 책임을
`OpenVisionPipelineReviewGuideResultProjectionOwner`로 분리했다. 기존
`OpenVisionPipelineReviewGuidePresenter`와
`OpenVisionPipelineReviewResultPresenter`의 실제 formatter를 owner가
조합하므로 출력 정책을 복제하지 않는다.

`OpenVisionPipelineReviewDocument`는 선택 index/mode, Pipeline과 validation,
실행 controller, Bitmap 입력 수명, dispatcher, View mutation을 계속 소유한다.
owner는 Window, UserControl, FrameworkElement, Dispatcher, Document 또는
execution controller를 참조하지 않으며 Bitmap을 보관하거나 dispose하지 않는다.
검증 오류/실행 중 guide도 같은 owner를 통해 생성한다. Recipe/XML, Layer routing,
명시적 Preview/Run, 실행 취소와 stale-result 억제는 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 선택 Step guide 조합 | `OpenVisionPipelineReviewDocument.SelectStep`의 직접 presenter 호출 | `OpenVisionPipelineReviewGuideResultProjectionOwner.ProjectSelected` |
| 결과 summary/detail, run-log | Document가 ResultPresenter를 직접 호출 | owner가 기존 ResultPresenter를 위임 호출 |
| pair action/metric | Document가 직접 조합 | owner projection의 `PairActionText`, `CanOpenPairAction`, `PairMetricText` |
| validation/running guide | `RunReviewAsync`의 직접 GuidePresenter 호출 | owner의 `ProjectValidationErrorGuide`/`ProjectRunningGuide` |
| View와 실행 상태 | Document | 변경 없음 |

호출 경로는 다음과 같다.

`Document.SelectStep` → `GuideResultProjectionOwner.ProjectSelected` → 기존
Guide/Result presenter → `SetSelectedStep`/`SetResultSummary`/`SetReviewGuide`
및 pair View 적용

## 보존한 observable contract

- 결과 없음, OK, 도구 NG, acceptance NG의 summary/detail과 guide decision/detail을
  기존 localized text 및 fallback으로 유지했다.
- 기존 run-log의 review state, validation, result, preview mode, input/output
  image 상태를 그대로 유지했다.
- Good/Bad pair action의 역할 문구와 open 가능 여부, pair metric 비교 문구를
  기존 규칙으로 유지했다.
- validation error와 running guide의 next action을 기존 GuidePresenter 정책으로
  유지했다.
- selected Step의 object/instance/geometry/circle/matcher View 적용 순서,
  fixture/designer/scale 갱신 순서와 실행/lifetime 경계는 유지했다.
- Recipe/XML 및 Layer 입력/출력 계약과 explicit Preview/Run 계약은 변경하지 않았다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewGuideResultProjectionOwner.cs`
  - Window-free concrete owner, request snapshot, projection 결과를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
  - selected Step와 validation/running guide의 직접 formatter 호출을 owner 호출로
    교체하고 View 적용 bridge를 유지했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--pipeline-review-guide-result-projection-contract` 무창 계약과 usage를
    추가했다.
- `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_GUIDE_RESULT_PROJECTION_20260908.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
  `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner는 `internal sealed class` concrete type이며 `partial`이 아니다.
- owner에는 WPF/View/Document/Controller/Dispatcher 참조가 없고, mutable workflow
  state나 image lifetime owner가 없다.
- Document에는 Guide/Result presenter의 대상 formatter 직접 호출이 남아 있지 않다.
- Document는 owner projection을 받은 뒤 기존 `SetSelectedStep`,
  `SetResultSummary`, `SetReviewGuide`, pair View mutation을 계속 수행한다.
- 정적 증거는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-guide-result-projection-20260908\static-guide-result-ownership-proof.log`에 기록했다.

## 검증

- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Debug --no-restore`:
  경고 0, 오류 0.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Release --no-restore`:
  경고 0, 오류 0.
- `VisionRecipeRunnerSmoke` Debug/Release build: 경고 0, 오류 0.
- 새 `--pipeline-review-guide-result-projection-contract` Debug/Release: PASS.
  결과 없음, OK, tool-NG, acceptance-NG, validation-error, running, pair action,
  pair metric, run-log projection을 직접 확인했다.
- 기존 `--pipeline-review-result-status-projection-contract` Debug/Release와
  `--pipeline-review-execution-contract` Debug/Release: PASS.
- 기존 Recipe pipeline exchange, Recipe Manager summary, workspace lifecycle,
  review-bundle dry-run projection contract: Debug PASS.
- `OpenVisionLab.sln` Debug/Any CPU build와 `OpenVisionReadinessCheck` Debug: PASS,
  경고 0, 오류 0.
- `PipelineViewerScreenshotSmoke` Debug/Release build: 오류 0. 기존
  `tools/PipelineViewerScreenshotSmoke/Program.cs:10722` CS8600 경고 1개가 각
  구성에서 그대로 발생했다.
- `wpf_shell_host_pipeline_review` 및 `_ng` 화면 smoke를 Debug/Release에서
  실행했다. 모두 `check=OK`, `layout=0|text=0|internal=0`, `1600x900`으로
  통과했다. 대표 캡처는 evidence root의 `ui-final-debug-*`와
  `ui-final-release-*` 아래에 있다.
- Windows monitor probe에서 실제 창 `26,26,1600x900`이 단일
  `\\.\DISPLAY2` bounds `0,0,1920,1080`과 교차했다. probe 결과는
  `monitor-window-proof.log`에 있다. Debug/Release UI smoke 후 target EXE
  process는 남지 않았다.
- 전체 evidence root:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-guide-result-projection-20260908`

## 범위 경계

이번 완료는 guide/result-detail projection owner 분리에 한정한다. 선택 Step
domain evidence(object/instance/geometry/circle/matcher), validation/Step Edit,
image/Layer lifetime, 전체 theme/layout/DPI(100/125/150/175/200%)와 다중 모니터,
hover/pressed/focus/resize/keyboard 상호작용 matrix는 이 실행에서 완전히 증명하지
않았다. before baseline 캡처도 별도로 확보하지 않았다.

## Refactor proof

- Current owner: `OpenVisionPipelineReviewDocument.SelectStep`와
  `RunReviewAsync`의 Guide/Result formatter 조합 호출.
- Intended owner: `OpenVisionPipelineReviewGuideResultProjectionOwner`.
- Dependency direction: Document → projection owner; owner → 기존 Guide/Result
  presenter와 plain Pipeline/result/sample snapshot.
- State owner: 선택 상태, View/dispatcher, image lifetime, validation과 실행
  controller는 Document/기존 controller가 소유하고, 순수 text projection만 owner가
  소유한다.
- Observable contract: 기존 localized guide/result/log/pair text, Recipe/XML/Layer
  routing, explicit Preview/Run, XAML binding 이름.

Status: Complete
Scope: OVL-07 Pipeline Review guide/result-detail projection owner 분리.
Acceptance criteria: 직접 formatter 조합 제거, Window-free concrete owner 연결,
기존 presenter/실행/lifetime/Recipe 계약 보존, Debug/Release focused contract와
대표 UI smoke 통과.
Verification: app/runner/ScreenshotSmoke builds, focused and existing Review /
Recipe contracts, solution/readiness, Debug/Release normal+NG smoke, monitor probe,
static ownership proof, evidence capture and cleanup.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-guide-result-projection-20260908`.
Boundary / next dependency: domain evidence projection과 full UI matrix는 이
slice에서 증명하지 않았다. 다음 단일 작업은 OVL-07 Pipeline Review selected-Step
domain evidence projection owner 분리다.
