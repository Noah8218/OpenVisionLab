# OpenVisionLab Catalog Pair -> Validation Set

Updated: 2026-07-29 KST

## Outcome

Status: Complete

The Recipe Manager now turns the selected catalog OK/NG pair into a
recipe-local Validation Set through one explicit `쌍을 검증 세트로 / Save pair
as set` action.

This closes one non-algorithm commercial-workflow gap: OpenVisionLab already
had a large sample catalog, pair checking, and Local Validation Sets, but the
operator still had to create a set and register the OK and NG files separately.

## User Workflow

1. Select a catalog sample in Recipe Manager.
2. Review its expected metric and pair context.
3. Select `쌍을 검증 세트로`.
4. Review the newly selected Local Validation Set.
5. Explicitly select `목록 검증 실행 / Run suite` when ready.

The import action itself never runs Preview, Run, pair check, or the Validation
Suite.

## Persisted Contract

- Scope: current Recipe.
- Pair identity: catalog source plus `PairGroup`.
- Roles: Good/OK -> expected OK; Bad/NG/ExpectedFailure -> expected NG.
- Evidence: absolute image path and current image SHA-256.
- Variant: exact catalog sample name, bounded to the existing 80-character
  Variant contract.
- Expected metrics: the catalog name/minimum/maximum strings are retained.
  Semicolon-separated multi-metric contracts are now validated and replayable
  by Local Validation Set and Qualified Snapshot preflight.
- Reuse: importing the same catalog pair again updates its owned set instead of
  creating duplicate rows or duplicate sets.
- Collision safety: an unrelated user-created set with the preferred name is
  not overwritten; a unique name is created. Hash-locked evidence is not
  overwritten.

## Current-Sample Proof

The focused smoke used the existing public pair:

- `Public_Matching_DiePad_Good`
- `Public_Matching_DiePad_NoTarget_Bad`

The saved set retained:

- OK 1 / NG 1;
- both image SHA-256 values;
- Good metric names `ResultCount;ScoreMax`;
- Good minimums `3;80`;
- Good maximums `3;100`;
- save/reload/reopen fidelity;
- second-import update with no duplicate set or image.

During both first import and repeat import:

- Preview/Run count was unchanged;
- layer count and active workspace layer were unchanged;
- input and output routing were unchanged.

## Verification

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build "tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj" -c Debug -p:Platform="Any CPU" -p:UseWpfAppHost=false
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target p250_catalog_pair_validation_set "artifacts\p250_catalog_pair_validation_set_20260729\final"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_recipe_local_validation_set "artifacts\p250_catalog_pair_validation_set_20260729\regression_local"
dotnet "tools\PipelineViewerScreenshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\PipelineViewerScreenshotSmoke.dll" --target wpf_shell_host_recipe_qualified_snapshot "artifacts\p250_catalog_pair_validation_set_20260729\regression_qualified"
dotnet run --no-build --project "tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj" -c Debug -- "C:\Git\OpenVisionLab_Dev"
```

Results:

- Full Debug solution build: pass, 0 warnings, 0 errors.
- Screenshot runner build: pass, 0 warnings, 0 errors.
- P250 focused current-source UI/storage smoke: pass.
- Existing Local Validation Set regression: pass.
- Existing Qualified Snapshot regression: pass.
- Readiness contract: all checks pass.
- Current `OpenVisionLab.dll` SHA-256:
  `F0A2BA4CE4F970A4AF5E718BD8F0D709C6CFF7AD88EC8B1CE951D994B5838DF5`.

Evidence:

- Before:
  `artifacts\p250_catalog_pair_validation_set_20260729\before\wpf_shell_host_recipe_local_validation_set.png`
- After:
  `artifacts\p250_catalog_pair_validation_set_20260729\final\p250_catalog_pair_validation_set.png`

## Commercial-Gap Conclusion

Inspection algorithms are not the only commercial gap. The remaining gap
classes are:

1. Workflow compression and persisted setup: connect teaching, sample evidence,
   validation, correction, and reuse with fewer repeated setup actions.
2. Beginner/operator clarity: explain intent, effective settings, judgment, and
   next correction without requiring prior product knowledge.
3. Qualification depth: prove named recipes on representative independent
   production variation, calibration, timing, and failure modes.
4. Algorithm breadth and robustness: add or strengthen a tool only when an
   existing sample or named operator task proves the current family cannot
   express the inspection.
5. Maintainability and test seams: continue moving durable policy out of large
   UI owners when a real responsibility boundary is available.

Camera, lighting, PLC/I/O, MES, account, deployment, and industrial-controller
platform scope remain deliberate exclusions, not missing work for the current
product identity.

## Closure Record

Status: Complete

Scope: Explicit selected catalog OK/NG pair import into one recipe-local
Validation Set, including multi-metric contracts and repeat-import update.

Acceptance criteria:

- One explicit action creates/selects the set: pass.
- OK/NG roles, image hashes, and expected metrics persist: pass.
- Save/reload/reopen round trip: pass.
- Repeat import does not duplicate: pass.
- No Preview/Run, layer, workspace, or route side effect: pass.
- Existing Local Validation Set and Qualified Snapshot contracts remain valid:
  pass.

Verification: Commands and results are listed above.

Evidence:
`artifacts\p250_catalog_pair_validation_set_20260729`.

Boundary / next dependency: This proves catalog-to-validation workflow and
storage fidelity. It does not qualify the imported recipe, replace explicit
Run suite, prove field robustness, or authorize an unproven algorithm.
