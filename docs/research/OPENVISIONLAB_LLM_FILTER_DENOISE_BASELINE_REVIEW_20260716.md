# Filter Denoise LLM Baseline Review

Updated: 2026-07-16 KST

## Decision

`Filter(MedianBlur) -> Threshold -> Contour` is ready for one bounded manual GPT XML-authoring round. This is prompt and baseline readiness only; no external GPT response or correction loop exists yet.

## Verified Contract

- Step 1 reads `Main`, applies `FilterType=MedianBlur`, `MedianKernelSize=5`, and `BorderType=Reflect101`, and creates `Filter_Denoised`.
- Step 2 reads `Filter_Denoised`, uses `Threshold=130`, `MaxValue=255`, and `ThresholdType=Binary`, and creates `Filter_Denoise_Binary`.
- Step 3 reads `Filter_Denoise_Binary`, disables its internal threshold/ROI/draw-image options, and counts external `ApproxSimple` contours from 20 through 5000 pixels.
- Acceptance uses `ResultCount` exactly `4` through `4` on Step 3.
- The route is sequential: `Main -> Filter_Denoised -> Filter_Denoise_Binary -> Filter_Denoise_Preview`. There is no branch input and no output layer overwrites an input layer.

## Current-Build Evidence

1. The existing public baseline pipeline `docs/samples/public/Public_Filter_Denoise.pipeline.xml` validated/imported successfully with three Steps, zero errors, zero warnings, and no external dependencies. `ImageRun: SKIPPED` confirms import did not execute the inspection.
2. Explicit latest-EXE nominal execution passed at `ResultCount=4`, with Step channel transitions `3 -> 3 -> 1`, four overlays, and `29.848 ms` total elapsed time.
3. Explicit latest-EXE missing-target execution produced the expected product NG at `ResultCount=2 < 4`, with the same Step channel transitions, two overlays, and `27.179 ms` total elapsed time.
4. Both image commands used `--expect-run-success` and returned exit code 0 because the expected inspection result was declared explicitly.

The full solution build passed with zero warnings and zero errors before these actual-EXE replays. Current evidence is under `artifacts/p55_filter_denoise_baseline_current_exe_20260716`.

## Manual GPT Packet

`docs/evidence/llm/prompt-packets/filter_denoise` contains:

- `README.md`
- `COPY_THIS_TO_GPT.txt`
- `PASTE_VALIDATION_NG_BACK_TO_GPT.txt`
- `Filter_Denoise_Synthetic_OK.png`
- `Filter_Denoise_Synthetic_Missing_NG.png`

The two packet images are byte-identical copies of registered public synthetic assets:

- `Filter_Denoise_Synthetic_OK.png`: `2F415394EE5F38275790406FB38F58C93E7DCC2764BE806A1098A9C67FC333EA`.
- `Filter_Denoise_Synthetic_Missing_NG.png`: `514585BB54689B32F5F0FD4C5784418F20BC2C1B2E4CB394A3D2AE826A7672ED`.

The user sends both PNGs and the complete `COPY_THIS_TO_GPT.txt` content in one new GPT conversation. The first response must be preserved unchanged. Use the correction template only after a real current-build validation or nominal/negative run failure.

## Limits

- This is a tightly constrained three-Step authoring task, not a demonstration of independent inspection-design capability.
- The public synthetic pair does not prove robustness against lighting variation, blur, occlusion, or real production variation.
- No auto-run, layer mutation, tool-parameter UI change, equipment integration, or product-scope expansion is part of this packet.
