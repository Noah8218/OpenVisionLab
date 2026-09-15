# OpenVisionLab 2D qualification identity and stale-result audit — 2026-09-15

Status: Complete for the bounded 2D-025 identity/stale contract. The current
WPF target was also rebuilt, but its end-to-end run stopped at the existing
batch-row source-snapshot preflight before a Snapshot was created; that
fixture limitation is recorded below and is not attributed to this slice.

## Immediate priority and product boundary

The scheduled slice was 2D-025: prevent a previous Good/Bad qualification from
being presented as current after the Recipe, selected input bytes, dependency,
or loaded SDK/runtime changes. The existing
`QualifiedRecipeSnapshotStore` remains the qualification owner. No new
qualification database, dataset service, Labeling Studio integration, release,
or deployment path was added.

The product identity remains a Windows x64/.NET 8 OpenCvSharp4 rule-based
recipe workbench. Camera/PLC/I/O integration, cross-platform packaging, and
field or production qualification remain out of scope.

## Owner map and reading order

| Responsibility | Current owner and call path |
| --- | --- |
| Immutable payload and historical integrity | `QualifiedRecipeSnapshotStore.Create/Verify` -> `QualifiedRecipeSnapshotPreflight` -> manifest, inventory, archived Pipeline/Validation Set/summary/rows/dependencies/runtime fingerprint. |
| Current identity decision | `RecipeCommandSurface.VerifyQualifiedSnapshot` -> `OpenVisionRecipeQualifiedSnapshotController.Verify` -> `EvaluateCurrentIdentity` -> store payload verification, current Pipeline/input/dependency recomputation, and current runtime source fingerprint comparison. |
| Current list projection | `RecipeCommandSurface.RefreshQualifiedSnapshotOptions` -> contextual `OpenVisionRecipeQualifiedSnapshotController.List` -> `OpenVisionRecipeQualifiedSnapshotOption.IntegrityState`. |
| Mutable state writer | The selected Recipe/Validation Set editors and existing Run History owners. This slice only reads their current files/values; it does not rewrite them. |
| Lifetime/release owner | `QualifiedRecipeSnapshotStore` owns immutable archive creation and `QualifiedRecipeSnapshotWorkingCopyService` owns editable-copy creation. Historical evidence remains store-owned. |
| Focused verification | `tools/QualifiedRecipeSnapshotSmoke/Program.cs` exercises the core store and current-identity controller through the friend assembly contract. |

Shortest code-reading order:

1. This report and the stable contract section “Qualified Recipe Snapshot Core
   And Run History UI”.
2. `OpenVisionRecipeQualifiedSnapshotController.cs` — `Verify`,
   `EvaluateCurrentIdentity`, and the current validation/runtime comparison.
3. `QualifiedRecipeSnapshotStore.cs` and `QualifiedRecipeSnapshotPreflight.cs` —
   immutable payload identity and historical verification.
4. `RecipeCommandSurface.cs` — contextual list/Verify call path and the
   existing pending-edit/creation gate.
5. `tools/QualifiedRecipeSnapshotSmoke/Program.cs` — mutation/recovery cases
   and generated evidence.

## Findings before the change

The core store already froze the important historical identity:

- exact Pipeline file SHA-256 and Pipeline definition SHA-256;
- ordered Validation Set/input identity and source SHA-256 values;
- archived dependency paths, sizes, and SHA-256 values;
- per-row source/report/Pipeline evidence and review-queue identity; and
- bounded runtime fingerprints for OpenVisionLab, Vision2D, OpenCvSharp, and
  the optional native runtime file.

`QualifiedRecipeSnapshotStore.Verify` correctly failed closed when the
payload or stored runtime fingerprint was invalid while preserving
`PayloadIntegrityValid=true` for a runtime-only drift. The UI controller,
however, treated an intact payload as a successful Verify result and did not
compare it with the currently selected Recipe/Pipeline or Validation Set. A
same-filename input edit or a side-by-side SDK replacement could therefore
leave an old lifecycle record looking current in the panel.

## Bounded implementation

- Added the existing preflight image-set SHA helper as an internal reuse point;
  no new identity service or interface was introduced.
- Added `EvaluateCurrentIdentity` to the existing WPF qualification controller.
  It compares Recipe/Pipeline names and exact current Pipeline bytes, rebuilds
  the selected Validation Set using the actual bytes on disk (not a stale
  stored SHA field), compares ordered input/dependency identity, and compares
  the currently loaded runtime source path/version/size/SHA set.
- `Verify` now returns a blocked/stale result when any required current identity
  differs. It does not mutate lifecycle state, delete evidence, or silently
  restore approval.
- Snapshot list projection now shows `Payload OK / Qualification stale` for a
  current-identity mismatch. `Open evidence` and `Working copy` retain their
  payload-only historical/read-only semantics; working copies still do not
  inherit qualification.
- `RecipeCommandSurface` passes the selected Recipe, Pipeline, Pipeline file,
  and Validation Set into the existing controller call path. It does not add a
  second state owner.

## Acceptance matrix

| Case | Expected contract | Evidence |
| --- | --- | --- |
| Same immutable Snapshot reopened | Current identity matches and Verify succeeds. | Debug/Release `SMOKE_RESULT.txt`, `CurrentIdentitySameSnapshotAccepted=True`. |
| One Pipeline parameter changed | Current Recipe is stale; Verify and new Snapshot creation fail until re-evaluation. | `CurrentRecipeEditMarkedStale=True`, `ReevaluationRequiredBeforeNewSnapshot=True`. |
| Same input filename, changed bytes | Current input identity is stale; old evidence remains readable; new qualification is not created. | `CurrentInputEditMarkedStale=True`, `HistoricalEvidenceReadableWhileStale=True`. |
| SDK/runtime replacement copy | Different loaded runtime path/bytes is stale even when the old archived file remains intact. | `SdkReplacementMarkedStale=True`. |
| Pending Step edit during qualification | Existing `IsSelectedStepEditDirty` preflight gate remains the creation boundary; no new result is restored from dirty state. | Existing WPF qualification fixture source contract plus current preflight call path; the current run stopped earlier at the existing batch source-snapshot check. |
| Historical report/evidence | Payload remains readable and immutable after current identity becomes stale. | Controller evidence-directory checks in the focused smoke. |

## Verification actually run

- `dotnet build tools/QualifiedRecipeSnapshotSmoke/QualifiedRecipeSnapshotSmoke.csproj -c Debug -v:minimal` — pass, 0 warnings, 0 errors.
- `dotnet build tools/QualifiedRecipeSnapshotSmoke/QualifiedRecipeSnapshotSmoke.csproj -c Release -v:minimal` — pass, 0 warnings, 0 errors.
- `QualifiedRecipeSnapshotSmoke` Debug and Release — pass; each created,
  reopened, tampered/restored, stale-mutated, and cleaned its own fixture.
- `dotnet build tools/PipelineViewerScreenshotSmoke/PipelineViewerScreenshotSmoke.csproj -c Debug -v:minimal` — pass, 0 warnings, 0 errors.
- `wpf_shell_host_recipe_qualified_snapshot` Debug target — two fresh attempts
  stopped before Snapshot creation with
  `BLOCKED | Batch row 1: stored source snapshot is missing or changed.` The
  failure is owned by the existing `QualifiedRecipeSnapshotPreflight` batch
  source-artifact check; the only change in that owner for this slice was the
  access modifier on the already-existing image-set hash helper. Therefore the
  changed current-identity UI state was not visually exercised in this run.
- Targeted `git diff --check` — no whitespace errors; Git reported only the
  repository's existing LF/CRLF normalization warnings.

Evidence root:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d025-qualification-20260915`

## Boundary and next dependency

This proves current identity and stale policy through the existing owner and
focused controller/store smoke. It does not prove a full WPF theme/layout/DPI
matrix, camera/SDK hardware parity, field qualification, or release behavior.
The failed WPF target requires a separate repair or fixture decision for the
existing stored-source snapshot preflight; it must not be bypassed by weakening
the qualification contract.

Completion record:

```text
Status: Complete
Scope: 2D-025 current Recipe/input/dependency/runtime identity and stale qualification projection.
Acceptance criteria: same snapshot reopen PASS; Recipe/input/SDK changes stale and require re-evaluation PASS; historical evidence readable PASS.
Verification: QualifiedRecipeSnapshotSmoke Debug/Release PASS; focused WPF target build PASS; WPF runtime target blocked before the changed path by an existing batch source-snapshot preflight failure.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d025-qualification-20260915 and this report.
Boundary / next dependency: full WPF visual matrix and repair/decision for the unrelated stored-source fixture remain unverified.
```
