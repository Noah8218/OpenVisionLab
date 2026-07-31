# HSV Color-Patch LLM Baseline Review

Updated: 2026-07-16 KST

## Decision

`HSV` is ready for one bounded manual GPT XML-authoring round. This is prompt and baseline readiness only; no external GPT response or correction loop exists yet.

## Verified Contract

- One `HSV` Step reads `Main` and writes a separate `HSV_Red_Mask` layer.
- `HueMin=170` and `HueMax=10` are valid OpenCV hue-wrap values. The range crosses the 179/0 boundary for red and must not be reordered.
- `SaturationMin=100`, `SaturationMax=255`, `ValueMin=100`, `ValueMax=255`, and `USE_ROI=false` are the bounded public-sample inputs.
- Acceptance uses `MaskPixelRatio` from `0.05` through `0.07`.
- `MaskPixelRatio` is the primary coverage gate; a later Blob or Contour Step is not part of this one-Step packet.

## Validation Repair

The HSV runtime already handled hue wrap by combining the high-hue and low-hue ranges, but the generic XML validator previously rejected `HueMin > HueMax`. The validator now keeps the 0..179 bounds while allowing the intentional circular order. The LLM tool catalog and XML authoring guide document the same behavior.

## Current-Build Evidence

1. The pre-change latest-EXE Recipe Manager draft validation rejected the `170 -> 10` XML only because of the generic min/max rule. Evidence: `artifacts/p44_hsv_llm_contract_before_20260716/direct_validation/report.txt`.
2. After the repair, the latest-EXE `llm-xml-draft-file` smoke validated and imported the same XML with one Step, zero errors, zero warnings, and `ImageRun: SKIPPED`. Evidence: `artifacts/p44_hsv_llm_contract_after_20260716/direct_validation/report.txt`.
3. The explicit nominal run passed with `MaskPixelCount=14000` and `MaskPixelRatio=0.058`.
4. The explicit missing-patch negative run produced the expected inspection NG with `MaskPixelCount=3500` and `MaskPixelRatio=0.015 < 0.05`.

The latest full solution build passed with zero warnings and zero errors before these replays.

## Public Packet

`docs/evidence/llm/prompt-packets/hsv_color_patch` contains only the first-round material:

- `README.md`
- `COPY_THIS_TO_GPT.txt`
- `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`
- `HSV_ColorPatch_Synthetic_OK.png`
- `HSV_ColorPatch_Synthetic_Missing_NG.png`

The two packet images are byte-identical copies of registered public synthetic assets:

- `HSV_ColorPatch_Synthetic_OK.png`: `FC12CB41A7B6D662BDDB91F89BE508702A4D2A23A3020CA64DCEE3E5A3C49FBD`
- `HSV_ColorPatch_Synthetic_Missing_NG.png`: `2DDC1DACEC1F6BB1109A4F53B4164B438DB99A762E1DA1A1C9421CA75F4C3854`

The user sends the two PNGs and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation. The first response must be preserved unchanged. Use the correction template only after a real current-build validation or nominal/negative run failure.

## Next Gate

The next evidence depends on a real manually transferred GPT response. Preserve its prompt and response under `artifacts/llm_transcripts/raw`, validate/import it with the latest build without automatic image execution, then run the nominal and negative images explicitly. Do not invent a failed response or correction round.
