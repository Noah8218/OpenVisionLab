# OpenVisionLab 2.1.0 RC1 Publication

Date: 2026-08-05 KST

## Result

OpenVisionLab `v2.1.0-rc.1` is published as a GitHub pre-release from the
validated original-repository commit
`9ee613676940fd3f593ec45f7e5a96f7a5880e36`.

- Release: <https://github.com/Noah8218/OpenVisionLab/releases/tag/v2.1.0-rc.1>
- GitHub release ID: `365575941`
- Annotated tag object: `b25ac08c232758c771275ed4419673739c6ba012`
- Tag target: `9ee613676940fd3f593ec45f7e5a96f7a5880e36`
- Dev preparation commit: `8ff25ad834f7107592cd0a4a8fe38aec83476196`
- Original release commit: `9ee613676940fd3f593ec45f7e5a96f7a5880e36`
- Application file version: `2.1.0.0`
- Vision SDK: `3.0.0` at
  `ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982`

## Scope

Included:

- Windows x64 portable framework-dependent package;
- Korean and English offline Guide files packaged beside the EXE;
- exact source, package-manifest, ZIP checksum, public-sample, copied-runtime,
  hosted-CI, publication, and public-download verification;
- GitHub pre-release label and five explicit release assets.

Excluded:

- stable/GA designation;
- installer, signing, update, rollback, or uninstall behavior;
- self-contained runtime packaging;
- SBOM/legal review, multi-PC qualification, hardware integration, calibrated
  metrology, or field robustness.

## Verification

The authoritative clean checkout was
`D:\OpenVisionLab-TestData\OVL-RC1-20260805` at the exact original commit.
Test data, generated outputs, publish output, copied launch data, and download
verification remained physically on `D:`.

The full local command was:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifyReleaseCandidate.ps1 -OutputDir artifacts\release_candidate_2_1_0_rc1
```

Result:

- locked restore passed with .NET SDK `8.0.423`;
- Debug and Release builds passed with zero warnings and zero errors;
- readiness passed `13/13`;
- vendored Vision SDK/OpenCvSharp reference hashes passed;
- public sample asset policy passed with 33 catalog rows, 229 manifest assets,
  and 17 Pipelines;
- all 33 runnable sample expectations passed, including 17 required success
  rows and 16 expected-failure rows;
- clean Release publish produced 77 files and no PDB files;
- copied-location EXE launch passed with writable data under the task-local
  `D:` evidence folder;
- GitHub Actions run
  <https://github.com/Noah8218/OpenVisionLab/actions/runs/31013137100>
  passed on the same original commit.

## Published Assets

GitHub reported all five assets as `uploaded`. Each was downloaded again from
its public release URL. Downloaded length and SHA-256 matched both the GitHub
API digest and the pre-publication local asset.

| Asset | Bytes | SHA-256 |
| --- | ---: | --- |
| `OpenVisionLab-2.1.0-rc.1-win-x64-framework-dependent.zip` | 48,965,985 | `0F8851599CC8ABFA51B4828CF414F4E2F7030CCCCD4B1DBECBFA6C2E0535E733` |
| `OpenVisionLab-2.1.0-rc.1-win-x64-framework-dependent.zip.sha256` | 123 | `959C487F378379B1841546F30089C2C2B39D742C0FD641CD5E1F78E20205C3D2` |
| `OpenVisionLab-2.1.0-rc.1-clean-runtime-manifest.json` | 22,469 | `8D9DAF7B40AECB24E1117DD3662EAFB5623EC9C4B90A078FD97563FA1FAE28ED` |
| `OpenVisionLab-2.1.0-rc.1-release-candidate-summary.json` | 785 | `C86617628D3E28D3C54BC191F9E681BBF335B4EFEAB6E4F13E9B3CF8D1679BBF` |
| `OpenVisionLab-2.1.0-rc.1-public-sample-summary.json` | 64,643 | `DD4F87644DC6F2F0335854AA71B74240AD35642357551DD6BF8C7297E5BFF5CB` |

The downloaded ZIP was expanded under
`D:\OpenVisionLab-TestData\OpenVisionLab\release_2_1_0_rc1_20260805\published-download`.
Its EXE reports `2.1.0.0` and product version
`2.1.0+9ee613676940fd3f593ec45f7e5a96f7a5880e36`; its manifest names the same
source commit and 77 payload files. Because the public ZIP hash is identical
to the locally gated ZIP, the copied-location launch evidence applies to the
published bytes without a second presentation run.

## Evidence Location

- Work contract and exact-port plan:
  `D:\OpenVisionLab-TestData\OpenVisionLab\release_2_1_0_rc1_20260805`
- Full local gate output:
  `D:\OpenVisionLab-TestData\OVL-RC1-20260805\artifacts\release_candidate_2_1_0_rc1`
- Copied launch evidence:
  `D:\OpenVisionLab-TestData\OVL-RC1-20260805\artifacts\release_launch_smoke_20260805_230636`
- Published asset round trip:
  `D:\OpenVisionLab-TestData\OpenVisionLab\release_2_1_0_rc1_20260805\published-download`

## Closure

```text
Status: Complete
Scope: OpenVisionLab v2.1.0-rc.1 Windows x64 framework-dependent GitHub pre-release publication
Acceptance criteria: exact validated tag -> pass; full local gate -> pass; hosted CI -> pass; five release assets -> pass; public-download hash round trip -> pass
Verification: VerifyReleaseCandidate.ps1 without SkipLaunch; GitHub Actions 31013137100; GitHub release API; five public downloads with length and SHA-256 comparison
Evidence: GitHub release v2.1.0-rc.1 and the D-drive evidence locations listed above
Boundary / next dependency: this is an unsigned portable pre-release, not installer/signing/update/uninstall, self-contained, multi-PC, hardware, field, or commercial-GA evidence
```
