# OpenVisionLab Edge Global Polarity v1 Contract

Updated: 2026-07-28 KST

## Status And Scope

CVR-11 v1 adds one opt-in EdgeBasedMatching behavior for a single taught
feature whose complete edge-gradient direction reverses with global contrast.
It does not ignore polarity independently at each edge.

The only new recipe parameter is:

```xml
<Parameter>
  <Key>ALLOW_GLOBAL_POLARITY_REVERSAL</Key>
  <Value>true</Value>
</Parameter>
```

Missing keys and `false` preserve the legacy Same-only matcher.

## Runtime Contract

- Same-only mode uses the unchanged signed gradient-direction score.
- Opt-in mode compares the same full candidate under exactly two states:
  taught direction and one global reversal.
- The higher valid global state supplies the existing Edge score.
- A successful result records `PolarityReversed=false` for Same and `true` for
  Reversed.
- Metrics publish `GlobalPolarity.AllowReversal`,
  single-result `GlobalPolarity.Reversed`, and exact
  `GlobalPolarity.SameCount`/`ReversedCount`.
- Result drawings label the selected state as `Same` or `Reversed`.
- Score, unique-match, search ROI, angle, scale, suppression, and result-count
  gates remain in force.
- A no-target input still fails with no MatchingResult.

This is not local polarity ignore, automatic mode selection, adaptive
thresholding, or an instruction to enable reversal by default.
Opt-in scoring evaluates all retained model points before selecting the global
sign, so operators must include runtime in their task-specific N-sample review.

## Product Contract

- Tool View and selected-Step PropertyGrid expose `Allow global polarity
  reversal`.
- Editing the option does not invoke Preview or Run.
- Pipeline/XML and Recipe Manager round-trip the exact Boolean value.
- Pipeline validation rejects a present value that is not `true` or `false`.
- Missing XML restores `false`.
- Existing Run Report metric persistence retains the two numeric polarity
  metrics without a separate report format.

## Evidence Gate

The bounded project-authored synthetic matrix freezes one asymmetric feature,
one template, one full-image search region, and one score/search configuration:

- Train: 4 Same + 4 Reversed targets;
- Validation: 2 Same + 2 Reversed targets + 2 no-target rows;
- Held-out: 2 Same + 2 Reversed targets + 2 no-target rows.

All 20 rows must match their labelled outcome, every successful target must
report the correct global state, center error must be at most 2 px, and all
no-target rows must reject. One separate legacy reversed probe must reject.

## Boundary

The evidence is deterministic project-authored synthetic evidence. It does not
prove that a real physical feature changes polarity, local mixed-polarity
robustness, illumination robustness, deformation tolerance, production
accuracy, or field qualification. A physical task must supply its own labelled
N-sample and held-out evidence before enabling this option in a qualified
recipe.

## Completion Record

```text
Status: Complete
Scope: Opt-in whole-candidate global edge-polarity reversal with legacy Same-only default, exact state metrics/drawing, and Product XML/PropertyGrid integration.
Acceptance criteria: Legacy reversed probe rejects; 20/20 Train/Validation/Held-out rows match labels; successful rows report exact Same/Reversed state and <=2 px center error; no-target rows reject; PropertyGrid/XML round trip and no-auto-Preview contract pass.
Verification: OpenVisionLab Vision SDK Release build and inspection smoke; OpenVisionLab Debug build; VisionRecipeRunnerSmoke --edge-global-polarity-contract; current-source EdgeBasedMatching Tool View smoke.
Evidence: artifacts/cvr11_global_polarity_20260728, docs/reports/OPENVISIONLAB_CVR11_GLOBAL_POLARITY_20260728.md
Boundary / next dependency: Synthetic global reversal only. Physical qualification needs a named feature, labelled representative captures, frozen settings, and held-out replay.
```
