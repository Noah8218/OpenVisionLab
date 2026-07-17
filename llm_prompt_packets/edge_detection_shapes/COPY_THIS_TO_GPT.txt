You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based inspection that finds the edges of four bright rectangular shapes, closes the small edge gaps, then counts the joined shapes. The nominal image must pass and the missing-shape negative image must fail.

Attached images:
- Nominal image: `EdgeDetection_Shapes_Synthetic_OK.png`
- Negative image: `EdgeDetection_Shapes_Synthetic_Missing_NG.png`
- Both images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- Use the nominal image to author the pipeline. Use the negative image only to understand the expected reject condition.

Inspection intent:
- Use Canny edge detection to make a binary edge layer from the original color image.
- Use morphology Close to join the small gaps around each rectangular edge.
- Count only the joined rectangular contours inside the declared shape band ROI.
- Do not use Threshold, Blob, Matching, LineDistance, FeatureMatching, EdgeBasedMatching, or unsupported custom tools.
- Required tool sequence: `EdgeDetection`, then `Morphology`, then `Contour`.
- This is a sequential pipeline, not a branch. Each Step must read the previous Step's output layer.
- Required layer route: `Main -> EdgeDetection_Edge -> EdgeDetection_EdgeJoin -> EdgeDetection_Shape_Preview`.

Verified starting values:
- Edge type: `Canny`
- Canny low threshold: `40`
- Canny high threshold: `120`
- Canny aperture size: `3`
- L2 gradient: `true`
- Morphology shape: `Rect`
- Morphology operator: `Close`
- Morphology kernel width: `3`
- Morphology kernel height: `3`
- Morphology iterations: `1`
- Contour ROI: `90,100,410,95`
- Contour internal threshold, adaptive threshold, bitwise invert, multi-ROI, and draw-image options: disabled because Contour reads the joined edge layer.
- Contour approximation: `ApproxSimple`
- Contour detect mode: `External`
- Contour minimum area: `500` pixels
- Contour maximum area: `5000` pixels
- Nominal ResultCount acceptance: minimum `4`, maximum `4`
- The missing-shape negative image contains only two valid joined contours and must fail the same ResultCount gate.

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
- Use only supported `ToolType` names: `EdgeDetection`, `Morphology`, and `Contour` for this task.
- `InputLayer` must be `Main` or the `OutputLayer` of an earlier enabled Step.
- Each enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Step 1 requirements:
- Name: `01 Shape Canny Edge`
- ToolType: `EdgeDetection`
- InputLayer: `Main`
- OutputLayer: `EdgeDetection_Edge`
- Parameters, exactly:
  - `EdgeType=Canny`
  - `CannyThresholdLow=40`
  - `CannyThresholdHigh=120`
  - `CannyApertureSize=3`
  - `UseL2Gradient=true`
- Do not add an acceptance gate to this Step.

Step 2 requirements:
- Name: `02 Edge Join`
- ToolType: `Morphology`
- InputLayer: `EdgeDetection_Edge`
- OutputLayer: `EdgeDetection_EdgeJoin`
- Parameters, exactly:
  - `Shape=Rect`
  - `Operator=Close`
  - `KernelWidth=3`
  - `KernelHeight=3`
  - `Iterations=1`
- Do not add an acceptance gate to this Step.

Step 3 requirements:
- Name: `03 Edge Shape Count`
- ToolType: `Contour`
- InputLayer: `EdgeDetection_EdgeJoin`
- OutputLayer: `EdgeDetection_Shape_Preview`
- Parameters, exactly:
  - `Name=GPT_EdgeDetection_Shapes`
  - `PIXELPERMM=0.006`
  - `USE_THRESHOLD=false`
  - `USE_ADAPTIVE_THRESHOLD=false`
  - `USE_BITWISENOT=false`
  - `USE_ROI=true`
  - `CvROI=90,100,410,95`
  - `USE_MULTI_ROI=false`
  - `USE_DRAW_IMAGE=false`
  - `ApproximationModes=ApproxSimple`
  - `DetectMode=External`
  - `MIN_AREA=500`
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
