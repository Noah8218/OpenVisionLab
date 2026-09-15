# OpenVisionLab OVL-07 Pipeline Review selected-Step domain evidence projection owner 분리 — 2026-09-08

## 의미와 범위

이번 실행은 Pipeline Review에서 선택 Step의 object, instance, geometry,
circle, matcher evidence family 지원 정책과 같은 실행 summary의 evidence
data projection을 `OpenVisionPipelineReviewDomainEvidenceProjectionOwner`로
분리했다. 기존 family 판정 규칙과 summary collection 참조를 그대로 보존하고,
View가 이미 소유한 grid/render/image clone·dispose 동작은 이동하지 않았다.

`OpenVisionPipelineReviewDocument`는 선택 index/mode, Pipeline과 validation,
실행 controller, dispatcher, View mutation, input/layer lifetime을 계속 소유한다.
새 owner는 Window, UserControl, View, Document, controller, Dispatcher, Bitmap을
참조하지 않고 mutable workflow state도 보관하지 않는다. Recipe/XML, Layer
routing, 명시적 Preview/Run, 실행 중 Reset/Close와 stale-result 방지는 변경하지
않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| evidence family 지원 판정 | `OpenVisionPipelineReviewDocument`의 다섯 `Is*Tool` helper | `OpenVisionPipelineReviewDomainEvidenceProjectionOwner`의 다섯 `Supports*` 정책 |
| selected summary data 전달 | Document가 `summary?.*`를 View setter에 직접 전달 | owner가 `OpenVisionPipelineReviewDomainEvidenceProjection`으로 한 번 투영한 뒤 전달 |
| View 적용과 image lifetime | `OpenVisionPipelineReviewView` | 변경 없음 |
| lower presenter/renderer | 기존 presenter 및 View helper | 변경 없음 |

호출 경로는 다음과 같다.

`Document.SelectStep` → `DomainEvidenceProjectionOwner.Project` →
`View.SetObjectResults` / `SetInstanceResults` / `SetGeometryResults` /
`SetCircleEvidence` / `SetMatcherDiagnostics`

## 보존한 observable contract

- Blob/Contour, MultiMatchMean/MultiFixtureMean, Line/LineGauge/CircleGauge,
  GeometryMeasure/GeometricMeasurement, CircleGauge, EdgeBased matching의
  기존 family 인식과 Tool suffix/공백/underscore 정규화를 유지했다.
- 기존 `VisionPipelineStepResultSummary`의 object/instance/geometry/circle/
  matcher/metric collection을 복사하지 않고 동일 참조로 전달한다.
- `SelectStep`의 selected-step, summary, object, instance, geometry, circle,
  matcher, fixture/designer/scale View setter 순서를 유지했다.
- View의 source/output image clone, residual evidence 생성, matcher presenter
  호출과 dispose 수명 경계를 변경하지 않았다.
- Recipe/XML 교환, 입력/출력 Layer routing, explicit Preview/Run 및 validation/
  execution 상태 계약은 변경하지 않았다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewDomainEvidenceProjectionOwner.cs`
  - Window-free concrete owner와 immutable-like projection result를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
  - 다섯 직접 family helper를 제거하고 새 owner projection을 기존 View setter에
    연결했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--pipeline-review-domain-evidence-projection-contract` 무창 계약과 usage를
    추가했다.
- `tools/OpenVisionReadinessCheck/Program.cs`
  - matcher same-run evidence 검사를 새 domain owner 경계에 맞춰 확인하도록
    갱신했다.
- `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_REVIEW_DOMAIN_EVIDENCE_PROJECTION_20260908.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner는 `internal sealed class` concrete type이며 `partial`이 아니다.
- owner에는 WPF/View/Document/controller/Dispatcher/Bitmap 참조가 없고,
  instance field나 image lifetime 관리가 없다.
- Document의 기존 다섯 `Is*Tool` helper는 제거되었고
  `domainEvidenceProjectionOwner.Project` 호출만 남았다.
- Document는 owner projection을 받은 뒤 기존 View setter를 동일한 순서와
  동일한 input/output image 인자로 호출한다.
- 정적 증거는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-domain-evidence-projection-20260908\static-domain-evidence-ownership-proof.log`에 기록했다.

## 검증

- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Debug --no-restore`:
  경고 0, 오류 0.
- `dotnet build src/OpenVisionLab/OpenVisionLab.csproj --configuration Release --no-restore`:
  순차 재실행에서 경고 0, 오류 0. 같은 시각 병렬 실행의 첫 앱 빌드는 WPF
  generated `.g.cs` 공유 경합으로 `CS2001` 25건이 발생했으며,
  `app-release-build-concurrent-race.log`에 원본 실패를 보존했다. 코드 수정 후
  순차 빌드는 통과했다.
- `VisionRecipeRunnerSmoke` Debug/Release 순차 build: 경고 0, 오류 0.
- 새 `--pipeline-review-domain-evidence-projection-contract` Debug/Release:
  PASS. Blob/Contour, MultiFixtureMean, Geometry_Measure, CircleGauge,
  EdgeTemplateMatching, unsupported tool과 evidence data identity를 확인했다.
- 기존 Pipeline Review guide/result, result/status, execution 및 Recipe
  pipeline exchange/review-bundle dry-run projection contracts Release: PASS.
- `OpenVisionLab.sln` Debug/Any CPU build와 `OpenVisionReadinessCheck` Debug:
  PASS, 경고 0, 오류 0.
- `PipelineViewerScreenshotSmoke` Debug/Release build: 오류 0. 기존
  `tools/PipelineViewerScreenshotSmoke/Program.cs:10722` CS8600 경고 1개가
  각 구성에서 그대로 발생했다.
- `wpf_shell_host_pipeline_review` 및 `_ng` UI smoke를 Debug/Release에서
  실행했다. 네 실행 모두 `check=OK`, `layout=0|text=0|internal=0`,
  `1600x900`으로 통과했다. 캡처는 evidence root의 `ui-debug-*`와
  `ui-release-*` 아래에 있다. 대표 정상/NG 이미지를 직접 확인했다.
- 동적 monitor probe는 단일 `\\.\DISPLAY2`, bounds `0,0,1920x1080`,
  working area `0,0,1920x1032`를 기록했다. 실제 창 교차는 smoke runner의
  monitor-aware placement 경로로 확인했고 target process는 종료되었다.
- 전체 evidence root:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-domain-evidence-projection-20260908`

## 범위 경계

이번 완료는 selected-Step domain evidence policy/data projection owner 분리에
한정한다. image/Layer lease/dispose owner, validation/Step Edit, Native Tool
event/queue, 전체 theme/layout/DPI(100/125/150/175/200%)와 다중 모니터,
hover/pressed/focus/resize/keyboard interaction matrix는 이 실행에서 완전히
증명하지 않았다. 별도 before baseline 캡처도 확보하지 않았다.

## Refactor proof

- Current owner: `OpenVisionPipelineReviewDocument.SelectStep` 내부의 다섯
  evidence family 판정과 summary data 직접 전달.
- Intended owner: `OpenVisionPipelineReviewDomainEvidenceProjectionOwner`.
- Dependency direction: Document → stateless domain projection owner → plain
  Pipeline/result snapshot; Document → existing View setter.
- State owner: 선택 상태, View/dispatcher, input/output image lifetime,
  validation과 execution controller는 기존 Document/View/controller가 소유한다.
  새 owner는 family policy와 projection result만 소유한다.
- Observable contract: 기존 evidence family 판정, summary collection identity,
  View setter order, Recipe/XML/Layer routing, explicit Preview/Run, XAML binding.

Status: Complete
Scope: OVL-07 Pipeline Review selected-Step domain evidence projection owner 분리.
Acceptance criteria: 다섯 family 정책과 summary projection을 Window-free concrete
owner로 이동, 기존 View/lifetime/Recipe 계약 보존, focused Debug/Release contract,
build/readiness와 대표 Debug/Release normal+NG UI smoke 통과.
Verification: app/runner/ScreenshotSmoke builds, focused and existing Review /
Recipe contracts, solution/readiness, dynamic monitor-aware UI smoke, static
ownership proof, evidence capture.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-review-domain-evidence-projection-20260908`.
Boundary / next dependency: image/Layer lifetime과 full UI matrix는 이 slice에서
증명하지 않았다. 다음 단일 작업은 OVL-07 Pipeline Review image/Layer lifetime
owner boundary다.
