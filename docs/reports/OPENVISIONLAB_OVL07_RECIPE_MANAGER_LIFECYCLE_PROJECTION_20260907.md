# OpenVisionLab OVL-07 Recipe Manager lifecycle 결과/status projection owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 Recipe Manager lifecycle 결과를 Shell 상태 문자열로 바꾸는
책임을 한 단위로 분리했다. `Create`, `Duplicate`, `Rename`, `Delete`의
성공·실패 status 문구와 결과 이름 투영은 새 Window-free concrete
`OpenVisionRecipeWorkspaceLifecycleProjectionOwner`가 담당한다.

Recipe workspace 생성·복제·이름 변경·삭제의 저장소 변이, pending-edit 전환,
active Recipe 전환, 옵션 refresh, fallback 선택, 명령 바인딩과 UI 상태는
기존 Shell/use case 경로에 남겼다. Recipe/XML schema, 경로 규칙, active
Pipeline, Layer routing, explicit Preview/Run 계약과 XAML은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| lifecycle 결과 status | Shell CRUD 메서드의 inline localized 문자열 | `OpenVisionRecipeWorkspaceLifecycleProjectionOwner.Project` |
| 성공 결과 이름 | 각 Shell 메서드가 직접 선택 | projection DTO의 `RecipeName` |
| Delete 표시 이름 | fallback 결과 이름과 삭제 이름이 Shell에 혼재 | DTO 결과 이름은 fallback, status 표시 이름은 `deletedName` snapshot |
| 실패 status | Duplicate/Rename/Delete Shell inline 문자열 | projection owner가 operation별 localized 문자열 생성 |
| Create 실패 | 기존처럼 조용히 종료 | 빈 status를 유지 |
| 저장소 변이·전환·refresh | 기존 use case와 Shell | 변경 없음 |

호출 경로는 다음과 같다.

`WPF Execute → Shell pending-edit guard → OpenVisionRecipeWorkspaceUseCase mutation`

`→ OpenVisionRecipeWorkspaceLifecycleProjectionOwner.Project → Shell status`

projection owner는 mutable UI state, Window, UserControl, storage service를
참조하지 않는다. Shell은 mutation 결과와 Delete 전에 보관한 삭제 이름만
전달하고, switch/refresh 및 status property 갱신을 계속 소유한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/OpenVisionRecipeWorkspaceLifecycleProjectionOwner.cs`
  - lifecycle operation enum, 결과/status DTO, localized success/failure
    projection을 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - projection owner 의존성을 생성하고 보유한다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`
  - Create/Duplicate/Rename/Delete의 inline lifecycle status 생성을 제거하고
    owner를 호출한다. 기존 mutation, pending-edit, switch, refresh 순서는
    유지했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--recipe-workspace-lifecycle-projection-contract` 무창 계약과 사용법을
    추가했다.
- `docs/LLM_DOCUMENT_INDEX.json`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
  - 현재 구조 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- static proof에서 owner concrete/non-partial, Window-free, operation/DTO와
  localized success/failure 문구 소유를 확인했다.
- Shell에서 기존 Create/Duplicate/Rename/Delete mutation 호출과
  `TryLeaveSelectedStepEdit`, `BeginRecipeSwitchingState`, `switchRecipe`,
  `RefreshAfterRecipeSwitchIfNeeded`, `RefreshOptions` 호출이 남아 있음을
  확인했다.
- Shell의 lifecycle 성공 status literal은 제거되고 네 operation 모두
  `workspaceLifecycleProjectionOwner.Project`로 위임된다.
- Delete projection은 삭제 전 이름을 별도 인자로 받아 status에 사용하고,
  use case가 반환한 fallback RecipeName은 DTO에 보존한다.

정적 결과: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-projection-20260907\static-recipe-manager-lifecycle-projection-check.log`

## 검증

- 변경 전 Release Recipe Manager CRUD smoke 기준선은
  `wpf_shell_host_recipe_language_controls=OK|check=OK`였다.
- x64 Debug/Release OpenVisionLab app과 `VisionRecipeRunnerSmoke` 빌드는
  각각 오류 0개로 통과했다.
- `--recipe-workspace-lifecycle-projection-contract` Debug/Release가
  PASS했다. Create/Duplicate/Rename/Delete 성공 status, Delete 삭제 이름,
  Duplicate/Rename/Delete 실패 status, Create 조용한 실패 동작을 확인했다.
- 기존 `--recipe-workspace-policy-contract`도 Debug/Release에서 재실행하여
  PASS했다.
- `PipelineViewerScreenshotSmoke` Debug/Release rebuild는 기존
  `Program.cs:10722` CS8600 경고 1개와 오류 0개였다.
- 변경 후 Recipe Manager WPF CRUD smoke Debug/Release가 모두
  `wpf_shell_host_recipe_language_controls=OK|check=OK`, exit 0,
  `layout=0|text=0|internal=0`, size `1600x900`으로 통과했다.
  실행 시 토폴로지는 단일 `\\.\DISPLAY2`(bounds `0,0,1920x1080`, work area
  `0,0,1920x1032`)였고 실제 창 사각형은 `160,66--1760,966`이었다.
- Release 첫 시도는 기존 clipboard 기반 XML paste 단계의 일시적인
  `CLIPBRD_E_CANT_OPEN (0x800401D0)`으로 실패했지만 프로세스 종료를 확인한
  뒤 새 data/evidence root에서 재시도하여 같은 target이 PASS했다.
- Release PNG는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-projection-20260907\smoke-release-retry\wpf-recipe-manager-release\wpf_shell_host_recipe_language_controls.png`
  에 보관했다.
- `git diff --check`와 이번 변경 파일 trailing whitespace 검사는 별도
  evidence에 PASS로 기록했다. 기존 dirty untracked 문서는 범위 밖이라
  수정하지 않았다.
- 문서 index 검증은 `DocumentationIndex=PASS`로 통과했다.

## 범위 경계

이번 완료는 lifecycle 결과/status projection owner 분리에 한정한다. Recipe
Manager summary/library projection, Pipeline CRUD, validation/Step edit,
이미지·Layer lifetime, 전체 theme/layout/DPI(100/125/150/175/200%)/
multi-monitor 행렬과 전체 solution/readiness/CI는 검증하지 않았다.
Original 저장소, commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** `OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`
  의 lifecycle 성공·실패 status 문자열 조합.
- **Intended owner:** `OpenVisionRecipeWorkspaceLifecycleProjectionOwner`
  concrete module.
- **Dependency direction:** Shell adapter → existing workspace use case
  (mutation) 및 projection owner (status DTO) → Shell property update.
- **State owner:** selected Recipe, pending-edit, switching, refresh, fallback와
  WPF status property는 Shell; projection owner는 전달받은 결과 snapshot만
  읽는다.
- **Observable contract:** command binding, localized status, Recipe workspace
  CRUD, Recipe/XML, active Pipeline, Layer routing, explicit Preview/Run
  semantics를 유지한다.

Status: Complete
Scope: OVL-07 Recipe Manager lifecycle 결과/status projection owner 분리.
Acceptance criteria: Shell inline lifecycle status 제거, Window-free concrete
owner 호출, 기존 mutation/전환/refresh 흐름 유지, Debug/Release projection
contract와 Recipe Manager WPF smoke 통과.
Verification: 변경 전 Release 기준선, Debug/Release app·Runner build,
projection contract, ScreenshotSmoke rebuild, Debug/Release WPF smoke, static
proof, documentation index, scoped diff check.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-projection-20260907`.
Boundary / next dependency: 전체 Recipe Manager summary/library projection과
다른 Pipeline/validation 경계는 별도 작업이며, 다음 단일 작업은 OVL-07 Recipe
Manager summary/library projection owner 분리다.
