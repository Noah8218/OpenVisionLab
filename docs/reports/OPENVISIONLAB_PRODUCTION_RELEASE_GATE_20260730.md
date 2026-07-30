# OpenVisionLab P273 Production Release Candidate Gate

Updated: 2026-07-30 KST

## Decision

P273 is `Complete` for a reproducible, portable, framework-dependent Windows
x64 Release Candidate gate. This result is a deployable package foundation,
not a claim that OpenVisionLab is a signed installer or commercial GA release.

## Scope

The approved scope was:

- commit and push the current reviewed work;
- clone the pushed repository into a different local path;
- restore and build Debug and Release from committed source only;
- run the current readiness, dependency, public-asset, and public-sample gates;
- produce and verify a portable Release package;
- start the packaged EXE from a copied location;
- prove that the same source commit produces the same payload and ZIP in a
  second independent clone.

Installer authoring, code signing, update/rollback, uninstall, Windows
`Program Files` data-path migration, SBOM/legal approval, and field
qualification were explicitly outside this bounded gate.

## Clean-Clone Defects Reproduced

The initial clean clone exposed release assumptions that a retained developer
workspace had hidden:

- the historical 60-row platform precheck referenced untracked local
  `Sample\...` inputs and failed all rows in a clean clone;
- the application project redirected output to an absolute local path;
- Release builds were not deterministic and still defined `DEBUG`/`TEST`;
- `OpenVisionLab.Mvvm` was absent from the solution and could be reused from a
  Debug output path during Release work;
- the publish folder included PDBs and lacked a complete customer-facing
  manifest/checksum contract;
- CI only exercised the narrower Debug-oriented checks;
- the first ZIP implementation retained wall-clock timestamps, so identical
  payloads could produce different archive hashes.

The repository-backed public catalog is the clean-clone release sample gate:
`docs\samples\OpenVisionLab.PublicSampleCatalog.csv`. It contains 33
repository-portable rows. The local 60-row catalog remains developer evidence
only until all referenced inputs are committed under the public asset policy.

## Implemented Release Contract

- `tools\VerifyReleaseCandidate.ps1` is the one-command clean-clone gate.
- Locked restore, Debug solution build, Release solution build, readiness,
  external references, public asset policy, all 33 public sample rows,
  Release packaging, manifest/hash/archive verification, and copied-location
  EXE launch are required.
- `tools\BuildCleanRuntime.ps1 -Mode Release` creates a framework-dependent
  `win-x64` package, removes symbols by default, includes `LICENSE`, `NOTICE`,
  and deployment instructions, records the clean source identity, hashes all
  payload files, and creates a deterministic ZIP.
- `tools\TestReleaseDistribution.ps1` verifies source identity, every manifest
  hash, required files, absence of PDBs, .NET Desktop 8 runtime contract,
  archive/checksum integrity, and optional copied-location launch.
- `.github\workflows\ci.yml` calls the same release-candidate gate with
  `-SkipLaunch` and uploads the verified portable archive and evidence.
- Release outputs no longer depend on an absolute developer path.
- `OpenVisionLab.Mvvm` is part of the solution and participates in the proper
  Release build.

Canonical command:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifyReleaseCandidate.ps1
```

## Verification Evidence

| Check | Result |
| --- | --- |
| Verified source commit | `38e7eec8188b494b1c3f5d81a82cefa1ee9d19fe` |
| Branch | `codex/public-sample-ux-docs` |
| SDK | `.NET SDK 8.0.421` |
| Clean clone A | `C:\Git\OpenVisionLab_Production_RC_Final_20260730` |
| Clean clone B | `C:\Git\OpenVisionLab_Production_Repro_Final_20260730` |
| Debug solution build | PASS, 0 warnings, 0 errors |
| Release solution build | PASS, 0 warnings, 0 errors |
| Readiness | PASS, 12/12 |
| External references | PASS |
| Public asset policy | PASS, 33 catalog rows, 229 manifest assets, 17 pipelines |
| Public sample execution | PASS, 33/33 |
| Release payload | PASS, 75 files, 0 PDB |
| Copied-location EXE launch | PASS |
| Clone A ZIP SHA-256 | `E8244D5EDF13E3BBE515E4C1F4EAFE0A9695AD11E3591DCF6EAF59236FEEC524` |
| Clone B ZIP SHA-256 | `E8244D5EDF13E3BBE515E4C1F4EAFE0A9695AD11E3591DCF6EAF59236FEEC524` |
| Archive reproducibility | PASS, exact ZIP hash match |

Primary evidence:

- `C:\Git\OpenVisionLab_Production_RC_Final_20260730\artifacts\production_release_candidate_38e7eec\release_candidate_summary.json`
- `C:\Git\OpenVisionLab_Production_RC_Final_20260730\artifacts\release_launch_smoke_20260730_212448`
- `C:\Git\OpenVisionLab_Production_RC_Final_20260730\dist\OpenVisionLab-win-x64-framework-dependent.zip`
- `C:\Git\OpenVisionLab_Production_Repro_Final_20260730\dist\OpenVisionLab-win-x64-framework-dependent.zip`

## Operator Deployment Boundary

The current artifact is a portable folder/ZIP and requires Microsoft .NET 8
Desktop Runtime x64. It currently initializes writable `CONFIG`, `RECIPE`, and
`Log` state beside the executable. Deploy it only to an operator-writable
folder; do not install it under a protected `Program Files` location until
runtime data is moved to an explicit per-user or shared-data root.

The package is unsigned. There is no installer, update/rollback service,
uninstall cleanup contract, signing identity, SBOM approval, third-party legal
review, support bundle, performance qualification, equipment integration, or
field acceptance evidence. Those items remain required before a commercial GA
claim.

## Next Production Priorities

1. Separate immutable installation files from writable user/recipe/log data
   and define migration/backup behavior.
   Recommended model: `gpt-5.6-sol` | Reasoning effort: `high`.
2. Select the distribution model and obtain the code-signing identity and
   certificate before implementing installer, signed binaries, update,
   rollback, and uninstall.
   Recommended model: none until the prerequisites exist; `gpt-5.6-sol`
   afterward | Reasoning effort: none until prerequisites; `high` afterward.
3. Generate and review dependency/SBOM/license evidence, then add an operator
   support bundle and bounded startup/run performance criteria.
   Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.

## Completion Record

Status: Complete

Scope: Reproducible framework-dependent Windows x64 portable Release Candidate
gate from committed source.

Acceptance criteria: clean clone/restore -> pass; Debug and Release solution
build -> pass; readiness/dependency/public-asset/public-sample gates -> pass;
portable package manifest/hash/archive verification -> pass; copied-location
EXE launch -> pass; second-clone byte-identical ZIP -> pass.

Verification: `tools\VerifyReleaseCandidate.ps1` passed in clean clone A;
`tools\BuildCleanRuntime.ps1 -Mode Release` and
`tools\TestReleaseDistribution.ps1 -SkipLaunch` passed in clean clone B; both
archives have SHA-256
`E8244D5EDF13E3BBE515E4C1F4EAFE0A9695AD11E3591DCF6EAF59236FEEC524`.

Evidence: the two clean-clone paths, package paths, launch artifact, and
release-candidate summary listed above.

Boundary / next dependency: This proves a reproducible unsigned portable RC,
not commercial GA. A writable-data-root decision and migration contract are
the next implementation prerequisite; installer/signing also requires an
approved distribution model and signing certificate.
