# OpenVisionLab Optional Binary Prune

Date: 2026-08-25 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Scope: Dev worktree only

## Decision

The user explicitly requested that the unused `Vila.Core` binary and the
optional OpenCV FFmpeg runtime be removed. The user also stated that
`WPG-CUSTOM` was created by the user. WPG-CUSTOM is therefore retained as a
user-provided provenance statement, but that statement is not treated as an
independent source, license, or redistribution-notice record.

No change was made to `C:\Git\OpenVisionLab`, no commit or push was performed,
and no release or publication action was authorized.

## Exact removal scope

Before removal, the two tracked files were recorded in
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1\before_inventory.json`:

| Path | Length | SHA-256 | Current source use before removal |
| --- | ---: | --- | --- |
| `dll/Vila.Core.dll` | 470,528 | `DE0EA89F28C49BEED0F4BCEFE684B8E5FC2D90702C7B1368BC3DCCCD606BA127` | Only the direct project reference in `src/OpenVisionLab/OpenVisionLab.csproj`; no source symbol use found |
| `dll/OpenCVSharp/opencv_ffmpeg400_64.dll` | 18,652,160 | `F68B8A8F2DBFC3036E0065D6CB78DAD121BF4F5DD9AEED86B0560C1FD0D52BF3` | Only the optional ImageCanvas content-copy entry; no `VideoCapture`, `VideoWriter`, or `opencv_ffmpeg` source use found |

The following source changes were made:

- Removed the `Vila.Core` reference from `src/OpenVisionLab/OpenVisionLab.csproj`.
- Removed the `ImageCanvasCopyOpenCvVideoRuntime` default property and the
  `opencv_ffmpeg400_64.dll` content-copy entry from
  `src/Libraries/OpenVisionLab.ImageCanvas/OpenVisionLab.ImageCanvas.csproj`.
- Deleted the two exact DLL paths from the Dev worktree.
- Kept both paths in
  `docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json`
  as `deleted-in-worktree` and `forbidden`. Their HEAD length and SHA-256 are
  retained so the gate fails if either file is reintroduced.

`WPG-CUSTOM` was not removed. Its DLL/XML files remain unchanged and continue
to satisfy the current PropertyGrid build contract.

## Verification

| Check | Result | Evidence |
| --- | --- | --- |
| Debug solution build | PASS; 0 warnings, 0 errors | Command: `dotnet build OpenVisionLab.sln -c Debug -p:Platform="Any CPU"` |
| External reference gate | PASS after manifest records were updated | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1\external_references_after_manifest.txt` |
| Readiness contract | PASS | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1\readiness.txt` |
| Public sample asset gate | PASS; 33 catalog rows, 229 manifest assets, 17 pipelines | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1\public_sample_assets.txt` |
| Public sample catalog | PASS; 33 runnable, 17 required, 16 expected-failure, 33 OK, 0 NG, no failed samples or artifact/metadata issues | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1\public_sample_catalog\sample_catalog_summary.json` and `sample_catalog_report.md` |

The first external-reference run after deleting the binaries correctly failed
because tracked deleted paths were not yet represented in the manifest. The
manifest was then updated with explicit absent/forbidden records, and the
final gate passed. The final gate output contains:

```text
ABSENT | forbidden | dll/OpenCVSharp/opencv_ffmpeg400_64.dll
ABSENT | forbidden | dll/Vila.Core.dll
Vendored DLL check passed.
```

## Closure and boundary

```text
Status: Complete
Scope: Remove the explicitly requested Vila.Core and optional FFmpeg runtime from the Dev worktree and verify the current source/sample gates.
Acceptance criteria: both exact DLL paths absent; active references/copy entries removed; Debug build, external-reference gate, readiness, public assets, and public sample catalog pass.
Verification: dotnet build; TestExternalReferences.ps1; OpenVisionReadinessCheck; TestPublicSampleAssets.ps1; RunVisionSampleCatalog.ps1.
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1 and this report.
Boundary / next dependency: PL-0005 Release admission remains blocked by retained WPG-CUSTOM source/license/NOTICE evidence and other retained blocked or provenance-incomplete binaries. No post-prune Release publish or RC2 publication was claimed.
```

