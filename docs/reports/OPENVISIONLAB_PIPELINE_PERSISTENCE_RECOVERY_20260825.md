# OpenVisionLab Recoverable Pipeline Persistence

Date: 2026-08-25 KST  
Issue: PL-0009  
Repository: `C:\Git\OpenVisionLab_Dev` (Dev only)

## Status

Complete for the PL-0009 Pipeline lifecycle-persistence scope. Release
publication remains blocked independently by PL-0005 DLL license and
provenance evidence.

## Scope

The change covers the mutable Recipe/Pipeline persistence boundary for:

- Pipeline rename and delete transactions.
- The active-Pipeline pointer and its existing-inventory contract.
- Restart/reopen recovery of an interrupted lifecycle transaction.
- Fail-closed handling when the journal or recovery backup cannot be trusted.
- Normal duplicate, rename, delete, active selection, Recipe creation, and
  Recipe/Pipeline reopen without Preview/Run, layer, or route side effects.

Explicit export files, Recipe-workspace directory rename/delete, and external
release or deployment mutation are outside this issue.

## Implementation result

`src/OpenVisionLab/Core/Pipeline/Storage/VisionPipelineStorage.cs` now owns a
small journaled lifecycle transaction for Pipeline rename/delete:

- The journal is written atomically before mutation and records the operation,
  source/target/fallback names, prior pointer, expected pointer, backup name,
  and durable stage.
- The prior XML is preserved under the validated Recipe/VISION directory. The
  recovery path restores it atomically and verifies that a backup is a valid
  copy of the prior Pipeline before using it.
- Rename/delete recovery either rolls back to the prior valid state or adopts
  an already-completed state only after the source/target, fallback, pointer,
  and inventory conditions are proven.
- The final journal stage is durable before backup removal. This closes the
  dangerous window where the backup could be gone while the journal still
  claimed that the source had only been removed.
- An unreadable or ownership-ambiguous journal is retained and exposed as
  `LifecycleRecoveryRequired`; operator files are not silently deleted.
- `pipeline.active.xml` is replaced through a temporary file and atomic
  replacement. A pointer write is rejected unless its target is present in the
  current valid Pipeline inventory.
- New Recipe creation loads/creates the default Pipeline before writing the
  active pointer, so the pointer cannot reference a not-yet-existing file.

The persistence presenter exposes localized `LifecycleRecoveryRequired` and
`LifecycleRecovered` states. It does not authorize Run/validation evidence
while recovery remains required.

## Acceptance evidence

| Criterion | Result | Evidence |
| --- | --- | --- |
| C1. Active pointer writes are atomic and validated against existing inventory | PASS | Focused PL-0009 contract checked replacement of two existing Pipelines, rejected a missing Pipeline, and confirmed the prior pointer remained unchanged. |
| C2. Rename/delete recover or return to a valid prior state across injected intermediate failures | PASS | Focused contract covered six rename stages and five applicable delete stages. Every case produced either byte-identical prior state or a proven completed state, with no retained journal/backup/temp artifact. |
| C3. Startup/reopen detects, repairs, or explains interrupted state without silently deleting operator Pipeline | PASS | The contract clears in-memory persistence state after each injected failure, then re-enters through `LoadActivePipelineName`; journal-backed rollback/completed-state adoption and `LifecycleRecovered` state were verified. Untrusted journal/backup paths fail closed and retain operator files. |
| C4. Normal lifecycle and Recipe reopen have no execution/layer/routing side effects | PASS | Focused contract covered duplicate, active selection, rename, delete, and reopen on a storage-only path; adjacent current-source WPF change-safety, context-switch, and direct-teaching persistence targets passed without Preview/Run or layer/routing mutation. |

## Verification actually run

All generated test evidence was written under the D-drive test root as
required.

1. `dotnet build "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug` — PASS, 0 warnings, 0 errors after the final recovery-order and backup-validation change.
2. `dotnet run --no-build --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --pipeline-persistence-recovery-contract "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r2"` — PASS. Report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r2\pipeline_persistence_recovery_contract.txt`.
3. `dotnet run --no-build --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --recipe-storage-path-contract "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r1\path_contract"` — PASS, preserving the PL-0007 storage-boundary regression check.
4. `dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform=x64` — PASS, 0 warnings, 0 errors.
5. After the final current-source WPF smoke build, `wpf_shell_host_recipe_change_safety`, `wpf_shell_host_recipe_context_switch`, `p254_direct_teaching_pipeline_persistence`, and the dedicated `wpf_shell_host_pipeline_lifecycle_recovery` target — PASS. The dedicated target injected an interrupted rename, reopened Recipe Manager, rendered the localized recovery status, checked journal cleanup, and verified no Preview/Run, layer, document, or route mutation. Current images are under `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r8\ui_smoke`.
6. `wpf_shell_host_recipe_manager_summary` was rerun in isolation and remains an existing separate target failure at its unrelated explicit Pipeline Review assertion (`Result='리뷰 실행 필요'`, `Detail='선택 Step 실행 결과 없음.'`). This turn did not change that Pipeline Review controller or target. It is not used as PL-0009 evidence; the four focused lifecycle-adjacent targets above are the applicable UI evidence.
7. `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` — PASS, 0 warnings, 0 errors after the final storage-only ordering and backup-validation patch.
8. `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` — PASS, all 13 readiness checks.
9. `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` — technical check PASS with exit 0. Its known `BLOCKED` DLL license/provenance rows remain the independent PL-0005 release blocker.
10. `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` — PASS (`CatalogRows=33`, `ManifestAssets=229`, `Pipelines=17`).
11. `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1` — PASS (`IndexedPaths=71`, `Routes=12`, `RootRedirects=101`).

## Structural proof

- Pipeline lifecycle and pointer mutation remain owned by
  `VisionPipelineStorage`; no database, framework, or new persistence
  dependency was introduced.
- The focused contract executes the storage owner through real Recipe/VISION
  filesystem workspaces and checks bytes, pointer state, journal/backup/temp
  cleanup, and reopen behavior rather than only testing helper return values.
- `tools/PipelineViewerScreenshotSmoke/Program.cs` contains a dedicated
  current-source WPF lifecycle-recovery target that verifies the localized
  state in the rendered Recipe Manager surface, not only its ViewModel text.
- The new Recipe creation order prevents an active pointer from being written
  before its default Pipeline exists.
- No original-repository files were changed, and no commit, tag, push, release
  publication, or deployment was performed.

## Boundary / remaining dependency

The restart check is a same-process smoke simulation: it clears runtime
in-memory state and re-enters the storage API after an injected failure. It is
not a claim of multi-process crash qualification, power-loss durability,
network-share behavior, installer recovery, or hardware qualification.

The isolated Recipe Manager summary target remains outside this issue's
evidence because its existing Pipeline Review execution assertion fails before
the final return-to-Recipe checks. PL-0009's storage contract and the focused
adjacent no-side-effect targets pass independently; the dedicated lifecycle
status target provides the applicable runtime UI evidence.

`PL-0005` still requires the missing external DLL license/copyright/provenance
evidence package before any RC2 release gate can pass.

## Completion record

```text
Status: Complete
Scope: Journaled recoverable Pipeline rename/delete persistence, atomic active-pointer replacement and inventory validation, fail-closed recovery state, and normal Recipe/Pipeline lifecycle ordering in Dev.
Acceptance criteria: C1 PASS; C2 PASS; C3 PASS; C4 PASS.
Verification: PL-0009 focused recovery contract r2, PL-0007 path contract, final current-source WPF lifecycle-adjacent and dedicated recovery-status targets, zero-warning solution build, readiness, external-reference technical check, public-sample check, and documentation-index check passed as listed above.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r2\pipeline_persistence_recovery_contract.txt; current-source WPF images under D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r8\ui_smoke; this report.
Boundary / next dependency: The Recipe Manager Review target remains a separate existing UI failure; PL-0005 external DLL license/provenance evidence remains the release blocker. No release mutation was authorized or performed.
```
