# Mean Brightness LLM Baseline Review

Updated: 2026-07-16 KST

## Decision

`Mean` is ready for one bounded manual GPT XML-authoring round. This is prompt and baseline readiness only; no external GPT response or correction loop exists yet.

## Verified Contract

- One `Mean` Step reads `Main` and writes a separate `Mean_Brightness_Preview` layer.
- The full image is inspected with `MEAN_TYPES=Mean`, `USE_ROI=false`, and internal threshold/adaptive/invert/multi-ROI options disabled.
- Acceptance uses `MeanValueAvg` from `185` through `220`.
- A later Threshold, Blob, or Contour Step is not part of this one-Step packet.

## Current-Build Evidence

1. The existing public baseline pipeline `docs/samples/public/Public_Mean_BrightnessDrift.pipeline.xml` validated successfully with zero errors and warnings.
2. Explicit latest-EXE nominal execution passed with `MeanValueAvg=201.5`, `ResultCount=1`, source channels 3, and `5.802 ms`.
3. Explicit latest-EXE dark negative execution produced the expected product NG with `MeanValueAvg=117.5 < 185`, `ResultCount=1`, source channels 3, and `5.924 ms`.
4. Both baseline commands used `--expect-run-success` and returned exit code 0 because the expected inspection result was declared explicitly.

The latest full solution build passed with zero warnings and zero errors before these replays.

## Public Packet

`docs/evidence/llm/prompt-packets/mean_brightness` contains only the first-round material:

- `README.md`
- `COPY_THIS_TO_GPT.txt`
- `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`
- `Mean_Brightness_Synthetic_OK.png`
- `Mean_Brightness_Synthetic_Dark_NG.png`

The two packet images are byte-identical copies of registered public synthetic assets:

- `Mean_Brightness_Synthetic_OK.png`: `4C8EB56BA73703719865AA62446944DC4DC991410BC7DDB746133577A4ED4FE9`
- `Mean_Brightness_Synthetic_Dark_NG.png`: `28DF874ABE9B4CD8B0E5460210499CD167A00C69E455D470DA960FCCA88BD4D9`

The user sends the two PNGs and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation. The first response must be preserved unchanged. Use the correction template only after a real current-build validation or nominal/negative run failure.

## Next Gate

The next evidence depends on a real manually transferred GPT response. Preserve its prompt and response under `artifacts/llm_transcripts/raw`, validate/import it with the latest build without automatic image execution, then run the nominal and negative images explicitly. Do not invent a failed response or correction round.
