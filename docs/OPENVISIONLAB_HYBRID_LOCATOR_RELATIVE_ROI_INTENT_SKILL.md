# Hybrid Locator -> Relative ROI Inspection-Intent Skill

## Product Decision

OpenVisionLab may offer a bounded hybrid inspection-intent skill in which deterministic location detection establishes a reference pose and a separate deterministic rule-based tool inspects a relative ROI. This is the approved alternative when a fixed raw-image ROI cannot remain on the intended physical target.

This does **not** mean that the LLM detects the target in each production image. The LLM may author a constrained starter XML. Matching, normalization, edge/segmentation tools, acceptance gates, explicit Run, retained drawings, and operator review own execution and evidence.

## Operator-Visible Meaning

The workflow is:

1. The operator teaches one reviewed reference image and a distinctive locator object.
2. `Matching` finds the locator center, angle, and uniform scale in a new image.
3. `RotateScale` with `FIXTURE_APPLY_MODE=NormalizeImage` maps the image back to the reviewed reference coordinate system.
4. The inspection ROI is stored once in that reference coordinate system. It therefore follows the detected part pose without per-image coordinates.
5. A locked rule-based tool such as `LineDistance` inspects only that relative ROI.
6. Missing, weak, ambiguous, out-of-angle, out-of-scale, or low-coverage location evidence fails closed before measurement.

In the first dark-band candidate, “relative ROI” is implemented by normalizing the source to the reference image and then applying the fixed reference ROI. It is not a hidden raw-coordinate offset and it does not mutate the operator's source image or input layer.

## Required Operator Inputs

- inspection intent and physical target;
- reviewed reference image;
- locator template cropped around a distinctive object, with irrelevant background minimized;
- reference center, angle, scale, image width, and image height;
- allowed position/angle/scale variation and ambiguity policy;
- one reference-coordinate inspection ROI;
- target polarity and the locked downstream tool family;
- pixel-only or calibrated-unit boundary;
- tolerance only when supplied and independently owned by the operator;
- Train/Validation/Test or equivalent frozen sample split and semantic gold drawings.

If the locator object is not distinctive across the declared variation, setup is incomplete. The assistant must not compensate by silently widening gates or moving the ROI per image.

## Locked XML Shape For The Dark-Band Pilot

The first candidate uses exactly this ordered family:

1. `Matching`, `NUM_MATCH=2`: ambiguity audit with `ScoreMargin` acceptance.
2. `Matching`, `NUM_MATCH=1`: publish the bounded fixture pose with score, angle, and scale gates.
3. `RotateScale`, `FIXTURE_APPLY_MODE=NormalizeImage`: create a reference-sized normalized layer with a valid-pixel gate.
4. `LineDistance`, `USE_GAP_EDGE_PAIR=true`: select the supported upper/lower boundary pair inside the reviewed reference ROI.

The LLM must not replace this family with Blob, Contour, a raw fixed ROI, per-image coordinates, or an unapproved model detector. It must not invent acceptance tolerances or calibration.

## Required Metrics And Drawings

Location stage:

- `ScoreMax`, `ScoreSecond`, `ScoreMargin`;
- fixture center, angle, angle delta, scale, and scale ratio;
- normalized valid-pixel ratio;
- both candidate rectangles for an ambiguity failure and the selected pose rectangle for a success.

Measurement stage:

- distance minimum, maximum, average, and range;
- selected support ratio, dark contrast, dark coverage, and candidate-pair margin;
- reference ROI, candidate edges, selected upper/lower lines, and sampled measurement segments.

Every declared sample executes, but semantic review uses the deterministic scalable-validation queue. Mechanical execution count alone is not semantic accuracy.

## Fail-Closed Rules

Do not measure when any required location or normalization gate fails. A failed locator is not an NG part classification; it is an inspection-unavailable outcome. Folder names such as `OK` and `NG` are not Gap truth unless the operator explicitly defines them as such for this measurement.

The first candidate also remains measurement-only and pixel-only. It does not classify the dark-band Gap without an operator-owned tolerance, and it does not report physical units without independent calibration evidence.

## P192/P193 Evidence And Current Decision

- Frozen candidate XML: `artifacts\p192_top_right_hybrid_gap_20260722\candidate\top_right_hybrid_gap_candidate.pipeline.xml`.
- P192 ten-row replay: four correct measurements and six fail-closed locator-ambiguity outcomes; all current drawings were reviewed.
- P193 unchanged all-500 replay: 356 measurements, 144 named fail-closed outcomes, and zero missing inputs.
- The deterministic P193 queue contains all 144 failures plus 106 measurement/audit/extreme rows. All 42 contact sheets were opened. The reviewed measurements did not repeat P190's wrong-pass pattern on lower secondary structures.
- Some reviewed successes use only a short supported segment; the minimum observed support ratio is `0.269230769231`. This is retained as a limitation, not promoted as field-ready evidence.

Decision: `Hybrid candidate`. The architecture is approved and materially safer than the general raw-coordinate candidate on this corpus, but the specific small center-joint locator has only `356/500` measurement coverage and remains too ambiguous for product-skill completion.

## P195 Guided Setup And LLM Contract Evidence

P195 completes Phase 1 in Dev without adding an algorithm family:

- Guided Setup now exposes a separate `Locator-aligned Gap (NormalizeImage)` intent while preserving the direct raw-ROI dark-band skill.
- Required inputs are the cropped locator template, search ROI, reviewed reference center/angle/scale/image dimensions, reference-coordinate measurement ROI, score/margin/angle/scale/valid-pixel gates, and the explicit pixel-only/no-judgement boundary.
- Starter generation creates exactly `Matching (NUM_MATCH=2) -> Matching (NUM_MATCH=1 fixture publisher) -> RotateScale (NormalizeImage) -> LineDistance (DarkBandGap)` with the reviewed routes and fail-closed gates.
- The LLM prompt locks that order and forbids Blob, Contour, a raw fixed ROI, per-image coordinates, invented calibration/tolerance, or a model detector.
- Strict validation accepts the generated contract and rejects a changed tool family, changed measurement ROI, or weakened locator margin. Import readiness remains separate from execution, and all runtime drawings remain `WAIT` until the operator explicitly runs the Pipeline.
- Current-source UI and contract evidence is retained in `artifacts\p195_hybrid_relative_roi_phase1_20260722`. This slice does not add new runtime semantic evidence beyond P192/P193 and does not make the present locator product-ready.

## Phase Gates

Phase 1 passes only when Guided Setup can collect the required operator inputs, generate the exact locked starter, reject altered tool families/ROIs/gates, import it, and leave drawings in `WAIT` until explicit Run.

P195 passes this Phase 1 gate. Phase 2 remains bounded by the P192/P193 evidence and its `Hybrid candidate` decision until a more distinctive operator-approved locator is frozen and replayed.

Phase 2 passes only when frozen N-sample evidence retains source/XML/template hashes, location and measurement metrics, same-run drawings, named fail-closed outcomes, and a reviewed deterministic queue with no confirmed repeated wrong-pass group.

Phase 3 passes only after a genuine failed LLM first draft is preserved, corrected from working evidence, frozen, and replayed once on a previously unused held-out split without hidden regressions.

## Setup Checklist

- [ ] The locator is a distinctive physical feature, not merely a convenient dark patch.
- [ ] The template and reference pose are operator reviewed.
- [ ] Search and ambiguity gates are frozen before corpus replay.
- [ ] The inspection ROI is defined in reference coordinates.
- [ ] The downstream tool family and measurement meaning are locked.
- [ ] Failure drawings show why measurement was blocked.
- [ ] Success drawings show the actual physical boundaries used.
- [ ] No per-image tuning or coordinate substitution occurred.
- [ ] Calibration and tolerance claims have independent evidence, or remain excluded.
- [ ] The skill closes as `Keep`, `Keep with documented limits`, `Hybrid candidate`, or `Reject`.
