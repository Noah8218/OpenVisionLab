# OpenVisionLab Beginner Tutorial v2

Date: 2026-08-01 KST

## Scope

- Replace the outdated broad tutorial with one first-use path: public Blob sample, direct Blob Preview, Good/Bad comparison, and a saved `Threshold -> Blob` Recipe.
- Keep Preview and Run Review explicit.
- Make the canonical tutorial easy to find from the repository README, docs index, Learn index, and the application Guide command.
- Generate a single-file portable HTML tutorial.

## Result

- Canonical sources:
  - `docs/learn/OPENVISIONLAB_TUTORIAL.md`
  - `docs/learn/OPENVISIONLAB_TUTORIAL.html`
  - `docs/learn/OPENVISIONLAB_TUTORIAL_PORTABLE.html`
- The tutorial uses the existing verified public Blob drawing
  `docs/assets/tutorial/current/public_blob_particles_good_result.png`
  (`ResultCount=12`) instead of adding screenshots that do not match the
  described workflow.
- P281 closes the copied/distributed Guide gap. The runtime now owns
  `Guide\OpenVisionLab_User_Manual.html` plus a hash manifest, rejects the old
  moved stubs, and keeps repository fallback limited to a verified development
  checkout.
- `docs/LLM_DOCUMENT_INDEX.json` has a dedicated beginner tutorial/Learn route.

## Verification

- `tools/BuildCleanRuntime.ps1 -Mode Dev -OutputDir artifacts\p279_beginner_tutorial_v2_20260801\clean_runtime`: PASS, warnings 0, errors 0.
- `tools/BuildPortableTutorial.ps1`: PASS, one source image embedded and zero relative image paths retained.
- `tools/TestDocumentationIndex.ps1`: PASS, 42 indexed paths, 11 routes, 99 root redirects.
- `dotnet run --project tools/OpenVisionReadinessCheck/OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev`: PASS, including tutorial/Learn and Guide-path contracts.
- `git diff --check`: PASS.
- Clean runtime EXE SHA-256: `810C21910879A1DE79C9F6A97375C88D11338B61B9E49ABDF563A355C2AD3020`.
- Portable tutorial SHA-256: `F301777ED2579C64E1F73BE9F2A57FA1FD35A1A3F0160D44B9888E2478F5B9CB`.
- Generated build/test evidence is physically D-backed through
  `artifacts -> D:\OpenVisionLab_Data\Dev\artifacts`; test TEMP/TMP used
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\temp\p279_beginner_tutorial_v2_20260801`.
- P281 follow-up: the clean runtime build passed with zero warnings/errors,
  103/103 copied distribution files matched by path/length/SHA-256, and direct
  UI Automation invoked the copied EXE's Guide button on the dynamic leftmost
  monitor without a missing/damaged/open-failed dialog.

## Boundary

- No new human-paced tutorial or promotional video was produced. The attempted
  presentation recording is retained only as a recorder diagnostic and was not
  rerun after the user requested faster tests.
- This is documentation and agent-operated contract evidence. It does not
  complete CVR-00, which still requires three independent first-time users.
- It does not qualify Blob, Threshold, or an arbitrary Recipe for production.
- The copied/distributed EXE Guide contract is complete through P281. See
  `docs/reports/OPENVISIONLAB_DISTRIBUTABLE_USER_MANUAL_20260801.md`.

```text
Status: Complete
Scope: Beginner tutorial v2, canonical discovery, portable HTML, and the copied/distributed Guide criterion closed by P281
Acceptance criteria: concise current first-use flow -> pass; every local reference resolves -> pass; portable images embed -> pass; copied/distributed EXE opens the actual Guide -> pass through P281; documentation index and readiness contracts -> pass
Verification: original P279 clean runtime/portable/index/readiness checks PASS; P281 clean runtime build 0 warnings/errors; copied distribution 103/103 path/length/SHA-256 PASS; actual copied EXE Guide smoke PASS
Evidence: docs/learn/OPENVISIONLAB_TUTORIAL.md; docs/learn/OPENVISIONLAB_TUTORIAL.html; docs/learn/OPENVISIONLAB_TUTORIAL_PORTABLE.html; docs/reports/OPENVISIONLAB_DISTRIBUTABLE_USER_MANUAL_20260801.md
Boundary / next dependency: CVR-00 still separately requires three independent first-time participants; no inspection algorithm is qualified by this documentation work
```
