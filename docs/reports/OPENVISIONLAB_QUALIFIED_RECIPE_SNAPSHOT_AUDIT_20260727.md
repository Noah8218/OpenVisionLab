# OpenVisionLab Qualified Recipe Snapshot Audit

Date: 2026-07-27
Source commit: `9d7fa796ed94d90e50d840607b441a2954278947`
Workspace: `C:\Git\OpenVisionLab_Dev`

## Decision

OpenVisionLab has most of the evidence pieces needed for Recipe qualification,
but it does not yet have a general qualification record.

The current product can:

- save a mutable Recipe and Pipeline;
- protect pending selected-Step edits with explicit Apply/Discard/Cancel;
- hash-lock one Validation Set to an exact Pipeline, dependencies, and images;
- execute expected OK/NG rows and retain per-image Pipeline/source/drawing
  evidence;
- save a deterministic batch review queue.

It cannot yet state, with one durable and fail-closed identity:

> This exact Pipeline and dependency set, executed by this exact runtime, passed
> this exact expected-outcome image set, and these are the retained reports and
> drawings that support that statement.

The selected next implementation is therefore a bounded
`Qualified Recipe Snapshot v1`. It must be an application-managed immutable,
self-contained evidence archive outside the mutable Recipe workspace. It must
not be presented as production, field, or metrology qualification.

The audit originally attributed a result-semantics defect to the Local
Validation Set handler's `Success` reassignment. Deeper implementation tracing
after this document was first written showed that
`VisionPipelineSampleCheckService.Success` was already expected-outcome
normalized; the Local handler was reversing that normalization to recover the
actual Pipeline result. The persisted Local result was therefore not proven
wrong by that line alone.

The underlying design risk was still real: the same `Success` name carried
validation correctness in the execution service and actual Pipeline outcome in
the Local batch row. This implicit conversion was too ambiguous for a
qualification boundary. The completed follow-up implementation now stores
execution state, actual outcome, expected outcome, and judgment correctness
separately while preserving the old aggregate field for compatibility.

## Post-Audit Implementation Status

The prerequisite outcome contract was completed later on 2026-07-27:

- batch summary schema v2 and row outcome schema v1 persist explicit fields;
- `Success` remains the legacy aggregate validation result;
- Run History and review queue use explicit actual/expected/judgment fields;
- execution errors are no longer classified as false accepts/rejects;
- legacy summaries still load through a read-only fallback and remain
  distinguishable from new explicit rows.

Implementation and verification evidence:

- `docs\reports\OPENVISIONLAB_VALIDATION_OUTCOME_CONTRACT_IMPLEMENTATION_20260727.md`
- `artifacts\qualified_recipe_outcome_contract_20260727\final`

## Product Boundary

The qualified snapshot belongs to the deterministic rule-based Recipe
workbench. It records operator-supplied inspection intent, fixed Pipeline/tool
parameters, fixed evidence, and explicit validation results.

It does not add:

- camera, lighting, PLC, I/O, MES, account, or deployment control;
- electronic signatures, access-control roles, or regulatory certification;
- automatic Recipe activation;
- autonomous inspection design or LLM expansion;
- a claim that a finite evidence set proves unseen production robustness.

Preview and Run remain explicit actions. Snapshot preflight, creation, viewing,
revocation, superseding, and working-copy creation must not run a Pipeline,
create or delete layers, change the active layer, or mutate input/output routes.

## Current Capability And Gap Matrix

| Capability | Current evidence | Assessment for qualification |
| --- | --- | --- |
| Mutable working Recipe | `RecipeWorkspaceService` stores and can duplicate, rename, or delete a whole `RECIPE\<name>` directory. | Keep as the editable authoring workspace; do not treat it as an immutable release. |
| Pending-edit protection | The 2026-07-27 Recipe Change Safety slice centralizes Apply/Discard/Cancel for Step, Pipeline, Recipe, and manager-close transitions. | Sufficient authoring safety prerequisite. |
| Pipeline round trip | Recipe Manager applies, saves, reloads, and validates XML before leaving a dirty edit session. | Reuse as a promotion preflight, but qualification still needs exact content identity. |
| Validation Set identity | `OpenVisionRecipeValidationSetStorage` supports Pipeline definition SHA-256, dependency SHA-256 values, image SHA-256 values, and an ordered image-set SHA-256. | Strong reusable input identity. A snapshot copy must be frozen even when the working set remains editable. |
| Locked locator handoff | P235 promotes one all-success Matching-family session to an exact hash-locked expected-OK locator set. | Reuse the identity pattern. It proves locator execution stability only, not defect classification. |
| Per-image evidence | `VisionPipelineRunReportStorage` saves `pipeline.xml`, a source snapshot and SHA-256, Step parameters/metrics/objects, and retained drawings. | Strong row-level evidence. Copy and verify it inside the snapshot. |
| Batch evidence | `VisionPipelineBatchRunSummaryStorage` saves rows, counts, review policy, review entries, and `ReviewQueueSha256`. | Useful aggregation, but not a qualification authority by itself. |
| Batch Pipeline snapshot | The storage API can save one when `pipelineSnapshot` is supplied. Current Recipe Manager Local Validation Set and other general suite calls omit it. | Confirmed gap. Promotion must require or reconstruct and verify one exact batch-level Pipeline identity. |
| Expected versus actual result | The completed follow-up now stores execution state, expected outcome, actual outcome, and judgment correctness separately while retaining legacy `Success` compatibility. | Prerequisite complete. Snapshot promotion can require explicit outcome rows and reject legacy ambiguity. |
| Partial-run state | Stopped Local Validation Set runs use `LocalValidationSetPartial` and a partial note. | Reuse as a fail-closed ineligibility rule. |
| Baseline comparison | Run History dynamically selects a compatible prior batch for comparison. | A useful analysis choice, not a stored approval, release, or qualification state. |
| Runtime provenance | Some historical evidence records EXE/DLL versions and hashes, but the general batch/Recipe contract does not freeze them. | Add a bounded first-party/runtime fingerprint to the snapshot. |
| Immutable qualified identity | No general snapshot ID, payload inventory, qualification gate, or exact Pipeline+set+evidence binding exists. | Primary missing capability. |
| Lifecycle | No general qualified/superseded/revoked state or append-only reason record exists. | Add status events without rewriting snapshot payload. |
| Safe editable derivative | Whole working Recipes can be duplicated, but there is no “create working copy from this qualified snapshot” operation. | Add an explicit copy action; never edit a qualified payload in place. |

## Result-Semantics Clarification

The pre-implementation flow was:

1. `VisionRecipeRunResult.Success` represented the raw Pipeline outcome.
2. `VisionPipelineSampleCheckService.Success` normalized that outcome against
   `ExpectedFailure` and expected metric checks.
3. The Local Validation Set handler reversed the expected-NG normalization so
   its persisted `Success` could be displayed as the actual Pipeline result.
4. Other batch consumers used `Success` as an aggregate validation result.

The Local UI result was semantically recoverable, but the contract depended on
knowing which layer had already inverted the Boolean. Execution errors also
shared the same `false` value as a controlled NG result. Qualification must not
depend on that implicit knowledge.

The implemented prerequisite model therefore has unambiguous fields:

| Field | Meaning |
| --- | --- |
| `ExecutionState` | `Completed`, `Error`, or `Cancelled`; never inferred from judgment. |
| `ExpectedOutcome` | Operator-owned `OK` or `NG`. |
| `ActualOutcome` | Pipeline-owned `OK` or `NG`; absent when execution did not complete. |
| `JudgmentCorrect` | Derived only as `ExpectedOutcome == ActualOutcome`. |
| `ErrorCode` / `ErrorMessage` | Exact fail-closed execution or evidence error. |

Legacy summary loading derives the old display where possible, but legacy
ambiguous rows are not eligible for qualification. Qualification requires a new
schema record produced after this separation.

## Commercial Lessons To Emulate

The useful commercial pattern is separation between editing, saving,
validation, and runtime selection:

- Cognex In-Sight jobs require explicit save decisions, and a `.jobx` contains
  the full job configuration. Cognex audit logging separately records job and
  system changes such as save/load and backup/restore. OpenVisionLab should
  emulate explicit lifecycle actions and traceable identity, not Cognex sensor
  online/offline or equipment-control scope.
- MVTec MERLIC stores recipes as separate files that reference an MVApp and its
  parameter values. Imported recipes must be valid, and changed recipe/MVApp
  information must be explicitly re-imported or reloaded. OpenVisionLab should
  similarly distinguish a mutable working definition from an explicitly
  qualified snapshot and fail closed when referenced content changes.

Official references:

- Cognex, [Managing Spreadsheet Jobs](https://support.cognex.com/docs/isvs_500/web/EN/InSight_Sheet/Content/Topics/EZB/getting-started-managing-jobs.htm)
- Cognex, [Job Files](https://support.cognex.com/docs/is2d_2311/web/EN/InSight_Sheet/Content/Topics/DeployProject/projectfiles.htm?TocPath=Deployment%7C_____1)
- Cognex, [Audit Logging](https://support.cognex.com/docs/is-usp_2440/web/EN/InSight_Sheet/Content/Topics/HowTo/audit-log-utility.htm?TocPath=How+To+Use+Spreadsheet%7C_____2)
- MVTec, [MERLIC Recipe Files](https://www.mvtec.com/doc/merlic/5.7/manual/en-us/Content/RTE/Setup/Recipes/merlic_recipes.html)
- MVTec, [Importing MERLIC Recipes](https://www.mvtec.com/doc/merlic/26.03/manual/en-us/Content/RTE/Setup/Recipes/import_recipe.html)

## Qualified Recipe Snapshot V1 Contract

### Qualification Scope

Every snapshot must declare one scope. The scope is part of the immutable
identity and visible in every qualified label.

| Scope | Required evidence | Allowed claim |
| --- | --- | --- |
| `InspectionJudgment` | At least one expected OK and one expected NG row; all rows completed; zero false accepts, false rejects, runtime errors, and evidence gaps. | “Qualified for this frozen OK/NG validation set.” |
| `LocatorStability` | One or more expected-OK locator rows; every row completed with usable locator output; zero runtime errors and evidence gaps. | “Locator execution qualified for this frozen expected-success set.” |

No minimum N beyond the semantic requirement is invented in v1. The UI must
show row counts and must not shorten either claim to “production qualified”.
The P235 all-OK set can only use `LocatorStability`.

### Frozen Identity

The following content is immutable after snapshot creation:

1. snapshot schema version, scope, ID, creation UTC, display label, and required
   operator qualification note;
2. source Recipe and Pipeline names as historical labels;
3. exact serialized `pipeline.xml` and SHA-256;
4. copied Pipeline dependencies with original logical path, archived relative
   path, size, and SHA-256;
5. frozen Validation Set definition, ordered expected roles, notes, image
   identities, image-set SHA-256, and set-definition SHA-256;
6. completed batch summary with separate expected/actual/correctness fields;
7. every per-image report, source snapshot, retained result/drawing artifact,
   and their inventory hashes;
8. review queue policy, entries, and verified queue SHA-256;
9. qualification counts: total, expected OK/NG, correct accept/reject, false
   accept/reject, execution error, and evidence-gap counts;
10. bounded runtime fingerprint: OpenVisionLab executable version/hash,
    `Lib.OpenCV.dll` version/hash, `OpenCvSharp.dll` version/hash, and
    `OpenCvSharpExtern.dll` hash when present;
11. optional predecessor snapshot ID and change reason when superseding.

Machine name, OS/.NET version, and original absolute paths may be retained as
diagnostic metadata, but they are not a substitute for content hashes.

### Storage Boundary

Store v1 snapshots outside mutable Recipe directories so Recipe rename/delete
cannot silently delete qualified evidence:

```text
QUALIFIED_RECIPE\
  <SnapshotId>\
    snapshot.xml
    pipeline.xml
    validation-set.xml
    inventory.sha256
    dependencies\
      <sha256>_<original-name>
    evidence\
      summary.xml
      summary.tsv
      runs\
        0001_<sample-key>\
          report.xml
          pipeline.xml
          source.*
          drawings-and-results.*
  lifecycle\
    <SnapshotId>.events\
      000001_<EventSha256>.xml
```

`SnapshotId` is the SHA-256 of a canonical identity manifest that excludes
mutable lifecycle events. `inventory.sha256` covers every payload file by
archive-relative path and content hash. Creation uses a temporary sibling
directory, verifies a complete reload and inventory, then atomically renames
the directory to the final ID. A failed creation leaves no qualified entry.

Lifecycle events are stored outside the hashed payload. They are create-once,
append-only application records and contain UTC, action, required reason,
related snapshot ID when applicable, and the previous event hash. Separate
event files refine the initial single-XML illustration so appending never
rewrites a prior event. This is tamper-evident local evidence, not a
cryptographic signature or access-control system.

No UI command deletes a snapshot payload in v1. External filesystem mutation
is detected by verification and changes the displayed integrity state to
`Tampered or incomplete`; filesystem hashes do not make the files physically
write-proof.

### State Model

```text
Working Recipe (mutable)
  -> Eligible run
  -> Qualified snapshot (immutable payload)
       -> Superseded by <new snapshot>
       -> Revoked with required reason

Qualified snapshot
  -> Create working copy (new mutable Recipe; snapshot unchanged)
```

State rules:

- `Working` is never itself relabeled as an immutable object.
- `Eligible` is computed, not stored: current preflight and selected batch must
  pass every gate.
- `Qualified` belongs to the exact snapshot ID, not merely to a Recipe or
  Pipeline name.
- `Superseded` and `Revoked` do not rewrite or delete the qualified payload.
- A working Pipeline may display `Matches snapshot` only when its Pipeline and
  dependency hashes match. Runtime fingerprint match is shown separately.
- Any Pipeline/dependency change yields `Changed since snapshot`; it does not
  revoke historical evidence.
- A revoked or integrity-failed snapshot cannot be used as the source of a
  “qualified” badge, but its evidence remains viewable.

### Promotion Preflight

`Create qualified snapshot` is enabled only when all applicable checks pass:

1. no selected-Step pending edit exists;
2. current Pipeline round-trip validation passes;
3. a named Local Validation Set and a completed, non-partial
   `LocalValidationSet` batch are selected;
4. batch Recipe/Pipeline/set identity matches the current selection;
5. the Pipeline, every dependency, every validation source, every per-row
   Pipeline snapshot, and every stored source snapshot exists and matches its
   expected hash;
6. batch row count, order, expected role, and source identity exactly match the
   frozen Validation Set copy;
7. every row uses the new unambiguous result schema;
8. `InspectionJudgment` has both OK and NG evidence and zero incorrect/error/gap
   rows, or `LocatorStability` satisfies its bounded all-success gate;
9. the deterministic review queue rebuilds to the stored SHA-256;
10. runtime fingerprint capture succeeds;
11. the operator enters a non-empty qualification note and confirms the
    displayed scope/boundary statement.

Preflight reads evidence only. It must not rerun stale rows to make them
eligible. If evidence is missing or changed, the operator must explicitly run a
new Validation Suite first.

### UI Placement And Operator Workflow

Use the existing Recipe Manager `Run History` tab because qualification consumes
a selected completed batch and its review evidence. Do not add another top-level
workspace or navigation system.

Minimum panel:

- selected batch and Validation Set identity;
- qualification scope selector with full claim text;
- preflight checklist with exact fail reasons;
- counts for expected/actual/correctness/error/evidence gaps;
- Pipeline, dependency, image-set, review-queue, and runtime fingerprints;
- required qualification note;
- explicit `Create qualified snapshot`;
- read-only qualified snapshot list with integrity/lifecycle status;
- `Open evidence`, `Verify integrity`, `Create working copy`, `Supersede`, and
  `Revoke` actions.

`Revoke` requires confirmation and a non-empty reason. `Supersede` creates a new
snapshot first and only then appends the relation event. `Create working copy`
must choose a new Recipe name and copy the frozen Pipeline/dependencies without
copying a qualified badge.

Example operator-facing summary:

```text
Scope: Inspection judgment
Identity: Pipeline 6A91… / image set 02F4… / runtime C818…
Evidence: 48 rows (36 OK, 12 NG), 48 completed
Judgment: 36 correct accepts, 12 correct rejects, 0 false accepts,
          0 false rejects, 0 execution errors, 0 evidence gaps
Claim: Qualified only for this frozen validation set
Action: Create qualified snapshot
```

## Minimum Implementation Order

1. **Separate actual outcome from judgment correctness — Complete**
   - introduce the new batch row schema;
   - migrate presentation and review-queue logic;
   - keep legacy loading, but mark ambiguous legacy rows ineligible;
   - add a four-outcome OK/NG matrix and a real Local Validation Set integration
     smoke that checks stored fields, UI judgment, counts, and review reasons.
2. **Add the qualification domain and storage boundary — Complete**
   - new cohesive owner under `src\OpenVisionLab\Core\Recipe\Qualification`;
   - manifest/inventory models, preflight use case, atomic archive writer,
     full verifier, and lifecycle event store;
   - no WPF types or command-surface private state in the domain owner.
   - Evidence:
     `docs\reports\OPENVISIONLAB_QUALIFIED_RECIPE_SNAPSHOT_CORE_IMPLEMENTATION_20260727.md`
     and `artifacts\qualified_recipe_snapshot_core_20260727\final`.
3. **Connect the bounded Recipe Manager panel — Complete**
   - consume one selected completed Local Validation Set batch;
   - create/view/verify/copy/supersede/revoke explicitly;
   - preserve Preview/Run, layers, active layer, and routes.
4. **Prove tamper and lifecycle behavior — Complete**
   - pass/fail preflight matrix;
   - Pipeline/dependency/source/report/drawing/runtime hash mismatch cases;
   - interrupted temporary creation and reload;
   - qualified -> working-copy -> changed-since-snapshot;
   - qualified -> superseded/revoked without payload mutation;
   - fresh current-build before/after UI captures.

Do not combine this work with Recipe history, crash recovery, installer work,
new algorithms, LLM expansion, or external deployment.

## Acceptance Criteria For V1

1. The result-semantics ambiguity is removed and all four expected/actual
   outcomes persist and render correctly.
2. Only a completed exact Local Validation Set batch with complete evidence can
   pass preflight.
3. The archive is self-contained and still verifies after the source working
   Recipe is renamed or deleted.
4. Any payload file change, deletion, path substitution, wrong runtime
   fingerprint, or review-queue mismatch fails integrity verification with the
   exact reason.
5. Snapshot creation is atomic and idempotent for the same identity.
6. Qualified payload files are never edited in place by product commands.
7. Supersede and revoke are append-only lifecycle events with required reasons.
8. A working copy is editable but has no inherited qualified status; exact and
   changed identity states are distinguished.
9. Every qualification control preserves explicit Preview/Run and
   layer/routing contracts.
10. Focused domain tests, current-build UI smoke, solution build, and fresh UI
    evidence pass.

## Deferred From V1

- digital signatures, certificates, user accounts, permissions, and electronic
  approval workflows;
- remote audit servers, cloud synchronization, and shared-repository locking;
- installer/export package signing;
- automatic activation, rollback, scheduling, or machine deployment;
- retention quotas and archive deletion UI;
- semantic comparison or automatic migration across runtime versions;
- production capability indices or statistical qualification claims;
- automatic minimum sample-count recommendations.

These may be reconsidered only after a concrete operator or regulatory need is
supplied. They are not prerequisites for a useful local evidence-qualified
Recipe workflow.

## Source Evidence

- `src\OpenVisionLab\Core\Recipe\RecipeWorkspaceService.cs`
- `src\OpenVisionLab\UI\Menu\Wpf\Recipe\Validation\OpenVisionRecipeValidationSetStorage.cs`
- `src\OpenVisionLab\UI\Menu\Wpf\NativeTools\Review\VisionToolNImageValidationPromotionService.cs`
- `src\OpenVisionLab\UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.Handlers.cs`
- `src\OpenVisionLab\Core\Pipeline\Storage\VisionPipelineRunReportStorage.cs`
- `src\OpenVisionLab\Core\Pipeline\Storage\VisionPipelineBatchRunSummaryStorage.cs`
- `src\OpenVisionLab\UI\Menu\Wpf\OpenVisionShellHostRecipeCommandSurface.RunHistory.cs`
- `src\OpenVisionLab\UI\Menu\Wpf\Recipe\Models\OpenVisionRecipeSampleRunModels.cs`
- `tools\PipelineViewerScreenshotSmoke\Program.cs`
- `docs\contracts\openvisionlab\OPENVISIONLAB_TOOL_VIEW_N_IMAGE_VERIFICATION_DESIGN.md`
- `docs\reports\OPENVISIONLAB_VALIDATION_SUITE_RESULT_ARCHIVE_DESIGN_20260707.md`
- `docs\reports\OPENVISIONLAB_RECIPE_CHANGE_SAFETY_IMPLEMENTATION_20260727.md`

## Completion Record

Status: Complete
Scope: Current-source Qualified Recipe Snapshot capability, data-identity,
lifecycle, UI-placement, and commercial-reference audit; no product
implementation
Acceptance criteria: Existing evidence capabilities and gaps were traced to
source; the result-semantics ambiguity was identified and its initial cause
statement was corrected after deeper tracing; v1 frozen identity, scope,
storage, lifecycle, preflight, UI workflow, implementation order, and acceptance
gates were defined
Verification: Source and current contract documents were inspected; official
Cognex and MVTec documentation was checked; report paths and stated storage
call sites were re-searched in the current working tree
Evidence: This report and the source files listed above
Boundary / next dependency: This audit itself made no product changes. Its
result-semantics prerequisite and qualification domain/storage boundary were
subsequently completed; see
`OPENVISIONLAB_VALIDATION_OUTCOME_CONTRACT_IMPLEMENTATION_20260727.md` and
`OPENVISIONLAB_QUALIFIED_RECIPE_SNAPSHOT_CORE_IMPLEMENTATION_20260727.md`.
The Recipe Manager qualification panel and UI adapter are now complete. See
`OPENVISIONLAB_QUALIFIED_RECIPE_SNAPSHOT_UI_IMPLEMENTATION_20260727.md` and
`artifacts\qualified_recipe_snapshot_ui_20260727`. No additional feature is
selected without a reproduced current-source operator blocker or regression.
