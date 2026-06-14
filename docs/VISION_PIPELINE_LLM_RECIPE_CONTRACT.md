# Vision Pipeline LLM Recipe Contract

This document defines the minimum contract for generating OpenVisionLab pipeline XML from an image and a user inspection goal.

The goal is not to make a perfect detector in one prompt. The goal is to generate a conservative first recipe that OpenVisionLab can import, run, review, tune, and save.

## Inputs To The LLM

An LLM recipe request should provide:

- Source image or a clear image description.
- Target object or defect to detect.
- Expected output style: count, boxes, pass/fail, measurement, or transformed image.
- Known constraints such as ROI, minimum size, polarity, speed limit, or allowed false positives.
- Preferred tool chain if known.

## Required Output

The LLM should output only:

1. A short recipe summary.
2. A complete `VisionPipeline` XML document.
3. A tuning checklist with 3 to 5 concrete parameters to adjust.

The XML must be directly importable by OpenVisionLab.

## XML Rules

- Root must be `VisionPipeline`.
- Every `Step` must include `Name`, `ToolType`, `Enabled`, `InputLayer`, and `OutputLayer`.
- The first step should usually read from `Main`.
- Each later step should normally read from the previous step output unless the tool intentionally needs the original source.
- Parameter values must use invariant culture:
  - Boolean: `true` or `false`
  - Number: `127.5`, not localized formats
  - Enum: C# enum name used by the OpenVisionLab property
  - ROI: `x,y,width,height`
  - ROI list: `x,y,width,height;x,y,width,height`
- Do not invent parameter names. Use names already supported by the relevant tool property.
- Do not embed source images in the pipeline XML.

## Preferred First-Pass Chains

Use these chains as conservative starting points:

- Bright/dark region detection:
  `Threshold -> Morphology -> Blob or Contour`
- Text, number, symbol candidates:
  `Threshold -> Morphology -> Contour`
- Edge presence or position:
  `Filter or EdgeDetection -> LineGauge`
- Template-like part presence:
  `Matching`
- Repeated feature-like targets:
  `FeatureMatching`

## Acceptance Rules

Use acceptance only when the user gave a detectable rule or when a safe default exists.

Recommended metrics:

- `ResultCount`: object count, contour count, blob count, matching count
- `AreaMin`, `AreaMax`, `AreaAvg`: blob/contour area sanity
- `ScoreMax`, `ScoreAvg`: matching confidence
- `EdgeCount`, `EdgePointCount`: line/edge detection
- `MeanValueAvg`: brightness or intensity checks

Do not make acceptance too tight in the first recipe. A good first pass should fail only when the recipe is clearly broken.

## Example: Contour Text Symbols

Use `docs/samples/Contour_TextSymbols.pipeline.xml` as the reference pattern:

- `Main -> TextSymbol_Binary`
- `TextSymbol_Binary -> TextSymbol_Clean`
- `TextSymbol_Clean -> TextSymbol_Contour`
- Acceptance: `ResultCount` between `35` and `80`

This chain makes every intermediate image reviewable in OpenVisionLab.

## Review Loop

After import and run:

1. Check the final step status and acceptance metric.
2. Open the step preview viewer.
3. Review false positives and missed targets using overlay zoom/pan.
4. Adjust threshold, morphology kernel, ROI, and min/max area.
5. Save the tuned pipeline as the active recipe.

## OpenVisionLab Import Flow

Use `Pipeline > AI Recipe` for generated XML:

1. Paste or open the generated `VisionPipeline` XML.
2. Use `Sample` when a known reference recipe is needed.
3. Validate the XML and step/layer references.
4. Run Preview with either the current display layers or a loaded image file.
5. Apply to Pipeline only after validation succeeds.
6. OpenVisionLab selects the final review step and runs a preview automatically when an input image is available.
7. Save from the Pipeline form after reviewing the generated steps.

## Do Not

- Do not claim that a recipe is production-ready without sample validation.
- Do not choose hidden image-dependent magic values without listing them in the tuning checklist.
- Do not mix two independent detection goals into one step.
- Do not overwrite an existing user pipeline name unless explicitly requested.
