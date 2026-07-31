# GPT Filter Denoise Test

This folder is a self-contained first-round GPT packet. No other OpenVisionLab document needs to be sent to GPT.

## Send To GPT

1. Start a new GPT conversation.
2. Attach these two images from this folder:
   - `Filter_Denoise_Synthetic_OK.png`
   - `Filter_Denoise_Synthetic_Missing_NG.png`
3. Paste the complete content of `COPY_THIS_TO_GPT.txt` in the same message.
4. Preserve GPT's first response unchanged and send it back to Codex for OpenVisionLab validation/import and explicit Good/NG replay.

The initial request contains normal operator wording. The response must start with `<?xml`, end with `</VisionPipeline>`, and contain XML only so OpenVisionLab can validate it without manual cleanup.

## If Validation Or Execution Is NG

1. Obtain the complete unedited OpenVisionLab validation or run report.
2. Open the same GPT conversation that produced the first XML.
3. Paste `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` and replace its one placeholder with the complete report.
4. Preserve the complete corrected XML response unchanged and return it to Codex.

Do not use the correction template unless a real current-build validation, nominal run, or negative run actually fails. A correction transcript must record a real defect, not a fabricated failure.

## Scope

- Target workflow: `Filter(MedianBlur) -> Threshold -> Contour`.
- The Filter output becomes the Threshold input; the Threshold output becomes the Contour input.
- The Filter removes isolated salt-like noise before the final target count.
- Nominal image must pass exactly four contours; the missing-target image must fail the same count gate.
- Preview and Run remain explicit OpenVisionLab actions. GPT does not operate the application, load images, create layers, or accept the recipe.

The two images are byte-identical copies of project-authored public synthetic assets. No user, customer, equipment, or private production asset is included.
