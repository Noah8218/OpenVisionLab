# OpenVisionLab Current Project Handoff History Through 2026-08-30

Archived: 2026-08-30 KST, before the successful Dev path flattening

This file preserves the former cumulative handoff, including its uncommitted
2026-08-30 path-migration notes, for historical lookup. It is not current-status
authority. Use `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` for live status and
priority; stable behavior belongs to `docs/contracts`, and task evidence belongs
to `docs/reports`.

## Current Product Identity

- OpenVisionLab is an OpenCvSharp4 rule-based vision recipe workbench.
- The normal workflow is sample image -> PropertyGrid teaching -> Pipeline
  composition -> explicit Preview/Run -> drawing/metric/layer comparison ->
  N-sample validation -> saved Recipe.
- LLM XML authoring is optional maintenance-mode assistance. It is not the
  product core and is not a prerequisite for operation.
- Camera, lighting, PLC/I/O, account, MES, deployment-controller, and field
  integration scope remains excluded unless the user explicitly changes the
  product direction.

## Evidence-Based Maturity

- The bounded deterministic workbench workflow is broadly connected through
  Tool Views, Pipeline, Pipeline Review, Recipe Manager, Validation Sets, Run
  History, public samples, drawings, and saved reports.
- The localized offline Guide is complete for Korean and English with the same
  26 chapters, 17 Tool entries, numbered current-UI figures, language-at-click
  routing, and fail-closed manifest/hash validation.
- A clean GitHub clone of original `main` passes the framework-dependent
  `win-x64` Release candidate gate on the restored Windows workstation.
- The Dev source-build policy supports .NET SDK `8.0.100+` in the `8.x` line
  and the `9.x` SDK bundled with current Visual Studio 2022. Full gates passed
  with `8.0.100`, `8.0.300`, and `9.0.316` plus the required .NET 8 runtimes.
  Installed Visual Studio 2022 17.14.37 with the `.NET desktop development`
  workload also built the solution with zero warnings and zero errors.
- The five CP949 source files that blocked GitHub Actions are now exact-ported
  to UTF-8. A clean D-drive snapshot and the hosted GitHub Actions runs for
  both Dev and original passed the Release candidate gate.
- Dev's released-baseline dependency remains the manifest-verified OpenVisionLab
  Vision SDK 3.0 at source commit `ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982`;
  the predecessor DLL root and duplicate managed OpenCvSharp files are removed.
  Standalone SDK, app Debug/Release, functional contracts, focused UI, and
  clean-runtime launch checks pass for that baseline.
- The current Dev worktree also carries a bounded, not-yet-promoted Vision SDK
  object-candidate candidate at source commit
  `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`. Blob/Contour publish one-pass
  candidate evidence and Dev consumes it without the former audit rerun. The
  bounded C4 Blob-mask, Blob/Contour Multi-ROI, object-candidate,
  selection/drawing, timing, Run History, and public-sample parity is now
  resolved in Dev; Contour mask classification and broader external-consumer
  shape/selection scope remain outside that contract. Evidence is recorded in
  `docs/reports/OPENVISIONLAB_PL0010_C4_PARITY_REPLAY_20260827.md` and the
  separate Viewer boundary in
  `docs/reports/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_CONTRACT_20260827.md`.
- OpenVisionLab `v2.1.0-rc.1` is published from original commit
  `9ee613676940fd3f593ec45f7e5a96f7a5880e36` as an unsigned Windows x64
  framework-dependent GitHub pre-release. Its full local gate, hosted CI,
  copied launch, five uploaded assets, and public-download SHA-256 round trip
  pass.
- A fresh 2026-08-23 Dev analysis passed Debug build, readiness 13/13,
  external-reference hashes, 33-row/229-asset/17-Pipeline public samples,
  focused runtime/object-audit/MultiMatchMean contracts, and a public
  LineDistance run. The production Pipeline and direct native UI Preview/
  Auto MPoint SDK Tool lifetime slices are now creator-owned and verified in
  Dev. ImageSpaceFrame Borrow/TakeOwnership, owned OpenCvSharp Canvas file
  loading with Emgu removal, exact 4512 store/viewer/texture process-resource
  gates, one frozen-Recipe 1,000-run plateau, coordinated display-store/
  central/docked/popout viewer lease retirement, native OpenGL readback, and
  a per-process GPU allocation plateau are also complete in Dev. This evidence
  still excludes every possible OpenGL fault injection, current actual-EXE
  theme/DPI/monitor, multi-PC, arbitrary-duration, and field qualification.
- This is not commercial GA, installer/signing/update evidence, multi-PC or
  hardware qualification, calibrated metrology, or field robustness.

## Active 2.1.0 RC2 Hardening Direction

- The user-authorized 2026-08-25 shared GPT Pro review has been reconciled
  against current Dev rather than adopted from its older public-main baseline.
  The decision record and paste-ready Luna restart are in
  `docs/reports/OPENVISIONLAB_SHARED_GPT_PRO_ANALYSIS_RECONCILIATION_20260825.md`.
- The focused Native Tool smoke slice is now a known current-build baseline;
  `PL-0003` is resolved with current localized signal-inspector, generated
  PropertyGrid, and stable evidence-state assertions. Its evidence is recorded
  in `docs/reports/OPENVISIONLAB_NATIVE_TOOL_FOCUSED_SMOKE_BASELINE_20260825.md`.
- The previously broad reliability bundle is now separated into grouped
  pushed Dev commits, each with one responsibility group. The branch is clean
  at the latest recorded commit and matches
  `origin/codex/public-sample-ux-docs`; the grouped
  commit list and boundaries are recorded below.
- `PL-0005` is resolved in Dev for the reviewed external-binary scope. The
  user-authorized prune removed the unused `Vila.Core.dll`, optional
  `opencv_ffmpeg400_64.dll`, seven first-candidate DLLs, and the two
  MaterialDesign DLLs; WPG-CUSTOM remains under the explicit owner declaration.
  Retained NOTICE coverage passes for all 10 present `allow*` manifest entries.
  The complete clean Release candidate gate passed at commit
  `0292656befc07a79e4e2d43ddd24d386a6b24909`: Debug/Release builds have zero
  warnings/errors, readiness is 13/13, the public sample gate is 33/33, the
  copied win-x64 framework-dependent distribution contract passes, and the
  actual copied-runtime EXE launch/data-root smoke passes twice. The reviewed
  commits have now also been promoted to original `main` at
  `4b92289133a593356c74b22505714a1f91112380`; product release, tag,
  publication, and deployment remain separate authorization boundaries.
- `PL-0006` BitmapImageConverter memory safety is complete in Dev. `PL-0007`
  Recipe/Pipeline storage path containment is also complete in Dev; its current
  path-boundary, lifecycle, sample/report, qualified-snapshot, and public-runtime
  evidence is recorded in
  `docs/reports/OPENVISIONLAB_RECIPE_PIPELINE_STORAGE_PATH_BOUNDARY_20260825.md`.
- `PL-0008` immutable original/effective Pipeline provenance is complete in Dev;
  the execution-copy boundary, single normalization pass, report/batch identity,
  legacy report compatibility, and qualified-snapshot compatibility evidence is
  recorded in
  `docs/reports/OPENVISIONLAB_PIPELINE_EXECUTION_PROVENANCE_20260825.md`.
- `PL-0009` recoverable Pipeline persistence is complete in Dev. Journal-backed
  rename/delete recovery, atomic active-pointer replacement with inventory
  validation, fail-closed recovery state, and normal lifecycle/reopen evidence
  are recorded in
  `docs/reports/OPENVISIONLAB_PIPELINE_PERSISTENCE_RECOVERY_20260825.md`.
- With `PL-0009` resolved, `PL-0010` was measured at its external SDK boundary
  and is now resolved for the bounded C4 Dev parity replay. The coordinated
  candidate contract remains available locally at SDK commit
  `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`; the Dev manifest and vendored
  Release DLLs are aligned, and the old audit rerun has been removed. The
  current mask/multi-ROI/selection/timing/drawing/Run History/public-sample
  evidence is recorded in
  `docs/reports/OPENVISIONLAB_PL0010_C4_PARITY_REPLAY_20260827.md`.
  `PL-0008` and `PL-0009` are no longer open implementation priorities.
- `PL-0011` coordinates the exact RC2 gate. Source version remains `2.1.0`;
  normal issue commits do not bump it. The verified `v2.1.0-rc.2` tag and
  GitHub Release draft now exist; publication and deployment remain separate
  authorization boundaries.
- Use `gpt-5.6-luna` for high-risk implementation in this train. The issue
  records contain exact acceptance criteria and the recommended reasoning
  effort. The user explicitly authorized the grouped Dev commit/push operation
  recorded below; future pushes remain separate explicit boundaries.
- Existing ImageSpace/viewer lifetime (`PL-0004`), Emgu removal, and OpenGL GPU/
  coordinate reliability evidence remain complete and are not reopened by the
  shared review.
- The pre-existing remaining product priority is still `CVR-00`, externally
  deferred until three independent first-time participants and their unedited
  observations exist. The RC2 hardening train does not convert agent-operated
  recordings into novice evidence.

## Active Repository Path Migration

- The user requires actual repository directories, not junction-only aliases,
  under `C:\Git\2D`. The managed 2D scope is exactly `Dev`, `Original`, and the
  management `README.md`; unrelated Labeling, 3D, SDK, Machine Studio, and YOLO
  repositories remain outside this scope.
- `C:\Git\OpenVisionLab` was moved by same-volume directory rename to the
  physical `C:\Git\2D\Original`. Directory file ID
  `0x00000000000000000003000000043296`, HEAD
  `cc5340b46461cb52a505380c8a7c7711f983905c`, branch `main`, origin, the
  existing untracked `Temp.txt`, and all three linked worktree states matched
  before and after the move. The linked worktree Git administrative paths were
  repaired to the new main-worktree location.
- Dev physical relocation is blocked only by the currently running Codex task:
  Windows denied DELETE access on the root itself with error 32, and both
  `Move-Item` and direct `Directory.Move` failed. The rollback restored
  `C:\Git\2D\Dev` as a junction to the unchanged
  `C:\Git\OpenVisionLab_Dev` directory.
- Required prerequisite: fully exit Codex so the old Dev root handle is
  released, run the D-drive `Complete-DevRepositoryMove.ps1`, then restart
  Codex and open `C:\Git\2D\Dev`. Do not claim the migration complete until
  `C:\Git\OpenVisionLab_Dev` is absent and `C:\Git\2D\Dev` is a physical
  directory with matching file ID and Git state.

## Latest Completed Work

### 2026-08-28 Dev Versioned Checkpoint Push - Complete In Dev

- The user authorized an intermediate commit/push of the Dev worktree, split
  by the product/component version identity. The target is
  `C:\Git\2D\Dev`, branch `codex/public-sample-ux-docs`, remote
  `OpenVisionLab_Dev`; the original `C:\Git\2D\Original` and
  `C:\Git\OpenVisionLab-Machine-Studio` repositories were not changed.
- The canonical application version remains `2.1.0`; normal development
  commits did not bump it. The grouped identities are `0.2.0-alpha.1` for the
  2D exchange contract, `3.0.1-dev.20260826.candidate.2` for the SDK candidate
  contract, and `2.1.0` for the application/UI/teaching slices.
- The exact pushed commit sequence, verification commands, D-drive evidence,
  and authorization boundaries are recorded in
  `docs/reports/OPENVISIONLAB_DEV_VERSIONED_PUSH_CHECKPOINT_20260828.md`.
  The current remote head is `67b43e0053353937ab150afd5bdaf5feaadec036`.
- The 2D exchange, SDK object-candidate parity, teaching batch, PowerShell
  script parsing, Pipeline Review/N-image, ImageCanvas, locator, and docking
  checks passed within their bounded scopes. No performance test, PC restart,
  tag, Release publication, deployment, or original-repository mutation was
  performed for this checkpoint.
- `.proofline/drafts/` remains an intentionally untracked release-draft
  workspace and is not part of the Dev source checkpoint.

### 2026-08-25 PL-0003 Native Tool Focused Smoke Baseline - Complete In Dev

- `tools/PipelineViewerScreenshotSmoke/Program.cs` now checks the current
  localized `VisionToolSignalInspectorView` text surface for Range evidence,
  keeps the marker/value/Preview and no-side-effect assertions, removes the
  Basic Threshold dependency on the transient evidence-cue lifetime, and
  checks the current Korean generated EdgeBasedMatching PropertyGrid labels
  after restoring advanced-row visibility without triggering Preview.
- No product UI behavior or Tool runtime code changed; this was test-contract
  maintenance only.
- Current-build focused WPF smoke passed in two combined runs, two isolated
  Basic Threshold runs, two isolated Range Threshold runs, and one isolated
  EdgeBasedMatching Auto MPoint run. The durable issue record is
  `.proofline/issues/PL-0003.json`.
- Evidence and runtime boundary are recorded in
  `docs/reports/OPENVISIONLAB_NATIVE_TOOL_FOCUSED_SMOKE_BASELINE_20260825.md`.

### 2026-08-25 PL-0007 Recipe/Pipeline Storage Path Boundaries - Complete In Dev

- `RecipeWorkspaceService` now owns strict storage-segment validation and
  `Path.GetFullPath` root/child containment before Recipe/Pipeline filesystem
  mutation. Traversal, reserved Windows device names, separators, control
  characters, trailing spaces/periods, and case collisions are covered.
- Pipeline XML, active/config/data files, image/run/sample/batch artifacts,
  validation records, dependency copies, template captures, report evidence,
  and qualified working-copy paths use the shared policy. Legacy absolute
  evidence paths remain read-compatible only where required; new artifacts are
  relative and contained.
- The current contract passed Recipe/Pipeline CRUD, sample-set
  save/load/context/delete, Run Report and Batch Summary save/list, qualified
  snapshot lifecycle/tamper/runtime checks, and public Matching replay.
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_current_20260825_r4\recipe_storage_path_contract.txt`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0007_qualified_snapshot_20260825_r2\SMOKE_RESULT.txt`,
  and the report above. The durable issue record is `.proofline/issues/PL-0007.json`.

### 2026-08-25 PL-0008 Pipeline Execution Provenance - Complete In Dev

- `VisionPipelineExecutionPlan` now creates a serializable effective copy,
  normalizes that copy exactly once, and keeps the caller/object or source XML
  bytes unchanged. `VisionRecipeRunner`, Pipeline Review, sample validation, and
  the compatibility execution entry point all use the prepared copy.
- Run Reports retain schema version, `pipeline.original.xml`, effective
  `pipeline.xml`, original/effective SHA-256, structured property-level
  normalization changes, application identity, Vision SDK identity, and SDK
  manifest identity/hash. Batch summaries carry the same identity/change set.
- Qualified snapshot preflight now binds the qualification definition to the
  original snapshot and separately verifies the effective snapshot hash. Legacy
  reports without the new fields remain readable.
- Current evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\pipeline_provenance_contract.txt`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_current_20260825_r3\completion.txt`,
  and `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0008_qualified_snapshot_20260825_r1\SMOKE_RESULT.txt`.
- Boundary: this preserves/report execution identity but does not obtain the
  external DLL license/provenance evidence required by `PL-0005`, and it does
  not authorize release, original-repository promotion, commit, or push.

### 2026-08-25 PL-0009 Recoverable Pipeline Persistence - Complete In Dev

- `VisionPipelineStorage` now journals Pipeline rename/delete lifecycle stages,
  preserves a validated rollback copy, and adopts a completed state only when
  the source/target/fallback/pointer inventory is proven valid.
- Active Pipeline pointer writes use a temporary-file replacement and reject a
  name outside the current valid Pipeline inventory. New Recipe creation now
  creates/loads the default Pipeline before writing that pointer.
- The current focused contract injected six rename failures and five applicable
  delete failures, then cleared runtime state and reopened the storage path.
  Every case rolled back byte-identically or adopted a proven completed state;
  no journal, backup, or temporary artifact remained.
- A dedicated current-source WPF target also rendered the localized recovered
  lifecycle state in Recipe Manager and verified no Preview/Run, layer,
  document, or route mutation. The separate Recipe Manager Review target still
  has an existing explicit-review assertion failure and is not used here.
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r2\pipeline_persistence_recovery_contract.txt`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0009_current_20260825_r8\ui_smoke\wpf_shell_host_pipeline_lifecycle_recovery.png`,
  and
  `docs/reports/OPENVISIONLAB_PIPELINE_PERSISTENCE_RECOVERY_20260825.md`.
- Boundary: the restart check is a same-process reopen simulation, not
  multi-process crash/power-loss, installer, multi-PC, or field qualification.
  The separate Recipe Manager Review target remains an existing UI smoke
  failure and is not used as PL-0009 evidence.

### 2026-08-25 PL-0010 Blob/Contour Audit Baseline - Blocked at SDK Boundary

- The focused current-source contract warmed and measured the primary and
  relaxed audit Tool calls for representative Blob and Contour cases. Both
  cases returned candidate IDs `1..5`; current review evidence retained five
  rows, one accepted row, four exact reject reasons, and one accepted overlay.
- Source inspection confirms the current audit failure behavior: an audit
  exception or unsuccessful result becomes an empty audit list and the caller
  falls back to accepted-only evidence. The product audit path was not
  changed.
- The vendored SDK `3.0.0` result types expose geometry/measurement fields and
  `Index`, but not applied limits, accepted state, or reject reason. The
  one-pass replacement therefore remains blocked until an updated SDK result
  contract and manifest are supplied and app parity is proven.
- Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010_baseline_20260825_r4\audit_baseline.tsv`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010_baseline_20260825_r4\observations.txt`,
  and `docs/reports/OPENVISIONLAB_BLOB_CONTOUR_AUDIT_BASELINE_20260825.md`.
- Boundary: no SDK DLL, manifest, stable object-review behavior, or release
  state was changed; no one-pass parity, SDK release, or production-corpus
  performance qualification is claimed.

### 2026-08-25 PL-0011 RC2 Read-Only Preflight - Blocked

- The canonical application version remains `2.1.0`; the intended candidate
  identity remains `v2.1.0-rc.2`. At the preflight capture Dev was at
  `827a22e92eba94445e98d1143b94e8d3ea4619b7` on
  `codex/public-sample-ux-docs`, with 101 tracked changes and 26 untracked
  files. It is not an exact release-candidate commit boundary.
- The read-only `VerifyReleaseCandidate.ps1` probe stopped before restore or
  package generation with `Release candidate verification requires a clean
  tracked working tree.` No release output was produced by the probe. Existing
  `dist` and `artifacts` roots are also present and must not be reused as RC2
  evidence.
- PL-0005 remains the external DLL license/provenance release blocker.
  PL-0010 remains blocked at the SDK result-contract boundary with an explicit
  defer decision. PL-0006 through PL-0009 are resolved in Dev but are not by
  themselves an approved release commit.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_RC2_PREFLIGHT_20260825.md` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0011_rc2_preflight_20260825_r1`.
- Boundary: no version edit, original-repository change, commit, push, tag,
  draft, publication, deployment, cleanup, or reset was performed. RC2 gate
  execution resumes only after PL-0005 evidence, the explicit PL-0010
  include/defer decision, an exact clean candidate, and separate release-stage
  authorization are present.

### 2026-08-25 PL-0005 Optional Binary Prune - Complete in Dev; Release Still Blocked

- Per the user's explicit request, `dll/Vila.Core.dll` and
  `dll/OpenCVSharp/opencv_ffmpeg400_64.dll` were removed from the Dev
  worktree. The unused `Vila.Core` project reference and optional ImageCanvas
  FFmpeg content-copy entry were removed as well.
- The external binary manifest keeps both exact paths as
  `deleted-in-worktree`/`forbidden` with their HEAD length and SHA-256 so a
  later reintroduction fails the gate. WPG-CUSTOM remains present and its
  PropertyGrid contract is unchanged.
- Current Dev verification passed: zero-warning/zero-error Debug solution
  build, external-reference gate, readiness contract, public sample asset
  gate, and the 33-row public sample catalog (17 required, 16 expected
  failures, 33 OK, 0 NG).
- The user stated that WPG-CUSTOM was created by the user and selected the
  owner-declaration classification. The manifest now excludes WPG-CUSTOM from
  the third-party license blocker while retaining its DLL/XML, SHA-256, and
  PropertyGrid runtime references. PL-0005 remains open for the other retained
  blocked/provenance-incomplete binaries and NOTICE coverage.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_OPTIONAL_BINARY_PRUNE_20260825.md` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_binary_prune_20260825_r1`.
- Boundary: no original-repository change, commit, push, release publish, or
  RC2 approval was performed.

### 2026-08-26 WPG-CUSTOM Owner Classification - Complete in Dev; PL-0005 Still Open

- The user selected option 1: keep the WPG-CUSTOM runtime and PropertyGrid
  project references, but remove WPG-CUSTOM from the PL-0005 external
  third-party license blocker.
- The manifest now records `user-created-owner-declaration` and
  `allow-with-user-ownership-declaration`. No WPG DLL/XML or project reference
  was deleted or changed.
- PL-0005 still covers the other retained blocked/provenance-incomplete DLLs:
  remaining third-party NOTICE coverage and the conditional MaterialDesign
  retention/removal decision. The seven first-candidate DLLs were removed by a
  later user-authorized Dev prune.
- The Dev branch remains `codex/public-sample-ux-docs` at `827a22e9`, matching
  `origin/codex/public-sample-ux-docs`. This classification and the prior
  binary-prune changes are not committed or pushed. The canonical version is
  still `2.1.0`; `v2.1.0-rc.2` has not been created.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_WPG_CUSTOM_OWNER_CLASSIFICATION_20260826.md`.

### 2026-08-26 PL-0005 Remaining Binary Evidence Sweep - Partial

- `dll/MaterialDesign/MaterialDesignColors.dll` exactly matches the
  `MaterialDesignColors 3.0.0` official NuGet `net462` DLL, and
  `dll/MaterialDesign/MaterialDesignThemes.Wpf.dll` exactly matches the
  `MaterialDesignThemes 5.0.0` official NuGet `net462` DLL. Both manifest
  entries now record MIT, exact-package binary provenance, and
  `allow-with-third-party-notice`.
- Cyotek ImageBox and SharpGL.SceneGraph were compared against their official
  NuGet package candidates. No candidate matched the repository SHA-256, so
  both remain blocked rather than being approved from upstream license pages
  alone. CircularProgressBar, EzBasicAxl, Matrox, TabControl, and
  WinFormAnimation remain unresolved as well.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_REMAINING_BINARY_EVIDENCE_SWEEP_20260826.md`,
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_remaining_binary_evidence_20260826_r1`,
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_remaining_binary_evidence_20260826_r2`.
- PL-0005 is still `Incomplete`; no original-repository change, commit, push,
  tag, release, or deployment was performed. The canonical version remains
  `2.1.0`, and `v2.1.0-rc.2` has not been created.

### 2026-08-26 PL-0005 First-Candidate DLL Prune - Complete in Dev; Release Still Blocked

- The user authorized removal of exactly seven repository-only DLLs:
  CircularProgressBar, Cyotek ImageBox, EzBasicAxl, Matrox, SharpGL.SceneGraph,
  TabControl, and WinFormAnimation. Each was backed up to D: with its length,
  SHA-256, and HEAD blob SHA-1 before deletion.
- The manifest now records those seven paths as
  `deleted-in-worktree`/`forbidden`. The required `SharpGL.dll` and
  `SharpGL.WinForms.dll` remain; the conditional MaterialDesign pair also
  remains untouched.
- Current verification passed: zero-warning/zero-error Debug solution build,
  external-reference gate with all seven paths `ABSENT | forbidden`, readiness
  contract, and public sample asset check (`CatalogRows=33`,
  `ManifestAssets=229`, `Pipelines=17`). No deletion error occurred.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_FIRST_CANDIDATE_DLL_PRUNE_20260826.md` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_first_candidate_prune_20260826`.
- The deletion slice is complete in Dev. PL-0005 remains open for the
  MaterialDesign NOTICE/retention decision and the final distribution gate.
  No original-repository change, commit, push, tag, release, or deployment was
  performed; the canonical version remains `2.1.0`.

### 2026-08-26 PL-0005 MaterialDesign DLL Prune - Complete in Dev; NOTICE/Distribution Gate Remains

- The user authorized deletion of `dll/MaterialDesign/MaterialDesignColors.dll`
  and `dll/MaterialDesign/MaterialDesignThemes.Wpf.dll`. Both were backed up
  with length, SHA-256, and HEAD blob identity before deletion. The excluded
  `BooleanToEyeIconConverter .cs` source file was not changed.
- The manifest records both paths as `deleted-in-worktree`/`forbidden`. All
  nine previously repository-only candidate DLLs are now absent; required
  runtime DLLs, including WPG-CUSTOM and the two required SharpGL runtime
  siblings, remain present and hash-verified.
- Current verification passed: zero-warning/zero-error Debug solution build,
  external-reference gate, readiness contract, and public sample asset check
  (`CatalogRows=33`, `ManifestAssets=229`, `Pipelines=17`). No deletion or
  validation error occurred.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_MATERIALDESIGN_DLL_PRUNE_20260826.md` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_materialdesign_prune_20260826`.
- PL-0005 remains `Incomplete` only for retained-dependency NOTICE coverage
  and the final clean Release distribution gate. No original-repository change,
  commit, push, tag, release, or deployment was performed; the canonical
  version remains `2.1.0`.

### 2026-08-26 PL-0005 Retained Dependency NOTICE Coverage - Complete in Dev; Clean Release Gate Remains

- Root `NOTICE` now covers every present manifest entry whose release policy is
  `allow*`: FontAwesome.Sharp and its embedded Font Awesome Free 5.15.1 fonts,
  OpenCvSharp managed/native runtime, OpenVisionLab Vision SDK 3.0, SharpGL,
  and the separate WPG-CUSTOM owner declaration.
- `tools/TestThirdPartyNoticeCoverage.ps1` requires a `noticeMarker` for every
  present allowlisted manifest entry and fails when the marker is absent from
  the selected NOTICE file. `VerifyReleaseCandidate.ps1` checks the repository
  NOTICE before publish, and `TestReleaseDistribution.ps1` checks the copied
  distribution NOTICE after publish.
- Current verification passed: NOTICE coverage for all 10 present `allow*`
  entries, Debug/Release external references, Debug/Release solution builds
  with zero warnings/errors, Release readiness, public sample assets, document
  index, and PL-0005 ledger validation.
- Evidence is recorded in
  `docs/reports/OPENVISIONLAB_RETAINED_DEPENDENCY_NOTICE_COVERAGE_20260826.md`
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0005_notice_coverage_20260826`.
- PL-0005 remains `Incomplete` only for the final clean Release distribution
  gate. The current Dev worktree still contains pre-existing tracked changes,
  so `VerifyReleaseCandidate.ps1` cannot admit this worktree as the exact clean
  candidate. No original-repository change, commit, push, tag, release, or
  deployment was performed; the canonical version remains `2.1.0`.

### 2026-08-26 PL-0005 Clean Release Candidate Gate and Grouped Dev Push - Complete in Dev

- After the user explicitly authorized grouped version-managed commits and
  pushes, the reliability changes were committed by responsibility and pushed
  sequentially to `origin/codex/public-sample-ux-docs`:
  `84fd5852` resource/image/SDK ownership, `bf5ab066` ImageCanvas/OpenGL
  cleanup and coordinate gates, `f9ed9080` BitmapImageConverter memory safety,
  `8dab07a4` Recipe/Pipeline storage boundaries and execution provenance,
  `63f022aa` smoke/operator walkthrough contracts, `f4cb0d12` forbidden DLL
  removal and NOTICE enforcement, and `cb3ead00` documentation, handoff,
  issue-ledger, and RC2 evidence; `0292656b` records the clean Release gate.
- The canonical product version remains `2.1.0`; these development commits do
  not create or change `v2.1.0-rc.2`. No tag, GitHub release, deployment, or
  release-stage mutation was performed in this Dev push; the separate original
  `main` promotion is recorded in the following section.
- From the now-clean worktree at
  `0292656befc07a79e4e2d43ddd24d386a6b24909`,
  `VerifyReleaseCandidate.ps1
  -OutputDir artifacts\release_candidate_20260826_actual_launch` passed.
  Locked restore, Debug/Release zero-warning/zero-error builds, readiness
  13/13, external references, NOTICE coverage, public assets, all 33 public
  sample rows, clean Release publish, copied distribution NOTICE, archive, and
  checksum passed. The framework-dependent `win-x64` package contains 73
  payload files and has SHA-256
  `11A74CB7D10645FBEEA530FE5E2F13D80049F75435206146473A777CD84837F0`.
- Evidence is recorded in
  `.proofline/issues/PL-0005.json` (`E16`/`E17`/`D6`),
  `docs/reports/OPENVISIONLAB_RETAINED_DEPENDENCY_NOTICE_COVERAGE_20260826.md`,
  `C:\Git\2D\Dev\artifacts\release_candidate_20260826_actual_launch\release_candidate_summary.json`,
  the clean runtime manifest, the generated Release archive, and
  `artifacts\release_launch_smoke_20260826_122127`. The copied runtime started
  twice, data-root migration completed with `Status=Complete`, and the Error
  log was empty; only the expected migration-completion warning was recorded.
- The generated pre-gate `dist\\OpenVisionLab` output was moved to the
  recoverable D-drive backup
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pre_gate_backup_20260826`
  before regeneration; source files and the original repository were not
  changed.

### 2026-08-26 Original Main Promotion - Complete After Full Gate

- The nine reviewed Dev commits were cherry-picked to the original repository
  `main` as `40e4124`, `d529ea9`, `2d66702`, `a11610e`, `90ff5b8`, `b712f53`,
  `204f9b8`, `5d55e33`, and `4b92289`. The original `Temp.txt` remained
  untracked and unchanged.
- The original `main` full gate passed at
  `4b92289133a593356c74b22505714a1f91112380`: Debug/Release builds had zero
  warnings/errors, readiness 13/13, external references and NOTICE passed,
  all 33 public sample rows passed, the 73-file win-x64 framework-dependent
  distribution passed, and actual copied-runtime launch/data-root smoke passed.
  Archive SHA-256:
  `0ECCBA97AC03ED48C2E2199DE89B281D49DB5E1E7167B1AEF29C511F59306E66`.
- The exact commit was pushed to
  `https://github.com/Noah8218/OpenVisionLab.git` as `origin/main`.
  No tag, GitHub Release, or deployment was performed. Evidence is recorded
  in `.proofline/issues/PL-0005.json` (`E18`/`D7`) and
  `C:\Git\2D\Original\artifacts\release_candidate_20260826_original_main\release_candidate_summary.json`.

### 2026-08-26 PL-0011 Status Reconciliation - Release-Stage Authorization Pending

- The earlier `PL-0011` read-only preflight snapshot is superseded by the later
  clean Dev gate, copied-runtime launch, original-main full gate, and reviewed
  original-main promotion recorded above. `PL-0005` through `PL-0009` are
  current resolved prerequisites; `PL-0010` remains an explicit SDK-boundary
  defer.
- `PL-0011` milestones M1 through M4 are now recorded as complete in the issue
  ledger. The exact candidate evidence is prepared, but the canonical version
  remains `2.1.0`; no `v2.1.0-rc.2` tag, tag push, GitHub Release draft or
  publication, or deployment has been performed.
- The durable issue record is `.proofline/issues/PL-0011.json`. Its remaining
  action is to confirm the exact release target and obtain each later release
  stage authorization separately. Do not rerun the same clean gate when its
  source, build, artifact, and evidence identities are unchanged.

### 2026-08-26 PL-0011 Scope Freeze - Authorization Pending

- The user confirmed sequential execution with the RC2 release stage first.
  The proposed scope is now frozen for read-only release preparation at the
  reviewed original-main candidate
  `4b92289133a593356c74b22505714a1f91112380`, with canonical source version
  `2.1.0` and candidate identity `v2.1.0-rc.2`.
- The RC2 scope includes the resolved PL-0005 through PL-0009 hardening
  evidence and explicitly defers PL-0010 until the Vision SDK result contract
  and manifest are supplied. Current dirty Dev Integration changes and
  unapproved feature expansion are excluded from this candidate.
- The scope confirmation authorized preparation only. The separately authorized
  local annotated tag `v2.1.0-rc.2` now points to the candidate commit. The
  separately authorized tag push is recorded below; GitHub Release draft or
  publication, and deployment, remain unperformed and separately gated in
  `.proofline/issues/PL-0011.json`.

### 2026-08-26 PL-0011 Local Tag Created - Tag Push Pending

- The original repository now contains annotated `v2.1.0-rc.2` as tag object
  `5f9fc82c0db085e41b5e3d3f6be90cd80cb8d7e7`, dereferencing to
  `4b92289133a593356c74b22505714a1f91112380`. Creating this local reference
  did not create a commit, change source files, or push anything remotely.
- `git ls-remote` confirms that `origin` does not yet contain
  `v2.1.0-rc.2`. The next action is a separate explicit authorization for the
  exact tag push; Release draft/publication and deployment remain separate.

### 2026-08-26 PL-0011 Tag Push Complete - Release Draft Pending

- The exact `refs/tags/v2.1.0-rc.2` ref was pushed from the original repository
  to `origin`. The remote tag object
  `5f9fc82c0db085e41b5e3d3f6be90cd80cb8d7e7` matches the local annotated tag
  and dereferences to `4b92289133a593356c74b22505714a1f91112380`.
- No branch push was performed in this step. No GitHub Release draft or
  publication, and no deployment, has been performed. The next action is
  separate authorization to create the Release draft.

### 2026-08-26 PL-0011 Release Draft Created - Publication Pending

- The GitHub Release draft `RE_kwDOStA7nM4WeAoP` was created for
  `v2.1.0-rc.2` with title `OpenVisionLab 2.1.0 RC2`, `isDraft=true`,
  `isPrerelease=true`, and `publishedAt=null`. It contains five candidate
  assets: the framework-dependent ZIP, checksum, clean-runtime manifest,
  release-candidate summary, and public-sample summary.
- The draft body records the verified commit and archive hash, the explicit
  PL-0010 defer, the absence of an ImageCompare asset in this application
  release unit, and the failed legacy `RunVisionPlatformPrecheck.ps1 -SkipUi`
  boundary caused by the intentionally absent `Sample\Contour.jpg`. The
  draft is therefore review-only and is not publication approval.
- No Release publication or deployment has been performed. The next action is
  separate explicit authorization after reviewing the precheck limitation and
  draft assets.

### 2026-08-26 PL-0012 Release Precheck Public Catalog Alignment - Complete In Dev; Clean Candidate Pending

- `tools\RunVisionPlatformPrecheck.ps1` now accepts an explicit `-CatalogPath`,
  forwards it to the sample runner, and records it in the report and
  `platform_precheck_summary.json`. The release policy now invokes
  `docs\samples\OpenVisionLab.PublicSampleCatalog.csv`; the legacy catalog
  remains available only for explicit local historical checks.
- The same script now constructs its Korean tutorial-term checks from Unicode
  code points so the release policy's Windows PowerShell 5.1 invocation does
  not parse the BOM-less UTF-8 source literals as mojibake.
- Current Dev full non-UI precheck passed at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\rc2_public_platform_precheck_20260826_r2`:
  top-level `Status=OK`, all 13 gates `OK`, public sample `33/33`, zero failed
  samples, metadata issues, and artifact issues, and `Tutorial Portable
  Contract=Gate=OK`.
- The completion report is
  `docs/reports/OPENVISIONLAB_RELEASE_PRECHECK_PUBLIC_CATALOG_ALIGNMENT_20260826.md`;
  the durable follow-up issue is `.proofline/issues/PL-0012.json`.
- This proves the corrected path only on the current dirty Dev worktree. It does
  not alter the frozen `v2.1.0-rc.2` tag or draft and does not establish a clean
  release commit, new candidate tag, publication, or deployment.

### 2026-08-26 2D Local Image Buffer Consumer - Complete In Current Dev Changes

- The Dev 2D consumer now decodes a copied local `Image` artifact into an
  actual `OpenCvSharp.Mat`, runs the existing `VisionRecipeRunner` only after
  an explicit accepted acknowledgement, and publishes the correlated Run
  Record plus v2 Result.
- The consumer can also publish an explicit `Rejected` acknowledgement with a
  reason. A rejected Handoff is refused by the execution entry point and does
  not publish a Result.
- Release build, readiness, external-reference, and public-sample checks pass.
  The focused smoke reports `Good=Pass`, `Bad=Ng`, rejection execution block,
  and tamper rejection. Evidence is recorded in
  `docs/reports/OPENVISIONLAB_2D_IMAGE_BUFFER_RUNTIME_20260826.md` and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\2d-integration\two-d-integration-20260826-075941-47ac5b24302e457cad02c15b4f85059d`.
- This remains current dirty Dev work and is excluded from the already frozen
  RC2 candidate. No original-repository change, commit, or push was performed.

### 2026-08-26 2D Cross-Repository Producer/Consumer Smoke - Complete In Dev

- A Machine Studio producer smoke process now calls the existing
  `MachineIntegrationHandoffPublisher`, writes a `TwoD/Image` transaction, and
  exits. A separate Dev consumer process reads that transaction, performs the
  explicit acknowledgement, decodes the copied local PNG into
  `OpenCvSharp.Mat`, runs `VisionRecipeRunner`, and publishes `Completed/Pass`.
- The paired smoke passed with transaction
  `5fc2be1c-cd2d-4d57-9051-3d133e5ddeef` and Run ID
  `2d-73c5236986bdf58112fbd33ba6d394db4115f597738468482c892ff5d2639911`.
  Evidence is recorded in
  `docs/reports/OPENVISIONLAB_2D_CROSS_REPOSITORY_SMOKE_20260826.md` and
  `D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\2d\2d-cross-repo-20260826-172213-9e5e95ec12654124a755e501cf36ee34`.
- The Handoff declares clean build identities because the shared v2 validator
  requires them; the producer and consumer development worktrees were both
  observed dirty, so clean-release provenance remains unverified.
- This is current Dev/Machine Studio smoke work and is excluded from the frozen
  RC2 candidate. No original 2D repository change, commit, push, PC restart, or
  performance measurement was performed.
- The user explicitly deferred performance validation and selected development
  continuation. The next development slice is a ThreeD HeightMap/C3D paired
  producer-to-consumer process smoke | Recommended model: GPT-5.3-Codex |
  Reasoning effort: high. The quiet-host performance prerequisite remains
  deferred.

### 2026-08-26 PL-0010 Vision SDK Object Candidate - Bounded Slice Complete; Parity Doing

- The exact SDK source repository `C:\Git\OpenVisionLab-Vision-SDK` was updated
  locally at commit `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`. The additive
  `VisionObjectCandidate` contract exposes stable candidate identity/native
  index, raw geometry and measurements, applied limits, accepted state,
  reject code/text, drawing geometry, generation stage, and `SourceImage`
  coordinate frame.
- Blob and Contour now publish all candidates from one execution while keeping
  the legacy filtered result list for compatibility. Dev maps the candidate
  collection into Pipeline object rows and persisted Run History; the former
  `TryCaptureUnfiltered`/relaxed second execution and silent accepted-only
  fallback are removed.
- SDK Debug/Release builds and the complete deterministic smoke suite passed
  `189/189` in both configurations after the mask/multi-ROI test-only follow-up.
  The isolated package-only consumer passed for local package version
  `3.0.1-dev.20260826.candidate.2`. Dev solution, Runner, readiness,
  external-reference, public-sample, object-dimension, and direct public Blob
  Good/Bad replay checks passed; current evidence is under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\vision-sdk-candidate-f4f0c0d\app-object-dimension-r3`.
- The implementation report is
  `docs/reports/OPENVISIONLAB_VISION_SDK_OBJECT_CANDIDATE_IMPLEMENTATION_20260826.md`.
  The durable issue record is `.proofline/issues/PL-0010.json`, now `doing`
  with C2/C3 and bounded evidence recorded. Full mask/multi-ROI/selection/
  timing/drawing/Run History/public-sample parity is not yet claimed.
- The current-source direct WPF `cvr05_object_metric_distribution` view smoke
  passed. A prior host-target attempt returned `리뷰 실행 필요` after
  explicit review; that result is historical for the earlier build identity.
  The current rebuilt host targets are recorded in the 2026-08-27 UI report
  below and do not change the remaining SDK parity boundary.
- SDK `main` has a local commit only; no SDK push, original-repository change,
  release publication, installation, rollout, deployment, or EXE launch smoke
  was performed in this slice.

### 2026-08-27 Pipeline Review Bottom Visibility - Complete In Dev

- The fixed-height Pipeline Review detail row was corrected without changing
  execution or routing semantics: normal detail heights are now `240` pixels
  for the default/object case, with larger matcher/circle diagnostic heights;
  compact layout retains `220` pixels for the default/object case.
- Step Details now keeps a `MinHeight=280` result surface inside an explicit
  vertical scroll viewer, so Validation, Result, and Run Log remain reachable
  while the input/output previews retain useful height.
- Object Results now uses explicit vertical/horizontal scrolling, an
  `880`-pixel minimum content width, a `320`-pixel minimum content height, and
  a `300`-pixel DataGrid cap. This prevents long candidate tables from
  expanding the plot to an unbounded height and exposes the plot through a
  deliberate compact-width scroll path.
- The final current-source direct WPF targets passed after the latest build:
  `wpf_shell_host_pipeline_review`,
  `wpf_shell_host_pipeline_review_ng`, and
  `wpf_shell_host_workspace_sample_pipeline_review_metrics`, all at
  `1600x900`. Verification-only `1280x800` checks also reached the Step
  Details run-log end and the Object Results plot right/end scroll state.
- Evidence and the bounded acceptance record are in
  `docs/reports/OPENVISIONLAB_PIPELINE_REVIEW_BOTTOM_CLIPPING_FIX_20260827.md`
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-bottom-final-20260827-final2`.
- The repository UI-precheck wrapper passed the Object Results target when run
  alone, but a single combined three-target invocation timed out after its
  normal and NG targets completed. This is retained as a smoke-harness
  sequencing boundary; the direct separate-process targets and the singleton
  wrapper run are the current UI evidence.
- This is a Dev UI fix only. English/theme/DPI/monitor matrices, EXE launch
  smoke, original-repository mutation, commit/push, release, installation,
  rollout, and deployment remain separate and unperformed.

### 2026-08-27 Pipeline Review Image-First Layout - Complete In Dev

- The selected Step's Input/Output previews are now the default primary area.
  The existing Flow, Parameters, Validation, Result, Run Log, and diagnostic
  tabs remain available through a closed-by-default `리뷰 상세` drawer.
- Step Flow now has a themed collapse control that reduces the left rail from
  `300` to `44` pixels. Both the drawer and rail toggles are presentation-only:
  current-source smoke asserts no Preview/Run, layer, active-layer, or
  input/output route mutation.
- Korean and English drawer/tooltip catalog entries were added. Current-built
  Korean normal OK/NG/Object, Korean `1280x800` compact, English normal, and
  localization-catalog targets all passed. Evidence and exact paths are in
  `docs/reports/OPENVISIONLAB_PIPELINE_REVIEW_IMAGE_FIRST_LAYOUT_20260827.md`
  and the `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pipeline-image-first-20260827-*`
  folders.
- This is a Dev current-source WPF view capture, not EXE launch evidence. Full
  theme/DPI/monitor matrices, original-repository mutation, commit/push,
  release, installation, rollout, and deployment remain separate and
  unperformed. The project priority is unchanged: finish `PL-0010` SDK
  mask/multi-ROI/selection/timing/drawing/Run History/public-sample parity.

### 2026-08-27 PL-0010 C4 Object-Candidate Parity Replay - Complete In Dev

- The current vendored SDK identity remains `f4f0c0dc8bee5b7a849ae6eb66a5307bed4b8a6b`
  (`Release`). The current Dev Runner replay passed Blob mask, Blob/Contour
  Multi-ROI, source-coordinate geometry, candidate identity, applied limits,
  reject code/text, accepted metrics, drawings, Step timing, persisted Run
  History, and the four public Blob/Contour Good/Bad samples. Evidence is under
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-parity-20260827-r4`.
- Blob mask retained five source candidates with one `Masked` reject and four
  accepted results (`ResultCount=4`); Blob and Contour Multi-ROI retained
  `RegionIndex=0/1` and five source-coordinate rows each. Public replay passed
  Blob Good `12`, Blob Sparse Bad expected NG `3`, Contour Good `5`, and
  Contour Missing Bad expected NG `2`.
- `VisionPipelineObjectResult`/Run History now persist `RegionIndex`, applied
  limits, reject code/text, generation stage, and coordinate frame. Pipeline
  Review exposes the Multi-ROI description and a Region column. Current-source
  `cvr05_object_metric_distribution`, Blob Good, Contour Missing Bad,
  normal/NG Pipeline Review, compact `1280x800`, and Run History review-queue
  targets all passed; the latter's current screenshot is
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pl0010-c4-ui-run-history-20260827-r14\wpf_shell_host_recipe_run_history_review_queue.png`.
- Run History teardown exposed a WPF animation/template exception when the
  shell nested `Content` was cleared during disposal. The owning Window already
  detaches the host after disposal, so `OpenVisionShellHostView` now releases
  the display manager without duplicating that nested-content teardown. The
  same current target then exited `OK` and retained the queue contract evidence.
- `PL-0010` is now `resolved` for C4 bounded Dev parity. The complete report is
  `docs/reports/OPENVISIONLAB_PL0010_C4_PARITY_REPLAY_20260827.md`. No original
  repository change, commit, push, tag, Release publication, installation,
  rollout, deployment, or EXE launch smoke was performed in this slice. The
  SDK mask contract is claimed for Blob only; this SDK release does not define
  Contour mask classification.
- The next development priority is the bounded
  `IMAGE-CANVAS-EXTERNAL-CONSUMER-CONTRACT` Viewer selection/highlighting slice
  | Recommended model: gpt-5.6-terra | Reasoning effort: medium. Keep
  `CVR-00` deferred until three independent first-time participants and their
  unedited observations exist.

### 2026-08-27 IMAGE-CANVAS-EXTERNAL-CONSUMER-CONTRACT - Complete In Dev

- The public Viewer boundary is now a validated prototype in
  `src/Libraries/OpenVisionLab.ImageCanvas/External/ImageCanvasExternalConsumerSession.cs`.
  It exposes explicit `Borrow`, `Clone`, and `TakeOwnership` image ownership,
  source-image top-left rectangle coordinates, accepted/rejected overlay
  metadata, one programmatic selection at a time, selection-change metadata,
  existing CanvasRect editing/wider-line highlighting, Fit, view-state
  capture/apply, and UI-dispatcher-only idempotent disposal.
- A separate `.NET 8 WPF` WinExe consumer under
  `tools/ImageCanvasExternalConsumerSmoke` references only
  `OpenVisionLab.ImageCanvas`; it has no OpenVisionLab application
  `ProjectReference` and no package reference. The facade-owned
  `Grid`/`WindowsFormsHost` isolates the external-host sizing failure while
  leaving the shared `RoiImageCanvasView` layout unchanged.
- The current-source desktop smoke passed in
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\imagecanvas-external-consumer-20260827-r28`:
  two accepted/rejected overlays, rejected selection and
  `WidthBelowMinimum` reason, Borrow/Clone/TakeOwnership caller-lifetime
  checks, Fit -> Capture -> Apply -> Capture round-trip, visibility/no-op
  checks, 100 create/dispose cycles, native HWND color evidence, and dynamic
  two-monitor placement all passed. The selected monitor was `\\.\DISPLAY2`
  at `1.25x1.25` DPI; resource delta was handles `-5` and working set
  `+17211392` bytes.
- Current-build `wpf_imagecanvas_owned_mat_load` and
  `wpf_opengl_native_readback` regression targets also exited `OK`. The
  completion report and machine-readable boundary are
  `docs/reports/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_CONTRACT_20260827.md`
  and
  `docs/contracts/openvisionlab/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_MANIFEST.json`.
- This closes only the bounded programmatic rectangle selection/highlight
  contract. Freehand hit-test, multi-selection, non-rectangle shapes,
  Pipeline/Layer/Route/Preview/Run/Run History ownership, `net48`,
  Linux/Avalonia, single-DLL redistribution, NuGet publication, original
  repository promotion, release, installation, rollout, and deployment remain
  unperformed and out of scope.

### 2026-08-27 Cross-modal 2D geometry handoff - Complete for bounded contract

- `TwoDIntegrationExchange` now persists actual source image dimensions and
  runtime overlay bounds, centers, endpoints, angles, and source points in
  schema-`1.1` Run Records. This preserves the geometry produced by the 2D
  recipe rather than exporting metrics only.
- The paired 3D adapter consumes this Run Record through the versioned
  coordinate-projection sidecar and returns its validated reverse coordinates
  as Result evidence. The final separate-process smoke recorded 4 forward and
  5 reverse points. Evidence is recorded in
  `docs/reports/OPENVISIONLAB_2D_CROSS_MODAL_GEOMETRY_20260827.md` and the
  D-backed cross-repository run root.
- This is a local software-coordinate exchange only. Calibration, camera pose,
  physical metrology, and automatic execution remain separate contracts. The
  approved external ImageCanvas consumer now renders the validated reverse
  points as read-only markers; direct connection to the main Pipeline Review
  remains separate.

### 2026-08-24 PL-0004 Display-Store/Viewer Lifetime - Complete In Dev

- `ImageSpaceService` now owns each stored Bitmap through a reference-counted
  image owner. Replacement, deletion, reload, and service disposal release the
  store reference; the image retires only after every active lease releases.
- The central workspace holds a lease while its Canvas/fallback presenters
  borrow the image. Docked and popout viewers clone under a short lease and
  dispose viewer-owned images on refresh, close, deletion, or Shell shutdown.
- Shell shutdown detaches command/visual bindings before disposing its display
  store. Canvas shutdown cancels queued refreshes, disposes child/host state,
  and stops SharpGL 3.1.1's unowned drawing timer before OpenGL teardown.
- The exact five-cycle 4512 lifetime gate passed: all replacement hashes
  matched store/dock/popout, active layer returned to Main, Preview/Run and
  routes were unchanged, live viewer count stayed one, and retained ranges
  were private 16.8 MB, working set 4.3 MB, managed 0.1 MB, handle range 21
  with positive growth 2, GDI 0, USER 0, and 70.311 seconds.
- `HistoryContractCheck`, eight focused same-process WPF regressions, the
  zero-warning solution build, readiness 13/13, vendored references, and
  33-row/229-asset/17-Pipeline public sample checks passed.
- Evidence:
  `docs/reports/OPENVISIONLAB_PROJECT_ANALYSIS_AND_RELIABILITY_PRIORITY_20260823.md`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260824\after-timer-stop-5cycles`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260824\final-directional-handle-gate-5cycles`,
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab\pl-0004-display-lifetime-20260824\final-focused-regression`.
- Boundary: GPU VRAM, intra-operation peaks, native framebuffer readback,
  every OpenGL exception path, actual-EXE theme/DPI/monitor, arbitrary
  duration, multi-PC, original-repo promotion, commit, and push remain
  separate.

### 2026-08-24 OpenGL/GPU/Viewer Coordinate Reliability - Complete In Dev

- CP0/CP1 froze the current Dev baseline, traced the OpenGL callers, and
  recorded the Texture/FBO/RBO/PBO/display-list/Bitmap-lock/context/timer
  owner and failure matrix.
- CP2 added local allocation-ID/flag `try/finally` cleanup for FBO/RBO/PBO,
  Bitmap locks, texture conversion, overlay texture/display-list failures,
  neutral bindings, and primary-exception preservation. The forced callback
  exception was followed by a successful render in the same context.
- CP3 passed the predeclared per-process GPU dedicated/shared plateau gate for
  the exact 4512 x 4512 workflow. CP4 passed native four-corner/edge/1x1/full/
  region/bounds identity and exact half-open region restore.
- CP5 passed the current-source ten-target focused suite, solution build,
  HistoryContract, readiness, external-reference, public-sample, and
  documentation-index gates. The dated completion record is
  `docs/reports/OPENVISIONLAB_OPENGL_GPU_COORDINATE_RELIABILITY_20260824.md`.
- Evidence is under
  `D:\OpenVisionLab-TestData\OpenVisionLab\opengl-gpu-coordinate-20260824-114354`.
- Boundary: one invalid Windows GPU performance-counter sample was retained
  in the log; valid late samples still passed both plateau checks. Native
  bitmap/readback is the pixel proof because hosted OpenGL may not appear in
  `RenderTargetBitmap`. No actual desktop EXE, multi-monitor/DPI/theme, or
  multi-PC qualification is claimed. An older composite smoke ordering with
  `reliability` before `lifetime` still reproduced a lifetime
  `NullReferenceException`; the final current-source suite uses lifetime-first
  ordering and all ten targets pass. This is a smoke-order precondition, not
  an all-order guarantee.

### 2026-08-23 Image Ownership And Reliability Gates - Complete In Dev

- `ImageSpaceFrame` now exposes explicit `Borrow(Bitmap)` and
  `TakeOwnership(Bitmap)` creation. DisplayManager synchronously consumes every
  frame after creating one independent store clone; borrowed caller Bitmaps and
  source Mats remain caller-owned.
- `CanvasImageLoader` now returns an owned OpenCvSharp Mat through
  `Cv2.ImRead(..., AnyColor)`. ImageCanvas no longer references/copies Emgu and
  the three inactive Emgu managed/native DLLs were removed after verified
  D-drive backup.
- The frozen Mean Recipe soak passed 1,000/1,000 runs with zero failure,
  metric/image drift, or Recipe/source mutation. p95 was 42.665 ms, max
  313.284 ms, maximum private growth 1.141 MB, and every late plateau range was
  zero.
- The exact 4512 x 4512 8bpp current-source WPF gate verifies source/store raw
  identity, workspace/automatic-dock dimensions and texture creation, and the
  maximum of AfterSet, RenderedBeforeGc, and retained process-resource
  snapshots. It does not claim native pixel readback, intra-SetMain peak, or
  GPU VRAM.
- One central selected-layer refresh, one trailing dock refresh, and two
  unsampled base-image mipmap generations were removed. Command CanExecute
  re-evaluation remains explicit. Two independent final-code runs retained
  exact identity/dimensions and reduced retained private growth from the prior
  624.1 MB baseline to 523.0/524.2 MB; SetMain was 1,589/1,319 ms versus the
  prior 3,572 ms.
- Focused workspace image-load, quick-action command, layer management,
  popout, 5200 large-image, owned-Mat, and template-editor smokes passed from
  the current `Any CPU` build.
- Evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab\imagespace-frame-ownership-20260823`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\emgu-owned-loader-20260823`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\soak-rerun`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-final-verified`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-mipmap-final-verified-rerun`,
  `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\final-current-build`,
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab\reliability-gates-20260823\refresh-command-final-regressions`.
- Boundary at that checkpoint: PL-0004 above now closes coordinated store/
  viewer retirement. Further clone removal, hidden presentation allocations,
  OpenGL exceptional/GPU cleanup, actual-EXE DPI/theme/monitor,
  original-repo promotion, commit, and push remain separate.

### 2026-08-23 Native UI Preview/Teaching SDK Tool Lifetime - Complete In Dev

- All 15 active direct SDK Tool construction sites across native Tool Preview
  and Auto MPoint teaching now have a creator-side disposal owner. Thirteen use
  lexical `using`; Filter and Morphology use the shared custom executor's
  `finally` and clean up if `SetProperty` fails.
- The Preview controller now disposes the complete `VisionToolResult`. Auto
  MPoint uses the SDK Tool/result disposal contract rather than manually
  disposing three Tool-owned Mats, and the unused `Func<IVisionTool>` document
  constructor that bypassed ownership was removed.
- The Debug solution build passed with zero warnings/errors. The 15-site source
  contract, six focused current-source WPF Preview/teaching captures,
  `VisionUiContractCheck`, runtime stability, readiness 13/13, vendored
  references, and 33-row/229-asset/17-Pipeline public samples passed.
- No visible control, parameter, algorithm, result, layer, active-layer,
  routing, Recipe, Pipeline, or explicit Preview behavior changed. This is
  current-source WPF view evidence, not an actual `OpenVisionLab.exe`
  theme/DPI matrix.
- Three stale/transient focused-smoke assertions found during verification are
  separated as `PL-0003`; alternate passing targets cover the same product
  paths without misclassifying the text/timing failures as product failures.
- Evidence:
  `docs/reports/OPENVISIONLAB_PROJECT_ANALYSIS_AND_RELIABILITY_PRIORITY_20260823.md`
  and
  `D:\OpenVisionLab-TestData\OpenVisionLab\ui-tool-lifetime-20260823`.
- Boundary at that checkpoint: the latest entry above now closes the
  ImageSpaceFrame, Emgu loader, 4512, and 1,000-run slices. End-to-end
  store/viewer leases and actual-EXE theme/DPI qualification remain. Original
  was not touched.

### 2026-08-23 Project Analysis And Pipeline Tool Lifetime - Complete In Dev

- The requested repository/product analysis is recorded in the required
  39-section structure, with source, current-run, historical, and unverified
  findings kept separate. It selects reliability ownership before new
  algorithms or platform expansion.
- SDK 3.0 reflection found 14 concrete IVisionTool implementations and all are
  disposable. Runtime probes proved that Threshold and Mean returned result
  images and metrics remain valid after Tool disposal; no defensive result
  clone is required.
- All three production `VisionPipelineAppToolFactory.Create` consumers now
  dispose disposable Tools. A capture failure disposes the unreturned result;
  Blob/Contour audit, MultiMatchMean, NormalizeImage, and composite
  LineDistance/LineIntersection temporaries now have explicit local owners.
- Debug solution build passed with zero warnings/errors. Runtime stability,
  object-dimension audit, CVR-10 MultiMatchMean, and public LineDistance runtime
  checks passed without changing metrics, overlays, rejection reasons, or
  Pipeline routing. Readiness 13/13, external references, and public samples
  also passed.
- Evidence:
  `docs/reports/OPENVISIONLAB_PROJECT_ANALYSIS_AND_RELIABILITY_PRIORITY_20260823.md`
  and `D:\OpenVisionLab-TestData\OpenVisionLab\tool-lifetime-20260823`.
- Boundary at that checkpoint: direct native UI Tool lifetime and the later
  ImageSpaceFrame, Emgu loader, 4512, and 1,000-run slices are now closed by the
  entries above. Store/viewer lease ownership remains. No WPF UI was changed by
  the Pipeline slice; original was not touched.

### 2026-08-21 GitHub Actions Artifact Storage Cleanup - Complete In Dev And Original

- The public original repository's 18 live
  `OpenVisionLab-win-x64-framework-dependent-*` workflow artifacts were
  deleted after exact name/count verification, freeing `836.06 MiB` of current
  Actions artifact storage. A post-delete API read returned zero matching live
  artifacts.
- The five published `v2.1.0-rc.1` Release assets were verified before and
  after cleanup and remain unchanged.
- Dev and original `.github/workflows/ci.yml` keep the full Release Candidate
  gate on push/PR, but upload the approximately 46 MB package only for explicit
  `workflow_dispatch` runs and retain it for three days. The release policy
  records the same distinction between CI evidence and user-facing Release
  assets.
- `git diff --check`, the focused workflow storage assertions, and
  `TestDocumentationIndex.ps1` (`62` indexed paths, `12` routes, `101` root
  redirects) pass.
- Boundary: routine push/PR runs still build and verify the package but do not
  retain it. A manual workflow run deliberately creates one three-day
  Artifact. Product code, tags, and the five Release assets are unchanged.

### 2026-08-13 Main Pipeline Button Actual-EXE Regression - Complete In Dev And Original

- Pipeline Review remains cached across native Tool selection, its hidden
  central document is suspended instead of detached, and Add Pipeline refreshes
  the same cached Recipe/Pipeline document before the operator opens it.
- The exact main no-image `Pipeline 열기` command now records command,
  selection, render, application-idle, cache, and internal timing phases.
- Before promotion, the original EXE returned in `108 ms` directly and
  `396 ms` after the Tool path. After explicit approval and promotion, the
  rebuilt original returned in `102/98 ms`; its trace reached UI idle in
  `40/29 ms` and recorded `CachedBefore=True` for both paths.
- The Dev focused regression passed at `36/13/46 ms`; the promoted original
  independently passed at `33/14/39 ms`. Both retained Preview `0 -> 0`, Layer
  count `1 -> 1`, active Layer `Main`, and unchanged immediate routing. The
  original build passed with zero warnings/errors and readiness passed 13/13.
- Evidence:
  `docs/reports/OPENVISIONLAB_PIPELINE_REVIEW_ENTRY_AND_COMPACT_LAYOUT_20260812.md`.
- Runtime evidence:
  `D:\OpenVisionLab-TestData\OpenVisionLab\pipeline-main-button-original-promotion-20260813`.
- Boundary: the originally reported ten-second observation was later reproduced
  only under Visual Studio F5, isolated, corrected, and independently verified
  in original. Reopen only with a new current-build trace; no PR or Release was
  created.

### 2026-08-12 Startup Feedback, Pipeline Review Entry, And Compact Layout - Complete In Dev And Original

- A localized, themed startup window now appears before the last Recipe is
  restored and cannot close until the shell and Pipeline Review cache are
  ready. The actual no-window interval changed from `4937 ms` to visible
  feedback at `1666 ms`; this is responsiveness evidence, not a total startup
  duration claim.
- A Recipe/Pipeline without a matching review cache receives a themed,
  shell-blocking loading overlay. Initial all-Tool warmup no longer competes
  with first paint; existing post-selection idle warmup remains.
- Pipeline Review now prewarms after idle, after Recipe selection, and after
  native Tool use without extending the operator's click or Recipe-loading
  critical path. Cached restore refreshes only when the Pipeline file changed;
  Main/native output changes invalidate stale review results without running.
- Focused current-source timings were Startup `5 ms`, Recipe switch `6 ms`,
  and after Tool `47 ms`. All paths retained Preview runs `0 -> 0` and Layer
  count `1 -> 1`. The actual main `Pipeline 열기` EXE scenario measured
  `12 ms` after the redundant shell Layer-row refresh was removed.
- The header, readiness, guide, Step summary, and detail allocation are more
  compact. At 1280 x 800, input/output image content remains `208 x 156` with
  the guide collapsed and `185 x 138` with it expanded; the lower detail tabs
  yield space only while the compact guide is open.
- Current actual-EXE Korean/English startup captures, Pipeline loading overlay,
  and Korean review captures at 1920 x 1500, 1600 x 900, and 1280 x 800 were
  inspected with no clipped text/icon, overlap, hidden button label, or white
  theme leak. Build 0/0 and focused loading/UI/performance smokes pass.
- Evidence:
  `docs/reports/OPENVISIONLAB_PIPELINE_REVIEW_ENTRY_AND_COMPACT_LAYOUT_20260812.md`.
- Dev and original independently pass zero-warning Debug builds, readiness
  13/13, vendored references, public assets, and both loading smokes. All 26
  promoted Git object hashes match; original-only `Temp.txt` remains untracked.
- Boundary: commit and push are the remaining publication actions. This work
  does not publish a new tagged Release.

### 2026-08-12 Portfolio Multi-Stage Processing Showcase - Complete In Dev Working Tree

- A reproducible public perforated-plate Pipeline now exposes one source image
  through `Filter -> Threshold -> Morphology`, then branches the cleaned Layer
  to Blob and Contour. Both branches retain 34 intended holes and exclude the
  full-image background contour.
- A shaft-pitting Pipeline confines 18 retained Contour candidates to one
  reviewed ROI after preserving smaller point-like candidates with
  `Threshold=100` and `MIN_AREA=2 px^2`. Its defect-free `ResultCount=0`
  contract reports this defective sample as NG rather than passing a broad
  count range. The existing semiconductor lead-width Pipeline retains 16
  scans at 42.012 px / 0.252 mm. The public edge sample demonstrates
  `EdgeDetection -> Morphology -> Contour` with four accepted shapes.
- Current-EXE Korean and English captures show the source, every intermediate
  Layer, final drawings on the source image, and completed Pipeline Review.
  The capture reports record monitor/window intersection, EXE and input hashes,
  pane counts, exact language display, and no Computer Use overlay.
- README now leads with the six-panel English actual-EXE comparison and links
  the reproducible public Pipeline.
- Evidence:
  `docs/reports/OPENVISIONLAB_PORTFOLIO_STAGE_SHOWCASE_20260812.md`.
- Boundary: public sample/runtime/UI evidence only. The shaft sample has no
  independent pixel-level ground truth, so the 18 candidates prove only the
  configured runtime selection, not recall, accuracy, calibrated production
  metrology, or field robustness for other data, optics, or hardware.

### 2026-08-12 Portfolio Pattern Comparison And Clean EXE Capture - Complete In Dev Working Tree

- The approved card Matching demonstration now uses a bounded `-20..20`
  angle and `0.85..1.15` scale search. Five current runs scored
  `91.949-99.072` and visibly selected the same taught lower pattern.
- Pipeline Review now presents its current-run rendered detection overlay, and
  the main toolbar exposes a themed, editable layer name plus Rename action.
- A current-source Debug `OpenVisionLab.exe` produced a clean six-pane
  comparison and a readable Pipeline Review input/output capture. The render
  path used no Computer Use, cursor visualization, Codex chrome, or desktop
  background. Debug build 0/0, focused UI smoke 2/2, readiness 13/13,
  vendored references, public assets, and `git diff --check` passed.
- Evidence:
  `docs/reports/OPENVISIONLAB_PORTFOLIO_PATTERN_CAPTURE_20260812.md`.
- Boundary: this is portfolio/runtime evidence in the Dev working tree, not a
  release, physical-part robustness, calibrated metrology, camera, lighting,
  or PLC/I/O qualification claim.

### 2026-08-11 Recipe Switch And Result Canvas Correction - Complete In Dev And Original

- Recipe changes no longer construct the full Pipeline Review WPF document on
  the critical path. Comparable actual-EXE `Default -> FieldPilot_BentPin`
  readiness improved from `1168.2 ms` to `577.6/579.5 ms`; subsequent switches
  in both directions remained responsive at `577.6-589.7 ms`.
- Explicit Blob and Contour Preview drawings now use the routed source image as
  their canvas. The separate threshold-teaching Preview remains binary.
- Current-source Blob/Contour and recipe-switch smokes passed with zero layout,
  text, or internal failures. Debug builds, readiness 13/13, vendored external
  references, and public sample assets passed. The actual EXE Contour Preview
  retained the source image and reported one detection at `462 ms`.
- Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_SWITCH_AND_RESULT_CANVAS_FIX_20260811.md`.
- The original independently passed its zero-warning Debug build, four focused
  Blob/Contour/Recipe smokes, readiness 13/13, external references, public
  assets, and documentation index. All six promoted Git object hashes match
  Dev; the unrelated original-only untracked `Temp.txt` was preserved.

### 2026-08-11 Tool Preview Large Viewer - Complete In Dev And Original

- Double-clicking any non-empty Tool View Input/Output preview now opens the
  exact routed layer in one reusable 960x720 large viewer with the existing
  zoom, pan, fit, and resize behavior. Arithmetic reuses the same window while
  switching Input A, Input B, and Output.
- The viewer follows explicit Preview result refreshes, closes when its owning
  Tool changes or closes, and updates its Tool/role title live for Korean and
  English. Plain-click layer activation and inline double-click fit reset are
  preserved.
- The Dev and original actual EXEs independently passed floating and docked
  Line, arithmetic A/B,
  explicit-result refresh, Korean/English switching, monitor placement, and
  zero Preview/Run, layer, active-layer, or route side effects. Focused Line,
  arithmetic, dock/float, and localization smokes also passed without layout,
  text, or internal failures.
- Evidence:
  `docs/reports/OPENVISIONLAB_TOOL_PREVIEW_LARGE_VIEWER_20260811.md`.
- Dev and original full Debug builds passed with zero warnings/errors, both
  readiness runs passed 13/13, and all 29 promoted file contents matched.

### 2026-08-11 Contour First-Open Crash Correction - Complete In Dev And Original

- Contour could throw a WPF `Grid.SetFinalSizeMaxDiscrepancy`
  `NullReferenceException` when the operator selected it before native Tool
  prewarm reached Contour. Tool selection cancelled the remaining prewarm
  queue, while the on-demand creation path showed the new PropertyGrid view
  without the hosted-layout preparation used by the background path.
- Newly created native Tool documents now receive the existing layout
  preparation before first presentation. Cached/prewarmed and reopened
  documents do not repeat that work.
- The exact former failure conditions passed with native prewarm disabled and
  with both native prewarm and floating-window preparation disabled. Contour
  opened without Preview/Run or layer changes. Normal Contour, the Blob
  comparison, all 17 native Tool layer routes, Contour floating/docked views,
  Tool dock/float cycling, Debug build, and readiness 13/13 also passed.
- Evidence:
  `docs/reports/OPENVISIONLAB_CONTOUR_FIRST_OPEN_CRASH_FIX_20260811.md`.
- The original actual EXE independently passed the former failure condition
  with both native Tool prewarm and floating-window preparation disabled;
  Preview count and layer count remained zero.

### 2026-08-11 Line Signal Non-Blocking Cue - Complete In Dev And Original

- A successful Line Edge/Measure Preview still retains the exact signal
  evidence, but no longer forces the detailed Signal Inspector over the
  parameter editor.
- The localized `신호 검토` / `Review signal` command becomes available and a
  non-interactive `신호 갱신됨` / `Signal updated` cue appears beside it for
  three seconds. Repeated evidence replaces the same cue instead of queuing
  notifications.
- The detailed plot opens only when the operator selects the review command.
  A manually opened inspector remains open while new evidence updates it.
- Dev and original actual EXEs independently passed the same public 572x420
  pin sample: Edge mode retained the parameter editor, the cue auto-dismissed,
  manual review retained the detailed plot, and the following Measure run
  returned 37 px / 0.222 mm / 24 detections. Preview count, layers, active
  layer, and routes remained unchanged during cue/review interaction.
- The test fixture now uses the tracked public pin sample instead of the
  Dev-only legacy `Sample\EasyGauge\Pins.bmp`. No detection, XML,
  calibration, acceptance, or routing behavior changed. Evidence:
  `docs/reports/OPENVISIONLAB_LINE_SIGNAL_TRANSIENT_CUE_20260811.md`.

### 2026-08-11 ROI Edge Resize And Zoom Editing - Complete In Dev And Original

- Full-image ROI handles now stay inside a 14-pixel fit-view margin, so the
  left edge can shrink inward instead of remaining stuck at X=0.
- Mouse wheel and localized side-panel controls zoom from 25% to 1600%;
  middle-button drag pans, and the same image-coordinate transform keeps ROI
  creation, movement, and resize active after zoom/pan.
- The original actual EXE loaded `Pins_OK_0001.jpg` at 768x576, changed the
  full ROI from `X=0/W=768` to `X=92/W=676`, then at 125% zoom plus 25px
  pan made a finer `X=101/W=667` adjustment and returned to 100%.
- Dev/original focused builds and ROI gates passed; full Debug builds,
  readiness 13/13, five-file canonical Git equivalence, monitor placement, and
  themed hover/actual pointer-down evidence passed. No Preview/Run, layer, or
  routing behavior changed.
  Evidence:
  `docs/reports/OPENVISIONLAB_ROI_ZOOM_RESIZE_20260811.md`.


### 2026-08-11 Recipe Switch Loading Lifetime Correction - Complete In Dev And Original

- The pushed `42d840a9`/`0582d226` implementation displayed the loading overlay
  but closed it before deferred Pipeline Review preparation finished. The user
  report invalidated that completion claim; the current Dev and original
  working-tree correction keeps the overlay open until the preparation task
  completes.
- Recipe selection no longer repeats the Pipeline/validation/history/summary
  refresh already performed by `RecipeState.EventChangedRecipe`. Recipe changes
  also no longer restart the whole native Tool prewarm queue immediately after
  the switch; the explicitly selected Tool still opens through the normal Tool
  selection path.
- The actual EXE reproduced a post-overlay unresponsive sample at 1,069.9 ms
  before the correction. After the correction, the overlay covered the
  preparation interval and the process stayed responsive from 866.0 ms through
  3,269.4 ms. Post-load Recipe filter input rendered in 40.6 ms.
- The same nine-file patch was applied to the original working tree with all
  Dev/original file hashes matching. The rebuilt original EXE showed the
  loading overlay during `Edge_Base -> Default`, remained responsive from the
  first post-overlay frame at 1,503.4 ms through 12,634.3 ms, accepted Recipe
  filter input in 71.1 ms, and restored the initial `Edge_Base` Recipe.
- Debug solution and ScreenshotSmoke builds, readiness 13/13, rebuilt Recipe
  lifetime/safety/context, Recipe summary, native Tool open, and actual-EXE
  monitor-placement evidence passed.
  Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_SWITCH_LOADING_RESPONSIVENESS_20260811.md`.

### 2026-08-11 Recipe, Layer, N-image, And Edge Regression Batch - Complete In Dev

- Right-click image load now targets the exact clicked layer, Recipe save is
  available from the toolbar and `Ctrl+S`, and valid last image directories are
  reused across the affected workspace, Tool View, layer, and N-image dialogs.
- Recipe create/switch owns a busy-state contract. The original current-source
  evidence did not prove actual-EXE visibility; the actual-EXE correction and
  performance evidence are recorded in the newer Recipe switch report above.
  Failed Step XML save/round-trip no longer opens a second pending-edit dialog
  while retaining a dirty editor.
- N-image verification now lists selected files before Run, distinguishes
  `ERROR`, `NG`, `OK`, and ungated `RUN OK`, selects failed evidence, and uses
  localized controls/tooltips/report structure. EdgeBasedMatching separates
  its guide/teaching panel and defaults to a dependency-aware compact parameter
  set.
- Debug build, readiness 13/13, seven focused current-source WPF targets, and
  the six-Tool x 30-image contract plus gated NG check passed. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_LAYER_NIMAGE_EDGE_REGRESSION_FIX_20260811.md`.
- The identical implementation patch is pushed as Dev `dc08dde5f42a` and
  original `a6bbf277dea4`. The original independently passed the same build,
  readiness, N-image, and focused WPF gates before push.

### P294 Main No-Image Pipeline Open Performance - Complete In Dev And Original

- The corrected scope is the main no-image workspace's `Pipeline 열기` button,
  not Recipe Manager's Open Pipeline command.
- Pipeline Review now prepares one exact current Recipe/Pipeline document and
  its hidden central-workspace layout during startup idle work. The no-image
  guide remains visible until the explicit click; stale contexts are rejected,
  and Recipe changes schedule preparation for the new context.
- Across three fresh actual-EXE processes, visible readiness changed from
  768-1,075 ms (median 820 ms) to 411-435 ms (median 428 ms). Internal selection
  changed from 244-327 ms (median 255 ms) to 22-28 ms (median 27 ms).
- Wide/Compact actual EXE, empty-workspace, Recipe context-switch, dock/float,
  deterministic public-sample review, Debug build, readiness 13/13, and Vision
  UI contract checks passed without automatic Preview/Run. Evidence:
  `docs/reports/OPENVISIONLAB_WORKSPACE_EMPTY_PIPELINE_OPEN_PERFORMANCE_20260809.md`.
- The 2026-08-13 exact-button correction and trace were promoted to original;
  current original direct/after-Tool application idle is `40/29 ms`.
- A later current-Dev Visual Studio F5 run reproduced the operator's exact
  button delay despite the direct-EXE result: three same-context F5 processes
  returned in `10,139/10,119/9,971 ms`.
- Phase logging isolated `9,947 ms` to the Tool-specific dock-mode visual-tree
  traversal being applied to the central Pipeline Review document. General
  documents and the Pipeline Review floating path now skip that traversal;
  native Tool Views retain it.
- Three fresh F5 processes now return in `23/19/19 ms`; a fresh direct-EXE
  actual-button run returns in `15 ms` and reaches application idle in `39 ms`.
  Debug build 0/0, Pipeline Review/entry-performance/dock-float smokes, and
  readiness 13/13 pass. Evidence:
  `docs/reports/OPENVISIONLAB_PIPELINE_F5_DOCKMODE_PERFORMANCE_20260813.md`.
- The exact correction was promoted as Dev `4b4d3db1` and original `e60adc3`.
  The rebuilt original independently passed a zero-warning Debug build, all
  three focused Pipeline Review/entry-performance/dock-float targets, readiness
  13/13, and the actual Visual Studio F5 main-screen button at `20 ms` command
  return, `30 ms` render priority, and `54 ms` application idle. Its current
  verification used `Portfolio_Pattern_Rotation_Scale`, so the result is not
  limited to the Dev measurement Recipe.
- A current-Dev follow-up reproduced a separate placement regression: closing
  the central Pipeline Review caused the next `Pipeline 열기` to create a
  floating window because close and Float shared one persisted restore flag.
  Normal Pipeline entry now always targets the central document workspace;
  explicit Float affects only the current window lifetime. Central close/reopen,
  explicit Float close/reopen, and Visual Studio F5 close/reopen all return to
  the central workspace with no separate floating window. Direct-EXE reopen
  commands were `82/64 ms`; F5 close/reopen was `82 ms` command return and
  `257 ms` UI idle. The updated dock/float cycle smoke covers both sequences
  while preserving no-execution/layer/routing invariants. The exact four-file
  implementation/test port is Dev `9401a01` and original `4f75fc6`; all four
  Git blob IDs match. The independently rebuilt original passed Debug build
  0/0, all three focused targets with zero layout/text/internal issues, and
  readiness 13/13. The result is recorded in the same P294 report.

### P293 Recipe Manager And Pipeline Entry UX - Complete In Dev

- Recipe lifecycle commands and the name editor now remain together above the
  Recipe Manager workbench in Wide and Compact layouts. A panel-scoped field
  template removes the Wpf.Ui light-template leak, so entered names and all
  normal/focus/hover/read-only/disabled/error states use the shell theme.
- Pipeline Review now defaults to the central document workspace instead of
  paying the separate floating-window construction cost on normal first entry.
  Explicit Float remains available for the current document window; after that
  floating window is closed, the next Pipeline entry returns to the central
  workspace.
- On the current workstation, the measured two-Step first open changed from
  860 ms to 661 ms; internal activation changed from 329 ms to 214 ms. Central
  same-context reopen reached the visible control in 311-322 ms and completed
  internally in 10-12 ms. The reported three-second delay was not reproduced.
- Korean/English Recipe Manager and dock-cycle smokes, Debug build, readiness
  13/13, actual-EXE Wide/Compact control visibility, and no-execution/layer/
  routing checks passed. Evidence:
  `docs/reports/OPENVISIONLAB_RECIPE_MANAGER_PIPELINE_ENTRY_UX_20260809.md`.

### P292 Tool View Dock And Interaction UX - Complete In Dev

- Native algorithm Tool Views remain in the right inspector, while Pipeline
  Review now docks in a central document workspace with its complete review
  surface available in Wide and Compact layouts.
- The P291 same-context document survives Return to Recipe and reopens in the
  central workspace. The current dock-cycle regression confirms no automatic
  Preview/Run, layer-count, active-layer, or routing change.
- The Compact no-image guide now shows all four steps, four actions, and its
  operator hint. Common Tool View, dock-header, and title-bar buttons expose
  themed focus/hover/pressed states and localized accessible names.
- Recipe Manager top and advanced-review toggles, action buttons, and tabs now
  expose themed hover/pressed/focus states. The actual-EXE top toggle changed
  zero pixels before the fix and 884 pixels after it; Korean and English
  Recipe Manager round-trip smokes pass against the central document workspace.
- Current-build Korean/English actual-EXE Wide/Compact runs passed on the
  leftmost monitor. All 16 native Tool Views including Affine Transform passed
  open/layer regression; Debug build, readiness 13/13, references, and public
  assets passed. Evidence:
  `docs/reports/OPENVISIONLAB_TOOLVIEW_DOCK_INTERACTION_UX_20260809.md`.

### P291 Pipeline Review Reopen Performance - Complete In Dev

- Command enablement no longer enumerates sample/catalog files during WPF
  `CanExecute` reevaluation. The real command still rebuilds and validates the
  catalog immediately before execution.
- Returning from a floating Pipeline Review now suspends one exact
  Recipe/Pipeline document and hides its window. Reopening the same context
  restores that document and refreshes layer presentation; context changes,
  another Tool, user close, docked return, and application close still dispose
  it.
- Current-build actual-EXE reopen time is 96-115 ms across five cycles, down
  from the 423-451 ms baseline. All cycles remained responsive and the visible
  review stayed unexecuted with no automatic Preview/Run.
- Debug build, Recipe Manager summary smoke, Recipe Context Switch smoke,
  readiness 13/13, and patch hygiene passed. Evidence:
  `docs/reports/OPENVISIONLAB_PIPELINE_REOPEN_PERFORMANCE_20260809.md`.

### P290 Shared Analysis Runtime Stability Review - Complete In Dev

- The shared project analysis was adopted as a bounded decision input, not as
  an undifferentiated P0/P1 roadmap. OpenVisionLab remains a deterministic
  rule-based vision Recipe workbench; equipment integration, Worker Process,
  plugin SDK, installer/signing, and new algorithms were not activated.
- Pipeline timeout/cancellation now classifies the deadline and drains the
  already-started in-process Step before disposing run-owned input, Context,
  late result image, or run result. This prevents detached work from using
  disposed state; it is not hard termination of a hung native call.
- Sample verification awaits Recipe execution end to end with cancellation.
  Unexpected dispatcher exceptions remain fatal after logging; only expected
  cancellation is marked handled.
- Indexed Bitmap conversion, OpenGL glyph allocation/deletion, log cleanup
  visibility, and caller-frame capture now have explicit resource/diagnostic
  ownership and focused regression coverage.
- Debug solution build, runtime stability contract, readiness 13/13, external
  references, public assets, documentation index, and current-source WPF view
  lifecycle/render checks passed. Evidence:
  `docs/reports/OPENVISIONLAB_SHARED_ANALYSIS_STABILITY_REVIEW_20260806.md`.

### OpenVisionLab 2.1.0 RC1 Publication - Complete

- Annotated tag `v2.1.0-rc.1` points to original commit
  `9ee613676940fd3f593ec45f7e5a96f7a5880e36`; the GitHub release is published
  with `Pre-release`, not stable/latest, intent.
- A new short D-drive clone passed the full Release candidate command without
  `-SkipLaunch`: Debug/Release zero-warning builds, readiness 13/13, vendored
  references, 33 public sample rows, 77-file package, SHA-256, and copied EXE
  launch.
- Hosted CI passed on the same commit. All five public assets were downloaded
  again and matched GitHub digests and pre-publication hashes. The versioned
  ZIP SHA-256 is
  `0F8851599CC8ABFA51B4828CF414F4E2F7030CCCCD4B1DBECBFA6C2E0535E733`.
- Release:
  <https://github.com/Noah8218/OpenVisionLab/releases/tag/v2.1.0-rc.1>
- Evidence:
  `docs/reports/OPENVISIONLAB_2_1_0_RC1_PUBLICATION_20260805.md`.

### P289 OpenVisionLab Vision SDK 3.0 Migration - Complete

- Dev references now use `OpenVisionLab.Core`, `OpenVisionLab.Vision2D`, and
  `OpenVisionLab.Vision2D.Blob` from the tracked manifest-verified SDK 3.0 DLL
  set. The former Library-Noah and duplicate managed OpenCvSharp payloads are
  removed.
- The SDK-removed bitmap converter is retained as an independently verified
  application-owned exact port. Detected-point Affine app metadata is consumed
  at the application boundary before the SDK's strict property factory runs;
  unknown SDK parameters still fail closed.
- SDK Release and 142/142 inspection smoke, isolated package consumer, locked
  restore, Debug/Release zero-warning builds, readiness, external hashes,
  public assets, fixture, Affine, edge polarity, XML, snapshot, focused UI, and
  clean runtime EXE checks passed.
- P289 was committed and pushed as Dev `9068a9bd9e58` and exact-ported to
  original `c34adad70efd`; the publication record followed in both repositories.
- Evidence:
  `docs/reports/OPENVISIONLAB_VISION_SDK_3_MIGRATION_20260805.md`.

### P288 Responsive Shell Scale - Complete

- The complete WPF shell now scales from the 1600 x 900 reference layout up to
  1.5 based on the smaller logical window dimension, preserving the reference
  layout while making 1920 x 1032 and 2560 x 1392 work areas more readable.
- The title bar, navigation, toolbar, workspace guidance, log panel, status bar,
  caption hit area, and resize border scale together; Windows continues to own
  DPI conversion.
- Current-source captures passed at 1600 x 900, 1920 x 1032 maximized, 2560 x
  1392, and compact tool-rail layout. The current EXE also matched the leftmost
  monitor work area exactly without running Preview/Run or changing layers.
- The maximized-window screenshot check now resolves the hosting monitor rather
  than incorrectly comparing a non-primary window with the primary work area.
- Evidence:
  `docs/reports/OPENVISIONLAB_RESPONSIVE_SHELL_SCALE_20260805.md`.

### P287 GitHub CI UTF-8 Source Repair - Complete

- Five tracked C# files were converted from CP949 to UTF-8 without BOM.
- Strict source/target comparison passed for all five files with identical
  decoded characters and identical per-file CRLF/LF counts.
- The repository-wide strict scan passed for all 1,437 tracked text files.
- A clean D-drive snapshot passed the GitHub Actions Release candidate command:
  Debug/Release with zero warnings and errors, readiness 13/13, references,
  public assets, all 33 public sample rows, and the 78-file package contract.
- Dev commit `28bd8501f659169d02d6c5ccf951419b9feea53b` passed Actions run
  `30995729839`. Its exact 15-file port is original commit
  `a17cfe6bdb48f2e583cc7e9d46fc7afd4dd4bca4`, which passed Actions run
  `30995933851`.
- Evidence:
  `docs/reports/OPENVISIONLAB_GITHUB_CI_UTF8_SOURCE_REPAIR_20260805.md`.

### P286 .NET 8 SDK Compatibility - Complete

- `global.json` declares SDK `8.0.100` with `major`, preferring an installed
  compatible 8.x SDK and allowing the 9.x SDK bundled with current Visual
  Studio 2022 when no 8.x SDK is present.
- The source-build verifier reports the selected SDK and validates the minimum
  SDK 8 boundary and maximum SDK 9 boundary instead of requiring exact SDK
  `8.0.421`.
- Locked restore, Debug, Release, readiness 13/13, and vendored references
  passed with isolated SDKs `8.0.100`, `8.0.300`, and `9.0.316`. New D-drive
  Git checkouts with no prior outputs passed with `8.0.300` and the Visual
  Studio 2022 SDK 9/.NET 8 runtime combination.
- README now makes Visual Studio 2022 17.8+ with `.NET desktop development` the
  recommended path. Git, PowerShell, and a separate SDK are needed only for
  the command-line alternative.
- Evidence:
  `docs/reports/OPENVISIONLAB_VISUAL_STUDIO_2022_SOURCE_BUILD_20260805.md`.

### P285 Document Control-Plane Cleanup - Complete

- The global OpenVisionLab block no longer owns a stale LLM-first product
  identity, Pin skill priority, or mandatory full-history read bundle. It now
  routes each task through the nearest project `AGENTS.md` and
  `docs/LLM_DOCUMENT_INDEX.json`.
- Project `AGENTS.md` no longer embeds 96 P-number evidence records. Current
  invariants remain there; P history is discoverable through reports and the
  historical route.
- The live handoff shrank from 448,525 bytes to about 7 KB. Its former content
  is preserved at
  `docs/admin/archive/OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_THROUGH_P284.md`.
- Four unindexed current-sounding legacy documents moved under explicit
  `archive` ownership while root compatibility redirects remain valid.
- Documentation validation now uses strict UTF-8 and includes root `.html`
  redirects. The former CP949 XSD redirect is UTF-8.
- Documentation index, all route paths, 101 root redirects, all tracked text
  encodings, Markdown relative links, stale path searches, patch hygiene, and
  readiness 13/13 passed.
- Evidence:
  `docs/reports/OPENVISIONLAB_DOCUMENT_CONTROL_PLANE_CLEANUP_20260805.md`.

### P284 GitHub Clone Release Verification - Complete

- Dev `codex/public-sample-ux-docs` was pushed at
  `eec754950f6a7482f23a5e408e93fff97605d4e2`.
- The reviewed original `main` result was pushed at
  `f550c4338dbc45bed060096d74e1cad083396ae2`.
- The authoritative final clone is
  `D:\OpenVisionLab-TestData\OVL_GitHub_R3` at that exact original commit.
- Debug/Release builds, readiness 13/13, vendored references, public assets,
  all 33 public sample rows, 78-file package, and copied-location launch passed.
- Final framework-dependent archive SHA-256:
  `96B2AE514C68A107F7F9B58A846DEA0AAC8CC4A8A650912E9F0162080224998E`.
- A deep checkout with Windows long paths disabled reproduced a 261-character
  WPF generated path. Release verification now rejects that condition before
  build work and points to a short checkout root.
- Repository evidence:
  `docs/reports/OPENVISIONLAB_GITHUB_CLONE_RELEASE_VERIFICATION_20260803.md`.

### Recent Supporting Completions

| Work | Current result | Evidence |
| --- | --- | --- |
| 2026-08-06 Tool/panel and workspace restore regression | all 17 sidebar Tool/Pipeline entries open; floating Tool windows follow the owner monitor; Matching -> Main image reload -> close/reopen -> Line switch leaves no stale Preview or automatic run; last recipe and last image directory restore with safe fallback | `D:\OpenVisionLab-TestData\OpenVisionLab\tool_audit_20260806` |
| P283 restored-workstation recovery | SDK/build/readiness/references/XML/public samples/current EXE and Guide checks passed | D-drive recovery report referenced by the P284 report |
| P282 localized user manual | Korean/English language-at-click Guide, schema-2 manifest and exact hashes passed | `docs/reports/OPENVISIONLAB_LOCALIZED_USER_MANUAL_20260801.md` |
| P278 local data externalization | approved generated/test/cache paths are physically D-backed; tracked source remains portable | `docs/reports/OPENVISIONLAB_LOCAL_DATA_EXTERNALIZATION_20260731.md` |
| P277 document discovery | human entrypoint, machine routes, canonical paths and redirect checks established | `docs/reports/OPENVISIONLAB_LLM_DOCUMENT_DISCOVERY_20260731.md` |
| P276 source layout migration | application/library ownership moved under `src` with build and contract evidence | `docs/reports/OPENVISIONLAB_SRC_LAYOUT_MIGRATION_20260731.md` |
| P274 runtime data root | Release installation files remain immutable; writable state is external | `docs/reports/OPENVISIONLAB_RUNTIME_DATA_ROOT_V1_20260730.md` |

## Current Priority And Activation Conditions

- Current activation order after the 2026-08-27 external ImageCanvas marker
  extension:
  1. keep the bounded 2D geometry/3D projection contract and its validated
     external marker presentation immutable unless a named calibration,
     reproduced correlation, or consumer regression is supplied | Recommended
     model: none until that scope exists | Reasoning effort: none until that
     prerequisite exists;
  2. define a separate contract only if direct marker presentation in the main
     OpenVisionLab Pipeline Review is explicitly requested; it is not implied
     by the external ImageCanvas screen evidence | Recommended model:
     gpt-5.6-sol | Reasoning effort: high;
  3. keep the validated
     `IMAGE-CANVAS-EXTERNAL-CONSUMER-CONTRACT` prototype immutable unless a
     named consumer extension or reproduced regression is supplied | Recommended
     model: none until that scope exists | Reasoning effort: none until that
     prerequisite exists;
   4. keep the resolved `P256` route-clarity and `PL-0010` C4 evidence immutable
     unless their source/build/contract/evidence identity changes or a
     regression is reproduced | Recommended model: none until that condition
     exists | Reasoning effort: none until that prerequisite exists;
   5. keep `CVR-00` incomplete and deferred until three independent first-time
     participants and their unedited observations exist | Recommended model:
     none until observations exist | Reasoning effort: none until observations
     exist;
   6. only after an operator reports a measured sequential bottleneck and
     explicitly requests parallelism, audit isolated-worker equivalence and
     thread safety before considering bounded parallel execution |
     Recommended model: gpt-5.6-sol | Reasoning effort: high;
   7. keep `PL-0012` clean-candidate work and `PL-0011` release/tag state as
      separate, explicitly authorized release-stage decisions; do not infer
      publication or deployment from this development work | Recommended model:
      none until a clean candidate and explicit release request exist |
      Reasoning effort: none until that prerequisite exists;
   8. the explicitly reopened `locator-relative-blob-v1` track now has a
      bounded two-sample runtime pilot, and its current evidence contract
      requires two retained locator candidates before `ScoreMargin` can be
      exported or validated; the synthetic native two-candidate path is now
      proven, while the next data prerequisite is a real/native corpus case
      plus operator review, and the skill must remain at pilot status until an
      exact frozen Train/Validation/Held-out corpus and label scope are supplied
      | Recommended model: none until those prerequisites exist | Reasoning
      effort: none until those prerequisites exist.

- Original Artifact cleanup and the recurrence-prevention workflow policy are
  complete in Dev and original. No further repository storage correction is
  active without new current-usage evidence.
- The 2026-08-11 Contour first-open, Line signal cue, ROI editing, Recipe
  loading-lifetime, and Tool Preview large-viewer corrections are complete,
  independently verified, and pushed in Dev `5134e43c` and original
  `32bc70c`. No PR or Release publication is active.
- The 2026-08-11 Recipe/layer/N-image/Edge regression batch is complete,
  independently verified, and pushed in Dev and original. No PR or Release
  publication is active.
- The main `Pipeline 열기` direct-EXE cache/logging and F5 dock-mode corrections
  are complete in Dev and original. Dev returned in `23/19/19 ms` across three
  F5 processes and `15 ms` in a fresh direct EXE; the independently rebuilt
  original returned in `20 ms` and reached UI idle in `54 ms` from its actual
  Visual Studio F5 main-screen button. No PR, tag, or Release is active.
- P294's earlier direct-EXE evidence is historical support only for that path;
  the current F5 evidence and boundary are recorded in the 2026-08-13 dock-mode
  performance report.
- P293 is complete and verified in Dev. Promotion to the original repository,
  commit, and push remain separate explicitly authorized tasks.
- P292 is complete and verified in Dev. Promotion to the original repository
  remains a separate explicitly authorized task.
- P291 is complete and verified in Dev. Promotion to the original repository
  remains a separate explicitly authorized task.
- P290 is complete and verified in Dev. Promotion to the original repository
  remains a separate explicitly authorized task.
- P286 and P287 are published in Dev and original and their hosted GitHub
  Actions runs pass.
- P289 remains complete in Dev and original, and its Vision SDK 3.0 result is
  included in the published `v2.1.0-rc.1` pre-release. The release tag, five
  assets, full local gate, hosted CI, and public-download hash round trip pass.
- Stable/GA promotion is not active. It requires explicit user acceptance of
  RC feedback and a separate distribution decision for any installer,
  signing, update, rollback, uninstall, or self-contained scope.
- The user authorized the 2026-08-23 analysis-driven reliability program. Its
  production Pipeline and direct native UI Preview/Auto MPoint SDK Tool-
  lifetime slices, ImageSpaceFrame transfer, owned Canvas loader/Emgu removal,
  exact 4512 process-resource gate, frozen-Recipe 1,000-run soak, and first
  duplicate-refresh/mipmap reduction are complete in Dev.
- `PL-0004` coordinated display-store/viewer lifetime is complete in Dev. Do
  not reopen it or remove more full-image clones without changed evidence.
- The OpenGL exceptional-cleanup/GPU-allocation/viewer-coordinate slice is
  complete in Dev. Its current-source completion record is
  `docs/reports/OPENVISIONLAB_OPENGL_GPU_COORDINATE_RELIABILITY_20260824.md`.
- P256 is complete in Dev. The current-source walkthrough closed the bounded
  four-Step `Filter -> Threshold -> Morphology -> Blob -> restart -> explicit
  Run Review` route with all four routes preserved and the explicit review
  returning `4 OK / 0 NG / 0 WAIT` in `24.3 ms`. Its completion record is
  `docs/reports/OPENVISIONLAB_P256_FOUR_STEP_ROUTE_CLARITY_20260824.md`.
- P256 is a frozen acceptance for its recorded source/build/fixture/recipe
  hashes and monitor bounds. Do not rerun the same walkthrough or focused
  smoke merely to reconfirm it. Reopen only for a source/build, contract,
  fixture/recipe, runtime/monitor, evidence-validity, harness/measurement, or
  reproduced-regression change; the completion report is the confirmation for
  the unchanged condition.
- The 2026-08-24 desktop TIFF retry is recorded in
  `docs/reports/OPENVISIONLAB_DESKTOP_TIFF_LOAD_20260824.md`. The current Dev
  EXE now fails closed on the observed `System.Drawing.Bitmap` overflow and
  preserves the empty Main-layer/routing state, but the original
  `31,800 x 96,800` TIFF is not loaded for inspection. Do not repeat the same
  retry when its input/source/build/loader/monitor/evidence identity is
  unchanged; reopen only after one of those identities changes or a regression
  is reproduced.
- After the active source-hardening train, the remaining project priority is
  CVR-00, deferred until three independent first-time participants and their
  unedited observations exist. Agent-run recordings are not participant
  evidence. Recommended model: none until observations exist | Reasoning
  effort: none until observations exist.
- Execute the actual-EXE theme, Wide/Compact, DPI, resize, and monitor matrix
  only when the next visible UI slice is admitted. Recommended model:
  gpt-5.6-sol | Reasoning effort: high.
- CVR-00 remains deferred until three independent first-time participants and
  their unedited observations exist. Agent-operated recordings are not novice
  evidence.
- Installer, signing, automatic update/rollback, uninstall, self-contained
  packaging, SBOM/legal review, and multi-PC qualification require a separate
  explicit distribution decision.
- Repeated inspection campaigns, dataset tuning, and LLM/provider validation
  remain closed until the user explicitly names and authorizes one task.
- Commercial tools teach guided configuration, visible result evidence,
  recipe management, and deterministic review. They do not justify expanding
  OpenVisionLab into equipment integration.

## Non-Regression Owners

- Operating boundaries and current product direction: `AGENTS.md`
- Stable UI/runtime behavior: `docs/contracts/openvisionlab/OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`
- Product/view ownership: `docs/roadmap/OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
- Task-specific reading routes: `docs/LLM_DOCUMENT_INDEX.json`
- Localized Guide source: `docs/manual/README.md`
- Release policy: `docs/contracts/openvisionlab/OPENVISIONLAB_RELEASE_VERSION_POLICY.md`

## Historical Lookup

- Former cumulative current handoff through P284:
  `docs/admin/archive/OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_THROUGH_P284.md`
- Detailed chronology through P276:
  `docs/admin/OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`
- Completion reports: `docs/reports`
- Historical evidence packages: `docs/evidence`
- Do not use old readiness percentages, dated plans, or archived `Next Work`
  documents as a current priority.

## Restart Checklist

1. Run `git status --short` and `git log --oneline -5` in
   `C:\Git\2D\Dev`.
2. Read `AGENTS.md`, `docs/README.md`, and this file.
3. Load the matching `routes[].read` list from
   `docs/LLM_DOCUMENT_INDEX.json`.
4. State the immediate priority, remaining priority, product identity,
   evidence-based maturity, commercial lesson, and excluded scope before
   changing anything.
5. Do not touch `C:\Git\2D\Original` or push unless the user explicitly asks.

## Latest continuation: external ImageCanvas 3D-to-2D marker consumer (2026-08-27)

The bounded external consumer slice now includes a read-only reverse-projection
display. `ImageCanvasExternalProjectionResultReader` validates the shared
schema/transaction/dimension contract, `ImageCanvasExternalConsumerSession`
replaces only its projection marker IDs, and the WPF sample coalesces Result
file notifications on the dispatcher. It does not invoke Pipeline/Run,
acknowledge a Handoff, or mutate layers/routes.

Final cross-process evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\projection\cross-modal-projection-20260827-163714-c93911d218f94f7e9b4586a7bda7dcae\cross-modal-projection-summary.txt`;
- `D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\projection\cross-modal-projection-20260827-163714-c93911d218f94f7e9b4586a7bda7dcae\2d-screen-consumer\cross-modal-2d-screen-smoke.txt`;
- `D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\projection\cross-modal-projection-20260827-163714-c93911d218f94f7e9b4586a7bda7dcae\2d-screen-consumer\cross-modal-2d-screen.png`.

The screen started before 3D Result publication and passed with five separate
processes, `4` 2D overlays, `4` forward points, `5` reverse points, one
projection refresh, five visible markers, monitor containment, and explicit
`PipelineExecution=NotInvoked` / `LayerMutation=NotInvoked` evidence. The
screen used dynamically selected `\\.\DISPLAY2` at `1.25x1.25` DPI.

The external ImageCanvas report and manifest are the reusable contract records:
`docs/reports/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_CONTRACT_20260827.md`
and
`docs/contracts/openvisionlab/OPENVISIONLAB_IMAGECANVAS_EXTERNAL_CONSUMER_MANIFEST.json`.
The main OpenVisionLab Pipeline Review is intentionally not connected by this
slice. Quiet-host performance and unsupported DPI values remain unverified.

## Latest continuation: LLM `locator-relative-blob-v1` Phase E pilot (2026-08-27)

The user explicitly reopened the LLM maintenance-mode track after reviewing the
full GPT Pro 2D analysis. The bounded first skill is now implemented as a typed
Plan + evidence-packet contract and deterministic compiler. It does not change
the product identity: OpenVisionLab remains a rule-based OpenCvSharp4 recipe
workbench, and LLM remains optional XML authoring assistance.

The fixed graph is `Matching(2) -> Matching(1 fixture LocatorFrame) ->
RotateScale NormalizeImage -> Threshold -> fixed reference-coordinate Blob`.
The model can select only a CandidateId from a hash-verified evidence packet;
unknown IDs, stale source/template/overlay hashes, wrong frames, duplicate
native indexes, multiple accepted candidates, and compile-without-evidence fail
closed. Guided Setup exposes the template and reuses the existing Hybrid
locator/Blob input panels. ResultCount is measurement-only unless Min=Max is an
explicit operator-owned exact gate.

Current contract and scoped evidence:

- `docs/contracts/openvisionlab/OPENVISIONLAB_LOCATOR_RELATIVE_BLOB_INTENT_SKILL.md`
- `docs/reports/OPENVISIONLAB_LLM_LOCATOR_RELATIVE_BLOB_PHASE_E_20260827.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-v1-contract-20260827`

The contract smoke passes the positive Plan/pipeline/packet/compile path and
negative unknown-ID, hash-mismatch, wrong-frame, ambiguity, tampered-ROI, and
missing-evidence cases. A current-build runtime pilot now also passes for the
public Good/missing-Bad pair: retained Matching candidates are rendered into
current overlays, exported with source/template/overlay hashes, reloaded, and
compiled through the CandidateId-only boundary before an explicit replay. The
pilot evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-runtime-pilot-20260827-final`;
its manifest explicitly says `PilotOnly=true` and `Qualification=false`.
Each sample folder also retains a source copy, `pipeline.xml`, locator overlay,
fixed-ROI/Blob annotated overlay, result image, packet, and metrics summary.

The current Guided Setup now provides the operator-facing packet slice: `Packet
로드` verifies and displays packet metadata/candidates/overlay, while
`검증·Compile` compares the packet with the current settings and prepares only
the deterministic XML draft. XML validation, Import, and the existing explicit
Run remain separate, and packet review/compile does not mutate layers or
routing. The current source/build UI smoke passed at 1600x900 with the packet
path, review text, overlay, and no-auto-run state visible:
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-ui-20260827-packet\wpf_shell_host_recipe_locator_relative_blob_guided_setup.png`.

This remains a two-sample runtime pilot, not algorithm qualification. Frozen
Train/Validation/Held-out replay, Recipe/Run History packet linkage, and the
actual EXE theme/Wide/Compact/DPI/resize/mouse/keyboard matrix remain open.
Provider API, browser automation, per-image tuning, and release/deployment stay
out of scope.

## Latest continuation: external corpus intake for `locator-relative-blob-v1` (2026-08-27)

The user supplied `E:\라벨테스트` as a varied sample source. A current recursive
inventory found 33 top-level dataset directories, 153,988 files, and 98,619 raster
images. The reproducible inventory is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-inventory-20260827\inventory.json`.

The bounded first connection reuses the existing native Die Pad evidence instead of
inventing a second dataset framework. It selects
`EasyMatch_Die_Pad_500(1)\EasyMatch_Die_Pad_500`, `source_file=Die Pad 1.bmp`,
122 rows (train 82 / val 27 / test 13; role labels OK 62 / NG 60). The new
`--locator-relative-blob-corpus-pilot` mode in
`tools/LocatorRelativeBlobSkillSmoke/Program.cs` validates the native batch,
executes the deterministic five-step locator-relative Blob pipeline per source,
exports only hash-verified packets, compiles CandidateId-only XML, and replays the
compiled XML without an LLM.

Current evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260827-die-pad1-final`.
It contains 122 row records, 120 packets, 120 compiled replay matches, 120 locator
overlays, 120 Blob overlays, and two explicit locator rejections:
`train_NG_die_pad_026_ng` and `val_NG_die_pad_198_ng`. The result is intentionally
`PARTIAL`/exit 1 because those rows have no locator candidate; the rejection and
reason are preserved in `intake.csv` and `corpus-freeze-manifest.tsv` rather than
being converted into success. `PilotOnly=true` and `Qualification=false` remain.

This run also exposed a qualification caveat: every accepted row retained one
candidate, and the current ScoreMargin metric uses best-score minus zero when a
second candidate is absent. That is not equivalent to observing a real competing
candidate margin. Before qualification, verify second-candidate provenance or add a
distinct missing-second state. Blob ResultCount varied from 0 to 4 and remains
measurement evidence, not OK/NG truth. Other dataset families, multi-ROI/mask,
per-image ROI, physical labels, and Recipe/Run History packet linkage remain out of
scope until the operator names a separate intent and approves its contract.

## Latest continuation: locator second-candidate evidence gate (2026-08-29)

The locator evidence boundary is now fail-closed at both export and packet
validation. `OpenVisionRecipeLocatorRelativeBlobEvidenceExporter` refuses to
create a packet when the runtime `VisionPipelineMatchResultStore` retains fewer
than two candidates, even if the generic runtime metric contains a numeric
`ScoreMargin`. The evidence packet schema is explicitly revised to
`locator-relative-blob-evidence-v1.1`; its save/load/compile validation applies
the same minimum while preserving the existing rule that exactly one candidate
is accepted. The `locator-relative-blob-v1` skill ID and global Matching metric
enrichment remain unchanged.

The corpus intake now records `LocatorScoreMarginState` as
`ObservedSecondCandidate`, `MissingSecondCandidate`, or `Unavailable`, and does
not copy the numeric margin into the CSV unless two candidates are present. A
fresh run over the existing 122-row Die Pad 1 native batch produced the expected
fail-closed boundary: 0 packets, 0 replays, 120 `MissingSecondCandidate` rows,
2 `Unavailable` rows, and 0 processing errors. The command returned
`PARTIAL`/exit 1 because no row currently supplied positive two-candidate
evidence; this is an evidence prerequisite, not a processing exception.

A separate bounded native provenance probe used the existing public synthetic
source and template, added a deliberately degraded second instance, and passed
through the real Matching runtime. It retained two candidates with scores
`100`/`82.205`, observed margin `17.795`, exported a v1.1 packet with one
accepted candidate, and completed CandidateId-only compile plus same-outcome
replay. At the user's direction, this explicitly selected public fixture and
fixed ROI were rerun in the current smoke check. The latest evidence is at
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5`.
This proves the positive implementation path only; it does not qualify the
external corpus or a physical locator.

Current code/build and evidence are recorded in:

- `docs/contracts/openvisionlab/OPENVISIONLAB_LOCATOR_RELATIVE_BLOB_INTENT_SKILL.md`
- `docs/reports/OPENVISIONLAB_LOCATOR_RELATIVE_BLOB_SECOND_CANDIDATE_GATE_20260829.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-v1.1-contract-20260829`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-dual-candidate-probe-20260829-v5`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-corpus-pilot-20260829-second-candidate-gate-final2`

The remaining prerequisite is a real/native corpus run that retains two
competing locator candidates and passes the ambiguity gate for an approved
unique locator, followed by operator review of which corpus rows may enter
frozen Train/Validation/Held-out subsets. Existing 2026-08-27 one-candidate
packets remain historical evidence without automatic migration and are not
current observed-margin evidence.

## Latest continuation: external Die Array second-candidate provenance (2026-08-29)

The historical P227 `Die Array / Die1.tif` candidate was used as a bounded
diagnostic input because its prior review already stopped it as a repeated-grid
candidate rather than a unique physical locator. The new
`--locator-relative-blob-external-die-array-provenance` mode runs the current
native Matching path with `NUM_MATCH=2` against the preserved
`die_array_003_ok.jpg` source and the prior 96x96 template evidence.

The current run retained two native candidates: score `100` at `192,176` and
`96.276` at `56,464`, with a real `ScoreMargin=3.724`. The Matching tool
succeeded, but the locator ambiguity gate returned `acceptance=false` because
the current requirement is `10`; the overall pipeline consequently reports
`runtime_success=false`. This is the intended fail-closed result. The same run
was passed to the actual evidence exporter, which returned `false` with
`The runtime locator did not pass its explicit success and ambiguity gates.`;
it returned no packet or exporter overlay path and left `evidence.packet.json`
absent. The probe passed its bounded provenance criterion; current visual
correspondence remains `NOT_REVIEWED`.

Evidence:

- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2\candidate-provenance.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2\probe-summary.txt`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2\locator-runtime-overlay.png`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2\source.jpg`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-die-array-provenance-20260829-r2\locator-template.png`

This confirms real external second-candidate retention, ambiguity rejection, and
actual exporter packet refusal; it does not approve the Die Array locator, lower
the margin threshold, create a v1.1 packet, or qualify the skill. The next
prerequisite is an operator-approved real/native locator that retains two
candidates and passes the ambiguity gate, then a frozen
Train/Validation/Held-out review scope.

## Latest continuation: Die Pad 188 Matching detection-power study (2026-08-29)

The user explicitly reopened a bounded image-validation task to improve
Matching detection power while preserving the approved same-source `188` r2
material-boundary template, ROI `(169,172,188,142)`, teaching angle `0°`, and
physical correspondence requirement. A fixed 500-image corpus (`NG 250 + OK
250`) was replayed with per-image source, result, and runtime overlay evidence.

The study-level global candidate `USE_FIND_SCALE=true`, `FIND_SCALE_MIN=0.90`,
`FIND_SCALE_MAX=1.10`, `FIND_SCALE_STEP=0.05` with the existing
`CCoeffNormed`, `SCORE_MIN=0.60`, `NUM_MATCH=3`, angle `-10..+10°/0.1°`, and
Canny disabled was selected. It found `500/500`, versus `445/500` for fixed
scale and `480/500` for the lower-score-floor diagnostic. The selected candidate
preserved all 445 baseline detections and recovered the 55 previous NoResult
rows. Representative ordinary, rotated, difficult, defective, and recovered
overlays all remained on the two Pads, hole order, Trace entries, and
lower/right physical boundary.

The accuracy result is not an operating-default or release qualification: mean
elapsed increased from `599.696 ms` to `3,754.370 ms` (about 6.3x), and the
study did not include a held-out corpus or individual human review of every
overlay. Evidence and the complete candidate table are recorded in
`docs/reports/OPENVISIONLAB_MATCHING_DETECTION_POWER_STUDY_20260829.md` and
`D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-full`.

The rule-based teaching skill now routes explicitly authorized detection-power
studies through a finite global scale-sensitivity check before lowering
`SCORE_MIN` or changing the locked template:
`C:\Users\USER\.codex\skills\openvisionlab-rule-based-teaching\references\detection-power-study.md`.
This remains global-only; no per-image fallback or automatic runtime retry was
introduced. The remaining work is a separately authorized held-out visual
qualification and, if needed, a bounded performance study before applying the
scale candidate as an operating default.

## Latest continuation: Die Pad 188 held-out sanity and scale-step performance (2026-08-29)

The separately authorized follow-up was completed in Dev. A SHA-256 audit of
the 550 die-pad `all_images` found only 25 novel-content samples: NG 25. The
external OK 25 files are content duplicates of the existing 500-image
selection and were excluded from independent qualification. The selected
`0.90..1.10 / 0.05` candidate ran on the novel NG 25 with `25/25` pipeline and
step success, `ResultCount=1`, `Error=0`, mean `ScoreMax=92.887`, and mean
elapsed `4,380.993 ms`. Representative current overlays remained on the two
Pads, hole order, L-shaped/right traces, and lower/right physical boundary.
This is an NG-only sanity check; full NG/OK held-out qualification remains
false because no novel OK corpus was available.

A full 500-image performance candidate changed only
`FIND_SCALE_STEP` from `0.05` to `0.10`. It preserved `500/500` pipeline pass,
`StepStatus=OK`, and `ResultCount=1`, but lowered mean `ScoreMax` from `93.084`
to `91.548`, reduced `ScoreMax>=90` from `389` to `340`, and changed `142/500`
result-image hashes. Mean elapsed improved from `3,754.370 ms` to
`2,441.768 ms` (`34.96%`). Because status/count can remain green while the
selected box and score move, the coarse candidate is rejected as the global
accuracy default; retain `STEP=0.05` and revisit `STEP=0.10` only as an
explicit, tolerance-backed performance profile.

Evidence/report:

- `docs/reports/OPENVISIONLAB_MATCHING_SCALE_HELDOUT_PERFORMANCE_20260829.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-step-heldout-performance-summary.json`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\pattern-registration-188-r2-expanded\detection-power-study-20260829\scale-0p90-1p10-step0p10-full-20260829`

The rule-based teaching reference now explicitly requires per-image score and
overlay-geometry parity before a coarse scale step can be promoted. No recipe,
application source, original repository, push, tag, release, deployment, or
EXE launch smoke was changed or executed.

## Latest continuation: Machine Studio 2D locator handoff (2026-08-29)

Status: Complete for the explicit cross-process integration scope.

The Dev 2D consumer now recognizes the
`locator-relative-blob-integration-recipe-v1` recipe, resolves the
hash-checked `locator-template` staged by Machine Studio, and runs the existing
locator measurement pipeline only after explicit acknowledgement and run. It
publishes a correlated v2 Result with the `locator-relative-blob-evidence-v1.1`
packet and persisted runtime overlay. A separate Machine Studio result-reader
smoke validated the same Result through `MachineIntegrationExchange.ReadResult`.

Final evidence is under
`D:\OpenVisionLab-TestData\OpenVisionLab-CrossRepo\locator\locator-cross-repo-20260829-111511-4eaff9f69caa4a9f8344c3d35e99ae17`.
The run returned `Accepted`, `Completed/Pass`, source `572x420`, two retained
candidates, selected `locator-1`, score margin `17.79458522796631`, and packet
plus overlay evidence count `2`. The Machine Studio and Dev reports agree.
The consumer-side implementation note is
`docs/reports/OPENVISIONLAB_MACHINE_STUDIO_2D_LOCATOR_HANDOFF_20260829.md`.

The source is a deterministic synthetic dual-candidate fixture. This validates
the exchange contract, explicit execution boundary, fail-closed evidence
publication, and result reading; it does not qualify the external corpus or a
physical locator. 3D projection, locator-specific UI, performance, PC restart,
commit, push, release, and deployment remain outside this slice.

Next development priority:

1. Obtain operator-approved native two-candidate locator evidence and complete
   frozen Train/Validation/Held-out review | Recommended model: `gpt-5.6-sol` |
   Reasoning effort: `high`.

## Current active task override: Die Pad 188 scale follow-up (2026-08-29)

For the explicitly reopened image-validation task in this thread, the latest
active evidence is the Die Pad 188 held-out sanity and scale-step performance
follow-up. The selected accuracy candidate remains
`FIND_SCALE_STEP=0.05`; the `0.10` performance candidate is rejected as a
global accuracy default because `142/500` result images changed and
`ScoreMax>=90` fell from `389` to `340`, despite `500/500` execution status.
Full NG/OK held-out qualification is still blocked only by the absence of
novel OK content; the exact evidence and boundary are in
`docs/reports/OPENVISIONLAB_MATCHING_SCALE_HELDOUT_PERFORMANCE_20260829.md`.

## Latest continuation: native locator candidate ambiguity gate (2026-08-29)

The next-priority native check was executed in Dev with a finite set taken from
the already recorded P227 `SUGGESTED` rows. A new explicit smoke mode accepts
`source`, `template`, `inspection ROI`, `reference pose`, and a new D-drive output
directory; it does not invent coordinates or alter the existing hard-coded
external Die Array probe. The mode remains `REVIEW_ONLY` and retains exact
source/template hashes, all native `NUM_MATCH=2` candidates, locator overlay,
pose-normalized candidate patches, side-by-side template comparisons, and 50%
blends.

The fixed 13-candidate probe found one numeric ambiguity-gate candidate:
`IC Frame4` with `100` and `83.999`, `ScoreMargin=16.001` against the fixed
minimum `10`. The two Die Array candidates remained negative at margins `3.724`
and `2.922`; ten other candidates did not retain a second native candidate.

The fixed eight-row IC Frame4 pilot retained two candidates and passed the margin
gate on `5/8` rows. The other rows were `1/8` missing-second-candidate and `2/8`
no-result. All five gate-passing rows exported v1.1 packets, reloaded/compiled,
and replayed with the same outcome. Current spot review of ordinary `01-ok` and
defective `197-ng`/`236-ng` rows found the selected bottom-right pin-row plus
diagonal-corner structure visually corresponding, while the competing candidate
was a repeated right-side pin segment without the corner datum.

This is not an approved or qualified locator. The source material is still the
synthetic/labeling compatibility corpus, `REVIEW_ONLY` approval is absent, and
the fixed downstream Blob step returned `ResultCount=0` on the gate-passing
rows. The current result proves only bounded native locator candidate retention,
ambiguity gating, packet replay, and spot-review evidence.

Report and evidence:

- `docs/reports/OPENVISIONLAB_LLM_LOCATOR_RELATIVE_BLOB_NATIVE_CANDIDATE_GATE_20260829.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-candidates-20260829`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-ic-frame4-pilot-20260829-r2`

Remaining prerequisite before further candidate implementation: an operator must
approve this locator intent or provide a replacement physical datum and define
the downstream inspection ROI/tolerance. Until that decision is available, do
not promote IC Frame4 to a Recipe default or start full Train/Validation/Held-out
qualification.

## Latest continuation: locator review-decision handoff contract (2026-08-29)

The native candidate boundary now has a separate file-backed
`locator-relative-blob-review-decision-v1` seam. A successful native candidate
packet produces `review-decision.template.json` beside the packet, but the
template is always `PENDING / NOT_REVIEWED` with no reviewer or review time.
It binds the packet file hash, source/template/overlay hashes, a deterministic
Plan fingerprint, the retained CandidateId, fixed inspection ROI, threshold,
area range, and optional exact ResultCount. Packet or Plan changes fail closed
as stale. Only an explicit `APPROVED` record with `PASS` visual correspondence,
reviewer/time/note, and matching accepted candidate can validate; that state
still does not qualify the locator or authorize release.

Verification completed in Dev:

- `LocatorRelativeBlobSkillSmoke` build: 0 errors, 10 existing nullable
  warnings;
- contract smoke: `PASS` for pending creation/save/load, synthetic
  contract-only approval, and stale packet/Plan rejection;
- fresh `01-ok` native candidate smoke: two candidates, margin `16.001`, packet
  export/reload/compile/replay pass, pending template written;
- fresh evidence confirms `operator_approval=NOT_APPROVED`,
  `visual_correspondence=NOT_REVIEWED`, and `qualification=false`.

Evidence/report:

- `docs/reports/OPENVISIONLAB_LOCATOR_RELATIVE_BLOB_REVIEW_DECISION_CONTRACT_20260829.md`
- `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\locator-relative-blob-external-native-ic-frame4-review-decision-r4`

No WPF runtime, performance test, EXE launch, PC restart, original repository,
commit, push, release, or deployment operation was performed.

Next development priority:

1. Wire the review-decision record into the existing Guided Setup review
   surface and make Recipe promotion consume only a current `APPROVED` record;
   then obtain the operator's actual candidate/ROI decision before frozen
   Train/Validation/Held-out qualification | Recommended model: `gpt-5.6-luna` |
   Reasoning effort: `medium`.

## Latest continuation: review-decision Guided Setup wiring (2026-08-29)

The review-decision contract is now wired into the existing Recipe Manager
Guided Setup surface in Dev. The surface loads a verified Evidence Packet first,
then loads its hash- and Plan-bound decision JSON. It exposes visual
correspondence (`NOT_REVIEWED/PASS/FAIL`), reviewer, notes, decision status, and
explicit `Approve candidate`, `Reject candidate`, and `Request replacement`
commands. A pending template is never treated as approval, and saving a
decision beside `review-decision.template.json` preserves the pending template.

Locator Recipe promotion is fail-closed in two layers: the Import command is
disabled without an in-memory approved decision, and the Import handler reloads
the persisted JSON and revalidates the current packet hash, source/template/
overlay hashes, CandidateId, Plan fingerprint, ROI, threshold, area, and count
before any dependency copy or Recipe mutation. Approval remains separate from
qualification, Preview, Run, layer mutation, and routing.

Verification completed in Dev:

- `LocatorRelativeBlobSkillSmoke`: build 0 errors, 10 existing nullable
  warnings; contract smoke `PASS`;
- `OpenVisionReadinessCheck`: all checks `OK`;
- `PipelineViewerScreenshotSmoke`: build 0 errors, 1 existing nullable
  warning; functional WPF target `OK`;
- the actual WPF window was placed and verified on the dynamically selected
  smaller-left monitor (`-1920,365..0,1397`), with window rectangle
  `-1900,385..-300,1285` and exit code `0`;
- the smoke loaded the current v1.1 packet, verified pending import blocking,
  persisted a synthetic UI-only approval, verified the approved command gate,
  and reloaded the pending template to verify blocking again without Preview,
  Run, or layer-count changes;
- fresh screenshot evidence is at
  `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ui-smoke-locator-review-20260829-r4\wpf_shell_host_recipe_locator_relative_blob_guided_setup.png`.

The synthetic `APPROVED` record is only a plumbing assertion and is explicitly
not an operator or physical-datum approval. The native evidence itself remains
`PENDING / NOT_REVIEWED / NOT_APPROVED / qualification=false`; the downstream
fixed Blob step also remains a known no-result boundary from the native pilot.

Report:

- `docs/reports/OPENVISIONLAB_LOCATOR_RELATIVE_BLOB_REVIEW_DECISION_UI_WIRING_20260829.md`

Status: **Incomplete for the full UI gate**. Implementation, source checks, and
functional smoke passed, but the user deferred performance testing. Full WPF
state/theme/layout/DPI coverage and the project-mandated performance smoke were
not run. No commit, push, release, deployment, original-repository change, or
PC restart was performed.

Next development priority:

1. Have an operator inspect the native candidate comparison artifacts and record
   the actual candidate decision plus downstream ROI/tolerance definition; do
   not use the synthetic smoke record for qualification or promotion |
   Recommended model: `gpt-5.6-sol` | Reasoning effort: `high`.

Deferred verification:

1. Reopen the full WPF completion gate, including performance smoke, only when
   the user authorizes it | Recommended model: `gpt-5.6-luna` | Reasoning effort:
   `medium`.
