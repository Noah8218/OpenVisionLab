# OpenVisionLab WPG-CUSTOM Owner Classification

Date: 2026-08-26 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Scope: PL-0005 classification only; runtime files and project references retained

## Decision

The user selected option 1: keep the WPG-CUSTOM DLL/XML and all PropertyGrid
project/runtime references, but remove WPG-CUSTOM from the PL-0005 external
third-party license blocker. The user explicitly declares that WPG-CUSTOM was
created by the user. This is recorded as an owner declaration for the project
classification; it is not an upstream third-party license claim.

## Applied classification

`docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json`
now records the WPG DLL as:

```text
licenseStatus: user-created-owner-declaration
releasePolicy: allow-with-user-ownership-declaration
```

The DLL remains `runtime-required`, and these files/references were not
removed:

- `dll/System.Windows.Controls.WpfPropertyGrid.dll`
- `dll/System.Windows.Controls.WpfPropertyGrid.xml`
- `src/OpenVisionLab/OpenVisionLab.csproj`
- `src/Libraries/WpfPropertyGridBridge/WpfPropertyGridBridge.csproj`

The WPG DLL remains hash-gated. Its manifest SHA-256 is
`CB418BC1D20FF950AE559FCD8662BEBD519EAFAC3B6C016135B03C89573D1E8B`.

## Remaining PL-0005 boundary

PL-0005 remains open for other retained `blocked` or provenance-incomplete
entries, including CircularProgressBar, Cyotek ImageBox, EzBasicAxl,
MaterialDesign, Matrox, SharpGL.SceneGraph, TabControl, and WinFormAnimation,
plus the remaining third-party NOTICE coverage. This decision does not approve
those files and does not authorize Release.

## Dev commit, push, and version status

- Branch: `codex/public-sample-ux-docs`
- Local HEAD: `827a22e92eba94445e98d1143b94e8d3ea4619b7`
- Dev remote branch: `origin/codex/public-sample-ux-docs` points to the same
  commit.
- The current WPG classification and prior binary-prune changes are working
  tree changes and have not been committed or pushed.
- `C:\Git\OpenVisionLab` was not modified or promoted.
- Canonical product version remains `2.1.0`. No version bump, `v2.1.0-rc.2`
  tag, RC2 release, or deployment was created.

## Verification plan for this classification-only change

- Run `tools\TestExternalReferences.ps1` and confirm WPG remains present,
  hash-matched, and no longer appears as a blocked manifest item.
- Validate the documentation index, JSON manifest, and PL-0005 issue ledger.
- Reuse the current-source Debug build and public-sample evidence from the
  2026-08-25 binary-prune run because no runtime source, DLL, or project
  reference changed in this classification update.

```text
Status: Complete
Scope: Reclassify WPG-CUSTOM as user-created for PL-0005 while retaining its runtime contract.
Acceptance criteria: WPG DLL/XML and project references unchanged; manifest no longer marks WPG as blocked; remaining blocked binaries remain explicit; version/push status recorded.
Verification: TestExternalReferences.ps1, documentation-index validation, JSON parse, PL-0005 ledger validation, and Git remote/status inspection.
Evidence: this report and the manifest.
Boundary / next dependency: PL-0005 remains open for the other retained blocked/provenance-incomplete binaries and NOTICE coverage; no commit, push, release, or deployment is implied.
```

