# OpenVisionLab Release Precheck Public Catalog Alignment

Date: 2026-08-26 KST
Repository: `C:\Git\OpenVisionLab_Dev`
Scope: Dev-side precheck routing and Windows PowerShell compatibility for the next release candidate

## Decision

The release platform precheck now accepts an explicit `-CatalogPath` while
retaining the historical legacy catalog as its default. Release instructions
use `docs\samples\OpenVisionLab.PublicSampleCatalog.csv`, which is the
repository-portable catalog and the only catalog suitable for public release
evidence.

The existing `v2.1.0-rc.2` tag and GitHub Release draft remain unchanged. This
Dev change is not part of that frozen candidate and does not authorize a
commit, tag, tag push, release publication, or deployment.

## Root cause

`RunVisionPlatformPrecheck.ps1` previously invoked `RunVisionSampleCatalog.ps1`
without a catalog path, so the sample stage always used the legacy
`OpenVisionLab.SampleCatalog.csv`. That catalog references ignored local
`Sample\...` inputs such as `Sample\Contour.jpg`; those inputs are intentionally
not part of the public distribution.

The release-candidate verifier already used the public catalog explicitly. The
two gates therefore tested different sample contracts.

The precheck script also contained three Korean tutorial terms as UTF-8
literals. The script has no BOM and the release policy invokes Windows
PowerShell 5.1, which parsed those literals as mojibake. The terms are now
constructed from Unicode code points so the same script works under Windows
PowerShell without changing the tutorial files.

## Changes

- `tools\RunVisionPlatformPrecheck.ps1`
  - Added `CatalogPath`, defaulting to the legacy catalog for backward-compatible
    local checks.
  - Passed `CatalogPath` to `RunVisionSampleCatalog.ps1`.
  - Recorded the selected catalog in the human-readable report and top-level
    `platform_precheck_summary.json`.
  - Replaced the three non-ASCII tutorial literals with Windows PowerShell-safe
    Unicode code-point construction.
- `docs\contracts\openvisionlab\OPENVISIONLAB_RELEASE_VERSION_POLICY.md`
  - Release precheck and sample-catalog commands now explicitly select the
    public catalog.
  - Legacy catalog use is documented as an explicit local-only historical check,
    not release evidence.

## Verification

### Focused source check

PowerShell parser validation and a wiring guard passed:

```text
PowerShell parse/encoding guard: PASS
```

### Current Dev full precheck

Command:

```powershell
$env:TEMP='D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2'
$env:TMP=$env:TEMP
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\RunVisionPlatformPrecheck.ps1 `
  -SkipUi `
  -CatalogPath docs\samples\OpenVisionLab.PublicSampleCatalog.csv `
  -OutputDir D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2
```

Observed result:

```text
Status=OK
Gates=13, all Status=OK
SampleCatalogPath=docs\samples\OpenVisionLab.PublicSampleCatalog.csv
SampleGateStatus=OK
RunnableRows=33
RequiredRows=17
OKRows=33
NGRows=0
FailedSamples=0
MetadataIssueCount=0
ArtifactIssueCount=0
Tutorial Portable Contract=Gate=OK
```

The run also passed the vendored DLL check, solution restore/build, Vision UI
Contract, History Contract, Localization Catalog Contract, OpenVision Readiness,
XML Compatibility, WPF shell contract, and the tutorial portable contract.

The first public-catalog run (`..._r1`) reached the same `33/33` sample gate but
stopped at the tutorial contract because Windows PowerShell misread the Korean
source literals. That run is retained as the before-fix failure evidence; the
`..._r2` run is the current after-fix result.

## Evidence

- Top-level summary: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2\platform_precheck_summary.json`
- Human-readable report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2\platform_precheck_report.md`
- Public sample summary: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2\samples\sample_catalog_summary.json`
- Standalone external-reference output: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2\external_references.txt`
- Standalone public-asset output: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2\public_sample_assets.txt`
- Prior encoding failure report: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r1\platform_precheck_report.md`

## Remaining release boundary

This proves the corrected precheck on the current, dirty Dev worktree only. It
does not prove a clean release commit, artifact reproducibility, a new tag, or
publication. The current `v2.1.0-rc.2` candidate remains frozen at its existing
commit and must not be force-moved. A future candidate should be rebuilt from a
clean reviewed commit and receive a new release-candidate identifier.
