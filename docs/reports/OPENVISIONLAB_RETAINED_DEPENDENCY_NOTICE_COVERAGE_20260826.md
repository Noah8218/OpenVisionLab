# OpenVisionLab Retained Dependency NOTICE Coverage

Date: 2026-08-26 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0005`

## Scope

This report covers every current `dll` manifest entry whose
`repositoryState` is `present` and whose `releasePolicy` begins with `allow`.
It separates third-party notices from the user-owned WPG-CUSTOM declaration.
It does not approve an external release, tag, commit, push, publication, or
deployment.

The machine-readable file identity, length, SHA-256, reference observations,
source evidence, and release policy remain owned by
`docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json`.
This report proves that each retained allowlisted entry has a stable
`noticeMarker` and that the marker is present in the repository `NOTICE`.

## Coverage matrix

| NOTICE marker | Retained files | Source identity | License / attribution |
|---|---|---|---|
| `FontAwesome.Sharp` | `dll/FontAwesome.Sharp.dll` | Assembly `5.15.3.0`; embedded Font Awesome Free `5.15.1` Regular, Solid, and Brands fonts | FontAwesome.Sharp Apache-2.0; `Awesome Incremented 2015-2020`; embedded Font Awesome fonts use CC BY 4.0 for icons and SIL OFL 1.1 for fonts |
| `OpenCvSharp` | `dll/OpenVisionLab-Vision-SDK/OpenCvSharp.dll`; `dll/OpenCVSharp/OpenCvSharpExtern.dll` | Managed and shared native runtime; exact file identities are in the external binary manifest and `sdk-manifest.json` | Apache-2.0; `Copyright 2008 shimat` |
| `OpenVisionLab Vision SDK` | `OpenVisionLab.Core.dll`; `OpenVisionLab.Vision2D.dll`; `OpenVisionLab.Vision2D.Blob.dll`; `OpenCvSharp.Blob.dll` | SDK `3.0.0`; source commit `ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982` | MIT; `Copyright (c) 2026 최노아(Noah-Choi)` |
| `SharpGL` | `dll/SharpGL/SharpGL.dll`; `dll/SharpGL/SharpGL.WinForms.dll` | Assembly `3.1.1.0`; exact file identities are in the external binary manifest | MIT; `Copyright (c) 2014 Dave Kerr` |
| `WPG-CUSTOM` | `dll/System.Windows.Controls.WpfPropertyGrid.dll` and its XML companion | User-provided owner declaration; exact DLL/XML identity is recorded separately | User-created owner artifact; not a third-party license entry |

## Upstream evidence

- FontAwesome.Sharp: <https://github.com/awesome-inc/FontAwesome.Sharp>
- Font Awesome Free 5.15.1 license: <https://fontawesome.com/license/free>
- OpenCvSharp: <https://github.com/shimat/opencvsharp>
- OpenVisionLab Vision SDK exact source commit: <https://github.com/Noah8218/OpenVisionLab-Vision-SDK/tree/ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982>
- SharpGL: <https://github.com/dwmkerr/sharpgl>
- WPG-CUSTOM owner record: `docs/reports/OPENVISIONLAB_WPG_CUSTOM_OWNER_CLASSIFICATION_20260826.md`

The root `NOTICE` includes the component attribution, license identifiers,
source links, exact SDK identity where applicable, the Font Awesome embedded
asset license split, and the WPG-CUSTOM owner declaration. The root `LICENSE`
continues to provide the repository Apache License 2.0 text and also serves the
same Apache license text required by the Apache-licensed retained components.

## Verification

The new `tools/TestThirdPartyNoticeCoverage.ps1` reads the external binary
manifest and checks every present `allow*` entry. An entry without a
`noticeMarker`, or a marker absent from the selected NOTICE file, fails the
command. `VerifyReleaseCandidate.ps1` checks the repository NOTICE before
publishing; `TestReleaseDistribution.ps1` checks the copied distribution NOTICE
again after publish.

## Completion record

```text
Status: Complete
Scope: Manifest-scoped retained dependency NOTICE coverage definition and implementation.
Acceptance criteria: root NOTICE entries and manifest markers -> PASS for all 10 present allow* entries; the third-party groups and WPG-CUSTOM owner declaration are all covered.
Verification: TestThirdPartyNoticeCoverage.ps1 -> PASS; Debug/Release TestExternalReferences.ps1 -> PASS; Release build -> 0 warnings/0 errors; readiness -> PASS; public assets -> PASS; documentation index -> PASS; ledger validation -> PASS.
Evidence: this report; NOTICE; OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_notice_coverage_20260826\third_party_notice_coverage.txt; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_notice_coverage_20260826\external_references_debug.txt; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_notice_coverage_20260826\external_references_release.txt.
Boundary / next dependency: run the complete clean Release candidate gate from an exact clean source snapshot. No commit, push, tag, release, or deployment is implied.
```
