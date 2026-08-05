# OpenVisionLab GitHub CI UTF-8 Source Repair

Date: 2026-08-05 KST

Status: Complete

## Scope

Convert the five tracked C# files that GitHub Actions could not decode from
CP949 to UTF-8 without BOM. Preserve the exact decoded C# character stream and
each file's existing CRLF/LF layout. No code, comment, identifier, or runtime
behavior change is included.

## Exact Port Mapping

| File | Source byte SHA-256 | UTF-8 byte SHA-256 | Character and newline result |
| --- | --- | --- | --- |
| `src/OpenVisionLab/Common/BitmapDrawing.cs` | `C2571E4072A7DBE4B71C9F51246F9A75DABF03F364965BBE0EB1FB79EE200399` | `0AEC8B6FA7FF85F4909DEF5A0F996316FA90FB100AB7A4646A92E435DA8ADE59` | exact; CRLF 242 -> 242 |
| `src/OpenVisionLab/Common/DefectList.cs` | `EA1F4251B8A26A5D6834251C851DA7671AA121B2F6D8CE84AE1C6215E8998612` | `9262D010CE7BC70DD05767BF2FC9B360D93F64D317D1407DC4DCA0FA7534499F` | exact; CRLF 89 -> 89 |
| `src/OpenVisionLab/Common/DefectListResult.cs` | `0F81781EB3DE97E86154DDE394B46A2044EE7EEB3BC50500D41FDB9482BD2483` | `DADAA6D880335B69E1ACB1933BD2B0BF4B5A224CD3BB6BEEFE817B72664252B7` | exact; CRLF 96 -> 96 |
| `src/OpenVisionLab/Common/ParameterManager.cs` | `4002D9B3034637E4A31B7A2E62F4413C4A31154D3343C3D10E88BFC2D52E0D26` | `4D308E8B8552BC37CDB195FDA736210E787F6595A78D9A7610DB7C627B1F7030` | exact; CRLF 284 -> 284 |
| `src/OpenVisionLab/UI/Menu/Wpf/Views/OpenVisionPipelineReviewViewRenderService.cs` | `C24D24600ABDAB335D45CE246C45F5C367969EF67540EF6EF70D6DB499A2A074` | `B2B7E8A546DC31A938F69A8C006F09FD059C32074F46CD7B8A300C75BD41058D` | exact; CRLF 1 -> 1, LF-only 186 -> 186 |

For every row, strict CP949 decoding of the preserved source bytes and strict
UTF-8 decoding of the repository file produced ordinally equal .NET strings.
The UTF-16 character-stream SHA-256 also matched and no replacement character
was present. The repository-wide strict scan found 0 invalid files among 1,437
tracked text files.

## Verification

The current uncommitted Dev working tree was copied into a new D-drive Git
clone and committed only inside that disposable verification clone. This kept
the Dev repository unstaged and allowed the clean-tree precondition in the
same script used by `.github/workflows/ci.yml`.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File `
  tools\VerifyReleaseCandidate.ps1 `
  -SkipLaunch `
  -OutputDir artifacts\ci_release_candidate
```

Result:

- `ReleaseCandidateVerification=PASS`
- Debug build: PASS, 0 warnings, 0 errors
- Release build: PASS, 0 warnings, 0 errors
- readiness: PASS, 13 contracts
- external references: PASS
- public sample asset policy: PASS
- public sample execution: `OK`, 33 rows
- Release distribution contract: PASS
- runtime: `win-x64`, framework-dependent, 78 payload files
- archive SHA-256:
  `474F84CE0BD66349C4B53017DC224F0BC2A7E0325C724F66CBA1FC3BA0F8E31A`

Evidence:

- original byte backup and manifests:
  `D:\OpenVisionLab-TestData\OpenVisionLab\encoding_migration_20260805`
- clean verification checkout:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ci_utf8_current_20260805`
- release-candidate summary:
  `D:\OpenVisionLab-TestData\OpenVisionLab\ci_utf8_current_20260805\artifacts\ci_release_candidate\release_candidate_summary.json`
- temporary verification commit:
  `f213e98a186297e9d0df633073bc01cf471b8948`

## Boundary / Next Dependency

- Dev commit `28bd8501f659169d02d6c5ccf951419b9feea53b` passed hosted Actions
  run `30995729839`:
  `https://github.com/Noah8218/OpenVisionLab_Dev/actions/runs/30995729839`.
- The same 15 file blobs were verified in original commit
  `a17cfe6bdb48f2e583cc7e9d46fc7afd4dd4bca4`, which passed hosted Actions
  run `30995933851`:
  `https://github.com/Noah8218/OpenVisionLab/actions/runs/30995933851`.
- The temporary D-drive verification commit is not a Dev or original-repository
  commit and must not be promoted by hash.

```text
Status: Complete
Scope: Five tracked CP949 C# sources converted to UTF-8 without BOM with exact character and newline preservation
Acceptance criteria: 5/5 exact ports PASS; tracked-text strict UTF-8 scan 1437/1437 PASS; local GitHub-equivalent release-candidate gate PASS
Verification: VerifyReleaseCandidate.ps1 -SkipLaunch completed in 133.091 seconds with Debug/Release/readiness/references/public samples/package PASS
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab\encoding_migration_20260805 and D:\OpenVisionLab-TestData\OpenVisionLab\ci_utf8_current_20260805\artifacts\ci_release_candidate\release_candidate_summary.json
Boundary / next dependency: This proves the repository CI/build/package scope; installer, signing, hardware, and multi-PC qualification remain separate scopes
```
