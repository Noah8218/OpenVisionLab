# OpenVisionLab Qualified Recipe Snapshot Core Implementation

Date: 2026-07-27 KST

## Outcome

The non-WPF `Qualified Recipe Snapshot` core boundary is implemented under
`src\OpenVisionLab\Core\Recipe\Qualification`. It now owns eligibility checks, immutable manifest
identity, self-contained evidence copying, payload inventory, atomic creation,
idempotent same-identity reuse, full reload verification, current-runtime
fingerprint comparison, and terminal supersede/revoke records.

This does not add the Recipe Manager qualification panel. An operator cannot
yet create or manage these snapshots from the product UI.

## Implemented Boundary

### Preflight

`QualifiedRecipeSnapshotPreflight` fails closed unless:

- the Pipeline XML exists, round-trips, has at least one Step, and matches the
  selected Pipeline identity;
- the frozen Validation Set Pipeline text hash, ordered image-set hash, each
  image hash, and every dependency hash match;
- the selected batch is schema-v2 `LocalValidationSet` evidence with exact
  Recipe, Pipeline, set, row-order, expected-role, and source identities;
- every row uses explicit outcome schema v1, completed, and has a correct
  judgment;
- every linked report contains the same Pipeline definition, verified source
  snapshot, and at least one retained drawing/result artifact;
- the deterministic review queue rebuilds to the stored policy/SHA and has no
  execution/evidence gap;
- `InspectionJudgment` contains at least one OK and one NG row, or
  `LocatorStability` contains expected-OK rows only;
- OpenVisionLab, `Lib.OpenCV.dll`, and `OpenCvSharp.dll` fingerprints can be
  captured.

Pending selected-Step edit remains a UI-owned prerequisite and will be checked
by the later Recipe Manager adapter before it calls this core.

### Archive And Identity

The store writes a sibling `.creating-<guid>` directory, reloads and verifies
the completed payload, then renames it atomically to:

```text
QUALIFIED_RECIPE\<SnapshotId>\
```

The archive contains:

- `snapshot.xml`;
- exact `pipeline.xml`;
- serialized `validation-set.xml`;
- `inventory.sha256`;
- copied dependency files;
- original batch `summary.xml` and `summary.tsv`;
- one self-contained report directory per row, including report, Pipeline,
  source snapshot, drawings, results, and any other retained report artifact.

`SnapshotId` is a SHA-256 over the canonical immutable identity, including the
inventory SHA, a stable idempotency key, and creation UTC. A separate stable
idempotency key excludes creation UTC; before final rename the store searches
verified archives for that key, so a repeated request with identical frozen
content and qualification meaning reuses the existing Snapshot instead of
creating a clock-only duplicate. Display label, qualification note,
predecessor/change reason, runtime fingerprints, counts, and evidence rows
remain identity-bearing.

The inventory covers every payload file except `snapshot.xml` and
`inventory.sha256`; the manifest binds the inventory hash and its canonical
identity binds the manifest to the directory ID. Verification also reloads the
batch and per-row report identities instead of relying on file existence alone.

### Runtime State

Payload integrity and current-runtime compatibility are calculated separately:

- `PayloadIntegrityValid` means the immutable archive is complete and unchanged;
- `RuntimeFingerprintMatches` means the current recorded runtime file paths
  still contain the exact frozen bytes;
- the combined verification result fails closed when either is false.

The binaries are fingerprinted, not copied into each Snapshot. This is a local
compatibility record, not a deployment package or digital signature.

### Lifecycle

The implementation uses one create-once XML file per terminal event:

```text
QUALIFIED_RECIPE\lifecycle\<SnapshotId>.events\
  000001_<EventSha256>.xml
```

This deliberately refines the audit's illustrative single
`<SnapshotId>.events.xml` path. A single XML list would require rewriting the
existing file; create-once event files make the append-only application
contract enforceable. Each event binds sequence, UTC, action, reason, related
Snapshot ID, and previous-event hash. V1 permits exactly one terminal action:
`Superseded` or `Revoked`. Supersede requires a different verified successor;
both actions require a reason. No core API deletes or edits a Snapshot payload.

## Focused Evidence

Permanent smoke:

```text
tools\QualifiedRecipeSnapshotSmoke
```

Retained evidence:

```text
artifacts\qualified_recipe_snapshot_core_20260727\final
```

The smoke created a two-row `InspectionJudgment` archive:

- 2 total;
- 1 expected OK / actual OK / correct accept;
- 1 expected NG / actual NG / correct reject;
- zero false accept, false reject, execution error, legacy row, or evidence gap;
- one copied dependency;
- three required runtime fingerprints;
- copied summary/TSV and two complete report/source/Pipeline/drawing directories.

It then proved:

1. initial atomic creation and complete reload;
2. same-identity idempotent reuse;
3. a distinct successor Snapshot with predecessor/change reason;
4. supersede and revoke terminal events plus rejection of a second terminal
   event;
5. lifecycle reason mutation breaks the event hash/sequence verification and
   exact-byte restoration passes;
6. exact payload-byte and manifest creation-time mutation fail integrity
   verification and exact-byte restoration passes;
7. current-runtime byte drift fails combined verification while retaining a
   true payload-integrity state, then passes after exact-byte restoration;
8. a forced dependency archive-name collision after temporary creation fails
   and removes the owned temporary directory without adding a qualified ID;
9. an interrupted `.creating-*` directory is never enumerated as qualified;
10. both archives still verify after deletion of the mutable source Recipe
   directory.

Snapshot IDs from the retained run:

```text
Initial:  F1D98AD79EA5D864FDD3E59FEF3C07E09BB5A6CDED8F93293D5F8364FFA9398D
Revision: 8D721DF93674D328C583AD2739EC861DC110661DE22055616230A617411DBFEE
```

## Verification

Commands actually run:

```powershell
dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU"
dotnet build src/OpenVisionLab/OpenVisionLab.csproj -c Debug -p:Platform="Any CPU" -m:1
dotnet build tools\QualifiedRecipeSnapshotSmoke\QualifiedRecipeSnapshotSmoke.csproj -c Debug -p:Platform="Any CPU" -m:1
dotnet tools\QualifiedRecipeSnapshotSmoke\bin\Any CPU\Debug\net8.0-windows7.0\QualifiedRecipeSnapshotSmoke.dll artifacts\qualified_recipe_snapshot_core_20260727\final
dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug
```

All successful builds completed with zero warnings and zero errors. The smoke
returned `qualified_recipe_snapshot_core=OK`, and the full readiness contract
passed.

One earlier direct tool-project build encountered the repository's shared WPF
BAML output collision. Rebuilding sequentially with the explicit platform
completed cleanly; it was a build-output collision, not a source failure.

## Boundary

- No Recipe Manager UI, qualification note editor, snapshot list, evidence
  opener, working-copy command, or user-facing integrity badge is connected.
- Pending-edit, Preview/Run, layer, active-layer, and route invariants therefore
  have not yet been exercised through qualification controls.
- The smoke evidence is synthetic storage/integrity evidence. It is not
  industrial validation, field qualification, deployment approval, access
  control, or cryptographic signing.
- Runtime fingerprints are bound to exact local binary paths. A future UI must
  distinguish intact historical payload from a changed/currently unavailable
  runtime.

## Completion Record

Status: Complete
Scope: Qualified Recipe Snapshot core preflight, immutable manifest/inventory,
atomic self-contained archive, full verifier, runtime comparison, idempotency,
and append-only terminal lifecycle storage; no UI
Acceptance criteria: Eligible exact evidence creates one verified content ID;
same identity reuses it; payload tamper fails; interrupted temporary state is
not qualified; source Recipe deletion does not break the archive; supersede and
revoke are create-once reasoned events -> all passed
Verification: Debug solution, main project, and focused smoke builds passed with
zero warnings/errors; focused smoke returned
`qualified_recipe_snapshot_core=OK`; readiness contract passed
Evidence:
`artifacts\qualified_recipe_snapshot_core_20260727\final` and this report
Boundary / next dependency: Connect a bounded Recipe Manager Run History panel
and UI adapter for pending-edit check, request construction, list/verify,
evidence open, working copy, supersede, and revoke without Preview/Run or
layer/routing side effects.
