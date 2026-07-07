# Pin Gap Distance GPT Packet

Use this folder when asking GPT to draft OpenVisionLab XML for pin-array distance or spacing checks.

## What To Send

Preferred path inside OpenVisionLab:

1. Open Recipe Manager -> `LLM XML`.
2. Select `Pin gap / edge distance (LineDistance)`.
3. Set ROI samples, Min/Max, Range, and mm/px.
4. Click `Build prompt`, then `Copy prompt`.
5. Paste that copied prompt into GPT with the inspection image.

Use the files in this folder when you want an external/manual fallback packet:

1. Attach the inspection image:
   - `C:\Git\OpenVisionLab_Dev\Sample\EasyGauge\Pin 1.jpg`
2. If a specific pair or region is intended, attach a marked crop/screenshot. If not, the default request is the whole pin array.
3. Open `COPY_THIS_TO_GPT.md`, copy the whole file, and paste it into GPT.
4. GPT should return one `VisionPipeline` XML document only.
5. Paste the XML into OpenVisionLab Recipe Manager, run `Validate`, and do not import until validation is OK.

The user should not need to know prompt-order rules. `COPY_THIS_TO_GPT.md` already contains the natural-language whole-array request plus the XML-only output contract.

## Correction Loop

If OpenVisionLab validation is NG:

1. Open `02_PASTE_VALIDATION_NG_BACK_TO_GPT.md`.
2. Paste the OpenVisionLab validation report into the placeholder.
3. Ask GPT to return corrected XML only.

## Reference Files

The paste prompt already contains the essential rules for this case. Full references remain here if you want to upload them too:

- `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`
- `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_LLM_TOOL_CATALOG.json`
- `C:\Git\OpenVisionLab_Dev\docs\samples\public\Public_Line_Pins_Distance.pipeline.xml`

## Advanced Files

- `01_PASTE_TO_GPT_PIN_GAP.md`: expanded authoring prompt.
- `00_FINAL_USER_MESSAGE_XML_ONLY.md`: final XML-only command, useful if GPT starts explaining instead of returning XML.
- `02_PASTE_VALIDATION_NG_BACK_TO_GPT.md`: correction prompt after OpenVisionLab validation fails.
- `03_CAPTURE_TRANSCRIPT.md`: transcript capture notes.
