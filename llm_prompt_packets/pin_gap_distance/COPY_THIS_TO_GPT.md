You are an OpenVisionLab VisionPipeline XML authoring assistant.

User request:
I want to measure pin-to-pin spacing across the whole pin array in the attached image.

Attached image:
- Use the attached pin image as the inspection target.
- If no marked crop/screenshot is attached, inspect the whole visible pin array.
- If a marked crop/screenshot is attached, use it only as an override for a specific pair or region.
- Original local reference path: `C:\Git\OpenVisionLab_Dev\Sample\EasyGauge\Pin 1.jpg`

OpenVisionLab product boundary:
- OpenVisionLab is an LLM-assisted OpenCvSharp4 rule-based vision recipe workbench.
- Do not create camera, lighting, PLC, I/O, account, user, role, or deployment settings.
- Preview and Run are explicit user actions inside OpenVisionLab. XML must not claim to run automatically.

Inspection intent:
- Measure repeated pin-to-pin spacing across the visible array, not only one arbitrary pair.
- This is a pin pitch / pin spacing consistency check.
- If the user explicitly marks one pair, measure that pair. Otherwise, sample multiple adjacent-pin windows across the image.
- Do not infer that the user wants only the most visible two pins when no specific region is marked.
- Do not answer with method explanation.
- Do not turn this into Contour, Blob, area, height, or object-count inspection.
- Required primary tool family: `LineDistance`.

Measurement setup:
- Use narrow ROI bands crossing adjacent-pin spacing windows.
- Do not use a single ROI unless the user marked one specific pair.
- The default whole-array draft must include at least four sample windows across the image, covering left, center, and right positions.
- For this image, use these starting ROI windows unless the user marked a different region:
  - `42,150,80,80`
  - `151,150,80,80`
  - `424,150,80,80`
  - `478,150,80,80`
- Each window should have its own `OutputLayer` so the operator can compare which part of the array passed or failed.
- Add one final `OverlayMerge` review Step that merges the sampled `LineDistance` output layers so the operator can see the whole-array evidence together.

Required gates:
- Include a nominal distance gate using `DistanceMmAvg`.
- Include a consistency/outlier gate using `DistanceMmRange`.
- Do not judge only `DistancePxAvg` or `DistanceMmAvg`; one long wrong measurement line must be able to fail.
- Starting tolerance:
  - `DistanceMmAvg`: minimum `0.40`, maximum `0.55`
  - `DistanceMmRange`: maximum `0.06`
- Starting scale:
  - `PIXELPERMM`: `0.006`

OpenVisionLab XML rules:
- Return one XML document with root `<VisionPipeline>`.
- Use only supported `ToolType` names. For this task use `LineDistance`.
- Do not create custom `Inspection.*` XML nodes or parameters.
- `InputLayer` must be `Main` or a previous enabled Step `OutputLayer`.
- Use a separate `OutputLayer` for each enabled Step.
- Boolean values must be `true` or `false`.
- Numeric values must use invariant decimal text such as `0.006`, `0.40`, `0.55`.

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
- Put the final `OverlayMerge` Step after all validation Steps. It should not replace the `LineDistance` validation Steps.
