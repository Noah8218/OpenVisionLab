# GPT Arithmetic Invert Test

This folder is a self-contained first-round GPT packet. No other OpenVisionLab document needs to be sent to GPT.

## Send To GPT

1. Start a new GPT conversation.
2. Attach these two images from this folder:
   - `Arithmetic_Invert_Synthetic_OK.png`
   - `Arithmetic_Invert_Synthetic_Bright_NG.png`
3. Paste the complete content of `COPY_THIS_TO_GPT.txt` in the same message.
4. Preserve GPT's first response unchanged and send it back to Codex for OpenVisionLab validation/import and explicit Good/NG replay.

The supplied prompt states the inspection intent in normal operator language. Its XML-only response requirement exists only for this import-validation test: OpenVisionLab needs one directly importable draft without manual cleanup.

## If Validation Or Execution Is NG

1. Obtain the complete unedited OpenVisionLab validation or run report.
2. Open the same GPT conversation that produced the first XML.
3. Paste `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` and replace its one placeholder with the complete report.
4. Preserve the complete corrected XML response unchanged and return it to Codex.

Do not use the correction template unless a real current-build validation, nominal run, or negative run actually fails. A correction transcript must record a real defect, not a fabricated failure.

## Scope

- Target workflow: `Arithmetic(Bitwise_NOT) -> Mean`.
- The full source image is inverted into `Arithmetic_Invert_Result`; Mean then judges that separate layer.
- The nominal dark input must pass the inverted brightness gate.
- The bright-input negative must fail the same inverted brightness gate.
- Preview and Run remain explicit OpenVisionLab actions. GPT does not operate the application, load images, create layers, or accept the recipe.

The two images are byte-identical copies of project-authored public synthetic assets. No user, customer, equipment, or private production asset is included.
