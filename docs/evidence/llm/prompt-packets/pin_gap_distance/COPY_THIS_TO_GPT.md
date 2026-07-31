You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
Create a rule-based inspection that measures the edge-to-edge gap between adjacent pins across the whole visible pin array. The nominal image must pass and the wide-pin negative image must fail.

Attached images:
- Nominal target: `Line_Pins_Synthetic_OK.png`
- Negative reference: `Line_Pins_Synthetic_WidePin_NG.png`
- Both images are 572 x 420 project-authored synthetic samples.
- Use the nominal image to define the XML. Use the negative image only to understand the expected reject condition.

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, or deployment settings.
- Preview and Run are explicit user actions inside OpenVisionLab. XML must not claim to run automatically.

Inspection intent:
- Measure the clear edge-to-edge gap between adjacent bright pins, not pin width, package clearance, center pitch, area, height, or object count.
- Inspect four adjacent-pin windows from left to right so one local defect cannot be hidden by a whole-image average.
- Do not answer with method explanation.
- Do not turn this into Contour, Blob, area, height, or object-count inspection.
- Required primary tool family: `LineDistance`.

Measurement setup:
- Use exactly these four verified ROI windows on the nominal 572 x 420 image:
  - `108,170,65,120`
  - `204,170,65,120`
  - `300,170,65,120`
  - `396,170,65,120`
- Each ROI begins inside one bright pin and ends inside the next bright pin so `WTOB` projections select the two inner edges.
- Each window should have its own `OutputLayer` so the operator can compare which part of the array passed or failed.
- Add one final `OverlayMerge` review Step that merges the sampled `LineDistance` output layers so the operator can see the whole-array evidence together.

Required gates:
- Include a nominal distance gate using `DistanceMmAvg`.
- Include a consistency/outlier gate using `DistanceMmRange`.
- Do not judge only `DistancePxAvg` or `DistanceMmAvg`; one long wrong measurement line must be able to fail.
- Verified starting tolerance for the nominal image:
  - `DistanceMmAvg`: minimum `0.14`, maximum `0.17`
  - `DistanceMmRange`: maximum `0.02`
- Starting scale:
  - `PIXELPERMM`: `0.006`

OpenVisionLab XML rules:
- Return one XML document with root `<VisionPipeline>`.
- Use only supported `ToolType` names. For this task use `LineDistance`.
- Do not create custom `Inspection.*` XML nodes or parameters.
- `InputLayer` must be `Main` or a previous enabled Step `OutputLayer`.
- Use a separate `OutputLayer` for each enabled Step.
- All LineDistance Steps read `Main`. Add `ALLOW_BRANCH_INPUT=true` to every LineDistance Step after the first one.
- Boolean values must be `true` or `false`.
- Numeric values must use invariant decimal text such as `0.006`, `0.14`, `0.17`.

Required LineDistance parameters to include:
- `Name`
- `PIXELPERMM`
- `USE_THRESHOLD`
- `USE_ADAPTIVE_THRESHOLD`
- `USE_BITWISENOT`
- `USE_ROI`
- `CvROI`
- `LeftPRJ_DIR`
- `RightPRJ_DIR`
- `PRJ_PORALITY`
- `CONTRAST`
- `THICKNESS`
- `SAMPLING_STEP`
- `POINT_RANGE`
- `VER_PRJ_DIR`
- `USE_MANUAL_ANGLE`
- `MANUAL_ANGLE_VALUE`
- `SHOW_EDGE`
- `SHOW_VERTICAL_LINE`

Use these starting LineDistance values:
- `USE_THRESHOLD=false`
- `USE_ADAPTIVE_THRESHOLD=false`
- `USE_BITWISENOT=false`
- `USE_ROI=true`
- `LeftPRJ_DIR=X_LTOR`
- `RightPRJ_DIR=X_RTOL`
- `PRJ_PORALITY=WTOB`
- `CONTRAST=18`
- `THICKNESS=2`
- `SAMPLING_STEP=16`
- `POINT_RANGE=8`
- `VER_PRJ_DIR=X_RTOL`
- `USE_MANUAL_ANGLE=true`
- `MANUAL_ANGLE_VALUE=89`
- `SHOW_EDGE=true`
- `SHOW_VERTICAL_LINE=true`

Output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanation, analysis, tables, markdown fences, notes, warnings, or measurement estimates.
- Include both `DistanceMmAvg` and `DistanceMmRange` acceptance gates.
- For every ROI window, include both `DistanceMmAvg` and `DistanceMmRange` acceptance gates.
- If one Step can judge only one metric, duplicate the same `LineDistance` parameters into a paired validation Step with a separate `OutputLayer`.
- Set `UseAcceptance=true`, `ExpectedSuccess=true`, and `MaxElapsedMilliseconds=500` on every LineDistance validation Step.
- Put the final `OverlayMerge` Step after all validation Steps. It must read `Main`, use a new output layer, merge the four Range output layers through `SourceLayers`, and set `BurnIn=true`, `DrawLabels=true`, and `AllowEmpty=false`.
- Do not add acceptance gates to the final `OverlayMerge` Step.
