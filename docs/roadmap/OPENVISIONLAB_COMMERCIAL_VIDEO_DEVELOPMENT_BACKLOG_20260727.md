# OpenVisionLab Commercial-Video Development Backlog

Updated: 2026-07-27 KST

## Decision And Authority

This document is the canonical, durable, priority-ordered inventory of
development candidates derived from the 16 Cognex, HALCON, and MERLIC videos
reviewed on 2026-07-27.

The user explicitly requested that the full list survive future Codex chat
changes. A new chat must read this document after the current handoff before
selecting commercial-comparison work.

This document preserves the full queue. It does not override:

- `AGENTS.md`;
- current product identity and stable behavior contracts;
- `docs/OPENVISIONLAB_CURRENT_HANDOFF.md` for the one task that is active now;
- the feature-admission gate in
  `docs/OPENVISIONLAB_RULE_BASED_UI_GAP_AUDIT_20260723.md`.

The current decision is:

- only `CVR-00` is an active prerequisite;
- `CVR-01` is complete after the user's explicit 2026-07-27 activation;
- `CVR-02` is complete after the user's explicit 2026-07-27 continuation;
- no new product feature is automatically active;
- the earliest incomplete row whose trigger has actual current-source evidence
  becomes the next implementation candidate;
- a later row may move ahead only through an explicit user decision or evidence
  showing that its blocker is more important;
- completed rows stay in this document with their evidence. Do not delete them
  and silently recreate the same work in a later chat.

## Product Boundary

OpenVisionLab is an OpenCvSharp4 deterministic rule-based vision recipe
workbench. Its core workflow is:

```text
sample/operator image
  -> Learn or direct PropertyGrid teaching
  -> explicit Preview/Run
  -> layer, drawing, metric, and object review
  -> N-image or labelled Validation Set
  -> Run History and deterministic review queue
  -> Qualified Recipe Snapshot
```

The commercial lessons to emulate are:

- image-first tool/Step/result context;
- visible fixture relationships;
- signal and distribution evidence for parameter teaching;
- Good/Bad sequence review;
- direct failure reason and next-action guidance;
- explicit operator acceptance of suggestions.

Camera, lighting, PLC, I/O, MES, accounts, deployment, regulatory compliance,
general scripting IDEs, autonomous AI classification, and consumer-web LLM
automation remain outside the current platform scope.

## Status Vocabulary

| Status | Meaning |
| --- | --- |
| `Active prerequisite` | Current work or external evidence required before feature selection. |
| `First implementation candidate` | The first feature to implement after its trigger is reproduced and the user approves development. |
| `Conditional` | Preserved candidate; do not start until its exact trigger exists. |
| `Deferred` | Known idea with no current task or evidence. |
| `Out of scope` | Deliberately excluded from the current product identity. |
| `Complete` | Acceptance and verification passed; retain evidence and do not repeat. |

## Common Admission And Completion Gate

Every implementation row below must satisfy all of the following:

1. Name one operator task and reproduce the current-source blocker.
2. Prove that the existing Tool View, Pipeline, Review, or Validation workflow
   cannot complete the task clearly and safely.
3. Define the smallest input, output, metric, drawing, failure, persistence, and
   compatibility contract.
4. Keep algorithm tools PropertyGrid-based.
5. Preserve explicit Preview/Run, no automatic route/layer mutation, and no
   stale evidence reuse.
6. Capture fresh before/after current-build UI evidence for visible changes.
7. Run the smallest focused smoke plus the relevant stable-contract regression.
8. Record source/data identity, commands, results, artifact path, and the
   boundary that prevents overclaiming.
9. Update this backlog, the current handoff, and the documentation map.

## Ordered Development Queue

Model recommendations reflect the models available on 2026-07-27. A later chat
must use the closest currently available equivalent if a label changes.

| Order | ID | Development item | Status | Activation trigger | Recommended model | Reasoning effort |
| ---: | --- | --- | --- | --- | --- | --- |
| 0 | `CVR-00` | Independent first-time operator study | Active prerequisite | At least three independent novice participants use the existing protocol. | None before observations; `gpt-5.6-terra` for evidence synthesis afterward | None before observations; low afterward |
| 1 | `CVR-01` | Shared Tool Signal Inspector foundation | Complete | Explicit user selection on 2026-07-27 activated the bounded shared foundation; the Histogram Tool is the representative integration. | `gpt-5.6-sol` | high |
| 2 | `CVR-02` | Threshold gray-histogram teaching view | Complete | Explicit user continuation on 2026-07-27 activated the bounded Basic/Range full-image integration; the public BandPads pair is the frozen replay. | `gpt-5.6-sol` | medium |
| 3 | `CVR-03` | Line intensity and edge-response profile | Conditional on `CVR-01` | A named Line/LineDistance task cannot justify polarity, contrast, or selected edge from current overlays. | `gpt-5.6-sol` | high |
| 4 | `CVR-04` | Circle radial-sample, inlier/outlier, and residual review | Conditional on `CVR-01` | A CircleGauge task cannot explain fit support or a wrong circle from current drawings and aggregate residual. | `gpt-5.6-sol` | high |
| 5 | `CVR-05` | Blob/Contour object-metric distribution chart | Conditional on `CVR-01` | A labelled object population needs distribution evidence for existing area/width/height gates. | `gpt-5.6-sol` | medium |
| 6 | `CVR-06` | Matcher model/pyramid/candidate diagnostic surface | Conditional on `CVR-01` | A frozen matcher run cannot be diagnosed as wrong feature, lost pyramid candidate, no match, or ambiguity from existing result review. | `gpt-5.6-sol` | high |
| 7 | `CVR-07` | Bounded task-specific teaching suggestions | Conditional | Repeated operator evidence shows that one specific Threshold, Line, or Circle teaching action is slow or error-prone after `CVR-01` through `CVR-06`. | `gpt-5.6-sol` | high |
| 8 | `CVR-08` | Generic typed fixture-transform consumer for multiple downstream ROIs | Conditional | One qualified locator must drive at least two downstream ROI/measurement Steps and current P212 NormalizeImage/P219 Affine cannot express the task safely. | `gpt-5.6-sol` | high |
| 9 | `CVR-09` | Straight-edge/dual-edge fixture producer | Conditional after `CVR-08` contract review | A named part has no durable template feature but has two independently verified physical datum edges that must define a fixture. | `gpt-5.6-sol` | high |
| 10 | `CVR-10` | Multi-instance fixture and bounded sub-recipe fan-out | Conditional after `CVR-08` | A real task requires the same inspection on multiple accepted matches and defines stable instance identity, output naming, review, and acceptance semantics. | `gpt-5.6-sol` | high |
| 11 | `CVR-11` | Edge matcher polarity modes | Conditional | A labelled N-sample set proves that the same physical feature reverses polarity and current threshold/Canny/search settings cannot preserve the match. | `gpt-5.6-sol` | high |
| 12 | `CVR-12` | Bounded matcher deformation/elasticity | Conditional after `CVR-11` review | A labelled task proves bounded physical deformation, not pose/scale/blur/ROI error, and requires a documented deformation limit. | `gpt-5.6-sol` | high |
| 13 | `CVR-13` | Anisotropic X/Y matcher scale search | Conditional | A named task has verified non-uniform target scale change that uniform scale and Affine normalization cannot handle. | `gpt-5.6-sol` | high |
| 14 | `CVR-14` | Multi-result overlap and suppression semantics | Conditional | A multiple-instance matching task needs an operator-defined overlap rule and existing result spacing is insufficient. | `gpt-5.6-sol` | high |
| 15 | `CVR-15` | Synthetic edge-model/geometry teaching | Deferred | A real target has no usable Good template image but has an operator-certified geometric model and a replay corpus. | `gpt-5.6-sol` | high |
| 16 | `CVR-16` | Additional per-object shape descriptors | Conditional | A named Blob/Contour task proves existing area/width/height filters cannot separate OK/NG objects. Add only the one required descriptor first. | `gpt-5.6-sol` | high |
| 17 | `CVR-17` | Region algebra operators | Conditional | A named inspection requires reviewed union/intersection/difference/complement semantics that current masks/layers cannot express. | `gpt-5.6-sol` | high |
| 18 | `CVR-18` | Bounded derived-metric expression Step | Conditional | A real recipe needs a derived scalar judgment that cannot be expressed by one existing metric gate. | `gpt-5.6-sol` | high |
| 19 | `CVR-19` | Validation variants for multiple approved part styles | Conditional | One recipe/job must validate multiple explicitly named product styles with different expected values while retaining one auditable identity. | `gpt-5.6-sol` | high |
| 20 | `CVR-20` | Display-only overlay style and image-coordinate label controls | Deferred | Current-run evidence becomes unreadable in a named task and existing tool-owned colors/line widths cannot resolve it. | `gpt-5.6-terra` | medium |

## Detailed Contracts

### CVR-00 — Independent first-time operator study

Scope:

- use the protocol in
  `docs/reports/OPENVISIONLAB_FIRST_TIME_OPERATOR_JOURNEY_AUDIT_20260727.md`;
- run the core Sample -> Recipe -> Run Review task for every participant;
- run the Validation Set -> Qualified Snapshot advanced task after the core task;
- preserve task completion, help request, hesitation, incorrect mental model,
  evidence interpretation, and unintended-action observations.

Acceptance:

- at least three independent novice observations exist;
- raw observations are retained without rewriting them as successful outcomes;
- immediate fixes are allowed for crash, data loss, unintended execution, wrong
  route/layer mutation, or false evidence;
- a bounded UX correction is selected only if at least two of the first three
  participants fail the same transition or form the same incorrect mental model.

Boundary:

- this is usability evidence, not production or inspection qualification;
- no model tokens should be spent trying to simulate human observations.

### CVR-01 — Shared Tool Signal Inspector foundation

Scope:

- a read-only inspector bound to one retained explicit Preview/Run result;
- stable source image hash, Step/property identity, ROI/scan coordinates, and
  coordinate-space metadata;
- shared plot selection, pan, zoom, cursor value, threshold/edge markers, and
  evidence export;
- an extension contract for Threshold, Line, Circle, Blob/Contour, and matching
  diagnostics without one-off duplicated chart frameworks.

Acceptance:

- opening, selecting, panning, zooming, and exporting do not run Preview/Run;
- stale source/Step identity is rejected rather than mixed with a new image;
- no layer creation, active-layer change, input/output route mutation, or
  parameter auto-apply;
- a changed parameter follows the existing tool-specific manual/auto-preview
  contract;
- current-run chart data and the visible result drawing resolve to the same
  source and coordinates.

Exclusions:

- automatic threshold selection;
- automatic acceptance-gate changes;
- arbitrary general-purpose plotting or scripting.

Completion:

- the shared evidence model retains tool, input layer, full-image region,
  parameter summary, source/result SHA-256, stable evidence ID, axes, and named
  series;
- the reusable read-only WPF plot supports X-axis zoom, pan, cursor values,
  reset, legend, and explicit provenance-preserving TSV export;
- the existing Histogram Tool is the representative integration and publishes
  two 256-bin grayscale population series (`Source` and `Result`) only after a
  successful Preview;
- parameter/input changes clear the old signal evidence synchronously and the
  existing debounced Preview policy creates the replacement evidence;
- reset, zoom/pan, and export do not run Preview/Run, create or select a layer,
  or mutate active layer or input/output routes;
- `CVR-02` through `CVR-06` remain separate tool-specific integrations. CVR-02
  was completed later; CVR-03 through CVR-06 remain conditional. This
  foundation completion does not itself claim Threshold markers, Line edge
  profiles, Circle residuals, object distributions, or matcher diagnostics.

Evidence:

- `docs/reports/OPENVISIONLAB_TOOL_SIGNAL_INSPECTOR_FOUNDATION_20260727.md`;
- `artifacts/cvr01_tool_signal_inspector_20260727`;
- focused smokes `wpf_simple_preprocess_result_review` and
  `wpf_preprocess_output_preview_flow`;
- current Debug/screenshot-runner builds with zero warnings and zero errors.

### CVR-02 — Threshold gray-histogram teaching

Minimum slice:

- grayscale population inside the reviewed ROI;
- movable lower/upper markers that edit only the existing teaching model;
- current segmentation preview remains governed by the existing Threshold
  Preview contract;
- original and thresholded pixel counts are explainable;
- optional dual population overlay only when Good/Bad evidence identities are
  explicit and compatible.

Completion requires one task where the chart changes an evidence-backed
threshold choice and a Good/Bad replay confirms the same frozen value.

Completion:

- the existing Threshold Tool successful Preview publishes one 256-bin
  grayscale population tied to tool/mode, input layer, full-image region,
  parameters, source/result SHA-256, and a deterministic evidence ID;
- Basic exposes one editable `T` marker, while Range exposes editable `Lower`
  and `Upper` markers and preserves `Lower <= Upper`;
- marker motion is transient until release. Release edits only the existing
  Threshold teaching model, clears stale evidence synchronously, and schedules
  the existing debounced Preview to create replacement evidence;
- the chart is a full-panel review overlay so the docked parameter editor is
  not clipped. Back/open, zoom/pan/cursor/reset, and TSV export do not run
  Preview/Run or mutate layers, active layer, or input/output routes;
- Adaptive mode deliberately retains its existing controls without a global
  cutoff chart because it has no single global threshold marker;
- the frozen public `Public_Threshold_BandPads.pipeline.xml` decision
  was taught from `T=127` to its unchanged frozen `T=130` marker on the Good
  distribution, then replayed as Binary, Max 255: Good produced
  `ResultCount=4`, while the expected-NG missing-pad reference produced
  `ResultCount=1`;
- fresh current-source before/after captures, Basic/Range/Adaptive assertions,
  provenance TSVs, and the exact Good/Bad replay are retained.

Evidence:

- `docs/reports/OPENVISIONLAB_THRESHOLD_HISTOGRAM_TEACHING_20260727.md`;
- `artifacts/cvr02_threshold_histogram_teaching_20260727`;
- focused smokes `wpf_shell_host_threshold_basic_tool`,
  `wpf_shell_host_threshold_tool`, and
  `wpf_threshold_signal_good_bad_replay`.

Boundary:

- Threshold Tool currently has no ROI teaching contract, so `Full image` is the
  only claimed reviewed region;
- the implementation does not select an automatic threshold, overlay Good and
  Bad populations in one interactive chart, change acceptance gates, or prove
  unseen-data/field robustness.

### CVR-03 — Line intensity and edge-response profile

Minimum slice:

- selected scan line or representative scan set;
- intensity profile and signed edge response;
- polarity, minimum contrast, selected peak, rejected alternatives, and image
  coordinates;
- exact correspondence with current scan/edge/fitted-line drawing.

Do not change the existing `LineGauge`/`LineDistance` runtime merely to make the
chart look clean. A runtime change requires its own defect evidence.

### CVR-04 — Circle sampling and residual review

Minimum slice:

- reviewed radial scan locations;
- per-scan intensity/edge response;
- accepted/rejected edge sample state;
- fitted radius residual distribution and current support gate;
- two-way selection between a sample row/plot and the image drawing.

It remains pixel geometry unless an existing positive, verified uniform scale is
applied. It does not add camera calibration.

### CVR-05 — Object-metric distribution

Minimum slice:

- current retained Blob/Contour candidate rows;
- distribution of existing `Area`, `BoundsWidth`, or `BoundsHeight`;
- accepted/rejected colors and exact reject reason;
- lower/upper markers bound to one existing PropertyGrid range;
- no new descriptor in this slice.

Add circularity, aspect ratio, angle, holes, or gray features only through
`CVR-16` after a separate named task proves the need.

### CVR-06 — Matcher diagnostic surface

Minimum slice:

- trained edge/model visualization;
- active pyramid/coarse-fine level;
- chosen candidate and strongest spatially distinct alternative;
- score, margin, angle, scale, search ROI, and relevant internal diagnostics;
- exact `Success`, `NoMatch`, or `Ambiguous` reason from the retained run.

The surface is diagnostic only. It must not lower gates, change defaults,
auto-select a pattern, or turn diagnostic risk metrics into acceptance rules.

### CVR-07 — Bounded teaching suggestions

Implement one task-specific suggestion contract at a time:

- explain why a candidate is suggested or rejected;
- preview its exact ROI/geometry;
- require explicit `Use` or `Apply`;
- keep the previous teaching state recoverable;
- do not run the inspection merely because a suggestion is selected.

Do not create a generic MERLIC easyTouch clone. Existing Auto MPoint remains the
reference for suggestion/explicit-accept separation.

### CVR-08 — Generic typed fixture consumer

Minimum slice:

- one earlier accepted fixture identity;
- finite transform and same source frame;
- one or more immutable reference-coordinate downstream ROIs;
- transformed source polygons plus unchanged reference rectangles;
- consumer-by-consumer provenance and fail-closed diagnostics;
- explicit Run applies the transform.

Exclude multi-instance fan-out, homography, automatic locator selection, and
per-image recipe mutation.

### CVR-09 — Straight-edge/dual-edge fixture producer

Minimum slice:

- operator-reviewed physical horizontal/vertical or two non-parallel datum
  edges;
- support, polarity, contrast, angle, intersection/origin, and ambiguity gates;
- exact edge-support and fixture-axis drawings;
- one typed fixture output compatible with `CVR-08`;
- N-sample proof that reflections or repeated rails do not replace the intended
  physical datum.

### CVR-10 — Multi-instance fixture and sub-recipe fan-out

Before implementation, define:

- deterministic instance ordering and stable identity;
- maximum instance count and overlap rules;
- output layer/report/drawing naming;
- partial failure and aggregate acceptance semantics;
- how each instance is selected in Pipeline Review and Run History;
- evidence-size and performance limits.

Do not implement a generic graph engine as a shortcut.

### CVR-11 Through CVR-15 — Matcher expansion

Common requirements:

- opt-in parameters;
- missing keys preserve legacy behavior;
- PropertyGrid/XML/Pipeline/report round trip;
- fixed template, search ROI, and labelled N-sample matrix;
- drawing of the physical feature and alternatives;
- held-out replay;
- no parameter default change from one demonstration;
- no claim of PatMax/PatFlex or commercial parity.

Specific boundaries:

- `CVR-11`: polarity only;
- `CVR-12`: a numeric bounded deformation contract only;
- `CVR-13`: separately searched X/Y scale with finite bounds;
- `CVR-14`: explicit overlap/suppression rule for multi-result inspection;
- `CVR-15`: operator-certified synthetic geometry, not an automatically invented
  template.

### CVR-16 — Additional object descriptors

Candidate descriptors include:

- aspect ratio;
- circularity;
- orientation;
- rotated width/height;
- hole count;
- gray-value statistics.

Add only one descriptor needed by the first named task. It must have:

- exact PropertyGrid and XML names;
- finite defaults preserving legacy behavior when missing;
- accepted/rejected object rows with exact reason;
- aggregate metrics only when their semantics are separately defined;
- current-run drawing and labelled Good/NG evidence.

### CVR-17 — Region algebra

Possible bounded operations are union, intersection, difference, and complement.
Before implementation, define:

- whether input is a mask layer, object set, or ROI;
- image-size and coordinate-frame compatibility;
- empty-set behavior;
- output image/mask and metrics;
- drawing and downstream routing semantics.

Do not introduce a general iconic-variable language.

### CVR-18 — Derived-metric expression

If activated, prefer a small safe expression grammar over arbitrary code.
Define:

- exact allowed operators and functions;
- named earlier metric references;
- numeric type, missing/non-finite behavior, and unit compatibility;
- validation and fail-closed diagnostics;
- saved expression and input provenance;
- deterministic evaluation in reports and Validation Sets.

No file, process, reflection, network, dynamic language runtime, or arbitrary
method invocation is allowed.

### CVR-19 — Validation variants

Minimum slice:

- one immutable recipe/Pipeline identity;
- explicit named variant identity;
- variant-specific expected metrics or sample roles;
- no hidden parameter mutation between rows;
- Run History, review queue, and Qualified Snapshot retain the variant;
- comparisons never merge incompatible variants.

Do not add user accounts, electronic signatures, or regulatory-compliance
claims.

### CVR-20 — Display-only overlay controls

Minimum slice:

- only when current evidence is unreadable;
- project-owned bounded color, fill/margin, line width, and image-coordinate
  label options;
- stable defaults and backward compatibility;
- display changes do not alter metrics or acceptance;
- evidence export records the rendering option.

Do not add arbitrary visualization scripting.

## Explicit Exclusion Register

These items are not in the development queue. A future chat must not revive them
from the videos without an explicit product-direction decision.

| Excluded item | Decision |
| --- | --- |
| General ML classifier and label-training environment | Outside the deterministic rule-based product direction. |
| HALCON-style general scripting IDE, breakpoint debugger, iconic/control variable language | Would change the product from a recipe workbench into a programming platform. |
| Generic easyTouch across all tools | Defer; use bounded task-specific suggestions only after evidence. |
| OCR/barcode tool family | No named operator task. |
| Camera acquisition and lighting control | Out of scope. |
| PLC, I/O, MES, industrial controller integration | Out of scope. |
| Account, role, SSO, electronic-signature, and regulatory audit platform | Out of scope. |
| Installer, fleet, cloud, and deployment-management platform | Out of scope unless the user changes product direction. |
| Lens/camera calibration and certified metrology | Current two-point scale is not this; requires a separate explicit product decision and physical evidence. |
| Homography, global anchors, ODB/CAD integration | No approved task; do not infer from Affine or matching videos. |
| Autonomous threshold/gate tuning | Conflicts with operator-owned tolerance and evidence review. |
| New LLM provider, consumer-web automation, prompt family, or intent-skill campaign | Frozen by P196 maintenance-mode decision. |

## Source-Video Traceability

| Video | Backlog IDs informed |
| --- | --- |
| 01 VisionPro QuickBuild | `CVR-08`, `CVR-10`; ML classifier excluded |
| 02 VisionPro PMAlign | `CVR-06`, `CVR-11` through `CVR-15` |
| 03 HDevelop GUI/Navigation | `CVR-01`, `CVR-06`; general IDE excluded |
| 04 HDevelop Variables | `CVR-16` through `CVR-18`; general variable language excluded |
| 05 HDevelop Visualization | `CVR-20` |
| 06 MERLIC easyTouch | `CVR-07`; generic easyTouch/OCR excluded |
| 07 MERLIC Alignment | `CVR-08`, `CVR-09` |
| 08 MERLIC Calibrated Measurement | Calibration boundary only |
| 09 In-Sight Charts | `CVR-01` through `CVR-03` |
| 10 In-Sight Validation | `CVR-00`, `CVR-19`; account/compliance platform excluded |
| 11 MERLIC Five-Minute Application | `CVR-07`, `CVR-18` |
| 12 Shape Matching Introduction | `CVR-06` |
| 13 Shape Matching Advanced | `CVR-06`, `CVR-11` through `CVR-14` |
| 14 HALCON 2D Metrology | `CVR-04`; calibrated world metrology excluded |
| 15 HALCON Regions | `CVR-02`, `CVR-05`, `CVR-16`, `CVR-17` |
| 16 Shape Matching Align ROI/Images | `CVR-08`, `CVR-10`, `CVR-14` |

## New-Chat Selection Protocol

Every new chat that selects work from this queue must:

1. read `AGENTS.md`, the current handoff, and this backlog;
2. rerun current Git status/log and inspect the latest source/evidence;
3. check whether `CVR-00` or another row's exact trigger now exists;
4. select the earliest triggered incomplete row;
5. state product identity, current maturity, immediate priority, remaining
   priority, commercial lessons, and out-of-scope platform areas;
6. define acceptance and verification before implementation;
7. after completion, mark the row `Complete`, add evidence, and update the
   handoff/documentation map.

If no row is triggered, report that no feature is active. Do not spend tokens
implementing the queue speculatively.

## Completion Record

```text
Status: Complete
Scope: Durable full commercial-video development candidate inventory, ordered priorities, activation gates, acceptance boundaries, model recommendations, exclusion register, and new-chat continuation protocol.
Acceptance criteria: All 16 reviewed videos map to retained candidates or explicit exclusions; every ordered row has a trigger and model/reasoning recommendation; future chats are instructed not to lose or auto-activate the queue.
Verification: Cross-checked against the 2026-07-27 commercial-video report, current handoff, first-time operator audit, P217 feature-admission decision, AGENTS.md, and the canonical documentation map.
Evidence: docs/roadmap/OPENVISIONLAB_COMMERCIAL_VIDEO_DEVELOPMENT_BACKLOG_20260727.md and docs/reports/OPENVISIONLAB_COMMERCIAL_RULEBASE_VIDEO_REVIEW_20260727.md
Boundary / next dependency: CVR-01 and CVR-02 are complete. CVR-00 remains the only active prerequisite; CVR-03 and later rows still require their exact current-source trigger or an explicit user decision and must not be auto-activated.
```
