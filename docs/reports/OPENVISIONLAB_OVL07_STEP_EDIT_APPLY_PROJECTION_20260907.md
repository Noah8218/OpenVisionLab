# OpenVisionLab OVL-07 Step Edit apply 결과/상태 projection owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 OVL-07에서 독립적으로 검증 가능한 한 가지 작업으로,
`OpenVisionRecipeStepEditApplyResult`를 사용자에게 표시할 Step Edit 상태,
Shell 상태, corrected-output review 텍스트로 변환하는 정책을
`OpenVisionShellHostRecipeCommandSurface.Handlers.cs`에서 Window-free
concrete `OpenVisionRecipeStepEditApplyProjectionOwner`로 이동했다.

Shell은 기존 PropertyGrid commit callback, XML apply transaction 호출,
`OpenVisionRecipeStepEditSessionViewModel`의 dirty/clean 상태, pipeline
refresh/selection, 명시적 Preview/Run 순서를 계속 소유한다. Recipe/XML
schema·경로, Layer routing, 기존 한국어/영어 문구와 상태 binding 이름은
변경하지 않았다. XAML이나 제품 UI 레이아웃은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| apply 실패 상태/복원 상태 문구 | Shell handler의 inline 분기 | `ProjectFailure` |
| 성공 Step 상태 문구 | Shell handler의 inline 문자열 조합 | `ProjectSuccess` |
| corrected-output review 문구 | Shell handler가 Presenter 직접 호출 | projection owner가 기존 Presenter 호출 |
| dirty/clean·refresh·선택·Preview/Run | Shell/session | 변경 없음. Shell이 projection을 반영 |

호출 경로는 다음과 같다.

`WPF Apply command → Shell transaction owner → OpenVisionRecipeStepEditApplyResult`
` → OpenVisionRecipeStepEditApplyProjectionOwner → existing session/status/refresh`

Projection owner는 WPF/View/Shell 객체를 참조하지 않고 텍스트와 결과 DTO만
반환한다. 실패 projection은 편집 상태와 Shell 상태를 분리하며, 성공
projection은 기존 Pipeline/Step/검증 문구와 corrected-output review를 함께
반환한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeStepEditApplyProjectionOwner.cs`
  - 성공/실패 apply 결과를 기존 localized status 및 corrected-output review
    텍스트로 변환하는 concrete owner와 projection DTO를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - projection owner dependency를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - inline apply-result 문자열 조합을 제거하고 projection owner 호출 후 기존
    session/status/refresh 흐름에 결과를 반영한다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--step-edit-apply-projection-contract` 무창 계약을 추가했다.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`
  - 구조 증거, 검증 결과, 다음 단일 우선순위를 기록했다.

## 구조 증거

- Handler에는 `ProjectFailure`와 `ProjectSuccess` 호출이 남고, 기존
  `RestoreSucceeded` 분기, 성공 상태 문자열, `BuildCorrectedOutputAppliedText`
  직접 호출은 제거됐다.
- `OpenVisionRecipeStepEditApplyProjectionOwner`가 기존
  `OpenVisionRecipeText`와 `OpenVisionRecipePipelineStepReviewPresenter`를
  호출하지만 `Window`, `UserControl`, `System.Windows`,
  `OpenVisionShellHost` 의존성은 없다.
- `selectedStepEditSession.MarkClean`, `RefreshPipelineOptions`, Step 재선택,
  `SetSelectedStepEditStatus`, `SetCorrectedOutputReview`, `StatusText` 반영은
  Shell에 남아 mutable UI/session 상태 owner를 변경하지 않았다.
- 구조 정적 검사 결과는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-apply-projection-20260907\static-step-edit-apply-projection-check.log`
  에 `Result=PASS`로 기록했다.

## 검증

- x64 Debug/Release app build: 각각 0 warnings / 0 errors.
- x64 Debug/Release `VisionRecipeRunnerSmoke` build: 각각 0 warnings / 0 errors.
- `--step-edit-apply-projection-contract` Debug/Release: PASS.
  - 정상 apply의 pipeline/Step/검증 상태와 corrected-output review 투영
  - 저장 실패의 편집 상태 전용 오류 투영
  - 왕복 검증 실패 후 복원 성공/실패 Shell 상태 투영
  - 지원하지 않는 PropertyGrid 오류의 상태 채널 보존
- 기존 `--step-edit-apply-owner-contract` Debug/Release도 PASS하여 앞 단계
  XML 저장·왕복검증·복원 계약이 유지됨을 확인했다.
- `PipelineViewerScreenshotSmoke` Debug/Release build: 기존 nullable
  conversion warning 1개(`Program.cs:10722`), 0 errors.
- 최신 WPF Debug/Release의
  `wpf_shell_host_pipeline_step_edit_handoff`와
  `wpf_shell_host_fixture_step_edit_apply_rerun` 모두 exit 0,
  `OK|check=OK`, `layout=0|text=0|internal=0`, `size=1600x900`이었다.
- 두 구성 모두 정확히 두 모니터를 동적으로 조회해 작은 왼쪽
  `\\.\DISPLAY2`(work area `-1920,365,1920x1032`)를 선택했고 실제 창
  사각형 `-1760,431..-160,1331`을 확인했다. 실행 로그와 최신 PNG는
  evidence directory에 보관했다.
- `git diff --check`, 문서 index 검증 결과도 evidence directory에 기록했다.

## 범위 경계

이번 완료는 Step Edit apply 결과/상태/corrected-output projection owner 분리에
한정한다. Recipe Manager lifecycle/CRUD, 이미지·Layer lifetime, 전체
theme/layout/DPI(100/125/150/175/200%)/multi-monitor 행렬은 이번 실행에서
검증하지 않았다. WPF smoke는 기본 theme/language, 1600x900에서 실행했다.
Original 저장소, commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** Shell handler의 apply-result 상태/알림/corrected-output
  문자열 projection.
- **Intended owner:** `OpenVisionRecipeStepEditApplyProjectionOwner` concrete
  module.
- **Dependency direction:** Shell handler → projection owner → 기존 text/
  review presenter; projection DTO → 기존 session/status/refresh state.
- **State owner:** projection owner는 상태를 보유하지 않고 DTO만 만들며,
  mutable edit/dirty/status/review 상태는 기존 session, refresh와 실행 순서는
  Shell이 계속 소유한다.
- **Observable contract:** 기존 localized messages, binding names, XML/Layer
  behavior, explicit Preview/Run semantics를 유지한다.

Status: Complete
Scope: OVL-07 Step Edit apply result/status/corrected-output projection owner 분리.
Acceptance criteria: Handler inline projection 제거, Window-free concrete owner 호출, 기존 session/refresh/Recipe/XML/Preview/Run 계약 유지, Debug/Release projection contract와 WPF smoke 통과.
Verification: Debug/Release builds, projection/owner contracts, monitor-aware WPF smoke, static structure, `git diff --check`, documentation-index validation.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-apply-projection-20260907`.
Boundary / next dependency: OVL-07 Recipe Manager lifecycle/CRUD 책임 owner 분리가 다음 단일 작업이며, 전체 UI 행렬과 Original 저장소는 이번 결과가 증명하지 않는다.
