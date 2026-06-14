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
  Matching, Mean, FeatureMatching.
- Use stable InputLayer and OutputLayer names.
- The first step usually reads from Main.
- Later steps should read from the previous output layer.
- Use invariant culture numbers.
- Use C# enum names.
- Do not invent parameter names.
- Do not embed image data.
- Use acceptance rules only when they are conservative.

Return only:
1. Recipe summary
2. Complete VisionPipeline XML
3. Tuning checklist with 3 to 5 concrete parameters
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
```

## Tuning Checklist Examples

- If too many objects are detected, increase `MIN_AREA` or adjust `Threshold`.
- If targets are missing, lower `MIN_AREA` or change `ThresholdType`.
- If objects are broken, increase Morphology `KernelWidth`, `KernelHeight`, or `Iterations`.
- If objects are merged, use Morphology `Open` or reduce kernel size.
- If runtime is too slow, add ROI and tighten area limits.

## Reference Sample

Use `docs/samples/Contour_TextSymbols.pipeline.xml` as the baseline for text, number, and symbol candidate detection.
