# OpenVisionLab Retained Dependency NOTICE Coverage

Date: 2026-08-26 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0005`

## Scope

This report covers every current `dll` manifest entry whose
`repositoryState` is `present` and whose `releasePolicy` begins with `allow`.
It separates third-party notices from the user-owned WPG-CUSTOM declaration.
It does not itself approve a product release, tag, publication, or deployment.
The later user-authorized Dev commit/push boundary is recorded separately in
the current handoff.

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
Scope: PL-0005 retained-dependency NOTICE coverage plus the exact clean Release candidate distribution gate in Dev.
Acceptance criteria: root NOTICE entries and manifest markers -> PASS for all 10 present allow* entries; the third-party groups and WPG-CUSTOM owner declaration are all covered; the complete clean Release candidate gate -> PASS at commit 0292656befc07a79e4e2d43ddd24d386a6b24909.
Verification: TestThirdPartyNoticeCoverage.ps1 -> PASS; VerifyReleaseCandidate.ps1 without -SkipLaunch -> PASS; locked restore; Debug/Release solution builds -> 0 warnings/0 errors; readiness 13/13 -> PASS; external references -> PASS; public sample asset policy -> PASS; 33-row public sample gate -> OK; clean win-x64 framework-dependent publish -> PASS; copied distribution NOTICE, archive/checksum, and actual EXE launch/data-root migration contract -> PASS; PL-0005 ledger validation -> PASS.
Evidence: this report; NOTICE; OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_notice_coverage_20260826\third_party_notice_coverage.txt; C:\Git\OpenVisionLab_Dev\artifacts\release_candidate_20260826_actual_launch\release_candidate_summary.json; C:\Git\OpenVisionLab_Dev\artifacts\release_launch_smoke_20260826_122127; C:\Git\OpenVisionLab_Dev\dist\OpenVisionLab\clean_runtime_manifest.json; C:\Git\OpenVisionLab_Dev\dist\OpenVisionLab-win-x64-framework-dependent.zip; archive SHA-256 11A74CB7D10645FBEEA530FE5E2F13D80049F75435206146473A777CD84837F0.
Boundary / next dependency: Original-repository promotion was completed separately and is recorded in the current handoff and PL-0005 E18/D7. This evidence does not authorize a tag, product release publication, or deployment; the canonical product version remains 2.1.0.
```
