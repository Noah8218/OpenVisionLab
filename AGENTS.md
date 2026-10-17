# Contributor Instructions

Open `OpenVisionLab.sln` and set `src/OpenVisionLab/OpenVisionLab.csproj` as the startup project (`Debug` / `Any CPU`, Windows x64). Use [docs/README.md](docs/README.md) and its single [owner map](docs/admin/CODEBASE_STRUCTURE.md) to find the affected source, stable contracts and matching tests.

## Product Identity

- OpenVisionLab is an OpenCvSharp4-based rule-based vision recipe workbench. Rule-based teaching, execution, drawings, and repeatable validation are the product core.
- LLM support is an optional, maintenance-mode XML authoring aid. It is not the primary product direction and must not be a prerequisite for operating the workbench.
- It is for learning, verifying, and composing image-processing inspection recipes with tools such as Threshold, Blob, Contour, Line/Length, Matching, EdgeBasedMatching, and Feature/Shape-style workflows.
- The main workflow is sample image -> direct PropertyGrid teaching and Pipeline composition -> explicit Preview/Run -> layer/result comparison -> N-sample validation -> saved recipe. Existing LLM XML draft/validation/import may optionally accelerate the composition step.


## Localized User Manual Contract

- The application `Guide` command must select the offline manual from
  `OpenVisionLanguageService.CurrentLanguage`: Korean UI opens only the Korean
  manual and English UI opens only the English manual.
- Do not silently fall back to a manual in a different language. A missing,
  damaged, duplicated, or hash-mismatched selected-language entry must fail
  closed with the localized Guide error.
- Package all supported manual languages beside the EXE under `Guide`, and
  validate each exact language/file/SHA-256 mapping through
  `Guide\guide-manifest.json` before opening it.
- Keep Korean and English manuals structurally equivalent: the same supported
  workflow chapters, Tool coverage, reference scope, numbered callouts, and
  explicit Preview/Run contracts. A language-specific manual must use current
  UI captures from that same application language wherever UI text is visible.
- A language change or Guide open must not run Preview/Run, create/delete/select
  layers, change the active layer, or mutate Pipeline routing.
- Verify both language selections from the current build and a copied clean
  runtime. Check exact HTML language markers, manifest hashes, missing-language
  failure, and that switching Korean -> English -> Korean changes only the
  selected manual path and localized presentation state.


## User-Centered Workflow And Persisted Setup

- Design each feature from the operator's goal and the shortest safe normal
  workflow, not from internal screen, component, class, or storage boundaries.
- When one task requires related settings across several views, dialogs, or
  buttons, treat that friction as a design problem. Consolidate settings that
  belong to one durable workflow into one coherent first-use option or setup
  surface.
- Persist reusable setup only after explicit operator confirmation and at the
  narrowest correct Tool, Recipe, project, workspace, or user scope. Restore
  it on the next equivalent use so the operator does not repeat the same setup.
- Keep restored values visible and editable, provide an explicit reset/default
  path, validate them before reuse, and explain stale or incompatible state.
- Do not silently share task-specific settings across unrelated Recipes,
  projects, workspaces, or users. Do not remember destructive,
  security-sensitive, or safety-critical choices implicitly.
- Restoring configuration must not execute Preview/Run, create/delete/select
  layers, change the active layer, or mutate Pipeline routing.
- Verify reusable setup through save, close/reload/reopen, exact restoration,
  visible reset, stale-state rejection where applicable, and zero unintended
  execution/layer/routing side effects.


## Recipe, Pipeline, And Manager Responsibilities

- Tool Views configure one algorithm; Pipeline owns step order, input/output
  routing and acceptance; Pipeline Review explains retained results; Recipe
  Manager owns the recipe library and lifecycle.
- Select a Pipeline step in the main workspace and edit it in the right Tool
  panel with input/results visible. Preserve parameter models, pending edits,
  working-copy/save, execution, cancellation, save-failure recovery and lifetime.
- Recipe selection/navigation restores configuration without Preview/Run,
  layer creation or route changes. History and XML assistance retain their own
  explicit actions and must not silently modify the current inspection.

## Stable Contracts

- Keep algorithm tools PropertyGrid-based.
- A model property object assigned to PropertyGrid `SelectedObject` should generate the parameter UI.
- Keep business logic out of View code-behind. Move it to ViewModel, Controller, Presenter, Behavior, Converter, Runtime, or Service classes.
- Creating an output layer must not automatically change the input layer.
- Boolean visibility toggles must not trigger Preview or Run.
- Layer create/delete/load-image actions must not auto-run tools.
- Pipeline Review must distinguish a genuinely absent input image from a downstream input that an earlier enabled Step will produce. Show the former as `입력 없음` and keep the latter in the explicit-run `WAIT` state; selecting either Step must not trigger Preview/Run.
- Run History batch analytics must derive from persisted per-sample elapsed values, keep correctness (`failure rate`) separate from performance (`average`, `median`, nearest-rank `p95`, `maximum`), and remain read-only. Do not claim per-Step batch analytics until the batch path persists and links structured Step run reports.
- Run History baseline timing may compare only runs with the same suite kind, suite name, and exact sample-image multiset, and only when every sample has a valid positive elapsed value. Outcome rows may still be compared independently; a different or incomplete set must show that performance comparison was skipped rather than imply a timing regression.
- Do not remove viewer zoom/pan/drag, ROI overlay, template editor, layer comparison, or docking features.
- Do not remove the main window title-bar minimize, maximize/restore, and close controls.
- Do not add `Dirkster.AvalonDock` directly to `src/OpenVisionLab/OpenVisionLab.csproj`; AvalonDock ownership belongs in `src\Libraries\OpenVisionLab.Docking.Controls`.
- Do not reintroduce SDK sample assets into public sample paths.
- Do not reintroduce `dll\Library-Noah` or `OpenCvSharp.Extensions.dll`; OpenVisionLab uses the manifest-verified `dll\OpenVisionLab-Vision-SDK` 3.0 managed DLLs and the single shared `dll\OpenCVSharp\OpenCvSharpExtern.dll`.


## Completion Means Commands Pass

Do not mark work complete by explanation alone. Completion requires command evidence.

For OpenVisionLab changes, run the smallest meaningful set from the list below:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "$PWD"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1
```

- If code tests, linters, or type checks exist for the touched area, run them.
- If a frontend, Python, Rust, or other subproject has its own checks, completion means its relevant command passes, for example `pnpm test`, `pnpm lint`, `pnpm typecheck`, `pytest`, or `cargo test`.
- If a required command is unavailable, report the exact command and the reason it could not run.
- Smoke tests that launch `OpenVisionLab.exe` must use the latest updated build output from the current workspace. Build first, or otherwise verify the EXE timestamp/path corresponds to the current source changes before capturing screenshots or reporting smoke results.
- For EXE smoke tests, build the affected source first and record its exact runtime identity.
- For screenshot smoke tools that instantiate WPF views directly, run the tool from the current Dev workspace after the latest relevant source changes and report it as a current-source view capture, not as an EXE smoke. If the user asks for EXE evidence, launch the latest built EXE instead.
- src/OpenVisionLab/UI/UX changes require current-build before/after evidence. Do not reuse old screenshots.
- When reviewing UI screenshots, explicitly inspect visible controls for clipped text, clipped icons, hidden button content, combo box text visibility, input text visibility, and incoherent overlap.
- For every editable text control visible in a changed panel or container, enter a representative non-empty value and verify the rendered value in the current build. A binding value, `Text` property assertion, placeholder-only screenshot, or ViewModel update does not prove input-text visibility.
- If a focused smoke target exists for the changed panel or container, run it. Any failure blocks `Complete` even when another narrower target passes; repair or separate an unrelated stale precondition before using that smoke as visual evidence.
- Image paths may be included as supporting evidence, but they do not replace inspection of the actual rendered result.


## No Guessing

- Do not assume behavior, file ownership, or current state when it can be checked.
- If unsure, open the relevant file, test, log, screenshot, or command output and cite that evidence in the response.
- If evidence conflicts, state the conflict instead of smoothing it over.


## Think Before Coding

Before editing:

- State the concrete goal in executable terms, for example "make this smoke pass" instead of "improve the feature".
- Identify assumptions.
- If an assumption materially changes the implementation and cannot be verified, ask the user.
- If the task becomes confused or contradictory, stop and re-orient before editing.


## Simplicity First

- Prefer the simplest change that satisfies the requested behavior and can be verified.
- Do not add unrequested features, abstractions, fallback paths, or broad error handling.
- Add abstractions only when they reduce real duplication or match an established local pattern.


## Surgical Changes

- Change only the files and behavior needed for the request.
- Do not reformat unrelated files.
- Do not revert unrelated dirty files.
- Preserve existing public contracts unless the user explicitly asks to change them.


## Source Organization And Folder Rules

- 폴더는 실제 책임을 찾기 위한 신호입니다. 현재 배치는 코드가 원본이며 탐색은
  [CODEBASE_STRUCTURE](docs/admin/CODEBASE_STRUCTURE.md)를 사용합니다. 여기에 클래스·폴더별
  구현 목록을 다시 만들거나 계획만으로 빈 폴더를 만들지 않습니다.
- 새 작업은 기존 feature/module의 concrete owner를 먼저 찾습니다. 폴더 정리나 파일 길이만으로
  완료된 owner를 다시 분할하지 않습니다. 이동은 실제 책임이 명확하고 탐색·변경에 도움이 될 때만 합니다.
- 새 Core 코드는 WPF presentation에 의존하지 않습니다. 기존 호환 의존성을 새 서비스로 복사하지
  않고 표시 변환은 WPF adapter에 둡니다. Pipeline sample 저장·실행은 Core 책임이며 Tool UI에 두지 않습니다.
- 알고리즘 파라미터·PropertyGrid는 해당 Tool/runtime이 소유합니다. 입력 계열의 controller/runtime/
  view-base는 같은 책임 안에서 찾을 수 있게 유지합니다. generic Common/Utils/Helpers/Legacy에 모으지 않습니다.
- Recipe validation/review/sample/batch DTO는 기존 Recipe Models 경계를 사용합니다.
  DTO가 명령 실행·callback·선택·ViewModel 조정을 맡지 않도록 command surface 또는 명명된
  Controller/Presenter에 둡니다.
- Pipeline 실행, review-only 결과 cache, 실행 context와 결과 이미지 해제는 기존 execution owner에
  둡니다. View의 event 연결·선택 탐색과 실행 정책을 섞지 않습니다. hosted-document 수명과
  docking-only adapter 역시 해당 기존 owner를 유지합니다.
- View code-behind는 View 전용 lifecycle·control wiring·framework plumbing만 맡습니다.
  명령 판단·표시값 도출·파일 접근·검증·Pipeline 상태 변경은 해당 ViewModel/Presenter/Controller/
  Runtime/Service에 둡니다. 일반 Views/ViewModels/Documents보다 도메인 owner를 우선합니다.
- 독립적으로 검증할 책임·반복 상태 도출·실제 서비스 경계가 있을 때만 분리합니다.
  새 owner는 명시적 입력과 의존성을 받고 결과/상태 변경을 반환하며 이전 owner의 무관한
  private state에 기대지 않아야 합니다. 그 경계를 확인할 focused test가 필요합니다.
- partial 파일 수나 물리적 이동은 MVVM 경계의 증거가 아닙니다. 생성기/framework 필수 조합과
  응집된 타입 외의 임시 partial은 목표 owner·종료 조건을 기록하고, 줄 수를 줄이려고 추가하지 않습니다.
  같은 partial 타입을 다른 책임 폴더에 흩어 놓지 않습니다.
- 물리 이동은 clean한 응집 파일 그룹만 다룹니다. 사용자 승인 없이 dirty 파일을 옮기지 않고,
  동작·namespace·formatting 변경이나 무관한 리팩토링과 섞지 않습니다.
- XAML/code-behind는 함께 이동하고 x:Class·resource·automation 계약과 namespace를 보존합니다.
  Shell composition 파일은 길거나 이름이 넓다는 이유로 옮기지 않습니다. 새 책임 경계가
  필요하다는 증거와 그 owner 검증을 먼저 확보합니다.
- 새 production .cs를 UI/Menu 바로 아래에 두지 않고 기존 Wpf 책임에 둡니다.
  composition/XAML 쌍·dirty 파일·미확정 owner는 현재 위치를 유지하고 필요 시 인계에 이유를 남깁니다.
  새 폴더에는 응집된 파일 둘 이상 또는 확인된 근시일 owner가 있어야 합니다.
- 이동 전 tracked/clean 여부와 project include를 확인합니다. source check가 경로를 참조하면
  같은 변경에서 새 명시 경로로 고칩니다. 무제한 파일 검색 fallback으로 잘못된 배치를 감추지 않습니다.
- 그룹별 최소 build·관련 smoke를 실행합니다. Core/UI 경계를 함께 바꾸면 solution build와
  readiness도 확인합니다. 완료는 새 call path가 사용되고 이전 owner의 책임이 제거됐다는 증거로 판단합니다.


## Goal-Driven Execution

- Convert broad requests into concrete success criteria.
- Work toward passing checks and preserving stable behavior, not toward producing a long explanation.
- Keep reports grounded in changed files and command results.


## Image and Event Lifetime

Name bitmap/Mat/lease creation and release owners. Dispose temporary copies and unsubscribe events/timers deterministically. UI mutations follow dispatcher affinity. Preserve cancellation, reentry, failure recovery, explicit Preview/Run and pending edits across navigation and shutdown.

Run checks only with isolated test data and an explicit `OPENVISIONLAB_DATA_ROOT`; never use existing user Recipe/CONFIG as a smoke fixture. Preserve dependency manifests, LICENSE, NOTICE and public-sample redistribution rights.
