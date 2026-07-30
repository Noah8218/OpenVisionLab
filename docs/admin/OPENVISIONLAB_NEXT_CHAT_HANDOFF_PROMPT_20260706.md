# OpenVisionLab Next Chat Handoff Prompt

Updated: 2026-07-30 KST

This is a clean restart prompt, not the detailed history. The live status
authority is `docs\OPENVISIONLAB_CURRENT_HANDOFF.md`; the compact full
commercial-video queue handoff is
`docs\reports\OPENVISIONLAB_COMMERCIAL_VIDEO_QUEUE_HANDOFF_20260728.md`; the
detailed P1-P252 chronology is `docs\OPENVISIONLAB_NEXT_SESSION_HANDOFF.md`.

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

The ordered commercial-video queue remains complete through bounded CVR-20.
After CVR-00 participants became temporarily unavailable, the user explicitly
authorized bounded video-gated operator development without treating agent
recordings as participant evidence. The Pipeline-definition/result,
detection-candidate, zero-size ROI/full-image wording, and direct-Teaching
Basic/Fast/Precise plus threshold-rationale slices are complete.

The user then explicitly directed development to use the many existing samples
and address missing commercial workflows rather than looking for more sample
data. P250 closes the first reproduced non-algorithm gap: the selected catalog
OK/NG pair can now be saved as a recipe-local Validation Set in one explicit
action with roles, hashes, Variants, and single or multi-metric gates retained.
Evidence:
`docs\reports\OPENVISIONLAB_CATALOG_PAIR_VALIDATION_SET_20260729.md`.

P251 then closes the reproduced failure-to-correction handoff. One explicit
`Prepare correction` action now selects the retained failed Step, respects
pending edits, loads its PropertyGrid and exact retained sample, and opens
XML/Steps without Preview/Run, layer creation/deletion, workspace selection, or
route mutation. The existing explicit rerun remains the final operator action.
Evidence:
`docs\reports\OPENVISIONLAB_FAILURE_CORRECTION_HANDOFF_20260729.md`.

P252 closes the remaining rerun-scope mismatch. Local Validation Set history
now reruns that same saved set and persists a comparable new Run History
summary; missing or cross-context sets fail closed instead of silently using
the catalog pair, while non-Local-Set correction retains Good/Bad rerun.
Evidence:
`docs\reports\OPENVISIONLAB_CONTEXTUAL_CORRECTION_RERUN_20260729.md`.

P257/P258 then design and implement the user-requested contextual PropertyGrid
`Parameter Guide`. The shared mouse/keyboard selection contract and in-Tool
collapsible overlay are complete for detailed `Matching`,
`EdgeBasedMatching`, `LineGauge`, and `LineDistance` guidance, with visible
Basic fallback for every browsable pilot property. The overlay replaced a
rejected stacked layout that reduced the PropertyGrid below its established
minimum. Focused localization, navigation, side-effect, and shared-shell
regressions passed. Evidence:
`docs\reports\OPENVISIONLAB_CONTEXTUAL_PARAMETER_GUIDE_IMPLEMENTATION_20260730.md`.

P259 expands that verified detailed guidance to every browsable Threshold,
Blob, Contour, Morphology, and Filter property. Dedicated parameter-card
controls and PropertyGrid rows now feed the same guide; localization,
conditional applicability, exact units, and zero guide-caused execution/layer/
route side effects passed. Evidence:
`docs\reports\OPENVISIONLAB_PARAMETER_GUIDE_FAMILY_EXPANSION_20260730.md`.

P260 completes the current Basic-fallback audit and the selected EdgeDetection
slice. The audit covered 318 browsable properties, moved detailed coverage from
225 to 236, and reduced Basic fallback from 93 to 82. All 11 EdgeDetection
properties now have runtime-grounded Korean/English guidance and the dynamic
parameter cards feed the shared guide without guide-caused execution/layer/
route side effects. Evidence:
`docs\reports\OPENVISIONLAB_EDGE_DETECTION_PARAMETER_GUIDE_20260730.md`.

P261 then uses the actual current Debug EXE to reject the in-Tool overlay:
at `920 x 660` it covered Canny High, Canny Aperture, and L2 teaching
controls. The shared guide is now a nonmodal Tool-owned sidecar that does not
take keyboard focus, can be explicitly hidden/reopened with `?`, remembers a
session hide, and does not auto-open for docked Tools. Final actual-EXE
EdgeDetection and RotateScale checks found no obstruction and no guide-caused
Preview/Run or layer changes. All five RotateScale properties now have
runtime-grounded Korean/English detailed guidance. Evidence:
`docs\reports\OPENVISIONLAB_NON_OBSTRUCTING_PARAMETER_GUIDE_AND_ROTATE_SCALE_20260730.md`.

Next priority: reassess the remaining 77 Basic fallback properties and admit
only one runtime-grounded family with a concrete operator need. `Mean` is the
smallest candidate at 3 Basic entries, but is not pre-approved for
implementation. Recommended model: `gpt-5.6-sol` | Reasoning effort:
`medium`. CVR-00 remains deferred and requires three real independent
first-time participants; no model is recommended before those observations
exist.

P262 admits and completes that bounded Mean slice. The Direct Tool's friendly
controls map to stable `MEAN_TYPES`/`MEAN_MIN`/`MEAN_MAX` identities. Guidance
distinguishes average brightness from MeanStdDev standard deviation, identifies
Min/Max as inclusive Direct Preview review bounds rather than image-processing
values, and directs saved Pipeline users to its separate Step acceptance
metric/minimum/maximum. Actual current Debug EXE before/after evidence proves
the guide remains non-obstructing with zero execution/layer side effects. The
standalone canonical audit is now 244/318 detailed and 74 Basic. Evidence:
`docs\reports\OPENVISIONLAB_MEAN_PARAMETER_GUIDE_20260730.md`.

Next priority: audit the three remaining `FeatureMatching` Basic entries
(`PATTERN_PATH`, `SCORE_MIN`, `RANSAC_REPROJ_THRESHOLD`) before admitting
implementation. Recommended model: `gpt-5.6-sol` | Reasoning effort:
`medium`.

P263 admits and completes that FeatureMatching slice. Guidance distinguishes
the Lowe ratio `SCORE_MIN` on 0..1, where smaller is stricter, from runtime
`ScoreMax` as the RANSAC inlier percentage on 0..100. RANSAC tolerance is
correctly presented in pixels; template guidance covers readable dependency,
feature-rich crop, keypoints, GoodMatches, transformed quadrilateral, and
N-sample evidence. Actual current Debug EXE before/after evidence proves
detailed non-obstructing presentation with zero guide-caused execution/layer
side effects. The standalone audit is now 247/318 detailed and 71 Basic.
Evidence:
`docs\reports\OPENVISIONLAB_FEATURE_MATCHING_PARAMETER_GUIDE_20260730.md`.

P264 completes all eight remaining Matching entries while keeping their four
runtime responsibilities distinct. `MAGNIFIATION` is the working-resolution
divisor, not target scale variation. Coarse-to-fine angle guidance names its
angle/fine-step activation conditions. Pyramid proposal guidance names its
angle-off path, separate 0..1 proposal gate, and full-search fallback. Padding
false is Reflect rather than black. Actual current Debug EXE before/after
evidence proves detailed non-obstructing presentation with zero guide-caused
execution/layer side effects. Matching is 42/42 detailed; the standalone audit
is 255/318 detailed and 63 Basic. Evidence:
`docs\reports\OPENVISIONLAB_MATCHING_SEARCH_PARAMETER_GUIDE_20260730.md`.

Next priority: audit the 11 remaining `LineGauge/LineDistance` Basic entries
and separate algorithm controls from drawing-only toggles before admitting
implementation. Recommended model: `gpt-5.6-sol` | Reasoning effort:
`medium`.

P265 completes that Line audit and detailed guide slice. Manual angle is
documented as LineDistance sample-line direction rather than edge search.
Fitted-edge distance requires both A/B extend toggles; extend length is drawing
extent only. The three average-filter values persist but are not consumed by
the current runtime, and the four drawing flags apply only to the legacy
bitmap Draw path while current WPF/Pipeline evidence stays visible. Actual
current Debug EXE before/after remained non-obstructing with zero guide-caused
execution/layer side effects. LineGauge/LineDistance is 36/36 detailed; the
standalone audit is 266/318 detailed and 52 Basic. Evidence:
`docs\reports\OPENVISIONLAB_LINE_PARAMETER_GUIDE_20260730.md`.

Next priority: give the seven inactive/legacy Line controls an explicit
non-misleading UI treatment while preserving existing Recipe/Preset values and
mandatory current-run evidence. Do not invent an average-filter runtime
contract in this cleanup. Recommended model: `gpt-5.6-sol` | Reasoning effort:
`medium`.

P266 completes that compatibility cleanup. The three inactive average-filter
values and four legacy bitmap drawing values remain visible in Direct Line and
Recipe Manager for saved Recipe/XML review, but their editors and bridge
mutation paths are read-only. Korean/English labels identify
`Compatibility (inactive)` or `Legacy draw`; rows remain selectable for
detailed guidance. Basic/Fast/Precise presets preserve all seven values, and
asymmetric Line A/B values passed no-edit apply/save/reload exactly. Actual
current Debug EXE before/after remained non-obstructing with zero
Preview/Run/layer/route side effects. The standalone audit remains 266/318
detailed and 52 Basic because this is operator-trust cleanup, not guide
expansion. Evidence:
`docs\reports\OPENVISIONLAB_LINE_INACTIVE_LEGACY_CONTROLS_20260730.md`.

Next priority: audit the 20 remaining AffineTransform Basic entries against
current runtime and tests, grouped by coordinate definition, output policy,
interpolation/border behavior, and fail-closed geometry/coverage gates, before
admitting detailed guide implementation. Recommended model: `gpt-5.6-sol` |
Reasoning effort: `medium`.

P267 completes that AffineTransform audit and guide slice. All 20 coordinate,
output-canvas, sampling/border, and fail-closed gate entries now have detailed
Korean/English guidance; AffineTransform is 38/38 detailed. Ordered point
correspondence, pixel-only coordinates, Recipe detected-Point replacement,
zero-dimension input-size retention, no canvas-driven coordinate rescaling,
the exact supported interpolation/border policies, Constant-only BorderValue,
collinear rejection even at a zero area gate, and source-mask coverage that
excludes border fill are explicit. Existing known-matrix, alias, XML round
trip, collinear, and coverage-failure contracts passed. Actual current Debug
EXE before/after remained non-obstructing with zero Preview/Run/layer/route
side effects. The standalone audit is 286/318 detailed and 32 Basic, all in
EdgeBasedMatching. Evidence:
`docs\reports\OPENVISIONLAB_AFFINE_TRANSFORM_PARAMETER_GUIDE_20260730.md`.

Next priority: audit the 32 remaining EdgeBasedMatching Basic entries by
template identity/drawing compatibility, Auto MPoint teaching controls,
Canny/contour/model construction, and coarse/refine/pyramid/hybrid runtime
search groups before admitting guide implementation. Recommended model:
`gpt-5.6-sol` | Reasoning effort: `medium`.

P268 completes that EdgeBasedMatching audit and guide slice. All 32 formerly
Basic entries now have detailed Korean/English guidance; EdgeBasedMatching is
65/65 detailed and the standalone canonical audit is 318/318 detailed with
zero Basic entries. The guide separates registered template identity, global
polarity, limited result-bitmap display, explicit Auto MPoint teaching,
Canny/contour model construction, and coarse/refine/pyramid/hybrid runtime
search. Score, uniqueness margin, and Suggested rank are not physical-feature
identity evidence. Auto MPoint remains explicit Analyze candidates plus
operator Use this pattern, and `USE_DRAW_IMAGE` does not hide successful
current WPF/Pipeline evidence. The audit also closed a Recipe selected-Step
round-trip defect so existing scale, subpixel, and pyramid runtime values
survive create/apply/reload. Actual current Debug EXE before/after remained
non-obstructing with zero Preview/Run/layer/route side effects; focused/shared,
Direct Tool, Auto MPoint, localization, and 20/20 global-polarity runtime
regressions passed. Evidence:
`docs\reports\OPENVISIONLAB_EDGE_BASED_MATCHING_PARAMETER_GUIDE_20260730.md`
and `artifacts\p268_edge_based_matching_parameter_guide_20260730`.

Next priority: perform a static post-guide usability reassessment before
selecting another implementation. The guide backlog is closed; admit a new
feature only from a concrete current-source operator blocker or verified
regression. Recommended model: `gpt-5.6-terra` | Reasoning effort: `low`.

P269 completes that reassessment and one bounded persistence-trust correction.
Direct Tool property save exceptions were previously swallowed while an
undifferentiated saved event made the Tool appear normal. Failed saves now
retain the current memory value but publish Tool/Recipe-scoped Korean/English
status explaining reopen-loss risk and the cause. The next successful save
reports recovery once; ordinary successes add no repeated status noise. Full
text remains available through Tooltip and accessibility HelpText. Actual
current Debug EXE before/after remained non-obstructing with zero
Preview/Run/layer/route side effects; focused, P254 persistence, P257 guide,
isolated P268 guide, Blob Tool, localization, full build, and readiness
checks passed. Evidence:
`docs\reports\OPENVISIONLAB_PROPERTY_PERSISTENCE_FAILURE_FEEDBACK_20260730.md`
and `artifacts\p269_property_persistence_feedback_20260730`.

P270 completes that paired Direct PropertyGrid Tool load correction. Valid,
first-use missing, and invalid-file-replaced outcomes are retained. Missing and
valid files remain warning-free; invalid/deserialization-incompatible files
retain an exact backup and show default-substitution/review guidance, while
unreadable exceptions state that the saved file was not changed. Session and
Recipe-repository-preloaded paths both retain the result, and a successful
explicit save clears the warning through P269 recovery. Actual current Debug
EXE before/after remained non-obstructing with zero Preview/Run/layer/route
side effects. Evidence:
`docs\reports\OPENVISIONLAB_PROPERTY_LOAD_RECOVERY_FEEDBACK_20260730.md` and
`artifacts\p270_property_load_feedback_20260730`.

Next priority: audit the separate `OpenVisionNativeToolSettingsStore` used by
Threshold, Filter, Morphology, Arithmetic, and SimplePreprocess. Implement
save/load feedback parity only if its current undifferentiated result is
reproduced. Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.
Syntactically valid semantic staleness remains blocked on a future explicit
schema/version contract. CVR-00 still requires three real independent
first-time participants.

P271 completes that separate settings-store correction. Threshold, Filter,
Morphology, Arithmetic, EdgeDetection, RotateScale, Mean, HSV, and Histogram
now distinguish normal first use and valid restore from invalid-file
replacement, unreadable load, disk-save failure, and explicit-save recovery.
Invalid originals retain exact backups. Failed saves explicitly identify
memory-only values and reopen-loss risk; the next successful save clears the
failure and reports recovery once. Full Korean/English status remains
nonmodal, ellipsized with Tooltip/accessibility HelpText, and Tool
initialization cannot auto-save away the warning. Actual current Debug EXE
before/after and affected-family regressions passed with zero
Preview/Run/layer/route side effects. Evidence:
`docs\reports\OPENVISIONLAB_SETTINGS_STORE_PERSISTENCE_FEEDBACK_20260730.md`
and `artifacts\p271_settings_persistence_feedback_20260730`.

Next priority: statically audit higher-impact Recipe/Pipeline persistence,
beginning with `VisionPipelineStorage` and `RecipeDataStorage`. Implement
feedback parity only after reproducing an operator-visible silent fallback.
Recommended model: `gpt-5.6-terra` | Reasoning effort: `medium`.
Syntactically valid semantic staleness remains blocked on a future explicit
schema/version contract. CVR-00 still requires three real independent
first-time participants.

## 2026-07-29 P253 / Workspace Sample / Recipe Context Sync

- Status: Complete.
- An agent-operated actual-EXE walkthrough exposed and closed a stale
  Recipe-Manager context blocker after workspace sample opening.
- Successful sample opening now synchronizes the exact generated Pipeline and
  sample after the existing pending-edit preflight. Cancel is fail-closed, and
  synchronization does not Preview/Run or mutate result layers/routes.
- A 112-second current-build recording completes the Matching pair-save,
  saved-set run, NG correction preparation, no-op XML Apply, explicit same-set
  rerun, and comparison workflow.
- Evidence:
  `artifacts\novice_matching_correction_loop_20260729`,
  `artifacts\p253_workspace_sample_recipe_context_sync_20260729`, and
  `docs\reports\OPENVISIONLAB_WORKSPACE_SAMPLE_RECIPE_CONTEXT_SYNC_20260729.md`.
- This does not complete CVR-00 and does not qualify Matching.

The next direct-Teaching persistence workflow is completed by P254 below; this
historical P253 priority is no longer active.

## 2026-07-29 P254 / Direct Teaching Pipeline Persistence

- Status: Complete.
- The existing direct Tool View action already saved immediately, but its
  former label/status did not tell a first-time operator that saving was
  complete, where it occurred, or what to do next.
- The action now says `Add and save to Pipeline` and reports the saved Step,
  exact `Recipe > Pipeline` destination, and `Next: Open Pipeline`.
- Focused current-source runtime evidence reloaded the saved 3-Step Pipeline,
  refreshed Recipe Manager, reopened Pipeline Review, and explicitly ran the
  added Blob Step with retained result/drawing evidence. Add/save itself
  changed no Preview/Run, layer, active-layer, or route state.
- The current Debug EXE recording proves the new visible wording and exact
  destination through add/save. It is not an end-to-end recording because
  desktop capture automation failed to dismiss the floating Tool View
  afterward.
- Evidence:
  `artifacts\p254_direct_teaching_pipeline_persistence_20260729` and
  `docs\reports\OPENVISIONLAB_DIRECT_TEACHING_PIPELINE_PERSISTENCE_20260729.md`.
- Boundary: agent/developer evidence only; CVR-00 remains incomplete and Blob
  remains unqualified.

The historical P254 next-priority state is superseded by the user-approved
P255 agent-role workflow below.

## 2026-07-29 P255 / Scratch Threshold -> Blob Recipe Walkthrough

- Status: Complete.
- One clean-runtime recording completed blank workspace -> unique Recipe ->
  image -> explicit Threshold Preview/save -> direct Blob transition ->
  `Threshold_Preview` input -> explicit Blob Preview/save -> two-Step route
  review -> application restart -> Recipe/image restore -> explicit Run
  Review.
- The persisted route is
  `Main -> Threshold_Preview -> Blob_Preview`.
- Restore retained both Steps as `WAIT` and did not run automatically. The
  one explicit Run completed both Steps as `OK / 21.5 ms` with 13 Blob
  candidates, drawings, and metrics.
- The focused smoke was corrected to prove zero run on Threshold slider
  change and exactly one run after explicit Preview.
- Evidence:
  `artifacts\p255_scratch_threshold_blob_recipe_20260729`,
  `artifacts\openvisionlab_clean_runtime_p255_r7_20260729`, and
  `docs\reports\OPENVISIONLAB_SCRATCH_THRESHOLD_BLOB_RECIPE_WALKTHROUGH_20260729.md`.
- Boundary: agent/developer workflow evidence only; this is not CVR-00,
  production algorithm qualification, or arbitrary long-Pipeline proof.

Next bounded priority: record one longer operator-authored Pipeline and admit
an input/output clarity change only if it reproduces a concrete current-build
route-selection blocker. Recommended model: gpt-5.6-sol | Reasoning effort:
medium.

CVR-00 remains incomplete and still requires three independent first-time
participants with unedited observations. Physical-task CVR-09/CVR-11
qualification, another CVR-10 family, and CVR-12 through CVR-18 implementation
still require their named operator/data packets. CVR-19 and CVR-20 remain
complete.

The complete ordered status/trigger/model table is in
`docs\reports\OPENVISIONLAB_COMMERCIAL_VIDEO_QUEUE_HANDOFF_20260728.md`.

Do not start another dataset run, recipe tuning cycle, algorithm family,
parallelization project, or LLM campaign merely to keep work moving.

Only after the operator reports a measured sequential bottleneck and explicitly
requests parallel execution should isolated-worker `1/2/4` equivalence and
thread safety be audited.

## 2026-07-29 P256 / Four-Step Route Clarity Walkthrough

- Status: Complete.
- The isolated 335.33-second clean-runtime recording authored, saved,
  reviewed, restarted, restored in `WAIT`, and explicitly ran
  `Filter -> Threshold -> Morphology -> Blob`.
- Exact route:
  `Main -> Filter_Preview -> Threshold_Preview -> Morphology_Preview ->
  Blob_Preview`.
- Blob Basic preserved `Morphology_Preview` and did not auto-run. The one
  explicit post-restart Run completed `OK 4 / NG 0 / WAIT 0`,
  `OK / 21.5 ms`, with 12 Blob rows and drawings.
- Focused current-source evidence proves Basic/Fast/Precise presets preserve
  an explicitly selected input route and cause zero Preview.
- Evidence:
  `artifacts\p256_four_step_route_clarity_20260729`,
  `artifacts\openvisionlab_clean_runtime_p256_before_20260729`, and
  `docs\reports\OPENVISIONLAB_FOUR_STEP_ROUTE_CLARITY_WALKTHROUGH_20260729.md`.
- This remains agent/developer evidence, not CVR-00 or production
  qualification. No feature is admitted from the completed chain.

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

P258-P263 completed the shared contextual Parameter Guide, its
Matching/EdgeBasedMatching/LineGauge/LineDistance pilot, exhaustive
Threshold/Blob/Contour/Morphology/Filter/EdgeDetection/RotateScale/Mean
and FeatureMatching guidance, and the non-obstructing presentation correction.
In a floating Tool, the guide must remain an adjacent nonmodal sidecar; it must
not cover teaching controls or take input focus. Docked Tools do not auto-open
it. Audit the remaining 71 Basic fallback entries before selecting another bounded
expansion; preserve
explicit Preview/Run and zero guide-caused value/layer/route side effects.
CVR-00 still requires three independent novice participants and raw
observations; agent recordings do not count. Named CVR-09/CVR-11 physical
packets or CVR-12/CVR-13/CVR-14/CVR-15/CVR-16/CVR-17/CVR-18 admission packets
take priority when supplied. CVR-19 and CVR-20 are complete. Do not invent
CVR-21.

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
