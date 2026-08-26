# OpenVisionLab MaterialDesign DLL Prune

Date: 2026-08-26 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0005`  
Scope: user-authorized deletion of the two conditional MaterialDesign DLLs

## Decision and scope

The user authorized deletion of both conditional MaterialDesign DLLs after the
first-candidate prune. Only these two DLL paths were deleted:

- `dll/MaterialDesign/MaterialDesignColors.dll`
- `dll/MaterialDesign/MaterialDesignThemes.Wpf.dll`

The excluded, untracked `BooleanToEyeIconConverter .cs` source file was not
changed. Required Vision SDK, OpenCvSharp, WPG-CUSTOM, FontAwesome.Sharp, and
the required `SharpGL.dll` and `SharpGL.WinForms.dll` files were not changed.

## Deleted files and preserved identity

| Path | Length | SHA-256 | HEAD blob SHA-1 |
|---|---:|---|---|
| `dll/MaterialDesign/MaterialDesignColors.dll` | 316,928 | `F06826845732D945421C341C8D1ABB337AB9A2E757D90A763AC618AA445BF63E` | `cbc79bb159544e2b22fadb356bb1d1b1c1c859b4` |
| `dll/MaterialDesign/MaterialDesignThemes.Wpf.dll` | 9,890,304 | `F8144C2D063144A98E6FAA4E4D6F11CB3D08D20313E196CDD03ADDB8186CA6FD` | `5deca003beb418aadf19b90f476f1d03840d2ef8` |

Before deletion, both files were copied to the D: evidence backup and the
backup SHA-256 values matched the working-tree values. The external binary
manifest now records both paths as `deleted-in-worktree` and `forbidden`, so a
future reintroduction fails the gate.

## Verification and error result

The deletion and validation completed without errors:

```text
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" -> 0 warnings, 0 errors
TestExternalReferences.ps1 -> PASS; both MaterialDesign paths ABSENT | forbidden
OpenVisionReadinessCheck -> PASS
TestPublicSampleAssets.ps1 -> PASS; CatalogRows=33 ManifestAssets=229 Pipelines=17
```

All nine previously repository-only candidate DLLs are now absent and
forbidden: the seven first-candidate files plus these two MaterialDesign files.
The retained runtime DLLs reported `OK`.

## PL-0005 boundary after deletion

Deleting the pair removes their NOTICE/retention decision from the remaining
scope. It does not complete PL-0005 because the root `NOTICE` still does not
provide complete third-party attribution coverage for the retained allowed
dependencies, and the final clean Release distribution gate has not run.

Evidence is stored under:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_materialdesign_prune_20260826`

- `before_inventory.json`
- `backup_before_delete\`
- `external_references_after_delete.txt`

```text
Status: Complete
Scope: Delete the two user-authorized conditional MaterialDesign DLLs from Dev.
Acceptance criteria: exact two paths absent -> PASS; pre-delete hashes and backup retained -> PASS; Debug build -> PASS; external-reference gate -> PASS; readiness and public sample assets -> PASS.
Verification: explicit path deletion, post-delete absence check, dotnet build, TestExternalReferences.ps1, OpenVisionReadinessCheck, and TestPublicSampleAssets.ps1.
Evidence: this report; docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_materialdesign_prune_20260826.
Boundary / next dependency: complete third-party NOTICE coverage for retained allowed dependencies and run the final clean Release distribution gate before closing PL-0005. No commit, push, tag, publication, deployment, or original-repository promotion was performed.
```
