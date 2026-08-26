# OpenVisionLab First-Candidate DLL Prune

Date: 2026-08-26 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0005`  
Scope: user-authorized deletion of the seven first-candidate repository-only DLLs; MaterialDesign conditional candidates retained

## Decision and scope

The user authorized deletion of the seven DLLs previously listed as the first
deletion candidates. The two separately listed conditional MaterialDesign DLLs
were not deleted. No runtime-required DLL, WPG-CUSTOM DLL/XML, source project
reference, original repository, commit, push, tag, or release was changed.

## Deleted files

| Path | Length | SHA-256 | HEAD blob SHA-1 |
|---|---:|---|---|
| `dll/CircularProgressBar.dll` | 17,920 | `DF4AC6198A8D31FCB1E0CE89E99750BB02377A87F3342B4F9157C47589669DA9` | `2556abf927f0934140195fcf2b02000f1cb14af7` |
| `dll/Cyotek.Windows.Forms.ImageBox.dll` | 80,384 | `AFA43B986A709CEB145DE9F41E2796B73DA037E3FDC6C3C2CFE40A912BAB339B` | `54267843459602a2d8208b90f356896fc62df25f` |
| `dll/EzBasicAxl.dll` | 113,664 | `EC2F82EAEB10A99DBF8878D9F3B413C652654D85861FEE84A637AF656964E672` | `5461eea7adc37e593e4f8c27e707482832dffc53` |
| `dll/Matrox.MatroxImagingLibrary.dll` | 1,590,784 | `D821E99D4D094ACDFDD18B75FB8771CF2E483271A8F08DC1B5E50CF8B8EFA572` | `a71dd0390cfc9db58f92512dc61947743903f015` |
| `dll/SharpGL/SharpGL.SceneGraph.dll` | 137,728 | `8A67A33D170622907F08260346D16300332F912995EA8D4DA58EEE52D4306A05` | `3fb55afe50d0563ce8d20995e61828d3c7fb718b` |
| `dll/TabControl.dll` | 49,152 | `13CB16CB2CA969710D1A860BDE5ABF32DB8D156CF61E379444B7FAEC517301B8` | `90c14291f7060203436fa72ac914227a0915d1dc` |
| `dll/WinFormAnimation.dll` | 38,400 | `D9052729B560D819C8D75149B6CA92C48B9E1B1B0CCAA50080A74166DB8EAA12` | `e2655ee3e5964c0510dd8d1ddfb4fcbf420853c2` |

Before deletion, each file was copied to the D: evidence backup and its local
working-tree hash was verified against the backup hash. The manifest now marks
each path `deleted-in-worktree` and `forbidden`, preserving the HEAD identity
so reintroduction fails the external-reference gate.

## Retained conditional candidates

These were intentionally not touched in this operation:

- `dll/MaterialDesign/MaterialDesignColors.dll`
- `dll/MaterialDesign/MaterialDesignThemes.Wpf.dll`

Their exact official NuGet binary matches remain recorded, but their future
retention/removal decision and third-party NOTICE coverage are separate from
this seven-file prune.

## Verification and errors

The deletion command completed without an error. The post-delete checks passed:

```text
dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU" -> 0 warnings, 0 errors
TestExternalReferences.ps1 -> PASS; all seven deleted paths ABSENT | forbidden
OpenVisionReadinessCheck -> PASS
TestPublicSampleAssets.ps1 -> PASS; CatalogRows=33 ManifestAssets=229 Pipelines=17
```

The retained runtime DLLs reported `OK`, including the OpenVisionLab Vision SDK,
OpenCvSharp native runtime, WPG-CUSTOM, FontAwesome.Sharp, and both required
SharpGL runtime DLLs. `SharpGL.SceneGraph.dll` was removed; `SharpGL.dll` and
`SharpGL.WinForms.dll` were retained.

Evidence is stored under:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_first_candidate_prune_20260826`

- `before_inventory.json`
- `backup_before_delete\` with the seven recoverable pre-delete binaries
- `external_references_after_delete.txt`

```text
Status: Complete
Scope: Delete the seven user-authorized first-candidate repository-only DLLs from Dev.
Acceptance criteria: exact seven paths absent -> PASS; pre-delete hashes and backup retained -> PASS; Debug build -> PASS; external-reference gate -> PASS; readiness and public sample assets -> PASS.
Verification: explicit path deletion, post-delete absence check, dotnet build, TestExternalReferences.ps1, OpenVisionReadinessCheck, and TestPublicSampleAssets.ps1.
Evidence: this report; docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_first_candidate_prune_20260826.
Boundary / next dependency: MaterialDesign conditional candidates remain; third-party NOTICE coverage and the PL-0005 distribution gate are still required before Release. No commit, push, tag, publication, deployment, or original-repository promotion was performed.
```
