You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based HSV color-coverage inspection that selects red inspection patches across the full supplied image. The nominal image must pass and the missing-patch negative image must fail.

Attached files:
- Nominal image: `HSV_ColorPatch_Synthetic_OK.png`
- Negative image: `HSV_ColorPatch_Synthetic_Missing_NG.png`
- Both images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- Use the negative image only to understand the expected reject condition.

Inspection intent:
- Select the red patch color with one HSV range mask across the full image.
- The hue range crosses OpenCV's 179/0 boundary. It must use `HueMin=170` and `HueMax=10`; this is intentional wrap-around for red.
- The nominal image must pass because red-mask coverage is in the verified `MaskPixelRatio` range.
- The negative image must fail because most red patches are missing and its mask coverage is below the same range.
- Do not use Threshold, Blob, Contour, Matching, EdgeBasedMatching, FeatureMatching, LineDistance, Mean, or any unsupported custom tool.
- Required tool sequence: exactly one `HSV` Step.

Verified starting values:
- HueMin: `170`
- HueMax: `10`
- SaturationMin: `100`
- SaturationMax: `255`
- ValueMin: `100`
- ValueMax: `255`
- USE_ROI: `false`; inspect the full image
- Nominal `MaskPixelRatio` acceptance: `0.05` through `0.07`
- Current-build baseline: nominal `MaskPixelRatio=0.058`; negative `MaskPixelRatio=0.015`

Important parameter and metric rules:
- HSV Hue values are integers from 0 through 179 in OpenCV. `HueMin=170` greater than `HueMax=10` means a circular red range across the 179/0 boundary. Do not reorder those values.
- Saturation and Value values are integers from 0 through 255; their minimum values must not exceed their maximum values.
- `MaskPixelRatio` is selected-mask pixels divided by the full inspected image area because `USE_ROI=false`.
- The HSV output must be a separate mask layer. Do not change `InputLayer` to the output layer.
- The values above were verified with the current OpenVisionLab build. Do not estimate replacement values from the images.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, deployment, or automatic execution settings.
- Preview and Run are explicit user actions inside OpenVisionLab. The XML must not claim to run automatically.

OpenVisionLab XML contract:
- Return exactly one XML document with root `<VisionPipeline>`.
- Use `<Name>` and `<Steps>` directly under `<VisionPipeline>`.
- Set the pipeline `<Name>` to `HSV_ColorPatch_Inspection`.
- Every `<Step>` must contain `<Name>`, `<ToolType>`, `<Enabled>`, `<InputLayer>`, `<OutputLayer>`, and `<Parameters>`.
- Every parameter must use `<Parameter><Key>...</Key><Value>...</Value></Parameter>`.
- Boolean values must be lowercase `true` or `false`.
- Use invariant numeric text.
- Use only the supported `HSV` ToolType for this task.
- `InputLayer` must be `Main`.
- The enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Required Step:
- Name: `01 Red Color Coverage`
- ToolType: `HSV`
- Enabled: `true`
- InputLayer: `Main`
- OutputLayer: `HSV_Red_Mask`
- Parameters:
  - `Name=GPT_HSV_ColorPatch`
  - `HueMin=170`
  - `HueMax=10`
  - `SaturationMin=100`
  - `SaturationMax=255`
  - `ValueMin=100`
  - `ValueMax=255`
  - `USE_ROI=false`
- Acceptance fields must be direct children of the HSV `<Step>`, after `</Parameters>`:
  - `<UseAcceptance>true</UseAcceptance>`
  - `<ExpectedSuccess>true</ExpectedSuccess>`
  - `<MaxElapsedMilliseconds>500</MaxElapsedMilliseconds>`
  - `<AcceptanceMetricName>MaskPixelRatio</AcceptanceMetricName>`
  - `<UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>`
  - `<AcceptanceMetricMinimum>0.05</AcceptanceMetricMinimum>`
  - `<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>`
  - `<AcceptanceMetricMaximum>0.07</AcceptanceMetricMaximum>`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanations, analysis, tables, Markdown fences, notes, warnings, image estimates, or follow-up questions.
- Do not add a second Step.
- Do not alter the verified starting values.
