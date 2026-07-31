# GPT Feature Card Test

This folder is a self-contained manual GPT test packet. Do not attach the full OpenVisionLab authoring guide or tool catalog.

## First GPT request

1. Open a new GPT conversation.
2. Attach these three files together:
   - `Feature_Card_Synthetic_OK.png`
   - `Feature_Card_Synthetic_Wrong_NG.png`
   - `Feature_Card_Synthetic_Template.png`
3. Paste the complete contents of `COPY_THIS_TO_GPT.txt` once.
4. Return GPT's complete response to Codex unchanged.

Do not overwrite `COPY_THIS_TO_GPT.txt` with GPT's response. Do not repair prose, Markdown fences, paths, parameter names, or invalid XML before returning it. The unchanged first response is the evidence.

## Only after an actual OpenVisionLab failure

Use `PASTE_VALIDATION_NG_BACK_TO_GPT.txt` only when Codex supplies a real current-build validation or run report. Insert that complete report into its placeholder, then send the correction request in the same GPT conversation.

Do not request a correction when the first XML already passes validation, nominal execution, and expected negative rejection.
