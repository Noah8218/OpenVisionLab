# Vision Pipeline LLM Prompt Template

Use this template when asking an LLM to create an OpenVisionLab first-pass recipe.

## Prompt

```text
You are generating an OpenVisionLab VisionPipeline XML recipe.

Goal:
- Detect: <target object, symbol, defect, edge, measurement, or region>
- Input layer: Main
- Expected result: <count / boxes / OK-NG / transformed image / measurement>
- Allowed false positives: <low / medium / high>
- Preferred chain, if any: <Threshold -> Morphology -> Contour, etc.>

Image notes:
- Polarity: <dark target on bright background / bright target on dark background / unknown>
- ROI: <x,y,width,height or "full image">
- Approximate target size: <pixel or mm range if known>
- Important misses to avoid: <notes>

OpenVisionLab rules:
- Output a complete VisionPipeline XML.
- Use only supported ToolType values:
  Threshold, Morphology, Filter, EdgeDetection, Blob, Contour, LineGauge,
  RotateScale, Matching, Mean, FeatureMatching, OverlayMerge.
- Do not output form-only or demo-only features as pipeline ToolType:
  HSV, Histogram, Arithmetic, Color, Barcode, QR, OCR, EasyBarCode,
  EasyQRCode, EasyOcr, or semantic decoders.
- If the requested target needs a decoder or classifier that the pipeline runner
  does not support yet, generate only the supported candidate-detection steps
  and state the gap in the summary.
- Use stable InputLayer and OutputLayer names.
- The first step usually reads from Main.
- Later steps should read from the previous output layer.
- Do not overwrite Main unless the user explicitly requests it.
- If a later step reads Main after preprocessing, verify that this is intentional. Otherwise change it to the previous OutputLayer.
- If a later inspection reads Main after preprocessing, treat it as an intentional branch and explain why.
- If the recipe uses independent branches, add a final OverlayMerge step.
- OverlayMerge should read the base review layer, usually Main, and write one final review layer.
- OverlayMerge SourceLayers must list branch result layers separated by semicolons.
- The final review image should contain all branch detections in one output layer.
- Users should not need to inspect multiple branch output images to judge the result.
- Use invariant culture numbers.
- Use C# enum names.
- Do not invent parameter names.
- Use score and weight parameters such as `SCORE_MIN`, `GREEDINESS`, and `HYBRID_VERIFY_IMAGE_WEIGHT` as `0..1` decimals, not percentages. Use `0.8`, not `80`.
- Use positive numeric values for `MAGNIFIATION`, `RANSAC_REPROJ_THRESHOLD`, and `COARSE_ANGLE_STEP`.
- Keep `FIND_ANGLE_MIN` less than or equal to `FIND_ANGLE_MAX`.
- Use only existing template/image dependency paths. If no real file is available, omit dependency path parameters until OpenVisionLab attaches a reference image.
- Do not embed image data.
- Use acceptance rules only when they are conservative.
- Prefer sample-backed metric gates from the Sample Catalog. If no close sample exists, keep acceptance loose and explain which metric should be tuned.
- When Good/Bad sample pairs exist, use them to set acceptance gates that pass the good sample and fail the bad sample for an explainable metric.

Result channel contract:
- OpenVisionLab will derive `Inspection.Status`, `Inspection.FailedStep`,
  `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction`
  from XML validation and explicit sample runs.
- Do not add `Inspection.*` XML nodes, custom elements, or custom parameters.
- Make step names, layer routes, output layers, judgement parameters, and
  dependency paths clear enough for those channels to be computed after import.

Return only:
Complete `VisionPipeline` XML that can be pasted into OpenVisionLab and validated before import.
```

## Recommended First-Pass Chains

```text
Bright/dark region:
Threshold -> Morphology -> Blob

Text, number, symbol candidates:
Threshold -> Morphology -> Contour

Edge or line position:
Filter -> EdgeDetection -> LineGauge

Template-like part presence:
Matching

Feature-like target:
FeatureMatching

Independent branch review:
Branch A + Branch B + ... -> OverlayMerge
```

## Tuning Checklist Examples

- If too many objects are detected, increase `MIN_AREA` or adjust `Threshold`.
- If targets are missing, lower `MIN_AREA` or change `ThresholdType`.
- If objects are broken, increase Morphology `KernelWidth`, `KernelHeight`, or `Iterations`.
- If objects are merged, use Morphology `Open` or reduce kernel size.
- If runtime is too slow, add ROI and tighten area limits.
- For branched recipes, tune each branch first, then check final `MergeOverlayCount`.

## Reference Sample

Use `docs/samples/Contour_TextSymbols.pipeline.xml` as the baseline for text, number, and symbol candidate detection.

Use `docs/samples/Contour_AllSymbolsAndFaint_LLM.pipeline.xml` when the target needs multiple branches but the user must review one final overlay image.

Use `docs/samples/Feature_Template_Review.pipeline.xml` when the target needs a feature/template review path with template image, detected crop, score, and overlay confirmation.
