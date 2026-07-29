# OpenVisionLab Next Chat Handoff Prompt

Updated: 2026-07-29 KST

This is a clean restart prompt, not the detailed history. The live status
authority is `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`; the compact full
commercial-video queue handoff is
`docs\reports\OPENVISIONLAB_COMMERCIAL_VIDEO_QUEUE_HANDOFF_20260728.md`; the
detailed P1-P248 chronology is `docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`.

## Required Reading

Read in this order before changing code or documentation:

1. `C:\Git\OpenVisionLab_Dev\AGENTS.md`
2. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_CURRENT_HANDOFF.md`
3. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_DOCUMENTATION_MAP.md`
4. `C:\Git\OpenVisionLab_Dev\docs\reports\OPENVISIONLAB_COMMERCIAL_VIDEO_QUEUE_HANDOFF_20260728.md`
5. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_COMMERCIAL_VIDEO_DEVELOPMENT_BACKLOG_20260727.md`
6. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md`
7. `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`

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
  recipe management, and repeatable operator validation, not equipment scope.
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
- CVR-01 through bounded CVR-11 are complete. CVR-09 `LineFixture` physical
  qualification remains blocked on its named operator/data packet. CVR-10
  `MultiMatchMean` v1 provides stable row-major multi-match identities, one
  fixed reference-coordinate Mean fan-out, individual review, and aggregate
  acceptance; another per-instance family requires its own named task.
- CVR-11 adds opt-in whole-candidate global edge-polarity reversal with
  Same-only missing-key defaults and synthetic Train/Validation/Held-out
  evidence. Physical qualification still requires its own named packet.
- CVR-12 through CVR-18 activation audits are complete. None admitted
  implementation; use their six-section task packets before reopening them.
- CVR-19 Validation Variant v1 and CVR-20 Overlay Rendering v1 are complete at
  their bounded contracts.
- Concurrent N-image workers are not implemented. Promoted `Expected OK` means
  locator success, not inherited defect truth.
- Broad industrial variation, certified metrology, calibration, unseen-data
  robustness, and field qualification remain unproven.
- `OuterCornerIntersection` remains experimental.
- Detailed completed/rejected/incomplete evidence is indexed in the current and
  chronological handoffs; do not repeat a completed dataset campaign.

## Current Priority

There is no active implementation priority after bounded CVR-20 v1. `CVR-00`
independent novice use remains the active external prerequisite. Physical-task
CVR-09/CVR-11 qualification and another CVR-10 per-instance family require
named operator/data packets. The CVR-12 through CVR-18 activation audits are
complete and did not admit implementation; require their six-section packets
in `docs\reports\OPENVISIONLAB_CVR12_TRIGGER_AUDIT_20260728.md`,
`docs\reports\OPENVISIONLAB_CVR13_TRIGGER_AUDIT_20260728.md`, and
`docs\reports\OPENVISIONLAB_CVR14_TRIGGER_AUDIT_20260728.md`, and
`docs\reports\OPENVISIONLAB_CVR15_TRIGGER_AUDIT_20260728.md`, and
`docs\reports\OPENVISIONLAB_CVR16_TRIGGER_AUDIT_20260728.md`,
`docs\reports\OPENVISIONLAB_CVR17_TRIGGER_AUDIT_20260728.md`, and
`docs\reports\OPENVISIONLAB_CVR18_TRIGGER_AUDIT_20260728.md`. CVR-19 bounded
Validation Variant v1 and CVR-20 Overlay Rendering v1 completed on 2026-07-29;
use `docs\reports\OPENVISIONLAB_CVR19_VALIDATION_VARIANTS_20260729.md` and
`docs\reports\OPENVISIONLAB_CVR20_OVERLAY_RENDERING_20260729.md` as their
completion evidence. The ordered commercial-video queue has no remaining
implementation row. Historical text below that calls CVR-19 or CVR-20 next is
superseded by this update.

The complete ordered status/trigger/model table is in
`docs\reports\OPENVISIONLAB_COMMERCIAL_VIDEO_QUEUE_HANDOFF_20260728.md`.

Do not start another dataset run, recipe tuning cycle, algorithm family,
parallelization project, or LLM campaign merely to keep work moving.

Only after the operator reports a measured sequential bottleneck and explicitly
requests parallel execution should isolated-worker `1/2/4` equivalence and
thread safety be audited.

## Current Git Continuation State

At the 2026-07-28 handoff, Dev was observed on
`codex/public-sample-ux-docs` at `e64a9d0`, one commit ahead of its tracked
origin and dirty with CVR-10/CVR-11 implementation plus CVR-12 through CVR-18
audits, documentation integrity maintenance, and handoff documentation.
Library-Noah `main` was observed at `584f233` and
dirty with CVR-11 source/smoke/document changes.

This is not a commit or publication claim. Rerun status/log. Preserve
unrelated `.codex-temp/` and legacy demo GIF/MP4 files. Do not commit, push, or
touch the original repository unless the active user request explicitly asks.

## Paste-Ready Request

```text
Work in C:\Git\OpenVisionLab_Dev.

Read AGENTS.md, docs\OPENVISIONLAB_CURRENT_HANDOFF.md,
docs\OPENVISIONLAB_DOCUMENTATION_MAP.md,
docs\reports\OPENVISIONLAB_COMMERCIAL_VIDEO_QUEUE_HANDOFF_20260728.md,
docs\OPENVISIONLAB_COMMERCIAL_VIDEO_DEVELOPMENT_BACKLOG_20260727.md,
docs\OPENVISIONLAB_PRODUCT_TARGET_AND_MAIN_VIEWS.md, and
docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md first. Run git status -sb and
git log --oneline -5.

Before changing anything, state the current product identity, evidence-based
maturity without reusing historical percentages, immediate priority, remaining
priority, commercial lessons to emulate, and out-of-scope platform areas.

The default next priority is CVR-00 only when three independent novice
participants and raw observations exist. Named CVR-09/CVR-11 physical packets
or CVR-12/CVR-13/CVR-14/CVR-15/CVR-16/CVR-17/CVR-18 admission packets take
priority when supplied. CVR-19 and CVR-20 are complete. If I ask to continue
without new evidence, report that the ordered queue has no remaining
implementation row; do not fabricate a task or invent CVR-21. Recommended
model: none until evidence exists | Reasoning effort: none until evidence
exists.

Use the current handoff and commercial-video queue handoff as the compact
truth and the chronological handoff only for detailed P-number evidence.
Preserve PropertyGrid tools,
explicit Preview/Run, layer/routing, viewer, drawing, and no-auto-run contracts.
Design every admitted feature from the operator goal and shortest safe normal
workflow. Consolidate related durable settings into one coherent first-use
setup, persist them only after explicit confirmation at the narrowest correct
scope, restore them visibly with reset/stale-state handling, and verify
save/reload/reopen with zero unintended Preview/Run, layer, or routing changes.
Read
`docs\reports\OPENVISIONLAB_USER_CENTERED_WORKFLOW_DIRECTION_20260729.md`
for the reusable admission template.
Do not reopen rejected candidates, repeated dataset tuning, parallel execution,
or LLM expansion without the named prerequisite and explicit request.

Preserve all dirty work. Do not commit, push, or import into
C:\Git\OpenVisionLab unless I explicitly request that action in the active
chat.
```
