# Recipe Execution Session ViewModel Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Consolidate Validation Suite, Local Validation Set, selected-sample,
  Good/Bad pair, and Catalog execution activity state under one owner.
- Preserve the exact existing command guards, explicit execution paths,
  stop-after-current behavior, status bindings, and result projections.

## Excluded

- No Pipeline execution, result-summary calculation, sample iteration,
  judgment, report storage, Run History, XML, algorithm, Preview/Run, layer,
  route, or visible layout change.
- No new interface, service, factory, command-surface partial, concurrency, or
  parallel execution path.

## Structural Change

- Previous owners:
  `OpenVisionRecipeValidationRunSessionViewModel` owned Validation Suite,
  Local Validation Set, stop-requested, and status state, while the Shell
  directly owned selected-sample, Good/Bad pair, and Catalog running flags.
- Current owner:
  `OpenVisionRecipeExecutionSessionViewModel` owns all six running flags,
  stop-requested state, status text, and their explicit start/complete/stop
  transitions.
- Current call path:
  Shell command/execution loop -> Recipe Execution Session transition -> Shell
  notification adapter -> existing command and XAML bindings.
- Dependency direction:
  the Shell depends on one independent execution-session ViewModel; the
  ViewModel has no dependency on the Shell, storage, execution services, WPF
  views, recipes, or result summaries.
- Result summaries and their review projections remain separate Shell state.

## Acceptance Criteria

1. The Shell contains none of the six old running-state fields.
2. One non-partial ViewModel owns the execution activity and stop transitions.
3. Existing command guards retain their exact combinations; no new mutual
   exclusion or concurrent execution behavior is introduced.
4. Local Validation Set complete-run and stop/partial-save paths pass.
5. A real Good/Bad pair rerun and result-summary projection pass.
6. Explicit execution continues to avoid native Preview/Run, layer, workspace,
   and routing side effects.
7. Debug build and readiness check pass.

## Verification

- Source search found no Shell-owned `isValidationSuiteRunning`,
  `isLocalValidationSetRunning`, `isSampleCheckRunning`,
  `isPairCheckRunning`, `isCatalogBenchmarkRunning`,
  `validationSuiteStopRequested`, or `validationSuiteStatusText` field.
- Source search confirmed that all start/complete/stop transitions route
  through `OpenVisionRecipeExecutionSessionViewModel`.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  passed with 0 warnings and 0 errors.
- Current-source `wpf_shell_host_recipe_local_validation_set` passed with
  `check=OK`, `layout=0`, `text=0`, and `internal=0`. It covers a complete run,
  Stop availability, stop request, explicit partial-save result, and unchanged
  workspace state.
- Current-source `wpf_shell_host_fixture_step_edit_apply_rerun` passed with
  `check=OK`, `layout=0`, `text=0`, and `internal=0`. It covers a real
  Good/Bad pair execution, updated pair result summary, and unchanged native
  Preview/Run and layer state.
- `dotnet run --no-build --project
  tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  passed all readiness contracts.
- Current-source artifacts:
  - `artifacts/mvvm_recipe_execution_session_viewmodel_20260726/local_validation`
  - `artifacts/mvvm_recipe_execution_session_viewmodel_20260726/pair_check`

## Completion Record

Status: Complete

Scope: six Recipe execution activity flags, validation stop state, status
text, and their transitions consolidated under one independent ViewModel.

Acceptance criteria: old Shell fields absent; one current state owner present;
command guards preserved; local complete/stop and real pair paths passed;
workspace side effects absent; build and readiness passed.

Verification: source searches, Debug solution build, two focused current-source
UI smokes, visual review, and readiness check.

Evidence:
`docs/admin/OPENVISIONLAB_RECIPE_EXECUTION_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`
and `artifacts/mvvm_recipe_execution_session_viewmodel_20260726`.

Boundary / next dependency: result summaries remain separate because they are
execution outputs, not activity state. This does not prove parallel execution,
inspection accuracy, or whole-Shell MVVM completion.
