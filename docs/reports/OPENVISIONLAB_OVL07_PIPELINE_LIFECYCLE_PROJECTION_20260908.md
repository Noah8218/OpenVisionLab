# OpenVisionLab OVL-07 Pipeline lifecycle result/status projection owner 분리

작성일: 2026-09-08 KST

## 의미와 범위

이번 실행은 Recipe Manager Pipeline lifecycle 결과를 Shell `StatusText`로
투영하는 책임을 분리했다. Window-free concrete
`OpenVisionRecipePipelineLifecycleProjectionOwner`가 Activate, Duplicate,
Rename, Delete, DuplicateFromSample의 결과 snapshot을 받아 성공 여부,
resulting Pipeline name, 표시 문구를 반환한다.

Pipeline 저장·복제·이름 변경·삭제·active pointer와 fallback 선택은 기존
`OpenVisionRecipePipelineLifecycleUseCase`에 남겼다. Shell은 command guard,
delete confirmation, pending-edit 전환, 선택 상태, active 여부에 따른
refresh와 `StatusText` PropertyChanged를 계속 소유한다. Recipe/XML schema,
Layer routing, 명시적 Preview/Run 계약과 XAML은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| Activate status | `PipelineLifecycle.cs`의 inline localized format | projection owner `Project(Activate, result)` |
| Duplicate/Rename/Delete status | Shell의 `result.Detail` 직접 대입 | owner projection의 `StatusText` |
| DuplicateFromSample success/failure | Shell의 localized prefix/format 조합 | owner projection의 localized status |
| resulting Pipeline name | Shell이 raw result에서 직접 선택 | projection DTO의 `PipelineName` |
| 저장소 변이·fallback·refresh | UseCase와 Shell | 기존 owner와 Shell 유지 |

호출 경로는 다음과 같다.

`WPF command → PipelineLifecycle.cs → pipelineLifecycleUseCase → result`

`→ pipelineLifecycleProjectionOwner.Project → Shell StatusText/refresh`

실패 refresh에는 기존처럼 선택 option 이름을 사용하고, 성공 refresh에는
projection의 resulting Pipeline name을 사용해 이전 fallback 동작을 보존한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/OpenVisionRecipePipelineLifecycleProjectionOwner.cs`
  - lifecycle operation enum, projection owner, result/status DTO를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - projection owner 의존성을 생성하고 보유한다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs`
  - 다섯 lifecycle operation의 inline status/result 투영을 제거하고 owner를
    호출한다. guard, confirmation, pending-edit, refresh 순서는 유지했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--recipe-pipeline-lifecycle-projection-contract` Debug/Release 계약과
    사용법을 추가했다.
- `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_LIFECYCLE_PROJECTION_20260908.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
  `docs/LLM_DOCUMENT_INDEX.json`
  - 구조 증거, 검증 결과와 다음 우선순위를 기록한다.

## 구조 증거

- owner는 concrete/non-partial이며 `Window`, `UserControl`, Shell,
  `VisionPipelineStorage`를 참조하지 않는다.
- Shell adapter의 다섯 operation이 모두
  `pipelineLifecycleProjectionOwner.Project`를 호출한다.
- adapter의 `StatusText = result.Detail` 및 Activate/
  DuplicateFromSample localized status 조합은 제거됐다.
- adapter의 `TryLeaveSelectedStepEdit`, delete confirmation,
  `RefreshPipelineOptions`, `refreshAfterSwitch`,
  `UpdateSelectedRecipeSummary` 호출은 남아 있다.
- UseCase의 `VisionPipelineStorage` mutation과 fallback/name selection 호출은
  그대로 남아 있다.

정적 결과:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-lifecycle-projection-20260908\static-pipeline-lifecycle-projection-check.log`

## 검증

- x64 Debug/Release OpenVisionLab app과 `VisionRecipeRunnerSmoke` 빌드는
  경고 0개·오류 0개로 통과했다.
- `--recipe-pipeline-lifecycle-projection-contract`를 x64 Debug/Release
  산출물에서 실행해 Activate localized status, Duplicate/Rename/Delete
  storage detail, Delete fallback name, DuplicateFromSample success/failure
  prefix와 result name을 확인했다.
- 기존 Recipe workspace policy, workspace lifecycle projection, summary
  projection, Pipeline option projection 계약을 Debug/Release에서 모두
  재실행해 PASS했다.
- `OpenVisionLab.sln` Debug/Any CPU build는 경고 0개·오류 0개로 통과했고,
  `OpenVisionReadinessCheck`도 모든 readiness contract를 통과했다.
- `PipelineViewerScreenshotSmoke` Debug/Release rebuild는 기존
  `Program.cs:10722` CS8600 경고 1개와 오류 0개였다.
- Recipe Manager `wpf_shell_host_recipe_language_controls` smoke를 Debug/
  Release에서 실행해 모두 `check=OK`, `layout=0|text=0|internal=0`,
  `1600x900`, exit 0을 확인했다. 모니터-aware 재실행은 단일
  `\\.\DISPLAY2`(bounds `0,0,1920x1080`, work area `0,0,1920x1032`)를
  선택했고, 실제 창 rect `160,66-1760,966`가 선택 작업 영역 안에 있었다.
- Release 화면 증거:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-lifecycle-projection-20260908\smoke-Release-monitor-aware-foreground\wpf-recipe-manager\wpf_shell_host_recipe_language_controls.png`
- 모든 smoke process 종료, scoped `git diff --check`, trailing whitespace
  검사는 evidence에 기록한다. 기존 dirty worktree와 `Original`은 건드리지
  않았고 commit/push/merge/tag/release/deployment는 수행하지 않았다.

## 범위 경계

이번 완료는 Pipeline lifecycle 결과/status projection owner 분리에 한정한다.
Pipeline storage journal 자체, XML exchange/review, validation/Step Edit,
image·Layer lifetime, 전체 theme/layout/DPI(100/125/150/175/200%)와
multi-monitor matrix는 별도 작업이다.

## Refactor proof

- **Current owner:** `OpenVisionShellHostRecipeCommandSurface.PipelineLifecycle.cs`
  의 result-to-status inline 조합.
- **Intended owner:** `OpenVisionRecipePipelineLifecycleProjectionOwner`
  concrete module.
- **Dependency direction:** Shell adapter → existing Pipeline lifecycle
  UseCase와 projection owner → Shell status/refresh application.
- **State owner:** selected Pipeline, pending-edit, confirmation, active-state
  refresh, status binding은 Shell; projection owner는 전달받은 result snapshot만
  읽는다.
- **Observable contract:** localized Activate/sample status, storage detail,
  resulting name/fallback, Recipe/XML, Layer routing, explicit Preview/Run과
  command binding을 유지한다.

Status: Complete
Scope: OVL-07 Pipeline lifecycle result/status projection owner 분리.
Acceptance criteria: 다섯 lifecycle operation의 inline result/status 투영 제거,
Window-free concrete owner 호출, 기존 UseCase·UI side effect 유지,
Debug/Release focused contract와 Recipe Manager WPF smoke 통과.
Verification: x64 Debug/Release app·Runner build, focused projection contract,
existing regression contracts, solution/readiness, ScreenshotSmoke rebuild,
monitor-aware WPF smoke, static proof, documentation index, scoped diff check.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-lifecycle-projection-20260908`.
Boundary / next dependency: Pipeline exchange/review와 validation/Step Edit는
별도 범위이며, 다음 단일 작업은 OVL-07 Pipeline exchange/review result
projection owner 분리다.
