# OpenVisionLab release precheck — 2026-09-11

## Status

Precheck complete for the current Dev working tree. The source-build and
platform gates passed, but this is not a release commit, tag, push, or
publication. `TagReady` remains false because the working tree contains the
approved but uncommitted refactor batch and documentation changes.

## Baseline

| Item | Value |
| --- | --- |
| Repository | `C:\Git\2D\Dev` |
| Remote | `https://github.com/Noah8218/OpenVisionLab_Dev.git` |
| Branch | `codex/public-sample-ux-docs` |
| HEAD | `0a77e60e444b12757565ee977216a994446b53a6` |
| Target framework | `net8.0-windows7.0` |
| Product version | `2.2.0-dev.2` |
| Evidence | `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-review-recheck-20260911\release-precheck` |

The worktree was already dirty before this precheck. The precheck did not
stage, rewrite, revert, commit, push, tag, publish, deploy, or touch
`C:\Git\2D\Original`.

The checked-out branch is `codex/public-sample-ux-docs`, while the requested
main target would be `origin/main`. `origin/main` is at
`e551f307d4150d414cb24f7a8de23dece9b9c23c`; the current HEAD is
`0a77e60e444b12757565ee977216a994446b53a6`, with merge base
`1528d3b869ce67f439ac28fc2b8565cde58c28b2`. A direct push of the current
branch to `main` would therefore require an explicit merge/rebase or a separate
promotion worktree. That history decision was not guessed or performed in this
precheck.

## Checks actually run

`tools/VerifySourceBuild.ps1 -OutputDir artifacts\source_build_verification_r28_20260911`
passed with locked restore, Debug/Release solution builds, readiness, vendored
DLL checks, and both expected executables. The summary is at
`C:\Git\2D\Dev\artifacts\source_build_verification_r28_20260911\source_build_summary.json`
through the repository's D-backed artifacts junction.

`tools/RunVisionPlatformPrecheck.ps1` ran in Release/Any CPU mode with the
repository-portable public catalog and `-SkipUi`. Its summary is
`platform/platform_precheck_summary.json` under the evidence root. The result
was `Status=OK` after 144.057 seconds:

- build, Vision UI, History, Localization, XML, Readiness, Runner API, Tool
  Result, Sample Inventory, and Tutorial Portable gates: `OK`;
- public catalog: 33 runnable rows, 33 OK rows, 0 NG rows, 0 failed samples,
  0 artifact issues, 0 metadata issues;
- WPF shell contract artifacts: Preview, Workspace, Workspace Output, Native
  Tool, Pending Tool, ROI editor, Image Compare, Log Panel, and Localization
  captures all reported `OK`.

`tools/NewOpenVisionReleaseEvidence.ps1` produced
`openvisionlab_release_evidence_20260911_193312.json`. It reports
`ReleaseGateOk=True`, but `TagReady=False` and 155 changed files because the
worktree has not been committed. This is the required stop boundary before a
release tag or push.

The precheck left twelve `MSBuild.dll`/`VBCSCompiler.dll` node-reuse processes
after their parent verification process ended. Their exact PIDs and command
lines were checked, then those orphaned build nodes were stopped. A later
process check found zero `OpenVisionLab`, smoke-runner, or readiness processes.
This is verification-host cleanup evidence; no product shutdown path was
changed.

## Release boundary

The canonical `VerifyReleaseCandidate.ps1` clean-clone gate was not claimed for
this dirty checkout. It requires a clean tracked worktree and a verified exact
commit. A release artifact, version bump, tag, branch push, release draft, and
deployment therefore remain unperformed. The current version remains
`2.2.0-dev.2`; no version was changed merely to count refactor commits.

The public catalog still records 14 uncovered sample folders as `Backlog`. The
current gate treats those as documented uncovered inventory, not failed public
rows; broadening coverage is a separate product decision.

## Completion record

```text
Status: Complete
Scope: Current Dev source-build and platform release precheck
Version / channel: 2.2.0-dev.2 / development candidate
Repository / branch / commit: C:\Git\2D\Dev / codex/public-sample-ux-docs / 0a77e60e444b12757565ee977216a994446b53a6
Canonical version source: src/OpenVisionLab/OpenVisionLab.csproj
Tag: none
Artifacts / SHA-256: no distributable release artifact produced; precheck summaries and WPF/sample evidence are under the evidence path
Verification: VerifySourceBuild PASS; RunVisionPlatformPrecheck PASS; NewOpenVisionReleaseEvidence ReleaseGateOk=True, TagReady=False
Release state: none
Deployment: not authorized / not performed
Rollback: N/A; no externally visible release bytes were created
Evidence: D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-review-recheck-20260911\release-precheck
Authorization boundary: read-only precheck and local build evidence; commit/push/tag/release/deployment not performed
Boundary / next dependency: resolve the exact target branch and promotion history, then review a clean exact commit before any branch push or tag
```
