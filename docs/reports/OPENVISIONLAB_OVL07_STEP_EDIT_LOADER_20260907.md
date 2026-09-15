# OpenVisionLab OVL-07 Step Edit selected-Step loader 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 OVL-07에서 독립적으로 검증 가능한 한 가지 작업으로,
선택된 Step의 pipeline XML 로드, 선택 Step 재해결, PropertyGrid 편집 객체
투영 책임을 `OpenVisionShellHostRecipeCommandSurface`에서
`OpenVisionRecipeStepEditLoader`로 이동했다.

Shell은 기존 XAML binding/command façade와 Step Edit session 상태,
PropertyGrid 편집 확정, XML 반영·왕복 검증·실패 복원, 상태 메시지,
Preview/Run 명시 실행 계약을 계속 담당한다. Recipe/XML schema·경로,
PropertyGrid mapper 계약, 입력/출력 Layer 규칙은 변경하지 않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| pipeline 이름 결정과 XML 로드 | Shell handler가 active pipeline fallback과 Storage를 직접 호출 | `OpenVisionRecipeStepEditLoader.Load`가 소유 |
| 선택 Step 재해결 | Shell의 `TryLoadSelectedPipelineStep` | loader의 index 우선, Name/ToolType/OutputLayer fallback |
| PropertyGrid 객체 생성 | Shell handler가 `VisionPipelineStepPropertyMapper.CreateProperty`를 직접 호출 | loader가 기존 mapper와 context를 호출 |
| 편집 session 상태 | Shell의 기존 `OpenVisionRecipeStepEditSessionViewModel` | 변경 없음. loader 결과를 session에 로드 |
| XML 적용·왕복·복원·알림 | Shell과 기존 delegate/서비스 | 변경 없음. loader는 읽기/투영만 담당 |

## 호출 경로와 상태 소유권

`WPF command → Shell facade → OpenVisionRecipeStepEditLoader.Load →`
`VisionPipelineStorage/RecipeWorkspaceService → VisionPipelineStepPropertyMapper`
` → OpenVisionRecipeStepEditLoadResult → StepEditSessionViewModel`

- loader는 `Window`, `UserControl`, `System.Windows`, Shell 타입을 참조하지 않는
  concrete C# type이다.
- loader는 XML에서 읽은 `VisionPipeline`, 선택 `VisionPipelineStep`,
  PropertyGrid edit object와 실패 메시지만 결과로 반환한다.
- mutable PropertyGrid 편집 상태와 dirty/clean 전환은 기존
  `OpenVisionRecipeStepEditSessionViewModel`이 소유한다.
- XML 저장, 왕복 검증, 실패 시 기존 XML 복원, 상태/알림과 Preview/Run은
  Shell의 기존 호출 순서를 유지한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeStepEditLoader.cs`
  - Step Edit 읽기·identity fallback·PropertyGrid projection을 소유하는
    Window-free concrete loader와 결과 타입을 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - loader dependency를 추가하고 기존 session 상태 owner는 유지했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - Step Edit load/apply 진입을 loader 결과 기반으로 연결하고 Shell의 기존
    apply/save/round-trip/rollback/notification 경계를 유지했다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--step-edit-loader-contract` 무창 계약을 추가했다.
- `tools/PipelineViewerScreenshotSmoke/Program.cs`
  - 현재 product의 docked Pipeline Review와 details toggle을 먼저 여는
    Step Edit smoke precondition을 맞췄다. 제품 UI 동작 자체는 변경하지 않았다.
- `docs/LLM_DOCUMENT_INDEX.json`, `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`
  - 이번 구조 증거와 다음 단일 우선순위를 기록했다.

## 구조 증거

- `OpenVisionShellHostRecipeCommandSurface*.cs`에서 이전
  `TryLoadSelectedPipelineStep` 구현이 제거됐다.
- Shell handler에는 `VisionPipelineStepPropertyMapper.CreateProperty` 직접
  호출이 남아 있지 않고, `stepEditLoader.Load(...)` adapter만 남아 있다.
- loader 파일에는 `Window`, `UserControl`, `System.Windows`,
  `OpenVisionShellHost` 참조가 없다.
- Shell에는 기존 `ApplyProperty`, save, round-trip validation, restore,
  dirty/clean, status/notification, Preview/Run 호출이 남아 있다.
- Step Edit session ViewModel은 별도 상태 owner로 계속 유지되며 loader와
  mutable 편집 상태를 공유하지 않는다.

## 검증

- x64 Debug app build: 0 warnings / 0 errors, 15.17s.
- x64 Debug `VisionRecipeRunnerSmoke` build: 0 warnings / 0 errors, 9.12s.
- x64 Release app build: 0 warnings / 0 errors.
- x64 Release `VisionRecipeRunnerSmoke` build: 0 warnings / 0 errors.
- `--step-edit-loader-contract` Debug/Release: PASS.
  - XML Step 재해결과 `BlobProperty` projection
  - stale index의 Name/ToolType/OutputLayer identity fallback
  - active pipeline fallback
  - missing selection guard
- `wpf_shell_host_pipeline_step_edit_handoff` Debug/Release: PASS,
  `check=OK`, `layout=0`, `text=0`, `internal=0`, `size=1600x900`.
- `wpf_shell_host_fixture_step_edit_apply_rerun` Debug/Release: PASS,
  `check=OK`, `layout=0`, `text=0`, `internal=0`, `size=1600x900`.
- `PipelineViewerScreenshotSmoke` Debug/Release build: 0 errors, 기존 nullable
  conversion warning 1개(`Program.cs:10722`)가 남아 있다.
- 실행 직전 동적 monitor 조회: 단일 `\\.\DISPLAY1`, bounds
  `0,0,1920x1080`, working area `0,0,1920x1032`; smoke 창은 해당 화면에서
  실행했다.
- 정적 구조 검사와 `git diff --check`는 evidence directory에 기록한다.

모든 실행 증거는
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-loader-20260907`
에 있다. 직접 계약 결과는 `step_edit_loader_contract.txt`와
`selection-contract-release\step_edit_loader_contract.txt`, WPF 결과는
`wpf-step-edit\`, `wpf-step-edit-release\`, `wpf-fixture\`,
`wpf-fixture-release\` 아래에 있다.

## 범위 경계

이번 완료는 선택 Step의 load/resolve/projection owner 분리에 한정한다.
Step Edit apply persistence/validation/rollback의 추가 책임 분리, Recipe
workspace, Review History, 이미지/layer lifetime, 전체 theme/layout/DPI/
multi-monitor 행렬은 이번 실행에서 검증하지 않았다. 현재 WPF smoke는
기본 theme/language, 1600x900, 단일 모니터에서만 실행했다.
Original 저장소, commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** Shell handler의 pipeline XML load, selected-Step resolve,
  PropertyGrid projection.
- **Intended owner:** `OpenVisionRecipeStepEditLoader` concrete module.
- **Dependency direction:** Shell facade → loader → existing Storage/Workspace
  and PropertyGrid mapper; loader result → existing Step Edit session.
- **State owner:** loader는 호출별 읽기 결과만 만들고, mutable edit/dirty state는
  기존 session, XML mutation과 rollback은 Shell/기존 delegate가 소유한다.
- **Observable contract:** public binding names, Recipe/XML format and paths,
  identity fallback, PropertyGrid mapping, Preview/Run explicitness와
  Layer routing은 유지된다.

Status: Complete
Scope: OVL-07 Step Edit selected-Step XML load/resolve/PropertyGrid projection owner 분리.
Acceptance criteria: Shell 직접 load/resolve/projection 제거, Window-free concrete loader 호출 경로, 기존 session/apply/save/round-trip/rollback 계약 유지, Debug/Release 무창 계약과 WPF Step Edit smoke 통과.
Verification: 위 Debug/Release build, loader contract, Step Edit WPF smoke, monitor, static structure, `git diff --check` 검사.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-loader-20260907`.
Boundary / next dependency: OVL-07 Step Edit apply persistence/validation/rollback owner 분리가 다음 단일 작업이며, 전체 UI 행렬과 Original 저장소는 이번 결과가 증명하지 않는다.
