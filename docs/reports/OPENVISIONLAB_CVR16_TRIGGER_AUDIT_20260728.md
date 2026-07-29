# OpenVisionLab CVR-16 Activation Audit

Date: 2026-07-28  
Queue item: `CVR-16` — additional per-object shape descriptor  
Decision: **Not admitted — named object-separation task absent**

## 1. Question Audited

Should OpenVisionLab add aspect ratio, circularity, orientation, rotated
width/height, hole count, or gray-value filtering to Blob or Contour now?

Commercial region evaluators expose many such values. OpenVisionLab should add
only one when a named inspection proves that the current per-object
Area/Width/Height contract cannot separate the required OK and NG objects.
Adding a descriptor because it exists in a commercial menu would create
parameter surface without an operator-owned semantic gate.

## 2. Existing Responsibilities

| Existing path | Current behavior | Boundary |
| --- | --- | --- |
| Blob and Contour PropertyGrid/XML | Per-object `MIN/MAX_AREA`, axis-aligned `MIN/MAX_WIDTH`, and `MIN/MAX_HEIGHT` | No aspect, circularity, orientation, rotated-size, holes, or gray gate |
| Runtime object filtering | Applies Area/Width/Height before `ResultCount`, accepted metrics, and accepted drawings | Rejects with the first exact failed gate |
| Object Results Inspector | Retains accepted/rejected object rows with stable number, area, center, bounds, angle, and reject reason | Displayed `Angle` is evidence, not an individual acceptance parameter |
| Object metric distribution | Plots Area, BoundsWidth, or BoundsHeight for accepted/rejected rows | It does not define another descriptor or select a gate |
| Aggregate Step metrics | Publishes Area, Angle, BoundsWidth, and BoundsHeight min/max/average for accepted objects | A whole-Step angle metric is not equivalent to removing one wrong object |
| Saved Run Report | Round-trips the current object rows and reject reasons | New descriptor provenance and compatibility would need an explicit contract |

P215 selected Width/Height only after a rail fragment passed Area and created a
concrete per-object need. P216 completed that bounded slice. P217 then found no
remaining named operator task and closed proactive descriptor expansion.

## 3. Evidence Inventory Result

The repository contains:

- P211 selectable Blob/Contour object rows and saved evidence;
- P216 deterministic Area/Width/Height filter and compatibility evidence;
- historical segmentation cases where Area alone was insufficient;
- commercial reference material listing richer region descriptors;
- aggregate object angle metrics and a displayed per-object angle.

It does not contain a current packet with:

- one named part, segmented object, and inspection decision;
- OK and NG objects that both pass the same frozen Area/Width/Height ranges;
- a reviewed causal claim that one specific additional descriptor separates
  them;
- stable object identity under the task's segmentation settings;
- Train, Validation, and untouched Held-out rows;
- an operator tolerance and false-accept/false-reject gate.

The presence of an `Angle` column does not activate an orientation filter. Its
physical meaning, periodicity, and stability have not been defined for a named
task. No current source parameter implements any other CVR-16 candidate.

## 4. CVR-16 Admission Packet

Reopen CVR-16 only when one packet contains all six sections below.

### A. Named Object Task

- part/product and inspection name;
- exact Blob or Contour object that represents one physical feature;
- required per-object decision and downstream use of `ResultCount`;
- frozen input layer, ROI, threshold/segmentation, connectivity, and
  morphology;
- explicit reason a fixed ROI, Area, Width, Height, or an existing geometric
  measurement cannot express the task.

### B. Frozen Current-Path Failure

With one unchanged recipe retain:

- source and Pipeline SHA-256;
- every accepted/rejected object row and drawing;
- identical Area/Width/Height gates for every row;
- at least one wrong object that passes all current gates, or one required
  object that cannot be retained without admitting a wrong object;
- evidence that the cause is shape semantics rather than threshold, merge,
  split, border crop, ROI, morphology, or labelling error.

Aggregate image classification or total count alone is insufficient. The
failing physical object must be identified one-to-one.

### C. One Descriptor Mathematical Contract

Select exactly one descriptor and define it before implementation:

- **aspect ratio:** axis-aligned or rotated dimensions, numerator ordering,
  zero-size behavior, and scale/rotation expectations;
- **circularity:** exact formula, perimeter estimator, contour approximation,
  hole treatment, and valid range;
- **orientation:** source geometry, axis direction, `180°` periodicity,
  width/height swaps, and near-symmetric/degenerate behavior;
- **rotated width/height:** minimum-area rectangle convention, ordered sides,
  angle coupling, and tie behavior;
- **hole count:** foreground/background connectivity, hierarchy, border
  contact, minimum hole size, and nested-hole behavior;
- **gray statistic:** exact source layer/channel, object mask, statistic,
  valid-pixel rule, and whether preprocessing or the original image owns the
  intensity.

Do not combine several descriptors into a general feature expression for v1.

### D. Object And Filter Semantics

Define:

- whether the descriptor applies to Blob, Contour, or one family only;
- when it is computed relative to Area/Width/Height filtering;
- exact min/max PropertyGrid and XML keys;
- missing-key defaults that preserve current results;
- non-finite/undefined behavior and exact reject reason;
- whether rejected near-filter candidates remain bounded by the existing P211
  Contour audit rule;
- whether aggregate min/max/average metrics or a distribution view are
  genuinely needed by the task.

`ResultCount`, accepted drawings, object numbering, table/drawing selection,
and Run Report identity must stay consistent after filtering.

### E. Nuisance And Stability Matrix

Freeze cases covering the nuisances relevant to the selected descriptor:

- segmentation threshold and morphology variation inside the approved range;
- target rotation and scale;
- boundary pixel noise, small protrusions, and contour simplification;
- touching or split objects;
- border contact and partial crop;
- wrong objects close to the gate;
- no-object and excessive-object cases;
- deterministic ordering and runtime budget.

The packet must distinguish descriptor instability from actual physical
variation before a tolerance is chosen.

### F. Split And Completion Gate

- separate Train, Validation, and untouched Held-out rows before implementation;
- source, configuration, labels, and object-identity hashes;
- one pre-frozen descriptor range derived from Train only;
- zero runtime/integrity errors;
- task-defined false-accept and false-reject limits on Validation and Held-out;
- reviewed current-run drawings for every failure and a deterministic boundary
  queue;
- PropertyGrid/XML/save/reload/report round trip;
- missing XML keys reproduce legacy Area/Width/Height results;
- edits and row selection never trigger Preview/Run or mutate layers/routing.

## 5. First Admitted Implementation Boundary

If the packet passes, the first implementation is limited to:

- one descriptor;
- one named Blob or Contour task;
- one exact mathematical definition and finite validated range;
- one backward-compatible PropertyGrid/XML mapping;
- exact per-object value, accepted/rejected state, and reason in existing rows,
  drawings, and reports;
- one frozen Train/Validation/Held-out matrix.

It does not authorize a MERLIC-style full region-feature evaluator, arbitrary
feature expressions, automatic descriptor/gate selection, semantic
classification, OCR/barcode, region algebra, machine learning, another
dataset campaign, or commercial-parity claims.

## 6. Decision

CVR-16 is not admitted. No Blob/Contour runtime, PropertyGrid, XML, metric,
report, sample, or UI change is justified by current evidence. The existing
Area/Width/Height filter and object evidence remain the supported contract.

## 7. Verification

Commands run:

```powershell
dotnet run --no-build --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev
git diff --check
```

Additional static checks verified:

- all six admission sections and the durable completion record exist;
- the canonical queue marks CVR-16 audited-not-admitted;
- the continuation rule advances to CVR-17 rather than implementation;
- current source has no Blob/Contour aspect, circularity, orientation,
  rotated-size, hole-count, or gray-statistic acceptance parameter.

Observed results:

- readiness: all 12 contract categories passed;
- CVR-16 audit structure: passed;
- queue advancement to CVR-17: passed;
- `git diff --check`: passed with line-ending warnings only.

Reopen command:

```text
Audit this named object-separation packet against
docs/reports/OPENVISIONLAB_CVR16_TRIGGER_AUDIT_20260728.md.
Do not implement CVR-16 unless all six sections pass.
```

```text
Status: Complete
Scope: Read-only CVR-16 activation audit and reusable single-descriptor admission contract.
Acceptance criteria: Existing Area/Width/Height filtering, displayed/aggregate Angle evidence, object rows/distributions, and report responsibilities are separated; the missing named-task evidence and six-section admission packet are recorded.
Verification: Current Blob/Contour properties, Pipeline mapping/filtering, object result/report/UI paths, P211/P215/P216/P217 evidence, commercial-video backlog, and current handoff were cross-checked; the commands and observed results above passed.
Evidence: docs/reports/OPENVISIONLAB_CVR16_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: Implementation requires one complete named object-separation packet proving a causal current Area/Width/Height failure and defining one stable descriptor with frozen split evidence.
```
