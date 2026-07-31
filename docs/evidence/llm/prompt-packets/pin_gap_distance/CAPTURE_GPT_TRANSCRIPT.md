# Capture The Real GPT Transcript

After GPT returns XML, save the transcript evidence locally before changing it:

1. Record that these exact images were attached:
   - `C:\Git\OpenVisionLab_Dev\docs\samples\public\Line_Pins_Synthetic_OK.png`
   - `C:\Git\OpenVisionLab_Dev\docs\samples\public\Line_Pins_Synthetic_WidePin_NG.png`
2. Raw GPT prompt and response:
   - `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\raw\YYYYMMDD_pin_gap_gpt_round1.md`
3. If OpenVisionLab validation is NG, paste the validation report back to the same GPT task using `SEND_VALIDATION_NG_TO_GPT.md`.
4. Save the correction round:
   - `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\raw\YYYYMMDD_pin_gap_gpt_round2.md`
5. Only after removing private/customer data, copy a sanitized replay candidate to:
   - `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\sanitized\...`

Manual operator-repaired XML belongs in:

- `C:\Git\OpenVisionLab_Dev\artifacts\llm_transcripts\manual\...`

Manual replay is useful validation evidence, but it is not real GPT transcript evidence.
