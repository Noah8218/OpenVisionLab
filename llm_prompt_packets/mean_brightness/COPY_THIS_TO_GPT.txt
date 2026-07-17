You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based mean-brightness inspection that checks the full supplied image. The nominal image must pass and the dark negative image must fail.

Attached files:
- Nominal image: `Mean_Brightness_Synthetic_OK.png`
- Negative image: `Mean_Brightness_Synthetic_Dark_NG.png`
- Both images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- Use the negative image only to understand the expected reject condition.

Inspection intent:
- Measure overall brightness across the entire image with one Mean step.
- The nominal image must pass because its verified `MeanValueAvg` is `201.5`.
- The dark negative image must fail because its verified `MeanValueAvg` is `117.5`, below the same acceptance range.
- Do not use Threshold, Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching, LineDistance, HSV, Filter, Morphology, Arithmetic, RotateScale, OverlayMerge, or any unsupported custom tool.
- Required tool sequence: exactly one `Mean` Step.

Verified starting values:
- `MEAN_TYPES=Mean`
- `USE_THRESHOLD=false`
- `USE_ADAPTIVE_THRESHOLD=false`
- `USE_BITWISENOT=false`
- `USE_ROI=false`; inspect the full image
- `USE_MULTI_ROI=false`
- Nominal `MeanValueAvg` acceptance: `185` through `220`
- Current-build baseline: nominal `MeanValueAvg=201.5`; negative `MeanValueAvg=117.5`

Important parameter and metric rules:
- `MeanValueAvg` is the measured average intensity for the selected Mean mode.
- Because `USE_ROI=false`, the judgement applies to the full 572 x 420 image.
- The Mean output must be a separate result layer. Do not change `InputLayer` to the output layer.
- The values above were verified with the current OpenVisionLab build. Do not estimate replacement values from the images.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, deployment, or automatic execution settings.
- Preview and Run are explicit user actions inside OpenVisionLab. The XML must not claim to run automatically.

OpenVisionLab XML contract:
- Return exactly one XML document with root `<VisionPipeline>`.
- Use `<Name>` and `<Steps>` directly under `<VisionPipeline>`.
- Set the pipeline `<Name>` to `Mean_Brightness_Inspection`.
- Every `<Step>` must contain `<Name>`, `<ToolType>`, `<Enabled>`, `<InputLayer>`, `<OutputLayer>`, and `<Parameters>`.
- Every parameter must use `<Parameter><Key>...</Key><Value>...</Value></Parameter>`.
- Boolean values must be lowercase `true` or `false`.
- Use invariant numeric text.
- Use only the supported `Mean` ToolType for this task.
- `InputLayer` must be `Main`.
- The enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Required Step:
- Name: `01 Mean Brightness Drift`
- ToolType: `Mean`
- Enabled: `true`
- InputLayer: `Main`
- OutputLayer: `Mean_Brightness_Preview`
- Parameters:
  - `Name=GPT_Mean_Brightness`
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
  - `<AcceptanceMetricMinimum>185</AcceptanceMetricMinimum>`
  - `<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>`
  - `<AcceptanceMetricMaximum>220</AcceptanceMetricMaximum>`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanations, analysis, tables, Markdown fences, notes, warnings, image estimates, or follow-up questions.
- Do not add a second Step.
- Do not alter the verified starting values.
