# Pin Gap Distance GPT Packet

Use this packet to obtain one real GPT-authored OpenVisionLab XML draft for a whole-array pin-gap check.

## Send These Three Items

1. Attach the nominal image:
   - `C:\Git\OpenVisionLab_Dev\docs\samples\public\Line_Pins_Synthetic_OK.png`
2. Attach the negative reference image:
   - `C:\Git\OpenVisionLab_Dev\docs\samples\public\Line_Pins_Synthetic_WidePin_NG.png`
3. Open `COPY_THIS_TO_GPT.md`, copy the whole file, paste it as one message, and send it.

Do not send the other files in this folder on the first round. Do not add a final sentence such as "measure the pin gap" or "return XML only"; the copied prompt already contains both the operator intent and output contract.

GPT should return one `VisionPipeline` XML document only. Keep its response unchanged and bring it back to OpenVisionLab for validation.

## Validate In OpenVisionLab

1. Open Recipe Manager -> `LLM XML`.
2. Paste the unchanged GPT response.
3. Click `Validate`.
4. Do not import while validation is NG.
5. If validation is OK, import it and run the nominal and negative samples only through explicit Preview/Run actions.

## Correction Loop

If OpenVisionLab validation is NG:

1. Open `SEND_VALIDATION_NG_TO_GPT.md`.
2. Paste the OpenVisionLab validation report into the placeholder.
3. Ask GPT to return corrected XML only.

## Reference Files

The paste prompt already contains the essential rules. Do not upload these on the first round unless GPT cannot repair a validation error from the supplied report:

- `C:\Git\OpenVisionLab_Dev\docs\OPENVISIONLAB_LLM_XML_AUTHORING_GUIDE.md`
- `C:\Git\OpenVisionLab_Dev\docs\contracts\openvisionlab\OPENVISIONLAB_LLM_TOOL_CATALOG.json`

## Advanced Files

- `COPY_THIS_TO_GPT.md`: expanded authoring reference; not needed for the first GPT message.
- `FINAL_USER_MESSAGE_XML_ONLY.md`: final XML-only command, useful if GPT starts explaining instead of returning XML.
- `SEND_VALIDATION_NG_TO_GPT.md`: correction prompt after OpenVisionLab validation fails.
- `CAPTURE_GPT_TRANSCRIPT.md`: transcript capture notes.
