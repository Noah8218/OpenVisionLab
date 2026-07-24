# OpenVisionLab Next Chat Handoff Prompt

Updated: 2026-07-24 KST

This is a clean restart prompt, not the detailed history. The live status
authority is `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`; the detailed P1-P235
chronology is `docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`.

## Required Reading

Read in this order before changing code or documentation:

1. `C:\Git\OpenVisionLab_Dev\AGENTS.md`
2. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_CURRENT_HANDOFF.md`
3. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_DOCUMENTATION_MAP.md`
4. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
5. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`

Read the XML guide/catalog and sample/external/release policies only when the
task touches those areas. Search the chronological handoff by P-number when
detailed evidence is needed.

## First Commands

```powershell
cd C:\Git\OpenVisionLab_Dev
git status -sb
git log --oneline -5
```

When original-repository synchronization or publication is in scope, also run:

```powershell
cd C:\Git\OpenVisionLab
git status -sb
git log --oneline -5
gh auth status
```

## Current Product Truth

- OpenVisionLab is an OpenCvSharp4 rule-based vision recipe workbench.
- Direct PropertyGrid teaching, Pipeline composition, explicit Preview/Run,
  layers, drawings, deterministic N-sample review, and saved recipes are the
  product core.
- LLM XML assistance is optional and frozen in maintenance mode by P196.
- It is not an arbitrary-image autonomous inspection generator.
- It is not a camera, lighting, PLC/I/O, MES, account, deployment, or
  industrial-controller platform.
- Commercial products teach OpenVisionLab to improve guided configuration,
  fixture/relative coordinates, Caliper/segmentation evidence, drawings,
  recipe management, and repeatable operator validation—not equipment scope.
- Historical percentages such as 62-66% or 98% are not current release claims.

## Current Engineering State

- P197-P217 complete bounded deterministic fixture, measurement, result-review,
  calibration, and object-filter slices.
- P218-P221 complete Affine core/wiring and one coarse fixed-ROI linkage; P220
  remains incomplete at its separate frozen `<=3 px` gate.
- P222-P231 complete Auto MPoint teaching/reporting and one Die Pad 1
  qualification; the card `R` locator was rejected and other strata are not
  automatically qualified.
- P232-P235 complete sequential shared Tool View N-image verification, one
  24-image real-folder acceptance, and hash-locked locator expected-success
  promotion into Recipe Manager.
- Concurrent N-image workers are not implemented. Promoted `Expected OK` means
  locator success, not inherited defect truth.
- Broad industrial variation, certified metrology, calibration, unseen-data
  robustness, and field qualification remain unproven.
- `OuterCornerIntersection` remains experimental.
- Detailed completed/rejected/incomplete evidence is indexed in the current and
  chronological handoffs; do not repeat a completed dataset campaign.

## Current Priority

There is no active implementation priority after P235/P236. Wait for a concrete
operator workflow blocker or a verified current-build regression.

Do not start another dataset run, recipe tuning cycle, algorithm family,
parallelization project, or LLM campaign merely to keep work moving.

Only after the operator reports a measured sequential bottleneck and explicitly
requests parallel execution should isolated-worker `1/2/4` equivalence and
thread safety be audited.

## Publication State

The user explicitly requested publication of the consolidated Dev state and a
reviewed Git-based import into the original repository. At the P236 prerequisite
check:

- Dev: `C:\Git\OpenVisionLab_Dev`, branch `codex/public-sample-ux-docs`.
- Original: `C:\Git\OpenVisionLab`, branch `main`, clean.
- GitHub CLI was installed but not authenticated.

Before resuming publication, rerun the Git commands above. Never stage
`.codex-remote-attachments/`, never bulk-copy Dev over original, and never claim
a push until the observed command succeeds.

## Paste-Ready Request

```text
Work in C:\Git\OpenVisionLab_Dev.

Read AGENTS.md, docs\OPENVISIONLAB_CURRENT_HANDOFF.md,
docs\OPENVISIONLAB_DOCUMENTATION_MAP.md,
docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md, and
docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md first. Run git status -sb and
git log --oneline -5.

Before changing anything, state the current product identity, evidence-based
maturity without reusing historical percentages, immediate priority, remaining
priority, commercial lessons to emulate, and out-of-scope platform areas.

Use P236's current-state ledger as the compact truth and the chronological
handoff only for detailed P-number evidence. Preserve PropertyGrid tools,
explicit Preview/Run, layer/routing, viewer, drawing, and no-auto-run contracts.
Do not reopen rejected candidates, repeated dataset tuning, parallel execution,
or LLM expansion without the named prerequisite and explicit request.

If publication is still pending, verify GitHub authentication and both
repositories, commit/push Dev, import by Git patch/commit into the clean
C:\Git\OpenVisionLab original repository, build independently, then commit/push
original. Do not copy the repository directory wholesale and do not stage
.codex-remote-attachments.
```
