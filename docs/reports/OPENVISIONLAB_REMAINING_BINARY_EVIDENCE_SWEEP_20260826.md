# OpenVisionLab Remaining External Binary Evidence Sweep

Date: 2026-08-26 KST  
Repository: `C:\Git\OpenVisionLab_Dev`  
Issue: `PL-0005`  
Scope: exact package/hash provenance checks for retained repository-only DLLs; no deletion, release, commit, or push

## Outcome

The sweep confirmed exact NuGet package binary matches for the two
MaterialDesign DLLs. Their manifest entries are no longer blocked for binary
provenance and now require the remaining third-party NOTICE coverage before
release publication.

The Cyotek ImageBox and SharpGL.SceneGraph DLLs remain blocked. Their upstream
projects and permissive license references are identifiable, but none of the
tested official NuGet package DLLs has the same SHA-256 as the repository copy.
This is not sufficient evidence to approve the exact prebuilt binaries.

Unknown or vendor-specific entries were not changed:

- `dll/CircularProgressBar.dll`
- `dll/EzBasicAxl.dll`
- `dll/Matrox.MatroxImagingLibrary.dll`
- `dll/TabControl.dll`
- `dll/WinFormAnimation.dll`

WPG-CUSTOM is governed separately by the owner-declaration report and is not
included in the remaining third-party blocker list.

## Exact matches

| Repository DLL | Local length | Local SHA-256 | Official package | Package DLL path | Package DLL SHA-256 | License metadata |
|---|---:|---|---|---|---|---|
| `dll/MaterialDesign/MaterialDesignColors.dll` | 316,928 | `F06826845732D945421C341C8D1ABB337AB9A2E757D90A763AC618AA445BF63E` | `MaterialDesignColors 3.0.0` | `lib/net462/MaterialDesignColors.dll` | `F06826845732D945421C341C8D1ABB337AB9A2E757D90A763AC618AA445BF63E` | MIT |
| `dll/MaterialDesign/MaterialDesignThemes.Wpf.dll` | 9,890,304 | `F8144C2D063144A98E6FAA4E4D6F11CB3D08D20313E196CDD03ADDB8186CA6FD` | `MaterialDesignThemes 5.0.0` | `lib/net462/MaterialDesignThemes.Wpf.dll` | `F8144C2D063144A98E6FAA4E4D6F11CB3D08D20313E196CDD03ADDB8186CA6FD` | MIT |

The downloaded package archive hashes were:

- `MaterialDesignColors 3.0.0`: `D52E32D4008DE2419976067837B35E6E681F10751D9260B0EA4F0C022D0257E5`
- `MaterialDesignThemes 5.0.0`: `B2324262010D4EFCA9AB59E86A0C157F3D6D0461EF509CD84047618D9CC41BB1`

The package `.nuspec` files declare the MIT license expression and identify
the MaterialDesignInXamlToolkit project URL. The official project and license
references are [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
and the package pages [MaterialDesignColors 3.0.0](https://www.nuget.org/packages/MaterialDesignColors/3.0.0)
and [MaterialDesignThemes 5.0.0](https://www.nuget.org/packages/MaterialDesignThemes/5.0.0).

## No exact match

`dll/Cyotek.Windows.Forms.ImageBox.dll` has length 80,384, assembly version
`1.2.0.0`, file version `1.4.0.0`, and SHA-256
`AFA43B986A709CEB145DE9F41E2796B73DA037E3FDC6C3C2CFE40A912BAB339B`.
The official `CyotekImageBox 1.2.0` package contains
`lib/net20/Cyotek.Windows.Forms.ImageBox.dll` with SHA-256
`97AD888FC6847F8AEEABDC3A45D0751F3452B672998DC889ED892322DCBE8743`.
The broader stable-version sweep also found no exact hash match. The official
project identifies the ImageBox code as MIT-licensed, but that upstream fact
does not prove the source of this exact repository binary.

`dll/SharpGL/SharpGL.SceneGraph.dll` has length 137,728, assembly and file
version `2.4.0.0`, and SHA-256
`8A67A33D170622907F08260346D16300332F912995EA8D4DA58EEE52D4306A05`.
The official `SharpGL 2.4.0` package contains a SceneGraph DLL with SHA-256
`2E0CBF3A8B94BCDBACE5E813568C8AF6338DCCFF0F11D402528A95B8C89CF09B`, and
the `SharpGL.SceneGraph 2.4.4` package contains one with SHA-256
`82EE9ACAFBC12575B6F01C50468C43E22877C2C8B04EF6FB55725530857F3688`.
Neither matches. The official [SharpGL project](https://github.com/dwmkerr/sharpgl)
and [SharpGL.SceneGraph 2.4.4 package](https://www.nuget.org/packages/SharpGL.SceneGraph/2.4.4)
provide useful upstream identity/license leads, not exact-binary provenance.

## Reproducibility evidence

Generated artifacts are outside the repository on D: under:

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_remaining_binary_evidence_20260826_r1`

- `nuget_binary_hash_comparison.json` records the first exact-version checks,
  including both MaterialDesign matches and the Cyotek 1.2.0 mismatch.
- `packages/` retains the downloaded packages, extracted DLLs, and `.nuspec`
  metadata used for the comparison.

`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_remaining_binary_evidence_20260826_r2`
contains `candidate_package_hash_comparison.json`, which records 28 stable
Cyotek/SharpGL package candidate checks and their no-match result.

## PL-0005 status

```text
Status: Incomplete
Scope: exact package/hash evidence for MaterialDesign, Cyotek ImageBox, and SharpGL.SceneGraph
Acceptance criteria: MaterialDesign exact binary provenance -> PASS; Cyotek/SharpGL exact binary provenance -> BLOCKED; unknown/vendor provenance -> BLOCKED; third-party NOTICE completeness -> BLOCKED
Verification: NuGet flat-container package retrieval, package archive SHA-256, extracted DLL SHA-256, local assembly metadata, exact hash comparison, and stable candidate sweep
Evidence: this report; docs/contracts/openvisionlab/OPENVISIONLAB_EXTERNAL_BINARY_MANIFEST.json; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_remaining_binary_evidence_20260826_r1; D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_remaining_binary_evidence_20260826_r2
Boundary / next dependency: obtain exact provenance or approved removal/source-package evidence for Cyotek, SharpGL, CircularProgressBar, EzBasicAxl, Matrox, TabControl, and WinFormAnimation; complete third-party NOTICE coverage; then run the repository distribution gate. No release, tag, commit, push, or original-repository promotion is implied.
```
