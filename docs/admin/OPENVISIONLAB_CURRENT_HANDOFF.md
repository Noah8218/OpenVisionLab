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
- This is not commercial GA, installer/signing/update evidence, multi-PC or
  hardware qualification, calibrated metrology, or field robustness.

## Latest Completed Work

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
