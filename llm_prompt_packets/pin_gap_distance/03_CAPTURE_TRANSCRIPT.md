# Capture The Real GPT Transcript

After GPT returns XML, save the transcript evidence locally before changing it:

1. Raw GPT prompt and response:
   - `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\raw\YYYYMMDD_pin_gap_gpt_round1.md`
2. If OpenVisionLab validation is NG, paste the validation report back to GPT using `02_PASTE_VALIDATION_NG_BACK_TO_GPT.md`.
3. Save the correction round:
   - `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\raw\YYYYMMDD_pin_gap_gpt_round2.md`
4. Only after removing private/customer data, copy a sanitized replay candidate to:
   - `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\sanitized\...`

Manual operator-repaired XML belongs in:

- `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\manual\...`

Manual replay is useful validation evidence, but it is not real GPT transcript evidence.

