# OpenVisionLab OVL-07 Step Edit apply persistence/validation/rollback owner 분리

작성일: 2026-09-07 KST

## 의미와 범위

이번 실행은 OVL-07에서 독립적으로 검증 가능한 한 가지 작업으로,
선택된 Step의 XML 적용 트랜잭션(기존 저장 상태 백업 로드, PropertyGrid 값
반영, 저장, XML 왕복 검증, 실패 시 복원)을
`OpenVisionShellHostRecipeCommandSurface`에서 Window-free concrete
`OpenVisionRecipeStepEditApplyOwner`로 이동했다.

기존 PropertyGrid commit callback, `OpenVisionRecipeStepEditSessionViewModel`,
상태/알림 투영, Pipeline Review 갱신, corrected-output review, 명시적
Preview/Run 순서는 Shell에 남겼다. Recipe/XML schema·경로, 입력/출력 Layer
규칙, 기존 localised 오류 메시지와 test fault-injection 계약은 변경하지
않았다.

## 구조 변화

| 항목 | 이전 | 현재 |
| --- | --- | --- |
| 기존 XML 백업 로드 | Shell apply handler | `OpenVisionRecipeStepEditApplyOwner.Apply` |
| Step PropertyGrid 값 적용 | Shell이 `VisionPipelineStepPropertyMapper` 직접 호출 | apply owner가 기존 mapper 호출 |
| XML 저장과 왕복 검증 | Shell과 주입 delegate | apply owner가 Storage 기본 구현/호환 delegate 호출 |
| 실패 시 이전 Pipeline 복원 | Shell helper | apply owner의 `TryRestorePipelineAfterFailedApply` |
| dirty/clean·상태·Preview/Run | Shell/session | 변경 없음. Shell이 apply 결과를 투영 |

호출 경로는 다음과 같다.

`WPF command → Shell facade → OpenVisionRecipeStepEditApplyOwner.Apply →`
`VisionPipelineStorage/RecipeWorkspaceService + VisionPipelineStepPropertyMapper`
` → OpenVisionRecipeStepEditApplyResult → Shell status/session/Preview/Run`

Apply owner는 `Window`, `UserControl`, `System.Windows`,
`OpenVisionShellHost`를 참조하지 않는다. Shell 생성자의 기존 선택적 save/
validation delegate 매개변수는 내부 호환성을 위해 유지하되, 제품 View는 더
이상 해당 delegate를 만들거나 전달하지 않고 owner 기본 구현을 사용한다.

## 변경 파일

- `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Review/OpenVisionRecipeStepEditApplyOwner.cs`
  - XML apply/save/round-trip/restore 정책과 결과 타입, 기존 smoke용 일회성
    save/validation failure hook을 소유하는 concrete owner를 추가했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.cs`
  - apply owner를 주입하고 기존 호환 delegate는 owner 생성에만 전달한다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
  - 직접 apply/save/round-trip/restore를 제거하고 owner 결과를 기존
    상태·session·Preview/Run 흐름에 연결했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`
  - View의 Step Edit save/validation delegate 및 persistence fault flag를
    제거했다.
- `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.Interactions.cs`
  - View의 직접 XML save/round-trip 메서드를 제거했다. PropertyGrid commit
    callback은 UI 경계로 유지했다.
- `src/OpenVisionLab/UI/Menu/Wpf/Shell/Support/OpenVisionShellHostView.TestHooks.cs`
  - 기존 smoke hook을 Shell/apply owner로 전달한다.
- `tools/VisionRecipeRunnerSmoke/Program.cs`
  - `--step-edit-apply-owner-contract` 무창 회귀 계약을 추가했다.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md`, `docs/LLM_DOCUMENT_INDEX.json`
  - 이번 구조 경계, 증거, 다음 단일 우선순위를 기록했다.

## 구조 증거

- Shell handler에서 `VisionPipelineStepPropertyMapper.ApplyProperty`, 직접
  `VisionPipelineStorage.TryValidateRoundTrip`, `saveStepEditPipeline`,
  `validateStepEditRoundTrip`, 기존 restore helper가 제거됐다.
- `OpenVisionShellHostView.xaml.cs`와 `Interactions.cs`에 Step Edit XML
  save/validation 구현이 남아 있지 않다.
- `OpenVisionRecipeStepEditApplyOwner`만 apply/save/round-trip/restore와
  failure result를 소유하며, Shell은 결과를 상태와 기존 session/Preview/Run
  흐름으로 투영한다.
- 기존 Step Edit session ViewModel은 단일 non-partial concrete type으로
  유지되고 mutable dirty/clean 상태를 계속 소유한다.
- 정적 검사 결과는
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-apply-owner-20260907\static-step-edit-apply-owner-check.log`
  에 `Result=PASS`로 기록했다.

## 검증

- x64 Debug app build: 0 warnings / 0 errors.
- x64 Release app build: 0 warnings / 0 errors.
- x64 Debug `VisionRecipeRunnerSmoke` build: 0 warnings / 0 errors.
- x64 Release `VisionRecipeRunnerSmoke` build: 0 warnings / 0 errors.
- `--step-edit-apply-owner-contract` Debug/Release: PASS.
  - 정상 PropertyGrid 값 적용·저장·왕복 검증
  - 주입된 XML 저장 실패 후 이전 Pipeline 복원
  - 주입된 왕복 검증 실패 후 이전 Pipeline 복원
  - 지원하지 않는 PropertyGrid 객체의 저장 불변성
- `PipelineViewerScreenshotSmoke` Debug build: 0 warnings / 0 errors.
- `PipelineViewerScreenshotSmoke` Release build: 기존 nullable conversion
  warning 1개(`Program.cs:10722`), 0 errors.
- 최신 WPF Debug 실행: `wpf_shell_host_pipeline_step_edit_handoff`와
  `wpf_shell_host_fixture_step_edit_apply_rerun` 모두 `OK|check=OK`,
  `layout=0|text=0|internal=0|size=1600x900`, exit 0.
- 최신 WPF Release 실행: 같은 두 target이 모두 `OK|check=OK`,
  `layout=0|text=0|internal=0|size=1600x900`, exit 0.
- 두 WPF 실행 모두 동적으로 확인한 정확히 두 모니터 환경에서 규칙에 따라
  왼쪽의 작은 모니터 `\\.\DISPLAY2`(work area `-1920,365,1920x1032`)를
  선택하고 창을 `-1760,431..-160,1331`에 배치했다. 실제 사각형 검증은
  `window-rectangle-verification-debug-final.log`와
  `wpf-monitor-run-release-final2.log`에 기록했다. 최신 PNG는 각 target
  폴더에 보관했다.
- `git diff --check`, 문서 index 검증, 정적 구조 검증 결과는 동일 evidence
  directory에 보관한다.

## 범위 경계

이번 완료는 Step Edit XML apply persistence/validation/rollback owner 분리에
한정한다. 전체 Recipe manager CRUD/lifecycle, 이미지/layer lifetime, full
theme/layout/DPI(100/125/150/175/200%)/multi-monitor 행렬은 이번 실행에서
검증하지 않았다. WPF smoke는 현재 기본 theme/language와 1600x900에서만
실행했다. Original 저장소, commit, push, merge, 배포는 수행하지 않았다.

## Refactor proof

- **Current owner:** Shell handler와 View의 Step Edit apply/save/validation/
  restore 경로.
- **Intended owner:** `OpenVisionRecipeStepEditApplyOwner` concrete module.
- **Dependency direction:** Shell facade → apply owner → existing
  Storage/Workspace/PropertyGrid mapper; apply result → existing session and
  status/Preview/Run projection.
- **State owner:** apply owner는 호출별 Pipeline transaction/result와
  rollback을 소유하고, mutable edit/dirty state는 기존 session, UI commit은
  View callback이 소유한다.
- **Observable contract:** public binding names, Recipe/XML format/path,
  PropertyGrid mapping, error/status semantics, Layer routing, explicit
  Preview/Run behavior are preserved.

Status: Complete
Scope: OVL-07 Step Edit XML apply persistence/validation/rollback owner 분리.
Acceptance criteria: View/Shell의 직접 transaction 제거, Window-free concrete owner 호출 경로, 기존 Recipe/XML·session·status·Preview/Run 계약 유지, Debug/Release contract와 WPF smoke 통과.
Verification: 위 Debug/Release build, owner contract, monitor-aware WPF smoke, static structure, `git diff --check`, documentation-index validation.
Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\refactor-ovl07-step-edit-apply-owner-20260907`.
Boundary / next dependency: OVL-07 Step Edit apply 결과/상태 projection owner 분리가 다음 단일 작업이며, 전체 UI 행렬과 Original 저장소는 이번 결과가 증명하지 않는다.
