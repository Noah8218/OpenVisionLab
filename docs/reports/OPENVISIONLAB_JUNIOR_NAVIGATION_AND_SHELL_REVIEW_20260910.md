# OpenVisionLab 2D Shell·Pipeline Review 주니어 탐색성 검토

Updated: 2026-09-10 KST

Status: VERIFIED for the named Pipeline Review source path, focused builds, and
representative Pipeline Review smoke. Full WPF theme/DPI/input/long-run
qualification remains unverified.

## Scope

이 slice는 다음 질문에 답하는 데 집중합니다.

- Pipeline Review는 누가 만들고 누가 도킹하는가?
- Details 영역은 어느 View 상태가 여는가?
- 실행·이미지·종료 수명은 어느 owner가 갖는가?

Shell 전체를 다시 분리하거나 긴 타입명을 일괄 변경하지 않습니다. 이미
닫힌 owner의 책임과 XAML/public binding 계약을 보존하고, 탐색 경로를 한
단계 짧게 만드는 이름과 문서만 추가합니다.

## Current → intended owner and call path

| 항목 | 현재 owner | 이 slice의 의도 |
| --- | --- | --- |
| Tool selection entry | `OpenVisionShellHostToolWindowController.ShowSelectedTool` | Pipeline branch가 `ShowPipelineReview`라는 이름으로 드러나도록 유지 |
| Review document state | `OpenVisionShellHostDocumentController` + `OpenVisionPipelineReviewDocument` | restore/create/cache와 document lifecycle은 기존 owner에 유지 |
| Docking | `OpenVisionShellHostToolWindowLifecycleController.ShowDockedDocumentWorkspace` → `OpenVisionDockedDocumentWorkspaceController` | Pipeline Review의 기본 경로가 도킹 문서임을 명시 |
| Details visibility | `OpenVisionPipelineReviewView.btnReviewDetailsToggle` → `OnReviewDetailsToggleChanged` → `UpdateReviewDetailsLayout` | Details는 View-local presentation state로 유지; Document/Runner가 열지 않음 |
| Review execution | `OpenVisionPipelineReviewDocument` → `OpenVisionPipelineReviewExecutionController` → `VisionPipelineExecutionService.RunPreparedAsync` | 실행 policy, stale revision, result-image lifetime은 기존 owner에 유지 |
| Shutdown | `OpenVisionShellHostView.Dispose` → `OpenVisionShellHostSessionController`/`OpenVisionShellHostLifecycleController`; process-scoped default `DisplayManager`는 `OpenVisionLabApplication` | View가 공유 기본 manager를 해제하지 않는 기존 경계를 유지 |

실제 변경 후 핵심 호출 경로는 다음과 같습니다.

```text
OpenVisionShellHostView
  -> OpenVisionShellHostToolSelectionController
  -> OpenVisionShellHostToolWindowController.ShowSelectedTool
  -> ShowPipelineReview
  -> OpenVisionShellHostDocumentController.TryRestorePipelineReview / CreatePipelineReviewDocument
  -> OpenVisionShellHostToolWindowLifecycleController.ShowDockedDocumentWorkspace
  -> OpenVisionDockedDocumentWorkspaceController
  -> OpenVisionPipelineReviewDocument.View
```

`ShowPipelineReview`는 새 상태 owner가 아닙니다. 기존 `ShowSelectedTool`
안의 Pipeline branch를 이름 있는 private call path로 옮긴 것입니다.

## Shortest reading order

처음 한 번의 검색으로 시작하려면 다음 순서를 사용합니다.

1. `src/OpenVisionLab/Program.cs`
2. `src/OpenVisionLab/App/Bootstrap/OpenVisionLabApplication.cs`
3. `src/OpenVisionLab/UI/Menu/Wpf/OpenVisionShellHostView.xaml.cs`
4. `src/OpenVisionLab/UI/Menu/Wpf/Shell/Tooling/OpenVisionShellHostToolSelectionController.cs`
5. `src/OpenVisionLab/UI/Menu/Wpf/Shell/Tooling/OpenVisionShellHostToolWindowController.cs`의 `ShowSelectedTool` → `ShowPipelineReview`
6. `src/OpenVisionLab/UI/Menu/Wpf/Shell/Documents/OpenVisionShellHostDocumentController.cs`
7. `src/OpenVisionLab/UI/Menu/Wpf/Shell/Tooling/OpenVisionShellHostToolWindowLifecycleController.cs`
8. `src/OpenVisionLab/UI/Menu/Wpf/Documents/OpenVisionPipelineReviewDocument.cs`
9. `src/OpenVisionLab/UI/Menu/Wpf/PipelineReview/Execution/OpenVisionPipelineReviewExecutionController.cs`
10. `src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewView.xaml.cs`의 Details toggle handlers

빠른 검색:

```powershell
rg -n "ShowSelectedTool|ShowPipelineReview|TryRestorePipelineReview|ShowDockedDocumentWorkspace|btnReviewDetailsToggle|OnReviewDetailsToggleChanged" src/OpenVisionLab/UI/Menu/Wpf
```

## Mutable-state and lifetime ownership

- 선택된 Shell tool/document: `OpenVisionShellHostDocumentController`
- 도킹/부동 표시 상태: `OpenVisionShellHostToolWindowLifecycleController`와 기존 host
- Review 실행 세대·취소·stale callback: `OpenVisionPipelineReviewExecutionController`와 `OpenVisionPipelineReviewDocumentRevisionGate`
- Details expanded/collapsed와 탭/레이아웃: `OpenVisionPipelineReviewView` 내부 상태
- Review image snapshot/cache: `OpenVisionPipelineReviewLayerImageOwner`와 execution result owner
- 공유 기본 `DisplayManager`: `OpenVisionLabApplication`; per-session manager는 runtime context/session owner

새 slice는 이 상태 owner들을 서로 연결하는 EventBus, Manager, Factory,
Interface, manual partial로 숨기지 않습니다.

## Focused proof

- source path: `ShowSelectedTool`의 Pipeline branch가 `ShowPipelineReview`로 위임되고, 그 메서드가 기존 `TryRestorePipelineReview`와 `ShowDockedDocumentWorkspace`를 호출하는지 확인
- behavior: `wpf_shell_host_pipeline_review`, `wpf_shell_host_pipeline_review_ng`, `wpf_shell_host_workspace_sample_pipeline_review_metrics` smoke가 `OK`
- build: OpenVisionLab Debug/Release 0 warning/0 error, PipelineViewerScreenshotSmoke Release 0 error/기존 CS8600 1 warning
- static: `TestDocumentationIndex.ps1` PASS, `Invoke-RefactorAudit.ps1 -Verify` PASS, `git diff --check` clean
- evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-qualification-r10-pipeline-review-20260910`
- runtime boundary: 다른 theme, Wide/Compact, 100/125/150/175/200% DPI, keyboard/pointer matrix, camera/SDK, permanent native hang, long-run은 실제 실행 전까지 미검증

완료 문구는 실행 결과에 따라 `소스 코드 기준 검토 완료 / 실제 Runtime UI
검증 필요` 범위를 유지하며, 실행하지 않은 상태를 PASS로 표시하지 않습니다.

## Self-evaluation after this slice

| 질문 | 판단 |
| --- | --- |
| 시작점은 찾을 수 있는가? | `Program.Main`부터 Shell까지 고정된 순서가 있다. |
| Pipeline Review의 생성·도킹 owner는 찾을 수 있는가? | 이름 있는 `ShowPipelineReview`와 문서 경로로 한 번에 찾을 수 있다. |
| Details를 누가 여는가? | View의 `btnReviewDetailsToggle`가 명시적 owner다. |
| 비동기/Dispose를 어디서 추적하는가? | Review revision/execution/image owner와 Application shutdown 경로를 분리해 읽는다. |
| 전체 Shell이 단순해졌는가? | 아직 아니다. 생성자와 지원 객체 수는 유지되며, 다음 slice에서 실제 defect나 독립 lifecycle 경계가 확인될 때만 최소 변경한다. |

## PL-0014 follow-up — Shell composition reading route

Updated: 2026-09-11 KST

이번 후속 작업은 `OpenVisionShellHostView`의 기존 생성자 책임과 초기화
순서를 유지하면서, 처음 읽는 개발자가 단계 경계를 찾을 수 있도록 다섯 개의
이름 있는 주석 구간을 추가한 작업이다. 생성자 밖에서 `readonly` 필드를
대입하도록 메서드를 추출하는 시도는 컴파일러가 거부했고, 그 시도는
되돌렸다. 따라서 새 wrapper, interface, manager, partial, forwarding type은
추가하지 않았다.

### Owner and call path

- Current and intended composition owner: `OpenVisionShellHostView` constructor.
- Runtime and shared state owners: existing runtime/session context fields created
  by the constructor; no new owner was introduced.
- Mutable Shell state writers: existing Session, Layer, Workspace, Tooling, Recipe,
  Document, and Pipeline Review concrete owners remain unchanged.
- Lifetime boundary: existing event subscriptions and release callbacks remain in
  the constructor and `Dispose` path; no event or Dispose order changed.
- Shortest route: `Program.Main` -> `OpenVisionLabApplication.Run` ->
  `OpenVisionShellHostWindow` -> `OpenVisionShellHostView` constructor. Read the
  five constructor markers in order: runtime owners, visual wiring, Workspace/
  Recipe owners, commands/session/layer/test adapters, then event/release wiring.

The existing XAML and command contract remains the binding surface. The Shell
still composes `RecipeCommands`; Recipe state and command mutation remain owned by
the existing Recipe command surface and controller.

### Junior developer evaluation

From a first-time-contributor perspective, the change makes the constructor's
initialization order visible without requiring a second abstraction layer. A reader
can now search for `Composition order:` and follow the five phases before tracing
individual controller fields. The change is safe to accept because the constructor
body token contract is identical before and after the comments-only edit
(`22534` tokens on both sides), and no owner or binding contract moved.

The remaining burden is real: the constructor is still large, and the individual
controllers/presenters must still be followed to understand behavior. A further
split is not justified by readability alone because the readonly fields are owned
by the constructor and there is no independently testable lifetime boundary proven
in this slice.

### Focused evidence

- `OpenVisionShellHostView.xaml.cs` phase markers and owner map:
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0014-shell-composition-navigation-20260911\phase-map.txt`
- constructor token contract: `constructor-token-contract.txt` (`PASS`)
- Shell Recipe lifecycle contract: Debug and Release, `passed=7|failed=0`
- `OpenVisionReadinessCheck`: Debug and Release, passed
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS`
- `TestDocumentationIndex.ps1`: `DocumentationIndex=PASS`
- `git diff --check`: exit code `0` (existing LF/CRLF conversion warnings only)

### Runtime boundary

This is a source and contract verification result. WPF runtime theme states,
Wide/Compact layouts, DPI 100/125/150/175/200%, monitor placement, keyboard and
pointer matrix, camera/SDK calls, native permanent-hang behavior, and long-run
shutdown were not executed in this slice. Those remain
`소스 코드 기준 검토 완료 / 실제 Runtime UI 검증 필요`.

## PL-0015 follow-up — Shell View member navigation

Updated: 2026-09-11 KST

`OpenVisionShellHostView.xaml.cs`는 기능을 다시 쪼개지 않고, 기존 멤버를
책임별 영역으로 읽을 수 있게 구조화했다. 새 클래스·partial·interface·
manager·wrapper는 추가하지 않았다. 영역은 다음 순서로 읽는다.

1. `Constants and Dependency Properties`
2. `Fields`
3. `Constructors`
4. `Public View Contract`
5. `Lifetime`
6. `Interactions` 아래의 `Recipe Property Grid`, `Recipe Manager Navigation`,
   `Pipeline Review Navigation`, `Recipe Panel Drag`, adapter callback wiring,
   `Runtime and Localization`, `Sample and Layer Projection`, `Shell Log
   Presentation`, `Localization Helpers`, `Test Contract Helpers`
7. `Test Contract Compatibility` 아래의 `Surface and Test Hooks`, `Test
   Properties`, `Test Actions`

### Junior developer evaluation

처음 프로젝트를 여는 개발자는 이제 2,300줄대 파일에서 업무 영역을 찾기
위해 메서드 전체를 순서대로 읽지 않아도 된다. 생성·공개 View 계약·수명주기와
Recipe/Pipeline/대화상자/레이아웃/테스트 호환 표면이 별도 검색 지점으로
드러난다. 기존 테스트 호환 forwarding 메서드는 외부 smoke 호출을 보존하기
위해 유지했으며, `Test Contract Compatibility`라는 경계로 제품 코드와
구분된다.

남은 부담은 생성자 자체의 조합 규모와 기존 Controller·Presenter 연결이다.
이번 변경은 그 연결을 숨기는 새 계층을 만들지 않았으므로, 주니어 관점에서
탐색성은 개선되었지만 Shell View가 작은 클래스가 되었다고 평가하지 않는다.
추가 분리는 독립 상태·수명·테스트 경계가 재현될 때만 진행한다.

### Focused proof

- region-only token contract: `PL0015_REGION_TOKEN_CONTRACT=PASS`, before/after
  normalized SHA-256 identical
- Solution Debug/Release build: 0 warnings / 0 errors (sequential `-m:1`)
- VisionRecipeRunnerSmoke Debug/Release build: 0 warnings / 0 errors
- Shell Recipe lifecycle contract: Debug/Release `passed=7|failed=0`
- `OpenVisionReadinessCheck`: Debug/Release passed
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS`
- `TestDocumentationIndex.ps1`: `DocumentationIndex=PASS`
- `git diff --check`: exit code `0`
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0015-shell-view-member-navigation-20260911`

### Runtime boundary

This remains source and focused-contract verification. Full WPF themes,
Wide/Compact layouts, DPI 100/125/150/175/200%, monitor placement, pointer and
keyboard states, camera/SDK/GPU, native permanent hangs, and long-run shutdown
were not executed. The result remains
`소스 코드 및 focused contract 검토 완료 / 실제 Runtime UI·하드웨어 검증 필요`.

## PL-0016 follow-up — Recipe dialog side-effect boundary

Updated: 2026-09-11 KST

The Recipe dialog, file-picker, and qualified-evidence folder operations were
moved from `OpenVisionShellHostView.xaml.cs` to the concrete
`RecipeDialogAdapter` at
`src/OpenVisionLab/UI/Menu/Wpf/Shell/Recipe/RecipeDialogAdapter.cs`.
The adapter owns only WPF modal/file/process side effects and the two existing
test delegates. Recipe state, workflow policy, pending-edit decisions, and
resource lifetime remain owned by the existing Recipe command surface, Shell
test surface, and Shell lifetime controller.

### Owner and call path

- Current and intended dialog owner: `RecipeDialogAdapter`.
- Call path: `OpenVisionShellHostView` constructor -> adapter method-group
  callbacks -> `OpenVisionShellHostRecipeCommandSurface` -> adapter dialog or
  file-picker operation -> path/boolean result back to the command surface.
- Test hook path: Shell `*ForTest` property ->
  `OpenVisionShellHostViewTestSurface` -> adapter test delegate.
- Mutable Recipe state and persistence owner: existing
  `OpenVisionShellHostRecipeCommandSurface` and concrete Recipe owners.
- Lifetime owner: `OpenVisionShellHostView` creates the adapter and releases the
  existing Shell/session resources; the adapter owns no event, timer, Bitmap, or
  other disposable resource.

The existing callback signatures, language text, modal owner lookup, dialog
options, `Process.Start` failure result, and `QualifiedSnapshot*ForTest` names
remain unchanged. At the PL-0016 checkpoint, `DecidePendingRecipeEdit` was
still a separate existing Shell/test contract; PL-0017 below records its later
move into this adapter.

### Junior developer evaluation

This is a meaningful navigation improvement: a contributor can now search one
concrete file for Recipe confirmation, file selection, and evidence opening
without reading the Shell composition View. The adapter is concrete and direct;
no interface, factory, manager, or forwarding layer was added. The remaining
pending-edit dialog is explicitly documented as a separate boundary.

### Focused proof

- dialog body token contract: before/after normalized SHA-256 identical
- View contains no direct Recipe dialog/file-picker/process I/O calls
- all Recipe dialog callbacks route through `RecipeDialogAdapter`
- test delegates route through the adapter
- Solution and VisionRecipeRunnerSmoke Debug/Release builds: 0 warnings / 0 errors
- Shell Recipe lifecycle contract: Debug/Release `passed=7|failed=0`
- `OpenVisionReadinessCheck`: Debug/Release passed
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS`
- `TestDocumentationIndex.ps1`: `DocumentationIndex=PASS`
- `git diff --check`: exit code `0`
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0016-dialog-adapter-20260911`

### Runtime boundary

This is source, build, and focused-contract verification. Actual MessageBox and
file-picker interaction, external evidence-folder launch, localization rendering,
WPF theme/layout/DPI/monitor/input, camera/SDK/GPU, native permanent hangs, and
long-run shutdown were not executed. The result remains
`소스 코드 및 focused contract 검토 완료 / 실제 Runtime UI·하드웨어 검증 필요`.

## PL-0017 follow-up — pending Recipe edit decision boundary

Updated: 2026-09-11 KST

The remaining pending Recipe edit decision was moved from
`OpenVisionShellHostView.xaml.cs` into the existing concrete
`RecipeDialogAdapter` at
`src/OpenVisionLab/UI/Menu/Wpf/Shell/Recipe/RecipeDialogAdapter.cs`.
The adapter now owns the existing test decision queue and modal dialog choice.
No new interface, factory, manager, or partial was introduced.

### Owner and call path

- Current and intended pending-edit owner: `RecipeDialogAdapter`.
- Runtime path: `OpenVisionShellHostView` registers
  `RecipeDialogAdapter.DecidePendingEdit` with the existing Recipe command
  surface; the command surface requests a decision; the adapter dequeues a
  test decision or opens the existing modal; the decision returns unchanged.
- Test path: existing Shell
  `QueuePendingRecipeEditDecisionForTest` ->
  `OpenVisionShellHostViewTestSurface` -> adapter queue.
- Mutable Recipe state and persistence remain owned by the existing Recipe
  command surface and Recipe owners.
- View/session lifetime, event cleanup, image ownership, and shutdown remain
  with the existing Shell/session owners. The adapter owns no disposable,
  event, timer, or image resource.
- Shortest reading order: search `QueuePendingRecipeEditDecisionForTest` in
  `OpenVisionShellHostView.xaml.cs`, follow the public compatibility forwarder
  to `OpenVisionShellHostViewTestSurface`, then read the adapter queue and
  `DecidePendingEdit`; for runtime, follow the constructor method-group
  registration into the existing Recipe command surface.

The existing request/decision types, callback signature, public test method
name, queue order, owner lookup, default decision, and modal semantics remain
unchanged. A normalized before/after method token hash is identical.

### Junior developer evaluation

This closes the separately documented pending-edit boundary from PL-0016. A
developer can now find all Shell Recipe modal decisions in one concrete adapter
and follow the public test path without entering the Shell View implementation.
The Shell constructor and other UI-lifecycle wiring remain sizable, but no
additional independent ownership boundary was demonstrated by this change.

### Focused proof

- pending-edit contract: `PL0017_PENDING_EDIT_CONTRACT=PASS`
- View has no pending-edit queue or direct dialog construction
- callback and test queue route through `RecipeDialogAdapter`
- Solution Debug/Release: 0 warnings / 0 errors
- focused app and VisionRecipeRunnerSmoke Debug/Release: 0 warnings / 0 errors
- Shell Recipe lifecycle contract: Debug/Release `passed=7|failed=0`
- `OpenVisionReadinessCheck`: Debug/Release passed
- `Invoke-RefactorAudit.ps1 -Verify`: `REFACTOR_AUDIT=PASS`
- `TestDocumentationIndex.ps1`: `DocumentationIndex=PASS`
- `git diff --check`: exit code `0`
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0017-pending-edit-dialog-20260911`

### Runtime boundary

This is source, build, and focused-contract verification. Actual modal
interaction, owner-window and localization rendering, WPF theme/layout/DPI/
monitor/input, camera/SDK/GPU, native permanent hangs, and long-run shutdown
were not executed. The result remains
`소스 코드 및 focused contract 검토 완료 / 실제 Runtime UI·하드웨어 검증 필요`.
