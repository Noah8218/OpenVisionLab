# Qualified Recipe Snapshot UI Implementation

Updated: 2026-07-27 KST

## Outcome

Recipe Manager `Pipeline Review > History` now owns the complete local
Qualified Recipe Snapshot workflow. An operator can select one completed
`LocalValidationSet` run, choose the bounded qualification scope, enter the
claim note, and explicitly create an immutable Snapshot.

The same panel lists and verifies stored Snapshots, opens their evidence,
creates an editable working Recipe without inheriting qualification, creates a
verified successor before recording `Superseded`, and appends a reasoned
`Revoked` record without changing the evidence payload.

## Operator Contract

Creation is enabled only when:

1. no selected-Step edit is pending;
2. the selected history item is a saved, complete `LocalValidationSet` run;
3. the selected Validation Set name and selected Pipeline match that run;
4. the operator selected `InspectionJudgment` or `LocatorStability`;
5. the operator entered a non-empty claim note;
6. the exact core preflight revalidates Pipeline, image, dependency, batch,
   report, source, drawing, review-queue, and runtime identities.

An unlocked manual Validation Set is not silently marked hash-locked. At
Snapshot creation, its current ordered image/dependency hashes and current
Pipeline definition are captured into the immutable Snapshot request, then
checked against the saved run evidence. A previously hash-locked set must still
match its locked Pipeline identity.

Lifecycle actions require a non-empty reason and explicit confirmation.
Cancelling confirmation changes nothing. Supersede creates and verifies the
successor first, then writes the predecessor's terminal relation. Revoke writes
only an external terminal event; the payload remains intact.

## Working-Copy Contract

`Working copy` requires a new valid Recipe name. It:

- verifies the selected Snapshot payload;
- creates a new editable Recipe and restores the frozen Pipeline;
- copies archived dependencies into the new Recipe `PATTERN` directory and
  rewrites exact dependency paths;
- does not copy the qualified state, lifecycle events, Run History, or
  Validation Set identity.

Failure deletes only the newly created target Recipe.

## UI And Accessibility

The panel stays in the existing Run History surface rather than adding a new
workspace. Inputs have automation names, actions use familiar functional
icons plus text, and irreversible actions retain explanatory tooltips and
confirmation.

The selected row exposes scope, row count, payload/runtime integrity,
lifecycle state, and full Snapshot ID. `Verify integrity` reports payload and
current-runtime status separately.

## Verification

Commands run against current Dev source:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" -m:1
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -m:1
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target "wpf_shell_host_recipe_qualified_snapshot" "artifacts\qualified_recipe_snapshot_ui_20260727\after"
dotnet "tools\QualifiedRecipeSnapshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\QualifiedRecipeSnapshotSmoke.dll"
dotnet "tools\OpenVisionReadinessCheck\bin\Debug\net8.0\OpenVisionReadinessCheck.dll"
```

The focused UI smoke passed:

- one completed Expected-OK locator run;
- pending-Step-edit fail-closed behavior;
- Snapshot create/reload/verify;
- evidence-folder resolution;
- editable working copy without qualification inheritance;
- cancelled supersede with no state change;
- successor creation and predecessor `Superseded`;
- successor `Revoked`;
- unchanged Preview/Run count, layer count, active workspace layer, input route,
  and output route;
- all panel controls visible and keyboard/automation labelled.

The immutable core smoke, Recipe change-safety UI regression, Run History
review-queue regression, and full readiness contract also passed. The two UI
regressions were run in separate fresh processes because the screenshot runner
shares one WPF application per process.

Fresh UI evidence:

- before:
  `artifacts\qualified_recipe_snapshot_ui_20260727\before\wpf_shell_host_recipe_run_history_review_queue.png`
- after:
  `artifacts\qualified_recipe_snapshot_ui_20260727\after\wpf_shell_host_recipe_qualified_snapshot.png`
- lifecycle/integrity contract:
  `artifacts\qualified_recipe_snapshot_ui_20260727\after\wpf_shell_host_recipe_qualified_snapshot.evidence`

## Reusable Acceptance Checklist

- [x] Exact completed Local Validation Set evidence is required.
- [x] Pending selected-Step edits block creation.
- [x] Scope and operator claim are explicit.
- [x] Snapshot payload is immutable, self-contained, and hash-inventoried.
- [x] Payload integrity and current-runtime match are distinct states.
- [x] Working copies are editable and unqualified.
- [x] Supersede/revoke are confirmed, reasoned, append-only terminal events.
- [x] Qualification controls do not Preview/Run or mutate layers/routes.
- [x] Current-build before/after UI evidence exists.

## Boundary

This proves a local evidence-qualified Recipe workflow. It does not prove
production fitness, unseen-data robustness, certified metrology, electronic
signatures, user approvals, remote audit storage, deployment, rollback, PLC/I/O,
camera, or lighting integration.

```text
Status: Complete
Scope: Recipe Manager Run History Qualified Recipe Snapshot panel, adapter, working-copy action, and lifecycle actions.
Acceptance criteria: selected completed Local Validation Set only -> pass; pending edit gate -> pass; create/verify/open/copy/supersede/revoke -> pass; cancelled lifecycle -> no change; Preview/Run/layer/workspace/route invariants -> pass; accessible current-build UI -> pass.
Verification: Debug solution build 0 warnings/errors; screenshot runner build 0 warnings/errors; qualified core smoke OK; wpf_shell_host_recipe_qualified_snapshot current-build smoke OK at 1600x900; recipe change-safety and Run History review-queue regressions OK; readiness contract passed; git diff --check passed with line-ending warnings only.
Evidence: artifacts/qualified_recipe_snapshot_ui_20260727 and this report.
Boundary / next dependency: no active feature priority; reopen implementation only for a reproduced current-source operator blocker or regression. Production/field qualification remains external evidence, not a UI state.
```
