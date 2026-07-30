# OpenVisionLab P260 EdgeDetection Parameter Guide

Date: 2026-07-30 KST  
Status: Complete

## Outcome

The current canonical Tool-property audit inspected 318 browsable properties.
Before P260, 225 had detailed Parameter Guide content and 93 used the visible
Basic fallback. `EdgeDetection` was selected as the next bounded family because
all 11 of its properties used Basic fallback and incorrect threshold,
derivative, or kernel choices can directly destabilize the downstream
Contour/Line workflow.

P260 adds detailed Korean/English guidance for all 11 `EdgeDetection`
properties and connects the runtime-generated EdgeDetection parameter cards to
the existing shared in-Tool Parameter Guide. The post-change audit reports
236/318 detailed and 82 Basic fallback properties.

## Included Scope

- `EdgeType`
- Canny:
  `CannyThresholdLow`, `CannyThresholdHigh`, `CannyApertureSize`,
  `UseL2Gradient`
- Sobel:
  `SobelDegreeX`, `SobelDegreeY`, `SobelKernelSize`
- Scharr:
  `ScharrDegreeX`, `ScharrDegreeY`
- Laplacian:
  `LaplacianKernelSize`
- Runtime-generated ComboBox, TextBox, CheckBox, and Slider editors can publish
  their stable property identity to the existing shared guide binder.
- Applicability guidance follows the selected `EdgeType`.
- Guidance is grounded in
  `Core/Pipeline/Tools/VisionPipelineEdgeDetectionTool.cs` and the current
  Pipeline validation rules.

## Operator Guidance Contract

| Group | Meaning and tuning effect | Evidence after explicit Preview |
|---|---|---|
| Edge type | Distinguishes Canny binary edges from Sobel/Scharr/Laplacian derivative images and changes the active parameters. | Edge location, continuity, thickness, `EdgePointCount`, downstream Contour/Line stability |
| Canny | Explains weak/strong hysteresis thresholds, 3/5/7 aperture normalization, and L2 versus L1 gradient magnitude. | Low-contrast retention, noise, breaks, edge thickness, runtime |
| Sobel | Explains X/Y derivative direction, nonzero derivative-pair requirement, and odd 1..31 kernel behavior. | Intended horizontal/vertical/diagonal boundaries, minimum features, noise, runtime |
| Scharr | Explains fixed 3x3 behavior and runtime-clamped 0/1 derivative selectors. | Nonzero derivative pair, intended direction, noise |
| Laplacian | Explains all-direction second derivative and odd kernel trade-offs. | Edge location, double lines, noise, minimum features, downstream Contour stability |

The guide does not select a value, run the Tool, or claim that
`EdgePointCount` alone proves semantic correctness.

## UI Evidence

- Before:
  `artifacts/p260_edge_detection_parameter_guide_20260730/before/p260_edge_detection_parameter_guide_baseline.png`
- After:
  `artifacts/p260_edge_detection_parameter_guide_20260730/after/p260_edge_detection_parameter_guide.png`
- Focused smoke record:
  `artifacts/p260_edge_detection_parameter_guide_20260730/after/p260-edge-detection-parameter-guide-smoke.txt`
- Post-change fallback audit:
  `artifacts/p260_parameter_guide_fallback_audit_20260730/after_edge_detection/p260-parameter-guide-fallback-audit.tsv`

Visual comparison: before P260, focusing EdgeDetection parameters showed no
Parameter Guide. After P260, focusing `High threshold` opens the same shared
overlay drawer with current value/unit, meaning, effect, and evidence to check,
while retaining the existing parameter cards and explicit Preview surface.

## Acceptance Criteria And Verification

- Every browsable `EdgeDetectionToolProperty` resolves detailed content:
  passed, 11/11.
- Canny, Sobel, Scharr, and Laplacian inactive conditions are explicit:
  passed.
- Runtime-generated parameter cards select the correct stable property:
  passed for Edge type, Canny Low/High, and Sobel X focused paths.
- Korean and English content:
  passed.
- Guide selection does not change Preview/Run count, layers, active layer, or
  routes:
  passed.
- Current solution build:
  `dotnet build "OpenVisionLab.sln" -c Debug -p:Platform="Any CPU" --no-restore`
  passed with 0 warnings and 0 errors.
- Focused current-source UI smoke:
  `p260_edge_detection_parameter_guide` passed.
- Post-change canonical-family audit:
  `p260_parameter_guide_fallback_audit` passed at 236 detailed / 82 Basic.
- Shared P259 Parameter Guide regression:
  `p259_parameter_guide_expansion` passed.
- Full preprocess output/auto-preview regression:
  `wpf_preprocess_output_preview_flow` passed.
- OpenVisionLab readiness contracts:
  passed.
- `git diff --check`:
  passed; only existing line-ending conversion notices were printed.

## Boundary

P260 is operator guidance and shared-UI connection evidence. It does not
automatically tune EdgeDetection, alter the runtime algorithm, qualify an
inspection, prove field robustness, add camera/PLC/MES scope, or complete
CVR-00 participant validation.

## Completion Record

Status: Complete  
Scope: EdgeDetection 11/11 detailed Parameter Guide content plus dynamic-card
selection integration.  
Acceptance criteria: exhaustive content, conditional applicability,
Korean/English, correct focus identity, and zero guide-caused execution/layer/
route effects all passed.  
Verification: current solution build, focused UI smoke, post-change fallback
audit, shared-guide and preprocess regressions, readiness, visual inspection,
and diff check passed.  
Evidence:
`artifacts/p260_edge_detection_parameter_guide_20260730`,
`artifacts/p260_parameter_guide_fallback_audit_20260730`, and this report.  
Boundary / next dependency: the next bounded family is `RotateScale`, whose
five properties remain Basic fallback; CVR-00 still requires three independent
first-time participants and raw observations.

## Next Priority

1. Expand verified detailed guidance to `RotateScale` 0/5 | Recommended model: `gpt-5.6-sol` | Reasoning effort: `medium`
2. Reassess the remaining 77 Basic fallback properties after `RotateScale`; do not bulk-author advanced matcher guidance without runtime-grounded review | Recommended model: `gpt-5.6-sol` | Reasoning effort: `medium`
