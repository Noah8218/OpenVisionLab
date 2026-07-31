# Auto MPoint Teaching Controller Refactor Proof

Updated: 2026-07-26 KST

## Status

Status: Complete

Scope: `EdgeBasedMatchingToolWpfView`에 남아 있던 Auto MPoint 교육
워크플로 소유권을 하나의 전용 Controller로 이동했습니다. 알고리즘,
파라미터, 화면 문구, Preview/Run, 레이어, 라우팅 계약은 변경하지
않았습니다.

## Candidate Audit

- `OpenVisionLearnWindow`: 계산 모델 추출이 마감되어 재개하지 않았습니다.
- `RoiEditorWindow`: `RoiEditorViewModel`이 ROI 목록/선택 상태를 이미
  소유하며 남은 코드는 WPF 좌표 변환, hit test, drag, drawing입니다.
- `OpenGlTemplateEditorWindow`: 남은 큰 블록은 WPF/OpenGL viewer 입력과
  ROI rendering이며 template 추출은 기존 `TemplateImageExtraction`이
  소유합니다.
- `OpenVisionPipelineReviewView`: ViewModel, Presenter, Execution
  Controller, render service가 이미 존재합니다.
- `EdgeBasedMatchingToolWpfView`: 대표 이미지와 source bitmap 수명,
  Auto MPoint 실행, 후보/분석 revision, report 검증/내보내기, template
  적용을 View가 직접 소유해 실제 경계로 선택했습니다.

파일 길이는 후보 탐색 신호로만 사용했고, 분리 결정은 책임과 호출
경로를 기준으로 했습니다.

## Structural Changes Confirmed

### Before

- Responsibility owner: `EdgeBasedMatchingToolWpfView`
- Call path: WPF button event -> View private method -> Auto MPoint runtime /
  report exporter / template save
- Dependency direction: View -> algorithm, OpenCV image loading, file
  fingerprint, report exporter, PropertyGrid template save
- State/data owner: View가 source bitmap, representative paths, source
  revision, analyzed definition, applied template path를 보유

### After

- Responsibility owner:
  `src/OpenVisionLab/UI/VisionTest/Wpf/Tooling/Review/AutoMPointTeachingController.cs`
- Call path: View composition -> `AutoMPointTeachingController` -> existing
  matching controller/runtime/exporter
- Dependency direction: View는 Auto MPoint Controller만 사용하며,
  Controller가 기존 algorithm/export/template-save dependencies를 사용
- State/data owner: Controller가 source/representative image 수명,
  analysis identity, candidate selection enablement, report/apply workflow를
  보유

`EdgeBasedMatchingToolWpfView`에는 Tool View 구성, verification guide
접기/펼치기, 기존 test facade 위임, input preview 전달, resource disposal
연결만 남았습니다.

## Acceptance Criteria

- View에 `AutoMPointTool`, representative path collection,
  `CreateAnalysisDefinition`, report exporter, template save 호출이 남지
  않는다: Pass
- Controller가 analyze, candidate selection, representative-image
  selection, report export, pattern apply와 관련 상태를 소유한다: Pass
- 기존 Auto MPoint UI가 대표 이미지 3장, 후보 선택, pattern apply,
  N-image HTML report를 완료한다: Pass
- Auto MPoint 동작이 Preview count, layer count, input/output route,
  active layer를 변경하지 않는다: Pass
- 일반 Edge Based Matching Tool 화면과 selected-Step round trip이
  유지된다: Pass
- Debug build와 readiness contract가 통과한다: Pass

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -m:1 -nr:false`
  - Pass, 0 warnings / 0 errors
- `dotnet build ".\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -m:1 -nr:false`
  - Pass, 0 warnings / 0 errors
- `PipelineViewerScreenshotSmoke --target wpf_shell_host_edge_based_matching_auto_mpoint`
  - Pass, `check=OK`, `layout=0`, `text=0`, `internal=0`
  - 대표 이미지 3장, 후보 분석/선택, 패턴 적용, HTML export, no-auto-run /
    no-layer-route-mutation 계약 포함
- `PipelineViewerScreenshotSmoke --target wpf_shell_host_edge_based_matching_tool`
  - Pass, `check=OK`, `layout=0`, `text=0`, `internal=0`
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"`
  - Pass
- `git diff --check`
  - Pass

변경 중 첫 compile은 새 owner에 `Lib.OpenCV.Result` import가 빠져
실패했고 추가 후 통과했습니다. 첫 두 readiness 실행은 새 파일이
ToolViews/Review 소유 목록에 없어서 실패했습니다. Controller를
`Tooling/Review`로 이동하고 정확한 owner 목록을 갱신한 후 최종
readiness가 통과했습니다. 계약을 우회하는 탐색 fallback은 추가하지
않았습니다.

## UI Evidence

- Before:
  `artifacts/refactor_auto_mpoint_teaching_controller_20260726/before/wpf_shell_host_edge_based_matching_auto_mpoint.png`
- After:
  `artifacts/refactor_auto_mpoint_teaching_controller_20260726/after/wpf_shell_host_edge_based_matching_auto_mpoint.png`
- General Edge Based Tool:
  `artifacts/refactor_auto_mpoint_teaching_controller_20260726/edge_based_tool/wpf_shell_host_edge_based_matching_tool.png`

전후 화면의 배치, 컨트롤, 대표 이미지 수, 선택 후보와 결과 도면은
동일합니다. smoke가 매번 생성하는 template 파일명/시각과 Pipeline
Step 번호만 달라졌습니다.

## Boundary

이 변경은 기존 Auto MPoint 교육 UI의 소유권 이동만 증명합니다. 새
matcher, 새 locator qualification, 데이터셋 실행, 병렬화, LLM 확장을
추가하거나 증명하지 않습니다.

다음 구조 변경은 다른 큰 파일의 줄 수가 아니라, 구체적인 유지보수
변경이나 재현된 회귀가 새 owner 필요성을 증명할 때만 선택합니다.
