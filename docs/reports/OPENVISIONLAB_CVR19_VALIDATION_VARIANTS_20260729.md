# OpenVisionLab CVR-19 Validation Variants

Date: 2026-07-29  
Status: Complete

## Scope

Implemented one bounded Validation Variant workflow for multiple approved
styles under one recipe/Pipeline identity. CVR-00 remains incomplete and was
not used as implementation evidence.

## Completed behavior

- Image-level Variant ID and one expected metric range persist in
  `validation-sets.xml`.
- Selecting a row restores its saved values into one coherent setup row.
- Explicit Apply and Reset persist without Preview/Run, layer, workspace, or
  routing changes.
- Local Validation passes the contract to the existing metric evaluator without
  changing Pipeline parameters.
- Batch XML/TSV, Run History, row display, review queue, and Qualified Snapshot
  retain the Variant contract.
- Review-queue strata include Variant and expected role.
- Run/performance comparison refuses incompatible Variant/metric contracts.
- Invalid, non-finite, incomplete, or reversed ranges fail closed.
- Missing new XML fields retain legacy Default/no-metric-gate behavior.
- Snapshot schema v2 binds Variant fields while schema v1 and legacy
  review-queue v2 verification remain supported.

## Verification

- `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"`:
  passed, 0 warnings, 0 errors.
- `PipelineViewerScreenshotSmoke` target
  `wpf_shell_host_recipe_local_validation_set`: passed with `check=OK`,
  `layout=0`, `text=0`, `internal=0`, `1600x900`.
- The UI smoke proved Variant save/reload and unchanged Preview/Run, layers,
  active workspace, and routes.
- The same smoke replayed the two approved Product catalog variants through
  `Product_Field_DarkFeature_Contour.pipeline.xml` with their unchanged
  `ResultCount 3..8` and `ResultCount 1..4` gates; both passed.
- `QualifiedRecipeSnapshotSmoke`: passed. Its manifest retained both named
  Variant contracts and metric ranges through Snapshot creation and verification.

## Evidence

- Before:
  `artifacts\cvr19_validation_variants_20260729\ui\before\wpf_shell_host_recipe_local_validation_set.png`
- After:
  `artifacts\cvr19_validation_variants_20260729\ui\after\wpf_shell_host_recipe_local_validation_set.png`
- Snapshot smoke:
  `artifacts\cvr19_validation_variants_20260729\qualified_snapshot_smoke_schema2`
- Contract:
  `docs\contracts\openvisionlab\OPENVISIONLAB_VALIDATION_VARIANT_V1_CONTRACT.md`

## Closure record

Status: Complete  
Scope: CVR-19 image-level named Variant plus one metric range across Local
Validation Set, execution, history/review queue, and Qualified Snapshot.  
Acceptance criteria: persisted identity/range -> pass; unchanged Pipeline ->
pass; comparison isolation -> pass; save/reload and no side effects -> pass;
current UI before/after -> pass; Snapshot retention -> pass.  
Verification: full solution build, WPF smoke, approved two-sample replay, and
Qualified Snapshot smoke.  
Evidence: paths above.  
Boundary / next dependency: one metric per image and public Explore-sample
evidence only; no production qualification or compliance platform. CVR-00 still
requires three real independent first-time participants and unedited records.
