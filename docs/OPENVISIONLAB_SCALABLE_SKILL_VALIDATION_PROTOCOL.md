# Scalable Inspection-Intent Skill Validation Protocol

Updated: 2026-07-22 KST  
Status: Current operating contract

## Decision

OpenVisionLab keeps the bounded inspection-intent skill concept and rejects an unbounded `arbitrary image + prompt -> autonomously correct inspection` promise.

An LLM authors or corrects a constrained starter recipe. The deterministic OpenVisionLab runtime executes every declared image. The operator defines the physical target, reviewed ROI or measurement region, units, labels/tolerance when judgement is claimed, and a small semantic ground-truth set. Neither a high execution count nor an LLM visual opinion is ground truth.

Manual image-by-image feedback is allowed only to establish the initial physical meaning, approve the small gold set, or classify a genuinely new repeated failure group. It is not the normal large-corpus workflow.

## Scalable workflow

1. **Freeze the skill contract.** Record intent, required operator inputs, locked tool family, XML hash, parameter values, calibration boundary, expected drawings, and completion gates.
2. **Freeze corpus identity.** Hash every source, remove byte-identical duplicates from evidence counts, keep related/generated variants in the same split, and record the split rule before execution.
3. **Approve a small semantic gold set.** It must contain ordinary, boundary, shifted/difficult, and expected-failure examples. The operator records the intended geometry, acceptable measurement range, expected reject, or OK/NG truth needed by the skill. No fixed image count proves every use case.
4. **Execute the complete corpus.** Every image produces a result row, named fail-closed reason when applicable, source hash, XML identity, metrics, and current-run drawing. Running 10,000 images does not require manually opening 10,000 drawings.
5. **Build a deterministic review queue.** Before reviewing results, retain all runtime failures, all labelled misclassifications, all named fail-closed groups, lowest-confidence/lowest-support rows, measurement extremes and ranges, one or more representatives per declared stratum, and a content-hash-seeded random sample. Record the selection rule and selected-row list/hash so the queue cannot be silently cherry-picked.
6. **Review representative drawings.** Open every queued overlay. Group repeated failures by physical cause. A CSV count without drawings cannot establish semantic correctness.
7. **Correct at most twice.** Give the LLM a compact packet containing the frozen contract, exact failed XML, metrics, and representative failure drawings. Change one bounded rule for one repeated cause, rerun the full working corpus, and report regressions. Never tune per image.
8. **Freeze and replay held-out evidence once.** After a candidate is frozen, run the previously unused Test split once. Do not inspect or tune against it early.
9. **Close with one decision.** Mark the skill `Keep`, `Keep with documented limits`, `Hybrid candidate`, or `Reject`. Do not keep an inconclusive skill open through endless prompt or parameter retries.

## Review-queue minimum

The queue policy is skill-specific, but it must include these categories when available:

| Category | Required selection |
|---|---|
| Runtime integrity | Every load/runtime error and every named fail-closed outcome |
| Label evidence | Every false accept and false reject when labels and a valid acceptance gate exist |
| Semantic-risk metrics | Lowest ambiguity margin, support, contrast, or coverage rows used by the skill |
| Measurement extremes | Minimum, maximum, widest range, and other declared outlier metrics |
| Dataset variation | Deterministic representatives from each direction, folder, label, acquisition group, or other declared stratum |
| Unbiased audit | Content-hash-seeded random rows selected by a rule frozen before visual review |

The review budget is recorded before the run. Increase it when the corpus is diverse or the cost of a wrong pass is high. As a rough independent-random-sample planning aid only, zero observed failures in 100 reviewed rows still permits an approximate 95% upper error bound near 3%; 300 zero-failure rows approach 1%. Correlated or synthetic variants weaken that interpretation, so stratification and held-out acquisition evidence remain necessary.

## Go / no-go rules

Keep a rule-based skill only when:

- one unchanged recipe covers the declared variation without per-image ROI or parameter changes;
- the reviewed gold and held-out drawings select the intended physical geometry;
- known wrong geometry is not reported as a successful measurement;
- unsupported cases fail closed with a named reason;
- any OK/NG or physical-unit claim has operator-approved truth and calibration.

Stop or change direction when:

- the same semantic wrong-pass remains after two bounded correction cycles;
- individual images require individual tuning;
- the existing deterministic tool family cannot express the physical intent;
- no operator-owned truth exists for the claimed judgement; or
- target localization/segmentation requires learned semantic variation rather than bounded geometry.

In the last case, consider a separately approved hybrid where a trained locator or segmenter supplies a region and deterministic tools perform the measurement. Do not use an LLM as the per-image production detector.

## Required completion record

```text
Status: Complete | Incomplete | Blocked
Decision: Keep | Keep with documented limits | Hybrid candidate | Reject
Skill/version: <name/version>
Frozen identity: <XML hash, source manifest hash, split rule/hash, build identity>
Corpus: <total, unique, duplicates, strata, Train/Validation/Test counts>
Execution: <completed, measurement, fail-closed, load/runtime error counts>
Review queue: <selection policy/hash, selected count, reviewed count>
Semantic result: <correct geometry, wrong geometry, ambiguous, operator decision needed>
Correction cycles: <0, 1, or 2; exact changed rule and regression result>
Verification: <commands and results>
Evidence: <CSV, drawings, contact sheets, logs, report>
Boundary / next dependency: <what is not proved>
```

## First applied case: P190

P190 applied this protocol to the direct dark-band Gap skill on 500 unique `device_top_right` images. The frozen baseline queue exposed repeated semantic wrong passes despite 448 mechanical measurements. One bounded support correction increased fail-closed outcomes from 52 to 171 but did not remove wrong passes on long lower structures. The audit closed as `Keep with documented limits`; per-image tuning and a second unsupported numeric cycle were not performed. Evidence is `artifacts\p190_dark_band_gap_full_corpus_20260722`.

## Product integration: P191

P191 places a generic deterministic subset of this protocol inside the existing saved batch summary and Run History workflow. New runs freeze the v1 policy, queue SHA-256, result indices, and reasons; the operator can filter to queued rows and open the retained current-run drawing without another execution. The generic queue covers runtime failures, labelled misclassifications, evidence gaps, varying Step-metric extrema, and three content-hash-ordered audit rows per declared role stratum. Skill-specific contracts may require additional semantic-risk metrics or larger audit budgets; P191 does not replace the frozen skill contract, operator gold set, or held-out replay.
