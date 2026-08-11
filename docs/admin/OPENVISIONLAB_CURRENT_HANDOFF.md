# OpenVisionLab Current Project Handoff

Updated: 2026-08-11 KST

This is the compact live-status source for a new OpenVisionLab task. Read
`AGENTS.md`, `docs/README.md`, and `docs/LLM_DOCUMENT_INDEX.json` first. Stable
behavior belongs to `docs/contracts`; detailed completion evidence belongs to
`docs/reports`; P-number chronology belongs to the archived handoff and
`OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`.

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
- Dev now consumes the manifest-verified OpenVisionLab Vision SDK 3.0 at source
  commit `ba0055b713e0bf434b9d0a7fd3f4b0e445c1f982`. The predecessor DLL root and
  duplicate managed OpenCvSharp files are removed; standalone SDK, app
  Debug/Release, functional contracts, focused UI, and clean-runtime launch
  checks pass.
- OpenVisionLab `v2.1.0-rc.1` is published from original commit
  `9ee613676940fd3f593ec45f7e5a96f7a5880e36` as an unsigned Windows x64
  framework-dependent GitHub pre-release. Its full local gate, hosted CI,
  copied launch, five uploaded assets, and public-download SHA-256 round trip
  pass.
- This is not commercial GA, installer/signing/update evidence, multi-PC or
  hardware qualification, calibrated metrology, or field robustness.

## Latest Completed Work

### 2026-08-11 Recipe Switch Loading And Responsiveness - Complete In Dev

- The earlier Recipe busy-state claim did not prove that the state reached the
  actual EXE screen. Existing-Recipe selection now yields one WPF render turn
  before synchronous Recipe state restoration and covers Recipe Manager with a
  localized, themed, interaction-blocking loading overlay.
- Recipe selection no longer repeats the Pipeline/validation/history/summary
  refresh already performed by `RecipeState.EventChangedRecipe`. Recipe changes
  also no longer restart the whole native Tool prewarm queue immediately after
  the switch; the explicitly selected Tool still opens through the normal Tool
  selection path.
- In the final actual-EXE capture, the selection request returned in 39.2 ms,
  the loading overlay was visible in captured frames, and the selected Recipe
  and summary were complete by the first stable frame at 433.8 ms. A separate
  responsiveness probe returned in 29.2 ms and was responsive from 414.3 ms
  through its last 1,205.6 ms sample.
- Debug build, readiness 13/13, Recipe switch safety/summary/context focused
  checks, native Tool open, and actual-EXE monitor-placement evidence passed.
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

### P294 Main No-Image Pipeline Open Performance - Complete In Dev

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

### P293 Recipe Manager And Pipeline Entry UX - Complete In Dev

- Recipe lifecycle commands and the name editor now remain together above the
  Recipe Manager workbench in Wide and Compact layouts. A panel-scoped field
  template removes the Wpf.Ui light-template leak, so entered names and all
  normal/focus/hover/read-only/disabled/error states use the shell theme.
- Pipeline Review now defaults to the central document workspace instead of
  paying the separate floating-window construction cost on normal first entry.
  Explicit Float remains available and keeps the operator's floating choice.
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

- The 2026-08-11 Recipe/layer/N-image/Edge regression batch is complete,
  independently verified, and pushed in Dev and original. No PR or Release
  publication is active.
- P294 is complete and verified in Dev. Promotion to the original repository,
  commit, and push remain separate explicitly authorized tasks.
- P293 is complete and verified in Dev. Promotion to the original repository,
  commit, and push remain separate explicitly authorized tasks.
- P292 is complete and verified in Dev. Promotion to the original repository
  remains a separate explicitly authorized task.
- P291 is complete and verified in Dev. Promotion to the original repository
  remains a separate explicitly authorized task.
- P290 is complete and verified in Dev. Promotion to the original repository
  remains a separate explicitly authorized task.
- P286 and P287 are published in Dev and original and their hosted GitHub
  Actions runs pass. No repository reliability correction is active without a
  new verified regression.
- P289 remains complete in Dev and original, and its Vision SDK 3.0 result is
  included in the published `v2.1.0-rc.1` pre-release. The release tag, five
  assets, full local gate, hosted CI, and public-download hash round trip pass.
- Stable/GA promotion is not active. It requires explicit user acceptance of
  RC feedback and a separate distribution decision for any installer,
  signing, update, rollback, uninstall, or self-contained scope.
- No product feature is active without a named operator blocker or a verified
  current-build regression.
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
   `C:\Git\OpenVisionLab_Dev`.
2. Read `AGENTS.md`, `docs/README.md`, and this file.
3. Load the matching `routes[].read` list from
   `docs/LLM_DOCUMENT_INDEX.json`.
4. State the immediate priority, remaining priority, product identity,
   evidence-based maturity, commercial lesson, and excluded scope before
   changing anything.
5. Do not touch `C:\Git\OpenVisionLab` or push unless the user explicitly asks.
