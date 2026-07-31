You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based edge-geometry inspection that finds the supplied asymmetric L-shaped fiducial once in the nominal image. The nominal image must pass and the wrong-shape negative image must fail.

Attached files:
- Nominal image: `Edge_Fiducial_Synthetic_OK.png`
- Negative image: `Edge_Fiducial_Synthetic_Wrong_NG.png`
- Template image: `Edge_Fiducial_Synthetic_Template.png`
- The nominal and negative images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- The template image is 112 x 112 and contains the taught asymmetric L fiducial.
- Use the template image as the edge-matching pattern. Use the negative image only to understand the expected reject condition.

Inspection intent:
- Find one appearance of the supplied asymmetric L fiducial by edge geometry.
- Do not treat the background border, crossing lines, horizontal guide lines, circle, or black rectangle as the target.
- Reject the negative image because its central white shape is a T, not the taught L fiducial.
- Do not use intensity `Matching`, FeatureMatching, Threshold, Blob, Contour, LineDistance, or any unsupported custom tool.
- Required tool sequence: exactly one `EdgeBasedMatching` Step.

Verified starting values:
- TemplatePath: `..\..\docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`
- PATTERN_PATH: `..\..\docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`
- SCORE_MIN: `0.70`
- NUM_MATCH: `1`
- CANNY_LOW: `30`
- CANNY_HIGH: `90`
- CANNY_APERTURE_SIZE: `3`
- USE_L2_GRADIENT: `true`
- CONTOUR_RETRIEVAL_MODE: `External`
- CONTOUR_APPROXIMATION_MODE: `ApproxNone`
- GREEDINESS: `0.90`
- SEARCH_STEP: `1`
- MAX_TEMPLATE_POINTS: `260`
- MIN_GRADIENT_MAGNITUDE: `1`
- USE_DRAW_IMAGE: `true`
- USE_FIND_ANGLE: `false`
- USE_POSITION_REFINE: `true`
- USE_HYBRID_VERIFY: `false`
- USE_THRESHOLD: `false`
- USE_ADAPTIVE_THRESHOLD: `false`
- USE_ROI: `false`; inspect the full image
- Nominal `ScoreMax` acceptance: `70` through `100`

Important parameter and metric rules:
- `SCORE_MIN` is an input parameter on the normalized 0 through 1 scale. Keep it as `0.70`; do not write `70`.
- `ScoreMax` is an OpenVisionLab result metric on the 0 through 100 scale. Its acceptance minimum must be `70`, not `0.70`.
- `GREEDINESS` is also a normalized 0 through 1 input parameter. Keep it as `0.90`.
- `CANNY_LOW` must not exceed `CANNY_HIGH`, and `CANNY_APERTURE_SIZE` must remain the supported odd value `3`.
- OpenVisionLab resolves relative dependency paths from the application startup directory. Both template path parameters must use the exact verified startup-relative path above. Do not invent an absolute path or use the attached-file display path.
- The values above were verified with the current OpenVisionLab build. Do not estimate replacement values from the images.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, deployment, or automatic execution settings.
- Preview and Run are explicit user actions inside OpenVisionLab. The XML must not claim to run automatically.

OpenVisionLab XML contract:
- Return exactly one XML document with root `<VisionPipeline>`.
- Use `<Name>` and `<Steps>` directly under `<VisionPipeline>`.
- Set the pipeline `<Name>` to `Edge_Fiducial_Inspection`.
- Every `<Step>` must contain `<Name>`, `<ToolType>`, `<Enabled>`, `<InputLayer>`, `<OutputLayer>`, and `<Parameters>`.
- Every parameter must use `<Parameter><Key>...</Key><Value>...</Value></Parameter>`.
- Boolean values must be lowercase `true` or `false`.
- Use invariant numeric text.
- Use only the supported `EdgeBasedMatching` ToolType for this task.
- `InputLayer` must be `Main`.
- The enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Required Step:
- Name: `01 Synthetic Edge Fiducial Match`
- ToolType: `EdgeBasedMatching`
- Enabled: `true`
- InputLayer: `Main`
- OutputLayer: `EdgeBased_Preview`
- Parameters:
  - `Name=GPT_Edge_Fiducial`
  - `TemplatePath=..\..\docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`
  - `PATTERN_PATH=..\..\docs\samples\public\templates\Edge_Fiducial_Synthetic_Template.png`
  - `SCORE_MIN=0.70`
  - `NUM_MATCH=1`
  - `CANNY_LOW=30`
  - `CANNY_HIGH=90`
  - `CANNY_APERTURE_SIZE=3`
  - `USE_L2_GRADIENT=true`
  - `CONTOUR_RETRIEVAL_MODE=External`
  - `CONTOUR_APPROXIMATION_MODE=ApproxNone`
  - `GREEDINESS=0.90`
  - `SEARCH_STEP=1`
  - `MAX_TEMPLATE_POINTS=260`
  - `MIN_GRADIENT_MAGNITUDE=1`
  - `USE_DRAW_IMAGE=true`
  - `USE_FIND_ANGLE=false`
  - `USE_POSITION_REFINE=true`
  - `USE_HYBRID_VERIFY=false`
  - `USE_THRESHOLD=false`
  - `USE_ADAPTIVE_THRESHOLD=false`
  - `USE_ROI=false`
- Acceptance fields must be direct children of the EdgeBasedMatching `<Step>`, after `</Parameters>`:
  - `<UseAcceptance>true</UseAcceptance>`
  - `<ExpectedSuccess>true</ExpectedSuccess>`
  - `<MaxElapsedMilliseconds>3000</MaxElapsedMilliseconds>`
  - `<AcceptanceMetricName>ScoreMax</AcceptanceMetricName>`
  - `<UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>`
  - `<AcceptanceMetricMinimum>70</AcceptanceMetricMinimum>`
  - `<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>`
  - `<AcceptanceMetricMaximum>100</AcceptanceMetricMaximum>`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanations, analysis, tables, Markdown fences, notes, warnings, image estimates, or follow-up questions.
- Do not add a second Step.
- Do not alter the verified starting values.
