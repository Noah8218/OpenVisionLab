Return the OpenVisionLab recipe now.

Hard output contract:
- Output XML only.
- The first characters of your response must be `<?xml`.
- The last characters of your response must be `</VisionPipeline>`.
- Do not write explanation, analysis, tables, markdown fences, notes, warnings, or measurement estimates.
- If pitch vs gap is ambiguous, choose edge-to-edge gap and still return XML.
- If ROI is imperfect, choose the provided starting ROI and still return XML. The operator will tune ROI inside OpenVisionLab after validation.
- Use `ToolType=LineDistance`.
- Include both `DistanceMmAvg` and `DistanceMmRange` acceptance gates.
