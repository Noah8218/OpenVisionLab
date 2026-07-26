# Validation Run Session ViewModel Refactor Proof (2026-07-26)

## Status

Complete.

## Scope

- Move Validation Suite and Local Validation Set run-session state out of
  `OpenVisionShellHostRecipeCommandSurface`.
- Preserve explicit suite execution, stop-after-current behavior, partial
  result persistence, status bindings, and command enablement.

## Excluded

- No Pipeline execution, sample iteration, judgment, report storage, Run
  History, XML, algorithm, Preview/Run, layer, route, or visible layout change.
- No new service interface, factory, command-surface partial, or parallel
  execution path.
- Good/Bad and Catalog retain their existing dedicated execution flags.

## Structural Change

- Previous owner: Shell fields directly held Validation Suite running, Local
  Validation Set running, stop-requested, and status-text state.
- Current owner: `OpenVisionRecipeValidationRunSessionViewModel` owns those
  four mutable values and the `Start`, `RequestStop`, `Complete`, and
  `SetStatus` transitions.
- Current call path:
  Shell command/execution loop -> Validation Run Session ViewModel transition
  -> Shell notification adapter -> existing command and XAML bindings.
- Dependency direction:
  the Shell depends on the independent session ViewModel; the ViewModel has no
  dependency on the Shell, storage, execution services, WPF views, or recipes.
- Execution loops, frozen-identity validation, result judgment, report
  persistence, and Run History refresh remain in the Shell.

## Acceptance Criteria

1. The four old mutable Shell fields are absent.
2. One non-partial ViewModel owns run-session state and transitions.
3. Local Validation Set complete-run and stop/partial-save paths pass from
   current source.
4. The suite continues to avoid native Preview/Run, layer, workspace, and
   routing side effects.
5. Debug build, focused smoke project build, and readiness check pass.

## Verification

- Source search found no Shell-owned `isValidationSuiteRunning`,
  `isLocalValidationSetRunning`, `validationSuiteStopRequested`, or
  `validationSuiteStatusText` field.
- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --nologo`
  passed with 0 warnings and 0 errors.
- `dotnet run --project
  tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c
  Debug -- --target wpf_shell_host_recipe_local_validation_set
  artifacts\mvvm_validation_run_session_viewmodel_20260726` passed with
  `check=OK`, `layout=0`, `text=0`, and `internal=0`.
- The focused smoke executes a complete local set, confirms Stop is available
  during a second execution, requests stop, verifies an explicit partial
  result, and checks unchanged Preview/Run count, layers, workspace, and
  routes.
- A clean focused smoke project build passed with 0 warnings and 0 errors.
- `dotnet run --no-build --project
  tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug`
  passed all readiness contracts.
- Current-source artifact:
  `artifacts/mvvm_validation_run_session_viewmodel_20260726/wpf_shell_host_recipe_local_validation_set.png`.

## Completion Record

Status: Complete

Scope: Validation Suite and Local Validation Set mutable run-session state and
transitions moved from the Shell to one independent ViewModel.

Acceptance criteria: old Shell fields absent; new owner and call path present;
complete and stop/partial-save UI paths passed; workspace side effects absent;
build and readiness passed.

Verification: source searches, Debug solution build, focused local-validation
UI smoke, focused smoke project build, and readiness check.

Evidence:
`docs/admin/OPENVISIONLAB_VALIDATION_RUN_SESSION_VIEWMODEL_REFACTOR_PROOF_20260726.md`
and `artifacts/mvvm_validation_run_session_viewmodel_20260726`.

Boundary / next dependency: this does not claim that the whole Recipe command
surface is MVVM-complete or qualify inspection algorithms. Another structural
slice requires a separately audited mutable-state or business-rule owner.
