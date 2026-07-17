# OpenVisionLab Guided Inspection Setup Spec

Updated: 2026-07-10 KST

This spec defines the first non-LLM guided setup path for OpenVisionLab. It exists so OpenVisionLab can be useful as a rule-based vision workbench even when no GPT/Gemini/Claude transcript or API key is available.

## Product Boundary

Guided Inspection Setup is a recipe authoring helper, not a hardware wizard.

It may:

- Ask for inspection intent.
- Ask for sample, ROI, template, measurement region, and OK/NG tolerance.
- Pick a supported OpenVisionLab tool family.
- Generate starter XML from deterministic templates.
- Show required metrics and Good/Bad sample checks.
- Open the relevant Learn topic or Tool View by explicit user action.

It must not:

- Connect to cameras.
- Configure lighting.
- Configure PLC/I/O.
- Create accounts or deployment targets.
- Run Preview automatically.
- Run Review automatically.
- Create layers automatically.
- Change input/output routing automatically.
- Replace PropertyGrid tool editing.

## User Flow

1. Operator opens `Guided Inspection Setup`.
2. Operator chooses an intent.
3. The guide shows required inputs and missing readiness items.
4. Operator fills the required fields.
5. Operator clicks `Create Starter XML`.
6. OpenVisionLab creates XML in the draft/review area only.
7. Operator validates the XML.
8. Operator explicitly imports, previews, or runs review.

## Initial Starter Intents

| Intent | Use When | Tool Family | Required Inputs | Starter Metrics | Learn Path |
| --- | --- | --- | --- | --- | --- |
| Blob count / particle count | Bright or dark connected regions must be counted. | `Threshold` -> `Blob` -> optional `OverlayMerge` | sample image, object polarity, minimum area, maximum area, optional ROI | `ResultCount`, `AreaMin`, `AreaMax`, `AreaAvg`, `BoundsWidthMax`, `BoundsHeightMax` | `blob`, `preprocess` |
| Contour shape / outline | Shape, outline, or bounding region matters more than raw connected area. | `Threshold` -> `Morphology` -> `Contour` -> optional `OverlayMerge` | sample image, threshold range, contour area range, optional ROI | `ResultCount`, `AreaMin`, `AreaMax`, `BoundsWidthAvg`, `BoundsHeightAvg` | `contour`, `preprocess` |
| Pin gap / pitch measurement | Edge-to-edge gap, pitch, clearance, or width must be measured. | `LineDistance` -> `OverlayMerge` | sample image, measurement scope, ROI or whole-array region, pixel/mm scale, nominal distance, tolerance | `DistancePxAvg`, `DistancePxRange`, `DistancePxMax`, `DistanceMmAvg`, `DistanceMmRange`, `DistanceMmMax` | `line` |
| Template target presence | A stable visual target should be found or rejected. | `Matching` -> optional `OverlayMerge` | sample image, template image/path, search ROI, minimum score, expected count | `ResultCount`, `ScoreMax`, `ScoreAvg`, `AngleAvg` | `template-matching` |
| Brightness drift / mean value | Average gray or channel value decides OK/NG. | `Mean` -> optional `OverlayMerge` | sample image, ROI, mean type, minimum/maximum expected value | `MeanValueMin`, `MeanValueMax`, `MeanValueAvg`, `ResultCount` | `mean` |

## Intent Contracts

### Blob Count / Particle Count

Locked tool family:

- `Threshold`
- `Blob`
- optional `OverlayMerge`

Required readiness:

- `Main` sample image exists.
- Object polarity is known.
- `MIN_AREA` and `MAX_AREA` are set.
- Expected count min/max is set when OK/NG judgement is needed.

Starter XML rules:

- `Threshold.OutputLayer` feeds `Blob.InputLayer`.
- `Blob.USE_THRESHOLD=false` when the input is already binary.
- `OverlayMerge` is last when used for operator review.

### Contour Shape / Outline

Locked tool family:

- `Threshold`
- optional `Morphology`
- `Contour`
- optional `OverlayMerge`

Required readiness:

- `Main` sample image exists.
- Shape target is described as outline/bounds/area.
- `MIN_AREA` and `MAX_AREA` are set.
- ROI is recommended when multiple unrelated shapes exist.

Starter XML rules:

- Use `Contour` when the operator cares about outline, bounds, or shape review.
- Do not use `Contour` for pin gap/pitch distance unless the intent is actually shape/bounds inspection.
- Use `OverlayMerge` only as an explicit final review step.

### Pin Gap / Pitch Measurement

Locked tool family:

- `LineDistance`
- optional `OverlayMerge`

Required readiness:

- `Main` sample image exists.
- Measurement definition is selected: gap, pitch, width, or clearance.
- Measurement scope is selected: whole pin array or marked ROI.
- Pixel/mm scale is known or intentionally kept as px-only.
- Nominal distance and tolerance are set.

Starter XML rules:

- Do not choose `Contour` just because pins are visible.
- Do not judge by `DistancePxAvg` or `DistanceMmAvg` alone.
- Include a consistency or outlier gate: `DistancePxRange`, `DistanceMmRange`, `DistancePxMax`, or `DistanceMmMax`.
- If one step judges nominal distance and another judges consistency, duplicate the same `LineDistance` parameters into two validation steps with separate output layers.

Calibration review:

- Show `PIXELPERMM` as an explicit mm/px scale before Starter XML is accepted by the operator.
- Convert the nominal `DistanceMmAvg` min/max gate into px using `mm / PIXELPERMM`.
- Convert the `DistanceMmRange` max gate into px using `mm / PIXELPERMM`.
- Warn that average-only measurement is not enough; the setup must keep a range or max-outlier gate before the measurement can be trusted.

### Template Target Presence

Locked tool family:

- `Matching`
- optional `OverlayMerge`

Required readiness:

- `Main` sample image exists.
- Template image/path exists.
- Search ROI is set or intentionally full-image.
- `SCORE_MIN` is 0..1.
- Expected count is known.

Starter XML rules:

- Use `Matching` when target appearance is stable.
- Use `EdgeBasedMatching` or `FeatureMatching` later only when the operator explicitly chooses shape/edge robustness or feature robustness.
- Missing template dependencies must block import until fixed.

### Brightness Drift / Mean Value

Locked tool family:

- `Mean`
- optional `OverlayMerge`

Required readiness:

- `Main` sample image exists.
- ROI is set when only a local region matters.
- Mean type is selected.
- Minimum and/or maximum acceptable mean is set.

Starter XML rules:

- Use `MeanValueAvg` for average brightness checks.
- Use min/max gates when Good/Bad separation is known.
- Keep the result review explicit; Learn/sample navigation must not run review.

## Readiness Checklist For Any Intent

Before creating starter XML, the guide should display:

- Sample image: ready / missing
- Required ROI: ready / optional / missing
- Required template: ready / optional / missing
- Required second input: ready / not needed / missing
- Required calibration: ready / px-only / missing
- Acceptance metric: ready / missing
- Good/Bad sample pair: ready / optional / missing

## First UI Entry Point

The first UI entry should be small:

- A `Guided Setup` entry in Recipe Manager or Learn Mode.
- Intent list on the left.
- Readiness checklist and starter XML explanation on the right.
- `Create Starter XML` button that only writes XML into the draft/review area.
- `Validate` remains explicit.
- `Import`, `Preview`, and `Run Review` remain explicit.

Do not build a full-screen wizard until these intent contracts are validated against samples and readiness checks.

## Implementation Status 2026-07-11

- Recipe Manager has a separate `Guided setup` tab; Pipeline remains the default tab.
- The shared create action updates only the XML draft and keeps Validate, Import, Preview, and Run explicit.
- Pin gap/pitch exposes ROI samples, nominal min/max distance, range gate, and mm/px scale with `MM-READY`, `PX-ONLY`, or `MISSING` validation.
- A positive mm/px value generates `DistanceMmAvg` + `DistanceMmRange` gates and shows their px equivalents. A blank scale intentionally generates `DistancePxAvg` + `DistancePxRange` gates with `PIXELPERMM=0` and makes no physical-unit claim. A malformed nonblank scale blocks Starter XML creation.
- Latest-EXE direct smoke runs the public `Line_Pins_Synthetic_OK.png` and `Line_Pins_Synthetic_WidePin_NG.png` pair through both generated modes. Good is OK in both, Bad is NG in both, and `DistanceMmAvg=0.224` matches `DistancePxAvg=37.263 * 0.006` within the smoke tolerance.
- Blob count exposes ROI, threshold, count range, and area range with `READY`/`MISSING` validation.
- Contour shape exposes ROI, threshold, count range, and area range with `READY`/`MISSING` validation.
- Matching target presence exposes template path, search ROI, `SCORE_MIN`, and expected count with `READY`/`MISSING` validation.
- Mean brightness exposes optional ROI, `Mean`/`MeanStdDev` type, and Min/Max GV with `READY`/`MISSING` validation.
- The shared action routes Pin gap, Blob, Contour, Matching, and Mean to deterministic intent generators while keeping one Starter XML command.
- The first five standalone starter-intent contracts are complete; Validate, Import, Preview, and Run remain explicit follow-up actions.

## Verification Requirements

Initial implementation should add or extend checks for:

- Each starter intent maps to supported ToolType names.
- Each starter intent maps to an existing Learn path or Learn document.
- Each starter intent names known metrics from `VisionPipelineKnownMetrics`.
- Pin-gap starter includes a range or max distance consistency gate.
- Starter XML creation does not call Preview, Run Review, layer creation, or input-route mutation.

Focused UI evidence is required when the UI entry point is added:

- before screenshot from current build
- after screenshot from current build
- screenshot smoke layout/text/internal checks
- explicit no Preview/Run count assertion if starter XML creation is exercised
