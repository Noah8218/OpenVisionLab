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
- Use only pipeline-runner supported `ToolType` values:
  `Threshold`, `Morphology`, `Filter`, `EdgeDetection`, `Blob`, `Contour`,
  `LineGauge`, `RotateScale`, `Matching`, `Mean`, `FeatureMatching`, and `OverlayMerge`.
- Do not emit form-only or demo-only features as pipeline `ToolType`.
  `HSV`, `Histogram`, `Arithmetic`, `Color`, `Barcode`, `QR`, `OCR`,
  `EasyBarCode`, `EasyQRCode`, and `EasyOcr` are not pipeline runner steps.
- If the user's goal needs a decoder or classifier that is not runner-backed yet,
  generate only the supported candidate-detection steps and state the gap in the recipe summary.
- The first step should usually read from `Main`.
- Each later step should normally read from the previous step output unless the tool intentionally needs the original source.
- Do not use `Main` as an `OutputLayer` unless the user explicitly requested overwriting the source layer.
- If a later inspection step reads `Main` after preprocessing, it is a branch. The final result should make that branch relationship clear.
- If a later step reads `Main` only by accident, revise it to read the previous `OutputLayer`; `Main` should remain the original reference image.
- If a recipe intentionally uses independent branches, add a final `OverlayMerge` step.
- `OverlayMerge` should read the base review layer, usually `Main`, and write one final review layer.
- `OverlayMerge` `SourceLayers` must list branch result layers separated by semicolons, for example `TextSymbol_Contour;FaintTop_Contour`.
- The final review image must contain all branch detections in one output layer.
- Users should not need to inspect several separate branch images to decide whether the recipe worked.
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
- Branched inspection that must be reviewed in one image:
  `Branch A + Branch B + ... -> OverlayMerge`

## Acceptance Rules

Use acceptance only when the user gave a detectable rule or when a safe default exists.

Recommended metrics:

- `ResultCount`: object count, contour count, blob count, matching count
- `AreaMin`, `AreaMax`, `AreaAvg`: blob/contour area sanity
- `ScoreMax`, `ScoreAvg`: matching confidence
- `EdgeCount`, `EdgePointCount`: line/edge detection
- `MeanValueAvg`: brightness or intensity checks
- `MergeOverlayCount`, `MergeSourceCount`: merged branch review result

Do not make acceptance too tight in the first recipe. A good first pass should fail only when the recipe is clearly broken.

Prefer sample-backed metric gates from `docs/samples/OpenVisionLab.SampleCatalog.csv`.
If no close sample exists, use loose acceptance and explain which metric should be tuned first.

When a Good/Bad sample pair exists, use the pair to choose a conservative acceptance gate. The OK sample should pass with margin, and the NG sample should fail for an explainable metric such as count, bounds width/height, line length, score, or mean value.

## Example: Contour Text Symbols

Use `docs/samples/Contour_TextSymbols.pipeline.xml` as the reference pattern:

- `Main -> TextSymbol_Binary`
- `TextSymbol_Binary -> TextSymbol_Clean`
- `TextSymbol_Clean -> TextSymbol_Contour`
- Acceptance: `ResultCount` between `35` and `80`

This chain makes every intermediate image reviewable in OpenVisionLab.

## Example: Branched LLM Contour Recipe

Use `docs/samples/Contour_AllSymbolsAndFaint_LLM.pipeline.xml` as the reference pattern when one image needs multiple independent branches:

- Text/symbol branch creates `TextSymbol_Contour`.
- Faint top mark branch creates `FaintTop_Contour`.
- Faint phone mark branch creates `FaintPhone_Contour`.
- Final `OverlayMerge` step reads `Main`, collects those result layers, and writes `AllSymbols_Overlay`.

This keeps branch tuning explicit while still giving the user one final visual confirmation image.

The final review image is part of the contract:

- `MergeOverlayCount` must match the total overlay count from the selected branch contour steps.
- `MergeSourceCount` must match the number of selected branch outputs that produced overlays.
- The final image should show object-level detections. Do not use broad ROI-sized boxes as the final answer.
- The final image should include the main target branch plus faint or secondary branches when the user asked to detect all visible targets.
- Intermediate branch images are useful for tuning, but users should not need to inspect several separate branch images to know whether the recipe worked.

## Example: Feature Template Review

Use `docs/samples/Feature_Template_Review.pipeline.xml` as the reference pattern when the recipe needs a feature/template review:

- Input image stays in `Main`.
- Template image is loaded by the FeatureMatching tool parameters.
- Output layer contains the homography/box overlay and detected crop metadata.
- Acceptance uses loose `ScoreMax` and `ResultCount` checks for the first pass.

This keeps the template, detected crop, score, and output overlay reviewable in one OpenVisionLab flow.

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
