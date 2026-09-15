# OpenVisionLab OVL-07 Recipe Manager Pipeline option projection owner 분리

작성일: 2026-09-08 KST

## 의미와 범위

이번 실행은 Recipe Manager의 Pipeline 목록 투영 책임을 Shell에서 분리했다.
Window-free concrete `OpenVisionRecipePipelineOptionProjectionOwner`가
명시적 inventory/selection snapshot을 받아 Pipeline option 생성·정렬·선택
fallback을 계산하고, 검색 결과와 목록 count 문구도 투영한다.

Recipe workspace inventory 조회, active Pipeline 복구, 기본 Pipeline 생성
fallback, `PipelineOptions`/`FilteredPipelineOptions` PropertyChanged,
pending-edit guard, `PipelineEditName`, summary/validation/command refresh와
Pipeline CRUD/Preview/Run 흐름은 Shell에 남겼다. 기존
`OpenVisionRecipePipelineOption.Create`와 Recipe/XML inventory 계약을
재사용했으며 XML schema, Layer routing, explicit Preview/Run 동작은
변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| Pipeline option 생성·정렬 | `RefreshPipelineOptions` inline LINQ | `OpenVisionRecipePipelineOptionProjectionOwner.Project` |
| 선택 fallback | preferred/previous/active를 Shell에서 직접 계산 | projection owner가 request snapshot으로 계산 |
| 검색 | `ApplyPipelineFilter`의 Shell inline name/display/detail 검색 | owner `Filter` |
| Pipeline 목록 count | Shell `PipelineListSummaryText`의 inline 분기 | owner `ProjectListSummary` |
| 저장소 읽기·복구·UI side effects | Shell | 변경 없음 |

호출 경로는 다음과 같다.

`Shell inventory/recovery → PipelineOptionProjectionRequest
→ OpenVisionRecipePipelineOptionProjectionOwner.Project
→ Shell PipelineOptions/SelectedPipelineOption`

`Shell PipelineFilterText/PipelineOptions
→ owner.Filter → Shell FilteredPipelineOptions`

`PipelineListSummaryText → owner.ProjectListSummary`

owner는 `Window`, `UserControl`, `System.Windows`, Shell 참조와 mutable
UI state를 가지지 않는다. 저장된 XML의 개별 option 상태는 기존
`OpenVisionRecipePipelineOption.Create`와 validator를 통해 계산된다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/OpenVisionRecipePipelineOptionProjectionOwner.cs`
  - option 목록, active-first/name ordering, preferred/previous/active fallback,
    filter와 count text를 소유하는 owner/request/result를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - owner를 생성하고 `PipelineListSummaryText`를 위임한다.
  - 기존 Pipeline option property notification과 filter binding을 유지했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - inventory/storage recovery를 유지하면서 owner request를 전달한다.
  - inline option creation/order/selection/filter 계산을 제거했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--recipe-manager-pipeline-option-projection-contract` 무창 계약과
    usage를 추가했다.
- `tools/OpenVisionReadinessCheck/Program.cs`
  - summary projection owner로 이어진 stored-pipeline XML validation 위임을
    readiness 계약이 확인하도록 기존 직접 호출 토큰 검사를 갱신했다.
- `docs/reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_PIPELINE_OPTION_PROJECTION_20260908.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
  `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner는 concrete sealed non-partial이며 WPF/Shell 참조가 없다.
- Handlers에는 `LoadActivePipelineName`, `GetVisionPipelineNames`,
  recovery `VisionPipelineStorage.Load`와 persistence-state fallback,
  owner request, `PipelineOptions` assignment, summary/validation/command
  refresh가 남아 있다.
- Handlers의 `OpenVisionRecipePipelineOption.Create`, active-first
  `OrderBy`, display/detail filter와 Filtered option 직접 조립은 제거됐다.
- Shell의 Pipeline option setters는 `PipelineListSummaryText`
  PropertyChanged와 `ApplyPipelineFilter` 호출을 유지하고,
  `PipelineFilterText` binding도 유지한다.

정적 결과:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-pipeline-option-20260908\static-recipe-manager-pipeline-option-projection-check.log`

## 검증

- 변경 전 기준선은 이전 summary owner slice에서 저장한 Release Recipe
  Manager smoke `wpf_shell_host_recipe_language_controls=OK|check=OK`이며,
  기준선 경로는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-summary-20260907\baseline-release`다.
- x64 Debug/Release OpenVisionLab app과 `VisionRecipeRunnerSmoke` 빌드는
  경고 0개·오류 0개로 통과했다.
- `OpenVisionLab.sln` Debug/Any CPU final build가 경고 0개·오류 0개로 통과했고,
  `OpenVisionReadinessCheck`가 PASS했다. 최초 readiness 실행은 이미 완료된
  summary owner 분리를 반영하지 않은 직접 builder 호출 토큰 때문에 실패했으며,
  readiness 계약을 현재 owner 위임 경계로 갱신한 뒤 다시 실행해 PASS했다.
- 새 `--recipe-manager-pipeline-option-projection-contract`가 Debug/Release
  모두 PASS했다. 격리된 D: data root에서 실제 저장 XML option을 읽어 active-first/
  name ordering, valid XML, preferred/previous/active fallback, trimmed
  case-insensitive name/status filtering, empty/full/filtered count text를
  확인했고 reserved smoke Recipe cleanup도 확인했다.
- 기존 summary projection, workspace policy, lifecycle projection 계약도
  Debug/Release에서 모두 PASS했다.
- `PipelineViewerScreenshotSmoke` Debug/Release rebuild는 기존
  `Program.cs:10722` CS8600 경고 1개와 오류 0개였다.
- 변경 후 Recipe Manager WPF smoke Debug/Release가 모두 exit 0,
  `check=OK`, `layout=0|text=0|internal=0`, size `1600x900`으로
  통과했다. 동적 모니터 결과는 단일
  `\\.\DISPLAY2`(bounds `0,0,1920x1080`, work area
  `0,0,1920x1032`)였고 실제 창은 `160,66--1760,966`이었다.
- Release PNG:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-pipeline-option-20260908\smoke-release-final\wpf-recipe-manager-release\wpf_shell_host_recipe_language_controls.png`
- 모든 smoke process 종료를 확인했다. 전체 theme/layout/DPI(100/125/150/175/200%)/
  multi-monitor matrix와 별도 interactive filter UI path는 이번 실행에서
  완료하지 않았다.
- 문서 index 검증과 scoped diff/trailing-whitespace 검사는 같은 evidence
  root에 기록한다. 기존 dirty worktree와 out-of-scope 파일은 수정하지
  않았다.

## 범위 경계

이번 완료는 Recipe Manager Pipeline option projection owner 분리에 한정한다.
Pipeline lifecycle CRUD result/status, Pipeline exchange/review, validation/
Step Edit, 이미지·Layer lifetime, 전체 UI matrix와 broader solution/readiness/
CI는 별도 작업이다. Original 저장소, commit, push, merge, tag, release,
배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`의
  Refresh/Filter inline projection과
  `OpenVisionShellHostRecipeCommandSurface.cs`의 Pipeline count getter.
- **Intended owner:** `OpenVisionRecipePipelineOptionProjectionOwner`
  concrete module.
- **Dependency direction:** Shell adapter → storage/inventory recovery and
  existing option model factory → projection owner → Shell-bound option
  properties.
- **State owner:** recipe/pipeline storage recovery, selected option reference,
  pending-edit guard, edit name, notifications, summary/validation/command
  refresh와 CRUD/Preview/Run은 Shell; owner는 request snapshot과 existing
  option model을 통해 projection만 계산한다.
- **Observable contract:** Pipeline inventory exclusion rules, Recipe/XML,
  active-first ordering, deterministic selection fallback, filtering/count
  display, Layer routing, explicit Preview/Run, XAML binding과 existing option
  status semantics를 유지한다.

Status: Complete
Scope: OVL-07 Recipe Manager Pipeline option projection owner 분리.
Acceptance criteria: option 생성/정렬/선택/filter/count inline 책임 제거,
Window-free concrete owner 호출, storage recovery와 Shell UI side effects
유지, Debug/Release contract와 Recipe Manager WPF smoke 통과.
Verification: pre-change baseline, Debug/Release app·Runner build,
Debug/Any CPU solution build, readiness check, new option projection contract,
summary/policy/lifecycle regression contracts, ScreenshotSmoke rebuild,
Debug/Release WPF smoke, static proof, documentation index, scoped diff check.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-pipeline-option-20260908`.
Boundary / next dependency: 다음 단일 작업은 OVL-07 Pipeline lifecycle
result/status projection owner 분리이며, 전체 UI matrix와 broader readiness는
별도다.

