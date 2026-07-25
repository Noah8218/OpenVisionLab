# OpenVisionLab AI Recipe Automation Policy

Updated: 2026-06-18

## Decision

OpenVisionLab should not move to full automatic AI parameter tuning as the default behavior.

The recommended policy is assisted automation:

1. Apply safe mechanical fixes automatically when the user confirms them.
2. Suggest parameter changes with before/after reasons.
3. Keep acceptance loosening, defect criteria, and final OK/NG judgment under operator control.

## Why Full Automatic Tuning Is Risky

Full automatic tuning can make a bad recipe look successful by weakening the inspection.

Examples:

- Lowering `ResultCount` minimum can hide missed detections.
- Increasing area tolerance too much can accept noise as a valid object.
- Switching a branch input back to `Main` can make a chained pipeline look OK while skipping preprocessing.
- Relaxing matching score can report the wrong template location as a match.

For a vision validation platform, the user must be able to explain why the recipe is OK.
Automatic changes that only chase a passing result reduce traceability.

## Allowed Automatic Fixes

These changes are safe enough to offer as checked suggestions:

- Clamp threshold values to valid gray-value ranges.
- Swap invalid min/max ordering.
- Normalize odd/even kernel or block-size requirements.
- Prevent zero or negative scale, pixel/mm, sampling count, and timeout values.
- Fix obvious layer-flow mistakes when the previous output is the intended input.
- Reject unsupported ToolType before import.
- Add missing output layer names when they can be generated without changing inspection meaning.

## Operator Confirmation Required

These changes must stay manual or require explicit confirmation:

- Acceptance min/max changes.
- Matching score threshold relaxation.
- Contour/Blob area range widening.
- ROI expansion or relocation.
- Branch input changes when more than one valid source layer exists.
- Any change that turns an NG sample into OK without a paired Good/Bad validation.

## UX Direction

The AI Recipe screen should show suggestions as a review list:

| Suggestion Type | Default | Reason |
| --- | --- | --- |
| Invalid parameter correction | Checked | Mechanical correction with low inspection-risk |
| Layer flow correction | Checked only when one source is unambiguous | Prevents accidental `Main` branch without hiding intent |
| Acceptance loosen/tighten | Unchecked | Can hide real NG |
| ROI correction | Unchecked | Requires visual/operator judgment |
| Matching score change | Unchecked | Can produce false positive matches |

## Completion Criteria

- The user can apply safe fixes without editing XML.
- Every applied fix is logged as Step/Parameter/Layer Flow change.
- Manual-risk fixes are visible but not auto-applied by default.
- The final Preview report shows which AI suggestions were applied.
- Good/Bad sample pairs are used before recommending acceptance changes.
