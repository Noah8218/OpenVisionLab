# OpenVisionLab Recipe/Pipeline Storage Path Boundary Hardening

Date: 2026-08-25 KST  
Issue: PL-0007  
Repository: `C:\Git\OpenVisionLab_Dev` (Dev only)

## Status

Complete for the PL-0007 scope. Release publication remains blocked independently by PL-0005 DLL license and provenance evidence.

## Scope

The change hardens mutable Recipe/Pipeline storage paths for:

- Recipe workspaces and Recipe/Pipeline CRUD.
- Pipeline XML, active-pipeline/config/data files, template/reference files, and generated pattern images.
- Pipeline image, Run Report, sample-set, batch-run, validation-set, and intent-skill record artifacts.
- Pipeline project manifests and manifest-provided relative artifact paths.
- Qualified Recipe working-copy dependency paths and qualification report-artifact reads.
- Dependency-copy paths used by Recipe review/import.

Explicit user-selected export paths, repository public-sample catalog paths, and the separate qualified-snapshot archive root retain their existing contracts; they are not silently converted into Recipe workspace paths.

## Implementation result

`src/OpenVisionLab/Core/Recipe/RecipeWorkspaceService.cs` is now the domain owner of the path policy. It provides:

- Strict single-segment validation with explicit messages for required values, `.`/`..`, separators, control/invalid filename characters, Windows reserved device names (`CON`, `PRN`, `AUX`, `NUL`, `COM1`-`COM9`, `LPT1`-`LPT9`), and trailing spaces/periods.
- `Path.GetFullPath` canonicalization and same-root/child containment checks before filesystem mutation.
- Contained relative-artifact resolution for manifest/report paths.
- Case-insensitive Windows collision behavior preserved by canonical path resolution.

Pipeline storage, lifecycle/exchange, sample-set, Run Report, Batch Summary, validation records, dependency copy, template capture, and qualified working-copy callers now use the same policy instead of local sanitizers. Invalid user segments are validated before the corresponding `Ensure`/`CreateDirectory` mutation. Legacy absolute evidence paths remain read-compatible only where the existing read contract requires them; new generated artifacts are relative and contained.

## Acceptance evidence

| Criterion | Result | Evidence |
|---|---|---|
| C1. One domain-owned policy and operator-facing failures | PASS | Common policy in `RecipeWorkspaceService`; focused report records explicit rejection of traversal, reserved devices, trailing space, and control character. |
| C2. Canonical root containment before mutation | PASS | Focused report records all normal paths as contained, case collision canonicalization, rejected recipe/pipeline traversal, no outside directory, and no reserved directory. |
| C3. Valid-name and legacy compatibility | PASS | Korean/English/number/underscore-style names were used in the current contract; public legacy-compatible `Public_Matching_DiePad.pipeline.xml` replay passed. Absolute legacy evidence reads remain explicitly read-only compatibility paths. |
| C4. Lifecycle, samples, reports, snapshots, and runtime | PASS | The current contract exercised Recipe/Pipeline CRUD, sample-set save/load/context/delete, Run Report save/list, Batch Summary save/list; qualified snapshot lifecycle and current public Recipe/Pipeline runtime also passed. |

## Verification actually run

All generated test evidence was written under the D-drive test root as required.

1. `dotnet build "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug` — PASS, 0 warnings, 0 errors.
2. `dotnet run --no-build --project "tools\VisionRecipeRunnerSmoke\VisionRecipeRunnerSmoke.csproj" -c Debug -- --recipe-storage-path-contract "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_current_20260825_r4"` — PASS. Report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_current_20260825_r4\recipe_storage_path_contract.txt`.
3. `dotnet build "tools\QualifiedRecipeSnapshotSmoke\QualifiedRecipeSnapshotSmoke.csproj" -c Debug` — PASS, 0 warnings, 0 errors.
4. `dotnet run --no-build --project "tools\QualifiedRecipeSnapshotSmoke\QualifiedRecipeSnapshotSmoke.csproj" -c Debug -- "D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_qualified_snapshot_20260825_r2"` — PASS (`qualified_recipe_snapshot_core=OK`), including snapshot verification, idempotent reuse, revision/lifecycle transitions, tamper detection, runtime drift separation, interrupted-creation cleanup, and source Recipe deletion.
5. Current public runtime replay with `Public_Matching_DiePad.pipeline.xml` and `Matching_DiePad_Synthetic_OK.png` — PASS. Result: `Matching_Preview`, 572×420, `ResultCount=3`, 19 metrics, 3 overlays. Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_public_recipe_smoke_20260825_r2`.
6. `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"` — PASS, 0 warnings, 0 errors.
7. `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- "C:\Git\OpenVisionLab_Dev"` — PASS, all listed readiness contracts passed.
8. `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestExternalReferences.ps1` — technical check PASS with exit 0. Its `BLOCKED` license/provenance rows remain the independent PL-0005 release blocker; this is not release approval.
9. `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestPublicSampleAssets.ps1` — PASS (`CatalogRows=33`, `ManifestAssets=229`, `Pipelines=17`).

## Structural proof

- Recipe path construction is centralized in `RecipeWorkspaceService`; direct raw dynamic Recipe/Pipeline storage segments were removed from the inspected pipeline storage, lifecycle, validation, report, batch, dependency-copy, template-capture, and qualified working-copy callers.
- The focused contract is in `tools/VisionRecipeRunnerSmoke/Program.cs` and executes the storage owners rather than only asserting helper return values.
- No original-repository files were changed, and no commit, tag, push, release publication, or deployment was performed.

## Boundary / remaining dependency

PL-0007 does not provide DLL license, copyright, or provenance evidence. `PL-0005` remains open and is the Release blocker until the external evidence package is supplied and rechecked. No model-token recommendation is made for PL-0005 external evidence until that prerequisite exists.

## Completion record

```text
Status: Complete
Scope: Recipe/Pipeline storage path validation, canonicalization, containment, and lifecycle/report/sample/snapshot caller hardening in Dev.
Acceptance criteria: C1 PASS; C2 PASS; C3 PASS; C4 PASS.
Verification: Focused contract r4, qualified snapshot smoke r2, public Matching runtime replay r2, Debug solution build, readiness, external-reference technical check, and public-sample check all passed as listed above.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_current_20260825_r4\recipe_storage_path_contract.txt; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_qualified_snapshot_20260825_r2; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_public_recipe_smoke_20260825_r2; this report.
Boundary / next dependency: PL-0005 external DLL license/provenance evidence remains a separate Release blocker; no release mutation was authorized or performed.
```
