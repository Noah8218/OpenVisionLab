You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based feature-matching inspection that finds the supplied feature-rich card in the nominal image. The nominal image must pass and the wrong-card negative image must fail.

Attached files:
- Nominal image: `Feature_Card_Synthetic_OK.png`
- Negative image: `Feature_Card_Synthetic_Wrong_NG.png`
- Template image: `Feature_Card_Synthetic_Template.png`
- The nominal and negative images are 572 x 420 OpenVisionLab project-authored synthetic samples.
- The template image contains the taught feature-rich card crop.
- Use the template image as the feature pattern. Use the negative image only to understand the expected reject condition.

Inspection intent:
- Find the supplied card from local feature points and geometric verification.
- Inspect the full image.
- Reject the wrong-card image by the final `ScoreMax` acceptance gate.
- The wrong-card image can still produce one weak geometric hypothesis, so `ResultCount=1` alone does not mean a valid match.
- Do not use intensity Matching, EdgeBasedMatching, Threshold, Blob, Contour, LineDistance, or any unsupported custom tool.
- Required tool sequence: exactly one `FeatureMatching` Step.

Verified starting values:
- TemplatePath: `..\..\docs\samples\public\templates\Feature_Card_Synthetic_Template.png`
- PATTERN_PATH: `..\..\docs\samples\public\templates\Feature_Card_Synthetic_Template.png`
- SCORE_MIN: `0.85`
- RANSAC_REPROJ_THRESHOLD: `4`
- USE_THRESHOLD: `false`
- USE_ADAPTIVE_THRESHOLD: `false`
- USE_ROI: `false`; inspect the full image
- Nominal `ScoreMax` acceptance: `80` through `100`

Important parameter and metric rules:
- `SCORE_MIN` is the Lowe descriptor-ratio input on the normalized 0 through 1 scale. Keep it as `0.85`; do not write `85`.
- Smaller `SCORE_MIN` values are stricter and larger values retain more descriptor candidates. Do not replace the verified value.
- `ScoreMax` is an OpenVisionLab result metric on the 0 through 100 scale. Its acceptance minimum must be `80`, not `0.80`.
- `RANSAC_REPROJ_THRESHOLD` is a positive pixel tolerance. Keep the verified value `4`.
- OpenVisionLab resolves relative dependency paths from the application startup directory. Both template path parameters must use the exact verified startup-relative path above. Do not invent an absolute path or use the attached-file display path.
- The values above were verified with the current OpenVisionLab build. Do not estimate replacement values from the images.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, deployment, or automatic execution settings.
- Preview and Run are explicit user actions inside OpenVisionLab. The XML must not claim to run automatically.

OpenVisionLab XML contract:
- Return exactly one XML document with root `<VisionPipeline>`.
- Use `<Name>` and `<Steps>` directly under `<VisionPipeline>`.
- Set the pipeline `<Name>` to `Feature_Card_Inspection`.
- Every `<Step>` must contain `<Name>`, `<ToolType>`, `<Enabled>`, `<InputLayer>`, `<OutputLayer>`, and `<Parameters>`.
- Every parameter must use `<Parameter><Key>...</Key><Value>...</Value></Parameter>`.
- Boolean values must be lowercase `true` or `false`.
- Use invariant numeric text.
- Use only the supported `FeatureMatching` ToolType for this task.
- `InputLayer` must be `Main`.
- The enabled Step must have a distinct non-empty `OutputLayer`.
- Do not emit custom `Inspection.*` elements, comments, result values, or invented parameters.

Required Step:
- Name: `01 Synthetic Feature Card Match`
- ToolType: `FeatureMatching`
- Enabled: `true`
- InputLayer: `Main`
- OutputLayer: `Feature_Preview`
- Parameters:
  - `Name=GPT_Feature_Card`
  - `TemplatePath=..\..\docs\samples\public\templates\Feature_Card_Synthetic_Template.png`
  - `PATTERN_PATH=..\..\docs\samples\public\templates\Feature_Card_Synthetic_Template.png`
  - `SCORE_MIN=0.85`
  - `RANSAC_REPROJ_THRESHOLD=4`
  - `USE_THRESHOLD=false`
  - `USE_ADAPTIVE_THRESHOLD=false`
  - `USE_ROI=false`
- Acceptance fields must be direct children of the FeatureMatching `<Step>`, after `</Parameters>`:
  - `<UseAcceptance>true</UseAcceptance>`
  - `<ExpectedSuccess>true</ExpectedSuccess>`
  - `<MaxElapsedMilliseconds>3000</MaxElapsedMilliseconds>`
  - `<AcceptanceMetricName>ScoreMax</AcceptanceMetricName>`
  - `<UseAcceptanceMetricMinimum>true</UseAcceptanceMetricMinimum>`
  - `<AcceptanceMetricMinimum>80</AcceptanceMetricMinimum>`
  - `<UseAcceptanceMetricMaximum>true</UseAcceptanceMetricMaximum>`
  - `<AcceptanceMetricMaximum>100</AcceptanceMetricMaximum>`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanations, analysis, tables, Markdown fences, notes, warnings, image estimates, or follow-up questions.
- Do not add a second Step.
- Do not alter the verified starting values.
