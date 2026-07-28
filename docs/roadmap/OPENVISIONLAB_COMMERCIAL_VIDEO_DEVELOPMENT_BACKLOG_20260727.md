# OpenVisionLab Commercial-Video Development Backlog

Updated: 2026-07-28 KST

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
- `CVR-03` is complete after the user's explicit 2026-07-28 continuation;
- `CVR-04` is complete after the user's explicit 2026-07-28 continuation;
- `CVR-05` is complete after the user's explicit 2026-07-28 continuation;
- `CVR-06` is complete after the user's explicit 2026-07-28 continuation;
- `CVR-07` Threshold Basic v1 is complete after the user's explicit 2026-07-28
  continuation;
- `CVR-08` is complete after the user explicitly delegated the bounded task
  choice on 2026-07-28; the selected public synthetic task verifies one circular
  datum ROI and one pad-presence ROI behind the same existing fixture;
- `CVR-09` LineFixture v1 implementation and synthetic integration are complete
  after the user's explicit 2026-07-28 development continuation; named
  physical-part qualification remains blocked on the original operator/data
  trigger;
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
| `Implemented — qualification pending` | The reusable bounded product contract is implemented and verified, but the row's named physical-task/data gate remains unsatisfied. |
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
| 3 | `CVR-03` | Line intensity and edge-response profile | Complete | Explicit user continuation on 2026-07-28 activated the bounded current-Preview Line/LineDistance diagnostic; the public Line Pins Good/Bad pair is the frozen replay. | `gpt-5.6-sol` | high |
| 4 | `CVR-04` | Circle radial-sample, inlier/outlier, and residual review | Complete | Explicit user continuation on 2026-07-28 activated the bounded current-Run CircleGauge diagnostic; one frozen Good circle and Bad ellipse use identical settings. | `gpt-5.6-sol` | high |
| 5 | `CVR-05` | Blob/Contour object-metric distribution chart | Complete | Explicit user continuation on 2026-07-28 activated the bounded current-Run Object Results distribution; public Blob Good/Bad and Contour Bad exercise the product path. | `gpt-5.6-sol` | medium |
| 6 | `CVR-06` | Matcher model/pyramid/candidate diagnostic surface | Complete | Explicit user continuation on 2026-07-28 activated the bounded retained-run EdgeBasedMatching diagnostic; public Good/Wrong and a repeated-pattern matrix freeze Success/NoMatch/Ambiguous evidence. | `gpt-5.6-sol` | high |
| 7 | `CVR-07` | Bounded task-specific teaching suggestions | Complete — Threshold Basic v1 | Explicit user continuation on 2026-07-28 activated one bounded full-image Threshold Basic bright/dark significant-mode suggestion with explicit Use/Undo and public Good/Bad replay. | `gpt-5.6-sol` | high |
| 8 | `CVR-08` | Generic typed fixture-transform consumer for multiple downstream ROIs | Complete — bounded P212 multi-ROI review extension | User delegation selected the public synthetic fixture task: circular datum verification plus pad-presence inspection, with one locator/NormalizeImage chain and controlled Good/Bad replay. | `gpt-5.6-sol` | high |
| 9 | `CVR-09` | Straight-edge/dual-edge fixture producer | Implemented — qualification pending | User explicitly activated a bounded synthetic v1 on 2026-07-28. Physical-task qualification still requires a named part with no durable template feature and two independently verified datum edges. | `gpt-5.6-sol` | high |
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
- `CVR-02` through `CVR-06` remain separate tool-specific integrations. All
  five were completed later. This
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

Completion:

- a successful explicit Line Edge or Measure Preview selects the median
  successful scan row/column of the currently selected Line A/B result and
  publishes its prepared grayscale intensity plus signed scan-direction
  response through the shared Signal Inspector;
- the diagnostic independently replays the existing first contrast crossing
  plus thickness-continuity rule and publishes evidence only when that
  first-stable point exactly matches the retained `LineGauge` result;
- polarity, minimum contrast, thickness, sampling interval, ROI, scan
  endpoints, selected source-image point, signed response, spatially distinct
  unselected alternative, source/result SHA-256, and deterministic evidence ID
  are retained in the view and TSV;
- the result image draws the exact representative scan, selected point, and
  bounded alternatives. The chart keeps the selected and spatially distinct
  later stable edge separate instead of treating adjacent response samples as
  separate physical alternatives;
- the shared plot now supports negative signed values and a visible zero axis
  without changing positive-only Histogram/Threshold behavior;
- parameter/input changes, active Tool input-image load, and active `Main`
  workspace image replacement clear stale Line evidence/result state without
  Preview. Open/back, cursor, zoom/pan/reset, and TSV export preserve Preview
  count, layers, active layer, and routes;
- the frozen `Public_Line_Pins_Good` and
  `Public_Line_Pins_WidePin_Bad` pair replayed the same `LineDistance`
  parameters. Good measured `37 px / 0.222 mm / 24 edge points` and selected
  `(462,242)`; WidePin Bad measured `17.7 px / 0.106 mm / 24 edge points` and
  selected `(478,242)`. Both retained a spatially distinct later stable edge;
- an independent synthetic matrix passed `X_LTOR`, `X_RTOL`, `Y_TTOB`, and
  `Y_BTOT` first-stable replay, and the existing Line/Threshold signal
  regressions passed.

Evidence:

- `docs/reports/OPENVISIONLAB_LINE_SIGNAL_PROFILE_20260728.md`;
- `artifacts/cvr03_line_signal_profile_20260728`;
- focused smoke `wpf_line_signal_profile`;
- related smokes `wpf_shell_host_line_tool`,
  `wpf_shell_host_line_pins_measure_tool`,
  `wpf_shell_host_line_measure_tool`,
  `wpf_shell_host_line_intersection_tool`,
  `wpf_shell_host_line_presets`,
  `wpf_threshold_signal_good_bad_replay`,
  `wpf_shell_host_threshold_tool`, and
  `wpf_simple_preprocess_result_review`.

Boundary:

- this is one representative scan from the current selected Line result, not
  every scan row overlaid at once;
- distinct later stable transitions are diagnostic alternatives, not new
  acceptance rules;
- no `LineGauge`/`LineDistance` detection, fit, measurement, XML, calibration,
  or acceptance semantics changed;
- this does not qualify unseen data, field robustness, certified metrology, or
  any later candidate. `CVR-04` was completed separately from actual
  CircleGauge runtime evidence.

### CVR-04 — Circle sampling and residual review

Minimum slice:

- reviewed radial scan locations;
- per-scan intensity/edge response;
- accepted/rejected edge sample state;
- fitted radius residual distribution and current support gate;
- two-way selection between a sample row/plot and the image drawing.

It remains pixel geometry unless an existing positive, verified uniform scale is
applied. It does not add camera calibration.

Completion:

- the existing `CircleGauge` runtime now retains every reviewed radial scan
  used by the current Run: scan angle/endpoints, prepared intensity, signed
  response, selected edge position/radius/strength, contrast acceptance,
  robust-fit inlier state, signed radius residual, and exact reject reason;
- the evidence is captured inside the existing execution loop. It does not
  recompute an approximate circle from the final drawing or change edge
  selection, the initial/refined least-squares fits, the existing robust
  rejection threshold, support gate, radius gate, or residual gate;
- Pipeline Review exposes a Circle Evidence tab only for `CircleGauge`. It
  shows taught/fitted circle values, candidate/inlier/support/coverage and RMS
  gates, the complete sample table, the residual distribution, and the
  selected radial intensity/signed-response profile through the shared Tool
  Signal Inspector;
- selecting a sample row, residual plot position, or the compact reviewed
  drawing selects the same stable scan identity. The drawing shows the actual
  radial scan, fitted circle, selected edge point, inlier/outlier/reject state,
  and residual without another Preview/Run or layer/route change;
- frozen settings (`ROI=100,50,200,200`, center `200,150`, radius `50..80`,
  `180` scans, LightToDark, contrast `>=40`, support `>=0.8`, RMS `<=1 px`)
  accepted the Good circle at radius `67.831 px`, support `0.917`,
  coverage `330 deg`, and RMS `0.517 px`. Its 180 scans retained 171 edge
  candidates, 165 final inliers, 9 contrast rejects, and 6 robust-fit
  outliers;
- the identical settings rejected the Bad ellipse at RMS `3.427 px > 1 px`.
  Row/plot/drawing selection and the two-series selected profile were verified
  with zero Run Review requests.

Evidence:

- `docs/reports/OPENVISIONLAB_CIRCLE_RESIDUAL_REVIEW_20260728.md`;
- `artifacts/cvr04_circle_residual_review_20260728`;
- focused smoke `cvr04_circle_residual_review`;
- related geometry smokes `p213_geometry_review`,
  `p213_geometry_property_grid`, and `p214_two_point_scale`;
- shared signal regressions `wpf_line_signal_profile`,
  `wpf_threshold_signal_good_bad_replay`, and
  `wpf_shell_host_threshold_tool`.

Boundary:

- the evidence belongs to the current in-memory Pipeline Run and is not added
  to saved Run Report/history persistence in this slice;
- values remain pixel geometry. Existing positive uniform scale behavior is
  unchanged, and no camera calibration, distortion correction, or certified
  metrology is added;
- the synthetic Good/Bad pair proves the named diagnostic and unchanged
  frozen gate replay only. It does not prove unseen-data robustness,
  production accuracy, or field qualification;
- no new algorithm family, XML parameter, CircleGauge detection/fit/gate
  semantic, automatic parameter choice, or acceptance rule was introduced.
- `CVR-05` was completed separately from the existing Blob/Contour Object
  Results contract.

### CVR-05 — Object-metric distribution

Minimum slice:

- current retained Blob/Contour candidate rows;
- distribution of existing `Area`, `BoundsWidth`, or `BoundsHeight`;
- accepted/rejected colors and exact reject reason;
- lower/upper markers bound to one existing PropertyGrid range;
- no new descriptor in this slice.

Add circularity, aspect ratio, angle, holes, or gray features only through
`CVR-16` after a separate named task proves the need.

Completion:

- Pipeline Review reuses the current `VisionPipelineObjectResult` rows retained
  by the existing Blob/Contour execution path; it does not rerun segmentation
  or create a second candidate population;
- the Object Results tab now presents the selected-object drawing, existing
  object table, and a shared two-series binned distribution together. Operators
  explicitly select `Area`, `Bounds width`, or `Bounds height`;
- accepted and rejected candidates are separate green/red series. The exact
  selected row and reject reason remain visible, and table, image, and plot
  selection resolve to the same stable object number without another
  Preview/Run;
- the selected metric reads exactly one existing range:
  `MIN_AREA/MAX_AREA`, `MIN_WIDTH/MAX_WIDTH`, or
  `MIN_HEIGHT/MAX_HEIGHT`. Markers are read-only current Pipeline/PropertyGrid
  values; the view does not recommend or apply a gate;
- a missing legacy maximum preserves the existing `1000000` unbounded
  compatibility sentinel. It is stated as unbounded and omitted from plot
  scaling so a useful distribution is not compressed by a fake finite
  operating limit;
- source/result SHA-256, stable evidence ID, tool/input/region/parameter
  identity, counts, range values, and accepted/rejected bin counts use the
  shared Tool Signal evidence/TSV contract;
- a frozen five-row UI matrix retained two accepted and three rejected rows,
  including area-low, area-high, and width-high exact reasons. Blob
  Area/Width/Height and Contour Area identities/ranges passed;
- the actual public product path passed for
  `Public_Blob_Particles_Good` (`ResultCount=12`, 245 retained audit rows),
  `Public_Blob_Particles_Sparse_Bad` (`ResultCount=3`, 253 retained audit
  rows), and `Public_Contour_Shapes_Missing_Bad` (`ResultCount=2`, 2 retained
  rows) with two distribution series, two finite Area markers, a 64-character
  evidence ID, object row/drawing selection, and unchanged
  layer/route/Preview state.

Evidence:

- `docs/reports/OPENVISIONLAB_OBJECT_METRIC_DISTRIBUTION_20260728.md`;
- `artifacts/cvr05_object_metric_distribution_20260728`;
- focused smoke `cvr05_object_metric_distribution`;
- actual public-product smokes
  `wpf_shell_host_workspace_sample_pipeline_review_metrics`,
  `wpf_shell_host_workspace_sample_pipeline_review_blob_ng_metrics`, and
  `wpf_shell_host_workspace_sample_pipeline_review_bentpin_ng_metrics`;
- runtime regression `--object-dimension-filter-contract`.

Boundary:

- the distribution uses only existing axis-aligned pixel `Area`,
  `BoundsWidth`, and `BoundsHeight`. It does not add circularity, aspect ratio,
  rotated size, angle gates, holes, grayscale descriptors, or semantic
  classification;
- markers are review evidence, not an editing or automatic suggestion surface;
- the bounded synthetic/public samples do not prove unseen-data robustness,
  production accuracy, or field qualification;
- no Blob/Contour detector, filter order, ResultCount, aggregate metric,
  XML/property, report-persistence, or acceptance semantics changed.

### CVR-06 — Matcher diagnostic surface

Minimum slice:

- trained edge/model visualization;
- active pyramid/coarse-fine level;
- chosen candidate and strongest spatially distinct alternative;
- score, margin, angle, scale, search ROI, and relevant internal diagnostics;
- exact `Success`, `NoMatch`, or `Ambiguous` reason from the retained run.

The surface is diagnostic only. It must not lower gates, change defaults,
auto-select a pattern, or turn diagnostic risk metrics into acceptance rules.

Completion:

- Library-Noah retains the exact trained edge-model points, model center,
  search ROI, primary hypothesis, strongest spatially distinct alternative
  when present, decision state/reason, and existing model/candidate/unique
  metrics;
- Pipeline Review renders the trained model and exact source-coordinate
  candidates, and distinguishes accepted `Selected`, `Best observed (below
  gate)`, and `Rejected primary hypothesis`;
- model-pyramid usability levels are shown separately from the actual existing
  coarse proposal scale and proposal/verification/acceptance/fallback
  counters;
- one public Good run retained `Success`, one public Wrong run retained the
  exact `MatchingNoResult`, and a deterministic repeated-pattern matrix
  retained two equal-score spatially distinct hypotheses with the exact
  `MatchingAmbiguous` reason;
- diagnostic inspection does not rerun Preview/Run or change layers, active
  layer, routes, matcher defaults, XML, PropertyGrid, gates, or acceptance.

Evidence:

- `docs/reports/OPENVISIONLAB_MATCHER_DIAGNOSTIC_SURFACE_20260728.md`;
- `artifacts/cvr06_matcher_diagnostic_20260728`;
- Library-Noah Release build and `66/66` inspection smoke;
- focused current-source UI smokes `cvr06_matcher_diagnostic`,
  `wpf_shell_host_workspace_sample_pipeline_review_edge_ng_metrics`, and
  `wpf_shell_host_edge_based_matching_tool`.

Boundary:

- this does not qualify a physical feature or template, choose a pattern,
  change matching semantics, or complete any of `CVR-11` through `CVR-15`;
- `CVR-07` was subsequently activated by explicit user selection and completed
  as the bounded Threshold Basic v1 contract below.

### CVR-07 — Bounded teaching suggestions

Implement one task-specific suggestion contract at a time:

- explain why a candidate is suggested or rejected;
- preview its exact ROI/geometry;
- require explicit `Use` or `Apply`;
- keep the previous teaching state recoverable;
- do not run the inspection merely because a suggestion is selected.

Do not create a generic MERLIC easyTouch clone. Existing Auto MPoint remains the
reference for suggestion/explicit-accept separation.

Completion — Threshold Basic v1:

- one retained explicit Preview provides the full-image 256-bin source
  histogram; analysis does not run the tool again;
- Binary selects a bright-object candidate between the two highest retained
  significant gray modes, while BinaryInv mirrors the direction for a
  dark-object candidate;
- the UI shows one exact orange candidate marker, mode pair, separation, lower
  and upper populations, source hash, region, and stable evidence ID;
- a one-mode or undersized-class histogram rejects the candidate and leaves
  manual teaching unchanged;
- only explicit `Use T` changes the teaching value and follows the existing
  debounced Preview policy; the previous same-source value remains recoverable
  with `Undo`;
- the first global Otsu attempt was genuinely rejected at `T=73` because the
  public Good and Bad both returned `ResultCount=0`;
- one bounded bright-mode correction produced `T=138` from modes `97/178` and
  preserved public Good `ResultCount=4` / Bad `ResultCount=1`;
- Analyze, candidate review, and unrelated navigation do not change Preview/
  Run count, layers, active layer, or routes.

Evidence:

- `docs/reports/OPENVISIONLAB_THRESHOLD_TEACHING_SUGGESTION_20260728.md`;
- `artifacts/cvr07_threshold_suggestion_20260728`;
- focused `cvr07_threshold_suggestion`, Threshold Basic/full, frozen CVR-02
  Good/Bad, CVR-06, and readiness regressions.

Boundary:

- this completes one task-specific Threshold Basic v1 contract only;
- Range, Adaptive, ROI suggestions, Line, Circle, generic easyTouch,
  automatic apply, automatic gate changes, and additional suggestion families
  remain excluded until a separate exact trigger or explicit user selection.

### CVR-08 — Generic typed fixture consumer

2026-07-28 activation audit (historical):

- the current NormalizeImage runtime already produces a reusable
  reference-coordinate layer and normal Pipeline routing can reach multiple
  downstream Steps;
- P212 review returns the first reachable valid `CvROI`, so its current
  presentation is single-ROI;
- the only tracked public NormalizeImage Pipeline contains one downstream ROI;
- P235 preserves a 24-row qualified-with-limits locator set, but its exact
  promoted Pipeline is one EdgeBasedMatching Step without a published fixture
  frame, taught reference pose/image size, NormalizeImage, downstream ROI, or
  inspection gate;
- no named operator task currently supplies two physical ROIs, their existing
  tools/metrics/gates, Good/Bad evidence, and a demonstrated P212/P219 blocker.

Decision at audit time: `Blocked`. The audit remains the evidence for why a new
transform runtime was not justified. Audit and source evidence:
`docs/reports/OPENVISIONLAB_CVR08_TRIGGER_AUDIT_20260728.md`.

Subsequent explicit decision and completion:

- the user delegated the bounded task choice with `알아서 해주세요`;
- the selected public synthetic task uses the already reviewed fixture locator
  and `NormalizeImage` output;
- ROI A `210,240,55,55` verifies the circular datum with existing `Blob`,
  `Area=350..600`, and `ResultCount=1`;
- ROI B `320,180,60,50` verifies pad presence with existing `Blob`,
  `Area=700..1300`, and `ResultCount=1`;
- the Good sample passes both consumers; the controlled missing-pad Bad sample
  keeps the datum consumer OK and fails only the pad consumer;
- Pipeline Review now retains every reachable single-`CvROI` consumer with
  stable evidence identity, route/status/ROI columns, all source/reference
  polygons, selected highlighting, and selected Recipe Manager edit handoff;
- row selection does not execute Preview/Run or mutate layers, active layer,
  routes, or the saved recipe.

Evidence:

- `docs/reports/OPENVISIONLAB_CVR08_MULTI_ROI_FIXTURE_20260728.md`;
- `artifacts/cvr08_multi_roi_fixture_20260728`;
- focused `wpf_shell_host_workspace_sample_normalize_fixture_review` and legacy
  fixture-review regressions.

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

Implementation status:

- bounded v1 is implemented as `LineFixture` with compatibility alias
  `DualEdgeFixture`;
- it consumes two distinct exact typed `Segment` results from earlier accepted
  `Line`/`LineGauge` Steps and does not duplicate the Line detector;
- the intersection publishes the origin, Datum A publishes the fixture X axis,
  and the existing Fixture/`NormalizeImage` consumer owns application;
- PropertyGrid, XML, validation, known metrics, typed `Origin/Point`, exact
  drawings, and Pipeline Review quality text are connected;
- eight actual translation/rotation/repeated-rail synthetic pipelines passed,
  and duplicate source plus incompatible included angle failed closed;
- this completes reusable implementation and synthetic integration only.

Minimum slice:

- operator-reviewed physical horizontal/vertical or two non-parallel datum
  edges;
- support, polarity, contrast, angle, intersection/origin, and ambiguity gates;
- exact edge-support and fixture-axis drawings;
- one typed fixture output compatible with `CVR-08`;
- N-sample proof that reflections or repeated rails do not replace the intended
  physical datum.

Qualification prerequisite:

- named part and downstream inspection intent;
- representative images and allowed pose range;
- operator-certified Datum A/B physical identities and polarity/contrast
  expectations;
- evidence that a durable Matching/Affine locator is unsuitable;
- reviewed N-sample evidence that nearby rails/reflections do not replace
  either datum.

Evidence:

- `docs/contracts/openvisionlab/OPENVISIONLAB_LINE_FIXTURE_V1_CONTRACT.md`;
- `docs/reports/OPENVISIONLAB_CVR09_LINE_FIXTURE_20260728.md`;
- `artifacts/cvr09_line_fixture_20260728_r11`.

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
Boundary / next dependency: CVR-01 through CVR-08 are complete. CVR-09 bounded implementation/synthetic integration is complete but physical-task qualification remains blocked on its named operator/data packet. CVR-00 remains the only active external prerequisite; CVR-10 and later rows still require their exact current-source trigger or an explicit user decision and must not be auto-activated.
```
