Keep the original inspection intent unchanged: whole-array adjacent-pin edge-to-edge gap measurement using `LineDistance`.

The previous XML did not pass OpenVisionLab validation or did not satisfy the intent contract.

Hard output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanation, analysis, tables, markdown fences, notes, warnings, or measurement estimates.

Repair rules:
- Return only one corrected `VisionPipeline` XML document.
- Do not change the task into Contour, Blob, area, height, or object-count inspection.
- Use `ToolType=LineDistance` for the distance measurement.
- Keep both gates:
  - `DistanceMmAvg` minimum `0.14`, maximum `0.17`
  - `DistanceMmRange` maximum `0.02`
- Keep these four ROI windows: `108,170,65,120`, `204,170,65,120`, `300,170,65,120`, `396,170,65,120`.
- Keep `PIXELPERMM=0.006`.
- All LineDistance Steps read `Main`; add `ALLOW_BRANCH_INPUT=true` to every LineDistance Step after the first one.
- Do not invent layers. `InputLayer` must be `Main` or a previous enabled Step `OutputLayer`.
- Do not invent external dependency files.
- Do not emit `Inspection.*` XML nodes or parameters.
- Do not include camera, lighting, PLC, I/O, account, or deployment settings.

OpenVisionLab validation report:

```text
PASTE_VALIDATION_REPORT_HERE
```

Previous XML:

```xml
PASTE_PREVIOUS_XML_HERE
```
