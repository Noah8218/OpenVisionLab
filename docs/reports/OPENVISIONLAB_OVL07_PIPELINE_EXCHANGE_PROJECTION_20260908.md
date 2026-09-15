# OpenVisionLab OVL-07 Pipeline exchange/review result projection owner 분리 — 2026-09-08

## 의미와 범위

이번 실행은 Pipeline XML import/export와 review-bundle export의
UseCase 결과를 Shell `StatusText`로 투영하는 책임을 분리했다.
Window-free concrete `OpenVisionRecipePipelineExchangeProjectionOwner`가
세 exchange operation의 성공/실패, Pipeline 이름, 목적지 파일명과
localized status를 계산한다.

파일 선택, 선택 레시피 guard, pending Step edit 전환, 기존
`OpenVisionRecipePipelineExchangeUseCase`, review reference 수집, refresh
순서, review-bundle dry-run과 `LLM XML` review state는 Shell/기존 LLM
workflow에 남겼다. Recipe/XML schema, Layer routing, explicit Preview/Run
계약은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| XML import 결과 status | `PipelineExchange.cs` inline formatting | `OpenVisionRecipePipelineExchangeProjectionOwner.Project(Import, result)` |
| XML export 결과 status | Shell이 destination filename과 localized text 조립 | owner가 result detail에서 filename과 status 투영 |
| review-bundle export 결과 status | Shell이 success/failure prefix와 filename 조립 | owner가 success/failure status 투영 |
| storage/XML/bundle 변이 | Shell → existing UseCase | 변경 없음 |
| review-bundle dry-run | LLM workflow가 inspection state와 review tab을 갱신 | 변경 없음 |

호출 경로는 다음과 같다.

`Shell guard/file/reference workflow → PipelineExchangeUseCase →
OpenVisionRecipePipelineExchangeProjectionOwner.Project → Shell StatusText/
PipelineName refresh`

owner는 `Window`, `UserControl`, WPF type, storage service, mutable Shell
state를 가지지 않는다. UseCase는 계속 XML load/save, unique name, active
pointer, review-bundle 생성과 serialization을 소유한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipePipelineExchangeProjectionOwner.cs`
  - exchange operation/result를 받아 status와 resulting Pipeline name을
    반환하는 concrete owner/projection을 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - owner를 생성해 Shell 의존성으로 보유한다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs`
  - Import/Export/Review bundle의 inline 결과 status 조립을 제거하고
    owner를 호출한다. guard, pending-edit, reference collection, refresh,
    dry-run route는 유지했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--recipe-pipeline-exchange-projection-contract` 무창 계약과 usage를
    추가했다.
- `docs/reports/OPENVISIONLAB_OVL07_PIPELINE_EXCHANGE_PROJECTION_20260908.md`,
  `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`,
  `docs/LLM_DOCUMENT_INDEX.json`
  - 완료 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- owner는 `internal sealed` concrete non-partial이며 WPF/Shell/storage
  참조가 없다.
- `PipelineExchange.cs`의 Import/Export/Review bundle 세 경로가 모두
  `pipelineExchangeProjectionOwner.Project(...)`를 호출한다.
- 기존 localized success/failure 문자열과 `StatusText = result.Detail`
  inline 조립은 adapter에서 제거됐다.
- Shell에는 `CanUseSelectedRecipe`, `TryLeaveSelectedStepEdit`,
  `BuildRecipeReviewReferences`, `LoadReviewBundleForDryRun`,
  `RefreshPipelineOptions`, `RefreshOptions`, `refreshAfterSwitch`가 남아
  workflow와 side effect 소유권을 유지한다.
- `OpenVisionRecipePipelineExchangeUseCase`에는 storage/XML/bundle
  ownership token이 그대로 남아 있다.

정적 결과:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-exchange-projection-20260908\static-pipeline-exchange-projection-check.log`

## 검증

- x64 Debug/Release OpenVisionLab app과 `VisionRecipeRunnerSmoke` 빌드는
  경고 0개·오류 0개로 통과했다.
- 새 `--recipe-pipeline-exchange-projection-contract`는 x64 Debug/Release
  산출물에서 PASS했다. Import localized status, Export destination
  filename, review-bundle destination filename, storage detail과 failure
  prefix를 확인했다.
- 기존 workspace policy/lifecycle, Recipe Manager summary/Pipeline option,
  Pipeline lifecycle projection 계약은 Debug/Release에서 모두 PASS했다.
- `OpenVisionLab.sln` Debug/Any CPU build는 경고 0개·오류 0개였고,
  `OpenVisionReadinessCheck`도 PASS했다.
- `PipelineViewerScreenshotSmoke` Debug/Release rebuild는 기존
  `Program.cs:10722` CS8600 경고 1개와 오류 0개였다.
- 최신 Release 소스의 `wpf_shell_host_recipe_review_bundle_import`는
  `check=OK`, `layout=0|text=0|internal=0`, 1600x900으로 PASS했다. 이
  경로는 실제 review-bundle export setup, tamper rejection, relocation
  dry-run import와 Import/Preview/Run 무실행을 함께 확인한다. Debug도
  같은 target이 PASS했다.
- 기존 `wpf_shell_host_recipe_review_bundle` 단독 target은 summary 화면에서
  Advanced review 전환 없이 `HostRecipeExportXmlButton`을 찾도록 되어 있어
  실패했다. 이는 현재 UI 상태와 맞지 않는 기존 smoke 사전조건이며,
  이번 owner 변경의 실행 실패로 분류하지 않고 별도 test-maintenance
  위험으로 남긴다. 기대값 완화나 테스트 삭제는 하지 않았다.
- 문서 index JSON/`TestDocumentationIndex.ps1`, static proof, scoped
  `git diff --check`, 새 파일 trailing-whitespace 검사가 PASS했다.
- 모든 target process 종료를 확인했다. 기존 dirty worktree와
  `C:\Git\2D\Original`은 건드리지 않았고 commit/push/merge/tag/release/
  deployment는 수행하지 않았다.

증거 root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-exchange-projection-20260908`

## 범위 경계

이번 완료는 exchange UseCase 결과의 Shell status projection owner 분리에
한정한다. Review-bundle dry-run의 loaded inspection state, LLM XML validation,
Pipeline Review 실행/result state, validation/Step Edit, 이미지·Layer
lifetime, 전체 theme/layout/DPI(100/125/150/175/200%)와 multi-monitor
matrix는 별도 작업이다.

## Refactor proof

- **Current owner:** `OpenVisionShellHostRecipeCommandSurface.PipelineExchange.cs`
  의 Import/Export/Review bundle 결과 분기와 localized status formatting.
- **Intended owner:** `OpenVisionRecipePipelineExchangeProjectionOwner`
  concrete module.
- **Dependency direction:** Shell UI adapter → existing Exchange UseCase →
  exchange result → projection owner → Shell-bound status/name properties.
- **State owner:** selected recipe, file path, pending edit, reference list,
  status binding, refresh와 review-only state는 Shell/LLM workflow; XML,
  storage와 bundle mutation은 UseCase; owner는 immutable-like projection
  result만 계산한다.
- **Observable contract:** XML import/export, review-bundle package naming,
  localized status text, failure detail/prefix, Recipe/XML, Layer routing,
  explicit Preview/Run과 existing XAML binding을 유지한다.

Status: Complete
Scope: OVL-07 Pipeline exchange/review result projection owner 분리.
Acceptance criteria: 세 exchange operation의 inline result/status 투영 제거,
Window-free concrete owner 호출, 기존 UseCase·Shell workflow·dry-run 유지,
Debug/Release contract와 review-bundle import smoke 통과.
Verification: x64 Debug/Release app·Runner·ScreenshotSmoke build, focused
projection contract, existing regression contracts, solution/readiness,
review-bundle import smoke, static proof, documentation index, scoped diff
check.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-pipeline-exchange-projection-20260908`.
Boundary / next dependency: review-bundle dry-run state와 Pipeline Review
execution/result state는 별도 범위이며, 다음 단일 작업은 OVL-07 Review
bundle dry-run result/status projection owner 분리다.
