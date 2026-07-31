# GPT RotateScale Geometry Test

This folder is a self-contained first-round GPT packet. No other OpenVisionLab document needs to be sent to GPT.

## Send To GPT

1. Start a new GPT conversation.
2. Attach these two images from this folder:
   - `Geometry_RotateScale_Synthetic_OK.png`
   - `Geometry_RotateScale_Synthetic_Wide_NG.png`
3. Paste the complete content of `COPY_THIS_TO_GPT.txt` in the same message.
4. Preserve GPT's first response unchanged and send it back to Codex for OpenVisionLab validation/import and explicit Good/NG replay.

The supplied prompt describes the inspection in normal operator language. Its XML-only response requirement exists only for this import-validation test: OpenVisionLab needs one directly importable draft without manual cleanup.

## If Validation Or Execution Is NG

1. Obtain the complete unedited OpenVisionLab validation or run report.
2. Open the same GPT conversation that produced the first XML.
3. Paste `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` and replace its one placeholder with the complete report.
4. Preserve the complete corrected XML response unchanged and return it to Codex.

Do not use the correction template unless a real current-build validation, nominal run, or negative run actually fails. A correction transcript must record a real defect, not a fabricated failure.

## Scope

- Target workflow: one `RotateScale` Step that resizes the full source image to 50 percent without rotation.
- Nominal source size is 572x420 and must produce 286x210, passing `ResultImageWidth=286`.
- Wide-NG source size is 640x420 and must produce 320x210, failing the same width gate.
- The acceptance gate is about result image geometry, not target count, template matching, ROI, or physical calibration.
- Preview and Run remain explicit OpenVisionLab actions. GPT does not operate the application, load images, create layers, or accept the recipe.

The two images are byte-identical copies of project-authored public synthetic assets. No user, customer, equipment, or private production asset is included.
