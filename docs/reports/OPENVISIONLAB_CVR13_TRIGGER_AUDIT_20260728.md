# OpenVisionLab CVR-13 Activation Audit

Date: 2026-07-28  
Queue item: `CVR-13` — anisotropic X/Y matcher scale search  
Decision: **Not admitted — task packet absent**

## 1. Question Audited

Should OpenVisionLab add opt-in independent X/Y scale search to
`EdgeBasedMatching` now?

The commercial-video review identifies non-uniform scale search as a useful
matcher capability. That reference does not prove that a current
OpenVisionLab inspection needs it. This audit therefore separates three
different problems before any algorithm, XML, or UI work is selected.

| Observed change | Existing owner | CVR-13 decision |
| --- | --- | --- |
| The same rigid target changes by one scalar magnification | Current Matching/EdgeBasedMatching `USE_FIND_SCALE` and `FIND_SCALE_MIN/MAX/STEP` | Not CVR-13 |
| The complete image frame can be normalized from three stable corresponding Points | Existing `AffineTransform`, including P219 typed Point x3 binding and fixed downstream reference ROI | Not CVR-13 |
| A target must itself be located while its X and Y scales vary independently within certified finite bounds | No current matcher contract | Possible CVR-13 trigger |
| Local bending, elastic deformation, or independently moving sub-features | CVR-12 or a separately named task | Not CVR-13 |
| Perspective, lens distortion, or camera calibration | Separate product decision | Not CVR-13 |
| A fixed preprocessing resize chosen by the recipe author | Existing `RotateScale` `ScaleXPercent/ScaleYPercent` | Not CVR-13 |

## 2. Current-Source Evidence

- `MatchingProperty` and `EdgeBasedMatchingProperty` expose one scalar search
  dimension through `USE_FIND_SCALE` and `FIND_SCALE_MIN/MAX/STEP`.
- Their current validation and Tool View contracts do not expose independent
  searched X and Y scale ranges.
- `RotateScale` accepts independently authored `ScaleXPercent` and
  `ScaleYPercent`, but it does not estimate an unknown target pose or scale.
- The existing three-point `AffineTransform` publishes `AffineScaleX`,
  `AffineScaleY`, shear, rotation, translation, and the authoritative six
  matrix coefficients.
- P219 already binds three earlier deterministic Point results to that Affine
  transform and preserves a fixed downstream reference-coordinate ROI.

These facts prove that CVR-13 is a distinct missing matcher capability. They do
not prove that a current operator task needs it.

## 3. Evidence Inventory Result

The repository currently contains:

- commercial narration that lists uniform/non-uniform scale controls;
- synthetic/public rigid Matching and EdgeBasedMatching examples;
- a known-matrix Affine sample and one typed-Point Affine fixture workflow;
- an authored `RotateScale` wide-negative sample that tests output geometry;
- matcher evidence focused on identity, pose, search ROI, ambiguity, polarity,
  and locator stability.

It does not contain a named inspection in which:

- the same target's independently measured X/Y scale changes are the causal
  source of current matcher failure;
- the current uniform-scale matcher is replayed with frozen settings;
- the existing Affine normalization path is shown to be unavailable or
  unsuitable for a documented reason;
- acceptable X/Y scale ranges and localization error are operator-certified;
- Train, Validation, and Held-out rows are separated and hash-frozen.

The `Geometry_RotateScale_Synthetic_Wide_NG` asset is not admission evidence.
It is an authored preprocessing/output-size contract, not an unknown
anisotropically scaled target that must be found.

## 4. CVR-13 Admission Packet

Reopen CVR-13 only when one packet contains all six sections below.

### A. Named Task And Physical Identity

- part/product and inspection name;
- exact physical feature to locate;
- reference template image and template ROI;
- downstream inspection or fixture consumer that needs the located pose.

### B. Coordinate And Transform Contract

- source image dimensions and coordinate layer;
- whether the expected change is target-local or whole-frame;
- operator-certified reference width/height or landmark distances;
- exact definition of `ScaleX` and `ScaleY`;
- explicit statement that perspective, lens distortion, local deformation,
  and crop are not being relabelled as anisotropic scale.

### C. Labelled Numeric Truth

For every target row retain:

- source SHA-256 and role;
- target center and angle;
- independent ground-truth `ScaleX` and `ScaleY`;
- permitted center, angle, X-scale, and Y-scale error;
- expected success/failure.

The labels must come from certified geometry, fixture metadata, or reviewed
landmarks. Matcher output cannot be reused as its own ground truth.

### D. Frozen Existing-Path Comparison

Run without result-dependent tuning:

1. current uniform-scale Matching/EdgeBasedMatching;
2. existing three-Point Affine normalization when stable corresponding Points
   are available;
3. fixed `RotateScale` only when the scale is already known before matching.

Record score/state, center/angle/scale error, runtime, and current-run drawings.
The packet must identify why both the uniform matcher and applicable Affine
normalization fail or are structurally unsuitable.

### E. Nuisance Controls And Finite Search Budget

- freeze template ROI, search ROI, polarity, angle range, threshold/Canny,
  blur/noise bounds, occlusion/crop policy, and uniqueness gate;
- include nominal, X-only, Y-only, combined X/Y, wrong-target, and no-target
  rows;
- define finite `ScaleXMin/Max/Step` and `ScaleYMin/Max/Step`;
- define a runtime budget and maximum candidate-grid size before
  implementation.

### F. Split And Completion Gate

- separate Train, Validation, and untouched Held-out images before feature
  implementation;
- retain source/template/configuration hashes and every final drawing;
- require exact target identity plus bounded center/angle/X/Y scale error;
- require wrong/no-target rejection and no regression of missing-key legacy
  behavior;
- require PropertyGrid/XML/Pipeline/Run Report round trip and explicit
  Preview/Run isolation if implementation is admitted.

## 5. First Admitted Implementation Boundary

If the packet passes, the first implementation is limited to:

- one opt-in independent X/Y scale search for the existing edge matcher;
- finite validated X and Y ranges/steps;
- missing XML keys preserving the current single-scale behavior;
- exact selected `ScaleX`/`ScaleY`, score, state, runtime, and alternative
  evidence in drawings/metrics/reports;
- the packet's one frozen task and no automatic default change.

It does not authorize deformation, homography, perspective correction,
calibration, adaptive parameter growth, automatic template selection, or
commercial-parity claims.

## 6. Decision

CVR-13 is not admitted. No matcher, DLL, PropertyGrid, XML, sample, or UI
change is justified by the current evidence. Creating a convenient synthetic
anisotropic match set solely to make the feature pass would not satisfy the
trigger.

Reopen command:

```text
Audit this named anisotropic-scale packet against
docs/reports/OPENVISIONLAB_CVR13_TRIGGER_AUDIT_20260728.md.
Do not implement CVR-13 unless all six sections pass.
```

```text
Status: Complete
Scope: Read-only CVR-13 activation audit and reusable admission contract.
Acceptance criteria: Existing uniform-scale, authored RotateScale, three-Point Affine, deformation, and perspective/calibration responsibilities are separated; the current evidence gap and six-section admission packet are recorded.
Verification: Current source/property/schema searches, Affine v1 and detected-Point fixture contracts, commercial-video review, backlog, and current handoff were cross-checked; documentation readiness and diff checks are recorded with this turn.
Evidence: docs/reports/OPENVISIONLAB_CVR13_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: Implementation requires one complete named physical-task packet proving independent X/Y target scale is causal and that uniform-scale and applicable Affine normalization paths are insufficient.
```
