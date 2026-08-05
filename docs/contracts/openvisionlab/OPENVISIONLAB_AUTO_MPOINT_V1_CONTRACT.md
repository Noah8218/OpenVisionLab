# OpenVisionLab Auto MPoint V1 Contract

Status: P223 teaching integration, P224 optional runtime uniqueness, P229
representative-image automatic best-pattern selection, and P230 one-source-stratum
qualification complete with documented limits; production qualification pending

## Product Decision

Auto MPoint is an operator-assisted matching-template teaching feature. It examines
one reference image, proposes fixed-size regions that are likely to be useful for
the existing edge-based matcher, and exposes the evidence needed for the operator
to choose one.

It is not an inspection Pipeline Step and it does not automatically mutate a
recipe. The runtime inspection remains the existing deterministic
`EdgeBasedTemplateMatchingTool`.

## V1 Scope

- Input: one reference image.
- Analysis area: full image or one explicit rectangular ROI.
- Pattern size: operator-supplied fixed width and height.
- Candidate search: deterministic grid, whole ROI, or both.
- Output: up to five overlap-suppressed candidate regions.
- Target matcher: existing OpenVisionLab Vision SDK edge-based matcher with optional hybrid
  verification.
- Evidence: feature score, self/alternative match scores, uniqueness margin,
  synthetic replay success and pose error, median/P95 runtime, exact reject reason,
  result drawing, and public overlays.

Automatic size selection, SIFT/ORB candidate discovery, semantic segmentation,
production qualification, affine three-point grouping, Homography, and automatic
template application are outside V1.

## Candidate Evaluation

The library performs two bounded stages:

1. Convert the source once and rank fixed-size windows by contrast, edge density,
   four-quadrant edge balance, and X/Y orientation balance.
2. Evaluate only overlap-suppressed finalists with the existing edge matcher.

An exact finalist must:

- find its taught location;
- exceed the configured matching score;
- separate its self match from the strongest distant alternative by the configured
  uniqueness margin;
- pass three known synthetic whole-image replays;
- stay inside the configured position, angle, scale, and optional runtime gates.

The authored MPoint is the candidate rectangle center. The result also retains the
edge matcher's native center and the reference offset between those two points.

## Ownership

OpenVisionLab Vision SDK owns:

- `AutoMPointToolProperty` and its fail-closed parameter contract;
- candidate generation, feature scoring, overlap suppression, exact matching,
  uniqueness, synthetic replay, precision/runtime metrics, ranking, and drawings;
- stable Auto MPoint result/error types;
- synthetic smoke evidence.

OpenVisionLab owns the P223 operator surface that:

- uses the separately built, manifest-verified OpenVisionLab Vision SDK DLL;
- exposes PropertyGrid inputs;
- runs only from an explicit `Analyze candidates` action;
- shows candidate rows and the same result drawing;
- applies a chosen region only through a separate explicit `Use this pattern`
  action.

Candidate selection alone must not save a template, mutate XML, change an input or
output layer, run Preview, or run the pipeline.

The candidate list is a teaching-time shortlist and may contain more than one
accepted suggestion. P224 separately adds an opt-in runtime anchor contract: an
inspection match succeeds only when the selected result is sufficiently separated
from every spatially distinct plausible alternative. Auto MPoint analysis does
not enable that runtime option automatically.

## Historical Predecessor-Library Evidence

On 2026-07-24, the predecessor Library-Noah Release build completed with zero warnings/errors and
the full inspection smoke suite passed 60/60.

The historical `Lib.OpenCV.dll` retained assembly version `2.1.0.0`, file version
`2.8.0.0`, and SHA-256
`3D7A0B5D392B096DB3C14091D08E52BBB840772C1BDD1B30BEB15475ABAE28D9`.
P223 verifies the same hash for the source-library output, the Dev vendor copy,
and the current Debug build output.

The bounded matrix proves:

- one asymmetric feature is suggested first at ROI `64,64,64,64`;
- identical input executions retain the same ranking and identical drawing pixels;
- two identical repeated patterns both fail the uniqueness gate;
- invalid analysis ROI and oversized pattern definitions fail closed.

Historical source contract:
`C:\Git\Library-Noah\docs\AUTO_MPOINT_V1.md`

Historical evidence:
`C:\Git\Library-Noah\artifacts\auto_mpoint_v1_20260724`

## Completed OpenVisionLab Integration

P223 integrates the library core into the existing Edge Based Matching Tool View
instead of creating a new Pipeline tool family.

- Auto MPoint settings are grouped in the existing PropertyGrid.
- The teaching panel is collapsed by default so the existing docked PropertyGrid
  retains its working height.
- Property edits do not schedule Preview.
- `Analyze candidates` is the only analysis trigger.
- The panel shows the selected candidate ROI, uniqueness margin, synthetic
  position error, P95 runtime, and the library-owned result drawing.
- `Use this pattern` explicitly saves the selected ROI through the existing
  template-teaching path and updates `PATTERN_PATH`.
- Analyze, row selection, and apply preserve Preview/Run count, input/output
  layers, active layer, and routes.
- Applied patterns remain `Suggested`; an explicit Matching Preview and later
  N-sample qualification are separate operator actions.

Current UI evidence:

- before:
  `artifacts\p223_auto_mpoint_ui_20260724\before\wpf_shell_host_edge_based_matching_tool.png`
- after:
  `artifacts\p223_auto_mpoint_ui_20260724\after_verified\wpf_shell_host_edge_based_matching_auto_mpoint.png`

## Completed P224 Runtime Uniqueness Extension

P224 implements the first ordered matcher direction without changing the teaching
contract:

- legacy missing-key behavior remains disabled with default margin `0.03`;
- enabled mode requires external `NUM_MATCH=1` and one search region;
- the matcher retains at least eight internal candidates;
- `NoMatch` and `Ambiguous` return no `MatchingResult`;
- `Success` returns exactly one result;
- normalized state, alternative-count, score-margin, and exact-reason evidence is
  available to OpenVisionLab Pipeline review;
- the two PropertyGrid/XML fields do not auto-run Preview.

The focused matrix covers legacy repeated-pattern success, unique distinct
success, repeated-pattern ambiguity rejection, and absent-pattern no-match
rejection. See
`docs\OPENVISIONLAB_EDGE_BASED_UNIQUE_MATCH_V1_CONTRACT.md` and
`artifacts\p224_unique_match_runtime_20260724`.

The historical P224 source, vendored, and Debug `Lib.OpenCV.dll` files retained
assembly `2.1.0.0`, file `2.8.0.0`, and SHA-256
`000C75A7D0E796E166DF6F24C95F264FC001927881B1ED7DE7BAE31913099F6D`.

## P225 Fixed-ROI Candidate Decision

P225 reused the operator-approved P220/P221 `card_original` `R` anchor, rather
than inventing a new Auto MPoint result. The exact reference/template, 12 source
hashes, prior reviewed centers, search ROIs, angle/scale envelope, `0.45` score,
`0.03` unique margin, and `<=5 px` center gate were frozen before execution.

| Mode | Correct accept | Baseline mismatch >5 px | Ambiguous | No match |
| --- | ---: | ---: | ---: | ---: |
| reviewed ROI + unique | 0/12 | 2 | 2 | 8 |
| original broad ROI + legacy | 1/12 | 2 | 0 | 9 |
| original broad ROI + unique | 0/12 | 2 | 1 | 9 |

Current-run drawing review confirmed that one reviewed-ROI result selected the
`T` glyph instead of `R` at score `74.237` with no plausible alternative. The
unique gate therefore answers whether another similar candidate remains; it
does not prove that the selected feature has the operator's intended identity.
The fixed candidate decision is `Reject`.

P225 also closes the Pipeline handoff required by any future successful
candidate: existing EdgeBased scale/refinement settings survive builder/factory
execution, and exactly one successful EdgeBased result publishes typed
`Center`. This wiring is not qualification evidence by itself.

Evidence: `artifacts\p225_edge_unique_card_r_matrix_20260724`.

## P226 Public EasyMatch Candidate Presentation

P226 executes the existing Auto MPoint engine on five diverse public reference
images without applying a candidate:

- `BOARD.JPG`
- `Die Pad 1.bmp`
- `Floppies.jpg`
- `Frame 1.tif`
- `Switch1.tif`

The current product defaults were frozen before execution: `96x96` candidate
window, `16 px` stride, eight exact finalists, top-five display, minimum feature
quality `0.15`, matching score `0.75`, uniqueness margin `0.05`, and maximum
synthetic position error `2.5 px`. Five current-run drawings and 40 exact
candidate rows were retained. Twenty-eight candidates passed the internal gates,
and the display cap exposed 20 suggestions on four images.

`Frame 1.tif` rejected all eight repetitive finalists at uniqueness margins
`0.0011..0.0054`. `Floppies.jpg`, however, suggested repeated disk hubs whose
fixed orientations made them numerically distinct while angle search was off.
This contrast is the intended product boundary: Auto MPoint can reject
image-space ambiguity, but it cannot decide whether the pattern is a durable,
non-inspected physical locator.

No result-dependent tuning, pattern apply, cross-image Matching, Affine,
inspection, or OK/NG run occurred. Evidence:
`artifacts\p226_auto_mpoint_easymatch_candidates_20260724_r2`.

## P229 Representative-Image Automatic Best Selection

P229 extends the teaching action without creating a new Pipeline Step:

- the operator may select multiple same-size representative images;
- the library evaluates each accepted reference-image finalist with the existing
  edge matcher on every representative image;
- candidates below the configured representative success rate are rejected;
- survivors are ranked by success rate, minimum uniqueness margin, mean score,
  then the original one-image score;
- the UI marks rank one as `BEST` and selects it, but applying the pattern remains
  a separate explicit action;
- selecting images, analyzing, and selecting rank one do not Preview, Run, change
  layers, or change routing.

The bounded `Die Pad 1.bmp` pilot used one canonical OK image, four OK plus four NG
representative images, and a disjoint four OK plus four NG held-out set selected
by deterministic hash spread. The frozen `96x96`, stride `16`, top-eight,
score `0.75`, uniqueness `0.05`, angle `-8..8`, and scale `0.9..1.1` contract
automatically selected ROI `128,256,96,96`. This is the same ROI retained by the
earlier P227 operator-reviewed pilot. The selected template succeeded on all
representative `8/8` and held-out `8/8` rows with zero runtime or integrity
errors. Drawings consistently locate the same central pad/trace feature.

The first attempted run correctly returned no candidate because its validation
configuration omitted the known dataset angle/scale envelope and used a clustered
sample selection. P229 corrected only those evidence/configuration mismatches; it
did not lower the score, uniqueness, or representative-success gates.

Current UI evidence:
`artifacts\p229_auto_mpoint_representative_best_20260724\ui_after_current_r4\wpf_shell_host_edge_based_matching_auto_mpoint.png`.

Primary operator report:
`artifacts\p229_auto_mpoint_representative_best_20260724\die_pad_1_r3_current\OPENVISIONLAB_AUTO_MPOINT_REPRESENTATIVE_BEST_REPORT.html`.

Historical predecessor-library evidence:
`C:\Git\Library-Noah\artifacts\auto_mpoint_representative_v2_20260724`.

The historical source, vendored, and Debug `Lib.OpenCV.dll` files were assembly
`2.1.0.0`, file `2.8.0.0`, SHA-256
`B456BE7AFC002BA1535A5892092B746FB44560300961BD71342AAC0E7741B180`.

This proves bounded automatic numerical selection and replay for one
synthetic/augmented same-source stratum. It does not prove semantic identity,
automatic pattern size, a production motion envelope, all 500 rows, real captured
variation, or field qualification. Operator drawing review remains required before
a full-stratum replay.

## P230 Frozen Full-Stratum Qualification

After operator approval of the P229 drawings, P230 froze the exact
`128,256,96,96` template and existing matcher envelope. It did not rerun Auto
MPoint selection or change score, uniqueness, angle, scale, or ROI settings.

- corpus stratum: all 122 rows whose source identity is `Die Pad 1.bmp`;
- roles: 62 OK and 60 NG;
- runtime outcome: 122/122 success, zero ambiguous, no-match, runtime, or
  hash-integrity failures;
- evidence: 122/122 current-run drawings plus a deterministic 35-row review queue;
- visual result: every reviewed green result remained on the same central
  pad/trace feature;
- defect exposure: nine NG defect masks intersected the 96x96 matched bounds.

The first generated report treated any defect-mask intersection as a fatal locator
failure. That rule was corrected because intersection means the taught pattern may
contain changing defect pixels, not that the runtime selected the wrong location.
No image, template, matcher parameter, score, or runtime result changed. The
corrected report therefore closes as `Keep with documented limits`: wrong
location, ambiguity, and missing matches remain fatal, while the nine intersections
remain an explicit production-variation risk.

Primary report:
`artifacts\p230_auto_mpoint_die_pad_1_full_stratum_20260724_r2\OPENVISIONLAB_AUTO_MPOINT_FULL_STRATUM_REPORT.html`.

Visual review:
`artifacts\p230_auto_mpoint_die_pad_1_full_stratum_20260724_r2\drawing_review_record.txt`.

P230 completes only the `Die Pad 1.bmp` source stratum. The 500-image package also
contains distinct Die Pad 2-4 source strata, so this template must not be replayed
across those sources as if they shared one physical pattern.

## P231 Operator N-Image HTML Evidence Export

The product UI audit confirmed one real gap: the Edge Based Matching Tool View
could select representative images, calculate and select `BEST`, and explicitly
apply the pattern, but the P230-style evidence export existed only in a validation
tool. P231 adds one explicit `Save N-image report` action to that existing panel.

The report uses the selected candidate's already-retained
`RepresentativeMatches`; it does not execute the matcher again. One self-contained
HTML file contains:

- selected ROI, candidate rank, source PNG hash, and analysis definition;
- success count, minimum score/uniqueness, maximum runtime, and selected template;
- all failures and bounded score, uniqueness, runtime, angle, scale, and SHA-256
  spread drawings, with duplicates removed;
- every drawing when N is 24 or smaller;
- a complete N-row table with file hash, outcome, pose, score, uniqueness, runtime,
  message, and review-queue reason;
- browser print/PDF support with no external image dependency.

The action is enabled only after representative-image analysis. Changed source,
settings, representative path/size/timestamp identity, missing results, missing
files, or result/path count mismatch fail closed. Export does not apply the
candidate, run Preview/Run, create or select layers, or change routing.

Current-source evidence:
`artifacts\p231_auto_mpoint_operator_html_report_20260724\after_current_build_r3`.

This report is AutoMPoint locator-teaching evidence. It does not replace Recipe
Manager Validation Set roles, OK/NG acceptance, Run History, or a qualified
inspection report.

## GPT Pro Research Review And Adopted Direction

The operator-provided research was checked against the current
`EdgeBasedTemplateMatchingTool` source and official public commercial
documentation. Its central conclusion is accepted: keep and evolve the current
edge matcher rather than replace it.

### Current-source findings

| Research claim | Source check | Decision |
| --- | --- | --- |
| Candidate ambiguity is diagnostic only | Confirmed. `RecordCandidateAmbiguityDiagnostics` records distant alternatives, but normal success still primarily checks `candidate.Score >= SCORE_MIN`. | Make fail-closed uniqueness the next bounded algorithm contract. |
| Current subpixel is a five-score parabolic adjustment | Confirmed. X and Y offsets are independently estimated from center/left/right/top/bottom scores and clamped to a half pixel. | Keep for compatibility; evaluate joint full-resolution refinement only after uniqueness is proven. |
| Model downsampling is not spatial/orientation balanced | Confirmed. `Downsample` selects by sequence interval. | Later retain coarse compatibility and add a balanced real-valued refinement model only with measured benefit. |
| Model offsets are integer-valued | Confirmed in current edge model creation/cache path. | Do not rewrite the coarse matcher now; real-valued points belong to a later refinement slice. |
| Hybrid selection evidence is not public result evidence | Confirmed. candidates retain image/descriptor/hybrid scores internally, while `MatchingResult` exposes the final legacy score/pose/bounds only. | The unique-match slice must expose the exact score margin and reject reason; additional diagnostics follow only when they gate a real decision. |

The commercial comparison supports the direction without proving parity:

- [HALCON shape-model search](https://www.mvtec.com/doc/halcon/2605/en/find_shape_models.html)
  documents coarse-to-fine search, subpixel/least-squares options, and clutter
  constraints.
- [HALCON 26.05 release notes](https://www.mvtec.com/products/halcon/documentation/release-notes-2605-0)
  and
  [sample-based shape-model training](https://www.mvtec.com/doc/halcon/2605/en/set_generic_shape_model_param.html)
  document sample-based removal of unstable or misleading model contours.
- [Cognex PMAlign results](https://support.cognex.com/docs/vpromx_1000/web/en/visionpro/Content/Topics/users-guide/control-reference/pmalign/results-tab.htm?TocPath=VisionPro+Users+Guide%7CControl+Reference%7CPMAlign+Tool%7C_____6)
  expose fit error, coverage, clutter, score, and pose.
- [Cognex PatMax RedLine theory](https://support.cognex.com/docs/is_590/web/EN/ise/Content/Reference/PatMaxRedLineTheoryOfOperation.htm?tocpath=Function+Reference%7CVision+Tools+Functions%7CPattern+Match%7COverview+of+PatMax+RedLine%7C_____1)
  separates coarse candidate discovery from fine scoring and warns that a
  candidate lost during coarse search cannot be recovered later.

These sources justify separate coarse-candidate and final-acceptance contracts.
They do not establish that OpenVisionLab currently has HALCON/Cognex accuracy or
that their private implementations match the research description.

### Ordered development direction

1. **Unique-match runtime contract — completed by P224**
   - Preserve internal Top-K candidates even when the external requested result
     count is one.
   - Add an opt-in, backward-compatible validator: exactly one validated candidate
     is `Success`; zero is `NoMatch`; two or more plausible candidates is
     `Ambiguous`, with no `MatchingResult`.
   - Start with the already-computed score margin and exact reject reason. Do not
     add PSR, coverage, clutter, or fit metrics until one is tied to a frozen gate.
2. **Fixed-ROI evidence — P225 completed as a rejected candidate**
   - Freeze one representative ROI and motion envelope before tuning.
   - Measure accepted pose error, false accept, ambiguous rejection, and runtime on
     repeated/known-transform samples. One-image synthetic Auto MPoint evidence is
     not sufficient.
3. **Review a different suggested physical feature**
   - Do not retune the rejected `R` candidate. A second matrix requires one
     P226 Auto MPoint sample/rank/ROI explicitly approved by the operator as the
     same durable feature across representative images. Do not select by score
     alone.
4. **Joint full-resolution pose refinement**
   - Only after a fixed-ROI candidate passes, compare the current five-score
     interpolation with a `TranslationOnly` two-degree-of-freedom edge refinement.
   - Add angle refinement later only if the measured downstream error requires it.
5. **Balanced real-valued refinement model**
   - Preserve the current fast integer coarse model and evaluate spatial/orientation
     balance plus real-valued points for the refinement stage.
6. **Later, evidence-triggered options**
   - Adaptive window growth follows fixed-size teaching only if operator evidence
     shows that fixed windows are the blocker.
   - ODB/CAD hybrid models, global multi-anchor registration, Homography, shared
     multi-ROI gradient pyramids, and production auto-tuning stay out of scope until
     the user supplies that product direction and representative data.

Scale remains enabled only when the physical imaging setup requires it; fixed
optics should default to translation/rotation without silently adding degrees of
freedom. Values such as subpixel 3-sigma, P99, worst-case error, and zero false
accept are engineering targets to measure, not product guarantees.

## Honest Boundary

The completed result is a one-image synthetic suggestion engine, an optional
representative-image automatic ranking stage, an explicit OpenVisionLab teaching
UI with self-contained N-image evidence export, and an opt-in bounded synthetic
runtime uniqueness gate. It does not prove
that a proposed template is durable
under real production pose, lighting, focus, contamination, wear, or part
variation. The UI must keep the result labelled `Suggested`, not `Qualified`.
P225 supplies fixed-ROI evidence only for one rejected card `R` candidate. P230
qualifies one 122-row Die Pad source stratum with documented defect-overlap limits,
not the four-stratum 500-image package or production. Joint pose refinement,
automatic size selection, and field qualification remain unimplemented.

## Completion Record

Status: Complete

Scope: Explicit OpenVisionLab Auto MPoint teaching UI, current OpenVisionLab Vision SDK DLL
consumption/provenance, optional representative-image best-pattern ranking,
operator-reviewed apply action, self-contained N-image HTML evidence export,
documented matcher direction, and the P224 optional unique-result runtime/XML/UI
contract.

Acceptance criteria: PropertyGrid configuration, explicit analysis, candidate
rows/drawing, explicit apply/export, zero automatic Preview/Run or layer/routing
side effects, backward-compatible unique-result XML, representative and held-out
replay evidence, and separate success/ambiguous/no-match runtime states are
covered by the focused current-build UI and library smokes.

Verification: Initial predecessor-library Release build and 66/66 smoke; OpenVisionLab full
Debug build; focused Edge Based Matching/Auto MPoint UI smokes; localization,
readiness, external-reference, public-sample, JSON, DLL hash, and
`git diff --check`.

Evidence: `artifacts\p223_auto_mpoint_ui_20260724`,
`artifacts\p224_unique_match_runtime_20260724`, and
`artifacts\p225_edge_unique_card_r_matrix_20260724`, and
`artifacts\p226_auto_mpoint_easymatch_candidates_20260724_r2`, and
`artifacts\p229_auto_mpoint_representative_best_20260724`, and
`artifacts\p230_auto_mpoint_die_pad_1_full_stratum_20260724_r2`, and
`artifacts\p231_auto_mpoint_operator_html_report_20260724\after_current_build_r3`.

Boundary / next dependency: Production matcher identity, uniqueness, and
precision are not qualified. P230 closes the 122-row `Die Pad 1.bmp` stratum as
`Keep with documented limits`; nine supplied NG masks intersect the template
bounds, and the distinct Die Pad 2-4 strata remain untested by this template.
