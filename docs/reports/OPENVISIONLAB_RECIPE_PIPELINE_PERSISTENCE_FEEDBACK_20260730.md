# OpenVisionLab Recipe/Pipeline Persistence Feedback — P272

Date: 2026-07-30 KST
Status: Complete

## Outcome

P272 reproduced and corrected two current persistence defects:

1. A malformed active Pipeline was preserved as an invalid backup and replaced
   by an empty editable Pipeline, but Recipe Manager showed `XML OK` and did
   not explain that the taught definition had been replaced.
2. An unreadable Recipe `CData` file could propagate a load exception during
   Recipe switching, while invalid/default substitution and save failure had
   no retained operator state.

The correction is owned by `VisionPipelineStorage` and `RecipeDataStorage`.
It does not add a generic persistence framework, schema migration, autosave,
database, or modal dialog.

## Operator Contract

- Missing first-use files still create quiet editable defaults.
- Valid files restore without warnings.
- Malformed, truncated, and wrong-root files retain byte-exact
  `.invalid-<timestamp>.xml` backups.
- The invalid canonical file remains in place while an editable default is
  supplied in memory. This keeps the warning reproducible across restart.
  Explicit save performs the atomic replacement; a failed replacement leaves
  the canonical prior file available.
- Unreadable loads return an editable in-memory default, retain the full cause
  and path, and do not change the disk file.
- Save failures retain the current in-memory edit, state reopen-loss risk, and
  preserve the previous disk file.
- Recipe Manager shows a compact nonmodal warning in the Pipeline summary
  card. Full Recipe/Pipeline/path/cause/backup text is available through
  Tooltip and accessibility HelpText.
- Persistence failures force the Recipe summary to `XML NG` and disable sample,
  pair, catalog, and validation-set execution. Pipeline Review remains
  available for diagnosis and explicit replacement.
- Direct Tool `Add to Pipeline` reports the same memory-versus-disk failure.
  Retrying after the external condition is corrected persists exactly one
  Step.
- The next successful save reports recovery once. A later ordinary save clears
  the recovery notice.
- No persistence feedback or recovery action triggers Preview/Run, creates or
  selects layers, or changes Pipeline routes.

## R1–R10 Matrix

The machine-readable result is:

`artifacts\p272_recipe_pipeline_persistence_20260730\after_current_exe\p272_persistence_matrix.tsv`

| ID | Result | Key evidence |
| --- | --- | --- |
| R1 | PASS | Missing Pipeline and `CData` created quiet defaults with no retained failure. |
| R2 | PASS | Distinct two-Step identity, order, and route restored; current `CData` contract has no persisted fields. |
| R3 | PASS | Malformed Pipeline and `CData` produced byte-exact backups and retained substitution failures. |
| R4 | PASS | Wrong-root documents were distinguished from first use and retained as exact backups. |
| R5 | PASS | Exclusive read locks produced retained load failures; source hashes did not change. |
| R6 | PASS | Locked saves produced memory-only/save-failure state; prior disk hashes remained exact. |
| R7 | PASS | Truncated documents were not accepted as the taught definition. |
| R8 | PASS | Failed existing-file replacement preserved both previous valid files byte-for-byte. |
| R9 | PASS | First successful save produced one recovery state and exact reopen; the next ordinary save cleared it. |
| R10 | PASS with documented boundary | Unknown XML elements remain deserializable; no schema/version semantic-staleness rule exists. |

`DataState` is currently an empty `<CData>` contract. P272 proves its
load/save/file-integrity behavior, not restoration of business fields that do
not exist.

## Current EXE Evidence

Before correction:

`artifacts\p272_recipe_pipeline_persistence_20260730\before_current_exe`

- malformed active Pipeline was backed up and substituted with zero Steps;
- Recipe Manager showed `XML OK`;
- no damage/default-substitution explanation was visible.

After correction:

`artifacts\p272_recipe_pipeline_persistence_20260730\after_current_exe`

- the same case shows `XML NG`;
- the warning names the editable default substitution and required reopen
  verification;
- a separate EXE process reopened the same retained invalid canonical file,
  reused the byte-exact backup, and reproduced the failure state before any
  explicit save;
- Korean and English captures passed;
- Tooltip and accessibility HelpText contained full Recipe/Pipeline/path/cause
  and backup context;
- explicit sample Run was disabled;
- recovery appeared once;
- Direct Tool save failure/retry passed with zero execution/layer/route side
  effects.

## Verification

- Embedded current Debug EXE build: zero warnings, zero errors.
- `recipe-pipeline-persistence-feedback --expect-protection true`: PASS,
  including the separate-process reopen probe.
- `recipe-pipeline-roundtrip`: PASS, including WAIT restoration, Pipeline
  Review, selected-Step handoff, pending-edit behavior, zero automatic Run, and
  stable Recipe routing.
- P269 `property-persistence-feedback`: PASS.
- P270 `property-load-feedback`: PASS.
- P271 `settings-persistence-feedback`: PASS.
- Final standard Debug solution build: recorded in the current handoff.
- Readiness and patch-hygiene results: recorded in the current handoff.

## Boundary And Next Priority

- R10 is detection-boundary evidence, not permission to invent schema/version
  semantics.
- `CData` remains empty; P272 does not claim restored Recipe business data.
- This does not provide installer/signing/update support, field qualification,
  certified metrology, or commercial SDK parity.
- CVR-00 remains blocked on three independent first-time participants.
- No feature implementation is active after P272. A productionization track
  requires an explicit distribution decision; algorithm expansion requires a
  named operator task and admission evidence.

## Completion Record

Status: Complete
Scope: Recipe/Pipeline persistence load/save failure identity, exact invalid
backup, fail-closed Recipe feedback, one-time recovery, and Direct Tool retry.
Acceptance criteria: A1–A11 pass with the R10 boundary stated above; A12 is not
applicable because current operator-visible defects were reproduced and
production correction was admitted.
Verification: R1–R10 matrix, current Debug EXE Korean/English before/after,
Direct save failure/retry, Recipe Pipeline round trip, P269–P271 regressions,
standard build, readiness, and diff check.
Evidence:
`artifacts\p272_recipe_pipeline_persistence_20260730` and this report.
Boundary / next dependency: three independent novice participants for CVR-00,
or an explicit distribution/algorithm admission decision.
