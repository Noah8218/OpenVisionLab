# OpenVisionLab EdgeBasedMatching Parameter Guide

Date: 2026-07-30 KST  
Priority: P268  
Status: Complete

## Outcome

The 32 formerly Basic `EdgeBasedMatching` entries now have runtime-grounded
Korean/English detailed guidance. `EdgeBasedMatching` is 65/65 detailed and
the standalone canonical audit is 318/318 detailed with zero Basic entries.

The audit and guidance keep four responsibilities separate:

1. registered template identity, global polarity, and result-image display;
2. explicit Auto MPoint teaching-time candidate analysis;
3. Canny/contour edge-model construction;
4. coarse/refine/pyramid/hybrid runtime search.

The audit also exposed a Recipe Manager selected-Step round-trip defect.
Existing runtime-active EdgeBasedMatching scale, subpixel, and pyramid values
were written to Pipeline XML but were not all restored into the selected-Step
PropertyGrid. The mapper now restores and reapplies those values exactly.

## Runtime-Grounded Semantics

### Template identity and polarity

`PATTERN_PATH` selects the physical feature and the directed edge model. A
rotated ROI template is stored upright at 0 degrees. A high score, uniqueness
margin, or Auto MPoint rank does not prove that the detected location is the
same durable physical feature; the operator must review the outline, center,
strongest alternative, and representative samples.

`ALLOW_GLOBAL_POLARITY_REVERSAL` compares exactly two whole-candidate
orientations: every taught edge direction unchanged or every direction
globally reversed. It never ignores polarity independently per edge.

`USE_DRAW_IMAGE` is a limited result-bitmap option, not an inspection
algorithm control. Successful candidates still generate matching evidence
when it is off, and current WPF/Pipeline Review retains mandatory evidence.
Its practical effect is primarily result-bitmap preparation when no candidate
exists.

### Auto MPoint teaching

The 11 `AUTO_MPOINT_*` fields apply only to explicit `Analyze candidates`.
They do not run inspection Preview/Run, save a template, mutate a Recipe,
change layers, or alter routing. Results remain `Suggested`, not `Qualified`;
the operator must explicitly select `Use this pattern`.

Guidance now distinguishes:

- reviewed analysis ROI from inspection Search ROI;
- fixed candidate window width/height and grid stride;
- exposed suggestion count from qualification;
- feature quality from physical identity;
- teaching-time uniqueness from runtime unique-match validation;
- synthetic position error from real production accuracy;
- representative image count/rate from field qualification.

### Edge-model construction

The guide states that:

- Canny aperture is normalized to 3, 5, or 7;
- L2 gradient changes model construction rather than the acceptance gate;
- retrieval and approximation modes change the contour points used by the
  model;
- `MAX_TEMPLATE_POINTS` caps the final scoring points;
- `MIN_GRADIENT_MAGNITUDE` removes weak gradients from template and source
  scoring.

Operators are told to review the actual model outline and raw/used point
counts, not infer behavior from parameter text alone.

### Search and verification

- Coarse-to-fine angle search requires angle search and a coarse step greater
  than the fine step.
- Position refine re-scores neighborhoods at one-pixel intervals; subpixel
  refine fits the local 3x3 score peak and does not change `SCORE_MIN`.
- Greediness is an early-abandonment performance parameter, not an acceptance
  threshold.
- Pyramid proposal uses half-scale positions and original-resolution
  verification, falls back when weak, and is bypassed during scale search.
- Hybrid verification re-ranks top edge candidates with image similarity,
  while the public result score remains the edge score.

## Recipe Round-Trip Correction

The selected-Step PropertyGrid mapper now restores:

- `USE_FIND_SCALE`, `FIND_SCALE_MIN/MAX/STEP`;
- `USE_SUBPIXEL_REFINE`;
- `USE_PYRAMID_POSITION_PROPOSAL`,
  `PYRAMID_POSITION_TOP_N`,
  `PYRAMID_POSITION_MIN_SCORE`.

Focused create -> PropertyGrid -> apply -> reload evidence preserved the exact
configured values without Preview/Run, layer, active-layer, or route changes.
Auto MPoint remains a Direct Tool teaching-time workflow and is not presented
as an inspection runtime gate.

## Actual EXE Evidence

The same current Debug EXE and `920 x 660` Edge Based Matching Tool were used
for before and after:

- Before: `PATTERN_PATH` showed Basic fallback guidance.
- After: the same row explains model identity, physical-feature risk, and
  evidence to review.
- `ObstructedControls: None`
- `AutomaticShowFocusRetained: True`
- `ExplicitHideReopen: PASS`
- `PreviewRunCount: 0`
- `LayerCount: 0`

Evidence:

- `artifacts\p268_edge_based_matching_parameter_guide_20260730\actual_exe_before`
- `artifacts\p268_edge_based_matching_parameter_guide_20260730\actual_exe_after`

The sidecar remains beside the Tool and does not cover the PropertyGrid, input
or output viewers, layer selectors, Pipeline actions, or explicit Preview.

## Verification

- P268 focused smoke: `AuditedProperties=32/32`
- EdgeBasedMatching detailed coverage: `65/65`
- Recipe scale/subpixel/pyramid apply/reload: passed
- Conditional applicability for Auto MPoint ROI, coarse angle, pyramid, and
  hybrid fields: passed
- Direct Tool Korean/English selection and zero-side-effect checks: passed
- Shared P257 fallback/detailed guide regression: passed
- P266 inactive/legacy Line and P267 Affine guide regressions: passed
- Current Edge Based Matching Tool explicit Preview regression: passed
- Current Auto MPoint explicit analysis/apply regression: passed
- Global polarity runtime contract: 20/20 passed
- Localization catalog contract: passed
- Standalone canonical audit:
  - Browsable: `318`
  - Detailed: `318`
  - Basic fallback: `0`
- Full Debug build: zero warnings, zero errors

Evidence:

`artifacts\p268_edge_based_matching_parameter_guide_20260730`

## Next Bounded Priority

Run a static post-guide usability reassessment before selecting another
feature. The Parameter Guide backlog is closed at 318/318, so do not invent
another algorithm or parameter surface merely to continue development. Admit
implementation only if current-source operator workflow evidence exposes a
specific blocker or regression.

Recommended model: `gpt-5.6-terra` | Reasoning effort: `low`.

CVR-00 remains deferred until three independent first-time participants and
their raw observations exist.

## Completion Record

Status: Complete  
Scope: Runtime-grounded bilingual detailed guidance for all 32 formerly Basic
EdgeBasedMatching template/display, Auto MPoint teaching, edge-model, and
runtime-search entries, plus exact Recipe selected-Step restoration of active
scale, subpixel, and pyramid values.  
Acceptance criteria: 32/32 audited; EdgeBasedMatching 65/65 and canonical
318/318 detailed; physical identity is not inferred from score/margin/rank;
Auto MPoint remains explicit Suggested teaching; display/model/search
boundaries are stated; selected-Step apply/reload preserves active runtime
values; actual EXE remains non-obstructing; zero Preview/Run/layer/route side
effects.  
Verification: Focused/shared guide, Direct Tool, Auto MPoint, localization,
Recipe round-trip, global-polarity runtime, actual Debug EXE before/after,
standalone canonical audit, and full Debug build.  
Evidence:
`artifacts\p268_edge_based_matching_parameter_guide_20260730` and this report.  
Boundary / next dependency: This is guidance and persistence correction for
the existing deterministic matcher. It is not automatic physical-feature
selection, matcher qualification, new dataset evidence, calibration,
certified metrology, unseen-data robustness, field qualification, equipment
integration, LLM expansion, or CVR-00 participant evidence.
