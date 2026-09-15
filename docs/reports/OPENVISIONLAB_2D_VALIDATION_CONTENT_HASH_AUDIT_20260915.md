# OpenVisionLab 2D Validation Content-Hash Audit — 2026-09-15

Status: **Complete for the bounded role-aware split owner**

## Scope

2D-024 asked whether the current SampleCheck/N-image/Good-Bad/evaluation entry
already prevents the same image bytes from being counted as independent tuning
and evaluation evidence. The review kept the existing local Validation Set and
PinArrayGap identity owners. It did not add a dataset database, Labeling Studio
integration, perceptual similarity detection, or source-image mutation.

## Findings before the change

| Entry | Existing identity/role behavior | Gap relevant to 2D-024 |
| --- | --- | --- |
| Sample catalog / `VisionPipelineSampleCheckService` | Catalog rows expose `PairGroup`/`PairRole` such as Good/Bad; a plain sample check runs one selected image. | No tuning/evaluation role or source-content hash is inferred from a catalog row. |
| Tool View N-image | `VisionToolNImageVerificationService` retains `SourceSha256` per run row and writes `PairRole=UNLABELED`. | The run is execution evidence, not an automatic tuning/evaluation split; role remains unassigned. |
| General local Validation Set | `OpenVisionRecipeValidationSetImage` stores expected `OK`/`NG`, notes, optional variant data, and a SHA-256 when a frozen identity is created. | Registration deduplicates normalized paths within one set, but has no generic cross-set role or content-overlap policy. |
| PinArrayGap identity | Existing `Train`/`Validation`/`Test` roles, set names, and split `ContentSha256` are frozen and restored. | Before this slice, cross-split disjointness checked paths only; same bytes under different names could pass. |

## Implemented owner and call path

- Owner: `src/OpenVisionLab/UI/Menu/Wpf/Recipe/Validation/OpenVisionRecipePinArrayGapValidationRecordStorage.cs`
- Orchestration: `OpenVisionRecipePinArrayGapValidationIdentityOwner.Freeze` →
  `OpenVisionRecipePinArrayGapValidationRecordStorage.TrySave` →
  `TryCreateRecord` → `TryCreateSplitIdentity`.
- `TryCreateSplitIdentity` computes each current file SHA-256, rejects duplicate
  bytes inside a split, and returns a content-hash set alongside normalized
  paths. `TryCreateRecord` rejects hash overlap across Train/Validation/Test
  with a role-specific error before persisting the record.
- Mutable-state writer remains the existing XML record owner. No new registry,
  service, or partial was introduced.
- The persisted `Role`, `SetName`, `ImageCount`, and `ContentSha256` fields remain
  the observable evaluation identity. `TryMatchesCurrent` marks a renamed set or
  changed source stale without rewriting the saved record.

## Focused evidence

Evidence directory:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d024-content-hash-20260915-run\`

- Debug build: `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Debug --no-restore -v:minimal` — 0 errors; 16 pre-existing nullable warnings.
- Release build: `dotnet build tools/VisionRecipeRunnerSmoke/VisionRecipeRunnerSmoke.csproj -c Release --no-restore -v:minimal` — 0 errors; 16 pre-existing nullable warnings.
- Contract: `--pinarraygap-validation-identity-owner-contract` — **7 passed / 0 failed**.
- The same contract passed **7/7** against the Release smoke binary in
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d024-content-hash-20260915-release\`.
- The contract includes:
  - identical bytes in `Train` and `Validation` with different filenames are rejected by SHA-256;
  - normal freeze/evaluate and frozen split-name restoration remain valid;
  - a Validation set-name/role change becomes stale without rewriting the frozen role record;
  - a changed source image becomes stale;
  - missing Pipeline XML remains a read-only failure and does not rewrite the recipe XML.
- Text evidence: `pinarraygap-validation-identity-owner-contract.txt`.

## Boundaries

- Exact-byte duplicates are detected. Re-encoded or perceptually similar images
  are intentionally not detected and must not be described as duplicates.
- General Local Validation Sets still expose only their existing `OK`/`NG`
  expected outcome; Tool View N-image remains `UNLABELED`. The product has not
  approved a generic tuning/evaluation role vocabulary, so this slice does not
  invent one or assign `Unknown` rows arbitrarily.
- Full WPF/EXE UI, DPI/theme matrix, hardware, clean-checkout release
  distribution, user datasets, and `C:\Git\2D\Original` were not touched or
  verified in this slice.

## Developer reading order

1. `OpenVisionRecipePinArrayGapValidationIdentityOwner.cs`
2. `OpenVisionRecipePinArrayGapValidationRecordStorage.cs`
3. `OpenVisionRecipeValidationSetStorage.cs`
4. `PinArrayGapValidationIdentityOwnerContract.cs`
5. This report and the stable feature contract section above
