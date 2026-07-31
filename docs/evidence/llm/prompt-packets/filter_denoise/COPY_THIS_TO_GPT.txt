You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based inspection that removes small salt-like bright noise, then counts the four large bright rectangular targets. The nominal image must pass and the missing-target negative image must fail.

Attached images:
- Nominal image: `Filter_Denoise_Synthetic_OK.png`
- Negative image: `Filter_Denoise_Synthetic_Missing_NG.png`
- Both images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- Use the nominal image to author the pipeline. Use the negative image only to understand the expected reject condition.

Inspection intent:
- Apply a median denoise before brightness segmentation so isolated bright noise does not become target contours.
- Convert the filtered image to a binary layer.
- Count only the remaining large target contours.
- Do not use Blob, Matching, LineDistance, FeatureMatching, EdgeBasedMatching, Morphology, or unsupported custom tools.
- Required tool sequence: `Filter`, then `Threshold`, then `Contour`.
- This is a sequential pipeline, not a branch. Each Step must read the previous Step's output layer.

Verified starting values:
- Filter type: `MedianBlur`
- Median kernel size: `5`
- Border type: `Reflect101`
- Threshold mode: `Threshold`
- Threshold: `130`
- Threshold max value: `255`
- Threshold type: `Binary`
- Contour internal threshold, adaptive threshold, bitwise invert, ROI, multi-ROI, and draw-image options: disabled because Contour reads the binary layer.
- Contour approximation: `ApproxSimple`
- Contour detect mode: `External`
- Contour minimum area: `20` pixels
- Contour maximum area: `5000` pixels
- Nominal ResultCount acceptance: minimum `4`, maximum `4`
- The missing-target negative image contains only two valid targets after denoise and must fail the same ResultCount gate.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, deployment, or automatic execution settings.
- Preview and Run are explicit user actions inside OpenVisionLab. The XML must not claim to run automatically.

OpenVisionLab XML contract:
- Return exactly one XML document with root `<VisionPipeline>`.
- Use `<Name>` and `<Steps>` directly under `<VisionPipeline>`.
- Every `<Step>` must contain `<Name>`, `<ToolType>`, `<Enabled>`, `<InputLayer>`, `<OutputLayer>`, and `<Parameters>`.
- Every parameter must use `<Parameter><Key>...</Key><Value>...</Value></Parameter>`.
- Boolean values must be lowercase `true` or `false`.
- Use invariant numeric text.
- Use only supported `ToolType` names: `Filter`, `Threshold`, and `Contour` for this task.
- `InputLayer` must be `Main` or the `OutputLayer` of an earlier enabled Step.
- Each enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Step 1 requirements:
- Name: `01 Filter Median Denoise`
- ToolType: `Filter`
- InputLayer: `Main`
- OutputLayer: `Filter_Denoised`
- Parameters, exactly:
  - `FilterType=MedianBlur`
  - `MedianKernelSize=5`
  - `BorderType=Reflect101`
- Do not add an acceptance gate to this Step.

Step 2 requirements:
- Name: `02 Filter Denoise Binary`
- ToolType: `Threshold`
- InputLayer: `Filter_Denoised`
- OutputLayer: `Filter_Denoise_Binary`
- Parameters, exactly:
  - `Mode=Threshold`
  - `Threshold=130`
  - `MaxValue=255`
  - `ThresholdType=Binary`
- Do not add an acceptance gate to this Step.

Step 3 requirements:
- Name: `03 Filter Denoise Target Count`
- ToolType: `Contour`
- InputLayer: `Filter_Denoise_Binary`
- OutputLayer: `Filter_Denoise_Preview`
- Parameters, exactly:
  - `Name=GPT_Filter_Denoise`
  - `PIXELPERMM=0.006`
  - `USE_THRESHOLD=false`
  - `USE_ADAPTIVE_THRESHOLD=false`
  - `USE_BITWISENOT=false`
  - `USE_ROI=false`
  - `USE_MULTI_ROI=false`
  - `USE_DRAW_IMAGE=false`
  - `ApproximationModes=ApproxSimple`
  - `DetectMode=External`
  - `MIN_AREA=20`
  - `MAX_AREA=5000`
  - `ClrGridHtml=#00ff00`
  - `DrawThickness=2`
- Acceptance fields must be direct children of the Contour `<Step>`, after `</Parameters>`:
  - `<UseAcceptance>true</UseAcceptance>`
  - `<ExpectedSuccess>true</ExpectedSuccess>`
  - `<MaxElapsedMilliseconds>1000</MaxElapsedMilliseconds>`
  - `<AcceptanceMetricName>ResultCount</AcceptanceMetricName>`
  - `<UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>`
  - `<AcceptanceMetricMinimum>4</AcceptanceMetricMinimum>`
  - `<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>`
  - `<AcceptanceMetricMaximum>4</AcceptanceMetricMaximum>`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanations, analysis, tables, Markdown fences, notes, warnings, image estimates, or follow-up questions.
- Do not omit, reorder, or add Steps.
- Do not alter the verified starting values.
