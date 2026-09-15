# OpenVisionLab 문서 시작점

Updated: 2026-09-14 KST

이 파일은 사람과 LLM이 프로젝트 문서를 찾을 때 사용하는 단일 진입점입니다. 문서를 처음부터 전부 읽지 말고, 아래 최소 세트와 작업별 경로만 읽으세요.

기계 판독용 색인은 `docs/LLM_DOCUMENT_INDEX.json`, 전체 상세 등록부는 `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`입니다.

## 30초 시작 순서

1. `AGENTS.md` — 저장소 규칙, 제품 경계, 변경/검증 계약
2. `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` — 현재 상태, 최신 완료 근거, 실제 다음 우선순위만 담은 짧은 핸드오프
3. `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` — 제품 정체성과 화면별 책임
4. `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md` — 회귀시키면 안 되는 동작

사용 방법이나 초보자 흐름을 찾는 경우에는 `docs/manual/README.md`를 먼저 읽습니다.
배포본의 상단 `Guide` 버튼도 이 원본에서 생성한 단일 HTML을 엽니다.

## 제품 코드와 검증 도구를 구분하는 60초 경로

처음 기능을 수정할 때는 파일 크기보다 실행 목적을 먼저 구분합니다.

| 목적 | 먼저 열 파일 | 여기서 확인할 것 |
| --- | --- | --- |
| 제품 시작·Shell | `src/OpenVisionLab/Program.cs` → `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs` → `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs` | 실제 애플리케이션 진입점과 화면 조합 |
| Pipeline 실행·결과 | `src/OpenVisionLab/Core/Pipeline/Execution/VisionPipelineExecutionService.cs` → `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs` | 제품 실행 정책, 결과와 이미지 수명 |
| 계약 검증(화면 없음) | `tools/VisionRecipeRunnerSmoke/Program.cs` 및 `*Contract.cs` | 고정 입력·출력과 실패 계약 |
| 실제 WPF 화면 검증 | `tools/PipelineViewerScreenshotSmoke/Program.cs` → `ScreenshotSmokeTargetRunner.cs` | target 선택, 창 수명, 캡처와 UI 상태 |
| 제품 EXE에 붙는 Smoke 경로 | `tools/OpenVisionLab.DirectSmokeRunner/OpenVisionLabDirectSmokeRunner.cs` | `OpenVisionLabEnableEmbeddedSmokeRunner=true`에서만 조건부 포함되는 시나리오 조합 |

검증 도구의 `Program.cs`나 Direct runner가 길어도 제품 업무 규칙의 소유자라는 뜻은 아닙니다. 제품 동작을 바꿀 때는 먼저 `src/`의 concrete owner와 호출자를 수정하고, 계약·화면 검증이 그 동작을 확인하도록 유지합니다. 검증 실패를 통과시키기 위해 smoke assertion이나 fixture를 완화하지 않습니다. 독립 계약의 원시 결과와 화면·실행 산출물은 `D:\OpenVisionLab-TestData\OpenVisionLab_Dev`에 기록합니다.

가장 짧은 확인 순서는 `제품 owner → 해당 contract → 필요한 경우 WPF smoke → D: 증거`입니다. 전체 smoke 도구의 구조를 읽어야 하는 경우에는 `docs/admin/CODEBASE_STRUCTURE.md`의 `Smoke ... 책임을 읽는 순서` 항목을 사용합니다.
그 다음 `docs/LLM_DOCUMENT_INDEX.json`의 `routes`에서 현재 작업과 일치하는 항목만 추가로 읽습니다. 수백 KB의 `OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`는 특정 P 번호나 과거 결정의 상세 근거가 필요할 때만 검색합니다.

## 문서 권위 순서

충돌할 때는 아래 순서를 따르고, 충돌을 숨기지 않습니다.

| 순위 | 문서 | 용도 |
| --- | --- | --- |
| 1 | `AGENTS.md` | 작업 규칙, 저장소/제품 경계, 검증 의무 |
| 2 | `docs/contracts/**` | 안정 동작, XML, 배포, 외부 참조 계약 |
| 3 | `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md` | 제품 정체성과 책임 소유권 |
| 4 | `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` | 최신 상태, 증거, 다음 우선순위 |
| 5 | `docs/reports/**` | 특정 작업의 완료·실패·한계 증거 |
| 6 | `docs/admin/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md` 및 과거 평가 | 상세 연대기와 역사적 문맥 |

오래된 완성도 백분율, 우선순위, 상용 비교 결론은 현재 사실로 재사용하지 않습니다. 현재 핸드오프와 최신 코드·테스트·스크린샷으로 다시 확인합니다.

## 작업별 빠른 경로

최신 구조·신뢰성 재검토는
[2026-09-09 보고서](../../reports/OPENVISIONLAB_ARCHITECTURE_RELIABILITY_REVIEW_20260909.md)에
있습니다. 생성·상태 변경·해제 책임을 따라가는 코드는 기존
`admin/CODEBASE_STRUCTURE.md`의 1.1에서 시작합니다.
현재 P0/P1/P2 실행 순서와 5분 heartbeat 운영 계약은
[2026-09-10 실행 계획](../../reports/OPENVISIONLAB_PRIORITIZED_REFACTOR_SCHEDULE_20260910.md)에
있습니다. 이 계획은 사용자 입력·하드웨어 검증을 기다리지 않고 독립적인
코드 slice를 진행하며, commit/push/release는 별도 승인 단계로 남깁니다.
Shell에서 Pipeline Review의 생성·도킹·Details·종료 owner를 찾는 최단 경로는
[Shell/Pipeline Review 탐색성 검토](../../reports/OPENVISIONLAB_JUNIOR_NAVIGATION_AND_SHELL_REVIEW_20260910.md)와
`admin/CODEBASE_STRUCTURE.md` 3.1에 기록합니다.
전체 소스·프로젝트·MVVM·partial·이미지 수명과 주니어 탐색성의 현재 판정은
[전체 코드베이스 주니어 검토](../../reports/OPENVISIONLAB_JUNIOR_WHOLE_REPOSITORY_REVIEW_20260911.md)에
기록합니다. 이 문서는 새 production boundary를 자동으로 열지 않으며,
실행하지 않은 WPF/장비/장시간 조건을 미검증으로 남깁니다.
모든 XAML View의 MVVM 경계, 단일 View 이동 가능성, View 가족별 의존 묶음과
Visual Studio 첫 코드 읽기 순서는
[WPF View MVVM·이동 가능성 감사](../../reports/OPENVISIONLAB_WPF_VIEW_MVVM_PORTABILITY_AUDIT_20260913.md)에
기록합니다. 이 감사는 완료된 owner를 파일 크기만으로 다시 분리하지 않고,
소스 기준과 미검증 Runtime UI 범위를 구분합니다.
현재 54개 `partial` 선언을 기계적으로 병합하지 않고 각 caller·상태 writer·Dispose
owner·binding/public/test 계약을 대조한 결과와 실제 ROI/template 이미지 decode
이동은 [Partial 책임 구조 검토](../../reports/OPENVISIONLAB_PARTIAL_RESPONSIBILITY_REVIEW_20260913.md)에
기록합니다.
현재 Partial 중 실제로 제거 가능한 수동 책임을 concrete owner로 옮기는
후속 계획은 [Partial 구조 제거 계획](../../reports/OPENVISIONLAB_PARTIAL_STRUCTURAL_ELIMINATION_PLAN_20260913.md)과
`PL-0028` 원장에서 진행합니다. generated/XAML/designer/native/test 계약은
기계적으로 삭제하지 않고 제거 불가 근거를 남깁니다.
Sample Picker의 이미지 미리보기 파일/BitmapImage 책임을 기존 image factory로
옮긴 후속 경계는 [Sample Picker 이미지 경계](../../reports/OPENVISIONLAB_SAMPLE_PICKER_IMAGE_BOUNDARY_20260913.md)와
`PL-0030` 원장에 기록합니다. `SelectedImageSource` binding과 null/frozen 계약은
유지하며, 새 2D 예약 자동화는 활성화하지 않습니다.
N-image 검증 화면의 중복 BitmapImage 파일 디코드도 같은 기존 factory로 이동한
후속 경계이며 [N-image 이미지 경계](../../reports/OPENVISIONLAB_TOOL_N_IMAGE_VERIFICATION_IMAGE_BOUNDARY_20260913.md)와
`PL-0031` 원장에 기록합니다. 선택·검증 상태와 `BitmapImage` binding 타입은 유지합니다.
Image Compare Window Partial의 표시 좌표 매핑 정책을 기존 ViewModel로 이동한
후속 경계는 [Image Compare 좌표 매핑 경계](../../reports/OPENVISIONLAB_IMAGE_COMPARE_POINT_MAPPING_BOUNDARY_20260914.md)와
`PL-0032` 원장에 기록합니다. required XAML Partial, status binding, image
resource lifetime은 유지하고 Window에는 WPF event adapter만 남겼습니다.
ROI Image Canvas의 파일명 추출·기본 PNG 저장 이름 정책을 기존
`ImageCanvasDirectoryPolicy`로 이동한 후속 경계는
[ROI Image Canvas path-policy 경계](../../reports/OPENVISIONLAB_ROI_IMAGE_CANVAS_PATH_POLICY_BOUNDARY_20260914.md)와
`PL-0033` 원장에 기록합니다. `RoiImageCanvasViewModel`의 공유 Mat/ROI/
OpenGL/input/timer 상태와 binding/public lifetime은 유지하며 새 Partial은
추가하지 않았습니다.
Learn Window Partial의 문서 열기 concrete service 호출은 기존 Shell/Tool/
Threshold composition owner가 연결하는 `Action<string>` 경계로 이동했습니다.
[Learn Window 문서 경계](../../reports/OPENVISIONLAB_LEARN_WINDOW_DOCUMENT_BOUNDARY_20260914.md)와
`PL-0034` 원장에 기록하며, XAML/topic/threshold/animation/lifetime 계약은
유지합니다.
Signal Inspector Partial의 TSV file-I/O 직접 호출은 기존 Tool composition이
연결하는 `Action<VisionToolSignalEvidence, string>` 경계로 이동했습니다.
[Signal Inspector export 경계](../../reports/OPENVISIONLAB_SIGNAL_INSPECTOR_EXPORT_BOUNDARY_20260914.md)와
`PL-0035` 원장에 기록하며, SaveFileDialog는 View presentation으로 유지하고
기존 `VisionToolSignalEvidenceExporter`가 파일 출력 owner로 남습니다.
Morphology Tool Partial은 재감사 결과 별도 이동 가능한 책임이 없었습니다.
[Morphology Tool Partial retention 감사](../../reports/OPENVISIONLAB_MORPHOLOGY_TOOL_PARTIAL_RETENTION_20260914.md)와
`PL-0036` 원장에 View/presenter/ViewModel/interaction-controller/base lifetime
소유권을 기록하고, 파일 길이만으로 새 Partial이나 wrapper를 만들지 않았습니다.
Edge Based Matching/Auto MPoint XAML Partial도 같은 기준으로 재감사했습니다.
[Edge Based Matching Partial retention 감사](../../reports/OPENVISIONLAB_EDGE_BASED_MATCHING_PARTIAL_RETENTION_20260914.md)와
`PL-0037` 원장에 View/Panel/teaching-controller/report-exporter/
factory/base lifetime 소유권을 기록하고, 새 독립 seam 없이 Partial·wrapper를
추가하지 않았습니다.
Feature Matching Tool View XAML Partial도 factory/composition/ViewModel/shared
matching runtime/base lifetime까지 추적해 같은 기준으로 재감사했습니다.
[Feature Matching Partial retention 감사](../../reports/OPENVISIONLAB_FEATURE_MATCHING_PARTIAL_RETENTION_20260914.md)와
`PL-0038` 원장에 View/ViewModel/controller/runtime/factory/base lifetime 및
binding/test 계약을 기록하고, 새 독립 seam 없이 Partial·wrapper를 추가하지
않았습니다.
Matching Tool View Partial의 sample template-path와 property projection 책임은
기존 `VisionPipelineMatchingPropertyAdapter`로 이동했습니다. [Matching Tool
View Partial 경계](../../reports/OPENVISIONLAB_MATCHING_TOOL_PARTIAL_BOUNDARY_20260914.md)와
`PL-0039` 원장에 adapter/View/ViewModel/runtime/factory/base owner, callback
순서, defensive list-copy, binding/test 계약을 기록했으며 View는 XAML/controller
orchestration만 유지합니다.
Threshold Tool Partial의 teaching-suggestion Analyze/Use/Undo workflow 조정은
WPF control을 참조하지 않는 기존 owner 조합 경계로 이동했습니다. [Threshold
Tool Partial 경계](../../reports/OPENVISIONLAB_THRESHOLD_TOOL_PARTIAL_BOUNDARY_20260914.md)와
`PL-0040` 원장에 View/controller/session/interaction/ViewModel/factory/base
owner, call path, binding/test 계약을 기록했으며 XAML automation ID와 기존
Preview/Undo 동작을 유지합니다.
Line Tool Partial의 sample A/B property projection은 기존
`VisionPipelineLinePropertyAdapter`로 이동했습니다. [Line Tool Partial 경계](../../reports/OPENVISIONLAB_LINE_TOOL_PARTIAL_BOUNDARY_20260914.md)와
`PL-0041` 원장에 adapter/View/ViewModel/presenter/interaction/preview/factory/base
owner, callback 순서, defensive list-copy, binding/test 계약을 기록했으며
PL-0027 persistence 경계는 유지합니다.
Simple Preprocess Tool Partial은 실제 owner 충돌을 찾지 못해 no-change로
유지했습니다. [Simple Preprocess Partial retention](../../reports/OPENVISIONLAB_SIMPLE_PREPROCESS_PARTIAL_RETENTION_20260914.md)과
`PL-0042` 원장에 parameter controller/text presenter/property factory/preview
executor/document factory/base lifetime owner, call path, binding/test 계약과
분리하지 않은 이유를 기록했습니다.
Affine Transform Tool Partial도 실제 owner 충돌을 찾지 못해 no-change로
유지했습니다. [Affine Transform Partial retention](../../reports/OPENVISIONLAB_AFFINE_TRANSFORM_PARTIAL_RETENTION_20260914.md)과
`PL-0043` 원장에 ViewModel/generic controller/result presenter/preview/factory/
composition/registry/base owner, call path, binding/test 계약과 분리하지 않은
이유를 기록했습니다.
Arithmetic Tool View Partial도 interaction/text/preview와 shared double-input
runtime, factory/document, registry/base owner를 대조한 결과 no-change로
유지했습니다. [Arithmetic Tool Partial retention](../../reports/OPENVISIONLAB_ARITHMETIC_TOOL_PARTIAL_RETENTION_20260914.md)과
`PL-0044` 원장에 editor state, settings/routing, binding/public/test 계약,
release owner와 분리하지 않은 이유를 기록했습니다.
Filter Tool View Partial도 interaction/kernel/text/guide와 shared single-input
runtime, factory/document, registry/base owner를 대조한 결과 no-change로
유지했습니다. [Filter Tool Partial retention](../../reports/OPENVISIONLAB_FILTER_TOOL_PARTIAL_RETENTION_20260914.md)과
`PL-0045` 원장에 editor state, settings/routing, binding/public/test 계약,
release owner와 분리하지 않은 이유를 기록했습니다.
Blob Tool View Partial도 Blob ViewModel/normalization, generic property-grid
controller/runtime, area verification/threshold teaching presenter, factory/
document, preview/overlay, registry/pipeline/base owner를 대조한 결과
no-change로 유지했습니다. [Blob Tool Partial retention](../../reports/OPENVISIONLAB_BLOB_TOOL_PARTIAL_RETENTION_20260914.md)과
`PL-0046` 원장에 editor state, teaching/review, settings/routing,
binding/public/test 계약, algorithm and release owner와 분리하지 않은 이유를
기록했습니다.
Contour Tool View Partial도 Contour ViewModel/normalization, generic
property-grid controller/runtime, area verification/threshold teaching
presenter, factory/document, preview/overlay, registry/pipeline/base owner를
대조한 결과 no-change로 유지했습니다. [Contour Tool Partial retention](../../reports/OPENVISIONLAB_CONTOUR_TOOL_PARTIAL_RETENTION_20260914.md)과
`PL-0047` 원장에 editor state, teaching/review, settings/routing,
binding/public/test 계약, algorithm and release owner와 분리하지 않은 이유를
기록했습니다.
Binary Learn View Partial도 `BinaryLearnPresenter`/simulation model, Learn
Window topic/action composition, WPF cell painting, three animation timer
lifetimes, automation/test facade owner를 대조한 결과 no-change로
유지했습니다. [Binary Learn Partial retention](../../reports/OPENVISIONLAB_BINARY_LEARN_PARTIAL_RETENTION_20260914.md)과
`PL-0048` 원장에 lesson state, presentation/timer lifetime, related-tool
callback, binding/public/test 계약과 분리하지 않은 이유를 기록했습니다.
Foundation Learn View Partial도 `FoundationLearnPresenter`와 Learn Window
composition이 lesson state/role/text와 topic/action routing을 이미 소유하고,
View는 Mat/ROI 셀·marker·brush·두 animation timer만 담당하므로 no-change로
유지했습니다. [Foundation Learn Partial retention](../../reports/OPENVISIONLAB_FOUNDATION_LEARN_PARTIAL_RETENTION_20260914.md)과
`PL-0049` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
계약 및 monitor-aware WPF smoke 증거를 기록했습니다.
Grayscale Learn View Partial도 `GrayscaleLearnPresenter`/basic simulation
model이 Threshold·Brightness·Arithmetic·Filtering 상태/평가를 소유하고,
Learn Window가 topic·Apply/Close·Tool 결과를 조합하며 View는 WPF cells,
marker, 네 animation timer와 명시적 event 결과만 담당하므로 no-change로
유지했습니다. [Grayscale Learn Partial retention](../../reports/OPENVISIONLAB_GRAYSCALE_LEARN_PARTIAL_RETENTION_20260914.md)과
`PL-0050` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
계약 및 monitor-aware WPF smoke 증거를 기록했습니다.
Layer/Recipe Learn View Partial도 `LayerRecipeLearnPresenter`가 고정 Layer 목록,
Step route, formula/meaning/status와 animation state를 소유하고, Learn Window가
topic visibility·refresh·public facade·close lifetime을 조합하며 View는 WPF
cells, brush/text projection과 520ms timer만 담당하므로 no-change로 유지했습니다.
[Layer/Recipe Learn Partial retention](../../reports/OPENVISIONLAB_LAYER_RECIPE_LEARN_PARTIAL_RETENTION_20260914.md)과
`PL-0051` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
계약 및 monitor-aware WPF smoke 증거를 기록했습니다.
Geometry Learn View Partial도 `GeometryLearnPresenter`가 Angle·Scale·Rotate→
Scale→ROI review 단계, semantic role, formula/status와 Tool hint policy를
소유하고, Learn Window가 topic·callback·public facade·close lifetime을
조합하며 View는 WPF transform/brush/text projection과 520ms timer만 담당하므로
no-change로 유지했습니다. [Geometry Learn Partial retention](../../reports/OPENVISIONLAB_GEOMETRY_LEARN_PARTIAL_RETENTION_20260914.md)과
`PL-0052` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
계약 및 monitor-aware WPF smoke 증거를 기록했습니다.
Metrics Acceptance Learn View Partial도 `MetricsAcceptanceLearnPresenter`가
고정 샘플, 평균·범위·최대값 gate, 단계·문구를 소유하고, Learn Window가
topic/refresh/public facade/close lifetime을 조합하며 View는 WPF sample-cell,
brush/text projection과 520ms timer만 담당하므로 no-change로 유지했습니다.
[Metrics Acceptance Learn Partial retention](../../reports/OPENVISIONLAB_METRICS_ACCEPTANCE_LEARN_PARTIAL_RETENTION_20260914.md)과
`PL-0053` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
계약 및 monitor-aware WPF smoke 증거를 기록했습니다.
`VisionToolVerificationGuideView` Partial도 dependency-property/compact-density
visual adapter와 기존 area/matching presenter·공통 Tool shell owner를 대조한
결과 no-change로 유지했습니다. [Vision Tool Verification Guide Partial retention](../../reports/OPENVISIONLAB_VERIFICATION_GUIDE_PARTIAL_RETENTION_20260914.md)과
`PL-0054` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
계약 및 monitor-aware shared-shell WPF smoke 증거를 기록했습니다.
`VisionToolParameterGuideView` Partial도 XAML/content projection과 related-property
callback만 유지하고 기존 presenter/catalog/binder/sidecar/shell owner를
보호했습니다. 동시에 전역 PropertyGrid descriptor 필터가 숨긴 public dependent
property도 guidance를 잃지 않도록 기존 `VisionToolParameterGuideCatalog`에
reflection descriptor fallback을 추가했습니다. [Parameter Guide Partial retention](../../reports/OPENVISIONLAB_PARAMETER_GUIDE_PARTIAL_RETENTION_20260914.md)과
`PL-0055` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
contract, `p257`/`p259`/`p260` WPF smoke 및 monitor-aware 증거를 기록했습니다.
`VisionToolDoubleInputCustomToolShell` Partial은 XAML namescope, dependency
property, preview/button facade와 docked-density projection만 담당하고 기존
double-input runtime/controller/base View가 layer·preview callback과 lifetime을
소유하므로 no-change로 유지했습니다. [Double-input shell Partial retention](../../reports/OPENVISIONLAB_DOUBLE_INPUT_SHELL_PARTIAL_RETENTION_20260914.md)과
`PL-0056` 원장에 owner/call-path, binding/public/test 계약, Debug/Release
contract, Arithmetic WPF smoke 및 동적 모니터 배치 증거를 기록했습니다. 새
ViewModel·service·wrapper·Partial은 추가하지 않았습니다.
5개 부담 영역의 최종 재평가와 추천 코드 읽기 순서는
[2026-09-12 재평가 보고서](../../reports/OPENVISIONLAB_JUNIOR_BURDEN_REASSESSMENT_20260912.md)에
기록합니다.
현재 Windows 세션에서 실행 가능한 WPF DPI·테마·모니터 행과 미검증 행은
[WPF 런타임 환경 행렬 기록](../../reports/OPENVISIONLAB_WPF_RUNTIME_ENVIRONMENT_MATRIX_20260911.md)에
기록합니다.
현재 Pipeline Review 실행·이미지 수명·stale callback·문서 revision 계약의
재검증 결과는 [Pipeline Review 실행 재검증](../../reports/OPENVISIONLAB_PIPELINE_REVIEW_RECHECK_20260911.md)에
기록합니다. 이 보고서는 재현되지 않은 결함을 이유로 새 추상화나 partial을
추가하지 않고, 실제 계약 결과와 남은 UI/장비 검증 경계를 고정합니다.
현재 Dev working tree의 source-build·public catalog·WPF shell release
사전 점검과 commit/tag 경계는 [릴리스 사전 점검](../../reports/OPENVISIONLAB_RELEASE_PRECHECK_20260911.md)에
기록합니다. clean exact commit이 없으면 TagReady로 승격하지 않습니다.
ImageCanvas에 남아 있던 운영 stdout timing 출력 제거와 이미지 수명 보존
검증은 [ImageCanvas debug 출력 정리](../../reports/OPENVISIONLAB_IMAGECANVAS_DEBUG_OUTPUT_CLEANUP_20260911.md)에
기록합니다.
Shell code-behind에서 Recipe 선택·Workspace 이미지 경로 persistence가 어느
concrete owner로 가는지는 [Shell MVVM 상태 소유권 보정](../../reports/OPENVISIONLAB_MVVM_SHELL_STATE_OWNERSHIP_20260910.md)에
기록합니다.
Shell readiness 정책이 어느 상태 owner에서 계산되는지는
[Shell readiness MVVM 경계 보정](../../reports/OPENVISIONLAB_MVVM_SHELL_READINESS_20260910.md)에
기록합니다.
Shell Recipe 저장 callback의 concrete owner는
[Shell Recipe 저장 callback 소유권 보정](../../reports/OPENVISIONLAB_MVVM_SHELL_RECIPE_SAVE_20260910.md)에
기록합니다.
남은 Shell/Pipeline Review code-behind가 UI 전용인지 확인한 최종 감사는
[code-behind 경계 감사](../../reports/OPENVISIONLAB_SHELL_PIPELINE_CODEBEHIND_BOUNDARY_AUDIT_20260910.md)에
기록합니다.
물리적 source folder 배치와 root-class 후보 감사는
[2026-09-10 폴더 감사](../../reports/OPENVISIONLAB_FOLDER_ORGANIZATION_AUDIT_20260910.md)에서
확인합니다.
`src/OpenVisionLab/Common`의 책임별 하위 폴더 이동과 남겨 둔 legacy root
타입은 [Common 폴더 정리](../../reports/OPENVISIONLAB_COMMON_FOLDER_REORGANIZATION_20260910.md)에
기록합니다.
Shell validation/Step Edit 잔여 owner 감사는
[2026-09-09 Shell audit](../../reports/OPENVISIONLAB_SHELL_VALIDATION_STEP_EDIT_AUDIT_20260909.md)에서
호출 경로와 source-only 검증 범위를 확인할 수 있습니다.

| 작업 | 먼저 읽을 문서 |
| --- | --- |
| 작업 시작/계속 | `AGENTS.md` → 현재 핸드오프 → 제품 목표 → 안정 계약 |
| 현재 상태/다음 작업 | 현재 핸드오프 → `docs/reports/OPENVISIONLAB_CODEBASE_MODULE_AUDIT_20260908.md` → `docs/reports/OPENVISIONLAB_OVL37_TEMPLATE_IMAGE_EXTRACTION_NAMESPACE_20260909.md` → `docs/reports/OPENVISIONLAB_OVL36_PROPERTYGRID_METADATA_ADAPTER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL35_RECIPE_VALIDATION_SUITE_VIEW_20260909.md` → `docs/reports/OPENVISIONLAB_OVL34_VALIDATION_DATASET_DRAWING_EVIDENCE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL33_VALIDATION_DATASET_REVIEW_QUEUE_EVIDENCE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL32_VALIDATION_DATASET_EXECUTION_PROGRESS_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL31_VALIDATION_DATASET_CONFIGURATION_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL30_VALIDATION_DATASET_SUMMARY_ARTIFACT_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL29_SMOKE_RECIPE_WORKSPACE_CLEANUP_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL28_RECIPE_CONTEXT_FIXTURE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL27_VALIDATION_DATASET_ARTIFACT_WRITER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL26_SCREENSHOT_CAPTURE_LIFECYCLE_20260909.md` → `docs/reports/OPENVISIONLAB_OVL25_SCREENSHOT_PNG_WRITER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL24_SCREENSHOT_BITMAP_ASSERTIONS_20260909.md` → `docs/reports/OPENVISIONLAB_OVL23_LEARN_DOCUMENT_COPY_POLICY_20260909.md` → `docs/reports/OPENVISIONLAB_OVL22_SMOKE_RUNNER_TARGET_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL21_NAMESPACE_PROJECT_BOUNDARY_20260909.md` → `docs/reports/OPENVISIONLAB_OVL20_PROPERTYGRID_VALUE_CHANGE_SUBSCRIPTION_20260909.md` → `docs/reports/OPENVISIONLAB_OVL19_SHELL_RECIPE_BASIC_LIFECYCLE_VIEW_20260909.md` → `docs/reports/OPENVISIONLAB_OVL18_SHELL_STEP_PREVIEW_NAVIGATION_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL17_SHELL_VALIDATION_EVIDENCE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL16_RECIPE_LOCAL_VALIDATION_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL15_RECIPE_CATALOG_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL14_RECIPE_PAIR_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL13_RECIPE_SELECTED_SAMPLE_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL12_IMAGECOMPARE_RESOURCE_OWNER_20260908.md` → 이전 프로그램 감사/최신 관련 `docs/reports/` → 완료 추적기 |
| UI/운영 흐름 변경 | 안정 계약 → 사용자 중심 워크플로 보고서 → 관련 기능 계약 → UI smoke runbook |
| 초보자 매뉴얼/튜토리얼/Learn | `docs/manual/README.md` → `docs/manual/manual-visuals.json` → `docs/learn/OPENVISIONLAB_TUTORIAL.md` → 관련 Tool Learn 문서 |
| Recipe/Pipeline/XML 변경 | 안정 계약 → Vision Tool 계약/결과 계약 → LLM XML 가이드와 Tool Catalog(LLM 호환성이 관련될 때만) |
| 샘플/검증/외부 자산 | Public Sample 정책 → External Reference 정책 → 관련 검증 보고서 |
| 빌드/릴리스/배포 | Release Version 정책 → Source Build 보고서 → Runtime Data Root 계약/보고서 → Production Release Gate |
| 소스 구조/소유권 | `docs/admin/CODEBASE_STRUCTURE.md` → `docs/reports/OPENVISIONLAB_CODEBASE_MODULE_AUDIT_20260908.md` → `docs/reports/OPENVISIONLAB_OVL37_TEMPLATE_IMAGE_EXTRACTION_NAMESPACE_20260909.md` → `docs/reports/OPENVISIONLAB_OVL36_PROPERTYGRID_METADATA_ADAPTER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL35_RECIPE_VALIDATION_SUITE_VIEW_20260909.md` → `docs/reports/OPENVISIONLAB_OVL34_VALIDATION_DATASET_DRAWING_EVIDENCE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL33_VALIDATION_DATASET_REVIEW_QUEUE_EVIDENCE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL32_VALIDATION_DATASET_EXECUTION_PROGRESS_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL31_VALIDATION_DATASET_CONFIGURATION_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL30_VALIDATION_DATASET_SUMMARY_ARTIFACT_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL29_SMOKE_RECIPE_WORKSPACE_CLEANUP_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL28_RECIPE_CONTEXT_FIXTURE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL27_VALIDATION_DATASET_ARTIFACT_WRITER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL26_SCREENSHOT_CAPTURE_LIFECYCLE_20260909.md` → `docs/reports/OPENVISIONLAB_OVL25_SCREENSHOT_PNG_WRITER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL24_SCREENSHOT_BITMAP_ASSERTIONS_20260909.md` → `docs/reports/OPENVISIONLAB_OVL23_LEARN_DOCUMENT_COPY_POLICY_20260909.md` → `docs/reports/OPENVISIONLAB_OVL22_SMOKE_RUNNER_TARGET_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL21_NAMESPACE_PROJECT_BOUNDARY_20260909.md` → `docs/reports/OPENVISIONLAB_OVL20_PROPERTYGRID_VALUE_CHANGE_SUBSCRIPTION_20260909.md` → `docs/reports/OPENVISIONLAB_OVL19_SHELL_RECIPE_BASIC_LIFECYCLE_VIEW_20260909.md` → `docs/reports/OPENVISIONLAB_OVL18_SHELL_STEP_PREVIEW_NAVIGATION_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL17_SHELL_VALIDATION_EVIDENCE_OWNER_20260909.md` → `docs/reports/OPENVISIONLAB_OVL16_RECIPE_LOCAL_VALIDATION_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL15_RECIPE_CATALOG_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL14_RECIPE_PAIR_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL13_RECIPE_SELECTED_SAMPLE_EXECUTION_OWNER_20260908.md` → `docs/reports/OPENVISIONLAB_OVL12_IMAGECOMPARE_RESOURCE_OWNER_20260908.md` → 이전 프로그램 감사/구조 리팩터링 완료 기록 → 최신 Source Layout Migration 보고서 |
| 상용 제품 비교 | 현재 핸드오프 → Commercial Video Backlog/Queue → 과거 비교 문서(참고만) |
| 특정 `P###` 조사 | 현재 핸드오프에서 검색 → 완료 추적기 → 상세 세션 핸드오프 → 해당 보고서/증거 폴더 |

정확한 경로 목록과 작업별 `read` 배열은 `docs/LLM_DOCUMENT_INDEX.json`이 관리합니다.

## 폴더 의미

- `docs/admin/`: 현재 핸드오프, 문서 지도, 운영·구조 기록
- `docs/admin/archive/`, `docs/roadmap/archive/`: 현재 우선순위로 사용하지 않는 과거 핸드오프·계획 원문
- `docs/contracts/`: 현재 동작/정책/XML/API 계약
- `docs/roadmap/`: 제품 목표와 승인된 개발 큐
- `docs/reports/`: 작업 단위의 결과, 검증, 실패와 한계
- `docs/evidence/`: 원시 또는 상세 검증 자료
- `docs/research/`, `docs/analysis/`: 조사·비교·실험 분석(현재 권위가 아님)
- `docs/runbooks/`: 반복 가능한 실행/Smoke 절차
- `docs/learn/`: 사용자 학습 문서
- `docs/assets/`, `docs/samples/`: 문서 부속 자료와 샘플 메타데이터

## 루트 리다이렉트 규칙

`docs/` 바로 아래의 기존 문서 대부분은 이전 링크 호환을 위한 작은 이동 안내 파일입니다. LLM은 이동 안내에서 가리키는 하위 폴더의 본문을 읽어야 하며, 이동 안내 자체를 권위 문서로 인용하면 안 됩니다.

루트의 문서 진입점은 `README.md`와 `LLM_DOCUMENT_INDEX.json`입니다. 나머지 루트 문서는 이전 링크 호환용 이동 안내입니다. 배포용 튜토리얼은 `docs/learn/OPENVISIONLAB_TUTORIAL_PORTABLE.html`입니다.

## 검색 예시

```powershell
# 파일명 또는 문서 본문 찾기
rg --files docs | rg "CURRENT_HANDOFF|RUNTIME_DATA_ROOT|P276"
rg -n "P276|ResultCount|Preview/Run" AGENTS.md docs

# 최신 보고서 후보 보기
Get-ChildItem docs/reports -File | Sort-Object LastWriteTime -Descending | Select-Object -First 20

# 문서 색인과 모든 루트 리다이렉트 검증
powershell -NoProfile -ExecutionPolicy Bypass -File tools/TestDocumentationIndex.ps1
```

## 문서 갱신 규칙

1. 현재 상태나 우선순위가 바뀌면 `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`를 갱신합니다.
   최근 상태와 활성 조건만 남기고, 상세 명령과 긴 P 이력은 날짜가 있는
   `docs/reports/` 문서에 기록합니다. 완료 항목을 계속 누적해 현재
   핸드오프를 다시 연대기 문서로 만들지 않습니다.
2. 안정 동작이 바뀌면 해당 `docs/contracts/` 문서를 갱신합니다.
3. 완료/실패/검증 한계는 날짜가 있는 `docs/reports/` 문서에 기록합니다.
4. 새 문서가 반복 작업의 주요 입구라면 `docs/LLM_DOCUMENT_INDEX.json`의 관련 route에 추가합니다.
5. 전체 상세 등록이 필요하면 `docs/admin/OPENVISIONLAB_DOCUMENTATION_MAP.md`를 갱신합니다.
6. `tools/TestDocumentationIndex.ps1`가 통과해야 문서 정리가 완료됩니다.
