# OpenVisionLab OVL-07 Recipe Manager summary/library projection owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 Recipe Manager에서 선택 Recipe summary를 계산하고 library count
문구를 만드는 투영 책임을 한 단위로 분리했다.
Window-free concrete `OpenVisionRecipeManagerSummaryProjectionOwner`가
summary DTO, localized detail, stored XML validation report, pipeline preview
Step 목록과 library count 문구를 계산한다.

Recipe/XML 파일 읽기, active/preview Pipeline 선택, persistence failure 판정,
PropertyChanged, command state, Layer card callback, Recipe/Pipeline 변이와
명시적 Preview/Run 계약은 기존 Shell 경로에 남겼다. Recipe schema, XML
호환성, Layer routing과 XAML은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 선택 Recipe summary 조립 | `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`의 persistence 결과를 읽은 뒤 inline DTO/detail/report/preview 생성 | `OpenVisionRecipeManagerSummaryProjectionOwner.Project`가 명시적 snapshot을 받아 계산 |
| XML validation report | Shell이 builder를 직접 호출 | projection owner가 기존 Window-free builder를 재사용 |
| Pipeline preview Step 목록 | Shell private helper | projection owner 내부 helper |
| Recipe library count | Shell getter의 total/visible 분기 | `ProjectLibrarySummary` |
| 저장소 읽기·선택·UI 상태 | Shell | 변경 없음 |

호출 경로는 다음과 같다.

`WPF refresh/selection → Shell persistence reads and selected-state snapshot
→ OpenVisionRecipeManagerSummaryProjectionOwner → SelectedRecipeSummary`

`RecipeOptions/FilteredRecipeOptions → RecipeLibrarySummaryText getter
→ OpenVisionRecipeManagerSummaryProjectionOwner.ProjectLibrarySummary`

projection owner는 mutable UI state, Window, UserControl, Shell을 참조하지
않는다. Shell은 persistence, selection, notification, setter side effects,
commands, mutation과 explicit execution을 계속 소유한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/OpenVisionRecipeManagerSummaryProjectionOwner.cs`
  - summary/library projection owner와 explicit request snapshot을 추가했다.
  - 기존 stored XML validation report builder와 Step preview model을 재사용한다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - owner 의존성을 생성하고 `RecipeLibrarySummaryText`를 위임한다.
  - Recipe option 변경 시 기존 notification을 유지했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - persistence reads와 XML failure substitution을 유지하면서 owner request를
    전달한다.
  - inline summary/detail/report/preview 조립과 기존 private helper를 제거했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--recipe-manager-summary-projection-contract` 무창 계약과 usage를
    추가했다.
- `docs/reports/OPENVISIONLAB_OVL07_RECIPE_MANAGER_SUMMARY_PROJECTION_20260907.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
  `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner는 concrete sealed non-partial이며 `Window`, `UserControl`,
  `System.Windows`, Shell 참조가 없다.
- owner가 `Project`, `ProjectLibrarySummary`, request snapshot,
  localized detail, validation report와 preview Step 생성 책임을 가진다.
- Handlers에는 `GetVisionPipelineNames`, `LoadActivePipelineName`,
  `TryLoadFromFile`, `GetRecipeLastWriteTime`, persistence failure text
  substitution과 `PipelineVariantComparisonReport` notification이 남아 있고,
  summary DTO/detail/report/preview inline assembly는 없다.
- Shell의 `SelectedRecipeSummary` setter side effects,
  RecipeOptions/FilteredRecipeOptions notification과 command/UI 상태는 유지된다.

정적 결과:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-summary-20260907\static-recipe-manager-summary-projection-check.log`

## 검증

- 변경 전 Release Recipe Manager 화면 기준선은
  `wpf_shell_host_recipe_language_controls=OK|check=OK`였다.
  기준선 evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-summary-20260907\baseline-release`
- x64 Debug/Release OpenVisionLab app과 `VisionRecipeRunnerSmoke` 빌드는
  각각 경고 0개·오류 0개로 통과했다.
- `--recipe-manager-summary-projection-contract` Debug/Release가 PASS했다.
  valid summary의 identity/count/detail/report/Layer preview, invalid XML의 NG
  report와 empty preview, empty/full/filtered library count 형식을 확인했다.
- 기존 `--recipe-workspace-policy-contract`와
  `--recipe-workspace-lifecycle-projection-contract`도 Debug/Release에서
  재실행하여 모두 PASS했다.
- `PipelineViewerScreenshotSmoke` Debug/Release rebuild는 기존
  `Program.cs:10722` CS8600 경고 1개와 오류 0개였다.
- 변경 후 Recipe Manager WPF smoke Debug/Release가 모두 exit 0,
  `layout=0|text=0|internal=0`, size `1600x900`으로 통과했다.
  실행 토폴로지는 단일 `\\.\DISPLAY2`(bounds `0,0,1920x1080`, work area
  `0,0,1920x1032`)였고 실제 창 사각형은 `160,66--1760,966`이었다.
- Release 결과 PNG:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-summary-20260907\smoke-release-final\wpf-recipe-manager-release\wpf_shell_host_recipe_language_controls.png`
- 모든 smoke 프로세스 종료를 확인하고, 전체 지원 theme/layout/DPI/multi-monitor
  행렬은 실행하지 않았다.
- 문서 index 검증, scoped `git diff --check`, 이번 변경 파일 trailing
  whitespace 검사를 별도 evidence에 기록한다. 기존 dirty untracked 문서는
  범위 밖이라 수정하지 않았다.

## 범위 경계

이번 완료는 Recipe Manager summary/library projection owner 분리에 한정한다.
Pipeline option projection, Pipeline CRUD, validation/Step Edit, 이미지·Layer
lifetime, 전체 theme/layout/DPI(100/125/150/175/200%)/multi-monitor 행렬과
전체 solution/readiness/CI는 별도 작업이다. Original 저장소, commit, push,
merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** `OpenVisionShellHostRecipeCommandSurface.Handlers.cs`의
  summary inline assembly와 `OpenVisionShellHostRecipeCommandSurface.cs`의
  library count getter.
- **Intended owner:** `OpenVisionRecipeManagerSummaryProjectionOwner`
  concrete module.
- **Dependency direction:** Shell adapter → storage/selection snapshot →
  projection owner → Shell summary/library properties.
- **State owner:** selected Recipe, Pipeline options, persistence status,
  notifications, WPF side effects와 commands는 Shell; owner는 전달받은
  snapshot만 읽는다.
- **Observable contract:** Recipe/XML, active/preview Pipeline, Layer routing,
  summary/detail text, validation report, pipeline preview list, library count,
  explicit Preview/Run semantics와 XAML binding을 유지한다.

Status: Complete
Scope: OVL-07 Recipe Manager summary/library projection owner 분리.
Acceptance criteria: summary/detail/report/preview/library count inline assembly
제거, Window-free concrete owner 호출, persistence/selection/UI side effects
유지, Debug/Release contract와 Recipe Manager WPF smoke 통과.
Verification: baseline, Debug/Release app·Runner build, summary projection
contract, policy/lifecycle regression contracts, ScreenshotSmoke rebuild,
Debug/Release WPF smoke, static proof, documentation index, scoped diff check.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-summary-20260907`.
Boundary / next dependency: 다음 단일 작업은 OVL-07 Recipe Manager Pipeline
option projection owner 분리이며, 전체 UI matrix와 broader readiness는 별도다.

