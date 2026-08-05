# OpenVisionLab Current Project Handoff

Updated: 2026-08-05 KST

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
- This is not commercial GA, installer/signing/update evidence, multi-PC or
  hardware qualification, calibrated metrology, or field robustness.

## Latest Completed Work

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
| P283 restored-workstation recovery | SDK/build/readiness/references/XML/public samples/current EXE and Guide checks passed | D-drive recovery report referenced by the P284 report |
| P282 localized user manual | Korean/English language-at-click Guide, schema-2 manifest and exact hashes passed | `docs/reports/OPENVISIONLAB_LOCALIZED_USER_MANUAL_20260801.md` |
| P278 local data externalization | approved generated/test/cache paths are physically D-backed; tracked source remains portable | `docs/reports/OPENVISIONLAB_LOCAL_DATA_EXTERNALIZATION_20260731.md` |
| P277 document discovery | human entrypoint, machine routes, canonical paths and redirect checks established | `docs/reports/OPENVISIONLAB_LLM_DOCUMENT_DISCOVERY_20260731.md` |
| P276 source layout migration | application/library ownership moved under `src` with build and contract evidence | `docs/reports/OPENVISIONLAB_SRC_LAYOUT_MIGRATION_20260731.md` |
| P274 runtime data root | Release installation files remain immutable; writable state is external | `docs/reports/OPENVISIONLAB_RUNTIME_DATA_ROOT_V1_20260730.md` |

## Current Priority And Activation Conditions

- P286 and P287 are published in Dev and original and their hosted GitHub
  Actions runs pass. No repository reliability correction is active without a
  new verified regression.
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
