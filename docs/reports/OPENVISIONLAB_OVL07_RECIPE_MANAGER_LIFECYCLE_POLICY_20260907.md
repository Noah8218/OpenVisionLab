# OpenVisionLab OVL-07 Recipe Manager lifecycle 정책 owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 Recipe Manager lifecycle/CRUD 책임 분리의 한 단위로,
`OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`에 남아 있던
Recipe 이름 유효성·source membership·rename 충돌·마지막 Recipe 삭제 보호
정책을 기존 Window-free concrete `OpenVisionRecipeWorkspaceUseCase`로
이동했다.

Recipe workspace 생성·복제·이름 변경·삭제의 저장소 변이는 기존 use case와
`RecipeWorkspaceService` 경로를 유지한다. Shell은 PropertyGrid pending-edit
전환, switch/refresh, status, fallback 선택과 명령 바인딩을 계속 소유한다.
Recipe/XML schema, 경로 규칙, active Pipeline, Layer routing, Preview/Run
계약과 XAML은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 이름 생성 가능 여부 | Shell이 `IsValidRecipeName` 직접 호출 | `OpenVisionRecipeWorkspaceUseCase.CanCreate` |
| 복제 가능 여부 | Shell의 선택/유효성 분기 | `CanDuplicate`가 source membership·요청 이름 정책 소유 |
| 이름 변경 가능 여부 | Shell의 유효성·동일 이름·충돌 분기 | `CanRename`이 lifecycle 정책 소유 |
| 삭제 가능 여부 | Shell의 선택·Recipe 수 분기 | `CanDelete`가 마지막 Recipe 보호 정책 소유 |
| 저장소 변이·전환 | 기존 use case와 Shell | 변경 없음 |

호출 경로는 다음과 같다.

`WPF CanExecute → Shell adapter → OpenVisionRecipeWorkspaceUseCase.Can*`

`WPF Execute → Shell pending-edit guard → existing workspace use case mutation`
` → Shell switch/refresh/status`

새 정책 메서드는 WPF 객체를 참조하지 않고 Shell이 제공한
`RecipeOptions` snapshot만 읽는다. 정책 owner는 mutable UI state를 보유하지
않으며, 실제 Recipe workspace 저장은 기존 storage service를 통과한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/OpenVisionRecipeWorkspaceUseCase.cs`
  - create/duplicate/rename/delete admission policy와 공용 Recipe membership
    판단을 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.RecipeWorkspace.cs`
  - 네 개의 inline lifecycle 정책 분기를 제거하고 use case 호출로 위임했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--recipe-workspace-policy-contract` Window-free 계약과 사용법을 추가했다.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- 정적 검사 결과는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-lifecycle-20260907\static-recipe-workspace-policy-check.log`
  에 `Result=PASS`로 기록했다.
- Shell recipe workspace 파일에는
  `RecipeWorkspaceService.IsValidRecipeName` 및 Recipe 이름 충돌/수량 정책이
  남아 있지 않고 `recipeWorkspaceUseCase.CanCreate/CanDuplicate/CanRename/CanDelete`
  호출만 남는다.
- 생성·복제·이름 변경·삭제 mutation, pending-edit guard, `switchRecipe`,
  `RefreshOptions`, `refreshAfterSwitch`는 Shell/기존 use case 경로에 남아
  있다.
- `OpenVisionRecipeWorkspaceUseCase`는 concrete non-partial이며 WPF/View/Shell
  의존성이 없다.

## 검증

- 변경 전 Release 기준선 Recipe Manager CRUD smoke:
  `wpf_shell_host_recipe_language_controls=OK|check=OK`, 단일 모니터
  `\\.\DISPLAY2`, 실제 창 `160,66..1760,966`.
- x64 Debug/Release OpenVisionLab app build: 각각 0 warnings / 0 errors.
- x64 Debug/Release `VisionRecipeRunnerSmoke` build: 각각 0 warnings / 0 errors.
- `--recipe-workspace-policy-contract` Debug/Release: PASS.
  - blank/valid/invalid create name policy
  - duplicate source membership와 요청 이름 정책
  - rename identity/conflict 정책
  - 마지막 Recipe 삭제 보호
- `PipelineViewerScreenshotSmoke` Debug/Release rebuild: 기존
  `Program.cs:10722` CS8600 nullable warning 1개, 0 errors.
- 변경 후 Recipe Manager CRUD WPF smoke Debug/Release:
  `wpf_shell_host_recipe_language_controls=OK|check=OK`, exit 0,
  `layout=0|text=0|internal=0`, size `1600x900`.
  현재 토폴로지는 단일 `\\.\DISPLAY2`(work area `0,0,1920x1032`)였고,
  실제 창 사각형은 `160,66..1760,966`이었다. 최신 PNG와 stdout/stderr는
  evidence directory에 보관했다.
- `git diff --check`는 tracked 변경에 대해 exit 0으로 통과했고, 이번 변경
  파일의 trailing whitespace도 통과했다. 기존 dirty untracked 문서의
  whitespace는 범위 밖이라 수정하지 않았다.
- 문서 index 검증은 `DocumentationIndex=PASS IndexedPaths=171 Routes=13
  RootRedirects=102`로 통과했다.

## 범위 경계

이번 완료는 lifecycle admission policy owner 분리에 한정한다. Recipe Manager
전체 library projection/summary, lifecycle 실패 메시지 projection, Pipeline
CRUD, validation/Step edit, 이미지·Layer lifetime, 전체
theme/layout/DPI(100/125/150/175/200%)/multi-monitor 행렬은 이번 실행에서
검증하지 않았다. 전체 solution/readiness/CI도 실행하지 않았다.
Original 저장소, commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** Shell recipe workspace 파일의 이름 유효성·membership·충돌·마지막 삭제 정책.
- **Intended owner:** `OpenVisionRecipeWorkspaceUseCase` concrete module.
- **Dependency direction:** Shell adapter → workspace use case → existing `RecipeWorkspaceService`.
- **State owner:** `RecipeOptions`, selected/edit text, pending transition, switch/refresh state는 Shell; 정책 owner는 snapshot을 읽고 결과만 반환한다.
- **Observable contract:** 기존 localized status, command binding 이름, workspace CRUD, Recipe/XML, active Pipeline, Layer routing, explicit Preview/Run semantics를 유지한다.

Status: Complete
Scope: OVL-07 Recipe Manager lifecycle CRUD admission policy owner 분리.
Acceptance criteria: Shell inline policy 제거, concrete Window-free use case 호출, 기존 mutation/전환 흐름 유지, Debug/Release policy contract와 Recipe Manager WPF CRUD smoke 통과.
Verification: 기준선 Release smoke, Debug/Release app·Runner build, policy contract, ScreenshotSmoke rebuild, Debug/Release WPF smoke, static structure proof.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-recipe-manager-lifecycle-20260907`.
Boundary / next dependency: 전체 Recipe Manager lifecycle projection 및 나머지 pipeline/validation 경계는 별도 작업이며, 다음 단일 작업은 그 중 Recipe Manager lifecycle 결과/status projection owner 분리다.
