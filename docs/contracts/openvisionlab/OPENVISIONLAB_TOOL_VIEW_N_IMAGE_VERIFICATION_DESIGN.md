# Tool View N-Image Verification Design

Updated: 2026-07-24 KST

Status: Phase 1 complete (P233); bounded locator promotion complete (P235)

## Decision

OpenVisionLab already has a formal N-image inspection path in Recipe Manager,
but it is not a shared function inside every Tool View.

The recommended product shape is:

1. Keep each Tool View focused on teaching one current image.
2. Add one common `N-image verification` entry action to eligible Tool Views.
3. Open one shared verification window instead of copying a batch panel into
   every tool-specific XAML file.
4. Freeze the current Tool View parameters into a transient one-Step Pipeline
   and reuse the existing Pipeline execution, per-image report, batch summary,
   drawing, and review-queue contracts.
5. Keep Recipe Manager Validation Set as the formal OK/NG classification and
   saved recipe-history path.
6. Keep execution sequential in Phase 1. Add bounded parallel execution only
   after tool isolation and thread-safety are proven.
7. For completed all-success locator sessions only, provide one explicit
   hash-locked promotion into Recipe Manager without rerunning or inferring
   defect labels.

This makes quick multi-image tuning convenient without creating a second batch
engine or weakening the existing explicit Preview/Run rules.

## Current Verified Product State

| Capability | Current state | Source evidence |
| --- | --- | --- |
| One-image Tool View teaching | Present | `VisionToolSingleInputPropertyToolShell.xaml` exposes input/output previews, `Add Pipeline`, and explicit `Run Preview`. |
| Local N-image registration | Present | Recipe-local Validation Sets accept multiple selected files or one top-level folder with explicit OK/NG roles. |
| Maximum registered images | Present, 5,000 | `OpenVisionRecipeValidationSetStorage.MaximumImageCount`. |
| N-image recipe execution | Present | `RunLocalValidationSetAsync` executes every registered row through `VisionPipelineSampleCheckService`. |
| Per-image retained evidence | Present | Each persisted suite row links a `report.xml` containing the source identity, Step metrics, objects, result image, and overlay image when available. |
| Batch summary | Present | `VisionPipelineBatchRunSummaryStorage` writes `summary.xml` and `summary.tsv`. |
| Failure/extreme review | Present | Saved deterministic review queue and Run History `NG only` / `review queue only` filters. |
| Drawing re-open | Present | Run History `View drawing` opens retained source and Step drawings without rerunning. |
| Generic Tool View N-image action | Present (P233) | One shared capability-driven action/window serves thirteen one-Step single-input Tool Views. |
| Generic HTML batch report | Present (P233) | The shared exporter uses only retained summaries/reports/images and never reruns the algorithm. |
| Locator-session promotion | Present (P235) | Completed all-success Matching-family sessions explicitly save an exact hash-locked Pipeline/Validation Set without activating or running it. |
| Concurrent multi-image execution | Missing | Local Validation Set, pair, and catalog loops await each image in order. They are N-image batch execution, not parallel execution. |

P231 is an intentional exception: AutoMPoint accepts representative images and
exports a self-contained HTML report because it is a teaching-time candidate
selector, not a Pipeline Step. It does not replace recipe OK/NG validation.

## UI Responsibility

### Tool View: quick parameter verification

Use when the operator wants to answer:

- Do the current parameters execute across these N images?
- Which images fail?
- How do the metrics, objects, pose, and execution time vary?
- Do the retained drawings stay on the intended target?

The Tool View remains the authoritative editor. Selecting files, opening the
verification window, changing rows, or exporting a report must not run Preview,
create layers, or change input/output routing.

### Recipe Manager: formal recipe validation

Use when the operator needs:

- expected OK/NG labels;
- a full multi-Step recipe;
- acceptance-gate judgement;
- named Validation Sets;
- saved Run History and baseline comparison;
- durable recipe qualification evidence.

A quick Tool View run must not be presented as formal OK/NG accuracy unless the
operator explicitly promotes the configuration and samples to this path.

## Proposed Operator Flow

1. Load one representative image in a Tool View.
2. Teach ROI, threshold, template, measurement, or other PropertyGrid values.
3. Run the existing explicit one-image Preview and inspect the drawing.
4. Press the new common `N장 검증` button.
5. In the shared window, add multiple files or one top-level folder.
6. Press `N장 실행`.
7. Review the summary, result table, selected drawing, and deterministic review
   queue.
8. Export a self-contained HTML report. For an all-success Matching-family
   locator session, optionally press `위치검출 세트 승격`; for defect-labelled
   qualification, continue to create explicit Recipe Manager OK/NG roles.

The shared window should contain:

- frozen tool name and parameter-snapshot hash;
- selected image count and missing/unreadable-file state;
- explicit Run and Stop-after-current-image actions;
- progress, success/failure/error counts, and average/p95/maximum time;
- per-image status, metrics, message, and SHA-256;
- selected retained drawing;
- `all failures + metric/time extremes + hash-spread` review queue;
- `HTML report save`;
- a clear `quick execution result, not OK/NG accuracy` label.

## Execution Design

### Shared adapter

Use the existing Tool View `CreateStep` path that already powers `Add Pipeline`.
At Run:

1. commit the pending PropertyGrid edit;
2. create one immutable `VisionPipelineStep`;
3. serialize and hash the Step definition;
4. create a transient one-Step Pipeline with isolated input/output layers;
5. freeze the ordered image list and file identities;
6. execute each image with a new Pipeline/tool instance;
7. retain the result by original image index;
8. build the batch summary and HTML from retained results only.

Report export must never rerun the algorithm.

### Phase 1 supported Tool Views

The existing native registry contains 16 Tool View entries. Thirteen
single-input entries already have an `Add Pipeline` Step adapter and fit the
shared design:

- Threshold
- Filter
- Morphology
- Blob
- Contour
- Line
- Matching
- Edge Based Matching
- Feature Matching
- Edge Detection
- Rotate/Scale
- Affine Transform
- Mean

Template-backed tools must fail closed until a readable frozen template is
available. Affine Transform must fail closed when its point contract is invalid.

### Deferred or separate paths

| Tool/path | Reason |
| --- | --- |
| Arithmetic | It has two input layers. N-image verification needs an explicit A/B pairing policy and must not guess file pairs. |
| HSV | Runtime Pipeline support exists, but the current native HSV Tool View intentionally has no `Add Pipeline` Step adapter. Add the missing deterministic adapter before enabling the common action. |
| Histogram | The current Tool View has no Pipeline Step adapter or formal batch result contract. Define the required metrics first. |
| AutoMPoint | It is a teaching-time selector, not a Pipeline Step. Keep the completed P231 representative-image report. |
| Pipeline-only families such as PinArrayGap, GapEdgePair, CircleGauge, and GeometryMeasure | They already use Recipe Manager/Pipeline N-image execution. A standalone Tool View is a separate product decision. |

The common action must be capability-driven. An unsupported tool hides or
disables it with an exact reason; it must not silently execute a different
algorithm.

## N-Image Versus Parallel Execution

N-image execution means one frozen configuration is replayed over many images.
It does not require simultaneous processing.

Phase 1 remains sequential because current code and evidence establish ordered
`await` loops, not thread-safe shared OpenVisionLab Vision SDK/OpenCvSharp tool instances.
Sequential execution also preserves deterministic progress, stop-after-current,
report ordering, and memory usage.

Bounded parallelism may be added only when all of these pass:

- every worker creates an isolated Pipeline, tool, property, Mat, template, and
  result object;
- no worker reads or writes WPF controls, display layers, repository property
  sessions, or shared mutable static state;
- result ordering is restored by frozen image index;
- sequential and parallel outputs have identical status, metrics, drawings, and
  hashes on the same frozen input set;
- memory and runtime are measured for representative preprocessing, Blob,
  metrology, and template-matching tools;
- cancellation and partial-report behavior remain deterministic.

After that audit, expose only bounded choices such as `1 / 2 / 4 workers`.
Do not default to the CPU count or claim parallel safety from faster execution
alone.

## Generic HTML Report Contract

The report should be built from the saved batch summary and linked per-image
reports:

- tool/Step XML snapshot and SHA-256;
- ordered source paths and SHA-256;
- execution mode and worker count;
- total/success/failure/error counts;
- timing average, nearest-rank p95, and maximum;
- available metric distributions;
- every N-row result;
- deterministic review queue;
- embedded source/result drawings for queued rows;
- exact limitations and whether expected OK/NG labels were supplied.

The AutoMPoint exporter should remain separate initially. Its candidate-ranking
and pose-specific report is not the same result schema as a generic Pipeline
Step. Share code only after real duplication exists.

## Phase Plan And Gates

### Phase 1: shared sequential quick verification

Scope:

- one common `N장 검증` action and shared window;
- thirteen eligible single-input Tool Views;
- transient frozen one-Step Pipeline;
- sequential execution, progress, stop-after-current, table, drawing, and
  self-contained HTML;
- no automatic Preview/Run, layer, or routing mutation.

Gate:

- one shared implementation, not thirteen copied batch implementations;
- Threshold, Blob, Line, and Edge Based Matching reference cases match the same
  one-Step Pipeline results in Recipe Manager;
- 30-row report has 30 rows, matching source hashes, retained drawings, and a
  deterministic bounded review queue;
- changed parameter/image identity invalidates stale results and report export.

### Phase 2: formal handoff and exceptional tools

Scope:

- explicit promotion to a saved Pipeline/Validation Set;
- HSV Step adapter after current-property round-trip proof;
- Arithmetic A/B pairing only after the operator chooses a pairing rule;
- Histogram only after a useful metric/result contract is approved.

Gate:

- promotion preserves the exact Step hash and does not auto-run;
- Recipe Manager replay matches the quick run for the same files and Step;
- unsupported tools remain fail-closed.

P235 completes only the locator-promotion portion of Phase 2. It stores every
row as locator expected success, preserves the exact one-Step Pipeline and
source/dependency identities, and keeps the promoted row list read-only.
HSV/Arithmetic/Histogram remain deferred until their separately listed
prerequisites exist.

### Phase 3: bounded parallel execution

Scope:

- isolated workers and measured `1 / 2 / 4` concurrency;
- deterministic ordered results and partial-save behavior.

Gate:

- exact sequential/parallel result equivalence;
- no shared-state race in tool, template, drawing, report, or WPF state;
- measured speed benefit justifies the added complexity and memory use.

## P233 Phase 1 Implementation Result

Phase 1 now uses one shared implementation rather than per-tool batch panels:

- The shared single-input Tool View shell exposes `N-image verification` only
  when the native Tool View has a current one-Step Pipeline adapter.
- The modal shared window accepts multiple files or one top-level folder, with
  the existing 5,000-image limit and duplicate removal.
- Explicit Run commits and creates the current Step exactly once, hashes its
  serialized definition, and replays a transient one-Step Pipeline in the
  frozen source order.
- Execution is sequential. Stop is honored after the current image so the
  retained partial report remains deterministic.
- The same input-channel normalization used by the native Tool View Preview is
  applied to execution, while the original loaded source is retained and
  SHA-256 verified as visual evidence.
- Each row retains its run report, original source snapshot, result drawing,
  metrics, status, message, and elapsed time. The batch retains `summary.xml`,
  `summary.tsv`, `pipeline.xml`, and the deterministic review queue.
- The self-contained HTML exporter reads only saved summaries/reports/images.
  It embeds the bounded review gallery and never reruns the algorithm.
- Selecting rows, opening/closing the window, and exporting do not invoke the
  Tool View Preview, create layers, select a layer, or change routing.

Verification:

- `Threshold`, `Blob`, `Line`, `Matching`, `EdgeBasedMatching`, and
  `AffineTransform` each replayed 30 images.
- All six tools returned 30/30 successful rows on the deterministic success
  corpus.
- The frozen Step was created once per tool.
- Every retained row had a matching source SHA-256 and drawing.
- Direct replay of the frozen one-Step Pipeline matched the retained status and
  every published metric.
- HTML export preserved run-report modification times, proving no rerun.
- The 30-row EdgeBasedMatching HTML rendered with all 30 table rows and six
  embedded review images; SHA-256 cards wrapped within the viewport with no
  page-level horizontal overflow.
- Current-build UI smoke proved the shared entry and retained-result window, and
  a side-effect smoke retained Preview/Run count, layers, active layer, input
  route, and output route.

Evidence:
`artifacts\p233_tool_view_n_image_verification_20260724`.

Honest boundary: this surface reports execution success/failure and available
metrics. It does not infer expected OK/NG roles, inspection accuracy, semantic
correctness, industrial robustness, or field qualification. Formal labelled
validation remains Recipe Manager Validation Set and Run History. Phase 3
parallel workers remain unimplemented.

## Completion Record

Status: Complete

Scope: P232 design plus the shared sequential P233 Phase 1 Tool View
N-image verification UI, retained evidence, and self-contained HTML report.

Acceptance criteria: One shared eligible-Tool entry; exact once-per-run Step
freeze; up to 5,000 ordered files; sequential execution and stop-after-current;
retained source/report/drawing/metrics; deterministic queue; retained-only HTML;
zero Preview/layer/routing side effects; 30-image reference equivalence for
Threshold, Blob, Line, and EdgeBasedMatching.

Verification: Six representative Tool families x 30 images passed exact
direct-run status/metric equivalence; current-build entry/window/side-effect UI
smokes passed; the 30-row HTML rendered without page overflow; Debug builds
completed with zero warnings and errors.

Evidence: `artifacts\p233_tool_view_n_image_verification_20260724`, this
document, and `docs\OPENVISIONLAB_STABLE_FEATURE_CONTRACTS.md`.

Boundary / next dependency: No OK/NG role is inferred and no concurrent worker
exists. P235 subsequently completed the bounded locator-promotion slice. Any
next product change requires a concrete operator workflow blocker or a measured
sequential bottleneck.

## P234 Real-Folder Acceptance

The first real-folder use of the P233 surface is complete:

- Frozen input: the P230 `Die Pad 1` EdgeBasedMatching template and parameters,
  Step SHA-256
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`.
- Folder input: deterministic MD5-spread 12 OK + 12 NG task-local copies from
  the operator-supplied `EasyMatch_Die_Pad_500(1)` corpus.
- Result: folder registration 24/24, Step creation 1/1, execution 24/24,
  retained drawings 24/24, and zero evidence failures.
- Integration equivalence: retained source SHA-256 and decoded pixels matched;
  all `ScoreMax` values reproduced P230 within `0.068` percentage points under
  the frozen `<=0.1` integration gate.
- Drawing review: the minimum-score and maximum-baseline-delta rows both kept
  the rotated result rectangle and center on the approved central pad/trace.
- Evidence:
  `artifacts\p234_tool_n_image_real_folder_acceptance_20260724`.

This acceptance does not add an OK/NG gate. The role labels only balance the
sample. It does not strengthen P230 beyond its documented same-source limits,
qualify another stratum, or justify parallel execution.

Status: Complete

Boundary / next dependency: There is no active implementation priority. Resume
only for a concrete workflow blocker/current-build regression, or an explicit
parallelism request backed by a measured sequential bottleneck.

## P235 Locator Promotion Result

- The exact retained P234 EdgeBasedMatching Step SHA-256
  `7CEAEC5D50259ED1337AB912F0F0A63C673F4A74E692DCDEA01BAA14FC25658F`
  and 24 source identities were promoted without another matcher run.
- Recipe Manager reloaded 24/24 locator Expected OK rows, the linked Pipeline,
  Step hash, dependency hash, and image-set hash.
- Repeating promotion reused the same set. Selecting a different Pipeline
  disabled execution, image-hash tampering failed closed, and promoted rows
  could not be added, removed, or repaired.
- Promotion preserved the active Pipeline and produced zero automatic
  Preview/Run, layer, or routing changes.
- Legacy unlocked OK/NG Validation Set behavior and the shared N-image
  entry/window side-effect contracts remained green.

Status: Complete

Evidence: `artifacts\p235_n_image_locator_validation_promotion_20260724`.

Boundary: this is a durable locator expected-success contract, not automatic
defect labelling, semantic requalification, concurrency, or field robustness.
