# OpenVisionLab GitHub Clone Release Verification

Date: 2026-08-03 KST

Status: Complete

## Scope

- Push the reviewed recovery and release-path guard changes to the Dev and
  original GitHub repositories.
- Port Dev changes into the original repository without bulk-copying it.
- Clone original `main` onto `D:` and verify build, release packaging,
  copied-location launch, offline manuals, and an actual packaged EXE.

## Repository Results

| Repository | Branch | Final commit | Result |
| --- | --- | --- | --- |
| `Noah8218/OpenVisionLab_Dev` | `codex/public-sample-ux-docs` | `eec754950f6a7482f23a5e408e93fff97605d4e2` | local and remote heads matched after push |
| `Noah8218/OpenVisionLab` | `main` | `f550c4338dbc45bed060096d74e1cad083396ae2` | local and remote heads matched after push |

The recovery change was ported as Dev `0bd4789` -> original `c624390`. The
release-path guard was ported as Dev `eec7549` -> original `f550c43`. The
original README conflict existed because original already named the production
GitHub URL. It was resolved with no final content deviation. Final committed
blob identities matched between Dev and original:

- `README.md`: `88281bd471b0f1dd663ca30a944552968b4076d7`
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` at P284:
  `19d4072f35621d703e13c4969ecbade342d50ff9`
- `tools/VerifyReleaseCandidate.ps1`:
  `06ed86540f950af9632b8aaf22d1e9b8d4c8c573`

## Release-Path Defect And Correction

The first deliberately deep D-drive clone generated a 261-character WPF
intermediate path while Windows `LongPathsEnabled=0`. MSBuild failed while
rebuilding the public-sample runner after the main builds had passed.

`tools/VerifyReleaseCandidate.ps1` now computes the relevant generated path
before restore/build work and fails with an actionable short-checkout message.
`README.md` uses original `main` and a short checkout such as
`C:\src\OpenVisionLab` or `D:\src\OpenVisionLab`. Contract probes confirmed
that the deep path is rejected for the intended reason and a short path reaches
the normal clean-worktree guard.

## Authoritative Final GitHub Clone

- Clone: `D:\OpenVisionLab-TestData\OVL_GitHub_R3`
- Commit: `f550c4338dbc45bed060096d74e1cad083396ae2`
- Command:
  `powershell -NoProfile -ExecutionPolicy Bypass -File tools\VerifyReleaseCandidate.ps1 -OutputDir artifacts\github_clone_release_candidate_20260803_r3`
- Result: `PASS` in 209.798 seconds
- .NET SDK: `8.0.421`
- Debug build: `PASS`
- Release build: `PASS`
- Readiness: `PASS` (13/13)
- External references: `PASS`
- Public sample asset policy: `PASS`
- Public sample gate: `OK`, 33 rows
- Release payload: 78 files, `win-x64`, framework-dependent
- Copied-location launch smoke: `PASS`
- Archive: `dist\OpenVisionLab-win-x64-framework-dependent.zip`
- Archive SHA-256:
  `96B2AE514C68A107F7F9B58A846DEA0AAC8CC4A8A650912E9F0162080224998E`
- Machine summary:
  `D:\OpenVisionLab-TestData\OVL_GitHub_R3\artifacts\github_clone_release_candidate_20260803_r3\release_candidate_summary.json`

## Packaged EXE And Guide Evidence

The R2 packaged EXE was opened and inspected before the final guard-only
commit. The workstation exposed one monitor, `DISPLAY1`, bounds
`(0,0,1920,1080)`; the recorded application rectangle was
`(0,0,1920,1032)`. Accessibility exposed application version `2.1.0`, 17 Tool
entries, the Korean four-step first-use guide, and explicit Preview/Run wording.
The process closed cleanly. R3 independently passed the copied-location launch
smoke.

The packaged schema-2 Guide manifest contained both supported manuals with
matching hashes:

- Korean: `070FD81D92F528B1C672E920E145FF8FAEA85F11F7A45DCB12E17C16ADF2C0CB`
- English: `59B547F9A9D332EAA13B4B27A2CB8CEB9E3DD724D2882752C94A9E394A998D49`

## Acceptance Criteria

- Dev and original changes committed and pushed: PASS.
- Dev-to-original reviewed content preservation: PASS.
- Fresh D-drive GitHub clone at the pushed original commit: PASS.
- Debug/Release, readiness, references, public assets and samples: PASS.
- Framework-dependent package and copied-location launch: PASS.
- Actual packaged EXE and localized Guide inspection: PASS.
- Deep Windows checkout failure made actionable before build: PASS.

## Boundary

This proves source-based, framework-dependent `win-x64` distribution from
GitHub on one restored Windows workstation. It does not prove an installer,
code signing, automatic update/uninstall, self-contained runtime, multi-PC
compatibility, hardware integration, or field qualification.
