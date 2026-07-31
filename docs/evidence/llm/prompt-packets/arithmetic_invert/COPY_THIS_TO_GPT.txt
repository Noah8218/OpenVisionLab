You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based inspection that inverts the full source image, then checks the mean brightness of that inverted result. The normal dark source must pass; the excessively bright source must fail.

Attached images:
- Nominal image: `Arithmetic_Invert_Synthetic_OK.png`
- Negative image: `Arithmetic_Invert_Synthetic_Bright_NG.png`
- Both images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- Use the nominal image to author the pipeline. Use the negative image only to understand the expected reject condition.

Inspection intent:
- Invert the entire source image with a unary `Bitwise_NOT` arithmetic operation.
- Measure the mean gray value of that inverted layer across the entire image.
- The nominal dark source must pass because its verified inverted `MeanValueAvg` is `208`.
- The bright negative source must fail because its verified inverted `MeanValueAvg` is `76.7`, below the same acceptance range.
- Do not use Threshold, Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching, LineDistance, HSV, Filter, Morphology, RotateScale, OverlayMerge, a second input layer, or unsupported custom tools.
- Required tool sequence: `Arithmetic`, then `Mean`.
- This is a sequential pipeline, not a branch. The Mean Step must read the Arithmetic Step output layer.

Verified starting values:
- Arithmetic mode: `Operation`
- Arithmetic operation: `Bitwise_NOT`
- `UseConstantInput=false`
- `Bitwise_NOT` is a unary operation here. Do not add `InputLayerB`.
- Mean mode: `MEAN_TYPES=Mean`
- `USE_THRESHOLD=false`
- `USE_ADAPTIVE_THRESHOLD=false`
- `USE_BITWISENOT=false`
- `USE_ROI=false`; inspect the full image
- `USE_MULTI_ROI=false`
- Inverted `MeanValueAvg` acceptance: `190` through `230`
- Current-build baseline: nominal `MeanValueAvg=208`; bright negative `MeanValueAvg=76.7`

Important parameter and metric rules:
- `MeanValueAvg` is the measured average intensity for the selected Mean mode.
- The Arithmetic output must become the Mean input. Do not change `InputLayer` to the output layer within either Step.
- Because `USE_ROI=false`, the Mean judgement applies to the full 572 x 420 inverted image.
- The values above were verified with the current OpenVisionLab build. Do not estimate replacement values from the images.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, deployment, or automatic execution settings.
- Preview and Run are explicit user actions inside OpenVisionLab. The XML must not claim to run automatically.

OpenVisionLab XML contract:
- Return exactly one XML document with root `<VisionPipeline>`.
- Use `<Name>` and `<Steps>` directly under `<VisionPipeline>`.
- Set the pipeline `<Name>` to `Arithmetic_Invert_Inspection`.
- Every `<Step>` must contain `<Name>`, `<ToolType>`, `<Enabled>`, `<InputLayer>`, `<OutputLayer>`, and `<Parameters>`.
- Every parameter must use `<Parameter><Key>...</Key><Value>...</Value></Parameter>`.
- Boolean values must be lowercase `true` or `false`.
- Use invariant numeric text.
- Use only the supported `Arithmetic` and `Mean` ToolTypes for this task.
- `InputLayer` must be `Main` or the `OutputLayer` of an earlier enabled Step.
- Each enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Step 1 requirements:
- Name: `01 Arithmetic Invert`
- ToolType: `Arithmetic`
- Enabled: `true`
- InputLayer: `Main`
- OutputLayer: `Arithmetic_Invert_Result`
- Parameters, exactly:
  - `ArithmeticMode=Operation`
  - `ArithmeticOperation=Bitwise_NOT`
  - `UseConstantInput=false`
- Do not add `InputLayerB` or an acceptance gate to this Step.

Step 2 requirements:
- Name: `02 Inverted Mean Gate`
- ToolType: `Mean`
- Enabled: `true`
- InputLayer: `Arithmetic_Invert_Result`
- OutputLayer: `Arithmetic_Invert_Mean`
- Parameters, exactly:
  - `Name=GPT_Arithmetic_Invert`
  - `MEAN_TYPES=Mean`
  - `USE_THRESHOLD=false`
  - `USE_ADAPTIVE_THRESHOLD=false`
  - `USE_BITWISENOT=false`
  - `USE_ROI=false`
  - `USE_MULTI_ROI=false`
- Acceptance fields must be direct children of the Mean `<Step>`, after `</Parameters>`:
  - `<UseAcceptance>true</UseAcceptance>`
  - `<ExpectedSuccess>true</ExpectedSuccess>`
  - `<MaxElapsedMilliseconds>300</MaxElapsedMilliseconds>`
  - `<AcceptanceMetricName>MeanValueAvg</AcceptanceMetricName>`
  - `<UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>`
  - `<AcceptanceMetricMinimum>190</AcceptanceMetricMinimum>`
  - `<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>`
  - `<AcceptanceMetricMaximum>230</AcceptanceMetricMaximum>`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanations, analysis, tables, Markdown fences, notes, warnings, image estimates, or follow-up questions.
- Do not omit, reorder, or add Steps.
- Do not alter the verified starting values.
