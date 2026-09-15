# OpenVisionLab Dev main clean candidate verification — 2026-09-11

## Status

`Complete` for the last verified Dev main clean-candidate and non-UI release-gate scope.

The verified source is the exact commit already pushed to the Dev repository
`main` branch at the time of this check. No tag, GitHub Release publication, or
deployment was performed.

## Candidate identity

| Item | Value |
| --- | --- |
| Repository | `https://github.com/Noah8218/OpenVisionLab_Dev.git` |
| Target branch | `main` |
| Last verified main commit | `f35994b3c152b056c3649ef2a668083ba407122e` |
| Product version | `2.2.0-dev.2` |
| Target framework | `net8.0-windows7.0` |
| Validation branch | `release-candidate-main-f359-20260911` (local only) |
| Canonical version source | `src/OpenVisionLab/OpenVisionLab.csproj` |

The validation branch was created from the Dev `main` commit after the refactor
promotion and contains no source changes beyond the pushed commits. Its purpose
is to provide the branch identity required by the release manifest; it was not
pushed.

## Checks actually run

`powershell -NoProfile -ExecutionPolicy Bypass -File
tools\VerifyReleaseCandidate.ps1 -SkipLaunch -OutputDir
artifacts\release_candidate_main_f359_20260911` completed with
`ReleaseCandidateVerification=PASS`.

The gate recorded:

- locked restore, Debug and Release solution builds, and readiness: `PASS`;
- vendored external references, retained dependency NOTICE coverage, and public
  sample asset policy: `PASS`;
- public sample catalog: 33 rows, all 33 expected outcomes, no gate failure;
- framework-dependent `win-x64` runtime with 77 payload files;
- archive SHA-256:
  `40A45ED2EA09D1A63172D00A8FF0D26545D4ED981DAE3E112023312A31E00B52`.

The summary and logs are under
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-main-20260911`.

The launch smoke was explicitly skipped. This proves the clean source/build,
publish, manifest, archive, dependency, and public-catalog gates only; it does
not prove desktop EXE startup, monitor placement, alternate DPI/theme rows, or
long-running native shutdown on this exact commit. Those remain separate runtime
verification boundaries.

Hosted GitHub Actions run `34607252607` for the final docs commit `40eb9a03`
failed before any workflow step because GitHub reported failed account payments
or a spending-limit requirement. The same pre-step failure is recorded for
`34606460933`, `34605772634`, and `34604956854`. No hosted build or test ran;
repair the account billing prerequisite before retrying. This does not change
the local gate result.

## Ownership and reading route

The release gate is owned by `tools\VerifyReleaseCandidate.ps1`, which invokes
the existing external-reference, NOTICE, public-sample, clean-runtime, and
distribution checks. No new release service, wrapper, or abstraction was added.

Recommended reading order:

`AGENTS.md` → `docs\admin\OPENVISIONLAB_CURRENT_HANDOFF.md` → this report →
`tools\VerifyReleaseCandidate.ps1` → the output summary JSON.

## Completion record

```text
Status: Complete
Scope: Last verified Dev main clean candidate and non-UI release-gate verification
Acceptance criteria: exact verified commit identified; release gate PASS; public catalog 33/33; archive hash recorded
Verification: VerifyReleaseCandidate.ps1 -SkipLaunch PASS; source/build/readiness/dependency/catalog checks PASS
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\release-candidate-main-20260911\artifacts\release_candidate_main_f359_20260911
Boundary / next dependency: later documentation-only commits do not change product source but must be included in a fresh exact gate before a release tag; hosted CI is blocked by the recorded GitHub billing prerequisite; desktop launch and full WPF runtime matrix remain unverified; tag/release/deployment require separate authorization
```
