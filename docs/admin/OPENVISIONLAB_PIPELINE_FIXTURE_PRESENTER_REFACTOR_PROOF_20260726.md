# Pipeline Review Fixture Presenter Refactor Proof

Updated: 2026-07-26 KST

## Completion Record

Status: Complete

Scope: `OpenVisionPipelineReviewDocument`가 직접 소유하던 Fixture Designer의
표시 계산을 `OpenVisionPipelineReviewFixturePresenter`로 이동했습니다.
Fixture 체인 탐색, Step 파라미터 해석, 현재/기준 pose 계산, 상대 ROI 변환,
template/ROI preview bitmap 생성과 표시 문구가 Presenter의 응집된 책임이
됐습니다. Document는 선택 상태 반영, 명령 오케스트레이션, reference 저장과
재검증을 유지합니다.

Acceptance criteria:

- Fixture 계산과 preview 합성이 Document에서 제거되고 독립 Presenter가
  소유한다: Pass
- Reference Teach와 Fixture Designer의 기존 결과 및 명시적 실행 계약이
  유지된다: Pass
- 새 Presenter가 pipeline 실행이나 pipeline 저장을 소유하지 않는다: Pass
- Debug build, 정확한 두 UI smoke, readiness와 patch hygiene가 통과한다:
  Pass

Verification:

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -m:1 -nr:false`
  - Pass, 0 warnings / 0 errors
- `dotnet build ".\tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -m:1 -nr:false`
  - Pass, 0 warnings / 0 errors
- `PipelineViewerScreenshotSmoke --target wpf_shell_host_workspace_sample_normalize_fixture_review`
  - Pass, `check=OK`, `layout=0`, `text=0`, `internal=0`,
    `1180x890`
- `PipelineViewerScreenshotSmoke --target wpf_shell_host_workspace_sample_fixture_teach`
  - Pass, `check=OK`, `layout=0`, `text=0`, `internal=0`,
    `1180x890`
- `dotnet run --project .\tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  - Pass; 새 Fixture Presenter owner/delegation/금지 의존성 검사 포함
- 구조 검색
  - Document에는 Presenter 호출만 남고 `TryResolveFixtureChain`,
    `TransformReferenceRoi`, `DrawRoiOverlay`, `TryGetStepRoi`,
    `GetTemplateValue` 구현이 없음
  - Presenter가 위 계산과 preview 합성을 소유함
- `git diff --check`
  - Pass; CRLF 변환 알림만 존재하며 whitespace 오류 없음

Evidence:

- Before:
  `artifacts/refactor_pipeline_fixture_presenter_20260726/before/wpf_shell_host_workspace_sample_normalize_fixture_review.png`
- Final current-source Fixture Designer:
  `artifacts/refactor_pipeline_fixture_presenter_20260726/final/wpf_shell_host_workspace_sample_normalize_fixture_review.png`
- Final current-source Reference Teach:
  `artifacts/refactor_pipeline_fixture_presenter_20260726/fixture_teach_final/wpf_shell_host_workspace_sample_fixture_teach.png`
- 즉시 post-extraction 캡처는 before와 SHA-256
  `33634B8E027AAF83847744EE7230E6491952E9EB4FE9403FF042FA5A5556625F`로
  byte-identical이었습니다.
- 최종 재캡처는 before 대비 1,050,200 pixel 중 174 pixel
  (`0.016568%`)만 달랐고 차이는 `x=1083..1113`, `y=523..532`의
  31x10 영역에 한정됐습니다. 두 캡처 모두 exact semantic/layout smoke를
  통과했습니다.

Boundary / next dependency: 이 변경은 기존 Fixture Designer 표시 책임의
소유권 이동만 증명합니다. 새 locator, measurement algorithm, 자동 실행,
저장 형식, 산업 정확도 또는 field qualification을 추가하거나 증명하지
않습니다. 다음 구조 변경은 구체적인 유지보수 변경 또는 현재 빌드 회귀가
새 owner 필요성을 증명할 때만 다시 선택합니다.

## Structural Boundary

### Before

- Owner: `OpenVisionPipelineReviewDocument`
- Call path: Document state update -> Fixture chain/pose/ROI/template 계산 ->
  WPF-bound property 갱신
- Dependencies: Document가 pipeline graph, execution summary, drawing,
  bitmap preview와 표시 문구 계산을 함께 참조
- Size: Document 1,362 lines

### After

- Owner:
  `UI/Menu/Wpf/PipelineReview/Presenters/OpenVisionPipelineReviewFixturePresenter.cs`
- Call path: Document orchestration -> Presenter `Create` -> immutable
  presentation state -> Document의 WPF-bound property 갱신
- Dependencies: Presenter는 pipeline/summary/preview resolver를 입력받고,
  pipeline 실행과 저장에는 의존하지 않음
- State: `OpenVisionPipelineReviewFixtureState`가 생성한 preview bitmap
  수명과 표시 결과를 소유하고 `Dispose`로 정리
- Size: Document 942 lines, Presenter 594 lines

이는 파일 길이를 맞추기 위한 partial 분리가 아닙니다. Fixture Designer의
계산과 presentation state라는 독립 책임, 명시적 입력, 독립 resource
수명과 readiness 검증 경계를 기준으로 분리했습니다.

## Recovered Validation Failures

- 변경 전 baseline에서
  `wpf_shell_host_workspace_sample_fixture_review`는 현재 화면에 없는
  `PipelineReviewEditSelectedStepButton` AutomationId를 요구해 실패했습니다.
  Fixture Designer의 실제 안정 계약인
  `wpf_shell_host_workspace_sample_normalize_fixture_review`를 before/final
  gate로 사용했습니다. 오래된 target 실패를 기능 회귀로 포장하지
  않았습니다.
- 첫 extraction build는 C# definite assignment가 요구한 reference pose
  지역 변수 초기화가 빠져 실패했습니다. 네 값을 명시적으로 초기화한 뒤
  최종 빌드와 두 UI smoke가 통과했습니다.
