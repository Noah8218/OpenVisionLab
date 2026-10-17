# OpenVisionLab Auto MPoint V1 Contract

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

OpenVisionLab owns the operator surface that:

- uses the separately built, manifest-verified OpenVisionLab Vision SDK DLL;
- exposes PropertyGrid inputs;
- runs only from an explicit `Analyze candidates` action;
- shows candidate rows and the same result drawing;
- applies a chosen region only through a separate explicit `Use this pattern`
  action.

Candidate selection alone must not save a template, mutate XML, change an input or
output layer, run Preview, or run the pipeline.

The candidate list is a teaching-time shortlist and may contain more than one
accepted suggestion. Runtime uniqueness separately adds an opt-in runtime anchor contract: an
inspection match succeeds only when the selected result is sufficiently separated
from every spatially distinct plausible alternative. Auto MPoint analysis does
not enable that runtime option automatically.

## OpenVisionLab Integration

The Tool View integrates the library core into the existing Edge Based Matching Tool View
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

## Runtime Uniqueness

Runtime uniqueness extends the existing matcher without changing the teaching
contract:

- legacy missing-key behavior remains disabled with default margin `0.03`;
- enabled mode requires external `NUM_MATCH=1` and one search region;
- the matcher retains at least eight internal candidates;
- `NoMatch` and `Ambiguous` return no `MatchingResult`;
- `Success` returns exactly one result;
- normalized state, alternative-count, score-margin, and exact-reason evidence is
  available to OpenVisionLab Pipeline review;
- the two PropertyGrid/XML fields do not auto-run Preview.

See `docs/OPENVISIONLAB_EDGE_BASED_UNIQUE_MATCH_V1_CONTRACT.md` for the uniqueness contract.

## Representative-Image Automatic Best Selection

Representative analysis extends the teaching action without creating a new Pipeline Step:

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

## Operator N-Image HTML Evidence Export

The existing panel exposes one explicit `Save N-image report` action.

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

This report is AutoMPoint locator-teaching evidence. It does not replace Recipe
Manager Validation Set roles, OK/NG acceptance, Run History, or a qualified
inspection report.

## Applicability

The output is a suggestion for operator review, not proof that a template is a
durable locator under production pose, lighting, focus, contamination, wear or part
variation. Keep the result labelled `Suggested`, not `Qualified`.
Numerical uniqueness does not establish the operator's intended physical identity.
Applying the selected pattern, explicit Matching Preview and later N-image
validation remain separate actions.
