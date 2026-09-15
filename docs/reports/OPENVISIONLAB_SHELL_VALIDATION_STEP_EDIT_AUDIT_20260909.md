# OpenVisionLab Shell validation/Step Edit 책임 감사 — 2026-09-09

Status: **Complete for the source-level residual validation/Step Edit audit**.

## 범위

현재 Astra 작업 목록의 P1 항목인 Shell validation 및 Step Edit 잔여 호출
경계를 다시 읽었다. 완료된 OVL-17 validation evidence owner와 OVL-18 Step
preview navigation owner, OVL-07 Step Edit loader/apply owner를 파일 크기만으로
다시 분리하지 않았다. 이번 감사에서 발견한 실제 동작 보완은 Recipe 로드 실패가
Shell의 선택 성공 메시지로 보이지 않도록 `SelectRecipeAsync`가 runtime 현재
Recipe를 확인하는 한 가지 guard를 추가한 것이다.

## 현재 소유권과 호출 경로

| 흐름 | 현재 owner | Shell의 역할 | 상태/파일 수명 |
| --- | --- | --- | --- |
| Validation Set 문서 | `OpenVisionRecipeValidationSetDocumentOwner` → `OpenVisionRecipeValidationSetStorage` | command 입력과 결과 문구 투영 | 문서 mutable state는 document owner, XML atomic save/실패 상태는 storage |
| Validation Set 선택 | `OpenVisionRecipeValidationSetSelectionOwner` | 선택 항목과 binding projection | 선택 상태는 owner, 실행 상태는 `OpenVisionRecipeExecutionSessionViewModel` |
| Acceptance/calibration evidence | `OpenVisionRecipeValidationEvidenceOwner` → `VisionPipelineStorage` | `ValidationSetAcceptanceText`/calibration binding | 읽기 결과는 immutable evidence, Pipeline XML은 기존 storage |
| Step 실패 preview 선택 | `OpenVisionRecipeStepPreviewNavigationOwner` | 선택 Step 변경, status, command invalidation | preview 목록은 Recipe summary, matching/index policy는 owner |
| Step XML load | `OpenVisionRecipeStepEditLoader` → `VisionPipelineStorage`/mapper | PropertyGrid session에 결과 전달 | loaded Pipeline/property는 apply 전 edit session이 보유 |
| Step XML apply | `OpenVisionRecipeStepEditApplyOwner` → `VisionPipelineStorage` | 성공/실패 projection과 Preview/Run 연결 | apply 호출별 pipeline snapshot/restore는 apply owner, dirty state는 session VM |

대표 경로는 다음과 같다.

```text
Shell validation command
  -> ValidationSetDocumentOwner / ValidationEvidenceOwner
  -> existing RecipeValidationSetStorage or VisionPipelineStorage
  -> Shell binding/status projection

failed Step selection
  -> StepPreviewNavigationOwner
  -> StepEditLoader
  -> StepEditSessionViewModel
  -> StepEditApplyOwner
  -> StepEditApplyProjectionOwner
  -> Shell status and existing Preview/Run flow
```

`RefreshPipelineOptions`와 `UpdateSelectedRecipeSummary`의
`VisionPipelineStorage` 호출은 저장/적용 정책을 다시 구현하는 코드가 아니라
선택 목록과 Recipe Manager summary를 갱신하는 조합 계층의 읽기다. Validation
Set XML mutation, acceptance policy, Step XML apply/restore는 각각 위 owner에
남아 있으며 같은 책임의 두 구현은 확인되지 않았다.

## 실제 변경

변경 파일: `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`.

기존에는 `switchRecipe(normalized)`가 Recipe 파일 로드에 실패해도 callback이
현재 이름을 바꾸지 않은 사실을 Shell이 확인하지 않고 `Selected: ...`를 표시할
수 있었다. 현재는 callback 뒤 `currentRecipeProvider()`가 요청 이름과 같은지
확인하고, 다르면 `Recipe load failed: ...`를 표시하고 selection refresh와
성공 후속 경로를 중단한다. RecipeState의 atomic load/기존 identity 보존과
맞물려 실패를 성공으로 표시하지 않는다. WPF binding 이름, Preview/Run 순서,
validation/Step Edit owner는 변경하지 않았다.

## 구조 감사 결과

- Validation Set 문서/선택/evidence/실행은 서로 다른 변경 이유와 상태 수명을
  가진 기존 concrete owner를 통해 호출된다.
- Step preview matching, XML load, apply/restore, dirty session, message
  projection은 각각 한 owner이며 Shell은 orchestration과 binding projection만
  남긴다.
- 새 interface, factory, manager, partial은 추가하지 않았다.
- `RecipeState` load failure가 복구될 때 Shell success message를 차단하는
  호출 경로를 source search로 확인했다.

## 검증

이번 작업의 Debug/Release solution, 제품, `VisionRecipeRunnerSmoke` build는
0 warning/0 error다. Recipe save/load recovery, Tool XML rollback, Recipe
storage path, Step Edit apply, Pipeline Review execution/stale callback,
ImageSpace snapshot focused contract 9개가 Debug/Release 각각 통과했다. 별도
WPF EXE를 새로 띄우는 UI 행렬은 실행하지 않았다.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\architecture-reliability-20260909`.

## 완료 증거

- **Current owner:** Shell orchestration plus the named validation/Step Edit owners above.
- **Intended owner:** Existing owners remain canonical; only Recipe switch failure
  result is checked at the Shell callback boundary.
- **Call path:** The two text paths and source search above.
- **Mutable-state owner:** Validation document/selection, execution session, Step
  edit session, and RecipeState keep their existing owners.
- **Public/binding contract:** Existing command/property names and explicit
  Preview/Run behavior remain unchanged.
- **Reading order:** `README Start Here -> CODEBASE_STRUCTURE.md 1.1 ->`
  `OpenVisionShellHostRecipeCommandSurface -> named validation/Step Edit owner`
  files -> focused contract.

Status: Complete
Scope: residual Shell validation/Step Edit source audit and Recipe-switch failure projection guard.
Acceptance criteria: no duplicate owner admitted; failed Recipe load cannot be reported as successful selection; existing focused contracts and builds pass.
Verification: Debug/Release builds, focused contracts, source ownership search, and final diff check.
Boundary / next dependency: actual WPF theme/layout/DPI/pointer/keyboard runtime and long-run native-resource qualification remain environment-bound.
