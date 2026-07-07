# OpenVisionLab LLM XML Authoring Guide

Updated: 2026-07-06 19:50 KST

This guide is the API reference to give to GPT, Gemini, Claude, or another LLM before asking it to draft OpenVisionLab recipe XML.

OpenVisionLab is an OpenCvSharp4 rule-based vision workbench plus an LLM-assisted XML recipe authoring flow. It is not a camera, lighting, PLC, I/O, account, or deployment platform.

## Authoring Loop

Use this loop when collecting real LLM transcripts:

1. Give the LLM this guide plus `docs/OPENVISIONLAB_LLM_TOOL_CATALOG.json`.
2. Provide the reference image context, inspection goal, detection points, expected OK/NG condition, and any existing template/image dependency paths.
3. Ask for one `VisionPipeline` XML document only.
4. Paste or load the XML into Recipe Manager.
5. Run `Validate`. Do not import yet.
6. If validation is NG, copy the validation report and ask the LLM to repair the XML without changing the intent.
7. Import only after validation is OK and dependencies are present.
8. Run sample/Good/Bad checks explicitly in OpenVisionLab.

Do not ask the LLM to run Preview, Run, load images, switch layers, or accept the recipe. Those are explicit user actions inside OpenVisionLab.

For the pin-gap distance case, prefer the in-app flow first: select `Pin gap / edge distance (LineDistance)` in Recipe Manager, set the ROI/spec fields, click `Build prompt`, then `Copy prompt`. That copied prompt includes the XML-only pin-gap task contract. A file-based fallback packet is also available at `llm_prompt_packets/pin_gap_distance`.

## Minimal XML Shape

```xml
<?xml version="1.0" encoding="utf-8"?>
<VisionPipeline>
  <Name>Example_Pipeline</Name>
  <Steps>
    <Step>
      <Name>01 Threshold Precheck</Name>
      <ToolType>Threshold</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Threshold_Result</OutputLayer>
      <Parameters>
        <Parameter>
          <Key>Mode</Key>
          <Value>Threshold</Value>
        </Parameter>
        <Parameter>
          <Key>Threshold</Key>
          <Value>128</Value>
        </Parameter>
        <Parameter>
          <Key>MaxValue</Key>
          <Value>255</Value>
        </Parameter>
        <Parameter>
          <Key>ThresholdType</Key>
          <Value>Binary</Value>
        </Parameter>
      </Parameters>
    </Step>
  </Steps>
</VisionPipeline>
```

Required Step fields:

- `Name`: short, unique, numbered, and meaningful.
- `ToolType`: exact supported tool name or supported alias.
- `Enabled`: usually `true`.
- `InputLayer`: use `Main` for the source image, or use a previous enabled Step `OutputLayer`.
- `OutputLayer`: unique result layer for evidence review.
- `Parameters`: zero or more `Parameter` entries with `Key` and `Value`.

Optional judgement fields:

- `UseAcceptance`: `true` when this Step has an OK/NG gate.
- `ExpectedSuccess`: usually `true`; use `false` only when the Step is expected to fail.
- `MaxElapsedMilliseconds`: positive time budget for this Step.
- `AcceptanceMetricName`: known metric such as `ResultCount`, `ScoreMax`, `AreaAvg`, `DistanceMmAvg`, `DistanceMmRange`, `MergeOverlayCount`.
- `UseAcceptanceMetricMinimum`, `AcceptanceMetricMinimum`
- `UseAcceptanceMetricMaximum`, `AcceptanceMetricMaximum`

Parameter values are strings in XML but are validated by type. Boolean values must be `true` or `false`. Numeric values must use invariant decimal text such as `0.6`, `128`, or `3.5`.

## Hard Rules

- Return only a `VisionPipeline` XML document when the user asks for XML.
- Do not invent OpenVisionLab screens, hardware nodes, camera settings, lighting settings, PLC tags, I/O signals, users, roles, or deployment targets.
- Use `Main` only when the Step intentionally starts from the source image.
- Use previous Step output layers for sequential pipelines.
- For intentional fan-out from `Main` or another earlier layer, set parameter `ALLOW_BRANCH_INPUT` to `true`.
- Do not overwrite input layers. Prefer a separate `OutputLayer` for every enabled Step.
- Do not write the same output layer from multiple Steps unless overwrite review is intentional.
- Do not create custom `Inspection.*` XML nodes or parameters. `Inspection.Status`, `Inspection.FailedStep`, `Inspection.Evidence`, `Inspection.Benchmark`, and `Inspection.NextAction` are review channels derived by OpenVisionLab after validation/run evidence.
- Do not use placeholder dependency paths. If a template path is unknown, omit `TemplatePath` and `PATTERN_PATH`, then explain outside XML that the user must choose a real template path.
- Matching score parameters such as `SCORE_MIN`, `GREEDINESS`, and `HYBRID_VERIFY_IMAGE_WEIGHT` are 0..1 decimals. Use `0.6`, not `60` or `80`.
- Positive parameters must be positive: `MAGNIFIATION`, `RANSAC_REPROJ_THRESHOLD`, `COARSE_ANGLE_STEP`, `PIXELPERMM`, `ScaleXPercent`, `ScaleYPercent`, kernel sizes, `NUM_MATCH`, `SEARCH_STEP`.
- `FIND_ANGLE_MIN` must be less than or equal to `FIND_ANGLE_MAX`.
- Gray-level values such as `Threshold`, `MaxValue`, `RangeMin`, `RangeMax`, `CANNY_LOW`, and `CANNY_HIGH` must be within 0..255.
- `Arithmetic` operation mode needs `InputLayerB` unless the operation is `Bitwise_NOT` or `ABS`, the mode is `Offset`, or `UseConstantInput` is `true`.
- `OverlayMerge` should be the final enabled Step when it is the user-facing review result.
- For pin-to-pin, edge-to-edge, pitch, width, or clearance checks using `LineDistance`, do not judge only `DistancePxAvg` or `DistanceMmAvg`. Also constrain candidate consistency with `DistancePxRange`/`DistanceMmRange` or reject long outliers with `DistancePxMax`/`DistanceMmMax`. If one Step must judge the nominal distance and another must judge consistency, duplicate the same `LineDistance` parameters into a second validation Step with a separate `OutputLayer`.

## Result Channel Contract

LLMs often try to emit judgement objects directly. Do not do that.

OpenVisionLab derives these review channels:

- `Inspection.Status`: final OK/NG from XML validation plus explicit sample/Good/Bad checks.
- `Inspection.FailedStep`: based on each enabled Step `Name`, `InputLayer`, `OutputLayer`, and `ToolType`.
- `Inspection.Evidence`: output layers, metrics, overlays, and judgement parameters.
- `Inspection.Benchmark`: deterministic parameters, dependency paths, and run-history comparison.
- `Inspection.NextAction`: derived guidance after validation or run review.

The XML must provide enough Step structure and measurable parameters for those channels to be derived.

## Supported Tool Types

The validator currently accepts these names case-insensitively:

- `Threshold`
- `Morphology`
- `Filter`
- `EdgeDetection`, `Edge`
- `Blob`
- `Contour`
- `Line`, `LineGauge`
- `LineDistance`, `LineDistanceGauge`
- `LineIntersection`, `LineIntersectionGauge`
- `Matching`, `TemplateMatching`
- `EdgeBasedMatching`, `EdgeBasedTemplateMatching`, `EdgeTemplateMatching`
- `Mean`
- `RotateScale`, `RotateAndScale`
- `Feature`, `FeatureMatching`, `Sift`
- `Arithmetic`
- `OverlayMerge`, `ResultMerge`, `MergeResult`

Prefer the canonical names used in samples: `Threshold`, `Morphology`, `Filter`, `EdgeDetection`, `Blob`, `Contour`, `LineGauge`, `LineDistance`, `Matching`, `EdgeBasedMatching`, `FeatureMatching`, `Mean`, `RotateScale`, `Arithmetic`, and `OverlayMerge`.

## Common Patterns

### Threshold to Blob

Use when the target can be separated by brightness and measured by connected regions.

```xml
<VisionPipeline>
  <Name>LLM_Threshold_Blob</Name>
  <Steps>
    <Step>
      <Name>01 Binary</Name>
      <ToolType>Threshold</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Binary_Result</OutputLayer>
      <Parameters>
        <Parameter><Key>Threshold</Key><Value>128</Value></Parameter>
        <Parameter><Key>MaxValue</Key><Value>255</Value></Parameter>
        <Parameter><Key>ThresholdType</Key><Value>Binary</Value></Parameter>
      </Parameters>
    </Step>
    <Step>
      <Name>02 Blob Inspect</Name>
      <ToolType>Blob</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Binary_Result</InputLayer>
      <OutputLayer>Blob_Result</OutputLayer>
      <Parameters>
        <Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>
        <Parameter><Key>MIN_AREA</Key><Value>50</Value></Parameter>
        <Parameter><Key>MAX_AREA</Key><Value>999999</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>1000</MaxElapsedMilliseconds>
      <AcceptanceMetricName>ResultCount</AcceptanceMetricName>
      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
      <AcceptanceMetricMinimum>1</AcceptanceMetricMinimum>
    </Step>
  </Steps>
</VisionPipeline>
```

### Template Matching

Use when a stable local template exists.

```xml
<VisionPipeline>
  <Name>LLM_Template_Matching</Name>
  <Steps>
    <Step>
      <Name>01 Template Match</Name>
      <ToolType>Matching</ToolType>
      <Enabled>true</Enabled>
      <InputLayer>Main</InputLayer>
      <OutputLayer>Matching_Result</OutputLayer>
      <Parameters>
        <Parameter><Key>TemplatePath</Key><Value>docs\samples\templates\Contour_7PQRS_Template.png</Value></Parameter>
        <Parameter><Key>PATTERN_PATH</Key><Value>docs\samples\templates\Contour_7PQRS_Template.png</Value></Parameter>
        <Parameter><Key>MATCH_MODE</Key><Value>CCoeffNormed</Value></Parameter>
        <Parameter><Key>SCORE_MIN</Key><Value>0.85</Value></Parameter>
        <Parameter><Key>NUM_MATCH</Key><Value>1</Value></Parameter>
        <Parameter><Key>MAGNIFIATION</Key><Value>1</Value></Parameter>
        <Parameter><Key>USE_FIND_ANGLE</Key><Value>false</Value></Parameter>
      </Parameters>
      <UseAcceptance>true</UseAcceptance>
      <ExpectedSuccess>true</ExpectedSuccess>
      <MaxElapsedMilliseconds>1000</MaxElapsedMilliseconds>
      <AcceptanceMetricName>ResultCount</AcceptanceMetricName>
      <UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>
      <AcceptanceMetricMinimum>1</AcceptanceMetricMinimum>
    </Step>
  </Steps>
</VisionPipeline>
```

### Fan-Out With OverlayMerge

Use when several inspections start from the same prepared layer and should be reviewed together. Mark branch inputs explicitly.

```xml
<Step>
  <Name>03 Top Region</Name>
  <ToolType>Contour</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Clean_Binary</InputLayer>
  <OutputLayer>Top_Contour</OutputLayer>
  <Parameters>
    <Parameter><Key>ALLOW_BRANCH_INPUT</Key><Value>true</Value></Parameter>
    <Parameter><Key>USE_THRESHOLD</Key><Value>false</Value></Parameter>
    <Parameter><Key>USE_ROI</Key><Value>true</Value></Parameter>
    <Parameter><Key>CvROI</Key><Value>0,0,768,270</Value></Parameter>
    <Parameter><Key>MIN_AREA</Key><Value>100000</Value></Parameter>
    <Parameter><Key>MAX_AREA</Key><Value>300000</Value></Parameter>
  </Parameters>
</Step>
```

Then add a final merge Step:

```xml
<Step>
  <Name>05 Merge Review</Name>
  <ToolType>OverlayMerge</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Main</InputLayer>
  <OutputLayer>Review_Result</OutputLayer>
  <Parameters>
    <Parameter><Key>SourceLayers</Key><Value>Top_Contour;Bottom_Contour</Value></Parameter>
    <Parameter><Key>BurnIn</Key><Value>true</Value></Parameter>
    <Parameter><Key>DrawLabels</Key><Value>false</Value></Parameter>
    <Parameter><Key>AllowEmpty</Key><Value>false</Value></Parameter>
  </Parameters>
</Step>
```

### Arithmetic With Second Input

Use only after the second input layer already exists.

```xml
<Step>
  <Name>03 Compare Binary Layers</Name>
  <ToolType>Arithmetic</ToolType>
  <Enabled>true</Enabled>
  <InputLayer>Binary_A</InputLayer>
  <OutputLayer>Binary_Diff</OutputLayer>
  <Parameters>
    <Parameter><Key>ArithmeticMode</Key><Value>Operation</Value></Parameter>
    <Parameter><Key>ArithmeticOperation</Key><Value>ABSDIFF</Value></Parameter>
    <Parameter><Key>InputLayerB</Key><Value>Binary_B</Value></Parameter>
  </Parameters>
</Step>
```

## Correction Prompt Template

Use this when validation fails:

```text
You are repairing an OpenVisionLab VisionPipeline XML draft.

Keep the original inspection intent. Return only one VisionPipeline XML document.

Validation report:
{paste OpenVisionLab validation report}

Dependency report:
{paste dependency report if present}

Original draft:
{paste previous XML}

Repair rules:
- Use only supported ToolType names.
- Do not invent layers. InputLayer must be Main or a previous enabled Step OutputLayer.
- Do not invent dependency files. Existing template/image paths only.
- Do not emit Inspection.* XML nodes or parameters.
- SCORE_MIN, GREEDINESS, and HYBRID_VERIFY_IMAGE_WEIGHT are 0..1 decimals.
- Use separate OutputLayer values for evidence.
```

## Transcript Corpus Policy

Store raw external LLM experiments outside public sample paths until reviewed:

- Raw prompt/response: `artifacts\llm_transcripts\raw\...`
- User-provided or operator-repaired XML replay cases: `artifacts\llm_transcripts\manual\...`
- Sanitized replay corpus candidate: `artifacts\llm_transcripts\sanitized\...`
- Public documentation examples: only after private product names, customer data, old company recipe names, and non-public assets are removed.

Manual replay cases are useful validation evidence, but they are not real GPT/Gemini/Claude transcript evidence. Do not report them as external LLM correction-loop transcripts.

Commit only sanitized examples that are safe under the public sample asset policy.

## Grounding Sources

This guide is based on:

- `0. UI\0) MENU\Wpf\OpenVisionShellHostRecipeCommandSurface.cs`
- `1. Core\VisionPipelineValidation.cs`
- `1. Core\VisionPipelineStepParameterSchema.cs`
- `1. Core\VisionPipelineKnownMetrics.cs`
- `1. Core\VisionPipelineArithmeticStep.cs`
- `docs\samples\*.pipeline.xml`
- Direct smoke coverage in `OpenVisionLabDirectSmokeRunner.cs` for malformed XML, missing input layer, unsupported ToolType, missing dependency path, invalid parameter values, matching score percentage misuse, `Inspection.*` misuse, correction-bundle copy, corrected import, and missing Arithmetic `InputLayerB`.
